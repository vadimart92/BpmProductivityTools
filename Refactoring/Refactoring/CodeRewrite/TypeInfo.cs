using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public class TypeInfo : BaseSyntaxOperationProvider {
		public TypeInfo() {
			Regions = new List<Region>();
		}
		public readonly Dictionary<string, int> RegionNameCounters = new Dictionary<string, int>();
		private TypeDeclarationSyntax _syntax;
		public List<Region> Regions { get; set; }
		public List<BaseMemberInfo> Members { get; private set; }
		public TypeDeclarationSyntax Syntax {
			get { return _syntax; }
			set {
				_syntax = value;
				Analyze();
			}
		}

		public override TypeDeclarationSyntax ParentType {
			get { return base.ParentType; }
			set {
				base.ParentType = value;
				foreach (var item in Regions.Cast<BaseSyntaxOperationProvider>().Concat(Members)) {
					item.ParentType = value;
				}
			}
		}

		private void Analyze() {
			Name = Syntax.Identifier.Text;
			ParseRegions();
			ParceMethods();
			var nodesToTrack = GetNodesToTrack();
			_syntax = _syntax.TrackNodes(nodesToTrack);
		}

		public override IEnumerable<SyntaxNode> GetNodesToTrack() {
			return Members.Select(m => m.GetNode())
					.Concat(Regions.SelectMany(r=>r.GetNodesToTrack()))
					.ToList();
		}

		private void ParceMethods() {
			Members = Syntax.ChildNodes()
				.OfType<MemberDeclarationSyntax>()
				.Select(BaseMemberInfo.Create).ToList();
		}

		private void ParseRegions() {
			Regions.Clear();
			Regions.AddRange(RegionParser.ParceRegions(Syntax, this));
		}

		public void AddRegion(Region region) {
			Regions.Add(region);
		}

		private void CheckRegions() {
			foreach (var member in Members) {
				var actualRegion = FindActualRegion(member);
				var destinationRegion = Regions.FirstOrDefault(r => member.IsNeedToBeInRegion(r));
				if (destinationRegion == null) {
					destinationRegion = Region.Create(member.Type, member.Access);
					AddRegion(destinationRegion);
				}
				if (actualRegion != destinationRegion) {
					member.Move(destinationRegion, actualRegion);
				}
			}
		}

		private Region FindActualRegion(BaseMemberInfo member) {
			var actualRegion =
				Regions.Where(member.IsInRegion)
					.OrderByDescending(r => r.Type == member.Type && r.Access == member.Access)
					.ThenByDescending(r => r.Span.Length)
					.Take(1).FirstOrDefault();
			return actualRegion;
		}
		
		public override List<IChangeApplier> CreateChangeAppliers() {
			CheckRegions();
			var appliers = Regions.SelectMany(region => region.CreateChangeAppliers()).ToList();
			var memberAppliers = Members.SelectMany(member => member.CreateChangeAppliers()).ToList();
			var changes = appliers.Concat(memberAppliers).ToList();
			return changes;
		}

		protected override string GetNameSuffix(string name) {
			return "Class";
		}
	}
}

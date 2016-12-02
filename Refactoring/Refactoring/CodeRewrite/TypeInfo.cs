using System.Collections.Generic;
using System.Linq;
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

		private void Analyze() {
			Name = Syntax.Identifier.Text;
			ParseRegions();
			ParceMethods();
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
				var actualRegion =
					Regions.Where(r => member.IsInRegion(r))
						.OrderByDescending(r => r.Type == member.Type && r.Access == member.Access)
						.ThenByDescending(r => r.Span.Length)
						.Take(1).FirstOrDefault();
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

		public override List<ChangeApplier> CreateChangeAppliers() {
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

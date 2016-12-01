using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public class TypeInfo {
		public TypeInfo() {
			Regions = new List<Region>();
		}
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
			Regions.AddRange(RegionSyntaxWalker.ParceRegions(Syntax));
		}

		public void CheckRegions() {
			throw new System.NotImplementedException();
		}

	}
}

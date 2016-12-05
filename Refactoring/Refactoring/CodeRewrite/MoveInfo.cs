using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public class MoveInfo : IChangeApplier {
		
		public Region FromRegion {
			get; set;
		}
		public Region ToRegion {
			get; set;
		}

		public BaseMemberInfo Member { get; set; }

		public TypeDeclarationSyntax ApplyChanges(TypeDeclarationSyntax type, TypeInfo typeInfo) {
			var method = Member.FindSyntaxNode(type);
			type = type.RemoveNode(method, SyntaxRemoveOptions.KeepLeadingTrivia);
			typeInfo.ParentType = type;
			method = method.WithoutTrivia();
			var toRegionStart = ToRegion.Start;
			var methodInRegion = type.Members.First(m => ContainsRegion(m, toRegionStart));
			type = type.InsertNodesAfter(methodInRegion, new[] {method});
			typeInfo.ParentType = type;
			return type;
		}

		private static bool ContainsRegion(SyntaxNode m, RegionDirectiveTriviaSyntax toRegionStart) {
			var regions = m.GetLeadingTrivia().Where(t=>t.IsKind(SyntaxKind.RegionDirectiveTrivia)).ToList();
			return regions.Any(t=>t.Span == toRegionStart.Span);
		}
	}

}
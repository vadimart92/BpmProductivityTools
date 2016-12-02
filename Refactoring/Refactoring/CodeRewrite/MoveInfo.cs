using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public class MoveInfo : ChangeApplier {
		
		public Region FromRegion {
			get; set;
		}
		public Region ToRegion {
			get; set;
		}

		public override TypeDeclarationSyntax ApplyChanges(TypeDeclarationSyntax type, TypeInfo typeInfo) {
			var method = typeInfo.Members.Find(m => m.NodeId.Equals(NodeId, StringComparison.OrdinalIgnoreCase));
			var newType = type.RemoveNode(method.GetNode(), SyntaxRemoveOptions.KeepLeadingTrivia);
			typeInfo = new TypeInfo {Syntax = newType};
			var toRegion = typeInfo.Regions.Find(r => r.NodeId.Equals(ToRegion.NodeId, StringComparison.OrdinalIgnoreCase));
			var methodInregion = type.DescendantNodes().First(m => m.Contains(toRegion.Start));
			newType = newType.InsertNodesAfter(methodInregion, new[] {method.GetNode()});
			return newType;
		}
	}
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {

	public class ToRegionRewriter: CSharpSyntaxRewriter {
		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) {
			var classDeclaration = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);
			var typeInfo = new TypeInfo {
				Syntax = classDeclaration
			};
			typeInfo.CheckRegions();
			return typeInfo.Syntax;
		}
	}
}

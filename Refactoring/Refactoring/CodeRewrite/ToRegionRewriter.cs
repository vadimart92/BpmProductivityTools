using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {

	public class ToRegionRewriter: CSharpSyntaxRewriter {
		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) {
			var classDeclaration = (TypeDeclarationSyntax)base.VisitClassDeclaration(node);
			var typeInfo = new TypeInfo {Syntax = classDeclaration};
			var changes = typeInfo.CreateChangeAppliers();
			classDeclaration = typeInfo.Syntax;
			foreach (var change in changes) {
				classDeclaration = change.ApplyChanges(classDeclaration, typeInfo);
			}
			return classDeclaration;
		}
	}
}

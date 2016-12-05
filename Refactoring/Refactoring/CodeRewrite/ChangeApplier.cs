using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public interface IChangeApplier {
		TypeDeclarationSyntax ApplyChanges(TypeDeclarationSyntax type, TypeInfo typeInfo);
	}
}
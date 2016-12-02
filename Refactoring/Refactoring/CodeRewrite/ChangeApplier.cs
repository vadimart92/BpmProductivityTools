using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public abstract class ChangeApplier {
		public string NodeId {
			get; set;
		}
		public abstract TypeDeclarationSyntax ApplyChanges(TypeDeclarationSyntax type, TypeInfo typeInfo);
	}
}
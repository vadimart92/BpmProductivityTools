using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public abstract class BaseSyntaxOperationProvider {
		public abstract List<IChangeApplier> CreateChangeAppliers();

		public virtual string Name { get; protected set; }
		protected abstract string GetNameSuffix(string name);
		public abstract IEnumerable<SyntaxNode> GetNodesToTrack();
		public virtual TypeDeclarationSyntax ParentType { get; set; }
	}
}
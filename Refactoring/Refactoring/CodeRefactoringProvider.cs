using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace Refactoring {
	[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(RefactoringCodeRefactoringProvider)), Shared]
	internal class RefactoringCodeRefactoringProvider : CodeRefactoringProvider {

		public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context) {
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var node = root.FindNode(context.Span);
			var typeDecl = node as TypeDeclarationSyntax;
			if (typeDecl == null) {
				return;
			}
			var action = CodeAction.Create("Reverse type name", c => RefactoringHelper.ReverseTypeNameAsync(context.Document, typeDecl, c));
			context.RegisterRefactoring(action);
			action = CodeAction.Create("Wrap in region", token => RefactoringHelper.WrapToRegionInDoc(context.Document, node, token));
		}
	}
}
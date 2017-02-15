using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Refactoring.CodeRewrite;

namespace Refactoring {
	public static class RefactoringHelper {
		public static async Task<Solution> ReverseTypeNameAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken) {
			// Produce a reversed version of the type declaration's identifier token.
			var identifierToken = typeDecl.Identifier;
			var newName = new string(identifierToken.Text.ToCharArray().Reverse().ToArray());

			// Get the symbol representing the type to be renamed.
			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			var typeSymbol = semanticModel.GetDeclaredSymbol(typeDecl, cancellationToken);

			// Produce a new solution that has all references to that type renamed, including the declaration.
			var originalSolution = document.Project.Solution;
			var optionSet = originalSolution.Workspace.Options;
			var newSolution = await Renamer.RenameSymbolAsync(document.Project.Solution, typeSymbol, newName, optionSet, cancellationToken).ConfigureAwait(false);

			// Return the new solution with the now-uppercase type name.
			return newSolution;
		}

		public static async Task<Document> WrapToRegionInDoc(Document document, SyntaxNode node, CancellationToken cancellationToken) {
			var originalSolution = document.Project.Solution;
			var optionSet = originalSolution.Workspace.Options;
			var parent = node.Parent;
			var root = await document.GetSyntaxRootAsync(cancellationToken);
			var newRoot = NormalizeRegions(root, node);
			return document.WithSyntaxRoot(root);
		}

		public static SyntaxNode NormalizeRegions(SyntaxNode syntaxRoot, SyntaxNode currentNode) {
			var rewriter = new ToRegionRewriter();
			var newNode = rewriter.Visit(currentNode);
			var result = syntaxRoot.ReplaceNode(currentNode, newNode);
			var lineEndingsRewriter = new LineEndingsRewriter();
			result = lineEndingsRewriter.Visit(result);
			return result;
		}
	}
}
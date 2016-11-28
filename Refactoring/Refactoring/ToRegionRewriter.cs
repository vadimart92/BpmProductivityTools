using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ServiceStack;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Refactoring {
	public class ToRegionRewriter: CSharpSyntaxRewriter {
		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node) {
			var classDeclaration = (ClassDeclarationSyntax)base.VisitClassDeclaration(node) ;
			string name = classDeclaration.Identifier.Text;
			string typeName = "Class";
			var result = WrapToRegion(classDeclaration, name, typeName);
			return result;
		}

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
			var methodDeclaration = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);
			var accessModifiers = new List<SyntaxKind> {
				SyntaxKind.PublicKeyword ,
				SyntaxKind.PrivateKeyword,
				SyntaxKind.ProtectedKeyword,
				SyntaxKind.InternalKeyword
			};
			SyntaxToken token = methodDeclaration.Modifiers.FirstOrDefault(m=> accessModifiers.Any(a=>m.IsKind(a)));
			string name = token.Text.ToPascalCase();
			string typeName = "Methods";
			var result = WrapToRegion(methodDeclaration, name, typeName);
			return result;
		}

		private static SyntaxNode WrapToRegion(SyntaxNode c, string name, string typeName) {
			IEnumerable<SyntaxTrivia> leadingTrivia = c.GetLeadingTrivia().ToSyntaxTriviaList();
			IEnumerable<SyntaxTrivia> spaces = leadingTrivia.Where(t => t.Kind() == SyntaxKind.WhitespaceTrivia);
			var result = c.WithLeadingTrivia(
				Merge(
					CarriageReturnLineFeed,
					CarriageReturnLineFeed,
					spaces,
					CreateRegionStart(name, typeName),
					CarriageReturnLineFeed,
					leadingTrivia
				)
			);
			result = result.WithTrailingTrivia(CreateRegionEnd(spaces));
			return result;
		}

		private static IEnumerable<SyntaxTrivia> Merge(params object[] trivias) {
			foreach (object trivia in trivias) {
				var enumerable = trivia as IEnumerable<SyntaxTrivia>;
				if (enumerable != null) {
					foreach (var syntaxTrivia in enumerable) {
						yield return syntaxTrivia;
					}
				} else {
					yield return (SyntaxTrivia) trivia;
				}
			}
		}

		private static SyntaxTrivia CreateRegionStart(string regionName, string name) {
			return Trivia(
				RegionDirectiveTrivia(true)
					.WithEndOfDirectiveToken(
						Token(TriviaList(Space, PreprocessingMessage($"{name}: {regionName}")),
							SyntaxKind.EndOfDirectiveToken,
							TriviaList(CarriageReturnLineFeed)))
			);
		}
		private static SyntaxTrivia CreateRegionEnd(IEnumerable<SyntaxTrivia> spaces) {
			return Trivia(
				EndRegionDirectiveTrivia(true)
					.WithLeadingTrivia(Merge(CarriageReturnLineFeed, CarriageReturnLineFeed, spaces))
					.WithTrailingTrivia(CarriageReturnLineFeed, CarriageReturnLineFeed)
			);
		}
	}
}

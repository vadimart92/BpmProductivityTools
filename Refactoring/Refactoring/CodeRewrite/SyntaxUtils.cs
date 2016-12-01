using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Refactoring.CodeRewrite {
	public static class SyntaxUtils {

		public static SyntaxNode WrapToRegion(SyntaxNode c, string name, string typeName) {
			IEnumerable<SyntaxTrivia> leadingTrivia = c.GetLeadingTrivia().ToSyntaxTriviaList();
			IEnumerable<SyntaxTrivia> spaces = leadingTrivia.Where(t => CSharpExtensions.Kind((SyntaxTrivia) t) == SyntaxKind.WhitespaceTrivia);
			var result = c.WithLeadingTrivia(
				Merge(
					SyntaxFactory.CarriageReturnLineFeed,
					SyntaxFactory.CarriageReturnLineFeed,
					spaces,
					CreateRegionStart(name, typeName),
					SyntaxFactory.CarriageReturnLineFeed,
					leadingTrivia
				)
			);
			result = result.WithTrailingTrivia(CreateRegionEnd(spaces));
			return result;
		}

		public static IEnumerable<SyntaxTrivia> Merge(params object[] trivias) {
			foreach (object trivia in trivias) {
				var enumerable = trivia as IEnumerable<SyntaxTrivia>;
				if (enumerable != null) {
					foreach (var syntaxTrivia in enumerable) {
						yield return syntaxTrivia;
					}
				} else {
					yield return (SyntaxTrivia)trivia;
				}
			}
		}

		public static SyntaxTrivia CreateRegionStart(string regionName, string name) {
			return SyntaxFactory.Trivia(
				SyntaxFactory.RegionDirectiveTrivia(true)
					.WithEndOfDirectiveToken(
						SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.Space, SyntaxFactory.PreprocessingMessage($"{name}: {regionName}")),
							SyntaxKind.EndOfDirectiveToken,
							SyntaxFactory.TriviaList(SyntaxFactory.CarriageReturnLineFeed)))
			);
		}

		public static SyntaxTrivia CreateRegionEnd(IEnumerable<SyntaxTrivia> spaces) {
			return SyntaxFactory.Trivia(
				SyntaxFactory.EndRegionDirectiveTrivia(true)
					.WithLeadingTrivia(Merge(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed, spaces))
					.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed)
			);
		}
	}
}
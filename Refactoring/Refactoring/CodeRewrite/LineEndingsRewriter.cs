using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite
{

	public class LineEndingsRewriter : CSharpSyntaxRewriter {
		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node) {
			//TODO
			var preTrivia = new List<SyntaxTrivia>();
			var postTrivia = new List<SyntaxTrivia>();
			var currentCollection = preTrivia;
			bool newLineFound = false;
			bool isNormalized = false;
			var endOfLine = SyntaxFactory.EndOfLine(Environment.NewLine);
			foreach (var tr in node.GetLeadingTrivia().Reverse()) {
				currentCollection.Add(tr);
				if (!isNormalized) {
					var isNewLine = tr.Kind() == SyntaxKind.EndOfLineTrivia;
					if (newLineFound) {
						isNormalized = true;
						if (!isNewLine) {
							currentCollection.Add(endOfLine);
						}
					} else {
						if (isNewLine) {
							newLineFound = true;
							currentCollection = postTrivia;
						}
					}
				}
			}
			if (!postTrivia.Any()) {
				postTrivia.Add(endOfLine);
			}
			var trivias = preTrivia.Concat(postTrivia).Reverse().ToSyntaxTriviaList();
			node = node.WithoutLeadingTrivia().WithLeadingTrivia(trivias);
			node = (MethodDeclarationSyntax)base.VisitMethodDeclaration(node);
			return node;
		}
	}
}

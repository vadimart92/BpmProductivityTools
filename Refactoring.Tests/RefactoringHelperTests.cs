using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace Refactoring.Tests
{
	[TestFixture]
	public class RefactoringHelperTests {

		[Test]
		public void WrapToRegion() {
			string typeName = "TestClass1";
			string text = TestUtils.ReadSrcFile(typeName);
			var tree = CSharpSyntaxTree.ParseText(text);
			
			SyntaxNode root = tree.GetRoot();
			var doc = RefactoringHelper.NormalizeRegions(root, root.FindNode(new TextSpan(text.IndexOf(typeName), typeName.Length)));
			var res = doc.ToFullString();
		}

	}
}

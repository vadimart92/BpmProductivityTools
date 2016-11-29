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
			string text = @"
    public class Sample
    {
		public Sample(){}
		private const int _someConst = 1;
		private int _someField = 1;
		private virtual string SomeProp {get;set;}
       public void SomeMethod() {}
    }";
			var tree = CSharpSyntaxTree.ParseText(text);
			string typeName = "Sample";
			SyntaxNode root = tree.GetRoot();
			var doc = RefactoringHelper.WrapToRegion(root, root.FindNode(new TextSpan(text.IndexOf(typeName), typeName.Length)));
			var res = doc.ToFullString();
		}

	}
}

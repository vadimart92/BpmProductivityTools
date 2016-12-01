namespace Refactoring.Tests.Examples.Expected {

	using System.IO;

	class RegionTestClass1 {
		void PrivateMethod() {
			var x = File.ReadAllText("src");
		}
	}
}

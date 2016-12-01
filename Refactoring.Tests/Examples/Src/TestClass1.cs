namespace Refactoring.Tests.Examples.Src {

	using System.IO;

	class RegionTestClass1 {
		#region TestRegion

		#region  	   Methods: Private      

		static void PrivateMethodInRegion() {

		}

		static void SecondPrivateMethodInRegion() {

		}

		#endregion

		#region SomeEmptyRegion


		#endregion

		#endregion
		#region other
		void PrivateMethod() {
			var x = File.ReadAllText("src");
		}
		#endregion
	}
}

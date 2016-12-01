using System;
using System.IO;

namespace Refactoring.Tests {

	public class FileExtension {
		private string _string;

		private FileExtension(string s) {
			_string = s;
		}
		public override string ToString() {
			return _string;
		}

		public static FileExtension CScharp = new FileExtension(".cs");
	}

	public static class TestUtils {
		private static readonly string BaseDir = new DirectoryInfo(new Uri(typeof(TestUtils).Assembly.CodeBase).LocalPath).Parent?.Parent?.Parent?.FullName;
		public static string GetSrcFileName(string file, FileExtension extension = null) {
			return Path.Combine(BaseDir, "Examples\\Src", file + (extension ?? FileExtension.CScharp));
		}

		public static string ReadSrcFile(string fileName, FileExtension extension = null) {
			string filePath = GetSrcFileName(fileName, extension);
			return File.ReadAllText(filePath);
		}
	}
}

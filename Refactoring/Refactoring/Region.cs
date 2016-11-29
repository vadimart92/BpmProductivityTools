using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring {
	public class Region {
		public string Name { get; set; }
		public MemberType Type { get; set; }
		public MemberAccess Access { get; set; }
		public List<SyntaxNode> SyntaxNodes { get; set; }
		public static Region Create(string name, RegionDirectiveTriviaSyntax start, EndRegionDirectiveTriviaSyntax end) {
			name = name.Trim();
			MemberAccess access;
			MemberType type;
			// todo create region
			throw new NotImplementedException();
		}
	}
}
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Refactoring.CodeRewrite;

namespace Refactoring {
	public static class SyntaxUtils {
		public static bool Contains(this SyntaxTokenList modifiers, SyntaxKind kind) {
			return modifiers.Any() && modifiers.FirstOrDefault(m => m.IsKind(kind)) != default(SyntaxToken);
		}

		public static MemberAccess GetAccess(this SyntaxTokenList modifiers) {
			if (modifiers.Contains(SyntaxKind.PublicKeyword)) {
				return MemberAccess.Public;
			}
			if (modifiers.Contains(SyntaxKind.ProtectedKeyword) && modifiers.Contains(SyntaxKind.InternalKeyword)) {
				return MemberAccess.ProtectedInternal;
			}
			if (modifiers.Contains(SyntaxKind.ProtectedKeyword)) {
				return MemberAccess.Protected;
			}
			if (modifiers.Contains(SyntaxKind.InternalKeyword)) {
				return MemberAccess.Internal;
			}
			if (modifiers.Contains(SyntaxKind.PrivateKeyword)) {
				return MemberAccess.Private;
			}
			return MemberAccess.Private;
		}
	}
}
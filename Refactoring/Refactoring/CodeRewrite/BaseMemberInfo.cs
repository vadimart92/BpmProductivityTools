using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	[DebuggerDisplay("{Type}: {Name}")]
	public abstract class BaseMemberInfo {
		protected MemberDeclarationSyntax Node;

		protected BaseMemberInfo(MemberDeclarationSyntax node) {
			Node = node;
		}
		protected abstract SyntaxTokenList Modifiers { get; }
		protected abstract SyntaxToken Identifier { get; }
		
		protected abstract MemberType Type { get; }
		protected virtual string Name => Identifier.Text;
		protected virtual MemberAccess Access => Modifiers.GetAccess();
		protected virtual bool Static => Modifiers.Contains(SyntaxKind.StaticKeyword);
		protected virtual bool Abstract => Modifiers.Contains(SyntaxKind.AbstractKeyword);
		protected virtual bool Override => Modifiers.Contains(SyntaxKind.OverrideKeyword);
		protected virtual bool New => Modifiers.Contains(SyntaxKind.NewKeyword);
		protected virtual bool Virtual => Modifiers.Contains(SyntaxKind.VirtualKeyword);
		
		public static BaseMemberInfo Create(MemberDeclarationSyntax value) {
			var methodDeclarationSyntax = value as MethodDeclarationSyntax;
			if (methodDeclarationSyntax != null) {
				return new MethodInfo(methodDeclarationSyntax);
			}
			var propertyDeclarationSyntax = value as PropertyDeclarationSyntax;
			if (propertyDeclarationSyntax != null) {
				return new PropertyInfo(propertyDeclarationSyntax);
			}
			var constructorDeclarationSyntax = value as ConstructorDeclarationSyntax;
			if (constructorDeclarationSyntax != null) {
				return new ConstructorInfo(constructorDeclarationSyntax);
			}
			var fieldDeclarationSyntax = value as FieldDeclarationSyntax;
			if (fieldDeclarationSyntax != null) {
				var field = fieldDeclarationSyntax;
				var isConst = field.Modifiers.Contains(SyntaxKind.ConstKeyword);
				return isConst ? new ConstantInfo(field) : new FieldInfo(field);
			}
			var eventFieldDeclarationSyntax = value as EventFieldDeclarationSyntax;
			if (eventFieldDeclarationSyntax != null) {
				return new EventInfo(eventFieldDeclarationSyntax);
			}
			return new EmptyMemberInfo(value);
		}

	}
}
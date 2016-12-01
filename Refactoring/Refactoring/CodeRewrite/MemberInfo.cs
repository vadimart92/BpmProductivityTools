using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public abstract class MemberInfo<TSyntax> : BaseMemberInfo where TSyntax: MemberDeclarationSyntax {
		protected MemberInfo(TSyntax syntax): base(syntax) {}
		protected TSyntax Syntax => (TSyntax) Node;
	}

	public class EmptyMemberInfo : BaseMemberInfo {
		public EmptyMemberInfo(MemberDeclarationSyntax node) : base(node) { }
		protected override string Name => string.Empty;
		protected override SyntaxToken Identifier => SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.WhitespaceTrivia, String.Empty, String.Empty, SyntaxTriviaList.Empty);
		protected override MemberType Type => MemberType.Undefined;
		protected override SyntaxTokenList Modifiers => new SyntaxTokenList();
	}

	public class MethodInfo : MemberInfo<MethodDeclarationSyntax> {
		public MethodInfo(MethodDeclarationSyntax syntax) : base(syntax) {}
		protected override SyntaxToken Identifier => Syntax.Identifier;
		protected override MemberType Type => MemberType.Method;
		protected override SyntaxTokenList Modifiers => Syntax.Modifiers;
	}

	public class PropertyInfo : MemberInfo<PropertyDeclarationSyntax> {
		public PropertyInfo(PropertyDeclarationSyntax syntax) : base(syntax) {}
		protected override SyntaxToken Identifier => Syntax.Identifier;
		protected override MemberType Type => MemberType.Property;
		protected override SyntaxTokenList Modifiers => Syntax.Modifiers;
	}
	public class ConstructorInfo : MemberInfo<ConstructorDeclarationSyntax> {
		public ConstructorInfo(ConstructorDeclarationSyntax syntax) : base(syntax) {}
		protected override SyntaxToken Identifier => Syntax.Identifier;
		protected override MemberType Type => MemberType.Constructor;
		protected override SyntaxTokenList Modifiers => Syntax.Modifiers;
	}

	public abstract class BaseFieldInfo<TSyntax> : MemberInfo<TSyntax> where TSyntax : BaseFieldDeclarationSyntax {
		protected override SyntaxToken Identifier => Syntax.Declaration.Variables.First().Identifier;
		protected BaseFieldInfo(TSyntax syntax) : base(syntax) {}
		protected override SyntaxTokenList Modifiers => Syntax.Modifiers;
	}

	public class FieldInfo : BaseFieldInfo<FieldDeclarationSyntax> {
		public FieldInfo(FieldDeclarationSyntax syntax) : base(syntax) {}
		protected override MemberType Type => MemberType.Field;
	}

	public class ConstantInfo : FieldInfo {
		public ConstantInfo(FieldDeclarationSyntax syntax) : base(syntax) {}
		protected override MemberType Type => MemberType.Constant;
	}

	public class EventInfo : BaseFieldInfo<EventFieldDeclarationSyntax> {
		public EventInfo(EventFieldDeclarationSyntax syntax) : base(syntax) { }
		protected override MemberType Type => MemberType.Event;
	}

	/*

Enum
Delegate

Interface
Struct
Class

*Constant
-Field
-Constructor
-Property
Event
-Method
	 */
}
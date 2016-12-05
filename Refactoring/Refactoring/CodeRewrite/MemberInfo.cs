using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public abstract class MemberInfo<TSyntax> : BaseMemberInfo where TSyntax: MemberDeclarationSyntax {
		protected MemberInfo(TSyntax syntax) : base(syntax) {}
		protected TSyntax Syntax => (TSyntax) Node;
		protected override string GetNameSuffix(string name) {
			return $"{Type}_{Access}";
		}
		public TSyntax FindTypedNode(TypeDeclarationSyntax type) {
			return type.GetCurrentNode(Syntax);
		}
		public override IEnumerable<SyntaxNode> GetNodesToTrack() {
			return new[] {Node};
		}
	}

	public class EmptyMemberInfo : BaseMemberInfo {
		public EmptyMemberInfo(MemberDeclarationSyntax node) : base(node) { }
		public override string Name => string.Empty;
		protected override SyntaxToken Identifier => SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.WhitespaceTrivia, String.Empty, String.Empty, SyntaxTriviaList.Empty);
		public override MemberType Type => MemberType.Undefined;
		protected override SyntaxTokenList Modifiers => new SyntaxTokenList();
		protected override string GetNameSuffix(string name) {
			return string.Empty;
		}

		public override IEnumerable<SyntaxNode> GetNodesToTrack() {
			return new []{ Node };
		}
	}

	public abstract class BaseMethodInfo<TSyntax> : MemberInfo<TSyntax> where TSyntax : BaseMethodDeclarationSyntax {
		private readonly int _parametersHash;
		protected BaseMethodInfo(TSyntax syntax) : base(syntax) {
			_parametersHash = Syntax.ParameterList.NormalizeWhitespace(String.Empty, String.Empty).ToFullString().GetHashCode();
		}
		public override MemberType Type => MemberType.Method;
		protected override SyntaxTokenList Modifiers => Syntax.Modifiers;
		protected override string GetNameSuffix(string name) {
			return base.GetNameSuffix(name) + "_" + _parametersHash;
		}
	}

	public abstract class BaseFieldInfo<TSyntax> : MemberInfo<TSyntax> where TSyntax : BaseFieldDeclarationSyntax {
		protected override SyntaxToken Identifier => Syntax.Declaration.Variables.First().Identifier;
		protected BaseFieldInfo(TSyntax syntax) : base(syntax) { }
		protected override SyntaxTokenList Modifiers => Syntax.Modifiers;
	}

	public class PropertyInfo : MemberInfo<PropertyDeclarationSyntax> {
		public PropertyInfo(PropertyDeclarationSyntax syntax) : base(syntax) { }
		protected override SyntaxToken Identifier => Syntax.Identifier;
		public override MemberType Type => MemberType.Property;
		protected override SyntaxTokenList Modifiers => Syntax.Modifiers;
	}

	public class MethodInfo : BaseMethodInfo<MethodDeclarationSyntax> {
		public MethodInfo(MethodDeclarationSyntax syntax) : base(syntax) { }
		protected override SyntaxToken Identifier => Syntax.Identifier;
	}

	public class ConstructorInfo : BaseMethodInfo<ConstructorDeclarationSyntax> {
		public ConstructorInfo(ConstructorDeclarationSyntax syntax) : base(syntax) {}
		protected override SyntaxToken Identifier => Syntax.Identifier;
	}

	public class FieldInfo : BaseFieldInfo<FieldDeclarationSyntax> {
		public FieldInfo(FieldDeclarationSyntax syntax) : base(syntax) {}
		public override MemberType Type => MemberType.Field;
	}

	public class ConstantInfo : FieldInfo {
		public ConstantInfo(FieldDeclarationSyntax syntax) : base(syntax) {}
		public override MemberType Type => MemberType.Constant;
	}

	public class EventInfo : BaseFieldInfo<EventFieldDeclarationSyntax> {
		public EventInfo(EventFieldDeclarationSyntax syntax) : base(syntax) { }
		public override MemberType Type => MemberType.Event;
	}

}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	[DebuggerDisplay("{Type}: {Name}")]
	public abstract class BaseMemberInfo : BaseSyntaxOperationProvider {
		protected MemberDeclarationSyntax Node;

		protected BaseMemberInfo(MemberDeclarationSyntax node) {
			Node = node;
		}
		protected abstract SyntaxTokenList Modifiers { get; }
		protected abstract SyntaxToken Identifier { get; }
		
		public abstract MemberType Type { get; }
		public override string Name => Identifier.Text;
		public virtual MemberAccess Access => Modifiers.GetAccess();
		public virtual bool Static => Modifiers.Contains(SyntaxKind.StaticKeyword);
		public virtual bool Abstract => Modifiers.Contains(SyntaxKind.AbstractKeyword);
		public virtual bool Override => Modifiers.Contains(SyntaxKind.OverrideKeyword);
		public virtual bool New => Modifiers.Contains(SyntaxKind.NewKeyword);
		public virtual bool Virtual => Modifiers.Contains(SyntaxKind.VirtualKeyword);

		public SyntaxNode GetNode() {
			return Node;
		}
		
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

		public bool IsInRegion(Region region) {
			return region.Span.Start < Node.SpanStart && Node.Span.End < region.Span.End;
		}

		public bool IsNeedToBeInRegion(Region region) {
			return region.IsValidMemberRegion && region.Type == Type && region.Access == Access;
		}

		private MoveInfo _moveInfo;
		public void Move(Region to, Region from = null) {
			_moveInfo = new MoveInfo {FromRegion = from, ToRegion = to, Member = this};
		}

		public override List<IChangeApplier> CreateChangeAppliers() {
			var result = new List<IChangeApplier>();
			if (_moveInfo != null) {
				result.Add(_moveInfo);
			}
			return result;
		}

		public SyntaxNode FindSyntaxNode(TypeDeclarationSyntax type) {
			return type.GetCurrentNode(Node);
		}
	}
}
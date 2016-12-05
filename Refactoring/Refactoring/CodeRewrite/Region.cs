using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Refactoring.CodeRewrite {

	public class TypeRegion : Region {
		public string TypeName { get; private set; }

		protected override void TrySetAccess(string access) {
			Access = MemberAccess.Undefined;
			TypeName = access;
		}
		public override bool IsValidMemberRegion => false;

		public TypeRegion(TypeDeclarationSyntax typeDeclarationSyntax) : base(typeDeclarationSyntax) {
		}
	}

	public class Region : BaseSyntaxOperationProvider {
		protected Region(TypeDeclarationSyntax typeDeclarationSyntax) {
			ParentType = typeDeclarationSyntax;
		}

		protected override string GetNameSuffix(string name) {
			int counter;
			if (!_typeInfo.RegionNameCounters.TryGetValue(name, out counter)) {
				counter = 0;
			}
			counter++;
			_typeInfo.RegionNameCounters[name] = counter;
			return counter > 1 ? counter.ToString() : string.Empty;
		}

		public override IEnumerable<SyntaxNode> GetNodesToTrack() {
			return new List<SyntaxNode> {_start, _end};
		}

		private static readonly Regex NameRegex = new Regex("(?<Type>\\w+)(\\s*):(\\s*)(?<Access>\\w+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		private bool _isNew;
		private TypeInfo _typeInfo;
		private RegionDirectiveTriviaSyntax _start;
		private EndRegionDirectiveTriviaSyntax _end;

		public static Region Create(RegionDirectiveTriviaSyntax start, EndRegionDirectiveTriviaSyntax end, TypeInfo typeInfo) {
			Region result = new Region(typeInfo.Syntax);
			var message =
				start.EndOfDirectiveToken.GetAllTrivia().FirstOrDefault(t => t.IsKind(SyntaxKind.PreprocessingMessageTrivia));
			if (message != default(SyntaxTrivia)) {
				var name = message.ToFullString().Trim();
				var match = NameRegex.Match(name);
				if (match.Success) {
					string typeName = match.Groups["Type"].Value.Trim();
					var type = GetTypeByName(typeName);
					bool isType = type == MemberType.Class || type == MemberType.Interface || type == MemberType.Enum;
					if (isType) {
						result = new TypeRegion(typeInfo.Syntax);
					}
					result.Type = type;
					string access = match.Groups["Access"].Value.Trim();
					result.TrySetAccess(access);
				}
				result.Name = name;
			}
			result.Start = start;
			result.End = end;
			result._typeInfo = typeInfo;
			return result;
		}

		public static Region Create(MemberType type, MemberAccess access) {
			return new Region(null) {
				Type = type,
				Access = access,
				_isNew = true
			};
			
		}
		
		protected virtual void TrySetAccess(string access) {
			Access = GetMemberAccessByName(access);
		}
		
		private static MemberType GetTypeByName(string name) {
			if (TypeNameMap.ContainsValue(name)) {
				var mapItem = TypeNameMap.First(m => m.Value.Equals(name, StringComparison.OrdinalIgnoreCase));
				return mapItem.Key;
			}
			return MemberType.Undefined;
		}
		private static string GetTypeName(MemberType type) {
			return TypeNameMap[type];
		}

		private static MemberAccess GetMemberAccessByName(string name) {
			if (AccessMap.ContainsValue(name)) {
				var mapItem = AccessMap.First(m => m.Value.Equals(name, StringComparison.OrdinalIgnoreCase));
				return mapItem.Key;
			}
			return MemberAccess.Undefined;
		}

		private static string GetMemberAccessName(MemberAccess access) {
			return AccessMap[access];
		}

		private static readonly Dictionary<MemberType, string> TypeNameMap = new Dictionary<MemberType, string> {
			{ MemberType.Method, "Methods" },
			{ MemberType.Constant, "Constants" },
			{ MemberType.Constructor, "Constructors" },
			{ MemberType.Event, "Events" },
			{ MemberType.Field, "Fields" },
			{ MemberType.Property, "Properties" },
			{ MemberType.Class, "Class" },
			{ MemberType.Interface, "Interface" },
			{ MemberType.Enum, "Enum" },
			{MemberType.Undefined, "<Undefined>" }
		};

		private static readonly  Dictionary<MemberAccess, string> AccessMap = new Dictionary<MemberAccess, string> {
			{ MemberAccess.Undefined, "<Undefined>" },
			{ MemberAccess.Private, "Private" },
			{ MemberAccess.Internal, "Internal" },
			{ MemberAccess.ProtectedInternal, "ProtectedInternal" },
			{ MemberAccess.Protected, "Protected" },
			{ MemberAccess.Public, "Public" },
		};

		public override string Name {
			get { return base.Name ?? $"{GetTypeName(Type)}: {GetMemberAccessName(Access)}"; }
			protected set { base.Name = value; }
		}

		public MemberType Type { get; protected set; }
		public MemberAccess Access { get; protected set; }

		public virtual bool IsValidMemberRegion => Type != MemberType.Undefined && Access != MemberAccess.Undefined;

		public RegionDirectiveTriviaSyntax Start {
			get {
				var node = ParentType.GetCurrentNode(_start);
				return node == default (RegionDirectiveTriviaSyntax)? _start : node;
			}
			private set { _start = value; }
		}

		public EndRegionDirectiveTriviaSyntax End {
			get {
				var node = ParentType.GetCurrentNode(_end);
				return node == default(EndRegionDirectiveTriviaSyntax) ? _end : node;
			}
			private set { _end = value; }
		}

		public TextSpan Span => new TextSpan(_start.SpanStart, _end.Span.End);

		public override List<IChangeApplier> CreateChangeAppliers() {
			var result = new List<IChangeApplier>();
			if (!_isNew) {
				return result;
			}
			throw  new NotImplementedException();
		}
	}
}
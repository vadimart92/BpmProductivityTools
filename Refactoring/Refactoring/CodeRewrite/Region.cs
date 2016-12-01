using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public class Region {
		private readonly Regex _nameRegex = new Regex("(?<Type>\\w+)(\\s*):(\\s*)(?<Access>\\w+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		public Region(RegionDirectiveTriviaSyntax start, EndRegionDirectiveTriviaSyntax end) {
			Start = start;
			End = end;
			var message = Start.EndOfDirectiveToken.GetAllTrivia().FirstOrDefault(t => t.IsKind(SyntaxKind.PreprocessingMessageTrivia));
			if (message != default(SyntaxTrivia)) {
				Name = message.ToFullString().Trim();
				var match = _nameRegex.Match(Name);
				if (match.Success) {
					string typeName = match.Groups["Type"].Value.Trim();
					Type = GetTypeByName(typeName);
					string access = match.Groups["Access"].Value.Trim();
					if (Type == MemberType.Class || Type == MemberType.Interface || Type == MemberType.Enum) {
						TypeName = access;
					} else {
						Access = GetMemberAccessByName(access);
					}
				}
			}
		}

		private MemberType GetTypeByName(string name) {
			if (_typeNameMap.ContainsValue(name)) {
				var mapItem = _typeNameMap.First(m => m.Value.Equals(name, StringComparison.OrdinalIgnoreCase));
				return mapItem.Key;
			}
			return MemberType.Undefined;
		}
		private string GetTypeName(MemberType type) {
			return _typeNameMap[type];
		}
		private MemberAccess GetMemberAccessByName(string name) {
			if (_accessMap.ContainsValue(name)) {
				var mapItem = _accessMap.First(m => m.Value.Equals(name, StringComparison.OrdinalIgnoreCase));
				return mapItem.Key;
			}
			return MemberAccess.Undefined;
		}
		private string GetMemberAccessName(MemberAccess access) {
			return _accessMap[access];
		}

		private readonly Dictionary<MemberType, string> _typeNameMap = new Dictionary<MemberType, string> {
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

		private readonly  Dictionary<MemberAccess, string> _accessMap = new Dictionary<MemberAccess, string> {
			{ MemberAccess.Undefined, "<Undefined>" },
			{ MemberAccess.Private, "Private" },
			{ MemberAccess.Internal, "Internal" },
			{ MemberAccess.ProtectedInternal, "ProtectedInternal" },
			{ MemberAccess.Protected, "Protected" },
			{ MemberAccess.Public, "Public" },
		};

		public string Name { get; }
		public string TypeName { get; }
		public MemberType Type { get; }
		public MemberAccess Access { get; }
		public RegionDirectiveTriviaSyntax Start { get; }
		public EndRegionDirectiveTriviaSyntax End { get; }
	}
}
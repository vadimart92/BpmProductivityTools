using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Refactoring.CodeRewrite {
	public class RegionParser : CSharpSyntaxWalker {
		private RegionParser() : base(SyntaxWalkerDepth.StructuredTrivia) {}
		private readonly List<Region> _regions = new List<Region>();
		private readonly Stack<RegionDirectiveTriviaSyntax> _start = new Stack<RegionDirectiveTriviaSyntax>();

		public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node) {
			base.VisitRegionDirectiveTrivia(node);
			_start.Push(node);
		}
		public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node) {
			base.VisitEndRegionDirectiveTrivia(node);
			if (_start.Count > 0) {
				var start = _start.Pop();
				var region = Region.Create(start, node, _typeInfo);
				_regions.Add(region);
			}
		}

		private TypeInfo _typeInfo;
		public static IReadOnlyList<Region> ParceRegions(SyntaxNode node, TypeInfo typeInfo) {
			var regionParcer = new RegionParser {_typeInfo = typeInfo};
			regionParcer.Visit(node);
			return regionParcer._regions;
		}
	}
}
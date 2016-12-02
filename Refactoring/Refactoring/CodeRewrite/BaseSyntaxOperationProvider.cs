using System.Collections.Generic;

namespace Refactoring.CodeRewrite {
	public abstract class BaseSyntaxOperationProvider {
		private string _nodeId;

		public virtual string NodeId => _nodeId ?? (_nodeId = $"{GetType().Name}[{Name}_{GetNameSuffix(Name)}]");

		public abstract List<ChangeApplier> CreateChangeAppliers();

		public virtual string Name { get; protected set; }
		protected abstract string GetNameSuffix(string name);
	}
}
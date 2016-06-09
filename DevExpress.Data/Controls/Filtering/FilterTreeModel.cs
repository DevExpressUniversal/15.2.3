#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Summary;
using DevExpress.Utils;
namespace DevExpress.Data {
	public interface ITreeSelectableItem {
		ITreeSelectableItem Parent { get; }
		List<ITreeSelectableItem> Children { get; }
		bool AllowSelect { get; }
		string Text { get; }
	}
}
namespace DevExpress.XtraEditors {
	using DevExpress.XtraEditors.Filtering;
	public enum FilterChangedAction { RebuildWholeTree, FilterStringChanged, ValueChanged, FieldNameChange, OperationChanged, GroupTypeChanged, RemoveNode, AddNode, ClearAll, AggregateOperationChanged, AggregatePropertyChanged };
	public enum FilterControlAllowAggregateEditing { No, Aggregate, AggregateWithCondition }
	public class FilterChangedEventArgs : EventArgs {
		public FilterChangedEventArgs(FilterChangedAction action, Node node) {
			this.Action = action;
			this.CurrentNode = node;
		}
		public FilterChangedAction Action { get; private set; }
		public Node CurrentNode { get; private set; }
		public override string ToString() {
			return Action.ToString() + "," + (CurrentNode == null ? "null" : CurrentNode.GetType().Name);
		}
	}
}
namespace DevExpress.XtraEditors.Filtering {
	public enum ElementType { None, Property, Value, Operation, Group, FieldAction, CollectionAction, NodeRemove, NodeAction, NodeAdd, AdditionalOperandProperty,AdditionalOperandParameter, ItemCollection, AggregateOperation, AggregateProperty }
}
namespace DevExpress.XtraEditors.Filtering {
	using DevExpress.Data;
	using DevExpress.XtraEditors;
	using DevExpress.Data.Filtering;
	using System.Windows.Forms;
	using Compatibility.System.ComponentModel;
	using Compatibility.System.Data;
	public enum FilterChangedActionInternal { NodeElementsRebuilt, NodeParentChanged, NodeAdded, NodeRemoved, RootNodeReplaced }
	public interface IBoundPropertyCollection : IEnumerable {
		IBoundProperty this[int index] { get; }
		IBoundProperty this[string fieldName] { get; }
		IBoundPropertyCollection CreateChildrenProperties(IBoundProperty listProperty);
		int Count { get; }
		void Clear();
		void Add(IBoundProperty property);
		string GetDisplayPropertyName(OperandProperty property, string fullPath);
		string GetValueScreenText(OperandProperty property, object value);
	}
	public static class IBoundPropertyDefaults {
		public static FilterColumnClauseClass GetDefaultClauseClass(IBoundProperty property) {
			if (property.Type == typeof(string)) {
				return FilterColumnClauseClass.String;
			} else if (property.Type == typeof(DateTime) || property.Type == typeof(DateTime?)) {
				return FilterColumnClauseClass.DateTime;
			} else {
				return FilterColumnClauseClass.Generic;
			}
		}
	}
	public interface IFilterParametersOwner {
		IList<IFilterParameter> Parameters { get; }
		bool CanAddParameters { get; }
		void AddParameter(string name, Type type);
	}
	public interface IFilteredComponent : IFilteredComponentBase {
		IBoundPropertyCollection CreateFilterColumnCollection();
	}
	public static class IBoundPropertyCollectionExtension {
		static IBoundProperty GetDefaultColumnOnCreate(this IBoundPropertyCollection self, IBoundProperty property) {
			if (property != null && !property.IsList) return property;
			foreach (IBoundProperty item in self) {
				if (!item.IsList) return item;
			}
			return null;
		}
		public static OperandProperty CreateDefaultProperty(this IBoundPropertyCollection self, IBoundProperty property) {
			property = GetDefaultColumnOnCreate(self, property);
			string fullName = property != null ? property.GetFullName() : string.Empty;
			return new OperandProperty(fullName);
		}
		public static IBoundProperty GetProperty(this IBoundPropertyCollection self, OperandProperty property) {
			return ReferenceEquals(property, null) ? null : self[property.PropertyName];
		}
	}
	public class NodeEditableElement {
		Node node;
		ElementType elementType;
		string text, textBefore = string.Empty, textAfter = string.Empty;
		bool isEmpty;
		int valueIndex = -1;
		public NodeEditableElement(Node node, ElementType elementType, string text) {
			this.node = node;
			this.elementType = elementType;
			this.text = text;
		}
		public override string ToString() {
			return Text;
		}
		public ElementType ElementType { get { return elementType; } }
		public string Text { get { return text; } }
		public Node Node { get { return node; } }
		public int Index { get { return Node.Elements.IndexOf(this); } }
		public string TextBefore { get { return textBefore; } set { textBefore = value; } }
		public string TextAfter { get { return textAfter; } set { textAfter = value; } }
		public bool IsEmpty { get { return isEmpty; } set { isEmpty = value; } }
		public int ValueIndex { get { return valueIndex; } set { valueIndex = value; } }
		public bool IsValueElement { get { return ValueIndex > -1; } }
		public FilterControlFocusInfo CreateFocusInfo() { return new FilterControlFocusInfo(Node, Index); }
		public CriteriaOperator AdditionalOperand { get { return Node.GetAdditionalOperand(Index); } }
	}
	public struct FilterControlFocusInfo {
		public readonly Node Node;
		public readonly int ElementIndex;
		public FilterControlFocusInfo(Node node, int elementIndex) {
			this.Node = node;
			this.ElementIndex = elementIndex;
		}
		public static bool operator ==(FilterControlFocusInfo fi1, FilterControlFocusInfo fi2) {
			return fi1.Node == fi2.Node && fi1.ElementIndex == fi2.ElementIndex;
		}
		public static bool operator !=(FilterControlFocusInfo fi1, FilterControlFocusInfo fi2) {
			return !(fi1 == fi2);
		}
		public override int GetHashCode() {
			return Node.GetHashCode() ^ ElementIndex;
		}
		public override bool Equals(object obj) {
			if (!(obj is FilterControlFocusInfo)) return false;
			FilterControlFocusInfo another = (FilterControlFocusInfo)obj;
			return this == another;
		}
		public FilterControlFocusInfo OnRight() {
			if (ElementIndex >= Node.LastTabElementIndex)
				return OnDown();
			else
				return new FilterControlFocusInfo(Node, ElementIndex + 1);
		}
		public FilterControlFocusInfo OnLeft() {
			if (ElementIndex > 0) {
				return new FilterControlFocusInfo(Node, ElementIndex - 1);
			} else {
				Node prevNode = Node.GetPrevNode();
				return new FilterControlFocusInfo(prevNode, prevNode.LastTabElementIndex);
			}
		}
		public FilterControlFocusInfo OnUp() {
			return new FilterControlFocusInfo(Node.GetPrevNode(), 0);
		}
		public FilterControlFocusInfo OnDown() {
			return new FilterControlFocusInfo(Node.GetNextNode(), 0);
		}
		public void ChangeElement(ElementType elementType) {
			Node.ChangeElement(elementType);
		}
		public void ChangeElement(object value) {
			Node.ChangeElement(ElementIndex, value);
		}
		public CriteriaOperator AdditionalOperand { get { return Node.GetAdditionalOperand(ElementIndex); } }
		public IBoundProperty FocusedFilterProperty { get { return Node.GetFocusedFilterProperty(ElementIndex); } }
		public ElementType FocusedElementType {
			get {
				if(Node == null || ElementIndex < 0 || Node.Elements.Count == 0)
					return ElementType.None;
				return Node.Elements[ElementIndex].ElementType;
			}
		}
		public object GetCurrentValue() { return Node.GetCurrentValue(ElementIndex); }
	}
	public class CreateCriteriaParseContextEventArgs : EventArgs {
		public IDisposable Context { get; set; }
	}
	public class CreateCriteriaCustomParseEventArgs : EventArgs {
		string filterText;
		public CreateCriteriaCustomParseEventArgs(string filterText) {
			this.filterText = filterText;
		}
		public string FilterText { get { return filterText; } }
		public CriteriaOperator Criteria { get; set; }
	}
	public abstract class FilterTreeNodeModel : IDisposable, IIBoundPropertyCreator {
		FilterControlFocusInfo focusInfo = new FilterControlFocusInfo();
		FilterControlAllowAggregateEditing allowAggregateEditing = FilterControlAllowAggregateEditing.No;
		bool showGroupCommandsIcon;
		bool sortProperties = true;
		bool showIsNullOperatorsForStrings = false;
		int maxOperandsCount = 20;
		IBoundProperty defaultProperty;
		GroupNode rootNode;
		object sourceControl;
		IFilteredComponent filterSourceControl;
		bool showOperandTypeIcon;
		int updaterCounter = 0;
		FilterCriteriaSubscribers filterCriteriaSubscribers;
		FilterModelPickManager pickManager;
		SourceControlNotifier notifier;
		IBoundPropertyCollection filterProperties;
		protected FilterTreeNodeModel() {
			Action<CriteriaOperator> setCriteriaOperator =
				op => this.FilterCriteria = op;
			filterCriteriaSubscribers = new FilterCriteriaSubscribers(setCriteriaOperator);
			pickManager = CreateFilterModelPickManager();
			notifier = new SourceControlNotifier();
			notifier.OnPropertiesChanged += new SourceControlNotifier.PropertyChangedDelegate(OnSourceControlPropertiesChanged);
			AllowCreateDefaultClause = true;
		}
		protected virtual FilterModelPickManager CreateFilterModelPickManager() {
			return new FilterModelPickManager(this);
		}
		public FilterModelPickManager PickManager { get { return pickManager; } }
		public event EventHandler<CreateCriteriaParseContextEventArgs> CreateCriteriaParseContext;
		public event EventHandler<CreateCriteriaCustomParseEventArgs> CreateCriteriaCustomParse;
		public abstract void SetParent(IBoundProperty property, IBoundProperty parent);
		protected abstract IBoundPropertyCollection CreateIBoundPropertyCollection();
		public virtual Type GetFunctionType(string name) {
			return typeof(object);
		}
		public List<ITreeSelectableItem> GetTreeItemsByProperties() {
			return GetTreeItemsByProperties(FocusInfo.Node.FilterProperties);
		}
		public IBoundPropertyCollection FilterProperties {
			get {
				if (filterProperties == null)
					filterProperties = CreateIBoundPropertyCollection();
				return filterProperties;
			}
			set { filterProperties = value; }
		}
		public List<ITreeSelectableItem> GetTreeItemsByProperties(IEnumerable properties) {
			List<ITreeSelectableItem> items = new List<ITreeSelectableItem>();
			foreach (IBoundProperty property in properties) {
				if (!(property is ITreeSelectableItem)) continue;
				if (AllowAggregateEditing == FilterControlAllowAggregateEditing.No && property.IsList) continue;
				if (!property.HasChildren || !property.IsAggregate || (property.HasChildren && !property.IsList)) {
					items.Add((ITreeSelectableItem)property);
				}
			}
			if(SortProperties) {
				items.Sort((x, y) => x.Text.CompareTo(y.Text));
			}
			return items;
		}
		public bool IsUpdating { get { return this.updaterCounter != 0; } }
		public void BeginUpdate() { this.updaterCounter++; }
		public void CancelUpdate() {
			if (this.updaterCounter == 0)
				throw new InvalidOperationException();
			this.updaterCounter--;
		}
		public void EndUpdate(FilterChangedAction action) {
			EndUpdate(action, null);
		}
		public void EndUpdate(FilterChangedAction action, Node node) {
			CancelUpdate();
			if (!IsUpdating) {
				ModelChanged(new FilterChangedEventArgs(action, node));
			}
		}
		public void EndUpdate() {
			CancelUpdate();
		}
		public virtual bool DoesAllowItemCollectionEditor(IBoundProperty property) { return false; }
		public delegate void NotifyControlDelegate(FilterChangedEventArgs info);
		public event NotifyControlDelegate OnNotifyControl;
		void NotifyControl(FilterChangedEventArgs info) {
			if(OnNotifyControl != null)
				OnNotifyControl(info);
		}
		public abstract IBoundProperty CreateProperty(object dataSource, string dataMember, string displayName, bool isList, PropertyDescriptor property);
		public bool ShowIsNullOperatorsForStrings {
			get { return showIsNullOperatorsForStrings; }
			set { showIsNullOperatorsForStrings = value; }
		}
		protected void RecursiveVisitor(Node node, Action<Node> action) {
			if (node == null)
				return;
			action(node);
			foreach (var child in node.GetChildren()) {
				RecursiveVisitor((Node)child, action);
			}
		}
		public void ModelChanged(FilterChangedEventArgs info) {
			if (info.Action == FilterChangedAction.AddNode) {
				RecursiveVisitor(info.CurrentNode, child =>
					OnVisualChange(FilterChangedActionInternal.NodeAdded, child));
			}
			RebuildElements();
			if (info.Action == FilterChangedAction.RemoveNode) {
				RecursiveVisitor(info.CurrentNode, child =>
					OnVisualChange(FilterChangedActionInternal.NodeRemoved, child));
			}
			if (IsUpdating) return;
			NotifyControl(info);
		}
		protected abstract FilterColumnClauseClass GetClauseClass(IBoundProperty property);
		public virtual ClauseType GetDefaultOperation(IBoundPropertyCollection properties,OperandProperty operandProperty) {
			IBoundProperty column = properties.GetProperty(operandProperty);
			if (column == null)
				return ClauseType.Equals;
			switch (GetClauseClass(column)) {
				case FilterColumnClauseClass.Blob:
					return ClauseType.IsNotNull;
				case FilterColumnClauseClass.String:
					return ClauseType.BeginsWith;
				default:
					return ClauseType.Equals;
			}
		}
		public bool IsValidClause(IBoundProperty property, ClauseType clause) {
			return IsValidClause(clause, GetClauseClass(property));
		}
		public virtual bool IsValidClause(ClauseType clause, FilterColumnClauseClass clauseClass) {
			return FilterControlHelpers.IsValidClause(clause, clauseClass, ShowIsNullOperatorsForStrings);
		}
		public bool DoAddElement() {
			FocusInfo.Node.AddElement();
			return true;
		}
		public bool DoSwapPropertyValue() {
			if(FocusInfo.FocusedElementType == ElementType.FieldAction) {
				FocusInfo.ChangeElement(null);
				return true;
			} else return false;
		}
		public virtual void AddParameter(string parameterName, Type parameterType) {
			if (ParametersOwner != null && ParametersOwner.CanAddParameters) {
				ParametersOwner.AddParameter(parameterName, parameterType);
			}
		}
		public bool ShowOperandTypeIcon {
			get { return showOperandTypeIcon; }
			set {
				if(ShowOperandTypeIcon == value) return;
				showOperandTypeIcon = value;
				RebuildElements();
			}
		}
		public virtual IFilterParametersOwner ParametersOwner { get { return SourceControl as IFilterParametersOwner; } }
		public object SourceControl {
			get { return sourceControl; }
			set {
				if (SourceControl == value) return;
				if (sourceControl is ISupportInitializeNotification)
					((ISupportInitializeNotification)sourceControl).Initialized -= new EventHandler(FilterTreeNodeModel_Initialized);
				sourceControl = value;
				if (sourceControl is ISupportInitializeNotification) {
					((ISupportInitializeNotification)sourceControl).Initialized += new EventHandler(FilterTreeNodeModel_Initialized);
					if (!((ISupportInitializeNotification)sourceControl).IsInitialized)
						return;
				}
				UpdateFilterSourceControl(true);
				OnSourceControlChanged();
			}
		}
		void FilterTreeNodeModel_Initialized(object sender, EventArgs e) {
			UpdateFilterSourceControl(true);
		}
		public virtual CriteriaOperator ToCriteria(INode node) {
			return FilterControlHelpers.ToCriteria(node);
		}
		class FilterCriteriaSubscribers {
			IFilteredComponentBase filteredComponentBase;
			IFilteredDataSource filteredDataSource;
			IBindingListView bindingListView;
			DataView dataView;
			DataTable dataTable;
			Action<CriteriaOperator> onCriteriaChanged;
			public FilterCriteriaSubscribers(Action<CriteriaOperator> onCriteriaChanged) {
				this.onCriteriaChanged = onCriteriaChanged;
			}
			public void PropagateToSourceControl(CriteriaOperator criteria) {
				if(filteredComponentBase != null) {
					if(!Equals(filteredComponentBase.RowCriteria, criteria)) {
						filteredComponentBase.RowCriteria = criteria;
					}
				}
				if(filteredDataSource != null) {
					if(!Equals(filteredDataSource.Filter, criteria)) {
						filteredDataSource.Filter = criteria;
					}
					}
				if(bindingListView != null) {
					bindingListView.Filter = CriteriaToWhereClauseHelper.GetDataSetWhere(criteria);
				}
				if(dataView != null) {
					dataView.RowFilter = CriteriaToWhereClauseHelper.GetDataSetWhere(criteria);
				}
				if(dataTable != null) {
					dataTable.DefaultView.RowFilter = CriteriaToWhereClauseHelper.GetDataSetWhere(criteria);
			}
			}
			public void Set(object sourceControl, bool resetCriteria) {
				Clear();
				bindingListView = sourceControl as IBindingListView;
				filteredComponentBase = sourceControl as IFilteredComponentBase;
				if(filteredComponentBase != null) {
					if(resetCriteria) {
						onCriteriaChanged(filteredComponentBase.RowCriteria);
					}
					filteredComponentBase.RowFilterChanged += new EventHandler(filteredComponentBase_RowFilterChanged);
			}
				filteredDataSource = sourceControl as IFilteredDataSource;
				if(filteredDataSource != null && resetCriteria) {
					onCriteriaChanged(filteredDataSource.Filter);
				}
				dataView = sourceControl as DataView;
				dataTable = sourceControl as DataTable;
			}
			public List<Type> GetKnownInterfaces(object sourceControl) {
				List<Type> known = new List<Type>();
				if(sourceControl is IBindingListView)
					known.Add(typeof(IBindingListView));
				if(sourceControl is IFilteredComponentBase)
					known.Add(typeof(IFilteredComponentBase));
				if(sourceControl is IFilteredDataSource)
					known.Add(typeof(IFilteredDataSource));
				if(sourceControl is IFilterParametersOwner)
					known.Add(typeof(IFilterParametersOwner));
				return known;
			}
			void filteredComponentBase_RowFilterChanged(object sender, EventArgs e) {
				onCriteriaChanged(this.filteredComponentBase.RowCriteria);
			}
			public void Clear() {
				if (filteredComponentBase != null) {
					this.filteredComponentBase.RowFilterChanged -= new EventHandler(filteredComponentBase_RowFilterChanged);
				}
				filteredComponentBase = null;
				filteredDataSource = null;
				bindingListView = null;
				dataView = null;
				dataTable = null;
			}
		}
		bool werePropertiesCreatedByModel = false;
		void UpdateFilterSourceControl(bool resetCriteria) {
			List<IBoundProperty> list = pickManager.PickProperties(SourceControl, string.Empty, null);
			filterCriteriaSubscribers.Set(SourceControl, resetCriteria);
			foreach(Type type in filterCriteriaSubscribers.GetKnownInterfaces(SourceControl)) {
				RemoveInterfaceProperties(list, type);
			}
			FilterSourceControl = SourceControl as IFilteredComponent;
			if (FilterSourceControl != null) {
				var collection = FilterSourceControl.CreateFilterColumnCollection();
				if (ReferenceEquals(collection, FilterProperties)) {
					return;
				}
				list.Clear();
				foreach (IBoundProperty property in collection) {
					list.Add(property);
				}
				return;
			}
			notifier.SourceControl = SourceControl;
			if (SourceControl is IFilterParametersOwner
				&& !werePropertiesCreatedByModel
				&& FilterProperties.Count > 0) {
				return;
			}
			FilterProperties.Clear();
			werePropertiesCreatedByModel = true;
			foreach (IBoundProperty property in list) {
				FilterProperties.Add(property);
			}
			OnSourceControlChanged();
		}
		private static void RemoveInterfaceProperties(List<IBoundProperty> list, Type interfaceType) {
			list.RemoveAll(prop => {
				var props = interfaceType.GetProperties();
				foreach (var interfaceProp in props) {
					if (interfaceProp.Name == prop.Name) {
						return true;
					}
				}
				return false;
			});
		}
		void OnSourceControlPropertiesChanged() {
			UpdateFilterSourceControl(false);
		}
		public CriteriaOperator FilterCriteria {
			get {
				if (RootNode == null)
					return null;
				else
					return ToCriteria(RootNode);
			}
			set {
			   if (ReferenceEquals(value, null) || ReferenceEquals(FilterCriteria, null) || value.ToString() != FilterCriteria.ToString())
					CreateTree(value);
			}
		}
		IFilteredComponent FilterSourceControl {
			get {
				return filterSourceControl;
			}
			set {
				if (FilterSourceControl == value) return;
				if (FilterSourceControl != null) {
					FilterSourceControl.PropertiesChanged -= new EventHandler(SourceControl_DataSourceChanged);
				}
				filterSourceControl = value;
				if (FilterSourceControl != null) {
					FilterSourceControl.PropertiesChanged += new EventHandler(SourceControl_DataSourceChanged);
				}
				OnSourceControlChanged();
			}
		}
		protected abstract void SetFilterColumnsCollection(IBoundPropertyCollection propertyCollection);
		void CreateFilterColumnCollection() {
			if (FilterSourceControl == null) return;
			SetFilterColumnsCollection(FilterSourceControl.CreateFilterColumnCollection());
		}
		void OnSourceControlChanged() {
			CreateFilterColumnCollection();
			CreateTree(FilterCriteria);
		}
		public void ApplyFilter() {
			filterCriteriaSubscribers.PropagateToSourceControl(FilterCriteria);
		}
		void SourceControl_DataSourceChanged(object sender, EventArgs e) {
			OnSourceControlChanged();
		}
		public GroupNode RootNode {
			get { return rootNode; }
			set {
				if (rootNode == value)
					return;
				rootNode = value;
				OnVisualChange(FilterChangedActionInternal.RootNodeReplaced, null);
			}
		}
		public IBoundProperty GetDefaultProperty() {
			return defaultProperty;
		}
		public void SetDefaultProperty(IBoundProperty property) {
			if (property != null && property.IsList) return;
			this.defaultProperty = property;
		}
		public FilterControlFocusInfo FocusInfo {
			get { return focusInfo; }
			set {
				if (FocusInfo == value) return;
				focusInfo = value;
				OnFocusInfoChanged();
			}
		}
		public FilterControlAllowAggregateEditing AllowAggregateEditing {
			get { return allowAggregateEditing; }
			set { allowAggregateEditing = value; }
		}
		public bool SortProperties { get { return sortProperties; } set { sortProperties = value; } }
		public bool ShowGroupCommandsIcon {
			get { return showGroupCommandsIcon; }
			set {
				if (ShowGroupCommandsIcon == value) return;
				showGroupCommandsIcon = value;
				ModelChanged(new FilterChangedEventArgs(FilterChangedAction.RebuildWholeTree, null));
			}
		}
		public int MaxOperandsCount {
			get { return maxOperandsCount; }
			set {
				maxOperandsCount = value;
				if (maxOperandsCount < 0) maxOperandsCount = 0;
			}
		}
		protected INodesFactoryEx CreateNodesFactory() {
			return new FilterControlNodesFactory(this);
		}
		Node CreateNodeFromCriteria(CriteriaOperator criteria) {
			return (Node)CriteriaToTreeProcessor.GetTree(CreateNodesFactory(), criteria, null);
		}
		public GroupNode CreateGroupNode(GroupNode parent) {
			GroupNode result = CreateGroupNode();
			if (parent != null) {
				parent.AddNode(result);
			}
			return result;
		}
		public GroupNode AddGroup(GroupNode parent) {
			GroupNode groupNode = CreateGroupNode(parent);
			BeginUpdate();
			groupNode.AddElement();
			EndUpdate(FilterChangedAction.AddNode, groupNode);
			return groupNode;
		}
		public bool DoPasteElement(string clipboardText) {
			GroupNode addTo = FocusInfo.Node as GroupNode;
			if (addTo == null)
				addTo = ((INode)FocusInfo.Node).ParentNode as GroupNode;
			try {
				CriteriaOperator op = CriteriaFromString(clipboardText);
				if (ReferenceEquals(op, null))
					return false;
				Node cond = CreateNodeFromCriteria(op);
				if (cond == null)
					return false;
				addTo.AddNode(cond);
				FocusInfo = new FilterControlFocusInfo(cond, 0);
				RebuildElements();
				return true;
			} catch {
				return false;
			}
		}
		public void RebuildElements() {
			RebuildElements(RootNode);
		}
		void RebuildElements(Node node) {
			if (node == null) return;
			node.RebuildElements();
			OnVisualChange(FilterChangedActionInternal.NodeElementsRebuilt, node);
			foreach (Node child in node.GetChildren()) {
				RebuildElements(child);
			}
		}
		protected virtual void ValidateAdditionalOperands(IClauseNode node) {
			((ClauseNode)node).ValidateAdditionalOperands();
		}
		protected virtual ClauseNode CreateDefaultClauseNode(IBoundProperty property) {
			ClauseNode cond = CreateClauseNode();
			cond.FirstOperand = FilterProperties.CreateDefaultProperty(property);
			cond.Operation = GetDefaultOperation(filterProperties, cond.FirstOperand);
			ValidateAdditionalOperands(cond);
			return cond;
		}
		public abstract void OnVisualChange(FilterChangedActionInternal action, Node node);
		public CriteriaOperator CriteriaParse(string value) {
			CreateCriteriaParseContextEventArgs args = new CreateCriteriaParseContextEventArgs();
			if(CreateCriteriaParseContext != null) {
				CreateCriteriaParseContext(this, args);
			}
			try {
				CriteriaOperator result = null;
				if (CreateCriteriaCustomParse != null) {
					CreateCriteriaCustomParseEventArgs customArgs = new CreateCriteriaCustomParseEventArgs(value);
					CreateCriteriaCustomParse(this, customArgs);
					result = customArgs.Criteria;
				}
				return object.Equals(result, null) ? CriteriaFromString(value) : result;
			}
			finally {
				if(args.Context != null) {
					args.Context.Dispose();
				}
			}
		}
		protected virtual CriteriaOperator CriteriaFromString(string value) {
			return CriteriaOperator.Parse(value);
		}
		public string CriteriaSerialize(CriteriaOperator criteria) {
			return CriteriaToString(criteria);
		}
		protected virtual string CriteriaToString(CriteriaOperator criteria) {
			return CriteriaOperator.ToString(criteria);
		}
		public string FilterString {
			get { return CriteriaToString(FilterCriteria); }
			set {
				string oldString = FilterString;
				CreateTree(CriteriaParse(value));
				if(value != oldString)
				ModelChanged(new FilterChangedEventArgs(FilterChangedAction.FilterStringChanged, null));
			}
		}
		public virtual ClauseNode CreateDefaultClauseNode(IBoundPropertyCollection filterProperties) {
			IBoundProperty defaultProperty = GetDefaultProperty();
			if (filterProperties != null && filterProperties != FilterProperties) {
				defaultProperty = filterProperties[0];
			}
			return CreateDefaultClauseNode(defaultProperty);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Node CreateCriteriaByDefaultProperty() {
			IBoundProperty defaultProperty = GetDefaultProperty();
			return defaultProperty != null ? CreateDefaultClauseNode(defaultProperty) : null;
		}
		public bool AllowCreateDefaultClause { get; set; }
		public void CreateTree(CriteriaOperator criteria) {
			BeginUpdate();
			RootNode = null;
			Node node = CreateNodeFromCriteria(criteria);
			if (AllowCreateDefaultClause && node == null) {
				node = CreateCriteriaByDefaultProperty();
			}
			this.RootNode = node as GroupNode;
			if (RootNode == null) {
				RootNode = CreateGroupNode(null);
				if (node != null) {
					RootNode.AddNode(node);
				}
			}
			this.FocusInfo = new FilterControlFocusInfo(this.RootNode, 0);
			EndUpdate(FilterChangedAction.RebuildWholeTree);
		}
		public virtual ClauseNode CreateClauseNode() {
			return new ClauseNode(this);
		}
		public virtual GroupNode CreateGroupNode() {
			return new GroupNode(this);
		}
		public virtual AggregateNode CreateAggregateNode() {
			return new AggregateNode(this);
		}
		public List<IFilterParameter> GetParametersByType(Type type) {
			List<IFilterParameter> result = new List<IFilterParameter>();
			IList<IFilterParameter> parameters = GetParameters();
			if (parameters == null) return result;
			foreach (IFilterParameter parameter in parameters) {
				if (TypeConvertionValidator.CanConvertType(parameter.Type, type)) {
					result.Add(parameter);
				}
			}
			return result;
		}
		protected virtual IList<IFilterParameter> GetParameters() { return ParametersOwner != null ? ParametersOwner.Parameters : null; }
		public bool ShowParameterTypeIcon { get { return ParametersOwner != null; } }
		public virtual bool CanAddParameters { get { return ParametersOwner != null && ParametersOwner.CanAddParameters; } }
		protected virtual void OnFocusInfoChanged() { }
		public abstract string GetLocalizedStringForFilterEmptyParameter();
		public abstract string GetLocalizedStringForFilterEmptyEnter();
		public abstract string GetLocalizedStringForFilterClauseBetweenAnd();
		public abstract string GetLocalizedStringForFilterEmptyValue();
		#region IDisposable Members
		public void Dispose() {
			notifier.Dispose();
			if (filterCriteriaSubscribers != null)
				filterCriteriaSubscribers.Clear();
		}
		#endregion
		public abstract string GetMenuStringByType(GroupType type);
		public abstract string GetMenuStringByType(Aggregate type);
		public abstract string GetMenuStringByType(ClauseType type);
	}
	public abstract class Node : INode {
		static IList<INode> nullableChildren = new List<INode>();
		FilterTreeNodeModel model;
		Node parentNode;
		List<NodeEditableElement> elements;
		IBoundPropertyCollection filterProperties;
		internal ObservableList<INode> CreateObserveblaListForChildNodes() {
			return new ObservableList<INode>(
				(int index, INode item) =>
					NodeChanged(FilterChangedAction.AddNode, (Node)item),
				(int index, INode item) =>
					NodeChanged(FilterChangedAction.RemoveNode, (Node)item),
				(int index, INode item) => { }
			);
		}
		protected Node(FilterTreeNodeModel model) {
			this.model = model;
			this.elements = new List<NodeEditableElement>();
		}
		public List<Node> GetAbsoluteList() {
			List<Node> list = new List<Node>();
			GetAbsoluteList(list);
			return list;
		}
		public virtual void GetAbsoluteList(List<Node> list) {
			list.Add(this);
		}
		public int Index {
			get {
				if (RootNode != null) {
					List<Node> absList = new List<Node>();
					RootNode.GetAbsoluteList(absList);
					return absList.IndexOf(this);
				} else {
					return -1;
				}
			}
		}
		public Node RootNode {
			get {
				Node node = this;
				while (node.ParentNode != null) {
					node = node.ParentNode;
				}
				return node;
			}
		}
		public int Level {
			get {
				int level = 0;
				Node node = ParentNode;
				while (node != null) {
					node = node.ParentNode;
					level++;
				}
				return level;
			}
		}
		protected internal class ObservableList<T> : Collection<T> {
			ObservableListDelegate onInsert;
			ObservableListDelegate onRemove;
			ObservableListDelegate onSet;
			public delegate void ObservableListDelegate(int index, T item);
			public ObservableList(ObservableListDelegate onInsert, ObservableListDelegate onRemove, ObservableListDelegate onSet) {
				this.onInsert = onInsert;
				this.onRemove = onRemove;
				this.onSet = onSet;
			}
			protected override void InsertItem(int index, T item) {
				base.InsertItem(index, item);
				onInsert(index, item);
			}
			protected override void RemoveItem(int index) {
				T itemToRemove = this[index];
				base.RemoveItem(index);
				onRemove(index, itemToRemove);
			}
			protected override void SetItem(int index, T item) {
				base.SetItem(index, item);
				onSet(index, item);
			}
			protected override void ClearItems() {
				for (int index = 0; index < this.Count; ++index) {
					onRemove(index, this[index]);
				}
				base.ClearItems();
			}
		}
		public virtual IList<INode> GetChildren() { return nullableChildren; }
		protected FilterControlFocusInfo FocusInfo {
			get { return Model.FocusInfo; }
			set { Model.FocusInfo = value; }
		}
		public List<NodeEditableElement> Elements { get { return elements; } }
		public abstract void AddElement();
		public virtual void DeleteElement() {
			GroupNode parent = ParentNode as GroupNode;
			if (parent == null) return;  
			int indexOfFocusedNode = parent.GetChildren().IndexOf(FocusInfo.Node);
			System.Diagnostics.Debug.Assert(indexOfFocusedNode >= 0);
			parent.GetChildren().RemoveAt(indexOfFocusedNode);
			if (indexOfFocusedNode >= parent.GetChildren().Count)
				indexOfFocusedNode = parent.GetChildren().Count - 1;
			if (indexOfFocusedNode >= 0) {
				FocusInfo = new FilterControlFocusInfo((Node)parent.GetChildren()[indexOfFocusedNode], 0);
			} else {
				FocusInfo = new FilterControlFocusInfo(parent, 0);
			}
		}
		public virtual int LastTabElementIndex { get { return FilterControlHelpers.GetLastElementIndex(this); } }
		public virtual IBoundProperty GetFocusedFilterProperty(int elementIndex) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ChangeElement(int elementIndex, object value) {
			if (GetElement(elementIndex) != null) {
				ChangeElement(GetElement(elementIndex), value);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ChangeElement(ElementType elementType) {
			ChangeElement(elementType, null);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ChangeElement(ElementType elementType, object value) {
			if (GetElement(elementType) != null) {
				ChangeElement(GetElement(elementType), value);
			}
		}
		protected NodeEditableElement GetElement(int elementIndex) {
			if (elementIndex >= 0 && elementIndex < Elements.Count) return Elements[elementIndex];
			return null;
		}
		protected NodeEditableElement GetElement(ElementType elementType) {
			foreach (NodeEditableElement item in Elements) {
				if (item.ElementType == elementType) return item;
			}
			return null;
		}
		protected virtual void ChangeElement(NodeEditableElement element, object value) { }
		public virtual void RebuildElements() {
			Elements.Clear();
		}
		[Obsolete("Obsolete method. No need to update label infos manually.")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RecalcLabelInfo() {
		}
		protected NodeEditableElement AddEditableElement(ElementType elementType, string text) {
			NodeEditableElement element = new NodeEditableElement(this, elementType, text);
			Elements.Add(element);
			return element;
		}
		protected void NodeChanged(FilterChangedAction action) {
			NodeChanged(action, this);
		}
		protected virtual void NodeChanged(FilterChangedAction action, Node node) {
			Model.ModelChanged(new FilterChangedEventArgs(action, node));
		}
		public abstract string Text { get; }
		public virtual CriteriaOperator GetAdditionalOperand(int elementIndex) { return null; }
		public virtual object GetCurrentValue(int elementIndex) { return null; }
		[Obsolete("Use FilterControl.FilterCriteria or FilterControl.ToCriteria instead")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public CriteriaOperator ToCriteria() {
			return FilterControlHelpers.ToCriteria(this);
		}
		public FilterTreeNodeModel Model { get { return model; } }
		public IBoundPropertyCollection FilterProperties {
			get {
				if (Model == null) return null;
				if (this.filterProperties == null) {
					this.filterProperties = ParentNode == null ? Model.FilterProperties : ParentNode.GetChildrenFilterProperties();
				}
				return this.filterProperties;
			}
		}
		protected virtual IBoundPropertyCollection GetChildrenFilterProperties() { return FilterProperties; }
		public Node ParentNode { get { return parentNode; } set { parentNode = value; } }
		IGroupNode INode.ParentNode { get { return ParentNode as IGroupNode; } }
		public Node GetPrevNode() {
			List<Node> list = GetFlatNodeList();
			int index = list.IndexOf(this);
			if (index <= 0) return list[list.Count - 1];
			return list[index - 1];
		}
		public Node GetNextNode() {
			List<Node> list = GetFlatNodeList();
			int index = list.IndexOf(this);
			if (index >= list.Count - 1) return list[0];
			return list[index + 1];
		}
		List<Node> GetFlatNodeList() {
			Node root = this;
			while (root.ParentNode != null) {
				root = root.ParentNode;
			}
			List<Node> list = new List<Node>();
			root.CollectFlatNodeList(list);
			return list;
		}
		void CollectFlatNodeList(List<Node> list) {
			list.Add(this);
			foreach (Node node in GetChildren()) {
				node.CollectFlatNodeList(list);
			}
		}
		protected internal void SetParentNode(INode node) {
			parentNode = (Node)node;
			ClearFilterProperties();
			if (Model != null) {
				Model.OnVisualChange(FilterChangedActionInternal.NodeParentChanged, this);
			}
		}
		void ClearFilterProperties() {
			this.filterProperties = null;
			foreach (Node node in GetChildren()) {
				node.ClearFilterProperties();
			}
		}
		void INode.SetParentNode(IGroupNode node) {
			SetParentNode(node);
		}
		object INode.Accept(INodeVisitor visitor) {
			return Accept(visitor);
		}
		protected abstract object Accept(INodeVisitor visitor);
	}
	public class GroupNode : Node, IGroupNode {
		GroupType _NodeType;
		ObservableList<INode> _SubNodes;
		public GroupNode(FilterTreeNodeModel model)
			: base(model) {
			_SubNodes = CreateObserveblaListForChildNodes();
		}
		public GroupType NodeType {
			get { return _NodeType; }
			set {
				_NodeType = value;
				NodeChanged(FilterChangedAction.GroupTypeChanged);
			}
		}
		public IList<INode> SubNodes {
			get { return _SubNodes; }
		}
		public int GetHighestLevelOfChildren() {
			int maxLevel = 0;
			foreach (Node node in GetAbsoluteList()) {
				if (node.Level > maxLevel) {
					maxLevel = node.Level;
				}
			}
			return maxLevel + 1;
		}
		public override void GetAbsoluteList(List<Node> list) {
			base.GetAbsoluteList(list);
			for (int i = 0; i < SubNodes.Count; i++) {
				((Node)SubNodes[i]).GetAbsoluteList(list);
			}
		}
		public Node GetNodeByIndex(int index) {
			return (Node)GetAbsoluteList()[index];
		}
		public void AddNode(INode node) {
			SubNodes.Add(node);
			node.SetParentNode(this);
		}
		public override IList<INode> GetChildren() { return SubNodes; }
		public override void AddElement() {
			Model.BeginUpdate();
			ClauseNode newNode = Model.CreateDefaultClauseNode(FilterProperties);
			AddNode(newNode);
			Model.EndUpdate(FilterChangedAction.AddNode, (Node)newNode);
			FocusInfo = new FilterControlFocusInfo(newNode, 0);
		}
		public override void DeleteElement() {
			if (ParentNode == null) {
				Model.BeginUpdate();
				SubNodes.Clear();
				Model.EndUpdate(FilterChangedAction.ClearAll, this);
				FocusInfo = new FilterControlFocusInfo(this, 0);
			} else {
				base.DeleteElement();
			}
		}
		public override void RebuildElements() {
			base.RebuildElements();
			AddEditableElement(ElementType.Group, Model.GetMenuStringByType(NodeType));
			if (Model.ShowGroupCommandsIcon) {
				AddEditableElement(ElementType.NodeAction, "@*");
			}
			AddEditableElement(ElementType.NodeAdd, "@+");
		}
		protected override void ChangeElement(NodeEditableElement element, object value) {
			switch (element.ElementType) {
				case ElementType.Group:
					NodeType = (GroupType)value;
					break;
				case ElementType.NodeRemove:
					DeleteElement();
					break;
				case ElementType.NodeAdd:
					AddElement();
					break;
			}
		}
		protected internal void ReplaceClauseNodes(ClauseNode oldNode, ClauseNode newNode) {
			((INode)newNode).SetParentNode(this);
			SubNodes.Insert(SubNodes.IndexOf(oldNode), newNode);
			SubNodes.Remove(oldNode);
		}
		public override string Text { get { return NodeType.ToString(); } }
		protected override object Accept(INodeVisitor visitor) {
			return visitor.Visit(this);
		}
	}
	public class ClauseNode : Node, IClauseNode {
		OperandProperty _FirstOperand = new OperandProperty();
		ClauseType _Operation;
		readonly IList<CriteriaOperator> _AdditionalOperands;
		bool disableValueChangedNotification = false;
		public ClauseNode(FilterTreeNodeModel model)
			: base(model) {
			_AdditionalOperands = new ObservableList<CriteriaOperator>(AdditionalOperandsChaged, AdditionalOperandsChaged, AdditionalOperandsChaged);
		}
		void AdditionalOperandsChaged(int index, CriteriaOperator item) {
			if(this.disableValueChangedNotification || (Model != null && Model.IsUpdating)) return;
			NodeChanged(FilterChangedAction.ValueChanged);
		}
		public bool IsCollectionValues { get { return Operation == ClauseType.AnyOf || Operation == ClauseType.NoneOf; } }
		protected bool IsTwoFieldsClause { get { return Operation == ClauseType.Between || Operation == ClauseType.NotBetween; } }
		protected bool CanAddCollectionItem { get { return IsCollectionValues && !IsShowCollectionValueAsOnEditor; } }
		public IBoundProperty Property { get { return FilterProperties != null ? FilterProperties.GetProperty(FirstOperand) : null; } }
		public virtual IBoundProperty GetPropertyForEditing() { return Property; }
		public bool IsShowCollectionValueAsOnEditor {
			get {
				if (Property == null) return false;
				return IsCollectionValues && Model.DoesAllowItemCollectionEditor(Property) && AdditionalOperands.Count > Model.MaxOperandsCount;
			}
		}
		public object GetValue(int index) {
			OperandValue additionalOperands = AdditionalOperands[index] as OperandValue;
			if (Object.Equals(additionalOperands, null))
				return null; 
			object value = additionalOperands.Value;
			if (Object.Equals(value, DateTime.MinValue))
				return null; 
			return value;
		}
		public override IBoundProperty GetFocusedFilterProperty(int elementIndex) {
			NodeEditableElement element = GetElement(elementIndex);
			if (element == null) return null;
			if (element.ElementType == ElementType.Property) return Property;
			if (element.ElementType == ElementType.AdditionalOperandProperty) {
				OperandProperty operandProperty = AdditionalOperands[element.ValueIndex] as OperandProperty;
				return !ReferenceEquals(operandProperty, null) ? FilterProperties.GetProperty(operandProperty) : null;
			}
			return null;
		}
		public virtual List<ClauseType> GetAvailableOperations() {
			return GetAvailableOperations(Property);
		}
		protected List<ClauseType> GetAvailableOperations(IBoundProperty forProperty) {
			List<ClauseType> list = new List<ClauseType>();
			if (forProperty == null) return list;
			foreach (ClauseType type in Enum.GetValues(typeof(ClauseType))) {
				if (Model.IsValidClause(forProperty, type)) {
					list.Add(type);
				}
			}
			return list;
		}
		object GetCorrectedValueType(object value) {
			Type valueType = GetValueType();
			if (valueType == null) return value;
			return DevExpress.Data.Helpers.FilterHelper.CorrectFilterValueType(valueType, value);
		}
		protected virtual Type GetValueType() {
			return GetPropertyForEditing() != null ? GetPropertyForEditing().Type : null;
		}
		public override void AddElement() {
			NodeEditableElement element = GetElement(FocusInfo.ElementIndex);
			if (IsCollectionValues && element != null && (element.IsValueElement || element.ElementType == ElementType.ItemCollection ||
				element.ElementType == ElementType.CollectionAction || element.ElementType == ElementType.Operation)) {
				ChangeElement(ElementType.CollectionAction);
			} else {
				((Node)ParentNode).AddElement();
			}
		}
		public override void DeleteElement() {
			NodeEditableElement element = GetElement(FocusInfo.ElementIndex);
			if (IsCollectionValues && element != null && (element.IsValueElement || element.ElementType == ElementType.ItemCollection)) {
				if (element.ElementType != ElementType.ItemCollection && element.ValueIndex >= AdditionalOperands.Count - 1) {
					FocusInfo = new FilterControlFocusInfo(this, element.Index - 1);
				}
				AdditionalOperands.RemoveAt(element.ValueIndex);
			} else {
				base.DeleteElement();
			}
		}
		public override int LastTabElementIndex {
			get {
				for (int i = Elements.Count - 1; i >= 0; i--) {
					if (Elements[i].IsValueElement) return i;
					if (Elements[i].ElementType == ElementType.Operation) return i;
					if (Elements[i].ElementType == ElementType.Property) return i;
					if (Elements[i].ElementType == ElementType.AggregateOperation) return i;
				}
				return 0;
			}
		}
		public override void RebuildElements() {
			base.RebuildElements();
			AddEditableElement(ElementType.Property, GetDisplayText(FirstOperand)).TextAfter = " ";
			BuildOperationAndValueElements();
		}
		protected void BuildOperationAndValueElements() {
			AddEditableElement(ElementType.Operation, Model.GetMenuStringByType(Operation)).TextAfter = " "; ;
			BuildValueElements();
			if (CanAddCollectionItem) {
				AddEditableElement(ElementType.CollectionAction, "@+");
			}
			AddEditableElement(ElementType.NodeRemove, "@-");
		}
		protected void BuildValueElements() {
			if (AdditionalOperands == null) return;
			if (IsShowCollectionValueAsOnEditor) {
				AddEditableElement(ElementType.ItemCollection, GetCollectionValuesString());
				return;
			}
			for (int i = 0; i < AdditionalOperands.Count; i++) {
				CriteriaOperator op = AdditionalOperands[i];
				string text = StringAdaptation(GetDisplayText(FirstOperand, op));
				if (text == null || text.Length == 0)
					text = "''";
				ElementType elementType = ElementType.AdditionalOperandProperty;
				bool isEmpty = !ReferenceEquals(op, null) && op.ToString() == "?";
				if (op is OperandParameter) {
					elementType = ElementType.AdditionalOperandParameter;
					if (isEmpty) {
						text = Model.GetLocalizedStringForFilterEmptyParameter();
					}
				} else if (op is OperandValue) {
					elementType = ElementType.Value;
					if (isEmpty) {
						text = Model.GetLocalizedStringForFilterEmptyEnter();
					}
				}
				NodeEditableElement element = AddEditableElement(elementType, text);
				element.IsEmpty = isEmpty;
				element.ValueIndex = i;
				NodeEditableElement operandElement = null;
				if (ShowOperandTypeIcon) {
					operandElement = AddEditableElement(ElementType.FieldAction, "@#");
					operandElement.ValueIndex = i;
				}
				if (IsTwoFieldsClause && i == 1) {
					element.TextBefore = " " + Model.GetLocalizedStringForFilterClauseBetweenAnd() + " ";
				}
				if (IsCollectionValues && AdditionalOperands.Count > 1) {
					if (i == 0) {
						element.TextBefore = "(";
					}
					if (i > 0) {
						element.TextBefore = ", ";
					}
					if (i == AdditionalOperands.Count - 1) {
						if (operandElement != null) {
							operandElement.TextAfter = ")";
						} else {
							element.TextAfter = ")";
						}
					}
				}
			}
		}
		protected override void ChangeElement(NodeEditableElement element, object value) {
			switch (element.ElementType) {
				case ElementType.Operation:
					Model.BeginUpdate();
					Operation = (ClauseType)value;
					ValidateAdditionalOperands();
					Model.EndUpdate(FilterChangedAction.OperationChanged, this);
					FilterControlFocusInfo fi = FocusInfo.OnRight();
					if (fi.Node == FocusInfo.Node)
						FocusInfo = fi;
					break;
				case ElementType.CollectionAction:
					Model.BeginUpdate();
					AdditionalOperands.Add(new OperandValue());
					Model.EndUpdate();
					int focusIndex = GetElement(ElementType.Operation).Index + 1;
					if (!IsShowCollectionValueAsOnEditor) {
						focusIndex += AdditionalOperands.Count - 1;
					}
					NodeChanged(FilterChangedAction.ValueChanged);
					FocusInfo = new FilterControlFocusInfo(this, focusIndex);
					break;
				case ElementType.NodeRemove:
					DeleteElement();
					break;
				case ElementType.Value:
					ChangeValue(element.ValueIndex, value);
					break;
				case ElementType.AdditionalOperandParameter:
					string parameterName = GetParameterName(value.ToString());
					Model.AddParameter(parameterName, Property.Type);
					AdditionalOperands[element.ValueIndex] = new OperandParameter(parameterName);
					NodeChanged(FilterChangedAction.ValueChanged);
					break;
				case ElementType.AdditionalOperandProperty:
					CriteriaOperator cOperator;
					if (FilterProperties[value.ToString()] != null) {
						cOperator = new OperandProperty(value.ToString());
					} else {
						if (Enum.IsDefined(typeof(FunctionOperatorType), value.ToString())) {
							cOperator = new FunctionOperator((FunctionOperatorType)Enum.Parse(typeof(FunctionOperatorType), value.ToString()));
						} else {
							cOperator = new FunctionOperator(value.ToString());
						}
					}
					AdditionalOperands[element.ValueIndex] = cOperator;
					NodeChanged(FilterChangedAction.ValueChanged);
					break;
				case ElementType.ItemCollection:
					AdditionalOperands.Clear();
					List<object> objects = value as List<object>;
					if (objects != null) {
						foreach (object itemValue in objects) {
							AdditionalOperands.Add(new OperandValue(GetCorrectedValueType(itemValue)));
						}
						NodeChanged(FilterChangedAction.ValueChanged);
					}
					break;
				case ElementType.Property:
					if (ReferenceEquals(FirstOperand, null) || FirstOperand.PropertyName != value.ToString()) {
						ClauseNodeFirstOperandChanged(new OperandProperty(value.ToString()), element.Index);
					}
					break;
				case ElementType.FieldAction:
					if (element.IsValueElement) {
						SwapAdditionalOperandType(element.ValueIndex, CreateDefaultProperty(), CreateDefaultParameter());
						NodeChanged(FilterChangedAction.ValueChanged);
					}
					break;
			}
		}
		public void ChangeValue(int index, object value) {
			value = GetCorrectedValueType(value);
			AdditionalOperands[index] = new OperandValue(value);
		}
		const int MaxLength = 100;
		protected string StringAdaptation(string text) {
			text = text.Replace("\r", "_").Replace("\n", "").Replace("\t", " ");
			if (text.Length > MaxLength) text = text.Substring(0, MaxLength) + "...";
			return text;
		}
		protected string GetDisplayText(OperandProperty property) {
			return new OperandProperty(FilterProperties.GetDisplayPropertyName(property, null)).ToString();
		}
		protected string GetDisplayText(OperandProperty firstOperand, CriteriaOperator op) {
			OperandParameter parameter = op as OperandParameter;
			if (!ReferenceEquals(parameter, null)) {
				return op.ToString();
			}
			OperandValue value = op as OperandValue;
			if (!ReferenceEquals(value, null)) {
				return FilterProperties.GetValueScreenText(firstOperand, value.Value);
			}
			OperandProperty property = op as OperandProperty;
			if (!ReferenceEquals(property, null)) {
				return GetDisplayText(property);
			}
			FunctionOperator func = op as FunctionOperator;
			if (!ReferenceEquals(func, null)) {
				return FilterProperties.GetValueScreenText(firstOperand, func);
			}
			return Model.CriteriaSerialize(op);
		}
		string GetCollectionValuesString() {
			int MaxLength = 45;
			if (AdditionalOperands == null || AdditionalOperands.Count == 0) return string.Empty;
			string ret = StringAdaptation(GetDisplayText(FirstOperand, AdditionalOperands[0]));
			for (int i = 1; i < AdditionalOperands.Count; i++) {
				string aOperand = string.Format(", {0}", StringAdaptation(GetDisplayText(FirstOperand, AdditionalOperands[i])));
				if (aOperand.Length > 2)
					ret += aOperand;
				if (ret.Length > MaxLength + 1) break;
			}
			if (ret.Length > MaxLength)
				ret = ret.Substring(0, MaxLength) + "...";
			if (ret.Length == 0) ret = "...";
			return ret;
		}
		string GetParameterName(string parameterName) {
			StringBuilder builder = new StringBuilder();
			if (parameterName != null) {
				foreach (char ch in parameterName) {
					if (CriteriaLexer.CanContinueColumn(ch))
						builder.Append(ch);
				}
			}
			return builder.ToString();
		}
		protected virtual bool IsRequireChangeNodeType(IBoundProperty newProperty) {
			bool isListProperty = newProperty != null && newProperty.IsList;
			return IsList != isListProperty;
		}
		protected virtual void ClauseNodeFirstOperandChanged(OperandProperty newProp, int elementIndex) {
			Model.BeginUpdate();
			IBoundProperty newProperty = FilterProperties.GetProperty(newProp);
			if(IsRequireChangeNodeType(newProperty)) {
				ClauseNode newNode;
				if(newProperty.IsList) {
					newNode = Model.CreateAggregateNode();
				} else {
					newNode = Model.CreateClauseNode();
				}
				newNode.FirstOperand = newProp;
				GroupNode groupNode = (GroupNode)ParentNode;
				groupNode.ReplaceClauseNodes(this, newNode);
				newNode.ValidateAdditionalOperands();
				FocusInfo = new FilterControlFocusInfo(newNode, 0);
			} else {
				ClauseType oldDefaultOp = Model.GetDefaultOperation(FilterProperties, FirstOperand);
				FirstOperand = newProp;
				if (oldDefaultOp == Operation || (newProperty != null && !Model.IsValidClause(newProperty, Operation))) {
					Operation = Model.GetDefaultOperation(FilterProperties, newProp);
					ValidateAdditionalOperands();
				}
				UpdateAdditionalOperands();
				FocusInfo = new FilterControlFocusInfo(this, elementIndex + 1);
			}
			Model.EndUpdate(FilterChangedAction.FieldNameChange, this);
		}
		public void UpdateAdditionalOperands() {
			Type propertyType = GetPropertyForEditing() != null ? GetPropertyForEditing().Type : null;
			for (int i = 0; i < AdditionalOperands.Count; i++) {
				CriteriaOperator operand = AdditionalOperands[i];
				if (operand is OperandParameter) {
					Type parameterType = null;
					if (propertyType != null) {
						foreach (IFilterParameter parameter in Model.GetParametersByType(propertyType)) {
							if (string.Compare(parameter.Name, (operand as OperandParameter).ParameterName) == 0) {
								parameterType = parameter.Type;
								break;
							}
						}
					}
					if (parameterType == null || !TypeConvertionValidator.CanConvertType(parameterType, propertyType)) {
						AdditionalOperands[i] = CreateDefaultParameter();
					}
				} else if (operand is OperandValue) {
					object value = (operand as OperandValue).Value;
					object newValue;
					OperandValue newOperandValue = new OperandValue();
					if(value != null && TypeConvertionValidator.TryConvert(value, propertyType, out newValue)) {
						newOperandValue.Value = newValue;
					}
					AdditionalOperands[i] = newOperandValue;
				} else if (operand is OperandProperty) {
					IBoundProperty property = FilterProperties.GetProperty(operand as OperandProperty);
					if (property == null || !TypeConvertionValidator.CanConvertType(property.Type, propertyType)) {
						AdditionalOperands[i] = CreateDefaultProperty();
					}
				} else if(operand is FunctionOperator) {
					FunctionOperator fo = (FunctionOperator)operand;
					Type functionType = Model.GetFunctionType(fo.ToString());
					if(!TypeConvertionValidator.CanConvertType(functionType, propertyType)) {
						AdditionalOperands[i] = CreateDefaultProperty();
					}
				}
			}
		}
		public void ValidateAdditionalOperands() {
			FilterControlHelpers.ValidateAdditionalOperands(Operation, AdditionalOperands);
		}
		public OperandParameter CreateDefaultParameter() {
			OperandParameter defaultParameter = null;
			if (Property != null) {
				List<IFilterParameter> parameters = Model.GetParametersByType(Property.Type);
				if (parameters.Count > 0) {
					defaultParameter = new OperandParameter(parameters[0].Name);
				}
			}
			if (object.ReferenceEquals(defaultParameter, null) && Model.CanAddParameters) {
				defaultParameter = new OperandParameter();
			}
			return defaultParameter;
		}
		public OperandProperty CreateDefaultProperty() {
			return Property != null ? FirstOperand.Clone() : FilterProperties.CreateDefaultProperty(Model.GetDefaultProperty());
		}
		public OperandProperty FirstOperand {
			get { return _FirstOperand; }
			set {
				_FirstOperand = value;
				NodeChanged(FilterChangedAction.FieldNameChange);
			}
		}
		public ClauseType Operation {
			get { return _Operation; }
			set {
				_Operation = value;
				NodeChanged(FilterChangedAction.OperationChanged);
			}
		}
		public IList<CriteriaOperator> AdditionalOperands { get { return _AdditionalOperands; } }
		public void AdditionalOperands_AddRange(IEnumerable<CriteriaOperator> operands) {
			this.disableValueChangedNotification = true;
			foreach (CriteriaOperator value in operands) {
				AdditionalOperands.Add(value);
			}
			this.disableValueChangedNotification = false;
			NodeChanged(FilterChangedAction.ValueChanged);
		}
		public override CriteriaOperator GetAdditionalOperand(int elementIndex) {
			NodeEditableElement element = GetElement(elementIndex);
			if (element != null && element.ValueIndex < 0) return null;
			return AdditionalOperands[element.ValueIndex];
		}
		public override object GetCurrentValue(int elementIndex) {
			OperandValue operandValue = GetAdditionalOperand(elementIndex) as OperandValue;
			if (!ReferenceEquals(operandValue, null)) return operandValue.Value;
			NodeEditableElement element = GetElement(elementIndex);
			if (element != null && element.ElementType == ElementType.ItemCollection) {
				List<object> list = new List<object>();
				foreach (OperandValue value in AdditionalOperands) {
					list.Add(value.Value);
				}
				return list;
			}
			return null;
		}
		public virtual bool IsList { get { return false; } }
		public override string Text {
			get {
				string str = FirstOperand.ToString() + " " + Operation.ToString();
				foreach (CriteriaOperator op in AdditionalOperands) {
					str += " " + op.ToString();
				}
				return str;
			}
		}
		public bool ShowOperandTypeIcon {
			get { return Model.ShowOperandTypeIcon; }
		}
		public bool ShowParameterTypeIcon { get { return Model.ShowParameterTypeIcon; } }
		public bool ShowTypeIcon { get { return ShowOperandTypeIcon || ShowParameterTypeIcon; } }
		public void SwapAdditionalOperandType(int index, OperandProperty defaultProperty) {
			SwapAdditionalOperandType(index, defaultProperty, null);
		}
		public void SwapAdditionalOperandType(int index, OperandProperty defaultProperty, OperandParameter defaultParameter) {
			if (!ShowTypeIcon) return;
			CriteriaOperator operand = AdditionalOperands[index];
			if (operand is OperandParameter) {
				operand = new OperandValue();
			} else if (operand is OperandValue) {
				if (ShowOperandTypeIcon) {
					operand = defaultProperty;
				} else if (!object.ReferenceEquals(defaultParameter, null)) {
					operand = defaultParameter;
				}
			} else {
				if (ShowParameterTypeIcon) {
					operand = !object.ReferenceEquals(defaultParameter, null) ? defaultParameter : new OperandValue();
				} else {
					operand = new OperandValue();
				}
			}
			AdditionalOperands[index] = operand;
		}
		protected override object Accept(INodeVisitor visitor) {
			return visitor.Visit(this);
		}
	}
	public class AggregateNode : ClauseNode, IAggregateNode {
		Aggregate _Aggregate;
		OperandProperty _AggregateOperand;
		INode _AggregateCondtion;
		IList<INode> children;
		IBoundPropertyCollection childrenFilterProperties;
		public AggregateNode(FilterTreeNodeModel model)
			: base(model) {
			children = CreateObserveblaListForChildNodes();
			CreateAggregateCondtion();
		}
		public override bool IsList { get { return true; } }
		public Aggregate Aggregate {
			get { return _Aggregate; }
			set {
				_Aggregate = value;
				NodeChanged(FilterChangedAction.AggregateOperationChanged);
			}
		}
		public OperandProperty AggregateOperand {
			get { return _AggregateOperand; }
			set { _AggregateOperand = value; }
		}
		public INode AggregateCondition {
			get { return _AggregateCondtion; }
			set {
				if (AggregateCondition == value) return;
				_AggregateCondtion = value;
				AddAggregationToChildren();
			}
		}
		protected IBoundProperty GetChildByCaption(string caption) {
			if (Property == null || Property.Children == null) return null;
			foreach (IBoundProperty child in Property.Children) {
				if (caption == child.DisplayName) return child;
			}
			return null;
		}
		protected IBoundProperty GetChildByName(string name) {
			if (Property == null || Property.Children == null) return null;
			foreach (IBoundProperty child in Property.Children) {
				if (name == child.Name) return child;
			}
			return null;
		}
		public IBoundProperty AggregateProperty {
			get {
				if (ReferenceEquals(AggregateOperand, null) || Property == null) return null;
				IBoundProperty property = GetChildByCaption(AggregateOperand.PropertyName);
				if (property != null) return property;
				property = GetChildByName(AggregateOperand.PropertyName);
				if (property != null) return property;
				return FilterProperties[Property.GetFullName() + '.' + AggregateOperand.PropertyName];
			}
		}
		public override IList<INode> GetChildren() { return children; }
		void CreateAggregateCondtion() {
			if (Model.AllowAggregateEditing == FilterControlAllowAggregateEditing.AggregateWithCondition) {
				AggregateCondition = Model.CreateGroupNode(null);
			}
		}
		void AddAggregationToChildren() {
			this.children.Clear();
			if (AggregateCondition != null) {
				((Node)AggregateCondition).SetParentNode(this);
				this.children.Add(AggregateCondition);
			}
		}
		public override IBoundProperty GetPropertyForEditing() { return AggregateProperty; }
		protected override IBoundPropertyCollection GetChildrenFilterProperties() {
			if (this.childrenFilterProperties == null) {
				if (Property != null && Property.Children != null && Property.Children.Count > 0) {
					this.childrenFilterProperties = FilterProperties.CreateChildrenProperties(Property);
				}
				if (this.childrenFilterProperties == null) {
					this.childrenFilterProperties = FilterProperties;
				}
			}
			return this.childrenFilterProperties;
		}
		protected override void ClauseNodeFirstOperandChanged(OperandProperty newProp, int elementIndex) {
			this.childrenFilterProperties = null;
			base.ClauseNodeFirstOperandChanged(newProp, elementIndex);
		}
		protected override bool IsRequireChangeNodeType(IBoundProperty newProperty) {
			return true;
		}
		protected override Type GetValueType() {
			if (Aggregate == Aggregate.Count) return typeof(int);
			return base.GetValueType();
		}
		public List<IBoundProperty> GetAvailableAggregateProperties() {
			return GetAvailableAggregateProperties(Aggregate);
		}
		public List<Aggregate> GetAvailableAggregateOperations() {
			List<Aggregate> list = new List<Aggregate>();
			list.Add(Aggregate.Exists);
			list.Add(Aggregate.Count);
			if (Property == null || Property.Children == null || Property.Children.Count == 0) return list;
			list.Add(Aggregate.Max);
			list.Add(Aggregate.Min);
			if (GetAvailableAggregateProperties(Aggregate.Sum).Count > 0) {
				list.Add(Aggregate.Sum);
				list.Add(Aggregate.Avg);
			}
			return list;
		}
		public override List<ClauseType> GetAvailableOperations() {
			IBoundProperty property = AggregateProperty;
			if (property != null) return GetAvailableOperations(property);
			List<ClauseType> list = new List<ClauseType>();
			if (Aggregate == Aggregate.Exists) return list;
			foreach (ClauseType type in Enum.GetValues(typeof(ClauseType))) {
				if (Model.IsValidClause(type, FilterColumnClauseClass.Generic))
					list.Add(type);
			}
			return list;
		}
		protected List<IBoundProperty> GetAvailableAggregateProperties(Aggregate forAggregate) {
			List<IBoundProperty> list = new List<IBoundProperty>();
			if (Property == null || Property.Children == null) return list;
			if (forAggregate == Aggregate.Count || forAggregate == Aggregate.Exists) return list;
			if (forAggregate == Aggregate.Max || forAggregate == Aggregate.Min) {
				foreach (IBoundProperty boundProperty in Property.Children) {
					list.Add(boundProperty);
				}
			} else {
				foreach (IBoundProperty property in Property.Children) {
					if (SummaryItemTypeHelper.IsNumericalType(property.Type)) {
						list.Add(property);
					}
				}
			}
			list.Sort((l, r) => Comparer.Default.Compare(l.DisplayName, r.DisplayName));
			return list;
		}
		public override void RebuildElements() {
			Elements.Clear();
			AddEditableElement(ElementType.Property, GetDisplayText(FirstOperand)).TextAfter = " ";
			NodeEditableElement aggregationOperation = AddEditableElement(ElementType.AggregateOperation, Model.GetMenuStringByType(Aggregate));
			if (Aggregate == Aggregate.Exists) {
				AddEditableElement(ElementType.NodeRemove, "@-");
				return;
			}
			if (Aggregate == Aggregate.Count) {
				aggregationOperation.TextAfter = " ";
			} else {
				NodeEditableElement aggregatePropertyElement = AddEditableElement(ElementType.AggregateProperty, GetAggregatedDisplayText(AggregateOperand));
				aggregatePropertyElement.TextBefore = "(";
				aggregatePropertyElement.TextAfter = ") ";
				aggregatePropertyElement.IsEmpty = ReferenceEquals(AggregateOperand, null);
			}
			BuildOperationAndValueElements();
		}
		protected IBoundProperty GetChildrenFilterProperty(OperandProperty property) {
			if (ReferenceEquals(property, null) || Property == null || Property.Children == null) return null;
			foreach (IBoundProperty each in Property.Children) {
				if (each.Name == property.PropertyName) return each;
			}
			return null;
		}
		protected string GetAggregatedDisplayText(OperandProperty property) {
			if (ReferenceEquals(property, null)) return Model.GetLocalizedStringForFilterEmptyValue();
			IBoundProperty childProperty = GetChildrenFilterProperty(property);
			if (childProperty == null) return property.ToString();
			return new OperandProperty(childProperty.DisplayName).ToString();
		}
		protected override void ChangeElement(NodeEditableElement element, object value) {
			FilterControlFocusInfo fi;
			switch (element.ElementType) {
				case ElementType.AggregateOperation:
					Model.BeginUpdate();
					Aggregate = (Aggregate)value;
					ValidateAggregate();
					fi = FocusInfo.OnRight();
					if (fi.Node == FocusInfo.Node)
						FocusInfo = fi;
					Model.EndUpdate(FilterChangedAction.AggregateOperationChanged, this);
					break;
				case ElementType.AggregateProperty:
					Model.BeginUpdate();
					string propertyName;
					if (value is OperandProperty) {
						propertyName = ((OperandProperty)value).PropertyName;
					} else if (value is IBoundProperty) {
						propertyName = (value as IBoundProperty).Name;
					} else {
						propertyName = value.ToString();
					}
					AggregateOperand = GetAggregateProperty(propertyName);
					ValidateAggregateProperty();
					fi = FocusInfo.OnRight();
					if (fi.Node == FocusInfo.Node)
						FocusInfo = fi;
					Model.EndUpdate(FilterChangedAction.AggregatePropertyChanged, this);
					break;
			}
			base.ChangeElement(element, value);
		}
		OperandProperty GetAggregateProperty(string propertyName) {
			if (Property == null) return new OperandProperty(propertyName);
			string collectionProperty = Property.ToString() + '.';
			if (propertyName.StartsWith(collectionProperty)) {
				propertyName = propertyName.Remove(0, collectionProperty.Length);
			}
			return new OperandProperty(propertyName);
		}
		protected void ValidateAggregate() {
			if (Aggregate == Aggregate.Exists) {
				AdditionalOperands.Clear();
			}
			List<IBoundProperty> properties = GetAvailableAggregateProperties();
			if (properties.Count == 0) {
				AggregateOperand = null;
			} else {
				OperandProperty newProperty = GetAggregateProperty(properties[0].ToString());
				if (!ReferenceEquals(AggregateOperand, null)) {
					foreach (IBoundProperty property in properties) {
						OperandProperty aggregateProperty = GetAggregateProperty(property.ToString());
						if (aggregateProperty.PropertyName == AggregateOperand.PropertyName) {
							newProperty = aggregateProperty;
							break;
						}
					}
				}
				AggregateOperand = newProperty;
			}
			ValidateAggregateProperty();
		}
		protected void ValidateAggregateProperty() {
			ValidateOperation();
			ValidateAdditionalOperands();
			UpdateAdditionalOperands();
		}
		protected void ValidateOperation() {
			if (Aggregate == Aggregate.Exists) {
				Operation = ClauseType.IsNull;
			} else {
				ClauseType oldOperation = Operation;
				if (oldOperation == ClauseType.IsNull || oldOperation == ClauseType.IsNotNull || GetAvailableOperations().IndexOf(oldOperation) < 0) {
					Operation = ClauseType.Greater;
				}
			}
		}
		protected override object Accept(INodeVisitor visitor) {
			return visitor.Visit(this);
		}
	}
	public class FilterControlNodesFactory : INodesFactory, INodesFactoryEx {
		FilterTreeNodeModel model;
		public FilterControlNodesFactory(FilterTreeNodeModel model) {
			this.model = model;
		}
		protected internal FilterTreeNodeModel Model { get { return model; } }
		public IClauseNode Create(ClauseType type, OperandProperty firstOperand, ICollection<CriteriaOperator> operands) {
			try {
				Model.BeginUpdate();
				ClauseNode clauseNode = Model.CreateClauseNode();
				SetClauseNodeValues(clauseNode, type, firstOperand, operands);
				return clauseNode;
			} finally {
				Model.EndUpdate();
			}
		}
		public IGroupNode Create(GroupType type, ICollection<INode> subNodes) {
			try {
				Model.BeginUpdate();
				GroupNode groupNode = Model.CreateGroupNode();
				groupNode.NodeType = type;
				foreach (INode subNode in subNodes) {
					groupNode.AddNode(subNode);
				}
				return groupNode;
			} finally {
				Model.EndUpdate();
			}
		}
		protected void SetClauseNodeValues(ClauseNode node, ClauseType type, OperandProperty firstOperand, ICollection<CriteriaOperator> operands) {
			node.Operation = type;
			node.FirstOperand = firstOperand;
			if (operands != null) {
				foreach (CriteriaOperator op in operands)
					node.AdditionalOperands.Add(op);
			}
		}
		public IAggregateNode Create(OperandProperty firstOperand, Aggregate aggregate, OperandProperty aggregateOperand, ClauseType operation, ICollection<CriteriaOperator> operands, INode conditionNode) {
			try {
				Model.BeginUpdate();
				AggregateNode aggregateNode = Model.CreateAggregateNode();
				SetClauseNodeValues(aggregateNode, operation, firstOperand, operands);
				aggregateNode.Aggregate = aggregate;
				aggregateNode.AggregateOperand = aggregateOperand;
				IGroupNode conditionGroupNode = conditionNode as IGroupNode;
				if (conditionGroupNode == null) {
					ICollection<INode> subNodes = new List<INode>();
					if (conditionNode != null) {
						subNodes.Add(conditionNode);
					}
					conditionNode = Create(GroupType.And, subNodes);
				}
				aggregateNode.AggregateCondition = conditionNode;
				return aggregateNode;
			} finally {
				Model.EndUpdate();
			}
		}
	}
}
namespace DevExpress.Data.Filtering {
	using Compatibility.System.ComponentModel;
	using Compatibility.System.Data;
	using DevExpress.Data.Browsing;
	using DevExpress.Data.Browsing.Design;
	using DevExpress.XtraEditors.Filtering;
	public interface IIBoundPropertyCreator {
		IBoundProperty CreateProperty(object dataSource, string dataMember, string displayName, bool isList, PropertyDescriptor property);
		void SetParent(IBoundProperty property, IBoundProperty parent);
	}
	internal class SourceControlNotifier : IDisposable {
		object sourceControl;
		public object SourceControl {
			get { return sourceControl; }
			set {
				Unsubscribe();
				sourceControl = value;
				Subscribe();
			}
		}
		#region IDisposable Members
		public void Dispose() {
			Unsubscribe();
		}
		#endregion
		void Subscribe() {
			if (this.sourceControl is DataTable) {
				DataTable asDataTable = this.sourceControl as DataTable;
				asDataTable.Columns.CollectionChanged += new CollectionChangeEventHandler(OnSourceControlPropertiesChanged);
			}
			if (this.sourceControl is IBindingList) {
				IBindingList asIBindingList = this.sourceControl as IBindingList;
				asIBindingList.ListChanged += new ListChangedEventHandler(OnSourceControlPropertiesChanged);
			}
		}
		void Unsubscribe() {
			if (sourceControl is DataTable) {
				DataTable asDataTable = this.sourceControl as DataTable;
				asDataTable.Columns.CollectionChanged -= new CollectionChangeEventHandler(OnSourceControlPropertiesChanged);
			}
			if (this.sourceControl is IBindingList) {
				IBindingList asIBindingList = this.sourceControl as IBindingList;
				asIBindingList.ListChanged -= new ListChangedEventHandler(OnSourceControlPropertiesChanged);
			}
		}
		void OnSourceControlPropertiesChanged(object sender, EventArgs e) {
			if (e is ListChangedEventArgs) {
				var args = e as ListChangedEventArgs;
				if (args.ListChangedType == ListChangedType.PropertyDescriptorAdded
					|| args.ListChangedType == ListChangedType.PropertyDescriptorChanged
					|| args.ListChangedType == ListChangedType.PropertyDescriptorDeleted)
					RaiseOnPropertiesChanged();
			} else {
				RaiseOnPropertiesChanged();
			}
		}
		void RaiseOnPropertiesChanged() {
			if (OnPropertiesChanged != null)
				OnPropertiesChanged();
		}
		public delegate void PropertyChangedDelegate();
		public event PropertyChangedDelegate OnPropertiesChanged;
	}
	public class FilterModelPropertyNode : INode {
		FilterModelPropertyNode parent;
		IBoundProperty property;
		List<FilterModelPropertyNode> children = new List<FilterModelPropertyNode>();
		bool isDataSource = false, isDummy = false;
		public FilterModelPropertyNode(bool isDataSource, bool isDummy) {
			this.isDataSource = isDataSource;
			this.isDummy = isDummy;
		}
		public FilterModelPropertyNode(IBoundProperty property) : this(property, null) { }
		public FilterModelPropertyNode(IBoundProperty property, FilterModelPropertyNode parent) {
			this.property = property;
			this.parent = parent;
		}
		public IBoundProperty Property { get { return property; } }
		#region INode Members
		public IList ChildNodes { get { return this.children; } }
		public string DataMember { get { return IsDataMemberNode ? Property.Name : string.Empty; } }
		public void Expand(EventHandler callback) {
			if (callback != null)
				callback(this, EventArgs.Empty);
		}
		public bool HasDataSource(object dataSource) { return false; }
		public bool IsDataMemberNode { get { return Property != null; } }
		public bool IsDataSourceNode { get { return isDataSource; } }
		public bool IsDummyNode { get { return isDummy; } }
		public bool IsEmpty { get { return !IsDataMemberNode; } }
		public bool IsList { get { return false; } }
		public bool IsComplex { get { return false; } }
		public object Parent { get { return parent; } }
		#endregion
	}
	public class FilterModelPickManager : PickManagerBase {
		IIBoundPropertyCreator propertyCreator;
		public FilterModelPickManager(IIBoundPropertyCreator propertyCreator) {
			this.propertyCreator = propertyCreator;
		}
		protected override INode CreateDataMemberNode(object dataSource, string dataMember, string displayName, bool isList, object owner, IPropertyDescriptor property) {
			return new FilterModelPropertyNode(propertyCreator.CreateProperty(dataSource, dataMember, displayName, isList, ((FakedPropertyDescriptor)property).RealProperty));
		}
		protected override INode CreateDataSourceNode(object dataSource, string dataMember, string name, object owner) {
			return new FilterModelPropertyNode(true, false);
		}
		protected override INode CreateDummyNode(object owner) {
			return new FilterModelPropertyNode(false, true);
		}
		protected override object CreateNoneNode(object owner) {
			return new FilterModelPropertyNode(false, true);
		}
		protected override IPropertiesProvider CreateProvider() {
			return new FilteringPropertiesProvider();
		}
		protected override bool NodeIsEmpty(INode node) {
			return node != null;
		}
		public List<IBoundProperty> PickProperties(object dataSource, string dataMember, Type dataMemberType) {
			if (dataMemberType != null) {
				if (dataMemberType == typeof(DateTime) ||
					dataMemberType == typeof(string) ||
					dataMemberType.IsPrimitive())
				{
					return new List<IBoundProperty>();
				}
			}
			if (dataSource is Type) {
				var d1 = typeof(List<>);
				Type[] typeArgs = { (Type)dataSource };
				var makeme = d1.MakeGenericType(typeArgs);
				dataSource = Activator.CreateInstance(makeme);
			}
			List<FilterModelPropertyNode> nodeList = new List<FilterModelPropertyNode>();
			FillNodes(dataSource, dataMember, nodeList);
			if (nodeList.Count == 0)
				return null;
			Debug.Assert(nodeList.Count == 1);
			FilterModelPropertyNode root = nodeList[0];
			List<IBoundProperty> list = new List<IBoundProperty>();
			foreach (FilterModelPropertyNode child in root.ChildNodes) {
				list.Add(child.Property);
			}
			return list;
		}
	}
	class FilteringPropertiesProvider : PropertiesProvider {
		public FilteringPropertiesProvider() : base(new DataContext(true), null) {
		}
		protected override void SortProperties(IPropertyDescriptor[] properties) {
		}
	}
}

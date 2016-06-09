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
using System.Linq;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Utils;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Collections;
using DevExpress.Xpf.Core;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Data;
namespace DevExpress.Xpf.Grid {
	internal class RootTreeListNode : TreeListNode {
		public static bool IsRootNode(TreeListNode node) { return node is RootTreeListNode; }
		public RootTreeListNode(TreeListDataProvider provider)
			: base() {
			DataProvider = provider;
		}
		public override bool IsExpanded { get { return true; } set { } }
	}
	public class TreeListNode : INotifyPropertyChanged {
		readonly TreeListNodeCollection nodes;
		protected bool isExpandedCore;
		Binding expandStateBindingCore; 
		object content;
		protected internal TreeListNode parentNodeCore;
		internal DefaultBoolean isExpandButtonVisible = DefaultBoolean.Default;
		internal int rowHandle;
		internal int visibleIndex;
		ImageSource image;
		bool? isChecked;
		bool isCheckBoxEnabled;
		internal bool isCheckStateInitialized;
		internal Locker CheckBoxUpdateLocker;
		ExpandStateBindingEvaluator expandStateBindingEvaluatorCore;
		TreeListDataProvider dataProviderCore;
		public TreeListNode() {
			nodes = CreateNodeCollection();
			IsVisible = true;
			Id = visibleIndex = -1;
			rowHandle = GridControl.InvalidRowHandle;
			isChecked = false;
			isCheckBoxEnabled = true;
			isCheckStateInitialized = false;
			CheckBoxUpdateLocker = new Locker();
			ChildrenWereEverFetched = false;
		}
		public TreeListNode(object content)
			: this() {
			Content = content;
		}
		protected virtual TreeListNodeCollection CreateNodeCollection() {
			return new TreeListNodeCollection(this);
		}
		protected internal virtual TreeListDataProvider DataProvider {
			get { return dataProviderCore; }
			set {
				if(DataProvider == value) return;
				if(DataProvider != null)
					ReleaseExpandStateBindingEvaluator();
				dataProviderCore = value;
				if(DataProvider != null)
					UpdateExpandStateBindingEvaluator();
			} 
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeNodes")]
#endif
		public TreeListNodeCollection Nodes { get { return nodes; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeContent")]
#endif
		public object Content {
			get { return content; }
			set {
				if(ReferenceEquals(Content, value))
					return;
				content = value;
				UpdateExpandStateBindingEvaluator();
				NotifyDataProvider(NodeChangeType.Content);
				RaisePropertyChanged("Content");
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeIsExpandButtonVisible")]
#endif
		public DefaultBoolean IsExpandButtonVisible {
			get { return isExpandButtonVisible; }
			set {
				if(IsExpandButtonVisible == value)
					return;
				isExpandButtonVisible = value;
				NotifyDataProvider(NodeChangeType.ExpandButtonVisibility);
				RaisePropertyChanged("IsExpandButtonVisible");
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeImage")]
#endif
		public ImageSource Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				image = value;
				NotifyDataProvider(NodeChangeType.Image);
				RaisePropertyChanged("Image");
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeIsChecked")]
#endif
		public bool? IsChecked {
			get { return isChecked; }
			set {
				if(!SetNodeChecked(this, value))
					return;
				if(DataProvider != null && DataProvider.IsRecursiveCheckingAllowed(this)) {
					RecursiveCheckChildren(this);
					RecursiveCheckParents(this);
				}
				NotifyDataProvider(NodeChangeType.CheckBox);
				RaisePropertyChanged("IsChecked");
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeIsCheckBoxEnabled")]
#endif
		public bool IsCheckBoxEnabled {
			get { return isCheckBoxEnabled; }
			set {
				if(isCheckBoxEnabled == value)
					return;
				isCheckBoxEnabled = value;
				NotifyDataProvider(NodeChangeType.IsCheckBoxEnabled);
				RaisePropertyChanged("IsCheckBoxEnabled");
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeHasChildren")]
#endif
		public bool HasChildren {
			get { return nodes.Count > 0; }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeHasVisibleChildren")]
#endif
		public bool HasVisibleChildren {
			get { return nodes.GetFirstVisible() != null; }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeIsExpanded")]
#endif
		public virtual bool IsExpanded {
			get { return isExpandedCore; }
			set {
				if(IsExpanded == value || !this.IsTogglable) return;
				Expand(value);
				RaisePropertyChanged("IsExpanded");
			}
		}
		protected internal bool IsExpandedSetInternally { get; internal set; }
		public virtual Binding ExpandStateBinding {
			get { return expandStateBindingCore; }
			set {
				if(ExpandStateBinding == value)
					return;
				expandStateBindingCore = value;
				UpdateExpandStateBindingEvaluator();
				ForceUpdateExpandState();
			}
		}
		internal void ReleaseExpandStateBindingEvaluator() {
			if(expandStateBindingEvaluatorCore != null) {
				expandStateBindingEvaluatorCore.Release();
				expandStateBindingEvaluatorCore = null;
			}
		}
		internal void UpdateExpandStateBindingEvaluator() {
			ReleaseExpandStateBindingEvaluator();
			if(Content == null || DataProvider == null) return;
			if(ExpandStateBinding == null && DataProvider.View.ExpandStateBinding == null && String.IsNullOrEmpty(DataProvider.View.ExpandStateFieldName)) return;
			if(DataProvider.View.IsDesignTime) return;
			expandStateBindingEvaluatorCore = new ExpandStateBindingEvaluator(this, DataProvider.View.ExpandStateFieldName, ExpandStateBinding ?? DataProvider.View.ExpandStateBinding);
		}
		internal void Expand(bool isExpanded) {
			if(isExpandedCore == isExpanded) return;
			if(DataProvider != null) {
				if(DataProvider.OnNodeExpandingOrCollapsing(this)) {
					isExpandedCore = isExpanded;
					if(expandStateBindingEvaluatorCore != null && expandStateBindingEvaluatorCore.IsTwoWayBinding)
						expandStateBindingEvaluatorCore.SetExpanded(isExpandedCore);
					if(DataProvider != null)
						DataProvider.OnNodeExpandedOrCollapsed(this);
					NotifyDataProvider(NodeChangeType.Expand);
				}
			}
			else isExpandedCore = isExpanded;
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeParentNode")]
#endif
		public TreeListNode ParentNode {
			get {
				if(parentNodeCore is RootTreeListNode)
					return null;
				return parentNodeCore;
			}
			internal set {
				parentNodeCore = value;
			}
		}
		protected internal TreeListNode RootNode {
			get {
				TreeListNode parent = this;
				while(parent.ParentNode != null)
					parent = parent.ParentNode;
				return parent;
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeVisibleParent")]
#endif
		public TreeListNode VisibleParent {
			get {
				TreeListNode parent = ParentNode;
				while(parent != null) {
					if(parent.IsVisible) break;
					parent = parent.ParentNode;
				}
				return parent;
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeTag")]
#endif
		public object Tag { get; set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeLevel")]
#endif
		public int Level {
			get {
				if(ParentNode == null)
					return 0;
				return ParentNode.Level + 1;
			}
		}
		public int ActualLevel {
			get {
				if(!IsVisible)
					return -1;
				int actualLevel = Level;
				TreeListNode parent = ParentNode;
				while(parent != null) {
					if(!parent.IsVisible) actualLevel--;
					parent = parent.ParentNode;
				}
				return actualLevel;
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeIsFirst")]
#endif
		public bool IsFirst {
			get {
				if(parentNodeCore == null)
					return true;
				return parentNodeCore.Nodes[0] == this;
			}
		}
		internal bool IsFirstVisible {
			get {
				if(parentNodeCore == null)
					return true;
				else if(VisibleParent == null && DataProvider != null) {
					return DataProvider.RootNode.Nodes.GetFirstVisible() == this;
				}
				else
					return VisibleParent.Nodes.GetFirstVisible() == this;
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeIsLast")]
#endif
		public bool IsLast {
			get {
				if(ParentNode == null)
					return true;
				TreeListNodeCollection nodes = parentNodeCore.Nodes;
				return nodes[nodes.Count - 1] == this;
			}
		}
		internal bool IsLastVisible {
			get {
				if(parentNodeCore == null)
					return true;
				else if(VisibleParent == null && DataProvider != null) {
					return DataProvider.RootNode.Nodes.GetLastVisible() == this;
				}
				else
					return VisibleParent.Nodes.GetLastVisible() == this;
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeRowHandle")]
#endif
		public int RowHandle {
			get {
				if(DataProvider != null)
					return DataProvider.GetRowHandleByNode(this);
				return rowHandle;
			}
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListNodeIsFiltered")]
#endif
		public bool IsFiltered { get { return !IsVisible; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public IList ItemsSource { get; internal set; }
		protected internal bool IsVisible  { get; internal set; }
		internal int Id { get; set; }
		DataTemplate itemTemplateCore;
		internal DataTemplate ItemTemplate {
			get { return itemTemplateCore; }
			set {
				if(ItemTemplate == value)
					return;
				itemTemplateCore = value;
				RaisePropertyChanged("ItemTemplate");
			}
		}
		DataTemplate templateCore;
		internal DataTemplate Template {
			get { return templateCore; }
			set {
				if(Template == value)
					return;
				templateCore = value;
				RaisePropertyChanged("Template");
			}
		}
		internal int VisibleIndex {
			get {
				if(DataProvider != null)
					return DataProvider.GetVisibleIndexByNode(this);
				return visibleIndex;
			}
		}
		protected internal bool IsTogglable {
			get {
				if(isExpandButtonVisible == DevExpress.Utils.DefaultBoolean.Default)
					return HasVisibleChildren;
				else
					return true;
			}
		}
		internal bool ChildrenWereEverFetched { get; set; }
		public void ExpandAll() {
			ToggleExpandedAllChildren(true);
		}
		public void CollapseAll() {
			ToggleExpandedAllChildren(false);
		}
		void RecursiveCheckParents(TreeListNode node) {
			TreeListNode parentNode = node.ParentNode;
			if(parentNode == null) return;
			bool? checkStatus = parentNode.Nodes[0].IsChecked;
			foreach(TreeListNode child in parentNode.Nodes) {
				if(child.isChecked != checkStatus) {
					checkStatus = null;
					break;
				}
			}
			if(SetNodeChecked(parentNode, checkStatus))
				RecursiveCheckParents(parentNode);
		}
		void RecursiveCheckChildren(TreeListNode node) {
			foreach(TreeListNode child in new TreeListNodeIterator(node))
				SetNodeChecked(child, node.isChecked);
		}
		internal bool SetNodeChecked(TreeListNode node, bool? checkStatus) {
			if(node.isChecked == checkStatus)
				return false;
			node.isChecked = checkStatus;
			TreeListDataProvider provider = DataProvider;
			if(provider != null && !provider.View.IsDesignTime && !provider.DataHelper.IsLoading) {
				provider.SetObjectIsChecked(node, checkStatus);
				if(isCheckStateInitialized)
					provider.OnNodeCheckStateChanged(node);
			}
			return true;
		}
		internal void InitIsChecked() {
			if(DataProvider == null)
				return;
			isCheckStateInitialized = false;
			IsChecked = DataProvider.GetObjectIsChecked(this);
			isCheckStateInitialized = true;
		}
		internal void UpdateNodeChecked(bool? checkStatus) {
			if(IsChecked == checkStatus) {
				isCheckStateInitialized = true;
				return;
			}
			if(DataProvider != null && DataProvider.IsRecursiveCheckingAllowed(this))
				IsChecked = checkStatus;
			else
				CheckBoxUpdateLocker.DoLockedAction(delegate { IsChecked = checkStatus; });
		}
		void ToggleExpandedAllChildren(bool expand) {
			if(DataProvider != null)
				DataProvider.ToggleExpandedAllChildNodes(this, expand);
			else
				ToggleExpandedAllChildrenCore(expand);
		}
		internal void ToggleExpandedAllChildrenCore(bool expand) {
			ProcessNodeAndDescendantsAction((node) =>
			{
				node.IsExpanded = expand;
				return true;
			});
		}
		protected void NotifyDataProvider(NodeChangeType nodeChangeType) {
			if(DataProvider == null)
				return;
			DataProvider.OnNodeCollectionChanged(this, nodeChangeType);
		}
		[Browsable(false)]
		public bool IsDescendantOf(TreeListNode node) {
			TreeListNode parent = this.ParentNode;
			while(parent != null) {
				if(ReferenceEquals(parent, node))
					return true;
				parent = parent.ParentNode;
			}
			return false;
		}
		internal void UpdateId() {
			if(DataProvider != null)
				DataProvider.UpdateNodeId(this);
		}
		internal void ProcessNodeAndDescendantsAction(Func<TreeListNode, bool> action) {
			ProcessNodeAction(this, action);
		}
		internal static void ProcessNodeAction(TreeListNode node, Func<TreeListNode, bool> action) {
			if(!action.Invoke(node)) return;
			foreach(TreeListNode child in node.Nodes)
				ProcessNodeAction(child, action);
		}
		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#region inner classes
		public class ExpandStateBindingEvaluator {
			#region IExpandStateBindingStrategy
			public interface IExpandStateBindingStrategy {
				bool IsTwoWayBinding { get; }
				void UpdateExpandState();
				void SetExpanded(bool value);
				void Release();
			}
			#endregion
			public ExpandStateBindingEvaluator(TreeListNode node, string expandStateFieldName, Binding expandStateBinding) {
				Node = node;
				EvaluatorStrategy = CreateEvaluatorStrategy(expandStateFieldName, expandStateBinding);
			}
			protected TreeListNode Node { get; private set; }
			protected IExpandStateBindingStrategy EvaluatorStrategy { get; private set; }
			protected virtual IExpandStateBindingStrategy CreateEvaluatorStrategy(string expandStateFieldName, Binding expandStateBinding) {
				if((expandStateBinding == null && !string.IsNullOrEmpty(expandStateFieldName)))
					return new DataFieldExpandStateBindingStrategy(this, expandStateFieldName);
				if((string.IsNullOrEmpty(expandStateFieldName) && IsSimpleBinding(expandStateBinding)))
					return new DataFieldExpandStateBindingStrategy(this, expandStateBinding.Path.Path, expandStateBinding.Mode == BindingMode.TwoWay);
				return new BindingExpandStateBindingStrategy(this, BindingCloneHelper.Clone(expandStateBinding, Node.Content) as Binding);
			}
			protected virtual bool IsSimpleBinding(Binding expandStateBinding) {
				if(expandStateBinding.Path == null) return false;
				if(!SimpleBindingProcessor.IsFieldValid(expandStateBinding.Path.Path) || expandStateBinding.Converter != null) return false;
				return SimpleBindingProcessor.ValidateBindingProperties(expandStateBinding);
			}
			public bool IsTwoWayBinding { get { return EvaluatorStrategy.IsTwoWayBinding; } }
			public void SetExpanded(bool value) {
				EvaluatorStrategy.SetExpanded(value);
			} 
			public void UpdateExpandState() {
				EvaluatorStrategy.UpdateExpandState();
			}
			public void Release() {
				EvaluatorStrategy.Release();
				Node = null;
			}
			public class DataFieldExpandStateBindingStrategy : IExpandStateBindingStrategy {
				bool updating = false;
				bool isTwoWayBinding = false;
				public DataFieldExpandStateBindingStrategy(ExpandStateBindingEvaluator evaluator, string expandStateFieldName, bool userTwoWayBinding = true) {
					Evaluator = evaluator;
					FieldName = expandStateFieldName;
					if(Provider.Columns[FieldName] != null)
						isTwoWayBinding = !Provider.Columns[FieldName].ReadOnly && userTwoWayBinding;
				}
				public bool IsTwoWayBinding { get { return isTwoWayBinding; } }
				protected ExpandStateBindingEvaluator Evaluator { get; private set; }
				protected string FieldName { get; private set; }
				protected TreeListNode Node { get { return Evaluator.Node; } }
				protected TreeListDataProvider Provider { get { return Node.DataProvider; } }
				public void Release() {  }
				public void UpdateExpandState() {
					if(updating) return;
					updating = true;
					try {
						object value = Provider.DataHelper.GetValue(Node, FieldName);
						if(value == null || value == DBNull.Value) return;
						Node.IsExpanded = (bool)Convert.ChangeType(value, typeof(bool));
					}
					catch { }
					finally {
						updating = false;
					}
				}
				public void SetExpanded(bool value) {
					if(updating) return;
					Provider.DataHelper.SetValue(Node, FieldName, value);
				}
			}
			public class BindingExpandStateBindingStrategy : DependencyObject, IExpandStateBindingStrategy {
				static readonly DependencyProperty IsExpandedProperty = DependencyPropertyManager.Register("IsExpaned", typeof(bool), typeof(BindingExpandStateBindingStrategy), new FrameworkPropertyMetadata(false, (d, e) => ((BindingExpandStateBindingStrategy)d).UpdateExpandState()));
				public BindingExpandStateBindingStrategy(ExpandStateBindingEvaluator evaluator, Binding expandStateBinding) {
					Evaluator = evaluator;
					Binding = expandStateBinding;
					BindingOperations.SetBinding(this, BindingExpandStateBindingStrategy.IsExpandedProperty, Binding as Binding);
				}
				protected ExpandStateBindingEvaluator Evaluator { get; private set; }
				protected TreeListNode Node { get { return Evaluator.Node; } }
				protected Binding Binding { get; private set; }
				public bool IsTwoWayBinding {
					get { return Binding.Mode == BindingMode.TwoWay; }
				}
				public bool IsExpanded {
					get { return (bool)GetValue(IsExpandedProperty); }
					set { this.SetValue(IsExpandedProperty, value); }
				}
				public void Release() {
					BindingOperations.ClearBinding(this, BindingExpandStateBindingStrategy.IsExpandedProperty);
					Binding = null;
				}
				public void SetExpanded(bool value) {
					IsExpanded = value;
				}
				public void UpdateExpandState() {
					Node.IsExpanded = IsExpanded;
				}
			}
		}
		#endregion
		internal void ForceUpdateExpandState() {
			if(expandStateBindingEvaluatorCore != null && IsTogglable)
				expandStateBindingEvaluatorCore.UpdateExpandState();
		}
	}
}
namespace DevExpress.Xpf.Grid.Native {
	public static class TreeListNodeExtensions {
		public static void SetNodeExpanded(this TreeListNode node, bool expanded) {
			if(node.IsExpanded == expanded) return;
			 node.Expand(expanded);
		}
	}
}

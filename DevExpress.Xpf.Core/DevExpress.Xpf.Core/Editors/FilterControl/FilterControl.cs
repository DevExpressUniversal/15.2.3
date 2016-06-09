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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using System.Threading;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
#if !SL
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using System.Data;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Threading;
using DevExpress.Mvvm;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
namespace DevExpress.Xpf.Editors.Filtering {
	public interface IFilteredComponent : IFilteredComponentBase {
		IEnumerable<FilterColumn> CreateFilterColumnCollection();
	}
	public delegate void ShowValueEditorEventHandler(object sender, ShowValueEditorEventArgs e);
	public class ShowValueEditorEventArgs : RoutedEventArgs {
		ClauseNode node;
		OperandValue value;
		BaseEdit editor;
		public ShowValueEditorEventArgs(BaseEdit editor, ClauseNode node, OperandValue value) {
			this.editor = editor;
			this.node = node;
			this.value = value;
		}
		public virtual BaseEdit Editor {
			get { return editor; }
		}
		public ClauseNode CurrentNode {
			get { return node; }
		}
		public OperandValue OperandValue {
			get { return value; }
		}
		public ClauseType Operation {
			get {
				return CurrentNode.Operation;
			}
		}
		public BaseEditSettings CustomEditSettings { get; set; }
	}
	public partial class FilterControl : Control, IDialogContent {
		#region dependency properties fields
		public static readonly DependencyProperty FilterCriteriaProperty;
		static readonly DependencyPropertyKey RootNodePropertyKey;
		public static readonly DependencyProperty RootNodeProperty;
		public static readonly DependencyProperty SourceControlProperty;
		public static readonly DependencyProperty ShowDateTimeOperatorsProperty;
		public static readonly DependencyProperty DefaultColumnProperty;
		public static readonly DependencyProperty ShowOperandTypeIconProperty;
		public static readonly DependencyProperty ShowGroupCommandsIconProperty;
		public static readonly DependencyProperty ShowToolTipsProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty EmptyValueTemplateProperty;
		public static readonly DependencyProperty EmptyStringTemplateProperty;
		public static readonly DependencyProperty ValueTemplateProperty;
		public static readonly DependencyProperty BooleanValueTemplateProperty;
		public static readonly RoutedEvent BeforeShowValueEditorEvent;
		#endregion
		static FilterControl() {
			Type ownerType = typeof(FilterControl);
			FilterCriteriaProperty = DependencyPropertyManager.Register("FilterCriteria", typeof(CriteriaOperator), ownerType, new PropertyMetadata(null, (d, e) => ((FilterControl)d).OnFilterCriteriaChanged()));
			RootNodePropertyKey = DependencyPropertyManager.RegisterReadOnly("RootNode", typeof(GroupNode), ownerType, new PropertyMetadata(null));
			RootNodeProperty = RootNodePropertyKey.DependencyProperty;
			SourceControlProperty = DependencyPropertyManager.Register("SourceControl", typeof(object), ownerType, new PropertyMetadata(null, (d, e) => ((FilterControl)d).OnSourceControlChanged()));
			ShowDateTimeOperatorsProperty = DependencyPropertyManager.Register("ShowDateTimeOperators", typeof(bool), ownerType, new PropertyMetadata(true));
			DefaultColumnProperty = DependencyPropertyManager.Register("DefaultColumn", typeof(FilterColumn), ownerType, new PropertyMetadata(null));
			ShowOperandTypeIconProperty = DependencyPropertyManager.Register("ShowOperandTypeIcon", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowGroupCommandsIconProperty = DependencyPropertyManager.Register("ShowGroupCommandsIcon", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowToolTipsProperty = DependencyPropertyManager.Register("ShowToolTips", typeof(bool), ownerType, new PropertyMetadata(true));
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), ownerType, new PropertyMetadata(true));
			EmptyValueTemplateProperty = DependencyProperty.Register("EmptyValueTemplate", typeof(ControlTemplate), ownerType, null);
			EmptyStringTemplateProperty = DependencyProperty.Register("EmptyStringTemplate", typeof(ControlTemplate), ownerType, null);
			ValueTemplateProperty = DependencyProperty.Register("ValueTemplate", typeof(ControlTemplate), ownerType, null);
			BooleanValueTemplateProperty = DependencyProperty.Register("BooleanValueTemplate", typeof(ControlTemplate), ownerType, null);
			BeforeShowValueEditorEvent = EventManager.RegisterRoutedEvent("BeforeShowValueEditor", RoutingStrategy.Direct, typeof(ShowValueEditorEventHandler), ownerType);
		}
		#region dependency properties accessors
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("FilterControlFilterCriteria"),
#endif
Category("Options Filter")]
		public CriteriaOperator FilterCriteria {
			get { return (CriteriaOperator)GetValue(FilterCriteriaProperty); }
			set { SetValue(FilterCriteriaProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("FilterControlRootNode")]
#endif
public GroupNode RootNode {
			get { return (GroupNode)GetValue(RootNodeProperty); }
			private set { this.SetValue(RootNodePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("FilterControlSourceControl"),
#endif
Category("Options Filter")]
		public object SourceControl {
			get { return (object)GetValue(SourceControlProperty); }
			set { SetValue(SourceControlProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("FilterControlShowDateTimeOperators"),
#endif
Category("Options Filter")]
		public bool ShowDateTimeOperators {
			get { return (bool)GetValue(ShowDateTimeOperatorsProperty); }
			set { SetValue(ShowDateTimeOperatorsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("FilterControlDefaultColumn"),
#endif
Category("Options Filter")]
		public FilterColumn DefaultColumn {
			get { return (FilterColumn)GetValue(DefaultColumnProperty); }
			set { SetValue(DefaultColumnProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("FilterControlShowOperandTypeIcon"),
#endif
Category("Options Filter")]
		public bool ShowOperandTypeIcon {
			get { return (bool)GetValue(ShowOperandTypeIconProperty); }
			set { SetValue(ShowOperandTypeIconProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("FilterControlShowGroupCommandsIcon"),
#endif
Category("Options Filter")]
		public bool ShowGroupCommandsIcon {
			get { return (bool)GetValue(ShowGroupCommandsIconProperty); }
			set { SetValue(ShowGroupCommandsIconProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("FilterControlShowToolTips"),
#endif
Category("Options Filter")]
		public bool ShowToolTips {
			get { return (bool)GetValue(ShowToolTipsProperty); }
			set { SetValue(ShowToolTipsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfCoreLocalizedDescription("FilterControlShowBorder"),
#endif
Category("Options Filter")]
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("FilterControlEmptyValueTemplate")]
#endif
public ControlTemplate EmptyValueTemplate {
			get { return (ControlTemplate)GetValue(EmptyValueTemplateProperty); }
			set { SetValue(EmptyValueTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("FilterControlEmptyStringTemplate")]
#endif
public ControlTemplate EmptyStringTemplate {
			get { return (ControlTemplate)GetValue(EmptyStringTemplateProperty); }
			set { SetValue(EmptyStringTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("FilterControlValueTemplate")]
#endif
public ControlTemplate ValueTemplate {
			get { return (ControlTemplate)GetValue(ValueTemplateProperty); }
			set { SetValue(ValueTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("FilterControlBooleanValueTemplate")]
#endif
public ControlTemplate BooleanValueTemplate {
			get { return (ControlTemplate)GetValue(BooleanValueTemplateProperty); }
			set { SetValue(BooleanValueTemplateProperty, value); }
		}
		public event ShowValueEditorEventHandler BeforeShowValueEditor {
			add { AddHandler(BeforeShowValueEditorEvent, value); }
			remove { RemoveHandler(BeforeShowValueEditorEvent, value); }
		}
		#endregion
		public bool SupportDomainDataSource { get; set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("FilterControlActualFilterCriteria")]
#endif
public CriteriaOperator ActualFilterCriteria { get { return ToCriteria(RootNode); } }
		protected override bool HandlesScrolling { get { return true; } }
		internal InplaceFilterEditorOwner EditorsOwner { get; private set; }
		ImmediateActionsManager EditorsImmediateActionsManager { get; set; }
		public FilterControl() {
			ApplyFilterCommand = new DelegateCommand(ApplyFilter);
			this.SetDefaultStyleKey(typeof(FilterControl));
			EditorsImmediateActionsManager = new ImmediateActionsManager(this);
			EditorsOwner = new InplaceFilterEditorOwner(this);
			this.SetDefaultStyleKey(typeof(FilterControl));
			CreateTree(null);
			LayoutUpdated += delegate { EditorsImmediateActionsManager.ExecuteActions(); };
		}
		private IEnumerable<FilterColumn> filterColumns;
		[Browsable(false)]
		public IEnumerable<FilterColumn> FilterColumns {
			get { return filterColumns; }
			set {
				if(value == null) {
					filterColumns = new List<FilterColumn>();
				} else {
					filterColumns = value;
				}
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			focusVisualHelper = new FilterControlFocusVisualHelper(GetTemplateChild("PART_FocusVisualContainer") as Canvas, FocusVisualStyle);
#if SL
			OnApplyTemplateSLPart();
#endif
		}
		protected void OnFilterCriteriaChanged() {
			if(!object.Equals(FilterCriteria, ToCriteria(RootNode)))
				CreateTree(FilterCriteria);
		}
		protected void OnSourceControlChanged() {
			UpdateFilterSourceControl();
		}
		void UpdateFilterSourceControl() {
#if !SL
			if(SourceControl is DataTable)
				FilterSourceControl = new BindingListFilterProxy(((DataTable)SourceControl).DefaultView);
			else if(SourceControl is IBindingListView || SourceControl is IFilteredXtraBindingList)
				FilterSourceControl = new BindingListFilterProxy((IBindingList)SourceControl);
			else
#endif
				FilterSourceControl = SourceControl as IFilteredComponent;
		}
		IFilteredComponent filterSourceControl;
		IFilteredComponent FilterSourceControl {
			get {
				return filterSourceControl;
			}
			set {
				if(FilterSourceControl == value) return;
				if(FilterSourceControl != null) {
					FilterSourceControl.PropertiesChanged -= new EventHandler(SourceControlDataSourceChanged);
					FilterSourceControl.RowFilterChanged -= new EventHandler(SourceControlFilterChanged);
				}
				filterSourceControl = value;
				if(FilterSourceControl != null) {
					FilterSourceControl.PropertiesChanged += new EventHandler(SourceControlDataSourceChanged);
					FilterSourceControl.RowFilterChanged += new EventHandler(SourceControlFilterChanged);
				}
				CreateFilterColumnCollection();
			}
		}
		void SourceControlDataSourceChanged(object sender, EventArgs e) {
			CreateFilterColumnCollection();
		}
		void SourceControlFilterChanged(object sender, EventArgs e) {
			if(FilterSourceControl == null) return;
			FilterCriteria = FilterSourceControl.RowCriteria;
		}
		void CreateFilterColumnCollection() {
			if(FilterSourceControl == null) return;
			object columns = FilterColumns;
			FilterColumns = FilterSourceControl.CreateFilterColumnCollection();
			if(columns == null) {
				CreateTree(FilterCriteria);
			}
		}
		public FilterColumn GetColumnByFieldName(string fieldName) {
			if(FilterColumns == null)
				return null;
			foreach(FilterColumn filterColumn in FilterColumns) {
				if(filterColumn.FieldName == fieldName)
					return filterColumn;
			}
			return null;
		}
		protected internal virtual ClauseType GetDefaultOperation(string fieldName) {
			FilterColumn column = GetColumnByFieldName(fieldName);
			if(column == null)
				return ClauseType.Equals;
			switch(column.ClauseClass) {
				case FilterColumnClauseClass.Blob:
					return ClauseType.IsNotNull;
				case FilterColumnClauseClass.String:
					return ClauseType.BeginsWith;
				default:
					return ClauseType.Equals;
			}
		}
		protected internal virtual void RaiseBeforeShowValueEditor(ShowValueEditorEventArgs arg) {
			arg.RoutedEvent = BeforeShowValueEditorEvent;
			RaiseEvent(arg);
		}
		internal protected List<ClauseType> GetListOperationsByTypes(string fieldName) {
			FilterColumn column = GetColumnByFieldName(fieldName);
			List<ClauseType> list = new List<ClauseType>();
			foreach(ClauseType type in DevExpress.Utils.EnumExtensions.GetValues(typeof(ClauseType))) {
				if(column == null || !column.IsValidClause(type))
					continue;
				list.Add(type);
			}
			return list;
		}
		void CreateTree(CriteriaOperator criteria) {
			NodeBase newRootNode = (NodeBase)CriteriaToTreeProcessor.GetTree(CreateNodesFactory(), criteria, null);
			if(newRootNode == null)
				newRootNode = CreateDefaultRootNode();
			if(!(newRootNode is GroupNode)) {
				GroupNode wrapper = (GroupNode)CreateNodesFactory().Create(GroupType.And, new INode[] { newRootNode });
				newRootNode = wrapper;
			}
			RootNode = (GroupNode)newRootNode;
			RootNode.SetOwner(this, null);
		}
		protected virtual INodesFactory CreateNodesFactory() {
			return new FilterControlNodesFactory();
		}
		public ICommand ApplyFilterCommand { get; private set; }
		public void ApplyFilter() {
			FilterCriteria = ToCriteria(RootNode);
			if(FilterSourceControl == null) return;
				FilterSourceControl.RowCriteria = FilterCriteria;
		}
		protected virtual CriteriaOperator ToCriteria(INode node) {
			return FilterControlHelpers.ToCriteria(node);
		}
		internal NodeBase CreateDefaultRootNode() {
			if(DefaultColumn != null) {
				return CreateDefaultClauseNode(DefaultColumn);
			} else {
				return (GroupNode)CreateNodesFactory().Create(GroupType.And, new INode[0]);
			}
		}
		protected virtual ClauseNode CreateDefaultClauseNode(FilterColumn column) {
			if(FilterColumns == null)
				return null;
			if(column == null) {
				if(FilterColumns.Count() == 0)
					return null;
				column = FilterColumns.ElementAt(0);
			}
			OperandProperty prop = CreateDefaultProperty(column);
			ClauseType opType = GetDefaultOperation(column.FieldName);
			ClauseNode cond = (ClauseNode)CreateNodesFactory().Create(opType, prop, new CriteriaOperator[0]);
			ValidateAdditionalOperands(cond);
			return cond;
		}
		protected internal virtual void ValidateAdditionalOperands(IClauseNode node) {
			FilterControlHelpers.ValidateAdditionalOperands(node.Operation, node.AdditionalOperands);
		}
		internal protected OperandProperty CreateDefaultProperty(FilterColumn column) {
			if((FilterColumns != null) && (FilterColumns.Count() > 0)) {
				if(column != null)
					return new OperandProperty(column.FieldName);
				else
					return new OperandProperty(FilterColumns.ElementAt(0).FieldName);
			} else
				return new OperandProperty(string.Empty);
		}
		internal protected virtual OperandProperty CreateDefaultProperty(IClauseNode node) {
			if(GetColumnByFieldName(node.FirstOperand.PropertyName) != null) {
				return node.FirstOperand.Clone();
			} else {
				return CreateDefaultProperty(DefaultColumn);
			}
		}
		internal protected virtual string GetDefaultColumnCaption(IClauseNode node) {
			return node.FirstOperand.PropertyName;
		}
		protected internal ClauseNode AddClauseNode(GroupNode addTo) {
			ClauseNode cond = CreateDefaultClauseNode(DefaultColumn);
			if(cond != null) {
				addTo.SubNodes.Add(cond);
				cond.SetOwner(addTo.Owner, addTo);
			}
			return cond;
		}
		protected internal NodeBase AddGroup(GroupNode currentNode) {
			GroupNode gr = (GroupNode)CreateNodesFactory().Create(GroupType.And, new INode[0]);
			currentNode.SubNodes.Add(gr);
			gr.SetOwner(currentNode.Owner, currentNode);
			return AddClauseNode(gr) ?? (NodeBase)gr;
		}
		protected internal void RemoveNode(NodeBase node) {
			if(node.ParentNode == null)
				return;
			FilterControlNodeBase focusedVisualNode = LayoutHelper.FindLayoutOrVisualParentObject<FilterControlNodeBase>((DependencyObject)FocusHelper.GetFocusedElement());
			bool isNodeFocused = focusedVisualNode != null && (focusedVisualNode.Node == node ||
				node is GroupNode && IsChildNode(node as GroupNode, focusedVisualNode.Node));
			IFilterControlNavigationNode nodeToFocus = null;
			if(isNodeFocused) {
				Focus();
				IFilterControlNavigationNode parentNode = ((GroupNode)node.ParentNode).VisualNode;
				int index = node.ParentNode.SubNodes.IndexOf(node);
				if(index < parentNode.SubNodes.Count - 1)
					nodeToFocus = parentNode.SubNodes[index + 1];
				else if(index > 0)
					nodeToFocus = parentNode.SubNodes[index - 1];
				else
					nodeToFocus = parentNode;
			}
			node.ParentNode.SubNodes.Remove(node);
			if(nodeToFocus != null)
				FocusElement(nodeToFocus.Children[0]);
		}
		bool IsChildNode(GroupNode parentNode, NodeBase node) {
			if(parentNode == null)
				throw new ArgumentNullException("parentNode");
			if(node == null)
				throw new ArgumentNullException("node");
			while(node.ParentNode != null && node.ParentNode != parentNode)
				node = (GroupNode)node.ParentNode;
			return node.ParentNode == parentNode;
		}
		protected internal void ClearAll() {
			RootNode.SubNodes.Clear();
		}
		internal void PerformNavigationOnLeftButtonDown(DependencyObject originalSource) {
			FilterControlEditor editor = LayoutHelper.FindLayoutOrVisualParentObject<FilterControlEditor>(originalSource);
			SetFocusedEditor(editor);
		}
		InplaceFilterEditor lastActiveFilterEditor = null;
		void SetFocusedEditor(FilterControlEditor editor) {
			if(editor != null) {
				if(lastActiveFilterEditor == ((InplaceFilterEditor)editor.Content))
					return;
				if(lastActiveFilterEditor != null)
					lastActiveFilterEditor.IsEditorFocused = false;
				lastActiveFilterEditor = ((InplaceFilterEditor)editor.Content);
				if(lastActiveFilterEditor != null)
					lastActiveFilterEditor.IsEditorFocused = true;
			} else {
				if(lastActiveFilterEditor != null) {
					lastActiveFilterEditor.IsEditorFocused = false;
					lastActiveFilterEditor = null;
				}
			}
		}
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			EditorsOwner.ProcessMouseLeftButtonDown(e);
		}
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonUp(e);
			EditorsOwner.ProcessMouseLeftButtonUp(e);
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			EditorsOwner.ProcessIsKeyboardFocusWithinChanged();
		}
		protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnPreviewLostKeyboardFocus(e);
			EditorsOwner.ProcessPreviewLostKeyboardFocus(e);
		}
		#region Navigation
		FilterControlFocusVisualHelper focusVisualHelper;
		internal void ProcessKeyDown(System.Windows.Input.KeyEventArgs e) {
#if !SL
			if(e.Key == Key.Tab && ModifierKeysHelper.IsCtrlPressed(e.KeyboardDevice.Modifiers)) {
				EditorsOwner.MoveFocus(e);
				return;
			}
#endif
			IFilterControlNavigationNode focusedNode;
			UIElement focusedChild;
			GetFocusedElements((DependencyObject)e.OriginalSource, out focusedNode, out focusedChild);
			if (!e.Handled && focusedNode != null && !((focusedChild is InplaceFilterEditor) && (focusedChild as InplaceFilterEditor).IsEditorVisible)) {
				focusedNode.ProcessKeyDown(e, focusedChild);
				if(e.Handled) return;
			}
			FilterControlNavigationHelper.FilterControlNavigationDirection navigationDirection;
			if(FilterControlKeyboardHelper.IsNavigationKey(e, FlowDirection == FlowDirection.RightToLeft, out navigationDirection)) {
				ProcessNavigationKey(focusedNode, focusedChild, navigationDirection);
				e.Handled = true;
				return;
			}
			if(FilterControlKeyboardHelper.IsShowMenuKey(e) && focusedNode != null && focusedChild != null &&
					!(focusedChild is InplaceFilterEditor)) {
				FilterControlEditor editor = FilterControlKeyboardHelper.GetParentEditor(focusedChild);
				if (editor != null)
					e.Handled = ((IFilterControlNavigationItem)editor).ShowPopupMenu();
				else
					e.Handled = ((IFilterControlNavigationNode)focusedNode).ShowPopupMenu(focusedChild);
				return;
			}
			bool isShiftPressed;
			if(FilterControlKeyboardHelper.IsTabKey(e, out isShiftPressed))
				e.Handled |= ProcessTabKey(isShiftPressed);
		}
		void GetFocusedElements(DependencyObject focusedElement, out IFilterControlNavigationNode focusedNode, out UIElement focusedChild) {
			focusedNode = LayoutHelper.FindLayoutOrVisualParentObject<IFilterControlNavigationNode>(focusedElement);
			focusedChild = null;
			if(focusedNode != null) {
				IList<UIElement> focusedNodeChildren = focusedNode.Children;
				UIElement child = (UIElement)focusedElement;
				while(child != focusedNode) {
					if(focusedNodeChildren.IndexOf(child) != -1) {
						focusedChild = child;
						break;
					}
					child = (UIElement)LayoutHelper.GetParent(child);
				}
			}
		}
		void ProcessNavigationKey(IFilterControlNavigationNode focusedNode, UIElement focusedChild, FilterControlNavigationHelper.FilterControlNavigationDirection navigationDirection) {
			UIElement childToFocus = FilterControlNavigationHelper.GetChildToFocus(RootNode.VisualNode, focusedNode, focusedChild, navigationDirection);
			if(childToFocus != null)
				FocusElement(childToFocus);
		}
		bool ProcessTabKey(bool isShiftPressed) {
			return false; 
		}
		protected internal void FocusElement(UIElement element) {
			if(element is IFilterControlNavigationItem)
				element = (element as IFilterControlNavigationItem).Child;
			EditorsOwner.EditorWasClosed = true;
			SetFocusedEditor(FilterControlKeyboardHelper.GetParentEditor(element));
			element.Focus();
		}
		protected internal void FocusNodeChild(NodeBase node, int childIndex) {
#if DEBUGTEST
#if !SL
			DispatcherHelper.UpdateLayoutAndDoEvents(this);
#endif
			FocusNodeChildCore1(node, childIndex);
#else
#if !SL
			Dispatcher.BeginInvoke(new ThreadStart(() => FocusNodeChildCore1(node, childIndex)), DispatcherPriority.Background, null);
#else
			UpdateLayout();
			Dispatcher.BeginInvoke(() => FocusNodeChildCore1(node, childIndex));
#endif
#endif
		}
		protected internal void FocusNodeChildCore1(NodeBase node, int childIndex) {
			IFilterControlNavigationNode navigationNode = (IFilterControlNavigationNode)node.VisualNode;
			if(navigationNode == null)
				return;
			IList<UIElement> children = navigationNode.Children;
			if(0 <= childIndex && childIndex < children.Count)
				node.Owner.FocusElement(children[childIndex]);
		}
#if !SL
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnGotKeyboardFocus(e);
#else
		protected override void  OnGotFocus(System.Windows.RoutedEventArgs e) {
 			base.OnGotFocus(e);
#endif
			if(FocusHelper.GetFocusedElement() == this && RootNode.VisualNode != null) {
				IList<UIElement> navigationChildren = ((IFilterControlNavigationNode)RootNode.VisualNode).Children;
				if(navigationChildren.Count > 0)
					FocusElement(navigationChildren[0]);
			} else
				CheckFocusVisual();
		}
#if !SL
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnLostKeyboardFocus(e);
#else
		protected override void  OnLostFocus(System.Windows.RoutedEventArgs e) {
			base.OnLostFocus(e);
#endif
			CheckFocusVisual();
		}
		void CheckFocusVisual() {
			FrameworkElement focusedElement = (IsKeyboardFocusWithin && FocusHelper.GetFocusedElement() != this && EditorsOwner.ActiveEditor == null) ? FocusHelper.GetFocusedElement() as FrameworkElement : null;
			if(focusedElement != null) {
				IFilterControlNavigationNode focusedNode;
				UIElement focusedChild;
				GetFocusedElements(focusedElement, out focusedNode, out focusedChild);
				if(focusedChild == null || !LayoutHelper.IsChildElement(focusedChild, focusedElement))
					focusedElement = null;
				else
					focusedElement = (FrameworkElement)focusedChild;
			}
			focusVisualHelper.FocusedElement = focusedElement;
		}
		#endregion
		internal void EnqueueImmediateAction(IAction action) {
			EditorsImmediateActionsManager.EnqueueAction(action);
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyFilter();
		}
		void IDialogContent.OnOk() {
			ApplyFilter();
		}
		#endregion
	}
#if !SL
	class BindingListFilterProxy : BindingListFilterProxyBase, IFilteredComponent {
		public BindingListFilterProxy(IBindingList dataSource) : base(dataSource) { }
		IEnumerable<FilterColumn> IFilteredComponent.CreateFilterColumnCollection() {
			List<FilterColumn> list = new List<FilterColumn>();
			DataColumnInfo[] columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(null, DataSource, null);
			if(columns != null) {
				foreach(DataColumnInfo column in columns) {
					list.Add(new DataColumnInfoFilterColumn(column));
				}
			}
			return list;
		}
	}
#endif
	public class GroupFilterControlNodeToListConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return new List<GroupNode>() { (GroupNode)value };
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class FilterControlNodesFactory : INodesFactory {
		public IClauseNode Create(ClauseType type, OperandProperty firstOperand, System.Collections.Generic.ICollection<CriteriaOperator> operands) {
			ClauseNode rv = new ClauseNode();
			rv.Operation = type;
			rv.FirstOperand = firstOperand;
			rv.RepopulateAdditionalOperands(operands);
			return rv;
		}
		public IGroupNode Create(GroupType type, System.Collections.Generic.ICollection<INode> subNodes) {
			GroupNode rv = new GroupNode();
			rv.NodeType = type;
			foreach(INode subNode in subNodes) {
				subNode.SetParentNode(rv);
				rv.SubNodes.Add(subNode);
			}
			return rv;
		}
	}
	#region Filter Column
	public class FilterColumn {
		object columnCaption = null;
		public virtual object ColumnCaption {
			get { return ((columnCaption == null) || ((columnCaption as string) == string.Empty)) ? FieldName : columnCaption; }
			set { columnCaption = value; }
		}
		public virtual DataTemplate HeaderTemplate { get; set; }
		public virtual DataTemplateSelector HeaderTemplateSelector { get; set; }
		public virtual string FieldName { get; set; }
		BaseEditSettings editSettings = null;
		public virtual BaseEditSettings EditSettings {
			get {
				if(editSettings == null)
					editSettings = new TextEditSettings();
				return editSettings;
			}
			set { editSettings = value; }
		}
		public virtual FilterColumnClauseClass ClauseClass {
			get {
				if(ColumnType == typeof(string)) {
					return FilterColumnClauseClass.String;
				} else if(ColumnType == typeof(DateTime) || ColumnType == typeof(DateTime?)) {
					return FilterColumnClauseClass.DateTime;
				} else {
					return FilterColumnClauseClass.Generic;
				}
			}
		}
		public virtual Type ColumnType { get; set; }
		public virtual bool IsValidClause(ClauseType clause) {
			return FilterControlHelpers.IsValidClause(clause, ClauseClass);
		}
	}
	#endregion
	public class DataColumnInfoFilterColumn : FilterColumn {
		public readonly IDataColumnInfo Column;
		public DataColumnInfoFilterColumn(IDataColumnInfo column)
			: base() {
			Column = column;
			HeaderTemplate = null;
			HeaderTemplateSelector = null;
			EditSettings = CreateEditSettings();
		}
		public override Type ColumnType { get { return this.Column.FieldType; } }
		public override string FieldName { get { return Column.Name; } }
		public override object ColumnCaption { get { return Column.Caption; } }
		BaseEditSettings CreateEditSettings() {
			if(ColumnType == typeof(Boolean) || ColumnType == typeof(Boolean?))
				return new CheckEditSettings();
			if(ColumnType == typeof(DateTime) || ColumnType == typeof(DateTime?))
				return new DateEditSettings();
			return new TextEditSettings();
		}
	}
	#region NODES
	public abstract class NodeBase : DependencyObject, INode {
		FilterControl owner = null;
		GroupNode parentNode = null;
		FilterControlNodeBase visualNode;
		public FilterControl Owner { get { return owner; } }
		public FilterControlNodeBase VisualNode { get { return visualNode; } }
		public virtual void SetOwner(FilterControl owner, GroupNode parentNode) {
			SetParentNode(parentNode);
			this.owner = owner;
		}
		public virtual void SetVisualNode(FilterControlNodeBase visualNode) {
			this.visualNode = visualNode;
		}
		protected abstract object Accept(INodeVisitor visitor);
		void SetParentNode(INode node) {
			parentNode = (GroupNode)node;
		}
		void INode.SetParentNode(IGroupNode node) {
			SetParentNode(node);
		}
		object INode.Accept(INodeVisitor visitor) {
			return Accept(visitor);
		}
		public IGroupNode ParentNode { get { return parentNode; } }
	}
	public class GroupNode : NodeBase, IGroupNode {
		#region dependency properties fields
		public static readonly DependencyProperty NodeTypeProperty;
		#endregion
		static GroupNode() {
			Type ownerType = typeof(GroupNode);
			NodeTypeProperty = DependencyPropertyManager.Register("NodeType", typeof(GroupType), ownerType, new PropertyMetadata(GroupType.And));
		}
		#region dependency properties accessors
		public GroupType NodeType {
			get { return (GroupType)GetValue(NodeTypeProperty); }
			set { SetValue(NodeTypeProperty, value); }
		}
		#endregion
		ObservableCollection<INode> subNodes;
		public IList<INode> SubNodes { get { return subNodes; } }
		public override void SetOwner(FilterControl owner, GroupNode parentNode) {
			foreach(NodeBase subNode in SubNodes)
				subNode.SetOwner(owner, this);
			base.SetOwner(owner, parentNode);
		}
		public GroupNode() : base() {
			subNodes = new ObservableCollection<INode>();
		}
		protected override object Accept(INodeVisitor visitor) {
			return visitor.Visit(this);
		}
	}
	public enum OperandsCount { None, One, Two, Several, OneLocalDateTime };
	public class ClauseNode : NodeBase, IClauseNode, INotifyPropertyChanged {
		#region dependency properties fields
		public static readonly DependencyProperty FirstOperandProperty;
		public static readonly DependencyProperty ColumnHeaderCaptionProperty;
		public static readonly DependencyProperty ColumnHeaderTemplateProperty;
		public static readonly DependencyProperty OperationProperty;
		static readonly DependencyPropertyKey SecondOperandsCountPropertyKey;
		public static readonly DependencyProperty SecondOperandsCountProperty;
		#endregion
		static ClauseNode() {
			Type ownerType = typeof(ClauseNode);
			FirstOperandProperty = DependencyPropertyManager.Register("FirstOperand", typeof(OperandProperty), ownerType, new PropertyMetadata(null, (d, e) => ((ClauseNode)d).OnFirstOperandChange()));
			ColumnHeaderCaptionProperty = DependencyPropertyManager.Register("ColumnHeaderCaption", typeof(object), ownerType, new PropertyMetadata(null));
			ColumnHeaderTemplateProperty = DependencyPropertyManager.Register("ColumnHeaderTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null));
			OperationProperty = DependencyPropertyManager.Register("Operation", typeof(ClauseType), ownerType, new PropertyMetadata(ClauseType.Equals, (d, e) => ((ClauseNode)d).OnOperationChange()));
			SecondOperandsCountPropertyKey = DependencyPropertyManager.RegisterReadOnly("SecondOperandsCount", typeof(OperandsCount), ownerType, new PropertyMetadata(OperandsCount.One));
			SecondOperandsCountProperty = SecondOperandsCountPropertyKey.DependencyProperty;
		}
		#region dependency properties accessors
		public OperandProperty FirstOperand {
			get { return (OperandProperty)GetValue(FirstOperandProperty); }
			set { SetValue(FirstOperandProperty, value); }
		}
		public object ColumnHeaderCaption {
			get { return (object)GetValue(ColumnHeaderCaptionProperty); }
			set { SetValue(ColumnHeaderCaptionProperty, value); }
		}
		public DataTemplate ColumnHeaderTemplate {
			get { return (DataTemplate)GetValue(ColumnHeaderTemplateProperty); }
			set { SetValue(ColumnHeaderTemplateProperty, value); }
		}
		public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty = DependencyProperty.Register("ColumnHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(ClauseNode), new FrameworkPropertyMetadata(null));
		public DataTemplateSelector ColumnHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ColumnHeaderTemplateSelectorProperty); }
			set { SetValue(ColumnHeaderTemplateSelectorProperty, value); }
		}
		public ClauseType Operation {
			get { return (ClauseType)GetValue(OperationProperty); }
			set { SetValue(OperationProperty, value); }
		}
		public OperandsCount SecondOperandsCount {
			get { return (OperandsCount)GetValue(SecondOperandsCountProperty); }
			private set { this.SetValue(SecondOperandsCountPropertyKey, value); }
		}
		#endregion
		ObservableCollection<CriteriaOperator> additionalOperands;
		public IList<CriteriaOperator> AdditionalOperands { get { return additionalOperands; } }
		public void ResetAdditionalOperand(CriteriaOperator newOperand, int index) {
			if(object.Equals(AdditionalOperands[index], newOperand)) return;
			AdditionalOperands[index] = newOperand;
			UpdateLocalDateTimeLabel();
		}
		public void AddAdditionalOperand(CriteriaOperator op) {
			AdditionalOperands.Add(op);
			UpdateLocalDateTimeLabel();
		}
		public void RemoveAdditionalOperandAt(int index) {
			AdditionalOperands.RemoveAt(index);
			UpdateLocalDateTimeLabel();
		}
		public void RepopulateAdditionalOperands(IEnumerable<CriteriaOperator> newOps) {
			AdditionalOperands.Clear();
			newOps.ForEach(x => AddAdditionalOperand(x));
			UpdateLocalDateTimeLabel();
		}
		internal List<FilterControlEditor> Editors { get; private set; }
		public FilterControlClauseNode VisualClauseNode { get { return (FilterControlClauseNode)VisualNode; } }
		protected void OnFirstOperandChange() {
			ResetColumnHeaderCaption();
		}
		internal bool IsCollectionClause(ClauseType type) {
			return type == ClauseType.AnyOf || type == ClauseType.NoneOf;
		}
		bool IsTwoFieldsClause(ClauseType type) {
			return type == ClauseType.Between || type == ClauseType.NotBetween;
		}
		protected void OnOperationChange() {
			if(Owner != null)
				Owner.ValidateAdditionalOperands(this);
			ResetSecondOperandsCount();
		}
		void ResetSecondOperandsCount() {
			if(IsCollectionClause(Operation)) {
				SecondOperandsCount = OperandsCount.Several;
			}
			else if(IsTwoFieldsClause(Operation)) {
				SecondOperandsCount = OperandsCount.Two;
			}
			else if(AdditionalOperands.Count == 0) {
				SecondOperandsCount = OperandsCount.None;
			}
			else if(AdditionalOperands.Count == 1 && IsLocalDateTimeFunction) {
				SecondOperandsCount = OperandsCount.OneLocalDateTime;
			}
			else {
				SecondOperandsCount = OperandsCount.One;
			}
		}
		string localDateTimeLable;
		public string LocalDateTimeFunctionLabel {
			get { return localDateTimeLable; }
			private set { SetProperty(ref localDateTimeLable, value, BindableBase.GetPropertyName(() => LocalDateTimeFunctionLabel)); }
		}
		protected internal bool IsLocalDateTimeFunction {
			get {
				if(AdditionalOperands.Count < 1) return false;
				var cr = AdditionalOperands[0] as FunctionOperator;
				if(ReferenceEquals(cr, null)) return false;
				var op = cr.OperatorType;
				return op >= FunctionOperatorType.LocalDateTimeThisYear && op <= FunctionOperatorType.LocalDateTimeNextYear;
			}
		}
		void UpdateLocalDateTimeLabel() {
			if(!IsLocalDateTimeFunction) return;
			LocalDateTimeFunctionLabel = GetLocalDateTimeLabel();
		}
		string GetLocalDateTimeLabel() {
			return AdditionalOperands.Count > 0
				? LocalaizableCriteriaToStringProcessor.Process(AdditionalOperands[0])
				: string.Empty;
		}
		public override void SetOwner(FilterControl owner, GroupNode parentNode) {
			base.SetOwner(owner, parentNode);
			ResetColumnHeaderCaption();
		}
		void ResetColumnHeaderCaption() {
			if(Owner != null) {
				FilterColumn column = Owner.GetColumnByFieldName(FirstOperand.PropertyName);
				if(column != null) {
					ColumnHeaderCaption = column.ColumnCaption;
					ColumnHeaderTemplate = column.HeaderTemplate;
					ColumnHeaderTemplateSelector = column.HeaderTemplateSelector;
				} else {
					ColumnHeaderCaption = Owner.GetDefaultColumnCaption(this);
					ColumnHeaderTemplate = null;
					ColumnHeaderTemplateSelector = null;
				}
			}
		}
		public ClauseNode() : base() {
			Editors = new List<FilterControlEditor>();
			additionalOperands = new ObservableCollection<CriteriaOperator>();
			additionalOperands.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(AdditionalOperandsCollectionChanged);
		}
		void AdditionalOperandsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			ResetSecondOperandsCount();
		}
		protected override object Accept(INodeVisitor visitor) {
			return visitor.Visit(this);
		}
		public event PropertyChangedEventHandler PropertyChanged;
		void SetProperty<T>(ref T storage, T value, string propertyName) {
			storage = value;
			OnPropertyChanged(propertyName);
		}
		void OnPropertyChanged(string propertyName) {
			var handler = PropertyChanged;
			if(handler == null) return;
			handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	#endregion
	public class OperationHelper {
		public static string GetMenuStringByType(ClauseType type) {
			string typeString = type.ToString();
			string localizationId = "FilterClause" + typeString;
			if(Enum.IsDefined(typeof(EditorStringId), localizationId)) {
				return EditorLocalizer.GetString(localizationId);
			} else {
#if DEBUGTEST && !SL
				System.Diagnostics.Debug.Fail("Filter clause type " + typeString + " not localized");
#endif
				return typeString;
			}
		}
	}
	public class FirstToCollapsedConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (((StackPanelElementPosition)value == StackPanelElementPosition.First) || ((StackPanelElementPosition)value == StackPanelElementPosition.Single)) ? Visibility.Collapsed : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class AdditionalOperandsCountToVisibilityConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			int count = Convert.ToInt32(value);
			return count == 0 || count == 1 ? Visibility.Collapsed : Visibility.Visible;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
}

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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.LayoutControl {
	public enum LayoutGroupCollapseMode { None, NoChildrenVisible, OneChildVisible }
	public enum LayoutGroupPartStyle { GroupBox, Tabs, Tab }
	public enum LayoutGroupView { Group, GroupBox, Tabs }
	public enum LayoutItemLabelsAlignment { Default, Local }
	public interface ILayoutGroupModel : ILayoutModelBase {
		Orientation CollapseDirection { get; }
		LayoutGroupCollapseMode CollapseMode { get; }
#if !SILVERLIGHT
		bool MeasureUncollapsedChildOnly { get; }
#endif
		Orientation Orientation { get; set; }
		FrameworkElement UncollapsedChild { get; }
	}
	public interface ILayoutGroup : ILayoutControlBase, ILayoutGroupModel, ILiveCustomizationAreasProvider {
		void AllowItemSizingChanged();
		void ApplyItemStyle();
		void ChildHorizontalAlignmentChanged(FrameworkElement child);
		void ChildVerticalAlignmentChanged(FrameworkElement child);
		void ClearItemStyle();
		void CopyTabHeaderInfo(FrameworkElement fromChild, FrameworkElement toElement);
		bool DesignTimeClick(DXMouseButtonEventArgs args);
		FrameworkElements GetArrangedLogicalChildren(bool visibleOnly);
		Rect GetClipBounds(FrameworkElement child, FrameworkElement relativeTo);
		IEnumerable<string> GetDependencyPropertiesWithOverriddenDefaultValue();
		LayoutItemInsertionInfo GetInsertionInfoForEmptyArea(FrameworkElement element, Point p);
		LayoutItemInsertionKind GetInsertionKind(FrameworkElement destinationItem, Point p);
		LayoutItemInsertionPoint GetInsertionPoint(FrameworkElement element, FrameworkElement destinationItem,
			LayoutItemInsertionKind insertionKind, Point p);
		Rect GetInsertionPointBounds(bool isInternalInsertion, FrameworkElement relativeTo);
		void GetInsertionPoints(FrameworkElement element, FrameworkElement destinationItem, FrameworkElement originalDestinationItem,
			LayoutItemInsertionKind insertionKind, LayoutItemInsertionPoints points);
		Rect GetInsertionPointZoneBounds(FrameworkElement destinationItem, LayoutItemInsertionKind insertionKind, int pointIndex, int pointCount);
		FrameworkElement GetItem(Point p, bool ignoreLayoutGroups, bool ignoreLocking);
		HorizontalAlignment GetItemHorizontalAlignment(FrameworkElement item);
		VerticalAlignment GetItemVerticalAlignment(FrameworkElement item);
		Style GetItemStyle();
		void GetLayoutItems(LayoutItems layoutItems);
		int GetTabIndex(Point absolutePosition);
		void InitChildFromGroup(FrameworkElement child, FrameworkElement group);
		void InsertElement(FrameworkElement element, LayoutItemInsertionPoint insertionPoint, LayoutItemInsertionKind insertionKind);
		bool IsChildBorderless(ILayoutGroup child);
		bool IsChildPermanent(ILayoutGroup child, bool keepTabs);
		bool IsExternalInsertionPoint(FrameworkElement element, FrameworkElement destinationItem, LayoutItemInsertionKind insertionKind);
		bool IsRemovableForOptimization(bool considerContent, bool keepEmptyTabs);
		void LayoutItemLabelWidthChanged(FrameworkElement layoutItem);
		void LayoutItemLabelsAlignmentChanged();
		bool MakeChildVisible(FrameworkElement child);
		ILayoutGroup MoveChildrenToNewGroup();
		bool MoveChildrenToParent();
		ILayoutGroup MoveChildToNewGroup(FrameworkElement child);
		void MoveNonUserDefinedChildrenToAvailableItems();
		bool OptimizeLayout(bool keepEmptyTabs);
		void SetItemHorizontalAlignment(FrameworkElement item, HorizontalAlignment value, bool updateWidth);
		void SetItemVerticalAlignment(FrameworkElement item, VerticalAlignment value, bool updateHeight);
		void UpdatePartStyle(LayoutGroupPartStyle style);
		HorizontalAlignment ActualHorizontalAlignment { get; }
		VerticalAlignment ActualVerticalAlignment { get; }
		Size ActualMinSize { get; }
		Size ActualMaxSize { get; }
		Rect ChildAreaBounds { get; }
		HorizontalAlignment DesiredHorizontalAlignment { get; }
		VerticalAlignment DesiredVerticalAlignment { get; }
		bool HasNewChildren { get; }
		bool HasUniformLayout { get; }
		object Header { get; }
		DataTemplate HeaderTemplate { get; }
		bool IsBorderless { get; }
		bool IsCollapsed { set; }
		bool IsCustomization { get; set; }
		bool IsItemLabelsAlignmentScope { get; }
		bool IsLocked { get; set; }
		bool IsRoot { get; }
		bool IsUIEmpty { get; }
		Style ItemSizerStyle { get; set; }
		ILayoutControl Root { get; }
		int SelectedTabIndex { get; set; }
		Orientation VisibleOrientation { get; }
		GroupBoxDisplayMode ActualGroupBoxDisplayMode { get; }
	}
	public static class LayoutGroupExtensions {
		public static ILayoutControl GetLayoutControl(this FrameworkElement element) {
			FrameworkElement result = element;
			while (result != null && !result.IsLayoutControl()) {
				result = result.Parent as FrameworkElement;
				if (!(result is ILayoutGroup || result is LayoutControl.ItemsContainer))
					break;
			}
			return result as ILayoutControl;
		}
		public static bool IsLayoutControl(this UIElement element) {
			return element is ILayoutGroup && ((ILayoutGroup)element).IsRoot;
		}
		public static bool IsLayoutGroup(this UIElement element) {
			return element is ILayoutGroup && !((ILayoutGroup)element).IsRoot;
		}
	}
	[StyleTypedProperty(Property = "GroupBoxStyle", StyleTargetType = typeof(GroupBox))]
	[StyleTypedProperty(Property = "ItemStyle", StyleTargetType = typeof(LayoutItem))]
	[StyleTypedProperty(Property = "TabsStyle", StyleTargetType = typeof(DXTabControl))]
	[StyleTypedProperty(Property = "TabStyle", StyleTargetType = typeof(DXTabItem))]
	[DXToolboxBrowsable(DXToolboxItemKind.Free)]
#if !SILVERLIGHT
#endif
	public class LayoutGroup : LayoutControlBase, ILayoutGroup, ILayoutControlCustomizableItem {
		public const Orientation DefaultOrientation = Orientation.Horizontal;
		#region Dependency Properties
		public static readonly DependencyProperty GroupBoxStyleProperty =
			DependencyProperty.Register("GroupBoxStyle", typeof(Style), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnPartStyleChanged(LayoutGroupPartStyle.GroupBox)));
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(object), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnHeaderChanged()));
		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnHeaderTemplateChanged()));
		public static readonly DependencyProperty IsCollapsedProperty =
			DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnIsCollapsedChanged()));
		public static readonly DependencyProperty IsCollapsibleProperty =
			DependencyProperty.Register("IsCollapsible", typeof(bool), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnIsCollapsibleChanged()));
		public static readonly DependencyProperty IsLockedProperty =
			DependencyProperty.Register("IsLocked", typeof(bool), typeof(LayoutGroup), null);
		public static readonly DependencyProperty ItemLabelsAlignmentProperty =
			DependencyProperty.Register("ItemLabelsAlignment", typeof(LayoutItemLabelsAlignment), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnItemLabelsAlignmentChanged()));
		public static readonly DependencyProperty ItemStyleProperty =
			DependencyProperty.Register("ItemStyle", typeof(Style), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnItemStyleChanged()));
#if !SILVERLIGHT
		public static readonly DependencyProperty MeasureSelectedTabChildOnlyProperty =
			DependencyProperty.Register("MeasureSelectedTabChildOnly", typeof(bool), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnMeasureSelectedTabChildOnlyChanged()));
#endif
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(LayoutGroup),
				new PropertyMetadata(DefaultOrientation,
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (LayoutGroup)o;
#if SILVERLIGHT
						if (!o.EnsureDefaultValue(e.Property, control.GetDefaultOrientation(), true))
#endif
							control.OnOrientationChanged();
					}));
		public static readonly DependencyProperty SelectedTabIndexProperty =
			DependencyProperty.Register("SelectedTabIndex", typeof(int), typeof(LayoutGroup),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						if ((int)e.NewValue < 0)
							o.SetValue(e.Property, e.OldValue);
						else
							((LayoutGroup)o).OnSelectedTabIndexChanged((int)e.OldValue);
					}));
		protected static readonly DependencyProperty StoredSizeProperty =
			DependencyProperty.Register("StoredSize", typeof(double), typeof(LayoutGroup), new PropertyMetadata(double.NaN));
		public static readonly DependencyProperty TabsStyleProperty =
			DependencyProperty.Register("TabsStyle", typeof(Style), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnPartStyleChanged(LayoutGroupPartStyle.Tabs)));
		public static readonly DependencyProperty TabStyleProperty =
			DependencyProperty.Register("TabStyle", typeof(Style), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnPartStyleChanged(LayoutGroupPartStyle.Tab)));
		public static readonly DependencyProperty ViewProperty =
			DependencyProperty.Register("View", typeof(LayoutGroupView), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnViewChanged()));
		public static readonly DependencyProperty GroupBoxDisplayModeProperty =
			DependencyProperty.Register("GroupBoxDisplayMode", typeof(GroupBoxDisplayMode), typeof(LayoutGroup), 
				new PropertyMetadata(GroupBoxDisplayMode.Default, (d,e) => ((LayoutGroup)d).UpdateActualGroupBoxDisplayMode()));
		static readonly DependencyPropertyKey ActualGroupBoxDisplayModePropertyKey =
			DependencyProperty.RegisterReadOnly("ActualGroupBoxDisplayMode", typeof(GroupBoxDisplayMode), typeof(LayoutGroup),
				new PropertyMetadata(GroupBoxDisplayMode.Default, (d, e) => ((LayoutGroup)d).InitGroupBoxDisplayMode()));
		public static readonly DependencyProperty ActualGroupBoxDisplayModeProperty = ActualGroupBoxDisplayModePropertyKey.DependencyProperty;
		#endregion Dependency Properties
		static LayoutGroup() {
			PaddingProperty.OverrideMetadata(typeof(LayoutGroup), new PropertyMetadata(new Thickness(0)));
		}
		private bool _IsActuallyCollapsed;
		private bool _IsCustomization;
		private object _StoredMinSizePropertyValue;
		public LayoutGroup() {
			ItemSizers = CreateItemSizers();
			ItemSizers.SizingAreaWidth = ItemSpace;
			_IsActuallyCollapsed = IsActuallyCollapsed;
#if SILVERLIGHT
			this.EnsureDefaultValue(OrientationProperty, GetDefaultOrientation(), false);
#endif
		}
		public void ApplyItemStyle() {
			foreach (FrameworkElement child in GetLogicalChildren(false))
				ApplyItemStyle(child);
		}
		public virtual LayoutGroup CreateGroup() {
			var res = (LayoutGroup)GetGroupType().GetConstructor(Type.EmptyTypes).Invoke(null);
			if(GroupBoxDisplayMode != GroupBoxDisplayMode.Default)
				res.GroupBoxDisplayMode = GroupBoxDisplayMode;
			return res;
		}
		public FrameworkElements GetArrangedLogicalChildren(bool visibleOnly) {
			var result = GetLogicalChildren(visibleOnly);
			LayoutProvider.ArrangeItems(result);
			return result;
		}
		public ILayoutGroup MoveChildrenToNewGroup() {
			var group = CreateGroup();
			((ILayoutGroupModel)group).Orientation = Orientation;
			foreach (var child in GetLogicalChildren(false)) {
				Children.Remove(child);
				group.Children.Add(child);
			}
			Children.Add(group);
			return group;
		}
		public bool MoveChildrenToParent() {
			if(IsLocked)
				return false;
			var parent = Parent as Panel;
			if(parent == null)
				return false;
			var index = parent.Children.IndexOf(this);
			var children = GetLogicalChildren(false);
			for(int i = children.Count - 1; i >= 0; i--) {
				var child = children[i];
				Children.Remove(child);
				if(parent is ILayoutGroup)
					((ILayoutGroup)parent).InitChildFromGroup(child, this);
				parent.Children.Insert(index, child);
			}
			return true;
		}
		public ILayoutGroup MoveChildToNewGroup(FrameworkElement child) {
			var childIndex = Children.IndexOf(child);
			Children.RemoveAt(childIndex);
			LayoutGroup result = CreateGroup();
			InitGroupForChild(result, child);
			result.Children.Add(child);
			Children.Insert(childIndex, result);
			return result;
		}
		public bool OptimizeLayout() {
			return OptimizeLayout(false);
		}
		public bool OptimizeLayout(bool keepEmptyTabs) {
			bool result = false;
			int lastChildCount = 0;
			while (true) {
				FrameworkElements children = GetLogicalChildren(false);
				if (children.Count == lastChildCount)
					break;
				foreach (FrameworkElement child in children)
					if (child.IsLayoutGroup())
						result = ((ILayoutGroup)child).OptimizeLayout(keepEmptyTabs) || result;
				lastChildCount = children.Count;
			}
			result = OnOptimizeLayout(keepEmptyTabs) || result;
			return result;
		}
		public bool SelectTab(FrameworkElement child) {
			if (!HasTabs)
				return false;
			int index = GetLogicalChildren(true).IndexOf(child);
			bool isCorrectChild = index != -1;
			if (isCorrectChild)
				((ILayoutGroup)this).SelectedTabIndex = index;
			return isCorrectChild;
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			UpdateActualGroupBoxDisplayMode();
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupActualHorizontalAlignment")]
#endif
		public HorizontalAlignment ActualHorizontalAlignment {
			get { return this.IsPropertySet(HorizontalAlignmentProperty) ? HorizontalAlignment : DesiredHorizontalAlignment; }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupActualVerticalAlignment")]
#endif
		public VerticalAlignment ActualVerticalAlignment {
			get { return this.IsPropertySet(VerticalAlignmentProperty) ? VerticalAlignment : DesiredVerticalAlignment; }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupActualMinSize")]
#endif
		public Size ActualMinSize {
			get {
				var result = this.GetMinSize();
				var contentMinSize = LayoutProvider.GetActualMinOrMaxSize(GetLogicalChildren(true), true, CreateLayoutProviderParameters());
				SizeHelper.UpdateMaxSize(ref result, CalculateSizeFromContentSize(contentMinSize));
				return result;
			}
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupActualMaxSize")]
#endif
		public Size ActualMaxSize {
			get {
				var result = this.GetMaxSize();
				var contentMaxSize = LayoutProvider.GetActualMinOrMaxSize(GetLogicalChildren(true), false, CreateLayoutProviderParameters());
				SizeHelper.UpdateMinSize(ref result, CalculateSizeFromContentSize(contentMaxSize));
				return result;
			}
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupController")]
#endif
		public new LayoutGroupController Controller { get { return (LayoutGroupController)base.Controller; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupGroupBoxStyle")]
#endif
		public Style GroupBoxStyle {
			get { return (Style)GetValue(GroupBoxStyleProperty); }
			set { SetValue(GroupBoxStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupHeader")]
#endif
		[XtraSerializableProperty]
		public object Header {
			get { return GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupHeaderTemplate")]
#endif
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupIsActuallyCollapsed")]
#endif
		public bool IsActuallyCollapsed { get { return HasGroupBox && IsCollapsible && IsCollapsed; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupIsCollapsed")]
#endif
		[XtraSerializableProperty]
		public bool IsCollapsed {
			get { return (bool)GetValue(IsCollapsedProperty); }
			set { SetValue(IsCollapsedProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupIsCollapsible")]
#endif
		public bool IsCollapsible {
			get { return (bool)GetValue(IsCollapsibleProperty); }
			set { SetValue(IsCollapsibleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupIsLocked")]
#endif
		[XtraSerializableProperty]
		public bool IsLocked {
			get { return (bool)GetValue(IsLockedProperty); }
			set { SetValue(IsLockedProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupIsPermanent")]
#endif
		public bool IsPermanent { get { return GetIsPermanent(true); } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupIsRoot")]
#endif
		public virtual bool IsRoot { get { return false; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupItemLabelsAlignment")]
#endif
		public virtual LayoutItemLabelsAlignment ItemLabelsAlignment {
			get { return (LayoutItemLabelsAlignment)GetValue(ItemLabelsAlignmentProperty); }
			set { SetValue(ItemLabelsAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupItemStyle")]
#endif
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
#if !SILVERLIGHT
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupMeasureSelectedTabChildOnly")]
#endif
		public bool MeasureSelectedTabChildOnly {
			get { return (bool)GetValue(MeasureSelectedTabChildOnlyProperty); }
			set { SetValue(MeasureSelectedTabChildOnlyProperty, value); }
		}
#endif
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupOrientation")]
#endif
		[XtraSerializableProperty]
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupRoot")]
#endif
		public ILayoutControl Root { get { return this.GetLayoutControl(); } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupSelectedTabChild")]
#endif
		public FrameworkElement SelectedTabChild {
			get {
				if (!HasTabs)
					return null;
				FrameworkElements children = GetLogicalChildren(true);
				if (children.Count != 0)
					return children[TabControl.SelectedIndex];
				else
					return null;
			}
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupSelectedTabIndex")]
#endif
		[XtraSerializableProperty]
		public int SelectedTabIndex {
			get { return (int)GetValue(SelectedTabIndexProperty); }
			set { SetValue(SelectedTabIndexProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupTabsStyle")]
#endif
		public Style TabsStyle {
			get { return (Style)GetValue(TabsStyleProperty); }
			set { SetValue(TabsStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupTabStyle")]
#endif
		public Style TabStyle {
			get { return (Style)GetValue(TabStyleProperty); }
			set { SetValue(TabStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("LayoutGroupView")]
#endif
		[XtraSerializableProperty]
		public LayoutGroupView View {
			get { return (LayoutGroupView)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public GroupBoxDisplayMode GroupBoxDisplayMode {
			get { return (GroupBoxDisplayMode)GetValue(GroupBoxDisplayModeProperty); }
			set { SetValue(GroupBoxDisplayModeProperty, value); }
		}
		public GroupBoxDisplayMode ActualGroupBoxDisplayMode {
			get { return (GroupBoxDisplayMode)GetValue(ActualGroupBoxDisplayModeProperty); }
			protected set { SetValue(ActualGroupBoxDisplayModePropertyKey, value); }
		}
		public event EventHandler Collapsed;
		public event EventHandler Expanded;
		public event ValueChangedEventHandler<FrameworkElement> SelectedTabChildChanged;
		#region Children
		private static readonly DependencyProperty UnfocusableStateInfoProperty =
			DependencyProperty.RegisterAttached("UnfocusableStateInfo", typeof(object), typeof(LayoutGroup), null);
		protected static DependencyProperty ChildHorizontalAlignmentListener = RegisterChildPropertyListener("HorizontalAlignment", typeof(LayoutGroup));
		protected static DependencyProperty ChildVerticalAlignmentListener = RegisterChildPropertyListener("VerticalAlignment", typeof(LayoutGroup));
		protected static DependencyProperty ChildWidthListener = RegisterChildPropertyListener("Width", typeof(LayoutGroup));
		protected static DependencyProperty ChildHeightListener = RegisterChildPropertyListener("Height", typeof(LayoutGroup));
		protected static DependencyProperty ChildMaxWidthListener = RegisterChildPropertyListener("MaxWidth", typeof(LayoutGroup));
		protected static DependencyProperty ChildMaxHeightListener = RegisterChildPropertyListener("MaxHeight", typeof(LayoutGroup));
		protected static DependencyProperty ChildStyleListener = RegisterChildPropertyListener("Style", typeof(LayoutGroup));
		protected static DependencyProperty ChildVisibilityListener = RegisterChildPropertyListener("Visibility", typeof(LayoutGroup));
		protected static void InvalidateParents(FrameworkElement child) {
			FrameworkElement parent = child;
			do {
				parent = (FrameworkElement)parent.Parent;
				if (parent != null)
					parent.InvalidateMeasure();
			}
			while (parent.IsLayoutGroup());
		}
		protected void CheckUnfocusableState(FrameworkElement child) {
#if SILVERLIGHT
			if (child is Control)
#endif
				if (IsChildUnfocusable(child))
					InitializeUnfocusableState(child);
				else
					FinalizeUnfocusableState(child);
#if SILVERLIGHT
			else
				if (child is LayoutGroup)
					((LayoutGroup)child).CheckUnfocusableStateForChildren();
#endif
		}
		protected void CheckUnfocusableStateForChildren() {
			foreach (FrameworkElement child in Children)
				CheckUnfocusableState(child);
		}
		protected void InitializeUnfocusableState(FrameworkElement child) {
			if (this.IsInDesignTool())
				return;
			object info = child.GetValue(UnfocusableStateInfoProperty);
			if (info != null)
				return;
			child.SetValue(UnfocusableStateInfoProperty, child.StorePropertyValue(Control.IsEnabledProperty));
			child.SetValue(Control.IsEnabledProperty, false);
#if !SILVERLIGHT
			if (child.IsKeyboardFocusWithin && Keyboard.FocusedElement is DependencyObject) {
				DependencyObject focusScope = FocusManager.GetFocusScope((DependencyObject)Keyboard.FocusedElement);
				FocusManager.SetFocusedElement(focusScope, null);
			}
#endif
		}
		protected void FinalizeUnfocusableState(FrameworkElement child) {
			object info = child.GetValue(UnfocusableStateInfoProperty);
			if (info == null)
				return;
			child.RestorePropertyValue(Control.IsEnabledProperty, info);
			child.ClearValue(UnfocusableStateInfoProperty);
		}
		protected override void AttachChildPropertyListeners(FrameworkElement child) {
			base.AttachChildPropertyListeners(child);
			AttachChildPropertyListener(child, "HorizontalAlignment", ChildHorizontalAlignmentListener);
			AttachChildPropertyListener(child, "VerticalAlignment", ChildVerticalAlignmentListener);
			AttachChildPropertyListener(child, "Width", ChildWidthListener);
			AttachChildPropertyListener(child, "Height", ChildHeightListener);
			AttachChildPropertyListener(child, "MaxWidth", ChildMaxWidthListener);
			AttachChildPropertyListener(child, "MaxHeight", ChildMaxHeightListener);
			AttachChildPropertyListener(child, "Style", ChildStyleListener);
			AttachChildPropertyListener(child, "Visibility", ChildVisibilityListener);
		}
		protected override void DetachChildPropertyListeners(FrameworkElement child) {
			base.DetachChildPropertyListeners(child);
			if (!(child.Parent is LayoutGroup)) {
				DetachChildPropertyListener(child, ChildHorizontalAlignmentListener);
				DetachChildPropertyListener(child, ChildVerticalAlignmentListener);
				DetachChildPropertyListener(child, ChildWidthListener);
				DetachChildPropertyListener(child, ChildHeightListener);
				DetachChildPropertyListener(child, ChildMaxWidthListener);
				DetachChildPropertyListener(child, ChildMaxHeightListener);
				DetachChildPropertyListener(child, ChildStyleListener);
				DetachChildPropertyListener(child, ChildVisibilityListener);
			}
		}
		protected override IEnumerable<UIElement> GetInternalElements() {
			foreach (UIElement element in BaseGetInternalElements())
				yield return element;
			if (Border != null)
				yield return Border;
		}
		protected virtual bool IsChildBorderless(ILayoutGroup child) {
			return !HasTabs;
		}
		protected virtual bool IsChildPermanent(ILayoutGroup child, bool keepTabs) {
			return HasTabs && keepTabs;
		}
		protected virtual bool IsChildUnfocusable(FrameworkElement child) {
#if SILVERLIGHT
			return IsLogicalChild(child) && IsLogicalChildUnfocusable(child) ||
				Parent is LayoutGroup && ((LayoutGroup)Parent).IsChildUnfocusable(this);
#else
			return IsLogicalChild(child) && IsLogicalChildUnfocusable(child);
#endif
		}
		protected virtual bool IsLogicalChildUnfocusable(FrameworkElement logicalChild) {
			return IsActuallyCollapsed || HasTabs && logicalChild != SelectedTabChild;
		}
		protected override bool IsTempChild(UIElement child) {
			return base.IsTempChild(child) || ItemSizers.IsItem(child);
		}
		protected virtual bool MakeChildVisible(FrameworkElement child) {
			if (HasGroupBox) {
				if (IsActuallyCollapsed)
					IsCollapsed = false;
				return !IsActuallyCollapsed;
			}
			if (HasTabs)
				return SelectTab(child);
			return true;
		}
		protected override void OnChildAdded(FrameworkElement child) {
			base.OnChildAdded(child);
			if (child.IsLayoutGroup())
				OnChildGroupAdded((ILayoutGroup)child);
			if (Root != null)
				Root.ControlAdded(child);
			ApplyItemStyle(child);
			CheckUnfocusableState(child);
		}
		protected override void OnChildRemoved(FrameworkElement child) {
			base.OnChildRemoved(child);
			if (Root != null)
				Root.ControlRemoved(child);
			if (child.Parent == null)
				ClearItemStyle(child);
			if (!(child.Parent is LayoutGroup) && child is Control)
				FinalizeUnfocusableState((Control)child);
		}
		protected virtual void OnChildGroupAdded(ILayoutGroup childGroup) {
			childGroup.IsCustomization = IsCustomization;
			childGroup.ItemSizerStyle = ItemSizerStyle;
			foreach (LayoutGroupPartStyle style in DevExpress.Utils.EnumExtensions.GetValues(typeof(LayoutGroupPartStyle)))
				childGroup.UpdatePartStyle(style);
		}
		protected virtual void OnChildGroupHeaderChanged(LayoutGroup childGroup) {
			UpdateTab(childGroup, false);
		}
		protected virtual void OnChildGroupHeaderTemplateChanged(LayoutGroup childGroup) {
			UpdateTab(childGroup, false);
		}
		protected override void OnChildPropertyChanged(FrameworkElement child, DependencyProperty propertyListener, object oldValue, object newValue) {
			base.OnChildPropertyChanged(child, propertyListener, oldValue, newValue);
			if (propertyListener == ChildHorizontalAlignmentListener)
				OnChildHorizontalAlignmentChanged(child);
			if (propertyListener == ChildVerticalAlignmentListener)
				OnChildVerticalAlignmentChanged(child);
			if (propertyListener == ChildWidthListener || propertyListener == ChildHeightListener ||
				propertyListener == ChildMaxWidthListener || propertyListener == ChildMaxHeightListener)
				OnChildSizeChanged(child);
			if (propertyListener == ChildStyleListener)
				OnChildStyleChanged(child);
			if (propertyListener == ChildVisibilityListener &&
#if !SILVERLIGHT
				newValue != null &&	 
#endif
				!newValue.Equals(oldValue))
				OnChildVisibilityChanged(child);
		}
		protected virtual void OnChildHorizontalAlignmentChanged(FrameworkElement child) {
			InvalidateParents(child);
		}
		protected virtual void OnChildVerticalAlignmentChanged(FrameworkElement child) {
			InvalidateParents(child);
		}
		protected virtual void OnChildSizeChanged(FrameworkElement child) {
			InvalidateParents(child);
		}
		protected virtual void OnChildStyleChanged(FrameworkElement child) {
			var item = child as LayoutItem;
			if (item == null)
				return;
			if (item.Style == null)
				ApplyItemStyle(item);
			else
				if (GetIsItemStyleSetByGroup(item) && item.Style != GetItemStyle())
					SetIsItemStyleSetByGroup(item, false);
		}
		protected virtual void OnChildVisibilityChanged(FrameworkElement child) {
			if (Root != null)
				Root.ControlVisibilityChanged(child);
		}
		protected bool HasNewChildren { get; private set; }
		protected override bool NeedsChildChangeNotifications { get { return true; } }
		private IEnumerable<UIElement> BaseGetInternalElements() {
			return base.GetInternalElements();
		}
		#endregion Children
		#region Layout
		protected override Size MeasureOverride(Size availableSize) {
			Size result = base.MeasureOverride(availableSize);
			if ((result.Width > availableSize.Width || result.Height > availableSize.Height) &&
				!IsRoot && Parent is LayoutGroup && !((LayoutGroup)Parent).IsMeasuring)
				((LayoutGroup)Parent).InvalidateMeasure();
			return result;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			HasNewChildren = false;
			return base.ArrangeOverride(finalSize);
		}
		protected override void OnBeforeMeasure(Size availableSize) {
			CheckTabControlIndex();
			UpdateTabs(false);
			UpdateTabControlSelectedIndex();
			CheckSelectedTabChildChanged();
			if (Border != null && BorderContent != null) {
#if !SILVERLIGHT
				Thickness oldBorderContentPadding = BorderContentPadding;
#endif
				BorderContentPadding = new Thickness(0);
				Size availableClientSize = CalculateClientBounds(availableSize).Size();
				Size originalAvailableClientSize = availableClientSize;
				BorderContent.IsEmpty = LayoutProvider.IsContentEmpty(GetLogicalChildren(true));
#if !SILVERLIGHT
				Border.InvalidateParentsOfModifiedChildren();
#endif
				Border.Measure(availableClientSize);
				if (IsBorderContentVisible) {
					bool isFlexibleWidth = BorderContent.AvailableSize.Width == 0 && double.IsNaN(Width) &&
						ActualHorizontalAlignment != HorizontalAlignment.Stretch;
					bool isFlexibleHeight = BorderContent.AvailableSize.Height == 0 && double.IsNaN(Height) &&
						ActualVerticalAlignment != VerticalAlignment.Stretch;
					if (isFlexibleWidth || isFlexibleHeight) {
						if (isFlexibleWidth)
							availableClientSize.Width = double.PositiveInfinity;
						if (isFlexibleHeight)
							availableClientSize.Height = double.PositiveInfinity;
						Border.Measure(availableClientSize);
					}
				}
				BorderMinSize = Border.DesiredSize;
				if (double.IsInfinity(availableClientSize.Width) || double.IsInfinity(availableClientSize.Height)) {
					if (double.IsInfinity(availableClientSize.Width))
						availableClientSize.Width = Border.DesiredSize.Width;
					if (double.IsInfinity(availableClientSize.Height))
						availableClientSize.Height = Border.DesiredSize.Height;
#if !SILVERLIGHT
					if (IsBorderContentVisible)
						BorderContent.InvalidateMeasure();
#endif
					Border.Measure(availableClientSize);
				}
				if (IsBorderContentVisible)
#if !SILVERLIGHT
					if (!BorderContent.IsMeasureValid)
						BorderContentPadding = oldBorderContentPadding;
					else
#endif
					BorderContentPadding = new Thickness(
						availableClientSize.Width - BorderContent.AvailableSize.Width,
						availableClientSize.Height - BorderContent.AvailableSize.Height,
						0, 0);
				Border.Measure(originalAvailableClientSize);
			}
			base.OnBeforeMeasure(availableSize);
		}
		protected override void OnBeforeArrange(Size finalSize) {
			if (Border != null && BorderContent != null) {
				Thickness oldBorderContentPadding = BorderContentPadding;
				BorderContentPadding = new Thickness(0);
				Rect clientBounds = CalculateClientBounds(finalSize);
				Border.Arrange(clientBounds);
				if (IsBorderContentVisible && BorderContent.GetVisualParent() != null) {
					Rect borderContentBounds = BorderContent.GetBounds(this);
					if (borderContentBounds == new Rect())  
						BorderContentPadding = oldBorderContentPadding;
					else
						BorderContentPadding = RectHelper.Padding(clientBounds, borderContentBounds);
				}
			}
			base.OnBeforeArrange(finalSize);
		}
		protected override Size OnMeasure(Size availableSize) {
			var result = base.OnMeasure(availableSize);
			InitChildrenMaxSizeAsDesiredSize();
			return result;
		}
		protected override Size OnArrange(Rect bounds) {
			ItemSizers.MarkItemsAsUnused();
			var result = base.OnArrange(bounds);
			ItemSizers.DeleteUnusedItems();
			if (!IsItemLabelsAlignmentScope)
				ItemLabelsAlignmentChanged();
			else
				LayoutProvider.AlignLayoutItemLabels(this, GetChildrenForItemLabelsAlignment());
			return result;
		}
		protected override Thickness GetClientPadding() {
			Thickness result = base.GetClientPadding();
			if (Border != null)
				ThicknessHelper.Inc(ref result, BorderContentPadding);
			return result;
		}
		protected override void UpdateOriginalDesiredSize(ref Size originalDesiredSize) {
			base.UpdateOriginalDesiredSize(ref originalDesiredSize);
			if (Border != null) {
				Thickness padding = ContentPadding;
				originalDesiredSize.Width = Math.Max(originalDesiredSize.Width, BorderMinSize.Width - BorderContentPadding.Left - (padding.Left + padding.Right));
				originalDesiredSize.Height = Math.Max(originalDesiredSize.Height, BorderMinSize.Height - BorderContentPadding.Top - (padding.Top + padding.Bottom));
			}
		}
		protected override LayoutProviderBase CreateLayoutProvider() {
			return new LayoutGroupProvider(this);
		}
		protected override LayoutParametersBase CreateLayoutProviderParameters() {
			return new LayoutGroupParameters(ItemSpace, CanItemSizing() ? ItemSizers : null);
		}
		protected virtual bool IsRemovableForOptimization(bool considerContent, bool keepEmptyTabs) {
			bool isEmpty = considerContent && GetLogicalChildren(false).Count == 0;
			return !GetIsPermanent(!isEmpty || keepEmptyTabs) && (!considerContent || isEmpty);
		}
		protected virtual bool OnOptimizeLayout(bool keepEmptyTabs) {
			bool result = false;
			foreach (FrameworkElement child in GetLogicalChildren(false))
				if (child.IsLayoutGroup() && ((ILayoutGroup)child).IsRemovableForOptimization(true, keepEmptyTabs)) {
					Children.Remove(child);
					result = true;
				}
			FrameworkElements children = GetLogicalChildren(false);
			if (IsRoot || !IsRemovableForOptimization(false, keepEmptyTabs)) {
				if (children.Count == 1 && children[0].IsLayoutGroup()) {
					var childGroup = (ILayoutGroup)children[0];
					if (childGroup.IsRemovableForOptimization(false, keepEmptyTabs)) {
						if (Orientation != childGroup.Orientation) {
							((ILayoutGroupModel)this).Orientation = childGroup.Orientation;
							result = true;
						}
						if (childGroup.MoveChildrenToParent()) {
							if (Header == null && HeaderTemplate == null) {
								if (childGroup.Header != null)
									Header = childGroup.Header;
								if (childGroup.HeaderTemplate != null)
									HeaderTemplate = childGroup.HeaderTemplate;
							}
							Children.Remove(childGroup.Control);
							result = true;
						}
					}
				}
			}
			else
				if (children.Count == 1 || children.Count > 1 && Parent is ILayoutGroup && ((ILayoutGroup)Parent).Orientation == Orientation) {
					MoveChildrenToParent();
					result = true;
				}
			return result;
		}
		protected virtual FrameworkElement Border {
			get {
				if (HasGroupBox)
					return GroupBox;
				if (HasTabs)
					return TabControl;
				return null;
			}
		}
		protected virtual BorderContentFiller BorderContent {
			get {
				if (HasGroupBox)
					return GroupBoxContent;
				if (HasTabs)
					return TabControlContent;
				return null;
			}
		}
		protected Thickness BorderContentPadding { get; set; }
		protected Size BorderMinSize { get; private set; }
		protected virtual LayoutGroupCollapseMode CollapseMode {
			get {
				if (IsActuallyCollapsed)
					return LayoutGroupCollapseMode.NoChildrenVisible;
				if (HasTabs)
					return LayoutGroupCollapseMode.OneChildVisible;
				return LayoutGroupCollapseMode.None;
			}
		}
		protected HorizontalAlignment DesiredHorizontalAlignment {
			get { return LayoutProvider.GetDesiredHorizontalAlignment(GetLogicalChildren(true)); }
		}
		protected VerticalAlignment DesiredVerticalAlignment {
			get { return LayoutProvider.GetDesiredVerticalAlignment(GetLogicalChildren(true)); }
		}
		protected virtual bool IsBorderContentVisible { get { return !HasGroupBox || !IsActuallyCollapsed; } }
		protected virtual bool IsBorderless { get { return View == LayoutGroupView.Group; } }
		protected virtual bool IsUIEmpty { get { return IsBorderless && GetLogicalChildren(true).Count == 0; } }
		protected new LayoutGroupProvider LayoutProvider { get { return (LayoutGroupProvider)base.LayoutProvider; } }
		protected virtual FrameworkElement UncollapsedChild { get { return HasTabs ? SelectedTabChild : null; } }
		protected class BorderContentFiller : Control {
			private bool _IsEmpty;
			public BorderContentFiller(LayoutGroup owner) {
				IsTabStop = false;
				Owner = owner;
			}
			public Size AvailableSize { get; private set; }
			public bool IsEmpty {
				get { return _IsEmpty; }
				set {
					if (IsEmpty == value)
						return;
					_IsEmpty = value;
#if SILVERLIGHT
					InvalidateMeasure();
#else
					FrameworkElement element = this;
					do {
						element.InvalidateMeasure();
						element = element.GetVisualParent();
					} while (element != null && element != Owner);
#endif
				}
			}
			public LayoutGroup Owner { get; private set; }
			protected readonly Size EmptyDesiredSize = new Size(60, 30);
			protected override Size MeasureOverride(Size availableSize) {
				AvailableSize = availableSize;
				if (IsEmpty) {
					SizeHelper.UpdateMinSize(ref availableSize, EmptyDesiredSize);
					return availableSize;
				}
				else
					return base.MeasureOverride(availableSize);
			}
#if !SILVERLIGHT
			protected override Size ArrangeOverride(Size arrangeBounds) {
				if (!Owner.IsArranging)
					Owner.InvalidateArrange();
				return base.ArrangeOverride(arrangeBounds);
			}
#endif
		}
		#endregion Layout
		#region GroupBox
		private static DependencyProperty CollapseDirectionProperty =
			DependencyProperty.Register("CollapseDirection", typeof(Orientation), typeof(LayoutGroup),
				new PropertyMetadata((o, e) => ((LayoutGroup)o).OnCollapseDirectionChanged()));
		protected void CheckGroupBox() {
			if (HasGroupBox && GroupBox == null) {
				GroupBox = CreateGroupBox();
				InitGroupBox();
				Children.Add(GroupBox);
#if SILVERLIGHT
				AddForegroundStyles(GroupBox, out GroupBoxStyles);
#endif
			}
			if (!HasGroupBox && GroupBox != null) {
#if SILVERLIGHT
				RemoveForegroundStyles(ref GroupBoxStyles);
#endif
				Children.Remove(GroupBox);
				GroupBox = null;
			}
		}
		protected virtual GroupBox CreateGroupBox() {
			return new GroupBox();
		}
		protected virtual HorizontalAlignment? GetGroupBoxHorizontalAlignment() {
			if (IsActuallyCollapsed && CollapseDirection == Orientation.Horizontal)
				return HorizontalAlignment.Left;
			else
				return null;
		}
		protected virtual VerticalAlignment? GetGroupBoxVerticalAlignment() {
			if (IsActuallyCollapsed && CollapseDirection == Orientation.Vertical)
				return VerticalAlignment.Top;
			else
				return null;
		}
		protected virtual GroupBoxState GetGroupBoxState() {
			return IsActuallyCollapsed ? GroupBoxState.Minimized : GroupBoxState.Normal;
		}
		protected virtual void UpdateActualGroupBoxDisplayMode() {
			GroupBoxDisplayMode groupBoxDisplayMode;
			if(GroupBoxDisplayMode != GroupBoxDisplayMode.Default || Root == this)
				groupBoxDisplayMode = GroupBoxDisplayMode;
			else groupBoxDisplayMode = Root != null ? Root.ActualGroupBoxDisplayMode : GroupBoxDisplayMode;
			ActualGroupBoxDisplayMode = groupBoxDisplayMode;
		}
		protected virtual void InitGroupBox() {
			GroupBox.Content = new BorderContentFiller(this);
			((BorderContentFiller)GroupBox.Content).InvalidateMeasure(); 
			GroupBox.SetBinding(GroupBox.HeaderProperty, new Binding("Header") { Source = this });
			InitGroupBoxHeaderTemplate();
			GroupBox.SetBinding(GroupBox.MinimizeElementVisibilityProperty,
				new Binding("IsCollapsible") { Source = this, Converter = new BoolToVisibilityConverter() });
			InitGroupBoxAlignment();
			InitGroupBoxDisplayMode();
			InitGroupBoxState();
			InitGroupBoxStyle();
			GroupBox.SetZIndex(LowZIndex);
			GroupBox.StateChanged += (o, e) => OnGroupBoxStateChanged();
			SetBinding(CollapseDirectionProperty, new Binding("MinimizationDirection") { Source = GroupBox });
		}
		protected void InitGroupBoxAlignment() {
			if (GroupBox == null)
				return;
			GroupBox.SetValue(HorizontalAlignmentProperty, GetGroupBoxHorizontalAlignment() ?? DependencyProperty.UnsetValue);
			GroupBox.SetValue(VerticalAlignmentProperty, GetGroupBoxVerticalAlignment() ?? DependencyProperty.UnsetValue);
		}
		protected void InitGroupBoxHeaderTemplate() {
			if (GroupBox != null)
				GroupBox.SetValueIfNotDefault(GroupBox.HeaderTemplateProperty, HeaderTemplate);
		}
		protected void InitGroupBoxState() {
			if (GroupBox != null)
				GroupBox.State = GetGroupBoxState();
		}
		protected void InitGroupBoxDisplayMode() {
			UpdateActualGroupBoxDisplayMode();
			if(GroupBox != null)
				GroupBox.DisplayMode = ActualGroupBoxDisplayMode;
		}
		protected void InitGroupBoxStyle() {
			if (GroupBox != null)
				GroupBox.SetValueIfNotDefault(GroupBox.StyleProperty, GetPartActualStyle(LayoutGroupPartStyle.GroupBox));
		}
		protected virtual void OnCollapseDirectionChanged() {
			InitGroupBoxAlignment();
			Changed();
			InvalidateParents(this);
		}
		protected virtual void OnGroupBoxStateChanged() {
			if (IsCollapsible) {
				bool newIsCollapsed = GroupBox.State == GroupBoxState.Minimized;
				if (IsCollapsed != newIsCollapsed) {
					IsCollapsed = newIsCollapsed;
					if (Root != null)
						Root.ModelChanged(new LayoutControlModelPropertyChangedEventArgs(this, "IsCollapsed", IsCollapsedProperty));
				}
			}
		}
		protected Orientation CollapseDirection {
			get { return (Orientation)GetValue(CollapseDirectionProperty); }
		}
		protected GroupBox GroupBox { get; private set; }
		protected virtual bool HasGroupBox { get { return View == LayoutGroupView.GroupBox; } }
		private BorderContentFiller GroupBoxContent { get { return (BorderContentFiller)GroupBox.Content; } }
#if SILVERLIGHT
		private ResourceDictionary GroupBoxStyles;
#endif
		#endregion GroupBox
		#region Tabs
		private bool _HasTabs;
		private FrameworkElement _SelectedTabChild;
		protected void CheckHasTabsChanged() {
			if (HasTabs == _HasTabs)
				return;
			_HasTabs = HasTabs;
			OnHasTabsChanged();
		}
		protected void CheckSelectedTabChildChanged() {
			if (!HasTabs)
				return;
			FrameworkElement selectedTabChild = SelectedTabChild;
			if (selectedTabChild == _SelectedTabChild)
				return;
			FrameworkElement oldSelectedTabChild = _SelectedTabChild;
			_SelectedTabChild = selectedTabChild;
			OnSelectedTabChildChanged(oldSelectedTabChild);
		}
		protected void CheckTabControl() {
			if (HasTabs && TabControl == null) {
				TabControl = CreateTabControl();
				InitTabControl();
				if (this.IsInDesignTool())
					Children.Add(TabControl);
				else
					Children.Insert(0, TabControl);
#if SILVERLIGHT
				AddForegroundStyles(TabControl, out TabControlStyles);
#else
				SetBinding(Control.ForegroundProperty, new Binding("Foreground") { Source = TabControl });
#endif
			}
			if (!HasTabs && TabControl != null) {
#if SILVERLIGHT
				RemoveForegroundStyles(ref TabControlStyles);
#else
				ClearValue(Control.ForegroundProperty);
#endif
				FinalizeTabControl();   
				Children.Remove(TabControl);
				TabControl = null;
			}
		}
		protected void CheckTabControlIndex() {
			if (!HasTabs || this.IsInDesignTool())
				return;
			int index = Children.IndexOf(TabControl);
			if (index == 0)
				return;
			Children.RemoveAt(index);
			Children.Insert(0, TabControl);
		}
		protected void CopyTabHeaderInfo(FrameworkElement fromChild, FrameworkElement toElement) {
			var toGroup = toElement as LayoutGroup;
			object tabHeader = GetTabHeader(fromChild, false);
			DataTemplate tabHeaderTemplate = GetTabHeaderTemplate(fromChild);
			if (toGroup != null && !toGroup.IsRoot) {
				if (this.IsInDesignTool())
					return;
				toGroup.SetValueIfNotDefault(HeaderProperty, tabHeader);
				toGroup.SetValueIfNotDefault(HeaderTemplateProperty, tabHeaderTemplate);
			}
			else {
				LayoutControl.SetTabHeader(toElement, tabHeader);
				LayoutControl.SetTabHeaderTemplate(toElement, tabHeaderTemplate);
			}
		}
		protected virtual DXTabItem CreateTab() {
			return new DXTabItem();
		}
		protected virtual DXTabControl CreateTabControl() {
			return new DXTabControl();
		}
		protected virtual object GetTabHeader(FrameworkElement child, bool useDefaultValues) {
			if (child == null)
				return useDefaultValues ? LocalizationRes.LayoutGroup_TabHeader_Empty : null;
			if (LayoutControl.GetTabHeader(child) != null)
				return LayoutControl.GetTabHeader(child);
			var childGroup = child as LayoutGroup;
			if (childGroup != null && !childGroup.IsRoot && childGroup.Header != null)
				return childGroup.Header;
			return useDefaultValues ? LocalizationRes.LayoutGroup_TabHeader_Default : null;
		}
		protected virtual DataTemplate GetTabHeaderTemplate(FrameworkElement child) {
			if (child == null)
				return null;
			if (LayoutControl.GetTabHeaderTemplate(child) != null)
				return LayoutControl.GetTabHeaderTemplate(child);
			var childGroup = child as LayoutGroup;
			if (childGroup != null && !childGroup.IsRoot && childGroup.HeaderTemplate != null)
				return childGroup.HeaderTemplate;
			return null;
		}
		protected int GetTabIndex(FrameworkElement child) {
			for (int result = 0; result < TabControl.Items.Count; result++)
				if (((DXTabItem)TabControl.Items[result]).Tag == child)
					return result;
			return -1;
		}
		protected int GetTabIndex(Point absolutePosition) {
			UIElement tab = TabControl.FindElement(absolutePosition,
				element => element is DXTabItem && TabControl.Items.Contains(element));
			return tab != null ? TabControl.Items.IndexOf(tab) : -1;
		}
		protected virtual void InitTabControl() {
			InitTabs(true);
			InitTabControlStyle();
			TabControl.SetZIndex(LowZIndex);
			TabControl.SelectionChanged += OnTabControlSelectionChanged;
			UpdateTabControlSelectedIndex();
			CheckSelectedTabChildChanged();
		}
		protected virtual void FinalizeTabControl() {
			FinalizeTabs();
		}
		protected void InitTabControlStyle() {
			if (!HasTabs)
				return;
			TabControl.SetValueIfNotDefault(DXTabControl.StyleProperty, GetPartActualStyle(LayoutGroupPartStyle.Tabs));
			TabControl.ApplyStyleValuesToPropertiesWithLocalValues();
		}
		protected virtual void InitTab(DXTabItem tab, FrameworkElement child, bool initStyle) {
			tab.Header = GetTabHeader(child, true);
			tab.SetValueIfNotDefault(DXTabItem.HeaderTemplateProperty, GetTabHeaderTemplate(child));
			BorderContentFiller borderContent = tab.Content as BorderContentFiller;
			if(borderContent == null)
				borderContent = new BorderContentFiller(this);
			borderContent.InvalidateMeasure(); 
			tab.Content = borderContent;
			if (initStyle)
				tab.SetValueIfNotDefault(DXTabItem.StyleProperty, GetPartActualStyle(LayoutGroupPartStyle.Tab));
			tab.Tag = child;
			if (child != null)
				tab.Visibility = child.Visibility;
		}
		protected virtual void FinalizeTab(DXTabItem tab) {
			tab.Tag = null;
		}
		protected virtual void InitTabs(bool initStyle) {
			FrameworkElements children = GetLogicalChildren(true);
			if (children.Count == 0 && TabControl.Items.Count == 1 && GetTabIndex(null) == 0) {
				InitTab((DXTabItem)TabControl.Items[0], null, initStyle);
				return;
			}
			for (int i = 0; i < children.Count; i++) {
				FrameworkElement child = children[i];
				int tabIndex = GetTabIndex(child);
				DXTabItem tab;
				if (tabIndex == -1) {
					tab = CreateTab();
					TabControl.Items.Insert(i, tab);
				}
				else {
					tab = (DXTabItem)TabControl.Items[tabIndex];
					if (tabIndex != i) {
						TabControl.Items.RemoveAt(tabIndex);
						TabControl.Items.Insert(i, tab);
					}
				}
				InitTab(tab, child, tabIndex == -1 || initStyle);
			}
			for (int i = TabControl.Items.Count - 1; i >= children.Count; i--) {
				FinalizeTab((DXTabItem)TabControl.Items[i]);
				TabControl.Items.RemoveAt(i);
			}
			if (children.Count == 0) {
				DXTabItem tab = CreateTab();
				TabControl.Items.Add(tab);
				InitTab(tab, null, true);
			}
		}
		protected virtual void FinalizeTabs() {
			foreach (DXTabItem tab in TabControl.Items)
				FinalizeTab(tab);
		}
		protected virtual void OnHasTabsChanged() {
			CheckTabControl();
			IsItemLabelsAlignmentScopeChanged();
			CheckUnfocusableStateForChildren();
		}
		protected virtual void OnSelectedTabChildChanged(FrameworkElement oldValue) {
			UnfocusTabChild(oldValue);
			CheckUnfocusableStateForChildren();
#if SILVERLIGHT
			InvalidateArrange();
#else
			if (MeasureSelectedTabChildOnly)
				InvalidateMeasure();
			else
				InvalidateArrange();
#endif
			if (SelectedTabChildChanged != null)
				SelectedTabChildChanged(this, new ValueChangedEventArgs<FrameworkElement>(oldValue, SelectedTabChild));
		}
		protected virtual bool OnTabControlDesignTimeClick(DXMouseButtonEventArgs args) {
			int tabIndex = GetTabIndex(args.GetPosition(null));
			if (tabIndex != -1) {
				if (TabControl.SelectedIndex != tabIndex)
					TabControl.SelectedIndex = tabIndex;
				else
					Root.TabClicked(this, SelectedTabChild);
				return true;
			}
			return false;
		}
		protected virtual void OnTabControlSelectionChanged() {
			((ILayoutGroup)this).SelectedTabIndex = TabControl.SelectedIndex;
			if (TabControl.SelectedIndex != -1)
				Root.TabClicked(this, SelectedTabChild);
		}
		protected void UnfocusTabChild(FrameworkElement child) {
			if (child == null)
				return;
#if SILVERLIGHT
			var focusedElement = FocusManager.GetFocusedElement() as FrameworkElement;
#else
			var focusedElement = FocusManager.GetFocusedElement(FocusManager.GetFocusScope(child)) as FrameworkElement;
#endif
			if (focusedElement != null && focusedElement.FindIsInParents(child))
				TabControl.Focus();
		}
		protected void UpdateTab(FrameworkElement child, bool updateStyle) {
			if (!HasTabs)
				return;
			int tabIndex = GetTabIndex(child);
			if (tabIndex != -1)
				InitTab((DXTabItem)TabControl.Items[tabIndex], child, updateStyle);
		}
		protected void UpdateTabControlSelectedIndex() {
			if (!HasTabs)
				return;
			DisableTabControlSelectionChangedNotification = true;
			TabControl.SelectedIndex = Math.Min(Math.Max(0, SelectedTabIndex), TabControl.Items.Count - 1);
			DisableTabControlSelectionChangedNotification = false;
			CheckSelectedTabChildChanged();
		}
		protected void UpdateTabs(bool updateStyle) {
			if (!HasTabs)
				return;
			DisableTabControlSelectionChangedNotification = true;
			InitTabs(updateStyle);
			DisableTabControlSelectionChangedNotification = false;
		}
		protected bool DisableTabControlSelectionChangedNotification { get; set; }
		protected virtual bool HasTabs { get { return View == LayoutGroupView.Tabs; } }
		protected DXTabControl TabControl { get; private set; }
		private void OnTabControlSelectionChanged(object sender, TabControlSelectionChangedEventArgs e) {
			if (!DisableTabControlSelectionChangedNotification)
				OnTabControlSelectionChanged();
		}
		private BorderContentFiller TabControlContent { get { return (BorderContentFiller)TabControl.SelectedItemContent; } }
#if SILVERLIGHT
		private ResourceDictionary TabControlStyles;
#endif
		#endregion Tabs
		#region Item Style
		private static readonly DependencyProperty IsItemStyleSetByGroupProperty =
			DependencyProperty.RegisterAttached("IsItemStyleSetByGroup", typeof(bool), typeof(LayoutGroup), null);
		protected static bool GetIsItemStyleSetByGroup(LayoutItem item) {
			return (bool)item.GetValue(IsItemStyleSetByGroupProperty);
		}
		protected static void SetIsItemStyleSetByGroup(LayoutItem item, bool value) {
			if (value)
				item.SetValue(IsItemStyleSetByGroupProperty, value);
			else
				item.ClearValue(IsItemStyleSetByGroupProperty);
		}
		protected void ApplyItemStyle(FrameworkElement element) {
			if (element is LayoutItem)
				ApplyItemStyle((LayoutItem)element);
			else
				if (element.IsLayoutGroup())
					((ILayoutGroup)element).ApplyItemStyle();
		}
		protected void ApplyItemStyle(LayoutItem item) {
			if (item.Style == null || GetIsItemStyleSetByGroup(item)) {
				item.SetValueIfNotDefault(LayoutItem.StyleProperty, GetItemStyle());
				SetIsItemStyleSetByGroup(item, true);
			}
		}
		protected void ClearItemStyle() {
			if (GetItemStyle() != null)
				return;
			foreach (FrameworkElement child in GetLogicalChildren(false))
				ClearItemStyle(child);
		}
		protected void ClearItemStyle(FrameworkElement element) {
			if (element is LayoutItem)
				ClearItemStyle((LayoutItem)element);
			else
				if (element.IsLayoutGroup())
					((ILayoutGroup)element).ClearItemStyle();
		}
		protected void ClearItemStyle(LayoutItem item) {
			if (!GetIsItemStyleSetByGroup(item))
				return;
			SetIsItemStyleSetByGroup(item, false);
			item.ClearValue(StyleProperty);
		}
		protected virtual Style GetItemStyle() {
			Style result = ItemStyle;
			if (result == null && !IsRoot && Parent is ILayoutGroup)
				result = ((ILayoutGroup)Parent).GetItemStyle();
			return result;
		}
		protected virtual void OnItemStyleChanged() {
			ApplyItemStyle();
		}
		#endregion Item Style
		#region XML Storage
		protected override void WriteToXMLCore(XmlWriter xml) {
			if (IsPermanent)
				xml.WriteAttributeString(XMLIDName, GetXMLID(this));
			base.WriteToXMLCore(xml);
		}
		protected override FrameworkElement ReadChildFromXMLCore(FrameworkElement element, XmlReader xml, IList children, int index, string id) {
			if (xml.Name == GetGroupType().Name) {
				var group = element as LayoutGroup;
				if (group == null) {
					group = CreateGroup();
					if (element == null)
						Root.SetID(group, id);
					LayoutControl.SetIsUserDefined(group, true);
					element = group;
				}
				AddChildFromXML(children, group, index);
				group.ReadFromXML(xml);
			}
			else
				element = base.ReadChildFromXMLCore(element, xml, children, index, id);
			return element;
		}
		protected override void WriteChildToXML(FrameworkElement child, XmlWriter xml) {
			if (child.GetType() == GetGroupType())
				((LayoutGroup)child).WriteToXML(xml);
			else
				base.WriteChildToXML(child, xml);
		}
		protected override void ReadChildrenFromXML(XmlReader xml, out FrameworkElement lastChild) {
			base.ReadChildrenFromXML(xml, out lastChild);
			MoveChildrenToAvailableItems(lastChild);
		}
		protected override void ReadCustomizablePropertiesFromXML(XmlReader xml) {
			base.ReadCustomizablePropertiesFromXML(xml);
			this.ReadPropertyFromXML(xml, HeaderProperty, "Header", typeof(object));
			this.ReadPropertyFromXML(xml, IsCollapsedProperty, "IsCollapsed", typeof(bool));
			this.ReadPropertyFromXML(xml, IsLockedProperty, "IsLocked", typeof(bool));
			this.ReadPropertyFromXML(xml, ItemSpaceProperty, "ItemSpace", typeof(double));
			this.ReadPropertyFromXML(xml, OrientationProperty, "Orientation", typeof(Orientation));
			this.ReadPropertyFromXML(xml, PaddingProperty, "Padding", typeof(Thickness));
			this.ReadPropertyFromXML(xml, SelectedTabIndexProperty, "SelectedTabIndex", typeof(int));
			this.ReadPropertyFromXML(xml, ViewProperty, "View", typeof(LayoutGroupView));
			this.ReadPropertyFromXML(xml, StoredSizeProperty, "StoredSize", typeof(double));	
			if (!IsRoot)
				ReadCustomizablePropertiesFromXML(this, xml);
			else
				Root.ReadElementFromXML(xml, this);
		}
		protected override void ReadCustomizablePropertiesFromXML(FrameworkElement element, XmlReader xml) {
			base.ReadCustomizablePropertiesFromXML(element, xml);
			element.ReadPropertyFromXML(xml, HorizontalAlignmentProperty, "HorizontalAlignment", typeof(HorizontalAlignment));
			element.ReadPropertyFromXML(xml, VerticalAlignmentProperty, "VerticalAlignment", typeof(VerticalAlignment));
			element.ReadPropertyFromXML(xml, WidthProperty, "Width", typeof(double));
			element.ReadPropertyFromXML(xml, HeightProperty, "Height", typeof(double));
			if (element is LayoutItem)
				((LayoutItem)element).ReadCustomizablePropertiesFromXML(xml);
			Root.ReadElementFromXML(xml, element);
		}
		protected override void WriteCustomizablePropertiesToXML(XmlWriter xml) {
			base.WriteCustomizablePropertiesToXML(xml);
			this.WritePropertyToXML(xml, HeaderProperty, "Header");
			this.WritePropertyToXML(xml, IsCollapsedProperty, "IsCollapsed");
			this.WritePropertyToXML(xml, IsLockedProperty, "IsLocked");
			this.WritePropertyToXML(xml, ItemSpaceProperty, "ItemSpace");
			this.WritePropertyToXML(xml, OrientationProperty, "Orientation");
			this.WritePropertyToXML(xml, PaddingProperty, "Padding");
			this.WritePropertyToXML(xml, SelectedTabIndexProperty, "SelectedTabIndex");
			this.WritePropertyToXML(xml, ViewProperty, "View");
			this.WritePropertyToXML(xml, StoredSizeProperty, "StoredSize");
			if (!IsRoot)
				WriteCustomizablePropertiesToXML(this, xml);
			else
				Root.WriteElementToXML(xml, this);
		}
		protected override void WriteCustomizablePropertiesToXML(FrameworkElement element, XmlWriter xml) {
			base.WriteCustomizablePropertiesToXML(element, xml);
			element.WritePropertyToXML(xml, HorizontalAlignmentProperty, "HorizontalAlignment");
			element.WritePropertyToXML(xml, VerticalAlignmentProperty, "VerticalAlignment");
			element.WritePropertyToXML(xml, WidthProperty, "Width");
			element.WritePropertyToXML(xml, HeightProperty, "Height");
			if (element is LayoutItem)
				((LayoutItem)element).WriteCustomizablePropertiesToXML(xml);
			Root.WriteElementToXML(xml, element);
		}
		protected override void AddChildFromXML(IList children, FrameworkElement element, int index) {
			DependencyObject oldElementParent = element.Parent;
			Root.AvailableItems.Remove(element);
			base.AddChildFromXML(children, element, index);
			if (element.Parent == this && oldElementParent != this)
				HasNewChildren = true;
		}
		protected override FrameworkElement FindByXMLID(string id) {
			return Root.FindControl(id);
		}
		protected override string GetXMLID(FrameworkElement element) {
			return Root.GetID(element);
		}
		#endregion XML Storage
		protected override PanelControllerBase CreateController() {
			return new LayoutGroupController(this);
		}
		protected virtual ElementSizers CreateItemSizers() {
			return new ElementSizers(this);
		}
		protected virtual Type GetGroupType() {
			return GetType();
		}
		protected virtual bool CanItemSizing() {
			return Root != null && Root.AllowItemSizing;
		}
		protected void CheckIsActuallyCollapsedChanged() {
			if (IsActuallyCollapsed == _IsActuallyCollapsed)
				return;
			_IsActuallyCollapsed = IsActuallyCollapsed;
			OnIsActuallyCollapsedChanged();
		}
#if SILVERLIGHT
		protected virtual Orientation GetDefaultOrientation() {
			return DefaultOrientation;
		}
		protected override Thickness GetDefaultPadding() {
			return new Thickness(0);
		}
#endif
		protected void ForEachGroup(Action<ILayoutGroup> action) {
			foreach (FrameworkElement child in GetLogicalChildren(false))
				if (child.IsLayoutGroup())
					action((ILayoutGroup)child);
		}
		protected virtual FrameworkElements GetChildrenForItemLabelsAlignment() {
			if (IsActuallyCollapsed)
				return new FrameworkElements();
			if (HasTabs) {
				var result = new FrameworkElements();
				if (SelectedTabChild != null)
					result.Add(SelectedTabChild);
				return result;
			}
			return GetLogicalChildren(true);
		}
		protected Rect GetClipBounds(FrameworkElement relativeTo) {
			if (!IsRoot && Parent is ILayoutGroup)
				return ((ILayoutGroup)Parent).GetClipBounds(this, relativeTo);
			else
				return this.GetVisualBounds(relativeTo);
		}
		protected virtual bool GetIsPermanent() {
			return View != LayoutGroupView.Group || !string.IsNullOrEmpty(Name);
		}
		protected bool GetIsPermanent(bool keepTabs) {
			return GetIsPermanent() || !IsRoot && Parent is ILayoutGroup && ((ILayoutGroup)Parent).IsChildPermanent(this, keepTabs);
		}
		protected ILayoutGroup GetItemLabelsAlignmentScope(FrameworkElement item) {
			while (item != null && !(item is ILayoutGroup && ((ILayoutGroup)item).IsItemLabelsAlignmentScope))
				item = (FrameworkElement)item.Parent;
			return (ILayoutGroup)item;
		}
		protected Style GetPartActualStyle(LayoutGroupPartStyle style) {
			return GetPartStyle(style) ?? (Root != null ? Root.GetPartStyle(style) : null);
		}
		protected Style GetPartStyle(LayoutGroupPartStyle style) {
			switch (style) {
				case LayoutGroupPartStyle.GroupBox:
					return GroupBoxStyle;
				case LayoutGroupPartStyle.Tabs:
					return TabsStyle;
				case LayoutGroupPartStyle.Tab:
					return TabStyle;
				default:
					return null;
			}
		}
		protected virtual void InitChildrenMaxSizeAsDesiredSize() {
			foreach(var child in GetLogicalChildren(true)) {
				if(LayoutControl.GetUseDesiredWidthAsMaxWidth(child) && child.DesiredSize.Width != 0 && double.IsInfinity(child.MaxWidth))
					child.MaxWidth = child.DesiredSize.Width;
				if(LayoutControl.GetUseDesiredHeightAsMaxHeight(child) && child.DesiredSize.Height != 0 && double.IsInfinity(child.MaxHeight))
					child.MaxHeight = child.DesiredSize.Height;
			}
		}
		protected virtual void InitGroupForChild(LayoutGroup group, FrameworkElement child) {
			if (HasTabs)
				CopyTabHeaderInfo(child, group);
			else
				group.Orientation = Orientation;
			LayoutProvider.InitGroupForChild(group, child);
		}
		protected void InitPartStyle(LayoutGroupPartStyle style) {
			switch (style) {
				case LayoutGroupPartStyle.GroupBox:
					InitGroupBoxStyle();
					break;
				case LayoutGroupPartStyle.Tabs:
					InitTabControlStyle();
					break;
				case LayoutGroupPartStyle.Tab:
					UpdateTabs(true);
					break;
			}
		}
		protected void IsItemLabelsAlignmentScopeChanged() {
			ILayoutGroup scope = GetItemLabelsAlignmentScope((FrameworkElement)Parent);
			if (scope != null)
				scope.LayoutItemLabelsAlignmentChanged();
			if (IsItemLabelsAlignmentScope)
				UpdateItemLabelsAlignment();
		}
		protected void ItemLabelsAlignmentChanged() {
			ILayoutGroup scope = GetItemLabelsAlignmentScope(this);
			if (scope != null)
				scope.LayoutItemLabelsAlignmentChanged();
		}
		protected void MoveChildrenToAvailableItems(FrameworkElement lastRemainingChild = null) {
			FrameworkElements children = GetLogicalChildren(false);
			int startIndex = lastRemainingChild != null ? children.IndexOf(lastRemainingChild) + 1 : 0;
			for (int i = startIndex; i < children.Count; i++) {
				FrameworkElement child = children[i];
				if (child.GetType() == GetGroupType() && !((LayoutGroup)child).IsPermanent)
					((LayoutGroup)child).MoveChildrenToAvailableItems();
				else
					Root.AvailableItems.Add(child);
			}
		}
		protected void MoveNonUserDefinedChildrenToAvailableItems() {
			foreach (FrameworkElement child in GetLogicalChildren(false))
				if (!LayoutControl.GetIsUserDefined(child))
					Root.AvailableItems.Add(child);
				else
					if (child.IsLayoutGroup())
						((ILayoutGroup)child).MoveNonUserDefinedChildrenToAvailableItems();
		}
		protected virtual void OnAllowItemSizingChanged() {
			InvalidateArrange();
			ForEachGroup(group => group.AllowItemSizingChanged());
		}
		protected virtual void OnAllowHorizontalSizingChanged(FrameworkElement child) {
			InvalidateArrange();
		}
		protected virtual void OnAllowVerticalSizingChanged(FrameworkElement child) {
			InvalidateArrange();
		}
		protected override void OnAttachedPropertyChanged(FrameworkElement child, DependencyProperty property, object oldValue, object newValue) {
			base.OnAttachedPropertyChanged(child, property, oldValue, newValue);
			if (property == LayoutControl.AllowHorizontalSizingProperty)
				OnAllowHorizontalSizingChanged(child);
			if (property == LayoutControl.AllowVerticalSizingProperty)
				OnAllowVerticalSizingChanged(child);
			if (property == LayoutControl.TabHeaderProperty)
				OnTabHeaderChanged(child);
			if (property == LayoutControl.TabHeaderTemplateProperty)
				OnTabHeaderTemplateChanged(child);
			if (property == LayoutControl.UseDesiredWidthAsMaxWidthProperty)
				OnUseDesiredWidthAsMaxWidthChanged(child);
			if (property == LayoutControl.UseDesiredHeightAsMaxHeightProperty)
				OnUseDesiredHeightAsMaxHeightChanged(child);
		}
		protected virtual void OnCollapsed() {
			if (Collapsed != null)
				Collapsed(this, EventArgs.Empty);
		}
		protected virtual bool OnDesignTimeClick(DXMouseButtonEventArgs args) {
			return HasGroupBox && ((IGroupBox)GroupBox).DesignTimeClick(args) ||
				HasTabs && OnTabControlDesignTimeClick(args);
		}
		protected virtual void OnExpanded() {
			if (Expanded != null)
				Expanded(this, EventArgs.Empty);
		}
		protected virtual IEnumerable<Rect> OnGetLiveCustomizationAreas(FrameworkElement relativeTo) {
			foreach (ElementSizer itemSizer in ItemSizers)
				yield return itemSizer.GetBounds(relativeTo);
			if (HasGroupBox && GroupBox.MinimizeElementVisibility == Visibility.Visible) {
				Rect bounds = ((IGroupBox)GroupBox).MinimizeElementBounds;
				if (!bounds.IsEmpty) {
					bounds = GroupBox.MapRect(bounds, relativeTo);
					yield return bounds;
				}
			}
			if (HasTabs) {
				if (TabControl.TabPanel != null && TabControl.TabPanel.IsInVisualTree())
					yield return TabControl.TabPanel.GetBounds(relativeTo);
				if (TabControl.PrevButton != null && TabControl.PrevButton.IsInVisualTree())
					yield return TabControl.PrevButton.GetBounds(relativeTo);
				if (TabControl.NextButton != null && TabControl.NextButton.IsInVisualTree())
					yield return TabControl.NextButton.GetBounds(relativeTo);
			}
		}
		protected virtual void OnHeaderChanged() {
			if (!IsRoot && Parent is LayoutGroup)
				((LayoutGroup)Parent).OnChildGroupHeaderChanged(this);
		}
		protected virtual void OnHeaderTemplateChanged() {
			InitGroupBoxHeaderTemplate();
			if (!IsRoot && Parent is LayoutGroup)
				((LayoutGroup)Parent).OnChildGroupHeaderTemplateChanged(this);
		}
		protected virtual void OnIsActuallyCollapsedChanged() {
			InitGroupBoxAlignment();
			InitGroupBoxState();
			UpdateCollapsedSize();
			IsItemLabelsAlignmentScopeChanged();
			Changed();
			if (Parent is UIElement)
				((UIElement)Parent).InvalidateMeasure();
			CheckUnfocusableStateForChildren();
			if (IsActuallyCollapsed)
				OnCollapsed();
			else
				OnExpanded();
		}
		protected virtual void OnIsCollapsedChanged() {
			CheckIsActuallyCollapsedChanged();
		}
		protected virtual void OnIsCollapsibleChanged() {
			CheckIsActuallyCollapsedChanged();
		}
		protected virtual void OnIsCustomizationChanged() {
			InvalidateArrange();
		}
		protected virtual void OnItemLabelsAlignmentChanged() {
			IsItemLabelsAlignmentScopeChanged();
		}
		protected override void OnItemSpaceChanged(double oldValue, double newValue) {
			base.OnItemSpaceChanged(oldValue, newValue);
			ItemSizers.SizingAreaWidth = ItemSpace;
		}
#if !SILVERLIGHT
		protected virtual void OnMeasureSelectedTabChildOnlyChanged() {
			InvalidateMeasure();
		}
#endif
		protected virtual void OnOrientationChanged() {
			Changed();
		}
		protected virtual void OnPartStyleChanged(LayoutGroupPartStyle style) {
			InitPartStyle(style);
		}
		protected virtual void OnSelectedTabIndexChanged(int oldValue) {
			UpdateTabControlSelectedIndex();
		}
		protected virtual void OnTabHeaderChanged(FrameworkElement child) {
			UpdateTab(child, false);
		}
		protected virtual void OnTabHeaderTemplateChanged(FrameworkElement child) {
			UpdateTab(child, false);
		}
		protected virtual void OnUseDesiredWidthAsMaxWidthChanged(FrameworkElement child) {
			if(LayoutControl.GetUseDesiredWidthAsMaxWidth(child) && child.DesiredSize.Width != 0) {
				var prevChildWidth = child.Width;
				if(!double.IsNaN(prevChildWidth)) {
					child.Width = double.NaN;
					UpdateLayout();
				}
				child.MaxWidth = child.DesiredSize.Width;
				child.Width = prevChildWidth;
			}
			if(!LayoutControl.GetUseDesiredWidthAsMaxWidth(child))
				child.MaxWidth = double.PositiveInfinity;
		}
		protected virtual void OnUseDesiredHeightAsMaxHeightChanged(FrameworkElement child) {
			if(LayoutControl.GetUseDesiredHeightAsMaxHeight(child) && child.DesiredSize.Height != 0) {
				var prevChildHeight = child.Height;
				if(!double.IsNaN(prevChildHeight)) {
					child.Height = double.NaN;
					UpdateLayout();
				}
				child.MaxHeight = child.DesiredSize.Height;
				child.Height = prevChildHeight;
			}
			if(!LayoutControl.GetUseDesiredHeightAsMaxHeight(child))
				child.MaxHeight = double.PositiveInfinity;
		}
		protected virtual void OnViewChanged() {
			CheckGroupBox();
			CheckIsActuallyCollapsedChanged();
			CheckHasTabsChanged();
			Changed();
		}
		protected virtual void UpdateCollapsedSize() {
			DependencyProperty sizeProperty = CollapseDirection == Orientation.Horizontal ? WidthProperty : HeightProperty;
			DependencyProperty minSizeProperty = CollapseDirection == Orientation.Horizontal ? MinWidthProperty : MinHeightProperty;
			if (IsActuallyCollapsed) {
				var size = (double)GetValue(sizeProperty);
				var minSize = (double)GetValue(minSizeProperty);
				if (!double.IsNaN(size)) {
					StoredSize = size;
					SetValue(sizeProperty, double.NaN);
				}
				if (minSize != 0) {
					_StoredMinSizePropertyValue = this.StorePropertyValue(minSizeProperty);
					SetValue(minSizeProperty, 0.0);
				}
			}
			if (!IsActuallyCollapsed) {
				if (this.IsPropertyAssigned(StoredSizeProperty)) {
					SetValue(sizeProperty, StoredSize);
					ClearValue(StoredSizeProperty);
				}
				if (_StoredMinSizePropertyValue != null) {
					this.RestorePropertyValue(minSizeProperty, _StoredMinSizePropertyValue);
					_StoredMinSizePropertyValue = null;
				}
			}
		}
		protected virtual void UpdateItemLabelsAlignment() {
			if (!IsArranging)
				InvalidateArrange();
		}
		protected bool IsCustomization {
			get { return _IsCustomization; }
			set {
				if (IsCustomization == value)
					return;
				_IsCustomization = value;
				ForEachGroup(group => group.IsCustomization = IsCustomization);
				OnIsCustomizationChanged();
			}
		}
		protected virtual bool IsItemLabelsAlignmentScope {
			get { return ItemLabelsAlignment != LayoutItemLabelsAlignment.Default || IsActuallyCollapsed || HasTabs; }
		}
		protected ElementSizers ItemSizers { get; private set; }
		protected Style ItemSizerStyle {
			get { return ItemSizers.ItemStyle; }
			set {
				if (ItemSizerStyle == value)
					return;
				ItemSizers.ItemStyle = value;
				ForEachGroup(group => group.ItemSizerStyle = ItemSizerStyle);
			}
		}
		[XtraSerializableProperty]
		protected double StoredSize {
			get { return (double)GetValue(StoredSizeProperty); }
			set { SetValue(StoredSizeProperty, value); }
		}
		protected virtual Orientation VisibleOrientation {
			get {
				if (HasTabs) {
					HeaderLocation tabsLocation = TabControl.View.HeaderLocation;
					return tabsLocation == HeaderLocation.Top || tabsLocation == HeaderLocation.Bottom ? Orientation.Horizontal : Orientation.Vertical;
				}
				else
					return Orientation;
			}
		}
#if SILVERLIGHT
		protected void AddForegroundStyles(Control source, out ResourceDictionary styles) {
			styles = CreateForegroundStyles(source);
			Resources.MergedDictionaries.Add(styles);
		}
		protected void RemoveForegroundStyles(ref ResourceDictionary styles) {
			Resources.MergedDictionaries.Remove(styles);
			styles = null;
		}
		private Style CreateForegroundStyle(Type targetType, DependencyProperty targetForegroundProperty, Control source) {
			var result = new Style(targetType);
			result.Setters.Add(new Setter(targetForegroundProperty, new Binding("Foreground") { Source = source }));
			return result;
		}
		private ResourceDictionary CreateForegroundStyles(Control source) {
			var result = new ResourceDictionary();
			result.Add(typeof(TextBlock),
				CreateForegroundStyle(typeof(TextBlock), TextBlock.ForegroundProperty, source));
			result.Add(typeof(DXContentPresenter),
				CreateForegroundStyle(typeof(DXContentPresenter), DXContentPresenter.ForegroundProperty, source));
			result.Add(typeof(LayoutItemLabel),
				CreateForegroundStyle(typeof(LayoutItemLabel), LayoutItemLabel.ForegroundProperty, source));
			return result;
		}
#endif
		#region ILayoutGroupModel
		Orientation ILayoutGroupModel.CollapseDirection { get { return CollapseDirection; } }
		LayoutGroupCollapseMode ILayoutGroupModel.CollapseMode { get { return CollapseMode; } }
#if !SILVERLIGHT
		bool ILayoutGroupModel.MeasureUncollapsedChildOnly { get { return MeasureSelectedTabChildOnly; } }
#endif
		Orientation ILayoutGroupModel.Orientation {
			get { return Orientation; }
			set {
				if (Orientation == value)
					return;
				if (value == DefaultOrientation) {
					ClearValue(OrientationProperty);
					if (Orientation == value)
						return;
				}
				Orientation = value;
			}
		}
		FrameworkElement ILayoutGroupModel.UncollapsedChild { get { return UncollapsedChild; } }
		#endregion ILayoutGroupModel
		#region ILayoutGroup
		void ILayoutGroup.AllowItemSizingChanged() {
			OnAllowItemSizingChanged();
		}
		void ILayoutGroup.ChildHorizontalAlignmentChanged(FrameworkElement child) {
			OnChildHorizontalAlignmentChanged(child);
		}
		void ILayoutGroup.ChildVerticalAlignmentChanged(FrameworkElement child) {
			OnChildVerticalAlignmentChanged(child);
		}
		void ILayoutGroup.ClearItemStyle() {
			ClearItemStyle();
		}
		void ILayoutGroup.CopyTabHeaderInfo(FrameworkElement fromChild, FrameworkElement toElement) {
			CopyTabHeaderInfo(fromChild, toElement);
		}
		bool ILayoutGroup.DesignTimeClick(DXMouseButtonEventArgs args) {
			return OnDesignTimeClick(args);
		}
		Rect ILayoutGroup.GetClipBounds(FrameworkElement child, FrameworkElement relativeTo) {
			return LayoutProvider.GetClipBounds(this, child, relativeTo);
		}
		IEnumerable<string> ILayoutGroup.GetDependencyPropertiesWithOverriddenDefaultValue() {
			yield return "Padding";
		}
		LayoutItemInsertionInfo ILayoutGroup.GetInsertionInfoForEmptyArea(FrameworkElement element, Point p) {
			return LayoutProvider.GetInsertionInfoForEmptyArea(this, element, p);
		}
		LayoutItemInsertionKind ILayoutGroup.GetInsertionKind(FrameworkElement destinationItem, Point p) {
			if (destinationItem != this && destinationItem.IsLayoutGroup() && !((ILayoutGroup)destinationItem).IsLocked)
				return ((ILayoutGroup)destinationItem).GetInsertionKind(destinationItem, p);
			else
				return LayoutProvider.GetInsertionKind(this, destinationItem, p);
		}
		LayoutItemInsertionPoint ILayoutGroup.GetInsertionPoint(FrameworkElement element, FrameworkElement destinationItem,
			LayoutItemInsertionKind insertionKind, Point p) {
			var insertionPoints = new LayoutItemInsertionPoints();
			((ILayoutGroup)this).GetInsertionPoints(element, destinationItem, destinationItem, insertionKind, insertionPoints);
			for (int i = 0; i < insertionPoints.Count; i++)
				if (((ILayoutGroup)this).GetInsertionPointZoneBounds(destinationItem, insertionKind, i, insertionPoints.Count).Contains(p))
					return insertionPoints[i];
			return null;
		}
		Rect ILayoutGroup.GetInsertionPointBounds(bool isInternalInsertion, FrameworkElement relativeTo) {
			if (isInternalInsertion)
				return this.MapRect(ContentBounds, relativeTo);
			else
				return this.GetBounds(relativeTo);
		}
		void ILayoutGroup.GetInsertionPoints(FrameworkElement element, FrameworkElement destinationItem, FrameworkElement originalDestinationItem,
			LayoutItemInsertionKind insertionKind, LayoutItemInsertionPoints points) {
			LayoutProvider.GetInsertionPoints(this, element, destinationItem, originalDestinationItem, insertionKind, points);
		}
		Rect ILayoutGroup.GetInsertionPointZoneBounds(FrameworkElement destinationItem, LayoutItemInsertionKind insertionKind,
			int pointIndex, int pointCount) {
			if (destinationItem != this && destinationItem.IsLayoutGroup() && !((ILayoutGroup)destinationItem).IsLocked)
				return ((ILayoutGroup)destinationItem).GetInsertionPointZoneBounds(destinationItem, insertionKind, pointIndex, pointCount);
			else
				return LayoutProvider.GetInsertionPointZoneBounds(this, destinationItem, insertionKind, pointIndex, pointCount);
		}
		FrameworkElement ILayoutGroup.GetItem(Point p, bool ignoreLayoutGroups, bool ignoreLocking) {
			return Controller.GetItem(p, ignoreLayoutGroups, ignoreLocking);
		}
		HorizontalAlignment ILayoutGroup.GetItemHorizontalAlignment(FrameworkElement item) {
			return LayoutProvider.GetItemHorizontalAlignment(item);
		}
		VerticalAlignment ILayoutGroup.GetItemVerticalAlignment(FrameworkElement item) {
			return LayoutProvider.GetItemVerticalAlignment(item);
		}
		Style ILayoutGroup.GetItemStyle() {
			return GetItemStyle();
		}
		void ILayoutGroup.GetLayoutItems(LayoutItems layoutItems) {
			if(!IsItemLabelsAlignmentScope)
				LayoutProvider.GetLayoutItems(GetLogicalChildren(true), layoutItems);
		}
		int ILayoutGroup.GetTabIndex(Point absolutePosition) {
			return HasTabs ? GetTabIndex(absolutePosition) : -1;
		}
		void ILayoutGroup.InitChildFromGroup(FrameworkElement child, FrameworkElement group) {
			LayoutProvider.InitChildFromGroup(child, group);
		}
		void ILayoutGroup.InsertElement(FrameworkElement element, LayoutItemInsertionPoint insertionPoint, LayoutItemInsertionKind insertionKind) {
			LayoutProvider.InsertElement(this, element, insertionPoint, insertionKind);
		}
		bool ILayoutGroup.IsChildBorderless(ILayoutGroup child) {
			return IsChildBorderless(child);
		}
		bool ILayoutGroup.IsChildPermanent(ILayoutGroup child, bool keepTabs) {
			return IsChildPermanent(child, keepTabs);
		}
		bool ILayoutGroup.IsExternalInsertionPoint(FrameworkElement element, FrameworkElement destinationItem,
			LayoutItemInsertionKind insertionKind) {
			return LayoutProvider.IsExternalInsertionPoint(this, element, destinationItem, insertionKind);
		}
		bool ILayoutGroup.IsRemovableForOptimization(bool considerContent, bool keepEmptyTabs) {
			return IsRemovableForOptimization(considerContent, keepEmptyTabs);
		}
		void ILayoutGroup.LayoutItemLabelWidthChanged(FrameworkElement layoutItem) {
			ItemLabelsAlignmentChanged();
		}
		void ILayoutGroup.LayoutItemLabelsAlignmentChanged() {
			UpdateItemLabelsAlignment();
		}
		bool ILayoutGroup.MakeChildVisible(FrameworkElement child) {
			return MakeChildVisible(child);
		}
		void ILayoutGroup.MoveNonUserDefinedChildrenToAvailableItems() {
			MoveNonUserDefinedChildrenToAvailableItems();
		}
		void ILayoutGroup.SetItemHorizontalAlignment(FrameworkElement item, HorizontalAlignment value, bool updateWidth) {
			LayoutProvider.SetItemHorizontalAlignment(item, value, updateWidth);
		}
		void ILayoutGroup.SetItemVerticalAlignment(FrameworkElement item, VerticalAlignment value, bool updateHeight) {
			LayoutProvider.SetItemVerticalAlignment(item, value, updateHeight);
		}
		void ILayoutGroup.UpdatePartStyle(LayoutGroupPartStyle style) {
			InitPartStyle(style);
			ForEachGroup(group => group.UpdatePartStyle(style));
		}
		Rect ILayoutGroup.ChildAreaBounds { get { return ContentBounds; } }
		HorizontalAlignment ILayoutGroup.DesiredHorizontalAlignment { get { return DesiredHorizontalAlignment; } }
		VerticalAlignment ILayoutGroup.DesiredVerticalAlignment { get { return DesiredVerticalAlignment; } }
		bool ILayoutGroup.HasNewChildren {
			get {
				if (HasNewChildren)
					return true;
				foreach (FrameworkElement child in GetLogicalChildren(false))
					if (child.IsLayoutGroup() && ((ILayoutGroup)child).HasNewChildren)
						return true;
				return false;
			}
		}
		bool ILayoutGroup.HasUniformLayout { get { return LayoutProvider.IsLayoutUniform(GetLogicalChildren(true)); } }
		bool ILayoutGroup.IsBorderless { get { return IsBorderless; } }
		bool ILayoutGroup.IsCustomization {
			get { return IsCustomization; }
			set { IsCustomization = value; }
		}
		bool ILayoutGroup.IsItemLabelsAlignmentScope { get { return IsItemLabelsAlignmentScope; } }
		bool ILayoutGroup.IsUIEmpty { get { return IsUIEmpty; } }
		Style ILayoutGroup.ItemSizerStyle {
			get { return ItemSizerStyle; }
			set { ItemSizerStyle = value; }
		}
		int ILayoutGroup.SelectedTabIndex {
			get { return TabControl.SelectedIndex; }
			set {
				SelectedTabIndex = value;
			}
		}
		Orientation ILayoutGroup.VisibleOrientation { get { return VisibleOrientation; } }
		#endregion ILayoutGroup
		#region ILiveCustomizationAreasProvider
		void ILiveCustomizationAreasProvider.GetLiveCustomizationAreas(IList<Rect> areas, FrameworkElement relativeTo) {
			Rect clipBounds = GetClipBounds(relativeTo);
			if (clipBounds.IsEmpty)
				return;
			foreach (Rect area in OnGetLiveCustomizationAreas(relativeTo)) {
				area.Intersect(clipBounds);
				if (!area.IsEmpty)
					areas.Add(area);
			}
			foreach (FrameworkElement child in GetLogicalChildren(true))
				if (child is ILiveCustomizationAreasProvider)
					((ILiveCustomizationAreasProvider)child).GetLiveCustomizationAreas(areas, relativeTo);
		}
		#endregion ILiveCustomizationAreasProvider
		#region ILayoutControlCustomizableItem
		FrameworkElement ILayoutControlCustomizableItem.AddNewItem() {
			if (!HasTabs)
				return null;
			LayoutGroup tab = CreateGroup();
			Children.Add(tab);
			return tab;
		}
		bool ILayoutControlCustomizableItem.CanAddNewItems { get { return HasTabs; } }
		bool ILayoutControlCustomizableItem.HasHeader { get { return true; } }
		object ILayoutControlCustomizableItem.Header {
			get { return Header; }
			set { Header = value; }
		}
		#endregion ILayoutControlCustomizableItem
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.LayoutGroupAutomationPeer(this);
		}
		#endregion
	}
	public class LayoutGroupParameters : LayoutParametersBase {
		public LayoutGroupParameters(double itemSpace, ElementSizers itemSizers)
			: base(itemSpace) {
			ItemSizers = itemSizers;
		}
		public ElementSizers ItemSizers { get; private set; }
	}
	public class LayoutGroupProvider : LayoutProviderBase {
		public LayoutGroupProvider(ILayoutGroupModel model)
			: base(model) {
		}
		public void ArrangeItems(FrameworkElements items) {
			CreateItemInfos(0, 0).ArrangeItems(items, GetItemAlignment);
		}
		public override void CopyLayoutInfo(FrameworkElement from, FrameworkElement to) {
			base.CopyLayoutInfo(from, to);
			LayoutControl.SetAllowHorizontalSizing(to, LayoutControl.GetAllowHorizontalSizing(from));
			LayoutControl.SetAllowVerticalSizing(to, LayoutControl.GetAllowVerticalSizing(from));
			if (from.IsLayoutGroup()) {
				var group = (ILayoutGroup)from;
				var size = group.ActualMaxSize;
				to.MaxWidth = size.Width;
				to.MaxHeight = size.Height;
				to.HorizontalAlignment = group.ActualHorizontalAlignment;
				to.VerticalAlignment = group.ActualVerticalAlignment;
			}
			var parentGroup = from.Parent as ILayoutGroup;
			if (parentGroup != null)
				parentGroup.CopyTabHeaderInfo(from, to);
		}
		public virtual Size GetActualMinOrMaxSize(FrameworkElements items, bool getMinSize, LayoutParametersBase parameters) {
			if (items.Count == 0)
				return getMinSize ? SizeHelper.Zero : SizeHelper.Infinite;
			if (Model.CollapseMode == LayoutGroupCollapseMode.None)
				return GetActualMinOrMaxSizeWhenUncollapsed(items, getMinSize, parameters);
			else
				return GetActualMinOrMaxSizeWhenCollapsed(items, getMinSize, parameters);
		}
		public virtual Rect GetClipBounds(FrameworkElement control, FrameworkElement child, FrameworkElement relativeTo) {
			return child.GetVisualBounds(relativeTo);
		}
		public virtual HorizontalAlignment GetDesiredHorizontalAlignment(FrameworkElements items) {
			var result = HorizontalAlignment.Left;
			bool isOneStretchedItemMode = Model.CollapseMode == LayoutGroupCollapseMode.OneChildVisible ||
				Model.Orientation == Orientation.Vertical;
			bool isFirstItem = true;
			foreach (FrameworkElement item in items) {
				HorizontalAlignment alignment = GetItemHorizontalAlignment(item);
				if (isOneStretchedItemMode && alignment == HorizontalAlignment.Stretch ||
					!isOneStretchedItemMode && !isFirstItem && alignment != result) {
					result = HorizontalAlignment.Stretch;
					break;
				}
				if (isFirstItem) {
					isFirstItem = false;
					if (!isOneStretchedItemMode)
						result = alignment;
				}
			}
			if (result == HorizontalAlignment.Stretch &&
				Model.CollapseMode == LayoutGroupCollapseMode.NoChildrenVisible && Model.CollapseDirection == Orientation.Horizontal)
				result = HorizontalAlignment.Left;
			return result;
		}
		public virtual VerticalAlignment GetDesiredVerticalAlignment(FrameworkElements items) {
			var result = VerticalAlignment.Top;
			bool isOneStretchedItemMode = Model.CollapseMode == LayoutGroupCollapseMode.OneChildVisible ||
				Model.Orientation == Orientation.Horizontal;
			bool isFirstItem = true;
			foreach (FrameworkElement item in items) {
				VerticalAlignment alignment = GetItemVerticalAlignment(item);
				if (isOneStretchedItemMode && alignment == VerticalAlignment.Stretch ||
					!isOneStretchedItemMode && !isFirstItem && alignment != result) {
					result = VerticalAlignment.Stretch;
					break;
				}
				if (isFirstItem) {
					isFirstItem = false;
					if (!isOneStretchedItemMode)
						result = alignment;
				}
			}
			if (result == VerticalAlignment.Stretch &&
				Model.CollapseMode == LayoutGroupCollapseMode.NoChildrenVisible && Model.CollapseDirection == Orientation.Vertical)
				result = VerticalAlignment.Top;
			return result;
		}
		public virtual HorizontalAlignment GetItemHorizontalAlignment(FrameworkElement item) {
			var result = item.IsLayoutGroup() ? ((ILayoutGroup)item).ActualHorizontalAlignment : item.HorizontalAlignment;
			if(result == HorizontalAlignment.Stretch && !double.IsNaN(item.Width))
				result = Model.Orientation == Orientation.Horizontal ? HorizontalAlignment.Left : HorizontalAlignment.Center;
			return result;
		}
		public virtual VerticalAlignment GetItemVerticalAlignment(FrameworkElement item) {
			var result = item.IsLayoutGroup() ? ((ILayoutGroup)item).ActualVerticalAlignment : item.VerticalAlignment;
			if(result == VerticalAlignment.Stretch && !double.IsNaN(item.Height))
				result = Model.Orientation == Orientation.Horizontal ? VerticalAlignment.Center : VerticalAlignment.Top;
			return result;
		}
		public void InitChildFromGroup(FrameworkElement child, FrameworkElement group) {
			var childAlignment = GetItemAlignment(child);
			var groupAlignment = GetItemAlignment(group);
			if(groupAlignment == ItemAlignment.Center || groupAlignment == ItemAlignment.End)
				SetItemAlignment(child, groupAlignment);
			else
				if(childAlignment == ItemAlignment.Center || childAlignment == ItemAlignment.End)
					SetItemAlignment(child, ItemAlignment.Start);
		}
		public virtual void InitGroupForChild(LayoutGroup group, FrameworkElement child) {
			Func<ItemAlignment, bool> IsAlignmentTransferNeeded =
				alignment => alignment == ItemAlignment.Center || alignment == ItemAlignment.End;
			if ((Model.CollapseMode != LayoutGroupCollapseMode.None || Model.Orientation == Orientation.Horizontal) && 
				IsAlignmentTransferNeeded(GetItemHorizontalAlignment(child).GetItemAlignment()))
				group.HorizontalAlignment = child.HorizontalAlignment;
			if ((Model.CollapseMode != LayoutGroupCollapseMode.None || Model.Orientation == Orientation.Vertical) &&
				IsAlignmentTransferNeeded(GetItemVerticalAlignment(child).GetItemAlignment()))
				group.VerticalAlignment = child.VerticalAlignment;
		}
		public virtual bool IsContentEmpty(FrameworkElements items) {
			bool result = items.Count == 0;
			if (!result && Model.CollapseMode == LayoutGroupCollapseMode.OneChildVisible)
				foreach (FrameworkElement item in items) {
					result = item.IsLayoutGroup() && ((ILayoutGroup)item).IsUIEmpty;
					if (!result)
						break;
				}
			return result;
		}
		public virtual bool IsLayoutUniform(FrameworkElements items) {
			if (Model.CollapseMode != LayoutGroupCollapseMode.None)
				return true;
			ItemAlignment? alignment = null;
			foreach (FrameworkElement item in items) {
				ItemAlignment itemAlignment = GetItemAlignment(item);
				if (itemAlignment == ItemAlignment.Stretch)
					itemAlignment = ItemAlignment.Start;
				if (alignment == null)
					alignment = itemAlignment;
				else
					if (itemAlignment != alignment)
						return false;
			}
			return true;
		}
		public virtual void SetItemHorizontalAlignment(FrameworkElement item, HorizontalAlignment value, bool updateWidth) {
#if !SILVERLIGHT
			bool isAlignmentAssigned = item.IsPropertyAssigned(FrameworkElement.HorizontalAlignmentProperty);
			HorizontalAlignment alignment = item.HorizontalAlignment;
#endif
			if (item.IsLayoutGroup() && value == ((ILayoutGroup)item).DesiredHorizontalAlignment)
				item.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
			else
				item.HorizontalAlignment = value;
#if !SILVERLIGHT
			if (item.IsLayoutGroup() && item.HorizontalAlignment == alignment &&
				item.IsPropertyAssigned(FrameworkElement.HorizontalAlignmentProperty) != isAlignmentAssigned)
				((ILayoutGroup)item.Parent).ChildHorizontalAlignmentChanged(item);
#endif
			if (updateWidth && value == HorizontalAlignment.Stretch && !double.IsNaN(item.Width))
				item.Width = double.NaN;
		}
		public virtual void SetItemVerticalAlignment(FrameworkElement item, VerticalAlignment value, bool updateHeight) {
#if !SILVERLIGHT
			bool isAlignmentAssigned = item.IsPropertyAssigned(FrameworkElement.VerticalAlignmentProperty);
			VerticalAlignment alignment = item.VerticalAlignment;
#endif
			if (item.IsLayoutGroup() && value == ((ILayoutGroup)item).DesiredVerticalAlignment)
				item.ClearValue(FrameworkElement.VerticalAlignmentProperty);
			else
				item.VerticalAlignment = value;
#if !SILVERLIGHT
			if (item.IsLayoutGroup() && item.VerticalAlignment == alignment &&
				item.IsPropertyAssigned(FrameworkElement.VerticalAlignmentProperty) != isAlignmentAssigned)
				((ILayoutGroup)item.Parent).ChildVerticalAlignmentChanged(item);
#endif
			if (updateHeight && value == VerticalAlignment.Stretch && !double.IsNaN(item.Height))
				item.Height = double.NaN;
		}
		protected virtual Size ArrangeWhenCollapsed(FrameworkElements items, Rect bounds) {
#if SILVERLIGHT
			bool isCompletelyCollapsed = Model.CollapseMode == LayoutGroupCollapseMode.NoChildrenVisible;
#endif
			FrameworkElement visibleItem = Model.UncollapsedChild;
			foreach (FrameworkElement item in items) {
				Rect itemBounds;
				if (item.IsLayoutGroup() && ((ILayoutGroup)item).IsBorderless)
					itemBounds = bounds;
				else {
					itemBounds = RectHelper.New(GetItemDesiredSize(item));
					RectHelper.AlignHorizontally(ref itemBounds, bounds, GetItemHorizontalAlignment(item));
					RectHelper.AlignVertically(ref itemBounds, bounds, GetItemVerticalAlignment(item));
				}
				if (item != visibleItem) {
#if SILVERLIGHT
					if (!isCompletelyCollapsed && !DesignerProperties.IsInDesignTool)
						RectHelper.SetLocation(ref itemBounds, UIElementExtensions.InvisibleBounds.Location());
					else
						itemBounds = UIElementExtensions.InvisibleBounds;
#else
					if (Model.MeasureUncollapsedChildOnly && item.RenderSize.IsZero() &&
						(!item.IsLayoutGroup() || !((ILayoutGroup)item).HasNewChildren))	
						continue;
					itemBounds = UIElementExtensions.InvisibleBounds;
#endif
				}
				item.Arrange(itemBounds);
			}
			return bounds.Size();
		}
		protected virtual Size ArrangeWhenUncollapsed(FrameworkElements items, Rect bounds) {
			ItemInfos itemInfos = CreateItemInfos(GetItemLength(bounds.Size()), Parameters.ItemSpace);
			foreach (FrameworkElement item in items)
				itemInfos.Add(new ItemInfo(GetItemLength(GetItemDesiredSize(item)), GetItemLength(GetItemMinSize(item, true)),
										   GetItemLength(GetItemMaxSize(item, true)), GetItemAlignment(item)));
			itemInfos.Calculate();
			double startOffset = GetItemOffset(bounds);
			for (int i = 0; i < items.Count; i++) {
				var itemLocation = new Point();
				Size itemSize = GetItemDesiredSize(items[i]);
				SetItemOffset(ref itemLocation, startOffset + itemInfos[i].Offset);
				SetItemLength(ref itemSize, itemInfos[i].Length);
				var itemBounds = new Rect(itemLocation, itemSize);
				if (Model.Orientation == Orientation.Horizontal)
					RectHelper.AlignVertically(ref itemBounds, bounds, GetItemVerticalAlignment(items[i]));
				else
					RectHelper.AlignHorizontally(ref itemBounds, bounds, GetItemHorizontalAlignment(items[i]));
				items[i].Arrange(itemBounds);
			}
			if (Parameters.ItemSizers != null) {
				ArrangeItems(items);
				AddItemSizers(items);
			}
			return bounds.Size();
		}
		protected virtual ItemInfos CreateItemInfos(double availableLength, double itemSpace) {
			return new ItemInfos(availableLength, itemSpace);
		}
		protected virtual void MeasureItem(FrameworkElement item, Size maxSize) {
			Size minSize = GetItemMinSize(item, false);
			SetItemLength(ref maxSize, Math.Max(GetItemLength(minSize), GetItemLength(maxSize)));
			if (GetItemOrthogonalAlignment(item) == ItemAlignment.Stretch)
				SetItemWidth(ref maxSize, Math.Max(GetItemWidth(minSize), GetItemWidth(maxSize)));
			else
				SetItemWidth(ref maxSize, double.PositiveInfinity);
			item.Measure(maxSize);
		}
		protected void MeasureItem(FrameworkElement item, Size maxSize, ref double totalLength, ref double totalWidth) {
			MeasureItem(item, maxSize);
			UpdateTotalSize(ref totalLength, ref totalWidth, GetItemDesiredSize(item));
		}
		protected virtual void MeasureStretchedItems(FrameworkElements stretchedItems, double availableLength, double availableWidth,
			ref double totalLength, ref double totalWidth) {
			if (double.IsInfinity(availableLength)) {
				Size itemSize = GetItemSize(availableLength, availableWidth);
				foreach (FrameworkElement item in stretchedItems)
					MeasureItem(item, itemSize, ref totalLength, ref totalWidth);
			}
			else {
				var calculator = new StretchedLengthsCalculator(availableLength);
				foreach (FrameworkElement item in stretchedItems)
					calculator.Add(new ItemInfo(0, GetItemLength(GetItemMinSize(item, true)),
												GetItemLength(GetItemMaxSize(item, true)), ItemAlignment.Stretch));
				calculator.Calculate();
				bool useFullLength = false;
				for (int i = 0; i < stretchedItems.Count; i++) {
					MeasureItem(stretchedItems[i], GetItemSize(calculator[i].Length, availableWidth));
					useFullLength = useFullLength || GetItemLength(GetItemDesiredSize(stretchedItems[i])) == calculator[i].Length;
				}
				if (useFullLength && !calculator.NeedsMoreLength) {
					UpdateTotalSize(ref totalLength, ref totalWidth, GetItemSize(availableLength, 0));
				}
				else {
					double maxItemLength = 0;
					foreach (FrameworkElement item in stretchedItems)
						maxItemLength = Math.Max(maxItemLength, GetItemLength(GetItemDesiredSize(item)));
					double totalItemLength = 0;
					for (int i = 0; i < stretchedItems.Count; i++) {
						if (calculator[i].Length < maxItemLength && calculator[i].Length != calculator[i].MaxLength)
							MeasureItem(stretchedItems[i], GetItemSize(maxItemLength, availableWidth));
						totalItemLength += Math.Min(maxItemLength, calculator[i].MaxLength);
					}
					UpdateTotalSize(ref totalLength, ref totalWidth, GetItemSize(totalItemLength, 0));
				}
				foreach (FrameworkElement item in stretchedItems)
					UpdateTotalSize(ref totalLength, ref totalWidth, GetItemSize(0, GetItemWidth(GetItemDesiredSize(item))));
			}
		}
		protected virtual Size MeasureWhenCollapsed(FrameworkElements items, Size maxSize) {
			Size result = SizeHelper.Zero;
			bool isCompletelyCollapsed = Model.CollapseMode == LayoutGroupCollapseMode.NoChildrenVisible;
			if (isCompletelyCollapsed)
				maxSize = UIElementExtensions.InvisibleBounds.Size();
#if !SILVERLIGHT
			FrameworkElement visibleItem = Model.UncollapsedChild;
#endif
			foreach (FrameworkElement item in items) {
#if !SILVERLIGHT
				if (!isCompletelyCollapsed && Model.MeasureUncollapsedChildOnly && item != visibleItem)
					continue;
#endif
				MeasureItem(item, maxSize);
				if (!isCompletelyCollapsed)
					SizeHelper.UpdateMaxSize(ref result, GetItemDesiredSize(item));
			}
			return result;
		}
		protected virtual Size MeasureWhenUncollapsed(FrameworkElements items, Size maxSize) {
			double itemsSpace = (items.Count - 1) * Parameters.ItemSpace;
			SetItemLength(ref maxSize, Math.Max(0, GetItemLength(maxSize) - itemsSpace));
			double length;
			double width;
			while (true) {
				Size itemMaxSize = maxSize;
				SetItemLength(ref itemMaxSize, double.PositiveInfinity);
				length = 0;
				width = 0;
				var stretchedItems = new FrameworkElements();
				foreach (FrameworkElement item in items)
					if (GetItemAlignment(item) == ItemAlignment.Stretch)
						stretchedItems.Add(item);
					else
						MeasureItem(item, itemMaxSize, ref length, ref width);
				if (stretchedItems.Count != 0)
					MeasureStretchedItems(stretchedItems, Math.Max(0, GetItemLength(maxSize) - length), GetItemWidth(maxSize), ref length, ref width);
				if (width <= GetItemWidth(maxSize))
					break;
				else
					SetItemWidth(ref maxSize, width);
			}
			return GetItemSize(length != 0 ? length + itemsSpace : 0, width);
		}
		protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
			if (Model.CollapseMode == LayoutGroupCollapseMode.None)
				return MeasureWhenUncollapsed(items, maxSize);
			else
				return MeasureWhenCollapsed(items, maxSize);
		}
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			if (Model.CollapseMode == LayoutGroupCollapseMode.None)
				return ArrangeWhenUncollapsed(items, bounds);
			else
				return ArrangeWhenCollapsed(items, bounds);
		}
		protected virtual void UpdateTotalSize(ref double totalLength, ref double totalWidth, Size itemSize) {
			totalLength += GetItemLength(itemSize);
			totalWidth = Math.Max(totalWidth, GetItemWidth(itemSize));
		}
		protected ItemAlignment GetItemAlignment(FrameworkElement item) {
			if(Model.Orientation == Orientation.Horizontal)
				return GetItemHorizontalAlignment(item).GetItemAlignment();
			else
				return GetItemVerticalAlignment(item).GetItemAlignment();
		}
		protected ItemAlignment GetItemOrthogonalAlignment(FrameworkElement item) {
			if(Model.Orientation == Orientation.Horizontal)
				return GetItemVerticalAlignment(item).GetItemAlignment();
			else
				return GetItemHorizontalAlignment(item).GetItemAlignment();
		}
		protected void SetItemAlignment(FrameworkElement item, ItemAlignment alignment) {
			if(Model.Orientation == Orientation.Horizontal)
				SetItemHorizontalAlignment(item, alignment.GetHorizontalAlignment(), false);
			else
				SetItemVerticalAlignment(item, alignment.GetVerticalAlignment(), false);
		}
		protected double GetItemOffset(Rect itemBounds) {
			return Model.Orientation == Orientation.Horizontal ? itemBounds.X : itemBounds.Y;
		}
		protected double GetItemOffsetPlusLength(Rect itemBounds) {
			return GetItemOffset(itemBounds) + GetItemLength(itemBounds);
		}
		protected double GetItemLength(Size itemSize) {
			return Model.Orientation == Orientation.Horizontal ? itemSize.Width : itemSize.Height;
		}
		protected double GetItemLength(Rect itemBounds) {
			return GetItemLength(itemBounds.Size());
		}
		protected double GetItemWidth(Size itemSize) {
			return Model.Orientation == Orientation.Horizontal ? itemSize.Height : itemSize.Width;
		}
		protected Size GetItemSize(double itemLength, double itemWidth) {
			var result = new Size();
			SetItemLength(ref result, itemLength);
			SetItemWidth(ref result, itemWidth);
			return result;
		}
		protected void SetItemOffset(ref Point itemLocation, double offset) {
			if(Model.Orientation == Orientation.Horizontal)
				itemLocation.X = offset;
			else
				itemLocation.Y = offset;
		}
		protected void SetItemOffset(ref Rect itemBounds, double offset) {
			Point itemLocation = itemBounds.Location();
			SetItemOffset(ref itemLocation, offset);
			RectHelper.SetLocation(ref itemBounds, itemLocation);
		}
		protected void SetItemOffsetPlusLength(ref Rect itemBounds, double offsetPlusLength) {
			SetItemLength(ref itemBounds, offsetPlusLength - GetItemOffset(itemBounds));
		}
		protected void SetItemLength(ref Size itemSize, double length) {
			length = Math.Max(0, length);
			if(Model.Orientation == Orientation.Horizontal)
				itemSize.Width = length;
			else
				itemSize.Height = length;
		}
		protected void SetItemLength(ref Rect itemBounds, double length) {
			Size itemSize = itemBounds.Size();
			SetItemLength(ref itemSize, length);
			RectHelper.SetSize(ref itemBounds, itemSize);
		}
		protected void SetItemWidth(ref Size itemSize, double width) {
			if(Model.Orientation == Orientation.Horizontal)
				itemSize.Height = width;
			else
				itemSize.Width = width;
		}
		protected virtual Size GetActualMinOrMaxSizeWhenCollapsed(FrameworkElements items, bool getMinSize, LayoutParametersBase parameters) {
			if (Model.CollapseMode == LayoutGroupCollapseMode.NoChildrenVisible)
				return getMinSize ? SizeHelper.Zero : SizeHelper.Infinite;
			Size result = SizeHelper.Zero;
			foreach (FrameworkElement item in items) {
				Size itemSize = getMinSize ? GetItemMinSize(item, true) : GetItemMaxSize(item, true);
				SizeHelper.UpdateMaxSize(ref result, itemSize);
			}
			return result;
		}
		protected virtual Size GetActualMinOrMaxSizeWhenUncollapsed(FrameworkElements items, bool getMinSize, LayoutParametersBase parameters) {
			double length = getMinSize ? (items.Count - 1) * parameters.ItemSpace : double.PositiveInfinity;
			double width = 0;
			foreach (var item in items) {
				var itemSize = getMinSize ? GetItemMinSize(item, true) : GetItemMaxSize(item, true);
				var itemDesiredSize = GetItemDesiredSize(item);
				if (getMinSize && GetItemAlignment(item) != ItemAlignment.Stretch && GetItemLength(itemDesiredSize) != 0)
					SetItemLength(ref itemSize, GetItemLength(itemDesiredSize));
				UpdateTotalSize(ref length, ref width, itemSize);
			}
			return GetItemSize(length, width);
		}
		protected Size GetItemMinSize(FrameworkElement item, bool getActualMinSize) {
			return GetItemMinOrMaxSize(item, getActualMinSize && item.IsLayoutGroup() ? ((ILayoutGroup)item).ActualMinSize : item.GetMinSize());
		}
		protected Size GetItemMaxSize(FrameworkElement item, bool getActualMaxSize) {
			return GetItemMinOrMaxSize(item, getActualMaxSize && item.IsLayoutGroup() ? ((ILayoutGroup)item).ActualMaxSize : item.GetMaxSize());
		}
		private Size GetItemMinOrMaxSize(FrameworkElement item, Size originalMinOrMaxSize) {
			var result = originalMinOrMaxSize;
			if(!double.IsNaN(item.Width))
				result.Width = item.GetRealWidth();
			if(!double.IsNaN(item.Height))
				result.Height = item.GetRealHeight();
			SizeHelper.Inflate(ref result, item.Margin);
			return result;
		}
		protected Size GetItemDesiredSize(FrameworkElement item) {
			return item.IsLayoutGroup() ? ((ILayoutGroup)item).ActualDesiredSize : item.GetDesiredSize();
		}
		protected virtual void AddItemSizers(FrameworkElements items) {
			if(items.Count <= 1)
				return;
			var itemSizerElements = new FrameworkElement[items.Count + 1];
			DefineItemSizerElements(items, ItemAlignment.Start, itemSizerElements);
			DefineItemSizerElements(items, ItemAlignment.End, itemSizerElements);
			DefineItemSizerElements(items, ItemAlignment.Stretch, itemSizerElements);
			for(int i = 0; i < items.Count; i++) {
				var item = items[i];
				var itemAlignment = GetItemAlignment(item);
				bool isStartSizer;
				if(itemAlignment == ItemAlignment.Center)
					if(IsItemSizable(item))
						isStartSizer = false;
					else
						continue;
				else
					if(itemSizerElements[1 + i] == item)
						isStartSizer = false;
					else
						if(itemSizerElements[i] == item)
							isStartSizer = true;
						else
							continue;
				Parameters.ItemSizers.Add(item, GetItemSizerSide(isStartSizer));
			}
		}
		protected virtual void DefineItemSizerElements(FrameworkElements items, ItemAlignment alignment, FrameworkElement[] itemSizerElements) {
			for(int i = 0; i < items.Count; i++) {
				var item = items[i];
				var itemAlignment = GetItemAlignment(item);
				if(itemAlignment == alignment && IsItemSizable(item)) {
					var itemSizerIndex = 1 + i;
					if(itemAlignment == ItemAlignment.End)
						itemSizerIndex--;
					else
						if(i > 0 && i == items.Count - 1)
							if(itemSizerElements[itemSizerIndex - 1] == null)
								itemSizerIndex--;
							else
								if(itemAlignment == ItemAlignment.Stretch)
									break;
					if(itemSizerElements[itemSizerIndex] == null)
						itemSizerElements[itemSizerIndex] = item;
				}
			}
		}
		protected Side GetItemSizerSide(bool isStartSizer) {
			if(Model.Orientation == Orientation.Horizontal)
				return isStartSizer ? Side.Left : Side.Right;
			else
				return isStartSizer ? Side.Top : Side.Bottom;
		}
		protected virtual bool IsItemSizable(FrameworkElement item) {
			if(Model.Orientation == Orientation.Horizontal)
				return item.MinWidth < item.MaxWidth && LayoutControl.GetAllowHorizontalSizing(item);
			else
				return item.MinHeight < item.MaxHeight && LayoutControl.GetAllowVerticalSizing(item);
		}
		protected new ILayoutGroupModel Model { get { return (ILayoutGroupModel)base.Model; } }
		protected new LayoutGroupParameters Parameters { get { return (LayoutGroupParameters)base.Parameters; } }
		#region LayoutItem Labels Alignment
		public void AlignLayoutItemLabels(FrameworkElement scope, FrameworkElements items) {
			var layoutItems = new LayoutItems();
			GetLayoutItems(items, layoutItems);
			if (layoutItems.Count == 0)
				return;
			List<LayoutItems> columns = CalculateColumns(scope, layoutItems);
			foreach (LayoutItems columnItems in columns)
				SetLabelWidth(columnItems, CalculateCommonLabelWidth(columnItems));
		}
		public virtual void GetLayoutItems(FrameworkElements items, LayoutItems layoutItems) {
			foreach(FrameworkElement item in items)
				if(item is LayoutItem) {
					var layoutItem = (LayoutItem)item;
					if(CanAlignLayoutItemLabel(layoutItem))
						layoutItems.Add(layoutItem);
				} else
					if(item.IsLayoutGroup())
						((ILayoutGroup)item).GetLayoutItems(layoutItems);
		}
		protected virtual List<LayoutItems> CalculateColumns(FrameworkElement scope, LayoutItems layoutItems) {
			var result = new List<LayoutItems>();
			var columnOffsets = new List<double>();
			foreach (LayoutItem item in layoutItems) {
				double itemOffset = item.GetPosition(scope).X;
				int column = columnOffsets.IndexOf(itemOffset);
				if (column == -1) {
					column = columnOffsets.Count;
					columnOffsets.Add(itemOffset);
					result.Add(new LayoutItems());
				}
				result[column].Add(item);
			}
			return result;
		}
		protected virtual double CalculateCommonLabelWidth(LayoutItems layoutItems) {
			double result = 0.0;
			foreach (LayoutItem layoutItem in layoutItems)
				result = Math.Max(result, layoutItem.LabelWidth);
			return result;
		}
		protected virtual bool CanAlignLayoutItemLabel(LayoutItem layoutItem) {
			HorizontalAlignment layoutItemAlignment = GetItemHorizontalAlignment(layoutItem);
			return (layoutItemAlignment == HorizontalAlignment.Left || layoutItemAlignment == HorizontalAlignment.Stretch) &&
				layoutItem.IsLabelVisible && layoutItem.LabelPosition == LayoutItemLabelPosition.Left;
		}
		protected virtual void SetLabelWidth(LayoutItems layoutItems, double labelWidth) {
			foreach (LayoutItem layoutItem in layoutItems)
				layoutItem.LabelWidth = labelWidth;
		}
		#endregion LayoutItem Labels Alignment
		#region Layout Modification
		public virtual LayoutItemInsertionInfo GetInsertionInfoForEmptyArea(ILayoutGroup control, FrameworkElement element, Point p) {
			var result = new LayoutItemInsertionInfo();
			FrameworkElement insertionPointElement;
			CalculateInsertionInfoForEmptyArea(control, control.ClientBounds, p, control.GetLogicalChildren(true),
				out result.InsertionKind, out insertionPointElement);
			result.DestinationItem = insertionPointElement;
			var insertionPoints = new LayoutItemInsertionPoints();
			GetInsertionPoints(control, element, result.DestinationItem, result.DestinationItem, result.InsertionKind, insertionPoints);
			LayoutItemInsertionPoint insertionPoint = insertionPoints.Find(insertionPointElement);
			if (insertionPoint == null)
				insertionPoint = insertionPoints.Count > 0 ? insertionPoints[insertionPoints.Count - 1] : null;
			result.InsertionPoint = insertionPoint;
			return result;
		}
		public virtual LayoutItemInsertionKind GetInsertionKind(ILayoutGroup control, FrameworkElement destinationItem, Point p) {
			if (destinationItem == control && control.IsBorderless)
				return LayoutItemInsertionKind.None;
			for (var result = LayoutItemInsertionKind.Left; result <= LayoutItemInsertionKind.Bottom; result++)
				if (GetInsertionZoneBounds(control, destinationItem, result).Contains(p))
					return result;
			return LayoutItemInsertionKind.None;
		}
		public virtual void GetInsertionPoints(ILayoutGroup control, FrameworkElement element, FrameworkElement destinationItem,
			FrameworkElement originalDestinationItem, LayoutItemInsertionKind insertionKind, LayoutItemInsertionPoints points) {
			if (IsInsertionPoint(control, element, destinationItem, originalDestinationItem, insertionKind))
				points.Add(new LayoutItemInsertionPoint(destinationItem, insertionKind == LayoutItemInsertionKind.Inside));
			if (IsExternalInsertionPoint(control, element, destinationItem, insertionKind))
				GetExternalInsertionPoints(control, element, originalDestinationItem, insertionKind, points);
		}
		public virtual Rect GetInsertionPointZoneBounds(ILayoutGroup control, FrameworkElement destinationItem, LayoutItemInsertionKind insertionKind,
			int pointIndex, int pointCount) {
			Rect insertionZoneBounds = GetInsertionZoneBounds(control, destinationItem, insertionKind);
			if (insertionKind == LayoutItemInsertionKind.Inside)
				return insertionZoneBounds;
			if (insertionKind.GetSide() == null)
				return Rect.Empty;
			Side insertionSide = insertionKind.GetSide().Value;
			double zoneWidth = insertionSide.GetOrientation() == Orientation.Horizontal ? insertionZoneBounds.Height : insertionZoneBounds.Width;
			double pointZoneWidth = Math.Floor(zoneWidth / pointCount);
			Rect result = insertionZoneBounds;
			RectHelper.Inflate(ref result, insertionSide, -(pointCount - 1 - pointIndex) * pointZoneWidth);
			if (pointIndex == 0)
				pointZoneWidth += zoneWidth - pointCount * pointZoneWidth;
			switch (insertionSide) {
				case Side.Left:
					result.Width = pointZoneWidth;
					break;
				case Side.Right:
					RectHelper.SetLeft(ref result, result.Right - pointZoneWidth);
					break;
				case Side.Top:
					result.Height = pointZoneWidth;
					break;
				case Side.Bottom:
					RectHelper.SetTop(ref result, result.Bottom - pointZoneWidth);
					break;
			}
			return result;
		}
		public virtual void InsertElement(ILayoutGroup control, FrameworkElement element, LayoutItemInsertionPoint insertionPoint,
			LayoutItemInsertionKind insertionKind) {
			Side? insertionSide = insertionKind.GetSide();
			if (insertionPoint.Element == control && insertionPoint.IsInternalInsertion)
				if (insertionKind == LayoutItemInsertionKind.Inside) {
					Orientation orientation = element.Parent is ILayoutGroup ? ((ILayoutGroup)element.Parent).Orientation : Model.Orientation;
					control.Root.AvailableItems.Remove(element);
					element.SetParent(control.Control);
					Model.Orientation = orientation;
					return;
				}
				else
					if (insertionSide != null) {
						ILayoutGroup newGroup = control.MoveChildrenToNewGroup();
						LayoutControl.SetIsUserDefined(newGroup.Control, true);
						insertionPoint.Element = newGroup.Control;
						insertionPoint.IsInternalInsertion = false;
						Model.Orientation = Model.Orientation.OrthogonalValue();
					}
			if (insertionSide == null)
				return;
			if (Model.CollapseMode != LayoutGroupCollapseMode.None || Model.Orientation == insertionSide.Value.GetOrientation()) {
				ILayoutGroup newGroup = control.MoveChildToNewGroup(insertionPoint.Element);
				LayoutControl.SetIsUserDefined(newGroup.Control, true);
				newGroup.Orientation = insertionSide.Value.GetOrientation().OrthogonalValue();
				newGroup.InsertElement(element, insertionPoint, insertionKind);
			}
			else {
				control.Root.AvailableItems.Remove(element);
				element.SetParent(null);
				int newIndex = control.Children.IndexOf(insertionPoint.Element);
				if (insertionSide.Value.IsEnd())
					newIndex++;
				control.Children.Insert(newIndex, element);
				InitInsertedElement(element, insertionPoint.Element);
			}
		}
		public virtual bool IsExternalInsertionPoint(ILayoutGroup control, FrameworkElement element, FrameworkElement destinationItem,
			LayoutItemInsertionKind insertionKind) {
			if (insertionKind.GetSide() == null || control.CollapseMode != LayoutGroupCollapseMode.None)
				return false;
			{
				Side side = insertionKind.GetSide().Value;
				Rect parentBounds = control.ChildAreaBounds;
				Rect itemBounds = destinationItem.GetBounds(control.Control);
				if (parentBounds.GetSideOffset(side) == itemBounds.GetSideOffset(side))
					if (side.GetOrientation() == Model.Orientation) {
						FrameworkElements children = control.GetLogicalChildren(true);
						return children.Contains(element) ? children.Count >= 3 : children.Count >= 2;
					}
					else
						return control.IsBorderless && ((ILayoutGroup)control.Control.Parent).IsChildBorderless(control);
				else
					return false;
			}
		}
		protected virtual void CalculateInsertionInfoForEmptyArea(ILayoutGroup control, Rect bounds, Point p, FrameworkElements items,
			out LayoutItemInsertionKind insertionKind, out FrameworkElement insertionPoint) {
			if (items.Count == 0 && bounds.Contains(p)) {
				insertionKind = LayoutItemInsertionKind.Inside;
				insertionPoint = control.Control;
				return;
			}
			if (control.CollapseMode == LayoutGroupCollapseMode.None) {
				ArrangeItems(items);
				Rect itemAreaBounds = bounds;
				for (int i = 0; i < items.Count; i++) {
					Rect itemBounds = items[i].GetBounds();
					if (i < items.Count - 1)
						SetItemOffsetPlusLength(ref itemAreaBounds,
							(GetItemOffsetPlusLength(itemBounds) + GetItemOffset(items[i + 1].GetBounds())) / 2);
					else
						SetItemOffsetPlusLength(ref itemAreaBounds, GetItemOffsetPlusLength(bounds));
					if (itemAreaBounds.Contains(p)) {
						insertionPoint = items[i];
						insertionKind = GetInsertionKindForEmptyArea(itemBounds, p);
						return;
					}
					SetItemOffset(ref itemAreaBounds, GetItemOffsetPlusLength(itemAreaBounds));
				}
			}
			else
				if (control.UncollapsedChild != null) {
					insertionPoint = control.UncollapsedChild;
					insertionKind = GetInsertionKindForEmptyArea(insertionPoint.GetBounds(), p);
					return;
				}
			insertionKind = LayoutItemInsertionKind.None;
			insertionPoint = null;
		}
		protected virtual void GetExternalInsertionPoints(ILayoutGroup control, FrameworkElement element,
			FrameworkElement originalDestinationItem, LayoutItemInsertionKind insertionKind, LayoutItemInsertionPoints points) {
			var controlParent = control.Control.Parent as ILayoutGroup;
			if (control.IsBorderless && controlParent.IsChildBorderless(control))
				controlParent.GetInsertionPoints(element, control.Control, originalDestinationItem, insertionKind, points);
			else
				points.Add(new LayoutItemInsertionPoint(control.Control, true));
		}
		protected virtual LayoutItemInsertionKind GetInsertionKindForEmptyArea(Rect itemBounds, Point p) {
			var result = LayoutItemInsertionKind.None;
			Action CheckHorizontalInsertionKinds =
				delegate() {
					if (p.X < itemBounds.Left)
						result = LayoutItemInsertionKind.Left;
					if (p.X >= itemBounds.Right)
						result = LayoutItemInsertionKind.Right;
				};
			Action CheckVerticalInsertionKinds =
				delegate() {
					if (p.Y < itemBounds.Top)
						result = LayoutItemInsertionKind.Top;
					if (p.Y >= itemBounds.Bottom)
						result = LayoutItemInsertionKind.Bottom;
				};
			if (Model.CollapseMode == LayoutGroupCollapseMode.None && Model.Orientation == Orientation.Horizontal) {
				CheckVerticalInsertionKinds();
				CheckHorizontalInsertionKinds();
			}
			else {
				CheckHorizontalInsertionKinds();
				CheckVerticalInsertionKinds();
			}
			return result;
		}
		protected virtual Rect GetInsertionZoneBounds(ILayoutGroup control, FrameworkElement destinationItem, LayoutItemInsertionKind insertionKind) {
			var contentBounds = Rect.Empty;
			if (destinationItem == control && !control.IsBorderless && control.CollapseMode != LayoutGroupCollapseMode.NoChildrenVisible)
				contentBounds = control.ContentBounds;
			return GetInsertionZoneBounds(RectHelper.New(destinationItem.GetSize()), contentBounds, insertionKind);
		}
		protected virtual Rect GetInsertionZoneBounds(Rect bounds, Rect contentBounds, LayoutItemInsertionKind insertionKind) {
			if (insertionKind == LayoutItemInsertionKind.Inside)
				return contentBounds.IsEmpty ? bounds : contentBounds;
			if (insertionKind.GetSide() == null)
				return Rect.Empty;
			Side insertionSide = insertionKind.GetSide().Value;
			const double RelativeZoneWidth = 0.25;
			var result = bounds;
			if (result.Width > result.Height) {
				if (contentBounds.IsEmpty) {
					double zoneWidth = Math.Ceiling(RelativeZoneWidth * bounds.Width);
#if !SILVERLIGHT
					contentBounds = new Rect();
#endif
					contentBounds.X = bounds.Left + zoneWidth;
					contentBounds.Width = bounds.Width - 2 * zoneWidth;
					contentBounds.Y = Math.Floor((bounds.Top + bounds.Bottom) / 2);
					contentBounds.Height = 0;
				}
				if (insertionSide == Side.Left)
					RectHelper.SetRight(ref result, contentBounds.Left);
				else
					if (insertionSide == Side.Right)
						RectHelper.SetLeft(ref result, contentBounds.Right);
					else {
						result.X = contentBounds.X;
						result.Width = contentBounds.Width;
						if (insertionSide == Side.Top)
							RectHelper.SetBottom(ref result, contentBounds.Top);
						else
							RectHelper.SetTop(ref result, contentBounds.Bottom);
					}
			}
			else {
				if (contentBounds.IsEmpty) {
#if !SILVERLIGHT
					contentBounds = new Rect();
#endif
					contentBounds.X = Math.Floor((bounds.Left + bounds.Right) / 2);
					contentBounds.Width = 0;
					double zoneWidth = Math.Ceiling(RelativeZoneWidth * bounds.Height);
					contentBounds.Y = bounds.Top + zoneWidth;
					contentBounds.Height = bounds.Height - 2 * zoneWidth;
				}
				if (insertionSide == Side.Top)
					RectHelper.SetBottom(ref result, contentBounds.Top);
				else
					if (insertionSide == Side.Bottom)
						RectHelper.SetTop(ref result, contentBounds.Bottom);
					else {
						result.Y = contentBounds.Y;
						result.Height = contentBounds.Height;
						if (insertionSide == Side.Left)
							RectHelper.SetRight(ref result, contentBounds.Left);
						else
							RectHelper.SetLeft(ref result, contentBounds.Right);
					}
			}
			return result;
		}
		protected virtual bool IsInsertionPoint(ILayoutGroup control, FrameworkElement element, FrameworkElement destinationItem,
			FrameworkElement originalDestinationItem, LayoutItemInsertionKind insertionKind) {
			if (destinationItem == element)
				return false;
			if (insertionKind == LayoutItemInsertionKind.Inside && (destinationItem == control || destinationItem.IsLayoutGroup()))
				return true;
			if (control.CollapseMode != LayoutGroupCollapseMode.None)
				return true;
			if (insertionKind.GetSide() == null)
				return false;
			Side insertionSide = insertionKind.GetSide().Value;
			if (insertionSide.GetOrientation() != Model.Orientation) {
				FrameworkElements destinationItems = control.GetArrangedLogicalChildren(true);
				int index = destinationItems.IndexOf(element);
				if (index != -1) {
					int destinationIndex = destinationItems.IndexOf(destinationItem);
					if (insertionSide.IsStart() && destinationIndex == index + 1 || insertionSide.IsEnd() && destinationIndex == index - 1)
						return false;
				}
			}
			if (destinationItem.IsLayoutGroup()) {
				var destinationGroup = (ILayoutGroup)destinationItem;
				if (insertionSide.GetOrientation() == destinationGroup.Orientation)
					if (destinationItem != originalDestinationItem) {
						FrameworkElements destinationItemChildren = destinationGroup.GetLogicalChildren(true);
						return !destinationItemChildren.Contains(element) || destinationItemChildren.Count > 2 || originalDestinationItem == element;
					}
					else
						return true;
				else
					if (destinationItem != originalDestinationItem)
						return false;
					else
						if (destinationGroup.GetLogicalChildren(true).Contains(element))
							return !destinationGroup.IsExternalInsertionPoint(element, element, insertionKind);
						else
							return true;
			}
			else
				return true;
		}
		protected virtual void InitInsertedElement(FrameworkElement element, FrameworkElement insertionPoint) {
			var insertionPointAlignment = GetItemAlignment(insertionPoint);
			if(insertionPointAlignment == ItemAlignment.Start || insertionPointAlignment == ItemAlignment.Stretch) {
				var elementAlignment = GetItemAlignment(element);
				if(!(elementAlignment == ItemAlignment.Start || elementAlignment == ItemAlignment.Stretch))
					SetItemAlignment(element, ItemAlignment.Start);
			}
			else
				SetItemAlignment(element, insertionPointAlignment);
		}
		#endregion Layout Modification
	}
	public enum ItemAlignment { Start, Center, End, Stretch }
	public static class ItemAlignmentExtensions {
		public static ItemAlignment GetItemAlignment(this HorizontalAlignment alignment) {
			switch(alignment) {
				case HorizontalAlignment.Left:
					return ItemAlignment.Start;
				case HorizontalAlignment.Center:
					return ItemAlignment.Center;
				case HorizontalAlignment.Right:
					return ItemAlignment.End;
				case HorizontalAlignment.Stretch:
					return ItemAlignment.Stretch;
				default:
					return ItemAlignment.Start;
			}
		}
		public static ItemAlignment GetItemAlignment(this VerticalAlignment alignment) {
			switch(alignment) {
				case VerticalAlignment.Top:
					return ItemAlignment.Start;
				case VerticalAlignment.Center:
					return ItemAlignment.Center;
				case VerticalAlignment.Bottom:
					return ItemAlignment.End;
				case VerticalAlignment.Stretch:
					return ItemAlignment.Stretch;
				default:
					return ItemAlignment.Start;
			}
		}
		public static HorizontalAlignment GetHorizontalAlignment(this ItemAlignment alignment) {
			switch(alignment) {
				case ItemAlignment.Start:
					return HorizontalAlignment.Left;
				case ItemAlignment.Center:
					return HorizontalAlignment.Center;
				case ItemAlignment.End:
					return HorizontalAlignment.Right;
				case ItemAlignment.Stretch:
					return HorizontalAlignment.Stretch;
				default:
					return HorizontalAlignment.Left;
			}
		}
		public static VerticalAlignment GetVerticalAlignment(this ItemAlignment alignment) {
			switch(alignment) {
				case ItemAlignment.Start:
					return VerticalAlignment.Top;
				case ItemAlignment.Center:
					return VerticalAlignment.Center;
				case ItemAlignment.End:
					return VerticalAlignment.Bottom;
				case ItemAlignment.Stretch:
					return VerticalAlignment.Stretch;
				default:
					return VerticalAlignment.Top;
			}
		}
	}
	public class ItemInfo {
		public ItemInfo(double length, double minLength, double maxLength, ItemAlignment alignment) {
			MinLength = minLength;
			MaxLength = maxLength;
			Length = length;
			Alignment = alignment;
		}
		public ItemAlignment Alignment { get; private set; }
		public bool IsProcessed { get; set; }
		public double Length { get; set; }
		public double MinLength { get; set; }
		public double MaxLength { get; private set; }
		public double Offset = double.NaN;
	}
	public class ItemInfos : List<ItemInfo> {
		public ItemInfos(double availableLength, double itemSpace) {
			AvailableLength = availableLength;
			ItemSpace = itemSpace;
		}
		public virtual void ArrangeItems(FrameworkElements items, Func<FrameworkElement, ItemAlignment> getItemAlignment) {
			var centerAlignedItemCount = 0;
			var endAlignedItemCount = 0;
			var i = 0;
			FrameworkElement item;
			while(i < items.Count - (centerAlignedItemCount + endAlignedItemCount)) {
				item = items[i];
				switch(getItemAlignment(item)) {
					case ItemAlignment.Center:
						items.RemoveAt(i);
						items.Insert(items.Count - endAlignedItemCount, item);
						centerAlignedItemCount++;
						break;
					case ItemAlignment.End:
						items.RemoveAt(i);
						items.Insert(items.Count, item);
						endAlignedItemCount++;
						break;
					default:
						i++;
						break;
				}
			}
		}
		public virtual void Calculate() {
			CalculateStretchedLengths();
			CalculateOffsets();
		}
		public double AvailableLength { get; private set; }
		public double ItemSpace { get; private set; }
		protected virtual void CalculateOffsets() {
			var startOffset = CalculateOffsets(0, true,
				itemInfo => itemInfo.Alignment == ItemAlignment.Start || itemInfo.Alignment == ItemAlignment.Stretch);
			var endOffset = CalculateOffsets(Math.Max(Length, AvailableLength), false,
				itemInfo => itemInfo.Alignment == ItemAlignment.End);
			CalculateOffsets(Math.Round((startOffset + endOffset - CenteredLength) / 2), true,
				itemInfo => itemInfo.Alignment == ItemAlignment.Center);
		}
		protected virtual double CalculateOffsets(double offset, bool forward, Predicate<ItemInfo> isItemForProcessing) {
			ItemInfo itemInfo;
			var i = forward ? 0 : Count - 1;
			while(forward && i < Count || !forward && i >= 0) {
				itemInfo = this[i];
				if(isItemForProcessing(itemInfo)) {
					if(!forward)
						offset -= itemInfo.Length;
					itemInfo.Offset = offset;
					if(forward)
						offset += itemInfo.Length + ItemSpace;
					else
						offset -= ItemSpace;
				}
				if(forward)
					i++;
				else
					i--;
			}
			return offset;
		}
		protected virtual void CalculateStretchedLengths() {
			var stretchedItems = GetStretchedItems();
			if (stretchedItems.Count == 0)
				return;
			var calculator = new StretchedLengthsCalculator(
				Math.Max(0, AvailableLength - FixedLength - (stretchedItems.Count - 1) * ItemSpace));
			calculator.AddRange(stretchedItems);
			if (calculator.CanFitItemsUniformly())
				calculator.ForEach(
					delegate(ItemInfo item) {
						item.MinLength = item.Length;
						item.Length = 0;
					});
			calculator.Calculate();
		}
		protected virtual double GetLength(ItemAlignment? alignment) {
			double result = 0;
			foreach(var itemInfo in this)
				if(alignment == null || itemInfo.Alignment == alignment)
					result += itemInfo.Length + ItemSpace;
			if(result != 0)
				result -= ItemSpace;
			return result;
		}
		protected List<ItemInfo> GetStretchedItems() {
			var result = new List<ItemInfo>();
			foreach(var itemInfo in this)
				if(itemInfo.Alignment == ItemAlignment.Stretch)
					result.Add(itemInfo);
			return result;
		}
		protected double CenteredLength { get { return GetLength(ItemAlignment.Center); } }
		protected double FixedLength { get { return Length - GetLength(ItemAlignment.Stretch); } }
		protected double Length { get { return GetLength(null); } }
	}
	public class StretchedLengthsCalculator : List<ItemInfo> {
		public StretchedLengthsCalculator(double availableLength) {
			AvailableLength = availableLength;
		}
		public void Calculate() {
			if(Count == 0)
				return;
			var availableLength = double.IsInfinity(AvailableLength) ? ItemsLength : AvailableLength;
			do {
				var itemsLength = ItemsLength;
				double offset = 0, stretchedOffset, prevStretchedOffset = 0;
				double neededLength = 0, extraLength = 0;
				for(int i = 0; i < Count; i++) {
					ItemInfo itemInfo = this[i];
					if(!itemInfo.IsProcessed) {
						if(itemsLength != 0)
							stretchedOffset = Math.Round(availableLength * (offset + itemInfo.Length) / itemsLength);
						else
							stretchedOffset = GetUniformStretchedOffset(i + 1);
						offset += itemInfo.Length;
						itemInfo.Length = stretchedOffset - prevStretchedOffset;
						prevStretchedOffset += itemInfo.Length;
						if(itemInfo.Length < itemInfo.MinLength)
							neededLength += itemInfo.MinLength - itemInfo.Length;
						if(itemInfo.Length > itemInfo.MaxLength)
							extraLength += itemInfo.Length - itemInfo.MaxLength;
					}
				}
				if(neededLength == 0 && extraLength == 0)
					break;
				NeedsMoreLength = neededLength != 0;
				ProcessMinOrMaxLengthItems(neededLength > extraLength, ref availableLength);
			}
			while(true);
		}
		public bool CanFitItemsUniformly() {
			return ItemsLength != AvailableLength;
		}
		public double AvailableLength { get; private set; }
		public bool NeedsMoreLength { get; private set; }
		protected double GetUniformStretchedOffset(int index) {
			if (Count == 1)
				return AvailableLength * index;
			else
				return Math.Round(AvailableLength * index / Count);
		}
		protected void ProcessMinOrMaxLengthItems(bool processMinLengthItems, ref double availableLength) {
			foreach(var itemInfo in this)
				if(!itemInfo.IsProcessed &&
					(processMinLengthItems && itemInfo.Length < itemInfo.MinLength ||
					 !processMinLengthItems && itemInfo.Length > itemInfo.MaxLength)) {
					if(processMinLengthItems)
						itemInfo.Length = itemInfo.MinLength;
					else
						itemInfo.Length = itemInfo.MaxLength;
					itemInfo.IsProcessed = true;
					availableLength -= itemInfo.Length;
				}
			availableLength = Math.Max(0, availableLength);
		}
		protected double ItemsLength {
			get {
				double result = 0;
				foreach(var itemInfo in this)
					if(!itemInfo.IsProcessed)
						result += itemInfo.Length;
				return result;
			}
		}
	}
	public enum LayoutItemInsertionKind { None, Left, Top, Right, Bottom, Inside }
	public static class LayoutItemInsertionKindExtensions {
		public static Side? GetSide(this LayoutItemInsertionKind kind) {
			switch (kind) {
				case LayoutItemInsertionKind.Left:
					return Side.Left;
				case LayoutItemInsertionKind.Top:
					return Side.Top;
				case LayoutItemInsertionKind.Right:
					return Side.Right;
				case LayoutItemInsertionKind.Bottom:
					return Side.Bottom;
				default:
					return null;
			}
		}
	}
	public class LayoutItemInsertionPoint {
		public LayoutItemInsertionPoint(FrameworkElement element, bool isInternalInsertion) {
			Element = element;
			IsInternalInsertion = isInternalInsertion;
		}
		public override bool Equals(object obj) {
			return obj is LayoutItemInsertionPoint &&
				((LayoutItemInsertionPoint)obj).Element == Element && ((LayoutItemInsertionPoint)obj).IsInternalInsertion == IsInternalInsertion;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public FrameworkElement Element { get; set; }
		public bool IsInternalInsertion { get; set; }
	}
	public class LayoutItemInsertionPoints : List<LayoutItemInsertionPoint> {
		public LayoutItemInsertionPoint Find(FrameworkElement element) {
			for (int i = 0; i < Count; i++)
				if (this[i].Element == element)
					return this[i];
			return null;
		}
	}
	public struct LayoutItemInsertionInfo {
		public LayoutItemInsertionInfo(FrameworkElement destinationItem, LayoutItemInsertionKind insertionKind,
			LayoutItemInsertionPoint insertionPoint) {
			DestinationItem = destinationItem;
			InsertionKind = insertionKind;
			InsertionPoint = insertionPoint;
		}
		public override bool Equals(object obj) {
			if (obj is LayoutItemInsertionInfo) {
				var info = (LayoutItemInsertionInfo)obj;
				return DestinationItem == info.DestinationItem && InsertionKind == info.InsertionKind &&
					(InsertionPoint == null && info.InsertionPoint == null || InsertionPoint != null && InsertionPoint.Equals(info.InsertionPoint));
			}
			else
				return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public FrameworkElement DestinationItem;
		public LayoutItemInsertionKind InsertionKind;
		public LayoutItemInsertionPoint InsertionPoint;
	}
	public class LayoutGroupController : LayoutControllerBase {
		public LayoutGroupController(ILayoutGroup control)
			: base(control) {
		}
		public virtual FrameworkElement GetItem(Point p, bool ignoreLayoutGroups, bool ignoreLocking) {
			var result = (FrameworkElement)IPanel.ChildAt(p, true, true, true);
			if (result == null && !ignoreLayoutGroups && RectHelper.New(Control.GetSize()).Contains(p))
				return Control;
			if (!ignoreLocking && ILayoutGroup.IsLocked)
				return Control;
			if (result.IsLayoutGroup())
				result = ((ILayoutGroup)result).GetItem(Control.MapPoint(p, result), ignoreLayoutGroups, ignoreLocking);
			if (result == null && !ILayoutGroup.IsBorderless && !ILayoutGroup.IsRoot)
				result = Control;
			return result;
		}
		public ILayoutGroup ILayoutGroup { get { return IControl as ILayoutGroup; } }
		protected new LayoutGroupProvider LayoutProvider { get { return (LayoutGroupProvider)base.LayoutProvider; } }
		#region Scrolling
		public override bool IsScrollable() {
			return false;
		}
		#endregion Scrolling
		#region Drag&Drop
		public override FrameworkElement GetMoveableItem(Point p) {
			return GetItem(p, true, Control.IsInDesignTool());
		}
		protected override bool CanItemDragAndDrop() {
			return false;
		}
		#endregion
	}
}

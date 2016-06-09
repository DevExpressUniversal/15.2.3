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

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	public class DocumentGroup : TabbedGroup {
		#region static
		public static readonly DependencyProperty MDIStyleProperty;
		public static readonly DependencyProperty ClosePageButtonShowModeProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty LastClosePageButtonShowModeInternalProperty;
		public static readonly DependencyProperty ShowWhenEmptyProperty;
		public static readonly DependencyProperty PinLocationProperty;
		public static readonly DependencyProperty PinnedProperty;
		public new static readonly DependencyProperty ShowPinButtonProperty;
		static DocumentGroup() {
			var dProp = new DependencyPropertyRegistrator<DocumentGroup>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideMetadata(AllowFloatProperty, false);
			dProp.Register("MDIStyle", ref MDIStyleProperty, MDIStyle.Default,
				(dObj, e) => ((DocumentGroup)dObj).OnMDIStyleChanged((MDIStyle)e.NewValue));
			dProp.Register("ClosePageButtonShowMode", ref ClosePageButtonShowModeProperty, ClosePageButtonShowMode.Default,
				(dObj, e) => ((DocumentGroup)dObj).OnClosePageButtonShowModeChanged((ClosePageButtonShowMode)e.NewValue));
			dProp.RegisterAttached("LastClosePageButtonShowModeInternal", ref LastClosePageButtonShowModeInternalProperty, ClosePageButtonShowMode.Default);
			dProp.OverrideMetadata(GroupTemplateSelectorProperty, (DataTemplateSelector)new DefaultTemplateSelector(),
				(dObj, e) => ((DocumentGroup)dObj).OnGroupTemplateChanged());
			dProp.Register("ShowWhenEmpty", ref ShowWhenEmptyProperty, false,
				(dObj, e) => ((DocumentGroup)dObj).OnShowEmptyChanged((bool)e.OldValue, (bool)e.NewValue));
			dProp.RegisterAttached("PinLocation", ref PinLocationProperty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, TabHeaderPinLocation.Default);
			dProp.RegisterAttached("Pinned", ref PinnedProperty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, false);
			dProp.RegisterAttached("ShowPinButton", ref ShowPinButtonProperty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, false);
		}
		[XtraSerializableProperty]
		public static bool GetPinned(DependencyObject obj) {
			return (bool)obj.GetValue(PinnedProperty);
		}
		public static void SetPinned(DependencyObject obj, bool value) {
			obj.SetValue(PinnedProperty, value);
		}
		[XtraSerializableProperty]
		public static TabHeaderPinLocation GetPinLocation(DependencyObject obj) {
			return (TabHeaderPinLocation)obj.GetValue(PinLocationProperty);
		}
		public static void SetPinLocation(DependencyObject obj, TabHeaderPinLocation value) {
			obj.SetValue(PinLocationProperty, value);
		}
		public static bool GetShowPinButton(DependencyObject obj) {
			return (bool)obj.GetValue(ShowPinButtonProperty);
		}
		public static void SetShowPinButton(DependencyObject obj, bool value) {
			obj.SetValue(ShowPinButtonProperty, value);
		}
		#endregion static
		bool ContainsMaximizedDocument() {
			foreach(BaseLayoutItem item in Items) {
				if(MDIStateHelper.GetMDIState(item) == MDIState.Maximized)
					return true;
			}
			return false;
		}
		protected internal bool IsTabbed {
			get { return MDIStyle != MDIStyle.MDI; }
		}
		protected internal void ChangeMDIStyle() {
			if(IsTabbed) MDIStyle = MDIStyle.MDI;
			else ClearValue(MDIStyleProperty);
			LayoutItemsHelper.UpdateZIndexes(this);
		}
		protected override bool CoerceIsCloseButtonVisible(bool value) {
			if(IsTabbed) {
				ClosePageButtonShowMode mode = ClosePageButtonShowMode;
				switch(mode) {
					case ClosePageButtonShowMode.NoWhere:
					case ClosePageButtonShowMode.InAllTabPageHeaders:
					case ClosePageButtonShowMode.InActiveTabPageHeader:
						return false;
					default:
						return CanShowCloseButtonForSelectedItem();
				}
			}
			return ContainsMaximizedDocument() && CanShowCloseButtonForSelectedItem();
		}
		protected bool CanShowCloseButtonForSelectedItem() {
			return (SelectedItem != null) && SelectedItem.AllowClose && SelectedItem.ShowCloseButton;
		}
		protected bool CanShowRestoreButtonForSelectedItem() {
			return (SelectedItem is DocumentPanel) && ((DocumentPanel)SelectedItem).ShowRestoreButton;
		}
		protected override void OnSelectedItemChanged(BaseLayoutItem item, BaseLayoutItem oldItem) {
			base.OnSelectedItemChanged(item, oldItem);
			CoerceValue(IsRestoreButtonVisibleProperty);
			if(ContainsMaximizedDocument() && item != null && !item.AllowMaximize)
				MDIControllerHelper.Restore(item);
		}
		protected override bool CoerceDestroyOnClosingChildren(bool value) {
			return IsTabbed && value;
		}
		protected override void OnIsMaximizedChanged(bool maximized) {
			CoerceValue(SelectedItemProperty);
		}
		protected virtual void OnMDIStyleChanged(MDIStyle style) {
			CoerceValue(DestroyOnClosingChildrenProperty);
			CoerceValue(SelectedItemProperty);
			foreach(BaseLayoutItem item in Items) {
				item.CoerceValue(IsTabPageProperty);
			}
			if(IsTabbed) {
				DockLayoutManager manager = this.FindDockLayoutManager();
				if(manager != null && manager.ActiveMDIItem != null) {
					if(Items.Contains(manager.ActiveMDIItem))
						manager.ActiveMDIItem = null;
					if(Items.Contains(manager.ActiveDockItem))
						manager.ActiveDockItem = null;
				}
			}
			OnLayoutChanged();
			DockLayoutManagerExtension.Update(this);
		}
		protected internal override bool GetIsDocumentHost() {
			return Manager != null && Manager.DockingStyle != DockingStyle.Default && IsTabbed && Parent != null;
		}
		protected internal override void UpdateButtons() {
			base.UpdateButtons();
			CoerceValue(IsRestoreButtonVisibleProperty);
			CoerceValue(IsDropDownButtonVisibleProperty);
		}
		protected virtual void OnClosePageButtonShowModeChanged(ClosePageButtonShowMode style) {
			OnLayoutChanged();
			foreach(BaseLayoutItem item in Items) {
				item.CoerceValue(IsCloseButtonVisibleProperty);
			}
		}
		protected internal override void AfterItemRemoved(BaseLayoutItem item) {
			base.AfterItemRemoved(item);
			item.SetValue(LastClosePageButtonShowModeInternalProperty, ClosePageButtonShowMode);
			DocumentPanel document = item as DocumentPanel;
			if(document != null) {
				document.IsMDIChild = false;
			}
		}
		protected internal override void BeforeItemAdded(BaseLayoutItem item) {
			base.BeforeItemAdded(item);
			DocumentPanel document = item as DocumentPanel;
			if(document != null) {
				document.IsMDIChild = MDIStyle == MDIStyle.MDI;
			}
		}
		protected override LayoutItemType GetLayoutItemTypeCore() {
			return LayoutItemType.DocumentPanelGroup;
		}
		protected override BaseLayoutItem GetContainerForItemCore(object content, DataTemplate itemTemplate = null, DataTemplateSelector itemTemplateSelector = null) {
			BaseLayoutItem container;
			if(IsControlItemsHost)
				container = base.GetContainerForItemCore(content, itemTemplate, itemTemplateSelector);
			else
				container = Manager != null ? Manager.CreateDocumentPanel() : new DocumentPanel();
			return container;
		}
		protected override bool IsItemItsOwnContainer(object item) {
			return item is LayoutPanel;
		}
		internal bool IsAutoGenerated { get; set; }
		protected internal Size MDIAreaSize { get; set; }
		protected internal double MDIDocumentHeaderHeight { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override GroupBorderStyle GroupBorderStyle { get { return base.GroupBorderStyle; } set { } }
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DocumentGroupMDIStyle"),
#endif
 XtraSerializableProperty, Category("Content")]
		public MDIStyle MDIStyle {
			get { return (MDIStyle)GetValue(MDIStyleProperty); }
			set { SetValue(MDIStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DocumentGroupClosePageButtonShowMode"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public ClosePageButtonShowMode ClosePageButtonShowMode {
			get { return (ClosePageButtonShowMode)GetValue(ClosePageButtonShowModeProperty); }
			set { SetValue(ClosePageButtonShowModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DocumentGroupShowDropDownButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowDropDownButton {
			get { return (bool)GetValue(ShowDropDownButtonProperty); }
			set { SetValue(ShowDropDownButtonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfDockingLocalizedDescription("DocumentGroupShowRestoreButton"),
#endif
 XtraSerializableProperty, Category("Layout")]
		public bool ShowRestoreButton {
			get { return (bool)GetValue(ShowRestoreButtonProperty); }
			set { SetValue(ShowRestoreButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DocumentGroupIsDropDownButtonVisible")]
#endif
		public bool IsDropDownButtonVisible {
			get { return (bool)GetValue(IsDropDownButtonVisibleProperty); }
		}
#if !SL
	[DevExpressXpfDockingLocalizedDescription("DocumentGroupIsRestoreButtonVisible")]
#endif
		public bool IsRestoreButtonVisible {
			get { return (bool)GetValue(IsRestoreButtonVisibleProperty); }
		}
		public bool ShowWhenEmpty {
			get { return (bool)GetValue(ShowWhenEmptyProperty); }
			set { SetValue(ShowWhenEmptyProperty, value); }
		}
		protected virtual void OnShowEmptyChanged(bool oldValue, bool newValue) {
			Parent.Do(x => x.CoerceValue(HasNotCollapsedItemsProperty));
		}
		protected override bool CoerceIsDropDownButtonVisible(bool visible) {
			return HasTabHeader() && (Items.Count > 0) && ShowDropDownButton;
		}
		protected override bool CoerceIsRestoreButtonVisible(bool visible) {
			return !IsTabbed && ContainsMaximizedDocument() && CanShowRestoreButtonForSelectedItem() && ShowRestoreButton;
		}
		protected override bool HasTabHeader() {
			return IsTabbed;
		}
		protected override TabHeaderLayoutType CoerceTabHeaderLayoutType(TabHeaderLayoutType value) {
			if(value != TabHeaderLayoutType.Default) return value;
			return TabHeaderLayoutType.Scroll;
		}
		protected override Size CalcMaxSizeValue(Size value) {
			return IsTabbed ? base.CalcMaxSizeValue(value) : value;
		}
		protected override Size CalcMinSizeValue(Size value) {
			return IsTabbed ? base.CalcMinSizeValue(value) : value;
		}
		protected internal override void AfterItemAdded(int index, BaseLayoutItem item) {
			base.AfterItemAdded(index, item);
			if(!IsTabbed && IsMaximized && item is DocumentPanel) {
				DocumentPanel.SetMDIState(item, MDIState.Maximized);
			}
		}
		protected override LayoutGroup GetContainerHost(ContentItem container) {
			LayoutGroup target = null;
			if(!HasNotCollapsedItems && Parent != null && Parent.Items.IsDocumentHost(true)) {
				var lastActiveItem = Parent.GetNestedPanels().Where((item) => (item is LayoutPanel && item.Parent is DocumentGroup)).OrderBy((item) => ((LayoutPanel)item).LastActivationDateTime).LastOrDefault();
				if(lastActiveItem != null) {
					DocumentGroup lastActiveItemParent = lastActiveItem.Parent as DocumentGroup;
					if(lastActiveItemParent != null && lastActiveItemParent.IsAutoGenerated)
						target = lastActiveItem.Parent;
				}
			}
			return target ?? this;
		}
		#region Internal classes
		class DefaultTemplateSelector : DefaultItemTemplateSelectorWrapper.DefaultItemTemplateSelector {
			public override DataTemplate SelectTemplate(object item, DependencyObject container) {
				DocumentPaneContentPresenter presenter = container as DocumentPaneContentPresenter;
				DocumentGroup documentGroup = item as DocumentGroup;
				if(documentGroup != null && presenter != null && presenter.Owner != null)
					switch(documentGroup.MDIStyle) {
						case MDIStyle.Default:
						case MDIStyle.Tabbed:
							return presenter.Owner.TabbedTemplate;
						case MDIStyle.MDI:
							return presenter.Owner.MDITemplate;
					}
				return null;
			}
		}
		#endregion Internal classes
	}
}

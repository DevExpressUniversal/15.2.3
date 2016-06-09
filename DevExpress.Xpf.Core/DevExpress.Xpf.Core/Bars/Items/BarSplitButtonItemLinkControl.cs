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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using System.Windows.Documents;
using System.Windows.Threading;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
namespace DevExpress.Xpf.Bars {
	[TemplatePart(Name = PART_ArrowButton, Type = typeof(Grid))]
	[TemplatePart(Name = PART_CustomizationBorder, Type = typeof(ItemBorderControl))]
	[TemplatePart(Name = PART_ArrowBorder, Type = typeof(ItemBorderControl))]
	[TemplatePart(Name = PART_ControlBorder, Type = typeof(ItemBorderControl))]
	[TemplatePart(Name = PART_ArrowContent, Type = typeof(ContentControl))]
	[TemplatePart(Name = PART_ArrowControl, Type = typeof(BarSplitButtonItemArrowControl))]
	[TemplateVisualState(Name = "Normal", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Hot", GroupName = "ItemState")]
	[TemplateVisualState(Name = "ArrowHot", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Pressed", GroupName = "ItemState")]
	[TemplateVisualState(Name = "ArrowPressed", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Customization", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Disabled", GroupName = "ItemState")]
	[TemplateVisualState(Name = "Bottom", GroupName = "ArrowAlignment")]
	[TemplateVisualState(Name = "Right", GroupName = "ArrowAlignment")]
	[TemplateVisualState(Name = "NoArrow", GroupName = "ArrowAlignment")]
	public class BarSplitButtonItemLinkControl : BarButtonItemLinkControl, IPopupOwner 
{
		#region Constants
		const string PART_ArrowButton = "PART_ArrowButton";
		const string PART_CustomizationBorder = "PART_CustomizationBorder";
		const string PART_ArrowBorder = "PART_ArrowBorder";
		const string PART_ControlBorder = "PART_ControlBorder";
		const string PART_ArrowContent = "PART_ArrowContent";
		const string PART_ArrowControl = "PART_ArrowControl";
		#endregion Constants
		#region static
		public static readonly DependencyProperty ContentAndArrowLayoutPanelStyleProperty;
		static readonly DependencyPropertyKey IsArrowPressedPropertyKey;
		protected static readonly DependencyPropertyKey ShowArrowHotBorderPropertyKey;
		protected static readonly DependencyPropertyKey ShowArrowPressedBorderPropertyKey;
		static readonly DependencyPropertyKey ActualArrowAlignmentPropertyKey;
		static readonly DependencyPropertyKey ShowContentInArrowPropertyKey;
		protected static readonly DependencyPropertyKey ContentBorderStatePropertyKey;
		protected static readonly DependencyPropertyKey IsContentBorderActivePropertyKey;
		protected static readonly DependencyPropertyKey IsArrowBorderActivePropertyKey;
		protected static readonly DependencyPropertyKey ArrowBorderStatePropertyKey;
		public static readonly DependencyProperty IsArrowPressedProperty;
		public static readonly DependencyProperty ActualArrowAlignmentProperty;
		public static readonly DependencyProperty ShowContentInArrowProperty;
		public static readonly DependencyProperty ContentBorderStateProperty;
		public static readonly DependencyProperty IsContentBorderActiveProperty;
		public static readonly DependencyProperty IsArrowBorderActiveProperty;
		public static readonly DependencyProperty ArrowBorderStateProperty;
		public static readonly DependencyProperty ShowArrowPressedBorderProperty;
		public static readonly DependencyProperty ShowArrowHotBorderProperty;
		public static readonly DependencyProperty ItemPositionProperty;
		protected static readonly DependencyPropertyKey ContentPartItemPositionPropertyKey;
		protected static readonly DependencyPropertyKey ArrowPartItemPositionPropertyKey;
		public static readonly DependencyProperty ContentPartItemPositionProperty;
		public static readonly DependencyProperty ArrowPartItemPositionProperty;
		protected static readonly DependencyPropertyKey ActualActAsDropDownPropertyKey;
		public static readonly DependencyProperty ActualActAsDropDownProperty;
		public static readonly DependencyProperty ItemClickBehaviourProperty;
		static BarSplitButtonItemLinkControl() {
			ItemClickBehaviourProperty = DependencyPropertyManager.Register("ItemClickBehaviour", typeof(PopupItemClickBehaviour), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(PopupItemClickBehaviour.Undefined));
			ContentAndArrowLayoutPanelStyleProperty = DependencyPropertyManager.Register("ContentAndArrowLayoutPanelStyle", typeof(Style), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			IsArrowPressedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsArrowPressed", typeof(bool), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(false, (d, e) => ((BarSplitButtonItemLinkControl)d).OnIsArrowPressedChanged(e)));
			ShowArrowHotBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowArrowHotBorder", typeof(bool), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BarSplitButtonItemLinkControl)d).OnShowArrowHotBorderChanged(e)));
			ShowArrowPressedBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowArrowPressedBorder", typeof(bool), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BarSplitButtonItemLinkControl)d).OnShowArrowPressedBorderChanged(e)));
			ActualArrowAlignmentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualArrowAlignment", typeof(Dock), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(Dock.Right, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BarSplitButtonItemLinkControl)d).OnActualArrowAlignmentChanged(e)));
			ShowContentInArrowPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowContentInArrow", typeof(bool), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ContentBorderStatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ContentBorderState", typeof(BorderState), typeof(BarSplitButtonItemLinkControl), new UIPropertyMetadata(BorderState.Normal));
			IsContentBorderActivePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsContentBorderActive", typeof(bool), typeof(BarSplitButtonItemLinkControl), new UIPropertyMetadata(false));
			IsArrowBorderActivePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsArrowBorderActive", typeof(bool), typeof(BarSplitButtonItemLinkControl), new UIPropertyMetadata(false));
			ArrowBorderStatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ArrowBorderState", typeof(BorderState), typeof(BarSplitButtonItemLinkControl), new UIPropertyMetadata(BorderState.Normal));
			IsArrowPressedProperty = IsArrowPressedPropertyKey.DependencyProperty;
			ActualArrowAlignmentProperty = ActualArrowAlignmentPropertyKey.DependencyProperty;
			ShowContentInArrowProperty = ShowContentInArrowPropertyKey.DependencyProperty;
			ContentBorderStateProperty = ContentBorderStatePropertyKey.DependencyProperty;
			IsContentBorderActiveProperty = IsContentBorderActivePropertyKey.DependencyProperty;
			IsArrowBorderActiveProperty = IsArrowBorderActivePropertyKey.DependencyProperty;
			ArrowBorderStateProperty = ArrowBorderStatePropertyKey.DependencyProperty;
			ShowArrowPressedBorderProperty = ShowArrowPressedBorderPropertyKey.DependencyProperty;
			ShowArrowHotBorderProperty = ShowArrowHotBorderPropertyKey.DependencyProperty;
			ItemPositionProperty = DependencyPropertyManager.Register("ItemPosition", typeof(HorizontalItemPositionType), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(HorizontalItemPositionType.Single, (d, e) => ((BarSplitButtonItemLinkControl)d).UpdateActualItemPosition()));
			ContentPartItemPositionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ContentPartItemPosition", typeof(HorizontalItemPositionType), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(HorizontalItemPositionType.Left, (d, e) => ((BarSplitButtonItemLinkControl)d).UpdateActualItemPosition()));
			ArrowPartItemPositionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ArrowPartItemPosition", typeof(HorizontalItemPositionType), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(HorizontalItemPositionType.Right, (d, e) => ((BarSplitButtonItemLinkControl)d).UpdateActualItemPosition()));
			ContentPartItemPositionProperty = ContentPartItemPositionPropertyKey.DependencyProperty;
			ArrowPartItemPositionProperty = ArrowPartItemPositionPropertyKey.DependencyProperty;
			ActualActAsDropDownPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualActAsDropDown", typeof(bool), typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(false, (d, e) => ((BarSplitButtonItemLinkControl)d).OnActualActAsDropDownChanged()));
			ActualActAsDropDownProperty = ActualActAsDropDownPropertyKey.DependencyProperty;
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarSplitButtonItemLinkControl), typeof(BarSplitButtonItemLinkControlAutomationPeer), owner => new BarSplitButtonItemLinkControlAutomationPeer((BarSplitButtonItemLinkControl)owner));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarSplitButtonItemLinkControl), new FrameworkPropertyMetadata(typeof(BarSplitButtonItemLinkControl)));
		}
		#endregion Dependency Properties
		public Dock ActualArrowAlignment {
			get { return (Dock)GetValue(ActualArrowAlignmentProperty); }
			private set { this.SetValue(ActualArrowAlignmentPropertyKey, value); }
		}
		public bool ShowContentInArrow {
			get { return (bool)GetValue(ShowContentInArrowProperty); }
			protected set { this.SetValue(ShowContentInArrowPropertyKey, value); }
		}
		public PopupItemClickBehaviour ItemClickBehaviour {
			get { return (PopupItemClickBehaviour)GetValue(ItemClickBehaviourProperty); }
			set { SetValue(ItemClickBehaviourProperty, value); }
		}
		public HorizontalItemPositionType ItemPosition {
			get { return (HorizontalItemPositionType)GetValue(ItemPositionProperty); }
			set { SetValue(ItemPositionProperty, value); }
		}
		public HorizontalItemPositionType ContentPartItemPosition {
			get { return (HorizontalItemPositionType)GetValue(ContentPartItemPositionProperty); }
			protected set { this.SetValue(ContentPartItemPositionPropertyKey, value); }
		}
		public HorizontalItemPositionType ArrowPartItemPosition {
			get { return (HorizontalItemPositionType)GetValue(ArrowPartItemPositionProperty); }
			protected set { this.SetValue(ArrowPartItemPositionPropertyKey, value); }
		}
		[Browsable(false), ReadOnly(true)]
		public bool ShowArrowHotBorder {
			get { return (bool)GetValue(ShowArrowHotBorderProperty); }
			protected internal set { this.SetValue(ShowArrowHotBorderPropertyKey, value); }
		}
		[Browsable(false), ReadOnly(true)]
		public bool ShowArrowPressedBorder {
			get { return (bool)GetValue(ShowArrowPressedBorderProperty); }
			protected internal set { this.SetValue(ShowArrowPressedBorderPropertyKey, value); }
		}
		[Browsable(false), ReadOnly(true)]
		public bool IsArrowPressed {
			get { return (bool)base.GetValue(IsArrowPressedProperty); }
			protected set { this.SetValue(IsArrowPressedPropertyKey, value); }
		}
		public Style ContentAndArrowLayoutPanelStyle {
			get { return (Style)GetValue(ContentAndArrowLayoutPanelStyleProperty); }
			set { SetValue(ContentAndArrowLayoutPanelStyleProperty, value); }
		}
		public BorderState ContentBorderState {
			get { return (BorderState)GetValue(ContentBorderStateProperty); }
			protected set { this.SetValue(ContentBorderStatePropertyKey, value); }
		}
		public bool IsContentBorderActive {
			get { return (bool)GetValue(IsContentBorderActiveProperty); }
			protected set { this.SetValue(IsContentBorderActivePropertyKey, value); }
		}
		public bool IsArrowBorderActive {
			get { return (bool)GetValue(IsArrowBorderActiveProperty); }
			protected set { this.SetValue(IsArrowBorderActivePropertyKey, value); }
		}
		public BorderState ArrowBorderState {
			get { return (BorderState)GetValue(ArrowBorderStateProperty); }
			protected set { this.SetValue(ArrowBorderStatePropertyKey, value); }
		}
		public bool ActualActAsDropDown {
			get { return (bool)GetValue(ActualActAsDropDownProperty); }
			protected set { this.SetValue(ActualActAsDropDownPropertyKey, value); }
		}		
		protected object GetTemplateFromProvider(DependencyProperty prop, BarSplitButtonItemThemeKeys themeKeys) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarSplitButtonItemThemeKeyExtension() { ResourceKey = themeKeys })];
			if (res != null)
				return res;
			return GetValue(prop);
		}	 
		public BarSplitButtonItemLink SplitButtonLink { get { return base.Link as BarSplitButtonItemLink; } }
		public BarSplitButtonItem SplitButtonItem { get { return Item as BarSplitButtonItem; } }
		public bool IsPopupOpen { get { return GetPopupControl() != null && GetPopupControl().IsPopupOpen && GetPopupControl().With(x => x.Popup).With(x => x.OwnerLinkControl) == this; } }
		protected internal override bool CloseSubMenuOnClick {
			get {
				if (LayoutPanel.If(x => x.IsMouseOverSecondBorder).ReturnSuccess())
					return false;
				return base.CloseSubMenuOnClick;
			}
		}
		public BarSplitButtonItemLinkControl() : this(null) { }
		public BarSplitButtonItemLinkControl(BarSplitButtonItemLink link) : base(link) {
			popupExpandMode = BarPopupExpandMode.Classic;
			CreateBindings();
		}	  
		protected internal virtual IPopupControl GetPopupControl() {
			if(SplitButtonLink != null)
				return SplitButtonLink.PopupControl;
			if(Item is BarSplitButtonItem)
				return ((BarSplitButtonItem)Item).PopupControl;
			return null;
		}
		public void ShowPopup() {
			if(ContainerType == LinkContainerType.RadialMenu) {
				if(Item != null)
					this.VisualParents().OfType<RadialMenuControl>().First().ShowSubMenu(Item);
				return;
			}
			var popupControl = GetPopupControl();
			if(popupControl == null || popupControl.IsPopupOpen)
				return;
			UpdatePressedState();
			var popup = popupControl.Popup;
			popup.OwnerLinkControl = this;
			var expandablePopup = popup as BarPopupExpandable;
			if(expandablePopup != null && System.Windows.DependencyPropertyHelper.GetValueSource(expandablePopup, BarPopupExpandable.ExpandModeProperty).BaseValueSource != BaseValueSource.Local) {
				var parentPopup = PopupMenuManager.GetParentPopup(expandablePopup) as BarPopupExpandable;
				var expandMode = parentPopup.Return(p => p.ExpandMode, () => popupExpandMode);
				expandablePopup.SetCurrentValue(BarPopupExpandable.ExpandModeProperty, expandMode);
			}
			StopOpenPopupTimer();
			SubscribePopupControl();
			popup.IsBranchHeader = true;
			SavedPopupControlWidth = popup.Width;
			SavedPopupControlHeight = popup.Height;
			popup.IsRoot = IsOnBar || IsLinkInRibbon;
			popup.HorizontalOffset = 0d;
			if(IsOnBar) {
				popup.Placement = PlacementMode.Bottom;
				if(Orientation == Orientation.Vertical) {
					popup.Placement = PlacementMode.Right;
					if(RotateWhenVertical) {
						popup.VerticalOffset = Math.Max(0, RenderSize.Width - RenderSize.Height);
					}
				}
			} else {
				popup.Placement = BarManagerHelper.GetPopupPlacement(this);
			}
			BeforeOpenPopup();
			popup.ShowPopup(PopupPlacementTarget);
			AfterClosePopup();
			BarPopupBase.UpdateSubMenuBounds(this, popup);
		}
		BarPopupBase openedPopup = null;
		void BeforeOpenPopup() {
			var popup = GetPopupControl().With(x => x.Popup);
			if (popup == null || popup.IsOpen || openedPopup!=null)
				return;
			(popup.OwnerLinkControl as BarSplitButtonItemLinkControl).Do(x=>x.AfterClosePopup());
			if (VisualTreeHelper.GetParent(popup) != null)
				return;
			popup.PlacementTarget = PopupPlacementTarget;
			openedPopup = popup;
			PopupPlacementTarget.Child = popup;
			popup.OwnerLinkControl = this;
			BarManagerHelper.SetPopup(popup, BarManagerHelper.GetPopup(this));
		}
		void AfterClosePopup() {
			var popup = openedPopup;
			if (popup == null || popup.IsOpen)
				return;
			openedPopup = null;
			PopupPlacementTarget.Child = null;
			if (popup.OwnerLinkControl == this)
				popup.OwnerLinkControl = null;
			BarManagerHelper.SetPopup(popup, null);
		}
		protected override int VisualChildrenCount {
			get { return base.VisualChildrenCount + ((openedPopup != null) ? 1 : 0); }
		}
		protected override Visual GetVisualChild(int index) {
			if (openedPopup != null && index == VisualChildrenCount - 1)
				return openedPopup;
			return base.GetVisualChild(index);
		}
		protected internal override void ForceSetIsSelected(bool value) {
			base.ForceSetIsSelected(value);
			if (!value && IsLinkControlInMainMenu)
				ClosePopup();
		}
		protected virtual void UpdatePressedState() {
			if(IsPressed || IsArrowPressed) return;
			if(ActualActAsDropDown) {
				IsPressed = true;
			} else {
				IsArrowPressed = true;
			}
		}
		public void ClosePopup() {
			var popup = openedPopup ?? GetPopupControl().With(x => x.Popup);
			if (popup == null)
				return;
			if(popup.OwnerLinkControl == this && popup.IsOpen) {
				popup.ClosePopup();
			}
			IsArrowPressed = IsPressed = IsHighlighted = false;
			ShowArrowHotBorder = false;
			ShowArrowPressedBorder = false;
			UpdateShowArrowPressedBorder();
		}
		protected override void OnLinkInfoChanged(BarItemLinkInfo oldValue) {
			base.OnLinkInfoChanged(oldValue);
			if (LinkInfo == null)
				ClosePopup();				
		}
		public void ClosePopup(bool ignoreSetMenuMode) {
			GetPopupControl().With(x => x.Popup).Do(x => x.ClosePopup(ignoreSetMenuMode));
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected NonLogicalDecorator PopupPlacementTarget { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			PopupPlacementTarget = GetTemplateChild("PART_PopupPlacementTarget") as NonLogicalDecorator;
			if(LayoutPanel != null) {
				LayoutPanel.CalculateIsMouseOver = true;
				LayoutPanel.MouseDown += new MouseButtonEventHandler(OnArrowButtonMouseDown);
			}
			UpdateVisualStateByArrowAlignment();
		}
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get {
				return key => new BarSplitButtonItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName };
			}
		}
		protected override void UpdateLayoutPanel() {
			if(LayoutPanel == null) return;
			base.UpdateLayoutPanel();
			LayoutPanel.ShowArrow = true;
			LayoutPanel.SecondBorderPlacement = ContainerType == LinkContainerType.RadialMenu ? SecondBorderPlacement.ContentAndArrow : SecondBorderPlacement.Arrow;
			UpdateLayoutPanelShowSecondBorder();
			UpdateLayoutPanelArrowAlignment();
			UpdateLayoutPanelActAsDropDown();
		}
		protected internal override void UpdateLayoutPanelHorizontalItemPosition() {
			if(LayoutPanel == null) return;
			if(!LayoutPanel.ShowSecondBorder) {
				LayoutPanel.FirstBorderItemPosition = ArrowPartItemPosition;
			} else {
				LayoutPanel.FirstBorderItemPosition = ContentPartItemPosition;
				LayoutPanel.SecondBorderItemPosition = ArrowPartItemPosition;
			}
		}
		protected override void OnActualIsContentEnabledChanged(bool oldValue) {
			UpdateActualIsHoverEnabled();
		}
		protected virtual void UpdateActualIsHoverEnabled() {
			ActualIsHoverEnabled = !ActualActAsDropDown && ActualIsContentEnabled;
		}
		protected virtual void UpdateLayoutPanelShowSecondBorder() {
			if(LayoutPanel == null) return;
			LayoutPanel.ShowSecondBorder = !ActualActAsDropDown && !ShowCustomizationBorder;
			UpdateLayoutPanelHorizontalItemPosition();
		}
		private void UpdateLayoutPanelActAsDropDown() {
			if (LayoutPanel == null)
				return;
			LayoutPanel.ActAsDropDown = ActualActAsDropDown;
		}
		protected virtual void UpdateLayoutPanelArrowAlignment() {
			if(LayoutPanel == null) return;
			if(ActualActAsDropDown && ContainerType != LinkContainerType.RibbonPageGroup && ContainerType != LinkContainerType.ApplicationMenu &&
	ContainerType != LinkContainerType.DropDownGallery && ContainerType != LinkContainerType.Menu) {
				LayoutPanel.ShowSecondBorder = false;
			} else if(ContainerType == LinkContainerType.RadialMenu){
				LayoutPanel.ShowSecondBorder = true;
				LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Bottom;
			}
			else {
				LayoutPanel.ShowArrow = true;
				LayoutPanel.ShowSecondBorder = true;
				if(ActualArrowAlignment == Dock.Right)
					LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Left;
				else
					LayoutPanel.ContentAndGlyphToArrowAlignment = Dock.Top;
			}
		}
		protected override void UpdateLayoutPanelGlyphToContentAlignment() {
			if(LayoutPanel == null) return;
			LayoutPanel.GlyphToContentAlignment = ContainerType == LinkContainerType.RibbonPageGroup && CurrentRibbonStyle == RibbonItemStyles.Large ? Dock.Top : ActualGlyphAlignment;
		}
		protected override void OnSpacingModeChanged(SpacingMode oldValue) {
			base.OnSpacingModeChanged(oldValue);
			var pop = GetPopupControl() as PopupMenu;
			if (pop!=null)
			pop.ContentControl.SpacingMode = SpacingMode;
		}
		protected virtual bool CanShowPopup {
			get { return GetPopupControl() != null && !IsLinkInCustomizationMode; }
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateVisualStateByArrowAlignment();
		}
		protected virtual void CreateBindings() {
			Binding bnd = new Binding();
			bnd.Path = new PropertyPath("(0)", ItemPositionTypeProvider.HorizontalItemPositionProperty);
			bnd.Source = this;
			SetBinding(ItemPositionProperty, bnd);
		}
		protected override void OnToolTipOpening(ToolTipEventArgs e) {
			if(IsPopupOpen) {
				if(Mouse.DirectlyOver is DependencyObject && ((DependencyObject)Mouse.DirectlyOver).VisualParents().FirstOrDefault(d => d == this) != null) {
					return;
				}
				e.Handled = true;
			}
		}
		protected virtual void HideToolTip() {
			ToolTip toolTip = this.GetToolTip() as ToolTip;
			if(toolTip != null) toolTip.Visibility = Visibility.Collapsed;
		}
		protected virtual void ShowToolTip() {
			ToolTip toolTip = this.GetToolTip() as ToolTip;
			if(toolTip != null)
				toolTip.Visibility = Visibility.Visible;
			else UpdateToolTip();
		}
		protected override object OnIsPressedCoerce(object value) {
			bool bValue = (bool)value;
			bool result = GetShouldShowPopupControl(bValue);
			if(bValue && !result)
				ShowHotBorder = true;
			return bValue && (result || !ActualActAsDropDown);
		}
		bool GetShouldShowPopupControl(bool expectedValue) {
			return expectedValue && !(GetPopupControl() == null || !GetPopupControl().IsPopupOpen && isOpenJustChanged);
		}
		protected override void UpdateIsPressed() {			
			if(ActualActAsDropDown || LayoutPanel.With(x=>x.ElementFirstBorderControl)==null) {
				base.UpdateIsPressed();
				return;
			}
			IsPressed = ShouldPressItem();
		}
		protected override bool ShouldPressItem() {
			if (ActualActAsDropDown || LayoutPanel.With(x=>x.ElementFirstBorderControl) == null) {				
				return base.ShouldPressItem();
			}
			Point position = MouseHelper.GetPosition(LayoutPanel.ElementFirstBorderControl);
			if (new Rect(LayoutPanel.ElementFirstBorderControl.RenderSize).Contains(position) || LayoutPanel.ElementFirstBorderControl.IsMouseOver) {
				return true;
			}
			return false;
		}
		protected override void OnIsPressedChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsPressedChanged(e);
			bool newValue = (bool)e.NewValue;
			if(CanShowPopup) {
				if(ActualActAsDropDown && ContainerType != LinkContainerType.RadialMenu) {
					if(newValue)
						ShowPopup();
					else
						ClosePopup();
				}
			}
		}
		protected override void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsSelectedChanged(e);
			var popup = GetPopupControl().With(x => x.Popup);
			if (popup == null || popup.IsOpen)
				return;
			if (IsSelected)
				BeforeOpenPopup();			
		}
		protected virtual void OnActualArrowAlignmentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateVisualStateByArrowAlignment();
		}
		protected virtual void OnActualActAsDropDownChanged() {
			UpdateLayoutPanelShowSecondBorder();
			UpdateLayoutPanelActAsDropDown();
			UpdateActualIsHoverEnabled();
		}
		protected virtual void OnShowArrowHotBorderChanged(DependencyPropertyChangedEventArgs e) {
			UpdateItemBorderAndContent();
			UpdateItemVisualState();
		}
		protected virtual void OnShowArrowPressedBorderChanged(DependencyPropertyChangedEventArgs e) {
			UpdateItemBorderAndContent();
			UpdateItemVisualState();
		}
		protected override void OnCurrentRibbonStyleChanged(RibbonItemStyles oldValue) {
			base.OnCurrentRibbonStyleChanged(oldValue);			
			if(ContainerType == LinkContainerType.RibbonPageGroup) {				
				UpdateActualArrowAlignmentInRibbonPageGroup();
			}
			UpdateLayoutPanelGlyphToContentAlignment();
		}
		protected override void OnContainerTypeChanged(LinkContainerType oldValue) {
			base.OnContainerTypeChanged(oldValue);
			UpdateActualArrowAlignment();
		}	  
		protected virtual void OnIsArrowPressedChanged(DependencyPropertyChangedEventArgs e) {
			ShowArrowHotBorder = !IsArrowPressed && !IsLinkInCustomizationMode;
			UpdateShowArrowPressedBorder();
			bool newValue = (bool)e.NewValue;			
			PerformArrowClick(newValue);
		}
		protected override bool ShouldClickOnMouseLeftButtonUp(bool wasPressed) {
			bool baseValue = base.ShouldClickOnMouseLeftButtonUp(wasPressed);
			if(ContainerType == LinkContainerType.RadialMenu)
				return baseValue;
			return baseValue && !IsArrowPressed && (LayoutPanel == null ? false : !LayoutPanel.IsMouseOverSecondBorder);
		}
		protected virtual void UpdateShowArrowPressedBorder() {
			ShowArrowPressedBorder = IsArrowPressed && !IsLinkInCustomizationMode;
			UpdateItemBorderAndContent();
		}
		protected virtual void SubscribePopupControl() {
			UnsubscribePopupControl();
			if (GetPopupControl().With(x => x.Popup) == null)
				return;
			GetPopupControl().Popup.Opening += OnPopupControlOpening;
			GetPopupControl().Popup.IsOpenChanged += OnPopupControlIsOpenChanged;			
		}		
		protected virtual void UnsubscribePopupControl() {
			if (GetPopupControl().With(x => x.Popup) == null)
				return;
			GetPopupControl().Popup.Opening -= OnPopupControlOpening;
			GetPopupControl().Popup.IsOpenChanged -= OnPopupControlIsOpenChanged;			
		}
		protected virtual void OnPopupControlOpening(object sender, CancelEventArgs e) {
			if(GetPopupControl() == null)
				return;
			GetPopupControl().Popup.OwnerLinkControl = this;
		}
		bool isOpenJustChanged = false;
		protected virtual void OnPopupControlIsOpenChanged(object sender, EventArgs e) {
			isOpenJustChanged = true;
			Dispatcher.BeginInvoke(new Action(() => isOpenJustChanged = false));
			if(!((BarPopupBase)sender).IsOpen)
				OnPopupControlClosed(sender, e);
			else
				OnPopupControlOpened(sender, e);
		}
		protected virtual void OnPopupControlClosed(object sender, EventArgs e) {
			if(GetPopupControl() == null)
				return;
			AfterClosePopup();
			UnsubscribePopupControl(); 
			var expandablePopup = GetPopupControl().Popup as BarPopupExpandable;
			if(expandablePopup != null && System.Windows.DependencyPropertyHelper.GetValueSource(expandablePopup, BarPopupExpandable.ExpandModeProperty).BaseValueSource != BaseValueSource.Local)
				expandablePopup.ClearValue(BarPopupExpandable.ExpandModeProperty);
			GetPopupControl().Popup.OwnerLinkControl = null;
			GetPopupControl().Popup.Width = SavedPopupControlWidth;
			GetPopupControl().Popup.Height = SavedPopupControlHeight;
			IsArrowPressed = IsPressed = IsHighlighted = false;
			ShowArrowHotBorder = false;
			ShowArrowPressedBorder = false;
			UpdateShowArrowHotBorderProperty();
			ShowToolTip();
		}
		protected virtual void OnPopupControlOpened(object sender, EventArgs e) {
			HideToolTip();
			if (selectFirstOnPopupOpened) {
				GetBoundOwner().Do(x => NavigationTree.SelectElement(x));
				selectFirstOnPopupOpened = false;
			}			
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			UpdateShowArrowHotBorderProperty();
			base.OnMouseMove(e);
			if (!IsPopupOpen && !IsOnBar && !IsLinkInRadialMenu && (!IsLinkInRibbon || ContainerType == LinkContainerType.DropDownGallery) || IsLinkInApplicationMenu) {
				if (!(Item as BarSplitButtonItem).ActAsDropDown) {
					if (LayoutPanel.IsMouseOverSecondBorder) {
						if (!IsLinkInCustomizationMode)
							StartOpenPopupTimer();
					} else if (!IsLinkInCustomizationMode)
						StopOpenPopupTimer();
				} else {
					if (IsMouseOver) {
						if (!IsLinkInCustomizationMode)
							StartOpenPopupTimer();
					}
				}
			}
		}
		protected void StopOpenPopupTimer() {
			PopupMenuManager.CancelPopupOpening(GetPopupControl().With(x => x.Popup));
		}
		protected void StartOpenPopupTimer() {
			if(popupExpandMode != BarPopupExpandMode.TabletOffice)
				PopupMenuManager.ShowPopup(GetPopupControl().With(x => x.Popup), false, ShowPopup);
		}
		protected void StartClosePopupTimer() {
			if (IsPopupOpen)
				PopupMenuManager.ClosePopup(GetPopupControl().With(x => x.Popup), false, ClosePopup);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			StopOpenPopupTimer();			
			if(CanShowPopup && GetPopupControl().IsPopupOpen)
				return;			
			ShowArrowHotBorder = false;
			ShowArrowPressedBorder = false;
			UpdateItemBorderAndContent();
			base.OnMouseLeave(e);
		}
		protected override void UnpressOnMouseLeave(MouseEventArgs e) {
			if(GetPopupControl() != null && GetPopupControl().IsPopupOpen) {
				StartClosePopupTimer();
				return;
			}
			base.UnpressOnMouseLeave(e);
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			if(IsLinkInApplicationMenu) 
				StartOpenPopupTimer();
			UpdateShowArrowHotBorderProperty();
		}
		protected internal override void OnMouseLeftButtonDownCore(MouseButtonEventArgs e) {
			var osPopup = (e.OriginalSource as DependencyObject).With(BarManagerHelper.GetPopup);
			var ownPopup = GetPopupControl().With(x => x.Popup);
			if (osPopup!=null && ownPopup!=null && PopupMenuManager.PopupAncestors(osPopup, true).Contains(ownPopup))
				return;
			if (CanShowPopup && ActualActAsDropDown && !IsLinkControlInMenu) {
				IsPressed = !IsPressed;
				e.Handled = true;
				if(LinksControl != null) {
					if(LinksControl.LinksHolder != null && LinksControl.LinksHolder is Bar && ((Bar)LinksControl.LinksHolder).IsMainMenu)
						return;
				}
				IsPressed = IsPopupOpen;
			}
			base.OnMouseLeftButtonDownCore(e);
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if(e.Handled)
				return;
		}
		protected override void OnLostMouseCapture(MouseEventArgs e) {
			if(CanShowPopup && GetPopupControl().IsPopupOpen)
				return;
			base.OnLostMouseCapture(e);
		}
		protected bool IsMouseInArrow(MouseEventArgs e) {
			if(LayoutPanel.ElementSecondBorderControl == null) return false;
			return IsMouseInArrow(e.GetPosition(LayoutPanel.ElementSecondBorderControl));
		}
		protected bool IsMouseInArrow(Point pos) {
			return pos.X >= 0 && pos.Y >= 0 && pos.X <= LayoutPanel.ElementSecondBorderControl.ActualWidth && pos.Y <= LayoutPanel.ElementSecondBorderControl.ActualHeight;
		}
		protected void UpdateShowArrowHotBorderProperty() {
			ShowArrowHotBorder = IsMouseOver && (LayoutPanel != null ? LayoutPanel.IsMouseOverSecondBorder : true) && !IsLinkInCustomizationMode && (Manager != null ? !PopupMenuManager.IsAnyPopupOpened : true);
		}
		protected void PerformArrowClick(bool isOpen) {
			StopOpenPopupTimer();
			if(CanShowPopup) {
				if(isOpen)
					ShowPopup();
				else
					ClosePopup();
			}
		}
		void OnArrowButtonMouseDown(object sender, MouseButtonEventArgs e) {
			if(!LayoutPanel.IsMouseOverSecondBorder) return;
			if(!ActualActAsDropDown && e.LeftButton == MouseButtonState.Pressed) {
				if(GetShouldShowPopupControl(true)) {
					  if (!CanShowPopup) {
						e.Handled = true;
						return;
					}
					IsArrowPressed = !IsArrowPressed;
					IsArrowPressed = IsPopupOpen;
					e.Handled = true;
				} else {
					ShowHotBorder = true;
				}
			}
		}
		bool selectFirstOnPopupOpened = false;
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if (e.Key == Key.Escape && !e.Handled) {
				e.Handled = ProcessKeyDown(e);
			}
		}
		protected internal override bool ProcessKeyDown(KeyEventArgs e) {
			if(CanShowPopup && !GetPopupControl().IsPopupOpen) {
				Orientation orientation = BarManagerHelper.GetLinkContainerOrientation(this);
				if (e.Key == Key.Down && IsOnBar && Orientation == Orientation.Horizontal
					|| e.Key == Key.Right && (!IsOnBar || Orientation == Orientation.Vertical) && FlowDirection == FlowDirection.LeftToRight
					|| e.Key == Key.Left && (!IsOnBar || Orientation == Orientation.Vertical) && FlowDirection == FlowDirection.RightToLeft
					) {
					selectFirstOnPopupOpened = true;
					if (ActualActAsDropDown) {
						IsPressed = true;
					} else {						
						IsArrowPressed = true;
						ShowPopup();						
					}
					return true;
				}
			}
			if (IsPopupOpen) {
				bool isLeftKey = (IsLinkControlInMenu || Orientation == Orientation.Vertical) && e.Key == (FlowDirection == FlowDirection.LeftToRight ? Key.Left : Key.Right);
				bool isCloseInBarKey = !IsLinkControlInMenu && Orientation == Orientation.Horizontal && e.Key == Key.Up;				
				if (e.Key == Key.Escape || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down) {
					using (NavigationTree.Lock()) {
						GetPopupControl().With(x => x.Popup).Do(x => PopupMenuManager.ClosePopup(x, true));
					}
					NavigationTree.SelectElement(LinkInfo);
					return isLeftKey || isCloseInBarKey || e.Key == Key.Escape;
				}
			}
			return base.ProcessKeyDown(e);
		}
		protected override void UpdateItemBorderAndContent() {
			if(LayoutPanel == null) return;
			if(ShowCustomizationBorder) {
				LayoutPanel.ShowSecondBorder = false;
				LayoutPanel.FirstBorderState = BorderState.Customization;
				LayoutPanel.BorderState = BorderState.Customization;
				return;
			}
			UpdateLayoutPanelShowSecondBorder();
			if(!IsEnabled) {
				LayoutPanel.FirstBorderState = BorderState.Disabled;
				LayoutPanel.SecondBorderState = BorderState.Disabled;
				LayoutPanel.Opacity = LayoutPanel.DisabledOpacity;
				return;
			}
			LayoutPanel.Opacity = 1.0d;
			if(ShowArrowPressedBorder) {
				LayoutPanel.IsFirstBorderActive = false;
				LayoutPanel.IsSecondBorderActive = true;
				LayoutPanel.FirstBorderState = BorderState.Pressed;
				LayoutPanel.SecondBorderState = BorderState.Pressed;
				return;
			}
			if(ShowPressedBorder || (IsChecked==true)) {
				LayoutPanel.IsFirstBorderActive = true;
				LayoutPanel.IsSecondBorderActive = ShowArrowHotBorder;
				LayoutPanel.FirstBorderState = BorderState.Pressed;
				LayoutPanel.SecondBorderState = BorderState.Hover;
				return;
			}
			if(ShowArrowHotBorder) {
				LayoutPanel.IsFirstBorderActive = false;
				LayoutPanel.IsSecondBorderActive = true;
				LayoutPanel.FirstBorderState = ShowPressedBorder ? BorderState.Pressed : BorderState.Hover;
				LayoutPanel.SecondBorderState = BorderState.Hover;
				return;
			}
			if(ShowHotBorder) {
				LayoutPanel.IsFirstBorderActive = true;
				LayoutPanel.IsSecondBorderActive = false;
				LayoutPanel.FirstBorderState = BorderState.Hover;
				LayoutPanel.SecondBorderState = BorderState.Hover;
				return;
			}
			LayoutPanel.IsFirstBorderActive = true;
			LayoutPanel.IsSecondBorderActive = IsChecked==false;
			LayoutPanel.FirstBorderState = BorderState.Normal;
			LayoutPanel.SecondBorderState = BorderState.Normal;
		}
		#region IsEnabled logic
		protected internal override void UpdateActualIsArrowEnabled() {			
			ActualIsArrowEnabled = (IsEnabled && !IsDisabledByItem()) ? ((GetPopupControl() != null) && !ActualActAsDropDown)
				: (IsDisabledByParentCommandCanExecuteOnly(true) && (GetPopupControl() != null) && !ActualActAsDropDown);
		}
		protected override void UpdateActualIsContentEnabled() {
			ActualIsContentEnabled = (IsEnabled && !IsDisabledByItem()) ? true : (IsDisabledByParentCommandCanExecuteOnly() && !ActualActAsDropDown);
			UpdateActualIsHoverEnabled();
		}
		#endregion
		protected override void UpdateItemVisualState() {
			if(ShowCustomizationBorder)
				VisualStateManager.GoToState(this, "Customization", false);
			else if(!IsEnabled)
				VisualStateManager.GoToState(this, "Disabled", false);
			else if(ShowArrowPressedBorder)
				VisualStateManager.GoToState(this, "ArrowPressed", false);
			else if(ShowPressedBorder)
				VisualStateManager.GoToState(this, "Pressed", false);
			else if(ShowArrowHotBorder)
				VisualStateManager.GoToState(this, "ArrowHot", false);
			else if(ShowHotBorder)
				VisualStateManager.GoToState(this, "Hot", false);
			else
				VisualStateManager.GoToState(this, "Normal", false);
		}
		protected internal override void UpdateStyleByContainerType(LinkContainerType type) {
			base.UpdateStyleByContainerType(type);
			switch(type) {
				case LinkContainerType.Bar:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInBarProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyle);
					break;
				case LinkContainerType.DropDownGallery:
				case LinkContainerType.Menu:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInMenuProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyleInMainMenu);
					break;
				case LinkContainerType.MainMenu:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInMainMenuProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyleInMainMenu);
					break;
				case LinkContainerType.StatusBar:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInStatusBarProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyleInStatusBar);
					break;
				case LinkContainerType.RibbonQuickAccessToolbar:
				case LinkContainerType.RibbonQuickAccessToolbarFooter:
				case LinkContainerType.RibbonPageHeader:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInQuickAccessToolbarProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyleInRibbonToolbar);
					break;
				case LinkContainerType.RibbonStatusBarLeft:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInRibbonStatusBarLeftProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyleInRibbonStatusBarLeft);
					break;
				case LinkContainerType.RibbonStatusBarRight:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInRibbonStatusBarRightProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyleInRibbonStatusBarRight);
					break;
				case LinkContainerType.RibbonPageGroup:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInRibbonPageGroupProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyleInRibbonPageGroup);
					break;
				case LinkContainerType.BarButtonGroup:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInButtonGroupProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyleInBarButtonGroup);
					break;
				default:
					ContentAndArrowLayoutPanelStyle = (Style)GetTemplateFromProvider(BarSplitButtonItemLinkControlTemplateProvider.ArrowLayoutPanelStyleInBarProperty, BarSplitButtonItemThemeKeys.ArrowLayoutPanelStyle);
					break;
			}
		}
		protected internal virtual void UpdateVisualStateByArrowAlignment() {
			UpdateLayoutPanelArrowAlignment();
		}
		protected virtual void UpdateActualArrowAlignmentInRibbonPageGroup() {
			if(CurrentRibbonStyle == RibbonItemStyles.Large) {
				ActualArrowAlignment = Dock.Bottom;
				ShowContentInArrow = true;
			}
			else {
				ActualArrowAlignment = Dock.Right;
				ShowContentInArrow = false;
			}
		}
		protected internal virtual void UpdateActualItemClickBehaviour() {
			if (SplitButtonLink != null && SplitButtonLink.ItemClickBehaviour != PopupItemClickBehaviour.Undefined) {
				ItemClickBehaviour = SplitButtonLink.ItemClickBehaviour;
				return;
			}
			if (SplitButtonItem != null && SplitButtonItem.ItemClickBehaviour != PopupItemClickBehaviour.Undefined) {
				ItemClickBehaviour = SplitButtonItem.ItemClickBehaviour;
				return;
			}		  
		}
		protected internal virtual void UpdateActualArrowAlignment() {
			if(IsLinkControlInMenu || IsLinkInApplicationMenu) {
				ActualArrowAlignment = Dock.Right;
			}
			if(ContainerType == LinkContainerType.RibbonPageGroup) {
				UpdateActualArrowAlignmentInRibbonPageGroup();
				return;
			}
			ActualArrowAlignment = GetArrowAlignment();			
		}
		protected virtual Dock GetArrowAlignment() {
			if(SplitButtonLink != null && SplitButtonLink.UserArrowAlignment != null)
				return (Dock)SplitButtonLink.UserArrowAlignment.Value;
			if(SplitButtonItem != null)
				return SplitButtonItem.ArrowAlignment;
			return Dock.Right;
		}
		protected override void UpdateActualShowArrow() {
			ActualShowArrow = true;
		}
		protected virtual void UpdateActualItemPosition() {
			switch(ItemPosition) {
				case HorizontalItemPositionType.Single:
					ContentPartItemPosition = HorizontalItemPositionType.Left;
					ArrowPartItemPosition = HorizontalItemPositionType.Right;
					break;
				case HorizontalItemPositionType.Center:
					ContentPartItemPosition = HorizontalItemPositionType.Center;
					ArrowPartItemPosition = HorizontalItemPositionType.Center;
					break;
				case HorizontalItemPositionType.Left:
					ContentPartItemPosition = HorizontalItemPositionType.Left;
					ArrowPartItemPosition = HorizontalItemPositionType.Center;
					break;
				case HorizontalItemPositionType.Right:
					ContentPartItemPosition = HorizontalItemPositionType.Center;
					ArrowPartItemPosition = HorizontalItemPositionType.Right;
					break;
			}
			UpdateLayoutPanelHorizontalItemPosition();
		}
		BarPopupExpandMode popupExpandMode;
		public override void SetExpandMode(BarPopupExpandMode expandMode) {
			popupExpandMode = expandMode;
		}
		protected void SetItemBorderActivity(bool value) {
		}
		protected virtual void SetItemBorderState(BorderState state) {
		}
		protected virtual void SetArrowBorderActivity(bool value) {
		}
		protected virtual void SetArrowBorderState(BorderState state) {
		}
		protected virtual void SetControlBorderState(BorderState state) {
		}
		protected virtual void SetControlBorderActivity(bool isActive) {
		}
		protected internal virtual void OnSourceActAsDropDownChanged() {
			UpdateActualActAsDropDown();
		}
		protected virtual void UpdateActualActAsDropDown() {
			ActualActAsDropDown = GetActAsDropDown();
			UpdateVisualStateByArrowAlignment();			
		}
		protected virtual bool GetActAsDropDown() {			
			if(Item is BarSplitButtonItem)
				return ((BarSplitButtonItem)Item).ActAsDropDown;
			return false;
		}
		protected internal override void UpdateActualProperties() {
			base.UpdateActualProperties();
			UpdateActualActAsDropDown();
			UpdateActualIsHoverEnabled();
			UpdateActualArrowAlignment();
			UpdateActualItemClickBehaviour();
		}		
		double SavedPopupControlWidth { get; set; }
		double SavedPopupControlHeight { get; set; }
		bool IPopupOwner.ActAsDropdown {
			get { return ActualActAsDropDown; }
		}
		IPopupControl IPopupOwner.Popup {
			get { return GetPopupControl(); }
		}
		protected internal override INavigationOwner GetBoundOwner() {
			var currentPopup = GetPopupControl().With(x => x.Popup);
			var popupMenu = (currentPopup as PopupMenu).With(x => x.ContentControl);
			var boundOwner = (INavigationOwner)LayoutHelper.FindElement(currentPopup.With(x => x.PopupContent as FrameworkElement), x => x is INavigationOwner);
			return popupMenu ?? boundOwner;
		}
	}
	public class BarSplitButtonItemLinkControlTemplateProvider : DependencyObject {
		public static readonly DependencyProperty ArrowLayoutPanelStyleInBarProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInBar", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInMainMenu", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInStatusBar", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInMenu", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInRibbonPageGroup", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInButtonGroup", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInQuickAccessToolbar", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInQuickAccessToolbarFooterProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInQuickAccessToolbarFooter", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInRibbonPageHeader", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInRibbonStatusBarLeft", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ArrowLayoutPanelStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("ArrowLayoutPanelStyleInRibbonStatusBarRight", typeof(Style), typeof(BarSplitButtonItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static Style GetArrowLayoutPanelStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInRibbonStatusBarRightProperty);
		}
		public static void SetArrowLayoutPanelStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInRibbonStatusBarLeftProperty);
		}
		public static void SetArrowLayoutPanelStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInRibbonStatusBarLeftProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInRibbonPageHeaderProperty);
		}
		public static void SetArrowLayoutPanelStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInQuickAccessToolbarFooter(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInQuickAccessToolbarFooterProperty);
		}
		public static void SetArrowLayoutPanelStyleInQuickAccessToolbarFooter(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInQuickAccessToolbarFooterProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInQuickAccessToolbarProperty);
		}
		public static void SetArrowLayoutPanelStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInButtonGroupProperty);
		}
		public static void SetArrowLayoutPanelStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInButtonGroupProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInRibbonPageGroupProperty);
		}
		public static void SetArrowLayoutPanelStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInMenuProperty);
		}
		public static void SetArrowLayoutPanelStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInMenuProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInStatusBarProperty);
		}
		public static void SetArrowLayoutPanelStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInStatusBarProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInMainMenuProperty);
		}
		public static void SetArrowLayoutPanelStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInMainMenuProperty, value);
		}
		public static Style GetArrowLayoutPanelStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(ArrowLayoutPanelStyleInBarProperty);
		}
		public static void SetArrowLayoutPanelStyleInBar(DependencyObject target, Style value) {
			target.SetValue(ArrowLayoutPanelStyleInBarProperty, value);
		}
	}
}

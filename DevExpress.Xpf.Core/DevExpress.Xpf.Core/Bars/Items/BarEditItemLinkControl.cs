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
using System.Text;
using DevExpress.Xpf.Editors;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Bars.Themes;
using System.Windows.Controls;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Bars {
	public class BarEditItemLinkControl : BarItemLinkControl {
		#region static
		static readonly DependencyPropertyKey EditPropertyKey;
		public static readonly DependencyProperty EditProperty;
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleProperty;
		public static readonly DependencyProperty NormalEditStyleProperty;
		public static readonly DependencyProperty HotEditStyleProperty;
		public static readonly DependencyProperty PressedEditStyleProperty;
		public static readonly DependencyProperty DisabledEditStyleProperty;
		public static readonly DependencyProperty ActualHorizontalEditAlignmentProperty;
		protected static readonly DependencyPropertyKey ActualHorizontalEditAlignmentPropertyKey;
		public static readonly DependencyProperty ActualEditWidthProperty;
		protected static readonly DependencyPropertyKey ActualEditWidthPropertyKey;
		public static readonly DependencyProperty ActualEditHeightProperty;
		protected static readonly DependencyPropertyKey ActualEditHeightPropertyKey;		
		public static readonly DependencyProperty ActualContent2Property;
		protected static readonly DependencyPropertyKey ActualContent2PropertyKey;
		public static readonly DependencyProperty ActualContent2TemplateProperty;
		protected static readonly DependencyPropertyKey ActualContent2TemplatePropertyKey;
		public static readonly DependencyProperty ActualContent2VisibilityProperty;
		protected static readonly DependencyPropertyKey ActualContent2VisibilityPropertyKey;
		public static readonly DependencyProperty ActualEditStyleProperty;
		protected static readonly DependencyPropertyKey ActualEditStylePropertyKey;
		public static readonly DependencyProperty EditContentMarginProperty;
		public static readonly DependencyProperty InRibbonEditContentMarginProperty;
		public static readonly DependencyProperty ActualEditTemplateProperty;
		protected static readonly DependencyPropertyKey ActualEditTemplatePropertyKey;
		public static readonly DependencyProperty ShowInVerticalBarProperty;
		static BarEditItemLinkControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarEditItemLinkControl), typeof(BarEditItemLinkControlAutomationPeer), owner => new BarEditItemLinkControlAutomationPeer((BarEditItemLinkControl)owner));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(typeof(BarEditItemLinkControl)));
			EditPropertyKey = DependencyPropertyManager.RegisterReadOnly("Edit", typeof(BaseEdit), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnEditPropertyChanged)));
			EditProperty = EditPropertyKey.DependencyProperty;
			EditAndContentLayoutPanelStyleProperty = DependencyPropertyManager.Register("EditAndContentLayoutPanelStyle", typeof(Style), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			NormalEditStyleProperty = DependencyPropertyManager.Register("NormalEditStyle", typeof(Style), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			HotEditStyleProperty = DependencyPropertyManager.Register("HotEditStyle", typeof(Style), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			PressedEditStyleProperty = DependencyPropertyManager.Register("PressedEditStyle", typeof(Style), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			DisabledEditStyleProperty = DependencyPropertyManager.Register("DisabledEditStyle", typeof(Style), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ActualHorizontalEditAlignmentPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHorizontalEditAlignment", typeof(HorizontalAlignment), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch, (d, e) => ((BarEditItemLinkControl)d).UpdateLayoutPanelAdditionalContentHorizontalAlignment()));
			ActualHorizontalEditAlignmentProperty = ActualHorizontalEditAlignmentPropertyKey.DependencyProperty;
			ActualEditWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualEditWidth", typeof(double), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(0.0, (d, e) => ((BarEditItemLinkControl)d).UpdateLayoutPanelEditWidth()));
			ActualEditWidthProperty = ActualEditWidthPropertyKey.DependencyProperty;
			ActualEditHeightPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualEditHeight", typeof(double), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(double.NaN, (d,e)=>((BarEditItemLinkControl)d).UpdateLayoutPanelEditHeight()));
			ActualEditHeightProperty = ActualEditHeightPropertyKey.DependencyProperty;
			ActualContent2PropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContent2", typeof(object), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, (d, e) => ((BarEditItemLinkControl)d).UpdateLayoutPanelContent2()));
			ActualContent2Property = ActualContent2PropertyKey.DependencyProperty;
			ActualContent2TemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContent2Template", typeof(DataTemplate), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null));
			ActualContent2TemplateProperty = ActualContent2TemplatePropertyKey.DependencyProperty;
			ActualContent2VisibilityPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualContent2Visibility", typeof(Visibility), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(Visibility.Collapsed));
			ActualContent2VisibilityProperty = ActualContent2VisibilityPropertyKey.DependencyProperty;
			ActualEditStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualEditStyle", typeof(Style), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, (d,e)=>((BarEditItemLinkControl)d).UpdateLayoutPanelAdditionalContentStyle()));
			ActualEditStyleProperty = ActualEditStylePropertyKey.DependencyProperty;
			EditContentMarginProperty = DependencyPropertyManager.Register("EditContentMargin", typeof(Thickness), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnEditContentMarginPropertyChanged)));
			InRibbonEditContentMarginProperty = DependencyPropertyManager.Register("InRibbonEditContentMargin", typeof(Thickness), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(new Thickness(0), new PropertyChangedCallback(OnInRibbonEditContentMarginPropertyChanged)));
			ActualEditTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualEditTemplate", typeof(DataTemplate), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(null, (d,e)=>((BarEditItemLinkControl)d).UpdateLayoutPanelAdditionalContentTemplate()));
			ActualEditTemplateProperty = ActualEditTemplatePropertyKey.DependencyProperty;
			ShowInVerticalBarProperty = DependencyPropertyManager.Register("ShowInVerticalBar", typeof(DevExpress.Utils.DefaultBoolean), typeof(BarEditItemLinkControl), new FrameworkPropertyMetadata(DevExpress.Utils.DefaultBoolean.Default));
		}
		protected static void OnEditContentMarginPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItemLinkControl)obj).OnEditContentMarginPropertyChanged(e);
		}
		protected static void OnInRibbonEditContentMarginPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((BarEditItemLinkControl)d).OnInRibbonEditContentMarginChanged((Thickness)e.OldValue);
		}
		protected static void OnEditPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((BarEditItemLinkControl)obj).OnBaseEditChanged(e);
		}
		#endregion
		#region dep props
		public HorizontalAlignment ActualHorizontalEditAlignment {
			get { return (HorizontalAlignment)GetValue(ActualHorizontalEditAlignmentProperty); }
			protected set { this.SetValue(ActualHorizontalEditAlignmentPropertyKey, value); }
		}
		public double ActualEditWidth {
			get { return (double)GetValue(ActualEditWidthProperty); }
			protected set { this.SetValue(ActualEditWidthPropertyKey, value); }
		}
		public double ActualEditHeight {
			get { return (double)GetValue(ActualEditHeightProperty); }
			protected set { this.SetValue(ActualEditHeightPropertyKey, value); }
		}
		public object ActualContent2 {
			get { return (object)GetValue(ActualContent2Property); }
			protected set { this.SetValue(ActualContent2PropertyKey, value); }
		}
		public DataTemplate ActualContent2Template {
			get { return (DataTemplate)GetValue(ActualContent2TemplateProperty); }
			protected set { this.SetValue(ActualContent2TemplatePropertyKey, value); }
		}
		public Visibility ActualContent2Visibility {
			get { return (Visibility)GetValue(ActualContent2VisibilityProperty); }
			protected set { this.SetValue(ActualContent2VisibilityPropertyKey, value); }
		}
		public Style ActualEditStyle {
			get { return (Style)GetValue(ActualEditStyleProperty); }
			protected set { this.SetValue(ActualEditStylePropertyKey, value); }
		}
		public Style EditAndContentLayoutPanelStyle {
			get { return (Style)GetValue(EditAndContentLayoutPanelStyleProperty); }
			set { SetValue(EditAndContentLayoutPanelStyleProperty, value); }
		}
		public Style NormalEditStyle {
			get { return (Style)GetValue(NormalEditStyleProperty); }
			set { SetValue(NormalEditStyleProperty, value); }
		}
		public Style HotEditStyle {
			get { return (Style)GetValue(HotEditStyleProperty); }
			set { SetValue(HotEditStyleProperty, value); }
		}
		public Style PressedEditStyle {
			get { return (Style)GetValue(PressedEditStyleProperty); }
			set { SetValue(PressedEditStyleProperty, value); }
		}
		public Style DisabledEditStyle {
			get { return (Style)GetValue(DisabledEditStyleProperty); }
			set { SetValue(DisabledEditStyleProperty, value); }
		}
		public DataTemplate ActualEditTemplate {
			get { return (DataTemplate)GetValue(ActualEditTemplateProperty); }
			protected set { this.SetValue(ActualEditTemplatePropertyKey, value); }
		}
		public Thickness EditContentMargin {
			get { return (Thickness)GetValue(EditContentMarginProperty); }
			set { SetValue(EditContentMarginProperty, value); }
		}
		public DevExpress.Utils.DefaultBoolean ShowInVerticalBar {
			get { return (DevExpress.Utils.DefaultBoolean)GetValue(ShowInVerticalBarProperty); }
			set { SetValue(ShowInVerticalBarProperty, value); }
		}
		public Thickness InRibbonEditContentMargin {
			get { return (Thickness)GetValue(InRibbonEditContentMarginProperty); }
			set { SetValue(InRibbonEditContentMarginProperty, value); }
		}
		#endregion
		public bool IsEditFocused { get; private set; }
		public BarEditItemLinkControl() : base() {
			IsEditFocused = false;
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			var ownPopup = BarManagerHelper.GetPopup(this);
			var curPopup = (NavigationTree.CurrentElement as BarItemLinkInfo).With(x => x.LinkControl).If(x=>x is IPopupOwner).With(BarManagerHelper.GetPopup);
			if (ownPopup != null && Equals(ownPopup, curPopup))
				((INavigationElement)NavigationTree.CurrentElement).IsSelected = false;
		}
		public BarEditItemLinkControl(BarItemLink link) : base(link) {
			IsEditFocused = false;
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override RibbonItemStyles CalcRibbonStyleInPageGroup() {
			return CalcRibbonStyleInQAT();
		}
		protected override RibbonItemStyles CalcRibbonStyleInQAT() {
			if (Link.ActualRibbonStyle == RibbonItemStyles.SmallWithoutText)
				return RibbonItemStyles.SmallWithoutText;
			return RibbonItemStyles.SmallWithText;
		}
		protected override RibbonItemStyles  CalcRibbonStyleInStatusBar() {
			return CalcRibbonStyleInQAT();
		}
		protected override RibbonItemStyles CalcRibbonStyleInButtonGroup() {
			return RibbonItemStyles.SmallWithoutText;
		}
		#region actual properties updaters
		protected internal virtual void OnSourceHorizontalEditAlignmentChanged() {
			UpdateActualHorizontalEditAlignment();
		}
		protected internal virtual void OnSourceEditWidthChanged() {
			UpdateActualEditWidth();
		}
		protected internal virtual void OnSourceEditHeightChanged() {
			UpdateActualEditHeight();
		}
		protected internal virtual void OnSourceContent2Changed() {
			UpdateActualContent2();
		}
		protected internal virtual void OnSourceEditStyleChanged() {
			UpdateActualEditStyle();
		}
		protected internal virtual void OnSourceContent2TemplateChanged() {
			UpdateActualContent2Template();
		}
		protected internal virtual void UpdateActualHorizontalEditAlignment() {
			ActualHorizontalEditAlignment = GetHorizontalEditAlignment();
			if(Edit!=null) {
				DevExpress.Xpf.Bars.Helpers.BarLayoutHelper.InvalidateMeasureTree(Edit, this);
			}
		}
		protected virtual void UpdateActualEditWidth() {
			var value = GetEditWidth();
			ActualEditWidth = value.HasValue ? value.Value : double.NaN;
		}
		protected virtual void UpdateActualEditHeight() {
			var value = GetEditHeight();
			ActualEditHeight = value.HasValue ? value.Value : double.NaN;
		}
		protected virtual void UpdateActualContent2() {
			ActualContent2 = GetContent2();
			UpdateActualContent2Visibility();
		}
		protected virtual void UpdateActualEditStyle() {
			ActualEditStyle = GetEditStyle();
			UpdateEdit();
		}
		protected virtual void UpdateActualContent2Template() {
			ActualContent2Template = GetContent2Template();
			UpdateActualContent2Visibility();
		}
		protected virtual void UpdateActualContent2Visibility() {
			ActualContent2Visibility = ActualContent2 == null && ActualContent2Template == null ? Visibility.Collapsed : Visibility.Visible;
			UpdateLayoutPanelShowContent2();
		}
		protected virtual HorizontalAlignment GetHorizontalEditAlignment() {
			HorizontalAlignment? actualAlignmentCandidate = EditLink != null ? EditLink.EditHorizontalAlignment : null;
			if(actualAlignmentCandidate == null)
				actualAlignmentCandidate = EditItem != null ? EditItem.EditHorizontalAlignment : null;
			if(actualAlignmentCandidate != null)
				return ((HorizontalAlignment)actualAlignmentCandidate);
			if(ContainerType == LinkContainerType.RibbonPageGroup) {
				return HorizontalAlignment.Right;
			} else {
				return HorizontalAlignment.Stretch;
			}
		}
		protected virtual double? GetEditWidth() {
			double? res = 0;
			if(Item is BarEditItem) res = ((BarEditItem)Item).EditWidth;
			if(EditLink != null)
				res = EditLink.ActualEditWidth;
			return res;
		}
		protected virtual double? GetEditHeight() {
			double? res = 0;
			if(Item is BarEditItem) res = ((BarEditItem)Item).EditHeight;
			if(EditLink != null)
				res = EditLink.ActualEditHeight;
			return res;
		}
		protected virtual object GetContent2() {			
			if(EditLink != null && EditLink.UserContent2 != null)
				return EditLink.UserContent2;
			if(Item is BarEditItem)
				return ((BarEditItem)Item).Content2;
			return null;
		}
		protected virtual DataTemplate GetContent2Template() {
			if(Item is BarEditItem)
				return ((BarEditItem)Item).Content2Template;
			return null;
		}
		protected virtual Style GetEditStyle() {			
			if(EditLink != null && EditLink.EditStyle != null)
				return EditLink.EditStyle;
			if(Item is BarEditItem) return ((BarEditItem)Item).EditStyle;
			return null;
		}
		#endregion
		protected internal override void UpdateActualProperties() {
			base.UpdateActualProperties();
			UpdateActualHorizontalEditAlignment();
			UpdateEditIsReadOnlyState();
			UpdateActualContent2();
			UpdateActualContent2Template();
			UpdateActualEditWidth();
			UpdateActualEditHeight();
			UpdateActualEditStyle();
			UpdateActualEditTemplate();
			UpdateShowInVerticalBar();
		}
		protected internal override object OnShowHotBorderCoerce(object obj) {
			return false;
		}
		public BaseEdit Edit {
			get { return (BaseEdit)GetValue(EditProperty); }
			private set { this.SetValue(EditPropertyKey, value); }
		}
		public BarEditItemLink EditLink { get { return Link as BarEditItemLink; } }
		private BaseEdit templatedEditCore = null;
		public BaseEdit TemplatedEdit {
			get { return templatedEditCore; }
			set {
				if(templatedEditCore == value) return;
				BaseEdit oldValue = templatedEditCore;
				templatedEditCore = value;
				OnTemplatedEditChanged(oldValue);
			}
		}
		internal object GetActualEditValue() {
			if(TemplatedEdit != null)
				return TemplatedEdit.EditValue;
			if(Edit != null)
				return Edit.EditValue;
			return null;
		}
		protected virtual void OnTemplatedEditChanged(BaseEdit oldValue) {
		}		
		public BarEditItem EditItem { get { return Item as BarEditItem; } }
		public void OnItemChanged(BarEditItem oldValue, BarEditItem newValue) {
			CreateEdit();
		}
		protected virtual void CreateEdit() {
			if(Edit != null)
				UnsubscribeEditEvents();
			if(LinkInfo == null || EditItem == null) return;
			if(EditItem.EditSettings == null) {
				Edit = null;
			} else {
				Edit = EditItem.EditSettings.CreateEditor(EmptyDefaultEditorViewInfo.Instance) as BaseEdit;				
			}
			if(Edit != null) 
				SubscribeEditEvents();
		}
		public void RecreateEdit() {
			CreateEdit();
			UpdateEdit();
			UpdateLayoutPanelAdditionalContentCommon();
		}
		protected virtual void UnsubscribeEditEvents() {
			Edit.EditValueChanged -= new EditValueChangedEventHandler(OnEditValueChanged);
			Edit.GotFocus -= new RoutedEventHandler(OnEditGotFocus);
			Edit.LostFocus -= new RoutedEventHandler(OnEditLostFocus);
		}
		protected virtual void SubscribeEditEvents() {
			Edit.EditValueChanged += new EditValueChangedEventHandler(OnEditValueChanged);
			Edit.GotFocus += new RoutedEventHandler(OnEditGotFocus);
			Edit.LostFocus += new RoutedEventHandler(OnEditLostFocus);
		}
		protected virtual void SubscribeEditCoreEvents(FrameworkElement editCore) {
			if (editCore == null)
				return;
			UnsubscribeEditCoreEvents(editCore);			
		}
		protected virtual void UnsubscribeEditCoreEvents(FrameworkElement editCore) {
			if (editCore == null)
				return;			
		}
		protected virtual void UnsubscribeTemplatedEditEvents() {
			TemplatedEdit.EditValueChanged -= new EditValueChangedEventHandler(OnTemplatedEditValueChanged);
			TemplatedEdit.GotFocus -= new RoutedEventHandler(OnTemplatedEditGotFocus);
			TemplatedEdit.LostFocus -= new RoutedEventHandler(OnTemplatedEditLostFocus);
		}
		protected virtual void SubscribeTemplatedEditEvents() {
			TemplatedEdit.EditValueChanged += new EditValueChangedEventHandler(OnTemplatedEditValueChanged);
			TemplatedEdit.GotFocus += new RoutedEventHandler(OnTemplatedEditGotFocus);
			TemplatedEdit.LostFocus += new RoutedEventHandler(OnTemplatedEditLostFocus);			
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			if ((bool)e.NewValue)
				SubscribeEditCoreEvents((TemplatedEdit ?? Edit).With(x => x.EditCore));
			else
				UnsubscribeEditCoreEvents((TemplatedEdit ?? Edit).With(x => x.EditCore));
			base.OnIsKeyboardFocusWithinChanged(e);
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);			
		}
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			this.If(x => !(x is IPopupOwner)).With(x => BarManagerHelper.GetPopup(x)).Do(x => PopupMenuManager.CloseChildren(x, true));
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			RecreateEdit();
			base.OnLoaded(sender, e);			
			if(TemplatedEdit != null) {
				UnsubscribeTemplatedEditEvents();
				SubscribeTemplatedEditEvents();
			}
			UpdateLayoutPanelAdditionalContent();
		}		
		protected virtual void OnEditGotFocus(object sender, RoutedEventArgs e) {
			IsEditFocused = true;
		}
		internal bool focusEditorOnIsSelected = true;
		protected internal override bool CanSelectOnHoverInMenuMode {get {return false;}}
		protected override void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e) {
				base.OnIsSelectedChanged(e);
			if(!IsSelected) {							
				Dispatcher.BeginInvoke(new Action(OnAfterIsSelectedChanged));
			}
		}
		protected override bool SetFocus() {
			if (Edit != null && Edit.CanAcceptFocus && focusEditorOnIsSelected && !IsEditFocused) {
				return (IsEditFocused = Edit.Focus());
			}
			return false;
		}
		protected virtual void OnAfterIsSelectedChanged() {
			IsEditFocused = false;
		}
		protected virtual void OnTemplatedEditGotFocus(object sender, RoutedEventArgs e) {
			IsEditFocused = true;
		}
		protected virtual void OnEditLostFocus(object sender, RoutedEventArgs e) {
			IsEditFocused = false;
		}
		protected virtual void OnTemplatedEditLostFocus(object sender, RoutedEventArgs e) {
			IsEditFocused = false;
		}
		protected internal void UpdateEdit() {
			if(Edit == null || EditItem == null) return;
			SyncEditValue();
			if(ActualEditStyle != null) Edit.Style = ActualEditStyle;
			if(Manager != null && EditLink != null && !EditLink.IsPrivate)
				Edit.IsHitTestVisible = !BarManagerCustomizationHelper.IsInCustomizationMode(this);
			UpdateEditIsReadOnlyState();
		}
		protected internal void UpdateActualEditTemplate() {
			if(EditLink != null && EditLink.EditItem != null)
				ActualEditTemplate = EditLink.EditItem.EditTemplate;
		}
		protected virtual void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
			OnEditValueChangedCore(Edit, sender, e);
		}
		protected virtual void OnTemplatedEditValueChanged(object sender, EditValueChangedEventArgs e) {
			OnEditValueChangedCore(TemplatedEdit, sender, e);
		}
		protected virtual void OnEditValueChangedCore(BaseEdit edit, object sender, EditValueChangedEventArgs e) {
			if (IsLockEditValueChanged || EditItem == null)
				return;
			EditItem.SyncEditValue(this);
			if (EditItem == null)
				return;
			if (EditItem.ClosePopupOnChangingEditValue) {
				if (NavigationTree.CurrentElement != null && NavigationTree.CurrentElement != this.LinkInfo)
					return;
				SubMenuBarControl sm = LinksControl as SubMenuBarControl;
				if (sm == null)
					return;
				BarPopupBase menu = sm.Popup as PopupMenuBase;
				if (menu == null)
					return;
				menu = PopupMenuManager.PopupAncestors(menu, true).LastOrDefault() ?? menu;
				if (menu != null)
					menu.ClosePopup();
			}
		}
		protected virtual void OnBaseEditChanged(DependencyPropertyChangedEventArgs e) {
			if(Item != null && !Item.IsPropertySet(BarEditItem.EditValueProperty)) {
				SyncEditValueFromEdit();
			}
			else {
				SyncEditValue();
			}
			UpdateLayoutPanelAdditionalContent();
		}
		protected virtual void OnEditContentMarginPropertyChanged(DependencyPropertyChangedEventArgs e) {
			UpdateEditContentMargin();
		}
		protected virtual void OnInRibbonEditContentMarginChanged(Thickness oldValue) {
			UpdateEditContentMargin();
		}
		public void UpdateEditContentMargin() {
			if(this.GetElementByName("PART_EditContent") == null)
				return;
			if(ActualContent == null)
				((ContentControl)this.GetElementByName("PART_EditContent")).Margin = IsLinkInRibbon ? InRibbonEditContentMargin : EditContentMargin;
			else
				((ContentControl)this.GetElementByName("PART_EditContent")).Margin = new Thickness(0);
		}
		readonly Locker editValueChangedLocker = new Locker();
		protected Locker EditValueChangedLocker { get { return editValueChangedLocker; } }
		protected bool IsLockEditValueChanged { get { return editValueChangedLocker.IsLocked; } }
		protected internal virtual void SyncEditValue() {
			using (EditValueChangedLocker.Lock()) {
				var edit = TemplatedEdit ?? Edit;
				var item = EditItem;
				if (edit == null || item == null)
					return;
				var vSource = System.Windows.DependencyPropertyHelper.GetValueSource(item, BarEditItem.EditValueProperty);
				if (vSource.BaseValueSource == BaseValueSource.Default && !vSource.IsCurrent)
					return;
				edit.SetCurrentValue(BaseEdit.EditValueProperty, item.EditValue);
			}
		}
		protected virtual void SyncEditValueFromEdit() {
			using (EditValueChangedLocker.Lock()) {
				var edit = TemplatedEdit ?? Edit;
				var item = EditItem;
				if (edit == null || item == null)
					return;
				var vSource = System.Windows.DependencyPropertyHelper.GetValueSource(edit, BaseEdit.EditValueProperty);
				if (vSource.BaseValueSource == BaseValueSource.Default && !vSource.IsCurrent)
					return;
				item.SetCurrentValue(BarEditItem.EditValueProperty, edit.EditValue);
			}
		}
		protected internal override void OnCustomizationModeChanged() {
			base.OnCustomizationModeChanged();
			UpdateEdit();
		}
		protected internal virtual void UpdateEditIsReadOnlyState() {
			if(Edit != null && EditItem != null)
				Edit.IsReadOnly = EditItem.IsReadOnly;
		}
		protected internal virtual void UpdateShowInVerticalBar() {
			DevExpress.Utils.DefaultBoolean itemValue = EditItem != null ? EditItem.ShowInVerticalBar : DevExpress.Utils.DefaultBoolean.Default;
			DevExpress.Utils.DefaultBoolean linkValue = EditLink != null ? EditLink.ShowInVerticalBar : DevExpress.Utils.DefaultBoolean.Default;
			if (linkValue != DevExpress.Utils.DefaultBoolean.Default) {
				ShowInVerticalBar = linkValue;
				return;
			}
			if (itemValue != DevExpress.Utils.DefaultBoolean.Default) {
				ShowInVerticalBar = itemValue;
				return;
			}
			ShowInVerticalBar = DevExpress.Utils.DefaultBoolean.False;
		}
		protected object GetTemplateFromProvider(DependencyProperty prop, BarEditItemThemeKeys themeKey) {
			object res = GetCustomResources() == null ? null : GetCustomResources()[ResourceHelper.CorrectResourceKey(new BarEditItemThemeKeyExtension() { ResourceKey = themeKey })];
			if (res != null)
				return res;
			return GetValue(prop);
		}
		protected internal override void UpdateStyleByContainerType(LinkContainerType type) {
			base.UpdateStyleByContainerType(type);
			switch(type) { 
				case LinkContainerType.Bar:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInBarProperty, BarEditItemThemeKeys.LayoutPanelStyle);
					NormalEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.NormalEditStyleInBarProperty, BarEditItemThemeKeys.NormalEditStyle);
					HotEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.HotEditStyleInBarProperty, BarEditItemThemeKeys.HotEditStyle);
					PressedEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.PressedEditStyleInBarProperty, BarEditItemThemeKeys.PressedEditStyle);
					DisabledEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.DisabledEditStyleInBarProperty, BarEditItemThemeKeys.DisabledEditStyle);
					break;
				case LinkContainerType.Menu:
				case LinkContainerType.ApplicationMenu:
				case LinkContainerType.DropDownGallery:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInMenuProperty, BarEditItemThemeKeys.LayoutPanelStyleInMenu);
					NormalEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.NormalEditStyleInMenuProperty, BarEditItemThemeKeys.NormalEditStyleInMenu);
					HotEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.HotEditStyleInMenuProperty, BarEditItemThemeKeys.HotEditStyleInMenu);
					PressedEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.PressedEditStyleInMenuProperty, BarEditItemThemeKeys.PressedEditStyleInMenu);
					DisabledEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.DisabledEditStyleInMenuProperty, BarEditItemThemeKeys.DisabledEditStyleInMenu);
					break;
				case LinkContainerType.MainMenu:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInMainMenuProperty, BarEditItemThemeKeys.LayoutPanelStyleInMainMenu);
					NormalEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.NormalEditStyleInMainMenuProperty, BarEditItemThemeKeys.NormalEditStyleInMainMenu);
					HotEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.HotEditStyleInMainMenuProperty, BarEditItemThemeKeys.HotEditStyleInMainMenu);
					PressedEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.PressedEditStyleInMainMenuProperty, BarEditItemThemeKeys.PressedEditStyleInMainMenu);
					DisabledEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.DisabledEditStyleInMainMenuProperty, BarEditItemThemeKeys.DisabledEditStyleInMainMenu);
					break;
				case LinkContainerType.StatusBar:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInStatusBarProperty, BarEditItemThemeKeys.LayoutPanelStyleInStatusBar);
					NormalEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.NormalEditStyleInStatusBarProperty, BarEditItemThemeKeys.NormalEditStyleInStatusBar);
					HotEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.HotEditStyleInStatusBarProperty, BarEditItemThemeKeys.HotEditStyleInStatusBar);
					PressedEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.PressedEditStyleInStatusBarProperty, BarEditItemThemeKeys.PressedEditStyleInStatusBar);
					DisabledEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.DisabledEditStyleInStatusBarProperty, BarEditItemThemeKeys.DisabledEditStyleInStatusBar);
					break;
				case LinkContainerType.RibbonQuickAccessToolbar:
				case LinkContainerType.RibbonQuickAccessToolbarFooter:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInQuickAccessToolbarProperty, BarEditItemThemeKeys.LayoutPanelStyleInRibbonToolbar);
					break;
				case LinkContainerType.RibbonPageHeader:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInRibbonPageHeaderProperty, BarEditItemThemeKeys.LayoutPanelStyleInRibbonPageHeader);
					break;
				case LinkContainerType.BarButtonGroup:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInButtonGroupProperty, BarEditItemThemeKeys.LayoutPanelStyleInButtonGroup);
					NormalEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.NormalEditStyleInButtonGroupProperty, BarEditItemThemeKeys.NormalEditStyleInButtonGroup);
					HotEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.HotEditStyleInButtonGroupProperty, BarEditItemThemeKeys.HotEditStyleInButtonGroup);
					PressedEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.PressedEditStyleInButtonGroupProperty, BarEditItemThemeKeys.PressedEditStyleInButtonGroup);
					DisabledEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.DisabledEditStyleInButtonGroupProperty, BarEditItemThemeKeys.DisabledEditStyleInButtonGroup);
					break;
				case LinkContainerType.RibbonPageGroup:
					NormalEditStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.NormalEditStyleInRibbonPageGroupProperty, BarEditItemThemeKeys.NormalEditStyleInRibbonPageGroup);
					break;
				case LinkContainerType.RibbonStatusBarLeft:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInRibbonStatusBarLeftProperty, BarEditItemThemeKeys.LayoutPanelStyleInRibbonStatusBarLeft);
					break;
				case LinkContainerType.RibbonStatusBarRight:
					EditAndContentLayoutPanelStyle = (Style)GetTemplateFromProvider(BarEditItemLinkControlTemplateProvider.EditAndContentLayoutPanelStyleInRibbonStatusBarRightProperty, BarEditItemThemeKeys.LayoutPanelStyleInRibbonStatusBarRight);
					break;
			}
		}
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();			
			base.OnApplyTemplate();
			TemplatedEdit = FindTemplatedEdit();
			SubscribeTemplateEvents();
		}
		protected virtual void UnsubscribeTemplateEvents() {
		}
		protected virtual void SubscribeTemplateEvents() {
		}
		BaseEdit FindTemplatedEdit() {
			BaseEdit element = LayoutHelper.FindElement(this, (e) => { return (e is BaseEdit); }) as BaseEdit;
			return element;
		}
		protected override Func<BarItemLayoutPanelThemeKeys, BarItemLayoutPanelThemeKeyExtension> GetThemeKeyExtensionFunc {
			get {
				return key => new BarEditItemLayoutPanelThemeKeyExtension() { ResourceKey = key, ThemeName = ThemeName };
			}
		}
		protected override void UpdateLayoutPanel() {
			if(LayoutPanel == null) return;
			base.UpdateLayoutPanel();
			UpdateLayoutPanelShowContent2();
			LayoutPanel.ShowAdditionalContent = true;			
			UpdateLayoutPanelContent2();
			UpdateLayoutPanelContent2Template();
			UpdateLayoutPanelAdditionalContentCommon();
		}
		protected virtual void UpdateLayoutPanelAdditionalContentCommon() {
			UpdateLayoutPanelAdditionalContent();
			UpdateLayoutPanelAdditionalContentStyle();
			UpdateLayoutPanelEditWidth();
			UpdateLayoutPanelEditHeight();
			UpdateLayoutPanelAdditionalContentHorizontalAlignment();
			UpdateLayoutPanelAdditionalContentTemplate();
			UpdateLayoutPanelStretchAdditionalContentVertically();
		}
		protected virtual void UpdateLayoutPanelShowContent2() {
			if(LayoutPanel == null) return;
			LayoutPanel.ShowContent2 = ActualContent2Visibility == System.Windows.Visibility.Visible;
		}
		protected virtual void UpdateLayoutPanelEditWidth() {
			if(LayoutPanel == null) return;
			LayoutPanel.AdditionalContentSizeSettings.Width = GetEditWidth();
		}
		protected virtual void UpdateLayoutPanelAdditionalContentTemplate() {
			if(LayoutPanel == null) return;
			LayoutPanel.AdditionalContentTemplate = ActualEditTemplate;
		}
		protected virtual void UpdateLayoutPanelEditHeight() {
			if(LayoutPanel == null) return;
			LayoutPanel.AdditionalContentSizeSettings.Height = GetEditHeight();
		}
		protected virtual void UpdateLayoutPanelContent2Template() {
			if(LayoutPanel == null) return;
			LayoutPanel.Content2Template = ActualContent2Template;
		}
		protected virtual void UpdateLayoutPanelContent2() {
			if(LayoutPanel == null) return;
			LayoutPanel.Content2 = ActualContent2;
		}
		protected virtual void UpdateLayoutPanelStretchAdditionalContentVertically() {
			if(LayoutPanel == null) return;
			LayoutPanel.StretchAdditionalContentVertically = false;
		}
		protected virtual void UpdateLayoutPanelAdditionalContent() {
			if(LayoutPanel == null) return;
			LayoutPanel.AdditionalContent = Edit;
		}
		protected virtual void UpdateLayoutPanelAdditionalContentStyle() {
			if(LayoutPanel == null) return;
			LayoutPanel.AdditionalContentStyle = ActualEditStyle;
		}
		protected virtual void UpdateLayoutPanelAdditionalContentHorizontalAlignment() {
			if(LayoutPanel == null) return;
			LayoutPanel.AdditionalContentHorizontalAlignment = ActualHorizontalEditAlignment;
		}
		protected override Size MeasureOverride(Size constraint) {
			Size retValue = base.MeasureOverride(constraint);
			if(Edit == null && TemplatedEdit == null && LayoutPanel != null) {
				TemplatedEdit = FindTemplatedEdit();
				if(TemplatedEdit != null)
					SubscribeTemplatedEditEvents();
			}
			return retValue;
		}
		protected override void OnLinkInfoChanged(BarItemLinkInfo oldValue) {
			base.OnLinkInfoChanged(oldValue);
			CreateEdit();
		}		
		protected override void UpdateActualShowGlyphInRibbon() {
			if(CurrentRibbonStyle == RibbonItemStyles.SmallWithoutText) {
				ActualShowGlyph = false;
				return;
			}
			if(CurrentRibbonStyle == RibbonItemStyles.SmallWithText && ActualContent == null) {
				if(ActualGlyph == DefaultGlyph || ActualGlyph == DefaultLargeGlyph) {
					ActualShowGlyph = false;
					return;
				}									
			}
			base.UpdateActualShowGlyphInRibbon();
		}
	}
	public class BarEditItemLinkControlTemplateProvider : DependencyObject {
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInBarProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInMainMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInStatusBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInRibbonPageGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInButtonGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInQuickAccessToolbarProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInQuickAccessToolbar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInRibbonPageHeaderProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInRibbonPageHeader", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInRibbonStatusBarLeftProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInRibbonStatusBarLeft", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty EditAndContentLayoutPanelStyleInRibbonStatusBarRightProperty = DependencyPropertyManager.RegisterAttached("EditAndContentLayoutPanelStyleInRibbonStatusBarRight", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalEditStyleInBarProperty = DependencyPropertyManager.RegisterAttached("NormalEditStyleInBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotEditStyleInBarProperty = DependencyPropertyManager.RegisterAttached("HotEditStyleInBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedEditStyleInBarProperty = DependencyPropertyManager.RegisterAttached("PressedEditStyleInBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledEditStyleInBarProperty = DependencyPropertyManager.RegisterAttached("DisabledEditStyleInBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalEditStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("NormalEditStyleInMainMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotEditStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("HotEditStyleInMainMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedEditStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("PressedEditStyleInMainMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledEditStyleInMainMenuProperty = DependencyPropertyManager.RegisterAttached("DisabledEditStyleInMainMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalEditStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("NormalEditStyleInStatusBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotEditStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("HotEditStyleInStatusBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedEditStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("PressedEditStyleInStatusBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledEditStyleInStatusBarProperty = DependencyPropertyManager.RegisterAttached("DisabledEditStyleInStatusBar", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalEditStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("NormalEditStyleInMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotEditStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("HotEditStyleInMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedEditStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("PressedEditStyleInMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledEditStyleInMenuProperty = DependencyPropertyManager.RegisterAttached("DisabledEditStyleInMenu", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalEditStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("NormalEditStyleInButtonGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotEditStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("HotEditStyleInButtonGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedEditStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("PressedEditStyleInButtonGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledEditStyleInButtonGroupProperty = DependencyPropertyManager.RegisterAttached("DisabledEditStyleInButtonGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty NormalEditStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("NormalEditStyleInRibbonPageGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty HotEditStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("HotEditStyleInRibbonPageGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty PressedEditStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("PressedEditStyleInRibbonPageGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DisabledEditStyleInRibbonPageGroupProperty = DependencyPropertyManager.RegisterAttached("DisabledEditStyleInRibbonPageGroup", typeof(Style), typeof(BarEditItemLinkControlTemplateProvider), new UIPropertyMetadata(null));
		public static Style GetDisabledEditStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(DisabledEditStyleInButtonGroupProperty);
		}
		public static void SetDisabledEditStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(DisabledEditStyleInButtonGroupProperty, value);
		}
		public static Style GetPressedEditStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(PressedEditStyleInButtonGroupProperty);
		}
		public static void SetPressedEditStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(PressedEditStyleInButtonGroupProperty, value);
		}
		public static Style GetHotEditStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(HotEditStyleInButtonGroupProperty);
		}
		public static void SetHotEditStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(HotEditStyleInButtonGroupProperty, value);
		}
		public static Style GetNormalEditStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(NormalEditStyleInButtonGroupProperty);
		}
		public static void SetNormalEditStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(NormalEditStyleInButtonGroupProperty, value);
		}
		public static Style GetDisabledEditStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(DisabledEditStyleInRibbonPageGroupProperty);
		}
		public static void SetDisabledEditStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(DisabledEditStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetPressedEditStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(PressedEditStyleInRibbonPageGroupProperty);
		}
		public static void SetPressedEditStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(PressedEditStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetHotEditStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(HotEditStyleInRibbonPageGroupProperty);
		}
		public static void SetHotEditStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(HotEditStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetNormalEditStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(NormalEditStyleInRibbonPageGroupProperty);
		}
		public static void SetNormalEditStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(NormalEditStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetDisabledEditStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(DisabledEditStyleInMenuProperty);
		}
		public static void SetDisabledEditStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(DisabledEditStyleInMenuProperty, value);
		}
		public static Style GetPressedEditStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(PressedEditStyleInMenuProperty);
		}
		public static void SetPressedEditStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(PressedEditStyleInMenuProperty, value);
		}
		public static Style GetHotEditStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(HotEditStyleInMenuProperty);
		}
		public static void SetHotEditStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(HotEditStyleInMenuProperty, value);
		}
		public static Style GetNormalEditStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(NormalEditStyleInMenuProperty);
		}
		public static void SetNormalEditStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(NormalEditStyleInMenuProperty, value);
		}
		public static Style GetDisabledEditStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(DisabledEditStyleInStatusBarProperty);
		}
		public static void SetDisabledEditStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(DisabledEditStyleInStatusBarProperty, value);
		}
		public static Style GetPressedEditStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(PressedEditStyleInStatusBarProperty);
		}
		public static void SetPressedEditStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(PressedEditStyleInStatusBarProperty, value);
		}
		public static Style GetHotEditStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(HotEditStyleInStatusBarProperty);
		}
		public static void SetHotEditStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(HotEditStyleInStatusBarProperty, value);
		}
		public static Style GetNormalEditStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(NormalEditStyleInStatusBarProperty);
		}
		public static void SetNormalEditStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(NormalEditStyleInStatusBarProperty, value);
		}
		public static Style GetDisabledEditStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(DisabledEditStyleInMainMenuProperty);
		}
		public static void SetDisabledEditStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(DisabledEditStyleInMainMenuProperty, value);
		}
		public static Style GetPressedEditStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(PressedEditStyleInMainMenuProperty);
		}
		public static void SetPressedEditStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(PressedEditStyleInMainMenuProperty, value);
		}
		public static Style GetHotEditStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(HotEditStyleInMainMenuProperty);
		}
		public static void SetHotEditStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(HotEditStyleInMainMenuProperty, value);
		}
		public static Style GetNormalEditStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(NormalEditStyleInMainMenuProperty);
		}
		public static void SetNormalEditStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(NormalEditStyleInMainMenuProperty, value);
		}
		public static Style GetDisabledEditStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(DisabledEditStyleInBarProperty);
		}
		public static void SetDisabledEditStyleInBar(DependencyObject target, Style value) {
			target.SetValue(DisabledEditStyleInBarProperty, value);
		}
		public static Style GetPressedEditStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(PressedEditStyleInBarProperty);
		}
		public static void SetPressedEditStyleInBar(DependencyObject target, Style value) {
			target.SetValue(PressedEditStyleInBarProperty, value);
		}
		public static Style GetHotEditStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(HotEditStyleInBarProperty);
		}
		public static void SetHotEditStyleInBar(DependencyObject target, Style value) {
			target.SetValue(HotEditStyleInBarProperty, value);
		}
		public static Style GetNormalEditStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(NormalEditStyleInBarProperty);
		}
		public static void SetNormalEditStyleInBar(DependencyObject target, Style value) {
			target.SetValue(NormalEditStyleInBarProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInRibbonStatusBarRight(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInRibbonStatusBarRightProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInRibbonStatusBarRight(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInRibbonStatusBarLeft(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInRibbonStatusBarRightProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInRibbonStatusBarLeft(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInRibbonStatusBarRightProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInRibbonPageHeader(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInRibbonPageHeaderProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInRibbonPageHeader(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInRibbonPageHeaderProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInQuickAccessToolbar(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInQuickAccessToolbarProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInQuickAccessToolbar(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInQuickAccessToolbarProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInButtonGroup(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInButtonGroupProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInButtonGroup(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInButtonGroupProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInRibbonPageGroup(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInRibbonPageGroupProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInRibbonPageGroup(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInRibbonPageGroupProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInMenu(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInMenuProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInMenu(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInMenuProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInStatusBar(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInStatusBarProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInStatusBar(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInStatusBarProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInMainMenu(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInMainMenuProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInMainMenu(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInMainMenuProperty, value);
		}
		public static Style GetEditAndContentLayoutPanelStyleInBar(DependencyObject target) {
			return (Style)target.GetValue(EditAndContentLayoutPanelStyleInBarProperty);
		}
		public static void SetEditAndContentLayoutPanelStyleInBar(DependencyObject target, Style value) {
			target.SetValue(EditAndContentLayoutPanelStyleInBarProperty, value);
		}
	}
	class LostFocusWeakEventHandler : WeakEventHandler<BarEditItemLinkControl, KeyboardFocusChangedEventArgs, KeyboardFocusChangedEventHandler> {
		static Action<WeakEventHandler<BarEditItemLinkControl, KeyboardFocusChangedEventArgs, KeyboardFocusChangedEventHandler>, object> onDetachAction = (h, o) => ((FrameworkElement)o).PreviewLostKeyboardFocus -= h.Handler;
		static Func<WeakEventHandler<BarEditItemLinkControl, KeyboardFocusChangedEventArgs, KeyboardFocusChangedEventHandler>, KeyboardFocusChangedEventHandler> createHandlerFunction = h => h.OnEvent;
		public LostFocusWeakEventHandler(BarEditItemLinkControl owner, Action<BarEditItemLinkControl, object, KeyboardFocusChangedEventArgs> onEventAction)
			:
			base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
	class GotFocusWeakEventHandler : WeakEventHandler<BarEditItemLinkControl, KeyboardFocusChangedEventArgs, KeyboardFocusChangedEventHandler> {
		static Action<WeakEventHandler<BarEditItemLinkControl, KeyboardFocusChangedEventArgs, KeyboardFocusChangedEventHandler>, object> onDetachAction = (h, o) => ((FrameworkElement)o).PreviewGotKeyboardFocus -= h.Handler;
		static Func<WeakEventHandler<BarEditItemLinkControl, KeyboardFocusChangedEventArgs, KeyboardFocusChangedEventHandler>, KeyboardFocusChangedEventHandler> createHandlerFunction = h => h.OnEvent;
		public GotFocusWeakEventHandler(BarEditItemLinkControl owner, Action<BarEditItemLinkControl, object, KeyboardFocusChangedEventArgs> onEventAction)
			:
			base(owner, onEventAction, onDetachAction, createHandlerFunction) {
		}
	}
}

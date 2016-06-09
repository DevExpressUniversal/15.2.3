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

using System.Windows.Controls;
using System.Windows;
using System.Windows.Threading;
using System;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using System.Windows.Interop;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class GalleryItemControl : Control, INavigationElement {
		#region static
		public static readonly DependencyProperty IsHoverGlyphVisibleProperty;
		public static readonly DependencyProperty ItemProperty;
		public static readonly DependencyProperty GroupControlProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty ActualCaptionTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualCaptionTextStylePropertyKey;
		public static readonly DependencyProperty ActualCaptionProperty;
		protected static readonly DependencyPropertyKey ActualCaptionPropertyKey;
		public static readonly DependencyProperty ActualDescriptionProperty;
		protected static readonly DependencyPropertyKey ActualDescriptionPropertyKey;
		public static readonly DependencyProperty ActualDescriptionTextStyleProperty;
		protected static readonly DependencyPropertyKey ActualDescriptionTextStylePropertyKey;
		public static readonly DependencyProperty IsItemContentVisibleProperty;
		public static readonly DependencyProperty IsItemGlyphVisibleProperty;
		public static readonly DependencyProperty ActualIsItemGlyphVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsItemGlyphVisiblePropertyKey;
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty ActualIsItemContentVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsItemContentVisiblePropertyKey;
		static GalleryItemControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(GalleryItemControl), typeof(GalleryItemControlAutomationPeer), owner => new GalleryItemControlAutomationPeer((GalleryItemControl)owner));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItemControl), new FrameworkPropertyMetadata(typeof(GalleryItemControl)));
			ItemProperty = DependencyPropertyManager.Register("Item", typeof(GalleryItem), typeof(GalleryItemControl),
				new FrameworkPropertyMetadata(null, OnItemPropertyChanged));
			IsHoverGlyphVisibleProperty = DependencyPropertyManager.Register("IsHoverGlyphVisible", typeof(bool), typeof(GalleryItemControl),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsHoverGlyphVisiblePropertyChanged)));
			GroupControlProperty = DependencyPropertyManager.Register("GroupControl", typeof(GalleryItemGroupControl), typeof(GalleryItemControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupControlPropertyChanged)));
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(GalleryItemControl),
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedPropertyChanged)));
			ActualCaptionTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCaptionTextStyle", typeof(Style), typeof(GalleryItemControl), new FrameworkPropertyMetadata(null));
			ActualCaptionTextStyleProperty = ActualCaptionTextStylePropertyKey.DependencyProperty;
			ActualCaptionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCaption", typeof(object), typeof(GalleryItemControl), new FrameworkPropertyMetadata(null));
			ActualCaptionProperty = ActualCaptionPropertyKey.DependencyProperty;
			ActualDescriptionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDescription", typeof(object), typeof(GalleryItemControl), new FrameworkPropertyMetadata(null));
			ActualDescriptionProperty = ActualDescriptionPropertyKey.DependencyProperty;
			ActualDescriptionTextStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDescriptionTextStyle", typeof(Style), typeof(GalleryItemControl), new FrameworkPropertyMetadata(null));
			ActualDescriptionTextStyleProperty = ActualDescriptionTextStylePropertyKey.DependencyProperty;
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(GalleryItemControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((GalleryItemControl)d).OnGlyphChanged(e.OldValue as ImageSource))));
			IsItemContentVisibleProperty = DependencyPropertyManager.Register("IsItemContentVisible", typeof(bool), typeof(GalleryItemControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => ((GalleryItemControl)d).OnIsItemContentVisibleChanged((bool)e.NewValue))));
			IsItemGlyphVisibleProperty = DependencyPropertyManager.Register("IsItemGlyphVisible", typeof(bool), typeof(GalleryItemControl), new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => ((GalleryItemControl)d).OnIsItemGlyphVisibleChanged((bool)e.NewValue))));
			ActualIsItemGlyphVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsItemGlyphVisible", typeof(bool), typeof(GalleryItemControl), new FrameworkPropertyMetadata(true));
			ActualIsItemGlyphVisibleProperty = ActualIsItemGlyphVisiblePropertyKey.DependencyProperty;
			ActualIsItemContentVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsItemContentVisible", typeof(bool), typeof(GalleryItemControl), new FrameworkPropertyMetadata(true));
			ActualIsItemContentVisibleProperty = ActualIsItemContentVisiblePropertyKey.DependencyProperty;
		}
		static void OnIsHoverGlyphVisiblePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((GalleryItemControl)o).OnHoverGlyphVisibilityChanged();
		}
		static void OnItemPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((GalleryItemControl)o).OnItemChanged(e.OldValue as GalleryItem);
		}
		static void OnGroupControlPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((GalleryItemControl)o).OnGroupControlChanged(e.OldValue as GalleryItemGroupControl);
		}
		static void OnIsSelectedPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((GalleryItemControl)o).OnIsSelectedChanged((bool) e.OldValue);
		}
		#endregion
		#region dep props
		public object ActualCaption {
			get { return (object)GetValue(ActualCaptionProperty); }
			protected set { this.SetValue(ActualCaptionPropertyKey, value); }
		}
		public object ActualDescription {
			get { return (object)GetValue(ActualDescriptionProperty); }
			protected set { this.SetValue(ActualDescriptionPropertyKey, value); }
		}
		public bool IsHoverGlyphVisible {
			get { return (bool)GetValue(IsHoverGlyphVisibleProperty); }
			set { SetValue(IsHoverGlyphVisibleProperty, value); }
		}
		public GalleryItem Item {
			get { return (GalleryItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public GalleryItemGroupControl GroupControl {
			get { return (GalleryItemGroupControl)GetValue(GroupControlProperty); }
			set { SetValue(GroupControlProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public Style ActualCaptionTextStyle {
			get { return (Style)GetValue(ActualCaptionTextStyleProperty); }
			protected set { this.SetValue(ActualCaptionTextStylePropertyKey, value); }
		}
		public Style ActualDescriptionTextStyle {
			get { return (Style)GetValue(ActualDescriptionTextStyleProperty); }
			protected set { this.SetValue(ActualDescriptionTextStylePropertyKey, value); }
		}
		public bool IsItemContentVisible {
			get { return (bool)GetValue(IsItemContentVisibleProperty); }
			set { SetValue(IsItemContentVisibleProperty, value); }
		}
		public bool IsItemGlyphVisible {
			get { return (bool)GetValue(IsItemGlyphVisibleProperty); }
			set { SetValue(IsItemGlyphVisibleProperty, value); }
		}
		public bool ActualIsItemGlyphVisible {
			get { return (bool)GetValue(ActualIsItemGlyphVisibleProperty); }
			protected internal set { this.SetValue(ActualIsItemGlyphVisiblePropertyKey, value); }
		}
		public bool ActualIsItemContentVisible {
			get { return (bool)GetValue(ActualIsItemContentVisibleProperty); }
			protected internal set { this.SetValue(ActualIsItemContentVisiblePropertyKey, value); }
		}
		public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
		#endregion
		public GalleryItemControl() {
			Unloaded +=new RoutedEventHandler(OnUnloaded);
			Loaded += new RoutedEventHandler(OnLoaded);
			ToolTip toolTip = new BarItemLinkControlToolTip() { UseToolTipPlacementTarget = true };
			toolTip.Loaded += new RoutedEventHandler(OnToolTipLoaded);			
			IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
			ToolTipService.SetToolTip(this, toolTip);
			BindingOperations.SetBinding(this, TemplateProperty, new Binding() { Path = new PropertyPath(GalleryControl.ActualItemControlTemplateProperty), RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(GalleryControl), 1) });			
		}		
		private BorderState itemBorderStateCore = BorderState.Normal;
		protected BorderState ItemBorderState {
			get { return itemBorderStateCore; }
			set {
				if(itemBorderStateCore == value)
					return;
				BorderState oldValue = itemBorderStateCore;
				itemBorderStateCore = value;
				OnItemBorderStateChanged(oldValue);
			}
		}
		protected virtual void OnItemBorderStateChanged(BorderState oldValue) {
			SetItemBorderVisualState(ItemBorderState);
			UpdateActualCaptionTextStyle();
			UpdateActualDescriptionTextStyle();
		}
		protected virtual void UpdateActualCaptionTextStyle() {
			if(Gallery != null && Gallery.ItemCaptionTextStyleSelector != null) {
				ActualCaptionTextStyle = Gallery.ItemCaptionTextStyleSelector.SelectStyle(ItemBorderState);
				return;
			}
			if(GalleryControl != null && GalleryControl.ItemCaptionTextStyleSelector != null)
				ActualCaptionTextStyle = GalleryControl.ItemCaptionTextStyleSelector.SelectStyle(ItemBorderState);
		}
		protected virtual void UpdateActualDescriptionTextStyle() {
			if(Gallery != null && Gallery.ItemDescriptionTextStyleSelector != null) {
				ActualDescriptionTextStyle = Gallery.ItemDescriptionTextStyleSelector.SelectStyle(ItemBorderState);
				return;
			}
			if(GalleryControl != null && GalleryControl.ItemDescriptionTextStyleSelector != null)
				ActualDescriptionTextStyle = GalleryControl.ItemDescriptionTextStyleSelector.SelectStyle(ItemBorderState);
		}
		internal DependencyObject GetTemplateChildCore(string childName) {
			return GetTemplateChild(childName);
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		void OnIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			UpdateLayoutState();
		}
		void OnItemIsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			SetIsEnabled();
		}
		void OnCommandCanExecuteChanged(object sender, EventArgs e) {
			SetIsEnabled();
		}
		protected virtual void SetIsEnabled() {
			this.SetCurrentValue(IsEnabledProperty, CalculateActualIsEnabled());
		}
		protected virtual bool CalculateActualIsEnabled() {
			if(Item == null) return false;
			bool value = Item.IsEnabled && (Item.GetCommandCanExecute() || Item.Command == null);
			return value;
		}
		internal int DesiredRowIndex { get; set; }
		internal int DesiredColIndex { get; set; }
		internal bool DesiredStartOfLine { get; set; }
		protected override void OnToolTipOpening(ToolTipEventArgs e) {
			if(!IsToolTipEnable || !IsLoaded) {
				e.Handled = true;
				return;
			}
		}
		void OnToolTipLoaded(object sender, RoutedEventArgs e) {
			if(!IsToolTipEnable || !IsLoaded) {
				((ToolTip)sender).Content = null;
				((ToolTip)sender).IsOpen = false;
				return;
			}
			if(Gallery != null && Gallery.ToolTipTemplate != null) {
				((ToolTip)sender).Template = Gallery.ToolTipTemplate;
				((ToolTip)sender).ApplyTemplate();
			}
			if(Item != null) ((ToolTip)sender).DataContext = Item.Caption;
			((ToolTip)sender).Content = CreateSuperTipControl();
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			DestroyHoverToolTip(true);
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			CheckBringIntoView();
		}
		static int HoverDelay = 300;
		static double DefaultHoverGlyphScale = 2;
		DispatcherTimer hoverDelayTimer = null;
		ItemBorderControl ItemBorder { get; set; }
		ItemBorderControl GlyphBorder { get; set; }
		public ContentViewport GlyphViewport { get; private set; }
		public ContentViewport ContentViewport { get; private set; }
		Image Image { get; set; }
		internal GalleryItemHoverToolTip HoverToolTip { get; set; }
		bool IsLeftButtonPressed { get; set; }
		protected GalleryControl GalleryControl {
			get {
				if(GroupControl == null || GroupControl.GroupsControl == null) return null;
				return GroupControl.GroupsControl.GalleryControl;
			}
		}
		public Gallery Gallery {
			get {
				if(Item == null || Item.Group == null) return null;
				return Item.Group.Gallery;
			}
		}
		protected void SetBinding(DependencyObject obj, DependencyProperty prop, string propName) {
			Binding b = new Binding(propName);
			b.Source = Item;
			BindingOperations.SetBinding(obj, prop, b);
		}
		SuperTipControl CreateSuperTipControl() {
			if (Item == null) return null;
			SuperTip sp = Item.SuperTip;
			if(sp == null) {
				sp = new SuperTip();
				if(Item.Caption != null) {
					SuperTipHeaderItem h = new SuperTipHeaderItem() { Content = Item.Caption, ContentTemplate = Item.Group.Gallery.HintCaptionTemplate };
					sp.Items.Add(h);
				}
				if(Item.Hint != null) {
					SuperTipItem item = new SuperTipItem() { Content = Item.Hint, ContentTemplate = Item.Group.Gallery.HintTemplate };
					sp.Items.Add(item);
				}
			}
			return new SuperTipControl(sp);
		}
		object CreateToolTip() {
			return new ToolTip() { Content = CreateSuperTipControl() };
		}
		protected virtual void OnItemChanged(GalleryItem oldValue) {
			if(oldValue != null) {
				oldValue.Checked -= OnItemChecked;
				oldValue.Unchecked -= OnItemUnchecked;
				oldValue.IsEnabledChanged -= OnItemIsEnabledChanged;
				oldValue.CommandCanExecuteChanged -= OnCommandCanExecuteChanged;
				oldValue.CaptionChanged -= OnItemCaptionChanged;
				oldValue.DescriptionChanged -= OnItemDescriptionChanged;
				ClearValue(GlyphProperty);
			}
			if(Item != null) {
				Item.Checked += new EventHandler(OnItemChecked);
				Item.Unchecked += new EventHandler(OnItemUnchecked);
				Item.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnItemIsEnabledChanged);
				Item.CommandCanExecuteChanged += new EventHandler(OnCommandCanExecuteChanged);
				Item.CaptionChanged += OnItemCaptionChanged;
				Item.DescriptionChanged += OnItemDescriptionChanged;
				SetBinding(GlyphProperty, new Binding("Glyph") { Source = Item });
				var gallery = Item.With(x=>x.Group).With(x=>x.Gallery);
				if (gallery != null) {
					SetBinding(IsItemContentVisibleProperty, new Binding("IsItemContentVisible") { Source = gallery });
					SetBinding(IsItemGlyphVisibleProperty, new Binding("IsItemGlyphVisible") { Source = gallery });
				}
				SetIsEnabled();
			}
			UpdateActualCaption();
			UpdateActualDescription();
			UpdateActualIsItemGlyphVisible();
			UpdateActualIsItemContentVisible();
		}
		protected virtual void OnIsItemGlyphVisibleChanged(bool newValue) {
			UpdateActualIsItemGlyphVisible();
		}
		protected virtual void OnGlyphChanged(ImageSource oldValue) {
			UpdateActualIsItemGlyphVisible();
		}
		protected virtual void OnIsItemContentVisibleChanged(bool newValue) {
			UpdateActualIsItemContentVisible();
		}
		protected virtual void UpdateActualIsItemContentVisible() {
			ActualIsItemContentVisible = (ActualCaption != null || ActualDescription != null) && IsItemContentVisible;
		}
		protected virtual void UpdateActualIsItemGlyphVisible() {
			ActualIsItemGlyphVisible = (Glyph != null || GlyphBorder != null || GlyphViewport != null || Image != null) && IsItemGlyphVisible;
		}
		void OnItemCaptionChanged(object sender, EventArgs e) {
			UpdateActualCaption();
			UpdateActualIsItemContentVisible();
		}
		void OnItemDescriptionChanged(object sender, EventArgs e) {
			UpdateActualDescription();
			UpdateActualIsItemContentVisible();
		}
		void UpdateActualCaption() {
			ActualCaption = Item == null ? null : Item.Caption;
		}
		void UpdateActualDescription() {
			ActualDescription = Item == null ? null : Item.Description;
		}
		protected virtual void OnIsSelectedChanged(bool oldValue) {
			UpdateLayoutState();
		}
		protected virtual void OnGroupControlChanged(GalleryItemGroupControl oldValue) {
			UpdateActualValues();
		}
		protected internal virtual void UpdateActualValues() {
			UpdateActualCaptionTextStyle();
			UpdateActualDescriptionTextStyle();
		}
		void OnItemChecked(object sender, EventArgs e) {
			UpdateLayoutState();
			CheckBringIntoView();
		}
		void CheckBringIntoView() {
			if (GalleryControl != null && Item != null && Gallery!=null) {
				if (!Item.IsChecked || Gallery.GetFirstCheckedItem()!=Item)
					return;
				if(GalleryControl.GetFirstVisibleRowIndex()>=DesiredRowIndex || DesiredRowIndex>=GalleryControl.GetLastVisibleRowIndex())
					GalleryControl.ScrollToItem(Item);
			}
		}
		void OnItemUnchecked(object sender, EventArgs e) {
			UpdateLayoutState();
		}
		protected virtual void SetItemBorderVisualState(BorderState state) {
			if(ItemBorder != null) ItemBorder.State = state;
			VisualStateManager.GoToState(this, state.ToString(), false);
		}
		protected virtual void SetGlyphBorderVisualState(BorderState state) {
			if(GlyphBorder != null) GlyphBorder.State = state;
			VisualStateManager.GoToState(this, "Glyph" + state.ToString(), false);
		}
		protected virtual void UpdateLayoutState() {
			if(Item == null || Item.Group == null || Item.Group.Gallery == null) return;
			if(!IsEnabled) {
				ItemBorderState = BorderState.Disabled;
				SetGlyphBorderVisualState(BorderState.Disabled);
				VisualStateManager.GoToState(this, "Disabled", false);
				return;
			}
			VisualStateManager.GoToState(this, "Enabled", false);
			if(IsMouseOver || IsSelected) {
				if(Item.IsChecked || IsLeftButtonPressed) {
					ItemBorderState = BorderState.HoverChecked;
				}
				else {
					ItemBorderState = BorderState.Hover;
				}
			}
			else {
				if(Item.IsChecked) {
					if(Item.Group.Gallery.CheckDrawMode == GalleryCheckDrawMode.OnlyImage) {
						ItemBorderState = BorderState.Normal;
					}
					else {
						ItemBorderState = BorderState.Checked;
					}
				}
				else {
					ItemBorderState = BorderState.Normal;
				}
			}
			if(Item.IsChecked && Item.Group.Gallery.CheckDrawMode == GalleryCheckDrawMode.OnlyImage) {
				SetGlyphBorderVisualState(BorderState.Checked);
			}
			else {
				SetGlyphBorderVisualState(BorderState.Normal);
			}
		}
		internal void OnMouseLeftButtonUpCore(){
			DestroyHoverToolTip();
			if(GroupControl != null && GroupControl.GroupsControl != null && GroupControl.GroupsControl.GalleryControl != null)
				GroupControl.GroupsControl.GalleryControl.OnGalleryItemControlClick(this);
			IsLeftButtonPressed = false;
			if(Item == null) return;
			var  closePopup = ProcessClickBehavior();
			if(Item != null)
				Item.OnClick(this);
			if(Item != null && Item.ClonedFrom != null) Item.ClonedFrom.OnClick(null);
			UpdateLayoutState();
			if (closePopup)
				BarManagerHelper.GetPopup(this).Do(x => x.OnItemClick(this, null));
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			if(IsLeftButtonPressed == false)
				return;
			OnMouseLeftButtonUpCore();
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			IsHoverGlyphVisible = false;
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			IsLeftButtonPressed = true;
			IsHoverGlyphVisible = false;
			UpdateLayoutState();
		}
		bool ProcessClickBehavior() {
			if (Gallery == null || Item == null) return false;			
			switch(Gallery.ItemCheckMode) {
				case GalleryItemCheckMode.None:
					return true;
				case GalleryItemCheckMode.Single:
				case GalleryItemCheckMode.SingleInGroup:
					Item.IsChecked = true;
					if(Gallery != null && Gallery.SyncWithClone && Item.ClonedFrom != null) Item.ClonedFrom.IsChecked = Item.IsChecked;
					return true;
				case GalleryItemCheckMode.Multiple:					
					Item.IsChecked = !Item.IsChecked;
					if(Gallery != null && Gallery.SyncWithClone && Item.ClonedFrom != null) Item.ClonedFrom.IsChecked = Item.IsChecked;
					break;
			}
			return false;
		}
		bool IsToolTipEnable {
			get {
				if(Gallery == null || Item == null) return false;
				bool isContentVisible = (!Gallery.IsItemCaptionVisible && Item.Caption != null) || (!Gallery.IsItemDescriptionVisible && Item.Description != null)
					|| (Item.Hint != null) || Item.SuperTip != null || (!Gallery.IsItemGlyphVisible && Item.Glyph != null) || !(Item.Caption is string) && (Gallery.HintCaptionTemplate != null);
				if(!isContentVisible) return false;
				if(BarManagerHelper.GetOrFindBarManager(this) != null) {
					return Gallery.AllowToolTips && BarNameScope.GetService<ICustomizationService>(this).CustomizationHelper.Return(x => x.ShowScreenTips, () => true);
				}
				return Gallery.AllowToolTips;
			}
		}
		INavigationOwner INavigationElement.BoundOwner { get { return null; } }
		bool INavigationElement.IsSelected {
			get { return IsSelected; }
			set {
				if (!Equals(IsSelected, value) && (IsSelected = value))
					Focus();
			}
		}
		IBarsNavigationSupport IBarsNavigationSupport.Parent { get { return GalleryControl; } }
		int IBarsNavigationSupport.ID { get { return Item.GetHashCode(); } }
		bool IBarsNavigationSupport.IsSelectable { get { return IsEnabled && IsVisible; } }
		bool IBarsNavigationSupport.ExitNavigationOnMouseUp { get { return false; } }
		bool IBarsNavigationSupport.ExitNavigationOnFocusChangedWithin { get { return false; } }
		void StartHoverTimer() {
			if(hoverDelayTimer == null) {
				hoverDelayTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(HoverDelay), DispatcherPriority.Normal,
					new EventHandler(OnHoverTimerOverflow), this.Dispatcher);
			}
			hoverDelayTimer.Start();
		}
		void StopHoverTimer() {
			if(hoverDelayTimer != null)
				hoverDelayTimer.Stop();
		}
		void OnHoverTimerOverflow(object sender, EventArgs args) {
			if(Gallery != null && new Rect(RenderSize).Contains(Mouse.GetPosition(this))) {
				if(IsHoverGlyphVisible == false && Gallery.AllowHoverImages == true && (Item.Glyph != null || Item.HoverGlyph != null)) {
					IsHoverGlyphVisible = true;
				}
			}
			StopHoverTimer();
			if(Item != null) Item.OnHover(this);
		}
		public override void OnApplyTemplate() {
			DestroyHoverToolTip();
			base.OnApplyTemplate();
			GlyphViewport = GetTemplateChild("PART_GlyphViewport") as ContentViewport;
			ContentViewport newContentViewport = GetTemplateChild("PART_ContentViewport") as ContentViewport;
			if(newContentViewport != null && ContentViewport != null) {
				newContentViewport.Width = ContentViewport.Width;
				newContentViewport.Height = ContentViewport.Height;
			}
			ContentViewport = newContentViewport;
			Image = GetTemplateChild("PART_Image") as Image;
			ItemBorder = GetTemplateChild("PART_Border") as ItemBorderControl;
			GlyphBorder = GetTemplateChild("PART_GlyphBorder") as ItemBorderControl;
			UpdateLayoutState();
			SetItemBorderVisualState(ItemBorderState);
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsLeftButtonPressed = false;
			UpdateLayoutState();
			if(Gallery == null) return;
			IsHoverGlyphVisible = false;
			if(hoverDelayTimer != null && hoverDelayTimer.IsEnabled) {
				StopHoverTimer();
				return;
			}
			if(Item != null) Item.OnLeave(this);
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateLayoutState();
			if (Gallery == null) return;
			StartHoverTimer();
			if (Item != null) Item.OnEnter(this);
			EnsureSelectedInNavigationMode();
		}
		void EnsureSelectedInNavigationMode() {
			if (GalleryControl == null)
				return;
			if (!NavigationTree.SelectElement(this)) {
				(((INavigationOwner)GalleryControl).Parent as IMutableNavigationSupport).Do(x => x.RaiseChanged());
				NavigationTree.SelectElement(this);
			}			
		}
		void OnHoverGlyphVisibilityChanged() {
#if WPF
			if(BrowserInteropHelper.IsBrowserHosted)
				return;
#endif
			if(IsHoverGlyphVisible == false) {
				if(HoverToolTip != null)
					HoverToolTip.ShrinkAndClose();
				return;
			}
			if(HoverToolTip == null)
				HoverToolTip = CreateHoverToolTip();
			HoverToolTip.Grow();
		}
		bool IsItemFullyVisible() {
			GalleryGroupsViewer groupsViewer = LayoutHelper.FindParentObject<GalleryGroupsViewer>(this);
			if(groupsViewer == null) return false;
			return groupsViewer.IsItemFullyVisible(this);
		}
		GalleryItemHoverToolTip CreateHoverToolTip() {
			GalleryItemHoverToolTip ctrl = new GalleryItemHoverToolTip();
			ctrl.EndShrinkAnimation += new EventHandler(OnHoverToolTipEndAnimation);
			ctrl.AllowAnimation = Gallery.AllowHoverAnimation;
			ctrl.Duration = Gallery.HoverAnimationDuration;
			ctrl.Placement = PlacementMode.Bottom;
			ctrl.PlacementTarget = Image;
			ctrl.LargeGlyphSize = GetMaxHoverGlyphSize();
			ctrl.SmallGlyphSize = new Size(Image.ActualWidth, Image.ActualHeight);
			if(Item.HoverGlyph != null)
				ctrl.Glyph = Item.HoverGlyph;
			else if(Item.Glyph != null)
				ctrl.Glyph = Item.Glyph;
			ctrl.IsOpen = true;
			ctrl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			ctrl.HorizontalOffset = (Image.ActualWidth - ctrl.DesiredSize.Width) / 2;
			ctrl.VerticalOffset = (-Image.ActualHeight - ctrl.DesiredSize.Height) / 2;
			return ctrl;
		}
		void DestroyHoverToolTip() {
			DestroyHoverToolTip(false);
		}
		void DestroyHoverToolTip(bool onUnload) {
			if(HoverToolTip == null)
				return;
			HoverToolTip.IsOpen = false;
			HoverToolTip.EndShrinkAnimation -= OnHoverToolTipEndAnimation;
			HoverToolTip = null;
			IsHoverGlyphVisible = false;
		}
		void OnHoverToolTipEndAnimation(object sender, EventArgs e) {
			DestroyHoverToolTip();
		}
		Size GetMaxHoverGlyphSize() {
			if(Gallery != null) {
				if(Gallery.HoverGlyphSize != null)
					return (Size)Gallery.HoverGlyphSize;
				if(Item != null) {
					if(Item.HoverGlyph != null)
						return new Size(double.NaN, double.NaN);
					if(Item.Glyph != null)
						return new Size(Image.ActualWidth * DefaultHoverGlyphScale, Image.ActualHeight * DefaultHoverGlyphScale);
				}
			}
			return new Size(double.NaN, double.NaN);
		}
		bool INavigationElement.ProcessKeyDown(KeyEventArgs e) {
			if(e.Key == Key.Space || e.Key == Key.Enter) {
				OnMouseLeftButtonUpCore();
				return true;
			}
			return false;
		}
	}
}

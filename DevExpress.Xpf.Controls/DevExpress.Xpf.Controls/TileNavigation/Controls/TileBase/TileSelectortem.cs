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

using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
namespace DevExpress.Xpf.Navigation {
	public enum TileColorMode { Standard, Auto, Inverted }
	public abstract class TileSelectorItem : TileBase, ISelectorItem, IFlyoutEventListener, ICommandSource {
		#region static
		public static readonly DependencyProperty ShowFlyoutButtonProperty;
		protected static readonly DependencyPropertyKey IsFlyoutButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsFlyoutButtonVisibleProperty;
		public static readonly DependencyProperty AllowGlyphThemingProperty;
		static readonly DependencyPropertyKey IsCheckedPropertyKey;
		public static readonly DependencyProperty IsCheckedProperty;
		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;
		public static readonly DependencyProperty CommandTargetProperty;
		static readonly DependencyPropertyKey CalculatedBackgroundPropertyKey;
		static readonly DependencyPropertyKey CalculatedForegroundPropertyKey;
		public static readonly DependencyProperty CalculatedBackgroundProperty;
		public static readonly DependencyProperty CalculatedForegroundProperty;
		public static readonly DependencyProperty SelectionPaddingProperty;
		static readonly DependencyPropertyKey ActualSelectionPaddingPropertyKey;				
		public static readonly DependencyProperty ActualSelectionPaddingProperty;
		public static readonly DependencyProperty ColorModeProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected internal static readonly DependencyProperty ActualColorModeProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ShowShadowProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty ArrowDirectionProperty;
		static TileSelectorItem() {
			var dProp = new DependencyPropertyRegistrator<TileSelectorItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("ShowFlyoutButton", ref ShowFlyoutButtonProperty, true, OnShowFlyoutButtonChanged);
			dProp.RegisterReadonly("IsFlyoutButtonVisible", ref IsFlyoutButtonVisiblePropertyKey, ref IsFlyoutButtonVisibleProperty, false, null,
				OnCoerceIsFlyoutButtonVisible);
			dProp.Register("AllowGlyphTheming", ref AllowGlyphThemingProperty, false);
			dProp.RegisterReadonly("IsChecked", ref IsCheckedPropertyKey, ref IsCheckedProperty, false);
			dProp.Register("Command", ref CommandProperty, (ICommand)null, OnCommandChanged);
			dProp.Register("CommandParameter", ref CommandParameterProperty, (object)null);
			dProp.Register("CommandTarget", ref CommandTargetProperty, (IInputElement)null);
			dProp.RegisterReadonly("CalculatedBackground", ref CalculatedBackgroundPropertyKey, ref CalculatedBackgroundProperty, (Brush)null);
			dProp.RegisterReadonly("CalculatedForeground", ref CalculatedForegroundPropertyKey, ref CalculatedForegroundProperty, (Brush)null);
			dProp.Register("SelectionPadding", ref SelectionPaddingProperty, new Thickness(4));
			dProp.RegisterReadonly("ActualSelectionPadding", ref ActualSelectionPaddingPropertyKey, ref ActualSelectionPaddingProperty, new Thickness(4), null, OnCoerceActualSelectionPadding);
			dProp.Register("ColorMode", ref ColorModeProperty, TileColorMode.Standard, OnColorModeChanged);
			dProp.Register("ActualColorMode", ref ActualColorModeProperty, TileColorMode.Standard, OnActualColorModeChanged, OnCoerceActualColorMode);
			IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(TileSelectorItem),
				new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnIsSelectedChanged, OnCoerceIsSelected));
			ShowShadowProperty = DependencyProperty.Register("ShowShadow", typeof(bool), typeof(TileSelectorItem), new PropertyMetadata(false, OnShowShadowChanged));
			ArrowDirectionProperty = DependencyProperty.Register("ArrowDirection", typeof(ButtonDirection), typeof(TileSelectorItem), new PropertyMetadata(ButtonDirection.Down));
		}
		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				tileSelectorItem.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}
		private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				tileSelectorItem.OnIsSelectedChanged((bool)e.NewValue);
		}
		private static object OnCoerceIsSelected(DependencyObject d, object value) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				return tileSelectorItem.OnCoerceIsSelected((bool)value);
			else
				return value;
		}
		private static void OnColorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				tileSelectorItem.OnColorModeChanged((TileColorMode)e.OldValue, (TileColorMode)e.NewValue);
		}
		private static object OnCoerceActualColorMode(DependencyObject d, object value) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				return tileSelectorItem.OnCoerceActualColorMode((TileColorMode)value);
			else
				return value;
		}
		private static void OnActualColorModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				tileSelectorItem.OnActualColorModeChanged((TileColorMode)e.OldValue, (TileColorMode)e.NewValue);
		}
		private static void OnShowFlyoutButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				tileSelectorItem.OnShowFlyoutButtonChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static object OnCoerceIsFlyoutButtonVisible(DependencyObject d, object value) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				return tileSelectorItem.OnCoerceIsFlyoutButtonVisible((bool)value);
			else
				return value;
		}
		private static void OnShowShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				tileSelectorItem.OnShowShadowChanged((bool)e.OldValue, (bool)e.NewValue);
		}
		private static object OnCoerceActualSelectionPadding(DependencyObject d, object value) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				return tileSelectorItem.OnCoerceActualSelectionPadding((Thickness)value);
			else
				return value;
		}
		private static void OnSelectionPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TileSelectorItem tileSelectorItem = d as TileSelectorItem;
			if(tileSelectorItem != null)
				tileSelectorItem.OnSelectionPaddingChanged((Thickness)e.NewValue);
		}
		#endregion
		protected TileSelectorItem() {
			AttachPropertyListener("Background", BackgroundListener);
			AttachPropertyListener("Foreground", ForegroundListener);
		}
		protected override void OnDispose() {
			ownerCore = null;
			base.OnDispose();
		}
		protected virtual void OnBrushPropertyChanged() {
			UpdateCalculatedBrushes();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			UpdateCalculatedBrushes();
		}
		protected override void OnPropertyChanged(DependencyProperty propertyListener, object oldValue, object newValue) {
			base.OnPropertyChanged(propertyListener, oldValue, newValue);
			if(propertyListener == BackgroundListener)
				OnBrushPropertyChanged();
			if(propertyListener == ForegroundListener)
				OnBrushPropertyChanged();
		}
		protected virtual void UpdateCalculatedBrushes() {
			CoerceValue(ActualColorModeProperty);
			CalculatedBackground = GetCalculatedBackground();
			CalculatedForeground = GetCalculatedForeground();
		}
		protected virtual Brush GetCalculatedBackground() {
			switch(ActualColorMode) {
				case TileColorMode.Standard:
				case TileColorMode.Auto:
					return Background;
				case TileColorMode.Inverted:
					return Foreground;
			}
			return Background;
		}
		protected virtual Brush GetCalculatedForeground() {
			switch(ActualColorMode) {
				case TileColorMode.Standard:
				case TileColorMode.Auto:
					return Foreground;
				case TileColorMode.Inverted:
					return Background;
			}
			return Background;
		}
		TileBarItemArrowControl PartArrowButton;
		TileButtonControl PartButton;
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			PartArrowButton = GetTemplateChild("PART_ArrowButton") as TileBarItemArrowControl;
			PartButton = GetTemplateChild("PART_Button") as TileButtonControl;
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			if(PartArrowButton != null) {
				PartArrowButton.Click += PartArrowControl_Click;
				PartArrowButton.SetBinding(TileBarItemArrowControl.ArrowDirectionProperty, new Binding("ArrowDirection") { Source = this });
			}
			if(PartButton != null) PartButton.IsEnabled = CanExecute;
			Controller.UpdateState(false);
		}
		internal virtual FlyoutManager GetFlyoutManager() {
			return FlyoutManager.GetFlyoutManager(this);
		}
		protected override void OnClick() {
			base.OnClick();
			FlyoutManager flyoutManager = GetFlyoutManager();
			if(flyoutManager != null) {
				Dispatcher.BeginInvoke(new Action(() =>
				{
					bool cancel = false;
					OnFlyoutClosing(ref cancel);
					if (!cancel)
						flyoutManager.CloseAll();
				}));
			}
			SelectIfPossible();
			InvokeCommand();
		}
		protected void SelectIfPossible() {
			TileBar tileBar = Owner as TileBar;
			if(tileBar != null && CanExecute && tileBar.AllowItemSelection && CanBeSelected)
				SelectInParentContainer();
		}
		protected virtual bool CanBeSelected { get { return true; } }
		protected virtual void OnFlyoutClosing(ref bool cancel) { }
		protected override void ClearTemplateChildren() {
			if(PartArrowButton != null) {
				PartArrowButton.Click -= PartArrowControl_Click;
				PartArrowButton.ClearValue(TileArrowControl.ArrowDirectionProperty);
			}
			base.ClearTemplateChildren();
		}
		protected virtual void BeforeFlyoutOpened() {
		}
		protected virtual double GetFlyoutOffset() {
			return 10d;
		}
		private void UpdateFlyoutOffset(IFlyoutProvider provider) {
			var placement = provider.Placement;
			var flyoutControl = provider.FlyoutControl;
			UIElement parent = provider as UIElement;
			if(parent == null || flyoutControl == null) return;
			double flayoutOffset = GetFlyoutOffset();
			flyoutControl.VerticalOffset = 0.0;
			flyoutControl.HorizontalOffset = 0.0;
			if(placement == Editors.Flyout.FlyoutPlacement.Bottom) {
				var p = this.TranslatePoint(new Point(0, RenderSize.Height), parent);
				flyoutControl.VerticalOffset = -(parent.RenderSize.Height - p.Y) + flayoutOffset;
			}
			if(placement == Editors.Flyout.FlyoutPlacement.Top) {
				var p = this.TranslatePoint(new Point(0, 0), parent);
				flyoutControl.VerticalOffset = p.Y - flayoutOffset;
			}
			if(placement == Editors.Flyout.FlyoutPlacement.Right) {
				var p = this.TranslatePoint(new Point(RenderSize.Width, 0), parent);
				flyoutControl.HorizontalOffset = -(parent.RenderSize.Width - p.X) + flayoutOffset;
			}
			if(placement == Editors.Flyout.FlyoutPlacement.Left) {
				var p = this.TranslatePoint(new Point(0, 0), parent);
				flyoutControl.HorizontalOffset = p.X - flayoutOffset;
			}
		}
		private void UpdateFlyoutAlignment(IFlyoutProvider provider) {
			var flyoutControl = provider.FlyoutControl;
			var placement = provider.Placement;
			if(flyoutControl == null) return;
			flyoutControl.ClearValue(FrameworkElement.HorizontalAlignmentProperty);
			flyoutControl.ClearValue(FrameworkElement.VerticalAlignmentProperty);
			if(placement == Editors.Flyout.FlyoutPlacement.Bottom || placement == Editors.Flyout.FlyoutPlacement.Top)
				flyoutControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
			else
				flyoutControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
		}
		internal void PartArrowControl_Click(object sender, EventArgs e) {
			IFlyoutProvider provider = LayoutHelper.FindParentObject<IFlyoutProvider>(this);
			if(provider == null) {
			}
			if(provider != null) {
				var PartFlyoutControl = provider.FlyoutControl;
				PartFlyoutControl.Settings = new DevExpress.Xpf.Editors.Flyout.FlyoutSettings() { Placement = provider.Placement, IndicatorTarget = PartArrowButton, ShowIndicator = true };
				PartFlyoutControl.StaysOpen = true;
				PartFlyoutControl.Content = PartTileNavPaneContentControl;
				PartFlyoutControl.PlacementTarget = provider as UIElement;
				UpdateFlyout(PartFlyoutControl);
				PartFlyoutControl.SetBinding(BackgroundProperty, new Binding("Background") { Source = this });
				PartFlyoutControl.SetBinding(BorderBrushProperty, new Binding("Background") { Source = this });
				UpdateFlyoutAlignment(provider);
				UpdateFlyoutOffset(provider);
				FlyoutManager FlyoutManager = FlyoutManager.GetFlyoutManager(this);
				if(FlyoutManager != null) {
					if(PartFlyoutControl.IsOpen && FlyoutManager.GetFlyoutTarget(PartFlyoutControl) == this)
						FlyoutManager.Hide(provider, this as IFlyoutEventListener);
					else {
						BeforeFlyoutOpened();
						FlyoutManager.Show(provider, this as IFlyoutEventListener);
					}
				}
			}
		}
		protected virtual void UpdateFlyout(DevExpress.Xpf.Editors.Flyout.FlyoutControl flyoutControl) { }
		protected override void EnsureSizeManager() {
			FrameworkElement sizeManagerHolder = GetSizeManagerHolder();
			if(sizeManagerHolder != null) {
				object resSizeManager;
				resSizeManager = sizeManagerHolder.Resources["SizeManager"];
				SizeManager = resSizeManager as TileSizeManager;
				if(SizeManager != null)
					SizeManager.OwnerRef = new System.WeakReference(this);
				UpdateTileSize();
			}
		}
		protected virtual FrameworkElement GetSizeManagerHolder() {
			return PartButton;
		}
		protected virtual UIElement GetFlyoutContainer() {
			return null;
		}
		private void UpdateCanExecute() {
			CanExecute = Command != null ? CommandHelper.CanExecuteCommand(this) : true;
		}
		protected virtual void OnCommandChanged(ICommand oldValue, ICommand newValue) {
			if(oldValue != null)
				if(oldValue != null) oldValue.CanExecuteChanged -= OnCanExecuteChanged;
			if(newValue != null)
				if(newValue != null) newValue.CanExecuteChanged += OnCanExecuteChanged;
			UpdateCanExecute();
		}
		private void OnCanExecuteChanged(object sender, EventArgs e) {
			UpdateCanExecute();
		}
		protected virtual void OnIsSelectedChanged(bool isSelected) {
			EnsureOwnerSelection(isSelected);
		}
		protected virtual object OnCoerceIsSelected(bool value) {
			bool isSelected = CanExecute && value;
			TileBar tileBar = Owner as TileBar;
			return isSelected && (tileBar == null || tileBar.AllowItemSelection);
		}
		protected void EnsureOwnerSelection(bool isSelected) {
			RaiseEvent(new RoutedEventArgs(isSelected ? Selector.SelectedEvent : Selector.UnselectedEvent, this));
			if(isSelected) {
				SelectInParentContainer();
			}
			Controller.UpdateState(false);
		}
		protected virtual bool SelectInParentContainer() {
			var templatedParent = LayoutHelper.FindParentObject<ItemsControl>(this);
			ISelector contentSelector = Owner as ISelector ?? templatedParent as ISelector;
			if(contentSelector != null) {
				contentSelector.Select(this);
			}
			return contentSelector != null;
		}
		protected override ControlControllerBase CreateController() {
			return new TileSelectorItemController(this);
		}
		protected virtual void InvokeCommand() {
			CommandHelper.ExecuteCommand(this);
		}
		protected virtual void OnColorModeChanged(TileColorMode oldValue, TileColorMode newValue) {
			CoerceValue(ActualColorModeProperty);
		}
		protected virtual TileColorMode OnCoerceActualColorMode(TileColorMode value) {
			TileBar tileBar = Owner as TileBar;
			if(ColorMode != TileColorMode.Standard) return ColorMode;
			return tileBar != null ? tileBar.ItemColorMode : ColorMode;
		}
		protected virtual void OnActualColorModeChanged(TileColorMode oldValue, TileColorMode newValue) {
			UpdateCalculatedBrushes();
		}
		protected virtual Thickness OnCoerceActualSelectionPadding(Thickness value) {
			TileBar tileBar = Owner as TileBar;
			if(tileBar == null || tileBar.AllowItemSelection) {
				return SelectionPadding;
			}
			return new Thickness(0);
		}
		protected virtual void OnSelectionPaddingChanged(Thickness newValue) {
			CoerceValue(ActualSelectionPaddingProperty);
		}
		protected virtual void OnShowFlyoutButtonChanged(bool oldValue, bool newValue) {
			CoerceValue(IsFlyoutButtonVisibleProperty);
		}
		protected virtual object OnCoerceIsFlyoutButtonVisible(bool value) {
			return ShowFlyoutButton;
		}
		protected virtual void OnShowShadowChanged(bool oldValue, bool newValue) {
			Controller.UpdateState(false);
		}
		UIElement _PartTileNavPaneContentControl;
		UIElement PartTileNavPaneContentControl {
			get {
				if(_PartTileNavPaneContentControl == null) {
					_PartTileNavPaneContentControl = GetFlyoutContainer();
				}
				return _PartTileNavPaneContentControl;
			}
		}
		protected internal TileColorMode ActualColorMode {
			get { return (TileColorMode)GetValue(ActualColorModeProperty); }
			set { SetValue(ActualColorModeProperty, value); }
		}
		public TileColorMode ColorMode {
			get { return (TileColorMode)GetValue(ColorModeProperty); }
			set { SetValue(ColorModeProperty, value); }
		}
		public Brush CalculatedBackground {
			get { return (Brush)GetValue(CalculatedBackgroundProperty); }
			private set { SetValue(CalculatedBackgroundPropertyKey, value); }
		}
		public Brush CalculatedForeground {
			get { return (Brush)GetValue(CalculatedForegroundProperty); }
			private set { SetValue(CalculatedForegroundPropertyKey, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public Thickness ActualSelectionPadding {
			get { return (Thickness)GetValue(ActualSelectionPaddingProperty); }
			private set { SetValue(ActualSelectionPaddingProperty, value); }
		}
		public Thickness SelectionPadding {
			get { return (Thickness)GetValue(SelectionPaddingProperty); }
			set { SetValue(SelectionPaddingProperty, value); }
		}
		public bool ShowFlyoutButton {
			get { return (bool)GetValue(ShowFlyoutButtonProperty); }
			set { SetValue(ShowFlyoutButtonProperty, value); }
		}
		public bool IsFlyoutButtonVisible {
			get { return (bool)GetValue(IsFlyoutButtonVisibleProperty); }
			internal set { SetValue(IsFlyoutButtonVisiblePropertyKey, value); }
		}
		public bool AllowGlyphTheming {
			get { return (bool)GetValue(AllowGlyphThemingProperty); }
			set { SetValue(AllowGlyphThemingProperty, value); }
		}
		public bool IsChecked {
			get { return (bool)GetValue(IsCheckedProperty); }
			private set { SetValue(IsCheckedPropertyKey, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public object CommandParameter {
			get { return (object)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}
		public IInputElement CommandTarget {
			get { return (IInputElement)GetValue(CommandTargetProperty); }
			set { SetValue(CommandTargetProperty, value); }
		}
		ISelector ownerCore;
		public ISelector Owner {
			get { return ownerCore ?? Parent as ISelector; }
		}
		ISelector ISelectorItem.Owner {
			get { return ownerCore; }
			set { ownerCore = value; }
		}
		bool ISelectorItem.IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		bool _CanExecute = true;
		internal bool CanExecute {
			get { return _CanExecute; }
			set {
				if(value != _CanExecute) {
					_CanExecute = value;
					if(PartButton != null)
						PartButton.IsEnabled = value;
				}
			}
		}
		internal bool CanClick {
			get { return CanExecute; }
		}
		internal bool ShowShadow {
			get { return (bool)GetValue(ShowShadowProperty); }
			set { SetValue(ShowShadowProperty, value); }
		}
		#region IFlyoutEventListener Members
		protected virtual void OnFlyoutClosed() {
			IsChecked = false;
		}
		protected virtual void OnFlyoutOpened() {
			IsChecked = true;
		}
		void IFlyoutEventListener.OnFlyoutClosed() {
			OnFlyoutClosed();
		}
		void IFlyoutEventListener.OnFlyoutOpened() {
			OnFlyoutOpened();
		}
		void IFlyoutEventListener.OnMouseLeave() {
		}
		void IFlyoutEventListener.OnMouseEnter() {
		}
		void IFlyoutEventListener.OnFlyoutClosed(bool onClickThrough) {
			throw new NotImplementedException();
		}
		WindowsUI.Internal.Flyout.FlyoutBase IFlyoutEventListener.Flyout { get { return null; } }
		#endregion
	}
	class TileSelectorItemController : ClickableController {
		public TileSelectorItemController(TileSelectorItem control)
			: base(control) {
		}
		protected virtual void OnClickCore() {
			base.OnClick();
		}
		protected override void OnClick() {
			if(Control.CanClick)
				OnClickCore();
		}
		protected override void OnMouseLeftButtonDown(DXMouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(!Control.CanClick) e.Handled = true;
		}
		public new TileSelectorItem Control { get { return (TileSelectorItem)base.Control; } }
		public override void UpdateState(bool useTransitions) {
			base.UpdateState(useTransitions);
			string stateName = "EmptySelectedState";
			VisualStateManager.GoToState(Control, stateName, useTransitions);
			if(Control.IsSelected)
				stateName = "Selected";
			else
				stateName = "Unselected";
			VisualStateManager.GoToState(Control, stateName, useTransitions);			
			stateName = Control.ShowShadow ? "ShadowVisible" : "ShadowHidden";
			VisualStateManager.GoToState(Control, stateName, useTransitions);
		}
		protected override bool FocusOnMouseDown {
			get { return true; }
		}
	}
}

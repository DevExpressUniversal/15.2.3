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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Bars.Native;
using System.Linq;
using System.Collections.Specialized;
using System.Collections;
namespace DevExpress.Xpf.Bars {
	public class BarControl : LinksControl, IBarLayoutTableInfo, INavigationElement {
		#region Dependency Properties
		static readonly DependencyPropertyKey ActualShowQuickCustomizationButtonPropertyKey =
			DependencyPropertyManager.RegisterReadOnly("ActualShowQuickCustomizationButton", typeof(bool), typeof(BarControl), 
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BarControl)d).OnActualShowQuickCustomizationButtonChanged(e)));
		static readonly DependencyPropertyKey ActualShowDragWidgetPropertyKey =
			DependencyPropertyManager.RegisterReadOnly("ActualShowDragWidget", typeof(bool), typeof(BarControl), 
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, (d, e) => ((BarControl)d).OnActualShowDragWigetChanged(e)));
		static readonly DependencyPropertyKey ContainerOrientationPropertyKey =
			DependencyPropertyManager.RegisterReadOnly("ContainerOrientation", typeof(Orientation), typeof(BarControl), 
			new FrameworkPropertyMetadata(Orientation.Horizontal, (d, e) => ((BarControl)d).OnContainerOrientationChanged()));
		static readonly DependencyPropertyKey ActualShowSizeGripPropertyKey=
			DependencyPropertyManager.RegisterReadOnly("ActualShowSizeGrip",typeof(bool),typeof(BarControl),
			new FrameworkPropertyMetadata(false, (d, e) => ((BarControl)d).OnActualShowSizeGripChanged()));
		protected static readonly DependencyPropertyKey ActualShowContentPropertyKey =
			DependencyPropertyManager.RegisterReadOnly("ActualShowContent", typeof(bool), typeof(BarControl), new FrameworkPropertyMetadata(true));
		public static readonly DependencyProperty BarItemDisplayModeProperty =
			DependencyPropertyManager.Register("BarItemDisplayMode", typeof(BarItemDisplayMode), typeof(BarControl), new FrameworkPropertyMetadata(BarItemDisplayMode.Default, new PropertyChangedCallback((d, e) => ((BarControl)d).OnBarItemDisplayModeChanged((BarItemDisplayMode)e.OldValue))));
		public static readonly DependencyProperty ShowBackgroundProperty =
			DependencyProperty.Register("ShowBackground", typeof(bool), typeof(BarControl), new PropertyMetadata(true));
		public static readonly DependencyProperty ActualShowContentProperty = ActualShowContentPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualShowQuickCustomizationButtonProperty = ActualShowQuickCustomizationButtonPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualShowDragWidgetProperty = ActualShowDragWidgetPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ContainerOrientationProperty = ContainerOrientationPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualShowSizeGripProperty = ActualShowSizeGripPropertyKey.DependencyProperty;
		#endregion Dependency Properties
		public bool ActualShowContent {
			get { return (bool)GetValue(ActualShowContentProperty); }
			protected internal set { this.SetValue(ActualShowContentPropertyKey, value); }
		}
		public bool ActualShowQuickCustomizationButton {
			get { return (bool)GetValue(ActualShowQuickCustomizationButtonProperty); }
			private set { this.SetValue(ActualShowQuickCustomizationButtonPropertyKey, value); }
		}
		public bool ActualShowDragWidget {
			get { return (bool)GetValue(ActualShowDragWidgetProperty); }
			private set { this.SetValue(ActualShowDragWidgetPropertyKey, value); }
		}
		public Orientation ContainerOrientation {
			get { return (Orientation)GetValue(ContainerOrientationProperty); }
			protected internal set { this.SetValue(ContainerOrientationPropertyKey, value); }
		}
		public bool ActualShowSizeGrip {
			get { return (bool)GetValue(ActualShowSizeGripProperty); }
			protected internal set { this.SetValue(ActualShowSizeGripPropertyKey, value);}
		}
		public BarItemDisplayMode BarItemDisplayMode {
			get { return (BarItemDisplayMode)GetValue(BarItemDisplayModeProperty); }
			set { SetValue(BarItemDisplayModeProperty, value); }
		}
		public bool ShowBackground {
			get { return (bool)GetValue(ShowBackgroundProperty); }
			set { SetValue(ShowBackgroundProperty, value); }
		}		
		public Bar Bar {
			get {
				if(LinksHolder != null)
					return LinksHolder as Bar;
				return DataContext as Bar;
			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Key == Key.Escape && Manager != null && Manager.MainMenu == Bar && NavigationTree.CurrentElement!=null) {
				Manager.DeactivateMenu();
			}
		}
		public BarDockInfo DockInfo {
			get {
				if(Bar != null)
					return Bar.DockInfo;
				return null;
			}
		}
		protected internal double RowCount { get; set; }
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		public override BarItemLinkCollection ItemLinks {
			get {
				if(Bar == null)
					return null;
				return ((ILinksHolder)Bar).ActualLinks;
			}
		}
		static BarControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(BarControl), typeof(BarControlAutomationPeer), owner => new BarControlAutomationPeer((BarControl)owner));
			KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(BarControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.None));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(BarControl), new FrameworkPropertyMetadata(typeof(BarControl)));
		}
		public BarControl() {
			GotKeyboardFocus += OnGotKeyboardFocus;
		}
		protected virtual void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {			
		}
		void thumb_DragDelta(object sender, DragDeltaEventArgs e) {
			Window wnd = GetParentWindow();
			if(wnd != null && wnd.Content is UIElement && wnd.WindowState != WindowState.Maximized) {
				UIElement content = wnd.Content as UIElement;
				if(wnd.FlowDirection == FlowDirection.LeftToRight) {
					wnd.Width = Math.Max(wnd.Width + MinMax(wnd.Width + e.HorizontalChange, MinWidth, MaxWidth) - wnd.Width, wnd.MinWidth);
					wnd.Height = Math.Max(wnd.Height + MinMax(content.RenderSize.Height + e.VerticalChange, ActualHeight, wnd.MaxHeight) - content.RenderSize.Height, wnd.MinHeight);
				}
				else {
					double newWidth = Math.Max(wnd.Width + MinMax(wnd.Width + e.HorizontalChange, MinWidth, MaxWidth) - wnd.Width, wnd.MinWidth);
					double deltaWidth = wnd.Width - newWidth;
					wnd.Width = newWidth;
					wnd.Height = Math.Max(wnd.Height + MinMax(content.RenderSize.Height + e.VerticalChange, ActualHeight, wnd.MaxHeight) - content.RenderSize.Height, wnd.MinHeight);
					if(newWidth == wnd.ActualWidth)
						wnd.Left += deltaWidth;
				}
			}
		}
		Window GetParentWindow() {
			return this.GetRootParent() as Window;
		}
		void thumb_MouseEnter(object sender, MouseEventArgs e) {
			UpdateDragCursor();
		}
		protected virtual void UpdateDragCursor() {
			if(thumb == null)
				return;
			thumb.Cursor = FlowDirection == FlowDirection.LeftToRight ? Cursors.SizeNWSE : Cursors.SizeNESW;
		}
		double MinMax(double value, double minValue, double maxValue) {
			return Math.Max(minValue, Math.Min(value, maxValue));
		}
		public override void OnApplyTemplate() {
			UnSubscribeEvents();
			base.OnApplyTemplate();
			this.dragWidget = (DragWidget)GetTemplateChild("PART_DragWidget");
			if(dragWidget != null) {
				DragManager.SetDropTargetFactory(dragWidget, new BarControlDropTargetFactoryExtension());
			}
			this.quickCustomizationButton = (BarQuickCustomizationButton)GetTemplateChild("PART_QuickCustomizationButton");
			if(quickCustomizationButton != null) {
				DragManager.SetDropTargetFactory(quickCustomizationButton, new BarControlDropTargetFactoryExtension() { AlwaysAdd = true });
			}
			this.thumb = (Thumb)GetTemplateChild("PART_Thumb");
			UpdateQuickCustomizationButton();
			UpdateThumb();			
			ContentBackground = (ContentControl)GetTemplateChild("PART_Content");
			Root = (ContentControl)GetTemplateChild("PART_Bar");
			if(FrameworkElementHelper.GetIsLoaded(this)) {
				UpdateBarCustomizationButonsVisibility();
				UpdateVisualState();
			}
			SubscribeEvents();
			CheckContinueDragBar();
			UpdateQuickCustomizationButton();
			UpdateDragWidgetVisibility();
			UpdateThumb();
			UpdateItemsSource(((ILinksHolder)Bar).ActualLinks);
		}		
		protected internal DragWidget DragWidget { get { return dragWidget; } }
		protected internal BarQuickCustomizationButton QuickCustomizationButton { get { return quickCustomizationButton; } }		
		protected internal ContentControl ContentBackground { get; private set; }
		protected internal ContentControl Root { get; private set; }		
		protected virtual void SubscribeEvents() {
			if(DragWidget != null) {
				DragWidget.MouseDoubleClick += new MouseButtonEventHandler(OnDragWidgetDoubleClick);
			}
			if(QuickCustomizationButton != null) {
				QuickCustomizationButton.Unchecked += OnQuickCustomizationUnchecked;
				QuickCustomizationButton.Checked += OnQuickCustomizationChecked;
				QuickCustomizationButton.Manager = Manager;
			}
			if(thumb != null) {
				thumb.DragDelta += new DragDeltaEventHandler(thumb_DragDelta);
				thumb.MouseEnter += new MouseEventHandler(thumb_MouseEnter);
			}
		}		
		protected virtual void UnSubscribeEvents() {
			if(DragWidget != null) {
				DragWidget.MouseDoubleClick -= new MouseButtonEventHandler(OnDragWidgetDoubleClick);
			}
			if(QuickCustomizationButton != null) {
				QuickCustomizationButton.Unchecked -= OnQuickCustomizationUnchecked;
				QuickCustomizationButton.Checked -= OnQuickCustomizationChecked;
				QuickCustomizationButton.Manager = null;
			}
			if(thumb != null) {
				thumb.DragDelta -= new DragDeltaEventHandler(thumb_DragDelta);
				thumb.MouseEnter -= new MouseEventHandler(thumb_MouseEnter);
			}
		}		
		protected internal void OnLayoutPropertyChanged() {
			UpdateVisibility();
			if (layoutPropertyChanged != null)
				layoutPropertyChanged(this, EventArgs.Empty);
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			if(Bar == null)
				return;			
			UpdateBarCustomizationButonsVisibility();
			UpdateVisualState();
			FrameworkElementHelper.SetIsLoaded(this, true);
			OnBarVisibleChanged();
			if(!IsUnloaded)
				return;
			IsUnloaded = false;
			UpdateItemsSource(((ILinksHolder)Bar).ActualLinks);
		}
		protected override void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
			FrameworkElementHelper.SetIsLoaded(this, false);
			IsUnloaded = true;
			UpdateItemsSource(null);
		}
		protected virtual void OnActualShowDragWigetChanged(DependencyPropertyChangedEventArgs e) {
			UpdateDragWidgetVisibility();
		}
		protected virtual void UpdateDragWidgetVisibility() {
			if (DragWidget == null)
				return;
			DragWidget.Visibility = ActualShowDragWidget ? Visibility.Visible : Visibility.Collapsed;
		}
		protected virtual void OnActualShowQuickCustomizationButtonChanged(DependencyPropertyChangedEventArgs e) {
			UpdateQuickCustomizationButtonVisibility();
		}
		private void UpdateQuickCustomizationButtonVisibility() {
			if (QuickCustomizationButton == null)
				return;
			QuickCustomizationButton.Visibility = ActualShowQuickCustomizationButton ? Visibility.Visible : Visibility.Collapsed;
		}
		protected virtual void OnContainerOrientationChanged() {
			UpdateQuickCustomizationButton();
		}
		protected virtual void OnActualShowSizeGripChanged() {
			UpdateThumb();
		}
		protected internal virtual void OnGlyphSizeChanged(DependencyPropertyChangedEventArgs e) {
			ForceCalcMaxGlyphSize();
		}
		protected internal virtual void OnBarIsRemovedChanged() {
			UpdateVisibility();
		}
		protected internal virtual void OnBarVisibleChanged() {
			UpdateVisibility();
		}
		protected override void OnItemsChangedCore(NotifyCollectionChangedAction action, IList oldITems, IList newItems) {			
			base.OnItemsChangedCore(action, oldITems, newItems);
			UpdateVisibility();
			InvalidateMeasure();
		}
		protected internal virtual void UpdateVisibility() {			
			if(Bar != null && Bar.IsRemoved) {
				Visibility = Visibility.Collapsed;
				return;
			}			
			if(Bar != null && !Bar.ShowWhenBarManagerIsMerged && MergingProperties.GetHideElements(Bar) && Bar.CanBeMerged() && BarHasMergingCandidates()) {
				Visibility = Visibility.Collapsed;
				return;
			}
			Visibility = Bar != null && Bar.Visible && (!Bar.HideWhenEmpty || HasItems) ? Visibility.Visible : Visibility.Collapsed;
		}
		bool BarHasMergingCandidates() {
			object mergingRegistratorName = ((IMultipleElementRegistratorSupport)Bar).GetName(typeof(IMergingSupport));
			return ScopeTree.Ancestors(BarNameScope.GetScope(Bar)).Reverse().
				SelectMany(x => x.GetService<IElementRegistratorService>().
				GetElements<IMergingSupport>(mergingRegistratorName))
				.Where(x => BarNameScope.GetService<IMergingService>(x).CanMerge(x, Bar)).Any();
		}
		protected virtual void UpdateQuickCustomizationButton() {
			if(QuickCustomizationButton != null)
				QuickCustomizationButton.Orientation = ContainerOrientation;
			UpdateQuickCustomizationButtonVisibility();
		}
		protected virtual void UpdateThumb() {
			if(thumb == null)
				return;
			thumb.Visibility = Bar.IsStatusBar & Bar.ShowSizeGrip? Visibility.Visible: Visibility.Collapsed;
		}
		protected internal virtual void UpdateBarCustomizationButonsVisibility() {
			UpdateDragWidgetVisibility();
			UpdateQuickCustomizationButtonVisibility();			
		}
		internal bool ActualShowQuickCustomizationButtonCore { get; set; }
		protected internal virtual void CalcBarCustomizationButonsVisibility() {
			ActualShowQuickCustomizationButton = GetAllowQuickCustomizationButton();
			if (DockInfo.Container != null)
				ActualShowQuickCustomizationButton &= !DockInfo.Container.IsFloating & !Bar.IsStatusBar;
			bool actualShowDragWidget = Bar.ShowDragWidget && !Bar.IsMainMenu && !Bar.IsStatusBar;
			ActualShowDragWidget = actualShowDragWidget && (DockInfo.Container != null && !DockInfo.Container.IsFloating);
		}
		protected internal bool GetAllowQuickCustomizationButton() {
			bool allowQucikButton = Bar.IsAllowQuickCustomization;
			if (Bar.Manager != null)
				allowQucikButton &= Bar.Manager.AllowQuickCustomization;
			ActualShowQuickCustomizationButtonCore = allowQucikButton;
			if (!IsAllLinksVisible())
				allowQucikButton = true;
			return allowQucikButton;
		}
		protected virtual bool IsAllLinksVisible() {
			if (ItemsSource == null)
				return true;
			return !ItemsSource.OfType<BarItemLinkInfo>().Any(x => x.IsHidden());			
		}		
		protected virtual bool CanReduce() {
			if (ItemsSource == null)
				return false;
			return ItemsSource.OfType<BarItemLinkInfo>().Any(x => !x.IsHidden());
		}
		protected virtual void OnQuickCustomizationUnchecked(object sender, RoutedEventArgs e) {
			BarNameScope.GetService<ICustomizationService>(this).HideCustomizationMenu(QuickCustomizationButton);
		}
		protected virtual void OnQuickCustomizationChecked(object sender, RoutedEventArgs e) {			
			((ToggleButton)sender).IsChecked = BarNameScope.GetService<ICustomizationService>(this).ShowCustomizationMenu(QuickCustomizationButton);
		}
		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
			base.OnMouseRightButtonDown(e);
			e.Handled = BarNameScope.GetService<ICustomizationService>(this).ShowCustomizationMenu(this);
		}
		protected virtual void OnDragWidgetDoubleClick(object sender, MouseButtonEventArgs e) {
			Bar.IsCollapsed = !Bar.IsCollapsed;
		}		
		protected void CheckContinueDragBar() {
			if(Bar == null)
				return;
			if(!Bar.DockInfo.ContinueDragging)
				return;
			Bar.DockInfo.ContinueDragging = false;
			if(Bar.DockInfo.Container.IsFloating)
				return;
			if(DragWidget != null) {
				Dispatcher.BeginInvoke(new DragDelegate(DragWidget.StartDrag), DispatcherPriority.Normal, new object[] { });
			}
		}
		protected internal virtual void UpdateVisualState() {
			if(!Bar.IsMainMenu && !Bar.IsStatusBar) {
				VisualStateManager.GoToState(this, "Bar", false);
			} else if(Bar.IsMainMenu) {
				VisualStateManager.GoToState(this, "MainMenu", false);
			} else {
				VisualStateManager.GoToState(this, "StatusBar", false);
			}
		}
		protected internal virtual void UpdateItemsSource(BarItemLinkCollection itemLinks) {
			if(itemLinks != null && itemLinks.Count != 0)
				if(ItemsSource != null && ItemsSource is BarItemLinkInfoCollection &&
				   (ItemsSource as BarItemLinkInfoCollection).Source != null &&
				   (ItemsSource as BarItemLinkInfoCollection).Source == itemLinks)
					return;
			if(ItemsSource != null) ClearControlItemLinkCollection(ItemsSource as BarItemLinkInfoCollection);
			if(itemLinks != null) ItemsSource = new BarItemLinkInfoCollection(itemLinks);
			if(!IsUnloaded) CalculateMaxGlyphSize();
		}
		protected internal virtual void UpdateBarControlProperties() {
			if(Bar.IsMainMenu) {
				ContainerType = LinkContainerType.MainMenu;				
			} else if(Bar.IsStatusBar) {
				ContainerType = LinkContainerType.StatusBar;				
			} else {
				ContainerType = LinkContainerType.Bar;				
			}
			ActualShowContent = !Bar.IsCollapsed;
			MinHeight = Bar.IsMainMenu ? 0 : (double)MinHeightProperty.GetMetadata(typeof(LinksControl)).DefaultValue;
			CalcBarCustomizationButonsVisibility();
			UpdateThumb();
		}
		protected internal override void InvalidateMeasurePanel() {
			InvalidateMeasure();
			if(ItemsPresenter == null)
				return;
			Panel panel = null;
			if(VisualTreeHelper.GetChildrenCount(ItemsPresenter) != 0)
				panel = VisualTreeHelper.GetChild(ItemsPresenter, 0) as Panel;
			if(panel != null)
				BarLayoutHelper.InvalidateMeasureTree(panel, this);
			else
				BarLayoutHelper.InvalidateMeasureTree(ItemsPresenter, this);
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
		}
		protected override void OnAccessKeyPressed(AccessKeyPressedEventArgs e) {
			base.OnAccessKeyPressed(e);
			bool hasMainMenu = BarNameScope.GetService<IElementRegistratorService>(this).GetElements<IFrameworkInputElement>().OfType<Bar>().Any(x => x.IsMainMenu);
			if ((Bar != null && !Bar.IsMainMenu && hasMainMenu) || (Keyboard.FocusedElement is LinksControl && Keyboard.FocusedElement != this)) {
				e.Scope = this;
				e.Handled = true;
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			var oldvalue = GetAllowQuickCustomizationButton();
			CalcBarCustomizationButonsVisibility();
			var desiredSize = base.MeasureOverride(constraint);
			if (GetAllowQuickCustomizationButton() != oldvalue) {
				CalcBarCustomizationButonsVisibility();
				var tempConstraint = SizeHelper.Infinite;
				if (double.IsInfinity(constraint.Width) && double.IsInfinity(constraint.Height)) { tempConstraint = new Size(double.MaxValue, double.MaxValue); }
				base.MeasureOverride(tempConstraint);
				desiredSize = base.MeasureOverride(constraint);
			}
			double defaultWidth = desiredSize.Width - ItemsPresenter.DesiredSize.Width;
			if(defaultWidth < 0)
				defaultWidth = desiredSize.Width;
			if(Bar != null)
				Bar.DefaultBarSize = new Size(defaultWidth, desiredSize.Height);
			return desiredSize;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {			
			base.PrepareContainerForItemOverride(element, item);
			BarItemLinkControl lc = (element as BarItemLinkInfo).LinkControl as BarItemLinkControl;
			if(lc != null)
				lc.IsOnBar = true;
		}		
		protected override void OnLinksHolderChanged(DependencyPropertyChangedEventArgs e) {
			base.OnLinksHolderChanged(e);
			Bar prev = e.OldValue as Bar;
			if(prev != null) {
				UpdateItemsSource(null);
				ClearControlItemLinkCollection(ItemsSource as BarItemLinkInfoCollection);
				prev.DockInfo.BarControl = null;
			}
			Bar bar = e.NewValue as Bar;
			if (bar != null) {
				bar.DockInfo.BarControl = this;
				UpdateItemsSource(((ILinksHolder)bar).ActualLinks);
				UpdateBarControlProperties();
				BindingOperations.SetBinding(this, BarItemDisplayModeProperty, new Binding() { Source = bar, Path = new PropertyPath(Bar.BarItemDisplayModeProperty) });
			}
		}
		void OnBarItemDisplayModeChanged(BarItemDisplayMode oldValue) {
			UpdateItemsDisplayMode();
		}
		protected internal virtual void UpdateItemsDisplayMode() {
			foreach (BarItemLinkInfo linkInfo in ItemsSource) {
				(linkInfo.LinkControl as BarItemLinkControl).Do(x => x.UpdateActualBarItemDisplayMode());
			}
		}		
		DragWidget dragWidget;
		BarQuickCustomizationButton quickCustomizationButton;
		Thumb thumb;		
		delegate void DragDelegate();
		bool IsUnloaded { get; set; }
		Bar IBarLayoutTableInfo.Bar { get { return Bar; } }
		void IBarLayoutTableInfo.InvalidateMeasure() { InvalidateMeasure(); }
		void IBarLayoutTableInfo.Measure(Size constraint) { Measure(constraint); }
		void IBarLayoutTableInfo.Arrange(Rect finalRect) { Arrange(finalRect); }
		bool IBarLayoutTableInfo.CanDock(Dock dock) {
			if (Bar != null) {
				if (Bar.IsMainMenu)
					return dock != Dock.Top;
				if (Bar.IsStatusBar)
					return dock != Dock.Bottom;
			}
			return true;
		}
		bool IBarLayoutTableInfo.CanReduce { get { return CanReduce(); } }
		bool IBarLayoutTableInfo.UseWholeRow { get { return Bar.If(x => x.UseWholeRow == DevExpress.Utils.DefaultBoolean.True || (x.IsMainMenu || x.IsStatusBar) && x.UseWholeRow == DevExpress.Utils.DefaultBoolean.Default) != null; } }
		int IBarLayoutTableInfo.Row {
			get { return Bar.Return(x => x.DockInfo.Row, () => 0); }
			set {
				if (Bar == null)
					return;
				Bar.DockInfo.Row = value;
			}
		}
		int IBarLayoutTableInfo.Column {
			get { return Bar.Return(x => x.DockInfo.Column, () => 0); }
			set {
				if (Bar == null)
					return;
				Bar.DockInfo.Column = value;
			}
		}
		int IBarLayoutTableInfo.CollectionIndex { get { return GetCollectionIndex(); } }
		double IBarLayoutTableInfo.Offset { get { return Bar.Return(x => x.DockInfo.Offset, () => 0d); } set { if (Bar == null) return; Bar.DockInfo.Offset = value; } }
		Size IBarLayoutTableInfo.DesiredSize { get { return DesiredSize; } }
		Size IBarLayoutTableInfo.RenderSize { get { return RenderSize;  } }		
		bool IBarLayoutTableInfo.MakeFloating() {
			DragWidget.CancelDrag();
			DockInfo.Data.Save();
			DockInfo.FloatBarOffset = Mouse.GetPosition(Bar.With(BarNameScope.GetScope).With(x => x.Target as UIElement) ?? Window.GetWindow(Bar));
			DockInfo.MakeBarFloating(true);			
			return true;
		}
		EventHandler layoutPropertyChanged;
		event EventHandler IBarLayoutTableInfo.LayoutPropertyChanged {
			add { layoutPropertyChanged += value; }
			remove { layoutPropertyChanged -= value; }
		}
		int GetCollectionIndex() {
			if (Bar == null)
				return 0;
			if (Bar.IsMainMenu)
				return Int32.MinValue;
			if (Bar.IsStatusBar)
				return Int32.MaxValue;
			return ((IBar)Bar).Index;
		}
		#region INavigationOwner
		static List<BarContainerType> barContainerTypes = new List<BarContainerType> { BarContainerType.Top, BarContainerType.Left, BarContainerType.Right, BarContainerType.Bottom };
		protected override bool GetCanEnterMenuMode() {			
			if (Bar == null || !IsVisible)
				return false;
			if (Bar.IsMainMenu)
				return true;
			var bars = BarNameScope.GetService<IElementRegistratorService>(Bar).GetElements<IFrameworkInputElement>(ScopeSearchSettings.Local).OfType<Bar>();
			var hasMainMenu = bars.Where(x => x.IsMainMenu).Any();
			if (hasMainMenu)
				return false;
			var topBars = BarNameScope.GetService<IElementRegistratorService>(Bar).GetElements<IFrameworkInputElement>(ScopeSearchSettings.Ancestors).OfType<Bar>();
			if (topBars.Any())
				return false;
			var index = barContainerTypes.IndexOf(this.DockInfo.ContainerType);
			if (index == -1)
				return false;
			if (bars.Any(x => barContainerTypes.IndexOf(x.DockInfo.ContainerType) < index))
				return false;
			var tb = bars.Select(x => x.DockInfo.BarControl).Where(x=>x.IsVisible).Select(x => new { BC = x, Depth = x.VisualParents().Count() }).OrderBy(x => x.Depth).ToList();
			var depth = this.VisualParents().Count();
			if (tb[0].Depth < depth)
				return false;
			var np = GetNavigationParent() as INavigationOwner;
			if (np == null)
				return true;
			return np.Elements.IndexOf(this) == 0;
		}
		protected override IBarsNavigationSupport GetNavigationParent() { return ScopeNavigationOwner.GetOwner(this); }
		protected override Orientation GetNavigationOrientation() { return ContainerOrientation; }
		protected override NavigationKeys GetNavigationKeys() { return NavigationKeys.All; }
		protected override KeyboardNavigationMode GetNavigationMode() { return KeyboardNavigationMode.Cycle; }		
		INavigationOwner INavigationElement.BoundOwner { get { return this; } }		
		bool INavigationElement.IsSelected { get { return false; } set { } }
		protected override int GetNavigationID() { return GetHashCode(); }
		bool INavigationElement.ProcessKeyDown(KeyEventArgs e) { return false; }
		bool IBarsNavigationSupport.ExitNavigationOnMouseUp { get { return true; } }
		protected override IList<INavigationElement> GetNavigationElements() {
			var baseElements = base.GetNavigationElements()
				.OfType<BarItemLinkInfo>()
				.Select((x, i) => new { Index = i, Info = x })
				.OrderBy(x => GetNavigationElementIngex(x.Info))
				.ThenBy(x => x.Index)
				.Select(x => x.Info)
				.OfType<INavigationElement>()
				.ToList();
			CalcBarCustomizationButonsVisibility();
			if (QuickCustomizationButton == null || !ActualShowQuickCustomizationButton)
				return baseElements;
			return baseElements.Concat(new INavigationElement[] { QuickCustomizationButton }).ToList();
		}
		int GetNavigationElementIngex(BarItemLinkInfo info) {
			var linkAlignment = info.Link.Return(x => x.Alignment, () => BarItemAlignment.Default);
			var itemAlignment = info.Item.Return(x => x.Alignment, () => BarItemAlignment.Default);
			var actualAlignment = linkAlignment == BarItemAlignment.Default ? itemAlignment : linkAlignment;
			if (actualAlignment == BarItemAlignment.Far)
				return 1;
			return 0;
		}
		#endregion
	}  
}

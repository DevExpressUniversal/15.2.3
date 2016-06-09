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

using DevExpress.Data.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity.Internal;
using DevExpress.Xpf.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Interop;
namespace DevExpress.Xpf.Editors.Flyout.Native {
	public partial class FlyoutBase : ContentControl {
		public static readonly DependencyProperty HorizontalOffsetProperty;
		public static readonly DependencyProperty IsOpenProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ActualIsOpenProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty IndicatorDirectionProperty;
		public static readonly DependencyProperty PlacementTargetProperty;
		public static readonly DependencyProperty StaysOpenProperty;
		public static readonly DependencyProperty VerticalOffsetProperty;
		public static readonly DependencyProperty AllowOutOfScreenProperty;
		public static readonly DependencyProperty AllowMoveAnimationProperty;
		FlyoutPositionCalculator positionCalculator;
		FlyoutStrategy strategy;
		protected FlyoutPositionCalculator PositionCalculator {
			get { return positionCalculator ?? (positionCalculator = ActualSettings.CreatePositionCalculator()); }
		}
		internal protected FlyoutStrategy Strategy {
			get { return strategy ?? (strategy = ActualSettings.CreateStrategy()); }
		}
		protected UIElement PopupChild { get { return FlyoutContainer.Return(x => x.Child, null); } }
		protected internal Popup Popup { get; private set; }
		protected internal FlyoutContainer FlyoutContainer { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Storyboard CurrentAnimation { get; protected set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool AnimationInProgress { get { return CurrentAnimation != null; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FrameworkElement ChildContainer { get; protected set; }
		public bool IsLoadedInternal { get; protected set; }
		public event EventHandler Closed;
		public event EventHandler Opened;
		public event EventHandler PopupLoaded;
		internal bool IsReady { get; set; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool FollowTarget { get; set; }
		public static readonly DependencyProperty TargetBoundsProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ContainerStyleProperty;
		public static readonly DependencyProperty SettingsProperty;
		public static readonly DependencyProperty FlyoutProperty;
		public static readonly DependencyProperty AnimationDurationProperty;
		public static readonly DependencyProperty AlwaysOnTopProperty;
		FlyoutAnimatorBase animator;
		FlyoutSettingsBase defaultSetting;
		bool isInitialized;
		static FlyoutBase() {
			Type ownerType = typeof(FlyoutBase);
			HorizontalOffsetProperty = DependencyPropertyManager.Register("HorizontalOffset", typeof(double), ownerType, new PropertyMetadata((d, e) => ((FlyoutBase)d).HorizontalOffsetChanged()));
			VerticalOffsetProperty = DependencyPropertyManager.Register("VerticalOffset", typeof(double), ownerType, new PropertyMetadata((d, e) => ((FlyoutBase)d).VerticalOffsetChanged()));
			IsOpenProperty = DependencyPropertyManager.Register("IsOpen", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((FlyoutBase)d).IsOpenChanged((bool)e.NewValue)));
			ActualIsOpenProperty = DependencyPropertyManager.Register("ActualIsOpen", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((FlyoutBase)d).ActualIsOpenChanged((bool)e.NewValue)));
			PlacementTargetProperty = DependencyPropertyManager.Register("PlacementTarget", typeof(UIElement), ownerType, new PropertyMetadata((d, e) => ((FlyoutBase)d).PlacementTargetChanged(e)));
			StaysOpenProperty = DependencyPropertyManager.Register("StaysOpen", typeof(bool), ownerType, new PropertyMetadata(true));
			AllowOutOfScreenProperty = DependencyPropertyManager.Register("AllowOutOfScreen", typeof(bool), ownerType, new PropertyMetadata(false));
			IndicatorDirectionProperty = DependencyPropertyManager.Register("IndicatorDirection", typeof(IndicatorDirection), ownerType, new PropertyMetadata(IndicatorDirection.None));
			ContainerStyleProperty = DependencyPropertyManager.Register("ContainerStyle", typeof(Style), ownerType);
			SettingsProperty = DependencyPropertyManager.Register("Settings", typeof(FlyoutSettingsBase), ownerType, new PropertyMetadata(null));
			TargetBoundsProperty = DependencyPropertyManager.Register("TargetBounds", typeof(Rect), ownerType, new PropertyMetadata(Rect.Empty, (d, e) => ((FlyoutBase)d).TargetBoundsChanged(e)));
			FlyoutProperty = DependencyProperty.RegisterAttached("Flyout", typeof(FlyoutBase), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			AnimationDurationProperty = DependencyProperty.Register("AnimationDuration", typeof(Duration), ownerType, new FrameworkPropertyMetadata(new Duration(TimeSpan.FromMilliseconds(500d))));
			AlwaysOnTopProperty = DependencyProperty.Register("AlwaysOnTop", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowMoveAnimationProperty = DependencyProperty.Register("AllowMoveAnimation", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetFlyout(DependencyObject d, FlyoutBase flyout) {
			d.SetValue(FlyoutProperty, flyout);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static FlyoutBase GetFlyout(DependencyObject d) {
			return (FlyoutBase)d.GetValue(FlyoutProperty);
		}
		public FlyoutBase() {
			OpenCommand = CreateOpenCommand();
			CloseCommand = CreateCloseCommand();
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			SetFlyout(this, this);
		}
		public double HorizontalOffset {
			get { return (double)GetValue(HorizontalOffsetProperty); }
			set { SetValue(HorizontalOffsetProperty, value); }
		}
		public double VerticalOffset {
			get { return (double)GetValue(VerticalOffsetProperty); }
			set { SetValue(VerticalOffsetProperty, value); }
		}
		public IndicatorDirection IndicatorDirection {
			get { return (IndicatorDirection)GetValue(IndicatorDirectionProperty); }
			internal protected set { SetValue(IndicatorDirectionProperty, value); }
		}
		public bool ActualIsOpen {
			get { return (bool)GetValue(ActualIsOpenProperty); }
			protected set { SetValue(ActualIsOpenProperty, value); }
		}
		public bool IsOpen {
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value); }
		}
		public UIElement PlacementTarget {
			get { return (UIElement)GetValue(PlacementTargetProperty); }
			set { SetValue(PlacementTargetProperty, value); }
		}
		public bool StaysOpen {
			get { return (bool)GetValue(StaysOpenProperty); }
			set { SetValue(StaysOpenProperty, value); }
		}
		public bool AllowOutOfScreen {
			get { return (bool)GetValue(AllowOutOfScreenProperty); }
			set { SetValue(AllowOutOfScreenProperty, value); }
		}
		public FlyoutSettingsBase ActualSettings {
			get { return Settings ?? (defaultSetting = CreateDefaultSettings()); }
		}
		public FlyoutSettingsBase Settings {
			get { return (FlyoutSettingsBase)GetValue(SettingsProperty); }
			set { SetValue(SettingsProperty, value); }
		}
		public Style ContainerStyle {
			get { return (Style)GetValue(ContainerStyleProperty); }
			set { SetValue(ContainerStyleProperty, value); }
		}
		public Duration AnimationDuration {
			get { return (Duration)GetValue(AnimationDurationProperty); }
			set { SetValue(AnimationDurationProperty, value); }
		}
		public bool AlwaysOnTop {
			get { return (bool)GetValue(AlwaysOnTopProperty); }
			set { SetValue(AlwaysOnTopProperty, value); }
		}
		public bool AllowMoveAnimation {
			get { return (bool)GetValue(AllowMoveAnimationProperty); }
			set { SetValue(AllowMoveAnimationProperty, value); }
		}
		public Rect TargetBounds {
			get { return (Rect)GetValue(TargetBoundsProperty); }
			set { SetValue(TargetBoundsProperty, value); }
		}
		public bool IsInitializedInternal {
			get { return isInitialized; }
			protected set {
				isInitialized = value;
				if (isInitialized)
					OnInitialized();
			}
		}
		public bool AllowRecreateContent { get; set; }
		public ICommand CloseCommand { get; protected set; }
		public ICommand OpenCommand { get; protected set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FrameworkElement RenderGrid { get; protected set; }
		protected FlyoutAnimatorBase Animator {
			get {
				if (animator == null)
					animator = ActualSettings.CreateAnimator();
				return animator;
			}
		}
		protected virtual FlyoutContainer GetFlyoutContainer() {
			Popup = GetPopup();
			if (Popup != null)
				return new PopupFlyoutContainer() { Popup = Popup };
			return null;
		}
		protected virtual Popup GetPopup() {
			return GetTemplateChild("PART_Popup") as Popup;
		}
		protected virtual FrameworkElement GetChildContainer() {
			return (FrameworkElement)GetTemplateChild("PART_Container");
		}
		protected virtual void OnPopupChildContainerLostFocus(object sender, RoutedEventArgs e) {
		}
		protected virtual void OnPopupChildContainerGotFocus(object sender, RoutedEventArgs e) {
		}
		void OnPopupChildContainerLoaded(object sender, RoutedEventArgs e) {
			IsLoadedInternal = true;
			PopupLoaded.Do(x => x(sender, new EventArgs()));
		}
		void OnPopupChildContainerUnloaded(object sender, RoutedEventArgs e) {
			IsLoadedInternal = false;
		}
		protected virtual void OnPopupClosed(object sender, object e) {
			OnClosed(EventArgs.Empty);
			RaiseClosed();
		}
		protected virtual void OnPopupOpened(object sender, object e) {
			OnOpened(EventArgs.Empty);
			RaiseOpened();
		}
		System.Windows.Forms.Form subscribedForm;
		FrameworkElement subscribedRoot;
		Window subscribedWindow;
		protected virtual void SubscribeWindowCore() {
			subscribedRoot = LayoutHelper.GetTopLevelVisual(this);
			if (subscribedRoot == null)
				return;
			subscribedRoot.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(RootMouseDown), true);
			subscribedRoot.KeyDown += RootKeyDown;
			subscribedWindow = subscribedRoot as Window ?? Window.GetWindow(this);
			if (subscribedWindow != null) {
				subscribedWindow.StateChanged += WindowStateChanged;
				subscribedWindow.LocationChanged += WindowLocationChanged;
				subscribedWindow.SizeChanged += WindowSizeChanged;
				if (ActivatedHandler == null) {
					CreateActivatedHandlers();
				}
				subscribedWindow.Activated += ActivatedHandler.Handler;
				subscribedWindow.Deactivated += DeactivatedHandler.Handler;
			}
			else {
				var hwnd = (HwndSource)PresentationSource.FromDependencyObject(this);
				if (hwnd != null) {
					var host = System.Windows.Forms.Control.FromChildHandle(hwnd.Handle);
					if (host != null) {
						subscribedForm = host.TopLevelControl as System.Windows.Forms.Form;
						if (subscribedForm != null) {
							subscribedForm.LocationChanged += WindowLocationChanged;
							subscribedForm.SizeChanged += WindowSizeChanged;
							if (ActivatedHandler == null) {
								CreateActivatedHandlers();
							}
							subscribedForm.Activated += ActivatedHandler.Handler;
							subscribedForm.Deactivate += DeactivatedHandler.Handler;
						}
					}
				}
			}
		}
		void WindowStateChanged(object sender, EventArgs e) {
			if (CheckWindowState(WindowState.Minimized))
				HideOnWindowDeactivated();
			if (CheckWindowState(WindowState.Maximized) || CheckWindowState(WindowState.Normal))
				ShowOnWindowActivated();
		}
		protected virtual void UnsubscribeWindowCore() {
			if (subscribedRoot != null) {
				subscribedRoot.RemoveHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(RootMouseDown));
				subscribedRoot.KeyDown -= RootKeyDown;
				subscribedRoot = null;
			}
			if (subscribedWindow != null) {
				subscribedWindow.StateChanged -= WindowStateChanged;
				subscribedWindow.LocationChanged -= WindowLocationChanged;
				subscribedWindow.SizeChanged -= WindowSizeChanged;
				subscribedWindow.Activated -= ActivatedHandler.Handler;
				subscribedWindow.Deactivated -= DeactivatedHandler.Handler;
				subscribedWindow = null;
			}
			if (subscribedForm != null) {
				subscribedForm.LocationChanged -= WindowLocationChanged;
				subscribedForm.SizeChanged -= WindowSizeChanged;
				subscribedForm.Activated -= ActivatedHandler.Handler;
				subscribedForm.Deactivate -= DeactivatedHandler.Handler;
				subscribedForm = null;
			}
		}
		void WindowSizeChanged(object sender, EventArgs e) {
			InvalidateLocation();
		}
		void CreateActivatedHandlers() {
			ActivatedHandler = new WeakEventHandler<FlyoutBase, EventArgs, EventHandler>(this,
				(flyout, sender, handler) => flyout.ShowOnWindowActivated(),
				(h, sender) => (sender as Window).Activated -= h.Handler,
				h => h.OnEvent);
			DeactivatedHandler = new WeakEventHandler<FlyoutBase, EventArgs, EventHandler>(this,
				(flyout, sender, handler) => flyout.HideOnWindowDeactivated(),
				(h, sender) => (sender as Window).Deactivated -= h.Handler,
				h => h.OnEvent);
		}
		WeakEventHandler<FlyoutBase, EventArgs, EventHandler> ActivatedHandler { get; set; }
		WeakEventHandler<FlyoutBase, EventArgs, EventHandler> DeactivatedHandler { get; set; }
		Nullable<bool> isOpen;
		protected virtual void HideOnWindowDeactivated() {
			if (!AlwaysOnTop && !isOpen.HasValue) {
				if (StaysOpen)
					this.isOpen = IsOpen;
				SetIsOpen(false);
			}
		}
		protected virtual void ShowOnWindowActivated() {
			if (!AlwaysOnTop && isOpen.HasValue && StaysOpen) {
				bool isOpenValue = this.isOpen.Value;
				isOpen = null;
				Dispatcher.BeginInvoke(new Action(() => {
					SetIsOpen(isOpenValue);
				}));
			}
		}
		protected virtual void WindowLocationChanged(object sender, EventArgs e) {
			InvalidateLocation();
		}
		protected void ProcessCloseOnPointerPressed() {
			if (!StaysOpen)
				SetIsOpen(false);
		}
		protected virtual void ChildChanged() {
		}
		protected virtual void VerticalOffsetChanged() {
			InvalidateLocation();
		}
		protected virtual void HorizontalOffsetChanged() {
			InvalidateLocation();
		}
		protected virtual void RaiseClosed() {
			if (Closed != null)
				Closed(this, EventArgs.Empty);
		}
		protected virtual void RaiseOpened() {
			if (Opened != null)
				Opened(this, EventArgs.Empty);
		}
		protected Point CalcOffset(FrameworkElement element) {
			return element.TransformToVisual(element.GetRootVisual()).Transform(new Point());
		}
		public virtual void UpdatePopupPlacement() {
			if (!IsPopupVisible)
				return;
			if (PlacementTarget != null && !IsConnectedToPresentationSource(PlacementTarget)) {
				SetIsOpen(false);
				return;
			}
			CalcPopupBounds();
			if (PositionCalculator.Result.State == CalculationState.Finished) {
				UpdateIndicator();
				UpdatePopupLocation(GetLocation());
				UpdatePopupSize(PositionCalculator.Result.Size);
				UpdatePopupControls();
			}
		}
		bool CheckWindowState(WindowState state) {
			Window window = subscribedRoot as Window;
			return window != null && window.WindowState == state;
		}
		protected bool IsConnectedToPresentationSource(UIElement element) {
			return PresentationSource.FromVisual(element) != null;
		}
		private void UpdateIndicator() {
			FrameworkElement indicator = GetIndicator(IndicatorDirection);
			if (indicator == null)
				return;
			Point offset = PositionCalculator.Result.IndicatorOffset;
			TranslateTransform transform = indicator.RenderTransform as TranslateTransform;
			if (transform == null) {
				transform = new TranslateTransform();
				indicator.RenderTransform = transform;
			}
			transform.X = offset.X;
			transform.Y = offset.Y;
		}
		public FrameworkElement GetIndicator(IndicatorDirection indicatorDirection) {
			if (indicatorDirection == Native.IndicatorDirection.None)
				return null;
			return LayoutHelper.FindElementByName(RenderGrid, indicatorDirection.ToString() + "Indicator");
		}
		public virtual void UpdatePopupControls() {
#if !SL
			RenderGrid.Opacity = 0d;
			Rect screenRect = ScreenRect;
			FrameworkElement popupRoot = PopupRoot;
			if (popupRoot != null && popupRoot.GetType().FullName == "System.Windows.Controls.Primitives.PopupRoot") {
				popupRoot.Width = screenRect.Width;
				popupRoot.Height = screenRect.Height;
			}
			FlyoutContainer.VerticalOffset = screenRect.Top;
			FlyoutContainer.HorizontalOffset = screenRect.Left;
#endif
		}
		protected virtual void UpdatePopupLocation(Point location) {
			if (!IsPopupVisible || !IsValid(location))
				return;
			Point popupOffset = NormalizeToScreenOrigin(location);
			FrameworkElement contentControl = (FrameworkElement)GetTemplateChild("PART_cc");
			if (contentControl == null)
				return;
			FrameworkElement owner = DevExpress.Xpf.Core.Native.LayoutHelper.GetTopLevelVisual(this);
			if (owner != null) {
				MatrixTransform transform = this.TransformToVisual(owner) as MatrixTransform;
				Matrix matrix = transform.Matrix;
				matrix.Invert();
				transform = new MatrixTransform(Math.Abs(matrix.M11), matrix.M12, matrix.M21, Math.Abs(matrix.M22), 0, 0);
				if (!transform.Matrix.IsIdentity) {
					popupOffset = transform.Transform(popupOffset);
					contentControl.LayoutTransform = transform;
				}
			}
			Canvas.SetLeft(contentControl, popupOffset.X + HorizontalOffset);
			Canvas.SetTop(contentControl, popupOffset.Y + VerticalOffset);
		}
		protected virtual Point NormalizeToScreenOrigin(Point point) {
			Rect screenRect = ScreenRect;
			if (FlowDirection == System.Windows.FlowDirection.RightToLeft) {
				return new Point(screenRect.Right - point.X, point.Y - screenRect.Top);
			}
			point.Offset(-screenRect.Left, -screenRect.Top);
			return point;
		}
		Size MeasurePopup() {
			if (!IsPopupVisible)
				return Size.Empty;
			FrameworkElement renderElement = GetMeasureElement();
			renderElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			return renderElement.DesiredSize;
		}
		protected virtual bool IsPopupVisible {
			get {
				if (!IsConnectedToPresentationSource(this) ||
					(System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) && InteractionHelper.GetBehaviorInDesignMode(this) == InteractionBehaviorInDesignMode.Default))
					return false;
				return IsOpen && FlyoutContainer != null && ChildContainer != null;
			}
		}
		protected void CalcPopupBounds() {
			Rect targetBounds = ScreenHelper.GetScaledRect(TranslateHelper.ToScreen(PlacementTarget, GetTargetBounds()));
			PositionCalculator.Initialize(targetBounds, VerticalAlignment, HorizontalAlignment, AllowOutOfScreen);
			ActualSettings.Apply(PositionCalculator, this);
			if (IsPopupVisible) {
				PositionCalculator.PopupDesiredSize = MeasurePopup();
				PositionCalculator.IndicatorSize = GetIndicator(IndicatorDirection).Return(x => x.DesiredSize, () => new Size());
				PositionCalculator.CalcLocation();
				IndicatorDirection = ActualSettings.GetIndicatorDirection(PositionCalculator.Result.Placement);
			}
		}
		FrameworkElement Root { get { return LayoutHelper.GetTopLevelVisual(this) ?? this; } }
		protected bool IsValid(Point p) {
			return !(double.IsInfinity(p.X) || double.IsInfinity(p.Y) || double.IsNaN(p.X) || double.IsNaN(p.Y));
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			SubscribeWindowCore();
			SubscribeSettings(ActualSettings);
		}
		private void SubscribeSettings(FlyoutSettingsBase settings) {
			settings.Do(x => {
				x.PropertyChanged += SettingsPropertyChanged;
				InitializeSettings();
			});
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeWindowCore();
			UnsubscribeSettings(ActualSettings);
			SetIsOpen(false);
		}
		private void UnsubscribeSettings(FlyoutSettingsBase settings) {
			settings.Do(x => x.PropertyChanged -= SettingsPropertyChanged);
		}
		void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e) {
			ActualSettings.Do(x => x.OnPropertyChanged(this, e));
		}
		protected virtual FlyoutSettings CreateDefaultSettings() {
			return new FlyoutSettings();
		}
		protected virtual ICommand CreateOpenCommand() {
			return DelegateCommandFactory.Create<object>(
				delegate { SetIsOpen(true); }
				, CanExecuteOpenCommand, false);
		}
		protected virtual ICommand CreateCloseCommand() {
			return DelegateCommandFactory.Create<object>(
				delegate { SetIsOpen(false); }
				, CanExecuteCloseCommand, false);
		}
		protected virtual bool CanExecuteOpenCommand(object parameter) {
			return !IsOpen;
		}
		protected virtual bool CanExecuteCloseCommand(object parameter) {
			return IsOpen;
		}
		protected virtual void ProcessKeyDown(Key key) {
			if (key == Key.Escape && !StaysOpen)
				SetIsOpen(false);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (FlyoutContainer != null) {
				FlyoutContainer.Element.KeyDown -= RootKeyDown;
				FlyoutContainer.Element.MouseDown -= RootMouseDown;
				FlyoutContainer.Closed -= OnPopupClosed;
				FlyoutContainer.Opened -= OnPopupOpened;
			}
			FlyoutContainer = GetFlyoutContainer();
			if (FlyoutContainer != null) {
				FlyoutContainer.Element.KeyDown += RootKeyDown;
				FlyoutContainer.Element.MouseDown += RootMouseDown;
				FlyoutContainer.Closed += OnPopupClosed;
				FlyoutContainer.Opened += OnPopupOpened;
			}
			if (ChildContainer != null) {
				ChildContainer.Loaded -= OnPopupChildContainerLoaded;
				ChildContainer.Unloaded -= OnPopupChildContainerUnloaded;
				ChildContainer.LostFocus -= OnPopupChildContainerLostFocus;
				ChildContainer.GotFocus -= OnPopupChildContainerGotFocus;
			}
			ChildContainer = GetChildContainer();
			if (ChildContainer != null) {
				ChildContainer.Loaded += OnPopupChildContainerLoaded;
				ChildContainer.Unloaded += OnPopupChildContainerUnloaded;
				ChildContainer.LostFocus += OnPopupChildContainerLostFocus;
				ChildContainer.GotFocus += OnPopupChildContainerGotFocus;
			}
			if (RenderGrid != null) {
				RenderGrid.SizeChanged -= RenderGrid_SizeChanged;
			}
			RenderGrid = GetRenderGrid();
			if (RenderGrid != null) {
				RenderGrid.SizeChanged += RenderGrid_SizeChanged;
			}
			IsInitializedInternal = true;
		}
		protected virtual void RootMouseDown(object sender, MouseButtonEventArgs e) {
			if (PopupChild == null)
				return;
			DependencyObject source = e.OriginalSource as DependencyObject;
			if (source == null)
				return;
			DependencyObject obj = LayoutHelper.FindLayoutOrVisualParentObject(source, element => element == this, true, this.GetParent());
			if (obj == this)
				return;
			ProcessCloseOnPointerPressed();
		}
		protected virtual void RootKeyDown(object sender, KeyEventArgs e) {
			ProcessKeyDown(e.Key);
		}
		protected virtual void RenderGrid_SizeChanged(object sender, SizeChangedEventArgs e) {
			InvalidateLocation();
		}
		void OnInitialized() {
			InitializeSettings();
			OpenPopup();
		}
		private void InitializeSettings() {
			this.positionCalculator = null;
			this.strategy = null;
			this.animator = null;
			InvalidateLocation();
		}
		protected virtual void TargetBoundsChanged(DependencyPropertyChangedEventArgs e) {
			InvalidateLocation();
		}
		Nullable<Point> location = null;
		public void UpdateLocation() {
			if (!IsPopupVisible) {
				location = null;
				return;
			}
			UpdatePopupPlacement();
			if (!AllowMoveAnimation || PositionCalculator.Result.State != CalculationState.Finished)
				return;
			Point newLocation = GetLocation();
			if (location == null) {
				location = newLocation;
				return;
			}
			Point oldLocation = location.Value;
			location = newLocation;
			double x = oldLocation.X - newLocation.X;
			AnimateMove(new Point(FlowDirection == FlowDirection.RightToLeft ? -x : x, oldLocation.Y - newLocation.Y), new Point());
		}
		Point GetLocation() {
			if (PositionCalculator.Result.State != CalculationState.Finished)
				throw new Exception();
			Rect popupRect = new Rect(
					new Point(PositionCalculator.Result.Location.X + PositionCalculator.Result.Size.Width, PositionCalculator.Result.Location.Y),
					new Point(PositionCalculator.Result.Location.X, PositionCalculator.Result.Location.Y + PositionCalculator.Result.Size.Height));
			return FlowDirection == System.Windows.FlowDirection.RightToLeft ? popupRect.TopRight : popupRect.TopLeft;
		}
		public void InvalidateLocation() {
			if (!IsReady)
				return;
			IsReady = false;
			InvalidateArrange();
		}
		protected virtual void PlacementTargetChanged(DependencyPropertyChangedEventArgs e) {
			InvalidateLocation();
			(e.OldValue as FrameworkElement).Do(x => x.LayoutUpdated -= TargetLayoutUpdated);
			(e.NewValue as FrameworkElement).Do(x => x.LayoutUpdated += TargetLayoutUpdated);
		}
		void TargetLayoutUpdated(object sender, EventArgs e) {
			if (!FollowTarget)
				return;
			UpdateLocation();
		}
		public virtual Rect GetTargetBounds() {
			return !TargetBounds.IsEmpty ? TargetBounds : GetTargetBounds(PlacementTarget, PlacementTarget, () => Strategy.GetDefaultTargetBounds(this));
		}
		public virtual Rect GetTargetBounds(UIElement baseElement, UIElement element, Func<Rect> GetDefaultTargetBounds) {
			return element == null ? GetDefaultTargetBounds() : TranslateHelper.TranslateBounds(baseElement, element);
		}
		protected virtual FrameworkElement GetRenderGrid() {
			return (FrameworkElement)GetTemplateChild("PART_RenderGrid");
		}
		protected virtual void UpdatePopupSize(Size size) {
			if (Settings == null)
				return;
			Strategy.UpdatePopupSize(this, size);
		}
		internal protected FrameworkElement PopupRoot { get { return LayoutHelper.GetTopLevelVisual(ChildContainer); } }
		protected Rect ScreenRect { get { return PositionCalculator.ScreenRect; } }
		protected virtual void ActualIsOpenChanged(bool newValue) {
			SetIsOpen(newValue);
			(CloseCommand as DelegateCommand<object>).Do(x => x.RaiseCanExecuteChanged());
			(OpenCommand as DelegateCommand<object>).Do(x => x.RaiseCanExecuteChanged());
		}
		void SetIsOpen(bool value) {
			SetCurrentValue(IsOpenProperty, value);
		}
		protected virtual void IsOpenChanged(bool newValue) {
			if (newValue)
				OpenPopup();
			else
				ClosePopup();
		}
		protected virtual FlyoutAnimatorBase CreateAnimator() {
			return new FlyoutAnimator();
		}
		protected virtual Point GetOpenAnimationOffset() {
			UpdatePopupPlacement();
			return Strategy.GetOpenAnimationOffset(this);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			if (!IsReady) {
				UpdateLocation();
				IsReady = true;
			}
			return base.ArrangeOverride(arrangeBounds);
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == HorizontalAlignmentProperty || e.Property == VerticalAlignmentProperty)
				InvalidateLocation();
			if (e.Property == SettingsProperty && IsLoaded) {
				UnsubscribeSettings(e.OldValue as FlyoutSettingsBase);
				SubscribeSettings(e.NewValue as FlyoutSettingsBase);
			}
		}
		protected virtual FrameworkElement GetMeasureElement() {
			return Strategy.GetMeasureElement(this);
		}
		protected virtual void OpenPopup() {
			if (!IsPopupVisible)
				return;
			InvalidateLocation();
			if (IsOpen)
				FlyoutContainer.IsOpen = true;
			PopupOpened();
		}
		protected virtual void OnOpened(EventArgs e) {
			ActualIsOpen = true;
			if (AllowRecreateContent) {
				RecreateContent();
			}
		}
		protected virtual void RecreateContent() {
			Style s = ContainerStyle;
			ContainerStyle = null;
			ContainerStyle = s;
			DataTemplate dataTemplate = ContentTemplate;
			ContentTemplate = null;
			ContentTemplate = dataTemplate;
		}
		protected virtual void OnClosed(EventArgs e) {
			ActualIsOpen = false;
		}
		void PopupOpened() {
			Point offset = GetOpenAnimationOffset();
			if (!IsValid(offset))
				return;
			Storyboard storyboard = Animator.GetOpenAnimation(this, offset);
			Animate(storyboard);
		}
		protected virtual void AnimateMove(Point from, Point to) {
			if (!ActualIsOpen || !IsValid(from) || !IsValid(to) || from == to)
				return;
			Storyboard storyboard = Animator.GetMoveAnimation(this, from, to);
			Animate(storyboard);
		}
		protected virtual void ClosePopup() {
			if (FlyoutContainer == null)
				return;
			Storyboard storyboard = Animator.GetCloseAnimation(this);
			Animate(storyboard, () => {
				if (!IsOpen)
					FlyoutContainer.Do(x => x.IsOpen = false);
			});
		}
		void Animate(Storyboard storyboard, Action completeAction = null) {
			if (storyboard == null)
				return;
			if (AnimationInProgress) {
				CurrentAnimation.SkipToFill(this);
			}
			CurrentAnimation = storyboard;
			storyboard.Completed += (d, e) => {
				completeAction.Do(x => x());
				if (storyboard == CurrentAnimation)
					CurrentAnimation = null;
			};
			storyboard.Begin(this, true);
		}
	}
	public abstract class FlyoutContainer {
		public virtual UIElement Child { get; set; }
		public virtual FrameworkElement Element { get; private set; }
		public virtual double HorizontalOffset { get; set; }
		public virtual double VerticalOffset { get; set; }
		public virtual bool IsOpen { get; set; }
		public event EventHandler Opened {
			add { OpenedSubscribe(value); }
			remove { OpenedUnSubscribe(value); }
		}
		protected virtual void OpenedSubscribe(EventHandler value) { }
		protected virtual void OpenedUnSubscribe(EventHandler value) { }
		public event EventHandler Closed {
			add { ClosedSubscribe(value); }
			remove { ClosedUnSubscribe(value); }
		}
		protected virtual void ClosedSubscribe(EventHandler value) { }
		protected virtual void ClosedUnSubscribe(EventHandler value) { }	
	}
	public class PopupFlyoutContainer : FlyoutContainer {
		public Popup Popup { get; set; }								
		public override FrameworkElement Element {
			get {				
				return Popup;
			}
		}
		public override UIElement Child {
			get {
				return Popup.Child;
			}
			set {
				Popup.Child = value;
			}
		}
		public override double HorizontalOffset {
			get { return Popup.HorizontalOffset; }
			set { Popup.HorizontalOffset = value; } 
		}
		public override double VerticalOffset {
			get { return Popup.VerticalOffset; }
			set { Popup.VerticalOffset = value; }
		}
		public override bool IsOpen {
			get { return Popup.IsOpen; }
			set { Popup.IsOpen = value; }
		}
		protected override void OpenedSubscribe(EventHandler value) {
			Popup.Opened += value;
		}
		protected override void OpenedUnSubscribe(EventHandler value) {
			Popup.Opened -= value;
		}
		protected override void ClosedSubscribe(EventHandler value) {
			Popup.Closed += value;
		}
		protected override void ClosedUnSubscribe(EventHandler value) {
			Popup.Closed -= value;
		}
	}
}

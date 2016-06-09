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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
#if SILVERLIGHT
using System.Windows.Media;
#else
using System.Windows.Interop;
#endif
namespace DevExpress.Xpf.Core {
	public enum ScrollBars { None,  Auto }
	public interface IControl {
		FrameworkElement Control { get; }
		Controller Controller { get; }
		bool IsLoaded { get; }
	}
	public class Controller {
		#region Static
#if SILVERLIGHT
		private static bool AreRootMouseEventsAssigned;
		private static Point _MousePosition = PointHelper.Empty;
		public static Point MousePosition { get { return _MousePosition; } }
		private static void RootMouseLeave(object sender, MouseEventArgs e) {
			_MousePosition = PointHelper.Empty;
		}
		private static void RootMouseMove(object sender, MouseEventArgs e) {
			_MousePosition = e.GetPosition(null);
		}
#endif
		#endregion Static
		public Controller(IControl control) {
			IControl = control;
			CheckScrollParams();
			CheckScrollBars();
			AttachToEvents();
		}
		public virtual IEnumerable<UIElement> GetInternalElements() {
			if (HorzScrollBar != null)
				yield return HorzScrollBar;
			if (VertScrollBar != null)
				yield return VertScrollBar;
			if (CornerBox != null)
				yield return CornerBox;
			if (DragAndDropController != null)
				foreach (UIElement element in DragAndDropController.GetInternalElements())
					yield return element;
		}
		public FrameworkElement Control { get { return IControl.Control; } }
		public IControl IControl { get; private set; }
		public bool IsLoaded { get; private set; }
		protected virtual void AttachToEvents() {
			Control.KeyDown += (sender, e) => ProcessKeyDown(new DXKeyEventArgs(e, args => e.Handled = args.Handled));
			Control.KeyUp += (sender, e) => ProcessKeyUp(new DXKeyEventArgs(e, args => e.Handled = args.Handled));
			Control.MouseEnter += (sender, e) => OnMouseEnter(new DXMouseEventArgs(e));
			Control.MouseLeave += (sender, e) => OnMouseLeave(new DXMouseEventArgs(e));
			Control.MouseLeftButtonDown +=
				delegate(object sender, MouseButtonEventArgs e) {
#if SILVERLIGHT
					if (!IsMouseLeftButtonDown) 
#else
					if (Control.IsInVisualTree())
#endif
						OnMouseLeftButtonDown(new DXMouseButtonEventArgs(e, args => e.Handled = args.Handled));
				};
			Control.MouseLeftButtonUp += (sender, e) => OnMouseLeftButtonUp(new DXMouseButtonEventArgs(e, args => e.Handled = args.Handled));
			Control.AddHandler(UIElement.MouseLeftButtonUpEvent,
				new MouseButtonEventHandler(
					delegate(object sender, MouseButtonEventArgs e) {
						if (!e.Handled)
							return;
						if (IsMouseLeftButtonDown)
							OnMouseLeftButtonUp(new DXMouseButtonEventArgs(e, args => e.Handled = args.Handled));
					}),
				true);
			Control.MouseMove +=
				delegate(object sender, MouseEventArgs e) {
#if !SILVERLIGHT
					if (!IsMouseCaptureChanging && Control.IsInVisualTree())
#endif
						OnMouseMove(new DXMouseEventArgs(e));
				};
			Control.MouseWheel += (sender, e) => OnMouseWheel(new DXMouseWheelEventArgs(e, args => e.Handled = args.Handled));
			Control.TouchDown += (sender, e) => OnTouchDown(e);
			Control.TouchUp += (sender, e) => OnTouchUp(e);
			Control.TouchEnter += (sender, e) => OnTouchEnter(e);
			Control.TouchLeave += (sender, e) => OnTouchLeave(e);
			Control.TouchMove += (sender, e) => OnTouchMove(e);
			Control.LayoutUpdated += (sender, e) => OnLayoutUpdated();
			Control.Loaded += (sender, e) => OnLoaded();
		}
		protected virtual void OnLayoutUpdated() {
			if(IsLoaded && NeedsUnloadedEvent && !Control.IsInVisualTree())
				OnUnloaded();
		}
		protected virtual void OnLoaded() {
			IsLoaded = true;
#if SILVERLIGHT
			if(!AreRootMouseEventsAssigned && Application.Current.RootVisual != null) {
				Application.Current.RootVisual.MouseLeave += RootMouseLeave;
				Application.Current.RootVisual.MouseMove += RootMouseMove;
				AreRootMouseEventsAssigned = true;
			}
#endif
		}
		protected virtual void OnUnloaded() {
			IsLoaded = false;
		}
		protected virtual bool NeedsUnloadedEvent { get { return false; } }
		#region Mouse Capture
		public static UIElement MouseCaptureOwner { get; private set; }
		public bool CaptureMouse(Point? mousePosition = null) {
			if (IsMouseCaptured)
				return true;
			else {
				UIElement prevMouseCaptureOwner = MouseCaptureOwner;
				MouseCaptureOwner = Control;
				bool result;
				IsMouseCaptureChanging = true;
				try {
					result = Control.CaptureMouse() || Control.IsInDesignTool();  
#if SILVERLIGHT
					if (result && mousePosition != null) {
						Point p = Control.MapPoint(mousePosition.Value, null);
						var hitChildren = new List<UIElement>(VisualTreeHelper.FindElementsInHostCoordinates(p, Control));
						foreach (UIElement child in hitChildren)
							if (child is IControl && child != Control)
								((IControl)child).Controller.OnMouseLeave(null);
					}
#endif
				}
				finally {
					IsMouseCaptureChanging = false;
				}
				if (result)
					MouseCaptureOwner.LostMouseCapture += OnLostMouseCapture;
				else
					MouseCaptureOwner = prevMouseCaptureOwner;
				return result;
			}
		}
		public void ReleaseMouseCapture() {
			if (!IsMouseCaptured)
				return;
			MouseCaptureOwner.LostMouseCapture -= OnLostMouseCapture;
			MouseCaptureOwner = null;
			IsMouseCaptureChanging = true;
			try {
				Control.ReleaseMouseCapture();
			}
			finally {
				IsMouseCaptureChanging = false;
			}
		}
		public bool IsMouseCaptured { get { return MouseCaptureOwner == Control; } }
		protected bool IsMouseCaptureChanging { get; set; }
		protected virtual void OnMouseCaptureCancelled() {
			if(IsMouseLeftButtonDown)
				OnMouseLeftButtonUp(null);
		}
		private void OnLostMouseCapture(object sender, MouseEventArgs e) {
			ReleaseMouseCapture();
			OnMouseCaptureCancelled();
		}
		#endregion Mouse Capture
		#region Keyboard, Mouse, and Touch Handling
		private bool _IsMouseEntered;
		private bool _IsMouseLeftButtonDown;
		public bool IsMouseEntered {
			get { return _IsMouseEntered; }
			set {
				if (IsMouseEntered == value)
					return;
				_IsMouseEntered = value;
				OnIsMouseEnteredChanged();
			}
		}
		public bool IsMouseLeftButtonDown {
			get { return _IsMouseLeftButtonDown; }
			private set {
				if(IsMouseLeftButtonDown == value)
					return;
				_IsMouseLeftButtonDown = value;
				OnIsMouseLeftButtonDownChanged();
			}
		}
		protected virtual void OnIsMouseEnteredChanged() {
		}
		protected virtual void OnIsMouseLeftButtonDownChanged() {
		}
		protected virtual void OnKeyDown(DXKeyEventArgs e) {
		}
		protected virtual void OnKeyUp(DXKeyEventArgs e) {
		}
		protected virtual void OnMouseEnter(DXMouseEventArgs e) {
			IsMouseEntered = true;
		}
		protected virtual void OnMouseLeave(DXMouseEventArgs e) {
			IsMouseEntered = false;
			if(!IsMouseCaptured)
				IsMouseLeftButtonDown = false;
			if(IsDragAndDropPreparation)
				DragAndDropPrepared(false, null);
			AutoScrollingDirection = ScrollDirection.None;
		}
		protected virtual void OnMouseMove(DXMouseEventArgs e) {
			if(IsDragAndDropPreparation && !StartDragAreaBounds.Contains(e.GetPosition(Control))) {
				DragAndDropPrepared(true, e.GetPosition(Control));
				Control.UpdateLayout();
			}
			if(IsDragAndDrop)
				DragAndDrop(e.GetPosition(Control));
			CheckAutoScrolling(e.GetPosition(Control));
		}
		protected virtual void OnMouseWheel(DXMouseWheelEventArgs e) {
			if (!e.Handled)
				e.Handled = ProcessMouseWheelScrolling(e.Delta);
		}
		protected virtual void OnMouseDoubleClick(DXMouseButtonEventArgs e) {
		}
		protected virtual void OnMouseLeftButtonDown(DXMouseButtonEventArgs e) {
			IsMouseLeftButtonDown = true;
			CheckMouseDoubleClick(e);
			if (e.Handled)
				return;
			if (WantsDragAndDrop(e.GetPosition(Control), out _DragAndDropController))
				PrepareDragAndDrop(e.GetPosition(Control));
			if (CaptureMouseOnDown)
				CaptureMouse(e.GetPosition(Control));
			if (IsMouseCaptured || IsDragAndDropPreparation)
				e.Handled = true;
		}
		protected virtual void OnMouseLeftButtonUp(DXMouseButtonEventArgs e) {
			IsMouseLeftButtonDown = false;
			if (CaptureMouseOnDown)
				ReleaseMouseCapture();
			if (IsDragAndDropPreparation)
				DragAndDropPrepared(false, null);
			if (IsDragAndDrop) {
				EndDragAndDrop(e != null || AcceptDragAndDropWhenMouseCaptureCancelled);
				if (e != null)
					e.Handled = true;
			}
		}
		protected virtual void OnTouchDown(TouchEventArgs e) {
		}
		protected virtual void OnTouchUp(TouchEventArgs e) {
		}
		protected virtual void OnTouchEnter(TouchEventArgs e) {
		}
		protected virtual void OnTouchLeave(TouchEventArgs e) {
		}
		protected virtual void OnTouchMove(TouchEventArgs e) {
		}
		protected void ProcessKeyDown(DXKeyEventArgs e) {
			if (IsDragAndDrop)
				OnDragAndDropKeyDown(e);
			else
				OnKeyDown(e);
		}
		protected void ProcessKeyUp(DXKeyEventArgs e) {
			if (IsDragAndDrop)
				OnDragAndDropKeyUp(e);
			else
				OnKeyUp(e);
		}
		protected bool CaptureMouseOnDown { get; set; }
		private const int DoubleClickTime = 500;	
		private const int DoubleClickArea = 4;	  
		private Point _ClickPosition;
		private DateTime _ClickTime;
		private void CheckMouseDoubleClick(DXMouseButtonEventArgs e) {
			var clickPosition = e.GetPosition(Control);
			var clickTime = DateTime.UtcNow;
			if((clickTime - _ClickTime).TotalMilliseconds <= DoubleClickTime) {
				var doubleClickArea = new Rect(_ClickPosition, new Size(DoubleClickArea, DoubleClickArea));
				RectHelper.Offset(ref doubleClickArea, -doubleClickArea.Width / 2, -doubleClickArea.Height / 2);
				if(doubleClickArea.Contains(clickPosition))
					OnMouseDoubleClick(e);
			}
			_ClickPosition = clickPosition;
			_ClickTime = clickTime;
		}
		private bool ProcessMouseWheelScrolling(int delta) {
			if (!IsScrollable())
				return false;
			ScrollParams scrollParams;
			if (VertScrollParams.Enabled)
				scrollParams = VertScrollParams;
			else
				if (HorzScrollParams.Enabled)
					scrollParams = HorzScrollParams;
				else
					return false;
			CanAnimateScrolling = true;
			try {
				scrollParams.Scroll(scrollParams.Position - Math.Round((double)delta / 120 * scrollParams.SmallStep), false);
			}
			finally {
				CanAnimateScrolling = false;
			}
			return true;
		}
		#endregion Keyboard, Mouse, and Touch Handling
		#region Scrolling
		private ScrollBars _ScrollBars;
		public bool HasScrollBars() {
			return IsScrollable() && ScrollBars != ScrollBars.None;
		}
		public void ResetScrollBarsVisibility() {
			IsHorzScrollBarVisible = false;
			IsVertScrollBarVisible = false;
		}
		public void UpdateScrollBarsVisibility() {
			if (!HasScrollBars())
				return;
			bool prevHorzScrollBarVisible = IsHorzScrollBarVisible;
			bool prevVertScrollBarVisible = IsVertScrollBarVisible;
			bool oldHorzScrollBarVisible, oldVertScrollBarVisible;
			ResetScrollBarsVisibility();
			do {
				UpdateScrollParams();
				oldHorzScrollBarVisible = IsHorzScrollBarVisible;
				oldVertScrollBarVisible = IsVertScrollBarVisible;
				IsHorzScrollBarVisible = HorzScrollParams.Enabled;
				IsVertScrollBarVisible = VertScrollParams.Enabled;
			}
			while (IsHorzScrollBarVisible != oldHorzScrollBarVisible || IsVertScrollBarVisible != oldVertScrollBarVisible);
			if ((prevHorzScrollBarVisible || prevVertScrollBarVisible) && !IsHorzScrollBarVisible && !IsVertScrollBarVisible) {
				IsHorzScrollBarVisible = prevHorzScrollBarVisible;
				IsVertScrollBarVisible = prevVertScrollBarVisible;
			}
		}
		public bool UpdateScrolling() {
			if(!IsScrollable())
				return false;
			if(HasScrollBars()) {
				InitScrollBars();
				UpdateScrollBars();
			}
			else
				UpdateScrollParams();
			var result = Scroll(Orientation.Horizontal, HorzScrollParams.Position);
			result = Scroll(Orientation.Vertical, VertScrollParams.Position) || result;
			return result;
		}
		public ScrollParams HorzScrollParams { get; private set; }
		public ScrollParams VertScrollParams { get; private set; }
		public bool IsHorzScrollBarVisible { get; protected set; }
		public bool IsVertScrollBarVisible { get; protected set; }
		public ScrollBar HorzScrollBar { get; protected set; }
		public ScrollBar VertScrollBar { get; protected set; }
		public CornerBox CornerBox { get; protected set; }
		public ScrollBars ScrollBars {
			get { return _ScrollBars; }
			protected internal set {
				if(_ScrollBars != value) {
					_ScrollBars = value;
					CheckScrollBars();
					Control.InvalidateMeasure();
				}
			}
		}
		public event EventHandler ScrollParamsChanged;
		public bool CanScroll() {
			return IsScrollable() && (HorzScrollParams.Enabled || VertScrollParams.Enabled);
		}
		public virtual bool IsScrollable() {
			return false;
		}
		protected virtual void InitScrollParams(ScrollParams horzScrollParams, ScrollParams vertScrollParams) {
		}
		protected virtual bool Scroll(Orientation orientation, double position) {
			return false;
		}
		protected void CheckScrollParams() {
			if(IsScrollable())
				CreateScrollParams();
			else
				DestroyScrollParams();
		}
		private void CreateScrollParams() {
			if(HorzScrollParams != null)
				return;
			HorzScrollParams = new ScrollParams();
			HorzScrollParams.Change += ScrollParamsChange;
			HorzScrollParams.Scrolling += ScrollParamsScrolling;
			VertScrollParams = new ScrollParams();
			VertScrollParams.Change += ScrollParamsChange;
			VertScrollParams.Scrolling += ScrollParamsScrolling;
		}
		private void DestroyScrollParams() {
			if(HorzScrollParams == null)
				return;
			HorzScrollParams.Change -= ScrollParamsChange;
			HorzScrollParams.Scrolling -= ScrollParamsScrolling;
			VertScrollParams.Change -= ScrollParamsChange;
			VertScrollParams.Scrolling -= ScrollParamsScrolling;
			HorzScrollParams = null;
			VertScrollParams = null;
		}
		protected ScrollParams GetScrollParams(ScrollDirection direction) {
			switch(direction) {
				case ScrollDirection.Left:
				case ScrollDirection.Right:
					return HorzScrollParams;
				case ScrollDirection.Top:
				case ScrollDirection.Bottom:
					return VertScrollParams;
				default:
					return null;
			}
		}
		protected ScrollParams GetScrollParams(Orientation orientation) {
			return orientation == Orientation.Horizontal ? HorzScrollParams : VertScrollParams;
		}
		protected virtual void ScrollParamsChange(ScrollParams sender) {
			if (ScrollParamsChanged != null)
				ScrollParamsChanged(sender, EventArgs.Empty);
		}
		protected virtual void ScrollParamsScrolling(object sender, ScrollKind kind) {
			if(sender == HorzScrollParams)
				Scroll(Orientation.Horizontal, ((ScrollParams)sender).Position);
			else
				if(sender == VertScrollParams)
					Scroll(Orientation.Vertical, ((ScrollParams)sender).Position);
		}
		protected void UpdateScrollParams() {
			InitScrollParams(HorzScrollParams, VertScrollParams);
		}
		protected virtual void CheckScrollBars() {
		}
		protected virtual void InitScrollBars() {
			UpdateScrollParams();
			HorzScrollParams.AssignTo(HorzScrollBar);
			VertScrollParams.AssignTo(VertScrollBar);
		}
		protected virtual void ScrollBarScroll(object sender, ScrollEventArgs e) {
			var scrollBar = (ScrollBar)sender;
			CanAnimateScrolling = e.ScrollEventType != ScrollEventType.ThumbTrack;
			try {
				Scroll(scrollBar.Orientation, scrollBar.Value);
			}
			finally {
				CanAnimateScrolling = false;
			}
		}
		protected virtual void UpdateScrollBars() {
		}
		protected bool CanAnimateScrolling { get; set; }
		protected virtual Rect ScrollableAreaBounds { get { return RectHelper.New(Control.GetSize()); } }
		#endregion Scrolling
		#region Auto Scrolling
		private bool _AllowAutoScrolling;
		private ScrollDirection _AutoScrollingDirection;
		private bool _IsAutoScrolling;
		protected int AutoScrollingTimeInterval = 100;
		protected double AutoScrollingAreaWidth = 30;
		protected virtual void CheckAutoScrolling(Point p) {
			if(IsScrollable() && AllowAutoScrolling)
				for(var direction = ScrollDirection.Left; direction <= ScrollDirection.Bottom; direction++)
					if(GetScrollParams(direction).Enabled && GetAutoScrollingAreaBounds(direction).Contains(p)) {
						AutoScrollingDirection = direction;
						return;
					}
			AutoScrollingDirection = ScrollDirection.None;
		}
		protected virtual Rect GetAutoScrollingAreaBounds(ScrollDirection direction) {
			var result = ScrollableAreaBounds;
			switch(direction) {
				case ScrollDirection.Left:
					result.Width = AutoScrollingAreaWidth;
					break;
				case ScrollDirection.Top:
					result.Height = AutoScrollingAreaWidth;
					break;
				case ScrollDirection.Right:
					RectHelper.SetLeft(ref result, result.Right - AutoScrollingAreaWidth);
					break;
				case ScrollDirection.Bottom:
					RectHelper.SetTop(ref result, result.Bottom - AutoScrollingAreaWidth);
					break;
				default:
					result = Rect.Empty;
					break;
			}
			return result;
		}
		protected internal bool AllowAutoScrolling {
			get { return _AllowAutoScrolling; }
			set {
				if(_AllowAutoScrolling != value) {
					_AllowAutoScrolling = value;
					if(!AllowAutoScrolling)
						AutoScrollingDirection = ScrollDirection.None;
				}
			}
		}
		protected ScrollDirection AutoScrollingDirection {
			get { return _AutoScrollingDirection; }
			set {
				if(_AutoScrollingDirection != value) {
					_AutoScrollingDirection = value;
					if(AutoScrollingDirection == ScrollDirection.None)
						StopAutoScrolling();
					else
						StartAutoScrolling();
				}
			}
		}
		protected bool IsAutoScrolling { get { return _IsAutoScrolling; } }
		private void StartAutoScrolling() {
			if(IsAutoScrolling)
				return;
			_IsAutoScrolling = true;
			AutoScrollingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(AutoScrollingTimeInterval) };
			AutoScrollingTimer.Tick += AutoScrollingTimerTick;
			AutoScrollingTimer.Start();
		}
		private void StopAutoScrolling() {
			if(!IsAutoScrolling)
				return;
			_IsAutoScrolling = false;
			AutoScrollingTimer.Stop();
			AutoScrollingTimer = null;
		}
		private void AutoScrollingTimerTick(object sender, EventArgs e) {
			GetScrollParams(AutoScrollingDirection).DoSmallStep(
				AutoScrollingDirection == ScrollDirection.Right || AutoScrollingDirection == ScrollDirection.Bottom);
		}
		private DispatcherTimer AutoScrollingTimer { get; set; }
		#endregion Auto Scrolling
		#region Drag&Drop
		public const double StartDragAreaSize = 8;
		public static Rect GetStartDragAreaBounds(Point startDragPoint) {
			return new Rect(
				startDragPoint.X - StartDragAreaSize / 2, startDragPoint.Y - StartDragAreaSize / 2,
				StartDragAreaSize, StartDragAreaSize);
		}
		private DragAndDropController _DragAndDropController;
		private Cursor _OriginalCursor;
		private UIElement _OriginalFocusedElement;
		public DragAndDropController DragAndDropController {
			get { return _DragAndDropController; }
			protected set { _DragAndDropController = value; }
		}
		public bool IsDragAndDrop { get; private set; }
		public event EventHandler StartDrag;
		public event EventHandler EndDrag;
		protected void PrepareDragAndDrop(Point p) {
			StartDragPoint = p;
			if (DragAndDropController == null && IsImmediateDragAndDrop ||
				DragAndDropController != null && DragAndDropController.IsImmediateDragAndDrop)
				StartDragAndDrop(p);
			else {
				IsDragAndDropPreparation = true;
			}
		}
		protected void DragAndDropPrepared(bool isSuccess, Point? p) {
			IsDragAndDropPreparation = false;
			if (isSuccess)
				StartDragAndDrop(p.Value);
			else
				_DragAndDropController = null;
		}
		protected virtual bool WantsDragAndDrop(Point p, out DragAndDropController controller) {
			controller = null;
			return false;
		}
		protected virtual void StartDragAndDrop(Point p) {
			if (!CaptureMouse(p)) {
				_DragAndDropController = null;
				return;
			}
			if (StartDrag != null)
				StartDrag(Control, EventArgs.Empty);
			AttachToKeyboardEventsForDragAndDrop();
			IsDragAndDrop = true;
			_OriginalCursor = Control.Cursor;
			Control.Cursor = DragAndDropCursor;
			if (DragAndDropController != null)
				DragAndDropController.StartDragAndDrop(p);
		}
		protected virtual void DragAndDrop(Point p) {
			if(DragAndDropController != null)
				DragAndDropController.DragAndDrop(p);
		}
		protected virtual void EndDragAndDrop(bool accept) {
			ReleaseMouseCapture();
			Control.Cursor = _OriginalCursor;
			IsDragAndDrop = false;
			DetachFromKeyboardEventsForDragAndDrop();
			if (DragAndDropController != null) {
				DragAndDropController dragAndDropController = DragAndDropController;
				_DragAndDropController = null;
				dragAndDropController.EndDragAndDrop(accept);
			}
			if (EndDrag != null)
				EndDrag(Control, EventArgs.Empty);
		}
		protected void CancelDragAndDrop() {
			if(IsDragAndDrop)
				EndDragAndDrop(false);
		}
		protected virtual void OnDragAndDropKeyDown(DXKeyEventArgs e) {
			if (e.Key == Key.Escape) {
				CancelDragAndDrop();
				e.Handled = true;
			}
		}
		protected virtual void OnDragAndDropKeyUp(DXKeyEventArgs e) {
			if (Control.IsInDesignTool() && e.Key == Key.Escape) {
				CancelDragAndDrop();
				e.Handled = true;
			}
		}
		protected virtual bool AcceptDragAndDropWhenMouseCaptureCancelled { get { return false; } }
		protected virtual Cursor DragAndDropCursor {
			get {
				if(DragAndDropController == null || DragAndDropController.DragCursor == null)
					return Control.Cursor;
				else
					return DragAndDropController.DragCursor;
			}
		}
		protected bool IsDragAndDropPreparation { get; private set; }
		protected virtual bool IsImmediateDragAndDrop { get { return false; } }
		protected Rect StartDragAreaBounds { get { return GetStartDragAreaBounds(StartDragPoint); } }
		protected Point StartDragPoint { get; private set; }
		private void AttachToKeyboardEventsForDragAndDrop() {
			Control.GetRootVisual().KeyDown += DragAndDropKeyDown;
			Control.GetRootVisual().KeyUp += DragAndDropKeyUp;
#if SILVERLIGHT
			_OriginalFocusedElement = FocusManager.GetFocusedElement() as UIElement;
#else
			_OriginalFocusedElement = FocusManager.GetFocusedElement(FocusManager.GetFocusScope(Control)) as UIElement;
#endif
			if(_OriginalFocusedElement != null) {
				_OriginalFocusedElement.KeyDown += DragAndDropKeyDown;
				_OriginalFocusedElement.KeyUp += DragAndDropKeyUp;
			}
		}
		private void DetachFromKeyboardEventsForDragAndDrop() {
			if(_OriginalFocusedElement != null) {
				_OriginalFocusedElement.KeyDown -= DragAndDropKeyDown;
				_OriginalFocusedElement.KeyUp -= DragAndDropKeyUp;
			}
			Control.GetRootVisual().KeyDown -= DragAndDropKeyDown;
			Control.GetRootVisual().KeyUp -= DragAndDropKeyUp;
		}
		private void DragAndDropKeyDown(object sender, KeyEventArgs e) {
			ProcessKeyDown(new DXKeyEventArgs(e, args => e.Handled = args.Handled));
		}
		private void DragAndDropKeyUp(object sender, KeyEventArgs e) {
			ProcessKeyUp(new DXKeyEventArgs(e, args => e.Handled = args.Handled));
		}
		#endregion Drag&Drop
	}
	public class DragAndDropController {
		private bool _OriginalAllowAutoScrolling;
		public DragAndDropController(Controller controller, Point startDragPoint) {
			Controller = controller;
			StartDragPoint = startDragPoint;
		}
		public virtual void StartDragAndDrop(Point p) {
			DragImage = CreateDragImage();
			if(DragImage != null) {
				InitializeDragImage();
				ShowDragImage(p);
			}
			_OriginalAllowAutoScrolling = Controller.AllowAutoScrolling;
			Controller.AllowAutoScrolling = AllowAutoScrolling;
		}
		public virtual void DragAndDrop(Point p) {
			if(DragImage != null)
				MoveDragImage(p);
		}
		public virtual void EndDragAndDrop(bool accept) {
			Controller.AllowAutoScrolling = _OriginalAllowAutoScrolling;
			if(DragImage != null) {
				HideDragImage();
				FinalizeDragImage();
				DragImage = null;
			}
		}
		public virtual IEnumerable<UIElement> GetInternalElements() {
#if SILVERLIGHT
			if (_DragImagePopup != null)
				yield return _DragImagePopup.Popup;
#else
			return new UIElement[0];
#endif
		}
		public virtual void OnMeasure(Size availableSize) {
		}
		public virtual void OnArrange(Size finalSize) {
		}
		public virtual Cursor DragCursor { get { return null; } }
		public virtual bool IsImmediateDragAndDrop { get { return false; } }
		protected virtual bool AllowAutoScrolling { get { return false; } }
		protected Controller Controller { get; private set; }
		protected Point StartDragPoint { get; private set; }
		#region Drag Image
		private int _DragImageChildIndex;
		private Panel _DragImageParent;
		private TransparentPopup _DragImagePopup;
		protected virtual FrameworkElement CreateDragImage() {
			return null;
		}
		protected virtual Panel GetDragImageParent() {
			return DragImage.GetParent() as Panel;
		}
		protected virtual void InitializeDragImage() {
			_DragImageParent = GetDragImageParent();
			if (_DragImageParent == null)
				return;
			if (_DragImageParent is IPanel)
				_DragImageChildIndex = ((IPanel)_DragImageParent).GetLogicalChildren(false).IndexOf(DragImage);
			else
				_DragImageChildIndex = _DragImageParent.Children.IndexOf(DragImage);
			_DragImageParent.Children.Remove(DragImage);
		}
		protected virtual void FinalizeDragImage() {
			if (_DragImageParent == null)
				return;
			int dragImageIndex;
			if (_DragImageParent is IPanel) {
				FrameworkElements children = ((IPanel)_DragImageParent).GetLogicalChildren(false);
				if (_DragImageChildIndex < children.Count)
					dragImageIndex = _DragImageParent.Children.IndexOf(children[_DragImageChildIndex]);
				else
					if (children.Count > 0)
						dragImageIndex = _DragImageParent.Children.IndexOf(children[children.Count - 1]) + 1;
					else
						dragImageIndex = 0;
			}
			else
				dragImageIndex = _DragImageChildIndex;
			_DragImageParent.Children.Insert(dragImageIndex, DragImage);
		}
		protected virtual Point GetDragImageOffset() {
			return new Point(0, 0);
		}
		protected virtual Point GetDragImagePosition(Point p) {
			var offset = GetDragImageOffset();
			PointHelper.Offset(ref p, offset.X, offset.Y);
			return p;
		}
		protected FrameworkElement DragImage { get; private set; }
		protected virtual Container CreateDragImageContainer() {
			return new Container();
		}
		private void ShowDragImage(Point p) {
			_DragImagePopup = new TransparentPopup();
#if SILVERLIGHT
			_DragImagePopup.Child = new Canvas();
#else
			_DragImagePopup.PlacementTarget = Controller.Control;
			_DragImagePopup.Child = CreateDragImageContainer();
#endif
			_DragImagePopup.Child.IsHitTestVisible = false;
			((Panel)_DragImagePopup.Child).Children.Add(DragImage);
#if SILVERLIGHT
			if (Controller.Control is Panel)
				((Panel)Controller.Control).Children.Add(_DragImagePopup.Popup);
#endif
			_DragImagePopup.IsOpen = true;
			MoveDragImage(p);
		}
		private void HideDragImage() {
			_DragImagePopup.IsOpen = false;
#if SILVERLIGHT
			if (Controller.Control is Panel)
				((Panel)Controller.Control).Children.Remove(_DragImagePopup.Popup);
#endif
			((Panel)_DragImagePopup.Child).Children.Remove(DragImage);
			_DragImagePopup.Child = null;
			_DragImagePopup = null;
		}
		private void MoveDragImage(Point p) {
			p = GetDragImagePosition(p);
#if SILVERLIGHT
			p = Controller.Control.MapPoint(p, (FrameworkElement)DragImage.Parent);
			DragImage.SetLeft(p.X);
			DragImage.SetTop(p.Y);
#else
			_DragImagePopup.SetOffset(p);
#endif
		}
		#endregion Drag Image
		#region Delay Timer
		protected void CheckDelayTimer(ref Storyboard timer, int durationInMilliseconds, Func<bool> isDelayNeeded, Action onDelayExpired) {
			StopDelayTimer(ref timer);
			if (isDelayNeeded())
				timer = StartDelayTimer(durationInMilliseconds, onDelayExpired);
		}
		private Storyboard StartDelayTimer(int durationInMilliseconds, Action onDelayExpired) {
			var result = new Storyboard { Duration = TimeSpan.FromMilliseconds(durationInMilliseconds) };
			result.Completed += (o, e) => onDelayExpired();
			result.Begin();
			return result;
		}
		private void StopDelayTimer(ref Storyboard timer) {
			if (timer == null)
				return;
			timer.Stop();
			timer = null;
		}
		#endregion Delay Timer
	}
	public class DXKeyEventArgs : EventArgs {
		private bool _Handled;
		public DXKeyEventArgs() {
		}
		public DXKeyEventArgs(KeyEventArgs args, Action<DXKeyEventArgs> handledChanged) {
			Handled = args.Handled;
			Key = args.Key;
			HandledChanged = handledChanged;
		}
		public bool Handled {
			get { return _Handled; }
			set {
				if (Handled == value)
					return;
				_Handled = value;
				if (HandledChanged != null)
					HandledChanged(this);
			}
		}
		public Key Key { get; protected set; }
		protected Action<DXKeyEventArgs> HandledChanged { get; set; }
	}
	public class DXMouseEventArgs : EventArgs {
		public DXMouseEventArgs() {
		}
		public DXMouseEventArgs(MouseEventArgs args) {
			OriginalSource = args.OriginalSource;
			StylusDevice = args.StylusDevice;
			RelativePositionElement = null;
#if SILVERLIGHT
			RelativePosition = args.GetPosition(RelativePositionElement);
#else
			if (!BrowserInteropHelper.IsBrowserHosted) {
				PresentationSource presentationSource = PresentationSource.FromDependencyObject((DependencyObject)OriginalSource);
				if (presentationSource != null) {
					RelativePosition = args.GetPosition((System.Windows.IInputElement)presentationSource.RootVisual);
					RelativePosition = presentationSource.RootVisual.PointToScreen(RelativePosition);
				}
			}
			else
				RelativePosition = args.GetPosition(null);
#endif
		}
		public Point GetPosition(UIElement relativeTo) {
			if (relativeTo == RelativePositionElement)
				return RelativePosition;
#if SILVERLIGHT
			GeneralTransform transform;
			try {
				if (RelativePositionElement != null)
					transform = RelativePositionElement.TransformToVisual(relativeTo);
				else
					transform = relativeTo.TransformToVisual(null).Inverse;
			}
			catch {
				return RelativePosition;
			}
			return transform.Transform(RelativePosition);
#else
			if (RelativePositionElement != null)
				if (relativeTo != null)
					return RelativePositionElement.TranslatePoint(RelativePosition, relativeTo);
				else
					return RelativePositionElement.PointToScreen(RelativePosition);
			else
				return ((FrameworkElement)relativeTo).MapPointFromScreen(RelativePosition);
#endif
		}
		public object OriginalSource { get; protected set; }
		public StylusDevice StylusDevice { get; protected set; }
		protected Point RelativePosition { get; set; }
		protected UIElement RelativePositionElement { get; set; }
	}
	public class DXMouseButtonEventArgs : DXMouseEventArgs {
		private bool _Handled;
		public DXMouseButtonEventArgs() {
		}
		public DXMouseButtonEventArgs(MouseButtonEventArgs args, Action<DXMouseButtonEventArgs> handledChanged) : base(args) {
			Handled = args.Handled;
			HandledChanged = handledChanged;
		}
		public bool Handled {
			get { return _Handled; }
			set {
				if (Handled == value)
					return;
				_Handled = value;
				if (HandledChanged != null)
					HandledChanged(this);
			}
		}
		protected Action<DXMouseButtonEventArgs> HandledChanged { get; set; }
	}
	public class DXMouseWheelEventArgs : DXMouseEventArgs {
		private bool _Handled;
		public DXMouseWheelEventArgs() {
		}
		public DXMouseWheelEventArgs(MouseWheelEventArgs args, Action<DXMouseWheelEventArgs> handledChanged) : base(args) {
			Delta = args.Delta;
			Handled = args.Handled;
			HandledChanged = handledChanged;
		}
		public int Delta { get; protected set; }
		public bool Handled {
			get { return _Handled; }
			set {
				if (Handled == value)
					return;
				_Handled = value;
				if (HandledChanged != null)
					HandledChanged(this);
			}
		}
		protected Action<DXMouseWheelEventArgs> HandledChanged { get; set; }
	}
}

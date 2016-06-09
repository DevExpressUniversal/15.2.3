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

using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Core.Native {
	public abstract class DragControllerBase {
		protected virtual bool StartDragDropOnHandledMouseEvents { get { return false; } }
		protected FrameworkElement DragChild { get; private set; }
		protected Point StartPoint { get; private set; }
		protected Point CurrentPoint { get; private set; }
		protected bool IsDragStarted { get; private set; }
		bool shouldStartDrag;
		protected void Subscribe(FrameworkElement dragChild) {
			dragChild.PreviewMouseMove += OnMouseMove;
			dragChild.MouseLeftButtonUp += OnMouseLeftButtonUp;
			if(!StartDragDropOnHandledMouseEvents)
				dragChild.MouseLeftButtonDown += OnMouseLeftButtonDown;
			else dragChild.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
			dragChild.MouseLeave += OnMouseLeave;
		}
		protected void Unsubscribe(FrameworkElement dragChild) {
			dragChild.PreviewMouseMove -= OnMouseMove;
			dragChild.MouseLeftButtonUp -= OnMouseLeftButtonUp;
			if(!StartDragDropOnHandledMouseEvents)
				dragChild.MouseLeftButtonDown -= OnMouseLeftButtonDown;
			else dragChild.RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown));
			dragChild.MouseLeave -= OnMouseLeave;
		}
		bool suppressMouseMove = false;
		void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if(!IsMouseLeftButtonDownOnDragChild(sender, e))
				return;
			StartPoint = GetMousePosition();
			shouldStartDrag = true;
		}
		void OnMouseLeave(object sender, MouseEventArgs e) {
			if(!IsDragStarted) shouldStartDrag = false;
		}
		void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			OnMouseLeftButtonUpCore(() => {
				e.Handled = true;
				OnDragFinished();
			});
		}
		void OnMouseLeftButtonUpCore(Action finishDrag) {
			suppressMouseMove = true;
			try {
				shouldStartDrag = false;
				if(!IsDragStarted) return;
				StopDrag(true);
				finishDrag.Do(x => x());
			} finally { suppressMouseMove = false; }
		}
		void OnMouseMove(object sender, MouseEventArgs e) {
			if(suppressMouseMove) {
				e.Handled = true;
				return;
			}
			suppressMouseMove = true;
			try {
				if(e.LeftButton != MouseButtonState.Pressed) {
					OnMouseLeftButtonUpCore(() => {
						e.Handled = true;
						OnDragFinished();
					});
					return;
				}
				if(IsDragStarted) {
					CurrentPoint = GetMousePosition();
					OnDrag();
					e.Handled = true;
					return;
				}
				if(shouldStartDrag && ShouldStartDrag((FrameworkElement)sender, e)) {
					StartDrag((FrameworkElement)sender);
					e.Handled = true;
					return;
				}
			} finally { suppressMouseMove = false; }
		}
		bool ShouldStartDrag(FrameworkElement dragChild, MouseEventArgs e) {
			if(e.LeftButton != MouseButtonState.Pressed) return false;
			var currentPoint = GetMousePosition();
			var diff = StartPoint - currentPoint;
			return (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance);
		}
		protected virtual bool IsMouseLeftButtonDownOnDragChild(object sender, MouseButtonEventArgs e) {
			return true;
		}
		protected abstract Point GetMousePosition();
		protected abstract void OnDragStarted();
		protected abstract void OnDragStopped();
		protected abstract void OnDrag();
		protected abstract void OnDrop();
		protected virtual bool CanStartDrag(FrameworkElement dragChild) {
			return true;
		}
		protected internal void StartDrag(FrameworkElement dragChild, Point? dragStartPoint = null) {
			if(IsDragStarted) return;
			if(!CanStartDrag(dragChild)) return;
			DragChild = dragChild;
			suppressMouseMove = true;
			Mouse.Capture(DragChild);
			suppressMouseMove = false;
			StartPoint = dragStartPoint != null ? dragStartPoint.Value : GetMousePosition();
			CurrentPoint = GetMousePosition();
			IsDragStarted = true;
			OnDragStarted();
		}
		protected internal void StopDrag(bool doDrop) {
			if(!IsDragStarted) return;
			IsDragStarted = false;
			Mouse.Capture(null);
			OnDragStopped();
			if(doDrop) OnDrop();
			DragChild = null;
		}
		protected virtual void OnDragFinished() { }
	}
	public class DragWidgetWindow : Window, IDisposable {
#if DEBUGTEST
		public static List<WeakReference> Windows = new List<WeakReference>();
		public static void RemoveDeadWindows() {
			MemoryLeaksHelper.GarbageCollect();
			var toRemove = Windows.Where(x => !x.IsAlive).ToList();
			toRemove.ForEach(x => Windows.Remove(x));
		}
#endif
		public FrameworkElement DragObject { get; private set; }
		public Point OffsetDifference { get; set; }
		public DragWidgetWindow() {
#if DEBUGTEST
			Windows.Add(new WeakReference(this));
#endif
			ThemeManager.SetTheme(this, null);
			WindowStyle = WindowStyle.None;
			AllowsTransparency = true;
			AllowDrop = false;
			Background = null;
			IsHitTestVisible = false;
			SizeToContent = SizeToContent.WidthAndHeight;
			Topmost = true;
			ShowInTaskbar = false;
			ShowActivated = false;
		}
		public void Initialize(FrameworkElement dragObject) {
			DragObject = dragObject;
		}
		public void Dispose() {
			Content = null;
			DragObject = null;
		}
		public void UpdateWindowLocation() {
			var p = DragControllerHelper.GetMousePositionOnScreen();
			Left = p.X + OffsetDifference.X;
			Top = p.Y + OffsetDifference.Y;
		}
		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);
			DragControllerHelper.SetupDragWidget(this);
		}
	}
	public interface IDragPanelVisual {
		IDragPanel GetDragPanel(IDragPanel sourceDragPanel);
	}
	public interface IDragPanel {
		IDragPanelVisual VisualPanel { get; }
		string Region { get; }
		DragControllerBase Controller { get; }
		IEnumerable<FrameworkElement> Children { get; }
		event EventHandler ChildrenChanged;
		Orientation Orientation { get; }
		bool CanStartDrag(FrameworkElement child);
		void SetVisibility(FrameworkElement child, Visibility visibility);
		FrameworkElement Insert(FrameworkElement child, int index);
		void Remove(FrameworkElement child);
		FrameworkElement Move(FrameworkElement child, int index);
		void DropOnEmptySpace(FrameworkElement child);
		void OnDragFinished();
		DragWidgetWindow CreateDragWidget(FrameworkElement child);
	}
	public abstract class DragPanelControllerBase<T, TVisual> : DragControllerBase 
		where T : class, IDragPanel
		where TVisual : FrameworkElement, IDragPanelVisual {
		protected T DragPanel { get; private set; }
		protected TVisual VisualPanel { get { return (TVisual)DragPanel.VisualPanel; } }
		protected IEnumerable<FrameworkElement> Children {
			get { return children; }
			private set {
				if(children == value) return;
				if(children != null)
					children.ToList().ForEach(x => Unsubscribe(x));
				children = value;
				if(children != null)
					children.ToList().ForEach(x => Subscribe(x));
			}
		}
		IEnumerable<FrameworkElement> children;
		SizeHelperBase OrientationHelper { get { return SizeHelperBase.GetDefineSizeHelper(DragPanel.Orientation); } }
		public virtual void Initialize(T dragPanel) {
			DragPanel = dragPanel;
			Children = DragPanel.Children.ToList();
			DragPanel.ChildrenChanged += OnChildrenChanged;
		}
		public virtual void Uninitialize() {
			DragPanel.ChildrenChanged -= OnChildrenChanged;
			Children = null;
			DragPanel = null;
		}
		void OnChildrenChanged(object sender, EventArgs e) {
			Children = DragPanel.Children.ToList();
		}
		protected double CurrentX { get { return OrientationHelper.GetDefinePoint(CurrentPoint); } }
		protected double StartX { get { return OrientationHelper.GetDefinePoint(StartPoint); } }
		protected override Point GetMousePosition() {
			return Mouse.GetPosition(VisualPanel);
		}
		protected override bool CanStartDrag(FrameworkElement dragChild) {
			return DragPanel.CanStartDrag(dragChild);
		}
		protected double GetWidth(FrameworkElement child) {
			return OrientationHelper.GetDefineSize(new Size(child.ActualWidth, child.ActualHeight));
		}
		protected override void OnDragFinished() {
			base.OnDragFinished();
			DragPanel.Do(x => x.OnDragFinished());
		}
	}
	public static class DragControllerHelper {
		public static double DragOutHorizontalDistance = SystemParameters.MinimumHorizontalDragDistance;
		public static double DragOutVerticalDistance = SystemParameters.MinimumVerticalDragDistance + 12;
		public static T FindDragPanel<T>(Window w, Point pointOnScreen) where T : FrameworkElement {
			if(w == null || !w.IsVisible) return null;
			T dragPanel = null;
			HitTestFilterCallback filterCallback = (potentialHitTestTarget) => {
				if(potentialHitTestTarget is T) {
					dragPanel = (T)potentialHitTestTarget;
					return HitTestFilterBehavior.Stop;
				}
				return HitTestFilterBehavior.ContinueSkipSelf;
			};
			HitTestResultCallback resultCallback = (result) => {
				if(result.VisualHit is IDragPanel) {
					dragPanel = (T)result.VisualHit;
					return HitTestResultBehavior.Stop;
				}
				return HitTestResultBehavior.Continue;
			};
			VisualTreeHelper.HitTest(w, filterCallback, resultCallback, new PointHitTestParameters(w.PointFromScreen(pointOnScreen)));
			return dragPanel;
		}
		public static int FindDragOverIndex(IDragPanel dragPanel, FrameworkElement visualPanel, Point pointOnPanel) {
			if(IsPointOutOfPanel(visualPanel, pointOnPanel, false))
				return -1;
			foreach(FrameworkElement child in dragPanel.Children) {
				Point p = visualPanel.TranslatePoint(pointOnPanel, child);
				SizeHelperBase helper = SizeHelperBase.GetDefineSizeHelper(dragPanel.Orientation);
				double pX = helper.GetDefinePoint(p);
				double pY = helper.GetSecondaryPoint(p);
				double childActualWidth = dragPanel.Orientation == Orientation.Horizontal ? child.ActualWidth : child.ActualHeight;
				double childActualHeight = dragPanel.Orientation == Orientation.Horizontal ? child.ActualHeight : child.ActualWidth;
				if(pY <= 0 || pY >= childActualHeight)
					continue;
				if(pX > 0 && pX <= childActualWidth / 2)
					return dragPanel.Children.ToList().IndexOf(child);
				if(pX > childActualWidth / 2 && pX < childActualWidth)
					return dragPanel.Children.ToList().IndexOf(child) + 1;
			}
			return dragPanel.Children.Count();
		}
		public static bool IsPointOutOfPanel(FrameworkElement panel, Point pointOnPanel, bool useCoefs) {
			double horCoef = useCoefs ? DragOutHorizontalDistance : 0d;
			double verCoef = useCoefs ? DragOutVerticalDistance : 0d;
			return pointOnPanel.X < -horCoef || pointOnPanel.X > panel.ActualWidth + horCoef || 
				pointOnPanel.Y < -verCoef || pointOnPanel.Y > panel.ActualHeight + verCoef;
		}
		public static bool IsPointInsidePanel(FrameworkElement panel, Point pointOnPanel, bool useCoefs) {
			double horCoef = useCoefs ? SystemParameters.MinimumHorizontalDragDistance : 0d;
			double verCoef = useCoefs ? SystemParameters.MinimumVerticalDragDistance : 0d;
			return pointOnPanel.X > horCoef && pointOnPanel.X < panel.ActualWidth - horCoef &&
				pointOnPanel.Y > verCoef && pointOnPanel.Y < panel.ActualHeight - verCoef;
		}
		public static Rect GetCurrentScreenWorkingArea() {
			var mousePosition = GetMousePositionOnScreen();
			System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)mousePosition.X, (int)mousePosition.Y));
			return new Rect() {
				X = screen.WorkingArea.X,
				Y = screen.WorkingArea.Y,
				Width = screen.WorkingArea.Width,
				Height = screen.WorkingArea.Height,
			};
		}
		[SecuritySafeCritical]
		public static Point GetMousePositionOnScreen() {
			Win32Point p = new Win32Point();
			if(!GetCursorPos(ref p))
				return new Point();
			return new Point(p.X, p.Y);
		}
		[SecuritySafeCritical]
		public static Window GetWindowUnderMouse(Point pointOnScreen) {
			IntPtr hW = GetTopWindow(IntPtr.Zero);
			while(hW != IntPtr.Zero) {
				NativeMethods.RECT wRect;
				NativeMethods.GetWindowRect(hW, out wRect);
				if(pointOnScreen.X > wRect.left && pointOnScreen.X < wRect.right && pointOnScreen.Y > wRect.top && pointOnScreen.Y < wRect.bottom) {
					Window w = HwndSource.FromHwnd(hW).With(x => x.RootVisual) as Window;
					if(w != null && !(w is DragWidgetWindow))
						return w;
				}
				hW = GetWindow(hW, GW_HWNDNEXT);
			}
			return null;
		}
		[SecuritySafeCritical]
		public static void SetupDragWidget(Window dragWidget) {
			PresentationSource windowSource = PresentationSource.FromVisual(dragWidget);
			IntPtr handle = ((HwndSource)windowSource).Handle;
			Int32 styles = GetWindowLong(handle, GWL_EXSTYLE);
			SetWindowLong(handle, GWL_EXSTYLE, styles | WS_EX_LAYERED | WS_EX_TRANSPARENT);
		}
		[StructLayout(LayoutKind.Sequential)]
		struct Win32Point {
			public Int32 X;
			public Int32 Y;
		};
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetCursorPos(ref Win32Point pt);
		[DllImport("user32.dll")]
		static extern IntPtr GetTopWindow(IntPtr hWnd);
		const uint GW_HWNDNEXT = 2;
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
		const int WS_EX_TRANSPARENT = 0x00000020;
		const int GWL_EXSTYLE = (-20);
		const int WS_EX_LAYERED = 0x80000;
		[DllImport("user32.dll")]
		static extern int GetWindowLong(IntPtr hwnd, int index);
		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
	}
}

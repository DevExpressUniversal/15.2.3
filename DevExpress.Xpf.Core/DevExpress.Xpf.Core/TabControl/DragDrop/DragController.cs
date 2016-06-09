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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace DevExpress.Xpf.Core.Native {
	public class LocalDragPanelController<T, TVisual> : DragPanelControllerBase<T, TVisual> 
		where T : class, IDragPanel
		where TVisual : FrameworkElement, IDragPanelVisual {
		FrameworkElement OverChild;
		IEnumerable<double> ChildXs;
		double DragChildMinOffset, DragChildMaxOffset;
		protected int DragChildIndex { get { return Children.ToList().IndexOf(DragChild); } }
		int OverChildIndex { get { return Children.ToList().IndexOf(OverChild); } }
		protected internal bool SuppressAnimation = false;
		protected override void OnDragStarted() {
			GetCoordinates();
			SetUpOffset();
			OnDrag();
		}
		protected override void OnDragStopped() {
			CleanUpOffset();
		}
		protected override void OnDrop() {
			if(OverChild == null) return;
			DragPanel.Move(DragChild, OverChildIndex);
		}
		protected override void OnDrag() {
			double offset = CurrentX - StartX;
			offset = Math.Max(offset, DragChildMinOffset);
			offset = Math.Min(offset, DragChildMaxOffset);
			SetOffset(DragChild, offset);
			List<FrameworkElement> processedChilds = new List<FrameworkElement>() { DragChild };
			OverChild = GetOverChild();
			if(OverChild != null) {
				for(int i = OverChildIndex; i < DragChildIndex; i++) {
					FrameworkElement child = Children.ElementAt(i);
					SetOffset(child, GetDragOffset());
					processedChilds.Add(child);
				}
				for(int i = DragChildIndex + 1; i <= OverChildIndex; i++) {
					FrameworkElement child = Children.ElementAt(i);
					SetOffset(child, -GetDragOffset());
					processedChilds.Add(child);
				}
			}
			Children.Where(x => !processedChilds.Contains(x)).ToList().ForEach(x => SetOffset(x, 0));
		}
		protected virtual double GetDragOffset() {
			return GetWidth(DragChild);
		}
		FrameworkElement GetOverChild() {
			double offset = CurrentX - StartX;
			double x = ChildXs.ElementAt(DragChildIndex) + offset;
			if(offset < 0) {
				if(x < 0 && DragChildIndex != 0)
					return Children.First();
				for(int i = DragChildIndex - 1; i >= 0; i--) {
					double childX = ChildXs.ElementAt(i);
					double childWidth = GetWidth(Children.ElementAt(i));
					if(x >= childX && x <= childX + childWidth) {
						if(x <= childX + childWidth / 2)
							return Children.ElementAt(i);
						else return i + 1 < DragChildIndex ? Children.ElementAt(i + 1) : null;
					}
				}
			}
			if(offset > 0) {
				x += GetWidth(DragChild);
				for(int i = DragChildIndex + 1; i < Children.Count(); i++) {
					double childX = ChildXs.ElementAt(i);
					double childWidth = GetWidth(Children.ElementAt(i));
					if(x >= childX && x <= childX + childWidth) {
						if(x >= childX + childWidth / 2)
							return Children.ElementAt(i);
						else return i - 1 > DragChildIndex ? Children.ElementAt(i - 1) : null;
					}
				}
				if(x > ChildXs.Last() && DragChildIndex != Children.Count() - 1)
					return Children.Last();
			}
			return null;
		}
		void GetCoordinates() {
			var childXs = new List<double>();
			double x = 0;
			foreach(FrameworkElement child in Children) {
				childXs.Add(x);
				x += GetWidth(child);
			}
			ChildXs = childXs;
			DragChildMinOffset = GetDragChildMinOffset();
			DragChildMaxOffset = GetDragChildMaxOffset();
		}
		protected virtual double GetDragChildMinOffset() {
			return -ChildXs.ElementAt(DragChildIndex);
		}
		protected virtual double GetDragChildMaxOffset() {
			return ChildXs.Last() + GetWidth(Children.Last()) - ChildXs.ElementAt(DragChildIndex) - GetWidth(DragChild);
		}
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ChildStoryboardProperty = DependencyProperty.RegisterAttached("ChildStoryboard", typeof(Storyboard), typeof(LocalDragPanelController<T, TVisual>), new PropertyMetadata(null));
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ChildOffsetProperty = DependencyProperty.RegisterAttached("ChildOffset", typeof(double), typeof(LocalDragPanelController<T, TVisual>), new PropertyMetadata(0d));
		static Storyboard GetChildStoryboard(FrameworkElement obj) { return (Storyboard)obj.GetValue(ChildStoryboardProperty); }
		static void SetChildStoryboard(FrameworkElement obj, Storyboard value) { obj.SetValue(ChildStoryboardProperty, value); }
		static double GetChildOffset(FrameworkElement obj) { return (double)obj.GetValue(ChildOffsetProperty); }
		static void SetChildOffset(FrameworkElement obj, double value) { obj.SetValue(ChildOffsetProperty, value); }
		void SetUpOffset() {
			foreach(FrameworkElement child in Children) {
				var transform = new TranslateTransform();
				child.RenderTransform = transform;
				if(child == DragChild) continue;
				Storyboard storyboard = new Storyboard();
				DoubleAnimation animation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromSeconds(0.2)) };
				Storyboard.SetTarget(animation, child);
				Storyboard.SetTargetProperty(animation, new PropertyPath(DragPanel.Orientation == Orientation.Horizontal ? "RenderTransform.X" : "RenderTransform.Y"));
				storyboard.Children.Add(animation);
				SetChildStoryboard(child, storyboard);
			}
		}
		void CleanUpOffset() {
			foreach(FrameworkElement child in Children) {
				SetChildOffset(child, 0d);
				GetChildStoryboard(child).Do(x => x.Children.Clear());
				SetChildStoryboard(child, null);
				child.RenderTransform = null;
			}
		}
		void SetOffset(FrameworkElement child, double offset) {
			double currentOffset = GetChildOffset(child);
			if(offset.AreClose(currentOffset)) return;
			SetChildOffset(child, offset);
			if(child == DragChild) {
				TranslateTransform transform = ((TranslateTransform)child.RenderTransform);
				if(DragPanel.Orientation == Orientation.Horizontal)
					((TranslateTransform)child.RenderTransform).X = offset;
				else ((TranslateTransform)child.RenderTransform).Y = offset;
				return;
			}
			var storyboard = GetChildStoryboard(child);
			var animation = (DoubleAnimation)storyboard.Children.First();
			animation.To = offset;
			if(!SuppressAnimation)
				storyboard.Begin(child);
			else {
				storyboard.Begin(child, true);
				storyboard.SkipToFill(child);
			}
		}
	}
	public class GlobalDragPanelController<T, TVisual> : LocalDragPanelController<T, TVisual>
		where T : class, IDragPanel
		where TVisual : FrameworkElement, IDragPanelVisual {
		DragWidgetWindow DragWidget;
		void InitializeAndShowThumb(DragWidgetWindow dragWidget, FrameworkElement dragObject) {
			DragWidget = dragWidget;
			DragWidget.Initialize(dragObject);
			Subscribe(DragWidget);
			DragWidget.UpdateWindowLocation();
			DragWidget.Opacity = 0;
			DragWidget.Show();
		}
		void UninitializeAndCloseThumb() {
			if(DragWidget == null) return;
			Unsubscribe(DragWidget);
			DragWidget.Close();
			DragWidget.Dispose();
			DragWidget = null;
		}
		protected override Point GetMousePosition() {
			return DragWidget != null ? DragControllerHelper.GetMousePositionOnScreen() : base.GetMousePosition();
		}
		protected override bool CanStartDrag(FrameworkElement dragChild) {
			if(dragChild == DragWidget) return true;
			return base.CanStartDrag(dragChild);
		}
		protected override void OnDragStarted() {
			if(DragWidget != null) {
				OnDrag();
				return;
			}
			base.OnDragStarted();
		}
		protected override void OnDragStopped() {
			currentWindow = null;
			if(DragWidget != null) return;
			base.OnDragStopped();
		}
		protected override void OnDrop() {
			currentWindow = null;
			if(DragWidget != null) {
				OnGlobalDrop();
				return;
			}
			base.OnDrop();
		}
		protected override void OnDrag() {
			if(DragWidget != null) {
				TVisual visualPanel = DragControllerHelper.FindDragPanel<TVisual>(currentWindow, CurrentPoint);
				if(visualPanel != null && DragControllerHelper.IsPointInsidePanel(visualPanel, visualPanel.PointFromScreen(CurrentPoint), false)) {
					StartDragTimer();
					if(shouldStartDrag) {
						StartLocalDrag(visualPanel);
						return;
					}
				}
				shouldStartDrag = false;
				OnGlobalDrag();
				return;
			}
			if(DragControllerHelper.IsPointOutOfPanel(VisualPanel, CurrentPoint, true)) {
				StartGlobalDrag();
				return;
			}
			base.OnDrag();
		}
		DispatcherTimer startDragTimer = null;
		bool shouldStartDrag = false;
		void StartDragTimer() {
			if(startDragTimer != null) return;
			startDragTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.16) };
			startDragTimer.Tick += OnStartDragTimerTick;
			startDragTimer.Start();
		}
		void OnStartDragTimerTick(object sender, EventArgs e) {
			shouldStartDrag = true;
			startDragTimer.Tick -= OnStartDragTimerTick;
			startDragTimer.Stop();
			OnDrag();
			startDragTimer = null;
		}
		void StartGlobalDrag() {
			if(DragChild == null) {
				StopDrag(true);
				return;
			}
			var dragChild = DragChild;
			StopDrag(false);
			DragWidgetWindow dragWidget = DragPanel.CreateDragWidget(dragChild);
			DragPanel.SetVisibility(dragChild, Visibility.Collapsed);
			InitializeAndShowThumb(dragWidget, dragChild);
			DispatcherHelper.DoEvents();
			StartDrag(dragWidget);
			if(Mouse.LeftButton == MouseButtonState.Released) StopDrag(true);
			else dragWidget.Opacity = 1;
		}
		void StartLocalDrag(TVisual visualPanel) {
			IDragPanel dragPanel = visualPanel.GetDragPanel(DragPanel);
			int dragOverIndex = DragControllerHelper.FindDragOverIndex(dragPanel, visualPanel, visualPanel.PointFromScreen(CurrentPoint));
			if(dragOverIndex == -1) return;
			var dragChild = DragWidget.DragObject;
			StopDrag(false);
			UninitializeAndCloseThumb();
			if(dragPanel == DragPanel)
				DragPanel.SetVisibility(dragChild, Visibility.Visible);
			else {
				DragPanel.SetVisibility(dragChild, Visibility.Visible);
				DragPanel.Remove(dragChild);
				dragChild = dragPanel.Insert(dragChild, dragOverIndex);
			}
			LocalDragPanelController<T, TVisual> dragController = dragPanel.Controller as LocalDragPanelController<T, TVisual>;
			if(dragController != null) {
				var startDragPoint = dragChild.TranslatePoint(new Point(), visualPanel);
				startDragPoint.X += dragChild.ActualWidth / 2;
				startDragPoint.Y += dragChild.ActualHeight / 2;
				dragController.SuppressAnimation = true;
				dragController.StartDrag(dragChild, startDragPoint);
				DispatcherHelper.UpdateLayoutAndDoEvents(VisualPanel);
				dragController.SuppressAnimation = false;
			}
		}
		void OnGlobalDrop() {
			var dragObject = DragWidget.DragObject;
			UninitializeAndCloseThumb();
			DragPanel.DropOnEmptySpace(dragObject);
		}
		void OnGlobalDrag() {
			DragWidget.UpdateWindowLocation();
			var w = DragControllerHelper.GetWindowUnderMouse(CurrentPoint);
			currentWindow = IsSupportedWindow(w) ? w : null;
			currentWindow.If(x => !x.IsActive).Do(x => x.Activate());
		}
		bool IsSupportedWindow(Window w) {
			if(w == null) return false;
			return DragDropRegionManager.GetDragDropControls(DragPanel.Region).OfType<FrameworkElement>().Select(x => Window.GetWindow(x)).Contains(w);
		}
		Window currentWindow;
	}
	public abstract class TabControlStretchViewDragPanelBase : IDragPanel {
		public TabPanelStretchView Panel { get; private set; }
		public TabControlStretchView View { get { return Panel.Owner; } }
		public TabControlStretchViewDragPanelBase(TabPanelStretchView panel) {
			Panel = panel;
		}
		public void RaiseChildrenChanged() {
			childrenChanged.Do(x => x(this, EventArgs.Empty));
		}
		protected abstract IEnumerable<FrameworkElement> Children { get; }
		protected abstract DragControllerBase Controller { get; }
		protected abstract int CorrectIndexWhenMove(FrameworkElement child, int index);
		IDragPanelVisual IDragPanel.VisualPanel { get { return Panel; } }
		string IDragPanel.Region { get { return View.DragDropRegion; } }
		Orientation IDragPanel.Orientation { get { return Panel.Orientation; } }
		IEnumerable<FrameworkElement> IDragPanel.Children { get { return this.Children; } }
		DragControllerBase IDragPanel.Controller { get { return this.Controller; } }
		event EventHandler IDragPanel.ChildrenChanged { add { childrenChanged += value; } remove { childrenChanged -= value; } }
		EventHandler childrenChanged;
		bool IDragPanel.CanStartDrag(FrameworkElement child) {
			return View.CanStartDrag((DXTabItem)child);
		}
		void IDragPanel.SetVisibility(FrameworkElement child, Visibility visibility) {
			View.SetVisibility((DXTabItem)child, visibility);
			FullUpdate();
		}
		FrameworkElement IDragPanel.Insert(FrameworkElement child, int index) {
			View.Insert((DXTabItem)child, index);
			FullUpdate();
			return Panel.GetActiveChild();
		}
		void IDragPanel.Remove(FrameworkElement child) {
			View.Remove((DXTabItem)child);
			FullUpdate();
		}
		FrameworkElement IDragPanel.Move(FrameworkElement child, int index) {
			index = CorrectIndexWhenMove(child, index);
			View.Move((DXTabItem)child, index);
			FullUpdate();
			return Panel.GetActiveChild();
		}
		void IDragPanel.DropOnEmptySpace(FrameworkElement child) {
			View.DropOnEmptySpace((DXTabItem)child);
		}
		DragWidgetWindow IDragPanel.CreateDragWidget(FrameworkElement child) {
			return View.DragWidgetHelper.CreateDragWidgetWindow((DXTabItem)child);
		}
		void IDragPanel.OnDragFinished() {
			View.OnDragFinished();
		}
		void FullUpdate() {
			Panel.InvalidateMeasure();
			Panel.InvalidateArrange();
			Panel.UpdateLayout();
		}
	}
	public class TabControlStretchViewLocalDragPanelController : LocalDragPanelController<TabControlStretchViewDragPanelBase, TabPanelStretchView> {
		protected override bool IsMouseLeftButtonDownOnDragChild(object sender, MouseButtonEventArgs e) {
			return DXTabItem.IsMouseLeftButtonDownOnDXTabItem(sender, e);
		}
		protected override bool StartDragDropOnHandledMouseEvents { get { return true; } }
	}
	public class TabControlStretchViewGlobalDragPanelController : GlobalDragPanelController<TabControlStretchViewDragPanelBase, TabPanelStretchView> {
		protected override bool IsMouseLeftButtonDownOnDragChild(object sender, MouseButtonEventArgs e) {
			return DXTabItem.IsMouseLeftButtonDownOnDXTabItem(sender, e);
		}
		protected override bool StartDragDropOnHandledMouseEvents { get { return true; } }
	}
	public class TabControlDragWidgetHelper {
		public static readonly DependencyProperty UseWPFMethodProperty = DependencyProperty.RegisterAttached("UseWPFMethod", typeof(bool), typeof(TabControlDragWidgetHelper), new PropertyMetadata(true));
		public static bool GetUseWPFMethod(DXTabItem obj) { return (bool)obj.GetValue(UseWPFMethodProperty); }
		public static void SetUseWPFMethod(DXTabItem obj, bool value) { obj.SetValue(UseWPFMethodProperty, value); }
		public virtual DragWidgetWindow CreateDragWidgetWindow(DXTabItem tab) {
			TabViewInfo viewInfo = new TabViewInfo(tab);
			var dragWidget = CreateDragWidget(tab);
			double x = -30;
			double y = -12;
			Point offsetDifference = new Point(x, y);
			if(viewInfo.HeaderLocation == HeaderLocation.Bottom)
				offsetDifference = new Point(x, -dragWidget.Height - y);
			else if(viewInfo.HeaderLocation == HeaderLocation.Right)
				offsetDifference = new Point(-dragWidget.Width - y, x);
			else if(viewInfo.HeaderLocation == HeaderLocation.Left)
				offsetDifference = new Point(y, x);
			return new DragWidgetWindow() { Content = dragWidget, OffsetDifference = offsetDifference, UseLayoutRounding = true };
		}
		protected virtual FrameworkElement CreateDragWidget(DXTabItem tab) {
			TabViewInfo viewInfo = new TabViewInfo(tab);
			DXTabControl tabControl = tab.Owner;
			double contentWidth = tabControl.View.StretchView.TabNormalSize + 80;
			var contentScreenshot = ScreenshotFromContent(tab.Owner, contentWidth, contentWidth, GetUseWPFMethod(tab));
			TabLayoutPanel.SetLayoutPosition(contentScreenshot, 1);
			var tabScreenshot = ScreenshotFromTab(tab);
			tabScreenshot.HorizontalAlignment = HorizontalAlignment.Left;
			tabScreenshot.VerticalAlignment = VerticalAlignment.Top;
			tabScreenshot.Margin = RotatableHelper.CorrectThickness(new Thickness(2, 0, 2, -1), viewInfo);
			TabLayoutPanel.SetLayoutPosition(tabScreenshot, 0);
			TabLayoutPanel panel = new TabLayoutPanel() { ViewInfo = viewInfo };
			panel.LayoutDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			panel.LayoutDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
			panel.Children.Add(contentScreenshot);
			panel.Children.Add(tabScreenshot);
			Size sz = CalcSize(contentScreenshot, tabScreenshot, panel.ViewInfo.Orientation);
			panel.Width = sz.Width;
			panel.Height = sz.Height;
			return panel;
		}
		Size CalcSize(FrameworkElement content, FrameworkElement tab, Orientation orientation) {
			Func<FrameworkElement, Size> getSize = x => {
				double width = x.Width + x.Margin.Left + x.Margin.Right;
				double height = x.Height + x.Margin.Top + x.Margin.Bottom;
				return new Size(width, height);
			};
			Size contentSize = getSize(content);
			Size tabSize = getSize(tab);
			Size res = new Size();
			if(orientation == Orientation.Horizontal) {
				res.Width = Math.Max(contentSize.Width, tabSize.Width);
				res.Height = contentSize.Height + tabSize.Height;
			} else {
				res.Width = contentSize.Width + tabSize.Width;
				res.Height = Math.Max(contentSize.Height, tabSize.Height);
			}
			return res;
		}
		protected virtual FrameworkElement ScreenshotFromContent(DXTabControl tabControl, double width, double maxHeight, bool useWPFMethod) {
			var viewInfo = new TabViewInfo(tabControl);
			Window window = Window.GetWindow(tabControl);
			var content = tabControl.GetLayoutChild("PART_ContentHost");
			double _width = width - 2;
			double _height = (_width / content.ActualWidth) * content.ActualHeight;
			FrameworkElement screenshot = null;
			if(!useWPFMethod)
				screenshot = ScreenshotFromScreen(content, _width, _height);
			else {
				VisualBrush contentBrush = new VisualBrush(window) {
					Viewbox = new Rect(content.TranslatePoint(new Point(), window), new Size(content.ActualWidth, content.ActualHeight)),
					ViewboxUnits = BrushMappingMode.Absolute
				};
				screenshot = ScreenshotFromVisualBrush(contentBrush, _width, _height);
			}
			_width = width;
			_height = (width / content.ActualWidth) * content.ActualHeight;
			double _maxHeight = maxHeight;
			double _maxWidth = double.PositiveInfinity;
			if(viewInfo.Orientation == Orientation.Vertical) {
				_width = _height; _height = width;
				_maxHeight = double.PositiveInfinity; _maxWidth = maxHeight;
			}
			Grid res = new Grid() { Width = Math.Min(_width, _maxWidth), Height = Math.Min(_height, _maxHeight) };
			res.Children.Add(new ContentPresenter() { Content = new TabViewInfo(tabControl), ContentTemplate = tabControl.BackgroundTemplate });
			res.Children.Add(new Border() { Padding = new Thickness(1), Child = screenshot });
			return res;
		}
		protected virtual FrameworkElement ScreenshotFromTab(DXTabItem tab) {
			TabViewInfo viewInfo = new TabViewInfo(tab);
			DXTabControl tabControl = tab.Owner;
			var panel = tabControl.TabPanel;
			double width = viewInfo.Orientation == Orientation.Horizontal ? tab.ActualWidth : panel.ActualWidth;
			double height = viewInfo.Orientation == Orientation.Horizontal ? panel.ActualHeight : tab.ActualHeight;
			var res = ScreenshotFromVisualBrush(new VisualBrush(tab), width, height);
			return res;
		}
		protected static Rectangle ScreenshotFromScreen(FrameworkElement control, double width, double height) {
			var doublePoint = control.PointToScreen(new Point());
			var leftTop = new System.Drawing.Point((int)Math.Floor(doublePoint.X), (int)Math.Floor(doublePoint.Y));
			var size = new System.Drawing.Size((int)Math.Ceiling(control.ActualWidth), (int)Math.Ceiling(control.ActualHeight));
			ImageBrush res;
			using(System.Drawing.Bitmap image = new System.Drawing.Bitmap(size.Width, size.Height)) {
				using(System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image)) {
					graphics.CopyFromScreen(leftTop.X, leftTop.Y, 0, 0, size);
					var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(image.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
					res = new ImageBrush(bitmapSource);
				}
			}
			return new Rectangle() { Width = width, Height = height, Fill = res };
		}
		protected static Rectangle ScreenshotFromVisualBrush(VisualBrush brush, double width, double height) {
			var presentationSource = PresentationSource.FromVisual(Window.GetWindow(brush.Visual));
			double dpiX = 96.0 * presentationSource.CompositionTarget.TransformToDevice.M11;
			double dpiY = 96.0 * presentationSource.CompositionTarget.TransformToDevice.M22;
			DrawingVisual d = new DrawingVisual();
			using(DrawingContext dc = d.RenderOpen()) {
				dc.DrawRectangle(brush, null, new Rect(0, 0, width, height));
			}
			RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Math.Ceiling(width), (int)Math.Ceiling(height), dpiX, dpiY, PixelFormats.Default);
			bitmap.Render(d);
			Rectangle res = new Rectangle() { Width = width, Height = height };
			res.Fill = new ImageBrush(BitmapFrame.Create(bitmap));
			return res;
		}
	}
}

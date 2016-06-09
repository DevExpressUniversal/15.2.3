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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraCharts.Design {
	[DXToolboxItem(false)]
	public class ChartDesignControl : Control, IChartContainer, IChartRenderProvider, IChartDataProvider, IChartInteractionProvider, IGestureClient {
		static readonly object objectSelected = new object();
		static readonly object selectedItemsChanged = new object();
		static readonly object objectHotTracked = new object();
		[DllImport("user32.dll")]
		static extern IntPtr GetDC(IntPtr hWnd);
		[DllImport("user32.dll")]
		static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
		ElementSelectionMode selectionMode = ElementSelectionMode.None;
		bool mouseWheelZooming = false;
		bool useHandCursor = false;
		bool canDisposeItems = true;
		object parentDataSource;
		Chart chart;
		ChartContainerType containerType = ChartContainerType.WinControl;
		ChartNavigationController navigationController = null;
		GestureHelper gestureHelper;
		Graphics windowGraphics = null;
		IntPtr windowDC = IntPtr.Zero;
		OpenGLGraphics openGLGraphics = null;
		DataContext dataContext = null;
		int lockChangeServiceCounter = 0;
		IServiceProvider serviceProvider;
		bool Mode3D { get { return chart != null && chart.Is3DDiagram; } }
		public bool UseHandCursor { get { return this.useHandCursor; } set { this.useHandCursor = value; } }
		public Chart Chart { get { return chart; } set { chart = value; } }
		public IPrintable Printable { get { return null; } }
		public bool IsDesignControl { get { return true; } }
		public ElementSelectionMode SelectionMode {
			get { return selectionMode; }
			set { selectionMode = value; }
		}
		public bool RuntimeRotation { get { return true; } }
		public ChartNavigationController NavigationController { get { return navigationController; } }
		public bool MouseWheelZooming {
			get { return mouseWheelZooming; }
			set { mouseWheelZooming = value; }
		}
		public bool CanDisposeItems {
			get { return canDisposeItems; }
			set { canDisposeItems = value; }
		}
		public ChartDesignControl(ChartContainerType containerType, object parentDataSource, DataContext dataContext) {
			this.containerType = containerType;
			this.parentDataSource = parentDataSource;
			this.dataContext = dataContext;
			SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.Opaque, true);
			InitializeDrawing();
			navigationController = new ChartNavigationController(this);
			mouseWheelZooming = true;
			gestureHelper = new GestureHelper(this);
		}
		public event HotTrackEventHandler ObjectSelected {
			add { Events.AddHandler(objectSelected, value); }
			remove { Events.RemoveHandler(objectSelected, value); }
		}
		public event SelectedItemsChangedEventHandler SelectedItemsChanged {
			add { Events.AddHandler(selectedItemsChanged, value); }
			remove { Events.RemoveHandler(selectedItemsChanged, value); }
		}
		public event HotTrackEventHandler ObjectHotTracked {
			add { Events.AddHandler(objectHotTracked, value); }
			remove { Events.RemoveHandler(objectHotTracked, value); }
		}
		event EventHandler IChartContainer.EndLoading { add { } remove { } }
		IChartDataProvider IChartContainer.DataProvider { get { return this; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return this; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return null; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return this; } }
		bool IChartContainer.ShowDesignerHints { get { return true; } }
		IServiceProvider IChartContainer.ServiceProvider { get { return serviceProvider; } }
		ISite IChartContainer.Site { get { return null; } set { } }
		IComponent IChartContainer.Parent { get { return base.Parent; } }
		bool IChartContainer.DesignMode { get { return true; } }
		bool IChartContainer.IsEndUserDesigner { get { return false; } }
		ChartContainerType IChartContainer.ControlType { get { return containerType; } }
		bool IChartContainer.Loading { get { return false; } }
		bool IChartContainer.ShouldEnableFormsSkins { get { return false; } }
		void IChartContainer.LockChangeService() {
			lockChangeServiceCounter++;
		}
		void IChartContainer.UnlockChangeService() {
			lockChangeServiceCounter--;
		}
		void IChartContainer.Changing() { }
		void IChartContainer.Changed() {
			if (lockChangeServiceCounter == 0)
				((IChartRenderProvider)this).Invalidate();
		}
		void IChartContainer.ShowErrorMessage(string message, string title) {
			string actualTitle = String.IsNullOrEmpty(title) ? Name : title;
			XtraMessageBox.Show((UserLookAndFeel)((IChartRenderProvider)this).LookAndFeel, message, actualTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		void IChartContainer.RaiseRangeControlRangeChanged(object min, object max, bool invalidate) { }
		bool IChartContainer.GetActualRightToLeft() {
			return WindowsFormsSettings.GetIsRightToLeft(this);
		}
		#region ISupportBarsInteraction implementation
		CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler { get { return null; } }
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { } remove { } }
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI { add { } remove { } }
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(ChartCommandId id) {
			return null;
		}
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			return false;
		}
		void ICommandAwareControl<ChartCommandId>.Focus() {
		}
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() {
		}
		void ISupportBarsInteraction.RaiseUIUpdated() { }
		object IServiceProvider.GetService(Type serviceType) {
			return null;
		}
		#endregion
		#region IGestureClient implementation
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			return navigationController.CheckAllowGestures(point);
		}
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) {
		}
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			overPan = navigationController.OnGesturePan(info.Current.Point, delta, info.IsBegin, info.IsEnd);
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) {
			navigationController.OnGestureRotation(degreeDelta);
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) {
		}
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) {
			navigationController.OnGestureZoom(info.Current.Point, zoomDelta, info.IsBegin);
		}
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		#endregion
		#region IChartDataProvider implementation
		event BoundDataChangedEventHandler IChartDataProvider.BoundDataChanged { add { } remove { } }
		object IChartDataProvider.DataAdapter { get { return null; } set { } }
		object IChartDataProvider.DataSource { get { return null; } set { } }
		object IChartDataProvider.ParentDataSource { get { return parentDataSource; } }
		DataContext IChartDataProvider.DataContext { get { return dataContext; } }
		bool IChartDataProvider.CanUseBoundPoints { get { return true; } }
		bool IChartDataProvider.SeriesDataSourceVisible { get { return false; } }
		void IChartDataProvider.OnBoundDataChanged(EventArgs e) { }
		bool IChartDataProvider.ShouldSerializeDataSource(object dataSource) { return false; }
		void IChartDataProvider.OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e) { }
		void IChartDataProvider.OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e) { }
		#endregion
		#region IChartRenderProvider implementation
		Rectangle IChartRenderProvider.DisplayBounds { get { return Bounds; } }
		bool IChartRenderProvider.IsPrintingAvailable { get { return false; } }
		object IChartRenderProvider.LookAndFeel { get { return UserLookAndFeel.Default; } }
		void IChartRenderProvider.Invalidate() {
			InitializeDrawing();
			InvalidateAndUpdate();
		}
		void IChartRenderProvider.InvokeInvalidate() {
			base.Invalidate();
		}
		Bitmap IChartRenderProvider.LoadBitmap(string url) { return null; }
		ComponentExporter IChartRenderProvider.CreateComponentPrinter(IPrintable iPrintable) {
			return null;
		}
		#endregion
		#region IChartInteractionProvider implementation
		bool IChartInteractionProvider.HitTestingEnabled { get { return selectionMode != ElementSelectionMode.None; } }
		ElementSelectionMode IChartInteractionProvider.SelectionMode { get { return selectionMode; } }
		SeriesSelectionMode IChartInteractionProvider.SeriesSelectionMode { get { return SeriesSelectionMode.Series; } }
		bool IChartInteractionProvider.EnableChartHitTesting { get { return true; } }
		bool IChartInteractionProvider.CanShowTooltips { get { return true; } }
		bool IChartInteractionProvider.DragCtrlKeyRequired { get { return true; } }
		Point IChartInteractionProvider.PointToCanvas(Point p) {
			return PointToScreen(p);
		}
		void IChartInteractionProvider.OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e) { }
		void IChartInteractionProvider.OnScroll(ChartScrollEventArgs e) { }
		void IChartInteractionProvider.OnScroll3D(ChartScroll3DEventArgs e) { }
		void IChartInteractionProvider.OnZoom(ChartZoomEventArgs e) { }
		void IChartInteractionProvider.OnZoom3D(ChartZoom3DEventArgs e) { }
		void IChartInteractionProvider.OnQueryCursor(QueryCursorEventArgs e) { }
		void IChartInteractionProvider.OnLegendItemChecked(LegendItemCheckedEventArgs e) { }
		void IChartInteractionProvider.OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e) { }
		#endregion
		GestureAllowArgs[] CheckAllowGestures(Point point, IAnnotationDragPoint annotationDragPoint) {
			if (annotationDragPoint != null) {
				Annotation annotation = annotationDragPoint.Annotation;
				List<GestureAllowArgs> gestures = new List<GestureAllowArgs>();
				if (annotation.RuntimeMoving)
					gestures.Add(GestureAllowArgs.Pan);
				if (annotation.RuntimeRotation)
					gestures.Add(GestureAllowArgs.Rotate);
				if (annotation.RuntimeResizing)
					gestures.Add(GestureAllowArgs.Zoom);
				return gestures.ToArray();
			}
			if (chart.Diagram != null) {
				Diagram3D diagram3D = chart.Diagram as Diagram3D;
				if (diagram3D != null) {
					List<GestureAllowArgs> gestures = new List<GestureAllowArgs>();
					if (diagram3D.RuntimeZooming)
						gestures.Add(GestureAllowArgs.Zoom);
					if (diagram3D.RuntimeRotation && diagram3D.RotationOptions.UseTouchDevice && diagram3D.RotationType != RotationType.UseAngles)
						gestures.Add(GestureAllowArgs.Rotate);
					gestures.Add(GestureAllowArgs.Pan);
					return gestures.ToArray();
				}
				XYDiagram2D xyDiagram2D = chart.Diagram as XYDiagram2D;
				if (xyDiagram2D != null) {
					List<GestureAllowArgs> gestures = new List<GestureAllowArgs>();
					if (chart.CanZoomIn || chart.CanZoomOut)
						gestures.Add(GestureAllowArgs.Zoom);
					if (xyDiagram2D.IsScrollingEnabled)
						gestures.Add(GestureAllowArgs.Pan);
					return gestures.ToArray();
				}
			}
			return GestureAllowArgs.None;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				CanDisposeItems = true;
				ReleaseOpenGLGraphics();
				if (chart != null) {
					Chart.Dispose();
					Chart = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override void OnHandleDestroyed(EventArgs e) {
			ReleaseOpenGLGraphics();
			base.OnHandleDestroyed(e);
		}
		protected override void OnPaint(PaintEventArgs e) {
			Rectangle bounds = new Rectangle(0, 0, Bounds.Width, Bounds.Height);
			if (Mode3D && openGLGraphics != null)
				chart.DrawContent(e.Graphics, openGLGraphics, bounds, false, true);
			else {
				if (chart.BackColor == Color.Transparent)
					using (Brush brush = new SolidBrush(Parent.BackColor))
						e.Graphics.FillRectangle(brush, Bounds);
				chart.DrawContent(e.Graphics, bounds);
				if (navigationController != null)
					navigationController.DrawZoomRectangle(e.Graphics);
			}
			base.OnPaint(e);
		}
		void InvalidateAndUpdate() {
			base.Invalidate();
			if (Mode3D)
				Update();
		}
		public void Assign(Chart chart) {
			this.chart.Assign(chart);
		}
		public void OnObjectSelected(HotTrackEventArgs e) {
			HotTrackEventHandler handler = (HotTrackEventHandler)this.Events[objectSelected];
			if (handler != null)
				handler(this, e);
		}
		public void OnObjectHotTracked(HotTrackEventArgs e) {
			HotTrackEventHandler handler = (HotTrackEventHandler)this.Events[objectHotTracked];
			if (handler != null)
				handler(this, e);
		}
		public void OnSelectedItemsChanged(SelectedItemsChangedEventArgs e) {
			SelectedItemsChangedEventHandler handler = (SelectedItemsChangedEventHandler)this.Events[selectedItemsChanged];
			if (handler != null)
				handler(this, e);
		}
		public void SetServiceProvider(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		[System.Security.SecuritySafeCritical]
		void InitializeDrawing() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, !Mode3D);
			if (Mode3D) {
				if (openGLGraphics == null && chart != null) {
					if (windowDC == IntPtr.Zero)
						windowDC = GetDC(Handle);
					if (windowGraphics == null)
						windowGraphics = Graphics.FromHdc(windowDC);
					openGLGraphics = chart.CreateNativeGraphics(windowGraphics, windowDC, Size, GraphicsQuality.Lowest) as OpenGLGraphics;
				}
			}
			else
				ReleaseOpenGLGraphics();
		}
		[System.Security.SecuritySafeCritical]
		void ReleaseOpenGLGraphics() {
			if (chart != null)
				chart.DisposeDrawingHelper();
			if (openGLGraphics != null) {
				openGLGraphics.Dispose();
				openGLGraphics = null;
			}
			if (windowGraphics != null) {
				windowGraphics.Dispose();
				windowGraphics = null;
			}
			if (windowDC != IntPtr.Zero) {
				ReleaseDC(Handle, windowDC);
				windowDC = IntPtr.Zero;
			}
		}
		void SelectObjectByMouse(int x, int y) {
			if (!(chart.Diagram is Diagram3D))
				chart.SelectObjectsAt(new Point(x, y));
		}
		void MoveMouseOverObject(int x, int y) {
			if (!(chart.Diagram is Diagram3D))
				chart.HighlightObjectsAt(new Point(x, y));
		}
		void SetCursor(bool doDrag) {
			if (this.useHandCursor) {
				Cursor = doDrag ? DragCursors.HandDragCursor : DragCursors.HandCursor;
			}
			else
				Cursor = Cursors.Default;
		}
		protected override void OnSizeChanged(EventArgs e) {
			chart.ResetGraphicsCache();
			if (openGLGraphics != null)
				openGLGraphics.Size = Size;
			base.OnSizeChanged(e);
			chart.InvalidateDrawingHelper();
			((IChartRenderProvider)this).Invalidate();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			navigationController.OnKeyDown(e);
			base.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			navigationController.OnKeyUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			navigationController.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			navigationController.OnMouseLeave(e);
			base.OnMouseLeave(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if (TabStop)
				Focus();
			navigationController.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			navigationController.OnMouseUp(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			if (mouseWheelZooming)
				navigationController.OnMouseWheel(e);
		}
		protected override void WndProc(ref Message m) {
			if (!gestureHelper.WndProc(ref m))
				base.WndProc(ref m);
		}
		protected override void OnCursorChanged(EventArgs e) {
			base.OnCursorChanged(e);
			navigationController.OnCursorChanged(Cursor);
		}
		private void InitializeComponent() {
			this.SuspendLayout();
			this.ResumeLayout(false);
		}
	}
}

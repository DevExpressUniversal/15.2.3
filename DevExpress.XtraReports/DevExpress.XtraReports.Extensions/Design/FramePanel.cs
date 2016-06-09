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
using System.Text;
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Utils.Drawing;
using System.Drawing.Design;
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;
using DevExpress.XtraReports.Native;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Drawing.Printing;
using DevExpress.XtraEditors;
using System.ComponentModel;
using DevExpress.Utils.Drawing.Helpers;
using System.Diagnostics;
using DevExpress.XtraReports.Design.SnapLines;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraEditors.Drawing;
namespace DevExpress.XtraReports.Design {
	public interface IBandViewInfoService {
		Control View { get; }
		BandViewInfo[] ViewInfos { get; }
		RectangleF GetControlViewClientBounds(XRControl xrControl, Band band);
		RectangleF GetControlScreenBounds(XRControl xrControl, BandViewInfo viewInfo);
		RectangleF GetControlScreenBounds(XRControl xrControl);
		RectangleF GetControlViewClientBounds(XRControl xrControl);
		XRControl GetControlByScreenPoint(PointF screenPoint);
		XRControl GetContainerDropTarget(PointF screenPoint, XRControl[] dropControls);		
		BandViewInfo GetViewInfoByBand(Band band);
		BandViewInfo GetViewInfoByScreenPoint(PointF screenPoint);
		BandViewInfo GetNearestViewInfoByScreenPoint(PointF pt);
		void StartBandHeightChanging(int index);
		void StartWidthChanging(BandViewInfo viewInfo, Point screenPoint);
		PointF PointToClient(PointF screenPoint, DragDataObject dragData, XRControl ctlContainer);
		PointF PointToClientRelativeToBand(PointF basePoint, DragDataObject dragData);
		void InvalidateViewInfo();
		void Invalidate();
		void Invalidate(RectangleF rect);
		void UpdateView();
		void InvalidateControlView(XRControl xrcontrol);
		RectangleF RectangleFToClient(RectangleF rect);
		RectangleF RectangleFToScreen(RectangleF rect);
	}
	class FramePanel : System.Windows.Forms.Panel, IBandViewInfoService, IToolTipControlClient, IMouseWheelSupportIgnore {
		#region inner classes
		delegate void XRMouseEventHandler(IMouseTarget mouseTarget, Control control, BandMouseEventArgs e);
		class MouseEventHelper {
			public static void HandleMouseDown(IMouseTarget mouseTarget, Control control, BandMouseEventArgs args) {
				if(mouseTarget != null) 
					mouseTarget.HandleMouseDown(control, args);
			}
			public static void HandleMouseUp(IMouseTarget mouseTarget, Control control, BandMouseEventArgs args) {
				if(mouseTarget != null)
					mouseTarget.HandleMouseUp(control, args);
			}
			public static void HandleMouseMove(IMouseTarget mouseTarget, Control control, BandMouseEventArgs args) {
				if(mouseTarget != null)
					mouseTarget.HandleMouseMove(control, args);
			}
			public static void HandleDoubleClick(IMouseTarget mouseTarget, Control control, BandMouseEventArgs args) {
				if(mouseTarget != null)
					mouseTarget.HandleDoubleClick(control, args);
			}
			IMouseTarget lastMouseTarget;
			IDesignerHost host;
			FrameSelectionUIService selectionUIService;
			XRMouseEventHandler mouseHandler;
			Control control;
			public MouseEventHelper(IDesignerHost host, Control control) {
				this.host = host;
				this.control = control;
				selectionUIService = (FrameSelectionUIService)host.GetService(typeof(FrameSelectionUIService));
			}
			public bool HandleSelectionMouseEvent(Band band, XRMouseEventHandler mouseHandler, BandMouseEventArgs e) {
				try {
					this.mouseHandler = mouseHandler;
					bool handled = false;
					HandleMouseEvent(CreateSelectionEnumerator(band), e, ref handled);
					return handled;
				} finally {
					this.mouseHandler = null;
				}
			}
			public bool HandleMouseEvent(Band band, XRMouseEventHandler mouseHandler, BandMouseEventArgs e) {
				try {
					this.mouseHandler = mouseHandler;
					bool handled = false;
					HandleMouseEvent(CreateEnumerator(band), e, ref handled);
					return handled;
				} finally {
					this.mouseHandler = null;
				}
			}
			public void HandleMouseLeaveEvent() {
				FireMouseLeaveEventForLastTarget();
				lastMouseTarget = null;
			}
			void FireMouseLeaveEventForLastTarget() {
				if(lastMouseTarget != null && !lastMouseTarget.IsDisposed)
					lastMouseTarget.HandleMouseLeave(control, EventArgs.Empty);
			}
			private IEnumerator CreateSelectionEnumerator(Band band) {
				return selectionUIService.GetBandSelectionItems(band).GetEnumerator();
			}
			static IEnumerator CreateEnumerator(Band band) {
				ArrayList list = new ArrayList();
				foreach(XRCrossBandControl cbControl in band.RootReport.CrossBandControls) {
					if(cbControl.IsInsideBand(band))
						list.Add(cbControl);
				}
				list.Add(band);
				return list.GetEnumerator();
			}
			private void HandleMouseEvent(IEnumerator en, BandMouseEventArgs e, ref bool handled) {
				if(en == null)
					return;
				IMouseTarget mouseTarget;
				while(en.MoveNext()) {
					mouseTarget = GetMouseTarget(en.Current);
					Point pt = new Point(e.X, e.Y);
					if(mouseTarget == null || !mouseTarget.ContainsPoint(pt, e.ViewInfo))
						continue;
					if(en.Current is IEnumerable) {
						XRControlDesignerBase designer = host.GetDesigner((IComponent)en.Current) as XRControlDesignerBase;
						if(designer != null) {
							IEnumerator enumerator = designer.GetEnumerator();
							HandleMouseEvent(enumerator, e, ref handled);
						}
					}
					if(mouseHandler == null || mouseHandler.Method == null)
						continue;
					if((mouseHandler.Method.Name == "HandleMouseMove" || mouseHandler.Method.Name == "HandleMouseDown")  && ShouldHandleByParent(en.Current as XRControl, pt))
						break;
					if(!handled) {
						mouseHandler(mouseTarget, control, e);
						if(!object.Equals(lastMouseTarget, mouseTarget))
							FireMouseLeaveEventForLastTarget();
						lastMouseTarget = mouseTarget;
						handled = true;
						break;
					}
				}
			}
			private IMouseTarget GetMouseTarget(object obj) {
				return (obj is XRControl) ? MouseTargetService.GetMouseTarget(host, (XRControl)obj):
					(obj is IMouseTarget) ? (IMouseTarget)obj : null;
			}
			bool ShouldHandleByParent(XRControl xrControl, Point pt) {
				if(xrControl == null || !(xrControl.Parent is XRTableCell))
					return false;
				IBandViewInfoService service = host.GetService(typeof(IBandViewInfoService)) as IBandViewInfoService;
				if(service == null)
					return false;
				const int delta = 5;
				XRTableCell cell = xrControl.Parent as XRTableCell;
				RectangleF rect = service.GetControlViewClientBounds(cell);
				if(XRControlDesignerBase.IsFirstChild(cell) && FloatsComparer.Default.FirstEqualsSecond(xrControl.LeftF, 0) && pt.X < rect.Left + delta)
					return true;
				if(XRControlDesignerBase.IsLastChild(cell) && FloatsComparer.Default.FirstEqualsSecond(xrControl.RightF, cell.WidthF) && pt.X > rect.Right - delta)
					return true;
				if(XRControlDesignerBase.IsFirstChild(cell.Row) && FloatsComparer.Default.FirstEqualsSecond(xrControl.TopF, 0) && pt.Y < rect.Top + delta)
					return true;
				if(XRControlDesignerBase.IsLastChild(cell.Row) && FloatsComparer.Default.FirstEqualsSecond(xrControl.BottomF, cell.HeightF) && pt.Y > rect.Bottom - delta)
					return true;
				return false;
			}
		}
		#endregion
		private IDesignerHost host;
		private BandViewInfo[] viewInfos;
		private FrameSelectionUIService selectionUIService;
		private MouseEventHelper mouseEventHelper;
		private ReportFrame reportFrame;
		private DragController dragController;
		private IDragHandler dragHandler;
		ZoomService zoomService;
		PointF viewVisibleBoundsCenter = PointF.Empty;
		bool canUpdateViewVisibleBoundsCenter = true;
		int lastZoomFactor = ZoomService.DefaultZoomFactorInPercents;
		bool onPaintEntered;
		ObjectPainter painter;
		EditPanelViewInfo panelViewInfo;
		ViewInfoBuilder viewInfoBuilder;
		int captionsCountBeforePoint = 0;
		PointF zoomOffset = PointF.Empty;
		public event ExceptionEventHandler NotEnoughMemoryToPaint;
		public Rectangle BandBounds {
			get {
				return panelViewInfo != null ? panelViewInfo.Bounds : Rectangle.Empty;
			}
		}
		XtraReport Report {
			get { return (XtraReport)host.RootComponent; }
		}
		ReportDesigner ReportDesigner {
			get { return (ReportDesigner)host.GetDesigner(Report); }
		}
		public FramePanel(IDesignerHost host, ReportFrame reportFrame) {
			Dock = DockStyle.None;
			AllowDrop = true;
			this.host = host;
			this.reportFrame = reportFrame;
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlConstants.DoubleBuffer, true);
			selectionUIService = new FrameSelectionUIService(host, this);
			host.AddService(typeof(FrameSelectionUIService), selectionUIService);
			mouseEventHelper = new MouseEventHelper(host, this);
			dragController = new DragController(host, this);
			IDragDropService dragDropService = (IDragDropService)host.GetService(typeof(IDragDropService));
			dragHandler = dragDropService.GetDragHandler(null);
			zoomService = ZoomService.GetInstance(host);
			ToolTipController.DefaultController.AddClientControl(this);
		}
		public void Invalidate(RectangleF rect) {
			base.Invalidate(RectHelper.InflateRectFToInteger(rect));
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ToolTipController.DefaultController.RemoveClientControl(this);
				if(host != null) {
					host.RemoveService(typeof(FrameSelectionUIService));
				}
				selectionUIService.Dispose();
				if(dragController != null) {
					dragController.Dispose();
					dragController = null;
				}
				DisposeViewInfoBuilder();
			}
			base.Dispose(disposing);
		}
		void DisposeViewInfoBuilder() {
			if(viewInfoBuilder != null) {
				viewInfoBuilder.Dispose();
				viewInfoBuilder = null;
			}
		}
		#region IBandViewInfoService
		void IBandViewInfoService.StartBandHeightChanging(int index) {
			reportFrame.StartBandHeightChanging(index);
		}
		void IBandViewInfoService.StartWidthChanging(BandViewInfo viewInfo, Point screenPoint) {
			reportFrame.StartWidthChanging(viewInfo, screenPoint);
		}
		public BandViewInfo[] ViewInfos {
			get {
				if(viewInfos == null) {
					CreateViewInfos(reportFrame.LookAndFeel);
				}
				return viewInfos;
			}
		}
		public Control View {
			get { return this; }
		}
		public PointF ZoomOffset {
			get { return zoomOffset; }
			set { zoomOffset = value; }
		}
		public RectangleF RectangleFToScreen(RectangleF rect) {
			rect.Location = PointFToScreen(rect.Location);
			return rect;
		}
		PointF PointFToScreen(PointF point) {
			Point pt = PointToScreen(new Point(0, 0));
			point.X += pt.X;
			point.Y += pt.Y;
			return point;
		}
		public RectangleF RectangleFToClient(RectangleF rect) {
			rect.Location = PointFToClient(rect.Location);
			return rect;
		}
		PointF PointFToClient(PointF point) {
			Point pt = PointToClient(new Point(0, 0));
			point.X += pt.X;
			point.Y += pt.Y;
			return point;
		}
		RectangleF RectangleFToScreen(RectangleF rect, BandViewInfo viewInfo) {
			if(viewInfo != null)
				rect.Offset(viewInfo.ClientBandBounds.Location);
			return RectangleFToScreen(rect);
		}
		public RectangleF GetControlScreenBounds(XRControl xrControl, BandViewInfo viewInfo) {
			float dpi = ZoomService.GetInstance(host).ScaleValueF(GraphicsDpi.Pixel);
			RectangleF rect = xrControl.RectangleFToBand(xrControl.ClientRectangleF, dpi);
			return RectangleFToScreen(rect, viewInfo);
		}
		public RectangleF GetControlScreenBounds(XRControl xrControl) {
			IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(xrControl, host);
			return adapter.GetScreenBounds();
		}
		public RectangleF GetControlViewClientBounds(XRControl xrControl, Band band) {
			IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(xrControl, host); 
			return adapter != null ? adapter.GetClientBandBounds(this.GetViewInfoByBand(band)) : RectangleF.Empty;
		}
		public void InvalidateControlView(XRControl xrControl) {
			Invalidate(GetControlViewClientBounds(xrControl));
		}
		public RectangleF GetControlViewClientBounds(XRControl xrControl) {
			IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(xrControl, host);
			return adapter != null ? adapter.GetClientBandBounds() : RectangleF.Empty;
		}
		public void InvalidateViewInfo() {
			viewInfos = null;
		}
		public XRControl GetControlByScreenPoint(PointF pt) {
			BandViewInfo viewInfo = GetViewInfoByScreenPoint(pt);
			if(viewInfo == null)
				return null;
			PointF clientPoint = PointFToClient(pt);
			XRControl control = GetControlByPoint(viewInfo.Band.Controls, viewInfo.PointToBandDpi(clientPoint));
			return (control != null) ? control : viewInfo.Band;
		}
		public BandViewInfo GetViewInfoByBand(Band band) {
			foreach(BandViewInfo viewInfo in ViewInfos)
				if(Comparer.Equals(band, viewInfo.Band))
					return viewInfo;
			return null;
		}
		public BandViewInfo GetViewInfoByScreenPoint(PointF pt) {
			return GetViewInfoByClientPoint(PointFToClient(pt));
		}
		BandViewInfo GetViewInfoByClientPoint(PointF pt) {
			foreach(BandViewInfo viewInfo in ViewInfos)
				if(RectHelper.RectangleFContains(viewInfo.BoundsF, pt, 3))
					return viewInfo;
			return null;
		}
		int GetCaptionsCountBeforePoint(PointF pt) {
			PointF clientPoint = PointFToClient(pt);
			int result = 0;
			for(int i = 0; i < ViewInfos.Length; i++) {
				if(!(ViewInfos[i].Band is TopMarginBand || ViewInfos[i].Band is BottomMarginBand))
					result++;
				if(RectHelper.RectangleFContains(ViewInfos[i].BoundsF, clientPoint, 3))
					return result;
			}
			return 0;
		}
		public BandViewInfo GetNearestViewInfoByScreenPoint(PointF pt) {
			pt = PointFToClient(pt);
			if(this.ViewInfos.Length > 0) {
				RectangleF rect = Rectangle.Union(this.ViewInfos[0].Bounds, this.ViewInfos[this.ViewInfos.Length - 1].Bounds);
				if(!rect.Contains(pt)) {
					if(pt.Y < rect.Y)
						return this.ViewInfos[0];
					if(pt.Y > rect.Bottom)
						return this.ViewInfos[this.ViewInfos.Length - 1];
					pt.X = Math.Max(rect.Left, Math.Min(pt.X, rect.Right - 1));
					pt.Y = Math.Max(rect.Top, Math.Min(pt.Y, rect.Bottom - 1));
				}
				return this.GetViewInfoByClientPoint(pt);
			}
			return null;
		}
		public void UpdateView() {
			reportFrame.BeginUpdate();
			try {
				InvalidateViewInfo();
				CreateViewInfos(reportFrame.LookAndFeel);
			} finally {
				reportFrame.EndUpdate();
			}
			reportFrame.UpdateView();
			painter = CreatePainter(reportFrame.LookAndFeel);
		}
		static ObjectPainter CreatePainter(UserLookAndFeel lookAndFeel) {
			return ReportPaintStyles.GetPaintStyle(lookAndFeel).CreateEditPanelPainter(lookAndFeel);
		}
		public PointF PointToClientRelativeToBand(PointF basePoint, DragDataObject dragData) {
			PointF snappedPoint = basePoint;
			if(SnapLineHelper.IsSnappingAllowed) {
				if(dragData != null && Report.SnappingMode == SnappingMode.SnapLines && dragData.PrimaryControl.SupportSnapLines) {
					XRControl parent = GetContainerDropTarget(Control.MousePosition, dragData.Controls);
					PointF snapDiffPoint = ControlDragHandler.GetPrimaryDragControlSnapDiffPoint(snappedPoint, dragData.PrimaryControl, parent.Band, dragData.Controls, ReportDesigner.RootReport, host);
					snapDiffPoint = ZoomService.GetInstance(this.host).ToScaledPixels(snapDiffPoint, dragData.PrimaryControl.Dpi);
					snappedPoint.X += snapDiffPoint.X;
					snappedPoint.Y += snapDiffPoint.Y;
				}
				if(Report.SnapToGrid && Report.SnappingMode == SnappingMode.SnapToGrid) {
					snappedPoint = SnapPointToGridSize(snappedPoint);
				}
			}
			return snappedPoint;
		}		
		public PointF PointToClient(PointF screenPoint, DragDataObject dragData, XRControl ctlContainer) {
			PointF basePoint = (dragData != null) ? dragData.EvalBasePoint(screenPoint.X, screenPoint.Y) : screenPoint;
			PointF scrPoint = PointToClientRelativeToBand(basePoint, dragData);
			RectangleF rect = GetControlScreenBounds(ctlContainer);
			scrPoint.X -= rect.X;
			scrPoint.Y -= rect.Y;
			return scrPoint;
		}
		public XRControl GetContainerDropTarget(PointF screenPoint, XRControl[] dropControls) {
			XRControl target = GetContainerDropTarget(screenPoint);
			while(target != null) {
				if(IsValidDropTarget(target, dropControls))
					return target;
				else
					target = target.Parent;
			}
			return target;
		}
		#endregion
		PointF SnapPointToGridSize(PointF screenPoint) {
			return SnapPointToGridSize(screenPoint, GetNearestViewInfoByScreenPoint(screenPoint));
		}
		PointF SnapPointToGridSize(PointF screenPoint, BandViewInfo viewInfo) {
			return SnapPointToGridSize(screenPoint, viewInfo, Divider.DefaultInstance);
		}
		PointF SnapPointToGridSize(PointF screenPoint, BandViewInfo viewInfo, Divider divider) {
			if(viewInfo == null)
				return screenPoint;
			Rectangle bandBounds = viewInfo.ClientBandBounds;
			Point pos = PointToScreen(bandBounds.Location);
			screenPoint.X -= pos.X;
			screenPoint.Y -= pos.Y;
			PointF pt = divider.GetDivisiblePointF(screenPoint, ReportDesigner.ScaledGridSize);
			pt.X += pos.X;
			pt.Y += pos.Y;
			return pt;
		}
		XRControl GetContainerDropTarget(PointF screenPoint) {
			XRControl xrControl = GetControlByScreenPoint(screenPoint);
			return (xrControl == null) ? null : ((XRControlDesignerBase)host.GetDesigner(xrControl)).CanHaveChildren ?
				xrControl : xrControl.Parent;
		}
		static bool IsValidDropTarget(XRControl target, XRControl[] dropControls) {
			foreach(XRControl control in dropControls)
				if(!target.CanAddComponent(control))
					return false;
			return true;
		}
		#region IToolTipControlClient
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(System.Drawing.Point point) {
			ToolTipControlInfo info = new ToolTipControlInfo(reportFrame, ToolTipService.GetInstance(host).ToolTipText);
			info.ToolTipType = ToolTipType.SuperTip;
			return info;
		}
		bool IToolTipControlClient.ShowToolTips { get { return ToolTipService.GetInstance(host).IsToolTipActive; } }
		#endregion
		#region drag and drop
		protected override void OnDragEnter(DragEventArgs e) {
			IDragDropService dragDropService = (IDragDropService)host.GetService(typeof(IDragDropService));
			dragHandler = dragDropService.GetDragHandler(e.Data);
			dragHandler.HandleDragEnter(this, e);
		}
		protected override void OnDragOver(DragEventArgs e) {
			dragHandler.HandleDragOver(this, e);
		}
		protected override void OnDragDrop(DragEventArgs e) {
			dragHandler.HandleDragDrop(this, e);
			if(reportFrame != null && !reportFrame.IsDisposed)
				reportFrame.Focus();
		}
		protected override void OnDragLeave(EventArgs e) {
			dragHandler.HandleDragLeave(this, e);
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e) {
			dragHandler.HandleGiveFeedback(this, e);
		}
		#endregion
		void CreateViewInfos(UserLookAndFeel lookAndFeel) {
			Size indent = ReportPaintStyles.GetFullPageIndent(lookAndFeel);
			DisposeViewInfoBuilder();
			if(host != null && host.RootComponent != null) {
				viewInfoBuilder = new ViewInfoBuilder(host);
				viewInfos = viewInfoBuilder.Build(new Point(indent));
			} else {
				viewInfos = new BandViewInfo[] { };
			}
			UpdateCaptionsCount();
			UpdateViewBounds(lookAndFeel);
			reportFrame.UpdateRulers();
		}
		void UpdateViewBounds(UserLookAndFeel lookAndFeel) {
			SuspendUpdateViewVisibleBoundsCenter();
			Rectangle viewBounds = GetViewBounds();
			MinimumSize = new Size(viewBounds.Right, viewBounds.Bottom);
			if(Parent != null)
				((XtraScrollableControl)Parent).AutoScrollMinSize = MinimumSize;
			Margins borders = ReportPaintStyles.GetPageBorders(lookAndFeel);
			panelViewInfo = new EditPanelViewInfo(viewBounds, borders);
			if(lastZoomFactor != zoomService.ZoomFactorInPercents) {
				SetVisibleBoundsCenter();
				lastZoomFactor = zoomService.ZoomFactorInPercents;
			}
			ResumeUpdateViewVisibleBoundsCenter();
		}
		internal void UpdateViewVisibleBoundsCenter() {
			if(canUpdateViewVisibleBoundsCenter) {
				viewVisibleBoundsCenter = RectHelper.CenterOf(GetVisibleBounds());
				viewVisibleBoundsCenter = zoomService.UnscalePointF(viewVisibleBoundsCenter);
			}
		}
		void UpdateCaptionsCount() {
			captionsCountBeforePoint = GetCaptionsCountBeforePoint(MousePosition);
		}
		void SuspendUpdateViewVisibleBoundsCenter() {
			canUpdateViewVisibleBoundsCenter = false;
		}
		void ResumeUpdateViewVisibleBoundsCenter() {
			zoomOffset = PointF.Empty;
			canUpdateViewVisibleBoundsCenter = true;
			UpdateViewVisibleBoundsCenter();
		}
		Rectangle GetVisibleBounds() {
			return new Rectangle(
				-Location.X,
				-Location.Y,
				Math.Min(Parent.ClientRectangle.Width, Size.Width + Location.X),
				Math.Min(Parent.ClientRectangle.Height, Size.Height + Location.Y)
				);
		}
		void SetVisibleBoundsCenter() {
			PointF center = zoomService.ScalePointF(viewVisibleBoundsCenter);
			Size visibleSize = Parent.ClientRectangle.Size;
			center.X -= visibleSize.Width / 2;
			center.Y -= visibleSize.Height / 2;
			Point offset = new Point(Math.Min(0, -(int)center.X) - Location.X, Math.Min(0, -(int)center.Y) - Location.Y);
			if(!zoomOffset.IsEmpty) {
				float deltaZoom = (zoomService.ZoomFactorInPercents - lastZoomFactor) / 100f;
				float offsetForCaptions = captionsCountBeforePoint * ReportFrame.CaptionHeight * deltaZoom;
				float lastZoom = lastZoomFactor / 100f;
				offset.X -= (int)((zoomOffset.X * deltaZoom) / lastZoom);
				offset.Y -= (int)((zoomOffset.Y * deltaZoom) / lastZoom);
				offset.Y += (int)(offsetForCaptions / lastZoom);
			}
			reportFrame.SetScrollOffset(offset);
		}
		private XRControlDesignerBase GetDesignerByPoint(Point screenPoint, Band band) {
			XRControl xrControl = GetControlByScreenPoint(screenPoint);
			if(xrControl == null)
				xrControl = band;
			return (XRControlDesignerBase)host.GetDesigner(xrControl);
		}
		private XRControl GetControlByPoint(XRControlCollection xrControls, PointF pt) {
			for(int i = 0; i < xrControls.Count; i++) {
				if(xrControls[i].BoundsF.Contains(pt)) {
					pt.X -= xrControls[i].LeftF;
					pt.Y -= xrControls[i].TopF;
					XRControl xrControl = GetControlByPoint(xrControls[i].Controls, pt);
					if(xrControl != null) {
						return xrControl;
					}
					return xrControls[i];
				}
			}
			return null;
		}
		private Rectangle GetViewBounds() {
			Rectangle rect = Rectangle.Empty;
			for(int i = 0; i < ViewInfos.Length; i++) {
				BandViewInfo viewInfo = ViewInfos[i];
				if(i == 0)
					rect = viewInfo.Bounds;
				else
					rect = Rectangle.Union(rect, viewInfo.Bounds);
			}
			return rect;
		}
		bool IsViewInfoValid {
			get {
				foreach(BandViewInfo info in ViewInfos)
					if(info.Band == null || info.Band.IsDisposed)
						return false;
				return true;
			}
		}
		static bool IsInSerializaton(IComponent rootComponent) {
			return rootComponent.Site is DevExpress.XtraReports.Serialization.XRDesignSite;
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(onPaintEntered || host == null || Report == null || Report.IsDisposed)
				return;
			onPaintEntered = true;
			try {
				base.OnPaint(e);
				if(host.Loading || IsInSerializaton(Report) || !IsViewInfoValid) {
					Invalidate(e.ClipRectangle);
					return;
				}
				Rectangle oldClipRectangle = e.ClipRectangle;
				Rectangle clipRectangle = Rectangle.Intersect(e.ClipRectangle, BandBounds);
				using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
					if(painter != null && panelViewInfo != null) {
						ObjectPainter.DrawObject(cache, painter, panelViewInfo);
					}
					e.Graphics.Clip = new Region(clipRectangle);
					for(int i = 0; i < ViewInfos.Length; i++) {
						BandViewInfo viewInfo = ViewInfos[i];
						if(viewInfo.Band == null)
							break;
						if(!clipRectangle.IntersectsWith(viewInfo.Bounds))
							continue;
						viewInfo.DrawCaption(cache);
						if(clipRectangle.IntersectsWith(viewInfo.BandBounds))
							viewInfo.DrawBand(cache);
					}
					foreach(BandViewInfo bandViewInfo in ViewInfos) {
						bandViewInfo.DrawControlWarnings(cache);
					}
					e.Graphics.Clip = new Region(oldClipRectangle);
					DrawControlSelection(cache, ViewInfos);
					AdornerService adornerSrvice = host.GetService(typeof(AdornerService)) as AdornerService;
					if(adornerSrvice != null) {
						adornerSrvice.OnPaint(cache);
					}
				}
			} catch(Exception ex) {
				if(ExceptionHelper.IsCriticalException(ex)) {
					onPaintEntered = false;
					throw;
				}
			} finally {
				onPaintEntered = false;
			}
		}
		void DrawControlSelection(GraphicsCache cache, BandViewInfo[] bandViewInfos) {
			foreach(BandViewInfo bandViewInfo in bandViewInfos) {
				bandViewInfo.DrawSelection(cache);
			}
			foreach(BandViewInfo bandViewInfo in bandViewInfos) {
				selectionUIService.DrawSelectionItems(cache.Graphics, bandViewInfo.Band, bandViewInfo.Expanded);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			try {
				base.OnMouseDown(e);
				if(host.IsDebugging()) return;
				Parent.Focus();				
				BandViewInfo viewInfo = GetNearestViewInfoByScreenPoint(MousePosition);
				Point pt = PointToClient(MousePosition);
				if(viewInfo == null)
					return;
				if(HandleMouseEvent(viewInfo, new XRMouseEventHandler(MouseEventHelper.HandleMouseDown), e.Button))
					return;
				if(viewInfo.ButtonBounds.Contains(pt)) {
					IDesignFrame designFrame = host.GetDesigner(viewInfo.Band) as IDesignFrame;
					designFrame.Expanded = !viewInfo.Expanded;
					UpdateView();
				} else {
					if(viewInfo.CaptionBounds.Contains(pt))
						((XRControlDesignerBase)host.GetDesigner(viewInfo.Band)).SelectComponent();
					else
						reportFrame.SelectComponent(Report);
				}
			} catch(Exception ex) {
				if(ExceptionHelper.IsCriticalException(ex))
					throw;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			try {
				base.OnMouseUp(e);
				if(host.IsDebugging()) return;
				BandViewInfo viewInfo = GetViewInfoByScreenPoint(MousePosition);
				if(e.Button.IsRight()) {
					XRComponentDesigner designer = null;
					if(viewInfo != null)
						designer = GetDesignerByPoint(MousePosition, viewInfo.Band);
					else
						designer = ReportDesigner;
					if(designer != null)
						designer.OnContextMenu(MousePosition.X, MousePosition.Y);
				} else if(viewInfo != null) {
					mouseEventHelper.HandleMouseEvent(viewInfo.Band, new XRMouseEventHandler(MouseEventHelper.HandleMouseUp), CreateMouseEventArgs(e.Button, viewInfo));
				}
			} catch(Exception ex) {
				if(ExceptionHelper.IsCriticalException(ex))
					throw;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			selectionUIService.InvalidateSmartTag();
			BandViewInfo viewInfo = GetNearestViewInfoByScreenPoint(MousePosition);
			if(!HandleMouseEvent(viewInfo, new XRMouseEventHandler(MouseEventHelper.HandleMouseMove), e.Button))
				Cursor = Cursors.Default;
		}
		bool HandleMouseEvent(BandViewInfo viewInfo, XRMouseEventHandler handler, MouseButtons button) {
			if(HandleSelectionMouseEvent(GetViewInfoSiblings(viewInfo), handler, button))
				return true;
			return viewInfo != null && mouseEventHelper.HandleMouseEvent(viewInfo.Band, handler, CreateMouseEventArgs(button, viewInfo));
		}
		bool HandleSelectionMouseEvent(List<BandViewInfo> viewInfos, XRMouseEventHandler handler, MouseButtons button) {
			foreach(BandViewInfo viewInfo in viewInfos) {
				if(mouseEventHelper.HandleSelectionMouseEvent(viewInfo.Band, handler, CreateMouseEventArgs(button, viewInfo)))
					return true;
			}
			return false;
		}
		List<BandViewInfo> GetViewInfoSiblings(BandViewInfo middleSibling) {
			List<BandViewInfo> result = new List<BandViewInfo>();
			if(middleSibling != null)
				result.Add(middleSibling);
			BandViewInfo prevSibling = GetPreviousViewInfo(middleSibling);
			if(prevSibling != null)
				result.Add(prevSibling);
			BandViewInfo nextSibling = GetNextViewInfo(middleSibling);
			if(nextSibling != null)
				result.Add(nextSibling);
			return result;
		}
		BandViewInfo GetNextViewInfo(BandViewInfo viewInfo) {
			if(viewInfo == null)
				return null;
			int index = Array.IndexOf(ViewInfos, viewInfo) + 1;
			return index < ViewInfos.Length ? ViewInfos[index] : null;
			}
		BandViewInfo GetPreviousViewInfo(BandViewInfo viewInfo) {
			if(viewInfo == null)
				return null;
			int index = Array.IndexOf(ViewInfos, viewInfo) - 1;
			return index >= 0 ? ViewInfos[index] : null;
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			mouseEventHelper.HandleMouseLeaveEvent();
		}
		protected override void OnDoubleClick(EventArgs e) {
			base.OnDoubleClick(e);
			BandViewInfo viewInfo = GetViewInfoByScreenPoint(MousePosition);
			if(viewInfo != null)
				mouseEventHelper.HandleMouseEvent(viewInfo.Band, new XRMouseEventHandler(MouseEventHelper.HandleDoubleClick), CreateMouseEventArgs(MouseButtons.None, viewInfo));
		}
		private BandMouseEventArgs CreateMouseEventArgs(MouseButtons mouseButton, BandViewInfo viewInfo) {
			Point pt = PointToClient(MousePosition);
			return new BandMouseEventArgs(mouseButton, pt, viewInfo);
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_PAINT) {
				WmPaint(ref m);
				return;
			}
			base.WndProc(ref m);
		}
		private void WmPaint(ref Message m) {
			const int ERROR_NOT_ENOUGH_MEMORY = 8;
			Debug.Assert(m.Msg == MSG.WM_PAINT);
			try {
				base.WndProc(ref m);
			} catch(Win32Exception exception) {
				DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, exception);
				if(exception.NativeErrorCode != ERROR_NOT_ENOUGH_MEMORY)
					throw;
				if(NotEnoughMemoryToPaint != null)
					NotEnoughMemoryToPaint(this, new ExceptionEventArgs(exception));
			}
		}
		#region IMouseWheelSupportIgnore Members
		bool IMouseWheelSupportIgnore.Ignore {
			get { return true; }
		}
		#endregion
	}
}

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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Preview.Native;
using System.Collections.Generic;
using DevExpress.XtraReports.Design.SnapLines;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Design 
{
	public delegate void BoundsDelegate(RectangleF bounds);
	public class CapturePaintService {
		static readonly Type[] types = {typeof(CapturePaintService), typeof(SplitService), typeof(ResizeService)};
		public static RectangleF GetDragBounds(IServiceProvider provider) {
			CapturePaintService srv = (CapturePaintService)provider.GetService(typeof(CapturePaintService));
			System.Diagnostics.Debug.Assert(srv != null);
			return srv.DragBounds;
		}
		public static CapturePaintService GetRunningService(IServiceProvider serviceProvider) {
			foreach(Type type in types) {
				CapturePaintService svc = serviceProvider.GetService(type) as CapturePaintService;
				if(svc != null && svc.IsRunning) {
					return svc;
				}
			}
			return null;
		}
		static protected void ClearInvocationList(Delegate handler) {
			if(handler != null) {
				Delegate[] delegates = handler.GetInvocationList();
				foreach(Delegate item in delegates) {
					handler = System.Delegate.Remove(handler, item);
				}
			}
		}
		protected static Rectangle GetScreenBounds(Control control) {
			return ReportDesigner.GetScreenBounds(control);
		}
		#region inner classes
		protected class MyWindowTarget : WindowTargetBase 
		{
			private CapturePaintService captureSvc;
			public MyWindowTarget(Control control, CapturePaintService captureSvc) : base(control) {
				this.captureSvc = captureSvc;
			}
			protected override void HandleMessage(ref Message m, ref bool handled) {
				if(m.Msg == Win32.WM_DESTROY || (m.Msg == Win32.WM_CAPTURECHANGED && captureSvc.IsRunning)) {
					captureSvc.EndServiceInternal(true);
				}
				if(m.Msg == Win32.WM_MOUSEMOVE) {
					captureSvc.HandleMouseMove();
				} else if(m.Msg == Win32.WM_LBUTTONDOWN || m.Msg == Win32.WM_RBUTTONDOWN ||
					m.Msg == Win32.WM_MBUTTONDOWN || m.Msg == Win32.WM_XBUTTONDOWN) {
					captureSvc.HandleMouseDown();
				} else if(m.Msg == Win32.WM_LBUTTONUP || m.Msg == Win32.WM_RBUTTONUP ||
					m.Msg == Win32.WM_MBUTTONUP || m.Msg == Win32.WM_XBUTTONUP) {
					captureSvc.HandleMouseUp();
				}
				if(captureSvc.isRunning && m.Msg >= Win32.WM_MOUSEFIRST && m.Msg <= Win32.WM_MOUSELAST) {
					handled = true;
				}
			}
		}
		#endregion
		private MyWindowTarget winTarget;
		protected Control control;
		protected PointF fStartPos = PointF.Empty;
		private BoundsDelegate boundHandler;
		protected RectangleF[] rects = new RectangleF[] { };
		protected RectangleF fDragBounds;
		private bool isRunning;
		protected IDesignerHost designerHost;
		AdornerService adornerService;
		protected AdornerService AdornerService {
			get {
				if(adornerService == null)
					adornerService = (AdornerService)designerHost.GetService(typeof(AdornerService));
				return adornerService;
			}
		}
		public bool IsRunning { get { return isRunning; }
		}
		public PointF StartPos { get { return fStartPos; } set { fStartPos = value; }
		}
		public RectangleF DragBounds {
			get { return fDragBounds; }
			set {
				fDragBounds = value;
				fStartPos = value.Location;
			}
		}
		public CapturePaintService(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		public void Start(Control control, BoundsDelegate boundHandler) {
			Start(control, boundHandler, Control.MousePosition);
		}
		public void Start(Control control, BoundsDelegate boundHandler, Point startPos) {
			StartService(control, boundHandler, startPos);
			Paint();
		}
		private void StartService(Control control, BoundsDelegate boundHandler, Point startPos) {
			this.control = control;
			this.boundHandler = boundHandler;
			OnStartService(startPos);
		}
		void OnStartService(Point startPos) {
			EnableIdleTimer(false);
			System.Diagnostics.Debug.Assert(!isRunning);
			System.Diagnostics.Debug.Assert(winTarget == null);
			winTarget = new MyWindowTarget(control, this);
			isRunning = true;
			control.Capture = true;
			fStartPos = startPos;
			fDragBounds = Rectangle.Empty;
		}
		private void EnableIdleTimer(bool val) {
			if(designerHost != null) {
				IIdleService serv = designerHost.GetService(typeof(IIdleService)) as IIdleService;
				if(serv != null)
					serv.SetEnabled(val);
			}
		}
		public virtual void EndServiceInternal(bool cancel) {
			if( IsRunning ) EndService(cancel);
		}
		protected virtual void EndService(bool cancel) {
			System.Diagnostics.Debug.Assert(winTarget != null);
			winTarget.Dispose();
			winTarget = null;
			isRunning = false;
			control.Capture = false;
			AdornerService.ResetSnapping();
			fDragBounds = rects.Length > 0 ? rects[0] : new RectangleF(fStartPos, SizeF.Empty);
			if(boundHandler != null)
				boundHandler(fDragBounds);
			fStartPos = PointF.Empty;
			fDragBounds = RectangleF.Empty;
			EnableIdleTimer(true);
		}
		protected virtual void HandleMouseUp() {
			if(control.Capture) EndService(false);
		}
		protected virtual void HandleMouseDown() {
			if(IsRunning && !Control.MouseButtons.IsLeft())
				EndService(true);
		}
		protected virtual void HandleMouseMove() {
			if(control.Capture) Paint();
		}
		protected virtual void Paint() {
			rects = GetRects();
			AdornerService.DrawScreenRects(rects);
		}
		protected virtual RectangleF[] GetRects() {
			RectangleF rect = RectHelper.RectangleFFromPoints(fStartPos, Control.MousePosition);
			return (rect.Width == 0 && rect.Height == 0) ? new RectangleF[] { } : new RectangleF[] { rect };
		}
	}
	public class ResizeService : CapturePaintService 
	{ 	
		#region static
		public static RectangleF ResizeRectangle(RectangleF val, SelectionRules selectionRules, SizeF delta, Size minSize) {			
			RectangleF bounds = val;
			if( !delta.IsEmpty ) {				
				float dx = delta.Width;
				float dy = delta.Height;
				if( (selectionRules & SelectionRules.LeftSizeable) != 0 ) {
					float right = bounds.Right;
					bounds.X = NativeMethods.GetValidValue(0, bounds.Right - minSize.Width, bounds.Left + dx);
					bounds.Width = right - bounds.X;
				} 
				if( (selectionRules & SelectionRules.TopSizeable) != 0 ) {
					float bottom = bounds.Bottom;
					bounds.Y = NativeMethods.GetValidValue(0, bounds.Bottom - minSize.Height, bounds.Top + dy);
					bounds.Height = bottom - bounds.Y;  
				} 
				if( (selectionRules & SelectionRules.RightSizeable) != 0 ) {
					bounds.Width += dx; 
				}
				if( (selectionRules & SelectionRules.BottomSizeable) != 0 ) {
					bounds.Height += dy;  
				}
			}			
			bounds.Size = NativeMethods.GetMaxSize(bounds.Size, minSize);
			return bounds;
		}		
		#endregion
		private ISelectionService selectionServ;
		private DesignerTransaction trans;
		private SelectionRules selectionRules;
		private BoundsEventHandler onResizing;
		private EventHandler onResizeComplete;
		private XRControl[] selectControls = {};
		ReportDesigner repDesigner;
		bool IsShiftPressed { get { return (Control.ModifierKeys & Keys.Shift) == Keys.Shift; } }
		bool IsCtrlPressed { get { return (Control.ModifierKeys & Keys.Control) == Keys.Control; } }
		public ResizeService(IDesignerHost designerHost) : base(designerHost) {
			this.selectionServ = (ISelectionService)designerHost.GetService(typeof(ISelectionService));
			this.repDesigner = (ReportDesigner)designerHost.GetDesigner(designerHost.RootComponent);
		}
		public event BoundsEventHandler Resizing {
			add { onResizing = System.Delegate.Combine(onResizing, value) as BoundsEventHandler; }
			remove { onResizing = System.Delegate.Remove(onResizing, value) as BoundsEventHandler; }
		}
		public event EventHandler ResizeComplete {
			add { onResizeComplete = System.Delegate.Combine(onResizeComplete, value) as EventHandler; }
			remove { onResizeComplete = System.Delegate.Remove(onResizeComplete, value) as EventHandler; }
		}
		private void OnResizing(BoundsEventArgs e) {
			if(onResizing != null) onResizing(this, e);
		}
		private void OnResizeComplete(EventArgs e) {
			if(onResizeComplete != null) onResizeComplete(this, e);
		}
		protected override void HandleMouseMove() {
			base.HandleMouseMove();
			OnResizing( new BoundsEventArgs(GetRects()) );
		}
		protected override void EndService(bool cancel) {
			base.EndService(cancel);
			if(repDesigner.RootReport.SnappingMode == SnappingMode.SnapLines) 
				AdornerService.ResetSnapping();
			if(cancel) 
				trans.Cancel();
			else 
				try {
					CommitResizing();
					trans.Commit();
				}  
				catch {
					trans.Cancel();			
				}
			OnResizeComplete(EventArgs.Empty);
			selectControls = new XRControl[] {}; 
			ClearInvocationList(onResizing);
			onResizing = null;
			ClearInvocationList(onResizeComplete);
			onResizeComplete = null;
		}
		private void CommitResizing() {
			if(!((IsShiftPressed && TableHelper.IsRowCellsArray(selectControls)) ||
			   ((IsCtrlPressed || IsShiftPressed) && TableHelper.IsColumnCellsArray(selectControls)))){
				for(int i = 0; i < selectControls.Length; i++) {
					XRControl ctl = selectControls[i];
					if(RectHelper.RectangleFEquals(rects[i], GetScreenBounds(ctl), 0.001))
						continue;
					IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(ctl, designerHost);
					if(adapter != null)
						adapter.SetScreenBounds(rects[i]);
				}
			} else {
				IBoundsAdapter adapter = selectControls[0] is XRTableRow ?
					(IBoundsAdapter)new TableRowBoundsAdapter(selectControls[0] as XRTableRow, designerHost) :
					(IBoundsAdapter)new TableColumnBoundsAdapter(selectControls, designerHost, IsShiftPressed);
				if(RectHelper.RectangleFEquals(rects[0], GetScreenBounds(selectControls[0]), 0.001)) return;
				adapter.SetScreenBounds(rects[0]);
			}
		}
		public void StartResize(Control control, SelectionRules selectionRules) {
			this.selectionRules = selectionRules;
			selectControls = GetSelectedControls();
			string desc = (selectControls.Length == 1) ?
				String.Format(DesignSR.TransFmt_OneSize, ((IComponent)selectionServ.PrimarySelection).Site.Name) :
				String.Format(DesignSR.TransFmt_Size, selectControls.Length);
			trans = designerHost.CreateTransaction(desc);
			base.Start(control, null);
		}
		public XRControl[] GetSelectedControls() {
			List<XRControl> controls = new List<XRControl>();
			ICollection selectedComponents = selectionServ.GetSelectedComponents();
			foreach(IComponent component in selectedComponents) {
				XRControl control = FrameSelectionUIService.GetAsXRControl(designerHost, component);
				if(control != null)
					controls.Add(control);
			}
			return controls.ToArray();
		}
		protected override RectangleF[] GetRects() {
			SizeF delta = GetDelta();
			RectangleF[] rects;
			if(repDesigner.RootReport.SnappingMode == SnappingMode.SnapLines && SnapLineHelper.IsSnappingAllowed) {
				AdornerService.DrawControlsRects(GetDelta(), this.selectionRules);
				IBandViewInfoService svc = (IBandViewInfoService)designerHost.GetService(typeof(IBandViewInfoService));
				rects = Array.ConvertAll<RectangleF, RectangleF>(AdornerService.DrawRects, delegate(RectangleF value) { return svc.RectangleFToScreen(value); });
			} else {
				rects = new RectangleF[selectControls.Length];
				for(int i = 0; i < rects.Length; i++) {
					rects[i] = GetContolRectangle(selectControls[i], delta);
				}
			}
			return rects;
		}
		SizeF GetDelta() {
			PointF pt = Control.MousePosition;
			return new SizeF(pt.X - fStartPos.X, pt.Y - fStartPos.Y);
		}
		protected override void Paint() {
			if(repDesigner.RootReport.SnappingMode == SnappingMode.SnapLines && SnapLineHelper.IsSnappingAllowed) {
				AdornerService.DrawSnapLinesForPrimarySelection(GetDelta(), this.selectionRules);
			} else {
				AdornerService.ResetSnapping();
			}
			base.Paint();	  
		}
		RectangleF GetContolRectangle(XRControl control, SizeF delta) {
			XRControlDesignerBase controlDesigner = (XRControlDesignerBase)designerHost.GetDesigner(control);
			System.Diagnostics.Debug.Assert(controlDesigner != null);
			SelectionRules selectionRules = this.selectionRules & controlDesigner.GetSelectionRules();
			IBoundsAdapter adapter = BoundsAdapterService.GetAdapter(control, designerHost);
			RectangleF screenRect = adapter.GetScreenBounds();
			RectangleF resizeScreenRect = ResizeRectangle(screenRect, selectionRules, delta, XRConvert.Convert(control.GetMinSize(), control.Dpi, GraphicsDpi.Pixel));
			PointF leftTop = resizeScreenRect.Location;
			PointF rightBottom = new PointF(resizeScreenRect.Right, resizeScreenRect.Bottom);
			IBandViewInfoService svc = (IBandViewInfoService)designerHost.GetService(typeof(IBandViewInfoService));
			PointF snapLeftTop = SnapLineHelper.IsSnappingAllowed ? svc.PointToClientRelativeToBand(leftTop, null) : leftTop;
			PointF snapRightBottom = SnapLineHelper.IsSnappingAllowed ? svc.PointToClientRelativeToBand(rightBottom, null) : rightBottom;
			if((selectionRules & SelectionRules.LeftSizeable) != 0) {
				leftTop.X = snapLeftTop.X;
			}
			if((selectionRules & SelectionRules.TopSizeable) != 0) {
				leftTop.Y = snapLeftTop.Y;
			}
			if((selectionRules & SelectionRules.RightSizeable) != 0) {
				rightBottom.X = snapRightBottom.X;
			}
			if((selectionRules & SelectionRules.BottomSizeable) != 0) {
				rightBottom.Y = snapRightBottom.Y;
			}
			resizeScreenRect = RectangleF.FromLTRB(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);
			return resizeScreenRect;
		}
		protected RectangleF GetScreenBounds(XRControl control) {
			IBandViewInfoService svc = (IBandViewInfoService)designerHost.GetService(typeof(IBandViewInfoService));
			return svc.GetControlScreenBounds(control); 
		}
	}
	public class SplitEventArgs : EventArgs {
		float splitX;
		float splitY;
		int x;
		int y;
		public SplitEventArgs(int x, int y, float splitX, float splitY) {
			this.x = x;
			this.y = y;
			this.splitX = splitX;
			this.splitY = splitY;
		}
		public int RoundedSplitX {
			get {
				return (int)Math.Round(this.SplitX);
			}
		}
		public int RoundedSplitY {
			get {
				return (int)Math.Round(this.SplitY);
			}
		}
		public float SplitX {
			get {
				return this.splitX;
			}
		}
		public float SplitY {
			get {
				return this.splitY;
			}
		}
		public int X {
			get {
				return this.x;
			}
		}
		public int Y {
			get {
				return this.y;
			}
		}
	}
	public delegate void SplitEventHandler(object sender, SplitEventArgs e);
	public class SplitService : CapturePaintService 
	{
		private bool vertical = true;
		private SplitEventHandler onSplitterMoved;
		private SplitEventHandler onSplitterMove;
		private EventHandler onMoveCanceled;
		private PointF offset = Point.Empty;
		private FramePanel FramePanel {
			get { return (FramePanel)this.control; }
		}
		public bool Vertical {
			get { return vertical; }
		}
		#region Events
		public event EventHandler MoveCanceled {
			add { onMoveCanceled = System.Delegate.Combine(onMoveCanceled, value) as EventHandler; }
			remove { onMoveCanceled = System.Delegate.Remove(onMoveCanceled, value) as EventHandler; }
		}
		public event SplitEventHandler SplitterMoved {
			add { onSplitterMoved = System.Delegate.Combine(onSplitterMoved, value) as SplitEventHandler; }
			remove { onSplitterMoved = System.Delegate.Remove(onSplitterMoved, value) as SplitEventHandler; }
		}
		public event SplitEventHandler SplitterMove {
			add { onSplitterMove = System.Delegate.Combine(onSplitterMove, value) as SplitEventHandler; }
			remove { onSplitterMove = System.Delegate.Remove(onSplitterMove, value) as SplitEventHandler; }
		}
		private void OnMoveCanceled(EventArgs e) {
			if(onMoveCanceled != null) onMoveCanceled(this, e);
		}
		private void OnSplitterMoved(SplitEventArgs e) {
			if(onSplitterMoved != null) onSplitterMoved(this, e);
		}
		private void OnSplitterMove(SplitEventArgs e) {
			if(onSplitterMove != null) onSplitterMove(this, e);
		}
		#endregion
		public SplitService(IDesignerHost host) : base(host) {
		}
		internal void StartVMoving(FramePanel control) {
			vertical = true;
			this.offset = Point.Empty;
			base.Start(control, null);
		}
		internal void StartHMoving(FramePanel control, PointF offset) {
			vertical = false;
			this.offset = offset;
			base.Start(control, null);
		}
		protected override void HandleMouseMove() {
			base.HandleMouseMove();
			OnSplitterMove( CreateEventArgs() );
		}
		protected override void EndService(bool cancel) {
			base.EndService(cancel);
			if(cancel)	{
				OnMoveCanceled(EventArgs.Empty);
			} else 
				OnSplitterMoved( CreateEventArgs() );
			Delegate[] delegates = new Delegate[] {onSplitterMove, onSplitterMoved, onMoveCanceled};
			foreach(Delegate item in delegates)
				ClearInvocationList(item);
			onSplitterMove = onSplitterMoved = null;
			onMoveCanceled = null;
		}
		private SplitEventArgs CreateEventArgs() {
			RectangleF bounds = rects[0];
			Point pos = Control.MousePosition;
			return new SplitEventArgs(pos.X, pos.Y, bounds.X, bounds.Y);
		}
		protected override RectangleF[] GetRects() {
			RectangleF r = FramePanel.RectangleToScreen(FramePanel.BandBounds);
			r.Size = vertical ? new SizeF(2, r.Height) : new SizeF(r.Width, 2);
			PointF pos = Control.MousePosition;
			pos.X += this.offset.X;
			pos.Y += this.offset.Y;	
			r.Location = vertical ? new PointF(pos.X, r.Y) : new PointF(r.X, pos.Y);
			return new RectangleF[] { r }; 
		}		
	}
}

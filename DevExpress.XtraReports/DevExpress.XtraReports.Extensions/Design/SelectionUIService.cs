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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using System.Collections.Generic;
using DevExpress.XtraReports.Design.MouseTargets;
namespace DevExpress.XtraReports.Design {
	public class FrameSelectionUIService : IDisposable {
		#region inner classes
		protected class GrabSelectionItem : SelectionItemBase {
			RectangleF rect = Rectangle.Empty;
			public GrabSelectionItem(XRControl ctl, XRControlDesigner designer)
				: base(ctl, designer) {
				rect = svcBandViewInfo.GetControlViewClientBounds(ctl);
				rect.Width = rect.Height = grabWidth;
				rect.Offset(-rect.Width / 2, -rect.Height / 2);
			}
			public override void DoPaint(Graphics graph, Band band) {
				if(Comparer.Equals(band, this.Control.Band)) {
					RectangleF grabRect = XRConvert.Convert(rect, GraphicsDpi.Pixel, GraphicsDpi.Document);
					ControlPaintHelper.DrawContainerGrabHandle(graph, grabRect, XRConvert.Convert(1f, GraphicsDpi.Pixel, GraphicsDpi.GetGraphicsDpi(graph)));
				}
			}
			public override void HandleMouseMove(object sender, BandMouseEventArgs args) {
				((Control)sender).Cursor = Cursors.SizeAll;
			}
			public override void HandleMouseDown(object sender, BandMouseEventArgs args) {
				base.HandleMouseDown(sender, args);
				ISelectionService selSrv = (ISelectionService)GetService(typeof(ISelectionService));
				selSrv.SetSelectedComponents(new object[] { Control }, ControlConstants.SelectionTypeAuto);
			}
			public override bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
				return rect.Contains(pt);
			}
			public bool ContainsScreenPoint(Point pt) {
				return svcBandViewInfo.RectangleFToScreen(rect).Contains(pt);
			}
		}
		#endregion
		#region static
		public static XRControl GetAsXRControl(IDesignerHost host, object obj) {
			if(obj is XRControl)
				return (XRControl)obj;
			if(obj is FakeComponent)
				return ((FakeComponent)obj).Parent as XRControl;
			return GetComponentContainer(host, obj as IComponent);
		}
		public static XRControl GetComponentContainer(IDesignerHost host, IComponent component) {
			if(component != null) {
				BandCollection bands = ((XtraReport)host.RootComponent).Bands;
				NestedComponentEnumerator enumerator = new NestedComponentEnumerator(bands);
				while(enumerator.MoveNext()) {
					if(enumerator.Current.VisibleComponents.Contains(component))
						return enumerator.Current;
				}
			}
			return null;
		}
		protected const int grabWidth = 15;
		#endregion
		List<SelectionItemBase> selectionItems = new List<SelectionItemBase>();
		ISelectionService selSvc;
		IDesignerHost host;
		XRControl grabControl;
		GrabSelectionItem grabSelectionItem;
		SmartTagSelectionItem smartTagSelectionItem;
		Control control;
		ZoomService zoomService;
		private bool HostInProcess {
			get { return host.Loading || DesignMethods.IsDesignerInTransaction(host); }
		}
		bool IsReportLoading {
			get { return ((XtraReport)host.RootComponent).Loading; }
		}
		public ICollection<SelectionItemBase> SelectionItems {
			get { return selectionItems; }
		}
		public RectangleF SmartTagScreenRectangle {
			get { return smartTagSelectionItem != null ? smartTagSelectionItem.GetScreenRectangle() : RectangleF.Empty; }
		}
		public FrameSelectionUIService(IDesignerHost host, Control control) {
			this.control = control;
			Initialize(host);
		}
		void Initialize(IDesignerHost host) {
			System.Diagnostics.Debug.Assert(host != null);
			this.host = host;
			this.selSvc = (ISelectionService)host.GetService(typeof(ISelectionService));
			if(selSvc != null) {
				selSvc.SelectionChanged += new EventHandler(this.OnSelectionChanged);
			}
			IComponentChangeService changeSvc = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
			if(changeSvc != null) {
				changeSvc.ComponentRemoved += new ComponentEventHandler(this.OnComponentRemove);
				changeSvc.ComponentChanged += new ComponentChangedEventHandler(this.OnComponentChanged);
			}
			host.TransactionClosed += new DesignerTransactionCloseEventHandler(this.OnTransactionClosed);
			zoomService = ZoomService.GetInstance(host);
			zoomService.ZoomChanged += new EventHandler(OnZoomChanged);
		}
		public void Dispose() {
			host.TransactionClosed -= new DesignerTransactionCloseEventHandler(this.OnTransactionClosed);
			IComponentChangeService changeSvc = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
			if(changeSvc != null) {
				changeSvc.ComponentRemoved -= new ComponentEventHandler(this.OnComponentRemove);
				changeSvc.ComponentChanged -= new ComponentChangedEventHandler(this.OnComponentChanged);
			}
			if(selSvc != null) {
				selSvc.SelectionChanged -= new EventHandler(this.OnSelectionChanged);
			}
			DisposeSelectionItems();
			zoomService.ZoomChanged -= new EventHandler(OnZoomChanged);
		}
		public void InvalidateSmartTag() {
			if(smartTagSelectionItem != null)
				smartTagSelectionItem.Invalidate();
		}
		public void ShowGrabHandle(XRControl control) {
			if(Comparer.Equals(grabControl, control))
				return;
			if(grabControl != null)
				InvalidateRect(GetInvalidateBounds(grabControl));
			grabControl = control;
			SyncSelection(true);
			System.Diagnostics.Debug.Assert(grabSelectionItem != null);
			InvalidateRect(GetInvalidateBounds(grabControl));
		}
		Rectangle GetInvalidateBounds(XRControl ctl) {
			IBandViewInfoService svc = (IBandViewInfoService)host.GetService(typeof(IBandViewInfoService));
			RectangleF rect = RectangleF.Inflate(svc.GetControlScreenBounds(ctl), grabWidth, grabWidth);
			return RectHelper.InflateRectFToInteger(rect);
		}
		public void HideGrabHandle(XRControl ctl) {
			if(grabControl != null && grabControl == ctl) {
				Rectangle rect = GetInvalidateBounds(grabControl);
				grabControl = null;
				System.Diagnostics.Debug.Assert(grabSelectionItem != null);
				SyncSelection(true);
				System.Diagnostics.Debug.Assert(grabSelectionItem == null);
				InvalidateRect(rect);
			}
		}
		public IList GetBandSelectionItems(Band band) {
			List<SelectionItemBase> list = new List<SelectionItemBase>();
			foreach(SelectionItemBase item in selectionItems)
				if(item.Control.IsInsideBand(band))
					list.Add(item);
			return list;
		}
		public bool ContainsControl(XRControl control) {
			return GetSelectionItemByControl(control) != null;
		}
		public SelectionItemBase GetSelectionItemByControl(XRControl control) {
			foreach(SelectionItemBase item in this.selectionItems)
				if(item.Control == control)
					return item;
			return null;
		}
		private void InvalidateRect(Rectangle rect) {
			control.Invalidate(control.RectangleToClient(rect));
		}
		public bool GrabContainsPoint(Point pt) {
			if(grabSelectionItem != null)
				return grabSelectionItem.ContainsScreenPoint(pt);
			return false;
		}
		internal void DrawSelectionItems(Graphics gr, Band band, bool shouldDrawBand) {
			GraphicsUnit pageUnit = gr.PageUnit;
			try {
				gr.PageUnit = GraphicsUnit.Document;
				gr.ResetTransform();
				for(int i = selectionItems.Count - 1; i >= 0; i--) {
					SelectionItemBase selectionItem = (SelectionItemBase)selectionItems[i];
					if(selectionItem.Control.IsDisposed) continue;
					if(!shouldDrawBand) {
						if(selectionItem is BandSmartTagSelectionItem)
							selectionItem.DoPaint(gr, band);
					} else {
						selectionItem.DoPaint(gr, band);
					}
				}
			} finally {
				gr.PageUnit = pageUnit;
				gr.ResetTransform();
			}
		}
		private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e) {
			SyncSelection(true);
		}
		private void OnComponentRemove(object sender, ComponentEventArgs ce) {
			SyncSelection(false);
		}
		private void OnComponentChanged(object sender, ComponentChangedEventArgs ccevent) {
			SyncSelection(false);
		}
		private void OnSelectionChanged(object sender, EventArgs e) {
			if(!HostInProcess) {
				IBandViewInfoService bandViewSvc = (IBandViewInfoService)host.GetService(typeof(IBandViewInfoService));
				if(bandViewSvc != null)
					bandViewSvc.UpdateView();
				SyncSelection(true);
			}
		}
		private void OnZoomChanged(object sender, EventArgs e) {
			if(!HostInProcess) {
				SyncSelection(true);
			}
		}
		private void SyncSelection(bool force) {
			if((!force && HostInProcess) || IsReportLoading)
				return;
			DisposeSelectionItems();
			grabSelectionItem = null;
			IList selection = GetSelectedComponents();
			foreach(object comp in selection) {
				XRControl control = GetAsXRControl(host, comp);
				if(control == null || control is Band)
					continue;
				IMouseTargetService serv = host.GetService(typeof(IMouseTargetService)) as IMouseTargetService;
				SelectionItemBase item = serv.CreateSelectionItem(control);
				if(item != null)
					selectionItems.Add(item);
				if(Comparer.Equals(control, grabControl))
					grabControl = null;
			}
			if(grabControl != null) {
				XRControlDesigner designer = GetDesigner(grabControl);
				if(designer != null) {
					grabSelectionItem = new GrabSelectionItem(grabControl, designer);
					selectionItems.Insert(0, grabSelectionItem);
				} else
					grabControl = null;
			}
			if(selection.Count == 1) {
				XRControl control = GetAsXRControl(host, selection[0]);
				if(control != null) {
					XRControlDesignerBase designer = host.GetDesigner(control) as XRControlDesignerBase;
					if(XRSmartTagService.HasSmartTagPresentation(designer)) {
						smartTagSelectionItem = SmartTagSelectionItem.CreateInstance(control, designer, host);
						if(smartTagSelectionItem != null)
							selectionItems.Insert(0, smartTagSelectionItem);
					}
					XRSmartTagService svc = host.GetService(typeof(XRSmartTagService)) as XRSmartTagService;
					if(svc != null && smartTagSelectionItem != null)
						svc.UpdateSmartTagItem(smartTagSelectionItem);
				}
			}
		}
		IList GetSelectedComponents() {
			return new ArrayList(selSvc.GetSelectedComponents());
		}
		private XRControlDesigner GetDesigner(XRControl ctl) {
			System.Diagnostics.Debug.Assert(host != null);
			return host.GetDesigner(ctl) as XRControlDesigner;
		}
		private void DisposeSelectionItems() {
			selectionItems.Clear();
			smartTagSelectionItem = null;
		}
	}
	public class SelectionItemBase : IMouseTarget {
		protected static Color fSelectionColor = Color.FromArgb(0x7f, 0x71, 0x6f, 0x64);
		protected XRControl fControl;
		protected XRComponentDesigner fDesigner;
		protected IBandViewInfoService svcBandViewInfo;
		ZoomService zoomService;
		public XRControl Control {
			get { return fControl; }
		}
		protected ZoomService ZoomService {
			get {
				if(zoomService == null)
					zoomService = (ZoomService)GetService(typeof(ZoomService));
				return zoomService;
			}
		}
		public SelectionItemBase(XRControl control, XRComponentDesigner designer) {
			System.Diagnostics.Debug.Assert(control != null && designer != null);
			this.fControl = control;
			this.fDesigner = designer;
			svcBandViewInfo = (IBandViewInfoService)GetService(typeof(IBandViewInfoService));
		}
		public void CommitSelection(RectangleF bounds, IComponent defaultSelection) { 
		}
		public virtual void DoPaint(Graphics graph, Band band) {
		}
		public virtual void HandleMouseDown(object sender, BandMouseEventArgs args) {
			XRTextControlBaseDesigner designer = fDesigner as XRTextControlBaseDesigner;
			if(designer != null)
				designer.CommitInplaceEditor();
		}
		public virtual void HandleMouseUp(object sender, BandMouseEventArgs args) {
		}
		public virtual void HandleMouseMove(object sender, BandMouseEventArgs args) {
		}
		public virtual void HandleDoubleClick(object sender, BandMouseEventArgs e) {
		}
		public virtual void HandleMouseLeave(object sender, EventArgs e) {
		}
		public virtual bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
			return false;
		}
		public virtual bool IsDisposed {
			get { return false; }
		}
		protected object GetService(Type type) {
			return fDesigner.Component.Site.GetService(type);
		}
	}
	public abstract class SelectionItem : SelectionItemBase {
		#region inner classes
		protected class SelectionRulesEnumerator : IEnumerator {
			private const SelectionRules TopSizeable = SelectionRules.TopSizeable;
			private const SelectionRules TopLeftSizeable = SelectionRules.TopSizeable | SelectionRules.LeftSizeable;
			private const SelectionRules TopRightSizeable = SelectionRules.TopSizeable | SelectionRules.RightSizeable;
			private const SelectionRules LeftSizeable = SelectionRules.LeftSizeable;
			private const SelectionRules RightSizeable = SelectionRules.RightSizeable;
			private const SelectionRules BottomSizeable = SelectionRules.BottomSizeable;
			private const SelectionRules BottomLeftSizeable = SelectionRules.BottomSizeable | SelectionRules.LeftSizeable;
			private const SelectionRules BottomRightSizeable = SelectionRules.BottomSizeable | SelectionRules.RightSizeable;
			private static SelectionRules[] selections = new SelectionRules[8] { TopLeftSizeable, TopSizeable, TopRightSizeable, LeftSizeable, RightSizeable, BottomLeftSizeable, BottomSizeable, BottomRightSizeable };
			private static ContentAlignment[] aligments = new ContentAlignment[8] { ContentAlignment.TopLeft, ContentAlignment.TopCenter, ContentAlignment.TopRight, ContentAlignment.MiddleLeft, ContentAlignment.MiddleRight, ContentAlignment.BottomLeft, ContentAlignment.BottomCenter, ContentAlignment.BottomRight };
			private static Cursor[] cursors = new Cursor[8] { Cursors.SizeNWSE, Cursors.SizeNS, Cursors.SizeNESW, Cursors.SizeWE, Cursors.SizeWE, Cursors.SizeNESW, Cursors.SizeNS, Cursors.SizeNWSE };
			private int index = -1;
			object IEnumerator.Current {
				get { return selections[index]; }
			}
			public SelectionRules Current {
				get { return selections[index]; }
			}
			public ContentAlignment CurrentAlignment {
				get { return aligments[index]; }
			}
			public Cursor CurrentCursor {
				get { return cursors[index]; }
			}
			public SelectionRulesEnumerator() {
			}
			public virtual bool MoveNext() {
				index++;
				return index < selections.Length;
			}
			public virtual void Reset() {
				index = -1;
			}
		}
		#endregion
		const float GrabSizePx = 5f;
		const float GrabOffsetPx = 3f;
		static void TransformGraphics(Graphics gr, Band band) {
			IBandViewInfoService svcBandViewInfo = (IBandViewInfoService)band.Site.GetService(typeof(IBandViewInfoService));
			BandViewInfo bandViewInfo = svcBandViewInfo.GetViewInfoByBand(band);
			using(Matrix matrix = new Matrix()) {
				PointF pos = XRConvert.Convert((PointF)bandViewInfo.ClientBandBounds.Location, GraphicsDpi.Pixel, GraphicsDpi.UnitToDpi(gr.PageUnit));
				matrix.Translate(pos.X, pos.Y);
				gr.MultiplyTransform(matrix);
			}
			ZoomService.GetInstance(band.Site).ScaleGraphics(gr);
		}
		protected static readonly Brush fSelectionBrush = new SolidBrush(fSelectionColor);
		protected bool fPrimary;
		protected RectangleF fRect = Rectangle.Empty;
		protected RulerService RulerService {
			get { return (RulerService)GetService(typeof(RulerService)); }
		}
		protected SelectionItem(XRControl ctl, XRControlDesigner designer, bool primary)
			: base(ctl, designer) {
			this.fPrimary = primary;
		}
		public override void DoPaint(Graphics graph, Band band) {
			if(this.Control.IsDisposed || !this.Control.IsInsideBand(band))
				return;
			UpdateInsideRect();
			try {
				TransformGraphics(graph, band);
				RectangleF bounds = fDesigner.GetBounds(band, graph.PageUnit);
				if(!bounds.IsEmpty) 
					DrawGrabHandles(graph, bounds, band);
			} finally {
				graph.ResetTransform();
			}
		}
		protected virtual void UpdateInsideRect() {
			fRect = svcBandViewInfo.GetControlViewClientBounds(this.Control);
		}
		void DrawGrabHandles(Graphics graph, RectangleF rect, Band band) {
			float grabSize = ZoomService.FromScaledPixels(GrabSizePx, graph);
			float grabOffset = ZoomService.FromScaledPixels(GrabOffsetPx, graph);
			float offset = grabOffset + grabSize;
			rect.Inflate(offset, offset);
			SelectionRulesEnumerator en = new SelectionRulesEnumerator();
			RectangleF grabRect = new RectangleF(rect.X, rect.Y, grabSize, grabSize);
			while(en.MoveNext()) {
				if(!IsEnabledRules(en.Current, band))
					continue;
				bool printingWarning = Control.RootReport.DesignerOptions.ShowPrintingWarnings && Control.HasPrintingWarning();
				bool exportWarning = Control.RootReport.DesignerOptions.ShowExportWarnings && Control.HasExportWarning();
				ControlPaintHelper.DrawGrabHandle(
					graph,
					RectHelper.AlignRectangleF(grabRect, rect, en.CurrentAlignment),
					ZoomService.FromScaledPixels(1f, graph), 
					fPrimary,
					printingWarning || exportWarning);
			}
		}
		private bool IsEnabledRules(SelectionRules selectionRules, Band band) {
			float height = ZoomService.ToScaledPixels(Control.GetBounds(band).Height, Control.Dpi);
			if(height < GrabSizePx && (selectionRules == SelectionRules.LeftSizeable || selectionRules == SelectionRules.RightSizeable))
				return false;
			return (((XRControlDesigner)fDesigner).GetSelectionRules(band) & selectionRules) == selectionRules;
		}
		private RectangleF GetGrabRectangle(ContentAlignment aligment) {
			RectangleF relativeRect = fRect;
			float inflate = GrabSizePx + GrabOffsetPx;
			relativeRect.Inflate(inflate, inflate);
			const float MouseGrabInflate = 2f;
			float grabSize = GrabSizePx + MouseGrabInflate;
			RectangleF rect = new RectangleF(relativeRect.Location, new SizeF(grabSize, grabSize));
			rect = RectHelper.AlignRectangleF(rect, relativeRect, aligment);
			return rect;
		}
		public override void HandleMouseMove(object sender, BandMouseEventArgs args) {
			Point mousePoint = new Point(args.X, args.Y);
			SelectionRulesEnumerator en = GetRulesInfo(mousePoint, args.Band);
			Cursor cursor = (en != null) ? en.CurrentCursor :
				IsEnabledRules(SelectionRules.Moveable, args.Band) ? Cursors.SizeAll :
				Cursors.Default;
			((Control)sender).Cursor = cursor;
		}
		public override void HandleMouseDown(object sender, BandMouseEventArgs args) {
			base.HandleMouseDown(sender, args);
			SelectionRulesEnumerator en = GetRulesInfo(new Point(args.X, args.Y), args.Band);
			if(en != null && IsEnabledRules(en.Current, args.Band) && args.Button.IsLeft()) {
				ResizeService resizeSvc = (ResizeService)GetService(typeof(ResizeService));
				resizeSvc.StartResize((Control)sender, en.Current);
				resizeSvc.Resizing += OnResizing;
				resizeSvc.ResizeComplete += OnResizeComplete;
				return;
			}
			bool needsSelectionChangeOnMouseDown = !((System.Windows.Forms.Control.ModifierKeys & Keys.Control) == Keys.Control);
			if(needsSelectionChangeOnMouseDown) {
				((XRControlDesigner)fDesigner).SelectComponent();
			}
		}
		private void OnResizing(object sender, BoundsEventArgs e) {
			RulerService.DrawShadows(e.Bounds, Control.Band.NestedLevel);
		}
		private void OnResizeComplete(object sender, EventArgs e) {
			RulerService.HideShadows();
		}
		public override bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
			RectangleF rect = fRect;
			float offset = GrabSizePx + GrabOffsetPx;
			bool isRightSizeableEnabledRule = IsEnabledRules(SelectionRules.RightSizeable, viewInfo.Band);
			if(isRightSizeableEnabledRule) {
				rect.Width += offset;
			}
			bool isLeftSizeableEnabledRule = IsEnabledRules(SelectionRules.LeftSizeable, viewInfo.Band);
			if(isLeftSizeableEnabledRule) {
				rect.X -= offset;
				rect.Width += offset;
			}
			if(IsEnabledRules(SelectionRules.TopSizeable, viewInfo.Band)) {
				rect.Y -= offset;
				rect.Height += offset;
				if(!isRightSizeableEnabledRule) {
					rect.Width += offset;
				}
				if(!isLeftSizeableEnabledRule) {
					rect.X -= offset;
					rect.Width += offset;
				}
			}
			if(IsEnabledRules(SelectionRules.BottomSizeable, viewInfo.Band)) {
				rect.Height += offset;
				if(!isRightSizeableEnabledRule) {
					rect.Width += offset;
				}
				if(!isLeftSizeableEnabledRule) {
					rect.X -= offset;
					rect.Width += offset;
				}
			}
			return rect.Contains(pt) && !fRect.Contains(pt);
		}
		private SelectionRulesEnumerator GetRulesInfo(Point pt, Band band) {
			SelectionRulesEnumerator en = new SelectionRulesEnumerator();
			while(en.MoveNext()) {
				if(IsEnabledRules(en.Current, band)) {
					RectangleF rect = GetGrabRectangle(en.CurrentAlignment);
					if(rect.Contains(pt))
						return en;
				}
			}
			return null;
		}
	}
	public class ControlSelectionItem : SelectionItem {
		public ControlSelectionItem(XRControl control, XRControlDesigner designer, bool primary)
			: base(control, designer, primary) {
			fRect = svcBandViewInfo.GetControlViewClientBounds(control);
		}
	}
	class CrossBandSelectionItem : SelectionItem {
		public CrossBandSelectionItem(XRControl ctl, XRControlDesigner designer, bool primary)
			: base(ctl, designer, primary) {
		}
		public override void DoPaint(Graphics graph, Band band) {
			UpdateFields(band);
			base.DoPaint(graph, band);
		}
		public override void HandleMouseMove(object sender, BandMouseEventArgs args) {
			UpdateFields(args.Band);
			base.HandleMouseMove(sender, args);
		}
		public override void HandleMouseDown(object sender, BandMouseEventArgs args) {
			UpdateFields(args.Band);
			base.HandleMouseDown(sender, args);
		}
		public override void HandleMouseUp(object sender, BandMouseEventArgs args) {
			UpdateFields(args.Band);
			base.HandleMouseUp(sender, args);
		}
		public override void HandleDoubleClick(object sender, BandMouseEventArgs e) {
			UpdateFields(e.Band);
			base.HandleDoubleClick(sender, e);
		}
		public override bool ContainsPoint(Point pt, BandViewInfo viewInfo) {
			UpdateFields(viewInfo.Band);
			return base.ContainsPoint(pt, viewInfo);
		}
		protected override void UpdateInsideRect() {
		}
		void UpdateFields(Band band) {
			fRect = svcBandViewInfo.GetControlViewClientBounds(base.Control, band);
		}
	}
}

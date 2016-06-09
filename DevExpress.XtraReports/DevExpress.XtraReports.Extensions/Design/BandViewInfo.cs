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
using System.Drawing.Drawing2D;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design.Ruler;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.XtraReports.Design.Drawing;
namespace DevExpress.XtraReports.Design {
	public class BandViewInfo : IBandViewInfo {
		float bandHeight;
		float watermarkPartOffset;
		float watermarkPartHeight;
		protected RectangleF fBounds;
		protected RectangleF fBandBounds;
		protected IDesignerHost host;
		protected ReportDesigner fReportDesigner;
		BandViewPainter painter;
		BandCaptionPainter captionPainter;
		BandCaptionViewInfo bandCaptionViewInfo;
		Image gridPattern;
		IList bandControls;
		Collection<XRControl> allBandControls;
		Dictionary<XRControl, XRControlWarning> controlsInfo;
		Dictionary<XRControl, XRControlWarning> xbandControlsInfo;
		bool drawBottomMargin;
		ZoomService zoomService;
		IBandViewInfoService bandViewInfoService;
		public float ZoomFactor {
			get { return ZoomService.ZoomFactor; }
		}
		public bool DrawBottomMargin {
			get { return drawBottomMargin; }
			set { drawBottomMargin = value; }
		}
		public float BandHeight {
			get { return bandHeight; }
		}
		public RectangleF ReportClientRectangle {
			get {
				return fReportDesigner.RootReport.ClientRectangleF;
			}
		}
		public Dictionary<XRControl, XRControlWarning> XBandControlsInfo {
			get {
				if(xbandControlsInfo == null) {
					xbandControlsInfo = new Dictionary<XRControl, XRControlWarning>();
					foreach(XRControl control in AllBandControls) {
						if(!control.IsRealControl && !xbandControlsInfo.ContainsKey(control.RealControl)) 
							xbandControlsInfo.Add(control.RealControl, GetWarning(Band.RootReport, control));
					}
				}
				return xbandControlsInfo;
			}
		}
		public Dictionary<XRControl, XRControlWarning> ControlsInfo {
			get {
				if(controlsInfo == null) {
					controlsInfo = new Dictionary<XRControl, XRControlWarning>();
					foreach(XRControl control in AllBandControls) {
						controlsInfo.Add(control, GetWarning(Band.RootReport, control));
				}
				}
				return controlsInfo;
			}
		}
		XRControlWarning GetWarning(XtraReport report, XRControl control) {
			XRControlWarning warning = XRControlWarning.None;
			IWindowsService windowsService = host.GetService(typeof(IWindowsService)) as IWindowsService;
			if(report.DesignerOptions.ShowExportWarnings && control.HasExportWarning())
				warning |= XRControlWarning.Export;
			if(report.DesignerOptions.ShowPrintingWarnings && control.HasPrintingWarning())
				warning |= XRControlWarning.Printing;
			if(report.DesignerOptions.ShowPrintingWarnings && windowsService != null && windowsService.ShouldSaveSubreport((control as XRSubreport)))
				warning |= XRControlWarning.Printing;
			return warning;
		}
		public Collection<XRControl> AllBandControls {
			get {
				if(allBandControls == null) {
					allBandControls = new Collection<XRControl>();
					NestedComponentEnumerator en = new NestedComponentEnumerator(BandControls);
					while(en.MoveNext())
						allBandControls.Add(en.Current);
				}
				return allBandControls;
			}
		}
		public IList BandControls {
			get {
				if(bandControls == null)
					bandControls = Band.GetPrintableControls();
				return bandControls;
			}
		}
		public Image GridPattern {
			get { return gridPattern; }
			set { gridPattern = value; }
		}
		public float WatermarkPartOffset {
			get { return watermarkPartOffset; }
			set { watermarkPartOffset = value; }
		}
		public float WatermarkPartHeight {
			get { return watermarkPartHeight; }
			set { watermarkPartHeight = value; }
		}
		public Rectangle Bounds {
			get { return Rectangle.Round(fBounds); }
		}
		public RectangleF BoundsF {
			get { return fBounds; }
		}
		public Rectangle BandBounds {
			get { return RectHelper.CeilingVertical(fBandBounds); }
		}
		public RectangleF BandBoundsF {
			get { return fBandBounds; }
		}
		public Rectangle ClientBandBounds {
			get { return RectHelper.DeflateRect(BandBounds, LeftMargin, 0, RightMargin, 0); }
		}
		public Rectangle CaptionBounds {
			get { return bandCaptionViewInfo.Bounds; }
		}
		public RectangleF ButtonBounds {
			get { return bandCaptionViewInfo.ButtonBounds; }
		}
		public RectangleF TextBounds {
			get { return bandCaptionViewInfo.TextBounds; }
		}
		public int LeftMargin {
			get { return fReportDesigner.LeftMargin; }
		}
		public int RightMargin {
			get { return fReportDesigner.RightMargin; }
		}
		public int Level {
			get { return bandCaptionViewInfo.Level; }
		}
		public bool Expanded {
			get { return bandCaptionViewInfo.Expanded; }
		}
		public Band Band {
			get { return bandCaptionViewInfo.Band; }
		}
		public string Text {
			get { return bandCaptionViewInfo.Text; }
		}
		public bool Selected {
			get { return bandCaptionViewInfo.Selected; }
		}
		public bool Locked {
			get { return bandCaptionViewInfo.Locked; }
			set { bandCaptionViewInfo.Locked = value; }
		}
		public BandCaptionViewInfo CaptionViewInfo {
			get {
				return bandCaptionViewInfo;
			}
		}
		ZoomService ZoomService {
			get {
				if(zoomService == null)
					zoomService = ZoomService.GetInstance(host);
				return zoomService;
			}
		}
		IBandViewInfoService BandViewInfoService {
			get {
				if(bandViewInfoService == null)
					bandViewInfoService = (IBandViewInfoService)host.GetService(typeof(IBandViewInfoService));
				return bandViewInfoService;
			}
		}
		public Rectangle RightSideHitTestBounds {
			get {
				Rectangle r = BandBounds;
				return new Rectangle(r.Right - 1, r.Y, 3, r.Height);
			}
		}
		public Rectangle LeftMarginHitTestBounds {
			get {
				Rectangle r = ClientBandBounds;
				return new Rectangle(r.Left - 1, r.Y, 3, r.Height);
			}
		}
		public Rectangle RightMarginHitTestBounds {
			get {
				Rectangle r = ClientBandBounds;
				return new Rectangle(r.Right - 1, r.Y, 3, r.Height);
			}
		}
		public Rectangle BottomSideHitTestBounds {
			get {
				Rectangle r = BandBounds;
				return new Rectangle(r.X, r.Bottom - 3, r.Width, 4);
			}
		}
		internal Rectangle BoundsWithChildren {
			get {
				Rectangle bounds = Bounds;
				foreach(Band band in Band.OrderedBands) {
					BandViewInfo bvi = BandViewInfoService.GetViewInfoByBand(band);
					if(bvi != null)
						bounds = Rectangle.Union(bounds, bvi.BoundsWithChildren);
				}
				return bounds;
			}
		}
		public BandViewInfo(float bandHeight, IDesignerHost host, BandViewPainter painter, BandCaptionPainter captionPainter, BandCaptionViewInfo captionViewInfo) {
			this.fReportDesigner = (ReportDesigner)host.GetDesigner(host.RootComponent);
			this.bandHeight = bandHeight;
			this.host = host;
			this.painter = painter;
			this.captionPainter = captionPainter;
			this.bandCaptionViewInfo = captionViewInfo;
		}
		protected virtual BandCaptionViewInfo CreateBandCaptionViewInfo(IDesignFrame designFrame, Image image, int level) {
			return new BandCaptionViewInfo(designFrame, image, level);
		}
		public void Calculate(RectangleF baseBounds) {
			fBandBounds = bandCaptionViewInfo.Calculate(baseBounds, bandHeight);
			fBounds = RectangleF.Union(CaptionBounds, BandBoundsF);
		}
		public virtual void DrawCaption(GraphicsCache cache) {
			if(captionPainter != null)
				ObjectPainter.DrawObject(cache, captionPainter, this.bandCaptionViewInfo);
		}
		public virtual void DrawBand(GraphicsCache cache) {
			if(painter != null)
				painter.Draw(this, cache);
		}
		public void DrawControlWarnings(GraphicsCache cache) {
			if(painter != null && Band != null)
				painter.DrawControlWarnings(this, cache);
		}
		public void DrawSelection(GraphicsCache cache) {
			if(Selected) {
				const float delta = 1f;
				RectangleF rect = RectHelper.InflateRect(BoundsWithChildren, 3 * delta, 2 * delta, 3 * delta, 3 * delta);
				ControlPaintHelper.DrawRectangle(cache.Graphics, cache.GetSolidBrush(Color.FromArgb(0x7f4f79ad)), 3 * delta, rect);
			}
			if(painter != null && Band != null)
				painter.DrawControlSelections(this, cache);
		}
		public PointF PointToBandDpi(PointF pt) {
			return ZoomService.FromScaledPixels(PointToBandPx(pt), Band.Dpi);
		}
		PointF PointToBandPx(PointF pt) {
			pt.X -= ClientBandBounds.X;
			pt.Y -= ClientBandBounds.Y;
			return pt;
		}
	}
	public class ColumnBandViewInfo : BandViewInfo, IColumnBandViewInfo {
		XtraReports.UI.MultiColumn multiColumn;
		public string MultiColumnText { get; set; }
		public ColumnBandViewInfo(float bandHeight, IDesignerHost host, BandViewPainter painter, BandCaptionPainter captionPainter, BandCaptionViewInfo captionViewInfo, XtraReports.UI.MultiColumn multiColumn)
			: base(bandHeight, host, painter, captionPainter, captionViewInfo) {
			this.multiColumn = multiColumn;
		}
		public float GetUsefulColumnWidth() {
			return multiColumn.GetUsefulColumnWidth(Band.ClientRectangleF.Width, Band.Dpi);
		}
		public float GetColumnSpacing() {
			return multiColumn.GetColumnSpacingInDpi(Band.Dpi);
		}
	}
}

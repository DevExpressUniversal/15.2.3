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
using DevExpress.Utils.Drawing;
using DevExpress.XtraReports.UI;
using System.Drawing;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel.Design;
using System.Drawing.Drawing2D;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraPrinting.NativeBricks;
using System.Linq;
using DevExpress.XtraReports.Design.Drawing;
namespace DevExpress.XtraReports.Design {
	public class BandViewPainter : BandViewPainterBase<IBandViewInfo> {
		#region static
		static readonly Color contourColor = Color.FromArgb(64, Color.Black);
		static readonly Brush contourBrush = new SolidBrush(contourColor);
		static readonly Color marginColor = Ruler.RulerSection.MarginColor;
		#endregion
		ZoomService zoomService;
		protected IDesignerHost designerHost;
		protected ZoomService ZoomService {
			get {
				if(zoomService == null)
					zoomService = (ZoomService)GetService(typeof(ZoomService));
				return zoomService;
			}
		}
		protected BandViewInfo BandViewInfo {
			get { return (BandViewInfo)viewInfo; }
		}
		protected Rectangle BandBounds { get { return BandViewInfo.BandBounds; } }
		protected Rectangle ClientBandBounds { get { return BandViewInfo.ClientBandBounds; } }
		public BandViewPainter(IServiceProvider serviceProvider, PrintingSystem printingSystem)
			: base(serviceProvider, printingSystem) {
			designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
		}
		protected override void DrawMargins(IGraphicsCache cache) {
			using(Pen pen = new Pen(marginColor, 1) { DashStyle = DashStyle.Dash }) {
				cache.Graphics.DrawLine(pen, ClientBandBounds.Left, BandBounds.Top, ClientBandBounds.Left, BandBounds.Bottom);
				if(BandViewInfo.DrawBottomMargin)
					cache.Graphics.DrawLine(pen, BandBounds.Left, BandBounds.Top, BandBounds.Right, BandBounds.Top);
				pen.Color = MarginWarnigColor;
				cache.Graphics.DrawLine(pen, ClientBandBounds.Right, BandBounds.Top, ClientBandBounds.Right, BandBounds.Bottom);
			}
		}
		Color MarginWarnigColor {
			get {
				return Report.DesignerOptions.ShowPrintingWarnings && Report.HasPrintingWarning() ? Color.Red :
					marginColor;
			}
		}
		protected override void DrawContent(IGraphicsCache cache) {
			base.DrawContent(cache);
			if(Band != null)
				DrawXRControlCollection(cache.Graphics, BandViewInfo.BandControls);
		}
		private void DrawXRControlCollection(Graphics gr, IList xrControls) {
			GraphicsHelper helper = new GraphicsHelper(gr);
			try {
				printingSystem.Graph.PageUnit = GraphicsUnit.Document;
				helper.PrepareGraphics(BandViewInfo, ZoomFactor, GraphicsUnit.Document);
				Dictionary<XRControl, XRControlWarning>.Enumerator en = BandViewInfo.XBandControlsInfo.GetEnumerator();
				while(en.MoveNext()) {
					if(en.Current.Value == XRControlWarning.None)
						DrawXBandControlContour(gr, en.Current.Key);
				}
				IGraphics gdiGraphics = new GdiGraphics(gr, printingSystem);
				XRWriteInfo writeInfo = new XRWriteInfo(printingSystem);
				foreach(XRControl xrControl in xrControls)
					DrawXRControl(gdiGraphics, xrControl, writeInfo);
				en = BandViewInfo.ControlsInfo.GetEnumerator();
				while(en.MoveNext()) {
					XRControl control = en.Current.Key;
					if((!(control is XRPageBreak || control is XRTableRow) && control.IsRealControl && en.Current.Value == XRControlWarning.None) &&
						(control.BackColor == Color.Transparent || !control.ShouldSerializeBackColor())) {
						DrawControlContour(gr, control, writeInfo);
					}
				}
			} finally {
				helper.ResetGraphics();
			}
		}
		void DrawXBandControlContour(Graphics gr, XRControl control) {
			ControlPaintHelper.DrawRectangle(gr,
				contourBrush,
				ZoomService.FromScaledPixels(1f, gr),
				control.GetBounds(Band, gr.PageUnit),
				control.GetVisibleBorders(Band));
		}
		void DrawControlContour(Graphics gr, XRControl control, XRWriteInfo writeInfo) {
			BorderSide correctBorders = control is XRTableCell ?
				RowSpanHelper.GetCellBorders(control as XRTableCell, writeInfo.MergedCells) : control.VisibleContourBorders;
			RectangleF correctBounds = control is XRTableCell ? 
				RowSpanHelper.GetCellBounds(control as XRTableCell, writeInfo.MergedCells, gr) : control.GetClipppedBandBounds(gr.PageUnit);
			ControlPaintHelper.DrawRectangle(gr,
				contourBrush,
				ZoomService.FromScaledPixels(1f, gr),
				correctBounds,
				correctBorders);
		}
		public void DrawControlWarnings(BandViewInfo viewInfo, IGraphicsCache cache) {
			this.viewInfo = viewInfo;
			Graphics gr = cache.Graphics;
			GraphicsHelper helper = new GraphicsHelper(gr);
			helper.PrepareGraphics(viewInfo, ZoomFactor, GraphicsUnit.Document);
			float rightClientBound = XRConvert.Convert(viewInfo.ReportClientRectangle.Right, Band.Dpi, GraphicsDpi.Document);
			try {
				Dictionary<XRControl, XRControlWarning>.Enumerator en = viewInfo.ControlsInfo.GetEnumerator();
				while(en.MoveNext()) {
					XRControl control = en.Current.Key;
					XRControlWarning warningInfo = en.Current.Value;
					RectangleF controlRect = control.GetBounds(Band, gr.PageUnit);
					RectangleF clipBounds = control.GetClipppedBandBounds(gr.PageUnit);
					if(warningInfo != XRControlWarning.None)
						ControlPaintHelper.FillRectangle(gr, ControlPaintHelper.WarningBrush, controlRect, clipBounds);
					if(control.IsRealControl && warningInfo != XRControlWarning.None)
						ControlPaintHelper.DrawRectangle(gr,
							ControlPaintHelper.WarningBorderBrush,
							ZoomService.FromScaledPixels(1f, gr),
							clipBounds);
					if((warningInfo & XRControlWarning.Printing) > 0)
						ControlPaintHelper.DrawControlPrintingWarning(gr, controlRect, clipBounds, rightClientBound);
				}
				DrawXBandControlWarnings(gr);
			} finally {
				helper.ResetGraphics();
			}
		}
		public void DrawControlSelections(BandViewInfo viewInfo, IGraphicsCache cache) {
			this.viewInfo = viewInfo;
			Graphics gr = cache.Graphics;
			GraphicsHelper helper = new GraphicsHelper(gr);
			helper.PrepareGraphics(viewInfo, ZoomFactor, GraphicsUnit.Document);
			float increaseBorders = ZoomService.FromScaledPixels(2f, gr);
			try {
				Dictionary<XRControl, XRControlWarning>.Enumerator en = viewInfo.ControlsInfo.GetEnumerator();
				while(en.MoveNext()) {
					XRControl control = en.Current.Key;
					if(control.IsRealControl && ControlIsSelected(control)) {
						XRControlDesignerBase designer = designerHost.GetDesigner(control) as XRControlDesignerBase;
						RectangleF controlRect = designer.GetBounds(Band, gr.PageUnit);
						if(!controlRect.IsEmpty)
							ControlPaintHelper.DrawControlSelection(gr,
								RectangleF.Inflate(controlRect, increaseBorders, increaseBorders),
								BorderSide.All,
								ZoomService.FromScaledPixels(1f, gr),
								en.Current.Value != XRControlWarning.None);
					}
				}
			} finally {
				helper.ResetGraphics();
			}
			DrawXBandControlSelections(gr, increaseBorders);
		}
		bool ControlIsSelected(XRControl control) {
			FrameSelectionUIService selectionService = GetService(typeof(FrameSelectionUIService)) as FrameSelectionUIService;
			return selectionService != null ? selectionService.ContainsControl(control) : false;
		}
		void DrawXBandControlSelections(Graphics gr, float increaseBorders) {
			RectangleF oldClipBounds = gr.ClipBounds;
			Region region = gr.Clip;
			region.Exclude(BandViewInfo.CaptionBounds);
			gr.Clip = region;
			GraphicsHelper helper = new GraphicsHelper(gr);
			helper.PrepareGraphics(BandViewInfo, ZoomFactor, GraphicsUnit.Document);
			try {
				float height = this.ZoomService.FromScaledPixels((float)BandViewInfo.CaptionBounds.Height, GraphicsDpi.GetGraphicsDpi(gr));
				Dictionary<XRControl, XRControlWarning>.Enumerator en = BandViewInfo.XBandControlsInfo.GetEnumerator();
				while(en.MoveNext()) {
					XRControl control = en.Current.Key;
					if(!ControlIsSelected(control))
						continue;
					RectangleF controlRect = control.GetBounds(Band, gr.PageUnit);
					controlRect = ApplyCaptionHeight(control, controlRect, height);
					BorderSide borders = control.GetVisibleBorders(Band);
					ControlPaintHelper.DrawControlSelection(gr,
						RectHelper.InflateRect(controlRect, increaseBorders, increaseBorders, increaseBorders, increaseBorders, borders),
						borders,
						ZoomService.FromScaledPixels(1f, gr),
						en.Current.Value != XRControlWarning.None);
				}
			} finally {
				helper.ResetGraphics();
				gr.Clip = new Region(oldClipBounds);
			}
		}
		RectangleF ApplyCaptionHeight(XRControl realControl, RectangleF controlRect, float height) {
			if((realControl.LocationParent == Band && realControl.RightBottomParent != Band)
				|| (realControl.LocationParent == Band && realControl.RightBottomParent == Band)
				|| Band is BottomMarginBand)
				return controlRect;
			return RectHelper.InflateRect(controlRect, 0, (float)Math.Ceiling(height), 0, 0);
		}
		void DrawXBandControlWarnings(Graphics gr) {
			Dictionary<XRControl, XRControlWarning>.Enumerator en = BandViewInfo.XBandControlsInfo.GetEnumerator();
			while(en.MoveNext()) {
				if(en.Current.Value == XRControlWarning.None)
					continue;
				XRControl control = en.Current.Key;
				RectangleF controlRect = control.GetBounds(Band, gr.PageUnit);
				ControlPaintHelper.DrawRectangle(gr,
					ControlPaintHelper.WarningBorderBrush,
				ZoomService.FromScaledPixels(1f, gr),
					controlRect,
				control.GetVisibleBorders(Band));
			}
		}
	}
	[Flags]
	public enum XRControlWarning { None, Export, Printing };
}

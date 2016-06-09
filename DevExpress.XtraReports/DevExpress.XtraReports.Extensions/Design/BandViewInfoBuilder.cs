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
using DevExpress.XtraReports.UI;
using System.Drawing;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Design.Drawing;
using System.Linq;
namespace DevExpress.XtraReports.Design {
	public class ViewInfoBuilder : IDisposable {
		#region static
		static Image GetImage(BandKind bandKind) {
			if(bandKind == BandKind.BottomMargin)
				return XRBitmaps.BottomMarginBand;
			if(bandKind == BandKind.TopMargin)
				return XRBitmaps.TopMarginBand;
			if(bandKind == BandKind.Detail)
				return XRBitmaps.DetailBand;
			if(bandKind == BandKind.DetailReport)
				return XRBitmaps.DetailReport;
			if(bandKind == BandKind.GroupFooter)
				return XRBitmaps.GroupFooterBand;
			if(bandKind == BandKind.GroupHeader)
				return XRBitmaps.GroupHeaderBand;
			if(bandKind == BandKind.PageFooter)
				return XRBitmaps.PageFooterBand;
			if(bandKind == BandKind.PageHeader)
				return XRBitmaps.PageHeaderBand;
			if(bandKind == BandKind.ReportFooter)
				return XRBitmaps.ReportFooterBand;
			if(bandKind == BandKind.ReportHeader)
				return XRBitmaps.ReportHeaderBand;
			if(bandKind == BandKind.SubBand)
				return XRBitmaps.SubBand;
			return null;
		}
		#endregion
		ReportDesigner reportDesigner;
		List<BandViewInfo> viewInfos;
		IDesignerHost host;
		BandCaptionPainter captionPainter;
		BandViewPainter bandPainter;
		ZoomService zoomService;
		public int BaseWidth {
			get { return reportDesigner.PageWidth; }
		}
		Image gridPattern;
		public ViewInfoBuilder(IDesignerHost host) {
			this.host = host;
			reportDesigner = (ReportDesigner)host.GetDesigner(host.RootComponent);
			System.Diagnostics.Debug.Assert(reportDesigner != null);
			if(reportDesigner == null)
				return;
			captionPainter = new BandCaptionPainterFlat();
			bandPainter = new BandViewPainter(host, reportDesigner.PrintingSystem);
			zoomService = ZoomService.GetInstance(host);
		}
		public void Dispose() {
			if(gridPattern != null) {
				gridPattern.Dispose();
				gridPattern = null;
			}
		}
		public BandViewInfo[] Build(Point offset) {
			if(reportDesigner == null)
				return new BandViewInfo[0];
			gridPattern = GridPatternCreator.Create(reportDesigner.ScaledGridSize, XtraReport.GridCellCount, BaseWidth, reportDesigner.LeftMargin, 1);
			viewInfos = new List<BandViewInfo>();
			AddViewInfos(((XtraReport)host.RootComponent).OrderedBands);
			for(int i = 0; i < viewInfos.Count; i++)
				viewInfos[i].CaptionViewInfo.HasTopBorder = i != 0;
			RectangleF baseBounds = new Rectangle(offset.X, offset.Y, BaseWidth, 0);
			for(int i = 0; i < viewInfos.Count; i++) {
				BandViewInfo viewInfo = viewInfos[i];
				viewInfo.CaptionViewInfo.HasBottomBorder = viewInfo.Expanded && viewInfo.BandHeight != 0;
				BandViewInfo prevViewInfo = i > 0 ? viewInfos[i - 1] : null;
				if(viewInfo.Band is BottomMarginBand) {
					viewInfo.DrawBottomMargin = true;
					if(prevViewInfo != null && prevViewInfo.Expanded)
						prevViewInfo.CaptionViewInfo.HasBottomBorder = true;
				}
				viewInfo.Calculate(baseBounds);
				baseBounds.Y = viewInfo.BoundsF.Bottom;
			}
			new WatermarkInfoCalculator(viewInfos.ToArray<IBandViewInfo>(), reportDesigner.RootReport, zoomService.ZoomFactor).Calculate();
			return viewInfos.ToArray();
		}
		private void AddViewInfos(IEnumerable bands) {
			foreach(Band band in bands) {
				if(band.Site == null)
					continue;
				IDesignFrame designFrame = host.GetDesigner(band) as IDesignFrame;
				if(designFrame == null) continue;
				BandViewInfo viewInfo = CreateBandViewInfo(band, designFrame);
				viewInfos.Add(viewInfo);
				if(!designFrame.Expanded) continue;
				AddViewInfos(band.OrderedBands);
			}
		}
		private BandViewInfo CreateBandViewInfo(Band band, IDesignFrame designFrame) {
			BandViewInfo viewInfo = CreateBandViewInfoCore(band, designFrame);
			viewInfo.Locked = !LockService.GetInstance(host).CanChangeComponent(band);
			viewInfo.GridPattern = gridPattern;
			return viewInfo;
		}
		BandViewInfo CreateBandViewInfoCore(Band band, IDesignFrame designFrame) {
		   MultiColumn mc;
		   if(band is TopMarginBand || band is BottomMarginBand) {
				BandCaptionViewInfo captionViewInfo = new EmptyCaptionViewInfo(designFrame);
				return new BandViewInfo(GetBandHeight(band), host, bandPainter, null, captionViewInfo);
			} else if(!designFrame.Expanded || band is XtraReportBase) {
				BandCaptionViewInfo captionViewInfo = new BandCaptionViewInfo(designFrame, GetImage(band.BandKind), band.NestedLevel);
				return new BandViewInfo(0, host, null, captionPainter, captionViewInfo);
			} else if(Band.TryGetMultiColumn(band, out mc)) {
				BandCaptionViewInfo captionViewInfo = new BandCaptionViewInfo(designFrame, GetImage(band.BandKind), band.NestedLevel);
				string text = ReportStringId.MultiColumnDesignMsg1.GetString() + '\x000D' + '\x000A' + ReportStringId.MultiColumnDesignMsg2.GetString();
				return new ColumnBandViewInfo(GetBandHeight(band), host, bandPainter, captionPainter, captionViewInfo, mc) { MultiColumnText = text };
			} else {
				BandCaptionViewInfo captionViewInfo = new BandCaptionViewInfo(designFrame, GetImage(band.BandKind), band.NestedLevel);
				return new BandViewInfo(GetBandHeight(band), host, bandPainter, captionPainter, captionViewInfo);
			}
		}
		float GetBandHeight(Band band) {
			return zoomService.ToScaledPixels(band.HeightF, band.Dpi);
		}
	}
}

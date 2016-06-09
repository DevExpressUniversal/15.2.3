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
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design.Behaviours {
	class CrossBandDesignerBehaviour : DesignerBehaviour {
		XRCrossBandControl CrossBandControl {
			get { return XRControl as XRCrossBandControl; }
		}
		new XRCrossBandControlDesigner Designer {
			get { return (XRCrossBandControlDesigner)base.Designer; }
		}
		public CrossBandDesignerBehaviour(IServiceProvider servProvider)
			: base(servProvider) {
		}
		public override void SetDefaultComponentBounds() {
			CapturePaintService captPaintSvc = (CapturePaintService)GetService(typeof(CapturePaintService));
			System.Diagnostics.Debug.Assert(captPaintSvc != null);
			RectangleF bounds = captPaintSvc.DragBounds;
			captPaintSvc.DragBounds = Rectangle.Empty;
			CrossBandControl.SuspendLayout();
			try {
				if(RectHelper.RectangleFIsEmpty(bounds, 0.001)) {
					Band band = GetStartBand();
					RectangleF bandBounds = this.BandViewSvc.GetControlScreenBounds(band);
					bounds.Location = bandBounds.Location;
				}
				if(RectHelper.RectangleFIsEmptySize(bounds, 0.001))
					bounds.Size = XRConvert.Convert(Designer.DefaultControlSize, XRControl.Dpi, GraphicsDpi.Pixel);
				this.CrossBandControl.WidthF = ZoomService.GetInstance(servProvider).FromScaledPixels(bounds.Width, CrossBandControl.Dpi);
				foreach(BandViewInfo viewInfo in this.BandViewSvc.ViewInfos) {
					if(viewInfo.Band is XtraReportBase)
						continue;
					RectangleF bandBounds = this.BandViewSvc.GetControlScreenBounds(viewInfo.Band);
					if(!bandBounds.IntersectsWith(bounds))
						continue;
					RectangleF rect = RectangleF.Intersect(bandBounds, bounds);
					rect.Offset(-bandBounds.X, -bandBounds.Y);
					if(CrossBandControl.StartBand == null) {
						CrossBandControl.StartBand = viewInfo.Band;
						CrossBandControl.StartPointF = XRConvert.Convert(rect.Location, GraphicsDpi.Pixel, XRControl.Dpi);
					}
					CrossBandControl.EndBand = viewInfo.Band;
					CrossBandControl.EndPointF = XRConvert.Convert(new PointF(rect.Left, rect.Bottom), GraphicsDpi.Pixel, XRControl.Dpi);
				}
			} finally {
				CrossBandControl.ResumeLayout();
			}
		}
		Band GetStartBand() {
			ISelectionService selectionService = GetService(typeof(ISelectionService)) as ISelectionService;
			XRControl control = selectionService.PrimarySelection as XRControl;
			if(control is XtraReportBase)
				return ((XtraReportBase)control).Bands[BandKind.Detail];
			if(control is Band)
				return (Band)control;
			if(!(control is XRCrossBandControl))
				return control.Band;
			return this.RootReport.Bands[BandKind.Detail];
		}
	}
}

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
namespace DevExpress.XtraReports.Design.Adapters {
	class CrossBandBoundsAdapter : BoundsAdapter {
		XRCrossBandControl CrossBandControl {
			get { return (XRCrossBandControl)XRControl; }
		}
		XRCrossBandControlDesigner CrossBandControlDesigner {
			get { return (XRCrossBandControlDesigner)Designer; }
		}
		public CrossBandBoundsAdapter(XRCrossBandControl xrControl, IServiceProvider servProvider)
			: base(xrControl, servProvider) {
		}
		public override void SetScreenBounds(RectangleF screenRect) {
			Band startBand = this.BandViewSvc.GetNearestViewInfoByScreenPoint(screenRect.Location).Band;
			Band endBand = this.BandViewSvc.GetNearestViewInfoByScreenPoint(new PointF(screenRect.Left, screenRect.Bottom)).Band;
			SetScreenProperties(screenRect, startBand, endBand);
		}
		public override RectangleF GetScreenBounds() {
			BandViewInfo startViewInfo = BandViewSvc.GetViewInfoByBand(this.CrossBandControl.StartBand);
			RectangleF startRect = this.ClientBandBoundsToScreen(GetClientBandBounds(startViewInfo));
			BandViewInfo endViewInfo = BandViewSvc.GetViewInfoByBand(this.CrossBandControl.EndBand);
			RectangleF endRect = this.ClientBandBoundsToScreen(GetClientBandBounds(endViewInfo));
			return RectangleF.Union(startRect, endRect);
		}
		public override RectangleF GetClientBandBounds() {
			return GetClientBandBounds(BandViewSvc.GetViewInfoByBand(this.CrossBandControl.StartBand));
		}
		public override RectangleF GetClientBandBounds(BandViewInfo viewInfo) {
			if(viewInfo == null)
				return Rectangle.Empty;
			RectangleF bounds = this.CrossBandControl.GetBounds(viewInfo.Band);
			if(bounds == RectangleF.Empty)
				return Rectangle.Empty;
			RectangleF rect = this.ZoomService.ToScaledPixels(bounds, this.CrossBandControl.Dpi);
			rect.Offset(viewInfo.ClientBandBounds.Location);
			return rect;
		}
		void SetScreenProperties(RectangleF screenRect, Band startBand, Band endBand) {
			RectangleF validScreenRect = screenRect;
			if(PointToBand(screenRect.Location, startBand).Y < 0)
				validScreenRect = new RectangleF(screenRect.X, 0, screenRect.Width, screenRect.Height);
			PointF startPoint = PointToBand(validScreenRect.Location, startBand);
			PointF endPoint = PointToBand(new PointF(validScreenRect.Left, validScreenRect.Bottom), endBand);
			PointF rightBottom = PointToBand(new PointF(validScreenRect.Right, validScreenRect.Top), startBand);
			float width = rightBottom.X - startPoint.X;
			CrossBandControlDesigner.SetProperties(startBand, endBand, startPoint, endPoint, width);
		}
		PointF PointToBand(PointF point, Band band) {
			IBoundsAdapter bandAdapter = GetControlAdapter(band);
			RectangleF bandRect = bandAdapter.GetScreenBounds();
			point.X -= bandRect.X;
			point.Y = point.Y - bandRect.Y < 0 ? 0 : point.Y - bandRect.Y;
			return this.ZoomService.FromScaledPixels(point, XRControl.Dpi);
		}
	}
}

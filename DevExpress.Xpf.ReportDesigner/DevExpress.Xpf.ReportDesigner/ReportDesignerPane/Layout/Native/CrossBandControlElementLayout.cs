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
using DevExpress.XtraReports.UI;
using PointF = System.Drawing.PointF;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public class CrossBandControlElementLayout : BaseReportElementLayout {
		readonly XRCrossBandControl xrControl;
		XRControlStartBandEndBandTracker startBandEndBandTracker;
		public CrossBandControlElementLayout(XRCrossBandControl xrControl) {
			this.xrControl = xrControl;
			startBandEndBandTracker = new XRControlStartBandEndBandTracker(xrControl, () => { RaiseRef1Changed(); RaiseRef2Changed(); RaiseParam4Changed(); });
		}
		public override XRControl Ref1 { get { return xrControl.StartBand; } }
		public override XRControl Ref2 { get { return xrControl.EndBand; } }
		double? endPosition;
		public override double Param4 {
			get {
				if(endPosition != null)
					return endPosition.Value;
				endPosition = xrControl.EndBand == null ? 0.0 : BoundsConverter.ToDouble(xrControl.EndBand.HeightF - xrControl.EndPointF.Y, xrControl.Dpi);
				return endPosition.Value;
			}
			set {
				endPosition = null;
				if(xrControl.EndBand != null)
					xrControl.EndPointF = new PointF(xrControl.EndPointF.X, xrControl.EndBand.HeightF - BoundsConverter.ToFloat(value, xrControl.Dpi));
				RaiseParam4Changed();
			}
		}
		public override double Param3 {
			get { return BoundsConverter.ToDouble(xrControl.WidthF, xrControl.Dpi); }
			set {
				xrControl.WidthF = BoundsConverter.ToFloat(value, xrControl.Dpi);
				RaiseParam3Changed();
			}
		}
		public override double Param1 {
			get { return BoundsConverter.ToDouble(xrControl.StartPointF.X, xrControl.Dpi); }
			set {
				xrControl.StartPointF = new PointF(BoundsConverter.ToFloat(value, xrControl.Dpi), xrControl.StartPointF.Y);
				RaiseParam1Changed();
			}
		}
		public override double Param2 {
			get { return BoundsConverter.ToDouble(xrControl.StartPointF.Y, xrControl.Dpi); }
			set {
				xrControl.StartPointF = new PointF(xrControl.StartPointF.X, BoundsConverter.ToFloat(value, xrControl.Dpi));
				RaiseParam2Changed();
			}
		}
	}
}

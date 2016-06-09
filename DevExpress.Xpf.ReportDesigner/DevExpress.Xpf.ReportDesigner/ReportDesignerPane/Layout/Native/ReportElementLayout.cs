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
using System.Windows;
using DevExpress.XtraReports.UI;
using SizeF = System.Drawing.SizeF;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class ReportElementLayout : BaseReportElementLayout {
		readonly XRControl xrControl;
		public ReportElementLayout(XRControl xrControl) {
			this.xrControl = xrControl;
			xrControl.LocationChanged += OnXRControlLocationChanged;
			xrControl.SizeChanged += OnXRControlSizeChanged;
		}
		void OnXRControlLocationChanged(object sender, ChangeEventArgs e) {
			RaiseParam1Changed();
			RaiseParam2Changed();
		}
		void OnXRControlSizeChanged(object sender, ChangeEventArgs e) {
			var oldValue = (SizeF)e.OldValue;
			var newValue = (SizeF)e.NewValue;
			if(oldValue.Width != newValue.Width)
				RaiseParam3Changed();
			if(oldValue.Height != newValue.Height)
				RaiseParam4Changed();
		}
		public override XRControl Ref1 { get { return null; } }
		public override XRControl Ref2 { get { return null; } }
		public override double Param1 {
			get { return BoundsConverter.ToDouble(xrControl.LocationF.X, xrControl.Dpi); }
			set {
				var locationF = BoundsConverter.ToFloat(value, xrControl.Dpi);
				xrControl.LocationF = new System.Drawing.PointF(locationF, xrControl.LocationF.Y);
			}
		}
		public override double Param2 {
			get { return BoundsConverter.ToDouble(xrControl.LocationF.Y, xrControl.Dpi); }
			set {
				var locationF = BoundsConverter.ToFloat(value, xrControl.Dpi);
				xrControl.LocationF = new System.Drawing.PointF(xrControl.LocationF.X, locationF);
			}
		}
		public override double Param4 {
			get { return BoundsConverter.ToDouble(xrControl.HeightF, xrControl.Dpi); }
			set { xrControl.HeightF = BoundsConverter.ToFloat(value, xrControl.Dpi); }
		}
		public override double Param3 {
			get { return BoundsConverter.ToDouble(xrControl.WidthF, xrControl.Dpi); }
			set { xrControl.WidthF = BoundsConverter.ToFloat(value, xrControl.Dpi); }
		}
	}
}

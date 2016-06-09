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
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class ReportLayout : BaseReportElementLayout {
		readonly ReportWidthTracker widthTracker;
		readonly ReportBottomBandTracker bottomBandTracker;
		readonly ReportMarginTracker marginTracker;
		public ReportLayout(XtraReport report) {
			widthTracker = new ReportWidthTracker(report, RaiseParam3Changed);
			bottomBandTracker = new ReportBottomBandTracker(report, RaiseRef2Changed);
			marginTracker = new ReportMarginTracker(report, () => { RaiseParam1Changed(); RaiseParam2Changed(); });
		}
		public override XRControl Ref1 { get { return null; } }
		public override XRControl Ref2 { get { return bottomBandTracker.BottomBand; } }
		public override double Param4 {
			get { return 0.0; }
			set {
				if(value != 0.0)
					throw new NotSupportedException();
			}
		}
		public override double Param1 {
			get { return marginTracker.ReportLeftMargin; }
			set { throw new NotSupportedException(); }
		}
		public override double Param2 {
			get { return marginTracker.ReportRightMargin; }
			set { throw new NotSupportedException(); }
		}
		public override double Param3 {
			get { return widthTracker.ReportWidth; }
			set { throw new NotSupportedException(); }
		}
	}
}

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
using System.ComponentModel;
using DevExpress.Mvvm.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class ReportLayoutProvider : IReportLayoutProvider {
		public ReportLayoutProvider(XtraReport report) {
			Report = report;
		}
		public readonly XtraReport Report;
		public BaseReportElementLayout GetLayout(XRControl xrObject) {
			if(Report == xrObject)
				return new ReportLayout(Report);
			var subReport = xrObject as XtraReportBase;
			if(subReport != null)
				return new DetailReportLayout(subReport);
			var subBand = xrObject as SubBand;
			if(subBand != null)
				return new SubBandLayout(subBand);
			var band = xrObject as Band;
			if(band != null)
				return new BandLayout(band);
			var crossBandControl = xrObject as XRCrossBandControl;
			if(crossBandControl != null)
				return new CrossBandControlElementLayout(crossBandControl);
			return new ReportElementLayout(xrObject);
		}
	}
}

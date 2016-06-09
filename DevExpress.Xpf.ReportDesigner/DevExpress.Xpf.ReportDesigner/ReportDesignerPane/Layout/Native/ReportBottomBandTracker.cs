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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.ReportExtensions;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public sealed class ReportBottomBandTracker : IDisposable {
		readonly Action onReportBottomBandChanged;
		XtraReportBase report;
		readonly ReportBandsCollectionTracker bandsTracker;
		public ReportBottomBandTracker(XtraReportBase report, Action onReportBottomBandChanged) {
			this.report = report;
			bandsTracker = new ReportBandsCollectionTracker(report, OnReportBandsCollectionChanged);
			this.onReportBottomBandChanged = onReportBottomBandChanged;
		}
		public void Dispose() {
			bandsTracker.Dispose();
			report = null;
			bottomBand = null;
		}
		void OnReportBandsCollectionChanged() {
			bottomBand = null;
			bottomBandSet = false;
			if(onReportBottomBandChanged != null)
				onReportBottomBandChanged();
		}
		bool bottomBandSet;
		Band bottomBand;
		public Band BottomBand {
			get {
				if(!bottomBandSet) {
					bottomBand = GetReportBottomBand(report);
					bottomBandSet = true;
				}
				return bottomBand;
			}
		}
		static Band GetReportBottomBand(XtraReportBase report) {
			return report.OrderedBands.LastOrDefault();
		}
	}
}

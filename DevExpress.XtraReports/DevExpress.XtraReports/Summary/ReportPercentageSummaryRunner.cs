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
using System.Collections;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Native.Summary {
	public class ReportPercentageSummaryRunner : ReportSummaryRunner<PercentageUpdater> {
		public ReportPercentageSummaryRunner(XRSummary summary, PercentageUpdater updater)
			: base(summary, updater) {
			if(fSummary.Running != SummaryRunning.Report)
				throw new ArgumentException("summary");
		}
		public override void OnGroupUpdate() {
			if(fSummary.Control.Band is DetailBand)
				return;
			UpdateSummaryBricks();
		}
		public override void OnGroupFinished() {
			if(fSummary.Control.Band is DetailBand)
				return;
			Updater.Accumulate();
			fSummary.Reset();
		}
		public override void OnPageUpdate() {
			if(fSummary.Control.Band is PageBand || fSummary.Control.Band is MarginBand)
				UpdateSummaryBricks();
		}
		public override void OnPageFinished() {
			if(fSummary.Control.Band is PageBand || fSummary.Control.Band is MarginBand) {
				Updater.Accumulate();
				fSummary.Reset();
			}
		}
		public override void OnReportUpdate() {
			Updater.Evaluate();
			UpdateSummaryBricks();
		}
		public override void OnReportFinished() {
			fSummary.Reset();
		}
	}
	public class ReportPercentageUpdater : PercentageUpdater {
		public ReportPercentageUpdater(XRSummary summary)
			: base(summary) {
		}
		public override void Update() {
			object nativeValue = fSummary.GetNativeValue(fSummary.Values);
			foreach(VisualBrick brick in fSummary.Bricks) {
				object val = GetValueCore(brick.TextValue, nativeValue);
				RaiseSummaryCalculated(brick, val);
			}
		}
	}
}

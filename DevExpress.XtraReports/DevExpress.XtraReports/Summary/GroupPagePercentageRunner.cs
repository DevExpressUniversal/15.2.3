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
using DevExpress.XtraReports.Native;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native.Summary {
	public class PagePercentageGroupRunner : PageSummaryRunner<PercentageUpdater> {
		public PagePercentageGroupRunner(XRSummary summary, PercentageUpdater updater)
			: base(summary, updater) {
		}
		public override void OnGroupFinishedOnPage() {
			UpdateSummaryBricks();
			Updater.Accumulate();
			fSummary.Reset();
		}
		public override void OnPageUpdate() {
			UpdateSummaryBricks();
			Updater.Evaluate();
			UpdateSummaryBricks();
		}
		public override void OnPageFinished() {
			fSummary.Reset();
			Updater.Clear();
		}
	}
	public class PercentageUpdater : SummaryUpdater {
		ValuesRowsContainer valuesInfo = new ValuesRowsContainer();
		List<VisualBrick> bricks = new List<VisualBrick>();
		protected object percentageValue;
		public PercentageUpdater(XRSummary summary)
			: base(summary) {
		}
		public override void Clear() {
			this.valuesInfo.Clear();
			this.bricks.Clear();
			percentageValue = null;
		}
		public void Evaluate() {
			foreach(var brick in bricks)
				fSummary.Bricks.Add(brick);
			fSummary.ValuesInfo.AddRange(valuesInfo);
			percentageValue = fSummary.GetNativeValue(fSummary.Values);
		}
		public void Accumulate() {
			valuesInfo.AddRange(fSummary.ValuesInfo);
			bricks.AddRange(fSummary.Bricks);
		}
		public override void Update() {
			object nativeValue = fSummary.GetNativeValue(fSummary.Values);
			foreach(VisualBrick brick in fSummary.Bricks) {
				object val = GetValueCore(brick.TextValue, nativeValue);
				RaiseSummaryCalculated(brick, val);
			}
		}
		protected object GetValueCore(object textValue, object nativeValue) {
			try {
				return percentageValue != null ? Convert.ToDecimal(textValue) / Convert.ToDecimal(percentageValue) : nativeValue;
			}
			catch {
				return null;
			}
		}
	}
}

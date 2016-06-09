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
using DevExpress.XtraPrinting;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.Utils;
using System.Linq;
using DevExpress.Data;
namespace DevExpress.XtraReports.UI {
	using DevExpress.XtraPrinting.Native;
	static class SummaryExtensions {
		#region XRSummary
		public static string GetText(this XRSummary summary, object val) {
			string result = null;
			if(!String.IsNullOrEmpty(summary.FormatString)) {
				try {
					result = String.Format(summary.FormatString, val);
				} catch(FormatException ex) {
					DevExpress.XtraPrinting.Tracer.TraceError(NativeSR.TraceSource, ex);
				}
			}
			return result != null ? result : Convert.ToString(val);
		}
		public static bool NeedsPageCalculation(this XRSummary summary) {
			return (summary.Control.Band is PageBand || summary.Control.Band is MarginBand) && summary.IsReportRunningSum();
		}
		public static bool NeedsGroupCalculation(this XRSummary summary, XtraReportBase report, XRGroup group) {
			Band band = summary.Control.Band;
			return summary.IsAppropriateReport(report) && 
				(band is DetailBand || band == group.Header || band == group.Footer) &&
				(summary.Running == SummaryRunning.Group || summary.IsReportRunningSum());
		}
		static bool IsReportRunningSum(this XRSummary summary) {
			return summary.Running == SummaryRunning.Report && (summary.Func == SummaryFunc.RunningSum || summary.Func == SummaryFunc.Percentage);
		}
		public static bool IsAppropriateReport(this XRSummary summary, XtraReportBase report) {
			return summary.Control.Report == report;
		}
		#endregion
		#region SummaryFunc
		public static bool NeedsCalculation(this SummaryFunc func) {
			return func != SummaryFunc.Count && func != SummaryFunc.RecordNumber;
		}
		#endregion
		#region SummaryRunning
		static readonly Type[] ignoreBandTypes = new Type[] { typeof(ReportHeaderBand), typeof(ReportFooterBand), typeof(PageHeaderBand), typeof(PageFooterBand), typeof(TopMarginBand), typeof(BottomMarginBand) };
		public static bool IsAppropriateBand(this SummaryRunning running, Band band) {
			return running != SummaryRunning.Group || !ignoreBandTypes.Contains(band.GetType());
		}
		public static bool Exists(this SummaryRunning running) {
			return running != SummaryRunning.None;
		}
		#endregion
	}
}
namespace DevExpress.XtraReports.Native.Summary {
	public class PageSummaryContainer {
		IEnumerable<XRSummary> summaries;
		public PageSummaryContainer(IEnumerable<XRSummary> summaries) {
			this.summaries = summaries;
		}
		public void OnGroupBegin(XtraReportBase report) {
			foreach(XRSummary summary in summaries) {
				if(summary.IsAppropriateReport(report))
					summary.OnGroupBeginOnPage();
			}
		}
		public void OnGroupFinished(XtraReportBase report, GroupHeaderBand groupHeader) {
			foreach(XRSummary summary in summaries) {
				if(summary.IsAppropriateReport(report) && ReferenceEquals(groupHeader, summary.Control.Band))
					summary.OnGroupFinishedOnPage();
			}
		}
		public void OnRowChanged(XtraReportBase report) {
			foreach(XRSummary summary in summaries)
				summary.OnDataRowChangedOnPage(report);
		}
		public void AddSummaryBrick(VisualBrick brick) {
			foreach(XRSummary summary in summaries)
				if(summary.Control == brick.BrickOwner)
					summary.AddSummaryBrickOnPage(brick);
		}
		public void OnPageFinished() {
			foreach(XRSummary summary in summaries)
				summary.OnPageUpdate();
			foreach(XRSummary summary in summaries)
				summary.OnPageFinished();
		}
		public void OnReportFinished() {
			foreach(XRSummary summary in summaries) {
				summary.OnReportUpdate();
			}
			foreach(XRSummary summary in summaries) {
				summary.OnReportFinished();
			}
			foreach(XRSummary summary in summaries) {
				summary.Reset();
				summary.ResetRunningValues();
			}
		}
	}
	public class XRSummaryContainer {
		List<XRSummary> summaries = new List<XRSummary>();
		List<XRSummary> pageSummaries = new List<XRSummary>();
		Dictionary<XRSummary, int[]> summaryRowIndexes = new Dictionary<XRSummary, int[]>();
		Dictionary<XRLabel, IList<XRSummary>> controlSummaries = new Dictionary<XRLabel, IList<XRSummary>>();
		public IEnumerable<XRSummary> PageSummaries {
			get { return pageSummaries; }
		}
		public XRSummaryContainer(IEnumerable<XRLabel> labels) {
			foreach(XRLabel label in labels) {
				if(label.Summary.Running == SummaryRunning.None)
					continue;
				else if(label.Summary.Running == SummaryRunning.Page || label.Summary.NeedsPageCalculation())
					pageSummaries.Add(label.Summary);
				else
					summaries.Add(label.Summary);
			}
			ResetControlSummaries();
		}
		void ResetControlSummaries() {
			controlSummaries.Clear();
			ResetControlSummaries(summaries);
		}
		void ResetControlSummaries(IList<XRSummary> summaries) {
			foreach(XRSummary item in summaries)
				controlSummaries[item.Control] = new List<XRSummary>(new XRSummary[] { item });
		}
		IEnumerable<XRSummary> AllSummaries() {
			foreach(var item in controlSummaries) {
				foreach(var subItem in item.Value)
					yield return subItem;
			}
		}
		public void ResetCalculation(XtraReportBase report) {
			OnReportBegin(report);
			int[] rowIndexes = GetRowIndexes(report);
			foreach(XRSummary summary in AllSummaries()) {
				int[] indexes = GetSummaryRowIndexes(summary);
				if(summary.IsAppropriateReport(report) && (indexes == null || ArraysIdentical(rowIndexes, indexes) || ArraysEqual(rowIndexes, indexes, 1))) {
					summary.Reset();
					summary.ResetRunningValues();
				}
			}
		}
		static bool ArraysEqual(int[] first, int[] second, int startIndex) {
			return ArrayHelper.ArraysEqual<int>(first, second, startIndex, EqualityComparer<int>.Default);
		}
		static bool ArraysIdentical(int[] first, int[] second) {
			return (first == null || first.Length == 1) && (second == null || second.Length == 1);
		}
		public XRSummary GetActualSummary(XRLabel label) {
			IList<XRSummary> summaries;
			if(controlSummaries.TryGetValue(label, out summaries)) {
				int[] rowIndexes = GetRowIndexes(label.Report);
				foreach(var summary in summaries) {
					int[] indexes = GetSummaryRowIndexes(summary);
					if(ArraysEqual(rowIndexes, indexes, 1) && rowIndexes[0] >= indexes[0])
						return summary;
				}
				return null;
			} else
				return label.Summary;
		}
		public void ResetRunningCalculation() {
			ResetRunningCalculation(summaries);
			ResetRunningCalculation(pageSummaries);
			ResetControlSummaries();
		}
		void ResetRunningCalculation(IList<XRSummary> summaries) {
			foreach(XRSummary summary in summaries) {
				if(summary.Func == SummaryFunc.RunningSum)
					summary.ResetRunningValues();
			}
		}
		public void OnRowChangedOnReport(XtraReportBase report) {
			int[] rowIndexes = GetRowIndexes(report);
			foreach(XRSummary item in AllSummaries()) {
				int[] indexes = GetSummaryRowIndexes(item);
				if(report.DataBrowser.Count > 0 && (indexes == null || rowIndexes != null && ArrayHelper.EndsWith<int>(rowIndexes, indexes, indexes.Length - 1)))
					item.OnDataRowChangedOnReport(report);
			}
		}
		int[] GetRowIndexes(XtraReportBase report) {
			return report.GetRowIndexes();
		}
		public void OnGroupBegin(XtraReportBase report, XRGroup group) {
			if(group == null)
				return;
			int[] rowIndexes = GetRowIndexes(report);
			foreach(XRSummary item in summaries) {
				XRSummary summary = item;
				if(!summary.NeedsGroupCalculation(report, group)) continue;
				if(summaryRowIndexes.ContainsKey(summary)) {
					summary = summary.Clone();
					IList<XRSummary> items;
					if(controlSummaries.TryGetValue(summary.Control, out items)) {
						items.Add(summary);
						if(report.DataBrowser.Count > 0)
							summary.OnDataRowChangedOnReport(report);
					}
				}
				summaryRowIndexes[summary] = rowIndexes;
				summary.OnGroupBegin();
			}
		}
		public void OnGroupFinished(XtraReportBase report, XRGroup group) {
			if(group == null)
				return;
			int[] rowIndexes = GetRowIndexes(report);
			XRSummary[] selectedSummaries = AllSummaries().Where(
				summary => {
					int[] indexes = GetSummaryRowIndexes(summary);
					return summary.NeedsGroupCalculation(report, group) && (ArraysIdentical(rowIndexes, indexes) || ArraysEqual(rowIndexes, indexes, 1));
				}
			).ToArray();
			foreach(XRSummary summary in selectedSummaries) {
				summary.OnGroupUpdate();
			}
			foreach(XRSummary summary in selectedSummaries) {
				summary.OnGroupFinished();
				if(summary.Running != SummaryRunning.Report)
					RemoveSummary(summary);
			}
		}
		int[] GetSummaryRowIndexes(XRSummary summary) {
			int[] rowIndexes;
			summaryRowIndexes.TryGetValue(summary, out rowIndexes);
			return rowIndexes;
		}
		void OnReportBegin(XtraReportBase report) {
			foreach(XRSummary item in summaries) {
				XRSummary summary = item;
				if(summary.Running != SummaryRunning.Report || !summary.IsAppropriateReport(report)) continue;
				if(summaryRowIndexes.ContainsKey(summary)) {
					summary = summary.Clone();
					IList<XRSummary> items;
					if(controlSummaries.TryGetValue(summary.Control, out items))
						items.Add(summary);
				}
				summaryRowIndexes[summary] = GetRowIndexes(report);
			}
		}
		public void OnReportFinished(XtraReportBase report) {
			int[] rowIndexes = GetRowIndexes(report);
			XRSummary[] selectedSummaries = AllSummaries().Where(
				summary => {
					int[] indexes = GetSummaryRowIndexes(summary);
					return summary.IsAppropriateReport(report) && (ArraysIdentical(rowIndexes, indexes) || ArraysEqual(rowIndexes, indexes, 1));
				}
			).ToArray();
			foreach(XRSummary summary in selectedSummaries) {
				summary.OnReportUpdate();
			}
			foreach(XRSummary summary in selectedSummaries) {
				summary.OnReportFinished();
				RemoveSummary(summary);
			}
		}
		void RemoveSummary(XRSummary summary) {
			summaryRowIndexes.Remove(summary);
			if(ReferenceEquals(summary, summary.Control.Summary))
				return;
			IList<XRSummary> items;
			if(controlSummaries.TryGetValue(summary.Control, out items))
				items.Remove(summary);
		}
		internal void ResetReportCache() {
			foreach(XRSummary summary in AllSummaries())
				summary.ResetReportCache();
		}
	}
}

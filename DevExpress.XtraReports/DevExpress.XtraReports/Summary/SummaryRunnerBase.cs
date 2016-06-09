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
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Native.Summary {
	public interface ISummaryRunner {
		void Reset();
		void OnDataRowChangedOnReport(XtraReportBase report);
		void OnDataRowChangedOnPage(XtraReportBase report);
		void AddSummaryBrickOnReport(VisualBrick brick);
		void AddSummaryBrickOnPage(VisualBrick brick);
		void OnGroupUpdate();
		void OnGroupFinished();
		void OnGroupBegin();
		void OnReportUpdate();
		void OnReportFinished();
		void OnPageUpdate();
		void OnPageFinished();
		void OnGroupFinishedOnPage();
	}
	public class SummaryRunnerBase<T> : ISummaryRunner where T : SummaryUpdater {
		protected XRSummary fSummary;
		T updater;
		public T Updater {
			get { return updater; }
		}
		public SummaryRunnerBase(XRSummary summary, T updater) {
			fSummary = summary;
			this.updater = updater;
		}
		public void Reset() {
			Updater.Clear();
		}
		public virtual void OnDataRowChangedOnReport(XtraReportBase report) { }
		public virtual void OnDataRowChangedOnPage(XtraReportBase report) { }
		public virtual void AddSummaryBrickOnReport(VisualBrick brick) { }
		public virtual void AddSummaryBrickOnPage(VisualBrick brick) { }
		public virtual void OnGroupUpdate() { }
		public virtual void OnGroupFinished() { }
		public virtual void OnGroupBegin() { }
		public virtual void OnReportUpdate() { }
		public virtual void OnReportFinished() { }
		public virtual void OnPageUpdate() { }
		public virtual void OnPageFinished() { }
		protected virtual void AddValueFromBinding(XtraReportBase report, XRBinding binding) {
			object val = binding != null ? binding.GetImmediateColumnValue(report.DataContext, null) : String.Empty;
			if(fSummary.Func == SummaryFunc.RecordNumber) {
				UpdateSummaryBricks();
				fSummary.Bricks.Clear();
			}
			if(!fSummary.IgnoreNullValues || (val != null && !(val is DBNull)))
				fSummary.AddValue(val, report.CurrentRowIndex);
			fSummary.Control.OnSummaryRowChanged(EventArgs.Empty);
		}
		public virtual void OnGroupFinishedOnPage() {
		}
		protected void UpdateSummaryBricks() {
			Updater.Update();
		}
	}
	public class SummaryUpdater {
		protected XRSummary fSummary;
		public SummaryUpdater(XRSummary summary) {
			fSummary = summary;
		}
		public virtual void Clear() {
		}
		public virtual bool CanAddBrick {
			get {
				return true;
			}
		}
		public virtual void Update() {
			ProcessSummaryBricksCore(fSummary.Values);
		}
		protected void ProcessSummaryBricksCore(IList values) {
			object nativeValue = fSummary.GetNativeValue(values);
			foreach(VisualBrick brick in fSummary.Bricks)
				RaiseSummaryCalculated(brick, nativeValue);
		}
		protected void RaiseSummaryCalculated(VisualBrick brick, object val) {
			brick.BrickOwner.RaiseSummaryCalculated(brick, fSummary.GetText(val), fSummary.FormatString, val);
		}
	}
	class SimplePercentageUpdater : SummaryUpdater {
		public SimplePercentageUpdater(XRSummary summary)
			: base(summary) {
			if(summary.Func != SummaryFunc.Percentage)
				throw new ArgumentException("summary");
		}
		public override void Update() {
			object nativeValue = fSummary.GetNativeValue(fSummary.Values);
			for(int i = 0; i < fSummary.Bricks.Count; i++) {
				object item;
				object val = TryGetItem(fSummary.Values, i, out item) ? GetValue(item, nativeValue) : nativeValue;
				RaiseSummaryCalculated(fSummary.Bricks[i], val);
			}
		}
		static bool TryGetItem(IList items, int index, out object value) {
			if(index >= 0 && index < items.Count) {
				value = items[index];
				return true;
			}
			value = null;
			return false;
		}
		object GetValue(object val, object nativeValue) {
			if(val != null && fSummary.Control.Band is DetailBand && (fSummary.Running == SummaryRunning.Group || fSummary.Running == SummaryRunning.Page)) {
				try {
					return Convert.ToDecimal(val) / Convert.ToDecimal(nativeValue);
				} catch { }
			}
			return nativeValue;
		}
	}
	public class RunningSummaryUpdater : SummaryUpdater {
		ValuesRowsContainer accumulatedInfo = new ValuesRowsContainer();
		public RunningSummaryUpdater(XRSummary summary) : base(summary) {
			if(summary.Func != SummaryFunc.RunningSum)
				throw new ArgumentException("summary");
		}
		public override void Clear() {
			accumulatedInfo.Clear();
		}
		public override bool CanAddBrick {
			get {
				return fSummary.Control.Report.CurrentRowIndex >= accumulatedInfo.Count;
			}
		}
		public override void Update() {
			if(fSummary.Control.Band is DetailBand && fSummary.Bricks.Count == fSummary.ValuesInfo.Count) {
				for(int i = 0; i < fSummary.ValuesInfo.Count; i++) {
					accumulatedInfo.AddUnique(fSummary.ValuesInfo[i]);
					ProcessSummaryBrick(fSummary.Bricks[i], accumulatedInfo.Values);
				}
			} else if(fSummary.Control.Band is DetailBand) {
				int index = 0;
				int rowIndex;
				foreach(VisualBrick brick in fSummary.Bricks) {
					if(!brick.TryExtractAttachedValue(BrickAttachedProperties.RowIndex, out rowIndex))
						continue;
					for(; index < fSummary.ValuesInfo.Count && fSummary.ValuesInfo.RowIndices[0] + index <= rowIndex; index++)
						accumulatedInfo.AddUnique(fSummary.ValuesInfo[index]);
					ProcessSummaryBrick(brick, accumulatedInfo.Values);
				}
			} else {
				accumulatedInfo.AddRangeUnique(fSummary.ValuesInfo);
				ProcessSummaryBricksCore(accumulatedInfo.Values);
			}
		}
		void ProcessSummaryBrick(VisualBrick brick, IList values) {
			object nativeValue = fSummary.GetNativeValue(values);
			RaiseSummaryCalculated(brick, nativeValue);
		}
	}
	abstract class ListContainer<T1, T2> : IEnumerable {
		class Enumerator : IEnumerator {
			ListContainer<T1, T2> list;
			int index;
			object current;
			public Enumerator(ListContainer<T1, T2> list) {
				this.list = list;
				this.index = -1;
			}
			#region IEnumerator Members
			object IEnumerator.Current {
				get {
					if(current != null)
						return current;
					throw new InvalidOperationException(); 
				}
			}
			bool IEnumerator.MoveNext() {
				if(index < list.Count - 1) {
					current = list[++index];
					return true;
				}
				index = list.Count;
				current = null;
				return false;
			}
			void IEnumerator.Reset() {
				this.index = -1;
				this.current = null;
			}
			#endregion
		}
		protected List<T1> first;
		protected List<T2> second;
		protected ListContainer() {
			first = new List<T1>();
			second = new List<T2>();
		}
		public Pair<T1, T2> this[int index] {
			get { return new Pair<T1, T2>(first[index], second[index]); }
		}
		public virtual void Clear() {
			first.Clear();
			first.TrimExcess();
			second.Clear();
			second.TrimExcess();
		}
		public int Count {
			get { return first.Count; }
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}
	}
	class ValuesRowsContainer : ListContainer<object, int> {
		HashSet<int> uniqueIndices = new HashSet<int>();
		public List<object> Values {
			get { return first; }
		}
		public List<int> RowIndices {
			get { return second; }
		}
		public void Add(object value, int rowIndex) {
			this.Values.Add(value);
			this.RowIndices.Add(rowIndex);
			uniqueIndices.Add(rowIndex);
		}
		public void AddRange(ValuesRowsContainer source) {
			foreach(Pair<object, int> item in source)
				Add(item.First, item.Second);
		}
		public void AddRangeUnique(ValuesRowsContainer source) {
			foreach(Pair<object, int> item in source)
				AddUnique(item);
		}
		public void AddUnique(Pair<object, int> item) {
			AddUnique(item.First, item.Second);
		}
		public override void Clear() {
			base.Clear();
			uniqueIndices.Clear();
			uniqueIndices.TrimExcess();
		}
		public void AddUnique(object value, int rowIndex) {
			if(!uniqueIndices.Contains(rowIndex))
				Add(value, rowIndex);
		}
	}
}

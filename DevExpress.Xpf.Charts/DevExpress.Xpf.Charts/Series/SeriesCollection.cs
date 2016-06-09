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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Charts.Localization;
using System.Collections;
namespace DevExpress.Xpf.Charts {
	public class SeriesCollection : ChartElementCollection<Series> {
		bool autoSeriesLocked = true;
		protected override ChartElementChange Change {
			get { return ChartElementChange.ClearDiagramCache | ChartElementChange.UpdateXYDiagram2DItems; }
		}
		protected override bool BatchUpdateSupported { get { return true; } }
		internal void LockAutoSeries() {
			autoSeriesLocked = true;
		}
		internal void UnlockAutoSeries() {
			autoSeriesLocked = false;
		}
		protected override void PerformCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.PerformCollectionChanged(e);
			if (e.NewItems != null) {
				foreach (Series series in e.NewItems) {
					if (series != null && !series.IsAutoSeries) {
						series.UpdateSeriesBinding(true);
						series.Points.ClearUpdatesCache();
					}
				}
			}
		}
		protected override void ClearItems() {
			for (int i = Count - 1; i >= 0; i--)
				if (!autoSeriesLocked || !this[i].IsAutoSeries)
					Remove(this[i]);
		}
		protected override void MoveItem(int oldIndex, int newIndex) {
			if (autoSeriesLocked && this[oldIndex].IsAutoSeries != this[newIndex].IsAutoSeries)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesReordering));
			base.MoveItem(oldIndex, newIndex);
		}
		protected override void RemoveItem(int index) {
			if (autoSeriesLocked && this[index].IsAutoSeries)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesRemoving));
			base.RemoveItem(index);
		}
		protected override void SetItem(int index, Series item) {
			if (autoSeriesLocked && this[index].IsAutoSeries)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesReplacement));
			base.SetItem(index, item);
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newStartingIndex, int oldStartingIndex) {
			if ((action == NotifyCollectionChangedAction.Add && newItems != null && newItems.Count > 1) || (action == NotifyCollectionChangedAction.Remove && oldItems != null && oldItems.Count > 1))
				return new SeriesCollectionBatchUpdateInfo(this, GetOperation(action), GetCollection<ISeries>(oldItems), oldStartingIndex, GetCollection<ISeries>(newItems), newStartingIndex);
			ISeries oldSeries = oldItems != null ? oldItems[0] as ISeries : null;
			ISeries newSeries = newItems != null ? newItems[0] as ISeries : null;
			return new SeriesCollectionUpdateInfo(this, GetOperation(action), oldSeries, oldStartingIndex, newSeries, newStartingIndex);
		}
	}
	public class SeriesObservableCollection : ObservableCollection<Series>, IList<ISeries> {
		#region IEnumerable<ISeries> implementation
		IEnumerator<ISeries> IEnumerable<ISeries>.GetEnumerator() {
			foreach (Series series in this)
				yield return series;
		}
		#endregion
		#region ICollection<ISeries> implementation
		bool ICollection<ISeries>.IsReadOnly { get { return false; } }
		bool ICollection<ISeries>.Contains(ISeries series) {
			return Contains((Series)series);
		}
		void ICollection<ISeries>.Add(ISeries series) {
			Add((Series)series);
		}
		bool ICollection<ISeries>.Remove(ISeries series) {
			return Remove((Series)series);
		}
		void ICollection<ISeries>.CopyTo(ISeries[] array, int arrayIndex) {
			for (int i = 0, j = arrayIndex; i < Count; i++, j++)
				array[j] = this[i];
		}
		#endregion
		#region IList<ISeries> ipmplementation
		ISeries IList<ISeries>.this[int index] {
			get { return this[index]; }
			set { this[index] = (Series)value; }
		}
		int IList<ISeries>.IndexOf(ISeries item) {
			return IndexOf((Series)item);
		}
		void IList<ISeries>.Insert(int index, ISeries series) {
			Insert(index, (Series)series);
		}
		#endregion
	}
}

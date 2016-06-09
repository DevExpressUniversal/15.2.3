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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public class SeriesCollection : ChartCollectionBase, IOwnedElement, ICollection<ISeries> {
		public class Enumerator : IEnumerator<ISeries>, IEnumerator<Series> {
			const int startIndexValue = -1;
			readonly int actualCount;
			readonly IList series;
			int currentIndex;
			public Enumerator(IList series) {
				this.series = series;
				this.currentIndex = startIndexValue;
				this.actualCount = series.Count;
			}
			#region IDisposable
			void IDisposable.Dispose() { }
			#endregion
			#region IEnumerator
			object IEnumerator.Current {
				get {
					return series[currentIndex];
				}
			}
			bool IEnumerator.MoveNext() {
				if (currentIndex < (actualCount - 1)) {
					currentIndex++;
					return true;
				}
				return false;
			}
			void IEnumerator.Reset() {
				currentIndex = startIndexValue;
			}
			#endregion
			#region IEnumerator<Series>
			Series IEnumerator<Series>.Current { get { return (Series)series[currentIndex]; } }
			#endregion
			#region IEnumerator<ISeries>
			ISeries IEnumerator<ISeries>.Current { get { return (ISeries)series[currentIndex]; } }
			#endregion
		}
		string NamePrefix { get { return ChartLocalizer.GetString(ChartStringId.SeriesPrefix); } }
		internal Chart Chart { get { return (base.Owner != null) ? ((DataContainer)base.Owner).Chart : null; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("SeriesCollectionItem")]
#endif
		public Series this[int index] { get { return (Series)List[index]; } }
		public Series this[string name] {
			get {
				foreach (Series series in List)
					if (series.Name == name)
						return series;
				return null;
			}
		}
		internal SeriesCollection(DataContainer owner) : base() {
			Owner = owner;
		}
		#region IEnumerable<ISeries> Members
		IEnumerator<ISeries> IEnumerable<ISeries>.GetEnumerator() {
			return new Enumerator(this.List);
		}
		#endregion
		#region ICollection<ISeries>
		int ICollection<ISeries>.Count {
			get {
				return this.Count;
			}
		}
		void ICollection<ISeries>.Add(ISeries item) {
			this.Add(item as Series);
		}
		void ICollection<ISeries>.Clear() {
			this.Clear();
		}
		bool ICollection<ISeries>.Contains(ISeries item) {
			return this.Contains(item as Series);
		}
		void ICollection<ISeries>.CopyTo(ISeries[] array, int arrayIndex) {
			((ICollection)this).CopyTo(array, arrayIndex);
		}
		bool ICollection<ISeries>.IsReadOnly {
			get {
				return ((IList)this).IsReadOnly;
			}
		}
		bool ICollection<ISeries>.Remove(ISeries item) {
			int index = this.IndexOf(item as Series);
			if (index >= 0) {
				RemoveAt(index);
				return true;
			}
			return false;
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return base.GetEnumerator();
		}
		#endregion
		#region IOwnedElement implementation
		IOwnedElement IOwnedElement.Owner { get { return Owner; } }
		IChartContainer IOwnedElement.ChartContainer { get { return Owner.ChartContainer; } }
		#endregion
		string[] GetNames() {
			string[] names = new string[Count];
			for (int i = 0; i < Count; i++)
				names[i] = this[i].Name;
			return names;
		}
		protected override ICollection GetUpdateInfoSequence(int index) {
			return UpdateHelper.GetUpdateInfoSequence<ISeries>(index);
		}
		protected override void DisposeItemBeforeRemove(ChartElement item) {
			Series series = item as Series;
			if (series == null || (series.Autocreated && !series.Permanent))
				base.DisposeItemBeforeRemove(item);
			item.Owner = null;
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(ChartCollectionOperation operation, object oldItem, int oldIndex, object newItem, int newIndex) {
			return new SeriesCollectionUpdateInfo(this, operation, oldItem as ISeries, oldIndex, newItem as ISeries, newIndex);
		}
		protected override ChartUpdateInfoBase CreateBatchUpdateInfo(ChartCollectionOperation operation, ICollection oldItems, int oldIndex, ICollection newItems, int newIndex) {
			return new SeriesCollectionBatchUpdateInfo(this, operation, oldItems as ICollection<ISeries>, oldIndex, newItems as ICollection<ISeries>, newIndex);
		}
		protected override void OnClear() {
			ChartElement owner = Owner;
			if (owner != null && !owner.Loading)
				foreach (Series series in this)
					series.ClearAnnotations();
			base.OnClear();
		}
		protected override void OnRemove(int index, object value) {
			Series series = value as Series;
			if (series != null)
				series.ClearAnnotations();
			base.OnRemove(index, value);
		}
		protected override void ChangeOwnerForItem(ChartElement item) {
			base.ChangeOwnerForItem(item);
			Series series = item as Series;
			if (series != null) {
				foreach (SeriesPoint point in series.Points)
					point.UpdateAnnotationRepository();
			}
		}
		protected override void CheckSwapAbility(ChartElement element1, ChartElement element2) {
			Series series1 = element1 as Series;
			Series series2 = element2 as Series;
			if (series1 != null && series2 != null && ((series1.Autocreated && !series2.Autocreated) || (!series1.Autocreated && series2.Autocreated)))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgCantSwapSeries));
		}
		internal bool Contains(object obj) {
			foreach (Series series in this)
				if (series.Contains(obj))
					return true;
			return false;
		}
		internal Series GetSeriesByID(int id) {
			return GetElementByID(id) as Series;
		}
		protected internal override void ProcessChanged(ChartUpdateInfoBase changeInfo) {
			SeriesCollectionUpdateInfo seriesCollectionUpdateInfo = changeInfo as SeriesCollectionUpdateInfo;
			bool shouldRiseProcessChanged = true;
			if (seriesCollectionUpdateInfo != null) {
				Series newSeries = seriesCollectionUpdateInfo.NewItem as Series;
				Series oldSeries = seriesCollectionUpdateInfo.OldItem as Series;
				if (((newSeries != null) && (!newSeries.IsAutoCreated)) ||
					((oldSeries != null) && (!oldSeries.IsAutoCreated))) {
					DataContainer dataContainer = GetOwner<DataContainer>();
					if (dataContainer != null)
						shouldRiseProcessChanged = !dataContainer.UpdateBinding(false, true, true, changeInfo);
				}
			}
			if (shouldRiseProcessChanged)
				base.ProcessChanged(changeInfo);
			else
				Owner.ChartContainer.Changed();
		}
		public int Add(Series series) {
			series.Owner = (DataContainer)Owner;
			if (!Chart.Loading)
				series.CheckBinding(series.GetDataSource());
			series.View.CheckOnAddingSeries();
			return base.Add(series);
		}
		public int Add(string name, ViewType viewType) {
			return Add(new Series(name, viewType));
		}
		public void AddRange(Series[] coll) {
			foreach (Series series in coll)
				Add(series);
		}
		public void Remove(Series series) {
			base.Remove(series);
		}
		public void Insert(int index, Series series) {
			base.Insert(index, series);
		}
		public void Swap(Series series1, Series series2) {
			base.Swap(series1, series2);
		}
		public bool Contains(Series series) {
			return base.Contains(series);
		}
		public int IndexOf(Series series) {
			return base.IndexOf(series);
		}
		public new Series[] ToArray() {
			return (Series[])InnerList.ToArray(typeof(Series));
		}
		public string GenerateName() {
			return NameGenerator.UniqueName(NamePrefix, GetNames());
		}
	}
}

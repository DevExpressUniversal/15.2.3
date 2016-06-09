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
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Charts.Localization;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Charts {
	public class SeriesPointCollection : ChartElementCollection<SeriesPoint> {
		bool ReadOnly { get { return Series != null && Series.IsAutoPointsAdded; } }
		Series Series { get { return Owner as Series; } }
		protected override bool ShouldUpdateLogicalTree { get { return false; } }
		protected override bool BatchUpdateSupported { get { return true; } }
		protected override bool Loading {
			get {
				if (Series != null && Series.Diagram != null && Series.Diagram.ChartControl != null)
					return Series.Diagram.ChartControl.Loading;
				return base.Loading;
			}
		}
		protected override ChartElementChange Change { get { return ChartElementChange.ClearDiagramCache; } }
		internal void AddRange(IList<ISeriesPoint> items) {
			if (items.Count > 0) {
				BeginInit();
				int startIndex = this.Count;
				foreach (SeriesPoint item in items)
					Add(item);
				EndInit();				
			}
		}
		protected override void ClearItems() {
			if (ReadOnly)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCreation));
			base.ClearItems();
		}
		protected override void InsertItem(int index, SeriesPoint item) {
			if (ReadOnly)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCreation));
			base.InsertItem(index, item);
		}
		protected override void MoveItem(int oldIndex, int newIndex) {
			if (ReadOnly)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCreation));
			base.MoveItem(oldIndex, newIndex);
		}
		protected override void RemoveItem(int index) {
			if (ReadOnly)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCreation));
			base.RemoveItem(index);
		}
		protected override void SetItem(int index, SeriesPoint item) {
			if (ReadOnly)
				throw new InvalidOperationException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCreation));
			base.SetItem(index, item);
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(NotifyCollectionChangedAction action, IList newItems, IList oldItems, int newStartingIndex, int oldStartingIndex) {
			if ((action == NotifyCollectionChangedAction.Add && newItems != null && newItems.Count > 1) || (action == NotifyCollectionChangedAction.Remove && oldItems != null && oldItems.Count > 1))  
				return new SeriesPointCollectionBatchUpdateInfo(this, GetOperation(action), Series as ISeries, GetCollection<ISeriesPoint>(oldItems), oldStartingIndex, GetCollection<ISeriesPoint>(newItems), newStartingIndex);
			ISeriesPoint oldPoint = oldItems != null && oldItems.Count > 0 ? oldItems[0] as ISeriesPoint : null;
			ISeriesPoint newPoint = newItems != null && newItems.Count > 0 ? newItems[0] as ISeriesPoint : null;
			return new SeriesPointCollectionUpdateInfo(this, GetOperation(action), Series as ISeries, oldPoint, oldStartingIndex, newPoint, newStartingIndex);
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public class SeriesPointCollectionDecorator : IList<ISeriesPoint> {
		class SeriesPointEnumerator : IEnumerator<ISeriesPoint> {
			readonly IList<ISeriesPoint> collection;
			int currentIndex = -1;
			public ISeriesPoint Current { get { return collection[currentIndex]; } }
			public SeriesPointEnumerator(IList<ISeriesPoint> collection) {
				this.collection = collection;
				currentIndex = -1;
			}
			#region IDisposable implementation
			void IDisposable.Dispose() {
				currentIndex = -1;
			}
			#endregion
			#region IEnumerator implementation
			object IEnumerator.Current { get { return Current; } }
			bool IEnumerator.MoveNext() {
				return ++currentIndex < collection.Count;
			}
			void IEnumerator.Reset() {
				currentIndex = -1;
			}
			#endregion
			#region IEnumerator<ISeriesPoint> implementation
			ISeriesPoint IEnumerator<ISeriesPoint>.Current { get { return Current; } }
			#endregion
		}
		readonly SeriesPointCollection collection;
		IEnumerator<ISeriesPoint> IEnumerable<ISeriesPoint>.GetEnumerator() {
			return new SeriesPointEnumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return collection.GetEnumerator();
		}
		bool ICollection<ISeriesPoint>.IsReadOnly { get { return false; } }
		void ICollection<ISeriesPoint>.Add(ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			if (point != null)
				collection.Add(point);
		}
		bool ICollection<ISeriesPoint>.Remove(ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			return point != null && collection.Remove(point);
		}
		int ICollection<ISeriesPoint>.Count { get { return collection.Count; } }
		void ICollection<ISeriesPoint>.Clear() {
			collection.Clear();
		}
		bool ICollection<ISeriesPoint>.Contains(ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			return point != null && collection.Contains(point);
		}
		void ICollection<ISeriesPoint>.CopyTo(ISeriesPoint[] array, int arrayIndex) {
			foreach (ISeriesPoint point in collection)
				array[arrayIndex++] = point;
		}
		ISeriesPoint IList<ISeriesPoint>.this[int index] {
			get { return collection[index]; }
			set {
				SeriesPoint point = value as SeriesPoint;
				if (point != null)
					collection[index] = point;
			}
		}
		void IList<ISeriesPoint>.Insert(int index, ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			if (point != null)
				collection.Insert(index, point);
		}
		void IList<ISeriesPoint>.RemoveAt(int index) {
			collection.RemoveAt(index);
		}
		int IList<ISeriesPoint>.IndexOf(ISeriesPoint item) {
			SeriesPoint point = item as SeriesPoint;
			return point == null ? -1 : collection.IndexOf(point);
		}
		public SeriesPointCollectionDecorator(SeriesPointCollection collection) {
			this.collection = collection;
		}
	}
}

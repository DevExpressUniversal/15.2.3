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
namespace DevExpress.Charts.Native {
	public abstract partial class RefinedPointCollectionBase : IList<RefinedPoint> {
		#region Enumerator
		public class Enumerator : IEnumerator<RefinedPoint>, IEnumerator<IBasePoint> {
			const int startIndexValue = -1;
			readonly int actualCount;
			readonly RefinedPoint[] points;
			int currentIndex;
			public Enumerator(RefinedPoint[] points, int count) {
				this.points = points;
				this.currentIndex = startIndexValue;
				this.actualCount = count;
			}
			#region IDisposable
			void IDisposable.Dispose() { }
			#endregion
			#region IEnumerator
			object IEnumerator.Current {
				get {
					return points[currentIndex];
				}
			}
			bool IEnumerator.MoveNext() {
				if(currentIndex < (actualCount - 1)) {
					currentIndex++;
					return true;
				}
				return false;
			}
			void IEnumerator.Reset() {
				currentIndex = startIndexValue;
			}
			#endregion
			#region IEnumerator<RefinedPoint>
			RefinedPoint IEnumerator<RefinedPoint>.Current { get { return points[currentIndex]; } }
			#endregion
			#region IEnumerator<RefinedPoint>
			IBasePoint IEnumerator<IBasePoint>.Current { get { return points[currentIndex]; } }
			#endregion
		}
		#endregion
		const int minCollapsedCount = 32;
		const int defaultCapacity = 4;
		int count = -1;
		RefinedPoint[] points;
		bool IsReadOnly { get { return true; } }
		protected RefinedPoint[] Points { get { return points; } }
		public int Count {
			get {
				return count;
			}
			protected set {
				count = value;
			}
		}
		public RefinedPointCollectionBase(int capacity) {
			if(capacity < 0) {
				throw new ArgumentOutOfRangeException("Invalid capacity: " + capacity);
			}
			Realloc(capacity);
			this.count = 0;
		}
		#region IEnumerator<RefinedPoint>
		IEnumerator<RefinedPoint> IEnumerable<RefinedPoint>.GetEnumerator() {
			return (IEnumerator<RefinedPoint>)GetEnumerator();
		}
		#endregion
		#region IList<RefinedPoint>
		int IList<RefinedPoint>.IndexOf(RefinedPoint item) {
			return this.IndexOf((RefinedPoint)item);
		}
		void IList<RefinedPoint>.Insert(int index, RefinedPoint item) {
			ChartDebug.Fail();
		}
		void IList<RefinedPoint>.RemoveAt(int index) {
			ChartDebug.Fail();
		}
		RefinedPoint IList<RefinedPoint>.this[int index] {
			get {
				if (index >= count)
					throw new IndexOutOfRangeException();
				return points[index];
			}
			set {
				ChartDebug.Fail();
			}
		}
		#endregion
		#region ICollection<RefinedPoint>
		bool ICollection<RefinedPoint>.IsReadOnly { get { return this.IsReadOnly; } }
		void ICollection<RefinedPoint>.Add(RefinedPoint item) {
			this.Add((RefinedPoint)item);
		}
		void ICollection<RefinedPoint>.Clear() {
			this.Clear();
		}
		bool ICollection<RefinedPoint>.Contains(RefinedPoint item) {
			return this.Contains((RefinedPoint)item);
		}
		void ICollection<RefinedPoint>.CopyTo(RefinedPoint[] array, int arrayIndex) {
			this.CopyTo(array, arrayIndex);
		}
		bool ICollection<RefinedPoint>.Remove(RefinedPoint item) {
			return this.Remove((RefinedPoint)item);
		}
		#endregion
		int GetCapacityForCollapse() {
			int capacity = points.Length >> 1;
			while(capacity > count << 2) {
				capacity >>= 1;
			}
			return Math.Max(minCollapsedCount, capacity);
		}
		int GetCapacityForExpand() {
			int capacity = points.Length << 1;
			while(capacity < count)
				capacity <<= 1;
			return capacity;
		}
		void CollapsePoints() {
			RefinedPoint[] newPoints = new RefinedPoint[GetCapacityForCollapse()];
			Array.Copy(points, 0, newPoints, 0, newPoints.Length);
			points = newPoints;
		}
		void ExpandPoints() {
			RefinedPoint[] newPoints = new RefinedPoint[GetCapacityForExpand()];
			Array.Copy(points, 0, newPoints, 0, points.Length);
			points = newPoints;
		}
		protected void Realloc(int newCount) {
			if(count != newCount) {
				count = newCount;
				if(points == null) {
					int capacity = count < defaultCapacity ? defaultCapacity : count;
					points = new RefinedPoint[capacity];
				}
				else {
					if(points.Length < count)
						ExpandPoints();
					else if((points.Length > count << 2) && (points.Length > minCollapsedCount))
						CollapsePoints();
				}
			}
		}
		protected void MovePoint(int oldIndex, int newIndex) {
			if((oldIndex < 0) || (oldIndex >= count) || (newIndex < 0) || (newIndex >= count))
				throw new ArgumentOutOfRangeException("Invalid index");
			if(oldIndex == newIndex)
				return;
			RefinedPoint point = points[oldIndex];
			if(oldIndex > newIndex) {
				Array.Copy(points, newIndex, points, newIndex + 1, oldIndex - newIndex);
				points[newIndex] = point;
			}
			else {
				Array.Copy(points, oldIndex + 1, points, oldIndex, newIndex - oldIndex);
				points[newIndex] = point;
			}
		}
		public abstract int IndexOf(RefinedPoint item);
		public abstract bool Contains(RefinedPoint item);
		public abstract void Add(RefinedPoint item);
		public abstract void AddRange(ICollection<RefinedPoint> collection);
		public IEnumerator GetEnumerator() {
			return new Enumerator(points, count);
		}
		public RefinedPoint this[int index] {
			get {
				if(index < count) {
					if(points[index] == null)
						points[index] = new RefinedPoint();
					return points[index];
				}
				throw new IndexOutOfRangeException("Invalid index value: " + index.ToString());
			}
			set {
				if(index < count)
					points[index] = value;
				else
					throw new IndexOutOfRangeException("Invalid index value: " + index.ToString());
			}
		}
		public void Clear() {
			Realloc(0);
		}
		public bool Remove(RefinedPoint item) {
			int index = IndexOf(item);
			if(index < 0)
				return false;
			int newCount = count - 1;
			Array.Copy(points, index + 1, points, index, newCount - index);
			Realloc(newCount);
			return true;
		}
		public void CopyTo(IBasePoint[] array, int index) {
			Array.Copy(points, 0, array, index, count);
		}
	}
	public class RefinedPointCollection : RefinedPointCollectionBase {
		public RefinedPointCollection(int capacity) : base(capacity) { }
		public void Swap(int index1, int index2) {
			if((index1 < 0) || (index1 >= Count) || (index2 < 0) || (index2 >= Count))
				throw new ArgumentOutOfRangeException("Invalid index");
			if(index1 == index2)
				return;
			RefinedPoint swap = Points[index1];
			Points[index1] = Points[index2];
			Points[index2] = swap;
		}
		public void RemoveRange(int index, int count) {
			if((index < 0) || (index >= this.Count))
				throw new ArgumentOutOfRangeException("Invalid index: " + index);
			int endIndex = index + count;
			if(endIndex > this.Count)
				throw new ArgumentException();
			if(endIndex != this.Count)
				Array.Copy(Points, endIndex, Points, index, this.Count - endIndex);
			Realloc(this.Count - count);
		}
		public void Move(int oldIndex, int newIndex) {
			MovePoint(oldIndex, newIndex);
		}
		public void Insert(int index, RefinedPoint item) {
			int oldCount = Count;
			Realloc(Count + 1);
			Array.Copy(Points, index, Points, index + 1, oldCount - index);
			Points[index] = item;
		}
		public void InsertRange(int index, ICollection<RefinedPoint> collection) {
			if((index < 0) || (index > Count))
				throw new ArgumentOutOfRangeException("Invalid index value: " + index);
			if(collection == null)
				throw new ArgumentNullException();
			if(collection.Count == 0)
				return;
			int oldCount = Count;
			Realloc(Count + collection.Count);
			Array.Copy(Points, index, Points, index + collection.Count, oldCount - index);
			collection.CopyTo(Points, index);
		}
		public override int IndexOf(RefinedPoint item) {
			for(int i = 0; i < Count; i++)
				if(item == Points[i])
					return i;
			return -1;
		}
		public override bool Contains(RefinedPoint item) {
			return IndexOf(item) >= 0;
		}
		public override void Add(RefinedPoint item) {
			Insert(Count, item);
		}
		public override void AddRange(ICollection<RefinedPoint> collection) {
			InsertRange(Count, collection);
		}
	}
	public class SortedRefinedPointCollection : RefinedPointCollectionBase {
		RefinedPointComparerBase comparer;
		public SortedRefinedPointCollection(int capacity, RefinedPointComparerBase comparer)
			: base(capacity) {
			this.comparer = comparer;
		}
		public int BinarySearch(RefinedPoint item) {
			return Array.BinarySearch(Points, 0, Count, item, comparer);
		}
		public int BinarySearch(RefinedPoint item, Comparer<RefinedPoint> comparer) {
			return Array.BinarySearch(Points, 0, Count, item, comparer);
		}
		public void Sort(RefinedPointComparerBase comparer) {
			SetComparer(comparer);
			Sort();
		}
		public void Sort() {
			Array.Sort(Points, 0, Count, this.comparer);
		}
		public void Initialize(RefinedPointCollectionBase pointsCollection) {
			Realloc(pointsCollection.Count);
			pointsCollection.CopyTo(Points, 0);
			Sort();
		}
		public void Update(RefinedPoint oldItem, RefinedPoint newItem) {
			int oldIndex = IndexOf(oldItem);
			int newIndex = BinarySearch(newItem);
			if(newIndex < 0) {
				newIndex = ~newIndex;
				if(newIndex >= Count)
					newIndex = Count - 1;
			}
			Points[oldIndex] = newItem;
			MovePoint(oldIndex, newIndex);
		}
		public void SetComparer(RefinedPointComparerBase comparer) {
			this.comparer = comparer;
		}
		public override int IndexOf(RefinedPoint item) {
			int index = BinarySearch(item);
			if(index < 0)
				return -1;
			for(int i = index; (i < Count) && (comparer.Compare(Points[index], Points[i]) == 0); i++) {
				if(item == Points[i])
					return i;
			}
			for(int i = index - 1; (i >= 0) && (comparer.Compare(Points[index], Points[i]) == 0); i--) {
				if(item == Points[i])
					return i;
			}
			return -1;
		}
		public override bool Contains(RefinedPoint item) {
			return (BinarySearch(item) >= 0);
		}
		public override void Add(RefinedPoint item) {
			if (!comparer.IsSupportedPoint(item)) {
				Realloc(Count + 1);
				Points[Count-1] = item;
			} else {
				int oldCount = Count;
				int index = BinarySearch(item);
				Realloc(Count + 1);
				if (index < 0)
					index = ~index;
				Array.Copy(Points, index, Points, index + 1, oldCount - index);
				Points[index] = item;
			}
		}
		public override void AddRange(ICollection<RefinedPoint> collection) {
			if(collection == null)
				throw new ArgumentNullException();
			if(collection.Count == 0)
				return;
			foreach(RefinedPoint item in collection)
				Add(item);
		}
	}
}

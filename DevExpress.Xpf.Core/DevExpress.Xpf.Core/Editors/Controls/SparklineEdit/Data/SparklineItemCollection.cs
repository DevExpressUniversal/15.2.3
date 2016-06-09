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

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Editors {
	public enum SparklineSortOrder {
		Ascending = 1,
		Descending = 2,
	}
	public enum SparklineScaleType {
		Unknown,
		Numeric,
		DateTime,
	}
	public class SparklinePointCollection : IList<SparklinePoint>, INotifyCollectionChanged {
		readonly protected List<SparklinePoint> innerList;
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public SparklineScaleType ArgumentScaleType { get { return this.Count > 0 ? this[0].ArgumentScaleType : SparklineScaleType.Unknown; } }
		public SparklineScaleType ValueScaleType { get { return this.Count > 0 ? this[0].ValueScaleType : SparklineScaleType.Unknown; } }
		public int Count {
			get { return innerList.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public SparklinePointCollection() {
			innerList = new List<SparklinePoint>();
		}
		protected void RaiseCollectionChanged() {
			if (CollectionChanged != null)
				CollectionChanged(this, null);
		}
		public SparklinePoint this[int index] {
			get {
				return innerList[index];
			}
			set {
				innerList[index] = value;
				RaiseCollectionChanged();
			}
		}
		public virtual void Add(SparklinePoint item) {
			innerList.Add(item);
			RaiseCollectionChanged();
		}
		public virtual bool Remove(SparklinePoint item) {
			var isRemoved = innerList.Remove(item);
			RaiseCollectionChanged();
			return isRemoved;
		}
		public void Insert(int index, SparklinePoint item) {
			innerList.Insert(index, item);
			RaiseCollectionChanged();
		}
		public void RemoveAt(int index) {
			innerList.RemoveAt(index);
			RaiseCollectionChanged();
		}
		public void Clear() {
			innerList.Clear();
			RaiseCollectionChanged();
		}
		public bool Contains(SparklinePoint item) {
			return innerList.Contains(item);
		}
		public int IndexOf(SparklinePoint item) {
			return innerList.IndexOf(item);
		}
		public void CopyTo(SparklinePoint[] array, int arrayIndex) {
			innerList.CopyTo(array, arrayIndex);
		}
		public IEnumerator<SparklinePoint> GetEnumerator() {
			return innerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
	}
	public class SortedSparklinePointCollection : SparklinePointCollection {
		IComparer<SparklinePoint> comparer;
		public SortedSparklinePointCollection(IComparer<SparklinePoint> comparer) {
			this.comparer = comparer;
		}
		public void Sort(IComparer<SparklinePoint> comparer) {
			this.comparer = comparer;
			innerList.Sort(comparer);
			RaiseCollectionChanged();
		}
		public override void Add(SparklinePoint item) {
			int index = innerList.BinarySearch(item, comparer);
			if (index < 0)
				index = ~index;
			innerList.Insert(index, item);
			RaiseCollectionChanged();
		}
		public void Update(SparklinePoint oldItem, SparklinePoint newItem) {
			innerList.Remove(oldItem);
			Add(newItem);
		}
		public override bool Remove(SparklinePoint item) {
			var isRemoved = innerList.Remove(item);
			RaiseCollectionChanged();
			return isRemoved;
		}
		public void AddRange(IEnumerable<SparklinePoint> collection) {
			innerList.AddRange(collection);
			innerList.Sort(comparer);
			RaiseCollectionChanged();
		}
		public int BinarySearch(SparklinePoint point) {
			return innerList.BinarySearch(point, comparer);
		}
	}
}

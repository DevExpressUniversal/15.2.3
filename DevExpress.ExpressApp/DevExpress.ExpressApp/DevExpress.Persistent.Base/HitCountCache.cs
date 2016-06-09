#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
namespace DevExpress.Persistent.Base {
	public class HitCountCache<T> : ICollection<T>, IComparer<T> {
		private Dictionary<T, long> hitCounts;
		private List<T> list;
		private bool isSorted;
		private void EnsureSorting() {
			if(!isSorted) {
				list.Sort(this);
				isSorted = true;
			}
		}
		public HitCountCache() {
			list = new List<T>();
			hitCounts = new Dictionary<T, long>();
			isSorted = true;
		}
		public void Hit(T item) {
			long old = hitCounts[item];
			if(old != long.MaxValue) {
				hitCounts[item] = old + 1;
				isSorted = false;
			}
		}
		#region ICollection<T> Members
		public void Add(T item) {
			list.Add(item);
			hitCounts.Add(item, 0);
		}
		public void Clear() {
			list.Clear();
			hitCounts.Clear();
			isSorted = true;
		}
		public bool Contains(T item) {
			return hitCounts.ContainsKey(item);
		}
		public void CopyTo(T[] array, int arrayIndex) {
			EnsureSorting();
			list.CopyTo(array, arrayIndex);
		}
		public int Count {
			get { return list.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(T item) {
			hitCounts.Remove(item);
			return list.Remove(item);
		}
		public IEnumerator<T> GetEnumerator() {
			EnsureSorting();
			return list.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			EnsureSorting();
			return list.GetEnumerator();
		}
		#endregion
		#region IComparer<T> Members
		int IComparer<T>.Compare(T x, T y) {
			return hitCounts[y].CompareTo(hitCounts[x]);
		}
		#endregion
	}
}

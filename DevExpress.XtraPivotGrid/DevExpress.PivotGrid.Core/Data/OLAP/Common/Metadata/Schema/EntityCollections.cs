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
namespace DevExpress.PivotGrid.OLAP.SchemaEntities {
	class OlapDimensionCollection : OlapNamedEntityCollection<OlapDimension> {
		public OlapDimensionCollection()
			: base() {
		}
	}
	class OlapMeasureGroupCollection : OlapNamedEntityCollection<OlapMeasureGroup> {
	}
	class OlapLevelCollection : OlapNamedEntityCollection<OlapLevel> {
		readonly OlapHierarchy hierarchy;
		public OlapLevelCollection(OlapHierarchy hierarchy)
			: base() {
			this.hierarchy = hierarchy;
		}
		public OlapHierarchy ParentHierarchy {
			get { return hierarchy; }
		}
	}
	class OlapHierarchyCollection : OlapNamedEntityCollection<OlapHierarchy> {
		readonly OlapDimension dimension;
		public OlapHierarchyCollection(OlapDimension dimension)
			: base() {
			this.dimension = dimension;
		}
		public OlapDimension ParentDimension {
			get { return dimension; }
		}
	}
	abstract class OlapEntityCollection<T> : IOLAPCollection<T> where T : OlapEntityBase {
		List<T> innerList;
		public OlapEntityCollection() { }
		protected List<T> InnerList { get { return innerList ?? (innerList = CreateInnerList(null)); } }
		public int Count { get { return innerList == null ? 0 : innerList.Count; } }
		public T this[int index] { get { return GetItem(index); } }
		List<T> CreateInnerList(IEnumerable<T> items) {
			if(items == null)
				return new List<T>();
			return new List<T>(items);
		}
		protected virtual T GetItem(int index) {
			return InnerList[index];
		}
		public void Add(T item) {
			InnerList.Add(item);
			OnItemAdded(item, InnerList.Count - 1);
		}
		protected virtual void OnItemAdded(T item, int index) {
		}
		#region IEnumerable<T> Members
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return InnerList.GetEnumerator();
		}
		#endregion
	}
	abstract class OlapNamedEntityCollection<T> : OlapEntityCollection<T> where T : OlapEntity {
		public T this[string name] {
			get {
				T res = Find(name);
				if(res == null)
					throw new ArgumentException("element not found");
				return res;
			}
		}
		public T Find(string name) {
			return InnerList.Find((item) => item.Compare(name));
		}
	}
}

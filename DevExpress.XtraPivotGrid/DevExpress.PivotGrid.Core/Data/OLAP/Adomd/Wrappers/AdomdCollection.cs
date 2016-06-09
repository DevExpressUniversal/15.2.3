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
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.OLAP.AdoWrappers {
	public abstract class AdomdCollection<T> : ByObjectWrapper, IOLAPCollection<T> where T : AdomdCollectionItem {
		List<T> innerList;
		protected AdomdCollection(object instance)
			: base(instance) {
			if(!(instance is ICollection) || !(instance is IEnumerable))
				throw new ArgumentException("instance is not a collection");
		}
		protected List<T> InnerList {
			get {
				if(innerList == null)
					innerList = CreateInnerList();
				return innerList;
			}
		}
		protected abstract T CreateT(object instance);
		List<T> CreateInnerList() {
			ICollection list = Instance as ICollection;
			List<T> res;
			if(list != null)
				res = new List<T>(list.Count);
			else
				res = new List<T>();
			foreach(object item in (IEnumerable)Instance)
				res.Add(CreateT(item));
			return res;
		}
		public int Count {
			get {
				if(innerList != null)
					return innerList.Count;
				return (int)GetPropertyValue("Count");
			}
		}
		public T this[int index] {
			get { return InnerList[index]; }
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
	public class AdomdCollectionItem : ByObjectWrapper, IOLAPEntity {
		public AdomdCollectionItem(object instance)
			: base(instance) {
		}
		public virtual string Name {
			get { return (string)GetPropertyValue("Name"); }
		}
	}
}

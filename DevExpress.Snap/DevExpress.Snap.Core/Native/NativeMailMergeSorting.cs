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

using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Snap.Core.API;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Options;
using DevExpress.Snap.Core.Native.Options;
namespace DevExpress.Snap.Core.Native {
	public class NativeMailMergeSorting : SnapListSorting {
		readonly GroupFieldInfoCollection innerList;
		public NativeMailMergeSorting(GroupFieldInfoCollection innerList) {
			Guard.ArgumentNotNull(innerList, "innerList");
			this.innerList = innerList;
		}
		GroupFieldInfo SnapListGroupParamToGroupFieldInfo(SnapListGroupParam value) {
			return new GroupFieldInfo(value.FieldName) { GroupInterval = value.Interval, SortOrder = value.SortOrder };
		}
		SnapListGroupParam GroupFieldInfoToSnapListGroupParam(GroupFieldInfo value) {
			return new SnapListGroupParam(value.FieldName, value.SortOrder, value.GroupInterval);
		}
		public int IndexOf(SnapListGroupParam item) {
			return innerList.IndexOf(SnapListGroupParamToGroupFieldInfo(item));
		}
		public void Insert(int index, SnapListGroupParam item) {
			innerList.Insert(index, SnapListGroupParamToGroupFieldInfo(item));
		}
		public void RemoveAt(int index) {
			innerList.RemoveAt(index);
		}
		public SnapListGroupParam this[int index] {
			get { return GroupFieldInfoToSnapListGroupParam(innerList[index]); }
			set { innerList[index] = SnapListGroupParamToGroupFieldInfo(value); }
		}
		public void Add(SnapListGroupParam item) {
			innerList.Add(SnapListGroupParamToGroupFieldInfo(item));
		}
		public void Clear() {
			innerList.Clear();
		}
		public bool Contains(SnapListGroupParam item) {
			return innerList.Contains(SnapListGroupParamToGroupFieldInfo(item));
		}
		public void CopyTo(SnapListGroupParam[] array, int arrayIndex) {
			int count = innerList.Count;
			for (int i = 0; i < count; i++)
				array[arrayIndex + i] = GroupFieldInfoToSnapListGroupParam(innerList[i]);
		}
		public int Count { get { return innerList.Count; } }
		bool ICollection<SnapListGroupParam>.IsReadOnly { get { return false; } }
		public bool Remove(SnapListGroupParam item) {
			return innerList.Remove(SnapListGroupParamToGroupFieldInfo(item));
		}
		public IEnumerator<SnapListGroupParam> GetEnumerator() {
			for (int i = 0; i < innerList.Count; i++)
				yield return GroupFieldInfoToSnapListGroupParam(innerList[i]);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public void AddRange(IEnumerable<SnapListGroupParam> collection) {
			List<GroupFieldInfo> list = new List<GroupFieldInfo>();
			foreach (SnapListGroupParam item in collection)
				list.Add(SnapListGroupParamToGroupFieldInfo(item));
			this.innerList.AddRange(list);
		}
	}
}

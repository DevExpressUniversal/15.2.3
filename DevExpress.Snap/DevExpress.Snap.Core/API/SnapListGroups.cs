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
using System.Linq;
using System.Text;
namespace DevExpress.Snap.Core.API {
	public interface SnapListGroups : IList<SnapListGroupInfo> {
		SnapListGroupInfo CreateSnapListGroupInfo();
		SnapListGroupInfo CreateSnapListGroupInfo(SnapListGroupParam groupParameter);
		SnapListGroupInfo CreateSnapListGroupInfo(IEnumerable<SnapListGroupParam> groupParameters);
	}
}
namespace DevExpress.Snap.API.Native {
	using DevExpress.Snap.Core.API;
	using DevExpress.Snap.Core.Native.Data;
	using DevExpress.XtraRichEdit.Fields;
	public class NativeSnapListGroups : SnapListGroups {
		#region Fields
		NativeSnapList list;
		List<SnapListGroupInfo> data;
		bool obsolete;
		#endregion
		public NativeSnapListGroups(NativeSnapList parent, FieldDataMemberInfoItem info) {
			this.list = parent;
			data = new List<SnapListGroupInfo>((info != null && info.HasGroups) ? info.Groups.Count : 0);
			if(info != null && info.HasGroups) {
				foreach(GroupProperties record in info.Groups) {
					if(record.HasGroupTemplates)
						data.Add(new NativeSnapListGroupInfo(parent, record));
				}
			}
		}
		void EnsureValid() { if(obsolete) throw new InvalidOperationException(DevExpress.Snap.Localization.SnapLocalizer.GetString(Localization.SnapStringId.SnapListPropertyOutOfDataException)); }
		internal void SetObsolete() { obsolete = true; }
		#region SnapListGroups Members
		public SnapListGroupInfo CreateSnapListGroupInfo() {
			EnsureValid();
			list.EnsureUpdateBegan();
			GroupProperties group = new GroupProperties();
			return new NativeSnapListGroupInfo(list, group);
		}
		public SnapListGroupInfo CreateSnapListGroupInfo(SnapListGroupParam groupParameter) {
			EnsureValid();
			list.EnsureUpdateBegan();
			GroupProperties group = new GroupProperties();
			GroupFieldInfo info = new GroupFieldInfo(groupParameter.FieldName) { SortOrder = groupParameter.SortOrder, GroupInterval = groupParameter.Interval };
			group.GroupFieldInfos.Add(info);
			return new NativeSnapListGroupInfo(list, group);
		}
		public SnapListGroupInfo CreateSnapListGroupInfo(IEnumerable<SnapListGroupParam> groupParameters) {
			EnsureValid();
			list.EnsureUpdateBegan();
			GroupProperties group = new GroupProperties();
			foreach(SnapListGroupParam groupParameter in groupParameters) {
				GroupFieldInfo info = new GroupFieldInfo(groupParameter.FieldName) { SortOrder = groupParameter.SortOrder, GroupInterval = groupParameter.Interval };
				group.GroupFieldInfos.Add(info);
			}
			return new NativeSnapListGroupInfo(list, group);
		}
		#endregion
		#region IList<SnapListGroupInfo> Members
		public int IndexOf(SnapListGroupInfo item) {
			EnsureValid();
			return data.IndexOf(item);
		}
		public void Insert(int index, SnapListGroupInfo item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			data.Insert(index, item);
		}
		public void RemoveAt(int index) {
			EnsureValid();
			list.EnsureUpdateBegan();
			data[index].RemoveFooter();
			data[index].RemoveHeader();
			data[index].RemoveSeparator();
			data.RemoveAt(index);
		}
		public SnapListGroupInfo this[int index] {
			get {
				EnsureValid();
				return data[index];
			}
			set {
				EnsureValid();
				list.EnsureUpdateBegan();
				data[index] = value;
			}
		}
		#endregion
		#region ICollection<SnapListGroupInfo> Members
		public void Add(SnapListGroupInfo item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			data.Add(item);
		}
		public void Clear() {
			EnsureValid();
			list.EnsureUpdateBegan();
			data.Clear();
		}
		public bool Contains(SnapListGroupInfo item) {
			EnsureValid();
			return data.Contains(item);
		}
		public void CopyTo(SnapListGroupInfo[] array, int arrayIndex) {
			EnsureValid();
			data.CopyTo(array, arrayIndex);
		}
		public int Count { get { EnsureValid(); return data.Count; } }
		public bool IsReadOnly { get { return false; } }
		public bool Remove(SnapListGroupInfo item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			return data.Remove(item);
		}
		#endregion
		#region IEnumerable<SnapListGroupInfo> Members
		public IEnumerator<SnapListGroupInfo> GetEnumerator() {
			EnsureValid();
			return data.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			EnsureValid();
			return data.GetEnumerator();
		}
		#endregion
	}
}

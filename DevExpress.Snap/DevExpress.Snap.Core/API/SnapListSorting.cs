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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
namespace DevExpress.Snap.Core.API {
	public interface SnapListSorting : IList<SnapListGroupParam> {
		void AddRange(IEnumerable<SnapListGroupParam> collection);
	}
}
namespace DevExpress.Snap.API.Native {
	using DevExpress.Snap.Core.API;
	using DevExpress.Snap.Core.Native.Data;
	public class NativeSnapListSorting : SnapListSorting {
		NativeSnapList list;
		List<SnapListGroupParam> data;
		bool obsolete;
		public NativeSnapListSorting(NativeSnapList parent, FieldDataMemberInfoItem info) {
			this.list = parent;
			this.data = new List<SnapListGroupParam>();
			if(info != null && info.HasGroups)
				foreach(var group in info.Groups)
					if(!group.HasGroupTemplates)
						foreach(var rec in group.GroupFieldInfos)
							data.Add(new SnapListGroupParam(rec.FieldName, rec.SortOrder, rec.GroupInterval));
		}
		void EnsureValid() { if(obsolete) throw new InvalidOperationException(DevExpress.Snap.Localization.SnapLocalizer.GetString(Localization.SnapStringId.SnapListPropertyOutOfDataException)); }
		internal void SetObsolete() { obsolete = true; }
		#region IList<SnapListGroupParam> Members
		public int IndexOf(SnapListGroupParam item) {
			EnsureValid();
			return data.IndexOf(item);
		}
		public void Insert(int index, SnapListGroupParam item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			data.Insert(index, item);
		}
		public void RemoveAt(int index) {
			EnsureValid();
			list.EnsureUpdateBegan();
			data.RemoveAt(index);
		}
		public SnapListGroupParam this[int index] {
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
		public void AddRange(IEnumerable<SnapListGroupParam> collection) {
			EnsureValid();
			list.EnsureUpdateBegan();
			data.AddRange(collection);
		}
		#endregion
		#region ICollection<SnapListGroupParam> Members
		public void Add(SnapListGroupParam item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			data.Add(item);
		}
		public void Clear() {
			EnsureValid();
			list.EnsureUpdateBegan();
			data.Clear();
		}
		public bool Contains(SnapListGroupParam item) {
			EnsureValid();
			return data.Contains(item);
		}
		public void CopyTo(SnapListGroupParam[] array, int arrayIndex) {
			EnsureValid();
			data.CopyTo(array, arrayIndex);
		}
		public int Count {
			get { EnsureValid(); return data.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(SnapListGroupParam item) {
			EnsureValid();
			list.EnsureUpdateBegan();
			return data.Remove(item);
		}
		#endregion
		#region IEnumerable<SnapListGroupParam> Members
		public IEnumerator<SnapListGroupParam> GetEnumerator() {
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

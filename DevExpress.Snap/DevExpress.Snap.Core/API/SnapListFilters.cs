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

namespace DevExpress.Snap.Core.API {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	public interface SnapListFilters : IList<string> {
		void AddRange(IEnumerable<string> collection);
	}
}
namespace DevExpress.Snap.API.Native {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using DevExpress.Snap.Core.API;
	public class NativeSnapListFilters : SnapListFilters {
		NativeSnapList list;
		List<string> data;
		internal bool Modified { get; set; }
		public NativeSnapListFilters(NativeSnapList parent, List<string> filters) {
			this.list = parent;
			this.data = new List<string>(filters);
		}
		#region IList<string> Members
		public int IndexOf(string item) {
			return data.IndexOf(item);
		}
		public void Insert(int index, string item) {
			list.EnsureUpdateBegan();
			Modified = true;
			data.Insert(index, item);
		}
		public void RemoveAt(int index) {
			list.EnsureUpdateBegan();
			Modified = true;
			data.RemoveAt(index);
		}
		public string this[int index] {
			get {
				return data[index];
			}
			set {
				list.EnsureUpdateBegan();
				Modified = true;
				data[index] = value;
			}
		}
		public void AddRange(IEnumerable<string> collection) {
			list.EnsureUpdateBegan();
			Modified = true;
			data.AddRange(collection);
		}
		#endregion
		#region ICollection<string> Members
		public void Add(string item) {
			list.EnsureUpdateBegan();
			Modified = true;
			data.Add(item);
		}
		public void Clear() {
			list.EnsureUpdateBegan();
			Modified = true;
			data.Clear();
		}
		public bool Contains(string item) {
			return data.Contains(item);
		}
		public void CopyTo(string[] array, int arrayIndex) {
			data.CopyTo(array, arrayIndex);
		}
		public int Count {
			get { return data.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(string item) {
			list.EnsureUpdateBegan();
			Modified = true;
			return data.Remove(item);
		}
		#endregion
		#region IEnumerable<string> Members
		public IEnumerator<string> GetEnumerator() {
			return data.GetEnumerator();
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return data.GetEnumerator();
		}
		#endregion
	}
}

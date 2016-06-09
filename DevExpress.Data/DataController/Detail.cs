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
using System.ComponentModel;
using DevExpress.Data.Helpers;
#if SL
using DevExpress.Xpf.Collections;
#else
using System.Data;
#endif
namespace DevExpress.Data.Details {
	public class MasterRowInfo : CollectionBase {
		int parentListSourceRow;
		DataController parentController;
		object parentRowKey;
		public MasterRowInfo(DataController parentController, int parentListSourceRow, object parentRowKey) {
			this.parentController = parentController;
			this.parentListSourceRow = parentListSourceRow;
			this.parentRowKey = parentRowKey;
		}
		public DetailInfo CreateDetail(IList list, int relationIndex) {
			DetailInfo info = new DetailInfo(this, list, relationIndex);
			InnerList.Insert(0, info);
			return info;
		}
		public object ParentRowKey { get { return parentRowKey; } }
		public DataController ParentController { get { return parentController; } }
		public int ParentControllerRow { get { return ParentController.GetControllerRow(ParentListSourceRow); } }
		public object ParentRow { get { return ParentController.GetRow(ParentControllerRow); } }
		public int ParentListSourceRow { get { return parentListSourceRow; } }
		protected internal void SetParentListSourceRow(int value) {
			this.parentListSourceRow = value;
		}
		public void MakeDetailVisible(int relationIndex) {
			MoveToUp(FindDetail(relationIndex));
		}
		public virtual void MakeDetailVisible(DetailInfo info) {
			MoveToUp(info);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete (index, value);
			DetailInfo info = value as DetailInfo;
			info.Dispose();
		}
		protected override void OnClear() {
			try {
				for(int n = Count - 1; n >= 0; n--) {
					DetailInfo info = this[n];
					InnerList.RemoveAt(n);
					info.Dispose();
				}
			}
			finally {
			}
		}
		public DetailInfo this[int index] { get { return List[index] as DetailInfo; } }
		public void Remove(DetailInfo info) { List.Remove(info); }
		public int IndexOf(DetailInfo info) { return List.IndexOf(info); }
		public DetailInfo FindDetail(object detailOwner) {
			for(int n = 0; n < Count; n++) {
				DetailInfo info = this[n];
				if(info.DetailOwner == detailOwner) return info;
			}
			return null;
		}
		public DetailInfo FindDetail(int relationIndex) {
			if(relationIndex == -1 && Count > 0) return this[0];
			for(int n = 0; n < Count; n++) {
				DetailInfo info = this[n];
				if(info.RelationIndex == relationIndex) return info;
			}
			return null;
		}
		void MoveToUp(DetailInfo info) {
			if(info == null) return;
			int oldIndex = InnerList.IndexOf(info);
			if(oldIndex == 0) return;
			InnerList.RemoveAt(oldIndex);
			InnerList.Insert(0, info);
		}
	}
	public class MasterRowInfoCollection : CollectionBase, IIndexRenumber {
		DataController controller;
		public MasterRowInfoCollection(DataController controller) {
			this.controller = controller;
		}
		protected DataController Controller { get { return controller; } }
		public MasterRowInfo this[int index] { get { return List[index] as MasterRowInfo; } }
		public MasterRowInfo CreateRow(DataController parentController, int parentListSourceRow, object parentRowKey) {
			MasterRowInfo row = new MasterRowInfo(parentController, parentListSourceRow, parentRowKey);
			InnerList.Add(row);
			return row;
		}
		public virtual void Remove(MasterRowInfo row) {
			List.Remove(row);
		}
		public virtual MasterRowInfo Find(int listSourceRow) {
			for(int n = 0; n < Count; n++) {
				MasterRowInfo di = this[n];
				if(di.ParentListSourceRow == listSourceRow) return di;
			}
			return null;
		}
		public MasterRowInfo FindByKey(object rowKey) {
			for(int n = 0; n < Count; n++) {
				MasterRowInfo di = this[n];
				if(Object.Equals(di.ParentRowKey, rowKey)) return di;
			}
			return null;
		}
		public DetailInfo FindOwner(object detailOwner) {
			for(int n = 0; n < Count; n++) {
				MasterRowInfo row = this[n];
				DetailInfo di = row.FindDetail(detailOwner);
				if(di != null) return di;
			}
			return null;
		}
		void RenumberIndexes(int listSourceRow, bool increment) {
			for(int n = Count - 1; n >=0; n --) {
				MasterRowInfo info = this[n];
				int lr = info.ParentListSourceRow;
				if(lr >= listSourceRow) {
					info.SetParentListSourceRow(lr + (increment ? 1 : -1));
				}
			}
		}
		protected internal void OnItemMoved(int oldListSourceRow, int newListSourceRow) {
			MasterRowInfo info = Find(oldListSourceRow);
			IndexRenumber.RenumberIndexes(this, oldListSourceRow, newListSourceRow);
			if(info != null) {
				info.SetParentListSourceRow(newListSourceRow);
			}
		}
		protected internal void OnItemAdded(int listSourceRow) {
			IndexRenumber.RenumberIndexes(this, listSourceRow, true);
		}
		protected internal void OnItemDeleted(int listSourceRow) {
			MasterRowInfo info = Find(listSourceRow);
			if(info != null) {
				info.Clear();
				Remove(info);
			}
			IndexRenumber.RenumberIndexes(this, listSourceRow, false);
		}
		#region IIndexRenumber Members
		int IIndexRenumber.GetCount() { return Count; }
		int IIndexRenumber.GetValue(int pos) { return this[pos].ParentListSourceRow; }
		void IIndexRenumber.SetValue(int pos, int val) { this[pos].SetParentListSourceRow(val); }
		#endregion
	}
	public class DetailInfo : IDisposable {
		MasterRowInfo masterRow;
		IDisposable detailOwner;
		IList detailList;
		int relationIndex;
		public DetailInfo(MasterRowInfo masterRow, IList detailList, int relationIndex) {
			this.detailOwner = null;
			this.detailList = detailList;
			this.relationIndex = relationIndex;
			this.masterRow = masterRow;
		}
		public MasterRowInfo MasterRow { get { return masterRow; } }
		public IDisposable DetailOwner { get { return detailOwner; } set { detailOwner = value; } }
		public IList DetailList { get { return detailList; } }
		public int RelationIndex { get { return relationIndex; } }
		public virtual void Dispose() {
			if(DetailOwner != null) DetailOwner.Dispose();
			this.detailList = null;
			this.detailOwner = null;
			this.masterRow = null;
		}
	}
}

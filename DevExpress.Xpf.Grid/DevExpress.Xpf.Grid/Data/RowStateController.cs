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
using System.Windows;
using DevExpress.Data;
using DevExpress.Data.Selection;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid;
using System.Collections.Generic;
using DevExpress.Data.Helpers;
namespace DevExpress.Xpf.Data {
	public class CollapsedRowsKeeper : SelectedRowsKeeper {
		protected RowStateController RowStateController { get { return Controller.Selection as RowStateController; } }
		protected virtual RowStateCollection RowStateCollection { get { return RowStateController.CollapsedRows; } }
		public CollapsedRowsKeeper(DataController controller, bool allowKeepSelection) : base(controller, allowKeepSelection) { }
		protected override void RestoreCore(object row, int level, object value) {
			RowStateCollection.SetRowSelected(GetRowHandleByObject(row), true, value);
		}
		internal int GetRowHandleByObject(object row) {
			if(row is GroupRowInfo) {
#if DEBUGTEST
				throw new InvalidOperationException();
#else
				return DataController.InvalidRow;
#endif
			}
			return Controller.GetControllerRow((int)row);
		}
		public void SaveRow(int listSourceRow) {
			int controllerRow = Controller.GetControllerRow(listSourceRow);
			SaveRowCore(controllerRow, RowStateCollection.GetRowSelectedObject(controllerRow));
		}
		public void SaveCore() {
			if(RowStateCollection.Count == 0) return;
			int[] res = RowStateCollection.CopyToArray();
			for(int i = 0; i < res.Length; i++)
				SaveRow(res[i]);
		}
		public virtual void RestoreCore(bool clear) {
			if(Helper.Count > 0 && !IsEmpty) {
				for(int i = 0; i < Helper.Count; i++) {
					object key = GetRowKey(i);
					Restore(key, i);
				}
			}
			if(!clear) Clear();
		}
	}
	public class DetailInfoKeeper : CollapsedRowsKeeper {
		public DetailInfoKeeper(DataController controller, bool allowKeepSelection) : base(controller, allowKeepSelection) { }
		protected override RowStateCollection RowStateCollection { get { return RowStateController.DetailInfoCollection; } }
		List<object> cache = new List<object>();
		protected override void SaveRowCore(int controllerRow, object selectedObject) {
			base.SaveRowCore(controllerRow, selectedObject);
			if(selectedObject == null) return;
			cache.Add(selectedObject);
		}
		protected override void RestoreCore(object row, int level, object value) {
			base.RestoreCore(row, level, value);
			if(RowStateCollection.GetRowSelected(GetRowHandleByObject(row))) {
				cache.Remove(value);
			}
		}
		public override void RestoreCore(bool clear) {
			base.RestoreCore(clear);
			foreach(RowDetailContainer container in cache) {
				container.RemoveFromDetailClones();
				container.Detach();
			}
			cache.Clear();
		}
	}
	public class RowStateKeeper : ListSourceRowsKeeper {
		DetailInfoKeeper detailInfoHash;
		CollapsedRowsKeeper collapsedRowsHash;
		RowStateController RowStateController { get { return Controller.Selection as RowStateController; } }
		DetailInfoCollection DetailInfoCollection { get { return RowStateController.DetailInfoCollection; } }
		public RowStateKeeper(DataController dataController, SelectedRowsKeeper selectedRowsKeeper) : base(dataController, selectedRowsKeeper) {
			detailInfoHash = new DetailInfoKeeper(dataController, true);
			collapsedRowsHash = new CollapsedRowsKeeper(dataController, true);
		}
		public override void Save() {
			base.Save();
			if(detailInfoHash.IsEmpty)
				detailInfoHash.SaveCore();
			if(collapsedRowsHash.IsEmpty)
				collapsedRowsHash.SaveCore();
		}
		protected override bool RestoreCore(bool clear) {
			detailInfoHash.RestoreCore(clear);
			collapsedRowsHash.RestoreCore(clear);
			return base.RestoreCore(clear);
		}
		public override void Clear() {
			detailInfoHash.Clear();
			collapsedRowsHash.Clear();
			base.Clear();
		}
	}
	public class RowStateController : SelectionController {
		static T GetRowInfo<T>(RowStateCollection collection, int controllerRow, Func<T> createInfoDelegate, bool createNewIfNotExist = true) where T : class {
			T state = collection.GetRowSelectedObject(controllerRow) as T;
			if((state == null) && createNewIfNotExist) {
				state = createInfoDelegate();
				collection.SetRowSelected(controllerRow, true, state);
			}
			return state;
		}
		readonly RowStateCollection collapsedRows;
		public RowStateCollection CollapsedRows { get { return collapsedRows; } }
		readonly DetailInfoCollection detailInfoCollection;
		public DetailInfoCollection DetailInfoCollection { get { return detailInfoCollection; } }
		public RowStateController(DataController controller)
			: base(controller) {
			this.collapsedRows = new RowStateCollection(this);
			this.detailInfoCollection = new DetailInfoCollection(this);
		}
		public DependencyObject GetRowState(int controllerRow, bool createNewIfNotExist) {
			return GetRowInfo<DependencyObject>(collapsedRows, controllerRow, () => new RowStateObject(), createNewIfNotExist);
		}
		public RowDetailContainer GetRowDetailInfo(int controllerRow, Func<RowDetailContainer> createContainerDelegate, bool createNewIfNotExist = true) {
			return GetRowInfo<RowDetailContainer>(detailInfoCollection, controllerRow, createContainerDelegate, createNewIfNotExist);
		}
		static readonly int[] emptyIndices = new int[0];
		public IEnumerable<int> GetRowListIndicesWithExpandedDetails() {
			return detailInfoCollection.GetRowListIndicesWithExpandedDetails();
		}
		protected override List<DevExpress.Data.Selection.SelectedRowsCollection> CreateSelectionCollections() {
			List<DevExpress.Data.Selection.SelectedRowsCollection> list = base.CreateSelectionCollections();
			list.Add(collapsedRows);
			list.Add(detailInfoCollection);
			return list;
		}
		internal void ClearSelection() {
			base.Clear();
		}
		internal void SelectAllRows() {
			BeginSelection();
			try {
				ClearSelection();
				for(int n = 0; n < Controller.VisibleCount; n++) {
					SetSelected(Controller.GetControllerRowHandle(n), true);
				}
			}
			finally {
				EndSelection();
			}
		}
		public override void Clear() {
			base.Clear();
			detailInfoCollection.Clear();
			collapsedRows.Clear();
		}
		internal void ClearDetailInfo() {
			foreach(RowDetailContainer detailContainer in detailInfoCollection.GetContainers()) {
				detailContainer.Clear();
			}
			detailInfoCollection.Clear();
		}
	}
}

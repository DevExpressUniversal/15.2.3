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
using DevExpress.Data;
using DevExpress.Data.Selection;
using DevExpress.Xpf.Core;
using DevExpress.Data.Helpers;
using DevExpress.Data.Linq;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Data {
	public class StateServerModeDataController : ServerModeDataController {
		protected readonly IDataProviderOwner owner;
		public StateServerModeDataController(IDataProviderOwner owner) {
			this.owner = owner;
		}
		IListServerRefreshable ListServerRefreshable { get { return DataSource as IListServerRefreshable; } }
		protected override void OnDataSourceChanged() {
			base.OnDataSourceChanged();
			if(ListServerRefreshable != null)
				ListServerRefreshable.Refresh += OnListServerRefresh;
		}
		void OnListServerRefresh(object sender, EventArgs e) {
			VisualClient.UpdateLayout();
		}
		public override void Dispose() {
			if(ListServerRefreshable != null)
				ListServerRefreshable.Refresh -= OnListServerRefresh;
			base.Dispose();
		}
		protected override SelectionController CreateSelectionController() { return new RowStateController(this); }
		protected override bool UseFirstRowTypeWhenPopulatingColumns(Type rowType) {
			return rowType.FullName == ListDataControllerHelper.UseFirstRowTypeWhenPopulatingColumnsTypeName;
		}
		public override void CancelCurrentRowEdit() {
			base.CancelCurrentRowEdit();
			owner.RaiseCurrentRowCanceled(new ControllerRowEventArgs(CurrentControllerRow, GetRow(CurrentControllerRow)));
		}
		protected override bool CanSortColumnCore(DataColumnInfo column) {
			if(ListSourceEx2 == null) return base.CanSortColumnCore(column);
			return ListSourceEx2.CanSort;
		}
		protected override void OnDataSync_FilterSortGroupInfoChanged(object sender, CollectionViewFilterSortGroupInfoChangedEventArgs e) {
			int oldColumnCount = Columns.Count;
			base.OnDataSync_FilterSortGroupInfoChanged(sender, e);
			if(DataSync is ICollectionViewHelper && Columns.Count > oldColumnCount)
				UpdateTotalSummary();
		}
	}
	public class CollectionViewDataController : StateServerModeDataController {
		public bool IsSynchronizedWithCurrentItem { get { return owner.IsSynchronizedWithCurrentItem; } }
		public CollectionViewDataController(IDataProviderOwner owner) : base(owner) { }
		protected override SelectedRowsKeeper CreateSelectionKeeper() {
			return new CollectionViewCurrentAndSelectedRowsKeeper(this, true);
		}
	}
	public class CollectionViewCurrentAndSelectedRowsKeeper : ServerModeCurrentAndSelectedRowsKeeper {
		CollectionViewDataController CollectionViewDataController { get { return (CollectionViewDataController)Controller; } }
		public CollectionViewCurrentAndSelectedRowsKeeper(CollectionViewDataController controller, bool allowKeepSelection) : base(controller, allowKeepSelection) { }
		protected override void RestoreCurrentRow() {
			if(CollectionViewDataController.IsSynchronizedWithCurrentItem) {
				ICollectionViewHelper collectionViewHelper = (ICollectionViewHelper)Controller.ListSource;
				bool isDataRowFocused = Controller.CurrentControllerRow != CollectionViewDataController.NewItemRow && !Controller.IsGroupRowHandle(Controller.CurrentControllerRow);
				if(isDataRowFocused)
					Controller.CurrentControllerRow = Controller.FindRowByRowValue(collectionViewHelper.Collection.CurrentItem);
			}
			else {
				base.RestoreCurrentRow();
			}
		}
	}
}

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
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.Windows.Forms;
using DevExpress.Data.Details;
#endif
#if SL && DEBUGTEST
using DevExpress.Data.Test;
#endif 
namespace DevExpress.Data {
	public abstract class BaseListSourceDataController : DataController {
		public const int FilterRow = InvalidRow + 2;
		public const int NewItemRow = InvalidRow + 1;
		protected bool newItemRowEditing = false;
		public bool IsNewItemRowEditing { get { return newItemRowEditing; } }
		public abstract void AddNewRow();
		public override object GetRow(int controllerRow, OperationCompleted completed) {
			if(controllerRow == NewItemRow) 
				return Helper.GetNewRow();
			return base.GetRow(controllerRow, completed);
		}
		protected override void Reset() {
			this.newItemRowEditing = false;
			base.Reset();
		}
		protected override object GetRowValueDetail(int controllerRow, DataColumnInfo column) {
			if(column != null && controllerRow == NewItemRow) {
				return Helper.GetNewRowDetailValue(column);
			}
			return base.GetRowValueDetail(controllerRow, column);
		}
		public override object GetRowValue(int controllerRow, int column, OperationCompleted completed) {
			if(controllerRow == NewItemRow) {
				return Helper.GetNewRowValue(column);
			}
			return base.GetRowValue(controllerRow, column, completed);
		}
		protected override void SetRowValueCore(int controllerRow, int column, object val) {
			if(controllerRow == NewItemRow) {
				Helper.SetNewRowValue(column, val);
				return;
			}
			base.SetRowValueCore(controllerRow, column, val);
		}
		protected IEditableObject GetEditableObject(int controllerRow) {
			return GetRow(controllerRow) as IEditableObject;
		}
		protected void BeginRowEdit(int controllerRow) {
			IEditableObject editable = GetEditableObject(controllerRow);
			if(editable != null) editable.BeginEdit();
			IEditableCollectionView view = Helper.List as IEditableCollectionView;
			if(view != null)
				view.EditItem(GetRow(controllerRow));
		}
		protected void CancelRowEdit(int controllerRow) {
			IEditableObject editable = GetEditableObject(controllerRow);
			if(editable != null) editable.CancelEdit();
			IEditableCollectionView view = Helper.List as IEditableCollectionView;
			if(view != null && view.IsEditingItem) {
				if(view.CanCancelEdit)
					view.CancelEdit();
				else
					view.CommitEdit();
			}
			if(view != null && view.IsAddingNew)
				view.CancelNew();
		}
		protected void EndRowEdit(int controllerRow) {
			IEditableObject editable = GetEditableObject(controllerRow);
			if(editable != null) editable.EndEdit();
			IEditableCollectionView view = Helper.List as IEditableCollectionView;
			if(view != null && !view.IsAddingNew)
				view.CommitEdit();
		}
		public override bool IsGroupRowHandle(int controllerRowHandle) {
			if(controllerRowHandle == NewItemRow) return false;
			return base.IsGroupRowHandle(controllerRowHandle);
		}
		protected internal override void OnStartNewItemRow() { 
			this.newItemRowEditing = true;
		}
		protected internal override void OnEndNewItemRow() { 
			this.newItemRowEditing = false;
		}
		public virtual void SetDataSource(object dataSource) {
#if !SL || DEBUGTEST
			IList list = MasterDetailHelper.GetDataSource(null, dataSource, string.Empty);
#else
			IList list = (IList)dataSource;
#endif
			SetListSource(list);
		}
	}
	public class ListSourceDataController : BaseListSourceDataController {
		public new IList ListSource {
			get { return base.ListSource; }
			set {
				SetListSource(value);
			}
		}
#if (!SL && !DXPORTABLE) || DEBUGTEST
		public void SetListSource(BindingContext context, object dataSource, string dataMember) {
			SetListSource(MasterDetailHelper.GetDataSource(context, dataSource, dataMember));
		}
#endif
		public virtual int EndNewRowEdit() {
			if(!IsNewItemRowEditing) return InvalidRow;
			EndRowEdit(NewItemRow);
			this.newItemRowEditing = false;
			Helper.RaiseOnEndNewItemRow();
			return VisibleCount - 1;
		}
		public virtual void CancelNewRowEdit() {
			if(!IsNewItemRowEditing) return;
			try {
				CancelRowEdit(NewItemRow);
			} finally {
				this.newItemRowEditing = false;
			}
			Helper.RaiseOnEndNewItemRow();
		}
		public override void AddNewRow() {
			if(!IsReady || !AllowNew || IsSorted || IsFiltered) return;
			try {
				if(IsNewItemRowEditing) EndNewRowEdit();
				if(IsNewItemRowEditing) return;
				this.newItemRowEditing = true;
				Helper.SetDetachedListSourceRow(VisibleCount);
				Helper.AddNewRow();
				Helper.RaiseOnStartNewItemRow();
			}
			catch {
				this.newItemRowEditing = false;
				Helper.SetDetachedListSourceRow(DataController.InvalidRow);
				throw;
			}
		}
	}
}

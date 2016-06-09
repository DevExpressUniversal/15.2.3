#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.ExpressApp.Win.Core {
	public interface ISupportNewItemRow {
		bool IsNewItemRowCancelling { get; }
		object NewItemRowObject { get; }
		event EventHandler CancelNewRow;
	}
	public class XtraGridUtils {
		public static bool HasValidRowHandle(ColumnView view) {
			return ((view.GridControl.DataSource != null) && (view.FocusedRowHandle >= 0) && (view.RowCount > 0));
		}
		public static void SelectFocusedRow(ColumnView view) {
			SelectRowByHandle(view, view.FocusedRowHandle);
		}
		public static void SelectRowByHandle(ColumnView view, int rowHandle) {
			SelectRowByHandle(view, rowHandle, true);
		}
		public static void SelectRowByHandle(ColumnView view, int rowHandle, bool isFocused) {
			if(rowHandle != GridControl.InvalidRowHandle && view.GridControl != null) {
				view.BeginSelection();
				try {
					view.ClearSelection();
					view.SelectRow(rowHandle);
					if(isFocused) {
						view.FocusedRowHandle = rowHandle;
					}
				} finally {
					view.EndSelection();
				}
			}
		}
		public static object GetFocusedRowObject(ColumnView view) {
			return GetRow(view, view.FocusedRowHandle);
		}
		public static object GetNearestRowObject(ColumnView view) {
			object result = GetRow(view, view.FocusedRowHandle + 1);
			if (result == null) {
				result = GetRow(view, view.FocusedRowHandle - 1);
			}
			return result;
		}
		public static object GetRow(ColumnView view, int rowHandle) {
			return GetRow(null, view, rowHandle);
		}
		public static bool IsRowSelected(ColumnView view, int rowHandle) {
			int[] selected = view.GetSelectedRows();
			for(int i = 0; (selected != null) && (i < selected.Length - 1); i++) {
				if(selected[i] == rowHandle) {
					return true;
				}
			}
			return false;
		}
		public static Object GetRow(CollectionSourceBase collectionSource, ColumnView view, Int32 rowHandle) {
			if(
				(!view.IsDataRow(rowHandle) && !view.IsNewItemRow(rowHandle))
				||
				(view.GridControl == null)
				||
				(view.GridControl.DataSource == null)
				||
				((view.GridControl.MainView == view) && (view.DataSource != view.GridControl.DataSource) && !(view.GridControl.DataSource is IListSource)))
			{
				return null;
			}
			if((collectionSource is CollectionSource) && ((CollectionSource)collectionSource).IsServerMode && ((CollectionSource)collectionSource).IsAsyncServerMode) {
				if(!view.IsRowLoaded(rowHandle)) {
					return null;
				}
				String keyPropertyName = "";
				if(collectionSource.ObjectTypeInfo.KeyMember != null) {
					keyPropertyName = collectionSource.ObjectTypeInfo.KeyMember.Name;
				}
				if(!String.IsNullOrEmpty(keyPropertyName)) {
					Object objectKey = view.GetRowCellValue(rowHandle, keyPropertyName);
					return collectionSource.ObjectSpace.GetObjectByKey(collectionSource.ObjectTypeInfo.Type, objectKey);
				}
			}
			object result = view.GetRow(rowHandle);
			if(result == null && view is ISupportNewItemRow && ((ISupportNewItemRow)view).IsNewItemRowCancelling) {
				result = ((ISupportNewItemRow)view).NewItemRowObject;
			}
			return result;
		}
		public static Object GetFocusedRowObject(CollectionSourceBase collectionSource, ColumnView view) {
			return GetRow(collectionSource, view, view.FocusedRowHandle);
		}
		public static ErrorType GetErrorTypeByName(string errorName) {
			switch(errorName) {
				case "Error": return ErrorType.Critical;
				case "Information": return ErrorType.Information;
				case "Warning": return ErrorType.Warning;
				default: return ErrorType.Critical;
			}
		}
	}
}

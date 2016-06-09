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
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data {
	public interface IDataColumnInfo {
		List<IDataColumnInfo> Columns { get; }
		string UnboundExpression { get; }
		string Caption { get; }
		string FieldName { get; }
		string Name { get; }
		Type FieldType { get; }
		DataControllerBase Controller { get; }
	}
	public interface IListWrapper {
		Type WrappedListType { get; }
	}
	public interface IBoundControl {
		bool IsHandleCreated { get; }
		IAsyncResult BeginInvoke(Delegate method, params object[] arguments);
		bool InvokeRequired { get; }
	}
	public interface IDataControllerValidationSupport {
		IBoundControl BoundControl { get; }
		void OnStartNewItemRow();
		void OnEndNewItemRow();
		void OnBeginCurrentRowEdit();
		void OnCurrentRowUpdated(ControllerRowEventArgs e);
		void OnValidatingCurrentRow(ValidateControllerRowEventArgs e);
		void OnPostRowException(ControllerRowExceptionEventArgs e);
		void OnPostCellException(ControllerRowCellExceptionEventArgs e);
		void OnControllerItemChanged(ListChangedEventArgs e);
	}
	public class RowDeletedEventArgs : EventArgs {
		int rowHandle, listSourceIndex;
		object row;
		public RowDeletedEventArgs(int rowHandle, int listIndex, object row) {
			this.row = row;
			this.rowHandle = rowHandle;
			this.listSourceIndex = listIndex;
		}
		public int RowHandle { get { return rowHandle; } }
		public int ListSourceIndex { get { return listSourceIndex; } }
		public object Row { get { return row; } }
	}
	public class RowDeletingEventArgs : RowDeletedEventArgs {
		bool cancel;
		public RowDeletingEventArgs(int rowHandle, int listIndex, object row) : base(rowHandle, listIndex, row) { }
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public delegate void RowDeletedEventHandler(object sender, RowDeletedEventArgs e);
	public delegate void RowDeletingEventHandler(object sender, RowDeletingEventArgs e);
	public class ControllerRowEventArgs : EventArgs {
		int rowHandle;
		object row;
		public ControllerRowEventArgs(int rowHandle, object row) {
			this.row = row;
			this.rowHandle = rowHandle;
		}
		public int RowHandle { get { return rowHandle; } }
		public object Row { get { return row; } }
	}
	public class ValidateControllerRowEventArgs : ControllerRowEventArgs {
		bool valid;
		string errorText;
		public ValidateControllerRowEventArgs(int rowHandle, object row)
			: base(rowHandle, row) {
			this.valid = true;
			this.errorText = "";
		}
		public bool Valid {
			get { return valid; }
			set { valid = value; }
		}
		public string ErrorText {
			get { return errorText; }
			set { errorText = value; }
		}
	}
	public delegate void ValidateControllerRowEventHandler(object sender, ValidateControllerRowEventArgs e);
	public enum ExceptionAction { CancelAction, RetryAction }
	public class ControllerRowExceptionEventArgs : ControllerRowEventArgs {
		Exception exception;
		ExceptionAction action;
		public ControllerRowExceptionEventArgs(int controllerRow, object row, Exception exception)
			: base(controllerRow, row) {
			this.action = ExceptionAction.RetryAction;
			this.exception = exception;
		}
		public ExceptionAction Action { get { return action; } set { action = value; } }
		public Exception Exception { get { return exception; } }
	}
	public class ControllerRowCellExceptionEventArgs : ControllerRowExceptionEventArgs {
		int column;
		public ControllerRowCellExceptionEventArgs(int controllerRow, int column, object row, Exception exception)
			: base(controllerRow, row, exception) {
			this.column = column;
		}
		public int Column { get { return column; } }
	}
	public interface IDataControllerCurrentSupport {
		void OnCurrentControllerRowChanged(CurrentRowEventArgs e);
		void OnCurrentControllerRowObjectChanged(CurrentRowChangedEventArgs e);
	}
	public class CurrentRowEventArgs : EventArgs {
	}
	public class CurrentRowChangedEventArgs : EventArgs {
		object currentRow, previousRow;
		public CurrentRowChangedEventArgs(object previousRow, object currentRow) {
			this.currentRow = currentRow;
			this.previousRow = previousRow;
		}
		public object CurrentRow { get { return currentRow; } }
		public object PreviousRow { get { return previousRow; } }
	}
	public class SubstituteSortInfoEventArgs: EventArgs {
		public DataColumnSortInfo[] SortInfo { get; set; }
		public int GroupCount { get; internal set; }
		public SubstituteSortInfoEventArgs() { }
	}
	public interface IDataControllerSort {
		bool RequireDisplayText(DataColumnInfo column);
		string GetDisplayText(int listSourceRow, DataColumnInfo info, object value, string columnName);
		void BeforeSorting();
		void AfterSorting();
		void BeforeGrouping();
		void AfterGrouping();
		string[] GetFindByPropertyNames();
		bool? IsEqualGroupValues(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn);
		ExpressiveSortInfo.Row GetCompareRowsMethodInfo();
		ExpressiveSortInfo.Cell GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order);
		ExpressiveSortInfo.Cell GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType);
		void SubstituteSortInfo(SubstituteSortInfoEventArgs args);
	}
	public interface IDataControllerData {
		object GetUnboundData(int listSourceRow1, DataColumnInfo column, object value);
		void SetUnboundData(int listSourceRow1, DataColumnInfo column, object value);
		UnboundColumnInfoCollection GetUnboundColumns();
	}
	public class SubstituteFilterEventArgs: EventArgs {
		public CriteriaOperator Filter { get; set; }
		public SubstituteFilterEventArgs() { }
		public SubstituteFilterEventArgs(CriteriaOperator filter)
			: this() {
				this.Filter = filter;
		}
	}
	public interface IDataControllerData2 : IDataControllerData {
		void SubstituteFilter(SubstituteFilterEventArgs args);
		bool? IsRowFit(int listSourceRow, bool fit);
		bool HasUserFilter { get; }
		PropertyDescriptorCollection PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection);
		bool CanUseFastProperties { get; }
		ComplexColumnInfoCollection GetComplexColumns();
	}
}

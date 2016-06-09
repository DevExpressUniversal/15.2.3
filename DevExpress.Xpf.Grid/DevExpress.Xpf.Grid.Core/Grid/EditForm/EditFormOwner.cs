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

using DevExpress.Xpf.Editors.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Grid.EditForm {
	public interface IEditFormOwner {
		IEnumerable<EditFormColumnSource> CreateEditFormColumnSource();
		object GetValue(EditFormCellData data);
		void SetValue(EditFormCellData data);
		void OnInlineFormClosed();
		int ColumnCount { get; }
		bool ShowUpdateCancelButtons { get; }
		BaseValidationError Validate(EditFormCellData data);
		EditFormPostMode EditMode { get; }
		object Source { get; }
	}
	public class EditFormOwner : IEditFormOwner {
		readonly ITableView view;
		const string CaptionSeparator = ":";
		int editFormRowHandle;
		DataControlBase Grid { get { return DataView.DataControl; } }
		DataViewBase DataView { get { return view.ViewBase; } }
		public EditFormOwner(ITableView view) {
			if(view == null)
				throw new ArgumentNullException("view");
			this.view = view;
			editFormRowHandle = DataView.FocusedRowHandle;
		}
		public virtual IEnumerable<EditFormColumnSource> CreateEditFormColumnSource() {
			var columns = Grid.ColumnsCore.Cast<ColumnBase>();
			Func<ColumnBase, int> visibleIndexSelector = CreateVisibleIndexSelector(columns.Any(x => x.EditFormVisibleIndex != 0));
			return columns.Select(x => CreateEditFormColumnSource(x, visibleIndexSelector));
		}
		static EditFormColumnSource CreateEditFormColumnSource(ColumnBase column, Func<ColumnBase, int> visibleIndexSelector) {
			return new EditFormColumnSource() {
				FieldName = column.FieldName,
				EditSettings = column.ActualEditSettings,
				Caption = CalcCaption(column),
				ColumnSpan = column.EditFormColumnSpan,
				RowSpan = column.EditFormRowSpan,
				StartNewRow = column.EditFormStartNewRow,
				Visible = CalcVisible(column),
				VisibleIndex = visibleIndexSelector(column),
				EditorTemplate = column.EditFormTemplate,
				ReadOnly = column.ReadOnly
			};
		}
		static object CalcCaption(ColumnBase column) {
			if(column.EditFormCaption != null)
				return column.EditFormCaption;
			return column.HeaderCaption != null ? column.HeaderCaption.ToString() + CaptionSeparator : null;
		}
		static bool CalcVisible(ColumnBase column) {
			return column.EditFormVisible.HasValue ? column.EditFormVisible.Value : column.Visible;
		}
		static Func<ColumnBase, int> CreateVisibleIndexSelector(bool useEditFormIndexes) {
			if(useEditFormIndexes)
				return x => x.EditFormVisibleIndex;
			else
				return x => x.VisibleIndex;
		}
		public object GetValue(EditFormCellData data) {
			return Grid.GetCellValue(editFormRowHandle, data.FieldName);
		}
		public void SetValue(EditFormCellData data) {
			try {
				Grid.SetCellValueCore(editFormRowHandle, data.FieldName, data.Value);
			} catch(Exception e) {
				data.ValidationError = RowValidationHelper.CreateEditorValidationError(DataView, editFormRowHandle, e, GetColumn(data));
			}
		}
		public int ColumnCount { get { return view.EditFormColumnCount; } }
		public bool ShowUpdateCancelButtons { get { return view.ShowEditFormUpdateCancelButtons; } }
		public EditFormPostMode EditMode { get { return view.EditFormPostMode; } }
		public object Source { get { return DataView.GetRowData(editFormRowHandle); } }
		public BaseValidationError Validate(EditFormCellData data) {
			ColumnBase column = GetColumn(data);
			if(column == null)
				return null;
			BaseValidationError error = null;
			if(data.HasInnerError)
				error = RowValidationHelper.CreateEditorValidationError(DataView, editFormRowHandle, null, column);
			if(error != null)
				return error;
			error = RowValidationHelper.ValidateAttributes(DataView, data.Value, editFormRowHandle, column);
			if(error != null)
				return error;
			return RowValidationHelper.ValidateEvents(DataView, data, data.Value, editFormRowHandle, column);
		}
		ColumnBase GetColumn(EditFormCellData data) {
			return Grid.ColumnsCore[data.FieldName];
		}
		public void OnInlineFormClosed() {
			DataView.EditFormManager.OnInlineFormClosed();
		}
	}
	public class EmptyEditFormOwner : IEditFormOwner {
		static EmptyEditFormOwner instance;
		static EmptyEditFormOwner() { }
		public static EmptyEditFormOwner Instance {
			get {
				if(instance == null) {
					instance = new EmptyEditFormOwner();
				}
				return instance;
			}
		}
		public int ColumnCount { get { return 0; } }
		public bool ShowUpdateCancelButtons { get { return false; } }
		public EditFormPostMode EditMode { get { return EditFormPostMode.Cached; } }
		public object Source { get { return null; } }
		public IEnumerable<EditFormColumnSource> CreateEditFormColumnSource() {
			return Enumerable.Empty<EditFormColumnSource>();
		}
		public object GetValue(EditFormCellData data) {
			return null;
		}
		public void SetValue(EditFormCellData data) { }
		public BaseValidationError Validate(EditFormCellData data) {
			return null;
		}
		public void OnInlineFormClosed() { }
	}
}

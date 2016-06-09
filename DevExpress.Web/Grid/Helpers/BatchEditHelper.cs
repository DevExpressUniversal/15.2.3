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

using DevExpress.Web.Data;
using DevExpress.Web.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Internal {
	public abstract class GridBatchEditHelper {
		public const string
			ClientStateJSKey = "ClientState",
			ClientEditStateJSKey = "EditState",
			InsertValuesClientKey = "insertedRowValues",
			UpdateValuesClientKey = "modifiedRowValues",
			DeleteValuesClientKey = "deletedRowKeys",
			NewRowInitValuesKey = "NIV",
			ColorPlaceholder = "||dxcolor||";
		public GridBatchEditHelper(ASPxGridBase grid) {
			Grid = grid;
			SavedClientState = new Dictionary<object, object>();
			ClientInsertState = new Dictionary<int, Dictionary<string, string>>();
			ClientUpdateState = new Dictionary<string, Dictionary<string, string>>();
			ClientDeleteState = new List<string>();
			NewRowInitValues = new Dictionary<string, object>();
			ClientInsertedRowValues = new Dictionary<int, Dictionary<string, object>>();
			ClientUpdatedRowValues = new Dictionary<int, Dictionary<string, object>>();
			ClientDeletedRowIndices = new List<int>();
			InsertRowValidationState = new Dictionary<int, bool>();
			UpdateRowValidationState = new Dictionary<int, bool>();
			InsertedRowIndices = new List<int>();
			UpdatedRowKeys = new List<object>();
			DeletedRowKeys = new List<object>();
			EditorValidationErrors = new Dictionary<string, Dictionary<IWebGridDataColumn, string>>();
			RowValidationErrors = new Dictionary<string, string>();
			CallbackErrorData = new Hashtable();
		}
		public ASPxGridBase Grid { get; protected set; }
		public GridRenderHelper RenderHelper { get { return Grid.RenderHelper; } }
		public GridColumnHelper ColumnHelper { get { return Grid.ColumnHelper; } }
		public virtual WebDataProxy DataProxy { get { return Grid.DataProxy; } }
		public ReadOnlyCollection<IWebGridDataColumn> EditColumns { get { return ColumnHelper.EditColumns; } }
		public ASPxGridDataSecuritySettings SettingsDataSecurity { get { return Grid.SettingsDataSecurity; } }
		public virtual bool AllowInsert { get { return SettingsDataSecurity.AllowInsert; } }
		public virtual bool AllowEdit { get { return SettingsDataSecurity.AllowEdit; } }
		public virtual bool AllowDelete { get { return SettingsDataSecurity.AllowDelete; } }
		public Dictionary<object, object> SavedClientState { get; private set; }
		public Dictionary<int, Dictionary<string, string>> ClientInsertState { get; set; }
		public Dictionary<string, Dictionary<string, string>> ClientUpdateState { get; set; }
		public List<string> ClientDeleteState { get; set; }
		public Dictionary<string, object> NewRowInitValues { get; private set; }
		public Dictionary<int, Dictionary<string, object>> ClientInsertedRowValues { get; private set; }
		public Dictionary<int, Dictionary<string, object>> ClientUpdatedRowValues { get; private set; }
		public List<int> ClientDeletedRowIndices { get; private set; }
		public Dictionary<int, bool> InsertRowValidationState { get; private set; }
		public Dictionary<int, bool> UpdateRowValidationState { get; private set; }
		public List<int> InsertedRowIndices { get; private set; }
		public List<object> UpdatedRowKeys { get; private set; }
		public List<object> DeletedRowKeys { get; private set; }
		public Dictionary<string, Dictionary<IWebGridDataColumn, string>> EditorValidationErrors { get; private set; }
		public Dictionary<string, string> RowValidationErrors { get; private set; }
		public Hashtable CallbackErrorData { get; private set; }
		public object GetCheckColumnsDisplayHtml() {
			return GetColumnsDisplayHtml<CheckBoxProperties>((column, prop) => {
				var list = new ArrayList {
					new []{ prop.ValueUnchecked, GetCheckEditDisplayControlHtml(column, prop, prop.ValueUnchecked) },
					new []{ prop.ValueChecked, GetCheckEditDisplayControlHtml(column, prop, prop.ValueChecked) }
				};
				if(prop.AllowGrayed)
					list.Add(new []{ prop.ValueGrayed, GetCheckEditDisplayControlHtml(column, prop, prop.ValueGrayed) });
				return list;
			});
		}
		public object GetColorEditColumnsDisplayHtml() {
			return GetColumnsDisplayHtml<ColorEditProperties>(GetColorEditDisplayControlHtml);
		}
		public object GetBinaryImageColumnsDisplayHtml() {
			return GetColumnsDisplayHtml<BinaryImageEditProperties>((column, prop) => {
				var args = new CreateDisplayControlArgs(null, column.Adapter.DataType, null, null, 
					Grid.ImagesEditors, Grid.StylesEditors, Grid, Grid.DummyNamingContainer, Grid.DesignMode, prop.EncodeHtml);
				return RenderUtils.GetRenderResult(prop.CreateDisplayControl(args));
			});
		}
		public object GetColumnsDisplayHtml<T>(Func<IWebGridDataColumn, T, object> getHtml)  where T : EditPropertiesBase {
			var result = new Hashtable();
			foreach(var column in EditColumns) {
				var prop = RenderHelper.GetColumnEdit(column) as T;
				if(prop != null)
					result[ColumnHelper.GetColumnGlobalIndex(column)] = getHtml(column, prop);
			}
			return result;
		}
		protected virtual object GetCheckEditDisplayControlHtml(IWebGridDataColumn column, CheckBoxProperties prop, object value) {
			var args = GetCheckColumnDisplayControlArgs(column, prop, value);
			var displayControl = prop.CreateDisplayControl(args);
			Grid.DummyNamingContainer.Controls.Add(displayControl);
			return RenderUtils.GetRenderResult(displayControl);
		}
		protected virtual CreateDisplayControlArgs GetCheckColumnDisplayControlArgs(IWebGridDataColumn column, CheckBoxProperties prop, object value) {
			var provider = new CheckEditColumnSimpleValueProvider(column.FieldName, value);
			return new CreateDisplayControlArgs(value, column.Adapter.DataType, null, provider, Grid.ImagesEditors, Grid.StylesEditors, Grid, Grid.DummyNamingContainer, Grid.DesignMode, prop.EncodeHtml);
		}
		protected virtual object GetColorEditDisplayControlHtml(IWebGridDataColumn column, ColorEditProperties prop) {
			var args = new CreateDisplayControlArgs(null, column.Adapter.DataType, ColorPlaceholder, null, Grid.ImagesEditors, Grid.StylesEditors, Grid, Grid.DummyNamingContainer, Grid.DesignMode, prop.EncodeHtml);
			return RenderUtils.GetRenderResult(prop.CreateDisplayControl(args));
		}
		public bool CommitTransaction() {
			ValidateRows();
			if(BatchUpdate())
				return true;
			InsertRows();
			UpdateRows();
			DeleteRows();
			return InsertedRowIndices.Count > 0 || UpdatedRowKeys.Count > 0 || DeletedRowKeys.Count > 0;
		}
		protected virtual void ValidateRows() {
			ValidateAutoCreatedEditors();
			ValidateInsertedRows();
			ValidateUpdatedRows();
		}
		protected virtual bool BatchUpdate() {
			var args = new ASPxDataBatchUpdateEventArgs();
			if(AllowInsert) {
				foreach(var index in InsertRowValidationState.Where(p => p.Value).Select(p => p.Key))
					args.InsertValues.Add(DataProxy.CreateDataInsertValues(index));
			}
			if(AllowEdit) {
				foreach(var index in UpdateRowValidationState.Where(p => p.Value).Select(p => p.Key))
					args.UpdateValues.Add(DataProxy.CreateDataUpdateValues(index, null));
			}
			if(AllowDelete) {
				foreach(var index in ClientDeletedRowIndices) {
					if(CanDeleteRow(index))
						args.DeleteValues.Add(DataProxy.CreateDataDeleteValues(index));
				}
			}
			DataProxy.OnBatchUpdate(args);
			if(args.Handled) {
				InsertedRowIndices.AddRange(args.InsertValues.Select(v => v.RowIndex));
				UpdatedRowKeys.AddRange(args.UpdateValues.Select(v => v.RowKey));
				DeletedRowKeys.AddRange(args.DeleteValues.Select(v => v.RowKey));
			}
			return args.Handled;
		}
		protected virtual void ValidateAutoCreatedEditors() {
		}
		protected virtual void ValidateInsertedRows() {
			if(!AllowInsert) return;
			foreach(var visibleIndex in ClientInsertedRowValues.Keys.OrderByDescending(i => i))
				SafeExecuteAction(ValidateInsertedRow, visibleIndex);
		}
		protected virtual void ValidateUpdatedRows() {
			if(!AllowEdit) return;
			foreach(var visibleIndex in ClientUpdatedRowValues.Keys.OrderBy(i => i))
				SafeExecuteAction(ValidateUpdatedRow, visibleIndex);
		}
		protected virtual void ValidateInsertedRow(int visibleIndex) {
			InsertRowValidationState[visibleIndex] = DataProxy.ValidateRowCore(visibleIndex, false, true);
		}
		protected virtual void ValidateUpdatedRow(int visibleIndex) {
			UpdateRowValidationState[visibleIndex] = DataProxy.ValidateRowCore(visibleIndex, false, false);
		}
		protected virtual void InsertRows() {
			if(!AllowInsert) return;
			var indices = InsertRowValidationState.Where(p => p.Value).Select(p => p.Key);
			foreach(var index in indices) {
				SafeExecuteAction(DataProxy.InsertRow, index);
				InsertedRowIndices.Add(index);
			}
		}
		protected virtual void UpdateRows() {
			if(!AllowEdit) return;
			var indices = UpdateRowValidationState.Where(p => p.Value).Select(p => p.Key);
			foreach(var index in indices) {
				var key = DataProxy.GetRowKeyValue(index);
				SafeExecuteAction(DataProxy.UpdateRow, index);
				UpdatedRowKeys.Add(key);
			}
		}
		protected virtual void DeleteRows() {
			if(!AllowDelete) return;
			var deleteRowValues = new List<ASPxDataDeleteValues>();
			foreach(var visibleIndex in ClientDeletedRowIndices) {
				SafeExecuteAction(() => {
					deleteRowValues.Add(DataProxy.CreateDataDeleteValues(visibleIndex));
				}, visibleIndex);
			}
			foreach(var deleteValues in deleteRowValues) {
				SafeExecuteAction(() => { DataProxy.DeleteRowCore(deleteValues); }, deleteValues.RowKey);
				DeletedRowKeys.Add(deleteValues.RowKey);
			}
		}
		protected virtual bool CanDeleteRow(int visibleIndex) {
			if(DataProxy.GetRowType(visibleIndex) != WebRowType.Data)
				return false;
			return visibleIndex >= 0 && visibleIndex < DataProxy.VisibleRowCount;
		}
		protected virtual void GenerateCallbackErrorData(object rowKey) {
			CallbackErrorData["rowKey"] = rowKey is string ? rowKey : DataProxy.GetKeyValueForScript(rowKey);
			CallbackErrorData["updateInfo"] = GetUpdateInfo();
			CallbackErrorData["validationInfo"] = GetClientValidationInfo();
		}
		protected virtual void GenerateCallbackErrorData(int visibleIndex) {
			CallbackErrorData["rowKey"] = GetKeyValueForScript(visibleIndex);
			CallbackErrorData["updateInfo"] = GetUpdateInfo();
			CallbackErrorData["validationInfo"] = GetClientValidationInfo();
		}
		public virtual Hashtable GetUpdateInfo() {
			var result = new Hashtable();
			if(InsertedRowIndices.Count > 0)
				result["inserted"] = InsertedRowIndices;
			if(UpdatedRowKeys.Count > 0)
				result["updated"] = UpdatedRowKeys.Select(k => DataProxy.GetKeyValueForScript(k)).ToArray();
			if(DeletedRowKeys.Count > 0)
				result["deleted"] = DeletedRowKeys.Select(k => DataProxy.GetKeyValueForScript(k)).ToArray();
			return result;
		}
		public virtual object GetClientValidationInfo() {
			if(EditorValidationErrors.Count == 0 && RowValidationErrors.Count == 0)
				return null;
			var result = new Hashtable();
			var rowKeys = EditorValidationErrors.Keys.Concat(RowValidationErrors.Keys).Distinct();
			foreach(var keyString in rowKeys) {
				var rowError = RowValidationErrors.ContainsKey(keyString) ? RowValidationErrors[keyString] : null;
				var editorErrors = GenerateEditorErrorsClientInfo(keyString);
				if(rowError == null && editorErrors == null)
					continue;
				var rowInfo = new Dictionary<string, object>(2);
				rowInfo["row"] = rowError;
				rowInfo["editors"] = editorErrors;
				result[keyString] = rowInfo;
			}
			return result;
		}
		protected virtual object GenerateEditorErrorsClientInfo(string keyString) {
			if(!EditorValidationErrors.ContainsKey(keyString))
				return null;
			var editorErrors = EditorValidationErrors[keyString];
			return editorErrors.ToDictionary(pair => ColumnHelper.GetColumnGlobalIndex(pair.Key), pair => pair.Value);
		}
		public void SetNewRowInitValues(OrderedDictionary values) {
			foreach(DictionaryEntry pair in values)
				NewRowInitValues[pair.Key.ToString()] = pair.Value;
		}
		public object GetNewRowInitValue(string fieldName) {
			if(NewRowInitValues.ContainsKey(fieldName))
				return NewRowInitValues[fieldName];
			return null;
		}
		public virtual void CreateEditor(IWebGridDataColumn column, WebControl container) {
			if(HasEditItemTemplate(column))
				CreateTemplateEditor(column, container);
			else
				CreateGridEditor(column, container);
		}
		protected virtual bool HasEditItemTemplate(IWebGridDataColumn column) { 
			return false;
		}
		public abstract void CreateTemplateEditor(IWebGridDataColumn column, WebControl container); 
		public virtual void CreateGridEditor(IWebGridDataColumn column, WebControl container) { 
			var prop = RenderHelper.GetColumnEdit(column) as TextEditProperties;
			bool initialDisplayFormatMode = false;
			if(prop != null) {
				initialDisplayFormatMode = prop.DisplayFormatInEditMode;
				prop.DisplayFormatInEditMode = true;
			}
			var editBase = RenderHelper.CreateGridEditor(column, null, EditorInplaceMode.Inplace, false);
			if(prop != null) {
				prop.DisplayFormatInEditMode = initialDisplayFormatMode;
			}
			container.Controls.Add(editBase);
			ApplyEditorSettings(editBase, column);
			Grid.RaiseEditorInitialize(Grid.CreateCellEditorInitializeEventArgs(column, -1, editBase, null, null));
			var edit = editBase as ASPxEdit;
			if(edit != null)
				edit.ForceUseValueChangedClientEvent();
			var comboBox = edit as ASPxComboBox;
			if(comboBox != null && comboBox.LoadDropDownOnDemand)
				comboBox.LoadDropDownOnDemand = false;
		}
		protected virtual void ApplyEditorSettings(ASPxEditBase editBase, IWebGridDataColumn column) {
			editBase.ReadOnly = Grid.IsReadOnly(column);
		}
		public virtual object GetCurrentPageValues() {
			var result = new Hashtable();
			var columns = EditColumns.GroupBy(c => c.FieldName).Select(g => g.First()).ToList();
			var visibleEndIndex = DataProxy.VisibleStartIndex + DataProxy.VisibleRowCountOnPage - 1;
			for(var i = DataProxy.VisibleStartIndex; i <= visibleEndIndex; i++) {
				if(DataProxy.GetRowType(i) != WebRowType.Data)
					continue;
				var key = GetKeyValueForScript(i);
				var rowValues = new Dictionary<int, object>();
				foreach(var column in columns)
					rowValues.Add(ColumnHelper.GetColumnGlobalIndex(column), GetRowValueForJSON(i, column));
				result.Add(key, rowValues);
			}
			var newRowValues = new Dictionary<int, object>();
			foreach(var column in columns) {
				var fieldName = column.FieldName;
				var value = NewRowInitValues.ContainsKey(fieldName) ? NewRowInitValues[fieldName] : null;
				newRowValues.Add(ColumnHelper.GetColumnGlobalIndex(column), value);
			}
			result.Add(NewRowInitValuesKey, newRowValues); 
			return result;
		}
		object GetRowValueForJSON(int index, IWebGridDataColumn column) {
			var rowValue = DataProxy.GetRowValue(index, column.FieldName);
			if(column is GridViewDataBinaryImageColumn) {
				var binaryImage = (ASPxBinaryImage)RenderHelper.CreateGridEditor(column as GridViewDataColumn, null, EditorInplaceMode.Inplace, false);
				binaryImage.Value = rowValue;
				rowValue = binaryImage.ServerValueKey;
			} else if(rowValue is Color) {
				rowValue = ColorUtils.ToHexColor((Color) rowValue);
			}
			return rowValue;
		}
		public void LoadValidationErrors(ASPxGridDataValidationEventArgs e) {
			var keyString = GetKeyValueForScript(e.VisibleIndex);
			if(!string.IsNullOrEmpty(e.RowError))
				RowValidationErrors[keyString] = e.RowError;
			if(e.ErrorsInternal.Count == 0)
				return;
			var editorErrors = new Dictionary<IWebGridDataColumn, string>();
			foreach(var pair in e.ErrorsInternal) {
				var column = pair.Key as IWebGridDataColumn;
				if(column != null)
					editorErrors.Add(column, pair.Value);
			}
			if(editorErrors.Count > 0)
				EditorValidationErrors[keyString] = editorErrors;
		}
		public void ParseEditorValues() {
			ParseInsertEditorValues();
			ParseUpdateEditorValues();
			ParseDeleteKeys();
		}
		protected virtual void ParseInsertEditorValues() {
			foreach(var pair in ClientInsertState) {
				var visibleIndex = pair.Key;
				ClientInsertedRowValues[pair.Key] = ParseRowClientValues(pair.Value, visibleIndex);
			}
		}
		protected virtual void ParseUpdateEditorValues() {
			foreach(var pair in ClientUpdateState) {
				var key = DataProxy.GetKeyValueFromScript(pair.Key);
				if(key == null) continue;
				var visibleIndex = DataProxy.FindVisibleIndexByKey(key, false);
				if(visibleIndex < 0) continue;
				ClientUpdatedRowValues[visibleIndex] = ParseRowClientValues(pair.Value, visibleIndex);
			}
		}
		protected virtual void ParseDeleteKeys() {
			foreach(var keyString in ClientDeleteState) {
				var key = DataProxy.GetKeyValueFromScript(keyString);
				if(key == null) continue;
				var visibleIndex = DataProxy.FindVisibleIndexByKey(key, false);
				if(visibleIndex < 0) continue;
				ClientDeletedRowIndices.Add(visibleIndex);
			}
		}
		protected virtual Dictionary<string, object> ParseRowClientValues(IDictionary clientValues, int visibleIndex) {
			var result = new Dictionary<string, object>();
			try {
				foreach(DictionaryEntry cellPair in clientValues) {
					object parsedValue;
					var fieldName = cellPair.Key as string;
					object cellValue = GetCellValue(fieldName, cellPair);
					if(string.IsNullOrEmpty(fieldName) ||
					   !DataProxy.ParseEditorValue(visibleIndex, fieldName, cellValue, false, out parsedValue))
						continue;
					result[fieldName] = parsedValue;
				}
			} catch(Exception e) {
				GenerateCallbackErrorData(visibleIndex);
				throw e;
			}
			return result;
		}
		object GetCellValue(string fieldName, DictionaryEntry cellPair) {
			var binaryImageColumn = Grid.Columns[fieldName] as GridViewDataBinaryImageColumn;
			if(binaryImageColumn == null)
				return cellPair.Value;
			var dummyEditor = new ASPxBinaryImage {
				AllowEdit = true,
				BinaryStorageMode = binaryImageColumn.PropertiesBinaryImage.BinaryStorageMode
			};
			BinaryStorageData data = BinaryStorage.GetResourceData(dummyEditor,
				binaryImageColumn.PropertiesBinaryImage.BinaryStorageMode, cellPair.Value as string);
			return data == null ? null : data.Content;
		}
		public void LoadClientState(Hashtable state) {
			if(state == null) return;
			LoadClientState(state, SavedClientState, ClientInsertState, ClientUpdateState, ClientDeleteState);
		}
		protected virtual string GetKeyValueForScript(int visibleIndex) {
			if(visibleIndex < 0)
				return visibleIndex.ToString(); 
			return DataProxy.GetKeyValueForScript(visibleIndex);
		}
		protected virtual void SafeExecuteAction(Action<int> action, int visibleIndex) {
			SafeExecuteAction(() => { action(visibleIndex); }, visibleIndex);
		}
		protected virtual void SafeExecuteAction(Action action, object rowKey) {
			try {
				action();
			}
			catch(Exception e) {
				GenerateCallbackErrorData(rowKey);
				throw e;
			}
		}
		public static void LoadClientState(Hashtable state, Dictionary<object, object> savedClientState, Dictionary<int, Dictionary<string, string>> insertState, Dictionary<string, Dictionary<string, string>> updateState, List<string> deleteState) {
			if(state == null) return;
			LoadSavedClientState(savedClientState, state[ClientStateJSKey] as IDictionary);
			LoadClientEditState(state[ClientEditStateJSKey] as IDictionary, insertState, updateState, deleteState);
		}
		protected static void LoadSavedClientState(Dictionary<object, object> target, IDictionary source) {
			if(source == null || target == null) return;
			target.Clear();
			foreach(DictionaryEntry pair in source)
				target.Add(pair.Key, pair.Value);
		}
		protected static void LoadClientEditState(IDictionary editState, Dictionary<int, Dictionary<string, string>> insertState, Dictionary<string, Dictionary<string, string>> updateState, List<string> deleteState) {
			if(editState == null) return;
			LoadInsertState(editState[InsertValuesClientKey] as IDictionary, insertState);
			LoadUpdateState(editState[UpdateValuesClientKey] as IDictionary, updateState);
			LoadDeleteState(editState[DeleteValuesClientKey] as IList, deleteState);
		}
		protected static void LoadInsertState(IDictionary source, Dictionary<int, Dictionary<string, string>> target) {
			if(source == null || target == null) return;
			foreach(DictionaryEntry pair in source) {
				var row = GetRowState(pair.Value as IDictionary);
				if(row != null)
					target[Convert.ToInt32(pair.Key)] = row;
			}
		}
		protected static void LoadUpdateState(IDictionary source, Dictionary<string, Dictionary<string, string>> target) {
			if(source == null || target == null) return;
			foreach(DictionaryEntry pair in source) {
				var keyString = pair.Key.ToString();
				var row = GetRowState(pair.Value as IDictionary);
				if(row != null && !string.IsNullOrEmpty(keyString))
					target[keyString] = row;
			}
		}
		protected static void LoadDeleteState(IList source, List<string> target) {
			if(source == null || target == null) return;
			foreach(var item in source)
				target.Add(item.ToString());
		}
		protected static Dictionary<string, string> GetRowState(IDictionary state) {
			if(state == null || state.Count == 0) return null;
			var result = new Dictionary<string, string>();
			foreach(DictionaryEntry pair in state)
				result.Add(pair.Key.ToString(), pair.Value != null ? pair.Value.ToString() : null);
			return result;
		}
		protected static IDictionary DeserializeState(string stateString) {
			try {
				return HtmlConvertor.FromJSON(stateString) as IDictionary;
			} catch {
			}
			return null;
		}
	}
	class CheckEditColumnSimpleValueProvider : IValueProvider {
		public CheckEditColumnSimpleValueProvider(string fieldName, object value) {
			FieldName = fieldName;
			Value = value;
		}
		protected string FieldName { get; private set; }
		protected object Value { get; private set; }
		public object GetValue(string fieldName) {
			if(FieldName == fieldName)
				return Value;
			return null;
		}
	}
}

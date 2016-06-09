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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Web;
using DevExpress.Web.Data;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public interface IGridObjectSpaceCreator {
		IObjectSpace CreateObjectSpace(Type type);
	}
	public interface IGridBatchOperationProvider : IGridObjectSpaceCreator, ISupportClientValidation {
		object GetObject(object key);
		object CreateNewObject();
		IModelColumn GetModelColumn(GridViewDataColumn column);
		void CommitChanges();
		void Validate(ClientValidateObjectEventArgs args);
		void CustomizeAppearance(CustomizeAppearanceEventArgs args);
	}
	public class ASPxGridBatchModeHelper {
		internal static List<Type> EditorBatchEnabled = new List<Type>();
		private IGridBatchOperationProvider gridBatchOperationProvider;
		private ASPxGridView grid;
		private ITypeInfo objectTypeInfo;
		private IObjectSpace validationObjectSpace;
		private List<string> readOnlyColumns = new List<string>();
		public string ProtectedContentText { get; set; }
		static ASPxGridBatchModeHelper() {
			RegisterColumnBatch();
		}
		public ASPxGridBatchModeHelper(IGridBatchOperationProvider gridBatchOperationProvider, ITypeInfo objectTypeInfo) {
			this.gridBatchOperationProvider = gridBatchOperationProvider;
			this.objectTypeInfo = objectTypeInfo;
			ProtectedContentText = EditorsFactory.ProtectedContentDefaultText;
		}
		public void Attach(ASPxGridView grid) {
			this.grid = grid;
			SetupGrid();
			grid.InitNewRow += Grid_InitNewRow;
			grid.CellEditorInitialize += grid_CellEditorInitialize;
			grid.BatchUpdate += Grid_BatchUpdate;
			grid.RowValidating += grid_RowValidating;
			grid.CommandButtonInitialize += grid_CommandButtonInitialize;
			grid.CustomJSProperties += grid_CustomJSProperties;
			grid.HtmlRowPrepared += grid_HtmlRowPrepared;
		}
		public void Detach() {
			if(grid != null) {
				grid.InitNewRow -= Grid_InitNewRow;
				grid.CellEditorInitialize -= grid_CellEditorInitialize;
				grid.BatchUpdate -= Grid_BatchUpdate;
				grid.RowValidating -= grid_RowValidating;
				grid.CommandButtonInitialize -= grid_CommandButtonInitialize;
				grid.CustomJSProperties -= grid_CustomJSProperties;
				grid.HtmlRowPrepared -= grid_HtmlRowPrepared;
				grid = null;
			}
		}
		public static bool CheckEnabled(ASPxGridView grid, bool allowEdit) {
			if(!ASPxGridListEditor.UseASPxGridViewDataSpecificColumns || !allowEdit) {
				grid.SettingsEditing.Mode = GridViewEditingMode.Inline;
				return false;
			}
			return true;
		}
		protected void SetupGrid() {
			grid.SettingsEditing.BatchEditSettings.AllowValidationOnEndEdit = false;
			grid.SettingsEditing.BatchEditSettings.StartEditAction = GridViewBatchStartEditAction.Click;
			grid.SettingsEditing.BatchEditSettings.EditMode = GridViewBatchEditMode.Cell;
			grid.ClientSideEvents.BatchEditStartEditing = GetBatchEditStartEditingScript();
			grid.JSProperties["cpBatchEditMode"] = true;
			ASPxImageHelper.SetImageProperties(grid.SettingsCommandButton.CancelButton.Image, "Action_Cancel");
			ASPxImageHelper.SetImageProperties(grid.SettingsCommandButton.UpdateButton.Image, "Action_Save");
			readOnlyColumns.Clear();
		}
		protected virtual void ValidateRowObject(object obj, ASPxDataValidationEventArgs e) {
			ClientValidateObjectEventArgs args = new ClientValidateObjectEventArgs(obj, true, validationObjectSpace);
			gridBatchOperationProvider.Validate(args);
			if(!args.Valid) {
				e.RowError = args.ErrorText.Replace(Environment.NewLine, "<br>");
				foreach(var propertyError in args.PropertyErrors) {
					var column = grid.Columns[propertyError.Key];
					if(column != null) {
						e.Errors.Add(column, propertyError.Value);
					}
				}
			}
		}
		protected object GetRowObject(ASPxDataValidationEventArgs e) {
			object obj = null;
			if(e.IsNewRow) {
				obj = validationObjectSpace.CreateObject(objectTypeInfo.Type);
			}
			else {
				obj = validationObjectSpace.GetObjectByKey(objectTypeInfo.Type, e.Keys[0]);
				if(obj == null) {
					throw new ArgumentOutOfRangeException("Key=" + e.Keys[0]);
				}
			}
			UpdateObjectValues(obj, e.NewValues, e.OldValues);
			return obj;
		}
		protected List<string> GetAppearanceDisabledCells() {
			List<string> readOnlyCells = new List<string>();
			int startIndex = grid.VisibleStartIndex;
			int endIndex = grid.VisibleStartIndex + grid.VisibleRowCount;
			for(int i = startIndex; i < endIndex; i++) {
				object rowObject = grid.GetRow(i);
				if(rowObject == null) continue;
				foreach(var column in grid.VisibleColumns) {
					GridViewDataColumn dataColumn = column as GridViewDataColumn;
					bool cellEnabled = ApplyAppearance(dataColumn, rowObject);
					if(!cellEnabled) {
						readOnlyCells.Add(dataColumn.FieldName + i);
					}
				}
			}
			return readOnlyCells;
		}
		protected virtual void UpdateBatchValues(List<ASPxDataUpdateValues> updatedValues) {
			foreach(ASPxDataUpdateValues row in updatedValues) {
				object obj = gridBatchOperationProvider.GetObject(row.Keys[0]);
				if(obj == null) {
					throw new ArgumentOutOfRangeException("Key=" + row.Keys[0]);
				}
				UpdateObjectValues(obj, row.NewValues, row.OldValues);
			}
		}
		protected virtual void InsertBatchValues(List<ASPxDataInsertValues> insertedValues) {
			foreach(ASPxDataInsertValues newRow in insertedValues) {
				object newObject = gridBatchOperationProvider.CreateNewObject();
				foreach(string propertyName in newRow.NewValues.Keys) {
					IMemberInfo memberInfo = objectTypeInfo.FindMember(propertyName);
					if(memberInfo != null) {
						memberInfo.SetValue(newObject, newRow.NewValues[propertyName]);
					}
				}
			}
		}
		protected virtual void AddEtalonRowValues(OrderedDictionary etalonValues) {
			OrderedDictionary objectValues = ObjectEtalonProvider.GetObjectEtalon(objectTypeInfo.Type, gridBatchOperationProvider);
			if(objectValues.Count > 0) {
				foreach(GridViewColumn column in grid.Columns) {
					GridViewDataColumn dataColumn = column as GridViewDataColumn;
					if(dataColumn != null && objectValues.Contains(dataColumn.FieldName)) {
						etalonValues.Add(dataColumn.FieldName, objectValues[dataColumn.FieldName]);
					}
				}
			}
		}
		protected virtual void SetupDataColumnBatchEditor(GridViewDataColumn dataColumn) {
			IModelColumn modelColumn = gridBatchOperationProvider.GetModelColumn(dataColumn);
			if(modelColumn.AllowEdit == false || !EditorBatchEnabled.Contains(modelColumn.PropertyEditorType)) {
				readOnlyColumns.Add(dataColumn.FieldName);
			}
		}
		protected static void RemoveBottomBorderForNewRow(ASPxGridViewTableRowEventArgs e) {
			if(e.VisibleIndex < 0 && WebApplicationStyleManager.IsNewStyle) {
				foreach(TableCell cell in e.Row.Cells) {
					cell.Style["border-bottom-width"] = "0px";
				}
			}
		}
		private void grid_RowValidating(object sender, ASPxDataValidationEventArgs e) {
			StartClientValidation();
			object obj = GetRowObject(e);
			ValidateRowObject(obj, e);
		}
		private void UpdateObjectValues(object obj, OrderedDictionary newValues, OrderedDictionary oldValues) {
			foreach(string propertyName in newValues.Keys) {
				if(newValues[propertyName] != oldValues[propertyName]) {
					IMemberInfo memberInfo = objectTypeInfo.FindMember(propertyName);
					if(memberInfo != null) {
						memberInfo.SetValue(obj, newValues[propertyName]);
					}
				}
			}
		}
		private string GetBatchEditStartEditingScript() {
			return string.Format("function Grid_BatchEditStartEditing(s, e) {{ ProcessGridStartBatchEditing(s, e, '{0}'); }}", ProtectedContentText);
		}
		private void Grid_InitNewRow(object sender, ASPxDataInitNewRowEventArgs e) {
			AddEtalonRowValues(e.NewValues);
		}
		private void Grid_BatchUpdate(object sender, ASPxDataBatchUpdateEventArgs e) {
			EndClientValidation();
			UpdateBatchValues(e.UpdateValues);
			InsertBatchValues(e.InsertValues);
			if(e.UpdateValues.Count > 0 || e.InsertValues.Count > 0) {
				gridBatchOperationProvider.CommitChanges();
			}
			e.Handled = true;
		}
		private void StartClientValidation() {
			if(validationObjectSpace == null) {
				validationObjectSpace = gridBatchOperationProvider.CreateObjectSpace(objectTypeInfo.Type);
			}
		}
		private void EndClientValidation() {
			if(validationObjectSpace != null) {
				validationObjectSpace.Dispose();
				validationObjectSpace = null;
			}
		}
		private void grid_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e) {
			e.Editor.ReadOnly = false;
			GridViewDataColumn dataColumn = e.Column as GridViewDataColumn;
			if(dataColumn != null) {
				SetupDataColumnBatchEditor(dataColumn);
			}
		}
		private void grid_CommandButtonInitialize(object sender, ASPxGridViewCommandButtonEventArgs e) {
			HideDeleteButtonForExistingObjects(e);
		}
		private void HideDeleteButtonForExistingObjects(ASPxGridViewCommandButtonEventArgs e) {
			if(e.VisibleIndex >= 0 && e.ButtonType == ColumnCommandButtonType.Delete) {
				e.Visible = false;
			}
		}
		private void grid_CustomJSProperties(object sender, ASPxGridViewClientJSPropertiesEventArgs e) {
			List<string> readOnlyCells = GetAppearanceDisabledCells();
			e.Properties["cpReadOnlyColumns"] = readOnlyColumns;
			e.Properties["cpReadOnlyCells"] = readOnlyCells;
		}
		private void grid_HtmlRowPrepared(object sender, ASPxGridViewTableRowEventArgs e) {
			RemoveBottomBorderForNewRow(e);
		}
		private bool ApplyAppearance(GridViewDataColumn dataColumn, object rowObject) {
			if(dataColumn != null && !readOnlyColumns.Contains(dataColumn.FieldName)) {
				IModelColumn modelColumn = gridBatchOperationProvider.GetModelColumn(dataColumn);
				if(modelColumn != null) {
					ASPxGridBatchEnabledAdapter adapter = new ASPxGridBatchEnabledAdapter();
					gridBatchOperationProvider.CustomizeAppearance(new CustomizeAppearanceEventArgs(modelColumn.PropertyName, adapter, rowObject));
					return adapter.Enabled;
				}
			}
			return true;
		}
		private static void RegisterColumnBatch() {
			EditorBatchEnabled.Add(typeof(ASPxEnumPropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxIntPropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxInt64PropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxDoublePropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxFloatPropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxDecimalPropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxStringPropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxBytePropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxBooleanPropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxDateTimePropertyEditor));
			EditorBatchEnabled.Add(typeof(ASPxColorPropertyEditor));
		}
#if DebugTest
		public List<string> TestReadOnlyColumns {
			get { return readOnlyColumns; }
		}
		public void TestGridRowValidating(ASPxDataValidationEventArgs e) {
			grid_RowValidating(null, e);
		}
		public void TestGridBatchUpdate(ASPxDataBatchUpdateEventArgs e) {
			Grid_BatchUpdate(null, e);
		}
#endif
	}
	public static class ObjectEtalonProvider {
		private static Dictionary<Type, OrderedDictionary> ObjectEtalonCache {
			get {
				var manager = ValueManager.GetValueManager<Dictionary<Type, OrderedDictionary>>("ObjectEtalonProvider_Cache");
				if(manager.Value == null) {
					manager.Value = new Dictionary<Type, OrderedDictionary>();
				}
				return manager.Value;
			}
		}
		public static OrderedDictionary GetObjectEtalon(Type type, IGridObjectSpaceCreator objectSpaceCreator) {
			OrderedDictionary values;
			if(!ObjectEtalonCache.TryGetValue(type, out values)) {
				values = CreateObjectEtalon(type, objectSpaceCreator);
				ObjectEtalonCache[type] = values;
			}
			return values;
		}
		public static void ClearCache() {
			ObjectEtalonCache.Clear();
		}
		private static OrderedDictionary CreateObjectEtalon(Type type, IGridObjectSpaceCreator objectSpaceCreator) {
			Guard.ArgumentNotNull(objectSpaceCreator, "objectSpaceCreator");
			OrderedDictionary values = new OrderedDictionary();
			using(IObjectSpace os = objectSpaceCreator.CreateObjectSpace(type)) {
				if(os.CanInstantiate(type)) {
					object obj = os.CreateObject(type);
					foreach(System.Reflection.PropertyInfo property in type.GetProperties()) {
						if(property.PropertyType.IsValueType) {
							values.Add(property.Name, property.GetValue(obj, new object[] { }));
						}
					}
				}
			}
			return values;
		}
	}
	internal class ASPxGridBatchEnabledAdapter : IAppearanceEnabled {
		public ASPxGridBatchEnabledAdapter() {
			Enabled = true;
		}
		public bool Enabled { get; set; }
		public void ResetEnabled() {
		}
	}
}

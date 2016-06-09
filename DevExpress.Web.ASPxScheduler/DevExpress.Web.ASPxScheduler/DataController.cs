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
using System.Collections.Specialized;
using System.Web.UI;
using DevExpress.Data;
using DevExpress.XtraScheduler.Data;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.Web.ASPxScheduler.Internal {
	public interface IWebSchedulerDataProvider {
		DataSourceView GetData();
		object GetLastInsertedId();
		bool NeedToRebindAfterInsert { get; set; }
		void RaiseRowInserting(ASPxSchedulerDataInsertingEventArgs e);
		void RaiseRowInserted(ASPxSchedulerDataInsertedEventArgs e);
		void RaiseRowUpdating(ASPxSchedulerDataUpdatingEventArgs e);
		void RaiseRowUpdated(ASPxSchedulerDataUpdatedEventArgs e);
		void RaiseRowDeleting(ASPxSchedulerDataDeletingEventArgs e);
		void RaiseRowDeleted(ASPxSchedulerDataDeletedEventArgs e);
	}
	public class FieldValueDictionary : Dictionary<string, object> {
	}
	public class WebSchedulerDataController : SchedulerDataController {
		#region Fields
		IWebSchedulerDataProvider dataProvider;
		bool isOperationCompletedSuccessfully;
		OrderedDictionary keys;
		OrderedDictionary oldValues;
		OrderedDictionary newValues;
		#endregion
		public WebSchedulerDataController(IWebSchedulerDataProvider dataProvider) {
			CheckExistenceDataProvider(dataProvider);
			this.dataProvider = dataProvider;
		}
		protected virtual void CheckExistenceDataProvider(IWebSchedulerDataProvider dataProvider) {
			if (dataProvider == null)
				Exceptions.ThrowArgumentNullException("dataProvider");
		}
		#region Properties
		protected internal IWebSchedulerDataProvider DataProvider { get { return dataProvider; } }
		protected internal bool IsOperationCompletedSuccessfully { get { return isOperationCompletedSuccessfully; } }
		protected internal OrderedDictionary Keys { get { return keys; } }
		protected internal OrderedDictionary OldValues { get { return oldValues; } }
		protected internal OrderedDictionary NewValues { get { return newValues; } }
		#endregion
		protected internal virtual OrderedDictionary RemoveReadOnlyValues(OrderedDictionary values) {
			OrderedDictionary result = new OrderedDictionary();
			foreach (string key in values.Keys) {
				DataColumnInfo columnInfo = Columns[key];
				if (!columnInfo.ReadOnly)
					result.Add(key, values[key]);
			}
			return result;
		}
		protected internal virtual OrderedDictionary ReplaceDBNullWithNullValues(OrderedDictionary values) {
			OrderedDictionary result = new OrderedDictionary();
			List<Object> nullKeys = new List<Object>();
			foreach (object key in values.Keys) {
				if (values[key] == DBNull.Value) {
					nullKeys.Add(key);
				}
				result.Add(key, values[key]);
			}
			foreach (object key in nullKeys) {
				values[key] = null;
				result[key] = null;
			}
			return result;
		}
		protected internal override void BeginTrackRowCommit(int rowIndex, MappingCollection mappings) {
			this.keys = GetRowKeys(rowIndex);
			this.oldValues = GetRowValues(rowIndex, mappings);
			this.newValues = GetRowValues(rowIndex, mappings);
		}
		protected internal override void EndTrackRowCommit(int rowIndex) {
		}
		#region Insert new row
		object defaultNewRow = null;
		public override int EndNewRowEdit() {
			DataSourceView view = GetView();
			if (view == null)
				return ListSourceDataController.InvalidRow;
			bool isXpoDataSource = DataSourceDetector.IsXpoDataSource(view);
			ASPxSchedulerDataInsertingEventArgs insertArgs = CreateInsertArgs(isXpoDataSource);
			OnRowInserting(insertArgs);
			if (insertArgs.Cancel)
				return ListSourceDataController.InvalidRow;
			this.isOperationCompletedSuccessfully = true;
			this.defaultNewRow = GetRowValue(ListSourceDataController.NewItemRow, KeyFieldName);
			view.Insert(insertArgs.NewValues, new DataSourceViewOperationCallback(HandleDataSourceViewInsertOperationCallback));
			if (isOperationCompletedSuccessfully)
				return base.EndNewRowEdit();
			else
				return ListSourceDataController.InvalidRow;
		}
		protected internal ASPxSchedulerDataInsertingEventArgs CreateInsertArgs(bool isXpoDataSource) {
			OrderedDictionary insertValues = this.newValues;
			if (isXpoDataSource)
				insertValues = RemoveReadOnlyValues(insertValues);
			ASPxSchedulerDataInsertingEventArgs insertArgs = new ASPxSchedulerDataInsertingEventArgs(insertValues);
			return insertArgs;
		}
		protected internal virtual bool HandleDataSourceViewInsertOperationCallback(int affectedRecords, Exception e) {
			ASPxSchedulerDataInsertedEventArgs args = new ASPxSchedulerDataInsertedEventArgs(affectedRecords, e, this.newValues);
			args.KeyFieldValue = GetRowValue(ListSourceDataController.NewItemRow, KeyFieldName);
			OnRowInserted(args);
			this.newValues = null;
			if (e != null && !args.ExceptionHandled) {
				this.isOperationCompletedSuccessfully = false;
				return false;
			}
			if (IsNull(args.KeyFieldValue) || args.KeyFieldValue.Equals(this.defaultNewRow))
				args.KeyFieldValue = DataProvider.GetLastInsertedId();
			SetRowValue(ListSourceDataController.NewItemRow, KeyFieldName, args.KeyFieldValue);
			if (IsNull(args.KeyFieldValue) || args.KeyFieldValue.Equals(this.defaultNewRow))
				this.dataProvider.NeedToRebindAfterInsert = true;
			this.isOperationCompletedSuccessfully = true;
			this.defaultNewRow = null;
			return true;
		}
		bool IsNull(object value) {
			return value == DBNull.Value || value == null;
		}
		protected internal virtual void OnRowInserting(ASPxSchedulerDataInsertingEventArgs e) {
			this.dataProvider.RaiseRowInserting(e);
		}
		protected internal virtual void OnRowInserted(ASPxSchedulerDataInsertedEventArgs e) {
			this.dataProvider.RaiseRowInserted(e);
		}
		#endregion
		#region Update exising row
		protected internal override void ProtectedEndRowEdit(int rowIndex) {
			DataSourceView view = GetView();
			if (view == null)
				return;
			bool isXpoDataSource = DataSourceDetector.IsXpoDataSource(view);
			bool isLinqDataSource = DataSourceDetector.IsLinqDataSource(view);
			ASPxSchedulerDataUpdatingEventArgs updateArgs = GetUpdateArgs(isXpoDataSource, isLinqDataSource);
			OnRowUpdating(updateArgs);
			if (updateArgs.Cancel)
				return;
			this.isOperationCompletedSuccessfully = true;
			view.Update(updateArgs.Keys, updateArgs.NewValues, updateArgs.OldValues, new DataSourceViewOperationCallback(HandleDataSourceViewOperationCallback));
			if (isOperationCompletedSuccessfully)
				base.ProtectedEndRowEdit(rowIndex);
		}
		protected internal ASPxSchedulerDataUpdatingEventArgs GetUpdateArgs(bool isXpoDataSource, bool isLinqDataSource) {
			OrderedDictionary filteredOldValues = this.oldValues;
			OrderedDictionary filteredNewValues = this.newValues;
			if (isXpoDataSource) {
				filteredOldValues = RemoveReadOnlyValues(filteredOldValues);
				filteredNewValues = RemoveReadOnlyValues(filteredNewValues);
			}
			if (isLinqDataSource)
				filteredOldValues = ReplaceDBNullWithNullValues(filteredOldValues);
			ASPxSchedulerDataUpdatingEventArgs updateArgs = new ASPxSchedulerDataUpdatingEventArgs(this.keys, filteredOldValues, filteredNewValues);
			return updateArgs;
		}
		protected internal virtual bool HandleDataSourceViewOperationCallback(int affectedRecords, Exception e) {
			ASPxSchedulerDataUpdatedEventArgs args;
			args = new ASPxSchedulerDataUpdatedEventArgs(affectedRecords, e, this.keys, this.oldValues, this.newValues);
			OnRowUpdated(args);
			this.keys = null;
			this.oldValues = null;
			this.newValues = null;
			if (e != null && !args.ExceptionHandled)
				return false;
			return true;
		}
		protected internal virtual void OnRowUpdating(ASPxSchedulerDataUpdatingEventArgs e) {
			dataProvider.RaiseRowUpdating(e);
		}
		protected internal virtual void OnRowUpdated(ASPxSchedulerDataUpdatedEventArgs e) {
			dataProvider.RaiseRowUpdated(e);
		}
		#endregion
		#region Delete exising row
		protected internal override void BeforeDeleteRow(int rowIndex, MappingCollection mappings) {
			base.BeforeDeleteRow(rowIndex, mappings);
			this.keys = GetRowKeys(rowIndex);
			this.oldValues = GetRowValues(rowIndex, mappings);
		}
		public override void DeleteRow(int controllerRow) {
			DataSourceView view = GetView();
			if (view == null)
				return;
			bool isLinqDataSource = DataSourceDetector.IsLinqDataSource(view);
			ASPxSchedulerDataDeletingEventArgs args = CreateDeletingEventArgs(isLinqDataSource);
			OnRowDeleting(args);
			if (args.Cancel)
				return;
			view.Delete(args.Keys, args.Values, new DataSourceViewOperationCallback(HandleDataSourceViewDeleteOperationCallback));
			base.DeleteRow(controllerRow);
		}
		protected internal virtual ASPxSchedulerDataDeletingEventArgs CreateDeletingEventArgs(bool isLinqDataSource) {
			OrderedDictionary values = this.oldValues;
			if (isLinqDataSource)
				values = ReplaceDBNullWithNullValues(values);
			return new ASPxSchedulerDataDeletingEventArgs(this.keys, values);
		}
		protected virtual bool HandleDataSourceViewDeleteOperationCallback(int affectedRecords, Exception e) {
			ASPxSchedulerDataDeletedEventArgs args;
			args = new ASPxSchedulerDataDeletedEventArgs(affectedRecords, e, this.keys, this.oldValues);
			OnRowDeleted(args);
			this.keys = null;
			this.oldValues = null;
			if (e != null && !args.ExceptionHandled)
				return false;
			return true;
		}
		protected internal virtual void OnRowDeleting(ASPxSchedulerDataDeletingEventArgs e) {
			dataProvider.RaiseRowDeleting(e);
		}
		protected internal virtual void OnRowDeleted(ASPxSchedulerDataDeletedEventArgs e) {
			dataProvider.RaiseRowDeleted(e);
		}
		#endregion
		protected internal virtual OrderedDictionary GetRowValues(int rowIndex, MappingCollection mappings) {
			OrderedDictionary result = new OrderedDictionary();
			int count = mappings.Count;
			MappingErrors errors = new MappingErrors();
			for (int i = 0; i < count; i++) {
				MappingBase mapping = mappings[i];
				DataColumnInfo columnInfo = Columns[mapping.Member];
				if (columnInfo == null) {
					errors.AddMissingMappings(mapping);
					continue;
				}
				string columnName = columnInfo.Name;
				if (result.Contains(columnName)) {
					errors.AddDuplicatedMappings(columnName);
					continue;
				}
				result.Add(columnName, GetRowValue(rowIndex, columnInfo));
			}
			errors.Throw();
			return result;
		}
		protected internal virtual OrderedDictionary GetRowKeys(int rowIndex) {
			OrderedDictionary result = new OrderedDictionary();
			result.Add(KeyFieldName, GetRowValue(rowIndex, KeyFieldName));
			return result;
		}
		protected override void SetRowValueCore(int controllerRow, int column, object val) {
			base.SetRowValueCore(controllerRow, column, val);
			if (this.newValues != null) {
				if (val == DBNull.Value)
					val = null;
				this.newValues[Columns[column].Name] = val;
			}
		}
		protected internal virtual DataSourceView GetData() {
			return dataProvider.GetData();
		}
		protected internal virtual DataSourceView GetView() {
			DataSourceView view = null;
			bool isBoundUsingId = true; 
			if (isBoundUsingId) {
				view = GetData();
				if (view == null)
					throw new Exception("DataSource returned null");
			}
			return view;
		}
	}
	public static class DataSourceDetector {
		public static bool IsXpoDataSource(DataSourceView view) {
			return view.GetType().Name == "XpoDataSourceView";
		}
		public static bool IsLinqDataSource(DataSourceView view) {
			return view.GetType().Name == "LinqDataSourceView";
		}
	}
	public class MappingErrors {
		const string DuplicatedFieldsMessageFormat = @"Data field ""{0}"" is used in more than one mapping.";
		const string MissingMappingsMessageFormat = @"Mapping ""{0}"" uses non-existent data field ""{1}"".";
		List<MappingBase> missingMappings;
		List<string> duplicatedMappings;
		public MappingErrors() {
			this.missingMappings = new List<MappingBase>();
			this.duplicatedMappings = new List<string>();
		}
		public void AddMissingMappings(MappingBase mapping) {
			this.missingMappings.Add(mapping);
		}
		public void AddDuplicatedMappings(string fieldName) {
			if (this.duplicatedMappings.Contains(fieldName))
				return;
			this.duplicatedMappings.Add(fieldName);
		}
		public string GetErrorMessage() {
			string format = "{0}";
			string missing = GetMissingMappings();
			string duplicate = GetDuplicatedFileds();
			if (!String.IsNullOrEmpty(duplicate))
				format += "\n{1}";
			return String.Format(format, missing, duplicate);
		}
		string GetDuplicatedFileds() {
			List<string> result = new List<string>();
			int count = this.duplicatedMappings.Count;
			for (int i = 0; i < count; i++) {
				result.Add(String.Format(DuplicatedFieldsMessageFormat, this.duplicatedMappings[i]));
			}
			return String.Join("\n", result.ToArray());
		}
		string GetMissingMappings() {
			List<string> result = new List<string>();
			int count = this.missingMappings.Count;
			for (int i = 0; i < count; i++) {
				MappingBase mapping = this.missingMappings[i];
				result.Add(String.Format(MissingMappingsMessageFormat, mapping.Name, mapping.Member));
			}
			return String.Join("\n", result.ToArray());
		}
		public void Throw() {
			if (!HasErrors())
				return;
			throw new MappingException(GetErrorMessage());
		}
		bool HasErrors() {
			return this.missingMappings.Count > 0 || this.duplicatedMappings.Count > 0;
		}
	}
}

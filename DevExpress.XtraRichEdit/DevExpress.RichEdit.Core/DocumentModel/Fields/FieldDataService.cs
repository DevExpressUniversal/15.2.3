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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Office;
#if !SL
using System.Data;
using System.ComponentModel;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region MailMergeDataMode
	public enum MailMergeDataMode {
		None,
		ViewMergedData,
		FinalMerging
	}
	#endregion
	#region IFieldDataService
	public interface IFieldDataService {
		bool BoundMode { get; }
		object GetFieldValue(MailMergeProperties mailMergeProperties, string fieldName, bool mapFieldName, MailMergeDataMode options, PieceTable pieceTable, Field field);
		IEnumerator<double> GetReferenceValuesEnumerator(PieceTable pieceTable, Field field, string reference);
	}
	#endregion
	#region IDynamicObjectPropertyValueProvider
	public interface IDynamicObjectPropertyValueProvider {
		bool IsDynamicTypeInstance(object instance);
		object GetPropertyValue(object instance, string propertyName);
	}
	#endregion
	#region IMailMergeDataService
	public interface IMailMergeDataService : IFieldDataService {
		int RecordCount { get; }
		bool MoveNextRecord();
		bool SetCurrentRecordIndex(int index);
		int GetCurrentRecordIndex();
		IMailMergeDataService StartMailMerge(IMailMergeOptions options);
		void EndMailMerge(IMailMergeDataService fieldDataService);
	}
	#endregion
	#region MailMergeDataService
	public class MailMergeDataService : IMailMergeDataService, IFieldDataService {
		protected readonly static string FieldNameFormatString = "<<{0}>>";
		int lastRecordIndex;
		int firstRecordIndex;
		readonly RichEditDataControllerAdapterBase dataController;
		IDynamicObjectPropertyValueProvider dynamicObjectPropertyValueProvider;
		public MailMergeDataService(RichEditDataControllerAdapterBase dataController)
			: this(dataController, 0, -1) {
		}
		public MailMergeDataService(RichEditDataControllerAdapterBase dataController, int firstRecordIndex, int lastRecordIndex) {
			Guard.ArgumentNotNull(dataController, "dataController");
			Guard.ArgumentNonNegative(firstRecordIndex, "firstRecordIndex");
			this.dataController = dataController;
			this.firstRecordIndex = firstRecordIndex;
			this.lastRecordIndex = lastRecordIndex;
			this.dataController = dataController;
		}
		public virtual bool BoundMode { get { return dataController.IsReady; } }
		protected virtual RichEditDataControllerAdapterBase DataController { get { return dataController; } }
		public int RecordCount { get { return dataController.ListSourceRowCount; } }
		public IDynamicObjectPropertyValueProvider DynamicObjectPropertyValueProvider { get { return dynamicObjectPropertyValueProvider; } set { dynamicObjectPropertyValueProvider = value; } }
		#region IMailMergeDataService Members
		protected virtual bool IsIndexValid(int index) {
			int count = CalcActualRecordCount();
			return index >= firstRecordIndex && index < count && (lastRecordIndex == -1 || index <= lastRecordIndex);
		}
		protected virtual int CalcActualRecordCount() {
			return DataController.ListSourceRowCount;
		}
		public virtual bool SetCurrentRecordIndex(int index) {
			if (!IsIndexValid(index))
				return false;
			dataController.CurrentControllerRow = index;
			return true;
		}
		public virtual bool MoveNextRecord() {
			int nextIndex = dataController.CurrentControllerRow + 1;
			if (!IsIndexValid(nextIndex))
				return false;
			dataController.CurrentControllerRow = nextIndex;
			return true;
		}
		public virtual int GetCurrentRecordIndex() {
			return dataController.CurrentControllerRow;
		}
		public IMailMergeDataService StartMailMerge(IMailMergeOptions options) {
			RichEditDataControllerAdapterBase mailMergeController = CreateMailMergeDataController();
			if (options.DataSource != null) {
				mailMergeController.DataSource = options.DataSource;
				mailMergeController.DataMember = options.DataMember;
				if (!mailMergeController.IsReady) {
					mailMergeController.Dispose();
					return null;
				}
			}
			else {
				mailMergeController.DataSource = DataController.DataSource;
				mailMergeController.DataMember = DataController.DataMember;
			}
			MailMergeDataService result = CreateNew(mailMergeController, options);
			if (result.SetCurrentRecordIndex(options.FirstRecordIndex)) {
				return result;
			}
			else {
				mailMergeController.Dispose();
				return null;
			}
		}
		protected virtual MailMergeDataService CreateNew(RichEditDataControllerAdapterBase dataController, IMailMergeOptions options) {
			return new MailMergeDataService(dataController, options.FirstRecordIndex, options.LastRecordIndex);
		}
		protected virtual RichEditDataControllerAdapterBase CreateMailMergeDataController() {
			return new RichEditDataControllerAdapter(new OfficeDataController());
		}
		public void EndMailMerge(IMailMergeDataService fieldDataService) {
			((MailMergeDataService)fieldDataService).DataController.Dispose();
		}
		#endregion
		#region IFieldDataService Members
		public virtual object GetFieldValue(MailMergeProperties mailMergeProperties, string fieldName, bool mapFieldName, MailMergeDataMode options, PieceTable pieceTable, Field field) {
			Guard.ArgumentNotNull(mailMergeProperties, "mailMergeOptions");
			if (!BoundMode)
				Exceptions.ThrowInternalException();
			if (options == MailMergeDataMode.None)
				return String.Format(FieldNameFormatString, fieldName);
			int columnIndex = GetColumnIndexByName(mailMergeProperties, fieldName, mapFieldName);
			if (columnIndex < 0) {
				if (dynamicObjectPropertyValueProvider != null) {
					object obj = DataController.GetCurrentRow();
					if (obj != null && dynamicObjectPropertyValueProvider.IsDynamicTypeInstance(obj))
						return TryGetDynamicObjectValue(obj, fieldName);
				}
				columnIndex = DataController.GetColumnIndex(fieldName);
			}
			return DataController.GetCurrentRowValue(columnIndex);
		}
		object TryGetDynamicObjectValue(object obj, string fieldName) {
			string[] properties = fieldName.Split('.');
			int count = properties.Length;
			for (int i = 0; i < count; i++) {
				obj = TryGetDynamicObjectValueCore(obj, properties[i]);
				if (obj == null)
					return null;
			}
			return obj;
		}
		object TryGetDynamicObjectValueCore(object obj, string fieldName) {
			if (obj == null)
				return null;
			if (dynamicObjectPropertyValueProvider.IsDynamicTypeInstance(obj))
				return dynamicObjectPropertyValueProvider.GetPropertyValue(obj, fieldName);
			else {
				System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(fieldName);
				if (propertyInfo == null)
					return null;
				return propertyInfo.GetValue(obj, null);
			}
		}
		protected virtual int GetColumnIndexByName(MailMergeProperties mailMergeProperties, string fieldName, bool mapFieldName) {
			DataSourceObjectProperties dataSourceObjectProperties = mailMergeProperties.DataSourceObjectProperties;
			if (mapFieldName) {
				return dataSourceObjectProperties.FindMapDataByMapName(fieldName).ColumnIndex;
			}
			else {
				FieldMapData fieldMapData = dataSourceObjectProperties.FindMapDataByColumnName(fieldName);
				if (fieldMapData == null)
					return -1;
				else
					return fieldMapData.ColumnIndex;
			}
		}
		public virtual IEnumerator<double> GetReferenceValuesEnumerator(PieceTable pieceTable, Field field, string reference) {
			return new SingleReferenceValueEnumerator(0);
		}
		#endregion
	}
	#endregion
	public class SingleReferenceValueEnumerator : IEnumerator<double> {
		readonly double val;
		bool resetted;
		public SingleReferenceValueEnumerator(double val) {
			this.val = val;
			Reset();
		}
		public double Current {
			get { return val; }
		}
		public void Dispose() {
		}
		object IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			if (resetted) {
				resetted = false;
				return true;
			}
			else
				return false;
		}
		public void Reset() {
			resetted = true;
		}
	}
}

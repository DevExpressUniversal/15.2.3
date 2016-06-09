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
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Fields;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public abstract class DataAccessServiceBase {
		readonly SnapDocumentModel documentModel;
		protected DataAccessServiceBase(SnapDocumentModel documentModel) {
			this.documentModel = documentModel;
		}
		protected SnapDocumentModel DocumentModel { get { return documentModel; } }
		protected IDataSourceDispatcher DataSourceDispatcher { get { return documentModel.DataSourceDispatcher; } }
		protected DataBrowser GetDataBrowserForParentDataMember(SnapDataContext dataContext, object dataSource, string dataMember) {
			return dataContext.GetDataBrowser(dataSource, dataMember, true);
		}
	}
	public class DataAccessService : DataAccessServiceBase, IDataAccessService {
		class NoLimitDataAccessService : DataAccessService {
			public NoLimitDataAccessService(SnapDocumentModel documentModel) : base(documentModel) { }
		}
		readonly Dictionary<string, bool> sortableCache = new Dictionary<string, bool>();
		public DataAccessService(SnapDocumentModel documentModel) : base(documentModel) { }
		public virtual IDataAccessService GetDataAccessServiceForPrinting() {
			return new NoLimitDataAccessService(DocumentModel);
		}
		#region IDataAccessService Members
		public virtual Type GetFieldType(SnapFieldInfo fieldInfo) {
			return (Type)InvokeListControllerMethod((controller, columnName) => controller.GetColumnType(columnName), fieldInfo);
		}
		public virtual IEnumerable<FilterItem> GetFilterItems(SnapFieldInfo fieldInfo) {
			return (IEnumerable<FilterItem>)InvokeListControllerMethod(FilteredValuesHelper.GetUniqueFilteredValues, fieldInfo);
		}
		public IEnumerable<object> GetFilterValues(SnapFieldInfo fieldInfo) {
			return (IEnumerable<object>)InvokeListControllerMethod((controller, columnName) => controller.GetUniqueFilteredValues(columnName), fieldInfo);
		}
		object InvokeListControllerMethod(Func<SnapListController, string, object> action, SnapFieldInfo fieldInfo) {
			MergefieldField parsedInfo = fieldInfo.ParsedInfo as MergefieldField;
			if (parsedInfo == null)
				return null;
			if (DocumentModel.SnapMailMergeVisualOptions.DataSourceName == null) {
				 if (fieldInfo.Field.Parent == null)
					 return null;
				 SNListField parentParsedInfo = FieldsHelper.GetParsedInfoCore(fieldInfo.PieceTable, fieldInfo.Field.Parent) as SNListField;
				 if (parentParsedInfo == null)
					 return null;
			}
			IFieldDataAccessService fieldDataAccessService = DocumentModel.GetService<IFieldDataAccessService>();
			FieldDataValueDescriptor fieldDataValueDescriptor = fieldDataAccessService.GetFieldValueDescriptor(fieldInfo.PieceTable, fieldInfo.Field, parsedInfo.DataFieldName);
			ICalculationContext calculationContext = fieldDataAccessService.FieldContextService.BeginCalculation(DataSourceDispatcher);
			try {
				if (!string.IsNullOrEmpty(parsedInfo.DataFieldName) && parsedInfo.DataFieldName.Contains("."))
					calculationContext.FieldNames = new[] { parsedInfo.DataFieldName };
				SnapListController snapListController = calculationContext.RestoreListController(fieldDataValueDescriptor.ParentDataContext);
				if (snapListController == null)
					return null;
				return action(snapListController, parsedInfo.DataFieldName);
			}
			finally {
				fieldDataAccessService.FieldContextService.EndCalculation(calculationContext);
			}
		}
		public bool AllowGroupAndSort(object dataSource, string dataMember, string fieldName) {
			return IsSortable(dataSource, dataMember, fieldName, true);
		}
		public virtual bool AllowFilter(object dataSource, string dataMember, string fieldName) {
			return IsSortable(dataSource, dataMember, fieldName, false);
		}
		public virtual bool AllowSum(object dataSource, string dataMember, string fieldName, SummaryItemType summaryItemType) {
			if (dataSource == null || string.IsNullOrEmpty(fieldName))
				return false;
			using (SnapDataContext dataContext = new SnapDataContext(DataSourceDispatcher.GetCalculatedFields(dataSource), DocumentModel.Parameters, null)) {
				ListBrowser listBrowser = GetDataBrowserForParentDataMember(dataContext, dataSource, dataMember) as ListBrowser;
				if (listBrowser == null)
					return false;
				string dMember = !string.IsNullOrEmpty(dataMember) ? string.Format("{0}.{1}", dataMember, fieldName) : fieldName;
				DataBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dMember, true);
				if (dataBrowser == null)
					return false;
				if (summaryItemType == SummaryItemType.Count)
					return true;
				Type dataSourceType = dataBrowser.DataSourceType;
				Type underlyingType = Nullable.GetUnderlyingType(dataSourceType);
				if (underlyingType != null)
					dataSourceType = underlyingType;
				if (summaryItemType == SummaryItemType.Min || summaryItemType == SummaryItemType.Max)
					return typeof(IComparable).IsAssignableFrom(dataSourceType);
				if (summaryItemType == SummaryItemType.Average || summaryItemType == SummaryItemType.Sum) {
					if (dataSourceType.Equals(typeof(DateTime)) || dataSourceType.Equals(typeof(Boolean)))
						return false;
					return dataSourceType.IsValueType;
				}
				return true;
			}
		}
		public virtual string GetFieldDisplayName(object dataSource, string dataMember, string fieldName) {
			using (SnapDataContext dataContext = new SnapDataContext(DataSourceDispatcher.GetCalculatedFields(dataSource), DocumentModel.Parameters, null)) {
				if (string.IsNullOrEmpty(dataMember))
					return dataContext.GetDataMemberDisplayName(dataSource, fieldName);
				string dataMemberDisplayName = dataContext.GetDataMemberDisplayName(dataSource, dataMember);
				string fieldNameDisplayName = dataContext.GetDataMemberDisplayName(dataSource, string.Format("{0}.{1}", dataMember, fieldName));
				if (!string.IsNullOrEmpty(fieldNameDisplayName) && !string.IsNullOrEmpty(dataMemberDisplayName) && fieldNameDisplayName.StartsWith(dataMemberDisplayName))
					return fieldNameDisplayName.Substring(dataMemberDisplayName.Length + 1);
				return string.Format("{0}.{1}", dataMember, fieldName);
			}
		}
		bool IsSortable(object dataSource, string dataMember, string fieldName, bool allowEmptyDataSource) {
			if (dataSource == null || string.IsNullOrEmpty(fieldName))
				return false;
			string dataSourceName = DataSourceDispatcher.FindDataSourceName(dataSource);
			string key = string.Format("{0}:{1}.{2}.{3}", allowEmptyDataSource ? "1" : "0", dataSourceName, dataMember, fieldName);
			bool result;
			if (sortableCache.TryGetValue(key, out result))
				return result;
			string[] nestedFields = fieldName.Contains(".") ? new[] { fieldName } : null;
			using (SnapDataContext dataContext = new SnapDataContext(DataSourceDispatcher.GetCalculatedFields(dataSource), DocumentModel.Parameters, null, nestedFields)) {
				ListBrowser listBrowser = GetDataBrowserForParentDataMember(dataContext, dataSource, dataMember) as ListBrowser;
				if (listBrowser != null && (allowEmptyDataSource || listBrowser.Count != 0)) {
					dataContext.Initialize(listBrowser, null, null);
					result = ((SnapListController)listBrowser.ListController).AllowSort(fieldName);
					sortableCache.Add(key, result);
					return result;
				}
			}
			return false;
		}
		public void ClearCache() {
			sortableCache.Clear();
		}
		#endregion
	}
}

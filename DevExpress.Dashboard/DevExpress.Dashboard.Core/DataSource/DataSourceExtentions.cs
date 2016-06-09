#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid;
namespace DevExpress.DashboardCommon.Native {
	public static class DataSourceExtentions {
		public static CriteriaOperator GetPatchedFilterCriteria(this IDashboardDataSource dataSource, string dataMember, string filter, IActualParametersProvider parameters, bool actualValues) {
			if(dataSource == null)
				return CriteriaOperator.Parse(filter);
			if(dataSource.GetIsOlap())
				return DevExpress.DataAccess.Native.Sql.FilterHelper.GetCriteriaOperator(filter, dataSource.Parameters ?? new IParameter[0]);
			return dataSource.GetExpandedCalculatedFieldExpressionOperator(filter, null, false, dataMember, parameters, actualValues);
		}
		public static object GetDataSchema(this IDashboardDataSource dataSource, string queryName) {
			return dataSource.GetDataSourceInternal().GetDataSchema(queryName);
		}
		public static DataField GetField(this IDashboardDataSource dataSource, string dataMember, string dataSetName) {
			IDataSourceSchema dataSchema = dataSource.GetDataSourceSchema(dataSetName);
			return dataSchema != null ? dataSchema.GetField(dataMember) : null;
		}
		public static DataFieldType GetFieldType(this IDashboardDataSource dataSource, string fieldName, string dataSetName) {
			IDataSourceSchema dataSchema = dataSource.GetDataSourceSchema(dataSetName);
			return dataSchema != null ? dataSchema.GetFieldType(fieldName) : DataFieldType.Unknown;
		}
		public static Type GetFieldSourceType(this IDashboardDataSource dataSource, string fieldName, string dataSetName) {
			IDataSourceSchema dataSchema = dataSource.GetDataSourceSchema(dataSetName);
			return dataSchema != null ? dataSchema.GetFieldSourceType(fieldName) : typeof(object);
		}
		public static bool ContainsField(this IDashboardDataSource dataSource, string dataMember, string dataSetName){
			IDataSourceSchema dataSchema = dataSource.GetDataSourceSchema(dataSetName);
			return dataSchema != null ? dataSchema.ContainsField(dataMember) : false;
		}
		public static DataSourceNodeBase GetRootNode(this IDashboardDataSource dataSource, string dataSetName) {
			IDataSourceSchema dataSourceSchema = dataSource.GetDataSourceSchema(dataSetName);
			return dataSourceSchema != null ? dataSourceSchema.RootNode : null;
		}
		public static string GetExpandedCalculatedFieldExpression(this IDashboardDataSource dataSource, CalculatedField field, string dataSetName, IActualParametersProvider parameters, bool actualValues) {
			return dataSource.GetCalculatedFieldsController().GetExpandedCalculatedFieldExpression(field, parameters, actualValues);
		}
		public static string GetFieldCaption(this IDashboardDataSource dataSource, string dataMember, string dataSetName) {
			IDataSourceSchema dataSourceSchema = dataSource.GetDataSourceSchema(dataSetName);
			return dataSourceSchema!=null? dataSourceSchema.GetFieldCaption(dataMember):null;
		}
		public static string GetDataMemberCaption(this IDashboardDataSource dataSource, string dataMember, string dataSetName) {
			IDataSourceSchema dataSourceSchema = dataSource.GetDataSourceSchema(dataSetName);
			return dataSourceSchema!=null? dataSourceSchema.GetDataMemberCaption(dataMember):null;
		}
		public static List<string> GetDataMembers(this IDashboardDataSource dataSource, string dataSetName) {
			IDataSourceSchema dataSourceSchema = dataSource.GetDataSourceSchema(dataSetName);
			return dataSourceSchema!=null? dataSourceSchema.GetDataMembers():null;
		}
		public static IStorage GetStorage(this IDashboardDataSource dataSource, string dataSetName) {
			IDashboardDataSourceInternal dataSourceInternal = dataSource.GetDataSourceInternal();
			return dataSourceInternal != null ? dataSourceInternal.GetStorage(dataSetName) : null;
		}
		public static bool IsCalculatedFieldValid(this IDashboardDataSource dataSource, CalculatedField field, IActualParametersProvider parameters) {
			return dataSource.GetCalculatedFieldsController().CheckIsCalculatedFieldValid(field, parameters);
		}
		public static Type GetCalculatedFieldDesiredType(this IDashboardDataSource dataSource, string expression, string dataMember, string name, bool useCache, IActualParametersProvider provider) {
			return dataSource.GetCalculatedFieldsController().GetCalculatedFieldDesiredType(dataSource.GetRootNode(dataMember), expression, dataMember, name, useCache, provider);
		}
		public static CriteriaOperator GetExpandedCalculatedFieldExpressionOperator(this IDashboardDataSource dataSource, string expression, string calculatedFieldName, bool throwOnError, string dataSetName, IActualParametersProvider parameters, bool actualValues) {
			return dataSource.GetCalculatedFieldsController().GetExpandedCalculatedFieldExpressionOperator(expression, calculatedFieldName, throwOnError, parameters, actualValues);
		}
		public static IPivotGridDataSource GetOlapDataSource(this IDashboardDataSource dataSource) {
			return dataSource.GetPivotDataSource("");
		}
		public static IPivotGridDataSource GetPivotDataSource(this IDashboardDataSource dataSource, string queryName) {
			return dataSource != null ? dataSource.GetDataSourceInternal().GetPivotDataSource(queryName) : null;
		}
		public static IList GetListSource(this IDashboardDataSource dataSource, string queryName) {
			return dataSource != null ? dataSource.GetDataSourceInternal().GetListSource(queryName) : null;
		}
		public static void SetCalculatedFieldIsServerEvaluable(this IDashboardDataSource dataSource, CalculatedField field, bool value) {
			dataSource.GetCalculatedFieldsController().SetCalculatedFieldIsEvaluable(field, value);
		}
		public static void OnCalculatedFieldParametersChanged(this IDashboardDataSource dataSource, IList<CalculatedField> fields, string dataSetName) {
			dataSource.GetCalculatedFieldsController().OnCalculatedFieldParametersChanged(fields);
		}
		public static Type ServerGetUnboundExpressionType(this IDashboardDataSource dataSource, string expression, string dataMember) {
			return dataSource.GetDataSourceInternal().ServerGetUnboundExpressionType(expression, dataMember);
		}
		public static bool IsCalculatedFieldNameValid(this IDashboardDataSource dataSource, string name, string dataSetName) {
			return dataSource.GetCalculatedFieldsController().IsCalculatedFieldNameValid(name);
		}
		public static CalculatedFieldDataColumnInfo CreateCalculatedFieldColumnInfo(this IDashboardDataSource dataSource, CalculatedField field, DashboardParameterCollection parameters) {
			return dataSource.GetDataSourceInternal().CreateCalculatedFieldColumnInfo(field, parameters);
		}
		public static bool IsOlapHierarchyDataField(this IDashboardDataSource dataSource, string dataMember) {
			return dataSource.GetDataSourceSchema("").IsOlapHierarchyDataField(dataMember);
		}
		public static IList GetHierarchyDataNodes(this IDashboardDataSource dataSource, OlapHierarchyDataField field) {
			return dataSource.GetDataSourceSchema("").GetHierarchyDataNodes(field);
		}	
		public static bool ContainsParametersDisplayMember(this IDashboardDataSource dataSource, string valueMember, string displayMember, string queryName) {
			return dataSource.GetDataSourceInternal().ContainsParametersDisplayMember(valueMember, displayMember, queryName);
		}
		public static List<ViewModel.ParameterValueViewModel> GetParameterValues(this IDashboardDataSource dataSource, string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			return dataSource.GetDataSourceInternal().GetParameterValues(valueMember, displayMember, queryName, provider);
		}
		public static void SetParameters(this IDashboardDataSource dataSource, IEnumerable<DevExpress.Data.IParameter> parameters) {
			dataSource.GetDataSourceInternal().SetParameters(parameters);
		}
		public static bool GetIsOlap(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.IsOlap;
		}
		public static bool IsSqlServerMode(this IDashboardDataSource dataSource, string queryName) {
			return dataSource.GetDataSourceInternal().IsSqlServerMode(queryName);
		}
		public static bool GetIsMultipleDataMemberSupported(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.IsMultipleDataMemberSupported;
		}
		public static IEnumerable<string> GetDataSets(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().GetDataSets();
		}
		public static bool GetIsSortOrderNoneSupported(this IDataSourceSchema dataSource) {
			return dataSource.GetIsOlap();
		}
		public static bool GetIsSpecificSortModeSupported(this IDataSourceSchema dataSource) {
			return dataSource.GetIsOlap();
		}
		public static bool GetIsSpecificValueFormatSupported(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.IsSpecificValueFormatSupported;
		}
		public static bool GetIsDataLoadingSupported(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.DataLoadingSupported;
		}
		public static bool GetIsRangeFilterSupported(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.RangeFilterSupported;
		}
		public static bool GetIsCalculatedFieldsSupported(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.CalculatedFieldsSupported;
		}
		public static bool GetIsDistinctCountSupported(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.DistinctCountSupported;
		}
		public static bool GetShouldProvideFakeData(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.ShouldProvideFakeData;
		}
		public static void SetDashboard(this IDashboardDataSource dataSource, Dashboard dashboard) {
			dataSource.GetDataSourceInternal().Dashboard = dashboard;
		}
		public static bool GetIsSummaryTypesSupported(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.SummaryTypesSupported;
		}
		public static bool GetIsDimensionGroupIntervalSupported(this IDataSourceSchema dataSource) {
			return !dataSource.GetIsOlap();
		}
		public static bool GetIsVSDesignMode(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().Properties.IsVSDesignMode;
		}
		public static string GetName_13_1(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().GetName_13_1();
		}
		public static XElement SaveToXml(this IDashboardDataSource dataSource) {
			return dataSource.GetDataSourceInternal().SaveToXml();
		}
		public static void LoadFromXml(this IDashboardDataSource dataSource, XElement element) {
			dataSource.GetDataSourceInternal().LoadFromXml(element);
		}	  
		public static object GetOlapDimensionValueDisplayText(this IDashboardDataSource dataSource, string columnName, object value, string dataMember) {
			IOLAPMember member = GetOlapDimensionMember(dataSource, columnName, value as string, dataMember);
			if(member == null)
				return value;
			return member.Caption;
		}
		public static IOLAPMember GetOlapDimensionMember(this IDashboardDataSource dataSource, string columnName, string memberName, string dataMember) {
			if(dataSource == null || !dataSource.GetIsOlap() || memberName == null)
				return null;
		 	return DataSourceHelper.GetOlapDimensionMember(dataSource.GetPivotDataSource(dataMember) as IPivotOLAPDataSource, columnName, memberName);
		}
		public static IEnumerable<object> GetUniqueValues(this IDashboardDataSource dataSource, Dimension dimension, string dataMember, IActualParametersProvider provider) {
			  if(dataSource == null || dimension == null)
				return null;
			return DataSourceHelper.GetUniqueValues(dataSource, dimension, dataMember, provider);
		}
	}
}

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

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.CriteriaVisitors;
namespace DevExpress.DashboardCommon.Native {
	public interface IExternalSchemaConsumer {
		void SetSchema(string dataMember, string[] schema);	
	}
	public static class ExternalSchemaConsumerHelper {
		public static string SafeDataMember(string dataMember) {
			return dataMember==null? string.Empty: dataMember;				
		}
		public  static Dictionary<string, List<string>> GetSchemaLists(IEnumerable<DataDashboardItem> items, IActualParametersProvider dashboardParameters, IDashboardDataSource dataSource) {
			Dictionary<string, List<string>> dataMemberLists = new Dictionary<string, List<string>>();
			foreach(DataDashboardItem item in items) {
				if(item == null)
					continue;
				List<string> dataMembers = null;
				if(!dataMemberLists.TryGetValue(SafeDataMember(item.DataMember), out dataMembers)) {
					dataMembers = new List<string>();
					dataMemberLists.Add(SafeDataMember(item.DataMember), dataMembers);
				}
				if(item.DataSource == dataSource) {
					foreach(DataItem dataItem in item.DataItems)
						dataMembers.AddRange(GetExpandedDataMembers(dashboardParameters, dataSource, dataItem.DataMember));
				}
				else if(item.IsMasterFilterCrossDataSource) {
					foreach(DataItem dataItem in item.DataItems) {
						if(!dataSource.ContainsField(dataItem.DataMember, SafeDataMember(item.DataMember)))
							continue;
						dataMembers.Add(dataItem.DataMember);
					}
				}
			}
			foreach(DashboardParameter parameter in dashboardParameters.GetParameters()) {
				DynamicListLookUpSettings dynamicSettings = parameter.LookUpSettings as DynamicListLookUpSettings;
				if(dynamicSettings != null && dynamicSettings.DataSource == dataSource) {
					List<string> dataMembers = null;
					if(!dataMemberLists.TryGetValue(SafeDataMember(dynamicSettings.DataMember), out dataMembers)) {
						dataMembers = new List<string>();
						dataMemberLists.Add(SafeDataMember(dynamicSettings.DataMember), dataMembers);
					}
					if(!string.IsNullOrEmpty(dynamicSettings.ValueMember))
						dataMembers.AddRange(GetExpandedDataMembers(dashboardParameters, dataSource, dynamicSettings.ValueMember));
					if(!string.IsNullOrEmpty(dynamicSettings.DisplayMember))
						dataMembers.AddRange(GetExpandedDataMembers(dashboardParameters, dataSource, dynamicSettings.DisplayMember));
				}
			}
			return dataMemberLists;
		}
		static IEnumerable<string> GetExpandedDataMembers(IActualParametersProvider dashboardParameters, IDashboardDataSource dataSource, string dataMember) {
			CalculatedField field = dataSource.CalculatedFields == null ? null : dataSource.CalculatedFields[dataMember];
			if(field != null) {
				ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
				CriteriaOperator expression = dataSource.GetCalculatedFieldsController().GetExpandedCalculatedFieldExpressionOperator(field.Expression, dataMember, false, dashboardParameters, false);
				if(!ReferenceEquals(null, expression)) {
					expression.Accept(visitor);
					return visitor.ColumnNames.Where(c => !string.IsNullOrEmpty(c));
				}
				return Enumerable.Empty<string>();
			}
			else
				return new string[] { dataMember };
		}
		public static void ApplySchema(IDashboardDataSource dataSource, IEnumerable<DataDashboardItem> items, IActualParametersProvider dashboardParameters, Dictionary<string, string[]> forcedDataMemberLists = null) {
			IExternalSchemaConsumer externalSchemaConsumer = dataSource as IExternalSchemaConsumer;
			if(externalSchemaConsumer != null) {
				Dictionary<string, List<string>> dataMemberLists = GetSchemaLists(items, dashboardParameters, dataSource);
				foreach(KeyValuePair<string, List<string>> keyValue in dataMemberLists) {
					string[] forcedMembers = forcedDataMemberLists != null ? forcedDataMemberLists[keyValue.Key] : new string[0];
					string[] schema = keyValue.Value.Concat(forcedMembers).Distinct().ToArray();
					externalSchemaConsumer.SetSchema(keyValue.Key, schema);
				}
			}
		}
	}
}

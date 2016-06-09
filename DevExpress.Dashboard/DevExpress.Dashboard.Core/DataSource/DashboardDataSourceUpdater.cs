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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	public static class DashboardDataSourceUpdater { 
		public static Dictionary<string, string> GetAliasRenamingMap(TableQuery oldQuery, TableQuery newQuery) {
			Guard.ArgumentNotNull(oldQuery, "oldQuery");
			Guard.ArgumentNotNull(newQuery, "newQuery");
			Dictionary<string, string> renamingMap = new Dictionary<string, string>();
			foreach(TableInfo newTable in newQuery.Tables) {
				TableInfo oldTable = oldQuery.Tables.Find(table => table.Name == newTable.Name);
				if(oldTable != null) {
					FillAliasRenamingMap(renamingMap, oldTable.SelectedColumns, newTable.SelectedColumns);
				}
			}
			return renamingMap;
		}
		static void FillAliasRenamingMap(Dictionary<string, string> renamingMap, List<ColumnInfo> oldColumns, List<ColumnInfo> newColumns) {
			foreach(ColumnInfo newColumn in newColumns) {
				ColumnInfo oldColumn = oldColumns.Find(column => column.Name == newColumn.Name);
				if(oldColumn != null && oldColumn.ActualName != newColumn.ActualName)
					renamingMap.Add(oldColumn.ActualName, newColumn.ActualName);
			}
		}
		public static void RenameColumns(Dashboard dashboard, IDashboardDataSource dataSource, string dataMember, Dictionary<string, string> renamingMap) {
			IEnumerable<DataDashboardItem> affectedItems = dashboard.Items.OfType<DataDashboardItem>().Where(item => item.DataSource == dataSource && item.DataMember == dataMember);
			foreach(DataDashboardItem dashboardItem in affectedItems)
				renamingMap.ForEach(renamePair => dashboardItem.ChangeDataItemDataMember(renamePair.Key, renamePair.Value));			
			IEnumerable<CalculatedField> calculatedFields = dataSource.CalculatedFields.Where(cf => cf.DataMember == dataMember);
			foreach(CalculatedField cf in calculatedFields)
				cf.Expression = NamesCriteriaPatcher.Process(cf.Expression, renamingMap);
			IEnumerable<DynamicListLookUpSettings> settings = dashboard.Parameters.Select(param => param.LookUpSettings as DynamicListLookUpSettings).Where(setting => setting != null && setting.DataSource == dataSource && setting.DataMember == dataMember);
			foreach(DynamicListLookUpSettings setting in settings) {
				foreach(KeyValuePair<string, string> renamePair in renamingMap) {
					if(setting.DisplayMember == renamePair.Key) setting.DisplayMember = renamePair.Value;
					if(setting.ValueMember == renamePair.Key) setting.ValueMember = renamePair.Value;
				}
			}
		}
		public static void RenameDataMember(Dashboard dashboard, IDashboardDataSource dataSource, string oldDataMember, string newDataMember) {
			List<DataDashboardItem> affectedItems = dashboard.Items.OfType<DataDashboardItem>().Where(item => item.DataSource == dataSource && item.DataMember == oldDataMember).ToList();
			foreach(DataDashboardItem item in affectedItems)
				item.DataMember = newDataMember;
			List<CalculatedField> calculatedFields = dataSource.CalculatedFields.Where(cf => cf.DataMember == oldDataMember).ToList();
			foreach(CalculatedField cf in calculatedFields)
				cf.DataMember = newDataMember;
			List<DynamicListLookUpSettings> settings = dashboard.Parameters.Select(param => param.LookUpSettings as DynamicListLookUpSettings).Where(setting => setting != null && setting.DataSource == dataSource && setting.DataMember == oldDataMember).ToList();
			foreach(DynamicListLookUpSettings setting in settings)
				setting.DataMember = newDataMember;
		}
	}
}

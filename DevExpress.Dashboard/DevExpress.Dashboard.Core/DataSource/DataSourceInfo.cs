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
using DevExpress.XtraPivotGrid.Data;
using System.Text;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon.Native {
	public class DataSourceInfo {
		const string separator = ", ";
		public IDashboardDataSource DataSource { get; private set; }
		public string DataMember { get; private set; }
		public DataSourceInfo(IDashboardDataSource dataSource, string dataMember) {
			DataSource = dataSource;
			DataMember = dataMember;
		}
		public override bool Equals(object obj) {
			return DataSourceInfoComparer.Comparer.Equals(this, obj as DataSourceInfo);
		}
		public override int GetHashCode() {
			return DataSourceInfoComparer.Comparer.GetHashCode(this);
		}
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(DataSource.Name);
			if(!string.IsNullOrEmpty(DataMember)) {
				stringBuilder.Append(separator);
				stringBuilder.Append(DataMember);
			}
			return stringBuilder.ToString();
		}
	}
	public class DataSourceInfoComparer : IEqualityComparer<DataSourceInfo> {
		static DataSourceInfoComparer comparer;
		public static DataSourceInfoComparer Comparer {
			get {
				if(comparer == null)
					comparer = new DataSourceInfoComparer();
				return comparer;
			}
		}
		public bool Equals(DataSourceInfo x, DataSourceInfo y) {
			if(object.ReferenceEquals(x, y))
				return true;
			string dataMember1 = x.GetDataMemberSafe();
			string  dataMember2 = y.GetDataMemberSafe();
			return x.GetDataSourceSafe() == y.GetDataSourceSafe() &&
				(dataMember1 == dataMember2 || string.IsNullOrEmpty(dataMember1) && string.IsNullOrEmpty(dataMember2));
		}
		public int GetHashCode(DataSourceInfo obj) {
			return (obj.DataSource == null ? 0 : obj.DataSource.GetHashCode()) ^
					  (obj.DataMember == null ? 0 : obj.DataMember.GetHashCode());
		}
	}
	public static class DataSourceInfoExtensions {
		public static IStorage GetStorage(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null ? dataSourceInfo.DataSource.GetStorage(dataSourceInfo.DataMember) : null;
		}
		public static IPivotGridDataSource GetPivotSource(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null ? dataSourceInfo.DataSource.GetPivotDataSource(dataSourceInfo.DataMember) : null;
		}
		public static IList  GetListSource(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null ? dataSourceInfo.DataSource.GetListSource(dataSourceInfo.DataMember) : null;
		}
		public static IDataSourceSchema GetPickManager(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null ? dataSourceInfo.DataSource.GetDataSourceSchema(dataSourceInfo.DataMember) : null;
		}
		public static DataSourceNodeBase GetRootNode(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null ? dataSourceInfo.DataSource.GetRootNode(dataSourceInfo.DataMember) : null;
		}
		public static DataFieldType GetFieldType(this DataSourceInfo dataSourceInfo, string fieldName) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null ? dataSourceInfo.DataSource.GetFieldType(fieldName, dataSourceInfo.DataMember) : DataFieldType.Unknown;
		}
		public static string GetDataMemberSafe(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataMember != null ? dataSourceInfo.DataMember : null;
		}
		public static IDashboardDataSource GetDataSourceSafe(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null ? dataSourceInfo.DataSource : null;
		}
		public static bool GetIsConnected(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null && dataSourceInfo.DataSource.IsConnected;
		}
		public static bool GetIsOlap(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null && dataSourceInfo.DataSource.GetIsOlap();
		}
		public static DataProcessingMode GetDataProcessingMode(this DataSourceInfo dataSourceInfo) {
			return dataSourceInfo != null && dataSourceInfo.DataSource != null && dataSourceInfo.DataSource.IsSqlServerMode(dataSourceInfo.DataMember) ? DataProcessingMode.Server : DataProcessingMode.Client;
		}
	}
}

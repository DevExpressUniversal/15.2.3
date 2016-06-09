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

using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public abstract class ColorShemeProvider {
		readonly IDataSessionProvider dataSessionProvider;
		readonly IDataSourceInfoProvider dataInfoProvider;
		protected IDataSessionProvider DataSessionProvider { get { return dataSessionProvider; } }
		protected IDataSourceInfoProvider DataInfoProvider { get { return dataInfoProvider; } }
		protected ColorShemeProvider(IDataSessionProvider dataSessionProvider, IDataSourceInfoProvider dataInfoProvider) {
			Guard.ArgumentNotNull(dataSessionProvider, "dataSessionProvider");
			Guard.ArgumentNotNull(dataInfoProvider, "dataInfoProvider");
			this.dataSessionProvider = dataSessionProvider;
			this.dataInfoProvider = dataInfoProvider;
		}
		protected IList<object[]> GetDashboardItemColoringValues(DataDashboardItem dashboardItem, IEnumerable<DimensionDefinition> definitions) {
			DataSourceInfo dataInfo = dataInfoProvider.GetDataSourceInfo(dashboardItem.DataSourceName, dashboardItem.DataMember);
			List<object[]> result = new List<object[]>();
			if(dataInfo == null)
				return result;
			IDataSession dataSession = dataSessionProvider.GetDataSession(dashboardItem.ComponentName, dataInfo, dashboardItem.FakeDataSource);
			DataStorage storage = dataSession.GetData(((ISliceDataQueryProvider)dashboardItem).GetDataQuery(dataSessionProvider)).HierarchicalData.Storage;
			if (!storage.IsEmpty) {
				List<StorageColumn> columns = dashboardItem.ColoringDimensions.GroupBy(dimension => dimension.GetDimensionDefinition())
					.Select(group => group.First())
					.Select((d) => storage.GetColumn(dashboardItem.GetDataItemUniqueName(d)))
					.ToList();
				if (columns.Count != 0) {
					foreach (StorageRow row in storage.OrderByDescending((s) => s.KeyColumns.Count()).First()) {
						object[] vals = new object[columns.Count];
						for (int i = 0; i < vals.Length; i++)
							vals[i] = row[columns[i]].MaterializedValue;
						result.Add(vals);
					}
				}
			}
			return result;
		}
	}
}

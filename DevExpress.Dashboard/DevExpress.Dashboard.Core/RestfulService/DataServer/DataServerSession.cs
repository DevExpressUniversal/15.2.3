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
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Server;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using System.Collections;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	public sealed class DataServerSession : IDisposable, IDataConnectionParametersProvider, IErrorHandler {
		readonly string dashboardId;
		readonly IDashboardDataSource dataSource;
		readonly string dataMember;
		readonly IEnumerable<IParameter> parameters;
		List<DataLoaderError> dataLoaderErrors;
		public List<DataLoaderError> DataLoaderErrors { get { return dataLoaderErrors;  }  }
		public IDashboardDataSource DataSource { get { return dataSource; } }
		public event ConfigureServiceDataConnectionEventHandler ConfigureDataConnection;
		public event ConfigureServiceDataLoadingEventHandler ConfigureDataLoading;
		public DataServerSession(string dashboardId, IDashboardDataSource dataSource, string dataMember, IEnumerable<IParameter> parameters) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			this.dashboardId = dashboardId;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			this.parameters = parameters;
		}
		public void ReloadData() {
			DashboardDataLoader dataLoader = new DashboardDataLoader(
				this, null,
#if !DXPORTABLE
				null,
#endif
				null, this, null, null, null
			);
			dataLoader.LoadData(new[] { dataSource }, parameters);
		}
		public DataQueryResult GetItemData(SliceDataQuery sliceDataQuery, IDataSessionProvider dataSessionProvider) {
			IDataSession dataSession = dataSessionProvider.GetDataSession(null, new DataSourceInfo(dataSource, dataMember), null);
			PatchSliceQueryParameters(sliceDataQuery, dataSessionProvider);
#if !DXPORTABLE
			if (dataSource is DashboardObjectDataSource) { 
				List<string> schema = new List<string>();
				foreach (DataField dataField in dataSource.GetDataSourceSchema("").RootNode.ChildNodes.OfType<DataField>()) {
					schema.Add(dataField.DataMember);
				}
				((IExternalSchemaConsumer)dataSource).SetSchema("", schema.ToArray());
			}
#endif
			return dataSession.GetData(sliceDataQuery);
		}
		public void Dispose() {
			dataSource.Dispose();
		}
		void PatchSliceQueryParameters(SliceDataQuery sliceDataQuery, IActualParametersProvider provider) {
			foreach(SliceModel slice in sliceDataQuery.DataSlices) {
				if(!object.ReferenceEquals(slice.FilterCriteria, null))
					slice.FilterCriteria = dataSource.GetPatchedFilterCriteria(dataMember, slice.FilterCriteria.ToString(), provider, true);
				slice.Dimensions.ForEach(d => PatchDimModel(d, provider));
				slice.FilterDimensions.ForEach(d => PatchDimModel(d, provider));
				slice.Measures.ForEach(d => PatchDimModel(d, provider));
			}
			sliceDataQuery.Axis1.ForEach(d => PatchDimModel(d, provider));
			sliceDataQuery.Axis2.ForEach(d => PatchDimModel(d, provider));
		}
		void PatchDimModel(DataProcessing.DimensionModel d, IActualParametersProvider provider) {
			var s = dataSource.GetPatchedFilterCriteria(dataMember, d.UnboundExpression, provider, true);
			d.UnboundExpression = object.ReferenceEquals(s, null)?null:s.ToString();
		}
		void PatchDimModel(DataProcessing.MeasureModel m, IActualParametersProvider provider) {
			var s = dataSource.GetPatchedFilterCriteria(dataMember, m.UnboundExpression, provider, true);
			m.UnboundExpression = object.ReferenceEquals(s, null) ? null : s.ToString();
		}
		#region IDataConnectionParametersProvider members
		IDBSchemaProvider IDataConnectionParametersProvider.GetDbSchemaProvider(SqlDataConnection dataConnection) {
			return new DBSchemaProvider();
		}
		object IDataConnectionParametersProvider.RaiseDataLoading(string dataSourceComponentName, string dataSourceName, object data) {
			if(dataSource.ComponentName != dataSourceComponentName)
				throw new ArgumentException("IDataConnectionParametersProvider is processing undefined dataSource");
			if(dataSource.GetIsDataLoadingSupported()) {
				ConfigureServiceDataLoadingEventHandler handler = ConfigureDataLoading;
				if(handler != null) {
					ConfigureServiceDataLoadingEventArgs args = new ConfigureServiceDataLoadingEventArgs(dashboardId, dataSourceComponentName, dataSourceName, data);
					handler(this, args);
					return args.Data;
				}
				return data;
			} else
				return null;
		}
		CriteriaOperator IDataConnectionParametersProvider.RaiseCustomFilterExpression(CustomFilterExpressionEventArgs e) {
			return e.FilterExpression;
		}
		#endregion
		#region IDataConnectionParametersService members
		DataConnectionParametersBase IDataConnectionParametersProvider.RaiseConfigureDataConnection(string connectionName, string dataSourceName, DataConnectionParametersBase connectionParameters) {
			ConfigureServiceDataConnectionEventHandler handler = ConfigureDataConnection;
			if(handler != null) {
				ConfigureServiceDataConnectionEventArgs args = new ConfigureServiceDataConnectionEventArgs(connectionName, dataSourceName, connectionParameters);
				handler(this, args);
				return args.ConnectionParameters;
			}
			return connectionParameters;
		}
		DataConnectionParametersBase IDataConnectionParametersProvider.RaiseHandleConnectionError(string dataSourceName, ConnectionErrorEventArgs args) {
			return args.ConnectionParameters;
		}
		#endregion
		#region IErrorHandler members
		void IErrorHandler.ShowDataSourceLoaderResultMessageBox(DataSourceLoadingResultType result) {
		}
		void IErrorHandler.ShowDataSourceLoadingErrors(List<DataLoaderError> dataLoaderErrors) {
			this.dataLoaderErrors = new List<DataLoaderError>(dataLoaderErrors.FindAll(error => error.Type == DataLoaderErrorType.Connection));
		}
		#endregion
	}
}

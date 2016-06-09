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
using DevExpress.Data;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	public class DataServer : Server<DataServer.Key, DataServerSession>, IDataServer {
		#region Key
		public class Key {
			public string DashboardId { get; private set; }
			public string DataSourceName { get; private set; }
			public string DataMember { get; private set; }
			public IEnumerable<IParameter> Parameters { get; private set; }
			public Key(string dashboardId, string dataSourceName, string dataMember, IEnumerable<IParameter> parameters) {
				DashboardId = dashboardId;
				DataSourceName = dataSourceName;
				DataMember = dataMember;
				Parameters = parameters;
			}
			public override string ToString() {
				StringBuilder sb = new StringBuilder();
				foreach (IParameter p in Parameters.OrderBy(p => p, ParameterComparer.Default))
					sb.Append(string.Format("{0}={1}|", p.Name, p.Value));
				string parametersId = sb.Length != 0 ? string.Format(".{0}", sb.ToString()) : string.Empty;
				string dataSourceId = string.Format("{0}.{1}", DashboardId, DataSourceName);
				return string.Format("{0}.{1}{2}", dataSourceId, DataMember, parametersId);
			}
		}
		#endregion
		readonly IDataSourceFactory factory;
		event ConfigureServiceDataConnectionEventHandler configureDataConnection;
		event ConfigureServiceDataLoadingEventHandler configureDataLoading;
		public DataServer(IDataSourceFactory factory) {
			Guard.ArgumentNotNull(factory, "factory");
			this.factory = factory;
		}
		protected override DataServerSession CreateSessionInstance(Key key) {
			IDashboardDataSource dataSource = factory.CreateDataSource(key.DashboardId, key.DataSourceName);
			return new DataServerSession(key.DashboardId, dataSource, key.DataMember, key.Parameters);
		}
		protected override void OpenSession(DataServerSession session) {
			base.OpenSession(session);
			session.ConfigureDataConnection += OnSessionConfigureDataConnection;
			session.ConfigureDataLoading += OnSessionConfigureDataLoading;
			session.ReloadData();
		}
		protected override void CloseSession(DataServerSession session) {			
			session.ConfigureDataConnection -= OnSessionConfigureDataConnection;
			session.ConfigureDataLoading -= OnSessionConfigureDataLoading;
			base.CloseSession(session);
		}
		void OnSessionConfigureDataConnection(object sender, ConfigureServiceDataConnectionEventArgs e) {
			if (configureDataConnection != null)
				configureDataConnection(sender, e);
		}
		void OnSessionConfigureDataLoading(object sender, ConfigureServiceDataLoadingEventArgs e) {
			if (configureDataLoading != null)
				configureDataLoading(sender, e);
		}
		#region IDataServer Members
		event ConfigureServiceDataConnectionEventHandler IDataServer.ConfigureDataConnection {
			add { configureDataConnection += value; }
			remove { configureDataConnection -= value; }
		}
		event ConfigureServiceDataLoadingEventHandler IDataServer.ConfigureDataLoading {
			add { configureDataLoading += value; }
			remove { configureDataLoading -= value; }
		}
		ServerResult<DataServerSession> IDataServer.GetSession(string dashboardId, string dataSourceName, string dataMember, IEnumerable<IParameter> parameters) {
			return GetSession(new Key(dashboardId, dataSourceName, dataMember, parameters));
		}
		ServerResult<DataServerSession> IDataServer.GetSession(string dashboardId, string dataSourceName, string dataMember, IEnumerable<IParameter> parameters, TimeSpan timeout) {
			return GetSession(new Key(dashboardId, dataSourceName, dataMember, parameters), timeout);
		}
		#endregion
	}
}

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
using DevExpress.DataAccess.ConnectionParameters;
namespace DevExpress.DashboardCommon.Native.DashboardRestfulService {
	public delegate void ConfigureServiceDataConnectionEventHandler(object sender, ConfigureServiceDataConnectionEventArgs e);
	public class ConfigureServiceDataConnectionEventArgs : EventArgs {
		public string ConnectionName { get; private set; }
		public string DataSourceName { get; private set; }
		public DataConnectionParametersBase ConnectionParameters { get; set; }
		public ConfigureServiceDataConnectionEventArgs(string connectionName, string dataSourceName, DataConnectionParametersBase connectionParameters)
			: base() {
			ConnectionName = connectionName;
			DataSourceName = dataSourceName;
			ConnectionParameters = connectionParameters;
		}
	}
	public delegate void ConfigureServiceDataLoadingEventHandler(object sender, ConfigureServiceDataLoadingEventArgs e);
	public class ConfigureServiceDataLoadingEventArgs  {
		public string DataSourceComponentName { get; private set; }
		public string DataSourceName { get; private set; }
		public object Data { get; set; }
		public string DashboardId { get; private set; }
		public ConfigureServiceDataLoadingEventArgs(string dashboardId, string dataSourceComponentName, string dataSourceName, object data)
			: base() {
			DataSourceComponentName = dataSourceComponentName;
			DataSourceName = dataSourceName;
			Data = data;
			DashboardId = dashboardId;
		}
	}
}

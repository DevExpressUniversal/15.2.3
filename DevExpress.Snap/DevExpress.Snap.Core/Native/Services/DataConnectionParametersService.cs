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

using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.Snap.Core.Native;
using System.Collections.Generic;
namespace DevExpress.Snap.Native.Services {
	public class CompositeDataConnectionParametersService : IDataConnectionParametersService {
		readonly HashSet<IDataConnectionParametersService> innerList = new HashSet<IDataConnectionParametersService>();
		public void AddService(IDataConnectionParametersService srv) {
			this.innerList.Add(srv);
		}
		public DataConnectionParametersBase RaiseConfigureDataConnection(string connectionName, DataConnectionParametersBase parameters) {
			foreach (IDataConnectionParametersService srv in this.innerList)
				parameters = srv.RaiseConfigureDataConnection(connectionName, parameters);
			return parameters;
		}
		public DataConnectionParametersBase RaiseHandleConnectionError(ConnectionErrorEventArgs eventArgs) {
			foreach (IDataConnectionParametersService srv in this.innerList) {
				srv.RaiseHandleConnectionError(eventArgs);
				if (eventArgs.Handled)
					return eventArgs.Cancel ? null : eventArgs.ConnectionParameters;
			}
			return eventArgs.ConnectionParameters;
		}
	}
	public class DataConnectionParametersService : IDataConnectionParametersService {
		readonly SnapDocumentModel snapDocumentModel;
		public DataConnectionParametersService(SnapDocumentModel snapDocumentModel) {
			this.snapDocumentModel = snapDocumentModel;
		}
		#region IDataConnectionParametersProvider Members
		DataConnectionParametersBase IDataConnectionParametersService.RaiseConfigureDataConnection(string connectionName, DataConnectionParametersBase parameters) {
			return snapDocumentModel.RaiseConfigureDataConnection(connectionName, parameters);
		}
		DataConnectionParametersBase IDataConnectionParametersService.RaiseHandleConnectionError(ConnectionErrorEventArgs eventArgs) {
			return snapDocumentModel.RaiseHandleConnectionError(eventArgs);
		}
		#endregion
	}
}

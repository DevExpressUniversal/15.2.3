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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.LookAndFeel;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public class DataConnectionParametersProviderCoreUIBase<T> : IDataConnectionParametersService where T : IDataConnectionParametersService {
		T nativeConnectionProvider;
		readonly UserLookAndFeel lookAndFeel;
		readonly IWin32Window ownerForm;
		public DataConnectionParametersProviderCoreUIBase(T nativeConnectionProvider, UserLookAndFeel lookAndFeel, IWin32Window ownerForm) {
			this.nativeConnectionProvider = nativeConnectionProvider;
			this.lookAndFeel = lookAndFeel;
			this.ownerForm = ownerForm;
		}
		protected T NativeConnectionProvider { get { return this.nativeConnectionProvider; } set { this.nativeConnectionProvider = value; } }
		DataConnectionParametersForm CreateForm() {
			DataConnectionParametersForm form = new DataConnectionParametersForm { OwnerForm = ownerForm };
			form.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			return form;
		}
		#region IDataConnectionParametersService Members
		DataConnectionParametersBase IDataConnectionParametersService.RaiseConfigureDataConnection(string connectionName, DataConnectionParametersBase parameters) {
			return this.nativeConnectionProvider.RaiseConfigureDataConnection(connectionName, parameters);
		}
		DataConnectionParametersBase IDataConnectionParametersService.RaiseHandleConnectionError(ConnectionErrorEventArgs eventArgs) {
			DataConnectionParametersBase result = this.nativeConnectionProvider.RaiseHandleConnectionError(eventArgs);
			if(eventArgs.Handled)
				return result;
			eventArgs.Handled = true;
			Exception innerException = eventArgs.Exception.InnerException;
			return ShowForm(eventArgs.ConnectionName, eventArgs.ConnectionParameters, innerException != null ? innerException.Message : eventArgs.Exception.Message);
		}
		protected virtual DataConnectionParametersBase ShowForm(string connectionName, DataConnectionParametersBase connectionParameters, string message) {
			using(DataConnectionParametersForm form = CreateForm()) {
				form.ErrorMessage = message;
				return form.ShowParametersForm(connectionName, connectionParameters);
			}
		}
		#endregion
	}
	public class DataConnectionParametersServiceUI : DataConnectionParametersProviderCoreUIBase<IDataConnectionParametersService> {
		readonly TaskScheduler ui;
		public DataConnectionParametersServiceUI(TaskScheduler ui, IDataConnectionParametersService nativeConnectionProvider, UserLookAndFeel lookAndFeel, IWin32Window ownerForm)
			: base(nativeConnectionProvider, lookAndFeel, ownerForm) {
			   this.ui = ui;
		}
		protected override DataConnectionParametersBase ShowForm(string connectionName, DataConnectionParametersBase connectionParameters, string message) {
			Task<DataConnectionParametersBase> task = Task.Factory.StartNew(() => base.ShowForm(connectionName, connectionParameters, message), CancellationToken.None, TaskCreationOptions.None, this.ui);
			task.Wait();
			return task.Result;
		}
	}
}

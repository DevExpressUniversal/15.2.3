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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
namespace DevExpress.XtraReports.Design {
	public class WizPageConnectionVS : WizPageConnection {
		DataConnectionsServiceWrapper dataConnectionsService;
		public WizPageConnectionVS(XRWizardRunnerBase runner) : base(runner) {
			this.meConnectionString.Visible = false;
			this.btnConnectionStringExpander.Visible = false;
			this.lbConnectionStringCaption.Visible = false;
		}
		private void CreateConnectionManager() {
			if(dataConnectionsService != null)
				return;
			VsServerExplorerAccessor serverExplorer;
			try {
				serverExplorer = new VsServerExplorerAccessor(designerHost);
			} catch(Exception e) {
				throw new Exception("Unable to get ServerExplorer", e);
			}
			serverExplorer.Init();
			Guid guid = VsDataConnectionsServiceAccessor.Guid;
			object connectionServiceObject = serverExplorer.GetRootService(ref guid);
			if(connectionServiceObject == null)
				throw new SystemException("ServerExplorer doesn't provide IDataConnectionsService");
			try {
				dataConnectionsService = new DataConnectionsServiceWrapper(designerHost, connectionServiceObject);
			} catch(Exception e) {
				throw new Exception("Can't create Connection Manager", e);
			}
		}
		bool UsesODBCProvider(int serverExplorerConnectionIndex) {
			string providerString = "Provider=MSDASQL";
			string connectionString = dataConnectionsService.GetConnectionString(serverExplorerConnectionIndex);
			return (connectionString.IndexOf(providerString) != -1);
		}
		void FillConnections() {
			cbConnections.Properties.BeginUpdate();
			Cursor currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try {
				cbConnections.Properties.Items.Clear();
				StringCollection connections = dataConnectionsService.GetConnectionNames();
				for(int i = 0; i < connections.Count; i++)
						cbConnections.Properties.Items.Add(connections[i]);
			} finally {
				cbConnections.Properties.EndUpdate();
				Cursor.Current = currentCursor;
			}
		}
		void UpdateConnectionsSelection() {
			cbConnections.Text = fWizard.ConnectionString == String.Empty && cbConnections.Properties.Items.Count > 0 ?
				(string)cbConnections.Properties.Items[0] :
				dataConnectionsService.GetConnectionNameUsingConnectionString(fWizard.ConnectionString);
		}
		protected override bool OnSetActive() {
			CreateConnectionManager();
			FillConnections();
			UpdateConnectionsSelection();
			return true;
		}
		protected override string OnWizardNext() {
			using(DevExpress.Data.Native.VS2005ConnectionStringHelper helper = new DevExpress.Data.Native.VS2005ConnectionStringHelper()) {
				string s = dataConnectionsService.GetConnectionString(cbConnections.SelectedIndex);
				fWizard.ConnectionString = helper.GetPatchedConnectionString(designerHost, s);
			}
			return DevExpress.Utils.WizardForm.NextPage;
		}
		protected override void btnNewConnection_Click(object sender, EventArgs e) {
			int connectionIndex = dataConnectionsService.PromptAndAddConnection();
			if (connectionIndex >= 0) {
				FillConnections();
				cbConnections.Text = dataConnectionsService.GetConnectionName(connectionIndex);
				UpdateWizardButtons();
			}
		}
	}
}

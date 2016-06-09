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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.ConnectionStrategies;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.DashboardWin.Native {
	public enum ConnectionType {
		Server,
		LocalCubeFile,
		CustomConnectionString
	}
	public enum OlapConnectionParameterEdits {
		ConnectionType,
		ServerName,
		UserName,
		Password,
		LocalCubeFile,
		ConnectionString,
		Catalog,
		CubeName
	}
	public partial class OlapConnectionparametersControl : BaseContentControl {
		readonly Dictionary<OlapConnectionParameterEdits, BaseEdit> editsMap;		
		public OlapConnectionparametersControl() {
			InitializeComponent();
			editsMap = new Dictionary<OlapConnectionParameterEdits, BaseEdit> {
				{OlapConnectionParameterEdits.ConnectionType, connectionType},
				{OlapConnectionParameterEdits.ServerName, serverName},
				{OlapConnectionParameterEdits.UserName, userName},
				{OlapConnectionParameterEdits.Password, password},
				{OlapConnectionParameterEdits.LocalCubeFile, localCubeFile},
				{OlapConnectionParameterEdits.ConnectionString, customConnectionString},
				{OlapConnectionParameterEdits.Catalog, catalog},
				{OlapConnectionParameterEdits.CubeName, cube},
			};
			connectionType.SelectedIndex = 0;
			serverName.EditValue = DefaultServerName;
			serverName.EditValueChanged += serverNameOrCredentialsChanged;
			localCubeFile.EditValueChanged += localCubeFileOrCredentialsChanged;
		}
		const string DefaultServerName = "local";
		public override string ConnectionNamePatternServerPart { get { return ServerName; } }
		public override string ConnectionNamePatternDatabasePart { get { return CubeName; } }
		string ServerName {
			get { return serverName.EditValue == null ? string.Empty : serverName.EditValue.ToString(); }
			set { serverName.EditValue = value; }
		}
		string Catalog {
			get { return catalog.SelectedItem == null ? string.Empty : catalog.SelectedItem.ToString(); }
			set { catalog.SelectedItem = value; }
		}
		string CubeName {
			get { return cube.SelectedItem == null ? string.Empty : cube.SelectedItem.ToString(); }
			set { cube.SelectedItem = value; }
		}
		string LocalCubeFile {
			get { return localCubeFile.EditValue == null ? string.Empty : localCubeFile.EditValue.ToString(); }
			set { localCubeFile.EditValue = value; }
		}
		string CustomConnectionString {
			get { return customConnectionString.EditValue == null ? string.Empty : customConnectionString.EditValue.ToString(); }
			set { customConnectionString.EditValue = value; } 
		}		
		string DataSource {
			get {
				return (ConnectionType == Native.ConnectionType.Server) ? ServerName :
					(ConnectionType == Native.ConnectionType.LocalCubeFile) ? LocalCubeFile : null;
			}
		}
		ConnectionType ConnectionType {
			get {
				if(connectionType.SelectedIndex == 0)
					return Native.ConnectionType.Server;
				else if(connectionType.SelectedIndex == 1)
					return Native.ConnectionType.LocalCubeFile;
				else
					return Native.ConnectionType.CustomConnectionString;
			}
		}
		void OnAdoMetaGetterException(object sender, AdomdMetaGetterExceptionEventArgs e1) {
			IOLAPResponseException ex = e1.Exception as IOLAPResponseException;
			if(ex != null && ex.RaisedException != null)
				throw ex.RaisedException;
			else
				throw (e1.Exception);
		}
		void database_QueryPopUp(object sender, CancelEventArgs e) {
			if(catalog.Properties.Items.Count == 0) {
				DbDataConnectionControlsHelper.FillDataBasesList(WizardParentForm, catalog, () => { return GetCatalogs(); });
			}
		}
		void cube_QueryPopUp(object sender, CancelEventArgs e) {
			if(string.IsNullOrEmpty(catalog.EditValue as string))
				return;
			if(cube.Properties.Items.Count == 0) {
				DbDataConnectionControlsHelper.FillDataBasesList(WizardParentForm, cube, () => { return GetCubes(); });
			}
		}
		void serverNameOrCredentialsChanged(object sender, EventArgs e) {
			ResetCatalog();
			ResetCubeName();
			ChangeConnectionName();
		}
		void localCubeFileOrCredentialsChanged(object sender, EventArgs e) {
			ResetCatalog();
			catalog.EditValue = GetCatalogs()[0];
			ResetCubeName();
			cube.EditValue = GetCubes()[0];
			ChangeConnectionName();
		}
		void databaseList_SelectedIndexChanged(object sender, EventArgs e) {
			ResetCubeName();
		}
		void localCubeFile_ButtonClick(object sender, XtraEditors.Controls.ButtonPressedEventArgs e) {
			if(openDialog.ShowDialog() == DialogResult.OK)
				LocalCubeFile = openDialog.FileName;
		}
		void SetVisibility(OlapConnectionParameterEdits key, bool visible) {
			BaseEdit control = editsMap[key];
			layoutControl1.GetItemByControl(control).Visibility = visible ? LayoutVisibility.Always : LayoutVisibility.OnlyInCustomization;
		}
		void connectionType_SelectedIndexChanged(object sender, EventArgs e) {
			ResetCatalog();
			ResetCubeName();
			switch(ConnectionType) {
				case Native.ConnectionType.Server:
					SetVisibility(OlapConnectionParameterEdits.ServerName, true);
					SetVisibility(OlapConnectionParameterEdits.LocalCubeFile, false);
					SetVisibility(OlapConnectionParameterEdits.ConnectionString, false);
					SetVisibility(OlapConnectionParameterEdits.UserName, true);
					SetVisibility(OlapConnectionParameterEdits.Password, true);
					SetVisibility(OlapConnectionParameterEdits.Catalog, true);
					SetVisibility(OlapConnectionParameterEdits.CubeName, true);
					break;
				case Native.ConnectionType.LocalCubeFile:
					SetVisibility(OlapConnectionParameterEdits.ServerName, false);
					SetVisibility(OlapConnectionParameterEdits.LocalCubeFile, true);
					SetVisibility(OlapConnectionParameterEdits.ConnectionString, false);
					SetVisibility(OlapConnectionParameterEdits.UserName, false);
					SetVisibility(OlapConnectionParameterEdits.Password, false);
					SetVisibility(OlapConnectionParameterEdits.Catalog, true);
					SetVisibility(OlapConnectionParameterEdits.CubeName, true);
					break;
				case Native.ConnectionType.CustomConnectionString:
					SetVisibility(OlapConnectionParameterEdits.ServerName, false);
					SetVisibility(OlapConnectionParameterEdits.LocalCubeFile, false);
					SetVisibility(OlapConnectionParameterEdits.ConnectionString, true);
					SetVisibility(OlapConnectionParameterEdits.UserName, false);
					SetVisibility(OlapConnectionParameterEdits.Password, false);
					SetVisibility(OlapConnectionParameterEdits.Catalog, false);
					SetVisibility(OlapConnectionParameterEdits.CubeName, false);					
					break;
			}
		}
		void cube_EditValueChanged(object sender, EventArgs e) {
			ChangeConnectionName();
		}
		void ResetCatalog() {
			catalog.Properties.Items.Clear();
			catalog.EditValue = null;
		}
		void ResetCubeName() {
			cube.Properties.Items.Clear();
			cube.EditValue = null;
		}
		string[] GetCatalogs() {
			AdomdMetaGetter metaGetter = null;
			try {
				metaGetter = new AdomdMetaGetter();
				metaGetter.Exception += OnAdoMetaGetterException;
				metaGetter.ConnectionString = AddAuth(string.Format("Data Source={0};", DataSource));
				return (metaGetter.GetCatalogs() ?? new List<string>()).ToArray();
			} finally {
				metaGetter.Dispose();
			}
		}
		string[] GetCubes() {
			AdomdMetaGetter metaGetter = null;
			try {
				metaGetter = new AdomdMetaGetter();
				metaGetter.Exception += OnAdoMetaGetterException;
				metaGetter.ConnectionString = AddAuth(string.Format("Data Source={0};Initial Catalog={1};", DataSource, Catalog));
				return (metaGetter.GetCubes((string)catalog.EditValue) ?? new List<string>()).ToArray();
			} finally {
				metaGetter.Dispose();
			}
		}
		public string GetConnectionString() {
			string connectionString = String.Empty;
			switch(ConnectionType) {
				case ConnectionType.Server:
				case ConnectionType.LocalCubeFile:
					Guard.ArgumentIsNotNullOrEmpty(DataSource, lciServerName.Text);
					Guard.ArgumentIsNotNullOrEmpty(Catalog, lciCatalog.Text);
					Guard.ArgumentIsNotNullOrEmpty(CubeName, lciCubeName.Text);
					connectionString = AddAuth(string.Format("Data Source={0};Initial Catalog={1};Timeout=1200;Cube Name={2};", DataSource, Catalog, CubeName));
					break;
				case ConnectionType.CustomConnectionString:
					Guard.ArgumentIsNotNullOrEmpty(CustomConnectionString, lciCustomConnectonString.Text);
					connectionString = CustomConnectionString;
					break;
			}
			return connectionString;
		}		
		public string AddAuth(string connectionString){
			string auth = string.Empty;
			if(!string.IsNullOrEmpty(userName.Text) && !string.IsNullOrEmpty(password.Text))
				auth = string.Format("User ID={0};Password=\"{1}\";", userName.Text, password.Text);
			return string.Format("{0}{1}", connectionString, auth);
		}
		internal void ApplyParameters(string connectionString) {
			OLAPConnectionStringBuilder builder = new OLAPConnectionStringBuilder(connectionString);
			userName.Text = builder.UserId;
			password.Text = builder.Password;
			serverName.EditValue = builder.ServerName;
			catalog.EditValue = builder.CatalogName;
			cube.EditValue = builder.CubeName;
		}
		internal string GenerateConnectionName() {
			return String.Format("{0}_{1}_{2}", ServerName, Catalog, CubeName);
		}
		protected override void ApplyParameters(DataConnectionParametersBase parameters){
			ApplyParameters(((OlapConnectionParameters)parameters).ConnectionString);
		}
		protected override DataConnectionParametersBase GetParameters() {
			return new OlapConnectionParameters(GetConnectionString());
		}
	}	
}

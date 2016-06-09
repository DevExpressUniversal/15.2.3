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
namespace DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls {
	public partial class ConnectionParametersControl : XtraUserControl, IConnectionParametersControl {
		readonly Dictionary<ConnectionParameterEdits, BaseEdit> editsMap;
		IConnectionParametersStrategy strategy;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<ProviderLookupItem> Providers {
			get {
				return providers;
			}
			set {
				providers = value;
				SuspendLayout();
				try {
					int idx = value.IndexOf(editProvider.SelectedItem as ProviderLookupItem);
					if(idx < 0 && value.Count > 0)
						idx = 0;
					editProvider.Properties.Items.Clear();
					editProvider.Properties.Items.AddRange(value);
					editProvider.SelectedIndex = idx;
				}
				finally {
					ResumeLayout();
				}
			}
		}
		string customConnectionName;
		HashSet<string> existingConnections;
		ReadOnlyCollection<ProviderLookupItem> providers;
		public ConnectionParametersControl() {
			InitializeComponent();
			LocalizeComponent();
			editsMap = new Dictionary<ConnectionParameterEdits, BaseEdit> {
				{ConnectionParameterEdits.ServerType, editServerbased},
				{ConnectionParameterEdits.FileName, editFileName},
				{ConnectionParameterEdits.ServerName, editServerName},
				{ConnectionParameterEdits.Port, editPort},
				{ConnectionParameterEdits.AuthTypeMsSql, editAuthTypeMsSql},
				{ConnectionParameterEdits.UserName, editUserName},
				{ConnectionParameterEdits.Password, editPassword},
				{ConnectionParameterEdits.Database, editDatabase},
				{ConnectionParameterEdits.ProjectID, editProjectID},
				{ConnectionParameterEdits.DataSetID, editDataSetID},
				{ConnectionParameterEdits.AuthTypeBigQuery, editAuthTypeBigQuery},
				{ConnectionParameterEdits.KeyFileName, editKeyFileName},
				{ConnectionParameterEdits.ServiceEmail, editServiceAccountEmail},
				{ConnectionParameterEdits.OAuthClientID, editOAuthClientID},
				{ConnectionParameterEdits.OAuthClientSecret, editOAuthClientSecret},
				{ConnectionParameterEdits.OAuthRefreshToken, editOAuthRefreshToken},
				{ConnectionParameterEdits.CustomString, editCustomString},
				{ConnectionParameterEdits.Hostname, this.editHostname},
				{ConnectionParameterEdits.AdvantageServerType, this.editAdvantageServerType}
			};
			Providers = ProviderLookupItem.GetPredefinedItems();
			this.existingConnections = new HashSet<string>();
		}
		public string ConnectionName {
			get {
				if(customConnectionName != null)
					return customConnectionName;
				string name = strategy == null ? "Connection" : strategy.GetConnectionName(this);
				return CreateUniqueName(name);
			}
			set { customConnectionName = value; }
		}
		bool ShouldSerializeConnectionName() { return customConnectionName != null; }
		public void SetExistingConnections(IEnumerable<INamedItem> connections) {
			existingConnections = new HashSet<string>(connections.Select(c => c.Name), StringComparer.Ordinal);
		}
		public void SetProvider(string providerKey) {
			ProviderLookupItem item = Providers.FirstOrDefault(p => p.ProviderKey == providerKey);
			if(item == null)
				throw new KeyNotFoundException(string.Format("Provider '{0}' has not been registered.", providerKey));
			editProvider.SelectedItem = item;
		}
#if DEBUGTEST
		internal string GetProvider() {
			var selectedItem = editProvider.SelectedItem as ProviderLookupItem;
			if(selectedItem == null)
				return null;
			return selectedItem.ProviderKey;
		}
#endif
		public DataConnectionParametersBase GetParameters() {
			if(strategy == null)
				return null;
			return strategy.GetConnectionParameters(this);
		}
		public void SetParameters(DataConnectionParametersBase value) {
			SetProvider(DataConnectionParametersRepository.Instance.GetKeyByItem(value));
			strategy.InitializeControl(this, value);
		}
		void ChangeStrategy(IConnectionParametersStrategy newStrategy) {
			Guard.ArgumentNotNull(newStrategy, "newStrategy");
			if(newStrategy == strategy)
				return;
			UnsubscribeStrategy();
			strategy = newStrategy;
			UpdateControlsValues();
			UpdateControlsVisibility();
			SubscribeStrategy();
			if(strategy.GetEditsSet(this).HasFlag(ConnectionParameterEdits.Database))
				editDatabase.Properties.Buttons[0].Visible = strategy.CanGetDatabases;
		}
		string CreateUniqueName(string core) {
			if(!existingConnections.Contains(core))
				return core;
			int index = 1;
			for(; ; ) {
				string name = string.Format("{0} {1}", core, index++);
				if(!existingConnections.Contains(name))
					return name;
			}
		}
		#region Implementation of IConnectionParametersControl
		public bool ServerBased {
			get { return editServerbased.SelectedIndex != 1; }
			set { editServerbased.SelectedIndex = value ? 0 : 1; }
		}
		[DefaultValue("")]
		public string FileName { get { return editFileName.Text; } set { editFileName.Text = value; } }
		[DefaultValue("localhost")]
		public string ServerName { get { return editServerName.Text; } set { editServerName.Text = value; } }
		[DefaultValue("")]
		public string Port { get { return editPort.Text; } set { editPort.Text = value; } }
		public string Hostname { get { return this.editHostname.Text; } set { this.editHostname.Text = value; } }
		public MsSqlAuthorizationType AuthTypeMsSql {
			get {
				return this.editAuthTypeMsSql.SelectedIndex == 1
					? MsSqlAuthorizationType.SqlServer
					: MsSqlAuthorizationType.Windows;
			}
			set {
				switch(value) {
					case MsSqlAuthorizationType.Windows:
						this.editAuthTypeMsSql.SelectedIndex = 0;
						break;
					case MsSqlAuthorizationType.SqlServer:
						this.editAuthTypeMsSql.SelectedIndex = 1;
						break;
					default:
						this.editAuthTypeMsSql.SelectedIndex = -1;
						break;
				}
			}
		}
		[DefaultValue("")]
		public string UserName { get { return editUserName.Text; } set { editUserName.Text = value; } }
		[DefaultValue("")]
		public string Password { get { return editPassword.Text; } set { editPassword.Text = value; } }
		[DefaultValue("")]
		public string Database { get { return editDatabase.Text; } set { editDatabase.Text = value; } }
		[DefaultValue("")]
		public string CustomString { get { return editCustomString.Text; } set { editCustomString.Text = value; } }
		[DefaultValue("")]
		public string ProjectID { get { return editProjectID.Text; } set { editProjectID.Text = value; } }
		[DefaultValue("")]
		public string DataSetID { get { return editDataSetID.Text; } set { editDataSetID.Text = value; } }
		public BigQueryAuthorizationType AuthTypeBigQuery {
			get {
				return editAuthTypeBigQuery.SelectedIndex == 0
					? BigQueryAuthorizationType.OAuth
					: BigQueryAuthorizationType.PrivateKeyFile;
			}
			set {
				switch(value) {
					case BigQueryAuthorizationType.OAuth:
						editAuthTypeBigQuery.SelectedIndex = 0;
						break;
					case BigQueryAuthorizationType.PrivateKeyFile:
						editAuthTypeBigQuery.SelectedIndex = 1;
						break;
					default:
						editAuthTypeBigQuery.SelectedIndex = -1;
						break;
				}
			}
		}
		[DefaultValue("")]
		public string KeyFileName { get { return editKeyFileName.Text; } set { editKeyFileName.Text = value; } }
		[DefaultValue("")]
		public string ServiceEmail { get { return editServiceAccountEmail.Text; } set { editServiceAccountEmail.Text = value; } }
		[DefaultValue("")]
		public string OAuthClientID { get { return editOAuthClientID.Text; } set { editOAuthClientID.Text = value; } }
		[DefaultValue("")]
		public string OAuthClientSecret { get { return editOAuthClientSecret.Text; } set { editOAuthClientSecret.Text = value; } }
		[DefaultValue("")]
		public string OAuthRefreshToken { get { return editOAuthRefreshToken.Text; } set { editOAuthRefreshToken.Text = value; } }
		public AdvantageServerType ServerTypeAdvantage {
			get {
				switch(this.editAdvantageServerType.SelectedIndex) {
					case 1:
						return AdvantageServerType.Remote;
					case 2:
						return AdvantageServerType.Internet;
					default:
						return AdvantageServerType.Local;
				}
			}
			set {
				switch(value) {
					case AdvantageServerType.Local:
						this.editAuthTypeBigQuery.SelectedIndex = 0;
						break;
					case AdvantageServerType.Remote:
						this.editAuthTypeBigQuery.SelectedIndex = 1;
						break;
					case AdvantageServerType.Internet:
						this.editAuthTypeBigQuery.SelectedIndex = 2;
						break;
					default:
						editAuthTypeBigQuery.SelectedIndex = -1;
						break;
				}
			}
		}
		#endregion
		void LocalizeComponent() {
			editAuthTypeBigQuery.Properties.Items[0] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AuthenticationType_BigQueryOAuth);
			editAuthTypeBigQuery.Properties.Items[1] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AuthenticationType_BigQueryKeyFile);
			editAuthTypeMsSql.Properties.Items[0] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AuthenticationType_MSSqlWindows);
			editAuthTypeMsSql.Properties.Items[1] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AuthenticationType_MSSqlServer);
			editServerbased.Properties.Items[0] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ServerTypeServer);
			editServerbased.Properties.Items[1] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ServerTypeEmbedded);
			lciProvider.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_Provider);
			lciFileName.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_Database);
			lciPassword.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_Password);
			lciUserName.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_UserName);
			lciServerName.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ServerName);
			lciPort.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_Port);
			lciDatabase.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_Database);
			lciCustomString.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ConnectionString);
			lciAuthType.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AuthenticationType);
			lciServerbased.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ServerType);
			lciProjectID.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ProjectID);
			lciKeyFileName.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_KeyFileName);
			lciServiceAccountEmail.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ServiceAccountEmail);
			lciOAuthClientID.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ClientID);
			lciOAuthClientSecret.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_ClientSecret);
			lciOAuthRefreshToken.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_RefreshToken);
			lciAuthTypeBigQuery.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AuthenticationType);
			lciDataSetID.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_DataSetID);
			lciHostname.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_Hostname);
			lciAdvantageServerType.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AdvantageServerType);
			editAdvantageServerType.Properties.Items[0] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AdvantageServerTypeLocal);
			editAdvantageServerType.Properties.Items[1] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AdvantageServerTypeRemote);
			editAdvantageServerType.Properties.Items[2] = DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionProperties_AdvantageServerTypeInternet);
		}
		private void editProvider_SelectedValueChanged(object sender, EventArgs e) {
			ProviderLookupItem item = editProvider.SelectedItem as ProviderLookupItem;
			if(item == null)
				return;
			ChangeStrategy(item.Strategy);
		}
		private void editFileName_ButtonClick(object sender, ButtonPressedEventArgs e) {
			openDialog.Filter = strategy.FileNameFilter;
			if(openDialog.ShowDialog(FindForm()) == DialogResult.OK) { editFileName.Text = openDialog.FileName; }
		}
		void editDatabase_QueryPopUp(object sender, CancelEventArgs e) {
			DbDataConnectionControlsHelper.FillDataBasesList(FindForm() as XtraForm, this.editDatabase, () => strategy.GetDatabases(this).ToArray());
		}
		void editDataSetID_QueryPopUp(object sender, CancelEventArgs e) {
			DbDataConnectionControlsHelper.FillDataBasesList(FindForm() as XtraForm, this.editDataSetID, () => strategy.GetDatabases(this).ToArray());
		}
		void UpdateControlsValues() {
			foreach(KeyValuePair<ConnectionParameterEdits, BaseEdit> pair in editsMap) {
				ConnectionParameterEdits key = pair.Key;
				BaseEdit control = pair.Value;
				ComboBoxEdit cmb = control as ComboBoxEdit;
				if(cmb != null)
					cmb.SelectedIndex = strategy.GetDefaultIndex(key);
				else
					control.Text = strategy.GetDefaultText(key);
			}
		}
		void UpdateControlsVisibility() {
			ConnectionParameterEdits editsSet = this.strategy.GetEditsSet(this);
			var disabledEdits = strategy.GetDisabledEdits(this);
			foreach(ConnectionParameterEdits key in this.editsMap.Keys) {
				BaseEdit control = this.editsMap[key];
				bool visible = editsSet.HasFlag(key);
				this.layoutControl1.GetItemByControl(control).Visibility = visible
					? LayoutVisibility.Always
					: LayoutVisibility.OnlyInCustomization;
				if(visible)
					control.Enabled = !disabledEdits.HasFlag(key);
			}
		}
		void SubscribeStrategy() {
			ConnectionParameterEdits mask = strategy.Subscriptions;
			if(mask == ConnectionParameterEdits.None)
				return;
			foreach(KeyValuePair<ConnectionParameterEdits, BaseEdit> pair in editsMap) {
				ConnectionParameterEdits key = pair.Key;
				if(!mask.HasFlag(key))
					continue;
				BaseEdit control = pair.Value;
				ComboBoxEdit comboBox = control as ComboBoxEdit;
				if(comboBox != null)
					comboBox.SelectedIndexChanged += OnEditChanged;
				else
					control.EditValueChanged += OnEditChanged;
			}
		}
		void UnsubscribeStrategy() {
			if(strategy == null)
				return;
			foreach(BaseEdit control in editsMap.Values) {
				ComboBoxEdit comboBox = control as ComboBoxEdit;
				if(comboBox != null)
					comboBox.SelectedIndexChanged -= OnEditChanged;
				else
					control.EditValueChanged -= OnEditChanged;
			}
		}
		void OnEditChanged(object sender, EventArgs e) {
			UpdateControlsVisibility();
		}
		void editKeyFileName_ButtonClick(object sender, ButtonPressedEventArgs e) {
			openDialog.Filter = "Key file|*.p12";
			if(openDialog.ShowDialog(FindForm()) == DialogResult.OK) {
				editKeyFileName.Text = openDialog.FileName;
			}
		}
	}
}

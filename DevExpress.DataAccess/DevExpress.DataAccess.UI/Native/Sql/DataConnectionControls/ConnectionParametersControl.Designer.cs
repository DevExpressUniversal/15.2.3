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

using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
namespace DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls {
	partial class ConnectionParametersControl {
		private IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.editAuthTypeBigQuery = new DevExpress.XtraEditors.ComboBoxEdit();
			this.editAuthTypeMsSql = new DevExpress.XtraEditors.ComboBoxEdit();
			this.editServerbased = new DevExpress.XtraEditors.ComboBoxEdit();
			this.editCustomString = new DevExpress.XtraEditors.MemoEdit();
			this.editPort = new DevExpress.XtraEditors.TextEdit();
			this.editOAuthRefreshToken = new DevExpress.XtraEditors.TextEdit();
			this.editOAuthClientSecret = new DevExpress.XtraEditors.TextEdit();
			this.editOAuthClientID = new DevExpress.XtraEditors.TextEdit();
			this.editServiceAccountEmail = new DevExpress.XtraEditors.TextEdit();
			this.editProjectID = new DevExpress.XtraEditors.TextEdit();
			this.editHostname = new DevExpress.XtraEditors.TextEdit();
			this.editServerName = new DevExpress.XtraEditors.TextEdit();
			this.editUserName = new DevExpress.XtraEditors.TextEdit();
			this.editPassword = new DevExpress.XtraEditors.TextEdit();
			this.editKeyFileName = new DevExpress.XtraEditors.ButtonEdit();
			this.editFileName = new DevExpress.XtraEditors.ButtonEdit();
			this.editProvider = new DevExpress.XtraEditors.ComboBoxEdit();
			this.editDataSetID = new DevExpress.XtraEditors.ComboBoxEdit();
			this.editDatabase = new DevExpress.XtraEditors.ComboBoxEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciProvider = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciFileName = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPassword = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciUserName = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciServerName = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPort = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciDatabase = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCustomString = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciAuthType = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciServerbased = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciProjectID = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciKeyFileName = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciServiceAccountEmail = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciOAuthClientID = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciOAuthClientSecret = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciOAuthRefreshToken = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciAuthTypeBigQuery = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciDataSetID = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciHostname = new DevExpress.XtraLayout.LayoutControlItem();
			this.openDialog = new System.Windows.Forms.OpenFileDialog();
			this.editAdvantageServerType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lciAdvantageServerType = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.editAuthTypeBigQuery.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editAuthTypeMsSql.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editServerbased.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editCustomString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editPort.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editOAuthRefreshToken.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editOAuthClientSecret.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editOAuthClientID.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editServiceAccountEmail.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editProjectID.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editHostname.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editServerName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editUserName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editPassword.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editKeyFileName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editFileName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editProvider.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editDataSetID.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editDatabase.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciProvider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFileName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPassword)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUserName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciServerName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPort)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDatabase)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCustomString)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAuthType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciServerbased)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciProjectID)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciKeyFileName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciServiceAccountEmail)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOAuthClientID)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOAuthClientSecret)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOAuthRefreshToken)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAuthTypeBigQuery)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDataSetID)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciHostname)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.editAdvantageServerType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAdvantageServerType)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.editAuthTypeBigQuery);
			this.layoutControl1.Controls.Add(this.editAuthTypeMsSql);
			this.layoutControl1.Controls.Add(this.editAdvantageServerType);
			this.layoutControl1.Controls.Add(this.editServerbased);
			this.layoutControl1.Controls.Add(this.editCustomString);
			this.layoutControl1.Controls.Add(this.editPort);
			this.layoutControl1.Controls.Add(this.editOAuthRefreshToken);
			this.layoutControl1.Controls.Add(this.editOAuthClientSecret);
			this.layoutControl1.Controls.Add(this.editOAuthClientID);
			this.layoutControl1.Controls.Add(this.editServiceAccountEmail);
			this.layoutControl1.Controls.Add(this.editProjectID);
			this.layoutControl1.Controls.Add(this.editHostname);
			this.layoutControl1.Controls.Add(this.editServerName);
			this.layoutControl1.Controls.Add(this.editUserName);
			this.layoutControl1.Controls.Add(this.editPassword);
			this.layoutControl1.Controls.Add(this.editKeyFileName);
			this.layoutControl1.Controls.Add(this.editFileName);
			this.layoutControl1.Controls.Add(this.editProvider);
			this.layoutControl1.Controls.Add(this.editDataSetID);
			this.layoutControl1.Controls.Add(this.editDatabase);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(525, 517);
			this.layoutControl1.TabIndex = 0;
			this.editAuthTypeBigQuery.Location = new System.Drawing.Point(122, 300);
			this.editAuthTypeBigQuery.Name = "editAuthTypeBigQuery";
			this.editAuthTypeBigQuery.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.editAuthTypeBigQuery.Properties.Items.AddRange(new object[] {
			"OAuth",
			"Key file"});
			this.editAuthTypeBigQuery.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.editAuthTypeBigQuery.Size = new System.Drawing.Size(391, 20);
			this.editAuthTypeBigQuery.StyleController = this.layoutControl1;
			this.editAuthTypeBigQuery.TabIndex = 11;
			this.editAuthTypeMsSql.Location = new System.Drawing.Point(122, 156);
			this.editAuthTypeMsSql.Name = "editAuthTypeMsSql";
			this.editAuthTypeMsSql.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.editAuthTypeMsSql.Properties.Items.AddRange(new object[] {
			"Windows authentication",
			"Server authentication"});
			this.editAuthTypeMsSql.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.editAuthTypeMsSql.Size = new System.Drawing.Size(391, 20);
			this.editAuthTypeMsSql.StyleController = this.layoutControl1;
			this.editAuthTypeMsSql.TabIndex = 11;
			this.editServerbased.Location = new System.Drawing.Point(122, 36);
			this.editServerbased.Name = "editServerbased";
			this.editServerbased.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.editServerbased.Properties.Items.AddRange(new object[] {
			"Server",
			"Embedded"});
			this.editServerbased.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.editServerbased.Size = new System.Drawing.Size(391, 20);
			this.editServerbased.StyleController = this.layoutControl1;
			this.editServerbased.TabIndex = 10;
			this.editCustomString.Location = new System.Drawing.Point(122, 468);
			this.editCustomString.Name = "editCustomString";
			this.editCustomString.Size = new System.Drawing.Size(391, 37);
			this.editCustomString.StyleController = this.layoutControl1;
			this.editCustomString.TabIndex = 9;
			this.editPort.Location = new System.Drawing.Point(122, 132);
			this.editPort.Name = "editPort";
			this.editPort.Size = new System.Drawing.Size(391, 20);
			this.editPort.StyleController = this.layoutControl1;
			this.editPort.TabIndex = 7;
			this.editOAuthRefreshToken.Location = new System.Drawing.Point(122, 420);
			this.editOAuthRefreshToken.Name = "editOAuthRefreshToken";
			this.editOAuthRefreshToken.Size = new System.Drawing.Size(391, 20);
			this.editOAuthRefreshToken.StyleController = this.layoutControl1;
			this.editOAuthRefreshToken.TabIndex = 6;
			this.editOAuthClientSecret.Location = new System.Drawing.Point(122, 396);
			this.editOAuthClientSecret.Name = "editOAuthClientSecret";
			this.editOAuthClientSecret.Size = new System.Drawing.Size(391, 20);
			this.editOAuthClientSecret.StyleController = this.layoutControl1;
			this.editOAuthClientSecret.TabIndex = 6;
			this.editOAuthClientID.Location = new System.Drawing.Point(122, 372);
			this.editOAuthClientID.Name = "editOAuthClientID";
			this.editOAuthClientID.Size = new System.Drawing.Size(391, 20);
			this.editOAuthClientID.StyleController = this.layoutControl1;
			this.editOAuthClientID.TabIndex = 6;
			this.editServiceAccountEmail.Location = new System.Drawing.Point(122, 348);
			this.editServiceAccountEmail.Name = "editServiceAccountEmail";
			this.editServiceAccountEmail.Size = new System.Drawing.Size(391, 20);
			this.editServiceAccountEmail.StyleController = this.layoutControl1;
			this.editServiceAccountEmail.TabIndex = 6;
			this.editProjectID.Location = new System.Drawing.Point(122, 276);
			this.editProjectID.Name = "editProjectID";
			this.editProjectID.Size = new System.Drawing.Size(391, 20);
			this.editProjectID.StyleController = this.layoutControl1;
			this.editProjectID.TabIndex = 6;
			this.editHostname.Location = new System.Drawing.Point(122, 84);
			this.editHostname.Name = "editHostname";
			this.editHostname.Size = new System.Drawing.Size(391, 20);
			this.editHostname.StyleController = this.layoutControl1;
			this.editHostname.TabIndex = 6;
			this.editServerName.Location = new System.Drawing.Point(122, 108);
			this.editServerName.Name = "editServerName";
			this.editServerName.Size = new System.Drawing.Size(391, 20);
			this.editServerName.StyleController = this.layoutControl1;
			this.editServerName.TabIndex = 6;
			this.editUserName.Location = new System.Drawing.Point(122, 180);
			this.editUserName.Name = "editUserName";
			this.editUserName.Size = new System.Drawing.Size(391, 20);
			this.editUserName.StyleController = this.layoutControl1;
			this.editUserName.TabIndex = 5;
			this.editPassword.Location = new System.Drawing.Point(122, 204);
			this.editPassword.Name = "editPassword";
			this.editPassword.Properties.UseSystemPasswordChar = true;
			this.editPassword.Size = new System.Drawing.Size(391, 20);
			this.editPassword.StyleController = this.layoutControl1;
			this.editPassword.TabIndex = 1;
			this.editKeyFileName.Location = new System.Drawing.Point(122, 324);
			this.editKeyFileName.Name = "editKeyFileName";
			this.editKeyFileName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.editKeyFileName.Size = new System.Drawing.Size(391, 20);
			this.editKeyFileName.StyleController = this.layoutControl1;
			this.editKeyFileName.TabIndex = 1;
			this.editKeyFileName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.editKeyFileName_ButtonClick);
			this.editFileName.Location = new System.Drawing.Point(122, 60);
			this.editFileName.Name = "editFileName";
			this.editFileName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.editFileName.Size = new System.Drawing.Size(391, 20);
			this.editFileName.StyleController = this.layoutControl1;
			this.editFileName.TabIndex = 1;
			this.editFileName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.editFileName_ButtonClick);
			this.editProvider.Location = new System.Drawing.Point(122, 12);
			this.editProvider.Name = "editProvider";
			this.editProvider.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.editProvider.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.editProvider.Size = new System.Drawing.Size(391, 20);
			this.editProvider.StyleController = this.layoutControl1;
			this.editProvider.TabIndex = 4;
			this.editProvider.SelectedValueChanged += new System.EventHandler(this.editProvider_SelectedValueChanged);
			this.editDataSetID.Location = new System.Drawing.Point(122, 444);
			this.editDataSetID.Name = "editDataSetID";
			this.editDataSetID.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.editDataSetID.Size = new System.Drawing.Size(391, 20);
			this.editDataSetID.StyleController = this.layoutControl1;
			this.editDataSetID.TabIndex = 8;
			this.editDataSetID.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.editDataSetID_QueryPopUp);
			this.editDatabase.Location = new System.Drawing.Point(122, 252);
			this.editDatabase.Name = "editDatabase";
			this.editDatabase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.editDatabase.Size = new System.Drawing.Size(391, 20);
			this.editDatabase.StyleController = this.layoutControl1;
			this.editDatabase.TabIndex = 8;
			this.editDatabase.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.editDatabase_QueryPopUp);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciProvider,
			this.lciFileName,
			this.lciPassword,
			this.lciUserName,
			this.lciServerName,
			this.lciPort,
			this.lciDatabase,
			this.lciCustomString,
			this.lciAuthType,
			this.lciServerbased,
			this.lciProjectID,
			this.lciKeyFileName,
			this.lciServiceAccountEmail,
			this.lciOAuthClientID,
			this.lciOAuthClientSecret,
			this.lciOAuthRefreshToken,
			this.lciAuthTypeBigQuery,
			this.lciDataSetID,
			this.lciHostname,
			this.lciAdvantageServerType});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(525, 517);
			this.layoutControlGroup1.TextVisible = false;
			this.lciProvider.Control = this.editProvider;
			this.lciProvider.Location = new System.Drawing.Point(0, 0);
			this.lciProvider.Name = "lciProvider";
			this.lciProvider.Size = new System.Drawing.Size(505, 24);
			this.lciProvider.Text = "Provider:";
			this.lciProvider.TextSize = new System.Drawing.Size(107, 13);
			this.lciFileName.Control = this.editFileName;
			this.lciFileName.Location = new System.Drawing.Point(0, 48);
			this.lciFileName.Name = "lciFileName";
			this.lciFileName.Size = new System.Drawing.Size(505, 24);
			this.lciFileName.Text = "Database:";
			this.lciFileName.TextSize = new System.Drawing.Size(107, 13);
			this.lciPassword.Control = this.editPassword;
			this.lciPassword.Location = new System.Drawing.Point(0, 192);
			this.lciPassword.Name = "lciPassword";
			this.lciPassword.Size = new System.Drawing.Size(505, 24);
			this.lciPassword.Text = "Password:";
			this.lciPassword.TextSize = new System.Drawing.Size(107, 13);
			this.lciUserName.Control = this.editUserName;
			this.lciUserName.Location = new System.Drawing.Point(0, 168);
			this.lciUserName.Name = "lciUserName";
			this.lciUserName.Size = new System.Drawing.Size(505, 24);
			this.lciUserName.Text = "User name:";
			this.lciUserName.TextSize = new System.Drawing.Size(107, 13);
			this.lciServerName.Control = this.editServerName;
			this.lciServerName.Location = new System.Drawing.Point(0, 96);
			this.lciServerName.Name = "lciServerName";
			this.lciServerName.Size = new System.Drawing.Size(505, 24);
			this.lciServerName.Text = "Server name:";
			this.lciServerName.TextSize = new System.Drawing.Size(107, 13);
			this.lciPort.Control = this.editPort;
			this.lciPort.Location = new System.Drawing.Point(0, 120);
			this.lciPort.Name = "lciPort";
			this.lciPort.Size = new System.Drawing.Size(505, 24);
			this.lciPort.Text = "Port:";
			this.lciPort.TextSize = new System.Drawing.Size(107, 13);
			this.lciDatabase.Control = this.editDatabase;
			this.lciDatabase.Location = new System.Drawing.Point(0, 240);
			this.lciDatabase.Name = "lciDatabase";
			this.lciDatabase.Size = new System.Drawing.Size(505, 24);
			this.lciDatabase.Text = "Database:";
			this.lciDatabase.TextSize = new System.Drawing.Size(107, 13);
			this.lciCustomString.Control = this.editCustomString;
			this.lciCustomString.Location = new System.Drawing.Point(0, 456);
			this.lciCustomString.Name = "lciCustomString";
			this.lciCustomString.Size = new System.Drawing.Size(505, 41);
			this.lciCustomString.Text = "Connection string:";
			this.lciCustomString.TextSize = new System.Drawing.Size(107, 13);
			this.lciAuthType.Control = this.editAuthTypeMsSql;
			this.lciAuthType.Location = new System.Drawing.Point(0, 144);
			this.lciAuthType.Name = "lciAuthType";
			this.lciAuthType.Size = new System.Drawing.Size(505, 24);
			this.lciAuthType.Text = "Authentication type:";
			this.lciAuthType.TextSize = new System.Drawing.Size(107, 13);
			this.lciServerbased.Control = this.editServerbased;
			this.lciServerbased.Location = new System.Drawing.Point(0, 24);
			this.lciServerbased.Name = "lciServerbased";
			this.lciServerbased.Size = new System.Drawing.Size(505, 24);
			this.lciServerbased.Text = "Server type:";
			this.lciServerbased.TextSize = new System.Drawing.Size(107, 13);
			this.lciProjectID.Control = this.editProjectID;
			this.lciProjectID.Location = new System.Drawing.Point(0, 264);
			this.lciProjectID.Name = "lciProjectID";
			this.lciProjectID.Size = new System.Drawing.Size(505, 24);
			this.lciProjectID.Text = "Project ID:";
			this.lciProjectID.TextSize = new System.Drawing.Size(107, 13);
			this.lciKeyFileName.Control = this.editKeyFileName;
			this.lciKeyFileName.Location = new System.Drawing.Point(0, 312);
			this.lciKeyFileName.Name = "lciKeyFileName";
			this.lciKeyFileName.Size = new System.Drawing.Size(505, 24);
			this.lciKeyFileName.Text = "Key file name:";
			this.lciKeyFileName.TextSize = new System.Drawing.Size(107, 13);
			this.lciServiceAccountEmail.Control = this.editServiceAccountEmail;
			this.lciServiceAccountEmail.Location = new System.Drawing.Point(0, 336);
			this.lciServiceAccountEmail.Name = "lciServiceAccountEmail";
			this.lciServiceAccountEmail.Size = new System.Drawing.Size(505, 24);
			this.lciServiceAccountEmail.Text = "Service account email:";
			this.lciServiceAccountEmail.TextSize = new System.Drawing.Size(107, 13);
			this.lciOAuthClientID.Control = this.editOAuthClientID;
			this.lciOAuthClientID.Location = new System.Drawing.Point(0, 360);
			this.lciOAuthClientID.Name = "lciOAuthClientID";
			this.lciOAuthClientID.Size = new System.Drawing.Size(505, 24);
			this.lciOAuthClientID.Text = "Client ID:";
			this.lciOAuthClientID.TextSize = new System.Drawing.Size(107, 13);
			this.lciOAuthClientSecret.Control = this.editOAuthClientSecret;
			this.lciOAuthClientSecret.Location = new System.Drawing.Point(0, 384);
			this.lciOAuthClientSecret.Name = "lciOAuthClientSecret";
			this.lciOAuthClientSecret.Size = new System.Drawing.Size(505, 24);
			this.lciOAuthClientSecret.Text = "Client Secret:";
			this.lciOAuthClientSecret.TextSize = new System.Drawing.Size(107, 13);
			this.lciOAuthRefreshToken.Control = this.editOAuthRefreshToken;
			this.lciOAuthRefreshToken.Location = new System.Drawing.Point(0, 408);
			this.lciOAuthRefreshToken.Name = "lciOAuthRefreshToken";
			this.lciOAuthRefreshToken.Size = new System.Drawing.Size(505, 24);
			this.lciOAuthRefreshToken.Text = "Refresh Token:";
			this.lciOAuthRefreshToken.TextSize = new System.Drawing.Size(107, 13);
			this.lciAuthTypeBigQuery.Control = this.editAuthTypeBigQuery;
			this.lciAuthTypeBigQuery.Location = new System.Drawing.Point(0, 288);
			this.lciAuthTypeBigQuery.Name = "lciAuthTypeBigQuery";
			this.lciAuthTypeBigQuery.Size = new System.Drawing.Size(505, 24);
			this.lciAuthTypeBigQuery.Text = "Authentication type:";
			this.lciAuthTypeBigQuery.TextSize = new System.Drawing.Size(107, 13);
			this.lciDataSetID.Control = this.editDataSetID;
			this.lciDataSetID.Location = new System.Drawing.Point(0, 432);
			this.lciDataSetID.Name = "lciDataSetID";
			this.lciDataSetID.Size = new System.Drawing.Size(505, 24);
			this.lciDataSetID.Text = "DataSet ID:";
			this.lciDataSetID.TextSize = new System.Drawing.Size(107, 13);
			this.lciHostname.Control = this.editHostname;
			this.lciHostname.Location = new System.Drawing.Point(0, 72);
			this.lciHostname.Name = "lciHostname";
			this.lciHostname.Size = new System.Drawing.Size(505, 24);
			this.lciHostname.Text = "Hostname:";
			this.lciHostname.TextSize = new System.Drawing.Size(107, 13);
			this.editAdvantageServerType.Location = new System.Drawing.Point(122, 228);
			this.editAdvantageServerType.Name = "editAdvantageServerType";
			this.editAdvantageServerType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.editAdvantageServerType.Properties.Items.AddRange(new object[] {
			"Local",
			"Remote",
			"Internet"});
			this.editAdvantageServerType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.editAdvantageServerType.Size = new System.Drawing.Size(391, 20);
			this.editAdvantageServerType.StyleController = this.layoutControl1;
			this.editAdvantageServerType.TabIndex = 10;
			this.lciAdvantageServerType.Control = this.editAdvantageServerType;
			this.lciAdvantageServerType.Location = new System.Drawing.Point(0, 216);
			this.lciAdvantageServerType.Name = "lciAdvantageServerType";
			this.lciAdvantageServerType.Size = new System.Drawing.Size(505, 24);
			this.lciAdvantageServerType.Text = "Server type:";
			this.lciAdvantageServerType.TextSize = new System.Drawing.Size(107, 13);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.layoutControl1);
			this.Name = "ConnectionParametersControl";
			this.Size = new System.Drawing.Size(525, 517);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.editAuthTypeBigQuery.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editAuthTypeMsSql.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editServerbased.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editCustomString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editPort.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editOAuthRefreshToken.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editOAuthClientSecret.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editOAuthClientID.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editServiceAccountEmail.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editProjectID.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editHostname.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editServerName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editUserName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editPassword.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editKeyFileName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editFileName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editProvider.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editDataSetID.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editDatabase.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciProvider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFileName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPassword)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUserName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciServerName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPort)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDatabase)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCustomString)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAuthType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciServerbased)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciProjectID)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciKeyFileName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciServiceAccountEmail)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOAuthClientID)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOAuthClientSecret)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOAuthRefreshToken)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAuthTypeBigQuery)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDataSetID)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciHostname)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.editAdvantageServerType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciAdvantageServerType)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private LayoutControl layoutControl1;
		private LayoutControlGroup layoutControlGroup1;
		private ComboBoxEdit editProvider;
		private LayoutControlItem lciProvider;
		private ButtonEdit editFileName;
		private LayoutControlItem lciFileName;
		private TextEdit editPassword;
		private LayoutControlItem lciPassword;
		private TextEdit editUserName;
		private LayoutControlItem lciUserName;
		private TextEdit editServerName;
		private LayoutControlItem lciServerName;
		private TextEdit editPort;
		private LayoutControlItem lciPort;
		private LayoutControlItem lciDatabase;
		private MemoEdit editCustomString;
		private LayoutControlItem lciCustomString;
		private ComboBoxEdit editServerbased;
		private LayoutControlItem lciServerbased;
		private ComboBoxEdit editAuthTypeMsSql;
		private LayoutControlItem lciAuthType;
		private OpenFileDialog openDialog;
		private ComboBoxEdit editDatabase;
		private TextEdit editProjectID;
		private ComboBoxEdit editDataSetID;
		private LayoutControlItem lciProjectID;
		private LayoutControlItem lciDataSetID;
		private TextEdit editOAuthRefreshToken;
		private TextEdit editOAuthClientSecret;
		private TextEdit editOAuthClientID;
		private TextEdit editServiceAccountEmail;
		private ButtonEdit editKeyFileName;
		private LayoutControlItem lciKeyFileName;
		private LayoutControlItem lciServiceAccountEmail;
		private LayoutControlItem lciOAuthClientID;
		private LayoutControlItem lciOAuthClientSecret;
		private LayoutControlItem lciOAuthRefreshToken;
		private ComboBoxEdit editAuthTypeBigQuery;
		private LayoutControlItem lciAuthTypeBigQuery;
		private TextEdit editHostname;
		private LayoutControlItem lciHostname;
		private ComboBoxEdit editAdvantageServerType;
		private LayoutControlItem lciAdvantageServerType;
	}
}

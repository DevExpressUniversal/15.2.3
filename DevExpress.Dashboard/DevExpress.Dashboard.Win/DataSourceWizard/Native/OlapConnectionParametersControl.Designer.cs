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

using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
namespace DevExpress.DashboardWin.Native {
	partial class OlapConnectionparametersControl {
		private IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OlapConnectionparametersControl));
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.cube = new DevExpress.XtraEditors.ComboBoxEdit();
			this.connectionType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.customConnectionString = new DevExpress.XtraEditors.MemoEdit();
			this.serverName = new DevExpress.XtraEditors.TextEdit();
			this.userName = new DevExpress.XtraEditors.TextEdit();
			this.password = new DevExpress.XtraEditors.TextEdit();
			this.localCubeFile = new DevExpress.XtraEditors.ButtonEdit();
			this.catalog = new DevExpress.XtraEditors.ComboBoxEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciServerName = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCatalog = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciConnectionType = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciUserName = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPassword = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciLocalCubeFile = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCustomConnectonString = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCubeName = new DevExpress.XtraLayout.LayoutControlItem();
			this.openDialog = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cube.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.connectionType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.customConnectionString.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.serverName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.userName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.password.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.localCubeFile.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.catalog.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciServerName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCatalog)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciConnectionType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUserName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPassword)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciLocalCubeFile)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCustomConnectonString)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCubeName)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.cube);
			this.layoutControl1.Controls.Add(this.connectionType);
			this.layoutControl1.Controls.Add(this.customConnectionString);
			this.layoutControl1.Controls.Add(this.serverName);
			this.layoutControl1.Controls.Add(this.userName);
			this.layoutControl1.Controls.Add(this.password);
			this.layoutControl1.Controls.Add(this.localCubeFile);
			this.layoutControl1.Controls.Add(this.catalog);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.cube, "cube");
			this.cube.Name = "cube";
			this.cube.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cube.Properties.Buttons"))))});
			this.cube.Properties.Items.AddRange(new object[] {
			resources.GetString("cube.Properties.Items"),
			resources.GetString("cube.Properties.Items1")});
			this.cube.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cube.StyleController = this.layoutControl1;
			this.cube.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.cube_QueryPopUp);
			this.cube.EditValueChanged += new System.EventHandler(this.cube_EditValueChanged);
			resources.ApplyResources(this.connectionType, "connectionType");
			this.connectionType.Name = "connectionType";
			this.connectionType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("connectionType.Properties.Buttons"))))});
			this.connectionType.Properties.Items.AddRange(new object[] {
			resources.GetString("connectionType.Properties.Items"),
			resources.GetString("connectionType.Properties.Items1"),
			resources.GetString("connectionType.Properties.Items2")});
			this.connectionType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.connectionType.StyleController = this.layoutControl1;
			this.connectionType.SelectedIndexChanged += new System.EventHandler(this.connectionType_SelectedIndexChanged);
			resources.ApplyResources(this.customConnectionString, "customConnectionString");
			this.customConnectionString.Name = "customConnectionString";
			this.customConnectionString.StyleController = this.layoutControl1;
			resources.ApplyResources(this.serverName, "serverName");
			this.serverName.Name = "serverName";
			this.serverName.StyleController = this.layoutControl1;
			resources.ApplyResources(this.userName, "userName");
			this.userName.Name = "userName";
			this.userName.StyleController = this.layoutControl1;
			resources.ApplyResources(this.password, "password");
			this.password.Name = "password";
			this.password.Properties.UseSystemPasswordChar = true;
			this.password.StyleController = this.layoutControl1;
			resources.ApplyResources(this.localCubeFile, "localCubeFile");
			this.localCubeFile.Name = "localCubeFile";
			this.localCubeFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.localCubeFile.StyleController = this.layoutControl1;
			this.localCubeFile.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.localCubeFile_ButtonClick);
			resources.ApplyResources(this.catalog, "catalog");
			this.catalog.Name = "catalog";
			this.catalog.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("catalog.Properties.Buttons"))))});
			this.catalog.StyleController = this.layoutControl1;
			this.catalog.SelectedIndexChanged += new System.EventHandler(this.databaseList_SelectedIndexChanged);
			this.catalog.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.database_QueryPopUp);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciServerName,
			this.lciCatalog,
			this.lciConnectionType,
			this.lciUserName,
			this.lciPassword,
			this.lciLocalCubeFile,
			this.lciCustomConnectonString,
			this.lciCubeName});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Size = new System.Drawing.Size(525, 277);
			this.layoutControlGroup1.TextVisible = false;
			this.lciServerName.Control = this.serverName;
			this.lciServerName.Location = new System.Drawing.Point(0, 24);
			this.lciServerName.Name = "lciServerName";
			this.lciServerName.Size = new System.Drawing.Size(505, 24);
			resources.ApplyResources(this.lciServerName, "lciServerName");
			this.lciServerName.TextSize = new System.Drawing.Size(88, 13);
			this.lciCatalog.Control = this.catalog;
			resources.ApplyResources(this.lciCatalog, "lciCatalog");
			this.lciCatalog.Location = new System.Drawing.Point(0, 209);
			this.lciCatalog.Name = "lciDatabase";
			this.lciCatalog.Size = new System.Drawing.Size(505, 24);
			this.lciCatalog.TextSize = new System.Drawing.Size(88, 13);
			this.lciConnectionType.Control = this.connectionType;
			resources.ApplyResources(this.lciConnectionType, "lciConnectionType");
			this.lciConnectionType.Location = new System.Drawing.Point(0, 0);
			this.lciConnectionType.Name = "lciServerbased";
			this.lciConnectionType.Size = new System.Drawing.Size(505, 24);
			this.lciConnectionType.TextSize = new System.Drawing.Size(88, 13);
			this.lciUserName.Control = this.userName;
			this.lciUserName.Location = new System.Drawing.Point(0, 48);
			this.lciUserName.Name = "lciUserName";
			this.lciUserName.Size = new System.Drawing.Size(505, 24);
			resources.ApplyResources(this.lciUserName, "lciUserName");
			this.lciUserName.TextSize = new System.Drawing.Size(88, 13);
			this.lciPassword.Control = this.password;
			this.lciPassword.Location = new System.Drawing.Point(0, 72);
			this.lciPassword.Name = "lciPassword";
			this.lciPassword.Size = new System.Drawing.Size(505, 24);
			resources.ApplyResources(this.lciPassword, "lciPassword");
			this.lciPassword.TextSize = new System.Drawing.Size(88, 13);
			this.lciLocalCubeFile.Control = this.localCubeFile;
			resources.ApplyResources(this.lciLocalCubeFile, "lciLocalCubeFile");
			this.lciLocalCubeFile.Location = new System.Drawing.Point(0, 96);
			this.lciLocalCubeFile.Name = "lciFileName";
			this.lciLocalCubeFile.Size = new System.Drawing.Size(505, 24);
			this.lciLocalCubeFile.TextSize = new System.Drawing.Size(88, 13);
			this.lciCustomConnectonString.Control = this.customConnectionString;
			this.lciCustomConnectonString.Location = new System.Drawing.Point(0, 120);
			this.lciCustomConnectonString.Name = "lciCustomString";
			this.lciCustomConnectonString.Size = new System.Drawing.Size(505, 89);
			resources.ApplyResources(this.lciCustomConnectonString, "lciCustomConnectonString");
			this.lciCustomConnectonString.TextSize = new System.Drawing.Size(88, 13);
			this.lciCubeName.Control = this.cube;
			resources.ApplyResources(this.lciCubeName, "lciCubeName");
			this.lciCubeName.Location = new System.Drawing.Point(0, 233);
			this.lciCubeName.Name = "lciAuthType";
			this.lciCubeName.Size = new System.Drawing.Size(505, 24);
			this.lciCubeName.TextSize = new System.Drawing.Size(88, 13);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "OlapConnectionparametersControl";
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cube.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.connectionType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.customConnectionString.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.serverName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.userName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.password.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.localCubeFile.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.catalog.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciServerName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCatalog)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciConnectionType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUserName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPassword)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciLocalCubeFile)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCustomConnectonString)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCubeName)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private LayoutControl layoutControl1;
		private LayoutControlGroup layoutControlGroup1;
		private ButtonEdit localCubeFile;
		private LayoutControlItem lciLocalCubeFile;
		private TextEdit password;
		private LayoutControlItem lciPassword;
		private TextEdit userName;
		private LayoutControlItem lciUserName;
		private TextEdit serverName;
		private LayoutControlItem lciServerName;
		private LayoutControlItem lciCatalog;
		private MemoEdit customConnectionString;
		private LayoutControlItem lciCustomConnectonString;
		private ComboBoxEdit connectionType;
		private LayoutControlItem lciConnectionType;
		private ComboBoxEdit cube;
		private LayoutControlItem lciCubeName;
		private OpenFileDialog openDialog;
		private ComboBoxEdit catalog;
	}
}

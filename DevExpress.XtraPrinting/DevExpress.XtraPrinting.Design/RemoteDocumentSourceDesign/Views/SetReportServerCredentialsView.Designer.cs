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

namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views {
	partial class SetReportServerCredentialsView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.userNameEdit = new DevExpress.XtraEditors.TextEdit();
			this.passwordEdit = new DevExpress.XtraEditors.TextEdit();
			this.uriEdit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.linkLabel1 = new DevExpress.XtraEditors.HyperLinkEdit();
			this.authenticationEdit = new DevExpress.XtraEditors.LookUpEdit();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.userNameEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.passwordEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.uriEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.linkLabel1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.authenticationEdit.Properties)).BeginInit();
			this.SuspendLayout();
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.Controls.Add(this.labelControl1, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.labelControl2, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.labelControl3, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.labelControl4, 1, 5);
			this.tableLayoutPanel1.Controls.Add(this.userNameEdit, 2, 4);
			this.tableLayoutPanel1.Controls.Add(this.passwordEdit, 2, 5);
			this.tableLayoutPanel1.Controls.Add(this.uriEdit, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.linkLabel1, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.authenticationEdit, 2, 3);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 7;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(701, 360);
			this.tableLayoutPanel1.TabIndex = 0;
			this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11F);
			this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelControl1.Location = new System.Drawing.Point(110, 112);
			this.labelControl1.Margin = new System.Windows.Forms.Padding(0);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Padding = new System.Windows.Forms.Padding(0, 0, 24, 0);
			this.labelControl1.Size = new System.Drawing.Size(147, 28);
			this.labelControl1.TabIndex = 0;
			this.labelControl1.Text = "Server address:";
			this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11F);
			this.labelControl2.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelControl2.Location = new System.Drawing.Point(110, 164);
			this.labelControl2.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Padding = new System.Windows.Forms.Padding(0, 0, 24, 0);
			this.labelControl2.Size = new System.Drawing.Size(147, 28);
			this.labelControl2.TabIndex = 0;
			this.labelControl2.Text = "Authentication type:";
			this.labelControl3.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11F);
			this.labelControl3.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelControl3.Location = new System.Drawing.Point(110, 192);
			this.labelControl3.Margin = new System.Windows.Forms.Padding(0);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Padding = new System.Windows.Forms.Padding(0, 0, 24, 0);
			this.labelControl3.Size = new System.Drawing.Size(147, 28);
			this.labelControl3.TabIndex = 0;
			this.labelControl3.Text = "User name:";
			this.labelControl4.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11F);
			this.labelControl4.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl4.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelControl4.Location = new System.Drawing.Point(110, 220);
			this.labelControl4.Margin = new System.Windows.Forms.Padding(0);
			this.labelControl4.Name = "labelControl4";
			this.labelControl4.Padding = new System.Windows.Forms.Padding(0, 0, 24, 0);
			this.labelControl4.Size = new System.Drawing.Size(147, 28);
			this.labelControl4.TabIndex = 0;
			this.labelControl4.Text = "Password:";
			this.userNameEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.userNameEdit.EditValue = "";
			this.userNameEdit.Location = new System.Drawing.Point(257, 195);
			this.userNameEdit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.userNameEdit.Name = "userNameEdit";
			this.userNameEdit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.userNameEdit.Properties.Appearance.Options.UseFont = true;
			this.userNameEdit.Size = new System.Drawing.Size(332, 22);
			this.userNameEdit.TabIndex = 4;
			this.userNameEdit.TextChanged += new System.EventHandler(this.userNameEdit_TextChanged);
			this.passwordEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.passwordEdit.EditValue = "";
			this.passwordEdit.Location = new System.Drawing.Point(257, 223);
			this.passwordEdit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.passwordEdit.Name = "passwordEdit";
			this.passwordEdit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.passwordEdit.Properties.Appearance.Options.UseFont = true;
			this.passwordEdit.Properties.PasswordChar = '*';
			this.passwordEdit.Size = new System.Drawing.Size(332, 22);
			this.passwordEdit.TabIndex = 5;
			this.passwordEdit.TextChanged += new System.EventHandler(this.passwordEdit_TextChanged);
			this.uriEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.uriEdit.EditValue = "";
			this.uriEdit.Location = new System.Drawing.Point(257, 115);
			this.uriEdit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.uriEdit.Name = "uriEdit";
			this.uriEdit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.uriEdit.Properties.Appearance.Options.UseFont = true;
			this.uriEdit.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Segoe UI Light", 10F);
			this.uriEdit.Properties.AppearanceDropDown.Options.UseFont = true;
			this.uriEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.uriEdit.Size = new System.Drawing.Size(332, 22);
			this.uriEdit.TabIndex = 1;
			this.uriEdit.TextChanged += new System.EventHandler(this.uriEdit_TextChanged);
			this.uriEdit.Validated += new System.EventHandler(this.uriEdit_Validated);
			this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.linkLabel1.EditValue = "";
			this.linkLabel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.linkLabel1.Location = new System.Drawing.Point(257, 143);
			this.linkLabel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.linkLabel1.Properties.Appearance.ForeColor = System.Drawing.Color.Orange;
			this.linkLabel1.Properties.Appearance.Options.UseBackColor = true;
			this.linkLabel1.Properties.Appearance.Options.UseForeColor = true;
			this.linkLabel1.Properties.AppearanceDisabled.ForeColor = System.Drawing.Color.Gray;
			this.linkLabel1.Properties.AppearanceDisabled.Options.UseForeColor = true;
			this.linkLabel1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.linkLabel1.Properties.Caption = "Open this link in a browser";
			this.linkLabel1.Properties.StartLinkOnClickingEmptySpace = false;
			this.linkLabel1.Size = new System.Drawing.Size(332, 18);
			this.linkLabel1.TabIndex = 2;
			this.linkLabel1.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(this.linkLabel1_OpenLink);
			this.authenticationEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.authenticationEdit.EditValue = "";
			this.authenticationEdit.Location = new System.Drawing.Point(257, 167);
			this.authenticationEdit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.authenticationEdit.Name = "authenticationEdit";
			this.authenticationEdit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.authenticationEdit.Properties.Appearance.Options.UseFont = true;
			this.authenticationEdit.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Segoe UI Light", 10F);
			this.authenticationEdit.Properties.AppearanceDropDown.Options.UseFont = true;
			this.authenticationEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.authenticationEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Authentication", 20, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.False)});
			this.authenticationEdit.Properties.DisplayMember = "Value";
			this.authenticationEdit.Properties.DropDownRows = 3;
			this.authenticationEdit.Properties.NullText = "";
			this.authenticationEdit.Properties.PopupSizeable = false;
			this.authenticationEdit.Properties.ShowFooter = false;
			this.authenticationEdit.Properties.ShowHeader = false;
			this.authenticationEdit.Properties.ShowLines = false;
			this.authenticationEdit.Properties.ValueMember = "Key";
			this.authenticationEdit.Size = new System.Drawing.Size(332, 22);
			this.authenticationEdit.TabIndex = 3;
			this.authenticationEdit.EditValueChanged += new System.EventHandler(this.authenticationEdit_EditValueChanged);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SetReportServerCredentialsView";
			this.Size = new System.Drawing.Size(701, 360);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.userNameEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.passwordEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.uriEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.linkLabel1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.authenticationEdit.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.LabelControl labelControl4;
		private XtraEditors.TextEdit userNameEdit;
		private XtraEditors.TextEdit passwordEdit;
		private XtraEditors.ComboBoxEdit uriEdit;
		private DevExpress.XtraEditors.HyperLinkEdit linkLabel1;
		private XtraEditors.LookUpEdit authenticationEdit;
	}
}

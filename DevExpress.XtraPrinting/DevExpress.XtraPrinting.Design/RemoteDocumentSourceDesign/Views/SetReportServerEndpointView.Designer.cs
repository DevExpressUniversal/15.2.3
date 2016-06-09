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
	partial class SetReportServerEndpointView {
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
			this.endpointEdit = new DevExpress.XtraEditors.ComboBoxEdit();
			this.generateEndpoints = new DevExpress.XtraEditors.CheckEdit();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.endpointEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.generateEndpoints.Properties)).BeginInit();
			this.SuspendLayout();
			this.tableLayoutPanel1.ColumnCount = 4;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.tableLayoutPanel1.Controls.Add(this.labelControl1, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.endpointEdit, 2, 2);
			this.tableLayoutPanel1.Controls.Add(this.generateEndpoints, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(701, 360);
			this.tableLayoutPanel1.TabIndex = 0;
			this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11F);
			this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelControl1.Location = new System.Drawing.Point(111, 138);
			this.labelControl1.Margin = new System.Windows.Forms.Padding(0);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Padding = new System.Windows.Forms.Padding(3, 0, 24, 0);
			this.labelControl1.Size = new System.Drawing.Size(144, 30);
			this.labelControl1.TabIndex = 0;
			this.labelControl1.Text = "Endpoint:";
			this.endpointEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.endpointEdit.EditValue = "";
			this.endpointEdit.Location = new System.Drawing.Point(255, 141);
			this.endpointEdit.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.endpointEdit.Name = "endpointEdit";
			this.endpointEdit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 9F);
			this.endpointEdit.Properties.Appearance.Options.UseFont = true;
			this.endpointEdit.Properties.AppearanceDropDown.Font = new System.Drawing.Font("Segoe UI Light", 10F);
			this.endpointEdit.Properties.AppearanceDropDown.Options.UseFont = true;
			this.endpointEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.endpointEdit.Size = new System.Drawing.Size(334, 22);
			this.endpointEdit.TabIndex = 1;
			this.endpointEdit.TextChanged += new System.EventHandler(this.endpointEdit_TextChanged);
			this.tableLayoutPanel1.SetColumnSpan(this.generateEndpoints, 2);
			this.generateEndpoints.Dock = System.Windows.Forms.DockStyle.Fill;
			this.generateEndpoints.EditValue = true;
			this.generateEndpoints.Location = new System.Drawing.Point(111, 106);
			this.generateEndpoints.Margin = new System.Windows.Forms.Padding(0, 3, 0, 12);
			this.generateEndpoints.Name = "generateEndpoints";
			this.generateEndpoints.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11F);
			this.generateEndpoints.Properties.Appearance.Options.UseFont = true;
			this.generateEndpoints.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
			this.generateEndpoints.Properties.Caption = "Generate endpoint configurations in App.config file";
			this.generateEndpoints.Size = new System.Drawing.Size(478, 20);
			this.generateEndpoints.TabIndex = 3;
			this.generateEndpoints.CheckedChanged += new System.EventHandler(this.generateEndpoints_CheckedChanged);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SetReportServerEndpointView";
			this.Size = new System.Drawing.Size(701, 360);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.endpointEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.generateEndpoints.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.ComboBoxEdit endpointEdit;
		private XtraEditors.CheckEdit generateEndpoints;
	}
}

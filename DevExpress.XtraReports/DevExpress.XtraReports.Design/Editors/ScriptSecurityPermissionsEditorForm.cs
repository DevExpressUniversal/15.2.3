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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Design;
namespace DevExpress.XtraReports.Design
{
	public class ScriptSecurityPermissionsEditorForm : XtraForm 
	{
		private DevExpress.XtraEditors.SimpleButton buttonCancel;
		private DevExpress.XtraEditors.SimpleButton buttonAdd;
		private DevExpress.XtraEditors.SimpleButton buttonRemove;
		private System.Windows.Forms.Panel panel1;
		private DevExpress.Utils.Frames.PropertyGridEx propertyGrid;
		private System.ComponentModel.Container components = null;
		private DevExpress.XtraEditors.ListBoxControl lbSecurityPermissions;
		ScriptSecurityPermissionCollection scriptSecurityPermissions;
		public ScriptSecurityPermissionCollection EditValue {
			get { return scriptSecurityPermissions; }
			set { 
				scriptSecurityPermissions = value; 
				if (scriptSecurityPermissions.Count > 0) {
					foreach (ScriptSecurityPermission permission in scriptSecurityPermissions)
						lbSecurityPermissions.Items.Add(permission);
					lbSecurityPermissions.SelectedIndex = 0;
				}
			}
		}
		public ScriptSecurityPermissionsEditorForm(IServiceProvider servProvider) {
			InitializeComponent();
			propertyGrid.Site = new MySite(servProvider, propertyGrid);
			propertyGrid.CommandsVisibleIfAvailable = false;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.buttonAdd = new DevExpress.XtraEditors.SimpleButton();
			this.buttonRemove = new DevExpress.XtraEditors.SimpleButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.propertyGrid = new DevExpress.Utils.Frames.PropertyGridEx();
			this.lbSecurityPermissions = new DevExpress.XtraEditors.ListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.lbSecurityPermissions)).BeginInit();
			this.SuspendLayout();
			this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonCancel.Location = new System.Drawing.Point(404, 297);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 24);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Close";
			this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.buttonAdd.Location = new System.Drawing.Point(24, 257);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(80, 24);
			this.buttonAdd.TabIndex = 2;
			this.buttonAdd.Text = "&Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Location = new System.Drawing.Point(128, 257);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(80, 24);
			this.buttonRemove.TabIndex = 3;
			this.buttonRemove.Text = "&Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Location = new System.Drawing.Point(8, 289);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(476, 4);
			this.panel1.TabIndex = 6;
			this.propertyGrid.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.DrawFlat = true;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(232, 8);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(248, 273);
			this.propertyGrid.TabIndex = 4;
			this.propertyGrid.Text = "propertyGrid1";
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			this.lbSecurityPermissions.Location = new System.Drawing.Point(8, 8);
			this.lbSecurityPermissions.Name = "lbSecurityPermissions";
			this.lbSecurityPermissions.Size = new System.Drawing.Size(216, 241);
			this.lbSecurityPermissions.TabIndex = 7;
			this.lbSecurityPermissions.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
			this.lbSecurityPermissions.SelectedIndexChanged += new System.EventHandler(this.lbSecurityAttributes_SelectedIndexChanged);
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(492, 326);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lbSecurityPermissions,
																		  this.propertyGrid,
																		  this.panel1,
																		  this.buttonRemove,
																		  this.buttonAdd,
																		  this.buttonCancel});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(392, 152);
			this.Name = "ScriptSecurityPermissionsEditorForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Script Security Permissions";
			((System.ComponentModel.ISupportInitialize)(this.lbSecurityPermissions)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void buttonAdd_Click(object sender, System.EventArgs e) {
			ScriptSecurityPermission permission = new ScriptSecurityPermission(String.Empty);
			scriptSecurityPermissions.Add(permission);
			lbSecurityPermissions.SelectedIndex = lbSecurityPermissions.Items.Add(permission);
		}
		private void buttonRemove_Click(object sender, System.EventArgs e) {
			int index = lbSecurityPermissions.SelectedIndex;
			if (index >= 0) {
				scriptSecurityPermissions.RemoveAt(index);
				lbSecurityPermissions.Items.RemoveAt(index);
				if (index >= lbSecurityPermissions.ItemCount)
					index = lbSecurityPermissions.ItemCount - 1;
				if (index >= 0)
					lbSecurityPermissions.SelectedIndex = index;
			}
			UpdateControls();
		}
		private void lbSecurityAttributes_SelectedIndexChanged(object sender, System.EventArgs e) {
			int index = lbSecurityPermissions.SelectedIndex;
			propertyGrid.SelectedObject = 
				(index >= 0 && index < lbSecurityPermissions.ItemCount) ? 
					scriptSecurityPermissions[index] : null;
			UpdateControls();
		}
		void UpdateControls() {
			buttonRemove.Enabled = lbSecurityPermissions.ItemCount > 0;
		}
		private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			if (e.ChangedItem.PropertyDescriptor.Name == "Name") {
				((ScriptSecurityPermission)lbSecurityPermissions.Items[lbSecurityPermissions.SelectedIndex]).Name = (string)e.ChangedItem.Value;
				lbSecurityPermissions.Refresh();
			}
		}
	}
}

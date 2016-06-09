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

using DevExpress.XtraEditors;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Forms {
	partial class GroupUngroupForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupUngroupForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.rgRowColumns = new DevExpress.XtraEditors.RadioGroup();
			this.lblGroup = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.rgRowColumns.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.rgRowColumns, "rgRowColumns");
			this.rgRowColumns.Name = "rgRowColumns";
			this.rgRowColumns.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("rgRowColumns.Properties.Appearance.BackColor")));
			this.rgRowColumns.Properties.Appearance.Options.UseBackColor = true;
			this.rgRowColumns.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rgRowColumns.Properties.Columns = 1;
			this.rgRowColumns.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgRowColumns.Properties.Items"))), resources.GetString("rgRowColumns.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgRowColumns.Properties.Items2"))), resources.GetString("rgRowColumns.Properties.Items3"))});
			this.rgRowColumns.Properties.SelectedIndexChanged += new System.EventHandler(this.rgRowColumns_Properties_SelectedIndexChanged);
			resources.ApplyResources(this.lblGroup, "lblGroup");
			this.lblGroup.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lblGroup.LineVisible = true;
			this.lblGroup.Name = "lblGroup";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lblGroup);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.rgRowColumns);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GroupUngroupForm";
			((System.ComponentModel.ISupportInitialize)(this.rgRowColumns.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		SimpleButton btnCancel;
		SimpleButton btnOk;
		RadioGroup rgRowColumns;
		#endregion
		private LabelControl lblGroup;
	}
}

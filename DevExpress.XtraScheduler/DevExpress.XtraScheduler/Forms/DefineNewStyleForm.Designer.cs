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

namespace DevExpress.XtraScheduler.Forms {
	partial class DefineNewStylesForm {
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DefineNewStylesForm));
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.edtNewStyleName = new DevExpress.XtraEditors.TextEdit();
			this.lblSelectStyle = new DevExpress.XtraEditors.LabelControl();
			this.lblNewStyleName = new DevExpress.XtraEditors.LabelControl();
			this.ilbStyles = new DevExpress.XtraEditors.ImageListBoxControl();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnReset = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.edtNewStyleName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ilbStyles)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			resources.ApplyResources(this.edtNewStyleName, "edtNewStyleName");
			this.edtNewStyleName.Name = "edtNewStyleName";
			this.edtNewStyleName.Properties.AccessibleName = resources.GetString("edtNewStyleName.Properties.AccessibleName");
			this.edtNewStyleName.Properties.MaxLength = 45;
			resources.ApplyResources(this.lblSelectStyle, "lblSelectStyle");
			this.lblSelectStyle.Name = "lblSelectStyle";
			resources.ApplyResources(this.lblNewStyleName, "lblNewStyleName");
			this.lblNewStyleName.Name = "lblNewStyleName";
			resources.ApplyResources(this.ilbStyles, "ilbStyles");
			this.ilbStyles.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("ilbStyles.Appearance.BackColor")));
			this.ilbStyles.Appearance.Options.UseBackColor = true;
			this.ilbStyles.ItemHeight = 34;
			this.ilbStyles.Name = "ilbStyles";
			this.ilbStyles.SelectedIndexChanged += new System.EventHandler(this.ilbStyles_SelectedIndexChanged);
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			resources.ApplyResources(this.btnReset, "btnReset");
			this.btnReset.Name = "btnReset";
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnReset);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.ilbStyles);
			this.Controls.Add(this.lblNewStyleName);
			this.Controls.Add(this.lblSelectStyle);
			this.Controls.Add(this.edtNewStyleName);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnAdd);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DefineNewStylesForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.edtNewStyleName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ilbStyles)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected DevExpress.XtraEditors.ImageListBoxControl ilbStyles;
		protected DevExpress.XtraEditors.SimpleButton btnAdd;
		protected DevExpress.XtraEditors.SimpleButton btnClose;
		protected DevExpress.XtraEditors.TextEdit edtNewStyleName;
		protected System.ComponentModel.IContainer components = null;
		protected DevExpress.XtraEditors.SimpleButton btnDelete;
		protected DevExpress.XtraEditors.SimpleButton btnReset;
		protected DevExpress.XtraEditors.LabelControl lblSelectStyle;
		protected DevExpress.XtraEditors.LabelControl lblNewStyleName;
	}
}

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

namespace DevExpress.XtraRichEdit.Forms {
	partial class LanguageForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LanguageForm));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.languageListBox = new DevExpress.XtraEditors.ListBoxControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.checkGrammar = new DevExpress.XtraEditors.CheckEdit();
			this.checkLanguage = new DevExpress.XtraEditors.CheckEdit();
			this.btnSetDefault = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.languageListBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkGrammar.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkLanguage.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.languageListBox, "languageListBox");
			this.languageListBox.Name = "languageListBox";
			this.languageListBox.SortOrder = System.Windows.Forms.SortOrder.Ascending;
			this.languageListBox.Click += new System.EventHandler(this.languageListBox_Click);
			this.languageListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.languageListBox_KeyDown);
			this.languageListBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.languageListBox_KeyPress);
			this.languageListBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.languageListBox_KeyUp);
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.Name = "labelControl3";
			resources.ApplyResources(this.checkGrammar, "checkGrammar");
			this.checkGrammar.Name = "checkGrammar";
			this.checkGrammar.Properties.Caption = resources.GetString("checkGrammar.Properties.Caption");
			this.checkGrammar.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
			this.checkGrammar.CheckedChanged += new System.EventHandler(this.checkGrammar_CheckedChanged);
			this.checkGrammar.CheckStateChanged += new System.EventHandler(this.checkGrammar_CheckStateChanged);
			resources.ApplyResources(this.checkLanguage, "checkLanguage");
			this.checkLanguage.Name = "checkLanguage";
			this.checkLanguage.Properties.Caption = resources.GetString("checkLanguage.Properties.Caption");
			resources.ApplyResources(this.btnSetDefault, "btnSetDefault");
			this.btnSetDefault.Name = "btnSetDefault";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnSetDefault);
			this.Controls.Add(this.checkLanguage);
			this.Controls.Add(this.checkGrammar);
			this.Controls.Add(this.labelControl3);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.languageListBox);
			this.Controls.Add(this.labelControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LanguageForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.languageListBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkGrammar.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkLanguage.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.CheckEdit checkGrammar;
		private XtraEditors.CheckEdit checkLanguage;
		private XtraEditors.SimpleButton btnSetDefault;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.ListBoxControl languageListBox;
	}
}

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

namespace DevExpress.XtraEditors.Design {
	partial class ValidationRulesEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidationRulesEditorForm));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.controlsComboBox = new DevExpress.XtraEditors.ComboBoxEdit();
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.noValidationCheck = new DevExpress.XtraEditors.CheckEdit();
			this.condtionValidationCheck = new DevExpress.XtraEditors.CheckEdit();
			this.rulePropertiesGrid = new System.Windows.Forms.PropertyGrid();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.comparedValidationCheck = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.controlsComboBox.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.noValidationCheck.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.condtionValidationCheck.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.comparedValidationCheck.Properties)).BeginInit();
			this.SuspendLayout();
			this.labelControl1.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelControl1.Appearance.Font")));
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.controlsComboBox, "controlsComboBox");
			this.controlsComboBox.Name = "controlsComboBox";
			this.controlsComboBox.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("controlsComboBox.Properties.Buttons"))))});
			this.controlsComboBox.Properties.HighlightedItemStyle = DevExpress.XtraEditors.HighlightStyle.Skinned;
			this.controlsComboBox.Properties.Sorted = true;
			this.controlsComboBox.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.controlsComboBox.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.controlsComboBoxEdit_DrawItem);
			this.controlsComboBox.SelectedIndexChanged += new System.EventHandler(this.controlsComboBox_SelectedIndexChanged);
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.Name = "okButton";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.Name = "cancelButton";
			resources.ApplyResources(this.noValidationCheck, "noValidationCheck");
			this.noValidationCheck.Name = "noValidationCheck";
			this.noValidationCheck.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("noValidationCheck.Properties.Appearance.Font")));
			this.noValidationCheck.Properties.Appearance.Options.UseFont = true;
			this.noValidationCheck.Properties.Caption = resources.GetString("noValidationCheck.Properties.Caption");
			this.noValidationCheck.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.noValidationCheck.Properties.RadioGroupIndex = 0;
			this.noValidationCheck.TabStop = false;
			this.noValidationCheck.CheckedChanged += new System.EventHandler(this.CheckedChanged);
			resources.ApplyResources(this.condtionValidationCheck, "condtionValidationCheck");
			this.condtionValidationCheck.Name = "condtionValidationCheck";
			this.condtionValidationCheck.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("condtionValidationCheck.Properties.Appearance.Font")));
			this.condtionValidationCheck.Properties.Appearance.Options.UseFont = true;
			this.condtionValidationCheck.Properties.Caption = resources.GetString("condtionValidationCheck.Properties.Caption");
			this.condtionValidationCheck.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.condtionValidationCheck.Properties.RadioGroupIndex = 0;
			this.condtionValidationCheck.TabStop = false;
			this.condtionValidationCheck.CheckedChanged += new System.EventHandler(this.CheckedChanged);
			resources.ApplyResources(this.rulePropertiesGrid, "rulePropertiesGrid");
			this.rulePropertiesGrid.Name = "rulePropertiesGrid";
			this.rulePropertiesGrid.ToolbarVisible = false;
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.comparedValidationCheck, "comparedValidationCheck");
			this.comparedValidationCheck.Name = "comparedValidationCheck";
			this.comparedValidationCheck.Properties.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("comparedValidationCheck.Properties.Appearance.Font")));
			this.comparedValidationCheck.Properties.Appearance.Options.UseFont = true;
			this.comparedValidationCheck.Properties.Caption = resources.GetString("comparedValidationCheck.Properties.Caption");
			this.comparedValidationCheck.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.comparedValidationCheck.Properties.RadioGroupIndex = 0;
			this.comparedValidationCheck.TabStop = false;
			this.comparedValidationCheck.CheckedChanged += new System.EventHandler(this.CheckedChanged);
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.controlsComboBox);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.rulePropertiesGrid);
			this.Controls.Add(this.condtionValidationCheck);
			this.Controls.Add(this.comparedValidationCheck);
			this.Controls.Add(this.noValidationCheck);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ValidationRulesEditorForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.controlsComboBox.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.noValidationCheck.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.condtionValidationCheck.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.comparedValidationCheck.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private LabelControl labelControl1;
		private ComboBoxEdit controlsComboBox;
		private SimpleButton okButton;
		private SimpleButton cancelButton;
		private CheckEdit noValidationCheck;
		private CheckEdit condtionValidationCheck;
		private System.Windows.Forms.PropertyGrid rulePropertiesGrid;
		private LabelControl labelControl2;
		private CheckEdit comparedValidationCheck;
	}
}

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

namespace DevExpress.XtraBars.Ribbon {
	partial class RenameForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenameForm));
			this.labelControl = new DevExpress.XtraEditors.LabelControl();
			this.teName = new DevExpress.XtraEditors.TextEdit();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.teName.Properties)).BeginInit();
			this.SuspendLayout();
			this.labelControl.AccessibleDescription = null;
			this.labelControl.AccessibleName = null;
			resources.ApplyResources(this.labelControl, "labelControl");
			this.labelControl.Name = "labelControl";
			resources.ApplyResources(this.teName, "teName");
			this.teName.BackgroundImage = null;
			this.teName.EditValue = null;
			this.teName.Name = "teName";
			this.teName.Properties.AccessibleDescription = null;
			this.teName.Properties.AccessibleName = null;
			this.teName.Properties.AutoHeight = ((bool)(resources.GetObject("teName.Properties.AutoHeight")));
			this.teName.Properties.Mask.AutoComplete = ((DevExpress.XtraEditors.Mask.AutoCompleteType)(resources.GetObject("teName.Properties.Mask.AutoComplete")));
			this.teName.Properties.Mask.BeepOnError = ((bool)(resources.GetObject("teName.Properties.Mask.BeepOnError")));
			this.teName.Properties.Mask.EditMask = resources.GetString("teName.Properties.Mask.EditMask");
			this.teName.Properties.Mask.IgnoreMaskBlank = ((bool)(resources.GetObject("teName.Properties.Mask.IgnoreMaskBlank")));
			this.teName.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("teName.Properties.Mask.MaskType")));
			this.teName.Properties.Mask.PlaceHolder = ((char)(resources.GetObject("teName.Properties.Mask.PlaceHolder")));
			this.teName.Properties.Mask.SaveLiteral = ((bool)(resources.GetObject("teName.Properties.Mask.SaveLiteral")));
			this.teName.Properties.Mask.ShowPlaceHolders = ((bool)(resources.GetObject("teName.Properties.Mask.ShowPlaceHolders")));
			this.teName.Properties.Mask.UseMaskAsDisplayFormat = ((bool)(resources.GetObject("teName.Properties.Mask.UseMaskAsDisplayFormat")));
			this.teName.Properties.NullValuePrompt = resources.GetString("teName.Properties.NullValuePrompt");
			this.teName.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("teName.Properties.NullValuePromptShowForEmptyValue")));
			this.btnOk.AccessibleDescription = null;
			this.btnOk.AccessibleName = null;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.BackgroundImage = null;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.btnCancel.AccessibleDescription = null;
			this.btnCancel.AccessibleName = null;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.BackgroundImage = null;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.teName);
			this.Controls.Add(this.labelControl);
			this.Icon = null;
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RenameForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.teName.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private DevExpress.XtraEditors.LabelControl labelControl;
		private DevExpress.XtraEditors.TextEdit teName;
		private DevExpress.XtraEditors.SimpleButton btnOk;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
	}
}

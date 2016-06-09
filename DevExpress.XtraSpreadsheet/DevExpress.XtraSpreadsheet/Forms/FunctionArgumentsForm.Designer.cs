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

namespace DevExpress.XtraSpreadsheet.Forms {
	partial class FunctionArgumentsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionArgumentsForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.grpFunctionParameters = new DevExpress.XtraEditors.GroupControl();
			this.xtraScrollableControl = new DevExpress.XtraSpreadsheet.Forms.Design.VerticalScrollableItemsControl();
			this.lblResult = new DevExpress.XtraEditors.LabelControl();
			this.lblFunctionDescription = new DevExpress.XtraEditors.LabelControl();
			this.lblEqualSign = new DevExpress.XtraEditors.LabelControl();
			this.lblCurrentArgumentName = new DevExpress.XtraEditors.LabelControl();
			this.lblCurrentArgumentDescription = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.grpFunctionParameters)).BeginInit();
			this.grpFunctionParameters.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.xtraScrollableControl)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.grpFunctionParameters, "grpFunctionParameters");
			this.grpFunctionParameters.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpFunctionParameters.Controls.Add(this.xtraScrollableControl);
			this.grpFunctionParameters.Name = "grpFunctionParameters";
			resources.ApplyResources(this.xtraScrollableControl, "xtraScrollableControl");
			this.xtraScrollableControl.CurrentItemIndex = -2147483648;
			this.xtraScrollableControl.DataSource = null;
			this.xtraScrollableControl.Name = "xtraScrollableControl";
			this.xtraScrollableControl.CreateItemControl += new DevExpress.XtraSpreadsheet.Forms.Design.CreateItemControlEventHandler(this.OnCreateItemControl);
			resources.ApplyResources(this.lblResult, "lblResult");
			this.lblResult.Name = "lblResult";
			resources.ApplyResources(this.lblFunctionDescription, "lblFunctionDescription");
			this.lblFunctionDescription.Name = "lblFunctionDescription";
			resources.ApplyResources(this.lblEqualSign, "lblEqualSign");
			this.lblEqualSign.Name = "lblEqualSign";
			resources.ApplyResources(this.lblCurrentArgumentName, "lblCurrentArgumentName");
			this.lblCurrentArgumentName.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lblCurrentArgumentName.Appearance.Font")));
			this.lblCurrentArgumentName.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.lblCurrentArgumentName.Name = "lblCurrentArgumentName";
			resources.ApplyResources(this.lblCurrentArgumentDescription, "lblCurrentArgumentDescription");
			this.lblCurrentArgumentDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.lblCurrentArgumentDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lblCurrentArgumentDescription.Name = "lblCurrentArgumentDescription";
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lblCurrentArgumentDescription);
			this.Controls.Add(this.lblCurrentArgumentName);
			this.Controls.Add(this.lblEqualSign);
			this.Controls.Add(this.lblFunctionDescription);
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.grpFunctionParameters);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "FunctionArgumentsForm";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FunctionArgumentsForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.grpFunctionParameters)).EndInit();
			this.grpFunctionParameters.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.xtraScrollableControl)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.GroupControl grpFunctionParameters;
		private XtraEditors.LabelControl lblResult;
		private DevExpress.XtraSpreadsheet.Forms.Design.VerticalScrollableItemsControl xtraScrollableControl;
		private XtraEditors.LabelControl lblFunctionDescription;
		private XtraEditors.LabelControl lblEqualSign;
		private XtraEditors.LabelControl lblCurrentArgumentName;
		private XtraEditors.LabelControl lblCurrentArgumentDescription;
	}
}

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

namespace DevExpress.XtraMap.Design {
	partial class CustomEllipsoidEditorForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomEllipsoidEditorForm));
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.bottomLine = new DevExpress.XtraEditors.LabelControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.teInverceFlattering = new DevExpress.XtraEditors.TextEdit();
			this.lblInverseFlattering = new System.Windows.Forms.Label();
			this.lblSemimajorAxis = new System.Windows.Forms.Label();
			this.teSemimajorAxis = new DevExpress.XtraEditors.TextEdit();
			this.teName = new DevExpress.XtraEditors.TextEdit();
			this.lblName = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.teInverceFlattering.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teSemimajorAxis.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.teName.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.bottomLine, "bottomLine");
			this.bottomLine.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.bottomLine.LineVisible = true;
			this.bottomLine.Name = "bottomLine";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.teInverceFlattering, "teInverceFlattering");
			this.teInverceFlattering.Name = "teInverceFlattering";
			this.teInverceFlattering.Properties.Mask.EditMask = resources.GetString("teAbbreviation.Properties.Mask.EditMask");
			this.teInverceFlattering.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("teAbbreviation.Properties.Mask.MaskType")));
			resources.ApplyResources(this.lblInverseFlattering, "lblInverseFlattering");
			this.lblInverseFlattering.Name = "lblInverseFlattering";
			resources.ApplyResources(this.lblSemimajorAxis, "lblSemimajorAxis");
			this.lblSemimajorAxis.Name = "lblSemimajorAxis";
			resources.ApplyResources(this.teSemimajorAxis, "teSemimajorAxis");
			this.teSemimajorAxis.Name = "teSemimajorAxis";
			this.teSemimajorAxis.Properties.Mask.EditMask = resources.GetString("teName.Properties.Mask.EditMask");
			this.teSemimajorAxis.Properties.Mask.MaskType = ((DevExpress.XtraEditors.Mask.MaskType)(resources.GetObject("teName.Properties.Mask.MaskType")));
			resources.ApplyResources(this.teName, "teName");
			this.teName.Name = "teName";
			resources.ApplyResources(this.lblName, "lblName");
			this.lblName.Name = "lblName";
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.teName);
			this.Controls.Add(this.lblName);
			this.Controls.Add(this.teInverceFlattering);
			this.Controls.Add(this.lblInverseFlattering);
			this.Controls.Add(this.lblSemimajorAxis);
			this.Controls.Add(this.teSemimajorAxis);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.bottomLine);
			this.Controls.Add(this.btnCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CustomEllipsoidEditorForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.teInverceFlattering.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teSemimajorAxis.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.teName.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.LabelControl bottomLine;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.TextEdit teInverceFlattering;
		private System.Windows.Forms.Label lblInverseFlattering;
		private System.Windows.Forms.Label lblSemimajorAxis;
		private XtraEditors.TextEdit teSemimajorAxis;
		private XtraEditors.TextEdit teName;
		private System.Windows.Forms.Label lblName;
	}
}

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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Customization {
	public class EditForm : XtraForm {
		private SimpleButton btCancel;
		private SimpleButton btOk;
		public System.Windows.Forms.Label lbCaption;
		public TextEdit tbName;
		public EditForm() {
			InitializeComponent();
			this.lbCaption.Text = BarLocalizer.Active.GetLocalizedString(BarString.ToolbarNameCaption);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.btCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btOk = new DevExpress.XtraEditors.SimpleButton();
			this.lbCaption = new System.Windows.Forms.Label();
			this.tbName = new DevExpress.XtraEditors.TextEdit();
			((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).BeginInit();
			this.SuspendLayout();
			this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btCancel.Location = new System.Drawing.Point(225, 69);
			this.btCancel.Name = "btCancel";
			this.btCancel.Size = new System.Drawing.Size(84, 24);
			this.btCancel.TabIndex = 1;
			this.btCancel.Text = "Cancel";
			this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btOk.Location = new System.Drawing.Point(131, 69);
			this.btOk.Name = "btOk";
			this.btOk.Size = new System.Drawing.Size(85, 24);
			this.btOk.TabIndex = 0;
			this.btOk.Text = "OK";
			this.lbCaption.Location = new System.Drawing.Point(9, 10);
			this.lbCaption.Name = "lbCaption";
			this.lbCaption.Size = new System.Drawing.Size(300, 20);
			this.lbCaption.TabIndex = 3;
			this.lbCaption.Text = "&Toolbar Name:";
			this.tbName.EditValue = "";
			this.tbName.Location = new System.Drawing.Point(9, 30);
			this.tbName.Name = "tbName";
			this.tbName.Size = new System.Drawing.Size(300, 21);
			this.tbName.TabIndex = 0;
			this.AcceptButton = this.btOk;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.btCancel;
			this.ClientSize = new System.Drawing.Size(314, 104);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lbCaption,
																		  this.tbName,
																		  this.btCancel,
																		  this.btOk});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Custom";
			((System.ComponentModel.ISupportInitialize)(this.tbName.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
	}
}

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

namespace DevExpress.XtraSpreadsheet.Forms.Design {
	partial class FormatNumberCustomControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatNumberCustomControl));
			this.lbType = new DevExpress.XtraEditors.LabelControl();
			this.edtCustom = new DevExpress.XtraEditors.TextEdit();
			this.lbCustom = new DevExpress.XtraEditors.ListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.edtCustom.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCustom)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lbType, "lbType");
			this.lbType.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lbType.Name = "lbType";
			resources.ApplyResources(this.edtCustom, "edtCustom");
			this.edtCustom.Name = "edtCustom";
			resources.ApplyResources(this.lbCustom, "lbCustom");
			this.lbCustom.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbCustom.Name = "lbCustom";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lbCustom);
			this.Controls.Add(this.edtCustom);
			this.Controls.Add(this.lbType);
			this.Name = "FormatNumberCustomControl";
			((System.ComponentModel.ISupportInitialize)(this.edtCustom.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCustom)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl lbType;
		private XtraEditors.TextEdit edtCustom;
		private XtraEditors.ListBoxControl lbCustom;
	}
}

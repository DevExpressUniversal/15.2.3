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
	partial class FormatNumberTypeControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatNumberTypeControl));
			this.lbType = new DevExpress.XtraEditors.LabelControl();
			this.lBoxType = new DevExpress.XtraEditors.ListBoxControl();
			this.lblLocation = new DevExpress.XtraEditors.LabelControl();
			this.edtLocation = new DevExpress.XtraEditors.LookUpEdit();
			((System.ComponentModel.ISupportInitialize)(this.lBoxType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLocation.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lbType, "lbType");
			this.lbType.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lbType.Name = "lbType";
			resources.ApplyResources(this.lBoxType, "lBoxType");
			this.lBoxType.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lBoxType.Name = "lBoxType";
			resources.ApplyResources(this.lblLocation, "lblLocation");
			this.lblLocation.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblLocation.Name = "lblLocation";
			resources.ApplyResources(this.edtLocation, "edtLocation");
			this.edtLocation.Name = "edtLocation";
			this.edtLocation.Properties.AccessibleName = resources.GetString("edtLocation.Properties.AccessibleName");
			this.edtLocation.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.edtLocation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtLocation.Properties.Buttons"))))});
			this.edtLocation.Properties.DropDownRows = 6;
			this.edtLocation.Properties.NullText = resources.GetString("edtLocation.Properties.NullText");
			this.edtLocation.Properties.ShowFooter = false;
			this.edtLocation.Properties.ShowHeader = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.edtLocation);
			this.Controls.Add(this.lblLocation);
			this.Controls.Add(this.lBoxType);
			this.Controls.Add(this.lbType);
			this.Name = "FormatNumberTypeControl";
			((System.ComponentModel.ISupportInitialize)(this.lBoxType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtLocation.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl lbType;
		private XtraEditors.ListBoxControl lBoxType;
		private XtraEditors.LabelControl lblLocation;
		private XtraEditors.LookUpEdit edtLocation;
	}
}

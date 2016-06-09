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
	partial class CellReferenceAddressControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CellReferenceAddressControl));
			this.lblCellReference = new DevExpress.XtraEditors.LabelControl();
			this.teCellReference = new DevExpress.XtraEditors.TextEdit();
			this.tlCellReference = new DevExpress.XtraTreeList.TreeList();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.teCellReference.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tlCellReference)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblCellReference, "lblCellReference");
			this.lblCellReference.Name = "lblCellReference";
			resources.ApplyResources(this.teCellReference, "teCellReference");
			this.teCellReference.Name = "teCellReference";
			resources.ApplyResources(this.tlCellReference, "tlCellReference");
			this.tlCellReference.Name = "tlCellReference";
			this.tlCellReference.OptionsBehavior.Editable = false;
			this.tlCellReference.OptionsView.ShowColumns = false;
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.tlCellReference);
			this.Controls.Add(this.teCellReference);
			this.Controls.Add(this.lblCellReference);
			this.Name = "CellReferenceAddressControl";
			((System.ComponentModel.ISupportInitialize)(this.teCellReference.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tlCellReference)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl lblCellReference;
		private XtraEditors.TextEdit teCellReference;
		private XtraTreeList.TreeList tlCellReference;
		private XtraEditors.LabelControl labelControl1;
	}
}

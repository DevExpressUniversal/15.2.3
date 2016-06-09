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
	partial class OutlineSubtotalForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutlineSubtotalForm));
			this.lcChangeIn = new DevExpress.XtraEditors.LabelControl();
			this.clSubtotalTo = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.cbChangeIn = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcUseFunction = new DevExpress.XtraEditors.LabelControl();
			this.cbUseFunction = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lcSubtotalTo = new DevExpress.XtraEditors.LabelControl();
			this.ceReplaceCurrent = new DevExpress.XtraEditors.CheckEdit();
			this.cePageBreak = new DevExpress.XtraEditors.CheckEdit();
			this.ceSummaryBelow = new DevExpress.XtraEditors.CheckEdit();
			this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
			this.sbOK = new DevExpress.XtraEditors.SimpleButton();
			this.sbRemoveAll = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.clSubtotalTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbChangeIn.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbUseFunction.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceReplaceCurrent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cePageBreak.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceSummaryBelow.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lcChangeIn, "lcChangeIn");
			this.lcChangeIn.Name = "lcChangeIn";
			resources.ApplyResources(this.clSubtotalTo, "clSubtotalTo");
			this.clSubtotalTo.Name = "clSubtotalTo";
			this.clSubtotalTo.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			this.clSubtotalTo.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.clSubtotalTo_ItemCheck);
			resources.ApplyResources(this.cbChangeIn, "cbChangeIn");
			this.cbChangeIn.Name = "cbChangeIn";
			this.cbChangeIn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbChangeIn.Properties.Buttons"))))});
			this.cbChangeIn.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			resources.ApplyResources(this.lcUseFunction, "lcUseFunction");
			this.lcUseFunction.Name = "lcUseFunction";
			resources.ApplyResources(this.cbUseFunction, "cbUseFunction");
			this.cbUseFunction.Name = "cbUseFunction";
			this.cbUseFunction.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbUseFunction.Properties.Buttons"))))});
			resources.ApplyResources(this.lcSubtotalTo, "lcSubtotalTo");
			this.lcSubtotalTo.Name = "lcSubtotalTo";
			resources.ApplyResources(this.ceReplaceCurrent, "ceReplaceCurrent");
			this.ceReplaceCurrent.Name = "ceReplaceCurrent";
			this.ceReplaceCurrent.Properties.AutoWidth = true;
			this.ceReplaceCurrent.Properties.Caption = resources.GetString("ceReplaceCurrent.Properties.Caption");
			this.ceReplaceCurrent.CheckedChanged += new System.EventHandler(this.ceReplaceCurrent_CheckedChanged);
			resources.ApplyResources(this.cePageBreak, "cePageBreak");
			this.cePageBreak.Name = "cePageBreak";
			this.cePageBreak.Properties.AutoWidth = true;
			this.cePageBreak.Properties.Caption = resources.GetString("cePageBreak.Properties.Caption");
			resources.ApplyResources(this.ceSummaryBelow, "ceSummaryBelow");
			this.ceSummaryBelow.Name = "ceSummaryBelow";
			this.ceSummaryBelow.Properties.AutoWidth = true;
			this.ceSummaryBelow.Properties.Caption = resources.GetString("ceSummaryBelow.Properties.Caption");
			this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.sbCancel, "sbCancel");
			this.sbCancel.Name = "sbCancel";
			resources.ApplyResources(this.sbOK, "sbOK");
			this.sbOK.Name = "sbOK";
			this.sbOK.Click += new System.EventHandler(this.sbOK_Click);
			resources.ApplyResources(this.sbRemoveAll, "sbRemoveAll");
			this.sbRemoveAll.Name = "sbRemoveAll";
			this.sbRemoveAll.Click += new System.EventHandler(this.sbRemoveAll_Click);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.sbCancel;
			this.Controls.Add(this.sbRemoveAll);
			this.Controls.Add(this.sbOK);
			this.Controls.Add(this.sbCancel);
			this.Controls.Add(this.ceSummaryBelow);
			this.Controls.Add(this.cePageBreak);
			this.Controls.Add(this.ceReplaceCurrent);
			this.Controls.Add(this.lcSubtotalTo);
			this.Controls.Add(this.cbUseFunction);
			this.Controls.Add(this.lcUseFunction);
			this.Controls.Add(this.cbChangeIn);
			this.Controls.Add(this.clSubtotalTo);
			this.Controls.Add(this.lcChangeIn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OutlineSubtotalForm";
			((System.ComponentModel.ISupportInitialize)(this.clSubtotalTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbChangeIn.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbUseFunction.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceReplaceCurrent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cePageBreak.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceSummaryBelow.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl lcChangeIn;
		private XtraEditors.CheckedListBoxControl clSubtotalTo;
		private XtraEditors.ComboBoxEdit cbChangeIn;
		private XtraEditors.LabelControl lcUseFunction;
		private XtraEditors.ComboBoxEdit cbUseFunction;
		private XtraEditors.LabelControl lcSubtotalTo;
		private XtraEditors.CheckEdit ceReplaceCurrent;
		private XtraEditors.CheckEdit cePageBreak;
		private XtraEditors.CheckEdit ceSummaryBelow;
		private XtraEditors.SimpleButton sbCancel;
		private XtraEditors.SimpleButton sbOK;
		private XtraEditors.SimpleButton sbRemoveAll;
	}
}

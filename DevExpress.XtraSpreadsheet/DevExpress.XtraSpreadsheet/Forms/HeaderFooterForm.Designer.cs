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
	partial class HeaderFooterForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeaderFooterForm));
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabHeaderFooter = new DevExpress.XtraTab.XtraTabPage();
			this.headerFooterControl = new DevExpress.XtraSpreadsheet.Forms.Design.HeaderFooterEditControl();
			this.xtraTabFirstHeaderFooter = new DevExpress.XtraTab.XtraTabPage();
			this.firstHeaderFooterControl = new DevExpress.XtraSpreadsheet.Forms.Design.HeaderFooterEditControl();
			this.xtraTabOddHeaderFooter = new DevExpress.XtraTab.XtraTabPage();
			this.oddHeaderFooterControl = new DevExpress.XtraSpreadsheet.Forms.Design.HeaderFooterEditControl();
			this.xtraTabEvenHeaderFooter = new DevExpress.XtraTab.XtraTabPage();
			this.evenHeaderFooterControl = new DevExpress.XtraSpreadsheet.Forms.Design.HeaderFooterEditControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.xtraTabHeaderFooter.SuspendLayout();
			this.xtraTabFirstHeaderFooter.SuspendLayout();
			this.xtraTabOddHeaderFooter.SuspendLayout();
			this.xtraTabEvenHeaderFooter.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.xtraTabHeaderFooter;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabHeaderFooter,
			this.xtraTabFirstHeaderFooter,
			this.xtraTabOddHeaderFooter,
			this.xtraTabEvenHeaderFooter});
			this.xtraTabHeaderFooter.Controls.Add(this.headerFooterControl);
			this.xtraTabHeaderFooter.Name = "xtraTabHeaderFooter";
			resources.ApplyResources(this.xtraTabHeaderFooter, "xtraTabHeaderFooter");
			this.headerFooterControl.CenterFooter = "";
			this.headerFooterControl.CenterHeader = "";
			this.headerFooterControl.LeftFooter = "";
			this.headerFooterControl.LeftHeader = "";
			resources.ApplyResources(this.headerFooterControl, "headerFooterControl");
			this.headerFooterControl.Name = "headerFooterControl";
			this.headerFooterControl.RightFooter = "";
			this.headerFooterControl.RightHeader = "";
			this.xtraTabFirstHeaderFooter.Controls.Add(this.firstHeaderFooterControl);
			this.xtraTabFirstHeaderFooter.Name = "xtraTabFirstHeaderFooter";
			resources.ApplyResources(this.xtraTabFirstHeaderFooter, "xtraTabFirstHeaderFooter");
			this.firstHeaderFooterControl.CenterFooter = "";
			this.firstHeaderFooterControl.CenterHeader = "";
			this.firstHeaderFooterControl.LeftFooter = "";
			this.firstHeaderFooterControl.LeftHeader = "";
			resources.ApplyResources(this.firstHeaderFooterControl, "firstHeaderFooterControl");
			this.firstHeaderFooterControl.Name = "firstHeaderFooterControl";
			this.firstHeaderFooterControl.RightFooter = "";
			this.firstHeaderFooterControl.RightHeader = "";
			this.xtraTabOddHeaderFooter.Controls.Add(this.oddHeaderFooterControl);
			this.xtraTabOddHeaderFooter.Name = "xtraTabOddHeaderFooter";
			resources.ApplyResources(this.xtraTabOddHeaderFooter, "xtraTabOddHeaderFooter");
			this.oddHeaderFooterControl.CenterFooter = "";
			this.oddHeaderFooterControl.CenterHeader = "";
			this.oddHeaderFooterControl.LeftFooter = "";
			this.oddHeaderFooterControl.LeftHeader = "";
			resources.ApplyResources(this.oddHeaderFooterControl, "oddHeaderFooterControl");
			this.oddHeaderFooterControl.Name = "oddHeaderFooterControl";
			this.oddHeaderFooterControl.RightFooter = "";
			this.oddHeaderFooterControl.RightHeader = "";
			this.xtraTabEvenHeaderFooter.Controls.Add(this.evenHeaderFooterControl);
			this.xtraTabEvenHeaderFooter.Name = "xtraTabEvenHeaderFooter";
			resources.ApplyResources(this.xtraTabEvenHeaderFooter, "xtraTabEvenHeaderFooter");
			this.evenHeaderFooterControl.CenterFooter = "";
			this.evenHeaderFooterControl.CenterHeader = "";
			this.evenHeaderFooterControl.LeftFooter = "";
			this.evenHeaderFooterControl.LeftHeader = "";
			resources.ApplyResources(this.evenHeaderFooterControl, "evenHeaderFooterControl");
			this.evenHeaderFooterControl.Name = "evenHeaderFooterControl";
			this.evenHeaderFooterControl.RightFooter = "";
			this.evenHeaderFooterControl.RightHeader = "";
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "HeaderFooterForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.xtraTabHeaderFooter.ResumeLayout(false);
			this.xtraTabFirstHeaderFooter.ResumeLayout(false);
			this.xtraTabOddHeaderFooter.ResumeLayout(false);
			this.xtraTabEvenHeaderFooter.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraTab.XtraTabControl tabControl;
		private XtraTab.XtraTabPage xtraTabHeaderFooter;
		private XtraTab.XtraTabPage xtraTabFirstHeaderFooter;
		private XtraTab.XtraTabPage xtraTabOddHeaderFooter;
		private XtraTab.XtraTabPage xtraTabEvenHeaderFooter;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnOk;
		private Design.HeaderFooterEditControl headerFooterControl;
		private Design.HeaderFooterEditControl firstHeaderFooterControl;
		private Design.HeaderFooterEditControl oddHeaderFooterControl;
		private Design.HeaderFooterEditControl evenHeaderFooterControl;
	}
}

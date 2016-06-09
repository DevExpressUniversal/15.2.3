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

namespace DevExpress.XtraPdfViewer.FindControl
{
	partial class PdfFindControl
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfFindControl));
			this.btnFindNext = new DevExpress.XtraEditors.SimpleButton();
			this.btnFindPrev = new DevExpress.XtraEditors.SimpleButton();
			this.textEdit = new DevExpress.XtraEditors.TextEdit();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.searchLabel = new DevExpress.XtraEditors.LabelControl();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.ddBtnParameters = new DevExpress.XtraPdfViewer.Native.PdfSearchParametersButton();
			((System.ComponentModel.ISupportInitialize)(this.textEdit.Properties)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnFindNext, "btnFindNext");
			this.btnFindNext.Name = "btnFindNext";
			this.btnFindNext.Click += new System.EventHandler(this.OnBtnFindNextClick);
			resources.ApplyResources(this.btnFindPrev, "btnFindPrev");
			this.btnFindPrev.Name = "btnFindPrev";
			this.btnFindPrev.Click += new System.EventHandler(this.OnBtnFindPrevClick);
			resources.ApplyResources(this.textEdit, "textEdit");
			this.textEdit.Name = "textEdit";
			this.textEdit.Properties.AutoHeight = ((bool)(resources.GetObject("textEdit.Properties.AutoHeight")));
			this.btnClose.Appearance.Options.UseTextOptions = true;
			this.btnClose.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.btnClose.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.OnBtnCloseClick);
			resources.ApplyResources(this.searchLabel, "searchLabel");
			this.searchLabel.Name = "searchLabel";
			this.searchLabel.UseMnemonic = false;
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.searchLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.textEdit, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.ddBtnParameters, 2, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnClose, 6, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnFindPrev, 3, 0);
			this.tableLayoutPanel1.Controls.Add(this.btnFindNext, 4, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			resources.ApplyResources(this.ddBtnParameters, "ddBtnParameters");
			this.ddBtnParameters.AutoWidthInLayoutControl = true;
			this.ddBtnParameters.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
			this.ddBtnParameters.Name = "ddBtnParameters";
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "PdfFindControl";
			((System.ComponentModel.ISupportInitialize)(this.textEdit.Properties)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SimpleButton btnFindNext;
		private XtraEditors.SimpleButton btnFindPrev;
		private XtraEditors.TextEdit textEdit;
		private XtraEditors.SimpleButton btnClose;
		private XtraEditors.LabelControl searchLabel;
		private DevExpress.XtraPdfViewer.Native.PdfSearchParametersButton ddBtnParameters;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}

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

namespace DevExpress.XtraRichEdit.Forms {
	partial class BorderShadingForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BorderShadingForm));
			DevExpress.XtraRichEdit.Model.BorderInfo borderInfo1 = new DevExpress.XtraRichEdit.Model.BorderInfo();
			DevExpress.XtraRichEdit.Model.BorderInfo borderInfo2 = new DevExpress.XtraRichEdit.Model.BorderInfo();
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.pageBorders = new DevExpress.XtraTab.XtraTabPage();
			this.borderShadingTypeLineUserControl1 = new DevExpress.XtraRichEdit.Forms.Design.BorderShadingTypeLineUserControl();
			this.borderUserControl = new DevExpress.XtraRichEdit.Forms.Design.BorderShadingUserControl();
			this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btnCustom = new DevExpress.XtraEditors.CheckButton();
			this.btnGrid = new DevExpress.XtraEditors.CheckButton();
			this.btnAll = new DevExpress.XtraEditors.CheckButton();
			this.btnBox = new DevExpress.XtraEditors.CheckButton();
			this.btnNone = new DevExpress.XtraEditors.CheckButton();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.pageShading = new DevExpress.XtraTab.XtraTabPage();
			this.colorEdit = new DevExpress.XtraEditors.ColorEdit();
			this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
			this.shadingUserControl = new DevExpress.XtraRichEdit.Forms.Design.BorderShadingUserControl();
			this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.pageBorders.SuspendLayout();
			this.pageShading.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.colorEdit.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.xtraTabControl1, "xtraTabControl1");
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.pageBorders;
			this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.pageBorders,
			this.pageShading});
			this.pageBorders.Controls.Add(this.borderShadingTypeLineUserControl1);
			this.pageBorders.Controls.Add(this.borderUserControl);
			this.pageBorders.Controls.Add(this.labelControl7);
			this.pageBorders.Controls.Add(this.labelControl6);
			this.pageBorders.Controls.Add(this.labelControl5);
			this.pageBorders.Controls.Add(this.labelControl4);
			this.pageBorders.Controls.Add(this.labelControl3);
			this.pageBorders.Controls.Add(this.labelControl1);
			this.pageBorders.Controls.Add(this.btnCustom);
			this.pageBorders.Controls.Add(this.btnGrid);
			this.pageBorders.Controls.Add(this.btnAll);
			this.pageBorders.Controls.Add(this.btnBox);
			this.pageBorders.Controls.Add(this.btnNone);
			this.pageBorders.Controls.Add(this.labelControl2);
			this.pageBorders.Name = "pageBorders";
			resources.ApplyResources(this.pageBorders, "pageBorders");
			resources.ApplyResources(this.borderShadingTypeLineUserControl1, "borderShadingTypeLineUserControl1");
			this.borderShadingTypeLineUserControl1.DocumentModel = null;
			this.borderShadingTypeLineUserControl1.Name = "borderShadingTypeLineUserControl1";
			this.borderShadingTypeLineUserControl1.RichEditControl = null;
			this.borderShadingTypeLineUserControl1.BorderChanged += new System.EventHandler(this.borderShadingTypeLineUserControl1_BorderChanged);
			this.borderUserControl.BorderLineDown = null;
			this.borderUserControl.BorderLineHorizontalIn = null;
			this.borderUserControl.BorderLineHorizontalInVisible = false;
			this.borderUserControl.BorderLineLeft = null;
			this.borderUserControl.BorderLineRight = null;
			this.borderUserControl.BorderLineUp = null;
			this.borderUserControl.BorderLineVerticalIn = null;
			this.borderUserControl.BorderLineVerticalInVisible = false;
			this.borderUserControl.BtnsVisible = false;
			borderInfo1.Color = System.Drawing.Color.Red;
			borderInfo1.Frame = false;
			borderInfo1.Offset = 0;
			borderInfo1.Shadow = false;
			borderInfo1.Style = DevExpress.XtraRichEdit.Model.BorderLineStyle.Double;
			borderInfo1.Width = 10;
			this.borderUserControl.CurrentBorderInfo = borderInfo1;
			this.borderUserControl.DocumentModel = null;
			this.borderUserControl.FillColor = System.Drawing.Color.Empty;
			resources.ApplyResources(this.borderUserControl, "borderUserControl");
			this.borderUserControl.Name = "borderUserControl";
			this.borderUserControl.BorderLineUpChanged += new System.EventHandler(this.borderUserControl_BorderLineUpChanged);
			this.borderUserControl.BorderLineDownChanged += new System.EventHandler(this.borderUserControl_BorderLineDownChanged);
			this.borderUserControl.BorderLineHorizontalInChanged += new System.EventHandler(this.borderUserControl_BorderLineHorizontalInChanged);
			this.borderUserControl.BorderLineVerticalInChanged += new System.EventHandler(this.borderUserControl_BorderLineVerticalInChanged);
			this.borderUserControl.BorderLineLeftChanged += new System.EventHandler(this.borderUserControl_BorderLineLeftChanged);
			this.borderUserControl.BorderLineRightChanged += new System.EventHandler(this.borderUserControl_BorderLineRightChanged);
			this.borderUserControl.Click += new System.EventHandler(this.borderUserControl_Click);
			resources.ApplyResources(this.labelControl7, "labelControl7");
			this.labelControl7.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl7.LineVisible = true;
			this.labelControl7.Name = "labelControl7";
			resources.ApplyResources(this.labelControl6, "labelControl6");
			this.labelControl6.Name = "labelControl6";
			resources.ApplyResources(this.labelControl5, "labelControl5");
			this.labelControl5.Name = "labelControl5";
			resources.ApplyResources(this.labelControl4, "labelControl4");
			this.labelControl4.Name = "labelControl4";
			resources.ApplyResources(this.labelControl3, "labelControl3");
			this.labelControl3.Name = "labelControl3";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			this.btnCustom.Checked = true;
			resources.ApplyResources(this.btnCustom, "btnCustom");
			this.btnCustom.Name = "btnCustom";
			this.btnCustom.CheckedChanged += new System.EventHandler(this.btnCustom_CheckedChanged);
			resources.ApplyResources(this.btnGrid, "btnGrid");
			this.btnGrid.Name = "btnGrid";
			this.btnGrid.CheckedChanged += new System.EventHandler(this.btnGrid_CheckedChanged);
			resources.ApplyResources(this.btnAll, "btnAll");
			this.btnAll.Name = "btnAll";
			this.btnAll.CheckedChanged += new System.EventHandler(this.btnAll_CheckedChanged);
			resources.ApplyResources(this.btnBox, "btnBox");
			this.btnBox.Name = "btnBox";
			this.btnBox.CheckedChanged += new System.EventHandler(this.btnBox_CheckedChanged);
			resources.ApplyResources(this.btnNone, "btnNone");
			this.btnNone.Name = "btnNone";
			this.btnNone.CheckedChanged += new System.EventHandler(this.btnNone_CheckedChanged);
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl2.LineVisible = true;
			this.labelControl2.Name = "labelControl2";
			this.pageShading.Controls.Add(this.colorEdit);
			this.pageShading.Controls.Add(this.labelControl9);
			this.pageShading.Controls.Add(this.shadingUserControl);
			this.pageShading.Controls.Add(this.labelControl8);
			this.pageShading.Name = "pageShading";
			resources.ApplyResources(this.pageShading, "pageShading");
			resources.ApplyResources(this.colorEdit, "colorEdit");
			this.colorEdit.Name = "colorEdit";
			this.colorEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("colorEdit.Properties.Buttons"))))});
			this.colorEdit.ColorChanged += new System.EventHandler(this.colorEdit_ColorChanged);
			resources.ApplyResources(this.labelControl9, "labelControl9");
			this.labelControl9.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.labelControl9.LineVisible = true;
			this.labelControl9.Name = "labelControl9";
			this.shadingUserControl.BorderLineDown = null;
			this.shadingUserControl.BorderLineHorizontalIn = null;
			this.shadingUserControl.BorderLineHorizontalInVisible = false;
			this.shadingUserControl.BorderLineLeft = null;
			this.shadingUserControl.BorderLineRight = null;
			this.shadingUserControl.BorderLineUp = null;
			this.shadingUserControl.BorderLineVerticalIn = null;
			this.shadingUserControl.BorderLineVerticalInVisible = false;
			this.shadingUserControl.BtnsVisible = false;
			borderInfo2.Color = System.Drawing.Color.Red;
			borderInfo2.Frame = false;
			borderInfo2.Offset = 0;
			borderInfo2.Shadow = false;
			borderInfo2.Style = DevExpress.XtraRichEdit.Model.BorderLineStyle.Double;
			borderInfo2.Width = 10;
			this.shadingUserControl.CurrentBorderInfo = borderInfo2;
			this.shadingUserControl.DocumentModel = null;
			this.shadingUserControl.FillColor = System.Drawing.Color.Empty;
			resources.ApplyResources(this.shadingUserControl, "shadingUserControl");
			this.shadingUserControl.Name = "shadingUserControl";
			resources.ApplyResources(this.labelControl8, "labelControl8");
			this.labelControl8.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.labelControl8.LineVisible = true;
			this.labelControl8.Name = "labelControl8";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click_1);
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.xtraTabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BorderShadingForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.pageBorders.ResumeLayout(false);
			this.pageBorders.PerformLayout();
			this.pageShading.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.colorEdit.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraTab.XtraTabControl xtraTabControl1;
		private XtraTab.XtraTabPage pageBorders;
		private XtraTab.XtraTabPage pageShading;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.LabelControl labelControl7;
		private XtraEditors.LabelControl labelControl6;
		private XtraEditors.LabelControl labelControl5;
		private XtraEditors.LabelControl labelControl4;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.CheckButton btnCustom;
		private XtraEditors.CheckButton btnGrid;
		private XtraEditors.CheckButton btnAll;
		private XtraEditors.CheckButton btnBox;
		private XtraEditors.CheckButton btnNone;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.ColorEdit colorEdit;
		private XtraEditors.LabelControl labelControl9;
		private Design.BorderShadingUserControl shadingUserControl;
		private XtraEditors.LabelControl labelControl8;
		private Design.BorderShadingUserControl borderUserControl;
		private Design.BorderShadingTypeLineUserControl borderShadingTypeLineUserControl1;
	}
}

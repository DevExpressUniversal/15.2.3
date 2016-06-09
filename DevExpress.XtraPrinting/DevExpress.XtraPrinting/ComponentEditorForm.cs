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
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraLayout;
namespace DevExpress.XtraPrinting
{
	public class ComponentEditorForm : XtraForm {
		bool isRTLChanged;
		private System.ComponentModel.Container components = null;
		private XtraLayout.LayoutControl layoutControl1;
		private SimpleButton btnApply;
		private SimpleButton btnCancel;
		private SimpleButton btnOK;
		private SimpleButton btnHelp;
		private Panel propertyPanel;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlGroup layoutControlGroup2;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.LayoutControlGroup grpButtons;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private LabelControl lblDelimit;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private PrintableComponentLinkBase printableComponentLink;
		IPrintable Printable { get { return printableComponentLink.Component; } }
		ComponentEditorForm() {
			InitializeComponent();
		}
		public ComponentEditorForm(PrintableComponentLinkBase printableComponentLink) {
			InitializeComponent();
			this.printableComponentLink = printableComponentLink;
			InitializePrintable(Printable.PropertyEditorControl);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentEditorForm));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.lblDelimit = new DevExpress.XtraEditors.LabelControl();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnHelp = new DevExpress.XtraEditors.SimpleButton();
			this.propertyPanel = new System.Windows.Forms.Panel();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.lblDelimit);
			this.layoutControl1.Controls.Add(this.btnApply);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnOK);
			this.layoutControl1.Controls.Add(this.btnHelp);
			this.layoutControl1.Controls.Add(this.propertyPanel);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(793, 40, 1058, 877);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.lblDelimit.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.lblDelimit.LineVisible = true;
			resources.ApplyResources(this.lblDelimit, "lblDelimit");
			this.lblDelimit.Name = "lblDelimit";
			this.lblDelimit.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.StyleController = this.layoutControl1;
			this.btnApply.Click += new System.EventHandler(this.acceptButton_Click);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.Click += new System.EventHandler(this.cancelButton_Click);
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl1;
			this.btnOK.Click += new System.EventHandler(this.acceptButton_Click);
			resources.ApplyResources(this.btnHelp, "btnHelp");
			this.btnHelp.Name = "btnHelp";
			this.btnHelp.StyleController = this.layoutControl1;
			this.btnHelp.Click += new System.EventHandler(this.helpButton_Click);
			resources.ApplyResources(this.propertyPanel, "propertyPanel");
			this.propertyPanel.Name = "propertyPanel";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup2,
			this.grpButtons});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(320, 223);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1});
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Size = new System.Drawing.Size(320, 180);
			this.layoutControlItem1.Control = this.propertyPanel;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem1.Size = new System.Drawing.Size(320, 180);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem6});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 180);
			this.grpButtons.Name = "grpButtons";
			this.grpButtons.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 1;
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition1.Width = 100D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 76D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 3D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 76D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition5.Width = 3D;
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition6.Width = 76D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition7.Width = 3D;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition8.Width = 76D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5,
			columnDefinition6,
			columnDefinition7,
			columnDefinition8});
			rowDefinition1.Height = 13D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition2.Height = 26D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 4D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3});
			this.grpButtons.Size = new System.Drawing.Size(320, 43);
			this.layoutControlItem2.Control = this.btnHelp;
			this.layoutControlItem2.Location = new System.Drawing.Point(244, 13);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 7;
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem2.Size = new System.Drawing.Size(76, 26);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem3.Control = this.btnOK;
			this.layoutControlItem3.Location = new System.Drawing.Point(7, 13);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem3.Size = new System.Drawing.Size(76, 26);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.Control = this.btnCancel;
			this.layoutControlItem4.Location = new System.Drawing.Point(86, 13);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem4.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem4.Size = new System.Drawing.Size(76, 26);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.Control = this.btnApply;
			this.layoutControlItem5.Location = new System.Drawing.Point(165, 13);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 5;
			this.layoutControlItem5.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem5.Size = new System.Drawing.Size(76, 26);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem6.Control = this.lblDelimit;
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.ColumnSpan = 8;
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem6.Size = new System.Drawing.Size(320, 13);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.layoutControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ComponentEditorForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.ComponentEditorForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void InitializePrintable(UserControl propertyEditor) {
			try {
				propertyPanel.SuspendLayout();
				SuspendLayout();
				Size size = propertyEditor.Size;
				propertyEditor.Dock = DockStyle.Fill;
				propertyPanel.Controls.Add(propertyEditor);
				this.ClientSize = new System.Drawing.Size(Math.Max(size.Width, grpButtons.Width), size.Height + grpButtons.Height);
				propertyPanel.ResumeLayout(false);
				ResumeLayout(false);
				if( Printable.SupportsHelp() ) btnHelp.Enabled = true;
			} catch { }
		}
		private void acceptButton_Click(object sender, System.EventArgs e) {
			try {
				Printable.AcceptChanges();
				OkButtonClicked();
			} catch { }
		}
		private void cancelButton_Click(object sender, System.EventArgs e) {
			try {
				Printable.RejectChanges();
			} catch { }
		}
		private void helpButton_Click(object sender, System.EventArgs e) {
			try {
				Printable.ShowHelp();
			} catch { }
		}
		private void OkButtonClicked() {
			PrintControl printControl = this.printableComponentLink.PrintingSystemBase.Extend().ActiveViewer;
			if(printControl != null) {
				int index = printControl.SelectedPageIndex;
				printableComponentLink.CreateDocument();
				printControl.SelectedPageIndex = index;
			}
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			isRTLChanged = true;
		}
		private void ComponentEditorForm_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height)
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));  
			if(isRTLChanged)
				DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(layoutControlGroup1);
		}
		void InitializeGroupButtonsLayout() {
			int btnOKBestWidth = btnOK.CalcBestSize().Width;
			int btnCancelBestWidth = btnCancel.CalcBestSize().Width;
			int btnApplyBestWidth = btnApply.CalcBestSize().Width;
			int btnHelpBestWidth = btnHelp.CalcBestSize().Width;
			if(btnOKBestWidth <= btnOK.Width && btnCancelBestWidth <= btnCancel.Width && btnApplyBestWidth <= btnApply.Width && btnHelpBestWidth <= btnHelp.Width)
				return;
			int btnsMaxActualSize = Math.Max(Math.Max(btnOKBestWidth, btnCancelBestWidth), Math.Max(btnApplyBestWidth, btnHelpBestWidth));
			int delta = btnsMaxActualSize - btnOK.Width;
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width = grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[5].Width = grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[7].Width = btnsMaxActualSize + 2 + 2;
			grpButtons.Width += 4 * delta;
		}
	}
}

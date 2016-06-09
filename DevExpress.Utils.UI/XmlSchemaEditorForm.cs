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
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Design;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraLayout;
namespace DevExpress.Utils.UI {
	public class XmlSchemaEditorForm : ReportsEditorFormBase {
		private XmlSchemaEditorControl xmlSchemaEditorControl;
		private LayoutControl layoutControl1;
		private SimpleButton btnCancel;
		private SimpleButton btnOk;
		private LayoutControlGroup Root;
		private LayoutControlItem layoutControlItem1;
		private LayoutControlItem layoutControlItem2;
		private LayoutControlItem layoutControlItem3;
		private LayoutControlGroup grpButtons;
		private System.ComponentModel.Container components = null;
		public object EditValue { get { return xmlSchemaEditorControl.EditValue; } }
		public XmlSchemaEditorForm(IServiceProvider provider)
			: base(provider) {
			InitializeComponent();
			xmlSchemaEditorControl.Initialize(provider.GetService<IDTEService>());
			LookAndFeelProviderHelper.SetParentLookAndFeel(xmlSchemaEditorControl, provider);
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlSchemaEditorForm));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			this.xmlSchemaEditorControl = new DevExpress.Utils.UI.XmlSchemaEditorControl();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.xmlSchemaEditorControl, "xmlSchemaEditorControl");
			this.xmlSchemaEditorControl.Name = "xmlSchemaEditorControl";
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.xmlSchemaEditorControl);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnOk);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(482, 113, 1112, 867);
			this.layoutControl1.Root = this.Root;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.StyleController = this.layoutControl1;
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3,
			this.grpButtons});
			this.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition5.Width = 330D;
			this.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition5});
			rowDefinition2.Height = 105D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 26D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition2,
			rowDefinition3});
			this.Root.Size = new System.Drawing.Size(350, 151);
			this.layoutControlItem3.Control = this.xmlSchemaEditorControl;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(330, 105);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 105);
			this.grpButtons.Name = "grpButtons";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition1.Width = 100D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 80D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 2D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 80D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4});
			rowDefinition1.Height = 26D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1});
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 1;
			this.grpButtons.Size = new System.Drawing.Size(330, 26);
			this.layoutControlItem1.Control = this.btnCancel;
			this.layoutControlItem1.Location = new System.Drawing.Point(250, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem1.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem2.Control = this.btnOk;
			this.layoutControlItem2.Location = new System.Drawing.Point(168, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem2.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.layoutControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "XmlSchemaEditorForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.XmlSchemaEditorForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);
		}
		void UpdateОkButton() {
			this.btnOk.Enabled = xmlSchemaEditorControl.EnableSubmit;
		}
		void xmlSchemaEditorControl_Updated(object sender, EventArgs e) {
			UpdateОkButton();
		}
		#endregion
		private void XmlSchemaEditorForm_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height) {
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
			}
		}
		void InitializeGroupButtonsLayout() {
			int btnOkBestWidth = btnOk.CalcBestSize().Width;
			int btnCancelBestWidth = btnCancel.CalcBestSize().Width;
			if(btnOkBestWidth <= btnOk.Width && btnCancelBestWidth <= btnCancel.Width)
				return;
			int btnCancelOKActualSize = Math.Max(btnOkBestWidth, btnCancelBestWidth);
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnCancelOKActualSize + 2 + 2;
			if(grpButtons.Width < 2 * (btnCancelOKActualSize + 2 + 2))
				grpButtons.Width = 2 * (btnCancelOKActualSize + 2 + 2);
		}
	}
	public class XmlSchemaEditor : UITypeEditor {
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if(provider != null) {
				try {
					XmlSchemaEditorForm editor = new XmlSchemaEditorForm(provider);
					if(DialogRunner.ShowDialog(editor, provider) == DialogResult.OK)
						objValue = editor.EditValue;
				} catch {
					return String.Empty;
				}
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}

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

using DevExpress.XtraLayout;
using System;
namespace DevExpress.Utils.UI {
	partial class XmlSchemaEditorControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlSchemaEditorControl));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.cbClassName = new DevExpress.XtraEditors.ComboBoxEdit();
			this.checkBox1 = new DevExpress.XtraEditors.CheckEdit();
			this.label1 = new DevExpress.XtraEditors.LabelControl();
			this.tbXmlUrl = new DevExpress.XtraEditors.ButtonEdit();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbClassName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkBox1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbXmlUrl.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.cbClassName);
			this.layoutControl1.Controls.Add(this.checkBox1);
			this.layoutControl1.Controls.Add(this.label1);
			this.layoutControl1.Controls.Add(this.tbXmlUrl);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(609, 201, 905, 598);
			this.layoutControl1.Root = this.Root;
			resources.ApplyResources(this.cbClassName, "cbClassName");
			this.cbClassName.Name = "cbClassName";
			this.cbClassName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbClassName.Properties.Buttons"))))});
			this.cbClassName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbClassName.StyleController = this.layoutControl1;
			this.cbClassName.SelectedValueChanged += new System.EventHandler(this.cbClassName_SelectedValueChanged);
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Properties.Caption = resources.GetString("checkBox1.Properties.Caption");
			this.checkBox1.StyleController = this.layoutControl1;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.label1.StyleController = this.layoutControl1;
			resources.ApplyResources(this.tbXmlUrl, "tbXmlUrl");
			this.tbXmlUrl.Name = "tbXmlUrl";
			this.tbXmlUrl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.tbXmlUrl.StyleController = this.layoutControl1;
			this.tbXmlUrl.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.tbXmlUrl_ButtonClick);
			this.tbXmlUrl.TextChanged += new System.EventHandler(this.tbXmlUrl_TextChanged);
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem4});
			this.Root.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition1.Width = 280D;
			this.Root.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1});
			rowDefinition1.Height = 17D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition2.Height = 24D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 23D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition4.Height = 24D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.Root.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3,
			rowDefinition4});
			this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.Root.Size = new System.Drawing.Size(280, 88);
			this.layoutControlItem1.Control = this.cbClassName;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 64);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.OptionsTableLayoutItem.RowIndex = 3;
			this.layoutControlItem1.Size = new System.Drawing.Size(280, 24);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem2.Control = this.checkBox1;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 41);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem2.Size = new System.Drawing.Size(280, 23);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem3.Control = this.label1;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(280, 17);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.Control = this.tbXmlUrl;
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 17);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem4.Size = new System.Drawing.Size(280, 24);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "XmlSchemaEditorControl";
			this.Load += new System.EventHandler(this.XmlSchemaEditorControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbClassName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkBox1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbXmlUrl.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public LayoutControl layoutControl1;
		private XtraEditors.ComboBoxEdit cbClassName;
		private XtraEditors.CheckEdit checkBox1;
		private XtraEditors.LabelControl label1;
		private XtraEditors.ButtonEdit tbXmlUrl;
		private LayoutControlGroup Root;
		private LayoutControlItem layoutControlItem1;
		private LayoutControlItem layoutControlItem2;
		private LayoutControlItem layoutControlItem3;
		private LayoutControlItem layoutControlItem4;
	}
}

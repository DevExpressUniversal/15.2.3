#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class ChoroplethMapTitleForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			DevExpress.XtraBars.BarAndDockingController barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
			DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoroplethMapTitleForm));
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.ceUseBindingAttributeForTooltipText = new DevExpress.XtraEditors.CheckEdit();
			this.cbTitleAttribute = new DevExpress.DashboardWin.Native.MapShapeAttributesComboBoxEdit();
			this.cbTooltipAttribute = new DevExpress.DashboardWin.Native.MapShapeAttributesComboBoxEdit();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceUseBindingAttributeForTooltipText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbTitleAttribute.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbTooltipAttribute.Properties)).BeginInit();
			this.SuspendLayout();
			barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barManager1.Controller = barAndDockingController1;
			this.barManager1.Form = this;
			this.barManager1.MaxItemId = 0;
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.ceUseBindingAttributeForTooltipText, "ceUseBindingAttributeForTooltipText");
			this.ceUseBindingAttributeForTooltipText.Name = "ceUseBindingAttributeForTooltipText";
			this.ceUseBindingAttributeForTooltipText.Properties.Caption = resources.GetString("ceUseBindingAttributeForTooltipText.Properties.Caption");
			this.ceUseBindingAttributeForTooltipText.CheckedChanged += new System.EventHandler(this.ceUseForDisplayText_CheckedChanged);
			resources.ApplyResources(this.cbTitleAttribute, "cbTitleAttribute");
			this.cbTitleAttribute.Name = "cbTitleAttribute";
			this.cbTitleAttribute.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTitleAttribute.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTitleAttribute.Properties.Buttons1"))), resources.GetString("cbTitleAttribute.Properties.Buttons2"), ((int)(resources.GetObject("cbTitleAttribute.Properties.Buttons3"))), ((bool)(resources.GetObject("cbTitleAttribute.Properties.Buttons4"))), ((bool)(resources.GetObject("cbTitleAttribute.Properties.Buttons5"))), ((bool)(resources.GetObject("cbTitleAttribute.Properties.Buttons6"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("cbTitleAttribute.Properties.Buttons7"))), ((System.Drawing.Image)(resources.GetObject("cbTitleAttribute.Properties.Buttons8"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject2, resources.GetString("cbTitleAttribute.Properties.Buttons9"), ((object)(resources.GetObject("cbTitleAttribute.Properties.Buttons10"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("cbTitleAttribute.Properties.Buttons11"))), ((bool)(resources.GetObject("cbTitleAttribute.Properties.Buttons12"))))});
			resources.ApplyResources(this.cbTooltipAttribute, "cbTooltipAttribute");
			this.cbTooltipAttribute.Name = "cbTooltipAttribute";
			this.cbTooltipAttribute.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTooltipAttribute.Properties.Buttons")))),
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbTooltipAttribute.Properties.Buttons1"))), resources.GetString("cbTooltipAttribute.Properties.Buttons2"), ((int)(resources.GetObject("cbTooltipAttribute.Properties.Buttons3"))), ((bool)(resources.GetObject("cbTooltipAttribute.Properties.Buttons4"))), ((bool)(resources.GetObject("cbTooltipAttribute.Properties.Buttons5"))), ((bool)(resources.GetObject("cbTooltipAttribute.Properties.Buttons6"))), ((DevExpress.XtraEditors.ImageLocation)(resources.GetObject("cbTooltipAttribute.Properties.Buttons7"))), ((System.Drawing.Image)(resources.GetObject("cbTooltipAttribute.Properties.Buttons8"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, resources.GetString("cbTooltipAttribute.Properties.Buttons9"), ((object)(resources.GetObject("cbTooltipAttribute.Properties.Buttons10"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("cbTooltipAttribute.Properties.Buttons11"))), ((bool)(resources.GetObject("cbTooltipAttribute.Properties.Buttons12"))))});
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this.imageList, "imageList");
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.cbTooltipAttribute);
			this.Controls.Add(this.cbTitleAttribute);
			this.Controls.Add(this.ceUseBindingAttributeForTooltipText);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.labelControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChoroplethMapTitleForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceUseBindingAttributeForTooltipText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbTitleAttribute.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbTooltipAttribute.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.CheckEdit ceUseBindingAttributeForTooltipText;
		private MapShapeAttributesComboBoxEdit cbTitleAttribute;
		private MapShapeAttributesComboBoxEdit cbTooltipAttribute;
		private System.Windows.Forms.ImageList imageList;
		private XtraBars.BarManager barManager1;
	}
}

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
	partial class OlapFilterForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OlapFilterForm));
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.separator = new DevExpress.XtraEditors.LabelControl();
			this.buttonsPanel = new DevExpress.XtraEditors.PanelControl();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.cbDimension = new DevExpress.XtraEditors.ComboBoxEdit();
			this.treeFilter = new DevExpress.DashboardWin.Native.CheckedListTreeViewControl();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanel)).BeginInit();
			this.buttonsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDimension.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.treeFilter)).BeginInit();
			this.SuspendLayout();
			barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barManager1.Controller = barAndDockingController1;
			this.barManager1.Form = this;
			this.barManager1.MaxItemId = 0;
			resources.ApplyResources(this.separator, "separator");
			this.separator.LineVisible = true;
			this.separator.Name = "separator";
			resources.ApplyResources(this.buttonsPanel, "buttonsPanel");
			this.buttonsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.buttonsPanel.Controls.Add(this.btnApply);
			this.buttonsPanel.Controls.Add(this.btnOK);
			this.buttonsPanel.Controls.Add(this.btnCancel);
			this.buttonsPanel.Name = "buttonsPanel";
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.cbDimension, "cbDimension");
			this.cbDimension.Name = "cbDimension";
			this.cbDimension.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDimension.Properties.Buttons"))))});
			this.cbDimension.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDimension.SelectedValueChanged += new System.EventHandler(this.OncbDimensionSelectedValueChanged);
			this.treeFilter.ActionsQueue = null;
			resources.ApplyResources(this.treeFilter, "treeFilter");
			this.treeFilter.CheckOnClick = true;
			this.treeFilter.IsList = false;
			this.treeFilter.Name = "treeFilter";
			this.treeFilter.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.treeFilter.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.treeFilter_ItemCheck);
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.treeFilter);
			this.Controls.Add(this.cbDimension);
			this.Controls.Add(this.separator);
			this.Controls.Add(this.buttonsPanel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OlapFilterForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanel)).EndInit();
			this.buttonsPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbDimension.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.treeFilter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.LabelControl separator;
		private XtraEditors.PanelControl buttonsPanel;
		private XtraEditors.SimpleButton btnApply;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.ComboBoxEdit cbDimension;
		private CheckedListTreeViewControl treeFilter;
		private XtraBars.BarManager barManager1;
	}
}

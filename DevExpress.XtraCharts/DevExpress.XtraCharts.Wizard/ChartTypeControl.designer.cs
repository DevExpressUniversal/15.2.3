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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Wizard {
	partial class ChartTypeControl {
		protected override void Dispose(bool disposing) {
			if (chart != null) {
				Controls.Remove(designControl);
				chart = null;
			}
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartTypeControl));
			this.imageList = new DevExpress.Utils.ImageCollection(this.components);
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.panelControl2 = new DevExpress.XtraCharts.Wizard.ChartPanelControl();
			this.cbCategory = new DevExpress.XtraEditors.ComboBoxEdit();
			this.pnlType = new DevExpress.XtraEditors.PanelControl();
			this.images = new DevExpress.XtraCharts.Design.ChartImageListBoxContainer();
			((System.ComponentModel.ISupportInitialize)(this.imageList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCategory.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlType)).BeginInit();
			this.pnlType.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.images)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.imageList, "imageList");
			this.imageList.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageList.ImageStream")));
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.panelControl2);
			this.panelControl1.Controls.Add(this.cbCategory);
			this.panelControl1.Name = "panelControl1";
			this.panelControl2.BackColor = System.Drawing.Color.Transparent;
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.cbCategory, "cbCategory");
			this.cbCategory.Name = "cbCategory";
			this.cbCategory.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbCategory.Properties.Buttons"))))});
			this.cbCategory.Properties.DropDownRows = 10;
			this.cbCategory.Properties.Items.AddRange(new object[] {
			resources.GetString("cbCategory.Properties.Items"),
			resources.GetString("cbCategory.Properties.Items1"),
			resources.GetString("cbCategory.Properties.Items2"),
			resources.GetString("cbCategory.Properties.Items3"),
			resources.GetString("cbCategory.Properties.Items4"),
			resources.GetString("cbCategory.Properties.Items5"),
			resources.GetString("cbCategory.Properties.Items6"),
			resources.GetString("cbCategory.Properties.Items7"),
			resources.GetString("cbCategory.Properties.Items8"),
			resources.GetString("cbCategory.Properties.Items9")});
			this.cbCategory.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbCategory.SelectedIndexChanged += new System.EventHandler(this.cbCategory_SelectedIndexChanged);
			this.pnlType.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlType.Controls.Add(this.images);
			resources.ApplyResources(this.pnlType, "pnlType");
			this.pnlType.Name = "pnlType";
			resources.ApplyResources(this.images, "images");
			this.images.Name = "images";
			this.images.SelectedIndex = -1;
			this.images.TabStop = true;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.pnlType);
			this.Controls.Add(this.panelControl1);
			this.Name = "ChartTypeControl";
			((System.ComponentModel.ISupportInitialize)(this.imageList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbCategory.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlType)).EndInit();
			this.pnlType.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.images)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
#endregion
		IContainer components;
		ImageCollection imageList;
		private PanelControl panelControl1;
		private PanelControl pnlType;
		private ChartImageListBoxContainer images;
		private ChartPanelControl panelControl2;
		private ComboBoxEdit cbCategory;
	}
}

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

namespace DevExpress.XtraCharts.Design {
	partial class SeriesPointFilterCollectionForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesPointFilterCollectionForm));
			this.rgConjunction = new DevExpress.XtraEditors.RadioGroup();
			this.lbFilters = new DevExpress.XtraEditors.ListBoxControl();
			this.propertyGrid = new DevExpress.Utils.Frames.PropertyGridEx();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.rgConjunction.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFilters)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.rgConjunction, "rgConjunction");
			this.rgConjunction.Name = "rgConjunction";
			this.rgConjunction.Properties.Columns = 2;
			this.rgConjunction.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgConjunction.Properties.Items"))), resources.GetString("rgConjunction.Properties.Items1")),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(((object)(resources.GetObject("rgConjunction.Properties.Items2"))), resources.GetString("rgConjunction.Properties.Items3"))});
			this.rgConjunction.SelectedIndexChanged += new System.EventHandler(this.rgConjunction_SelectedIndexChanged);
			resources.ApplyResources(this.lbFilters, "lbFilters");
			this.lbFilters.Name = "lbFilters";
			this.lbFilters.SelectedIndexChanged += new System.EventHandler(this.lbFilters_SelectedIndexChanged);
			resources.ApplyResources(this.propertyGrid, "propertyGrid");
			this.propertyGrid.CommandsActiveLinkColor = System.Drawing.SystemColors.ActiveCaption;
			this.propertyGrid.CommandsDisabledLinkColor = System.Drawing.SystemColors.ControlDark;
			this.propertyGrid.CommandsLinkColor = System.Drawing.SystemColors.ActiveCaption;
			this.propertyGrid.DrawFlat = true;
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.ToolbarVisible = false;
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Name = "btnClose";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.lbFilters);
			this.Controls.Add(this.rgConjunction);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SeriesPointFilterCollectionForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Load += new System.EventHandler(this.SeriesPointFilterCollectionForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.rgConjunction.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFilters)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.RadioGroup rgConjunction;
		private DevExpress.XtraEditors.ListBoxControl lbFilters;
		private DevExpress.Utils.Frames.PropertyGridEx propertyGrid;
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private DevExpress.XtraEditors.SimpleButton btnRemove;
		private DevExpress.XtraEditors.SimpleButton btnClose;
		private DevExpress.XtraEditors.LabelControl labelControl1;
	}
}

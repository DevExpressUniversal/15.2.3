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
	partial class AxisVisibilityInPanesForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxisVisibilityInPanesForm));
			this.footerPanel = new DevExpress.XtraEditors.PanelControl();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.contentPanel = new DevExpress.XtraEditors.PanelControl();
			this.listViewPanes = new System.Windows.Forms.ListView();
			this.visibleColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.paneColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.footerPanel)).BeginInit();
			this.footerPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.contentPanel)).BeginInit();
			this.contentPanel.SuspendLayout();
			this.SuspendLayout();
			this.footerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.footerPanel.Controls.Add(this.simpleButton1);
			resources.ApplyResources(this.footerPanel, "footerPanel");
			this.footerPanel.Name = "footerPanel";
			resources.ApplyResources(this.simpleButton1, "simpleButton1");
			this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.simpleButton1.Name = "simpleButton1";
			this.contentPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.contentPanel.Controls.Add(this.listViewPanes);
			resources.ApplyResources(this.contentPanel, "contentPanel");
			this.contentPanel.Name = "contentPanel";
			this.listViewPanes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listViewPanes.CheckBoxes = true;
			this.listViewPanes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.visibleColumn,
			this.paneColumn});
			resources.ApplyResources(this.listViewPanes, "listViewPanes");
			this.listViewPanes.FullRowSelect = true;
			this.listViewPanes.GridLines = true;
			this.listViewPanes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewPanes.HideSelection = false;
			this.listViewPanes.Name = "listViewPanes";
			this.listViewPanes.UseCompatibleStateImageBehavior = false;
			this.listViewPanes.View = System.Windows.Forms.View.Details;
			this.listViewPanes.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewPanes_ItemChecked);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.contentPanel);
			this.Controls.Add(this.footerPanel);
			this.Controls.Add(this.labelControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AxisVisibilityInPanesForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Resize += new System.EventHandler(this.AxisVisibilityInPanesForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.footerPanel)).EndInit();
			this.footerPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.contentPanel)).EndInit();
			this.contentPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl footerPanel;
		private DevExpress.XtraEditors.SimpleButton simpleButton1;
		private DevExpress.XtraEditors.PanelControl contentPanel;
		private System.Windows.Forms.ListView listViewPanes;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private System.Windows.Forms.ColumnHeader visibleColumn;
		private System.Windows.Forms.ColumnHeader paneColumn;
	}
}

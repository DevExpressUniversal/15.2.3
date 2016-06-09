#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	partial class ModelEditorPanelControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.propertyPanel = new DevExpress.XtraEditors.PanelControl();
			this.rootSplitContainer = new DevExpress.XtraEditors.SplitContainerControl();
			this.propertySplitContainer = new DevExpress.XtraEditors.SplitContainerControl();
			this.infoBoxPanel = new DevExpress.XtraEditors.PanelControl();
			this.labelControl1 = new XafInfoPanel();
			((System.ComponentModel.ISupportInitialize)(this.propertyPanel)).BeginInit();
			this.propertyPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rootSplitContainer)).BeginInit();
			this.rootSplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.propertySplitContainer)).BeginInit();
			this.propertySplitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.infoBoxPanel)).BeginInit();
			this.infoBoxPanel.SuspendLayout();
			this.SuspendLayout();
			this.propertyPanel.Controls.Add(this.rootSplitContainer);
			this.propertyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyPanel.Location = new System.Drawing.Point(0, 0);
			this.propertyPanel.Name = "propertyPanel";
			this.propertyPanel.Size = new System.Drawing.Size(776, 371);
			this.propertyPanel.TabIndex = 1;
			this.rootSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rootSplitContainer.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
			this.rootSplitContainer.Location = new System.Drawing.Point(2, 2);
			this.rootSplitContainer.Name = "rootSplitContainer";
			this.rootSplitContainer.Panel1.MinSize = 150;
			this.rootSplitContainer.Panel1.Text = "Panel1";
			this.rootSplitContainer.Panel2.Controls.Add(this.propertySplitContainer);
			this.rootSplitContainer.Panel2.MinSize = 150;
			this.rootSplitContainer.Panel2.Text = "Panel2";
			this.rootSplitContainer.Size = new System.Drawing.Size(772, 367);
			this.rootSplitContainer.SplitterPosition = 270;
			this.rootSplitContainer.TabIndex = 0;
			this.rootSplitContainer.Text = "splitContainerControl1";
			this.propertySplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertySplitContainer.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
			this.propertySplitContainer.Location = new System.Drawing.Point(0, 0);
			this.propertySplitContainer.Name = "propertySplitContainer";
			this.propertySplitContainer.Panel1.MinSize = 150;
			this.propertySplitContainer.Panel1.Text = "Panel1";
			this.propertySplitContainer.Panel2.MinSize = 150;
			this.propertySplitContainer.Panel2.Text = "Panel2";
			this.propertySplitContainer.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
			this.propertySplitContainer.Size = new System.Drawing.Size(497, 367);
			this.propertySplitContainer.SplitterPosition = 235;
			this.propertySplitContainer.TabIndex = 0;
			this.propertySplitContainer.Text = "splitContainerControl2";
			this.infoBoxPanel.Appearance.BackColor = System.Drawing.SystemColors.Info;
			this.infoBoxPanel.Appearance.Options.UseBackColor = true;
			this.infoBoxPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.infoBoxPanel.Controls.Add(this.labelControl1);
			this.infoBoxPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.infoBoxPanel.Location = new System.Drawing.Point(0, 371);
			this.infoBoxPanel.Name = "infoBoxPanel";
			this.infoBoxPanel.Size = new System.Drawing.Size(776, 80);
			this.infoBoxPanel.TabIndex = 0;
			this.infoBoxPanel.MinimumSize = new System.Drawing.Size(0, 35);
			this.labelControl1.UseHtmlString = true;
			this.labelControl1.Appearance.BackColor = System.Drawing.SystemColors.Info;
			this.labelControl1.Appearance.ForeColor = System.Drawing.SystemColors.InfoText;
			this.labelControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelControl1.Location = new System.Drawing.Point(2, 2);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(63, 13);
			this.labelControl1.TabIndex = 0;
			this.labelControl1.Text = "labelControl1";
			this.labelControl1.AutoSize = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.propertyPanel);
			this.Controls.Add(this.infoBoxPanel);
			this.Name = "ModelEditorPanelControl";
			this.Size = new System.Drawing.Size(776, 451);
			((System.ComponentModel.ISupportInitialize)(this.propertyPanel)).EndInit();
			this.propertyPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rootSplitContainer)).EndInit();
			this.rootSplitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.propertySplitContainer)).EndInit();
			this.propertySplitContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.infoBoxPanel)).EndInit();
			this.infoBoxPanel.ResumeLayout(false);
			this.infoBoxPanel.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.PanelControl propertyPanel;
		private DevExpress.XtraEditors.SplitContainerControl rootSplitContainer;
		private DevExpress.XtraEditors.SplitContainerControl propertySplitContainer;
		private DevExpress.XtraEditors.PanelControl infoBoxPanel;
		private XafInfoPanel labelControl1;
	}
}

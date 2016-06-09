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

using System;
namespace DevExpress.ExpressApp.Design {
	partial class XafApplicationRootDesignerGUI {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(designer != null) {
					try {
						designer.SelectionService.SelectionChanged -= new EventHandler(SelectionService_SelectionChanged);
					}
					catch { }
				}
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
			this.headerPanelSplitContainer = new System.Windows.Forms.SplitContainer();
			this.applicationGroupBox = new System.Windows.Forms.GroupBox();
			this.applicationTray = new DevExpress.ExpressApp.Design.ApplicationTray();
			this.securitySplitContainer = new System.Windows.Forms.SplitContainer();
			this.securityGroupBox = new System.Windows.Forms.GroupBox();
			this.securityTray = new DevExpress.ExpressApp.Design.SecurityTray();
			this.connectionGroupBox = new System.Windows.Forms.GroupBox();
			this.connectionTray = new DevExpress.ExpressApp.Design.ConnectionTray();
			this.modulesGroupBox = new System.Windows.Forms.GroupBox();
			this.modulesSplitContainer = new System.Windows.Forms.SplitContainer();
			this.modulesTreeView = new DevExpress.ExpressApp.Design.ModulesTreeViewTray(this.components);
			this.modulesImageList = new System.Windows.Forms.ImageList(this.components);
			this.selectedModulePanel = new System.Windows.Forms.Panel();
			this.moduleDependensiesPanel = new System.Windows.Forms.Panel();
			this.moduleDetailsSplitContainer = new System.Windows.Forms.SplitContainer();
			this.controllersTreeView = new DevExpress.ExpressApp.Design.ControllersTreeViewTray(this.components);
			this.controllersImageList = new System.Windows.Forms.ImageList(this.components);
			this.conrollersHeaderPanel = new System.Windows.Forms.Panel();
			this.controllersHeaderLabel = new System.Windows.Forms.Label();
			this.businessClassesTreeView = new DevExpress.ExpressApp.Design.BusinessClassesTreeViewTray(this.components);
			this.persistentObjectsImageList = new System.Windows.Forms.ImageList(this.components);
			this.persistentObjectsHeaderPanel = new System.Windows.Forms.Panel();
			this.persistentObjectsHeaderLabel = new System.Windows.Forms.Label();
			this.moduleInfoPanel = new System.Windows.Forms.Panel();
			this.moduleAssemblyLabel = new System.Windows.Forms.Label();
			this.moduleDescriptionLabel = new System.Windows.Forms.Label();
			this.moduleNameLabel = new System.Windows.Forms.Label();
			this.mainSplitContainer.Panel1.SuspendLayout();
			this.mainSplitContainer.Panel2.SuspendLayout();
			this.mainSplitContainer.SuspendLayout();
			this.headerPanelSplitContainer.Panel1.SuspendLayout();
			this.headerPanelSplitContainer.Panel2.SuspendLayout();
			this.headerPanelSplitContainer.SuspendLayout();
			this.applicationGroupBox.SuspendLayout();
			this.securitySplitContainer.Panel1.SuspendLayout();
			this.securitySplitContainer.Panel2.SuspendLayout();
			this.securitySplitContainer.SuspendLayout();
			this.securityGroupBox.SuspendLayout();
			this.connectionGroupBox.SuspendLayout();
			this.modulesGroupBox.SuspendLayout();
			this.modulesSplitContainer.Panel1.SuspendLayout();
			this.modulesSplitContainer.Panel2.SuspendLayout();
			this.modulesSplitContainer.SuspendLayout();
			this.selectedModulePanel.SuspendLayout();
			this.moduleDependensiesPanel.SuspendLayout();
			this.moduleDetailsSplitContainer.Panel1.SuspendLayout();
			this.moduleDetailsSplitContainer.Panel2.SuspendLayout();
			this.moduleDetailsSplitContainer.SuspendLayout();
			this.conrollersHeaderPanel.SuspendLayout();
			this.persistentObjectsHeaderPanel.SuspendLayout();
			this.moduleInfoPanel.SuspendLayout();
			this.SuspendLayout();
			this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.mainSplitContainer.Name = "mainSplitContainer";
			this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.mainSplitContainer.Panel1.Controls.Add(this.headerPanelSplitContainer);
			this.mainSplitContainer.Panel2.Controls.Add(this.modulesGroupBox);
			this.mainSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(5, 0, 5, 5);
			this.mainSplitContainer.Size = new System.Drawing.Size(695, 612);
			this.mainSplitContainer.SplitterDistance = 120;
			this.mainSplitContainer.TabIndex = 5;
			this.mainSplitContainer.TabStop = false;
			this.headerPanelSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.headerPanelSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.headerPanelSplitContainer.Name = "headerPanelSplitContainer";
			this.headerPanelSplitContainer.Panel1.Controls.Add(this.applicationGroupBox);
			this.headerPanelSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(5, 5, 3, 2);
			this.headerPanelSplitContainer.Panel2.Controls.Add(this.securitySplitContainer);
			this.headerPanelSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3, 5, 5, 2);
			this.headerPanelSplitContainer.Size = new System.Drawing.Size(695, 120);
			this.headerPanelSplitContainer.SplitterDistance = 124;
			this.headerPanelSplitContainer.TabIndex = 1;
			this.headerPanelSplitContainer.TabStop = false;
			this.applicationGroupBox.Controls.Add(this.applicationTray);
			this.applicationGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.applicationGroupBox.Location = new System.Drawing.Point(5, 5);
			this.applicationGroupBox.Name = "applicationGroupBox";
			this.applicationGroupBox.Size = new System.Drawing.Size(116, 113);
			this.applicationGroupBox.TabIndex = 0;
			this.applicationGroupBox.TabStop = false;
			this.applicationGroupBox.Text = "Application";
			this.applicationTray.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.applicationTray.Dock = System.Windows.Forms.DockStyle.Fill;
			this.applicationTray.Location = new System.Drawing.Point(3, 16);
			this.applicationTray.Name = "applicationTray";
			this.applicationTray.Size = new System.Drawing.Size(110, 94);
			this.applicationTray.TabIndex = 1;
			this.applicationTray.UseCompatibleStateImageBehavior = false;
			this.securitySplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.securitySplitContainer.Location = new System.Drawing.Point(3, 5);
			this.securitySplitContainer.Name = "securitySplitContainer";
			this.securitySplitContainer.Panel1.Controls.Add(this.securityGroupBox);
			this.securitySplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.securitySplitContainer.Panel2.Controls.Add(this.connectionGroupBox);
			this.securitySplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.securitySplitContainer.Size = new System.Drawing.Size(559, 113);
			this.securitySplitContainer.SplitterDistance = 390;
			this.securitySplitContainer.TabIndex = 1;
			this.securitySplitContainer.TabStop = false;
			this.securityGroupBox.Controls.Add(this.securityTray);
			this.securityGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.securityGroupBox.Location = new System.Drawing.Point(0, 0);
			this.securityGroupBox.Name = "securityGroupBox";
			this.securityGroupBox.Size = new System.Drawing.Size(220, 113);
			this.securityGroupBox.TabIndex = 0;
			this.securityGroupBox.TabStop = false;
			this.securityGroupBox.Text = "Security";
			this.securityTray.AllowDrop = true;
			this.securityTray.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.securityTray.Dock = System.Windows.Forms.DockStyle.Fill;
			this.securityTray.Location = new System.Drawing.Point(3, 16);
			this.securityTray.MultiSelect = false;
			this.securityTray.Name = "securityTray";
			this.securityTray.Size = new System.Drawing.Size(214, 94);
			this.securityTray.TabIndex = 2;
			this.securityTray.UseCompatibleStateImageBehavior = false;
			this.connectionGroupBox.Controls.Add(this.connectionTray);
			this.connectionGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.connectionGroupBox.Location = new System.Drawing.Point(3, 0);
			this.connectionGroupBox.Name = "connectionGroupBox";
			this.connectionGroupBox.Size = new System.Drawing.Size(329, 113);
			this.connectionGroupBox.TabIndex = 1;
			this.connectionGroupBox.TabStop = false;
			this.connectionGroupBox.Text = "Connection";
			this.connectionTray.AllowDrop = true;
			this.connectionTray.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.connectionTray.Dock = System.Windows.Forms.DockStyle.Fill;
			this.connectionTray.Location = new System.Drawing.Point(3, 16);
			this.connectionTray.Name = "connectionTray";
			this.connectionTray.Size = new System.Drawing.Size(323, 94);
			this.connectionTray.TabIndex = 0;
			this.connectionTray.UseCompatibleStateImageBehavior = false;
			this.modulesGroupBox.Controls.Add(this.modulesSplitContainer);
			this.modulesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modulesGroupBox.Location = new System.Drawing.Point(5, 0);
			this.modulesGroupBox.Name = "modulesGroupBox";
			this.modulesGroupBox.Padding = new System.Windows.Forms.Padding(10, 6, 10, 10);
			this.modulesGroupBox.Size = new System.Drawing.Size(685, 483);
			this.modulesGroupBox.TabIndex = 0;
			this.modulesGroupBox.TabStop = false;
			this.modulesGroupBox.Text = "Modules";
			this.modulesSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modulesSplitContainer.Location = new System.Drawing.Point(10, 19);
			this.modulesSplitContainer.Name = "modulesSplitContainer";
			this.modulesSplitContainer.Panel1.Controls.Add(this.modulesTreeView);
			this.modulesSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.modulesSplitContainer.Panel2.AutoScroll = true;
			this.modulesSplitContainer.Panel2.Controls.Add(this.selectedModulePanel);
			this.modulesSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.modulesSplitContainer.Size = new System.Drawing.Size(665, 454);
			this.modulesSplitContainer.SplitterDistance = 240;
			this.modulesSplitContainer.TabIndex = 0;
			this.modulesSplitContainer.TabStop = false;
			this.modulesTreeView.AllowDrop = true;
			this.modulesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modulesTreeView.HideSelection = false;
			this.modulesTreeView.ImageIndex = 0;
			this.modulesTreeView.ImageList = this.modulesImageList;
			this.modulesTreeView.Location = new System.Drawing.Point(0, 0);
			this.modulesTreeView.Name = "modulesTreeView";
			this.modulesTreeView.SelectedImageIndex = 0;
			this.modulesTreeView.SelectedObject = null;
			this.modulesTreeView.ShowNodeToolTips = true;
			this.modulesTreeView.Size = new System.Drawing.Size(237, 454);
			this.modulesTreeView.TabIndex = 0;
			this.modulesImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.modulesImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.modulesImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.selectedModulePanel.Controls.Add(this.moduleDependensiesPanel);
			this.selectedModulePanel.Controls.Add(this.moduleInfoPanel);
			this.selectedModulePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.selectedModulePanel.Location = new System.Drawing.Point(3, 0);
			this.selectedModulePanel.Name = "selectedModulePanel";
			this.selectedModulePanel.Size = new System.Drawing.Size(418, 454);
			this.selectedModulePanel.TabIndex = 0;
			this.moduleDependensiesPanel.Controls.Add(this.moduleDetailsSplitContainer);
			this.moduleDependensiesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.moduleDependensiesPanel.Location = new System.Drawing.Point(0, 104);
			this.moduleDependensiesPanel.Name = "moduleDependensiesPanel";
			this.moduleDependensiesPanel.Size = new System.Drawing.Size(418, 350);
			this.moduleDependensiesPanel.TabIndex = 9;
			this.moduleDetailsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.moduleDetailsSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.moduleDetailsSplitContainer.Name = "moduleDetailsSplitContainer";
			this.moduleDetailsSplitContainer.Panel1.Controls.Add(this.controllersTreeView);
			this.moduleDetailsSplitContainer.Panel1.Controls.Add(this.conrollersHeaderPanel);
			this.moduleDetailsSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.moduleDetailsSplitContainer.Panel2.Controls.Add(this.businessClassesTreeView);
			this.moduleDetailsSplitContainer.Panel2.Controls.Add(this.persistentObjectsHeaderPanel);
			this.moduleDetailsSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.moduleDetailsSplitContainer.Size = new System.Drawing.Size(418, 350);
			this.moduleDetailsSplitContainer.SplitterDistance = 207;
			this.moduleDetailsSplitContainer.TabIndex = 8;
			this.moduleDetailsSplitContainer.TabStop = false;
			this.controllersTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controllersTreeView.ImageIndex = 0;
			this.controllersTreeView.ImageList = this.controllersImageList;
			this.controllersTreeView.Location = new System.Drawing.Point(0, 25);
			this.controllersTreeView.Name = "controllersTreeView";
			this.controllersTreeView.SelectedImageIndex = 0;
			this.controllersTreeView.SelectedObject = null;
			this.controllersTreeView.Size = new System.Drawing.Size(204, 325);
			this.controllersTreeView.TabIndex = 2;
			this.controllersImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.controllersImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.controllersImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.conrollersHeaderPanel.Controls.Add(this.controllersHeaderLabel);
			this.conrollersHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.conrollersHeaderPanel.Location = new System.Drawing.Point(0, 0);
			this.conrollersHeaderPanel.Name = "conrollersHeaderPanel";
			this.conrollersHeaderPanel.Size = new System.Drawing.Size(204, 25);
			this.conrollersHeaderPanel.TabIndex = 7;
			this.controllersHeaderLabel.AutoSize = true;
			this.controllersHeaderLabel.Location = new System.Drawing.Point(3, 6);
			this.controllersHeaderLabel.Name = "controllersHeaderLabel";
			this.controllersHeaderLabel.Size = new System.Drawing.Size(59, 13);
			this.controllersHeaderLabel.TabIndex = 0;
			this.controllersHeaderLabel.Text = "Controllers:";
			this.businessClassesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.businessClassesTreeView.ImageIndex = 0;
			this.businessClassesTreeView.ImageList = this.persistentObjectsImageList;
			this.businessClassesTreeView.Location = new System.Drawing.Point(3, 25);
			this.businessClassesTreeView.Name = "persistentObjectsTreeView";
			this.businessClassesTreeView.SelectedImageIndex = 0;
			this.businessClassesTreeView.SelectedObject = null;
			this.businessClassesTreeView.Size = new System.Drawing.Size(204, 325);
			this.businessClassesTreeView.TabIndex = 1;
			this.persistentObjectsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.persistentObjectsImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.persistentObjectsImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.persistentObjectsHeaderPanel.Controls.Add(this.persistentObjectsHeaderLabel);
			this.persistentObjectsHeaderPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.persistentObjectsHeaderPanel.Location = new System.Drawing.Point(3, 0);
			this.persistentObjectsHeaderPanel.Name = "persistentObjectsHeaderPanel";
			this.persistentObjectsHeaderPanel.Size = new System.Drawing.Size(204, 25);
			this.persistentObjectsHeaderPanel.TabIndex = 8;
			this.persistentObjectsHeaderLabel.AutoSize = true;
			this.persistentObjectsHeaderLabel.Location = new System.Drawing.Point(3, 6);
			this.persistentObjectsHeaderLabel.Name = "persistentObjectsHeaderLabel";
			this.persistentObjectsHeaderLabel.Size = new System.Drawing.Size(91, 13);
			this.persistentObjectsHeaderLabel.TabIndex = 0;
			this.persistentObjectsHeaderLabel.Text = "Exported Types:";
			this.moduleInfoPanel.BackColor = System.Drawing.Color.Transparent;
			this.moduleInfoPanel.Controls.Add(this.moduleAssemblyLabel);
			this.moduleInfoPanel.Controls.Add(this.moduleDescriptionLabel);
			this.moduleInfoPanel.Controls.Add(this.moduleNameLabel);
			this.moduleInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.moduleInfoPanel.Location = new System.Drawing.Point(0, 0);
			this.moduleInfoPanel.Name = "moduleInfoPanel";
			this.moduleInfoPanel.Size = new System.Drawing.Size(418, 104);
			this.moduleInfoPanel.TabIndex = 8;
			this.moduleAssemblyLabel.AutoSize = true;
			this.moduleAssemblyLabel.Location = new System.Drawing.Point(3, 32);
			this.moduleAssemblyLabel.Name = "moduleAssemblyLabel";
			this.moduleAssemblyLabel.Size = new System.Drawing.Size(57, 13);
			this.moduleAssemblyLabel.TabIndex = 4;
			this.moduleAssemblyLabel.Text = "Assembly: ";
			this.moduleDescriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.moduleDescriptionLabel.AutoEllipsis = true;
			this.moduleDescriptionLabel.Location = new System.Drawing.Point(3, 54);
			this.moduleDescriptionLabel.Name = "moduleDescriptionLabel";
			this.moduleDescriptionLabel.Size = new System.Drawing.Size(412, 47);
			this.moduleDescriptionLabel.TabIndex = 3;
			this.moduleDescriptionLabel.Text = "Description: ";
			this.moduleNameLabel.AutoSize = true;
			this.moduleNameLabel.Location = new System.Drawing.Point(3, 10);
			this.moduleNameLabel.Name = "moduleNameLabel";
			this.moduleNameLabel.Size = new System.Drawing.Size(48, 13);
			this.moduleNameLabel.TabIndex = 2;
			this.moduleNameLabel.Text = "Module: ";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainSplitContainer);
			this.Name = "XafApplicationDesignerGUI";
			this.Size = new System.Drawing.Size(695, 612);
			this.mainSplitContainer.Panel1.ResumeLayout(false);
			this.mainSplitContainer.Panel2.ResumeLayout(false);
			this.mainSplitContainer.ResumeLayout(false);
			this.headerPanelSplitContainer.Panel1.ResumeLayout(false);
			this.headerPanelSplitContainer.Panel2.ResumeLayout(false);
			this.headerPanelSplitContainer.ResumeLayout(false);
			this.applicationGroupBox.ResumeLayout(false);
			this.securitySplitContainer.Panel1.ResumeLayout(false);
			this.securitySplitContainer.Panel2.ResumeLayout(false);
			this.securitySplitContainer.ResumeLayout(false);
			this.securityGroupBox.ResumeLayout(false);
			this.connectionGroupBox.ResumeLayout(false);
			this.modulesGroupBox.ResumeLayout(false);
			this.modulesSplitContainer.Panel1.ResumeLayout(false);
			this.modulesSplitContainer.Panel2.ResumeLayout(false);
			this.modulesSplitContainer.ResumeLayout(false);
			this.selectedModulePanel.ResumeLayout(false);
			this.moduleDependensiesPanel.ResumeLayout(false);
			this.moduleDetailsSplitContainer.Panel1.ResumeLayout(false);
			this.moduleDetailsSplitContainer.Panel2.ResumeLayout(false);
			this.moduleDetailsSplitContainer.ResumeLayout(false);
			this.conrollersHeaderPanel.ResumeLayout(false);
			this.conrollersHeaderPanel.PerformLayout();
			this.persistentObjectsHeaderPanel.ResumeLayout(false);
			this.persistentObjectsHeaderPanel.PerformLayout();
			this.moduleInfoPanel.ResumeLayout(false);
			this.moduleInfoPanel.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.SplitContainer mainSplitContainer;
		private System.Windows.Forms.SplitContainer headerPanelSplitContainer;
		private System.Windows.Forms.GroupBox securityGroupBox;
		private SecurityTray securityTray;
		private System.Windows.Forms.GroupBox applicationGroupBox;
		private ApplicationTray applicationTray;
		private System.Windows.Forms.GroupBox modulesGroupBox;
		private System.Windows.Forms.SplitContainer modulesSplitContainer;
		private ModulesTreeViewTray modulesTreeView;
		private System.Windows.Forms.Panel selectedModulePanel;
		private System.Windows.Forms.ImageList controllersImageList;
		private System.Windows.Forms.ImageList persistentObjectsImageList;
		private System.Windows.Forms.Panel moduleInfoPanel;
		private System.Windows.Forms.Label moduleDescriptionLabel;
		private System.Windows.Forms.Label moduleNameLabel;
		private System.Windows.Forms.Panel moduleDependensiesPanel;
		private System.Windows.Forms.SplitContainer moduleDetailsSplitContainer;
		private ControllersTreeViewTray controllersTreeView;
		private BusinessClassesTreeViewTray businessClassesTreeView;
		private System.Windows.Forms.Label moduleAssemblyLabel;
		private System.Windows.Forms.ImageList modulesImageList;
		private System.Windows.Forms.Panel conrollersHeaderPanel;
		private System.Windows.Forms.Label controllersHeaderLabel;
		private System.Windows.Forms.Panel persistentObjectsHeaderPanel;
		private System.Windows.Forms.Label persistentObjectsHeaderLabel;
		private System.Windows.Forms.SplitContainer securitySplitContainer;
		private System.Windows.Forms.GroupBox connectionGroupBox;
		private ConnectionTray connectionTray;
	}
}

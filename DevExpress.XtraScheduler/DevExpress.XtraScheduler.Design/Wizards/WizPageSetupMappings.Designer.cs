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

namespace DevExpress.XtraScheduler.Design.Wizards {
	partial class WizPageSetupMappings<T> {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.propertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
			this.btnGenerateMappings = new DevExpress.XtraEditors.SimpleButton();
			this.btnClearMappings = new DevExpress.XtraEditors.SimpleButton();
			this.lblDescription = new DevExpress.XtraEditors.LabelControl();
			this.lblRequiredMappingDescription = new DevExpress.XtraEditors.LabelControl();
			this.gridPanel = new System.Windows.Forms.TableLayoutPanel();
			this.buttonPanel = new System.Windows.Forms.Panel();
			this.ucExtendedArea = new DevExpress.XtraScheduler.Design.SetupMappingWizardExtensionControl();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).BeginInit();
			this.gridPanel.SuspendLayout();
			this.buttonPanel.SuspendLayout();
			this.SuspendLayout();
			this.titleLabel.Text = "Standard Properties Mappings";
			this.subtitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.subtitleLabel.Text = "Set up relationships between standard properties and the data fields. Choose the " +
	"data fields name in a drop-down list or use buttons on the right pane.";
			this.headerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.headerPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.headerSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.headerSeparator.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.headerSeparator.Size = new System.Drawing.Size(485, 2);
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(3, 3);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.OptionsBehavior.ResizeRowHeaders = false;
			this.propertyGrid.OptionsBehavior.ResizeRowValues = false;
			this.propertyGrid.OptionsView.ShowRootCategories = false;
			this.gridPanel.SetRowSpan(this.propertyGrid, 4);
			this.propertyGrid.Size = new System.Drawing.Size(230, 249);
			this.propertyGrid.TabIndex = 5;
			this.propertyGrid.CustomPropertyDescriptors += new DevExpress.XtraVerticalGrid.Events.CustomPropertyDescriptorsEventHandler(this.propertyGrid_CustomPropertyDescriptors);
			this.btnGenerateMappings.Location = new System.Drawing.Point(3, 0);
			this.btnGenerateMappings.Name = "btnGenerateMappings";
			this.btnGenerateMappings.Size = new System.Drawing.Size(75, 23);
			this.btnGenerateMappings.TabIndex = 6;
			this.btnGenerateMappings.Text = "Generate";
			this.btnGenerateMappings.Click += new System.EventHandler(this.OnBtnGenerateMappingsClick);
			this.btnClearMappings.Location = new System.Drawing.Point(84, 0);
			this.btnClearMappings.Name = "btnClearMappings";
			this.btnClearMappings.Size = new System.Drawing.Size(75, 23);
			this.btnClearMappings.TabIndex = 6;
			this.btnClearMappings.Text = "Clear";
			this.btnClearMappings.Click += new System.EventHandler(this.OnBtnClearMappingsClick);
			this.lblDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lblDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblDescription.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblDescription.Location = new System.Drawing.Point(239, 33);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(230, 96);
			this.lblDescription.TabIndex = 7;
			this.lblRequiredMappingDescription.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.lblRequiredMappingDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lblRequiredMappingDescription.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblRequiredMappingDescription.Cursor = System.Windows.Forms.Cursors.Default;
			this.lblRequiredMappingDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblRequiredMappingDescription.Location = new System.Drawing.Point(239, 237);
			this.lblRequiredMappingDescription.Name = "lblRequiredMappingDescription";
			this.lblRequiredMappingDescription.Size = new System.Drawing.Size(230, 15);
			this.lblRequiredMappingDescription.TabIndex = 8;
			this.gridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.gridPanel.ColumnCount = 2;
			this.gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.gridPanel.Controls.Add(this.propertyGrid, 0, 0);
			this.gridPanel.Controls.Add(this.ucExtendedArea, 1, 2);
			this.gridPanel.Controls.Add(this.lblDescription, 1, 1);
			this.gridPanel.Controls.Add(this.lblRequiredMappingDescription, 1, 3);
			this.gridPanel.Controls.Add(this.buttonPanel, 1, 0);
			this.gridPanel.Location = new System.Drawing.Point(0, 58);
			this.gridPanel.Name = "gridPanel";
			this.gridPanel.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.gridPanel.RowCount = 4;
			this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.gridPanel.Size = new System.Drawing.Size(492, 255);
			this.gridPanel.TabIndex = 10;
			this.buttonPanel.Controls.Add(this.btnGenerateMappings);
			this.buttonPanel.Controls.Add(this.btnClearMappings);
			this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonPanel.Location = new System.Drawing.Point(239, 3);
			this.buttonPanel.Name = "buttonPanel";
			this.buttonPanel.Size = new System.Drawing.Size(230, 24);
			this.buttonPanel.TabIndex = 10;
			this.ucExtendedArea.Caption = "";
			this.ucExtendedArea.Description = "About";
			this.ucExtendedArea.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ucExtendedArea.IsRestrictionActive = false;
			this.ucExtendedArea.Link = "asdfasdf";
			this.ucExtendedArea.LinkCaption = "";
			this.ucExtendedArea.Location = new System.Drawing.Point(236, 132);
			this.ucExtendedArea.Margin = new System.Windows.Forms.Padding(0);
			this.ucExtendedArea.Name = "ucExtendedArea";
			this.ucExtendedArea.Size = new System.Drawing.Size(236, 102);
			this.ucExtendedArea.TabIndex = 9;
			this.Controls.Add(this.gridPanel);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "WizPageSetupMappings";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.Controls.SetChildIndex(this.gridPanel, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).EndInit();
			this.gridPanel.ResumeLayout(false);
			this.buttonPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraVerticalGrid.PropertyGridControl propertyGrid;
		private DevExpress.XtraEditors.SimpleButton btnGenerateMappings;
		private DevExpress.XtraEditors.SimpleButton btnClearMappings;
		private DevExpress.XtraEditors.LabelControl lblDescription;
		private DevExpress.XtraEditors.LabelControl lblRequiredMappingDescription;
		private SetupMappingWizardExtensionControl ucExtendedArea;
		private System.Windows.Forms.TableLayoutPanel gridPanel;
		private System.Windows.Forms.Panel buttonPanel;
	}
}

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

namespace DevExpress.DataAccess.UI.Wizard.Views {
	partial class ChooseEFContextPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.buttonBrowseForAssembly = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.listBoxContext = new DevExpress.XtraEditors.ListBoxControl();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemContextsList = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemBrowseButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.openDialog = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
			this.layoutControlBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).BeginInit();
			this.panelBaseContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBoxContext)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContextsList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBrowseButton)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.buttonBrowseForAssembly.Location = new System.Drawing.Point(456, 12);
			this.buttonBrowseForAssembly.Name = "buttonBrowseForAssembly";
			this.buttonBrowseForAssembly.Size = new System.Drawing.Size(103, 22);
			this.buttonBrowseForAssembly.StyleController = this.layoutControlContent;
			this.buttonBrowseForAssembly.TabIndex = 20;
			this.buttonBrowseForAssembly.Text = "Browse...";
			this.buttonBrowseForAssembly.Visible = false;
			this.buttonBrowseForAssembly.Click += new System.EventHandler(this.buttonBrowseForAssembly_Click);
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.buttonBrowseForAssembly);
			this.layoutControlContent.Controls.Add(this.listBoxContext);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(353, 234, 896, 655);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 21;
			this.listBoxContext.Location = new System.Drawing.Point(47, 12);
			this.listBoxContext.Name = "listBoxContext";
			this.listBoxContext.Size = new System.Drawing.Size(397, 313);
			this.listBoxContext.StyleController = this.layoutControlContent;
			this.listBoxContext.TabIndex = 17;
			this.listBoxContext.SelectedValueChanged += new System.EventHandler(this.contextList_SelectedValueChanged);
			this.listBoxContext.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.contextList_MouseDoubleClick);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemContextsList,
			this.layoutItemBrowseButton});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "Root";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(45, 45, 10, 10);
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutItemContextsList.Control = this.listBoxContext;
			this.layoutItemContextsList.Location = new System.Drawing.Point(0, 0);
			this.layoutItemContextsList.Name = "layoutItemContextsList";
			this.layoutItemContextsList.Size = new System.Drawing.Size(401, 317);
			this.layoutItemContextsList.TextLocation = DevExpress.Utils.Locations.Top;
			this.layoutItemContextsList.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemContextsList.TextVisible = false;
			this.layoutItemBrowseButton.Control = this.buttonBrowseForAssembly;
			this.layoutItemBrowseButton.Location = new System.Drawing.Point(401, 0);
			this.layoutItemBrowseButton.Name = "layoutItemBrowseButton";
			this.layoutItemBrowseButton.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
			this.layoutItemBrowseButton.Size = new System.Drawing.Size(115, 317);
			this.layoutItemBrowseButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemBrowseButton.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(48, 22, 48, 22);
			this.Name = "ChooseEFContextPageView";
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
			this.layoutControlBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).EndInit();
			this.panelBaseContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listBoxContext)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContextsList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBrowseButton)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraEditors.SimpleButton buttonBrowseForAssembly;
		protected System.Windows.Forms.OpenFileDialog openDialog;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemBrowseButton;
		protected XtraLayout.LayoutControlItem layoutItemContextsList;
		protected XtraEditors.ListBoxControl listBoxContext;
	}
}

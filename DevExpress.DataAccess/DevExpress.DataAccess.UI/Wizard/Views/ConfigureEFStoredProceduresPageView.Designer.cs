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

using DevExpress.DataAccess.UI.Native.ParametersGrid;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	partial class ConfigureEFStoredProceduresPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.buttonPreview = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlButtons = new DevExpress.XtraLayout.LayoutControl();
			this.buttonRemove = new DevExpress.XtraEditors.SimpleButton();
			this.buttonAdd = new DevExpress.XtraEditors.SimpleButton();
			this.layoutGroupButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemPreviewButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceButtons = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutItemAddButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemRemoveButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.parametersGrid = new DevExpress.DataAccess.UI.Native.ParametersGrid.ParametersGrid();
			this.listBoxProcedures = new DevExpress.XtraEditors.ListBoxControl();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemParametersGrid = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemProceduresList = new DevExpress.XtraLayout.LayoutControlItem();
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
			this.panelAdditionalButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlButtons)).BeginInit();
			this.layoutControlButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviewButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAddButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemRemoveButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listBoxProcedures)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemParametersGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemProceduresList)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(689, 133, 1091, 739);
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelAdditionalButtons.Controls.Add(this.layoutControlButtons);
			this.buttonPreview.Enabled = false;
			this.buttonPreview.Location = new System.Drawing.Point(189, 2);
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.Size = new System.Drawing.Size(83, 22);
			this.buttonPreview.StyleController = this.layoutControlButtons;
			this.buttonPreview.TabIndex = 12;
			this.buttonPreview.Text = "&Preview...";
			this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
			this.layoutControlButtons.Controls.Add(this.buttonRemove);
			this.layoutControlButtons.Controls.Add(this.buttonAdd);
			this.layoutControlButtons.Controls.Add(this.buttonPreview);
			this.layoutControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutControlButtons.Name = "layoutControlButtons";
			this.layoutControlButtons.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2391, 145, 895, 649);
			this.layoutControlButtons.Root = this.layoutGroupButtons;
			this.layoutControlButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutControlButtons.TabIndex = 0;
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Location = new System.Drawing.Point(100, 2);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(83, 22);
			this.buttonRemove.StyleController = this.layoutControlButtons;
			this.buttonRemove.TabIndex = 14;
			this.buttonRemove.Text = "&Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.buttonAdd.Location = new System.Drawing.Point(11, 2);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(83, 22);
			this.buttonAdd.StyleController = this.layoutControlButtons;
			this.buttonAdd.TabIndex = 13;
			this.buttonAdd.Text = "&Add...";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			this.layoutGroupButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupButtons.GroupBordersVisible = false;
			this.layoutGroupButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemPreviewButton,
			this.emptySpaceButtons,
			this.layoutItemAddButton,
			this.layoutItemRemoveButton});
			this.layoutGroupButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupButtons.Name = "layoutGroupButtons";
			this.layoutGroupButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(8, 0, 0, 0);
			this.layoutGroupButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutGroupButtons.TextVisible = false;
			this.layoutItemPreviewButton.Control = this.buttonPreview;
			this.layoutItemPreviewButton.Location = new System.Drawing.Point(178, 0);
			this.layoutItemPreviewButton.Name = "layoutItemPreviewButton";
			this.layoutItemPreviewButton.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.layoutItemPreviewButton.Size = new System.Drawing.Size(89, 26);
			this.layoutItemPreviewButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemPreviewButton.TextVisible = false;
			this.emptySpaceButtons.AllowHotTrack = false;
			this.emptySpaceButtons.Location = new System.Drawing.Point(267, 0);
			this.emptySpaceButtons.Name = "emptySpaceButtons";
			this.emptySpaceButtons.Size = new System.Drawing.Size(149, 26);
			this.emptySpaceButtons.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemAddButton.Control = this.buttonAdd;
			this.layoutItemAddButton.Location = new System.Drawing.Point(0, 0);
			this.layoutItemAddButton.Name = "layoutItemAddButton";
			this.layoutItemAddButton.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.layoutItemAddButton.Size = new System.Drawing.Size(89, 26);
			this.layoutItemAddButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemAddButton.TextVisible = false;
			this.layoutItemRemoveButton.Control = this.buttonRemove;
			this.layoutItemRemoveButton.Location = new System.Drawing.Point(89, 0);
			this.layoutItemRemoveButton.Name = "layoutItemRemoveButton";
			this.layoutItemRemoveButton.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.layoutItemRemoveButton.Size = new System.Drawing.Size(89, 26);
			this.layoutItemRemoveButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemRemoveButton.TextVisible = false;
			this.layoutControlContent.Controls.Add(this.parametersGrid);
			this.layoutControlContent.Controls.Add(this.listBoxProcedures);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3234, 160, 450, 350);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 0;
			this.parametersGrid.Enabled = false;
			this.parametersGrid.Location = new System.Drawing.Point(186, 12);
			this.parametersGrid.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
			this.parametersGrid.Name = "parametersGrid";
			this.parametersGrid.Padding = new System.Windows.Forms.Padding(2);
			this.parametersGrid.Size = new System.Drawing.Size(408, 313);
			this.parametersGrid.TabIndex = 0;
			this.listBoxProcedures.Location = new System.Drawing.Point(12, 12);
			this.listBoxProcedures.Name = "listBoxProcedures";
			this.listBoxProcedures.Size = new System.Drawing.Size(170, 313);
			this.listBoxProcedures.StyleController = this.layoutControlContent;
			this.listBoxProcedures.TabIndex = 4;
			this.listBoxProcedures.SelectedValueChanged += new System.EventHandler(this.listBoxProcedures_SelectedValueChanged);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemParametersGrid,
			this.layoutItemProceduresList});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemParametersGrid.Control = this.parametersGrid;
			this.layoutItemParametersGrid.Location = new System.Drawing.Point(174, 0);
			this.layoutItemParametersGrid.Name = "layoutItemParametersGrid";
			this.layoutItemParametersGrid.Size = new System.Drawing.Size(412, 317);
			this.layoutItemParametersGrid.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemParametersGrid.TextVisible = false;
			this.layoutItemProceduresList.Control = this.listBoxProcedures;
			this.layoutItemProceduresList.Location = new System.Drawing.Point(0, 0);
			this.layoutItemProceduresList.Name = "layoutItemProceduresList";
			this.layoutItemProceduresList.Size = new System.Drawing.Size(174, 317);
			this.layoutItemProceduresList.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemProceduresList.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this.Name = "ConfigureEFStoredProceduresPageView";
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
			this.panelAdditionalButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlButtons)).EndInit();
			this.layoutControlButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviewButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAddButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemRemoveButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listBoxProcedures)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemParametersGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemProceduresList)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraEditors.SimpleButton buttonPreview;
		protected XtraLayout.LayoutControl layoutControlButtons;
		protected XtraLayout.LayoutControlGroup layoutGroupButtons;
		protected XtraLayout.LayoutControlItem layoutItemPreviewButton;
		protected XtraLayout.EmptySpaceItem emptySpaceButtons;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected ParametersGrid parametersGrid;
		protected XtraLayout.LayoutControlItem layoutItemParametersGrid;
		protected XtraEditors.ListBoxControl listBoxProcedures;
		protected XtraLayout.LayoutControlItem layoutItemProceduresList;
		protected XtraEditors.SimpleButton buttonRemove;
		protected XtraEditors.SimpleButton buttonAdd;
		protected XtraLayout.LayoutControlItem layoutItemAddButton;
		protected XtraLayout.LayoutControlItem layoutItemRemoveButton;
	}
}

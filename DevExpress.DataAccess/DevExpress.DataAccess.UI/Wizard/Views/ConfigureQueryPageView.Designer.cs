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
	partial class ConfigureQueryPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.panel = new DevExpress.XtraEditors.PanelControl();
			this.radioGroupQueryType = new DevExpress.XtraEditors.RadioGroup();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemQueryType = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemQuery = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlButtons = new DevExpress.XtraLayout.LayoutControl();
			this.buttonQueryBuilder = new DevExpress.XtraEditors.SimpleButton();
			this.layoutGroupButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemQueryBuilderButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceButtons = new DevExpress.XtraLayout.EmptySpaceItem();
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
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radioGroupQueryType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemQueryType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemQuery)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlButtons)).BeginInit();
			this.layoutControlButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemQueryBuilderButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(718, 117, 749, 739);
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelBaseContent.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.panelBaseContent.Padding = new System.Windows.Forms.Padding(29, 0, 29, 0);
			this.panelAdditionalButtons.Controls.Add(this.layoutControlButtons);
			this.panelAdditionalButtons.Margin = new System.Windows.Forms.Padding(0);
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.panel);
			this.layoutControlContent.Controls.Add(this.radioGroupQueryType);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(29, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(898, 239, 990, 690);
			this.layoutControlContent.Padding = new System.Windows.Forms.Padding(29, 0, 29, 0);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(548, 337);
			this.layoutControlContent.TabIndex = 0;
			this.panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panel.Location = new System.Drawing.Point(14, 79);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(522, 228);
			this.panel.TabIndex = 5;
			this.radioGroupQueryType.Location = new System.Drawing.Point(12, 14);
			this.radioGroupQueryType.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.radioGroupQueryType.Name = "radioGroupQueryType";
			this.radioGroupQueryType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.radioGroupQueryType.Properties.Appearance.Options.UseBackColor = true;
			this.radioGroupQueryType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.radioGroupQueryType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Query"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(false, "Stored Procedure")});
			this.radioGroupQueryType.Size = new System.Drawing.Size(524, 61);
			this.radioGroupQueryType.StyleController = this.layoutControlContent;
			this.radioGroupQueryType.TabIndex = 4;
			this.radioGroupQueryType.EditValueChanged += new System.EventHandler(this.radioGroupQueryType_EditValueChanged);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemQueryType,
			this.layoutItemQuery});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 12, 10);
			this.layoutGroupContent.Size = new System.Drawing.Size(548, 337);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemQueryType.Control = this.radioGroupQueryType;
			this.layoutItemQueryType.Location = new System.Drawing.Point(0, 0);
			this.layoutItemQueryType.Name = "layoutItemQueryType";
			this.layoutItemQueryType.Size = new System.Drawing.Size(528, 65);
			this.layoutItemQueryType.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemQueryType.TextVisible = false;
			this.layoutItemQuery.Control = this.panel;
			this.layoutItemQuery.Location = new System.Drawing.Point(0, 65);
			this.layoutItemQuery.Name = "layoutItemQuery";
			this.layoutItemQuery.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 2, 2, 20);
			this.layoutItemQuery.Size = new System.Drawing.Size(528, 250);
			this.layoutItemQuery.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemQuery.TextVisible = false;
			this.layoutControlButtons.AllowCustomization = false;
			this.layoutControlButtons.Controls.Add(this.buttonQueryBuilder);
			this.layoutControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutControlButtons.Margin = new System.Windows.Forms.Padding(0);
			this.layoutControlButtons.Name = "layoutControlButtons";
			this.layoutControlButtons.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2786, 241, 735, 782);
			this.layoutControlButtons.Root = this.layoutGroupButtons;
			this.layoutControlButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutControlButtons.TabIndex = 0;
			this.buttonQueryBuilder.Location = new System.Drawing.Point(12, 2);
			this.buttonQueryBuilder.Name = "buttonQueryBuilder";
			this.buttonQueryBuilder.Size = new System.Drawing.Size(128, 22);
			this.buttonQueryBuilder.StyleController = this.layoutControlButtons;
			this.buttonQueryBuilder.TabIndex = 4;
			this.buttonQueryBuilder.Text = "&Run Query Builder...";
			this.layoutGroupButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupButtons.GroupBordersVisible = false;
			this.layoutGroupButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemQueryBuilderButton,
			this.emptySpaceButtons});
			this.layoutGroupButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupButtons.Name = "Root";
			this.layoutGroupButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutGroupButtons.TextVisible = false;
			this.layoutItemQueryBuilderButton.Control = this.buttonQueryBuilder;
			this.layoutItemQueryBuilderButton.Location = new System.Drawing.Point(0, 0);
			this.layoutItemQueryBuilderButton.Name = "layoutItemQueryBuilderButton";
			this.layoutItemQueryBuilderButton.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 3, 2, 2);
			this.layoutItemQueryBuilderButton.Size = new System.Drawing.Size(143, 26);
			this.layoutItemQueryBuilderButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemQueryBuilderButton.TextVisible = false;
			this.emptySpaceButtons.AllowHotTrack = false;
			this.emptySpaceButtons.Location = new System.Drawing.Point(143, 0);
			this.emptySpaceButtons.Name = "emptySpaceButtons";
			this.emptySpaceButtons.Size = new System.Drawing.Size(281, 26);
			this.emptySpaceButtons.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(0, 1432276408, 0, 1432276408);
			this.Name = "ConfigureQueryPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radioGroupQueryType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemQueryType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemQuery)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlButtons)).EndInit();
			this.layoutControlButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemQueryBuilderButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraEditors.RadioGroup radioGroupQueryType;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemQueryType;
		protected DevExpress.XtraEditors.PanelControl panel;
		protected XtraLayout.LayoutControlItem layoutItemQuery;
		protected XtraLayout.LayoutControl layoutControlButtons;
		protected XtraEditors.SimpleButton buttonQueryBuilder;
		protected XtraLayout.LayoutControlGroup layoutGroupButtons;
		protected XtraLayout.LayoutControlItem layoutItemQueryBuilderButton;
		protected XtraLayout.EmptySpaceItem emptySpaceButtons;
	}
}

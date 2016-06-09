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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Native.ParametersGrid;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraReports.Design;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	[ToolboxItem(false)]
	public class ParametersPageViewBase : WizardViewBase {
		readonly IServiceProvider propertyGridServices;
		readonly IParameterService parameterService;
		readonly IRepositoryItemsProvider repositoryItemsProvider;
		System.ComponentModel.IContainer components = null;
		protected XtraEditors.SimpleButton buttonPreview;
		protected ParametersGrid parametersGrid;
		protected XtraEditors.SimpleButton buttonRemove;
		protected XtraEditors.SimpleButton buttonAdd;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraEditors.LabelControl separatorBottom;
		protected XtraLayout.LayoutControlItem layoutItemGrid;
		protected XtraLayout.LayoutControl layoutControlButtons;
		protected XtraLayout.LayoutControlGroup layoutGroupButtons;
		protected XtraLayout.LayoutControlItem layoutItemPreviewButton;
		protected XtraLayout.LayoutControlItem layoutItemRemoveButton;
		protected XtraLayout.LayoutControlItem layoutItemAddButton;
		protected XtraLayout.EmptySpaceItem emptySpaceButtons;
		protected XtraLayout.LayoutControlItem layoutItemSeparatorBottom;
		protected ParametersPageViewBase(IServiceProvider propertyGridServices, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) {
			Guard.ArgumentNotNull(repositoryItemsProvider, "repositoryItemsProvider");
			this.propertyGridServices = propertyGridServices;
			this.parameterService = parameterService;
			this.repositoryItemsProvider = repositoryItemsProvider;
			InitializeComponent();
			LocalizeComponent();
			labelHeader.Text = HeaderDescription;
			parametersGrid.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		ParametersPageViewBase() : this(null, null, DefaultRepositoryItemsProvider.Instance) { }
		public IEnumerable<IParameter> GetParameters() {
			return parametersGrid.Parameters;
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		void InitializeComponent() {
			this.parametersGrid = new DevExpress.DataAccess.UI.Native.ParametersGrid.ParametersGrid();
			this.buttonPreview = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlButtons = new DevExpress.XtraLayout.LayoutControl();
			this.buttonRemove = new DevExpress.XtraEditors.SimpleButton();
			this.buttonAdd = new DevExpress.XtraEditors.SimpleButton();
			this.layoutGroupButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemPreviewButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemRemoveButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemAddButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceButtons = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.separatorBottom = new DevExpress.XtraEditors.LabelControl();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemGrid = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSeparatorBottom = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.layoutItemRemoveButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAddButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorBottom)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(864, 200, 749, 738);
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelAdditionalButtons.Controls.Add(this.layoutControlButtons);
			this.parametersGrid.Location = new System.Drawing.Point(0, 0);
			this.parametersGrid.Margin = new System.Windows.Forms.Padding(3, 0, 0, 3);
			this.parametersGrid.Name = "parametersGrid";
			this.parametersGrid.Padding = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.parametersGrid.Size = new System.Drawing.Size(606, 315);
			this.parametersGrid.TabIndex = 0;
			this.buttonPreview.Location = new System.Drawing.Point(12, 2);
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.Size = new System.Drawing.Size(83, 22);
			this.buttonPreview.StyleController = this.layoutControlButtons;
			this.buttonPreview.TabIndex = 12;
			this.buttonPreview.Text = "&Preview...";
			this.layoutControlButtons.Controls.Add(this.buttonPreview);
			this.layoutControlButtons.Controls.Add(this.buttonRemove);
			this.layoutControlButtons.Controls.Add(this.buttonAdd);
			this.layoutControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutControlButtons.Name = "layoutControlButtons";
			this.layoutControlButtons.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2493, 186, 811, 482);
			this.layoutControlButtons.Root = this.layoutGroupButtons;
			this.layoutControlButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutControlButtons.TabIndex = 16;
			this.layoutControlButtons.Text = "layoutControl2";
			this.buttonRemove.Location = new System.Drawing.Point(190, 2);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(83, 22);
			this.buttonRemove.StyleController = this.layoutControlButtons;
			this.buttonRemove.TabIndex = 15;
			this.buttonRemove.Text = "Remove";
			this.buttonAdd.Location = new System.Drawing.Point(101, 2);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(83, 22);
			this.buttonAdd.StyleController = this.layoutControlButtons;
			this.buttonAdd.TabIndex = 14;
			this.buttonAdd.Text = "Add";
			this.layoutGroupButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupButtons.GroupBordersVisible = false;
			this.layoutGroupButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemPreviewButton,
			this.layoutItemRemoveButton,
			this.layoutItemAddButton,
			this.emptySpaceButtons});
			this.layoutGroupButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupButtons.Name = "Root";
			this.layoutGroupButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutGroupButtons.TextVisible = false;
			this.layoutItemPreviewButton.Control = this.buttonPreview;
			this.layoutItemPreviewButton.Location = new System.Drawing.Point(0, 0);
			this.layoutItemPreviewButton.Name = "layoutItemPreviewButton";
			this.layoutItemPreviewButton.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 3, 2, 2);
			this.layoutItemPreviewButton.Size = new System.Drawing.Size(98, 26);
			this.layoutItemPreviewButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemPreviewButton.TextVisible = false;
			this.layoutItemRemoveButton.Control = this.buttonRemove;
			this.layoutItemRemoveButton.Location = new System.Drawing.Point(187, 0);
			this.layoutItemRemoveButton.Name = "layoutItemRemoveButton";
			this.layoutItemRemoveButton.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.layoutItemRemoveButton.Size = new System.Drawing.Size(89, 26);
			this.layoutItemRemoveButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemRemoveButton.TextVisible = false;
			this.layoutItemAddButton.Control = this.buttonAdd;
			this.layoutItemAddButton.Location = new System.Drawing.Point(98, 0);
			this.layoutItemAddButton.Name = "layoutItemAddButton";
			this.layoutItemAddButton.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.layoutItemAddButton.Size = new System.Drawing.Size(89, 26);
			this.layoutItemAddButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemAddButton.TextVisible = false;
			this.emptySpaceButtons.AllowHotTrack = false;
			this.emptySpaceButtons.Location = new System.Drawing.Point(276, 0);
			this.emptySpaceButtons.Name = "emptySpaceButtons";
			this.emptySpaceButtons.Size = new System.Drawing.Size(148, 26);
			this.emptySpaceButtons.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlContent.Controls.Add(this.separatorBottom);
			this.layoutControlContent.Controls.Add(this.parametersGrid);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Margin = new System.Windows.Forms.Padding(0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2575, 160, 908, 650);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 1;
			this.layoutControlContent.Text = "layoutControl1";
			this.separatorBottom.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.separatorBottom.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.separatorBottom.LineVisible = true;
			this.separatorBottom.Location = new System.Drawing.Point(0, 315);
			this.separatorBottom.MaximumSize = new System.Drawing.Size(0, 3);
			this.separatorBottom.Name = "separatorBottom";
			this.separatorBottom.Size = new System.Drawing.Size(606, 3);
			this.separatorBottom.StyleController = this.layoutControlContent;
			this.separatorBottom.TabIndex = 4;
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemGrid,
			this.layoutItemSeparatorBottom});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "Root";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemGrid.Control = this.parametersGrid;
			this.layoutItemGrid.Location = new System.Drawing.Point(0, 0);
			this.layoutItemGrid.Name = "layoutItemGrid";
			this.layoutItemGrid.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemGrid.Size = new System.Drawing.Size(606, 315);
			this.layoutItemGrid.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemGrid.TextVisible = false;
			this.layoutItemSeparatorBottom.Control = this.separatorBottom;
			this.layoutItemSeparatorBottom.Location = new System.Drawing.Point(0, 315);
			this.layoutItemSeparatorBottom.Name = "layoutItemSeparatorBottom";
			this.layoutItemSeparatorBottom.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 9);
			this.layoutItemSeparatorBottom.Size = new System.Drawing.Size(606, 22);
			this.layoutItemSeparatorBottom.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSeparatorBottom.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ParametersPageViewBase";
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
			((System.ComponentModel.ISupportInitialize)(this.layoutItemRemoveButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAddButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorBottom)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		protected void Initialize(IEnumerable<IParameter> parameters, bool previewDataRowLimit, Func<object> getPreviewDataFunc, bool fixedParameters) {
			var parametersGridModel = new ParametersGridModel(parameters);
			var parametersGridViewModel = new ParametersGridViewModel(parametersGridModel, parameterService, propertyGridServices, getPreviewDataFunc, fixedParameters, previewDataRowLimit);		   
			parametersGrid.Initialize(parametersGridViewModel, this.propertyGridServices, parameterService, repositoryItemsProvider);
			parametersGrid.SetButtons(buttonAdd, buttonRemove, buttonPreview);
			layoutItemAddButton.ContentVisible = !fixedParameters;
			layoutItemRemoveButton.ContentVisible = !fixedParameters;
		}
		protected void Initialize(IEnumerable<IParameter> parameters) {
			Initialize(parameters, false, null, true);
			parametersGrid.ShowPreviewButton = false;
		}
		void LocalizeComponent() {
			buttonAdd.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Add);
			buttonRemove.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Remove);
			buttonPreview.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Preview);
		}
	}
}

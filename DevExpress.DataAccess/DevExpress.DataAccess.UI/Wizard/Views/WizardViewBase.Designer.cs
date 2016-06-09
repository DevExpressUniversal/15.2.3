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
	partial class WizardViewBase {
		System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.layoutControlBase = new DevExpress.XtraLayout.LayoutControl();
			this.panelAdditionalButtons = new DevExpress.XtraEditors.PanelControl();
			this.separatorTop = new DevExpress.XtraEditors.LabelControl();
			this.labelHeader = new DevExpress.XtraEditors.LabelControl();
			this.panelBaseContent = new DevExpress.XtraEditors.PanelControl();
			this.buttonPrevious = new DevExpress.DataAccess.UI.Wizard.BackButton();
			this.buttonNext = new DevExpress.XtraEditors.SimpleButton();
			this.buttonFinish = new DevExpress.XtraEditors.SimpleButton();
			this.layoutGroupBase = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemFinishButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemNextButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemPreviousButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemBaseContentPanel = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemHeaderLabel = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSeparatorTop = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemAdditionalButtons = new DevExpress.XtraLayout.LayoutControlItem();
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
			this.layoutControlBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.AllowCustomization = false;
			this.layoutControlBase.AutoSize = true;
			this.layoutControlBase.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
			this.layoutControlBase.Controls.Add(this.panelAdditionalButtons);
			this.layoutControlBase.Controls.Add(this.separatorTop);
			this.layoutControlBase.Controls.Add(this.labelHeader);
			this.layoutControlBase.Controls.Add(this.panelBaseContent);
			this.layoutControlBase.Controls.Add(this.buttonPrevious);
			this.layoutControlBase.Controls.Add(this.buttonNext);
			this.layoutControlBase.Controls.Add(this.buttonFinish);
			this.layoutControlBase.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlBase.Location = new System.Drawing.Point(0, 0);
			this.layoutControlBase.Name = "layoutControlBase";
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(864, 200, 749, 738);
			this.layoutControlBase.Root = this.layoutGroupBase;
			this.layoutControlBase.Size = new System.Drawing.Size(610, 425);
			this.layoutControlBase.TabIndex = 0;
			this.panelAdditionalButtons.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelAdditionalButtons.Location = new System.Drawing.Point(0, 389);
			this.panelAdditionalButtons.Name = "panelAdditionalButtons";
			this.panelAdditionalButtons.Size = new System.Drawing.Size(424, 26);
			this.panelAdditionalButtons.TabIndex = 13;
			this.separatorTop.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.separatorTop.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.separatorTop.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.separatorTop.LineVisible = true;
			this.separatorTop.Location = new System.Drawing.Point(0, 46);
			this.separatorTop.Margin = new System.Windows.Forms.Padding(0, 5, 0, 1);
			this.separatorTop.Name = "separatorTop";
			this.separatorTop.Size = new System.Drawing.Size(610, 6);
			this.separatorTop.StyleController = this.layoutControlBase;
			this.separatorTop.TabIndex = 12;
			this.labelHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.labelHeader.Location = new System.Drawing.Point(56, 21);
			this.labelHeader.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.labelHeader.Name = "labelHeader";
			this.labelHeader.Size = new System.Drawing.Size(552, 13);
			this.labelHeader.StyleController = this.layoutControlBase;
			this.labelHeader.TabIndex = 11;
			this.panelBaseContent.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelBaseContent.Location = new System.Drawing.Point(2, 52);
			this.panelBaseContent.Name = "panelBaseContent";
			this.panelBaseContent.Size = new System.Drawing.Size(606, 337);
			this.panelBaseContent.TabIndex = 10;
			this.buttonPrevious.Location = new System.Drawing.Point(0, 12);
			this.buttonPrevious.Margin = new System.Windows.Forms.Padding(0);
			this.buttonPrevious.Name = "buttonPrevious";
			this.buttonPrevious.Size = new System.Drawing.Size(56, 30);
			this.buttonPrevious.StyleController = this.layoutControlBase;
			this.buttonPrevious.TabIndex = 1;
			this.buttonPrevious.TabStop = false;
			this.buttonNext.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonNext.Location = new System.Drawing.Point(426, 391);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(83, 22);
			this.buttonNext.StyleController = this.layoutControlBase;
			this.buttonNext.TabIndex = 5;
			this.buttonNext.Text = "&Next ";
			this.buttonFinish.Location = new System.Drawing.Point(515, 391);
			this.buttonFinish.Name = "buttonFinish";
			this.buttonFinish.Size = new System.Drawing.Size(83, 22);
			this.buttonFinish.StyleController = this.layoutControlBase;
			this.buttonFinish.TabIndex = 4;
			this.buttonFinish.Text = "&Finish";
			this.layoutGroupBase.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupBase.GroupBordersVisible = false;
			this.layoutGroupBase.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemFinishButton,
			this.layoutItemNextButton,
			this.layoutItemPreviousButton,
			this.layoutItemBaseContentPanel,
			this.layoutItemHeaderLabel,
			this.layoutItemSeparatorTop,
			this.layoutItemAdditionalButtons});
			this.layoutGroupBase.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupBase.Name = "Root";
			this.layoutGroupBase.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 12, 10);
			this.layoutGroupBase.Size = new System.Drawing.Size(610, 425);
			this.layoutGroupBase.TextVisible = false;
			this.layoutItemFinishButton.Control = this.buttonFinish;
			this.layoutItemFinishButton.Location = new System.Drawing.Point(515, 377);
			this.layoutItemFinishButton.Name = "layoutItemFinishButton";
			this.layoutItemFinishButton.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 12, 2, 2);
			this.layoutItemFinishButton.Size = new System.Drawing.Size(95, 26);
			this.layoutItemFinishButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemFinishButton.TextVisible = false;
			this.layoutItemNextButton.Control = this.buttonNext;
			this.layoutItemNextButton.Location = new System.Drawing.Point(424, 377);
			this.layoutItemNextButton.Name = "layoutItemNextButton";
			this.layoutItemNextButton.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 6, 2, 2);
			this.layoutItemNextButton.Size = new System.Drawing.Size(91, 26);
			this.layoutItemNextButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemNextButton.TextVisible = false;
			this.layoutItemPreviousButton.Control = this.buttonPrevious;
			this.layoutItemPreviousButton.Location = new System.Drawing.Point(0, 0);
			this.layoutItemPreviousButton.Name = "layoutItemPreviousButton";
			this.layoutItemPreviousButton.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 4);
			this.layoutItemPreviousButton.Size = new System.Drawing.Size(56, 34);
			this.layoutItemPreviousButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemPreviousButton.TextVisible = false;
			this.layoutItemBaseContentPanel.Control = this.panelBaseContent;
			this.layoutItemBaseContentPanel.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutItemBaseContentPanel.Location = new System.Drawing.Point(0, 40);
			this.layoutItemBaseContentPanel.Name = "layoutItemBaseContentPanel";
			this.layoutItemBaseContentPanel.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 0);
			this.layoutItemBaseContentPanel.Size = new System.Drawing.Size(610, 337);
			this.layoutItemBaseContentPanel.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemBaseContentPanel.TextVisible = false;
			this.layoutItemBaseContentPanel.TrimClientAreaToControl = false;
			this.layoutItemHeaderLabel.Control = this.labelHeader;
			this.layoutItemHeaderLabel.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.layoutItemHeaderLabel.FillControlToClientArea = false;
			this.layoutItemHeaderLabel.Location = new System.Drawing.Point(56, 0);
			this.layoutItemHeaderLabel.MinSize = new System.Drawing.Size(16, 20);
			this.layoutItemHeaderLabel.Name = "layoutItemHeaderLabel";
			this.layoutItemHeaderLabel.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 5);
			this.layoutItemHeaderLabel.Size = new System.Drawing.Size(554, 34);
			this.layoutItemHeaderLabel.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemHeaderLabel.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemHeaderLabel.TextVisible = false;
			this.layoutItemSeparatorTop.Control = this.separatorTop;
			this.layoutItemSeparatorTop.FillControlToClientArea = false;
			this.layoutItemSeparatorTop.Location = new System.Drawing.Point(0, 34);
			this.layoutItemSeparatorTop.MinSize = new System.Drawing.Size(10, 2);
			this.layoutItemSeparatorTop.Name = "layoutItemSeparatorTop";
			this.layoutItemSeparatorTop.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemSeparatorTop.Size = new System.Drawing.Size(610, 6);
			this.layoutItemSeparatorTop.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemSeparatorTop.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSeparatorTop.TextVisible = false;
			this.layoutItemAdditionalButtons.Control = this.panelAdditionalButtons;
			this.layoutItemAdditionalButtons.Location = new System.Drawing.Point(0, 377);
			this.layoutItemAdditionalButtons.MinSize = new System.Drawing.Size(1, 1);
			this.layoutItemAdditionalButtons.Name = "layoutItemAdditionalButtons";
			this.layoutItemAdditionalButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemAdditionalButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutItemAdditionalButtons.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemAdditionalButtons.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemAdditionalButtons.TextVisible = false;
			this.barManager.Controller = this.barAndDockingController;
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.MaxItemId = 0;
			this.barAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barDockControlTop.CausesValidation = false;
			this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
			this.barDockControlTop.Size = new System.Drawing.Size(610, 0);
			this.barDockControlBottom.CausesValidation = false;
			this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.barDockControlBottom.Location = new System.Drawing.Point(0, 425);
			this.barDockControlBottom.Size = new System.Drawing.Size(610, 0);
			this.barDockControlLeft.CausesValidation = false;
			this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
			this.barDockControlLeft.Size = new System.Drawing.Size(0, 425);
			this.barDockControlRight.CausesValidation = false;
			this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.barDockControlRight.Location = new System.Drawing.Point(610, 0);
			this.barDockControlRight.Size = new System.Drawing.Size(0, 425);
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.layoutControlBase);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "WizardViewBase";
			this.Size = new System.Drawing.Size(610, 425);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
			this.layoutControlBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControlBase;
		protected XtraLayout.LayoutControlGroup layoutGroupBase;
		protected DevExpress.XtraEditors.SimpleButton buttonNext;
		protected DevExpress.XtraEditors.SimpleButton buttonFinish;
		protected XtraLayout.LayoutControlItem layoutItemFinishButton;
		protected XtraLayout.LayoutControlItem layoutItemNextButton;
		protected XtraEditors.LabelControl separatorTop;
		protected XtraEditors.LabelControl labelHeader;
		protected XtraLayout.LayoutControlItem layoutItemHeaderLabel;
		protected XtraLayout.LayoutControlItem layoutItemSeparatorTop;
		protected BackButton buttonPrevious;
		protected XtraLayout.LayoutControlItem layoutItemPreviousButton;
		protected XtraEditors.PanelControl panelBaseContent;
		protected XtraLayout.LayoutControlItem layoutItemBaseContentPanel;
		private XtraBars.BarManager barManager;
		private XtraBars.BarAndDockingController barAndDockingController;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		protected XtraLayout.LayoutControlItem layoutItemAdditionalButtons;
		protected XtraEditors.PanelControl panelAdditionalButtons;
	}
}

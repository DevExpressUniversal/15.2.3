#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class FormatRuleCustomStyleForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatRuleCustomStyleForm));
			this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.pctPreview = new DevExpress.DashboardWin.Native.PreviewPanel();
			this.lbFontSize = new DevExpress.XtraEditors.ListBoxControl();
			this.lbFontFamily = new DevExpress.XtraEditors.ListBoxControl();
			this.cbUnderline = new DevExpress.XtraEditors.CheckButton();
			this.cbItalic = new DevExpress.XtraEditors.CheckButton();
			this.cbBold = new DevExpress.XtraEditors.CheckButton();
			this.edtColor = new DevExpress.XtraEditors.ColorPickEdit();
			this.edtBackground = new DevExpress.XtraEditors.ColorPickEdit();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCreate = new DevExpress.XtraEditors.SimpleButton();
			this.lciFontSize = new DevExpress.XtraLayout.LayoutControlItem();
			this.lcgRoot = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lcgUserControl = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lcgColors = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciColor = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciBackground = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciFontFamily = new DevExpress.XtraLayout.LayoutControlItem();
			this.lcgFontStyle = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciBold = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciItalic = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciUnderline = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPreview = new DevExpress.XtraLayout.LayoutControlItem();
			this.lcgButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lcgAlignedButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciCreate = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciOK = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItemButtons = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFontSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFontFamily)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtColor.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBackground.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFontSize)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgUserControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgColors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciColor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciBackground)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFontFamily)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgFontStyle)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciBold)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciItalic)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUnderline)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgAlignedButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCreate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButtons)).BeginInit();
			this.SuspendLayout();
			this.barAndDockingController.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barManager.Controller = this.barAndDockingController;
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.MaxItemId = 0;
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.layoutControl.AllowCustomization = false;
			resources.ApplyResources(this.layoutControl, "layoutControl");
			this.layoutControl.Controls.Add(this.pctPreview);
			this.layoutControl.Controls.Add(this.lbFontSize);
			this.layoutControl.Controls.Add(this.lbFontFamily);
			this.layoutControl.Controls.Add(this.cbUnderline);
			this.layoutControl.Controls.Add(this.cbItalic);
			this.layoutControl.Controls.Add(this.cbBold);
			this.layoutControl.Controls.Add(this.edtColor);
			this.layoutControl.Controls.Add(this.edtBackground);
			this.layoutControl.Controls.Add(this.btnCancel);
			this.layoutControl.Controls.Add(this.btnOK);
			this.layoutControl.Controls.Add(this.btnCreate);
			this.layoutControl.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciFontSize});
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(943, 166, 884, 793);
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControl.Root = this.lcgRoot;
			resources.ApplyResources(this.pctPreview, "pctPreview");
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Properties.AllowFocused = false;
			this.pctPreview.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
			this.pctPreview.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
			this.pctPreview.StyleController = this.layoutControl;
			resources.ApplyResources(this.lbFontSize, "lbFontSize");
			this.lbFontSize.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbFontSize.Name = "lbFontSize";
			this.lbFontSize.StyleController = this.layoutControl;
			this.lbFontSize.TabStop = false;
			resources.ApplyResources(this.lbFontFamily, "lbFontFamily");
			this.lbFontFamily.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbFontFamily.Name = "lbFontFamily";
			this.lbFontFamily.StyleController = this.layoutControl;
			this.cbUnderline.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("cbUnderline.Appearance.Font")));
			this.cbUnderline.Appearance.Options.UseFont = true;
			resources.ApplyResources(this.cbUnderline, "cbUnderline");
			this.cbUnderline.Name = "cbUnderline";
			this.cbUnderline.StyleController = this.layoutControl;
			this.cbItalic.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("cbItalic.Appearance.Font")));
			this.cbItalic.Appearance.Options.UseFont = true;
			resources.ApplyResources(this.cbItalic, "cbItalic");
			this.cbItalic.Name = "cbItalic";
			this.cbItalic.StyleController = this.layoutControl;
			this.cbBold.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("cbBold.Appearance.Font")));
			this.cbBold.Appearance.Options.UseFont = true;
			resources.ApplyResources(this.cbBold, "cbBold");
			this.cbBold.Name = "cbBold";
			this.cbBold.StyleController = this.layoutControl;
			resources.ApplyResources(this.edtColor, "edtColor");
			this.edtColor.Name = "edtColor";
			this.edtColor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtColor.Properties.Buttons"))))});
			this.edtColor.Properties.ColorAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.edtColor.Properties.ShowSystemColors = false;
			this.edtColor.Properties.ShowWebColors = false;
			this.edtColor.StyleController = this.layoutControl;
			resources.ApplyResources(this.edtBackground, "edtBackground");
			this.edtBackground.Name = "edtBackground";
			this.edtBackground.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtBackground.Properties.Buttons"))))});
			this.edtBackground.Properties.ColorAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.edtBackground.Properties.ShowSystemColors = false;
			this.edtBackground.Properties.ShowWebColors = false;
			this.edtBackground.StyleController = this.layoutControl;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl;
			this.btnCancel.Click += new System.EventHandler(this.OnCancelClick);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl;
			this.btnOK.Click += new System.EventHandler(this.OnApplyClick);
			resources.ApplyResources(this.btnCreate, "btnCreate");
			this.btnCreate.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnCreate.Name = "btnCreate";
			this.btnCreate.StyleController = this.layoutControl;
			this.btnCreate.Click += new System.EventHandler(this.OnApplyClick);
			this.lciFontSize.Control = this.lbFontSize;
			this.lciFontSize.Location = new System.Drawing.Point(327, 64);
			this.lciFontSize.Name = "lciFontSize";
			this.lciFontSize.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
			this.lciFontSize.Size = new System.Drawing.Size(99, 202);
			this.lciFontSize.TextSize = new System.Drawing.Size(0, 0);
			this.lciFontSize.TextVisible = false;
			this.lcgRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgRoot.GroupBordersVisible = false;
			this.lcgRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lcgUserControl,
			this.lcgButtons});
			this.lcgRoot.Location = new System.Drawing.Point(0, 0);
			this.lcgRoot.Name = "Root";
			this.lcgRoot.Size = new System.Drawing.Size(442, 400);
			this.lcgRoot.TextVisible = false;
			this.lcgUserControl.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgUserControl.GroupBordersVisible = false;
			this.lcgUserControl.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lcgColors,
			this.lciFontFamily,
			this.lcgFontStyle,
			this.lciPreview});
			this.lcgUserControl.Location = new System.Drawing.Point(0, 0);
			this.lcgUserControl.Name = "lcgUserControl";
			this.lcgUserControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lcgUserControl.Size = new System.Drawing.Size(422, 334);
			this.lcgUserControl.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lcgUserControl.TextVisible = false;
			this.lcgColors.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgColors.GroupBordersVisible = false;
			this.lcgColors.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciColor,
			this.lciBackground});
			this.lcgColors.Location = new System.Drawing.Point(0, 0);
			this.lcgColors.Name = "lcgColors";
			this.lcgColors.Size = new System.Drawing.Size(324, 64);
			this.lcgColors.TextVisible = false;
			this.lciColor.Control = this.edtColor;
			this.lciColor.Location = new System.Drawing.Point(150, 0);
			this.lciColor.Name = "lciColor";
			this.lciColor.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 2, 2);
			this.lciColor.Size = new System.Drawing.Size(150, 40);
			resources.ApplyResources(this.lciColor, "lciColor");
			this.lciColor.TextLocation = DevExpress.Utils.Locations.Top;
			this.lciColor.TextSize = new System.Drawing.Size(82, 13);
			this.lciBackground.Control = this.edtBackground;
			this.lciBackground.Location = new System.Drawing.Point(0, 0);
			this.lciBackground.Name = "lciBackground";
			this.lciBackground.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 10, 2, 2);
			this.lciBackground.Size = new System.Drawing.Size(150, 40);
			resources.ApplyResources(this.lciBackground, "lciBackground");
			this.lciBackground.TextLocation = DevExpress.Utils.Locations.Top;
			this.lciBackground.TextSize = new System.Drawing.Size(82, 13);
			this.lciFontFamily.Control = this.lbFontFamily;
			this.lciFontFamily.Location = new System.Drawing.Point(0, 64);
			this.lciFontFamily.Name = "lciFontFamily";
			this.lciFontFamily.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
			this.lciFontFamily.Size = new System.Drawing.Size(422, 200);
			this.lciFontFamily.TextSize = new System.Drawing.Size(0, 0);
			this.lciFontFamily.TextVisible = false;
			this.lcgFontStyle.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgFontStyle.GroupBordersVisible = false;
			this.lcgFontStyle.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciBold,
			this.lciItalic,
			this.lciUnderline});
			this.lcgFontStyle.Location = new System.Drawing.Point(324, 0);
			this.lcgFontStyle.Name = "lcgFontStyle";
			this.lcgFontStyle.Size = new System.Drawing.Size(98, 64);
			this.lcgFontStyle.TextVisible = false;
			this.lciBold.Control = this.cbBold;
			this.lciBold.ControlAlignment = System.Drawing.ContentAlignment.BottomCenter;
			this.lciBold.Location = new System.Drawing.Point(0, 0);
			this.lciBold.Name = "lciBold";
			this.lciBold.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 0);
			this.lciBold.Size = new System.Drawing.Size(24, 40);
			this.lciBold.TextSize = new System.Drawing.Size(0, 0);
			this.lciBold.TextVisible = false;
			this.lciBold.TrimClientAreaToControl = false;
			this.lciItalic.Control = this.cbItalic;
			this.lciItalic.ControlAlignment = System.Drawing.ContentAlignment.BottomCenter;
			this.lciItalic.Location = new System.Drawing.Point(24, 0);
			this.lciItalic.Name = "lciItalic";
			this.lciItalic.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 0);
			this.lciItalic.Size = new System.Drawing.Size(26, 40);
			this.lciItalic.TextSize = new System.Drawing.Size(0, 0);
			this.lciItalic.TextVisible = false;
			this.lciItalic.TrimClientAreaToControl = false;
			this.lciUnderline.Control = this.cbUnderline;
			this.lciUnderline.ControlAlignment = System.Drawing.ContentAlignment.BottomCenter;
			this.lciUnderline.Location = new System.Drawing.Point(50, 0);
			this.lciUnderline.Name = "lciUnderline";
			this.lciUnderline.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 0);
			this.lciUnderline.Size = new System.Drawing.Size(24, 40);
			this.lciUnderline.TextSize = new System.Drawing.Size(0, 0);
			this.lciUnderline.TextVisible = false;
			this.lciUnderline.TrimClientAreaToControl = false;
			this.lciPreview.Control = this.pctPreview;
			this.lciPreview.Location = new System.Drawing.Point(0, 264);
			this.lciPreview.Name = "lciPreview";
			this.lciPreview.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
			this.lciPreview.Size = new System.Drawing.Size(422, 70);
			this.lciPreview.TextSize = new System.Drawing.Size(0, 0);
			this.lciPreview.TextVisible = false;
			this.lcgButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgButtons.GroupBordersVisible = false;
			this.lcgButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lcgAlignedButtons,
			this.emptySpaceItemButtons});
			this.lcgButtons.Location = new System.Drawing.Point(0, 334);
			this.lcgButtons.Name = "lcgButtons";
			this.lcgButtons.Size = new System.Drawing.Size(422, 46);
			this.lcgButtons.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lcgAlignedButtons.GroupBordersVisible = false;
			this.lcgAlignedButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciCreate,
			this.lciCancel,
			this.lciOK});
			this.lcgAlignedButtons.Location = new System.Drawing.Point(180, 0);
			this.lcgAlignedButtons.Name = "lcgAlignedButtons";
			this.lcgAlignedButtons.Size = new System.Drawing.Size(222, 26);
			this.lcgAlignedButtons.TextVisible = false;
			this.lciCreate.Control = this.btnCreate;
			this.lciCreate.Location = new System.Drawing.Point(0, 0);
			this.lciCreate.Name = "lciCreate";
			this.lciCreate.Size = new System.Drawing.Size(74, 26);
			this.lciCreate.TextSize = new System.Drawing.Size(0, 0);
			this.lciCreate.TextVisible = false;
			this.lciCancel.Control = this.btnCancel;
			this.lciCancel.Location = new System.Drawing.Point(148, 0);
			this.lciCancel.Name = "lciCancel";
			this.lciCancel.Size = new System.Drawing.Size(74, 26);
			this.lciCancel.TextSize = new System.Drawing.Size(0, 0);
			this.lciCancel.TextVisible = false;
			this.lciOK.Control = this.btnOK;
			this.lciOK.Location = new System.Drawing.Point(74, 0);
			this.lciOK.Name = "lciOK";
			this.lciOK.Size = new System.Drawing.Size(74, 26);
			this.lciOK.TextSize = new System.Drawing.Size(0, 0);
			this.lciOK.TextVisible = false;
			this.emptySpaceItemButtons.AllowHotTrack = false;
			this.emptySpaceItemButtons.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItemButtons.Name = "emptySpaceItemButtons";
			this.emptySpaceItemButtons.Size = new System.Drawing.Size(180, 26);
			this.emptySpaceItemButtons.TextSize = new System.Drawing.Size(0, 0);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.layoutControl);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormatRuleCustomStyleForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pctPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFontSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbFontFamily)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtColor.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtBackground.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFontSize)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgUserControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgColors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciColor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciBackground)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciFontFamily)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgFontStyle)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciBold)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciItalic)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciUnderline)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgAlignedButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCreate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButtons)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnCreate;
		private XtraEditors.SimpleButton btnCancel;
		private XtraLayout.LayoutControl layoutControl;
		private XtraLayout.LayoutControlGroup lcgRoot;
		private XtraLayout.LayoutControlItem lciCreate;
		private XtraLayout.LayoutControlItem lciCancel;
		private XtraLayout.LayoutControlGroup lcgAlignedButtons;
		private XtraLayout.EmptySpaceItem emptySpaceItemButtons;
		private XtraLayout.LayoutControlGroup lcgUserControl;
		private XtraLayout.LayoutControlGroup lcgButtons;
		private XtraBars.BarManager barManager;
		private XtraBars.BarAndDockingController barAndDockingController;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
		private XtraEditors.ColorPickEdit edtBackground;
		private XtraLayout.LayoutControlItem lciBackground;
		private XtraEditors.ColorPickEdit edtColor;
		private XtraLayout.LayoutControlItem lciColor;
		private XtraEditors.CheckButton cbBold;
		private XtraLayout.LayoutControlItem lciBold;
		private XtraEditors.CheckButton cbItalic;
		private XtraLayout.LayoutControlItem lciItalic;
		private XtraEditors.CheckButton cbUnderline;
		private XtraLayout.LayoutControlItem lciUnderline;
		private XtraLayout.LayoutControlGroup lcgFontStyle;
		private XtraLayout.LayoutControlGroup lcgColors;
		private XtraEditors.ListBoxControl lbFontFamily;
		private XtraLayout.LayoutControlItem lciFontFamily;
		private XtraEditors.ListBoxControl lbFontSize;
		private XtraLayout.LayoutControlItem lciFontSize;
		private PreviewPanel pctPreview;
		private XtraLayout.LayoutControlItem lciPreview;
		private XtraEditors.SimpleButton btnOK;
		private XtraLayout.LayoutControlItem lciOK;
	}
}

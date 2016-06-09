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
	partial class FormatRuleFormBase {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormatRuleFormBase));
			this.barAndDockingController = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.lcgRoot = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lcgUserControl = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lcgButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lcgAlignedButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciOK = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciApply = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciClose = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItemButtons = new DevExpress.XtraLayout.EmptySpaceItem();
			this.separatorButtons = new DevExpress.XtraLayout.SimpleSeparator();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgUserControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgAlignedButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciApply)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciClose)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.separatorButtons)).BeginInit();
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
			this.layoutControl.Controls.Add(this.btnClose);
			this.layoutControl.Controls.Add(this.btnApply);
			this.layoutControl.Controls.Add(this.btnCancel);
			this.layoutControl.Controls.Add(this.btnOK);
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(792, 142, 884, 793);
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControl.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControl.Root = this.lcgRoot;
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Name = "btnClose";
			this.btnClose.StyleController = this.layoutControl;
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.StyleController = this.layoutControl;
			this.btnApply.Click += new System.EventHandler(this.OnBtnApplyClick);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl;
			this.btnCancel.Click += new System.EventHandler(this.OnBtnCancelClick);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl;
			this.btnOK.Click += new System.EventHandler(this.OnBtnOKClick);
			this.lcgRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgRoot.GroupBordersVisible = false;
			this.lcgRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lcgUserControl,
			this.lcgButtons,
			this.separatorButtons});
			this.lcgRoot.Location = new System.Drawing.Point(0, 0);
			this.lcgRoot.Name = "Root";
			this.lcgRoot.Padding = new DevExpress.XtraLayout.Utils.Padding(6, 6, 10, 6);
			this.lcgRoot.Size = new System.Drawing.Size(290, 300);
			this.lcgRoot.TextVisible = false;
			this.lcgUserControl.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgUserControl.GroupBordersVisible = false;
			this.lcgUserControl.Location = new System.Drawing.Point(0, 0);
			this.lcgUserControl.Name = "lcgUserControl";
			this.lcgUserControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lcgUserControl.Size = new System.Drawing.Size(278, 231);
			this.lcgUserControl.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.lcgUserControl.TextVisible = false;
			this.lcgButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgButtons.GroupBordersVisible = false;
			this.lcgButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lcgAlignedButtons,
			this.emptySpaceItemButtons});
			this.lcgButtons.Location = new System.Drawing.Point(0, 233);
			this.lcgButtons.Name = "lcgButtons";
			this.lcgButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 6, 0);
			this.lcgButtons.Size = new System.Drawing.Size(278, 51);
			this.lcgButtons.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 11, 0);
			this.lcgAlignedButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgAlignedButtons.GroupBordersVisible = false;
			this.lcgAlignedButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciOK,
			this.lciCancel,
			this.lciApply,
			this.lciClose});
			this.lcgAlignedButtons.Location = new System.Drawing.Point(10, 0);
			this.lcgAlignedButtons.Name = "lcgAlignedButtons";
			this.lcgAlignedButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.lcgAlignedButtons.Size = new System.Drawing.Size(268, 34);
			this.lcgAlignedButtons.TextVisible = false;
			this.lciOK.Control = this.btnOK;
			this.lciOK.Location = new System.Drawing.Point(0, 0);
			this.lciOK.Name = "lciOK";
			this.lciOK.Size = new System.Drawing.Size(62, 26);
			this.lciOK.TextSize = new System.Drawing.Size(0, 0);
			this.lciOK.TextVisible = false;
			this.lciCancel.Control = this.btnCancel;
			this.lciCancel.Location = new System.Drawing.Point(62, 0);
			this.lciCancel.Name = "lciCancel";
			this.lciCancel.Size = new System.Drawing.Size(62, 26);
			this.lciCancel.TextSize = new System.Drawing.Size(0, 0);
			this.lciCancel.TextVisible = false;
			this.lciApply.Control = this.btnApply;
			this.lciApply.Location = new System.Drawing.Point(124, 0);
			this.lciApply.Name = "lciApply";
			this.lciApply.Size = new System.Drawing.Size(62, 26);
			this.lciApply.TextSize = new System.Drawing.Size(0, 0);
			this.lciApply.TextVisible = false;
			this.lciClose.Control = this.btnClose;
			this.lciClose.Location = new System.Drawing.Point(186, 0);
			this.lciClose.Name = "lciClose";
			this.lciClose.Size = new System.Drawing.Size(74, 26);
			this.lciClose.TextSize = new System.Drawing.Size(0, 0);
			this.lciClose.TextVisible = false;
			this.lciClose.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.OnlyInCustomization;
			this.emptySpaceItemButtons.AllowHotTrack = false;
			this.emptySpaceItemButtons.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItemButtons.Name = "emptySpaceItemButtons";
			this.emptySpaceItemButtons.Size = new System.Drawing.Size(10, 34);
			this.emptySpaceItemButtons.TextSize = new System.Drawing.Size(0, 0);
			this.separatorButtons.AllowHotTrack = false;
			this.separatorButtons.Location = new System.Drawing.Point(0, 231);
			this.separatorButtons.Name = "separatorButtons";
			this.separatorButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
			this.separatorButtons.Size = new System.Drawing.Size(278, 2);
			this.separatorButtons.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			this.AcceptButton = this.btnOK;
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
			this.Name = "FormatRuleFormBase";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lcgRoot)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgUserControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgAlignedButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciApply)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciClose)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItemButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.separatorButtons)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnApply;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
		private XtraLayout.LayoutControl layoutControl;
		private XtraLayout.LayoutControlGroup lcgRoot;
		private XtraLayout.SimpleSeparator separatorButtons;
		private XtraLayout.LayoutControlItem lciOK;
		private XtraLayout.LayoutControlItem lciCancel;
		private XtraLayout.LayoutControlItem lciApply;
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
		private XtraEditors.SimpleButton btnClose;
		private XtraLayout.LayoutControlItem lciClose;
	}
}

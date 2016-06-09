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

namespace DevExpress.XtraLayout.Customization.Templates {
	partial class TemplateMangerAskNameForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateMangerAskNameForm));
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.errorPictureEdit = new DevExpress.XtraEditors.PictureEdit();
			this.detailsSimpleButton = new DevExpress.XtraEditors.SimpleButton();
			this.detailsMemoEdit = new DevExpress.XtraEditors.MemoEdit();
			this.errorLabelControl = new DevExpress.XtraEditors.LabelControl();
			this.templateNameTextEdit = new DevExpress.XtraEditors.TextEdit();
			this.rootLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.askNameGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.templateNameTextEditLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.okButtonLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.cancelButtonLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.errorMessageGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.errorMessageLCI = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceBetweenButtons = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciDetailsMemoEdit = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciErrorPictureEdit = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciDetailsSimpleButton = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorPictureEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.detailsMemoEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.templateNameTextEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rootLayoutControlGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.askNameGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.templateNameTextEditLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.okButtonLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cancelButtonLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorMessageGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorMessageLCI)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceBetweenButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDetailsMemoEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciErrorPictureEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDetailsSimpleButton)).BeginInit();
			this.SuspendLayout();
			this.okButton.Location = new System.Drawing.Point(330, 99);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(65, 22);
			this.okButton.StyleController = this.layoutControl;
			this.okButton.TabIndex = 5;
			this.okButton.Text = "Save";
			this.okButton.Click += new System.EventHandler(this.simpleButton1_Click);
			this.layoutControl.AllowCustomization = false;
			this.layoutControl.Controls.Add(this.simpleButton1);
			this.layoutControl.Controls.Add(this.errorPictureEdit);
			this.layoutControl.Controls.Add(this.detailsSimpleButton);
			this.layoutControl.Controls.Add(this.detailsMemoEdit);
			this.layoutControl.Controls.Add(this.errorLabelControl);
			this.layoutControl.Controls.Add(this.okButton);
			this.layoutControl.Controls.Add(this.templateNameTextEdit);
			this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl.Location = new System.Drawing.Point(0, 0);
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(777, 79, 687, 690);
			this.layoutControl.Root = this.rootLayoutControlGroup;
			this.layoutControl.Size = new System.Drawing.Size(480, 435);
			this.layoutControl.TabIndex = 0;
			this.layoutControl.Text = "layoutControl1";
			this.simpleButton1.Location = new System.Drawing.Point(401, 99);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(65, 22);
			this.simpleButton1.StyleController = this.layoutControl;
			this.simpleButton1.TabIndex = 12;
			this.simpleButton1.Text = "Cancel";
			this.simpleButton1.Click += new System.EventHandler(this.cancelButton_Click);
			this.errorPictureEdit.EditValue = ((object)(resources.GetObject("errorPictureEdit.EditValue")));
			this.errorPictureEdit.Location = new System.Drawing.Point(14, 134);
			this.errorPictureEdit.Name = "errorPictureEdit";
			this.errorPictureEdit.Properties.AllowFocused = false;
			this.errorPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.errorPictureEdit.Properties.Appearance.Options.UseBackColor = true;
			this.errorPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.errorPictureEdit.Properties.ReadOnly = true;
			this.errorPictureEdit.Size = new System.Drawing.Size(57, 35);
			this.errorPictureEdit.StyleController = this.layoutControl;
			this.errorPictureEdit.TabIndex = 11;
			this.detailsSimpleButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
			this.detailsSimpleButton.Location = new System.Drawing.Point(382, 236);
			this.detailsSimpleButton.Name = "detailsSimpleButton";
			this.detailsSimpleButton.Size = new System.Drawing.Size(86, 22);
			this.detailsSimpleButton.StyleController = this.layoutControl;
			this.detailsSimpleButton.TabIndex = 10;
			this.detailsSimpleButton.Text = "Details";
			this.detailsSimpleButton.Click += new System.EventHandler(this.simpleButton1_Click_1);
			this.detailsMemoEdit.EditValue = "";
			this.detailsMemoEdit.Location = new System.Drawing.Point(14, 262);
			this.detailsMemoEdit.Name = "detailsMemoEdit";
			this.detailsMemoEdit.Properties.AcceptsTab = true;
			this.detailsMemoEdit.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
			this.detailsMemoEdit.Properties.Appearance.Options.UseBackColor = true;
			this.detailsMemoEdit.Properties.ReadOnly = true;
			this.detailsMemoEdit.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.detailsMemoEdit.Size = new System.Drawing.Size(426, 159);
			this.detailsMemoEdit.StyleController = this.layoutControl;
			this.detailsMemoEdit.TabIndex = 9;
			this.errorLabelControl.AllowHtmlString = true;
			this.errorLabelControl.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.errorLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.errorLabelControl.Location = new System.Drawing.Point(78, 124);
			this.errorLabelControl.Name = "errorLabelControl";
			this.errorLabelControl.Size = new System.Drawing.Size(365, 108);
			this.errorLabelControl.StyleController = this.layoutControl;
			this.errorLabelControl.TabIndex = 8;
			this.errorLabelControl.Text = "Error Message";
			this.templateNameTextEdit.EditValue = "";
			this.templateNameTextEdit.Location = new System.Drawing.Point(52, 47);
			this.templateNameTextEdit.Name = "templateNameTextEdit";
			this.templateNameTextEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
			this.templateNameTextEdit.Properties.NullValuePrompt = "Enter template name";
			this.templateNameTextEdit.Properties.NullValuePromptShowForEmptyValue = true;
			this.templateNameTextEdit.Size = new System.Drawing.Size(234, 20);
			this.templateNameTextEdit.StyleController = this.layoutControl;
			this.templateNameTextEdit.TabIndex = 4;
			this.rootLayoutControlGroup.CustomizationFormText = "Root";
			this.rootLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.rootLayoutControlGroup.GroupBordersVisible = false;
			this.rootLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.askNameGroup,
			this.errorMessageGroup});
			this.rootLayoutControlGroup.Location = new System.Drawing.Point(0, 0);
			this.rootLayoutControlGroup.Name = "Root";
			this.rootLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 12, 12, 12);
			this.rootLayoutControlGroup.Size = new System.Drawing.Size(480, 435);
			this.rootLayoutControlGroup.Text = "Root";
			this.rootLayoutControlGroup.TextVisible = false;
			this.askNameGroup.CustomizationFormText = "AskNameGroup";
			this.askNameGroup.GroupBordersVisible = false;
			this.askNameGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.templateNameTextEditLCI,
			this.emptySpaceItem1,
			this.okButtonLCI,
			this.cancelButtonLCI});
			this.askNameGroup.Location = new System.Drawing.Point(0, 0);
			this.askNameGroup.Name = "AskNameGroup";
			this.askNameGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.askNameGroup.Size = new System.Drawing.Size(456, 110);
			this.askNameGroup.Text = "AskNameGroup";
			this.askNameGroup.TextVisible = false;
			this.templateNameTextEditLCI.Control = this.templateNameTextEdit;
			this.templateNameTextEditLCI.CustomizationFormText = "Name:";
			this.templateNameTextEditLCI.ImageToTextDistance = 4;
			this.templateNameTextEditLCI.Location = new System.Drawing.Point(0, 0);
			this.templateNameTextEditLCI.MaxSize = new System.Drawing.Size(314, 86);
			this.templateNameTextEditLCI.MinSize = new System.Drawing.Size(314, 86);
			this.templateNameTextEditLCI.Name = "templateNameTextEditLCI";
			this.templateNameTextEditLCI.Padding = new DevExpress.XtraLayout.Utils.Padding(40, 40, 20, 30);
			this.templateNameTextEditLCI.Size = new System.Drawing.Size(456, 86);
			this.templateNameTextEditLCI.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.templateNameTextEditLCI.Text = "Name:";
			this.templateNameTextEditLCI.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.templateNameTextEditLCI.TextLocation = DevExpress.Utils.Locations.Top;
			this.templateNameTextEditLCI.TextSize = new System.Drawing.Size(31, 13);
			this.templateNameTextEditLCI.TextToControlDistance = 2;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 86);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(108, 24);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(316, 24);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.okButtonLCI.Control = this.okButton;
			this.okButtonLCI.CustomizationFormText = "okButtonLCI";
			this.okButtonLCI.Location = new System.Drawing.Point(316, 86);
			this.okButtonLCI.MaxSize = new System.Drawing.Size(70, 23);
			this.okButtonLCI.MinSize = new System.Drawing.Size(70, 23);
			this.okButtonLCI.Name = "okButtonLCI";
			this.okButtonLCI.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 3, 1, 0);
			this.okButtonLCI.Size = new System.Drawing.Size(70, 24);
			this.okButtonLCI.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.okButtonLCI.Text = "okButtonLCI";
			this.okButtonLCI.TextSize = new System.Drawing.Size(0, 0);
			this.okButtonLCI.TextVisible = false;
			this.cancelButtonLCI.Control = this.simpleButton1;
			this.cancelButtonLCI.CustomizationFormText = "layoutControlItem1";
			this.cancelButtonLCI.Location = new System.Drawing.Point(386, 86);
			this.cancelButtonLCI.MaxSize = new System.Drawing.Size(70, 23);
			this.cancelButtonLCI.MinSize = new System.Drawing.Size(70, 23);
			this.cancelButtonLCI.Name = "cancelButtonLCI";
			this.cancelButtonLCI.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 2, 1, 0);
			this.cancelButtonLCI.Size = new System.Drawing.Size(70, 24);
			this.cancelButtonLCI.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.cancelButtonLCI.Text = "cancelButtonLCI";
			this.cancelButtonLCI.TextSize = new System.Drawing.Size(0, 0);
			this.cancelButtonLCI.TextVisible = false;
			this.errorMessageGroup.CustomizationFormText = "errorMessageGroup";
			this.errorMessageGroup.GroupBordersVisible = false;
			this.errorMessageGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.errorMessageLCI,
			this.emptySpaceBetweenButtons,
			this.lciDetailsMemoEdit,
			this.lciErrorPictureEdit,
			this.emptySpaceItem4,
			this.emptySpaceItem5,
			this.lciDetailsSimpleButton});
			this.errorMessageGroup.Location = new System.Drawing.Point(0, 110);
			this.errorMessageGroup.Name = "errorMessageGroup";
			this.errorMessageGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.errorMessageGroup.Size = new System.Drawing.Size(456, 301);
			this.errorMessageGroup.Text = "errorMessageGroup";
			this.errorMessageGroup.TextVisible = false;
			this.errorMessageGroup.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			this.errorMessageLCI.Control = this.errorLabelControl;
			this.errorMessageLCI.CustomizationFormText = "errorMessageLCI";
			this.errorMessageLCI.Location = new System.Drawing.Point(64, 0);
			this.errorMessageLCI.MaxSize = new System.Drawing.Size(392, 112);
			this.errorMessageLCI.MinSize = new System.Drawing.Size(392, 112);
			this.errorMessageLCI.Name = "errorMessageLCI";
			this.errorMessageLCI.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 25, 2, 2);
			this.errorMessageLCI.Size = new System.Drawing.Size(392, 112);
			this.errorMessageLCI.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.errorMessageLCI.Text = "errorMessageLCI";
			this.errorMessageLCI.TextSize = new System.Drawing.Size(0, 0);
			this.errorMessageLCI.TextVisible = false;
			this.emptySpaceBetweenButtons.AllowHotTrack = false;
			this.emptySpaceBetweenButtons.CustomizationFormText = "emptySpaceBetweenButtons";
			this.emptySpaceBetweenButtons.Location = new System.Drawing.Point(0, 112);
			this.emptySpaceBetweenButtons.MinSize = new System.Drawing.Size(104, 24);
			this.emptySpaceBetweenButtons.Name = "emptySpaceBetweenButtons";
			this.emptySpaceBetweenButtons.Size = new System.Drawing.Size(365, 26);
			this.emptySpaceBetweenButtons.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceBetweenButtons.Text = "emptySpaceBetweenButtons";
			this.emptySpaceBetweenButtons.TextSize = new System.Drawing.Size(0, 0);
			this.lciDetailsMemoEdit.Control = this.detailsMemoEdit;
			this.lciDetailsMemoEdit.CustomizationFormText = "lciDetailsMemoEdit";
			this.lciDetailsMemoEdit.Location = new System.Drawing.Point(0, 138);
			this.lciDetailsMemoEdit.MaxSize = new System.Drawing.Size(430, 163);
			this.lciDetailsMemoEdit.MinSize = new System.Drawing.Size(430, 163);
			this.lciDetailsMemoEdit.Name = "lciDetailsMemoEdit";
			this.lciDetailsMemoEdit.Size = new System.Drawing.Size(456, 163);
			this.lciDetailsMemoEdit.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciDetailsMemoEdit.Text = "lciDetailsMemoEdit";
			this.lciDetailsMemoEdit.TextSize = new System.Drawing.Size(0, 0);
			this.lciDetailsMemoEdit.TextVisible = false;
			this.lciDetailsMemoEdit.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			this.lciErrorPictureEdit.Control = this.errorPictureEdit;
			this.lciErrorPictureEdit.CustomizationFormText = "lciErrorPictureEdit";
			this.lciErrorPictureEdit.Location = new System.Drawing.Point(0, 10);
			this.lciErrorPictureEdit.MaxSize = new System.Drawing.Size(61, 39);
			this.lciErrorPictureEdit.MinSize = new System.Drawing.Size(61, 39);
			this.lciErrorPictureEdit.Name = "lciErrorPictureEdit";
			this.lciErrorPictureEdit.Size = new System.Drawing.Size(64, 39);
			this.lciErrorPictureEdit.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciErrorPictureEdit.Text = "lciErrorPictureEdit";
			this.lciErrorPictureEdit.TextSize = new System.Drawing.Size(0, 0);
			this.lciErrorPictureEdit.TextVisible = false;
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.CustomizationFormText = "emptySpaceItem4";
			this.emptySpaceItem4.Location = new System.Drawing.Point(0, 49);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Size = new System.Drawing.Size(64, 63);
			this.emptySpaceItem4.Text = "emptySpaceItem4";
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem5.AllowHotTrack = false;
			this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem5";
			this.emptySpaceItem5.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem5.MaxSize = new System.Drawing.Size(64, 10);
			this.emptySpaceItem5.MinSize = new System.Drawing.Size(64, 10);
			this.emptySpaceItem5.Name = "emptySpaceItem5";
			this.emptySpaceItem5.Size = new System.Drawing.Size(64, 10);
			this.emptySpaceItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem5.Text = "emptySpaceItem5";
			this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
			this.lciDetailsSimpleButton.Control = this.detailsSimpleButton;
			this.lciDetailsSimpleButton.CustomizationFormText = "lciDetailsSimpleButton";
			this.lciDetailsSimpleButton.Location = new System.Drawing.Point(365, 112);
			this.lciDetailsSimpleButton.MaxSize = new System.Drawing.Size(91, 26);
			this.lciDetailsSimpleButton.MinSize = new System.Drawing.Size(91, 26);
			this.lciDetailsSimpleButton.Name = "lciDetailsSimpleButton";
			this.lciDetailsSimpleButton.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 0, 2, 2);
			this.lciDetailsSimpleButton.Size = new System.Drawing.Size(91, 26);
			this.lciDetailsSimpleButton.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.lciDetailsSimpleButton.Text = "lciDetailsSimpleButton";
			this.lciDetailsSimpleButton.TextSize = new System.Drawing.Size(0, 0);
			this.lciDetailsSimpleButton.TextVisible = false;
			this.AcceptButton = this.okButton;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(480, 435);
			this.Controls.Add(this.layoutControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MinimizeBox = false;
			this.Name = "TemplateMangerAskNameForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Enter Template Name";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorPictureEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.detailsMemoEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.templateNameTextEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rootLayoutControlGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.askNameGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.templateNameTextEditLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.okButtonLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cancelButtonLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorMessageGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorMessageLCI)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceBetweenButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDetailsMemoEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciErrorPictureEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciDetailsSimpleButton)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private LayoutControl layoutControl;
		private XtraEditors.SimpleButton okButton;
		private XtraEditors.TextEdit templateNameTextEdit;
		private LayoutControlGroup rootLayoutControlGroup;
		private LayoutControlItem templateNameTextEditLCI;
		private LayoutControlItem okButtonLCI;
		private LayoutControlGroup askNameGroup;
		private EmptySpaceItem emptySpaceItem1;
		private LayoutControlGroup errorMessageGroup;
		private XtraEditors.LabelControl errorLabelControl;
		private LayoutControlItem errorMessageLCI;
		private XtraEditors.MemoEdit detailsMemoEdit;
		private EmptySpaceItem emptySpaceBetweenButtons;
		private XtraEditors.SimpleButton detailsSimpleButton;
		private LayoutControlItem lciDetailsMemoEdit;
		private LayoutControlItem lciDetailsSimpleButton;
		private XtraEditors.PictureEdit errorPictureEdit;
		private LayoutControlItem lciErrorPictureEdit;
		private EmptySpaceItem emptySpaceItem4;
		private EmptySpaceItem emptySpaceItem5;
		private XtraEditors.SimpleButton simpleButton1;
		private LayoutControlItem cancelButtonLCI;
	}
}

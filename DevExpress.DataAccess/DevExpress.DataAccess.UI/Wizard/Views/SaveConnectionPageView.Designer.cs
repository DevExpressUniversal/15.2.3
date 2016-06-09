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
	partial class SaveConnectionPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.textConnectionName = new DevExpress.XtraEditors.TextEdit();
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.radioGroupSaveCredentials = new DevExpress.XtraEditors.RadioGroup();
			this.labelSaveCredentials = new DevExpress.XtraEditors.LabelControl();
			this.labelSaveConnection = new DevExpress.XtraEditors.LabelControl();
			this.checkSaveConnection = new DevExpress.XtraEditors.CheckEdit();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutGroupConnection = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemConnectionCheckButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemConnectionName = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemConnectionText = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutGroupCredentials = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemCredentialsRadioButtons = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemCredentialsText = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.textConnectionName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.radioGroupSaveCredentials.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkSaveConnection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupConnection)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionCheckButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionName)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionText)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupCredentials)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCredentialsRadioButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCredentialsText)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(748, 382, 749, 739);
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor = System.Drawing.Color.LightGray;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F);
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.textConnectionName.Location = new System.Drawing.Point(100, 71);
			this.textConnectionName.Name = "textConnectionName";
			this.textConnectionName.Size = new System.Drawing.Size(406, 20);
			this.textConnectionName.StyleController = this.layoutControlContent;
			this.textConnectionName.TabIndex = 2;
			this.layoutControlContent.Controls.Add(this.radioGroupSaveCredentials);
			this.layoutControlContent.Controls.Add(this.textConnectionName);
			this.layoutControlContent.Controls.Add(this.labelSaveCredentials);
			this.layoutControlContent.Controls.Add(this.labelSaveConnection);
			this.layoutControlContent.Controls.Add(this.checkSaveConnection);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(0, -8, 1936, 1056);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 1;
			this.radioGroupSaveCredentials.EditValue = false;
			this.radioGroupSaveCredentials.Location = new System.Drawing.Point(98, 178);
			this.radioGroupSaveCredentials.MaximumSize = new System.Drawing.Size(0, 48);
			this.radioGroupSaveCredentials.Name = "radioGroupSaveCredentials";
			this.radioGroupSaveCredentials.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.radioGroupSaveCredentials.Properties.Appearance.Options.UseBackColor = true;
			this.radioGroupSaveCredentials.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.radioGroupSaveCredentials.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Yes, save all required parameters"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(false, "No, skip credentials for security reasons")});
			this.radioGroupSaveCredentials.Size = new System.Drawing.Size(410, 48);
			this.radioGroupSaveCredentials.StyleController = this.layoutControlContent;
			this.radioGroupSaveCredentials.TabIndex = 4;
			this.labelSaveCredentials.Location = new System.Drawing.Point(98, 136);
			this.labelSaveCredentials.Name = "labelSaveCredentials";
			this.labelSaveCredentials.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.labelSaveCredentials.Size = new System.Drawing.Size(252, 26);
			this.labelSaveCredentials.StyleController = this.layoutControlContent;
			this.labelSaveCredentials.TabIndex = 3;
			this.labelSaveCredentials.Text = "The connection uses server authentication. \r\nDo you want to save the user name an" +
	"d password?";
			this.labelSaveConnection.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.labelSaveConnection.Location = new System.Drawing.Point(100, 21);
			this.labelSaveConnection.Name = "labelSaveConnection";
			this.labelSaveConnection.Size = new System.Drawing.Size(406, 13);
			this.labelSaveConnection.StyleController = this.layoutControlContent;
			this.labelSaveConnection.TabIndex = 0;
			this.labelSaveConnection.Text = "Do you want to save the connection string to the application\'s configuration file" +
	"?";
			this.checkSaveConnection.EditValue = true;
			this.checkSaveConnection.Location = new System.Drawing.Point(100, 48);
			this.checkSaveConnection.Name = "checkSaveConnection";
			this.checkSaveConnection.Properties.Caption = "Yes, save the connection as:";
			this.checkSaveConnection.Size = new System.Drawing.Size(406, 19);
			this.checkSaveConnection.StyleController = this.layoutControlContent;
			this.checkSaveConnection.TabIndex = 1;
			this.checkSaveConnection.CheckStateChanged += new System.EventHandler(this.checkSaveConnection_CheckStateChanged);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutGroupConnection,
			this.layoutGroupCredentials});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "Root";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(96, 96, 17, 17);
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutGroupContent.TextVisible = false;
			this.layoutGroupConnection.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupConnection.GroupBordersVisible = false;
			this.layoutGroupConnection.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemConnectionCheckButton,
			this.layoutItemConnectionName,
			this.layoutItemConnectionText});
			this.layoutGroupConnection.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupConnection.Name = "layoutGroupConnection";
			this.layoutGroupConnection.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 39);
			this.layoutGroupConnection.Size = new System.Drawing.Size(414, 117);
			this.layoutGroupConnection.TextVisible = false;
			this.layoutItemConnectionCheckButton.Control = this.checkSaveConnection;
			this.layoutItemConnectionCheckButton.Location = new System.Drawing.Point(0, 27);
			this.layoutItemConnectionCheckButton.Name = "layoutItemConnectionCheckButton";
			this.layoutItemConnectionCheckButton.Size = new System.Drawing.Size(410, 23);
			this.layoutItemConnectionCheckButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemConnectionCheckButton.TextVisible = false;
			this.layoutItemConnectionName.Control = this.textConnectionName;
			this.layoutItemConnectionName.Location = new System.Drawing.Point(0, 50);
			this.layoutItemConnectionName.Name = "layoutItemConnectionName";
			this.layoutItemConnectionName.Size = new System.Drawing.Size(410, 24);
			this.layoutItemConnectionName.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemConnectionName.TextVisible = false;
			this.layoutItemConnectionText.Control = this.labelSaveConnection;
			this.layoutItemConnectionText.Location = new System.Drawing.Point(0, 0);
			this.layoutItemConnectionText.Name = "layoutItemConnectionText";
			this.layoutItemConnectionText.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 12);
			this.layoutItemConnectionText.Size = new System.Drawing.Size(410, 27);
			this.layoutItemConnectionText.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemConnectionText.TextVisible = false;
			this.layoutGroupCredentials.GroupBordersVisible = false;
			this.layoutGroupCredentials.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemCredentialsRadioButtons,
			this.layoutItemCredentialsText});
			this.layoutGroupCredentials.Location = new System.Drawing.Point(0, 117);
			this.layoutGroupCredentials.Name = "layoutGroupCredentials";
			this.layoutGroupCredentials.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 15);
			this.layoutGroupCredentials.Size = new System.Drawing.Size(414, 186);
			this.layoutItemCredentialsRadioButtons.Control = this.radioGroupSaveCredentials;
			this.layoutItemCredentialsRadioButtons.FillControlToClientArea = false;
			this.layoutItemCredentialsRadioButtons.Location = new System.Drawing.Point(0, 42);
			this.layoutItemCredentialsRadioButtons.Name = "layoutItemCredentialsRadioButtons";
			this.layoutItemCredentialsRadioButtons.Size = new System.Drawing.Size(414, 144);
			this.layoutItemCredentialsRadioButtons.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCredentialsRadioButtons.TextVisible = false;
			this.layoutItemCredentialsText.Control = this.labelSaveCredentials;
			this.layoutItemCredentialsText.Location = new System.Drawing.Point(0, 0);
			this.layoutItemCredentialsText.Name = "layoutItemCredentialsText";
			this.layoutItemCredentialsText.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 14);
			this.layoutItemCredentialsText.Size = new System.Drawing.Size(414, 42);
			this.layoutItemCredentialsText.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCredentialsText.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
			this.Name = "SaveConnectionPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.textConnectionName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.radioGroupSaveCredentials.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkSaveConnection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupConnection)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionCheckButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionName)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemConnectionText)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupCredentials)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCredentialsRadioButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCredentialsText)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraEditors.LabelControl labelSaveConnection;
		protected XtraEditors.CheckEdit checkSaveConnection;
		protected XtraEditors.TextEdit textConnectionName;
		protected XtraEditors.RadioGroup radioGroupSaveCredentials;
		protected XtraEditors.LabelControl labelSaveCredentials;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemConnectionText;
		protected XtraLayout.LayoutControlItem layoutItemConnectionCheckButton;
		protected XtraLayout.LayoutControlItem layoutItemConnectionName;
		protected XtraLayout.LayoutControlItem layoutItemCredentialsText;
		protected XtraLayout.LayoutControlItem layoutItemCredentialsRadioButtons;
		protected XtraLayout.LayoutControlGroup layoutGroupConnection;
		protected XtraLayout.LayoutControlGroup layoutGroupCredentials;
	}
}

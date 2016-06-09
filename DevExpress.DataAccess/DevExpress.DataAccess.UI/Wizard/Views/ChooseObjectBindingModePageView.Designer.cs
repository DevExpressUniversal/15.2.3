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
	partial class ChooseObjectBindingModePageView {
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
			this.labelActualData = new DevExpress.XtraEditors.LabelControl();
			this.radioButtonActualData = new DevExpress.DataAccess.UI.Wizard.RadioButton();
			this.labelSchemaOnly = new DevExpress.XtraEditors.LabelControl();
			this.radioButtonSchemaOnly = new DevExpress.DataAccess.UI.Wizard.RadioButton();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemSchemaOnlyButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSchemaOnlyText = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemActualDataButton = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemActualDataText = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.radioButtonActualData.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radioButtonSchemaOnly.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSchemaOnlyButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSchemaOnlyText)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemActualDataButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemActualDataText)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(864, 200, 749, 738);
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelBaseContent.Padding = new System.Windows.Forms.Padding(50, 10, 50, 25);
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.labelActualData);
			this.layoutControlContent.Controls.Add(this.radioButtonActualData);
			this.layoutControlContent.Controls.Add(this.labelSchemaOnly);
			this.layoutControlContent.Controls.Add(this.radioButtonSchemaOnly);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(50, 10);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(605, 147, 622, 490);
			this.layoutControlContent.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControlContent.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControlContent.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControlContent.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControlContent.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(506, 302);
			this.layoutControlContent.TabIndex = 1;
			this.labelActualData.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.labelActualData.Location = new System.Drawing.Point(20, 122);
			this.labelActualData.Name = "labelActualData";
			this.labelActualData.Size = new System.Drawing.Size(484, 26);
			this.labelActualData.StyleController = this.layoutControlContent;
			this.labelActualData.TabIndex = 6;
			this.labelActualData.Text = "The object data source automatically creates an instance of the specified type by" +
	" using one of the available constructors. If only one constructor is available, " +
	"this constructor will be used.";
			this.labelActualData.Click += new System.EventHandler(this.labelActualData_Click);
			this.radioButtonActualData.Location = new System.Drawing.Point(2, 99);
			this.radioButtonActualData.Name = "radioButtonActualData";
			this.radioButtonActualData.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.radioButtonActualData.Properties.Appearance.Options.UseFont = true;
			this.radioButtonActualData.Properties.Caption = "Retrieve the actual data";
			this.radioButtonActualData.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.radioButtonActualData.Properties.RadioGroupIndex = 0;
			this.radioButtonActualData.Size = new System.Drawing.Size(502, 19);
			this.radioButtonActualData.StyleController = this.layoutControlContent;
			this.radioButtonActualData.TabIndex = 9;
			this.radioButtonActualData.CheckedChanged += new System.EventHandler(this.radioActualData_CheckedChanged);
			this.radioButtonActualData.TabStopChanged += new System.EventHandler(this.radioActualData_TabStopChanged);
			this.labelSchemaOnly.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.labelSchemaOnly.Location = new System.Drawing.Point(20, 25);
			this.labelSchemaOnly.Name = "labelSchemaOnly";
			this.labelSchemaOnly.Size = new System.Drawing.Size(484, 52);
			this.labelSchemaOnly.StyleController = this.layoutControlContent;
			this.labelSchemaOnly.TabIndex = 7;
			this.labelSchemaOnly.Text = "Only the data source schema is retrieved from the specified object, without feedi" +
	"ng the actual data to the report until it is published.\r\n\r\nTo manually retrieve " +
	"the actual data, create a <...>";
			this.labelSchemaOnly.Click += new System.EventHandler(this.labelSchemaOnly_Click);
			this.radioButtonSchemaOnly.EditValue = true;
			this.radioButtonSchemaOnly.Location = new System.Drawing.Point(2, 2);
			this.radioButtonSchemaOnly.Name = "radioButtonSchemaOnly";
			this.radioButtonSchemaOnly.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.radioButtonSchemaOnly.Properties.Appearance.Options.UseFont = true;
			this.radioButtonSchemaOnly.Properties.Caption = "Retrieve the data source schema";
			this.radioButtonSchemaOnly.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.radioButtonSchemaOnly.Properties.RadioGroupIndex = 0;
			this.radioButtonSchemaOnly.Size = new System.Drawing.Size(502, 19);
			this.radioButtonSchemaOnly.StyleController = this.layoutControlContent;
			this.radioButtonSchemaOnly.TabIndex = 8;
			this.radioButtonSchemaOnly.CheckedChanged += new System.EventHandler(this.radioSchemaOnly_CheckedChanged);
			this.radioButtonSchemaOnly.TabStopChanged += new System.EventHandler(this.radioSchemaOnly_TabStopChanged);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemSchemaOnlyButton,
			this.layoutItemSchemaOnlyText,
			this.layoutItemActualDataButton,
			this.layoutItemActualDataText});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupContent.Size = new System.Drawing.Size(506, 302);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemSchemaOnlyButton.Control = this.radioButtonSchemaOnly;
			this.layoutItemSchemaOnlyButton.Location = new System.Drawing.Point(0, 0);
			this.layoutItemSchemaOnlyButton.Name = "layoutItemSchemaOnlyButton";
			this.layoutItemSchemaOnlyButton.Size = new System.Drawing.Size(506, 23);
			this.layoutItemSchemaOnlyButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSchemaOnlyButton.TextVisible = false;
			this.layoutItemSchemaOnlyText.Control = this.labelSchemaOnly;
			this.layoutItemSchemaOnlyText.Location = new System.Drawing.Point(0, 23);
			this.layoutItemSchemaOnlyText.Name = "layoutItemSchemaOnlyText";
			this.layoutItemSchemaOnlyText.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 2, 2, 20);
			this.layoutItemSchemaOnlyText.Size = new System.Drawing.Size(506, 74);
			this.layoutItemSchemaOnlyText.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSchemaOnlyText.TextVisible = false;
			this.layoutItemActualDataButton.Control = this.radioButtonActualData;
			this.layoutItemActualDataButton.Location = new System.Drawing.Point(0, 97);
			this.layoutItemActualDataButton.Name = "layoutItemActualDataButton";
			this.layoutItemActualDataButton.Size = new System.Drawing.Size(506, 23);
			this.layoutItemActualDataButton.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemActualDataButton.TextVisible = false;
			this.layoutItemActualDataText.Control = this.labelActualData;
			this.layoutItemActualDataText.Location = new System.Drawing.Point(0, 120);
			this.layoutItemActualDataText.Name = "layoutItemActualDataText";
			this.layoutItemActualDataText.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 2, 2, 20);
			this.layoutItemActualDataText.Size = new System.Drawing.Size(506, 182);
			this.layoutItemActualDataText.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemActualDataText.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(768, 162, 768, 162);
			this.Name = "ChooseObjectBindingModePageView";
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
			((System.ComponentModel.ISupportInitialize)(this.radioButtonActualData.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radioButtonSchemaOnly.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSchemaOnlyButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSchemaOnlyText)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemActualDataButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemActualDataText)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraEditors.LabelControl labelActualData;
		protected RadioButton radioButtonActualData;
		protected XtraEditors.LabelControl labelSchemaOnly;
		protected XtraLayout.LayoutControlItem layoutItemSchemaOnlyText;
		protected XtraLayout.LayoutControlItem layoutItemActualDataButton;
		protected XtraLayout.LayoutControlItem layoutItemActualDataText;
		protected RadioButton radioButtonSchemaOnly;
		protected XtraLayout.LayoutControlItem layoutItemSchemaOnlyButton;
	}
}

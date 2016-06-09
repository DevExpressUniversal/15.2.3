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

namespace DevExpress.DataAccess.UI.Native.Sql {
	partial class DataConnectionParametersForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (this.components != null)) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.separator = new DevExpress.XtraEditors.LabelControl();
			this.parametersPanel = new System.Windows.Forms.Panel();
			this.detailsText = new DevExpress.XtraEditors.MemoEdit();
			this.detailsHeaderText = new DevExpress.XtraEditors.LabelControl();
			this.layoutControlGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemHeaderText = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemDetailsText = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemParametersPanel = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSeparator = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).BeginInit();
			this.panelContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).BeginInit();
			this.layoutControlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.detailsText.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderText)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemDetailsText)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemParametersPanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparator)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.Location = new System.Drawing.Point(377, 308);
			this.btnCancel.Size = new System.Drawing.Size(119, 22);
			this.btnOK.Location = new System.Drawing.Point(252, 308);
			this.btnOK.Size = new System.Drawing.Size(119, 22);
			this.panelContent.Controls.Add(this.layoutControlContent);
			this.panelContent.Size = new System.Drawing.Size(503, 302);
			this.layoutControlMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(649, 190, 839, 575);
			this.layoutControlMain.Size = new System.Drawing.Size(507, 341);
			this.layoutControlMain.Controls.SetChildIndex(this.btnOK, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.btnCancel, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelContent, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelControlAdditionalButtons, 0);
			this.layoutControlGroupMain.Size = new System.Drawing.Size(507, 341);
			this.layoutItemContentPanel.Size = new System.Drawing.Size(507, 306);
			this.layoutItemButtonOk.Size = new System.Drawing.Size(134, 35);
			this.layoutItemButtonCancel.Location = new System.Drawing.Point(134, 0);
			this.layoutItemButtonCancel.Size = new System.Drawing.Size(133, 35);
			this.layoutControlGroupOkCancel.Location = new System.Drawing.Point(240, 0);
			this.layoutControlGroupOkCancel.Size = new System.Drawing.Size(267, 35);
			this.panelControlAdditionalButtons.Location = new System.Drawing.Point(2, 308);
			this.panelControlAdditionalButtons.Size = new System.Drawing.Size(236, 31);
			this.layoutControlItemAdditionalButtons.Size = new System.Drawing.Size(240, 35);
			this.layoutControlContent.Controls.Add(this.separator);
			this.layoutControlContent.Controls.Add(this.parametersPanel);
			this.layoutControlContent.Controls.Add(this.detailsText);
			this.layoutControlContent.Controls.Add(this.detailsHeaderText);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.Root = this.layoutControlGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(503, 302);
			this.layoutControlContent.TabIndex = 10;
			this.separator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.separator.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.separator.LineVisible = true;
			this.separator.Location = new System.Drawing.Point(2, 287);
			this.separator.Name = "separator";
			this.separator.Size = new System.Drawing.Size(499, 13);
			this.separator.StyleController = this.layoutControlContent;
			this.separator.TabIndex = 7;
			this.parametersPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.parametersPanel.Location = new System.Drawing.Point(10, 102);
			this.parametersPanel.Name = "parametersPanel";
			this.parametersPanel.Size = new System.Drawing.Size(483, 181);
			this.parametersPanel.TabIndex = 7;
			this.detailsText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.detailsText.EditValue = "";
			this.detailsText.Location = new System.Drawing.Point(42, 47);
			this.detailsText.Margin = new System.Windows.Forms.Padding(6);
			this.detailsText.Name = "detailsText";
			this.detailsText.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.detailsText.Properties.Appearance.Options.UseBackColor = true;
			this.detailsText.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.detailsText.Properties.ReadOnly = true;
			this.detailsText.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.detailsText.Size = new System.Drawing.Size(419, 43);
			this.detailsText.StyleController = this.layoutControlContent;
			this.detailsText.TabIndex = 8;
			this.detailsHeaderText.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
			this.detailsHeaderText.Cursor = System.Windows.Forms.Cursors.Default;
			this.detailsHeaderText.Location = new System.Drawing.Point(42, 26);
			this.detailsHeaderText.Name = "detailsHeaderText";
			this.detailsHeaderText.Size = new System.Drawing.Size(419, 13);
			this.detailsHeaderText.StyleController = this.layoutControlContent;
			this.detailsHeaderText.TabIndex = 6;
			this.detailsHeaderText.Text = "Unable to connect to the database. See details below.";
			this.layoutControlGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroupContent.GroupBordersVisible = false;
			this.layoutControlGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemHeaderText,
			this.layoutItemDetailsText,
			this.layoutItemParametersPanel,
			this.layoutItemSeparator});
			this.layoutControlGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroupContent.Name = "layoutControlGroupContent";
			this.layoutControlGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroupContent.Size = new System.Drawing.Size(503, 302);
			this.layoutControlGroupContent.TextVisible = false;
			this.layoutItemHeaderText.Control = this.detailsHeaderText;
			this.layoutItemHeaderText.Location = new System.Drawing.Point(0, 0);
			this.layoutItemHeaderText.MaxSize = new System.Drawing.Size(503, 41);
			this.layoutItemHeaderText.MinSize = new System.Drawing.Size(503, 41);
			this.layoutItemHeaderText.Name = "layoutItemHeaderText";
			this.layoutItemHeaderText.Padding = new DevExpress.XtraLayout.Utils.Padding(42, 42, 26, 2);
			this.layoutItemHeaderText.Size = new System.Drawing.Size(503, 41);
			this.layoutItemHeaderText.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemHeaderText.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemHeaderText.TextVisible = false;
			this.layoutItemDetailsText.Control = this.detailsText;
			this.layoutItemDetailsText.Location = new System.Drawing.Point(0, 41);
			this.layoutItemDetailsText.MaxSize = new System.Drawing.Size(0, 51);
			this.layoutItemDetailsText.MinSize = new System.Drawing.Size(102, 51);
			this.layoutItemDetailsText.Name = "layoutItemDetailsText";
			this.layoutItemDetailsText.Padding = new DevExpress.XtraLayout.Utils.Padding(42, 42, 6, 2);
			this.layoutItemDetailsText.Size = new System.Drawing.Size(503, 51);
			this.layoutItemDetailsText.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemDetailsText.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemDetailsText.TextVisible = false;
			this.layoutItemParametersPanel.Control = this.parametersPanel;
			this.layoutItemParametersPanel.Location = new System.Drawing.Point(0, 92);
			this.layoutItemParametersPanel.Name = "layoutItemParametersPanel";
			this.layoutItemParametersPanel.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 2);
			this.layoutItemParametersPanel.Size = new System.Drawing.Size(503, 193);
			this.layoutItemParametersPanel.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemParametersPanel.TextVisible = false;
			this.layoutItemSeparator.Control = this.separator;
			this.layoutItemSeparator.Location = new System.Drawing.Point(0, 285);
			this.layoutItemSeparator.Name = "layoutItemSeparator";
			this.layoutItemSeparator.Size = new System.Drawing.Size(503, 17);
			this.layoutItemSeparator.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSeparator.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = null;
			this.ClientSize = new System.Drawing.Size(507, 341);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DataConnectionParametersForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Connection error";
			this.Load += new System.EventHandler(this.DataConnectionParametersProviderForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.panelContent)).EndInit();
			this.panelContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlMain)).EndInit();
			this.layoutControlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContentPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemButtonCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupOkCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControlAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.detailsText.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderText)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemDetailsText)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemParametersPanel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparator)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraLayout.LayoutControl layoutControlContent;
		protected XtraEditors.LabelControl separator;
		private System.Windows.Forms.Panel parametersPanel;
		private XtraEditors.MemoEdit detailsText;
		private XtraEditors.LabelControl detailsHeaderText;
		private XtraLayout.LayoutControlGroup layoutControlGroupContent;
		private XtraLayout.LayoutControlItem layoutItemHeaderText;
		private XtraLayout.LayoutControlItem layoutItemDetailsText;
		private XtraLayout.LayoutControlItem layoutItemParametersPanel;
		private XtraLayout.LayoutControlItem layoutItemSeparator;
	}
}

﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	partial class ChooseDataSourceNamePageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.textDataSourceName = new DevExpress.XtraEditors.TextEdit();
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemContent = new DevExpress.XtraLayout.LayoutControlItem();
			this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
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
			((System.ComponentModel.ISupportInitialize)(this.textDataSourceName.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContent)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(864, 200, 749, 738);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.textDataSourceName.Location = new System.Drawing.Point(93, 158);
			this.textDataSourceName.MaximumSize = new System.Drawing.Size(420, 0);
			this.textDataSourceName.Name = "textDataSourceName";
			this.textDataSourceName.Size = new System.Drawing.Size(420, 20);
			this.textDataSourceName.StyleController = this.layoutControlContent;
			this.textDataSourceName.TabIndex = 4;
			this.textDataSourceName.TextChanged += new System.EventHandler(this.dataSourceNameTextEdit_TextChanged);
			this.layoutControlContent.Controls.Add(this.textDataSourceName);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 0;
			this.layoutControlContent.Text = "layoutControlContent";
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemContent});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemContent.Control = this.textDataSourceName;
			this.layoutItemContent.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutItemContent.FillControlToClientArea = false;
			this.layoutItemContent.Location = new System.Drawing.Point(0, 0);
			this.layoutItemContent.Name = "layoutItemContent";
			this.layoutItemContent.Size = new System.Drawing.Size(586, 317);
			this.layoutItemContent.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemContent.TextVisible = false;
			this.layoutItemContent.TrimClientAreaToControl = false;
			this.toolTipController.AllowHtmlText = true;
			this.toolTipController.AutoPopDelay = 7000;
			this.toolTipController.CloseOnClick = DevExpress.Utils.DefaultBoolean.True;
			this.toolTipController.IconSize = DevExpress.Utils.ToolTipIconSize.Large;
			this.toolTipController.Rounded = true;
			this.toolTipController.ShowBeak = true;
			this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.Standard;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "ChooseDataSourceNamePageView";
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
			((System.ComponentModel.ISupportInitialize)(this.textDataSourceName.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemContent)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraEditors.TextEdit textDataSourceName;
		protected Utils.ToolTipController toolTipController;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemContent;
	}
}

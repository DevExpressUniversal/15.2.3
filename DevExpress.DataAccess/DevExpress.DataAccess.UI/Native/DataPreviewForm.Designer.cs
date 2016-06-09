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

namespace DevExpress.DataAccess.UI.Native{
	partial class DataPreviewForm {
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.gridControl = new DevExpress.XtraGrid.GridControl();
			this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
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
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.Location = new System.Drawing.Point(609, 457);
			this.btnCancel.Size = new System.Drawing.Size(174, 22);
			this.btnCancel.Text = "Close";
			this.btnOK.Location = new System.Drawing.Point(428, 457);
			this.btnOK.Size = new System.Drawing.Size(175, 22);
			this.panelContent.Controls.Add(this.gridControl);
			this.panelContent.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.panelContent.Size = new System.Drawing.Size(790, 451);
			this.layoutControlMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(649, 190, 839, 575);
			this.layoutControlMain.Size = new System.Drawing.Size(794, 490);
			this.layoutControlMain.Controls.SetChildIndex(this.btnOK, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.btnCancel, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelContent, 0);
			this.layoutControlMain.Controls.SetChildIndex(this.panelControlAdditionalButtons, 0);
			this.layoutControlGroupMain.Size = new System.Drawing.Size(794, 490);
			this.layoutItemContentPanel.Size = new System.Drawing.Size(794, 455);
			this.layoutItemButtonOk.ContentVisible = false;
			this.layoutItemButtonOk.Size = new System.Drawing.Size(190, 35);
			this.layoutItemButtonCancel.Location = new System.Drawing.Point(190, 0);
			this.layoutItemButtonCancel.Size = new System.Drawing.Size(188, 35);
			this.layoutControlGroupOkCancel.Location = new System.Drawing.Point(416, 0);
			this.layoutControlGroupOkCancel.Size = new System.Drawing.Size(378, 35);
			this.panelControlAdditionalButtons.Location = new System.Drawing.Point(2, 457);
			this.panelControlAdditionalButtons.Size = new System.Drawing.Size(412, 31);
			this.layoutControlItemAdditionalButtons.Size = new System.Drawing.Size(416, 35);
			this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
			this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl.Location = new System.Drawing.Point(0, 0);
			this.gridControl.MainView = this.gridView;
			this.gridControl.Name = "gridControl";
			this.gridControl.Size = new System.Drawing.Size(790, 441);
			this.gridControl.TabIndex = 1;
			this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridView});
			this.gridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gridView.GridControl = this.gridControl;
			this.gridView.Name = "gridView";
			this.gridView.OptionsBehavior.Editable = false;
			this.gridView.OptionsBehavior.ReadOnly = true;
			this.gridView.OptionsDetail.EnableMasterViewMode = false;
			this.gridView.OptionsFilter.AllowFilterEditor = false;
			this.gridView.OptionsMenu.EnableColumnMenu = false;
			this.gridView.OptionsSelection.EnableAppearanceFocusedRow = false;
			this.gridView.OptionsView.BestFitMaxRowCount = 1000;
			this.gridView.OptionsView.ColumnAutoWidth = false;
			this.gridView.OptionsView.ShowGroupPanel = false;
			this.gridView.OptionsView.ShowIndicator = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(794, 490);
			this.Name = "DataPreviewForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Data Preview";
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
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraGrid.GridControl gridControl;
		private XtraGrid.Views.Grid.GridView gridView;
	}
}

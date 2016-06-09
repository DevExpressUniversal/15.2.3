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

using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder
{
	partial class FiltersView
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.textSkip = new DevExpress.XtraEditors.TextEdit();
			this.textTop = new DevExpress.XtraEditors.TextEdit();
			this.layoutItemTop = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSkip = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceAfterSkip = new DevExpress.XtraLayout.EmptySpaceItem();
			this.labelTopSkipText = new DevExpress.XtraEditors.LabelControl();
			this.layoutItemTextBeforeSkip = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutGroupGroupFilter = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemGroupFilter = new DevExpress.XtraLayout.LayoutControlItem();
			this.tabbedGroupFilter = new DevExpress.XtraLayout.TabbedControlGroup();
			this.layoutGroupFilter = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceAfterFilter = new DevExpress.XtraLayout.EmptySpaceItem();
			this.checkTopAndSkip = new DevExpress.XtraEditors.CheckEdit();
			this.layoutItemTopAndSkip = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupForm)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOk)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).BeginInit();
			this.layoutMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.textSkip.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textTop.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSkip)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceAfterSkip)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTextBeforeSkip)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupGroupFilter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGroupFilter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabbedGroupFilter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupFilter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceAfterFilter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkTopAndSkip.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTopAndSkip)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFilter)).BeginInit();
			this.SuspendLayout();
			this.layoutGroupForm.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemTop,
			this.layoutItemSkip,
			this.emptySpaceAfterSkip,
			this.layoutItemTextBeforeSkip,
			this.tabbedGroupFilter,
			this.emptySpaceAfterFilter,
			this.layoutItemTopAndSkip});
			this.layoutGroupForm.Name = "Root";
			this.layoutGroupForm.Size = new System.Drawing.Size(644, 384);
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(547, 350);
			this.buttonCancel.Size = new System.Drawing.Size(85, 22);
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(456, 350);
			this.buttonOk.Size = new System.Drawing.Size(85, 22);
			this.buttonOk.Text = "&OK";
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			this.layoutItemOk.Location = new System.Drawing.Point(444, 330);
			this.layoutItemOk.Size = new System.Drawing.Size(90, 34);
			this.layoutItemCancel.Location = new System.Drawing.Point(534, 330);
			this.layoutItemCancel.Size = new System.Drawing.Size(90, 34);
			this.emptySpaceBottom.Location = new System.Drawing.Point(0, 330);
			this.emptySpaceBottom.Size = new System.Drawing.Size(444, 34);
			this.filterControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.filterControl.Location = new System.Drawing.Point(24, 46);
			this.filterControl.Size = new System.Drawing.Size(596, 242);
			this.layoutMain.Controls.Add(this.checkTopAndSkip);
			this.layoutMain.Controls.Add(this.filterControlGroup);
			this.layoutMain.Controls.Add(this.labelTopSkipText);
			this.layoutMain.Controls.Add(this.textTop);
			this.layoutMain.Controls.Add(this.textSkip);
			this.layoutMain.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(512, 117, 960, 1040);
			this.layoutMain.Size = new System.Drawing.Size(644, 384);
			this.layoutMain.Controls.SetChildIndex(this.filterControl, 0);
			this.layoutMain.Controls.SetChildIndex(this.textSkip, 0);
			this.layoutMain.Controls.SetChildIndex(this.textTop, 0);
			this.layoutMain.Controls.SetChildIndex(this.buttonOk, 0);
			this.layoutMain.Controls.SetChildIndex(this.buttonCancel, 0);
			this.layoutMain.Controls.SetChildIndex(this.labelTopSkipText, 0);
			this.layoutMain.Controls.SetChildIndex(this.filterControlGroup, 0);
			this.layoutMain.Controls.SetChildIndex(this.checkTopAndSkip, 0);
			this.textSkip.EditValue = "0";
			this.textSkip.Enabled = false;
			this.textSkip.Location = new System.Drawing.Point(278, 318);
			this.textSkip.Name = "textSkip";
			this.textSkip.Properties.Mask.EditMask = "\\d+";
			this.textSkip.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
			this.textSkip.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.textSkip.Size = new System.Drawing.Size(53, 20);
			this.textSkip.StyleController = this.layoutMain;
			this.textSkip.TabIndex = 5;
			this.textSkip.Validating += new System.ComponentModel.CancelEventHandler(this.numTextEdits_Validating);
			this.textTop.EditValue = "0";
			this.textTop.Enabled = false;
			this.textTop.Location = new System.Drawing.Point(90, 318);
			this.textTop.Name = "textTop";
			this.textTop.Properties.Mask.EditMask = "\\d+";
			this.textTop.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
			this.textTop.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.textTop.Size = new System.Drawing.Size(52, 20);
			this.textTop.StyleController = this.layoutMain;
			this.textTop.TabIndex = 4;
			this.textTop.Validating += new System.ComponentModel.CancelEventHandler(this.numTextEdits_Validating);
			this.filterControlGroup.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.filterControlGroup.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.filterControlGroup.Location = new System.Drawing.Point(24, 46);
			this.filterControlGroup.Name = "filterControlGroup";
			this.filterControlGroup.Size = new System.Drawing.Size(596, 242);
			this.filterControlGroup.TabIndex = 8;
			this.layoutItemTop.Control = this.textTop;
			this.layoutItemTop.Location = new System.Drawing.Point(78, 306);
			this.layoutItemTop.MaxSize = new System.Drawing.Size(56, 24);
			this.layoutItemTop.MinSize = new System.Drawing.Size(56, 24);
			this.layoutItemTop.Name = "layoutItemTop";
			this.layoutItemTop.Size = new System.Drawing.Size(56, 24);
			this.layoutItemTop.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemTop.Text = "Top";
			this.layoutItemTop.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutItemTop.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemTop.TextVisible = false;
			this.layoutItemSkip.Control = this.textSkip;
			this.layoutItemSkip.Location = new System.Drawing.Point(266, 306);
			this.layoutItemSkip.MaxSize = new System.Drawing.Size(57, 24);
			this.layoutItemSkip.MinSize = new System.Drawing.Size(57, 24);
			this.layoutItemSkip.Name = "layoutItemSkip";
			this.layoutItemSkip.Size = new System.Drawing.Size(57, 24);
			this.layoutItemSkip.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemSkip.TextLocation = DevExpress.Utils.Locations.Left;
			this.layoutItemSkip.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSkip.TextVisible = false;
			this.emptySpaceAfterSkip.AllowHotTrack = false;
			this.emptySpaceAfterSkip.Location = new System.Drawing.Point(323, 306);
			this.emptySpaceAfterSkip.Name = "emptySpaceAfterSkip";
			this.emptySpaceAfterSkip.Size = new System.Drawing.Size(301, 24);
			this.emptySpaceAfterSkip.TextSize = new System.Drawing.Size(0, 0);
			this.labelTopSkipText.Location = new System.Drawing.Point(146, 321);
			this.labelTopSkipText.Name = "labelTopSkipText";
			this.labelTopSkipText.Size = new System.Drawing.Size(128, 13);
			this.labelTopSkipText.StyleController = this.layoutMain;
			this.labelTopSkipText.TabIndex = 7;
			this.labelTopSkipText.Text = "records starting with index";
			this.layoutItemTextBeforeSkip.Control = this.labelTopSkipText;
			this.layoutItemTextBeforeSkip.CustomizationFormText = "layoutControlItemTextTopSkip";
			this.layoutItemTextBeforeSkip.Location = new System.Drawing.Point(134, 306);
			this.layoutItemTextBeforeSkip.Name = "layoutItemTextBeforeSkip";
			this.layoutItemTextBeforeSkip.Size = new System.Drawing.Size(132, 24);
			this.layoutItemTextBeforeSkip.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 3, 0);
			this.layoutItemTextBeforeSkip.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemTextBeforeSkip.TextVisible = false;
			this.layoutGroupGroupFilter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemGroupFilter});
			this.layoutGroupGroupFilter.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupGroupFilter.Name = "layoutGroupGroupFilter";
			this.layoutGroupGroupFilter.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.layoutGroupGroupFilter.Size = new System.Drawing.Size(600, 246);
			this.layoutGroupGroupFilter.Text = "&Group Filter";
			this.layoutItemGroupFilter.Control = this.filterControlGroup;
			this.layoutItemGroupFilter.Location = new System.Drawing.Point(0, 0);
			this.layoutItemGroupFilter.Name = "layoutItemGroupFilter";
			this.layoutItemGroupFilter.Size = new System.Drawing.Size(600, 246);
			this.layoutItemGroupFilter.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemGroupFilter.TextVisible = false;
			this.tabbedGroupFilter.Location = new System.Drawing.Point(0, 0);
			this.tabbedGroupFilter.Name = "tabbedGroupFilter";
			this.tabbedGroupFilter.SelectedTabPage = this.layoutGroupFilter;
			this.tabbedGroupFilter.SelectedTabPageIndex = 0;
			this.tabbedGroupFilter.Size = new System.Drawing.Size(624, 292);
			this.tabbedGroupFilter.TabPages.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutGroupFilter,
			this.layoutGroupGroupFilter});
			this.layoutGroupFilter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemFilter});
			this.layoutGroupFilter.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupFilter.Name = "layoutGroupFilter";
			this.layoutGroupFilter.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupFilter.Size = new System.Drawing.Size(600, 246);
			this.layoutGroupFilter.Text = "&Filter";
			this.emptySpaceAfterFilter.AllowHotTrack = false;
			this.emptySpaceAfterFilter.Location = new System.Drawing.Point(0, 292);
			this.emptySpaceAfterFilter.MaxSize = new System.Drawing.Size(0, 14);
			this.emptySpaceAfterFilter.MinSize = new System.Drawing.Size(10, 14);
			this.emptySpaceAfterFilter.Name = "emptySpaceAfterFilter";
			this.emptySpaceAfterFilter.Size = new System.Drawing.Size(624, 14);
			this.emptySpaceAfterFilter.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceAfterFilter.TextSize = new System.Drawing.Size(0, 0);
			this.checkTopAndSkip.Location = new System.Drawing.Point(12, 318);
			this.checkTopAndSkip.Name = "checkTopAndSkip";
			this.checkTopAndSkip.Properties.Caption = "&Select only";
			this.checkTopAndSkip.Size = new System.Drawing.Size(74, 19);
			this.checkTopAndSkip.StyleController = this.layoutMain;
			this.checkTopAndSkip.TabIndex = 10;
			this.layoutItemTopAndSkip.Control = this.checkTopAndSkip;
			this.layoutItemTopAndSkip.Location = new System.Drawing.Point(0, 306);
			this.layoutItemTopAndSkip.MaxSize = new System.Drawing.Size(78, 24);
			this.layoutItemTopAndSkip.MinSize = new System.Drawing.Size(78, 24);
			this.layoutItemTopAndSkip.Name = "layoutItemTopAndSkip";
			this.layoutItemTopAndSkip.Size = new System.Drawing.Size(78, 24);
			this.layoutItemTopAndSkip.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemTopAndSkip.Text = "layoutItemCheckTopAndSkip";
			this.layoutItemTopAndSkip.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemTopAndSkip.TextVisible = false;
			this.layoutItemFilter.Size = new System.Drawing.Size(600, 246);
			this.CancelButton = null;
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(644, 384);
			this.MinimumSize = new System.Drawing.Size(490, 290);
			this.Name = "FiltersView";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FiltersView_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupForm)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemOk)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceBottom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutMain)).EndInit();
			this.layoutMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.textSkip.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textTop.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSkip)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceAfterSkip)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTextBeforeSkip)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupGroupFilter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGroupFilter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabbedGroupFilter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupFilter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceAfterFilter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkTopAndSkip.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTopAndSkip)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFilter)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraLayout.LayoutControlItem layoutItemTop;
		private XtraEditors.TextEdit textTop;
		private XtraEditors.TextEdit textSkip;
		private XtraLayout.LayoutControlItem layoutItemSkip;
		private XtraLayout.EmptySpaceItem emptySpaceAfterSkip;
		private XtraEditors.LabelControl labelTopSkipText;
		private XtraLayout.LayoutControlItem layoutItemTextBeforeSkip;
		private XtraLayout.TabbedControlGroup tabbedGroupFilter;
		private XtraLayout.LayoutControlGroup layoutGroupGroupFilter;
		private XtraLayout.LayoutControlGroup layoutGroupFilter;
		private XtraLayout.EmptySpaceItem emptySpaceAfterFilter;
		private XtraEditors.FilterControl filterControlGroup;
		private XtraLayout.LayoutControlItem layoutItemGroupFilter;
		private CheckEdit checkTopAndSkip;
		private XtraLayout.LayoutControlItem layoutItemTopAndSkip;
	}
}

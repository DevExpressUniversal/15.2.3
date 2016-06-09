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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Utils.Menu;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraGrid.Localization;
using System.Reflection;
using DevExpress.Data.Filtering;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.XtraGrid.FilterEditor {
	[ToolboxItem(false)]
	public class FilterBuilder : DevExpress.XtraEditors.XtraForm {
		protected IFilterControl fcMain = null;
		protected SimpleButton sbOK;
		protected SimpleButton sbCancel;
		protected SimpleButton sbApply;
		private System.ComponentModel.Container components = null;
		ColumnView view;
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup lcgMain;
		private XtraLayout.LayoutControlItem lciOK;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.LayoutControlItem lciCancel;
		private XtraLayout.LayoutControlItem lciApply;
		bool allowClose = true;
		internal IFilterControl GetIFilterControl() { return fcMain; }
		FilterBuilder() {
			InitializeComponent();
			this.MinimumSize = new Size(300, 200);
			CreateButtonCaptions();
		}
		public FilterBuilder(FilterColumnCollection columns, IDXMenuManager manager, UserLookAndFeel lookAndFeel, ColumnView view, FilterColumn fColumn)
			: this() {
			this.view = view;
			CreateFilterControl();
			fcMain.SetFilterColumnsCollection(columns, manager);
			fcMain.SetDefaultColumn(fColumn);
			fcMain.FilterCriteria = view.ActiveFilterCriteria;
			fcMain.ShowOperandTypeIcon = view.OptionsFilter.UseNewCustomFilterDialog;
			fcMain.FilterTextChanged += new FilterTextChangedEventHandler(fcMain_FilterTextChanged);
			if(view.GridControl != null && !view.GridControl.FormsUseDefaultLookAndFeel) {
				fcMain.LookAndFeel.Assign(lookAndFeel);
				SetLookAndFeel(view.ElementsLookAndFeel);
			}
			layoutControl1.Height = sbOK.CalcBestSize().Height + lciOK.Padding.Height + lcgMain.Padding.Height;
			if(view.IsRightToLeft)
				this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			view.InitDialogFormProperties(this);
		}
		public IFilterControl FilterControl { get { return fcMain; } }
		private void CreateFilterControl() {
			if(view.OptionsFilter.DefaultFilterEditorView != FilterEditorViewMode.Visual) {
				view.SetCursor(Cursors.WaitCursor);
				fcMain = FormatRuleMenuHelper.ConstructorInfoInvoker(
					AssemblyInfo.SRAssemblyRichEdit + ", Version=" + AssemblyInfo.Version,
					"DevExpress.XtraFilterEditor.FilterEditorControl") as IFilterControl;
				view.ResetDefaultCursor();
			}
			if(fcMain != null) 
				fcMain.SetViewMode(view.OptionsFilter.DefaultFilterEditorView);
			else 
				fcMain = new GridFilterControl();
			fcMain.UseMenuForOperandsAndOperators = view.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators;
			fcMain.AllowAggregateEditing = view.OptionsFilter.FilterEditorAggregateEditing;
			OnFilterControlCreated(fcMain);
			((Control)fcMain).Dock = DockStyle.Fill;
			((Control)fcMain).Parent = this;
			((Control)fcMain).BringToFront();
		}
		protected virtual void OnFilterControlCreated(IFilterControl filterControl) {
		}
		protected virtual void CreateButtonCaptions() {
			sbOK.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.FilterBuilderOkButton);
			sbCancel.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.FilterBuilderCancelButton);
			sbApply.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.FilterBuilderApplyButton);
			this.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.FilterBuilderCaption);
			this.MinimumSize = new Size(Math.Max(layoutControl1.GetPreferredSize(Size.Empty).Width - emptySpaceItem1.Width + emptySpaceItem1.MinSize.Width + 30, 
				this.MinimumSize.Width), this.MinimumSize.Height);
		}
		protected void HideApplyButton() {
			lciApply.Visibility = XtraLayout.Utils.LayoutVisibility.Never;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterBuilder));
			this.sbOK = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.sbCancel = new DevExpress.XtraEditors.SimpleButton();
			this.sbApply = new DevExpress.XtraEditors.SimpleButton();
			this.lcgMain = new DevExpress.XtraLayout.LayoutControlGroup();
			this.lciOK = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciCancel = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciApply = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lcgMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciApply)).BeginInit();
			this.SuspendLayout();
			this.sbOK.AutoWidthInLayoutControl = true;
			this.sbOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.sbOK, "sbOK");
			this.sbOK.Name = "sbOK";
			this.sbOK.StyleController = this.layoutControl1;
			this.sbOK.Click += new System.EventHandler(this.sbOK_Click);
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.sbCancel);
			this.layoutControl1.Controls.Add(this.sbApply);
			this.layoutControl1.Controls.Add(this.sbOK);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(568, 107, 450, 350);
			this.layoutControl1.Root = this.lcgMain;
			this.sbCancel.AutoWidthInLayoutControl = true;
			this.sbCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.sbCancel, "sbCancel");
			this.sbCancel.Name = "sbCancel";
			this.sbCancel.StyleController = this.layoutControl1;
			this.sbCancel.Click += new System.EventHandler(this.sbCancel_Click);
			this.sbApply.AutoWidthInLayoutControl = true;
			resources.ApplyResources(this.sbApply, "sbApply");
			this.sbApply.Name = "sbApply";
			this.sbApply.StyleController = this.layoutControl1;
			this.sbApply.Click += new System.EventHandler(this.sbApply_Click);
			this.lcgMain.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgMain.GroupBordersVisible = false;
			this.lcgMain.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.lciOK,
			this.emptySpaceItem1,
			this.lciCancel,
			this.lciApply});
			this.lcgMain.Location = new System.Drawing.Point(0, 0);
			this.lcgMain.Name = "lcgMain";
			this.lcgMain.Padding = new DevExpress.XtraLayout.Utils.Padding(9, 9, 10, 10);
			this.lcgMain.Size = new System.Drawing.Size(470, 46);
			this.lcgMain.TextVisible = false;
			this.lciOK.Control = this.sbOK;
			this.lciOK.Location = new System.Drawing.Point(227, 0);
			this.lciOK.Name = "lciOK";
			this.lciOK.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.lciOK.Size = new System.Drawing.Size(75, 26);
			this.lciOK.TextSize = new System.Drawing.Size(0, 0);
			this.lciOK.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(227, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.lciCancel.Control = this.sbCancel;
			this.lciCancel.Location = new System.Drawing.Point(302, 0);
			this.lciCancel.Name = "lciCancel";
			this.lciCancel.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.lciCancel.Size = new System.Drawing.Size(75, 26);
			this.lciCancel.TextSize = new System.Drawing.Size(0, 0);
			this.lciCancel.TextVisible = false;
			this.lciApply.Control = this.sbApply;
			this.lciApply.Location = new System.Drawing.Point(377, 0);
			this.lciApply.Name = "lciApply";
			this.lciApply.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 2, 2);
			this.lciApply.Size = new System.Drawing.Size(75, 26);
			this.lciApply.TextSize = new System.Drawing.Size(0, 0);
			this.lciApply.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.sbCancel;
			this.Controls.Add(this.layoutControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FilterBuilder";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lcgMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciOK)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciCancel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciApply)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void SetLookAndFeel(DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel) {
			this.LookAndFeel.Assign(lookAndFeel);
		}
		private void sbOK_Click(object sender, System.EventArgs e) {
			ApplyFilter();
		}
		private void sbApply_Click(object sender, System.EventArgs e) {
			ApplyFilter();
		}
		private void sbCancel_Click(object sender, EventArgs e) {
			allowClose = true;
		}
		void fcMain_FilterTextChanged(object sender, FilterTextChangedEventArgs e) {
			sbOK.Enabled = e.IsValid;
			sbApply.Enabled = e.IsValid;
		}
		protected virtual void ApplyFilter() {
			try {
				view.AssignActiveFilterFromFilterBuilder(fcMain.FilterCriteria);
				allowClose = true;
			}
			catch(Exception e) {
				XtraMessageBox.Show(LookAndFeel, e.Message, GridLocalizer.Active.GetLocalizedString(GridStringId.WindowErrorCaption), MessageBoxButtons.OK, MessageBoxIcon.Error);
				allowClose = false;
			}
		}
		private void fcMain_KeyPress(object sender, KeyPressEventArgs e) {
			if(e.KeyChar == (char)Keys.Escape) {
				allowClose = true;
				this.Close();
			}
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			base.OnFormClosing(e);
			e.Cancel = !allowClose;
		}
		public CriteriaOperator FilterCriteria { get { return fcMain.FilterCriteria; } }
	}
}

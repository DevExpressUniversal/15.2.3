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
using DevExpress.XtraEditors;
using System.Text.RegularExpressions;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native.Parameters;
using System.ComponentModel.Design;
using System.Collections.Generic;
using DevExpress.Data;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.XtraReports.Design
{
	public class FilterStringEditorForm : ReportsEditorFormBase {
		private DevExpress.XtraEditors.SimpleButton btnOk;
		private string filterString;
		private Panel bottomPanel;
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private SimpleButton btnCancel;
		private FilterStringControl filterControl;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.LayoutControlGroup grpButtons;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		public string FilterString {
			get { return filterString; }
			set {
				filterString = value;
				filterControl.FilterString = filterString;
			}
		}
#if DEBUGTEST
		public DevExpress.XtraFilterEditor.FilterEditorControl Test_FilterControl { get { return filterControl; } }
		public DevExpress.XtraRichEdit.RichEditControl Test_FilterControl_Editor { get { return filterControl.Test_Editor; } }
#endif
		private System.ComponentModel.Container components = null;
		FilterStringEditorForm()
			: base(null) {
			InitializeComponent();
		}
		public FilterStringEditorForm(IServiceProvider provider, object dataSource, string dataMember, IEnumerable<IParameter> parameters, DevExpress.XtraReports.Native.IExtensionsProvider extensionsProvider)
			: base(provider)
		{
			InitializeComponent();
			this.filterControl.Appearance.BackColor = this.BackColor;
			this.filterControl.Initialize(provider, dataSource, dataMember, parameters, extensionsProvider);
			if(this.filterControl.FilterColumns.Count == 0)
				this.filterControl.Enabled = false;
		}
		public FilterStringEditorForm(IServiceProvider provider, object dataSource, string dataMember, IList<IParameter> parameters)
			: base(provider) {
			InitializeComponent();
			this.filterControl.Appearance.BackColor = this.BackColor;
			this.filterControl.Initialize(provider, dataSource, dataMember, parameters);
			if(this.filterControl.FilterColumns.Count == 0)
				this.filterControl.Enabled = false;
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterStringEditorForm));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.filterControl = new DevExpress.XtraReports.Design.FilterStringControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.bottomPanel = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.StyleController = this.layoutControl1;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.filterControl);
			this.layoutControl1.Controls.Add(this.btnOk);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(173, 66, 1445, 888);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			this.filterControl.AllowAggregateEditing = DevExpress.XtraEditors.FilterControlAllowAggregateEditing.AggregateWithCondition;
			this.filterControl.AppearanceEmptyValueColor = System.Drawing.Color.Empty;
			this.filterControl.AppearanceFieldNameColor = System.Drawing.Color.Empty;
			this.filterControl.AppearanceGroupOperatorColor = System.Drawing.Color.Empty;
			this.filterControl.AppearanceOperatorColor = System.Drawing.Color.Empty;
			this.filterControl.AppearanceValueColor = System.Drawing.Color.Empty;
			this.filterControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.filterControl, "filterControl");
			this.filterControl.Name = "filterControl";
			this.filterControl.ShowOperandTypeIcon = true;
			this.filterControl.UseMenuForOperandsAndOperators = false;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.grpButtons});
			this.layoutControlGroup1.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition5.Width = 100D;
			this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition5});
			rowDefinition3.Height = 100D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition4.Height = 31D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup1.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition3,
			rowDefinition4});
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
			this.layoutControlGroup1.Size = new System.Drawing.Size(400, 238);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem1.Control = this.filterControl;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(390, 197);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3,
			this.layoutControlItem2});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 197);
			this.grpButtons.Name = "grpButtons";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition1.Width = 100D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 80D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 2D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 80D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4});
			rowDefinition1.Height = 5D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition2.Height = 26D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2});
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 1;
			this.grpButtons.Size = new System.Drawing.Size(390, 31);
			this.layoutControlItem3.Control = this.btnCancel;
			this.layoutControlItem3.Location = new System.Drawing.Point(310, 5);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem3.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem2.Control = this.btnOk;
			this.layoutControlItem2.Location = new System.Drawing.Point(228, 5);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem2.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.bottomPanel.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.bottomPanel, "bottomPanel");
			this.bottomPanel.Name = "bottomPanel";
			this.AcceptButton = this.btnOk;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.layoutControl1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FilterStringEditorForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.FilterStringEditorForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void btnOk_Click(object sender, System.EventArgs e) {
			if( Equals(this.filterString, filterControl.FilterString) ) {
				DialogResult = DialogResult.Cancel;
			} else {
				this.filterString = filterControl.FilterString;
				DialogResult = DialogResult.OK;
			}
		}
		private void FilterStringEditorForm_Load(object sender, EventArgs e) {
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height) {
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
			}
		}
		void InitializeGroupButtonsLayout() {
			int btnOkBestWidth = btnOk.CalcBestSize().Width;
			int btnCancelBestWidth = btnCancel.CalcBestSize().Width;
			if(btnOkBestWidth <= btnOk.Width && btnCancelBestWidth <= btnCancel.Width)
				return;
			int btnCancelOKActualSize = Math.Max(btnOkBestWidth, btnCancelBestWidth);
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
			grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnCancelOKActualSize + 2 + 2;
			if(grpButtons.Width < 2 * (btnCancelOKActualSize + 2 + 2))
				grpButtons.Width = 2 * (btnCancelOKActualSize + 2 + 2);
		}
	}
}

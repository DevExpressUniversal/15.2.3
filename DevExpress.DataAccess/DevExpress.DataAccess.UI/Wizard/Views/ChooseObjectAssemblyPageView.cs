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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.DataAccess.UI.Wizard.Views {
	public partial class ChooseObjectAssemblyPageView : WizardViewBase, IChooseObjectAssemblyPageView{
		const string HighlightedFieldName = "Highlighted";
		static readonly ColumnFilterInfo showAllFilter = new ColumnFilterInfo(string.Format("[{0}]", HighlightedFieldName));
		AssemblyViewInfo[] data;
		bool showAll = true;
		public ChooseObjectAssemblyPageView() {
			InitializeComponent();
			LocalizeComponent();
		}
		public bool ShowAll {
			get { return showAll; }
			protected set {
				checkEditShowOnlyHighlighted.Checked = !value;
				if(value == showAll)
					return;
				showAll = value;
				gridView.Columns[HighlightedFieldName].FilterInfo = value ? ColumnFilterInfo.Empty : showAllFilter;
			} }
		#region Overrides of WizardViewBase
		public override string HeaderDescription { get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectAssembly); } }
		#endregion
		#region Implementation of IChooseObjectAssemblyPageView
		public void Initialize() {
			ShowAll = false;
		}
		public void SetData(IEnumerable<AssemblyViewInfo> items, bool showAll) {
			data = items.ToArray();
			gridControl.DataSource = data;
			if(data.All(item => !item.Highlighted)) {
				ShowAll = true;
				checkEditShowOnlyHighlighted.Enabled = false;
			}
			else {
				ShowAll = showAll;
				checkEditShowOnlyHighlighted.Enabled = true;
			}
			gridControl.ForceInitialize();
			gridView.Focus();
		}
		public AssemblyViewInfo SelectedItem {
			get {
				int index = gridView.GetFocusedDataSourceRowIndex();
				return index < 0 ? null : data[index];
			}
			set {
				if(value != null && !value.Highlighted)
					ShowAll = true;
				int index = gridView.FindRow(value);
				gridView.FocusedRowHandle = index;
				gridView.SelectRow(index);
				gridView.Focus();
			}
		}
		public event EventHandler Changed;
		#endregion
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			checkEditShowOnlyHighlighted.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectAssembly_ShowOnlyHighlighted);
			gridColumnName.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Name);
			gridColumnVersion.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Version);
		}
		private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			RaiseChanged();
		}
		private void checkEditShowAll_CheckedChanged(object sender, EventArgs e) { ShowAll = !checkEditShowOnlyHighlighted.Checked; }
		private void gridView_DoubleClick(object sender, EventArgs e) {
			GridView view = (GridView)sender;
			GridHitInfo hi = view.CalcHitInfo(view.GridControl.PointToClient(MousePosition));
			if(hi.InRow) {
				this.MoveForward();
			}
		}
		private void gridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e) {
			GridView view = (GridView)sender;
			if(e.RowHandle == view.FocusedRowHandle) {
				Rectangle r = e.Bounds;
				r.X += gridControl.Margin.Left;
				r.Width -= gridControl.Margin.Right;
				e.Appearance.DrawString(e.Cache, e.DisplayText, r);
				e.Handled = true;
			}
		}
	}
}

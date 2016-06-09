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
	public partial class ChooseObjectConstructorPageView : WizardViewBase, IChooseObjectConstructorPageView {
		const string HighlightedFieldName = "Highlighted";
		static readonly ColumnFilterInfo showAllFilter = new ColumnFilterInfo(string.Format("[{0}]", HighlightedFieldName));
		ConstructorViewInfo[] data;
		bool showAll = true;
		public ChooseObjectConstructorPageView() {
			InitializeComponent();
			LocalizeComponent();
		}
		#region Overrides of WizardViewBase
		public override string HeaderDescription { get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectConstructor); } }
		#endregion
		#region Implementation of IChooseObjectConstructorPageView
		public void Initialize(IEnumerable<ConstructorViewInfo> items, bool showAll) {
			data = items.ToArray();
			ShowAll = true;
			checkEditShowOnlyHighlighted.Enabled = false;
			gridControlCtors.DataSource = data.OrderBy(info => info.Parameters.Data.Length);
			if(data.Any(info => info.Highlighted)) {
				ShowAll = showAll;
				checkEditShowOnlyHighlighted.Enabled = true;
			}
		}
		public ConstructorViewInfo Result {
			get {
				return gridViewCtors.GetFocusedRow() as ConstructorViewInfo;
			}
			set {
				if(value != null && !value.Highlighted)
					ShowAll = true;
				SelectRow(value);
			}
		}
		public event EventHandler Changed;
		#endregion
		public bool ShowAll {
			get { return showAll; }
			protected set {
				checkEditShowOnlyHighlighted.Checked = !value;
				if(value == showAll)
					return;
				showAll = value;
				gridViewCtors.Columns[HighlightedFieldName].FilterInfo = value
					? ColumnFilterInfo.Empty
					: showAllFilter;
			} 
		}
		protected void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			checkEditShowOnlyHighlighted.Properties.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardPageChooseObjectConstructor_ShowOnlyHighlighted);
		}
		void SelectRow(ConstructorViewInfo value) {
			int index = gridViewCtors.FindRow(value);
			gridViewCtors.FocusedRowHandle = index;
			gridViewCtors.SelectRow(index);
		}
		private void checkEditShowAll_CheckedChanged(object sender, EventArgs e) { ShowAll = !checkEditShowOnlyHighlighted.Checked; }
		private void gridViewCtors_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			RaiseChanged();
		}
		void gridViewCtors_CustomColumnDisplayText(object sender, XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e) {
			if(e.Value is ParametersViewInfo)
				e.DisplayText = GetCtorDisplayText((ParametersViewInfo)e.Value, e.DisplayText);
		}
		protected virtual string GetCtorDisplayText(ParametersViewInfo info, string defaultValue) {
			return defaultValue;
		}
		private void gridViewCtors_DoubleClick(object sender, EventArgs e) {
			GridView view = (GridView)sender;
			GridHitInfo hi = view.CalcHitInfo(view.GridControl.PointToClient(MousePosition));
			if(hi.InRow) {
				this.MoveForward();
			}
		}
		private void gridViewCtors_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e) {
			GridView view = (GridView)sender;
			if(e.RowHandle == view.FocusedRowHandle) {
				Rectangle r = e.Bounds;
				r.X += gridControlCtors.Margin.Left;
				r.Width -= gridControlCtors.Margin.Right;
				e.Appearance.DrawString(e.Cache, e.DisplayText, r);
				e.Handled = true;
			}
		}
	}
}

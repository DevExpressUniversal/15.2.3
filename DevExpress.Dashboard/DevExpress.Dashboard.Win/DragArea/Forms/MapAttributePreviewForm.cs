#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
namespace DevExpress.DashboardWin.Native {
	public partial class MapAttributePreviewForm : DashboardOkCancelForm {
		object dataSource;
		GridColumn selectedGridColumn;
		public object DataSource {
			get { return dataSource; }
			set {
				dataSource = value;
				UpdatePreview();
			}
		}
		public string SelectedAttribute {
			get { return selectedGridColumn != null ? selectedGridColumn.FieldName : null; }
			set {
				GridColumn gridColumn = gridView.Columns.FirstOrDefault(column => column.FieldName == value);
				ApplyColumnSelection(gridColumn);
			}
		}
		public MapAttributePreviewForm() {
			InitializeComponent();		
		}
		public void PrepareImages(object images) {
			ToolTipController toolTipController = new ToolTipController();
			toolTipController.ImageList = images;
			toolTipController.BeforeShow += (s, e) => {
				GridToolTipInfo info = (GridToolTipInfo)e.SelectedObject;
				GridColumn column = info.Object as GridColumn;
				if(column != null) {
					e.ImageIndex = DataFieldsBrowserItem.GetImageIndex(column.ColumnType);
					e.ToolTip = column.FieldName;
				}
			};
			gridControl.ToolTipController = toolTipController;
			foreach(GridColumn column in gridView.Columns)
				column.ToolTip = "ToolTip";
		}
		void UpdatePreview() {
			gridControl.BeginUpdate();
			try {
				gridView.Columns.Clear();
				gridControl.DataSource = dataSource;
				gridView.BestFitColumns();
			}
			finally {
				gridControl.EndUpdate();
			}
		}
		void gridView_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e) {
			ApplyColumnSelection(gridView.FocusedColumn);
		}
		void gridView_StartSorting(object sender, EventArgs e) {
			ApplyColumnSelection(gridView.SortedColumns[0]);
		}
		void ApplyColumnSelection(GridColumn column) { 
			if(selectedGridColumn != null)
				selectedGridColumn.AppearanceCell.Reset();
			selectedGridColumn = column;
			if(selectedGridColumn != null)
				selectedGridColumn.AppearanceCell.BackColor = Color.Gray;
		}
	}
}

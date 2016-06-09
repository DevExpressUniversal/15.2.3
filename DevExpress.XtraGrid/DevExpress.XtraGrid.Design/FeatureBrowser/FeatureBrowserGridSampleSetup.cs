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
using System.Windows.Forms;
using DevExpress.XtraEditors.FeatureBrowser;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
namespace DevExpress.XtraGrid.FeatureBrowser {
	public abstract class FeatureBrowserGridCustomizationBase : FeatureBrowserSampleControlCustomization {
		public FeatureBrowserGridCustomizationBase()  { }
		public ColumnView ColumnView { get { return SampleObject as ColumnView; } }
		public GridView GridView { get { return SampleObject as GridView; } }
		public CardView CardView { get { return SampleObject as CardView; } }
	}
	public class FeatureBrowserGridFilteringCustomization : FeatureBrowserGridCustomizationBase {
		public FeatureBrowserGridFilteringCustomization()  { }
		public override void Setup() {
			if(ColumnView == null) return;
			if(ColumnView.Columns["Category"] != null)
				ColumnView.Columns["Category"].FilterInfo = new ColumnFilterInfo(ColumnFilterType.Custom, 3, "[Category] = 3");
		}
	}
	public class FeatureBrowserGridSortingCustomization : FeatureBrowserGridCustomizationBase {
		public FeatureBrowserGridSortingCustomization()  { }
		public override void Setup() {
			if(ColumnView == null) return;
			if(ColumnView.Columns["Product Name"] != null) {
				ColumnView.SortInfo.Add(ColumnView.Columns["Product Name"], DevExpress.Data.ColumnSortOrder.Ascending);
			}
		}
	}
	public class FeatureBrowserGridGroupingCustomization : FeatureBrowserGridCustomizationBase {
		public FeatureBrowserGridGroupingCustomization()  { }
		public override void Setup() {
			if(ColumnView == null) return;
			if(ColumnView.Columns["Category"] != null)
				ColumnView.Columns["Category"].Group();
		}
	}
	public class FeatureBrowserGridPreviewCustomization : FeatureBrowserGridCustomizationBase {
		public FeatureBrowserGridPreviewCustomization()  { }
		public override void Setup() {
			if(GridView == null) return;
			GridView.CalcPreviewText += new CalcPreviewTextEventHandler(OnCalcPreviewTextSampleView);
			GridView.LayoutChanged();
		}
		void OnCalcPreviewTextSampleView(object sender, CalcPreviewTextEventArgs e) {
			string text = "";
			for(int i = 0; i < 10; i ++)
				text += "Preview test text. ";
			e.PreviewText = text;
		}
	}
	public class FeatureBrowserGridRowHeightCustomization : FeatureBrowserGridCustomizationBase {
		public FeatureBrowserGridRowHeightCustomization()  { }
		public override void Setup() {
			if(GridView == null) return;
			GridView.BeginDataUpdate();
			try {
				GridColumn column = GridView.Columns.Add();
				column.Caption = "Comments";
				column.UnboundType = DevExpress.Data.UnboundColumnType.String;
				column.FieldName = "Comments";
				GridView.CustomUnboundColumnData += new CustomColumnDataEventHandler(OnCustomUnboundColumnData);
				column.Visible = true;
				RepositoryItemMemoEdit memoRep = new RepositoryItemMemoEdit();
				GridView.GridControl.RepositoryItems.Add(memoRep);
				column.ColumnEdit = memoRep;
			}
			finally {
				GridView.EndDataUpdate();
			}
		}
		void OnCustomUnboundColumnData(object sender, CustomColumnDataEventArgs e) {
			string productName = (string)GridView.GetListSourceRowCellValue(e.ListSourceRowIndex, ColumnView.Columns["Product Name"]);
			e.Value = "Put your comments about the product '" + productName + "' here.";
		}
	}
}

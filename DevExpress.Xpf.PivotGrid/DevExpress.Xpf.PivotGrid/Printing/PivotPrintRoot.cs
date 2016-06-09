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
using System.Windows;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.Xpf.PivotGrid.Printing {
	public abstract class PivotPrintRoot : PivotDataNodeBase, IRootDataNode {
		PivotGridControl pivotGrid;
		Size rowFieldValuesSize, columnFieldValuesSize, columnHeadersSize , dataHeadersSize, rowHeadersSize;
		double printHeight, headersHeight;
		protected PivotPrintRoot(PivotGridControl pivotGrid, PrintSizeArgs sizes) {
			this.pivotGrid = pivotGrid;
			this.printHeight = -1;
			this.ReportFooterSize = sizes.ReportFooterSize;
			this.ReportHeaderSize = sizes.ReportHeaderSize;
			this.PageFooterSize = sizes.PageFooterSize;
			this.PageHeaderSize = sizes.PageHeaderSize;
			this.UsablePageSize = sizes.UsablePageSize;
			this.dataHeadersSize = this.rowHeadersSize = this.columnHeadersSize = Size.Empty;
			this.headersHeight = double.NaN;
#if DEBUGTEST && !SL
			DevExpress.Xpf.PivotGrid.Tests.PrintingTests.PrintPageGCTestHelper.Add(this);
#endif
		}
		protected PivotGridControl PivotGrid {
			get { return pivotGrid; }
		}
		protected PivotVisualItems VisualItems {
			get { return PivotGrid.VisualItems; }
		}
		protected virtual double PrintHeight {
			get {
				if(printHeight == -1)
					printHeight = GetPrintHeight();
				return printHeight;
			}
		}
		public virtual Size ReportHeaderSize { get; set; }
		public virtual Size ReportFooterSize { get; set; }
		public virtual Size PageHeaderSize { get; set; }
		public virtual Size PageFooterSize { get; set; }
		public virtual Size UsablePageSize { get; set; }
		protected internal bool IsReportHeaderOnFirstPage { get; private set; }
		protected internal Size RowFieldValuesSize {
			get { 
				if(rowFieldValuesSize.Width == 0.0)
					rowFieldValuesSize = GetFieldValuesSize(false);
				return rowFieldValuesSize; 
			}
		}
		protected internal Size ColumnFieldValuesSize {
			get {
				if(columnFieldValuesSize.Width == 0.0)
					columnFieldValuesSize = GetFieldValuesSize(true);
				return columnFieldValuesSize; 
			}
		}
		protected double HeadersHeight {
			get {
				if(double.IsNaN(headersHeight))
					CalculateHeadersSize();
				return headersHeight;
			}
		}
		protected Size ColumnHeadersSize {
			get {
				if(columnHeadersSize.IsEmpty)
					CalculateHeadersSize();
				return columnHeadersSize;
			}
		}
		protected Size DataHeadersSize {
			get {
				if(dataHeadersSize.IsEmpty)
					CalculateHeadersSize();
				return dataHeadersSize;
			}
		}
		protected Size RowHeadersSize {
			get {
				if(rowHeadersSize.IsEmpty)
					CalculateHeadersSize();
				return rowHeadersSize;
			}
		}
		void CalculateHeadersSize() {
			headersHeight = PivotPrintPage.GetHeadersSize(PivotGrid.Data, this, ref columnHeadersSize, ref dataHeadersSize, ref rowHeadersSize);
		}
		#region IRootDataNode
		int IRootDataNode.GetTotalDetailCount() {
			if(childNodes.Count == 0)
				CreatePages();
			return childNodes.Count;
		}
		#endregion
		#region IVisualDetailNode
		public override RowViewInfo GetDetail(bool allowContentReuse) {
			return null;
		}
		#endregion
		#region pages creating
		public override void Add(PivotDataNodeBase node) {
			if(node as PivotPrintPage != null)
				base.Add(node);
			else
				throw new ArgumentException("PivotPrintPage");
		}
		void CreatePages() {
			childNodes.Clear();
			IsReportHeaderOnFirstPage = GetIsReportHeaderOnFirstPage();
			CreatePagesCore();
		}
		protected abstract void CreatePagesCore();
		protected virtual PivotPrintPage CreatePrintPage() {
			return new PivotPrintPage(PivotGrid.Data);
		}
		protected void SetHeadersBorderThickness(PivotPrintPage page, double columnFieldValuesWidth) {
			page.ShowColumnAreaHeadersBottomBorder = columnFieldValuesWidth < ColumnHeadersSize.Width && page.ColumnHeadersVisible;
			if(!page.DataHeadersVisible || !page.RowHeadersVisible || VisualItems.GetLevelCount(true) != 1) return;
			if(RowHeadersSize.Height + DataHeadersSize.Height < ColumnFieldValuesSize.Height + ColumnHeadersSize.Height) return;
			page.ShowDataAreaHeadersBottomBorder = RowHeadersSize.Width < DataHeadersSize.Width;
			page.ShowRowAreaHeadersTopBorder = !page.ShowDataAreaHeadersBottomBorder;
		}
		Size GetFieldValuesSize(bool isColumn) {
			Size res = new Size();
			res.Width = GetFieldValuesSizeCore(isColumn, false);
			res.Height = GetFieldValuesSizeCore(isColumn, true);
			return res;
		}
		int GetFieldValuesSizeCore(bool isColumn, bool isHeight) {
			if(!isColumn && !isHeight && PivotGrid.RowTotalsLocation == FieldRowTotalsLocation.Tree)
				return VisualItems.RowTreeWidth;
			int valuesSize = 0;
			int levelCount = isHeight == isColumn ? VisualItems.GetLevelCount(isColumn) : VisualItems.GetLastLevelItemCount(isColumn);
			for(int i = 0; i < levelCount; i++)
				valuesSize += GetItemSize(isColumn, i, isHeight);
			return valuesSize;
		}
		protected int GetItemSize(bool isColumn, int i, bool height) {
			return height ? VisualItems.GetItemHeight(i, isColumn) : VisualItems.GetItemWidth(i, isColumn);
		}
		protected int GetItemSize(PivotFieldValueItem item, bool height) {
			return height ? VisualItems.GetLastLevelItemSize(item).Height : VisualItems.GetLastLevelItemSize(item).Width;
		}
		double GetPrintHeight() {
			double printHeight = UsablePageSize.Height;
			printHeight -= PageHeaderSize.Height;
			printHeight -= PageFooterSize.Height;
			return printHeight;
		}
		protected bool GetIsReportHeaderOnFirstPage() {
			double AllHeight = GetItemSize(VisualItems.GetItem(false, 0), true) + ColumnFieldValuesSize.Height + ReportHeaderSize.Height;
			if(PivotPrintPage.GetHeadersVisibility(PivotGrid.Data, 0))
				AllHeight += HeadersHeight;
			return AllHeight < PrintHeight;
		}
		#endregion
	}
}

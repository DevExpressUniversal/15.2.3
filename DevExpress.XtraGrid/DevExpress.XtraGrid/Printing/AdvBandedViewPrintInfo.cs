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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Printing;
namespace DevExpress.XtraGrid.Views.Printing {
	public class AdvBandedGridViewPrintInfo : BandedGridViewPrintInfo {
		int columnPanelRowCount;
		public AdvBandedGridViewPrintInfo(PrintInfoArgs args) : base(args) {
		}
		protected new AdvBandedGridViewInfo ViewViewInfo { get { return base.ViewViewInfo as AdvBandedGridViewInfo; } }
		protected override void CreateBands() {
			this.columnPanelRowCount = ViewViewInfo.CalcColumnPanelRowCount();
			this.fMaxRowWidth = 0;
			Columns.Clear();
			base.CreateBands();
		}
		protected override int RowLineCount { get { return ColumnPanelRowCount; } }
		protected int ColumnPanelRowCount { get { return columnPanelRowCount; } }
		protected override void CreatePrintColumnCollection() {
		}
		protected override int CalcColumnRowCount(BandedGridColumn column, int rowIndex, bool isLastRow, GridBandRow row) {
			return ViewViewInfo.CalcColumnRowCount(column, isLastRow, rowIndex, row.MaxColumnRowCount);
		}
		protected override void PrintDataRow(int rowHandle, int level) {
			base.PrintDataRow(rowHandle, level);
		}
		protected override int PreparePrintRowCell(PrintColumnInfo colInfo, Point indent, int rowHandle, GridCellInfo cell, int level, int lastRight) {
			Rectangle r = colInfo.Bounds;
			r.Y = CalcBandColumnY(colInfo);
			r.Offset(indent);
			r.Height = this.CurrentRowHeight * colInfo.RowCount;
			int levelIndent = level * ViewViewInfo.LevelIndent;
			if(colInfo.Column.VisibleIndex == 0) {
				r.X += levelIndent;
				r.Width -= levelIndent;
			}
			if(r.X < indent.X + levelIndent) {
				int delta = indent.X + levelIndent - r.X;
				r.X += delta;
				r.Width -= delta;
			}
			if(r.Width > 0 && r.Right > indent.X + levelIndent) {
				PrintRowCell(rowHandle, cell, r);
			} 
			return r.Right;
		}
	}
}

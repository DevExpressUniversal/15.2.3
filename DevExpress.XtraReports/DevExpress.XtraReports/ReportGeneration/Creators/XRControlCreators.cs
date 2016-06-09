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
using System.Linq;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.ReportGeneration.Creators {
	class TableCreator {
		XtraReport report;
		Band band;
		int x;
		XRTable table;
		int editAreaSize;
		int groupingStep;
		public TableCreator(XtraReport report, Band band, int x, int groupingStep) {
			this.report = report;
			this.band = band;
			this.x = x;
			this.groupingStep = groupingStep;
			this.table = new XRTable();
			this.editAreaSize = report.PageWidth - (report.Margins.Left + report.Margins.Right);
		}
		public void InitTable(int rows, int cells){
			table.BeginInit();
			if(band != null && rows != 0){
				report.Bands.Add(band);
				report.Bands.Last().Controls.Add(table);
				CalcTableBounds(x, rows);
				MarkTable(rows, cells);
				band.HeightF = 0;
			}
		}
		void MarkTable(int rows, int cells){
			for(int i = 0; i < rows; i++){
				XRTableRow row = new XRTableRow();
				for(int j = 0; j < cells; j++){
					XRTableCell cell = new XRTableCell();
					row.Cells.Add(cell);
					CalcCellBounds(cell, cells);
				}
				table.Rows.Add(row);
			}
		}
		void CalcCellBounds(XRTableCell cell, int cells){
			float w = cells != 0 ? editAreaSize/ cells : 0;
			if(cell.Index == 0) {
				w -= x;
			}
			cell.SizeF = new SizeF(w, cell.Height);
		}
		void CalcTableBounds(int x, int rows){
			table.Location = new Point(x, 0);
			table.Size = new Size(editAreaSize - x, table.Height*rows);
		}
		public Rectangle TableBounds { get { return table.Bounds; } }
		public XRTableRowCollection Rows { get { return table.Rows; } }
		public Band ReportBand { get { return band; } set { band = value; } }
		public XRTable Table { get { return table; } }
		public void EndInit(){
			table.EndInit();
		}
	}
}

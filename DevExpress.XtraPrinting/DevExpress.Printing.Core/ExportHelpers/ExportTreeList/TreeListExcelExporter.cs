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

using DevExpress.Export;
using DevExpress.Printing.ExportHelpers;
using DevExpress.Utils;
using DevExpress.XtraExport.Csv;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraExport.Xlsx;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DevExpress.Office.Utils;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.XtraExport.Helpers {
	public class TreeListExcelExporter<TCol, TRow> : GridViewExcelExporter<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		public TreeListExcelExporter(IGridView<TCol, TRow> viewToExport, IDataAwareExportOptions options) : base(viewToExport, options) { }
		public TreeListExcelExporter(IGridView<TCol, TRow> viewToExport) : base(viewToExport) { }
		protected override bool CanExportRow(IRowBase row) {
			return ExportInfo.СomplyWithFormatLimits();
		}
		internal override DataAwareExportContext<TCol, TRow> CreateContext(ExporterInfo<TCol, TRow> exportInfo) {
			return new TreeListDataAwareExportContext<TCol, TRow>(exportInfo);
		}
		protected override int ExportDataCore(ref TRow lastExportedRow, ref bool wasDataRow, ref int endExcelGroupIndex, ref int percentage, ref int groupId, TRow gridRow, int startExcelGroupIndex) {
			lastExportedRow = gridRow;
			int currentRowLevel = gridRow.GetRowLevel();
			endExcelGroupIndex = CompleteGrouping(lastExportedRow, endExcelGroupIndex, currentRowLevel);
			startExcelGroupIndex = ExportDataRow(ref wasDataRow, gridRow, startExcelGroupIndex);
			ExportStartGroup(ref wasDataRow, startExcelGroupIndex, endExcelGroupIndex, ref groupId, gridRow, currentRowLevel);
			ExportInfo.ReportProgress(ExportInfo.ExportRowIndex, ref percentage);
			return startExcelGroupIndex;
		}
		protected override void ExportStartGroup(ref bool wasDataRow, int startExcelGroupIndex, int endExcelGroupIndex, ref int groupId, TRow gridRow, int currentRowLevel){
			wasDataRow = false;
			base.ExportStartGroup(ref wasDataRow, startExcelGroupIndex, endExcelGroupIndex, ref groupId, gridRow, currentRowLevel);
		}
		protected override void AddGroupToList(int startIndex, int endIndex){
			base.AddGroupToList(1, endIndex);
		}
	}
	internal class TreeListDataAwareExportContext<TCol, TRow> : DataAwareExportContext<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		public TreeListDataAwareExportContext(ExporterInfo<TCol, TRow> exportInfo) : base(exportInfo) { }
		public override void PrintGroupRowHeader(TRow groupRow) {
		}
	}
}

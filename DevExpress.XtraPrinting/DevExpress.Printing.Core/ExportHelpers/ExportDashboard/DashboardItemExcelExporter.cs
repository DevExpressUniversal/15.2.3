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
using System.Linq;
using System.Text;
using DevExpress.Export;
using DevExpress.Export.Xl;
using DevExpress.Printing.ExportHelpers;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Helpers {
	public class DashboardItemExcelExporter<TCol, TRow> : GridViewExcelExporter<TCol, TRow> where TRow : class, IRowBase where TCol : class, IColumn {
		public DashboardItemExcelExporter(IGridView<TCol, TRow> viewToExport, IDataAwareExportOptions options) : base(viewToExport, options){
		}
		public DashboardItemExcelExporter(IGridView<TCol, TRow> viewToExport) : base(viewToExport){
		}
		protected override bool CanExportRow(IRowBase row){
			return ExportInfo.СomplyWithFormatLimits();
		}
		internal override DataAwareExportContext<TCol, TRow> CreateContext(ExporterInfo<TCol, TRow> exportInfo){
			return new DashboardDataAwareExportContext<TCol, TRow>(exportInfo);
		}
		protected override int ExportDataCore(ref TRow lastExportedRow, ref bool wasDataRow, ref int endExcelGroupIndex, ref int percentage, ref int groupId, TRow gridRow, int startExcelGroupIndex){
			lastExportedRow = gridRow;
			int currentRowLevel = gridRow.GetRowLevel();
			if (!gridRow.IsDataAreaRow && this.ExportInfo.DataAreaBottomRowIndex == 0)
				this.ExportInfo.DataAreaBottomRowIndex = ExportInfo.ExportRowIndex;
			endExcelGroupIndex = CompleteGrouping(lastExportedRow, endExcelGroupIndex, currentRowLevel);
			startExcelGroupIndex = ExportDataRow(ref wasDataRow, gridRow, startExcelGroupIndex);
			ExportStartGroup(ref wasDataRow, startExcelGroupIndex, endExcelGroupIndex, ref groupId, gridRow, currentRowLevel);
			ExportInfo.ReportProgress(ExportInfo.ExportRowIndex, ref percentage);
			return startExcelGroupIndex;
		}
		protected override Action<TCol> ExportColumn(){
			return gridColumn =>{
				int currentColumnLevel = gridColumn.GetColumnGroupLevel();
				while(ExportInfo.GroupsStack.Count > 0 && ExportInfo.GroupsStack.Peek().Group.OutlineLevel >= currentColumnLevel + 1){
					ExportInfo.GroupsStack.Pop();
					Exporter.EndGroup();
				}
				if(gridColumn.IsGroupColumn && Options.AllowGrouping == DefaultBoolean.True){
					ExportInfo.ColumnGrouping = true;
					Context.CreateColumn(gridColumn);
					Context.CreateExportDataGroup(currentColumnLevel, gridColumn.LogicalPosition, 0, gridColumn.IsCollapsed);
				} else Context.CreateColumn(gridColumn);
			};
		}
		protected override void AddGroupToList(int startIndex, int endIndex){
			if(this.ExportInfo.DataAreaBottomRowIndex != 0){
				endIndex = this.ExportInfo.DataAreaBottomRowIndex;
			}
			base.AddGroupToList(startIndex, endIndex);
		}
	}
	internal class DashboardDataAwareExportContext<TCol, TRow> : DataAwareExportContext<TCol, TRow> where TRow : class, IRowBase where TCol : class, IColumn {
		public DashboardDataAwareExportContext(ExporterInfo<TCol, TRow> exportInfo) : base(exportInfo){
		}
		public override void PrintGroupRowHeader(TRow groupRow){
		}
		protected override void CreateCell(Func<TCol, object> value, TRow gridRow, TCol gridColumn){
			IXlCell cell = exportInfo.Exporter.BeginCell();
			CombineFormatSettings(gridRow, gridColumn, cell);
			SetCellValue(gridColumn, gridRow, cell, value(gridColumn));
			exportInfo.Exporter.EndCell();
		}
	}
}

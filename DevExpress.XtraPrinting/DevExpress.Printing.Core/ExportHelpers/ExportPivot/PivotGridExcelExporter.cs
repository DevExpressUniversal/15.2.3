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
namespace DevExpress.XtraExport.Helpers{
	public class PivotGridExcelExporter<TCol, TRow> : GridViewExcelExporter<TCol, TRow> where TRow : class, IRowBase where TCol : class, IColumn {
		public PivotGridExcelExporter(IGridView<TCol, TRow> viewToExport, IDataAwareExportOptions options) : base(viewToExport, options){
		}
		public PivotGridExcelExporter(IGridView<TCol, TRow> viewToExport) : base(viewToExport){
		}
		internal override DataAwareExportContext<TCol, TRow> CreateContext(ExporterInfo<TCol, TRow> exportInfo){
			return new PivotDataAwareExportContext<TCol, TRow>(exportInfo);
		}
		protected override Action<TCol> ExportColumn(){
			return gridColumn => {
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
			if(startIndex == 0) startIndex += ExportInfo.View.FixedRowsCount;
			base.AddGroupToList(startIndex, endIndex);
		}
	}
	internal class PivotDataAwareExportContext<TCol, TRow> : DataAwareExportContext<TCol, TRow> where TRow : class, IRowBase where TCol : class, IColumn {
		public PivotDataAwareExportContext(ExporterInfo<TCol, TRow> exportInfo) : base(exportInfo){
		}
	}
}

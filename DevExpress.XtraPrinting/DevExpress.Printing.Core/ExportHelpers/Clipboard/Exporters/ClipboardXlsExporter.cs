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
using System.IO;
using System.Linq;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraExport.Helpers {
	public class ClipboardXlsExporter<TCol, TRow> : IClipboardExporter<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		MemoryStream ms;
		IXlExport exporter;																										   
		IXlDocument document;
		IXlSheet sheet;
		readonly XlBorder thinBorder;
		public ClipboardXlsExporter() {
			thinBorder = new XlBorder() {
				BottomLineStyle = XlBorderLineStyle.Thin,
				BottomColor = Color.Black,
				TopLineStyle = XlBorderLineStyle.Thin,
				TopColor = Color.Black,
				LeftLineStyle = XlBorderLineStyle.Thin,
				LeftColor = Color.Black,
				RightLineStyle = XlBorderLineStyle.Thin,
				RightColor = Color.Black
			};
		}
		public void BeginExport() {
			exporter = new XlsDataAwareExporter();
			(exporter as XlsDataAwareExporter).ClipboardMode = true; 
			ms = new MemoryStream();
			document = exporter.BeginExport(ms);
			document.Options.Culture = CultureInfo.CurrentCulture;
			sheet = exporter.BeginSheet();
		}
		public void EndExport() {
			exporter.EndSheet();
			exporter.EndExport();
		}
		public void SetDataObject(DataObject data) {
			data.SetData("Biff8", ms);
		}
		public void AddHeaders(IEnumerable<TCol> selectedColumns, IEnumerable<Export.Xl.XlCellFormatting> appearance) {
			IXlRow row = exporter.BeginRow();
			for(int n = 0; n < selectedColumns.Count(); n++) {
				IXlCell cell = exporter.BeginCell();
				cell.ApplyFormatting(appearance.ElementAt(n));
				cell.Value = selectedColumns.ElementAt(n).Header;
				cell.Formatting.Border = thinBorder;
				exporter.EndCell();
			}
			exporter.EndRow();
		}
		public void AddGroupHeader(string groupHeader, Export.Xl.XlCellFormatting appearance, int columnsCount) {
			IXlRow row = exporter.BeginRow();
			for(int i = 0; i < columnsCount; i++) {
				IXlCell cell = exporter.BeginCell();
				cell.ApplyFormatting(appearance);
				cell.Formatting.Border = thinBorder;
				if(i == 0) {
					cell.Value = groupHeader;
				}
				exporter.EndCell();
			}
			sheet.MergedCells.Add(XlCellRange.FromLTRB(0, row.RowIndex, columnsCount - 1, row.RowIndex));
			exporter.EndRow();
		}
		public void AddRow(IEnumerable<ClipboardCellInfo> rowInfo) {
			IXlRow row = exporter.BeginRow();
			for(int n = 0; n < rowInfo.Count(); n++) {
				ClipboardCellInfo cellInfo = rowInfo.ElementAt(n);
				XlCellFormatting format = cellInfo.Formatting;
				IXlCell cell = exporter.BeginCell();
				if(cellInfo.Value != null) {
					if(cellInfo.Value is String)
						cell.Value = XlVariantValue.FromObject(cellInfo.DisplayValue);
					else
						cell.Value = XlVariantValue.FromObject(cellInfo.Value);
				}
				cell.Formatting = XlCellFormatting.FromNetFormat(format.NetFormatString, format.IsDateTimeFormatString);
				cell.Formatting.MergeWith(cellInfo.Formatting);
				cell.Formatting.Border = thinBorder;
				exporter.EndCell();
			}
			exporter.EndRow();
		}
	}
}

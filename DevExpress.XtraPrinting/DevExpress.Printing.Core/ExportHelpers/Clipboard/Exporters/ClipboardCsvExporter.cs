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
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Csv;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraExport.Helpers {
	public class ClipboardCsvExporter<TCol, TRow> :IClipboardExporter<TCol, TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		MemoryStream ms;
		IXlExport exporter;
		IXlDocument document;
		IXlSheet sheet;
		public void BeginExport() {
			exporter = new CsvDataAwareExporter();
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
			data.SetData("Csv", ms);
		}
		public void AddHeaders(IEnumerable<TCol> selectedColumns, IEnumerable<Export.Xl.XlCellFormatting> appearance) {
			IXlRow row = exporter.BeginRow();
			foreach(TCol column in selectedColumns) {
				IXlCell cell = exporter.BeginCell();
				cell.Formatting = new XlCellFormatting();
				cell.Value = column.Header;
				exporter.EndCell();
			}
			exporter.EndRow();
		}
		public void AddGroupHeader(string groupHeader, Export.Xl.XlCellFormatting appearance, int columnsCount) {
			IXlRow row = exporter.BeginRow();
			for(int i = 0; i < columnsCount; i++) {
				IXlCell cell = exporter.BeginCell();
				if(i == 0) cell.Value = groupHeader;
				exporter.EndCell();
			}
			exporter.EndRow();
		}
		public void AddRow(IEnumerable<ClipboardCellInfo> rowsInfo) {
			IXlRow row = exporter.BeginRow();
			for(int n = 0; n < rowsInfo.Count(); n++) {
				IXlCell cell = exporter.BeginCell();
				cell.Value = XlVariantValue.FromObject(rowsInfo.ElementAt(n).DisplayValue);
				exporter.EndCell();
			}
			exporter.EndRow();
		}
	}
}

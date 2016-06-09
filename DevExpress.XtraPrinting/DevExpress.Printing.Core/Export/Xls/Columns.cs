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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraExport.Xls {
	using DevExpress.XtraExport.Implementation;
	#region XlsDataAwareExporter
	public partial class XlsDataAwareExporter {
		XlsTableColumn currentColumn = null;
		public IXlColumn BeginColumn() {
			if(rowContentStarted)
				throw new InvalidOperationException("Columns have to be created before rows and cells.");
			currentColumn = currentSheet.CreateXlsColumn();
			return currentColumn;
		}
		public void EndColumn() {
			if(currentColumn == null)
				throw new InvalidOperationException("BeginColumn/EndColumn calls consistency.");
			if(rowContentStarted)
				throw new InvalidOperationException("Columns have to be created before rows and cells.");
			if(currentColumn.ColumnIndex >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Column index out of range 0...{0}", options.MaxColumnCount - 1));
			currentSheet.RegisterColumn(currentColumn);
			this.currentColumnIndex = currentSheet.ColumnIndex;
			if(currentGroup != null && currentGroup.IsCollapsed)
				currentColumn.IsHidden = true;
			if(currentGroup != null && currentGroup.OutlineLevel > 0)
				currentColumn.OutlineLevel = Math.Min(7, currentGroup.OutlineLevel);
			int index = RegisterFormatting(currentColumn.Formatting);
			if(index < 0)
				index = XlsDefs.DefaultCellXFIndex;
			currentColumn.FormatIndex = index;
			currentColumn = null;
		}
		public void SkipColumns(int count) {
			Guard.ArgumentPositive(count, "count");
			if(currentColumn != null)
				throw new InvalidOperationException("Operation cannot be executed inside BeginColumn/EndColumn scope.");
			int newColumnIndex = currentSheet.ColumnIndex + count;
			if(newColumnIndex >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Column index goes beyond range 0..{0}.", options.MaxColumnCount - 1));
			if(CurrentOutlineLevel > 0) {
				for(int i = 0; i < count; i++) {
					BeginColumn();
					EndColumn();
				}
			}
			else {
				currentSheet.ColumnIndex = newColumnIndex;
			}
		}
		#region Worksheet columns
		void WriteWorksheetColumns() {
			defColWidthRecordPosition = writer.BaseStream.Position;
			WriteContent(XlsRecordType.DefColumnWidth, XlsDefs.DefaultColumnWidth);
			WriteColumns();
		}
		void WriteColumns() {
			List<XlsTableColumn> columns = currentSheet.Columns;
			if(columns.Count == 0)
				return;
			columns.Sort(CompareColumns);
			XlsContentColumnInfo content = new XlsContentColumnInfo();
			foreach(XlsTableColumn column in columns) {
				content.FirstColumn = column.ColumnIndex;
				content.LastColumn = column.ColumnIndex;
				content.Collapsed = column.IsCollapsed;
				content.Hidden = column.IsHidden;
				content.OutlineLevel = column.OutlineLevel;
				if(column.WidthInPixels >= 0) {
					content.ColumnWidth = (int)(ColumnWidthConverter.PixelsToCharactersWidth(column.WidthInPixels, currentSheet.DefaultMaxDigitWidthInPixels) * 256);
					content.CustomWidth = true;
				}
				else {
					content.ColumnWidth = (int)(ColumnWidthConverter.PixelsToCharactersWidth(currentSheet.DefaultColumnWidthInPixels, currentSheet.DefaultMaxDigitWidthInPixels) * 256);
					content.CustomWidth = false;
				}
				content.FormatIndex = column.FormatIndex;
				WriteContent(XlsRecordType.ColInfo, content);
			}
		}
		int CompareColumns(XlsTableColumn x, XlsTableColumn y) {
			if(x == null)
				return y == null ? 0 : -1;
			if(y == null)
				return 1;
			if(x.ColumnIndex < y.ColumnIndex)
				return -1;
			if(x.ColumnIndex > y.ColumnIndex)
				return 1;
			return 0;
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraExport.Implementation {
	#region XlsTableColumn
	public class XlsTableColumn : XlColumn {
		public XlsTableColumn(XlSheet sheet)
			: base(sheet) {
		}
		public int FormatIndex { get; set; }
	}
	#endregion
}

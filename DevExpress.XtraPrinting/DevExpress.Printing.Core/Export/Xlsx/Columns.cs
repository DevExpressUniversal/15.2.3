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
using DevExpress.XtraExport.Implementation;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Export.Xl;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		int columnIndex = -1;
		readonly List<XlColumn> pendingColumns = new List<XlColumn>();
		readonly Dictionary<int, IXlColumn> columns = new Dictionary<int, IXlColumn>();
		XlColumn currentColumn = null;
		public IXlColumn BeginColumn() {
			if(rowContentStarted)
				throw new InvalidOperationException("Columns have to be created before rows and cells.");
			currentColumn = new XlColumn(currentSheet);
			currentColumn.ColumnIndex = columnIndex;
			return currentColumn;
		}
		public void EndColumn() {
			if(currentColumn == null)
				throw new InvalidOperationException("BeginColumn/EndColumn calls consistency.");
			if(rowContentStarted)
				throw new InvalidOperationException("Columns have to be created before rows and cells.");
			if(currentColumn.ColumnIndex >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Column index out of range 0...{0}", options.MaxColumnCount - 1));
			currentSheet.RegisterColumnIndex(currentColumn);
			if(currentGroup != null && currentGroup.IsCollapsed)
				currentColumn.IsHidden = true;
			if(currentGroup != null && currentGroup.OutlineLevel > 0)
				currentColumn.OutlineLevel = Math.Min(7, currentGroup.OutlineLevel);
			pendingColumns.Add(currentColumn);
			columns[currentColumn.ColumnIndex] = currentColumn;
			columnIndex = currentColumn.ColumnIndex + 1;
			currentColumn = null;
		}
		public void SkipColumns(int count) {
			Guard.ArgumentPositive(count, "count");
			if(currentColumn != null)
				throw new InvalidOperationException("Operation cannot be executed inside BeginColumn/EndColumn scope.");
			int newColumnIndex = columnIndex + count;
			if(newColumnIndex >= options.MaxColumnCount)
				throw new ArgumentOutOfRangeException(string.Format("Column index goes beyond range 0..{0}.", options.MaxColumnCount - 1));
			if(CurrentOutlineLevel > 0) {
				for(int i = 0; i < count; i++) {
					BeginColumn();
					EndColumn();
				}
			}
			else {
				columnIndex = newColumnIndex;
			}
		}
		void ExportPendingColumns() {
			if (pendingColumns.Count <= 0)
				return;
			WriteShStartElement("cols");
			try {
				pendingColumns.ForEach(ExportColumn);
			}
			finally {
				WriteShEndElement();
			}
			pendingColumns.Clear();
		}
		protected internal virtual void ExportColumn(XlColumn column) {
			WriteShStartElement("col");
			try {
				ExportColumnProperties(column);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportColumnProperties(XlColumn column) {
			WriteShIntValue("min", column.ColumnIndex + 1);
			WriteShIntValue("max", column.ColumnIndex + 1);
			if (column.IsCollapsed)
				WriteShBoolValue("collapsed", true);
			if (column.IsHidden)
				WriteShBoolValue("hidden", true);
			if(column.OutlineLevel > 0)
				WriteShIntValue("outlineLevel", column.OutlineLevel);
			if (column.WidthInPixels >= 0) {
				WriteStringValue("width", ColumnWidthConverter.PixelsToCharactersWidth(column.WidthInPixels, currentSheet.DefaultMaxDigitWidthInPixels).ToString(CultureInfo.InvariantCulture));
				WriteShIntValue("customWidth", 1);
			}
			else {
				WriteStringValue("width", ColumnWidthConverter.PixelsToCharactersWidth(currentSheet.DefaultColumnWidthInPixels, currentSheet.DefaultMaxDigitWidthInPixels).ToString(CultureInfo.InvariantCulture));
			}
			int index = RegisterFormatting(column.Formatting);
			if (index > 0)
				WriteShIntValue("style", index);
		}
	}
}

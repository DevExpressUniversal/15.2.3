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
using System.Globalization;
using DevExpress.XtraExport.Implementation;
using DevExpress.Export.Xl;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Xlsx {
	partial class XlsxDataAwareExporter {
		int rowIndex = -1;
		XlRow pendingRow;
		IXlRow currentRow = null;
		public int CurrentRowIndex { get { return rowIndex; } }
		public IXlRow BeginRow() {
			ExportPendingSheet();
			this.columnIndex = 0;
			this.insideCellScope = false;
			this.pendingRow = new XlRow(this);
			this.pendingRow.RowIndex = rowIndex;
			this.currentRow = pendingRow;
			this.rowContentStarted = true;
			WriteShStartElement("row");
			return pendingRow;
		}
		public void EndRow() {
			if(currentRow == null)
				throw new InvalidOperationException("BeginRow/EndRow calls consistency.");
			if(currentRow.RowIndex >= options.MaxRowCount)
				throw new ArgumentOutOfRangeException("Maximum number of rows exceeded.");
			ExportPendingRow();
			rowIndex++;
			WriteShEndElement(); 
			currentRow = null;
		}
		public void SkipRows(int count) {
			Guard.ArgumentPositive(count, "count");
			if(currentRow != null)
				throw new InvalidOperationException("Operation cannot be executed inside BeginRow/EndRow scope.");
			int newRowIndex = rowIndex + count;
			if(newRowIndex >= options.MaxRowCount)
				throw new ArgumentOutOfRangeException(string.Format("Row index goes beyond range 0..{0}.", options.MaxRowCount - 1));
			if(CurrentOutlineLevel > 0) {
				for(int i = 0; i < count; i++) {
					BeginRow();
					EndRow();
				}
			}
			else {
				rowIndex = newRowIndex;
			}
		}
		void ExportPendingRow() {
			if (pendingRow == null)
				return;
			WriteShIntValue("r", pendingRow.RowIndex + 1);
			if (pendingRow.IsCollapsed) {
				WriteShBoolValue("collapsed", true);
			}
			if (pendingRow.IsHidden || (currentGroup != null && currentGroup.IsCollapsed))
				WriteShBoolValue("hidden", true);
			if (currentGroup != null && currentGroup.OutlineLevel > 0 && currentGroup.OutlineLevel < 10)
				WriteShIntValue("outlineLevel", currentGroup.OutlineLevel);
			int index = RegisterFormatting(pendingRow.Formatting);
			if (index > 0) {
				WriteShIntValue("s", index);
				WriteBoolValue("customFormat", true);
			}
			if (pendingRow.HeightInPixels >= 0) {
				float height = Math.Min(409.5f, PixelsToPointsF(pendingRow.HeightInPixels, DpiY));
				WriteStringValue("ht", height.ToString(CultureInfo.InvariantCulture));
				WriteBoolValue("customHeight", true);
			}
			this.rowIndex = pendingRow.RowIndex;
			pendingRow = null;
		}
	}
}

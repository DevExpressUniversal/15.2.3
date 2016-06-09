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
#if !SL
	using System.Drawing;
	using DevExpress.XtraPrinting.Native;
#endif
	#region XlsDataAwareExporter
	public partial class XlsDataAwareExporter {
		const int rowsInBlock = 32;
		readonly Stack<XlGroup> groups = new Stack<XlGroup>();
		XlGroup currentGroup;
		int rowCount = 0;
		XlsTableRow currentRow = null;
		readonly List<XlsTableRow> rowsToExport = new List<XlsTableRow>();
		readonly XlsDbCellCalculator dbCellCalculator = new XlsDbCellCalculator();
		public int CurrentOutlineLevel {
			get {
				if(this.currentGroup == null)
					return 0;
				return this.currentGroup.OutlineLevel;
			}
		}
		public IXlGroup BeginGroup() {
			XlGroup group = new XlGroup();
			if(currentGroup != null) {
				group.OutlineLevel = currentGroup.OutlineLevel;
				group.IsCollapsed = currentGroup.IsCollapsed;
			}
			else
				group.OutlineLevel = 1;
			groups.Push(group);
			this.currentGroup = group;
			return group;
		}
		public void EndGroup() {
			groups.Pop();
			if(groups.Count <= 0)
				currentGroup = null;
			else
				currentGroup = groups.Peek();
		}
		public IXlRow BeginRow() {
			this.rowContentStarted = true;
			this.currentColumnIndex = 0;
			currentRow = new XlsTableRow(this);
			currentRow.RowIndex = currentRowIndex;
			return currentRow;
		}
		public void EndRow() {
			if(currentRow == null)
				throw new InvalidOperationException("BeginRow/EndRow calls consistency.");
			if(currentRow.RowIndex >= options.MaxRowCount)
				throw new ArgumentOutOfRangeException("Maximum number of rows exceeded (XLS format). Reduce the number of rows or use XLSX format.");
			if(currentRow.RowIndex < currentRowIndex)
				throw new InvalidOperationException("Row index consistency.");
			this.writer = currentSheet.GetCellTableWriter();
			currentRowIndex = currentRow.RowIndex + 1;
			currentRow.IsHidden |= (currentGroup != null && currentGroup.IsCollapsed);
			if(!currentRow.IsDefault() || (currentGroup != null && currentGroup.OutlineLevel > 0)) {
				rowsToExport.Add(currentRow);
				CalculateAutomaticHeight(currentRow.Formatting);
				currentSheet.RegisterRow(currentRow);
				RegisterDimensions();
				WriteRow();
			}
			rowCount++;
			if(rowCount >= rowsInBlock) {
				WriteRowsContent();
				rowCount = 0;
			}
			currentRow = null;
		}
		public void SkipRows(int count) {
			Guard.ArgumentPositive(count, "count");
			if(currentRow != null)
				throw new InvalidOperationException("Operation cannot be executed inside BeginRow/EndRow scope.");
			int newRowIndex = currentRowIndex + count;
			if(newRowIndex >= options.MaxRowCount)
				throw new ArgumentOutOfRangeException(string.Format("Row index goes beyond range 0..{0}.", options.MaxRowCount - 1));
			if(CurrentOutlineLevel > 0) {
				for(int i = 0; i < count; i++) {
					BeginRow();
					EndRow();
				}
			}
			else {
				currentRowIndex = newRowIndex;
			}
		}
		void CalculateAutomaticHeight(XlCellFormatting formatting) {
#if !SL && !DXPORTABLE
			if (currentRow.IsHidden || currentRow.HeightInPixels >= 0 || formatting == null || formatting.Font == null)
				return;
			XlFont font = formatting.Font;
			FontStyle fontStyle = FontStyle.Regular;
			if(font.Bold)
				fontStyle |= FontStyle.Bold;
			if(font.Italic)
				fontStyle |= FontStyle.Italic;
			using(Font fnt = new Font(font.Name, (float)font.Size, fontStyle, GraphicsUnit.Point)) {
				FontMetrics metrics = FontMetrics.CreateInstance(fnt, GraphicsUnit.Point);
				int height = (int)(metrics.CalculateHeight(1) * DpiY / 72.0) + 3;
				currentRow.AutomaticHeightInPixels = Math.Max(currentRow.AutomaticHeightInPixels, height);
			}
#endif
		}
		void RegisterDimensions() {
			if(currentSheet == null)
				return;
			XlDimensions dimensions = currentSheet.Dimensions;
			if(dimensions == null) {
				dimensions = new XlDimensions();
				dimensions.FirstRowIndex = currentRow.RowIndex;
				dimensions.LastRowIndex = currentRow.RowIndex;
				if(currentRow.Cells.Count > 0) {
					dimensions.FirstColumnIndex = currentRow.FirstColumnIndex;
					dimensions.LastColumnIndex = currentRow.LastColumnIndex;
				}
				else
					dimensions.LastColumnIndex = -1;
				currentSheet.Dimensions = dimensions;
			}
			else {
				dimensions.FirstRowIndex = Math.Min(dimensions.FirstRowIndex, currentRow.RowIndex);
				dimensions.LastRowIndex = Math.Max(dimensions.LastRowIndex, currentRow.RowIndex);
				if(currentRow.Cells.Count > 0) {
					dimensions.FirstColumnIndex = Math.Min(dimensions.FirstColumnIndex, currentRow.FirstColumnIndex);
					dimensions.LastColumnIndex = Math.Max(dimensions.LastColumnIndex, currentRow.LastColumnIndex);
				}
			}
		}
		void WriteRow() {
			this.dbCellCalculator.RegisterRowPosition(writer.BaseStream.Position);
			XlsContentRow content = new XlsContentRow();
			if(currentRow.Cells.Count == 0) {
				content.FirstColumnIndex = 0;
				content.LastColumnIndex = 0;
			}
			else {
				content.FirstColumnIndex = currentRow.FirstColumnIndex;
				content.LastColumnIndex = currentRow.LastColumnIndex + 1;
			}
			int formatIndex = RegisterFormatting(currentRow.Formatting);
			if(formatIndex < 0)
				formatIndex = XlsDefs.DefaultCellXFIndex;
			content.HasFormatting = formatIndex != XlsDefs.DefaultCellXFIndex;
			content.FormatIndex = formatIndex;
			if(currentRow.HeightInPixels >= 0) {
				content.HeightInTwips = Math.Min(8192, PixelsToTwips(currentRow.HeightInPixels, DpiY));
				content.IsCustomHeight = true;
			}
			else if(currentRow.AutomaticHeightInPixels >= 0) {
				content.HeightInTwips = Math.Min(8192, PixelsToTwips(currentRow.AutomaticHeightInPixels, DpiY));
				content.IsCustomHeight = false;
			}
			else {
				content.HeightInTwips = XlsDefs.DefaultRowHeightInTwips;
				content.IsCustomHeight = false;
			}
			content.Index = currentRow.RowIndex;
			content.IsCollapsed = currentRow.IsCollapsed;
			content.IsHidden = currentRow.IsHidden || (currentGroup != null && currentGroup.IsCollapsed);
			if(currentGroup != null && currentGroup.OutlineLevel > 0 && currentGroup.OutlineLevel < 8) {
				content.OutlineLevel = currentGroup.OutlineLevel;
				maxRowOutlineLevel = Math.Max(maxRowOutlineLevel, content.OutlineLevel + 1);
			}
			WriteContent(XlsRecordType.Row, content);
		}
		void WriteRowsContent() {
			int count = this.rowsToExport.Count;
			for(int i = 0; i < count; i++) {
				this.dbCellCalculator.RegisterFirstCellPosition(writer.BaseStream.Position);
				WriteRowCells(this.rowsToExport[i]);
			}
			WriteDbCell();
			this.rowsToExport.Clear();
			this.dbCellCalculator.Reset();
		}
		void WriteDbCell() {
			long position = writer.BaseStream.Position;
			currentSheet.DbCellsPositions.Add(position);
			this.dbCellCalculator.RegisterDbCellPosition(position);
			XlsContentDbCell content = new XlsContentDbCell();
			content.FirstRowOffset = this.dbCellCalculator.CalculateFirstRowOffset();
			content.StreamOffsets.AddRange(this.dbCellCalculator.CalculateStreamOffsets());
			WriteContent(XlsRecordType.DbCell, content);
		}
		void WritePendingRowContent() {
			if(this.rowsToExport.Count > 0)
				WriteRowsContent();
		}
		#region Unit conversion
		float DpiX { get { return DevExpress.XtraPrinting.GraphicsDpi.Pixel; } }
		float DpiY { get { return DevExpress.XtraPrinting.GraphicsDpi.Pixel; } }
		internal static int PixelsToTwips(int val, float dpi) {
			return (int)((val * 1440) / dpi);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraExport.Implementation {
	public class XlsTableRow : XlRow {
		readonly List<XlCell> cells = new List<XlCell>();
		public XlsTableRow(IXlExport exporter) 
			: base(exporter) {
			AutomaticHeightInPixels = -1;
		}
		public IList<XlCell> Cells { get { return cells; } }
		public int FirstColumnIndex { get; set; }
		public int LastColumnIndex { get; set; }
		public int AutomaticHeightInPixels { get; set; }
		public bool IsDefault() {
			bool isCustom = this.Formatting != null || this.HeightInPixels >= 0 || this.IsCollapsed ||
				this.IsHidden || Cells.Count > 0;
			return !isCustom;
		}
	}
}

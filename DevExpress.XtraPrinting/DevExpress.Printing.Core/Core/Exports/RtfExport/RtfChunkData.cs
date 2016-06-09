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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
namespace DevExpress.Printing.Exports.RtfExport {
	public abstract class RtfChunkData {
		#region Statics
		protected static int ConvertDIPToTwips(float value) {
			return (int)GraphicsUnitConverter.Convert(value, GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.Twips);
		}
		protected static int ConvertToTwips(float value, float dpi) {
			return (int)GraphicsUnitConverter.Convert(value, dpi, GraphicsDpi.Twips);
		}
		#endregion
		#region Fields & Properties
		protected List<StringCollection> rowContents, rowDefinitions;
		protected List<int> colRights, rowHeights;
		RtfMultiColumn mc = RtfMultiColumn.None;
		bool autoFitContent = true;
		bool canAutoHeight = true;
		List<RowAdjustment> rowAdjustments;
		int? pageBreakLocation;
		int columnCount, usefulPageWidth;
		protected int currentColIndex, currentRowIndex, currentColSpan, currentRowSpan;
		protected int backColorIndex, borderColorIndex, foreColorIndex;
		protected BrickStyle style;
		public RtfMultiColumn MultiColumn { get { return mc; } set { mc = value; } } 
		public bool CanAutoHeight { get { return canAutoHeight; } set { canAutoHeight = value; } }
		public bool AutoFitContent { get { return autoFitContent; } set { autoFitContent = value; } }
		protected PaddingInfo Padding { get { return style.Padding; } }
		public int UsefulPageWidth { get { return usefulPageWidth; } set { usefulPageWidth = value; } }
		public int? PageBreakLocation { get { return pageBreakLocation; } set { pageBreakLocation = value; } }
		public int ColumnCount { get { return columnCount; } set { columnCount = value; } }
		protected abstract string BackColorTag { get; }
		#endregion
		#region Constructors
		public RtfChunkData(List<int> colWidths, List<int> rowHeights) {
			rowContents = new List<StringCollection>();
			rowDefinitions = new List<StringCollection>();
			rowAdjustments = new List<RowAdjustment>();
			this.rowHeights = rowHeights;
			InitColRights(colWidths);
			FillDefaultRowAdjustmentValues();
		}
		#endregion
		#region Methods
		void FillDefaultRowAdjustmentValues() {
			for(int i = 0; i < rowHeights.Count; i++) {
				rowAdjustments.Add(RowAdjustment.None);
			}
		}
		void InitColRights(List<int> colWidths) {
			int rightBound = 0;
			colRights = new List<int>();
			for(int i = 0; i < colWidths.Count; i++) {
				rightBound += colWidths[i];
				colRights.Add(ConvertDIPToTwips(rightBound));
			}
		}
		public void UpdateDataIndexes(int colIndex, int rowIndex, int colSpan, int rowSpan) {
			this.currentColIndex = colIndex;
			this.currentRowIndex = rowIndex;
			this.currentColSpan = colSpan;
			this.currentRowSpan = rowSpan;
		}
		public void UpdateColors(int backColorIndex, int borderColorIndex, int foreColorIndex) {
			this.backColorIndex = backColorIndex;
			this.borderColorIndex = borderColorIndex;
			this.foreColorIndex = foreColorIndex;
		}
		public void UpdateStyle(BrickStyle style) {
			this.style = style;
		}
		#region Abstraction
		public abstract void FillTemplate();
		public abstract void SetContent(string content);
		public abstract void SetFontString(string fontString);
		protected abstract void WriteHeaderRowDefinition(StringBuilder sb, int index);
		protected abstract void AppendRowContent(StringBuilder sb, StringCollection rowDefinition, StringCollection rowContent);
		protected abstract void InsertDataInContent(int index, int offset, string tag, string data);
		protected abstract void InsertDataInDefinition(int index, int offset, string tag, string data);
		protected abstract void SetTopBorder(int columnIndex, int rowIndex);
		protected abstract void SetBottomBorder(int columnIndex, int rowIndex);
		protected abstract void SetLeftBorder(int columnIndex, int rowIndex);
		protected abstract void SetRightBorder(int columnIndex, int rowIndex);
		public abstract void SetCellVAlign();
		public abstract void SetCellGAlign();
		public abstract void SetPadding();
		#endregion
		public void SetAutoHeightRows(int startRow, int rowCount) {
			int lastRowIndex = startRow + rowCount - 1;
			rowAdjustments[lastRowIndex] = RowAdjustment.AutoHeight;
		}
		public void SetBorders(int columnIndex, int rowIndex) {
			if(style == null || style.Sides == BorderSide.None || style.BorderWidth == 0)
				return;
			BorderSide sides = style.Sides;
			if((sides & BorderSide.Top) > 0) {
				SetTopBorder(columnIndex, rowIndex);
			}
			if((sides & BorderSide.Bottom) > 0) {
				SetBottomBorder(columnIndex, rowIndex);
			}
			if((sides & BorderSide.Left) > 0) {
				SetLeftBorder(columnIndex, rowIndex);
			}
			if((sides & BorderSide.Right) > 0) {
				SetRightBorder(columnIndex, rowIndex);
			}
		}
		public void SetBackColor() {
			SetCellTag(currentColIndex, currentRowIndex, String.Format(BackColorTag, backColorIndex));
		}
		public void SetForeColor() {
			SetContent(String.Format(RtfTags.Color, foreColorIndex));
		}
		public void SetCellUnion() {
			if(currentColSpan > 1) {
				for(int i = currentColIndex + 1; i < currentColIndex + currentColSpan; i++)
					for(int j = currentRowIndex; j < currentRowIndex + currentRowSpan; j++)
						MarkCellHMerged(i, j);
				for(int i = currentRowIndex; i < currentRowIndex + currentRowSpan; i++)
					MarkFirstCellHMerged(i, currentColIndex, currentColSpan);
			}
			if(currentRowSpan > 1)
				MarkFirstCellVMerged();
			for(int i = currentRowIndex + 1; i < currentRowIndex + currentRowSpan; i++) {
				MarkCellVMerged(i);
			}
		}
		public StringBuilder GetResultContent() {
			StringBuilder resultContent = new StringBuilder();
			if(mc == RtfMultiColumn.End) {
				resultContent.Append("}" + RtfTags.SectionEnd + RtfTags.SectionDefault + RtfTags.SectionNoBreak + "{");
			}
			FillContent(resultContent);
			if(mc == RtfMultiColumn.Start) {
				resultContent.Insert(0, "}" + RtfTags.SectionEnd + RtfTags.SectionDefault + RtfTags.SectionNoBreak + 
					string.Format(RtfTags.ColumnCount, columnCount) + RtfTags.SpaceBetweenColumns + "{");
			}
			 return resultContent;
		}
		void FillContent(StringBuilder sb) {
			if(rowDefinitions.Count <= 0) {
				AppendZeroRowContent(sb);
			} else {
				for(int i = 0; i < rowDefinitions.Count; i++) {
					WriteHeaderRowDefinition(sb, i);
					AppendRowContent(sb, rowDefinitions[i], rowContents[i]);
				}
			}
		}
		protected void AddRowDefinition(StringCollection rowDefinition) {
			rowDefinitions.Add(rowDefinition);
		}
		protected void AddRowContent(StringCollection rowContent) {
			rowContents.Add(rowContent);
		}
		protected string GetRowAdjustment(int index) {
			string actualRowTag = IsAutoHeightRow(index) ? RtfTags.AtLeastRowHeight : RtfTags.ExactlyRowHeight;
			string result = string.Format(actualRowTag, ConvertDIPToTwips(rowHeights[index]));
			return autoFitContent ? result + RtfTags.RowAutofit : result;
		}
		bool IsAutoHeightRow(int rowIndex) {
			return canAutoHeight && (rowAdjustments[rowIndex] == RowAdjustment.AutoHeight);
		}
		void AppendZeroRowContent(StringBuilder sb) {
			StringCollection zeroRowDefinition = new StringCollection();
			zeroRowDefinition.Add(RtfTags.ParagraphDefault + RtfTags.DefaultRow + RtfTags.NewLine);
			StringCollection zeroRowContent = new StringCollection();
			zeroRowContent.Add(RtfTags.EmptyCell + RtfTags.NewLine);
			AppendRowContent(sb, zeroRowDefinition, zeroRowContent);
		}
		protected void SetCellTag(int columnIndex, int rowIndex, string tag) {
			InsertDataInDefinition(rowIndex, columnIndex, RtfTags.CellRight, tag);
		}
		protected void SetBorder(string border, int columnIndex, int rowIndex) {
			int borderWidth = ConvertDIPToTwips(style.BorderWidth);
			BorderDashStyle borderStyle = style.BorderDashStyle;
			SetCellTag(columnIndex, rowIndex, RtfTags.NewLine + border);
			SetCellTag(columnIndex, rowIndex, RtfExportBorderHelper.GetFullBorderStyle(borderStyle, borderWidth, borderColorIndex));
		}
		void MarkCellHMerged(int columnIndex, int rowIndex) {
			SetBorders(columnIndex, rowIndex);
			SetCellTag(columnIndex, rowIndex, RtfTags.MergedCell);
		}
		void MarkFirstCellHMerged(int rowIndex, int colIndex, int colSpan) {
			string str = rowDefinitions[rowIndex][colIndex];
			int endIndex = str.IndexOf(RtfTags.CellRight) + RtfTags.CellRight.Length;
			rowDefinitions[rowIndex][colIndex] = str.Substring(0, endIndex) + colRights[colIndex + colSpan - 1].ToString();
			SetCellTag(colIndex, rowIndex, RtfTags.FirstMergedCell);
		}
		void MarkFirstCellVMerged() {
			SetCellTag(currentColIndex, currentRowIndex, RtfTags.FirstVerticalMergedCell);
		}
		void MarkCellVMerged(int rowIndex) {
			SetBorders(currentColIndex, rowIndex);
			SetCellTag(currentColIndex, rowIndex, RtfTags.VerticalMergedCell);
		}
		#endregion
	}
	public class TableRtfChunkData : RtfChunkData {
		protected override string BackColorTag { get { return RtfTags.BackgroundPatternBackgroundColor; } }
		public TableRtfChunkData(List<int> colWidths, List<int> rowHeights)
			: base(colWidths, rowHeights) {
		}
		public override void FillTemplate() {
			if(PrintingSettings.UseNewSingleFileRtfExport) {
				for(int i = 0; i < rowHeights.Count; i++) {
					int currentRowHeight = rowHeights[i];
					if(currentRowHeight == 0) {
						AddRowDefinition(new StringCollection());
						AddRowContent(new StringCollection());
						continue;
					}
					StringCollection rowDefinition = CreateTableRowDefinition();
					StringCollection rowContent = CreateTableRowContent();
					AddRowDefinition(rowDefinition);
					AddRowContent(rowContent);
				}
			} else {
				for(int i = 0; i < rowHeights.Count; i++) {
					AddRowDefinition(CreateTableRowDefinition());
					AddRowContent(CreateTableRowContent());
				}
			}
		}
		public override void SetContent(string content) {
			InsertDataInContent(currentRowIndex, currentColIndex, RtfTags.EndOfCell, content);
		}
		public override void SetFontString(string fontString) {
			SetContent(fontString);
		}
		protected override void WriteHeaderRowDefinition(StringBuilder sb, int index) {
			if(rowHeights[index] > 0) {
				sb.Append(String.Format(RtfTags.ParagraphDefault + RtfTags.DefaultRow + GetRowAdjustment(index) + RtfTags.NewLine));
				if(PageBreakLocation.HasValue) {
					sb.Append(RtfTags.PageBreak);
					PageBreakLocation = null;
				}
			}
		}
		protected override void AppendRowContent(StringBuilder sb, StringCollection rowDefinition, StringCollection rowContent) {
			System.Diagnostics.Debug.Assert(rowDefinition != null && rowContent != null);
			if(rowDefinition.Count > 0) {
				foreach(string str in rowDefinition) {
					sb.Append(str);
				}
				if(rowContent.Count > 0) {
					sb.Append(RtfTags.NewLine + RtfTags.LeftToRightWrite + RtfTags.NewLine + RtfTags.SuggestToTable + RtfTags.NewLine);
					foreach(string str in rowContent) {
						sb.Append(str);
					}
				}
				sb.Append(RtfTags.EndOfRow + RtfTags.NewLine + RtfTags.NewLine);
			}
		}
		protected override void InsertDataInDefinition(int index, int offset, string tag, string data) {
			string str = rowDefinitions[index][offset];
			int pos = str.IndexOf(tag);
			rowDefinitions[index][offset] = str.Insert(pos, data);
		}
		protected override void InsertDataInContent(int index, int offset, string tag, string data) {
			string str = rowContents[index][offset];
			int pos = str.IndexOf(tag);
			rowContents[index][offset] = str.Insert(pos, data);
		}
		StringCollection CreateTableRowContent() {
			StringCollection stringCollection = new StringCollection();
			for(int i = 0; i < colRights.Count; i++) {
				stringCollection.Add(RtfTags.EmptyCell + RtfTags.NewLine);
			}
			return stringCollection;
		}
		StringCollection CreateTableRowDefinition() {
			StringCollection stringCollection = new StringCollection();
			for(int i = 0; i < colRights.Count; i++) {
				stringCollection.Add(RtfTags.CellRight + colRights[i].ToString());
			}
			return stringCollection;
		}
		#region Borders processing
		protected override void SetTopBorder(int columnIndex, int rowIndex) {
			SetBorder(RtfTags.TopCellBorder, columnIndex, rowIndex);
		}
		protected override void SetBottomBorder(int columnIndex, int rowIndex) {
			SetBorder(RtfTags.BottomCellBorder, columnIndex, rowIndex);
		}
		protected override void SetLeftBorder(int columnIndex, int rowIndex) {
			SetBorder(RtfTags.LeftCellBorder, columnIndex, rowIndex);
		}
		protected override void SetRightBorder(int columnIndex, int rowIndex) {
			SetBorder(RtfTags.RightCellBorder, columnIndex, rowIndex);
		}
		#endregion
		public override void SetCellVAlign() {
			SetCellTag(currentColIndex, currentRowIndex, RtfAlignmentConverter.ToVertRtfAlignment(style.TextAlignment));
		}
		public override void SetCellGAlign() {
			SetContent(RtfAlignmentConverter.ToHorzRtfAlignment(style.TextAlignment));
		}
		public override void SetPadding() {
			if(Padding.Top > 0) 
				SetContent(string.Format(RtfTags.SpaceBefore, ConvertToTwips(Padding.Top, Padding.Dpi)));
			if(Padding.Bottom > 0)
				SetContent(string.Format(RtfTags.SpaceAfter, ConvertToTwips(Padding.Bottom, Padding.Dpi)));
			if(Padding.Left > 0)
				SetContent(string.Format(RtfTags.LeftIndent, ConvertToTwips(Padding.Left, Padding.Dpi)));
			if(Padding.Right > 0)
				SetContent(string.Format(RtfTags.RightIndent, ConvertToTwips(Padding.Right, Padding.Dpi)));
		}
	}
	public class PlainRtfChunkData : RtfChunkData {
		protected override string BackColorTag { get { return RtfTags.BackgroundPatternColor; } }
		public PlainRtfChunkData(List<int> colWidths, List<int> rowHeights)
			: base(colWidths, rowHeights) {
		}
		public override void FillTemplate() {
			StringCollection content = new StringCollection();
			content.Add("{}");
			StringCollection definition = new StringCollection();
			definition.Add("");
			AddRowDefinition(definition);
			AddRowContent(content);
		}
		public override void SetContent(string content) {
			InsertDataInContent(currentRowIndex, currentColIndex, "}", content);
		}
		public override void SetFontString(string fontString) {
			SetContent(fontString + " ");
		}
		protected override void WriteHeaderRowDefinition(StringBuilder sb, int index) {
			sb.Append(RtfTags.ParagraphDefault + RtfTags.PlainText);
			if(PageBreakLocation.HasValue) {
				sb.Append(RtfTags.PageBreak);
				PageBreakLocation = null;
			}
		}
		protected override void AppendRowContent(StringBuilder sb, StringCollection rowDefinition, StringCollection rowContent) {
			System.Diagnostics.Debug.Assert(rowDefinition != null && rowContent != null);
			if(rowDefinition.Count > 0) {
				sb.Append(rowDefinition[0]);
				if(Padding.Top > 0 || rowHeights.Count > 1) {
					int spaceBefore = 0;
					if(rowHeights.Count > 1)
						spaceBefore += ConvertDIPToTwips(rowHeights[0]);
					if(Padding.Top > 0)
						spaceBefore += ConvertToTwips(Padding.Top, Padding.Dpi);
					sb.AppendFormat(RtfTags.SpaceBefore, spaceBefore);
				}
				if(Padding.Bottom > 0)
					sb.AppendFormat(RtfTags.SpaceAfter, ConvertToTwips(Padding.Bottom, Padding.Dpi));
				if(colRights.Count > 1 || Padding.Left > 0) {
					int leftIndent = 0;
					if(colRights.Count > 1)
						leftIndent += colRights[0];
					if(Padding.Left > 0)
						leftIndent += ConvertToTwips(Padding.Left, Padding.Dpi);
					sb.AppendFormat(RtfTags.LeftIndent, leftIndent);
				}
				int rightIndent = UsefulPageWidth - colRights[colRights.Count - 1];
				if(Padding.Right > 0)
					rightIndent += ConvertToTwips(Padding.Right, Padding.Dpi);
				sb.AppendFormat(RtfTags.RightIndent, rightIndent);
				if(rowContent.Count > 0) {
					foreach(string str in rowContent) {
						sb.Append(str);
					}
				}
				sb.Append(RtfTags.ParagraphEnd + RtfTags.NewLine + RtfTags.NewLine);
			}
		}
		protected override void InsertDataInContent(int index, int offset, string tag, string data) {
			string str = rowContents[0][0];
			int pos = str.IndexOf(tag);
			rowContents[0][0] = str.Insert(pos, data);
		}
		protected override void InsertDataInDefinition(int index, int offset, string tag, string data) {
			rowDefinitions[0][0] += data;
		}
		#region Borders processing
		protected override void SetTopBorder(int columnIndex, int rowIndex) {
			SetBorder(RtfTags.TopBorder, columnIndex, rowIndex);
		}
		protected override void SetBottomBorder(int columnIndex, int rowIndex) {
			SetBorder(RtfTags.BottomBorder, columnIndex, rowIndex);
		}
		protected override void SetLeftBorder(int columnIndex, int rowIndex) {
			SetBorder(RtfTags.LeftBorder, columnIndex, rowIndex);
		}
		protected override void SetRightBorder(int columnIndex, int rowIndex) {
			SetBorder(RtfTags.RightBorder, columnIndex, rowIndex);
		}
		#endregion
		public override void SetCellVAlign() {
		}
		public override void SetCellGAlign() {
			InsertDataInDefinition(currentRowIndex, currentColIndex, "", RtfAlignmentConverter.ToHorzRtfAlignment(style.TextAlignment));
		}
		public override void SetPadding() {
		}
	}
	public enum RowAdjustment : byte {
		None = 0,
		AutoHeight = 1,  
	}
	public enum RtfMultiColumn : byte { 
		None,
		Start,
		Middle,
		End
	}
}

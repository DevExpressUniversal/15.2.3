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
using System.Collections;
using System.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.Web.Internal;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	internal class SpreadsheetClipboardHelper {
		private string PastedValue { get; set; }
		private DocumentModel Model { get; set; }
		private Worksheet ActiveSheet { get; set; }
		private InnerSpreadsheetControl InnerControl { get; set; }
		private Guid BufferId { get; set; }
		private int rigthBottomColIndex = -1;
		private int rigthBottomRowIndex = -1;
		public SpreadsheetClipboardHelper(InnerSpreadsheetControl innerControl, string pastedValue, Guid bufferId) {
			InnerControl = innerControl;
			Model = InnerControl.DocumentModel;
			ActiveSheet = Model.ActiveSheet;
			PastedValue = pastedValue;
			BufferId = bufferId;
		}
		public void ProcessClipboardEvent() {
			if(!string.IsNullOrEmpty(PastedValue)) {
				ArrayList cellValues = HtmlConvertor.FromJSON<ArrayList>(PastedValue);
				if(cellValues != null && cellValues.Count > 0) {
					Model.BeginUpdate();
					for(int i = 0; i < cellValues.Count; i++) {
						Hashtable cellFormat = cellValues[i] as Hashtable;
						if(cellFormat != null) {
							if(cellFormat.ContainsKey("ServerBufferId")) {
								Guid serverBufferId = new Guid(cellFormat["ServerBufferId"].ToString());
								if(!serverBufferId.Equals(Guid.Empty) && serverBufferId.Equals(BufferId)) {
									SpreadsheetCommand command = InnerControl.CreateCommand(SpreadsheetCommandId.PasteSelection);
									if(IsServerClipboardDataAvailable(command)) {
										Model.EndUpdate();
										command.Execute();
										return;
									} else {
										continue;
									}
								}
							} else {
								int ColIndex = int.Parse(cellFormat["ColIndex"].ToString());
								int RowIndex = int.Parse(cellFormat["RowIndex"].ToString());
								ICell cell = Model.ActiveSheet[ColIndex, RowIndex];
								ApplyMerging(cell, cellFormat);
								ApplyCellValue(cell, cellFormat);
								ApplyImageUrl(cell, cellFormat);
								ApplyCellFormat(cell, cellFormat);
								ApplyFontFormat(cell, cellFormat);
							}
						}
					}
					SelectPastedRange(cellValues);
					Model.EndUpdate();
				}
			}
		}
		protected bool IsServerClipboardDataAvailable(SpreadsheetCommand command) {
			if(command != null) {
				DefaultCommandUIState state = new DefaultCommandUIState();
				command.UpdateUIState(state);
				return state.Enabled;
			}
			return false;
		}
		protected void ApplyMerging(ICell cell, Hashtable cellFormat) {
			int ColSpan = int.Parse(cellFormat["ColSpan"].ToString());
			int RowSpan = int.Parse(cellFormat["RowSpan"].ToString());
			if(ColSpan > 1 || RowSpan > 1) {
				ActiveSheet.MergeCells(new CellRange(ActiveSheet,
					new CellPosition(cell.ColumnIndex, cell.RowIndex),
					new CellPosition(cell.ColumnIndex + ColSpan - 1, cell.RowIndex + RowSpan - 1)), DevExpress.XtraSpreadsheet.API.Native.Implementation.ApiErrorHandler.Instance);
			}
		}
		protected void ApplyCellValue(ICell cell, Hashtable cellFormat) {
			if(cellFormat["Value"] != null && !string.IsNullOrEmpty(cellFormat["Value"].ToString())) {
				if(cellFormat["Link"] != null && !string.IsNullOrEmpty(cellFormat["Link"].ToString())) {
					Hashtable linkProperty = cellFormat["Link"] as Hashtable;
					if(linkProperty != null && linkProperty.Count > 0 && linkProperty.ContainsKey("Url") && !string.IsNullOrEmpty(linkProperty["Url"].ToString())) {
						InsertHyperlinkCommand command = new InsertHyperlinkCommand(DevExpress.XtraSpreadsheet.API.Native.Implementation.ApiErrorHandler.Instance, ActiveSheet,
							new CellRange(ActiveSheet, new CellPosition(cell.ColumnIndex, cell.RowIndex), new CellPosition(cell.ColumnIndex, cell.RowIndex)), linkProperty["Url"].ToString(),
							true, cellFormat["Value"].ToString(), false);
						command.TooltipText = linkProperty["Alt"] != null ? linkProperty["Alt"].ToString() : string.Empty;
						command.Execute();
						return;
					}
				}
				cell.SetTextSmart(cellFormat["Value"].ToString());
			} else
				cell.SetValue("");
			rigthBottomColIndex = rigthBottomColIndex < cell.ColumnIndex ? cell.ColumnIndex : rigthBottomColIndex;
			rigthBottomRowIndex = rigthBottomRowIndex < cell.RowIndex ? cell.RowIndex : rigthBottomRowIndex;
		}
		protected void ApplyImageUrl(ICell cell, Hashtable cellFormat) {
			if(cellFormat["ImgUrl"] != null && !string.IsNullOrEmpty(cellFormat["ImgUrl"].ToString())) {
				string url = cellFormat["ImgUrl"].ToString();
				SpreadsheetDownloadHelper downloadHelper = new SpreadsheetDownloadHelper(Model);
				downloadHelper.InsertImageFromUrlToDocumentModel(cell, url);
			}
		}
		protected void ApplyCellFormat(ICell cell, Hashtable cellFormat) {
			if(cellFormat["BackgroundColor"] != null && !string.IsNullOrEmpty(cellFormat["BackgroundColor"].ToString())) {
				cell.Fill.PatternType = XlPatternType.Solid;
				cell.Fill.ForeColor = ColorUtils.ColorFromHexColor(cellFormat["BackgroundColor"].ToString());
			} else {
				cell.Fill.PatternType = XlPatternType.None;
				cell.Fill.ForeColor = Color.Transparent;
			}
			if(cellFormat.ContainsKey("Border"))
				ApplyBordersFormat(cell, cellFormat["Border"] as Hashtable);
			if(cellFormat["VAlign"] != null && !string.IsNullOrEmpty(cellFormat["VAlign"].ToString()))
				cell.Alignment.Vertical = (XlVerticalAlignment)Enum.Parse(typeof(XlVerticalAlignment), cellFormat["VAlign"].ToString());
			else
				cell.Alignment.Vertical = XlVerticalAlignment.Bottom;
			if(cellFormat["HAlign"] != null && !string.IsNullOrEmpty(cellFormat["HAlign"].ToString()))
				cell.Alignment.Horizontal = (XlHorizontalAlignment)Enum.Parse(typeof(XlHorizontalAlignment), cellFormat["HAlign"].ToString());
			else
				cell.Alignment.Horizontal = XlHorizontalAlignment.General;
			cell.Alignment.WrapText = cellFormat["Wrap"].ToString().ToLower() == "true";
		}
		#region Borders
		protected void ApplyBordersFormat(ICell cell, Hashtable borders) {
			if(borders != null) {
				MergedBorderInfo info = new MergedBorderInfo();
				CellRange range = ActiveSheet.MergedCells.GetMergedCellRange(cell.ColumnIndex, cell.RowIndex);
				if(range == null) {
					range = new CellRange(ActiveSheet, new CellPosition(cell.ColumnIndex, cell.RowIndex), new CellPosition(cell.ColumnIndex, cell.RowIndex));
				}
				ApplyLeftBorder(info, borders["Left"] as Hashtable);
				ApplyTopBorder(info, borders["Top"] as Hashtable);
				ApplyRightBorder(info, borders["Right"] as Hashtable);
				ApplyBottomBorder(info, borders["Bottom"] as Hashtable);
				var command = new ChangeRangeBordersCommand(range, info);
				command.Execute();
			}
		}
		protected void ApplyLeftBorder(MergedBorderInfo borderInfo, Hashtable border) {
			if(border["Color"] != null && !string.IsNullOrEmpty(border["Color"].ToString()))
				borderInfo.LeftColor = ColorUtils.ColorFromHexColor(border["Color"].ToString());
			if(border["Style"] != null && !string.IsNullOrEmpty(border["Style"].ToString()))
				borderInfo.LeftLineStyle = (XlBorderLineStyle)Enum.Parse(typeof(XlBorderLineStyle), border["Style"].ToString());
			else
				borderInfo.LeftLineStyle = XlBorderLineStyle.None;
		}
		protected void ApplyRightBorder(MergedBorderInfo borderInfo, Hashtable border) {
			if(border["Color"] != null && !string.IsNullOrEmpty(border["Color"].ToString()))
				borderInfo.RightColor = ColorUtils.ColorFromHexColor(border["Color"].ToString());
			if(border["Style"] != null && !string.IsNullOrEmpty(border["Style"].ToString()))
				borderInfo.RightLineStyle = (XlBorderLineStyle)Enum.Parse(typeof(XlBorderLineStyle), border["Style"].ToString());
			else
				borderInfo.RightLineStyle = XlBorderLineStyle.None;
		}
		protected void ApplyTopBorder(MergedBorderInfo borderInfo, Hashtable border) {
			if(border["Color"] != null && !string.IsNullOrEmpty(border["Color"].ToString()))
				borderInfo.TopColor = ColorUtils.ColorFromHexColor(border["Color"].ToString());
			if(border["Style"] != null && !string.IsNullOrEmpty(border["Style"].ToString()))
				borderInfo.TopLineStyle = (XlBorderLineStyle)Enum.Parse(typeof(XlBorderLineStyle), border["Style"].ToString());
			else
				borderInfo.TopLineStyle = XlBorderLineStyle.None;
		}
		protected void ApplyBottomBorder(MergedBorderInfo borderInfo, Hashtable border) {
			if(border["Color"] != null && !string.IsNullOrEmpty(border["Color"].ToString()))
				borderInfo.BottomColor = ColorUtils.ColorFromHexColor(border["Color"].ToString());
			if(border["Style"] != null && !string.IsNullOrEmpty(border["Style"].ToString()))
				borderInfo.BottomLineStyle = (XlBorderLineStyle)Enum.Parse(typeof(XlBorderLineStyle), border["Style"].ToString());
			else
				borderInfo.BottomLineStyle = XlBorderLineStyle.None;
		}
		#endregion
		protected void ApplyFontFormat(ICell cell, Hashtable cellFormat) {
			if(cellFormat.ContainsKey("Font")) {
				Hashtable fontFormat = cellFormat["Font"] as Hashtable;
				if(fontFormat["Color"] != null && !string.IsNullOrEmpty(fontFormat["Color"].ToString()))
					cell.Font.Color = ColorUtils.ColorFromHexColor(fontFormat["Color"].ToString());
				else
					cell.Font.Color = Color.Black;
				if(fontFormat["Name"] != null && !string.IsNullOrEmpty(fontFormat["Name"].ToString()))
					cell.Font.Name = fontFormat["Name"].ToString();
				else
					cell.Font.Name = "Calibri";
				if(fontFormat["Size"] != null && !string.IsNullOrEmpty(fontFormat["Size"].ToString()))
					cell.Font.Size = double.Parse(fontFormat["Size"].ToString());
				else
					cell.Font.Size = 11;
				if(fontFormat["Bold"] != null && !string.IsNullOrEmpty(fontFormat["Bold"].ToString()))
					cell.Font.Bold = fontFormat["Bold"].ToString().ToLower() == "true";
				else
					cell.Font.Bold = false;
				if(fontFormat["Style"] != null && !string.IsNullOrEmpty(fontFormat["Style"].ToString()))
					cell.Font.Italic = fontFormat["Style"].ToString() == "italic";
				else
					cell.Font.Italic = false;
				if(fontFormat["Decoration"] != null && !string.IsNullOrEmpty(fontFormat["Decoration"].ToString()))
					cell.Font.Underline = fontFormat["Decoration"].ToString().Contains("underline") ? XlUnderlineType.Single : XlUnderlineType.None;
				else
					cell.Font.Underline = XlUnderlineType.None;
				if(fontFormat["Decoration"] != null && !string.IsNullOrEmpty(fontFormat["Decoration"].ToString()))
					cell.Font.StrikeThrough = fontFormat["Decoration"].ToString().Contains("line-through");
				else
					cell.Font.StrikeThrough = false;
			}
		}
		public void SelectPastedRange(ArrayList cellValues) {
			Hashtable leftTopCellParams = cellValues[0] as Hashtable;
			if(leftTopCellParams.ContainsKey("ServerBufferId"))
				leftTopCellParams = cellValues[1] as Hashtable;
			if(leftTopCellParams != null) {
				CellPosition activeCell = new CellPosition(int.Parse(leftTopCellParams["ColIndex"].ToString()), int.Parse(leftTopCellParams["RowIndex"].ToString()));
				CellPosition rightBottomCell = new CellPosition(rigthBottomColIndex, rigthBottomRowIndex);
				CellRange cellRange = new CellRange(ActiveSheet, activeCell, rightBottomCell);
				ActiveSheet.Selection.SetSelection(cellRange, activeCell, true);
			}
		}
	}
}

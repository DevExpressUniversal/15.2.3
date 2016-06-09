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
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	public class XlsNoteInfo {
		CellPosition position = new CellPosition();
		public CellPosition Position { get { return position; } set { position = value; } }
		public bool IsShown { get; set; }
		public bool IsRowHidden { get; set; }
		public bool IsColumnHidden { get; set; }
		public int ObjId { get; set; }
		public string Author { get; set; }
		public void Register(XlsContentBuilder contentBuilder) {
			if(contentBuilder.CommentObjects.ContainsKey(ObjId)) {
				OfficeArtShapeContainer shape = contentBuilder.CommentObjects[ObjId];
				int authorId = contentBuilder.DocumentModel.CommentAuthors.AddIfNotPresent(Author);
				VmlShape modelShape = CreateModelShape(shape, Position, contentBuilder);
				int shapeId = contentBuilder.CurrentSheet.VmlDrawing.Shapes.RegisterShape(modelShape);
				Comment comment = new Comment(contentBuilder.CurrentSheet, Position, authorId, shapeId);
				SetupTextRuns(comment, shape, contentBuilder);
				contentBuilder.CurrentSheet.Comments.Add(comment);
			}
		}
		#region Shape properties
		VmlShape CreateModelShape(OfficeArtShapeContainer shape, CellPosition position, XlsContentBuilder contentBuilder) {
			VmlShape vmlShape = new VmlShape(contentBuilder.CurrentSheet);
			vmlShape.BeginUpdate();
			try {
				vmlShape.ClientData.ObjectType = VmlObjectType.Note;
				vmlShape.ClientData.Row = position.Row;
				vmlShape.ClientData.Column = position.Column;
				SetupVisibility(vmlShape, shape);
				SetupFill(vmlShape, shape, contentBuilder);
				SetupStroke(vmlShape, shape, contentBuilder);
				SetupShadow(vmlShape, shape, contentBuilder);
				SetupConnectionPoints(vmlShape, shape);
				SetupClientAnchor(vmlShape, contentBuilder.CurrentSheet, shape);
				SetupProtection(vmlShape, shape);
				SetupTextProperties(vmlShape, shape, contentBuilder);
			}
			finally {
				vmlShape.EndUpdate();
			}
			return vmlShape;
		}
		void SetupVisibility(VmlShape vmlShape, OfficeArtShapeContainer shape) {
			bool hidden = true;
			DrawingGroupShapeBooleanProperties bpGroupShape = shape.ArtProperties.GetGroupShapeBooleanProperties();
			if(bpGroupShape != null && bpGroupShape.UseHidden)
				hidden = bpGroupShape.Hidden;
			vmlShape.IsHidden = hidden;
		}
		void SetupClientAnchor(VmlShape vmlShape, Worksheet sheet, OfficeArtShapeContainer shape) {
			OfficeArtClientAnchorSheet clientAnchor = shape.ClientAnchor as OfficeArtClientAnchorSheet;
			if(clientAnchor != null) {
				vmlShape.ClientData.MoveWithCells = !clientAnchor.KeepOnMove;
				vmlShape.ClientData.SizeWithCells = !clientAnchor.KeepOnResize;
				PageGridCalculator calculator = new PageGridCalculator(sheet, Rectangle.Empty);
				VmlAnchorData anchorData = new VmlAnchorData(sheet);
				int leftColumn = clientAnchor.TopLeft.Column;
				int leftOffset = CalculateOffsetX(sheet, calculator, clientAnchor.TopLeft.Column, clientAnchor.DeltaXLeft);
				int topRow = clientAnchor.TopLeft.Row;
				int topOffset = CalculateOffsetY(sheet, calculator, clientAnchor.TopLeft.Row, clientAnchor.DeltaYTop);
				int rightColumn = clientAnchor.BottomRight.Column;
				int rightOffset = CalculateOffsetX(sheet, calculator, clientAnchor.BottomRight.Column, clientAnchor.DeltaXRight);
				int bottomRow = clientAnchor.BottomRight.Row;
				int bottomOffset = CalculateOffsetY(sheet, calculator, clientAnchor.BottomRight.Row, clientAnchor.DeltaYBottom);
				anchorData.SetFrom(sheet,leftColumn, leftOffset, topRow, topOffset);
				anchorData.SetTo(sheet, rightColumn, rightOffset, bottomRow, bottomOffset);
				vmlShape.ClientData.Anchor = anchorData;
			}
		}
		void SetupFill(VmlShape vmlShape, OfficeArtShapeContainer shape, XlsContentBuilder contentBuilder) {
			if(shape.ArtProperties.UseFilled)
				vmlShape.Filled = shape.ArtProperties.Filled;
			DrawingFillColor fillColor = shape.ArtProperties.GetFillColorProperties();
			if(fillColor != null)
				vmlShape.Fillcolor = OfficeColorToRGB(fillColor.ColorRecord, contentBuilder.DocumentModel);
			DrawingFillOpacity fillOpacity = shape.ArtProperties.GetFillOpacityProperties();
			if(fillOpacity != null)
				vmlShape.Fill.Opacity = (float)fillOpacity.Value;
			DrawingFillBackColor fillBackColor = shape.ArtProperties.GetFillBackColorProperties();
			if(fillBackColor != null)
				vmlShape.Fill.Color2 = OfficeColorToRGB(fillBackColor.ColorRecord, contentBuilder.DocumentModel);
		}
		void SetupStroke(VmlShape vmlShape, OfficeArtShapeContainer shape, XlsContentBuilder contentBuilder) {
			DrawingLineColor lineColor = shape.ArtProperties.GetLineColorProperties();
			if(lineColor != null)
				vmlShape.Strokecolor = OfficeColorToRGB(lineColor.ColorRecord, contentBuilder.DocumentModel);
			vmlShape.Strokeweight = contentBuilder.DocumentModel.UnitConverter.EmuToModelUnits(shape.ArtProperties.LineWidthInEMUs);
			if(shape.ArtProperties.UseLine)
				vmlShape.Stroked = shape.ArtProperties.Line;
		}
		void SetupShadow(VmlShape vmlShape, OfficeArtShapeContainer shape, XlsContentBuilder contentBuilder) {
			DrawingShadowColor shadowColor = shape.ArtProperties.GetShadowColorProperties();
			if(shadowColor != null)
				vmlShape.Shadow.Color = OfficeColorToRGB(shadowColor.ColorRecord, contentBuilder.DocumentModel);
			DrawingShadowStyleBooleanProperties shadowStyle = shape.ArtProperties.GetShadowStyleBooleanProperties();
			if(shadowStyle != null) {
				if(shadowStyle.UseShadow)
					vmlShape.Shadow.On = shadowStyle.Shadow;
				if(shadowStyle.UseShadowObscured)
					vmlShape.Shadow.Obscured = shadowStyle.ShadowObscured;
			}
		}
		void SetupConnectionPoints(VmlShape vmlShape, OfficeArtShapeContainer shape) {
			DrawingConnectionPointsType prop = shape.ArtProperties.GetConnectionPointsType();
			if(prop != null) {
				ConnectionPointsType type = prop.PointsType;
				switch(type) {
					case ConnectionPointsType.None:
						vmlShape.Path.Connecttype = VmlConnectType.None;
						break;
					case ConnectionPointsType.Segments:
						vmlShape.Path.Connecttype = VmlConnectType.Segments;
						break;
					case ConnectionPointsType.Custom:
						vmlShape.Path.Connecttype = VmlConnectType.Custom;
						break;
					case ConnectionPointsType.Rect:
						vmlShape.Path.Connecttype = VmlConnectType.Rect;
						break;
				}
			}
		}
		void SetupProtection(VmlShape vmlShape, OfficeArtShapeContainer shape) {
			OfficeArtClientData clientData = shape.ClientData;
			if(clientData != null) {
				XlsObjCommon commonProperties = clientData.Data.CommonProperties;
				if(commonProperties != null)
					vmlShape.ClientData.Locked = commonProperties.Locked;
			}
			OfficeArtClientTextbox clientTextbox = shape.ClientTextbox;
			if(clientTextbox != null)
				vmlShape.ClientData.LockText = clientTextbox.Data.IsLocked;
		}
		void SetupTextProperties(VmlShape vmlShape, OfficeArtShapeContainer shape, XlsContentBuilder contentBuilder) {
			OfficeArtClientTextbox clientTextbox = shape.ClientTextbox;
			if(clientTextbox != null) {
				OfficeTextReadingOrder direction = OfficeTextReadingOrder.LeftToRight;
				DrawingTextDirection prop = shape.ArtProperties.GetTextDirection();
				if(prop != null)
					direction = prop.Direction;
				if(direction == OfficeTextReadingOrder.LeftToRight)
					vmlShape.Textbox.TextDirection = XlReadingOrder.LeftToRight;
				else if(direction == OfficeTextReadingOrder.RightToLeft)
					vmlShape.Textbox.TextDirection = XlReadingOrder.RightToLeft;
				else
					vmlShape.Textbox.TextDirection = XlReadingOrder.Context;
				vmlShape.Textbox.TextAlign = clientTextbox.Data.HorizontalAlignment;
				vmlShape.ClientData.TextHAlign = clientTextbox.Data.HorizontalAlignment;
				vmlShape.ClientData.TextVAlign = clientTextbox.Data.VerticalAlignment;
			}
			DrawingTextBooleanProperties textProps = shape.ArtProperties.GetTextBooleanProperties();
			if(textProps != null) {
				if(textProps.UseAutoTextMargins)
					vmlShape.InsetMode = textProps.AutoTextMargins ? VmlInsetMode.Auto : VmlInsetMode.Custom;
				if(textProps.UseFitShapeToText)
					vmlShape.Textbox.FitShapeToText = textProps.FitShapeToText;
			}
			if(vmlShape.InsetMode == VmlInsetMode.Custom) {
				VmlInsetData inset = VmlInsetHelper.GetDefault(contentBuilder.DocumentModel);
				DocumentModelUnitConverter unitConverter = contentBuilder.DocumentModel.UnitConverter;
				DrawingTextLeft leftMargin = shape.ArtProperties.GetPropertyByType(typeof(DrawingTextLeft)) as DrawingTextLeft;
				if(leftMargin != null)
					inset.LeftMargin = unitConverter.EmuToModelUnits(leftMargin.Value);
				DrawingTextTop topMargin = shape.ArtProperties.GetPropertyByType(typeof(DrawingTextTop)) as DrawingTextTop;
				if(topMargin != null)
					inset.TopMargin = unitConverter.EmuToModelUnits(topMargin.Value);
				DrawingTextRight rightMargin = shape.ArtProperties.GetPropertyByType(typeof(DrawingTextRight)) as DrawingTextRight;
				if(rightMargin != null)
					inset.RightMargin = unitConverter.EmuToModelUnits(rightMargin.Value);
				DrawingTextBottom bottomMargin = shape.ArtProperties.GetPropertyByType(typeof(DrawingTextBottom)) as DrawingTextBottom;
				if(bottomMargin != null)
					inset.BottomMargin = unitConverter.EmuToModelUnits(bottomMargin.Value);
				vmlShape.Textbox.Inset = inset;
			}
		}
		int CalculateOffsetX(Worksheet sheet, PageGridCalculator calculator, int columnIndex, int delta) {
			float layoutColumnWidth = calculator.InnerCalculator.CalculateColumnWidth(sheet, columnIndex, calculator.MaxDigitWidth, calculator.MaxDigitWidthInPixels);
			float modelUnitsColumnWidth = sheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutColumnWidth);
			return sheet.Workbook.UnitConverter.ModelUnitsToPixels((int)(modelUnitsColumnWidth * delta / 1024), DocumentModel.DpiX);
		}
		int CalculateOffsetY(Worksheet sheet, PageGridCalculator calculator, int rowIndex, int delta) {
			float layoutRowHeight = calculator.InnerCalculator.CalculateRowHeight(sheet, rowIndex);
			float modelUnitsRowHeight = sheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutRowHeight);
			return sheet.Workbook.UnitConverter.ModelUnitsToPixels((int)(modelUnitsRowHeight * delta / 256), DocumentModel.DpiY);
		}
		Color OfficeColorToRGB(OfficeColorRecord colorRecord, DocumentModel workbook) {
			DrawingColorModelInfo colorInfo;
			if(colorRecord.ColorSchemeUsed) {
				ColorModelInfo colorModelInfo = ColorModelInfo.Create(colorRecord.ColorSchemeIndex);
				colorInfo = DrawingColorModelInfo.CreateARGB(colorModelInfo.ToRgb(workbook.StyleSheet.Palette, workbook.OfficeTheme.Colors));
			} else if(colorRecord.SystemColorUsed) {
				if(Enum.IsDefined(typeof(SystemColorValues), colorRecord.SystemColorIndex))
					colorInfo = DrawingColorModelInfo.CreateSystem((SystemColorValues)colorRecord.SystemColorIndex);
				else
					colorInfo = DrawingColorModelInfo.CreateSystem(SystemColorValues.Empty);
			} else
				colorInfo = DrawingColorModelInfo.CreateARGB(colorRecord.Color);
			return colorInfo.ToRgb(workbook.OfficeTheme.Colors);
		}
		#endregion
		#region Text runs
		void SetupTextRuns(Comment comment, OfficeArtShapeContainer shape, XlsContentBuilder contentBuilder) {
			OfficeArtClientTextbox clientTextbox = shape.ClientTextbox;
			if(clientTextbox != null) {
				string text = clientTextbox.Data.Text;
				IList<XlsFormatRun> formatRuns = clientTextbox.Data.FormatRuns;
				if(formatRuns.Count == 0)
					AddCommentRun(comment, text, -1);
				else {
					int lastCharIndex = 0;
					int lastFontIndex = 0;
					int length = text.Length;
					for(int i = 0; i < formatRuns.Count; i++) {
						XlsFormatRun formatRun = formatRuns[i];
						if(formatRun.CharIndex >= length)
							break; 
						int runLength = formatRun.CharIndex - lastCharIndex;
						if(runLength > 0) {
							AddCommentRun(comment, text.Substring(lastCharIndex, runLength),
								contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
						}
						lastCharIndex = formatRun.CharIndex;
						lastFontIndex = formatRun.FontIndex;
						if(lastFontIndex == XlsDefs.UnusedFontIndex)
							contentBuilder.ThrowInvalidFile("Invalid font index in comment run");
						else if(lastFontIndex > XlsDefs.UnusedFontIndex)
							lastFontIndex--;
					}
					if((length - lastCharIndex) > 0) {
						AddCommentRun(comment, text.Substring(lastCharIndex, length - lastCharIndex),
							contentBuilder.StyleSheet.GetFontInfoIndex(lastFontIndex));
					}
				}
			}
		}
		void AddCommentRun(Comment comment, string partContent, int fontIndex) {
			CommentRun item = new CommentRun(comment.Worksheet);
			item.Text = partContent;
			if(fontIndex >= 0) {
				RunFontInfo fontInfo = new RunFontInfo();
				fontInfo.CopyFrom(comment.Workbook.Cache.FontInfoCache[fontIndex]);
				item.ReplaceInfo(fontInfo, DocumentModelChangeActions.None);
			}
			comment.Runs.AddCore(item);
		}
		#endregion
	}
	public class XlsCommandNote : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 8;
		int objId;
		CellPosition position = new CellPosition();
		XLUnicodeString author = new XLUnicodeString();
		#endregion
		#region Properties
		public CellPosition Position { get { return position; } set { position = value; } }
		public bool IsShown { get; set; }
		public bool IsRowHidden { get; set; }
		public bool IsColumnHidden { get; set; }
		public int ObjId {
			get { return objId; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ObjId");
				objId = value;
			}
		}
		public string Author {
			get { return author.Value; }
			set {
				if(string.IsNullOrEmpty(value))
					value = " ";
				ValueChecker.CheckValue(value.Length, 1, 54, "Author name length");
				this.author.Value = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int row = reader.ReadUInt16();
			int column = reader.ReadUInt16() & 0x00ff;
			this.position = new CellPosition(column, row);
			ushort bitwiseField = reader.ReadUInt16();
			IsShown = Convert.ToBoolean(bitwiseField & 0x0002);
			IsRowHidden = Convert.ToBoolean(bitwiseField & 0x0080);
			IsColumnHidden = Convert.ToBoolean(bitwiseField & 0x0100);
			this.objId = reader.ReadUInt16();
			this.author = XLUnicodeString.FromStream(reader);
			int bytesToRead = Size - (fixedPartSize + this.author.Length);
			if(bytesToRead > 0)
				reader.ReadBytes(bytesToRead); 
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsNoteInfo info = new XlsNoteInfo();
			info.Position = Position;
			info.IsShown = IsShown;
			info.IsRowHidden = IsRowHidden;
			info.IsColumnHidden = IsColumnHidden;
			info.ObjId = ObjId;
			info.Author = Author;
			contentBuilder.Notes.Add(info);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)position.Row);
			writer.Write((ushort)position.Column);
			ushort bitwiseField = 0;
			if(IsShown)
				bitwiseField |= 0x0002;
			if(IsRowHidden)
				bitwiseField |= 0x0080;
			if(IsColumnHidden)
				bitwiseField |= 0x0100;
			writer.Write(bitwiseField);
			writer.Write((ushort)objId);
			this.author.Write(writer);
			writer.Write((byte)0); 
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + this.author.Length + 1);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandNote();
		}
	}
}

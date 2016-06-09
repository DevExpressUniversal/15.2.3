#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
namespace DevExpress.Pdf.Native {
	public class PdfTextParser {
		readonly PdfRectangle pageCropBox;
		readonly List<PdfTextBlock> pageBlocks = new List<PdfTextBlock>();
		readonly Dictionary<PdfFont, PdfFontData> fontDataStorage = new Dictionary<PdfFont, PdfFontData>();
		PdfTextParserState parserState;
		public PdfTextParser(PdfRectangle pageCropBox) {
			this.pageCropBox = pageCropBox;
		}
		public void AddBlock(PdfStringData data, PdfFont font, double fontSize, double fontSizeFactor, double characterSpacing, double textWidthFactor, double textHeightFactor, double[] glyphOffsets, PdfPoint location, double angle) {
			byte[][] charCodes = data.CharCodes;
			if (charCodes != null && charCodes.Length > 0 && glyphOffsets != null && glyphOffsets.Length > 0) {
				PdfFontData fontData;
				if (!fontDataStorage.TryGetValue(font, out fontData)) {
					fontData = PdfFontData.CreateFontData(font);
					fontDataStorage.Add(font, fontData);
				}
				pageBlocks.Add(new PdfTextBlock(data, fontData, fontSize * textHeightFactor, characterSpacing, fontSizeFactor * textWidthFactor, fontSizeFactor * textHeightFactor, glyphOffsets, location, angle));
			}
		}
		public IList<PdfTextLine> Parse() {
			if (pageBlocks == null || pageBlocks.Count < 1)
				return new List<PdfTextLine>();
			parserState = new PdfTextParserState(pageBlocks, new PdfRectangle(0, 0, pageCropBox.Width, pageCropBox.Height));
			return new List<PdfTextLine>(new PdfPageTextLineBuilder(parserState));
		}
	}
}

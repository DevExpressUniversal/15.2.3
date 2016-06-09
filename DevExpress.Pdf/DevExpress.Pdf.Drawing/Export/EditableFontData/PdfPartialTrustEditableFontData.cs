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
using System.Drawing;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfPartialTrustEditableFontData : PdfNotEmbeddedEditableFontData {
		const float fontSize = 72f;
		static readonly StringFormat stringFormat;
		static readonly Graphics graphics;
		static PdfPartialTrustEditableFontData() {
			graphics = DevExpress.XtraPrinting.Native.GraphicsHelper.CreateGraphics();
			graphics.PageUnit = GraphicsUnit.Point;
			stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
			stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;
		}
		readonly Font font;
		readonly double emFactor;
		protected override bool ShouldCreateTTFFile { get { return false; } }
		public PdfPartialTrustEditableFontData(FontStyle fontStyle, string family)
			: base(fontStyle, family) {
			font = new Font(family, fontSize, fontStyle);
			emFactor = 1000 / fontSize;
		}
		protected override void FillFontDescriptor(PdfReaderDictionary fontDescriptorDictionary) {
			base.FillFontDescriptor(fontDescriptorDictionary);
			fontDescriptorDictionary.Add("ItalicAngle", 0);
			fontDescriptorDictionary.Add(PdfFontDescriptor.FlagsDictionaryKey, (int)PdfFontFlags.Nonsymbolic);
		}
		protected override IPdfGlyphMapper CreateMapper() {
			return new PdfPartialTrustGlyphMapper(this);
		}
		public override double GetCharWidth(char ch) {
			return graphics.MeasureString(ch.ToString(), font, new SizeF(int.MaxValue, 999999f), stringFormat).Width * emFactor;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				font.Dispose();
		}
	}
}

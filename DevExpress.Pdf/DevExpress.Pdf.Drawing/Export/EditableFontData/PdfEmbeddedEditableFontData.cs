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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfEmbeddedEditableFontData : PdfEditableFontData {
		public override bool Embedded { get { return true; } }
		protected override string BaseFont { get { return GetTrueTypeFontName(true); } }
		PdfCIDType2Font EmbeddedFont { get { return (PdfCIDType2Font)PdfFont; } }
		public PdfEmbeddedEditableFontData(FontStyle fontStyle, string fontFamily)
			: base(fontStyle, fontFamily) {
		}
		public override void UpdateFont() {
			base.UpdateFont();
			PdfCIDType2Font font = EmbeddedFont;
			font.SetFontFileData(FontFile.CreateSubset(CharacterCache));
			SortedDictionary<int, double> widths = new SortedDictionary<int, double>();
			foreach (int gi in CharacterCache.Mapping.Keys)
				widths.Add(gi, GetCharWidth((ushort)gi));
			font.Widths = widths;
		}
		public override PdfStringFormatter CreateStringFormatter(double fontSize) {
			return new PdfStringFormatter(this, fontSize);
		}
		protected override IPdfGlyphMapper CreateMapper() {
			return new PdfGDIGlyphMapper(FontFamily, FontStyle, FontFile);
		}
		protected override PdfFont CreateFont(PdfReaderDictionary fontDescriptor) {
			return new PdfCIDType2Font(BaseFont, fontDescriptor);
		}
	}
}

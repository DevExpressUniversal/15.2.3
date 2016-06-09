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

using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfTrueTypeFontValidator : PdfDisposableObject {
		internal static PdfTrueTypeFontValidationResult Validate(ITrueTypeFont font, byte[] fontFileData) {
			using (PdfTrueTypeFontValidator validator = new PdfTrueTypeFontValidator(font, fontFileData))
				return new PdfTrueTypeFontValidationResult(validator.file.GetData(), validator.glyphRanges, validator.glyphMapping);
		}
		readonly PdfFontFile file;
		readonly List<PdfFontCmapGlyphRange> glyphRanges;
		readonly IDictionary<string, ushort> glyphMapping;
		PdfTrueTypeFontValidator(ITrueTypeFont font, byte[] fontFileData) {
			file = new PdfFontFile(PdfFontFile.TTFVersion, fontFileData);
			PdfFontHeadTableDirectoryEntry headEntry = file.Head;
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			bool isSymbolic = fontDescriptor != null && (fontDescriptor.Flags & PdfFontFlags.Symbolic) == PdfFontFlags.Symbolic;
			PdfFontCmapSegmentMappingFormatEntry segmentMappingFormatEntry;
			PdfFontCmapTableDirectoryEntry cmapEntry = file.CMap;
			if (cmapEntry == null) {
				segmentMappingFormatEntry = new PdfFontCmapSegmentMappingFormatEntry(isSymbolic ? PdfFontEncodingID.Undefined : PdfFontEncodingID.UGL);
				cmapEntry = new PdfFontCmapTableDirectoryEntry(segmentMappingFormatEntry);
				file.AddTable(cmapEntry);
			}
			else
				segmentMappingFormatEntry = cmapEntry.Validate(font is PdfCIDType2Font, isSymbolic);
			PdfFontNameTableDirectoryEntry nameEntry = file.Name;
			if (nameEntry == null) {
				nameEntry = new PdfFontNameTableDirectoryEntry(cmapEntry, font.BaseFont);
				file.AddTable(nameEntry);
			}
			else 
				nameEntry.Create(cmapEntry, font.BaseFont);
			PdfFontOS2TableDirectoryEntry os2Entry = file.OS2;
			if (os2Entry != null && headEntry != null)
				os2Entry.Validate(font, headEntry.UnitsPerEm);
			if (segmentMappingFormatEntry != null) {
				glyphRanges = segmentMappingFormatEntry.GlyphRanges;
				PdfFontPostTableDirectoryEntry postEntry = file.Post;
				if (postEntry != null && cmapEntry.ShouldWrite)
					glyphMapping = segmentMappingFormatEntry.GetGlyphMapping(postEntry.GlyphNames);
			}
			else 
				glyphRanges = new List<PdfFontCmapGlyphRange>();
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				file.Dispose();
		}
	}
}

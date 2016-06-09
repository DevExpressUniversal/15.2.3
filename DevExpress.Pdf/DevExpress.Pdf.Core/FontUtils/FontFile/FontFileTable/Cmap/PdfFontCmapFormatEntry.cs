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
	public abstract class PdfFontCmapFormatEntry {
		public const int MissingGlyphIndex = 0;
		public static PdfFontCmapFormatEntry CreateEntry(PdfFontPlatformID platformId, PdfFontEncodingID encodingId, PdfFontCmapFormatID format, PdfBinaryStream stream) {
			switch (format) {
				case PdfFontCmapFormatID.ByteEncoding:
					return new PdfFontCmapByteEncodingFormatEntry(platformId, encodingId, stream);
				case PdfFontCmapFormatID.HighByteMappingThrough:
					return new PdfFontCmapHighByteMappingThroughFormatEntry(platformId, encodingId, stream);
				case PdfFontCmapFormatID.SegmentMapping:
					return new PdfFontCmapSegmentMappingFormatEntry(platformId, encodingId, stream);
				case PdfFontCmapFormatID.TrimmedMapping:
					return new PdfFontCmapTrimmedMappingFormatEntry(platformId, encodingId, stream);
				case PdfFontCmapFormatID.MixedCoverage:
					return new PdfFontCmapMixedCoverageFormatEntry(platformId, encodingId, stream);
				case PdfFontCmapFormatID.TrimmedArray:
					return new PdfFontCmapTrimmedArrayFormatEntry(platformId, encodingId, stream);
				case PdfFontCmapFormatID.SegmentedCoverage:
					return new PdfFontCmapSegmentedCoverageFormatEntry(platformId, encodingId, stream);
				case PdfFontCmapFormatID.ManyToOneRangeMapping:
					return new PdfFontCmapManyToOneRangeMappingFormatEntry(platformId, encodingId, stream);
				case PdfFontCmapFormatID.UnicodeVariationSequences:
					return new PdfFontCmapUnicodeVariationSequenciesFormatEntry(platformId, encodingId, stream);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		readonly PdfFontPlatformID platformId;
		readonly PdfFontEncodingID encodingId;
		public PdfFontPlatformID PlatformId { get { return platformId; } }
		public PdfFontEncodingID EncodingId { get { return encodingId; } }
		protected abstract PdfFontCmapFormatID Format { get; }
		public abstract int Length { get; }
		protected PdfFontCmapFormatEntry(PdfFontPlatformID platformId, PdfFontEncodingID encodingId) {
			this.platformId = platformId;
			this.encodingId = encodingId;
		}
		public virtual void Write(PdfBinaryStream tableStream) {
			tableStream.WriteShort((short)Format);
		}
		public virtual int MapCode(char character) {
			return character;
		}
	}
}

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

namespace DevExpress.Pdf.Native {
	public class DefaultUVSTable {
		readonly int startUnicodeValue;
		readonly byte additionalCount;
		public int StartUnicodeValue { get { return startUnicodeValue; } }
		public byte AdditionalCount { get { return additionalCount; } }
		public DefaultUVSTable(int startUnicodeValue, byte additionalCount) {
			this.startUnicodeValue = startUnicodeValue;
			this.additionalCount = additionalCount;
		}
		public void Write(PdfBinaryStream tableStream) {
			byte[] startUnicodeValueBytes = new byte[3];
			startUnicodeValueBytes[2] = (byte)(startUnicodeValue & 0xFF);
			startUnicodeValueBytes[1] = (byte)((startUnicodeValue & 0xFF00) >> 8);
			startUnicodeValueBytes[0] = (byte)((startUnicodeValue & 0xFF0000) >> 16);
			tableStream.WriteArray(startUnicodeValueBytes);
			tableStream.WriteByte(additionalCount);
		}
	}
	public class NonDefaultUVSTable {
		readonly int unicodeValue;
		readonly short glyphId;
		public int UnicodeValue { get { return unicodeValue; } }
		public short GlyphId { get { return glyphId; } }
		public NonDefaultUVSTable(int unicodeValue, short glyphId) {
			this.unicodeValue = unicodeValue;
			this.glyphId = glyphId;
		}
		public void Write(PdfBinaryStream tableStream) {
			byte[] unicodeValueBytes = new byte[3];
			unicodeValueBytes[2] = (byte)(unicodeValue & 0xFF);
			unicodeValueBytes[1] = (byte)((unicodeValue & 0xFF00) >> 8);
			unicodeValueBytes[0] = (byte)((unicodeValue & 0xFF0000) >> 16);
			tableStream.WriteArray(unicodeValueBytes);
			tableStream.WriteShort(glyphId);
		}
	}
	public class PdfFontCmapUnicodeVariationSelectorRecord {
		readonly int varSelector;
		readonly DefaultUVSTable[] defaultUVSTables;
		readonly NonDefaultUVSTable[] nonDefaultUVSTables;
		public int VarSelector { get { return varSelector; } }
		public DefaultUVSTable[] DefaultUVSTables { get { return defaultUVSTables; } }
		public NonDefaultUVSTable[] NonDefaultUVSTables { get { return nonDefaultUVSTables; } }
		public PdfFontCmapUnicodeVariationSelectorRecord(int varSelector, DefaultUVSTable[] defaultUVSTables, NonDefaultUVSTable[] nonDefaultUVSTables) {
			this.varSelector = varSelector;
			this.defaultUVSTables = defaultUVSTables;
			this.nonDefaultUVSTables = nonDefaultUVSTables;
		}
		public int Write(PdfBinaryStream tableStream, int offset) {
			const int defaultUVSTableSize = 4;
			const int nonDefaultUVSTableSize = 5;
			byte[] varSelectorBytes = new byte[3];
			varSelectorBytes[2] = (byte)(varSelector & 0xFF);
			varSelectorBytes[1] = (byte)((varSelector & 0xFF00) >> 8);
			varSelectorBytes[0] = (byte)((varSelector & 0xFF0000) >> 16);
			tableStream.WriteArray(varSelectorBytes);
			if (defaultUVSTables == null)
				tableStream.WriteInt(0);
			else {
				tableStream.WriteInt(offset);
				offset += (defaultUVSTableSize * defaultUVSTables.Length) + 4;
			}
			if (nonDefaultUVSTables == null)
				tableStream.WriteInt(0);
			else {
				tableStream.WriteInt(offset);
				offset += (nonDefaultUVSTableSize * nonDefaultUVSTables.Length) + 4;
			}
			return offset;
		}
	}
}

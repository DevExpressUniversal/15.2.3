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

using System.Text;
namespace DevExpress.Pdf.Native {
	public class PdfFontNameRecord {
		readonly PdfFontPlatformID platformID;
		readonly PdfFontLanguageID languageID;
		readonly PdfFontNameID nameID;
		readonly PdfFontEncodingID encodingID;
		readonly byte[] nameBytes;
		readonly string name;
		public PdfFontPlatformID PlatformID { get { return platformID; } }
		public PdfFontLanguageID LanguageID { get { return languageID; } }
		public PdfFontNameID NameID { get { return nameID; } }
		public PdfFontEncodingID EncodingID { get { return encodingID; } }
		public byte[] NameBytes { get { return nameBytes; } }
		public string Name { get { return name; } }
		public PdfFontNameRecord(PdfFontPlatformID platformID, PdfFontLanguageID languageID, PdfFontNameID nameID, PdfFontEncodingID encodingID, byte[] nameBytes) {
			this.platformID = platformID;
			this.languageID = languageID;
			this.nameID = nameID;
			this.encodingID = encodingID;
			this.nameBytes = nameBytes;
			this.name = platformID == PdfFontPlatformID.Microsoft ? Encoding.BigEndianUnicode.GetString(nameBytes) : Encoding.UTF8.GetString(nameBytes);
		}
		public PdfFontNameRecord(PdfBinaryStream stream, int dataOffset) {
			platformID = (PdfFontPlatformID)stream.ReadUshort();
			encodingID = (PdfFontEncodingID)stream.ReadUshort();
			languageID = (PdfFontLanguageID)stream.ReadUshort();
			nameID = (PdfFontNameID)stream.ReadUshort();
			int length = stream.ReadUshort();
			int offset = stream.ReadUshort();
			long newPosition = dataOffset + offset;
			if (newPosition + length <= stream.Length) {
				long oldPosition = stream.Position;
				stream.Position = newPosition;
				nameBytes = stream.ReadArray(length);
				name = platformID == PdfFontPlatformID.Microsoft ? Encoding.BigEndianUnicode.GetString(nameBytes) : Encoding.UTF8.GetString(nameBytes);
				stream.Position = oldPosition;
			}
		}
	}
}

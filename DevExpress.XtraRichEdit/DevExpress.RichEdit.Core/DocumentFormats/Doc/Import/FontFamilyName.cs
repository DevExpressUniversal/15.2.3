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
using System.Text;
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class DocFontFamilyName {
		const int ignoredDataSize = 34;
		public static DocFontFamilyName FromStream(BinaryReader reader) {
			DocFontFamilyName result = new DocFontFamilyName();
			result.Read(reader);
			return result;
		}
		#region Fields
		const byte defaultFontFamilyId = 0x04; 
		const int baseLength = 39;
		const int delimiterSize = 2;
		const short defaultWeight = 400;
		const byte defaultCharset = 1;
		byte totalLength;
		byte fontFamilyId;
		short baseWeight;
		byte charsetIdentifier;
		byte alternateFontNameStartIndex;
		string fontName;
		string alternateFontName;
		#endregion
		public DocFontFamilyName() {
			this.fontFamilyId = defaultFontFamilyId; 
			this.baseWeight = defaultWeight;
			this.charsetIdentifier = defaultCharset;
			this.alternateFontNameStartIndex = 0;
		}
		#region Properties
		public string FontName {
			get { return fontName; }
			protected internal set { fontName = StringHelper.PrepareFontNameForDoc(value); }
		}
		public string AlternateFontName { get { return alternateFontName; } }
		public byte Charset {
			get { return charsetIdentifier; }
			protected internal set { charsetIdentifier = value; }
		}
		#endregion
		protected void Read(BinaryReader reader) {
			Guard.ArgumentNotNull(reader, "reader");
			this.totalLength = reader.ReadByte();
			this.fontFamilyId = reader.ReadByte();
			this.baseWeight = reader.ReadInt16();
			this.charsetIdentifier = reader.ReadByte();
			this.alternateFontNameStartIndex = reader.ReadByte();
			reader.BaseStream.Seek(ignoredDataSize, SeekOrigin.Current);
			byte[] buffer;
			string fontName;
			string alternateFontName;
			if (this.alternateFontNameStartIndex == 0) {
				buffer = reader.ReadBytes(this.totalLength - baseLength - delimiterSize); 
				fontName = Encoding.Unicode.GetString(buffer, 0, buffer.Length);
				alternateFontName = string.Empty;
			}
			else {
				buffer = reader.ReadBytes((this.alternateFontNameStartIndex - 1) * 2);
				fontName = Encoding.Unicode.GetString(buffer, 0, buffer.Length);
				reader.ReadInt16(); 
				buffer = reader.ReadBytes(this.totalLength - baseLength - (this.alternateFontNameStartIndex * 2) - delimiterSize);
				alternateFontName = Encoding.Unicode.GetString(buffer, 0, buffer.Length);
			}
			this.fontName =  StringHelper.PrepareFontNameForDoc(fontName);
			this.alternateFontName = StringHelper.PrepareFontNameForDoc(alternateFontName);
			reader.ReadInt16(); 
		}
		public void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			writer.Write(CalcTotalLength());
			writer.Write(this.fontFamilyId);
			writer.Write(this.baseWeight);
			writer.Write(this.charsetIdentifier);
			writer.Write(this.alternateFontNameStartIndex);
			writer.BaseStream.Seek(ignoredDataSize, SeekOrigin.Current);
			writer.Write(Encoding.Unicode.GetBytes(this.fontName));
			writer.Write((short)0);
		}
		byte CalcTotalLength() {
			return (byte)(baseLength + this.fontName.Length * 2 + 2); 
		}
	}
}

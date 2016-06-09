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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.Pdf.Common {
	class TTFName : TTFTable {
		#region inner classes
		struct Record {
			public ushort PlatformID;
			public ushort EncodingID;
			public ushort LanguageID;
			public ushort NameID;
			public ushort StringLength;
			public ushort StringOffset;
			public static int SizeOf { get { return TTFStream.SizeOf_UShort * 6; } }
		}
		#endregion
		const int stringsCount = 8;
		const int familyNameID = 1;
		const int postScriptNameID = 6;
		const int MacintoshPlatformID = 1;
		const int MicrosoftPlatformID = 3;
		public const int EnglishLanguageID = 1033;
		int startPosition;
		ushort numberOfRecords;
		ushort offsetOfStringStorage;
		Record[] records;
		string familyName;
		string macFamilyName;
		string postScriptName;
		public override int Length {
			get {
				int stringsLength = 0;
				stringsLength += familyName.Length * 2;
				stringsLength += macFamilyName.Length + postScriptName.Length;
				return
					TTFStream.SizeOf_UShort * 3 +
					Record.SizeOf * this.records.Length +
					stringsLength;
			}
		}
		public string FamilyName { get { return familyName; } }
		public string MacintoshFamilyName { get { return macFamilyName; } }
		public string PostScriptName { get { return postScriptName; } }
		protected internal override string Tag { get { return "name"; } }
		public TTFName(TTFFile ttfFile)
			: base(ttfFile) {
		}
		void ReadStringStorage(TTFStream ttfStream) {
			for(int i = 0; i < records.Length; i++) {
				if(records[i].PlatformID == MicrosoftPlatformID && records[i].EncodingID == 1 && records[i].LanguageID == EnglishLanguageID) {
					if(records[i].NameID == familyNameID)
						familyName = ReadUnicodeString(ttfStream, i);
				} else if(records[i].PlatformID == MacintoshPlatformID && records[i].EncodingID == 0 && records[i].LanguageID == 0) {
					switch(records[i].NameID) {
						case familyNameID:
							macFamilyName = ReadString(ttfStream, i);
							break;
						case postScriptNameID:
							postScriptName = ReadString(ttfStream, i);
							break;
					}
				}
			}
		}
		string ReadUnicodeString(TTFStream ttfStream, int i) {
			ttfStream.Seek(startPosition);
			ttfStream.Move(offsetOfStringStorage + records[i].StringOffset);
			return ttfStream.ReadUnicodeString(records[i].StringLength);
		}
		string ReadString(TTFStream ttfStream, int i) {
			ttfStream.Seek(startPosition);
			ttfStream.Move(offsetOfStringStorage + records[i].StringOffset);
			byte[] bytes = ttfStream.ReadBytes(records[i].StringLength);
			return DXEncoding.ASCII.GetString(bytes, 0, bytes.Length);
		}
		void ReadRecords(TTFStream ttfStream) {
			records = new Record[numberOfRecords];
			for(int i = 0; i < numberOfRecords; i++) {
				records[i].PlatformID = ttfStream.ReadUShort();
				records[i].EncodingID = ttfStream.ReadUShort();
				records[i].LanguageID = ttfStream.ReadUShort();
				records[i].NameID = ttfStream.ReadUShort();
				records[i].StringLength = ttfStream.ReadUShort();
				records[i].StringOffset = ttfStream.ReadUShort();
			}
		}
		protected override void ReadTable(TTFStream ttfStream) {
			startPosition = ttfStream.Position;
			ttfStream.Move(TTFStream.SizeOf_UShort);
			numberOfRecords = ttfStream.ReadUShort();
			offsetOfStringStorage = ttfStream.ReadUShort();
			ReadRecords(ttfStream);
			ReadStringStorage(ttfStream);
		}
		protected override void WriteTable(TTFStream ttfStream) {
			throw new TTFFileException("Not supported");
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			throw new TTFFileException("Not supported");
		}
	}
}

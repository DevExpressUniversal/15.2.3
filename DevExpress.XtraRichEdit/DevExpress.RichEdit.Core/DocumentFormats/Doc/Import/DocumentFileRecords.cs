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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.Doc {
	public class DocumentFileRecords {
		enum RecordType {
			FrameSetRoot = 0x00,
			Frame = 0x01,
			FrameChildMarker = 0x02,
			FrameName = 0x03,
			FrameFilePath = 0x04,
			FrameBorderAttributes = 0x05,
			ListStyles = 0x06
		}
		List<ListStylesRecordItem> listStyles;
		public static DocumentFileRecords FromStream(BinaryReader reader, int offset, int size) {
			DocumentFileRecords result = new DocumentFileRecords();
			result.Read(reader, offset, size);
			return result;
		}
		public List<ListStylesRecordItem> ListStyles { get { return listStyles; } }
		public DocumentFileRecords() {
			listStyles = new List<ListStylesRecordItem>();
		}
		void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size == 0 || reader.BaseStream.Length < Math.Abs(offset)) return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			while (size > 0) {
				int recordSize = ReadDocumentFileRecord(reader, offset);
				size -= recordSize;
				offset += recordSize;
			}
		}
		int ReadDocumentFileRecord(BinaryReader reader, int offset) {
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			int size = reader.ReadInt32();
			RecordType type = (RecordType)reader.ReadInt32();
			if (type == RecordType.ListStyles)
				ReadListStylesRecord(reader);
			return size;
		}
		void ReadListStylesRecord(BinaryReader reader) {
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				int listIndex = reader.ReadInt16();
				int tmp = reader.ReadInt16();
				int styleIndex = (tmp & 0x0FFF);
				bool styleDefinition = (tmp & 0x1000) != 0;
				listStyles.Add(new ListStylesRecordItem(listIndex, styleIndex, styleDefinition));
			}
		}
		public void Write(BinaryWriter writer) {
			byte[] listStyles = ExportListStyles();
			writer.Write(listStyles.Length + 4);
			writer.Write(listStyles, 0, listStyles.Length);
		}
		byte[] ExportListStyles() {
			int count = listStyles.Count;
			MemoryStream result = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(result);
			writer.Write((int)RecordType.ListStyles);
			writer.Write(count);
			for (int i = 0; i < count; i++) {
				ListStylesRecordItem item = listStyles[i];
				writer.Write((short)item.ListIndex);
				short secondByte = (short)((item.StyleIndex) & 0x0FFF);
				if (item.StyleDefinition)
					secondByte |= 0x1000;
				writer.Write(secondByte);
			}
			writer.Flush();
			return result.ToArray();
		}
	}
	public class ListStylesRecordItem {
		public ListStylesRecordItem(int listIndex, int styleIndex, bool styleDefinition) {
			ListIndex = listIndex;
			StyleIndex = styleIndex;
			StyleDefinition = styleDefinition;
		}
		public int ListIndex { get; private set; }
		public int StyleIndex { get; private set; }
		public bool StyleDefinition { get; private set; }
	}
}

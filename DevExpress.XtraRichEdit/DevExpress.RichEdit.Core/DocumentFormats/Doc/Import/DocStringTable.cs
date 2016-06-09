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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocStringTableBase
	public abstract class DocStringTableBase {
		internal const ushort ExtendedTypeCode = 0xffff;
		public bool IsExtended { get; protected internal set; }
		public Encoding Encoding { get; protected internal set; }
		public int Count { get; protected internal set; }
		public int ExtraDataSize { get; protected internal set; }
		protected internal virtual void Read(BinaryReader reader, int offset, int size) {
			Guard.ArgumentNotNull(reader, "reader");
			if (size == 0 || reader.BaseStream.Length < Math.Abs(offset)) return;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			IsExtended = CalcIsExtended(reader);
			Encoding = GetEncoding();
			Count = CalcRecordsCount(reader);
			ExtraDataSize = CalcExtraDataSize(reader);
			ReadCore(reader);
		}
		protected internal virtual void ReadCore(BinaryReader reader) {
			for (int i = 0; i < Count; i++) {
				ReadString(reader);
				ReadExtraData(reader);
			}
		}
		protected virtual bool CalcIsExtended(BinaryReader reader) {
			return reader.ReadUInt16() == ExtendedTypeCode;
		}
		protected virtual Encoding GetEncoding() {
			return Encoding.Unicode;
		}
		protected internal virtual int CalcRecordsCount(BinaryReader reader) {
			return reader.ReadInt16();
		}
		protected internal virtual int CalcExtraDataSize(BinaryReader reader) {
			return reader.ReadInt16();
		}
		protected internal virtual void Write(BinaryWriter writer) {
			Guard.ArgumentNotNull(writer, "writer");
			WriteIsExtended(writer);
			WriteCount(writer);
			WriteExtraDataSize(writer);
			WriteCore(writer);
		}
		protected virtual void WriteCore(BinaryWriter writer){
			for (int i = 0; i < Count; i++) {
				WriteString(writer, i);
				WriteExtraData(writer, i);
			}
		}
		protected virtual void WriteIsExtended(BinaryWriter writer) {
			writer.Write(ExtendedTypeCode);
		}
		protected virtual void WriteCount(BinaryWriter writer) {
			writer.Write((short)Count);
		}
		protected virtual void WriteExtraDataSize(BinaryWriter writer) {
			writer.Write((short)ExtraDataSize);
		}
		protected virtual void ReadString(BinaryReader reader) {
		}
		protected virtual void ReadExtraData(BinaryReader reader) {
		}
		protected virtual void WriteString(BinaryWriter writer, int index) {
		}
		protected virtual void WriteExtraData(BinaryWriter writer, int index) {
			byte[] extraData = new byte[ExtraDataSize];
			writer.Write(extraData);
		}
	}
	#endregion
	#region DocStringTable
	public class DocStringTable : DocStringTableBase {
		#region static
		public static DocStringTable FromStream(BinaryReader reader, int offset, int size) {
			DocStringTable result = new DocStringTable();
			result.Read(reader, offset, size);
			return result;
		}
		#endregion
		public bool ShouldWriteEmptyTable { get; protected internal set; }
		public DocStringTable() {
			Data = new List<string>();
		}
		public DocStringTable(List<string> data) {
			Data = data;
		}
		public List<string> Data { get; private set; }
		protected internal override void Write(BinaryWriter writer) {
			Count = Data.Count;
			if (Count > 0 || ShouldWriteEmptyTable)
				base.Write(writer);
		}
		protected override bool CalcIsExtended(BinaryReader reader) {
			bool result = (reader.ReadUInt16() == ExtendedTypeCode);
			if (!result)
				reader.BaseStream.Seek(-2, SeekOrigin.Current);
			return result;
		}
		protected override Encoding GetEncoding() {
			return IsExtended ? Encoding.Unicode : DXEncoding.ASCII;
		}
		protected override void ReadString(BinaryReader reader) {
			int length = IsExtended ? reader.ReadInt16() * 2 : reader.ReadByte();
			byte[] buffer = reader.ReadBytes(length);
			string result = Encoding.GetString(buffer, 0, buffer.Length);
			Data.Add(StringHelper.RemoveSpecialSymbols(result));
		}
		protected override void ReadExtraData(BinaryReader reader) {
			reader.BaseStream.Seek(ExtraDataSize, SeekOrigin.Current);
		}
		protected override void WriteString(BinaryWriter writer, int index) {
			writer.Write((short)Data[index].Length);
			writer.Write(Encoding.Unicode.GetBytes(Data[index]));
		}
	}
	#endregion
	public class RmdThreading {
		DocStringTable sttbMessage = new DocStringTable() { ShouldWriteEmptyTable = true };
		DocStringTable sttbStyle = new DocStringTable() { ShouldWriteEmptyTable = true };
		DocStringTable sttbAuthorAttrib = new DocStringTable() { ShouldWriteEmptyTable = true };
		DocStringTable sttbAuthorValue = new DocStringTable() { ShouldWriteEmptyTable = true };
		DocStringTable sttbMessageAttrib = new DocStringTable() { ShouldWriteEmptyTable = true };
		DocStringTable sttbMessageValue = new DocStringTable() { ShouldWriteEmptyTable = true };
		protected internal virtual void Write(BinaryWriter writer) {
			this.sttbMessage.ExtraDataSize = 8;
			this.sttbMessage.Write(writer);
			this.sttbStyle.Write(writer);
			this.sttbAuthorAttrib.ExtraDataSize = 2;
			this.sttbAuthorAttrib.Write(writer);
			this.sttbAuthorValue.Write(writer);
			this.sttbMessageAttrib.ExtraDataSize = 2;
			this.sttbMessageAttrib.Write(writer);
			this.sttbAuthorValue.Write(writer);
		}
	}
}

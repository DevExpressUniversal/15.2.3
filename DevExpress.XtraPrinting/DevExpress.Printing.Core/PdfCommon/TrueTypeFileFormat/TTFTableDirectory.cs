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
using System.Collections;
using System.Text;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Pdf.Common {
	class TTFTableDirectory {
		public static int SizeOf {
			get {
				return
					TTFStream.SizeOf_Fixed +
					TTFStream.SizeOf_UShort * 4;
			}
		}
		TTFFile owner;
		byte[] sfntVersion;
		ushort numTables;
		ushort searchRange;
		ushort entrySelector;
		ushort rangeShift;
		List<TTFTableDirectoryEntry> entries = new List<TTFTableDirectoryEntry>();
		int NumTables { get { return Convert.ToInt32(numTables); } }
		public TTFFile Owner { get { return owner; } }
		public int Count { get { return entries.Count; } }
		public TTFTableDirectoryEntry this[int index] { get { return entries[index]; } }
		public TTFTableDirectoryEntry this[string tag] {
			get {
				for(int i = 0; i < Count; i++)
					if(this[i].Tag == tag)
						return this[i];
				return null;
			}
		}
		public TTFTableDirectory(TTFFile owner) {
			this.owner = owner;
		}
		void ReadEntries(TTFStream ttfStream) {
			entries.Clear();
			for(int i = 0; i < NumTables; i++) {
				TTFTableDirectoryEntry entry = new TTFTableDirectoryEntry();
				entry.Read(ttfStream);
				entries.Add(entry);
			}
		}
		void WriteEntries(TTFStream ttfStream) {
			for(int i = 0; i < Count; i++)
				this[i].Write(ttfStream);
		}
		public void Read(TTFStream ttfStream) {
			ttfStream.Seek((int)owner.Offset);
			sfntVersion = ttfStream.ReadBytes(TTFStream.SizeOf_Fixed);
			numTables = ttfStream.ReadUShort();
			searchRange = ttfStream.ReadUShort();
			entrySelector = ttfStream.ReadUShort();
			rangeShift = ttfStream.ReadUShort();
			ReadEntries(ttfStream);
		}
		public void Write(TTFStream ttfStream) {
			ttfStream.Seek(0);
			ttfStream.WriteBytes(sfntVersion);
			ttfStream.WriteUShort(numTables);
			ttfStream.WriteUShort(searchRange);
			ttfStream.WriteUShort(entrySelector);
			ttfStream.WriteUShort(rangeShift);
			WriteEntries(ttfStream);
		}
		public void Register(TTFTable table) {
			TTFTableDirectoryEntry entry = new TTFTableDirectoryEntry();
			entry.Initialize(table);
			entries.Add(entry);
		}
		public void WriteOffsets(TTFStream ttfStream) {
			ttfStream.Seek(0);
			ttfStream.Move(TTFTableDirectory.SizeOf);
			for(int i = 0; i < Count; i++)
				this[i].WriteOffset(ttfStream);
		}
		public void WriteCheckSum(TTFStream ttfStream) {
			ttfStream.Seek(0);
			ttfStream.Move(TTFTableDirectory.SizeOf);
			for(int i = 0; i < Count; i++)
				this[i].WriteCheckSum(ttfStream);
		}
		public void Initialize(TTFTableDirectory pattern) {
			sfntVersion = new byte[pattern.sfntVersion.Length];
			pattern.sfntVersion.CopyTo(sfntVersion, 0);
			numTables = (ushort)Count;
			double power = Math.Floor(Math.Log(numTables, 2));
			double x = Math.Pow(2, power);
			searchRange = (ushort)(x * 16);
			entrySelector = (ushort)power;
			rangeShift = (ushort)(numTables * 16 - searchRange);
		}
	}
	class TTFTableDirectoryEntry {
		public static int SizeOf { get { return TTFStream.SizeOf_ULong * 4; } }
		byte[] tag;
		uint checkSum;
		uint offset;
		uint length;
		public string Tag {
			get {
				if(tag == null) return null;
				string result = "";
				for(int i = 0; i < tag.Length; i++)
					result += Convert.ToChar(tag[i]);
				return result;
			}
		}
		public int Offset { get { return Convert.ToInt32(offset); } }
		public int Length { get { return Convert.ToInt32(length); } }
		public uint CheckSum { get { return checkSum; } }
		public void Read(TTFStream ttfStream) {
			tag = ttfStream.ReadBytes(TTFStream.SizeOf_ULong);
			checkSum = ttfStream.ReadULong();
			offset = ttfStream.ReadULong();
			length = ttfStream.ReadULong();
		}
		public void Write(TTFStream ttfStream) {
			ttfStream.WriteBytes(tag);
			ttfStream.WriteULong(checkSum);
			ttfStream.WriteULong(offset);
			ttfStream.WriteULong(length);
		}
		public void WriteOffset(TTFStream ttfStream) {
			ttfStream.Move(TTFStream.SizeOf_ULong * 2);
			ttfStream.WriteULong(offset);
			ttfStream.Move(TTFStream.SizeOf_ULong);
		}
		public void WriteCheckSum(TTFStream ttfStream) {
			if(checkSum == 0)
				checkSum = TTFUtils.CalculateCheckSum(ttfStream, Offset, Length);
			ttfStream.Move(TTFStream.SizeOf_ULong);
			ttfStream.WriteULong(checkSum);
			ttfStream.Move(TTFStream.SizeOf_ULong * 2);
		}
		public void Initialize(TTFTable table) {
			if(table.Tag.Length != 4) return;
			tag = new byte[4];
			for(int i = 0; i < 4; i++)
				tag[i] = Convert.ToByte(table.Tag[i]);
			checkSum = 0;
			offset = 0;
			length = (uint)table.Length;
		}
		public void InitializeOffset(int offset) {
			if(this.offset == 0)
				this.offset = (uint)offset;
		}
	}
}

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
using System.IO;
using System;
namespace DevExpress.Pdf.Native {
	public class PdfBinaryStream : PdfDisposableObject {
		readonly MemoryStream stream;
		public byte[] Data { get { return stream.ToArray(); } }
		public long Length { get { return stream.Length; } }
		public long Position { 
			get { return stream.Position; }
			set { stream.Position = value; }
		}
		public PdfBinaryStream() {
			stream = new MemoryStream();
		}
		public PdfBinaryStream(byte[] data) {
			stream = new MemoryStream(data);
		}
		public PdfBinaryStream(int length) {
			stream = new MemoryStream(length);
		}
		public byte ReadByte() {
			return (byte)stream.ReadByte();
		}
		public short ReadShort() {
			return (short)((stream.ReadByte() << 8) + stream.ReadByte());
		}
		public int ReadUshort() {
			return (stream.ReadByte() << 8) + stream.ReadByte();
		}
		public int ReadInt() {
			return (stream.ReadByte() << 24) + (stream.ReadByte() << 16) + (stream.ReadByte() << 8) + stream.ReadByte();
		}
		public long ReadLong() {
			return (long)(stream.ReadByte() << 56) + (stream.ReadByte() << 48) + (stream.ReadByte() << 40) + (stream.ReadByte() << 32) + 
						 (stream.ReadByte() << 24) + (stream.ReadByte() << 16) + (stream.ReadByte() << 8) + stream.ReadByte();
		}
		public int ReadOffSet(int length) {
			switch (length) {
				case 2:
					return ReadUshort();
				case 3:
					return (ReadByte() << 16) + (ReadByte() << 8) + ReadByte();
				case 4:
					return ReadInt();
				default:
					return ReadByte();
			}
		}
		public byte[] ReadArray(int length) {
			byte[] array = new byte[length];
			stream.Read(array, 0, length);
			return array;
		}
		public short[] ReadShortArray(int length) {
			short[] array = new short[length];
			for (int i = 0; i < length; i++)
				array[i] = ReadShort();
			return array;
		}
		public string ReadString(int length) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < length; i++)
				sb.Append((char)stream.ReadByte());
			return sb.ToString();
		}
		public float ReadFixed() {
			return ReadInt() / 65536f;
		}
		public void WriteFixed(float value) {
			WriteInt((int)(value * 65536f));
		}
		public void WriteByte(byte value) {
			stream.WriteByte(value);
		}
		public void WriteShort(short value) {
			stream.WriteByte((byte)((value & 0xFF00) >> 8));
			stream.WriteByte((byte)(value & 0xFF));
		}
		public void WriteInt(int value) {
			stream.WriteByte((byte)((value & 0xFF000000) >> 24));
			stream.WriteByte((byte)((value & 0xFF0000) >> 16));
			stream.WriteByte((byte)((value & 0xFF00) >> 8));
			stream.WriteByte((byte)(value & 0xFF));
		}
		public void WriteLong(long value) {
			stream.WriteByte((byte)(((ulong)value & 0xFF00000000000000) >> 56));
			stream.WriteByte((byte)((value & 0xFF000000000000) >> 48));
			stream.WriteByte((byte)((value & 0xFF0000000000) >> 40));
			stream.WriteByte((byte)((value & 0xFF00000000) >> 32));
			stream.WriteByte((byte)((value & 0xFF000000) >> 24));
			stream.WriteByte((byte)((value & 0xFF0000) >> 16));
			stream.WriteByte((byte)((value & 0xFF00) >> 8));
			stream.WriteByte((byte)(value & 0xFF));
		}
		public void WriteArray(byte[] array) {
			stream.Write(array, 0, array.Length);
		}
		public void WriteShortArray(short[] array) {
			foreach (short value in array) 
				WriteShort(value);
		}
		public void WriteString(string str) {
			foreach (char c in str) 
				stream.WriteByte((byte)c);
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				stream.Dispose();
		}
	}
}

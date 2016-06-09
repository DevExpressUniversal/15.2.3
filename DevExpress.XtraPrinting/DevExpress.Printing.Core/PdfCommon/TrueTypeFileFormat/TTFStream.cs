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
using System.IO;
using System.Text;
namespace DevExpress.Pdf.Common {
	abstract class TTFStream {
		protected const string positionError = "error when working with .ttf file";
		public const int SizeOf_Byte = 1;
		public const int SizeOf_Char = 1;
		public const int SizeOf_UShort = 2;
		public const int SizeOf_Short = 2;
		public const int SizeOf_ULong = 4;
		public const int SizeOf_Long = 4;
		public const int SizeOf_Fixed = 4;
		public const int SizeOf_FWord = 2;
		public const int SizeOf_UFWord = 2;
		public const int SizeOf_F2Dot14 = 2;
		public const int SizeOf_InternationalDate = 8;
		public const int SizeOf_PANOSE = 10;
		public static float FixedToFloat(byte[] value) {
			if(value.Length != TTFStream.SizeOf_Fixed) return 0;
			byte[] temp = new byte[TTFStream.SizeOf_Fixed];
			if(BitConverter.IsLittleEndian) {
				for(int i = 0; i < TTFStream.SizeOf_Fixed; i++)
					temp[i] = value[TTFStream.SizeOf_Fixed - i - 1];
			} else {
				for(int i = 0; i < TTFStream.SizeOf_Fixed; i++)
					temp[i] = value[i];
			}
			short aliquot = BitConverter.ToInt16(temp, 2);
			ushort fraction = BitConverter.ToUInt16(temp, 0);
			if(fraction != 0) {
				double denominator = Math.Pow(10, Math.Ceiling(Math.Log10(fraction)));
				return Convert.ToSingle(aliquot + ((double)fraction / denominator) * Math.Sign(aliquot));
			} else
				return Convert.ToSingle(aliquot);
		}
		public abstract int Position { get; }
		public abstract int Length { get; }
		protected abstract byte _read();
		protected abstract void _write(byte value);
		protected abstract void _seek(int newPosition);
		public byte ReadByte() {
			if(Position < 0 || Position >= Length)
				throw new TTFFileException(positionError);
			return _read();
		}
		public sbyte ReadChar() {
			return Convert.ToSByte(ReadByte());
		}
		public ushort ReadUShort() {
			if(Position < 0 || Position >= (Length - 1))
				throw new TTFFileException(positionError);
			if(BitConverter.IsLittleEndian)
				return (ushort)((_read() << 8) + _read());
			else
				return (ushort)(_read() + (_read() << 8));
		}
		public short ReadShort() {
			if(Position < 0 || Position >= (Length - 1))
				throw new TTFFileException(positionError);
			if(BitConverter.IsLittleEndian)
				return (short)((_read() << 8) + _read());
			else
				return (short)(_read() + (_read() << 8));
		}
		public uint ReadULong() {
			if(Position < 0 || Position >= (Length - 3))
				throw new TTFFileException(positionError);
			if(BitConverter.IsLittleEndian) {
				return (uint)((_read() << 24) + (_read() << 16) + (_read() << 8) + _read());
			} else
				return (uint)(_read() + (_read() << 8) + (_read() << 16) + (_read() << 24));
		}
		public int ReadLong() {
			if(Position < 0 || Position >= (Length - 3))
				throw new TTFFileException(positionError);
			if(BitConverter.IsLittleEndian) {
				return (int)((_read() << 24) + (_read() << 16) + (_read() << 8) + _read());
			} else
				return (int)(_read() + (_read() << 8) + (_read() << 16) + (_read() << 24));
		}
		public ushort ReadUFWord() {
			return ReadUShort();
		}
		public short ReadFWord() {
			return ReadShort();
		}
		public TTFPanose ReadPanose() {
			if(Position < 0 || Position >= (Length - 9))
				throw new TTFFileException(positionError);
			TTFPanose result = new TTFPanose();
			result.bFamilyType = _read();
			result.bSerifType = _read();
			result.bWeight = _read();
			result.bProportion = _read();
			result.bContrast = _read();
			result.bStrokeVariation = _read();
			result.bArmStyle = _read();
			result.bLetterForm = _read();
			result.bMidline = _read();
			result.bXHeight = _read();
			return result;
		}
		public byte[] ReadF2Dot14() {
			if(Position < 0 || Position >= (Length - 1))
				throw new TTFFileException(positionError);
			byte[] result = new byte[2];
			result[0] = _read();
			result[1] = _read();
			return result;
		}
		public byte[] ReadBytes(int count) {
			if(Position < 0 || Position >= (Length - count + 1))
				throw new TTFFileException(positionError);
			byte[] result = new byte[count];
			for(int i = 0; i < count; i++)
				result[i] = _read();
			return result;
		}
		public string ReadUnicodeString(int lengthInBytes) {
			string result = "";
			for(int i = 0; i < lengthInBytes; i += 2)
				result += (char)ReadUShort();
			return result;
		}
		public void WriteBytes(byte[] buffer) {
			for(int i = 0; i < buffer.Length; i++)
				_write(buffer[i]);
		}
		public void WriteBytes(byte[] buffer, bool reverse) {
			if(reverse) {
				for(int i = buffer.Length - 1; i >= 0; i--)
					_write(buffer[i]);
			} else
				WriteBytes(buffer);
		}
		public void WriteByte(byte value) {
			_write(value);
		}
		public void WriteChar(sbyte value) {
			_write((byte)value);
		}
		public void WriteUShort(ushort value) {
			byte[] buffer = BitConverter.GetBytes(value);
			WriteBytes(buffer, BitConverter.IsLittleEndian);
		}
		public void WriteShort(short value) {
			byte[] buffer = BitConverter.GetBytes(value);
			WriteBytes(buffer, BitConverter.IsLittleEndian);
		}
		public void WriteULong(uint value) {
			byte[] buffer = BitConverter.GetBytes(value);
			WriteBytes(buffer, BitConverter.IsLittleEndian);
		}
		public void WriteLong(int value) {
			byte[] buffer = BitConverter.GetBytes(value);
			WriteBytes(buffer, BitConverter.IsLittleEndian);
		}
		public void WriteUFWord(ushort value) {
			WriteUShort(value);
		}
		public void WriteFWord(short value) {
			WriteShort(value);
		}
		public void WritePanose(TTFPanose value) {
			_write(value.bFamilyType);
			_write(value.bSerifType);
			_write(value.bWeight);
			_write(value.bProportion);
			_write(value.bContrast);
			_write(value.bStrokeVariation);
			_write(value.bArmStyle);
			_write(value.bLetterForm);
			_write(value.bMidline);
			_write(value.bXHeight);
		}
		public void WriteUnicodeString(string value) {
			for(int i = 0; i < value.Length; i++)
				WriteUShort((ushort)value[i]);
		}
		public void Pad4() {
			int padSize = Position % 4;
			for(int i = 4; i > padSize; i--)
				_write(0);
		}
		public void Move(int offset) {
			Seek(Position + offset);
		}
		public void Seek(int newPosition) {
			if(newPosition < 0 || newPosition > Length)
				throw new TTFFileException(positionError);
			_seek(newPosition);
		}
	}
	class TTFStreamAsStream : TTFStream {
		Stream stream;
		public override int Position { get { return (int)stream.Position; } }
		public override int Length { get { return (int)stream.Length; } }
		public TTFStreamAsStream(Stream stream) {
			this.stream = stream;
		}
		protected override byte _read() {
			return (byte)stream.ReadByte();
		}
		protected override void _write(byte value) {
			stream.WriteByte(value);
		}
		protected override void _seek(int newPosition) {
			stream.Seek(newPosition, SeekOrigin.Begin);
		}
	}
	class TTFStreamAsByteArray : TTFStream {
		byte[] data;
		int position;
		public override int Position { get { return position; } }
		public override int Length { get { return data.Length; } }
		public TTFStreamAsByteArray(byte[] data) {
			this.data = data;
		}
		protected override byte _read() {
			return data[position++];
		}
		protected override void _write(byte value) {
			data[position++] = value;
		}
		protected override void _seek(int newPosition) {
			position = newPosition;
		}
	}
}

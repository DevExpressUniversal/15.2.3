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
using System.IO;
namespace DevExpress.Utils.StructuredStorage.Internal.Reader {
	#region InputHandler
	[CLSCompliant(false)]
	public class InputHandler : AbstractIOHandler {
		const int HeaderSector = -1;
		public InputHandler(Stream stream) : base(stream) {
		}
		public override UInt64 IOStreamSize { get { return (UInt64)Stream.Length; } }
		internal long SeekToSector(long sector) {
			if (Header == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			if (sector < 0)
				throw new ArgumentOutOfRangeException("sector");
			if (sector == HeaderSector)
				return Stream.Seek(0, SeekOrigin.Begin);
			return Stream.Seek((sector << Header.SectorShift) + Measures.HeaderSize, SeekOrigin.Begin);
		}
		internal long SeekToPositionInSector(long sector, long position) {
			if (Header == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			if (position < 0 || position >= Header.SectorSize)
				throw new ArgumentOutOfRangeException("position");
			if (sector == HeaderSector)
				return Stream.Seek(position, SeekOrigin.Begin);
			return Stream.Seek((sector << Header.SectorShift) + Measures.HeaderSize + position, SeekOrigin.Begin);
		}
		internal byte ReadByte() {
			int result = Stream.ReadByte();
			if (result == -1)
				ThrowReadBytesAmountMismatchException();
			return (byte)result;
		}
		internal void Read(byte[] array) {
			Read(array, 0, array.Length);
		}
		internal void Read(byte[] array, int offset, int count) {
			int result = Stream.Read(array, offset, count);
			if (result != count)
				ThrowReadBytesAmountMismatchException();
		}
		internal int UncheckedRead(byte[] array, int offset, int count) {
			return Stream.Read(array, offset, count);
		}
		internal void ReadPosition(byte[] array, long position) {
			if (position < 0)
				throw new ArgumentOutOfRangeException("position");
			Stream.Seek(position, 0);
			int result = Stream.Read(array, 0, array.Length);
			if (result != array.Length)
				ThrowReadBytesAmountMismatchException();
		}
		internal UInt16 ReadUInt16() {
			if (BitConverter == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			byte[] array = new byte[2];
			Read(array);
			return BitConverter.ToUInt16(array);
		}
		internal UInt32 ReadUInt32() {
			if (BitConverter == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			byte[] array = new byte[4];
			Read(array);
			return BitConverter.ToUInt32(array);
		}
		internal UInt64 ReadUInt64() {
			if (BitConverter == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			byte[] array = new byte[8];
			Read(array);
			return BitConverter.ToUInt64(array);
		}
		internal UInt16 ReadUInt16(long position) {
			if (BitConverter == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			if (position < 0)
				throw new ArgumentOutOfRangeException("position");
			byte[] array = new byte[2];
			ReadPosition(array, position);
			return BitConverter.ToUInt16(array);
		}
		internal UInt32 ReadUInt32(long position) {
			if (BitConverter == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			if (position < 0)
				throw new ArgumentOutOfRangeException("position");
			byte[] array = new byte[4];
			ReadPosition(array, position);
			return BitConverter.ToUInt32(array);
		}
		internal UInt64 ReadUInt64(long position) {
			if (BitConverter == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			if (position < 0)
				throw new ArgumentOutOfRangeException("position");
			byte[] array = new byte[8];
			ReadPosition(array, position);
			return BitConverter.ToUInt64(array);
		}
		internal string ReadString(int size) {
			if (BitConverter == null)
				ThrowFileHandlerNotCorrectlyInitializedException();
			if (size < 1)
				throw new ArgumentOutOfRangeException("size");
			byte[] array = new byte[size];
			Read(array);
			return BitConverter.ToString(array);
		}
		void ThrowFileHandlerNotCorrectlyInitializedException() {
			throw new Exception("The file handler is not correctly initialized.");
		}
		void ThrowReadBytesAmountMismatchException() {
			throw new Exception("The number of bytes read mismatches the specified amount.");
		}
	}
	#endregion
	#region KeepOpenInputHandler
	[CLSCompliant(false)]
	public class KeepOpenInputHandler : InputHandler {
		public KeepOpenInputHandler(Stream stream)
			: base(stream) {
		}
		public override void CloseStream() {
		}
	}
	#endregion
}

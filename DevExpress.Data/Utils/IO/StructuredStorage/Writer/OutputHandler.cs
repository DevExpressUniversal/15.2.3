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
using DevExpress.Office.Utils;
namespace DevExpress.Utils.StructuredStorage.Internal.Writer {
	#region OutputHandler
	[CLSCompliant(false)]
	public class OutputHandler : AbstractIOHandler {
		public OutputHandler(Stream memoryStream) : base(memoryStream) {
			InitBitConverter(true);
		}
		internal Stream BaseStream { get { return Stream; } }
		public override UInt64 IOStreamSize { get { return UInt64.MaxValue; } }
		internal void WriteByte(byte value) {
			Stream.WriteByte(value);
		}
		internal void WriteUInt16(UInt16 value) {
			Stream.Write(BitConverter.GetBytes(value), 0, 2);
		}
		internal void WriteUInt32(UInt32 value) {
			Stream.Write(BitConverter.GetBytes(value), 0, 4);
		}
		internal void WriteUInt64(UInt64 value) {
			Stream.Write(BitConverter.GetBytes(value), 0, 8);
		}
		internal void Write(byte[] data) {
			Stream.Write(data, 0, data.Length);
		}
		internal void WriteSectors(byte[] data, UInt16 sectorSize, byte padding) {
			uint remaining = (uint)(data.Length % sectorSize);
			Stream.Write(data, 0, data.Length);
			if (remaining == 0)
				return;
			for (uint i = 0; i < (sectorSize - remaining); i++)
				Stream.WriteByte(padding);
		}
		internal void WriteSectors(byte[] data, int dataSize, UInt16 sectorSize, byte padding) {
			uint remaining = (uint)(dataSize % sectorSize);
			Stream.Write(data, 0, dataSize);
			if(remaining == 0)
				return;
			for(uint i = 0; i < (sectorSize - remaining); i++)
				Stream.WriteByte(padding);
		}
		internal void WriteSectors(byte[] data, UInt16 sectorSize, UInt32 padding) {
			uint remaining = (uint)(data.Length % sectorSize);
			Stream.Write(data, 0, data.Length);
			if (remaining == 0)
				return;
			if ((sectorSize - remaining) % sizeof(UInt32) != 0)
				throw new Exception("Inconsistency found while writing a sector.");
			for (uint i = 0; i < ((sectorSize - remaining) / sizeof(UInt32)); i++)
				WriteUInt32(padding);
		}
		internal void WriteToStream(Stream stream) {
			const int bytesToReadAtOnce = 512;
			byte[] buf = new byte[bytesToReadAtOnce];
			BaseStream.Seek(0, SeekOrigin.Begin);
			while(true) {
				int bytesRead = BaseStream.Read(buf, 0, bytesToReadAtOnce);
				stream.Write(buf, 0, bytesRead);
				if(bytesRead != bytesToReadAtOnce) {
					break;
				}
			}
			stream.Flush();
		}
	}
	#endregion
}

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
namespace DevExpress.XtraExport {
	[CLSCompliant(false)]
	public class XlsStream {
		const int MaxBytesWrite = XlsConsts.MaxRecSize97;
		Stream stream;
		bool continueWriting = false;
		long writeContinuePos = 0;
		int writtenBytes = 0;
		byte[] buffer = new byte[16];
		public XlsStream(Stream stream) {
			this.stream = stream;
		}
		internal Stream InnerStream { get { return stream; } }
		public long Position { get { return stream.Position; } }
		public long Length { get { return stream.Length; } }
		void IncWrittenCount(int count) {
			writtenBytes += count;
		}
		protected void CheckContinueWrite(byte[] buffer, int count) {
			if(count + writtenBytes > MaxBytesWrite) {
				int bytes = MaxBytesWrite - writtenBytes;
				Seek(writeContinuePos + 2, SeekOrigin.Begin);
				WriteCore(BitConverter.GetBytes(MaxBytesWrite), 0, 2);
				Seek(0, SeekOrigin.End);
				WriteCore(buffer, 0, bytes);
				this.writeContinuePos = Position;
				writtenBytes = 0;
				count -= bytes;
				byte[] newBuffer = new byte[count];
				Array.Copy(buffer, bytes, newBuffer, 0, count);
				WriteHeader(XlsConsts.BIFFRecId_Continue, 0);
				CheckContinueWrite(newBuffer, count);
			}
			else {
				WriteCore(buffer, 0, count);
				IncWrittenCount(count);
			}
		}
		protected internal virtual void WriteCore(byte[] buffer, int offset, int count) {
			stream.Write(buffer, offset, count);
		}
		internal void WriteCore(ushort value) {
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			WriteCore(buffer, 0, 2);
		}
		public virtual void Flush() {
			stream.Flush();
		}
		public virtual void Close() {
			stream.Close();
		}
		public void BeginContinueWrite() {
			continueWriting = true;
			this.writtenBytes = 0;
			this.writeContinuePos = Position;
		}
		public long Seek(long offset, SeekOrigin origin) {
			return stream.Seek(offset, origin);
		}
		public void EndContinueWrite() {
			if(writtenBytes > 0) {
				Seek(writeContinuePos + 2, SeekOrigin.Begin);
				System.Diagnostics.Debug.Assert(writtenBytes <= Math.Min(MaxBytesWrite, (int)ushort.MaxValue));
				ushort count = (ushort)writtenBytes;
				stream.Write(BitConverter.GetBytes(count), 0, 2);
				Seek(0, SeekOrigin.End);
			}
			continueWriting = false;
		}
		public void Write(ushort value) {
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			Write(buffer, 0, 2);
		}
		public void Write(byte value) {
			buffer[0] = value;
			Write(buffer, 0, 1);
		}
		public void Write(bool value) {
			buffer[0] = (byte)(value ? 1 : 0);
			Write(buffer, 0, 1);
		}
		public void Write(uint value) {
			Write((int)value);
		}
		public void Write(int value) {
			buffer[0] = (byte)value;
			buffer[1] = (byte)(value >> 8);
			buffer[2] = (byte)(value >> 16);
			buffer[3] = (byte)(value >> 24);
			Write(buffer, 0, 4);
		}
		public void Write(byte[] buffer, int offset, int count) {
			if(continueWriting)
				CheckContinueWrite(buffer, count);
			else {
				IncWrittenCount(count);
				WriteCore(buffer, offset, count);
			}
		}
		public void WriteHeader(ushort recId, int len) {
			BIFFHeader header = new BIFFHeader();
			header.RecId = recId;
			header.Length = Convert.ToUInt16(len & 0xFFFF);
			header.WriteToStream(this);
		}
	}
}

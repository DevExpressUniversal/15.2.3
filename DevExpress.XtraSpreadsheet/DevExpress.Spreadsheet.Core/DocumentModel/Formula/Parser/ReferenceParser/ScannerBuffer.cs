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
#if SL
using DevExpress.Utils;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region IBuffer
	public interface IBuffer {
		string GetString(int beg, int end);
		int Peek();
		int Pos { get; set; }
		int Read();
	}
	#endregion
	#region Buffer
	public abstract class Buffer : IBuffer {
		public const int EOF = char.MaxValue + 1;
		#region IBuffer Members
		public abstract string GetString(int beg, int end);
		public abstract int Peek();
		public abstract int Pos { get; set; }
		public abstract int Read();
		#endregion
	}
	#endregion
	public class StreamBuffer : Buffer {
		const int MIN_BUFFER_LENGTH = 1024; 
		const int MAX_BUFFER_LENGTH = MIN_BUFFER_LENGTH * 64; 
		byte[] buf;		 
		int bufStart;	   
		int bufLen;		 
		int fileLen;		
		int bufPos;		 
		Stream stream;	  
		bool isUserStream;  
		public StreamBuffer(Stream s, bool isUserStream) {
			stream = s; this.isUserStream = isUserStream;
			if (stream.CanSeek) {
				fileLen = (int)stream.Length;
				bufLen = Math.Min(fileLen, MAX_BUFFER_LENGTH);
				bufStart = Int32.MaxValue; 
			}
			else {
				fileLen = bufLen = bufStart = 0;
			}
			buf = new byte[(bufLen > 0) ? bufLen : MIN_BUFFER_LENGTH];
			if (fileLen > 0) Pos = 0; 
			else bufPos = 0; 
			if (bufLen == fileLen && stream.CanSeek) Close();
		}
		protected StreamBuffer(StreamBuffer b) { 
			buf = b.buf;
			bufStart = b.bufStart;
			bufLen = b.bufLen;
			fileLen = b.fileLen;
			bufPos = b.bufPos;
			stream = b.stream;
			b.stream = null;
			isUserStream = b.isUserStream;
		}
		~StreamBuffer() { Close(); }
		protected void Close() {
			if (!isUserStream && stream != null) {
				stream.Dispose();
				stream = null;
			}
		}
		public override int Read() {
			if (bufPos < bufLen) {
				return buf[bufPos++];
			}
			else if (Pos < fileLen) {
				Pos = Pos; 
				return buf[bufPos++];
			}
			else if (stream != null && !stream.CanSeek && ReadNextStreamChunk() > 0) {
				return buf[bufPos++];
			}
			else {
				return EOF;
			}
		}
		public override int Peek() {
			int curPos = Pos;
			int ch = Read();
			Pos = curPos;
			return ch;
		}
		public override string GetString(int beg, int end) {
			int len = 0;
			char[] buf = new char[end - beg];
			int oldPos = Pos;
			Pos = beg;
			while (Pos < end) buf[len++] = (char)Read();
			Pos = oldPos;
			return new String(buf, 0, len);
		}
		public override int Pos {
			get { return bufPos + bufStart; }
			set {
				if (value >= fileLen && stream != null && !stream.CanSeek) {
					while (value >= fileLen && ReadNextStreamChunk() > 0) ;
				}
				if (value < 0 || value > fileLen) {
					throw new FatalError("buffer out of bounds access, position: " + value);
				}
				if (value >= bufStart && value < bufStart + bufLen) { 
					bufPos = value - bufStart;
				}
				else if (stream != null) { 
					stream.Seek(value, SeekOrigin.Begin);
					bufLen = stream.Read(buf, 0, buf.Length);
					bufStart = value; bufPos = 0;
				}
				else {
					bufPos = fileLen - bufStart;
				}
			}
		}
		int ReadNextStreamChunk() {
			int free = buf.Length - bufLen;
			if (free == 0) {
				byte[] newBuf = new byte[bufLen * 2];
				Array.Copy(buf, newBuf, bufLen);
				buf = newBuf;
				free = bufLen;
			}
			int read = stream.Read(buf, bufLen, free);
			if (read > 0) {
				fileLen = bufLen = (bufLen + read);
				return read;
			}
			return 0;
		}
	}
	public class UTF8Buffer : StreamBuffer {
		public UTF8Buffer(StreamBuffer b) : base(b) { }
		public override int Read() {
			int ch;
			do {
				ch = base.Read();
			} while ((ch >= 128) && ((ch & 0xC0) != 0xC0) && (ch != EOF));
			if (ch < 128 || ch == EOF) {
			}
			else if ((ch & 0xF0) == 0xF0) {
				int c1 = ch & 0x07; ch = base.Read();
				int c2 = ch & 0x3F; ch = base.Read();
				int c3 = ch & 0x3F; ch = base.Read();
				int c4 = ch & 0x3F;
				ch = (((((c1 << 6) | c2) << 6) | c3) << 6) | c4;
			}
			else if ((ch & 0xE0) == 0xE0) {
				int c1 = ch & 0x0F; ch = base.Read();
				int c2 = ch & 0x3F; ch = base.Read();
				int c3 = ch & 0x3F;
				ch = (((c1 << 6) | c2) << 6) | c3;
			}
			else if ((ch & 0xC0) == 0xC0) {
				int c1 = ch & 0x1F; ch = base.Read();
				int c2 = ch & 0x3F;
				ch = (c1 << 6) | c2;
			}
			return ch;
		}
	}
	public class StringBuffer : Buffer {
		int stringLen;		
		int bufPos;		 
		string str;
		public StringBuffer(string str) {
			stringLen = str.Length;
			this.str = str;
			if (stringLen > 0) Pos = 0; 
			else bufPos = 0; 
		}
		public override int Read() {
			if (bufPos < stringLen)
				return str[bufPos++];
			else
				return EOF;
		}
		public override int Peek() {
			int curPos = Pos;
			int ch = Read();
			Pos = curPos;
			return ch;
		}
		public override string GetString(int beg, int end) {
			return str.Substring(beg, end - beg);
		}
		public override int Pos {
			get { return bufPos; }
			set {
				if (value < 0 || value > stringLen)
					throw new FatalError("buffer out of bounds access, position: " + value);
				bufPos = value;
			}
		}
	}
}

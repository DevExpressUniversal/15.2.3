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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.SpreadsheetSource.Implementation;
namespace DevExpress.SpreadsheetSource.Csv {
	using DevExpress.Utils;
	#region CsvStreamReader
	public class CsvStreamReader {
		#region Fields
		const int defaultBufferSize = 1024;
		const int minBufferSize = 256;
		readonly Stream stream;
		readonly Decoder decoder;
		byte[] byteBuffer;
		char[] charBuffer;
		int byteLen;
		int charLen;
		int charPos;
		#endregion
		public CsvStreamReader(Stream stream, Encoding encoding) 
			: this(stream, encoding, defaultBufferSize) {
		}
		public CsvStreamReader(Stream stream, Encoding encoding, int bufferSize) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(encoding, "encoding");
			bufferSize = Math.Max(bufferSize, minBufferSize);
			this.stream = stream;
			this.decoder = encoding.GetDecoder();
			this.byteBuffer = new byte[bufferSize];
			int charSize = encoding.GetMaxCharCount(bufferSize);
			this.charBuffer = new char[charSize];
			byteLen = 0;
			charLen = 0;
			charPos = 0;
		}
		public bool EndOfStream {
			get {
				if(charPos < charLen)
					return false;
				return ReadBuffer() == 0;
			}
		}
		public int Read() {
			if(charPos == charLen) {
				if(ReadBuffer() == 0) 
					return -1;
			}
			return charBuffer[charPos++];
		}
		public int Peek() {
			if(charPos == charLen) {
				if(ReadBuffer() == 0) 
					return -1;
			}
			return charBuffer[charPos];
		}
		int ReadBuffer() {
			charLen = 0;
			charPos = 0;
			byteLen = this.stream.Read(byteBuffer, 0, byteBuffer.Length);
			if(byteLen > 0)
				charLen = decoder.GetChars(byteBuffer, 0, byteLen, charBuffer, charLen);
			return charLen;
		}
	}
	#endregion
}

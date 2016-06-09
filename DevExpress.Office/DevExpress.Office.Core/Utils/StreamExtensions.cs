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
using System.Text;
namespace DevExpress.Office.Utils {
	#region StreamExtensions
	public static class StreamExtensions {
		public static void Write(this Stream stream, string value, Encoding encoding) {
			const int bufferSize = 32768;
			const int maxCharCount = 32768 / 4; 
			byte[] bytes = new byte[bufferSize];
			int length = value.Length;
			int offset = 0;
			while (length > maxCharCount) {
				int byteCount = encoding.GetBytes(value, offset, maxCharCount, bytes, 0);
				stream.Write(bytes, 0, byteCount);
				length -= maxCharCount;
				offset += maxCharCount;
			}
			if (length > 0) {
				int byteCount = encoding.GetBytes(value, offset, length, bytes, 0);
				stream.Write(bytes, 0, byteCount);
			}
		}
		public static void Write(this Stream stream, StringBuilder value, Encoding encoding) {
			const int bufferSize = 32768;
			const int maxCharCount = 32768 / 4; 
			byte[] bytes = new byte[bufferSize];
			int length = value.Length;
			int offset = 0;
			while (length > maxCharCount) {
				int byteCount = encoding.GetBytes(value.ToString(offset, maxCharCount), 0, maxCharCount, bytes, 0);
				stream.Write(bytes, 0, byteCount);
				length -= maxCharCount;
				offset += maxCharCount;
			}
			if (length > 0) {
				int byteCount = encoding.GetBytes(value.ToString(offset, length), 0, length, bytes, 0);
				stream.Write(bytes, 0, byteCount);
			}
		}
		public static void ReadToEnd(this Stream stream) {
			byte[] buffer = new byte[8192];
			for (; ; ) {
				int bytesRead = stream.Read(buffer, 0, buffer.Length);
				if (bytesRead < buffer.Length)
					break;
			}
		}
	}
	#endregion
}

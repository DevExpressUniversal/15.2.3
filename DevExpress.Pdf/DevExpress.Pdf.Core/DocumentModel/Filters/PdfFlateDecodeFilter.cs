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

using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfFlateDecodeFilter : PdfFlateLZWDecodeFilter {
		internal const string Name = "FlateDecode";
		internal const string ShortName = "Fl";
		const int bufferSize = 256;
		protected internal override string FilterName { get { return Name; } }
		internal PdfFlateDecodeFilter(PdfReaderDictionary parameters) : base(parameters) {
		}
		protected override byte[] PerformDecode(byte[] data) {
			int dataLength = data.Length;
			if (dataLength == 0)
				return data;
			if (dataLength < 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			byte compressionMethod = data[0];
			byte flags = data[1];
			int compressionInfo = ((compressionMethod & 0xf0) >> 4) + 8;
			if ((compressionMethod & 0x0f) != 8 || compressionInfo > 15 || (compressionMethod * 256 + flags) % 31 > 0 || (flags & 0x20) > 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			if (dataLength == 2)
				return new byte[0];
			using (MemoryStream stream = new MemoryStream(data)) {
				List<byte> result = new List<byte>();
				stream.Position = 2;
				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true)) {
					byte[] buffer = new byte[bufferSize];
					for (;;) {
						int count = deflateStream.Read(buffer, 0, bufferSize);
						if (count == bufferSize)
							result.AddRange(buffer);
						else { 
							if (count != 0) {
								Array.Resize<byte>(ref buffer, count);
								result.AddRange(buffer);
							}
							break;
						}
					}
				}
				return result.ToArray();
			}
		}
	}
}

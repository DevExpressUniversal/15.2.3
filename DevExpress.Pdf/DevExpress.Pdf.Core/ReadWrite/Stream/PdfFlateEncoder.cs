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

using System.IO;
using System.IO.Compression;
namespace DevExpress.Pdf.Native {
	public class PdfFlateEncoder : PdfDisposableObject {
		public static byte[] Encode(byte[] data) {
			using (PdfFlateEncoder encoder = new PdfFlateEncoder()) {
				encoder.DeflateStream.Write(data, 0, data.Length);
				encoder.Close();
				PdfAdler32.Calculate(data, encoder.Stream);
				return encoder.GetData();
			}
		}
		readonly MemoryStream stream;
		readonly DeflateStream deflateStream;
		public MemoryStream Stream { get { return stream; } }
		public DeflateStream DeflateStream { get { return deflateStream; } }
		public PdfFlateEncoder() {
			stream = new MemoryStream();
			stream.WriteByte(0x58);
			stream.WriteByte(0x85);
			deflateStream = new DeflateStream(stream, CompressionMode.Compress, true);
		}
		public void Close() {
			deflateStream.Dispose();
		}
		public byte[] GetData() {
			return stream.ToArray();
		}
		protected override void Dispose(bool disposing) {
			deflateStream.Dispose();
			stream.Dispose();
		}
	}
}

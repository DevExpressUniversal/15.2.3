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
namespace DevExpress.Pdf.Native {
	public class PdfUnsignedDocumentStream : Stream {
		readonly PdfDocumentStream stream;
		readonly long skipOffset;
		readonly int skipLength;
		readonly long skipEnd;
		readonly long initialPosition;
		public override bool CanSeek { get { return true; } }
		public override bool CanRead { get { return true; } }
		public override bool CanWrite { get { return false; } }
		public override long Length { get { return stream.Length - skipLength; } }
		public override long Position {
			get { 
				long position = stream.Position;
				return position <= skipOffset ? position : (position - skipLength);
			}
			set { stream.Position = (value >= skipOffset) ? (value + skipLength) : value; }
		}
		public PdfUnsignedDocumentStream(PdfDocumentStream stream, long skipOffset, int skipLength) {
			this.stream = stream;
			this.skipOffset = skipOffset;
			this.skipLength = skipLength;
			skipEnd = skipOffset + skipEnd;;
			initialPosition = stream.Position;
			Position = 0;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if (disposing)
				stream.Position = initialPosition;
		}
		public override long Seek(long offset, SeekOrigin origin) {
			long position;
			switch (origin) {
				case SeekOrigin.Current:
					position = Position + offset;
					break;
				case SeekOrigin.End:
					position = Length - offset;
					break;
				default:
					position = offset;
					break;
			}
			Position = position;
			return position;
		}
		public override int Read(byte[] buffer, int offset, int count) {
			bool shouldUpdatePosition = false;
			long position = Position;
			if (position < skipOffset) {
				int remain = (int)(position + count - skipOffset);
				if (remain > 0) {
					count -= remain;
					shouldUpdatePosition = true;
				}
			}
			int actualCount = Math.Min(count, (int)(Length - position));
			if (actualCount <= 0)
				return 0;
			byte[] readed = stream.ReadBytes(actualCount);
			Array.Copy(readed, 0, buffer, offset, actualCount);
			if (shouldUpdatePosition)
				Position = Position;
			return actualCount;
		}
		public override void Write(byte[] buffer, int offset, int count) {
		}
		public override void Flush() {
		}
		public override void SetLength(long value) {
		}
	}
}

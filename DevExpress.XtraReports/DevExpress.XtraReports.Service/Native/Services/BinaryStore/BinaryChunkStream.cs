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
using System.Linq;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Service.Native.Services.BinaryStore {
	class BinaryChunkStream : Stream {
		readonly long length;
		readonly IEnumerable<IBinaryContentProvider> chunks;
		IEnumerator<IBinaryContentProvider> enumerator;
		long position;
		int chunkOffset;
		bool firstMoveNextCalled;
		bool endOfChunks;
		public BinaryChunkStream(IEnumerable<IBinaryContentProvider> chunks) {
			Guard.ArgumentNotNull(chunks, "chunks");
			this.chunks = chunks;
			length = chunks.Sum(c => c.Content.LongLength);
			Reset();
		}
		#region Stream
		public override bool CanRead {
			get { return true; }
		}
		public override bool CanSeek {
			get { return true; }
		}
		public override bool CanWrite {
			get { return false; }
		}
		public override void Flush() {
			throw new NotSupportedException();
		}
		public override long Length {
			get { return length; }
		}
		public override long Position {
			get { return position; }
			set { Seek(value, SeekOrigin.Begin); }
		}
		public override int Read(byte[] buffer, int offset, int count) {
			if(!SafeFirstMoveNext() || endOfChunks) {
				return 0;
			}
			var chunk = enumerator.Current;
			var readLength = Math.Min(count, chunk.Content.Length - chunkOffset);
			Buffer.BlockCopy(chunk.Content, chunkOffset, buffer, offset, readLength);
			chunkOffset += readLength;
			position += readLength;
			if(chunkOffset >= chunk.Content.Length - 1) {
				MoveNext();
				chunkOffset = 0;
			}
			return readLength;
		}
		public override long Seek(long offset, SeekOrigin origin) {
			if(offset == 0L && origin == SeekOrigin.Begin) {
				Reset();
				return 0;
			}
			throw new NotSupportedException();
		}
		public override void SetLength(long value) {
			throw new NotSupportedException();
		}
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException();
		}
		#endregion
		void Reset() {
			enumerator = chunks.GetEnumerator();
			position = 0;
			chunkOffset = 0;
			firstMoveNextCalled = false;
			endOfChunks = false;
		}
		bool SafeFirstMoveNext() {
			if(!firstMoveNextCalled) {
				firstMoveNextCalled = true;
				return MoveNext();
			}
			return true;
		}
		bool MoveNext() {
			var hasMore = enumerator.MoveNext();
			if(!hasMore) {
				endOfChunks = true;
			}
			return hasMore;
		}
	}
}

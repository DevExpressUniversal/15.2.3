#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using DevExpress.Utils.Zip;
using System.Collections.Generic;
using DevExpress.Utils.Zip.Internal;
namespace DevExpress.Compression.Internal {
	#region SubStreamBase
	class SubStreamBase : Stream {
		#region Fields
		readonly Stream baseStream;
		readonly long basePosition;
		long length;
		long position;
		bool isPackedStream;
		#endregion
		public SubStreamBase(Stream baseStream, long basePosition, long length, bool isPackedStream) {
			this.baseStream = baseStream;
			this.basePosition = basePosition;
			this.length = length;
			this.isPackedStream = isPackedStream;
		}
		#region Properties
		public Stream BaseStream { get { return baseStream; } }
		public override bool CanRead { get { return BaseStream.CanRead; } }
		public override bool CanSeek { get { return BaseStream.CanSeek; } }
		public override bool CanWrite { get { return BaseStream.CanWrite; } }
		public override long Length {
			get {
				if (length < 0)
					throw new NotSupportedException();
				return length;
			}
		}
		public override long Position {
			get { return position; }
			set {
				if (value < 0 || (IsLeftActual() && (value > Length)))
					throw new ArgumentException();
				position = value;
				ValidateBaseStreamPosition();
			}
		}
		#endregion
		protected internal void ValidateBaseStreamPosition() {
			if (BaseStream.Position != basePosition + position)
				BaseStream.Seek(basePosition + position - BaseStream.Position, SeekOrigin.Current);
		}
		public sealed override int Read(byte[] buffer, int offset, int count) {
			if (!CanRead)
				return 0;
			ValidateBaseStreamPosition();
			if (this.length >= 0 && !this.isPackedStream) {
				if (this.position + count > length)
					count = (int)(length - this.position);
			}
			int actualByteCount = ReadCore(buffer, offset, count);
			this.position += actualByteCount;
			return actualByteCount;
		}
		public sealed override void Write(byte[] buffer, int offset, int count) {
			if (!CanWrite)
				return;
			ValidateBaseStreamPosition();
			WriteCore(buffer, offset, count);
			if (IsLeftActual())
				SetLength(Math.Max(Length, Position + count));
			Position += count;
		}
		public virtual void WriteCore(byte[] buffer, int offset, int count) {
			BaseStream.Write(buffer, offset, count);
		}
		protected virtual int ReadCore(byte[] buffer, int offset, int count) {
			return BaseStream.Read(buffer, offset, count);
		}
		public override long Seek(long offset, SeekOrigin origin) {
			if (!CanSeek)
				throw new NotSupportedException();
			if (origin < SeekOrigin.Begin || origin > SeekOrigin.End)
				throw new ArgumentException();
			if (origin == SeekOrigin.Begin)
				this.position = offset;
			else if (origin == SeekOrigin.Current)
				this.position = this.position + offset;
			else if (origin == SeekOrigin.End)
				this.position = Length - offset;
			if (this.position < 0)
				this.position = 0;
			else if (this.position > Length)
				this.position = Length;
			ValidateBaseStreamPosition();
			return position;
		}
		public override void SetLength(long value) {
			if (this.length < 0)
				throw new NotSupportedException();
			this.length = value;
		}
		bool IsLeftActual() {
			return this.length > -1;
		}
		public override void Flush() {
			throw new NotSupportedException();
		}
	}
	#endregion
}

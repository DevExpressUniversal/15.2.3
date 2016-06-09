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
using DevExpress.Utils.StructuredStorage.Internal;
using DevExpress.Utils.StructuredStorage.Internal.Reader;
using DevExpress.Office;
namespace DevExpress.Utils.StructuredStorage.Reader {
	#region VirtualStream
	[CLSCompliant(false)]
	public class VirtualStream : Stream {
		#region Fields
		readonly AbstractFat fat;
		readonly string name;
		readonly long length;
		readonly List<UInt32> sectors;
		UInt32 startSector;
		UInt32 lastReadSectorIndex = UInt32.MaxValue;
		byte[] lastReadSector;
		long position;
		#endregion
		public VirtualStream(AbstractFat fat, UInt32 startSector, long sizeOfStream, string name) {
			Guard.ArgumentNotNull(fat, "fat");
			this.fat = fat;
			this.startSector = startSector;
			this.length = sizeOfStream;
			this.name = name;
			if (startSector == SectorType.EndOfChain || startSector == SectorType.Free || Length == 0)
				return;
			this.sectors = fat.GetSectorChain(startSector, (UInt64)Math.Ceiling((double)length / fat.SectorSize), name);
			CheckConsistency();
			this.lastReadSector = new byte[fat.SectorSize];
		}
		#region Properties
		public override long Position { get { return position; } set { position = value; } }
		public override long Length { get { return length; } }
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return true; } }
		public override bool CanWrite { get { return false; } }
		#endregion
		public override int Read(byte[] array, int offset, int count) {
			return Read(array, offset, count, position);
		}
		public int Read(byte[] array, int offset, int count, long position) {
			if (array.Length < 1 || count < 1 || position < 0 || offset < 0)
				return 0;
			if (offset + count > array.Length)
				return 0;
			if (position + count > this.Length) {
				count = Convert.ToInt32(Length - position);
				if (count < 1)
					return 0;
			}
			this.position = position;
			int sectorInChain = (int)(position / fat.SectorSize);
			int bytesRead = 0;
			int totalBytesRead = 0;
			int positionInArray = offset;
			int positionInSector = Convert.ToInt32(position % fat.SectorSize);
			UInt32 currentSector = sectors[sectorInChain];
			if (currentSector != lastReadSectorIndex) {
				fat.SeekToPositionInSector(currentSector, 0);
				fat.UncheckedRead(lastReadSector, 0, fat.SectorSize);
				lastReadSectorIndex = currentSector;
			}
			int bytesToReadInFirstSector = (count > fat.SectorSize - positionInSector) ? (fat.SectorSize - positionInSector) : count;
			Array.Copy(lastReadSector, positionInSector, array, positionInArray, bytesToReadInFirstSector);
			bytesRead = bytesToReadInFirstSector;
			this.position += bytesRead;
			positionInArray += bytesRead;
			totalBytesRead += bytesRead;
			sectorInChain++;
			while (totalBytesRead + fat.SectorSize < count) {
				fat.SeekToPositionInSector(sectors[sectorInChain], 0);
				bytesRead = fat.UncheckedRead(array, positionInArray, fat.SectorSize);
				this.position += bytesRead;
				positionInArray += bytesRead;
				totalBytesRead += bytesRead;
				sectorInChain++;
				if (bytesRead != fat.SectorSize)
					return totalBytesRead;
			}
			if (totalBytesRead >= count)
				return totalBytesRead;
			lastReadSectorIndex = sectors[sectorInChain];
			fat.SeekToPositionInSector(lastReadSectorIndex, 0);
			fat.UncheckedRead(lastReadSector, 0, fat.SectorSize);
			bytesRead = count - totalBytesRead;
			Array.Copy(lastReadSector, 0, array, positionInArray, bytesRead);
			this.position += bytesRead;
			positionInArray += bytesRead;
			totalBytesRead += bytesRead;
			return totalBytesRead;
		}
		void CheckConsistency() {
			if(((UInt64)sectors.Count) < Math.Ceiling((double)length / fat.SectorSize))
				fat.ThrowChainSizeMismatchException(name);
		}
		public override void Flush() {
			throw new NotSupportedException("This method is not supported on a read-only stream.");
		}
		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case System.IO.SeekOrigin.Begin:
					this.position = offset;
					break;
				case System.IO.SeekOrigin.Current:
					this.position += offset;
					break;
				case System.IO.SeekOrigin.End:
					this.position = length - offset;
					break;
			}
			if (position < 0)
				position = 0;
			else if (position > length)
				position = length;
			return position;
		}
		public override void SetLength(long value) {
			throw new NotSupportedException("This method is not supported on a read-only stream.");
		}
		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotSupportedException("This method is not supported on a read-only stream.");
		}
		public Stream Clone() {
			return new VirtualStream(this.fat, this.startSector, this.length, this.name);
		}
	}
	#endregion
}

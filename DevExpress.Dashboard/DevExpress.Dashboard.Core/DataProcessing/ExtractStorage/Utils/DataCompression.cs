#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Runtime.InteropServices;
using System.Security;
namespace DevExpress.DashboardCommon.DataProcessing {
	public static class DataCompression {
		public static int CalculateBitsForElement(int count) {
			if (count == 1)
				return 1;
			int bitsForElement = 0;
			long power = 1;
			while (power < count) {
				bitsForElement++;
				power = power << 1;
			}
			return bitsForElement;
		}
		public static byte[] Compression(int[] indexes, int uniqueCount, int dataCount) {
			int bitsForElement = CalculateBitsForElement(uniqueCount);
			int countOfBytes = (dataCount * bitsForElement) / 8;
			if ((dataCount * bitsForElement) % 8 != 0)
				countOfBytes++;
			byte[] compressionData = new byte[countOfBytes];
			int indexCompression = 0;
			if (countOfBytes > 0) {
				int value = 0;
				int countBits = 0;
				for (int i = 0; i < indexes.Length; i++) {
					value = (value << bitsForElement) ^ indexes[i];
					countBits += bitsForElement;
					while (countBits >= 8) {
						byte byteValue = (byte)(value >> (countBits - 8));
						compressionData[indexCompression++] = byteValue;
						countBits -= 8;
					}
				}
				if (countBits > 0) 
					compressionData[indexCompression] = (byte)(value << (8 - countBits));
			}
			return compressionData;
		}
		public static int[] Decompression(ByteBuffer compressionData, int uniqueDataCount, int dataCount) {
			int[] decompressionData = new int[dataCount];
			int bitsForElement = CalculateBitsForElement(uniqueDataCount);
			int currentPosition = 0;
			ulong mask = (ulong)Math.Pow(2, bitsForElement) - 1;
			ulong value = 0;
			int countOfBits = 0;
			for (int i = 0; i < dataCount; i++) {
				while (countOfBits < bitsForElement) {
					value = (value << 8) ^ compressionData[currentPosition++];
					countOfBits += 8;
				}
				countOfBits -= bitsForElement;
				decompressionData[i] = (int)((value >> countOfBits) & mask);
			}
			return decompressionData;
		}		
		public static void Decompression(byte[] compressionData, int compressionOffset, int[] decompressionData, int uniqueDataCount, int dataCount) {
			int bitsForElement = CalculateBitsForElement(uniqueDataCount);
			int currentPosition = 0;
			ulong mask = (ulong)Math.Pow(2, bitsForElement) - 1;
			ulong value = (ulong)((compressionData[currentPosition++] << compressionOffset) >> compressionOffset);
			int countOfBits = 8 - compressionOffset;
			for (int i = 0; i < dataCount; i++) {
				while (countOfBits < bitsForElement) {
					value = (value << 8) ^ compressionData[currentPosition++];
					countOfBits += 8;
				}
				countOfBits -= bitsForElement;
				decompressionData[i] = (int)((value >> countOfBits) & mask);
			}
		}
	}
	public static class DataConvertor {
		[SecuritySafeCritical]
		public static byte[] Convert(int[] values) {
			byte[] buff = new byte[values.Length * 4];
			GCHandle dHandle = GCHandle.Alloc(buff, GCHandleType.Pinned);
			Marshal.Copy(values, 0, dHandle.AddrOfPinnedObject(), values.Length);
			dHandle.Free();
			return buff;
		}
		[SecuritySafeCritical]
		public static int[] Convert(byte[] values) {
			int[] buff = new int[values.Length / 4];
			GCHandle dHandle = GCHandle.Alloc(buff, GCHandleType.Pinned);
			Marshal.Copy(values, 0, dHandle.AddrOfPinnedObject(), values.Length);
			dHandle.Free();
			return buff;
		}
	}
}

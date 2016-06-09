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

#if SL
#define CUSTOM_MD5
#endif
using System;
using System.IO;
using System.Collections.Generic;
using DevExpress.Utils;
#if !SL
using System.Security.Cryptography;
#endif
namespace DevExpress.XtraSpreadsheet.Internal {
	public static class MD5Hash {
#if CUSTOM_MD5
		class MD5CustomImpl {
			const int blockLengthInBytes = 64; 
			const int alignLengthInBytes = 56; 
			const int originalLengthSize = 8; 
			static readonly UInt32[] T = new UInt32[] { 
				0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee, 0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
				0x698098d8,	0x8b44f7af,	0xffff5bb1,	0x895cd7be, 0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
				0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa, 0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
				0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed, 0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
				0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c, 0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
				0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05, 0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
				0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039, 0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
				0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1, 0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
			};
			class MD5StreamReader : BinaryReader {
				long dataLength;
				long dataPosition;
				byte[] padding;
				int paddingPos;
				public MD5StreamReader(Stream input)
					: base(input) {
					long inputLength = input.Length;
					long rest = inputLength % blockLengthInBytes;
					int paddingLength;
					if(rest < alignLengthInBytes)
						paddingLength = (int)(alignLengthInBytes - rest);
					else
						paddingLength = (int)(alignLengthInBytes + blockLengthInBytes - rest);
					paddingLength += originalLengthSize;
					dataLength = inputLength + paddingLength;
					padding = new byte[paddingLength];
					padding[0] = 0x80;
					UInt64 originalLengthInBits = (UInt64)inputLength << 3;
					Array.Copy(BitConverter.GetBytes(originalLengthInBits), 0, padding, paddingLength - originalLengthSize, originalLengthSize);
				}
				public bool CanRead { get { return dataPosition < dataLength; } }
				public byte[] ReadBlock() {
					int count = blockLengthInBytes;
					byte[] result = new byte[blockLengthInBytes];
					if(dataPosition >= BaseStream.Length) {
						Array.Copy(padding, paddingPos, result, 0, padding.Length - paddingPos);
					}
					else {
						if((dataPosition + count) <= BaseStream.Length) {
							base.Read(result, 0, count);
						}
						else {
							count = (int)(BaseStream.Length - dataPosition);
							if(count > 0)
								base.Read(result, 0, count);
							if(count < blockLengthInBytes) {
								paddingPos = blockLengthInBytes - count;
								Array.Copy(padding, 0, result, count, paddingPos);
							}
						}
					}
					dataPosition += blockLengthInBytes;
					return result;
				}
			}
			UInt32 A;
			UInt32 B;
			UInt32 C;
			UInt32 D;
			byte[] dataBlock;
			UInt32[] X = new UInt32[16];
			public byte[] ComputeHash(Stream stream) {
				Initialize();
				using(MD5StreamReader reader = new MD5StreamReader(stream)) {
					while(reader.CanRead) {
						dataBlock = reader.ReadBlock();
						ComputeHashCore();
					}
				}
				return GetComputedHash(); ;
			}
			public byte[] ComputeHash(byte[] buffer) {
				using(MemoryStream stream = new MemoryStream(buffer, 0, buffer.Length, false)) {
					return ComputeHash(stream);
				}
			}
			public byte[] ComputeHash(byte[] buffer, int start, int count) {
				using(MemoryStream stream = new MemoryStream(buffer, start, count, false)) {
					return ComputeHash(stream);
				}
			}
			void Initialize() {
				A = 0x67452301;
				B = 0xefcdab89;
				C = 0x98badcfe;
				D = 0x10325476;
			}
			void ComputeHashCore() {
				UInt32 prevA = A;
				UInt32 prevB = B;
				UInt32 prevC = C;
				UInt32 prevD = D;
				for(int i = 0; i < 16; i++)
					X[i] = BitConverter.ToUInt32(dataBlock, i * 4);
				A = MD5Transform1(A, B, C, D, X[0], T[0], 7);
				D = MD5Transform1(D, A, B, C, X[1], T[1], 12);
				C = MD5Transform1(C, D, A, B, X[2], T[2], 17);
				B = MD5Transform1(B, C, D, A, X[3], T[3], 22);
				A = MD5Transform1(A, B, C, D, X[4], T[4], 7);
				D = MD5Transform1(D, A, B, C, X[5], T[5], 12);
				C = MD5Transform1(C, D, A, B, X[6], T[6], 17);
				B = MD5Transform1(B, C, D, A, X[7], T[7], 22);
				A = MD5Transform1(A, B, C, D, X[8], T[8], 7);
				D = MD5Transform1(D, A, B, C, X[9], T[9], 12);
				C = MD5Transform1(C, D, A, B, X[10], T[10], 17);
				B = MD5Transform1(B, C, D, A, X[11], T[11], 22);
				A = MD5Transform1(A, B, C, D, X[12], T[12], 7);
				D = MD5Transform1(D, A, B, C, X[13], T[13], 12);
				C = MD5Transform1(C, D, A, B, X[14], T[14], 17);
				B = MD5Transform1(B, C, D, A, X[15], T[15], 22);
				A = MD5Transform2(A, B, C, D, X[1], T[16], 5);
				D = MD5Transform2(D, A, B, C, X[6], T[17], 9);
				C = MD5Transform2(C, D, A, B, X[11], T[18], 14);
				B = MD5Transform2(B, C, D, A, X[0], T[19], 20);
				A = MD5Transform2(A, B, C, D, X[5], T[20], 5);
				D = MD5Transform2(D, A, B, C, X[10], T[21], 9);
				C = MD5Transform2(C, D, A, B, X[15], T[22], 14);
				B = MD5Transform2(B, C, D, A, X[4], T[23], 20);
				A = MD5Transform2(A, B, C, D, X[9], T[24], 5);
				D = MD5Transform2(D, A, B, C, X[14], T[25], 9);
				C = MD5Transform2(C, D, A, B, X[3], T[26], 14);
				B = MD5Transform2(B, C, D, A, X[8], T[27], 20);
				A = MD5Transform2(A, B, C, D, X[13], T[28], 5);
				D = MD5Transform2(D, A, B, C, X[2], T[29], 9);
				C = MD5Transform2(C, D, A, B, X[7], T[30], 14);
				B = MD5Transform2(B, C, D, A, X[12], T[31], 20);
				A = MD5Transform3(A, B, C, D, X[5], T[32], 4);
				D = MD5Transform3(D, A, B, C, X[8], T[33], 11);
				C = MD5Transform3(C, D, A, B, X[11], T[34], 16);
				B = MD5Transform3(B, C, D, A, X[14], T[35], 23);
				A = MD5Transform3(A, B, C, D, X[1], T[36], 4);
				D = MD5Transform3(D, A, B, C, X[4], T[37], 11);
				C = MD5Transform3(C, D, A, B, X[7], T[38], 16);
				B = MD5Transform3(B, C, D, A, X[10], T[39], 23);
				A = MD5Transform3(A, B, C, D, X[13], T[40], 4);
				D = MD5Transform3(D, A, B, C, X[0], T[41], 11);
				C = MD5Transform3(C, D, A, B, X[3], T[42], 16);
				B = MD5Transform3(B, C, D, A, X[6], T[43], 23);
				A = MD5Transform3(A, B, C, D, X[9], T[44], 4);
				D = MD5Transform3(D, A, B, C, X[12], T[45], 11);
				C = MD5Transform3(C, D, A, B, X[15], T[46], 16);
				B = MD5Transform3(B, C, D, A, X[2], T[47], 23);
				A = MD5Transform4(A, B, C, D, X[0], T[48], 6);
				D = MD5Transform4(D, A, B, C, X[7], T[49], 10);
				C = MD5Transform4(C, D, A, B, X[14], T[50], 15);
				B = MD5Transform4(B, C, D, A, X[5], T[51], 21);
				A = MD5Transform4(A, B, C, D, X[12], T[52], 6);
				D = MD5Transform4(D, A, B, C, X[3], T[53], 10);
				C = MD5Transform4(C, D, A, B, X[10], T[54], 15);
				B = MD5Transform4(B, C, D, A, X[1], T[55], 21);
				A = MD5Transform4(A, B, C, D, X[8], T[56], 6);
				D = MD5Transform4(D, A, B, C, X[15], T[57], 10);
				C = MD5Transform4(C, D, A, B, X[6], T[58], 15);
				B = MD5Transform4(B, C, D, A, X[13], T[59], 21);
				A = MD5Transform4(A, B, C, D, X[4], T[60], 6);
				D = MD5Transform4(D, A, B, C, X[11], T[61], 10);
				C = MD5Transform4(C, D, A, B, X[2], T[62], 15);
				B = MD5Transform4(B, C, D, A, X[9], T[63], 21);
				A += prevA;
				B += prevB;
				C += prevC;
				D += prevD;
			}
			byte[] GetComputedHash() {
				byte[] result = new byte[16];
				Array.Copy(BitConverter.GetBytes(A), 0, result, 0, 4);
				Array.Copy(BitConverter.GetBytes(B), 0, result, 4, 4);
				Array.Copy(BitConverter.GetBytes(C), 0, result, 8, 4);
				Array.Copy(BitConverter.GetBytes(D), 0, result, 12, 4);
				return result;
			}
			UInt32 MD5FuncF(UInt32 x, UInt32 y, UInt32 z) {
				return (x & y) | (~x & z);
			}
			UInt32 MD5FuncG(UInt32 x, UInt32 y, UInt32 z) {
				return (x & z) | (~z & y);
			}
			UInt32 MD5FuncH(UInt32 x, UInt32 y, UInt32 z) {
				return x ^ y ^ z;
			}
			UInt32 MD5FuncI(UInt32 x, UInt32 y, UInt32 z) {
				return y ^ (~z | x);
			}
			UInt32 MD5Transform1(UInt32 a, UInt32 b, UInt32 c, UInt32 d, UInt32 x, UInt32 t, ushort n) {
				return b + LeftRotate32(a + MD5FuncF(b, c, d) + x + t, n);
			}
			UInt32 MD5Transform2(UInt32 a, UInt32 b, UInt32 c, UInt32 d, UInt32 x, UInt32 t, ushort n) {
				return b + LeftRotate32(a + MD5FuncG(b, c, d) + x + t, n);
			}
			UInt32 MD5Transform3(UInt32 a, UInt32 b, UInt32 c, UInt32 d, UInt32 x, UInt32 t, ushort n) {
				return b + LeftRotate32(a + MD5FuncH(b, c, d) + x + t, n);
			}
			UInt32 MD5Transform4(UInt32 a, UInt32 b, UInt32 c, UInt32 d, UInt32 x, UInt32 t, ushort n) {
				return b + LeftRotate32(a + MD5FuncI(b, c, d) + x + t, n);
			}
			UInt32 LeftRotate32(UInt32 value, ushort n) {
				return (value << n) | (value >> (32 - n));
			}
		}
#endif
		public static byte[] ComputeHash(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			var impl = CreateMD5();
			return impl.ComputeHash(stream);
		}
		public static byte[] ComputeHash(byte[] buffer) {
			Guard.ArgumentNotNull(buffer, "buffer");
			var impl = CreateMD5();
			return impl.ComputeHash(buffer);
		}
		public static byte[] ComputeHash(byte[] buffer, int start, int count) {
			Guard.ArgumentNotNull(buffer, "buffer");
			Guard.ArgumentNonNegative(start, "start");
			Guard.ArgumentNonNegative(count, "count");
			if(buffer.Length < (start + count))
				throw new ArgumentException("Buffer is not long enough for that start and count!");
			var impl = CreateMD5();
			return impl.ComputeHash(buffer, start, count);
		}
#if CUSTOM_MD5
		static MD5CustomImpl CreateMD5() {
			return new MD5CustomImpl();
		}
#else
		static MD5 CreateMD5() {
			return MD5.Create();
		}
#endif
	}
}

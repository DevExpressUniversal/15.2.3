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
using DevExpress.Utils.Zip;
namespace DevExpress.Utils.Crypt {
	#region MD4HashCalculator
	[CLSCompliant(false)]
	public class MD4HashCalculator : ICheckSumCalculator<uint[]> {
		#region Fields
		const uint A = 0x67452301;
		const uint B = 0xefcdab89;
		const uint C = 0x98badcfe;
		const uint D = 0x10325476;
		const uint rootOf2 = 0x5a827999;
		const uint rootOf3 = 0x6ed9eba1;
		const int blockLength = 64;
		const int wordsPerStep = 16;
		static readonly int[] stepFCoeffs = new int[] { 3, 7, 11, 19 };
		static readonly int[] stepGCoeffs = new int[] { 3, 5, 9, 13 };
		static readonly int[] stepHIndices = new int[] { 0, 2, 1, 3 };
		static readonly int[] stepHCoeffs = new int[] { 3, 9, 11, 15 };
		int position;
		int bytesRead;
		int tailLength;
		int index;
		Int64 bitsCount;
		byte[] hashBuffer;
		byte[] tail;
		#endregion
		public MD4HashCalculator() {
			hashBuffer = new byte[blockLength];
			tail = new byte[blockLength];
		}
		#region ICheckSumCalculator<uint[]> Members
		public uint[] InitialCheckSumValue {
			get { return new uint[] { A, B, C, D }; }
		}
		public uint[] UpdateCheckSum(uint[] value, byte[] buffer, int offset, int count) {
			if(count == 0)
				return value;
			index = (int)((bitsCount >> 3) & 0x3f);
			int partLength = blockLength - index;
			byte[] bufferWithTail;
			if(tailLength == 0)
				bufferWithTail = buffer;
			else {
				bufferWithTail = new byte[tailLength + count];
				Array.Copy(tail, 0, bufferWithTail, 0, tailLength);
				Array.Copy(buffer, 0, bufferWithTail, tailLength, count);
			}
			count += tailLength;
			if(bytesRead + count >= partLength) {
				if(bytesRead == 0) {
					Array.Copy(buffer, offset, hashBuffer, index, partLength);
					Transform(hashBuffer, 0, value);
				}
				for(position = partLength + bytesRead; position + blockLength - 1 < bytesRead + count; position += blockLength) {
					Transform(buffer, offset + position - bytesRead, value);
				}
				index = 0;
			}
			if(position < bytesRead + count) {
				tailLength = bytesRead + count - position;
				Array.Copy(buffer, offset + position - bytesRead, tail, 0, tailLength);
			}
			bytesRead = position;
			return value;
		}
		public uint[] GetFinalCheckSum(uint[] value) {
			bytesRead += tailLength;
			bitsCount += bytesRead << 3;
			if(tailLength > 0)
				Array.Copy(tail, 0, hashBuffer, index, tailLength);
			Reset();
			index = (int)((bitsCount >> 3) & 0x3f);
			int padLen = (index < 56) ? (56 - index) : (120 - index);
			byte[] bits = new byte[padLen + 8];
			bits[0] = (byte)0x80;
			for(int i = 0; i < 8; i++) {
				bits[padLen + i] = (byte)((bitsCount) >> (8 * i));
			}
			return UpdateCheckSum(value, bits, 0, bits.Length);
		}
		void Reset() {
			bytesRead = 0;
			position = 0;
			tailLength = 0;
			tail = new byte[blockLength];
		}
		#endregion
		static void Transform(byte[] block, int offset, uint[] hash) {
			uint[] decodedBlock = Decode(block, offset);
			TransformCore(decodedBlock, hash);
		}
		static void TransformCore(uint[] block, uint[] hash) {
			uint a = hash[0];
			uint b = hash[1];
			uint c = hash[2];
			uint d = hash[3];
			int count = 4;
			for(int i = 0; i < count; i++) {
				a = StepF(a, b, c, d, block[i * 4], stepFCoeffs[0]);
				d = StepF(d, a, b, c, block[i * 4 + 1], stepFCoeffs[1]);
				c = StepF(c, d, a, b, block[i * 4 + 2], stepFCoeffs[2]);
				b = StepF(b, c, d, a, block[i * 4 + 3], stepFCoeffs[3]);
			}
			for(int i = 0; i < count; i++) {
				a = StepG(a, b, c, d, block[i], stepGCoeffs[0]);
				d = StepG(d, a, b, c, block[i + 4], stepGCoeffs[1]);
				c = StepG(c, d, a, b, block[i + 8], stepGCoeffs[2]);
				b = StepG(b, c, d, a, block[i + 12], stepGCoeffs[3]);
			}
			for(int i = 0; i < count; i++) {
				int index = stepHIndices[i];
				a = StepH(a, b, c, d, block[index], stepHCoeffs[0]);
				d = StepH(d, a, b, c, block[index + 8], stepHCoeffs[1]);
				c = StepH(c, d, a, b, block[index + 4], stepHCoeffs[2]);
				b = StepH(b, c, d, a, block[index + 12], stepHCoeffs[3]);
			}
			hash[0] = unchecked(hash[0] + a);
			hash[1] = unchecked(hash[1] + b);
			hash[2] = unchecked(hash[2] + c);
			hash[3] = unchecked(hash[3] + d);
		}
		static uint[] Decode(byte[] input, int offset) {
			uint[] output = new uint[blockLength];
			for(int i = 0; i < 16; i++) {
				output[i] = ((uint)input[offset++] & 0xff) |
					(((uint)input[offset++] & 0xff) << 8) |
					(((uint)input[offset++] & 0xff) << 16) |
					(((uint)input[offset++] & 0xff) << 24);
			}
			return output;
		}
		static uint F(uint x, uint y, uint z) {
			return (x & y) | ((~x) & z);
		}
		static uint G(uint x, uint y, uint z) {
			return (x & y) | (x & z) | (y & z);
		}
		static uint H(uint x, uint y, uint z) {
			return x ^ y ^ z;
		}
		static uint StepF(uint a, uint b, uint c, uint d, uint x, int s) {
			a = unchecked(a + F(b, c, d) + x);
			return RotateLeftCircularly(a, s);
		}
		static uint StepG(uint a, uint b, uint c, uint d, uint x, int s) {
			a = unchecked(a + G(b, c, d) + x + rootOf2);
			return RotateLeftCircularly(a, s);
		}
		static uint StepH(uint a, uint b, uint c, uint d, uint x, int s) {
			a = unchecked(a + H(b, c, d) + x + rootOf3);
			return RotateLeftCircularly(a, s);
		}
		static uint RotateLeftCircularly(uint x, int s) {
			return (x << s) | (x >> (32 - s));
		}
	}
	#endregion
	#region MD4HashConverter
	[CLSCompliant(false)]
	public static class MD4HashConverter {
		public static byte[] ToArray(uint[] digest) {
			byte[] result = new byte[16];
			for(int i = 0; i < 4; i++) {
				Pack(result, i * 4, digest[i]);
			}
			return result;
		}
		static void Pack(byte[] destination, int offset, uint value) {
			for(int i = 0; i < 4; i++) {
				destination[offset + i] = (byte)((value >> 8 * i) & 0xff);
			}
		}
	}
	#endregion
}

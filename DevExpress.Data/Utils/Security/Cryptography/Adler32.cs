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
using System.Collections.Generic;
namespace DevExpress.Utils.Zip {
	public class Adler32 {
		public static long CalculateChecksum(IList<byte> values) {
			return new Adler32().Calculate(values, 0, values.Count);
		}
		const uint base_ = 65521;
		uint checksum = 1;
		public long Checksum { get { return checksum; } }
		public long Calculate(byte[] values) {
			return Calculate(values, 0, values.Length);
		}
		public long Calculate(int[] values) {
			for (int i = 0; i < values.Length; i++)
				Calculate(BitConverter.GetBytes(values[i]));
			return checksum;
		}
		internal static uint Update(uint checkSum, IList<byte> values, int offset, int length) {
			uint s1 = checkSum & 0xFFFF;
			uint s2 = checkSum >> 16;
			while (length > 0) {
				int n = 5552;
				if (n > length)
					n = length;
				length -= n;
				n += offset;
				while (offset < n) {
					s1 += values[offset++];
					s2 += s1;
				}
				s1 %= base_;
				s2 %= base_;
			}
			checkSum = (s2 << 16) | s1;
			return checkSum;
		}
		public long Calculate(IList<byte> values, int offset, int length) {
			checksum = Update(checksum, values, offset, length);
			return checksum;
		}
	}
	#region Adler32CheckSumCalculator
#if !WINRT
	[CLSCompliant(false)]
#endif
	public class Adler32CheckSumCalculator : ICheckSumCalculator<uint> {
		static Adler32CheckSumCalculator instance;
		public static Adler32CheckSumCalculator Instance {
			get {
				if (instance == null)
					instance = new Adler32CheckSumCalculator();
				return instance;
			}
		}
		#region ICheckSumCalculator<uint> Members
		public uint InitialCheckSumValue { get { return 1; } }
		public uint UpdateCheckSum(uint value, byte[] buffer, int offset, int count) {
			return Adler32.Update(value, buffer, offset, count);
		}
		public uint GetFinalCheckSum(uint value) {
			return value;
		}
		#endregion
	}
	#endregion
	#region Adler32Stream
#if !WINRT
	[CLSCompliant(false)]
#endif
	public class Adler32Stream : CheckSumStream<uint> {
		public Adler32Stream(Stream stream)
			: base(stream, Adler32CheckSumCalculator.Instance) {
		}
	}
	#endregion
}

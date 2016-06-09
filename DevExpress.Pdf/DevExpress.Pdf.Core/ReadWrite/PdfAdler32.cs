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

using System.Collections.Generic;
using System.IO;
namespace DevExpress.Pdf.Native {
	public class PdfAdler32 {
		const int adler32Base = 65521;
		const uint maxAdler = uint.MaxValue / 2;
		internal static void Calculate(IList<byte> values, Stream stream) {
			int s1 = 1;
			int s2 = 0;
			int length = values.Count;
			int i = 0;
			while (length > 0) {
				int k = length < 5552 ? length : 5552;
				length -= k;
				while (k >= 16) {
					s1 += values[i++]; 
					s2 += s1;
					s1 += values[i++]; 
					s2 += s1;
					s1 += values[i++]; 
					s2 += s1;
					s1 += values[i++]; 
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					s1 += values[i++];
					s2 += s1;
					k -= 16;
				}
				if (k != 0) {
					do {
						s1 += values[i++] & 0xff; s2 += s1;
					}
					while (--k != 0);
				}
				s1 %= adler32Base;
				s2 %= adler32Base;
			}
			stream.WriteByte((byte)((s2 & 0xff00) >> 8));
			stream.WriteByte((byte)(s2 & 0xff));
			stream.WriteByte((byte)((s1 & 0xff00) >> 8));
			stream.WriteByte((byte)(s1 & 0xff));
		}
		uint adler32s1 = 1;
		uint adler32s2 = 0;
		public void Add(byte value) {
			adler32s1 += value;
			adler32s2 += adler32s1;
			if (adler32s2 > maxAdler)
				Normalize();
		}
		public void Add(byte[] value) {
			int offset = 0;
			int count = value.Length;
			while (count > 0) {
				Normalize();
				int n = 5552;
				if (n > count)
					n = count;
				count -= n;
				n += offset;
				while (offset < n) {
					adler32s1 += value[offset++];
					adler32s2 += adler32s1;
				}
			}
		}
		public void Write(Stream stream) {
			Normalize();
			stream.WriteByte((byte)((adler32s2 & 0xff00) >> 8));
			stream.WriteByte((byte)(adler32s2 & 0xff));
			stream.WriteByte((byte)((adler32s1 & 0xff00) >> 8));
			stream.WriteByte((byte)(adler32s1 & 0xff));
		}
		void Normalize() {
			adler32s2 %= adler32Base;
			adler32s1 %= adler32Base;
		}
	}
}

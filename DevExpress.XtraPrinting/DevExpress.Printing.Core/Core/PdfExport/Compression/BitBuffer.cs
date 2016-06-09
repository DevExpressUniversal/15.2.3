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
using System.Collections;
using System.IO;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Export.Pdf.Compression {
	public class BitBuffer {
		ArrayList byteList = new ArrayList();
		const int ItemLength = 4096;
		int end;
		int bitCount;
		uint bits;
		public BitBuffer() {
			UpdateByteList();
		}
		void UpdateByteList() {
			byteList.Add(new byte[ItemLength]);
			end = 0;
		}
		void SetByte(byte value) {
			CurrentBytes[end++] = value;
			if(end >= CurrentBytes.Length)
				UpdateByteList();
		}
		byte[] GetBytes(int index) {
			return (byte[])byteList[index];
		}
		void TailToStream(Stream stream) {
			if(bitCount == 0) return;
			stream.WriteByte((byte)(bits));
			if(bitCount > 8)
				stream.WriteByte((byte)(bits >> 8));
		}
		public void WriteBits(int b, int count) {
			bits |= (uint)(b << bitCount);
			bitCount += count;
			if(bitCount >= 16) {				
				SetByte((byte)bits);
				SetByte((byte)(bits >> 8));
				bits >>= 16;
				bitCount -= 16;
			}
		}
		public void WriteShortMSB(int s) {
			SetByte((byte)(s >> 8));
			SetByte((byte)s);
		}
		public void AlignToByte() {
			if (bitCount > 0) {
				SetByte((byte)bits);
				if (bitCount > 8) {
					SetByte((byte)(bits >> 8));
				}
			}
			bits = 0;
			bitCount = 0;
		}
		public void ToStream(Stream stream) {
			if(Count == 0) return;
			for(int i = 0; i < Count - 1; i++) {
				byte[] buffer = GetBytes(i);
				stream.Write(buffer, 0, buffer.Length);
			}
			if(end > 0)
				stream.Write(CurrentBytes, 0, end);
			TailToStream(stream);
		}
		byte[] CurrentBytes { get { return (byte[])byteList[Count - 1]; } }
		int Count { get { return byteList.Count; } }
	}
}

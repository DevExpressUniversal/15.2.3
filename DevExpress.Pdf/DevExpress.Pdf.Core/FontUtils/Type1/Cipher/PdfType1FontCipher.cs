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
namespace DevExpress.Pdf.Native {
	public abstract class PdfType1FontCipher {
		readonly byte[] data;
		readonly int endPosition;
		int currentPosition;
		ushort r;
		protected virtual int SkipBytesCount { get { return 0; } }
		protected abstract int BytesPerResultByte { get; }
		protected abstract int R { get; }
		protected PdfType1FontCipher(byte[] data, int startPosition, int dataLength) {
			this.data = data;
			endPosition = startPosition + dataLength;
			currentPosition = startPosition;
			r = (ushort)R;
		}
		short DecodeNextChar() {
			short c = NextChar();
			if (c < 0)
				return c;
			byte result = (byte)(c ^ (r >> 8));
			r = (ushort)((c + r) * 52845 + 22719);
			return result;
		}
		public byte[] Decode() {
			List<byte> result = new List<byte>();
			int skipBytesCount = SkipBytesCount;
			for (;;) {
				short nextChar = DecodeNextChar();
				if (nextChar < 0)
					return result.ToArray();
				else if (skipBytesCount > 0)
					skipBytesCount--;
				else 
					result.Add((byte)nextChar);
			}
		}
		protected short NextByte() {
			return currentPosition >= endPosition ? (short)-1 : (short)data[currentPosition++];
		}
		protected abstract short NextChar();
	}
}

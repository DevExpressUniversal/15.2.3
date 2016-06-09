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

namespace DevExpress.Pdf.Native {
	public class JBIG2ArithmeticState {
		JBIG2StreamHelper helper;
		uint c;
		uint ct;
		uint a;
		uint buffer0;
		uint buffer1;
		public JBIG2ArithmeticState(JBIG2StreamHelper helper) {
			this.helper = helper;
			buffer0 = helper.ReadByte();
			buffer1 = helper.ReadByte();
			c = (buffer0 ^ 0xff) << 16;
			ReadByte();
			c = c << 7;
			ct -= 7;
			a = 0x80000000;
		}
		public JBIG2ArithmeticState(byte[] data)
			: this(new JBIG2StreamHelper(data)) {
		}
		void ReadByte() {
			if (buffer0 == 0xff) {
				if (buffer1 > 0x8f) {
					ct = 8;
				}
				else {
					buffer0 = buffer1;
					if (helper.Finish)
						PdfDocumentReader.ThrowIncorrectDataException();
					buffer1 = helper.ReadByte();
					c = c + 0xfe00 - (buffer0 << 9);
					ct = 7;
				}
			}
			else {
				buffer0 = buffer1;
				if (helper.Finish)
					PdfDocumentReader.ThrowIncorrectDataException();
				buffer1 = helper.ReadByte();
				c = c + 0xff00 - (buffer0 << 8);
				ct = 8;
			}
		}
		internal int Decode(byte[] cx, int index) {
			JBIG2ArithmeticQe pqe = JBIG2ArithmeticQe.Values[cx[index] >> 1];
			int mpsCX = cx[index] & 1;
			uint qe = pqe.Qe;
			a -= qe;
			bool cLessA = c < a;
			bool aLessQe = a < qe;
			int cLessABit = cLessA ? 1 : 0;
			int aLessQeBit = aLessQe ? 1 : 0;
			if (!cLessA) {
				c -= a;
				a = qe;
			}
			else if ((a & 0x80000000) != 0)
				return mpsCX;
			cx[index] = (byte)(cLessA ^ aLessQe ? pqe.MpsXor | mpsCX : pqe.LpsXor | mpsCX ^ pqe.Switch);
			Renormd();
			return mpsCX ^ (aLessQeBit ^ (~cLessABit & 1));
		}
		void Renormd() {
			do {
				if (ct == 0)
					ReadByte();
				a = a << 1;
				c = c << 1;
				ct--;
			} while ((a & 0x80000000) == 0);
		}
	}
}

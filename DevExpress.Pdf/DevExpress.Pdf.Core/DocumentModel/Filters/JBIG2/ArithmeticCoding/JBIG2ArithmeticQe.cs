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
	public struct JBIG2ArithmeticQe {
		public static JBIG2ArithmeticQe[] Values { get { return values; } }
		static readonly JBIG2ArithmeticQe[] values = new JBIG2ArithmeticQe[] {
										new JBIG2ArithmeticQe( 0x56010000,  1 << 1,  1 << 1, 1),
										new JBIG2ArithmeticQe( 0x34010000,  2 << 1,  6 << 1, 0),
										new JBIG2ArithmeticQe( 0x18010000,  3 << 1,  9 << 1, 0),
										new JBIG2ArithmeticQe( 0x0AC10000,  4 << 1, 12 << 1, 0),
										new JBIG2ArithmeticQe( 0x05210000,  5 << 1, 29 << 1, 0),
										new JBIG2ArithmeticQe( 0x02210000, 38 << 1, 33 << 1, 0),
										new JBIG2ArithmeticQe( 0x56010000,  7 << 1,  6 << 1, 1),
										new JBIG2ArithmeticQe( 0x54010000,  8 << 1, 14 << 1, 0),
										new JBIG2ArithmeticQe( 0x48010000,  9 << 1, 14 << 1, 0),
										new JBIG2ArithmeticQe( 0x38010000, 10 << 1, 14 << 1, 0),
										new JBIG2ArithmeticQe( 0x30010000, 11 << 1, 17 << 1, 0),
										new JBIG2ArithmeticQe( 0x24010000, 12 << 1, 18 << 1, 0),
										new JBIG2ArithmeticQe( 0x1C010000, 13 << 1, 20 << 1, 0),
										new JBIG2ArithmeticQe( 0x16010000, 29 << 1, 21 << 1, 0),
										new JBIG2ArithmeticQe( 0x56010000, 15 << 1, 14 << 1, 1),
										new JBIG2ArithmeticQe( 0x54010000, 16 << 1, 14 << 1, 0),
										new JBIG2ArithmeticQe( 0x51010000, 17 << 1, 15 << 1, 0),
										new JBIG2ArithmeticQe( 0x48010000, 18 << 1, 16 << 1 ,0),
										new JBIG2ArithmeticQe( 0x38010000, 19 << 1, 17 << 1, 0),
										new JBIG2ArithmeticQe( 0x34010000, 20 << 1, 18 << 1, 0),
										new JBIG2ArithmeticQe( 0x30010000, 21 << 1, 19 << 1, 0),
										new JBIG2ArithmeticQe( 0x28010000, 22 << 1, 19 << 1, 0),
										new JBIG2ArithmeticQe( 0x24010000, 23 << 1, 20 << 1, 0),
										new JBIG2ArithmeticQe( 0x22010000, 24 << 1, 21 << 1, 0),
										new JBIG2ArithmeticQe( 0x1C010000, 25 << 1, 22 << 1, 0),
										new JBIG2ArithmeticQe( 0x18010000, 26 << 1, 23 << 1, 0),
										new JBIG2ArithmeticQe( 0x16010000, 27 << 1, 24 << 1, 0),
										new JBIG2ArithmeticQe( 0x14010000, 28 << 1, 25 << 1, 0),
										new JBIG2ArithmeticQe( 0x12010000, 29 << 1, 26 << 1, 0),
										new JBIG2ArithmeticQe( 0x11010000, 30 << 1, 27 << 1, 0),
										new JBIG2ArithmeticQe( 0x0AC10000, 31 << 1, 28 << 1, 0),
										new JBIG2ArithmeticQe( 0x09C10000, 32 << 1, 29 << 1, 0),
										new JBIG2ArithmeticQe( 0x08A10000, 33 << 1, 30 << 1, 0),
										new JBIG2ArithmeticQe( 0x05210000, 34 << 1, 31 << 1, 0),
										new JBIG2ArithmeticQe( 0x04410000, 35 << 1, 32 << 1, 0),
										new JBIG2ArithmeticQe( 0x02A10000, 36 << 1, 33 << 1, 0),
										new JBIG2ArithmeticQe( 0x02210000, 37 << 1, 34 << 1, 0),
										new JBIG2ArithmeticQe( 0x01410000, 38 << 1, 35 << 1, 0),
										new JBIG2ArithmeticQe( 0x01110000, 39 << 1, 36 << 1, 0),
										new JBIG2ArithmeticQe( 0x00850000, 40 << 1, 37 << 1, 0),
										new JBIG2ArithmeticQe( 0x00490000, 41 << 1, 38 << 1, 0),
										new JBIG2ArithmeticQe( 0x00250000, 42 << 1, 39 << 1, 0),
										new JBIG2ArithmeticQe( 0x00150000, 43 << 1, 40 << 1, 0),
										new JBIG2ArithmeticQe( 0x00090000, 44 << 1, 41 << 1, 0),
										new JBIG2ArithmeticQe( 0x00050000, 45 << 1, 42 << 1, 0),
										new JBIG2ArithmeticQe( 0x00010000, 45 << 1, 43 << 1, 0),
										new JBIG2ArithmeticQe( 0x56010000, 46 << 1, 46 << 1, 0)
										};
		private uint qe;
		private byte mpsXor;
		private byte lpsXor;
		private byte sw;
		internal uint Qe { get { return qe; } }
		internal byte MpsXor { get { return mpsXor; } } 
		internal byte LpsXor { get { return lpsXor; } } 
		internal byte Switch { get { return sw; } }
		JBIG2ArithmeticQe(uint qe, byte mpsXor, byte lpsXor, byte sw)
			: this() {
			this.qe = qe;
			this.mpsXor = mpsXor;
			this.lpsXor = lpsXor;
			this.sw = sw;
		}
	}
}

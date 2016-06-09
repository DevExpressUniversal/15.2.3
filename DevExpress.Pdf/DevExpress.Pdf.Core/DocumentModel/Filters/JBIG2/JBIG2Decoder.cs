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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Pdf.Native {
	public class JBIG2Decoder {
		static int FindLength(int tmp) {
			int i = 0;
			for (i = 0; (1 << i) < tmp; i++) ;
			return i;
		}
		public static JBIG2Decoder Create(JBIG2StreamHelper sh, int maxId) {
			return new JBIG2Decoder(sh, FindLength(maxId), 16, 16);
		}
		readonly JBIG2ArithmeticContext gb;
		readonly JBIG2ArithmeticContext gr;
		readonly JBIG2ArithmeticContext iaid;
		readonly JBIG2ArithmeticIntContext iardw = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iardh = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iardx = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iardy = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iadh = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iadw = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iaex = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iaai = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iadt = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iafs = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iads = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iait = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticIntContext iari = new JBIG2ArithmeticIntContext();
		readonly JBIG2ArithmeticState arithState;
		bool lastCode;
		public bool LastCode { get { return lastCode; } }
		public JBIG2Decoder(JBIG2StreamHelper sh, int idLength, int gbLength, int grLength) {
			arithState = new JBIG2ArithmeticState(sh);
			iaid = new JBIG2ArithmeticContext(idLength);
			gb = new JBIG2ArithmeticContext(gbLength);
			gr = new JBIG2ArithmeticContext(grLength);
		}
		public int DecodeDT() { return Decode(iadt); }
		public int DecodeFS() { return Decode(iafs); }
		public int DecodeDS() { return Decode(iads); }
		public int DecodeIT() { return Decode(iait); }
		public int DecodeRI() { return Decode(iari); }
		public int DecodeRDW() { return Decode(iardw); }
		public int DecodeRDH() { return Decode(iardh); }
		public int DecodeRDX() { return Decode(iardx); }
		public int DecodeRDY() { return Decode(iardy); }
		public int DecodeDH() { return Decode(iadh); }
		public int DecodeDW() { return Decode(iadw); }
		public int DecodeAI() { return Decode(iaai); }
		public int DecodeEX() { return Decode(iaex); }
		public int DecodeID() {
			lastCode = false;
			return iaid.Decode(arithState);
		}
		public bool DecodeGB(int context) {
			return gb.DecodeBit(arithState, context);
		}
		public bool DecodeGR(int context) {
			return gr.DecodeBit(arithState, context);
		}
		int Decode(JBIG2ArithmeticIntContext ctx) {
			JBIG2ArithmeticDecoderResult result = ctx.Decode(arithState);
			lastCode = result.Code;
			return result.Result;
		}
	}
}

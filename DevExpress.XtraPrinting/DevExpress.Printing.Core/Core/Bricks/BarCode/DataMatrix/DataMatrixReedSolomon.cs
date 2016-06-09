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
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraPrinting.BarCode.Native {
	class ECC200ReedSolomon {
		int gfPoly;
		int symSize;
		int logmod;
		int rlen;
		int[] log = null;
		int[] alog = null;
		int[] rspoly = null;
		public ECC200ReedSolomon(int poly, int nsym, int index) {
			InitGf(poly);
			InitCode(nsym, index);
		}
		public void InitGf(int poly) {
			int m, b;
			for(b = 1, m = 0; b <= poly; b <<= 1) m++;
			b >>= 1;
			m--;
			gfPoly = poly;
			symSize = m;
			logmod = (1 << m) - 1;
			log = new int[logmod + 1];
			alog = new int[logmod];
			for(int p = 1, v = 0; v < logmod; v++) {
				alog[v] = p;
				log[p] = v;
				p <<= 1;
				if((p & b) != 0) p ^= poly;
			}
		}
		public void InitCode(int nsym, int index) {
			rspoly = new int[nsym + 1];
			rlen = nsym;
			rspoly[0] = 1;
			for(int i = 1; i <= nsym; i++) {
				rspoly[i] = 1;
				for(int k = i - 1; k > 0; k--) {
					if(rspoly[k] != 0)
						rspoly[k] =
							alog[(log[rspoly[k]] + index) % logmod];
					rspoly[k] ^= rspoly[k - 1];
				}
				rspoly[0] = alog[(log[rspoly[0]] + index) % logmod];
				index++;
			}
		}
		public void Encode(byte[] data, int len, out byte[] res) {
			res = new byte[rlen];
			for(int i = 0; i < rlen; i++) res[i] = 0;
			for(int i = 0; i < len; i++) {
				int m = res[rlen - 1] ^ data[i];
				for(int k = rlen - 1; k > 0; k--) {
					if(m != 0 && rspoly[k] != 0)
						res[k] = (byte)(res[k - 1] ^ alog[(log[m] + log[rspoly[k]]) % logmod]);
					else
						res[k] = res[k - 1];
				}
				if(m != 0 && rspoly[0] != 0)
					res[0] = (byte)alog[(log[m] + log[rspoly[0]]) % logmod];
				else
					res[0] = 0;
			}
		}
	}
}

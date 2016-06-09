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
	public class JBIG2ArithmeticIntContext {
		byte[] iax = new byte[512];
		public JBIG2ArithmeticDecoderResult Decode(JBIG2ArithmeticState state) {
			int prev = 1;
			int s, v;
			int bit;
			int n_tail, offset;
			int i;
			s = state.Decode(iax, prev);
			prev = (prev << 1) | s;
			bit = state.Decode(iax, prev);
			prev = (prev << 1) | bit;
			if (bit != 0) {
				bit = state.Decode(iax, prev);
				prev = (prev << 1) | bit;
				if (bit != 0) {
					bit = state.Decode(iax, prev);
					prev = (prev << 1) | bit;
					if (bit != 0) {
						bit = state.Decode(iax, prev);
						prev = (prev << 1) | bit;
						if (bit != 0) {
							bit = state.Decode(iax, prev);
							prev = (prev << 1) | bit;
							if (bit != 0) {
								n_tail = 32;
								offset = 4436;
							}
							else {
								n_tail = 12;
								offset = 340;
							}
						}
						else {
							n_tail = 8;
							offset = 84;
						}
					}
					else {
						n_tail = 6;
						offset = 20;
					}
				}
				else {
					n_tail = 4;
					offset = 4;
				}
			}
			else {
				n_tail = 2;
				offset = 0;
			}
			v = 0;
			for (i = 0; i < n_tail; i++) {
				bit = state.Decode(iax, prev);
				prev = ((prev << 1) & 511) | (prev & 256) | bit;
				v = (v << 1) | bit;
			}
			v += offset;
			v = s != 0 ? -v : v;
			return new JBIG2ArithmeticDecoderResult(v, s != 0 && v == 0);
		}
	}
}

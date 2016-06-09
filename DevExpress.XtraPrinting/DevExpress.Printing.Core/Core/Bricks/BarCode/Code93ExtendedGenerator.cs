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
using System.Text;
using DevExpress.XtraPrinting.BarCode.Native;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.BarCode {
	public class Code93ExtendedGenerator : Code93Generator {
		static string validCharSet = charSetAll;
		static Hashtable substituteTable = new Hashtable();
		#region static
		static Code93ExtendedGenerator() {
			substituteTable[(char)0x00] = ":U";
			substituteTable[(char)0x01] = "?A";
			substituteTable[(char)0x02] = "?B";
			substituteTable[(char)0x03] = "?C";
			substituteTable[(char)0x04] = "?D";
			substituteTable[(char)0x05] = "?E";
			substituteTable[(char)0x06] = "?F";
			substituteTable[(char)0x07] = "?G";
			substituteTable[(char)0x08] = "?H";
			substituteTable[(char)0x09] = "?I";
			substituteTable[(char)0x0A] = "?J";
			substituteTable[(char)0x0B] = "?K";
			substituteTable[(char)0x0C] = "?L";
			substituteTable[(char)0x0D] = "?M";
			substituteTable[(char)0x0E] = "?N";
			substituteTable[(char)0x0F] = "?O";
			substituteTable[(char)0x10] = "?P";
			substituteTable[(char)0x11] = "?Q";
			substituteTable[(char)0x12] = "?R";
			substituteTable[(char)0x13] = "?S";
			substituteTable[(char)0x14] = "?T";
			substituteTable[(char)0x15] = "?U";
			substituteTable[(char)0x16] = "?V";
			substituteTable[(char)0x17] = "?W";
			substituteTable[(char)0x18] = "?X";
			substituteTable[(char)0x19] = "?Y";
			substituteTable[(char)0x1A] = "?Z";
			substituteTable[(char)0x1B] = ":A";
			substituteTable[(char)0x1C] = ":B";
			substituteTable[(char)0x1D] = ":C";
			substituteTable[(char)0x1E] = ":D";
			substituteTable[(char)0x1F] = ":E";
			substituteTable['!'] = "!A";
			substituteTable['"'] = "!B";
			substituteTable['#'] = "!C";
			substituteTable['$'] = "!D"; 
			substituteTable['%'] = "!E"; 
			substituteTable['&'] = "!F";
			substituteTable['\''] = "!G";
			substituteTable['('] = "!H";
			substituteTable[')'] = "!I";
			substituteTable['*'] = "!J";
			substituteTable['+'] = "!K"; 
			substituteTable[','] = "!L";
			substituteTable['/'] = "!O"; 
			substituteTable[':'] = "!Z";
			substituteTable[';'] = ":F";
			substituteTable['<'] = ":G";
			substituteTable['='] = ":H";
			substituteTable['>'] = ":I";
			substituteTable['?'] = ":J";
			substituteTable['@'] = ":V";
			substituteTable['['] = ":K";
			substituteTable['\\'] = ":L";
			substituteTable[']'] = ":M";
			substituteTable['^'] = ":N";
			substituteTable['_'] = ":O";
			substituteTable['`'] = ":W";
			substituteTable['a'] = "~A";
			substituteTable['b'] = "~B";
			substituteTable['c'] = "~C";
			substituteTable['d'] = "~D";
			substituteTable['e'] = "~E";
			substituteTable['f'] = "~F";
			substituteTable['g'] = "~G";
			substituteTable['h'] = "~H";
			substituteTable['i'] = "~I";
			substituteTable['j'] = "~J";
			substituteTable['k'] = "~K";
			substituteTable['l'] = "~L";
			substituteTable['m'] = "~M";
			substituteTable['n'] = "~N";
			substituteTable['o'] = "~O";
			substituteTable['p'] = "~P";
			substituteTable['q'] = "~Q";
			substituteTable['r'] = "~R";
			substituteTable['s'] = "~S";
			substituteTable['t'] = "~T";
			substituteTable['u'] = "~U";
			substituteTable['v'] = "~V";
			substituteTable['w'] = "~W";
			substituteTable['x'] = "~X";
			substituteTable['y'] = "~Y";
			substituteTable['z'] = "~Z";
			substituteTable['{'] = ":P";
			substituteTable['|'] = ":Q";
			substituteTable['}'] = ":R";
			substituteTable['~'] = ":S";
			substituteTable[(char)0x7F] = ":T";
		}
		static string PerformSubstitutions(string text) {
			int count = text.Length;
			StringBuilder source = new StringBuilder(text);
			StringBuilder target = new StringBuilder(count);
			for(int i = 0; i < count; i++) {
				string subst = substituteTable[source[i]] as string;
				if(subst != null)
					target.Append(subst);
				else
					target.Append(source[i]);
			}
			return target.ToString();
		}
		#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("Code93ExtendedGeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.Code93Extended;
			}
		}
		public Code93ExtendedGenerator() {
		}
		Code93ExtendedGenerator(Code93ExtendedGenerator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new Code93ExtendedGenerator(this);
		}
		protected override char[] PrepareText(string text) {
			return base.PrepareText(PerformSubstitutions(text));
		}
		protected override string GetValidCharSet() { return validCharSet; }
	}
}

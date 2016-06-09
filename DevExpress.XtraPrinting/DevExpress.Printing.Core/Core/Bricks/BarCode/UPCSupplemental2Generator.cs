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
using DevExpress.XtraPrinting.BarCode.Native;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.BarCode {
	public class UPCSupplemental2Generator : UPCSupplementalNGeneratorBase {
		static string[] parityString = new string[] { "00", "0a", "a0", "aa" };
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("UPCSupplemental2GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.UPCSupplemental2; }
		}
		public UPCSupplemental2Generator() {
		}
		UPCSupplemental2Generator(UPCSupplemental2Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new UPCSupplemental2Generator(this);
		}
		protected override int CalcCheckDigit(string text) {
			System.Diagnostics.Debug.Assert(text.Length == GetMaxCharsCount());
			int sum = Char2Int(text[0]) * 10 + Char2Int(text[1]);
			return sum % 4;
		}
		protected override int GetMaxCharsCount() {
			return 2;
		}
		protected override string GetParityString(int checkDigit) {
			return parityString[checkDigit];
		}
	}
}

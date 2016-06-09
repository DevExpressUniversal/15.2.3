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
	public class UPCSupplemental5Generator : UPCSupplementalNGeneratorBase {
		static string[] parityString = new string[] {
					"aa000",
					"a0a00",
					"a00a0",
					"a000a",
					"0aa00",
					"00aa0",
					"000aa",
					"0a0a0",
					"0a00a",
					"00a0a"
				};
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("UPCSupplemental5GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.UPCSupplemental5; }
		}
		public UPCSupplemental5Generator() {
		}
		UPCSupplemental5Generator(UPCSupplemental5Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new UPCSupplemental5Generator(this);
		}
		protected override int CalcCheckDigit(string text) {
			int count = text.Length;
			System.Diagnostics.Debug.Assert(count == GetMaxCharsCount());
			int sum = 0;
			for(int i = 0; i < count; i++) {
				int weight = (i % 2 == 0) ? 3 : 9;
				sum += weight * Char2Int(text[i]);
			}
			return sum % 10;
		}
		protected override int GetMaxCharsCount() {
			return 5;
		}
		protected override string GetParityString(int checkDigit) {
			return parityString[checkDigit];
		}
	}
}

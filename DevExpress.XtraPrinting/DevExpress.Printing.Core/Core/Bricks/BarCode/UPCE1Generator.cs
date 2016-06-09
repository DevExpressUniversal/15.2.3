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
using DevExpress.XtraPrinting.BarCode.Native;
using System.ComponentModel;
namespace DevExpress.XtraPrinting.BarCode {
	public class UPCE1Generator : UPCEGeneratorBase {
		static Hashtable parityTable = new Hashtable();
		static UPCE1Generator() {
			parityTable['0'] = "000aaa"; 
			parityTable['1'] = "00a0aa"; 
			parityTable['2'] = "00aa0a"; 
			parityTable['3'] = "00aaa0"; 
			parityTable['4'] = "0a00aa"; 
			parityTable['5'] = "0aa00a"; 
			parityTable['6'] = "0aaa00"; 
			parityTable['7'] = "0a0a0a"; 
			parityTable['8'] = "0a0aa0"; 
			parityTable['9'] = "0aa0a0"; 
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("UPCE1GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get {
				return BarCodeSymbology.UPCE1;
			}
		}
		public UPCE1Generator() {
		}
		protected UPCE1Generator(UPCE1Generator source)
			: base(source) {
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new UPCE1Generator(this);
		}
		protected override string GetParityString(char checkDigit) {
			return (string)parityTable[checkDigit];
		}
		protected override char GetNumberSystemDigit() {
			return '1';
		}
	}
}

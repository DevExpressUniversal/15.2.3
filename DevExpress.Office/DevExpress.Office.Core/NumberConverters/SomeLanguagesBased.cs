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
using DevExpress.Office.Utils;
namespace DevExpress.Office.NumberConverters {
	#region SomeLanguagesBase (abstract class)
	public abstract class SomeLanguagesBased : DescriptiveNumberConverterBase {
		#region Properties
		protected internal bool FlagIntegerThousand { get; set; }
		protected internal bool FlagIntegerMillion { get; set; }
		protected internal bool FlagIntegerBillion { get; set; }
		protected internal bool FlagIntegerTrillion { get; set; }
		protected internal bool FlagIntegerQuadrillion { get; set; }
		protected internal bool FlagIntegerQuintillion { get; set; }
		#endregion
		public override string ConvertNumberCore(long value) {
			FlagIntegerThousand = false;
			FlagIntegerMillion = false;
			FlagIntegerBillion = false;
			FlagIntegerTrillion = false;
			FlagIntegerQuadrillion = false;
			FlagIntegerQuintillion = false;
			return base.ConvertNumberCore(value);
		}
		protected internal override void GenerateDigitsInfo(DigitInfoCollection digits, long value) {
			if (value / 1000000000000000000 != 0) {
				FlagIntegerQuintillion = (value % 1000000000000000000 == 0);
				GenerateQuintillionDigits(digits, value / 1000000000000000000);
			}
			value %= 1000000000000000000;
			if (value / 1000000000000000 != 0) {
				FlagIntegerQuadrillion = (value % 1000000000000000 == 0);
				GenerateQuadrillionDigits(digits, value / 1000000000000000);
			}
			value %= 1000000000000000;
			if (value / 1000000000000 != 0) {
				FlagIntegerTrillion = (value % 1000000000000 == 0);
				GenerateTrillionDigits(digits, value / 1000000000000);
			}
			value %= 1000000000000;
			if (value / 1000000000 != 0) {
				FlagIntegerBillion = (value % 1000000000 == 0);
				GenerateBillionDigits(digits, value / 1000000000);
				value %= 1000000000;
			}
			if (value / 1000000 != 0) {
				FlagIntegerMillion = (value % 1000000 == 0);
				GenerateMillionDigits(digits, value / 1000000);
			}
			value %= 1000000;
			if (value / 1000 != 0) {
				FlagIntegerThousand = (value % 1000 == 0);
				GenerateThousandDigits(digits, value / 1000);
			}
			value %= 1000;
			if (value / 100 != 0)
				GenerateHundredDigits(digits, value / 100);
			value %= 100;
			if (value == 0)
				return;
			if (value >= 20)
				GenerateTenthsDigits(digits, value);
			else {
				if (value >= 10)
					GenerateTeensDigits(digits, value % 10);
				else
					GenerateSinglesDigits(digits, value);
			}
		}
	}
	#endregion
}

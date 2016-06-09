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
using System.Numerics;
using DevExpress.XtraSpreadsheet.Model;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Utils {
	#region SpreadsheetComplex
	public struct SpreadsheetComplex {
		public Complex Value { get; set; }
		public char Suffix { get; set; }
		public override string ToString() {
			double imaginary = Value.Imaginary;
			if (imaginary < 0)
				return Value.Real.ToString() + imaginary.ToString() + Suffix;
			else
				return Value.Real.ToString() + "+" + imaginary.ToString() + Suffix;
		}
		public static bool TryParse(string value, WorkbookDataContext context, out SpreadsheetComplex result) {
			return context.ComplexNumberParser.TryParse(value, context.GetDecimalSymbol(), out result);
		}
		public VariantValue ToVariantValue(WorkbookDataContext context) {
			Complex val = Value;
			if (val.Imaginary == 0.0) {
				VariantValue realValue = val.Real;
				return realValue.ToText(context);
			}
			string imaginaryPart;
			if (val.Imaginary == 1.0)
				imaginaryPart = Suffix.ToString();
			else if(val.Imaginary == -1.0)
				imaginaryPart = "-" + Suffix.ToString();
			else {
				VariantValue imaginary = val.Imaginary;
				imaginaryPart = imaginary.ToText(context).InlineTextValue + Suffix;
			}
			if (val.Real == 0.0)
				return imaginaryPart;
			else {
				VariantValue real = val.Real;
				if (val.Imaginary < 0)
					return real.ToText(context).InlineTextValue + imaginaryPart;
				else
					return real.ToText(context).InlineTextValue + "+" + imaginaryPart;
			}
		}
	}
	#endregion
}

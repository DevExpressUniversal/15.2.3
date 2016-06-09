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

using System.Numerics;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionImDiv
	public class FunctionImDiv : WorksheetFunctionTwoComplexArgumentsBase {
		public override string Name { get { return "IMDIV"; } }
		public override int Code { get { return 0x018D; } }
		protected override VariantValue EvaluateArguments(SpreadsheetComplex argument1, SpreadsheetComplex argument2, WorkbookDataContext context) {
			if(argument2.Value == Complex.Zero)
				return VariantValue.ErrorNumber;
			char suffix = GetConsistentSuffix(argument1, argument2);
			if(suffix == '\0')
				return VariantValue.ErrorNumber;
			Complex complexResult = Complex.Divide(argument1.Value, argument2.Value);
			SpreadsheetComplex result = new SpreadsheetComplex() { Value = complexResult, Suffix = suffix };
			return result.ToVariantValue(context); ;
		}
	}
	#endregion
}

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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public enum TriStateFlowControlValue { Break, Continue, Fail };
	#region FunctionMultiArgImResult
	public abstract class FunctionMultiArgImResult : FunctionResult {
		#region Fields
		SpreadsheetComplex result;
		const char defaultSuffixValue = 'i';
		const char unknownSuffixValue = '\0';
		#endregion
		protected FunctionMultiArgImResult(WorkbookDataContext context)
			: base(context) {
				result = new SpreadsheetComplex() { Value = new Complex(), Suffix = unknownSuffixValue };
		}
		protected SpreadsheetComplex Result { get { return result; } set { result = value; } }
		public override bool ProcessSingleValue(VariantValue value) {
			if(value.IsEmpty) {
				value = 1;
				switch(EmptyValueAction()) {
					case TriStateFlowControlValue.Break:
						return true;
					case TriStateFlowControlValue.Continue:
						break;
					case TriStateFlowControlValue.Fail:
						return false;
					default:
						Exceptions.ThrowInvalidOperationException("EmptyValueAction() yield incorrect value");
						break;
				}
			}
			if(value.IsError) {
				Error = value;
				return false;
			}
			SpreadsheetComplex arg;
			VariantValue argument = Context.GetTextComplexValue(value);
			if(argument.IsError) {
				Error = argument;
				return false;
			}
			if(!SpreadsheetComplex.TryParse(argument.InlineTextValue, Context, out arg)) {
				Error = VariantValue.ErrorNumber;
				return false;
			}
			if(arg.Value.Imaginary == 0)
				arg.Suffix = unknownSuffixValue;
			if(result.Suffix == unknownSuffixValue)
				result.Suffix = arg.Suffix;
			else if((arg.Suffix != unknownSuffixValue) && (result.Suffix != arg.Suffix)) {
				Error = VariantValue.ErrorInvalidValueInFunction;
				return false;
			}
			return PerformAction(arg);
		}
		protected void SetResultValue(Complex value) {
			result.Value = value;
		}
		protected abstract bool PerformAction(SpreadsheetComplex argument);
		protected abstract TriStateFlowControlValue EmptyValueAction();
		public override bool ShouldProcessValueCore(VariantValue value) {
			return true;
		}
		public override bool EndArrayProcessingCore() { return !Error.IsError; }
		public override VariantValue ConvertValue(VariantValue value) {
			throw new System.InvalidOperationException("This method should never be called");
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			throw new System.InvalidOperationException("This method should never be called");
		}
		public override VariantValue GetFinalValue() {
			if(result.Suffix == unknownSuffixValue)
				result.Suffix = defaultSuffixValue;
			return result.ToVariantValue(Context);
		}
	}
	#endregion
}

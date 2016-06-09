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

using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionsUsingYearFracBase
	public abstract class FunctionsUsingYearFracBase : FunctionYearFracBase {
		#region Static Members
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			return collection;
		}
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		#endregion
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue basis;
			int basisId = 0;
			if (arguments.Count > 4) {
				basis = GetBasis(arguments[4], context);
				if (basis.IsError)
					return basis;
				basisId = (int)basis.NumericValue;
			}
			VariantValue settlement = DateToSerialNumber(arguments[0], context);
			if (settlement.IsError)
				return settlement;
			VariantValue maturity = DateToSerialNumber(arguments[1], context);
			if (maturity.IsError)
				return maturity;
			VariantValue argument1 = context.ToNumericWithoutCrossing(arguments[2]);
			if (argument1.IsError)
				return argument1;
			VariantValue argument2 = context.ToNumericWithoutCrossing(arguments[3]);
			if (argument2.IsError)
				return argument2;
			return GetResult((int)settlement.NumericValue, (int)maturity.NumericValue, argument1.NumericValue, argument2.NumericValue, basisId, context);
		}
		protected VariantValue GetResult(int settlementSerialNumber, int maturitySerialNumber, double argument1, double argument2, int basisId, WorkbookDataContext context) {
			DayCountBasisBase basis = DayCountBasisFactory.GetBasis(basisId);
			int maxSerialNumber = basis.GetMaxDateTimeSerialNumber(context.DateSystem);
			double yearFrac = basis.GetYearFrac(settlementSerialNumber, maturitySerialNumber, context);
			if (CheckValid(settlementSerialNumber, maturitySerialNumber, maxSerialNumber, argument1, argument2) || 
				AdditionalConditions(settlementSerialNumber, maturitySerialNumber, argument1, argument2, yearFrac)) 
				return VariantValue.ErrorNumber;
			return GetFinalResult(argument1, argument2, yearFrac);
		}
		#region Internal
		bool CheckValid(int settlementSerialNumber, int maturitySerialNumber, int maxSerialNumber, double argument1, double argument2) {
			return maturitySerialNumber < settlementSerialNumber || argument1 <= 0 || argument2 <= 0 ||
				   settlementSerialNumber > maxSerialNumber || maturitySerialNumber > maxSerialNumber;
		}
		protected virtual bool AdditionalConditions(int settlementSerialNumber, int maturitySerialNumber, double argument1, double argument2, double yearFrac) {
			return maturitySerialNumber == settlementSerialNumber;
		}
		protected abstract VariantValue GetFinalResult(double argument1, double argument2, double yearFrac);
		#endregion
	}
	#endregion
	#region FunctionDisc
	public class FunctionDisc : FunctionsUsingYearFracBase {
		#region Properties
		public override string Name { get { return "DISC"; } }
		public override int Code { get { return 0x01B3; } }
		#endregion
		protected override VariantValue GetFinalResult(double price, double redemption, double yearFrac) {
			return (redemption - price) / (redemption * yearFrac);
		}
	}
	#endregion
}

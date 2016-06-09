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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionsUsingYearFracBase
	public abstract class FunctionYearFracBase : FunctionSerialNumberBase {
		#region Static Members
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference, FunctionParameterOption.NonRequired));
			return collection;
		}
		#endregion
		#region Properties
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue basis;
			int basisId = 0;
			if (arguments.Count > 2) {
				basis = GetBasis(arguments[2], context);
				if (basis.IsError)
					return basis;
				basisId = (int)basis.NumericValue;
			}
			VariantValue startDate = DateToSerialNumber(arguments[0], context);
			if (startDate.IsError)
				return startDate;
			VariantValue endDate = DateToSerialNumber(arguments[1], context);
			if (endDate.IsError)
				return endDate;
			return GetResult((int)startDate.NumericValue, (int)endDate.NumericValue, basisId, context);
		}
		VariantValue GetResult(int startDateValue, int endDateValue, int basisId, WorkbookDataContext context) {
			DayCountBasisBase basis = DayCountBasisFactory.GetBasis(basisId);
			int maxSerialNumber = basis.GetMaxDateTimeSerialNumber(context.DateSystem);
			if (startDateValue > maxSerialNumber || endDateValue > maxSerialNumber)
				return VariantValue.ErrorNumber;
			return basis.GetYearFrac(Math.Min(startDateValue, endDateValue), Math.Max(startDateValue, endDateValue), context);
		}
	}
	#endregion
	#region FunctionYearFrac
	public class FunctionYearFrac : FunctionYearFracBase {
		#region Properties
		public override string Name { get { return "YEARFRAC"; } }
		public override int Code { get { return 0x01C3; } }
		#endregion
	}
	#endregion
}

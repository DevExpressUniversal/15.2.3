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

using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionRankDotAvg
	public class FunctionRankDotAvg : FunctionRank {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "RANK.AVG"; } }
		public override int Code { get { return 0x404F; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionRankDotAvgResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
	#region FunctionPercentRankResult
	public class FunctionRankDotAvgResult : FunctionRankResult {
		int numbersCount;
		int result = 1;
		bool isContainsNumber;
		int sameNumbersCount = 0;
		public FunctionRankDotAvgResult(WorkbookDataContext context)
			: base(context) {
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			numbersCount++;
			double doubleValue = value.NumericValue;
			if (doubleValue == Number) {
				isContainsNumber = true;
				sameNumbersCount++;
			}
			if (IsAscendingSortOrder && doubleValue > Number)
				result++;
			if (!IsAscendingSortOrder && doubleValue < Number)
				result++;
			return true;
		}
		public override VariantValue GetFinalValue() {
			int sameNumberRankSum = 0;
			if (!isContainsNumber || numbersCount == 0)
				return VariantValue.ErrorValueNotAvailable;
			if (sameNumbersCount > 1) {
				for (int i = 0; i < sameNumbersCount; i++)
					sameNumberRankSum += result + i;
				return (double)sameNumberRankSum / (double)sameNumbersCount;
			}
			return result;
		}
	}
	#endregion
}

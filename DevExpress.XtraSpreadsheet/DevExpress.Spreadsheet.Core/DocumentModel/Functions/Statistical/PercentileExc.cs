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
	#region FunctionPercentileExc
	public class FunctionPercentileExc : FunctionPercentile {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "PERCENTILE.EXC"; } }
		public override int Code { get { return 0x4049; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionPercentileExcResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Array | OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
	#endregion
	#region FunctionPercentileResult
	public class FunctionPercentileExcResult : FunctionPercentileResult {
		public FunctionPercentileExcResult(WorkbookDataContext context)
			: base(context) {
		}
		public override VariantValue GetFinalValue() {
			if (ListNumbers.Count == 0)
				return VariantValue.ErrorNumber;
			ListNumbers.Sort();
			int count = ListNumbers.Count;
			ListNumbers.Add(ListNumbers[count - 1] + 1);
			ListNumbers.Insert(0, ListNumbers[0] - 1);
			int lastIndex = count + 1;
			if (Percentile < 1.0 / lastIndex || Percentile > (double)(lastIndex - 1) / lastIndex)
				return VariantValue.ErrorNumber;
			return GetFinalValueCore(lastIndex);
		}
	}
	#endregion
}

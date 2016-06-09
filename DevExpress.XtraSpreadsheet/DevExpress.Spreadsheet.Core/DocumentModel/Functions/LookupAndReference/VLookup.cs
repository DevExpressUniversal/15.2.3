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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using System.Text.RegularExpressions;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionVLookup
	public class FunctionVLookup : FunctionOrderedLookupBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "VLOOKUP"; } }
		public override int Code { get { return 0x0066; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value | OperandDataType.Array; } }
		protected internal override int CalculateSecondaryDimension(VariantValue tableArray) {
			if (tableArray.IsCellRange)
				return tableArray.CellRangeValue.Width;
			else
				return tableArray.ArrayValue.Width;
		}
		protected internal override IVector<VariantValue> GetLookupVector(VariantValue tableArray) {
			if (tableArray.IsCellRange)
				return new RangeVerticalVector(tableArray.CellRangeValue.GetFirstInnerCellRange(), 0);
			else
				return new ArrayVerticalVector(tableArray.ArrayValue, 0);
		}
		protected internal override IVector<VariantValue> GetResultVector(VariantValue tableArray, int secondaryDirectionIndex) {
			if (tableArray.IsCellRange)
				return new RangeVerticalVector(tableArray.CellRangeValue.GetFirstInnerCellRange(), secondaryDirectionIndex);
			else
				return new ArrayVerticalVector(tableArray.ArrayValue, secondaryDirectionIndex);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value, FakeParameterType.Any));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array, FakeParameterType.Number));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array, FakeParameterType.Number));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired, FakeParameterType.Logical));
			return collection;
		}
	}
	#endregion
}

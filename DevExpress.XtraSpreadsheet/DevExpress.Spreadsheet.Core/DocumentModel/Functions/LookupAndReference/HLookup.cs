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
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionHLookup
	public class FunctionHLookup : FunctionOrderedLookupBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "HLOOKUP"; } }
		public override int Code { get { return 0x0065; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value | OperandDataType.Array; } }
		protected internal override int CalculateSecondaryDimension(VariantValue tableArray) {
			if (tableArray.IsCellRange)
				return tableArray.CellRangeValue.Height;
			else
				return tableArray.ArrayValue.Height;
		}
		protected internal override IVector<VariantValue> GetLookupVector(VariantValue tableArray) {
			if (tableArray.IsCellRange)
				return new RangeHorizontalVector(tableArray.CellRangeValue.GetFirstInnerCellRange(), 0);
			else
				return new ArrayHorizontalVector(tableArray.ArrayValue, 0);
		}
		protected internal override IVector<VariantValue> GetResultVector(VariantValue tableArray, int secondaryDirectionIndex) {
			if (tableArray.IsCellRange)
				return new RangeHorizontalVector(tableArray.CellRangeValue.GetFirstInnerCellRange(), secondaryDirectionIndex);
			else
				return new ArrayHorizontalVector(tableArray.ArrayValue, secondaryDirectionIndex);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
	}
	#endregion
}

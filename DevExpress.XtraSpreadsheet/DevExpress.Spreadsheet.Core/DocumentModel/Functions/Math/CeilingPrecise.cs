﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	#region FunctionCeilingPrecise
	public class FunctionCeilingPrecise : FunctionCeiling {
		#region Static Members
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Value, FunctionParameterOption.NonRequired));
			return collection;
		}
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		#endregion
		#region Properties
		public override string Name { get { return "CEILING.PRECISE"; } }
		public override int Code { get { return 0x4071; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		#endregion
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue number = arguments[0].ToNumeric(context);
			if (number.IsError)
				return number;
			double significanceValue = 1;
			if (arguments.Count > 1) {
				VariantValue significance = arguments[1];
				if (!significance.IsEmpty) {
					significance = significance.ToNumeric(context);
					if (significance.IsError)
						return significance;
					significanceValue = Math.Abs(significance.NumericValue);
				}
			}
			return GetNumericResult(number.NumericValue, significanceValue);
		}
	}
	#endregion
}

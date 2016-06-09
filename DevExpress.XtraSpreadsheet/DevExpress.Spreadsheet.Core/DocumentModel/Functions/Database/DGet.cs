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

using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionDGet
	public class FunctionDGet : WorksheetDatabaseFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "DGET"; } }
		public override int Code { get { return 0x00EB; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference));
			collection.Add(new FunctionParameter(OperandDataType.Value));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value));
			return collection;
		}
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionDGetResult(context); 
		}
	}
	public class FunctionDGetResult : FunctionResult {
		#region Fields
		VariantValue matchedValue;
		int counter;
		#endregion
		public FunctionDGetResult(WorkbookDataContext context)
			: base(context) {
			this.matchedValue = VariantValue.Empty;
			this.counter = 0;
			ProcessErrorValues = true;
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			return !value.IsEmpty;
		}
		public override bool ProcessSingleValue(VariantValue value) {
			if(counter == 0) {
				matchedValue = value;
			}
			counter++;
			return true;
		}
		public override VariantValue ConvertValue(VariantValue value) {
			throw new System.InvalidOperationException("This method should never be called");
		}
		public override bool ProcessConvertedValue(VariantValue value) {
			throw new System.InvalidOperationException("This method should never be called");
		}
		public override VariantValue GetFinalValue() {
			switch(counter) {
				case 0:
					return VariantValue.ErrorInvalidValueInFunction;
				case 1:
					if(matchedValue.IsSharedString)
						return matchedValue.GetTextValue(Context.StringTable);
					return matchedValue;
			}
			return VariantValue.ErrorNumber;
		}
	}
	#endregion
}

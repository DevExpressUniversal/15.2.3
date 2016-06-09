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
	#region FunctionVara
	public class FunctionVara : FunctionVar {
		#region Properties
		public override string Name { get { return "VARA"; } }
		public override int Code { get { return 0x16F; } }
		#endregion
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionVaraResult(context);
		}
	}
	#endregion
	#region FunctionVaraResult
	public class FunctionVaraResult : FunctionVarResult {
		public FunctionVaraResult(WorkbookDataContext context)
			: base(context) {
		}
		public override bool ShouldProcessValueCore(VariantValue value) {
			if (ArrayProcessing)
				return value.IsNumeric || value.IsError;
			return CellRangeProcessing ? !value.IsEmpty : true;
		}
		public override VariantValue ConvertValue(VariantValue value) {
			if (value.IsError)
				return value;
			VariantValue convertedValue = value.ToNumeric(Context);
			return CellRangeProcessing && convertedValue.IsError ? 0 : convertedValue;
		}
	}
	#endregion
}

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
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionStdevpa
	public class FunctionStdevpa : FunctionStdeva {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "STDEVPA"; } }
		public override int Code { get { return 0x016C; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		protected override FunctionResult CreateInitialFunctionResult(WorkbookDataContext context) {
			return new FunctionStdevpaResult(context);
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Reference | OperandDataType.Value | OperandDataType.Array, FunctionParameterOption.NonRequiredUnlimited));
			return collection;
		}
	}
	public class FunctionStdevpaResult : FunctionStdevaResult {
		protected override byte ErrorValue { get { return 0; } }
		public FunctionStdevpaResult(WorkbookDataContext context)
			: base(context) {
		}
	}
	#endregion
}

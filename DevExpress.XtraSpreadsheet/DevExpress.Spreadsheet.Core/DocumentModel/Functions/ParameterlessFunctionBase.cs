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
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorksheetParameterLessFunctionBase (abstract class)
	public abstract class WorksheetParameterlessFunctionBase : ISpreadsheetFunction {
		readonly static FunctionParameterCollection emptyParametersList = new FunctionParameterCollection();
		public static FunctionParameterCollection EmptyParametersList { get { return emptyParametersList; } }
		#region Properties
		public abstract int Code { get; }
		public abstract string Name { get; }
		public virtual bool IsVolatile { get { return false; } }
		public abstract OperandDataType ReturnDataType { get; }
		public bool HasFixedParametersCount { get { return true; } }
		#endregion
		public OperandDataType GetDefaultDataType() {
			OperandDataType returnType = ReturnDataType;
			if ((returnType & OperandDataType.Reference) > 0)
				return OperandDataType.Reference;
			return OperandDataType.Array;
		}
		protected abstract VariantValue GetResult(WorkbookDataContext context);
		public FunctionParameterCollection Parameters { get { return emptyParametersList; } }
		public VariantValue Evaluate(IList<VariantValue> arguments, WorkbookDataContext context, bool arrayResultEvaluation) {
			Debug.Assert(arguments == null || arguments.Count == 0);
			return GetResult(context);
		}
		public FunctionParameter GetParameterByExpressionIndex(int index) {
			return null;
		}
	}
	#endregion
}

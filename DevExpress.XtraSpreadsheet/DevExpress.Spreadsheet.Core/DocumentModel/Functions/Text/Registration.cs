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
	public partial class FormulaCalculator {
		#region AddTextFunctions
		public static void AddTextFunctions(Dictionary<string, ISpreadsheetFunction> result) {
			AddFunction(result, new FunctionAsc());
			AddFunction(result, new FunctionBahtText());
			AddFunction(result, new FunctionChar());
			AddFunction(result, new FunctionClean());
			AddFunction(result, new FunctionCode());
			AddFunction(result, new FunctionConcatenate());
			AddFunction(result, new FunctionDollar());
			AddFunction(result, new FunctionExact());
			AddFunction(result, new FunctionFind());
			AddFunction(result, new FunctionFindB());
			AddFunction(result, new FunctionFixed());
			AddFunction(result, new FunctionLeft());
			AddFunction(result, new FunctionLeftB());
			AddFunction(result, new FunctionLen());
			AddFunction(result, new FunctionLenB());
			AddFunction(result, new FunctionLower());
			AddFunction(result, new FunctionMid());
			AddFunction(result, new FunctionMidB());
			AddFunction(result, new FunctionNumberValue());
			AddFunction(result, new FunctionPhonetic());
			AddFunction(result, new FunctionProper());
			AddFunction(result, new FunctionReplace());
			AddFunction(result, new FunctionReplaceB());
			AddFunction(result, new FunctionRept());
			AddFunction(result, new FunctionRight());
			AddFunction(result, new FunctionRightB());
			AddFunction(result, new FunctionSearch());
			AddFunction(result, new FunctionSearchB());
			AddFunction(result, new FunctionSubstitute());
			AddFunction(result, new FunctionT());
			AddFunction(result, new FunctionText());
			AddFunction(result, new FunctionTrim());
			AddFunction(result, new FunctionUnicode());
			AddFunction(result, new FunctionUpper());
			AddFunction(result, new FunctionValue());
		}
		#endregion
	}
}

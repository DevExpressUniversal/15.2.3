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

using System.Text;
using DevExpress.Utils;
using System.ComponentModel;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Spreadsheet.Formulas {
	#region BinaryOperatorExpression (abstract class)
	public abstract class BinaryOperatorExpression : Expression {
		#region Fields
		IExpression leftExpression;
		IExpression rightExpression;
		#endregion
		protected BinaryOperatorExpression(IExpression leftExpression, IExpression rightExpression) {
			Guard.ArgumentNotNull(leftExpression, "leftExpression");
			Guard.ArgumentNotNull(rightExpression, "rightExpression");
			this.leftExpression = leftExpression;
			this.rightExpression = rightExpression;
		}
		protected BinaryOperatorExpression() {
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("BinaryOperatorExpressionLeftExpression")]
#endif
		public IExpression LeftExpression {
			get { return leftExpression; }
			set {
				Guard.ArgumentNotNull(value, "leftExpression");
				leftExpression = value;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("BinaryOperatorExpressionRightExpression")]
#endif
		public IExpression RightExpression {
			get { return rightExpression; }
			set {
				Guard.ArgumentNotNull(value, "rightExpression");
				rightExpression = value;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("BinaryOperatorExpressionOperatorText")]
#endif
		public abstract string OperatorText { get; }
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			LeftExpression.BuildExpressionString(result, workbook);
			result.Append(OperatorText);
			RightExpression.BuildExpressionString(result, workbook);
		}
		protected void CopyFrom(BinaryOperatorExpression value) {
			base.CopyFrom(value);
			leftExpression = value.LeftExpression.Clone();
			rightExpression = value.RightExpression.Clone();
		}
	}
	#endregion
}

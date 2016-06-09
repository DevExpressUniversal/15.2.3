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
	#region UnaryOperatorExpression (abstract class)
	public abstract class UnaryOperatorExpression : Expression {
		IExpression innerExpression;
		protected UnaryOperatorExpression(IExpression innerExpression) {
			Guard.ArgumentNotNull(innerExpression, "innerExpression");
			this.innerExpression = innerExpression;
		}
		protected UnaryOperatorExpression() {
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("UnaryOperatorExpressionInnerExpression")]
#endif
		public IExpression InnerExpression {
			get { return innerExpression; }
			set {
				Guard.ArgumentNotNull(value, "innerExpression");
				innerExpression = value;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("UnaryOperatorExpressionOperatorText")]
#endif
		public abstract string OperatorText { get; }
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			result.Append(OperatorText);
			InnerExpression.BuildExpressionString(result, workbook);
		}
		protected void CopyFrom(UnaryOperatorExpression value) {
			base.CopyFrom(value);
			innerExpression = value.InnerExpression.Clone();
		}
	}
	#endregion
}

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
using DevExpress.Office;
using DevExpress.Utils;
using System.ComponentModel;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Spreadsheet.Formulas {
	#region ConstantExpression
	public class ConstantExpression : Expression, ISupportsCopyFrom<ConstantExpression>, ICloneable<ConstantExpression> {
		CellValue value;
		public ConstantExpression(CellValue value) {
			Guard.ArgumentNotNull(value, "value");
			this.value = value;
		}
		protected ConstantExpression() {
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ConstantExpressionValue")]
#endif
		public CellValue Value {
			get { return value; }
			set {
				Guard.ArgumentNotNull(value, "value");
				this.value = value;
			}
		}
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			Model.ValueParsedThing modelValue = Model.ValueParsedThing.CreateInstance(value.ModelVariantValue, context);
			modelValue.BuildExpressionStringCore(result, context);
		}
		#region ISupportsCopyFrom<ConstantExpression> Members
		public void CopyFrom(ConstantExpression value) {
			base.CopyFrom(value);
			CellValue patternValue = value.value;
			this.value = new CellValue(patternValue.ModelVariantValue, patternValue.ModelDataContext);
		}
		#endregion
		#region ICloneable<ConstantExpression> Members
		public ConstantExpression Clone() {
			ConstantExpression result = new ConstantExpression();
			result.CopyFrom(this);
			return result;
		}
		protected override IExpression CloneCore() {
			return Clone();
		}
		#endregion
	}
	#endregion
}

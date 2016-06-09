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
using System.Text;
using DevExpress.Office;
using DevExpress.Spreadsheet.Functions;
using DevExpress.Utils;
using System.ComponentModel;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Spreadsheet.Formulas {
	#region ArrayConstantExpression
	public class ArrayConstantExpression : Expression, ISupportsCopyFrom<ArrayConstantExpression>, ICloneable<ArrayConstantExpression> {
		#region Fields
		Model.IVariantArray variantArray;
		#endregion
		public ArrayConstantExpression(CellValue[,] values) {
			Guard.ArgumentNotNull(values, "values");
			SetArray(values);
		}
		internal ArrayConstantExpression(Model.IVariantArray variantArray) {
			Guard.ArgumentNotNull(variantArray, "variantArray");
			this.variantArray = variantArray;
		}
		protected ArrayConstantExpression() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ArrayConstantExpressionValues")]
#endif
		public CellValue[,] Values {
			get { return GetArray(); }
			set {
				Guard.ArgumentNotNull(value, "values");
				SetArray(value);
			}
		}
		#endregion
		void SetArray(CellValue[,] values) {
			ParameterValue parameterValue = values;
			this.variantArray = parameterValue.ModelVariantValue.ArrayValue;
		}
		CellValue[,] GetArray() {
			Model.VariantValue value = Model.VariantValue.FromArray(variantArray);
			ParameterValue parameterValue = new ParameterValue(value);
			return parameterValue.ArrayValue;
		}
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			Model.ParsedThingArray modelThing = new Model.ParsedThingArray();
			modelThing.ArrayValue = variantArray;
			modelThing.BuildExpressionStringCore(result, context);
		}
		#region ISupportsCopyFrom<ArrayConstantExpression> Members
		public void CopyFrom(ArrayConstantExpression value) {
			base.CopyFrom(value);
			this.variantArray = value.variantArray.Clone();
		}
		#endregion
		#region ICloneable<ArrayConstantExpression> Members
		public ArrayConstantExpression Clone() {
			ArrayConstantExpression result = new ArrayConstantExpression();
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

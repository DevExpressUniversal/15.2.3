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
using System.Text;
using DevExpress.Office;
using System.ComponentModel;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Spreadsheet.Formulas {
	#region DefinedNameReferenceExpression
	public class DefinedNameReferenceExpression : ReferenceExpression, ISupportsCopyFrom<DefinedNameReferenceExpression>, ICloneable<DefinedNameReferenceExpression> {
		#region Fields
		string definedName;
		#endregion
		public DefinedNameReferenceExpression(string definedName, SheetReference sheetReference)
			: base(sheetReference) {
			this.definedName = definedName;
		}
		public DefinedNameReferenceExpression(string definedName) {
			this.definedName = definedName;
		}
		protected DefinedNameReferenceExpression() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("DefinedNameReferenceExpressionDefinedName")]
#endif
		public string DefinedName { get { return definedName; } set { definedName = value; } }
		#endregion
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			base.BuildExpressionStringCore(result, workbook, context);
			result.Append(definedName);
		}
		#region ISupportsCopyFrom<DefinedNameReferenceExpression> Members
		public void CopyFrom(DefinedNameReferenceExpression value) {
			base.CopyFrom(value);
			this.definedName = value.definedName;
		}
		#endregion
		#region ICloneable<DefinedNameReferenceExpression> Members
		public DefinedNameReferenceExpression Clone() {
			DefinedNameReferenceExpression result = new DefinedNameReferenceExpression();
			result.CopyFrom(this);
			return result;
		}
		protected override IExpression CloneCore() {
			return Clone();
		}
		#endregion
		internal Model.ParsedThingName ToModelThing() {
			return new Model.ParsedThingName(definedName);
		}
	}
	#endregion
}

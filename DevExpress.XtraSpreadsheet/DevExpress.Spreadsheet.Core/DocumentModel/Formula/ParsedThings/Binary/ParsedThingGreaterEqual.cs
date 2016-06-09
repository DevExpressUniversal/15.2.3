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

namespace DevExpress.XtraSpreadsheet.Model {
	#region ParsedThingGreaterEqual
	public class ParsedThingGreaterEqual : BinaryBooleanParsedThing {
		#region Fields
		static ParsedThingGreaterEqual instance = new ParsedThingGreaterEqual();
		#endregion
		protected ParsedThingGreaterEqual() {
		}
		#region Properties
		public static ParsedThingGreaterEqual Instance {
			get {
				return instance;
			}
		}
		public override ConditionPriority Priority { get { return ConditionPriority.Low; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override string GetOperatorText(WorkbookDataContext context) {
			return ">=";
		}
		protected override VariantValue CalculateBooleanResult(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			return context.Compare(leftOperand, rightOperand) >= 0;
		}
	}
	#endregion
	#region ParsedThingGreaterEqualStrictTypeMatch
	public class ParsedThingGreaterEqualStrictTypeMatch : ParsedThingGreaterEqual, IHeplerParsedThing {
		public ParsedThingGreaterEqualStrictTypeMatch()
			: base() {
		}
		protected internal override VariantValue EvaluateSimpleOperands(WorkbookDataContext context, VariantValue leftOperand, VariantValue rightOperand) {
			if (leftOperand.InversedSortType != rightOperand.InversedSortType)
				return false;
			return base.EvaluateSimpleOperands(context, leftOperand, rightOperand);
		}
	}
	#endregion
}

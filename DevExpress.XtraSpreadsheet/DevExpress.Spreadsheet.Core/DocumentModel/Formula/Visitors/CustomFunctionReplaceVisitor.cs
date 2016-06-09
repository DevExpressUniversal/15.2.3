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
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	class CustomFunctionReplaceVisitor : ExpressionModifierVisitor {
		public CustomFunctionReplaceVisitor(WorkbookDataContext context)
			: base(context) {
		}
		public override void OnFunction(ParsedThingFunc thing, ThingPosition position, int paramCount) {
			if (thing is ParsedThingCustomFunc) {
				ReplaceFunctionByCalculatedValue(position, paramCount);
				return;
			}
			base.OnFunction(thing, position, paramCount);
		}
	}
	class CustomFunctionReplaceVisitor2 :ExpressionModifierVisitor {
		#region static
		static bool IsMailMergeFunction(ParsedThingFunc thing) {
			bool result = false;
			if ((thing.Function.Code & 0x4100) == 0x4100) {
				result = FunctionHasName(thing, "FIELD");
				result = result || FunctionHasName(thing, "RANGE");
				result = result || FunctionHasName(thing, "FIELDPICTURE");
				result = result || FunctionHasName(thing, "PARAMETER");
			}
			return result;
		}
		static bool FunctionHasName(ParsedThingFunc thing, string name) {
			return StringExtensions.CompareInvariantCultureIgnoreCase(thing.Function.Name, name) == 0;
		}
		#endregion
		public CustomFunctionReplaceVisitor2(WorkbookDataContext context)
			: base(context) {
		}
		public override void OnFunction(ParsedThingFunc thing, ThingPosition position, int paramCount) {
			if (IsMailMergeFunction(thing)) {
				ReplaceFunctionByCalculatedValue(position, paramCount);
				return;
			}
			base.OnFunction(thing, position, paramCount);
		}
	}
	class CustomFunctionFinderVisitor : ExpressionModifierVisitor {
		public CellRangeList References { get; private set; }
		public CustomFunctionFinderVisitor(WorkbookDataContext context)
			: base(context) {
			References = new CellRangeList();
		}
		public override void OnFunction(ParsedThingFunc thing, ThingPosition position, int paramCount) {
			if (StringExtensions.CompareInvariantCultureIgnoreCase(thing.Function.Name, "RANGE") == 0) {
				position.End--;
				VariantValue value = CalculateSubExpression(position);
				if(value.IsCellRange)
					References.Add(value.CellRangeValue);
				position.End++;
			}
			base.OnFunction(thing, position, paramCount);
		}
	}
	class FunctionFieldFinderVisitor : ParsedThingVisitor {
		public bool HaveFunctionField { get; set; }
		public bool HaveFunctionFieldPicture { get; set; }
		public override void VisitFunction(ParsedThingFunc thing) {
			base.VisitFunction(thing);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(thing.Function.Name, "FIELD") == 0) {
				HaveFunctionField = true;
			}
			if (StringExtensions.CompareInvariantCultureIgnoreCase(thing.Function.Name, "FIELDPICTURE") == 0) {
				HaveFunctionFieldPicture = true;
			}
			if (StringExtensions.CompareInvariantCultureIgnoreCase(thing.Function.Name, "PARAMETER") == 0) {
				HaveFunctionField = true;
			}
		}
	}
	public static class SimpleExpressionChecker {
		static HashSet<Type> allowedThingTypesTable = CreateAllowedThingTypes();
		static HashSet<Type> CreateAllowedThingTypes() {
			HashSet<Type> result = new HashSet<Type>();
			result.Add(typeof(ParsedThingInteger));
			result.Add(typeof(ParsedThingBoolean));
			result.Add(typeof(ParsedThingNumeric));
			result.Add(typeof(ParsedThingStringValue));
			result.Add(typeof(ParsedThingRefErr));
			result.Add(typeof(ParsedThingAreaErr));
			result.Add(typeof(ParsedThingError));
			result.Add(typeof(ParsedThingAttrSpaceSemi));
			result.Add(typeof(ParsedThingAttrSpace));
			result.Add(typeof(ParsedThingAttrSemi));
			return result;
		}
		public static bool CheckExpression(ParsedExpression expression) {
			if(expression == null)
				return true;
			foreach (IParsedThing thing in expression) {
				if (!allowedThingTypesTable.Contains(thing.GetType()))
					return false;
			}
			return true;
		}
	}
}

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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.StructuredStorage.Writer;
using DevExpress.XtraSpreadsheet.Import.Xls;
using System.Text;
using DevExpress.XtraSpreadsheet.Internal;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class SheetDefinitionWalkerXls : SheetDefinitionWalkerBase {
		XlsRPNContext context;
		protected override int ProcessSheetDefinition(int sheetDefinitionIndex) {
			context.RegisterSheetDefinition(context.WorkbookContext.GetSheetDefinition(sheetDefinitionIndex));
			return sheetDefinitionIndex;
		}
		public SheetDefinitionWalkerXls(XlsRPNContext context)
			: base(context.WorkbookContext) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
		}
		public XlsRPNContext RPNContext { get { return context; } }
		public override void Visit(ParsedThingName expression) {
			DefinedNameBase definedName = context.WorkbookContext.GetDefinedName(expression.DefinedName);
			if(definedName == null && !context.IsRegisteredDefinedName(expression.DefinedName, XlsDefs.NoScope))
				context.RegisterDefinedName(expression.DefinedName, XlsDefs.NoScope);
		}
		public override void Visit(ParsedThingNameX expression) {
			SheetDefinition sheetDefinition = context.WorkbookContext.GetSheetDefinition(expression.SheetDefinitionIndex);
			if(!sheetDefinition.IsExternalReference) {
				DefinedNameBase definedName = context.WorkbookContext.GetDefinedName(expression.DefinedName, sheetDefinition);
				if(definedName == null) {
					int scope = context.GetScope(sheetDefinition);
					if(!context.IsRegisteredDefinedName(expression.DefinedName, scope))
						context.RegisterDefinedName(expression.DefinedName, scope);
				}
			}
			context.RegisterSheetDefinition(sheetDefinition, expression.DefinedName);
		}
		public override void Visit(ParsedThingAddinFunc expression) {
			context.RegisterSheetDefinition(null, expression.Name);
			base.Visit(expression);
		}
		void ProcessFunction(ParsedThingFunc expression) {
			ISpreadsheetFunction function = expression.Function;
			if (XlsParsedThingConverter.IsFutureFunction(function.Code)) {
				string name = WorksheetFunctionBase.FUTURE_FUNCTION_PREFIX + function.Name;
				if (!context.IsRegisteredDefinedName(name, XlsDefs.NoScope))
					context.RegisterDefinedName(name, XlsDefs.NoScope);
			}
			if (XlsParsedThingConverter.IsInternalFunction(function.Code)) {
				string name = function.Name;
				if (!context.IsRegisteredDefinedName(name, XlsDefs.NoScope))
					context.RegisterDefinedName(name, XlsDefs.NoScope);
			}
			if (XlsParsedThingConverter.IsPredefinedFunction(function.Code)) {
				context.RegisterSheetDefinition(null, function.Name);
			}
		}
		public override void Visit(ParsedThingFunc expression) {
			ProcessFunction(expression);
		}
		public override void Visit(ParsedThingFuncVar expression) {
			ProcessFunction(expression);
		}
		public override void Visit(ParsedThingUnknownFunc expression) {
			string functionName = expression.Name;
			if(PredefinedFunctions.IsExcel2010PredefinedNonFutureFunction(functionName)) {
				context.RegisterSheetDefinition(null, functionName);
			}
			else {
				if(!context.IsRegisteredDefinedName(functionName, XlsDefs.NoScope))
					context.RegisterDefinedName(functionName, XlsDefs.NoScope);
			}
		}
		public override void Visit(ParsedThingCustomFunc expression) {
			string functionName = expression.Name;
			if (!context.IsRegisteredDefinedName(functionName, XlsDefs.NoScope))
				context.RegisterDefinedName(functionName, XlsDefs.NoScope);
		}
		public override void Visit(ParsedThingUnknownFuncExt expression) {
			string functionName = expression.Name;
			SheetDefinition sheetDefinition = context.WorkbookContext.GetSheetDefinition(expression.SheetDefinitionIndex);
			if(!sheetDefinition.IsExternalReference) {
				int scope = context.GetScope(sheetDefinition);
				if(!context.IsRegisteredDefinedName(functionName, scope))
					context.RegisterDefinedName(functionName, scope);
			}
			context.RegisterSheetDefinition(sheetDefinition, functionName);
		}
		public override void Visit(ParsedThingTable expression) {
			SheetDefinition sheetDefinition = expression.GetSheetDefinition(context.WorkbookContext);
			if(sheetDefinition != null && !sheetDefinition.IsCurrentSheetReference)
				context.RegisterSheetDefinition(sheetDefinition);
		}
	}
}

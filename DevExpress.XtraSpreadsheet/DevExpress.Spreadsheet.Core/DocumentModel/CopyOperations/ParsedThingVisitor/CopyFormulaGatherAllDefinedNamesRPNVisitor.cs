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
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class CopyFormulaGatherAllDefinedNamesRPNVisitor : ParsedThingVisitor {
		HashSet<string> result;
		HashSet<ParsedThingNameX> thingsWithSheetDefinitions;
		readonly ICopyEverythingArguments args;
		bool suppressAddExternalReferenceIfExpressionRefersToOuterSourceRangeArea;
		public CopyFormulaGatherAllDefinedNamesRPNVisitor(ICopyEverythingArguments args)
			: base() {
			Guard.ArgumentNotNull(args, "ICopyEverythingArguments");
			this.result = new HashSet<string>();
			thingsWithSheetDefinitions = new HashSet<ParsedThingNameX>();
			this.args = args;
			suppressAddExternalReferenceIfExpressionRefersToOuterSourceRangeArea = false;
		}
		public HashSet<string> Result { get { return this.result; } set { this.result = value; } }
		public HashSet<ParsedThingNameX> ThingsWithExternalSheetDefinitions { get { return thingsWithSheetDefinitions; } set { thingsWithSheetDefinitions = value; } }
		public bool SuppressParsedThingXFinding { get; set; }
		public bool IsCopyWorksheetOperation { get { return args.IsCopyWorksheetOperation; } }
		WorkbookDataContext SourceDataContext { get { return args.SourceDataContext; } }
		public override void Visit(ParsedThingName thing) {
			base.Visit(thing);
			string definedName = thing.DefinedName;
			ProcessParsedthingName(definedName);
		}
		void ProcessParsedthingName(string definedName) {
			if (!result.Contains(definedName)) {
				DefinedNameBase foundDefinedName = args.SourceDataContext.GetDefinedName(definedName);
				if (foundDefinedName != null) {
					result.Add(definedName);
					ParsedExpression definedNameExpression = foundDefinedName.Expression;
					try {
						this.suppressAddExternalReferenceIfExpressionRefersToOuterSourceRangeArea = true;
						this.Process(definedNameExpression);
					}
					finally {
						this.suppressAddExternalReferenceIfExpressionRefersToOuterSourceRangeArea = false;
					}
				}
			}
		}
		public override void Visit(ParsedThingNameX thing) {
			base.Visit(thing);
			if (SuppressParsedThingXFinding)
				return;
			string definedName = thing.DefinedName;
			SheetDefinition sourceSheetDefinition = SourceDataContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			string sheetNameStart = sourceSheetDefinition.SheetNameStart;
			bool isSheetDefinitionExternal = sourceSheetDefinition.IsExternalReference;
			bool isWorkbookGlobalNotExternalDefinedName = !isSheetDefinitionExternal && (String.IsNullOrEmpty(sheetNameStart) || SourceDataContext.GetDefinedName(thing.DefinedName, sourceSheetDefinition).ScopedSheetId < 0);
			IWorksheet sourceDataContextCurrentWorksheet = SourceDataContext.CurrentWorksheet;
			bool definedNameIsLocalInSourceSheet = !isSheetDefinitionExternal && !isWorkbookGlobalNotExternalDefinedName && String.Equals(sheetNameStart, sourceDataContextCurrentWorksheet.Name, StringComparison.OrdinalIgnoreCase);
			if (IsCopyWorksheetOperation && (definedNameIsLocalInSourceSheet || isWorkbookGlobalNotExternalDefinedName))
				ProcessParsedthingName(definedName);
			else
				if (!thingsWithSheetDefinitions.Contains(thing)) {
				if (!suppressAddExternalReferenceIfExpressionRefersToOuterSourceRangeArea
					|| isSheetDefinitionExternal) 
					thingsWithSheetDefinitions.Add(thing);
			}
		}
	}
}

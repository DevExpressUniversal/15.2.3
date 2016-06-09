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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model{
	#region DefinedNameRenamedFormulaWalker
	public class DefinedNameRenamedFormulaWalker : WorkbookFormulaWalker {
		readonly string oldName;
		readonly string newName;
		readonly int sheetId;
		bool sheetContainsDNWithSameName;
		int currentSheetId;
		public DefinedNameRenamedFormulaWalker(string oldName, string newName, int sheetId, WorkbookDataContext context)
			: base(context) {
			this.oldName = oldName;
			this.newName = newName;
			this.sheetId = sheetId;
		}
		public override void Walk(Worksheet sheet) {
			currentSheetId = sheet.SheetId;
			sheetContainsDNWithSameName = currentSheetId != sheetId && (sheetId >= 0 || sheet.DefinedNames.Contains(oldName));
			base.Walk(sheet);
		}
		public override void Visit(ParsedThingName thing) {
			if (sheetContainsDNWithSameName)
				return;
			if (StringExtensions.CompareInvariantCultureIgnoreCase(oldName, thing.DefinedName) == 0) {
				thing.DefinedName = newName;
				FormulaChanged = true;
			}
		}
		public override void Visit(ParsedThingNameX thing) {
			if (StringExtensions.CompareInvariantCultureIgnoreCase(oldName, thing.DefinedName) != 0)
				return;
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference)
				return;
			IWorksheet referencedSheet = sheetDefinition.GetSheetStart(Context);
			if ((referencedSheet == null) || (sheetId >= 0 && referencedSheet.SheetId != sheetId))
				return;
			if (sheetId < 0 && referencedSheet.DefinedNames.Contains(oldName))
				return;
			if (referencedSheet == null && sheetContainsDNWithSameName)
				return;
			thing.DefinedName = newName;
			FormulaChanged = true;
		}
	}
	#endregion
}

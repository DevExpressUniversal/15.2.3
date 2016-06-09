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

using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region IgnoredErrorsDestination
	public class IgnoredErrorsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ignoredError", OnIgnoredError);
			return result;
		}
		static Destination OnIgnoredError(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new IgnoredErrorDestination(importer);
		}
		#endregion
		public IgnoredErrorsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			if (Importer.CurrentWorksheet.IgnoredErrors.Count < 1)
				Importer.ThrowInvalidFile("Zero ignored errors in collection");
		}
	}
	#endregion
	#region IgnoredErrorDestination
	public class IgnoredErrorDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		IgnoredError ignoredError;
		public IgnoredErrorDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		IgnoredErrorInfo DefaultInfo { get { return Importer.CurrentWorksheet.Workbook.Cache.IgnoredErrorInfoCache.DefaultItem; } }
		public override void ProcessElementOpen(XmlReader reader) {
			CellRangeBase range = Importer.GetWpSTSqref(reader, "sqref", Importer.CurrentWorksheet);
			if (range == null)
				Importer.ThrowInvalidFile("Error sqref is not specified");
			ignoredError = new IgnoredError(Importer.CurrentWorksheet, range);
			ignoredError.BeginUpdate();
			try {
				ignoredError.EvaluateToError = Importer.GetWpSTOnOffValue(reader, "evalError", DefaultInfo.EvaluateToError);
				ignoredError.TwoDidgitTextYear = Importer.GetWpSTOnOffValue(reader, "twoDigitTextYear", DefaultInfo.TwoDidgitTextYear);
				ignoredError.NumberAsText = Importer.GetWpSTOnOffValue(reader, "numberStoredAsText", DefaultInfo.NumberAsText);
				ignoredError.InconsistentFormula = Importer.GetWpSTOnOffValue(reader, "formula", DefaultInfo.InconsistentFormula);
				ignoredError.FormulaRangeError = Importer.GetWpSTOnOffValue(reader, "formulaRange", DefaultInfo.FormulaRangeError);
				ignoredError.UnlockedFormula = Importer.GetWpSTOnOffValue(reader, "unlockedFormula", DefaultInfo.UnlockedFormula);
				ignoredError.EmptyCellReferences = Importer.GetWpSTOnOffValue(reader, "emptyCellReference", DefaultInfo.EmptyCellReferences);
				ignoredError.ListDataValidation = Importer.GetWpSTOnOffValue(reader, "listDataValidation", DefaultInfo.ListDataValidation);
				ignoredError.InconsistentColumnFormula = Importer.GetWpSTOnOffValue(reader, "calculatedColumn", DefaultInfo.InconsistentColumnFormula);
			}
			finally {
				ignoredError.EndUpdate();
			}
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (ignoredError != null)
				Importer.CurrentWorksheet.IgnoredErrors.AddCore(ignoredError);
		}
	}
	#endregion
}

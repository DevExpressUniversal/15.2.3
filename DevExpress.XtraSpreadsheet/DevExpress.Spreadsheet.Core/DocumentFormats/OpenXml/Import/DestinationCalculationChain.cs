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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class CalculationChainDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("c", OnCell);
			return result;
		}
		static Destination OnCell(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return CalculationChainElement.GetInstance(importer);
		}
		#endregion
		public CalculationChainDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			if (Importer.NonRegisteredCellsInCellsChainCount != 0)
				Importer.DocumentModel.CalculationChain.CellsChain.Reset(); 
		}
	}
	public class CalculationChainElement : LeafElementDestination<SpreadsheetMLBaseImporter> {
		[ThreadStatic]
		static CalculationChainElement instance;
		int lastSheetId = -1;
		private CalculationChainElement(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public static CalculationChainElement GetInstance(SpreadsheetMLBaseImporter importer) {
			if (instance == null || instance.Importer != importer)
				instance = new CalculationChainElement(importer);
			return instance;
		}
		public static void ClearInstance() {
			instance = null;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			lastSheetId = Importer.GetWpSTIntegerValue(reader, "i", lastSheetId);
			IWorksheet sheet = Importer.SheetIdsTable[lastSheetId];
			if (sheet == null)
				return;
			string reference = Importer.ReadAttribute(reader, "r");
			CellPosition position = CellReferenceParser.TryParse(reference);
			ICell cell = sheet.TryGetCell(position.Column, position.Row) as ICell;
			if (cell == null)
				return;
			CellsChain cellsChain = Importer.DocumentModel.CalculationChain.CellsChain;
			if (CellsChainContainsCell(cellsChain, cell)) {
				Importer.NonRegisteredCellsInCellsChainCount = -1;
				return;
			}
			Importer.DocumentModel.CalculationChain.CellsChain.Add(cell);
			--Importer.NonRegisteredCellsInCellsChainCount;
		}
		bool CellsChainContainsCell(CellsChain chain, ICell cell) {
			if (cell.FormulaInfo.NextCell != null)
				return true;
			if (object.ReferenceEquals(chain.Footer, cell))
				return true;
			return false;
		}
	}
}

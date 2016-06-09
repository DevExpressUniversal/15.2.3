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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region MergeCellsDestination
	public class MergeCellsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		MergedCellsRangeOverlapChecker overlapChecker;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("mergeCell", OnMergeCell);
			return result;
		}
		static Destination OnMergeCell(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new MergeCellDestination(importer, GetThis(importer).overlapChecker);
		}
		static MergeCellsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (MergeCellsDestination)importer.PeekDestination();
		}
		public MergeCellsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.overlapChecker = new MergedCellsRangeOverlapChecker();
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region MergeCellDestination
	public class MergeCellDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		MergedCellsRangeOverlapChecker overlapChecker;
		public MergeCellDestination(SpreadsheetMLBaseImporter importer, MergedCellsRangeOverlapChecker overlapChecker)
			: base(importer) {
				this.overlapChecker = overlapChecker;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellRange range = Importer.ReadCellRange(reader, "ref", Importer.CurrentWorksheet);
			if(range != null && IsNotOverlapped(range) && range.CellCount > 1)
				Importer.CurrentWorksheet.MergedCells.Add(range);
		}
		bool IsNotOverlapped(CellRange range) {
			CellRangeInfo info = new CellRangeInfo(range.TopLeft, range.BottomRight);
			return this.overlapChecker.IsNotOverlapped(info);
		}
	}
	#endregion
}

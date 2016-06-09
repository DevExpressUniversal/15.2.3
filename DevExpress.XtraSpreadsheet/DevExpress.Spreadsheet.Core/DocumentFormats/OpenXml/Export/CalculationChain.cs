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

using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Model;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal virtual void AddCalculationChainPackageContent() {
			if (!ShouldExportCalculationChain())
				return;
			AddPackageContent(@"xl\calcChain.xml", ExportCalculationChain());
		}
		protected internal virtual bool ShouldExportCalculationChain() {
			CalculationChain chain = Workbook.CalculationChain;
			return shouldExportCalculationChain && chain.Enabled && chain.CellsChain.Header != null;
		}
		protected internal virtual CompressedStream ExportCalculationChain() {
			return CreateXmlContent(GenerateCalculationChainXmlContent);
		}
		protected internal virtual void GenerateCalculationChainXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateCalculationChainContent();
		}
		protected internal virtual void GenerateCalculationChainContent() {
			WriteShStartElement("calcChain");
			try {
				CellsChain chain = Workbook.CalculationChain.CellsChain;
				ICell cell = chain.Header;
				int lastSheetId = -1;
				do {
					GenerateCalcChainCell(cell, lastSheetId);
					lastSheetId = cell.SheetId;
					cell = cell.FormulaInfo.NextCell;
				}
				while (cell != null);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateCalcChainCell(ICell cell, int lastSheetId) {
			WriteShStartElement("c");
			try {
				if (cell.FormulaType == FormulaType.Array)
					WriteIntValue("a", 1);
				if (cell.SheetId != lastSheetId)
					WriteIntValue("i", cell.SheetId);
				WriteCellPosition("r", cell.Position);
			}
			finally {
				WriteShEndElement();
			}
		}
	}
}

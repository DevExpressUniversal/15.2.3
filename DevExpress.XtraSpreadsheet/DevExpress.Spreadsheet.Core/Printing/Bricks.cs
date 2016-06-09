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
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Layout;
using DevExpress.Office.Printing;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Export.Xl;
#if !SL
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraSpreadsheet.Printing {
	#region BorderLineBrick
#if !SL
	[BrickExporter(typeof(BorderLineBrickExporter))]
#endif
	public class BorderLineBrick : PatternLineBrick<XlBorderLineStyle> {
		public BorderLineBrick(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override PatternLine<XlBorderLineStyle> GetPatternLine() {
			return DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(PatternLineType);
		}
	}
	#endregion
	#region VerticalBorderLineBrick
#if !SL
	[BrickExporter(typeof(VerticalBorderLineBrickExporter))]
#endif
	public class VerticalBorderLineBrick : PatternLineBrick<XlBorderLineStyle> {
		public VerticalBorderLineBrick(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override PatternLine<XlBorderLineStyle> GetPatternLine() {
			return DocumentModel.DefaultBorderLineRepository.GetPatternLineByType(PatternLineType);
		}
	}
	#endregion
#if !SL
	#region BorderLineBrickExporter
	public class BorderLineBrickExporter : PatternLineBrickExporter<XlBorderLineStyle> {
		protected override IPatternLinePainter<XlBorderLineStyle> CreateLinePainter(IGraphicsPainter painter, DocumentLayoutUnitConverter unitConverter) {
			return new SpreadsheetHorizontalPatternLinePainter(painter, unitConverter);
		}
	}
	#endregion
	#region VerticalBorderLineBrickExporter
	public class VerticalBorderLineBrickExporter : PatternLineBrickExporter<XlBorderLineStyle> {
		protected override IPatternLinePainter<XlBorderLineStyle> CreateLinePainter(IGraphicsPainter painter, DocumentLayoutUnitConverter unitConverter) {
			return new SpreadsheetVerticalPatternLinePainter(painter, unitConverter);
		}
	}
	#endregion
#endif
}

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
using System.Collections;
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Layout;
using DevExpress.Office.Printing;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
#if !SL
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.BrickExporters;
#else
#endif
namespace DevExpress.XtraRichEdit.Printing {
	#region UnderlineBrick
#if !SL
	[BrickExporter(typeof(UnderlineBrickExporter))]
#endif
	public class UnderlineBrick : PatternLineBrick<UnderlineType> {
		public UnderlineBrick(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override PatternLine<UnderlineType> GetPatternLine() {
			return DocumentModel.DefaultUnderlineRepository.GetPatternLineByType(PatternLineType);
		}
	}
	#endregion
	#region VerticalUnderlineBrick
#if !SL
	[BrickExporter(typeof(VerticalUnderlineBrickExporter))]
#endif
	public class VerticalUnderlineBrick : PatternLineBrick<UnderlineType> {
		public VerticalUnderlineBrick(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override PatternLine<UnderlineType> GetPatternLine() {
			return DocumentModel.DefaultUnderlineRepository.GetPatternLineByType(PatternLineType);
		}
	}
	#endregion
	#region StrikeoutBrick
#if !SL
	[BrickExporter(typeof(StrikeoutBrickExporter))]
#endif
	public class StrikeoutBrick : PatternLineBrick<StrikeoutType> {
		public StrikeoutBrick(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override PatternLine<StrikeoutType> GetPatternLine() {
			return DocumentModel.DefaultStrikeoutRepository.GetPatternLineByType(PatternLineType);
		}
	}
	#endregion
#if !SL
	#region UnderlineBrickExporter
	public class UnderlineBrickExporter : PatternLineBrickExporter<UnderlineType> {
		protected override IPatternLinePainter<UnderlineType> CreateLinePainter(IGraphicsPainter painter, DocumentLayoutUnitConverter unitConverter) {
			return new RichEditHorizontalPatternLinePainter(painter, unitConverter);
		}
	}
	#endregion
	#region StrikeoutBrickExporter
	public class StrikeoutBrickExporter : PatternLineBrickExporter<StrikeoutType> {
		protected override IPatternLinePainter<StrikeoutType> CreateLinePainter(IGraphicsPainter painter, DocumentLayoutUnitConverter unitConverter) {
			return new RichEditHorizontalPatternLinePainter(painter, unitConverter);
		}
	}
	#endregion
	#region VerticalUnderlineBrickExporter
	public class VerticalUnderlineBrickExporter : VerticalPatternLineBrickExporter<UnderlineType> {
		protected override IPatternLinePainter<UnderlineType> CreateLinePainter(IGraphicsPainter painter, DocumentLayoutUnitConverter unitConverter) {
			return new RichEditVerticalPatternLinePainter(painter, unitConverter);
		}
	}
	#endregion
#endif
}

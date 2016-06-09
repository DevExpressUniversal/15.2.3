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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.Office.Layout;
#if !SL
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.Office.Printing {
	#region OfficePanelBrick
#if !SL
	[BrickExporter(typeof(OfficePanelBrickExporter))]
#endif
	public class OfficePanelBrick : PanelBrick {
		internal readonly DocumentLayoutUnitConverter unitConverter;
		public PointF AbsoluteLocation { get; set; }
		public OfficePanelBrick(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
	}
	#endregion
#if !SL
	#region OfficePanelBrickExporter
	public class OfficePanelBrickExporter : PanelBrickExporter {
		OfficePanelBrick OfficePanelBrick { get { return (OfficePanelBrick)Brick; } }
		protected override void DrawBackground(IGraphics gr, RectangleF rect) {
			if (gr is DevExpress.XtraPrinting.Export.Pdf.PdfGraphics)
				base.DrawBackground(gr, rect);
			else {
				Rectangle bounds = Rectangle.Round(OfficePanelBrick.unitConverter.DocumentsToLayoutUnits(rect));
				base.DrawBackground(gr, bounds);
			}
		}
		protected override RectangleF GetClipRect(RectangleF rect, IGraphics gr) {
			if (gr is DevExpress.XtraPrinting.Export.Pdf.PdfGraphics)
				return base.GetClipRect(rect, gr);
			else {
				RectangleF clientRect = OfficePanelBrick.unitConverter.DocumentsToLayoutUnits(BrickPaint.GetClientRect(rect));
				return OfficePanelBrick.Padding.Deflate(clientRect, OfficePanelBrick.unitConverter.Dpi);
			}
		}
	}
	#endregion
#endif
}

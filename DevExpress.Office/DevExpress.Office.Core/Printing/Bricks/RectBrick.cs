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
using System.Drawing.Drawing2D;
#endif
namespace DevExpress.Office.Printing {
	#region OfficeRectBrick
#if !SL
	[BrickExporter(typeof(OfficeRectBrickExporter))]
#endif
	public class OfficeRectBrick : VisualBrick {
		internal readonly DocumentLayoutUnitConverter unitConverter;
		public OfficeRectBrick(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
	}
	#endregion
#if !SL
	#region OfficeRectBrickExporter
	public class OfficeRectBrickExporter : VisualBrickExporter {
		protected override void DrawBackground(IGraphics gr, RectangleF rect) {
			if (gr is DevExpress.XtraPrinting.Export.Pdf.PdfGraphics)
				base.DrawBackground(gr, rect);
			else {
				GdiGraphics gdiGraphics = gr as GdiGraphics;
				RectangleF bounds = (Brick as OfficeRectBrick).unitConverter.DocumentsToLayoutUnits(rect);
				if (gdiGraphics != null) {
					PointF[] points = new PointF[2] { new PointF(0, 0), new PointF(bounds.Width, bounds.Height) };
					gdiGraphics.Graphics.TransformPoints(CoordinateSpace.Device, CoordinateSpace.World, points);
					float width = Math.Max(points[1].X - points[0].X, 1);
					float height = Math.Max(points[1].Y - points[0].Y, 1);
					points = new PointF[2] { new PointF(0, 0), new PointF(width, height) };
					gdiGraphics.Graphics.TransformPoints(CoordinateSpace.World, CoordinateSpace.Device, points);
					bounds.Width = points[1].X - points[0].X;
					bounds.Height = points[1].Y - points[0].Y;
				}
				base.DrawBackground(gr, bounds);
			}
		}
	}
	#endregion
#endif
}

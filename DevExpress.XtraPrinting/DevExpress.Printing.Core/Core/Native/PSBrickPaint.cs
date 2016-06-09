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
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting.Native {
	public class PSBrickPaint {
		public static void DrawWarningRect(IGraphics gr, RectangleF r) {
			gr.FillRectangle(Brushes.White, r);
			Pen pen = new Pen(Brushes.Black, GraphicsUnitConverter.PixelToDoc(1));
			gr.DrawRectangle(pen, r);
			pen.Dispose();
			pen = new Pen(Brushes.Red, 2 * GraphicsUnitConverter.PixelToDoc(1));
			gr.DrawLine(pen, r.Location, new PointF(r.Right, r.Bottom));
			gr.DrawLine(pen, new PointF(r.Left, r.Bottom), new PointF(r.Right, r.Top));
			pen.Dispose();
		}
		public static Image GetBrickImage(IBrick brick, RectangleF rect, Color backColor, PrintingSystemBase ps) {
			System.Diagnostics.Debug.Assert(brick != null);
			rect.Location = PointF.Empty;
			Image metaFile = MetafileCreator.CreateInstance(PSUnitConverter.DocToPixel(rect), MetafileFrameUnit.Pixel);
			using (Graphics gr = Graphics.FromImage(metaFile)) {
				gr.PageUnit = GraphicsUnit.Document;
				using (Brush brush = new SolidBrush(backColor)) {
					gr.FillRectangle(brush, rect);
				}
				BrickExporter exporter = ps.ExportersFactory.GetExporter(brick) as BrickExporter;
				exporter.Draw(gr, rect);
			}
			return metaFile;
		}
	}
}

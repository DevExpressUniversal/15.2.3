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
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraScheduler.Printing.Native;
namespace DevExpress.XtraScheduler.Printing {
	[BrickExporter(typeof(PolygonBrickExporter))]
	public class PolygonBrick : EmptyBrick, IDisposable {
		Pen pen;
		Brush brush;
		Point[] points;
		bool fillBackground;
		public PolygonBrick(Pen pen, Point[] relativePoints) {			
			this.pen = (Pen)pen.Clone();
			this.points = relativePoints;
			this.fillBackground = false;
		}
		public PolygonBrick(Brush brush, Point[] relativePoints) {
			this.brush = (Brush)brush.Clone();
			this.points = relativePoints;
			this.fillBackground = true;
		}
		internal Brush Brush { get { return brush; } }
		internal Pen Pen { get { return pen; } }
		internal Point[] Points { get { return points; } }
		internal bool FillBackground { get { return fillBackground; } }		
		public void Dispose() {
			if (brush != null)
				brush.Dispose();
			brush = null;
			if (pen != null)
				pen.Dispose();
			pen = null;			
			points = null;
		}
	}
	public class PolygonBrickExporter : BrickExporter {
		PolygonBrick PolygonBrick { get { return (PolygonBrick)base.Brick; } }
		public override void Draw(Graphics gr, RectangleF rect) {
			if ((rect.Width == 0) || (rect.Height == 0))
				return;			
			Matrix prevTransform = gr.Transform;
			ApplyTransform(gr, rect);
			if (PolygonBrick.FillBackground)
				gr.FillPolygon(PolygonBrick.Brush, PolygonBrick.Points);
			else
				gr.DrawPolygon(PolygonBrick.Pen, PolygonBrick.Points);
			gr.Transform = prevTransform;
		}
		protected internal virtual void ApplyTransform(Graphics gr, RectangleF brickRect) {
			RectangleF pointsRect = PrintingHelper.GetPointBounds(PolygonBrick.Points);					  
			gr.TranslateTransform(brickRect.X, brickRect.Y);
			gr.ScaleTransform(brickRect.Width / pointsRect.Width, brickRect.Height / pointsRect.Height, MatrixOrder.Prepend);
		}
	}
	public abstract class EmptyBrick : IBrick {
		void IBrick.SetProperties(object[,] properties) {
			Accessor.SetProperties(this, properties);
		}
		void IBrick.SetProperties(System.Collections.Hashtable properties) {
			Accessor.SetProperties(this, properties);
		}
		System.Collections.Hashtable IBrick.GetProperties() {
			System.Collections.Hashtable ht = new System.Collections.Hashtable();
			Accessor.GetProperties(this, ht);
			return ht;
		}		
	}	
}

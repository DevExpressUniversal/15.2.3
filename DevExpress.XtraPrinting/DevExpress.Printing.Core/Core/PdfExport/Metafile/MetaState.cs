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
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;
using DevExpress.XtraPrinting.Export.Pdf;
using System.IO;
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting;
namespace DevExpress.Printing.Core.PdfExport.Metafile {
	public class MetaState {
		PolyFillMode polygonFillMode = PolyFillMode.ALTERNATE;
		internal MetafileObjectTable objectTable = new MetafileObjectTable();
		internal Color BackgroundColor { get; set; }
		internal Color TextColor { get; set; }
		internal MixMode BackgroundMode { get; set; }
		internal TextAlignmentMode TextAlign { get; set; }
		internal WmfFontObject CurrentFont { get; set; }
		internal Brush CurrentBrush { get; set; }
		internal Pen CurrentPen { get; set; }
		internal Point LogicalDpi { get; set; }
		public Point WindowOrigin { get; set; }
		public PointF WindowExtent { get; set; }
		public MetaState() {
			LogicalDpi = new Point(96, 96);
		}
		public MetaState(Image image) : this() {
			HorizontalResolution = GraphicsDpi.Point;
			VerticalResolution = GraphicsDpi.Point;
			if(image != null) {
				ImageSize = image.Size;
				HorizontalResolution = image.HorizontalResolution;
				VerticalResolution = image.VerticalResolution;
			}
		}
		public void AddObject(object metaObject) {
			for(int i = 0; i < objectTable.Count; i++)
				if(objectTable[i] == null) {
					objectTable[i] = metaObject;
					return;
				}
			objectTable.Add(metaObject);
		}
		public void AddObject(int index, object metaObject) {
			objectTable[index] = metaObject;
		}
		public void RemoveObject(int objectIndex) {
			objectTable.Remove(objectIndex);
		}
		public object GetObject(int index) {
			return objectTable[index];
		}
		public void SelectObject(int objectIndex, PdfDrawContext template) {
			object obj = objectTable[objectIndex];
			SelectObject(obj, template);
		}
		public void SelectObject(object obj, PdfDrawContext template) {
			if(obj is WmfFontObject) {
				this.CurrentFont = (WmfFontObject)obj;
			} else if(obj is Brush) {
				Brush brush = (Brush)obj;
				CurrentBrush = brush;
				if(brush is SolidBrush) {
					Color color = ((SolidBrush)CurrentBrush).Color;
					template.SetRGBFillColor(color);
				} else if(brush is HatchBrush) {
					Color color = ((HatchBrush)brush).BackgroundColor;
					template.SetRGBFillColor(color);
				} else if(brush is LinearGradientBrush) {
					Color color = ((LinearGradientBrush)brush).LinearColors[0];
					template.SetRGBFillColor(color);
				}
			} else if(obj is Pen) {
				Pen pen = (Pen)obj;
				CurrentPen = pen;
				if(pen.Color != Color.Transparent) {
					template.SetRGBStrokeColor(pen.Color);
					float penWidth = pen.Width;
					if(!WindowExtent.IsEmpty) {
						penWidth *= (float)ImageSize.Height / WindowExtent.Y;
					}
					penWidth *= GraphicsDpi.Point / VerticalResolution;
					template.SetLineWidth(Math.Abs((float)penWidth));
					float[] dashArray;
					switch(pen.DashStyle) {
						case DashStyle.Custom:
							dashArray = pen.DashPattern;
							break;
						case DashStyle.Dash:
							dashArray = new float[] { 3, 1 };
							break;
						case DashStyle.DashDot:
							dashArray = new float[] { 3, 1, 1, 1 };
							break;
						case DashStyle.DashDotDot:
							dashArray = new float[] { 3, 1, 1, 1, 1, 1 };
							break;
						case DashStyle.Dot:
							dashArray = new float[] { 1 };
							break;
						case DashStyle.Solid:
							dashArray = new float[0];
							break;
						default:
							dashArray = new float[0];
							break;
					}
					for(int i = 0; i < dashArray.Length; i++) {
						dashArray[i] *= pen.Width;
					}
					template.SetDash(dashArray, 0);
				}
			}
		}
		public void SetMapMode(MapMode mapMode) {
		}
		internal float TransformY(float y) {
			float resultY = y - WindowOrigin.Y;
			if(!ImageSize.IsEmpty) {
				if(!WindowExtent.IsEmpty) {
					resultY *= (float)ImageSize.Height / WindowExtent.Y;
				}
				resultY = ImageSize.Height - resultY;
				resultY *= GraphicsDpi.Point / VerticalResolution;
			}
			return resultY;
		}
		internal float TransformX(float x) {
			float resultX = x - WindowOrigin.X;
			if(!ImageSize.IsEmpty && !WindowExtent.IsEmpty) {
				resultX *= (float)ImageSize.Width / WindowExtent.X;
				resultX *= GraphicsDpi.Point / HorizontalResolution;
			}
			return resultX;
		}
		public PointF[] Transform(PointF[] points) {
			PointF[] p = new PointF[points.Length];
			for(int i = 0; i < points.Length; i++) {
				p[i] = Transform(points[i]);
			}
			return p;
		}
		public PointF Transform(Point point) {
			return Transform(new PointF(point.X, point.Y));
		}
		public PointF Transform(PointF point) {
			return new PointF(TransformX(point.X), TransformY(point.Y));
		}
		public RectangleF Transform(RectangleF rectangle) {
			return new RectangleF(TransformX(rectangle.X),
				TransformY(rectangle.Y),
				TransformX(rectangle.Width) - TransformX(0),
				TransformY(rectangle.Height) - TransformY(0));
		}
		public PolyFillMode PolygonFillMode { get { return polygonFillMode; } set { polygonFillMode = value; } }
		public Size ImageSize { get; set; }
		public float HorizontalResolution { get; set; }
		public float VerticalResolution { get; set; }
	}
}

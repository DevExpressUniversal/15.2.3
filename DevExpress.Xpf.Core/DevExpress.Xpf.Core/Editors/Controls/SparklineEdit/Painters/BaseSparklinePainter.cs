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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Media;
namespace DevExpress.Xpf.Editors.Internal {
	public struct Bounds {
		int x;
		int y;
		int width;
		int height;
		public int X { get { return x; } }
		public int Y { get { return y; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public Bounds(int x, int y, int width, int height) {
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}
	}
	public abstract class BaseSparklinePainter {
		#region PointPresentationType
		public enum PointPresentationType {
			HighPoint,
			LowPoint,
			StartPoint,
			EndPoint,
			NegativePoint,
			SimplePoint
		}
		#endregion
		readonly Stack antialiasingStack = new Stack();
		SparklineMappingBase mapping;
		IList<SparklinePoint> data;
		SparklineControl view;
		SparklineDrawingCache localCache;
		ExtremePointIndexes extremeIndexes;
		protected SparklineMappingBase Mapping { get { return mapping; } }
		protected IList<SparklinePoint> Data { get { return data; } }
		protected SparklineControl View { get { return view; } }
		protected abstract bool EnableAntialiasing { get; }
		public abstract SparklineViewType SparklineType { get; }
		protected PointPresentationType GetPointPresentationType(int index) {
			if(index >= 0 && index < Data.Count) {
				if(View.ActualHighlightMaxPoint && extremeIndexes.IsMaxPoint(index))
					return PointPresentationType.HighPoint;
				else if (View.ActualHighlightMinPoint && extremeIndexes.IsMinPoint(index))
					return PointPresentationType.LowPoint;
				else if (View.ActualHighlightStartPoint && extremeIndexes.IsStartPoint(index))
					return PointPresentationType.StartPoint;
				else if (View.ActualHighlightEndPoint && extremeIndexes.IsEndPoint(index))
					return PointPresentationType.EndPoint;
				else if(Data[index].Value < 0 && View.ActualShowNegativePoint) {
					return PointPresentationType.NegativePoint;
				}
			}
			return PointPresentationType.SimplePoint;
		}
		protected SolidColorBrush GetSolidBrush(Color color) {
			if(localCache != null)
				return localCache.GetSolidBrush(color);
			throw new Exception("Incorrect cache");
		}
		protected Pen GetPen(SolidColorBrush brush, int width) {
			Pen pen = null;
			if(localCache != null)
				pen = localCache.GetPen(brush, width);
			if(pen == null)
				throw new Exception("Incorrect cache");
			if(pen.Thickness != width)
				pen.Thickness = width;
			return pen;
		}
		protected Pen GetPen(Color color, int width) {
			Pen pen = null;
			if(localCache != null)
				pen = localCache.GetPen(localCache.GetSolidBrush(color), width);
			if(pen == null)
				throw new Exception("Incorrect cache");
			if(pen.Thickness != width)
				pen.Thickness = width;
			return pen;
		}
		protected virtual SolidColorBrush GetPointBrush(PointPresentationType pointType) {
			switch(pointType) {
				case PointPresentationType.HighPoint:
					return View.ActualMaxPointBrush;
				case PointPresentationType.LowPoint:
					return View.ActualMinPointBrush;
				case PointPresentationType.StartPoint:
					return View.ActualStartPointBrush;
				case PointPresentationType.EndPoint:
					return View.ActualEndPointBrush;
				case PointPresentationType.NegativePoint:
					return View.ActualNegativePointBrush;
				default:
					return View.ActualBrush;
			}
		}
		protected abstract void DrawInternal(DrawingContext drawingContext);
		public void Initialize(IList<SparklinePoint> data, SparklineControl view, SparklineMappingBase mapping, ExtremePointIndexes extremeIndexes) {
			if (view == null || data == null)
				return;
			this.data = data;
			this.view = view;
			this.extremeIndexes = extremeIndexes;
			if (view.Type != SparklineType)
				return;
			this.mapping = mapping;
		}
		public void Draw(DrawingContext drawingContext) {
			if(mapping != null) {
				if(localCache == null)
					localCache = new SparklineDrawingCache();
				DrawInternal(drawingContext);
			}
		}
	}
}

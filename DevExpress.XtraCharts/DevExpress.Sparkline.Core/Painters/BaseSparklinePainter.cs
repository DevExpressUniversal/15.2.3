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

using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Utils.Drawing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace DevExpress.Sparkline.Core {
	public enum PointPresentationType {
		HighPoint,
		LowPoint,
		StartPoint,
		EndPoint,
		NegativePoint,
		SimplePoint
	}
	public abstract class BaseSparklinePainter : IDisposable {
		readonly Stack antialiasingStack = new Stack();
		bool disposed;
		Rectangle clippingBounds;
		SparklineDataProvider dataProvider;
		SparklineMappingBase mapping;
		SparklineViewBase view;
		IGraphicsCache cache;
		SparklineDrawingCache localCache;
		protected SparklineDataProvider DataProvider { get { return dataProvider; } }
		protected SparklineViewBase View { get { return view; } }
		protected abstract bool EnableAntialiasing { get; }
		internal SparklineMappingBase Mapping { get { return mapping; } }
		public abstract SparklineViewType SparklineType { get; }
		Rectangle InflateRelatively(Rectangle rect, SparklineRangeData range) {
			rect.Width = SparklineMathUtils.Round(rect.Width / (range.Max - range.Min));
			rect.X = SparklineMathUtils.Round(rect.X - rect.Width * range.Min);
			return rect;
		}
		Padding DeterminePadding(ISparklineSettings settings) {
			Padding markersPadding = GetMarkersPadding();
			return new Padding() {
				Left = Math.Max(markersPadding.Left, settings.Padding.Left),
				Right = Math.Max(markersPadding.Right, settings.Padding.Right),
				Top = Math.Max(markersPadding.Top, settings.Padding.Top),
				Bottom = Math.Max(markersPadding.Bottom, settings.Padding.Bottom)
			};
		}
		protected void SetAntialiasingMode(Graphics graphics, SmoothingMode mode) {
			antialiasingStack.Push(graphics.SmoothingMode);
			graphics.SmoothingMode = mode;
		}
		protected void RestoreAntialiasingMode(Graphics graphics) {
			if (antialiasingStack.Count > 0) {
				graphics.SmoothingMode = (SmoothingMode)antialiasingStack.Pop();
			}
		}
		protected int GetIndexOfFirstPointForDrawing() {
			return (DataProvider.FilteredPointRange.Min == 0) ? 0 : DataProvider.FilteredPointRange.Min - 1;
		}
		protected int GetIndexOfLastPointForDrawing() {
			return (DataProvider.FilteredPointRange.Max == DataProvider.SortedPoints.Count - 1) ? DataProvider.SortedPoints.Count - 1 : DataProvider.FilteredPointRange.Max + 1;
		}
		protected internal SolidBrush GetSolidBrush(Color color) {
			if (cache != null)
				return (SolidBrush)cache.GetSolidBrush(color);
			if (localCache != null)
				return localCache.GetSolidBrush(color);
			throw new Exception("Incorrect cache");
		}
		protected internal Pen GetPen(Color color, int width) {
			Pen pen = null;
			if (cache != null)
				pen = cache.GetPen(color);
			else if (localCache != null)
				pen = localCache.GetPen(color);
			if (pen == null)
				throw new Exception("Incorrect cache");
			if (pen.Width != width)
				pen.Width = width;
			return pen;
		}
		protected internal PointPresentationType GetPointPresentationType(int index) {
			if (index >= 0 && index < DataProvider.SortedPoints.Count) {
				if (View.HighlightMaxPoint && mapping.IsMaxValuePoint(index))
					return PointPresentationType.HighPoint;
				else if (View.HighlightMinPoint && mapping.IsMinValuePoint(index))
					return PointPresentationType.LowPoint;
				else if (View.HighlightStartPoint && mapping.IsStartPoint(index))
					return PointPresentationType.StartPoint;
				else if (View.HighlightEndPoint && mapping.IsEndPoint(index))
					return PointPresentationType.EndPoint;
				else if (DataProvider.SortedPoints[index].Value < 0 && View.ActualShowNegativePoint) {
					return PointPresentationType.NegativePoint;
				}
			}
			return PointPresentationType.SimplePoint;
		}
		protected internal virtual Color GetPointColor(PointPresentationType pointType) {
			switch (pointType) {
				case PointPresentationType.HighPoint:
					return View.ActualMaxPointColor;
				case PointPresentationType.LowPoint:
					return View.ActualMinPointColor;
				case PointPresentationType.StartPoint:
					return View.ActualStartPointColor;
				case PointPresentationType.EndPoint:
					return View.ActualEndPointColor;
				case PointPresentationType.NegativePoint:
					return View.ActualNegativePointColor;
				default:
					return View.ActualColor;
			}
		}
		protected virtual void Dispose(bool disposing) {
			if (!disposed) {
				if (disposing && localCache != null)
					localCache.Dispose();
				localCache = null;
				disposed = true;
			}
		}
		protected abstract void DrawInternal(Graphics graphics);
		protected abstract Padding GetMarkersPadding();
		public void Initialize(ISparklineData data, ISparklineSettings settings, Rectangle bounds) {
			Initialize(new SparklineDataProvider(new SparklineDataWrapper(data), new SparklineRangeData(0, 1)), settings, bounds, null, null);
		}
		public void Initialize(SparklineDataProvider dataProvider, ISparklineSettings settings, Rectangle bounds, SparklineInteractionRanges interactionRanges, Matrix normalTransform) {
			if ((dataProvider == null) || (settings == null))
				return;
			this.dataProvider = dataProvider;
			if (!settings.ValueRange.IsAuto)
				dataProvider.DataValueRange = new SparklineRangeData(settings.ValueRange.Min, settings.ValueRange.Max);
			view = settings.View;
			if ((view == null) || (view.Type != SparklineType))
				throw new ArgumentException("Incorrect sparkline view");
			mapping = null;
			if (dataProvider.SortedPoints.Count > 0) {
				Padding padding = dataProvider.AllowPaddingCorrection ? DeterminePadding(settings) : settings.Padding;
				Rectangle mappingBounds = InflateRelatively(bounds, dataProvider.FilterRange);
				Rectangle finalMappingBounds = new Rectangle(
					mappingBounds.X + padding.Left,
					mappingBounds.Y + padding.Top,
					mappingBounds.Width - padding.Left - padding.Right,
					mappingBounds.Height - padding.Top - padding.Bottom);
				clippingBounds = bounds;
				if (interactionRanges != null)
					mapping = SparklineMappingBase.Create(View.Type, finalMappingBounds, dataProvider, interactionRanges, normalTransform);
				else
					mapping = SparklineMappingBase.Create(View.Type, finalMappingBounds, dataProvider);
			}
		}
		public void Draw(Graphics graphics, IGraphicsCache cache) {
			if (mapping != null) {
				this.cache = cache;
				if (cache == null)
					localCache = new SparklineDrawingCache();
				if (EnableAntialiasing)
					SetAntialiasingMode(graphics, SmoothingMode.HighQuality);
				Region oldClipping = graphics.Clip;
				graphics.SetClip(clippingBounds, CombineMode.Intersect);
				DrawInternal(graphics);
				graphics.SetClip(oldClipping, CombineMode.Replace);
				if (EnableAntialiasing)
					RestoreAntialiasingMode(graphics);
				if (localCache != null) {
					localCache.Dispose();
					localCache = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}

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

using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Globalization;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Diagram {
	public class RulerScalePresenter : FrameworkElement {
		public static readonly DependencyProperty ZoomProperty;
		public static readonly DependencyProperty LabelOffsetProperty;
		public static readonly DependencyProperty MeasureUnitProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty OffsetProperty;
		public static readonly DependencyProperty LocationProperty;
		public static readonly DependencyProperty TickBrushProperty;
		static RulerScalePresenter() {
			var registrator = DependencyPropertyRegistrator<RulerScalePresenter>.New();
			registrator
				.Register(x => x.Zoom, out ZoomProperty, 1, FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.LabelOffset, out LabelOffsetProperty, default(Point), FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(d => d.MeasureUnit, out MeasureUnitProperty, MeasureUnits.Pixels, FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(d => d.Orientation, out OrientationProperty, Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(d => d.Location, out LocationProperty, default(Point), FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.Offset, out OffsetProperty, 0, FrameworkPropertyMetadataOptions.AffectsRender)
				.Register(x => x.TickBrush, out TickBrushProperty, default(Brush), FrameworkPropertyMetadataOptions.AffectsRender)
				.OverrideMetadata(ClipToBoundsProperty, true)
				;
			FormattedTextCache.DependentProperties
				.ForEach(property => registrator.OverrideMetadata(property, x => x.InvalidateCache()));
		}
		public Brush TickBrush {
			get { return (Brush)GetValue(TickBrushProperty); }
			set { SetValue(TickBrushProperty, value); }
		}
		public double Zoom {
			get { return (double)GetValue(ZoomProperty); }
			set { SetValue(ZoomProperty, value); }
		}
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		public Point Location {
			get { return (Point)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public Point LabelOffset {
			get { return (Point)GetValue(LabelOffsetProperty); }
			set { SetValue(LabelOffsetProperty, value); }
		}
		public MeasureUnit MeasureUnit {
			get { return (MeasureUnit)GetValue(MeasureUnitProperty); }
			set { SetValue(MeasureUnitProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public RulerScalePresenter() {
			formattedTextCache = new FormattedTextCache(this);
			InvalidateCache();
		}
		protected Pen Pen { get { return pen.Value; } }
		protected Pen TickPen { get { return tickPen.Value; } }
		protected FormattedTextCache formattedTextCache;
		void InvalidateCache() {
			pen = new Lazy<Pen>(() => new Pen(TextBlock.GetForeground(this), 1).Do(x => x.Freeze()));
			tickPen = new Lazy<Pen>(() => new Pen(TickBrush, 1).Do(x => x.Freeze()));
			formattedTextCache.Invalidate();
			InvalidateVisual();
		}
		protected override void OnRender(DrawingContext drawingContext) {
			var tickMaxSize = Orientation.Rotate().GetSize(RenderSize);
			RulerRenderHelper.DrawRuler(MeasureUnit, Orientation, RenderSize, Offset - Orientation.GetPoint(Location), Zoom,
			(position, weight) => {
				DrawTick(drawingContext, tickMaxSize, position, weight);
			},
			(position, label) => {
				DrawLabel(drawingContext, position, label);
			});
		}
		void DrawTick(DrawingContext drawingContext, double totalSize, double position, double weight) {
			double actualPosition = Math.Round(position) + .5;
			Point from = Orientation.MakePoint(actualPosition, totalSize * (1 - weight) + 1);
			Point to = Orientation.MakePoint(actualPosition, totalSize);
			drawingContext.DrawLine(TickPen, from, to);
		}
		void DrawLabel(DrawingContext drawingContext, double position, double label) {
			var textLocation = Orientation.MakePoint(position, 0).OffsetPoint(LabelOffset);
			var formattedText = formattedTextCache.Get(GetLabelText(label));
			if(Orientation == Orientation.Vertical)
				textLocation = textLocation.OffsetY(formattedText.Width);
			drawingContext.PushTransform(new RotateTransform(Orientation.GetRotationAngle(), textLocation.X, textLocation.Y));
			drawingContext.DrawText(formattedText, textLocation);
			drawingContext.Pop();
		}
		string GetLabelText(double label) {
			return string.Format(CultureInfo.CurrentCulture, "{0:0.##}", label);
		}
		Lazy<Pen> pen, tickPen;
	}
}

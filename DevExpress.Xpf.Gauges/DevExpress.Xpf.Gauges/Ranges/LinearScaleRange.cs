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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class LinearScaleRange : RangeBase {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(LinearScaleRangePresentation), typeof(LinearScaleRange), new PropertyMetadata(null, PresentationPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleRangePresentation"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleRangePresentation Presentation {
			get { return (LinearScaleRangePresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("LinearScaleRangePredefinedPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedLinearScaleRangePresentations.PresentationKinds; }
		}
		new LinearScale Scale { get { return base.Scale as LinearScale; } }
		LinearGaugeControl Gauge { get { return Scale != null ? Scale.Gauge : null; } }
		LinearScaleRangeModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetRangeModel(Scale.Ranges.IndexOf(this)) : null; } }
		protected override LayerPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultLinearScaleRangePresentation();
			}
		}
		protected internal override RangeOptions ActualOptions {
			get {
				if (Options != null)
					return Options;
				if (Model != null && Model.Options != null)
					return Model.Options;
				return new RangeOptions();
			}
		}
		double GetRotationAngleByScaleLayoutMode() {
			double angle = 0.0;
			switch (Scale.LayoutMode) {
				case LinearScaleLayoutMode.TopToBottom: angle = 180.0;
					break;
				case LinearScaleLayoutMode.LeftToRight: angle = 90.0;
					break;
				case LinearScaleLayoutMode.RightToLeft: angle = -90.0;
					break;
			}
			return angle;
		}
		double GetClipPointYByScaleLayoutMode(double value) {
			double pointCoordinate = 0.0;
			if (Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom || Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop)
				pointCoordinate = Math.Abs(Scale.Mapping.GetPointByValue(value, true).Y - (Scale.Mapping.Layout.AnchorPoint.Y + Scale.Mapping.Layout.ScaleVector.Y));
			else
				pointCoordinate = Math.Abs(Scale.Mapping.GetPointByValue(value, true).X - (Scale.Mapping.Layout.AnchorPoint.X + Scale.Mapping.Layout.ScaleVector.X));
			return pointCoordinate;
		}
		protected override ElementLayout CreateLayout(Size constraint) {
			if (Scale != null && Scale.Mapping != null && !Scale.Mapping.Layout.IsEmpty) {
				double rangeBarHeight;
				if (Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop || Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom)
					rangeBarHeight = Scale.Mapping.Layout.InitialBounds.Height;
				else
					rangeBarHeight = Scale.Mapping.Layout.InitialBounds.Width;
				return new ElementLayout(ActualOptions.Thickness, rangeBarHeight);
			}
			else
				return null;
		}
		protected override void CompleteLayout(ElementInfoBase elementInfo) {
			Point arrangePoint = Scale.Mapping.Layout.AnchorPoint;
			if (Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop || Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom)
				arrangePoint.X += ActualOptions.Offset;
			else
				arrangePoint.Y += ActualOptions.Offset;
			Point offset = Scale.GetLayoutOffset();
			arrangePoint.X += offset.X;
			arrangePoint.Y += offset.Y;
			double rotationAngle = GetRotationAngleByScaleLayoutMode();
			RotateTransform transform = new RotateTransform() { Angle = rotationAngle };
			RectangleGeometry clipGeometry = new RectangleGeometry();
			Point clipAnchorPoint = new Point(0.0, GetClipPointYByScaleLayoutMode(StartValueAbsolute));
			Point clipValuePoint = new Point(ActualOptions.Thickness, GetClipPointYByScaleLayoutMode(EndValueAbsolute));
			clipGeometry.Rect = new Rect(clipAnchorPoint, clipValuePoint);
			elementInfo.Layout.CompleteLayout(arrangePoint, transform, clipGeometry);
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LinearScaleRange();
		}
	}
	public class LinearScaleRangeCollection : LayerCollection<LinearScaleRange> {
		public LinearScaleRangeCollection(LinearScale scale)
			: base(scale) {
		}
	}
}

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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class LinearScaleRangeBarOptions : RangeBarOptionsBase {
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(LinearScaleRangeBarOptions), new PropertyMetadata(10, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleRangeBarOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LinearScaleRangeBarOptions();
		}
	}
	public class LinearScaleRangeBar : LinearScaleIndicator {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(LinearScaleRangeBarPresentation), typeof(LinearScaleRangeBar), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty AnchorValueProperty = DependencyPropertyManager.Register("AnchorValue",
			typeof(double), typeof(LinearScaleRangeBar), new PropertyMetadata(0.0, IndicatorPropertyChanged));
		public static readonly DependencyProperty OptionsProperty = DependencyPropertyManager.Register("Options",
			typeof(LinearScaleRangeBarOptions), typeof(LinearScaleRangeBar), new PropertyMetadata(OptionsPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleRangeBarPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleRangeBarPresentation Presentation {
			get { return (LinearScaleRangeBarPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleRangeBarAnchorValue"),
#endif
		Category(Categories.Data)
		]
		public double AnchorValue {
			get { return (double)GetValue(AnchorValueProperty); }
			set { SetValue(AnchorValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleRangeBarOptions"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleRangeBarOptions Options {
			get { return (LinearScaleRangeBarOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LinearScaleRangeBar rangeBar = d as LinearScaleRangeBar;
			if (rangeBar != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as LinearScaleRangeBarOptions, e.NewValue as LinearScaleRangeBarOptions, rangeBar);
				rangeBar.OnOptionsChanged();
			}
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("LinearScaleRangeBarPredefinedPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedLinearScaleRangeBarPresentations.PresentationKinds; }
		}
		LinearScaleRangeBarModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetRangeBarModel(Scale.RangeBars.IndexOf(this)) : null; } }
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
		protected override ValueIndicatorPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultLinearScaleRangeBarPresentation();
			}
		}
		internal LinearScaleRangeBarOptions ActualOptions {
			get {
				if (Options != null)
					return Options;
				if (Model != null && Model.Options != null)
					return Model.Options;
				return new LinearScaleRangeBarOptions();
			}
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LinearScaleRangeBar();
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
			Point clipAnchorPoint = new Point(0.0, GetPointYByScaleLayoutMode(AnchorValue));
			Point clipValuePoint = new Point(ActualOptions.Thickness, GetPointYByScaleLayoutMode(ActualValue));
			clipGeometry.Rect = new Rect(clipAnchorPoint, clipValuePoint);
			elementInfo.Layout.CompleteLayout(arrangePoint, transform, clipGeometry);
		}
	}
	public class LinearScaleRangeBarCollection : LinearScaleIndicatorCollection<LinearScaleRangeBar> {
		public LinearScaleRangeBarCollection(LinearScale scale)
			: base(scale) {
		}
	}
}

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
	public enum ArcScaleMarkerOrientation {
		RadialToCenter,
		RadialFromCenter,
		Tangent,
		Normal,
		UpsideDown,
		RotateClockwise,
		RotateCounterclockwise
	}
	public abstract class MarkerOptionsBase : GaugeDependencyObject {
		public static readonly DependencyProperty OffsetProperty = DependencyPropertyManager.Register("Offset",
			typeof(double), typeof(MarkerOptionsBase), new PropertyMetadata(-32.0, NotifyPropertyChanged));
		public static readonly DependencyProperty FactorWidthProperty = DependencyPropertyManager.Register("FactorWidth",
			typeof(double), typeof(MarkerOptionsBase), new PropertyMetadata(1.0, NotifyPropertyChanged), FactorsValidation);
		public static readonly DependencyProperty FactorHeightProperty = DependencyPropertyManager.Register("FactorHeight",
			typeof(double), typeof(MarkerOptionsBase), new PropertyMetadata(1.0, NotifyPropertyChanged), FactorsValidation);
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("MarkerOptionsBaseOffset"),
#endif
		Category(Categories.Layout)
		]
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("MarkerOptionsBaseFactorWidth"),
#endif
		Category(Categories.Layout)
		]
		public double FactorWidth {
			get { return (double)GetValue(FactorWidthProperty); }
			set { SetValue(FactorWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("MarkerOptionsBaseFactorHeight"),
#endif
		Category(Categories.Layout)
		]
		public double FactorHeight {
			get { return (double)GetValue(FactorHeightProperty); }
			set { SetValue(FactorHeightProperty, value); }
		}
		static bool FactorsValidation(object value) {
			return (double)value > 0;
		}
	}
	public class ArcScaleMarkerOptions : MarkerOptionsBase {
		public static readonly DependencyProperty OrientationProperty = DependencyPropertyManager.Register("Orientation",
			typeof(ArcScaleMarkerOrientation), typeof(ArcScaleMarkerOptions), new PropertyMetadata(ArcScaleMarkerOrientation.RadialToCenter, NotifyPropertyChanged));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(ArcScaleMarkerOptions), new PropertyMetadata(100, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleMarkerOptionsOrientation"),
#endif
		Category(Categories.Layout)
		]
		public ArcScaleMarkerOrientation Orientation {
			get { return (ArcScaleMarkerOrientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleMarkerOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleMarkerOptions();
		}
	}
	public class ArcScaleMarker : ArcScaleIndicator {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(ArcScaleMarkerPresentation), typeof(ArcScaleMarker), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty OptionsProperty = DependencyPropertyManager.Register("Options",
			typeof(ArcScaleMarkerOptions), typeof(ArcScaleMarker), new PropertyMetadata(OptionsPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleMarkerPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public ArcScaleMarkerPresentation Presentation {
			get { return (ArcScaleMarkerPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleMarkerOptions"),
#endif
		Category(Categories.Presentation)
		]
		public ArcScaleMarkerOptions Options {
			get { return (ArcScaleMarkerOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ArcScaleMarker marker = d as ArcScaleMarker;
			if (marker != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ArcScaleMarkerOptions, e.NewValue as ArcScaleMarkerOptions, marker);
				marker.OnOptionsChanged();
			}
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("ArcScaleMarkerPredefinedPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedArcScaleMarkerPresentations.PresentationKinds; }
		}
		ArcScaleMarkerModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetMarkerModel(Scale.Markers.IndexOf(this)) : null; } }
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
		protected override ValueIndicatorPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultArcScaleMarkerPresentation();
			}
		}
		internal ArcScaleMarkerOptions ActualOptions {
			get {
				if (Options != null)
					return Options;
				if (Model != null && Model.Options != null)
					return Model.Options;
				return new ArcScaleMarkerOptions();
			}
		}
		double CorrectAngleByOrientation(double angle) {
			switch (ActualOptions.Orientation) {
				case ArcScaleMarkerOrientation.RadialToCenter: return angle;
				case ArcScaleMarkerOrientation.RadialFromCenter: return angle + 180;
				case ArcScaleMarkerOrientation.Tangent: return angle - 90;
				case ArcScaleMarkerOrientation.Normal: return 0;
				case ArcScaleMarkerOrientation.UpsideDown: return 180;
				case ArcScaleMarkerOrientation.RotateClockwise: return 90;
				case ArcScaleMarkerOrientation.RotateCounterclockwise: return -90;
				default: return angle;
			}
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleMarker();
		}
		protected override ElementLayout CreateLayout(Size constraint) {
			return Scale != null && Scale.Mapping != null && !Scale.Mapping.Layout.IsEmpty ? new ElementLayout() : null;
		}
		protected override void CompleteLayout(ElementInfoBase elementInfo) {
			double valueAngle = Scale.Mapping.GetAngleByValue(ActualValue);
			double markerAngle = CorrectAngleByOrientation(valueAngle);
			TransformGroup transform = new TransformGroup();
			transform.Children.Add(new ScaleTransform() { ScaleX = ActualOptions.FactorWidth, ScaleY = ActualOptions.FactorHeight });
			transform.Children.Add(new RotateTransform() { Angle = markerAngle });
			Point anchorPoint = Scale.Mapping.GetPointByValue(ActualValue, ActualOptions.Offset);
			Point offset = Scale.GetLayoutOffset();
			anchorPoint.X += offset.X;
			anchorPoint.Y += offset.Y;
			elementInfo.Layout.CompleteLayout(anchorPoint, transform, null);
		}
	}
	public class ArcScaleMarkerCollection : ArcScaleIndicatorCollection<ArcScaleMarker> {
		public ArcScaleMarkerCollection(ArcScale scale)
			: base(scale) {
		}
	}
}

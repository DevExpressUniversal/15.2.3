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
	public enum LinearScaleMarkerOrientation {
		Normal,
		Reversed
	}
	public class LinearScaleMarkerOptions : MarkerOptionsBase {
		public static readonly DependencyProperty OrientationProperty = DependencyPropertyManager.Register("Orientation",
			typeof(LinearScaleMarkerOrientation), typeof(LinearScaleMarkerOptions), new PropertyMetadata(LinearScaleMarkerOrientation.Normal, NotifyPropertyChanged));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(LinearScaleMarkerOptions), new PropertyMetadata(100, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleMarkerOptionsOrientation"),
#endif
		Category(Categories.Layout)
		]
		public LinearScaleMarkerOrientation Orientation {
			get { return (LinearScaleMarkerOrientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleMarkerOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LinearScaleMarkerOptions();
		}
	}
	public class LinearScaleMarker : LinearScaleIndicator {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(LinearScaleMarkerPresentation), typeof(LinearScaleMarker), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty OptionsProperty = DependencyPropertyManager.Register("Options",
			typeof(LinearScaleMarkerOptions), typeof(LinearScaleMarker), new PropertyMetadata(OptionsPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleMarkerPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleMarkerPresentation Presentation {
			get { return (LinearScaleMarkerPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleMarkerOptions"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleMarkerOptions Options {
			get { return (LinearScaleMarkerOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LinearScaleMarker marker = d as LinearScaleMarker;
			if (marker != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as LinearScaleMarkerOptions, e.NewValue as LinearScaleMarkerOptions, marker);
				marker.OnOptionsChanged();
			}
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("LinearScaleMarkerPredefinedPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedLinearScaleMarkerPresentations.PresentationKinds; }
		}
		LinearScaleMarkerModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetMarkerModel(Scale.Markers.IndexOf(this)) : null; } }
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
		protected override ValueIndicatorPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultLinearScaleMarkerPresentation();
			}
		}
		internal LinearScaleMarkerOptions ActualOptions {
			get {
				if (Options != null)
					return Options;
				if (Model != null && Model.Options != null)
					return Model.Options;
				return new LinearScaleMarkerOptions();
			}
		}
		double GetActualFactorWidth() {
			return Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop || Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom ? ActualOptions.FactorWidth : ActualOptions.FactorHeight;
		}
		double GetActualFactorHeight() {
			return Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop || Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom ? ActualOptions.FactorHeight : ActualOptions.FactorWidth;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LinearScaleMarker();
		}
		protected override ElementLayout CreateLayout(Size constraint) {
			return Scale != null && Scale.Mapping != null && !Scale.Mapping.Layout.IsEmpty ? new ElementLayout() : null;
		}
		protected override void CompleteLayout(ElementInfoBase elementInfo) {
			double scaleFactorX = ActualOptions.Orientation == LinearScaleMarkerOrientation.Normal ? GetActualFactorWidth() : -GetActualFactorWidth();
			double rotationAngle = Scale.LayoutMode == LinearScaleLayoutMode.LeftToRight || Scale.LayoutMode == LinearScaleLayoutMode.RightToLeft ? 90.0 : 0.0;
			TransformGroup transform = new TransformGroup();
			transform.Children.Add(new ScaleTransform() { ScaleX = scaleFactorX, ScaleY = GetActualFactorHeight() });
			transform.Children.Add(new RotateTransform() { Angle = rotationAngle });
			Point anchorPoint = Scale.Mapping.GetPointByValue(ActualValue, ActualOptions.Offset);
			Point offset = Scale.GetLayoutOffset();
			anchorPoint.X += offset.X;
			anchorPoint.Y += offset.Y;
			elementInfo.Layout.CompleteLayout(anchorPoint, transform, null);
		}
	}
	public class LinearScaleMarkerCollection : LinearScaleIndicatorCollection<LinearScaleMarker> {
		public LinearScaleMarkerCollection(LinearScale scale)
			: base(scale) {
		}
	}
}

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
	public class ArcScaleNeedleOptions : GaugeDependencyObject {
		public static readonly DependencyProperty StartOffsetProperty = DependencyPropertyManager.Register("StartOffset",
			typeof(double), typeof(ArcScaleNeedleOptions), new PropertyMetadata(0.0, NotifyPropertyChanged));
		public static readonly DependencyProperty EndOffsetProperty = DependencyPropertyManager.Register("EndOffset",
			typeof(double), typeof(ArcScaleNeedleOptions), new PropertyMetadata(37.0, NotifyPropertyChanged));
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(ArcScaleNeedleOptions), new PropertyMetadata(150, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleNeedleOptionsStartOffset"),
#endif
		Category(Categories.Layout)
		]
		public double StartOffset {
			get { return (double)GetValue(StartOffsetProperty); }
			set { SetValue(StartOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleNeedleOptionsEndOffset"),
#endif
		Category(Categories.Layout)
		]
		public double EndOffset {
			get { return (double)GetValue(EndOffsetProperty); }
			set { SetValue(EndOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleNeedleOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleNeedleOptions();
		}
	}
	public class ArcScaleNeedle : ArcScaleIndicator {		
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(ArcScaleNeedlePresentation), typeof(ArcScaleNeedle), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty OptionsProperty = DependencyPropertyManager.Register("Options",
			typeof(ArcScaleNeedleOptions), typeof(ArcScaleNeedle), new PropertyMetadata(OptionsPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleNeedlePresentation"),
#endif
		Category(Categories.Presentation)
		]
		public ArcScaleNeedlePresentation Presentation {
			get { return (ArcScaleNeedlePresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleNeedleOptions"),
#endif
		Category(Categories.Presentation)
		]
		public ArcScaleNeedleOptions Options {
			get { return (ArcScaleNeedleOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ArcScaleNeedle needle = d as ArcScaleNeedle;
			if (needle != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ArcScaleNeedleOptions, e.NewValue as ArcScaleNeedleOptions, needle);
				needle.OnOptionsChanged();
			}
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("ArcScaleNeedlePredefinedPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedArcScaleNeedlePresentations.PresentationKinds; }
		}
		const int lengthPrecision = 1;
		ArcScaleNeedleModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetNeedleModel(Scale.ActualLayoutMode, Scale.Needles.IndexOf(this)) : null; } } 
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
		protected override ValueIndicatorPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultArcScaleNeedlePresentation();
			}
		}
		internal ArcScaleNeedleOptions ActualOptions { 
			get {
				if (Options != null)
					return Options;
				if (Model != null && Model.Options != null)
					return Model.Options;
				return new ArcScaleNeedleOptions(); 
			} 
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleNeedle();
		}
		protected override ElementLayout CreateLayout(Size constraint) {
			if (Scale != null && Scale.Mapping != null) {
				Point centerPoint = Scale.Mapping.Layout.EllipseCenter;
				Point scalePoint = Scale.Mapping.GetPointByValue(ActualValue, -ActualOptions.EndOffset);
				double needleLength = Math.Round(MathUtils.CalculateDistance(centerPoint, scalePoint) - ActualOptions.StartOffset, lengthPrecision);
				needleLength = needleLength >= 0 ? needleLength : 0;
				if (!Scale.Mapping.Layout.IsEmpty)
					return new ElementLayout(needleLength);
			}
			return null;
		}
		protected override void CompleteLayout(ElementInfoBase elementInfo) {
			double needleThickness = elementInfo.PresentationControl.DesiredSize.Height;
			double needleAngle = Scale.Mapping.GetAngleByValue(ActualValue);
			Transform transform = new RotateTransform() { Angle = needleAngle };
			Point anchorPoint = Scale.Mapping.Layout.EllipseCenter;
			anchorPoint.X += ActualOptions.StartOffset * Math.Cos(MathUtils.Degree2Radian(needleAngle));
			anchorPoint.Y += ActualOptions.StartOffset * Math.Sin(MathUtils.Degree2Radian(needleAngle));
			Point offset = Scale.GetLayoutOffset();
			anchorPoint.X += offset.X;
			anchorPoint.Y += offset.Y;
			elementInfo.Layout.CompleteLayout(anchorPoint, transform, null);
		}
	}
	public class ArcScaleNeedleCollection : ArcScaleIndicatorCollection<ArcScaleNeedle> {
		public ArcScaleNeedleCollection(ArcScale scale) : base(scale) {
		}
	}
}

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
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class RangeBarOptionsBase : GaugeDependencyObject {
		public static readonly DependencyProperty OffsetProperty = DependencyPropertyManager.Register("Offset",
			typeof(double), typeof(RangeBarOptionsBase), new PropertyMetadata(-90.0, NotifyPropertyChanged));
		public static readonly DependencyProperty ThicknessProperty = DependencyPropertyManager.Register("Thickness",
			typeof(int), typeof(RangeBarOptionsBase), new PropertyMetadata(10, NotifyPropertyChanged), ThicknessValidation);
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeBarOptionsBaseOffset"),
#endif
		Category(Categories.Layout)
		]
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeBarOptionsBaseThickness"),
#endif
		Category(Categories.Layout)
		]
		public int Thickness {
			get { return (int)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}
		static bool ThicknessValidation(object value) {
			return (int)value > 0;
		}
	}
	public class ArcScaleRangeBarOptions : RangeBarOptionsBase {
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(ArcScaleRangeBarOptions), new PropertyMetadata(50, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleRangeBarOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleRangeBarOptions();
		}
	}
	public class ArcScaleRangeBar : ArcScaleIndicator {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(ArcScaleRangeBarPresentation), typeof(ArcScaleRangeBar), new PropertyMetadata(null, PresentationPropertyChanged));
		public static readonly DependencyProperty AnchorValueProperty = DependencyPropertyManager.Register("AnchorValue",
			typeof(double), typeof(ArcScaleRangeBar), new PropertyMetadata(0.0, IndicatorPropertyChanged));
		public static readonly DependencyProperty OptionsProperty = DependencyPropertyManager.Register("Options",
			typeof(ArcScaleRangeBarOptions), typeof(ArcScaleRangeBar), new PropertyMetadata(OptionsPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleRangeBarPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public ArcScaleRangeBarPresentation Presentation {
			get { return (ArcScaleRangeBarPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleRangeBarAnchorValue"),
#endif
		Category(Categories.Data)
		]
		public double AnchorValue {
			get { return (double)GetValue(AnchorValueProperty); }
			set { SetValue(AnchorValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ArcScaleRangeBarOptions"),
#endif
		Category(Categories.Presentation)
		]
		public ArcScaleRangeBarOptions Options {
			get { return (ArcScaleRangeBarOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		static void OptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ArcScaleRangeBar rangeBar = d as ArcScaleRangeBar;
			if (rangeBar != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ArcScaleRangeBarOptions, e.NewValue as ArcScaleRangeBarOptions, rangeBar);
				rangeBar.OnOptionsChanged();
			}
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("ArcScaleRangeBarPredefinedPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedPresentations { get { return PredefinedArcScaleRangeBarPresentations.PresentationKinds; } }
		ArcScaleRangeBarModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetRangeBarModel(Scale.RangeBars.IndexOf(this)) : null; } } 
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
		protected override ValueIndicatorPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Model != null && Model.Presentation != null)
					return Model.Presentation;
				return new DefaultArcScaleRangeBarPresentation();
			}
		}		
		internal ArcScaleRangeBarOptions ActualOptions { 
			get {
				if (Options != null)
					return Options;
				if (Model != null && Model.Options != null)
					return Model.Options;
				return new ArcScaleRangeBarOptions(); 
			} 
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleRangeBar();
		}
		protected override ElementLayout CreateLayout(Size constraint) {
			if (Scale != null && Scale.Mapping != null && !Scale.Mapping.Layout.IsEmpty) {
				return ArcSegmentCalculator.CreateRangeLayout(Scale.Mapping, ActualOptions.Offset, ActualOptions.Thickness);
			}
			else
				return null;
		}
		protected override void CompleteLayout(ElementInfoBase elementInfo) {
			double startValue = Scale.GetLimitedValue(AnchorValue);
			ArcSegmentCalculator.CompleteRangeLayout(elementInfo, Scale, startValue, ActualValue, ActualOptions.Offset, ActualOptions.Thickness);
		}
	}
	public class ArcScaleRangeBarCollection : ArcScaleIndicatorCollection<ArcScaleRangeBar> {
		public ArcScaleRangeBarCollection(ArcScale scale) : base(scale) {
		}
	}
}

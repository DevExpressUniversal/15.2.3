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
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public enum LinearScaleLayoutMode {
		LeftToRight,
		RightToLeft,
		BottomToTop,
		TopToBottom
	}
	public class LinearScale : Scale {
		public static readonly DependencyProperty LayoutModeProperty = DependencyPropertyManager.Register("LayoutMode",
			typeof(LinearScaleLayoutMode), typeof(LinearScale), new PropertyMetadata(LinearScaleLayoutMode.BottomToTop, InvalidateLayout));
		public static readonly DependencyProperty LabelOptionsProperty = DependencyPropertyManager.Register("LabelOptions",
			typeof(LinearScaleLabelOptions), typeof(LinearScale), new PropertyMetadata(null, LabelOptionsPropertyChanged));
		internal static readonly DependencyPropertyKey MarkersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Markers",
			typeof(LinearScaleMarkerCollection), typeof(LinearScale), new PropertyMetadata());
		public static readonly DependencyProperty MarkersProperty = MarkersPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey RangeBarsPropertyKey = DependencyPropertyManager.RegisterReadOnly("RangeBars",
			typeof(LinearScaleRangeBarCollection), typeof(LinearScale), new PropertyMetadata());
		public static readonly DependencyProperty RangeBarsProperty = RangeBarsPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey LevelBarsPropertyKey = DependencyPropertyManager.RegisterReadOnly("LevelBars",
			typeof(LinearScaleLevelBarCollection), typeof(LinearScale), new PropertyMetadata());
		public static readonly DependencyProperty LevelBarsProperty = LevelBarsPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey LayersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Layers",
			typeof(LinearScaleLayerCollection), typeof(LinearScale), new PropertyMetadata());
		public static readonly DependencyProperty LayersProperty = LayersPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey RangesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Ranges",
			typeof(LinearScaleRangeCollection), typeof(LinearScale), new PropertyMetadata());
		public static readonly DependencyProperty RangesProperty = RangesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty LinePresentationProperty = DependencyPropertyManager.Register("LinePresentation",
			typeof(LinearScaleLinePresentation), typeof(LinearScale), new PropertyMetadata(null, PresentationPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLayoutMode"),
#endif
		Category(Categories.Layout)
		]
		public LinearScaleLayoutMode LayoutMode {
			get { return (LinearScaleLayoutMode)GetValue(LayoutModeProperty); }
			set { SetValue(LayoutModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLabelOptions"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleLabelOptions LabelOptions {
			get { return (LinearScaleLabelOptions)GetValue(LabelOptionsProperty); }
			set { SetValue(LabelOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleMarkers"),
#endif
		Category(Categories.Elements)
		]
		public LinearScaleMarkerCollection Markers {
			get { return (LinearScaleMarkerCollection)GetValue(MarkersProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleRangeBars"),
#endif
		Category(Categories.Elements)
		]
		public LinearScaleRangeBarCollection RangeBars {
			get { return (LinearScaleRangeBarCollection)GetValue(RangeBarsProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLevelBars"),
#endif
		Category(Categories.Elements)
		]
		public LinearScaleLevelBarCollection LevelBars {
			get { return (LinearScaleLevelBarCollection)GetValue(LevelBarsProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLayers"),
#endif
		Category(Categories.Elements)
		]
		public LinearScaleLayerCollection Layers {
			get { return (LinearScaleLayerCollection)GetValue(LayersProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleRanges"),
#endif
		Category(Categories.Elements)
		]
		public LinearScaleRangeCollection Ranges {
			get { return (LinearScaleRangeCollection)GetValue(RangesProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLinePresentation"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleLinePresentation LinePresentation {
			get { return (LinearScaleLinePresentation)GetValue(LinePresentationProperty); }
			set { SetValue(LinePresentationProperty, value); }
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("LinearScalePredefinedLinePresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedLinePresentations {
			get { return PredefinedLinearScaleLinePresentations.PresentationKinds; }
		}
		internal override ScaleLinePresentation ActualLinePresentation {
			get {
				if(LinePresentation != null)
					return LinePresentation;
				if(Model != null && ((LinearScaleModel)Model).LinePresentation != null)
					return ((LinearScaleModel)Model).LinePresentation;
				return new DefaultLinearScaleLinePresentation();
			}
		}
		protected override ScaleModel Model { get { return Gauge != null && Gauge.ActualModel != null ? Gauge.ActualModel.GetScaleModel(Gauge.Scales.IndexOf(this)) : null; } }
		protected internal override ScaleLabelOptions ActualLabelOptions {
			get {
				if(LabelOptions != null)
					return LabelOptions;
				if(Model != null && ((LinearScaleModel)Model).LabelOptions != null)
					return ((LinearScaleModel)Model).LabelOptions;
				return new LinearScaleLabelOptions();
			}
		}
		protected internal override IEnumerable<IElementInfo> Elements {
			get {
				foreach(LinearScaleLayer layer in Layers)
					yield return layer.ElementInfo;
				foreach(ValueIndicatorBase indicator in Indicators)
					foreach(ValueIndicatorInfo elementInfo in indicator.Elements)
						yield return elementInfo;
				foreach(LinearScaleRange range in Ranges)
					yield return range.ElementInfo;
				yield return LineInfo;
				yield return MinorTickmarksInfo;
				yield return MajorTickmarksInfo;
				yield return LabelsInfo;
				foreach (ScaleCustomLabel label in CustomLabels)
					yield return label;
				foreach (ScaleCustomElement element in CustomElements)
					yield return element;
			}
		}
		protected internal override IEnumerable<ValueIndicatorBase> Indicators {
			get {
				if(Markers != null)
					foreach(LinearScaleIndicator indicator in Markers)
						yield return indicator;
				if(RangeBars != null)
					foreach(LinearScaleIndicator indicator in RangeBars)
						yield return indicator;
				if(LevelBars != null)
					foreach(LinearScaleIndicator indicator in LevelBars)
						yield return indicator;
			}
		}
		internal new LinearScaleMapping Mapping { get { return base.Mapping as LinearScaleMapping; } }
		internal new LinearGaugeControl Gauge { get { return base.Gauge as LinearGaugeControl; } }
		protected override ScaleElementLayout CalculateLineLayout() {
			if(!ActualShowLine || Mapping.Layout.IsEmpty)
				return null;
			double angle = 0;
			switch(Mapping.Scale.LayoutMode) {
				case LinearScaleLayoutMode.BottomToTop:
					angle = 180;
					break;
				case LinearScaleLayoutMode.LeftToRight:
					angle = 270;
					break;
				case LinearScaleLayoutMode.RightToLeft:
					angle = 90;
					break;
				case LinearScaleLayoutMode.TopToBottom:
					angle = 0;
					break;
			}
			Point startPoint = Mapping.GetPointByPercent(0, ActualLineOptions.Offset);
			Size size = new Size(Math.Max(ActualLineOptions.Thickness, 0),
				Math.Max(MathUtils.CalculateDistance(startPoint, Mapping.GetPointByPercent(1, ActualLineOptions.Offset)), 0));
			return new ScaleElementLayout(angle, startPoint, size);
		}
		public LinearScale() {
			DefaultStyleKey = typeof(LinearScale);
			this.SetValue(MarkersPropertyKey, new LinearScaleMarkerCollection(this));
			this.SetValue(RangeBarsPropertyKey, new LinearScaleRangeBarCollection(this));
			this.SetValue(LevelBarsPropertyKey, new LinearScaleLevelBarCollection(this));
			this.SetValue(LayersPropertyKey, new LinearScaleLayerCollection(this));
			this.SetValue(RangesPropertyKey, new LinearScaleRangeCollection(this));
			UpdateElementsInfo();
		}
		protected override void UpdateModel() {
			base.UpdateModel();
			foreach(LinearScaleLayer layer in Layers)
				((IModelSupported)layer).UpdateModel();
			foreach(LinearScaleRange range in Ranges)
				((IModelSupported)range).UpdateModel();
		}
		protected internal override void CheckIndicatorEnterLeaveRange(ValueIndicatorBase indicator, double oldValue, double newValue) {
			foreach(LinearScaleRange range in Ranges)
				range.OnIndicatorEnterLeave(indicator, oldValue, newValue);
		}
		protected override ScaleMapping CalculateMapping(Size constraint) {
			return new LinearScaleMapping(this, new Rect(0, 0, constraint.Width, constraint.Height));
		}
	}
	public class LinearScaleCollection : ScaleCollection<LinearScale> {
		public LinearScaleCollection(LinearGaugeControl gauge)
			: base(gauge) {
		}
	}
}

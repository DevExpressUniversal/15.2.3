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
	public class LinearScaleLayer : ScaleLayerBase {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(LinearScaleLayerPresentation), typeof(LinearScaleLayer), new PropertyMetadata(null, PresentationPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearScaleLayerPresentation"),
#endif
		Category(Categories.Presentation)
		]
		public LinearScaleLayerPresentation Presentation {
			get { return (LinearScaleLayerPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("LinearScaleLayerPredefinedPresentations")]
#endif
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedLinearScaleLayerPresentations.PresentationKinds; }
		}
		LinearScale Scale { get { return Owner as LinearScale; } }
		LinearGaugeControl Gauge { get { return Scale != null ? Scale.Gauge : null; } }
		protected override LayerPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Gauge != null && Gauge.ActualModel != null) {
					LayerModel model = Gauge.ActualModel.GetScaleLayerModel(Scale.Layers.IndexOf(this));
					if (model != null && model.Presentation != null)
						return model.Presentation;
				}
				return new DefaultLinearScaleBackgroundLayerPresentation();
			}
		}
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
		internal LayerOptions ActualOptions {
			get {
				if (Options != null)
					return Options;
				if (Gauge != null && Gauge.ActualModel != null) {
					LayerModel model = Gauge.ActualModel.GetScaleLayerModel(Scale.Layers.IndexOf(this));
					if (model != null && model.Options != null)
						return model.Options;
				}
				return new LayerOptions();
			}
		}
		Point GetArrangePointByScaleLayoutMode() {
			double arrangePointX = 0.0;
			double arrangePointY = 0.0;
			if (Scale.LayoutMode == LinearScaleLayoutMode.TopToBottom || Scale.LayoutMode == LinearScaleLayoutMode.BottomToTop) {
				arrangePointX = Scale.Mapping.Layout.AnchorPoint.X;
				arrangePointY = Scale.Mapping.Layout.AnchorPoint.Y + Scale.Mapping.Layout.ScaleVector.Y / 2.0;
			}
			else {
				arrangePointX = Scale.Mapping.Layout.AnchorPoint.X + Scale.Mapping.Layout.ScaleVector.X / 2.0;
				arrangePointY = Scale.Mapping.Layout.AnchorPoint.Y;
			}
			return new Point(arrangePointX, arrangePointY);
		}
		protected override GaugeDependencyObject CreateObject() {
			return new LinearScaleLayer();
		}
		protected override ElementLayout CreateLayout(Size constraint) {
			if(Scale != null && Scale.Mapping != null && !Scale.Mapping.Layout.IsEmpty)
				return new ElementLayout(Scale.Mapping.Layout.InitialBounds.Width, Scale.Mapping.Layout.InitialBounds.Height);
			else
				return null;
		}
		protected override void CompleteLayout(ElementInfoBase elementInfo) {
			Point arrangePoint = GetArrangePointByScaleLayoutMode();
			Point offset = Scale.GetLayoutOffset();
			arrangePoint.X += offset.X;
			arrangePoint.Y += offset.Y;			
			elementInfo.Layout.CompleteLayout(arrangePoint, null, null);
		}
	}
	public class LinearScaleLayerCollection : LayerCollection<LinearScaleLayer> {
		public LinearScaleLayerCollection(LinearScale scale) : base(scale) {
		}
	}
}

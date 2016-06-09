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
	public class DigitalGaugeLayer : GaugeLayerBase {
		public static readonly DependencyProperty PresentationProperty = DependencyPropertyManager.Register("Presentation",
			typeof(DigitalGaugeLayerPresentation), typeof(DigitalGaugeLayer), new PropertyMetadata(null, PresentationPropertyChanged));
		[
		Category(Categories.Presentation)
		]
		public DigitalGaugeLayerPresentation Presentation {
			get { return (DigitalGaugeLayerPresentation)GetValue(PresentationProperty); }
			set { SetValue(PresentationProperty, value); }
		}
		public static List<PredefinedElementKind> PredefinedPresentations {
			get { return PredefinedDigitalGaugeLayerPresentations.PresentationKinds; }
		}
		new DigitalGaugeControl Gauge { get { return base.Gauge as DigitalGaugeControl; } }
		protected override LayerPresentation ActualPresentation {
			get {
				if (Presentation != null)
					return Presentation;
				if (Gauge != null && Gauge.ActualModel != null) {
					LayerModel model = Gauge.ActualModel.GetLayerModel(Gauge.Layers.IndexOf(this));
					if (model != null && model.Presentation != null)
						return model.Presentation;
				}
				return new DefaultDigitalGaugeBackgroundLayerPresentation();
			}
		}
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
		LayerOptions ActualOptions {
			get {
				if (Options != null)
					return Options;
				if (Gauge != null && Gauge.ActualModel != null) {
					LayerModel model = Gauge.ActualModel.GetLayerModel(Gauge.Layers.IndexOf(this));
					if (model != null && model.Options != null)
						return model.Options;
				}
				return new LayerOptions();
			}
		}
		protected override GaugeDependencyObject CreateObject() {
			return new DigitalGaugeLayer();
		}
		protected override ElementLayout CreateLayout(Size constraint) {
			return new ElementLayout();
		}
	}
	public class DigitalGaugeLayerCollection : GaugeLayerCollection<DigitalGaugeLayer> {
		public DigitalGaugeLayerCollection(DigitalGaugeControl gauge) : base(gauge) {
		}
	}
}

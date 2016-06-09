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
using System.Windows.Automation.Peers;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Gauges.Localization;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[DXToolboxBrowsableAttribute]
	public class CircularGaugeControl : AnalogGaugeControl {
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(CircularGaugeModel), typeof(CircularGaugeControl), new PropertyMetadata(null, ModelProperytChanged));
		internal static readonly DependencyPropertyKey LayersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Layers",
			typeof(CircularGaugeLayerCollection), typeof(CircularGaugeControl), new PropertyMetadata());
		public static readonly DependencyProperty LayersProperty = LayersPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey ScalesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Scales",
			typeof(ArcScaleCollection), typeof(CircularGaugeControl), new PropertyMetadata());
		public static readonly DependencyProperty ScalesProperty = ScalesPropertyKey.DependencyProperty;						
		static readonly DependencyPropertyKey ActualModelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualModel",
			typeof(CircularGaugeModel), typeof(CircularGaugeControl), new PropertyMetadata(null, ActualModelProperytChanged));
		public static readonly DependencyProperty ActualModelProperty = ActualModelPropertyKey.DependencyProperty;
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("CircularGaugeControlModel"),
#endif
		Category(Categories.Presentation)
		]
		public CircularGaugeModel Model {
			get { return (CircularGaugeModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("CircularGaugeControlLayers"),
#endif
		Category(Categories.Elements)
		]
		public CircularGaugeLayerCollection Layers {
			get { return (CircularGaugeLayerCollection)GetValue(LayersProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("CircularGaugeControlScales"),
#endif
		Category(Categories.Elements)
		]
		public ArcScaleCollection Scales {
			get { return (ArcScaleCollection)GetValue(ScalesProperty); }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public CircularGaugeModel ActualModel {
			get { return (CircularGaugeModel)GetValue(ActualModelProperty); }
		}
		static void ModelProperytChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CircularGaugeControl gauge = d as CircularGaugeControl;
			if (gauge != null) {
				CircularGaugeModel model = e.NewValue as CircularGaugeModel;
				if (model == null)
					gauge.SetValue(CircularGaugeControl.ActualModelPropertyKey, new CircularDefaultModel());
				else
					gauge.SetValue(CircularGaugeControl.ActualModelPropertyKey, model);
			}
		}
		static void ActualModelProperytChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IModelSupported obj = d as IModelSupported;
			IOwnedElement model = e.NewValue as IOwnedElement;
			if (model != null)
				model.Owner = d as CircularGaugeControl;
			if (obj != null)
				obj.UpdateModel();
		}
		static CircularGaugeControl() {
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("CircularGaugeControlPredefinedModels")]
#endif
		public static List<PredefinedElementKind> PredefinedModels {
			get { return PredefinedCircularGaugeModels.ModelKinds; }
		}
		UIElement ClipElement { get { return GetTemplateChild("PART_ClipElement") as UIElement; } }
		public CircularGaugeControl() {
			DefaultStyleKey = typeof(CircularGaugeControl);
			this.SetValue(ActualModelPropertyKey, new CircularDefaultModel());
			this.SetValue(LayersPropertyKey, new CircularGaugeLayerCollection(this));
			this.SetValue(ScalesPropertyKey, new ArcScaleCollection(this));			
		}
		protected internal override void Animate() {
			foreach (ArcScale scale in Scales)
				scale.AnimateIndicators(DesignerProperties.GetIsInDesignMode(this));
		}
		protected override void UpdateModel() {
			if (Scales != null)
				foreach (ArcScale scale in Scales)
					((IModelSupported)scale).UpdateModel();
			if (Layers != null)
				foreach (CircularGaugeLayer layer in Layers)
					((IModelSupported)layer).UpdateModel();
		}
		protected override Size ArrangeOverride(Size finalSize) {
			RectangleGeometry clipGeometry = new RectangleGeometry();
			clipGeometry.Rect = new Rect(new Point(0.0, 0.0), finalSize);
			ClipElement.Clip = clipGeometry;
			return base.ArrangeOverride(finalSize);
		}
		protected override void GaugeUnloaded(object sender, RoutedEventArgs e) {
			base.GaugeUnloaded(sender, e);
			foreach (ArcScale scale in Scales) {
				scale.ClearAnimation();
			}
		}
		protected override IEnumerable<IElementInfo> GetElements() { 
			foreach (CircularGaugeLayer layer in Layers)
				 yield return layer.ElementInfo;
			foreach (ArcScale scale in Scales)
				foreach (IElementInfo elementInfo in scale.Elements)
					yield return elementInfo;
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new CircularGaugeControlAutomationPeer(this);
		}
		public CircularGaugeHitInfo CalcHitInfo(Point point) {
			return new CircularGaugeHitInfo(this, point);
		}
	}
	public class CircularGaugeControlAutomationPeer : FrameworkElementAutomationPeer {
		public CircularGaugeControlAutomationPeer(FrameworkElement owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "CircularGaugeControl";
		}
		protected override string GetLocalizedControlTypeCore() {
			return GaugeLocalizer.GetString(GaugeStringId.CircularGaugeLocalizedControlType);
		}
		protected override bool IsContentElementCore() {
			return false;
		}
		protected override string GetHelpTextCore() {
			string helpTextBase = base.GetHelpTextCore();
			if (String.IsNullOrEmpty(helpTextBase))
				return GaugeLocalizer.GetString(GaugeStringId.CircularGaugeAutomationPeerHelpText);
			else
				return helpTextBase;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			CircularGaugeControl gauge = Owner as CircularGaugeControl;
			if (gauge != null)
				foreach (ArcScale scale in gauge.Scales) {
					AutomationPeer scalePeer = FrameworkElementAutomationPeer.CreatePeerForElement(scale);
					if (scalePeer != null)
						children.Add(scalePeer);
				}
			return children;
		}
	}
}

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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Gauges.Localization;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	[DXToolboxBrowsableAttribute]
	public class LinearGaugeControl : AnalogGaugeControl {
		public static readonly DependencyProperty ModelProperty = DependencyPropertyManager.Register("Model",
			typeof(LinearGaugeModel), typeof(LinearGaugeControl), new PropertyMetadata(null, ModelProperytChanged));
		internal static readonly DependencyPropertyKey LayersPropertyKey = DependencyPropertyManager.RegisterReadOnly("Layers",
			typeof(LinearGaugeLayerCollection), typeof(LinearGaugeControl), new PropertyMetadata());
		public static readonly DependencyProperty LayersProperty = LayersPropertyKey.DependencyProperty;
		internal static readonly DependencyPropertyKey ScalesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Scales",
			typeof(LinearScaleCollection), typeof(LinearGaugeControl), new PropertyMetadata());
		public static readonly DependencyProperty ScalesProperty = ScalesPropertyKey.DependencyProperty;
		static readonly DependencyPropertyKey ActualModelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualModel",
			typeof(LinearGaugeModel), typeof(LinearGaugeControl), new PropertyMetadata(null, ActualModelProperytChanged));
		public static readonly DependencyProperty ActualModelProperty = ActualModelPropertyKey.DependencyProperty;
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearGaugeControlModel"),
#endif
		Category(Categories.Presentation)
		]
		public LinearGaugeModel Model {
			get { return (LinearGaugeModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearGaugeControlLayers"),
#endif
		Category(Categories.Elements)
		]
		public LinearGaugeLayerCollection Layers {
			get { return (LinearGaugeLayerCollection)GetValue(LayersProperty); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("LinearGaugeControlScales"),
#endif
		Category(Categories.Elements)
		]
		public LinearScaleCollection Scales {
			get { return (LinearScaleCollection)GetValue(ScalesProperty); }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public LinearGaugeModel ActualModel {
			get { return (LinearGaugeModel)GetValue(ActualModelProperty); }
		}
		static LinearGaugeControl() {
		}
		static void ModelProperytChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LinearGaugeControl gauge = d as LinearGaugeControl;
			if (gauge != null) {
				LinearGaugeModel model = e.NewValue as LinearGaugeModel;
				if (model == null)
					gauge.SetValue(LinearGaugeControl.ActualModelPropertyKey, new LinearDefaultModel());
				else
					gauge.SetValue(LinearGaugeControl.ActualModelPropertyKey, model);
			}
		}
		static void ActualModelProperytChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IModelSupported obj = d as IModelSupported;
			IOwnedElement model = e.NewValue as IOwnedElement;
			if (model != null)
				model.Owner = d as LinearGaugeControl;
			if (obj != null)
				obj.UpdateModel();
		}
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("LinearGaugeControlPredefinedModels")]
#endif
		public static List<PredefinedElementKind> PredefinedModels {
			get { return PredefinedLinearGaugeModels.ModelKinds; }
		}
		public LinearGaugeControl() {
			DefaultStyleKey = typeof(LinearGaugeControl);
			this.SetValue(ActualModelPropertyKey, new LinearDefaultModel());
			this.SetValue(LayersPropertyKey, new LinearGaugeLayerCollection(this));
			this.SetValue(ScalesPropertyKey, new LinearScaleCollection(this));
		}
		protected internal override void Animate() {
			foreach (LinearScale scale in Scales)
				scale.AnimateIndicators(DesignerProperties.GetIsInDesignMode(this));
		}
		protected override void UpdateModel() {
			if (Scales != null)
				foreach (LinearScale scale in Scales)
					((IModelSupported)scale).UpdateModel();
			if (Layers != null)
				foreach (LinearGaugeLayer layer in Layers)
					((IModelSupported)layer).UpdateModel();
		}
		protected override void GaugeUnloaded(object sender, RoutedEventArgs e) {
			base.GaugeUnloaded(sender, e);
			foreach (LinearScale scale in Scales) {
				scale.ClearAnimation();
			}
		}
		protected override IEnumerable<IElementInfo> GetElements() {
			foreach (LinearGaugeLayer layer in Layers)
				yield return layer.ElementInfo;
			foreach (LinearScale scale in Scales)
				foreach (IElementInfo elementInfo in scale.Elements)
					yield return elementInfo;
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new LinearGaugeControlAutomationPeer(this);
		}
		public LinearGaugeHitInfo CalcHitInfo(Point point) {
			return new LinearGaugeHitInfo(this, point);
		}
	}
	public class LinearGaugeControlAutomationPeer : FrameworkElementAutomationPeer {
		public LinearGaugeControlAutomationPeer(FrameworkElement owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "LinearGaugeControl";
		}
		protected override string GetLocalizedControlTypeCore() {
			return GaugeLocalizer.GetString(GaugeStringId.LinearGaugeLocalizedControlType);
		}
		protected override bool IsContentElementCore() {
			return false;
		}
		protected override string GetHelpTextCore() {
			string helpTextBase = base.GetHelpTextCore();
			if (String.IsNullOrEmpty(helpTextBase))
				return GaugeLocalizer.GetString(GaugeStringId.LinearGaugeAutomationPeerHelpText);
			else
				return helpTextBase;
		}
		protected override List<AutomationPeer> GetChildrenCore() {
			List<AutomationPeer> children = new List<AutomationPeer>();
			LinearGaugeControl gauge = Owner as LinearGaugeControl;
			if (gauge != null)
				foreach (LinearScale scale in gauge.Scales) {
					AutomationPeer scalePeer = FrameworkElementAutomationPeer.CreatePeerForElement(scale);
					if (scalePeer != null)
						children.Add(scalePeer);
				}
			return children;
		}
	}
}

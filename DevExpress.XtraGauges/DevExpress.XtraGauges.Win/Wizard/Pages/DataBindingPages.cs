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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using DevExpress.XtraGauges.Win.Gauges.Digital;
using DevExpress.XtraGauges.Win.Gauges.Linear;
using DevExpress.XtraGauges.Win.Gauges.State;
namespace DevExpress.XtraGauges.Win.Wizard {
	public abstract class GaugeDataBindingPage<T> : BaseGaugeDesignerPage
		where T : class, IBindableComponent, INamed {
		SplitContainerControl splitContainer;
		DesignerPropertyGrid propertyGridCore;
		DesignerBindableItemsList<T> bindableItemsListCore;
		public GaugeDataBindingPage(int index, string caption)
			: base(2, index, caption, UIHelper.UIOtherImages[0]) {
			BindableItemsList.SelectedItemChanged += OnSelectedItemChanged;
		}
		protected override void OnCreate() {
			this.splitContainer = new SplitContainerControl();
			this.propertyGridCore = new DesignerPropertyGrid(DesignerPropertyGridMode.DataBindingView);
			this.bindableItemsListCore = new DesignerBindableItemsList<T>();
			splitContainer.Parent = this;
			splitContainer.Dock = DockStyle.Fill;
			splitContainer.Panel1.MinSize = 200;
			splitContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainer.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainer.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			splitContainer.Panel2.Padding = new Padding(2);
			BindableItemsList.Parent = splitContainer.Panel1;
			BindableItemsList.Dock = DockStyle.Fill;
			PropertyGrid.Parent = splitContainer.Panel2;
			PropertyGrid.Dock = DockStyle.Fill;
		}
		protected override void OnDispose() {
			if(PropertyGrid != null) {
				PropertyGrid.Parent = null;
				PropertyGrid.Dispose();
				propertyGridCore = null;
			}
			if(BindableItemsList != null) {
				BindableItemsList.SelectedItemChanged -= OnSelectedItemChanged;
				BindableItemsList.Parent = null;
				BindableItemsList.Dispose();
				bindableItemsListCore = null;
			}
		}
		protected DesignerBindableItemsList<T> BindableItemsList {
			get { return bindableItemsListCore; }
		}
		protected DesignerPropertyGrid PropertyGrid {
			get { return propertyGridCore; }
		}
		protected override void OnSetDesignerControl(GaugeDesignerControl designer) {
			PropertyGrid.SetDesignerControl(designer);
		}
		protected internal override bool IsModified { get { return false; } }
		protected internal override void ApplyChanges() { }
		void OnSelectedItemChanged(object primitive) {
			PropertyGrid.SetSelectedPrimitive(primitive);
			PropertyGrid.ExpandAllGridItems();
		}
		protected internal override void LayoutChanged() {
			Invalidate();
			if(!string.IsNullOrEmpty(settings)) {
				splitContainer.SplitterPosition = Int32.Parse(settings);
				settings = null;
			}
		}
		public override string SaveSettings() {
			return splitContainer.SplitterPosition.ToString();
		}
		string settings;
		public override void LoadSettings(string property) {
			settings = property;
		}
		protected internal override bool IsHidden {
			get { return PropertyGrid.ServiceProvider == null || PropertyGrid.ServiceProvider.Site == null; }
		}
	}
	public class CircularGaugeDataBindingPage : GaugeDataBindingPage<ArcScaleComponent> {
		CircularGauge gaugeCore;
		public CircularGaugeDataBindingPage(int index, string caption, CircularGauge gauge)
			: base(index, caption) {
			this.gaugeCore = gauge;
			BindableItemsList.Primitives = Gauge.Scales.ToArray();
		}
		protected override void OnDispose() {
			gaugeCore = null;
			base.OnDispose();
		}
		protected CircularGauge Gauge {
			get { return gaugeCore; }
		}
		protected internal override bool IsAllowed {
			get { return Gauge.Scales.Count > 0; }
		}
	}
	public class LinearGaugeDataBindingPage : GaugeDataBindingPage<LinearScaleComponent> {
		LinearGauge gaugeCore;
		public LinearGaugeDataBindingPage(int index, string caption, LinearGauge gauge)
			: base(index, caption) {
			this.gaugeCore = gauge;
			BindableItemsList.Primitives = Gauge.Scales.ToArray();
		}
		protected override void OnDispose() {
			gaugeCore = null;
			base.OnDispose();
		}
		protected LinearGauge Gauge {
			get { return gaugeCore; }
		}
		protected internal override bool IsAllowed {
			get { return Gauge.Scales.Count > 0; }
		}
	}
	public class StateIndicatorGaugeDataBindingPage : GaugeDataBindingPage<StateIndicatorComponent> {
		StateIndicatorGauge gaugeCore;
		public StateIndicatorGaugeDataBindingPage(int index, string caption, StateIndicatorGauge gauge)
			: base(index, caption) {
			this.gaugeCore = gauge;
			BindableItemsList.Primitives = Gauge.Indicators.ToArray();
		}
		protected override void OnDispose() {
			gaugeCore = null;
			base.OnDispose();
		}
		protected StateIndicatorGauge Gauge {
			get { return gaugeCore; }
		}
		protected internal override bool IsAllowed {
			get { return Gauge.Indicators.Count > 0; }
		}
	}
	public class DigitalGaugeDataBindingPage : GaugeDataBindingPage<DigitalGauge> {
		DigitalGauge gaugeCore;
		public DigitalGaugeDataBindingPage(int index, string caption, DigitalGauge gauge)
			: base(index, caption) {
			this.gaugeCore = gauge;
			BindableItemsList.Primitives = new DigitalGauge[] { Gauge };
		}
		protected override void OnDispose() {
			gaugeCore = null;
			base.OnDispose();
		}
		protected DigitalGauge Gauge {
			get { return gaugeCore; }
		}
		protected internal override bool IsAllowed {
			get { return Gauge != null; }
		}
	}
}

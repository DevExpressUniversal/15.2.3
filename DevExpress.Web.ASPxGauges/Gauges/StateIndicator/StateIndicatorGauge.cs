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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
namespace DevExpress.Web.ASPxGauges.Gauges.State {
	public class StateIndicatorGaugeProvider : BaseGaugeProviderWeb {
		StateIndicatorComponentCollection indicatorsCore;
		public StateIndicatorGaugeProvider(StateIndicatorGauge owner, BaseGaugeChangedHandler handler)
			: base(owner, handler) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.indicatorsCore = new StateIndicatorComponentCollection();
			Indicators.CollectionChanged += OnIndicatorsCollectionChanged;
		}
		protected override void OnDispose() {
			if(Indicators != null) {
				Indicators.CollectionChanged -= OnIndicatorsCollectionChanged;
				Indicators.Dispose();
				indicatorsCore = null;
			}
			base.OnDispose();
		}
		void OnIndicatorsCollectionChanged(CollectionChangedEventArgs<StateIndicatorComponent> ea) {
			OnCollectionElementChanged(ea.ChangedType, ea.Element);
		}
		public StateIndicatorComponentCollection Indicators {
			get { return indicatorsCore; }
		}
		public override void BuildModel(BaseGaugeModel model) {
			base.BuildModel(model);
			model.Composite.AddRange(Indicators.ToArray());
		}
	}
	[ToolboxItem(false), DesignTimeVisibleAttribute(false)]
	[Designer("DevExpress.Web.ASPxGauges.Design.StateIndicatorGaugeDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.ComponentModel.Design.IDesigner))]
	public class StateIndicatorGauge : BaseGaugeWeb, IStateIndicatorGauge {
		public StateIndicatorGauge()
			: base() {
		}
		protected override void OnDispose() {
			base.OnDispose();
		}
		protected override BaseGaugeProviderWeb CreateGaugeProvider() {
			return new StateIndicatorGaugeProvider(this, OnComponentsChanged);
		}
		protected override BaseGaugeModel CreateModel() {
			BaseGaugeModel model = new StateIndicatorGaugeModel(this);
			StateIndicatorGaugeProvider.BuildModel(model);
			return model;
		}
		protected StateIndicatorGaugeProvider StateIndicatorGaugeProvider {
			get { return GaugeProvider as StateIndicatorGaugeProvider; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public StateIndicatorComponentCollection Indicators {
			get { return StateIndicatorGaugeProvider.Indicators; }
		}
		protected override List<ISerizalizeableElement> GetChildernCore() {
			List<ISerizalizeableElement> list = new List<ISerizalizeableElement>(base.GetChildernCore());
			CollectChildren(list, Indicators, "Indicators");
			return list;
		}
		protected override void ClearCore() {
			base.ClearCore();
			Indicators.Clear();
		}
		protected override string GetPrefixType() { return "state"; }
		protected override void InitializeDefaultCore() {
			AddIndicator();
		}
		public StateIndicatorComponent AddIndicator() {
			BeginUpdate();
			string[] names = new string[Indicators.Count];
			int i = 0;
			Indicators.Accept(
					delegate(StateIndicatorComponent s) { names[i++] = s.Name; }
				);
			StateIndicatorComponent indicator = new StateIndicatorComponent(UniqueNameHelper.GetUniqueName(Prefix("Indicator"), names, Indicators.Count + 1));
			InitializeIndicatorDefault(indicator);
			Indicators.Add(indicator);
			AddComponentToDesignTimeSurface(indicator);
			EndUpdate();
			return indicator;
		}
		protected void InitializeIndicatorDefault(StateIndicator indicator) {
			indicator.BeginUpdate();
			indicator.States.Add(new IndicatorStateWeb("Default"));
			indicator.EndUpdate();
		}
		protected override void SetEnabledCore(bool enabled) {
			ComponentCollectionExtention.SetEnabled(Labels, enabled);
			ComponentCollectionExtention.SetEnabled(Indicators, enabled);
		}
		protected sealed override List<string> GetNamesCore() {
			List<string> names = new List<string>(base.GetNamesCore());
			ComponentCollectionExtention.CollectNames(Indicators, names);
			return names;
		}
		protected sealed override void AddGaugeElementToComponentCollection(IComponent component) {
			if(ComponentCollectionExtention.TryAddComponent(Labels, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Indicators, component)) return;
		}
		protected sealed override BaseElement<IRenderableElement> DuplicateGaugeElementCore(BaseElement<IRenderableElement> component) {
			IComponent duplicate = null;
			if(ComponentCollectionExtention.TryDuplicateComponent(Labels, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Indicators, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			return duplicate as BaseElement<IRenderableElement>;
		}
		protected override IStateManagedHierarchyObject[] GetStateManagedHierarchyObjects() {
			List<IStateManagedHierarchyObject> objects = new List<IStateManagedHierarchyObject>(base.GetStateManagedHierarchyObjects());
			objects.Add(Indicators);
			return objects.ToArray();
		}
		protected internal override void CheckElementsAffinity() { }
	}
	public class StateIndicatorGaugeModel : BaseGaugeModel {
		public StateIndicatorGaugeModel(BaseGaugeWeb gauge)
			: base(gauge) {
		}
		protected StateIndicatorGauge StateIndicatorGauge {
			get { return Owner as StateIndicatorGauge; }
		}
		public override void Calc(IGauge owner, RectangleF bounds) {
			OnShapesChanged();
			base.Calc(owner, bounds);
		}
		public override SizeF ContentSize {
			get { return new SizeF(250, 250); }
		}
		protected override CustomizeActionInfo[] GetActionsCore() {
			ArrayList list = new ArrayList(GetActions());
			list.AddRange(base.GetActionsCore());
			return (CustomizeActionInfo[])list.ToArray(typeof(CustomizeActionInfo));
		}
		CustomizeActionInfo[] actions;
		CustomizeActionInfo[] GetActions() {
			if(actions == null)
				actions = new CustomizeActionInfo[]{
					new CustomizeActionInfo("RunDesigner", "Run CircularGauge Designer", "Run Designer", UIHelper.GaugeTypeImages[2]),
					new CustomizeActionInfo("AddIndicator", "Add default StateIndicator", "Add Indicator", UIHelper.CircularGaugeElementImages[7],"Indicators"),
				};
			return actions;
		}
	}
}

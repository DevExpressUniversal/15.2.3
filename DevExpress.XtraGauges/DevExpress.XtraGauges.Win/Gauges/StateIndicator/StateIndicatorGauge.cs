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
using System.Linq;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Wizard;
namespace DevExpress.XtraGauges.Win.Gauges.State {
	public class StateIndicatorGaugeProvider : BaseGaugeProviderWin, IStateIndicatorGauge {
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
	[ToolboxItem(false)]
	[Designer("DevExpress.XtraGauges.Win.Design.StateIndicatorGaugeDesigner, " + AssemblyInfo.SRAssemblyGaugesDesignWin, typeof(System.ComponentModel.Design.IDesigner))]
	public class StateIndicatorGauge : BaseGaugeWin, IStateIndicatorGauge {
		public StateIndicatorGauge() : base() { }
		protected override void OnDispose() {
			base.OnDispose();
		}
		protected override BaseGaugeProviderWin CreateGaugeProvider() {
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
		[
#if !SL
	DevExpressXtraGaugesWinLocalizedDescription("StateIndicatorGaugeIndicators"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
			StateIndicatorGaugeExtention.InitializeIndicatorDefault(indicator);
		}
		protected void RunDesigner(bool editIsWin) {
			using(GaugeDesignerForm designerform = new GaugeDesignerForm(this)) {
				designerform.Pages = new BaseGaugeDesignerPage[] { 
					new StateIndicatorGaugeOverviewDesignerPage("StateIndicator Gauge"),
					new PrimitiveCustomizationDesignerPage<StateIndicatorComponent>(1,"Indicators",UIHelper.CircularGaugeElementImages[7], Indicators.ToArray(),this) ,
					new PrimitiveCustomizationDesignerPage<LabelComponent>(9,"Labels",UIHelper.UIOtherImages[1],Labels.ToArray(),this) , 
					new PrimitiveCustomizationDesignerPage<ImageIndicatorComponent>(9,"Images",UIHelper.UIOtherImages[2],Images.ToArray(),this) ,
					new StateIndicatorGaugeDataBindingPage(10,"Data Bindings",this)
				};
				if(!editIsWin) {
					List<BaseGaugeDesignerPage> pages = designerform.Pages.ToList();
					pages.RemoveAll(p => p.Caption.Equals("Images") || p.Caption.Equals("Image Indicators"));
					designerform.Pages = pages.ToArray();
				}
				designerform.ShowDialog();
			}
		}
		protected override void SetEnabledCore(bool enabled) {
			ComponentCollectionExtention.SetEnabled(Labels, enabled);
			ComponentCollectionExtention.SetEnabled(Images, enabled);
			ComponentCollectionExtention.SetEnabled(Indicators, enabled);
		}
		protected sealed override List<string> GetNamesCore() {
			List<string> names = new List<string>(base.GetNamesCore());
			ComponentCollectionExtention.CollectNames(Indicators, names);
			return names;
		}
		protected sealed override void AddGaugeElementToComponentCollection(IComponent component) {
			if(ComponentCollectionExtention.TryAddComponent(Labels, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Images, component)) return;
			if(ComponentCollectionExtention.TryAddComponent(Indicators, component)) return;
		}
		protected sealed override BaseElement<IRenderableElement> DuplicateGaugeElementCore(BaseElement<IRenderableElement> component) {
			IComponent duplicate = null;
			if(ComponentCollectionExtention.TryDuplicateComponent(Labels, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Images, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			if(ComponentCollectionExtention.TryDuplicateComponent(Indicators, component as IComponent, out duplicate)) return duplicate as BaseElement<IRenderableElement>;
			return duplicate as BaseElement<IRenderableElement>;
		}
	}
	public class StateIndicatorGaugeModel : BaseGaugeModel {
		public StateIndicatorGaugeModel(BaseGaugeWin gauge)
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
			ArrayList list = new ArrayList(
				new CustomizeActionInfo[]{
					new CustomizeActionInfo("RunDesigner", "Run CircularGauge Designer", "Run Designer", UIHelper.GaugeTypeImages[2]),
					new CustomizeActionInfo("AddIndicator", "Add default StateIndicator", "Add Indicator", UIHelper.CircularGaugeElementImages[7],"Indicators"),
				});
			list.AddRange(base.GetActionsCore());
			return (CustomizeActionInfo[])list.ToArray(typeof(CustomizeActionInfo));
		}
	}
}

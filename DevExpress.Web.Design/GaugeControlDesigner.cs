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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using DevExpress.Utils.About;
using DevExpress.Utils.Serializing;
using DevExpress.Web.Design;
using DevExpress.Web.ASPxGauges.Design.Base;
using DevExpress.Web.ASPxGauges.Gauges;
using DevExpress.Web.ASPxGauges.Gauges.Circular;
using DevExpress.Web.ASPxGauges.Gauges.Digital;
using DevExpress.Web.ASPxGauges.Gauges.Linear;
using DevExpress.Web.ASPxGauges.Gauges.State;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Layout;
using DevExpress.XtraGauges.Presets.PresetManager;
using DevExpress.Web.ASPxGauges.Base;
using DevExpress.Utils.Design;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
namespace DevExpress.Web.ASPxGauges.Design {
	public class ASPxGaugeControlDesigner : ASPxWebControlDesigner {
		ASPxGaugeControl gaugeControlCore;
		const string ASPxGaugesRegistryPath = "Software\\Developer Express\\ASPxGauges\\";
		const string ASPxGaugesShowDesignerRegistryEntry = "ShowDesigner";
		public override bool IsThemableControl() { return false; }
		public ASPxGaugeControl GaugeControl {
			get { return this.gaugeControlCore; }
		}
		public IGaugeContainer GaugeContainer {
			get { return this.gaugeControlCore as IGaugeContainer; }
		}
		public override void Initialize(IComponent component) {
			this.gaugeControlCore = (ASPxGaugeControl)component;
			((ASPxGaugeControl)component).Designer = this;
			base.Initialize(component);
			GaugeContainer.DesignerLoaded();
			RegisterTagPrefix(typeof(GaugesTagPrefix));
			RegisterTagPrefix(typeof(LinearTagPrefix));
			RegisterTagPrefix(typeof(CircularTagPrefix));
			RegisterTagPrefix(typeof(StateIndicatorTagPrefix));
			RegisterTagPrefix(typeof(DigitalTagPrefix));
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(
				AssemblyInfo.SRAssemblyUtils,
				AssemblyInfo.SRAssemblyGaugesCore,
				AssemblyInfo.SRAssemblyASPxGauges,
				AssemblyInfo.SRAssemblyGaugesPresets
			);
		}
		public static bool ShowDesignerOnComponentAdding {
			get {
				DevExpress.Utils.Design.PropertyStore store = new DevExpress.Utils.Design.PropertyStore(ASPxGaugesRegistryPath);
				if(store == null) return true;
				store.Restore();
				return store.RestoreBoolProperty(ASPxGaugesShowDesignerRegistryEntry, true);
			}
			set {
				DevExpress.Utils.Design.PropertyStore store = new DevExpress.Utils.Design.PropertyStore(ASPxGaugesRegistryPath);
				if(store == null) return;
				store.AddProperty(ASPxGaugesShowDesignerRegistryEntry, value);
				store.Store();
			}
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new WebActionList(this);
		}
		public void RunStyleManager() {
			StyleManager.Show(GaugeControl);
		}
		object syncObj = new object();
		public void RunPresetManager() {
			StartAsyncWhenEditorFormIsActive(RunPresetManagerAction);
		}
		public void CustomizeCurrent() {
			StartAsyncWhenEditorFormIsActive(CustomizeAction);
		}
		void RunPresetManagerAction(IGaugeContainer container) {
			using(PresetManagerForm presetManager = new PresetManagerForm(container)) {
				presetManager.InitCurrentPreset(container);
				presetManager.ShowManagerOnComponentAdding = ShowDesignerOnComponentAdding;
				presetManager.ShowDialog();
				ShowDesignerOnComponentAdding = presetManager.ShowManagerOnComponentAdding;
				if(presetManager.TargetPreset != null && presetManager.DialogResult == System.Windows.Forms.DialogResult.OK) {
					using(new UndoEngineLockHelper(this.GetService(typeof(UndoEngine)) as UndoEngine)) {
						using(new ComponentTransaction(GaugeControl)) {
							GaugeControl.BeginUpdate();
							using(MemoryStream ms = new MemoryStream(presetManager.TargetPreset.LayoutInfo)) {
								new BinaryXtraSerializer().DeserializeObject(GaugeControl, ms, "IGaugeContainer");
								ms.Close();
							}
							GaugeControl.CheckElementsAffinity();
							GaugeControl.CheckGaugesID();
							GaugeControl.CheckViewStateTracking();
							GaugeControl.CancelUpdate();
						}
					}
				}
			}
		}
		void CustomizeAction(IGaugeContainer container) {
			using(PresetCustomizeForm presetCustomzeForm = new PresetCustomizeForm(container)) {
				using(new UndoEngineLockHelper(this.GetService(typeof(UndoEngine)) as UndoEngine)) {
					presetCustomzeForm.ShowDialog();
				}
			}
		}
		void StartAsyncWhenEditorFormIsActive(Action<IGaugeContainer> action) {
			EditorFormBase editForm = GaugeControl.EditorForm as EditorFormBase;
			if(editForm != null) {
				editForm.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				BackgroundWorker asyncTask = new BackgroundWorker();
				asyncTask.RunWorkerCompleted += delegate(object sender, RunWorkerCompletedEventArgs e) {
					DoActionAndUpdate(action);
				};
				asyncTask.RunWorkerAsync(GaugeContainer);
			}
			else DoActionAndUpdate(action);
		}
		void DoActionAndUpdate(Action<IGaugeContainer> action) {
			lock(syncObj) {
				action(GaugeContainer);
			}
			UpdateDesignTimeHtml();
		}
		public override ICollection AssociatedComponents {
			get { return GetComponents(Component as IGaugeContainer); }
		}
		ICollection GetComponents(IGaugeContainer container) {
			ArrayList list = new ArrayList(container.Gauges);
			ArrayList result = new ArrayList();
			foreach(IGauge g in list) {
				List<ISerizalizeableElement> children = ((ISerizalizeableElement)g).GetChildren();
				if(children != null) result.AddRange(children);
			}
			result.AddRange(list);
			return result;
		}
		protected bool IsDesignerUpdateLocked {
			get { return lockChanged > 0; }
		}
		int lockChanged = 0;
		public override void OnComponentChanging(object sender, ComponentChangingEventArgs ce) {
			if(IsDesignerUpdateLocked) return;
			lockChanged++;
			base.OnComponentChanging(sender, ce);
			lockChanged--;
		}
		public override void OnComponentChanged(object sender, ComponentChangedEventArgs ce) {
			if(IsDesignerUpdateLocked) return;
			lockChanged++;
			if(ce.Member != null && (ce.Member.Name == "Width" || ce.Member.Name == "Height")) {
				((ILayoutManagerContainer)GaugeControl).DoLayout();
			}
			base.OnComponentChanged(sender, ce);
			lockChanged--;
		}
		public override void ShowAbout() {
			GaugeControlAboutDialogHelper.ShowAbout(Component.Site);
		}
		public override bool HasCommonDesigner() {
			return false;
		}
	}
	internal class UndoEngineLockHelper : IDisposable {
		bool undoEngineSaveState;
		UndoEngine UndoEngine;
		public UndoEngineLockHelper(UndoEngine uEngine) {
			UndoEngine = uEngine;
			DisableUndoEngine();
		}
		public void Dispose() {
			EnableUndoEngine();
			UndoEngine = null;
		}
		protected void DisableUndoEngine() {
			if(UndoEngine == null) return;
			undoEngineSaveState = UndoEngine.Enabled;
			UndoEngine.Enabled = false;
		}
		protected void EnableUndoEngine() {
			if(UndoEngine == null) return;
			UndoEngine.Enabled = undoEngineSaveState;
		}
	}
	public class LabelComponentDesigner : BaseGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"Name","ZOrder",
				"Shader",
				"AppearanceBackground" ,  "AppearanceText",
				"Position","Size","FormatString","AllowHTMLString",
				"Text","TextOrientation"
			};
		}
	}
}
namespace DevExpress.Web.ASPxGauges.Design.Base {
	public class WebActionList : ASPxWebControlDesignerActionList {
		ASPxGaugeControlDesigner designerCore;
		public WebActionList(ASPxGaugeControlDesigner designer)
			: base(designer) {
			this.designerCore = designer;
		}
		protected override string RunDesignerItemCaption {
			get {
				return "Client Side Events...";
			}
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection items = new DesignerActionItemCollection();
			if(this.GaugeControl != null) {
				items.Add(new DesignerActionHeaderItem("Presets"));
				items.Add(new DesignerActionMethodItem(this, "RunPresetManager", "Run Preset Manager...", "Presets", true));
				items.Add(new DesignerActionMethodItem(this, "CustomizeCurrent", "Customize Gauge Control...", "Presets", true));
				items.Add(new DesignerActionHeaderItem("Control Style"));
				items.Add(new DesignerActionMethodItem(this, "RunStyleManager", "Run Style Manager...", "Control Style", true));
				items.Add(new DesignerActionHeaderItem("Properties"));
				items.Add(new DesignerActionPropertyItem("AutoLayout", "Enable Auto Layout", "Properties"));
			}
			DesignerActionItemCollection baseItems = base.GetSortedActionItems();
			foreach(DesignerActionItem item in baseItems)
				items.Add(item);
			return items;
		}
		public void RunStyleManager() {
			designerCore.RunStyleManager();
		}
		public void RunPresetManager() {
			designerCore.RunPresetManager();
		}
		public void CustomizeCurrent() {
			designerCore.CustomizeCurrent();
		}
		public ASPxGaugeControl GaugeControl {
			get { return this.designerCore.Component as ASPxGaugeControl; }
		}
		public bool AutoLayout {
			get { return (this.GaugeControl == null) ? false : this.GaugeControl.AutoLayout; }
			set { DevExpress.Web.Design.EditorContextHelper.SetPropertyValue(this.designerCore, this.GaugeControl, "AutoLayout", value); }
		}
	}
	public class GaugeDesigner : BaseComponentDesignerSimple {
		protected BaseGaugeWeb Gauge {
			get { return base.Component as BaseGaugeWeb; }
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList components = new ArrayList();
				if(!Gauge.IsDisposing) components.AddRange(Gauge.Labels);
				return components;
			}
		}
	}
	public abstract class BaseGaugeComponentDesigner : BaseComponentDesignerSimple {
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			if(AllowFilterProperties()) FilterPropertiesHelper.PrefilterProperties(properties, GetExpectedProperties());
		}
		protected virtual bool AllowFilterProperties() {
			return !(Component as BaseLeafPrimitive).IsXtraSerializing;
		}
		protected abstract string[] GetExpectedProperties();
	}
}

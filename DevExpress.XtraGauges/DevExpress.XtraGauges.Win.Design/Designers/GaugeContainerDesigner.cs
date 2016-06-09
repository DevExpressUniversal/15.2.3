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
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Presets.PresetManager;
using DevExpress.XtraGauges.Win.Base;
using DevExpress.XtraGauges.Win.Design.Base;
namespace DevExpress.XtraGauges.Win.Design {
	public class GaugeContainerDesigner : BaseParentControlDesigner {
		IToolboxService toolBoxService;
		const string XtraGaugesRegistryPath = "Software\\Developer Express\\XtraGauges\\";
		const string XtraGaugesShowDesignerRegistryEntry = "ShowDesigner";
		public GaugeContainerDesigner()
			: base() {
		}
		public override DesignerActionListCollection ActionLists {
			get {
				DesignerActionListCollection actionLists = new DesignerActionListCollection();
				actionLists.Add(new WinActionList(this));
				if(base.ActionLists != null) actionLists.AddRange(base.ActionLists);
				return actionLists;
			}
		}
		protected GaugeControl GaugeControl {
			get { return Component as GaugeControl; }
		}
		protected IGaugeContainer IGaugeContainerComponent {
			get { return Component as IGaugeContainer; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			IGaugeContainerComponent.DesignerLoaded();
		}
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			base.RegisterActionLists(list);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			if(ToolBoxService != null) ToolBoxService.SetSelectedToolboxItem(null);
			if(ShowDesignerOnComponentAdding) RunPresetManager();
		}
		public static bool ShowDesignerOnComponentAdding {
			get {
				PropertyStore store = new PropertyStore(XtraGaugesRegistryPath);
				if(store == null)
					return true;
				store.Restore();
				return store.RestoreBoolProperty(XtraGaugesShowDesignerRegistryEntry, true);
			}
			set {
				PropertyStore store = new PropertyStore(XtraGaugesRegistryPath);
				if(store == null)
					return;
				store.AddProperty(XtraGaugesShowDesignerRegistryEntry, value);
				store.Store();
			}
		}
		protected override bool GetHitTest(Point point) {
			if(GaugeControl == null || !AllowDesigner || DebuggingState) return false;
			Point pt = GaugeControl.PointToClient(point);
			return GetHitTestCore(pt) ? true : base.GetHitTest(point);
		}
		protected virtual bool GetHitTestCore(Point clientPoint) {
			BasePrimitiveHitInfo hi = (GaugeControl as IGaugeContainer).CalcHitInfo(clientPoint);
			return hi != null && hi.Element != null;
		}
		public void RunStyleManager() {
			StyleManager.Show(GaugeControl);
		}
		public void RunPresetManager() {
			using(PresetManagerForm presetManager = new PresetManagerForm(GaugeControl)) {
				presetManager.InitCurrentPreset(GaugeControl);
				presetManager.ShowManagerOnComponentAdding = ShowDesignerOnComponentAdding;
				presetManager.ShowDialog();
				ShowDesignerOnComponentAdding = presetManager.ShowManagerOnComponentAdding;
				if(presetManager.TargetPreset != null && presetManager.DialogResult == DialogResult.OK) {
					using(new UndoEngineLockHelper(this.GetService(typeof(UndoEngine)) as UndoEngine)) {
						using(MemoryStream ms = new MemoryStream(presetManager.TargetPreset.LayoutInfo)) {
							new BinaryXtraSerializer().DeserializeObject(GaugeControl, ms, "IGaugeContainer");
							GaugeControl.Invalidate();
							ms.Close();
						}
					}
				}
			}
		}
		public void CustomizeCurrent() {
			using(PresetCustomizeForm presetCustomzeForm = new PresetCustomizeForm(GaugeControl)) {
				using(new UndoEngineLockHelper(this.GetService(typeof(UndoEngine)) as UndoEngine)) {
					presetCustomzeForm.ShowDialog();
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ICollection toDispose = AssociatedComponents;
				ISite site = Component.Site;
				foreach(object o in toDispose) {
					if(o is IGauge) ((IGauge)o).SetContainer(null);
				}
				foreach(IComponent componentToDispose in toDispose) {
					site.Container.Remove(componentToDispose);
					componentToDispose.Dispose();
				}
				base.Dispose(disposing);
			}
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList(((IGaugeContainer)Component).Gauges);
				ArrayList result = new ArrayList();
				foreach(IGauge g in list) {
					List<ISerizalizeableElement> children = ((ISerizalizeableElement)g).GetChildren();
					if(children != null) result.AddRange(children);
				}
				result.AddRange(list);
				return result;
			}
		}
		protected override bool AllowEditInherited {
			get { return false; }
		}
		protected IToolboxService ToolBoxService {
			get {
				if(toolBoxService == null) toolBoxService = (IToolboxService)GetService(typeof(IToolboxService));
				return toolBoxService;
			}
		}
	}
	public class GaugeDesigner : BaseComponentDesigner, IGaugeDesigner {
		protected override bool AllowEditInherited { get { return false; } }
		protected BaseGaugeWin Gauge {
			get { return base.Component as BaseGaugeWin; }
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList components = new ArrayList();
				if(!Gauge.IsDisposing) components.AddRange(Gauge.Labels);
				return components;
			}
		}
		protected string Name {
			get { return Component.Site.Name; }
			set {
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host == null || (host != null && !host.Loading))
					Component.Site.Name = value;
			}
		}
		#region IGaugeDesigner implementation
		bool IGaugeDesigner.IsUndoInProgress {
			get {
				UndoEngine undoEngine = GetService(typeof(UndoEngine)) as UndoEngine;
				return (undoEngine != null) && undoEngine.UndoInProgress;
			}
		}
		#endregion
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			ShadowTheProperty(properties, "Name");
		}
		static void ShadowTheProperty(IDictionary properties, string property) {
			PropertyDescriptor prop = (PropertyDescriptor)properties[property];
			if(prop != null)
				properties[property] = TypeDescriptor.CreateProperty(typeof(GaugeDesigner), prop, new Attribute[0]);
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
	public abstract class BaseGaugeComponentDesigner : BaseComponentDesigner {
		protected override bool AllowEditInherited { get { return false; } }
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			if(AllowFilterProperties()) FilterPropertiesHelper.PrefilterProperties(properties, GetExpectedProperties());
		}
		protected virtual bool AllowFilterProperties() {
			var primitive = Component as BaseLeafPrimitive;
			return (primitive != null) && !primitive.IsXtraSerializing;
		}
		protected abstract string[] GetExpectedProperties();
		protected override void PostFilterProperties(IDictionary properties) {
			PropertyDescriptor pd = new ComponentNamePropDescriptor("(Name)",
				new Attribute[] { 
					new DesignOnlyAttribute(true), 
					new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
					new CategoryAttribute("Design"),
					new RefreshPropertiesAttribute(RefreshProperties.All)
				}
			);
			if(properties.Contains("ComponentName")) properties["ComponentName"] = pd;
			else properties.Add("ComponentName", pd);
			base.PostFilterProperties(properties);
		}
		class ComponentNamePropDescriptor : PropertyDescriptor {
			PropertyDescriptor descriptorCore;
			public ComponentNamePropDescriptor(string name, Attribute[] attribs)
				: base(name, attribs) {
				descriptorCore = TypeDescriptor.CreateProperty(typeof(ComponentWrapper), "ComponentName", typeof(string), new Attribute[0]);
			}
			public override Type ComponentType { get { return descriptorCore.ComponentType; } }
			public override bool IsReadOnly { get { return false; } }
			public override Type PropertyType { get { return typeof(string); } }
			public override bool CanResetValue(object component) { return false; }
			public override object GetValue(object component) {
				return new ComponentWrapper(component as IComponent).Name;
			}
			public override void SetValue(object component, object value) {
				new ComponentWrapper(component as IComponent).Name = value as string;
			}
			public override void ResetValue(object component) { }
			public override bool ShouldSerializeValue(object component) { return true; }
		}
		class ComponentWrapper {
			IComponent componentCore;
			public ComponentWrapper(IComponent component) {
				componentCore = component;
			}
			protected IComponent Component { get { return componentCore; } }
			protected ISite Site { get { return Component.Site; } }
			[DesignOnly(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("DesignTime")]
			public string Name {
				get { return Site != null ? Site.Name : null; }
				set { if(Site != null) Site.Name = value; }
			}
		}
	}
	public class LabelComponentDesigner : BaseGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"AppearanceBackground" ,  "AppearanceText",
				"Position","Size","FormatString","AllowHTMLString",
				"Text","TextOrientation","UseColorScheme"
			};
		}
	}
	public class ImageIndicatorComponentDesigner : BaseGaugeComponentDesigner {
		protected override string[] GetExpectedProperties() {
			return new string[] { 
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"AppearanceBackground" ,
				"Position","Size", "Image", "ImageLayoutMode", "StateImages", "StateIndex", "AllowImageSkinning", "Color"
			};
		}
	}
}

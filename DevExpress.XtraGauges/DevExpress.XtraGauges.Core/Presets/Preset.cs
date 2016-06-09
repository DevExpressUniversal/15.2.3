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
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Localization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Styles;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Windows.Forms;
namespace DevExpress.XtraGauges.Core.Presets {
	public enum PresetCategory {
		Digital,
		CircularFull,
		CircularHalf,
		CircularQuarter,
		CircularThreeFourth,
		LinearHorizontal,
		LinearVertical,
		StateIndicator,
		CircularWide,
		Complex
	}
	public class BaseGaugePreset : BaseObject, IBaseGaugePreset, IXtraSerializable {
		string nameCore;
		string descriptionCore;
		byte[] layoutInfoCore;
		PresetCategory presetCategotyCore;
		bool fInitializedCore;
		bool fAllowApplyCore;
		bool fAllowLoadCore;
		bool fAllowSaveCore;
		protected override void OnCreate() {
			this.fInitializedCore = false;
			this.fAllowApplyCore = false;
			this.fAllowLoadCore = false;
			this.fAllowSaveCore = false;
			this.nameCore = string.Empty;
			this.descriptionCore = string.Empty;
		}
		[XtraSerializableProperty]
		public string Name {
			get { return nameCore; }
			set { nameCore = value; }
		}
		[XtraSerializableProperty]
		public string Description {
			get { return descriptionCore; }
			set { descriptionCore = value; }
		}
		[XtraSerializableProperty]
		public byte[] LayoutInfo {
			get { return layoutInfoCore; }
			set { layoutInfoCore = value; }
		}
		public int IconIndex {
			get { return -1; }
		}
		[XtraSerializableProperty]
		public PresetCategory Category {
			get { return presetCategotyCore; }
			set { presetCategotyCore = value; }
		}
		public bool AllowApply {
			get { return IsInitialized && AllowApplyCore; }
		}
		public bool AllowLoad {
			get { return true; }
		}
		public bool AllowSave {
			get { return true; }
		}
		public void Initialize(IGaugeContainer gauge) {
			if (!IsInitialized) {
				this.nameCore = GetName();
				this.fInitializedCore = true;
			}
		}
		public void Apply(IGaugeContainer gauge) {
			if (AllowApply) ApplyCore(gauge);
		}
		public void LoadFromStream(Stream stream) {
			if (AllowLoad) LoadFromStreamCore(stream);
		}
		public void SaveToStream(Stream stream) {
			if (AllowSave) SaveToStreamCore(stream);
		}
		protected bool IsInitialized {
			get { return fInitializedCore; }
		}
		protected virtual bool AllowApplyCore {
			get { return fAllowApplyCore; }
		}
		protected virtual bool AllowSaveCore {
			get { return fAllowSaveCore; }
		}
		protected virtual bool AllowLoadCore {
			get { return fAllowLoadCore; }
		}
		protected virtual void ApplyCore(IGaugeContainer gauge) { }
		protected virtual void LoadFromStreamCore(Stream stream) {
			RestoreLayoutCore(new BinaryXtraSerializer(), stream);
		}
		protected virtual void SaveToStreamCore(Stream stream) {
			SaveLayoutCore(new BinaryXtraSerializer(), stream);
		}
		protected virtual string GetName() { return String.Empty; }
		protected virtual Image GetImage(IGaugeContainer gauge) { return null; }
		public static BaseGaugePreset FromGaugeContainer(IGaugeContainer gauge) {
			BaseGaugePreset preset = new GaugePreset();
			preset.Initialize(gauge);
			return preset;
		}
		void IXtraSerializable.OnEndDeserializing(string layoutVersion) { }
		void IXtraSerializable.OnEndSerializing() { }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {}
		void IXtraSerializable.OnStartSerializing() { }
		protected virtual void SaveLayoutCore(XtraSerializer serializer, object path) {
			StartStoreRestore(true);
			Stream stream = path as Stream;
			if (stream != null)
				serializer.SerializeObject(this, stream, this.GetType().Name);
			else
				serializer.SerializeObject(this, path.ToString(), this.GetType().Name);
			EndStoreRestore(true);
		}
		protected virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			StartStoreRestore(false);
			Stream stream = path as Stream;
			if (stream != null)
				serializer.DeserializeObject(this, stream, this.GetType().Name);
			else
				serializer.DeserializeObject(this, path.ToString(), this.GetType().Name);
			EndStoreRestore(false);
		}
		protected virtual void StartStoreRestore(bool isStoring) { }
		protected virtual void EndStoreRestore(bool isStoring) { }
	}
	public class GaugePreset : BaseGaugePreset {
		public GaugePreset()
			: base() {
		}
		protected override bool AllowLoadCore { get { return true; } }
		protected override bool AllowSaveCore { get { return true; } }
		protected override bool AllowApplyCore { 
			get { return false; } 
		}
		protected override string GetName() {
			return "Current";
		}
		protected override Image GetImage(IGaugeContainer container) {
			return container.GetImage(container.Bounds.Width, container.Bounds.Height);
		}
		protected override void ApplyCore(IGaugeContainer gauge) { }
	}
	public class PresetHelper : IXtraSerializable {
		IGaugeContainer owner;
		public PresetHelper(IGaugeContainer owner) {
			this.owner = owner;
		}
		public void OnAddItem(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<IGauge> ea) {
			if(itemsCore != null && !itemsCore.Contains(ea.Element as ISerizalizeableElement))
				itemsCore.Add(ea.Element as ISerizalizeableElement);
		}
		public void OnRemoveItem(DevExpress.XtraGauges.Core.Base.CollectionChangedEventArgs<IGauge> ea) {
			if(itemsCore != null) itemsCore.Remove(ea.Element as ISerizalizeableElement);
		}
		protected string GetTypeName() {
			return typeof(IGaugeContainer).Name;
		}
		public virtual void SaveLayoutCore(XtraSerializer serializer, object path) {
			StartStoreRestore(true);
			Stream stream = path as Stream;
			if(stream != null)
				serializer.SerializeObject(owner, stream, GetTypeName());
			else
				serializer.SerializeObject(owner, path.ToString(), GetTypeName());
			EndStoreRestore(true);
		}
		public virtual void RestoreLayoutCore(XtraSerializer serializer, object path) {
			StartStoreRestore(false);
			Stream stream = path as Stream;
			if(stream != null)
				serializer.DeserializeObject(owner, stream, GetTypeName());
			else
				serializer.DeserializeObject(owner, path.ToString(), GetTypeName());
			EndStoreRestore(false);
		}
		protected virtual void StartStoreRestore(bool isStoring) { }
		protected virtual void EndStoreRestore(bool isStoring) { }
		void IXtraSerializable.OnEndSerializing() { }
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			CheckItems();
			ArrayList gauges = new ArrayList(owner.Gauges);
			foreach(IGauge g in gauges) {
				g.Clear();
				if(owner.DesignMode || owner.ForceClearOnRestore) g.Dispose();
			}
			owner.Gauges.ShallowClear();
			if(owner.DesignMode || owner.ForceClearOnRestore) Items.Clear();
		}
		protected virtual void EndRestoreGauges() {
			foreach(ISerizalizeableElement element in Items) {
				if(element.ParentCollectionName == "Gauges") {
					IGauge gb = (IGauge)element;
					gb.ForceUpdateChildren();
					gb.EndUpdate();
				}
			}
		}
		protected virtual void BeginRestoreGauges() {
			foreach(ISerizalizeableElement element in Items) {
				if(element.ParentCollectionName == "Gauges") {
					IGauge gb = (IGauge)element;
					gb.BeginUpdate();
					owner.Gauges.Add(gb);
				}
			}
		}
		protected ISerizalizeableElement FindParentItems(ISerizalizeableElement element) {
			foreach(ISerizalizeableElement celement in Items) {
				if(element.ParentName == celement.Name) return celement;
			}
			return null;
		}
		protected void RestoreBoundElements() {
			foreach(ISerizalizeableElement element in Items) {
				if(element.BoundElementName != null) {
					ISerizalizeableElement bElement = FindBoundElement(element);
					if(bElement != null) {
						RestoreBoundElementsCore(bElement, element);
					}
					else throw new Exception(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgGaugeRestoreError));
				}
			}
		}
		protected ISerizalizeableElement FindBoundElement(ISerizalizeableElement element) {
			foreach(ISerizalizeableElement bElement in Items)
				if(bElement.Name == element.BoundElementName) return bElement;
			return null;
		}
		protected void RestoreBoundElementsCore(ISerizalizeableElement boundElement, ISerizalizeableElement element) {
			if(boundElement == null) return;
			bool restored = false;
			IScaleStateIndicator indicatorComponent = element as IScaleStateIndicator;
			if(indicatorComponent != null) {
				indicatorComponent.IndicatorScale = boundElement as IScale;
				restored = true;
			}
			IStateImageIndicator stateImageIndicatorComponent = element as IStateImageIndicator;
			if(stateImageIndicatorComponent != null) {
				stateImageIndicatorComponent.IndicatorScale = boundElement as IScale;
				restored = true;
			}
			if(!restored) {
				IArcScaleComponent arcScaleComponent = element as IArcScaleComponent;
				if(arcScaleComponent != null) {
					arcScaleComponent.ArcScale = boundElement as IArcScale;
					restored = true;
				}
			}
			if(!restored) {
				ILinearScaleComponent linearScaleComponent = element as ILinearScaleComponent;
				if(linearScaleComponent != null) {
					linearScaleComponent.LinearScale = boundElement as ILinearScale;
					restored = true;
				}
			}
		}
		protected void AddToParentCollection(ISerizalizeableElement element, ISerizalizeableElement parent) {
			owner.AddToParentCollection(element, parent);
		}
		protected virtual void RestoreGaugeChildCollections() {
			foreach(ISerizalizeableElement element in Items) {
				if(element.ParentCollectionName != "Gauges") {
					ISerizalizeableElement parent = FindParentItems(element);
					if(parent != null) {
						AddToParentCollection(element, parent);
					}
					else throw new Exception(String.Format(GaugesCoreLocalizer.GetString(GaugesCoreStringId.MsgGaugeRestoreError), element.Name));
				}
			}
		}
		void IXtraSerializable.OnEndDeserializing(string layoutVersion) {
			BeginRestoreGauges();
			RestoreGaugeChildCollections();
			RestoreBoundElements();
			EndRestoreGauges();
		}
		void IXtraSerializable.OnStartSerializing() {
			if(isStyleSaving > 0) return;
			itemsCore = null;
		}
		List<ISerizalizeableElement> itemsCore = null;
		[XtraSerializableProperty(false, true, false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public List<ISerizalizeableElement> Items {
			get {
				if(itemsCore == null) CheckItems();
				return itemsCore;
			}
			set { itemsCore = value; }
		}
		protected void CheckItems() {
			FillItemsInternal(null, "");
			CheckForSameNames();
		}
		protected void UpdateISerizalizeableElementFields(ISerizalizeableElement element, string parentName) {
			IComponent component = element as IComponent;
			if(string.IsNullOrEmpty(element.Name)) {
				if(component.Site != null) element.Name = component.Site.Name;
			}
			element.ParentName = parentName;
			IScaleComponent isc = element as IScaleComponent;
			if(isc != null && !isc.Scale.IsEmpty) element.BoundElementName = ((ISerizalizeableElement)isc.Scale).Name;
		}
		protected virtual void FillItemsInternal(ISerizalizeableElement element, string parentName) {
			if(element == null) {
				itemsCore = new List<ISerizalizeableElement>();
				foreach(ISerizalizeableElement gauge in owner.Gauges) {
					FillItemsInternal(gauge, "");
				}
			}
			else {
				itemsCore.Add(element);
				UpdateISerizalizeableElementFields(element, parentName);
				List<ISerizalizeableElement> children = element.GetChildren();
				if(children != null) {
					foreach(ISerizalizeableElement child in children) {
						FillItemsInternal(child, element.Name);
					}
				}
			}
		}
		protected virtual void CheckForSameNames() {
		}
		protected virtual ISerizalizeableElement CreateSerializableInstance(XtraPropertyInfo info, XtraPropertyInfo infoType) {
			return owner.CreateSerializableInstance(info, infoType);
		}
		protected virtual ISerizalizeableElement FindSerializableByName(XtraPropertyInfo infoName) {
			foreach(ISerizalizeableElement element in Items)
				if(element.Name == (string)infoName.Value) return element;
			return null;
		}
		public object XtraFindItemsItem(XtraItemEventArgs e) {
			XtraPropertyInfo info = e.Item.ChildProperties["Name"];
			XtraPropertyInfo infoType = e.Item.ChildProperties["TypeNameEx"];
			if(infoType == null) {
				throw new NotImplementedException();
			}
			ISerizalizeableElement result = null;
			result = FindSerializableByName(info);
			if(result == null) {
				result = CreateSerializableInstance(info, infoType);
				itemsCore.Add(result);
			}
			if(result == null) throw new NotImplementedException();
			return result;
		}
		public void SaveStyle(object path) {
			Style controlStyle = new Style(owner.GetType());
			bool IsWinGaugeContainer = typeof(Control).IsAssignableFrom(owner.GetType());
			InitControlStyle(controlStyle, IsWinGaugeContainer);
			StoreStylesCore(path, CollectStoreStylesInfo(controlStyle));
		}
		public void RestoreStyle(object path) {
			Style controlStyle = new Style(owner.GetType());
			XtraObjectInfo[] objects = CollectRestoreStylesInfo(controlStyle);
			RestoreStylesCore(path, objects);
			controlStyle.Apply(owner);
			for(int i = 0; i < objects.Length-1; i++) {
				IGauge gauge = owner.Gauges[objects[i].Name];
				((StyleCollection)objects[i++].Instance).Apply(gauge);
			}
		}
		void StoreStylesCore(object path, XtraObjectInfo[] objects) {
			Stream stream = path as Stream;
			if(stream != null)
				new BinaryXtraSerializer().SerializeObjects(objects, stream, GetTypeName());
			else
				new XmlXtraSerializer().SerializeObjects(objects, (string)path, GetTypeName());
		}
		void RestoreStylesCore(object path, XtraObjectInfo[] objects) {
			Stream stream = path as Stream;
			if(stream != null)
				new BinaryXtraSerializer().DeserializeObjects(objects, stream, GetTypeName());
			else
				new XmlXtraSerializer().DeserializeObjects(objects, (string)path, GetTypeName());
		}
		int isStyleSaving = 0;
		XtraObjectInfo[] CollectStoreStylesInfo(Style controlStyle) {
			XtraObjectInfo[] objects = new XtraObjectInfo[owner.Gauges.Count + 1];
			StyleReader reader = new StyleReader(GaugesStyleMapService.Resolve());
			int i = 0;
			foreach(IGauge gauge in owner.Gauges) {
				SerializeHelper helper = new SerializeHelper(gauge);
				StyleCollectionKey key = new StyleCollectionKey(GetScope(gauge));
				StyleCollection gaugeStyles = new StyleCollection(key);
				isStyleSaving++;
				itemsCore = new List<ISerizalizeableElement>();
				try {
					FillItemsInternal(gauge as ISerizalizeableElement, string.Empty);
					XtraPropertyCollection store = new XtraPropertyCollection();
					store.AddRange(helper.SerializeObject(this, OptionsLayoutBase.FullLayout));
					reader.Read(store, gaugeStyles);
					objects[i++] = new XtraObjectInfo(gauge.Name, gaugeStyles);
				}
				finally { isStyleSaving--; }
			}
			objects[i++] = new XtraObjectInfo(owner.Name, controlStyle);
			return objects;
		}
		string GetScope(IGauge gauge) {
			string scope = string.Empty;
			switch(gauge.GetType().Name) {
				case "DigitalGauge": scope = "Digital"; break;
				case "CircularGauge": scope = "Circular"; break;
				case "LinearGauge": scope = "Linear"; break;
			}
			return scope;
		}
		XtraObjectInfo[] CollectRestoreStylesInfo(Style controlStyle) {
			XtraObjectInfo[] objects = new XtraObjectInfo[owner.Gauges.Count + 1];
			int i = 0;
			foreach(IGauge gauge in owner.Gauges)
				objects[i++] = new XtraObjectInfo(gauge.Name, new StyleCollection());
			objects[i++] = new XtraObjectInfo(owner.Name, controlStyle);
			return objects;
		}
		void InitControlStyle(Style controlStyle, bool isWinGaugeContainer) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(owner);
			ReadControlStyleProperty(controlStyle, properties, "BackColor");
			ReadControlStyleProperty(controlStyle, properties, "AutoLayout");
			ReadControlStyleProperty(controlStyle, properties, "LayoutPadding");
			ReadControlStyleProperty(controlStyle, properties, "LayoutInterval");
			if(isWinGaugeContainer) {
				ReadControlStyleProperty(controlStyle, properties, "BackgroundImage");
				ReadControlStyleProperty(controlStyle, properties, "BackgroundImageLayout");
			}
		}
		void ReadControlStyleProperty(Style controlStyle, PropertyDescriptorCollection properties, string property) {
			PropertyDescriptor descriptor = properties[property];
			if(descriptor != null)
				controlStyle.Setters.Add(property, descriptor.GetValue(owner));
		}
	}
}

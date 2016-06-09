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
using System.ComponentModel;
namespace DevExpress.Utils.VisualEffects {
	[ToolboxItem(false), DesignTimeVisible(false)]
	public abstract class AdornerElement : Component, IAdornerElement {
		bool visibleCore, isDisposing;
		object targetElementCore;
		static readonly object updatedCore = new object();
		static readonly object targetChanged = new object();
		AdornerElementViewInfo viewInfoCore;
		AdornerElementPainter painterCore;
		Base.IBaseDefaultProperties propertiesCore;
		AppearanceObject appearanceCore, parentAppearanceCore;
		public AdornerElement() {
			visibleCore = true;
			propertiesCore = CreateProperties();
			appearanceCore = CreateAppearance();
			Subscribe();
		}
		#region IAdornerElement Members
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AdornerElementTargetElement"),
#endif
 DefaultValue(null), Category("Behavior")]
		[TypeConverter(typeof(TargetElementTypeConverter))]
		public object TargetElement {
			get { return targetElementCore; }
			set {
				if(targetElementCore == value) return;
				if(!CheckTargetValue(value)) return;
				UnregisterTargetElement(targetElementCore);
				targetElementCore = value;
				RegisterTargetElement(value);
				OnTargetElementChanged();
			}
		}
		protected virtual bool CheckTargetValue(object value) {
			if(value == null) return true;
			return value is Control || value is ISupportAdornerElement || value is ISupportAdornerElementBarItem;
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AdornerElementVisible"),
#endif
 DefaultValue(true), Category("Behavior")]
		public bool Visible {
			get { return visibleCore; }
			set {
				if(visibleCore == value) return;
				visibleCore = value;
				OnVisibleChanged();
			}
		}
		[ Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Appearance { get { return appearanceCore; } }
		bool ShouldSerializeAppearance() {
			return !IsDisposing && Appearance.ShouldSerialize();
		}
		void ResetAppearance() {
			Appearance.Reset();
		}
		[Browsable(false)]
		public bool IsDisposing { get { return isDisposing; } }
		protected sealed override void Dispose(bool disposing) {
			if(disposing && !isDisposing) {
				isDisposing = disposing;
				OnDispose();
			}
			base.Dispose(disposing);
		}
		public void Assign(IAdornerElement element) {
			if(element == null || IsDisposing) return;
			AssignCore(element);
		}
		public object Clone() { return CloneCore(); }
		#endregion
		protected void UpdatePainter() {
			DevExpress.LookAndFeel.ISupportLookAndFeel lookAndFeel = FindLookAndFeel(TargetElement);
			DevExpress.Skins.ISkinProvider provider = null;
			if(lookAndFeel != null && lookAndFeel.LookAndFeel.ActiveStyle == LookAndFeel.ActiveLookAndFeelStyle.Skin)
				provider = lookAndFeel.LookAndFeel.ActiveLookAndFeel;
			painterCore = CreatePainter(provider);
		}
		protected virtual void Subscribe() {
			propertiesCore.Changed += OnPropertyChanged;
			appearanceCore.Changed += OnAppearanceChanged;
		}
		protected virtual void Unsubscribe() {
			propertiesCore.Changed -= OnPropertyChanged;
			appearanceCore.Changed -= OnAppearanceChanged;
		}
		protected virtual void OnTargetElementChanged() { RaiseTargetChanged(); }
		protected virtual void Invalidate() {
			if(IsDisposing) return;
			RaiseUpdated();
		}
		protected internal virtual void Update() {
			if(IsDisposing) return;
			if(viewInfoCore != null)
				viewInfoCore.SetDirty();
			RaiseUpdated();
		}
		protected internal virtual void UpdateStyle() {
			if(IsDisposing) return;
			if(painterCore != null)
				painterCore.ResetDefaultAppearance();
			Update();
		}
		protected virtual void OnDispose() {
			UnregisterTargetElement(TargetElement);
			targetElementCore = null;
			Unsubscribe();
			parentAppearanceCore = null;
			Ref.Dispose(ref propertiesCore);
			Ref.Dispose(ref appearanceCore);
		}
		protected void RaiseTargetChanged() {
			EventHandler target = Events[targetChanged] as EventHandler;
			if(target == null) return;
			target(this, EventArgs.Empty);
		}
		protected void RaiseUpdated() {
			EventHandler updated = Events[updatedCore] as EventHandler;
			if(updated == null) return;
			updated(this, EventArgs.Empty);
		}
		protected virtual AppearanceObject CreateAppearance() { return new AppearanceObject(); }
		protected virtual int NCHitTestCore { get { return DevExpress.Utils.Drawing.Helpers.NativeMethods.HT.HTTRANSPARENT; } }
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) { Update(); }
		protected virtual void OnPropertyChanged(object sender, EventArgs e) { Update(); }
		protected virtual void OnVisibleChanged() { Update(); }
		protected virtual bool HitTestCore(System.Drawing.Point p) { return viewInfoCore.Bounds.Contains(p); }
		protected virtual void AssignCore(IAdornerElement element) {
			appearanceCore.Assign(element.Appearance);
			propertiesCore.Assign(element.Properties);
			this.visibleCore = element.Visible;
		}
		protected abstract IAdornerElement CloneCore();
		protected virtual bool CheckVisible() {
			if(TargetElement == null) return false;
			bool ownerVisible = false;
			if(TargetElement is Control)
				ownerVisible = ((Control)TargetElement).Visible;
			if(TargetElement is ISupportAdornerElement)
				ownerVisible = ((ISupportAdornerElement)TargetElement).IsVisible;
			if(TargetElement is ISupportAdornerElementBarItem)
				ownerVisible = true;
			return ownerVisible && Visible;
		}
		internal abstract AdornerElementViewInfo CreateViewInfo();
		internal abstract AdornerElementPainter CreatePainter(DevExpress.Skins.ISkinProvider provider);
		protected abstract Base.IBaseDefaultProperties CreateProperties();
		void IAdornerElement.Update() { Update(); }
		void IAdornerElement.Invalidate() { Invalidate(); }
		event EventHandler IAdornerElement.Updated {
			add { Events.AddHandler(updatedCore, value); }
			remove { Events.RemoveHandler(updatedCore, value); }
		}
		event EventHandler IAdornerElement.TargetChanged {
			add { Events.AddHandler(targetChanged, value); }
			remove { Events.RemoveHandler(targetChanged, value); }
		}
		bool IAdornerElement.HitTest(System.Drawing.Point p) {
			if(viewInfoCore == null) return false;
			if(!Visible || TargetElement == null) return false;
			return HitTestCore(p);
		}
		AppearanceObject IAdornerElement.ParentAppearance { get { return parentAppearanceCore; } }
		int IAdornerElement.NCHitTest { get { return NCHitTestCore; } }
		Base.IBaseDefaultProperties IAdornerElement.Properties { get { return propertiesCore; } }
		IAdornerElementViewInfo IAdornerElement.ViewInfo { get { return viewInfoCore; } }
		AdornerElementPainter IAdornerElement.Painter { get { return painterCore; } }
		bool IAdornerElement.IsVisible { get { return CheckVisible(); } }
		void IAdornerElement.EnsureParentProperties(DevExpress.Utils.Base.IBaseProperties parentProperties) {
			if(propertiesCore != null)
				propertiesCore.EnsureParentProperties(parentProperties);
		}
		void IAdornerElement.EnsureParentAppearance(AppearanceObject parentAppearance) { parentAppearanceCore = parentAppearance; }
		DevExpress.LookAndFeel.ISupportLookAndFeel FindLookAndFeel(object source) {
			if(source == null) return null;
			DevExpress.LookAndFeel.ISupportLookAndFeel provider = source as DevExpress.LookAndFeel.ISupportLookAndFeel;
			if(provider == null) {
				Control control = source as Control;
				if(control != null)
					return FindLookAndFeel(control.Parent);
			}
			return provider;
		}
		void UnregisterTargetElement(object oldValue) {
			if(oldValue == null) return;
			Ref.Dispose(ref viewInfoCore);
			painterCore = null;
		}
		void RegisterTargetElement(object newValue) {
			if(newValue == null) return;
			viewInfoCore = CreateViewInfo();
			UpdatePainter();
		}
	}
	public class TargetElementTypeConverter : TypeConverter {
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			System.ComponentModel.Design.IDesignerHost host = context.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) as System.ComponentModel.Design.IDesignerHost;
			return GetStandardValues(host);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) { return true; }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) { return true; }
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			if(value == null) return null;
			if(context != null && value is string) {
				string source = value.ToString();
				if(source == "None") return null;
				System.ComponentModel.Design.IDesignerHost host = context.GetService(typeof(System.ComponentModel.Design.IDesignerHost)) as System.ComponentModel.Design.IDesignerHost;
				if(host == null || host.Container == null) return null;
				foreach(IComponent c in host.Container.Components) {
					if(c.Site != null && c.Site.Name == source)
						return c;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType.Equals(typeof(string))) {
				if(value == null) return "None";
				IComponent c = value as IComponent;
				if(c != null && c.Site != null) return c.Site.Name;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.Design.IDesignerHost host) {
			if(host == null || host.Container == null) return null;
			System.Collections.Generic.List<IComponent> components = new System.Collections.Generic.List<IComponent>();
			foreach(IComponent component in host.Container.Components) {
				if(CheckComponent(component))
					components.Add(component);
			}
			components.Sort(
				(x, y) =>
				{
					if(x == null || y == null) return 0;
					if(x.Site == null) return -1;
					if(y.Site == null) return 1;
					return x.Site.Name.CompareTo(y.Site.Name);
				}
				);
			return new StandardValuesCollection(components);
		}
		bool CheckComponent(IComponent component) { return component is Control || component is ISupportAdornerElement || component is ISupportAdornerElementBarItem; }
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
	}
}

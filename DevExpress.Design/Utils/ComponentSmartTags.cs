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
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design.Behavior;
using System.Windows.Forms.Design;
namespace DevExpress.Utils.Design {
	public interface ISmartTagProvider : IDisposable {
		SmartTagInfo UpdateGlyphs(GlyphCollection glyphs);
	}
	public interface IComponentSmartTagInfoParser {
		bool HasSmartTag(IComponent component);
		SmartTagInfo GetInfo(IComponent component, IServiceProvider serviceProvider);
	}
	public class WinSmartTagPropertyInfo {
		PropertyInfo propertyInfo;
		public WinSmartTagPropertyInfo(PropertyInfo propertyInfo) {
			this.propertyInfo = propertyInfo;
		}
		public PropertyInfo PropertyInfo { get { return propertyInfo; } }
	}
	public class SmartTagInfo {	   
		public SmartTagInfo(SmartTagSupportAttribute.SmartTagCreationMode creationMode) {
			this.CreationMode = creationMode;
			this.Bounds = Rectangle.Empty;
			this.TargetComponent = null;
			this.OwnerControl = null;
			this.GlyphBounds = Rectangle.Empty;
			this.ActionLists = null;
		}
		public void Assign(SmartTagInfo info) {
			this.Bounds = info.Bounds;
			this.TargetComponent = info.TargetComponent;
			this.OwnerControl = info.OwnerControl;
			this.GlyphBounds = info.GlyphBounds;
			this.ActionLists = info.ActionLists;
		}
		public Rectangle Bounds { get; set; }
		public Rectangle GlyphBounds { get; set; }
		public IComponent TargetComponent { get; set; }
		public Control OwnerControl { get; set; }
		public DesignerActionListCollection ActionLists { get; set; }
		public SmartTagSupportAttribute.SmartTagCreationMode CreationMode { get; set; }
	}
	#region Exceptions
	public class InvalidBoundsProviderTypeException : Exception {
		public InvalidBoundsProviderTypeException(string msg)
			: base(msg) {
		}
	}
	#endregion
	public class ControlSmartTagProviderBase : SmartTagProviderBase {
		public ControlSmartTagProviderBase(IServiceProvider serviceProvider) :base(serviceProvider) {   }						
		protected override bool CheckInfo(SmartTagInfo info, IComponent component) {
			Control owner = info.OwnerControl;
			IComponent currentObject = (ServiceProvider as ISite).Component;
			if(owner == null || (currentObject != owner && GetRelatedControl(component) != currentObject)) return false;
			ISmartDesignerActionListOwner listOwner = owner as ISmartDesignerActionListOwner;
			if(listOwner != null && !listOwner.AllowSmartTag(component)) {
				return false;	
			}
			return true;
		}
		protected ISmartTagClientBoundsProviderEx GetBoundsProvider(SmartTagSupportAttribute attribute) {
			ISmartTagClientBoundsProviderEx boundsProvider = Activator.CreateInstance(attribute.BoundsProviderType) as ISmartTagClientBoundsProviderEx;		
			return boundsProvider;
		}
		protected SmartTagSupportAttribute GetSmartTagSupportAttribute(IComponent component) {
			return Attribute.GetCustomAttribute(component.GetType(), typeof(SmartTagSupportAttribute)) as SmartTagSupportAttribute;
		}
		Control GetRelatedControl(IComponent component) {
			Control relatedControl;
			SmartTagSupportAttribute attribute = GetSmartTagSupportAttribute(component);
			ISmartTagClientBoundsProviderEx boundsProvider = GetBoundsProvider(attribute);
			if(boundsProvider == null) return null;
			boundsProvider.GetObserver(component, out relatedControl);
			return relatedControl;
		}
		protected override void CreateGlyph(GlyphCollection glyphs, SmartTagInfo info) {
			ControlDesignerActionListGlyphHelper.CreateGlyph(glyphs, info);
		}
	}
	public class SmartTagProviderBase : ISmartTagProvider { 
		IServiceProvider serviceProvider;
		IComponentSmartTagInfoParser smartTagInfoParser;
		ISelectionService selectionServiceCore = null;
		public SmartTagProviderBase(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			this.smartTagInfoParser = CreateSmartTagInfoParser();
		}
		#region IComponentSmartTagProvider
		SmartTagInfo ISmartTagProvider.UpdateGlyphs(GlyphCollection glyphs) {
			return GetGlyphsCore(glyphs);
		}
		#endregion
		protected virtual IComponentSmartTagInfoParser CreateSmartTagInfoParser() {
			return new ComponentSmartTagInfoParserBase();
		}		
		protected ISelectionService SelectionService {
			get {
				if(selectionServiceCore == null && serviceProvider != null) {
					selectionServiceCore = ServiceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
				}
				return selectionServiceCore;
			}
		}
		protected virtual SmartTagInfo GetGlyphsCore(GlyphCollection glyphs) {
			if(SelectionService == null)
				return null;
			IComponent target = GetTargetComponent();			
			if(!CheckComponent(target))
				return null;
			var info = SmartTagInfoParser.GetInfo(target, ServiceProvider);
			if(!CheckInfo(info, target))
				return null;
			CreateGlyph(glyphs, info);
			return info;
		}
		protected virtual void CreateGlyph(GlyphCollection glyphs, SmartTagInfo info) { }
		protected virtual bool CheckInfo(SmartTagInfo info, IComponent component) {
			ISmartDesignerActionListOwner componentOwner = component as ISmartDesignerActionListOwner;
			if(componentOwner != null && !componentOwner.AllowSmartTag(component)) {
				return false;
			}
			return true;
		}
		protected virtual IComponent GetTargetComponent() {
			if(SelectionService == null) return null;
			return SelectionService.PrimarySelection as IComponent;
		}
		protected virtual bool CheckComponent(IComponent component){
			return !(component == null || !SmartTagInfoParser.HasSmartTag(component));
		}
		public IServiceProvider ServiceProvider { 
			get { return serviceProvider; } 
		}
		public IComponentSmartTagInfoParser SmartTagInfoParser { 
			get { return smartTagInfoParser; } 
		}
		#region IDisposable Members
		bool isDisposing;
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				Dispose();
			}
			GC.SuppressFinalize(this);
		}		
		#endregion
		protected virtual void Dispose(){
			serviceProvider = null;
			selectionServiceCore = null;
			smartTagInfoParser = null;
			BaseDesignerActionListGlyphHelper.ResetCache();
		}
	}
	public class SelectionChangedEventArgs {
		SelectionState selectionStateCore;
		public SelectionState SelectionState { get { return selectionStateCore; } }
		public SelectionChangedEventArgs(SelectionState state) {
			selectionStateCore = state;
		}
	}
	public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs args);	
	public interface IComponentSmartTagProvider : ISmartTagProvider {
		void RemoveGlyph();
		void RefreshGlyph(Rectangle bounds);
		event SelectionChangedEventHandler SelectionChanged;
	}
	public enum SelectionState { Select, UnSelect }
	public class ComponentSmartTagProviderBase : SmartTagProviderBase, IComponentSmartTagProvider {
		IComponentSmartTagController infoCore;	   
		SelectionState selectionState;
		public ComponentSmartTagProviderBase(IServiceProvider serviceProvider) : base(serviceProvider) {
			if(SelectionService != null) {
				SelectionService.SelectionChanged += ServiceSelectionChanged;
				SelectionService.SelectionChanging += ServiceSelectionChanging;
			}
			selectionState = SelectionState.UnSelect;
		}		 
		void ServiceSelectionChanging(object sender, EventArgs e) {
			IComponent selectComponent = GetTargetComponent();
			if(selectComponent == null || SelectionChanged == null) return;
			if(selectionState == SelectionState.Select && selectComponent.Site != ServiceProvider) {
				selectionState = SelectionState.UnSelect;
				SelectionChanged(sender, new SelectionChangedEventArgs(selectionState));
			}
		}
		void ServiceSelectionChanged(object sender, EventArgs e) {
			IComponent selectComponent = GetTargetComponent();
			if(selectComponent == null || SelectionChanged == null) return;
			if(selectionState == SelectionState.UnSelect && selectComponent.Site == ServiceProvider) {
				selectionState = SelectionState.Select;
				SelectionChanged(sender, new SelectionChangedEventArgs(selectionState));
			}
		}		
		protected override void CreateGlyph(GlyphCollection glyphs, SmartTagInfo info) {			
			infoCore = new ComponentSmartTagController(new ComponentSmartTagInfo(ServiceProvider, info));
			infoCore.CreateGlyph();
		}
		void IComponentSmartTagProvider.RemoveGlyph() { 
			 if(infoCore != null)
				infoCore.RemoveGlyph();
		}
		public event SelectionChangedEventHandler SelectionChanged;
		void IComponentSmartTagProvider.RefreshGlyph(Rectangle bounds) {
			IComponent selectComponent = GetTargetComponent();
			if(selectComponent == null) return;
			if(infoCore != null && selectComponent.Site == ServiceProvider)
				infoCore.RefreshGlyph(bounds);
		}
		protected override void Dispose() {
			if(SelectionService != null) {
				SelectionService.SelectionChanged -= ServiceSelectionChanged;
				SelectionService.SelectionChanging -= ServiceSelectionChanging;
			}
			if(infoCore != null) {
				infoCore.RemoveGlyph();
				infoCore = null;
			}
			base.Dispose();
		}
	}
	public interface IComponentSmartTagInfo {
		IComponent TargetComponent { get; }
		Rectangle Bounds { get; set; }
		BehaviorService BehaviorService { get; }
		DesignerActionListCollection ActionLists { get; }
		Adorner Adorner { get; }
	}
	public interface IComponentSmartTagController {
		void CreateGlyph();
		void RemoveGlyph();
		void RefreshGlyph(Rectangle bounds);
	}
	public class ComponentSmartTagInfo : SmartTagInfo, IComponentSmartTagInfo {
		Adorner adorner;
		BehaviorService behaviorService;
		public ComponentSmartTagInfo(IServiceProvider provider, SmartTagInfo info) : base(info.CreationMode) {
			this.Assign(info);
			adorner = new Adorner();
			behaviorService = provider.GetService(typeof(BehaviorService)) as BehaviorService;			
		}
		public BehaviorService BehaviorService {
			get { return behaviorService; }
		}
		public Adorner Adorner {
			get { return adorner; }
		}
	}
	class ComponentSmartTagController : IComponentSmartTagController {
		IComponentSmartTagInfo infoCore;
		public ComponentSmartTagController(IComponentSmartTagInfo info) {
			infoCore = info;
		}
		void HostClosing(object sender, ToolStripDropDownClosingEventArgs e) {
			ComponentDesignerActionListGlyphHelper.CloseDesignerActionHost(infoCore.TargetComponent);
		}
		void SubscribeStandart(ToolStripDropDown designerActionHost) {
			Unsubscribe(designerActionHost);
			ComponentDesignerActionListGlyphHelper.SubscribeHostClosing(infoCore);
		}
		void Unsubscribe(ToolStripDropDown designerActionHost) {
			ComponentDesignerActionListGlyphHelper.UnsubscribeHostClosing(infoCore);
			designerActionHost.LocationChanged -= HostLocationChanged;
			designerActionHost.Closing -= HostClosing;
		}
		void Subscribe(ToolStripDropDown designerActionHost) {
			Unsubscribe(designerActionHost);
			designerActionHost.LocationChanged += HostLocationChanged;
			designerActionHost.Closing += HostClosing;			
		}
		void IComponentSmartTagController.RefreshGlyph(Rectangle bounds) {
			if(!bounds.IsEmpty) {
				infoCore.Bounds = bounds;
				infoCore.Adorner.Glyphs.Clear();
				if(infoCore.BehaviorService.Adorners.Contains(infoCore.Adorner))
					infoCore.BehaviorService.Adorners.Remove(infoCore.Adorner);
				ToolStripDropDown designerActionHost = ComponentDesignerActionListGlyphHelper.GetDesignerActionHost(infoCore);
				if(designerActionHost != null) {
					Subscribe(designerActionHost);
					if(designerActionHost.IsHandleCreated) {
						var glyph = ComponentDesignerActionListGlyphHelper.RefreshDesignerGlyphWrapper(infoCore);
						designerActionHost.BeginInvoke(new Action<ToolStripDropDown, Glyph>(ShowSmartTag),
							designerActionHost, glyph);
					}
				}
			}
		}
		void ShowSmartTag(ToolStripDropDown designerActionHost, Glyph glyph) {
			if(glyph != null)
				infoCore.Adorner.Glyphs.Add(glyph);
			if(infoCore.Adorner != null)
				infoCore.BehaviorService.Adorners.Add(infoCore.Adorner);
		}
		void HostLocationChanged(object sender, EventArgs e) {
			ToolStripDropDown designerActionHost = sender as ToolStripDropDown;
			SmartTagCacheElement element = SmartTagInfoCache.Default.GetInfo(infoCore.TargetComponent);
			if(designerActionHost != null && element != null) {
				Point loc = new Point(element.Glyph.Bounds.Right, element.Glyph.Bounds.Y);
				designerActionHost.Bounds = new Rectangle(infoCore.BehaviorService.AdornerWindowPointToScreen(loc), designerActionHost.Size);
			}
		}
		void IComponentSmartTagController.RemoveGlyph() {
			if(infoCore.BehaviorService.Adorners.Contains(infoCore.Adorner)) {
				ToolStripDropDown designerActionHost = ComponentDesignerActionListGlyphHelper.GetDesignerActionHost(infoCore);
				SubscribeStandart(designerActionHost);
				infoCore.BehaviorService.Adorners.Remove(infoCore.Adorner);
				infoCore.Adorner.Glyphs.Clear();
			}
		}
		void CreateGlyph(){
			if(!infoCore.BehaviorService.Adorners.Contains(infoCore.Adorner)) {
				infoCore.BehaviorService.Adorners.Add(infoCore.Adorner);
			}
			Glyph glyph = ComponentDesignerActionListGlyphHelper.CreateDesignerGlyphWrapper(infoCore);
			if(glyph != null) {
				ToolStripDropDown designerActionHost = ComponentDesignerActionListGlyphHelper.GetDesignerActionHost(infoCore);
				Subscribe(designerActionHost);
				ShowSmartTag(designerActionHost, glyph);						  
			}
		}
		void IComponentSmartTagController.CreateGlyph() { CreateGlyph(); }
	}
	public class ComponentSmartTagInfoParserBase : IComponentSmartTagInfoParser {
		public ComponentSmartTagInfoParserBase() {
		}
		#region IComponentSmartTagInfoParser
		bool IComponentSmartTagInfoParser.HasSmartTag(IComponent component) {
			return HasComponentSmartTagCore(component);
		}
		SmartTagInfo IComponentSmartTagInfoParser.GetInfo(IComponent component, IServiceProvider serviceProvider) {
			return GetInfoCore(component, serviceProvider);
		}
		#endregion
		protected virtual bool HasComponentSmartTagCore(IComponent component) {
			return GetSmartTagSupportAttribute(component) != null;
		}
		protected SmartTagSupportAttribute GetSmartTagSupportAttribute(IComponent component) {
			return Attribute.GetCustomAttribute(component.GetType(), typeof(SmartTagSupportAttribute)) as SmartTagSupportAttribute;
		}
		protected virtual SmartTagInfo GetInfoCore(IComponent component, IServiceProvider serviceProvider) {
			SmartTagSupportAttribute attribute = GetSmartTagSupportAttribute(component);
			ISmartTagClientBoundsProvider boundsProvider = GetBoundsProvider(attribute);
			SmartTagInfo res = new SmartTagInfo(attribute.CreationType);
			res.Bounds = boundsProvider != null ? boundsProvider.GetBounds(component) : Rectangle.Empty;
			res.TargetComponent = component;
			res.OwnerControl = boundsProvider.GetOwnerControl(component);
			res.ActionLists = GetActionListsCore(res, serviceProvider);
			return res;
		}
		protected ISmartTagClientBoundsProvider GetBoundsProvider(SmartTagSupportAttribute attribute) {
			ISmartTagClientBoundsProvider boundsProvider = Activator.CreateInstance(attribute.BoundsProviderType) as ISmartTagClientBoundsProvider;
			if(boundsProvider == null) {
				throw new InvalidBoundsProviderTypeException("Bounds Provider type must implement ISmartTagClientBoundsProvider interface");
			}
			return boundsProvider;
		}
		protected virtual DesignerActionListCollection GetActionListsCore(SmartTagInfo info, IServiceProvider serviceProvider) {
			DesignerActionListCollection lists = new DesignerActionListCollection();
			lists.Add(CreateActionListCore(info, serviceProvider));
			return lists;
		}
		protected virtual DesignerActionList CreateActionListCore(SmartTagInfo info, IServiceProvider serviceProvider) {
			IDesignerHost host = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return null;
			IComponent component = info.TargetComponent;
			ComponentDesigner designer = host.GetDesigner(info.TargetComponent) as ComponentDesigner;
			return CreateDesignerActionListCore(designer, info.TargetComponent);
		}
		protected virtual DesignerActionList CreateDesignerActionListCore(ComponentDesigner designer, IComponent component) {
			return new SmartDesignerActionList(designer, component, true);
		}
	}
	public class ReflectionHelper {
		public static object GetValueField(object owner, string nameField, BindingFlags flags) {
			FieldInfo fieldInfo = GetFieldInfo(owner, nameField, flags);		  
			return GetValueField(owner, fieldInfo);
		}
		public static object GetValueField(object owner, FieldInfo fieldInfo) {			
			if(fieldInfo != null) return fieldInfo.GetValue(owner);
			return null;
		}
		public static void SetValueField(object owner, string nameField, BindingFlags flags, object value) {
			FieldInfo fieldInfo = GetFieldInfo(owner, nameField, flags);
			SetValueField(owner, fieldInfo, value);
		}
		public static void SetValueField(object owner, FieldInfo fieldInfo, object value) {
			if(owner == null || fieldInfo == null) return;
			fieldInfo.SetValue(owner, value);
		}
		public static FieldInfo GetFieldInfo(Type type, string nameField, BindingFlags flags) {
			if(type == null) return null;
			return type.GetField(nameField, flags);
		}
		public static FieldInfo GetFieldInfo(object owner, string nameField, BindingFlags flags) {
			if(owner == null) return null;
			return GetFieldInfo(owner.GetType(), nameField, flags);
		}
		public static MethodInfo GetMethodInfo(Type type, string nameMethod, BindingFlags flags) {
			if(type == null) return null;
			return type.GetMethod(nameMethod, flags);
		}
		public static MethodInfo GetMethodInfo(object owner, string nameMethod, BindingFlags flags) {
			if(owner == null) return null;
			return GetMethodInfo(owner.GetType(), nameMethod, flags);
		}
		public static object InvokeMethod(object owner, string nameMethod, BindingFlags flags, object[] args) {
			MethodInfo methodInfo = GetMethodInfo(owner, nameMethod, flags);
			return InvokeMethod(owner, methodInfo, args);
		}
		public static object InvokeMethod(object owner, MethodInfo methodInfo, object[] args) {			
			if(methodInfo == null || owner == null) return null;
			return methodInfo.Invoke(owner, args);
		}
		public static EventInfo GetEventInfo(Type type, string nameEvent, BindingFlags flags) {
			if(type == null) return null;
			return type.GetEvent(nameEvent, flags);
		}
		public static EventInfo GetEventInfo(object owner, string nameEvent, BindingFlags flags) {
			if(owner == null) return null;
			return GetEventInfo(owner.GetType(), nameEvent, flags);
		}
		public static void Subscribe(object owner, EventInfo eventInfo, Delegate action) {
			if(eventInfo == null || owner == null) return;
			eventInfo.AddEventHandler(owner, action);
		}
		public static void Unsubscribe(object owner, EventInfo eventInfo, Delegate action) {   
			if(eventInfo == null || owner == null) return;
			eventInfo.RemoveEventHandler(owner, action);
		}
		public static ConstructorInfo GetConstructorInfo(Type type, Type[] args){
			if(type == null) return null;
			return type.GetConstructor(args);
		}
		public static PropertyInfo GetPropertyInfo(Type type, string propertyName, BindingFlags flags) {
			if(type == null) return null;
			return type.GetProperty(propertyName, flags);
		}
		public static PropertyInfo GetPropertyInfo(object owner, string propertyName, BindingFlags flags) {
			if(owner == null) return null;
			return GetPropertyInfo(owner.GetType(), propertyName, flags);
		}
		public static object GetValueProperty(object owner, string propertyName, BindingFlags flags) {
			if(owner == null) return null;
			PropertyInfo propertyInfo = GetPropertyInfo(owner, propertyName, flags);
			return GetValueProperty(owner, propertyInfo);
		}
		public static object GetValueProperty(object owner, PropertyInfo propertyInfo) {
			if(owner == null || propertyInfo == null) return null;
			return propertyInfo.GetValue(owner, null);
		}
		public static void SetValueProperty(object owner, string propertyName, BindingFlags flags, object value) {
			if(owner == null) return;
			PropertyInfo propertyInfo = GetPropertyInfo(owner, propertyName, flags);
			SetValueProperty(owner, propertyInfo, value);
		}
		public static void SetValueProperty(object owner, PropertyInfo propertyInfo, object value) {
			if(owner == null || propertyInfo == null) return;
			propertyInfo.SetValue(owner, value, null);
		}
	}
	class ComponentDesignerActionListGlyphHelper : BaseDesignerActionListGlyphHelper {
		public static Glyph RefreshDesignerGlyphWrapper(IComponentSmartTagInfo info) {
			SmartTagCacheElement element = SmartTagInfoCache.Default.GetInfo(info.TargetComponent);
			if(element == null) return null;
			CalcBoundsGlyph(element.Glyph, info);
			return element.Glyph;
		}
		public static void CloseDesignerActionHost(IComponent targerComponent) {
			SmartTagCacheElement element = SmartTagInfoCache.Default.GetInfo(targerComponent);
			if(element == null) return;
			object actionUI = element.ActionUI;
			FieldInfo lastPanelComponentInfo = ReflectionHelper.GetFieldInfo(actionUI, "lastPanelComponent", BindingFlags.NonPublic | BindingFlags.Instance);
			System.Collections.Hashtable table = ReflectionHelper.GetValueField(actionUI, "componentToGlyph", BindingFlags.NonPublic | BindingFlags.Instance) as System.Collections.Hashtable;
			object lastPanelComponent = lastPanelComponentInfo.GetValue(actionUI);
			if(lastPanelComponent != null) {
				Glyph gl = table[lastPanelComponent] as Glyph;
				ReflectionHelper.SetValueField(gl.Behavior, "ignoreNextMouseUp", BindingFlags.NonPublic | BindingFlags.Instance, true);
				ReflectionHelper.InvokeMethod(gl, "InvalidateOwnerLocation", BindingFlags.NonPublic | BindingFlags.Instance, null);
				lastPanelComponentInfo.SetValue(actionUI, null);
			}
		}		
		static void CreateDesignerActionHost(ToolStripDropDown designerActionHost) {						
			designerActionHost.AutoSize = false;
			designerActionHost.Padding = Padding.Empty;
			designerActionHost.Renderer = new NoBorderRenderer();
			designerActionHost.Text = "DesignerActionTopLevelForm";			
		}
		public static ToolStripDropDown GetDesignerActionHost(IComponentSmartTagInfo info) {
			object actionUI = GetActionUI(info);
			if(actionUI == null) return null;
			return ReflectionHelper.GetValueField(actionUI, "designerActionHost", BindingFlags.NonPublic | BindingFlags.Instance) as ToolStripDropDown;
		}
		static object GetActionUI(IComponentSmartTagInfo info) {
			if(info == null) return null;
			SmartTagCacheElement element = SmartTagInfoCache.Default.GetInfo(info.TargetComponent);
			if(element != null) return element.ActionUI;
			if(info.BehaviorService == null) return null;
			return ReflectionHelper.GetValueProperty(info.BehaviorService, "DesignerActionUI", BindingFlags.NonPublic | BindingFlags.Instance);
		}
		internal static void SubscribeHostClosing(IComponentSmartTagInfo info) {
			Subscriber(info, ReflectionHelper.Subscribe);
		}
		static void Subscriber(IComponentSmartTagInfo info, Action<object, EventInfo, Delegate> action){
			 object actionUI = GetActionUI(info);
			if(actionUI == null) return;
			ToolStripDropDown designerActionHost = ReflectionHelper.GetValueField(actionUI, "designerActionHost", BindingFlags.NonPublic | BindingFlags.Instance) as ToolStripDropDown;
			if(designerActionHost != null) {
				EventInfo eventInfo = ReflectionHelper.GetEventInfo(designerActionHost, "Closing", BindingFlags.Public | BindingFlags.Instance);
				MethodInfo methodInfo = ReflectionHelper.GetMethodInfo(actionUI, "toolStripDropDown_Closing", BindingFlags.NonPublic | BindingFlags.Instance);
				action(designerActionHost, eventInfo, Delegate.CreateDelegate(eventInfo.EventHandlerType, actionUI, methodInfo));
			}
		}
		internal static void UnsubscribeHostClosing(IComponentSmartTagInfo info) {
		   Subscriber(info, ReflectionHelper.Unsubscribe);
		}
		static void CalcBoundsGlyph(Glyph gl, IComponentSmartTagInfo info) {
			Point location = info.BehaviorService.ScreenToAdornerWindow(new Point(info.Bounds.Right - 10, info.Bounds.Y));
			ReflectionHelper.SetValueField(gl, "bounds", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, new Rectangle(location, new Size(10, 10)));
		}
		public static Glyph CreateDesignerGlyphWrapper(IComponentSmartTagInfo info) {
			if(info == null || info.TargetComponent == null || info.BehaviorService == null || info.ActionLists == null) return null;				   
			try {
				object actionUI = GetActionUI(info);
				FieldInfo designerActionHostInfo = ReflectionHelper.GetFieldInfo(actionUI, "designerActionHost", BindingFlags.NonPublic | BindingFlags.Instance);
				ToolStripDropDown designerActionHost = designerActionHostInfo.GetValue(actionUI) as ToolStripDropDown;
				if(designerActionHost == null) {
					ConstructorInfo constructorInfo = ReflectionHelper.GetConstructorInfo(designerActionHostInfo.FieldType, new Type[] { actionUI.GetType(), typeof(IWin32Window) });
					object mainParentWindow = ReflectionHelper.GetValueField(actionUI, "mainParentWindow", BindingFlags.NonPublic | BindingFlags.Instance);
					designerActionHost = constructorInfo.Invoke(new object[] { actionUI, mainParentWindow }) as ToolStripDropDown;					
					CreateDesignerActionHost(designerActionHost);
					designerActionHostInfo.SetValue(actionUI, designerActionHost);					
				}		  
				Glyph gl = ReflectionHelper.InvokeMethod(actionUI, "GetDesignerActionGlyph", BindingFlags.NonPublic | BindingFlags.Instance, new object[] { info.TargetComponent, info.ActionLists }) as Glyph;
				if(gl != null) {
					CalcBoundsGlyph(gl, info);
					FieldInfo svp = ReflectionHelper.GetFieldInfo(gl.Behavior, "serviceProvider", BindingFlags.Instance | BindingFlags.NonPublic);
					GlyphServiceProvider provider = new GlyphServiceProvider((IServiceProvider)svp.GetValue(gl.Behavior));
					svp.SetValue(gl.Behavior, provider);
					SmartTagInfoCache.Default.Add(info.TargetComponent, new SmartTagCacheElement(gl, info.TargetComponent, null, actionUI, info.BehaviorService));
				}			  
				return gl;
			}
			catch { return null;  }			
		}
		class NoBorderRenderer : ToolStripProfessionalRenderer {
			[System.Runtime.TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			public NoBorderRenderer() { }
			protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) { }
		}
	}
	public class BaseDesignerActionListGlyphHelper {
		public static void RefreshSmartPanel(IComponent component) {
			RefreshSmartPanelBounds(component);
			RefreshSmartPanelContent(component);
		}
		public static void RefreshSmartPanelBounds(IComponent component) {
			if(!SmartTagInfoCache.Default.Check(component))
				return;
			var element = SmartTagInfoCache.Default.GetInfo(component);
			if(element.Owner != null) {
				element.Owner.Refresh();
				element.Owner.Update();
			}
			element.UpdateAdornerBounds();
			element.UpdateGlyphLocation();
			element.UpdatePopupPanelLocation();
		}
		public static bool HideSmartPanel(IComponent component) {
			if(!SmartTagInfoCache.Default.Check(component))
				return false;
			var element = SmartTagInfoCache.Default.GetInfo(component);
			return element.HidePopupPanel();
		}
		public static bool RefreshSmartPanelContent(IComponent component) {
			if(!SmartTagInfoCache.Default.Check(component))
				return false;
			var element = SmartTagInfoCache.Default.GetInfo(component);
			return element.UpdatePopupPanelContent();
		}
		public static void ResetCache() {
			SmartTagInfoCache.Reset();
		}
	}
	public class ControlDesignerActionListGlyphHelper : BaseDesignerActionListGlyphHelper {		
		public static void CreateGlyph(GlyphCollection glyphColl, SmartTagInfo info) {
			var lists = info.CreationMode == SmartTagSupportAttribute.SmartTagCreationMode.Auto ? info.ActionLists : null;
			info.GlyphBounds = DesignTimeGlyphHelper.CalcGlyphBounds(info.OwnerControl, info.Bounds);
			CreateDesignerGlyphWrapper(glyphColl, info.TargetComponent, info.OwnerControl, info.Bounds, lists);
		}
		public static void CreateDesignerGlyphWrapper(GlyphCollection glyphColl, IComponent targetComponent, Control ownerControl, Rectangle bounds) {
			CreateDesignerGlyphWrapper(glyphColl, targetComponent, ownerControl, bounds, null);
		}
		public static void CreateDesignerGlyphWrapper(GlyphCollection glyphColl, IComponent targetComponent, Control ownerControl, Rectangle bounds, DesignerActionListCollection lists) {
			ISite site = targetComponent.Site;
			if(site == null) site = ownerControl.Site;
			if(site == null) return;
			BehaviorService svc = (BehaviorService)site.GetService(typeof(BehaviorService));
			if(svc == null) {
				svc = (BehaviorService)ownerControl.Site.GetService(typeof(BehaviorService));
			}
			if(svc == null) return;
			DesignerActionListCollection actionLists = GetActionLists(site, targetComponent, lists);
			if(actionLists == null) return;
			Point basePoint = svc.ControlToAdornerWindow(ownerControl);
			bounds.Offset(basePoint);
			try {
				object actionUI = ReflectionHelper.GetValueProperty(svc, "DesignerActionUI", BindingFlags.NonPublic | BindingFlags.Instance);
				Glyph gl = ReflectionHelper.InvokeMethod(actionUI, "GetDesignerActionGlyph", BindingFlags.NonPublic | BindingFlags.Instance, new object[] { ownerControl, actionLists }) as Glyph; 
				if(gl != null) {
					ReflectionHelper.SetValueField(gl, "bounds", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, DesignTimeGlyphHelper.CalcGlyphBounds(bounds));
					FieldInfo svp = ReflectionHelper.GetFieldInfo(gl.Behavior, "serviceProvider", BindingFlags.Instance | BindingFlags.NonPublic);
					GlyphServiceProvider provider = new GlyphServiceProvider((IServiceProvider)svp.GetValue(gl.Behavior));
					svp.SetValue(gl.Behavior, provider);			  
					SmartTagInfoCache.Default.Add(targetComponent, new SmartTagCacheElement(gl, targetComponent, ownerControl, actionUI, svc));
					glyphColl.Add(gl);
				}
			}
			catch { }
		}
		static DesignerActionListCollection GetActionLists(ISite site, IComponent targetComponent, DesignerActionListCollection lists) {
			if(lists != null) return lists;
			IDesignerHost host = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return null;
			ComponentDesigner designer = host.GetDesigner(targetComponent) as ComponentDesigner;
			return designer != null ? designer.ActionLists : null;
		}
	}
	class GlyphServiceProvider : IServiceProvider {
		IServiceProvider source;
		public GlyphServiceProvider(IServiceProvider source) {
			this.source = source;
		}
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType.Equals(typeof(ISelectionService))) return null;
			return source.GetService(serviceType);
		}
	}
	public static class DesignTimeGlyphHelper {
		public static Rectangle CalcGlyphBounds(Control ownerControl, Rectangle componentBounds) {
			Rectangle res = CalcGlyphBounds(componentBounds);
			res.Offset(ownerControl.Location);
			return res;
		}
		public static Rectangle CalcGlyphBounds(Rectangle componentBounds) {
			return new Rectangle(componentBounds.Right - 10, componentBounds.Y, GlyphSize.Width, GlyphSize.Height);
		}
		static Size glyphSize = Size.Empty;
		public static Size GlyphSize {
			get {
				if (glyphSize == Size.Empty) glyphSize = GetGlyphSize();
				return glyphSize;
			}
		}
		static Size GetGlyphSize() {
			try {
				Type type = typeof(ControlDesigner).Assembly.GetType("System.Windows.Forms.DpiHelper");
				if (type != null) {
					PropertyInfo isScaling = type.GetProperty("IsScalingRequired", BindingFlags.Static | BindingFlags.Public);
					if ((bool)isScaling.GetValue(null, null)) {
						Bitmap bmp = new Bitmap(10, 10);
						object[] args = new object[] { bmp };
						type.GetMethod("ScaleBitmapLogicalToDevice", BindingFlags.Static | BindingFlags.Public).Invoke(null, args);
						bmp = (Bitmap)args[0];
						return bmp.Size;
					}
				}
			}
			catch {
			}
			return new Size(10, 10);
		}
	}
	public class SmartTagInfoCache {
		KeyValuePair<IComponent, SmartTagCacheElement> pairCore;
		protected SmartTagInfoCache() {
			pairCore = new KeyValuePair<IComponent, SmartTagCacheElement>(null, null);
		}
		public void Add(IComponent component, SmartTagCacheElement element) {
			pairCore = new KeyValuePair<IComponent, SmartTagCacheElement>(component, element);
		}
		public bool Check(IComponent component) {
			return object.ReferenceEquals(pairCore.Key, component);
		}
		public SmartTagCacheElement GetInfo(IComponent component) {
			if(!Check(component))
				return null;
			return pairCore.Value;
		}
		static SmartTagInfoCache defaultCore = null;
		public static SmartTagInfoCache Default {
			get {
				if(defaultCore == null) defaultCore = new SmartTagInfoCache();
				return defaultCore;
			}
		}
		public static bool IsCreated { get { return defaultCore != null; } }
		public static void Reset() {
			if(!IsCreated)
				return;
			if(Default.pairCore.Key != null && Default.pairCore.Value != null) {
				Default.pairCore.Value.Dispose();
			}
			defaultCore = null;
		}
	}
	public class SmartTagCacheElement : IDisposable {
		public SmartTagCacheElement(Glyph glyph, IComponent component, Control owner, object actionUI, BehaviorService svc) {
			this.Glyph = glyph;
			this.Owner = owner;
			this.Component = component;
			this.ActionUI = actionUI;
			this.BehaviorService = svc;
		}
		protected Rectangle CalcBounds() {
			ISmartTagClientBoundsProvider provider = GetBoundsProvider();
			return provider.GetBounds(Component);
		}
		public void UpdateGlyphLocation() {
			Rectangle bounds = CalcBounds();
			if(bounds == null || Owner == null) return;
			Point basePoint = BehaviorService.ControlToAdornerWindow(Owner);
			bounds.Offset(basePoint);
			bounds = DesignTimeGlyphHelper.CalcGlyphBounds(bounds);
			ReflectionHelper.SetValueField(Glyph, "bounds", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, bounds);
			BehaviorService.Invalidate();
			OnNotifyObserver();
		}
		public void UpdateAdornerBounds() {
			object obj = ReflectionHelper.GetValueField(ActionUI, "selSvc", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
			if(obj == null) return;
			ReflectionHelper.InvokeMethod(obj, "OnSelectionChanged", BindingFlags.NonPublic | BindingFlags.Instance, null);
		}
		public bool UpdatePopupPanelContent() {
			var mi = ActionUI.GetType().GetMethod("RecreatePanel", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(Glyph) }, null);
			if(mi == null) return false;
			mi.Invoke(ActionUI, new object[] { Glyph });
			return true;
		}
		public bool HidePopupPanel() {
			var mi = ActionUI.GetType().GetMethod("HideDesignerActionPanel", BindingFlags.Instance | BindingFlags.NonPublic);
			if(mi == null) return false;
			mi.Invoke(ActionUI, null);
			return true;
		}
		public void UpdatePopupPanelLocation() {
			ToolStripDropDown dd = ReflectionHelper.GetValueField(ActionUI, "designerActionHost", BindingFlags.NonPublic | BindingFlags.Instance) as ToolStripDropDown;
			if(dd == null || Owner == null) return;
			Rectangle bounds = Owner.RectangleToScreen(CalcBounds());
			dd.Location = new Point(bounds.Right, bounds.Top);
		}
		protected ISmartTagClientBoundsProvider GetBoundsProvider() {
			if(Component == null) return null;
			SmartTagSupportAttribute attribute = Attribute.GetCustomAttribute(Component.GetType(), typeof(SmartTagSupportAttribute)) as SmartTagSupportAttribute;
			if(attribute == null) return null;
			return Activator.CreateInstance(attribute.BoundsProviderType) as ISmartTagClientBoundsProvider;
		}
		protected virtual void OnNotifyObserver() {
			ISmartTagClientBoundsProviderEx boundsProvider = GetBoundsProvider() as ISmartTagClientBoundsProviderEx;
			if(boundsProvider == null) return;
			Control parent;
			ISmartTagGlyphObserver observer = boundsProvider.GetObserver(Component, out parent);
			if(observer != null) observer.OnComponentSmartTagChanged(boundsProvider.GetOwnerControl(Component), CalcGlyphBounds(parent));
		}
		protected Rectangle CalcGlyphBounds(Control parent) {
			Rectangle bounds = Owner.RectangleToScreen(CalcBounds());
			return DesignTimeGlyphHelper.CalcGlyphBounds(parent.RectangleToClient(bounds));
		}
		public Glyph Glyph { get; private set; }
		public Control Owner { get; private set; }
		public IComponent Component { get; private set; }
		public object ActionUI { get; private set; }
		public BehaviorService BehaviorService { get; private set; }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal virtual void Dispose(bool disposing) {
			if(disposing) {
				HidePopupPanel();
			}
			Glyph = null;
			Owner = null;
			Component = null;
			ActionUI = null;
			BehaviorService = null;
		}
	}
}

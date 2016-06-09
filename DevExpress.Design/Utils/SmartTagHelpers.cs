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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using DevExpress.Design.UI;
namespace DevExpress.Utils.Design {
	internal class TypeConverterWrapper : TypeConverter, ITypeDescriptorContext {
		PropertyDescriptor descriptor;
		TypeConverter source;
		object instance;
		public TypeConverterWrapper(object instance, PropertyDescriptor descriptor, TypeConverter source) {
			this.instance = instance;
			this.descriptor = descriptor;
			this.source = source;
		}
		#region ITypeDescriptorContext Members
		IContainer ITypeDescriptorContext.Container {
			get {
				return GetSite() == null ? null : GetSite().Container;
			}
		}
		ISite GetSite() {
			return SmartTagHelper.GetSite(instance);
		}
		object ITypeDescriptorContext.Instance { get { return instance; } }
		void ITypeDescriptorContext.OnComponentChanged() { }
		bool ITypeDescriptorContext.OnComponentChanging() { return false; }
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return descriptor; } }
		object IServiceProvider.GetService(Type serviceType) {
			if(GetSite() == null) return null;
			return GetSite().GetService(serviceType);
		}
		ITypeDescriptorContext GetContext(ITypeDescriptorContext source) {
			return this;
		}
		#endregion
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return source.CanConvertFrom(GetContext(context), sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return source.CanConvertTo(GetContext(context), destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			return source.ConvertFrom(GetContext(context), culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			return source.ConvertTo(GetContext(context), culture, value, destinationType);
		}
		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues) {
			return source.CreateInstance(GetContext(context), propertyValues);
		}
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
			return source.GetCreateInstanceSupported(GetContext(context));
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return source.GetProperties(GetContext(context), value, attributes);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return source.GetPropertiesSupported(GetContext(context));
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return source.GetStandardValues(GetContext(context));
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return source.GetStandardValuesExclusive(GetContext(context));
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return source.GetStandardValuesSupported(GetContext(context));
		}
		public override bool IsValid(ITypeDescriptorContext context, object value) {
			return source.IsValid(GetContext(context), value);
		}
	}
	internal class UITypeEditorWrapper : UITypeEditor, ITypeDescriptorContext {
		PropertyDescriptor descriptor;
		UITypeEditor source;
		object instance;
		public UITypeEditorWrapper(object instance, PropertyDescriptor descriptor, UITypeEditor source) {
			this.instance = instance;
			this.descriptor = descriptor;
			this.source = source;
		}
		#region ITypeDescriptorContext Members
		IContainer ITypeDescriptorContext.Container {
			get {
				return GetSite() == null ? null : GetSite().Container;
			}
		}
		ISite GetSite() {
			return SmartTagHelper.GetSite(instance);
		}
		object ITypeDescriptorContext.Instance { get { return instance; } }
		void ITypeDescriptorContext.OnComponentChanged() { }
		bool ITypeDescriptorContext.OnComponentChanging() { return false; }
		PropertyDescriptor ITypeDescriptorContext.PropertyDescriptor { get { return descriptor; } }
		object IServiceProvider.GetService(Type serviceType) {
			if(GetSite() == null) return null;
			return GetSite().GetService(serviceType);
		}
		ITypeDescriptorContext GetContext(ITypeDescriptorContext source) {
			return this;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			return source.EditValue(GetContext(context), provider, value);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return source.GetEditStyle(GetContext(context));
		}
		public override bool IsDropDownResizable { get { return source.IsDropDownResizable; } }
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			return source.GetPaintValueSupported(GetContext(context));
		}
		public override void PaintValue(PaintValueEventArgs e) {
			e = new PaintValueEventArgs(GetContext(e.Context), e.Value, e.Graphics, e.Bounds);
			source.PaintValue(e);
		}
		#endregion
	}
	public class SmartDesignerActionList : DesignerActionList, ICustomTypeDescriptor {		
		public class SmartDesignerPropertyDescriptorCollection : PropertyDescriptorCollection {
			public SmartDesignerPropertyDescriptorCollection(PropertyDescriptor[] properties) : base(properties) { }
			PropertyDescriptor[] Properties { get { return this.GetType().BaseType.GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this) as PropertyDescriptor[]; } }
			public override PropertyDescriptor this[string name] {
				get {
					foreach(var property in Properties){
						SmartDesignerPropertyDescriptor descriptor = property as SmartDesignerPropertyDescriptor;
						if(descriptor != null) {
							string realName = descriptor.Instance.GetType().ToString() + descriptor.Name;
							if(name.Equals(realName)) return descriptor;
						}
					}					
					return base[name];
				}
			}
		}
		protected internal class SmartDesignerPropertyDescriptor : PropertyDescriptor {
			PropertyDescriptor source;
			object instance;
			public SmartDesignerPropertyDescriptor(object instance, PropertyDescriptor source)
				: base(source) {
				this.instance = instance;
				this.source = source;
			}
			UITypeEditor editor = null;
			public override object GetEditor(Type editorBaseType) {
				if(editorBaseType.Equals(typeof(UITypeEditor))) {
					if(editor == null) editor = base.GetEditor(editorBaseType) as UITypeEditor;
					if(editor != null) return new UITypeEditorWrapper(instance, this.source, this.editor);
					return base.GetEditor(editorBaseType);
				}
				return base.GetEditor(editorBaseType);
			}
			TypeConverter converter = null;
			public override TypeConverter Converter {
				get {
					if(converter != null) return converter;
					converter = base.Converter;
					if(converter != null) {
						converter = new TypeConverterWrapper(this.instance, this.source, converter);
					}
					return converter;
				}
			}
			public object Instance { get { return instance; } }
			public override bool CanResetValue(object component) { return source.CanResetValue(Instance); }
			public override Type ComponentType { get { return source.ComponentType; } }
			public override object GetValue(object component) { return source.GetValue(Instance); }
			public override bool IsReadOnly { get { return source.IsReadOnly; } }
			public override Type PropertyType { get { return source.PropertyType; } }
			public override void ResetValue(object component) { source.ResetValue(Instance); }
			public override void SetValue(object list, object value) {
				IComponent component = ((SmartDesignerActionList)list).Component;
				OnComponentPropertyChanging(new ComponentPropertyChingingEventArgs(source, component));
				try {
					source.SetValue(Instance, GetEffectiveValue(value));
				}
				catch { }
				OnComponentPropertyChanged(new ComponentPropertyChangedEventArgs(source, value));
			}
			protected object GetEffectiveValue(object value) { 
				if(value == null) return value;
				TypeConverterWrapper tc = Converter as TypeConverterWrapper;
				if(tc != null && tc.CanConvertFrom(tc, value.GetType())) {
					object res = tc.ConvertFrom(tc, CultureInfo.CurrentCulture, value);
					if(res == null || res.GetType() != value.GetType()) value = res;
				}
				return value;
			}
			public override bool ShouldSerializeValue(object component) { return source.ShouldSerializeValue(((SmartDesignerActionList)component).Component); }
			#region Property Changed 
			public class ComponentPropertyChangedEventArgs : EventArgs {
				object newValue;
				PropertyDescriptor pd;
				public ComponentPropertyChangedEventArgs(PropertyDescriptor pd, object newValue) {
					this.pd = pd;
					this.newValue = newValue;
				}
				public object NewValue { get { return newValue; } }
				public PropertyDescriptor Pd { get { return pd; } }
			}
			public class ComponentPropertyChingingEventArgs : EventArgs {
				IComponent component;
				PropertyDescriptor pd;
				public ComponentPropertyChingingEventArgs(PropertyDescriptor pd, IComponent component) {
					this.pd = pd;
					this.component = component;
				}
				public PropertyDescriptor Pd { get { return pd; } }
				public IComponent Component { get { return component; } }
			}
			public delegate void PropertyChangedCallback(object sender, ComponentPropertyChangedEventArgs e);
			public delegate void PropertyChingingCallback(object sender, ComponentPropertyChingingEventArgs e);
			public event PropertyChingingCallback ComponentPropertyChanging;
			protected void OnComponentPropertyChanging(ComponentPropertyChingingEventArgs e) {
				if(ComponentPropertyChanging != null)
					ComponentPropertyChanging(this, e);
			}
			public event PropertyChangedCallback ComponentPropertyChanged;
			protected void OnComponentPropertyChanged(ComponentPropertyChangedEventArgs e) {
				if(ComponentPropertyChanged != null)
					ComponentPropertyChanged(this, e);
			}
			#endregion
		}
		bool allowUpdateLocation;
		ComponentDesigner designer;
		public SmartDesignerActionList(ComponentDesigner designer, IComponent component)
			: this(designer, component, false) {
		}
		public SmartDesignerActionList(ComponentDesigner designer, IComponent component, bool allowUpdateLocation)
			: base(component) {
			this.designer = designer;
			this.allowUpdateLocation = allowUpdateLocation;
			designerActionProperties = new Dictionary<PropertyDescriptor, DesignerActionPropertyItem>();
		}
		protected ComponentDesigner Designer { get { return designer; } }
		protected bool AllowUpdateLocation { get { return allowUpdateLocation; } }
		#region ICustomTypeDescriptor Members
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return new AttributeCollection();
		}
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(this, true); }
		string ICustomTypeDescriptor.GetComponentName() { return string.Empty; }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(this, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(this, true); }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return FilterProperties(TypeDescriptor.GetProperties(Component, attributes));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return FilterProperties(TypeDescriptor.GetProperties(Component));
		}
		void ChangeCategoryNestedProperties(PropertyDescriptorCollection source, string category) {
			foreach(var q in source.Cast<PropertyDescriptor>()) {
				SmartTagPropertyAttribute tag = (SmartTagPropertyAttribute)q.Attributes[typeof(SmartTagPropertyAttribute)];
				if(tag != null && (tag.Category == null || tag.Category == "")) tag.Category = category;
			}
		}
		List<PropertyDescriptor> FilterSmartTagProperties(PropertyDescriptorCollection source, object component) {			
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			if(source == null) return properties;
			foreach(var q in source.Cast<PropertyDescriptor>()) {
				if(PreFilterProperties(q) && BrowsableProperty(q)) {
					if(q.Attributes[typeof(SmartTagPropertyAttribute)] != null)
						properties.Add(CreateSmartPropertyDescriptor(q, component));
					if(q.Attributes[typeof(SmartTagSearchNestedPropertiesAttribute)] != null) {
						object instance = q.GetValue(component);
						SmartTagSearchNestedPropertiesAttribute tag = (SmartTagSearchNestedPropertiesAttribute)q.Attributes[typeof(SmartTagSearchNestedPropertiesAttribute)];
						ChangeCategoryNestedProperties(TypeDescriptor.GetProperties(instance), tag.Category);					   
						properties.AddRange(FilterSmartTagProperties(TypeDescriptor.GetProperties(instance), instance));
					}
				}
			}	 
			return properties;
		}
		Dictionary<PropertyDescriptor, DesignerActionPropertyItem> designerActionProperties;
		List<PropertyDescriptor> FilterDesignerProperties() {
			List<PropertyDescriptor> visibleProperties = new List<PropertyDescriptor>();
			designerActionProperties.Clear();
			if(Designer == null || Designer.ActionLists == null) return visibleProperties;
			foreach(DesignerActionList actionList in Designer.ActionLists) {
				if(actionList is SmartDesignerActionList || actionList == null) continue;
				var properties = TypeDescriptor.GetProperties(actionList.GetType());												
				DesignerActionItemCollection itemCollection = actionList.GetSortedActionItems();
				if(itemCollection == null || properties == null) continue;
				foreach(DesignerActionItem actionItem in itemCollection) {
					DesignerActionPropertyItem actionPropertyItem = actionItem as DesignerActionPropertyItem;
					if(actionPropertyItem != null) {						
						SmartDesignerPropertyDescriptor property = CreateSmartPropertyDescriptor(properties[actionPropertyItem.MemberName], actionList);
						designerActionProperties.Add(property, actionPropertyItem);
						visibleProperties.Add(property);
					}
				}
			}
			return visibleProperties;
		}
		SmartDesignerPropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection source) {
			var visibleProperties = FilterSmartTagProperties(source, Component);
			visibleProperties.AddRange(FilterDesignerProperties());
			return new SmartDesignerPropertyDescriptorCollection(visibleProperties.OrderBy(q => q.Name).ToArray());
		}
		protected virtual bool BrowsableProperty(PropertyDescriptor pd) {
			BrowsableAttribute attribute = pd.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;
			if(attribute != null) return attribute.Browsable;
			return true;
		}
		protected virtual SmartDesignerPropertyDescriptor CreateSmartPropertyDescriptor(PropertyDescriptor pd, object component) {
			SmartDesignerPropertyDescriptor res = new SmartDesignerPropertyDescriptor(component, pd);
			res.ComponentPropertyChanged += (sender, e) => {				
				if(ShouldRefreshBoundsSmartTag(e.Pd) || ShouldRefreshSmartTag(e.Pd)) BaseDesignerActionListGlyphHelper.RefreshSmartPanelBounds(Component);
				if(ShouldRefreshContentSmartTag(e.Pd) || ShouldRefreshSmartTag(e.Pd)) BaseDesignerActionListGlyphHelper.RefreshSmartPanelContent(Component);
			};
			return res;
		}
		protected virtual SmartDesignerPropertyDescriptor CreateSmartPropertyDescriptor(PropertyDescriptor pd) {
			return CreateSmartPropertyDescriptor(pd, Component);
		}
		SmartTagPropertyAttribute GetSmartTagPropertyAttribute(PropertyDescriptor pd) {
			return pd.Attributes[typeof(SmartTagPropertyAttribute)] as SmartTagPropertyAttribute;
		}
		bool AllowFinalAction(PropertyDescriptor pd, SmartTagActionType action) {
			SmartTagPropertyAttribute attribute = GetSmartTagPropertyAttribute(pd);
			return attribute != null && attribute.FinalAction == action;
		}
		protected virtual bool ShouldRefreshBoundsSmartTag(PropertyDescriptor pd) {
			if(!AllowUpdateLocation) return false;
			return AllowFinalAction(pd, SmartTagActionType.RefreshBoundsAfterExecute);
		}
		protected virtual bool ShouldRefreshSmartTag(PropertyDescriptor pd) {
			if(!AllowUpdateLocation) return false;
			return AllowFinalAction(pd, SmartTagActionType.RefreshAfterExecute);
		}
		protected virtual bool ShouldRefreshContentSmartTag(PropertyDescriptor pd) {			
			return AllowFinalAction(pd, SmartTagActionType.RefreshContentAfterExecute);
		}
		protected ISmartTagFilter GetInstanceFilter() {
			SmartTagFilterAttribute filter = Attribute.GetCustomAttribute(GetMethodsSourceObject().GetType(), typeof(SmartTagFilterAttribute)) as SmartTagFilterAttribute;
			if(filter != null) {
				ISmartTagFilter instance = Activator.CreateInstance(filter.FilterProvedrType) as ISmartTagFilter;
				instance.SetComponent(Component);
				return instance;
			}
			return null;
		}
		protected virtual bool PreFilterProperties(MemberDescriptor descriptor) {
			ISmartTagFilter instance = GetInstanceFilter();
			if(instance != null)
				return instance.FilterProperty(descriptor);
			return true;
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			if(Component.GetType().IsAssignableFrom(pd.ComponentType)) return Component;
			return this;
		}
		#endregion
		#region PropertiesComparer
		internal class PropertiesComparer : IComparer, IComparer<SmartDesignerPropertyDescriptor> {
			public int Compare(object x, object y) {
				SmartDesignerPropertyDescriptor xPd = (SmartDesignerPropertyDescriptor)x;
				SmartDesignerPropertyDescriptor yPd = (SmartDesignerPropertyDescriptor)y;
				return Compare(xPd, yPd);
			}
			public int Compare(SmartDesignerPropertyDescriptor x, SmartDesignerPropertyDescriptor y) {
				var attrX = x.Attributes[typeof(SmartTagPropertyAttribute)] as SmartTagPropertyAttribute;
				var attrY = y.Attributes[typeof(SmartTagPropertyAttribute)] as SmartTagPropertyAttribute;				
				if(attrX == null) return CompareCoreNullX(x.Name, y.Name, attrY);
				if(attrY == null) return CompareCoreNullY(x.Name, y.Name, attrX);
				return CompareCore(x.Name, y.Name, attrX, attrY);
			}
			protected internal int CompareCoreNullX(string xName, string yName, SmartTagPropertyAttribute attrY) {
				if(attrY != null && attrY.SortOrder != SmartTagPropertyAttribute.DefaultSortOrder) return -1;
				return xName.CompareTo(yName);
			}
			protected internal int CompareCoreNullY(string xName, string yName, SmartTagPropertyAttribute attrX) {
				if(attrX != null && attrX.SortOrder != SmartTagPropertyAttribute.DefaultSortOrder) return 1;
				return xName.CompareTo(yName);
			}
			protected internal int CompareCore(string xName, string yName, SmartTagPropertyAttribute attrX, SmartTagPropertyAttribute attrY) {				
				if(attrX.SortOrder == SmartTagPropertyAttribute.DefaultSortOrder && attrY.SortOrder != SmartTagPropertyAttribute.DefaultSortOrder)
					return -1;
				if(attrY.SortOrder == SmartTagPropertyAttribute.DefaultSortOrder && attrX.SortOrder != SmartTagPropertyAttribute.DefaultSortOrder)
					return 1;
				if(attrX.SortOrder == attrY.SortOrder)
					return xName.CompareTo(yName);
				return attrX.SortOrder.CompareTo(attrY.SortOrder);
			}
		}
		#endregion
		#region CategoriesComparer
		internal class CategoriesComparer : IComparer, IComparer<string> {
			public int Compare(object x, object y) {
				return Compare((string)x, (string)y);
			}
			public int Compare(string x, string y) {
				if(string.IsNullOrEmpty(x)) return 1;
				if(string.IsNullOrEmpty(y)) return -1;
				return x.CompareTo(y);
			}
		}
		#endregion
		public override DesignerActionItemCollection GetSortedActionItems() {
			DesignerActionItemCollection res = new DesignerActionItemCollection();
			PropertyDescriptorCollection properties = ((ICustomTypeDescriptor)this).GetProperties().Sort(new PropertiesComparer());
			CreateCategories(res, properties);
			CreateProperties(res, properties);			
			CreateActions(res);			
			return res;
		}
		protected IList GetDesignerActionItems(IList collection, Func<DesignerActionItem, object> getObject) {
			if(Designer == null || Designer.ActionLists == null) return collection;
			foreach(DesignerActionList actionList in Designer.ActionLists) {
				if(actionList is SmartDesignerActionList) continue;
				GetDesignerActionItemsFromDesignerActionList(actionList, collection, getObject);
			}
			return collection;
		}
		void GetDesignerActionItemsFromDesignerActionList(DesignerActionList actionList, IList collection, Func<DesignerActionItem, object> getObject) {
			if(actionList == null || getObject == null) return;
			DesignerActionItemCollection itemCollection = actionList.GetSortedActionItems();
			if(itemCollection != null) {
				foreach(DesignerActionItem actionItem in itemCollection)
					GetDesignerActionItemsFromDesignerActionItemCollection(actionItem, collection, getObject);
			}
		}
		void GetDesignerActionItemsFromDesignerActionItemCollection(DesignerActionItem actionItem, IList collection, Func<DesignerActionItem, object> getObject) {
			if(actionItem == null) return;
			object item = getObject(actionItem);
			if(item != null)
				collection.Add(item);
		}
		protected object GetDesignerCategory(DesignerActionItem actionItem){
			if(actionItem is DesignerActionHeaderItem) return actionItem.DisplayName;
			return null;
		}
		protected virtual List<string> GetDesignerCategories() {
			List<string> categories = new List<string>();					  
			return GetDesignerActionItems(categories, GetDesignerCategory) as List<string>;
		}
		protected virtual void CreateCategories(DesignerActionItemCollection res, PropertyDescriptorCollection properties) {
			List<string> categories = new List<string>(GetDesignerCategories());			
			foreach(PropertyDescriptor pd in properties) {
				string name, displayName, category;
				ResolveProperty(pd, out name, out displayName, out category);
				if(string.IsNullOrEmpty(category) || categories.Contains(category))
					continue;
				categories.Add(category);
			}			
			categories.Sort(new CategoriesComparer());
			categories.ForEach(category => res.Add(new DesignerActionHeaderItem(category, category)));
		}
		protected virtual void CreateProperties(DesignerActionItemCollection res, PropertyDescriptorCollection properties) {
			foreach(PropertyDescriptor pd in properties) {
				string name, displayName, category;				
				ResolveProperty(pd, out name, out displayName, out category);
				SmartDesignerPropertyDescriptor p = pd as SmartDesignerPropertyDescriptor;
				res.Add(new DesignerActionPropertyItem(p.Instance.GetType().ToString() + name, displayName, category) { RelatedComponent = Component });
			}
		}	   
		bool IsValidAction(DesignerActionMethodItem actionMethodItem) {
			if(actionMethodItem == null) return false;
			return PreFilterMethod(actionMethodItem);
		}
		List<DesignerActionItem> CreateActionList(Attribute[] actions, MethodInfo info) {
			List<DesignerActionItem> actionList = new List<DesignerActionItem>();
			if(actions == null) return actionList;
			foreach(SmartTagActionAttribute attr in actions) {
				string name = info != null ? info.Name : attr.MethodName;
				SmartDesignerActionMethodItem action = new SmartDesignerActionMethodItem(this, attr.Type, name, attr.DisplayName, attr.SortOrder, attr.FinalAction);
				if(IsValidAction(action)) actionList.Add(action);
			}
			return actionList;
		}
		List<DesignerActionItem> GetSmartTagActions() {
			List<DesignerActionItem> actionList = new List<DesignerActionItem>();
			MethodInfo[] methods = Component.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			foreach(MethodInfo mi in methods) {
				Attribute[] miActions = Attribute.GetCustomAttributes(mi, typeof(SmartTagActionAttribute));
				actionList.AddRange(CreateActionList(miActions, mi));
			}
			Attribute[] actions = (from action in Attribute.GetCustomAttributes(GetMethodsSourceObject().GetType(), typeof(SmartTagActionAttribute)).Cast<SmartTagActionAttribute>() select action).OrderBy(attr => attr.SortOrder).ThenBy(attr => attr.DisplayName).ToArray();
			actionList.AddRange(CreateActionList(actions, null));
			return actionList;
		}
		List<DesignerActionItem> GetDesignerActions() {
			List<DesignerActionItem> actionList = new List<DesignerActionItem>();
			return GetDesignerActionItems(actionList, GetDesignerAction) as List<DesignerActionItem>;
		}
		protected object GetDesignerAction(DesignerActionItem actionItem) {
			DesignerActionMethodItem action = actionItem as DesignerActionMethodItem;
			if(IsValidAction(action)) return actionItem;
			return null;
		}
		protected virtual void CreateActions(DesignerActionItemCollection res) {
			List<DesignerActionItem> actions = GetSmartTagActions();
			actions.AddRange(GetDesignerActions());
			foreach(DesignerActionItem actionItem in actions)
				res.Add(actionItem);
		}
		protected virtual bool PreFilterMethod(DesignerActionMethodItem actionMethodItem) {
			 ISmartTagFilter instance = GetInstanceFilter();
			if(instance != null)
				return instance.FilterMethod(actionMethodItem.MemberName, actionMethodItem);
			return true; 
		}
		protected object GetMethodsSourceObject() {
			if(Component is IDXObjectWrapper) {
				return ((IDXObjectWrapper)Component).SourceObject;
			}
			return Component;
		}
		protected void ResolveProperty(PropertyDescriptor descriptor, out string name, out string displayName, out string category) {
			category = string.Empty;
			name = descriptor.Name;
			displayName = descriptor.DisplayName;
			SmartTagPropertyAttribute attr = (SmartTagPropertyAttribute)descriptor.Attributes[typeof(SmartTagPropertyAttribute)];
			if(attr != null) {
				if(!string.IsNullOrEmpty(attr.DisplayName)) displayName = attr.DisplayName;
				if(!string.IsNullOrEmpty(attr.Category)) category = attr.Category;
				return;
			}
			DesignerActionPropertyItem actionPropertyItem;
			if(designerActionProperties.TryGetValue(descriptor, out actionPropertyItem)) {
				if(!string.IsNullOrEmpty(actionPropertyItem.DisplayName)) displayName = actionPropertyItem.DisplayName;
				if(!string.IsNullOrEmpty(actionPropertyItem.Category)) category = actionPropertyItem.Category;
			}
		}
		#region SmartDesignerActionMethodItem      
		public class SmartDesignerActionMethodItem : SmartDesignerActionMethodObjectItem {
			Type targetType;
			int sortOrder;
			public SmartDesignerActionMethodItem(DesignerActionList list, Type targetType, string memberName, string displayName, SmartTagActionType finalAction)
				: this(list, targetType, memberName, displayName, -1, finalAction) {
			}
			public SmartDesignerActionMethodItem(DesignerActionList list, Type targetType, string memberName, string displayName, int sortOrder, SmartTagActionType finalAction)
				: base(list, memberName, displayName, finalAction) {			
				this.targetType = targetType;
				this.sortOrder = sortOrder;
			}
			public int SortOrder {
				get { return sortOrder; }
			}
			protected override void InvokeCore() {
				if(targetType == null) {
				   base.InvokeCore();
				   return;
				}
				object instance = Activator.CreateInstance(targetType);
				MethodInfo methodInfo = this.targetType.GetMethod(MemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				methodInfo.Invoke(instance, new object[] { List.Component });
			}		 
		}
		public class SmartDesignerActionMethodObjectItem : DesignerActionMethodItem {			
			DesignerActionList list;
			SmartTagActionType finalAction;
			public SmartDesignerActionMethodObjectItem(DesignerActionList list, string memberName, string displayName, SmartTagActionType finalAction)
				: base(list, memberName, displayName, CategoryName) {
				this.list = list;
				this.finalAction = finalAction;
			}
			public override void Invoke() {
				if(ShouldHideSmartTag) {
					OnHideSmartTag();
				}
				InvokeCore();
				if(ShouldRefreshBoundsSmartTag || ShouldRefreshSmartTag) {
					OnRefreshBoundsSmartTag();
				}
				if(ShouldRefreshContentSmartTag || ShouldRefreshSmartTag) {
					OnRefreshContentSmartTag();
				}
			}
			protected virtual void InvokeCore() {
				object instance = List.Component;
				MethodInfo methodInfo = instance.GetType().GetMethod(MemberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				methodInfo.Invoke(instance, null);
			}
			protected virtual bool ShouldHideSmartTag {
				get {
					if(ActionUIService == null || GetSiteHolder() == null) return false;
					return FinalAction == SmartTagActionType.CloseAfterExecute;
				}
			}
			bool AllowFinalAction(SmartTagActionType action) {
				if(ShouldHideSmartTag || GetSiteHolder() == null)
					return false;
				return FinalAction == action;
			}
			protected virtual bool ShouldRefreshSmartTag {
				get {
					return AllowFinalAction(SmartTagActionType.RefreshAfterExecute);
				}
			}
			protected virtual bool ShouldRefreshBoundsSmartTag {
				get {
					return AllowFinalAction(SmartTagActionType.RefreshBoundsAfterExecute);
				}
			}
			protected virtual bool ShouldRefreshContentSmartTag {
				get {
					return AllowFinalAction(SmartTagActionType.RefreshContentAfterExecute);
				}
			}
			protected virtual void OnHideSmartTag() {
				BaseDesignerActionListGlyphHelper.HideSmartPanel(GetSiteHolder());
			}
			protected IComponent GetSiteHolder() {
				return List.Component;
			}
			protected virtual void OnRefreshSmartTag() {
				OnRefreshBoundsSmartTag();
				OnRefreshContentSmartTag();
			}
			protected virtual void OnRefreshBoundsSmartTag() {
				BaseDesignerActionListGlyphHelper.RefreshSmartPanelBounds(GetSiteHolder());
			}
			protected virtual void OnRefreshContentSmartTag() {
				BaseDesignerActionListGlyphHelper.RefreshSmartPanelContent(GetSiteHolder());
			}
			DesignerActionUIService actionUIService = null;
			protected DesignerActionUIService ActionUIService {
				get {
					if(actionUIService == null) {
						actionUIService = GetService<DesignerActionUIService>();
					}
					return actionUIService;
				}
			}
			protected T GetService<T>() where T : class {
				IComponent component = List.Component;
				if(List.Component is IChildVisualElement) {
					component = ((IChildVisualElement)component).Owner;
				}
				if(component == null || component.Site == null)
					return null;
				return component.Site.GetService(typeof(T)) as T;
			}
			public DesignerActionList List { get { return list; } }
			public SmartTagActionType FinalAction { get { return finalAction; } }
			public static readonly string CategoryName = "Actions";
		}
		#endregion
	}
}

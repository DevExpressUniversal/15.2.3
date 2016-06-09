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
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace DevExpress.Utils.Design {
	public class InitAssemblyResolverAttribute : Attribute {
		static InitAssemblyResolverAttribute() {
			DevExpress.Utils.Design.DXAssemblyResolverEx.Init();
		}
	}
	[Obsolete("The ResourceStreamAttribute has become obsolete. Use the ToolboxBitmap24Attribute or ToolboxBitmap32Attribute instead.")]
	public sealed class ResourceStreamAttribute : Attribute {
		string resourceName;
		public string Name { get; private set; }
		Assembly assembly;
		Assembly Assembly {
			get {
				if(assembly == null) {
					string[] items = resourceName.Split(',');
					assembly = Assembly.Load(items[1].Trim());
				}
				return assembly;
			}
		}
		public ResourceStreamAttribute(string name, string resourceName) {
			Name = name;
			this.resourceName = resourceName;
		}
		public System.IO.Stream GetStream() {
			string[] items = resourceName.Split(',');
			return Assembly.GetManifestResourceStream(items[0].Trim());
		}
		public override bool Equals(object obj) {
			ResourceStreamAttribute attr = obj as ResourceStreamAttribute;
			if(attr != null)
				return Equals(Name, attr.Name) && Equals(resourceName, attr.resourceName);
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public abstract class ToolboxBitmapAttributeBase : Attribute { 
		string resourceName;
		Assembly assembly;
		Assembly Assembly {
			get {
				if(assembly == null) {
					string[] items = resourceName.Split(',');
					assembly = Assembly.Load(items[1].Trim());
				}
				return assembly;
			}
		}
		public ToolboxBitmapAttributeBase(string resourceName) {
			this.resourceName = resourceName;
		}
		public System.IO.Stream GetStream() {
			string[] items = resourceName.Split(',');
			return Assembly.GetManifestResourceStream(items[0].Trim());
		}
		public override bool Equals(object obj) {
			ToolboxBitmapAttributeBase attr = obj as ToolboxBitmapAttributeBase;
			if(attr != null)
				return Equals(resourceName, attr.resourceName);
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public sealed class ToolboxBitmap24Attribute : ToolboxBitmapAttributeBase {
		public ToolboxBitmap24Attribute(string resourceName)
			: base(resourceName) {
		}
	}
	public sealed class ToolboxBitmap32Attribute : ToolboxBitmapAttributeBase { 
		public ToolboxBitmap32Attribute(string resourceName)
			: base(resourceName) {
		}
	}
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyServiceClassAttribute : Attribute {
		public static IDXImagesProvider CreateDXImagesProvider(Assembly imagesAssembly) {
			AssemblyServiceClassAttribute attr = Attribute.GetCustomAttribute(imagesAssembly, typeof(AssemblyServiceClassAttribute)) as AssemblyServiceClassAttribute;
			if(attr == null)
				return null;
			return imagesAssembly.CreateInstance(attr.TypeName) as IDXImagesProvider;
		}
		string typeName;
		public AssemblyServiceClassAttribute(string typeName) {
			this.typeName = typeName;
		}
		public string TypeName { get { return typeName; } }
	}
	#region EditorLoader
	public class EditorLoader : UITypeEditor {
		UITypeEditor baseEditor;
		protected EditorLoaderAttribute GetEditorLoaderAttribute(ITypeDescriptorContext context) {
			return (EditorLoaderAttribute)context.PropertyDescriptor.Attributes[typeof(EditorLoaderAttribute)];
		}
		protected virtual UITypeEditor GetBaseEditor(ITypeDescriptorContext context) {
			if(baseEditor == null) {
				EditorLoaderAttribute attr = GetEditorLoaderAttribute(context);
				if(attr != null) {
					if(string.IsNullOrEmpty(attr.TypeName))
						throw new ArgumentException("TypeName");
					if(string.IsNullOrEmpty(attr.AssemblyName))
						throw new ArgumentException("AssemblyName");
					string assemblyQualifiedName = GetAssemblyQualifiedName(attr.TypeName, attr.AssemblyName);
					Type type = Type.GetType(assemblyQualifiedName, false, false);
					if(type != null)
						baseEditor = Activator.CreateInstance(type) as UITypeEditor;
				}
			}
			return baseEditor;
		}
		string GetAssemblyQualifiedName(string typeName, string assemblyName) {
			string name = GetType().AssemblyQualifiedName;
			string[] nameParts = name.Split(',');
			nameParts[0] = typeName;
			nameParts[1] = assemblyName;
			return string.Join(",", nameParts);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			UITypeEditor baseEditor = GetBaseEditor(context);
			if(baseEditor != null)
				return baseEditor.EditValue(context, provider, value);
			else
				return base.EditValue(context, provider, value);
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			UITypeEditor baseEditor = GetBaseEditor(context);
			if(baseEditor != null)
				return baseEditor.GetEditStyle(context);
			else
				return base.GetEditStyle(context);
		}
		public override bool GetPaintValueSupported(ITypeDescriptorContext context) {
			UITypeEditor baseEditor = GetBaseEditor(context);
			if(baseEditor != null)
				return baseEditor.GetPaintValueSupported(context);
			else
				return base.GetPaintValueSupported(context);
		}
	}
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class EditorLoaderAttribute : Attribute {
		readonly string typeName, assemblyName;
		public EditorLoaderAttribute(string typeName, string assemblyName) {
			this.typeName = typeName;
			this.assemblyName = assemblyName;
		}
		public string TypeName { get { return typeName; } }
		public string AssemblyName { get { return assemblyName; } }
	}
	#endregion
	#region Component SmartTags
	public interface ISmartTagClientBoundsProvider {
		Rectangle GetBounds(IComponent component);
		Control GetOwnerControl(IComponent component);
	}
	public interface ISmartTagClientBoundsProviderEx : ISmartTagClientBoundsProvider {
		ISmartTagGlyphObserver GetObserver(IComponent component, out Control relatedControl);
	}
	public interface ISmartTagGlyphObserver {
		void OnComponentSmartTagChanged(Control owner, Rectangle glyphBounds);
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class SmartTagFilterAttribute : Attribute {
		Type filterProviderType;
		public SmartTagFilterAttribute(Type filterProviderType) {
			this.filterProviderType = filterProviderType;
		}
		public Type FilterProvedrType { get { return filterProviderType; } }
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class SmartTagSupportAttribute : Attribute {
		Type boundsProviderType;
		SmartTagCreationMode creationType;
		public SmartTagSupportAttribute(Type boundsProviderType, SmartTagCreationMode creationType) {
			this.creationType = creationType;
			this.boundsProviderType = boundsProviderType;
		}
		public Type BoundsProviderType { get { return boundsProviderType; } }
		public SmartTagCreationMode CreationType { get { return creationType; } }
		public enum SmartTagCreationMode {
			UseComponentDesigner,
			Auto
		}
	}
	[AttributeUsage(AttributeTargets.Property)]
	public class SmartTagPropertyAttribute : Attribute {
		public SmartTagPropertyAttribute()
			: this(string.Empty, string.Empty) {
		}
		public SmartTagPropertyAttribute(string displayName, string category)
			: this(displayName, category, DefaultSortOrder) {
		}
		public SmartTagPropertyAttribute(string displayName, string category, int sortOrder)
			: this(displayName, category, sortOrder, SmartTagActionType.None) {
		}
		public SmartTagPropertyAttribute(string displayName, string category, SmartTagActionType finalAction)
			: this(displayName, category, DefaultSortOrder, finalAction) {
		}
		public SmartTagPropertyAttribute(string displayName, string category, int sortOrder, SmartTagActionType finalAction) {
			this.DisplayName = displayName;
			this.Category = category;
			this.SortOrder = sortOrder;
			this.FinalAction = finalAction;
		}  
		public string DisplayName { get; set; }		
		public string Category { get; set; }
		public int SortOrder { get; set; }
		public SmartTagActionType FinalAction { get; set; }
		public static readonly int DefaultSortOrder = -1;
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class SmartTagActionAttribute : Attribute {
		Type type;
		string methodName;
		string displayName;
		int sortOrder;
		SmartTagActionType finalAction;
		public SmartTagActionAttribute(string displayName)
			: this(null, null, displayName) {
		}
		public SmartTagActionAttribute(Type type, string methodName, string displayName)
			: this(type, methodName, displayName, SmartTagActionType.None) {
		}
		public SmartTagActionAttribute(Type type, string methodName, string displayName, SmartTagActionType finalAction) 
			: this(type, methodName, displayName, -1, finalAction){
		}
		public SmartTagActionAttribute(Type type, string methodName, string displayName, int sortOrder, SmartTagActionType finalAction) {
			this.type = type;
			this.methodName = methodName;
			this.displayName = displayName;
			this.sortOrder = sortOrder;
			this.finalAction = finalAction;
		}
		public Type Type { get { return type; } }
		public string MethodName { get { return methodName; } }
		public string DisplayName { get { return displayName; } }
		public int SortOrder { get { return sortOrder; } }
		public SmartTagActionType FinalAction { get { return finalAction; } }
	}
	public enum SmartTagActionType {
		None,
		CloseAfterExecute,
		RefreshAfterExecute,
		RefreshBoundsAfterExecute,
		RefreshContentAfterExecute
	}
	#endregion
	public interface IChildVisualElement {
		IComponent Owner { get; }
	}
	public class DesignTimeBoundsProviderAttribute : Attribute {
		public virtual bool ShouldDrawSelection { get { return true; } }
		public virtual Rectangle GetBounds(object obj) { return Rectangle.Empty; }
		public virtual Control GetOwnerControl(object obj) { return null; }
	}
	public enum DesignTimeActionCategory { AddItems, AddDataSource }
	public interface ISmartDesignerActionListFiler {
		bool PreFilterMember(MemberDescriptor member);
	}
	public interface ISmartDesignerActionListOwner {
		bool AllowSmartTag(IComponent component);
	}
	public interface ISmartTagAction {
		void Execute(object component);
		string Text { get; }
		Image Glyph { get; }
	}
	public interface ISmartTagActionWithOptions : ISmartTagAction {
		object[] Actions { get; }
	}
	public interface ISmartTagFilter {
		void SetComponent(IComponent component);
		bool FilterProperty(MemberDescriptor descriptor);
		bool FilterMethod(string MethodName, object actionMethodItem);		
	}
	[AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = true)]
	public abstract class SmartTagActionsProviderAttribute : Attribute {
		public SmartTagActionsProviderAttribute() { }
		public abstract DesignTimeActionCategory ActionsCategory { get; }
		public abstract object[] GetActions(object component);
	}
	public enum SmartTagEditorType { Auto, String, Image, Tag, Enum, ImageIndex, Boolean, Custom, SuperTip }
	public class SmartTagPropertyInfo {
		public SmartTagPropertyInfo(string property) {
			Property = property;
		}
		public SmartTagPropertyInfo(string property, SmartTagEditorType editorType)
			: this(property) {
			EditorType = editorType;
		}
		public SmartTagPropertyInfo(string property, string editorType)
			: this(property) {
			EditorType = SmartTagEditorType.Custom;
			CustomEditorType = editorType;
		}
		public string Property { get; set; }
		public string CustomEditorType { get; set; }
		public SmartTagEditorType EditorType { get; set; }
	}
	public class SmartTagSearchNestedPropertiesAttribute : Attribute {
		public SmartTagSearchNestedPropertiesAttribute(){			
		}
		public SmartTagSearchNestedPropertiesAttribute(string Category) {
			this.Category = Category;
		}
		public string Category { get; set; }
	}
	public class SmartTagSuperTipPropertyInfo : SmartTagPropertyInfo {
		public SmartTagSuperTipPropertyInfo() : base("SuperTip", SmartTagEditorType.SuperTip) { }
	}
	public abstract class SmartTagPropertiesProviderAttribute : Attribute {
		public SmartTagPropertiesProviderAttribute() {
		}
		public abstract object[] GetProperties(object component);
	}
	public class ReferencesHelper {
		public static void EnsureReferences(IDesignerHost provider, params string[] assemblies) {
			try {
				object references = GetReferences(provider);
				if(references == null) return;
				foreach(string asm in assemblies) {
					AddAssemblyReference(references, asm);
				}
			}
			catch { }
		}
		static object GetReferences(IServiceProvider provider) {
			Assembly envDTE = FindEnvDTE();
			object projectItem = FindProjectItem(envDTE, provider);
			if (projectItem == null) {
				projectItem = FindSolutionItemForSharePointApplication(envDTE, provider);
				if (projectItem != null)
					projectItem = GetProjectItemForSharePointSolution(projectItem);
				if (projectItem == null)
					return null;
			}
			return GetPropertyValue(GetPropertyValue(GetPropertyValue(projectItem, "ContainingProject"), "Object"), "References");
		}
		static Assembly FindEnvDTE() {
			try {
				Assembly[] list = AppDomain.CurrentDomain.GetAssemblies();
				foreach(Assembly asm in list) {
					if(asm.GetName().Name.ToLowerInvariant() == "envdte") {
						return asm;
					}
				}
			}
			catch { }
			return null;
		}
		static object FindProjectItem(Assembly envDTE, IServiceProvider provider) {
			if(envDTE == null || provider == null) return null;
			Type type = envDTE.GetType("EnvDTE.ProjectItem", false);
			if(type == null) return null;
			return provider == null ? null : provider.GetService(type);
		}
		static object FindSolutionItemForSharePointApplication(Assembly envDTE, IServiceProvider provider) {
			if (envDTE == null || provider == null) return null;
			Type type = envDTE.GetType("EnvDTE.DTE", false);
			return type == null ? null : provider.GetService(type);
		}
		static object GetProjectItemForSharePointSolution(object vsSharePointSolution) {
			return GetPropertyValue(GetPropertyValue(vsSharePointSolution, "ActiveDocument"), "ProjectItem");
		}
		static void AddAssemblyReference(object references, string identity) {
			if(references == null) return;
			try {
				if(references.GetType().InvokeMember("Find", BindingFlags.Public | BindingFlags.InvokeMethod, null, references, new object[] { identity }) == null) {
					references.GetType().InvokeMember("Add", BindingFlags.Public | BindingFlags.InvokeMethod, null, references, new object[] { identity });
				}
			}
			catch { }
		}
		static object GetPropertyValue(object obj, string name) {
			if(obj == null) return null;
			PropertyDescriptor pd = TypeDescriptor.GetProperties(obj)[name];
			if(pd == null) return null;
			return pd.GetValue(obj);
		}
	}
	public interface ICollectionEditorSupport {
		void ReplaceItems(object[] items);
	}
	public class DXInheritedPropertyDescriptor : PropertyDescriptor {
		static DXInheritedPropertyDescriptor() {
			DXInheritedPropertyDescriptor.noDefault = new object();
		}
		public DXInheritedPropertyDescriptor(PropertyDescriptor propertyDescriptor, object component, bool rootComponent)
			: base(propertyDescriptor, new Attribute[0]) {
			this.propertyDescriptor = propertyDescriptor;
			this.InitInheritedDefaultValue(component, rootComponent);
			bool flag1 = false;
			if(typeof(ICollection).IsAssignableFrom(propertyDescriptor.PropertyType) && propertyDescriptor.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content)) {
				if(propertyDescriptor.Attributes[typeof(InheritableCollectionAttribute)] == null) {
					ICollection collection1 = propertyDescriptor.GetValue(component) as ICollection;
					if((collection1 != null) && (collection1.Count > 0)) {
						MethodInfo[] infoArray2 = TypeDescriptor.GetReflectionType(collection1).GetMethods(BindingFlags.Public | BindingFlags.Instance);
						for(int num1 = 0; num1 < infoArray2.Length; num1++) {
							MethodInfo info1 = infoArray2[num1];
							ParameterInfo[] infoArray1 = info1.GetParameters();
							if(infoArray1.Length == 1) {
								string text1 = info1.Name;
								Type type1 = null;
								if(text1.Equals("AddRange") && infoArray1[0].ParameterType.IsArray) {
									type1 = infoArray1[0].ParameterType.GetElementType();
								}
								else if(text1.Equals("Add")) {
									type1 = infoArray1[0].ParameterType;
								}
								if((type1 != null) && !typeof(IComponent).IsAssignableFrom(type1)) {
									List<Attribute> list1 = new List<Attribute>(this.AttributeArray);
									list1.Add(DesignerSerializationVisibilityAttribute.Hidden);
									list1.Add(ReadOnlyAttribute.Yes);
									list1.Add(new EditorAttribute(typeof(UITypeEditor), typeof(UITypeEditor)));
									list1.Add(new TypeConverterAttribute(typeof(DXInheritedPropertyDescriptor.ReadOnlyCollectionConverter)));
									Attribute[] attributeArray1 = list1.ToArray();
									this.AttributeArray = attributeArray1;
									flag1 = true;
									break;
								}
							}
						}
					}
				}
			}
			if(!flag1 && (this.defaultValue != DXInheritedPropertyDescriptor.noDefault)) {
				List<Attribute> list2 = new List<Attribute>(this.AttributeArray);
				list2.Add(new DefaultValueAttribute(this.defaultValue));
				Attribute[] attributeArray2 = new Attribute[list2.Count];
				list2.CopyTo(attributeArray2, 0);
				this.AttributeArray = attributeArray2;
			}
		}
		public override bool CanResetValue(object component) {
			if(this.defaultValue == DXInheritedPropertyDescriptor.noDefault) {
				return this.propertyDescriptor.CanResetValue(component);
			}
			return !object.Equals(this.GetValue(component), this.defaultValue);
		}
		private object ClonedDefaultValue(object value) {
			DesignerSerializationVisibility visibility1;
			DesignerSerializationVisibilityAttribute attribute1 = (DesignerSerializationVisibilityAttribute)this.propertyDescriptor.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
			if(attribute1 == null) {
				visibility1 = DesignerSerializationVisibility.Visible;
			}
			else {
				visibility1 = attribute1.Visibility;
			}
			if((value != null) && (visibility1 == DesignerSerializationVisibility.Content)) {
				if(value is ICloneable) {
					value = ((ICloneable)value).Clone();
					return value;
				}
				value = DXInheritedPropertyDescriptor.noDefault;
			}
			return value;
		}
		protected override void FillAttributes(IList attributeList) {
			base.FillAttributes(attributeList);
			foreach(Attribute attribute1 in this.propertyDescriptor.Attributes) {
				attributeList.Add(attribute1);
			}
		}
		public override object GetValue(object component) {
			return this.propertyDescriptor.GetValue(component);
		}
		private void InitInheritedDefaultValue(object component, bool rootComponent) {
			try {
				object obj1;
				if(!this.propertyDescriptor.ShouldSerializeValue(component)) {
					DefaultValueAttribute attribute1 = (DefaultValueAttribute)this.propertyDescriptor.Attributes[typeof(DefaultValueAttribute)];
					if(attribute1 != null) {
						this.defaultValue = attribute1.Value;
						obj1 = this.defaultValue;
					}
					else {
						this.defaultValue = DXInheritedPropertyDescriptor.noDefault;
						obj1 = this.propertyDescriptor.GetValue(component);
					}
				}
				else {
					this.defaultValue = this.propertyDescriptor.GetValue(component);
					obj1 = this.defaultValue;
					this.defaultValue = this.ClonedDefaultValue(this.defaultValue);
				}
				this.SaveOriginalValue(obj1);
			}
			catch {
				this.defaultValue = DXInheritedPropertyDescriptor.noDefault;
			}
		}
		public override void ResetValue(object component) {
			if(this.defaultValue == DXInheritedPropertyDescriptor.noDefault) {
				this.propertyDescriptor.ResetValue(component);
			}
			else {
				this.SetValue(component, this.defaultValue);
			}
		}
		private void SaveOriginalValue(object value) {
			if(value is ICollection) {
				this.originalValue = new object[((ICollection)value).Count];
				((ICollection)value).CopyTo((Array)this.originalValue, 0);
			}
			else {
				this.originalValue = value;
			}
		}
		public override void SetValue(object component, object value) {
			this.propertyDescriptor.SetValue(component, value);
		}
		public override bool ShouldSerializeValue(object component) {
			if(this.IsReadOnly) {
				if(this.propertyDescriptor.ShouldSerializeValue(component)) {
					return this.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);
				}
				return false;
			}
			if(this.defaultValue == DXInheritedPropertyDescriptor.noDefault) {
				return this.propertyDescriptor.ShouldSerializeValue(component);
			}
			return !object.Equals(this.GetValue(component), this.defaultValue);
		}
		public override Type ComponentType {
			get {
				return this.propertyDescriptor.ComponentType;
			}
		}
		public override bool IsReadOnly {
			get {
				if(!this.propertyDescriptor.IsReadOnly) {
					return this.Attributes[typeof(ReadOnlyAttribute)].Equals(ReadOnlyAttribute.Yes);
				}
				return true;
			}
		}
		public object OriginalValue { get { return originalValue; } }
		public ICollection OriginalCollection { get { return OriginalValue as ICollection; } }
		public bool IsCollection { get { return OriginalCollection != null; } }
		public bool IsEmptyOriginalCollection { get { return IsCollection && OriginalCollection.Count == 0; } }
		public PropertyDescriptor PropertyDescriptor {
			get {
				return this.propertyDescriptor;
			}
			set {
				this.propertyDescriptor = value;
			}
		}
		public override Type PropertyType {
			get {
				return this.propertyDescriptor.PropertyType;
			}
		}
		private object defaultValue;
		private static object noDefault;
		private object originalValue;
		private PropertyDescriptor propertyDescriptor;
		private class ReadOnlyCollectionConverter : TypeConverter {
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if(destinationType == typeof(string)) {
					return "Read-only";
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}
	public class InheritanceHelper {
		static object disableVisualInheritance = null;
		public static bool DisableVisualInheritance {
			get {
				if(disableVisualInheritance == null) {
					disableVisualInheritance = true;
					try {
						Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Developer Express");
						if(key != null) disableVisualInheritance = bool.Parse(key.GetValue("DisableVisualInheritance", "true").ToString());
					}
					catch {
						disableVisualInheritance = true;
					}
				}
				return (bool)disableVisualInheritance;
			}
		}
		class Provider : IServiceProvider {
			internal static Provider NullServiceProvider = new Provider();
			object IServiceProvider.GetService(Type serviceType) { return null; }
		}
		public static Component GetComponent(object obj) {
			IDesigner designer = obj as IDesigner;
			if(designer != null) return designer.Component as Component;
			Component comp = obj as Component;
			if(comp != null) return comp;
			return null;
		}
		public static IServiceProvider GetServiceProvider(object obj) {
			Component component = GetComponent(obj);
			if(component != null && component.Site != null) return component.Site;
			IServiceProvider provider = obj as IServiceProvider;
			if(provider == null) provider = Provider.NullServiceProvider;
			return provider;
		}
		public static object GetService(object obj, Type type) { return GetServiceProvider(obj).GetService(type); }
		public static IInheritanceService GetInheritanceService(object obj) {
			return GetService(obj, typeof(IInheritanceService)) as IInheritanceService;
		}
		public static InheritanceAttribute GetInheritanceAttribute(object obj) {
			Component component = GetComponent(obj);
			if(component == null) return null;
			IInheritanceService srv = GetInheritanceService(component);
			if(srv != null) return srv.GetInheritanceAttribute(component);
			return null;
		}
		public static bool AllowCollectionModify(Component baseComponent, PropertyDescriptor collectionDescriptor, ICollection collection) {
			InheritanceAttribute attr = (InheritanceAttribute)TypeDescriptor.GetAttributes(baseComponent)[typeof(InheritanceAttribute)];
			if(attr != null && attr.Equals(InheritanceAttribute.InheritedReadOnly)) return false;
			if(collectionDescriptor.Converter.GetType().Name == "ReadOnlyCollectionConverter") return false;
			DXInheritedPropertyDescriptor pd = collectionDescriptor as DXInheritedPropertyDescriptor;
			if(pd != null) return pd.IsEmptyOriginalCollection || IsComponentCollection(collection);
			return IsComponentCollection(collection);
		}
		public static bool AllowCollectionItemEdit(Component baseComponent, PropertyDescriptor collectionDescriptor, ICollection collection) {
			InheritanceAttribute attr = (InheritanceAttribute)TypeDescriptor.GetAttributes(baseComponent)[typeof(InheritanceAttribute)];
			if(attr != null && attr.Equals(InheritanceAttribute.InheritedReadOnly)) return false;
			return IsComponentCollection(collection);
		}
		public static bool AllowCollectionItemEdit(Component baseComponent, PropertyDescriptor collectionDescriptor, ICollection collection, object item) {
			InheritanceAttribute attribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(item)[typeof(InheritanceAttribute)];
			if(attribute != null && attribute.InheritanceLevel == InheritanceLevel.InheritedReadOnly) return false;
			return IsComponentCollection(collection);
		}
		public static bool AllowCollectionItemRemove(Component baseComponent, PropertyDescriptor collectionDescriptor, ICollection collection, object item) {
			if(!AllowCollectionModify(baseComponent, collectionDescriptor, collection)) return false;
			InheritanceAttribute attribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(item)[typeof(InheritanceAttribute)];
			if(attribute != null && attribute.InheritanceLevel != InheritanceLevel.NotInherited) return false;
			return true;
		}
		internal static bool IsComponentCollection(ICollection collection) {
			Type type = GetIndexerPropertyType(collection);
			return type != null && typeof(IComponent).IsAssignableFrom(type);
		}
		public static Type GetIndexerPropertyType(ICollection collection) {
			if(collection == null) return null;
			System.Reflection.PropertyInfo[] props = collection.GetType().GetProperties();
			for(int i = 0; i < props.Length; i++) {
				if("Item".Equals(props[i].Name) && props[i].PropertyType != typeof(object)) {
					return props[i].PropertyType;
				}
			}
			return null;
		}
	}
	public class InheritableCollectionAttribute : Attribute {
		public InheritableCollectionAttribute() { }
	}
	public class HiddenInheritableCollectionAttribute : InheritableCollectionAttribute {
		public HiddenInheritableCollectionAttribute() : base() { }
	}
	[AttributeUsageAttribute(AttributeTargets.Class)]
	public sealed class DefaultBindingPropertyExAttribute : Attribute {
		public DefaultBindingPropertyExAttribute(string name) { }
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class DXDocumentationProviderAttribute : Attribute {
		string description, link;
		public DXDocumentationProviderAttribute(string description, string link) {
			this.description = description;
			this.link = link;
		}
		public string Description { get { return description; } }
		public string Link { get { return link; } }
		public string GetUrl() { return string.Format("{0}{1}", AssemblyInfo.SRDocumentationLink, Link); }
	}
	public class DXClientDocumentationProviderAttribute : DXDocumentationProviderAttribute {
		public DXClientDocumentationProviderAttribute(string link)
			: base("Client-Side API Documentation", link) {
		}
	}
	public class DXClientDocumentationProviderWebAttribute : DXDocumentationProviderAttribute {
		public DXClientDocumentationProviderWebAttribute(string typeName)
			: base("Client-Side API Documentation", GetLinkByControlType(typeName)) {
		}
		static string GetLinkByControlType(string controlType) {
			return "#AspNet/clsDevExpressWebScripts" + controlType.Replace("ASPx", "ASPxClient") + "topic";
		}
	}
	public class CustomPropertyDescriptor : PropertyDescriptor {
		PropertyDescriptor parentPropertyDescriptor;
		bool browsable;
		bool serializable;
		public override Type ComponentType { get { return this.parentPropertyDescriptor.ComponentType; } }
		public override bool IsReadOnly { get { return this.parentPropertyDescriptor.IsReadOnly; } }
		public override Type PropertyType { get { return this.parentPropertyDescriptor.PropertyType; } }
		public override System.ComponentModel.AttributeCollection Attributes {
			get {
				List<Attribute> list = new List<Attribute>();
				DesignerSerializationVisibilityAttribute dsvAttrOld = null;
				foreach(Attribute attr in this.parentPropertyDescriptor.Attributes) {
					if(attr.GetType() == typeof(DesignerSerializationVisibilityAttribute))
						dsvAttrOld = (DesignerSerializationVisibilityAttribute)attr;
					if(attr.GetType() != typeof(DesignerSerializationVisibilityAttribute) &&
						attr.GetType() != typeof(BrowsableAttribute) &&
						attr.GetType() != typeof(EditorBrowsableAttribute))
						list.Add(attr);
				}
				DesignerSerializationVisibility oldVisibility = dsvAttrOld == null ? DesignerSerializationVisibility.Visible : dsvAttrOld.Visibility;
				list.Add(new DesignerSerializationVisibilityAttribute(serializable ? oldVisibility : DesignerSerializationVisibility.Hidden));
				list.Add(new BrowsableAttribute(browsable));
				list.Add(new EditorBrowsableAttribute(browsable ? EditorBrowsableState.Always : EditorBrowsableState.Never));
				Attribute[] attrs = new Attribute[list.Count];
				list.CopyTo(attrs);
				return new System.ComponentModel.AttributeCollection(attrs);
			}
		}
		public CustomPropertyDescriptor(PropertyDescriptor parentPropertyDescriptor, bool visible)
			: this(parentPropertyDescriptor, visible, visible) {
		}
		public CustomPropertyDescriptor(PropertyDescriptor parentPropertyDescriptor, bool browsable, bool serializable)
			: base(parentPropertyDescriptor) {
			this.parentPropertyDescriptor = parentPropertyDescriptor;
			this.browsable = browsable;
			this.serializable = serializable;
		}
		public override bool CanResetValue(object component) {
			return this.parentPropertyDescriptor.CanResetValue(component);
		}
		public override object GetValue(object component) {
			return this.parentPropertyDescriptor.GetValue(component);
		}
		public override void ResetValue(object component) {
			this.parentPropertyDescriptor.ResetValue(component);
		}
		public override void SetValue(object component, object value) {
			this.parentPropertyDescriptor.SetValue(component, value);
		}
		public override bool ShouldSerializeValue(object component) {
			return this.parentPropertyDescriptor.ShouldSerializeValue(component);
		}
	}
	public class DXAssemblyResolverEx {
		static bool initialized = false;
		static public void Init() {
			if(DevExpress.Data.Helpers.SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.ControlAppDomain)))
				InitInternal();
		}
		[System.Security.SecuritySafeCritical]
		static void InitInternal() {
			if(initialized) return;
			try {
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(OnAssemblyResolve);
			}
			catch {
			}
			initialized = true;
		}
		static int locked = 0;
		[System.Security.SecuritySafeCritical]
		static System.Reflection.Assembly OnAssemblyResolve(object sender, ResolveEventArgs e) {
			if(locked != 0) return null;
			if(e.Name.StartsWith("DevExpress")) {
				if(e.Name.Contains("PublicKeyToken=null")) return null; 
				locked++;
				bool isDesign = e.Name.Contains(".Design"); 
				try {
					Assembly res = FindAssembly(e.Name, !isDesign);
					if(res != null) return res;
					if(e.Name.Contains(".resources")) return null;
					return DevExpress.Data.Utils.AssemblyCache.LoadWithPartialName(GetValidAssemblyName(e.Name, !isDesign));
				}
				catch {
				}
				finally {
					locked--;
				}
			}
			return null;
		}
		[ThreadStatic]
		static Dictionary<string, Assembly> assemblies;
		public static System.Reflection.Assembly FindAssembly(string name) { return FindAssembly(name, true); }
		public static System.Reflection.Assembly FindAssembly(string name, bool patchVersion) {
			string shortName = GetValidShortName(name, patchVersion);
			if(assemblies != null && assemblies.ContainsKey(shortName))
				return assemblies[shortName];
			Assembly[] list = AppDomain.CurrentDomain.GetAssemblies();
			for(int n = list.Length - 1; n >= 0; n--) {
				if(list[n].FullName.StartsWith(shortName, StringComparison.InvariantCulture)) {
					AssemblyName asmName = list[n].GetName();
					if(asmName.Name == shortName && asmName.Version.ToString() == AssemblyInfo.Version) {
						if (assemblies == null) assemblies = new Dictionary<string, Assembly>();
						assemblies[shortName] = list[n];
						return list[n];
					}
				}
			}
			return null;
		}
		static string GetValidShortName(string assemblyName, bool patchVersion) {
			string res = GetValidAssemblyName(assemblyName, patchVersion);
			string[] sname = res.Split(',');
			if(sname.Length > 0) return sname[0];
			return res;
		}
		static Regex typeModuleRegEx;
		static Regex GetTypeModuleRegEx() {
			if(typeModuleRegEx == null)
				typeModuleRegEx = new Regex(@"\[(?<TypeName>[\w\s\.=]+?)\,+\s*(?<AssemblyName>.+?)\]", RegexOptions.Compiled);
			return typeModuleRegEx;
		}
		internal static string GetValidTypeName(string typeName) {
			if(string.IsNullOrEmpty(typeName) || !typeName.Contains("DevExpress")) return typeName;
			return GetTypeModuleRegEx().Replace(typeName, new MatchEvaluator(delegate(Match target) {
				Group asmName = target.Groups["AssemblyName"];
				Group typeGroup = target.Groups["TypeName"];
				if(asmName == null || !asmName.Success || typeGroup == null || !typeGroup.Success) return target.ToString();
				return string.Format("[{0}, {1}]", typeGroup.Value, GetValidAssemblyName(asmName.Value, true));
			}));
		}
		internal static string GetValidAssemblyName(string assemblyName) { return GetValidAssemblyName(assemblyName, true); }
		internal static string GetValidAssemblyName(string assemblyName, bool patchVersion) {
			if(assemblyName == null) return assemblyName;
			string lower = assemblyName.ToLower(CultureInfo.InvariantCulture);
			if(lower.StartsWith("devexpress")) {
				if(lower.StartsWith("devexpress.expressapp")) return assemblyName;
				if(lower.StartsWith("devexpress.persistence")) return assemblyName;
				if(lower.StartsWith("devexpress.persistent")) return assemblyName;
				if(lower.StartsWith("devexpress.workflow")) return assemblyName;
				string[] typeParts = assemblyName.Split(new char[] { ',' });
				if(typeParts != null && typeParts.Length > 1) {
					string partialName = patchVersion ? GetValidModuleName(typeParts[0].Trim()) : typeParts[0].Trim();
					for(int i = 1; i < typeParts.Length; ++i) {
						string s = typeParts[i].Trim();
						if((!s.StartsWith("Version=") && !s.StartsWith("version=")) || !patchVersion) {
							partialName = partialName + ", " + s;
						}
					}
					assemblyName = partialName;
				}
				else {
					assemblyName = patchVersion ? GetValidModuleName(assemblyName) : assemblyName;
				}
				if(patchVersion) assemblyName += ", Version=" + AssemblyInfo.Version;
			}
			return assemblyName;
		}
		static Regex VersionRegEx = new Regex(@".v\d+.\d", RegexOptions.Compiled | RegexOptions.Singleline);
		internal static string GetValidModuleName(string assemblyName) {
			if(VersionRegEx.IsMatch(assemblyName))
				return VersionRegEx.Replace(assemblyName, AssemblyInfo.VSuffix);
			if(assemblyName.IndexOf("3") != -1)
				return assemblyName.Replace("3", AssemblyInfo.VSuffix);
			if(assemblyName.IndexOf("4") != -1)
				return assemblyName.Replace("4", AssemblyInfo.VSuffix);
			if(assemblyName.IndexOf(".Design") != -1)
				return assemblyName.Replace(".Design", AssemblyInfo.VSuffix + ".Design");
			return assemblyName + AssemblyInfo.VSuffix;
		}
	}
	public class DXAssemblyResolver {
		static bool Checked = false;
		[System.Security.SecuritySafeCritical]
		static public void Init() {
			if(Checked) return;
			Checked = true;
			try {
				Assembly asm = System.Reflection.Assembly.GetEntryAssembly();
				if(asm == null) return;
				string name = System.Reflection.Assembly.GetEntryAssembly().FullName;
				if(!name.StartsWith("WinRes")) return;
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(OnAssemblyResolve);
			}
			catch {
			}
		}
		static int locked = 0;
		static System.Reflection.Assembly OnAssemblyResolve(object sender, ResolveEventArgs e) {
			if(locked != 0) return null;
			if(e.Name.Contains(".Design")) return null; 
			if(e.Name.StartsWith("DevExpress")) {
				locked++;
				try {
					return DevExpress.Data.Utils.Helpers.LoadWithPartialName(e.Name);
				}
				catch {
				}
				finally {
					locked--;
				}
			}
			return null;
		}
	}
}
namespace DevExpress.Utils.Design.Filtering {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class FilteringModelMetadataAttribute : Attribute {
		public string Platform {
			get;
			set;
		}
		public string ModelTypeProperty {
			get;
			set;
		}
		public string CustomAttributesProperty {
			get;
			set;
		}
	}
}
namespace DevExpress.Utils.Design.DataAccess {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DataAccessMetadataAttribute : Attribute {
		string supportedTechnologiesCore;
		public DataAccessMetadataAttribute(string technologies) {
			this.supportedTechnologiesCore = technologies;
			EnableDirectBinding = true;
		}
		public string SupportedTechnologies {
			get { return supportedTechnologiesCore; }
		}
		public string SupportedProcessingModes {
			get;
			set;
		}
		public string DataSourceProperty {
			get;
			set;
		}
		public string DataMemberProperty {
			get;
			set;
		}
		public string Platform {
			get;
			set;
		}
		public bool EnableDirectBinding {
			get;
			set;
		}
		public bool EnableBindingToEnum {
			get;
			set;
		}
	}
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class OLAPDataAccessMetadataAttribute : DataAccessMetadataAttribute {
		public OLAPDataAccessMetadataAttribute(string technologies)
			: base(technologies) {
		}
		public string OLAPConnectionStringProperty {
			get;
			set;
		}
		public string OLAPDataProviderProperty {
			get;
			set;
		}
	}
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DashboardDataAccessMetadataAttribute : DataAccessMetadataAttribute {
		public DashboardDataAccessMetadataAttribute(string technologies)
			: base(technologies) {
		}
		public string DesignTimeElementTypeProperty {
			get;
			set;
		}
	}
	public interface ICustomBindingProperty {
		string PropertyName { get; }
		string DisplayName { get; }
		string Description { get; }
	}
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public abstract class CustomBindingPropertiesAttribute : Attribute {
		public virtual IEnumerable<ICustomBindingProperty> GetCustomBindingProperties() {
			var attributes = GetType().GetCustomAttributes(typeof(CustomBindingPropertyAttribute), true);
			for(int i = 0; i < attributes.Length; i++)
				yield return attributes[i] as ICustomBindingProperty;
		}
		[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
		protected class CustomBindingPropertyAttribute : Attribute, ICustomBindingProperty {
			public CustomBindingPropertyAttribute(string propertyName, string displayName, string description) {
				this.PropertyName = propertyName;
				this.DisplayName = displayName;
				this.Description = description;
			}
			public string PropertyName { get; private set; }
			public string DisplayName { get; private set; }
			public string Description { get; private set; }
		}
	}
}

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
using System.Linq;
using System.Text;
using DevExpress.Data.Helpers;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System.ComponentModel;
using DevExpress.Xpf.Editors;
using System.Windows.Markup;
using System.Windows.Data;
using System.Globalization;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Editors.Settings.Extension;
using System.Collections;
using DevExpress.Utils.Design;
using XmlNamespaceConstants = DevExpress.Xpf.Core.Native.XmlNamespaceConstants;
using DevExpress.Entity.Model;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Design.Wizards.Mvvm {
	public class XamlInfo {
		public XamlInfo(string xaml, XamlNamespaces xamlNamespaces) {
			this.Xaml = xaml;
			this.Namespaces = xamlNamespaces;
		}
		public XamlNamespaces Namespaces { get; private set; }
		public string Xaml { get; private set; }
	}
	public class XamlEditingContext : IEditingContext {
		public XamlEditingContext() {
			Namespaces = new XamlNamespaces();
		}
		public XamlInfo GetXamlInfo(IModelItemCollection collection) {
			StringBuilder builder = new StringBuilder();
			foreach(XamlModelItem item in collection) {
				if(builder.Length > 0)
					builder.AppendLine();
				builder.Append(item.GetXaml());
			}
			return new XamlInfo(builder.ToString(), Namespaces);
		}
		public IModelItem CreateItem(Type type) {
			return new XamlModelItem(this, type);
		}
		public string GetTypeXaml(Type type) {
			return GetPrefix(type) + type.Name;
		}
		public XamlNamespaces Namespaces { get; private set; }
		string GetPrefix(Type type) {
			string result = Namespaces.GetPrefix(type);
			if(string.IsNullOrEmpty(result))
				return result;
			return result + ":";
		}
		#region not implemented
		IModelItem IEditingContext.CreateItem(DXTypeIdentifier typeIdentifier) {
			throw new NotImplementedException();
		}
		IModelItem IEditingContext.CreateStaticMemberItem(Type type, string memberName) {
			throw new NotImplementedException();
		}
		IServiceProvider IEditingContext.Services {
			get { throw new NotImplementedException(); }
		}
		IModelItem IEditingContext.CreateItem(DXTypeIdentifier typeIdentifier, bool useDefaultInitializer) {
			throw new NotImplementedException();
		}
		IModelItem IEditingContext.CreateItem(Type type, bool useDefaultInitializer) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class XamlModelItemCollection : IModelItemCollection {
		readonly XamlEditingContext context;
		List<XamlModelItem> items = new List<XamlModelItem>();
		public XamlModelItemCollection(XamlEditingContext context) {
			this.context = context;
		}
		void IModelItemCollection.Add(IModelItem value) {
			items.Add((XamlModelItem)value);
		}
		IEnumerator<IModelItem> IEnumerable<IModelItem>.GetEnumerator() {
			return items.Cast<IModelItem>().GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}
		#region not implemented
		IModelItem IModelItemCollection.Add(object value) {
			throw new NotImplementedException();
		}
		void IModelItemCollection.Clear() {
			throw new NotImplementedException();
		}
		int IModelItemCollection.IndexOf(IModelItem item) {
			throw new NotImplementedException();
		}
		void IModelItemCollection.Insert(int index, IModelItem valueItem) {
			throw new NotImplementedException();
		}
		void IModelItemCollection.Insert(int index, object value) {
			throw new NotImplementedException();
		}
		bool IModelItemCollection.Remove(object item) {
			throw new NotImplementedException();
		}
		bool IModelItemCollection.Remove(IModelItem item) {
			throw new NotImplementedException();
		}
		void IModelItemCollection.RemoveAt(int index) {
			throw new NotImplementedException();
		}
		IModelItem IModelItemCollection.this[int index] {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		#endregion
	}
	public class ExplicitTextXamlModelItem : XamlModelItem {
		readonly string text;
		public ExplicitTextXamlModelItem(XamlEditingContext context, Type type, string text) : base(context, type) {
			this.text = text;
		}
		protected override string GetTags() {
			return text;
		}
	}
	public class XamlModelItem : IModelItem {
		readonly XamlEditingContext context;
		readonly Type type;
		public Type Type { get { return type; } }
		readonly XamlModelPropertyCollection properties;
		public XamlModelItem(XamlEditingContext context, Type type) {
			this.context = context;
			this.type = type;
			properties = new XamlModelPropertyCollection(this);
		}
		public string GetXaml() {
			string tags = GetTags();
			if(tags.Length == 0)
				return string.Format("<{0} {1}/>", context.GetTypeXaml(type), GetAttributes());
			else
				return string.Format("<{0} {1}>\r\n{2}\r\n</{0}>", context.GetTypeXaml(type), GetAttributes(), tags);
		}
		string GetAttributes() {
			StringBuilder builder = new StringBuilder();
			foreach(XamlModelProperty item in GetAssignedProperties(false)) {
				builder.Append(item.GetXaml());
			}
			return builder.ToString();
		}
		protected virtual string GetTags() {
			StringBuilder builder = new StringBuilder();
			foreach(XamlModelProperty item in GetAssignedProperties(true)) {
				if(builder.Length > 0)
					builder.AppendLine();
				builder.Append(item.GetXaml());
			}
			return builder.ToString();
		}
		IEnumerable<XamlModelProperty> GetAssignedProperties(bool isTag) {
			return properties.Cast<XamlModelProperty>().Where(x => x.IsSet && x.IsTag == isTag);
		}
		IModelPropertyCollection IModelItem.Properties { get { return properties; } }
		IEditingContext IModelItem.Context { get { return context; } }
		public XamlEditingContext Context { get { return context; } }
		Type IModelItem.ItemType { get { return type; } }
		public Type ItemType { get { return type; } }
		#region not implemented
		IModelEditingScope IModelItem.BeginEdit(string description) {
			throw new NotImplementedException();
		}
		IEnumerable<object> IModelItem.GetAttributes(Type attributeType) {
			throw new NotImplementedException();
		}
		object IModelItem.GetCurrentValue() {
			throw new NotImplementedException();
		}
		string IModelItem.Name {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		IModelItem IModelItem.Parent {
			get { throw new NotImplementedException(); }
		}
		IModelSubscribedEvent IModelItem.SubscribeToPropertyChanged(EventHandler handler) { throw new NotImplementedException(); }
		void IModelItem.UnsubscribeFromPropertyChanged(IModelSubscribedEvent e) { throw new NotImplementedException(); }
		IModelItem IModelItem.Root {
			get { throw new NotImplementedException(); }
		}
		IViewItem IModelItem.View {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
	public class XamlModelPropertyCollection : IModelPropertyCollection {
		readonly XamlModelItem item;
		public XamlModelItem Item { get { return item; } }
		readonly List<XamlModelProperty> properties = new List<XamlModelProperty>();
		public XamlModelPropertyCollection(XamlModelItem item) {
			this.item = item;
		}
		IModelProperty IModelPropertyCollection.this[string propertyName] {
			get {
				IModelProperty result = Find(item.ItemType, propertyName);
				if(result == null)
					throw new ArgumentException(propertyName, "propertyName");
				return result;
			}
		}
		IModelProperty IModelPropertyCollection.Find(Type propertyType, string propertyName) {
			return Find(propertyType, propertyName);
		}
		IEnumerator<IModelProperty> IEnumerable<IModelProperty>.GetEnumerator() {
			return properties.Cast<IModelProperty>().GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return properties.GetEnumerator();
		}
		IModelProperty Find(Type propertyType, string propertyName) {
			IModelProperty existing = properties.Find(x => ((IModelProperty)x).Name == propertyName);
			if(existing != null)
				return existing;
			PropertyDescriptor property = TypeDescriptor.GetProperties(item.Type)[propertyName];
			if(property == null)
				return null;
			XamlModelProperty modelProperty = new XamlModelProperty(this, property);
			properties.Add(modelProperty);
			return modelProperty;
		}
		#region not imlemented
		IModelProperty IModelPropertyCollection.Find(string propertyName) {
			return Find(null, propertyName);
		}
		IModelProperty IModelPropertyCollection.this[DXPropertyIdentifier propertyName] {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
	public class XamlModelProperty : IModelProperty {
		static object GetDefaultValue(PropertyDescriptor property, Type itemType) {
			DependencyPropertyDescriptor dProperty = DependencyPropertyDescriptor.FromProperty(property);
			if(dProperty != null)
				return dProperty.DependencyProperty.GetMetadata(itemType).DefaultValue;
			return null;
		}
		readonly PropertyDescriptor property;
		readonly XamlModelPropertyCollection ownerCollection;
		readonly XamlModelItemCollection collection;
		object value;
		public object Value { get { return value ; } private set { this.value = value; } }
		bool isSet;
		public bool IsTag { get { return Value is XamlModelItem || IsCollection; } }
		public XamlModelProperty(XamlModelPropertyCollection ownerCollection, PropertyDescriptor property) {
			this.property = property;
			this.ownerCollection = ownerCollection;
			if(typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
				collection = new XamlModelItemCollection(ownerCollection.Item.Context);
			Value = GetDefaultValue(property, ownerCollection.Item.ItemType);
		}
		IModelItem IModelProperty.SetValue(object value) {
			isSet = true;
			Value = value;
			return null;
		}
		bool IModelProperty.IsSet { get { return IsSet; } }
		public bool IsSet { get { return isSet || IsCollection; } }
		bool IsCollection { get { return (collection != null && collection.Any()); } }
		void IModelProperty.ClearValue() {
			isSet = false;
			Value = null;
		}
		string IModelProperty.Name {
			get { return property.Name; }
		}
		object IModelProperty.ComputedValue {
			get { return Value; }
		}
		public string GetXaml() {
			if(IsTag) {
				string xaml = GetValueXaml();
				ContentPropertyAttribute contentPropertyAttribute = ownerCollection.Item.Type.GetCustomAttributes(typeof(ContentPropertyAttribute), true).FirstOrDefault() as ContentPropertyAttribute;
				if(contentPropertyAttribute == null || contentPropertyAttribute.Name != property.Name) {
					string tagName = string.Format("{0}.{1}", ownerCollection.Item.Context.GetTypeXaml(ownerCollection.Item.Type), property.Name);
					xaml = string.Format("<{0}>\r\n", tagName) + xaml + string.Format("\r\n</{0}>", tagName);
				}
				return xaml;
			} else {
				return string.Format("{0}=\"{1}\" ", property.Name, ConvertPropertyValueToString(Value));
			}
		}
		string GetValueXaml() {
			if(IsCollection) {
				return ownerCollection.Item.Context.GetXamlInfo(collection).Xaml;
			}
			return ((XamlModelItem)Value).GetXaml();
		}
		string ConvertPropertyValueToString(object value) {
			Binding binding = value as Binding;
			if(binding != null) {
				StringBuilder bindingString = new StringBuilder("{Binding ");
				bindingString.Append(binding.Path.Path);
				if(binding.UpdateSourceTrigger != UpdateSourceTrigger.Default) {
					bindingString.Append(", UpdateSourceTrigger=");
					bindingString.Append(binding.UpdateSourceTrigger);
				}
				if(binding.ValidatesOnDataErrors)
					bindingString.Append(", ValidatesOnDataErrors=True");
				if(binding.NotifyOnSourceUpdated)
					bindingString.Append(", NotifyOnSourceUpdated=True");
				bindingString.Append("}");
				return bindingString.ToString();
			}
			Type type = value as Type;
			if(type != null)
				return ownerCollection.Item.Context.GetTypeXaml(type);
			return Convert.ToString(value, CultureInfo.InvariantCulture);
		}
		IModelItemCollection IModelProperty.Collection {
			get { return collection; }
		}
		Type IModelProperty.PropertyType {
			get { return property.PropertyType; }
		}
		#region not implemented
		IModelItemDictionary IModelProperty.Dictionary {
			get { throw new NotImplementedException(); }
		}
		bool IModelProperty.IsReadOnly {
			get { throw new NotImplementedException(); }
		}
		IModelItem IModelProperty.Parent {
			get { throw new NotImplementedException(); }
		}
		IModelItem IModelProperty.Value {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
	static class XamlTypes {
		public static readonly Lazy<Type> LookUpEditType;
		public static readonly Lazy<Type> GridControlType;
		public static readonly Lazy<Type> LayoutItemType;
		public static readonly Lazy<Type> LayoutGroupType;
		public static readonly Lazy<Type> LayoutControlType;
		public static readonly Lazy<Type> ScaffoldingBandsGeneratorType;
		static XamlTypes() {
			GridControlType = new Lazy<Type>(() => DXAssemblyHelper.GetTypeFromAssembly("GridControl", XmlNamespaceConstants.GridNamespace, AssemblyInfo.SRAssemblyXpfGrid));
			LookUpEditType = new Lazy<Type>(() => DXAssemblyHelper.GetTypeFromAssembly("LookUpEdit", XmlNamespaceConstants.GridLookUpNamespace, AssemblyInfo.SRAssemblyXpfGrid));
			LayoutItemType = new Lazy<Type>(() => DXAssemblyHelper.GetTypeFromAssembly("LayoutItem", XmlNamespaceConstants.LayoutControlNamespace, AssemblyInfo.SRAssemblyXpfLayoutControl));
			LayoutGroupType = new Lazy<Type>(() => DXAssemblyHelper.GetTypeFromAssembly("LayoutGroup", XmlNamespaceConstants.LayoutControlNamespace, AssemblyInfo.SRAssemblyXpfLayoutControl));
			LayoutControlType = new Lazy<Type>(() => DXAssemblyHelper.GetTypeFromAssembly("LayoutControl", XmlNamespaceConstants.LayoutControlNamespace, AssemblyInfo.SRAssemblyXpfLayoutControl));
			ScaffoldingBandsGeneratorType = new Lazy<Type>(() => DXAssemblyHelper.GetTypeFromAssembly("ScaffoldingBandsGenerator", XmlNamespaceConstants.GridNamespace + ".Native", AssemblyInfo.SRAssemblyXpfGrid));
		}
	}
	public class ModelGridColumnGenerator {
		public static void GenerateColumns(IEnumerable<IEdmPropertyInfo> properties, IModelItem gridControl, TypeNamePropertyPair[] pairs) {
			var rootGroupInfo = new LayoutGroupInfo(null, GroupView.GroupBox, Orientation.Horizontal);
			var groupGenerator = (IGroupGenerator)Activator.CreateInstance(XamlTypes.ScaffoldingBandsGeneratorType.Value, gridControl);
			EditorsSource.GenerateEditors(rootGroupInfo, properties, groupGenerator, null, GenerateEditorOptions.ForGridScaffolding(pairs), false);
		}
	}
	public class PropertyEditorGroupInfo {
		public string Name { get; set; }
		public GroupView View { get; set; }
		public Orientation Orientation { get; set; }
		public IEnumerable<PropertyEditorInfo> Items { get; set; }
		public IEnumerable<PropertyEditorGroupInfo> Groups { get; set; }
	}
	public class PropertyLookupEditorInfo {
		public string ItemsSource { get; set; }
		public string DisplayMemberPropertyName { get; set; }
		public ForeignKeyInfo ForeignKeyInfo { get; set; }
		public string BindingPath { get; set; }
	}
	public class PropertyEditorInfo {
		public IEdmPropertyInfo Property { get; set; }
		public string Label { get; set; }
		public bool IsReadonly { get; set; }
		public PropertyLookupEditorInfo Lookup { get; set; }
		public bool IsImage { get; set; }
		public bool IsLookup { get { return Lookup != null; } }
	}
	class FluffyEditorsSource {
		class DumbLayoutItemGenerator : EditorsGeneratorBase {
			List<PropertyEditorInfo> infos = new List<PropertyEditorInfo>();
			PropertyEditorGroupInfo group;
			public DumbLayoutItemGenerator(List<PropertyEditorInfo> infos, PropertyEditorGroupInfo group) {
				this.infos = infos;
				this.group = group;
			}
			protected override EditorsGeneratorMode Mode { get { return EditorsGeneratorMode.Edit; } }
			protected override Type GetLookUpEditType() { throw new NotImplementedException(); }
			protected override void GenerateEditor(IEdmPropertyInfo property, Type editType, Initializer initializer) {
				AddInfo(new PropertyEditorInfo { Property = property });
			}
			public override void Image(IEdmPropertyInfo property, bool readOnly) {
				AddInfo(new PropertyEditorInfo { Property = property, IsImage = true, IsReadonly = readOnly });
			}
			public override void Object(IEdmPropertyInfo property) {
				AddInfo(new PropertyEditorInfo { Property = property });
			}
			public override void LookUp(IEdmPropertyInfo property, string itemsSource, string displayMember, ForeignKeyInfo foreignKeyInfo) {
				var lookup = new PropertyLookupEditorInfo {
					ForeignKeyInfo = foreignKeyInfo,
					DisplayMemberPropertyName = displayMember,
					ItemsSource = itemsSource,
					BindingPath = property.Name + ((string.IsNullOrEmpty(displayMember) ? null : "." + displayMember))
				};
				var info = new PropertyEditorInfo {
					Property = property,
					Lookup = lookup
				};
				if (string.IsNullOrEmpty(itemsSource)) {
					info.IsReadonly = true;
					AddInfo(info);
				} else if (foreignKeyInfo != null) {
					AddInfo(info);
				}
			}
			private void AddInfo(PropertyEditorInfo info) {
				info.Label = SplitStringHelper.SplitPascalCaseString(info.Property.Name);
				AttributesApplier.ApplyBaseAttributes(info.Property,
				   setDisplayMember: null,
				   setDisplayName: x => info.Label = x,
				   setDisplayShortName: x => info.Label = x,
				   setDescription: null,
				   setReadOnly: () => info.IsReadonly = true,
				   setEditable: null,
				   setInvisible: null,
				   setHidden: null,
				   setRequired: null);
				infos.Add(info);
			}
			public override EditorsGeneratorTarget Target { get { return EditorsGeneratorTarget.Unknown; } }
			public override void GenerateEditorFromResources(IEdmPropertyInfo property, object resourceKey, Initializer initializer) {
				GenerateEditor(property, null, initializer);
			}
		}
		class DumbGroupGenerator : IGroupGenerator {
			EditorsGeneratorBase generator;
			public List<PropertyEditorInfo> infos = new List<PropertyEditorInfo>();
			public PropertyEditorGroupInfo groupInfo = new PropertyEditorGroupInfo();
			public List<DumbGroupGenerator> nested = new List<DumbGroupGenerator>();
			public DumbGroupGenerator() {
				generator = new DumbLayoutItemGenerator(infos, groupInfo);
			}
			public EditorsGeneratorBase EditorsGenerator { get { return generator; } }
			public void OnAfterGenerateContent() { }
			public void ApplyGroupInfo(string name, GroupView view, Orientation orientation) {
				groupInfo.Name = name;
				groupInfo.View = view;
				groupInfo.Orientation = orientation;
			}
			public IGroupGenerator CreateNestedGroup(string name, GroupView view, Orientation orientation) {
				var group = new DumbGroupGenerator();
				nested.Add(group);
				return group;
			}
		}
		public static PropertyEditorGroupInfo GenerateEditors(
			IEnumerable<IEdmPropertyInfo> properties,
			GenerateEditorOptions options,
			Func<IEdmPropertyInfo, ForeignKeyInfo> getForegnKeyProperty)
		{
			options = options ?? GenerateEditorOptions.ForLayoutScaffolding(null);
			var groupGenerator = new DumbGroupGenerator();
			var rootGroupInfo = new LayoutGroupInfo(null, GroupView.Group, Orientation.Vertical);
			EditorsSource.GenerateEditors(rootGroupInfo, properties, groupGenerator, null, options, true, true, getForegnKeyProperty);
			return ConvertToGroupInfo(groupGenerator);
		}
		static PropertyEditorGroupInfo ConvertToGroupInfo(DumbGroupGenerator groupGenerator) {
			var groupInfo = groupGenerator.groupInfo;
			groupInfo.Items = groupGenerator.infos;
			groupInfo.Groups = groupGenerator.nested.Select(ConvertToGroupInfo);
			return groupInfo;
		}
	}
}

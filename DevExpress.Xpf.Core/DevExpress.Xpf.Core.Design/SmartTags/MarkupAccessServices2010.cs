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

extern alias Platform;
using Microsoft.Windows.Design.Model;
using System.ComponentModel;
using Microsoft.Windows.Design.Services;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using Microsoft.Windows.Design.Metadata;
using System.Reflection;
using DevExpress.Xpf.Core.Design.Wizards.Utils;
using System.Collections.Specialized;
using DevExpress.Design.SmartTags;
using DevExpress.Utils.Design;
using VSLangProj;
using System.Diagnostics;
using System.Text;
using System.Windows.Data;
using Guard = Platform::DevExpress.Utils.Guard;
using ISourceReader = DevExpress.Design.SmartTags.ISourceReader;
#if !SL
using DevExpress.CodeParser.Xaml;
using DevExpress.CodeParser;
using DevExpress.CodeParser.Xml;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Utils;
using System.Windows.Markup;
using DevExpress.Design;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
#endif
namespace DevExpress.Xpf.Core.Design {
	public abstract class XpfVirtualDocumentNode : RuntimeBase<IVirtualDocumentNode, object>, IVirtualDocumentNode {
		public static IVirtualDocumentNode FromDocumentNode(object value) {
			if(value == null) return null;
			switch(value.GetType().Name) {
				case "XamlItem": return new XpfXamlItem(value);
				case "XamlMarkupExtensionItem": return new XpfXamlMarkupExtensionItem(value);
				case "ChangedModelDocumentItem": return new XpfChangedModelDocumentItem(value);
				case "ChangedModelDocumentProperty": return new XpfChangedModelDocumentProperty(value);
				case "XamlMarkupExtensionProperty": return new XpfXamlMarkupExtensionProperty(value);
				case "XamlProperty": return new XpfXamlProperty(value);
				default: DebugHelper.Assert(false); return null;
			}
		}
		public static object ToDocumentNode(IVirtualDocumentNode documentNode) {
			return documentNode == null ? null : Guard.ArgumentMatchType<XpfVirtualDocumentNode>(documentNode, "documentNode").Value;
		}
		protected XpfVirtualDocumentNode(object value) : base(value) { }
	}
	public class XpfMarkupLocation : RuntimeBase<IMarkupLocation, object>, IMarkupLocation {
		public static IMarkupLocation FromMarkupLocation(object value) {
			return value == null ? null : new XpfMarkupLocation(value);
		}
		static Type iMarkupLocationType;
		static PropertyInfo offsetProperty;
		static PropertyInfo lengthProperty;
		protected XpfMarkupLocation(object value) : base(value) { }
		public int Offset { get { return (int)OffsetProperty.GetValue(Value, null); } }
		public int Length { get { return (int)LengthProperty.GetValue(Value, null); } }
		Type IMarkupLocationType {
			get {
				if(iMarkupLocationType == null)
					iMarkupLocationType = Value.GetType().GetInterface("IMarkupLocation");
				return iMarkupLocationType;
			}
		}
		PropertyInfo OffsetProperty {
			get {
				if(offsetProperty == null)
					offsetProperty = IMarkupLocationType.GetProperty("Offset");
				return offsetProperty;
			}
		}
		PropertyInfo LengthProperty {
			get {
				if(lengthProperty == null)
					lengthProperty = IMarkupLocationType.GetProperty("Length");
				return lengthProperty;
			}
		}
	}
	public class XpfSourceReader : RuntimeBase<ISourceReader, object>, ISourceReader {
		public static ISourceReader FromSourceReader(object value) {
			return value == null ? null : new XpfSourceReader(value);
		}
		static Type iSourceReaderType;
		static MethodInfo readMethod;
		protected XpfSourceReader(object value) : base(value) { }
		public string Read(long location, int length) { return (string)ReadMethod.Invoke(Value, new object[] { location, length }); }
		Type ISourceReaderType {
			get {
				if(iSourceReaderType == null)
					iSourceReaderType = Value.GetType().GetInterface("ISourceReader");
				return iSourceReaderType;
			}
		}
		MethodInfo ReadMethod {
			get {
				if(readMethod == null)
					readMethod = ISourceReaderType.GetMethod("Read");
				return readMethod;
			}
		}
	}
	public class XpfMarkupSourceProvider : RuntimeBase<IMarkupSourceProvider, object>, IMarkupSourceProvider {
		public static IMarkupSourceProvider FromMarkupSourceProvider(object value) {
			return value == null ? null : new XpfMarkupSourceProvider(value);
		}
		static Type iMarkupSourceProviderType;
		static MethodInfo createReaderMethod;
		protected XpfMarkupSourceProvider(object value) : base(value) { }
		public ISourceReader CreateReader() { return XpfSourceReader.FromSourceReader(CreateReaderMethod.Invoke(Value, null)); }
		Type IMarkupSourceProviderType {
			get {
				if(iMarkupSourceProviderType == null)
					iMarkupSourceProviderType = Value.GetType().GetInterface("IMarkupSourceProvider");
				return iMarkupSourceProviderType;
			}
		}
		MethodInfo CreateReaderMethod {
			get {
				if(createReaderMethod == null)
					createReaderMethod = IMarkupSourceProviderType.GetMethod("CreateReader");
				return createReaderMethod;
			}
		}
	}
	public class XpfXamlSourceDocument : RuntimeBase<IXamlSourceDocument, IDisposable>, IXamlSourceDocument {
		public static IXamlSourceDocument FromXamlSourceDocument(IDisposable value) {
			return value == null ? null : new XpfXamlSourceDocument(value);
		}
		static Type xamlSourceDocumentType;
		static PropertyInfo providerProperty;
		static MethodInfo resolveTypeMethod;
		protected XpfXamlSourceDocument(IDisposable value) : base(value) { }
		public IMarkupSourceProvider Provider { get { return XpfMarkupSourceProvider.FromMarkupSourceProvider(ProviderProperty.GetValue(Value, null)); } }
		public IVS2010TypeMetadata ResolveType(IXamlElement element, string name) {
			return VS2010TypeMetadata.Get(ResolveTypeMethod.Invoke(Value, new object[] { XpfXamlObjectElement.ToXamlElement(element), name }));
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				Value.Dispose();
		}
		Type XamlSourceDocumentType {
			get {
				if(xamlSourceDocumentType == null)
					xamlSourceDocumentType = Value.GetType();
				DebugHelper.Assert(xamlSourceDocumentType.Name == "XamlSourceDocument");
				return xamlSourceDocumentType;
			}
		}
		PropertyInfo ProviderProperty {
			get {
				if(providerProperty == null)
					providerProperty = XamlSourceDocumentType.GetProperty("Provider", BindingFlags.Instance | BindingFlags.NonPublic);
				return providerProperty;
			}
		}
		MethodInfo ResolveTypeMethod {
			get {
				if(resolveTypeMethod == null)
					resolveTypeMethod = XamlSourceDocumentType.GetMethod("ResolveType");
				return resolveTypeMethod;
			}
		}
	}
	public static class XpfMarkupLocationProvider {
		static Type iMarkupLocationProviderType;
		static MethodInfo getLocationMethod;
		public static MarkupLocation GetLocation(object value) {
			return new MarkupLocation(XpfMarkupLocation.FromMarkupLocation(GetGetLocationMethod(value).Invoke(value, new object[] { })));
		}
		static Type GetIMarkupLocationProviderType(object value) {
			if(iMarkupLocationProviderType == null)
				iMarkupLocationProviderType = value.GetType().GetInterface("IMarkupLocationProvider");
			return iMarkupLocationProviderType;
		}
		static MethodInfo GetGetLocationMethod(object value) {
			if(getLocationMethod == null)
				getLocationMethod = GetIMarkupLocationProviderType(value).GetMethod("GetLocation");
			return getLocationMethod;
		}
	}
	public static class XpfSourceTextValue {
		static Type iSourceTextValueType;
		static PropertyInfo sourceProperty;
		public static string GetSource(object value) {
			return (string)GetSourceProperty(value).GetValue(value, null);
		}
		static Type GetISourceTextValueType(object value) {
			if(iSourceTextValueType == null)
				iSourceTextValueType = value.GetType().GetInterface("ISourceTextValue");
			return iSourceTextValueType;
		}
		static PropertyInfo GetSourceProperty(object value) {
			if(sourceProperty == null)
				sourceProperty = GetISourceTextValueType(value).GetProperty("Source");
			return sourceProperty;
		}
	}
	public abstract class XpfVirtualDocumentItem : XpfVirtualDocumentNode, IVirtualDocumentItem {
		protected XpfVirtualDocumentItem(object value) : base(value) { }
		static Type documentItemType;
		static PropertyInfo itemTypeProperty;
		static PropertyInfo propertiesProperty;
		public IVS2010TypeMetadata ItemType { get { return VS2010TypeMetadata.Get(ItemTypeProperty.GetValue(Value, null)); } }
		public IEnumerable<IVirtualDocumentProperty> Properties {
			get {
				IEnumerable items = (IEnumerable)PropertiesProperty.GetValue(Value, null);
				return items == null ? null : GetProperties(items);
			}
		}
		IEnumerable<IVirtualDocumentProperty> GetProperties(IEnumerable properties) {
			foreach(object property in properties)
				yield return (IVirtualDocumentProperty)XpfVirtualDocumentProperty.FromDocumentNode(property);
		}
		Type DocumentItemType {
			get {
				if(documentItemType == null)
					documentItemType = Value.GetType().BaseTypes().Where(t => t.Name == "DocumentItem").First();
				return documentItemType;
			}
		}
		PropertyInfo ItemTypeProperty {
			get {
				if(itemTypeProperty == null)
					itemTypeProperty = DocumentItemType.GetProperty("ItemType");
				return itemTypeProperty;
			}
		}
		PropertyInfo PropertiesProperty {
			get {
				if(propertiesProperty == null)
					propertiesProperty = DocumentItemType.GetProperty("Properties");
				return propertiesProperty;
			}
		}
	}
	public abstract class XpfVirtualDocumentProperty : XpfVirtualDocumentNode, IVirtualDocumentProperty {
		protected XpfVirtualDocumentProperty(object value) : base(value) { }
		static Type documentPropertyType;
		static PropertyInfo propertyTypeProperty;
		static PropertyInfo valueProperty;
		static PropertyInfo itemsProperty;
		static PropertyInfo nameProperty;
		public string Name { get { return (string)NameProperty.GetValue(Value, null); } }
		public IVS2010TypeMetadata PropertyType { get { return VS2010TypeMetadata.Get(PropertyTypeProperty.GetValue(Value, null)); } }
		public IVirtualDocumentValue PropertyValue { get { return XpfVirtualDocumentValue.FromDocumentValue(ValueProperty.GetValue(Value, null)); } }
		public IEnumerable<IVirtualDocumentItem> Items {
			get {
				IEnumerable items = (IEnumerable)ItemsProperty.GetValue(Value, null);
				return items == null ? null : GetItems(items);
			}
		}
		IEnumerable<IVirtualDocumentItem> GetItems(IEnumerable items) {
			foreach(object item in items)
				yield return (IVirtualDocumentItem)XpfVirtualDocumentItem.FromDocumentNode(item);
		}
		Type DocumentPropertyType {
			get {
				if(documentPropertyType == null)
					documentPropertyType = Value.GetType().BaseTypes().Where(t => t.Name == "DocumentProperty").First();
				return documentPropertyType;
			}
		}
		PropertyInfo PropertyTypeProperty {
			get {
				if(propertyTypeProperty == null)
					propertyTypeProperty = DocumentPropertyType.GetProperty("PropertyType");
				return propertyTypeProperty;
			}
		}
		PropertyInfo ValueProperty {
			get {
				if(valueProperty == null)
					valueProperty = DocumentPropertyType.GetProperty("Value");
				return valueProperty;
			}
		}
		PropertyInfo ItemsProperty {
			get {
				if(itemsProperty == null)
					itemsProperty = DocumentPropertyType.GetProperty("Items");
				return itemsProperty;
			}
		}
		PropertyInfo NameProperty {
			get {
				if(nameProperty == null)
					nameProperty = DocumentPropertyType.GetProperty("Name");
				return nameProperty;
			}
		}
	}
	public class XpfXamlMarkupExtensionItem : XpfVirtualDocumentItem, IXamlMarkupExtensionItem {
		internal XpfXamlMarkupExtensionItem(object value) : base(value) { }
		public MarkupLocation GetLocation() { return XpfMarkupLocationProvider.GetLocation(Value); }
	}
	public class XpfXamlMarkupExtensionProperty : XpfVirtualDocumentProperty, IXamlMarkupExtensionProperty {
		static Type xamlMarkupExtensionPropertyType;
		static PropertyInfo sourceProperty;
		internal XpfXamlMarkupExtensionProperty(object value) : base(value) { }
		public string Source { get { return (string)SourceProperty.GetValue(Value, null); } }
		public MarkupLocation GetLocation() { return XpfMarkupLocationProvider.GetLocation(Value); }
		Type XamlMarkupExtensionPropertyType {
			get {
				if(xamlMarkupExtensionPropertyType == null)
					xamlMarkupExtensionPropertyType = Value.GetType();
				DebugHelper.Assert(xamlMarkupExtensionPropertyType.Name == "XamlMarkupExtensionProperty");
				return xamlMarkupExtensionPropertyType;
			}
		}
		PropertyInfo SourceProperty {
			get {
				if(sourceProperty == null)
					sourceProperty = XamlMarkupExtensionPropertyType.GetProperty("Source");
				return sourceProperty;
			}
		}
	}
	public class XpfChangedModelDocumentItem : XpfVirtualDocumentItem, IChangedModelDocumentItem {
		internal XpfChangedModelDocumentItem(object value) : base(value) { }
	}
	public class XpfChangedModelDocumentProperty : XpfVirtualDocumentProperty, IChangedModelDocumentProperty {
		internal XpfChangedModelDocumentProperty(object value) : base(value) { }
	}
	public class XpfXamlItem : XpfVirtualDocumentItem, IXamlItem {
		static Type xamlItemType;
		static PropertyInfo documentProperty;
		static PropertyInfo elementProperty;
		internal XpfXamlItem(object value) : base(value) { }
		public MarkupLocation GetLocation() { return XpfMarkupLocationProvider.GetLocation(Value); }
		public IXamlSourceDocument Document { get { return XpfXamlSourceDocument.FromXamlSourceDocument((IDisposable)DocumentProperty.GetValue(Value, null)); } }
		public IXamlObjectElement Element { get { return XpfXamlObjectElement.FromXamlObjectElement(ElementProperty.GetValue(Value, null)); } }
		Type XamlItemType {
			get {
				if(xamlItemType == null)
					xamlItemType = Value.GetType();
				DebugHelper.Assert(xamlItemType.Name == "XamlItem");
				return xamlItemType;
			}
		}
		PropertyInfo DocumentProperty {
			get {
				if(documentProperty == null)
					documentProperty = XamlItemType.GetProperty("Document", BindingFlags.NonPublic | BindingFlags.Instance);
				return documentProperty;
			}
		}
		PropertyInfo ElementProperty {
			get {
				if(elementProperty == null)
					elementProperty = XamlItemType.GetProperty("Element", BindingFlags.NonPublic | BindingFlags.Instance);
				return elementProperty;
			}
		}
	}
	public class XpfXamlProperty : XpfVirtualDocumentProperty, IXamlProperty {
		static Type xamlPropertyType;
		internal XpfXamlProperty(object value) : base(value) { }
		public MarkupLocation GetLocation() { return XpfMarkupLocationProvider.GetLocation(Value); }
		public string Source { get { return XpfSourceTextValue.GetSource(Value); } }
		Type XamlPropertyType {
			get {
				if(xamlPropertyType == null)
					xamlPropertyType = Value.GetType();
				DebugHelper.Assert(xamlPropertyType.Name == "XamlProperty");
				return xamlPropertyType;
			}
		}
	}
	public class XpfXamlObjectElement : RuntimeBase<IXamlObjectElement, object>, IXamlObjectElement {
		public static IXamlObjectElement FromXamlObjectElement(object value) {
			return value == null ? null : new XpfXamlObjectElement(value);
		}
		public static object ToXamlElement(IXamlElement xamlObjectElement) {
			return xamlObjectElement == null ? null : Guard.ArgumentMatchType<XpfXamlObjectElement>(xamlObjectElement, "xamlObjectElement").Value;
		}
		protected XpfXamlObjectElement(object value) : base(value) { }
	}
	public class XpfVirtualModelHost : RuntimeBase<IVirtualModelHost, object>, IVirtualModelHost {
		public static IVirtualModelHost FromVirtualModelHost(object value) {
			return value == null ? null : new XpfVirtualModelHost(value);
		}
		static Type iVirtualModelHostType;
		static MethodInfo findNodeMethod;
		static PropertyInfo rootProperty;
		static PropertyInfo metadataProperty;
		protected XpfVirtualModelHost(object value) : base(value) { }
		public IVirtualModelItem Root { get { return XpfVirtualModelItem.FromVirtualModelItem(RootProperty.GetValue(Value, null)); } }
		public IVirtualDocumentNode FindNode(object identity) { return XpfVirtualDocumentNode.FromDocumentNode(FindNodeMethod.Invoke(Value, new object[] { identity })); }
		public IVS2010MetadataContext Metadata { get { return VS2010MetadataContext.Get(MetadataProperty.GetValue(Value, null)); } }
		Type IVirtualModelHostType {
			get {
				if(iVirtualModelHostType == null)
					iVirtualModelHostType = Value.GetType().GetInterface("IVirtualModelHost");
				return iVirtualModelHostType;
			}
		}
		MethodInfo FindNodeMethod {
			get {
				if(findNodeMethod == null)
					findNodeMethod = IVirtualModelHostType.GetMethod("FindNode");
				return findNodeMethod;
			}
		}
		PropertyInfo RootProperty {
			get {
				if(rootProperty == null)
					rootProperty = IVirtualModelHostType.GetProperty("Root");
				return rootProperty;
			}
		}
		PropertyInfo MetadataProperty {
			get {
				if(metadataProperty == null)
					metadataProperty = IVirtualModelHostType.GetProperty("Metadata");
				return metadataProperty;
			}
		}
	}
	public class XpfVirtualModelItem : RuntimeBase<IVirtualModelItem, object>, IVirtualModelItem {
		public static IVirtualModelItem FromVirtualModelItem(object value) {
			return value == null ? null : new XpfVirtualModelItem(value);
		}
		static Type iVirtualModelItemType;
		static PropertyInfo hostProperty;
		static PropertyInfo identityProperty;
		protected XpfVirtualModelItem(object value) : base(value) { }
		public IVirtualModelHost Host { get { return XpfVirtualModelHost.FromVirtualModelHost(HostProperty.GetValue(Value, null)); } }
		public object Identity { get { return IdentityProperty.GetValue(Value, null); } }
		Type IVirtualModelItemType {
			get {
				if(iVirtualModelItemType == null)
					iVirtualModelItemType = Value.GetType().GetInterface("IVirtualModelItem");
				return iVirtualModelItemType;
			}
		}
		PropertyInfo HostProperty {
			get {
				if(hostProperty == null)
					hostProperty = IVirtualModelItemType.GetProperty("Host");
				return hostProperty;
			}
		}
		PropertyInfo IdentityProperty {
			get {
				if(identityProperty == null)
					identityProperty = IVirtualModelItemType.GetProperty("Identity");
				return identityProperty;
			}
		}
	}
	public class XpfVirtualDocumentValue : RuntimeBase<IVirtualDocumentValue, object>, IVirtualDocumentValue {
		public static IVirtualDocumentValue FromDocumentValue(object value) {
			return value == null ? null : new XpfVirtualDocumentValue(value);
		}
		static Type documentValueType;
		static PropertyInfo valueProperty;
		protected XpfVirtualDocumentValue(object value) : base(value) { }
		public string Source { get { return (string)ValueProperty.GetValue(Value, null); } }
		Type DocumentValueType {
			get {
				if(documentValueType == null)
					documentValueType = Value.GetType();
				DebugHelper.Assert(documentValueType.Name == "DocumentValue");
				return documentValueType;
			}
		}
		PropertyInfo ValueProperty {
			get {
				if(valueProperty == null)
					valueProperty = DocumentValueType.GetProperty("Value");
				return valueProperty;
			}
		}
	}
	public class XpfMarkupAccessService2010 : IMarkupAccessService2010 {
		public static IMarkupAccessService2010 Create(IModelItem modelItem) {
			ModelItem value = XpfModelItem.ToModelItem(modelItem);
			return value.GetType().GetInterface("IVirtualModelItem") == null ? null : new XpfMarkupAccessService2010();
		}
		protected XpfMarkupAccessService2010() { }
		IVirtualModelItem IMarkupAccessService2010.GetModelItem(IModelItem modelItem) {
			return XpfVirtualModelItem.FromVirtualModelItem(XpfModelItem.ToModelItem(modelItem));
		}
	}
	public static class TypeExtensions {
		public static IEnumerable<Type> BaseTypes(this Type type) {
			for(Type baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
				yield return baseType;
		}
	}
}

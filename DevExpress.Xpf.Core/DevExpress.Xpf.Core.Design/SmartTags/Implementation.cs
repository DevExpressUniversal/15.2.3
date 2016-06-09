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
using DevExpress.Design.SmartTags;
using DevExpress.Utils.Design;
using VSLangProj;
using System.Diagnostics;
using System.Text;
using System.Windows.Data;
using Guard = Platform::DevExpress.Utils.Guard;
using ISourceReader = DevExpress.Design.SmartTags.ISourceReader;
using DevExpress.Design;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using Platform::DevExpress.Data.Utils;
using System.IO;
#if !SL
using System.Collections.Specialized;
using DevExpress.CodeParser.Xaml;
using DevExpress.CodeParser;
using DevExpress.CodeParser.Xml;
using System.Windows.Media.Imaging;
using DevExpress.Utils;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Design.SmartTags;
#else
using Platform::System.Collections.Specialized;
#endif
namespace DevExpress.Xpf.Core.Design {
	public abstract class RuntimeBaseCore {
		protected static T Safe<T>(Func<T> value, T ifError) {
			try {
				return value();
			} catch {
				return ifError;
			}
		}
		protected static IEnumerable<object> EnumerateSafe(Func<IEnumerable> source) {
			IEnumerable sourceEnumerable = Safe(source, new object[] { });
			for(IEnumerator e = sourceEnumerable.GetEnumerator(); Safe(() => e.MoveNext(), false); ) {
				yield return e.Current;
			}
		}
	}
	public abstract class RuntimeBase<TInterface, TImplementation> : RuntimeBaseCore where TInterface : class {
		readonly TImplementation value;
		public TImplementation Value { get { return value; } }
		protected RuntimeBase(TImplementation value) {
			this.value = value;
		}
		#region Equality
		public override int GetHashCode() {
			return value.GetHashCode();
		}
		public static bool operator ==(RuntimeBase<TInterface, TImplementation> r1, RuntimeBase<TInterface, TImplementation> r2) {
			bool r1IsNull = (object)r1 == null;
			bool r2IsNull = (object)r2 == null;
			if(r1IsNull && r2IsNull) return true;
			if(r1IsNull || r2IsNull) return false;
			return object.Equals(r1.value, r2.value);
		}
		public static bool operator !=(RuntimeBase<TInterface, TImplementation> r1, RuntimeBase<TInterface, TImplementation> r2) {
			return !(r1 == r2);
		}
		public override bool Equals(object obj) {
			return this == obj as RuntimeBase<TInterface, TImplementation>;
		}
		#endregion
		public override string ToString() { return value.ToString(); }
	}
	public class XpfModelService : RuntimeBase<IModelService, ModelService>, IModelService {
		static FieldInfo modelChangedEventField;
		protected XpfModelService(ModelService value) : base(value) { }
		public static IModelService FromModelService(ModelService value) {
			return value == null ? null : new XpfModelService(value);
		}
		IModelItem IModelService.Root {
			get { return XpfModelItem.FromModelItem(Value.Root); }
		}
		IModelSubscribedEvent IModelService.SubscribeToModelChanged(EventHandler handler) {
			return ModelSubscribedEvent<EventHandler<ModelChangedEventArgs>>.Subscribe((s, a) => handler(s, a), h => Value.ModelChanged += h);
		}
		void IModelService.UnsubscribeFromModelChanged(IModelSubscribedEvent e) {
			ModelSubscribedEvent<EventHandler<ModelChangedEventArgs>>.Unsubscribe(e, h => Value.ModelChanged -= h);
		}
		void IModelService.RaiseModelChanged() {
			Delegate d = (Delegate)ModelChangedEventField.GetValue(Value);
			if(d != null)
				d.DynamicInvoke(Value, new EmptyModelChangedEventArgs());
		}
		FieldInfo ModelChangedEventField {
			get {
				if(modelChangedEventField == null) {
					Type modelServiceType = Value.GetType();
					modelChangedEventField = modelServiceType.GetField("ModelChanged", BindingFlags.Instance | BindingFlags.NonPublic);
				}
				return modelChangedEventField;
			}
		}
	}
	public class XpfEditingContext : RuntimeBase<IEditingContext, EditingContext>, IEditingContext {
		public static IEditingContext FromEditingContext(EditingContext value) {
			return value == null ? null : new XpfEditingContext(value);
		}
		XpfEditingContext(EditingContext value)
			: base(value) {
			Services = new ServiceManagerBase();
			Services.AddInnerProvider(Value.Services);
			Services.Publish<IModelService>(() => XpfModelService.FromModelService(Value.Services.GetService<ModelService>()));
#if !SL
			Services.Publish<IDXTypeDiscoveryService>(() => DXTypeDiscoveryServiceFactory.Create(this));
			Services.Publish<IMarkupAccessService>(CreateMarkupAccessService);
			Services.Publish<IUriContext>(() => new XpfUriContext(this));
			Services.Publish<IEnvDteService>(() => new XpfEnvDteService());
			Services.Publish<ICreateXamlMarkupService>(CreateCreateXamlMarkupService);
#endif
		}
		IModelItem IEditingContext.CreateItem(Type type) {
			return ((IEditingContext)this).CreateItem(type, false);
		}
		IModelItem IEditingContext.CreateItem(Type type, bool useDefaultInitializer) {
			EnsureAssemblyReferenced(type.Assembly);
			CreateOptions createOptions = useDefaultInitializer ? CreateOptions.InitializeDefaults : CreateOptions.None;
			return XpfModelItem.FromModelItem(ModelFactory.CreateItem(Value, type, createOptions));
		}
		IModelItem IEditingContext.CreateStaticMemberItem(Type type, string memberName) {
			EnsureAssemblyReferenced(type.Assembly);
			return XpfModelItem.FromModelItem(ModelFactory.CreateStaticMemberItem(Value, type, memberName));
		}
		IModelItem IEditingContext.CreateItem(DXTypeIdentifier typeIdentifier) {
			return ((IEditingContext)this).CreateItem(typeIdentifier, false);
		}
		IModelItem IEditingContext.CreateItem(DXTypeIdentifier typeIdentifier, bool useDefaultInitializer) {
			CreateOptions createOptions = useDefaultInitializer ? CreateOptions.InitializeDefaults : CreateOptions.None;
			return XpfModelItem.FromModelItem(ModelFactory.CreateItem(Value, XpfModelPropertyColleciton.CreateTypeIdentifier(typeIdentifier), createOptions));
		}
		int GetCurrentVSMajorVersion() {
			return Process.GetCurrentProcess().MainModule.FileVersionInfo.FileMajorPart;
		}
		void EnsureAssemblyReferenced(Assembly assembly) {
#if !SL && ! DEBUGTEST
			using (new MessageFilter()) {
				if (GetCurrentVSMajorVersion() == 10) {
					VSLangProj.VSProject project = (VSLangProj.VSProject)DTEHelper.GetCurrentProject().Object;
					if (!Contains(assembly, project.References)) {
						TryAddReference(assembly, project);
					}
					foreach (AssemblyName name in assembly.GetReferencedAssemblies())
						if (CheckAssemblyName(name))
							EnsureAssemblyReferenced(Assembly.Load(name));
				}
			}
#endif
		}
#if !SL
		IMarkupAccessService CreateMarkupAccessService() {
			IModelService modelService = Services.GetService<IModelService>();
			IModelItem modelServiceRoot = modelService.Root;
			IMarkupAccessService service = XpfMarkupAccessService2012.Create(modelServiceRoot);
			if(service != null) return service;
			return XpfMarkupAccessService2010.Create(modelServiceRoot);
		}
		ICreateXamlMarkupService CreateCreateXamlMarkupService() {
			IMarkupAccessService2010 markupAccessService = Services.GetService<IMarkupAccessService>() as IMarkupAccessService2010;
			return markupAccessService == null ? (ICreateXamlMarkupService)new CreateXamlMarkupService2012() : new CreateXamlMarkupService2010(this, markupAccessService);
		}
		static void TryAddReference(Assembly assembly, VSLangProj.VSProject project) {
			try {
				project.References.Add(assembly.Location);
			} catch { }
		}
		bool CheckAssemblyName(AssemblyName name) {
			if(name.FullName.Contains("DevExpress"))
				return true;
			return false;
		}
		static bool Contains(Assembly assembly, References references) {
			string localAssemblyName = references.ContainingProject.Properties.Item("AssemblyName").Value.ToString();
			if(assembly.GetName().Name == localAssemblyName)
				return true;
			foreach(VSLangProj.Reference reference in references) {
				try {
					if(reference.Name == assembly.GetName().Name)
						return true;
				} catch { }
			}
			return false;
		}
#endif
		public ServiceManagerBase Services { get; private set; }
		IServiceProvider IEditingContext.Services { get { return Services; } }
	}
	public class XpfModelPropertyColleciton : RuntimeBase<IModelPropertyCollection, ModelPropertyCollection>, IModelPropertyCollection {
		public static IModelPropertyCollection FromModelPropertyCollection(ModelPropertyCollection value) {
			return value == null ? null : new XpfModelPropertyColleciton(value);
		}
		static PropertyIdentifier CreatePropertyIdentifier(DXPropertyIdentifier pi) {
			if(pi.DeclaringType != null)
				return new PropertyIdentifier(pi.DeclaringType, pi.Name);
			return new PropertyIdentifier(CreateTypeIdentifier(pi.DeclaringTypeIdentifier), pi.Name);
		}
		internal static TypeIdentifier CreateTypeIdentifier(DXTypeIdentifier ti) {
			return new TypeIdentifier(ti.Name);
		}
		XpfModelPropertyColleciton(ModelPropertyCollection value) : base(value) { }
		IModelProperty IModelPropertyCollection.this[string propertyName] {
			get { return XpfModelProperty.FromModelProperty(Value[propertyName]); }
		}
		IModelProperty IModelPropertyCollection.this[DXPropertyIdentifier propertyIdentifier] {
			get { return XpfModelProperty.FromModelProperty(Value[CreatePropertyIdentifier(propertyIdentifier)]); }
		}
		IModelProperty IModelPropertyCollection.Find(string propertyName) {
			return XpfModelProperty.FromModelProperty(Value.Find(propertyName));
		}
		IModelProperty IModelPropertyCollection.Find(Type propertyType, string propertyName) {
			PropertyIdentifier pi = new PropertyIdentifier(propertyType, propertyName);
			try {
				return XpfModelProperty.FromModelProperty(Value.Find(pi));
			} catch { 
				return null;
			}
		}
		public IEnumerator<IModelProperty> GetEnumerator() {
			foreach(ModelProperty p in Value)
				yield return XpfModelProperty.FromModelProperty(p);
		}
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
	}
	public class XpfModelItemColleciton : RuntimeBase<IModelItemCollection, ModelItemCollection>, IModelItemCollection {
		public static IModelItemCollection FromModelItemCollection(ModelItemCollection value) {
			return value == null ? null : new XpfModelItemColleciton(value);
		}
		XpfModelItemColleciton(ModelItemCollection value) : base(value) { }
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public IEnumerator<IModelItem> GetEnumerator() {
			foreach(ModelItem item in Value)
				yield return XpfModelItem.FromModelItem(item);
		}
		void IModelItemCollection.Add(IModelItem value) {
			ModelItem item = XpfModelItem.ToModelItem(value);
			if(item != null)
				Value.Add(item);
		}
		bool IModelItemCollection.Remove(IModelItem item) {
			return Value.Remove(XpfModelItem.ToModelItem(item));
		}
		public IModelItem Add(object value) {
			return XpfModelItem.FromModelItem(Value.Add(value));
		}
		public void Clear() {
			Value.Clear();
		}
		public int IndexOf(IModelItem item) {
			return Value.IndexOf(XpfModelItem.ToModelItem(item));
		}
		public IModelItem this[int index] {
			get { return XpfModelItem.FromModelItem(Value[index]); }
			set { Value[index] = XpfModelItem.ToModelItem(value); }
		}
		public void Insert(int index, IModelItem valueItem) {
			Insert(index, XpfModelItem.ToModelItem(valueItem));
		}
		public void Insert(int index, object value) {
			Value.Insert(index, value);
		}
		public void RemoveAt(int index) {
			Value.RemoveAt(index);
		}
		public bool Remove(object item) {
			return Value.Remove(item);
		}
	}
	public class XpfModelItemDictionary : RuntimeBase<IModelItemDictionary, ModelItemDictionary>, IModelItemDictionary {
		public static IModelItemDictionary FromModelItemDictionary(ModelItemDictionary value) {
			return value == null ? null : new XpfModelItemDictionary(value);
		}
		XpfModelItemDictionary(ModelItemDictionary value) : base(value) { }
		void IModelItemDictionary.Add(object key, object value) {
			XpfModelProperty.ExtractUnderlyingModel(ref value);
			XpfModelProperty.ExtractUnderlyingModel(ref key);
			this.Value.Add(key, value);
		}
		IEnumerable<IModelItem> IModelItemDictionary.Keys {
			get { return Value.Keys.Select(modelItem => XpfModelItem.FromModelItem(modelItem)); }
		}
		IEnumerable<IModelItem> IModelItemDictionary.Values {
			get { return Value.Values.Select(modelItem => XpfModelItem.FromModelItem(modelItem)); }
		}
		IModelItem IModelItemDictionary.this[IModelItem key] {
			get { return XpfModelItem.FromModelItem(Value[XpfModelItem.ToModelItem(key)]); }
			set { Value[XpfModelItem.ToModelItem(key)] = XpfModelItem.ToModelItem(value); }
		}
		IModelItem IModelItemDictionary.this[object key] {
			get { return XpfModelItem.FromModelItem(Value[key]); }
			set { Value[key] = XpfModelItem.ToModelItem(value); }
		}
	}
	public class XpfModelProperty : RuntimeBase<IModelProperty, ModelProperty>, IModelProperty {
		public static void ExtractUnderlyingModel(ref object value) {
			if(value is XpfModelItem)
				value = ((XpfModelItem)value).Value;
		}
		public static IModelProperty FromModelProperty(ModelProperty value) {
			return value == null ? null : new XpfModelProperty(value);
		}
		XpfModelProperty(ModelProperty value) : base(value) { }
		IModelItem IModelProperty.Parent {
			get { return XpfModelItem.FromModelItem(Value.Parent); }
		}
		string IModelProperty.Name {
			get { return Value.Name; }
		}
		bool IModelProperty.IsSet {
			get { return Value.IsSet; }
		}
		bool IModelProperty.IsReadOnly {
			get { return Value.IsReadOnly; }
		}
		void IModelProperty.ClearValue() {
			Value.ClearValue();
		}
		IModelItem IModelProperty.SetValue(object value) {
			ExtractUnderlyingModel(ref value);
			return XpfModelItem.FromModelItem(Value.SetValue(value));
		}
		object IModelProperty.ComputedValue { get { return Value.ComputedValue; } }
		IModelItem IModelProperty.Value {
			get { return XpfModelItem.FromModelItem(Value.Value); }
		}
		IModelItemCollection IModelProperty.Collection {
			get { return XpfModelItemColleciton.FromModelItemCollection(Value.Collection); }
		}
		IModelItemDictionary IModelProperty.Dictionary {
			get { return XpfModelItemDictionary.FromModelItemDictionary(Value.Dictionary); }
		}
		Type IModelProperty.PropertyType {
			get { return Value.PropertyType; }
		}
	}
	public class XpfModelEditingScope : RuntimeBase<IModelEditingScope, ModelEditingScope>, IModelEditingScope {
		public static IModelEditingScope FromModelEditingScope(ModelEditingScope value, IModelItem modelItem) {
			return value == null ? null : new XpfModelEditingScope(value, modelItem);
		}
		public static ModelEditingScope ToModelItem(IModelEditingScope value) {
			RuntimeBase<IModelEditingScope, ModelEditingScope> item = value as RuntimeBase<IModelEditingScope, ModelEditingScope>;
			return item != null ? item.Value : null;
		}
		XpfModelEditingScope(ModelEditingScope value, IModelItem modelItem)
			: base(value) {
			this.editingContext = modelItem.Context;
		}
		readonly IEditingContext editingContext;
		protected virtual void Dispose(bool disposing) {
			if(!disposing) return;
			Value.Dispose();
		}
		void IModelEditingScope.Complete() {
			Value.Complete();
			editingContext.Services.GetService<IModelService>().RaiseModelChanged();
		}
		string IModelEditingScope.Description {
			get {
				return Value.Description;
			}
			set {
				Value.Description = value;
			}
		}
		void IModelEditingScope.Update() {
			Value.Update();
		}
		void IDisposable.Dispose() {
			Dispose(true);
		}
	}
	public class XpfModelItem : RuntimeBase<IModelItem, ModelItem>, IModelItem {
		public static IModelItem FromModelItem(ModelItem value) {
			return value == null ? null : new XpfModelItem(value);
		}
		public static ModelItem ToModelItem(IModelItem value) {
			RuntimeBase<IModelItem, ModelItem> item = value as RuntimeBase<IModelItem, ModelItem>;
			return item != null ? item.Value : null;
		}
		XpfModelItem(ModelItem value) : base(value) { }
		IModelPropertyCollection IModelItem.Properties {
			get { return XpfModelPropertyColleciton.FromModelPropertyCollection(Value.Properties); }
		}
		IModelEditingScope IModelItem.BeginEdit(string description) {
			return XpfModelEditingScope.FromModelEditingScope(Value.BeginEdit(description), this);
		}
		public IEditingContext Context {
			get { return XpfEditingContext.FromEditingContext(Value.Context); }
		}
		IModelSubscribedEvent IModelItem.SubscribeToPropertyChanged(EventHandler handler) {
			return ModelSubscribedEvent<PropertyChangedEventHandler>.Subscribe((s, a) => handler(s, a), h => Value.PropertyChanged += h);
		}
		void IModelItem.UnsubscribeFromPropertyChanged(IModelSubscribedEvent e) {
			ModelSubscribedEvent<PropertyChangedEventHandler>.Unsubscribe(e, h => Value.PropertyChanged -= h);
		}
		IViewItem IModelItem.View {
			get { return XpfViewItem.FromViewItem(Value.View); }
		}
		Type IModelItem.ItemType {
			get { return Value.ItemType; }
		}
		IModelItem IModelItem.Root {
			get { return XpfModelItem.FromModelItem(Value.Root); }
		}
		object IModelItem.GetCurrentValue() {
			return Value.GetCurrentValue();
		}
		IEnumerable<object> IModelItem.GetAttributes(Type attributeType) {
			return Value.GetAttributes(attributeType);
		}
		string IModelItem.Name {
			get { return Value.Name; }
			set { Value.Name = value; }
		}
		IModelItem IModelItem.Parent {
			get { return XpfModelItem.FromModelItem(Value.Parent); }
		}
	}
	public class XpfViewItem : RuntimeBase<IViewItem, ViewItem>, IViewItem {
		public static IViewItem FromViewItem(ViewItem value) {
			return value == null ? null : new XpfViewItem(value);
		}
		XpfViewItem(ViewItem value) : base(value) { }
		object IViewItem.PlatformObject {
			get { return Value.PlatformObject; }
		}
	}
#if !SL
	public static class MarkupHelper {
		public static string GetBaseUri(IModelItem modelItem) {
			IMarkupAccessService markupAccessService = modelItem.Context.Services.GetService<IMarkupAccessService>();
			IMarkupAccessService2012 markupAccessService2012 = markupAccessService as IMarkupAccessService2012;
			if(markupAccessService2012 != null)
				return GetBaseUriByMarkupAccessService2012(markupAccessService2012, modelItem);
			IEnvDteService envDteService = modelItem.Context.Services.GetService<IEnvDteService>();
			if(envDteService != null)
				return GetBaseUriByEnvDte(envDteService, modelItem);
			return null;
		}
		public static object ParseMarkupExtension(object value, ICreateXamlMarkupService2010 createXamlMarkupService) {
			Guard.ArgumentNotNull(createXamlMarkupService, "createXamlMarkupService");
			try {
				string code = value as string;
				if(code == null || code.Length < 2) return value;
				if(code[0] != '{') return value;
				if(code[1] == '}') return value;
				try {
					if(createXamlMarkupService == null) return value;
					return ParseMarkupExtensionCore(code, createXamlMarkupService);
				} catch(MarkupParseException) {
					return value;
				}
			} catch(Exception e) {
				DebugHelper.Assert(e);
				return value;
			}
		}
		public static string GetCurrentValueSourceText(IModelProperty modelProperty) {
			IModelItem modelItem = modelProperty.Value;
			if(modelItem == null) return null;
			IMarkupAccessService markupAccessService = modelItem.Context.Services.GetService<IMarkupAccessService>();
			IMarkupAccessService2012 markupAccessService2012 = markupAccessService as IMarkupAccessService2012;
			if(markupAccessService2012 != null)
				return GetCurrentValueSourceTextByMarkupAccessService2012(markupAccessService2012, modelItem);
			return GetCurrentValueSourceTextByMarkupAccessService2010((IMarkupAccessService2010)markupAccessService, modelItem);
		}
		static string GetBaseUriByMarkupAccessService2012(IMarkupAccessService2012 markupAccessService, IModelItem modelItem) {
			ISceneNodeModelItem sceneNodeModelItem = markupAccessService.GetModelItem(modelItem);
			ISceneNode sceneNode = sceneNodeModelItem.SceneNode;
			IProjectContext projectContext = markupAccessService.GetModelItem(modelItem).SceneNode.ProjectContext;
			string pathToContext = FixFolderName(Path.GetDirectoryName(sceneNode.DocumentContext.DocumentLocator.RelativePath));
			return GetPackUri(projectContext.Metadata.ProjectAssembly.Name, pathToContext);
		}
		static string GetBaseUriByEnvDte(IEnvDteService envDteService, IModelItem modelItem) {
			string pathToContext = FixFolderName(Path.GetDirectoryName(envDteService.GetActiveProjectItemRelativePath()));
			string assemblyName = envDteService.GetActiveProjectAssemblyName();
			return GetPackUri(assemblyName, pathToContext);
		}
		static string GetPackUri(string assemblyName, string path) {
			return string.Format("{0}/{1};component/{2}", "pack://application:,,,", assemblyName, path);
		}
		static string FixFolderName(string path) {
			if(string.IsNullOrEmpty(path)) return null;
			path = path.Replace('\\', '/');
			return path[path.Length - 1] != '/' ? path + "/" : path;
		}
		static string GetCurrentValueSourceTextByMarkupAccessService2012(IMarkupAccessService2012 markupAccessService2012, IModelItem modelItem) {
			try {
				ISceneNodeModelItem sceneNodeModelItem = markupAccessService2012.GetModelItem(modelItem);
				ISceneNode sceneNode = sceneNodeModelItem.SceneNode;
				ISceneDocumentNode documentNode = sceneNode.DocumentNode;
				return sceneNodeModelItem.XamlExpressionSerializer.GetStringFromExpression(documentNode, documentNode.Parent);
			} catch(Exception e) {
				DebugHelper.Assert(e);
				return null;
			}
		}
		static string GetCurrentValueSourceTextByMarkupAccessService2010(IMarkupAccessService2010 markupAccessService2010, IModelItem modelItem) {
			try {
				IVirtualModelItem virtualModelItem = markupAccessService2010.GetModelItem(modelItem);
				IVirtualModelHost virtualModelHost = virtualModelItem.Host;
				object virtualModelItemIdentity = virtualModelItem.Identity;
				IVirtualDocumentNode documentNode = virtualModelHost.FindNode(virtualModelItemIdentity);
				string s;
				ISourceTextValue sourceTextValue = documentNode as ISourceTextValue;
				if(sourceTextValue != null) {
					s = sourceTextValue.Source;
				} else {
					IMarkupLocationProvider markupLocationProvider = documentNode as IMarkupLocationProvider;
					if(markupLocationProvider == null) {
						IChangedModelDocumentItem markupExtensionItem = documentNode as IChangedModelDocumentItem;
						if(markupExtensionItem == null) return null;
						return GetMarkupExtensionSourceText(markupExtensionItem);
					}
					MarkupLocation markupLocation = markupLocationProvider.GetLocation();
					IXamlItem xamlRootItem = virtualModelHost.FindNode(virtualModelHost.Root.Identity) as IXamlItem;
					if(xamlRootItem == null) return null;
					DevExpress.Design.SmartTags.ISourceReader reader = xamlRootItem.Document.Provider.CreateReader();
					s = reader.Read(markupLocation.Offset, markupLocation.Length);
				}
				if(s.TrimStart(' ').StartsWith("{", StringComparison.Ordinal)) return s;
				int b = s.IndexOfAny(new char[] { '\"', '\'' });
				if(b < 0) return s;
				int e = s.LastIndexOf(s[b]);
				if(e <= b)
					e = s.Length;
				return s.Substring(b + 1, e - b - 1);
			} catch(Exception e) {
				DebugHelper.Assert(e);
				return null;
			}
		}
		static string GetMarkupExtensionSourceText(IVirtualDocumentItem markupExtensionItem) {
			string markupName = GetMarkupExtensionName(markupExtensionItem);
			if(markupName == null) return null;
			string defaultPropertyName;
			switch(markupName) {
				case "StaticResource": defaultPropertyName = "ResourceKey"; break;
				case "DynamicResource": defaultPropertyName = "ResourceKey"; break;
				case "RelativeSource": defaultPropertyName = "Mode"; break;
				default: defaultPropertyName = null; break;
			}
			StringBuilder bindingString = new StringBuilder("{" + markupName);
			bool first = true;
			foreach(IVirtualDocumentProperty property in markupExtensionItem.Properties) {
				string name = property.Name;
				string value = GetPropertyValueSourceText(property);
				if(first) {
					bindingString.Append(' ');
					first = false;
				} else {
					bindingString.Append(", ");
				}
				if(string.IsNullOrEmpty(name) || name[0] == '<' || string.Equals(name, defaultPropertyName, StringComparison.Ordinal)) {
					bindingString.Append(value);
				} else {
					bindingString.Append(name);
					bindingString.Append('=');
					bindingString.Append(value);
				}
			}
			bindingString.Append('}');
			return bindingString.ToString();
		}
		static string GetPropertyValueSourceText(IVirtualDocumentProperty property) {
			IXamlMarkupExtensionProperty markupExtensionProperty = property as IXamlMarkupExtensionProperty;
			if(markupExtensionProperty != null) {
				string source = markupExtensionProperty.Source;
				if(!string.IsNullOrEmpty(source)) return source;
			} else {
				IVirtualDocumentValue documentValue = property.PropertyValue;
				string value = documentValue == null ? null : documentValue.Source;
				if(!string.IsNullOrEmpty(value)) return value;
			}
			IVirtualDocumentItem item = property.Items.Single();
			return GetMarkupExtensionSourceText(item);
		}
		static readonly string ObjectTypeFullName = typeof(object).FullName;
		static readonly string MarkupExtensionTypeFullName = typeof(MarkupExtension).FullName;
		static string GetMarkupExtensionName(IVirtualDocumentItem documentItem) {
			IVS2010TypeMetadata typeMetadata = documentItem.ItemType;
			string fullName = typeMetadata.FullName;
			IVS2010TypeMetadata baseType = typeMetadata;
			string baseTypefullName = fullName;
			while(true) {
				if(baseTypefullName == ObjectTypeFullName) return null;
				if(baseTypefullName == MarkupExtensionTypeFullName) break;
				baseType = baseType.BaseType;
				if(baseType == null) return null;
				baseTypefullName = baseType.FullName;
			}
			int d = fullName.LastIndexOf('.');
			return d < 0 ? fullName : fullName.Substring(d + 1);
		}
		class MarkupParseException : Exception { }
		static object ParseMarkupExtensionCore(string code, ICreateXamlMarkupService2010 createXamlMarkupService) {
			MarkupExtensionParser parser = new MarkupExtensionParser();
			ExpressionParserBase expressionParser = parser.CreateExpressionParser();
			MarkupExtensionExpression expression = expressionParser.Parse(new SourceStringReader(code)) as MarkupExtensionExpression;
			if(expression == null) return code;
			return ParseMarkupExtensionExpression(expression, createXamlMarkupService);
		}
		static object ParseExpression(DevExpress.CodeParser.Expression expression, ICreateXamlMarkupService2010 createXamlMarkupService) {
			MarkupExtensionExpression markup = expression as MarkupExtensionExpression;
			return markup == null ? Generate(expression) : ParseMarkupExtensionExpression(markup, createXamlMarkupService);
		}
		static object ParseMarkupExtensionExpression(MarkupExtensionExpression markup, ICreateXamlMarkupService2010 createXamlMarkupService) {
			string markupName = Generate(markup.Qualifier);
			IModelItem markupInstance = createXamlMarkupService.CreateXamlMarkup(markupName);
			if(markupInstance == null) throw new MarkupParseException();
			if(markup.Initializers != null) {
				foreach(AttributeVariableInitializer initializer in markup.Initializers) {
					string propertyName = Generate(initializer.LeftSide);
					object propertyValue = ParseExpression(initializer.RightSide, createXamlMarkupService);
					IModelProperty property = markupInstance.Properties[propertyName];
					if(property == null) throw new MarkupParseException();
					property.SetValue(propertyValue);
				}
			}
			string defaultPropertyName;
			switch(markupName) {
				case "Binding": defaultPropertyName = "Path"; break;
				case "StaticResource": defaultPropertyName = "ResourceKey"; break;
				case "DynamicResource": defaultPropertyName = "ResourceKey"; break;
				case "RelativeSource": defaultPropertyName = "Mode"; break;
				default: defaultPropertyName = null; break;
			}
			if(markup.Arguments != null && markup.Arguments.Count != 0) {
				if(defaultPropertyName == null || markup.Arguments.Count > 1) throw new MarkupParseException();
				object argumentValue = ParseExpression(markup.Arguments[0], createXamlMarkupService);
				IModelProperty property = markupInstance.Properties[defaultPropertyName];
				if(property == null) throw new MarkupParseException();
				property.SetValue(argumentValue);
			}
			return markupInstance;
		}
		static string Generate(LanguageElement element) {
			return element.ToString(); 
		}
	}
	public static class ViewModelHelper {
		public static Func<IDXAssemblyMetadata, bool> ForceIncludeAssembly = null;
		public static Func<string, bool> ForceIncludeNamespace = null;
		public static Func<IDXTypeMetadata, bool> ForceIncludeType = null;
		public static IEnumerable<IDXTypeMetadata> GetViewModels(bool fromActiveProjectOnly, IModelItem modelItem) {
			IDXTypeDiscoveryService typeDiscoveryService = modelItem.Context.Services.GetService<IDXTypeDiscoveryService>();
			return typeDiscoveryService.GetTypes(
				a => ViewModelAssemblyPredicate(a, ForceIncludeAssembly),
				t => ViewModelTypePredicate(t, ForceIncludeNamespace, ForceIncludeType),
				fromActiveProjectOnly
				).ToList();
		}
		public static bool ViewModelAssemblyPredicate(IDXAssemblyMetadata assembly, Func<IDXAssemblyMetadata, bool> forceIncludeAssembly) {
			if(assembly == null) return false;
			if(forceIncludeAssembly == null || !forceIncludeAssembly(assembly)) {
				string assemblyName = assembly.Name;
				if(string.Equals(assemblyName, "mscorlib", StringComparison.Ordinal)) return false;
				if(string.Equals(assemblyName, "System", StringComparison.Ordinal)) return false;
				if(string.Equals(assemblyName, "WindowsBase", StringComparison.Ordinal)) return false;
				if(string.Equals(assemblyName, "PresentationCore", StringComparison.Ordinal)) return false;
				if(string.Equals(assemblyName, "PresentationFramework", StringComparison.Ordinal)) return false;
				if(string.Equals(assemblyName, "UIAutomationTypes", StringComparison.Ordinal)) return false;
				if(string.Equals(assemblyName, "VSLangProj", StringComparison.OrdinalIgnoreCase)) return false;
				if(string.Equals(assemblyName, "Accessibility", StringComparison.Ordinal)) return false;
				if(string.Equals(assemblyName, "log4net", StringComparison.Ordinal)) return false;
				if(assemblyName.StartsWith("TestDriven.", StringComparison.Ordinal)) return false;
				if(assemblyName.StartsWith("CR_", StringComparison.Ordinal)) return false;
				if(assemblyName.StartsWith("Moq.", StringComparison.Ordinal)) return false;
				if(assemblyName.StartsWith("Castle.", StringComparison.Ordinal)) return false;
				if(assemblyName.StartsWith("NUnit", StringComparison.OrdinalIgnoreCase)) return false;
				if(assemblyName.StartsWith("System.", StringComparison.Ordinal)) return false;
				if(assemblyName.StartsWith("Microsoft.", StringComparison.Ordinal)) return false;
				if(assemblyName.StartsWith("PresentationFramework.", StringComparison.Ordinal)) return false;
				if(IsDXControlsAssembly(assemblyName) || assemblyName.StartsWith("DevExpress.Mvvm") || assemblyName.Equals("DevExpress.Xpf.LayoutControl.Tests")) return false;
				if(assemblyName.StartsWith("EnvDTE", StringComparison.OrdinalIgnoreCase)) return false;
			}
			return true;
		}
		static bool IsDXControlsAssembly(string assemblyName) {
			return assemblyName.StartsWith("DevExpress.", StringComparison.Ordinal) && assemblyName.Contains(AssemblyInfo.VSuffix);
		}
		static bool ViewModelTypePredicate(IDXTypeMetadata type, Func<string, bool> forceIncludeNamespace = null, Func<IDXTypeMetadata, bool> forceIncludeType = null) {
			if(type == null) return false;
			string typeNamespace = type.Namespace;
			if(string.IsNullOrEmpty(typeNamespace)) return false;
			if(forceIncludeNamespace == null || !forceIncludeNamespace(typeNamespace)) {
				if(string.Equals(typeNamespace, "NUnit", StringComparison.OrdinalIgnoreCase)) return false;
				if(string.Equals(typeNamespace, "System", StringComparison.Ordinal)) return false;
				if(string.Equals(typeNamespace, "Microsoft", StringComparison.Ordinal)) return false;
				if(string.Equals(typeNamespace, "DevExpress", StringComparison.Ordinal)) return false;
				if(string.Equals(typeNamespace, "Castle.Proxies", StringComparison.Ordinal)) return false;
				if(typeNamespace.StartsWith("Castle.Proxies.", StringComparison.Ordinal)) return false;
				if(typeNamespace.StartsWith("CR_", StringComparison.Ordinal)) return false;
				if(typeNamespace.StartsWith("NUnit.", StringComparison.OrdinalIgnoreCase)) return false;
				if(typeNamespace.StartsWith("System.", StringComparison.Ordinal)) return false;
				if(typeNamespace.StartsWith("Microsoft.", StringComparison.Ordinal)) return false;
				if(typeNamespace.StartsWith("DevExpress.", StringComparison.Ordinal) && IsDXControlsAssembly(type.Assembly.Name)) return false;
			}
			if(forceIncludeType == null || !forceIncludeType(type)) {
				if(type.FullName.EndsWith(".Properties.Resources", StringComparison.Ordinal)) return false;
				if(type.IsValueType || type.IsArray || type.IsEnum) return false;
				if(type.IsGenericType || type.IsInterface) return false;
				if(!type.IsVisible) return false;
				if(type.ImplementsInterface(typeof(IValueConverter))) return false;
				for(IDXTypeMetadata baseType = type; baseType != null; baseType = baseType.BaseType) {
					string baseTypeFullName = baseType.FullName;
					if(string.IsNullOrEmpty(baseTypeFullName)) continue;
					if(baseTypeFullName.StartsWith("System.Windows.", StringComparison.Ordinal)) return false;
					if(baseTypeFullName.StartsWith("System.Configuration.", StringComparison.Ordinal)) return false;
				}
				bool isPocoViewModel = type.IsPocoViewModel;
				if(!type.HasDefaultConstructor && !isPocoViewModel) return false;
				if(type.IsAbstract && !isPocoViewModel) return false;
			}
			return true;
		}
	}
	public class XpfUriContext : IUriContext {
		readonly IEditingContext editingContext;
		public XpfUriContext(IEditingContext editingContext) {
			this.editingContext = editingContext;
		}
		public Uri BaseUri {
			get {
				IModelItem modelItem = editingContext.Services.GetService<IModelService>().Root;
				string baseUri = MarkupHelper.GetBaseUri(modelItem);
				return string.IsNullOrEmpty(baseUri) ? null : new Uri(baseUri);
			}
			set {
				try {
					throw new NotSupportedException();
				} catch(Exception e) {
					DebugHelper.Assert(e);
				}
			}
		}
	}
	public static class ProjectReferencesHelper {
		public static void EnsureAssemblyReferenced(IModelItem modelItem, string simpleNameOrPath, bool forceCopyLocal) {
			try {
				EnsureAssemblyReferencedCore(modelItem, simpleNameOrPath, forceCopyLocal);
			} catch(Exception e) {
				DebugHelper.Assert(e);
			}
		}
		static void EnsureAssemblyReferencedCore(IModelItem modelItem, string simpleNameOrPath, bool forceCopyLocal) {
			IMarkupAccessService markupAccessService = modelItem.Context.Services.GetService<IMarkupAccessService>();
			if(!forceCopyLocal) {
				IMarkupAccessService2012 markupAccessService2012 = markupAccessService as IMarkupAccessService2012;
				if(markupAccessService2012 != null) {
					EnsureAssemblyReferencedByMarkupAccessService2012(markupAccessService2012, modelItem, simpleNameOrPath);
					return;
				}
			}
			IEnvDteService envDteService = modelItem.Context.Services.GetService<IEnvDteService>();
			if(envDteService != null)
				EnsureAssemblyReferencedByEnvDte(envDteService, modelItem, simpleNameOrPath, forceCopyLocal);
		}
		static void EnsureAssemblyReferencedByMarkupAccessService2012(IMarkupAccessService2012 markupAccessService2012, IModelItem modelItem, string simpleNameOrPath) {
			ISceneNodeModelItem sceneNodeModelItem = markupAccessService2012.GetModelItem(modelItem);
			sceneNodeModelItem.SceneNode.ProjectContext.Metadata.EnsureAssemblyReferenced(simpleNameOrPath, false);
		}
		static void EnsureAssemblyReferencedByEnvDte(IEnvDteService envDteService, IModelItem modelItem, string simpleNameOrPath, bool forceCopyLocal) {
			envDteService.EnsureAssemblyReferencedByEnvDte(simpleNameOrPath, forceCopyLocal);
		}
	}
#endif
	public class EmptyModelChangedEventArgs : ModelChangedEventArgs {
		public override IEnumerable<ModelItem> ItemsAdded { get { return new ModelItem[] { }; } }
		public override IEnumerable<ModelItem> ItemsRemoved { get { return new ModelItem[] { }; } }
		public override IEnumerable<ModelProperty> PropertiesChanged { get { return new ModelProperty[] { }; } }
		public override IEnumerable<string> PropertyNamesChanged { get { return new string[] { }; } }
	}
	public static class ImageResourceHelper {
#if SL
		static readonly string[] extensions = new string[] { ".jpg", ".png", ".jpeg" }; 
#else
		static readonly string[] extensions = new string[] { ".bmp", ".jpg", ".png", ".gif", ".ico", ".dib", ".jpe", ".jpeg", ".tif", ".tiff" }; 
#endif
		public static IEnumerable<Uri> GetImageUris(IEditingContext editingContext) {
			Guard.ArgumentNotNull(editingContext, "editingContext");
			ExternalResourceService requiredService = editingContext.Services.GetService<ExternalResourceService>();
			if(requiredService == null)
				throw new InvalidOperationException();
			return requiredService.ResourceUris.Where(u => IsImageFile(u));
		}
		static bool IsImageFile(Uri resourceUri) {
			string pathFromPackUri = GetPathFromPackUri(resourceUri);
			if(!string.IsNullOrEmpty(pathFromPackUri)) {
				string extension = Path.GetExtension(pathFromPackUri);
				foreach(string current in extensions) {
					if(string.Compare(extension, current, System.StringComparison.OrdinalIgnoreCase) == 0) {
						return true;
					}
				}
				return false;
			}
			return false;
		}
		public static string GetPathFromPackUri(System.Uri uri) {
			string text;
			if(uri.IsAbsoluteUri && uri.IsFile) {
				text = uri.LocalPath;
			} else {
				if(uri.IsAbsoluteUri) {
					text = System.IO.Packaging.PackUriHelper.GetPartUri(uri).ToString();
				} else {
					text = uri.ToString();
				}
			}
			int num = 0;
			if(text[0] == '/') {
				num = 1;
			}
			string result = text.Substring(num);
			int num2 = text.IndexOf('/', num);
			if(num2 > 0) {
				string text2 = text.Substring(num, num2 - num);
				if(text2.EndsWith(";component", System.StringComparison.OrdinalIgnoreCase)) {
					result = text.Substring(num2 + 1);
				}
			}
			return result;
		}
	}
}

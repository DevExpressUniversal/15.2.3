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
using System.IO;
using System.Resources;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using DevExpress.Serialization.Services;
using System.Drawing;
using DevExpress.Data.Utils;
using DevExpress.XtraReports.Serialization;
namespace DevExpress.XtraReports.Serialization {
	[ProvideProperty("Modifiers", typeof(IComponent))]
	public class SModifiersInheritedExtenderProvider : IExtenderProvider {
		private IComponent baseComponent;
		public bool CanExtend(object obj) {
			if(baseComponent == null) {
				ISite site = ((IComponent)obj).Site;
				if(site != null) {
					IDesignerHost host = (IDesignerHost)site.GetService(typeof(IDesignerHost));
					if(host != null) {
						baseComponent = host.RootComponent;
					}
				}
			}
			InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(obj)[typeof(InheritanceAttribute)];
			return obj == baseComponent || !inheritanceAttribute.Equals(InheritanceAttribute.NotInherited) ?
				false : true;
		}
		[
		DesignOnly(true),
		DefaultValue(MemberAttributes.Private),
		]
		public MemberAttributes GetModifiers(IComponent comp) {
			System.Diagnostics.Debug.Assert(baseComponent != null);
			Type baseType = baseComponent.GetType();
			ISite site = comp.Site;
			if(site != null) {
				string name = site.Name;
				FieldInfo field = baseType.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
				if(field != null) {
					if(field.IsPrivate) return MemberAttributes.Private;
					if(field.IsPublic) return MemberAttributes.Public;
					if(field.IsFamily) return MemberAttributes.Family;
					if(field.IsAssembly) return MemberAttributes.Assembly;
				}
			}
			return MemberAttributes.Private;
		}
	}
	public class SContainer : IContainer {
		ArrayList components = new ArrayList();
		public SContainer() {
		}
		public void Add(IComponent component) {
			components.Add(component);
		}
		public void Add(IComponent component, string name) {
		}
		public void Remove(IComponent component) {
		}
		public void Dispose() {
		}
		public ComponentCollection Components {
			get {
				return new ComponentCollection((IComponent[])components.ToArray(typeof(IComponent)));
			}
		}
	}
	public class XRDesignSite : ISite, IDictionaryService {
		protected IComponent fComponent;
		string name = "";
		IContainer container = null;
		SDictionaryService dictionaryService;
		protected IServiceProvider serviceProvider = null;
		public XRDesignSite(IComponent component, string name, IContainer container, IServiceProvider serviceProvider) {
			this.container = container;
			this.fComponent = component;
			this.name = name;
			this.serviceProvider = serviceProvider;
			this.dictionaryService = new SDictionaryService();
		}
		public IComponent Component {
			get { return fComponent; }
		}
		public IContainer Container {
			get { return container; }
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public object GetService(Type serviceType) {
			if(serviceType == typeof(IDictionaryService))
				return this;
			return serviceProvider.GetService(serviceType);
		}
		public bool DesignMode { 
			get {
				IDesignerHost designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				return (designerHost != null && designerHost.RootComponent == fComponent) || !(fComponent is XtraReports.UI.XtraReport);
			}
		}
		object IDictionaryService.GetKey(object value) {
			return dictionaryService.GetKey(value);
		}
		object IDictionaryService.GetValue(object key) {
			return dictionaryService.GetValue(key);
		}
		void IDictionaryService.SetValue(object key, object value) {
			dictionaryService.SetValue(key, value);
		}
	}
	public class SDesignerHost : IDesignerHost {
		IComponent rootComponent;
		IDesignerHost nativeDesignerHost;
		public SDesignerHost(IComponent rootComponent) {
			this.rootComponent = rootComponent;
			if(rootComponent.Site != null)
				nativeDesignerHost = (IDesignerHost)rootComponent.Site.GetService(typeof(IDesignerHost));
		}
		public bool Loading {
			get { return false; }
		}
		public void Activate() {
		}
		public event EventHandler Activated { add { } remove { } }
		public event EventHandler Deactivated { add { } remove { } }
		public event EventHandler LoadComplete { add { } remove { } }
		public IContainer Container {
			get { return rootComponent.Site.Container; }
		}
		public IComponent RootComponent {
			get { return rootComponent; }
		}
		public string RootComponentClassName {
			get { return String.Empty; }
		}
		public IComponent CreateComponent(Type componentClass, string name) {
			return null;
		}
		public IComponent CreateComponent(Type componentClass) {
			return null;
		}
		public void DestroyComponent(IComponent component) {
		}
		public IDesigner GetDesigner(IComponent component) {
			return nativeDesignerHost != null ? nativeDesignerHost.GetDesigner(component) : null;
		}
		public Type GetType(string typeName) {
			return null;
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) { }
		public void AddService(Type serviceType, object serviceInstance) { }
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) { }
		public void AddService(Type serviceType, object serviceInstance, bool promote) { }
		public void RemoveService(Type serviceType) { }
		public void RemoveService(Type serviceType, bool promote) { }
		public object GetService(Type serviceType) { return null; }
		public bool InTransaction {
			get { return false; }
		}
		public string TransactionDescription {
			get { return ""; }
		}
		public DesignerTransaction CreateTransaction(string description) {
			return null;
		}
		public DesignerTransaction CreateTransaction() {
			return this.CreateTransaction(null);
		}
		public event EventHandler TransactionOpened { add { } remove { } }
		public event EventHandler TransactionOpening { add { } remove { } }
		public event DesignerTransactionCloseEventHandler TransactionClosed { add { } remove { } }
		public event DesignerTransactionCloseEventHandler TransactionClosing { add { } remove { } }
	}
	public interface IResourceWriterBuilder {
		ResourceWriter CreateResourceWriter(MemoryStream stream);
	}
	public class SResourceWriter : IResourceWriter, IDisposable {
		MemoryStream memoryStream;
		ResourceWriter resourceWriter;
		public SResourceWriter(IServiceProvider serviceProvider) {
			memoryStream = new MemoryStream();
			IResourceWriterBuilder serv = serviceProvider.GetService(typeof(IResourceWriterBuilder)) as IResourceWriterBuilder;
			resourceWriter = serv != null ? serv.CreateResourceWriter(memoryStream) : new ResourceWriter(memoryStream);
		}
		public string Resources {
			get {
				byte[] bytes = memoryStream.ToArray();
				return Convert.ToBase64String(bytes);
			}
		}
		#region IResourceWriter implementation
		public void AddResource(string name, object value) {
			if(value is Image) {
				byte[] array = new ImageTool().ToArray((Image)value);
				if(array.Length == 0)
					array = new ImageTool().ToArray((Image)value, System.Drawing.Imaging.ImageFormat.Png);
				if(array.Length > 0)
					resourceWriter.AddResource(XRResourceManager.ToImageSpecificName(name), array);
			} else
				resourceWriter.AddResource(name, value);
		}
		public void AddResource(string name, string value) {
			resourceWriter.AddResource(name, value);
		}
		public void AddResource(string name, byte[] value) {
			resourceWriter.AddResource(name, value);
		}
		public void Close() {
			resourceWriter.Close();
		}
		public void Generate() {
			resourceWriter.Generate();
		}
		#endregion
		public void Dispose() {
			resourceWriter.Dispose();
		}
	}
	public static class ComponentCompiler {
		static IComponent GetSerializedComponent(Assembly assembly) {
			Type[] types = assembly.GetTypes();
			foreach(Type type in types) {
				if(typeof(IComponent).IsAssignableFrom(type)) {
					ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
					return ci.Invoke(new object[] { }) as IComponent;
				}
			}
			return null;
		}
		static CompilerParameters CreateCompilerParams(string[] references) {
			CompilerParameters compilerParams = new CompilerParameters();
			compilerParams.ReferencedAssemblies.AddRange(references);
			compilerParams.GenerateInMemory = true;
			return compilerParams;
		}
		static CompilerResults CompileAssemblyFromSource(CodeDomProvider provider, CompilerParameters parameters, string source) {
			return provider.CompileAssemblyFromSource(parameters, source);
		}
		public static IComponent Compile(Stream stream, string[] references) {
			stream.Seek(0, SeekOrigin.Begin);
			using(CSharpCodeProvider provider = new CSharpCodeProvider())
			using(StreamReader reader = new StreamReader(stream)) {
				return Compile(provider, reader.ReadToEnd(), references);
			}
		}
		public static IComponent Compile(CodeDomProvider provider, Stream stream, string[] references) {
			stream.Seek(0, SeekOrigin.Begin);
			using(StreamReader reader = new StreamReader(stream)) {
				return Compile(provider, reader.ReadToEnd(), references);
			}
		}
		public static IComponent Compile(CodeDomProvider provider, string code, string[] references) {
			CompilerResults results;
			results = CompileAssemblyFromSource(provider, CreateCompilerParams(references), code);
			if(results.Errors.HasErrors)
				return null;
			return GetSerializedComponent(results.CompiledAssembly);
		}
		public static string[] GetReferences(Type type) {
			AssemblyName[] names = type.Assembly.GetReferencedAssemblies();
			string[] references = new string[names.Length + 1];
			for(int i = 0; i < names.Length; i++)
				references[i] = Assembly.Load(names[i]).Location;
			references[names.Length] = type.Assembly.Location;
			return references;
		}
	}
}

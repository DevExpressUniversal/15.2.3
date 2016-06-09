#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.CodeGeneration;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Utils.Design;
namespace DevExpress.ExpressApp.Model.Core {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ModelApplicationCreator : IModelNodeValidatorRegistrator, IModelNodeUpdaterRegistrator {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Boolean DebugMode {
			get { return GlobalModelAssemblyRegistry.DebugMode; }
			set { GlobalModelAssemblyRegistry.DebugMode = value; }
		}
		private readonly ModelNodeInfo emptyModelNodeInfo;
		private readonly ModelApplicationCreatorProperties properties;
		private readonly Type modelApplicationNodeBaseInterface;
		private readonly List<string> aspects;
		private ModelNodesGeneratorUpdaters generatorUpdaters;
		private readonly List<ModelApplicationCreatorOnAddNodeFromXml> onAddNodeFromXmlDelegates;
		internal readonly ModelValueFactory modelValueFactory;
		private ModelApplicationCreatorInfoBase creatorInfo;
		#region CreateCreatorInfo
		private static ModelApplicationCreatorInfoBase CreateCreatorInfo(ModelApplicationCreator modelApplicationCreator) {
			Assembly assembly = GetModelAssembly(modelApplicationCreator.properties);
			Type creatorInfoType = assembly.GetType(GetModelClassName(typeof(IModelApplicationCreatorInfo)));
			return (ModelApplicationCreatorInfoBase)TypeHelper.CreateInstance(creatorInfoType, modelApplicationCreator);
		}
		private readonly static Object locker = new Object();
		private static Assembly fileModelAssembly = null;
		private static Assembly GetModelAssembly(ModelApplicationCreatorProperties properties) {
			lock(locker) {
				Assembly modelAssembly;
				if(File.Exists(properties.AssemblyFileAbsolutePath)) {
					if(fileModelAssembly == null) {
						fileModelAssembly = Assembly.LoadFrom(properties.AssemblyFileAbsolutePath); 
					}
					modelAssembly = fileModelAssembly;
				}
				else {
					properties.CollectInterfaces();
					IList<Type> customInterfacesToGenerate = properties.GetInterfacesToGenerate();
					ClassDescriptionCollection classDescriptions = GetModelClassDescriptions(properties);
					modelAssembly = GlobalModelAssemblyRegistry.Get(customInterfacesToGenerate, classDescriptions, properties.CustomLogics, properties.InterfacesExtenders, properties.AssemblyFileAbsolutePath);
				}
				return modelAssembly;
			}
		}
		private static ClassDescriptionCollection GetModelClassDescriptions(ModelApplicationCreatorProperties properties) {
			ClassDescriptionCollection result = new ClassDescriptionCollection();
			foreach(Type interfaceType in properties.GetInterfacesToImplement()) {
				String className = GetModelClassName(interfaceType);
				Type baseClass = GetModelNodeBaseClass(properties, interfaceType);
				Type[] implementors = GetAllInterfacesByBaseInterface(properties, interfaceType);
				result.Add(className, interfaceType, baseClass, implementors);
			}
			Type creatorInfoType = typeof(IModelApplicationCreatorInfo);
			result.Add(GetModelClassName(creatorInfoType), creatorInfoType, typeof(ModelApplicationCreatorInfoBase), new Type[] { creatorInfoType });
			return result;
		}
		private static Type GetModelNodeBaseClass(ModelApplicationCreatorProperties properties, Type interfaceType) {
			if(interfaceType == properties.ModelApplicationNodeBaseInterface) {
				return properties.ModelApplicationNodeBaseClass;
			}
			if(HasGenericInterface(interfaceType)) {
				return typeof(ModelNodeList<object>);
			}
			return typeof(ModelNode);
		}
		private static bool HasGenericInterface(Type interfaceType) {
			foreach(Type type in interfaceType.GetInterfaces()) {
				if(type.IsGenericType)
					return true;
			}
			return false;
		}
		private static Type[] GetAllInterfacesByBaseInterface(ModelApplicationCreatorProperties properties, Type baseInterface) {
			List<Type> interfaces = new List<Type>();
			interfaces.Add(baseInterface);
			FillInterfaceExtenders(properties, baseInterface, interfaces);
			if(!interfaces.Contains(typeof(IModelNode))) {
				interfaces.Add(typeof(IModelNode));
			}
			return interfaces.ToArray();
		}
		private static void FillInterfaceExtenders(ModelApplicationCreatorProperties properties, Type baseInterface, List<Type> interfaces) {
			List<Type> interfaceExtenders = new List<Type>();
			GetInterfaceExtendersByInterface(properties, baseInterface, interfaceExtenders);
			foreach(Type interfaceExtender in interfaceExtenders) {
				if(!interfaces.Contains(interfaceExtender)) {
					interfaces.Add(interfaceExtender);
					FillInterfaceExtenders(properties, interfaceExtender, interfaces);
				}
			}
		}
		private static void GetInterfaceExtendersByInterface(ModelApplicationCreatorProperties properties, Type baseInterface, List<Type> interfaces) {
			interfaces.AddRange(properties.InterfacesExtenders.GetInterfaceExtenders(baseInterface));
			foreach(Type implementedInterface in TypeHelper.GetInterfaces(baseInterface)) {
				GetInterfaceExtendersByInterface(properties, implementedInterface, interfaces);
			}
		}
		#endregion
		private static readonly List<ModelApplicationCreator> creatorList = new List<ModelApplicationCreator>();
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelApplicationCreator GetModelApplicationCreator(ModelApplicationCreatorProperties properties) {
			try { 
				if(DesignTimeTools.IsDesignMode) {
					return new ModelApplicationCreator(properties);
				}
			}
			catch(SecurityException) { }
			foreach(ModelApplicationCreator item in creatorList) {
				if(item.properties.Equals(properties)) {
					lock(TypesInfo.lockObject) {
						item.ClearValidators(); 
						item.ClearUpdaters(); 
						item.ClearAddNodeFromXmlDelegate(); 
						item.ClearGeneratorsUpdaters(); 
						item.aspects.Clear(); 
					}
					return item;
				}
			}
			ModelApplicationCreator creator = new ModelApplicationCreator(properties);
			creatorList.Add(creator);
			return creator;
		}
		private ModelApplicationCreator(ModelApplicationCreatorProperties properties) {
			this.properties = properties;
			modelApplicationNodeBaseInterface = properties.ModelApplicationNodeBaseInterface;
			aspects = new List<string>();
			onAddNodeFromXmlDelegates = new List<ModelApplicationCreatorOnAddNodeFromXml>();
			emptyModelNodeInfo = new ModelNodeInfo(this, null, typeof(ModelNode), typeof(IModelNode), ModelNodesDefaultInterfaceGenerator.Instance, ModelValueNames.Id);
			modelValueFactory = new ModelValueFactory();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetModelClassName(Type type) {
			Guard.ArgumentNotNull(type, "type");
			string name = type.Name;
			if(type.IsGenericType) {
				name = string.Format("ModelNodeList<{0}>", type.GetGenericArguments()[0].Name);
			}
			else if(type.IsInterface) {
				name = type.Name.Remove(0, 1);
			}
			return name;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetDefaultXmlName(Type nodeType) {
			string name = GetModelClassName(nodeType);
			if(name.StartsWith("Model")) {
				name = name.Remove(0, "Model".Length);
			}
			return name;
		}
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private ModelApplicationCreatorInfoBase CreatorInfo {
			get {
				if(creatorInfo == null) {
					creatorInfo = CreateCreatorInfo(this);
					AssignNodesGeneratorUpdaters();
				}
				return creatorInfo;
			}
		}
		private bool IsCreatorInfoInitialized { get { return creatorInfo != null; } }
		private List<IModelNodesNotifierGenerator> NotifierGenerators { get { return CreatorInfo.NotifierGenerators; } }
		private List<ModelApplicationCreatorOnAddNodeFromXml> OnAddNodeFromXmlDelegates { get { return onAddNodeFromXmlDelegates; } }
		internal ModelNodeInfo EmptyModelNodeInfo { get { return emptyModelNodeInfo; } }
		public void AddAspects(IEnumerable<string> aspects) {
			if(aspects != null) {
				lock(TypesInfo.lockObject) {
					foreach(string aspect in aspects) {
						if(!this.aspects.Contains(aspect)) {
							this.aspects.Add(aspect);
						}
					}
				}
			}
		}
		internal void ClearAddNodeFromXmlDelegate() {
			OnAddNodeFromXmlDelegates.Clear();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void AddOnAddNodeFromXmlDelegate(ModelApplicationCreatorOnAddNodeFromXml method) {
			OnAddNodeFromXmlDelegates.Add(method);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveOnAddNodeFromXmlDelegate(ModelApplicationCreatorOnAddNodeFromXml method) {
			OnAddNodeFromXmlDelegates.Remove(method);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelApplicationBase CreateModelApplication() {
			ModelApplicationBase result = (ModelApplicationBase)GetNodeInfo(modelApplicationNodeBaseInterface).CreateNode(string.Empty);
			lock(TypesInfo.lockObject) {
				foreach(string aspect in aspects) {
					result.AddAspect(aspect);
				}
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNodeInfo GetNodeInfo(Type nodeType) {
			return CreatorInfo.GetNodeInfo(nodeType);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<ModelNodeInfo> GetNodeInfos(Type nodeType) {
			return CreatorInfo.GetNodeInfos(nodeType);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void OnNodePropertyChanged(ModelNode node, string propertyName) {
			ModelApplicationBase application = node.Root as ModelApplicationBase;
			if(application == null || application.IsLoading) return;
			foreach(IModelNodesNotifierGenerator generator in NotifierGenerators) {
				if(generator.IsCompatible(node)) {
					generator.OnPropertyChanged(node, propertyName);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void OnNodeAdded(ModelNode node) {
			foreach(IModelNodesNotifierGenerator generator in NotifierGenerators) {
				if(generator.IsCompatible(node)) {
					generator.OnNodeAdded(node);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void OnNodeRemoved(ModelNode node) {
			foreach(IModelNodesNotifierGenerator generator in NotifierGenerators) {
				if(generator.IsCompatible(node)) {
					generator.OnNodeRemoved(node);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearValidators() {
			if(IsCreatorInfoInitialized) {
				CreatorInfo.ClearValidators();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearUpdaters() {
			if(IsCreatorInfoInitialized) {
				CreatorInfo.ClearUpdaters();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void AddNodesGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
			generatorUpdaters = updaters;
			AssignNodesGeneratorUpdaters();
		}
		private void AssignNodesGeneratorUpdaters() {
			if(IsCreatorInfoInitialized && generatorUpdaters != null) {
				foreach(ModelNodesGeneratorBase generator in CreatorInfo.Generators) {
					generator.AddUpdaters(generatorUpdaters[generator.GetType()]);
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearGeneratorsUpdaters() {
			foreach(ModelNodesGeneratorBase generator in CreatorInfo.Generators) {
				generator.ClearUpdaters();
			}
		}
		internal ConvertXmlParameters FillAddNodeFromXml(ModelNode node, Type nodeType, string nodeName, IDictionary<string, string> values) {
			ConvertXmlParameters convertXmlParameters = new ConvertXmlParameters();
			convertXmlParameters.Assign(node, nodeType, nodeName, values);
			foreach(ModelApplicationCreatorOnAddNodeFromXml method in OnAddNodeFromXmlDelegates) {
				method(convertXmlParameters);
			}
			if(convertXmlParameters.NodeType != null && convertXmlParameters.NodeType.IsInterface) {
				convertXmlParameters.NodeType = GetModelType(GetModelClassName(convertXmlParameters.NodeType));
			}
			return convertXmlParameters;
		}
		private Type GetImplementorType(Type type) {
			if(type.IsClass)
				return type;
			ModelNodeInfo nodeInfo = GetNodeInfo(type);
			return nodeInfo != null ? nodeInfo.GeneratedClass : null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type GetModelType(string className) { return CreatorInfo.GetModelType(className); }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelNode CreateNode(string nodeId, Type nodeType) {
			return GetNodeInfo(nodeType).CreateNode(nodeId);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanCreateNodeByType(ModelNode parent, string id, Type type) {
			string dummy;
			return CanCreateNodeByType(parent, ref id, type, out dummy);
		}
		internal bool CanCreateNodeByType(ModelNode parent, ref string id, Type type, out string exceptionText) {
			exceptionText = string.Empty;
			if(type == null) {
				exceptionText = "Type can't be null.";
				return false;
			}
			if(string.IsNullOrEmpty(id)) {
				id = GetIdByType(parent, type);
				if(!string.IsNullOrEmpty(id)) return true;
			}
			if(type.IsClass || type.IsInterface) {
				if(parent != null) {
					if(type.IsInterface) {
						Type implementorType = GetImplementorType(type);
						if(implementorType == null) {
							exceptionText = string.Format("The interface {0} is not a model interface.", type);
							return false;
						}
						type = implementorType;
					}
					if(!parent.NodeInfo.CanCreateListChildNode(type)) {
						exceptionText = string.Format("The type {0} doesn't support childs with type {1}.", parent.GetType(), type);
					}
					if(string.IsNullOrEmpty(exceptionText) && !string.IsNullOrEmpty(id)) {
						Type childType = parent.NodeInfo.GetTypeByChildNodeName(id);
						if(childType != null && !childType.IsAssignableFrom(type)) {
							exceptionText = string.Format("The type {0} has been excpected for id {1}, but was type {2}.", childType, id, type);
						}
					}
				}
			}
			else {
				exceptionText = string.Format("The type {0} is not acceptable.", type);
			}
			return string.IsNullOrEmpty(exceptionText);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string GetXmlName(ModelNode node) {
			Type type = node.NodeInfo.GeneratedClass;
			if(!node.IsRoot) {
				if(node.IsListNode) {
					string result = node.Parent.NodeInfo.GetXmlNameByChildNodeType(type);
					return !string.IsNullOrEmpty(result) ? result : GetDefaultXmlName(type);
				}
				else {
					return node.Id;
				}
			}
			return GetDefaultXmlName(type);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string GetIdByType(ModelNode parent, Type type) {
			string result = null;
			if(type != null && parent != null && parent.NodeInfo != null) {
				Type implementorType = GetImplementorType(type);
				result = parent.NodeInfo.GetNameByChildNodeType(implementorType);
			}
			return result;
		}
		#region IModelNodeValidatorRegistrator Members
		public void AddValidator<T>(IModelNodeValidator<T> validator) where T : IModelNode {
			foreach(ModelNodeInfo nodeInfo in GetNodeInfos(typeof(T))) {
				nodeInfo.AddValidator(validator);
			}
		}
		#endregion
		#region IModelNodeUpdaterRegistrator Members
		public void AddUpdater<T>(IModelNodeUpdater<T> updater) where T : IModelNode {
			foreach(ModelNodeInfo nodeInfo in GetNodeInfos(typeof(T))) {
				nodeInfo.AddUpdater(updater);
			}
		}
		#endregion
#if DebugTest
		public static void DebugTest_ClearCreatorList() {
			creatorList.Clear();
		}
#endif
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void ModelApplicationCreatorOnAddNodeFromXml(ConvertXmlParameters parameters);
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate ModelNode ModelNodeCreatorMethod(ModelNodeInfo nodeInfo, string nodeId);
	public interface IModelNodeUpdater<T> where T : IModelNode {
		void UpdateNode(T node, IModelApplication application);
	}
	internal interface IModelNodeUpdater {
		void UpdateNode(IModelNode node, IModelApplication application);
	}
	public interface IModelNodeUpdaterRegistrator {
		void AddUpdater<T>(IModelNodeUpdater<T> updater) where T : IModelNode;
	}
	public interface IModelNodeValidator<T> where T : IModelNode {
		bool IsValid(T node, IModelApplication application);
	}
	internal interface IModelNodeValidator {
		bool IsValid(IModelNode node, IModelApplication application);
	}
	public interface IModelNodeValidatorRegistrator {
		void AddValidator<T>(IModelNodeValidator<T> validator) where T : IModelNode;
	}
	public sealed class ModelNodesGeneratorUpdaters {
		static readonly ReadOnlyCollection<IModelNodesGeneratorUpdater> emptyList = new ReadOnlyCollection<IModelNodesGeneratorUpdater>(new IModelNodesGeneratorUpdater[0]);
		readonly Dictionary<Type, List<IModelNodesGeneratorUpdater>> items;
#if DebugTest
		public ModelNodesGeneratorUpdaters(int dummyToAvoidConflictWithRealConstructor)
			: this() {
		}
		public Dictionary<Type, List<IModelNodesGeneratorUpdater>> Items { get { return items; } }
#endif
		internal ModelNodesGeneratorUpdaters() {
			items = new Dictionary<Type, List<IModelNodesGeneratorUpdater>>();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int GeneratatorCount { get { return items.Count; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ReadOnlyCollection<IModelNodesGeneratorUpdater> this[Type generatorType] {
			get {
				Guard.ArgumentNotNull(generatorType, "generatorType");
				List<IModelNodesGeneratorUpdater> result;
				return items.TryGetValue(generatorType, out result) ? result.AsReadOnly() : emptyList;
			}
		}
		public void Add(IModelNodesGeneratorUpdater updater) {
			Guard.ArgumentNotNull(updater, "updater");
			List<IModelNodesGeneratorUpdater> updaters;
			if(!items.TryGetValue(updater.GeneratorType, out updaters)) {
				updaters = new List<IModelNodesGeneratorUpdater>();
				items[updater.GeneratorType] = updaters;
			}
			if(!updaters.Contains(updater)) {
				updaters.Add(updater);
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class SortChildNodesHelper {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static int DoSortNodesByDefault(ModelNode node1, ModelNode node2) {
			bool hasValue1 = node1.Index.HasValue;
			bool hasValue2 = node2.Index.HasValue;
			if(hasValue1 != hasValue2) {
				return hasValue1 ? -1 : 1;
			}
			int result = 0;
			if(hasValue1) {
				result = Comparer<int>.Default.Compare(node1.Index.Value, node2.Index.Value);
			}
			if(result == 0) {
				result = Comparer<string>.Default.Compare(node1.Id, node2.Id);
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static int DoSortNodesByIndex(ModelNode node1, ModelNode node2) {
			bool hasValue1 = node1.HasValue(ModelValueNames.Index) && node1.Index.HasValue;
			bool hasValue2 = node2.HasValue(ModelValueNames.Index) && node2.Index.HasValue;
			if(hasValue1 != hasValue2) {
				return hasValue1 ? -1 : 1;
			}
			if(hasValue1 && hasValue2) {
				return Comparer<int>.Default.Compare(node1.Index.Value, node2.Index.Value);
			}
			List<ModelNode> unsortedNodes = node1.Parent.GetUnsortedChildren();
			return Comparer<int>.Default.Compare(unsortedNodes.IndexOf(node1), unsortedNodes.IndexOf(node2));
		}
	}
	static class GlobalModelAssemblyRegistry {
		#region RegistryEntry
		sealed class RegistryEntry {
			readonly HashSet<Type> customInterfacesToGenerate;
			readonly ClassDescriptionCollection classDescriptions;
			readonly CustomLogics customLogics;
			readonly ModelInterfaceExtenders interfaceExtenders;
			readonly string assemblyFileAbsolutePath;
			readonly Assembly assembly;
			public RegistryEntry(ICollection<Type> customInterfacesToGenerate, ClassDescriptionCollection classDescriptions, CustomLogics customLogics, ModelInterfaceExtenders interfaceExtenders, string assemblyFileAbsolutePath, Assembly assembly) {
				Guard.ArgumentNotNull(customInterfacesToGenerate, "customInterfacesToGenerate");
				Guard.ArgumentNotNull(classDescriptions, "classDescriptions");
				Guard.ArgumentNotNull(customLogics, "customLogics");
				Guard.ArgumentNotNull(interfaceExtenders, "interfaceExtenders");
				Guard.ArgumentNotNull(assembly, "assembly");
				this.customInterfacesToGenerate = new HashSet<Type>(customInterfacesToGenerate);
				this.classDescriptions = classDescriptions.Clone();
				this.customLogics = customLogics.Clone();
				this.interfaceExtenders = interfaceExtenders.Clone();
				this.assemblyFileAbsolutePath = assemblyFileAbsolutePath;
				this.assembly = assembly;
			}
			public bool Equals(ICollection<Type> customInterfacesToGenerate, ClassDescriptionCollection classDescriptions, CustomLogics customLogics, ModelInterfaceExtenders interfaceExtenders, string assemblyFileAbsolutePath) {
				Guard.ArgumentNotNull(customInterfacesToGenerate, "customInterfacesToGenerate");
				Guard.ArgumentNotNull(classDescriptions, "classDescriptions");
				Guard.ArgumentNotNull(customLogics, "customLogics");
				Guard.ArgumentNotNull(interfaceExtenders, "interfaceExtenders");
				if(customInterfacesToGenerate.Count != this.customInterfacesToGenerate.Count) {
					return false;
				}
				foreach(Type customInterface in customInterfacesToGenerate) {
					if(!this.customInterfacesToGenerate.Contains(customInterface)) {
						return false;
					}
				}
				return this.classDescriptions.Equals(classDescriptions)
					&& this.customLogics.Equals(customLogics)
					&& this.interfaceExtenders.Equals(interfaceExtenders)
					&& this.assemblyFileAbsolutePath == assemblyFileAbsolutePath;
			}
			public Assembly Assembly { get { return assembly; } }
		}
		#endregion
		readonly static ICollection<RegistryEntry> cache = new List<RegistryEntry>();
		public static bool DebugMode { get; set; }
		public static Assembly Get(ICollection<Type> customInterfacesToGenerate, ClassDescriptionCollection classDescriptions, CustomLogics customLogics, ModelInterfaceExtenders interfaceExtenders, string assemblyFileAbsolutePath) {
			lock(cache) {
				Assembly modelAssembly;
				if(!TryGet(customInterfacesToGenerate, classDescriptions, customLogics, interfaceExtenders, assemblyFileAbsolutePath, out modelAssembly)) {
					modelAssembly = CompileModelAssembly(customInterfacesToGenerate, classDescriptions, customLogics, interfaceExtenders, assemblyFileAbsolutePath);
					Add(customInterfacesToGenerate, classDescriptions, customLogics, interfaceExtenders, assemblyFileAbsolutePath, modelAssembly);
				}
				return modelAssembly;
			}
		}
		private static bool TryGet(ICollection<Type> customInterfacesToGenerate, ClassDescriptionCollection classDescriptions, CustomLogics customLogics, ModelInterfaceExtenders interfaceExtenders, string assemblyFileAbsolutePath, out Assembly assembly) {
			foreach(RegistryEntry entry in cache) {
				if(entry.Equals(customInterfacesToGenerate, classDescriptions, customLogics, interfaceExtenders, assemblyFileAbsolutePath)) {
					assembly = entry.Assembly;
					return true;
				}
			}
			assembly = null;
			return false;
		}
		private static void Add(ICollection<Type> customInterfacesToGenerate, ClassDescriptionCollection classDescriptions, CustomLogics customLogics, ModelInterfaceExtenders interfaceExtenders, string assemblyFileAbsolutePath, Assembly assembly) {
			RegistryEntry entry = new RegistryEntry(customInterfacesToGenerate, classDescriptions, customLogics, interfaceExtenders, assemblyFileAbsolutePath, assembly);
			cache.Add(entry);
		}
		private static Assembly CompileModelAssembly(ICollection<Type> customInterfacesToGenerate, ClassDescriptionCollection classDescriptions, CustomLogics customLogics, ModelInterfaceExtenders interfaceExtenders, string assemblyFileAbsolutePath) {
			ModelApplicationCodeGenerator codeGenerator = new ModelApplicationCodeGenerator(customInterfacesToGenerate, classDescriptions, customLogics, interfaceExtenders);
			String[] references;
			String source = codeGenerator.GenerateCode(out references);
			CSCodeCompiler compiler = new CSCodeCompiler();
			compiler.IsDebug = DebugMode;
			return compiler.Compile(source, references, assemblyFileAbsolutePath);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelNodesComparer : IComparer<IModelNode>, IComparer<IModelColumn> {
		protected virtual bool ShouldCompareByIndex() {
			return true;
		}
		protected virtual string GetModelNodeDisplayValue(IModelNode node) {
			return ((ModelNode)node).Id;
		}
		protected virtual int? GetModelNodeIndex(IModelNode node) {
			return node.Index;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual int Compare(IModelNode node1, IModelNode node2) {
			int result = 0;
			if(ShouldCompareByIndex()) {
				int? index1 = GetModelNodeIndex(node1);
				int? index2 = GetModelNodeIndex(node2);
				if(index1 < 0 || index2 < 0) {
					if(!(index1 < 0 && index2 < 0)) {
						return index1 < 0 ? 1 : -1;
					}
				}
				bool hasValue1 = index1.HasValue;
				if(hasValue1 != index2.HasValue) {
					return hasValue1 ? -1 : 1;
				}
				if(hasValue1) {
					result = Comparer<int?>.Default.Compare(index1.Value, index2.Value);
				}
				if(index1 < 0 && index2 < 0) {
					result = result * -1;
				}
				else {
					if(result == -1) {
						if(index1 < -1 && index2 > -1) {
							result = 1;
						}
					}
					else {
						if(result == 1) {
							if(index1 > -1 && index2 < -1) {
								result = -1;
							}
						}
					}
				}
			}
			if(result == 0) {
				result = Comparer<string>.Default.Compare(GetModelNodeDisplayValue(node1), GetModelNodeDisplayValue(node2));
			}
			return result;
		}
		int IComparer<IModelColumn>.Compare(IModelColumn node1, IModelColumn node2) {
			return Compare(node1, node2);
		}
	}
}

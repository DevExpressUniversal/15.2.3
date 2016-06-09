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
using System.IO;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using DevExpress.XtraReports.UI;
using System.Data;
using DevExpress.XtraReports.Native;
using System.Collections.Generic;
using DevExpress.Serialization.Services;
using DevExpress.XtraReports.Wizards.Builder;
namespace DevExpress.XtraReports.Serialization
{
	public class XRCodeDOMGenerator {
		#region static
		static protected ArrayList classNames = null;
		static private CodeGeneratorOptions codeGeneratorOptions = null;
		static void ClearStaticResources() {
			classNames.Clear();
		}
		static Native.ReferenceCollection references = new Native.ReferenceCollection();
		static readonly HashSet<Assembly> referencedAssemblies = new HashSet<Assembly>();
		static readonly HashSet<string> xtraReportsRefs = new HashSet<string>(CompilerReferences.XtraReportsRefs);
		public static XRCodeDOMGenerator Default { get { return new XRCodeDOMGenerator(); } }
		const string subReportsLayout = "LoadSubreportsLayout";
		public const string ObjectDataSourceTypeName = "DevExpress.DataAccess.ObjectBinding.ObjectDataSource";
		public const string EFDataSourceTypeName = "DevExpress.DataAccess.EntityFramework.EFDataSource";
		static void WriteStreamToWriter(TextWriter writer, Stream stream) {
			stream.Position = 0;
			StreamReader streamReader = new StreamReader(stream);
			try {
				string source = streamReader.ReadToEnd();
				writer.Write(source);
			} finally {
				streamReader.Close();
			}
		}
		static void AddTypeReferences(Type type) {
			if(type != null && !TypeExistsInReferencedAssemblies(type))
				AddAssemblyReferences(type.Assembly);
		}
		static void AddAssemblyReferences(Assembly assembly) {
			if(xtraReportsRefs.Contains(assembly.Location) || referencedAssemblies.Contains(assembly))
				return;
			referencedAssemblies.Add(assembly);
			references.Add(assembly.Location);
			AssemblyName[] referencedAssemblies1 = assembly.GetReferencedAssemblies();
			foreach(AssemblyName assemblyName in referencedAssemblies1) {
				try {
					Assembly loadedAssembly = Assembly.Load(assemblyName);
					AddAssemblyReferences(loadedAssembly);
				} catch { }
			}
		}
		static bool TypeExistsInReferencedAssemblies(Type type) {
			foreach(Assembly assembly in referencedAssemblies) {
				if(assembly.GetType(type.FullName) != null)
					return true;
			}
			return false;
		}
		static CodeMemberMethod CreateMethod(string name) {
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = name;
			method.ReturnType = new CodeTypeReference("System.Void");
			return method;
		}
		static XRCodeDOMGenerator() {
			codeGeneratorOptions = new CodeGeneratorOptions();
			codeGeneratorOptions.BlankLinesBetweenMembers = false;
			classNames = new ArrayList();
		}
		#endregion
		CodeDomProvider codeProvider;
		CodeNamespace codeNamespace;
		IList components;
		IComponent rootComponent;
		Hashtable componentSites = new Hashtable();
		Type baseType = typeof(DevExpress.XtraReports.UI.XtraReport);
		Hashtable dataSetsHT = new Hashtable();
		Hashtable subreportsHT = new Hashtable();
		Hashtable compNamesHT;
		IDesignerSerializationProvider dataSetSerializationProvider = new DevExpress.XtraReports.Native.DataSetSerializationProvider();
		bool throwOnError;
		IServiceProvider nativeServiceProvider;
		bool isSubreport;
		Dictionary<string, string> resourceStorage;
		Dictionary<string, string> ResourceStorage {
			get {
				if(resourceStorage == null)
					resourceStorage = new Dictionary<string, string>();
				return resourceStorage;
			}
		}
		public XRCodeDOMGenerator() { 
		}
		public XRCodeDOMGenerator(CodeDomProvider codeProvider) {
			this.codeProvider = codeProvider;
		}
		public XRCodeDOMGenerator(XtraReport rootComponent, IList components, CodeDomProvider codeProvider, bool throwOnError, IServiceProvider nativeServiceProvider, Dictionary<string, string> resourceDictionary, bool isSubreport)
			: this(rootComponent, components, codeProvider, throwOnError) {
			this.nativeServiceProvider = nativeServiceProvider;
			this.isSubreport = isSubreport;
			this.resourceStorage = resourceDictionary;
		}
		public XRCodeDOMGenerator(XtraReport rootComponent, IList components, CodeDomProvider codeProvider, bool throwOnError)
			: this(rootComponent, components, codeProvider) {
			this.throwOnError = throwOnError;
			this.compNamesHT = rootComponent.Site == null ? rootComponent.GetComponentNamesHT(this.components) : null;
		}
		public XRCodeDOMGenerator(IComponent rootComponent, IList components, CodeDomProvider codeProvider)
			: this(codeProvider) {
			this.rootComponent = rootComponent;
			this.components = FilterNonSerializableComponents(components);
		}
		static IList FilterNonSerializableComponents(IList components) {
			ArrayList result = new ArrayList();
			foreach(object component in components) {
				if(CanBeSerialized(component))
					result.Add(component);
			}
			return result;
		}
		static bool CanBeSerialized(object obj) {
			IComponent component = obj as IComponent;
			if(component == null)
				return false;
			TypeConverter typeConverter = TypeDescriptor.GetConverter(component);
			bool canConvertTo = false;
			try {
				canConvertTo = typeConverter.CanConvertTo(null, typeof(InstanceDescriptor));
			} catch {
			}
			return canConvertTo ? true : HasComponentDefaultConstructor(component);
		}
		static bool HasComponentDefaultConstructor(IComponent component) {
			return component.GetType().GetConstructor(Type.EmptyTypes) != null;
		}
		void OnCodeGenerated(Stream stream) {
			XRTypeInfo.Serialize(rootComponent.GetType(), ((XtraReport)rootComponent).Version, references.ToStringArray(), ResourceStorage, stream);
			referencedAssemblies.Clear();
			references.Clear();
		}
		public string CodeNamespaceName {
			get { return XtraReport.SerializationNamespace; }
		}
		protected void GenerateCodeInternal(TextWriter writer, string codeNamespaceName, string rootClassName)	{
			XRCodeDomDesignerSerializationManager serializationManager = new XRCodeDomDesignerSerializationManager(new SDesignerHost(rootComponent), isSubreport);
			serializationManager.AddSerializationProvider(dataSetSerializationProvider);
			try {
				codeNamespace = new CodeNamespace(codeNamespaceName);
				SaveComponentSites();
				serializationManager.Initialize(GetNativeServiceProvider(), GetRootNativeServiceProvider());
				AddComponentsToContainer(serializationManager);
				ProcessDataSets();
				SetSerializationNames(serializationManager);
				RetriveComponentReferences();
				string fullRootClassName;
				if(!String.IsNullOrEmpty(rootClassName)) {
					serializationManager.SetName(rootComponent, rootClassName);
					fullRootClassName = string.Format("{0}.{1}", codeNamespace.Name, rootClassName);
				} else {
					fullRootClassName = string.Format("{0}.{1}", codeNamespace.Name, rootComponent.Site.Name);
				}
				classNames.Add(fullRootClassName);
				CodeDomSerializer rootSerializer = serializationManager.GetRootSerializer(rootComponent.GetType());
				System.Diagnostics.Debug.Assert(rootSerializer != null);
				CodeTypeDeclaration codeTypeDeclaration = (CodeTypeDeclaration)rootSerializer.Serialize(serializationManager, rootComponent);
				XRDesignerResourceService designerResourceService = (XRDesignerResourceService)serializationManager.GetService(typeof(IResourceService));
				SerializeEvents(designerResourceService);
				serializationManager.OnSerializationComplete();
				string resources = designerResourceService.SerializationEnded(true);
				ResourceStorage.Add(fullRootClassName, resources);
				AddCodeConstructor(codeTypeDeclaration);
				GenerateSubreportsLayoutMethod(codeTypeDeclaration);
				codeNamespace.Types.Add(codeTypeDeclaration);
				CorrectResourceDeclaration(codeNamespace, codeTypeDeclaration, !string.IsNullOrEmpty(resources) ? fullRootClassName : null);
				CorrectBaseTypeDeclaration(codeTypeDeclaration);
				StreamWriter dataSetWriter = new StreamWriter(new MemoryStream());
				GenerateDataSetClass(dataSetWriter);
				dataSetWriter.Flush();
				try {
					CorrectDataSetDeclarations(codeTypeDeclaration);
					GenerateCodeFromNamespace(codeNamespace, writer);
					WriteStreamToWriter(writer, dataSetWriter.BaseStream);
				} finally {
					dataSetWriter.Close();
				}
				GenerateSubreportCode(writer);
			} catch {
				if(throwOnError)
					throw;
			} finally {
				serializationManager.RemoveSerializationProvider(dataSetSerializationProvider);
				serializationManager.Dispose();
				RestoreComponentSites();
			}
		}
		void RetriveComponentReferences() {
			IServiceProvider provider = GetNativeServiceProvider();
			if(provider != null) {
				IDesignerHost host = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				if(host != null)
					AddTypeReferences(host.GetType(host.RootComponentClassName));
			}
			string xrAssemblyName = baseType.Assembly.FullName;
			foreach(IComponent component in components) {
				if(component.GetType().Assembly.FullName != xrAssemblyName)
					AddTypeReferences(component.GetType());
				if(component.GetType().FullName == ObjectDataSourceTypeName)
					AddObjectDataSourceReferences(component);
				if(component.GetType().FullName == EFDataSourceTypeName) {
					IApplicationPathService serv = provider != null ? provider.GetService(typeof(IApplicationPathService)) as IApplicationPathService : null;
					string path = IApplicationPathServiceExtensions.GetFullPath(serv ?? new DefaultApplicationPathService(), "EntityFramework.dll");
					if(!string.IsNullOrEmpty(path)) references.Add(path);
				}
			}
		}
		void AddObjectDataSourceReferences(IComponent component) {
			AddTypeReferences(((DevExpress.DataAccess.ObjectBinding.ObjectDataSource)component).DataSource as Type);
		}
		void SerializeEvents(IResourceService resourceService) {
			System.Diagnostics.Debug.Assert(resourceService != null);
			System.Resources.IResourceWriter resourceWriter = resourceService.GetResourceWriter(null);
			System.Diagnostics.Debug.Assert(resourceWriter != null);
			string s = new EventSerializer().Serialize((XRControl)rootComponent);
			if(s.Length > 0)
				resourceWriter.AddResource(EventSerializer.ResourceName, s);
			((Serialization.IXRSerializable)rootComponent).SerializeProperties(new Serialization.XRSerializer(new Serialization.ResourceSerializationInfo(resourceWriter)));
		}
		protected ISite CreateSite(IComponent component, string name, IContainer container, XRCodeDomDesignerSerializationManager serializationManager) {
			return new XRDesignSite(component, name, container, serializationManager);
		}
		protected Type GetResourceManagerType() {
			return typeof(XRResourceManager);
		}
		void ProcessDataSets() {
			foreach(IComponent component in components) {
				if(component.GetType().Equals(typeof(System.Data.DataSet))) {
					System.Data.DataSet dataSet = (System.Data.DataSet)component;
					try {
						dataSet.DataSetName = component.Site.Name;
					} catch { }
					dataSetsHT[component.Site.Name] = dataSet.DataSetName;
				}
			}
		}
		void GenerateSubreportsLayoutMethod(CodeTypeDeclaration codeTypeDeclaration) {
			CodeMemberMethod loadSubreportsLayoutMethod = CreateMethod(subReportsLayout);
			foreach (IComponent component in components)
				if(component is SubreportBase) {
					SubreportBase subreport = (SubreportBase)component;
					XtraReport reportSource = subreport.ReportSource;
					if(ShouldSerialize(reportSource)) {
						string typeName = GetSubreportLayoutClassName(reportSource);
						int index = typeName.LastIndexOf(".");
						subreportsHT[component] = index >=0 ? typeName.Substring(index + 1) : typeName;
						CodeFieldReferenceExpression subReportField = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), component.Site.Name);
						string asmName = reportSource.GetType().Assembly.GetName().Name;
						CodeMethodInvokeExpression method = new CodeMethodInvokeExpression(subReportField, "LoadSubreportLayout", 
							new CodePrimitiveExpression(reportSource.GetType().FullName + "," + asmName), new CodeObjectCreateExpression(typeName));
						loadSubreportsLayoutMethod.Statements.Add(method);
					}
				}
			if(loadSubreportsLayoutMethod.Statements.Count > 0) {
				codeTypeDeclaration.Members.Add(loadSubreportsLayoutMethod);
				ForContructorStatements(codeTypeDeclaration, statements => statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), subReportsLayout))));
			}
		}
		static void ForContructorStatements(CodeTypeDeclaration codeTypeDeclaration, Action<CodeStatementCollection> action) {
			foreach(CodeTypeMember codeTypeMember in codeTypeDeclaration.Members) {
				CodeConstructor codeConstructor = codeTypeMember as CodeConstructor;
				if(codeConstructor != null) {
					action(codeConstructor.Statements);
				}
			}
		}
		string GetSubreportLayoutClassName(XtraReport report) {
			if(report.GetType().Equals(typeof(XtraReport))) {
				string name = CodeNamespaceName + "." + report.Site.Name;
				return !classNames.Contains(name) ? name : CodeNamespaceName + "." + GenerateSubreportLayoutClassName();
			}
			return report.GetType().FullName;
		}
		static string GenerateSubreportLayoutClassName() {
			string classNameBase = "SubreportLayout";
			int i = 1;
			for(;classNames.Contains(classNameBase + i); i++);
			return classNameBase + i;
		}
		protected string GetSubreportCodeNamespaceName(object report) {
			return report.GetType().Equals(typeof(XtraReport)) ? CodeNamespaceName : report.GetType().Namespace;
		}
		private string GetSubreportClassName(SubreportBase subreport) {
			string className = subreportsHT[subreport] as string;
			return className != null ? className : subreport.ReportSource != null ?  subreport.ReportSource.GetType().Name : null;
		}
		void GenerateSubreportCode(TextWriter writer) {
			foreach (IComponent component in components)  {
				if(component is SubreportBase) {
					XtraReport report = ((SubreportBase)component).ReportSource;
					string rootClassName = GetSubreportClassName((SubreportBase)component);
					if(ShouldSerialize(report) && ShouldGenerateCodeForType(rootClassName)) {
						string codeNamespaceName = GetSubreportCodeNamespaceName(report);
						new XRCodeDOMGenerator(report, report.GetReportComponents(), new CSharpCodeProvider(), throwOnError, GetRootNativeServiceProvider(), ResourceStorage, true)
							.GenerateCodeInternal(writer, codeNamespaceName, rootClassName);
					}
				}
			}
		}
		bool ShouldSerialize(IComponent  component) {
			return component != null && this.components.Contains(component);
		}
		void GenerateDataSetClass(TextWriter writer) {
			foreach(IComponent component in components)  {
				DataSet dataSet = component as DataSet;
				if(dataSet != null && ShouldGenerateCodeForDataSet(dataSet.GetType())) {
					CodeNamespace dataSetNamespace = new CodeNamespace(CodeNamespaceName);
					dataSet.DataSetName = dataSet.GetType().Equals(typeof(DataSet)) ?
						CorrectDataSetName(dataSet.DataSetName, "DataSet") :
						dataSet.GetType().Name;
					string name = System.Data.Design.TypedDataSetGenerator.Generate((DataSet)component, dataSetNamespace, codeProvider);
					dataSetsHT[component.Site.Name] = name;
					GenerateCodeFromNamespace(dataSetNamespace, writer);
				}
			}
		}
		static string CorrectDataSetName(string dataSetName, string postfix) {
			return dataSetName.EndsWith(postfix, StringComparison.OrdinalIgnoreCase) ? dataSetName : dataSetName + postfix;
		}
		bool ShouldGenerateCodeForDataSet(Type dataSetType) {
			return dataSetType.Equals(typeof(DataSet)) || (ShouldGenerateCodeForType(dataSetType.Name) && dataSetType.Namespace == CodeNamespaceName);
		}
 		protected IServiceProvider GetRootNativeServiceProvider() {
			return GetNativeServiceProvider() ?? nativeServiceProvider;
		}
		protected string GetName(IComponent component, ComponentCollection components, XRNameCreationService nameCreationService) {
			ISite site = (ISite)componentSites[component];
			if(site != null && components[site.Name] == null && !String.IsNullOrEmpty(site.Name))
				return site.Name;
			if(compNamesHT != null && compNamesHT.ContainsKey(component))
				return (string)compNamesHT[component];
			if(component == rootComponent) {
				string name = GetRootComponentName(component);
				if(IsValidName(name,components))
					return name;
			}
			return GetNameByProperty(component, components, nameCreationService);
		}
		static string GetRootComponentName(IComponent component) {
			string name = GetNameByProperty(component);
			if(String.IsNullOrEmpty(name))
				name = component.GetType().Name;
			return name;
		}
		void CorrectDataSetDeclarations(CodeTypeDeclaration codeTypeDeclaration) {
			CodeMemberMethod codeMethod = FindMethod(codeTypeDeclaration, "InitializeComponent");
			for(int i=0; i < codeMethod.Statements.Count; i++) {
				if(codeMethod.Statements[i] is CodeAssignStatement) {
					CodeAssignStatement codeAssignStatament = codeMethod.Statements[i] as CodeAssignStatement;
					if(codeAssignStatament.Right is CodeObjectCreateExpression) {
						CodeObjectCreateExpression codeObjectCreateExpression = codeAssignStatament.Right as CodeObjectCreateExpression;
						if(codeObjectCreateExpression.CreateType.BaseType.Equals("System.Data.DataSet")) {
							CodeFieldReferenceExpression codeFieldReferenceExpression = codeAssignStatament.Left as CodeFieldReferenceExpression;
							string baseType = String.Format("{0}.{1}", CodeNamespaceName, (string)dataSetsHT[codeFieldReferenceExpression.FieldName]);
							codeObjectCreateExpression.CreateType.BaseType = baseType;
						}
					}
				}
			}
		}
		public void GenerateCode(Stream stream) {
			StreamWriter streamWriter = new StreamWriter(new MemoryStream());
			try {
				GenerateCodeInternal(streamWriter);
				streamWriter.Flush();
			}
			finally {
				OnCodeGenerated(stream);
				CopyStream(streamWriter.BaseStream, stream);
				streamWriter.Close();
				ClearStaticResources();
			}
		}
		public void GenerateCode(string fileName) {
			FileStream fs = File.Create(fileName);
			try {
				GenerateCode(fs);
			} finally {
				fs.Close();
			}
		}
		static void CopyStream(Stream src, Stream dest) {
			src.Position = 0;
			byte[] buffer = new byte[src.Length];
			src.Read(buffer, 0, (int)src.Length);
			dest.Write(buffer, 0, (int)src.Length);
		}
		void GenerateCodeInternal(TextWriter writer) {
			GenerateCodeInternal(writer, CodeNamespaceName, String.Empty);
		}
		protected void SaveComponentSites() {
			foreach(IComponent component in components) {
				componentSites[component] = component.Site;
			}
		}
		protected void RestoreComponentSites() {
			foreach(IComponent component in components) {
				component.Site = (ISite)componentSites[component];
			}
		}
		protected IServiceProvider GetNativeServiceProvider() {
			return !(componentSites[rootComponent] is XRDesignSite) ? (IServiceProvider)componentSites[rootComponent] : null;
		}
		protected void GenerateCodeFromNamespace(CodeNamespace codeNamespace, TextWriter writer) {
			codeProvider.GenerateCodeFromNamespace(codeNamespace, writer, codeGeneratorOptions);
		}
		protected void AddComponentsToContainer(XRCodeDomDesignerSerializationManager serializationManager) {
			IContainer container = new SContainer();
			XRNameCreationService nameCreationService = (XRNameCreationService)serializationManager.GetService(typeof(INameCreationService));
			foreach(IComponent component in components)  {
				string name = GetName(component, container.Components, nameCreationService);
				try {
					component.Site = CreateSite(component, name, container, serializationManager);
				}
				catch { }
				container.Add(component);
			}
		}
		protected void SetSerializationNames(XRCodeDomDesignerSerializationManager serializationManager) {
			foreach(IComponent component in components) {
				serializationManager.SetName(component, component.Site.Name);
			}
		}
		protected static void AddCodeConstructor(CodeTypeDeclaration codeTypeDeclaration) {
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			codeConstructor.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "InitializeComponent")));
			codeTypeDeclaration.Members.Add(codeConstructor);
		}
		protected void CorrectBaseTypeDeclaration(CodeTypeDeclaration codeTypeDeclaration ) {
			codeTypeDeclaration.BaseTypes.Clear();
			codeTypeDeclaration.BaseTypes.Add(baseType);
		}
		protected static bool ShouldGenerateCodeForType(string typeName) {
			if(!classNames.Contains(typeName)) {
				classNames.Add(typeName);
				return true;
			}
			return false;
		}
		protected static string GetNameByProperty(IComponent component, ComponentCollection components, XRNameCreationService nameCreationService) {
			string name = GetNameByProperty(component);
			if(IsValidName(name,components))
					return name;
			return nameCreationService.CreateNameByType(components, component.GetType());
		}
		protected static bool IsValidName(string name, ComponentCollection components) {
			return !XRNameCreationService.HasWrongCharacters(name) && components[name] == null;
			}
		protected static string GetNameByProperty(IComponent component) {
			PropertyDescriptor pdName = TypeDescriptor.GetProperties(component)["Name"];
			return pdName != null ? (string)pdName.GetValue(component) : null;
		}
		protected void CorrectResourceDeclaration(CodeNamespace codeNamespace, CodeTypeDeclaration codeTypeDeclaration, string fullRootClassName) {
			if(string.IsNullOrEmpty(fullRootClassName)) return;
			CodeMemberMethod codeMethod = FindMethod(codeTypeDeclaration, "InitializeComponent");
			CodeStatement codeStatement = FindVariableDeclaration(codeMethod.Statements, "resources");
			if(codeStatement != null)
				codeMethod.Statements.Remove(codeStatement);
			codeTypeDeclaration.Members.Add(new CodeMemberField("System.Resources.ResourceManager", "_resources"));
			codeTypeDeclaration.Members.Add(new CodeMemberField(typeof(string), "_resourceString"));
			CodeMemberProperty prop = CreateProperty(codeTypeDeclaration, "resources", "System.Resources.ResourceManager");
			codeTypeDeclaration.Members.Add(prop);
			CodeStatement ifStatement = new CodeConditionStatement(new CodeVariableReferenceExpression("_resources == null"), GetIfStatements(fullRootClassName));
			prop.GetStatements.Add(ifStatement);
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_resources")));
			var getResourceExpression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(GetResourceManagerType()), "GetResourceFor", new CodePrimitiveExpression(fullRootClassName));
			ForContructorStatements(codeTypeDeclaration, statements => statements.Insert(0, new CodeAssignStatement(GetResourceStringFieldReference(), getResourceExpression)));
		}
		static CodeExpression GetResourceStringFieldReference() {
			return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_resourceString");
		}
		CodeStatement[] GetIfStatements(string fullRootClassName) {
			List<CodeStatement> result = new List<CodeStatement>();
			var createResourceManagerExpression = new CodeObjectCreateExpression(GetResourceManagerType(), GetResourceStringFieldReference());
			result.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_resources"), createResourceManagerExpression));
			return result.ToArray();
		}
		static CodeMemberProperty CreateProperty(CodeTypeDeclaration codeTypeDeclaration, string name, string typeName) {
			CodeMemberProperty prop = new CodeMemberProperty();
			prop.HasGet = true;
			prop.HasSet = false;
			prop.Type = new CodeTypeReference(typeName);
			prop.Name = name;
			return prop;
		}
		static CodeStatement FindVariableDeclaration(System.CodeDom.CodeStatementCollection statementCollection, string varName) {
			for(int i=0; i < statementCollection.Count; i++) {
				if(statementCollection[i] is CodeVariableDeclarationStatement && 
					((CodeVariableDeclarationStatement)statementCollection[i]).Name == varName) {
					return statementCollection[i];
				}
			}
			return null;
		}
		protected static CodeMemberMethod FindMethod(CodeTypeDeclaration codeTypeDeclaration, string methodName) {
			foreach(CodeTypeMember codeTypeMember in codeTypeDeclaration.Members) {
				if(codeTypeMember is CodeMemberMethod && codeTypeMember.Name == methodName) {
					return codeTypeMember as CodeMemberMethod;
				}
			}
			return null;
		}
	}
}

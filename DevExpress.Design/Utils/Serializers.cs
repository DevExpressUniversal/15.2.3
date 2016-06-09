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
using System.CodeDom;
using System.Collections;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Reflection;
using System.Resources;
using DevExpress.Utils.Design;
using EnvDTE;
namespace DevExpress.Utils.Serializers
{
	public class HashCodeDomSerializer : CodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			ICollection levels = value as ICollection;
			if(levels == null || levels.Count == 0) return null;
			object stackVal = manager.Context.Current;
			if(stackVal == null) return null;
#if DXWhidbey
			if(stackVal is System.ComponentModel.Design.Serialization.ExpressionContext) {
				stackVal = ((System.ComponentModel.Design.Serialization.ExpressionContext)stackVal).Expression;
			}
#endif
			CodeExpression retExp = null;
			if(stackVal is CodePropertyReferenceExpression) {
				retExp = stackVal as CodePropertyReferenceExpression;
			} else {
				if(stackVal.GetType().Name == "CodeValueExpression") {
					retExp = stackVal.GetType().InvokeMember("Expression", System.Reflection.BindingFlags.GetProperty, null, stackVal, null) as CodeExpression;
				}
			}
			if(retExp == null) return null;
			CodeStatementCollection cs = new CodeStatementCollection();
			CodeExpression ce = new CodeExpression();
			CodeMethodReferenceExpression met = new CodeMethodReferenceExpression(retExp, AddMethodName);
			foreach(DictionaryEntry entry in levels) {
				if(!IsNeedSerializeEntry(value, entry)) continue;
				CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression();
				invoke.Method = met;
				invoke.Parameters.Add(SerializeToExpression(manager, entry.Key));
				invoke.Parameters.Add(SerializeToExpression(manager, entry.Value));
				cs.Add(invoke);
			}
			return cs;
		}
		public override object Deserialize(IDesignerSerializationManager manager,object codeObject) {
			return null;
		}
		protected virtual string AddMethodName { get { return "Add"; } 
		}
		protected virtual bool IsNeedSerializeEntry(object value, DictionaryEntry entry) {
			return true;
		}
	}
	public sealed class ViewStylesCodeDomSerializer : DevExpress.Utils.Serializers.HashCodeDomSerializer {
		protected override bool IsNeedSerializeEntry(object value, DictionaryEntry entry) {
			if(entry.Key != null && entry.Value != null && entry.Key.Equals(entry.Value)) return false;
			return true;
		}
		protected override string AddMethodName { get { return "AddReplace"; } 
		}
	}
	#region ComponentCodeDomeSerializer
	public abstract class ComponentCodeDomeSerializer : TypeCodeDomSerializer {
		const string appGlobalResources = "App_GlobalResources";
		const string resMethodName = "GetResourceManager";
		const string fullPathPropertyName = "FullPath";
		Type type;
		#region inner classes
		class App_GlobalResourcesResolver : IDisposable {
			IServiceProvider serviceProvider;
			public App_GlobalResourcesResolver(IServiceProvider serviceProvider) {
				this.serviceProvider = serviceProvider;
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			}
			Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
				if(args.Name.StartsWith(appGlobalResources, StringComparison.OrdinalIgnoreCase)) {
					ITypeResolutionService svc = serviceProvider.GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
					if(svc != null)
						return svc.GetAssembly(new AssemblyName(args.Name), false);
				}
				return null;
			}
			public void Dispose() {
				AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
			}
		}
		#endregion
		public ComponentCodeDomeSerializer(Type type) {
			this.type = type;
		}
		public override object Deserialize(IDesignerSerializationManager manager, CodeTypeDeclaration codeTypeDeclaration) {
			TypeCodeDomSerializer serializer = GetSerializer(manager);
			if(serializer == null) return null;
			if(!ProjectHelper.IsWebProject(manager))
				return serializer.Deserialize(manager, codeTypeDeclaration);
			ProjectItem projectItem = GetProjectItem(manager);
			string filePath = GetResxFileName(codeTypeDeclaration);
			if(filePath == null || filePath.Length == 0)
				filePath = GetResxFileName(projectItem);
			filePath = Path.Combine(appGlobalResources, filePath);
			filePath = Path.Combine(DTEHelper.GetPropertyValue(projectItem.ContainingProject, fullPathPropertyName), filePath);
			if(!File.Exists(filePath))
				return serializer.Deserialize(manager, codeTypeDeclaration);
			IServiceContainer serviceContainer = (IServiceContainer)manager.GetService(typeof(IServiceContainer));
			IResourceService resService = (IResourceService)manager.GetService(typeof(IResourceService));
			serviceContainer.RemoveService(typeof(IResourceService));
			ComponentCodeDomeSerializerResourceService reportResService = new ComponentCodeDomeSerializerResourceService(manager, filePath);
			serviceContainer.AddService(typeof(IResourceService), reportResService);
			try {
				using(new App_GlobalResourcesResolver(serviceContainer))
					return serializer.Deserialize(manager, codeTypeDeclaration);
			} catch {
				return null;
			} finally {
				serviceContainer.RemoveService(typeof(IResourceService));
				reportResService.Dispose();
				if(resService != null)
					serviceContainer.AddService(typeof(IResourceService), resService);  
			}
		}
		TypeCodeDomSerializer GetSerializer(IDesignerSerializationManager manager) {
			return manager.GetSerializer(type.BaseType, typeof(TypeCodeDomSerializer)) as TypeCodeDomSerializer;
		}
		string GetResxFileName(CodeTypeDeclaration typeDeclaration) {
			CodeMemberMethod codeMethod = FindInitializeMethod(typeDeclaration);
			CodeVariableDeclarationStatement codeStatement = codeMethod != null ? FindVariableDeclaration(codeMethod.Statements, "resourceFileName") as CodeVariableDeclarationStatement : null;
			if(codeStatement != null) {
				CodePrimitiveExpression initExpression = codeStatement.InitExpression as CodePrimitiveExpression;
				if(initExpression != null && initExpression.Value is string)
					return (string)initExpression.Value;
			}
			return String.Empty;
		}
		ProjectItem GetProjectItem(IServiceProvider serviceProvider) {
			return (ProjectItem)serviceProvider.GetService(typeof(ProjectItem));
		}
		ProjectItem ForceResourceProjectItem(IServiceProvider serviceProvider) {
			ProjectItem projectItem = GetProjectItem(serviceProvider);
			string fileName = GetResxFileName(projectItem);
			Project project = projectItem.ContainingProject;
			ProjectItem appResourceItem = DTEHelper_GetProjectItem(project.ProjectItems, appGlobalResources);
			if(appResourceItem == null)
				appResourceItem = project.ProjectItems.AddFolder(appGlobalResources, "");
			ProjectItem resxItem = DTEHelper_GetProjectItem(appResourceItem.ProjectItems, fileName);
			if(resxItem == null) {
				fileName = Path.Combine(DTEHelper.GetPropertyValue(appResourceItem, fullPathPropertyName), fileName);
				FileStream fileStream = File.Create(fileName);
				fileStream.Close();
				resxItem = appResourceItem.ProjectItems.AddFromFile(fileName);
			}
			CloseDocument(resxItem, vsSaveChanges.vsSaveChangesNo);
			return resxItem;
		}
		string GetResxFileName(ProjectItem projectItem) {
			string fileName = DTEHelper_GetFileName(projectItem);
			return Path.ChangeExtension(fileName, "resx");
		}
		void CloseDocument(ProjectItem projectItem, vsSaveChanges saveChanges) {
			if(projectItem != null && projectItem.Document != null)
				projectItem.Document.Close(saveChanges);
		}
		public override CodeTypeDeclaration Serialize(IDesignerSerializationManager manager, object value, ICollection members) {
			TypeCodeDomSerializer serializer = GetSerializer(manager);
			if(serializer == null) return null;
			IDesignerHost host = (IDesignerHost)manager.GetService(typeof(IDesignerHost));
			if(!ProjectHelper.IsWebProject(manager))
				return serializer.Serialize(manager, value, members);
			IServiceContainer serviceContainer = (IServiceContainer)manager.GetService(typeof(IServiceContainer));
			IResourceService resService = (IResourceService)manager.GetService(typeof(IResourceService));
			serviceContainer.RemoveService(typeof(IResourceService));
			string filePath = DTEHelper.GetPropertyValue(ForceResourceProjectItem(manager), fullPathPropertyName);
			ComponentCodeDomeSerializerResourceService reportResService = new ComponentCodeDomeSerializerResourceService(manager, filePath);
			serviceContainer.AddService(typeof(IResourceService), reportResService);
			try {
				CodeTypeDeclaration codeObject = serializer.Serialize(manager, value, members);
				if(ProjectHelper.IsWebProject(manager)) {
					ProjectItem projectItem = GetProjectItem(manager);
					string fileName = DTEHelper_GetFileName(projectItem);
					CodeTypeDeclaration codeTypeDeclaration = codeObject as CodeTypeDeclaration;
					PatchWebCodeStatements(manager, codeTypeDeclaration, "Resources." + Path.GetFileNameWithoutExtension(fileName), Path.ChangeExtension(fileName, ".resx"));
				}
				return codeObject;
			} catch {
				return null;
			} finally {
				serviceContainer.RemoveService(typeof(IResourceService));
				reportResService.Dispose();
				if(resService != null)
					serviceContainer.AddService(typeof(IResourceService), resService);
			}
		}
		void PatchWebCodeStatements(IServiceProvider serviceProvider, CodeTypeDeclaration typeDeclaration, string resourceType, string resourceFileName) {
			if(typeDeclaration == null)
				return;
			CodeMemberMethod initializeMethod = FindInitializeMethod(typeDeclaration);
			CodeStatement codeStatement = FindVariableDeclaration(initializeMethod.Statements, "resources");
			if(codeStatement != null) {
				initializeMethod.Statements.Remove(codeStatement);
				CodeVariableDeclarationStatement resourceVariableDeclaration = new CodeVariableDeclarationStatement("System.Resources.ResourceManager", "resources");
				CodePropertyReferenceExpression codePropReference = new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference(resourceType, CodeTypeReferenceOptions.GlobalReference)), "ResourceManager");
				resourceVariableDeclaration.InitExpression = codePropReference;
				initializeMethod.Statements.Insert(0, resourceVariableDeclaration);
			}
			if(initializeMethod != null) {
				CodeVariableDeclarationStatement declarationStatement = new CodeVariableDeclarationStatement(typeof(string), "resourceFileName");
				declarationStatement.InitExpression = new CodePrimitiveExpression(resourceFileName);
				initializeMethod.Statements.Insert(0, declarationStatement);
			}
		}
		CodeMemberMethod FindInitializeMethod(CodeTypeDeclaration codeTypeDeclaration) {
			return FindMethod(codeTypeDeclaration, "InitializeComponent");
		}
		CodeMemberMethod FindMethod(CodeTypeDeclaration codeTypeDeclaration, string methodName) {
			foreach(CodeTypeMember codeTypeMember in codeTypeDeclaration.Members) {
				if(codeTypeMember is CodeMemberMethod && codeTypeMember.Name == methodName) {
					return codeTypeMember as CodeMemberMethod;
				}
			}
			return null;
		}
		CodeStatement FindVariableDeclaration(System.CodeDom.CodeStatementCollection statementCollection, string varName) {
			for(int i = 0; i < statementCollection.Count; i++) {
				if(statementCollection[i] is CodeVariableDeclarationStatement &&
					((CodeVariableDeclarationStatement)statementCollection[i]).Name == varName) {
					return statementCollection[i];
				}
			}
			return null;
		}
		ProjectItem DTEHelper_GetProjectItem(ProjectItems projectItems, string fileName) {
			try {
				return projectItems.Item(fileName);
			} catch { }
			return null;
		}
		string DTEHelper_GetFileName(ProjectItem projectItem) {
			return DevExpress.Utils.Design.DTEHelper.GetPropertyValue(projectItem, "FileName");
		}
	}
	public abstract class MultiTargetBuilder {
		protected readonly bool skipMultitargetService;
		protected readonly ConstructorInfo nodeCostructor;
		protected readonly Delegate typeNameConverter;
		public MultiTargetBuilder(IServiceProvider provider) {
			Type iMultitargetHelperServiceType = Type.GetType("System.ComponentModel.Design.IMultitargetHelperService, System.Design");
			if(iMultitargetHelperServiceType == null) {
				skipMultitargetService = true;
				return;
			}
			object service = provider.GetService(iMultitargetHelperServiceType);
			if(service == null) {
				skipMultitargetService = true;
				return;
			}
			MethodInfo mi = service.GetType().GetMethod("TypeNameConverter");
			Type delegateType = Type.GetType("System.Func`2[[System.Type, mscorlib],[System.String, mscorlib]], mscorlib");
			typeNameConverter = Delegate.CreateDelegate(delegateType, service, mi);
			nodeCostructor = CreateConstructor(delegateType);
		}
		protected abstract ConstructorInfo CreateConstructor(Type delegateType);
	}
	public class ComponentCodeDomeSerializerResourceService : IResourceService, IDisposable {
		class ResXResourceWriterBuilder : MultiTargetBuilder {
			public ResXResourceWriterBuilder(IServiceProvider provider)
				: base(provider) {
			}
			protected override ConstructorInfo CreateConstructor(Type delegateType) {
				return typeof(ResXResourceWriter).GetConstructor(new Type[] { typeof(string), delegateType });
			}
			public ResXResourceWriter CreateResXResourceWriter(string path) {
				if(skipMultitargetService)
					return new ResXResourceWriter(path);
				return (ResXResourceWriter)nodeCostructor.Invoke(new object[] { path, typeNameConverter });
			}
		}
		ResXResourceWriter resourceWriter;
		ResXResourceReader resourceReader;
		string fileName;
		ResXResourceWriterBuilder writerBuilder;
		public ComponentCodeDomeSerializerResourceService(IServiceProvider provider, string fileName) {
			this.fileName = fileName;
			writerBuilder = new ResXResourceWriterBuilder(provider);
		}
		#region System.ComponentModel.Design.IResourceService interface implementation
		public System.Resources.IResourceWriter GetResourceWriter(System.Globalization.CultureInfo info) {
			try {
				if(resourceWriter == null)
					resourceWriter = writerBuilder.CreateResXResourceWriter(fileName);
				return resourceWriter;
			} catch {
				return null;
			}
		}
		public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info) {
			try {
				if(resourceReader == null)
					resourceReader = new ResXResourceReader(fileName);
				return resourceReader;
			} catch {
				return null;
			}
		}
		#endregion
		public void Dispose() {
			if(resourceWriter != null) {
				resourceWriter.Dispose();
				resourceWriter = null;
			}
			writerBuilder = null;
		}
	}
	#endregion
}

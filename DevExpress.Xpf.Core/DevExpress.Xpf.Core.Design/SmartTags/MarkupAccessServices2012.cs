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
using System.ComponentModel.Design;
using DevExpress.Mvvm.Native;
#endif
namespace DevExpress.Xpf.Core.Design {
	public class XpfSceneDocumentNode : RuntimeBase<ISceneDocumentNode, object>, ISceneDocumentNode {
		public static ISceneDocumentNode FromDocumentNode(object value) {
			return value == null ? null : new XpfSceneDocumentNode(value);
		}
		public static object ToDocumentNode(ISceneDocumentNode documentNode) {
			return documentNode == null ? null : Guard.ArgumentMatchType<XpfSceneDocumentNode>(documentNode, "documentNode").Value;
		}
		static Type documentNodeType;
		static PropertyInfo parentProperty;
		static PropertyInfo sceneNodeProperty;
		static PropertyInfo typeProperty;
		static PropertyInfo sitePropertyKeyProperty;
		static PropertyInfo propertiesProperty;
		protected XpfSceneDocumentNode(object value) : base(value) { }
		public ISceneDocumentNode Parent { get { return XpfSceneDocumentNode.FromDocumentNode(ParentProperty.GetValue(Value, null)); } }
		public ISceneNode SceneNode { get { return XpfSceneNode.FromSceneNode(SceneNodeProperty.GetValue(Value, null)); } }
		public IVS2012TypeMetadata Type { get { return VS2012TypeMetadata.Get(TypeProperty.GetValue(Value, null)); } }
		public IVS2012PropertyMetadata SitePropertyKey { get { return VS2012PropertyMetadata.Get(SitePropertyKeyProperty.GetValue(Value, null)); } }
		public IEnumerable<ISceneDocumentNode> Properties {
			get {
				IEnumerable properties = (IEnumerable)PropertiesProperty.GetValue(Value, null);
				return properties.Cast<object>().Select(p => XpfSceneDocumentNode.FromDocumentNode(p.GetType().GetProperty("Value").GetValue(p, null)));
			}
		}
		Type DocumentNodeType {
			get {
				if(documentNodeType == null)
					documentNodeType = Value.GetType();
				return documentNodeType;
			}
		}
		PropertyInfo ParentProperty {
			get {
				if(parentProperty == null)
					parentProperty = DocumentNodeType.GetProperty("Parent");
				return parentProperty;
			}
		}
		PropertyInfo SceneNodeProperty {
			get {
				if(sceneNodeProperty == null)
					sceneNodeProperty = DocumentNodeType.GetProperty("SceneNode");
				return sceneNodeProperty;
			}
		}
		PropertyInfo TypeProperty {
			get {
				if(typeProperty == null)
					typeProperty = DocumentNodeType.GetProperty("Type");
				return typeProperty;
			}
		}
		PropertyInfo SitePropertyKeyProperty {
			get {
				if(sitePropertyKeyProperty == null)
					sitePropertyKeyProperty = DocumentNodeType.GetProperty("SitePropertyKey");
				return sitePropertyKeyProperty;
			}
		}
		PropertyInfo PropertiesProperty {
			get {
				if(propertiesProperty == null)
					propertiesProperty = DocumentNodeType.GetProperty("Properties");
				return propertiesProperty;
			}
		}
	}
	public class XpfProjectContext : RuntimeBase<IProjectContext, IDisposable>, IProjectContext {
		public static IProjectContext FromProjectContext(IDisposable value) {
			return value == null ? null : new XpfProjectContext(value);
		}
		static Type iProjectRootType;
		static Type iProjectContextType;
		static Type xamlProjectContextType;
		static PropertyInfo metadataProperty;
		static PropertyInfo projectRootProperty;
		static PropertyInfo applicationProperty;
		static PropertyInfo designerContextProperty;
		protected XpfProjectContext(IDisposable value) : base(value) { }
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				Value.Dispose();
		}
		public IVS2012TypeResolver Metadata { get { return VS2012TypeResolver.Get(MetadataProperty.GetValue(Value, null)); } }
		public string ProjectRoot {
			get {
				object val = ProjectRootProperty.GetValue(Value, null);
				return val == null ? string.Empty : Convert.ToString(val);
			}
		}
		public IProjectDocument Application { get { return XpfProjectDocument.FromProjectDocument(ApplicationProperty.GetValue(Value, null)); } }
		public IDesignerContext DesignerContext { get { return XpfDesignerContext.FromDesignerContext((IServiceContainer)DesignerContextProperty.GetValue(Value, null)); } }
		Type IProjectContextType {
			get {
				if(iProjectContextType == null)
					iProjectContextType = Value.GetType().GetInterface("IProjectContext");
				return iProjectContextType;
			}
		}
		Type IProjectRootType {
			get {
				if(iProjectRootType == null)
					iProjectRootType = Value.GetType().GetInterface("IProjectRoot");
				return iProjectRootType;
			}
		}
		Type XamlProjectContextType {
			get {
				if(xamlProjectContextType == null)
					xamlProjectContextType = Value.GetType();
				return xamlProjectContextType;
			}
		}
		PropertyInfo MetadataProperty {
			get {
				if(metadataProperty == null)
					metadataProperty = IProjectRootType.GetProperty("Metadata");
				return metadataProperty;
			}
		}
		PropertyInfo ProjectRootProperty {
			get {
				if(projectRootProperty == null)
					projectRootProperty = IProjectContextType.GetProperty("ProjectRoot");
				return projectRootProperty;
			}
		}
		PropertyInfo ApplicationProperty {
			get {
				if(applicationProperty == null)
					applicationProperty = IProjectContextType.GetProperty("Application");
				return applicationProperty;
			}
		}
		PropertyInfo DesignerContextProperty {
			get {
				if(designerContextProperty == null)
					designerContextProperty = XamlProjectContextType.GetProperty("DesignerContext");
				return designerContextProperty;
			}
		}
	}
	public class XpfDesignerContext : RuntimeBase<IDesignerContext, IServiceContainer>, IDesignerContext {
		public static IDesignerContext FromDesignerContext(IServiceContainer value) {
			return value == null ? null : new XpfDesignerContext(value);
		}
		static Type iDesignerContextType;
		static PropertyInfo resourceManagerProperty;
		protected XpfDesignerContext(IServiceContainer value) : base(value) { }
		public IServiceContainer Services { get { return Value; } }
		public IResourceManager ResourceManager { get { return XpfResourceManager.FromResourceManager(ResourceManagerProperty.GetValue(Value, null)); } }
		Type IDesignerContextType {
			get {
				if(iDesignerContextType == null)
					iDesignerContextType = Value.GetType().GetInterface("IDesignerContext");
				return iDesignerContextType;
			}
		}
		PropertyInfo ResourceManagerProperty {
			get {
				if(resourceManagerProperty == null)
					resourceManagerProperty = IDesignerContextType.GetProperty("ResourceManager");
				return resourceManagerProperty;
			}
		}
	}
	public class XpfResourceManager : RuntimeBase<IResourceManager, object>, IResourceManager {
		public static IResourceManager FromResourceManager(object value) {
			return value == null ? null : new XpfResourceManager(value);
		}
		static Type resourceManagerType;
		static MethodInfo getResourcesInElementsScopeMethod;
		protected XpfResourceManager(object value) : base(value) { }
		public IEnumerable<ISceneDocumentNode> GetResourcesInElementsScope(ISceneNode element, IVS2012TypeMetadata type, bool includeApplicationResources, bool uniqueKeysOnly) {
			int options = includeApplicationResources ? 1 : 0;
			if(uniqueKeysOnly)
				options |= 2;
			VS2012TypeMetadata typeMetadata = Guard.ArgumentMatchType<VS2012TypeMetadata>(type, "type");
			IEnumerable nodes = (IEnumerable)GetResourcesInElementsScopeMethod.Invoke(Value, new object[] { XpfSceneNode.ToSceneNode(element), typeMetadata.Value, options });
			return nodes.Cast<object>().Select(n => XpfSceneDocumentNode.FromDocumentNode(n));
		}
		Type ResourceManagerType {
			get {
				if(resourceManagerType == null)
					resourceManagerType = Value.GetType();
				return resourceManagerType;
			}
		}
		MethodInfo GetResourcesInElementsScopeMethod {
			get {
				if(getResourcesInElementsScopeMethod == null)
					getResourcesInElementsScopeMethod = ResourceManagerType.GetMethods().Where(m => m.Name == "GetResourcesInElementsScope" && m.GetParameters().First().ParameterType.Name == "SceneNode").First();
				return getResourcesInElementsScopeMethod;
			}
		}
	}
	public class XpfProjectDocument : RuntimeBase<IProjectDocument, object>, IProjectDocument {
		public static IProjectDocument FromProjectDocument(object value) {
			return value == null ? null : new XpfProjectDocument(value);
		}
		static Type iProjectDocumentType;
		static PropertyInfo documentRootProperty;
		protected XpfProjectDocument(object value) : base(value) { }
		public IDocumentRoot DocumentRoot { get { return XpfDocumentRoot.FromDocumentRoot((IDisposable)DocumentRootProperty.GetValue(Value, null)); } }
		Type IProjectDocumentType {
			get {
				if(iProjectDocumentType == null)
					iProjectDocumentType = Value.GetType().GetInterface("IProjectDocument");
				return iProjectDocumentType;
			}
		}
		PropertyInfo DocumentRootProperty {
			get {
				if(documentRootProperty == null)
					documentRootProperty = IProjectDocumentType.GetProperty("DocumentRoot");
				return documentRootProperty;
			}
		}
	}
	public class XpfDocumentRoot : RuntimeBase<IDocumentRoot, IDisposable>, IDocumentRoot {
		public static IDocumentRoot FromDocumentRoot(IDisposable value) {
			return value == null ? null : new XpfDocumentRoot(value);
		}
		static Type iDocumentRootType;
		static PropertyInfo rootNodeProperty;
		protected XpfDocumentRoot(IDisposable value) : base(value) { }
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing)
				Value.Dispose();
		}
		public ISceneDocumentNode RootNode { get { return XpfSceneDocumentNode.FromDocumentNode(RootNodeProperty.GetValue(Value, null)); } }
		Type IDocumentRootType {
			get {
				if(iDocumentRootType == null)
					iDocumentRootType = Value.GetType().GetInterface("IDocumentRoot");
				return iDocumentRootType;
			}
		}
		PropertyInfo RootNodeProperty {
			get {
				if(rootNodeProperty == null)
					rootNodeProperty = IDocumentRootType.GetProperty("RootNode");
				return rootNodeProperty;
			}
		}
	}
	public class XpfDocumentLocator : RuntimeBase<IDocumentLocator, object>, IDocumentLocator {
		public static IDocumentLocator FromDocumentLocator(object value) {
			return value == null ? null : new XpfDocumentLocator(value);
		}
		static Type iDocumentLocatorType;
		static PropertyInfo relativePathProperty;
		static PropertyInfo pathProperty;
		protected XpfDocumentLocator(object value) : base(value) { }
		public string RelativePath { get { return (string)RelativePathProperty.GetValue(Value, null); } }
		public string Path { get { return (string)PathProperty.GetValue(Value, null); } }
		Type IDocumentLocatorType {
			get {
				if(iDocumentLocatorType == null)
					iDocumentLocatorType = Value.GetType().GetInterface("IDocumentLocator");
				return iDocumentLocatorType;
			}
		}
		PropertyInfo RelativePathProperty {
			get {
				if(relativePathProperty == null)
					relativePathProperty = IDocumentLocatorType.GetProperty("RelativePath");
				return relativePathProperty;
			}
		}
		PropertyInfo PathProperty {
			get {
				if(pathProperty == null)
					pathProperty = IDocumentLocatorType.GetProperty("Path");
				return pathProperty;
			}
		}
	}
	public class XpfDocumentContext : RuntimeBase<IDocumentContext, object>, IDocumentContext {
		public static IDocumentContext FromDocumentContext(object value) {
			return value == null ? null : new XpfDocumentContext(value);
		}
		static Type iDocumentContextType;
		static PropertyInfo documentUrlProperty;
		static PropertyInfo documentLocatorProperty;
		protected XpfDocumentContext(object value) : base(value) { }
		public string DocumentUrl { get { return (string)DocumentUrlProperty.GetValue(Value, null); } }
		public IDocumentLocator DocumentLocator { get { return XpfDocumentLocator.FromDocumentLocator(DocumentLocatorProperty.GetValue(Value, null)); } }
		Type IDocumentContextType {
			get {
				if(iDocumentContextType == null)
					iDocumentContextType = Value.GetType().GetInterface("IDocumentContext");
				return iDocumentContextType;
			}
		}
		PropertyInfo DocumentUrlProperty {
			get {
				if(documentUrlProperty == null)
					documentUrlProperty = IDocumentContextType.GetProperty("DocumentUrl");
				return documentUrlProperty;
			}
		}
		PropertyInfo DocumentLocatorProperty {
			get {
				if(documentLocatorProperty == null)
					documentLocatorProperty = IDocumentContextType.GetProperty("DocumentLocator");
				return documentLocatorProperty;
			}
		}
	}
	public class XpfSceneNode : RuntimeBase<ISceneNode, object>, ISceneNode {
		public static ISceneNode FromSceneNode(object value) {
			return value == null ? null : new XpfSceneNode(value);
		}
		public static object ToSceneNode(ISceneNode sceneNode) {
			return sceneNode == null ? null : Guard.ArgumentMatchType<XpfSceneNode>(sceneNode, "sceneNode").Value;
		}
		static Type sceneNodeType;
		static PropertyInfo documentNodeProperty;
		static PropertyInfo projectContextProperty;
		static PropertyInfo documentContextProperty;
		static PropertyInfo modelItemProperty;
		protected XpfSceneNode(object value) : base(value) { }
		public ISceneDocumentNode DocumentNode { get { return XpfSceneDocumentNode.FromDocumentNode(DocumentNodeProperty.GetValue(Value, null)); } }
		public IProjectContext ProjectContext { get { return XpfProjectContext.FromProjectContext((IDisposable)ProjectContextProperty.GetValue(Value, null)); } }
		public IDocumentContext DocumentContext { get { return XpfDocumentContext.FromDocumentContext(DocumentContextProperty.GetValue(Value, null)); } }
		public ISceneNodeModelItem ModelItem { get { return XpfSceneNodeModelItem.FromSceneNodeModelItem((ModelItem)ModelItemProperty.GetValue(Value, null)); } }
		Type SceneNodeType {
			get {
				if(sceneNodeType == null)
					sceneNodeType = Value.GetType();
				return sceneNodeType;
			}
		}
		PropertyInfo DocumentNodeProperty {
			get {
				if(documentNodeProperty == null)
					documentNodeProperty = SceneNodeType.GetProperty("DocumentNode");
				return documentNodeProperty;
			}
		}
		PropertyInfo ProjectContextProperty {
			get {
				if(projectContextProperty == null)
					projectContextProperty = SceneNodeType.GetProperty("ProjectContext");
				return projectContextProperty;
			}
		}
		PropertyInfo DocumentContextProperty {
			get {
				if(documentContextProperty == null)
					documentContextProperty = SceneNodeType.GetProperty("DocumentContext");
				return documentContextProperty;
			}
		}
		PropertyInfo ModelItemProperty {
			get {
				if(modelItemProperty == null)
					modelItemProperty = SceneNodeType.GetProperty("ModelItem");
				return modelItemProperty;
			}
		}
	}
	public class XpfXamlExpressionSerializer : IXamlExpressionSerializer {
		readonly Type xamlExpressionSerializerType;
		MethodInfo getStringFromExpressionMethod;
		public XpfXamlExpressionSerializer(Assembly expressionDesignSurfaceAssembly) {
			xamlExpressionSerializerType = expressionDesignSurfaceAssembly.GetType("Microsoft.Expression.DesignSurface.Properties.XamlExpressionSerializer");
			if(xamlExpressionSerializerType == null)
				xamlExpressionSerializerType = expressionDesignSurfaceAssembly.GetType("Microsoft.VisualStudio.DesignTools.Designer.Properties.XamlExpressionSerializer"); 
		}
		public string GetStringFromExpression(ISceneDocumentNode expression, ISceneDocumentNode parentNode) {
			if(xamlExpressionSerializerType == null)
				return XpfModelItem.ToModelItem(expression.SceneNode.ModelItem.ModelItem).Source.ComputedValue.With(x => x.ToString());
			return (string)GetStringFromExpressionMethod.Invoke(null, new object[] { XpfSceneDocumentNode.ToDocumentNode(expression), XpfSceneDocumentNode.ToDocumentNode(parentNode) });
		}
		public MethodInfo GetStringFromExpressionMethod {
			get {
				if(getStringFromExpressionMethod == null)
					getStringFromExpressionMethod = xamlExpressionSerializerType.GetMethod("GetStringFromExpression", BindingFlags.Static | BindingFlags.Public);
				return getStringFromExpressionMethod;
			}
		}
	}
	public class XpfSceneNodeModelItem : RuntimeBase<ISceneNodeModelItem, ModelItem>, ISceneNodeModelItem {
		public static ISceneNodeModelItem FromSceneNodeModelItem(ModelItem value) {
			return value == null ? null : new XpfSceneNodeModelItem(value);
		}
		static Type iSceneNodeModelItemType;
		static PropertyInfo sceneNodeProperty;
		static IXamlExpressionSerializer xamlExpressionSerializer;
		protected XpfSceneNodeModelItem(ModelItem value) : base(value) { }
		public IModelItem ModelItem { get { return XpfModelItem.FromModelItem(Value); } }
		public ISceneNode SceneNode { get { return XpfSceneNode.FromSceneNode(SceneNodeProperty.GetValue(Value, null)); } }
		Type ISceneNodeModelItemType {
			get {
				if(iSceneNodeModelItemType == null)
					iSceneNodeModelItemType = Value.GetType().GetInterface("ISceneNodeModelItem");
				return iSceneNodeModelItemType;
			}
		}
		public IXamlExpressionSerializer XamlExpressionSerializer {
			get {
				if(xamlExpressionSerializer == null)
					xamlExpressionSerializer = new XpfXamlExpressionSerializer(ISceneNodeModelItemType.Assembly);
				return xamlExpressionSerializer;
			}
		}
		PropertyInfo SceneNodeProperty {
			get {
				if(sceneNodeProperty == null)
					sceneNodeProperty = ISceneNodeModelItemType.GetProperty("SceneNode");
				return sceneNodeProperty;
			}
		}
	}
	public class XpfMarkupAccessService2012 : IMarkupAccessService2012 {
		public static IMarkupAccessService2012 Create(IModelItem modelItem) {
			ModelItem value = XpfModelItem.ToModelItem(modelItem);
			return value.GetType().GetProperty("SceneNode") == null ? null : new XpfMarkupAccessService2012();
		}
		protected XpfMarkupAccessService2012() { }
		ISceneNodeModelItem IMarkupAccessService2012.GetModelItem(IModelItem modelItem) {
			return XpfSceneNodeModelItem.FromSceneNodeModelItem(XpfModelItem.ToModelItem(modelItem));
		}
	}
}

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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Model {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class ModelEditorHelper {
		public const string pathSeparator = "\\";
		public const string rootNodeName = "Application";
		private static List<string> CalculatePathNodes(string nodePath) {
			if(nodePath == null) {
				return null;
			}
			List<string> result = new List<string>(nodePath.Split(new string[] { pathSeparator }, StringSplitOptions.None));
			int emptyIndex;
			while((emptyIndex = result.IndexOf(string.Empty)) >= 0) {
				if(emptyIndex > 0 && emptyIndex < result.Count - 1) {
					result[emptyIndex - 1] += pathSeparator + result[emptyIndex + 1];
					result.RemoveRange(emptyIndex, 2);
				}
				else {
					result.RemoveAt(emptyIndex);
				}
			}
			return result;
		}
		public static ModelNode FindNodeByPath(string nodePath, ModelNode rootNode, bool inThisLayer, bool findLastNodeByPath) {
			List<string> pathNodes = CalculatePathNodes(nodePath);
			if(pathNodes == null || rootNode == null) {
				return null;
			}
			if(pathNodes.Count == 1 && pathNodes[0] == rootNodeName) {
				return rootNode;
			}
			ModelNode currentNode = rootNode;
			foreach(string id in pathNodes) {
				ModelNode nextNode = null;
				if(inThisLayer) {
					nextNode = currentNode.GetNodeInThisLayer(id);
				}
				else {
					nextNode = currentNode.GetNode(id);
				}
				if(nextNode == null) {
					return findLastNodeByPath ? currentNode : null;
				}
				currentNode = nextNode;
			}
			return currentNode;
		}
		public static ModelNode FindNodeByPath(string nodePath, ModelNode model) {
			return FindNodeByPath(nodePath, model, false, true);
		}
		public static string GetModelNodePath(ModelNode node) {
			if(node == null) {
				return null;
			}
			if(node.IsRoot) {
				return rootNodeName;
			}
			string result = string.Empty;
			if(node.Parent != null && node.Parent.Parent != null) {
				result = GetModelNodePath(node.Parent) + pathSeparator;
			}
			result += node.Id.Replace(pathSeparator, pathSeparator + pathSeparator);
			return result;
		}
		public static string GetPropertyPath(ModelNode node, string propertyName) {
			return GetModelNodePath(node) + pathSeparator + propertyName;
		}
		public static T GetPropertyAttribute<T>(ModelNode node, string propertyName) where T : Attribute {
			return GetPropertyAttribute<T>(node.GetType(), propertyName);
		}
		public static List<T> GetPropertyAttributes<T>(ModelNode node, string propertyName) where T : Attribute {
			return GetPropertyAttributes<T>(node.GetType(), propertyName);
		}
		public static T GetPropertyAttribute<T>(Type type, string propertyName) where T : Attribute {
			IList<T> attributes = GetPropertyAttributes<T>(type, propertyName);
			if(attributes.Count > 0) {
				return attributes[0];
			}
			return null;
		}
		public static List<T> GetPropertyAttributes<T>(Type type, string propertyName) where T : Attribute {
			foreach(Type implementedInterface in TypeHelper.GetInterfaces(type)) {
				List<T> attributes = GetPropertyAttributesInternal<T>(implementedInterface, propertyName);
				if(attributes.Count > 0) {
					return attributes;
				}
			}
			return GetPropertyAttributesInternal<T>(type, propertyName);
		}
		private static List<T> GetPropertyAttributesInternal<T>(Type type, string propertyName) where T : Attribute {
			if(TypeHelper.ContainsProperty(type, propertyName)) {
				PropertyInfo propertyInfo = TypeHelper.GetProperty(type, propertyName);
				return new List<T>(AttributeHelper.GetAttributes<T>(propertyInfo, true));
			}
			return new List<T>();
		}
		public static T GetNodeAttribute<T>(IModelNode node) where T : Attribute {
			return GetNodeAttribute<T>(node.GetType());
		}
		public static T GetNodeAttribute<T>(Type nodeType) where T : Attribute {
			List<T> result = GetNodeAttributes<T>(nodeType, true);
			if(result.Count > 0) {
				return result[0];
			}
			return null;
		}
		public static List<T> GetNodeAttributes<T>(IModelNode node) where T : Attribute {
			return GetNodeAttributes<T>(node.GetType());
		}
		public static List<T> GetNodeAttributes<T>(Type nodeType) where T : Attribute {
			return GetNodeAttributes<T>(nodeType, false);
		}
		private static List<T> GetNodeAttributes<T>(Type nodeType, bool returnFirst) where T : Attribute {
			List<T> result = new List<T>();
			T[] attributes;
			foreach(Type item in TypeHelper.GetInterfaces(nodeType)) {
				attributes = AttributeHelper.GetAttributes<T>(item, true);
				if(attributes.Length > 0) {
					result.Add(attributes[0]);
					if(returnFirst) {
						break;
					}
				}
			}
			if(result.Count == 0) {
				attributes = AttributeHelper.GetAttributes<T>(nodeType, true);
				if(attributes.Length > 0) {
					result.Add(attributes[0]);
				}
			}
			return result;
		}
		public static Type GetPropertyType(ModelNode node, string propertyName) {
			Type type = node.GetType();
			if(TypeHelper.ContainsProperty(type, propertyName)) {
				return TypeHelper.GetProperty(type, propertyName).PropertyType;
			}
			return null;
		}
		public static List<Type> GetPropertyOwnerTypes(ModelNode node, string propertyName) {
			return GetPropertyOwnerTypes(node.GetType(), propertyName);
		}
		public static List<Type> GetPropertyOwnerTypes(Type type, string propertyName) {
			List<Type> result = new List<Type>();
			PropertyInfo propertyInfo;
			foreach(Type item in TypeHelper.GetInterfaces(type)) {
				if(TypeHelper.ContainsProperty(item, propertyName)) {
					propertyInfo = TypeHelper.GetProperty(item, propertyName);
					if(propertyInfo.DeclaringType == item) {
						result.Add(item);
					}
				}
			}
			if(result.Count == 0) {
				if(TypeHelper.ContainsProperty(type, propertyName)) {
					propertyInfo = TypeHelper.GetProperty(type, propertyName);
					result.Add(propertyInfo.DeclaringType);
				}
			}
			return result;
		}
		public static Type GetNodeInstanceInterfaceType(ModelNode modelNode) {
			return modelNode.NodeInfo.BaseInterface;
		}
		public static List<Type> GetOwnerTypes(ModelNode modelNode) {
			if(modelNode.Parent != null) {
				Type modelNodeType = modelNode.GetType();
				ModelNodeInfo parentNodeInfo = modelNode.Parent.NodeInfo;
				foreach(KeyValuePair<string, Type> item in parentNodeInfo.GetListChildrenTypes()) {
					if(item.Value == modelNodeType) {
						List<Type> result = new List<Type>();
						result.Add(parentNodeInfo.BaseInterface);
						return result;
					}
				}
				return GetPropertyOwnerTypes(modelNode.Parent, modelNode.Id);
			}
			return null;
		}
		public static bool IsGenerateContentNode(ModelNode modelNode) { 
			if(modelNode == null) return false;
			Dictionary<string, Type> listChildrenTypes = modelNode.NodeInfo.GetChildrenTypes();
			foreach(KeyValuePair<string, Type> listChildrenType in listChildrenTypes) {
				ModelNode childrenListNode = modelNode.GetNode(listChildrenType.Key);
				if(HasNonDefaultGenerator(childrenListNode)) {
					return true;
				}
			}
			return false;
		}
		public static void GenerateContent(ModelNode modelNode) { 
			ModelNode generatedNode = modelNode.Parent.AddNode(modelNode.GetType());
			generatedNode.ApplyDiffValues(modelNode);
			Dictionary<string, Type> listChildrenTypes = modelNode.NodeInfo.GetChildrenTypes();
			foreach(KeyValuePair<string, Type> listChildrenType in listChildrenTypes) {
				ModelNode childrenListNode = generatedNode.GetNode(listChildrenType.Key);
				if(childrenListNode.NodeInfo.NodesGenerator != null) {
					childrenListNode.NodeInfo.NodesGenerator.GenerateNodes(childrenListNode);
				}
				if(modelNode.GetNode(listChildrenType.Key) == null) {
					modelNode.AddNode(listChildrenType.Key, listChildrenType.Value);
				}
				modelNode[listChildrenType.Key].Merge(childrenListNode);
			}
			generatedNode.Delete();
		}
		private static bool HasNonDefaultGenerator(ModelNode modelNode) {
			if(modelNode != null && modelNode.NodeInfo.NodesGenerator != null) {
				ModelGenerateContentActionAttribute[] attributes = AttributeHelper.GetAttributes<ModelGenerateContentActionAttribute>(modelNode.NodeInfo.NodesGenerator.GetType(), true);
				if(attributes.Length > 0) {
					return attributes[0].Visible;
				}
			}
			return false;
		}
		public static T CreateNewViewNode<T>(ModelNode sourceNode) where T : IModelView {
			IModelNode generatedNode = sourceNode.CreatorInstance.CreateNode(string.Empty, sourceNode.GetType());
			return (T)generatedNode;
		}
		public static string GetNodeDescription(ModelNode node) {
			string result = string.Empty;
			if(node != null) {
#if DebugTest
				DXDescriptionAttribute descriptionAttribute = ModelEditorHelper.GetNodeAttribute<DXDescriptionAttribute>(node);
#else
				DescriptionAttribute descriptionAttribute = ModelEditorHelper.GetNodeAttribute<DescriptionAttribute>(node);
#endif
				result = GetInfoDescription(ModelEditorHelper.GetOwnerTypes(node), ModelEditorHelper.GetNodeInstanceInterfaceType(node), "Node type");
				if(descriptionAttribute != null) {
					result += descriptionAttribute.Description;
				}
			}
			return result;
		}
		public static string GetPropertyDescriptionAttributeValue(ModelNode node, string propertyName) { 
			string result = string.Empty;
			if(!string.IsNullOrEmpty(propertyName) && node != null) {
#if DebugTest
				DXDescriptionAttribute descriptionAttribute = ModelEditorHelper.GetPropertyAttribute<DXDescriptionAttribute>(node, propertyName);
#else
				DescriptionAttribute descriptionAttribute = ModelEditorHelper.GetPropertyAttribute<DescriptionAttribute>(node, propertyName);
#endif
				if(descriptionAttribute != null) {
					result += descriptionAttribute.Description;
				}
			}
			return result;
		}
		public static string GetPropertyDescription(ModelNode node, string propertyName) { 
			string result = string.Empty;
			if(!string.IsNullOrEmpty(propertyName) && node != null) {
				result = GetInfoDescription(ModelEditorHelper.GetPropertyOwnerTypes(node, propertyName), ModelEditorHelper.GetPropertyType(node, propertyName), "Property type");
				result += GetPropertyDescriptionAttributeValue(node, propertyName);
			}
			return result;
		}
		public static string GetInfoDescription(List<Type> ownerTypes, Type memberType, string elementTitle) { 
			if(memberType == null) {
				return null;
			}
			string result = string.Format("<b>{0}: </b>{1}", elementTitle, GetFriendlyTypeName(memberType));
			if(ownerTypes != null && ownerTypes.Count > 0) {
				string interfaceTitle = "Member of ";
				if(ownerTypes.Count > 1) {
					interfaceTitle += " interfaces";
				}
				else if(ownerTypes[0].IsInterface) {
					interfaceTitle += "interface";
				}
				else {
					interfaceTitle += "type";
				}
				string ownedInterfaces = string.Join(", ", Enumerator.ToArray<string>(Enumerator.Convert<Type, string>(ownerTypes, delegate(Type sourceType) { return GetFriendlyTypeName(sourceType); })));
				result += string.Format(",  <b>{0}: </b>{1}", interfaceTitle, ownedInterfaces);
			}
			return result + "<br>";
		}
		public static string GetFriendlyTypeName(Type type) {
			if(type == null) {
				return null;
			}
			if(type.IsGenericParameter) {
				return type.Name;
			}
			if(!type.IsGenericType) {
				return type.FullName;
			}
			StringBuilder builder = new System.Text.StringBuilder();
			string name = type.Name;
			int index = name.IndexOf("`");
			builder.AppendFormat("{0}.{1}", type.Namespace, name.Substring(0, index));
			builder.Append('<');
			bool first = true;
			foreach(Type arg in type.GetGenericArguments()) {
				if(!first) {
					builder.Append(',');
				}
				builder.Append(GetFriendlyTypeName(arg));
				first = false;
			}
			builder.Append('>');
			return builder.ToString();
		}
		public static ModelNode GetNodeInLayer(ModelNode source, ModelNode layer) {
			string nodePath = ModelEditorHelper.GetModelNodePath(source);
			return ModelEditorHelper.FindNodeByPath(nodePath, layer, true, false);
		}
		public static List<string> GetAspectNames(ModelApplicationBase modelApplication) {
			List<string> result = new List<string>(((ModelApplicationBase)modelApplication).GetAspectNames());
			result.Sort();
			result.Insert(0, CaptionHelper.DefaultLanguage);
			return result;
		}
		private static ModelNode GetMaster(ModelNode node) {
			ModelNode result = node;
			while(result.Master != null) {
				result = result.Master;
			}
			return result;
		}
		public static bool IsNodeEqual(ModelNode node1, ModelNode node2) {
			if((node1 == null && node2 != null) || (node2 == null && node1 != null)) {
				return false;
			}
			if(node1 == null && node2 == null) {
				return true;
			}
			return GetMaster(node1) == GetMaster(node2);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelApplicationBase GetUnusableModel(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			return model.IsMaster ? model.LastLayer.UnusableModel : model.UnusableModel;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelApplicationBase GetFullUnusableModel(ModelApplicationBase model) {
			Guard.ArgumentNotNull(model, "model");
			ModelApplicationBase unusableModel = model.CalculateUnusableModel();
			return unusableModel ?? new ModelApplicationBase.UnusableModelApplication(model.NodeInfo);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelNode AddCloneNode(ModelNode owner, ModelNode source, string id) {
			Guard.ArgumentNotNull(owner, "owner");
			return owner.AddClonedNode(source, id);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelApplicationBase MoveNodeToOtherLayer(ModelNode modelNode, string layerId, IModelNodeMoveInfo modelNodeMoveInfo) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNullOrEmpty(layerId, "layerId");
			Guard.ArgumentNotNull(modelNodeMoveInfo, "modelNodeMoveInfo");
			string modelNodePath = ModelEditorHelper.GetModelNodePath(modelNode);
			ModelNode modelNodeInLastLayer = ModelEditorHelper.FindNodeByPath(modelNodePath, ((ModelApplicationBase)modelNode.Application).GetOrCreateWritableLayer(), true, false);
			if(modelNodeInLastLayer != null) {
				ModelApplicationBase targetLayer = ModelNode.GetModuleLayerById((ModelApplicationBase)modelNode.Application, layerId);
				if(targetLayer != null) {
					ModelNode targetNode = ModelNode.MoveNodeToOtherLayer(modelNodeInLastLayer, targetLayer, modelNodeMoveInfo);
					if(targetNode != null) {
						return (ModelApplicationBase)targetNode.Application;
					}
				}
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelNodeInfo CreateDummyNodeInfo() {
			ModelApplicationCreatorProperties properties = ModelApplicationCreatorProperties.CreateDefault();
			ModelApplicationCreator creator = ModelApplicationCreator.GetModelApplicationCreator(properties);
			return new ModelNodeInfo(creator, null, typeof(ModelNode), typeof(IModelNode), ModelNodesDefaultInterfaceGenerator.Instance, ModelValueNames.Id);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void AddPropertyChangingHandler(ModelNode modelNode, PropertyChangingEventHandler handler) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNull(handler, "handler");
			modelNode.AddPropertyChangingHandler(handler);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void RemovePropertyChangingHandler(ModelNode modelNode, PropertyChangingEventHandler handler) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNull(handler, "handler");
			modelNode.RemovePropertyChangingHandler(handler);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void AddPropertyChangedHandler(ModelNode modelNode, PropertyChangedEventHandler handler) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNull(handler, "handler");
			modelNode.AddPropertyChangedHandler(handler);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void RemovePropertyChangedHandler(ModelNode modelNode, PropertyChangedEventHandler handler) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNull(handler, "handler");
			modelNode.RemovePropertyChangedHandler(handler);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static ModelNode[] GetChildNodes(ModelNode modelNode) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			return ModelNodeInfo.GetChildNodes(modelNode);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void RereadLastLayer(IModelApplication model, ModelStoreBase store) { 
			Guard.ArgumentNotNull(model, "model");
			ApplicationModelManager.RecreateModel(model, store != null ? store : ModelStoreBase.Empty);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void ForceSetValueInCurrentAspect<ValueType>(ModelNode modelNode, string name, ValueType value) {
			ModelNode node = modelNode.GetOrCreateWritableLayer();
			node.SetValueInThisLayer<ValueType>(name, value);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool HasValueInCurrentAspect(ModelNode modelNode, string propertyName) {
			return modelNode.HasValueInCurrentAspect(propertyName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void Dispose(object target) { 
			Guard.ArgumentNotNull(target, "target");
			IModelEditorDisposable disposable = target as IModelEditorDisposable;
			if(disposable != null) {
				disposable.Dispose();
			}
		}
		private static bool modelCalculatorsCacheEnabled = false;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool ModelCalculatorsCacheEnabled {
			get {
				return modelCalculatorsCacheEnabled;
			}
			set {
				modelCalculatorsCacheEnabled = value;
				DevExpress.ExpressApp.Model.DomainLogics.ViewNamesCalculator.CacheEnabled = modelCalculatorsCacheEnabled;
				DevExpress.ExpressApp.Model.DomainLogics.ModelMemberLogic.CacheEnabled = modelCalculatorsCacheEnabled;
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ModelNodeMoveInfo : IModelNodeMoveInfo {
		private Dictionary<string, ModelNodeInfo> targetNodeInfos = new Dictionary<string, ModelNodeInfo>();
		private ModelApplicationBase modelApplication;
		private List<Type> requiredTypes = new List<Type>();
		private void FillRequiredTypes(ApplicationModulesManager moduleManager) {
			foreach(ModuleBase module in moduleManager.Modules) {
				IEnumerable<Type> regularTypes = module.GetRegularTypes();
				if(regularTypes != null) {
					foreach(Type regularType in regularTypes) {
						requiredTypes.Add(regularType);
					}
				}
			}
		}
		public ModelNodeMoveInfo(ModelApplicationBase application, ApplicationModulesManager moduleManager) {
			Guard.ArgumentNotNull(application, "application");
			Guard.ArgumentNotNull(moduleManager, "module");
			this.modelApplication = application;
			FillRequiredTypes(moduleManager);
			foreach(ModelNodeInfo modelNodeInfo in modelApplication.NodeInfo.ApplicationCreator.GetNodeInfos(typeof(ModelNode))) {
				targetNodeInfos[modelNodeInfo.GeneratedClass.FullName] = modelNodeInfo;
			};
		}
		#region IModelNodeMoveInfo Members
		public bool ShouldMoveNode(ModelNodeInfo nodeInfo, ModelNodeInfo childNodeInfo) {
			bool result = false;
			if(requiredTypes.Contains(childNodeInfo.BaseInterface)) {
				ModelNodeInfo targetNodeInfo;
				if(targetNodeInfos.TryGetValue(nodeInfo.GeneratedClass.FullName, out targetNodeInfo)) {
					foreach(Type childrenType in targetNodeInfo.GetChildrenTypes().Values) {
						if(childrenType.FullName == childNodeInfo.GeneratedClass.FullName) {
							result = true;
							break;
						}
					}
				}
			}
			return result;
		}
		public bool ShouldMoveValue(ModelNodeInfo nodeInfo, string valueName) {
			if(nodeInfo.GetValueInfo(valueName) == null) {
				return true;
			}
			ModelNodeInfo targetNodeInfo = null;
			if(targetNodeInfos.TryGetValue(nodeInfo.GeneratedClass.FullName, out targetNodeInfo)) {
				return targetNodeInfo.GetValueInfo(valueName) != null;
			}
			return false;
		}
		public bool DoesNodeExist(ModelNode node) {
			string nodePath = ModelEditorHelper.GetModelNodePath(node);
			return ModelEditorHelper.FindNodeByPath(nodePath, modelApplication, false, false) != null;
		}
		#endregion
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class RuntimeModelNodeMoveInfo : IModelNodeMoveInfo {
		#region IModelNodeMoveInfo Members
		public bool ShouldMoveNode(ModelNodeInfo nodeInfo, ModelNodeInfo childNodeInfo) {
			return true;
		}
		public bool ShouldMoveValue(ModelNodeInfo nodeInfo, string valueName) {
			return true;
		}
		public bool DoesNodeExist(ModelNode node) {
			return true;
		}
		#endregion
	}
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IModelEditorDisposable {   
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void Dispose();
	}
}

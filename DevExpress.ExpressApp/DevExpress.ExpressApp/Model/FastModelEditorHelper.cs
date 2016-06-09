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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Model {
	public class FastModelEditorHelper {
		Dictionary<Type, Dictionary<string, Attribute>> nodeAttributes = new Dictionary<Type, Dictionary<string, Attribute>>();
		Dictionary<Type, Dictionary<string, Attribute>> attributesCache = new Dictionary<Type, Dictionary<string, Attribute>>();
		Dictionary<Type, Dictionary<string, object>> allAttributesCache = new Dictionary<Type, Dictionary<string, object>>();
		Dictionary<Type, DisplayPropertyAttribute> nodeDisplayPropertyAttributeCache = new Dictionary<Type, DisplayPropertyAttribute>();
		Dictionary<string, string> descriptionsCache = new Dictionary<string, string>();
		private readonly object lockObject = new object();
		public T GetPropertyAttribute<T>(ITypeInfo typeInfo, string propertyName) where T : Attribute {
			lock(lockObject) {
				Attribute result;
				Type nodeType = typeInfo.Type;
				Dictionary<string, Attribute> nodeAttributesCache;
				if(!attributesCache.TryGetValue(nodeType, out nodeAttributesCache)) {
					nodeAttributesCache = new Dictionary<string, Attribute>();
					attributesCache[nodeType] = nodeAttributesCache;
				}
				string key = propertyName + ":" + typeof(T).FullName;
				if(nodeAttributesCache.ContainsKey(key)) {
					result = nodeAttributesCache[key];
				}
				else {
					result = ModelEditorHelper.GetPropertyAttribute<T>(typeInfo.Type, propertyName);
					nodeAttributesCache[key] = result;
				}
				return (T)result;
			}
		}
		public T GetPropertyAttribute<T>(ModelNode node, string propertyName) where T : Attribute {
			ITypeInfo type = XafTypesInfo.Instance.FindTypeInfo(node.GetType());
			return GetPropertyAttribute<T>(type, propertyName);
		}
		public IList<T> GetPropertyAttributes<T>(ModelNode node, string propertyName) where T : Attribute {
			lock(lockObject) {
				IList<T> result;
				Type nodeType = node.GetType();
				Dictionary<string, object> nodeAttributesCache;
				if(!allAttributesCache.TryGetValue(nodeType, out nodeAttributesCache)) {
					nodeAttributesCache = new Dictionary<string, object>();
					allAttributesCache[nodeType] = nodeAttributesCache;
				}
				string key = propertyName + ":" + typeof(T).FullName;
				if(nodeAttributesCache.ContainsKey(key)) {
					result = (IList<T>)nodeAttributesCache[key];
				}
				else {
					result = ModelEditorHelper.GetPropertyAttributes<T>(node, propertyName);
					nodeAttributesCache[key] = result;
				}
				return result;
			}
		}
		public T GetNodeAttribute<T>(Type nodeType) where T : Attribute {
			lock(lockObject) {
				Dictionary<string, Attribute> nodeAttributesCache;
				if(!nodeAttributes.TryGetValue(nodeType, out nodeAttributesCache)) {
					nodeAttributesCache = new Dictionary<string, Attribute>();
					nodeAttributes[nodeType] = nodeAttributesCache;
				}
				Attribute result;
				string key = typeof(T).FullName;
				if(nodeAttributesCache.ContainsKey(key)) {
					result = nodeAttributesCache[key];
				}
				else {
					result = ModelEditorHelper.GetNodeAttribute<T>(nodeType);
					nodeAttributesCache[key] = result;
				}
				return (T)result;
			}
		}
		public T GetNodeAttribute<T>(IModelNode node) where T : Attribute {
			return GetNodeAttribute<T>(XafTypesInfo.Instance.FindTypeInfo(node.GetType()).Type);
		}
		public bool IsPropertyModelBrowsableVisible(ModelNode parentNode, string propertyName) {
			bool isModelBrowsable = true;
			IEnumerable<ModelBrowsableAttribute> modelBrowsableAttributes = GetPropertyAttributes<ModelBrowsableAttribute>(parentNode, propertyName);
			foreach(ModelBrowsableAttribute modelBrowsableAttribute in modelBrowsableAttributes) {
				Type visibilityCalculatorType = modelBrowsableAttribute.VisibilityCalculatorType;
				IModelIsVisible modelIsVisible = Activator.CreateInstance(visibilityCalculatorType) as IModelIsVisible;
				if(modelIsVisible != null) {
					isModelBrowsable = isModelBrowsable && modelIsVisible.IsVisible(parentNode, propertyName);
				}
			}
			IEnumerable<BrowsableAttribute> browsableAttributes = GetPropertyAttributes<BrowsableAttribute>(parentNode, propertyName);
			foreach(BrowsableAttribute modelBrowsableAttribute in browsableAttributes) {
				if(!modelBrowsableAttribute.Browsable) {
					isModelBrowsable = false;
					break;
				}
			}
			return isModelBrowsable;
		}
		public string GetPropertyDescription(ModelNode node, string propertyName) {
			lock(lockObject) {
				if(!string.IsNullOrEmpty(propertyName) && node != null) {
					string result;
					string key = node.GetType().FullName + propertyName;
					if(!descriptionsCache.TryGetValue(key, out result)) {
						result = ModelEditorHelper.GetPropertyDescription(node, propertyName);
						descriptionsCache[key] = result;
					}
					return result;
				}
				return string.Empty;
			}
		}
		public string GetNodeDescription(ModelNode node) {
			lock(lockObject) {
				if(node != null) {
					string result;
					string key = node.GetType().FullName;
					if(!descriptionsCache.TryGetValue(key, out result)) {
						result = ModelEditorHelper.GetNodeDescription(node);
						descriptionsCache[key] = result;
					}
					return result;
				}
				return string.Empty;
			}
		}
		public Boolean IsRequired(ModelNode node, String propertyName) {
			Boolean result = false;
			RequiredAttribute requiredAttribute = GetPropertyAttribute<RequiredAttribute>(node, propertyName);
			if(requiredAttribute != null) {
				if(requiredAttribute.RequiredCalculatorType != null) {
					IModelIsRequired requiredCalculator = Activator.CreateInstance(requiredAttribute.RequiredCalculatorType) as IModelIsRequired;
					if(requiredCalculator != null) {
						result = requiredCalculator.IsRequired(node, propertyName);
					}
				}
				else {
					result = true;
				}
			}
			return result;
		}
		public bool IsReadOnly(ModelNode node, String propertyName) {
			if(String.IsNullOrEmpty(propertyName)) {
				return IsReadOnly(node, (ModelNode)null);
			}
			bool result = false;
			ModelReadOnlyAttribute modelReadOnlyAttribute = GetPropertyAttribute<ModelReadOnlyAttribute>(node, propertyName);
			if(modelReadOnlyAttribute != null) {
				if(modelReadOnlyAttribute.ReadOnlyCalculatorType == typeof(DesignerOnlyCalculator)) {
					result = !DesignerOnlyCalculator.IsRunFromDesigner;
				}
				else {
					IModelIsReadOnly modelIsVisible = Activator.CreateInstance(modelReadOnlyAttribute.ReadOnlyCalculatorType) as IModelIsReadOnly;
					if(modelIsVisible != null) {
						result = modelIsVisible.IsReadOnly(node, propertyName);
					}
				}
			}
			ReadOnlyAttribute readOnlyAttribute = GetPropertyAttribute<ReadOnlyAttribute>(node, propertyName);
			if(readOnlyAttribute != null) {
				result |= readOnlyAttribute.IsReadOnly;
			}
			ModelValueInfo valueInfo = node.GetValueInfo(propertyName);
			if(valueInfo != null) {
				result |= valueInfo.IsReadOnly;
			}
			return result;
		}
		public Boolean IsReadOnly(ModelNode node, ModelNode childNode) {
			if(node == null) {
				return true;
			}
			bool result = false;
			ModelReadOnlyAttribute modelReadOnlyAttribute = GetNodeAttribute<ModelReadOnlyAttribute>(node);
			if(modelReadOnlyAttribute != null) {
				if(modelReadOnlyAttribute.ReadOnlyCalculatorType == typeof(DesignerOnlyCalculator)) {
					result = !DesignerOnlyCalculator.IsRunFromDesigner;
				}
				else {
					IModelIsReadOnly modelIsVisible = Activator.CreateInstance(modelReadOnlyAttribute.ReadOnlyCalculatorType) as IModelIsReadOnly;
					if(modelIsVisible != null) {
						result = modelIsVisible.IsReadOnly(node, childNode);
					}
				}
			}
			ReadOnlyAttribute readOnly = GetNodeAttribute<ReadOnlyAttribute>(node);
			if(readOnly != null) {
				result |= readOnly.IsReadOnly;
			}
			return result;
		}
		public bool CanDeleteNode(ModelNode node, bool readOnlyModel) { 
			if(node is IModelApplication || readOnlyModel) {
				return false;
			}
			if(node != null && node.Parent != null) {
				bool readOnly = true;
				foreach(Type item in node.Parent.NodeInfo.GetListChildrenTypes().Values) {
					if(node.GetType() == item) {
						readOnly = false;
						break;
					}
				}
				readOnly |= IsReadOnly(node.Parent, node);
				return !(readOnlyModel || readOnly);
			}
			return !readOnlyModel;
		}
		public string GetNodeDisplayName(Type type) {
			if(type == null) {
				return "";
			}
			ModelDisplayNameAttribute modelDisplayNameAttribute = GetNodeAttribute<ModelDisplayNameAttribute>(type);
			if(modelDisplayNameAttribute != null && !string.IsNullOrEmpty(modelDisplayNameAttribute.ModelDisplayName)) {
				return modelDisplayNameAttribute.ModelDisplayName;
			}
			return ModelApplicationCreator.GetDefaultXmlName(type);
		}
		public Dictionary<string, Type> GetListChildNodeTypes(ModelNodeInfo nodeInfo) {
			Dictionary<string, Type> result = new Dictionary<string, Type>();
			foreach(KeyValuePair<string, Type> item in nodeInfo.GetListChildrenTypes()) {
				string caption = GetNodeDisplayName(item.Value);
				if(result.ContainsKey(caption) && item.Value != null) {
					caption = ModelApplicationCreator.GetDefaultXmlName(item.Value);
					if(result.ContainsKey(caption)) {
						caption = item.Value.Name;
					}
				}
				result.Add(caption, item.Value);
			}
			ModelVirtualTreeAddItemAttribute[] addAttributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeAddItemAttribute>(nodeInfo.GeneratedClass, true);
			if(addAttributes != null && addAttributes.Length > 0) {
				ModelVirtualTreeAddItemAttribute addAttr = addAttributes[0];
				if(addAttr.RealParentNode != null) {
					foreach(Type nodeType in addAttr.SupportedTypes) {
						string caption = GetNodeDisplayName(nodeType);
						if(result.ContainsKey(caption) && nodeType != null) {
							caption = ModelApplicationCreator.GetDefaultXmlName(nodeType);
							if(result.ContainsKey(caption)) {
								caption = nodeType.Name;
							}
						}
						result.Add(caption, nodeType);
					}
				}
			}
			return result;
		}
		public Dictionary<string, Type> GetChildNodeTypes(ModelNode node) {
			Dictionary<string, Type> result = new Dictionary<string, Type>();
			if(node != null) {
				if(!IsReadOnly(node, (ModelNode)null)) {
					result = GetListChildNodeTypes(node.NodeInfo);
				}
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanDragNode(ModelNode destNode, params ModelNode[] draggedNodes) {
			foreach(ModelNode node in draggedNodes) {
				if(IsReadOnly(node.Parent, node)) {
					return false;
				}
			}
			return CanAddNode(destNode, draggedNodes);
		}
		public bool CanAddNode(ModelNode destNode, params ModelNode[] addingNodes) {
			if(addingNodes.Length == 0 || destNode == null) {
				return false;
			}
			foreach(ModelNode node in addingNodes) {
				if(node == null) {
					return false;
				}
				bool result = false;
				foreach(Type childType in GetChildNodeTypes(destNode).Values) {
					ModelVirtualTreeCreatableItemsFilterAttribute[] filterAttributes = AttributeHelper.GetAttributesConsideringInterfaces<ModelVirtualTreeCreatableItemsFilterAttribute>(node.GetType(), true);
					bool disableByFilterAttribute = false;
					if(filterAttributes != null) {
						foreach(ModelVirtualTreeCreatableItemsFilterAttribute att in filterAttributes) {
							if(att.FilteredTypes != null) {
								foreach(Type filerType in att.FilteredTypes) {
									if(filerType.IsAssignableFrom(destNode.GetType())) {
										disableByFilterAttribute = true;
										break;
									}
								}
								if(disableByFilterAttribute) {
									break;
								}
							}
						}
					}
					if(!disableByFilterAttribute && node.NodeInfo.GeneratedClass.IsAssignableFrom(childType)) {
						result = true;
					}
				}
				if(!result) {
					return false;
				}
			}
			return true;
		}
		public string GetModelNodeDisplayValue(IModelNode modelNode) {
			lock(lockObject) {
				Type modelNodeType = modelNode.GetType();
				DisplayPropertyAttribute attribute;
				if(!nodeDisplayPropertyAttributeCache.TryGetValue(modelNodeType, out attribute)) {
					attribute = GetModelNodeDisplayPropertyAttribute(modelNodeType);
					nodeDisplayPropertyAttributeCache[modelNodeType] = attribute;
				}
				return GetModelNodeDisplayValue(modelNode, attribute);
			}
		}
		public static string GetModelNodeDisplayValue_Static(IModelNode modelNode) {
			return GetModelNodeDisplayValue(modelNode, GetModelNodeDisplayPropertyAttribute(modelNode.GetType()));
		}
		private static string GetModelNodeDisplayValue(IModelNode modelNode, DisplayPropertyAttribute displayPropertyAttribute) {
			if(displayPropertyAttribute != null) {
				string propertyName = displayPropertyAttribute.PropertyName;
				object displayValue = TypeHelper.GetPropertyValue(modelNode, propertyName);
				if(displayValue != null) {
					return displayValue.ToString();
				}
				else {
					return ((ModelNode)modelNode).Id;
				}
			}
			return ((ModelNode)modelNode).Id;
		}
		private static DisplayPropertyAttribute GetModelNodeDisplayPropertyAttribute(Type modelNodeType) {
			DisplayPropertyAttribute result = null;
			foreach(Type implementedInterface in TypeHelper.GetInterfaces(modelNodeType)) {
				DisplayPropertyAttribute[] displayPropertyAttributes = AttributeHelper.GetAttributes<DisplayPropertyAttribute>(implementedInterface, true);
				if(displayPropertyAttributes.Length > 0) {
					DisplayPropertyAttribute displayPropertyAttribute = displayPropertyAttributes[0];
					if(TypeHelper.ContainsProperty(implementedInterface, displayPropertyAttribute.PropertyName)) {
						result = displayPropertyAttribute;
						break;
					}
				}
			}
			return result;
		}
	}
}

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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Editors {
	public abstract class EditorInfoCalculator {
		internal delegate bool IsEditorDefault(IEditorTypeRegistration editorTypeRegistration);
		internal static Dictionary<IEditorTypeRegistration, IAliasRegistration> GetEditorToAliasMap(IEnumerable<EditorDescriptor> editorDescriptors, IsEditorDefault isEditorCompatible) {
			Dictionary<IEditorTypeRegistration, IAliasRegistration> result = new Dictionary<IEditorTypeRegistration, IAliasRegistration>();
			foreach(EditorDescriptor editorDescriptor in editorDescriptors) {
				IEditorTypeRegistration editorTypeRegistration = editorDescriptor.RegistrationParams as IEditorTypeRegistration;
				if(editorTypeRegistration != null) {
					bool isAliasFound = false;
					foreach(EditorDescriptor editorDescriptor_ in editorDescriptors) {
						IAliasRegistration aliasRegistration = editorDescriptor_.RegistrationParams as IAliasRegistration;
						if(aliasRegistration != null &&
							editorTypeRegistration.Alias == aliasRegistration.Alias &&
							editorTypeRegistration.ElementType == aliasRegistration.ElementType) {
							if(!aliasRegistration.IsDefaultAlias || (isEditorCompatible == null || isEditorCompatible(editorTypeRegistration)))
								result.Add((IEditorTypeRegistration)editorTypeRegistration, aliasRegistration);
							isAliasFound = true;
							break;
						}
					}
					if(!isAliasFound) {
						throw new Exception(string.Format("Cannot register the {0} editor type with the {1} alias. The alias for type {2} isn't registered.",
							editorTypeRegistration.EditorType, editorTypeRegistration.Alias, editorTypeRegistration.ElementType));
					}
				}
			}
			return result;
		}
	}
	public abstract class EditorInfoCalculator<T> : EditorInfoCalculator where T : IModelNode {
		protected abstract Type GetElementType(T modelNode);
		protected abstract EditorTypeRegistrations GetEditorTypeRegistrations(T modelNode);
		protected abstract bool IsCompatibleEditorDescriptor(EditorDescriptor editorDescriptor);
		protected abstract Type GetDefaultEditorTypeFromModel(IEditorTypeRegistration editorRegistration, IAliasRegistration aliasRegistration, IModelNode modelNode);
		private static List<Type> GetInheritanceTree(Type type) {
			List<Type> result = new List<Type>();
			if(TypeHelper.IsNullable(type)) {
				result.Add(type);
				type = TypeHelper.GetUnderlyingType(type);
			}
			result.Add(type);
			while(type.BaseType != null) {
				result.Add(type.BaseType);
				type = type.BaseType;
			}
			if(type.IsInterface) {
				result.Add(typeof(Object));
			}
			return result;
		}
		private static Dictionary<Type, IEnumerable<Type>> interfaceInheritanceTreeCache = new Dictionary<Type, IEnumerable<Type>>();
		private IEnumerable<Type> GetInterfaceInheritanceTree(Type type) {
			IEnumerable<Type> result;
			if(!interfaceInheritanceTreeCache.TryGetValue(type, out result)) {
				lock(interfaceInheritanceTreeCache) {
					if(!interfaceInheritanceTreeCache.TryGetValue(type, out result)) {
						result = GetInterfaceInheritanceTreeCore(type);
						interfaceInheritanceTreeCache.Add(type, result);
					}
				}
			}
			return result;
		}
		private IEnumerable<Type> GetInterfaceInheritanceTreeCore(Type type) {
			List<Type> result = new List<Type>();
			Type[] interfaces = TypeHelper.GetInterfaces(type);
			List<Type> interfacesList = new List<Type>(interfaces.Length + 1);
			if(type.IsInterface) {
				interfacesList.Add(type);
			}
			interfacesList.AddRange(interfaces);
			while(interfacesList.Count > 0) {
				for(int i = interfacesList.Count - 1; i >= 0; i--) {
					Type currentInterface = interfacesList[i];
					Boolean isCurrentInterfaceLeafDescendant = true;
					foreach(Type implementedInterface in interfacesList) {
						if(currentInterface != implementedInterface && currentInterface.IsAssignableFrom(implementedInterface)) {
							isCurrentInterfaceLeafDescendant = false;
							break;
						}
					}
					if(isCurrentInterfaceLeafDescendant) {
						result.Add(currentInterface);
						interfacesList.Remove(currentInterface);
					}
				}
			}
			return result;
		}
		private bool DefaultIsCompatible(Type editorType) { return true; }
		protected Type GetEditorType(T modelNode, IEnumerable<Type> targetTypes, EditorTypeRegistrations allEditorRegistrations) {
			return GetEditorType(modelNode, targetTypes, allEditorRegistrations, DefaultIsCompatible);
		}
		protected Type GetEditorType(T modelNode, IEnumerable<Type> targetTypes, EditorTypeRegistrations allEditorRegistrations, Predicate<Type> isEditorCompatible) {
			Type result = null;
			foreach(Type targetType in targetTypes) {
				IEnumerable<IEditorTypeRegistration> editorRegistrations = allEditorRegistrations.GetEditorRegistrations(targetType);
				IEditorTypeRegistration defaultEditorRegistration = allEditorRegistrations.GetDefaultEditorRegistration(targetType);
				foreach(IEditorTypeRegistration editorRegistration in editorRegistrations) {
					IAliasRegistration aliasRegistration = allEditorRegistrations.GetAliasRegistration(editorRegistration);
					Boolean? isCompatible = aliasRegistration.IsCompatible(modelNode);
					if(!isCompatible.HasValue && defaultEditorRegistration == null && editorRegistration.IsDefaultEditor) {
						result = editorRegistration.EditorType;
						break;
					}
					if(defaultEditorRegistration != aliasRegistration && isCompatible.HasValue && isCompatible.Value) {
						result = GetDefaultEditorTypeFromModel(editorRegistration, aliasRegistration, modelNode);
						break;
					}
				}
				if(result == null && defaultEditorRegistration != null) {
					result = GetDefaultEditorTypeFromModel(defaultEditorRegistration, null, modelNode);
				}
				if(result != null && isEditorCompatible(result)) {
					break;
				}
			}
			return result;
		}
		protected Type GetEditorType(T modelNode, Predicate<Type> isEditorCompatible) {
			Type elementType = GetElementType(modelNode);
			EditorTypeRegistrations editorRegistrations = GetEditorTypeRegistrations(modelNode);
			IEnumerable<Type> interfaceTypes = GetInterfaceInheritanceTree(elementType);
			Type interfaceEditorType = GetEditorType(modelNode, interfaceTypes, editorRegistrations, isEditorCompatible);
			List<Type> objectTypes = GetInheritanceTree(elementType);
			Type objectEditorType = GetEditorType(modelNode, objectTypes, editorRegistrations, isEditorCompatible);
			Type result = objectEditorType;
			if(interfaceEditorType != null && !interfaceEditorType.IsAssignableFrom(objectEditorType)) {
				result = interfaceEditorType;
			}
			return result;
		}
		public Type GetEditorType(T modelNode) {
			return GetEditorType(modelNode, DefaultIsCompatible);
		}
		public IEnumerable<Type> GetEditorsType(T modelNode) {
			List<Type> result = new List<Type>();
			Type elementType = GetElementType(modelNode);
			if(elementType != null) {
				List<Type> possibleTypes = new List<Type>(TypeHelper.GetInterfaces(elementType));
				possibleTypes.AddRange(GetInheritanceTree(elementType));
				EditorTypeRegistrations editorTypeRegistrations = GetEditorTypeRegistrations(modelNode);
				foreach(Type possibleType in possibleTypes) {
					IEnumerable<IEditorTypeRegistration> editorRegistrations = editorTypeRegistrations.GetEditorRegistrations(possibleType);
					foreach(IEditorTypeRegistration editorRegistration in editorRegistrations) {
						IAliasRegistration aliasRegistration = editorTypeRegistrations.GetAliasRegistration(editorRegistration);
						Boolean? isCompatible = aliasRegistration.IsCompatible(modelNode);
						if((!isCompatible.HasValue || isCompatible.Value)
							&& !result.Contains(editorRegistration.EditorType)) {
							result.Add(editorRegistration.EditorType);
						}
					}
				}
			}
			return result;
		}
	}
	public class ClassEditorInfoCalculator : EditorInfoCalculator<IModelClass> {
		public ClassEditorInfoCalculator() : base() { }
		protected override EditorTypeRegistrations GetEditorTypeRegistrations(IModelClass modelNode) {
			return ((IModelSources)modelNode.Application).EditorDescriptors.ListEditorRegistrations;
		}
		protected override Type GetElementType(IModelClass modelClass) {
			return modelClass.TypeInfo.Type;
		}
		protected override bool IsCompatibleEditorDescriptor(EditorDescriptor editorDescriptor) {
			return editorDescriptor is ListEditorDescriptor;
		}
		protected override Type GetDefaultEditorTypeFromModel(IEditorTypeRegistration registration, IAliasRegistration aliasRegistration, IModelNode modelNode) { 
			IModelViews views = modelNode.Application.Views;
			if (views != null && registration.ElementType == typeof(object) && views.DefaultListEditor != null) {
				return views.DefaultListEditor;
			}
			return registration.EditorType;
		}
	}
	public class MemberEditorInfoCalculator : EditorInfoCalculator<IModelMember> {
		protected override EditorTypeRegistrations GetEditorTypeRegistrations(IModelMember modelNode) {
			return ((IModelSources)modelNode.Application).EditorDescriptors.PropertyEditorRegistrations;
		}
		protected override Type GetDefaultEditorTypeFromModel(IEditorTypeRegistration registration, IAliasRegistration aliasRegistration, IModelNode modelNode) {
			IModelRegisteredViewItems viewItems = modelNode.Application.ViewItems;
			if (viewItems != null) {
				IModelRegisteredPropertyEditors propertyEditorsReg = viewItems.PropertyEditors;
				bool isDefaulObjectEditor = aliasRegistration == null ? registration.ElementType == typeof(Object) : ModelRegisteredViewItemsGenerator.GetIsDefaultEditor(aliasRegistration);
				if (isDefaulObjectEditor) {
					if (propertyEditorsReg.DefaultItemType != null) {
						return propertyEditorsReg.DefaultItemType;
					}
				} else {
					IModelRegisteredPropertyEditor propertyEditorReg = null;
					if (aliasRegistration != null && aliasRegistration.HasCompatibleDelegate) {
						propertyEditorReg = propertyEditorsReg[aliasRegistration.Alias];
					} else {
					propertyEditorReg = propertyEditorsReg[registration.ElementType.FullName];
					}
					if (propertyEditorReg != null && propertyEditorReg.EditorType != null) {
						return propertyEditorReg.EditorType;
					}
				}
			}
			return registration.EditorType;
		}
		protected override Type GetElementType(IModelMember modelNode) {
			if(modelNode.MemberInfo != null) {
				return modelNode.MemberInfo.MemberType;
			}
			else {
				if(modelNode.IsCustom) {
					return modelNode.Type;
				}
			}
			return null;
		}
		protected override bool IsCompatibleEditorDescriptor(EditorDescriptor editorDescriptor) {
			return editorDescriptor is PropertyEditorDescriptor;
		}
		public Type GetEditorType(IModelMember modelMember, string editorAlias) {
			Type modelMemberType = modelMember.Type;
			foreach (IEditorTypeRegistration item in GetEditorTypeRegistrations(modelMember)) {
				if (item.Alias == editorAlias && item.ElementType.IsAssignableFrom(modelMemberType)) {
					return item.EditorType;
				}
			}
			return GetEditorType(modelMember);
		}
		public Type GetEditorType(IModelMember modelMember, Type supportedType) {
			return GetEditorType(modelMember, delegate(Type editorType) { return supportedType.IsAssignableFrom(editorType); });
		}
		public Type GetEditorType(IModelViewItem detailViewItem) {
			if (detailViewItem is IModelMemberViewItem) {
				return GetEditorType((((IModelMemberViewItem)detailViewItem).ModelMember));
			}
			IEditorTypeRegistration registation = null;
			foreach (ViewItemDescriptor editorDescriptor in ((IModelSources)detailViewItem.Application).EditorDescriptors.ViewItems) {
				if (editorDescriptor.RegistrationParams.EditorType != null &&
					editorDescriptor.RegistrationParams.ElementType.IsAssignableFrom(detailViewItem.GetType())) {
					registation = editorDescriptor.RegistrationParams;
				}
			}
			if (registation != null) {
				foreach (IModelRegisteredViewItem modelDetailViewItem in detailViewItem.Application.ViewItems) {
					if (modelDetailViewItem.Name == registation.Alias) {
						return modelDetailViewItem.DefaultItemType;
					}
				}
			}
			return null;
		}
	}
}

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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.NodeGenerators {
	public class ModelBOModelClassNodesGenerator : ModelNodesGeneratorBase {
		public static string GetTypeId(Type type) {
			CheckType(type);
			return GetTypeIdInternal(type, curType => curType.FullName);
		}
		public static bool TryGetTypeId(Type type, out string typeId) {
			if(type == null || type.ContainsGenericParameters) {
				typeId = null;
				return false;
			}
			else {
				typeId = GetTypeIdInternal(type, curType => curType.FullName);
				return true;
			}
		}
		public static string GetTypeNameId(Type type) {
			CheckType(type);
			return GetTypeIdInternal(type, curType => curType.Name);
		}
		private static void CheckType(Type type) {
			Guard.ArgumentNotNull(type, "type");
			if(type.ContainsGenericParameters) {
				throw new ArgumentException(string.Format("The {0} type contains generic parameters.", type.FullName), "type");
			}
		}
		private static string GetTypeIdInternal(Type type, Func<Type, string> getTypeIdPart) {
			string result;
			if(type.IsGenericType) {
				result = GetTypeIdPartWithoutGenericPart(type, getTypeIdPart);
				result += '[';
				Type[] genericArguments = type.GetGenericArguments();
				result += GetTypeIdInternal(genericArguments[0], getTypeIdPart);
				for(int i = 1; i < genericArguments.Length; i++) {
					result += ',';
					result += GetTypeIdInternal(genericArguments[i], getTypeIdPart);
				}
				result += ']';
			}
			else {
				result = getTypeIdPart(type);
			}
			return result;
		}
		private static string GetTypeIdPartWithoutGenericPart(Type type, Func<Type, string> getTypeIdPart) {
			string typeIdPart = getTypeIdPart(type);
			return typeIdPart.Remove(typeIdPart.IndexOf('`'));
		}
		protected override void GenerateNodesCore(ModelNode node) {
			foreach(Type type in ((IModelSources)node.Application).BOModelTypes) {
				GenerateBOClass(node, XafTypesInfo.Instance.FindTypeInfo(type));
			}
		}
		private void GenerateBOClass(ModelNode node, ITypeInfo typeInfo) {
			if(FindBOClass(node, typeInfo) == null) {
				IModelClass classNode = node.AddNode<IModelClass>(GetTypeId(typeInfo.Type));
				classNode.SetValue<ITypeInfo>("TypeInfo", typeInfo);
				classNode.SetValue<string>("DefaultDetailView_ID", ModelDetailViewNodesGenerator.GetDetailViewId(typeInfo.Type));
				classNode.SetValue<string>("DefaultListView_ID", ModelListViewNodesGenerator.GetListViewId(typeInfo.Type));
				classNode.SetValue<string>("DefaultLookupListView_ID", ModelLookupViewNodesGeneratorHelper.GetLookupListViewId(typeInfo.Type));
				classNode.Caption = GetBOModelCaption(classNode, typeInfo);
				if(typeInfo.KeyMember != null && typeInfo.KeyMember.Owner == typeInfo) {
					classNode.KeyProperty = typeInfo.KeyMember.Name;
				}
				foreach(ModelDefaultAttribute attribute in typeInfo.FindAttributes<ModelDefaultAttribute>()) {
					((ModelNode)classNode).SetSerializedValue(attribute.PropertyName, attribute.PropertyValue);
				}
				foreach(ModelDefaultAttribute attribute in typeInfo.FindAttributes<ModelDefaultAttribute>(false)) {
					((ModelNode)classNode).SetSerializedValue(attribute.PropertyName, attribute.PropertyValue);
				}
			}
		}
		private IModelClass FindBOClass(ModelNode node, ITypeInfo typeInfo) {
			return ((IModelBOModel)node).GetClass(typeInfo.Type);
		}
		private string GetBOModelCaption(IModelClass classNode, ITypeInfo typeInfo) {
			string classCaption = GetCaptionFromAttributes(typeInfo);
			if(string.IsNullOrEmpty(classCaption) && !TryGetCaptionForInterface(typeInfo, out classCaption)) {
				classCaption = CaptionHelper.ConvertCompoundName(classNode.ShortName);
			}
			return classCaption;
		}
		private string GetCaptionFromAttributes(ITypeInfo typeInfo) {
			string result = null;
			System.ComponentModel.DisplayNameAttribute displayNameAttribute = typeInfo.FindAttribute<System.ComponentModel.DisplayNameAttribute>(false);
			if(displayNameAttribute != null) {
				result = displayNameAttribute.DisplayName;
			}
			if(string.IsNullOrEmpty(result)) {
				XafDisplayNameAttribute attribute = typeInfo.FindAttribute<XafDisplayNameAttribute>(false);
				if(attribute != null) {
					result = attribute.DisplayName;
				}
			}
			return result;
		}
		private bool TryGetCaptionForInterface(ITypeInfo typeInfo, out string caption) {
			caption = null;
			bool result = false;
			if(typeInfo.IsInterface && typeInfo.IsDomainComponent) {
				string intermediateCaption;
				Type generatedEntityType = FindGeneratedEntityType(typeInfo);
				if(generatedEntityType != null) {
					intermediateCaption = generatedEntityType.Name;
				}
				else {
					intermediateCaption = typeInfo.Name;
					if(intermediateCaption.Length > 1 && intermediateCaption[0] == 'I' && char.IsUpper(intermediateCaption[1])) {
						intermediateCaption = intermediateCaption.Substring(1);
					}
				}
				caption = CaptionHelper.ConvertCompoundName(intermediateCaption);
				result = true;
			}
			return result;
		}
		private Type FindGeneratedEntityType(ITypeInfo typeInfo) {
			Type generatedEntityType = null;
			if(typeInfo is BaseInfo && ((BaseInfo)typeInfo).Source is IDCEntityStore) {
				generatedEntityType = ((IDCEntityStore)((BaseInfo)typeInfo).Source).GetGeneratedEntityType(typeInfo.Type);
			}
			return generatedEntityType;
		}
	}
	public class ModelBOModelMemberNodesGenerator : ModelNodesGeneratorBase {
		private IEnumerable<IMemberInfo> CollectModelClassMembers(IModelClass modelClass) {
			ITypeInfo typeInfo = modelClass.TypeInfo;
			return typeInfo.OwnMembers;
		}
		protected override void GenerateNodesCore(ModelNode node) {
			IModelClass modelClass = (IModelClass)node.Parent;
			IEnumerable<IMemberInfo> members = CollectModelClassMembers(modelClass);
			foreach(IMemberInfo memberInfo in members) {
				if((memberInfo.IsVisible || (memberInfo.IsPublic && memberInfo.Owner.KeyMembers.Contains(memberInfo)))
					&& (modelClass.OwnMembers[memberInfo.Name] == null)
				) {
					CreateMemberNode(modelClass, memberInfo);
				}
			}
		}
		protected virtual void CreateMemberNode(IModelClass classNode, IMemberInfo memberInfo) {
			IModelMember propertyNode = classNode.OwnMembers.AddNode<IModelMember>(memberInfo.Name);
			propertyNode.SetValue<IMemberInfo>("MemberInfo", memberInfo);
			IndexAttribute indexAttribute = memberInfo.FindAttribute<IndexAttribute>();
			if(indexAttribute != null) {
				propertyNode.Index = (int)indexAttribute.Value;
			}
			propertyNode.Caption = GetMemberCaption(memberInfo);
			foreach(ModelDefaultAttribute attribute in memberInfo.FindAttributes<ModelDefaultAttribute>()) {
				((ModelNode)propertyNode).SetSerializedValue(attribute.PropertyName, attribute.PropertyValue);
			}
			Dictionary<string, object> exportedAttributeValues = new Dictionary<string, object>();
			foreach(ModelExportedValuesAttribute attribute in memberInfo.FindAttributes<ModelExportedValuesAttribute>()) {
				attribute.FillValues(exportedAttributeValues);
			}
			if(memberInfo.FindAttribute<ImageEditorAttribute>() != null && memberInfo.MemberType == typeof(byte[])) {
				UpdatePropertyEditorTypeForBinaryImage(propertyNode);
			}
			((ModelNode)propertyNode).SetSerializableValues(exportedAttributeValues);
		}
		private void UpdatePropertyEditorTypeForBinaryImage(IModelMember propertyNode) {
			EditorDescriptors editorDescriptors = ((IModelSources)propertyNode.Application).EditorDescriptors;
			if(editorDescriptors != null && editorDescriptors.PropertyEditorRegistrations != null) {
				IEnumerable<IEditorTypeRegistration> editorTypeRegistrations = editorDescriptors.PropertyEditorRegistrations.TypeRegistrations;
				foreach(IEditorTypeRegistration typeRegistration in editorTypeRegistrations) {
					if(typeRegistration.Alias == EditorAliases.ImagePropertyEditor) {
						propertyNode.PropertyEditorType = typeRegistration.EditorType;
					}
				}
			}
		}
		public static string GetMemberCaption(IMemberInfo memberInfo) {
			string memberCaption = null;
			if(string.IsNullOrEmpty(memberCaption)) {
				System.ComponentModel.DisplayNameAttribute displayNameAttr =
					memberInfo.FindAttribute<System.ComponentModel.DisplayNameAttribute>();
				if(displayNameAttr != null) {
					memberCaption = displayNameAttr.DisplayName;
				}
			}
			if(string.IsNullOrEmpty(memberCaption)) {
				XafDisplayNameAttribute attribute = memberInfo.FindAttribute<XafDisplayNameAttribute>();
				if(attribute != null) {
					memberCaption = attribute.DisplayName;
				}
			}
			if(string.IsNullOrEmpty(memberCaption)) {
				memberCaption = memberInfo.DisplayName;
			}
			if(string.IsNullOrEmpty(memberCaption)) {
				memberCaption = CaptionHelper.ConvertCompoundName(memberInfo.Name);
			}
			return memberCaption;
		}
	}
	public sealed class ModelClassInterfacesNodesGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			IModelClassInterfaces links = (IModelClassInterfaces)node;
			IModelBOModel boModel = node.Application.BOModel;
			IModelClass[] list = DevExpress.ExpressApp.Model.DomainLogics.ModelClassLogic.GetInterfaces((IModelClass)node.Parent);
			foreach(IModelClass item in list) {
				IModelInterfaceLink link = links.AddNode<IModelInterfaceLink>(item.Name);
				link.SetValue<IModelClass>("Interface", item);
			}
		}
	}
}

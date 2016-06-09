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
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.DomainLogics {
	[DomainLogic(typeof(IModelBOModel))]
	public static class ModelBOModelLogic {
		public static IModelClass GetClass(IModelBOModel modelBOModel, Type type) {
			string typeId;
			return ModelBOModelClassNodesGenerator.TryGetTypeId(type, out typeId) ? modelBOModel[typeId] : null;
		}
	}
	[DomainLogic(typeof(IModelClass))]
	public static class ModelClassLogic {
		public const string ModelViewsByClassCriteria = "(AsObjectView Is Not Null) And (AsObjectView.ModelClass Is Not Null) And (IsAssignableFromViewModelClass('@This.TypeInfo', AsObjectView))";
		private static ClassEditorInfoCalculator calculator = new ClassEditorInfoCalculator();
		private static DefaultListViewOptionsAttribute defaultListViewOptions = new DefaultListViewOptionsAttribute();
		internal static IModelClass[] GetInterfaces(IModelClass classModel) {
			IModelBOModel boModel = classModel.Application.BOModel;
			List<IModelClass> list = new List<IModelClass>();
			foreach(ITypeInfo info in classModel.TypeInfo.ImplementedInterfaces) {
				IModelClass item = boModel.GetClass(info.Type);
				if(item != null) {
					list.Add(item);
				}
			}
			return list.ToArray();
		}
		internal static IModelClass[] GetDescendants(IModelClass classModel) {
			IModelBOModel boModel = classModel.Application.BOModel;
			List<IModelClass> list = new List<IModelClass>();
			foreach(ITypeInfo info in classModel.TypeInfo.Descendants) {
				IModelClass item = boModel.GetClass(info.Type);
				if(item != null) {
					list.Add(item);
				}
			}
			return list.ToArray();
		}
		public static IModelList<IModelMember> Get_AllMembers(IModelClass modelClass) {
			IEnumerable<IModelMember> source = null;
			if(modelClass.TypeInfo.IsInterface) {   
				Dictionary<string, IModelMember> map = new Dictionary<string, IModelMember>();
				foreach(IModelMember ownMember in modelClass.OwnMembers) {
					map.Add(ownMember.Name, ownMember);
				}
				IModelClass[] interfaceNodes = ModelClassLogic.GetInterfaces(modelClass);
				foreach(IModelClass interfaceNode in interfaceNodes) {
					foreach(IModelMember member in interfaceNode.OwnMembers) {
						if(!map.ContainsKey(member.Name)) {
							map.Add(member.Name, member);
						}
					}
				}
				source = map.Values;
			}
			else {
				IModelClass current = modelClass;
				List<IModelMember> list = new List<IModelMember>();
				while(current != null) {
					list.AddRange(current.OwnMembers);
					current = current.BaseClass;
				}
				source = list;
			}
			CalculatedModelNodeList<IModelMember> result = new CalculatedModelNodeList<IModelMember>("Name", source);
			return result;
		}
		private static IModelMember FindMemberCore(IModelClass modelClass, string memberName) {
			IModelMember modelMember = modelClass.OwnMembers[memberName];
			if(modelMember == null) {
				IModelClass baseModelClass = modelClass.BaseClass;
				if(baseModelClass != null) {
					modelMember = FindMemberCore(baseModelClass, memberName);
				}
			}
			if(modelMember == null) {
				IModelClass[] list = ModelClassLogic.GetInterfaces(modelClass);
				foreach(IModelClass item in list) {
					modelMember = item.OwnMembers[memberName];
					if(modelMember != null) break;
				}
			}
			return modelMember;
		}
		private static string[] SplitComplexMemberName(string memberName) {
			return (string[])DevExpress.Data.Filtering.Helpers.EvaluatorProperty.Create(
				new DevExpress.Data.Filtering.OperandProperty(memberName)).PropertyPathTokenized.Clone();
		}
		private static IModelClass FindDescendantModelClass(IModelClass modelClass, string className) {
			IModelClass[] list = ModelClassLogic.GetDescendants(modelClass);
			IModelClass result = Array.Find<IModelClass>(list, delegate(IModelClass x) { return x.Name == className; });
			if(result == null) {
				result = Array.Find<IModelClass>(list, delegate(IModelClass x) { return x.TypeInfo.Name == className; });
			}
			return result;
		}
		private static IModelMember FindComplexMember(IModelClass modelClass, string memberName) {
			string[] parts = SplitComplexMemberName(memberName);
			int partsLength = parts.Length;
			IModelBOModel modelBOModel = (IModelBOModel)modelClass.Parent;
			IModelClass currentModelClass = modelClass;
			IModelMember currentModelMember = null;
			int i = 0;
			while(i < partsLength) {
				if(currentModelClass == null) {
					return null;
				}
				string currentMemberName = parts[i];
				if(string.IsNullOrEmpty(currentMemberName)) {
					return null; 
				}
				if(currentMemberName[0] == '<') {	
					int k = currentMemberName.IndexOf('>', 1);
					if(k >= 0) {
						string descendantClassName = currentMemberName.Substring(1, k - 1);
						currentMemberName = currentMemberName.Substring(k + 1);
						currentModelClass = FindDescendantModelClass(currentModelClass, descendantClassName);
						if(currentModelClass == null) {
							return null;
						}
					}
					else {
						return null;
					}
				}
				currentModelMember = FindMemberCore(currentModelClass, currentMemberName);
				if(currentModelMember == null) {
					return null;
				}
				if(currentModelMember.Type.IsValueType) {
					int j = i + 1;
					while(j < partsLength && currentModelMember.Type.IsValueType) {
						currentMemberName += "." + parts[j];
						currentModelMember = FindMemberCore(currentModelClass, currentMemberName);
						if(currentModelMember == null) {
							return null;
						}
						++j;
					}
					i = j - 1;
				}
				currentModelClass = modelBOModel.GetClass(currentModelMember.Type);
				++i;
			}
			return currentModelMember;
		}
		public static IModelMember FindMember(IModelClass modelClass, string memberName) {
			Guard.ArgumentNotNullOrEmpty(memberName, "memberName");
			IModelMember modelMember = FindMemberCore(modelClass, memberName);
			if(modelMember == null) {
				modelMember = FindComplexMember(modelClass, memberName);	
			}
			return modelMember;
		}
		public static IModelMember FindOwnMember(IModelClass modelClass, string memberName) {
			return modelClass.OwnMembers[memberName];
		}
		public static IModelClass Get_BaseClass(IModelClass modelClass) {
			ITypeInfo typeInfo = modelClass.TypeInfo;
			if(typeInfo != null && typeInfo.Base != null) {
				return modelClass.Application.BOModel.GetClass(typeInfo.Base.Type);
			}
			return null;
		}
		public static string Get_ShortName(IModelClass modelClass) {
			ITypeInfo typeInfo = modelClass.TypeInfo;
			if(typeInfo != null) {
				Type type = typeInfo.Type;
				if(!type.ContainsGenericParameters) {
					return ModelBOModelClassNodesGenerator.GetTypeNameId(type);
				}
			}
			return null;
		}
		public static string Get_DefaultProperty(IModelClass modelClass) {
			ITypeInfo typeInfo = modelClass.TypeInfo;
			if(typeInfo != null) {
				if((typeInfo.DefaultMember != null) && typeInfo.DefaultMember.IsVisible && !typeInfo.KeyMembers.Contains(typeInfo.DefaultMember)) {
					return typeInfo.DefaultMember.Name;
				}
			}
			return "";
		}
		public static string Get_FriendlyKeyProperty(IModelClass modelClass) {
			return FriendlyKeyPropertyAttribute.FindFriendlyKeyMemberName(modelClass.TypeInfo, true);
		}
		public static string Get_ObjectCaptionFormat(IModelClass modelClass) {
			string result = "";
			ITypeInfo typeInfo = modelClass.TypeInfo;
			if(typeInfo != null) {
				ObjectCaptionFormatAttribute objectCaptionFormatAttribute = typeInfo.FindAttribute<ObjectCaptionFormatAttribute>(true);
				if(objectCaptionFormatAttribute != null && !String.IsNullOrEmpty(objectCaptionFormatAttribute.FormatString)) {
					result = objectCaptionFormatAttribute.FormatString;
				}
				else {
					string friendlyKeyProperty = ModelClassLogic.Get_FriendlyKeyProperty(modelClass);
					SplitString objectCaptionFormat = new SplitString();
					objectCaptionFormat.Separator = ": ";
					if(!string.IsNullOrEmpty(friendlyKeyProperty) && modelClass.OwnMembers[friendlyKeyProperty] != null)
						objectCaptionFormat.FirstPart = "{0:" + friendlyKeyProperty + "}";
					if(typeInfo.DefaultMember != null && typeInfo.Equals(typeInfo.DefaultMember.Owner) && typeInfo.DefaultMember.Name != friendlyKeyProperty)
						objectCaptionFormat.SecondPart = "{0:" + typeInfo.DefaultMember.Name + "}";
					result = objectCaptionFormat.Text;
				}
				if(string.IsNullOrEmpty(result)) {
					if(typeInfo.IsInterface) {
						if(typeInfo.DefaultMember != null) {
							IModelClass modelImplementedInterface = modelClass.Application.BOModel.GetClass(typeInfo.DefaultMember.Owner.Type);
							if(modelImplementedInterface != null) {
								result = modelImplementedInterface.ObjectCaptionFormat;
							}
						}
					}
					else {
						IModelClass modelBaseClass = modelClass.BaseClass;
						if(modelBaseClass != null) {
							result = modelBaseClass.ObjectCaptionFormat;
						}
					}
				}
			}
			return result;
		}
		public static string Get_ImageName(IModelClass modelClass) {
			string imageName = "";
			ITypeInfo typeInfo = modelClass.TypeInfo;
			if(typeInfo != null) {
				ImageNameAttribute imageNameAttribute =
					typeInfo.FindAttribute<ImageNameAttribute>();
				if(imageNameAttribute != null) {
					imageName = imageNameAttribute.ImageName;
				}
			}
			if(string.IsNullOrEmpty(imageName)) {
				IModelClass modelBaseClass = modelClass.BaseClass;
				if(modelBaseClass != null)
					imageName = modelBaseClass.ImageName;
			}
			return imageName;
		}
		public static string Get_DefaultListViewImage(IModelClass modelClass) {
			return modelClass.ImageName;
		}
		public static string Get_DefaultDetailViewImage(IModelClass modelClass) {
			return modelClass.ImageName;
		}
		public static bool Get_DefaultListViewAllowEdit(IModelClass modelClass) {
			bool result = defaultListViewOptions.AllowEdit;
			ITypeInfo typeInfo = modelClass.TypeInfo;
			if(typeInfo != null) {
				DefaultListViewOptionsAttribute defaultListViewOptionsAttribute = typeInfo.FindAttribute<DefaultListViewOptionsAttribute>();
				if(defaultListViewOptionsAttribute != null) {
					result = defaultListViewOptionsAttribute.AllowEdit;
				}
			}
			return result;
		}
		public static DevExpress.ExpressApp.MasterDetailMode Get_DefaultListViewMasterDetailMode(IModelClass modelClass) {
			MasterDetailMode result = defaultListViewOptions.MasterDetailMode;
			ITypeInfo typeInfo = modelClass.TypeInfo;
			if(typeInfo != null) {
				DefaultListViewOptionsAttribute defaultListViewOptionsAttribute = typeInfo.FindAttribute<DefaultListViewOptionsAttribute>();
				if(defaultListViewOptionsAttribute != null) {
					result = defaultListViewOptionsAttribute.MasterDetailMode;
				}
			}
			return result;
		}
		public static bool Get_IsCreatableItem(IModelClass modelClass) {
			ITypeInfo typeInfo = modelClass.TypeInfo;
			if(typeInfo != null) {
				CreatableItemAttribute creatableItemAttribute =
					typeInfo.FindAttribute<CreatableItemAttribute>();
				if(creatableItemAttribute != null) {
					return creatableItemAttribute.IsCreatableItem;
				}
				if(typeInfo.FindAttribute<DefaultClassOptionsAttribute>() != null) {
					return true;
				}
			}
			return false;
		}
	}
	[DomainLogic(typeof(IModelBOModelClassMembers))]
	public static class ModelBOModelClassMembersLogic {
		public static void AfterConstruction(IModelBOModelClassMembers members) {
			((ModelNode)members).NodeInfo.ChildNodesComparison = SortChildNodesHelper.DoSortNodesByIndex;
		}
	}
}

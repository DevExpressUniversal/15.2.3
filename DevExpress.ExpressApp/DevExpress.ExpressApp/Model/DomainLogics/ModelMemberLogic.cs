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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model.Core;
namespace DevExpress.ExpressApp.Model.DomainLogics {
	[DomainLogic(typeof(IModelMember))]
	public static class ModelMemberLogic {
		private static bool cacheEnabled = false;
		private static Dictionary<IMemberInfo, Type> modelMemberEditorTypeCache = new Dictionary<IMemberInfo, Type>();
		[System.ComponentModel.Browsable(false), System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		internal static bool CacheEnabled {
			get {
				return cacheEnabled;
			}
			set {
				cacheEnabled = value;
				if(!cacheEnabled) {
					ClearCache();
				}
			}
		}
		private static void ClearCache() {
			modelMemberEditorTypeCache.Clear();
		}
		public static int MaxShortStringLength = 255;
		private static bool IsShortString(int size) {
			return ((size >= 0) && (size <= MaxShortStringLength));
		}
		private static AttributeType FindAttribute<AttributeType>(IModelMember modelMember) where AttributeType : Attribute {
			AttributeType result = null;
			IMemberInfo memberInfo = modelMember.MemberInfo;
			if(memberInfo != null) {
				result = memberInfo.FindAttribute<AttributeType>();
			}
			return result;
		}
		private static bool? GetIsVisibleInView<T>(IModelMember modelMember) where T : ModelExportedBoolValueAttribute {
			bool? isVisibleInCurrentTypeView = GetAttributeValue<T, bool?>(modelMember, null);
			if(isVisibleInCurrentTypeView != null) {
				return isVisibleInCurrentTypeView;
			}
			BrowsableAttribute browsableAttribute = FindAttribute<BrowsableAttribute>(modelMember);
			if(browsableAttribute != null) {
				return browsableAttribute.Browsable;
			}
			return null;
		}
		internal delegate T GetProperty<T>(IMemberInfo memberInfo);
		internal static T GetMemberInfoProperty<T>(IModelMember node, GetProperty<T> getProperty) {
			IMemberInfo memberInfo = node.MemberInfo;
			if(memberInfo != null) {
				return getProperty(memberInfo);
			}
			else {
				return default(T);
			}
		}
		public static V GetAttributeValue<T, V>(IModelMember node, V defaultValue) where T : ModelExportedValueAttribute {
			return GetMemberInfoProperty<V>(node, delegate(IMemberInfo memberInfo) {
				ModelExportedValueAttribute attribute = memberInfo.FindAttribute<T>();
				if(attribute != null) {
					return (V)attribute.Value;
				}
				return defaultValue;
			});
		}
		public static V GetAttributeValue<T, V>(IModelMember node) where T : ModelExportedValueAttribute {
			return GetAttributeValue<T, V>(node, default(V));
		}
		public static string GetAttributeStringValue<T>(IModelMember node) where T : ModelExportedValueAttribute {
			return GetAttributeValue<T, string>(node);
		}
		public static IModelClass Get_ModelClass(IModelMember modelMember) {
			if(modelMember.Parent != null && modelMember.Parent.Parent != null) {
				return (IModelClass)(modelMember.Parent.Parent);
			}
			return null;
		}
		public static IMemberInfo Get_MemberInfo(IModelMember modelMember) {
			Guard.ArgumentNotNull(modelMember.ModelClass, "modelMember.ModelClass");
			Guard.ArgumentNotNull(modelMember.ModelClass.TypeInfo, "modelMember.ModelClass.TypeInfo");
			IMemberInfo memberInfo = modelMember.ModelClass.TypeInfo.FindMember(modelMember.Name);
			if(memberInfo != null) {
				modelMember.SetValue<IMemberInfo>("MemberInfo", memberInfo);
			}
			return memberInfo;
		}
		public static int? Get_Index(IModelMember modelMember) {
			return GetAttributeValue<IndexAttribute, int?>(modelMember);
		}
		public static int Get_Size(IModelMember modelMember) {
			if(((ModelNode)modelMember).HasValue("MemberInfo")) {
				return modelMember.MemberInfo.Size;
			}
			return -1;
		}
		public static Type Get_Type(IModelMember modelMember) {
			return GetMemberInfoProperty<Type>(modelMember, delegate(IMemberInfo memberInfo) { return memberInfo.MemberType; });
		}
		public static bool Get_AllowEdit(IModelMember modelMember) {
			return GetMemberInfoProperty<bool>(modelMember, delegate(IMemberInfo memberInfo) {
				return !(memberInfo.IsKey && memberInfo.IsAutoGenerate) && !(memberInfo.IsReadOnly && SimpleTypes.IsSimpleType(memberInfo.MemberType));
			});
		}
		public static AllowAdd Get_AllowAdd(IModelMember modelMember) {
			return GetMemberInfoProperty<AllowAdd>(modelMember, delegate(IMemberInfo memberInfo) {
				foreach(ModelDefaultAttribute allowAddAttribute in memberInfo.FindAttributes<ModelDefaultAttribute>()) {
					if(allowAddAttribute.PropertyName == "AllowAdd") {
						return (AllowAdd)Enum.Parse(typeof(AllowAdd), allowAddAttribute.PropertyValue);
					}
				}
				return AllowAdd.Default;
			});
		}
		public static string Get_DataSourceCriteria(IModelMember modelMember) {
			return GetAttributeStringValue<DataSourceCriteriaAttribute>(modelMember);
		}
		public static String Get_DataSourceCriteriaProperty(IModelMember modelMember) {
			return GetAttributeStringValue<DataSourceCriteriaPropertyAttribute>(modelMember);
		}
		public static bool Get_ImmediatePostData(IModelMember modelMember) {
			return GetAttributeValue<ImmediatePostDataAttribute, bool>(modelMember);
		}
		public static bool? Get_IsVisibleInDetailView(IModelMember modelMember) {
			return GetIsVisibleInView<VisibleInDetailViewAttribute>(modelMember);
		}
		public static bool? Get_IsVisibleInLookupListView(IModelMember modelMember) {
			return GetIsVisibleInView<VisibleInLookupListViewAttribute>(modelMember);
		}
		public static bool? Get_IsVisibleInListView(IModelMember modelMember) {
			return GetIsVisibleInView<VisibleInListViewAttribute>(modelMember);
		}
		public static string Get_LookupProperty(IModelMember modelMember) {
			return GetMemberInfoProperty<string>(modelMember, delegate(IMemberInfo memberInfo) {
				if(memberInfo.IsList && memberInfo.ListElementTypeInfo != null) {
					IMemberInfo lookupProperty = memberInfo.ListElementTypeInfo.DefaultMember;
					if(lookupProperty != null) {
						return lookupProperty.Name;
					}
				}
				IModelClass modelClass = modelMember.Application.BOModel.GetClass(memberInfo.MemberType);
				return modelClass == null ? "" : modelClass.DefaultProperty;
			});
		}
		public static bool Get_IsPassword(IModelMember modelMember) {
			return GetMemberInfoProperty<bool>(modelMember, delegate(IMemberInfo memberInfo) {
				PasswordPropertyTextAttribute passwordPropertyTextAttr =
					memberInfo.FindAttribute<PasswordPropertyTextAttribute>();
				if(passwordPropertyTextAttr != null) {
					return passwordPropertyTextAttr.Password;
				}
				return false;
			});
		}
		public static string Get_DisplayFormat(IModelMember modelMember) {
			return GetMemberInfoProperty<string>(modelMember, delegate(IMemberInfo memberInfo) {
				string lookupPropertyName = modelMember.LookupProperty;
				if(!string.IsNullOrEmpty(lookupPropertyName)) {
					IMemberInfo lookupMemberInfo = memberInfo.MemberTypeInfo.FindMember(lookupPropertyName);
					if(lookupMemberInfo != null) {
						IModelClass lookupPropertyClass = modelMember.Application.BOModel.GetClass(modelMember.Type);
						if(lookupPropertyClass != null) {
							IModelMember lookupModelMember = lookupPropertyClass.OwnMembers[lookupPropertyName];
							if(lookupModelMember != null) {
								return lookupModelMember.DisplayFormat;
							}
						}
					}
				}
				string result = string.Empty;
				IModelRegisteredPropertyEditor modelRegisteredPropertyEditor = FindRegisteredPropertyEditor(modelMember);
				if(modelRegisteredPropertyEditor != null) {
					result = modelRegisteredPropertyEditor.DefaultDisplayFormat;
				}
				else if(memberInfo != null) {
					result = FormattingProvider.GetDisplayFormat(memberInfo.MemberType);
				}
				return result;
			});
		}
		public static string Get_EditMask(IModelMember modelMember) {
			return GetMemberInfoProperty<string>(modelMember, delegate(IMemberInfo memberInfo) {
				string result = string.Empty;
				IModelRegisteredPropertyEditor modelRegisteredPropertyEditor = FindRegisteredPropertyEditor(modelMember);
				if(modelRegisteredPropertyEditor != null) {
					result = modelRegisteredPropertyEditor.DefaultEditMask;
				}
				else if(memberInfo != null) {
					result = FormattingProvider.GetEditMask(memberInfo.MemberType);
				}
				return result;
			});
		}
		public static EditMaskType Get_EditMaskType(IModelMember modelMember) {
			return GetMemberInfoProperty<EditMaskType>(modelMember, delegate(IMemberInfo memberInfo) {
				EditMaskType result = EditMaskType.Default;
				IModelRegisteredPropertyEditor modelRegisteredPropertyEditor = FindRegisteredPropertyEditor(modelMember);
				if(modelRegisteredPropertyEditor != null) {
					result = modelRegisteredPropertyEditor.DefaultEditMaskType;
				}
				else if(memberInfo != null) {
					result = FormattingProvider.GetEditMaskType(memberInfo.MemberType);
				}
				return result;
			});
		}
		private static IModelRegisteredPropertyEditor FindRegisteredPropertyEditor(IModelMember modelMember) {
			if((modelMember != null) && (modelMember.Application != null)) {
				IMemberInfo memberInfo = modelMember.MemberInfo;
				if(memberInfo != null) {
					Type fixedType = Nullable.GetUnderlyingType(memberInfo.MemberType) ?? memberInfo.MemberType;
					return modelMember.Application.ViewItems.PropertyEditors[fixedType.FullName];
				}
			}
			return null;
		}
		public static int Get_RowCount(IModelMember modelMember) {
			return GetMemberInfoProperty<int>(modelMember, delegate(IMemberInfo memberInfo) {
				if(memberInfo.MemberType == typeof(String) && !IsShortString(memberInfo.Size)) {
					return 3;
				}
				return 0;
			});
		}
		public static LookupEditorMode Get_LookupEditorMode(IModelMember modelMember) {
			IModelClass modelClass = null;
			IMemberInfo memberInfo = modelMember.MemberInfo;
			if(memberInfo != null) {
				modelClass = modelMember.Application.BOModel.GetClass(memberInfo.MemberType);
			}
			return modelClass != null ? modelClass.DefaultLookupEditorMode : LookupEditorMode.Auto;
		}
		private static MemberEditorInfoCalculator calculator = new MemberEditorInfoCalculator();
		public static string Get_CaptionForTrue(IModelMember modelMember) {
			IModelApplication modelApplication = modelMember.Application;
			if(modelApplication != null) {
				IModelLocalization modelLocalizationNode = modelApplication.Localization;
				if(modelLocalizationNode != null) {
					IModelLocalizationGroup textsGroup = modelLocalizationNode["Texts"];
					if(textsGroup != null) {
						IModelLocalizationItem captionForTrueLocalizationItem = textsGroup["CaptionForTrue"] as IModelLocalizationItem;
						if(captionForTrueLocalizationItem != null) {
							return captionForTrueLocalizationItem.GetValue<string>("Value");
						}
					}
				}
			}
			return null;
		}
		public static string Get_CaptionForFalse(IModelMember modelMember) {
			IModelApplication modelApplication = modelMember.Application;
			if(modelApplication != null) {
				IModelLocalization modelLocalizationNode = modelApplication.Localization;
				if(modelLocalizationNode != null) {
					IModelLocalizationGroup textsGroup = modelLocalizationNode["Texts"];
					if(textsGroup != null) {
						IModelLocalizationItem captionForFalseLocalizationItem = textsGroup["CaptionForFalse"] as IModelLocalizationItem;
						if(captionForFalseLocalizationItem != null) {
							return captionForFalseLocalizationItem.GetValue<string>("Value");
						}
					}
				}
			}
			return null;
		}
		public static Boolean Get_AllowClear(IModelMember modelMember) {
			return true;
		}
		public static Type Get_PropertyEditorType(IModelMember modelMember) {
			Type result = null;
			IMemberInfo memberInfo = modelMember.MemberInfo;
			if(memberInfo != null) {
				if(!CacheEnabled || !modelMemberEditorTypeCache.TryGetValue(memberInfo, out result)) {
					EditorAliasAttribute editorAlias = memberInfo.FindAttribute<EditorAliasAttribute>();
					if(editorAlias == null) {
						foreach(ITypeInfo interfaceInfo in memberInfo.Owner.ImplementedInterfaces) {
							IMemberInfo interfaceMemberInfo = interfaceInfo.FindMember(memberInfo.Name);
							if(interfaceMemberInfo != null) {
								editorAlias = interfaceMemberInfo.FindAttribute<EditorAliasAttribute>();
								if(editorAlias != null) {
									break;
								}
							}
						}
					}
					if(editorAlias == null) {
						editorAlias = memberInfo.MemberTypeInfo.FindAttribute<EditorAliasAttribute>();
					}
					if(editorAlias != null) {
						result = calculator.GetEditorType(modelMember, editorAlias.Alias);
					}
					else {
						result = calculator.GetEditorType(modelMember);
					}
				}
				if(CacheEnabled) {
					modelMemberEditorTypeCache[memberInfo] = result;
				}
			}
			else if(modelMember.IsCustom && (modelMember.Type != null)) {
				result = calculator.GetEditorType(modelMember);
			}
			return result;
		}
		public static IEnumerable<Type> Get_PropertyEditorTypes(IModelMember modelMember) {
			IEnumerable<Type> result = Type.EmptyTypes;
			if((modelMember.MemberInfo != null) || modelMember.IsCustom) {
				result = calculator.GetEditorsType(modelMember);
			}
			return result;
		}
		public static bool Get_AllowNull(IModelMember modelMember) {
			if(modelMember != null) {
				IMemberInfo memberInfoCore = modelMember.MemberInfo;
				if(memberInfoCore != null) {
					return !memberInfoCore.LastMember.MemberTypeInfo.IsValueType ||
						memberInfoCore.LastMember.MemberTypeInfo.IsNullable;
				}
			}
			return false;
		}
	}
}

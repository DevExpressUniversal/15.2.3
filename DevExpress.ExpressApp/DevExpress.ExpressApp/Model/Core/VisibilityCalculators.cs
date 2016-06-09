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
namespace DevExpress.ExpressApp.Model.Core {
	public class VisibilityCalculatorBase {
		protected ITypeInfo GetTypeInfo(IModelNode node, string propertyName) {
			ITypeInfo elementType = null;
			if(node == null) {
				return null;
			}
			if(node is IModelMember) {
				if(((IModelMember)node).MemberInfo == null) {
					return null;
				}
				elementType = ((IModelMember)node).MemberInfo.MemberTypeInfo;
			}
			else if(node is IModelMemberViewItem) {
				IModelMemberViewItem modelMemberViewItem = (IModelMemberViewItem)node;
				if((modelMemberViewItem.ModelMember == null) || (modelMemberViewItem.ModelMember.MemberInfo == null)) {
					return null;
				}
				elementType = ((IModelMemberViewItem)node).ModelMember.MemberInfo.MemberTypeInfo;
			}
			return elementType;
		}
	}
	public class ImagePropertyEditorCalculator : VisibilityCalculatorBase, IModelIsVisible {
		private Type GetPropertyEditorType(IModelNode node) { 
			if(node is IModelMember) {
				return ((IModelMember)node).PropertyEditorType;
			}
			else if(node is IModelMemberViewItem) {
				return ((IModelMemberViewItem)node).PropertyEditorType;
			}
			return null;
		}
		public bool IsVisible(IModelNode node, String propertyName) {
			bool isImagePropertyEditor = false;
			Type propertyEditorType = GetPropertyEditorType(node);
			if(propertyEditorType != null) {
				ITypeInfo propertyTypeInfo = GetTypeInfo(node, propertyName);	  
				EditorDescriptors editorDescriptors = ((IModelSources)node.Application).EditorDescriptors;
				if(editorDescriptors != null) {
					foreach(IEditorTypeRegistration typeRegistration in editorDescriptors.PropertyEditorRegistrations.TypeRegistrations) {
						if((typeRegistration.EditorType == propertyEditorType) && (typeRegistration.Alias == EditorAliases.ImagePropertyEditor)) {
							isImagePropertyEditor = true;
							break;
						}
					}
				}
			}
			return isImagePropertyEditor;
		}
	}
	public class TypePropertyOnlyCalculator : VisibilityCalculatorBase, IModelIsVisible {
		private Type checkType;
		public TypePropertyOnlyCalculator(Type type) {
			checkType = type;
		}
		public bool IsVisible(IModelNode node, String propertyName) {
			ITypeInfo elementType = GetTypeInfo(node, propertyName);
			if(elementType != null) {
				return elementType.UnderlyingTypeInfo.Type == checkType;
			}
			return false;
		}
	}
	public class BooleanPropertyOnlyCalculator : TypePropertyOnlyCalculator {
		public BooleanPropertyOnlyCalculator() : base(typeof(bool))  {}
	}
	public class StringPropertyOnlyCalculator : TypePropertyOnlyCalculator {
		public StringPropertyOnlyCalculator() : base(typeof(string)) { }
	}
	public class ImagePropertyOnlyCalculator : TypePropertyOnlyCalculator {
		public ImagePropertyOnlyCalculator() : base(typeof(System.Drawing.Image)) { }
	}
	public class DateTimePropertyOnlyCalculator : TypePropertyOnlyCalculator {
		public DateTimePropertyOnlyCalculator() : base(typeof(DateTime)) { }
	}
	public class ModelReadOnlyCalculator : IModelIsReadOnly {
		#region IModelIsReadOnly Members
		public bool IsReadOnly(IModelNode node, string propertyName) {
			return true;
		}
		public bool IsReadOnly(IModelNode node, IModelNode childNode) {
			return true;
		}
		#endregion
	}
	public class DesignerOnlyCalculator : IModelIsVisible, IModelIsReadOnly {
		public static Boolean IsRunFromDesigner = false;
		public static Boolean IsRunTime {
			get { return !IsRunFromDesigner; }
		}
		public bool IsVisible(IModelNode node, String propertyName) {
			return IsRunFromDesigner;
		}
		public bool IsReadOnly(IModelNode node, String propertyName) {
			return !IsRunFromDesigner;
		}
		public bool IsReadOnly(IModelNode node, IModelNode childNode) {
			return !IsRunFromDesigner;
		}
	}
	public class CollectionPropertyOnlyCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			IModelMember member = null;
			if (node is IModelMember) member = ((IModelMember)node);
			else if (node is IModelMemberViewItem) {
				member = ((IModelMemberViewItem)node).ModelMember;
			}
			if((member != null) && (member.MemberInfo != null) && member.MemberInfo.IsList && (member.MemberInfo.ListElementType != null)) {
				return node.Application.BOModel.GetClass(member.MemberInfo.ListElementType) != null;
			}
			return false;
		}
	}
	public class LookupViewItemOnlyCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			if((node is IModelCommonMemberViewItem) && (node.Application is IModelSources)) {
				Type propertyEditorType = ((IModelCommonMemberViewItem)node).PropertyEditorType;
				EditorDescriptors editorDescriptors = ((IModelSources)node.Application).EditorDescriptors;
				if(editorDescriptors != null) {
					foreach(IEditorTypeRegistration typeRegistration in editorDescriptors.PropertyEditorRegistrations.TypeRegistrations) {
						if(typeRegistration.EditorType == propertyEditorType && typeRegistration.Alias == EditorAliases.LookupPropertyEditor) {
							return true;
						}
					}
				}
			}
			return false;
		}
	}
	public class EditMaskTypeVisibilityCalculator : VisibilityCalculatorBase, IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			ITypeInfo elementType = GetTypeInfo(node, propertyName);
			if(elementType != null) {
				return FormattingProvider.GetEditMaskType(elementType.Type) != EditMaskType.Default;
			}
			return false;
		}
	}
	public class EnumTypeNameLocalizationOnlyCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			return node.Parent != null && ((ModelNode)(node.Parent)).Id == "Enums";
		}
	}
	public class NotNewNodeVisibleCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, String propertyName) {
			return !((ModelNode)node).IsNewValueModified;
		}
	}
	public sealed class InterfaceTypePropertyOnlyCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, string propertyName) {
			return ((IModelClass)node).TypeInfo.IsInterface;
		}
	}
	public class ModelClassMembersReadOnlyCalculator : IModelIsReadOnly {
		public Boolean IsReadOnly(IModelNode node, String propertyName) {
			return false;
		}
		public Boolean IsReadOnly(IModelNode node, IModelNode childNode) {
			Boolean result = false;
			if(childNode is IModelMember) {
				IModelMember memberModel = (IModelMember)childNode;
				if(memberModel.IsCustom) {
					result = !memberModel.IsCalculated && DesignerOnlyCalculator.IsRunTime && !ModelMemberReadOnlyCalculator.AllowPersistentCustomProperties;
				}
				else {
					result = true;
				}
			}
			else {
				IModelClass modelClass = (IModelClass)((IModelBOModelClassMembers)node).Parent;
				result = modelClass.TypeInfo.IsInterface && DesignerOnlyCalculator.IsRunFromDesigner;
			}
			return result;
		}
	}
	public class ModelMemberVisibilityCalculator : IModelIsVisible {
		public Boolean IsVisible(IModelNode node, String propertyName) {
			Boolean result = false;
			if(node is IModelMember) {
				IModelMember memberModel = (IModelMember)node;
				if(propertyName.ToLower() == ("IsCustom").ToLower()) {
					result = memberModel.IsCustom;
				}
				else if(propertyName.ToLower() == ("IsCalculated").ToLower()) {
					result = memberModel.IsCustom;
				}
				else if(propertyName.ToLower() == ("Expression").ToLower()) {
					result = memberModel.IsCustom && memberModel.IsCalculated;
				}
			}
			return result;
		}
	}
	public class ModelMemberReadOnlyCalculator : IModelIsReadOnly {
		public Boolean IsReadOnly(IModelNode node, String propertyName) {
			Boolean result = false;
			if(node is IModelMember) {
				IModelMember memberModel = (IModelMember)node;
				if(propertyName.ToLower() == ("Type").ToLower()) {
					result = !memberModel.IsCustom || (DesignerOnlyCalculator.IsRunTime && !memberModel.IsCalculated && !AllowPersistentCustomProperties);
				}
				else if(propertyName.ToLower() == ("IsCalculated").ToLower()) {
					result = !memberModel.IsCustom || (DesignerOnlyCalculator.IsRunTime && !AllowPersistentCustomProperties);
				}
				else if(propertyName.ToLower() == ("Expression").ToLower()) {
					result = !memberModel.IsCustom || !memberModel.IsCalculated;
				}
			}
			return result;
		}
		public Boolean IsReadOnly(IModelNode node, IModelNode childNode) {
			return false;
		}
		public static Boolean AllowPersistentCustomProperties { get; set; }
	}
	public class ModelMemberRequiredCalculator : IModelIsRequired {
		public Boolean IsRequired(IModelNode node, String propertyName) {
			Boolean result = false;
			if(node is IModelMember) {
				IModelMember memberModel = (IModelMember)node;
				if(propertyName.ToLower() == ("PropertyEditorType").ToLower()) {
					if(!memberModel.IsCustom || DesignerOnlyCalculator.IsRunTime) {
						result = true;
					}
				}
				else if(propertyName.ToLower() == ("Expression").ToLower()) {
					if(memberModel.IsCustom && memberModel.IsCalculated) {
						result = true;
					}
				}
			}
			else if(node is IModelMemberViewItem) {
				result = (propertyName.ToLower() == ("PropertyEditorType").ToLower());
			}
			return result;
		}
	}
}

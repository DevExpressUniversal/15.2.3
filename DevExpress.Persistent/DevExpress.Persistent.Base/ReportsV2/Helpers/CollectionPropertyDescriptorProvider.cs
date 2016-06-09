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
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.Persistent.Base.ReportsV2 {
	public class CollectionPropertyDescriptorProvider : ArrayList, ITypedList {
		private PropertyDescriptorProvider propertyDescriptorProvider;
		public CollectionPropertyDescriptorProvider(string targetType, string listName, IServiceProvider serviceProvider, bool showCollectionProperty) {
			propertyDescriptorProvider = CreatePropertyDescriptorProvider(targetType, listName, serviceProvider, showCollectionProperty);
		}
		public void SetObjectType(string targetTypeName) {
			propertyDescriptorProvider.SetObjectType(targetTypeName);
		}
		protected virtual PropertyDescriptorProvider CreatePropertyDescriptorProvider(string targetType, string listName, IServiceProvider serviceProvider, bool showCollectionProperty) {
			return new PropertyDescriptorProvider(targetType, listName, serviceProvider, showCollectionProperty);
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			return propertyDescriptorProvider.GetItemProperties(listAccessors);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return propertyDescriptorProvider.GetListName(listAccessors);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class PropertyDescriptorProvider : ITypedList {
		private ITypeInfo typeInfo;
		private XafPropertyDescriptorCollection propertyDescriptorCollection;
		private IServiceProvider serviceProvider;
		private ITypesInfo typesInfo;
		private bool showCollection = true;
		private string listName;
		private string targetTypeName;
		public PropertyDescriptorProvider(string targetType, string listName, IServiceProvider serviceProvider, bool showCollectionProperty) {
			this.showCollection = showCollectionProperty;
			this.listName = listName;
			this.targetTypeName = targetType;
			this.serviceProvider = serviceProvider;
			this.typesInfo = serviceProvider.GetService(typeof(ITypesInfo)) as ITypesInfo;
			SetObjectType(targetType);
		}
		public void SetObjectType(string targetTypeName) {
			if(typesInfo != null) {
				typeInfo = string.IsNullOrEmpty(targetTypeName) ? null : typesInfo.FindTypeInfo(targetTypeName);
				RefreshPropertyDescriptorCollection();
			}
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(TypeInfoIsValid()) {
				if(listAccessors == null) {
					if(propertyDescriptorCollection != null) {
						return propertyDescriptorCollection;
					}
				}
				else {
					if(listAccessors.Length > 0) {
						if(listAccessors[listAccessors.Length - 1] != null) {
							Type propertyType = listAccessors[listAccessors.Length - 1].PropertyType;
							ITypeInfo propertyTypeInfo = typesInfo.FindTypeInfo(propertyType);
							if(propertyTypeInfo != null) {
								if(propertyTypeInfo.IsDomainComponent) {
									return CreatePropertyDescriptorCollection(propertyTypeInfo, false);
								}
								else if(propertyTypeInfo.IsListType) {
									Type ownerType = listAccessors[listAccessors.Length - 1].ComponentType;
									ITypeInfo ownerTypeInfo = typesInfo.FindTypeInfo(ownerType);
									IMemberInfo memberInfo = ownerTypeInfo.FindMember(listAccessors[listAccessors.Length - 1].Name);
									if(memberInfo.ListElementType != null) {
										ITypeInfo listElementTypeInfo = typesInfo.FindTypeInfo(memberInfo.ListElementType);
										if(listElementTypeInfo != null && listElementTypeInfo.IsDomainComponent) {
											return CreatePropertyDescriptorCollection(listElementTypeInfo, false);
										}
									}
								}
							}
						}
					}
				}
			}
			return new PropertyDescriptorCollection(null);
		}
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return listName;
		}
		protected virtual XafPropertyDescriptorCollection CreateXafPropertyDescriptorCollection(ITypeInfo pdcTypeInfo) {
			return new XafPropertyDescriptorCollection(pdcTypeInfo);
		}
		private void RefreshPropertyDescriptorCollection() {
			propertyDescriptorCollection = CreatePropertyDescriptorCollection();
		}
		private bool TypeInfoIsValid() {
			if(typesInfo == null) {
				typesInfo = serviceProvider.GetService(typeof(ITypesInfo)) as ITypesInfo;
			}
			if(typeInfo == null) {
				SetObjectType(targetTypeName);
			}
			return typeInfo != null;
		}
		private XafPropertyDescriptorCollection CreatePropertyDescriptorCollection(ITypeInfo pdcTypeInfo, Boolean includeInvisibleMembers) {
			if(pdcTypeInfo == null) return null;
			XafPropertyDescriptorCollection xafPropertyDescriptorCollection = CreateXafPropertyDescriptorCollection(pdcTypeInfo);
			List<IMemberInfo> list = new List<IMemberInfo>(pdcTypeInfo.Members);
			list.Sort(delegate(IMemberInfo left, IMemberInfo right) {
				return Comparer<string>.Default.Compare(left.Name, right.Name);
			});
			foreach(IMemberInfo memberInfo in list) {
				if(IsAccessibleMember(memberInfo, pdcTypeInfo, includeInvisibleMembers)) {
					xafPropertyDescriptorCollection.CreatePropertyDescriptor(memberInfo, memberInfo.Name);
				}
			}
			return xafPropertyDescriptorCollection;
		}
		private XafPropertyDescriptorCollection CreatePropertyDescriptorCollection() {
			if(typeInfo == null) return null;
			return CreatePropertyDescriptorCollection(typeInfo, typeInfo.IsInterface);
		}
		private Boolean IsAccessibleMember(IMemberInfo memberInfo, ITypeInfo owner, Boolean includeInvisibleMember) {
			bool showCollectionModificator = showCollection ? true : !memberInfo.IsList;
			return showCollectionModificator && (includeInvisibleMember || memberInfo.IsVisible || owner.KeyMembers.Contains(memberInfo));
		}
	}
}

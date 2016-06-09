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
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp {
	public class XafPropertyDescriptor : PropertyDescriptor {
		private IMemberInfo memberInfo;
		private String displayName;
		public XafPropertyDescriptor(IMemberInfo memberInfo, String name)
			: base(name, null) {
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
			this.memberInfo = memberInfo;
		}
		public override Object GetValue(Object obj) {
			return memberInfo.GetValue(obj);
		}
		public override void SetValue(Object obj, Object value) {
			memberInfo.SetValue(obj, value);
		}
		public override Boolean CanResetValue(Object obj) {
			return false;
		}
		public override void ResetValue(Object obj) {
		}
		public override Boolean ShouldSerializeValue(Object obj) {
			return false;
		}
		public override Type ComponentType {
			get { return memberInfo.Owner.Type; }
		}
		public override Boolean IsReadOnly {
			get { return memberInfo.IsReadOnly; }
		}
		public override Boolean IsBrowsable {
			get { return memberInfo.IsVisible; }
		}
		public override Type PropertyType {
			get { return memberInfo.MemberType; }
		}
		public override String DisplayName {
			get {
				if(displayName == null) {
					if(Name.Contains("!")) {
						displayName = base.DisplayName;
					}
					else {
						ITypeInfo ownerInfo = memberInfo.Owner;
						displayName = CaptionHelper.GetFullMemberCaption(ownerInfo, Name);
					}
				}
				return displayName; 
			}
		}
		public IMemberInfo MemberInfo {
			get { return memberInfo; }
		}
	}
	public class XafPropertyDescriptorCollection : PropertyDescriptorCollection {
		private ITypeInfo typeInfo;
		private String displayableMembers = "";
		public XafPropertyDescriptorCollection(ITypeInfo typeInfo)
			: base(new PropertyDescriptor[] { }) {
			this.typeInfo = typeInfo;
		}
		public override PropertyDescriptor Find(String name, Boolean ignoreCase) {
			PropertyDescriptor result = base.Find(name, ignoreCase);
			if(result == null) {
				IMemberInfo memberInfo = typeInfo.FindMember(name);
				if(memberInfo != null) {
					result = CreatePropertyDescriptor(memberInfo, name);
				}
			}
			return result;
		}
		public PropertyDescriptor CreatePropertyDescriptor(IMemberInfo memberInfo, String name) {
			PropertyDescriptor propertyDescriptor = CreatePropertyDescriptorCore(memberInfo, name);
			Add(propertyDescriptor);
			displayableMembers = displayableMembers + name + ";";
			return propertyDescriptor;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual XafPropertyDescriptor CreatePropertyDescriptorCore(IMemberInfo memberInfo, String name) {
			return new XafPropertyDescriptor(memberInfo, name);
		}
		public String DisplayableMembers {
			get { return displayableMembers; }
			set {
				displayableMembers = "";
				Clear();
				if(!String.IsNullOrEmpty(value)) {
					IList<String> displayableMembersList = value.Split(';');
					foreach(String displayableMember in displayableMembersList) {
						Find(displayableMember, false);
					}
				}
			}
		}
		public ITypeInfo TypeInfo {
			get { return typeInfo; }
		}
	}
	public class XafCustomTypeDescriptor : CustomTypeDescriptor {
		private XafPropertyDescriptorCollection propertyDescriptorCollection;
		public XafCustomTypeDescriptor(ITypeInfo typeInfo) {
			propertyDescriptorCollection = new XafPropertyDescriptorCollection(typeInfo);
		}
		public override PropertyDescriptorCollection GetProperties() {
			return propertyDescriptorCollection;
		}
	}
	public class XafTypeDescriptionProvider : TypeDescriptionProvider {
		private ITypesInfo typesInfo;
		private Dictionary<Type, ICustomTypeDescriptor> customTypeDescriptors;
		public XafTypeDescriptionProvider(ITypesInfo typesInfo) {
			this.typesInfo = typesInfo;
			customTypeDescriptors = new Dictionary<Type, ICustomTypeDescriptor>();
		}
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, Object instance) {
			ICustomTypeDescriptor result = null;
			if(!customTypeDescriptors.TryGetValue(objectType, out result)) {
				result = new XafCustomTypeDescriptor(typesInfo.FindTypeInfo(objectType));
				customTypeDescriptors.Add(objectType, result);
			}
			return result;
		}
	}
}

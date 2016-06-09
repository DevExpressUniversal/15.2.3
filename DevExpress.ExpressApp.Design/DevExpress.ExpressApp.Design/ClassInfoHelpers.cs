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
using System.Text;
using System.ComponentModel;
using DevExpress.Xpo.Metadata;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.ComponentModel.Design;
using System.Reflection;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Design {
	public class XPMemberProperiesProvider : Component {
		private IMemberInfo memberInfo;
		private string name;
		private Type type;
		private string displayName;
		private Type ownerClassType;
		private ITypeInfo ownerClassInfo;
		private XPMemberProperiesProvider() { }
		public XPMemberProperiesProvider(IMemberInfo memberInfo) {
			this.memberInfo = memberInfo;
			this.type = memberInfo.MemberType;
			this.name = memberInfo.Name;
			this.displayName = memberInfo.DisplayName;
			this.ownerClassType = memberInfo.Owner.Type;
			this.ownerClassInfo = memberInfo.Owner;
		}
		public string Name {
			get {
				return name;
			}
		}
		public string DisplayName {
			get {
				return displayName;
			}
		}
		public Type Type {
			get {
				return type;
			}
		}
		public Type OwnerClassType {
			get {
				return ownerClassType;
			}
		}
		[Browsable(false)]
		public ITypeInfo OwnerClassInfo {
			get {
				return ownerClassInfo;
			}
		}
	}
	public class XPClassProperiesProvider : Component {
		private Boolean canBeUsed;
		private ITypeInfo typeInfo;
		private XPClassProperiesProvider() { }
		public XPClassProperiesProvider(ITypeInfo typeInfo) : this(typeInfo, false) { }
		public XPClassProperiesProvider(ITypeInfo typeInfo, Boolean canBeUsed) {
			this.typeInfo = typeInfo;
			this.canBeUsed = canBeUsed;
		}
		[Browsable(false)]
		public bool CanBeUsed {
			get { return canBeUsed; }
		}
		[Browsable(false)]
		public ITypeInfo ClassInfo {
			get { return typeInfo; }
		}
		public string FullName {
			get { return typeInfo.FullName; }
		}
		public string ShortName {
			get { return typeInfo.Name; }
		}
		public string ClassCaption {
			get { return XPClassInfoHelper.GetClassCaption(typeInfo.Type); }
		}
		public string BaseClassName {
			get { 
				return (typeInfo.Base != null) ? typeInfo.Base.FullName : "";
			}
		}
		public string DefaultPropertyName {
			get {
				return (typeInfo.DefaultMember != null) ? typeInfo.DefaultMember.Name : "";
			}
		}
		public string FriendlyKey {
			get { return XPClassInfoHelper.FindFriendlyKeyPropertyName(typeInfo); }
		}
		public string ObjectCaptionFormat {
			get { return XPClassInfoHelper.GetObjectCaptionFormat(typeInfo); }
		}
	}
	public class AssemblyPropertiesProvider : Component {
		private bool canBeUsed = false;
		private Assembly assembly;
		private AssemblyName assemblyName; 
		public AssemblyPropertiesProvider() { }
		public AssemblyPropertiesProvider(Assembly assembly) {
			canBeUsed = false;
			this.assembly = assembly;
			this.assemblyName = this.assembly.GetName();
		}
		public AssemblyPropertiesProvider(Assembly assembly, bool canBeUsed) : this(assembly) {
			this.canBeUsed = canBeUsed;
		}
		public string Name {
			get {
				return assemblyName.Name;
			}
		}
		public string Version {
			get {
				return assemblyName.Version.ToString();
			}
		}
		[Browsable(false)]
		public Assembly Assembly {
			get {
				return assembly;
			}
		}
		[Browsable(false)]
		public bool CanBeUsed {
			get {
				return canBeUsed;
			}
		}
	}
	public static class XPClassInfoHelper {
		public static string GetClassCaption(Type objectType) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
			System.ComponentModel.DisplayNameAttribute displayNameAttr =
				typeInfo.FindAttribute<System.ComponentModel.DisplayNameAttribute>();
			if(displayNameAttr != null) {
				return displayNameAttr.DisplayName;
			}
			return objectType.Name;
		}
		public static bool IsMemberVisible(IMemberInfo memberInfo) {
			if(memberInfo == null)
				return false;
			if(!memberInfo.IsPublic
				|| !memberInfo.IsVisible
				|| memberInfo.MemberType.ContainsGenericParameters) {
				return false;
			}
			BrowsableAttribute browsableAttribute = memberInfo.FindAttribute<BrowsableAttribute>();
			if((browsableAttribute != null) && !browsableAttribute.Browsable)
				return false;
			if(memberInfo.Owner.Type.Assembly == typeof(XPBaseObject).Assembly
				&& (memberInfo.Owner.Type != typeof(XPObject) || memberInfo.Name != "Oid")) {
				return false;
			}
			return true;
		}
		public static string FindFriendlyKeyPropertyName(ITypeInfo classInfo) {
			foreach(FriendlyKeyPropertyAttribute attr in classInfo.Type.GetCustomAttributes(typeof(FriendlyKeyPropertyAttribute), true)) {
				if(classInfo.FindMember(attr.MemberName) != null) {
					return attr.MemberName;
				}
			}
			return null;
		}
		public static string GetObjectCaptionFormat(ITypeInfo typeInfo) {
			ObjectCaptionFormatAttribute detailViewCaptionFormatAttribute = typeInfo.FindAttribute<ObjectCaptionFormatAttribute>();
			if(detailViewCaptionFormatAttribute != null && !String.IsNullOrEmpty(detailViewCaptionFormatAttribute.FormatString)) {
				return detailViewCaptionFormatAttribute.FormatString;
			}
			return null;
		}
	}
}

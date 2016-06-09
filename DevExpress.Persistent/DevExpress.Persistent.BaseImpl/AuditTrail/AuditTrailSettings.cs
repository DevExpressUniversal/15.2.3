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
using System.Collections.ObjectModel;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using System.Collections;
namespace DevExpress.Persistent.AuditTrail {
	public class AuditTrailSettings {
		List<XPDictionary> xpDictionaries;
		public AuditTrailSettings() {
			typesToAudit = new Dictionary<Type, AuditTrailClassInfo>();
			xpDictionaries = new List<XPDictionary>();
		}
		public AuditTrailSettings(XPDictionary xpDictionary)
			: this() {
			SetXPDictionary(xpDictionary);
		}
		private Dictionary<Type, AuditTrailClassInfo> typesToAudit;
		private bool IsValidTypeToAudit(Type type, out string errorMessage) {
			errorMessage = null;
			if(type != null) {
				if(!typeof(IXPSimpleObject).IsAssignableFrom(type) &&
					!((DevExpress.ExpressApp.DC.TypesInfo)DevExpress.ExpressApp.XafTypesInfo.Instance).IsRegisteredEntity(type)) {
					errorMessage = string.Format("The audited type: '{0}' must implement IXPSimpleObject", type.FullName);
				}
				XPClassInfo info = GetClassInfo(type);
				if(info != null && !info.IsPersistent) {
					errorMessage = string.Format("To be audited, the '{0}' type must be persistent", type.FullName);
				}
			}
			else {
				errorMessage = (new ArgumentNullException("type")).Message;
			}
			return errorMessage == null;
		}
		private XPClassInfo GetClassInfo(Type type) {
			foreach(XPClassInfo classInfo in GetClasses()) {
				if(classInfo.ClassType == type) {
					return classInfo;
				}
			}
			return null;
		}
		private ICollection GetClasses() {
			ArrayList result = new ArrayList();
			foreach(XPDictionary dictionary in xpDictionaries) {
				result.AddRange(dictionary.Classes);
			}
			return result;
		}
		private void CheckTypeToAudit(Type type) {
			string errorMessage;
			if(!IsValidTypeToAudit(type, out errorMessage)) {
				throw new InvalidOperationException(errorMessage);
			}
		}
		public ReadOnlyCollection<AuditTrailClassInfo> TypesToAudit {
			get {
				List<AuditTrailClassInfo> result = new List<AuditTrailClassInfo>(typesToAudit.Values);
				return result.AsReadOnly();
			}
		}
		public void SetXPDictionary(XPDictionary xpDictionary) {
			List<XPDictionary> dictionaries = new List<XPDictionary>() { xpDictionary };
			SetXPDictionaries(dictionaries);
		}
		public void SetXPDictionaries(IList<XPDictionary> dictionaries) {
			foreach(XPDictionary xpDictionary in dictionaries) {
				if(!xpDictionaries.Contains(xpDictionary)) {
					xpDictionaries.Add(xpDictionary);
				}
			}
			Clear();
		}
		public void AddAllTypes() {
			string errorMessage;
			foreach(XPClassInfo info in GetClasses()) {
				if(IsValidTypeToAudit(info.ClassType, out errorMessage)) {
					AddType(info.ClassType);
				}
			}
		}
		public void AddType(Type type, params string[] memberNames) {
			AddType(type, false, memberNames);
		}
		public void AddType(Type type, bool includeRelatedTypes) {
			CheckTypeToAudit(type);
			AddType(type, includeRelatedTypes, GetAllPublicProperties(type));
		}
		public void AddType(Type type) {
			AddType(type, false);
		}
		public void AddType(Type type, bool includeRelatedTypes, params string[] memberNames) {
			CheckTypeToAudit(type);
			if(!typesToAudit.ContainsKey(type)) {
				List<string> propertiesCollection = new List<string>();
				foreach(string memberName in memberNames) {
					propertiesCollection.Add(memberName);
				}
				XPClassInfo classInfo = GetClassInfo(type);
				typesToAudit.Add(type, new AuditTrailClassInfo(classInfo, propertiesCollection));
				if(includeRelatedTypes) {
					foreach(Type relatedType in GetRelatedTypes(type)) {
						AddType(relatedType, true);
					}
				}
			}
		}
		public void RemoveType(Type type) {
			typesToAudit.Remove(type);
		}
		public void RemoveProperties(Type type, params string[] memberNames) {
			AuditTrailClassInfo typeInfo = FindAuditTrailClassInfo(type);
			if(typeInfo != null) {
				foreach(string property in memberNames) {
					for(int i = 0; i < typeInfo.Properties.Count; i++)
						if(typeInfo.Properties[i].Name == property) {
							typeInfo.properties.Remove(typeInfo.Properties[i]);
						}
				}
			}
		}
		public void AddProperties(Type type, params string[] memberNames) {
			AuditTrailClassInfo typeInfo = FindAuditTrailClassInfo(type);
			if(typeInfo != null) {
				AddType(type, null);
			}
			typeInfo = FindAuditTrailClassInfo(type);
			bool isFound = false;
			foreach(string property in memberNames) {
				for(int i = 0; i < typeInfo.Properties.Count; i++) {
					if(typeInfo.Properties[i].Name == property) {
						isFound = true;
						break;
					}
				}
				if(!isFound) {
					typeInfo.properties.Add(new AuditTrailMemberInfo(property, typeInfo));
				}
			}
		}
		public void Clear() {
			typesToAudit.Clear();
		}
		public bool IsTypeToAudit(Type type) {
			AuditTrailClassInfo result;
			typesToAudit.TryGetValue(type, out result);
			return result != null;
		}
		public AuditTrailClassInfo FindAuditTrailClassInfo(Type type) {
			AuditTrailClassInfo result;
			typesToAudit.TryGetValue(type, out result);
			return result;
		}
		private string[] GetAllPublicProperties(Type type) {
			List<string> result = new List<string>();
			XPClassInfo currentClassInfo = GetClassInfo(type);
			while(currentClassInfo != null && currentClassInfo.ClassType.Assembly != typeof(XPObject).Assembly) {
				foreach(XPMemberInfo memberInfo in currentClassInfo.OwnMembers) {
					if(memberInfo.IsPublic && (IsCollection(memberInfo) || !memberInfo.IsReadOnly)) {
						Type memberType = memberInfo.MemberType;
						XPClassInfo memberClassInfo = GetClassInfo(memberType);
						if(memberClassInfo != null && !memberClassInfo.IsPersistent) {
							continue;
						}
						result.Add(memberInfo.Name);
					}
				}
				currentClassInfo = currentClassInfo.BaseClass;
			}
			return result.ToArray();
		}
		private bool IsCollection(XPMemberInfo memberInfo) {
			return memberInfo.IsCollection ||  memberInfo.IsAssociationList;
		}
		private Type[] GetRelatedTypes(Type type) {
			List<Type> result = new List<Type>();
			XPClassInfo currentClassInfo = GetClassInfo(type);
			string errorMessage;
			while(currentClassInfo != null && currentClassInfo.ClassType.Assembly != typeof(XPObject).Assembly) {
				foreach(XPMemberInfo memberInfo in currentClassInfo.OwnMembers) {
					if((IsCollection(memberInfo) || !memberInfo.IsReadOnly)) {
						Type targetType = null;
						if(IsCollection(memberInfo)) {
							targetType = memberInfo.CollectionElementType.ClassType;
						}
						else if(memberInfo.ReferenceType != null) {
							targetType = memberInfo.ReferenceType.ClassType;
						}
						if(targetType != null && !result.Contains(targetType) && IsValidTypeToAudit(targetType, out errorMessage)) {
							result.Add(targetType);
						}
					}
				}
				currentClassInfo = currentClassInfo.BaseClass;
			}
			return result.ToArray();
		}
	}
	public class AuditTrailClassInfo {
		private XPClassInfo classInfo;
		public XPClassInfo ClassInfo {
			get { return classInfo; }
			set { classInfo = value; }
		}
		internal List<AuditTrailMemberInfo> properties;
		public ReadOnlyCollection<AuditTrailMemberInfo> Properties {
			get { return properties.AsReadOnly(); }
		}
		public AuditTrailClassInfo(XPClassInfo classInfo, ICollection<string> properties) {
			this.classInfo = classInfo;
			this.properties = new List<AuditTrailMemberInfo>();
			Initialize(properties);
		}
		private void Initialize(ICollection<string> properties) {
			foreach(string memberName in properties) {
				this.properties.Add(new AuditTrailMemberInfo(memberName, this));
			}
		}
		public AuditTrailMemberInfo FindAuditMemberInfo(string memberName) {
			foreach(AuditTrailMemberInfo auditMemberInfo in properties) {
				if(auditMemberInfo.Name == memberName) {
					return auditMemberInfo;
				}
			}
			return null;
		}
		public override string ToString() {
			return base.ToString() + "(" + classInfo.ClassType.Name + ")";
		}
	}
	public class AuditTrailMemberInfo {
		public AuditTrailMemberInfo(string memberName, AuditTrailClassInfo parent) {
			this.name = memberName;
			this.parent = parent;
		}
		private string name;
		public string Name {
			get { return name; }
			set { name = value; }
		}
		XPMemberInfo memberInfo;
		public XPMemberInfo MemberInfo {
			get {
				if(memberInfo == null) {
					memberInfo = parent.ClassInfo.FindMember(Name);
				}
				return memberInfo;
			}
		}
		private AuditTrailClassInfo parent;
		public override string ToString() {
			return base.ToString() + "(" + Name + ")";
		}
	}
}

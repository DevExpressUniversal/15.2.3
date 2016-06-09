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
using System.Data;
using System.Reflection;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
namespace DevExpress.Persistent.Base {
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = true)]
	public class ObjectValidatorIgnoreIssueAttribute : Attribute {
		private Type[] issueTypes;
		private bool ignoreAll = false;
		public ObjectValidatorIgnoreIssueAttribute(params Type[] issueTypes) {
			this.issueTypes = issueTypes;
		}
		public ObjectValidatorIgnoreIssueAttribute(bool ignoreAll) {
			this.ignoreAll = ignoreAll;
		}
		public bool Contains(Type issueType) {
			if(IgnoreAll) {
				return true;
			}
			foreach(Type type in IssueTypes) {
				if(issueType == type) {
					return true;
				}
			}
			return false;
		}
		public Type[] IssueTypes {
			get { return issueTypes; }
		}
		public bool IgnoreAll {
			get { return ignoreAll; }
		}
	}
	public enum ObjectValidatorIssueStatus { None, Skipped, Ignored, Passed, Failed }
	public abstract class ObjectValidatorIssue {
		public static List<Type> IgnoreIssues = new List<Type>();
		private XPClassInfo classInfo;
		private string text;
		private ObjectValidatorIssueStatus status = ObjectValidatorIssueStatus.None;
		public ObjectValidatorIssue(XPClassInfo classInfo) {
			this.classInfo = classInfo;
		}
		public string Text {
			get { return text; }
		}
		protected abstract bool ValidateCore(out string text);
		protected bool IsIssueIgnored(XPTypeInfo typeInfo, Type issueType) {
			ObjectValidatorIgnoreIssueAttribute ignoreIssueAttr = (ObjectValidatorIgnoreIssueAttribute)
				typeInfo.FindAttributeInfo(typeof(ObjectValidatorIgnoreIssueAttribute));
			if((ignoreIssueAttr != null) && (ignoreIssueAttr.Contains(issueType))) {
				return true;
			}
			return false;
		}
		protected bool IsCustomAttributeApplied(XPTypeInfo typeInfo, string customAttributeName) {
			foreach(Attribute attribute in typeInfo.Attributes) {
				CustomAttribute customAttribute = attribute as CustomAttribute;
				if(customAttribute != null && customAttribute.Name == customAttributeName) {
					return true;
				}
			}
			return false;
		}
		protected bool IsModelDefaultAttributeApplied(XPTypeInfo typeInfo, string modelDefaultPropertyName) {
			foreach(Attribute attribute in typeInfo.Attributes) {
				ModelDefaultAttribute modelDefaultAttribute = attribute as ModelDefaultAttribute;
				if(modelDefaultAttribute != null && modelDefaultAttribute.PropertyName == modelDefaultPropertyName) {
					return true;
				}
			}
			return false;
		}
		protected virtual bool GetIsIgnoredCore(out string reason) {
			reason = "";
			if(IsIssueIgnored(ClassInfo, GetType())) {
				reason += "The '" + GetType().Name + "' issues is ignored\r\n";
				return true;
			}
			return false;
		}
		protected bool GetIsIgnored(out string reason) {
			reason = "";
			if(IgnoreIssues.Contains(GetType())) {
				reason += "The '" + GetType().Name + " is globally ignored\r\n";
				return true;
			}
			return GetIsIgnoredCore(out reason);
		}
		protected virtual bool GetIsSkipped(out string reason) {
			reason = "";
			if(ClassInfo.ClassType == null) {
				reason += "ClassInfo.ClassType is null";
				return true;
			}
			return false;
		}
		public void Validate() {
			text = "";
			if(GetIsIgnored(out text)) {
				Status = ObjectValidatorIssueStatus.Ignored;
			}
			else {
				if(GetIsSkipped(out text)) {
					Status = ObjectValidatorIssueStatus.Skipped;
				}
				else {
					if(ValidateCore(out text)) {
						Status = ObjectValidatorIssueStatus.Passed;
					}
					else {
						Status = ObjectValidatorIssueStatus.Failed;
					}
				}
			}
			text += ", " + GetKey();
		}
		public ObjectValidatorIssueStatus Status {
			get { return status; }
			set { status = value; }
		}
		public virtual string GetKey() {
			return GetType().Name + ", " + classInfo.ToString();
		}
		public XPClassInfo ClassInfo {
			get { return classInfo; }
		}
		public override string ToString() {
			return GetKey();
		}
		public static T Find<T>(List<ObjectValidatorIssue> issues, Type type) where T : ObjectValidatorIssue {
			foreach(ObjectValidatorIssue issue in issues) {
				T typeIssue = issue as T;
				if(typeIssue != null && typeIssue.ClassInfo.ClassType == type) {
					return typeIssue;
				}
			}
			return null;
		}
	}
	public abstract class ObjectValidatorClassIssue : ObjectValidatorIssue {
		public ObjectValidatorClassIssue(XPClassInfo classInfo) : base(classInfo) { }
	}
	public abstract class ObjectValidatorMemberIssue : ObjectValidatorIssue {
		private XPMemberInfo memberInfo;
		protected override bool GetIsIgnoredCore(out string reason) {
			reason = "";
			if(IsIssueIgnored(MemberInfo, GetType()) || IsIssueIgnored(ClassInfo, GetType())) {
				reason += "The '" + GetType().Name + "' issues is ignored\r\n";
				return true;
			}
			return false;
		}
		public ObjectValidatorMemberIssue(XPClassInfo classInfo, XPMemberInfo memberInfo)
			: base(classInfo) {
			this.memberInfo = memberInfo;
		}
		public override string GetKey() {
			return base.GetKey() + ", " + memberInfo.Name;
		}
		public XPMemberInfo MemberInfo {
			get { return memberInfo; }
		}
		public static T Find<T>(List<ObjectValidatorIssue> issues, Type type, string memberName) where T : ObjectValidatorMemberIssue {
			foreach(ObjectValidatorIssue issue in issues) {
				T memberIssue = issue as T;
				if(memberIssue != null && memberIssue.ClassInfo.ClassType == type && memberIssue.MemberInfo.Name == memberName) {
					return memberIssue;
				}
			}
			return null;
		}
	}
	public class ObjectValidatorSessionConstructorIsAbsentInPersistentType : ObjectValidatorClassIssue {
		protected override bool GetIsSkipped(out string reason) {
			if(base.GetIsSkipped(out reason)) {
				return true;
			}
			if(!ClassInfo.IsPersistent) {
				reason += "class is nonpersistent";
				return true;
			}
			return false;
		}
		public ObjectValidatorSessionConstructorIsAbsentInPersistentType(XPClassInfo classInfo)
			: base(classInfo) { }
		protected override bool ValidateCore(out string text) {
			text = "";
			ConstructorInfo constructorInfo = ClassInfo.ClassType.GetConstructor(
				BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(Session) }, null);
			if(constructorInfo == null) {
				text = "The '" + ClassInfo.FullName + "' persistent class doesn't have a Session specific constructor (a specific 'Session' object should passed instead of use the Session.DefaultSession object)";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorCustomAttributeSpecifiesClassCaptionInCode : ObjectValidatorClassIssue {
		public ObjectValidatorCustomAttributeSpecifiesClassCaptionInCode(XPClassInfo classInfo) : base(classInfo) { }
		protected override bool ValidateCore(out string text) {
			text = "";
			if(IsCustomAttributeApplied(ClassInfo, "Caption")) {
				text = "A caption for the member is specified via the Custom attribute in code. Use the 'System.ComponentModel.DisplayName' attribute instead.";
				return false;
			}
			if(IsModelDefaultAttributeApplied(ClassInfo, "Caption")) {
				text = "A caption for the member is specified via the ModelDefault attribute in code. Use the 'System.ComponentModel.DisplayName' attribute instead.";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorDefaultPropertyIsVirtual : ObjectValidatorMemberIssue {
		System.Reflection.PropertyInfo reflectionMemberInfo;
		protected override bool GetIsSkipped(out string reason) {
			if(base.GetIsSkipped(out reason)) {
				return true;
			}
			if(!ClassInfo.IsPersistent) {
				reason = "class is nonpersistent";
				return true;
			}
			string defaultPropertyName = string.Empty;
			XafDefaultPropertyAttribute xafDefaultPropertyAttribute = FindAttribute<XafDefaultPropertyAttribute>(ClassInfo);
			if(xafDefaultPropertyAttribute != null) {
				defaultPropertyName = xafDefaultPropertyAttribute.Name;
			}
			else {
				DefaultPropertyAttribute defaultPropertyAttribute = FindAttribute<DefaultPropertyAttribute>(ClassInfo);
				if(defaultPropertyAttribute == null) {
					reason = "DefaultPropertyAttribute is not specified for the class";
					return true;
				}
				defaultPropertyName = defaultPropertyAttribute.Name;
			}
			if(defaultPropertyName != MemberInfo.Name) {
				reason = "This property is not default ('" + defaultPropertyName + "')";
				return true;
			}
			reflectionMemberInfo = ClassInfo.ClassType.GetProperty(MemberInfo.Name);
			if(reflectionMemberInfo == null) {
				reason = "cannot get Reflection.PropertyInfo object to check 'virtual'/'abstract' modifier";
				return true;
			}
			return false;
		}
		private AttrType FindAttribute<AttrType>(XPClassInfo ci) where AttrType : Attribute {
			XPClassInfo currentClassInfo = ci;
			do {
				AttrType attr = (AttrType)currentClassInfo.FindAttributeInfo(typeof(AttrType));
				if(attr != null) {
					return attr;
				}
				currentClassInfo = currentClassInfo.BaseClass;
			}
			while(currentClassInfo != null && currentClassInfo.ClassType != typeof(object) && currentClassInfo.ClassType != null);
			return null;
		}
		public ObjectValidatorDefaultPropertyIsVirtual(XPClassInfo classInfo, XPMemberInfo memberInfo) : base(classInfo, memberInfo) { }
		protected override bool ValidateCore(out string text) {
			text = "";
			if(((reflectionMemberInfo.GetGetMethod().Attributes & MethodAttributes.Abstract) == MethodAttributes.Abstract)
				|| (((reflectionMemberInfo.GetGetMethod().Attributes & MethodAttributes.Virtual) == MethodAttributes.Virtual)
					&& (reflectionMemberInfo.GetGetMethod().Attributes & MethodAttributes.Final) != MethodAttributes.Final)) {
				text = "The default property should not be virtual or abstract";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorDefaultPropertyIsNonPersistentNorAliased : ObjectValidatorMemberIssue {
		protected override bool GetIsSkipped(out string reason) {
			if(base.GetIsSkipped(out reason)) {
				return true;
			}
			if(!ClassInfo.IsPersistent) {
				reason = "class is nonpersistent";
				return true;
			}
			string defaultPropertyName = string.Empty;
			XafDefaultPropertyAttribute xafDefaultPropertyAttribute = (XafDefaultPropertyAttribute)ClassInfo.FindAttributeInfo(typeof(XafDefaultPropertyAttribute));
			if(xafDefaultPropertyAttribute != null) {
				defaultPropertyName = xafDefaultPropertyAttribute.Name;
			}
			else {
				DefaultPropertyAttribute defaultPropertyAttribute = (DefaultPropertyAttribute)ClassInfo.FindAttributeInfo(typeof(DefaultPropertyAttribute));
				if(defaultPropertyAttribute == null) {
					reason = "DefaultPropertyAttribute is not specified for the class";
					return true;
				}
				defaultPropertyName = defaultPropertyAttribute.Name;
			}
			if(defaultPropertyName != MemberInfo.Name) {
				reason = "This property is not default ('" + defaultPropertyName + "')";
				return true;
			}
			return false;
		}
		public ObjectValidatorDefaultPropertyIsNonPersistentNorAliased(XPClassInfo classInfo, XPMemberInfo memberInfo) : base(classInfo, memberInfo) { }
		protected override bool ValidateCore(out string text) {
			text = "";
			if(!MemberInfo.IsPersistent && !MemberInfo.IsAliased) {
				text = "The default property should be Persistent or Aliased.";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorCollectionElementTypeIsLost : ObjectValidatorMemberIssue {
		private IMemberInfo dcMemberInfo;
		public ObjectValidatorCollectionElementTypeIsLost(XPClassInfo classInfo, XPMemberInfo memberInfo) : base(classInfo, memberInfo) { }
		protected override bool GetIsSkipped(out string reason) {
			if(base.GetIsSkipped(out reason)) {
				return true;
			}
			if(!MemberInfo.IsPublic) {
				reason += "member is private";
				return true;
			}
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(ClassInfo.ClassType);
			dcMemberInfo = typeInfo.FindMember(MemberInfo.Name);
			if(dcMemberInfo == null) {
				reason += "Cannot find IMemberInfo for this member: " + MemberInfo.Name + "\r\n";
				return true;
			}
			if(!typeof(IList).IsAssignableFrom(MemberInfo.MemberType)) {
				reason += "MemberType is not IList\r\n";
				return true;
			}
			return false;
		}
		protected override bool ValidateCore(out string text) {
			text = "";
			if(dcMemberInfo.ListElementType == null) {
				text = "Cannot determine elements type of a collection property (this colleciton cannot be shown properly in UI)";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorCollectionReturnsNewValueOnEachAccess : ObjectValidatorMemberIssue {
		protected object obj;
		protected UnitOfWork session;
		public ObjectValidatorCollectionReturnsNewValueOnEachAccess(XPClassInfo classInfo, XPMemberInfo memberInfo)
			: base(classInfo, memberInfo) {
			DataSet dataSet = new DataSet();
			IDataLayer dataLayer = new SimpleDataLayer(MemberInfo.Owner.Dictionary, new DataSetDataStore(dataSet, AutoCreateOption.DatabaseAndSchema));
			session = new UnitOfWork(dataLayer);
		}
		protected override bool GetIsSkipped(out string reason) {
			if(base.GetIsSkipped(out reason)) {
				return true;
			}
			if(ClassInfo.ClassType.IsAbstract) {
				reason += "cannot check: class is abstract\r\n";
				return true;
			}
			if(!ClassInfo.IsPersistent) {
				reason += "cannot check: class is NonPersistent\r\n";
				return true;
			}
			if(!typeof(IList).IsAssignableFrom(MemberInfo.MemberType)) {
				reason += "MemberType is not IList\r\n";
				return true;
			}
			ConstructorInfo constructorInfo = ClassInfo.ClassType.GetConstructor(
				BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(Session) }, null);
			if(constructorInfo == null) {
				reason += "Session specific constructor is absent\r\n";
				return true;
			}
			try {
				obj = TypeHelper.CreateInstance(ClassInfo.ClassType, session);
			}
			catch(Exception e) {
				reason += "cannot create object:" + e.Message + " \r\n";
				return true;
			}
			return false;
		}
		protected override bool ValidateCore(out string text) {
			text = "";
			object val1 = MemberInfo.GetValue(obj);
			object val2 = MemberInfo.GetValue(obj);
			if(val1 != val2) {
				text = "A collection property returns a new object as result each time it is accessed (bad performance)";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorCollectionAlreadyLoaded : ObjectValidatorCollectionReturnsNewValueOnEachAccess {
		public ObjectValidatorCollectionAlreadyLoaded(XPClassInfo classInfo, XPMemberInfo memberInfo) : base(classInfo, memberInfo) { }
		protected override bool ValidateCore(out string text) {
			text = "";
			session.Save(obj);
			session.CommitChanges();
			session.Reload(obj);
			XPBaseCollection xpCollection = MemberInfo.GetValue(obj) as XPBaseCollection;
			if((xpCollection != null) && xpCollection.IsLoaded) {
				text = "A collection property is loaded immediately (bad performance)";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorCollectionDoesntImplementIBindingList : ObjectValidatorMemberIssue {
		public ObjectValidatorCollectionDoesntImplementIBindingList(XPClassInfo classInfo, XPMemberInfo memberInfo) : base(classInfo, memberInfo) { }
		protected override bool GetIsSkipped(out string reason) {
			if(base.GetIsSkipped(out reason)) {
				return true;
			}
			BrowsableAttribute browsableClass = (BrowsableAttribute)ClassInfo.FindAttributeInfo(typeof(BrowsableAttribute));
			bool isClassVisible = (browsableClass == null) || browsableClass.Browsable;
			if(!isClassVisible) {
				reason += "class is not browsable";
				return true;
			}
			BrowsableAttribute browsableMember = (BrowsableAttribute)MemberInfo.FindAttributeInfo(typeof(BrowsableAttribute));
			bool isMemberVisible = (browsableMember == null) || browsableMember.Browsable;
			if(!isMemberVisible) {
				reason += "Member is not browsable";
				return true;
			}
			if(!MemberInfo.IsVisibleInDesignTime) {
				reason += "IsVisibleInDesignTime is false";
				return true;
			}
			if(!MemberInfo.IsPublic) {
				reason += "member is private";
				return true;
			}
			if(!typeof(IList).IsAssignableFrom(MemberInfo.MemberType)) {
				reason += "MemberType is not IList\r\n";
				return true;
			}
			return false;
		}
		protected override bool ValidateCore(out string text) {
			text = "";
			if(!typeof(IBindingList).IsAssignableFrom(MemberInfo.MemberType)) {
				text = "A collection property is visible but doesn't implement the IBindingList interface (this colleciton cannot be shown properly in UI)";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorLargeNonDelayedMember : ObjectValidatorMemberIssue {
		public ObjectValidatorLargeNonDelayedMember(XPClassInfo classInfo, XPMemberInfo memberInfo) : base(classInfo, memberInfo) { }
		protected override bool GetIsSkipped(out string reason) {
			if(base.GetIsSkipped(out reason)) {
				return true;
			}
			if(!MemberInfo.IsPersistent) {
				reason += "non persistent";
				return true;
			}
			if(MemberInfo.MappingFieldSize != SizeAttribute.Unlimited && MemberInfo.MappingFieldSize < 1000
				&& (MemberInfo.Converter == null || MemberInfo.Converter.StorageType != typeof(byte[]))) {
				reason += "member size is not Unlimited, or is less than 1000, or its Converter.StorageType is not 'byte[]'";
				return true;
			}
			return false;
		}
		protected override bool ValidateCore(out string text) {
			text = "";
			if(!MemberInfo.IsDelayed) {
				text = "The member is large but it is not marked as delayed (large data requires some time to load from database and some memory to manage the value. Usually this property is not necessary when a list of objects is loaded.";
				return false;
			}
			return true;
		}
	}
	public class ObjectValidatorCustomAttributeSpecifiesMemberCaptionInCode : ObjectValidatorMemberIssue {
		public ObjectValidatorCustomAttributeSpecifiesMemberCaptionInCode(XPClassInfo classInfo, XPMemberInfo memberInfo) : base(classInfo, memberInfo) { }
		protected override bool ValidateCore(out string text) {
			text = "";
			if(IsCustomAttributeApplied(MemberInfo, "Caption")) {
				text = "A caption for the member is specified via the Custom attribute in code. Use the 'System.ComponentModel.DisplayName' attribute instead.";
				return false;
			}
			if(IsModelDefaultAttributeApplied(MemberInfo, "Caption")) {
				text = "A caption for the member is specified via the ModelDefault attribute in code. Use the 'System.ComponentModel.DisplayName' attribute instead.";
				return false;
			}
			return true;
		}
	}
	public class PersistentObjectValidator {
		public List<ObjectValidatorIssue> ValidateObjects(XPDictionary dictionary) {
			return ValidateObjects(dictionary, false);
		}
		public List<ObjectValidatorIssue> ValidateObjects(XPDictionary dictionary, bool returnFailedIssuesOnly) {
			List<Type> classIssues = new List<Type>();
			List<Type> memberIssues = new List<Type>();
			foreach(Type type in GetType().Assembly.GetTypes()) {
				if(!type.IsAbstract && typeof(ObjectValidatorIssue).IsAssignableFrom(type)) {
					if(typeof(ObjectValidatorMemberIssue).IsAssignableFrom(type)) {
						memberIssues.Add(type);
					}
					else {
						classIssues.Add(type);
					}
				}
			}
			List<ObjectValidatorIssue> results = new List<ObjectValidatorIssue>();
			foreach(XPClassInfo ci in dictionary.Classes) {
				List<ObjectValidatorIssue> currentClassIssues = CreateClassIssues(ci, classIssues);
				foreach(ObjectValidatorIssue issue in currentClassIssues) {
					issue.Validate();
					results.Add(issue);
				}
				foreach(XPMemberInfo mi in GetMembers(ci)) {
					List<ObjectValidatorIssue> currentMemberIssues = CreateMemberIssues(ci, mi, memberIssues);
					foreach(ObjectValidatorIssue issue in currentMemberIssues) {
						issue.Validate();
						results.Add(issue);
					}
				}
			}
			if(returnFailedIssuesOnly) {
				for(int i = results.Count - 1; i >= 0; i--) {
					if(results[i].Status != ObjectValidatorIssueStatus.Failed) {
						results.RemoveAt(i);
					}
				}
			}
			return results;
		}
		private IEnumerable<XPMemberInfo> GetMembers(XPClassInfo ci) {
			if(ci.IsPersistent) {
				List<XPMemberInfo> result = new List<XPMemberInfo>();
				XPClassInfo currentClassInfo = ci;
				do {
					result.AddRange(currentClassInfo.OwnMembers);
					currentClassInfo = currentClassInfo.BaseClass;
				}
				while(currentClassInfo != null && currentClassInfo.ClassType != typeof(object) && currentClassInfo.ClassType != null);
				return result;
			}
			else {
				return ci.OwnMembers;
			}
		}
		private List<ObjectValidatorIssue> CreateClassIssues(XPClassInfo ci, List<Type> classIssueTypes) {
			List<ObjectValidatorIssue> results = new List<ObjectValidatorIssue>();
			foreach(Type type in classIssueTypes) {
				results.Add((ObjectValidatorIssue)TypeHelper.CreateInstance(type, ci));
			}
			return results;
		}
		private List<ObjectValidatorIssue> CreateMemberIssues(XPClassInfo ci, XPMemberInfo mi, List<Type> memberIssueTypes) {
			List<ObjectValidatorIssue> results = new List<ObjectValidatorIssue>();
			foreach(Type type in memberIssueTypes) {
				results.Add((ObjectValidatorIssue)TypeHelper.CreateInstance(type, ci, mi));
			}
			return results;
		}
	}
}

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
using System.Diagnostics;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.ClassGeneration;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.Security.ClientServer;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
namespace DevExpress.ExpressApp.Security {
	public class DefaultValuesHelper {
		public static object GetDefaultValue(ITypeInfo typeInfo) {
			if(typeInfo.IsNullable) {
				return null;
			}
			if(typeInfo.Type == typeof(bool)) {
				return default(bool);
			}
			if(typeInfo.Type == typeof(byte)) {
				return default(byte);
			}
			if(typeInfo.Type == typeof(char)) {
				return default(char);
			}
			if(typeInfo.Type == typeof(decimal)) {
				return default(decimal);
			}
			if(typeInfo.Type == typeof(double)) {
				return default(decimal);
			}
			if(typeInfo.Type == typeof(float)) {
				return default(decimal);
			}
			if(typeInfo.Type == typeof(int)) {
				return default(int);
			}
			if(typeInfo.Type == typeof(long)) {
				return default(long);
			}
			if(typeInfo.Type == typeof(sbyte)) {
				return default(sbyte);
			}
			if(typeInfo.Type == typeof(short)) {
				return default(short);
			}
			if(typeInfo.Type == typeof(uint)) {
				return default(uint);
			}
			if(typeInfo.Type == typeof(ushort)) {
				return default(ushort);
			}
			if(typeInfo.Type == typeof(ulong)) {
				return default(ulong);
			}
			return default(object);
		}
	}
	public class SecurityRuleProvider : ISecurityRuleProvider {
		public readonly ISecurityRule securityRule;
		public SecurityRuleProvider(ISecurityRule securityRule) {
			this.securityRule = securityRule;
		}
		public SecurityRuleProvider(XPDictionary dic, ISelectDataSecurity security) : this(dic, security, null) { }
		public SecurityRuleProvider(XPDictionary dic, ISelectDataSecurity security, DelayedPermissionsProvider delayedPermissionsListProvider) {
			if(SecurityStrategy.TraceLevel != TraceLevel.Off) {
				securityRule = new SecurityRuleLogger(dic, security, new FilterLogger(Logger.ConvertToLogLevel(SecurityStrategy.TraceLevel), Logger.Instance), delayedPermissionsListProvider);
			}
			else {
				securityRule = new SecurityRule(dic, security, delayedPermissionsListProvider);
			}
		}
		public ISecurityRule GetRule(XPClassInfo classInfo) {
			return securityRule;
		}
	}
	public class XpoSecurityExpressionEvaluator : ISecurityExpressionEvaluator {
		bool isParentSession;
		private readonly DevExpress.Xpo.SecurityContext context;
		public XpoSecurityExpressionEvaluator(DevExpress.Xpo.SecurityContext context, bool isParentSession) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			this.isParentSession = isParentSession;
		}
		public bool Fit(string criteriaString, object targetObject) {
			Guard.ArgumentNotNull(targetObject, "targetObject");
			XPClassInfo classInfo = context.GetClassInfo(targetObject);
			Guard.ArgumentNotNull(classInfo, "session.GetClassInfo(targetObject)");
			CriteriaOperator criteria = isParentSession ? context.ParseCriteriaOnParentSession(criteriaString) : context.ParseCriteria(criteriaString);
			if(targetObject != null && targetObject is XPObject && ((XPObject)targetObject).Session.ObjectLayer is SecuredSessionObjectLayer) {
				SecuredSessionObjectLayer directObjectLayer = ((XPObject)targetObject).Session.ObjectLayer as SecuredSessionObjectLayer;
				TypesInfo typesInfo = (TypesInfo)XafTypesInfo.Instance;
				XpoTypeInfoSource xpoTypeInfoSource = XpoTypesInfoHelper.GetXpoTypeInfoSource();
				XPObjectSpace xpOS = new XPObjectSpace(XafTypesInfo.Instance, xpoTypeInfoSource, () => new UnitOfWork(new SimpleObjectLayer(directObjectLayer.DataLayer)));
				ExpressionEvaluator expressionEvaluator = xpOS.GetExpressionEvaluator(xpOS.GetObjectType(targetObject), criteria);
				xpOS.CreateObject(targetObject.GetType());
				return expressionEvaluator.Fit(targetObject);
			}
			return this.context.Fit(classInfo, criteria, targetObject);
		}
	}
	public class SecurityRule : ISecurityRule {
		private readonly ISelectDataSecurity security;
		private readonly XPClassInfo[] supportedObjectTypes;
		private readonly DelayedPermissionsProvider delayedPermissionsListProvider;
		private static CriteriaOperator ParseCriteriaList(DevExpress.Xpo.SecurityContext context, IList<string> criteriaList) {
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(criteriaList, "criteriaList");
			CriteriaOperator result = null;
			if(criteriaList.Count == 0) {
				return null;
			}
			else if(criteriaList.Count == 1) {
				result = context.ParseCriteriaOnParentSession(criteriaList[0]);
			}
			else {
				result = new GroupOperator(GroupOperatorType.Or);
				foreach(string criteriaString in criteriaList) {
					((GroupOperator)result).Operands.Add(context.ParseCriteriaOnParentSession(criteriaString));
				}
			}
			return result;
		}
		internal virtual bool TryGetIsGrantedForIntermediateObject(DevExpress.Xpo.SecurityContext context, object theObject, object realObjectOnLoad, string operation, out bool isGranted) {
			if(TryGetIsGrantedForIntermediateObject(context, theObject, operation, false, out isGranted)) {
				if(isGranted && realObjectOnLoad != null) {
					TryGetIsGrantedForIntermediateObject(context, realObjectOnLoad, operation, true, out isGranted);
				}
				return true;
			}
			isGranted = false;
			return false;
		}
		internal virtual bool TryGetIsGrantedForIntermediateObject(DevExpress.Xpo.SecurityContext context, object targetObject, string operation, bool isParentSession, out bool isGranted) {
			object leftObject, rightObject;
			if(IsIntermediateObject(targetObject, out leftObject, out rightObject)) {
				isGranted = IsGranted(context, leftObject, null, operation, isParentSession) && IsGranted(context, rightObject, null, operation, isParentSession);
				return true;
			}
			isGranted = false;
			return false;
		}
		internal virtual bool IsIntermediateObject(object theObject, out object leftObject, out object rightObject) {
			IntermediateObject xpoIntermediateObject = theObject as IntermediateObject;
			if(xpoIntermediateObject != null) {
				leftObject = xpoIntermediateObject.LeftIntermediateObjectField;
				rightObject = xpoIntermediateObject.RightIntermediateObjectField;
				return true;
			}
			IDCIntermediateObject dcIntermediateObject = theObject as IDCIntermediateObject;
			if(dcIntermediateObject != null) {
				leftObject = dcIntermediateObject.LeftObject;
				rightObject = dcIntermediateObject.RightObject;
				return true;
			}
			leftObject = null;
			rightObject = null;
			return false;
		}
		internal virtual bool IsIntermediateObjectType(Type type) {
			return typeof(IntermediateObject).IsAssignableFrom(type) || typeof(IDCIntermediateObject).IsAssignableFrom(type);
		}
		internal virtual bool IsGranted(DevExpress.Xpo.SecurityContext context, object theObject, object realObjectOnLoad, string memberName, string operation) {
			if(realObjectOnLoad != null && !IsGranted(context, realObjectOnLoad, memberName, operation, true)) {
				return false;
			}
			if(operation == SecurityOperations.Delete) {
				return true;
			}
			return IsGranted(context, theObject, memberName, operation, false);
		}
		internal virtual bool IsGranted(DevExpress.Xpo.SecurityContext context, object targetObject, string memberName, string operation, bool isParentSession) {
			bool isGranted;
			if(!TryGetIsGrantedForSharedPart(context, targetObject, memberName, operation, isParentSession, out isGranted)) {
				isGranted = IsGrantedCore(context, targetObject, memberName, operation, isParentSession);
			}
			return isGranted;
		}
		internal virtual bool TryGetIsGrantedForSharedPart(DevExpress.Xpo.SecurityContext context, object targetObject, string memberName, string operation, bool isParentSession, out bool isGranted) {
			ISharedPart sharedPart = targetObject as ISharedPart;
			if(sharedPart != null) {
				isGranted = IsGrantedCore(context, sharedPart.Entity, memberName, operation, isParentSession);
				return true;
			}
			isGranted = false;
			return false;
		}
		internal virtual bool IsSharedPartType(Type type) {
			return typeof(ISharedPart).IsAssignableFrom(type);
		}
		private bool IsReferenceToPersistentInterfaceData(XPMemberInfo memberInfo) {
			return memberInfo.Name.StartsWith("Alias_")
				&& IsPersistentInterfaceDataType(memberInfo.MemberType);
		}
		private bool IsPersistentInterfaceDataType(Type type) {
			return type.IsInterface
				&& type.IsGenericType
				&& type.GetGenericTypeDefinition() == typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>);
		}
		private bool IsGrantedCore(DevExpress.Xpo.SecurityContext context, object targetObject, string memberName, string operation, bool isParentSession) {
			IPermissionDictionary permissionDictionary = null;
			if(delayedPermissionsListProvider != null) {
				permissionDictionary = delayedPermissionsListProvider.Permissions;
			}
			ServerPermissionRequest request = new ServerPermissionRequest(targetObject, memberName, operation, new XpoSecurityExpressionEvaluator(context, isParentSession), permissionDictionary);
			return security.IsGranted(request);
		}
		public SecurityRule(XPDictionary dic, ISelectDataSecurity security) : this(dic, security, null) { }
		public SecurityRule(XPDictionary dic, ISelectDataSecurity security, DelayedPermissionsProvider delayedPermissionsListProvider) {
			this.security = security ?? new EmptySelectDataSecurity();
			supportedObjectTypes = new List<XPClassInfo>(Enumerator.Convert<XPClassInfo>(dic.Classes)).ToArray();
			this.delayedPermissionsListProvider = delayedPermissionsListProvider;
		}
		public virtual bool GetSelectFilterCriteria(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, out CriteriaOperator criteria) { 
			Guard.ArgumentNotNull(classInfo, "classInfo");
			criteria = null;
			if(classInfo.ClassType == null) { 
				return false;
			}
			if(IsIntermediateObjectType(classInfo.ClassType)
				|| IsSharedPartType(classInfo.ClassType) 
			) {
				return false;
			}
			IList<string> criteriaList = null;
			IPermissionDictionary permissionDictionary = null;
			if(delayedPermissionsListProvider != null) {
				permissionDictionary = delayedPermissionsListProvider.Permissions;
			}
			if(security is ISecurityCriteriaProvider3 && permissionDictionary != null) {
				criteriaList = ((ISecurityCriteriaProvider3)security).GetObjectCriteria(classInfo.ClassType, permissionDictionary);
			}
			else {
				criteriaList = security.GetObjectCriteria(classInfo.ClassType);
			}
			if((criteriaList == null) || (criteriaList.Count == 0)) {
				return false;
			}
			else {
				criteria = ParseCriteriaList(context, criteriaList);
				return true;
			}
		}
		public virtual bool GetSelectMemberExpression(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, XPMemberInfo memberInfo, out CriteriaOperator expression) {
			Guard.ArgumentNotNull(classInfo, "classInfo");
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
			expression = null;
			if(classInfo.ClassType == null) { 
				return false;
			}
			if(IsIntermediateObjectType(classInfo.ClassType)
				|| IsSharedPartType(classInfo.ClassType) 
				|| IsSharedPartType(memberInfo.MemberType) 
				|| IsReferenceToPersistentInterfaceData(memberInfo)
			) {
				return false;
			}
			IList<string> criteriaList = null;
			IPermissionDictionary permissionDictionary = null;
			if(delayedPermissionsListProvider != null) {
				permissionDictionary = delayedPermissionsListProvider.Permissions;
			}
			if(security is ISecurityCriteriaProvider3 && permissionDictionary != null) {
				criteriaList = ((ISecurityCriteriaProvider3)security).GetMemberCriteria(classInfo.ClassType, memberInfo.Name, permissionDictionary);
			}
			else {
				criteriaList = security.GetMemberCriteria(classInfo.ClassType, memberInfo.Name);
			}
			if((criteriaList == null) || (criteriaList.Count == 0)) {
				expression = null;
				return false;
			}
			else {
				CriteriaOperator criteria = ParseCriteriaList(context, criteriaList);
				if(CriteriaOperator.CriterionEquals(criteria, ServerPermissionRequestProcessor.FalseCriteriaOperator)) {
					expression = new OperandValue(null);
				}
				else {
					expression = new FunctionOperator(FunctionOperatorType.Iif, criteria, new OperandProperty(memberInfo.Name), new OperandValue(null));
				}
				return true;
			}
		}
		private bool AreEqual(XPMemberInfo memberInfo, object left, object right) {
			if(memberInfo.ReferenceType == null) {
				return Object.Equals(left, right);
			}
			else if(left == right) {
				return true;
			}
			else if((left == null && right != null) || (right == null && left != null)) {
				return false;
			}
			else if(memberInfo.ReferenceType.KeyProperty != null) {
				return Object.Equals(memberInfo.ReferenceType.KeyProperty.GetValue(right), memberInfo.ReferenceType.KeyProperty.GetValue(left));
			}
			else {
				return false;
			}
		}
		public virtual ValidateMemberOnSaveResult ValidateMemberOnSave(DevExpress.Xpo.SecurityContext context, XPMemberInfo memberInfo, object theObject, object realObjectOnLoad, object value, object valueOnLoad, object realValueOnLoad) {
			Guard.ArgumentNotNull(theObject, "theObject");
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
			if(IsSharedPartType(memberInfo.MemberType) 
				|| (memberInfo.Name == "_Instance" && IsSharedPartType(theObject.GetType())) 
				|| (memberInfo.Name.EndsWith("_Data") && IsSharedPartType(theObject.GetType()) && IsPersistentInterfaceDataType(memberInfo.MemberType)) 
				|| IsReferenceToPersistentInterfaceData(memberInfo)
			) {
				return ValidateMemberOnSaveResult.DoSaveMember;
			}
			object leftObject, rightObject;
			if(IsIntermediateObject(theObject, out leftObject, out rightObject)) {
				return ValidateMemberOnSaveResult.DoSaveMember; 
			}
			else if(!IsGranted(context, theObject, realObjectOnLoad, memberInfo.Name, SecurityOperations.Read)) {
				if((value == null || value.Equals(DefaultValuesHelper.GetDefaultValue(XafTypesInfo.Instance.FindTypeInfo(memberInfo.MemberType)))) && (valueOnLoad == null)) {
					return ValidateMemberOnSaveResult.DoNotSaveMember; 
				}
				else {
					if(context.IsObjectMarkedDeleted(theObject)) {
						return ValidateMemberOnSaveResult.DoSaveMember;
					}
					else {
						return ValidateMemberOnSaveResult.DoRaiseException;
					}
				}
			}
			else if(!IsGranted(context, theObject, realObjectOnLoad, memberInfo.Name, SecurityOperations.Write)) {
				if(AreEqual(memberInfo, value, valueOnLoad)) {
					return ValidateMemberOnSaveResult.DoSaveMember; 
				}
				else {
					if(context.IsObjectMarkedDeleted(theObject)) {
						return ValidateMemberOnSaveResult.DoSaveMember;
					}
					else {
						return ValidateMemberOnSaveResult.DoRaiseException; 
					}
				}
			}
			else {
				return ValidateMemberOnSaveResult.DoSaveMember; 
			}
		}
		public virtual ValidateMemberOnSaveResult ValidateMemberOnSave(DevExpress.Xpo.SecurityContext context, XPMemberInfo memberInfo, object targetObject, object value, object valueOnLoad) {
			Guard.ArgumentNotNull(targetObject, "targetObject");
			Guard.ArgumentNotNull(memberInfo, "memberInfo");
			if(IsSharedPartType(memberInfo.MemberType) 
				|| (memberInfo.Name == "_Instance" && IsSharedPartType(targetObject.GetType())) 
				|| (memberInfo.Name.EndsWith("_Data") && IsSharedPartType(targetObject.GetType()) && IsPersistentInterfaceDataType(memberInfo.MemberType)) 
				|| IsReferenceToPersistentInterfaceData(memberInfo)
			) {
				return ValidateMemberOnSaveResult.DoSaveMember;
			}
			object leftObject, rightObject;
			if(IsIntermediateObject(targetObject, out leftObject, out rightObject)) {
				return ValidateMemberOnSaveResult.DoSaveMember; 
			}
			else if(!IsGranted(context, targetObject, memberInfo.Name, SecurityOperations.Read, true)) {
				if((value == null || value.Equals(DefaultValuesHelper.GetDefaultValue(XafTypesInfo.Instance.FindTypeInfo(memberInfo.MemberType)))) && (valueOnLoad == null)) {
					return ValidateMemberOnSaveResult.DoNotSaveMember; 
				}
				else {
					if(context.IsObjectMarkedDeleted(targetObject)) {
						return ValidateMemberOnSaveResult.DoSaveMember;
					}
					else {
						return ValidateMemberOnSaveResult.DoRaiseException;
					}
				}
			}
			else if(!IsGranted(context, targetObject, memberInfo.Name, SecurityOperations.Write, true)) {
				if(AreEqual(memberInfo, value, valueOnLoad)) {
					return ValidateMemberOnSaveResult.DoSaveMember; 
				}
				else {
					if(context.IsObjectMarkedDeleted(targetObject)) {
						return ValidateMemberOnSaveResult.DoSaveMember;
					}
					else {
						return ValidateMemberOnSaveResult.DoRaiseException; 
					}
				}
			}
			else {
				return ValidateMemberOnSaveResult.DoSaveMember; 
			}
		}
		public virtual bool ValidateObjectOnDelete(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object theObject, object realObjectOnLoad) {
			bool isGranted;
			if(!TryGetIsGrantedForIntermediateObject(context, theObject, realObjectOnLoad, SecurityOperations.Write, out isGranted)) {
				isGranted = IsGranted(context, theObject, realObjectOnLoad, null, SecurityOperations.Delete);
			}
			return isGranted;
		}
		public virtual bool ValidateObjectOnSave(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object theObject, object realObjectOnLoad) {
			bool isGranted;
			if(!TryGetIsGrantedForIntermediateObject(context, theObject, realObjectOnLoad, SecurityOperations.Write, out isGranted)) {
				string operation = realObjectOnLoad == null ? SecurityOperations.Create : SecurityOperations.Write;
				if(context.IsObjectMarkedDeleted(theObject)) {
					operation = SecurityOperations.Delete;
				}
				isGranted = IsGranted(context, theObject, realObjectOnLoad, null, operation);
			}
			return isGranted;
		}
		public virtual bool ValidateObjectOnSave(DevExpress.Xpo.SecurityContext context, object targetObject) {
			bool isGranted;
			if(!TryGetIsGrantedForIntermediateObject(context, targetObject, SecurityOperations.Write, true, out isGranted)) {
				ISessionProvider sessionProvider = targetObject as ISessionProvider;
				if(sessionProvider != null) {
					string operation = sessionProvider.Session.IsObjectMarkedDeleted(targetObject) ? SecurityOperations.Delete : SecurityOperations.Write;
					isGranted = IsGranted(context, targetObject, null, operation, true);
				}
			}
			return isGranted;
		}
		public virtual bool ValidateObjectOnSelect(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object realObjectOnLoad) {
			Guard.ArgumentNotNull(realObjectOnLoad, "realObjectOnLoad"); 
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(classInfo, "classInfo");
			bool isGranted;
			if(!TryGetIsGrantedForIntermediateObject(context, realObjectOnLoad, SecurityOperations.Read, true, out isGranted)) {
				isGranted = IsGranted(context, realObjectOnLoad, null, SecurityOperations.Read, true);
			}
			return isGranted;
		}
		public XPClassInfo[] SupportedObjectTypes {
			get { return supportedObjectTypes; }
		}
		#region ISupportExtensionSecurityRule Members
		public bool EnableSupportExtensionSecurityRule { get; set; }
		#endregion       
	}
}

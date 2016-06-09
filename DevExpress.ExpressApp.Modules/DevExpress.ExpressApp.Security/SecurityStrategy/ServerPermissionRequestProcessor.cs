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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security {
	public interface ISecurityExpressionEvaluator {
		bool Fit(string criteriaString, object targetObject);
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class SecurityExpressionEvaluator : ISecurityExpressionEvaluator {
		IObjectSpace objectSpace;
		Dictionary<string, bool> fitCash = new Dictionary<string, bool>();
		public SecurityExpressionEvaluator(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
		public bool Fit(string criteriaString, object targetObject) {
			string key = targetObject.GetHashCode().ToString() + criteriaString;
			bool result;
			ExpressionEvaluator evaluator;
			if(!fitCash.TryGetValue(key, out result)) {
				if(targetObject is XafDataViewRecord) {
					DataViewEvaluatorContextDescriptor contextDescriptor = new DataViewEvaluatorContextDescriptor(objectSpace);
					evaluator = objectSpace.GetExpressionEvaluator(contextDescriptor, objectSpace.ParseCriteria(criteriaString));
				}
				else {
					evaluator = objectSpace.GetExpressionEvaluator(targetObject.GetType(), objectSpace.ParseCriteria(criteriaString));
				}
				result = evaluator.Fit(targetObject);
				fitCash[key] = result;
			}
			return result;
		}
		public IObjectSpace ObjectSpace { get { return objectSpace; } }
	}
	public class ServerPermissionRequest : OperationPermissionRequestBase, IObjectTypePermissionRequst {
		private object hashCodeObject;
		public ServerPermissionRequest(Type objectType, object targetObject, string memberName, string operation, ISecurityExpressionEvaluator expressionEvaluatorProvider) : this(objectType, targetObject, memberName, operation, expressionEvaluatorProvider, null) { }
		public ServerPermissionRequest(Type objectType, object targetObject, string memberName, string operation, ISecurityExpressionEvaluator expressionEvaluatorProvider, IPermissionDictionary permissionDictionary)
			: base(operation) {
			MemberName = memberName;
			TargetObject = targetObject;
			ObjectType = objectType;
			Guard.ArgumentNotNull(expressionEvaluatorProvider, "expressionEvaluatorProvider");
			ExpressionEvaluatorProvider = expressionEvaluatorProvider;
			PermissionDictionary = permissionDictionary;
		}
		public ServerPermissionRequest(object targetObject, string memberName, string operation, ISecurityExpressionEvaluator expressionEvaluatorProvider)
			: this(GetObjectType(targetObject), targetObject, memberName, operation, expressionEvaluatorProvider) { }
		public ServerPermissionRequest(object targetObject, string memberName, string operation, ISecurityExpressionEvaluator expressionEvaluatorProvider, IPermissionDictionary permissionDictionary)
			: this(GetObjectType(targetObject), targetObject, memberName, operation, expressionEvaluatorProvider, permissionDictionary) { }
		public override object GetHashObject() {
			if(hashCodeObject == null) {
				System.Text.StringBuilder key = new System.Text.StringBuilder();
				key.Append(GetType().Name);
				key.Append(" - ");
				key.Append(ObjectType.FullName);
				key.Append(" - ");
				key.Append(Operation);
				key.Append(" - ");
				string targetObjectHashCode = (TargetObject == null ? "null" : TargetObject.GetHashCode().ToString());
				key.Append(targetObjectHashCode);
				key.Append(" - ");
				key.Append(MemberName);
				if(UseStringHashCodeObject) {
					hashCodeObject = key.ToString();
				}
				else {
					hashCodeObject = key.ToString().GetHashCode();
				}
			}
			return hashCodeObject;
		}
		private static Type GetObjectType(object targetObject) {
			Guard.ArgumentNotNull(targetObject, "targetObject");
			return targetObject.GetType();
		}
		public Type ObjectType { get; private set; }
		public string MemberName { get; private set; }
		public ISecurityExpressionEvaluator ExpressionEvaluatorProvider { get; private set; }
		public object TargetObject { get; private set; }
		public IPermissionDictionary PermissionDictionary { get; set; }
		public IObjectSpace ObjectSpace {
			get {
				if(ExpressionEvaluatorProvider is SecurityExpressionEvaluator) {
					return ((SecurityExpressionEvaluator)ExpressionEvaluatorProvider).ObjectSpace;
				}
				return null;
			}
		}
	}
	public class RequestGrantedCache {
		private Dictionary<object, bool> cacheIsGranted = new Dictionary<object, bool>();
		public void Clear() {
			cacheIsGranted.Clear();
		}
		public void Set(IPermissionRequest request, bool isGranted) {
			cacheIsGranted[request.GetHashObject()] = isGranted;
		}
		public bool TryGetValue(IPermissionRequest request, out bool isGranted) {
			if(cacheIsGranted.TryGetValue(request.GetHashObject(), out isGranted)) {
				return true;
			}
			return false;
		}
	}
	public class ServerPermissionRequestProcessor : IPermissionRequestProcessor, ISecurityCriteriaProvider2, ISecurityCriteriaProvider3, IReadPermissionVersionProvider {
		public static char[] Delimiters = new char[] { ';', ',' }; 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use 'CanUseCache' instead.", true)]
		public static bool CanUseCacheDefaultValue = true;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(true)]
		public static bool CanUseCache = true;
		public readonly static string FalseCriteria = "1=0";
		public readonly static CriteriaOperator FalseCriteriaOperator = CriteriaOperator.Parse(FalseCriteria);
		public const string SelectAllString = "*";
		internal const int MaxMembersCount = 1000; 
		private IPermissionDictionary permissions;
		private RequestGrantedCache requestGrantedCache = new RequestGrantedCache();
		internal static bool IsCriteriaVersionNumber(int val) {
			return (val < 0 ) || (val > MaxMembersCount );
		}
		private static void AddMember(Dictionary<Type, HashSet<string>> grantedMembers, Type type, string memberName) {
			HashSet<string> members;
			if(!grantedMembers.TryGetValue(type, out members)) {
				members = new HashSet<string>();
				grantedMembers[type] = members;
			}
			members.Add(memberName);
		}
		private static ITypeInfo FindTypeInfo(Type type) {
			return XafTypesInfo.Instance.FindTypeInfo(type);
		}
		private static string GetSubPath(IList<IMemberInfo> memberPathList, int startIndex) {
			string result = "";
			for(int i = startIndex; i < memberPathList.Count; i++) {
				result += memberPathList[i].Name + '.';
			}
			return result.TrimEnd('.');
		}
		private static bool IsEmpty(IEnumerable source) {
			IEnumerator enumerator = source.GetEnumerator();
			return !enumerator.MoveNext();
		}
		private static Type GetTypeToProcess(Type targetType) {
			Type result = targetType;
			if(targetType != null) {
				ITypeInfo typeInfo = FindTypeInfo(targetType);
				if(typeInfo != null) {
					result = typeInfo.Type;
				}
			}
			return result;
		}
		private static bool TryGetCanDeleteObjectWithManyToManyCollection(object targetObject, out bool result) {
			result = false;
			return false;
		}
		private static bool IsMemberOperation(string operation) {
			return (operation == SecurityOperations.Read || operation == SecurityOperations.Write);
		}
		private bool MemberIsUpCasted(IMemberInfo memberInfo, Type currentType) {
			if(memberInfo != null && memberInfo.Owner != null && XafTypesInfo.Instance.FindTypeInfo(memberInfo.Owner.Type) != null) {
				return !memberInfo.Owner.Type.IsAssignableFrom(currentType) && currentType.IsAssignableFrom(memberInfo.Owner.Type);
			}
			return false;
		}
		private bool ProcessUpCastingMember(IMemberInfo memberInfo, Type currentType, string operation, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary.FindFirst<TypeOperationPermission>(memberInfo.Owner.Type, operation) != null ||
				permissionDictionary.GetPermissions<MemberOperationPermission>().Any(p => p.ObjectType == memberInfo.Owner.Type && Contains(p.MemberName, memberInfo.Name))) {
				return true;
			}
			return false;
		}
		protected virtual bool TryCreateReferenceMemberSubRequest(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, bool needToCheckBackReference, out ServerPermissionRequest request) {
			if(memberPathList != null && memberPathList.Count > 0) {
				IMemberInfo mi = memberPathList[0];
				request = null;
				if(!mi.IsAssociation) {
					if(!IsSecuredType(mi.MemberType)) {
						return false;
					}
					if(memberPathList.Count == 1 && (permissionRequest.Operation == SecurityOperations.Write)) {
						request = null;
						return false;
					}
					object targetObject = permissionRequest.TargetObject;
					object referencedObject = (targetObject != null) ? mi.GetValue(targetObject) : null;
					request = new ServerPermissionRequest(mi.MemberType,
							referencedObject, GetSubPath(memberPathList, 1), permissionRequest.Operation, permissionRequest.ExpressionEvaluatorProvider);
					return true;
				}
				else if(mi.IsAssociation && needToCheckBackReference && (permissionRequest.Operation == SecurityOperations.Write || permissionRequest.Operation == SecurityOperations.Read)) {
					object associatedObject = mi.IsList ? null : mi.GetValue(permissionRequest.TargetObject);
					request = new ServerPermissionRequest(mi.AssociatedMemberInfo.Owner.Type, associatedObject, mi.AssociatedMemberInfo.Name,
						permissionRequest.Operation, permissionRequest.ExpressionEvaluatorProvider);
					return true;
				}
			}
			request = null;
			return false;
		}
		protected virtual bool TryExtractMemberPath(Type targetType, string memberName, out IList<IMemberInfo> memberPath) {
			if(String.IsNullOrEmpty(memberName)) {
				memberPath = null;
				return true;
			}
			ITypeInfo typeInfo = FindTypeInfo(targetType);
			IMemberInfo memberInfo = typeInfo.FindMember(memberName);
			if(memberInfo != null) {
				memberPath = memberInfo.GetPath();
				return true;
			}
			else {
				memberPath = null;
				return false;
			}
		}
		protected virtual bool Contains(string permissionMemberName, string requestedMemberName) {
			bool result = false;
			if(!string.IsNullOrEmpty(permissionMemberName)) {
				if(requestedMemberName == permissionMemberName) {
					result = true;
				}
				else {
					List<string> targetItems = new List<string>();
					foreach(string s in permissionMemberName.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries)) {
						targetItems.Add(s.Trim());
					}
					result = targetItems.Contains(requestedMemberName);
					if(targetItems.Contains(SelectAllString)) {
						result = !result;
					}
				}
			}
			return result;
		}
		protected virtual bool IsOperationGrantedByTypePermissions(Type targetObjectType, string operation) {
			return IsOperationGrantedByTypePermissions(targetObjectType, operation, permissions);
		}
		private bool IsOperationGrantedByTypePermissions(Type targetObjectType, string operation, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			return (permissionDictionary.FindFirst<TypeOperationPermission>(targetObjectType, operation) != null);
		}
		protected virtual bool IsOperationGrantedByObjectPermissions(object targetObject, ISecurityExpressionEvaluator expressionEvaluatorProvider, string operation) {
			return IsOperationGrantedByObjectPermissions(targetObject, expressionEvaluatorProvider, operation, permissions);
		}
		private bool IsOperationGrantedByObjectPermissions(object targetObject, ISecurityExpressionEvaluator expressionEvaluatorProvider, string operation, IPermissionDictionary permissionDictionary) {
			Guard.ArgumentNotNull(targetObject, "targetObject");
			Guard.ArgumentNotNull(expressionEvaluatorProvider, "expressionEvaluatorProvider");
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			Type objectType;
			if(targetObject is XafDataViewRecord) {
				objectType = ((XafDataViewRecord)targetObject).ObjectType;
			}
			else {
				objectType = targetObject.GetType();
			}
			foreach(ObjectOperationPermission objectOperationPermission in permissionDictionary.GetPermissions<ObjectOperationPermission>(objectType, operation, CompareTypeMode.IsAssignable)) {
				if(expressionEvaluatorProvider.Fit(objectOperationPermission.Criteria, targetObject)) {
					return true;
				}
			}
			return false;
		}
		protected virtual bool IsOperationGrantedByMemberPermissions(Type targetObjectType, string memberName, string operation) {
			return IsOperationGrantedByMemberPermissions(targetObjectType, memberName, operation, permissions);
		}
		private bool IsOperationGrantedByMemberPermissions(Type targetObjectType, string memberName, string operation, IPermissionDictionary permissionDictionary) {
			if(!IsMemberOperation(operation)) {
				return false;
			}
			Guard.ArgumentNotNull(targetObjectType, "targetObjectType");
			Guard.ArgumentNotNullOrEmpty(memberName, "memberName");
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			foreach(MemberOperationPermission p in permissionDictionary.GetPermissions<MemberOperationPermission>(targetObjectType, operation, CompareTypeMode.IsAssignable)) {
				if(Contains(p.MemberName, memberName)) {
					return true;
				}
			}
			return false;
		}
		protected virtual bool IsOperationGrantedByMemberCriteriaPermissions(object targetObject, string memberName, ISecurityExpressionEvaluator expressionEvaluatorProvider, string operation) {
			return IsOperationGrantedByMemberCriteriaPermissions(targetObject, memberName, expressionEvaluatorProvider, operation, permissions);
		}
		private bool IsOperationGrantedByMemberCriteriaPermissions(object targetObject, string memberName, ISecurityExpressionEvaluator expressionEvaluatorProvider, string operation, IPermissionDictionary permissionDictionary) {
			if(!IsMemberOperation(operation)) {
				return false;
			}
			Guard.ArgumentNotNull(targetObject, "targetObject");
			Guard.ArgumentNotNull(expressionEvaluatorProvider, "expressionEvaluatorProvider");
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			Type objectType;
			if(targetObject is XafDataViewRecord) {
				objectType = ((XafDataViewRecord)targetObject).ObjectType;
			}
			else {
				objectType = targetObject.GetType();
			}
			foreach(MemberCriteriaOperationPermission p in permissionDictionary.GetPermissions<MemberCriteriaOperationPermission>(objectType, operation, CompareTypeMode.IsAssignable)) {
				if((string.IsNullOrEmpty(memberName) || Contains(p.MemberName, memberName)) && expressionEvaluatorProvider.Fit(p.Criteria, targetObject)) {
					return true;
				}
			}
			return false;
		}
		protected virtual bool IsAnyObjectPermissionGranted(Type targetObjectType, string operation) {
			return IsAnyObjectPermissionGranted(targetObjectType, operation, false);
		}
		private bool IsAnyObjectPermissionGranted(Type targetObjectType, string operation, IPermissionDictionary permissionDictionary) {
			return IsAnyObjectPermissionGranted(targetObjectType, operation, false, permissionDictionary);
		}
		protected virtual bool IsAnyObjectPermissionGranted(Type targetObjectType, string operation, bool checkInheritance) {
			return IsAnyObjectPermissionGranted(targetObjectType, operation, checkInheritance, permissions);
		}
		private bool IsAnyObjectPermissionGranted(Type targetObjectType, string operation, bool checkInheritance, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			bool result = false;
			while(targetObjectType != null && targetObjectType != typeof(object)) {
				result = permissionDictionary.FindFirst<ObjectOperationPermission>(targetObjectType, operation) != null;
				if(result || !checkInheritance) {
					break;
				}
				targetObjectType = targetObjectType.BaseType;
			}
			return result;
		}
		protected virtual bool IsAnyMemberPermissionGranted(Type targetObjectType, string operation) {
			return IsAnyMemberPermissionGranted(targetObjectType, operation, permissions);
		}
		private bool IsAnyMemberPermissionGranted(Type targetObjectType, string operation, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			if(!IsMemberOperation(operation)) {
				return false;
			}
			IEnumerable<MemberOperationPermission> items = permissionDictionary.GetPermissions<MemberOperationPermission>(targetObjectType, operation, CompareTypeMode.IsAssignable);
			return !IsEmpty(items);
		}
		protected virtual bool IsAnyMemberCriteriaPermissionGranted(Type targetObjectType, string operation) {
			return IsAnyMemberCriteriaPermissionGranted(targetObjectType, operation, permissions);
		}
		private bool IsAnyMemberCriteriaPermissionGranted(Type targetObjectType, string operation, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			if(!IsMemberOperation(operation)) {
				return false;
			}
			IEnumerable<MemberCriteriaOperationPermission> memberCriteriaItems = permissionDictionary.GetPermissions<MemberCriteriaOperationPermission>(targetObjectType, operation, CompareTypeMode.IsAssignable);
			return !IsEmpty(memberCriteriaItems);
		}
		protected virtual bool IsAnyMemberCriteriaPermissionGranted(Type targetObjectType, string memberName, string operation) {
			return IsAnyMemberCriteriaPermissionGranted(targetObjectType, memberName, operation, permissions);
		}
		private bool IsAnyMemberCriteriaPermissionGranted(Type targetObjectType, string memberName, string operation, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			if(!IsMemberOperation(operation)) {
				return false;
			}
			IEnumerable<MemberCriteriaOperationPermission> items = permissionDictionary.GetPermissions<MemberCriteriaOperationPermission>(targetObjectType, operation, CompareTypeMode.IsAssignable);
			foreach(MemberCriteriaOperationPermission p in items) {
				if(Contains(p.MemberName, memberName)) {
					return true;
				}
			}
			return false;
		}
		protected virtual bool GetIsDeleteGranted(object targetObject, Type targetType, ISecurityExpressionEvaluator expressionEvaluatorProvider) {
			return GetIsDeleteGranted(targetObject, targetType, expressionEvaluatorProvider, permissions);
		}
		private bool GetIsDeleteGranted(object targetObject, Type targetType, ISecurityExpressionEvaluator expressionEvaluatorProvider, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary == null) {
				permissionDictionary = permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			ITypeInfo typeInfo = FindTypeInfo(targetType);
			foreach(IMemberInfo mi in typeInfo.Members) {
				if(mi.IsAssociation) {
					if(!mi.IsList) {
						if(targetObject != null && mi.GetValue(targetObject) != null) {
							ServerPermissionRequest backReferenceRequest = new ServerPermissionRequest(mi.AssociatedMemberInfo.Owner.Type, mi.GetValue(targetObject), mi.AssociatedMemberInfo.Name, SecurityOperations.Write, expressionEvaluatorProvider);
							if(!IsGrantedCore(backReferenceRequest, permissionDictionary, false)) {
								return false;
							}
						}
					}
					else if(mi.IsManyToMany) {
						bool result;
						if(TryGetCanDeleteObjectWithManyToManyCollection(targetObject, out result)) {
							return result;
						}
					}
				}
			}
			return true;
		}
		protected virtual bool IsMemberGrantedSimplePath(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, Type targetType, bool needToCheckBackReference) {
			return IsMemberGrantedSimplePath(permissionRequest, memberPathList, targetType, needToCheckBackReference, permissions);
		}
		private bool IsMemberGrantedSimplePath(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, Type targetType, bool needToCheckBackReference, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary == null ) {
				permissionDictionary = permissionRequest.PermissionDictionary ?? permissions;
			}
			Guard.ArgumentNotNull(permissionDictionary, "permissionDictionary");
			if(!IsMemberOperation(permissionRequest.Operation)) {
				return false;
			}
			Guard.ArgumentNotNull(permissionRequest, "permissionRequest");
			Guard.ArgumentNotNull(memberPathList, "memberPathList");
			Guard.ArgumentNotNull(targetType, "targetType");
			bool isMemberGranted = false;
			IMemberInfo mi = memberPathList[0];
			if(!mi.Owner.Type.IsAssignableFrom(targetType)) {
				if(MemberIsUpCasted(mi, targetType)) {
					if(mi != null && permissionRequest.TargetObject != null && mi.Owner.Type != targetType.GetType()) {
						return true;
					}
					else {
						isMemberGranted = IsOperationGrantedByTypePermissions(mi.Owner.Type, permissionRequest.Operation, permissionDictionary);
						ServerPermissionRequest request = new ServerPermissionRequest(mi.Owner.Type, permissionRequest.TargetObject, mi.Name, permissionRequest.Operation, permissionRequest.ExpressionEvaluatorProvider);
						return IsGrantedCore(request, permissionDictionary, false);
					}
				}
				else {
					throw new ArgumentException(targetType.FullName + ", " + mi.Owner.Type.FullName, "targetType, mi.Owner.Type");
				}
			}
			if(!isMemberGranted) {
				isMemberGranted = IsOperationGrantedByMemberPermissions(targetType, mi.Name, permissionRequest.Operation, permissionDictionary);
				if(!isMemberGranted) {
					if(permissionRequest.TargetObject != null) {
						isMemberGranted = IsOperationGrantedByObjectPermissions(permissionRequest.TargetObject, permissionRequest.ExpressionEvaluatorProvider, permissionRequest.Operation, permissionDictionary);
						if(!isMemberGranted) {
							isMemberGranted = IsOperationGrantedByMemberCriteriaPermissions(permissionRequest.TargetObject, mi.Name, permissionRequest.ExpressionEvaluatorProvider, permissionRequest.Operation, permissionDictionary);
						}
					}
					else {
						isMemberGranted = IsAnyObjectPermissionGranted(targetType, permissionRequest.Operation, true, permissionDictionary);
						if(!isMemberGranted) {
							isMemberGranted = IsAnyMemberCriteriaPermissionGranted(targetType, mi.Name, permissionRequest.Operation, permissionDictionary);
						}
					}
				}
			}
			if(isMemberGranted) {
				ServerPermissionRequest subRequest = null;
				if(TryCreateReferenceMemberSubRequest(permissionRequest, memberPathList, needToCheckBackReference, out subRequest)) {
					return IsGrantedCore(subRequest, permissionDictionary, false);
				}
			}
			return isMemberGranted;
		}
		protected virtual bool IsMemberGrantedComplexPath(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, Type targetType) {
			return IsMemberGrantedComplexPath(permissionRequest, memberPathList, targetType, permissions);
		}
		protected virtual bool IsMemberGrantedComplexPath(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, Type targetType, IPermissionDictionary permissionDictionary) {
			if(permissionDictionary == null) {
				permissionDictionary = permissionRequest.PermissionDictionary ?? permissions;
			}
			object currentObject = null;
			Type currentObjectType;
			currentObject = permissionRequest.TargetObject;
			currentObjectType = permissionRequest.ObjectType;
			if(currentObjectType == null) {
				currentObjectType = memberPathList[0].Owner.Type;
			}
			for(int i = 0; i < memberPathList.Count; i++) {
				IMemberInfo mi = memberPathList[i];
				Type typeToProcess = currentObjectType;
				string operation = (i == (memberPathList.Count - 1)) ? permissionRequest.Operation : SecurityOperations.Read;
				if(currentObject != null && !(currentObject is XafDataViewRecord)) {
					typeToProcess = currentObject.GetType();
				}
				if(mi != null && MemberIsUpCasted(mi, typeToProcess)) {
					if(mi != null && currentObject != null && currentObject.GetType() != mi.Owner.Type) {
						return true;
					}
					else {
						typeToProcess = mi.Owner.Type;
					}
				}
				ServerPermissionRequest subRequest = new ServerPermissionRequest(typeToProcess, currentObject, mi.Name, operation, permissionRequest.ExpressionEvaluatorProvider, permissionDictionary);
				if(!IsGrantedCore(subRequest, permissionDictionary, false)) {
					return false;
				}
				if(currentObject != null) {
					currentObject = mi.GetValue(currentObject);
				}
				currentObjectType = mi.MemberType; 
			}
			return true;
		}
		protected virtual bool IsPersistentType(Type type) {
			Guard.ArgumentNotNull(type, "type");
			ITypeInfo typeInfo = FindTypeInfo(type);
			if(typeInfo == null) {
				throw new InvalidOperationException(String.Format(ServerSecurityLogLocalizer.Active.GetLocalizedString(ServerSecurityLogMessagesId.CannotFindTypeInfo, type.FullName)));
			}
			return typeInfo.IsPersistent;
		}
		protected virtual bool IsSecuredType(Type type) {
			return SecurityStrategy.IsSecuredType(type);
		}
		protected virtual Type GetTypeToProcess(Type targetType, object targetObject) {
			Type result;
			if(targetObject != null && !(targetObject is XafDataViewRecord)) {
				result = targetObject.GetType();
				if(!targetType.IsAssignableFrom(result)) {
					throw new InvalidCastException(ServerSecurityLogLocalizer.Active.GetLocalizedString(ServerSecurityLogMessagesId.IncompatibleTypes));
				}
			}
			else {
				result = targetType;
			}
			return GetTypeToProcess(result);
		}
		protected virtual bool TryGetReferenceMember(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, bool needToCheckBackReference, out ServerPermissionRequest request) {
			if(memberPathList != null && memberPathList.Count > 0) {
				IMemberInfo mi = memberPathList[0];
				if(!mi.IsAssociation) {
					if(!IsSecuredType(mi.MemberType)) {
						request = null;
						return false;
					}
					if(memberPathList.Count == 1 && (permissionRequest.Operation == SecurityOperations.Write)) {
						request = null;
						return false;
					}
					object targetObject = permissionRequest.TargetObject;
					object referencedObject = (targetObject != null) ? mi.GetValue(targetObject) : null;
					request = new ServerPermissionRequest(mi.MemberType,
							referencedObject, GetSubPath(memberPathList, 1), permissionRequest.Operation, permissionRequest.ExpressionEvaluatorProvider);
					return true;
				}
				else if(mi.IsAssociation && needToCheckBackReference && (permissionRequest.Operation == SecurityOperations.Write || permissionRequest.Operation == SecurityOperations.Read)) {
					object associatedObject = mi.IsList ? null : mi.GetValue(permissionRequest.TargetObject);
					request = new ServerPermissionRequest(mi.AssociatedMemberInfo.Owner.Type, associatedObject, mi.AssociatedMemberInfo.Name,
						permissionRequest.Operation, permissionRequest.ExpressionEvaluatorProvider);
					return true;
				}
			}
			request = null;
			return false;
		}
		protected virtual bool IsGrantedCore(ServerPermissionRequest permissionRequest, IPermissionDictionary permissions, bool needToCheckBackReference) {
			if(permissions == null) {
				permissions = this.permissions;
			}
			Guard.ArgumentNotNull(permissions, "permissions");
			if(permissionRequest.Operation == SecurityOperations.Navigate && !string.IsNullOrEmpty(permissionRequest.MemberName)) {
				throw new InvalidOperationException(ServerSecurityLogLocalizer.Active.GetLocalizedString(ServerSecurityLogMessagesId.MemberNavigate));
			}
			Type targetType = GetTypeToProcess(permissionRequest.ObjectType, permissionRequest.TargetObject);
			if(targetType == null) {
				throw new InvalidOperationException(ServerSecurityLogLocalizer.Active.GetLocalizedString(ServerSecurityLogMessagesId.CannotObtainType));
			}
			if(!IsSecuredType(targetType)) {
				return true;
			}
			IList<IMemberInfo> memberPathList;
			if(!TryExtractMemberPath(targetType, permissionRequest.MemberName, out memberPathList)) {
				return false;
			}
			if(memberPathList != null && memberPathList.Count == 1 && MemberIsUpCasted(memberPathList[0], targetType)) {
				if(permissionRequest.TargetObject != null && permissionRequest.TargetObject.GetType() != memberPathList[0].Owner.Type) {
					return true;
				}
				else {
					ServerPermissionRequest request = new ServerPermissionRequest(memberPathList[0].Owner.Type, permissionRequest.TargetObject, memberPathList[0].Name, permissionRequest.Operation, permissionRequest.ExpressionEvaluatorProvider);
					return IsGrantedCore(request, permissions, false);
				}
			}
			bool isOperationGrantedByTypePermissions;
			isOperationGrantedByTypePermissions = IsOperationGrantedByTypePermissions(targetType, permissionRequest.Operation, permissions);
			if(isOperationGrantedByTypePermissions) {
				if(permissionRequest.Operation == SecurityOperations.Delete && !GetIsDeleteGranted(permissionRequest.TargetObject, targetType, permissionRequest.ExpressionEvaluatorProvider, permissions)) {
					return false;
				}
				if(memberPathList != null) {
					if(memberPathList.Count < 2) {
						ServerPermissionRequest subRequest = null;						
						if(TryCreateReferenceMemberSubRequest(permissionRequest, memberPathList, needToCheckBackReference, out subRequest)) {
							return IsGrantedCore(subRequest, permissions, false);
						}
					}
					else {
						return IsMemberGrantedComplexPath(permissionRequest, memberPathList, targetType, permissions);
					}
				}
				return true;
			}
			else if(memberPathList != null) {
				if((permissionRequest.Operation != SecurityOperations.Read) && (permissionRequest.Operation != SecurityOperations.Write)) {
					throw new ArgumentException(permissionRequest.Operation + ", " + permissionRequest.MemberName);
				}
				if(memberPathList.Count == 1) {
					return IsMemberGrantedSimplePath(permissionRequest, memberPathList, targetType, needToCheckBackReference, permissions);
				}
				return IsMemberGrantedComplexPath(permissionRequest, memberPathList, targetType, permissions);
			}
			else if((memberPathList == null) && (permissionRequest.TargetObject != null)) {
				if(IsOperationGrantedByObjectPermissions(permissionRequest.TargetObject, permissionRequest.ExpressionEvaluatorProvider, permissionRequest.Operation, permissions)) {
					return true;
				}
				if(IsAnyMemberPermissionGranted(targetType, permissionRequest.Operation, permissions)) {
					return true;
				}
				return IsOperationGrantedByMemberCriteriaPermissions(permissionRequest.TargetObject, null, permissionRequest.ExpressionEvaluatorProvider, permissionRequest.Operation, permissions);
			}
			else if((memberPathList == null) && (permissionRequest.TargetObject == null)) {
				if(permissionRequest.Operation == SecurityOperations.Navigate) {
					return false;
				}
				if(IsAnyMemberPermissionGranted(targetType, permissionRequest.Operation, permissions)
					|| IsAnyObjectPermissionGranted(targetType, permissionRequest.Operation, true, permissions)
					|| IsAnyMemberCriteriaPermissionGranted(targetType, permissionRequest.Operation, permissions)
					) {
					return true;
				}
				return false;
			}
			else {
				throw new ArgumentException();
			}
		}
		public ServerPermissionRequestProcessor() {
		}
		public ServerPermissionRequestProcessor(IPermissionDictionary permissions) {
			this.permissions = permissions;
		}
		#region IPermissionRequestProcessor
		public bool IsGranted(ServerPermissionRequest permissionRequest) {
			bool cachedResult = false;
			if(CanUseCache && requestGrantedCache.TryGetValue(permissionRequest, out cachedResult)) {
				return cachedResult;
			}
			bool result;
			result = IsGrantedCore(permissionRequest, permissionRequest.PermissionDictionary, true);
			if(CanUseCache) {
				requestGrantedCache.Set(permissionRequest, result);
			}
			return result;
		}
		bool IPermissionRequestProcessor.IsGranted(IPermissionRequest permissionRequest) {
			return IsGranted((ServerPermissionRequest)permissionRequest);
		}
		#endregion
		#region ISecurityCriteriaProvider2
		protected virtual IList<string> GetSelectMemberCriteriaCore(Type type, IPermissionDictionary permissions, string memberName) {
			Guard.ArgumentNotNullOrEmpty(memberName, "memberName");
			Guard.ArgumentNotNull(type, "type");
			type = GetTypeToProcess(type);
			Type targetType = GetTypeToProcess(type);
			if(targetType == null) {
				throw new ArgumentException(type.FullName, "type");
			}
			if(!IsSecuredType(type)) {
				return new string[0];
			}
			IList<IMemberInfo> memberPathList;
			if(!TryExtractMemberPath(type, memberName, out memberPathList)) {
				return new string[0];
			}
			if(memberPathList.Count != 1) {
				throw new ArgumentException(memberName, "memberName");
			}
			IMemberInfo memberInfo = memberPathList[0];
			bool isOperationGrantedByTypePermissions;
			if(permissions == null) {
				isOperationGrantedByTypePermissions = IsOperationGrantedByTypePermissions(targetType, SecurityOperations.Read);
			}
			else {
				isOperationGrantedByTypePermissions = IsOperationGrantedByTypePermissions(targetType, SecurityOperations.Read, permissions);
			}
			if(isOperationGrantedByTypePermissions) {
				if(memberInfo.IsAssociation) {
					if(IsOperationGrantedByTypePermissions(memberInfo.MemberType, SecurityOperations.Read)) {
						return new string[0];
					}
					else
						if(IsAnyMemberPermissionGranted(memberInfo.MemberType, SecurityOperations.Read, permissions)
							|| IsAnyObjectPermissionGranted(memberInfo.MemberType, SecurityOperations.Read, true, permissions)
							|| IsAnyMemberCriteriaPermissionGranted(memberInfo.MemberType, SecurityOperations.Read, permissions)) {
							return new string[0];
						}
						else {
							return new string[] { FalseCriteria };
						}
				}
				else {
					return new string[0];
				}
			}
			else {
				if(IsOperationGrantedByMemberPermissions(targetType, memberInfo.Name, SecurityOperations.Read, permissions)) {
					return new string[0];
				}
				else {
					List<string> result = new List<string>();
					IEnumerable<MemberCriteriaOperationPermission> availableMemberCriteriaPermissions =
						permissions.GetPermissions<MemberCriteriaOperationPermission>(targetType, SecurityOperations.Read, CompareTypeMode.IsAssignable);
					foreach(MemberCriteriaOperationPermission p in availableMemberCriteriaPermissions) {
						if(Contains(p.MemberName, memberName)) {
							result.Add(p.Criteria);
						}
					}
					IEnumerable<ObjectOperationPermission> availableObjectPermissions =
						permissions.GetPermissions<ObjectOperationPermission>(targetType, SecurityOperations.Read, CompareTypeMode.IsAssignable);
					foreach(ObjectOperationPermission p in availableObjectPermissions) {
						result.Add(p.Criteria);
					}
					if(result.Count == 0) {
						result.Add(FalseCriteria);
					}
					return result;
				}
			}
		}
		protected virtual IList<string> GetSelectObjectCriteriaCore(Type type, IPermissionDictionary permissions) {
			List<string> result = new List<string>();
			type = GetTypeToProcess(type);
			if(!IsSecuredType(type)) {
				return new string[0];
			}
			if(IsOperationGrantedByTypePermissions(type, SecurityOperations.Read, permissions)
				|| IsAnyMemberPermissionGranted(type, SecurityOperations.Read, permissions)) {
				return new string[0];
			}
			foreach(ITypeInfo descendantType in XafTypesInfo.Instance.FindTypeInfo(type).Descendants) {
				GroupOperator criteria = new GroupOperator(GroupOperatorType.Or);
				if(IsOperationGrantedByTypePermissions(descendantType.Type, SecurityOperations.Read, permissions)
					|| IsAnyMemberPermissionGranted(descendantType.Type, SecurityOperations.Read, permissions)) {
					criteria.Operands.Add(CriteriaOperator.Parse("IsExactType(This,?)", descendantType.FullName, permissions));
				}
				if(criteria.Operands.Count > 0) {
					result.Add(criteria.ToString());
				}
			}
			foreach(ObjectOperationPermission p in permissions.GetPermissions<ObjectOperationPermission>(type, SecurityOperations.Read, CompareTypeMode.IsAssignable)) {
				result.Add(p.Criteria);
			}
			foreach(MemberCriteriaOperationPermission p in permissions.GetPermissions<MemberCriteriaOperationPermission>(type, SecurityOperations.Read, CompareTypeMode.IsAssignable)) {
				result.Add(p.Criteria);
			}
			if(result.Count == 0) {
				result.Add(FalseCriteria);
			}
			return result.ToArray();
		}
		public IList<string> GetObjectCriteria(Type type) {
			return GetSelectObjectCriteriaCore(type, permissions);
		}
		public IList<string> GetMemberCriteria(Type type, string memberName) {
			return GetSelectMemberCriteriaCore(type, permissions, memberName);
		}
		#endregion
		#region ISecurityCriteriaProvider3 Members
		IList<string> ISecurityCriteriaProvider3.GetObjectCriteria(Type type, IPermissionDictionary permissionDictionary) {
			return GetSelectObjectCriteriaCore(type, permissionDictionary);
		}
		IList<string> ISecurityCriteriaProvider3.GetMemberCriteria(Type type, string memberName, IPermissionDictionary permissionDictionary) {
			return GetSelectMemberCriteriaCore(type, permissionDictionary, memberName);
		}
		#endregion
		#region IReadPermissionVersionProvider
		public Dictionary<string, int> CompleteVersion(Dictionary<string, int> sourceVersion) {
			Guard.ArgumentNotNull(sourceVersion, "sourceVersion");
			List<Type> fullyGrantedTypes = new List<Type>();
			List<Type> partiallyGrantedTypes = new List<Type>();
			Dictionary<Type, HashSet<string>> grantedMembers = new Dictionary<Type, HashSet<string>>();
			foreach(MemberOperationPermission memberOperationPermission in permissions.GetPermissions<MemberOperationPermission>()) {
				if(memberOperationPermission.Operation == SecurityOperations.Read) {
					if(!sourceVersion.ContainsKey(memberOperationPermission.ObjectType.ToString())) {
						if(fullyGrantedTypes.Contains(memberOperationPermission.ObjectType)) {
							continue;
						}
						else if(partiallyGrantedTypes.Contains(memberOperationPermission.ObjectType)) {
							AddMember(grantedMembers, memberOperationPermission.ObjectType, memberOperationPermission.MemberName);
						}
						else {
							bool isFullyGranted = false;
							foreach(TypeOperationPermission typeOperationPermission in permissions.GetPermissions<TypeOperationPermission>(memberOperationPermission.ObjectType, SecurityOperations.Read)) {
								isFullyGranted = true;
								break;
							}
							if(isFullyGranted) {
								fullyGrantedTypes.Add(memberOperationPermission.ObjectType);
							}
							else {
								partiallyGrantedTypes.Add(memberOperationPermission.ObjectType);
								AddMember(grantedMembers, memberOperationPermission.ObjectType, memberOperationPermission.MemberName);
							}
						}
					}
				}
			}
			Dictionary<Type, string> criteriaGrantedTypes = new Dictionary<Type, string>();
			foreach(Type key in grantedMembers.Keys) {
				foreach(ObjectOperationPermission p in permissions.GetPermissions<ObjectOperationPermission>(key, SecurityOperations.Read)) {
					if(!string.IsNullOrEmpty(p.Criteria)) {
						string criteria;
						if(!criteriaGrantedTypes.TryGetValue(key, out criteria)) {
							criteria = "";
						}
						criteriaGrantedTypes[key] = criteria + p.Criteria;
					}
				}
			}
			Dictionary<string, int> result = new Dictionary<string, int>(sourceVersion);
			foreach(Type key in grantedMembers.Keys) {
				string criteria;
				if(criteriaGrantedTypes.TryGetValue(key, out criteria)) {
					result[key.ToString()] = criteria.GetHashCode();
				}
				else {
					result[key.ToString()] = grantedMembers[key].Count;
				}
			}
			return result;
		}
		public bool CompareReadPermissionVersion(List<string> targetTypes, Dictionary<string, int> initial, Dictionary<string, int> current) {
			foreach(string typeName in targetTypes) {
				int initialVersion;
				if(initial.TryGetValue(typeName, out initialVersion)) {
					int currentVersion;
					if(current.TryGetValue(typeName, out currentVersion)) {
						if(!IsCriteriaVersionNumber(initialVersion)) {
							if(initialVersion < currentVersion) {
								return false;
							}
						}
						else {
							return (initialVersion == currentVersion);
						}
					}
				}
				else {
					return false; 
				}
			}
			return true;
		}
		#endregion
	}
}

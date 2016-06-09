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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security.ClientServer;
namespace DevExpress.ExpressApp.Security {
	public class ServerPermissionRequestProcessorLogger : ServerPermissionRequestProcessor {
		#region constants
		public const int IsGrantedCoreCode = 600;
		public const int GetTargetTypeCode = 601;
		public const int IsTypeNonPersistentGrantedCode = 602;
		public const int TryExtractMemberPathListCode = 603;
		public const int IsTypeOperationGrantedCode = 604;
		public const int TryGetIsDeleteGrantedCode = 605;
		public const int GetIsMemberGrantedSimplePathCode = 606;
		public const int GetIsMemberGrantedComplexPathCode = 607;
		public const int IsObjectOperationGrantedCode = 608;
		public const int IsAnyMemberOperationGrantedCode = 609;
		public const int IsAnyObjectPermissionGrantedCode = 610;
		public const int IsMemberOperationGrantedCode = 611;
		public const int TryCreateReferenceMemberSubRequestCode = 612;
		#endregion
		private ILogger logger;
		private string GetRequestDescription(ServerPermissionRequest request) {
			StringBuilder description = new StringBuilder();
			description.Append(request.GetType().Name + " {");
			description.Append(String.Format("ObjectType = {0};", request.ObjectType == null ? "null" : request.ObjectType.ToString()));
			description.Append(String.Format("MemberName = {0};", request.MemberName == null ? "null" : request.MemberName));
			description.Append(String.Format("TargetObject = {0};", request.TargetObject == null ? "null" : request.TargetObject));
			description.Append(String.Format("Operation = {0};", request.Operation));
			description.Append("}");
			return description.ToString();
		}
		private string GetCollectionDescription(IList<IMemberInfo> collection) {
			if(collection == null) {
				return "null";
			}
			String result = String.Empty;
			foreach(IMemberInfo obj in collection) {
				if(obj != null) {
					result += obj.ToString();
					if(obj != collection.Last()) { result += "."; }
				}
			}
			return result;
		}
		public ServerPermissionRequestProcessorLogger(IPermissionDictionary permissions, ILogger logger) : base(permissions) {
			Guard.ArgumentNotNull(logger, "logger");
			this.logger = logger;
		}
		protected override bool IsGrantedCore(ServerPermissionRequest permissionRequest, IPermissionDictionary permissions, bool needToCheckBackReference) {
			SecurityLogHelper.ReportOperationBegin("IsGrantedCore", logger, new Arg("permissionRequest", permissionRequest));
			bool result = false;
			try {
				result = base.IsGrantedCore(permissionRequest, permissions, needToCheckBackReference);
				SecurityLogHelper.ReportOperationEnd("IsGrantedCore", result, logger);
			}
			catch(Exception e) {
				logger.Log(Tracing.FormatExceptionReportDefault(e), LogLevel.Error, 600);
				throw;
			}
			return result;
		}
		protected override Type GetTypeToProcess(Type sourceType, object targetObject) {
			SecurityLogHelper.ReportOperationBegin("GetTypeToProcess", logger, new Arg("sourceType", sourceType), new Arg("targetObject", targetObject));
			Type result = base.GetTypeToProcess(sourceType, targetObject);
			SecurityLogHelper.ReportOperationEnd("GetTypeToProcess", result, logger);
			return result;
		}
		protected override bool TryExtractMemberPath(Type targetType, string memberName, out IList<IMemberInfo> memberPath) {
			SecurityLogHelper.ReportOperationBegin("TryExtractMemberPath", logger, new Arg("memberName", memberName));
			bool result = base.TryExtractMemberPath(targetType, memberName, out memberPath);
			SecurityLogHelper.ReportOperationEnd("TryExtractMemberPath", result, "memberPath = " + GetCollectionDescription(memberPath), logger);
			return result;
		}
		protected override bool IsOperationGrantedByTypePermissions(Type targetObjectType, string operation) {
			SecurityLogHelper.ReportOperationBegin("IsTypeOperationGranted", logger, new Arg("targetObjectType", targetObjectType), new Arg("operation", operation));
			bool result = base.IsOperationGrantedByTypePermissions(targetObjectType, operation);
			SecurityLogHelper.ReportOperationEnd("IsTypeOperationGranted", result, logger);
			return result;
		}
		protected override bool  GetIsDeleteGranted(object targetObject, Type type, ISecurityExpressionEvaluator expressionEvaluatorProvider) {
			SecurityLogHelper.ReportOperationBegin("GetIsDeleteGranted", logger, new Arg("targetObject", targetObject), new Arg("type", type));
			bool result = base.GetIsDeleteGranted(targetObject, type, expressionEvaluatorProvider);
			SecurityLogHelper.ReportOperationEnd("GetIsDeleteGranted", result, logger);
			return result;
		}
		protected override bool TryCreateReferenceMemberSubRequest(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, bool needToCheckBackReference, out ServerPermissionRequest request) {
			SecurityLogHelper.ReportOperationBegin("TryCreateReferenceMemberSubRequest", logger, new Arg("permissionRequest", permissionRequest), new Arg("memberPathList", GetCollectionDescription(memberPathList)), new Arg("needToCheckBackReference", needToCheckBackReference));
			bool result = base.TryCreateReferenceMemberSubRequest(permissionRequest, memberPathList, needToCheckBackReference, out request);
			SecurityLogHelper.ReportOperationEnd("TryCreateReferenceMemberSubRequest", result, "subRequest = " + (ReferenceEquals(request, null) ? "null" : request.ToString()), logger);
			return result;
		}
		protected override bool IsMemberGrantedSimplePath(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, Type targetType, bool needToCheckBackReference) {
			SecurityLogHelper.ReportOperationBegin("IsMemberGrantedSimplePath", logger, new Arg("memberPathList", GetCollectionDescription(memberPathList)), new Arg("targetType", targetType), new Arg("needToCheckBackReference", needToCheckBackReference));
			bool result = base.IsMemberGrantedSimplePath(permissionRequest, memberPathList, targetType, needToCheckBackReference);
			SecurityLogHelper.ReportOperationEnd("IsMemberGrantedSimplePath", result, logger);
			return result;
		}
		protected override bool IsMemberGrantedComplexPath(ServerPermissionRequest permissionRequest, IList<IMemberInfo> memberPathList, Type targetType) {
			SecurityLogHelper.ReportOperationBegin("IsMemberGrantedComplexPath", logger, new Arg("memberPathList", GetCollectionDescription(memberPathList)), new Arg("targetType", targetType));
			bool result = base.IsMemberGrantedComplexPath(permissionRequest, memberPathList, targetType);
			SecurityLogHelper.ReportOperationEnd("IsMemberGrantedComplexPath", result, logger);
			return result;
		}
		protected override bool IsOperationGrantedByObjectPermissions(object targetObject, ISecurityExpressionEvaluator expressionEvaluatorProvider, string operation) {
			SecurityLogHelper.ReportOperationBegin("IsObjectOperationGranted", logger, new Arg("targetObject", targetObject), new Arg("operation", operation));
			bool result = base.IsOperationGrantedByObjectPermissions(targetObject, expressionEvaluatorProvider, operation);
			SecurityLogHelper.ReportOperationEnd("IsObjectOperationGranted", result, logger);
			return result;
		}
		protected override bool IsAnyMemberPermissionGranted(Type targetObjectType, string operation) {
			SecurityLogHelper.ReportOperationBegin("IsAnyMemberOperationGranted", logger, new Arg("targetObjectType", targetObjectType), new Arg("operation", operation));
			bool result = base.IsAnyMemberPermissionGranted(targetObjectType, operation);
			SecurityLogHelper.ReportOperationEnd("IsAnyMemberOperationGranted", result, logger);
			return result;
		}
		protected override bool IsAnyObjectPermissionGranted(Type targetObjectType, string operation) {
			SecurityLogHelper.ReportOperationBegin("IsAnyObjectPermissionGranted", logger, new Arg("targetObjectType", targetObjectType), new Arg("operation", operation));
			bool result = base.IsAnyObjectPermissionGranted(targetObjectType, operation);
			SecurityLogHelper.ReportOperationEnd("IsAnyObjectPermissionGranted", result, logger);
			return result;
		}
		protected override bool IsOperationGrantedByMemberPermissions(Type targetObjectType, string memberName, string operation) {
			SecurityLogHelper.ReportOperationBegin("IsMemberOperationGranted", logger, new Arg("targetObjectType", targetObjectType), new Arg("memberName", memberName), new Arg("operation", operation));
			bool result = base.IsOperationGrantedByMemberPermissions(targetObjectType, memberName, operation);
			SecurityLogHelper.ReportOperationEnd("IsMemberOperationGranted", result, logger);
			return result;
		}
		protected override bool IsPersistentType(Type type) {
			SecurityLogHelper.ReportOperationBegin("IsPersistentType", logger, new Arg("type", type));
			bool result = base.IsPersistentType(type);
			SecurityLogHelper.ReportOperationEnd("IsPersistentType", result, logger);
			return result;
		}
		protected override bool IsSecuredType(Type type) {
			SecurityLogHelper.ReportOperationBegin("IsSecuredType", logger, new Arg("type", type));
			bool result = base.IsSecuredType(type);
			SecurityLogHelper.ReportOperationEnd("IsSecuredType", result, logger);
			return result;
		}
	}
}

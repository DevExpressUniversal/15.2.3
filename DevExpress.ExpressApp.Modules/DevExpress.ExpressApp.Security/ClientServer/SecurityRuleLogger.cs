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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Diagnostics;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security.ClientServer;
namespace DevExpress.ExpressApp.Security {
	internal class Arg {
		private object value;
		public Arg(string argName, object argValue) {
			Name = argName;
			Value = argValue;
		}
		public string Name { get; set; }
		public object Value {
			get { return value == null ? "null" : value; }
			set { this.value = value; }
		}
	}
	public class FilterLogger : ILogger {
		private LogLevel level;
		private ILogger logger;
		public FilterLogger(LogLevel level, ILogger logger) {
			this.level = level;
			this.logger = logger;
		}
		public void Log(string message, LogLevel level, int messageId) {
			if(level >= this.level) {
				logger.Log(message, level, messageId);
			}
		}
	}
	public class SecurityRuleLogger : SecurityRule {
		private ILogger logger;
		private DelayedPermissionsProvider delayedPermissionsListProvider;
		public SecurityRuleLogger(XPDictionary dic, ISelectDataSecurity security, ILogger logger) : this(dic,security,logger,null) { }
		public SecurityRuleLogger(XPDictionary dic, ISelectDataSecurity security, ILogger logger, DelayedPermissionsProvider delayedPermissionsListProvider)
			: base(dic, security, delayedPermissionsListProvider) {
			this.logger = logger;
			this.delayedPermissionsListProvider = delayedPermissionsListProvider;
		}
		public override bool GetSelectFilterCriteria(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, out CriteriaOperator criteria) {
			bool result = false;
			try {
				SecurityLogHelper.ReportOperationBegin("GetSelectFilterCriteria", logger, new Arg("classInfo", classInfo));
				result = base.GetSelectFilterCriteria(context, classInfo, out criteria);
				SecurityLogHelper.ReportOperationEnd("GetSelectFilterCriteria", result, ReferenceEquals(criteria, null) ? null : "criteria = " + criteria.ToString(), logger);
			}
			catch(Exception e) {
				logger.Log(Tracing.FormatExceptionReportDefault(e), LogLevel.Error, 0);
				throw;
			}
			return result;
		}
		public override bool GetSelectMemberExpression(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, XPMemberInfo memberInfo, out CriteriaOperator expression) {
			bool result = false;
			try {
				SecurityLogHelper.ReportOperationBegin("GetSelectMemberExpression", logger, new Arg("classInfo", classInfo), new Arg("memberInfo", memberInfo));
				result = base.GetSelectMemberExpression(context, classInfo, memberInfo, out expression);
				SecurityLogHelper.ReportOperationEnd("GetSelectMemberExpression", result, ReferenceEquals(expression, null) ? null : "expression = " + expression.ToString(), logger);
			}
			catch(Exception e) {
				logger.Log(Tracing.FormatExceptionReportDefault(e), LogLevel.Error, 0);
				throw;
			}
			return result;
		}
		internal override bool IsGranted(DevExpress.Xpo.SecurityContext context, object targetObject, string memberName, string operation, bool isParentSession) {
			SecurityLogHelper.ReportOperationBegin("IsGranted", logger, new Arg("targetObject", targetObject), new Arg("memberName", memberName), new Arg("operation", operation), new Arg("isParentSession", isParentSession));
			bool result = base.IsGranted(context, targetObject, memberName, operation, isParentSession);
			SecurityLogHelper.ReportOperationEnd("IsGranted", result, logger);
			return result;
		}
		internal override bool IsIntermediateObject(object theObject, out object leftObject, out object rightObject) {
			SecurityLogHelper.ReportOperationBegin("IsIntermediateObject", logger, new Arg("theObject", theObject));
			bool result = base.IsIntermediateObject(theObject, out leftObject, out rightObject);
			string additionalData = null;
			if(result) {
				additionalData = "leftObject: " + leftObject != null ? leftObject.ToString() : "null; rightObject: " + rightObject != null ? rightObject.ToString() : "null";
			}
			SecurityLogHelper.ReportOperationEnd("IsIntermediateObject", result, additionalData, logger);
			return result;
		}
		internal override bool TryGetIsGrantedForIntermediateObject(DevExpress.Xpo.SecurityContext context, object targetObject, string operation, bool isParentSession, out bool isGranted) {
			SecurityLogHelper.ReportOperationBegin("TryGetIsGrantedForIntermediateObject", logger, new Arg("targetObject", targetObject), new Arg("operation", operation), new Arg("isParentSession", isParentSession));
			bool result = base.TryGetIsGrantedForIntermediateObject(context, targetObject, operation, isParentSession, out isGranted);
			SecurityLogHelper.ReportOperationEnd("TryGetIsGrantedForIntermediateObject", result, "isGranted = " + isGranted, logger);
			return result;
		}
		internal override bool TryGetIsGrantedForSharedPart(DevExpress.Xpo.SecurityContext context, object targetObject, string memberName, string operation, bool isParentSession, out bool isGranted) {
			SecurityLogHelper.ReportOperationBegin("TryGetIsGrantedForSharedPart", logger, new Arg("targetObject", targetObject), new Arg("memberName", memberName), new Arg("operation", operation), new Arg("isParentSession", isParentSession));
			bool result = base.TryGetIsGrantedForSharedPart(context, targetObject, memberName, operation, isParentSession, out isGranted);
			SecurityLogHelper.ReportOperationEnd("TryGetIsGrantedForSharedPart", result, "isGranted = " + isGranted, logger);
			return result;
		}
		public override ValidateMemberOnSaveResult ValidateMemberOnSave(DevExpress.Xpo.SecurityContext context, XPMemberInfo memberInfo, object theObject, object realObjectOnLoad, object value, object valueOnLoad, object realValueOnLoad) {
			ValidateMemberOnSaveResult result = ValidateMemberOnSaveResult.DoRaiseException;
			try {
				SecurityLogHelper.ReportOperationBegin("ValidateMemberOnSave", logger, new Arg("memberInfo", memberInfo), new Arg("theObject", theObject), new Arg("realObjectOnLoad", realObjectOnLoad), new Arg("value", value), new Arg("valueOnLoad", valueOnLoad), new Arg("realValueOnLoad", realObjectOnLoad));
				result = base.ValidateMemberOnSave(context, memberInfo, theObject, realObjectOnLoad, value, valueOnLoad, realValueOnLoad);
				SecurityLogHelper.ReportOperationEnd("ValidateMemberOnSave", result, logger);
			}
			catch(Exception e) {
				logger.Log(Tracing.FormatExceptionReportDefault(e), LogLevel.Error, 0);
				throw;
			}
			return result;
		}
		public override bool ValidateObjectOnDelete(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object theObject, object realObjectOnLoad) {
			bool result = false;
			try {
				SecurityLogHelper.ReportOperationBegin("ValidateObjectOnDelete", logger, new Arg("classInfo", classInfo), new Arg("object", theObject), new Arg("realObjectOnLoad", realObjectOnLoad));
				result = base.ValidateObjectOnDelete(context, classInfo, theObject, realObjectOnLoad);
				SecurityLogHelper.ReportOperationEnd("ValidateObjectOnDelete", result, logger);
			}
			catch(Exception e) {
				logger.Log(Tracing.FormatExceptionReportDefault(e), LogLevel.Error, 0);
				throw;
			}
			return result;
		}
		public override bool ValidateObjectOnSave(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object theObject, object realObjectOnLoad) {
			bool result = false;
			try {
				SecurityLogHelper.ReportOperationBegin("ValidateObjectOnSave", logger, new Arg("classInfo", classInfo), new Arg("object", theObject), new Arg("realObjectOnLoad", realObjectOnLoad));
				result = base.ValidateObjectOnSave(context, classInfo, theObject, realObjectOnLoad);
				SecurityLogHelper.ReportOperationEnd("ValidateObjectOnSave", result, logger);
			}
			catch(Exception e) {
				logger.Log(Tracing.FormatExceptionReportDefault(e), LogLevel.Error, 0);
				throw;
			}
			return result;
		}
		public override bool ValidateObjectOnSelect(DevExpress.Xpo.SecurityContext context, XPClassInfo classInfo, object realObjectOnLoad) {
			bool result = false;
			try {
				SecurityLogHelper.ReportOperationBegin("ValidateObjectOnSelect", logger, new Arg("classInfo", classInfo), new Arg("realObjectOnLoad", realObjectOnLoad));
				result = base.ValidateObjectOnSelect(context, classInfo, realObjectOnLoad);
				SecurityLogHelper.ReportOperationEnd("ValidateObjectOnSelect", result, logger);
			}
			catch(Exception e) {
				logger.Log(Tracing.FormatExceptionReportDefault(e), LogLevel.Error, 0);
				throw;
			}
			return result;
		}
		internal override bool IsGranted(DevExpress.Xpo.SecurityContext context, object theObject, object realObjectOnLoad, string memberName, string operation) {
			SecurityLogHelper.ReportOperationBegin("IsGranted", logger, new Arg("object", theObject), new Arg("realObjectOnLoad", realObjectOnLoad), new Arg("memberName", memberName), new Arg("operation", operation));
			bool result = base.IsGranted(context, theObject, realObjectOnLoad, memberName, operation);
			SecurityLogHelper.ReportOperationEnd("IsGranted", result, logger);
			return result;
		}
		internal override bool IsSharedPartType(Type type) {
			SecurityLogHelper.ReportOperationBegin("IsSharedPartType", logger, new Arg("type", type));
			bool result = base.IsSharedPartType(type);
			SecurityLogHelper.ReportOperationEnd("IsSharedPartType", result, logger);
			return result;
		}
		internal override bool IsIntermediateObjectType(Type type) {
			SecurityLogHelper.ReportOperationBegin("IsIntermediateObjectType", logger, new Arg("type", type));
			bool result = base.IsIntermediateObjectType(type);
			SecurityLogHelper.ReportOperationEnd("IsIntermediateObjectType", result, logger);
			return result;
		}
		internal override bool TryGetIsGrantedForIntermediateObject(DevExpress.Xpo.SecurityContext context, object theObject, object realObjectOnLoad, string operation, out bool isGranted) {
			SecurityLogHelper.ReportOperationBegin("TryGetIsGrantedForIntermediateObject", logger, new Arg("object", theObject), new Arg("realObjectOnLoad", realObjectOnLoad), new Arg("operation", operation));
			bool result = base.TryGetIsGrantedForIntermediateObject(context, theObject, realObjectOnLoad, operation, out isGranted);
			SecurityLogHelper.ReportOperationEnd("TryGetIsGrantedForIntermediateObject", result, "isGranted = " + isGranted, logger);
			return result;
		}
	}  
}

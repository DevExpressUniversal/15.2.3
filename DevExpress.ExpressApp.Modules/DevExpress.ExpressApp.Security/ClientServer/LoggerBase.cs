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
using DevExpress.Utils;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public abstract class LoggerBase  {
		private readonly ILogger logger;
		private string GetDisplayText(IClientInfo clientInfo) {
			string logonParameterDisplayText = "null";
			if(clientInfo.LogonParameters != null)
				logonParameterDisplayText = clientInfo.LogonParameters.ToString();
			return string.Format("ClientId: '{0}'; WorkspaceId: '{1}'; LogonParameters: '{2}'", clientInfo.ClientId.ToString(), clientInfo.WorkspaceId.ToString(), logonParameterDisplayText);
		}
		private void Log(string message, LogLevel level, int messageId, IClientInfo clientInfo) {
			string clientInfoText = clientInfo.ToString();
			message += Environment.NewLine + GetDisplayText(clientInfo);
			logger.Log(message, level, messageId);
		}
		protected T Execute<T>(OperationResultPredicate<T> predicate, string operationName, int operationId, IClientInfo clientInfo) {
			return Execute<T>(predicate, operationName, operationId, clientInfo, null);
		}
		protected T Execute<T>(OperationResultPredicate<T> predicate, string operationName, int operationId, IClientInfo clientInfo, string additionalData) {
			Log(string.Format(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.OperationRequest), operationName), LogLevel.Info, operationId, clientInfo);
			try {
				T result = predicate();
				string currentUserId = (SecuritySystem.Instance != null && SecuritySystem.Instance.UserId != null) ? SecuritySystem.Instance.UserId.ToString() : "null";
				string currentUserName = (SecuritySystem.Instance != null && SecuritySystem.Instance.UserName != null) ? SecuritySystem.Instance.UserName : "null";
				string operationCompletedMessage = String.Format(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.OperationCompleted), operationName, currentUserId, currentUserName); 
				if(result != null) {
					operationCompletedMessage += Environment.NewLine + String.Format(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.OperationResultTemplate, result.ToString()));
				}
				Log(operationCompletedMessage, LogLevel.Info, operationId, clientInfo);
				return result;
			}
			catch(Exception ex) {
				StringBuilder errorMessage = new StringBuilder();
				errorMessage.AppendLine(String.Format(ServerDataLogLocalizer.Active.GetLocalizedString(ServerLogMessagesId.OperationFailed), operationName, DevExpress.Persistent.Base.Tracing.FormatExceptionReportDefault(ex)));
				if(!string.IsNullOrEmpty(additionalData)) {
					errorMessage.AppendLine(additionalData);
				}
				Log(errorMessage.ToString(), LogLevel.Error, operationId, clientInfo);
				throw;
			}
		}
		public LoggerBase(ILogger logger) {
			Guard.ArgumentNotNull(logger, "logger");
			this.logger = logger;
		}
	}
}

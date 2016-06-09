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
namespace DevExpress.ExpressApp.Security.ClientServer {
	internal static class SecurityLogHelper {
		private static int currentIndent;
		internal static void ReportOperationBegin(string operationName, ILogger logger, params Arg[] args) {
			StringBuilder message = new StringBuilder();
			message.Append(String.Format(ServerSecurityLogLocalizer.Active.GetLocalizedString(ServerSecurityLogMessagesId.OperationStarted), operationName));
			message.Append(' ');
			if(args != null) {
				foreach(Arg arg in args) {
					message.Append(arg.Name + " = " + arg.Value + "; ");
				}
			}
			logger.Log(message.ToString(), LogLevel.Info, 600);
			currentIndent++;
		}
		internal static void ReportOperationEnd(string operationName, object result, string additionalInfo, ILogger logger) {
			string resultString = result != null ? result.ToString() : "null";
			if(String.IsNullOrEmpty(resultString)) resultString = "null";
			string message = String.Format(ServerSecurityLogLocalizer.Active.GetLocalizedString(ServerSecurityLogMessagesId.Result), operationName, resultString);
			if(!String.IsNullOrEmpty(additionalInfo)) {
				message += "; " + additionalInfo;
			}
			currentIndent--;
			logger.Log(message, LogLevel.Info, 600);
		}
		internal static void ReportOperationEnd(string operationName, object result, ILogger logger) {
			ReportOperationEnd(operationName, result, null, logger);
		}
	}
}

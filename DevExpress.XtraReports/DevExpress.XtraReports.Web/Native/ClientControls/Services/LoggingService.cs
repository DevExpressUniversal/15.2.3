#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Diagnostics;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Web.Native.ClientControls.Services {
	class LoggingService : ILoggingService {
		readonly string prefix;
		readonly TraceSwitch traceSwitch;
		public LoggingService(string prefix) {
			Guard.ArgumentIsNotNullOrEmpty(prefix, "prefix");
			traceSwitch = new TraceSwitch("DevExpress.XtraReports.Web", string.Empty);
			this.prefix = prefix;
		}
		public void Info(string message) {
			if(traceSwitch.TraceInfo) {
				message = FormatMessage(message);
				Trace.WriteLineIf(traceSwitch.TraceInfo, message);
			}
		}
		public void Error(string message) {
			if(traceSwitch.TraceError) {
				message = FormatMessage(message, true);
				Trace.WriteLineIf(traceSwitch.TraceError, message);
			}
		}
		string FormatMessage(string message, bool isError = false) {
			var errorIndicator = isError ? " ERROR!" : string.Empty;
			return string.Format("[{0:HH:mm:ss.fff}{1}] {2} - {3}", DateTime.Now, errorIndicator, prefix, message);
		}
	}
}

#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Diagnostics;
using System.Globalization;
namespace DevExpress.XtraReports.Design {
	public interface IOutputService {
		void OutputString(string text);
	}
	public class DesignTraceListener : TraceListener {
		IOutputService serv;
		public DesignTraceListener(IOutputService serv, string name) : base(name) {
			this.serv = serv;
		}
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data) {
			if(this.Filter == null || this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null)) {
				WriteHeader(eventType);
				string message = string.Empty;
				if(data != null) {
					message = data.ToString();
				}
				WriteLine(message);
			}
		}
		void WriteHeader(TraceEventType eventType) {
			this.Write(string.Format(CultureInfo.InvariantCulture, "{0}: ", eventType));
		}
		public override void Write(string message) {
			serv.OutputString(message);
		}
		public override void WriteLine(string message) {
			serv.OutputString(message + "\r\n");
		}
	}
}

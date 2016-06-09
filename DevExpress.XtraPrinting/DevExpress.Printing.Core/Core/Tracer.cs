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
using System.Text;
using System.Diagnostics;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Helpers;
using System.Security.Permissions;
namespace DevExpress.XtraPrinting {
	public static class Tracer {
#if SL
		public static void TraceError(string traceSource, object data) {
		}
#else
		[ThreadStatic]
		static Dictionary<string, TraceSource> traceSources;
		static Dictionary<string, TraceSource> TraceSources {
			get {
				if(traceSources == null)
					traceSources = new Dictionary<string, TraceSource>();
				return traceSources;
			}
		}
		public static TraceSource GetSource(string traceSource) {
			TraceSource ts;
			if(!TraceSources.TryGetValue(traceSource, out ts)) {
				ts = new TraceSource(traceSource, SourceLevels.Off);
				if(SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)))
					RemoveListener(ts, "Default");
				TraceSources[traceSource] = ts;
			}
			return ts;
		}
		public static TraceSource GetSource(string traceSource, SourceLevels mandatoryLevel) {
			TraceSource ts = GetSource(traceSource);
			if((ts.Switch.Level & mandatoryLevel) != mandatoryLevel)
				ts.Switch.Level |= mandatoryLevel;
			return ts;
		}
		static void RemoveListener(TraceSource ts, string name) {
			ts.Listeners.Remove(name);
		}
		public static void TraceWarning(string traceSource, object data) {
			TraceData(traceSource, TraceEventType.Warning, data);
		}
		public static void TraceInformation(string traceSource, object data) {
			TraceData(traceSource, TraceEventType.Information, data);
		}
		[Conditional("DEBUG")]
		public static void TraceInformationTest(string traceSource, object data) {
			TraceData(traceSource, TraceEventType.Information, data);
		}
		public static void TraceError(string traceSource, object data) {
			TraceData(traceSource, TraceEventType.Error, data);
		}
		public static void TraceData(string traceSource, TraceEventType eventType, object data) {
			TraceSource ts;
			if(traceSources != null && traceSources.TryGetValue(traceSource, out ts))
				ts.TraceData(eventType, 0, data);
		}
		public static void TraceData(string traceSource, TraceEventType eventType, Func<object> getData) {
			TraceSource ts;
			if(traceSources != null && traceSources.TryGetValue(traceSource, out ts)) {
				object data = getData();
				if(data != null)
					ts.TraceData(eventType, 0, data);
			}
		}
#endif
	}
}

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
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Native {
	using System.Text;
	using System.Diagnostics;
	using System.IO;
	using System.Collections.Concurrent;
	using DevExpress.XtraReports.UI;
	static class XRControlExtensions {
		static ConcurrentDictionary<string, Type> types = new ConcurrentDictionary<string, Type>();
		public static Type GetObjectType(string typeName) {
			return types.GetOrAdd(typeName, x => x.Contains(",") ? TypeResolver.GetType(x) : Type.GetType(string.Format("{0}.{1}", typeof(XtraReport).Namespace, x)));
		}
		internal static string GetTraceName(this XRControl control) {
			return !string.IsNullOrEmpty(control.Name) ? control.Name : control.GetType().Name;
		}
		internal static void SaveXmlToStream(this XRControl control, Stream stream) {
			XRControlXmlSerializer serializer = new XRControlXmlSerializer();
			serializer.SerializeRootObject(control, stream);
		}
		internal static void LoadFromXml(this XRControl control, Stream stream) {
			XRControlXmlSerializer serializer = new XRControlXmlSerializer();
			serializer.DeserializeObject(control, stream, string.Empty);
		}
	}
	static class TraceSR {
		public const string
			Format_InvalidPropertyValue = "The {0} property has an invalid value",
			Format_InvalidBinding = "The {0} binding is not valid",
			MessageSeparator = " ---> ",
			DocumentCreation = "Document creation";
	}
	static class TraceCenter {
		[ThreadStatic]
		static HashSet<MultiKey> tracedErrors;
		static HashSet<MultiKey> TracedErrors {
			get {
				if(tracedErrors == null)
					tracedErrors = new HashSet<MultiKey>();
				return tracedErrors;
			}
		}
		public static void ClearHistory() {
			TracedErrors.Clear();
		}
		public static void TraceErrorOnce(XRControl control, string format, object arg) {
			Tracer.TraceData(NativeSR.TraceSource, TraceEventType.Error, () => {
				MultiKey key = new MultiKey(control, format, arg);
				if(!TracedErrors.Contains(key)) {
					TracedErrors.Add(key);
					return BuildData(control, format, arg);
				}
				return null;
			});
		}
		public static void TraceError(XRControl control, string format, object arg) {
			Tracer.TraceData(NativeSR.TraceSource, TraceEventType.Error, BuildData(control, format, arg));
		}
		static object BuildData(XRControl control, string format, object arg) {
			StringBuilder builder = new StringBuilder(500);
			BuildPath(control, control.RealControl.Band, builder);
			return builder.ToString() + TraceSR.MessageSeparator + string.Format(format, arg);
		}
		static void BuildPath(XRControl src, XRControl dest, StringBuilder builder) {
			builder.Insert(0, "\\" + src.GetTraceName());
			if(!ReferenceEquals(src, dest))
				BuildPath(src.RealControl.Parent, dest, builder);
		}
	}
}

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

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Web;
using InternalUtils = DevExpress.Web.Mvc.Internal.Utils;
namespace DevExpress.Web.Mvc {
	[ToolboxItem(false)]
	public class MVCxDocumentViewer : ASPxDocumentViewer {
		public object CallbackRouteValues { get; set; }
		public object ExportRouteValues { get; set; }
		public new MVCxDocumentViewerClientSideEvents ClientSideEvents {
			get { return (MVCxDocumentViewerClientSideEvents)base.ClientSideEventsInternal; }
		}
		public override bool IsCallback {
			get { return base.IsCallback || MvcUtils.IsCallback(); }
		}
		public MVCxDocumentViewer() { }
		internal void PrepareControl() {
			DocumentViewerInternal.PrepareControl();
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxDocumentViewer), InternalUtils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxDocumentViewer), InternalUtils.ReportScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			if(CallbackRouteValues != null)
				stb.Append(string.Concat(localVarName, ".callbackUrl=\"", InternalUtils.GetUrl(CallbackRouteValues), "\";\n"));
			if(ExportRouteValues != null)
				stb.Append(string.Concat(localVarName, ".exportUrl=\"", InternalUtils.GetUrl(ExportRouteValues), "\";\n"));
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientDocumentViewer";
		}
		protected internal void EnsureChildControlsRecursive() {
			base.EnsureChildControlsRecursive(this);
		}
		protected override string ExternalRibbonClientID {
			get { return AssociatedRibbonID; }
		}
#if DebugTest
		protected override void PrepareUserControl(System.Web.UI.Control userControl, System.Web.UI.Control parent, string id, bool builtInControl) {
			base.PrepareUserControl(userControl, parent, id, builtInControl);
			userControl.AppRelativeTemplateSourceDirectory = null;
		}
#endif
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MVCxDocumentViewerClientSideEvents();
		}
		internal static Hashtable LoadClientObjectStateInternal(NameValueCollection postCollection, string name) {
			return LoadClientObjectState(postCollection, name);
		}
		internal static string GetClientObjectStateValueStringInternal(Hashtable clientObjectState, string key) {
			return GetClientObjectStateValueString(clientObjectState, key);
		}
		internal static T GetClientObjectStateValueInternal<T>(Hashtable clientObjectState, string key) {
			return GetClientObjectStateValue<T>(clientObjectState, key);
		}
	}
}

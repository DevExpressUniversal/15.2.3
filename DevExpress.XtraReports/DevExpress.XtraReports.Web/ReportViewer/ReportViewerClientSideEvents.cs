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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Web;
namespace DevExpress.XtraReports.Web {
	public class ReportViewerClientSideEvents : ClientSideEvents {
		const string
			PageLoadEvent = "PageLoad",
			BeginCallbackEvent = "BeginCallback",
			EndCallbackEvent = "EndCallback",
			CallbackErrorEvent = "CallbackError";
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new string Init {
			get { return string.Empty; }
			set { }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerClientSideEventsPageLoad")]
#endif
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string PageLoad {
			get { return GetEventHandler(PageLoadEvent); }
			set { SetEventHandler(PageLoadEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerClientSideEventsBeginCallback")]
#endif
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string BeginCallback {
			get { return GetEventHandler(BeginCallbackEvent); }
			set { SetEventHandler(BeginCallbackEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerClientSideEventsEndCallback")]
#endif
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string EndCallback {
			get { return GetEventHandler(EndCallbackEvent); }
			set { SetEventHandler(EndCallbackEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerClientSideEventsCallbackError")]
#endif
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string CallbackError {
			get { return GetEventHandler(CallbackErrorEvent); }
			set { SetEventHandler(CallbackErrorEvent, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(PageLoadEvent);
			names.Add(BeginCallbackEvent);
			names.Add(EndCallbackEvent);
			names.Add(CallbackErrorEvent);
		}
	}
}

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
using System.ComponentModel;
using System.Text;
using DevExpress.Data.Utils;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.ReportDesigner.Native.Services;
using InternalUtils = DevExpress.Web.Mvc.Internal.Utils;
namespace DevExpress.Web.Mvc {
	[ToolboxItem(false)]
	public class MVCxReportDesigner : ASPxReportDesigner {
		public object SaveCallbackRouteValues { get; set; }
		readonly IJSContentGenerator<ReportDesignerModel> jsContentGenerator;
		static MVCxReportDesigner() {
			ASPxReportDesigner.StaticInitialize();
		}
		public MVCxReportDesigner()
			: this(DefaultReportDesignerContainer.Current) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxReportDesigner(IServiceProvider serviceProvider)
			: this(
			serviceProvider.GetService<IReportDesignerModelGenerator>(),
			serviceProvider.GetService<IReportDesignerHtmlContentGenerator>(),
			serviceProvider.GetService<IJSContentGenerator<ReportDesignerModel>>()) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MVCxReportDesigner(
			IReportDesignerModelGenerator reportDesignerModelGenerator,
			IReportDesignerHtmlContentGenerator htmlContentGenerator,
			IJSContentGenerator<ReportDesignerModel> jsContentGenerator)
			: base(
			reportDesignerModelGenerator,
			htmlContentGenerator,
			jsContentGenerator) {
			this.jsContentGenerator = jsContentGenerator;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(MVCxReportDesigner), InternalUtils.UtilsScriptResourceName);
			RegisterIncludeScript(typeof(MVCxReportDesigner), InternalUtils.ReportDesignerScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "MVCxClientReportDesigner";
		}
		protected override bool IsCallBacksEnabled() {
			return SaveCallbackRouteValues != null;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(Model != null) {
				jsContentGenerator.Generate(stb, localVarName, Model);
			}
			if(SaveCallbackRouteValues != null) {
				stb.Append(localVarName + ".callbackUrl=\"" + InternalUtils.GetUrl(SaveCallbackRouteValues) + "\";\n");
			}
		}
		internal ReportDesignerModel Model { get; set; }
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MVCxReportDesignerClientSideEvents();
		}
	}
}

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

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
namespace DevExpress.Web.Mvc {
	public class WebDocumentViewerExtension : ExtensionBase {
		static WebDocumentViewerExtension() {
			MVCxWebDocumentViewer.StaticInitialize();
		}
		public static bool ShouldClearReportScripts {
			get { return ReportLayoutJsonSerializer.ShouldClearScripts; }
			set { ReportLayoutJsonSerializer.ShouldClearScripts = value; }
		}
		public WebDocumentViewerExtension(SettingsBase settings)
			: base(settings) {
		}
		public WebDocumentViewerExtension(SettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxWebDocumentViewer Control {
			get { return (MVCxWebDocumentViewer)base.Control; }
			protected set { base.Control = value; }
		}
		internal new WebDocumentViewerSettings Settings {
			get { return (WebDocumentViewerSettings)base.Settings; }
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxWebDocumentViewer();
		}
		protected override void AssignInitialProperties() {
			Control.Style.Clear();
			base.AssignInitialProperties();
			Control.ClientSideEvents.Assign(Settings.ClientSideEventsInternal);
			Control.MenuItems.Assign(Settings.MenuItems);
			Control.ReportSourceId = Settings.ReportSourceId;
		}
		public WebDocumentViewerExtension Bind(XtraReport report) {
			Control.OpenReport(report);
			return this;
		}
		public WebDocumentViewerExtension Bind(byte[] reportXmlLayout) {
			Control.OpenReportXmlLayout(reportXmlLayout);
			return this;
		}
		public WebDocumentViewerExtension Bind(WebDocumentViewerModel model) {
			Control.Model = model;
			return this;
		}
		public static WebDocumentViewerModel GetModel(XtraReport report) {
			return GetModel(report, null);
		}
		public static WebDocumentViewerModel GetModel(XtraReport report, IEnumerable<ClientControlsMenuItem> menuItems) {
			MVCxWebDocumentViewer.StaticInitialize();
			var reportModel = new ReportModelInfo(report);
			var menuItemModels = menuItems != null
				? menuItems.Select(x => x.ToModel())
				: null;
			var webDocumentViewerModelGenerator = DefaultWebDocumentViewerContainer.Current.GetService<IWebDocumentViewerModelGenerator>();
			return webDocumentViewerModelGenerator.Generate(reportModel, menuItemModels);
		}
	}
}

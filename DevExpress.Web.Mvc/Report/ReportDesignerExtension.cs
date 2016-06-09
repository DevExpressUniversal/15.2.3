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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DevExpress.Data.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.WebClientUIControl.Internal;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
using DevExpress.XtraReports.Web.ReportDesigner.Native.Services;
namespace DevExpress.Web.Mvc {
	public class ReportDesignerExtension : ExtensionBase {
		static ReportDesignerExtension() {
			MVCxReportDesigner.StaticInitialize();
		}
		public static ReportDesignerModel GetModel(XtraReport report) {
			return GetModel(report, null, null, null);
		}
		public static ReportDesignerModel GetModel(XtraReport report, IDictionary<string, object> dataSources) {
			return GetModel(report, dataSources, null, null);
		}
		public static ReportDesignerModel GetModel(XtraReport report, IEnumerable<ClientControlsMenuItem> menuItems) {
			return GetModel(report, null, null, menuItems);
		}
		public static ReportDesignerModel GetModel(XtraReport report, IDictionary<string, string> subreports) {
			return GetModel(report, null, subreports, null);
		}
		public static ReportDesignerModel GetModel(XtraReport report, IDictionary<string, object> dataSources, IEnumerable<ClientControlsMenuItem> menuItems) {
			return GetModel(report, dataSources, null, menuItems);
		}
		public static ReportDesignerModel GetModel(XtraReport report, IDictionary<string, string> subreports, IEnumerable<ClientControlsMenuItem> menuItems) {
			return GetModel(report, null, subreports, menuItems);
		}
		public static ReportDesignerModel GetModel(XtraReport report, IDictionary<string, object> dataSources, IDictionary<string, string> subreports, IEnumerable<ClientControlsMenuItem> menuItems) {
			return GetModel(report, dataSources, subreports, menuItems, new ReportDesignerModelSettings());
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static ReportDesignerModel GetModel(XtraReport report, IDictionary<string, object> dataSources, IDictionary<string, string> subreports, IEnumerable<ClientControlsMenuItem> menuItems, ReportDesignerModelSettings settings) {
			var reportDesignerModelGenerator = DefaultReportDesignerContainer.Current.GetService<IReportDesignerModelGenerator>();
			var menuItemModel = menuItems != null
				? menuItems.Select(x => x.ToModel())
				: null;
			return reportDesignerModelGenerator.Generate(report, dataSources, subreports, menuItemModel, settings);
		}
		public static byte[] GetReportXml(string name) {
			var hiddenFieldName = name + ReportDesignerValueProvider.Postfix;
			var value = HttpUtils.GetValueFromRequest(hiddenFieldName);
			var eventArgument = GetEventArgument(value);
			var input = ReportDesignerInputLoader.FromString(eventArgument);
			return input.ReportLayout;
		}
		public static byte[] ConvertReportJsonToXml(string json) {
			return ReportLayoutJsonSerializer.LoadFromJsonAndReturnXml(json);
		}
		static string GetEventArgument(string eventArgument) {
			const char CallbackSeparator = ':';
			var separatorPosition = eventArgument.IndexOf(CallbackSeparator);
			return eventArgument.Substring(separatorPosition + 1);
		}
		protected internal new MVCxReportDesigner Control {
			get { return (MVCxReportDesigner)base.Control; }
		}
		internal new ReportDesignerSettings Settings {
			get { return (ReportDesignerSettings)base.Settings; }
		}
		public ReportDesignerExtension(SettingsBase settings)
			: base(settings) {
		}
		public ReportDesignerExtension(SettingsBase settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public ReportDesignerExtension Bind(XtraReport report) {
			Control.OpenReport(report);
			return this;
		}
		public ReportDesignerExtension BindToUrl(string reportUrl) {
			Control.OpenReport(reportUrl);
			return this;
		}
		public ReportDesignerExtension Bind(string reportXmlLayout) {
			Control.OpenReportXmlLayout(Encoding.UTF8.GetBytes(reportXmlLayout));
			return this;
		}
		public ReportDesignerExtension Bind(byte[] reportXmlLayout) {
			Control.OpenReportXmlLayout(reportXmlLayout);
			return this;
		}
		public ReportDesignerExtension Bind(ReportDesignerModel model) {
			Control.Model = model;
			return this;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxReportDesigner();
		}
		protected override void AssignInitialProperties() {
			Control.Style.Clear();
			base.AssignInitialProperties();
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			AssignDictionary(Control.DataSources, Settings.DataSources);
			AssignDictionary(Control.Subreports, Settings.Subreports);
			Control.MenuItems.Assign(Settings.MenuItems);
			Control.ShouldDisposeReport = Settings.ShouldDisposeReport;
			Control.ShouldDisposeDataSources = Settings.ShouldDisposeDataSources;
			Control.ShouldShareReportDataSources = Settings.ShouldShareReportDataSources;
			Control.SaveCallbackRouteValues = Settings.SaveCallbackRouteValues;
			Control.CustomJSProperties += Settings.CustomJSProperties;
		}
		void AssignDictionary<TKey, TValue>(IDictionary<TKey, TValue> destinationDictionary, Dictionary<TKey, TValue> sourceDictionary) {
			foreach(var pair in sourceDictionary) {
				destinationDictionary[pair.Key] = pair.Value;
			}
		}
	}
}

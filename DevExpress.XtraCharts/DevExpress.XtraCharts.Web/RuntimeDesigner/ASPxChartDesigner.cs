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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Web.Designer.Native;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using DevExpress.XtraPrinting.WebClientUIControl.DataContracts;
namespace DevExpress.XtraCharts.Web.Designer {
	[DXWebToolboxItem(true),
	Designer("DevExpress.XtraCharts.Web.Design.ASPxChartDesignerDesigner," + AssemblyInfo.SRAssemblyChartsWebDesign),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "ASPxChartDesigner.bmp"),
	ToolboxTabName(AssemblyInfo.DXTabData)]
	public class ASPxChartDesigner : ASPxWebClientUIControl {
		const string StylesCssResourcePathPrefix = "DevExpress.XtraCharts.Web.Css.chart-designer-";
		const string ScriptResourcePath = "DevExpress.XtraCharts.Web.Scripts.";
		const string ClientSideCategoryName = "Client-Side";
		const string ThisName = "DevExpress.XtraCharts.Web.Designer.ASPxChartDesigner";
		const string DefaultHandlerUri = "DXXRD.axd";
#if DEBUG
		internal const string ScriptCommonDesignerResourceName = "DevExpress.XtraCharts.Web.Scripts.dx-designer.js";
		internal const string ScriptResourceName = "DevExpress.XtraCharts.Web.Scripts.chart-designer.js";
		internal const string StylesCssResourcePathLight = "DevExpress.XtraCharts.Web.Css.chart-designer-light.css";
		internal const string StylesCssResourcePathDark = "DevExpress.XtraCharts.Web.Css.chart-designer-dark.css";
#else
		internal const string ScriptCommonDesignerResourceName = "DevExpress.XtraCharts.Web.Scripts.dx-designer.min.js";
		internal const string ScriptResourceName = "DevExpress.XtraCharts.Web.Scripts.chart-designer.min.js";
		internal const string StylesCssResourcePathLight = "DevExpress.XtraCharts.Web.Css.chart-designer-light.min.css";
		internal const string StylesCssResourcePathDark = "DevExpress.XtraCharts.Web.Css.chart-designer-dark.min.css";
#endif
		internal const string ScriptDXResourceName = "DevExpress.XtraCharts.Web.Scripts.ASPxChartDesigner.js";
		internal const string ControlHierachyHtml = "DevExpress.XtraCharts.Web.Content.chart-designer.html";
		internal const string ASPxChartDesignerDesignTimeView = "DevExpress.XtraCharts.Web.Content.ASPxChartDesignerDesignTimeView.html";
		internal const string ASPxChartDesignerDesignTimeIconName = "ASPxChartDesignerDesignTimeIcon";
		internal const string ASPxChartDesignerImagesResourcePath = "DevExpress.XtraCharts.Web.Images.";
		internal const string ASPxChartDesignerDesignTimeIconFullName = ASPxChartDesignerImagesResourcePath + ASPxChartDesignerDesignTimeIconName + ".png";
		static readonly object SaveChartLayoutEvent = new object();
		static readonly Unit WidthDefault = Unit.Percentage(100);
		static readonly Unit HeightDefault = Unit.Pixel(850);
		static ASPxChartDesigner() {
			ASPxHttpHandlerModule.Subscribe(new ChartHandler());
		}
		WebChartControl chartControl;
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		[DefaultValue(typeof(Unit), "100%")]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[DefaultValue(typeof(Unit), "850px")]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[DXDescription(ThisName + ",ClientInstanceName"),
		AutoFormatDisable,
		Category(ClientSideCategoryName),
		DefaultValue(""),
		Localizable(false)]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		[DXDescription(ThisName + ",MenuItems"),
		PersistenceMode(PersistenceMode.InnerProperty),
		MergableProperty(false),
		AutoFormatDisable]
		internal ClientControlMenuItemCollection MenuItems { get; private set; }
		[DXDescription(ThisName + ",ClientSideEvents"),
		MergableProperty(false),
		Category(ClientSideCategoryName),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable]
		public ClientSideEvents ClientSideEvents {
			get { return (ClientSideEvents)ClientSideEventsInternal; }
		}
		[DXDescription(ThisName + ",JSProperties"),
		Category("Client-Side"),
		Browsable(false),
		AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[DXDescription(ThisName + ",SaveChartLayout")]
		public event SaveChartLayoutEventHandler SaveChartLayout {
			add { Events.AddHandler(SaveChartLayoutEvent, value); }
			remove { Events.RemoveHandler(SaveChartLayoutEvent, value); }
		}
		[DXDescription(ThisName + ",CustomJSProperties"),
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		public ASPxChartDesigner() {
			MenuItems = new ClientControlMenuItemCollection(this);
			Width = WidthDefault;
			Height = HeightDefault;
		}
		void RaiseSaveChartLayout(string chartLayoutJSON) {
			var ev = Events[SaveChartLayoutEvent] as SaveChartLayoutEventHandler;
			if (ev != null) {
				byte[] chartLayoutXmlBytes = FromString(chartLayoutJSON);
				string chartLayoutXml = Encoding.UTF8.GetString(chartLayoutXmlBytes);
				var args = new SaveChartLayoutEventArgs(chartLayoutXml);
				ev(this, args);
			}
		}
		byte[] FromString(string argument) {
			if (string.IsNullOrEmpty(argument)) {
				return null;
			}
			var arguments = argument.Split(new[] { '&' }, 2)
				.Select(x => x.Split(new[] { '=' }, 2))
				.ToDictionary(x => x[0], x => x.Length > 1 ? x[1] : "");
			string chartLayoutJson;
			if (!arguments.TryGetValue("chartLayout", out chartLayoutJson)) {
				throw new InvalidOperationException("There is no 'chartLayout' argument");
			}
			var decodedChartXmlLayoutString = HttpUtility.UrlDecode(chartLayoutJson);
			return JsonConverter.JsonToXml(decodedChartXmlLayoutString);
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterDefaultRenderCssFile() {
			if (DesignMode)
				return;
			RegisterDevExtremeCss(Page, UsefulColorScheme);
			RegisterJQueryUICss(Page);
#if !DEBUG
			ResourceManager.RegisterCssResource(Page, typeof(ASPxChartDesigner), StylesCssResourcePathPrefix + UsefulColorScheme + ".min.css");
#else
			ResourceManager.RegisterCssResource(Page, typeof(ASPxChartDesigner), StylesCssResourcePathPrefix + UsefulColorScheme + ".css");
#endif
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientChartDesigner";
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			RaiseSaveChartLayout(eventArgument);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (chartControl != null) {
				using (var ms = new MemoryStream()) {
					var menuItems = MenuItems.Select(x => x.ToContract());
					var menuActions = MenuItems.Select(x => x.JSClickAction);
					string guid = "";
					if(chartControl != null && chartControl.DataSource != null) {
						guid = Guid.NewGuid().ToString();
						ChartHandler.dataSources.Add(guid, chartControl.DataSource);
					}
					var chart = (IChartContainer)chartControl;
					chart.Chart.SaveLayout(ms);
					if (ms.Position > 0) {
						ms.Position = 0;
					}
					var layout = JsonConverter.XmlToJson(ms);
					new JsAssignmentGenerator(stb, localVarName)
						.AppendRaw("chartModel", layout)
						.AppendContract("dataSource", new DataSourceInfo { Id = guid, Name = "Data Source", Data = "" })
						.AppendRaw("width", ((WebChartControl)chartControl).Width.Value.ToString())
						.AppendRaw("height", ((WebChartControl)chartControl).Height.Value.ToString())
						.AppendAsString("handlerUri", ASPxChartDesigner.DefaultHandlerUri)
						.AppendContract("menuItems", menuItems)
						.AppendRawArray("menuItemActions", menuActions);
				}
			}
		}
		protected override void RegisterDefaultSpriteCssFile() {
		}
		protected override void RegisterSystemCssFile() {
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterJQueryScript(Page);
			RegisterJQueryUIScript(Page);
			RegisterGlobalizeScript(Page);
			RegisterGlobalizeCulturesScript(Page);
			RegisterKnockoutScript(Page);
			RegisterDevExtremeBaseScript(Page);
			RegisterDevExtremeWebWidgetsScript(Page);
			RegisterIncludeScript(typeof(ASPxChartDesigner), ScriptDXResourceName);
			RegisterIncludeScript(typeof(ASPxChartDesigner), ScriptCommonDesignerResourceName);
			RegisterIncludeScript(typeof(ASPxChartDesigner), ScriptResourceName);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ClientSideEvents();
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			var type = typeof(ASPxChartDesigner);
			using (var reader = new StreamReader(type.Assembly.GetManifestResourceStream(ControlHierachyHtml))) {
				var text = reader.ReadToEnd();
				Controls.Add(new LiteralControl(text + "<div class='dx-designer' data-bind='template: \"dxrd-designer\"'><span class='dxwcuic-platformError dxwcuic-hidden'></span></div>"));
			}
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		public void OpenChart(WebChartControl chart) {
			if (this.chartControl == chart) {
				return;
			}
			this.chartControl = chart;
			LayoutChanged();
		}
	}
	public delegate void SaveChartLayoutEventHandler(object sender, SaveChartLayoutEventArgs e);
	public class SaveChartLayoutEventArgs : EventArgs {
		public string ChartLayoutXml { get; private set; }
		public SaveChartLayoutEventArgs(string chartLayoutXml) {
			ChartLayoutXml = chartLayoutXml;
		}
	}
}

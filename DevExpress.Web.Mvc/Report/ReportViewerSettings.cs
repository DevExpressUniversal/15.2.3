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
using DevExpress.Web;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web;
using Constants = DevExpress.XtraReports.Web.Native.Constants.ReportViewer;
namespace DevExpress.Web.Mvc {
	public class ReportViewerSettings : SettingsBase {
		internal SettingsLoadingPanel SettingsLoadingPanel { get; set; }
		public XtraReport Report { get; set; }
		public object CallbackRouteValues { get; set; }
		public object ExportRouteValues { get; set; }
		public MVCxReportViewerClientSideEvents ClientSideEvents { get { return (MVCxReportViewerClientSideEvents)ClientSideEventsInternal; } }
		public int LoadingPanelDelay { get { return SettingsLoadingPanel.Delay; } set { SettingsLoadingPanel.Delay = value; } }
		public ImagePosition LoadingPanelImagePosition { get { return SettingsLoadingPanel.ImagePosition; } set { SettingsLoadingPanel.ImagePosition = value; } }
		public string LoadingPanelText { get { return SettingsLoadingPanel.Text; } set { SettingsLoadingPanel.Text = value; } }
		public bool ShowLoadingPanelImage { get { return SettingsLoadingPanel.ShowImage; } set { SettingsLoadingPanel.ShowImage = value; } }
		public Border BookmarkSelectionBorder { get { return ((ReportViewerStyle)ControlStyle).BookmarkSelectionBorder; } }
		public ImagesBase Images { get { return ImagesInternal; } }
		public bool PageByPage { get; set; }
		public bool PrintUsingAdobePlugIn { get; set; }
		public bool AutoSize { get; set; }
		public bool TableLayout { get; set; }
		public ImagesEmbeddingMode ImagesEmbeddingMode { get; set; }
		public bool ShouldDisposeReport { get; set; }
		public bool EnableReportMargins { get; set; }
		public Paddings Paddings { get; private set; }
		public EditorImages SearchDialogEditorsImages { get; private set; }
		public ImagesDialogForm SearchDialogFormImages { get; private set; }
		public EditorStyles SearchDialogEditorsStyles { get; private set; }
		public ButtonControlStyles SearchDialogButtonStyles { get; private set; }
		public PopupControlStyles SearchDialogFormStyles { get; private set; }
		public CacheReportDocumentEventHandler CacheReportDocument { get; set; }
		public RestoreReportDocumentFromCacheEventHandler RestoreReportDocumentFromCache { get; set; }
		public EventHandler<DeserializeClientParameterEventArgs> DeserializeClientParameters { get; set; }
		public ReportViewerSettings() {
			Paddings = new Paddings();
			SearchDialogEditorsImages = new EditorImages(null);
			SearchDialogFormImages = new ImagesDialogForm(null);
			SearchDialogEditorsStyles = new EditorStyles(null);
			SearchDialogButtonStyles = new ButtonControlStyles(null);
			SearchDialogFormStyles = new PopupControlStyles(null);
			SettingsLoadingPanel = new SettingsLoadingPanel(null);
			PageByPage = Constants.DefaultPageByPage;
			PrintUsingAdobePlugIn = Constants.DefaultPrintUsingAdobePlugIn;
			AutoSize = Constants.DefaultAutoSize;
			TableLayout = Constants.DefaultTableLayout;
			ImagesEmbeddingMode = Constants.DefaultImagesEmbeddingMode;
			ShouldDisposeReport = Constants.DefaultShouldDisposeReport;
			EnableReportMargins = Constants.DefaultEnableReportMargins;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MVCxReportViewerClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return null;
		}
		protected override AppearanceStyleBase CreateControlStyle() {
			return new ReportViewerStyle();
		}
	}
	public class MVCxReportViewerClientSideEvents : ReportViewerClientSideEvents {
		const string BeforeExportRequestName = "BeforeExportRequest";
		public string BeforeExportRequest {
			get { return GetEventHandler(BeforeExportRequestName); }
			set { SetEventHandler(BeforeExportRequestName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(BeforeExportRequestName);
		}
	}
}

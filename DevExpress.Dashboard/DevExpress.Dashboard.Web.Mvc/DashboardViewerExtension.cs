#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Web.Mvc.UI;
namespace DevExpress.DashboardWeb.Mvc {
	public static class MvcDashboardFactory {
		public static DashboardViewerExtension DashboardViewer(this ExtensionsFactory factory, DashboardViewerSettings settings) {
			return new DashboardViewerExtension(settings);
		}
		public static DashboardViewerExtension DashboardViewer(this ExtensionsFactory factory, Action<DashboardViewerSettings> method) {
			DashboardViewerSettings settings = new DashboardViewerSettings();
			method(settings);
			return new DashboardViewerExtension(settings);
		}
	}
	public class DashboardViewerExtension : ExtensionBase {
		const string ExportCommandArgument = "DXMVCDashboardExportArgument";
		public static FileStreamResult Export(string name, DashboardSourceModel dataSettings) {
			DashboardViewerExtension extension = new DashboardViewerExtension(new DashboardViewerSettings() { Name = name });
			extension.BindToSource(dataSettings);
			try {
				extension.PrepareControl();
				extension.LoadPostData();
				string requestArguments = extension.PostDataCollection[ExportCommandArgument]; ;
				return extension.Control.PerformExport(requestArguments);
			} finally {
				extension.DisposeControl();
			}
		}
		public DashboardViewerExtension(DashboardViewerSettings settings)
			: base(settings) { 
		}
		public DashboardViewerExtension(DashboardViewerSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public DashboardViewerExtension BindToSource(DashboardSourceModel model) {
			Control.DashboardSource = model.DashboardSource;
			Control.DashboardId = model.DashboardId;
			Control.ConfigureDataConnection += model.ConfigureDataConnection;
			Control.CustomFilterExpression += model.CustomFilterExpression;
			Control.CustomParameters += model.CustomParameters;
			Control.DashboardLoaded += model.DashboardLoaded;
			Control.DashboardLoading += model.DashboardLoading;
			Control.DataLoading += model.DataLoading;
			Control.SingleFilterDefaultValue += model.SingleFilterDefaultValue;
			Control.FilterElementDefaultValues += model.FilterElementDefaultValues;
			Control.RangeFilterDefaultValue += model.RangeFilterDefaultValue;
			Control.ValidateCustomSqlQuery += model.ValidateCustomSqlQuery;
			return this;
		}
		protected internal new MVCxDashboardViewer Control {
			get { return (MVCxDashboardViewer)base.Control; }
		}
		protected internal new DashboardViewerSettings Settings {
			get { return (DashboardViewerSettings)base.Settings; }
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.AllowExportDashboardItems = Settings.AllowExportDashboardItems;
			Control.FullscreenMode = Settings.FullscreenMode;
			Control.RedrawOnResize = Settings.RedrawOnResize;
			Control.CalculateHiddenTotals = Settings.CalculateHiddenTotals;
			Control.HandleServerErrors = Settings.HandleServerErrors;
			Control.AllowExportDashboard = Settings.AllowExportDashboard;
			Control.ClientInstanceName = Settings.Name;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ExportOptions.Assign(Settings.ExportOptions);
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.ExportRouteValues = Settings.ExportRouteValues;
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxDashboardViewer();
		}
	}
}

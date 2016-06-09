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
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.ClientControls;
using Constants = DevExpress.XtraReports.Web.Native.Constants.ReportDesigner;
namespace DevExpress.Web.Mvc {
	public class ReportDesignerSettings : SettingsBase {
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new string Theme {
			get { return base.Theme; }
		}
		public MVCxReportDesignerClientSideEvents ClientSideEvents { get { return (MVCxReportDesignerClientSideEvents)ClientSideEventsInternal; } }
		public Dictionary<string, object> DataSources { get; private set; }
		public Dictionary<string, string> Subreports { get; private set; }
		public ClientControlsMenuItemCollection<ClientControlsMenuItem> MenuItems { get; private set; }
		public bool ShouldDisposeReport { get; set; }
		public bool ShouldDisposeDataSources { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldShareReportDataSources { get; set; }
		public object SaveCallbackRouteValues { get; set; }
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public ReportDesignerSettings() {
			DataSources = new Dictionary<string, object>();
			Subreports = new Dictionary<string, string>();
			MenuItems = new ClientControlsMenuItemCollection<ClientControlsMenuItem>(null);
			Width = Constants.WidthDefault;
			Height = Constants.HeightDefault;
			ShouldDisposeReport = Constants.ShouldDisposeReportDefault;
			ShouldDisposeDataSources = Constants.ShouldDisposeDataSourcesDefault;
			ShouldShareReportDataSources = Constants.ShouldShareReportDataSourcesDefault;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new MVCxReportDesignerClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new ImagesBase(null);
		}
		protected override StylesBase CreateStyles() {
			return null;
		}
	}
	public class MVCxReportDesignerClientSideEvents : ReportDesignerClientSideEvents {
		const string SaveCommandExecutedName = "SaveCommandExecuted";
		public string SaveCommandExecuted {
			get { return GetEventHandler(SaveCommandExecutedName); }
			set { SetEventHandler(SaveCommandExecutedName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(SaveCommandExecutedName);
		}
	}
}

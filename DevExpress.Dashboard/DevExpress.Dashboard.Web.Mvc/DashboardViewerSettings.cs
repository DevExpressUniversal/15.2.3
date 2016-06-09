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
using System.ComponentModel;
using DevExpress.DashboardCommon;
using DevExpress.Web;
using DevExpress.Web.Mvc;
namespace DevExpress.DashboardWeb.Mvc {
	public class DashboardViewerSettings : SettingsBase {
		const int DefaultHeight = 600;
		const int DefaultWidth = 800;
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never),
		ToolboxItem(false)
		]
		public new bool EnableTheming {
			get { return base.EnableTheming; }
			set { base.EnableTheming = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		ToolboxItem(false)
		]
		public new virtual string SkinID {
			get { return base.SkinID; }
			set { base.SkinID = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		ToolboxItem(false)
		]
		public new string Theme {
			get { return base.Theme; }
			set { base.Theme = value;  }
		}
		public bool FullscreenMode { get; set; }
		public bool RedrawOnResize { get; set; }
		[Obsolete("This property is now obsolete. You no longer need to set it to true in order to use data access API.")]
		public bool UseDataAccessApi { get; set; }
		public bool CalculateHiddenTotals { get; set; }
		public bool HandleServerErrors { get; set; }
		public bool AllowExportDashboard { get; set; }
		public bool AllowExportDashboardItems { get; set; }
		public double SessionTimeout { get; set; }
		public DashboardExportOptions ExportOptions { get; set; }
		public DashboardClientSideEvents ClientSideEvents {
			get { return (DashboardClientSideEvents)ClientSideEventsInternal; } 
		}
		public CustomJSPropertiesEventHandler CustomJSProperties { get; set; }
		public object CallbackRouteValues { get; set; }
		public object ExportRouteValues { get; set; }
		public DashboardViewerSettings() : base() {
			CalculateHiddenTotals = false;
			HandleServerErrors = false;
			RedrawOnResize = true;
			AllowExportDashboard = true;
			AllowExportDashboardItems = false;
			FullscreenMode = false;
			Height = DefaultHeight;
			Width = DefaultWidth;
			ExportOptions = new DashboardExportOptions();
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new DashboardClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return null;
		}
		protected override StylesBase CreateStyles() {
			return null;
		}
	}
	public class DashboardSourceModel {
		public object DashboardSource { get; set; }
		public string DashboardId { get; set; }
		public DashboardSourceModel() {
			DashboardId = string.Empty;
		}
		public ConfigureDataConnectionWebEventHandler ConfigureDataConnection { get; set; }
		public CustomFilterExpressionWebEventHandler CustomFilterExpression { get; set; }
		public CustomParametersWebEventHandler CustomParameters { get; set; }
		public DashboardLoadingEventHandler DashboardLoading { get; set; }
		public DashboardLoadedWebEventHandler DashboardLoaded { get; set; }
		public DataLoadingWebEventHandler DataLoading { get; set; }
		public SingleFilterDefaultValueEventHandler SingleFilterDefaultValue { get; set; }
		public FilterElementDefaultValuesEventHandler FilterElementDefaultValues { get; set; }
		public RangeFilterDefaultValueEventHandler RangeFilterDefaultValue { get; set; }
		public ValidateDashboardCustomSqlQueryWebEventHandler ValidateCustomSqlQuery { get; set; }
	}
}

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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Web;
namespace DevExpress.DashboardWeb {
	public class DashboardClientSideEvents : CallbackClientSideEventsBase {
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsActionAvailabilityChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ActionAvailabilityChanged {
			get { return GetEventHandler("ActionAvailabilityChanged"); }
			set { SetEventHandler("ActionAvailabilityChanged", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsMasterFilterSet"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string MasterFilterSet {
			get { return GetEventHandler("MasterFilterSet"); }
			set { SetEventHandler("MasterFilterSet", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsMasterFilterCleared"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string MasterFilterCleared {
			get { return GetEventHandler("MasterFilterCleared"); }
			set { SetEventHandler("MasterFilterCleared", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsDrillDownPerformed"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string DrillDownPerformed {
			get { return GetEventHandler("DrillDownPerformed"); }
			set { SetEventHandler("DrillDownPerformed", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsDrillUpPerformed"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string DrillUpPerformed {
			get { return GetEventHandler("DrillUpPerformed"); }
			set { SetEventHandler("DrillUpPerformed", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsLoaded"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string Loaded {
			get { return GetEventHandler("Loaded"); }
			set { SetEventHandler("Loaded", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsItemClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ItemClick {
			get { return GetEventHandler("ItemClick"); }
			set { SetEventHandler("ItemClick", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsItemVisualInteractivity"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ItemVisualInteractivity {
			get { return GetEventHandler("ItemVisualInteractivity"); }
			set { SetEventHandler("ItemVisualInteractivity", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsItemSelectionChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ItemSelectionChanged {
			get { return GetEventHandler("ItemSelectionChanged"); }
			set { SetEventHandler("ItemSelectionChanged", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsItemElementCustomColor"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ItemElementCustomColor {
			get { return GetEventHandler("ItemElementCustomColor"); }
			set { SetEventHandler("ItemElementCustomColor", value); }
		}
		[
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsItemWidgetCreated"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ItemWidgetCreated {
			get { return GetEventHandler("ItemWidgetCreated"); }
			set { SetEventHandler("ItemWidgetCreated", value); }
		}
		[
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ItemWidgetUpdating {
			get { return GetEventHandler("ItemWidgetUpdating"); }
			set { SetEventHandler("ItemWidgetUpdating", value); }
		}
		[
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ItemWidgetUpdated {
			get { return GetEventHandler("ItemWidgetUpdated"); }
			set { SetEventHandler("ItemWidgetUpdated", value); }
		}
		[Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is obsolete now. Client widgets are not recreated now. Use the ItemWidgetUpdating property instead and ensure the necessity of it.", false),
#if !SL
	DevExpressDashboardWebLocalizedDescription("DashboardClientSideEventsItemBeforeWidgetDisposed"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string ItemBeforeWidgetDisposed {
			get { return GetEventHandler("ItemBeforeWidgetDisposed"); }
			set { SetEventHandler("ItemBeforeWidgetDisposed", value); }
		}
		public DashboardClientSideEvents()
			: base() {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ActionAvailabilityChanged");
			names.Add("MasterFilterSet");
			names.Add("MasterFilterCleared");
			names.Add("DrillDownPerformed");
			names.Add("DrillUpPerformed");
			names.Add("Loaded");
			names.Add("ItemClick");
			names.Add("ItemVisualInteractivity");
			names.Add("ItemSelectionChanged");
			names.Add("ItemWidgetCreated");
			names.Add("ItemWidgetUpdating");
			names.Add("ItemWidgetUpdated");
			names.Add("ItemBeforeWidgetDisposed");
			names.Add("ItemElementCustomColor");
		}
	}
}

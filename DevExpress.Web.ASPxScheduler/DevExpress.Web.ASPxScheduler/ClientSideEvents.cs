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
using System.Drawing.Design;
using DevExpress.Web;
namespace DevExpress.Web.ASPxScheduler {
	public class SchedulerClientSideEvents : CallbackClientSideEventsBase {
		#region AppointmentDeleting
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsAppointmentDeleting"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string AppointmentDeleting {
			get { return GetEventHandler("AppointmentDeleting"); }
			set { SetEventHandler("AppointmentDeleting", value); }
		}
		#endregion
		#region ActiveViewChanging
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsActiveViewChanging"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ActiveViewChanging {
			get { return GetEventHandler("ActiveViewChanging"); }
			set { SetEventHandler("ActiveViewChanging", value); }
		}
		#endregion
		#region ActiveViewChanged
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsActiveViewChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ActiveViewChanged {
			get { return GetEventHandler("ActiveViewChanged"); }
			set { SetEventHandler("ActiveViewChanged", value); }
		}
		#endregion
		#region SelectionChanging
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsSelectionChanging"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string SelectionChanging {
			get { return GetEventHandler("SelectionChanging"); }
			set { SetEventHandler("SelectionChanging", value); }
		}
		#endregion
		#region SelectionChanged
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsSelectionChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string SelectionChanged {
			get { return GetEventHandler("SelectionChanged"); }
			set { SetEventHandler("SelectionChanged", value); }
		}
		#endregion
		#region VisibleIntervalChanged
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsVisibleIntervalChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string VisibleIntervalChanged {
			get { return GetEventHandler("VisibleIntervalChanged"); }
			set { SetEventHandler("VisibleIntervalChanged", value); }
		}
		#endregion
		#region MoreButtonClicked
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsMoreButtonClicked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string MoreButtonClicked {
			get { return GetEventHandler("MoreButtonClicked"); }
			set { SetEventHandler("MoreButtonClicked", value); }
		}
		#endregion
		#region MenuItemClicked
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsMenuItemClicked"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string MenuItemClicked {
			get { return GetEventHandler("MenuItemClicked"); }
			set { SetEventHandler("MenuItemClicked", value); }
		}
		#endregion
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsAppointmentClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string AppointmentClick {
			get { return GetEventHandler("AppointmentClick"); }
			set { SetEventHandler("AppointmentClick", value); }
		}
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsAppointmentDoubleClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string AppointmentDoubleClick {
			get { return GetEventHandler("AppointmentDoubleClick"); }
			set { SetEventHandler("AppointmentDoubleClick", value); }
		}
		[DefaultValue(""), EditorBrowsable(EditorBrowsableState.Never), Localizable(false), NotifyParentProperty(true)]
		public string MouseUp {
			get { return GetEventHandler("MouseUp"); }
			set { SetEventHandler("MouseUp", value); }
		}
		#region AppointmentsSelectionChanged
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsAppointmentsSelectionChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string AppointmentsSelectionChanged {
			get { return GetEventHandler("AppointmentsSelectionChanged"); }
			set { SetEventHandler("AppointmentsSelectionChanged", value); }
		}
		#endregion
		#region AppointmentDrop
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsAppointmentDrop"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string AppointmentDrop {
			get { return GetEventHandler("AppointmentDrop"); }
			set { SetEventHandler("AppointmentDrop", value); }
		}
		#endregion
		#region AppointmentResize
		[
#if !SL
	DevExpressWebASPxSchedulerLocalizedDescription("SchedulerClientSideEventsAppointmentResize"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string AppointmentResize {
			get { return GetEventHandler("AppointmentResize"); }
			set { SetEventHandler("AppointmentResize", value); }
		}
		#endregion
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ActiveViewChanging");
			names.Add("ActiveViewChanged");
			names.Add("SelectionChanged");
			names.Add("SelectionChanging");
			names.Add("VisibleIntervalChanged");
			names.Add("AppointmentsSelectionChanged");
			names.Add("MoreButtonClicked");
			names.Add("MenuItemClicked");
			names.Add("AppointmentDrop");
			names.Add("AppointmentResize");
			names.Add("AppointmentClick");
			names.Add("AppointmentDoubleClick");
			names.Add("AppointmentDeleting");
			names.Add("MouseUp");
		}
	}
}

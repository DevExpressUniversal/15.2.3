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
using System.Text;
using System.ComponentModel;
using DevExpress.Web;
namespace DevExpress.Web {
	public class DockManagerClientSideEvents : ClientSideEvents {
		const string
		  BeforeDockEventName = "BeforeDock",
		  AfterDockEventName = "AfterDock",
		  BeforeFloatEventName = "BeforeFloat",
		  AfterFloatEventName = "AfterFloat",
		  StartPanelDraggingEventName = "StartPanelDragging",
		  EndPanelDraggingEventName = "EndPanelDragging",
		  PanelClosingEventName = "PanelClosing",
		  PanelCloseUpEventName = "PanelCloseUp",
		  PanelPopUpEventName = "PanelPopUp",
		  PanelShownEventName = "PanelShown",
		  PanelResizeEventName = "PanelResize";
		public DockManagerClientSideEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsBeforeDock"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string BeforeDock
		{
			get { return GetEventHandler(BeforeDockEventName); }
			set { SetEventHandler(BeforeDockEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsAfterDock"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string AfterDock
		{
			get { return GetEventHandler(AfterDockEventName); }
			set { SetEventHandler(AfterDockEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsBeforeFloat"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string BeforeFloat {
			get { return GetEventHandler(BeforeFloatEventName); }
			set { SetEventHandler(BeforeFloatEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsAfterFloat"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
		   typeof(System.Drawing.Design.UITypeEditor))]
		public string AfterFloat {
			get { return GetEventHandler(AfterFloatEventName); }
			set { SetEventHandler(AfterFloatEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsStartPanelDragging"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string StartPanelDragging
		{
			get { return GetEventHandler(StartPanelDraggingEventName); }
			set { SetEventHandler(StartPanelDraggingEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsEndPanelDragging"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string EndPanelDragging
		{
			get { return GetEventHandler(EndPanelDraggingEventName); }
			set { SetEventHandler(EndPanelDraggingEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsPanelClosing"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string PanelClosing
		{
			get { return GetEventHandler(PanelClosingEventName); }
			set { SetEventHandler(PanelClosingEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsPanelCloseUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string PanelCloseUp
		{
			get { return GetEventHandler(PanelCloseUpEventName); }
			set { SetEventHandler(PanelCloseUpEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsPanelPopUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string PanelPopUp
		{
			get { return GetEventHandler(PanelPopUpEventName); }
			set { SetEventHandler(PanelPopUpEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsPanelShown"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string PanelShown
		{
			get { return GetEventHandler(PanelShownEventName); }
			set { SetEventHandler(PanelShownEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockManagerClientSideEventsPanelResize"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string PanelResize
		{
			get { return GetEventHandler(PanelResizeEventName); }
			set { SetEventHandler(PanelResizeEventName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(BeforeDockEventName);
			names.Add(AfterDockEventName);
			names.Add(BeforeFloatEventName);
			names.Add(AfterFloatEventName);
			names.Add(StartPanelDraggingEventName);
			names.Add(EndPanelDraggingEventName);
			names.Add(PanelClosingEventName);
			names.Add(PanelCloseUpEventName);
			names.Add(PanelPopUpEventName);
			names.Add(PanelShownEventName);
			names.Add(PanelResizeEventName);
		}
	}
}

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
using DevExpress.Web;
using System.ComponentModel;
namespace DevExpress.Web {
	public class DockPanelClientSideEvents : PopupControlClientSideEvents {
		const string
			 BeforeDockEventName = "BeforeDock",
			 AfterDockEventName = "AfterDock",
			 BeforeFloatEventName = "BeforeFloat",
			 AfterFloatEventName = "AfterFloat",
			 StartDraggingEventName = "StartDragging",
			 EndDraggingEventName = "EndDragging";
		public DockPanelClientSideEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockPanelClientSideEventsBeforeDock"),
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
	DevExpressWebLocalizedDescription("DockPanelClientSideEventsAfterDock"),
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
	DevExpressWebLocalizedDescription("DockPanelClientSideEventsBeforeFloat"),
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
	DevExpressWebLocalizedDescription("DockPanelClientSideEventsAfterFloat"),
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
	DevExpressWebLocalizedDescription("DockPanelClientSideEventsStartDragging"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string StartDragging
		{
			get { return GetEventHandler(StartDraggingEventName); }
			set { SetEventHandler(StartDraggingEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DockPanelClientSideEventsEndDragging"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string EndDragging
		{
			get { return GetEventHandler(EndDraggingEventName); }
			set { SetEventHandler(EndDraggingEventName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(BeforeDockEventName);
			names.Add(AfterDockEventName);
			names.Add(BeforeFloatEventName);
			names.Add(AfterFloatEventName);
			names.Add(StartDraggingEventName);
			names.Add(EndDraggingEventName);
		}
	}
}

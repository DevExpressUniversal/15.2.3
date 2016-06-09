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
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web {
	public class PopupControlClientSideEvents : CallbackClientSideEventsBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsClosing"),
#endif
DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Closing {
			get { return GetEventHandler("Closing"); }
			set { SetEventHandler("Closing", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsCloseButtonClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CloseButtonClick {
			get { return GetEventHandler("CloseButtonClick"); }
			set { SetEventHandler("CloseButtonClick", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsCloseUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CloseUp {
			get { return GetEventHandler("CloseUp"); }
			set { SetEventHandler("CloseUp", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsPopUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PopUp {
			get { return GetEventHandler("PopUp"); }
			set { SetEventHandler("PopUp", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsResize"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Resize {
			get { return GetEventHandler("Resize"); }
			set { SetEventHandler("Resize", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsBeforeResizing"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BeforeResizing {
			get { return GetEventHandler("BeforeResizing"); }
			set { SetEventHandler("BeforeResizing", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsAfterResizing"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string AfterResizing {
			get { return GetEventHandler("AfterResizing"); }
			set { SetEventHandler("AfterResizing", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsShown"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Shown {
			get { return GetEventHandler("Shown"); }
			set { SetEventHandler("Shown", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("PopupControlClientSideEventsPinnedChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PinnedChanged {
			get { return GetEventHandler("PinnedChanged"); }
			set { SetEventHandler("PinnedChanged", value); }
		}
		public PopupControlClientSideEvents()
			: base() {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("CloseButtonClick");
			names.Add("PopUp");
			names.Add("Closing");
			names.Add("CloseUp");
			names.Add("Resize");
			names.Add("Shown");
			names.Add("BeforeResizing");
			names.Add("AfterResizing");
			names.Add("PinnedChanged");
		}
	}
}

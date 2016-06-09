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
using System.Text;
using System.Collections.Generic;
using DevExpress.Web;
using System.ComponentModel;
namespace DevExpress.Web {
	public class GlobalEventsClientSideEvents : ClientSideEventsBase {
		private const string ControlsInitializedEventName = "ControlsInitialized";
		private const string BrowserWindowResizedEventName = "BrowserWindowResized";
		private const string EndCallbackEventName = "EndCallback";
		private const string CallbackErrorEventName = "CallbackError";
		private const string BeginCallbackEventName = "BeginCallback";
		private const string ValidationCompletedEventName = "ValidationCompleted";
		[
#if !SL
	DevExpressWebLocalizedDescription("GlobalEventsClientSideEventsControlsInitialized"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ControlsInitialized {
			get { return GetEventHandler(ControlsInitializedEventName); }
			set { SetEventHandler(ControlsInitializedEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GlobalEventsClientSideEventsBrowserWindowResized"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BrowserWindowResized {
			get { return GetEventHandler(BrowserWindowResizedEventName); }
			set { SetEventHandler(BrowserWindowResizedEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GlobalEventsClientSideEventsEndCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string EndCallback {
			get { return GetEventHandler(EndCallbackEventName); }
			set { SetEventHandler(EndCallbackEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GlobalEventsClientSideEventsBeginCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BeginCallback {
			get { return GetEventHandler(BeginCallbackEventName); }
			set { SetEventHandler(BeginCallbackEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GlobalEventsClientSideEventsCallbackError"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CallbackError {
			get { return GetEventHandler(CallbackErrorEventName); }
			set { SetEventHandler(CallbackErrorEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("GlobalEventsClientSideEventsValidationCompleted"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ValidationCompleted {
			get { return GetEventHandler(ValidationCompletedEventName); }
			set { SetEventHandler(ValidationCompletedEventName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(ControlsInitializedEventName);
			names.Add(BrowserWindowResizedEventName);
			names.Add(BeginCallbackEventName);
			names.Add(EndCallbackEventName);
			names.Add(CallbackErrorEventName);
			names.Add(ValidationCompletedEventName);
		}
	}
}

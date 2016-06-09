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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using DevExpress.Web;
namespace DevExpress.Web {
	public class CalendarClientSideEvents : EditClientSideEvents {
		public CalendarClientSideEvents()
			: base() {
		}
		public CalendarClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsVisibleMonthChanged"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string VisibleMonthChanged {
			get { return GetEventHandler("VisibleMonthChanged"); }
			set { SetEventHandler("VisibleMonthChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsCustomDisabledDate"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomDisabledDate {
			get { return GetEventHandler("CustomDisabledDate"); }
			set { SetEventHandler("CustomDisabledDate", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsSelectionChanged"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectionChanged {
			get { return GetEventHandler("SelectionChanged"); }
			set { SetEventHandler("SelectionChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsKeyDown"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyDown {
			get { return base.KeyDown; }
			set { base.KeyDown = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsKeyPress"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyPress {
			get { return base.KeyPress; }
			set { base.KeyPress = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsKeyUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyUp {
			get { return base.KeyUp; }
			set { base.KeyUp = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsBeginCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BeginCallback {
			get { return GetEventHandler("BeginCallback"); }
			set { SetEventHandler("BeginCallback", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsEndCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string EndCallback {
			get { return GetEventHandler("EndCallback"); }
			set { SetEventHandler("EndCallback", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CalendarClientSideEventsCallbackError"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CallbackError {
			get { return GetEventHandler("CallbackError"); }
			set { SetEventHandler("CallbackError", value); }
		}
		[NotifyParentProperty(true)]
		protected internal string SelectionChanging {
			get { return GetEventHandler("SelectionChanging"); }
			set { SetEventHandler("SelectionChanging", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("VisibleMonthChanged");
			names.Add("CustomDisabledDate");
			names.Add("SelectionChanged");
			names.Add("SelectionChanging");
			names.Add("BeginCallback");
			names.Add("EndCallback");
			names.Add("CallbackError");
		}
	}
}

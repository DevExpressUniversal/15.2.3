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
	public class ListEditClientSideEvents : EditClientSideEvents {
		public ListEditClientSideEvents()
			: base() {
		}
		public ListEditClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListEditClientSideEventsSelectedIndexChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectedIndexChanged {
			get { return GetEventHandler("SelectedIndexChanged"); }
			set { SetEventHandler("SelectedIndexChanged", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("SelectedIndexChanged");
		}
	}
	public class ListBoxClientSideEvents : ListEditClientSideEvents {
		public ListBoxClientSideEvents()
			: base() {
		}
		public ListBoxClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxClientSideEventsBeginCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BeginCallback {
			get { return GetEventHandler("BeginCallback"); }
			set { SetEventHandler("BeginCallback", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxClientSideEventsEndCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string EndCallback {
			get { return GetEventHandler("EndCallback"); }
			set { SetEventHandler("EndCallback", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxClientSideEventsCallbackError"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CallbackError {
			get { return GetEventHandler("CallbackError"); }
			set { SetEventHandler("CallbackError", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemDoubleClick {
			get { return GetEventHandler("ItemDoubleClick"); }
			set { SetEventHandler("ItemDoubleClick", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxClientSideEventsKeyDown"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyDown {
			get { return base.KeyDown; }
			set { base.KeyDown = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxClientSideEventsKeyPress"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyPress {
			get { return base.KeyPress; }
			set { base.KeyPress = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ListBoxClientSideEventsKeyUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyUp {
			get { return base.KeyUp; }
			set { base.KeyUp = value; }
		}
		protected internal string ItemClick {
			get { return GetEventHandler("ItemClick"); }
			set { SetEventHandler("ItemClick", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("BeginCallback");
			names.Add("EndCallback");
			names.Add("CallbackError");
			names.Add("ItemClick");
			names.Add("ItemDoubleClick");
		}
	}
}

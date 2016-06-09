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
	public class MenuClientSideEvents: ClientSideEvents {
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuClientSideEventsItemClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemClick {
			get { return GetEventHandler("ItemClick"); }
			set { SetEventHandler("ItemClick", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuClientSideEventsItemMouseOver"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemMouseOver {
			get { return GetEventHandler("ItemMouseOver"); }
			set { SetEventHandler("ItemMouseOver", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuClientSideEventsItemMouseOut"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemMouseOut {
			get { return GetEventHandler("ItemMouseOut"); }
			set { SetEventHandler("ItemMouseOut", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuClientSideEventsPopUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PopUp {
			get { return GetEventHandler("PopUp"); }
			set { SetEventHandler("PopUp", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("MenuClientSideEventsCloseUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CloseUp {
			get { return GetEventHandler("CloseUp"); }
			set { SetEventHandler("CloseUp", value); }
		}
		public MenuClientSideEvents()
			: base() {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ItemClick");
			names.Add("ItemMouseOver");
			names.Add("ItemMouseOut");
			names.Add("PopUp");
			names.Add("CloseUp");
		}
	}
}

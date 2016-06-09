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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Web;
namespace DevExpress.Web {
	public class ButtonClientSideEvents: ClientSideEvents {
		public ButtonClientSideEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonClientSideEventsClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string Click {
			get { return GetEventHandler("Click"); }
			set { SetEventHandler("Click", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonClientSideEventsCheckedChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string CheckedChanged {
			get { return GetEventHandler("CheckedChanged"); }
			set { SetEventHandler("CheckedChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonClientSideEventsGotFocus"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string GotFocus {
			get { return GetEventHandler("GotFocus"); }
			set { SetEventHandler("GotFocus", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonClientSideEventsLostFocus"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string LostFocus {
			get { return GetEventHandler("LostFocus"); }
			set { SetEventHandler("LostFocus", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("Click");
			names.Add("GotFocus");
			names.Add("LostFocus");
			names.Add("CheckedChanged");
		}
	}
}

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
	public class TextEditClientSideEvents : EditClientSideEvents {
		public TextEditClientSideEvents()
			: base() { 
		}
		public TextEditClientSideEvents(IPropertiesOwner owner) 
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditClientSideEventsKeyDown"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyDown {
			get { return base.KeyDown; }
			set { base.KeyDown = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditClientSideEventsKeyPress"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyPress {
			get { return base.KeyPress; }
			set { base.KeyPress = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditClientSideEventsKeyUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string KeyUp {
			get { return base.KeyUp; }
			set { base.KeyUp = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditClientSideEventsTextChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string TextChanged {
			get { return GetEventHandler("TextChanged"); }
			set { SetEventHandler("TextChanged", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("TextChanged");
		}
	}
	public class TextBoxClientSideEventsBase : TextEditClientSideEvents {
		public TextBoxClientSideEventsBase()
			: base() { 
		}
		public TextBoxClientSideEventsBase(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class TextBoxClientSideEvents : TextBoxClientSideEventsBase {
		public TextBoxClientSideEvents()
			: base() { 
		}
		public TextBoxClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class ButtonEditClientSideEventsBase : TextBoxClientSideEventsBase {
		public ButtonEditClientSideEventsBase()
			: base() { 
		}
		public ButtonEditClientSideEventsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ButtonEditClientSideEventsBaseButtonClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ButtonClick {
			get { return GetEventHandler("ButtonClick"); }
			set { SetEventHandler("ButtonClick", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ButtonClick");
		}
	}
	public class ButtonEditClientSideEvents : ButtonEditClientSideEventsBase {
		public ButtonEditClientSideEvents()
			: base() { 
		}
		public ButtonEditClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
	}
}

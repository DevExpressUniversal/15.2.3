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
	public class EditClientSideEventsBase : ClientSideEvents {
		public EditClientSideEventsBase()
			: base() {
		}
		public EditClientSideEventsBase(IPropertiesOwner owner)
			: base(owner) {
		}
	}
	public class EditClientSideEvents : EditClientSideEventsBase {
		public EditClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditClientSideEventsGotFocus"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string GotFocus {
			get { return GetEventHandler("GotFocus"); }
			set { SetEventHandler("GotFocus", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditClientSideEventsLostFocus"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string LostFocus {
			get { return GetEventHandler("LostFocus"); }
			set { SetEventHandler("LostFocus", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditClientSideEventsValidation"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Validation {
			get { return GetEventHandler("Validation"); }
			set {
				SetEventHandler("Validation", value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditClientSideEventsValueChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ValueChanged {
			get { return GetEventHandler("ValueChanged"); }
			set { SetEventHandler("ValueChanged", value); }
		}
		protected internal string KeyDown {
			get { return GetEventHandler("KeyDown"); }
			set { SetEventHandler("KeyDown", value); }
		}
		protected internal string KeyPress {
			get { return GetEventHandler("KeyPress"); }
			set { SetEventHandler("KeyPress", value); }
		}
		protected internal string KeyUp {
			get { return GetEventHandler("KeyUp"); }
			set { SetEventHandler("KeyUp", value); }
		}
		public EditClientSideEvents()
			: base() { 
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("GotFocus");
			names.Add("LostFocus");
			names.Add("Validation");
			names.Add("ValueChanged");
			names.Add("KeyDown");
			names.Add("KeyPress");
			names.Add("KeyUp");
		}
	}
}

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
using System.Web.UI;
namespace DevExpress.Web {
	public class DropDownClientSideEvents : ButtonEditClientSideEventsBase {
		public DropDownClientSideEvents()
			: base() {
		}
		public DropDownClientSideEvents(IPropertiesOwner owner) 
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownClientSideEventsDropDown"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DropDown {
			get { return GetEventHandler("DropDown"); }
			set { SetEventHandler("DropDown", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownClientSideEventsCloseUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CloseUp {
			get { return GetEventHandler("CloseUp"); }
			set { SetEventHandler("CloseUp", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownClientSideEventsQueryCloseUp"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string QueryCloseUp {
			get {
				return GetEventHandler("QueryCloseUp");
			}
			set {
				SetEventHandler("QueryCloseUp", value);
			}
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("DropDown");
			names.Add("CloseUp");
			names.Add("QueryCloseUp");
		}
	}
	public class DateEditClientSideEvents : DropDownClientSideEvents {		
		public DateEditClientSideEvents()
			: base() {
		}
		public DateEditClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditClientSideEventsDateChanged"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DateChanged {
			get { return GetEventHandler("DateChanged"); }
			set { SetEventHandler("DateChanged", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditClientSideEventsCalendarCustomDisabledDate"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CalendarCustomDisabledDate {
			get { return GetEventHandler("CalendarCustomDisabledDate"); }
			set { SetEventHandler("CalendarCustomDisabledDate", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DateEditClientSideEventsParseDate"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ParseDate {
			get { return GetEventHandler("ParseDate"); }
			set { SetEventHandler("ParseDate", value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string TextChanged {
			get { return ""; }
			set { }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("DateChanged");
			names.Add("ParseDate");
			names.Add("CalendarCustomDisabledDate");
		}
	}
	public class AutoCompleteBoxClientSideEvents : DropDownClientSideEvents {
		public AutoCompleteBoxClientSideEvents()
			: base() {
		}
		public AutoCompleteBoxClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxClientSideEventsBeginCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BeginCallback
		{
			get { return GetEventHandler("BeginCallback"); }
			set { SetEventHandler("BeginCallback", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxClientSideEventsEndCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string EndCallback
		{
			get { return GetEventHandler("EndCallback"); }
			set { SetEventHandler("EndCallback", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxClientSideEventsCallbackError"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CallbackError
		{
			get { return GetEventHandler("CallbackError"); }
			set { SetEventHandler("CallbackError", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("BeginCallback");
			names.Add("EndCallback");
			names.Add("CallbackError");
		}
	}
	public class ComboBoxClientSideEvents : AutoCompleteBoxClientSideEvents {
		public ComboBoxClientSideEvents()
			: base() {
		}
		public ComboBoxClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ComboBoxClientSideEventsSelectedIndexChanged"),
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
	public class TokenBoxClientSideEvents : AutoCompleteBoxClientSideEvents {
		public TokenBoxClientSideEvents()
			: base() {
		}
		public TokenBoxClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxClientSideEventsTokensChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string TokensChanged {
			get { return GetEventHandler("TokensChanged"); }
			set { SetEventHandler("TokensChanged", value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string ButtonClick {
			get { return base.ButtonClick; }
			set { base.ButtonClick = value; }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("TokensChanged");
		}
	}
	public class ColorEditClientSideEvents : DropDownClientSideEvents {
		public ColorEditClientSideEvents()
			: base() {
		}
		public ColorEditClientSideEvents(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ColorEditClientSideEventsColorChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColorChanged {
			get { return GetEventHandler("ColorChanged"); }
			set { SetEventHandler("ColorChanged", value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string TextChanged {
			get { return ""; }
			set {  }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ColorChanged");
		}
	}
}

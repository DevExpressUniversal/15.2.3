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
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class ColorButtonClientSideEvents : ClientSideEvents {
		public ColorButtonClientSideEvents() 
			: base(){
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string ColorChanged {
			get { return GetEventHandler("ColorChanged"); }
			set { SetEventHandler("ColorChanged", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ColorChanged");
		}
	}
	public class ItemPickerButtonClientSideEvents : ClientSideEvents {
		public ItemPickerButtonClientSideEvents()
			: base() {
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string ItemPickerItemClick {
			get { return GetEventHandler("ItemPickerItemClick"); }
			set { SetEventHandler("ItemPickerItemClick", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ItemPickerItemClick");
		}
	}
	public class ToolbarClientSideEvents : MenuClientSideEvents {
		public ToolbarClientSideEvents()
			: base() {
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Command {
			get { return GetEventHandler("Command"); }
			set { SetEventHandler("Command", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DropDownItemBeforeFocus {
			get { return GetEventHandler("DropDownItemBeforeFocus"); }
			set { SetEventHandler("DropDownItemBeforeFocus", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DropDownItemCloseUp {
			get { return GetEventHandler("DropDownItemCloseUp"); }
			set { SetEventHandler("DropDownItemCloseUp", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomComboBoxInit {
			get { return GetEventHandler("CustomComboBoxInit"); }
			set { SetEventHandler("CustomComboBoxInit", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("Command");
			names.Add("DropDownItemBeforeFocus");
			names.Add("DropDownItemCloseUp");
			names.Add("CustomComboBoxInit");
		}
	}
	public class BarDocControlClientSideEvents : ClientSideEvents {
		public BarDocControlClientSideEvents()
			: base() {
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string Command {
			get { return GetEventHandler("Command"); }
			set { SetEventHandler("Command", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DropDownItemBeforeFocus {
			get { return GetEventHandler("DropDownItemBeforeFocus"); }
			set { SetEventHandler("DropDownItemBeforeFocus", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DropDownItemCloseUp {
			get { return GetEventHandler("DropDownItemCloseUp"); }
			set { SetEventHandler("DropDownItemCloseUp", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomComboBoxInit {
			get { return GetEventHandler("CustomComboBoxInit"); }
			set { SetEventHandler("CustomComboBoxInit", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("Command");
			names.Add("DropDownItemBeforeFocus");
			names.Add("DropDownItemCloseUp");
			names.Add("CustomComboBoxInit");
		}
	}
	public class ToolbarComboBoxClientSideEvents : ComboBoxClientSideEvents {
		public ToolbarComboBoxClientSideEvents()
			: base() {
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BeforeFocus {
			get { return GetEventHandler("BeforeFocus"); }
			set { SetEventHandler("BeforeFocus", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ItemClick {
			get { return GetEventHandler("ItemClick"); }
			set { SetEventHandler("ItemClick", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("BeforeFocus");
			names.Add("ItemClick");
		}
	}
}

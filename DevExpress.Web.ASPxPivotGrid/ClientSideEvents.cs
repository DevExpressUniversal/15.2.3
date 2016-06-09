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
using DevExpress.Utils.Serializing;
namespace DevExpress.Web.ASPxPivotGrid {
	public class PivotGridClientSideEvents : ClientSideEvents {
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridClientSideEventsCustomizationFieldsVisibleChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string CustomizationFieldsVisibleChanged {
			get { return GetEventHandler("CustomizationFieldsVisibleChanged"); }
			set { SetEventHandler("CustomizationFieldsVisibleChanged", value); }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridClientSideEventsCellClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string CellClick {
			get { return GetEventHandler("CellClick"); }
			set { SetEventHandler("CellClick", value); }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridClientSideEventsCellDblClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string CellDblClick {
			get { return GetEventHandler("CellDblClick"); }
			set { SetEventHandler("CellDblClick", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string BeginCallback {
			get { return GetEventHandler("BeginCallback"); }
			set { SetEventHandler("BeginCallback", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string EndCallback {
			get { return GetEventHandler("EndCallback"); }
			set { SetEventHandler("EndCallback", value); }
		}
		[
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CallbackError {
			get { return GetEventHandler("CallbackError"); }
			set { SetEventHandler("CallbackError", value); }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridClientSideEventsPopupMenuItemClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public string PopupMenuItemClick {
			get { return GetEventHandler("PopupMenuItemClick"); }
			set { SetEventHandler("PopupMenuItemClick", value); }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridClientSideEventsInit"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty()]
		public new string Init {
			get { return GetEventHandler("Init"); }
			set { SetEventHandler("Init", value); }
		}
		#region Obsoleted
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridClientSideEventsBeforeCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),		
		XtraSerializableProperty(),
		Obsolete("Use the BeginCallback property instead")]
		public string BeforeCallback {
			get { return GetEventHandler("BeforeCallback"); }
			set { SetEventHandler("BeforeCallback", value); }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("PivotGridClientSideEventsAfterCallback"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(),
		Obsolete("Use the EndCallback property instead")]
		public string AfterCallback {
			get { return GetEventHandler("AfterCallback"); }
			set { SetEventHandler("AfterCallback", value); }
		}
		void AddObsoletedNames(List<string> names) {
			names.Add("BeforeCallback");
			names.Add("AfterCallback");
		}
		#endregion
		public PivotGridClientSideEvents()
			: base() {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("CustomizationFieldsVisibleChanged");
			names.Add("CellClick");
			names.Add("CellDblClick");
			names.Add("BeginCallback");
			names.Add("EndCallback");
			names.Add("CallbackError");
			names.Add("PopupMenuItemClick");
			AddObsoletedNames(names);
		}
	}
}

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
namespace DevExpress.Web {
	public class GridClientSideEvents : CallbackClientSideEventsBase {
		public GridClientSideEvents()
			: base() {
		}
		protected internal string CustomizationWindowCloseUp { get { return GetEventHandler("CustomizationWindowCloseUp"); } set { SetEventHandler("CustomizationWindowCloseUp", value); } }
		protected internal string SelectionChanged { get { return GetEventHandler("SelectionChanged"); } set { SetEventHandler("SelectionChanged", value); } }
		protected internal string ColumnSorting { get { return GetEventHandler("ColumnSorting"); } set { SetEventHandler("ColumnSorting", value); } }
		protected internal string CustomButtonClick { get { return GetEventHandler("CustomButtonClick"); } set { SetEventHandler("CustomButtonClick", value); } }
		protected internal string BatchEditStartEditing { get { return GetEventHandler("BatchEditStartEditing"); } set { SetEventHandler("BatchEditStartEditing", value); } }
		protected internal string BatchEditEndEditing { get { return GetEventHandler("BatchEditEndEditing"); } set { SetEventHandler("BatchEditEndEditing", value); } }
		protected internal string BatchEditConfirmShowing { get { return GetEventHandler("BatchEditConfirmShowing"); } set { SetEventHandler("BatchEditConfirmShowing", value); } }
		protected internal string BatchEditTemplateCellFocused { get { return GetEventHandler("BatchEditTemplateCellFocused"); } set { SetEventHandler("BatchEditTemplateCellFocused", value); } }
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.AddRange(new string[] {
				"SelectionChanged",
				"ColumnSorting",
				"CustomizationWindowCloseUp",
				"CustomButtonClick",
				"BatchEditStartEditing",
				"BatchEditEndEditing",
				"BatchEditConfirmShowing",
				"BatchEditTemplateCellFocused"
			});
		}
	}
	public class GridViewClientSideEvents : GridClientSideEvents {
		public GridViewClientSideEvents() : base() { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsSelectionChanged"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string SelectionChanged { get { return base.SelectionChanged; } set { base.SelectionChanged = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsCustomizationWindowCloseUp"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string CustomizationWindowCloseUp { get { return base.CustomizationWindowCloseUp; } set { base.CustomizationWindowCloseUp = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsColumnSorting"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string ColumnSorting { get { return base.ColumnSorting; } set { base.ColumnSorting = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsCustomButtonClick"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string CustomButtonClick { get { return base.CustomButtonClick; } set { base.CustomButtonClick = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsBatchEditStartEditing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string BatchEditStartEditing { get { return base.BatchEditStartEditing; } set { base.BatchEditStartEditing = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsBatchEditEndEditing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string BatchEditEndEditing { get { return base.BatchEditEndEditing; } set { base.BatchEditEndEditing = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsBatchEditConfirmShowing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string BatchEditConfirmShowing { get { return base.BatchEditConfirmShowing; } set { base.BatchEditConfirmShowing = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsBatchEditTemplateCellFocused"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public new string BatchEditTemplateCellFocused { get { return base.BatchEditTemplateCellFocused; } set { base.BatchEditTemplateCellFocused = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsFocusedRowChanged"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FocusedRowChanged { get { return GetEventHandler("FocusedRowChanged"); } set { SetEventHandler("FocusedRowChanged", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsRowClick"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string RowClick { get { return GetEventHandler("RowClick"); } set { SetEventHandler("RowClick", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsRowDblClick"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string RowDblClick { get { return GetEventHandler("RowDblClick"); } set { SetEventHandler("RowDblClick", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsContextMenu"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ContextMenu { get { return GetEventHandler("ContextMenu"); } set { SetEventHandler("ContextMenu", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsContextMenuItemClick"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ContextMenuItemClick { get { return GetEventHandler("ContextMenuItemClick"); } set { SetEventHandler("ContextMenuItemClick", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsColumnGrouping"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnGrouping { get { return GetEventHandler("ColumnGrouping"); } set { SetEventHandler("ColumnGrouping", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsColumnMoving"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnMoving { get { return GetEventHandler("ColumnMoving"); } set { SetEventHandler("ColumnMoving", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsColumnStartDragging"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnStartDragging { get { return GetEventHandler("ColumnStartDragging"); } set { SetEventHandler("ColumnStartDragging", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsColumnResizing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnResizing { get { return GetEventHandler("ColumnResizing"); } set { SetEventHandler("ColumnResizing", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsColumnResized"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnResized { get { return GetEventHandler("ColumnResized"); } set { SetEventHandler("ColumnResized", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsRowExpanding"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string RowExpanding { get { return GetEventHandler("RowExpanding"); } set { SetEventHandler("RowExpanding", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsRowCollapsing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string RowCollapsing { get { return GetEventHandler("RowCollapsing"); } set { SetEventHandler("RowCollapsing", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsDetailRowExpanding"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DetailRowExpanding { get { return GetEventHandler("DetailRowExpanding"); } set { SetEventHandler("DetailRowExpanding", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsDetailRowCollapsing"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DetailRowCollapsing { get { return GetEventHandler("DetailRowCollapsing"); } set { SetEventHandler("DetailRowCollapsing", value); } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewClientSideEventsBatchEditRowValidating"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BatchEditRowValidating { get { return GetEventHandler("BatchEditRowValidating"); } set { SetEventHandler("BatchEditRowValidating", value); } }
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.AddRange(new string[] {
				"FocusedRowChanged",
				"ColumnGrouping",
				"ColumnResizing",
				"ColumnResized",
				"ColumnMoving",
				"ColumnStartDragging", 
				"RowExpanding",
				"RowCollapsing",
				"DetailRowExpanding",
				"DetailRowCollapsing",
				"RowClick",
				"RowDblClick",
				"ContextMenu",
				"ContextMenuItemClick",
				"BatchEditRowValidating"
			});
		}
   }
}

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
using DevExpress.Web;
using DevExpress.Web.ASPxTreeList.Internal;
namespace DevExpress.Web.ASPxTreeList {
	public class TreeListClientSideEvents : CallbackClientSideEventsBase {
		public TreeListClientSideEvents() 
			: base() {
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsFocusedNodeChanged"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string FocusedNodeChanged {
			get { return GetEventHandler("FocusedNodeChanged"); }
			set { SetEventHandler("FocusedNodeChanged", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsSelectionChanged"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectionChanged {
			get { return GetEventHandler("SelectionChanged"); }
			set { SetEventHandler("SelectionChanged", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsCustomizationWindowCloseUp"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomizationWindowCloseUp {
			get { return GetEventHandler("CustomizationWindowCloseUp"); }
			set { SetEventHandler("CustomizationWindowCloseUp", value); }
		}
#if !SL
	[DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsCustomDataCallback")]
#endif
		public new string CustomDataCallback {
			get { return base.CustomDataCallback; }
			set { base.CustomDataCallback = value; }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsNodeClick"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string NodeClick {
			get { return GetEventHandler("NodeClick"); }
			set { SetEventHandler("NodeClick", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsNodeDblClick"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string NodeDblClick {
			get { return GetEventHandler("NodeDblClick"); }
			set { SetEventHandler("NodeDblClick", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsContextMenu"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ContextMenu {
			get { return GetEventHandler("ContextMenu"); }
			set { SetEventHandler("ContextMenu", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsStartDragNode"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string StartDragNode {
			get { return GetEventHandler("StartDragNode"); }
			set { SetEventHandler("StartDragNode", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsEndDragNode"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string EndDragNode {
			get { return GetEventHandler("EndDragNode"); }
			set { SetEventHandler("EndDragNode", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsCustomButtonClick"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomButtonClick {
			get { return GetEventHandler("CustomButtonClick"); }
			set { SetEventHandler("CustomButtonClick", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsNodeFocusing"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string NodeFocusing {
			get { return GetEventHandler("NodeFocusing"); }
			set { SetEventHandler("NodeFocusing", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsNodeExpanding"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string NodeExpanding {
			get { return GetEventHandler("NodeExpanding"); }
			set { SetEventHandler("NodeExpanding", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsNodeCollapsing"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string NodeCollapsing {
			get { return GetEventHandler("NodeCollapsing"); }
			set { SetEventHandler("NodeCollapsing", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsColumnResizing"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnResizing {
			get { return GetEventHandler("ColumnResizing"); }
			set { SetEventHandler("ColumnResizing", value); }
		}
		[
#if !SL
	DevExpressWebASPxTreeListLocalizedDescription("TreeListClientSideEventsColumnResized"),
#endif
		NotifyParentProperty(true), Localizable(false), DefaultValue(""), Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string ColumnResized {
			get { return GetEventHandler("ColumnResized"); }
			set { SetEventHandler("ColumnResized", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("FocusedNodeChanged");
			names.Add("SelectionChanged");
			names.Add("CustomizationWindowCloseUp");
			names.Add("NodeClick");
			names.Add("NodeDblClick");
			names.Add("ContextMenu");
			names.Add("StartDragNode");
			names.Add("EndDragNode");
			names.Add("CustomButtonClick");
			names.Add("NodeFocusing");
			names.Add("NodeExpanding");
			names.Add("NodeCollapsing");
			names.Add("ColumnResizing");
			names.Add("ColumnResized");
		}
	}
}

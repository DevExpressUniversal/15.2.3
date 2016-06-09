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
	public class SplitterClientSideEvents: ClientSideEvents {
		public SplitterClientSideEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterClientSideEventsPaneResizing"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PaneResizing {
			get { return GetEventHandler("PaneResizing"); }
			set { SetEventHandler("PaneResizing", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterClientSideEventsPaneResized"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PaneResized {
			get { return GetEventHandler("PaneResized"); }
			set { SetEventHandler("PaneResized", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterClientSideEventsPaneResizeCompleted"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PaneResizeCompleted {
			get { return GetEventHandler("PaneResizeCompleted"); }
			set { SetEventHandler("PaneResizeCompleted", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterClientSideEventsPaneCollapsing"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PaneCollapsing {
			get { return GetEventHandler("PaneCollapsing"); }
			set { SetEventHandler("PaneCollapsing", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterClientSideEventsPaneCollapsed"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PaneCollapsed {
			get { return GetEventHandler("PaneCollapsed"); }
			set { SetEventHandler("PaneCollapsed", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterClientSideEventsPaneExpanding"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PaneExpanding {
			get { return GetEventHandler("PaneExpanding"); }
			set { SetEventHandler("PaneExpanding", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterClientSideEventsPaneExpanded"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PaneExpanded {
			get { return GetEventHandler("PaneExpanded"); }
			set { SetEventHandler("PaneExpanded", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("SplitterClientSideEventsPaneContentUrlLoaded"),
#endif
DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string PaneContentUrlLoaded
		{
			get { return GetEventHandler("PaneContentUrlLoaded"); }
			set { SetEventHandler("PaneContentUrlLoaded", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("PaneResizing");
			names.Add("PaneResized");
			names.Add("PaneResizeCompleted");
			names.Add("PaneCollapsing");
			names.Add("PaneCollapsed");
			names.Add("PaneExpanding");
			names.Add("PaneExpanded");
			names.Add("PaneContentUrlLoaded");
		}
	}
}

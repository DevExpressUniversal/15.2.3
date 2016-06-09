#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
namespace DevExpress.XtraReports.Web.DocumentViewer {
	public class DocumentViewerClientSideEvents : CallbackClientSideEventsBase {
		internal const string
			ToolbarItemValueChangedName = "ToolbarItemValueChanged",
			ToolbarItemClickName = "ToolbarItemClick",
			RibbonCommandExecutedName = "RibbonCommandExecuted",
			PageLoadName = "PageLoad";
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerClientSideEventsToolbarItemValueChanged")]
#endif
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ToolbarItemValueChanged {
			get { return GetEventHandler(ToolbarItemValueChangedName); }
			set { SetEventHandler(ToolbarItemValueChangedName, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerClientSideEventsToolbarItemClick")]
#endif
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string ToolbarItemClick {
			get { return base.GetEventHandler(ToolbarItemClickName); }
			set { base.SetEventHandler(ToolbarItemClickName, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerClientSideEventsPageLoad")]
#endif
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public string PageLoad {
			get { return base.GetEventHandler(PageLoadName); }
			set { base.SetEventHandler(PageLoadName, value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(ToolbarItemValueChangedName);
			names.Add(ToolbarItemClickName);
			names.Add(PageLoadName);
		}
	}
}

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
namespace DevExpress.Web {
	public class TreeViewClientSideEvents : CallbackClientSideEventsBase {
		const string
			NodeClickEventName = "NodeClick",
			ExpandedChangedEventName = "ExpandedChanged",
			ExpandedChangingEventName = "ExpandedChanging",
			CheckedChangedEventName = "CheckedChanged";
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewClientSideEventsNodeClick"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string NodeClick {
			get { return GetEventHandler(NodeClickEventName); }
			set { SetEventHandler(NodeClickEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewClientSideEventsExpandedChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string ExpandedChanged {
			get { return GetEventHandler(ExpandedChangedEventName); }
			set { SetEventHandler(ExpandedChangedEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewClientSideEventsExpandedChanging"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string ExpandedChanging {
			get { return GetEventHandler(ExpandedChangingEventName); }
			set { SetEventHandler(ExpandedChangingEventName, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TreeViewClientSideEventsCheckedChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
			typeof(System.Drawing.Design.UITypeEditor))]
		public string CheckedChanged {
			get { return GetEventHandler(CheckedChangedEventName); }
			set { SetEventHandler(CheckedChangedEventName, value); }
		}
		public TreeViewClientSideEvents()
			: base() {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add(NodeClickEventName);
			names.Add(ExpandedChangedEventName);
			names.Add(ExpandedChangingEventName);
			names.Add(CheckedChangedEventName);
		}
	}
}

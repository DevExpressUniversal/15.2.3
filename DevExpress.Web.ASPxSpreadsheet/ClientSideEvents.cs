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
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SpreadsheetClientSideEvents : CallbackClientSideEventsBase {
		public SpreadsheetClientSideEvents()
			: base() {
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetClientSideEventsSelectionChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string SelectionChanged {
			get { return GetEventHandler("SelectionChanged"); }
			set { SetEventHandler("SelectionChanged", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetClientSideEventsCustomCommandExecuted"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string CustomCommandExecuted {
			get { return GetEventHandler("CustomCommandExecuted"); }
			set { SetEventHandler("CustomCommandExecuted", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetClientSideEventsDocumentChanged"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string DocumentChanged {
			get { return GetEventHandler("DocumentChanged"); }
			set { SetEventHandler("DocumentChanged", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetClientSideEventsBeginSynchronization"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string BeginSynchronization {
			get { return GetEventHandler("BeginSynchronization"); }
			set { SetEventHandler("BeginSynchronization", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpreadsheetLocalizedDescription("SpreadsheetClientSideEventsEndSynchronization"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string EndSynchronization {
			get { return GetEventHandler("EndSynchronization"); }
			set { SetEventHandler("EndSynchronization", value); }
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("SelectionChanged");
			names.Add("CustomCommandExecuted");
			names.Add("DocumentChanged");
			names.Add("BeginSynchronization");
			names.Add("EndSynchronization");
		}
	}
}

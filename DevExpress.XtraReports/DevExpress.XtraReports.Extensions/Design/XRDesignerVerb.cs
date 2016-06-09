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
using DevExpress.XtraReports.UserDesigner;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.Design {
	public class XRDesignerVerbBase : DesignerVerb {
		bool includeInSmartTag;
		public bool IncludeInSmartTag { get { return includeInSmartTag; } }
		public XRDesignerVerbBase(string text, EventHandler handler, bool visible, bool includeInSmartTag)
			: base(text, handler) {
			Visible = visible;
			this.includeInSmartTag = includeInSmartTag;
		}
		public XRDesignerVerbBase(string text, EventHandler handler, CommandID commandID ,bool visible, bool includeInSmartTag)
			: base(text, handler, commandID) {
			Visible = visible;
			this.includeInSmartTag = includeInSmartTag;
		}
		public override void Invoke() {
			try {
				base.Invoke();
			} catch { }
		}
	}
	public class XRDesignerVerb : XRDesignerVerbBase {
		public XRDesignerVerb(string text, EventHandler handler, ReportCommand reportCommand)
			: this(text, handler, reportCommand, true, true) {
		}
		public XRDesignerVerb(string text, EventHandler handler, ReportCommand reportCommand, bool visible, bool includeInSmartTag)
			: base(text, handler, Commands.CommandIDReportCommandConverter.GetCommandID(reportCommand, StandardCommands.VerbFirst), visible, includeInSmartTag) {
		}
		public XRDesignerVerb(string text, EventHandler handler, CommandID commandID, bool visible, bool includeInSmartTag)
			: base(text, handler, commandID, visible, includeInSmartTag) {
		}
	}
}

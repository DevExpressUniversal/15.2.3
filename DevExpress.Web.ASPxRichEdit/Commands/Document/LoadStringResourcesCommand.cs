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

using DevExpress.Office.Localization;
using DevExpress.XtraRichEdit.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class LoadStringResourcesCommand : WebRichEditLoadModelCommandBase {
		public LoadStringResourcesCommand(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters)
			: base(parentCommand, parameters) { }
		public override CommandType Type { get { return CommandType.LoadStringResources; } }
		protected override void FillHashtable(Hashtable result) {
			result["header"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_PageHeader);
			result["footer"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_PageFooter);
			result["firstPageHeader"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_FirstPageHeader);
			result["firstPageFooter"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_FirstPageFooter);
			result["evenPageHeader"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_EvenPageHeader);
			result["evenPageFooter"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_EvenPageFooter);
			result["oddPageHeader"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_OddPageHeader);
			result["oddPageFooter"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_OddPageFooter);
			result["sameAsPrevious"] = XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_SameAsPrevious);
		}
		protected override bool IsEnabled() {
			return true;
		}
	}
}

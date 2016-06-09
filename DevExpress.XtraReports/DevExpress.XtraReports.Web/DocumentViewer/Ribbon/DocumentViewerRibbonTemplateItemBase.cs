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

using DevExpress.Web;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native;
namespace DevExpress.XtraReports.Web.DocumentViewer.Ribbon {
	public class DocumentViewerRibbonTemplateItemBase : RibbonTemplateItem {
		string commandId = null;
		protected virtual string CommandName { get { return RibbonCommandService.CommandPrefix + "RibbonTemplateItem"; } }
		protected virtual string DefaultGroupName { get { return string.Empty; } }
		protected virtual string DefaultToolTip { get { return string.Empty; } }
		protected virtual RibbonItemSize DefaultItemSize {
			get { return RibbonItemSize.Small; }
		}
		protected override string GetName() {
			return commandId == null ? commandId = RibbonCommandService.GetCommandID(CommandName) : commandId;
		}
		protected override string GetText() {
			return string.Empty;
		}
		protected override string GetToolTip() {
			if(!string.IsNullOrEmpty(base.ToolTip))
				return base.GetToolTip();
			return DefaultToolTip;
		}
		protected override string GetSubGroupName() {
			if(!string.IsNullOrEmpty(base.SubGroupName))
				return base.GetSubGroupName();
			return DefaultGroupName;
		}
		protected override RibbonItemSize GetSize() {
			return DefaultItemSize;
		}
	}
}

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

using DevExpress.Utils.Internal;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit {
	public class RERToggleShowHorizontalRulerCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleShowHorizontalRuler; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleShowHorizontalRuler); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleShowHorizontalRulerDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.RulerHorizontal; } }
		public RERToggleShowHorizontalRulerCommand() 
			: base() { }
		public RERToggleShowHorizontalRulerCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleFullScreenCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.FullScreen; } }
		protected override string DefaultText { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_ToggleFullScreen); } }
		protected override string DefaultToolTip { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_ToggleFullScreenDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.FullScreen; } }
		public RERToggleFullScreenCommand() 
			: base() { }
		public RERToggleFullScreenCommand(RibbonItemSize size)
			: base(size) { }
	}
}

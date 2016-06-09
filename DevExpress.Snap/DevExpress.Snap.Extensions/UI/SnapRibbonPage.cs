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

using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.Snap.Extensions.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraBars.Ribbon;
using DevExpress.Utils;
namespace DevExpress.Snap.Extensions.UI {
	public class AppearanceRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.Caption_Appearance); } }
		protected override XtraBars.Ribbon.RibbonPage CreatePage() {
			return new AppearanceRibbonPage();
		}
	}
	public class DataToolsRibbonPageCategory : ControlCommandBasedRibbonPageCategory<RichEditControl, RichEditCommandId> {
		public DataToolsRibbonPageCategory() {
			this.Color = DXColor.FromArgb(0xff, 0x1f, 0xa6, 0xcf);
		}
		public override string DefaultText { get { return SnapExtensionsLocalizer.GetString(SnapExtensionsStringId.Caption_Data); } }
		protected override RichEditCommandId EmptyCommandId { get { return RichEditCommandId.None; } }
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new DataToolsRibbonPageCategory();
		}
	}
}

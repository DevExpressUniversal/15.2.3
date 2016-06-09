﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region SetPortraitPageOrientationCommand
	public class SetPortraitPageOrientationCommand : ToggleChangeSectionFormattingCommandBase<bool> {
		public SetPortraitPageOrientationCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetPortraitPageOrientationCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.SetPortraitPageOrientation; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetPortraitPageOrientationCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SetPortraitPageOrientation; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SetPortraitPageOrientationCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SetPortraitPageOrientationDescription; } }
		#endregion
		protected internal override SectionPropertyModifier<bool> CreateModifier(ICommandUIState state) {
			return new SectionPageOrientationLandscapeModifier(false);
		}
		protected internal override bool IsCheckedValue(bool value) {
			return !value;
		}
	}
	#endregion
}

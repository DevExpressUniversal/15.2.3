#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.HtmlPropertyEditor.Win {
	public enum HtmlEditorControlId {
		BoldHint,
		BackcolorHint,
		CutHint,
		CopyHint,
		FontHint,
		FontCaption,
		FontSizeCaption,
		FontSizeHint,
		ForecolorHint,
		IndentHint,
		InsertHyperlinkHint,
		InsertImageHint,
		InsertOrderedListHint,
		InsertUnorderedListHint,
		ItalicHint,
		JustifyCenterHint,
		JustifyFullHint,
		JustifyLeftHint,
		JustifyRightHint,
		OutdentHint,
		PasteHint,
		RedoHint,
		RemoveFormatingHint,
		RemoveHyperlinkHint,
		StrikeoutHint,
		StylesCaption,
		StylesHint,
		SubscriptHint,
		SuperscriptHint,
		UnderlineHint,
		UndoHint,
		TabHtmlCaption,
		TabDesignCaption,
		StyleNormalCaption,
		StyleHeadingCaption,
		StyleAddressCaption
	}
	[System.ComponentModel.DisplayName("HtmlEditor Control")]
	public class HtmlEditorControlLocalizer : XafResourceLocalizer<HtmlEditorControlId> {
		private static HtmlEditorControlLocalizer activeLocalizer;
		static HtmlEditorControlLocalizer() {
			activeLocalizer = new HtmlEditorControlLocalizer();
		}
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			return new XafResourceManagerParameters(
				"HtmlEditor",
				"DevExpress.ExpressApp.HtmlPropertyEditor.Win.Localization",
				String.Empty,
				GetType().Assembly
				);
		}
		public static HtmlEditorControlLocalizer Active {
			get { return activeLocalizer; }
			set { activeLocalizer = value; }
		}
		public override void Activate() {
			Active = this;
		}
	}
}

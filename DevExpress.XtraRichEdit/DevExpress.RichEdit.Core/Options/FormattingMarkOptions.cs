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
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraRichEdit {
	#region RichEditFormattingMarkVisibility
	[ComVisible(true)]
	public enum RichEditFormattingMarkVisibility {
		Auto,
		Visible,
		Hidden
	}
	#endregion
	#region FormattingMarkVisibilityOptions
	[ComVisible(true)]
	public class FormattingMarkVisibilityOptions : RichEditNotificationOptions {
		#region Fields
		const RichEditFormattingMarkVisibility defaultParagraphMarkVisibility = RichEditFormattingMarkVisibility.Auto;
		const RichEditFormattingMarkVisibility defaultTabCharacterVisibility = RichEditFormattingMarkVisibility.Auto;
		const RichEditFormattingMarkVisibility defaultSpaceVisibility = RichEditFormattingMarkVisibility.Auto;
		const RichEditFormattingMarkVisibility defaultHiddenTextVisibility = RichEditFormattingMarkVisibility.Auto;
		const RichEditFormattingMarkVisibility defaultSeparatorVisibility = RichEditFormattingMarkVisibility.Auto;
		RichEditFormattingMarkVisibility paragraphMark;
		RichEditFormattingMarkVisibility tabCharacter;
		RichEditFormattingMarkVisibility space;
		RichEditFormattingMarkVisibility separator;
		RichEditFormattingMarkVisibility hiddenText;
		bool showHiddenText;
		#endregion
		#region Properties
		#region ParagraphMark
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("FormattingMarkVisibilityOptionsParagraphMark"),
#endif
NotifyParentProperty(true), DefaultValue(defaultParagraphMarkVisibility), XtraSerializableProperty()]
		public RichEditFormattingMarkVisibility ParagraphMark {
			get { return paragraphMark; }
			set {
				if (ParagraphMark == value)
					return;
				RichEditFormattingMarkVisibility oldValue = ParagraphMark;
				paragraphMark = value;
				OnChanged("ParagraphMark", oldValue, value);
			}
		}
		#endregion
		#region TabCharacter
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("FormattingMarkVisibilityOptionsTabCharacter"),
#endif
NotifyParentProperty(true), DefaultValue(defaultTabCharacterVisibility), XtraSerializableProperty()]
		public RichEditFormattingMarkVisibility TabCharacter {
			get { return tabCharacter; }
			set {
				if (TabCharacter == value)
					return;
				RichEditFormattingMarkVisibility oldValue = TabCharacter;
				tabCharacter = value;
				OnChanged("TabCharacter", oldValue, value);
			}
		}
		#endregion
		#region Space
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("FormattingMarkVisibilityOptionsSpace"),
#endif
NotifyParentProperty(true), DefaultValue(defaultSpaceVisibility), XtraSerializableProperty()]
		public RichEditFormattingMarkVisibility Space {
			get { return space; }
			set {
				if (Space == value)
					return;
				RichEditFormattingMarkVisibility oldValue = Space;
				space = value;
				OnChanged("Space", oldValue, value);
			}
		}
		#endregion
		#region Separator
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("FormattingMarkVisibilityOptionsSeparator"),
#endif
NotifyParentProperty(true), DefaultValue(defaultSpaceVisibility), XtraSerializableProperty()]
		public RichEditFormattingMarkVisibility Separator {
			get { return separator; }
			set {
				if (Separator == value)
					return;
				RichEditFormattingMarkVisibility oldValue = Separator;
				separator = value;
				OnChanged("Separator", oldValue, value);
			}
		}
		#endregion
		#region HiddenText
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("FormattingMarkVisibilityOptionsHiddenText"),
#endif
NotifyParentProperty(true), DefaultValue(defaultHiddenTextVisibility), XtraSerializableProperty()]
		public RichEditFormattingMarkVisibility HiddenText {
			get { return hiddenText; }
			set {
				if (HiddenText == value)
					return;
				RichEditFormattingMarkVisibility oldValue = HiddenText;
				hiddenText = value;
				OnChanged("HiddenText", oldValue, value);
			}
		}
		#endregion
		#region ShowHiddenText
		[NotifyParentProperty(true)]
		internal bool ShowHiddenText {
			get { return showHiddenText; }
			set {
				if (ShowHiddenText == value)
					return;
				showHiddenText = value;
				OnChanged("ShowHiddenText", !value, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			ParagraphMark = defaultParagraphMarkVisibility;
			TabCharacter = defaultTabCharacterVisibility;
			Space = defaultSpaceVisibility;
			Separator = defaultSeparatorVisibility;
			HiddenText = defaultHiddenTextVisibility;
			ShowHiddenText = false;
		}
	}
	#endregion
}

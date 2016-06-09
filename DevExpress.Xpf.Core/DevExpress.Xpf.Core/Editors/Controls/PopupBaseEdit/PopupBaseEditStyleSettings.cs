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

using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Themes;
namespace DevExpress.Xpf.Editors {
	public class PopupBaseEditStyleSettings : ButtonEditStyleSettings {
		protected internal virtual bool ShouldFocusPopup { get { return false; } }
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			PopupBaseEdit popupBaseEdit = editor as PopupBaseEdit;
			if (popupBaseEdit == null)
				return;
			popupBaseEdit.UpdatePopupElements();
		}
		public virtual ControlTemplate GetPopupTopAreaTemplate(PopupBaseEdit editor) {
			return null;
		}
		public virtual ControlTemplate GetPopupBottomAreaTemplate(PopupBaseEdit editor) {
			PopupBaseEditThemeKeyExtension key = new PopupBaseEditThemeKeyExtension() { ResourceKey = PopupBaseEditThemeKeys.PopupBottomAreaTemplate, ThemeName = ThemeHelper.GetEditorThemeName(editor) };
			return (ControlTemplate)ResourceHelper.FindResource(editor, key);
		}
		public virtual PopupFooterButtons GetPopupFooterButtons(PopupBaseEdit editor) {
			return PopupFooterButtons.None;
		}
		protected internal virtual bool GetShowSizeGrip(PopupBaseEdit editor) {
			return false;
		}
		public virtual bool ShouldCaptureMouseOnPopup { get { return true; } }
		public virtual bool StaysPopupOpen() {
			return false;
		}
		protected internal virtual PopupCloseMode GetClosePopupOnClickMode(PopupBaseEdit editor) {
			return editor.PopupSettings.GetClosePopupOnClickMode();
		}
	}
}

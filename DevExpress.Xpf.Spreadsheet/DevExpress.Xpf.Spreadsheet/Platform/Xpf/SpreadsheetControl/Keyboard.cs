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
using System.Windows.Input;
using DevExpress.Office.Internal;
using DevExpress.Xpf.Utils;
using PlatformIndepententKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatfromIndependentKeys = System.Windows.Forms.Keys;
namespace DevExpress.Xpf.Spreadsheet {
	public partial class SpreadsheetControl {
		protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if (e.Handled) return;
			if (e.Key == Key.Tab && !AcceptsTab)
				return;
			if (e.Key == Key.Return && !AcceptsReturn)
				return;
			if (e.Key == Key.Escape && !AcceptsEscape)
				return;
			PlatformIndepententKeyEventArgs args = FlowDirection == System.Windows.FlowDirection.LeftToRight ? e.ToPlatformIndependent() : ConvertToRightToLeft(e);
			if (InnerControl != null) {
				InnerControl.OnKeyDown(args);
				e.Handled = args.Handled;
			}
			if (args.KeyCode == PlatfromIndependentKeys.ProcessKey) {
				ProcessImeStart(args);
			}
		}
		private PlatformIndepententKeyEventArgs ConvertToRightToLeft(KeyEventArgs e) {
			if (e.Key == Key.Left) return new PlatformIndepententKeyEventArgs((PlatfromIndependentKeys)((int)PlatfromIndependentKeys.Right | GetModKeys()));
			if (e.Key == Key.Right) return new PlatformIndepententKeyEventArgs((PlatfromIndependentKeys)((int)PlatfromIndependentKeys.Left | GetModKeys()));
			return e.ToPlatformIndependent();
		}
		static int GetModKeys() {
			int res = 0;
			if (KeyboardHelper.IsShiftPressed) res |= (int)(PlatfromIndependentKeys.Shift);
			if (KeyboardHelper.IsControlPressed) res |= (int)(PlatfromIndependentKeys.Control);
			if (KeyboardHelper.IsAltPressed) res |= (int)(PlatfromIndependentKeys.Alt);
			return res;
		}
		void ProcessImeStart(PlatformIndepententKeyEventArgs e) {
		}
		protected override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e) {
			base.OnPreviewKeyUp(e);
			if (InnerControl != null)
				InnerControl.OnKeyUp(e.ToPlatformIndependent());
		}
		protected override void OnTextInput(TextCompositionEventArgs e) {
			base.OnTextInput(e);
			char[] text = e.Text.ToCharArray();
			if (InnerControl != null && text.Length > 0)
				InnerControl.OnKeyPress(new System.Windows.Forms.KeyPressEventArgs(text[0]));
		}
	}
}

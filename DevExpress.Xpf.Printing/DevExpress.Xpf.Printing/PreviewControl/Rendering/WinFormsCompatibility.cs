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
using System.Drawing;
using System.Linq;
using System.Text;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using Rectange = System.Drawing.Rectangle;
using System.Windows;
using System.Windows.Forms;
namespace DevExpress.Xpf.Printing.PreviewControl.Native {
	public static class WinFormsCompatibilityExtensions {
		public static MouseButtons ToWinFormsMouseButtons(this System.Windows.Input.MouseButton button) {
			MouseButtons buttons;
			switch(button) {
				case System.Windows.Input.MouseButton.Left:
					buttons = MouseButtons.Left;
					break;
				case System.Windows.Input.MouseButton.Middle:
					buttons = MouseButtons.Middle;
					break;
				case System.Windows.Input.MouseButton.Right:
					buttons = MouseButtons.Right;
					break;
				case System.Windows.Input.MouseButton.XButton1:
					buttons = MouseButtons.XButton1;
					break;
				case System.Windows.Input.MouseButton.XButton2:
					buttons = MouseButtons.XButton2;
					break;
				default:
					buttons = MouseButtons.None;
					break;
			}
			return buttons;
		}
		public static Keys ToWinFormsModifierKeys(this System.Windows.Input.ModifierKeys key) {
			switch(key) {
				case System.Windows.Input.ModifierKeys.Shift:
					return Keys.Shift;
				case System.Windows.Input.ModifierKeys.Control:
					return Keys.Control;
				case System.Windows.Input.ModifierKeys.Alt:
					return Keys.Alt;
				default:
					return Keys.None;
			}
		}
	}
}

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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.NoteHint;
namespace DevExpress.Description.Controls {
	public class GuideFormAlt : IGuideForm {
		#region IGuideForm Members
		void IGuideForm.Dispose() {
			formClosed = null;
			note.Hide();
		}
		bool IGuideForm.Visible {
			get { return note != null && note.IsVisible; }
		}
		NoteWindow note;
		void IGuideForm.Show(GuideControl owner, GuideControlDescription description) {
			note = new NoteWindow();
			var helper = new System.Windows.Interop.WindowInteropHelper(note);
			helper.Owner = owner.Window.Handle;
			note.IsVisibleChanged += (s, e) => {
				if(!note.IsVisible && formClosed != null) formClosed(this, EventArgs.Empty);
			};
			string text = "";
			if(!string.IsNullOrEmpty(description.Description)) {
				text = description.Description;
			}
			else {
				text = string.Format("<b>{0}</b><br>", description.GetTypeName()) + text;
			}
			note.ShowHtmlCloseButton = true;
			note.SetHtmlContent(text, description);
			var bounds = description.ScreenBounds;
			note.HintPosition = NoteHintPosition.Down;
			note.Show(new System.Windows.Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height));
		}
		event EventHandler formClosed;
		event EventHandler IGuideForm.FormClosed {
			add { formClosed += value; }
			remove { formClosed -= value; }
		}
		bool IGuideForm.IsHandle(IntPtr hwnd) {
			if(note == null || hwnd == IntPtr.Zero) return false;
			var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
			if(source == null) return false;
			Window window = (Window)source.RootVisual;
			if(window == note) return true;
			return false; 
		}
		#endregion
	}
}

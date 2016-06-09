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
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
namespace DevExpress.Utils {
	[Flags]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags")]
	public enum DragDropEffects {
		Scroll = -2147483648,
		All = -2147483645,
		None = 0,
		Copy = 1,
		Move = 2,
		Link = 4
	}
	public class DragEventArgs : EventArgs {
		public DragEventArgs(IDataObject data, int keyState, int x, int y, DragDropEffects allowedEffect, DragDropEffects effect) {
			AllowedEffect = allowedEffect;
			Data = data;
			Effect = effect;
			KeyState = keyState;
			X = x;
			Y = y;
		}
		public DragDropEffects AllowedEffect { get; private set; }
		public IDataObject Data { get; private set; }
		public DragDropEffects Effect { get; set; }
		public int KeyState { get; private set; }
		public int X { get; private set; }
		public int Y { get; private set; }
	}
	public class GiveFeedbackEventArgs : EventArgs {
		public GiveFeedbackEventArgs(DragDropEffects effect, bool useDefaultCursors) {
			Effect = effect;
			UseDefaultCursors = useDefaultCursors;
		}
		public DragDropEffects Effect { get; private set; }
		public bool UseDefaultCursors { get; set; }
	}
	public enum DragAction {
		Continue = 0,
		Drop = 1,
		Cancel = 2,
	}
	public class QueryContinueDragEventArgs : EventArgs {
		public QueryContinueDragEventArgs(int keyState, bool escapePressed, DragAction action) {
			Action = action;
			EscapePressed = escapePressed;
			KeyState = keyState;
		}
		public DragAction Action { get; set; }
		public bool EscapePressed { get; private set; }
		public int KeyState { get; private set; }
	}
	public class OfficeScroller : IOfficeScroller {
		protected virtual void OnVScroll(int delta) {
		}
		protected virtual bool AllowHScroll { get { return false; } }
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
#if WPF
		#region IOfficeScroller Members
		public void Start(System.Windows.Forms.Control control) {
		}
		public void Start(System.Windows.Forms.Control control, System.Drawing.Point screenPoint) {
		}
		#endregion
#else
		#region IOfficeScroller Members
		public void Start(Control control) {
		}
		public void Start(Control control, System.Drawing.Point screenPoint) {
		}
		#endregion
#endif
	}
	public class Timer : DispatcherTimer, IDisposable {
		public bool Enabled {
			get { return IsEnabled; }
			set {
				if (IsEnabled == value)
					return;
				if (value)
					Start();
				else
					Stop();
			}
		}
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
}

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
using System.Windows.Forms;
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public abstract class UIService : IUIService {
		int lockEventProcessing = 0;
		IUIView senderCore;
		public bool IsInEvent {
			get { return lockEventProcessing > 0; }
		}
		public IUIView Sender {
			get { return senderCore; }
			private set { senderCore = value; } 
		}
		public void BeginEvent(IUIView sender) {
			lockEventProcessing++;
			Sender = sender;
			BeginEventOverride(sender);
		}
		public void EndEvent() {
			EndEventOverride();
			Sender = null;
			lockEventProcessing--;
		}
		public bool ProcessMouse(IUIView view, MouseEventType eventType, MouseEventArgs ea) {
			if(IsInEvent || view == null) return false;
			bool result = false;
			BeginEvent(view);
			try {
				result = ProcessMouseOverride(view, eventType, CheckMouseArgs(ea));
			}
			finally { EndEvent(); }
			return result;
		}
		public bool ProcessFlickEvent(IUIView view, Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			if(IsInEvent || view == null) return false;
			bool result = false;
			BeginEvent(view);
			try {
				result = ProcessFlickOverride(view, point, args);
			}
			finally { EndEvent(); }
			return result;
		}
		public bool ProcessGesture(IUIView view, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			if(IsInEvent || view == null) return false;
			bool result = false;
			BeginEvent(view);
			try {
				result = ProcessGestureOverride(view, gid, args, parameters);
			}
			finally { EndEvent(); }
			return result;
		}
		public bool ProcessKey(IUIView view, KeyEventType eventype, Keys key) {
			if(IsInEvent || view == null) return false;
			bool result = false;
			BeginEvent(view);
			try {
				if(Array.IndexOf(GetKeys(), key) != -1) {
					result = ProcessKeyOverride(view, eventype, key);
				}
			}
			finally { EndEvent(); }
			return result;
		}
		public void Reset() { 
			ResetCore();
		}
		[System.Diagnostics.DebuggerStepThrough]
		static MouseEventArgs CheckMouseArgs(MouseEventArgs ea) {
			return ea ?? new MouseEventArgs(MouseButtons.None, 0, int.MinValue, int.MinValue, 0);
		}
		protected abstract void ResetCore();
		protected virtual void BeginEventOverride(IUIView sender) { }
		protected virtual void EndEventOverride() { }
		protected abstract bool ProcessKeyOverride(IUIView view, KeyEventType eventype, Keys key);
		protected abstract bool ProcessMouseOverride(IUIView view, MouseEventType eventType, MouseEventArgs ea);
		protected abstract bool ProcessFlickOverride(IUIView view, Point point, DevExpress.Utils.Gesture.FlickGestureArgs args);
		protected abstract bool ProcessGestureOverride(IUIView view, GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters);
		protected abstract Keys[] GetKeys();
		public static Point InvalidPoint = new Point(int.MinValue, int.MinValue);
	}
}

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

using DevExpress.Xpf.Layout.Core.Base;
using System;
namespace DevExpress.Xpf.Layout.Core {
	public abstract class UIService : BaseObject, IUIService {
		int lockEventProcessing = 0;
		public bool IsInEvent {
			get { return lockEventProcessing > 0; }
		}
		public IView Sender { get; private set; }
		public void BeginEvent(IView sender) {
			lockEventProcessing++;
			Sender = sender;
			BeginEventOverride(sender);
		}
		public void EndEvent() {
			EndEventOverride();
			Sender = null;
			lockEventProcessing--;
		}
		public bool ProcessMouse(IView view, Platform.MouseEventType eventType, Platform.MouseEventArgs ea) {
			if(IsDisposing || IsInEvent || view == null) return false;
			BeginEvent(view);
			bool result = ProcessMouseOverride(view, eventType, CheckMouseArgs(ea));
			EndEvent();
			return result;
		}
		public bool ProcessKey(IView view, Platform.KeyEventType eventype, System.Windows.Input.Key key) {
			if(IsDisposing || IsInEvent || view == null) return false;
			bool result = false;
			BeginEvent(view);
			if(Array.IndexOf(GetKeys(), key) != -1) {
				result = ProcessKeyOverride(view, eventype, key);
			}
			EndEvent();
			return result;
		}
		public static System.Windows.Point InvalidPoint = new System.Windows.Point(double.NaN, double.NaN);
		[System.Diagnostics.DebuggerStepThrough]
		static Platform.MouseEventArgs CheckMouseArgs(Platform.MouseEventArgs ea) {
			return ea ?? new Platform.MouseEventArgs(InvalidPoint, Platform.MouseButtons.None);
		}
		protected virtual bool ProcessKeyOverride(IView view, Platform.KeyEventType eventype, System.Windows.Input.Key key) { return false; }
		protected virtual bool ProcessMouseOverride(IView view, Platform.MouseEventType eventType, Platform.MouseEventArgs ea) { return false; }
		protected virtual void BeginEventOverride(IView sender) { }
		protected virtual void EndEventOverride() { }
		protected virtual System.Windows.Input.Key[] GetKeys() {
			return new System.Windows.Input.Key[] { };
		}
	}
}

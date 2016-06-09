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

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System;
using DevExpress.Utils;
#if SL
using Thumb = DevExpress.Xpf.Core.DXThumb;
#endif
namespace DevExpress.Xpf.Core {
	public interface IResizeHelperOwner {
		SizeHelperBase SizeHelper { get; }
		double ActualSize { get; set; }
		void ChangeSize(double delta);
		void OnDoubleClick();
		void SetIsResizing(bool isResizing);
	}
	public class ResizeHelper : IDisposable {
		IResizeHelperOwner owner;
		Thumb gripper;
		double startWidth;
		public ResizeHelper(IResizeHelperOwner owner) {
			this.owner = owner;
		}
		public bool IsResizing { get { return gripper != null ? gripper.IsDragging : false; } }
		protected IResizeHelperOwner Owner { get { return owner; } }
		protected double Width { get { return Owner.ActualSize; } set { Owner.ActualSize = value; } }
		protected Thumb Gripper { get { return gripper; } }
		public void Init(Thumb gripper) {
			UnhookEvents();
			this.gripper = gripper;
			HookupEvents();
		}
		protected virtual void UnhookEvents() {
			if(Gripper == null) return;
			Gripper.DragStarted -= new DragStartedEventHandler(OnDragStarted);
			Gripper.DragDelta -= new DragDeltaEventHandler(OnResize);
			Gripper.DragCompleted -= new DragCompletedEventHandler(OnDragCompleted);
			Gripper.MouseDoubleClick -= new MouseButtonEventHandler(OnDoubleClicked);
		}
		protected virtual void HookupEvents() {
			if(Gripper == null) return;
			Gripper.DragStarted += new DragStartedEventHandler(OnDragStarted);
			Gripper.DragDelta += new DragDeltaEventHandler(OnResize);
			Gripper.DragCompleted += new DragCompletedEventHandler(OnDragCompleted);
			Gripper.MouseDoubleClick += new MouseButtonEventHandler(OnDoubleClicked);
		}
		void OnDragStarted(object sender, DragStartedEventArgs e) {
			startWidth = Width;
			Owner.SetIsResizing(true);
			SetHandledTrue(e);
		}
		void OnDragCompleted(object sender, DragCompletedEventArgs e) {
			Owner.SetIsResizing(false);
			if(e.Canceled)
				Width = startWidth;
			SetHandledTrue(e);
		}
		void OnResize(object sender, DragDeltaEventArgs e) {
			Owner.ChangeSize(Owner.SizeHelper.GetDefinePoint(new Point(e.HorizontalChange, e.VerticalChange)));
			SetHandledTrue(e);
		}
		void SetHandledTrue(RoutedEventArgs e) {
#if !SILVERLIGHT
			e.Handled = true;
#endif
		}
		void OnDoubleClicked(object sender, MouseButtonEventArgs e) {
			Owner.OnDoubleClick();
			e.Handled = true;
		}
		#region IDisposable Members
		public void Dispose() {
			Init(null);
		}
		#endregion
	}
}

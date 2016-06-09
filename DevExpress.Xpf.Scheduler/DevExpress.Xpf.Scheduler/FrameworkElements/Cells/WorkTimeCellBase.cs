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
using System.Windows;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.Utils;
#if SL
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public abstract class VisualWorkTimeCellBase : VisualTimeCellBase {
		static VisualWorkTimeCellBase() {
		}
		protected VisualWorkTimeCellBase() {
			DefaultStyleKey = typeof(VisualWorkTimeCellBase);
		}
		protected override SchedulerHitTest HitTest { get { return SchedulerHitTest.Cell; } }
		protected bool IsWorkTime { get; set; }
		protected override void SubscribeContentEvents(object content) {
			base.SubscribeContentEvents(content);
			VisualWorkTimeCellBaseContent cellContent = content as VisualWorkTimeCellBaseContent;
			if (cellContent == null)
				return;
			if (IsWorkTime != cellContent.IsWorkTime) {
				RecalculateBrush(true);
			}
			cellContent.IsWorkTimeChanged += new EventHandler(OnIsWorkTimeChanged);
		}
		protected override void UnsubscribeContentEvents(object content) {
			base.UnsubscribeContentEvents(content);
			VisualWorkTimeCellBaseContent cellContent = content as VisualWorkTimeCellBaseContent;
			if (cellContent == null)
				return;
			cellContent.IsWorkTimeChanged -= new EventHandler(OnIsWorkTimeChanged);
		}
		void OnIsWorkTimeChanged(object sender, EventArgs e) {
			RecalculateBrush(true);
		}
		protected override void RecalculateBrush(bool visualViewInfoChanged) {
			base.RecalculateBrush(visualViewInfoChanged);
			VisualWorkTimeCellBaseContent cellContent = Content as VisualWorkTimeCellBaseContent;
			if (cellContent == null)
				return;
			IsWorkTime = cellContent.IsWorkTime;
		}		
	}
	public abstract class VisualWorkTimeCellBaseContent : VisualTimeCellBaseContent {
		#region IsWorkTime
		public static readonly DependencyProperty IsWorkTimeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualWorkTimeCellBaseContent, bool>("IsWorkTime", false, (d, e) => d.OnIsWorkTimeChanged(e.OldValue, e.NewValue));
		WeakEventHandler<EventArgs, EventHandler> onIsWorkTimeChanged;
		public event EventHandler IsWorkTimeChanged { add { onIsWorkTimeChanged += value; } remove { onIsWorkTimeChanged -= value; } }
		protected virtual void RaiseOnIsWorkTimeChanged(bool oldValue, bool newValue) {
			if (onIsWorkTimeChanged != null) {
				onIsWorkTimeChanged.Raise(this, EventArgs.Empty);
			}
			lastSettedIsWorkTime = newValue;
		}
		protected virtual void OnIsWorkTimeChanged(bool oldValue, bool newValue) {
			RaiseOnIsWorkTimeChanged(oldValue, newValue);
		}
		public bool IsWorkTime { get { return (bool)GetValue(IsWorkTimeProperty); } set { SetValue(IsWorkTimeProperty, value); } }
		#endregion
		bool lastSettedIsWorkTime;
		protected internal override bool CopyFromCore(ResourceCellBase source) {
			bool wasChanged = base.CopyFromCore(source);
			WorkTimeCellBase workTimeCellBaseSource = (WorkTimeCellBase)source;
			if (lastSettedIsWorkTime != workTimeCellBaseSource.IsWorkTime) {
				IsWorkTime = workTimeCellBaseSource.IsWorkTime;
				wasChanged = true;
			}
			return wasChanged;
		}
	}
}

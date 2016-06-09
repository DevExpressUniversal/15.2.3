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
using DevExpress.Utils;
namespace DevExpress.Xpf.Scheduler {
	#region DragDropMode
	public enum DragDropMode {
		Standard,
		Manual
	}
	#endregion
	#region MovementType
	public enum MovementType {
		Smooth,
		SnapToCells
	}
	#endregion
#if !SL
	public class DragDropOptions : Freezable {
#else
	public class DragDropOptions : DependencyObject {
#endif
		#region DragDropMode
		public static readonly DependencyProperty DragDropModeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DragDropOptions, DragDropMode>("DragDropMode", DragDropMode.Standard, (d,e) => d.OnDragDropModeChangedHandler(e.OldValue, e.NewValue));
		public DragDropMode DragDropMode {
			get { return (DragDropMode)GetValue(DragDropModeProperty); }
			set { SetValue(DragDropModeProperty, value); }
		}
		WeakEventHandler<EventArgs, EventHandler> onDragDropModeChanged;
		public event EventHandler DragDropModeChanged { add { onDragDropModeChanged += value; } remove { onDragDropModeChanged -= value; } }
		protected virtual void RaiseOnDragDropModeChanged() {
			if (onDragDropModeChanged != null)
				onDragDropModeChanged.Raise(this, EventArgs.Empty);
		}
		protected virtual void OnDragDropModeChangedHandler(DragDropMode oldMode, DragDropMode newMode) {
			RaiseOnDragDropModeChanged();
		}
		#endregion
		#region MovementType
		public static readonly DependencyProperty MovementTypeProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<DragDropOptions, MovementType>("MovementType", MovementType.Smooth, (d, e) => d.OnMovementTypeChangedHandler(e.OldValue, e.NewValue));
		public MovementType MovementType {
			get { return (MovementType)GetValue(MovementTypeProperty); }
			set { SetValue(MovementTypeProperty, value); }
		}
		WeakEventHandler<EventArgs, EventHandler> onMovementTypeChanged;
		public event EventHandler MovementTypeChanged {
			add { onMovementTypeChanged += value; }
			remove { onMovementTypeChanged -= value; }
		}
		protected virtual void RaiseOnMovementTypeChnaged() {
			if (onMovementTypeChanged != null)
				onMovementTypeChanged.Raise(this, EventArgs.Empty);
		}
		protected virtual void OnMovementTypeChangedHandler(MovementType oldMode, MovementType newMode) {
			RaiseOnMovementTypeChnaged();
		}
		#endregion
#if !SL
		protected override bool FreezeCore(bool isChecking) {
			if(isChecking)
				return false;
			return base.FreezeCore(isChecking);
		}
		protected override void CloneCore(Freezable sourceFreezable) {
			if(sourceFreezable.GetType() != this.GetType())
				return;
			base.CloneCore(sourceFreezable);
		}
		protected override Freezable CreateInstanceCore() {
			return new DragDropOptions();
		}
#endif
	}
}

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
using DevExpress.XtraScheduler;
using System.Collections.Generic;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Scheduler.Internal {
	public static class VisualStateManagerHelper {
		public static void GoToState(FrameworkElement element, string stateName, bool useTransitions) {
			VisualStateManager.GoToState(element, stateName, useTransitions);
		}
	}
	public static partial class XpfMouseUtils {
		public static System.Windows.Forms.DragEventArgs ConverToNativeDragEventArgs(System.Windows.DragEventArgs e, IInputElement control) {
			System.Windows.Forms.DragDropEffects formsDragDropEffects = (System.Windows.Forms.DragDropEffects)e.Effects;
			System.Windows.Forms.DragDropEffects formsAllowedEffects = (System.Windows.Forms.DragDropEffects)e.AllowedEffects;
			Point pt = e.GetPosition(control);
			System.Windows.Forms.DataObject wrapper = new System.Windows.Forms.DataObject();
			wrapper.SetData(typeof(SchedulerDragData), e.Data.GetData(typeof(SchedulerDragData)));
			return new System.Windows.Forms.DragEventArgs(wrapper, (int)e.KeyStates, (int)pt.X, (int)pt.Y, formsAllowedEffects, formsDragDropEffects);
		}
	}
	public abstract class SchedulerSealableObject : Freezable {
		protected override bool FreezeCore(bool isChecking) {
			if (isChecking)
				return true;
			return base.FreezeCore(isChecking);
		}
	}
}

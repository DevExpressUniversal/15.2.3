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
using DevExpress.Utils;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Threading;
namespace DevExpress.Xpf.Grid.Native {
	public static class DragDropScroller {
		public static bool IsDragging(DependencyObject sender) {
#if !SL
			FrameworkElement topLevel = LayoutHelper.GetTopLevelVisual(sender) as FrameworkElement;
#else
			DependencyObject topLevel = Application.Current.RootVisual;
#endif
			if(topLevel == null)
				return false;
			return DragManager.GetIsDragging(topLevel);
		}
		public static void StartScrolling(DataViewBase view) {
			if(view is ITableView && ((ITableView)view).AllowScrollHeaders)
				view.ScrollTimer.Start();
		}
		public static void StopScrolling(DataViewBase view) {
			if(view is ITableView)
				view.ScrollTimer.Stop();
		}
	}
	public class GridDragDropElementHelper : DragDropElementHelper {
		public GridDragDropElementHelper(ISupportDragDropColumnHeader supportDragDrop, bool isRelativeMode = true) : base(supportDragDrop, isRelativeMode) { }
		protected override BaseDragDropStrategy CreateDragDropStrategy() {
			return new GridDragDropStrategy((ISupportDragDropColumnHeader)SupportDragDrop, this);
		}
	}
	public class GridDragDropStrategy : BaseDragDropStrategy {
		ISupportDragDropColumnHeader SupportDragDropColumnHeader { get { return (ISupportDragDropColumnHeader)SupportDragDrop; } }
		public override FrameworkElement SubscribedElement { get { return SupportDragDrop.SourceElement; } }
		public GridDragDropStrategy(ISupportDragDropColumnHeader supportDragDrop, DragDropElementHelper helper) : base(supportDragDrop, helper) { }
		public override FrameworkElement GetSourceElement() {
			return (FrameworkElement)Helper.GetDropTargetByHitElement(SupportDragDropColumnHeader.SourceElement);
		}
		public override FrameworkElement GetDragElement() {
			return SupportDragDropColumnHeader.RelativeDragElement;
		}
		public override FrameworkElement GetTopVisual(FrameworkElement node) {
#if !SL
			return SupportDragDropColumnHeader.TopVisual;
#else
			return (FrameworkElement)Application.Current.RootVisual;
#endif
		}
		public override void UpdateLocation(IndependentMouseEventArgs e) {
			SupportDragDropColumnHeader.UpdateLocation(e);
		}
	}
}

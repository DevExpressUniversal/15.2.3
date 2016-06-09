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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid.Native {
	public class RemoveBandDropTarget : RemoveColumnDropTarget {
		protected override UIElement GetTargetElement(DataViewBase view) {
			return view.DataControl.BandsLayoutCore.GetBandsContainerControl();
		}
	}
	public class RemoveColumnDropTarget : IDropTarget {
		public ColumnHeaderDragElement dragElement { get; protected set; }
		DataViewBase dataView;
		public DataViewBase GetDataView(UIElement source) {
			return dataView ?? (dataView = DataControlBase.FindCurrentView(source));
		}
		public virtual void Drop(UIElement source, Point pt) {
			if(IsPositionInDropZone(source, pt))
				ColumnChooserDropTarget.DropColumnCore(source);
			DragDropScroller.StopScrolling(dataView);
		}
		public virtual void OnDragLeave() {
			if(dragElement != null) {
				BaseGridColumnHeader.SetIsInDropArea(dragElement.DragPreviewElement, false);
				DragDropScroller.StopScrolling(dataView);
			}
		}
		public virtual void OnDragOver(UIElement source, Point pt) {
			DragDropElementHelper helper = ((BaseGridHeader)source).DragDropHelper;
			dragElement = (ColumnHeaderDragElement)helper.DragElement;
			BaseGridColumnHeader.SetIsInDropArea(dragElement.DragPreviewElement, CanDrop(source, pt));
			DragDropScroller.StartScrolling(dataView);
		}
		bool CanDrop(UIElement source, Point pt) {
			return !ColumnChooserDropTarget.IsForbiddenUngroupGesture(source) && IsPositionInDropZone(source, pt);
		}
		protected virtual bool IsPositionInDropZone(UIElement source, Point pt) {
			DataViewBase view = GetDataView(source);
			if(!DesignerHelper.GetValue(view, view.AllowMoveColumnToDropArea, false))
				return false;
			if(!IsPositionInsideElement(view.RootView.DataControl, pt, new Thickness(0)))
				return true;
			if(GetTargetElement(view) == null)
				return true;
			return !IsPositionInsideElement(GetTargetElement(view), pt, new Thickness(50));
		}
		protected virtual UIElement GetTargetElement(DataViewBase view) { return view.HeadersPanel; }
		protected virtual bool IsPositionInsideElement(UIElement source, Point location, Thickness margin) {
#if !SL
			UIElement parent = LayoutHelper.GetTopLevelVisual(source) as UIElement;
#else
			UIElement parent = Application.Current.RootVisual;
#endif
			Rect rect = LayoutHelper.GetRelativeElementRect(source, parent);
			Point newLocation = new Point(location.X - rect.X, location.Y - rect.Y);
			return LayoutHelper.IsPointInsideElementBounds(newLocation, (FrameworkElement)source, margin);
		}
	}
}

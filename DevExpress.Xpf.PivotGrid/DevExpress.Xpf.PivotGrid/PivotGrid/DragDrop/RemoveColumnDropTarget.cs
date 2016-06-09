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
using DevExpress.XtraPivotGrid.Data;
using System.Windows.Controls;
using DevExpress.XtraPivotGrid;
using System.Collections.Generic;
using System.Windows.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows.Markup;
using System.Collections;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Windows.Media;
using DevExpress.Xpf.Utils;
using System.Windows.Controls.Primitives;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class RemoveColumnDropTarget : IDropTarget {
		ContentPresenter dragPreviewElement;
		#region IDropTarget Members
		void IDropTarget.Drop(UIElement source, Point pt) {
			if(source as FieldHeader == null)
				return;
			PivotGridField field = ((FieldHeaderBase)source).Field;
			if(field == null || field.PivotGrid == null)
				return;
			if(dragPreviewElement != null && GetCanHideCore(ref pt, field)) {
				PivotFieldItem item = field.FieldItem;
				if(item == null || !item.Visible && !field.PivotGrid.DeferredUpdates)
					return;
				field.PivotGrid.Data.FieldListFields.HideField(item);
			}
		}
		void IDropTarget.OnDragLeave() {
			SetCanHide(false);
		}
		void IDropTarget.OnDragOver(UIElement source, Point pt) {
			if(source as FieldHeader == null)
				return;
			FieldHeaderDragElement elem = ((FieldHeaderBase)source).DragDropHelper.DragElement as FieldHeaderDragElement;
			if(elem == null)
				return;
			dragPreviewElement = elem.DragPreviewElement;
			if(dragPreviewElement == null)
				return;
			PivotGridField field = ((FieldHeaderBase)source).Field;
			SetCanHide(GetCanHideCore(ref pt, field));
		}
		private bool GetCanHideCore(ref Point pt, PivotGridField field) {
			bool canHide = field != null && field.CanHide && field.PivotGrid != null && !field.PivotGrid.Data.FieldListFields.HiddenFields.Contains(field);
			canHide = canHide && !IsInsidePivotGrid(pt, field);
			return canHide;
		}
		bool IsInsidePivotGrid(Point pt, PivotGridField field) {
			if(field == null || field.PivotGrid == null)
				return false;
			if(IsPositionInsideElement(field.PivotGrid, field.PivotGrid, pt, new Thickness()))
				return true;
			if(field.PivotGrid.IsFieldListVisible && field.PivotGrid.ActualFieldList != null && field.PivotGrid.ActualFieldList.TopContainer != null && IsPositionInsideElement(field.PivotGrid, field.PivotGrid.ActualFieldList.TopContainer, pt, new Thickness()))
				return true;
			return false;
		}
		private void SetCanHide(bool value) {
			if(dragPreviewElement != null)
				DragFieldHeader.SetCanHide(dragPreviewElement, value);
		}
		#endregion
		protected virtual bool IsPositionInsideElement(PivotGridControl owner, UIElement source, Point location, Thickness margin) {
#if !SL
			UIElement parent = LayoutHelper.GetTopLevelVisual(owner) as UIElement;
			UIElement sourceParent = LayoutHelper.GetTopLevelVisual(source) as UIElement;
			if(parent != sourceParent)
				return false;
#else
			UIElement parent = Application.Current.RootVisual;
#endif
			Rect rect = LayoutHelper.GetRelativeElementRect(source, parent);
			Point newLocation = new Point(location.X - rect.X, location.Y - rect.Y);
			return LayoutHelper.IsPointInsideElementBounds(newLocation, (FrameworkElement)source, margin);
		}
	}
}

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
	public class VerticalCellsDropTarget : CellsDropTarget {
		public VerticalCellsDropTarget(CellsAreaPresenter presenter) : base(presenter) {
		}
		protected override Orientation GetDropPlaceOrientation(DependencyObject element) {
			return Orientation.Vertical;
		}
		protected override Point CorrectDragIndicatorLocation(Point point) {
			point.X = 0;
			if(Cells.Items.Count > 0 && (((IScrollInfo)Cells).ViewportHeight >= ((IScrollInfo)Cells).ExtentHeight || Cells.GetViewPortHeight(Cells.RenderSize.Height, false) + Cells.Top == Cells.MaxLevel + 1))
				point.Y = Cells.Data.VisualItems.GetHeightDifference(Cells.Top, Cells.MaxLevel + 1, false) - Cells.TopOffset;
			else
				point.Y = 0;
			return point;
		}
		protected override void SetColumnHeaderDragIndicatorSize(DependencyObject element, double value) {
			PivotGridControl.SetFieldHeaderDragIndicatorSize(element, GetDragIndicatorHeight());
		}
		double GetDragIndicatorHeight() {
			return Math.Min(Convert.ToDouble(Data.VisualItems.GetWidthDifference(Cells.Left, Cells.Left + Cells.GetViewPortWidth(Cells.RenderSize.Width, true), false)), Cells.RenderSize.Width);
		}
		protected override void SetDropPlaceOrientation(DependencyObject element, Orientation value) {
			  element.SetValue(UIElement.RenderTransformProperty, new RotateTransform() { Angle = -90 });
		}
	}
	public class CellsDropTarget : HeaderDropTargetBase {
		public CellsDropTarget(CellsAreaPresenter presenter)
			: base(presenter) {
			this.Cells = presenter;
		}
		public CellsAreaPresenter Cells { get; protected internal set; }
		protected PivotGridWpfData Data { get { return Cells.Data; } }
		protected PivotGridControl PivotGrid { get { return Data.PivotGrid; } }
		protected override string DragIndicatorTemplatePropertyName { get { return PivotGridControl.FieldHeaderDragIndicatorTemplatePropertyName; } }
		protected override FrameworkElement DragIndicatorTemplateSource { get { return PivotGrid; } }
		protected override int DropIndexCorrection { get { return 0; } }
		protected override FrameworkElement GridElement { get { return PivotGrid; } }
		protected override string HeaderButtonTemplateName { get { return PivotGridScroller.TemplatePartCells; } }
		protected override bool CanDrop(UIElement source, int dropIndex) {
			FieldHeaderBase header = source as FieldHeaderBase;
			if(header == null || header.Field == null || dropIndex < 0)
				return false;
			if(header.Field.PivotGrid != PivotGrid)
				return false;
			dropIndex = GetCorrectedDropIndex(header.Field);
			if(dropIndex < 0 || dropIndex == GetFieldIndex(header.Field))
				return false;
			return Data.OnFieldAreaChanging(header.Field.InternalField, PivotArea.DataArea, dropIndex);
		}
		int GetFieldIndex(PivotGridField field) {
			int curIndex;
			if(field.Area != FieldArea.DataArea || !field.Visible)
				return -1;
			if(field.Group == null)
				curIndex = field.AreaIndex;
			else
				curIndex = field.Group[field.Group.VisibleCount - 1].AreaIndex;
			return curIndex;
		}
		int GetCorrectedDropIndex(PivotGridField pivotGridField) {
			int correction = pivotGridField.Area == FieldArea.DataArea && pivotGridField.Visible ? 0 : 1;
			return Math.Max(0, Data.GetFieldCountByArea(FieldArea.DataArea) + correction - 1);
		}
		protected override HeaderDropTargetBase.HeaderHitTestResult CreateHitTestResult() {
			return new PivotHeaderHitTestResult();
		}
		protected override int GetDragIndex(int dropIndex) {
			return 0;
		}
		protected override Orientation GetDropPlaceOrientation(DependencyObject element) {
			return Orientation.Horizontal;
		}
		protected override int GetRelativeVisibleIndex(UIElement element) {
			FieldHeaderBase header = element as FieldHeaderBase;
			if(header == null || header.Field == null)
				return 0;
			return GetFieldIndex(header.Field);
		}
		protected override int GetVisibleIndex(DependencyObject obj) {
			return Math.Max(0, Data.GetFieldCountByArea(FieldArea.DataArea) - 1);
		}
		protected override bool IsSameSource(UIElement element) {
			FieldHeaderBase header = element as FieldHeaderBase;
			if(header == null || header.Field == null)
				return false;
			return header.Field.Area == FieldArea.DataArea && header.Field.Visible;
		}
		protected override void MoveColumnTo(UIElement source, int dropIndex) {
			FieldHeaderBase header = source as FieldHeaderBase;
			if(header == null || header.Field == null || dropIndex < 0)
				return;
			dropIndex = GetCorrectedDropIndex(header.Field);
			if(dropIndex < 0)
				return;
			Data.SetFieldAreaPositionAsync(header.Field.InternalField, PivotArea.DataArea, dropIndex, false);
		}
		protected override Point CorrectDragIndicatorLocation(Point point) {
			point.Y = 0;
			if(Cells.Items.Count > 0 && (((IScrollInfo)Cells).ViewportWidth >= ((IScrollInfo)Cells).ExtentWidth || Cells.GetViewPortWidthCore(Cells.RenderSize.Width, false) + Cells.Left == Cells.MaxIndex + 1))
				point.X = Cells.Data.VisualItems.GetWidthDifference(Cells.Left, Cells.MaxIndex + 1, true) - Cells.LeftOffset;
			else
				point.X = 0;
			return point;
		}
		protected override void SetColumnHeaderDragIndicatorSize(DependencyObject element, double value) {
			PivotGridControl.SetFieldHeaderDragIndicatorSize(element, GetDragIndicatorHeight());
		}
		double GetDragIndicatorHeight() {
			return Math.Min(Convert.ToDouble(Data.VisualItems.GetHeightDifference(Cells.Top, Cells.Top + Cells.GetViewPortHeight(Cells.RenderSize.Height, true), false)), Cells.RenderSize.Height);
		}
		protected override void SetDropPlaceOrientation(DependencyObject element, Orientation value) {
			if(value != Orientation.Horizontal)
				throw new ArgumentException("Only vertical orientation is supported");
		}
		protected class PivotHeaderHitTestResult : HeaderHitTestResult {
			public PivotHeaderHitTestResult() {
			}
			protected override DropPlace GetDropPlace(DependencyObject visualHit) {
				return DropPlace.Next;
			}
			protected override DependencyObject GetGridColumn(DependencyObject visualHit) {
				return visualHit;
			}
		}
	}
}

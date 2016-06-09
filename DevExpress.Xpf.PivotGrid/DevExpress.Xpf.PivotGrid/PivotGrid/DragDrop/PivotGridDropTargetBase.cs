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
	public interface IDragOverHandler {
		void DragOver(bool over);
	}
	public abstract class PivotGridDropTargetBase : HeaderDropTargetBase {
		PivotDropTargetMouseOverEventHandler mouseOver;
		bool isMouseOver;
		FieldListArea fieldListArea;
		PivotGridWpfData data;
		IList children;
		ContentPresenter dragPreviewElement;
		protected PivotGridDropTargetBase(Panel panel)
			: base(panel) {
				this.fieldListArea = GetFieldListArea(panel);
				this.children = GetChildren(panel);
		}
		protected PivotGridWpfData Data {
			get {
				if(data == null)
					data = PivotGridControl.GetDataFromAttachedPivot(PivotChild);
				return data;
			}
		}
		protected abstract DependencyObject PivotChild { get; }
		protected PivotGridControl PivotGrid { get { return Data != null ? Data.PivotGrid : null; } }
		protected override FrameworkElement GridElement { get { return PivotGrid; } }
		protected override FrameworkElement DragIndicatorTemplateSource { get { return PivotGrid; } }
		protected override int DropIndexCorrection { get { return 0; } }
		protected override string DragIndicatorTemplatePropertyName {
			get { return PivotGridControl.FieldHeaderDragIndicatorTemplatePropertyName; }
		}
		protected override int ChildrenCount { get { return children.Count; } }
		protected override System.Collections.IList Children { get { return children; } }
		protected void SetChildren(System.Collections.IList children) {
			this.children = children;
		}
		protected bool IsRealSource {
			get {
				FieldHeadersBase headers = PivotChild as FieldHeadersBase;
				return headers == null || !headers.ShowListSource;
			}
		}
		public event PivotDropTargetMouseOverEventHandler MouseOver {
			add { mouseOver += value; }
			remove { mouseOver -= value; }
		}
		public bool IsMouseOver {
			get { return isMouseOver; }
			protected set {
				if(isMouseOver == value)
					return;
				isMouseOver = value;
				RaiseMouseOver(isMouseOver);
			}
		}
		protected virtual void RaiseMouseOver(bool isMouseOver) { 
			if(mouseOver != null)
				mouseOver(this, new PivotDropTargetMouseOverEventArgs(isMouseOver));
			if(PivotChild as IDragOverHandler != null)
				((IDragOverHandler)PivotChild).DragOver(isMouseOver);
		}
		protected override void UpdateDragAdornerLocation(int headerIndex) {
			base.UpdateDragAdornerLocation(headerIndex);
			if(PivotGrid != null)
				PivotGrid.IsDragging = true;
			IsMouseOver = true;
		}
		protected override void DestroyDragAdorner() {
#if !SL
			if(((FrameworkElement)AdornableElement).IsLoaded)
#endif
			base.DestroyDragAdorner();
			IsMouseOver = false;
			if(dragPreviewElement != null)
				DragFieldHeader.SetCanHide(dragPreviewElement, false);
			if(PivotGrid != null)
				PivotGrid.IsDragging = false;
		}
		protected override bool CanDrop(UIElement source, int dropIndex) {
			FieldHeaderBase header = source as FieldHeaderBase;
			if(header != null && header.DragDropHelper != null) {
				FieldHeaderDragElement del = header.DragDropHelper.DragElement as FieldHeaderDragElement;
				if(del != null)
					dragPreviewElement = del.DragPreviewElement;
			}
			if(dropIndex >= 0 && !PivotGrid.IsAsyncInProgress) {
				if(header != null) {
					if(header.Field != null && header.Field.PivotGrid != PivotGrid)
						return false;
					return FieldAreaChanging(header.Field, dropIndex, source);
				}
				return true;
			} else
				return false;
		}
		protected bool FieldAreaChanging(PivotGridField field, int dropIndex, UIElement source) {
			if(field == null)
				return false;
			int areaIndex;
			PivotArea area;
			if(FieldListArea == FieldListArea.All) {
				areaIndex = -1;
				area = field.Area.ToPivotArea();
				return field.CanHide && (IsRealSource ? field.Visible : !Data.FieldListFields.HiddenFields.Contains(field));
			} else {
				areaIndex = dropIndex;
				area = FieldListArea.ToPivotArea();
			}
			return Data.OnFieldAreaChanging(field.InternalField, area, GetCorrectedDropIndex(areaIndex, source));
		}
		protected int GetCorrectedDropIndex(int dropIndex, UIElement source) {
			int newDropIndex = IsRealSource ? AddGroupCount(dropIndex) : dropIndex;
			int sourceIndex = GetVisibleIndex(source);
			if(IsSameSource(source) && sourceIndex < dropIndex && sourceIndex != -1)
				newDropIndex--;
			return newDropIndex;
		}
		protected virtual int AddGroupCount(int dropIndex) {
			int newDropIndex = dropIndex;
			List<PivotGridField> fields = new List<PivotGridField>();
			foreach(PivotGridField areaField in Data.GetFieldsByArea(FieldListArea.ToFieldArea(), true))
				if(areaField.Group == null || areaField.Group.IndexOf(areaField) < 1)
					fields.Add(areaField);
			fields.Sort(PivotGridFieldComparisonByAreaIndex);
			for(int i = 0; i < Math.Min(dropIndex, fields.Count); i++) {
				PivotGridGroup group = fields[i].Group;
				if(group != null && group.VisibleCount > 1)
					newDropIndex += group.VisibleCount - 1;
			}
			return newDropIndex;
		}
		protected override HeaderDropTargetBase.HeaderHitTestResult CreateHitTestResult() {
			return new FieldHeaderHitTestResult();
		}
		protected override string HeaderButtonTemplateName {
			get { return FieldHeader.TemplatePartHeaderButtonName; }
		}
		protected override int GetRelativeVisibleIndex(UIElement element) {
			return GetVisibleIndex(element);	
		}
		protected override bool IsSameSource(UIElement element) {
			FieldListArea pres = FieldHeadersBase.GetFieldListArea(element);
			if(pres == FieldListArea.All)
				pres = InnerFieldListDropTarget.GetFieldListElementArea(element);
			return FieldListArea == pres;
		}
		protected override void MoveColumnTo(UIElement source, int dropIndex) {
			PivotGridField field = FieldHeader.GetField(source);
			if(IsRealSource) {
				if(FieldListArea.IsPivotArea())
					SetFieldAreaPositionAsync(field, GetCorrectedDropIndex(dropIndex, source));
				else
					field.Hide();
			} else {
				if(FieldListArea.IsPivotArea())
					PivotGrid.Data.FieldListFields.MoveField(field.FieldItem, FieldListArea.ToPivotArea(), GetCorrectedDropIndex(dropIndex, source));
				else
					PivotGrid.Data.FieldListFields.HideField(field.FieldItem);
			}
		}
		void SetFieldAreaPositionAsync(PivotGridField field, int correctedIndex) {
			Data.SetFieldAreaPositionAsync(field.InternalField, FieldListArea.ToPivotArea(), correctedIndex, false);
		}
		protected override void SetColumnHeaderDragIndicatorSize(DependencyObject element, double value) {
			PivotGridControl.SetFieldHeaderDragIndicatorSize(element, value);
		}
		protected override void SetDropPlaceOrientation(DependencyObject element, Orientation value) {
			element.SetValue(UIElement.VisibilityProperty, (FieldListArea != FieldListArea.All) ? Visibility.Visible : Visibility.Collapsed);
			if(FieldListArea != FieldListArea.All)
				element.SetValue(UIElement.RenderTransformProperty, new RotateTransform() { Angle = value == Orientation.Horizontal ? 0 : -90 });
			else
				if(dragPreviewElement != null)
					DragFieldHeader.SetCanHide(dragPreviewElement, true);
		}
		public virtual FieldListArea FieldListArea { get { return fieldListArea; } }
		protected abstract FieldListArea GetFieldListArea(Panel panel);
		protected abstract IList GetChildren(Panel panel);
		public static int PivotGridFieldComparisonByAreaIndex(PivotGridField field1, PivotGridField field2) {
			if(field1.AreaIndex == field2.AreaIndex && field1 != field2)
				if(field1.InternalField.IsDataField || !field2.InternalField.IsDataField)
					return Comparer.Default.Compare(field1.AreaIndex, field2.AreaIndex + 1);
			return Comparer.Default.Compare(field1.AreaIndex, field2.AreaIndex);
		}
		protected class FieldHeaderHitTestResult : HeaderDropTargetBase.HeaderHitTestResult {
			protected override DropPlace GetDropPlace(DependencyObject visualHit) {
				return FieldHeadersBase.GetDropPlace(visualHit);
			}
			protected override DependencyObject GetGridColumn(DependencyObject visualHit) {
				FieldHeaderBase header = LayoutHelper.FindParentObject<FieldHeaderBase>(visualHit);
				return header != null ? FieldHeader.GetField(header) : null;
			}
		}
		protected static Panel GetAnyPanel(FrameworkElement owner) {
			Panel panel = (Panel)LayoutHelper.FindElement(owner, (d) => d is VirtualizingStackPanel);
			if(panel == null)
				panel = (Panel)LayoutHelper.FindElement(owner, (d) => d is Grid);
			return panel;
		}
	}
}

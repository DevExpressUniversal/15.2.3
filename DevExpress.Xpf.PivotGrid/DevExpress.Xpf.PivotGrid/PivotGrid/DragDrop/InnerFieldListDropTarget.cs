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
	public class InnerFieldListDropTarget : PivotGridDropTargetBase {
		InnerFieldListControl innerFieldList;
		public InnerFieldListDropTarget(InnerFieldListControl innerFieldList)
			: base(GetAnyPanel(innerFieldList)) {
			this.innerFieldList = innerFieldList;
		}
		protected override FieldListArea GetFieldListArea(Panel panel) {
			return GetFieldListElementArea(panel);
		}
		internal static FieldListArea GetFieldListElementArea(UIElement panel) {
			FieldHeadersBase ctrl = LayoutHelper.FindParentObject<FieldHeadersBase>(panel);
			return ctrl != null ? ctrl.Area : FieldListArea.All;
		}
		protected override IList GetChildren(Panel panel) {
			List<FieldHeaderBase> headers = new List<FieldHeaderBase>();
			if(FieldListArea == Xpf.PivotGrid.FieldListArea.All)
				return headers;
			LayoutHelper.ForEachElement(panel, delegate(FrameworkElement e) {
				FieldHeaderBase header = e as FieldHeaderBase;
				if(header != null)
					headers.Add(header);
			});
			return headers;
		}
		protected override DependencyObject PivotChild { get { return innerFieldList; } }
		protected override Orientation GetDropPlaceOrientation(DependencyObject element) {
			return Orientation.Vertical;
		}
		protected override int GetVisibleIndex(DependencyObject obj) {
			if(obj as DependencyObject == null)
				return -1;
			FieldHeaderBase header = LayoutHelper.FindParentObject<FieldHeaderBase>((DependencyObject)obj);
			return header != null && innerFieldList.ItemsSource != null ? innerFieldList.ItemsSource.IndexOf(header.Field) : -1;
		}
		protected override Point CorrectDragIndicatorLocation(Point point) {
			if(ChildrenCount == 0)
				return new Point(point.X, 2);
			return base.CorrectDragIndicatorLocation(point);
		}
		protected override int AddGroupCount(int dropIndex) {
			return dropIndex;
		}
		protected override void SetColumnHeaderDragIndicatorSize(DependencyObject element, double value) {
			if(ChildrenCount != 0)
				base.SetColumnHeaderDragIndicatorSize(element, value);
			else
				base.SetColumnHeaderDragIndicatorSize(element, innerFieldList.RenderSize.Width - 7);
		}
		protected override object GetHeaderButtonOwner(int index) {
			object button = GetHeaderButtonByIndex(index);
			return button ?? base.GetHeaderButtonOwner(index);
		}
		object GetHeaderButtonByIndex(int index) {
			index = Math.Min(innerFieldList.ItemsSource.Count - 1, index);
			for(int i = 0; i < Children.Count; i++)
				if(((FieldHeaderBase)Children[i]).Field == innerFieldList.ItemsSource[index])
					return Children[i];
			SetChildren(GetChildren(Panel));
			for(int i = 0; i < Children.Count; i++)
				if(((FieldHeaderBase)Children[i]).Field == innerFieldList.ItemsSource[index])
					return Children[i];
			return null;
		}
		protected override int ChildrenCount {
			get {
				return Children.Count > 0 ? innerFieldList.ItemsSource.Count : 0;
			}
		}
		protected override void UpdateDragAdornerLocation(int headerIndex) {
			FieldHeaderBase header = ((FieldHeaderBase)GetHeaderButtonByIndex(headerIndex));
			if(ChildrenCount > 0 && (header == null || !header.IsLoaded)) 
				return;
			base.UpdateDragAdornerLocation(headerIndex);
		}
	}
}

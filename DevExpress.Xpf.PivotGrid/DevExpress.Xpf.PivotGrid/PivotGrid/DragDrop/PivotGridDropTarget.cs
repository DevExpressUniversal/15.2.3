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
	public class DataAreaDropTarget : PivotGridDropTarget {
		DataAreaDropTarget(Panel panel) : base(panel) { }
		internal static PivotGridDropTargetBase Create(DataAreaPopupEdit edit) {
			FieldHeadersPanel panel = edit.GetHeadersPanel();
			if(!edit.IsPopupOpen && edit.NeedShowPopup(panel)) {
				if(edit.PivotGrid != null)
					edit.PivotGrid.IsDragging = true;
				edit.IsSelfMouseOver = true;
				return null;
			} else {
				return new PivotHeadersDropTarget(edit.GetHeaders());
			}
		}
	}
	public class PivotHeadersDropTarget : PivotGridDropTarget {
		FieldHeaders headers;
		public PivotHeadersDropTarget(FieldHeaders headers)
			: base(GetHeadersPanel(headers)) {
				this.headers = headers;
		}
		static System.Windows.Controls.Panel GetHeadersPanel(FieldHeaders headers) {
			return LayoutHelper.FindElement(headers, (d) => d is FieldHeadersPanel) as FieldHeadersPanel;
		}
		protected override DependencyObject PivotChild { get { return headers; } }
	}
	public class PivotGridDropTarget : PivotGridDropTargetBase {
		public PivotGridDropTarget(Panel panel)
			: base(panel) {
		}
		protected override FieldListArea GetFieldListArea(Panel panel) {
			return FieldHeadersBase.GetFieldListArea(panel);
		}
		protected override DependencyObject PivotChild { get { return LayoutHelper.FindParentObject<FieldHeaders>(Panel) ?? (DependencyObject)Panel; } }
		protected override IList GetChildren(Panel panel) {
			FieldHeadersPanel resizingPanel = panel as FieldHeadersPanel;
			if(resizingPanel != null)
				return resizingPanel.SortedChildren;
			return GetVisibleChildren(panel);
		}
		IList GetVisibleChildren(Panel panel) {
			UIElementCollection children = panel.Children;
			List<UIElement> res = new List<UIElement>(children.Count);
			for(int i = 0; i < children.Count; i++) {
				if(children[i].Visibility == Visibility.Visible)
					res.Add(children[i]);
			}
			return res;
		}
		protected override Orientation GetDropPlaceOrientation(DependencyObject element) {
			return Orientation.Horizontal;
		}
		protected override int GetVisibleIndex(DependencyObject obj) {
			if(obj as DependencyObject == null)
				return -1;
			FieldHeaderBase header = LayoutHelper.FindParentObject<FieldHeaderBase>((DependencyObject)obj);
			return header != null ? FieldHeader.GetActualAreaIndex(header) : -1;
		}
	}
}

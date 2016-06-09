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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class SelectionControl : Control {
		public static readonly DependencyProperty IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(SelectionControl), new PropertyMetadata(false, OnUpdateState));
		public static readonly DependencyProperty IsHighlightedProperty = DependencyPropertyManager.Register("IsHighlighted", typeof(bool), typeof(SelectionControl), new PropertyMetadata(false, OnUpdateState));
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public bool IsHighlighted {
			get { return (bool)GetValue(IsHighlightedProperty); }
			set { SetValue(IsHighlightedProperty, value); }
		}
		static void OnUpdateState(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SelectionControl selectionControl = d as SelectionControl;
			if (selectionControl != null)
				selectionControl.UpdateState();
		}
		void UpdateState() {
			if (IsHighlighted)
				VisualStateManager.GoToState(this, "Highlighted", false);
			else if (IsSelected)
				VisualStateManager.GoToState(this, "Selected", false);
			else
				VisualStateManager.GoToState(this, "Normal", false);
		}
		public SelectionControl() {
			DefaultStyleKey = typeof(SelectionControl);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateState();
		}
	}
	[NonCategorized]
	public class SelectionPresentation : SelectionControl, ILayoutElement {
		SelectionGeometryItem selectionGeometryItem;
		public SelectionGeometryItem SelectionGeometryItem { get { return selectionGeometryItem; } }
		public ILayout Layout { get { return null; } }
		public SelectionPresentation(SelectionGeometryItem selectionGeometryItem) {
			this.selectionGeometryItem = selectionGeometryItem;
			DefaultStyleKey = typeof(SelectionPresentation);
			SetBinding(SelectionPresentation.IsSelectedProperty, new Binding("IsSelected") { Source = selectionGeometryItem.SelectionInfo });
			SetBinding(SelectionPresentation.IsHighlightedProperty, new Binding("IsHighlighted") { Source = selectionGeometryItem.SelectionInfo });
		}
		protected override Size MeasureOverride(Size availableSize) {
			return Size.Empty;
		}
	}
	[NonCategorized]
	public class SelectionGeometryControl : SelectionControl {
		public static readonly DependencyProperty GeometryProperty = DependencyPropertyManager.Register("Geometry", typeof(List<Rect>), typeof(SelectionGeometryControl), new PropertyMetadata(new List<Rect>()));
		public List<Rect> Geometry {
			get { return (List<Rect>)GetValue(GeometryProperty); }
			set { SetValue(GeometryProperty, value); }
		}
		public SelectionGeometryControl() {
			DefaultStyleKey = typeof(SelectionGeometryControl);
		}
		public bool ShouldSerializeGeometry(XamlDesignerSerializationManager manager) {
			return false;
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	[NonCategorized]
	public class SelectionInfo : NotifyPropertyChangedObject {
		bool isSelected;
		bool isHighlighted;
		public bool IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value) {
					isSelected = value;
					OnPropertyChanged("IsSelected");
				}
			}
		}
		public bool IsHighlighted {
			get { return isHighlighted; }
			set {
				if (isHighlighted != value) {
					isHighlighted = value;
					OnPropertyChanged("IsHighlighted");
				}
			}
		}
	}
	[NonCategorized]
	public class SelectionGeometryItem : NotifyPropertyChangedObject {
		SelectionInfo selectionInfo;
		List<Rect> geometry;
		public SelectionInfo SelectionInfo { get { return selectionInfo; } }
		public List<Rect> Geometry {
			get { return geometry; }
			set {
				if (geometry != value) {
					geometry = value;
					OnPropertyChanged("Geometry");
				}
			}
		}
		public SelectionGeometryItem(SelectionInfo selectionInfo) {
			this.selectionInfo = selectionInfo;
		}
	}
}

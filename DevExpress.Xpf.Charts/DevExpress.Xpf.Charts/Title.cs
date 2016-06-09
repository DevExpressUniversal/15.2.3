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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class Title : TitleBase, IHitTestableElement, IInteractiveElement, ISupportVisibilityControlElement {
		public static readonly DependencyProperty DockProperty = DependencyPropertyManager.Register("Dock", typeof(Dock), typeof(Title), new PropertyMetadata(Dock.Top, ChartElementHelper.Update));
		static Title() {
			bool? newValue = null; 
			VisibleProperty.OverrideMetadata(typeof(Title), new FrameworkPropertyMetadata(newValue));
		}
		SelectionInfo selectionInfo;
		Size availableSize;
		Rect bounds = Rect.Empty;
		bool autoLayoutVisible = true;
		int visibilityPriority = int.MaxValue;
		UIElement rootElement;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("TitleDock"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public Dock Dock {
			get { return (Dock)GetValue(DockProperty); }
			set { SetValue(DockProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SelectionInfo SelectionInfo { get { return selectionInfo; } }
		public Title() {
			DefaultStyleKey = typeof(Title);
			selectionInfo = new SelectionInfo();
		}
		#region IHitTestableElement implementation
		Object IHitTestableElement.Element { get { return this; } }
		Object IHitTestableElement.AdditionalElement { get { return null; } }
		#endregion
		#region IInteractiveElement implementation
		bool IInteractiveElement.IsHighlighted {
			get { return selectionInfo.IsHighlighted; }
			set { selectionInfo.IsHighlighted = value; }
		}
		bool IInteractiveElement.IsSelected {
			get { return selectionInfo.IsSelected; }
			set { selectionInfo.IsSelected = value; }
		}
		object IInteractiveElement.Content { get { return this; } }
		#endregion
		Geometry CalcHitTestableGeometry() {
			UIElement owner = ((IOwnedElement)this).Owner as UIElement;
			RectangleGeometry geometry = new RectangleGeometry();
			if (this != null && owner != null)
				geometry.Rect = LayoutHelper.GetRelativeElementRect(this, owner);
			return geometry;
		}
		internal void Assign(Title title) {
			if (title != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, title, DockProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, ContentProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, StyleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, TemplateProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, HorizontalAlignmentProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, VerticalAlignmentProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, MarginProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, PaddingProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, FontFamilyProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, FontSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, ForegroundProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, FontStretchProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, FontStyleProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, FontWeightProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, title, VisibilityProperty);
			}
		}
		internal override bool ActualVisible {
			get {
				if (Visible.HasValue)
					return Visible.Value;
				return autoLayoutVisible;
			}
		}
		protected override Size MeasureOverride(Size constraint) {
			Size size = base.MeasureOverride(constraint);
			if (!size.IsEmpty && size.Height != 0 && size.Width != 0)
				availableSize = size;
			return size;
		}
		#region ISupportVisibilityControlElement implementation
		int ISupportVisibilityControlElement.Priority {
			get {
				return visibilityPriority;
			}
		}
		bool ISupportVisibilityControlElement.Visible {
			get {
				return ActualVisible;
			}
			set {
				if (!this.Visible.HasValue) {
					autoLayoutVisible = value;
					Visibility = autoLayoutVisible ? Visibility.Visible : Visibility.Collapsed;
				}
			}
		}
		GRealRect2D ISupportVisibilityControlElement.Bounds {
			get {
				return new GRealRect2D(0, 0, availableSize.Width + Margin.Left + Margin.Right, availableSize.Height + Margin.Bottom + Margin.Top);
			}
		}
		VisibilityElementOrientation ISupportVisibilityControlElement.Orientation {
			get {
				if (this.Dock == System.Windows.Controls.Dock.Left || this.Dock == System.Windows.Controls.Dock.Right)
					return VisibilityElementOrientation.Vertical;
				return VisibilityElementOrientation.Horizontal;
			}
		}
		#endregion
		internal void SetVisibilityPriority(int visibilityPriority) {
			if (this.visibilityPriority > visibilityPriority)
				this.visibilityPriority = visibilityPriority;
		}
		internal void SetRootElement(UIElement rootElement) {
			this.rootElement = rootElement;
		}
	}
	public class TitleCollection : ChartElementCollection<Title> {
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (Title title in e.NewItems) {
					title.SetVisibilityPriority((int)ChartElementVisibilityPriority.SeriesTitle);
					Series series = this.Owner as Series;
					if (series != null && series.Diagram != null)
						title.SetRootElement(series.Diagram.ChartControl);
				}
			base.OnCollectionChanged(e);
		}
	}
	public class ChartTitleCollection : TitleCollection {
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Add)
				foreach (Title title in e.NewItems) {
					title.SetVisibilityPriority((int)ChartElementVisibilityPriority.ChartTitle);
					title.SetRootElement(this.Owner as UIElement);
				}
			base.OnCollectionChanged(e);
		}
	}
}

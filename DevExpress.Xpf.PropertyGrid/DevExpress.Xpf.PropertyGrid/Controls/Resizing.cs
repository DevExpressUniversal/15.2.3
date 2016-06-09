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

using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using DevExpress.Data.Utils;
using System.Threading;
using System.Windows.Data;
using DevExpress.Xpf.PropertyGrid.Internal;
using System.Windows.Documents;
namespace DevExpress.Xpf.PropertyGrid {
	public interface IPropertyGridResizingStrategy {
		bool AdjustAvailableSize(ref Size availableSize);
		void SetCurrentRow(RowControl row);
		void ResetCurrentRow(RowControl row);
		double HeaderPercent { get; }
		event EventHandler HeaderPercentChanged;
	}
	public class NullPropertyGridResizingStrategy : IPropertyGridResizingStrategy {
		static NullPropertyGridResizingStrategy instance = new NullPropertyGridResizingStrategy();
		public static NullPropertyGridResizingStrategy Instance { get { return instance; } }
		NullPropertyGridResizingStrategy() { }
		event EventHandler IPropertyGridResizingStrategy.HeaderPercentChanged { add { } remove { } }
		public bool AdjustAvailableSize(ref Size availableSize) { return false; }
		public void SetCurrentRow(RowControl row) { }
		public void ResetCurrentRow(RowControl row) { }
		public void PostHeaderPosition(double value) { }
		public double HeaderPercent { get { return 0.5d; } }
	}
	public class PropertyGridResizingStrategy : IPropertyGridResizingStrategy {
		readonly PropertyGridView view;
		protected PropertyGridView View { get { return view; } }		
		protected RowDataGenerator Generator { get { return view.PropertyGrid.With(x=>x.RowDataGenerator); } }
		protected RowsCollectionView RowsCollectionView { get { return Generator.With(x=>x.ItemsSource); } }
		public PropertyGridResizingStrategy(PropertyGridView propertyGridView) {
			this.view = propertyGridView;
			view.MouseMove += OnViewMouseMove;
			view.SizeChanged += OnViewSizeChanged;
			view.MouseLeftButtonDown += OnViewMouseLeftButtonDown;
			view.MouseLeftButtonUp += OnViewMouseLeftButtonUp;
		}		
		#region inifinite size check
		double? infiniteWidth = null;
		public bool AdjustAvailableSize(ref Size availableSize) {
			if (UpdateAvailableSize(ref availableSize))
				return true;
			if (double.IsInfinity(availableSize.Width)) {
				infiniteWidth = CalcInfiniteHeaderWidthImpl() / headerPercent + 1;
				UpdateAvailableSize(ref availableSize);
				return true;
			}
			return false;
		}
		bool UpdateAvailableSize(ref Size availableSize) {
			if (infiniteWidth.HasValue) {
				availableSize.Width = infiniteWidth.Value;
				return true;
			}
			return false;
		}
		double CalcInfiniteHeaderWidthImpl() {
			int index = 0;
			double ew = View.GetResourceProvider().GetExpanderWidth();
			double totalWidth = 0d;
			var hcmw = View.PropertyGrid.HeaderColumnMinWidth;
			while (index < 10 && index < RowsCollectionView.Count) {
				var data = RowsCollectionView.GetItemAt(index) as RowDataBase;
				if (data == null)
					break;
				var ft = new FormattedText(data.Header, System.Globalization.CultureInfo.CurrentUICulture, View.FlowDirection, GetTypeFace(View), TextElement.GetFontSize(View), View.Foreground);
				totalWidth += Math.Max(ft.Width + ew * data.Level, hcmw);
				index++;
			}
			if (index == 0d)
				return hcmw;
			return totalWidth / index;
		}
		Typeface GetTypeFace(DependencyObject element) {
			FontFamily fontFamily = (FontFamily)element.GetValue(TextElement.FontFamilyProperty);
			FontStyle fontStyle = (FontStyle)element.GetValue(TextElement.FontStyleProperty);
			FontWeight fontWeight = (FontWeight)element.GetValue(TextElement.FontWeightProperty);
			FontStretch fontStretch = (FontStretch)element.GetValue(TextElement.FontStretchProperty);
			return new Typeface(fontFamily, fontStyle, fontWeight, fontStretch);
		}
		#endregion //inifinite size check
		#region resizing
		double headerPercent = 0.5d;
		public double HeaderPercent {
			get { return headerPercent; }
			protected set {
				if (headerPercent == value)
					return;
				headerPercent = value;
				if (HeaderPercentChanged == null)
					return;
				HeaderPercentChanged(this, EventArgs.Empty);
			}
		}
		double? rowWidth = null;
		int currentRowHC = -1;
		bool dragging;
		HeaderShowMode headerShowMode = HeaderShowMode.OnlyHeader;
		bool mouseOverSplitter;
		public event EventHandler HeaderPercentChanged;
		protected double RowWidth { get { return rowWidth ?? (double)(rowWidth = View.ItemsPresenter.ActualWidth); } }		
		protected bool MouseOverSplitter {
			get { return mouseOverSplitter; }
			set {
				if (mouseOverSplitter == value)
					return;
				mouseOverSplitter = value;
				OnMouseOverSplitterChanged();
			}
		}		
		protected bool Dragging {
			get { return dragging; }
			set {
				if (dragging == value)
					return;
				dragging = value;
				OnDraggingChanged();
			}
		}		
		protected virtual void OnViewSizeChanged(object sender, SizeChangedEventArgs e) {
			rowWidth = null;
		}
		protected virtual void OnViewMouseMove(object sender, MouseEventArgs e) {
			UpdateMouseOverSplitter();
			if (!Dragging)
				return;
			var mousePosition = Mouse.GetPosition(View.ItemsPresenter).X;
			if (mousePosition > RowWidth) {
				HeaderPercent = 1d;
				return;
			}		  
			if(mousePosition<=0) {
				HeaderPercent = 0d;
				return;
			}
			HeaderPercent = mousePosition / RowWidth;
		}
		protected virtual void OnViewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {			
			Dragging = false;
		}
		protected virtual void OnViewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if (!MouseOverSplitter)
				return;
			Dragging = true;
		}
		void UpdateMouseOverSplitter() {
			if (Dragging)
				return;
			if (headerShowMode != HeaderShowMode.Left) {
				MouseOverSplitter = false;
				return;
			}				
			double currentSplitterPosition = headerPercent * RowWidth;
			double halfSplitterWidth = View.GetResourceProvider().GetSplitterWidth() / 2;
			double min = currentSplitterPosition - halfSplitterWidth;
			double max = currentSplitterPosition + halfSplitterWidth;
			var mPosition = Mouse.GetPosition(View.ItemsPresenter);
			MouseOverSplitter = mPosition.X >= min && mPosition.X <= max;
		}
		void OnMouseOverSplitterChanged() {
			if(MouseOverSplitter) {
				View.Cursor = Cursors.SizeWE;
			} else {
				View.Cursor = Cursors.Arrow;
			}
		}
		protected virtual void OnDraggingChanged() {
			if (Dragging)
				Mouse.Capture(View, CaptureMode.SubTree);
			else
				View.ReleaseMouseCapture();
		}		
		public void SetCurrentRow(RowControl row) {
			currentRowHC = row.GetHashCode();
			headerShowMode = row.HeaderShowMode;
		}
		public void ResetCurrentRow(RowControl row) {
			if (currentRowHC != row.GetHashCode())
				return;
			currentRowHC = -1;
			headerShowMode = HeaderShowMode.OnlyHeader;
			UpdateMouseOverSplitter();
		}				
		#endregion //resizing        
	}
}

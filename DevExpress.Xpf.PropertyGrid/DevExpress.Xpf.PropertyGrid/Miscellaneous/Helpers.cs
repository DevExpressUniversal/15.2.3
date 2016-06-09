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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
namespace DevExpress.Xpf.PropertyGrid {
	public static class PropertyGridViewHelper {
		public static RowControlBase GetRowControl(this ItemsControl propertyGridView, RowDataBase data) {
			if (data == null)
				return null;
			if (data.Parent == null || data.Parent.Handle.IsRoot)
				return propertyGridView.ItemContainerGenerator.ContainerFromItem(data) as RowControlBase;
			var rowControl = GetRowControl(propertyGridView, data.Parent) as RowControlBase;
			if (rowControl == null)
				return null;
			if (rowControl.RowData == data)
				return rowControl;
			return null;
		}
	}
	public class PropertyGridHelper {
		public static PropertyGridView GetView(DependencyObject obj) {
			return (PropertyGridView)obj.GetValue(ViewProperty);
		}
		public static void SetView(DependencyObject obj, PropertyGridView value) {
			obj.SetValue(ViewProperty, value);
		}
		public static RowControlBase GetRowControl(DependencyObject obj) {
			if (obj == null)
				return null;
			return (RowControlBase)obj.GetValue(RowControlProperty);
		}
		public static void SetRowControl(DependencyObject obj, RowControlBase value) {
			obj.SetValue(RowControlProperty, value);
		}
		public static RowDataBase GetRowData(DependencyObject obj) {
			return (RowDataBase)obj.GetValue(RowDataProperty);
		}
		public static void SetRowData(DependencyObject obj, RowDataBase value) {
			obj.SetValue(RowDataProperty, value);
		}
		public static Size GetViewportSize(DependencyObject obj) {
			return (Size)obj.GetValue(ViewportSizeProperty);
		}
		public static void SetViewportSize(DependencyObject obj, Size value) {
			obj.SetValue(ViewportSizeProperty, value);
		}
		public static CellEditorPresenter GetEditorPresenter(DependencyObject obj) {
			if (obj == null)
				return null;
			return (CellEditorPresenter)obj.GetValue(EditorPresenterProperty);
		}
		public static void SetEditorPresenter(DependencyObject obj, CellEditorPresenter value) {
			obj.SetValue(EditorPresenterProperty, value);
		}
		public static PropertyGridControl GetPropertyGrid(DependencyObject obj) {
			return (PropertyGridControl)obj.GetValue(PropertyGridProperty);
		}
		public static void SetPropertyGrid(DependencyObject obj, PropertyGridControl value) {
			obj.SetValue(PropertyGridProperty, value);
		}
		public static readonly DependencyProperty PropertyGridProperty =
			DependencyProperty.RegisterAttached("PropertyGrid", typeof(PropertyGridControl), typeof(PropertyGridHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty EditorPresenterProperty =
			DependencyProperty.RegisterAttached("EditorPresenter", typeof(CellEditorPresenter), typeof(PropertyGridHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty ViewportSizeProperty =
			DependencyProperty.RegisterAttached("ViewportSize", typeof(Size), typeof(PropertyGridHelper), new FrameworkPropertyMetadata(new Size(), FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty RowDataProperty =
			DependencyPropertyManager.RegisterAttached("RowData", typeof(RowDataBase), typeof(PropertyGridHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty RowControlProperty =
			DependencyPropertyManager.RegisterAttached("RowControl", typeof(RowControlBase), typeof(PropertyGridHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
		public static readonly DependencyProperty ViewProperty =
			DependencyPropertyManager.RegisterAttached("View", typeof(PropertyGridView), typeof(PropertyGridHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
	}
}
namespace DevExpress.Xpf.PropertyGrid.Internal {
	public class DoubleClickHelper {
		public event MouseButtonEventHandler DoubleClick;
		public bool IsClicked { get; private set; }
		System.Windows.Threading.DispatcherTimer timer;
		public DoubleClickHelper(FrameworkElement owner) {
			this.IsClicked = false;
			owner.MouseLeftButtonUp += OnOwnerMouseLeftButtonUp;
			timer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(System.Windows.Forms.SystemInformation.DoubleClickTime) };
			timer.Tick += (o, e) => Reset();
		}
		object olock = new object();
		void OnOwnerMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
			lock (olock) {
				if (IsClicked) {
					IsClicked = false;
					timer.Stop();
					OnDoubleClick(sender, e);
				}
				else {
					IsClicked = true;
					timer.Start();
				}
			}
		}
		void Reset() {
			timer.Stop();
			lock (olock) {
				IsClicked = false;
			}
		}
		private void OnDoubleClick(object sender, MouseButtonEventArgs e) {
			if (DoubleClick != null) {
				DoubleClick(sender, e);
			}
		}
	}
	public class PropertyGridEditSettingsHelper {
		public static readonly DependencyProperty IsReadOnlyProperty =
			DependencyPropertyManager.RegisterAttached("IsReadOnly", typeof(bool), typeof(PropertyGridEditSettingsHelper), new PropertyMetadata(false));
		public static readonly DependencyProperty IsStandardEditSettingsProperty =
			DependencyPropertyManager.RegisterAttached("IsStandardEditSettings", typeof(bool), typeof(PropertyGridEditSettingsHelper), new PropertyMetadata(false));
		public static readonly DependencyProperty PostOnEditValueChangedProperty =
			DependencyPropertyManager.RegisterAttached("PostOnEditValueChanged", typeof(bool), typeof(PropertyGridEditSettingsHelper), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty PostOnPopupClosedProperty =
			DependencyPropertyManager.RegisterAttached("PostOnPopupClosed", typeof(bool), typeof(PropertyGridEditSettingsHelper), new FrameworkPropertyMetadata(false));
		public static bool GetPostOnPopupClosed(BaseEditSettings obj) {
			if (obj == null)
				return false;
			return (bool)obj.GetValue(PostOnPopupClosedProperty);
		}
		public static void SetPostOnPopupClosed(BaseEditSettings obj, bool value) {
			if (obj == null)
				return;
			obj.SetValue(PostOnPopupClosedProperty, value);
		}
		public static bool GetPostOnEditValueChanged(BaseEditSettings obj) {
			if (obj == null)
				return false;
			return (bool)obj.GetValue(PostOnEditValueChangedProperty);
		}
		public static void SetPostOnEditValueChanged(BaseEditSettings obj, bool value) {
			if (obj == null)
				return;
			obj.SetValue(PostOnEditValueChangedProperty, value);
		}
		public static bool GetIsStandardEditSettings(DependencyObject obj) {
			if (obj == null)
				return false;
			return (bool)obj.GetValue(IsStandardEditSettingsProperty);
		}
		public static void SetIsStandardEditSettings(DependencyObject obj, bool value) {
			if (obj == null)
				return;
			obj.SetValue(IsStandardEditSettingsProperty, value);
		}
		public static bool GetIsReadOnly(DependencyObject obj) {
			if (obj == null)
				return false;
			return (bool)obj.GetValue(IsReadOnlyProperty);
		}
		public static void SetIsReadOnly(DependencyObject obj, bool value) {
			obj.SetValue(IsReadOnlyProperty, value);
		}
	}
	internal class ConditionalVisualTreeEnumerator : VisualTreeEnumerator, IEnumerator<DependencyObject> {
		Predicate<DependencyObject> predicate;
		public ConditionalVisualTreeEnumerator(DependencyObject obj, Predicate<DependencyObject> predicate)
			: base(obj) {
			this.predicate = predicate;
		}
		protected override System.Collections.IEnumerator GetNestedObjects(object obj) {
			if (!predicate(obj as DependencyObject))
				return NestedObjectEnumeratorBase.EmptyEnumerator;
			return base.GetNestedObjects(obj);
		}
		DependencyObject IEnumerator<DependencyObject>.Current {
			get { return Current as DependencyObject; }
		}
		void IDisposable.Dispose() {
		}
	}
}

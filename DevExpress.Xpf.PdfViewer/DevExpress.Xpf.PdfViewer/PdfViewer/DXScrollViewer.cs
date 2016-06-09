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
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Resources;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.Native;
using Point = System.Windows.Point;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Flyout;
using System.Windows.Threading;
using XmlNamespaceConstants = DevExpress.Xpf.Core.Native.XmlNamespaceConstants;
namespace DevExpress.Xpf.PdfViewer {
	public class DXScrollViewer : DevExpress.Xpf.DocumentViewer.DXScrollViewer {
		public static readonly DependencyProperty VerticalScrollBarWidthProperty;
		static readonly DependencyPropertyKey VerticalScrollBarWidthPropertyKey;
		static DXScrollViewer() {
			Type ownerType = typeof(DXScrollViewer);
			VerticalScrollBarWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("VerticalScrollBarWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d));
			VerticalScrollBarWidthProperty = VerticalScrollBarWidthPropertyKey.DependencyProperty;
		}
		public double VerticalScrollBarWidth {
			get { return (double)GetValue(VerticalScrollBarWidthProperty); }
			private set { SetValue(VerticalScrollBarWidthPropertyKey, value); }
		}
		ScrollBar VerticalScrollBar { get; set; }
		const string VerticalScrollBarPart = "PART_VerticalScrollBar";
		public DXScrollViewer() {
			SizeChanged += OnSizeChanged;
			LayoutUpdated += OnLayoutUpdated;
		}
		private void OnLayoutUpdated(object sender, EventArgs e) {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			VerticalScrollBar.Do(x => x.SizeChanged -= OnVerticalScrollBarSizeChanged);
			VerticalScrollBar = (ScrollBar)GetTemplateChild(VerticalScrollBarPart);
			VerticalScrollBar.Do(x => x.SizeChanged += OnVerticalScrollBarSizeChanged);
		}
		protected virtual void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateVerticalScrollBarWidth();
		}
		protected virtual void OnVerticalScrollBarSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateVerticalScrollBarWidth();
		}
		void UpdateVerticalScrollBarWidth() {
			VerticalScrollBarWidth = VerticalScrollBar.DesiredSize.Width;
		}
	}
	public class CursorModeToCanMouseScrollConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (CursorModeType)value == CursorModeType.HandTool;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (bool)value ? CursorModeType.HandTool : CursorModeType.SelectTool;
		}
	}
	public class CursorModeToSelectionVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (CursorModeType)value == CursorModeType.SelectTool ? Visibility.Hidden : Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (Visibility)value == Visibility.Hidden ? CursorModeType.SelectTool : CursorModeType.HandTool;
		}
	}
}

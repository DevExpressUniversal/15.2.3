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
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Pdf;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfOutlinesViewerNodeSelector : TreeListNodeImageSelector {
		public PdfOutlinesViewerNodeSelector() {
		}
		public override ImageSource Select(TreeListRowData rowData) {
			string dllName = Assembly.GetExecutingAssembly().GetName().Name;
			return new BitmapImage(UriHelper.GetUri(dllName, @"Images\Outlines\Bookmark_16x16.png"));
		}
	}
	public class PdfColorToColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			PdfColor pdfColor = (PdfColor)value;
			Color color = Color.FromArgb(Byte.MaxValue, (byte)(Byte.MaxValue * pdfColor.Components[0]), (byte)(Byte.MaxValue * pdfColor.Components[1]), (byte)(Byte.MaxValue * pdfColor.Components[2]));
			return new SolidColorBrush(color);
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PdfItalicToItalicConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool isItalic = (bool)value;
			return isItalic ? FontStyles.Italic : FontStyles.Normal;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PdfBoldToBoldConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			bool isBold = (bool)value;
			return isBold ? FontWeights.Bold : FontWeights.Normal;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class ShowingDockHintsAttachedBehavior : Behavior<DockLayoutManager> {
		protected override void OnAttached() {
			base.OnAttached();
			AssociatedObject.ShowingDockHints += AssociatedObjectOnShowingDockHints;
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			AssociatedObject.ShowingDockHints -= AssociatedObjectOnShowingDockHints;
		}
		void AssociatedObjectOnShowingDockHints(object sender, ShowingDockHintsEventArgs e) {
			e.Hide(DockGuide.Top);
			e.Hide(DockGuide.Bottom);
			e.Hide(DockGuide.Center);
			e.Handled = true;
		}
	}
}

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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	#region Inner classes
	public class SeriesViewItem {
		public SeriesViewItem(ImageSource image, string name, SeriesViewType seriesViewType) {
			this.image = image;
			this.name = name;
			this.seriesViewType = seriesViewType;
		}
		readonly ImageSource image;
		public ImageSource Image { get { return image; } }
		readonly string name;
		public string Name { get { return name; } }
		readonly SeriesViewType seriesViewType;
		public SeriesViewType SeriesViewType { get { return seriesViewType; } }
	}
	#endregion
	public class SeriesViewUITypeEditor : Control {
		public static readonly DependencyProperty EditValueProperty;
		static readonly DependencyPropertyKey SeriesViewsPropertyKey;
		public static readonly DependencyProperty SeriesViewsProperty;
		static SeriesViewUITypeEditor() {
			DependencyPropertyRegistrator<SeriesViewUITypeEditor>.New()
				.Register(d => d.EditValue, out EditValueProperty, new SeriesViewType(), d => d.OnEditValueChanged(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.RegisterReadOnly(d => d.SeriesViews, out SeriesViewsPropertyKey, out SeriesViewsProperty, new List<SeriesViewItem>())
				.OverrideDefaultStyleKey()
			;
		}
		public SeriesViewType EditValue {
			get { return (SeriesViewType)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		void OnEditValueChanged() {
			SeriesViews.Clear();
			for(int i = 0; i < SeriesViewFactory.StringIDs.Length; i++) {
				var seriesViewType = new SeriesViewType(EditValue.View, SeriesViewFactory.GetViewType(SeriesViewFactory.StringIDs[i]));
				SeriesViews.Add(new SeriesViewItem(WinImageHelper.GetImageSource(SeriesViewFactory.SeriesViewImages[i]), SeriesViewFactory.StringIDs[i], seriesViewType));
			}
		}
		public List<SeriesViewItem> SeriesViews {
			get { return (List<SeriesViewItem>)GetValue(SeriesViewsProperty); }
			private set { SetValue(SeriesViewsPropertyKey, value); }
		}
	}
}

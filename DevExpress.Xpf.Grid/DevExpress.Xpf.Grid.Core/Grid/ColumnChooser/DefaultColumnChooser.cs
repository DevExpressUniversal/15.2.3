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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Utils;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	class ColumnChooserCaptionLocalizationStringConvertor : IValueConverter {
		readonly DataViewBase view;
		public ColumnChooserCaptionLocalizationStringConvertor(DataViewBase view) {
			this.view = view;
		}
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			LocalizationDescriptor descriptor = value as LocalizationDescriptor;
			if(descriptor == null) {
				return null;
			}
			if(view.DataControl.AllowBandChooser)
				return descriptor.GetValue(GridControlRuntimeStringId.ColumnBandChooserCaption.ToString());
			return view.DetailHeaderContent == null ?
				descriptor.GetValue(GridControlRuntimeStringId.ColumnChooserCaption.ToString()) :
				string.Format(descriptor.GetValue(GridControlRuntimeStringId.ColumnChooserCaptionForMasterDetail.ToString()), view.DetailHeaderContent);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class DefaultColumnChooser : ColumnChooserBase {
		public DefaultColumnChooser(DataViewBase view)
			: base(view) {
				UpdateCaption();
		}
#if SL
		protected override void OnOwnerUnloaded(object sender, RoutedEventArgs e) {
			View.HideColumnChooser();
		}
#endif
		protected DataViewBase View { get { return (DataViewBase)base.Owner; } }
		protected override ILogicalOwner Owner { get { return View.RootView; } }
		protected override Control CreateContentControl() {
			Control contentControl = base.CreateContentControl();
			DataControlBase.SetCurrentViewInternal(contentControl, View);
			string propName = DataViewBase.ActualColumnChooserTemplateProperty.GetName();
			contentControl.SetBinding(Control.TemplateProperty,
				new Binding(propName) { Source = View });
			return contentControl;
		}
		protected override void OnContainerHidden(object sender, RoutedEventArgs e) {
			View.IsColumnChooserVisible = false;
		}
		internal void UpdateCaption() {
			BindingOperations.SetBinding(this, ColumnChooserBase.CaptionProperty, new Binding("LocalizationDescriptor") { Source = View, Converter = new ColumnChooserCaptionLocalizationStringConvertor(View) });
		}
	}
	public sealed class DefaultColumnChooserFactory : IColumnChooserFactory {
		public static readonly DefaultColumnChooserFactory Instance = new DefaultColumnChooserFactory();
		#region IColumnChooserFactory Members
		IColumnChooser IColumnChooserFactory.Create(Control owner) {
			return new DefaultColumnChooser((DataViewBase)owner);
		}
		#endregion
	}
}

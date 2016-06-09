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
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Reports.UserDesigner.Editors.Native;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm;
using DevExpress.Xpf.Diagram;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using System.Collections;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class DiagramItemTypeToVisibilityConverterExtension : MarkupExtension {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return new DiagramItemTypeToVisibilityConverter();
		}
	}
	public class DiagramItemTypeToVisibilityConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(values == null || values[0] == DependencyProperty.UnsetValue) return null;
			var func = (Func<IMultiModel, bool>)values[0];
			var multiModel = (IMultiModel)values[1];
			return func(multiModel) ? Visibility.Visible : Visibility.Collapsed;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public static class SelectionModelHelper<TRoot, T> where T: class{
		public static T GetUnderlyingItem(IMultiModel item) {
			var propertiesProvider = (PropertiesProvider<TRoot, T>)(item).PropertiesProvider;
			return propertiesProvider.Context.GetComponent(propertiesProvider.MainComponent);
		}
	}
	public class SubBandCollectionUITypeEditor : SingleSelectionCollectionEditor {
		public static readonly DependencyProperty ReportModelProperty;
		static SubBandCollectionUITypeEditor() {
			DependencyPropertyRegistrator<SubBandCollectionUITypeEditor>.New()
				.Register(owner => owner.ReportModel, out ReportModelProperty, null)
				.OverrideDefaultStyleKey()
			;
		}
		public XtraReportModelBase ReportModel {
			get { return (XtraReportModelBase)GetValue(ReportModelProperty); }
			set { SetValue(ReportModelProperty, value); }
		}
		public override Func<IMultiModel, bool> IsEditorItem { get { return item => SelectionModelHelper<IDiagramItem, DiagramItem>.GetUnderlyingItem(item) is BandDiagramItem; } }
		public override object CreateItem() {
			return ReportModel.DiagramItem.Diagram.ItemFactory(new SubBand());
		}
	}
}

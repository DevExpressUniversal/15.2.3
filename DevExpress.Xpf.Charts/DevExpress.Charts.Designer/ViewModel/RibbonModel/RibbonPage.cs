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

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Charts.Designer.Native {
	public static class RibbonPagesNames {
		public const string DefaultCategoryChartPage = "DefaultCategory_ChartPage";
		public const string DefaultCategoryElementsPage = "DefaultCategory_ElementsPage";
		public const string AxisOptionsPage = "AxisOptionsPage";
		public const string SeriesOptionsMainPage = "SeriesOptions_MainPage";
		public const string SeriesOptionsDataPage = "SeriesOptions_DataPage";
		public const string ChartTitleOptionsPage = "ChartTitleOptionsPage";
		public const string ConstantLineOptionsPage = "ConstantLineOptionsPage";
		public const string IndicatorOptionsPage = "IndicatorOptionsPage";
		public const string LegendOptionsPage = "LegendOptionsPage";
		public const string StripOptionsPage = "StripOptionsPage";
	}
	public class RibbonPageViewModel : RibbonItemViewModelBase {
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RibbonPageViewModel),
			new PropertyMetadata(false));
		string name;
		ObservableCollection<RibbonPageGroupViewModel> groups = new ObservableCollection<RibbonPageGroupViewModel>();
		public ObservableCollection<RibbonPageGroupViewModel> Groups {
			get { return groups; }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public string Name { 
			get { return name; } 
		}
		public RibbonPageViewModel(WpfChartModel chartModel, string name) {
			this.name = name;
			Binding binding = new Binding("SelectedPageName") { Source = chartModel, Mode = BindingMode.TwoWay, Converter = new PageSelectedConverter(), ConverterParameter = name };
			BindingOperations.SetBinding(this, IsSelectedProperty, binding);
		}
		public override void CleanUp() {
			base.CleanUp();
			foreach (var model in Groups)
				model.CleanUp();
		}
	}
}

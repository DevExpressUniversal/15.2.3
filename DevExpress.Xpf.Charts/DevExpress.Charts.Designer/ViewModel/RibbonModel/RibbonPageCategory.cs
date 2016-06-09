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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
namespace DevExpress.Charts.Designer.Native {
	public sealed class RibbonPageCategoryViewModel : RibbonItemViewModelBase {
		public static readonly DependencyProperty VisibleProperty = DependencyProperty.Register("Visible", typeof(bool), typeof(RibbonPageCategoryViewModel),
			new PropertyMetadata(false));
		readonly bool isDefaultCategory = false;
		ObservableCollection<RibbonPageViewModel> pages = new ObservableCollection<RibbonPageViewModel>();
		public ObservableCollection<RibbonPageViewModel> Pages {
			get { return pages; }
		}
		public bool IsDefaultCategory {
			get { return isDefaultCategory; }
		}
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		public RibbonPageCategoryViewModel(WpfChartModel chartModel) 
			: this(chartModel, null) { }
		public RibbonPageCategoryViewModel(WpfChartModel chartModel, Type selectedModelType) {
			if (selectedModelType == null) {
				this.isDefaultCategory = true;
				this.Visible = true;
			} 
			else {
				this.isDefaultCategory = false;
				Binding binding = new Binding("SelectedModel") { Source = chartModel, Converter = new PageCategoryVisibleConverter(), ConverterParameter = selectedModelType };
				BindingOperations.SetBinding(this, VisibleProperty, binding);
			}
		}
		public override void CleanUp() {
			base.CleanUp();
			foreach (var model in Pages)
				model.CleanUp();
		}
	}
}

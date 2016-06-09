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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Core;
#if !SILVERLIGHT
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class AdditionalRowContainerControlBase : Control {
		public static readonly DependencyProperty RowTemplateProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty IndicatorWidthProperty;
		static AdditionalRowContainerControlBase() {
			Type ownerType = typeof(AdditionalRowContainerControlBase);
			RowTemplateProperty = DependencyProperty.Register("RowTemplate", typeof(ControlTemplate), ownerType, null);
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), ownerType, null);
			IndicatorWidthProperty = DependencyProperty.Register("IndicatorWidth", typeof(double), ownerType, new PropertyMetadata(16d));
		}
		public ControlTemplate RowTemplate {
			get { return (ControlTemplate)GetValue(RowTemplateProperty); }
			set { SetValue(RowTemplateProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public double IndicatorWidth {
			get { return (double)GetValue(IndicatorWidthProperty); }
			set { SetValue(IndicatorWidthProperty, value); }
		}
		protected internal abstract int RowHandle { get; }
		protected AdditionalRowContainerControlBase() {
#if !SILVERLIGHT
			SetBinding(RowData.RowDataProperty, new Binding());
#endif
			SetBinding(RowData.CurrentRowDataProperty, new Binding());
		}
	}
	[DXToolboxBrowsable(false)]
	public class AdditionalRowItemsControl : ItemsControl, INotifyCurrentViewChanged {
		ITableView View { get { return DataControlBase.GetCurrentView(this) as ITableView; } }
		#region INotifyCurrentViewChanged Members
		void INotifyCurrentViewChanged.OnCurrentViewChanged(DependencyObject d) {
			if(View != null)
				View.TableViewBehavior.AdditionalRowItemsControl = this;
		}
		#endregion
	}
}

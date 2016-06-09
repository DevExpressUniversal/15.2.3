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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using DevExpress.Utils;
using GridColumnDefinition = System.Windows.Controls.ColumnDefinition;
using GridRowDefinition = System.Windows.Controls.RowDefinition;
namespace DevExpress.Xpf.Grid {
	public class ColumnDefinition : DependencyObject {
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof(GridLength), typeof(ColumnDefinition), null);
		public static readonly DependencyProperty MinWidthProperty =
			DependencyProperty.Register("MinWidth", typeof(double), typeof(ColumnDefinition), new PropertyMetadata(8d));
		public GridLength Width {
			get { return (GridLength)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public double MinWidth {
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}
		public GridColumnDefinition CreateGridColumnDefinition() {
			GridColumnDefinition column = new GridColumnDefinition() { MinWidth = this.MinWidth };
			BindingOperations.SetBinding(column, GridColumnDefinition.WidthProperty, new Binding("Width") { Source = this, Mode = BindingMode.TwoWay });
			BindingOperations.SetBinding(column, GridColumnDefinition.MinWidthProperty, new Binding("MinWidth") { Source = this, Mode = BindingMode.TwoWay });
			return column;
		}
	}
	public class RowDefinition : DependencyObject {
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height", typeof(GridLength), typeof(RowDefinition), null);
		public GridLength Height {
			get { return (GridLength)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		public GridRowDefinition CreateGridRowDefinition() {
			return new GridRowDefinition() { Height = this.Height };
		}
	}
	public class ColumnDefinitions : List<ColumnDefinition> { }
	public class RowDefinitions : List<RowDefinition> { }
}

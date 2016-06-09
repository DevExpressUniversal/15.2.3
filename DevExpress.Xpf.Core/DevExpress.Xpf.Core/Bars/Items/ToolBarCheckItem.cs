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
using System.Linq;
using System.Text;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Bars.Customization;
namespace DevExpress.Xpf.Bars {
	public class ToolbarCheckItem : BarCheckItem {
		public ToolbarCheckItem() : this(null) { }
		public ToolbarCheckItem(Bar bar) {
			Bar = bar;
			CreateBindings();
			IsPrivate = true;
			CloseSubMenuOnClick = false;
		}
		protected virtual void CreateBindings() {
			Binding contentBinding = new Binding("Caption");
			Binding visibilityBinding = new Binding("Visible");
			Binding allowHideBinding = new Binding("AllowHide");
			contentBinding.Source = Bar;
			visibilityBinding.Source = Bar;
			contentBinding.Mode = BindingMode.OneWay;
			visibilityBinding.Mode = BindingMode.TwoWay;
			allowHideBinding.Source = Bar;
			allowHideBinding.Mode = BindingMode.OneWay;
			allowHideBinding.Converter = new DefaultBooleanToBooleanConverter();
			allowHideBinding.ConverterParameter = Bar;
			BindingOperations.SetBinding(this, ToolbarCheckItem.IsEnabledProperty, allowHideBinding);
			BindingOperations.SetBinding(this, BarItem.ContentProperty, contentBinding);
			BindingOperations.SetBinding(this, IsCheckedProperty, visibilityBinding);
		}
		public Bar Bar { get; private set; }
	}
}

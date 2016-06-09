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

using System.Windows.Data;
using System.Windows;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Grid {
	public class CardHeaderData : GridDataBase {
		public static readonly DependencyProperty BindingProperty;
		public static readonly DependencyProperty ValueProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty DataInternalProperty;
		public static readonly DependencyProperty RowDataProperty;
		static CardHeaderData() {
			BindingProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CardHeaderData, BindingBase>("Binding", null, (d, e) => d.UpdateValue());
			ValueProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CardHeaderData, object>("Value", null, (d, e) => d.OnContentChanged());
			DataInternalProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CardHeaderData, object>("DataInternal", null, (d, e) => ((CardHeaderData)d).Data = e.NewValue);
			RowDataProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<CardHeaderData, RowData>("RowData", null);
		}
		public CardHeaderData() {
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("CardHeaderDataBinding")]
#endif
		public BindingBase Binding {
			get { return (BindingBase)GetValue(BindingProperty); }
			set { SetValue(BindingProperty, value); }
		}
		public new object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		protected internal override void UpdateValue() {
			ClearValue(ValueProperty);
			if(Binding != null && Data != null)
				BindingOperations.SetBinding(this, ValueProperty, Binding);
		}
		public RowData RowData {
			get { return (RowData)GetValue(RowDataProperty); }
			set { SetValue(RowDataProperty, value); }
		}
	}
}

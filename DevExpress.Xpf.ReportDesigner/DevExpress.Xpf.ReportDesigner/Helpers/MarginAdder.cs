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
using System.Windows;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public abstract class MarginAdder {
		public static readonly DependencyProperty Margin1Property;
		public static readonly DependencyProperty Margin2Property;
		static MarginAdder() {
			DependencyPropertyRegistrator<MarginAdder>.New()
				.RegisterAttached((FrameworkElement d) => GetMargin1(d), out Margin1Property, null, UpdateMargin)
				.RegisterAttached((FrameworkElement d) => GetMargin2(d), out Margin2Property, null, UpdateMargin)
			;
		}
		public static Thickness? GetMargin1(FrameworkElement d) { return (Thickness?)d.GetValue(Margin1Property); }
		public static void SetMargin1(FrameworkElement d, Thickness? v) { d.SetValue(Margin1Property, v); }
		public static Thickness? GetMargin2(FrameworkElement d) { return (Thickness?)d.GetValue(Margin2Property); }
		public static void SetMargin2(FrameworkElement d, Thickness? v) { d.SetValue(Margin2Property, v); }
		static void UpdateMargin(FrameworkElement d) {
			var margin1 = GetMargin1(d);
			var margin2 = GetMargin2(d);
			if(margin1 != null && margin2 != null)
				d.Margin = new Thickness(margin1.Value.Left + margin2.Value.Left, margin1.Value.Top + margin2.Value.Top, margin1.Value.Right + margin2.Value.Right, margin1.Value.Bottom + margin2.Value.Bottom);
			else if(margin1 != null) {
				d.Margin = margin1.Value;
			} else if(margin2 != null) {
				d.Margin = margin2.Value;
			}
		}
	}
}

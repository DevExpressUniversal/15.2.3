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
using DevExpress.Data;
using DevExpress.Xpf.Editors.ExpressionEditor;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public class FormatExpressionEditorControl : ExpressionEditorControl {
		public static readonly DependencyProperty CurrentColumnInfoProperty = DependencyProperty.Register("CurrentColumnInfo", typeof(IDataColumnInfo), typeof(FormatExpressionEditorControl), new PropertyMetadata(null));
		public static readonly DependencyProperty BindableExpressionProperty = DependencyProperty.Register("BindableExpression", typeof(string), typeof(FormatExpressionEditorControl), new PropertyMetadata(string.Empty));
		public IDataColumnInfo CurrentColumnInfo {
			get { return (IDataColumnInfo)GetValue(CurrentColumnInfoProperty); }
			set { SetValue(CurrentColumnInfoProperty, value); }
		}
		public string BindableExpression {
			get { return (string)GetValue(BindableExpressionProperty); }
			set { SetValue(BindableExpressionProperty, value); }
		}
		public override void OnApplyTemplate() {
			ColumnInfo = CurrentColumnInfo;
			base.OnApplyTemplate();
		}
		public FormatExpressionEditorControl() {
			Loaded += OnLoaded;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			Loaded -= OnLoaded;
			Window.GetWindow(this).Closing += OnClosing;
		}
		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			((Window)sender).Closing -= OnClosing;
			BindableExpression = Expression;
		}
	}
}

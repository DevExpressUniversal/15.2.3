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

using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
using DevExpress.Xpf.Editors.ExpressionEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.DataAccess.Editors {
	public class ExpressionEditor : Decorator {
		protected class ExpressionEditorCore : ExpressionEditorControl {
			public static readonly DependencyProperty EditorLogicFuncProperty;
			static ExpressionEditorCore() {
				DependencyPropertyRegistrator<ExpressionEditorCore>.New()
				   .Register(d => d.EditorLogicFunc, out EditorLogicFuncProperty, null)
				   ;
			}
			public Func<IExpressionEditor, ExpressionEditorLogic> EditorLogicFunc {
				get { return (Func<IExpressionEditor, ExpressionEditorLogic>)GetValue(EditorLogicFuncProperty); }
				set { SetValue(EditorLogicFuncProperty, value); }
			}
			public ExpressionEditorCore(Func<IExpressionEditor, ExpressionEditorLogic> editorLogicFunc) {
				EditorLogicFunc = editorLogicFunc;
			}
			public string GetExpression(CancelEventArgs cancelEventArgs) {
				var editorLogic = ((IExpressionEditor)this).EditorLogic;
				if(editorLogic.CanCloseWithOKResult()) {
					return editorLogic.GetExpression();
				} else {
					cancelEventArgs.Cancel = true;
					return null;
				}
			}
			protected override ExpressionEditorLogic GetExpressionEditorLogic() {
				return EditorLogicFunc(this);
			}
		}
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty EditorLogicFuncProperty;
		static ExpressionEditor() {
			DependencyPropertyRegistrator<ExpressionEditor>.New()
			   .Register(d => d.EditorLogicFunc, out EditorLogicFuncProperty, null, d => d.OnEditorLogicFuncChanged())
			   .Register(d => d.EditValue, out EditValueProperty, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
			   ;
		}
		public Func<IExpressionEditor, ExpressionEditorLogic> EditorLogicFunc {
			get { return (Func<IExpressionEditor, ExpressionEditorLogic>)GetValue(EditorLogicFuncProperty); }
			set { SetValue(EditorLogicFuncProperty, value); }
		}
		public string EditValue {
			get { return (string)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public ICommand<CancelEventArgs> SaveCommand { get; private set; }
		public ExpressionEditor() {
			SaveCommand = new DelegateCommand<CancelEventArgs>(x => (((ExpressionEditorCore)Child).GetExpression(x)).Do(y => SetCurrentValue(EditValueProperty, y)), x => Child != null);
		}
		protected virtual ExpressionEditorCore GetExpressionEditor(Func<IExpressionEditor, ExpressionEditorLogic> editorLogicFunc) {
			return new ExpressionEditorCore(editorLogicFunc);
		}
		void OnEditorLogicFuncChanged() {
			if(EditorLogicFunc != null)
				Child = GetExpressionEditor(EditorLogicFunc);
		}
	}
}

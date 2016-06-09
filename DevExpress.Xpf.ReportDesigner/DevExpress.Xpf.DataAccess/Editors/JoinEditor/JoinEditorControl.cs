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
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class JoinEditorControl : Control {
		public static readonly DependencyProperty EditValueProperty;
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		static readonly Action<JoinEditorControl, Action<IMessageBoxService>> MessageBoxServiceAccessor;
		static JoinEditorControl() {
			DependencyPropertyRegistrator<JoinEditorControl>.New()
				.Register(d => d.EditValue, out EditValueProperty, null, d => d.OnEditValueChanged())
				.RegisterServiceTemplateProperty(d => d.MessageBoxServiceTemplate, out MessageBoxServiceTemplateProperty, out MessageBoxServiceAccessor)
				.OverrideDefaultStyleKey();
		}
		public JoinEditorView EditValue {
			get { return (JoinEditorView)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		void OnEditValueChanged() {
			EditValue.DoWithMessageBoxService = DoWithMessageBoxService;
		}
		public void DoWithMessageBoxService(Action<IMessageBoxService> action) { MessageBoxServiceAccessor(this, action); }
		public DataTemplate MessageBoxServiceTemplate {
			get { return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty); }
			set { SetValue(MessageBoxServiceTemplateProperty, value); }
		}
	}
}

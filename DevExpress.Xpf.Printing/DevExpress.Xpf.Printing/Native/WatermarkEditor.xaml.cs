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

using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Printing.Native {
	public partial class WatermarkEditor : DXWindow {
		public WatermarkEditorViewModel Model {
			get { return (WatermarkEditorViewModel)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}
		public static readonly DependencyProperty ModelProperty =
			DependencyPropertyManager.Register("Model", typeof(WatermarkEditorViewModel), typeof(WatermarkEditor), new PropertyMetadata(null,OnModelChanged));
		public WatermarkEditor() {
			InitializeComponent();
		}
		static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			WatermarkEditorViewModel model = e.NewValue as WatermarkEditorViewModel;
			if(model == null) {
				return;
			}
			WatermarkEditor editor = (WatermarkEditor)d;
			editor.DataContext = model;
			model.DialogService = new DialogService((FrameworkElement)d);
		}
		private void ButtonClearAllClick(object sender, RoutedEventArgs e) {
			Model.ClearAllCommand.Execute(null);
		}
		void ButtonOkClick(object sender, RoutedEventArgs e) {
			DialogResult = true;
			this.Close();
		}
		void pageRange_Validate(object sender, Editors.ValidationEventArgs e) {			
			e.IsValid = WatermarkEditorViewModel.ValidatePageRange((string)e.Value);
		}
		void fontSize_Validate(object sender, Editors.ValidationEventArgs e) {			
			e.IsValid = WatermarkEditorViewModel.ValidateFontSize((string)e.Value);
		}
	}
}

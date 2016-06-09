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

using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Printing.Native {
	public partial class ScaleWindow : DXDialog {
		public ScaleWindow(float scaleFactor, int pagesToFit) {
			InitializeComponent();
			ScaleWindowViewModel viewModel = new ScaleWindowViewModel(scaleFactor, pagesToFit);
			ViewModel = viewModel;
			Loaded += ScaleWindow_Loaded;
#if SL
			KeyUp += ScaleWindow_KeyUp;
#endif
		}
#if SL
		void ScaleWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
			if(e.Key == System.Windows.Input.Key.Enter && ViewModel.ApplyCommand.CanExecute(null)) {
				ViewModel.ApplyCommand.Execute(null);
				OnClosing();
			}
		}
#endif
		public ScaleWindowViewModel ViewModel {
			get { return LayoutRoot.DataContext as ScaleWindowViewModel; }
			set { LayoutRoot.DataContext = value; }
		}
		void ScaleWindow_Loaded(object sender, System.Windows.RoutedEventArgs e) {
			OkButton.Command = ViewModel.ApplyCommand;
		}
		void cmbBxPagesToFit_Validate(object sender, Editors.ValidationEventArgs e) {
			FinishValidation(e, ViewModel.ValidatePagesToFit(e.Value));
		}
		void cmbBxScaleFactor_Validate(object sender, Editors.ValidationEventArgs e) {
			FinishValidation(e, ViewModel.ValidateScaleFactor(e.Value));
		}
		static void FinishValidation(Editors.ValidationEventArgs e, ValidationResult validationResult) {
			e.IsValid = validationResult.IsValid;
			if(!e.IsValid)
				e.ErrorContent = validationResult.ErrorMessage;
		}
	}
}

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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class PasteSpecialFormControl : UserControl {
		public PasteSpecialFormControl(PasteSpecialFormControllerParameters controllerParameters) {
			PasteSpecialViewModel vm = new PasteSpecialViewModel(controllerParameters);
			DataContext = vm;
			InitializeComponent();
			Loaded += OnLoaded;
		}
		private void OnLoaded(object sender, RoutedEventArgs e) {
			DXDialog dialog = LayoutHelper.FindParentObject<DXDialog>(this);
			if (dialog != null) {
				dialog.Closing += OnDialogClosing;
			}
		}
		void OnDialogClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (((DXDialog)sender).DialogResult == true) {
				((PasteSpecialViewModel)DataContext).Controller.ApplyChanges();
			}
		}
		internal PasteSpecialInfo GetSourcePasteSpecialInfo() {
			return ((PasteSpecialViewModel)DataContext).Controller.SourcePasteSpecialInfo;
		}
	}
	public class PasteSpecialViewModel : BindableBase {
		public PasteSpecialViewModel(PasteSpecialFormControllerParameters controllerParameters) {
			Controller = new PasteSpecialFormController(controllerParameters);
		}
		public PasteSpecialFormController Controller { get; private set; }
		PasteSpecialListBoxItem selectedCommandType;
		public PasteSpecialListBoxItem SelectedCommandType {
			get { return selectedCommandType; }
			set {
				if (selectedCommandType == value) return;
				SetProperty(ref selectedCommandType, value, "SelectedCommandType");
				OnSelectedCommandTypeChanged();
			}
		}
		IList<PasteSpecialListBoxItem> availableCommandTypes;
		public IList<PasteSpecialListBoxItem> AvailableCommandTypes {
			get {
				if (availableCommandTypes == null) availableCommandTypes = CreateAvailableCommandTypes();
				return availableCommandTypes; 
			}
		}
		private IList<PasteSpecialListBoxItem> CreateAvailableCommandTypes() {
			List<PasteSpecialListBoxItem> result = new List<PasteSpecialListBoxItem>();
			ISpreadsheetControl control = Controller.Control;
			foreach(Type type in Controller.AvailableCommandTypes)
				result.Add(new PasteSpecialListBoxItem(control, type));
			return result;
		}
		private void OnSelectedCommandTypeChanged() {
			Controller.PasteCommandType = SelectedCommandType.CommandType;
		}
	}
}

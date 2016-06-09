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
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class PasteSpecialLocalControl : UserControl {
		public PasteSpecialLocalControl(PasteSpecialLocalFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			PasteSpecialLocalViewModel vm = new PasteSpecialLocalViewModel(controllerParameters);
			ValidateMnemonic(vm);
			DataContext = vm;
			InitializeComponent();
			Loaded += OnLoaded;
		}
		void ValidateMnemonic(PasteSpecialLocalViewModel viewModel) {
			IList<string> names = viewModel.PasteTypeNames;
			for (int i = 0; i < names.Count; i++) {
				names[i] = names[i].Replace("&", "_");
			}
		}
		private void OnLoaded(object sender, RoutedEventArgs e) {
			DXDialog dialog = LayoutHelper.FindParentObject<DXDialog>(this);
			if (dialog != null) {
				dialog.Closing += OnDialogClosing;
			}
		}
		void OnDialogClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (((DXDialog)sender).DialogResult == true) {
				((PasteSpecialLocalViewModel)DataContext).Controller.ApplyChanges();
			}
		}
		internal XtraSpreadsheet.Model.ModelPasteSpecialOptions GetSourcePasteSpecialOptions() {
			return ((PasteSpecialLocalViewModel)DataContext).Controller.SourcePasteSpecialOptions;
		}
	}
	public class PasteSpecialLocalViewModel : BindableBase {
		public PasteSpecialLocalViewModel(PasteSpecialLocalFormControllerParameters controllerParameters) {
			this.Controller = CreateController(controllerParameters);
		}
		public PasteSpecialLocalFormController Controller { get; private set; }
		IList<string> pasteTypeNames;
		public IList<string> PasteTypeNames { 
			get {
				if (pasteTypeNames == null) pasteTypeNames = Controller.GetPasteSpecialItems();
				return pasteTypeNames; 
			} 
		}
		bool skipBlans;
		public bool SkipBlanks {
			get { return skipBlans; }
			set {
				if (skipBlans == value) return;
				SetProperty(ref skipBlans, value, "SkipBlanks");
				OnSlipBlanksChanged();
			}
		}
		private void OnSlipBlanksChanged() {
			Controller.SkipBlanks = skipBlans;
		}
		int pasteType = 0;
		public int PasteType {
			get { return pasteType; }
			set {
				if (pasteType == value) return;
				SetProperty(ref pasteType, value, "PasteType");
				OnPasteTypeChanged();
			}
		}
		private void OnPasteTypeChanged() {
			Controller.CurrentPasteSpecialItemIndex = PasteType;
		}
		private PasteSpecialLocalFormController CreateController(PasteSpecialLocalFormControllerParameters controllerParameters) {
			return new PasteSpecialLocalFormController(controllerParameters);
		}
	}
}

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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing.Native {
	public enum ScaleMode { AdjustToPercent, FitToPageWidth }
	public class ValidationResult {
		public ValidationResult(bool isValid)
			: this(isValid, string.Empty) {
		}
		public ValidationResult(bool isValid, string errorMessage) {
			IsValid = isValid;
			ErrorMessage = errorMessage;
		}
		public bool IsValid { get; private set; }
		public string ErrorMessage { get; private set; }
	}
	public class ScaleWindowViewModelEventArgs : EventArgs {
		public ScaleWindowViewModelEventArgs(ScaleMode scaleMode, float scaleFactor, int pagesToFit) {
			ScaleMode = scaleMode;
			ScaleFactor = scaleFactor;
			PagesToFit = pagesToFit;
		}
		public ScaleMode ScaleMode { get; private set; }
		public float ScaleFactor { get; private set; }
		public int PagesToFit { get; private set; }
	}
	public class ScaleWindowViewModel : INotifyPropertyChanged {
		bool isInputValid = true;
		readonly int currentScaleFactor;
		int pagesToFit;
		int scaleFactor;
		ScaleMode scaleMode;
		readonly DelegateCommand<object> applyCommand;
		public event EventHandler<ScaleWindowViewModelEventArgs> ScaleApplied;
		public ScaleWindowViewModel(float scaleFactor, int pagesToFit) {
			applyCommand = DelegateCommandFactory.Create<object>(Apply, CanApply, false);
			ScaleFactorValues = new List<int>(new int[] { 10, 25, 50, 100, 200, 300, 500, 700, 1000 });
			PagesToFitValues = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			bool isPreviousScaleModeAdjustToPercent = pagesToFit == 0;
			this.pagesToFit = isPreviousScaleModeAdjustToPercent ? 1 : pagesToFit;
			this.scaleFactor = (int)Math.Round(scaleFactor * 100f);
			currentScaleFactor = this.scaleFactor;
		}
		static int MinPagesToFit {
			get { return 1; }
		}
		static int MaxPagesToFit {
			get { return 10; }
		}
		static int MinScaleFactor {
			get { return 1; }
		}
		static int MaxScaleFactor {
			get { return 1000; }
		}
		public List<int> ScaleFactorValues {
			get;
			private set;
		}
		public List<int> PagesToFitValues {
			get;
			private set;
		}
		public ScaleMode ScaleMode {
			get { return scaleMode; }
			set {
				scaleMode = value;
				RaisePropertyChanged(() => ScaleMode);
				applyCommand.RaiseCanExecuteChanged();
			}
		}
		public int ScaleFactor {
			get { return scaleFactor; }
			set {
				if(value >= MinScaleFactor && value <= MaxScaleFactor)
					scaleFactor = value;
				RaisePropertyChanged(() => ScaleFactor);
				applyCommand.RaiseCanExecuteChanged();
			}
		}
		public int PagesToFit {
			get { return pagesToFit; }
			set {
				if(value >= MinPagesToFit && value <= MaxPagesToFit)
					pagesToFit = value;
				RaisePropertyChanged(() => PagesToFit);
				applyCommand.RaiseCanExecuteChanged();
			}
		}
		public ICommand ApplyCommand {
			get { return applyCommand; }
		}
#if DEBUGTEST
		internal
#endif
		bool IsInputValid {
			get { return isInputValid; }
			set {
				if(isInputValid != value)
					isInputValid = value;
				applyCommand.RaiseCanExecuteChanged();
			}
		}
		bool CanApply(object parameter) {
			if(!IsInputValid)
				return false;
			if(ScaleMode == ScaleMode.AdjustToPercent)
				return currentScaleFactor != ScaleFactor;
			if(ScaleMode == ScaleMode.FitToPageWidth)
				return true;
			throw new NotSupportedException();
		}
		void Apply(object parameter) {
			if(ScaleApplied != null)
				ScaleApplied(this, new ScaleWindowViewModelEventArgs(ScaleMode, ToFloatFactor(ScaleFactor), PagesToFit));
		}
		static float ToFloatFactor(int value) {
			return Convert.ToSingle(value) / 100f;
		}
		public ValidationResult ValidateScaleFactor(object value) {
			return Validate(value, ScaleWindowViewModel.MinScaleFactor, ScaleWindowViewModel.MaxScaleFactor);
		}
		public ValidationResult ValidatePagesToFit(object value) {
			return Validate(value, ScaleWindowViewModel.MinPagesToFit, ScaleWindowViewModel.MaxPagesToFit);
		}
		ValidationResult Validate(object validatingValue, int minValue, int maxValue) {
			if(validatingValue == null) {
				IsInputValid = false;
				return new ValidationResult(false, PrintingLocalizer.GetString(PrintingStringId.Scaling_ComboBoxEdit_Validation_Error));
			}
			int intValue = 0;
			string text = validatingValue as String;
			if(string.IsNullOrEmpty(text)) {
				try {
					intValue = Convert.ToInt32(validatingValue);
				} catch {
					IsInputValid = false;
					return new ValidationResult(false, PrintingLocalizer.GetString(PrintingStringId.Scaling_ComboBoxEdit_Validation_Error));
				}
			} else if(!int.TryParse(text, out intValue)) {
				IsInputValid = false;
				return new ValidationResult(false, PrintingLocalizer.GetString(PrintingStringId.Scaling_ComboBoxEdit_Validation_Error)); ;
			}
			if(intValue >= minValue && intValue <= maxValue) {
				IsInputValid = true;
				return new ValidationResult(true);
			} else {
				IsInputValid = false;
				return new ValidationResult(false, PrintingLocalizer.GetString(PrintingStringId.Scaling_ComboBoxEdit_Validation_OutOfRange_Error));
			}
		}
		#region INotifyPropertyChanged Members
		public event PropertyChangedEventHandler PropertyChanged;
		void RaisePropertyChanged<T>(Expression<Func<T>> property) {
			PropertyExtensions.RaisePropertyChanged(this, PropertyChanged, property);
		}
		#endregion
	}
}

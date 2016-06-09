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

using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Forms {
	#region HeaderFooterViewModel
	public class HeaderFooterViewModel : ViewModelBase {
		#region Fields
		string oddHeader;
		string oddFooter;
		string evenHeader;
		string evenFooter;
		string firstHeader;
		string firstFooter;
		HeaderFooterBuilder oddHeaderBuilder;
		HeaderFooterBuilder oddFooterBuilder;
		HeaderFooterBuilder evenHeaderBuilder;
		HeaderFooterBuilder evenFooterBuilder;
		HeaderFooterBuilder firstHeaderBuilder;
		HeaderFooterBuilder firstFooterBuilder;
		HeaderFooterFormatTagProvider provider;
		ISpreadsheetControl control;
		bool differentOddEven = false;
		bool differentFirstPage = false;
		bool scaleWithDocument = false;
		bool alignWithMargins = false;
		bool isUpdateAllowed = false;
		#endregion
		public HeaderFooterViewModel(ISpreadsheetControl Control) {
			this.control = Control;
			provider = new HeaderFooterFormatTagProvider(DocumentModel.ActiveSheet);
			oddHeaderBuilder = new HeaderFooterBuilder(String.Empty, true);
			oddFooterBuilder = new HeaderFooterBuilder(String.Empty, true);
			evenHeaderBuilder = new HeaderFooterBuilder(String.Empty, true);
			evenFooterBuilder = new HeaderFooterBuilder(String.Empty, true);
			firstHeaderBuilder = new HeaderFooterBuilder(String.Empty, true);
			firstFooterBuilder = new HeaderFooterBuilder(String.Empty, true);
		}
		public HeaderFooterViewModel(PageSetupViewModel viewModel) {
			this.control = viewModel.Control;
			DifferentOddEven = viewModel.DifferentOddEven;
			DifferentFirstPage = viewModel.DifferentFirstPage;
			ScaleWithDocument = viewModel.ScaleWithDocument;
			AlignWithMargins = viewModel.AlignWithMargins;
			provider = viewModel.Provider;
			oddHeaderBuilder = new HeaderFooterBuilder(FormatTagHFConverter.ConvertToAnalogTag(viewModel.OddHeader), true, provider);
			oddFooterBuilder = new HeaderFooterBuilder(FormatTagHFConverter.ConvertToAnalogTag(viewModel.OddFooter), true, provider);
			evenHeaderBuilder = new HeaderFooterBuilder(FormatTagHFConverter.ConvertToAnalogTag(viewModel.EvenHeader), true, provider);
			evenFooterBuilder = new HeaderFooterBuilder(FormatTagHFConverter.ConvertToAnalogTag(viewModel.EvenFooter), true, provider);
			firstHeaderBuilder = new HeaderFooterBuilder(FormatTagHFConverter.ConvertToAnalogTag(viewModel.FirstHeader), true, provider);
			firstFooterBuilder = new HeaderFooterBuilder(FormatTagHFConverter.ConvertToAnalogTag(viewModel.FirstFooter), true, provider);
		}
		#region Properties
		DocumentModel DocumentModel { get { return control.InnerControl.DocumentModel; } }
		public ISpreadsheetControl Control { get { return control; } }
		public bool DifferentOddEven {
			get { return differentOddEven; }
			set {
				if (DifferentOddEven == value)
					return;
				differentOddEven = value;
				OnPropertyChanged("DifferentOddEven");
			}
		}
		public bool DifferentFirstPage {
			get { return differentFirstPage; }
			set {
				if (DifferentFirstPage == value)
					return;
				differentFirstPage = value;
				OnPropertyChanged("DifferentFirstPage");
			}
		}
		public bool ScaleWithDocument {
			get { return scaleWithDocument; }
			set {
				if (ScaleWithDocument == value)
					return;
				scaleWithDocument = value;
				OnPropertyChanged("ScaleWithDocument");
			}
		}
		public bool AlignWithMargins {
			get { return alignWithMargins; }
			set {
				if (AlignWithMargins == value)
					return;
				alignWithMargins = value;
				OnPropertyChanged("AlignWithMargins");
			}
		}
		public string OddHeader {
			get { return oddHeader; }
			set {
				if (OddHeader == value)
					return;
				oddHeader = value;
			}
		}
		public string OddFooter {
			get { return oddFooter; }
			set {
				if (OddFooter == value)
					return;
				oddFooter = value;
			}
		}
		public string EvenHeader {
			get { return evenHeader; }
			set {
				if (EvenHeader == value)
					return;
				evenHeader = value;
			}
		}
		public string EvenFooter {
			get { return evenFooter; }
			set {
				if (EvenFooter == value)
					return;
				evenFooter = value;
			}
		}
		public string FirstHeader {
			get { return firstHeader; }
			set {
				if (FirstHeader == value)
					return;
				firstHeader = value;
			}
		}
		public string FirstFooter {
			get { return firstFooter; }
			set {
				if (FirstFooter == value)
					return;
				firstFooter = value;
			}
		}
		public string OddLeftHeader {
			get { return oddHeaderBuilder.Left; }
			set {
				if (OddLeftHeader == value)
					return;
				oddHeaderBuilder.Left = value;
				OnPropertyChanged("OddLeftHeader");
			}
		}
		public string OddCenterHeader {
			get { return oddHeaderBuilder.Center; }
			set {
				if (OddCenterHeader == value)
					return;
				oddHeaderBuilder.Center = value;
				OnPropertyChanged("OddCenterHeader");
			}
		}
		public string OddRightHeader {
			get { return oddHeaderBuilder.Right; }
			set {
				if (OddRightHeader == value)
					return;
				oddHeaderBuilder.Right = value;
				OnPropertyChanged("OddRightHeader");
			}
		}
		public string OddLeftFooter {
			get { return oddFooterBuilder.Left; }
			set {
				if (OddLeftFooter == value)
					return;
				oddFooterBuilder.Left = value;
				OnPropertyChanged("OddLeftFooter");
			}
		}
		public string OddCenterFooter {
			get { return oddFooterBuilder.Center; }
			set {
				if (OddCenterFooter == value)
					return;
				oddFooterBuilder.Center = value;
				OnPropertyChanged("OddCenterFooter");
			}
		}
		public string OddRightFooter {
			get { return oddFooterBuilder.Right; }
			set {
				if (OddRightFooter == value)
					return;
				oddFooterBuilder.Right = value;
				OnPropertyChanged("OddRightFooter");
			}
		}
		public string EvenLeftHeader {
			get { return evenHeaderBuilder.Left; }
			set {
				if (EvenLeftHeader == value)
					return;
				evenHeaderBuilder.Left = value;
				OnPropertyChanged("EvenLeftHeader");
			}
		}
		public string EvenCenterHeader {
			get { return evenHeaderBuilder.Center; }
			set {
				if (EvenCenterHeader == value)
					return;
				evenHeaderBuilder.Center = value;
				OnPropertyChanged("EvenCenterHeader");
			}
		}
		public string EvenRightHeader {
			get { return evenHeaderBuilder.Right; }
			set {
				if (EvenRightHeader == value)
					return;
				evenHeaderBuilder.Right = value;
				OnPropertyChanged("EvenRightHeader");
			}
		}
		public string EvenLeftFooter {
			get { return evenFooterBuilder.Left; }
			set {
				if (EvenLeftFooter == value)
					return;
				evenFooterBuilder.Left = value;
				OnPropertyChanged("EvenLeftFooter");
			}
		}
		public string EvenCenterFooter {
			get { return evenFooterBuilder.Center; }
			set {
				if (EvenCenterFooter == value)
					return;
				evenFooterBuilder.Center = value;
				OnPropertyChanged("EvenCenterFooter");
			}
		}
		public string EvenRightFooter {
			get { return evenFooterBuilder.Right; }
			set {
				if (EvenRightFooter == value)
					return;
				evenFooterBuilder.Right = value;
				OnPropertyChanged("EvenRightFooter");
			}
		}
		public string FirstLeftHeader {
			get { return firstHeaderBuilder.Left; }
			set {
				if (FirstLeftHeader == value)
					return;
				firstHeaderBuilder.Left = value;
				OnPropertyChanged("FirstLeftHeader");
			}
		}
		public string FirstCenterHeader {
			get { return firstHeaderBuilder.Center; }
			set {
				if (FirstCenterHeader == value)
					return;
				firstHeaderBuilder.Center = value;
				OnPropertyChanged("FirstCenterHeader");
			}
		}
		public string FirstRightHeader {
			get { return firstHeaderBuilder.Right; }
			set {
				if (FirstRightHeader == value)
					return;
				firstHeaderBuilder.Right = value;
				OnPropertyChanged("FirstRightHeader");
			}
		}
		public string FirstLeftFooter {
			get { return firstFooterBuilder.Left; }
			set {
				if (FirstLeftFooter == value)
					return;
				firstFooterBuilder.Left = value;
				OnPropertyChanged("FirstLeftFooter");
			}
		}
		public string FirstCenterFooter {
			get { return firstFooterBuilder.Center; }
			set {
				if (FirstCenterFooter == value)
					return;
				firstFooterBuilder.Center = value;
				OnPropertyChanged("FirstCenterFooter");
			}
		}
		public string FirstRightFooter {
			get { return firstFooterBuilder.Right; }
			set {
				if (FirstRightFooter == value)
					return;
				firstFooterBuilder.Right = value;
				OnPropertyChanged("FirstRightFooter");
			}
		}
		public bool IsUpdateAllowed {
			get { return isUpdateAllowed; }
			set {
				if (IsUpdateAllowed == value)
					return;
				isUpdateAllowed = value;
			}
		}
		#endregion
		public bool ValidateHeaderFooter() {
			if (oddHeaderBuilder.ToString().Length > 255 || oddFooterBuilder.ToString().Length > 255 || evenHeaderBuilder.ToString().Length > 255 ||
				evenFooterBuilder.ToString().Length > 255 || firstHeaderBuilder.ToString().Length > 255 || firstFooterBuilder.ToString().Length > 255) {
				ModelErrorInfo errorInfo = new ModelErrorInfo(ModelErrorType.HeaderFooterTooLongTextString);
				control.InnerControl.ErrorHandler.HandleError(errorInfo);
				return false;
			}
			return true;
		}
		public void ApplyChanges() {
			OddHeader = FormatTagHFConverter.ConvertToOriginalTag(oddHeaderBuilder.ToString());
			OddFooter = FormatTagHFConverter.ConvertToOriginalTag(oddFooterBuilder.ToString());
			EvenHeader = FormatTagHFConverter.ConvertToOriginalTag(evenHeaderBuilder.ToString());
			EvenFooter = FormatTagHFConverter.ConvertToOriginalTag(evenFooterBuilder.ToString());
			FirstHeader = FormatTagHFConverter.ConvertToOriginalTag(firstHeaderBuilder.ToString());
			FirstFooter = FormatTagHFConverter.ConvertToOriginalTag(firstFooterBuilder.ToString());
		}
	}
	#endregion
}

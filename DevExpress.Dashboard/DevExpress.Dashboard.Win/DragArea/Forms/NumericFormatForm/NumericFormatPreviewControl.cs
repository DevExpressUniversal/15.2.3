#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public partial class NumericFormatPreviewControl : MemoEdit {
		decimal previewValue;
		protected override Size DefaultSize { get { return new Size(336, 60); } }
		public NumericFormatPreviewControl() {
			InitializeComponent();
			Size = DefaultSize;
			Font = new Font(Font.FontFamily, 16);
			TabStop = false;
			Properties.ReadOnly = true;
			Properties.ScrollBars = ScrollBars.None;
			Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
			Properties.Appearance.TextOptions.VAlignment = VertAlignment.Center;
		}
		public void Initialize(decimal previewValue) {
			this.previewValue = previewValue;
		}
		public void UpdatePreview(string currencyCultureName) {
			NumericFormatViewModel viewModel = new NumericFormatViewModel(NumericFormatType.Currency, 0, DataItemNumericUnit.Ones, false, false, 3, currencyCultureName);
			ShowPreview(viewModel);
		}
		public void UpdatePreview(DataItem dataItem, DataItemNumericFormat format) {
			NumericFormatViewModel viewModel = dataItem.CreateNumericFormatViewModel(format);
			ShowPreview(viewModel);
		}
		void ShowPreview(NumericFormatViewModel viewModel) {
			NumericFormatter formatter = NumericFormatter.CreateInstance(Thread.CurrentThread.CurrentCulture, viewModel);
			Text = (viewModel.FormatType == NumericFormatType.Currency) ?
				String.Join(Environment.NewLine, new string[] { formatter.Format(previewValue), formatter.Format(-previewValue) }) :
				formatter.Format(previewValue);
		}
	}
}

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

using DevExpress.Data.XtraReports.Wizard;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using DevExpress.Xpf.Printing.PreviewControl.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages.Common {
	public class ReportStyleViewModel {
		public FontOptionsViewModel Title { get; private set; }
		public FontOptionsViewModel Caption { get; private set; }
		public FontOptionsViewModel Data { get; private set; }
		public ReportStyleId StyleId { get; private set; }
		public string DisplayName { get { return StyleId.ToString(); } }
		public ReportStyleViewModel(ReportStyleId style) {
			StyleId = style;
			CreateStyles();
		}
		void CreateStyles() {
			switch (StyleId) {
				case ReportStyleId.Bold:
					Title = new FontOptionsViewModel("Times New Roman", Color.FromArgb(255, 128, 0, 0), FromwWinFormsSize(20d), FontStyles.Normal, FontWeights.Bold);
					Caption = new FontOptionsViewModel("Arial", Color.FromArgb(255, 128, 0, 0), FromwWinFormsSize(10d), FontStyles.Normal, FontWeights.Normal);
					Data = new FontOptionsViewModel("Times New Roman", Colors.Black, FromwWinFormsSize(10d), FontStyles.Normal, FontWeights.Normal);
					break;
				case ReportStyleId.Casual:
					Title = new FontOptionsViewModel("Tahoma", Color.FromArgb(255, 0, 128, 128), FromwWinFormsSize(24d), FontStyles.Normal, FontWeights.Normal);
					Caption = new FontOptionsViewModel("Arial", Colors.Black, FromwWinFormsSize(10d), FontStyles.Normal, FontWeights.Normal);
					Data = new FontOptionsViewModel("Arial", Colors.Black, FromwWinFormsSize(10d), FontStyles.Normal, FontWeights.Normal);
					break;
				case ReportStyleId.Compact:
					Title = new FontOptionsViewModel("Times New Roman", Colors.Black, FromwWinFormsSize(21d), FontStyles.Normal, FontWeights.Normal);
					Caption = new FontOptionsViewModel("Times New Roman", Colors.Black, FromwWinFormsSize(10d), FontStyles.Normal, FontWeights.Normal);
					Data = new FontOptionsViewModel("Arial", Colors.Black, FromwWinFormsSize(9d), FontStyles.Normal, FontWeights.Normal);
					break;
				case ReportStyleId.Corporate:
					Title = new FontOptionsViewModel("Times New Roman", Color.FromArgb(255, 0, 0, 128), FromwWinFormsSize(20d), FontStyles.Italic, FontWeights.Bold);
					Caption = new FontOptionsViewModel("Times New Roman", Color.FromArgb(255, 0, 0, 128), FromwWinFormsSize(11d), FontStyles.Italic, FontWeights.Bold);
					Data = new FontOptionsViewModel("Arial", Colors.Black, FromwWinFormsSize(8d), FontStyles.Normal, FontWeights.Normal);
					break;
				case ReportStyleId.Formal:
					Title = new FontOptionsViewModel("Times New Roman", Colors.Black, FromwWinFormsSize(24d), FontStyles.Normal, FontWeights.Bold);
					Caption = new FontOptionsViewModel("Times New Roman", Colors.Black, FromwWinFormsSize(10d), FontStyles.Normal, FontWeights.Bold);
					Data = new FontOptionsViewModel("Times New Roman", Colors.Black, FromwWinFormsSize(18d), FontStyles.Normal, FontWeights.Normal);
					break;
				default:
					throw new InvalidOperationException();
			}
		}
		static double FromwWinFormsSize(double winFormsSize) {
			return winFormsSize * 96 / 72;
		}
	}
	public class FontOptionsViewModel {
		public FontWeight FontWeight { get; private set; }
		public FontStyle FontStyle { get; private set; }
		public FontFamily FontFamily { get; private set; }
		public double FontSize { get; private set; }
		public Color FontColor { get; private set; }
		public FontOptionsViewModel(string fontName, Color color, double fontSize, FontStyle style, FontWeight weight) {
			FontFamily = new FontFamily(fontName);
			FontColor = color;
			FontSize = fontSize;
			FontStyle = style;
			FontWeight = weight;
		}
	}
}

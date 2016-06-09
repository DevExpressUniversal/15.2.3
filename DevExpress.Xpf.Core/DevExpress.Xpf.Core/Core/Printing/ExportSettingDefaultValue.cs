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
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Printing {
	public static class ExportSettingDefaultValue {
		public static readonly TargetType TargetType = TargetType.None;
		public static readonly Color Background = Colors.Transparent;
		public static readonly Color Foreground = Colors.Black;
		public static readonly Color BorderColor = Colors.Black;
		public static readonly Thickness BorderThickness = new Thickness(0);
		public static readonly string Url = string.Empty;
		public static readonly IOnPageUpdater OnPageUpdater = null;
		public static readonly object MergeValue = null;
		public static readonly bool? BooleanValue = null;
		public static readonly string CheckText = null;
		public static readonly ImageSource ImageSource = null;
		public static readonly string PageNumberFormat = string.Empty;
		public static readonly PageNumberKind PageNumberKind = PageNumberKind.NumberOfTotal;
		public static readonly HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Left;
		public static readonly VerticalAlignment VerticalAlignment = VerticalAlignment.Top;
		public static readonly string Text = null;
		public static readonly object TextValue = null;
		public static readonly string TextValueFormatString = string.Empty;
		public static readonly FontFamily FontFamily = new FontFamily("Arial");
		public static readonly FontStyle FontStyle = FontStyles.Normal;
		public static readonly FontWeight FontWeight = FontWeights.Normal;
		public static readonly double FontSize = 10d;
		public static readonly Thickness Padding = new Thickness(0);
		public static readonly TextWrapping TextWrapping = TextWrapping.Wrap;
		public static readonly bool NoTextExport = false;
		public static readonly bool? XlsExportNativeFormat = null;
		public static readonly string XlsxFormatString = null;
		public static readonly ImageRenderMode ImageRenderMode = ImageRenderMode.MakeScreenshot;
		public static readonly object ImageKey = null;
		public static readonly BorderDashStyle BorderDashStyle = BorderDashStyle.Solid;
		public static readonly TextDecorationCollection TextDecorations = new TextDecorationCollection();
		public static readonly TextTrimming TextTrimming = System.Windows.TextTrimming.None;
		public static readonly int ProgressBarPosition = 0;
		public static readonly int TrackBarPosition = 0;
		public static readonly int TrackBarMinimum = 0;
		public static readonly int TrackBarMaximum = 0;
	}
}

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

using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class HeaderFooterPreviewControl : Control {
		public static readonly DependencyProperty HeaderFooterValueProperty;
		public static readonly DependencyProperty ProviderProperty;
		public static readonly DependencyProperty HeaderFooterLeftValueProperty;
		public static readonly DependencyProperty HeaderFooterCenterValueProperty;
		public static readonly DependencyProperty HeaderFooterRightValueProperty;
		static HeaderFooterPreviewControl() {
			Type ownerType = typeof(HeaderFooterPreviewControl);
			HeaderFooterValueProperty = DependencyProperty.Register("HeaderFooterValue", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			ProviderProperty = DependencyProperty.Register("Provider", typeof(HeaderFooterFormatTagProvider), ownerType,
				new FrameworkPropertyMetadata(default(HeaderFooterFormatTagProvider), FrameworkPropertyMetadataOptions.AffectsArrange));
			HeaderFooterLeftValueProperty = DependencyProperty.Register("HeaderFooterLeftValue", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			HeaderFooterCenterValueProperty = DependencyProperty.Register("HeaderFooterCenterValue", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			HeaderFooterRightValueProperty = DependencyProperty.Register("HeaderFooterRightValue", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
		}
		public HeaderFooterPreviewControl() {
			DefaultStyleKey = typeof(HeaderFooterPreviewControl);
		}
		public string HeaderFooterValue {
			get { return (string)GetValue(HeaderFooterValueProperty); }
			set { SetValue(HeaderFooterValueProperty, value); }
		}
		public HeaderFooterFormatTagProvider Provider {
			get { return (HeaderFooterFormatTagProvider)GetValue(ProviderProperty); }
			set { SetValue(ProviderProperty, value); }
		}
		public string HeaderFooterLeftValue {
			get { return (string)GetValue(HeaderFooterLeftValueProperty); }
			set { SetValue(HeaderFooterLeftValueProperty, value); }
		}
		public string HeaderFooterCenterValue {
			get { return (string)GetValue(HeaderFooterCenterValueProperty); }
			set { SetValue(HeaderFooterCenterValueProperty, value); }
		}
		public string HeaderFooterRightValue {
			get { return (string)GetValue(HeaderFooterRightValueProperty); }
			set { SetValue(HeaderFooterRightValueProperty, value); }
		}
		public void ShowPreview() {
			if (HeaderFooterValue == null || Provider == null)
				return;
			HeaderFooterBuilder builder = new HeaderFooterBuilder(HeaderFooterValue, true, Provider);
			HeaderFooterLeftValue = builder.Left;
			HeaderFooterCenterValue = builder.Center;
			HeaderFooterRightValue = builder.Right;
		}
	}
}

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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Layout;
using System;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class HeaderFooterEditControl : Control {
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty FooterProperty;
		public static readonly DependencyProperty ProviderProperty;
		public static readonly DependencyProperty LeftHeaderProperty;
		public static readonly DependencyProperty CenterHeaderProperty;
		public static readonly DependencyProperty RightHeaderProperty;
		public static readonly DependencyProperty LeftFooterProperty;
		public static readonly DependencyProperty CenterFooterProperty;
		public static readonly DependencyProperty RightFooterProperty;
		static HeaderFooterEditControl() {
			Type ownerType = typeof(HeaderFooterEditControl);
			HeaderProperty = DependencyProperty.Register("Header", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			FooterProperty = DependencyProperty.Register("Footer", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			ProviderProperty = DependencyProperty.Register("Provider", typeof(HeaderFooterFormatTagProvider), ownerType,
				new FrameworkPropertyMetadata(default(HeaderFooterFormatTagProvider), FrameworkPropertyMetadataOptions.AffectsArrange));
			LeftHeaderProperty = DependencyProperty.Register("LeftHeader", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			CenterHeaderProperty = DependencyProperty.Register("CenterHeader", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			RightHeaderProperty = DependencyProperty.Register("RightHeader", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			LeftFooterProperty = DependencyProperty.Register("LeftFooter", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			CenterFooterProperty = DependencyProperty.Register("CenterFooter", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
			RightFooterProperty = DependencyProperty.Register("RightFooter", typeof(string), ownerType,
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.AffectsArrange));
		}
		#region Fields
		object uiEditControl;
		TextEdit edtLeftHeader;
		TextEdit edtCenterHeader;
		TextEdit edtRightHeader;
		TextEdit edtLeftFooter;
		TextEdit edtCenterFooter;
		TextEdit edtRightFooter;
		Button btnPage;
		Button btnPages;
		Button btnDate;
		Button btnTime;
		Button btnFileName;
		Button btnFilePath;
		Button btnSheetName;
		#endregion
		public HeaderFooterEditControl() {
			DefaultStyleKey = typeof(HeaderFooterEditControl);
		}
		public HeaderFooterFormatTagProvider Provider {
			get { return (HeaderFooterFormatTagProvider)GetValue(ProviderProperty); }
			set { SetValue(ProviderProperty, value); }
		}
		public string LeftHeader {
			get { return (string)GetValue(LeftHeaderProperty); }
			set { SetValue(LeftHeaderProperty, value); }
		}
		public string CenterHeader {
			get { return (string)GetValue(CenterHeaderProperty); }
			set { SetValue(CenterHeaderProperty, value); }
		}
		public string RightHeader {
			get { return (string)GetValue(RightHeaderProperty); }
			set { SetValue(RightHeaderProperty, value); }
		}
		public string LeftFooter {
			get { return (string)GetValue(LeftFooterProperty); }
			set { SetValue(LeftFooterProperty, value); }
		}
		public string CenterFooter {
			get { return (string)GetValue(CenterFooterProperty); }
			set { SetValue(CenterFooterProperty, value); }
		}
		public string RightFooter {
			get { return (string)GetValue(RightFooterProperty); }
			set { SetValue(RightFooterProperty, value); }
		}
		public override void OnApplyTemplate() {
			UnsubscribeEvents();
			base.OnApplyTemplate();
			edtLeftHeader = LayoutHelper.FindElementByName(this, "edtLeftHeader") as TextEdit;
			edtCenterHeader = LayoutHelper.FindElementByName(this, "edtCenterHeader") as TextEdit;
			edtRightHeader = LayoutHelper.FindElementByName(this, "edtRightHeader") as TextEdit;
			edtLeftFooter = LayoutHelper.FindElementByName(this, "edtLeftFooter") as TextEdit;
			edtCenterFooter = LayoutHelper.FindElementByName(this, "edtCenterFooter") as TextEdit;
			edtRightFooter = LayoutHelper.FindElementByName(this, "edtRightFooter") as TextEdit;
			btnPage = LayoutHelper.FindElementByName(this, "btnPage") as Button;
			btnPages = LayoutHelper.FindElementByName(this, "btnPages") as Button;
			btnDate = LayoutHelper.FindElementByName(this, "btnDate") as Button;
			btnTime = LayoutHelper.FindElementByName(this, "btnTime") as Button;
			btnFileName = LayoutHelper.FindElementByName(this, "btnFileName") as Button;
			btnFilePath = LayoutHelper.FindElementByName(this, "btnFilePath") as Button;
			btnSheetName = LayoutHelper.FindElementByName(this, "btnSheetName") as Button;
			SubscribeEvents();
		}
		void UnsubscribeEvents() {
			if (edtLeftHeader != null)
				edtLeftHeader.LostFocus -= edtLeftHeaderLostFocus;
			if (edtCenterHeader != null)
				edtCenterHeader.LostFocus -= edtCenterHeaderLostFocus;
			if (edtRightHeader != null)
				edtRightHeader.LostFocus -= edtRightHeaderLostFocus;
			if (edtLeftFooter != null)
				edtLeftFooter.LostFocus -= edtLeftFooterLostFocus;
			if (edtCenterFooter != null)
				edtCenterFooter.LostFocus -= edtCenterFooterLostFocus;
			if (edtRightFooter != null)
				edtRightFooter.LostFocus -= edtRightFooterLostFocus;
			if (btnPage != null)
				btnPage.Click -= OnPageClick;
			if (btnPages != null)
				btnPages.Click -= OnPagesClick;
			if (btnDate != null)
				btnDate.Click -= OnDateClick;
			if (btnTime != null)
				btnTime.Click -= OnTimeClick;
			if (btnFileName != null)
				btnFileName.Click -= OnFileNameClick;
			if (btnFilePath != null)
				btnFilePath.Click -= OnFilePathClick;
			if (btnSheetName != null)
				btnSheetName.Click -= OnSheetNameClick;
		}
		void SubscribeEvents() {
			if (edtLeftHeader != null)
				edtLeftHeader.LostFocus += edtLeftHeaderLostFocus;
			if (edtCenterHeader != null)
				edtCenterHeader.LostFocus += edtCenterHeaderLostFocus;
			if (edtRightHeader != null)
				edtRightHeader.LostFocus += edtRightHeaderLostFocus;
			if (edtLeftFooter != null)
				edtLeftFooter.LostFocus += edtLeftFooterLostFocus;
			if (edtCenterFooter != null)
				edtCenterFooter.LostFocus += edtCenterFooterLostFocus;
			if (edtRightFooter != null)
				edtRightFooter.LostFocus += edtRightFooterLostFocus;
			if (btnPage != null)
				btnPage.Click += OnPageClick;
			if (btnPages != null)
				btnPages.Click += OnPagesClick;
			if (btnDate != null)
				btnDate.Click += OnDateClick;
			if (btnTime != null)
				btnTime.Click += OnTimeClick;
			if (btnFileName != null)
				btnFileName.Click += OnFileNameClick;
			if (btnFilePath != null)
				btnFilePath.Click += OnFilePathClick;
			if (btnSheetName != null)
				btnSheetName.Click += OnSheetNameClick;
		}
		void GetUIEditControl(object control) {
			uiEditControl = control;
		}
		void edtLeftHeaderLostFocus(object sender, RoutedEventArgs e) {
			GetUIEditControl(sender);
		}
		void edtCenterHeaderLostFocus(object sender, RoutedEventArgs e) {
			GetUIEditControl(sender);
		}
		void edtRightHeaderLostFocus(object sender, RoutedEventArgs e) {
			GetUIEditControl(sender);
		}
		void edtLeftFooterLostFocus(object sender, RoutedEventArgs e) {
			GetUIEditControl(sender);
		}
		void edtCenterFooterLostFocus(object sender, RoutedEventArgs e) {
			GetUIEditControl(sender);
		}
		void edtRightFooterLostFocus(object sender, RoutedEventArgs e) {
			GetUIEditControl(sender);
		}
		void OnPageClick(object sender, RoutedEventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as TextEdit).Text += FormatTagHFConverter.PageNumberAnalog;
		}
		void OnPagesClick(object sender, RoutedEventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as TextEdit).Text += FormatTagHFConverter.PageTotalAnalog;
		}
		void OnDateClick(object sender, RoutedEventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as TextEdit).Text += FormatTagHFConverter.DateAnalog;
		}
		void OnTimeClick(object sender, RoutedEventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as TextEdit).Text += FormatTagHFConverter.TimeAnalog;
		}
		void OnFileNameClick(object sender, RoutedEventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as TextEdit).Text += FormatTagHFConverter.WorkbookFileNameAnalog;
		}
		void OnFilePathClick(object sender, RoutedEventArgs e) {
			if (uiEditControl != null) {
				(uiEditControl as TextEdit).Text += FormatTagHFConverter.WorkbookFilePathAnalog;
				(uiEditControl as TextEdit).Text += FormatTagHFConverter.WorkbookFileNameAnalog;
			}
		}
		void OnSheetNameClick(object sender, RoutedEventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as TextEdit).Text += FormatTagHFConverter.WorksheetNameAnalog;
		}
	}
}

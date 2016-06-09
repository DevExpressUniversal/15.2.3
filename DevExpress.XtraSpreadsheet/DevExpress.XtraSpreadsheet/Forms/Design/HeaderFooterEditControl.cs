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

using DevExpress.XtraEditors;
using System.ComponentModel;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public partial class HeaderFooterEditControl : XtraUserControl, INotifyPropertyChanged {
		#region Fields
		object uiEditControl;
		#endregion
		public HeaderFooterEditControl() {
			InitializeComponent();
			SubscribeEvents();
		}
		#region Properties
		public string LeftHeader {
			get { return edtLeftHeader.Text; }
			set {
				if (LeftHeader == value)
					return;
				edtLeftHeader.Text = value;
				OnPropertyChanged("LeftHeader");
			}
		}
		public string CenterHeader {
			get { return edtCenterHeader.Text; }
			set {
				if (CenterHeader == value)
					return;
				edtCenterHeader.Text = value;
				OnPropertyChanged("CenterHeader");
			}
		}
		public string RightHeader {
			get { return edtRightHeader.Text; }
			set {
				if (RightHeader == value)
					return;
				edtRightHeader.Text = value;
				OnPropertyChanged("RightHeader");
			}
		}
		public string LeftFooter {
			get { return edtLeftFooter.Text; }
			set {
				if (LeftFooter == value)
					return;
				edtLeftFooter.Text = value;
				OnPropertyChanged("LeftFooter");
			}
		}
		public string CenterFooter {
			get { return edtCenterFooter.Text; }
			set {
				if (CenterFooter == value)
					return;
				edtCenterFooter.Text = value;
				OnPropertyChanged("CenterFooter");
			}
		}
		public string RightFooter {
			get { return edtRightFooter.Text; }
			set {
				if (RightFooter == value)
					return;
				edtRightFooter.Text = value;
				OnPropertyChanged("RightFooter");
			}
		}
		#endregion
		#region Events
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected void OnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		void SubscribeEvents() {
			this.edtLeftHeader.TextChanged += OnLeftHeaderValueChanged;
			this.edtCenterHeader.TextChanged += OnCenterHeaderValueChanged;
			this.edtRightHeader.TextChanged += OnRightHeaderValueChanged;
			this.edtLeftFooter.TextChanged += OnLeftFooterValueChanged;
			this.edtCenterFooter.TextChanged += OnCenterFooterValueChanged;
			this.edtRightFooter.TextChanged += OnRightFooterValueChanged;
			this.btnDate.Click += OnDateClick;
			this.btnTime.Click += OnTimeClick;
			this.btnFileName.Click += OnFileNameClick;
			this.btnFilePath.Click += OnFilePathClick;
			this.btnSheetName.Click += OnSheetNameClick;
			this.edtLeftHeader.LostFocus += OnLeftHeaderLostFocus;
			this.edtCenterHeader.LostFocus += OnCenterHeaderLostFocus;
			this.edtRightHeader.LostFocus += OnRightHeaderLostFocus;
			this.edtLeftFooter.LostFocus += OnLeftFooterLostFocus;
			this.edtCenterFooter.LostFocus += OnCenterFooterLostFocus;
			this.edtRightFooter.LostFocus += OnRightFooterLostFocus;
		}
		void GetUIEditControl(object control) {
			uiEditControl = control;
		}
		void OnLeftHeaderValueChanged(object sender, System.EventArgs e) {
			OnPropertyChanged("LeftHeader");
		}
		void OnCenterHeaderValueChanged(object sender, System.EventArgs e) {
			OnPropertyChanged("CenterHeader");
		}
		void OnRightHeaderValueChanged(object sender, System.EventArgs e) {
			OnPropertyChanged("RightHeader");
		}
		void OnLeftFooterValueChanged(object sender, System.EventArgs e) {
			OnPropertyChanged("LeftFooter");
		}
		void OnCenterFooterValueChanged(object sender, System.EventArgs e) {
			OnPropertyChanged("CenterFooter");
		}
		void OnRightFooterValueChanged(object sender, System.EventArgs e) {
			OnPropertyChanged("RightFooter");
		}
		void OnPageClick(object sender, System.EventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as MemoEdit).Text += FormatTagHFConverter.PageNumberAnalog;
		}
		void OnPagesClick(object sender, System.EventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as MemoEdit).Text += FormatTagHFConverter.PageTotalAnalog;
		}
		void OnDateClick(object sender, System.EventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as MemoEdit).Text += FormatTagHFConverter.DateAnalog;
		}
		void OnTimeClick(object sender, System.EventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as MemoEdit).Text += FormatTagHFConverter.TimeAnalog;
		}
		void OnFileNameClick(object sender, System.EventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as MemoEdit).Text += FormatTagHFConverter.WorkbookFileNameAnalog;
		}
		void OnFilePathClick(object sender, System.EventArgs e) {
			if (uiEditControl != null) {
				(uiEditControl as MemoEdit).Text += FormatTagHFConverter.WorkbookFilePathAnalog;
				(uiEditControl as MemoEdit).Text += FormatTagHFConverter.WorkbookFileNameAnalog;
			}
		}
		void OnSheetNameClick(object sender, System.EventArgs e) {
			if (uiEditControl != null)
				(uiEditControl as MemoEdit).Text += FormatTagHFConverter.WorksheetNameAnalog;
		}
		void OnCenterHeaderLostFocus(object sender, System.EventArgs e) {
			GetUIEditControl(sender);
		}
		void OnLeftHeaderLostFocus(object sender, System.EventArgs e) {
			GetUIEditControl(sender);
		}
		void OnRightHeaderLostFocus(object sender, System.EventArgs e) {
			GetUIEditControl(sender);
		}
		void OnCenterFooterLostFocus(object sender, System.EventArgs e) {
			GetUIEditControl(sender);
		}
		void OnLeftFooterLostFocus(object sender, System.EventArgs e) {
			GetUIEditControl(sender);
		}
		void OnRightFooterLostFocus(object sender, System.EventArgs e) {
			GetUIEditControl(sender);
		}
	}
}

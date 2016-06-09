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
using System.Windows;
using System.Windows.Controls;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.Editors;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class HyperlinkFormControl : UserControl, IDialogContent {
		readonly HyperlinkFormController controller;
		public HyperlinkFormControl() {
			InitializeComponent();
			SubscribeControlsEvents();
		}
		public HyperlinkFormControl(HyperlinkFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			InitializeComponent();
			Loaded += OnLoaded;
			this.controller = CreateController(controllerParameters);
			PopulateTargetFrame();
			PopulateBookmarks();
			if (String.IsNullOrEmpty(Controller.NavigateUri) && !String.IsNullOrEmpty(Controller.Anchor)) {
				SwitchToBookmark();
				UseNavigateUri = false;
			}
			SubscribeControlsEvents();
			UpdateForm();
		}
		#region Properties
		public string TextToDisplay { get { return textToDisplay.EditValue as String; } set { textToDisplay.EditValue = value; } }
		public string ScreenTip { get { return screenTip.EditValue as String; } set { screenTip.EditValue = value; } }
		public ComboBoxEdit TargetFrame { get { return targetFrame; } }
		public string Address { get { return address.EditValue as String; } set { address.EditValue = value; } }
		public ComboBoxEdit Bookmark { get { return bookmark; } }
		public bool UseNavigateUri {
			get { return linkToAddress.IsChecked.HasValue && linkToAddress.IsChecked.Value; }
			set {
				linkToAddress.IsChecked = value;
				linkToBookmark.IsChecked = !value;
			}
		}
		public HyperlinkFormController Controller { get { return controller; } }
		IRichEditControl Control { get { return Controller != null ? Controller.Control : null; } }
		#endregion
		protected virtual HyperlinkFormController CreateController(HyperlinkFormControllerParameters controllerParameters) {
			return new HyperlinkFormController(controllerParameters);
		}
		protected void PopulateTargetFrame() {
			ListItemCollection items = TargetFrame.Items;
			items.Add(new TargetFrameItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.TargetFrameDescription_Blank), "_blank"));
			items.Add(new TargetFrameItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.TargetFrameDescription_Parent), "_parent"));
			items.Add(new TargetFrameItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.TargetFrameDescription_Self), "_self"));
			items.Add(new TargetFrameItem(XtraRichEditLocalizer.GetString(XtraRichEditStringId.TargetFrameDescription_Top), "_top"));
		}
		protected void PopulateBookmarks() {
			if (Control == null)
				return;
			Bookmark.Items.Add(XtraRichEditLocalizer.GetString(XtraRichEditStringId.HyperlinkForm_SelectedBookmarkNone));
			List<Bookmark> bookmarks = Control.InnerControl.DocumentModel.GetBookmarks();
			bookmarks.Sort(new BookmarkNameComparer());
			foreach (Bookmark bookmark in bookmarks)
				Bookmark.Items.Add(bookmark.Name);
			Bookmark.SelectedIndex = 0;
		}
		public void SubscribeControlsEvents() {
			textToDisplay.EditValueChanged += ContentChanged;
			screenTip.EditValueChanged += ContentChanged;
			targetFrame.SelectedIndexChanged += ContentChanged;
			address.EditValueChanged += ContentChanged;
			bookmark.SelectedIndexChanged += ContentChanged;
			linkToAddress.Checked += ContentChanged;
			linkToBookmark.Checked += ContentChanged;
		}
		public void UnsubscribeControlsEvents() {
			textToDisplay.EditValueChanged -= ContentChanged;
			screenTip.EditValueChanged -= ContentChanged;
			targetFrame.SelectedIndexChanged -= ContentChanged;
			address.EditValueChanged -= ContentChanged;
			bookmark.SelectedIndexChanged -= ContentChanged;
			linkToAddress.Checked -= ContentChanged;
			linkToBookmark.Checked -= ContentChanged;
		}
		void ContentChanged(object sender, EventArgs e) {
			AssignValuesToController();
		}
		public void SwitchToAddress() {
			VisualStateManager.GoToState(this, "AddressSelect", true);
		}
		public void SwitchToBookmark() {
			VisualStateManager.GoToState(this, "BookmarkSelect", true);
		}
		protected virtual void OnLinkAddressChecked(object sender, RoutedEventArgs e) {
			SwitchToAddress();
		}
		protected virtual void OnLinkBookmarkChecked(object sender, RoutedEventArgs e) {
			SwitchToBookmark();
		}
		protected virtual void OnAddressButtonClick(object sender, RoutedEventArgs e) {
#if SL
			OpenFileDialog openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
				Address = openFileDialog.File.Name;
#else
			System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				Address = openFileDialog.FileName;
#endif
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateForm();
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
			UpdateOKButton();
		}
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
			TextToDisplay = Controller.TextToDisplay;
			ScreenTip = Controller.HyperlinkInfo.ToolTip;
			Address = Controller.NavigateUri;
			if (!String.IsNullOrEmpty(Controller.Target)) {
				foreach (TargetFrameItem item in TargetFrame.Items) {
					if (String.Compare(item.Value, Controller.Target) == 0) {
						TargetFrame.SelectedItem = item;
						break;
					}
				}
			}
			int index = Bookmark.Items.IndexOf(Controller.Anchor);
			if (index >= 0)
				Bookmark.SelectedIndex = index;
			Dispatcher.BeginInvoke(new Action(() => UpdateLayout()));
		}
		protected virtual void ApplyChanges() {
			Controller.ApplyChanges(UseNavigateUri);
		}
		protected virtual void AssignValuesToController() {
			if (Controller == null)
				return;
			Controller.TextToDisplay = TextToDisplay;
			Controller.ToolTip = ScreenTip;
			TargetFrameItem selectedItem = TargetFrame.SelectedItem as TargetFrameItem;
			Controller.Target = selectedItem != null ? selectedItem.Value : string.Empty;
			Controller.NavigateUri = Address;
			Controller.Anchor = Bookmark.SelectedIndex > 0 ? (string)Bookmark.SelectedItem : String.Empty;
			UpdateOKButton();
		}
		void UpdateOKButton() {
			if (Controller == null)
				return;
#if SL
			bool enabled = UseNavigateUri ? !String.IsNullOrEmpty(Controller.NavigateUri) : !String.IsNullOrEmpty(Controller.Anchor);
			DXDialog dialog = FloatingContainer.GetDialogOwner(this) as DXDialog;
			if (dialog != null)
				dialog.OkButton.IsEnabled = enabled;
#endif
		}
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
	}
	public class TargetFrameItem {
		readonly string displayText;
		readonly string value;
		public TargetFrameItem(string displayText, string value) {
			this.displayText = displayText;
			this.value = value;
		}
		public string DisplayText { get { return displayText; } }
		public string Value { get { return value; } }
	}
}

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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Office.Internal;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.RichEdit.UI;
namespace DevExpress.XtraRichEdit.Forms {
	#region BorderShadingFormControl
	public partial class BorderShadingFormControl: UserControl, IDialogContent {
		BorderShadingFormController controller;
		public BorderShadingFormControl(BorderShadingFormController controller) {
			Guard.ArgumentNotNull(controller, "controller");
			this.controller = controller;
			InitializeComponent();
			UnsubscribeControlsEvents();
			Loaded += OnLoaded;
			InitializeForm();
			SubscribeControlsEvents();
			InitializePreviewControl(borderUserControl);
			borderUserControl.BtnsVisible = true;
			colorEdit.Color = XpfTypeConverter.ToPlatformColor(controller.GetActiveColor());
			InitializePreviewControl(shadingUserControl);
			shadingUserControl.BtnsVisible = false;
			borderUserControl.UpdateCount++;
			SetButtonState();
			borderUserControl.UpdateCount--;
		}
		void InitializePreviewControl(BorderShadingUserControl control) {
			control.BeginSetProperties();
			control.CurrentBorderInfo = controller.GetInitialBorder();
			control.BorderLineUp = controller.BorderLineUp;
			control.BorderLineDown = controller.BorderLineDown;
			control.BorderLineHorizontalIn = controller.BorderLineHorizontalIn;
			control.BorderLineLeft = controller.BorderLineLeft;
			control.BorderLineRight = controller.BorderLineRight;
			control.BorderLineVerticalIn = controller.BorderLineVerticalIn;
			control.BorderLineHorizontalInVisible = controller.BorderLineHorizontalInVisible;
			control.BorderLineVerticalInVisible = controller.BorderLineVerticalInVisible;
			control.FillColor =  XpfTypeConverter.ToPlatformColor(controller.GetActiveColor());
			control.DrawColumns = SetDrawColumns();
			if (!control.DrawColumns)
				control.DrawParagraph = SetDrawParagraph();
			control.EndSetProperties();
		}
		bool SetDrawColumns() {
			return controller.BorderLineVerticalInVisible;
		}
		bool SetDrawParagraph() {
			return controller.BorderLineHorizontalInVisible;
		}
		void SetButtonState() {
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.All)
				btnAll.IsChecked = true;
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.Box)
				btnBox.IsChecked = true;
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.Grid)
				btnGrid.IsChecked = true;
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.Custom)
				btnCustom.IsChecked = true;
			if (controller.SetModeButton == BorderShadingFormController.SetModeButtons.None)
				btnNone.IsChecked = true;
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
		}
		void InitializeForm() {
			borderUserControl.DocumentModel = controller.DocumentModel;
			shadingUserControl.DocumentModel = controller.DocumentModel;
			borderShadingTypeLineUserControl.DocumentModel = controller.DocumentModel;
			borderShadingTypeLineUserControl.RichEditControl = controller.RichEditControl;
			borderShadingTypeLineUserControl.SetInition(controller.GetInitialBorder());
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			borderUserControl.BorderLineDownChanged -= borderUserControl_BorderLineDownChanged;
			borderUserControl.BorderLineUpChanged -= borderUserControl_BorderLineUpChanged;
			borderUserControl.BorderLineVerticalInChanged -= borderUserControl_BorderLineVerticalInChanged;
			borderUserControl.BorderLineHorizontalInChanged -= borderUserControl_BorderLineHorizontalInChanged;
			borderUserControl.BorderLineLeftChanged -= borderUserControl_BorderLineLeftChanged;
			borderUserControl.BorderLineRightChanged -= borderUserControl_BorderLineRightChanged;
			colorEdit.ColorChanged -= colorEdit_ColorChanged;
			borderShadingTypeLineUserControl.BorderChanged -= borderShadingTypeLineUserControl_BorderChanged;
		}
		protected internal virtual void SubscribeControlsEvents() {
			borderUserControl.BorderLineDownChanged += borderUserControl_BorderLineDownChanged;
			borderUserControl.BorderLineUpChanged += borderUserControl_BorderLineUpChanged;
			borderUserControl.BorderLineVerticalInChanged += borderUserControl_BorderLineVerticalInChanged;
			borderUserControl.BorderLineHorizontalInChanged += borderUserControl_BorderLineHorizontalInChanged;
			borderUserControl.BorderLineLeftChanged += borderUserControl_BorderLineLeftChanged;
			borderUserControl.BorderLineRightChanged += borderUserControl_BorderLineRightChanged;
			colorEdit.ColorChanged += colorEdit_ColorChanged;
			borderShadingTypeLineUserControl.BorderChanged += borderShadingTypeLineUserControl_BorderChanged;
		}
		protected internal virtual void UpdateForm() {
		}
		protected internal virtual void ApplyChanges() {
			if (controller != null)
				controller.ApplyChanges();
		}
		private void borderUserControl_BorderLineUpChanged(object sender, EventArgs e) {
			controller.BorderLineUp = borderUserControl.BorderLineUp;
		}
		private void borderUserControl_BorderLineDownChanged(object sender, EventArgs e) {
			controller.BorderLineDown = borderUserControl.BorderLineDown;
		}
		private void borderUserControl_BorderLineHorizontalInChanged(object sender, EventArgs e) {
			controller.BorderLineHorizontalIn = borderUserControl.BorderLineHorizontalIn;
		}
		private void borderUserControl_BorderLineLeftChanged(object sender, EventArgs e) {
			controller.BorderLineLeft = borderUserControl.BorderLineLeft;
		}
		private void borderUserControl_BorderLineRightChanged(object sender, EventArgs e) {
			controller.BorderLineRight = borderUserControl.BorderLineRight;
		}
		private void borderUserControl_BorderLineVerticalInChanged(object sender, EventArgs e) {
			controller.BorderLineVerticalIn = borderUserControl.BorderLineVerticalIn;
		}
		private void btnNone_Checked(object sender, RoutedEventArgs e) {
				borderUserControl.SetNoneMode();
				ButtonChecked(btnNone);
		}
		private void borderShadingTypeLineUserControl_BorderChanged(object sender, EventArgs e) {
			borderUserControl.CurrentBorderInfo = borderShadingTypeLineUserControl.Border.Clone();
		}
		private void btnBox_Checked(object sender, RoutedEventArgs e) {
				borderUserControl.SetBoxMode();
				ButtonChecked(btnBox);
		}
		private void btnAll_Checked(object sender, RoutedEventArgs e) {
			borderUserControl.SetNoneMode();
			borderUserControl.SetAllMode();
			ButtonChecked(btnAll);
		}
		private void btnGrid_Checked(object sender, RoutedEventArgs e) {
			borderUserControl.SetNoneMode(); 
			borderUserControl.SetGridMode();
			ButtonChecked(btnGrid);
		}
		private void btnCustom_Checked(object sender, RoutedEventArgs e) {
				borderUserControl.SetCustomMode();
				ButtonChecked(btnCustom);
		}
		void ButtonChecked(ToggleButton tgButton) {
			btnNone.Checked -= btnNone_Checked;
			btnAll.Checked -= btnAll_Checked;
			btnGrid.Checked -= btnGrid_Checked;
			btnBox.Checked -= btnBox_Checked;
			btnCustom.Checked -= btnCustom_Checked;
			btnNone.IsChecked = false;
			btnAll.IsChecked = false;
			btnGrid.IsChecked = false;
			btnBox.IsChecked = false;
			btnCustom.IsChecked = false;
			tgButton.IsChecked = true;
			btnNone.Checked += btnNone_Checked;
			btnAll.Checked += btnAll_Checked;
			btnGrid.Checked += btnGrid_Checked;
			btnBox.Checked += btnBox_Checked;
			btnCustom.Checked += btnCustom_Checked;
		}
		private void colorEdit_ColorChanged(object sender, RoutedEventArgs e) {
			System.Windows.Media.Color mediacolor = colorEdit.Color;
#if!SL
			controller.FillColor = System.Drawing.Color.FromArgb(mediacolor.A, mediacolor.R, mediacolor.G, mediacolor.B);
#else
			controller.FillColor = mediacolor;
#endif
			borderUserControl.FillColor = colorEdit.Color;
			shadingUserControl.FillColor = colorEdit.Color;
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
	#endregion
}

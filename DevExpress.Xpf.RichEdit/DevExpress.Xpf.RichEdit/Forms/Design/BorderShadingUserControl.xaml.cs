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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Model;
using System.Windows.Controls.Primitives;
namespace DevExpress.Xpf.RichEdit.UI {
	public partial class BorderShadingUserControl : UserControl, IBorderShadingUserControl {
		#region Fields
		BorderInfo borderLineUp;
		BorderInfo borderLineDown;
		BorderInfo borderLineHorizontalIn;
		BorderInfo borderLineLeft;
		BorderInfo borderLineRight;
		BorderInfo borderLineVerticalIn;
		BorderInfo currentBorderInfo = new BorderInfo();
		BorderInfo inactiveBorderInfo = new BorderInfo();
		int updateCount;
		SetMode setMode;
		bool btnsVisible;
		bool borderLineHorizontalInVisible;
		bool borderLineVerticalInVisible;
		#endregion
		public BorderShadingUserControl() {
			InitializeComponent();
			inactiveBorderInfo.Style = BorderLineStyle.None;
			setMode = SetMode.Custom;
			Loaded += BorderShadingUserControl_Loaded;
			InitializePreviewUserControl(previewBSUserControl);
		}
		void BorderShadingUserControl_Loaded(object sender, RoutedEventArgs e) {
#if!SL
			textBlock.Visibility = System.Windows.Visibility.Hidden;
			comboBoxEdit.Visibility = System.Windows.Visibility.Hidden;
			btnTableOptions.Visibility = System.Windows.Visibility.Hidden;
#else
			textBlock.Opacity = 0;
			textBlock.IsHitTestVisible = false;
			comboBoxEdit.Opacity = 0;
			comboBoxEdit.IsHitTestVisible = false;
			btnTableOptions.Opacity = 0;
			btnTableOptions.IsHitTestVisible = false;
#endif        
		}
		void InitializePreviewUserControl(PreviewBSUserControl control) {
			control.BeginSetProperties();
			previewBSUserControl.BorderInfoSource = this;
			control.EndSetProperties();
		}
		public void BeginSetProperties() {
			updateCount++;
			previewBSUserControl.BeginSetProperties();
		}
		public void EndSetProperties() {
			updateCount--;
			previewBSUserControl.EndSetProperties();
		}
		#region Properties
		public DocumentModel DocumentModel {
			get { return previewBSUserControl.DocumentModel; }
			set {
				previewBSUserControl.DocumentModel = value;
			}
		}
		internal bool DrawColumns {
			get { return previewBSUserControl.DrawColumns; }
			set {
				previewBSUserControl.DrawColumns = value;
			}
		}
		internal bool DrawParagraph {
			get { return previewBSUserControl.DrawParagraph; }
			set {
				previewBSUserControl.DrawParagraph = value;
			}
		}
		bool InsideSetProperties { get { return updateCount > 0; } }
		public int UpdateCount {
			get { return updateCount; }
			set {
				updateCount = value;
			}
		}
		public Color FillColor {
			get { return (previewBSUserControl.FillColor); }
			set {
				previewBSUserControl.FillColor = value;
			}
		}
		public bool BtnsVisible {
			get { return btnsVisible; }
			set {
				btnsVisible = value;
				BtnsVisibleChanged();
			}
		}
		public bool BorderLineHorizontalInVisible {
			get { return borderLineHorizontalInVisible; }
			set {
				borderLineHorizontalInVisible = value;
				if (borderLineHorizontalInVisible)
					SetButtonVisibleState(btnHorizontBorderIn);
				else {
					SetButtonUnvisibleState(btnHorizontBorderIn);
					previewBSUserControl.HorizontalLineIn = BorderLineState.No;
				}
			}
		}
		public bool BorderLineVerticalInVisible {
			get { return borderLineVerticalInVisible; }
			set {
				borderLineVerticalInVisible = value;
				if (borderLineVerticalInVisible)
					SetButtonVisibleState(btnVerticalBorderIn);
				else {
					SetButtonUnvisibleState(btnVerticalBorderIn);
					previewBSUserControl.VerticalLineIn = BorderLineState.No;
				}
			}
		}
		public BorderInfo CurrentBorderInfo {
			get { return (currentBorderInfo); }
			set {
				currentBorderInfo = value;
				if (setMode != SetMode.Custom)
					SetNoneModeCore();
				if (setMode == SetMode.Box)
					SetBoxModeCore();
				if (setMode == SetMode.All)
					SetAllModeCore();
				if (setMode == SetMode.Grid)
					SetGridModeCore();
			}
		}
		public BorderInfo BorderLineUp {
			get { return (borderLineUp); }
			set {
				borderLineUp = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnHorizontBorderUp.IsChecked = true;
						previewBSUserControl.HorizontalLineUp = BorderLineState.Known;
					}
					else {
						btnHorizontBorderUp.IsChecked = false;
						previewBSUserControl.HorizontalLineUp = BorderLineState.No;
					}
				}
				else {
					btnHorizontBorderUp.IsChecked = false;
					previewBSUserControl.HorizontalLineUp = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineDown {
			get { return (borderLineDown); }
			set {
				borderLineDown = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnHorizontBorderDown.IsChecked = true;
						previewBSUserControl.HorizontalLineDown = BorderLineState.Known;
					}
					else {
						btnHorizontBorderDown.IsChecked = false;
						previewBSUserControl.HorizontalLineDown = BorderLineState.No;
					}
				}
				else {
					btnHorizontBorderDown.IsChecked = false;
					previewBSUserControl.HorizontalLineDown = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineHorizontalIn {
			get { return (borderLineHorizontalIn); }
			set {
				borderLineHorizontalIn = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnHorizontBorderIn.IsChecked = true;
						previewBSUserControl.HorizontalLineIn = BorderLineState.Known;
					}
					else {
						btnHorizontBorderIn.IsChecked = false;
						previewBSUserControl.HorizontalLineIn = BorderLineState.No;
					}
				}
				else {
					btnHorizontBorderIn.IsChecked = false;
					previewBSUserControl.HorizontalLineIn = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineLeft {
			get { return (borderLineLeft); }
			set {
				borderLineLeft = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnVerticalBorderLeft.IsChecked = true;
						previewBSUserControl.VerticalLineLeft = BorderLineState.Known;
					}
					else {
						btnVerticalBorderLeft.IsChecked = false;
						previewBSUserControl.VerticalLineLeft = BorderLineState.No;
					}
				}
				else {
					btnVerticalBorderLeft.IsChecked = false;
					previewBSUserControl.VerticalLineLeft = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineRight {
			get { return (borderLineRight); }
			set {
				borderLineRight = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnVerticalBorderRight.IsChecked = true;
						previewBSUserControl.VerticalLineRight = BorderLineState.Known;
					}
					else {
						btnVerticalBorderRight.IsChecked = false;
						previewBSUserControl.VerticalLineRight = BorderLineState.No;
					}
				}
				else {
					btnVerticalBorderRight.IsChecked = false;
					previewBSUserControl.VerticalLineRight = BorderLineState.Unknown;
				}
			}
		}
		public BorderInfo BorderLineVerticalIn {
			get { return (borderLineVerticalIn); }
			set {
				borderLineVerticalIn = value;
				if (value != null) {
					if (value.Style != BorderLineStyle.None) {
						btnVerticalBorderIn.IsChecked = true;
						previewBSUserControl.VerticalLineIn = BorderLineState.Known;
					}
					else {
						btnVerticalBorderIn.IsChecked = false;
						previewBSUserControl.VerticalLineIn = BorderLineState.No;
					}
				}
				else {
					btnVerticalBorderIn.IsChecked = false;
					previewBSUserControl.VerticalLineIn = BorderLineState.Unknown;
				}
			}
		}
		public static BorderLineState HorizontalLineUp { get; set; }
		#endregion
		#region Events
		public event EventHandler BorderLineUpChanged;
		protected void RaiseBorderLineUpChanged() {
			if (BorderLineUpChanged != null)
				BorderLineUpChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineDownChanged;
		protected void RaiseBorderLineDownChanged() {
			if (BorderLineDownChanged != null)
				BorderLineDownChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineHorizontalInChanged;
		protected void RaiseBorderLineHorizontalInChanged() {
			if (BorderLineHorizontalInChanged != null)
				BorderLineHorizontalInChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineVerticalInChanged;
		protected void RaiseBorderLineVerticalInChanged() {
			if (BorderLineVerticalInChanged != null)
				BorderLineVerticalInChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineLeftChanged;
		protected void RaiseBorderLineLeftChanged() {
			if (BorderLineLeftChanged != null)
				BorderLineLeftChanged(this, EventArgs.Empty);
		}
		public event EventHandler BorderLineRightChanged;
		protected void RaiseBorderLineRightChanged() {
			if (BorderLineRightChanged != null)
				BorderLineRightChanged(this, EventArgs.Empty);
		}
		#endregion
		void btnHorizontBorderUp_Checked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineUp = currentBorderInfo;
			previewBSUserControl.HorizontalLineUp = BorderLineState.Known;
			RaiseBorderLineUpChanged();
		}
		void btnHorizontBorderUp_Unchecked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineUp = inactiveBorderInfo;
			previewBSUserControl.HorizontalLineUp = BorderLineState.No;
			RaiseBorderLineUpChanged();
		}
		private void previewBSUserControl1_Load(object sender, EventArgs e) {
		}
		void btnHorizontBorderIn_Checked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			if (setMode == SetMode.Grid && borderLineHorizontalInVisible){
				borderLineHorizontalIn = currentBorderInfo.Clone();
				borderLineHorizontalIn.Style = BorderLineStyle.Single;
				borderLineHorizontalIn.Width = DocumentModel.UnitConverter.TwipsToModelUnits(15);
			}
			else
				borderLineHorizontalIn = currentBorderInfo; 
			previewBSUserControl.HorizontalLineIn = BorderLineState.Known;
			RaiseBorderLineHorizontalInChanged();
		}
		void btnHorizontBorderIn_Unchecked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineHorizontalIn = inactiveBorderInfo;
			previewBSUserControl.HorizontalLineIn = BorderLineState.No;
			RaiseBorderLineHorizontalInChanged();
		}
		void btnHorizontBorderDown_Checked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineDown = currentBorderInfo;
			previewBSUserControl.HorizontalLineDown = BorderLineState.Known;
			RaiseBorderLineDownChanged();
		}
		void btnHorizontBorderDown_Unchecked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineDown = inactiveBorderInfo;
			previewBSUserControl.HorizontalLineDown = BorderLineState.No;
			RaiseBorderLineDownChanged();
		}
		void btnVerticalBorderLeft_Checked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineLeft = currentBorderInfo;
			previewBSUserControl.VerticalLineLeft = BorderLineState.Known;
			RaiseBorderLineLeftChanged();
		}
		void btnVerticalBorderLeft_Unchecked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineLeft = inactiveBorderInfo;
			previewBSUserControl.VerticalLineLeft = BorderLineState.No;
			RaiseBorderLineLeftChanged();
		}
		void btnVerticalBorderIn_Checked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			if ((setMode == SetMode.Grid) && borderLineVerticalInVisible) {
				borderLineVerticalIn = currentBorderInfo.Clone();
				borderLineVerticalIn.Style = BorderLineStyle.Single;
				borderLineVerticalIn.Width = DocumentModel.UnitConverter.TwipsToModelUnits(15);
			}
			else
				borderLineVerticalIn = currentBorderInfo; 
			previewBSUserControl.VerticalLineIn = BorderLineState.Known;
			RaiseBorderLineVerticalInChanged();
		}
		void btnVerticalBorderIn_Unchecked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineVerticalIn = inactiveBorderInfo;
			previewBSUserControl.VerticalLineIn = BorderLineState.No;
			RaiseBorderLineVerticalInChanged();
		}
		void btnVerticalBorderRight_Checked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineRight = currentBorderInfo;
			previewBSUserControl.VerticalLineRight = BorderLineState.Known;
			RaiseBorderLineRightChanged();
		}
		void btnVerticalBorderRight_Unchecked(object sender, RoutedEventArgs e) {
			if (InsideSetProperties)
				return;
			borderLineRight = inactiveBorderInfo;
			previewBSUserControl.VerticalLineRight = BorderLineState.No;
			RaiseBorderLineRightChanged();
		}
		void SetNoneModeCore() {
			btnHorizontBorderDown.IsChecked = false;
			btnHorizontBorderUp.IsChecked = false;
			btnVerticalBorderRight.IsChecked = false;
			btnVerticalBorderLeft.IsChecked = false;
			btnHorizontBorderIn.IsChecked = false;
			btnVerticalBorderIn.IsChecked = false;
		}
		public void SetNoneMode() {
			setMode = SetMode.None;
			SetNoneModeCore();
		}
		void SetBoxModeCore() {
			btnHorizontBorderDown.IsChecked = true;
			btnHorizontBorderUp.IsChecked = true;
			btnVerticalBorderRight.IsChecked = true;
			btnVerticalBorderLeft.IsChecked = true;
			btnHorizontBorderIn.IsChecked = false;
			btnVerticalBorderIn.IsChecked = false;
		}
		public void SetBoxMode() {
			setMode = SetMode.Box;
			SetBoxModeCore();
		}
		void SetAllModeCore() {
			btnHorizontBorderDown.IsChecked = true;
			btnHorizontBorderUp.IsChecked = true;
			btnVerticalBorderRight.IsChecked = true;
			btnVerticalBorderLeft.IsChecked = true;
			btnHorizontBorderIn.IsChecked = true;
			btnVerticalBorderIn.IsChecked = true;
		}
		public void SetAllMode() {
			setMode = SetMode.All;
			SetAllModeCore();
		}
		void SetGridModeCore() {
			btnHorizontBorderDown.IsChecked = true;
			btnHorizontBorderUp.IsChecked = true;
			btnVerticalBorderRight.IsChecked = true;
			btnVerticalBorderLeft.IsChecked = true;
			btnHorizontBorderIn.IsChecked = true;
			btnVerticalBorderIn.IsChecked = true;
		}
		public void SetGridMode() {
			setMode = SetMode.Grid;
			SetGridModeCore();
		}
		void SetCustomModeCore() {
			btnHorizontBorderDown.IsChecked = true;
			btnHorizontBorderUp.IsChecked = true;
			btnVerticalBorderRight.IsChecked = true;
			btnVerticalBorderLeft.IsChecked = true;
			btnHorizontBorderIn.IsChecked = true;
			btnVerticalBorderIn.IsChecked = true;
		}
		public void SetCustomMode() {
			setMode = SetMode.Custom;
			SetCustomModeCore();
		}
		public enum SetMode { None, Box, All, Grid, Custom }
		private void BtnsVisibleChanged() {
			if (btnsVisible) {
				SetButtonVisibleState(btnHorizontBorderUp);
				if (borderLineHorizontalInVisible) {
					SetButtonVisibleState(btnHorizontBorderIn);
				}
				SetButtonVisibleState(btnHorizontBorderDown);
				if (borderLineVerticalInVisible) {
					SetButtonVisibleState(btnVerticalBorderIn);
				}
				SetButtonVisibleState(btnVerticalBorderLeft);
				SetButtonVisibleState(btnVerticalBorderRight);
				SetTextBlockVisibleState(textBlock1);
				SetTextBlockVisibleState(textBlock2);
			}
			else {
				SetButtonUnvisibleState(btnHorizontBorderUp);
				SetButtonUnvisibleState(btnHorizontBorderIn);
				SetButtonUnvisibleState(btnHorizontBorderDown);
				SetButtonUnvisibleState(btnVerticalBorderIn);
				SetButtonUnvisibleState(btnVerticalBorderLeft);
				SetButtonUnvisibleState(btnVerticalBorderRight);
				SetTextBlockUnvisibleState(textBlock1);
				SetTextBlockUnvisibleState(textBlock2);
			}
		}
		void SetButtonVisibleState(ToggleButton tgButton) {
#if!SL
			tgButton.Visibility = System.Windows.Visibility.Visible;
#else
			tgButton.Opacity = 1;
			tgButton.IsHitTestVisible = true;
#endif
		}
		void SetButtonUnvisibleState(ToggleButton tgButton) {
#if!SL
			tgButton.Visibility = System.Windows.Visibility.Hidden;
#else
			tgButton.Opacity = 0;
			tgButton.IsHitTestVisible = false;
#endif
		}
		void SetTextBlockUnvisibleState(TextBlock textBlock) {
			textBlock.Visibility = System.Windows.Visibility.Hidden;
		}
		void SetTextBlockVisibleState(TextBlock textBlock) {
			textBlock.Visibility = System.Windows.Visibility.Visible;
		}
	}
}

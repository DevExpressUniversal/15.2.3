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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.DashboardWin.Native {
	public class DashboardTitlePresenter {
		public const int ImageSize = 24;
		internal const int ImageTextPadding = 3;
		const int ImageLeftPadding = 3;
		internal const int TextFilterImagePadding = 2;
		internal const int ImagePadding = 4;
		public static string FormatFilterValue(FormattableValue value) {
			string text = string.Empty;
			FormatterBase formatterBase = FormatterBase.CreateFormatter(value.Format);
			if(value.Value != null)
				text = formatterBase.Format(value.Value);
			else if(value.RangeLeft != null && value.RangeRight != null)
				text = String.Format("{0} - {1}", formatterBase.Format(value.RangeLeft), formatterBase.Format(value.RangeRight));
			return text;
		}
		readonly IDashboardTitleView view;
		readonly IDashboardTitleCustomizationService customizationService;
		DashboardTitleViewModel titleViewModel;
		string titleText;
		IList<DimensionFilterValues> masterFilterValues;
		bool exportButtonVisible;
		Size imageSize;
		Size textBestSize;
		Rectangle imageBounds;
		Rectangle textBounds;
		Rectangle filterImageBounds;
		public string TitleText { get { return titleText; } }
		public bool TitleVisible { get { return titleViewModel.Visible; } }
		TitleLayoutViewModel LayoutViewModel { get { return titleViewModel.LayoutModel; } }
		DashboardImageViewModel ImageViewModel { 
			get { 
				return LayoutViewModel != null ? 
					LayoutViewModel.ImageViewModel : 
					null;
			}
		}			 
		DashboardTitleAlignment Alignment { 
			get {
				return LayoutViewModel != null ?
					LayoutViewModel.Alignment : 
					DashboardTitleAlignment.Left; 
			}
		}
		bool HasMasterFilterValues { 
			get { 
				return titleViewModel.IncludeMasterFilterValues &&
					masterFilterValues != null && 
					masterFilterValues.Count > 0; 
			} 
		}
		bool FilterTextVisible { 
			get {
				return HasMasterFilterValues && 
					masterFilterValues.Count == 1 && 
					masterFilterValues[0].Values.Count == 1; 
			}
		}
		bool FilterImageVisible {
			get {
				return HasMasterFilterValues &&
					!FilterTextVisible;
			}
		}
		FormattableValue FilterValue { get { return masterFilterValues[0].Values[0]; } }
		public DashboardTitlePresenter(IDashboardTitleView view, IDashboardTitleCustomizationService customizationService) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.customizationService = customizationService;
			view.SizeChanged += (s, e) => {
				if(titleViewModel != null && !updateLocker.IsLocked) {
					CalculateHeight();
					CalculateBoundsHorizontally();
					SetBounds();
				}
			};
		}
		public void Update(IList<DimensionFilterValues> masterFilterValues) {
			Update(titleViewModel, masterFilterValues, exportButtonVisible);
		}
		public void Update(bool exportButtonVisible) {
			Update(titleViewModel, masterFilterValues, exportButtonVisible);
		}
		Locker updateLocker = new Locker();
		public void Update(DashboardTitleViewModel newModel, IList<DimensionFilterValues> masterFilterValues, bool exportButtonVisible) {
			bool shouldUpdateTitle;
			EnsureTitleViewModel(newModel, out shouldUpdateTitle);
			if (shouldUpdateTitle) {
				updateLocker.Lock();
				view.BeginUpdate();
				try {
					this.masterFilterValues = masterFilterValues;
					this.exportButtonVisible = exportButtonVisible;
					if (titleViewModel != null && titleViewModel.Visible) {
						view.Visible = true;
						UpdateImage();
						UpdateFilterImage();
						UpdateText();
						CalculateHeight();
						CalculateButtons();
						CalculateBoundsVertically();
						CalculateBoundsHorizontally();
						SetBounds();
					}
					else
						view.Visible = false;
				}
				finally {
					view.EndUpdate();
					updateLocker.Unlock();
				}
			}
		}
		void EnsureTitleViewModel(DashboardTitleViewModel newModel, out bool shouldUpdateTitle) {
			shouldUpdateTitle = false;
			if (newModel != null) {				
				titleViewModel = newModel;
				shouldUpdateTitle = true;
			}
			if (titleViewModel != null) {
				if (customizationService != null) {
					string newText = customizationService.CustomizeTitleText(titleViewModel.Text);
					if (newText != titleText) {						
						titleText = newText;
						shouldUpdateTitle = true;						
					}
				}
				else
					titleText = titleViewModel.Text;
			}
		}
		void UpdateImage() {
			DashboardImageViewModel imageViewModel = ImageViewModel;
			bool hasImage = false;
			if(imageViewModel != null) {
				DashboardTitleImageInfo imageInfo = DashboardTitleImageInfo.Create(imageViewModel);
				if(imageInfo != null) {
					view.UpdateImage(imageInfo.Image);
					imageSize = imageInfo.ImageSize;
					hasImage = true;
				}
			}
			if(!hasImage) {
				view.UpdateImage(null);
				imageSize = Size.Empty;
			}
		}
		void UpdateFilterImage() {
			if(titleViewModel.IncludeMasterFilterValues)
				view.UpdateMasterFilterValues(masterFilterValues);
			view.UpdateFilterImageVisible(FilterImageVisible);
		}
		void UpdateText() {
			string filterText = string.Empty;
			if(FilterTextVisible)
				filterText = String.Format("  {0}", FormatFilterValue(FilterValue));
			textBestSize = view.UpdateText(titleText, filterText);
		}
		void CalculateHeight() {
			int height = Math.Max(DashboardTitleViewModel.MinCaptionHeight, textBestSize.Height);
			height = Math.Max(height, imageSize.Height);
			height = Math.Min(DashboardTitleViewModel.MaxCaptionHeight, height);
			view.Height = height;
		}
		void CalculateButtons() {
			int width = 0;
			int buttonTop = (view.Height - ImageSize) / 2;
			if(exportButtonVisible) {
				view.UpdateExportButtonBounds(new Rectangle(ImageLeftPadding, buttonTop, ImageSize, ImageSize));
				width += ImageSize + ImagePadding;
			} else
				view.UpdateExportButtonBounds(null);
			if(titleViewModel.ShowParametersButton) {
				view.UpdateParameterButtonBounds(new Rectangle(ImageLeftPadding + width, buttonTop, ImageSize, ImageSize));
				width += ImageSize + ImagePadding;
			} else
				view.UpdateParameterButtonBounds(null);
			if(exportButtonVisible || titleViewModel.ShowParametersButton)
				width += ImagePadding;
			view.ButtonsWidth = width;
		}
		void CalculateBoundsVertically() {
			imageBounds.Y = 0;
			imageBounds.Height = imageSize.Height;
			textBounds.Y = (view.Height - textBestSize.Height) / 2;
			textBounds.Height = textBestSize.Height;
			filterImageBounds.Y = textBounds.Y;
			filterImageBounds.Height = view.Height;
		}
		void CalculateBoundsHorizontally() {
			int contentWidth = view.Width - view.ButtonsWidth;
			int imageTextPadding = imageSize.Width > 0 && textBestSize.Width > 0 ? ImageTextPadding : 0;
			int filterWidth = FilterImageVisible ? ImageSize + TextFilterImagePadding : 0;
			int fullWidth = DashboardTitleViewModel.LeftPadding + textBestSize.Width + imageSize.Width + imageTextPadding + filterWidth;
			if(fullWidth < contentWidth) {
				int centerOffset = DashboardTitleViewModel.LeftPadding;
				if(Alignment == DashboardTitleAlignment.Center)
					centerOffset += (contentWidth - fullWidth) / 2;
				imageBounds.X = centerOffset;
				imageBounds.Width = imageSize.Width;
				textBounds.X = imageBounds.Right + imageTextPadding;
				textBounds.Width = textBestSize.Width;
				filterImageBounds.X = textBounds.Right;
				filterImageBounds.Width = filterWidth;
			} else {
				imageBounds.X = DashboardTitleViewModel.LeftPadding;
				imageBounds.Width = imageSize.Width;
				filterImageBounds.Width = filterWidth;
				filterImageBounds.X = contentWidth - filterWidth;
				textBounds.X = imageBounds.Right + imageTextPadding;
				textBounds.Width = filterImageBounds.X - textBounds.X;
			}
		}
		void SetBounds() {
			view.UpdateImageBounds(imageBounds);
			view.UpdateLabelBounds(textBounds);
			view.UpdateFilterImageBounds(filterImageBounds);
		}
	}
}

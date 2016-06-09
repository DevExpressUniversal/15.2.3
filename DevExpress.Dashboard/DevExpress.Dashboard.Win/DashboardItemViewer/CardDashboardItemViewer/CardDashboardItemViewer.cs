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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Skins;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class CardDashboardItemViewer : DataDashboardItemViewer {
		readonly CardContainerControl cardContainer = new CardContainerControl();
		readonly ContentScrollableControl contentScrollableControl;
		protected override string ElementName { get { return DashboardLocalizer.GetString(DashboardStringId.ElementNameCards); } }
		protected override bool SupportsTransparentBackColor { get { return true; } }
#if DEBUGTEST
		public CardContainerControl CardContainer { get { return cardContainer; } }
		public ContentScrollableControl ContentScrollableControl { get { return contentScrollableControl; } }
#endif
		CardDashboardItemViewModel CardViewModel { get { return (CardDashboardItemViewModel)ViewModel; } }
		public CardDashboardItemViewer() {
			contentScrollableControl = new ContentScrollableControl(cardContainer);
		}
		protected override DataPointInfo GetDataPointInfo(Point location, bool onlyTargetAxes) {
			IValuesProvider valueProvider = contentScrollableControl.GetHitItem(location) as IValuesProvider;
			if(valueProvider != null) {
				return GetDataPoint(valueProvider);
			}
			return null;
		}
		DataPointInfo GetDataPoint(IValuesProvider hitItem) {
			DataPointInfo dataPointInfo = new DataPointInfo();
			IList dimensionValues = hitItem.SelectionValues;
			string id = hitItem.MeasureID;
			if(dimensionValues != null) {
				dataPointInfo.DimensionValues.Add(DashboardDataAxisNames.DefaultAxis, dimensionValues);
			}
			if(CardViewModel != null && CardViewModel.Cards != null && CardViewModel.Cards.Count > 0) {
				foreach(CardViewModel card in CardViewModel.Cards) {
					string cardId = card.ID;
					if(cardId == id) {
						if(card.DataItemType == KpiElementDataItemType.Delta)
							dataPointInfo.Deltas.Add(cardId);
						else
							dataPointInfo.Measures.Add(cardId);
					}
				}
			}
			return dataPointInfo;
		}
		void OnContentScrollableControlMouseClick(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseClick(e);
			RaiseClick(e.Location);
		}
		void OnContentScrollableControlMouseDoubleClick(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseDoubleClick(e);
			RaiseDoubleClick(e.Location);
		}
		void OnContentScrollableControlMouseMove(object sender, MouseEventArgs e) {
			OnDashboardItemViewerMouseMove(e);
			RaiseMouseMove(e.Location);
		}
		void OnContentScrollableControlMouseEnter(object sender, EventArgs e) {
			RaiseMouseEnter();
		}
		void OnContentScrollableControlMouseLeave(object sender, EventArgs e) {
			OnDashboardItemViewerMouseLeave();
			RaiseMouseLeave();
		}
		void OnContentScrollableControlMouseDown(object sender, MouseEventArgs e) {
			RaiseMouseDown(e.Location);
		}
		void OnContentScrollableControlMouseUp(object sender, MouseEventArgs e) {
			RaiseMouseUp(e.Location);
		}
		void OnContentScrollableControlMouseWheel(object sender, MouseEventArgs e) {
			RaiseMouseWheel();
		}
		void OnContentScrollableControlMouseHover(object sender, EventArgs e) {
			RaiseMouseHover();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				cardContainer.Paint -= DrawDisabledControl;
				contentScrollableControl.MouseClick -= OnContentScrollableControlMouseClick;
				contentScrollableControl.MouseDoubleClick -= OnContentScrollableControlMouseDoubleClick;
				contentScrollableControl.MouseMove -= OnContentScrollableControlMouseMove;
				contentScrollableControl.MouseEnter -= OnContentScrollableControlMouseEnter;
				contentScrollableControl.MouseLeave -= OnContentScrollableControlMouseLeave;
				contentScrollableControl.MouseDown -= OnContentScrollableControlMouseDown;
				contentScrollableControl.MouseUp -= OnContentScrollableControlMouseUp;
				contentScrollableControl.MouseHover -= OnContentScrollableControlMouseHover;
				contentScrollableControl.MouseWheel -= OnContentScrollableControlMouseWheel;
				contentScrollableControl.KeyDown -= OnContentScrollableControlKeyDown; 
				contentScrollableControl.KeyUp -= OnContentScrollableControlKeyUp;
				contentScrollableControl.LostFocus -= OnContentScrollableControlLostFocus;
				cardContainer.Dispose();
				contentScrollableControl.Dispose();
			}
			base.Dispose(disposing);
		}
		public override void OnLookAndFeelChanged() {
			cardContainer.OnLookAndFeelChanged();
			cardContainer.UpdateVisualState(CardViewModel);
			UpdateDashboardItemAppearance();
		}
		void UpdateDashboardItemAppearance() {
			bool showCaption =  CaptionViewModel != null && CaptionViewModel.ShowCaption;
			ItemContainer.ShowBorders = showCaption;
			if(showCaption) {
				Skin skin = CommonSkins.GetSkin(LookAndFeel);
				SetBackColors(skin.Colors.GetColor(CommonColors.Window));
				contentScrollableControl.Model.ContentArrangementOptions = ContentArrangementOptions.Default;
			}
			else {
				SetBackColors(Color.Transparent);
				contentScrollableControl.Model.ContentArrangementOptions = ContentArrangementOptions.IgnoreMargins;
			}
		}
		void SetBackColors(Color color) {
			BackColor = contentScrollableControl.BackColor = cardContainer.BackColor = color;
		}
		protected override Control GetViewControl() {
			return contentScrollableControl;
		}
		protected override void PrepareViewControl() {
			base.PrepareViewControl();
			cardContainer.Paint += DrawDisabledControl;
			contentScrollableControl.MouseClick += OnContentScrollableControlMouseClick;
			contentScrollableControl.MouseDoubleClick += OnContentScrollableControlMouseDoubleClick;
			contentScrollableControl.MouseMove += OnContentScrollableControlMouseMove;
			contentScrollableControl.MouseEnter += OnContentScrollableControlMouseEnter;
			contentScrollableControl.MouseLeave += OnContentScrollableControlMouseLeave;
			contentScrollableControl.MouseDown += OnContentScrollableControlMouseDown;
			contentScrollableControl.MouseUp += OnContentScrollableControlMouseUp;
			contentScrollableControl.MouseHover += OnContentScrollableControlMouseHover;
			contentScrollableControl.MouseWheel += OnContentScrollableControlMouseWheel;
			contentScrollableControl.KeyDown += OnContentScrollableControlKeyDown;
			contentScrollableControl.KeyUp += OnContentScrollableControlKeyUp;
			contentScrollableControl.LostFocus += OnContentScrollableControlLostFocus;
		}
		void OnContentScrollableControlLostFocus(object sender, EventArgs e) {
			OnDashboardItemViewerLostFocus();
		}
		void OnContentScrollableControlKeyDown(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyDown(e);
		}
		void OnContentScrollableControlKeyUp(object sender, KeyEventArgs e) {
			OnDashboardItemViewerKeyUp(e);
		}		
		protected override void UpdateViewer() {
			base.UpdateViewer();
			if(ViewModel.ShouldIgnoreUpdate)
				return;
			contentScrollableControl.Model.InitializeContent(CardViewModel.ContentDescription);
			cardContainer.UpdateVisualState(CardViewModel);
			contentScrollableControl.Model.InitializeArranger();
			DataDashboardItemDesigner designer = DesignerProvider.GetDesigner<DataDashboardItemDesigner>();
			if(designer != null) {
				cardContainer.Update(CardViewModel, MultiDimensionalData, DrillDownUniqueValues != null, true);
				designer.SetDataReducedImage(cardContainer.IsDataReduced);
			}
			else
				cardContainer.Update(CardViewModel, MultiDimensionalData, DrillDownUniqueValues != null, false);
			cardContainer.Invalidate();
		}
		protected override void UpdateCaption() {
			base.UpdateCaption();
			UpdateDashboardItemAppearance();
		}
		protected override void PrepareClientState(ItemViewerClientState state) {
			state.ViewerArea = GetControlClientArea(contentScrollableControl);
			contentScrollableControl.PrepareScrollingState(state);
			state.SpecificState = new Dictionary<string, object>();
			state.SpecificState.Add("CardMarginX", ((IWinContentProvider)cardContainer).ContentProvider.ItemMargin.Height);
			state.SpecificState.Add("CardMarginY", ((IWinContentProvider)cardContainer).ContentProvider.ItemMargin.Width);
			state.SpecificState.Add("CardAdditionalMarginX", 3); 
			state.SpecificState.Add("CardAdditionalMarginY", 3);
		}
		protected override void SetHighlight(List<AxisPointTuple> higtlight) {
			contentScrollableControl.HighlightValues(GetDimestionValueByAxis(higtlight, TargetAxes));
		}
		protected override void SetSelection(List<AxisPointTuple> selection) {
			contentScrollableControl.SelectValues(GetDimestionValueByAxis(selection, TargetAxes)); 
		}
	}
}

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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraWaitForm;
using DevExpress.Map.Localization;
namespace DevExpress.XtraMap.Native {
	public class MapSearchController : MapDisposableObject {
		readonly TextEdit inputEdit;
		readonly InteractionController owner;
		readonly LabelControl clearButton;
		readonly ListBoxControl searchResultList;
		string showMoreItemsString;
		string showOnlyItemString;
		SearchPanel searchPanel;
		InformationLayer activeInformationLayer;
		InnerMap Map { get { return this.owner.Map; } }
		bool IsAlternateResultState { get; set; }
		protected TextEdit InputEdit { get { return inputEdit; } }
		protected LabelControl ClearButton { get { return clearButton; } }
		protected ListBoxControl SearchResultList { get { return searchResultList; } }
		protected internal SearchPanel SearchPanel {
			get {
				if(searchPanel == null) {
					searchPanel = new SearchPanel(Map.OwnedControl.ToolTipController);
				}
				return searchPanel;
			}
		}
		BingSearchDataProvider BingSearchProvider { get { return activeInformationLayer != null ? activeInformationLayer.DataProvider as BingSearchDataProvider : null; } }
		public bool IsSearchPanelVisible { get { return searchPanel != null && searchPanel.Visible; } }
		bool IsAlternateResultAvailable { get { return BingSearchProvider != null ? BingSearchProvider.HasAlternateResults : false; } }
		public MapSearchController(InteractionController owner) {
			this.owner = owner;
			this.inputEdit = SearchPanel.InputEdit;
			this.clearButton = SearchPanel.ClearButton;
			this.searchResultList = SearchPanel.SearchResultList;
			UpdateAdditionalItemStrings();
		}
		public event EventHandler CommitChanges;
		public event EventHandler RollbackChanges;
		void UpdateClearButtonStyle() {
			ClearButton.BackColor = InputEdit.BackColor;
		}
		void UpdateInputEditStyle(ISkinProvider provider) {
			if(provider == null) {
				InputEdit.LookAndFeel.SetFlatStyle();
				return;
			}
			InputEdit.LookAndFeel.SetStyle(LookAndFeel.LookAndFeelStyle.Skin, false, false);
			InputEdit.LookAndFeel.SetSkinStyle(provider.SkinName);
		}
		void UpdateSearchResultListStyle(ISkinProvider provider) {
			if(provider == null) {
				SearchResultList.LookAndFeel.SetFlatStyle();
				return;
			}
			SearchResultList.LookAndFeel.SetStyle(LookAndFeel.LookAndFeelStyle.Skin, false, false);
			SearchResultList.LookAndFeel.SetSkinStyle(provider.SkinName);
		}
		void OnSearchResultListMouseDoubleClick(object sender, MouseEventArgs e) {
			if(SearchResultList.SelectedIndex < SearchResultList.Items.Count - 1)
				StartAlternateSearch();
		}
		void OnInputTextChanged(object sender, EventArgs e) {
			if(SearchResultList.Visible)
				SearchResultList.SelectedIndex = -1;
			StartSearch(InputEdit.Text);
		}
		void StartAlternateSearch() {
			if(InputEdit != null && SearchResultList != null && SearchResultList.SelectedItem != null) {
				InputEdit.Focus();
				InputEdit.Text = SearchResultList.SelectedItem.ToString();
				SearchResultList.SelectedIndex = -1;
			}
		}
		void OnClearButtonClick(object sender, EventArgs e) {
			RecreateLocalizedStrings();
			SearchPanel.Reset();
			BingSearchProvider.ClearResults();
			Map.RenderController.UpdateSearchPanel();
		}
		void AlternateButtonClick() {
			if(SearchPanel != null && BingSearchProvider != null && IsAlternateResultAvailable) {
				SubscribeRequestCompletedEvents();
				if(SearchPanel.AreAlternateRequestsListed) {
					IsAlternateResultState = false;
					BingSearchProvider.GenerateSearchResults();
				}
				else {
					IsAlternateResultState = true;
					BingSearchProvider.GenerateAlternateSearchRequests();
				}
				SearchPanel.UpdateAlternateRequestsListed();
			}
			Map.RenderController.UpdateSearchPanel();
		}
		void OnSearchResultListMouseClick(object sender, MouseEventArgs e) {
			if(BingSearchProvider != null && BingSearchProvider.ShowSearchPanel && searchPanel != null) {
				if(SearchResultList.SelectedIndex == SearchResultList.Items.Count - 1 && SearchResultList.Items.Count > 0) {
					AlternateButtonClick();
					return;
				}
				MoveToResult();
			}
		}
		void MoveToResult() {
			foreach(MapItem item in activeInformationLayer.DataItems) {
				MapPushpin pushpin = item as MapPushpin;
				if(pushpin != null) {
					LocationInformation locationInfo = pushpin.Information as LocationInformation;
					if(Map != null && locationInfo != null && locationInfo == searchPanel.SelectedResult)
						Map.CenterPoint = pushpin.CurrentLocation;
				}
			}
		}
		InformationLayer FindSearchPanelLayer() {
			foreach(LayerBase item in Map.ActualLayers) {
				InformationLayer layer = item as InformationLayer;
				if(Map.OperationHelper.CanShowSearchPanel(layer))
					return layer;
			}
			return null;
		}
		void UpdateSearchPanelVisibility(bool visible) {
			if(visible) {
				LayoutSearchPanel(Map.ContentRectangle);
			}
			else
				SearchPanel.Reset();
			SearchPanel.Visible = visible;
		}
		void SubscribeRequestCompletedEvents() {
			activeInformationLayer.DataRequestCompleted += OnDataRequestCompleted;
		}
		void UnsubscribeRequestCompletedEvents() {
			activeInformationLayer.DataRequestCompleted -= OnDataRequestCompleted;
		}
		void StartSearch(string searchString) {
			if(this.activeInformationLayer == null)
				return;
			IsAlternateResultState = false;
			if(activeInformationLayer.StartSearch(searchString)) {
				SubscribeRequestCompletedEvents();
				searchPanel.IsBusy = true;
				searchPanel.UpdateVisualState(false);
			}
		}
		void OnDataRequestCompleted(object sender, RequestCompletedEventArgs e) {
			UnsubscribeRequestCompletedEvents();
			UpdateSearchPanel(e);
		}
		void UpdateSearchPanel(RequestCompletedEventArgs e) {
			if(searchPanel != null && BingSearchProvider != null) {
				searchPanel.SearchResultList.Items.Clear();
				List<LocationInformation> addresses = e.UserState as List<LocationInformation>;
				if(!e.Cancelled && e.Error == null && addresses != null)
					foreach(LocationInformation address in addresses)
						searchPanel.SearchResultList.Items.Add(address);
				string additionalItem = GetAdditionalItemText();
				if (!string.IsNullOrEmpty(additionalItem)) {
					BingAddress address = new BingAddress() { FormattedAddress = additionalItem };
					SearchResultList.Items.Add(new LocationInformation() { Address = address });
				}
				searchPanel.IsBusy = false;
				MoveToResult();
				searchPanel.UpdateVisualState(IsAlternateResultAvailable);
				SearchResultList.Height = SearchPanel.GetResultListHeight();
				Map.RenderController.UpdateSearchPanel();
			}
		}
		protected override void DisposeOverride() {
			if(searchPanel != null) {
				searchPanel.RemoveControls(Map.OwnedControl);
				searchPanel.Dispose();
				searchPanel = null;
			}
		}
		internal void LayoutSearchPanel(Rectangle contentRectangle) {
			Rectangle rect = CalculateSearchPanelBounds(contentRectangle);
			SearchPanel.Bounds = new Rectangle(rect.Location, new Size(SearchPanel.MinWidth, SearchPanel.MinHeight));
		}
		void RecreateLocalizedStrings() {
			if (searchPanel != null)
				searchPanel.SetNullPromptText();
			UpdateAdditionalItemStrings();
			string additionalItem = GetAdditionalItemText();
			if (!string.IsNullOrEmpty(additionalItem) && SearchResultList.ItemCount > 1) {
				BingAddress address = new BingAddress() { FormattedAddress = additionalItem };
				SearchResultList.Items[SearchResultList.ItemCount - 1] = new LocationInformation() { Address = address };
			}
		}
		void UpdateAdditionalItemStrings() {
			showMoreItemsString = MapLocalizer.GetString(MapStringId.SearchPanelShowMoreItems);
			showOnlyItemString = MapLocalizer.GetString(MapStringId.SearchPanelShowOnlyItem);
		}
		string GetAdditionalItemText() { 
			if(IsAlternateResultState)
				return showOnlyItemString;
			else
				if(IsAlternateResultAvailable)
					return showMoreItemsString;
			return string.Empty;
		}
		protected internal virtual void SubscribeSearchPanelEvents() {
			InputEdit.GotFocus += OnInputEditGotFocus;
			InputEdit.LostFocus += OnInputEditLostFocus;
			InputEdit.KeyDown += OnInputEditKeyDown;
			InputEdit.TextChanged += OnInputTextChanged;
			ClearButton.Click += OnClearButtonClick;
			SearchResultList.MouseDoubleClick += OnSearchResultListMouseDoubleClick;
			SearchResultList.MouseClick += OnSearchResultListMouseClick;
		}
		void OnInputEditGotFocus(object sender, EventArgs e) {
			RecreateLocalizedStrings();
		}
		protected internal virtual void UnsubscribeSearchPanelEvents() {
			InputEdit.LostFocus -= OnInputEditLostFocus;
			InputEdit.KeyDown -= OnInputEditKeyDown;
			InputEdit.TextChanged -= OnInputTextChanged;
			ClearButton.Click -= OnClearButtonClick;
			SearchResultList.MouseDoubleClick -= OnSearchResultListMouseDoubleClick;
			SearchResultList.MouseClick -= OnSearchResultListMouseClick;
		}
		protected internal virtual void OnInputEditLostFocus(object sender, EventArgs e) {
			OnCommitChanges();
		}
		protected internal virtual void OnInputEditKeyDown(object sender, KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Escape:
					OnRollbackChanges();
					e.Handled = true;
					break;
				case Keys.Return:
					OnCommitChanges();
					e.Handled = true;
					break;
			}
		}
		protected internal virtual void OnRollbackChanges() {
			if(RollbackChanges != null)
				RollbackChanges(this, EventArgs.Empty);
		}
		protected internal virtual void OnCommitChanges() {
			RaiseCommitChanges();
		}
		protected internal virtual void RaiseCommitChanges() {
			if(CommitChanges != null)
				CommitChanges(this, EventArgs.Empty);
		}
		public virtual void Activate() {
			searchPanel.AddControls(Map.OwnedControl);
			SubscribeSearchPanelEvents();
			InputEdit.Focus();
		}
		public virtual void Deactivate() {
			UnsubscribeSearchPanelEvents();
			searchPanel.RemoveControls(Map.OwnedControl);
		}
		public void UpdateSearchPanel() {
			this.activeInformationLayer = FindSearchPanelLayer();
			bool visible = activeInformationLayer != null;
			UpdateSearchPanelVisibility(visible);
			if(visible)
				Activate();
			else
				Deactivate();
		}
		public void ResetSearchPanel() {
			OnClearButtonClick(this, null);
		}
		public Rectangle CalculateSearchPanelBounds(Rectangle contentBounds) {
			Rectangle panelClientBounds = new Rectangle(contentBounds.Location, new Size(SearchPanel.MinWidth, SearchPanel.MinHeight));
			Rectangle bounds = RectUtils.AlignRectangle(panelClientBounds, contentBounds, ContentAlignment.TopRight);
			bounds.Offset(-(SearchPanelViewInfo.Margin + SearchPanelViewInfo.Padding), SearchPanelViewInfo.Margin + SearchPanelViewInfo.Padding);
			return bounds;
		}
		public void UpdatePanelStyle(ISkinProvider provider) {
			UpdateSearchResultListStyle(provider);
			UpdateInputEditStyle(provider);
			UpdateClearButtonStyle();
		}
	}
}

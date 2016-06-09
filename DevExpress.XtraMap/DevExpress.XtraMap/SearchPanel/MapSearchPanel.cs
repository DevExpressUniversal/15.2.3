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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using System.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraWaitForm;
using DevExpress.Utils;
using DevExpress.Map.Localization;
namespace DevExpress.XtraMap.Native {
	public class SearchPanel : MapDisposableObject {
		internal const int MinWidth = 160;
		internal const int MinHeight = 22;
		const int ListBoxBorderHeight = 2;
		ProgressPanel progressPanel1;
		TextEdit edSearch;
		LabelControl clearButton;
		ListBoxControl lbSearchResult;
		ToolTipController toolTipController;
		bool isBusy;
		bool areAlternateRequestsListed;
		bool visible;
		Rectangle bounds;
		public bool AreAlternateRequestsListed { get { return areAlternateRequestsListed; } }
		public object SelectedResult { get { return lbSearchResult != null && lbSearchResult.SelectedItem != null ? lbSearchResult.SelectedItem : null; } }
		public bool IsBusy {
			get { return isBusy; }
			set {
				if(isBusy == value)
					return;
				isBusy = value;
				OnBusyChanged();
			}
		}
		public TextEdit InputEdit { get { return edSearch; } }
		public LabelControl ClearButton { get { return clearButton; } }
		public ListBoxControl SearchResultList { get { return lbSearchResult; } }
		public ProgressPanel ProgressPanel { get { return progressPanel1; } }
		public Color BackColor { get; set; }
		public bool Visible {
			get { return visible; }
			set {
				if(visible == value)
					return;
				visible = value;
				OnVisibleChanged();
			}
		}
		public Rectangle Bounds {
			get { return bounds; }
			set {
				if(bounds == value)
					return;
				bounds = value;
				UpdateControlBounds(bounds);
			}
		}
		public SearchPanel(ToolTipController toolTipController) {
			CreateControls();
			InitializeControls();
			UpdateState();
			this.toolTipController = toolTipController;
		}
		#region InitializeControls
		void InitializeControls() {
			this.edSearch.Controls.Add(this.clearButton);
			this.edSearch.Name = "edSearch";
			this.edSearch.TabIndex = 14;
			this.edSearch.MinimumSize = new Size(MinWidth, MinHeight);
			SetNullPromptText();
			this.edSearch.BringToFront();
			this.clearButton.Appearance.Image = MapUtils.GetResourceImage("clearButton.png");
			this.clearButton.Appearance.ImageAlign = ContentAlignment.MiddleCenter;
			this.clearButton.Margin = new Padding(0);
			this.clearButton.Name = "clearButton";
			this.clearButton.Padding = new Padding(0, 2, 2, 2);
			this.clearButton.Margin = new Padding(0, 2, 2, 2);
			this.clearButton.Size = new Size(24, 18);
			this.clearButton.MinimumSize = new Size(24, 18);
			this.clearButton.Location = new Point(MinWidth - this.clearButton.Width - 1, 1);
			this.clearButton.TabIndex = 16;
			this.clearButton.BringToFront();
			this.lbSearchResult.Location = new System.Drawing.Point(5, 30);
			this.lbSearchResult.Name = "lbSearchResult";
			this.lbSearchResult.Size = new System.Drawing.Size(250, 89);
			this.lbSearchResult.MinimumSize = new Size(MinWidth, 10);
			this.lbSearchResult.TabIndex = 7;
			this.lbSearchResult.HorizontalScrollbar = false;
			this.lbSearchResult.MouseMove += listBoxControl_MouseMove;
			this.lbSearchResult.MouseLeave += listBoxControl_MouseLeave;
		}
		#endregion
		void listBoxControl_MouseMove(object sender, MouseEventArgs e) {
			ListBoxControl listBoxControl = sender as ListBoxControl;
			int index = listBoxControl.IndexFromPoint(new Point(e.X, e.Y));
			if(index != -1) {
				LocationInformation item = listBoxControl.GetItem(index) as LocationInformation;
				if(item != null)
					toolTipController.ShowHint(item.ToString(), listBoxControl.PointToScreen(new Point(e.X, e.Y)));
			}
			else 
				toolTipController.HideHint();
		}
		void listBoxControl_MouseLeave(object sender, EventArgs e) {
			toolTipController.HideHint();
		}
		void UpdateControlBounds(Rectangle bounds) {
			Rectangle rect = bounds;
			edSearch.Bounds = bounds;
			rect.Height = GetResultListHeight();
			rect.Offset(0, bounds.Height + ListBoxBorderHeight);
			lbSearchResult.Bounds = rect;
		}
		void CreateControls() {
			this.edSearch = new TextEdit();
			this.lbSearchResult = new ListBoxControl();
			this.clearButton = new LabelControl();
		}
		void OnVisibleChanged() {
			edSearch.Visible = Visible;
			lbSearchResult.Visible = (String.IsNullOrEmpty(InputEdit.Text)) ? false : Visible;
		}
		void OnBusyChanged() {
			if (IsBusy)
				ShowProgressPanel();
			else
				HideProgressPanel();
			UpdateState();
		}
		void HideProgressPanel(){
			if (progressPanel1 != null){
				this.edSearch.Controls.Remove(this.progressPanel1);
				this.progressPanel1.Dispose();
				this.progressPanel1 = null;
			}
		}
		void ShowProgressPanel(){
			this.progressPanel1 = new DevExpress.XtraWaitForm.ProgressPanel();
			this.edSearch.Controls.Add(this.progressPanel1);
			this.progressPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.progressPanel1.MinimumSize = new Size(24, 18);
			this.progressPanel1.Name = "progressPanel1";
			this.progressPanel1.Padding = new Padding(0, 2, 2, 2);
			this.progressPanel1.Size = new System.Drawing.Size(24, 18);
			this.progressPanel1.Location = new Point(MinWidth - this.progressPanel1.Width - 1, 1);
			this.progressPanel1.TabIndex = 15;
			this.progressPanel1.BorderStyle = XtraEditors.Controls.BorderStyles.NoBorder;
			this.progressPanel1.BringToFront();
			this.progressPanel1.Appearance.BackColor = edSearch.BackColor;
		}
		void UpdateState() {
			clearButton.Visible = !IsBusy && !String.IsNullOrEmpty(InputEdit.Text);
			UpdateResultListVisibility();
		}
		void UpdateResultListVisibility() {
			if(SearchResultList.Items.Count == 0 || IsBusy)
				SearchResultList.Visible = false;
			else {
				SearchResultList.SelectedIndex = 0;
				SearchResultList.Visible = true;
			}
		}
		internal void RemoveControls(IMapControl mapControl) {
			mapControl.RemoveChildControl(edSearch);
			mapControl.RemoveChildControl(lbSearchResult);
		}
		protected override void DisposeOverride() {
			base.DisposeOverride();
			HideProgressPanel();
			this.lbSearchResult.Dispose();
			this.edSearch.Dispose();
			this.clearButton.Dispose();
		}
		internal void AddControls(IMapControl mapControl) {
			mapControl.AddChildControl(edSearch);
			mapControl.AddChildControl(lbSearchResult);
		}
		internal int GetResultListHeight() {
			return lbSearchResult.ItemCount * lbSearchResult.GetItemRectangle(0).Height + ListBoxBorderHeight;
		}
		internal void SetNullPromptText() {
			edSearch.Properties.NullValuePrompt = MapLocalizer.GetString(MapStringId.SearchPanelNullText);
			edSearch.Properties.NullValuePromptShowForEmptyValue = true;
		}
		public void UpdateVisualState(bool visible) {
			if(!visible) {
				areAlternateRequestsListed = false;
				UpdateState();
			}
		}
		public void UpdateAlternateRequestsListed() {
			areAlternateRequestsListed = !areAlternateRequestsListed;
		}
		public void Reset() {
			if(InputEdit != null)
				InputEdit.Text = "";
			if(SearchResultList != null)
				SearchResultList.Items.Clear();
			UpdateVisualState(false);
		}
	}
}

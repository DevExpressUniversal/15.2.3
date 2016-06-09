#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
namespace DevExpress.ExpressApp.Win.Templates {
	[ToolboxItem(false)]
	public partial class LookupControlTemplate : UserControl, IFrameTemplate, ILookupPopupFrameTemplate, IViewSiteTemplate, ISupportUpdate, ISupportViewChanged, IButtonsContainersOwner {
		public const int DefaultPreferredWidth = 400;
		public const int DefaultPreferredHeight = 300;
		public const string SearchFilterName = "search filter";
		private bool showButtonsContainersPanel = true;
		private const int minWidth = 400;
		private const int minHeight = 300;
		private DevExpress.ExpressApp.ListView listView;
		private bool isSearchEnabled;
		private Size preferredSize;
		private void searchActionContainer_ActionItemAdded(object sender, ActionItemEventArgs e) {
			e.Item.LayoutItem.TextVisible = false;
			e.Item.Action.Executed += new EventHandler<ActionBaseEventArgs>(ParametrizedAction_Executed);
			typeAndFindPanel.Changed -= TypeAndFindPanel_Changed;
			typeAndFindPanel.Changed += TypeAndFindPanel_Changed;
		}
		private void ParametrizedAction_Executed(object sender, ActionBaseEventArgs e) {
			if(listView != null && listView.CollectionSource.List != null && listView.CollectionSource.List.Count > 0) {
				FocusListViewEditor();
			}
		}
		private void TypeAndFindPanel_Changed(object sender, EventArgs e) {
			typeAndFindPanel.MaximumSize = new Size(0, typeAndFindPanel.Root.MinSize.Height + 4);
		}
		private void RefreshSize() {
			if(listView != null && listView.IsControlCreated) {
				int viewControlMinWidth = viewSitePanel.Controls[0].MinimumSize.Width;
				int viewControlMinHeight = viewSitePanel.Controls[0].MinimumSize.Height;
				int searchPanelMinWidth = isSearchEnabled ? ((IXtraResizableControl)typeAndFindPanel).MinSize.Width + viewSiteInfoPanel.Padding.Horizontal : 0;
				int searchPanelMinHeight = isSearchEnabled ? viewSiteInfoPanel.Height + 20 : 0;
				int contentMinWidth = Math.Max(viewControlMinWidth, searchPanelMinWidth);
				int contentMinHeight = Math.Max(viewControlMinHeight, searchPanelMinHeight);
				if(preferredSize.IsEmpty) {
					preferredSize = new Size(
						Math.Max(viewControlMinWidth, DefaultPreferredWidth),
						Math.Max(viewControlMinHeight, DefaultPreferredHeight) + (showButtonsContainersPanel ? buttonsContainersPanel.Height : 0));
				}
				MinimumSize = new Size(
					Math.Max(contentMinWidth, minWidth),
					Math.Max(contentMinHeight + (showButtonsContainersPanel ? buttonsContainersPanel.Height : 0), minHeight));
			}
		}
		private void FocusListViewEditor() {
			Control listViewEditor = GetListViewEditor();
			if(listViewEditor != null) {
				listViewEditor.Focus();
			}
		}
		private Control GetListViewEditor() {
			if(listView != null && listView.Editor != null && listView.Editor.Control != null) {
				return listView.Editor.Control as Control;
			}
			return null;
		}
		private bool ContainsActiveAction(IActionContainer actionContainer) {
			foreach(ActionBase action in actionContainer.Actions) {
				if(action.Active) {
					return true;
				}
			}
			return false;
		}
		protected override void OnSizeChanged(EventArgs e) {
			if(Visible && (FindForm() != null)) {
				SetPreferredSize(Size);
			}
			base.OnSizeChanged(e);
		}
		public LookupControlTemplate() {
			InitializeComponent();
			IsSearchEnabled = false;
		}
		private ButtonEdit SearchActionContainerEditor {
			get {
				ButtonEdit editor = null;
				if(SearchActionContainer.Actions.Count > 0) {
					ButtonsContainersActionItemBase actionItem = SearchActionContainer.ActionItems[SearchActionContainer.Actions[0]];
					editor = (ButtonEdit)actionItem.Control;
				}
				return editor;
			}
		}
		public override Size GetPreferredSize(Size proposedSize) {
			return preferredSize;
		}
		public void SetView(DevExpress.ExpressApp.View view) {
			viewSiteManager.SetView(view);
			if(view != null) {
				listView = (DevExpress.ExpressApp.ListView)view;
				typeValueLabel.Text = view.Caption;
				if(!isSearchEnabled) {
					FocusListViewEditor();
				}
				RefreshSize();
			}
			else {
				listView = null;
			}
			if(ViewChanged != null) {
				ViewChanged(this, new TemplateViewChangedEventArgs(view));
			}
		}
		public void SetPreferredSize(Size preferredSize) {
			this.preferredSize = preferredSize;
			if(PreferredSizeChanged != null) {
				PreferredSizeChanged(this, EventArgs.Empty);
			}
		}
		public ICollection<IActionContainer> GetContainers() {
			return actionContainersManager.GetContainers();
		}
		public bool IsButtonsContainersPanelEmpty() {
			return !ContainsActiveAction(popupActionsContainer) && !ContainsActiveAction(objectsCreationContainer) && !ContainsActiveAction(diagnosticContainer);
		}
		public void BeginUpdate() { }
		public void EndUpdate() { }
		public void SetStartSearchString(string searchString) {
			if(IsSearchEnabled && SearchActionContainerEditor != null) {
				SearchActionContainerEditor.Text = searchString;
				if(!String.IsNullOrEmpty(SearchActionContainerEditor.Text)) {
					SearchActionContainerEditor.Select(SearchActionContainerEditor.Text.Length, 0);
				}
			}
			else {
				listView.Editor.StartIncrementalSearch(searchString);
			}
		}
		public IActionContainer DefaultContainer {
			get { return actionContainersManager.DefaultContainer; }
		}
		public bool IsSearchEnabled {
			get { return isSearchEnabled; }
			set {
				isSearchEnabled = value;
				viewSiteInfoPanel.Visible = value;
				bottomSeparatorPanel.Visible = value;
				viewSitePanel.DockPadding.All = isSearchEnabled ? 12 : 0;
				RefreshSize();
			}
		}
		public DevExpress.ExpressApp.ListView ListView {
			get { return listView; }
		}
		public ButtonEdit FindEditor {
			get { return SearchActionContainerEditor; }
		}
		public Control BottomSeparator {
			get { return bottomSeparatorPanel; }
		}
		public LayoutControl ButtonsContainersPanel {
			get { return buttonsContainersPanel; }
		}
		public object ViewSiteControl {
			get { return viewSiteManager.ViewSiteControl; }
		}
		public bool ShowButtonsContainersPanel {
			get { return showButtonsContainersPanel; }
			set {
				if(showButtonsContainersPanel != value) {
					if(value) {
						if(!Controls.Contains(ButtonsContainersPanel)) {
							Controls.Add(ButtonsContainersPanel);
						}
					}
					else {
						if(Controls.Contains(ButtonsContainersPanel)) {
							Controls.Remove(ButtonsContainersPanel);
						}
					}
					showButtonsContainersPanel = value;
				}
			}
		}
		public ButtonsContainer DiagnosticContainer {
			get { return diagnosticContainer; }
		}
		public ButtonsContainer ObjectsCreationContainer {
			get { return objectsCreationContainer; }
		}
		public ButtonsContainer PopupActionsContainer {
			get { return popupActionsContainer; }
		}
		public ButtonsContainer SearchActionContainer {
			get { return searchActionContainer; }
		}
		public event EventHandler<EventArgs> PreferredSizeChanged;
		public event EventHandler<TemplateViewChangedEventArgs> ViewChanged;
		public void FocusFindEditor() {
			if(IsSearchEnabled && SearchActionContainerEditor != null) {
				Form form = FindForm();
				if(form != null) {
					form.ActiveControl = SearchActionContainerEditor;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				typeAndFindPanel.Changed -= TypeAndFindPanel_Changed;
			}
			base.Dispose(disposing);
			if(disposing) {
				buttonsContainersPanel.Dispose();
			}
		}
	}
}

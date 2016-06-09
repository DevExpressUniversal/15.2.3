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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Mdi;
using DevExpress.Utils.Text;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public enum ItemSortMode { Default, Alphabetical, Custom }
	public class DocumentSelectorAdornerElementInfoArgs : AdornerElementInfoArgs, IDocumentSelector, IStringImageProvider {
		DocumentManager ownerCore;
		public DocumentManager Owner {
			get { return ownerCore; }
		}
		public BaseView View {
			get { return Owner.View; }
		}
		public AppearanceObject Normal { get; set; }
		public AppearanceObject Hot { get; set; }
		public AppearanceObject Selected { get; set; }
		public DocumentSelectorItemsListInfo Panels { get; private set; }
		public DocumentSelectorItemsListInfo Documents { get; private set; }
		public DocumentSelectorHeaderInfo Header { get; private set; }
		public DocumentSelectorFooterInfo Footer { get; private set; }
		public Point RenderOrigin { get; private set; }
		public DocumentSelectorPreviewInfo PreviewInfo { get; private set; }
		public Size MaxPreviewSize { get; set; }
		public Rectangle Client { get; private set; }
		public Rectangle Content { get; private set; }
		public int MaxRowCount { get; set; }
		public int MaxColumnCount { get; set; }
		public DocumentSelectorOpaquePainter Painter { get; private set; }
		public DocumentSelectorAdornerElementInfoArgs(DocumentManager owner) {
			Painter = new DocumentSelectorOpaquePainter();
			ownerCore = owner;
			PreviewInfo = new DocumentSelectorPreviewInfo((Image)null);
			Normal = new FrozenAppearance();
			Hot = new FrozenAppearance();
			Selected = new FrozenAppearance();
			Normal.ForeColor = SystemColors.ControlText;
			Hot.BackColor = SystemColors.HotTrack;
			Selected.BackColor = SystemColors.Highlight;
			MaxPreviewSize = new Size(160, 320);
			MaxColumnCount = 2;
			MaxRowCount = 15;
			previewCache = new Dictionary<IntPtr, DocumentSelectorPreviewInfo>();
		}
		public bool IsOwnControl(Control control) {
			Control parent = Owner.GetContainer();
			while(control != null) {
				if(control == parent)
					return true;
				if(control.Parent == null) {
					if(control is Docking.FloatForm || control is FloatDocumentForm)
						return true;
					BaseDocument document;
					return Owner.View.FloatDocuments.TryGetValue(control, out document);
				}
				control = control.Parent;
			}
			return false;
		}
		bool IDocumentSelector.ShowMenu() {
			return (View != null) && View.IsFocused && View.ShowDocumentSelectorMenu();
		}
		IList<DockPanel> visibleDockPanelsCore;
		public IList<DockPanel> VisibleDockPanels {
			get { return visibleDockPanelsCore; }
		}
		IList<BaseDocument> visibleDocumentsCore;
		public IList<BaseDocument> VisibleDocuments {
			get { return visibleDocumentsCore; }
		}
		protected internal void CreateLists() {
			if(View != null) {
				BuildVisibleDockPanels();
				BuildVisibleDocuments();
				SortItems();
				List<DocumentSelectorItem> documents = new List<DocumentSelectorItem>();
				List<DocumentSelectorItem> panels = new List<DocumentSelectorItem>();
				foreach(DockPanel panel in VisibleDockPanels)
					AddDockPanel(panels, panel);
				foreach(BaseDocument document in VisibleDocuments)
					AddDocument(documents, document);
				Panels = new DocumentSelectorItemsListInfo(panels, 1, this);
				Panels.MaxRowCount = MaxRowCount;
				Documents = new DocumentSelectorItemsListInfo(documents, MaxColumnCount, this);
				Documents.MaxRowCount = MaxRowCount;
			}
			SetSelectItem(false);
		}
		void SortItems() {
			DocumentSelectorCustomSortItemsEventArgs ea = null;
			if(View.DocumentSelectorProperties.CanUseCustomComparer)
				ea = View.RaiseDocumentSelectorCustomSortItems();
			if(View.DocumentSelectorProperties.CanUseAlphabeticalComparer)
				ea = new DocumentSelectorCustomSortItemsEventArgs();
			if(ea != null && visibleDockPanelsCore != null)
				(visibleDockPanelsCore as List<DockPanel>).Sort(ea.DockPanelComparer);
			if(ea != null && visibleDocumentsCore != null)
				(visibleDocumentsCore as List<BaseDocument>).Sort(ea.DocumentComparer);
		}
		void AddDockPanel(List<DocumentSelectorItem> panels, DockPanel panel) {
			DocumentSelectorItem item = new DocumentSelectorItem(panel);
			AssingItemFormats(item, true);
			panels.Add(item);
		}
		void AddDocument(List<DocumentSelectorItem> documents, BaseDocument document) {
			DocumentSelectorItem item = new DocumentSelectorItem(document);
			AssingItemFormats(item, false);
			documents.Add(item);
		}
		void AssingItemFormats(DocumentSelectorItem item, bool isPanel) {
			IBaseDocumentSelectorProperties properties = View.DocumentSelectorProperties;
			item.CaptionFormat = properties.ItemCaptionFormat;
			item.HeaderFormat = isPanel ? properties.PanelHeaderFormat : properties.DocumentHeaderFormat;
			item.FooterFormat = isPanel ? properties.PanelFooterFormat : properties.DocumentFooterFormat;
		}
		void BuildVisibleDockPanels() {
			visibleDockPanelsCore = new List<DockPanel>();
			if(Owner.DockManager == null) return;
			foreach(DockPanel panel in Owner.ActivationInfo.PanelActivationList) {
				if(CanAddDockPanel(panel) && !panel.IsDisposed) {
					VisibleDockPanels.Add(panel);
				}
			}
			foreach(DockPanel panel in Owner.DockManager.Panels) {
				if(CanAddDockPanel(panel) && !VisibleDockPanels.Contains(panel)) {
					VisibleDockPanels.Add(panel);
				}
			}
		}
		protected bool CanAddDockPanel(DockPanel panel) {
			bool result = panel.Visibility != DockVisibility.Hidden && !panel.IsMdiDocument && panel.Count <= 1;
			if(panel.RootPanel != null)
				result &= panel.RootPanel.Visibility != DockVisibility.Hidden;
			return result;
		}
		void BuildVisibleDocuments() {
			visibleDocumentsCore = new List<BaseDocument>();
			foreach(BaseDocument document in Owner.ActivationInfo.DocumentActivationList) {
				if(!document.IsVisible || document.IsContainer) continue;
				Views.Tabbed.Document tabbedDocument = document as Views.Tabbed.Document;
				if(tabbedDocument != null) {
					if(tabbedDocument.Parent != null && !tabbedDocument.Parent.Visible) continue;
					if(!tabbedDocument.Properties.CanShowInDocumentSelector) continue;
				}
				VisibleDocuments.Add(document);
			}
		}
		public void Calc(GraphicsCache cache, DocumentSelectorOpaquePainter painter) {
			DocumentSelectorItemsListInfo activeList;
			DocumentSelectorItemsListInfo inactiveList;
			GetCurrentSelectedList(out activeList, out inactiveList);
			if(activeList.SelectItemIndex == -1)
				activeList.SelectItemIndex = 0;
			DocumentSelectorItemInfo activeItemInfo = activeList.ItemsInfo[activeList.SelectItemIndex];
			Header = new DocumentSelectorHeaderInfo(GetFormattedText(activeItemInfo, true), activeItemInfo.Image, this);
			Footer = new DocumentSelectorFooterInfo(GetFormattedText(activeItemInfo, false), this);
			Header.AllowGlyphSkinning = activeItemInfo.GetAllowGlyphSkinning();
			Rectangle tmpBounds = painter.CalcBoundsByClientRectangle(this, new Rectangle(0, 0, 1000, 1000));
			Padding contentMargin = new Padding(0 - tmpBounds.Left, 0 - tmpBounds.Top, tmpBounds.Right - 1000, tmpBounds.Bottom - 1000);
			DocumentSelectorBackgroundPainter bgPainter = GetDocumentSelectorBackgroundPainter() as DocumentSelectorBackgroundPainter;
			Padding additionalContentMargin = bgPainter.GetAdditionalContentMargin();
			DocumentSelectorHeaderPainter headerPainter = GetDocumentSelectorHeaderPainter() as DocumentSelectorHeaderPainter;
			int headerHeight = Header.CalcMinHeight(cache, headerPainter);
			CalculateListBounds(cache, Panels, new Point(contentMargin.Left + additionalContentMargin.Left, contentMargin.Top + headerHeight + additionalContentMargin.Top));
			CalculateListBounds(cache, Documents, new Point(contentMargin.Left + additionalContentMargin.Left + Panels.Bounds.Width, contentMargin.Top + headerHeight + additionalContentMargin.Top));
			MaxPreviewSize = GetPreviewSize();
			CalculatePreviewBounds(new Point(contentMargin.Left + additionalContentMargin.Left + Panels.Bounds.Width + Documents.Bounds.Width, contentMargin.Top + headerHeight + additionalContentMargin.Top));
			Size content = new Size(Documents.Bounds.Width + Panels.Bounds.Width + MaxPreviewSize.Width + additionalContentMargin.Horizontal, MaxPreviewSize.Height + additionalContentMargin.Vertical);
			DocumentSelectorFooterPainter footerPainter = GetDocumentSelectorFooterPainter() as DocumentSelectorFooterPainter;
			Header.Calc(cache, headerPainter, new Point(contentMargin.Left, contentMargin.Top), content.Width);
			Footer.Calc(cache, footerPainter, new Point(contentMargin.Left, contentMargin.Top + headerHeight + content.Height), content.Width);
			Bounds = new Rectangle(0, 0, content.Width + contentMargin.Horizontal, Header.Bounds.Height + content.Height + Footer.Bounds.Height + contentMargin.Vertical);
			Client = new Rectangle(contentMargin.Left, contentMargin.Top, Bounds.Width - contentMargin.Horizontal, Bounds.Height - contentMargin.Vertical);
			Content = new Rectangle(contentMargin.Left, contentMargin.Top + Header.Bounds.Height, content.Width, content.Height);
			Rectangle dockingRect = (Owner != null) ? Owner.GetDockingRect() : Rectangle.Empty;
			if(Owner != null && Owner.IsRightToLeftLayout())
				dockingRect = new Rectangle(Owner.Bounds.Left - (dockingRect.Right - Owner.Bounds.Right), dockingRect.Top, dockingRect.Width, dockingRect.Height);
			RenderOrigin = new Point(dockingRect.Left + (dockingRect.Width - Bounds.Width) / 2,
				dockingRect.Top + (dockingRect.Height - Bounds.Height) / 2);
		}
		string GetFormattedText(DocumentSelectorItemInfo activeItemInfo, bool isHeader) {
			if(View == null && View.DocumentSelectorProperties == null) return activeItemInfo.Text;
			string format = isHeader ? activeItemInfo.HeaderFormat : activeItemInfo.FooterFormat;
			string text = isHeader ? activeItemInfo.Header : activeItemInfo.Footer;
			return string.Format(format, text, activeItemInfo.Text);
		}
		protected override int CalcCore() {
			using(IMeasureContext context = View.BeginMeasure()) {
				using(GraphicsCache cache = new GraphicsCache(context.Graphics)) {
					Calc(cache, Painter);
					return CalcState(Documents.SelectItemIndex + Panels.SelectItemIndex, Documents.HotItemIndex, Documents.TopRowIndex);
				}
			}
		}
		bool IsShowPreview {
			get {
				if(View.DocumentSelectorProperties != null)
					return View.DocumentSelectorProperties.ShowPreview;
				return true;
			}
		}
		Size GetPreviewSize() {
			int width = 0;
			if(IsShowPreview)
				width = MaxPreviewSize.Width;
			return new Size(width, Math.Max(Documents.Bounds.Height, Panels.Bounds.Height));
		}
		void CalculatePreviewBounds(Point previewOffset) {
			if(!IsShowPreview) return;
			bool hasPaneListSelected = (Documents.SelectItemIndex < 0);
			Control target = GetTargetControl(hasPaneListSelected);
			DocumentSelectorPreviewInfo info;
			if(target == null) {
				PreviewInfo = NoPreviewAvailableInfo;
			}
			else if(!previewCache.TryGetValue(target.Handle, out info)) {
				PreviewInfo = new DocumentSelectorPreviewInfo(MDIChildPreview.Create(target, Color.Magenta));
				CalculatePreviewSize(previewOffset);
				previewCache.Add(target.Handle, new DocumentSelectorPreviewInfo(PreviewInfo));
				return;
			}
			else PreviewInfo = info;
			CalculatePreviewSize(previewOffset);
		}
		Control GetTargetControl(bool hasPaneListSelected) {
			if(hasPaneListSelected)
				return  (Control)VisibleDockPanels[Panels.SelectItemIndex];
			BaseDocument document = VisibleDocuments[Documents.SelectItemIndex];
			Control target = document.IsControlLoaded ? Owner.GetChild(document) : null;
			return target ?? document.Form;
		}
		DocumentSelectorPreviewInfo noPreviewAvailableInfoCore;
		DocumentSelectorPreviewInfo NoPreviewAvailableInfo {
			get {
				if(noPreviewAvailableInfoCore == null)
					noPreviewAvailableInfoCore = new NoPreviewAvailableInfo();
				return noPreviewAvailableInfoCore;
			}
		}
		void CalculatePreviewSize(Point previewOffset) {
			if(PreviewInfo.PreviewImage == null) {
				PreviewInfo.RealPreviewSize = Size.Empty;
				PreviewInfo.PreviewBounds = Rectangle.Empty;
				return;
			}
			if(PreviewInfo.PreviewImage.Width > MaxPreviewSize.Width * 2 && PreviewInfo.PreviewImage.Height > MaxPreviewSize.Height * 2) {
				PreviewInfo.RealPreviewSize = new Size(MaxPreviewSize.Width * 2, MaxPreviewSize.Height * 2);
				PreviewInfo.PreviewBounds = new Rectangle(previewOffset, MaxPreviewSize);
				return;
			}
			if(PreviewInfo.PreviewImage.Width > MaxPreviewSize.Width && PreviewInfo.PreviewImage.Height > MaxPreviewSize.Height) {
				PreviewInfo.RealPreviewSize = MaxPreviewSize;
				PreviewInfo.PreviewBounds = new Rectangle(previewOffset, MaxPreviewSize);
				return;
			}
			if(PreviewInfo.PreviewImage.Width > MaxPreviewSize.Width) {
				PreviewInfo.RealPreviewSize = new Size(MaxPreviewSize.Width, PreviewInfo.PreviewImage.Height);
				PreviewInfo.PreviewBounds = new Rectangle(previewOffset, PreviewInfo.RealPreviewSize);
			}
			else if(PreviewInfo.PreviewImage.Height > MaxPreviewSize.Height) {
				PreviewInfo.RealPreviewSize = new Size(PreviewInfo.PreviewImage.Width, MaxPreviewSize.Height);
				PreviewInfo.PreviewBounds = new Rectangle(previewOffset, PreviewInfo.RealPreviewSize);
			}
			else {
				PreviewInfo.RealPreviewSize = new Size(PreviewInfo.PreviewImage.Width, PreviewInfo.PreviewImage.Height);
				PreviewInfo.PreviewBounds = new Rectangle(previewOffset, PreviewInfo.RealPreviewSize);
			}
		}
		void CalculateListBounds(GraphicsCache cache, DocumentSelectorItemsListInfo list, Point offset) {
			list.Calc(cache, (DocumentSelectorItemsListPainter)GetDocumentSelectorItemsListPainter(), offset);
		}
		void SetSelectItem(bool last) {
			int focuseIndex = -1;
			if(VisibleDocuments.Count > 0 && Documents.ItemsInfo.Count > 0) {
				if(last) {
					Documents.SelectItemIndex = VisibleDocuments.Count - 1;
					return;
				}
				if(VisibleDocuments.Count > 1 && Documents.ItemsInfo.Count > 1)
					focuseIndex = 1;
				else focuseIndex = 0;
				Documents.SelectItemIndex = focuseIndex;
				return;
			}
			if(VisibleDockPanels.Count > 0) {
				if(last) {
					Panels.SelectItemIndex = VisibleDockPanels.Count - 1;
					return;
				}
				if(VisibleDockPanels.Count > 1)
					focuseIndex = 1;
				else focuseIndex = 0;
				Panels.SelectItemIndex = focuseIndex;
			}
		}
		public void RaiseShown() {
			if(Owner != null && View != null)
				View.RaiseDocumentSelectorShown();
		}
		public void RaiseHidden() {
			if(Owner != null && View != null)
				View.RaiseDocumentSelectorHidden();
		}
		public void SelectNext() {
			MoveUpDownSelect(true);
		}
		public void SelectPrev() {
			MoveUpDownSelect(false);
		}
		void MoveUpDownSelect(bool next) {
			bool hasPaneListSelected = (Documents.SelectItemIndex < 0);
			if(hasPaneListSelected) ChangeSelection(Panels, Documents, next);
			else ChangeSelection(Documents, Panels, next);
		}
		void ChangeSelection(DocumentSelectorItemsListInfo currentSelect, DocumentSelectorItemsListInfo nonSelect, bool next) {
			int offset = (next) ? 1 : -1;
			if(currentSelect.SelectItemIndex == 0 && !next) {
				currentSelect.SelectItemIndex += offset;
				if(nonSelect.PaintItemInfo.Count == 0)
					nonSelect = currentSelect;
				nonSelect.SelectItemIndex = GetIndexLastItemInfo(nonSelect.PaintItemInfo);
			}
			else if(currentSelect.SelectItemIndex == currentSelect.ItemsInfo.Count - 1 && next) {
				currentSelect.SelectItemIndex = -1;
				if(nonSelect.PaintItemInfo.Count == 0)
					nonSelect = currentSelect;
				nonSelect.SelectItemIndex = GetIndexFirstItemInfo(nonSelect.PaintItemInfo);
			}
			else {
				currentSelect.SelectItemIndex += offset;
				CorrectionTopRowIndex(currentSelect);
			}
		}
		public bool SetNextDocument(bool next) {
			if(View.CanUseDocumentSelector() && View.UseDocumentSelector == DefaultBoolean.False) {
				View.SetNextDocument(next);
				return true;
			}
			return false;
		}
		void SelectNextItem(bool next) {
			DocumentSelectorItemsListInfo list = (Documents.SelectItemIndex > -1) ? Documents : Panels;
			int offset = (next) ? 1 : -1;
			if(list.SelectItemIndex == 0 && !next) {
				list.SelectItemIndex = GetIndexLastItemInfo(list.PaintItemInfo);
			}
			else if(list.SelectItemIndex == list.ItemsInfo.Count - 1 && next) {
				list.SelectItemIndex = GetIndexFirstItemInfo(list.PaintItemInfo);
			}
			else {
				list.SelectItemIndex += offset;
				CorrectionTopRowIndex(list);
			}
		}
		internal void CorrectionTopRowIndex(DocumentSelectorItemsListInfo list) {
			list.TopRowIndex = (list.TopRowIndex > list.SelectItemIndex) ? list.SelectItemIndex : GetIndexFirstItemInfo(list.PaintItemInfo);
			list.TopRowIndex += (list.SelectItemIndex > GetIndexLastItemInfo(list.PaintItemInfo)) ? list.SelectItemIndex - GetIndexLastItemInfo(list.PaintItemInfo) : 0;
		}
		internal void CorrectSelectedIndex(DocumentSelectorItemsListInfo list) {
			if(list.SelectItemIndex > GetIndexLastItemInfo(list.PaintItemInfo))
				list.SelectItemIndex = GetIndexLastItemInfo(list.PaintItemInfo);
			if(list.SelectItemIndex < list.TopRowIndex)
				list.SelectItemIndex = list.TopRowIndex;
		}
		int GetIndexFirstItemInfo(IList<DocumentSelectorItemInfo> list) {
			return (list[0] is DocumentSelectorScrollItemInfo) ? list[1].Index : list[0].Index;
		}
		int GetIndexLastItemInfo(IList<DocumentSelectorItemInfo> list) {
			return (list[list.Count - 1] is DocumentSelectorScrollItemInfo) ? list[list.Count - 2].Index : list[list.Count - 1].Index;
		}
		bool SearchNewSelectionItemIndex(Point pointInNewSelection, DocumentSelectorItemsListInfo listInfo, out int index) {
			foreach(DocumentSelectorItemInfo item in listInfo.PaintItemInfo) {
				if(item.Bounds.Contains(pointInNewSelection)) {
					if(item is DocumentSelectorScrollUpItemInfo) {
						index = listInfo.PaintItemInfo[1].Index;
						return true;
					}
					else if(item is DocumentSelectorScrollDownItemInfo) {
						index = listInfo.PaintItemInfo[listInfo.PaintItemInfo.Count - 2].Index;
						return true;
					}
					else {
						index = item.Index;
						return true;
					}
				}
			}
			index = -1;
			return false;
		}
		protected DocumentSelectorItemInfo GetItemAtPoint(Point point, DocumentSelectorItemsListInfo listInfo) {
			foreach(DocumentSelectorItemInfo item in listInfo.PaintItemInfo) {
				if(item.Bounds.Contains(point))
					return item;
			}
			return null;
		}
		Rectangle GetSelectedItemBounds(DocumentSelectorItemsListInfo list) {
			foreach(DocumentSelectorItemInfo item in list.PaintItemInfo) {
				if(item.Index == list.SelectItemIndex)
					return item.Bounds;
			}
			return Rectangle.Empty;
		}
		public void MoveLeftSelection() {
			MoveLeftRightSelect(false);
		}
		public void MoveRightSelection() {
			MoveLeftRightSelect(true);
		}
		void MoveLeftRightSelect(bool next) {
			Point pointInNewSelection;
			DocumentSelectorItemsListInfo currentSelect;
			DocumentSelectorItemsListInfo nonSelect;
			GetCurrentSelectedList(out currentSelect, out nonSelect);
			int leftBorder;
			int rightBorder;
			if(nonSelect.PaintItemInfo.Count == 0 && currentSelect.PaintItemInfo.Count != 0)
				nonSelect = currentSelect;
			GetBorders(currentSelect, nonSelect, out leftBorder, out rightBorder);
			pointInNewSelection = GetPointForNextSelection(next, currentSelect);
			if(pointInNewSelection.X < leftBorder) {
				pointInNewSelection.X = rightBorder - 20;
			}
			if(pointInNewSelection.X > rightBorder) {
				pointInNewSelection.X = leftBorder + 20;
			}
			MoveSelection(pointInNewSelection, currentSelect, nonSelect);
		}
		void GetBorders(DocumentSelectorItemsListInfo currentSelect, DocumentSelectorItemsListInfo nonSelect, out int leftBorder, out int rightBorder) {
			leftBorder = Math.Min(currentSelect.PaintItemInfo[0].Bounds.X, nonSelect.PaintItemInfo[0].Bounds.X);
			rightBorder = Math.Max(currentSelect.PaintItemInfo[currentSelect.PaintItemInfo.Count - 1].Bounds.X + currentSelect.PaintItemInfo[currentSelect.PaintItemInfo.Count - 1].Bounds.Width,
				nonSelect.PaintItemInfo[nonSelect.PaintItemInfo.Count - 1].Bounds.X + nonSelect.PaintItemInfo[nonSelect.PaintItemInfo.Count - 1].Bounds.Width);
		}
		void GetCurrentSelectedList(out DocumentSelectorItemsListInfo currentSelect, out DocumentSelectorItemsListInfo nonSelect) {
			bool hasPaneListSelected = (Documents.SelectItemIndex < 0);
			if(hasPaneListSelected) {
				currentSelect = Panels;
				nonSelect = Documents;
			}
			else {
				currentSelect = Documents;
				nonSelect = Panels;
			}
		}
		void GetCurrentHotList(out DocumentSelectorItemsListInfo currentHot, out DocumentSelectorItemsListInfo nonHot) {
			bool hasPaneListHot = (Documents.HotItemIndex < 0);
			if(hasPaneListHot) {
				currentHot = Panels;
				nonHot = Documents;
			}
			else {
				currentHot = Documents;
				nonHot = Panels;
			}
		}
		bool MoveSelection(Point point, DocumentSelectorItemsListInfo selectedList, DocumentSelectorItemsListInfo nonSelectedList) {
			return MoveSelection(point, selectedList, nonSelectedList, true);
		}
		bool MoveSelection(Point point, DocumentSelectorItemsListInfo selectedList, DocumentSelectorItemsListInfo nonSelectedList, bool search) {
			int index;
			for(int i = point.Y; i >= selectedList.Bounds.Y; i--) {
				point.Y = i;
				if(SearchNewSelectionItemIndex(point, selectedList, out index)) {
					selectedList.SelectItemIndex = index;
					return true;
				}
				if(SearchNewSelectionItemIndex(point, nonSelectedList, out index)) {
					selectedList.SelectItemIndex = -1;
					nonSelectedList.SelectItemIndex = index;
					return true;
				}
				if(!search) break;
			}
			return false;
		}
		Point GetPointForNextSelection(bool next, DocumentSelectorItemsListInfo SelectListInfo) {
			Rectangle boundsSelectItem = GetSelectedItemBounds(SelectListInfo);
			int offset = (next) ? 20 : -20;
			if(next) return new Point(boundsSelectItem.X + offset + boundsSelectItem.Width, boundsSelectItem.Y + boundsSelectItem.Height / 2);
			else return new Point(boundsSelectItem.X + offset, boundsSelectItem.Y + boundsSelectItem.Height / 2);
		}
		protected override IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			return !opaque ? new Rectangle[] { } : new Rectangle[] { new Rectangle(RenderOrigin, Bounds.Size) };
		}
		public static DocumentSelectorAdornerElementInfoArgs EnsureInfoArgs(ref AdornerElementInfo target, DocumentManager owner) {
			DocumentSelectorAdornerElementInfoArgs args;
			if(target == null) {
				args = new DocumentSelectorAdornerElementInfoArgs(owner);
				args.MaxRowCount = 12;
				args.MaxColumnCount = 2;
				target = new AdornerElementInfo(args.Painter, args);
			}
			else args = target.InfoArgs as DocumentSelectorAdornerElementInfoArgs;
			args.SetDirty();
			return args;
		}
		#region IDocumentSelector Members
		IDictionary<IntPtr, DocumentSelectorPreviewInfo> previewCache;
		public bool Show(bool showLast) {
			if(previewCache == null)
				previewCache = new Dictionary<IntPtr, DocumentSelectorPreviewInfo>();
			CreateLists();
			SetSelectItem(showLast);
			View.SetCursor(null);
			SetCapture();
			DocumentSelectorItemsListInfo currentSelect;
			DocumentSelectorItemsListInfo nonSelect;
			GetCurrentSelectedList(out currentSelect, out nonSelect);
			if(currentSelect.ItemsInfo.Count == nonSelect.ItemsInfo.Count && currentSelect.ItemsInfo.Count == 0)
				return false;
			Owner.Adorner.Show(Owner.documentSelectorInfo);
			SetDirty();
			Calc();
			CorrectionTopRowIndex(currentSelect);
			Owner.Adorner.Invalidate();
			return true;
		}
		public void Hide() {
			View.SetCursor(null);
			ReleaseCapture();
			Owner.Adorner.Reset(Owner.documentSelectorInfo);
			Owner.Adorner.Clear();
			Ref.DisposeItemsAndClear(ref previewCache);
			SelectControl();
		}
		void SelectControl() {
			bool hasPaneListSelected = (Documents.SelectItemIndex < 0);
			if(hasPaneListSelected) {
				var nextPanel = GetNextDockPanel();
				if(nextPanel != Owner.DockManager.ActivePanel) {
					Owner.DockManager.ActivePanel = nextPanel;
				}
				else nextPanel.Focus();
			}
			else {
				BaseDocument nextDocument = GetNextDocument();
				if(!View.RaiseNextDocument(View.ActiveDocument, ref nextDocument, true)) {
					if(View.Controller.Activate(nextDocument)) {
						if(!nextDocument.IsFloating && !Owner.IsMdiStrategyInUse) {
							var hostsContext = Owner.GetDocumentsHostContext();
							if(hostsContext == null)
								DocumentsHostContext.CheckHostFormActive(Owner);
						}
					}
				}
			}
		}
		DockPanel GetNextDockPanel() {
			return VisibleDockPanels[Panels.ItemsInfo[Panels.SelectItemIndex].Index];
		}
		BaseDocument GetNextDocument() {
			return VisibleDocuments[Documents.ItemsInfo[Documents.SelectItemIndex].Index];
		}
		public void Cancel() {
			if(Owner == null) return;
			if(Owner.Adorner != null)
				Owner.Adorner.Reset(Owner.documentSelectorInfo);
			if(View != null)
				View.SetCursor(null);
			ReleaseCapture();
			Ref.DisposeItemsAndClear(ref previewCache);
			Ref.Dispose(ref noPreviewAvailableInfoCore);
		}
		void SetCapture() {
			Control control = Owner.GetOwnerControl();
			if(control != null) control.Capture = true;
		}
		void ReleaseCapture() {
			Control control = Owner.GetOwnerControl();
			if(control != null) control.Capture = false;
		}
		public void Next() {
			SelectNextItem(true);
			SetDirty();
			Owner.Adorner.Invalidate();
		}
		public void Prev() {
			SelectNextItem(false);
			SetDirty();
			Owner.Adorner.Invalidate();
		}
		public void MoveLeft() {
			MoveLeftSelection();
			SetDirty();
			Owner.Adorner.Invalidate();
		}
		public void MoveRight() {
			MoveRightSelection();
			SetDirty();
			Owner.Adorner.Invalidate();
		}
		public void MoveUp() {
			SelectPrev();
			SetDirty();
			Owner.Adorner.Invalidate();
		}
		public void MoveDown() {
			SelectNext();
			SetDirty();
			Owner.Adorner.Invalidate();
		}
		public void Hover(Point screenPoint) {
			bool result = false;
			Point offset = Owner.ClientToScreen(RenderOrigin);
			Point point = new Point(screenPoint.X - offset.X, screenPoint.Y - offset.Y);
			int hotState = Documents.HotItemIndex * 1000 + Panels.HotItemIndex;
			DocumentSelectorItemsListInfo currentSelect;
			DocumentSelectorItemsListInfo nonSelect;
			GetCurrentHotList(out currentSelect, out nonSelect);
			DocumentSelectorItemInfo item = GetItemAtPoint(point, currentSelect);
			if(item != null) {
				currentSelect.HotItemIndex = item.Index;
				View.SetCursor(Cursors.Hand);
				result = true;
			}
			item = GetItemAtPoint(point, nonSelect);
			if(item != null) {
				currentSelect.HotItemIndex = -1;
				nonSelect.HotItemIndex = item.Index;
				View.SetCursor(Cursors.Hand);
				result = true;
			}
			if(!result) {
				View.SetCursor(null);
				currentSelect.HotItemIndex = -1;
				nonSelect.HotItemIndex = -1;
			}
			if(hotState != Documents.HotItemIndex * 1000 + Panels.HotItemIndex) {
				SetDirty();
				Owner.Adorner.Invalidate();
			}
		}
		public bool? Click(Point screenPoint) {
			Point offset = Owner.ClientToScreen(RenderOrigin);
			Point point = new Point(screenPoint.X - offset.X, screenPoint.Y - offset.Y);
			if(!Bounds.Contains(point))
				return null;
			bool isScroll = false;
			DocumentSelectorItemsListInfo selectedList;
			DocumentSelectorItemsListInfo nonSelectedList;
			GetCurrentSelectedList(out selectedList, out nonSelectedList);
			int index = selectedList.SelectItemIndex;
			DocumentSelectorItemInfo item = GetItemAtPoint(point, selectedList);
			if(item != null) {
				if(item is DocumentSelectorScrollUpItemInfo) {
					selectedList.TopRowIndex--;
					CalcCore();
					CorrectSelectedIndex(selectedList);
					isScroll = true;
				}
				if(item is DocumentSelectorScrollDownItemInfo) {
					selectedList.TopRowIndex++;
					CalcCore();
					CorrectSelectedIndex(selectedList);
					isScroll = true;
				}
			}
			item = GetItemAtPoint(point, nonSelectedList);
			if(item != null) {
				if(item is DocumentSelectorScrollUpItemInfo) {
					nonSelectedList.TopRowIndex--;
					isScroll = true;
				}
				if(item is DocumentSelectorScrollDownItemInfo) {
					nonSelectedList.TopRowIndex++;
					isScroll = true;
				}
			}
			if(!isScroll) {
				if(MoveSelection(point, selectedList, nonSelectedList, false)) {
					SetDirty();
					Owner.Adorner.Invalidate();
					View.SetCursor(null);
					return true;
				}
				else return false;
			}
			else {
				SetDirty();
				Owner.Adorner.Invalidate();
				return false;
			}
		}
		#endregion
		protected internal ObjectPainter GetDocumentSelectorHeaderPainter() {
			return View.Painter.GetDocumentSelectorHeaderPainter();
		}
		protected internal ObjectPainter GetDocumentSelectorFooterPainter() {
			return View.Painter.GetDocumentSelectorFooterPainter();
		}
		protected internal ObjectPainter GetDocumentSelectorItemsListPainter() {
			return View.Painter.GetDocumentSelectorItemsListPainter();
		}
		protected internal ObjectPainter GetDocumentSelectorPreviewPainter() {
			return View.Painter.GetDocumentSelectorPreviewPainter();
		}
		protected internal ObjectPainter GetDocumentSelectorBackgroundPainter() {
			return View.Painter.GetDocumentSelectorBackgroundPainter();
		}
		public Image GetImage(string id) {
			if(Owner != null && Owner.Images != null) {
				return ImageCollection.GetImageListImage(Owner.Images, id);
			}
			return null;
		}
		#region IBaseAdorner Members
		bool IBaseAdorner.Show() { return false; }
		public AdornerHitTest HitTest(Point point) {
			return AdornerHitTest.None;
		}
		#endregion
	}
	public class DocumentSelectorOpaquePainter : AdornerOpaquePainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DocumentSelectorAdornerElementInfoArgs info = e as DocumentSelectorAdornerElementInfoArgs;
			Rectangle client = new Rectangle(0, 0, info.Client.Width, info.Client.Height);
			using(Bitmap clientImage = new Bitmap(client.Width, client.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
				Point clientOffset = new Point(info.RenderOrigin.X + info.Client.X, info.RenderOrigin.Y + info.Client.Y);
				using(Graphics g = Graphics.FromImage(clientImage)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, client)) {
						bg.Graphics.TranslateTransform(-info.Client.X, -info.Client.Y);
						using(GraphicsCache cache = new GraphicsCache(bg.Graphics)) {
							DrawBackground(cache, info);
							DrawHeader(cache, info);
							DrawFooter(cache, info);
							DrawContent(cache, info);
							DrawPreview(cache, info);
						}
						bg.Render();
					}
				}
				DrawBackground(e.Cache, info, info.RenderOrigin);
				e.Graphics.DrawImageUnscaled(clientImage, clientOffset);
			}
		}
		protected virtual void DrawPreview(GraphicsCache graphicsCache, DocumentSelectorAdornerElementInfoArgs info) {
			ObjectPainter.DrawObject(graphicsCache, info.GetDocumentSelectorPreviewPainter(), info.PreviewInfo);
		}
		protected void DrawHeader(GraphicsCache graphicsCache, DocumentSelectorAdornerElementInfoArgs info) {
			ObjectPainter.DrawObject(graphicsCache, info.GetDocumentSelectorHeaderPainter(), info.Header);
		}
		protected void DrawFooter(GraphicsCache graphicsCache, DocumentSelectorAdornerElementInfoArgs info) {
			ObjectPainter.DrawObject(graphicsCache, info.GetDocumentSelectorFooterPainter(), info.Footer);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			DocumentSelectorAdornerElementInfoArgs info = e as DocumentSelectorAdornerElementInfoArgs;
			return info.GetDocumentSelectorBackgroundPainter().CalcBoundsByClientRectangle(new BackgroundInfoArgs(info.Bounds), client);
		}
		protected void DrawBackground(GraphicsCache cache, DocumentSelectorAdornerElementInfoArgs info) {
			BackgroundInfoArgs args = new BackgroundInfoArgs(info.Bounds, info.Content, info.Client);
			ObjectPainter.DrawObject(cache, info.GetDocumentSelectorBackgroundPainter(), args);
		}
		protected void DrawBackground(GraphicsCache cache, DocumentSelectorAdornerElementInfoArgs info, Point offset) {
			Rectangle relativeBounds = new Rectangle(offset.X + info.Bounds.X, offset.Y + info.Bounds.Y, info.Bounds.Width, info.Bounds.Height);
			ObjectPainter.DrawObject(cache, info.GetDocumentSelectorBackgroundPainter(), new BackgroundInfoArgs(relativeBounds));
		}
		protected void DrawContent(GraphicsCache cache, DocumentSelectorAdornerElementInfoArgs info) {
			ObjectPainter painter = info.GetDocumentSelectorItemsListPainter();
			ObjectPainter.DrawObject(cache, painter, info.Panels);
			ObjectPainter.DrawObject(cache, painter, info.Documents);
		}
	}
	public class DocumentSelectorPreviewInfo : ObjectInfoArgs, IDisposable {
		public void Dispose() {
			DisposeCore();
		}
		protected virtual void DisposeCore() {
			Ref.Dispose(ref imgCore);
		}
		protected void ClearImage(){
			imgCore = null;
		}
		Image imgCore;
		public Image PreviewImage {
			get { return imgCore; }
			set {
				if(imgCore == value) return;
				Ref.Dispose(ref imgCore);
				imgCore = value;
			}
		}
		public Rectangle PreviewBounds { get; set; }
		public Size RealPreviewSize { get; set; }
		public DocumentSelectorPreviewInfo(Image image) {
			PreviewImage = image;
			PreviewBounds = Rectangle.Empty;
			RealPreviewSize = Size.Empty;
		}
		public DocumentSelectorPreviewInfo(DocumentSelectorPreviewInfo info) {
			PreviewImage = info.PreviewImage;
			PreviewBounds = info.PreviewBounds;
			RealPreviewSize = info.RealPreviewSize;
		}
	}
	public class NoPreviewAvailableInfo : DocumentSelectorPreviewInfo, IDisposable {
		public NoPreviewAvailableInfo()
			: base(new Bitmap(160, 320)) {
		}
		protected override void DisposeCore() { ClearImage(); }
		public string Text { get { return DocumentManagerLocalizer.GetString(DocumentManagerStringId.NoPreviewAvailableText); } }
	}
	public class DocumentSelectorPreviewPainter : ObjectPainter {
		static readonly Size DefaultPadding = new Size(-20, -10);
		protected virtual Color GetFadingColor() {
			return SystemColors.Control;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			DocumentSelectorPreviewInfo info = e as DocumentSelectorPreviewInfo;
			DrawPreviewImage(info.Cache, info);
			DrawPreviewBorder(info.Cache, info);
		}
		AppearanceObject paintAppearanceCore;
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearanceCore == null)
					paintAppearanceCore = new FrozenAppearance();
				return paintAppearanceCore;
			}
		}
		protected virtual void DrawPreviewBorder(GraphicsCache cache, DocumentSelectorPreviewInfo info) {
			Color color = GetFadingColor();
			System.Drawing.Imaging.ImageAttributes fadingAttributes = new System.Drawing.Imaging.ImageAttributes();
			fadingAttributes.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(new float[][]{
				new float[] {1, 0, 0, 0, 0},
				new float[] {0, 1, 0, 0, 0},
				new float[] {0, 0, 1, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {(float)color.R/255.0f, (float)color.G/255.0f, (float)color.B/255.0f, 0, 1}	}));
			Image img = Resources.CommonResourceLoader.GetImage("PreviewMask");
			cache.Graphics.DrawImage(img, info.PreviewBounds, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, fadingAttributes);
		}
		protected virtual void DrawPreviewImage(GraphicsCache cache, DocumentSelectorPreviewInfo info) {
			Rectangle srcRect = new Rectangle(Point.Empty, info.RealPreviewSize);
			if(!srcRect.IsEmpty) {
				cache.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				cache.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				cache.Graphics.CompositingQuality = CompositingQuality.HighQuality;
			}
			if(info is NoPreviewAvailableInfo) {
				InitPaintAppearance();
				srcRect = info.PreviewBounds;
				srcRect.Inflate(DefaultPadding);
				Size textSize = Size.Round(PaintAppearance.CalcTextSize(cache.Graphics, (info as NoPreviewAvailableInfo).Text, srcRect.Width));
				Rectangle textBounds = PlacementHelper.Arrange(textSize, srcRect, ContentAlignment.TopCenter);
				cache.DrawString((info as NoPreviewAvailableInfo).Text, PaintAppearance.GetFont(), PaintAppearance.GetForeBrush(cache), textBounds, PaintAppearance.GetStringFormat());
			}
			else if(info.PreviewImage != null) {
				cache.Graphics.DrawImage(info.PreviewImage, Rectangle.Inflate(info.PreviewBounds, -1, -1), srcRect, GraphicsUnit.Pixel);
			}
		}
		void InitPaintAppearance() {
			PaintAppearance.Assign(DefaultAppearance);
			PaintAppearance.TextOptions.HAlignment = HorzAlignment.Center;
			PaintAppearance.TextOptions.WordWrap = WordWrap.Wrap;
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault appearance = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control,
					new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + 4));
				return appearance ;
			}
		}
	}
	public class DocumentSelectorPreviewSkinPainter : DocumentSelectorPreviewPainter {
		ISkinProvider providerCore;
		public DocumentSelectorPreviewSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		protected override Color GetFadingColor() {
			return GetContentBackground().Color.GetBackColor();
		}
		protected SkinElement GetContentBackground() {
			return RibbonSkins.GetSkin(providerCore)[RibbonSkins.SkinPopupGalleryBackground];
		}
		protected SkinElement GetBackground() {
			return RibbonSkins.GetSkin(providerCore)[RibbonSkins.SkinGalleryButton];
		}
		protected SkinElement GetGalleryBackground() {
			SkinElement backgroud = DockingSkins.GetSkin(providerCore)[DockingSkins.SkinDocumentSelector];
			return backgroud ?? BarSkins.GetSkin(providerCore)[BarSkins.SkinAlertWindow];
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault appearance = new AppearanceDefault();
				SkinElement backElement = GetBackground();
				if(backElement.Color.GetForeColor().IsEmpty)
					backElement = GetGalleryBackground();
				appearance.ForeColor = DockingSkins.GetSkin(providerCore).Colors.GetColor(DockingSkins.DocumentSelectorForeColor, backElement.Color.GetForeColor());
				appearance.Font = MetroUISkins.GetSkin(providerCore)[MetroUISkins.SkinPageGroupItemHeaderButton].GetFont(AppearanceObject.DefaultFont);
				return appearance;
			}
		}
	}
	public class BackgroundInfoArgs : ObjectInfoArgs {
		public BackgroundInfoArgs(Rectangle bounds) {
			Bounds = bounds;
		}
		public BackgroundInfoArgs(Rectangle bounds, Rectangle content, Rectangle client) {
			Bounds = bounds;
			Content = content;
			Client = client;
		}
		public Rectangle Content { get; private set; }
		public Rectangle Client { get; private set; }
	}
	public class DocumentSelectorBackgroundPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DrawBackground(e.Cache, e as BackgroundInfoArgs);
		}
		protected virtual void DrawBackground(GraphicsCache cache, BackgroundInfoArgs info) {
			cache.FillRectangle(SystemBrushes.Control, info.Bounds);
			cache.DrawRectangle(SystemPens.ControlDark, info.Bounds);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 1, 8);
		}
		public virtual Padding GetAdditionalContentMargin() {
			return new Padding(7);
		}
	}
	public class DocumentSelectorBackgroundSkinPainter : DocumentSelectorBackgroundPainter {
		ISkinProvider providerCore;
		public DocumentSelectorBackgroundSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		protected virtual SkinElement GetDocumentSelector() {
			return DockingSkins.GetSkin(providerCore)[DockingSkins.SkinDocumentSelector];
		}
		protected virtual SkinElement GetBackground() {
			return BarSkins.GetSkin(providerCore)[BarSkins.SkinAlertWindow];
		}
		protected virtual SkinElement GetBackgroundTop() {
			return BarSkins.GetSkin(providerCore)[BarSkins.SkinAlertCaptionTop];
		}
		protected SkinElement GetContentBackground() {
			return RibbonSkins.GetSkin(providerCore)[RibbonSkins.SkinPopupGalleryBackground];
		}
		protected override void DrawBackground(GraphicsCache cache, BackgroundInfoArgs info) {
			SkinElement documentSelectorElement = GetDocumentSelector();
			if(documentSelectorElement != null) {
				SkinElementInfo backGroundInfo = new SkinElementInfo(documentSelectorElement, info.Bounds);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, backGroundInfo);
			}
			else DrawCompositeBackground(cache, info);
			if(!info.Content.IsEmpty) {
				SkinElementInfo contentInfo = new SkinElementInfo(GetContentBackground(), info.Content);
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, contentInfo);
			}
		}
		protected void DrawCompositeBackground(GraphicsCache cache, BackgroundInfoArgs info) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default,
				new SkinElementInfo(GetBackground(), info.Bounds));
			ObjectPainter.DrawObject(cache, new EmptySkinElementPainter(),
				new SkinElementInfo(GetBackgroundTop(), GetTopRectangle(info.Bounds)));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElement documentSelectorElement = GetDocumentSelector();
			if(documentSelectorElement != null)
				return documentSelectorElement.ContentMargins.Inflate(client);
			return Rectangle.Inflate(client, 1, 4);
		}
		public override Padding GetAdditionalContentMargin() {
			SkinElement documentSelectorElement = GetDocumentSelector();
			if(documentSelectorElement != null) {
				SkinProperties props = DockingSkins.GetSkin(providerCore).Properties;
				int h = props.GetInteger(DockingSkins.DocumentSelectorHorizontalContentMargin);
				int v = props.GetInteger(DockingSkins.DocumentSelectorVerticalContentMargin);
				return new Padding(h, v, h, v);
			}
			return new Padding(9);
		}
		Rectangle GetTopRectangle(Rectangle bounds) {
			return new Rectangle(bounds.X, bounds.Y, bounds.Width, 10);
		}
		class EmptySkinElementPainter : SkinElementPainter {
			protected override void DrawSkinForeground(SkinElementInfo ee) { }
		}
	}
	public class DefaultDocumentComparer : IComparer<BaseDocument> {
		public int Compare(BaseDocument document1, BaseDocument document2) {
			if(string.Equals(document1.Caption, document2.Caption)) return 0;
			return document1.Caption.CompareTo(document2.Caption);
		}
	}
	public class DefaultDockPanelComparer : IComparer<DockPanel> {
		public int Compare(DockPanel dockPanel1, DockPanel dockPanel2) {
			if(string.Equals(dockPanel1.Text, dockPanel2.Text)) return 0;
			return dockPanel1.Text.CompareTo(dockPanel2.Text);
		}
	}
}

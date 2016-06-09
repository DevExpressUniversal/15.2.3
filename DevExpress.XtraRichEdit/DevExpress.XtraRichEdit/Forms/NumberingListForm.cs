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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Design;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraTab;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils.Internal;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.tabControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.tabBulleted")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.tabNumbered")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.tabOutlineNumbered")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.btnCustomize")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.bulletedListBox")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.numberedListBox")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.outlineNumberedListBox")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.restartNumberingCheckEdit")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.NumberingListForm.continuePreviousListCheckEdit")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class NumberingListForm : XtraForm, IFormOwner {
		#region Fields
		readonly IRichEditControl control;
		readonly NumberingListFormController controller;
		#endregion
		NumberingListForm() {
			InitializeComponent();
		}
		public NumberingListForm(NumberingListFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeComponent();
			InitializeTabs();
			InitializeNumberingMode();
		}
		public ParagraphList Paragraphs { get { return Controller.Paragraphs; } }
		public NumberingListFormController Controller { get { return controller; } }
		protected internal NumberingListBox ActiveGallery { get { return (NumberingListBox)tabControl.SelectedTabPage.Tag; } }
		public IRichEditControl Control { get { return control; } }
		public DocumentModelUnitConverter UnitConverter { get { return Control.InnerControl.DocumentModel.UnitConverter; } }
		public bool ContinuePreviousList {
			get { return continuePreviousListCheckEdit.Checked; }
			set { continuePreviousListCheckEdit.Checked = value; }
		}
		public bool RestartNumbering {
			get { return restartNumberingCheckEdit.Checked; }
			set { restartNumberingCheckEdit.Checked = value; }
		}
		protected internal NumberingListFormController CreateController(NumberingListFormControllerParameters controllerParameters) {
			return new NumberingListFormController(controllerParameters);
		}
		protected internal virtual void InitializeTabs() {
			InitializeTabsCore();
			AbstractNumberingList abstractList = Controller.GetSelectedAbstractNumberingList();
			XtraTabPage selectedTabPage = GetInitialTabPage(abstractList);
			if (!selectedTabPage.PageEnabled) {
				selectedTabPage = ChangeSelectedTabPage(selectedTabPage);
				abstractList = null;
			}
			tabControl.SelectedTabPage = selectedTabPage;
			OnSelectedPageChanging(this, new TabPageChangingEventArgs(tabControl.SelectedTabPage, tabControl.SelectedTabPage));
			OnSelectedPageChanged(this, new TabPageChangedEventArgs(tabControl.SelectedTabPage, tabControl.SelectedTabPage));
			tabControl.SelectedPageChanging += OnSelectedPageChanging;
			tabControl.SelectedPageChanged += OnSelectedPageChanged;
			ActiveGallery.SelectedAbstractList = abstractList;
		}
		protected internal void InitializeTabsCore() {
			NumberingOptions numberingOptions = Control.InnerControl.Options.DocumentCapabilities.Numbering;
			this.tabBulleted.PageEnabled = numberingOptions.BulletedAllowed;
			this.tabNumbered.PageEnabled = numberingOptions.SimpleAllowed;
			this.tabOutlineNumbered.PageEnabled = numberingOptions.MultiLevelAllowed;
			this.bulletedListBox.InitializeControl((RichEditControl)control, NumberingType.Bullet, Controller.LevelIndex);
			this.numberedListBox.InitializeControl((RichEditControl)control, NumberingType.Simple, Controller.LevelIndex);
			this.outlineNumberedListBox.InitializeControl((RichEditControl)control, NumberingType.MultiLevel, Controller.LevelIndex);
			this.tabBulleted.Tag = bulletedListBox;
			this.tabNumbered.Tag = numberedListBox;
			this.tabOutlineNumbered.Tag = outlineNumberedListBox;
		}
		private void InitializeNumberingMode() {
			ParagraphIndex indexOfParagInListBeforeCurrent = GetIndexOfParagraphInListBeforeCurrent();
			bool areNumberingListsDifferent = IsParagraphInDifferentListFromCurrent(indexOfParagInListBeforeCurrent);
			RestartNumbering = areNumberingListsDifferent;
			ContinuePreviousList = !areNumberingListsDifferent;
		}
		protected internal XtraTabPage ChangeSelectedTabPage(XtraTabPage currentSelectedTabPage) {
			if (currentSelectedTabPage == tabBulleted) {
				if (tabNumbered.PageEnabled)
					return tabNumbered;
				else
					return tabOutlineNumbered;
			}
			if (currentSelectedTabPage == tabNumbered) {
				if (tabBulleted.PageEnabled)
					return tabBulleted;
				else
					return tabOutlineNumbered;
			}
			if (currentSelectedTabPage == tabOutlineNumbered) {
				if (tabBulleted.PageEnabled)
					return tabBulleted;
				else
					return tabNumbered;
			}
			Debug.Assert(false);
			return null;
		}
		protected internal XtraTabPage GetInitialTabPage(AbstractNumberingList list) {
			if (list == NumberingListFormController.NoneList)
				return tabBulleted;
			switch (GetNumberingListType(list)) {
				default:
				case NumberingType.Bullet:
					return tabBulleted;
				case NumberingType.Simple:
					return tabNumbered;
				case NumberingType.MultiLevel:
					return tabOutlineNumbered;
			}
		}
		protected internal NumberingType GetNumberingListType(AbstractNumberingList list) {
			if (list == null)
				return NumberingType.Bullet;
			return NumberingListHelper.GetListType(list);
		}
		void OnOkClick(object sender, EventArgs e) {
			if (ActiveGallery.SelectedIndex == -1)
				this.DialogResult = DialogResult.Cancel;
			else {
				ApplyChangesWithoutCustomization();
				this.DialogResult = DialogResult.OK;
			}
		}
		protected internal virtual void ApplyChanges(AbstractNumberingList newList) {
			ApplyChanges(newList, NumberingListApplyScope.ToSelectedText, NumberingType.Bullet, NumberingListIndex.ListIndexNotSetted);
		}
		protected void ApplyChanges(AbstractNumberingList newList, NumberingListApplyScope applyScope, NumberingType newListType, NumberingListIndex newListIndex) {
			Controller.ApplyScope = applyScope;
			Controller.NewListType = newListType;
			Controller.NewListIndex = newListIndex;
			Controller.NewAbstractList = newList;
			Controller.ApplyChanges();
		}
		protected internal SimpleRichEditControl GetSelectedSimpleControl() {
			return ActiveGallery.GetControls()[ActiveGallery.SelectedIndex];
		}
		void OnCustomizeClick(object sender, EventArgs e) {
			using (DocumentModel tempDocumentModel = new DocumentModel(RichEditDocumentFormatsDependecies.CreateDocumentFormatsDependecies())) {
				AbstractNumberingList clone = Controller.CreateAbstractNumberingListCopy(tempDocumentModel, ActiveGallery.SelectedAbstractList);
				clone.SetId(clone.GenerateNewId());
				if(IsBulletedTabSelected())
					ShowBulletedListForm(clone, clone.Levels);
				else if(IsNumberedTabSelected())
					ShowSimpleNumberingListForm(clone, clone.Levels);
				else
					ShowMultiLevelNumberingListForm(clone, clone.Levels);
			}
		}
		void ShowMultiLevelNumberingListForm(AbstractNumberingList source, ListLevelCollection<ListLevel> listLevels) {
			ShowNumberingListFormCore(source, listLevels, true);
		}
		void ShowSimpleNumberingListForm(AbstractNumberingList source, ListLevelCollection<ListLevel> listLevels) {
			ShowNumberingListFormCore(source, listLevels, false);
		}
		void ShowNumberingListFormCore(AbstractNumberingList source, ListLevelCollection<ListLevel> listLevels, bool multiLevels) {
			bool areThereParagraphsInList = AreThereParagraphsInList();
			var firstSelectedParagraph = GetFirstParagraphInList();
			int[] firstSelectedParagraphStartIndexes = areThereParagraphsInList ? firstSelectedParagraph.PieceTable.GetRangeListCounters(firstSelectedParagraph) : new int[] { 1 };
			ParagraphIndex indexOfParagInListBeforeCurrent = GetIndexOfParagraphInListBeforeCurrent();
			bool isPrevParagInDifferentList = IsParagraphInDifferentListFromCurrent(indexOfParagInListBeforeCurrent);
			bool selectedParagraphsHaveDifferentNumberingLists = DoSelectedParagraphsHaveDifferentNumberingLists();
			int levelIndex = areThereParagraphsInList ? Controller.LevelIndex : 0;
			int firstParagraphStartIndex = firstSelectedParagraphStartIndexes[levelIndex];
			if(RestartNumbering || selectedParagraphsHaveDifferentNumberingLists || !areThereParagraphsInList)
				firstParagraphStartIndex = 1;
			else if(isPrevParagInDifferentList) {
				var parag = firstSelectedParagraph.PieceTable.Paragraphs[indexOfParagInListBeforeCurrent];
				int[] prevParagraphInDifferentListStartIndexes = firstSelectedParagraph.PieceTable.GetRangeListCounters(parag);
				if(prevParagraphInDifferentListStartIndexes.Length == firstSelectedParagraphStartIndexes.Length)
					firstParagraphStartIndex = prevParagraphInDifferentListStartIndexes[Controller.LevelIndex] + 1;
			}
			using(SimpleNumberingListFormBase form = CreateNumberingListFormCore(listLevels, firstParagraphStartIndex, multiLevels)) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				if(FormTouchUIAdapter.ShowDialog(form, this) == DialogResult.OK) {
					NumberingListApplyScope applyScope = NumberingListApplyScope.ToSelectedText;
					NumberingType newListType = multiLevels ? NumberingType.MultiLevel : NumberingType.Simple;
					if(!selectedParagraphsHaveDifferentNumberingLists && areThereParagraphsInList) {
						bool isListLevelStartIndexCorrect = firstSelectedParagraphStartIndexes[Controller.LevelIndex] == source.Levels[Controller.LevelIndex].ListLevelProperties.Start;
						if(ContinuePreviousList && isPrevParagInDifferentList)
							isListLevelStartIndexCorrect = firstParagraphStartIndex == source.Levels[Controller.LevelIndex].ListLevelProperties.Start;
						if(ContinuePreviousList && isListLevelStartIndexCorrect)
							applyScope = isPrevParagInDifferentList ? NumberingListApplyScope.ContinuePreviousList : NumberingListApplyScope.ModifyCurrentList;
						else
							applyScope = NumberingListApplyScope.RestartNumbering;
					}
					NumberingListIndex newNumbListIndex = indexOfParagInListBeforeCurrent >= ParagraphIndex.MinValue ? firstSelectedParagraph.PieceTable.Paragraphs[indexOfParagInListBeforeCurrent].NumberingListIndex : NumberingListIndex.ListIndexNotSetted;
					ApplyChanges(source, applyScope, newListType, newNumbListIndex);
				}
			}
		}
		SimpleNumberingListFormBase CreateNumberingListFormCore(ListLevelCollection<ListLevel> listLevels, int firstParagraphStartIndex, bool multiLevels) {
			if(multiLevels)
				return new MultiLevelNumberingListForm(listLevels, Controller.LevelIndex, (RichEditControl)control, firstParagraphStartIndex, this);
			return new SimpleNumberingListForm(listLevels, Controller.LevelIndex, (RichEditControl)control, firstParagraphStartIndex, this);
		}
		bool IsNumberedTabSelected() {
			return tabControl.SelectedTabPage == tabNumbered;
		}
		bool IsOutlineNumberedTabSelected() {
			return tabControl.SelectedTabPage == tabOutlineNumbered;
		}
		bool IsBulletedTabSelected() {
			return tabControl.SelectedTabPage == tabBulleted;
		}
		bool DoSelectedParagraphsHaveDifferentNumberingLists() {
			NumberingListIndex firstSelectedParagraphListIndex = Controller.Paragraphs.First.NumberingListIndex;
			foreach(var parapraph in Controller.Paragraphs) {
				if(!parapraph.IsInList())
					continue;
				if(firstSelectedParagraphListIndex < NumberingListIndex.MinValue) {
					firstSelectedParagraphListIndex = parapraph.NumberingListIndex;
					continue;
				}
				if(parapraph.NumberingListIndex != firstSelectedParagraphListIndex)
					return true;
			}
			return false;
		}
		ParagraphIndex GetIndexOfParagraphInListBeforeCurrent() {
			ParagraphIndex wrongParagraphIndex = new ParagraphIndex(-1);
			if(!AreThereParagraphsInList())
				return wrongParagraphIndex;
			Paragraph firstSelectedParagraph = Controller.Paragraphs.First;
			ParagraphCollection paragraphs = firstSelectedParagraph.PieceTable.Paragraphs;
			for(ParagraphIndex i = firstSelectedParagraph.Index - 1; i >= paragraphs.First.Index; i--) {
				if(paragraphs[i].IsInList())
					return i;
			}
			return wrongParagraphIndex;
		}
		bool IsParagraphInDifferentListFromCurrent(ParagraphIndex index) {
			if(index < ParagraphIndex.MinValue || !AreThereParagraphsInList())
				return false;
			var firstSelectedParagraph = GetFirstParagraphInList();
			ParagraphCollection paragraphs = firstSelectedParagraph.PieceTable.Paragraphs;
			bool areNumberingListsIndexesDifferent = paragraphs[index].NumberingListIndex != firstSelectedParagraph.NumberingListIndex;
			return areNumberingListsIndexesDifferent;
		}
		void ApplyChangesWithoutCustomization() {
			if((IsNumberedTabSelected() || IsOutlineNumberedTabSelected()) && !DoSelectedParagraphsHaveDifferentNumberingLists() && AreThereParagraphsInList()) {
				var firstSelectedParagraph = GetFirstParagraphInList();
				var paragraphs = firstSelectedParagraph.PieceTable.Paragraphs;
				bool isOutlineNumbered = IsOutlineNumberedTabSelected();
				NumberingListApplyScope applyScope = NumberingListApplyScope.ToSelectedText;
				NumberingType newListType = isOutlineNumbered ? NumberingType.MultiLevel : NumberingType.Simple;
				ParagraphIndex indexOfParagInListBeforeCurrent = GetIndexOfParagraphInListBeforeCurrent();
				bool isNumbListOfParagDifferent = IsParagraphInDifferentListFromCurrent(indexOfParagInListBeforeCurrent);
				if(RestartNumbering)
					applyScope = NumberingListApplyScope.RestartNumbering;
				else if(ContinuePreviousList)
					applyScope = isNumbListOfParagDifferent ? NumberingListApplyScope.ContinuePreviousList : NumberingListApplyScope.ModifyCurrentList;
				NumberingListIndex newListIndex = indexOfParagInListBeforeCurrent >= ParagraphIndex.MinValue ? paragraphs[indexOfParagInListBeforeCurrent].NumberingListIndex : NumberingListIndex.ListIndexNotSetted;
				ApplyChanges(ActiveGallery.SelectedAbstractList, applyScope, newListType, newListIndex);
			}
			else
				ApplyChanges(ActiveGallery.SelectedAbstractList);
		}
		bool AreThereParagraphsInList() {
			foreach(var parag in this.Paragraphs) {
				if(parag.IsInList())
					return true;
			}
			return false;
		}
		Paragraph GetFirstParagraphInList() {
			foreach(var parag in this.Paragraphs) {
				if(parag.IsInList())
					return parag;
			}
			return null;
		}
		void ShowBulletedListForm(AbstractNumberingList source, ListLevelCollection<ListLevel> levels) {
			using(BulletedListForm form = new BulletedListForm(levels, Controller.LevelIndex, (RichEditControl)control, this)) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				if (FormTouchUIAdapter.ShowDialog(form, this) == DialogResult.OK)
					ApplyChanges(source);
			}
		}
		void OnSelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			ParagraphIndex indexOfParagInListBeforeCurrent = GetIndexOfParagraphInListBeforeCurrent();
			bool isParagValid = indexOfParagInListBeforeCurrent >= ParagraphIndex.MinValue;
			restartNumberingCheckEdit.Enabled = continuePreviousListCheckEdit.Enabled =
					(IsNumberedTabSelected() || IsOutlineNumberedTabSelected())
				&&  !DoSelectedParagraphsHaveDifferentNumberingLists()
				&&  isParagValid;
			NumberingListBox gallery = (NumberingListBox)e.Page.Tag;
			NumberingListBox prevGallery = (NumberingListBox)e.PrevPage.Tag;
			SubscribeGalleryEvents(gallery);
			int maxIndex = gallery.GetControls().Count - 1;
			if (prevGallery.SelectedIndex > maxIndex)
				gallery.SelectedIndex = maxIndex;
			else
				gallery.SelectedIndex = prevGallery.SelectedIndex;
		}
		void OnSelectedPageChanging(object sender, TabPageChangingEventArgs e) {
			UnsubscribeGalleryEvents((NumberingListBox)e.Page.Tag);
		}
		protected internal virtual void SubscribeGalleryEvents(NumberingListBox gallery) {
			gallery.MouseDoubleClick += OnActiveGalleryMouseDoubleClick;
			gallery.SelectedIndexChanged += OnSelectedIndexChanged;
		}
		protected internal virtual void UnsubscribeGalleryEvents(NumberingListBox gallery) {
			gallery.MouseDoubleClick -= OnActiveGalleryMouseDoubleClick;
			gallery.SelectedIndexChanged -= OnSelectedIndexChanged;
		}
		void OnActiveGalleryMouseDoubleClick(object sender, MouseEventArgs e) {
			if (((NumberingListBox)sender).isMouseClickInItems) {
				ApplyChangesWithoutCustomization();
				this.DialogResult = DialogResult.OK;
			}
		}
		void OnSelectedIndexChanged(object sender, EventArgs e) {
			if (((NumberingListBox)sender).SelectedIndex < 1)
				this.btnCustomize.Enabled = false;
			else
				this.btnCustomize.Enabled = true;
		}
		#region IFormOwner Members
		void IFormOwner.Hide() {
			this.Hide();
		}
		void IFormOwner.Show() {
			this.Show();
		}
		void IFormOwner.Close() {
			this.Close();
		}
		#endregion
	}
}

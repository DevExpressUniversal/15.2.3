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
using DevExpress.Utils;
using DevExpress.Office.History;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
namespace DevExpress.XtraRichEdit.Forms {
	#region NumberingListFormControllerParameters
	public class NumberingListFormControllerParameters : FormControllerParameters {
		readonly ParagraphList paragraphs;
		internal NumberingListFormControllerParameters(IRichEditControl control, ParagraphList paragraphs)
			: base(control) {
			Guard.ArgumentNotNull(paragraphs, "paragraphs");
			this.paragraphs = paragraphs;
		}
		internal ParagraphList Paragraphs { get { return paragraphs; } }
	}
	#endregion
	public enum NumberingListApplyScope {
		RestartNumbering,
		ModifyCurrentList,
		ContinuePreviousList,
		ToSelectedText
	}
	public class NumberingListFormController : FormController {
		#region Field
		static readonly AbstractNumberingList noneList = new AbstractNumberingList(new DocumentModel(DocumentFormatsDependencies.Empty));
		public static AbstractNumberingList NoneList { get { return noneList; } }
		NumberingListIndex selectedParagraphsListIndex;
		NumberingListIndex newListIndex;
		int levelIndex;
		readonly IRichEditControl control;
		readonly ParagraphList paragraphs;
		AbstractNumberingList newAbstractList;
		NumberingListApplyScope applyScope;
		NumberingType newListType;
		#endregion
		public NumberingListFormController(NumberingListFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.paragraphs = controllerParameters.Paragraphs;
			this.applyScope = NumberingListApplyScope.ToSelectedText;
			this.newListType = NumberingType.Bullet;
			InitializeController();
		}
		#region Properties
		public int LevelIndex { get { return levelIndex; } set { levelIndex = value; } }
		public NumberingListIndex SelectedParagraphsListIndex { get { return selectedParagraphsListIndex; } set { selectedParagraphsListIndex = value; } }
		public AbstractNumberingList NewAbstractList { get { return newAbstractList; } set { newAbstractList = value; } }
		public NumberingListIndex NewListIndex { get { return newListIndex; } set { newListIndex = value; } }
		public ParagraphList Paragraphs { get { return paragraphs; } }
		public DocumentModel DocumentModel { get { return control.InnerControl.DocumentModel; } }
		public NumberingListApplyScope ApplyScope { get { return applyScope; } set { applyScope = value; } }
		public NumberingType NewListType { get { return newListType; } set { newListType = value; } }
		#endregion
		protected internal virtual void InitializeController() {
			selectedParagraphsListIndex = GetSelectedParagraphsListIndex();
			if(selectedParagraphsListIndex >= NumberingListIndex.MinValue)
				levelIndex = Paragraphs.First.GetListLevelIndex();
		}
		protected internal virtual NumberingListIndex GetSelectedParagraphsListIndex() {
			if (!AreThereParagraphsInList() || AreThereParagraphsInDifferentLists())
				return NumberingListIndex.ListIndexNotSetted;
			return GetFirstNumberingListIndex();
		}
		protected internal virtual AbstractNumberingList GetSelectedAbstractNumberingList() {
			if(AreThereParagraphsInDifferentLists())
				return null;
			if(!AreThereParagraphsInList())
				return NoneList;
			NumberingListIndex listIndex = GetFirstNumberingListIndex();
			NumberingList list = DocumentModel.NumberingLists[listIndex];
			return list.AbstractNumberingList;
		}
		public AbstractNumberingList CreateAbstractNumberingListCopy(DocumentModel targetModel, AbstractNumberingList sourceList) {
			AbstractNumberingList result = new AbstractNumberingList(targetModel);
			result.CopyFrom(sourceList);
			return result;
		}
		public override void ApplyChanges() {
			control.BeginUpdate();
			DocumentModel.BeginUpdate();
			try {
				ApplyChangesCore();
			}
			finally {
				DocumentModel.EndUpdate();
				control.EndUpdate();
			}
		}
		protected internal virtual void ApplyChangesCore() {
			if (Object.ReferenceEquals(NewAbstractList, NoneList))
				RemoveNumberingList();
			else {
				switch (ApplyScope) {
					case NumberingListApplyScope.ToSelectedText:
						ApplyChangesToSelectedText();
						break;
					case NumberingListApplyScope.RestartNumbering:
						ApplyChangesRestartNumbering();
						break;
					case NumberingListApplyScope.ModifyCurrentList:
						ApplyChangesModifyCurrentList();
						break;
					case NumberingListApplyScope.ContinuePreviousList:
						ApplyChangesContinuePreviousList();
						break;
				}
			}
		}
		void RemoveNumberingList() {
			(new DeleteNumerationFromParagraphCommand(control)).Execute();
		}
		protected internal virtual void ApplyChangesRestartNumbering() {
			if(!AreThereParagraphsInList() || AreThereParagraphsInDifferentLists())
				return;
			DevExpress.XtraRichEdit.Utils.NumberingListIndexCalculator calculator = DocumentModel.CommandsCreationStrategy.CreateNumberingListIndexCalculator(DocumentModel, NewListType);
			NumberingListIndex newNumberingListIndex = calculator.CreateNewList(NewAbstractList);
			NumberingList newNumberingList = this.DocumentModel.NumberingLists[newNumberingListIndex];
			NumberingListIndex firstSelectedParagraphListIndex = GetFirstNumberingListIndex();
			int levelCount = NewListType == NumberingType.MultiLevel ? NewAbstractList.Levels.Count : 1;
			for(int i = 0; i < levelCount; i++)
				ModifyNumberingListLevel(newNumberingList, i);
			newNumberingList.Levels[this.LevelIndex].ListLevelProperties.Start = NewAbstractList.Levels[this.LevelIndex].ListLevelProperties.Start;
			UpdateParagraphsDown(firstSelectedParagraphListIndex, newNumberingListIndex);
		}
		protected internal virtual void ApplyChangesModifyCurrentList() {
			if(!AreThereParagraphsInList() || AreThereParagraphsInDifferentLists())
				return;
			var numberingListIndex = GetFirstNumberingListIndex();
			var list = this.DocumentModel.NumberingLists[numberingListIndex];
			int levelCount = NewAbstractList.Levels.Count;
			for(int i = 0; i < levelCount; i++)
				ModifyNumberingListLevel(list, i);
		}
		protected internal virtual void ApplyChangesContinuePreviousList() {
			if(!AreThereParagraphsInList() || AreThereParagraphsInDifferentLists())
				return;
			UpdateParagraphsDown(GetFirstNumberingListIndex(), NewListIndex);
		}
		bool AreThereParagraphsInList() {
			foreach(var parag in this.Paragraphs) {
				if(parag.IsInList())
					return true;
			}
			return false;
		}
		bool AreThereParagraphsInDifferentLists() {
			NumberingListIndex prevNumbListIndex = NumberingListIndex.ListIndexNotSetted;
			foreach(var parag in this.Paragraphs) {
				if(!parag.IsInList())
					continue;
				if(prevNumbListIndex == NumberingListIndex.ListIndexNotSetted) {
					prevNumbListIndex = parag.NumberingListIndex;
					continue;
				}
				if(prevNumbListIndex != parag.NumberingListIndex)
					return true;
			}
			return false;
		}
		NumberingListIndex GetFirstNumberingListIndex() {
			foreach(var parag in this.Paragraphs) {
				if(parag.IsInList())
					return parag.NumberingListIndex;
			}
			return NumberingListIndex.ListIndexNotSetted;
		}
		void UpdateParagraphsDown(NumberingListIndex numberingListIndex, NumberingListIndex newListIndex) {
			Paragraph firstSelectedParagraph = this.Paragraphs.First;
			ParagraphCollection paragraphs = firstSelectedParagraph.PieceTable.Paragraphs;
			ParagraphIndex firstSelectedParagraphIndex = firstSelectedParagraph.Index;
			for(ParagraphIndex i = firstSelectedParagraphIndex; i <= paragraphs.Last.Index; i++) {
				var parag = paragraphs[i];
				if(!parag.IsInList() || parag.NumberingListIndex != numberingListIndex)
					continue;
				if(NewListType == NumberingType.Simple && parag.GetOwnListLevelIndex() != this.LevelIndex)
					continue;
				ModifyParagraph(parag, newListIndex);
			}
		}
		void ModifyParagraph(Paragraph parag, NumberingListIndex listIndex) {
			if(listIndex < NumberingListIndex.MinValue || parag.NumberingListIndex < NumberingListIndex.MinValue)
				return;
			int levelIndex = NewListType == NumberingType.MultiLevel ? parag.GetOwnListLevelIndex() : this.LevelIndex;
			parag.PieceTable.RemoveParagraphFromList(parag.Index);
			parag.PieceTable.AddParagraphToList(parag.Index, listIndex, levelIndex);
		}
		void ModifyNumberingListLevel(NumberingList targetList, int levelIndex) {
			var characterFormattingInfo = NewAbstractList.Levels[levelIndex].CharacterProperties.Info.FormattingInfo;
			var characterFormattingOptions = NewAbstractList.Levels[levelIndex].CharacterProperties.Info.FormattingOptions;
			var characterMergedProperties = new MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions>(characterFormattingInfo, characterFormattingOptions);
			targetList.Levels[levelIndex].CharacterProperties.CopyFrom(characterMergedProperties);
			var paragraphFormattingInfo = NewAbstractList.Levels[levelIndex].ParagraphProperties.Info.FormattingInfo;
			var paragraphFormattingOptions = NewAbstractList.Levels[levelIndex].ParagraphProperties.Info.FormattingOptions;
			var paragraphMergedProperties = new MergedProperties<ParagraphFormattingInfo, ParagraphFormattingOptions>(paragraphFormattingInfo, paragraphFormattingOptions);
			targetList.Levels[levelIndex].ParagraphProperties.CopyFrom(paragraphMergedProperties);
			targetList.Levels[levelIndex].ListLevelProperties.Alignment = NewAbstractList.Levels[levelIndex].ListLevelProperties.Alignment;
			targetList.Levels[levelIndex].ListLevelProperties.Format = NewAbstractList.Levels[levelIndex].ListLevelProperties.Format;
		}
		protected internal virtual void ApplyChangesToSelectedText() {
			InsertListMode mode = CalculateListInsertionMode();
			NumberingListCommandBase command;
			if(NewAbstractList == NoneList)
				command = new DeleteNumerationFromParagraphCommand(control);
			else {
				if(!Object.ReferenceEquals(this.DocumentModel, NewAbstractList.DocumentModel))
					NewAbstractList = CreateAbstractNumberingListCopy(this.DocumentModel, NewAbstractList);
				command = new InsertListFormCommand(control, NewAbstractList, LevelIndex, mode);
			}
			command.Execute();
		}
		protected internal virtual InsertListMode CalculateListInsertionMode() {
			AbstractNumberingList selectedList = GetSelectedAbstractNumberingList();
			if (selectedList == null || selectedList == NoneList) {
				if (NumberingListHelper.GetListType(NewAbstractList) == NumberingType.MultiLevel)
					return InsertListMode.CalculateLevelIndexByIndent;
				else
					return InsertListMode.ChangeLevelIndex; 
			}
			else {
				NumberingType selectedListType = NumberingListHelper.GetListType(selectedList);
				NumberingType newListType = NumberingListHelper.GetListType(NewAbstractList);
				if (selectedListType == newListType || newListType == NumberingType.Bullet)
					return InsertListMode.KeepLevelIndex;
				else if (newListType == NumberingType.Simple)
					return InsertListMode.ChangeLevelIndex; 
				else
					return InsertListMode.KeepLevelIndex;
			}
		}
	}
}

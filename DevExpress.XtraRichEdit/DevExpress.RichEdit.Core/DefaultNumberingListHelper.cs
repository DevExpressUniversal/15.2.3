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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraRichEdit.Internal {
	public static class DefaultNumberingListHelper {
		internal static void InsertNumberingLists(DocumentModel documentModel, DocumentModelUnitConverter unitConverter, int defaultTabWidth) {
			InsertDefaultNumberingList(documentModel, unitConverter, defaultTabWidth, CreateDefaultSimpleAbstractNumberingList);
			InsertDefaultNumberingList(documentModel, unitConverter, defaultTabWidth, CreateDefaultBulletAbstractNumberingList);
			InsertDefaultNumberingList(documentModel, unitConverter, defaultTabWidth, CreateDefaultMultilevelAbstractNumberingList);
		}
		internal static AbstractNumberingList[] GetNumberingLists(DocumentModel documentModel, DocumentModelUnitConverter unitConverter, int defaultTabWidth) {
			return new AbstractNumberingList[] {
				CreateDefaultSimpleAbstractNumberingList(documentModel, unitConverter, defaultTabWidth),
				CreateDefaultBulletAbstractNumberingList(documentModel, unitConverter, defaultTabWidth),
				CreateDefaultMultilevelAbstractNumberingList(documentModel, unitConverter, defaultTabWidth)
			};
		}
		internal static void InsertDefaultSimpleNumberingList(DocumentModel documentModel, DocumentModelUnitConverter unitConverter, int defaultTabWidth) {
			InsertDefaultNumberingList(documentModel, unitConverter, defaultTabWidth, CreateDefaultSimpleAbstractNumberingList);
		}
		internal static void InsertDefaultBulletNumberingList(DocumentModel documentModel, DocumentModelUnitConverter unitConverter, int defaultTabWidth) {
			InsertDefaultNumberingList(documentModel, unitConverter, defaultTabWidth, CreateDefaultBulletAbstractNumberingList);
		}
		static void InsertDefaultNumberingList(DocumentModel documentModel, DocumentModelUnitConverter unitConverter, int defaultTabWidth, Func<DocumentModel, DocumentModelUnitConverter, int, AbstractNumberingList> createFunction) {
			AbstractNumberingList abstractNumberingList = createFunction(documentModel, unitConverter, defaultTabWidth);
			documentModel.AddAbstractNumberingListUsingHistory(abstractNumberingList);
			AbstractNumberingListIndex abstractNumberingListIndex = new AbstractNumberingListIndex(documentModel.AbstractNumberingLists.Count - 1);
			documentModel.AddNumberingListUsingHistory(new NumberingList(documentModel, abstractNumberingListIndex));
		}
		static AbstractNumberingList CreateDefaultBulletAbstractNumberingList(DocumentModel documentModel, DocumentModelUnitConverter unitConverter, int defaultTabWidth) {
#if !SL
			string[] symbolDisplayFormat = new string[] { "\u00B7", "\u006F" };
#endif
			int levelOffset = unitConverter.DocumentsToModelUnits(150);
			AbstractNumberingList abstractNumberingList = new AbstractNumberingList(documentModel);
			for(int i = 0; i < abstractNumberingList.Levels.Count; i++) {
				ListLevel level = new ListLevel(documentModel);
				abstractNumberingList.Levels[i] = level;
				level.CharacterProperties.BeginInit();
#if !SL
				level.CharacterProperties.FontName = "Symbol";
#else
				level.CharacterProperties.FontName = "Arial";
#endif
				level.CharacterProperties.EndInit();
				int firstLineIndent = levelOffset * (i + 1);
				SetFirstLineIndent(abstractNumberingList.Levels[i], firstLineIndent, defaultTabWidth / 2);
#if !SL
				int stringFormatIndex = i % symbolDisplayFormat.Length;
				SetDisplayFormatString(abstractNumberingList.Levels[i], symbolDisplayFormat[stringFormatIndex]);
#else
				SetDisplayFormatString(abstractNumberingList.Levels[i], "\u2022");
#endif
				SetTemplateCode(abstractNumberingList.Levels[i], NumberingListHelper.GenerateNewTemplateCode(documentModel));
				level.ListLevelProperties.Format = NumberingFormat.Bullet;
			}
			return abstractNumberingList;
		}
		static AbstractNumberingList CreateDefaultSimpleAbstractNumberingList(DocumentModel documentModel, DocumentModelUnitConverter unitConverter, int defaultTabWidth) {
			AbstractNumberingList abstractNumberingList = new AbstractNumberingList(documentModel);
			int levelOffset = unitConverter.DocumentsToModelUnits(150);
			for(int i = 0; i < abstractNumberingList.Levels.Count; i++) {
				ListLevel level = new ListLevel(documentModel);
				abstractNumberingList.Levels[i] = level;
				int firstLineIndent = levelOffset * (i + 1);
				SetFirstLineIndent(abstractNumberingList.Levels[i], firstLineIndent, defaultTabWidth / 2);
				SetDisplayFormatString(abstractNumberingList.Levels[i], String.Format("{{{0}}}.", i));
				SetTemplateCode(abstractNumberingList.Levels[i], NumberingListHelper.GenerateNewTemplateCode(documentModel));
			}
			return abstractNumberingList;
		}
		static AbstractNumberingList CreateDefaultMultilevelAbstractNumberingList(DocumentModel documentModel, DocumentModelUnitConverter unitConverter, int defaultTabWidth) {
			AbstractNumberingList abstractNumberingList = new AbstractNumberingList(documentModel);
			int levelOffset = unitConverter.DocumentsToModelUnits(75);
			int[] alignPositionsInDocuments = new int[] { 75, 165, 255, 360, 465, 570, 675, 780, 900 };
			for(int i = 0; i < abstractNumberingList.Levels.Count; i++) {
				ListLevel level = new ListLevel(documentModel);
				abstractNumberingList.Levels[i] = level;
				int firstLinePosition = levelOffset * i;
				int leftIndent = unitConverter.DocumentsToModelUnits(alignPositionsInDocuments[Math.Min(i, alignPositionsInDocuments.Length - 1)]);
				SetFirstLineIndent(abstractNumberingList.Levels[i], leftIndent, leftIndent - firstLinePosition);
			}
			SetDisplayFormatString(abstractNumberingList.Levels[0], "{0}.");
			SetDisplayFormatString(abstractNumberingList.Levels[1], "{0}.{1}.");
			SetDisplayFormatString(abstractNumberingList.Levels[2], "{0}.{1}.{2}.");
			SetDisplayFormatString(abstractNumberingList.Levels[3], "{0}.{1}.{2}.{3}.");
			SetDisplayFormatString(abstractNumberingList.Levels[4], "{0}.{1}.{2}.{3}.{4}.");
			SetDisplayFormatString(abstractNumberingList.Levels[5], "{0}.{1}.{2}.{3}.{4}.{5}.");
			SetDisplayFormatString(abstractNumberingList.Levels[6], "{0}.{1}.{2}.{3}.{4}.{5}.{6}.");
			SetDisplayFormatString(abstractNumberingList.Levels[7], "{0}.{1}.{2}.{3}.{4}.{5}.{6}.{7}.");
			SetDisplayFormatString(abstractNumberingList.Levels[8], "{0}.{1}.{2}.{3}.{4}.{5}.{6}.{7}.{8}.");
			return abstractNumberingList;
		}
		static void SetTemplateCode(IListLevel level, int templateCode) {
			level.ListLevelProperties.BeginInit();
			try {
				level.ListLevelProperties.TemplateCode = templateCode;
			} finally {
				level.ListLevelProperties.EndInit();
			}
		}
		static void SetDisplayFormatString(IListLevel level, string displayFormatString) {
			level.ListLevelProperties.BeginInit();
			try {
				level.ListLevelProperties.DisplayFormatString = displayFormatString;
			} finally {
				level.ListLevelProperties.EndInit();
			}
		}
		static void SetFirstLineIndent(IListLevel level, int lineIndent, int firstLineIndent) {
			level.ParagraphProperties.BeginInit();
			try {
				level.ParagraphProperties.LeftIndent = lineIndent;
				level.ParagraphProperties.FirstLineIndentType = DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent.Hanging;
				level.ParagraphProperties.FirstLineIndent = firstLineIndent;
			} finally {
				level.ParagraphProperties.EndInit();
			}
		}
	}
}

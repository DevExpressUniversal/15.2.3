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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Model;
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentCache
	public class DocumentCache : IDisposable {
		#region Fields
		bool isDisposed;
		CharacterFormattingInfoCache mergedCharacterFormattingInfoCache;
		CharacterFormattingInfoCache characterFormattingInfoCache;
		CharacterFormattingOptionsCache characterFormattingOptionsCache;
		CharacterFormattingCache characterFormattingCache;
		ParagraphFormattingInfoCache paragraphFormattingInfoCache;
		ParagraphFormattingOptionsCache paragraphFormattingOptionsCache;
		ParagraphFormattingCache paragraphFormattingCache;
		ParagraphFormattingInfoCache mergedParagraphFormattingInfoCache;
		ParagraphFrameFormattingInfoCache paragraphFrameFormattingInfoCache;
		ParagraphFrameFormattingOptionsCache paragraphFrameFormattingOptionsCache;
		ParagraphFrameFormattingCache paragraphFrameFormattingCache;
		ParagraphFrameFormattingInfoCache mergedParagraphFrameFormattingInfoCache;
		DocumentInfoCache documentInfoCache;
		MarginsInfoCache marginsInfoCache;
		ColumnsInfoCache columnsInfoCache;
		PageInfoCache pageInfoCache;
		GeneralSectionInfoCache generalSectionInfoCache;
		PageNumberingInfoCache pageNumberingInfoCache;
		LineNumberingInfoCache lineNumberingInfoCache;
		TabFormattingInfoCache tabFormattingInfoCache;
		InlinePictureInfoCache inlinePictureInfoCache;
		InlineCustomObjectInfoCache inlineCustomObjectInfoCache;
		TablePropertiesOptionsCache tablePropertiesOptionsCache;
		WidthUnitInfoCache unitInfoCache;
		BorderInfoCache borderInfoCache;
		ListLevelInfoCache listLevelInfoCache;
		TableGeneralSettingsInfoCache tableGeneralSettingsInfoCache;
		TableFloatingPositionInfoCache tableFloatingPositionInfoCache;
		TableCellGeneralSettingsInfoCache tableCellGeneralSettingsInfoCache;
		TableCellPropertiesOptionsCache tableCellPropertiesOptionsCache;
		HeightUnitInfoCache heightUnitInfoCache;
		TableRowGeneralSettingsInfoCache tableRowGeneralSettingsInfoCache;
		TableRowPropertiesOptionsCache tableRowPropertiesOptionsCache;
		RangePermissionInfoCache rangePermissionInfoCache;
		DocumentProtectionInfoCache documentProtectionInfoCache;
		FootNoteInfoCache footNoteInfoCache;
		FloatingObjectInfoCache floatingObjectInfoCache;
		FloatingObjectOptionsCache floatingObjectOptionsCache;
		FloatingObjectFormattingCache floatingObjectFormattingCache;
		ShapeInfoCache shapeInfoCache;
		ShapeOptionsCache shapeOptionsCache;
		ShapeFormattingCache shapeFormattingCache;
		TextBoxInfoCache textBoxInfoCache;
		TextBoxOptionsCache textBoxOptionsCache;
		TextBoxFormattingCache textBoxFormattingCache;
		ColorModelInfoCache colorModelInfoCache;
		#endregion
		public DocumentCache() {
		}
		#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		public CharacterFormattingOptionsCache CharacterFormattingOptionsCache { get { return characterFormattingOptionsCache; } }
		public CharacterFormattingInfoCache MergedCharacterFormattingInfoCache { get { return mergedCharacterFormattingInfoCache; } }
		public CharacterFormattingInfoCache CharacterFormattingInfoCache { get { return characterFormattingInfoCache; } }
		public CharacterFormattingCache CharacterFormattingCache { get { return characterFormattingCache; } }
		public DocumentInfoCache DocumentInfoCache { get { return documentInfoCache; } }
		public MarginsInfoCache MarginsInfoCache { get { return marginsInfoCache; } }
		public ColumnsInfoCache ColumnsInfoCache { get { return columnsInfoCache; } }
		public PageInfoCache PageInfoCache { get { return pageInfoCache; } }
		public GeneralSectionInfoCache GeneralSectionInfoCache { get { return generalSectionInfoCache; } }
		public PageNumberingInfoCache PageNumberingInfoCache { get { return pageNumberingInfoCache; } }
		public LineNumberingInfoCache LineNumberingInfoCache { get { return lineNumberingInfoCache; } }
		public ParagraphFormattingOptionsCache ParagraphFormattingOptionsCache { get { return paragraphFormattingOptionsCache; } }
		public ParagraphFormattingInfoCache ParagraphFormattingInfoCache { get { return paragraphFormattingInfoCache; } }
		public ParagraphFormattingInfoCache MergedParagraphFormattingInfoCache { get { return mergedParagraphFormattingInfoCache; } }
		public ParagraphFormattingCache ParagraphFormattingCache { get { return paragraphFormattingCache; } }
		public ParagraphFrameFormattingOptionsCache ParagraphFrameFormattingOptionsCache { get { return paragraphFrameFormattingOptionsCache; } }
		public ParagraphFrameFormattingInfoCache ParagraphFrameFormattingInfoCache { get { return paragraphFrameFormattingInfoCache; } }
		public ParagraphFrameFormattingInfoCache MergedParagraphFrameFormattingInfoCache { get { return mergedParagraphFrameFormattingInfoCache; } }
		public ParagraphFrameFormattingCache ParagraphFrameFormattingCache { get { return paragraphFrameFormattingCache; } }
		public TabFormattingInfoCache TabFormattingInfoCache { get { return tabFormattingInfoCache; } }
		public WidthUnitInfoCache UnitInfoCache { get { return unitInfoCache; } }
		public BorderInfoCache BorderInfoCache { get { return borderInfoCache; } }
		public ListLevelInfoCache ListLevelInfoCache { get { return listLevelInfoCache; } }
		public TableFloatingPositionInfoCache TableFloatingPositionInfoCache { get { return tableFloatingPositionInfoCache; } }
		public TableCellGeneralSettingsInfoCache TableCellGeneralSettingsInfoCache { get { return tableCellGeneralSettingsInfoCache; } }
		public TableGeneralSettingsInfoCache TableGeneralSettingsInfoCache { get { return tableGeneralSettingsInfoCache; } }
		public InlinePictureInfoCache InlinePictureInfoCache { get { return inlinePictureInfoCache; } }
		public InlineCustomObjectInfoCache InlineCustomObjectInfoCache { get { return inlineCustomObjectInfoCache; } }
		public TablePropertiesOptionsCache TablePropertiesOptionsCache { get { return tablePropertiesOptionsCache; } }
		public HeightUnitInfoCache HeightUnitInfoCache { get { return heightUnitInfoCache; } }
		public TableRowPropertiesOptionsCache TableRowPropertiesOptionsCache { get { return tableRowPropertiesOptionsCache; } }
		public TableRowGeneralSettingsInfoCache TableRowGeneralSettingsInfoCache { get { return tableRowGeneralSettingsInfoCache; } }
		public TableCellPropertiesOptionsCache TableCellPropertiesOptionsCache { get { return tableCellPropertiesOptionsCache; } }
		public RangePermissionInfoCache RangePermissionInfoCache { get { return rangePermissionInfoCache; } }
		public DocumentProtectionInfoCache DocumentProtectionInfoCache { get { return documentProtectionInfoCache; } }
		public FootNoteInfoCache FootNoteInfoCache { get { return footNoteInfoCache; } }
		public FloatingObjectInfoCache FloatingObjectInfoCache { get { return floatingObjectInfoCache; } }
		public FloatingObjectOptionsCache FloatingObjectOptionsCache { get { return floatingObjectOptionsCache; } }
		public FloatingObjectFormattingCache FloatingObjectFormattingCache { get { return floatingObjectFormattingCache; } }
		public ShapeInfoCache ShapeInfoCache { get { return shapeInfoCache; } }
		public ShapeOptionsCache ShapeOptionsCache { get { return shapeOptionsCache; } }
		public ShapeFormattingCache ShapeFormattingCache { get { return shapeFormattingCache; } }
		public TextBoxInfoCache TextBoxInfoCache { get { return textBoxInfoCache; } }
		public TextBoxOptionsCache TextBoxOptionsCache { get { return textBoxOptionsCache; } }
		public TextBoxFormattingCache TextBoxFormattingCache { get { return textBoxFormattingCache; } }
		public ColorModelInfoCache ColorModelInfoCache { get { return colorModelInfoCache; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region Initialize
		protected internal virtual void Initialize(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			DocumentModelUnitConverter unitConverter = documentModel.UnitConverter;
			this.mergedCharacterFormattingInfoCache = new CharacterFormattingInfoCache(unitConverter);
			this.characterFormattingInfoCache = new CharacterFormattingInfoCache(unitConverter);
			this.characterFormattingOptionsCache = new CharacterFormattingOptionsCache(unitConverter);
			this.characterFormattingCache = new CharacterFormattingCache(documentModel);
			this.documentInfoCache = new DocumentInfoCache(unitConverter);
			this.marginsInfoCache = new MarginsInfoCache(unitConverter);
			this.columnsInfoCache = new ColumnsInfoCache(unitConverter);
			this.pageInfoCache = new PageInfoCache(unitConverter);
			this.generalSectionInfoCache = new GeneralSectionInfoCache(unitConverter);
			this.pageNumberingInfoCache = new PageNumberingInfoCache(unitConverter);
			this.lineNumberingInfoCache = new LineNumberingInfoCache(unitConverter);
			this.paragraphFormattingInfoCache = new ParagraphFormattingInfoCache(unitConverter);
			this.mergedParagraphFormattingInfoCache = new ParagraphFormattingInfoCache(unitConverter);
			this.paragraphFormattingOptionsCache = new ParagraphFormattingOptionsCache(unitConverter);
			this.paragraphFormattingCache = new ParagraphFormattingCache(documentModel);
			this.paragraphFrameFormattingInfoCache = new ParagraphFrameFormattingInfoCache(unitConverter);
			this.mergedParagraphFrameFormattingInfoCache = new ParagraphFrameFormattingInfoCache(unitConverter);
			this.paragraphFrameFormattingOptionsCache = new ParagraphFrameFormattingOptionsCache(unitConverter);
			this.paragraphFrameFormattingCache = new ParagraphFrameFormattingCache(documentModel);
			this.tabFormattingInfoCache = new TabFormattingInfoCache(unitConverter);
			this.inlinePictureInfoCache = new InlinePictureInfoCache(unitConverter);
			this.inlineCustomObjectInfoCache = new InlineCustomObjectInfoCache(unitConverter);
			this.tablePropertiesOptionsCache = new TablePropertiesOptionsCache(unitConverter);
			this.unitInfoCache = new WidthUnitInfoCache(unitConverter);
			this.borderInfoCache = new BorderInfoCache(unitConverter);
			this.listLevelInfoCache = new ListLevelInfoCache(unitConverter);
			this.tableGeneralSettingsInfoCache = new TableGeneralSettingsInfoCache(unitConverter);
			this.tableFloatingPositionInfoCache = new TableFloatingPositionInfoCache(unitConverter);
			this.tableCellGeneralSettingsInfoCache = new TableCellGeneralSettingsInfoCache(unitConverter);
			this.tableCellPropertiesOptionsCache = new TableCellPropertiesOptionsCache(unitConverter);
			this.tableRowGeneralSettingsInfoCache = new TableRowGeneralSettingsInfoCache(unitConverter);
			this.tableRowPropertiesOptionsCache = new TableRowPropertiesOptionsCache(unitConverter);
			this.heightUnitInfoCache = new HeightUnitInfoCache(unitConverter);
			this.rangePermissionInfoCache = new RangePermissionInfoCache(unitConverter);
			this.documentProtectionInfoCache = new DocumentProtectionInfoCache(unitConverter);
			this.footNoteInfoCache = new FootNoteInfoCache(unitConverter);
			this.floatingObjectInfoCache = new FloatingObjectInfoCache(unitConverter);
			this.floatingObjectOptionsCache = new FloatingObjectOptionsCache(unitConverter);
			this.floatingObjectFormattingCache = new FloatingObjectFormattingCache(documentModel);
			this.shapeInfoCache = new ShapeInfoCache(unitConverter);
			this.shapeOptionsCache = new ShapeOptionsCache(unitConverter);
			this.shapeFormattingCache = new ShapeFormattingCache(documentModel);
			this.textBoxInfoCache = new TextBoxInfoCache(unitConverter);
			this.textBoxOptionsCache = new TextBoxOptionsCache(unitConverter);
			this.textBoxFormattingCache = new TextBoxFormattingCache(documentModel);
			this.colorModelInfoCache = new ColorModelInfoCache(unitConverter);
		}
		#endregion
		public List<SizeOfInfo> GetSizeOfInfo() {
			List<SizeOfInfo> result = ObjectSizeHelper.CalculateSizeOfInfo(this);
			result.Insert(0, ObjectSizeHelper.CalculateTotalSizeOfInfo(result, "DocumentModel.Cache Total"));
			return result;
		}
		public void Clear() {
		}
	}
	#endregion
}

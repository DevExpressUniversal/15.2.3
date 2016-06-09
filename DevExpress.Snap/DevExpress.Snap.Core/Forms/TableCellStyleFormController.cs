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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Forms {
	public class TableCellStyleFormControllerParameters : TableStyleFormControllerParametersBase {
		#region Fields
		TableCellStyle tableCellSourceStyle;
		#endregion
		public TableCellStyleFormControllerParameters(IRichEditControl control, TableCellStyle sourceCellStyle)
			: base(control) {
			Guard.ArgumentNotNull(sourceCellStyle, "style");
			this.tableCellSourceStyle = sourceCellStyle;
		}
		#region Properties
		internal TableCellStyle TableSourceStyle { get { return tableCellSourceStyle; } set { tableCellSourceStyle = value; } }
		#endregion
	}
	public class TableCellStyleFormController : TableStyleFormControllerBase {
		#region Fields
		readonly TableCellStyle editedTableCellStyle;
		readonly int editedTableStyleIndex;
		#endregion
		public TableCellStyleFormController(IRichEditControl previewStyleControl, FormControllerParameters parameters)
			: base(previewStyleControl, parameters) {
			TableCellStyleCollection sourceTableCellStyles = parameters.Control.InnerControl.DocumentModel.TableCellStyles;
			DocumentModel previewModel = previewStyleControl.InnerDocumentServer.DocumentModel;
			foreach (TableCellStyle style in sourceTableCellStyles) {
				if (!style.Deleted)
					style.Copy(previewModel);
			}
			this.editedTableStyleIndex = ((TableCellStyleFormControllerParameters)parameters).TableSourceStyle.Copy(previewModel);
			this.editedTableCellStyle = previewModel.TableCellStyles[editedTableStyleIndex];
		}
		#region Properties
		public override string StyleName { get { return IntermediateTableCellStyle.LocalizedStyleName; } set { IntermediateTableCellStyle.StyleName = value; } }
		public override int StyleIndex { get { return editedTableStyleIndex; } }
		public TableCellStyle TableSourceStyle { get { return ((TableCellStyleFormControllerParameters)Parameters).TableSourceStyle; } }
		public TableCellStyle IntermediateTableCellStyle { get { return editedTableCellStyle; } }
		public override ITableBorders CurrentBorders {
			get { return ConditionalStyleType == ConditionalTableStyleFormattingTypes.WholeTable ? IntermediateTableCellStyle.TableCellProperties.Borders : (ITableBorders)TableCellProperties.Borders; }
		}
		public override CharacterProperties CharacterProperties { get { return IntermediateTableCellStyle.CharacterProperties; } }
		public override ParagraphProperties ParagraphProperties { get { return IntermediateTableCellStyle.ParagraphProperties; } }
		public override TableCellProperties TableCellProperties { get { return IntermediateTableCellStyle.TableCellProperties; } }
		public override TableProperties TableProperties { get { return null; } }
		private TableCellStyle TableCellStyleParent { get { return IntermediateTableCellStyle.Parent; } }
		public override ConditionalTableStyleFormattingTypes ConditionalStyleType { get; set; }
		public override TableConditionalStyleProperties ConditionalStyleProperties { get { return null; } }
		public override TabProperties Tabs { get { return IntermediateTableCellStyle.Tabs; } }
		#endregion
		public override TabFormattingInfo GetTabInfo() {
			return IntermediateTableCellStyle.GetTabs();
		}
		public override MergedCharacterProperties GetMergedCharacterProperties() {
			return IntermediateTableCellStyle.GetMergedWithDefaultCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal);
		}
		public override MergedParagraphProperties GetMergedParagraphProperties() {
			return IntermediateTableCellStyle.GetMergedWithDefaultParagraphProperties();
		}
		public override CharacterFormattingInfo GetMergedWithDefaultCharacterProperties() {
			return GetMergedCharacterProperties().Info;
		}
		public override CombinedCellPropertiesInfo GetMergedTableCellProperties() {
			return IntermediateTableCellStyle.GetMergedTableCellProperties().Info;
		}
		public override void ApplyChanges() {
			TableSourceStyle.BeginUpdate();
			try {
				TableSourceStyle.CopyProperties(IntermediateTableCellStyle);
				TableSourceStyle.StyleName = IntermediateTableCellStyle.StyleName;
				TableSourceStyle.Parent = TableSourceStyle.DocumentModel.TableCellStyles.GetStyleByName(IntermediateTableCellStyle.Parent.StyleName);
			}
			finally {
				TableSourceStyle.EndUpdate();
			}
		}
		public override CharacterFormattingInfo GetParentCharacterProperties() {
			return TableCellStyleParent != null ? TableCellStyleParent.GetMergedCharacterProperties(ConditionalRowType.Normal, ConditionalColumnType.Normal).Info : null;
		}
		public override ParagraphFormattingInfo GetParentParagraphProperties() {
			return TableCellStyleParent != null ? TableCellStyleParent.GetMergedParagraphProperties().Info : null;
		}
		public override ParagraphFormattingOptions GetParentParagraphPropertiesOptions() {
			return TableCellStyleParent != null ? TableCellStyleParent.GetMergedParagraphProperties().Options : null;
		}
		public override TableCellGeneralSettingsInfo GetParentTableCellProperties() {
			return TableCellStyleParent != null ? TableCellStyleParent.GetMergedTableCellProperties().Info.GeneralSettings : null;
		}
		public override TableCellPropertiesOptions GetParentTableCellPropertiesOptions() {
			return TableCellStyleParent != null ? TableCellStyleParent.GetMergedTableCellProperties().Options : null;
		}
		protected override void SetCurrentStyle(Table tempTable) {
			TableCellProcessorDelegate setStyleIndex = delegate(TableCell cell) {
				cell.StyleIndex = StyleIndex;
			};
			tempTable.ForEachCell(setStyleIndex);
		}
		public override void AddStyle() {
		}
		public override bool IsValidName(string name) {
			DocumentModel model = TableSourceStyle.DocumentModel;
			return EditStyleHelper.IsValidStyleName<TableCellStyle>(name, TableSourceStyle, model.TableCellStyles);
		}
	}
}

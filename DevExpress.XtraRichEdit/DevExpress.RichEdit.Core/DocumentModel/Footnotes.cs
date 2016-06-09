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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.NumberConverters;
namespace DevExpress.XtraRichEdit.Model {
	#region FootNoteBase<T>
	public class FootNoteBase<T> : ContentTypeBase where T : FootNoteBase<T> {
		FootNoteRunBase<T> referenceRun;
		public FootNoteBase(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		public override bool IsMain { get { return false; } }
		public override bool IsHeaderFooter { get { return false; } }
		public override bool IsFooter { get { return false; } }
		public override bool IsHeader { get { return false; } }
		public override bool IsNote { get { return true; } }
		public override bool IsReferenced { get { return ReferenceRun != null; } }
		public FootNoteRunBase<T> ReferenceRun { get { return referenceRun; } set { referenceRun = value; } }
		#endregion
		protected internal override SectionIndex LookupSectionIndexByParagraphIndex(ParagraphIndex paragraphIndex) {
			return SectionIndex.DontCare;
		}
		protected internal override void FixLastParagraphOfLastSection(int originalParagraphCount) {
		}
	}
	#endregion
	#region FootNote
	public class FootNote : FootNoteBase<FootNote> {
		public static readonly string FootNoteCounterId = "_counter_footnotes";
		public FootNote(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		public override bool IsFootNote { get { return true; } }
		public override bool IsEndNote { get { return false; } }
		#endregion
	}
	#endregion    
	public class FootNoteCollection : List<FootNote> {
	}
	#region FootNotePosition
	public enum FootNotePosition {
		BottomOfPage, 
		BelowText, 
		EndOfDocument, 
		EndOfSection 
	}
	#endregion
	#region FootNoteInfo
	public class FootNoteInfo : ICloneable<FootNoteInfo>, ISupportsCopyFrom<FootNoteInfo>, ISupportsSizeOf {
		#region Fields
		FootNotePosition position;
		NumberingFormat numberingFormat;
		LineNumberingRestart numberingRestartType;
		int startingNumber;
		string customMark;
		#endregion
		#region Properties
		public FootNotePosition Position { get { return position; } set { position = value; } }
		public NumberingFormat NumberingFormat { get { return numberingFormat; } set { numberingFormat = value; } }
		public LineNumberingRestart NumberingRestartType { get { return numberingRestartType; } set { numberingRestartType = value; } }
		public int StartingNumber { get { return startingNumber; } set { startingNumber = value; } }
		public string CustomMark { get { return customMark; } set { customMark = value; } }
		#endregion
		#region ICloneable<FootNoteInfo> Members
		public FootNoteInfo Clone() {
			FootNoteInfo info = new FootNoteInfo();
			info.CopyFrom(this);
			return info;
		}
		#endregion
		public virtual void CopyFrom(FootNoteInfo info) {
			this.Position = info.Position;
			this.NumberingFormat = info.NumberingFormat;
			this.NumberingRestartType = info.NumberingRestartType;
			this.StartingNumber = info.StartingNumber;
			this.CustomMark = info.CustomMark;
		}
		public override bool Equals(object obj) {
			FootNoteInfo info = (FootNoteInfo)obj;
			return
				this.Position == info.Position &&
				this.NumberingFormat == info.NumberingFormat &&
				this.NumberingRestartType == info.NumberingRestartType &&
				this.StartingNumber == info.StartingNumber &&
				this.CustomMark == info.CustomMark;
		}
		public override int GetHashCode() {
			return (int)Position ^ (int)NumberingFormat ^ (int)NumberingRestartType ^ StartingNumber ^ (customMark == null ? 0 : CustomMark.GetHashCode());
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region FootNoteInfoCache
	public class FootNoteInfoCache : UniqueItemsCache<FootNoteInfo> {
		public FootNoteInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		public static int DefaultFootNoteItemIndex { get { return 0; } }
		public static int DefaultEndNoteItemIndex { get { return 1; } }
		protected override FootNoteInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			FootNoteInfo info = new FootNoteInfo();
			info.Position = FootNotePosition.BottomOfPage;
			info.NumberingFormat = NumberingFormat.Decimal;
			info.NumberingRestartType = LineNumberingRestart.Continuous;
			info.StartingNumber = 1;
			info.CustomMark = String.Empty;
			return info;
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			base.InitItems(unitConverter);
			FootNoteInfo info = new FootNoteInfo();
			info.Position = FootNotePosition.EndOfDocument;
			info.NumberingFormat = NumberingFormat.LowerRoman;
			info.NumberingRestartType = LineNumberingRestart.Continuous;
			info.StartingNumber = 1;
			info.CustomMark = String.Empty;
			AppendItem(info);
		}
	}
	#endregion
	#region SectionFootNote
	public class SectionFootNote : RichEditIndexBasedObject<FootNoteInfo> {
		public SectionFootNote(DocumentModel documentModel)
			: base(Section.GetMainPieceTable(documentModel)) {
		}
		#region Properties
		#region Position
		public FootNotePosition Position {
			get { return Info.Position; }
			set {
				if (Position == value)
					return;
				SetPropertyValue(SetPositionCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetPositionCore(FootNoteInfo info, FootNotePosition value) {
			info.Position = value;
			return SectionFootNoteChangeActionsCalculator.CalculateChangeActions(SectionFootNoteChangeType.Position);
		}
		#endregion
		#region NumberingFormat
		public NumberingFormat NumberingFormat {
			get { return Info.NumberingFormat; }
			set {
				if (NumberingFormat == value)
					return;
				SetPropertyValue(SetNumberingFormatCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetNumberingFormatCore(FootNoteInfo info, NumberingFormat value) {
			info.NumberingFormat = value;
			return SectionFootNoteChangeActionsCalculator.CalculateChangeActions(SectionFootNoteChangeType.NumberingFormat);
		}
		#endregion
		#region NumberingRestartType
		public LineNumberingRestart NumberingRestartType {
			get { return Info.NumberingRestartType; }
			set {
				if (NumberingRestartType == value)
					return;
				SetPropertyValue(SetNumberingRestartTypeCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetNumberingRestartTypeCore(FootNoteInfo info, LineNumberingRestart value) {
			info.NumberingRestartType = value;
			return SectionFootNoteChangeActionsCalculator.CalculateChangeActions(SectionFootNoteChangeType.NumberingRestartType);
		}
		#endregion
		#region StartingNumber
		public int StartingNumber {
			get { return Info.StartingNumber; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("StartingNumber", value);
				if (StartingNumber == value)
					return;
				SetPropertyValue(SetStartingNumberCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetStartingNumberCore(FootNoteInfo info, int value) {
			info.StartingNumber = value;
			return SectionFootNoteChangeActionsCalculator.CalculateChangeActions(SectionFootNoteChangeType.StartingNumber);
		}
		#endregion
		#region CustomMark
		public string CustomMark {
			get { return Info.CustomMark; }
			set {
				if (CustomMark == value)
					return;
				SetPropertyValue(SetCustomMarkCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetCustomMarkCore(FootNoteInfo info, string value) {
			info.CustomMark = value;
			return SectionFootNoteChangeActionsCalculator.CalculateChangeActions(SectionFootNoteChangeType.CustomMark);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<FootNoteInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.FootNoteInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return SectionFootNoteChangeActionsCalculator.CalculateChangeActions(SectionFootNoteChangeType.BatchUpdate);
		}
		public string FormatCounterValue(int value) {
			OrdinalBasedNumberConverter converter = OrdinalBasedNumberConverter.CreateConverter(NumberingFormat, LanguageId.English);
			return converter.ConvertNumber(value);
		}
	}
	#endregion
	#region SectionFootNoteChangeType
	public enum SectionFootNoteChangeType {
		None = 0,
		Position,
		NumberingFormat,
		NumberingRestartType,
		StartingNumber,
		CustomMark,
		BatchUpdate
	}
	#endregion
	#region SectionFootNoteChangeActionsCalculator
	public static class SectionFootNoteChangeActionsCalculator {
		internal class SectionFootNoteChangeActionsTable : Dictionary<SectionFootNoteChangeType, DocumentModelChangeActions> {
		}
		internal static SectionFootNoteChangeActionsTable sectionFootNoteChangeActionsTable = CreateSectionFootNoteChangeActionsTable();
		internal static SectionFootNoteChangeActionsTable CreateSectionFootNoteChangeActionsTable() {
			SectionFootNoteChangeActionsTable table = new SectionFootNoteChangeActionsTable();
			table.Add(SectionFootNoteChangeType.None, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionFootNoteChangeType.Position, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionFootNoteChangeType.NumberingFormat, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionFootNoteChangeType.NumberingRestartType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionFootNoteChangeType.StartingNumber, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionFootNoteChangeType.CustomMark, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionFootNoteChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(SectionFootNoteChangeType change) {
			return sectionFootNoteChangeActionsTable[change];
		}
	}
	#endregion
}

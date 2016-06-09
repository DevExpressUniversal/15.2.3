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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.API.Native {
	#region ParagraphAlignment
	[ComVisible(true)]
	public enum ParagraphAlignment {
		Left = DevExpress.XtraRichEdit.Model.ParagraphAlignment.Left,
		Right = DevExpress.XtraRichEdit.Model.ParagraphAlignment.Right,
		Center = DevExpress.XtraRichEdit.Model.ParagraphAlignment.Center,
		Justify = DevExpress.XtraRichEdit.Model.ParagraphAlignment.Justify
	}
	#endregion
	#region ParagraphLineSpacing
	[ComVisible(true)]
	public enum ParagraphLineSpacing {
		Single = DevExpress.XtraRichEdit.Model.ParagraphLineSpacing.Single,
		Sesquialteral = DevExpress.XtraRichEdit.Model.ParagraphLineSpacing.Sesquialteral,
		Double = DevExpress.XtraRichEdit.Model.ParagraphLineSpacing.Double,
		Multiple = DevExpress.XtraRichEdit.Model.ParagraphLineSpacing.Multiple,
		Exactly = DevExpress.XtraRichEdit.Model.ParagraphLineSpacing.Exactly,
		AtLeast = DevExpress.XtraRichEdit.Model.ParagraphLineSpacing.AtLeast
	}
	#endregion
	#region ParagraphFirstLineIndent
	[ComVisible(true)]
	public enum ParagraphFirstLineIndent {
		None = DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent.None,
		Indented = DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent.Indented,
		Hanging = DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent.Hanging
	}
	#endregion
	#region TabAlignmentType
	[ComVisible(true)]
	public enum TabAlignmentType {
		Left = DevExpress.XtraRichEdit.Model.TabAlignmentType.Left,
		Center = DevExpress.XtraRichEdit.Model.TabAlignmentType.Center,
		Right = DevExpress.XtraRichEdit.Model.TabAlignmentType.Right,
		Decimal = DevExpress.XtraRichEdit.Model.TabAlignmentType.Decimal
	}
	#endregion
	#region TabLeaderType
	[ComVisible(true)]
	public enum TabLeaderType {
		None = DevExpress.XtraRichEdit.Model.TabLeaderType.None,
		Dots = DevExpress.XtraRichEdit.Model.TabLeaderType.Dots,
		EqualSign = DevExpress.XtraRichEdit.Model.TabLeaderType.EqualSign,
		Hypens = DevExpress.XtraRichEdit.Model.TabLeaderType.Hyphens,
		MiddleDots = DevExpress.XtraRichEdit.Model.TabLeaderType.MiddleDots,
		Underline = DevExpress.XtraRichEdit.Model.TabLeaderType.Underline,
	}
	#endregion
	#region TabInfo
	[ComVisible(true)]
	public class TabInfo {
		TabAlignmentType alignment;
		TabLeaderType leader;
		float position;
		bool deleted;
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("TabInfoAlignment")]
#endif
		public TabAlignmentType Alignment { get { return alignment; } set { alignment = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("TabInfoLeader")]
#endif
		public TabLeaderType Leader { get { return leader; } set { leader = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("TabInfoPosition")]
#endif
		public float Position { get { return position; } set { position = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("TabInfoDeleted")]
#endif
		public bool Deleted { get { return deleted; } set { deleted = value; } }
	}
	#endregion
	#region TabInfoCollection
	[ComVisible(true)]
	public class TabInfoCollection : List<TabInfo> {
	}
	#endregion
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217")]
	[Flags]
	public enum ParagraphPropertiesMask {
		None = 0x00000000,
		Alignment = 0x00000001,
		LeftIndent = 0x00000002,
		RightIndent = 0x00000004,
		SpacingBefore = 0x00000008,
		SpacingAfter = 0x00000010,
		LineSpacing = 0x00000020,
		FirstLineIndent = 0x00000040,
		SuppressHyphenation = 0x00000080,
		SuppressLineNumbers = 0x00000100,
		ContextualSpacing = 0x00000200,
		PageBreakBefore = 0x00000400,
		KeepLinesTogether = 0x00004000,
		OutlineLevel = 0x00010000,
		BackColor = 0x00020000,
		All = 0x7FFFFFFF
	}
	#region ParagraphPropertiesBase
	[ComVisible(true)]
	public interface ParagraphPropertiesBase {
		ParagraphAlignment? Alignment { get; set; }
		float? LeftIndent { get; set; }
		float? RightIndent { get; set; }
		float? SpacingBefore { get; set; }
		float? SpacingAfter { get; set; }
		ParagraphLineSpacing? LineSpacingType { get; set; }
		float? LineSpacing { get; set; }
		float? LineSpacingMultiplier { get; set; }
		ParagraphFirstLineIndent? FirstLineIndentType { get; set; }
		float? FirstLineIndent { get; set; }
		bool? SuppressHyphenation { get; set; }
		bool? SuppressLineNumbers { get; set; }
		int? OutlineLevel { get; set; }
		bool? KeepLinesTogether { get; set; }
		bool? PageBreakBefore { get ; set; }
		Color? BackColor { get; set; }
		bool? ContextualSpacing { get; set; }
		void Reset();
		void Reset(ParagraphPropertiesMask mask);
	}
	#endregion
	#region ParagraphPropertiesWithTabs
	[ComVisible(true)]
	public interface ParagraphPropertiesWithTabs : ParagraphPropertiesBase {
		TabInfoCollection BeginUpdateTabs(bool onlyOwnTabs);
		void EndUpdateTabs(TabInfoCollection tabs);
	}
	#endregion
	#region ParagraphProperties
	[ComVisible(true)]
	public interface ParagraphProperties : ParagraphPropertiesWithTabs {
		ParagraphStyle Style { get; set; }
	}
	#endregion
	#region Paragraph
	[ComVisible(true)]
	public abstract class Paragraph {
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphRange")]
#endif
		public abstract DocumentRange Range { get; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphLeftIndent")]
#endif
		public abstract float LeftIndent { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphRightIndent")]
#endif
		public abstract float RightIndent { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphSpacingBefore")]
#endif
		public abstract float SpacingBefore { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphSpacingAfter")]
#endif
		public abstract float SpacingAfter { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphAlignment")]
#endif
		public abstract ParagraphAlignment Alignment { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphLineSpacingType")]
#endif
		public abstract ParagraphLineSpacing LineSpacingType { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphLineSpacing")]
#endif
		public abstract float LineSpacing { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphLineSpacingMultiplier")]
#endif
		public abstract float LineSpacingMultiplier { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphFirstLineIndentType")]
#endif
		public abstract ParagraphFirstLineIndent FirstLineIndentType { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphFirstLineIndent")]
#endif
		public abstract float FirstLineIndent { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphSuppressHyphenation")]
#endif
		public abstract bool SuppressHyphenation { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphSuppressLineNumbers")]
#endif
		public abstract bool SuppressLineNumbers { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphOutlineLevel")]
#endif
		public abstract int OutlineLevel { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphKeepLinesTogether")]
#endif
		public abstract bool KeepLinesTogether { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphPageBreakBefore")]
#endif
		public abstract bool PageBreakBefore { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphContextualSpacing")]
#endif
		public abstract bool ContextualSpacing { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphBackColor")]
#endif
		public abstract Color BackColor { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphStyle")]
#endif
		public abstract ParagraphStyle Style { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphIndex")]
#endif
		public abstract int Index { get; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphListIndex")]
#endif
		public abstract int ListIndex { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphListLevel")]
#endif
		public abstract int ListLevel { get; set; }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ParagraphIsInList")]
#endif
		public abstract bool IsInList { get; }
		public abstract TabInfoCollection BeginUpdateTabs(bool onlyOwnTabs);
		public abstract void EndUpdateTabs(TabInfoCollection tabs);
		protected internal abstract DevExpress.XtraRichEdit.Model.ParagraphIndex ParagraphIndex { get; }
		protected internal abstract bool EqualsCore(Paragraph paragraph);
		protected internal abstract int GetHashCodeCore();
		public abstract void Reset();
		#region Operators
		public static bool operator ==(Paragraph paragraph1, Paragraph paragraph2) {
			if (Object.ReferenceEquals(paragraph1, null) && Object.ReferenceEquals(paragraph2, null))
				return true;
			if (Object.ReferenceEquals(paragraph1, null) || Object.ReferenceEquals(paragraph2, null))
				return false;
			return paragraph1.EqualsCore(paragraph2);
		}
		public static bool operator !=(Paragraph paragraph1, Paragraph paragraph2) {
			return !(paragraph1 == paragraph2);
		}
		#endregion
		public override bool Equals(object obj) {
			return EqualsCore((Paragraph)obj);
		}
		public override int GetHashCode() {
			return GetHashCodeCore();
		}
	}
	#endregion
	#region ReadOnlyParagraphCollection
	[ComVisible(true)]
	public interface ReadOnlyParagraphCollection : ISimpleCollection<Paragraph> {
		ReadOnlyParagraphCollection Get(DocumentRange range);
		Paragraph Get(DocumentPosition pos);
	}
	#endregion
	#region ParagraphCollection
	[ComVisible(true)]
	public interface ParagraphCollection : ReadOnlyParagraphCollection {
		Paragraph Insert(DocumentPosition pos);
		Paragraph Insert(DocumentPosition pos, InsertOptions insertOptions);
		Paragraph Append();
		void AddParagraphsToList(DocumentRange range, NumberingList list, int levelIndex);
		void AddParagraphToList(Paragraph paragraph, NumberingList list, int levelIndex);
		void AddParagraphToList(Paragraph paragraph, int numberingListIndex, int levelIndex);
		void RemoveNumberingFromParagraph(Paragraph paragraph);
		void RemoveNumberingFromParagraphs(DocumentRange range);
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using DevExpress.XtraRichEdit.Localization;
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelParagraph = DevExpress.XtraRichEdit.Model.Paragraph;
	using ModelParagraphCollection = DevExpress.Office.Utils.List<DevExpress.XtraRichEdit.Model.Paragraph, DevExpress.XtraRichEdit.Model.ParagraphIndex>;
	using ModelParagraphIndex = DevExpress.XtraRichEdit.Model.ParagraphIndex;
	using ModelTabFormattingInfo = DevExpress.XtraRichEdit.Model.TabFormattingInfo;
	using ModelTabInfo = DevExpress.XtraRichEdit.Model.TabInfo;
	using ModelParagraphStyle = DevExpress.XtraRichEdit.Model.ParagraphStyle;
	using DevExpress.Office.Utils;
	using ModelNumberingListIndex = DevExpress.XtraRichEdit.Model.NumberingListIndex;
	using ModelParagraphFormattingOptions = DevExpress.XtraRichEdit.Model.ParagraphFormattingOptions;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelDocumentModel = DevExpress.XtraRichEdit.Model.DocumentModel;
	using Compatibility.System.Drawing;
	#region NativeParagraph
	public class NativeParagraph : Paragraph {
		#region Fields
		readonly NativeSubDocument document;
		readonly ModelParagraph innerParagraph;
		bool isValid;
		#endregion
		internal NativeParagraph(NativeSubDocument document, ModelParagraph innerParagraph) {
			this.document = document;
			this.innerParagraph = innerParagraph;
			this.isValid = true;
		}
		#region Properties
		public ModelParagraph InnerParagraph { get { return innerParagraph; } }
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		public InternalAPI InternalAPI { get { return Document.InternalAPI; } }
		public NativeSubDocument Document { get { return document; } }
		protected internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get { return innerParagraph.DocumentModel; } }
		protected internal DevExpress.XtraRichEdit.Model.PieceTable PieceTable { get { return innerParagraph.PieceTable; } }
		public override int Index { get { return ((IConvertToInt<ModelParagraphIndex>)InnerParagraph.Index).ToInt(); } }
		#region LeftIndent
		public override float LeftIndent {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(InnerParagraph.LeftIndent);
			}
			set {
				if (LeftIndent == value)
					return;
				InnerParagraph.LeftIndent = Document.UnitsToModelUnits(value);
				OnChanged();
			}
		}
		#endregion
		#region RightIndent
		public override float RightIndent {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(InnerParagraph.RightIndent);
			}
			set {
				if (RightIndent == value)
					return;
				InnerParagraph.RightIndent = Document.UnitsToModelUnits(value);
				OnChanged();
			}
		}
		#endregion
		#region SpacingBefore
		public override float SpacingBefore {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(InnerParagraph.SpacingBefore);
			}
			set {
				if (SpacingBefore == value)
					return;
				InnerParagraph.SpacingBefore = Document.UnitsToModelUnits(value);
				OnChanged();
			}
		}
		#endregion
		#region SpacingAfter
		public override float SpacingAfter {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(InnerParagraph.SpacingAfter);
			}
			set {
				if (SpacingAfter == value)
					return;
				InnerParagraph.SpacingAfter = Document.UnitsToModelUnits(value);
				OnChanged();
			}
		}
		#endregion
		#region Alignment
		public override ParagraphAlignment Alignment {
			get {
				CheckValid();
				return (ParagraphAlignment)InnerParagraph.Alignment;
			}
			set {
				CheckValid();
				DevExpress.XtraRichEdit.Model.ParagraphAlignment val = (DevExpress.XtraRichEdit.Model.ParagraphAlignment)value;
				if (InnerParagraph.Alignment == val)
					return;
				InnerParagraph.Alignment = val;
				OnChanged();
			}
		}
		#endregion
		#region LineSpacingType
		public override ParagraphLineSpacing LineSpacingType {
			get {
				CheckValid();
				return (ParagraphLineSpacing)InnerParagraph.LineSpacingType;
			}
			set {
				CheckValid();
				DevExpress.XtraRichEdit.Model.ParagraphLineSpacing val = (DevExpress.XtraRichEdit.Model.ParagraphLineSpacing)value;
				if (InnerParagraph.LineSpacingType == val)
					return;
				InnerParagraph.LineSpacingType = val;
				OnChanged();
			}
		}
		#endregion
		#region LineSpacing
		public override float LineSpacing {
			get {
				CheckValid();
				return Document.ModelUnitsToUnitsF(InnerParagraph.LineSpacing);
			}
			set {
				if (LineSpacing == value)
					return;
				InnerParagraph.LineSpacing = Document.UnitsToModelUnitsF(value);
				OnChanged();
			}
		}
		#endregion
		#region LineSpacingMultiplier
		public override float LineSpacingMultiplier {
			get {
				CheckValid();
				return InnerParagraph.LineSpacing;
			}
			set {
				if (LineSpacingMultiplier == value)
					return;
				InnerParagraph.LineSpacing = value;
				OnChanged();
			}
		}
		#endregion
		#region FirstLineIndentType
		public override ParagraphFirstLineIndent FirstLineIndentType {
			get {
				CheckValid();
				return (ParagraphFirstLineIndent)InnerParagraph.FirstLineIndentType;
			}
			set {
				CheckValid();
				DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent val = (DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent)value;
				if (InnerParagraph.FirstLineIndentType == val)
					return;
				InnerParagraph.FirstLineIndentType = val;
				OnChanged();
			}
		}
		#endregion
		#region FirstLineIndent
		public override float FirstLineIndent {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(InnerParagraph.FirstLineIndent);
			}
			set {
				if (FirstLineIndent == value)
					return;
				InnerParagraph.FirstLineIndent = Document.UnitsToModelUnits(value);
				OnChanged();
			}
		}
		#endregion
		#region SuppressHyphenation
		public override bool SuppressHyphenation {
			get {
				CheckValid();
				return InnerParagraph.SuppressHyphenation;
			}
			set {
				CheckValid();
				if (InnerParagraph.SuppressHyphenation == value)
					return;
				InnerParagraph.SuppressHyphenation = value;
				OnChanged();
			}
		}
		#endregion
		#region SuppressLineNumbers
		public override bool SuppressLineNumbers {
			get {
				CheckValid();
				return InnerParagraph.SuppressLineNumbers;
			}
			set {
				CheckValid();
				if (InnerParagraph.SuppressLineNumbers == value)
					return;
				InnerParagraph.SuppressLineNumbers = value;
				OnChanged();
			}
		}
		#endregion
		#region OutlineLevel
		public override int OutlineLevel {
			get {
				CheckValid();
				return InnerParagraph.OutlineLevel;
			}
			set {
				CheckValid();
				if (InnerParagraph.OutlineLevel == value)
					return;
				InnerParagraph.OutlineLevel = value;
				OnChanged();
			}
		}
		#endregion
		#region KeepLinesTogether
		public override bool KeepLinesTogether {
			get {
				CheckValid();
				return InnerParagraph.KeepLinesTogether;
			}
			set {
				CheckValid();
				if (InnerParagraph.KeepLinesTogether == value)
					return;
				InnerParagraph.KeepLinesTogether = value;
				OnChanged();
			}
		}
		#endregion
		#region PageBreakBefore
		public override bool PageBreakBefore {
			get {
				CheckValid();
				return InnerParagraph.PageBreakBefore;
			}
			set {
				CheckValid();
				if (InnerParagraph.PageBreakBefore == value)
					return;
				InnerParagraph.PageBreakBefore = value;
				OnChanged();
			}
		}
		#endregion
		#region ContextualSpacing
		public override bool ContextualSpacing {
			get {
				CheckValid();
				return InnerParagraph.ContextualSpacing;
			}
			set {
				CheckValid();
				if (InnerParagraph.ContextualSpacing == value)
					return;
				InnerParagraph.ContextualSpacing = value;
				OnChanged();
			}
		}
		#endregion
		#region BackColor
		public override Color BackColor {
			get {
				CheckValid();
				return InnerParagraph.BackColor;
			}
			set {
				CheckValid();
				if (InnerParagraph.BackColor == value)
					return;
				InnerParagraph.BackColor = value;
				OnChanged();
			}
		}
		#endregion
		#region Style
		public override ParagraphStyle Style {
			get {
				CheckValid();
				NativeParagraphStyleCollection styles = (NativeParagraphStyleCollection)Document.MainDocument.ParagraphStyles;
				return styles.GetStyle(InnerParagraph.ParagraphStyle);
			}
			set {
				CheckValid();
				ModelParagraphStyle style = value != null ? ((NativeParagraphStyle)value).InnerStyle : null;
				InnerParagraph.ParagraphStyleIndex = DocumentModel.ParagraphStyles.IndexOf(style);
			}
		}
		#endregion
		#endregion
		void CheckValid() {
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedParagraphError);
		}
		#region Paragraph Members
		public override DocumentRange Range {
			get {
				ModelPosition start = ModelPosition.FromParagraphStart(PieceTable, InnerParagraph.Index);
				ModelPosition end = ModelPosition.FromParagraphEnd(PieceTable, InnerParagraph.Index);
				return new NativeDocumentRange(document, start, end);
			}
		}
		protected internal override DevExpress.XtraRichEdit.Model.ParagraphIndex ParagraphIndex { get { return InnerParagraph.Index; } }
		#endregion
		protected internal override bool EqualsCore(Paragraph paragraph) {
			return ParagraphIndex == paragraph.ParagraphIndex;
		}
		protected internal override int GetHashCodeCore() {
			return ((IConvertToInt<DevExpress.XtraRichEdit.Model.ParagraphIndex>)ParagraphIndex).ToInt();
		}
		protected internal virtual void OnChanged() {
		}
		public override TabInfoCollection BeginUpdateTabs(bool onlyOwnTabs) {
			CheckValid();
			ModelTabFormattingInfo tabs;
			if (onlyOwnTabs)
				tabs = InnerParagraph.GetOwnTabs();
			else
				tabs = InnerParagraph.GetTabs();
			return NativeParagraphProperties.CreateTabInfoCollection(Document, tabs);
		}
		public override void EndUpdateTabs(TabInfoCollection tabs) {
			CheckValid();
			if (tabs == null)
				return;
			ModelTabFormattingInfo tabInfo = NativeParagraphProperties.CreateModelTabInfoCollection(Document, tabs);
			InnerParagraph.SetOwnTabs(tabInfo);
		}
		public override void Reset() {
			CheckValid();
			InnerParagraph.ParagraphProperties.ResetAllUse();
		}
		public override int ListIndex {
			get {
				CheckValid();
				return ((IConvertToInt<Model.NumberingListIndex>)InnerParagraph.GetNumberingListIndex()).ToInt();
			}
			set {
				CheckValid();
				innerParagraph.DocumentModel.BeginUpdate();
				try {
					DevExpress.XtraRichEdit.Model.Paragraph modelParagraph = innerParagraph;
					if (modelParagraph.IsInList())
						modelParagraph.PieceTable.RemoveNumberingFromParagraph(modelParagraph);
					InnerParagraph.NumberingListIndex = new Model.NumberingListIndex(value);
					OnChanged();
				}
				finally {
					innerParagraph.DocumentModel.EndUpdate();
				}
			}
		}
		public override int ListLevel {
			get {
				CheckValid();
				return InnerParagraph.GetListLevelIndex();
			}
			set {
				CheckValid();
				try {
					InnerParagraph.DocumentModel.BeginUpdate();
					DevExpress.XtraRichEdit.Model.Paragraph modelParagraph = innerParagraph;
					modelParagraph.PieceTable.ApplyListLevelIndexToParagraph(modelParagraph, value);
					OnChanged();
				}
				finally {
					InnerParagraph.DocumentModel.EndUpdate();
				}
			}
		}
		public override bool IsInList {
			get {
				CheckValid();
				return InnerParagraph.IsInList();
			}
		}
	}
	#endregion
	#region NativeParagraphCollection
	public class NativeParagraphCollection : List<NativeParagraph>, ParagraphCollection {
		readonly NativeSubDocument document;
		public NativeParagraphCollection(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		ModelDocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		#region ISimpleCollection<Paragraph> Members
		Paragraph ISimpleCollection<Paragraph>.this[int index] {
			get { return this[index]; }
		}
		#endregion
		#region IEnumerable<Paragraph> Members
		IEnumerator<Paragraph> IEnumerable<Paragraph>.GetEnumerator() {
			return new EnumeratorAdapter<Paragraph, NativeParagraph>(this.GetEnumerator());
		}
		#endregion
		public Paragraph Insert(DocumentPosition pos) {
			return Insert(pos, InsertOptions.MatchDestinationFormatting);
		}
		public Paragraph Insert(DocumentPosition pos, InsertOptions insertOptions) {
			document.CheckValid();
			document.CheckDocumentPosition(pos);
			NativeDocumentPosition nativePosition = (NativeDocumentPosition)pos;
			ModelParagraphIndex paragraphIndex = nativePosition.Position.ParagraphIndex;
			ModelLogPosition logPosition = document.NormalizeLogPosition(nativePosition.Position.LogPosition);
			PieceTable.InsertParagraph(logPosition);
			DevExpress.XtraRichEdit.Model.Paragraph paragraph = PieceTable.Paragraphs[paragraphIndex + 1];
			if (paragraph.IsInList() && insertOptions == InsertOptions.KeepSourceFormatting)
				PieceTable.RemoveNumberingFromParagraph(paragraph);
			return this[document.ParagraphIndexToInt(paragraphIndex + 1)];
		}
		public Paragraph Append() {
			return Insert(document.EndPosition);
		}
		public Paragraph Get(DocumentPosition pos) {
			document.CheckValid();
			document.CheckDocumentPosition(pos);
			ModelParagraphIndex paragraphIndex = PieceTable.FindParagraphIndex(pos.LogPosition, false);
			int index = ((IConvertToInt<ModelParagraphIndex>)paragraphIndex).ToInt();
			if (index < 0)
				return null;
			return this[index];
		}
		public ReadOnlyParagraphCollection Get(DocumentRange range) {
			document.CheckValid();
			Guard.ArgumentNotNull(range, "range");
			DevExpress.XtraRichEdit.API.Native.Implementation.NativeSubDocument.ParagraphRange paragraphsRange = document.CalculateParagraphsRange(range);
			int firstIndex = ((IConvertToInt<ModelParagraphIndex>)paragraphsRange.Start).ToInt();
			int lastIndex = firstIndex + paragraphsRange.Length - 1;
			NativeParagraphCollection result = new NativeParagraphCollection(document);
			for (int i = firstIndex; i <= lastIndex; i++)
				result.Add(this[i]);
			return result;
		}
		public void AddParagraphToList(Paragraph paragraph, NumberingList list, int levelIndex) {
			AddParagraphsToList(paragraph.Range, list, levelIndex);
		}
		public void AddParagraphToList(Paragraph paragraph, int numberingListIndex, int levelIndex) {
			AddParagraphsToList(paragraph.Range, document.MainDocument.NumberingLists[numberingListIndex], levelIndex);
		}
		public void AddParagraphsToList(DocumentRange range, NumberingList list, int levelIndex) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			DocumentModel.BeginUpdate();
			try {
				NativeParagraphCollection paragraphs = (NativeParagraphCollection)Get(range);
				DevExpress.XtraRichEdit.Model.NumberingList innerNumberingList = ((NativeNumberingList)list).InnerNumberingList;
				int index = ((NativeNumberingListCollection)(document.MainDocument.NumberingLists)).InnerLists.IndexOf(innerNumberingList);
				if (index < 0)
					throw new InvalidOperationException(XtraRichEditLocalizer.GetString(XtraRichEditStringId.Msg_NumberingListNotInListCollection));
				Model.NumberingListIndex numberingListIndex = new Model.NumberingListIndex(index);
				for (int i = 0; i < paragraphs.Count; i++) {
					NativeParagraph paragraph = paragraphs[i];
					DevExpress.XtraRichEdit.Model.Paragraph modelParagraph = paragraph.InnerParagraph;
					if (modelParagraph.IsInList())
						modelParagraph.PieceTable.RemoveNumberingFromParagraph(modelParagraph);
					this.PieceTable.AddNumberingListToParagraph(modelParagraph, numberingListIndex, levelIndex);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void RemoveNumberingFromParagraph(Paragraph paragraph) {
			RemoveNumberingFromParagraphs(paragraph.Range);
		}
		public void RemoveNumberingFromParagraphs(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			DocumentModel.BeginUpdate();
			try {
				NativeParagraphCollection paragraphs = (NativeParagraphCollection)Get(range);
				for (int i = 0; i < paragraphs.Count; i++) {
					NativeParagraph paragraph = paragraphs[i];
					DevExpress.XtraRichEdit.Model.Paragraph modelParagraph = paragraph.InnerParagraph;
					if (modelParagraph.IsInList())
						modelParagraph.PieceTable.RemoveNumberingFromParagraph(modelParagraph);
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region ParagraphNullablePropertyAccessor<T> (abstract class)
	public abstract class ParagraphNullablePropertyAccessor<T> : SmartPropertyAccessor<Nullable<T>> where T : struct {
		readonly ModelParagraphCollection paragraphs;
		protected ParagraphNullablePropertyAccessor(ModelParagraphCollection paragraphs) {
			Guard.ArgumentNotNull(paragraphs, "paragraphs");
			this.paragraphs = paragraphs;
		}
		public ModelParagraphCollection Paragraphs { get { return paragraphs; } }
		protected internal override T? CalculateValueCore() {
			ModelParagraphIndex count = new ModelParagraphIndex(Paragraphs.Count);
			if (count <= ModelParagraphIndex.Zero)
				return null;
			T result = CalculateValueCore(Paragraphs[new ModelParagraphIndex(0)]);
			for (ModelParagraphIndex i = new ModelParagraphIndex(1); i < count; i++) {
				T value = CalculateValueCore(Paragraphs[i]);
				if (!value.Equals(result))
					return null;
			}
			return result;
		}
		protected internal override bool SetValueCore(T? value) {
			if (!value.HasValue)
				return false;
			ModelParagraphIndex count = new ModelParagraphIndex(Paragraphs.Count);
			if (count <= ModelParagraphIndex.Zero)
				return false;
			SetValueCore(Paragraphs[new ModelParagraphIndex(0)], value.Value);
			for (ModelParagraphIndex i = new ModelParagraphIndex(1); i < count; i++)
				SetValueCore(Paragraphs[i], value.Value);
			return true;
		}
		protected internal abstract T CalculateValueCore(ModelParagraph paragraph);
		protected internal abstract void SetValueCore(ModelParagraph paragraph, T value);
	}
	#endregion
	#region ParagraphLeftIndentPropertyAccessor
	public class ParagraphLeftIndentPropertyAccessor : ParagraphNullablePropertyAccessor<int> {
		public ParagraphLeftIndentPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override int CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.LeftIndent;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, int value) {
			paragraph.LeftIndent = value;
		}
	}
	#endregion
	#region ParagraphRightIndentPropertyAccessor
	public class ParagraphRightIndentPropertyAccessor : ParagraphNullablePropertyAccessor<int> {
		public ParagraphRightIndentPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override int CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.RightIndent;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, int value) {
			paragraph.RightIndent = value;
		}
	}
	#endregion
	#region ParagraphSpacingBeforePropertyAccessor
	public class ParagraphSpacingBeforePropertyAccessor : ParagraphNullablePropertyAccessor<int> {
		public ParagraphSpacingBeforePropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override int CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.SpacingBefore;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, int value) {
			paragraph.SpacingBefore = value;
		}
	}
	#endregion
	#region ParagraphSpacingAfterPropertyAccessor
	public class ParagraphSpacingAfterPropertyAccessor : ParagraphNullablePropertyAccessor<int> {
		public ParagraphSpacingAfterPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override int CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.SpacingAfter;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, int value) {
			paragraph.SpacingAfter = value;
		}
	}
	#endregion
	#region ParagraphAlignmentPropertyAccessor
	public class ParagraphAlignmentPropertyAccessor : ParagraphNullablePropertyAccessor<ParagraphAlignment> {
		public ParagraphAlignmentPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override ParagraphAlignment CalculateValueCore(ModelParagraph paragraph) {
			return (ParagraphAlignment)paragraph.Alignment;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, ParagraphAlignment value) {
			DevExpress.XtraRichEdit.Model.ParagraphAlignment val = (DevExpress.XtraRichEdit.Model.ParagraphAlignment)value;
			paragraph.Alignment = val;
		}
	}
	#endregion
	#region ParagraphLineSpacingTypePropertyAccessor
	public class ParagraphLineSpacingTypePropertyAccessor : ParagraphNullablePropertyAccessor<ParagraphLineSpacing> {
		public ParagraphLineSpacingTypePropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override ParagraphLineSpacing CalculateValueCore(ModelParagraph paragraph) {
			return (ParagraphLineSpacing)paragraph.LineSpacingType;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, ParagraphLineSpacing value) {
			DevExpress.XtraRichEdit.Model.ParagraphLineSpacing val = (DevExpress.XtraRichEdit.Model.ParagraphLineSpacing)value;
			paragraph.LineSpacingType = val;
		}
	}
	#endregion
	#region ParagraphLineSpacingPropertyAccessor
	public class ParagraphLineSpacingPropertyAccessor : ParagraphNullablePropertyAccessor<float> {
		public ParagraphLineSpacingPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override float CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.LineSpacing;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, float value) {
			paragraph.LineSpacing = value;
		}
	}
	#endregion
	#region ParagraphFirstLineIndentTypePropertyAccessor
	public class ParagraphFirstLineIndentTypePropertyAccessor : ParagraphNullablePropertyAccessor<ParagraphFirstLineIndent> {
		public ParagraphFirstLineIndentTypePropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override ParagraphFirstLineIndent CalculateValueCore(ModelParagraph paragraph) {
			return (ParagraphFirstLineIndent)paragraph.FirstLineIndentType;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, ParagraphFirstLineIndent value) {
			DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent val = (DevExpress.XtraRichEdit.Model.ParagraphFirstLineIndent)value;
			paragraph.FirstLineIndentType = val;
		}
	}
	#endregion
	#region ParagraphFirstLineIndentPropertyAccessor
	public class ParagraphFirstLineIndentPropertyAccessor : ParagraphNullablePropertyAccessor<int> {
		public ParagraphFirstLineIndentPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override int CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.FirstLineIndent;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, int value) {
			paragraph.FirstLineIndent = value;
		}
	}
	#endregion
	#region ParagraphSuppressHyphenationPropertyAccessor
	public class ParagraphSuppressHyphenationPropertyAccessor : ParagraphNullablePropertyAccessor<bool> {
		public ParagraphSuppressHyphenationPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override bool CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.SuppressHyphenation;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, bool value) {
			paragraph.SuppressHyphenation = value;
		}
	}
	#endregion
	#region ParagraphSuppressLineNumbersPropertyAccessor
	public class ParagraphSuppressLineNumbersPropertyAccessor : ParagraphNullablePropertyAccessor<bool> {
		public ParagraphSuppressLineNumbersPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override bool CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.SuppressLineNumbers;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, bool value) {
			paragraph.SuppressLineNumbers = value;
		}
	}
	#endregion
	#region ParagraphOutlineLevelPropertyAccessor
	public class ParagraphOutlineLevelPropertyAccessor : ParagraphNullablePropertyAccessor<int> {
		public ParagraphOutlineLevelPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override int CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.OutlineLevel;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, int value) {
			paragraph.OutlineLevel = value;
		}
	}
	#endregion
	#region ParagraphKeepLinesTogetherPropertyAccessor
	public class ParagraphKeepLinesTogetherPropertyAccessor : ParagraphNullablePropertyAccessor<bool> {
		public ParagraphKeepLinesTogetherPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override bool CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.KeepLinesTogether;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, bool value) {
			paragraph.KeepLinesTogether = value;
		}
	}
	#endregion
	#region ParagraphPageBreakBeforePropertyAccessor
	public class ParagraphPageBreakBeforePropertyAccessor : ParagraphNullablePropertyAccessor<bool> {
		public ParagraphPageBreakBeforePropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override bool CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.PageBreakBefore;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, bool value) {
			paragraph.PageBreakBefore = value;
		}
	}
	#endregion
	#region ParagraphBackColorPropertyAccessor
	public class ParagraphBackColorPropertyAccessor : ParagraphNullablePropertyAccessor<Color> {
		public ParagraphBackColorPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override Color CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.BackColor;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, Color value) {
			paragraph.BackColor = value;
		}
	}
	#endregion
	#region ParagraphPropertyAccessor<T> (abstract class)
	public abstract class ParagraphPropertyAccessor<T> : SmartPropertyAccessor<T> where T : class {
		readonly ModelParagraphCollection paragraphs;
		protected ParagraphPropertyAccessor(ModelParagraphCollection paragraphs) {
			Guard.ArgumentNotNull(paragraphs, "paragraphs");
			this.paragraphs = paragraphs;
		}
		public ModelParagraphCollection Paragraphs { get { return paragraphs; } }
		protected internal override T CalculateValueCore() {
			ModelParagraphIndex count = new ModelParagraphIndex(Paragraphs.Count);
			if (count <= ModelParagraphIndex.Zero)
				return null;
			T result = CalculateValueCore(Paragraphs[new ModelParagraphIndex(0)]);
			for (ModelParagraphIndex i = new ModelParagraphIndex(1); i < count; i++) {
				T value = CalculateValueCore(Paragraphs[i]);
				if (!Compare(value, result))
					return null;
			}
			return result;
		}
		protected internal virtual bool Compare(T value, T result) {
			return value.Equals(result);
		}
		protected internal override bool SetValueCore(T value) {
			if (value == null)
				return false;
			ModelParagraphIndex count = new ModelParagraphIndex(Paragraphs.Count);
			if (count <= ModelParagraphIndex.Zero)
				return false;
			bool result = SetValueCore(Paragraphs[new ModelParagraphIndex(0)], value);
			for (ModelParagraphIndex i = new ModelParagraphIndex(1); i < count; i++)
				result = SetValueCore(Paragraphs[i], value) || result;
			return result;
		}
		protected internal abstract T CalculateValueCore(ModelParagraph paragraph);
		protected internal abstract bool SetValueCore(ModelParagraph paragraph, T value);
	}
	#endregion
	#region ParagraphOwnTabsPropertyAccessor
	public class ParagraphOwnTabsPropertyAccessor : ParagraphPropertyAccessor<ModelTabFormattingInfo> {
		public ParagraphOwnTabsPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override ModelTabFormattingInfo CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.GetOwnTabs();
		}
		protected internal override bool SetValueCore(ModelParagraph paragraph, ModelTabFormattingInfo value) {
			if (value.Equals(paragraph.GetOwnTabs()))
				return false;
			paragraph.SetOwnTabs(value);
			return true;
		}
	}
	#endregion
	#region ParagraphTabsPropertyAccessor
	public class ParagraphTabsPropertyAccessor : ParagraphOwnTabsPropertyAccessor {
		public ParagraphTabsPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override ModelTabFormattingInfo CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.GetTabs();
		}
	}
	#endregion
	#region ParagraphStylePropertyAccessor
	public class ParagraphStylePropertyAccessor : ParagraphPropertyAccessor<ParagraphStyle> {
		readonly NativeDocument document;
		public ParagraphStylePropertyAccessor(NativeDocument document, ModelParagraphCollection paragraphs)
			: base(paragraphs) {
			this.document = document;
		}
		protected internal override bool Compare(ParagraphStyle value, ParagraphStyle result) {
			return Object.ReferenceEquals(((NativeParagraphStyle)value).InnerStyle, ((NativeParagraphStyle)result).InnerStyle);
		}
		protected internal override ParagraphStyle CalculateValueCore(ModelParagraph paragraph) {
			NativeParagraphStyleCollection styles = (NativeParagraphStyleCollection)document.ParagraphStyles;
			return styles.GetStyle(paragraph.ParagraphStyle);
		}
		protected internal override bool SetValueCore(ModelParagraph paragraph, ParagraphStyle value) {
			ModelParagraphStyle style = value != null ? ((NativeParagraphStyle)value).InnerStyle : null;
			int index = document.DocumentModel.ParagraphStyles.IndexOf(style);
			if (paragraph.GetOwnNumberingListIndex() == ModelNumberingListIndex.NoNumberingList) {
				paragraph.ResetNumberingListIndex(ModelNumberingListIndex.ListIndexNotSetted);
				paragraph.ParagraphProperties.ResetUse(ModelParagraphFormattingOptions.Mask.UseFirstLineIndent | ModelParagraphFormattingOptions.Mask.UseLeftIndent);
			}
			if (index == paragraph.ParagraphStyleIndex)
				return false;
			paragraph.ParagraphStyleIndex = index;
			return true;
		}
	}
	#endregion
	#region ParagraphResetUseAccessor
	public class ParagraphResetUseAccessor : ParagraphNullablePropertyAccessor<bool> {
		public ParagraphResetUseAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override bool CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.ParagraphProperties.Info.Options.Value == DevExpress.XtraRichEdit.Model.ParagraphFormattingOptions.Mask.UseNone;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, bool value) {
			paragraph.ParagraphProperties.ResetAllUse();
		}
	}
	#endregion
	#region ParagraphResetUseMaskAccessor
	public class ParagraphResetUseMaskAccessor : ParagraphNullablePropertyAccessor<ModelParagraphFormattingOptions.Mask> {
		public ParagraphResetUseMaskAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override ModelParagraphFormattingOptions.Mask CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.ParagraphProperties.Info.Options.Value;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, ModelParagraphFormattingOptions.Mask value) {
			paragraph.ParagraphProperties.ResetUse(value);
		}
	}
	#endregion
	#region ParagraphContextualSpacingPropertyAccessor
	public class ParagraphContextualSpacingPropertyAccessor : ParagraphNullablePropertyAccessor<bool> {
		public ParagraphContextualSpacingPropertyAccessor(ModelParagraphCollection paragraphs)
			: base(paragraphs) {
		}
		protected internal override bool CalculateValueCore(ModelParagraph paragraph) {
			return paragraph.ContextualSpacing;
		}
		protected internal override void SetValueCore(ModelParagraph paragraph, bool value) {
			paragraph.ContextualSpacing = value;
		}
	}
	#endregion
	#region NativeParagraphPropertiesBase (abstract class)
	public abstract class NativeParagraphPropertiesBase : ParagraphPropertiesWithTabs {
		#region Fields
		bool isValid = true;
		readonly NativeSubDocument document;
		PropertyAccessor<int?> leftIndent;
		PropertyAccessor<int?> rightIndent;
		PropertyAccessor<int?> spacingBefore;
		PropertyAccessor<int?> spacingAfter;
		PropertyAccessor<ParagraphAlignment?> alignment;
		PropertyAccessor<ParagraphLineSpacing?> lineSpacingType;
		PropertyAccessor<float?> lineSpacing;
		PropertyAccessor<ParagraphFirstLineIndent?> firstLineIndentType;
		PropertyAccessor<int?> firstLineIndent;
		PropertyAccessor<bool?> suppressHyphenation;
		PropertyAccessor<bool?> suppressLineNumbers;
		PropertyAccessor<int?> outlineLevel;
		PropertyAccessor<bool?> keepLinesTogether;
		PropertyAccessor<bool?> pageBreakBefore;
		PropertyAccessor<Color?> backColor;
		PropertyAccessor<bool?> resetAccessor;
		PropertyAccessor<ModelParagraphFormattingOptions.Mask?> resetMaskAccessor;
		PropertyAccessor<bool?> contextualSpacing;
		#endregion
		internal NativeParagraphPropertiesBase(NativeSubDocument document) {
			this.document = document;
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		public NativeSubDocument Document { get { return document; } }
		#region IParagraphProperties Members
		#region LeftIndent
		public float? LeftIndent {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(leftIndent.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (leftIndent.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region RightIndent
		public float? RightIndent {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(rightIndent.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (rightIndent.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region SpacingBefore
		public float? SpacingBefore {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(spacingBefore.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (spacingBefore.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region SpacingAfter
		public float? SpacingAfter {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(spacingAfter.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (spacingAfter.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region Alignment
		public ParagraphAlignment? Alignment {
			get {
				CheckValid();
				return alignment.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (alignment.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineSpacingType
		public ParagraphLineSpacing? LineSpacingType {
			get {
				CheckValid();
				return lineSpacingType.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (lineSpacingType.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region LineSpacing
		public float? LineSpacing {
			get {
				CheckValid();
				return Document.ModelUnitsToUnitsF(lineSpacing.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (lineSpacing.SetValue(Document.UnitsToModelUnitsF(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region LineSpacingMultiplier
		public float? LineSpacingMultiplier {
			get {
				CheckValid();
				return lineSpacing.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (LineSpacingType != ParagraphLineSpacing.Multiple)
					LineSpacingType = ParagraphLineSpacing.Multiple;
				if (lineSpacing.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region FirstLineIndentType
		public ParagraphFirstLineIndent? FirstLineIndentType {
			get {
				CheckValid();
				return firstLineIndentType.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (firstLineIndentType.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region FirstLineIndent
		public float? FirstLineIndent {
			get {
				CheckValid();
				return Document.ModelUnitsToUnits(firstLineIndent.GetValue());
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (firstLineIndent.SetValue(Document.UnitsToModelUnits(value.Value)))
					RaiseChanged();
			}
		}
		#endregion
		#region SuppressHyphenation
		public bool? SuppressHyphenation {
			get {
				CheckValid();
				return suppressHyphenation.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (suppressHyphenation.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region SuppressLineNumbers
		public bool? SuppressLineNumbers {
			get {
				CheckValid();
				return suppressLineNumbers.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (suppressLineNumbers.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region OutlineLevel
		public int? OutlineLevel {
			get {
				CheckValid();
				return outlineLevel.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (outlineLevel.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region KeepLinesTogether
		public bool? KeepLinesTogether {
			get {
				CheckValid();
				return keepLinesTogether.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (keepLinesTogether.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region PageBreakBefore
		public bool? PageBreakBefore {
			get {
				CheckValid();
				return pageBreakBefore.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (pageBreakBefore.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region BackColor
		public Color? BackColor {
			get {
				CheckValid();
				return backColor.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (backColor.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		#region ContextualSpacing
		public bool? ContextualSpacing {
			get {
				CheckValid();
				return contextualSpacing.GetValue();
			}
			set {
				CheckValid();
				if (!value.HasValue)
					return;
				if (contextualSpacing.SetValue(value.Value))
					RaiseChanged();
			}
		}
		#endregion
		public virtual void Reset() {
			CheckValid();
			if (resetAccessor.SetValue(true))
				RaiseChanged();
		}
		public virtual void Reset(ParagraphPropertiesMask mask) {
			CheckValid();
			if (resetMaskAccessor.SetValue((ModelParagraphFormattingOptions.Mask)mask))
				RaiseChanged();
		}
		#endregion
		public TabInfoCollection BeginUpdateTabs(bool onlyOwnTabs) {
			CheckValid();
			PropertyAccessor<ModelTabFormattingInfo> accessor = CreateTabsAccessor(onlyOwnTabs);
			DevExpress.XtraRichEdit.Model.TabFormattingInfo tabInfo = accessor.GetValue();
			if (tabInfo == null)
				return null;
			return CreateTabInfoCollection(document, tabInfo);
		}
		public void EndUpdateTabs(TabInfoCollection tabs) {
			CheckValid();
			if (tabs == null)
				return;
			PropertyAccessor<ModelTabFormattingInfo> accessor = CreateTabsAccessor(true);
			accessor.SetValue(CreateModelTabInfoCollection(document, tabs));
		}
		protected internal static TabInfoCollection CreateTabInfoCollection(NativeSubDocument document, ModelTabFormattingInfo tabs) {
			TabInfoCollection result = new TabInfoCollection();
			int count = tabs.Count;
			for (int i = 0; i < count; i++)
				result.Add(CreateTabInfo(document, tabs[i]));
			return result;
		}
		protected internal static ModelTabFormattingInfo CreateModelTabInfoCollection(NativeSubDocument document, TabInfoCollection tabs) {
			ModelTabFormattingInfo result = new ModelTabFormattingInfo();
			int count = tabs.Count;
			for (int i = 0; i < count; i++)
				result.Add(NativeParagraphProperties.CreateModelTabInfo(document, tabs[i]));
			return result;
		}
		static TabInfo CreateTabInfo(NativeSubDocument document, ModelTabInfo tabInfo) {
			TabInfo result = new TabInfo();
			result.Alignment = (TabAlignmentType)tabInfo.Alignment;
			result.Leader = (TabLeaderType)tabInfo.Leader;
			result.Position = document.ModelUnitsToUnits(tabInfo.Position);
			result.Deleted = tabInfo.Deleted;
			return result;
		}
		static ModelTabInfo CreateModelTabInfo(NativeSubDocument document, TabInfo tabInfo) {
			int position = document.UnitsToModelUnits(tabInfo.Position);
			return new ModelTabInfo(position, (DevExpress.XtraRichEdit.Model.TabAlignmentType)tabInfo.Alignment, (DevExpress.XtraRichEdit.Model.TabLeaderType)tabInfo.Leader, tabInfo.Deleted);
		}
		protected void CheckValid() {
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseInvalidParagraphProperties);
		}
		protected internal virtual void CreateAccessors() {
			this.leftIndent = CreateLeftIndentAccessor();
			this.rightIndent = CreateRightIndentAccessor();
			this.spacingBefore = CreateSpacingBeforeAccessor();
			this.spacingAfter = CreateSpacingAfterAccessor();
			this.alignment = CreateAlignmentAccessor();
			this.lineSpacingType = CreateLineSpacingTypeAccessor();
			this.lineSpacing = CreateLineSpacingAccessor();
			this.firstLineIndentType = CreateFirstLineIndentTypeAccessor();
			this.firstLineIndent = CreateFirstLineIndentAccessor();
			this.suppressHyphenation = CreateSuppressHyphenationAccessor();
			this.suppressLineNumbers = CreateSuppressLineNumbersAccessor();
			this.outlineLevel = CreateOutlineLevelAccessor();
			this.keepLinesTogether = CreateKeepLinesTogetherAccessor();
			this.pageBreakBefore = CreatePageBreakBeforeAccessor();
			this.backColor = CreateBackColorAccessor();
			this.resetAccessor = CreateResetAccessor();
			this.resetMaskAccessor = CreateResetMaskAccessor();
			this.contextualSpacing = CreateContextualSpacingAccessor();
		}
		protected internal abstract void RaiseChanged();
		protected internal abstract PropertyAccessor<int?> CreateLeftIndentAccessor();
		protected internal abstract PropertyAccessor<int?> CreateRightIndentAccessor();
		protected internal abstract PropertyAccessor<int?> CreateSpacingBeforeAccessor();
		protected internal abstract PropertyAccessor<int?> CreateSpacingAfterAccessor();
		protected internal abstract PropertyAccessor<ParagraphAlignment?> CreateAlignmentAccessor();
		protected internal abstract PropertyAccessor<ParagraphLineSpacing?> CreateLineSpacingTypeAccessor();
		protected internal abstract PropertyAccessor<float?> CreateLineSpacingAccessor();
		protected internal abstract PropertyAccessor<ParagraphFirstLineIndent?> CreateFirstLineIndentTypeAccessor();
		protected internal abstract PropertyAccessor<int?> CreateFirstLineIndentAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateSuppressHyphenationAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateSuppressLineNumbersAccessor();
		protected internal abstract PropertyAccessor<int?> CreateOutlineLevelAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateKeepLinesTogetherAccessor();
		protected internal abstract PropertyAccessor<bool?> CreatePageBreakBeforeAccessor();
		protected internal abstract PropertyAccessor<Color?> CreateBackColorAccessor();
		protected internal abstract PropertyAccessor<ModelTabFormattingInfo> CreateTabsAccessor(bool onlyOwnTabs);
		protected internal abstract PropertyAccessor<bool?> CreateResetAccessor();
		protected internal abstract PropertyAccessor<ModelParagraphFormattingOptions.Mask?> CreateResetMaskAccessor();
		protected internal abstract PropertyAccessor<bool?> CreateContextualSpacingAccessor();
	}
	#endregion
	#region NativeParagraphProperties
	public class NativeParagraphProperties : NativeParagraphPropertiesBase, ParagraphProperties, IDocumentModelModifier {
		#region Fields
		readonly ModelParagraphCollection paragraphs;
		PropertyAccessor<ParagraphStyle> style;
		#endregion
		internal NativeParagraphProperties(NativeSubDocument document, ModelParagraphCollection paragraphs)
			: base(document) {
			this.paragraphs = paragraphs;
			CreateAccessors();
		}
		#region Style
		public ParagraphStyle Style {
			get {
				CheckValid();
				return style.GetValue();
			}
			set {
				CheckValid();
				if (value == null)
					return;
				if (style.SetValue(value))
					RaiseChanged();
			}
		}
		#endregion
		#region IDocumentModelModifier Members
		#region Changed
		EventHandler onChanged;
		public event EventHandler Changed { add { onChanged += value; } remove { onChanged -= value; } }
		protected internal override void RaiseChanged() {
			if (onChanged != null)
				onChanged(this, EventArgs.Empty);
		}
		#endregion
		public void ResetCachedValues() {
			CreateAccessors();
		}
		#endregion
		protected internal override void CreateAccessors() {
			base.CreateAccessors();
			NativeDocument document = Document as NativeDocument;
			if (document == null)
				document = Document.MainDocument;
			this.style = new ParagraphStylePropertyAccessor(document, paragraphs);
		}
		protected internal override PropertyAccessor<int?> CreateLeftIndentAccessor() {
			return new ParagraphLeftIndentPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<int?> CreateRightIndentAccessor() {
			return new ParagraphRightIndentPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<int?> CreateSpacingBeforeAccessor() {
			return new ParagraphSpacingBeforePropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<int?> CreateSpacingAfterAccessor() {
			return new ParagraphSpacingAfterPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<ParagraphAlignment?> CreateAlignmentAccessor() {
			return new ParagraphAlignmentPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<ParagraphLineSpacing?> CreateLineSpacingTypeAccessor() {
			return new ParagraphLineSpacingTypePropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<float?> CreateLineSpacingAccessor() {
			return new ParagraphLineSpacingPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<ParagraphFirstLineIndent?> CreateFirstLineIndentTypeAccessor() {
			return new ParagraphFirstLineIndentTypePropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<int?> CreateFirstLineIndentAccessor() {
			return new ParagraphFirstLineIndentPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<bool?> CreateSuppressHyphenationAccessor() {
			return new ParagraphSuppressHyphenationPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<bool?> CreateSuppressLineNumbersAccessor() {
			return new ParagraphSuppressLineNumbersPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<int?> CreateOutlineLevelAccessor() {
			return new ParagraphOutlineLevelPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<bool?> CreateKeepLinesTogetherAccessor() {
			return new ParagraphKeepLinesTogetherPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<bool?> CreatePageBreakBeforeAccessor() {
			return new ParagraphPageBreakBeforePropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<Color?> CreateBackColorAccessor() {
			return new ParagraphBackColorPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<ModelTabFormattingInfo> CreateTabsAccessor(bool onlyOwnTabs) {
			if (onlyOwnTabs)
				return new ParagraphOwnTabsPropertyAccessor(paragraphs);
			else
				return new ParagraphTabsPropertyAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<bool?> CreateResetAccessor() {
			return new ParagraphResetUseAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<ModelParagraphFormattingOptions.Mask?> CreateResetMaskAccessor() {
			return new ParagraphResetUseMaskAccessor(paragraphs);
		}
		protected internal override PropertyAccessor<bool?> CreateContextualSpacingAccessor() {
			return new ParagraphContextualSpacingPropertyAccessor(paragraphs);
		}
	}
	#endregion
	public static class ParagraphHelper {
		public static int CalculateHashCode(Paragraph paragraph) {
			NativeParagraph nativeParagraph = (NativeParagraph)paragraph;
			return nativeParagraph.InnerParagraph.CalculateHashCode();
		}
	}
}

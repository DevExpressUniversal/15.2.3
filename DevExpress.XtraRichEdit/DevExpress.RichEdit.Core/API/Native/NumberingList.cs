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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
#if !SL
using System.Drawing;
using System.Diagnostics;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.API.Native {
	[ComVisible(true)]
	public enum NumberingType {
		Simple = Model.NumberingType.Simple,
		MultiLevel = Model.NumberingType.MultiLevel,
		Bullet = Model.NumberingType.Bullet,
	}
	[ComVisible(true)]
	public interface ListLevel : ListLevelProperties {
		ParagraphPropertiesBase ParagraphProperties { get; }
		CharacterPropertiesBase CharacterProperties { get; }
		bool BulletLevel { get; }
		ParagraphStyle ParagraphStyle { get; set; }
	}
	[ComVisible(true)]
	public interface OverrideListLevel : ListLevel {
		bool OverrideStart { get; }
		int NewStart { get; set; }
		void SetOverrideStart(bool value);
	}
	[ComVisible(true)]
	public interface ReadOnlyListLevelCollection<T> : ISimpleCollection<T> where T : ListLevel {
	}
	[ComVisible(true)]
	public interface ListLevelCollection<T> : ReadOnlyListLevelCollection<T> where T : ListLevel {
		OverrideListLevel Add();
		OverrideListLevel CreateNew();
		void Add(OverrideListLevel level);
	}
	[ComVisible(true)]
	public interface NumberingListBase {
		ReadOnlyListLevelCollection<ListLevel> Levels { get; }
		NumberingType NumberingType { get; set; }
		int Id { get; set; }
	}
	[ComVisible(true)]
	public interface AbstractNumberingList : NumberingListBase {
		int Index { get; }
	}
	[ComVisible(true)]
	public interface NumberingList : NumberingListBase {
		int Index { get; }
		int AbstractNumberingListIndex { get; }
		AbstractNumberingList AbstractNumberingList { get; }
		new ListLevelCollection<OverrideListLevel> Levels { get; }
	}
	[ComVisible(true)]
	public interface TemplateAbstractNumberingList : NumberingListBase {
		AbstractNumberingList CreateNew();
	}
	[ComVisible(true)]
	public interface NumberingListCollection : ISimpleCollection<NumberingList> {
		NumberingList CreateNew(int abstractNumberingListIndex);
		NumberingList Add(int abstractNumberingListIndex);
		void Add(NumberingList list);
	}
	[ComVisible(true)]
	public interface AbstractNumberingListCollection : ISimpleCollection<AbstractNumberingList> {
		TemplateAbstractNumberingList NumberedListTemplate { get; }
		TemplateAbstractNumberingList MultiLevelListTemplate { get; }
		TemplateAbstractNumberingList BulletedListTemplate { get; }
		AbstractNumberingList CreateNew();
		AbstractNumberingList Add();
		void Add(AbstractNumberingList list);
	}
	[ComVisible(true)]
	public interface ListLevelProperties {
		int Start { get; set; }
		NumberingFormat NumberingFormat { get; set; }
		bool ConvertPreviousLevelNumberingToDecimal { get; set; }
		char Separator { get; set; }
		bool SuppressRestart { get; set; }
		bool SuppressBulletResize { get; set; }
		string DisplayFormatString { get; set; }
		int RelativeRestartLevel { get; set; }
	}
	#region NumberingFormat
	[ComVisible(true)]
	public enum NumberingFormat {
		Decimal = DevExpress.Office.NumberConverters.NumberingFormat.Decimal,
		AIUEOHiragana = DevExpress.Office.NumberConverters.NumberingFormat.AIUEOHiragana,
		AIUEOFullWidthHiragana = DevExpress.Office.NumberConverters.NumberingFormat.AIUEOFullWidthHiragana,
		ArabicAbjad = DevExpress.Office.NumberConverters.NumberingFormat.ArabicAbjad,
		ArabicAlpha = DevExpress.Office.NumberConverters.NumberingFormat.ArabicAlpha,
		Bullet = DevExpress.Office.NumberConverters.NumberingFormat.Bullet,
		CardinalText = DevExpress.Office.NumberConverters.NumberingFormat.CardinalText,
		Chicago = DevExpress.Office.NumberConverters.NumberingFormat.Chicago,
		ChineseCounting = DevExpress.Office.NumberConverters.NumberingFormat.ChineseCounting,
		ChineseCountingThousand = DevExpress.Office.NumberConverters.NumberingFormat.ChineseCountingThousand,
		ChineseLegalSimplified = DevExpress.Office.NumberConverters.NumberingFormat.ChineseLegalSimplified,
		Chosung = DevExpress.Office.NumberConverters.NumberingFormat.Chosung,
		DecimalEnclosedCircle = DevExpress.Office.NumberConverters.NumberingFormat.DecimalEnclosedCircle,
		DecimalEnclosedCircleChinese = DevExpress.Office.NumberConverters.NumberingFormat.DecimalEnclosedCircleChinese,
		DecimalEnclosedFullstop = DevExpress.Office.NumberConverters.NumberingFormat.DecimalEnclosedFullstop,
		DecimalEnclosedParenthses = DevExpress.Office.NumberConverters.NumberingFormat.DecimalEnclosedParenthses,
		DecimalFullWidth = DevExpress.Office.NumberConverters.NumberingFormat.DecimalFullWidth,
		DecimalFullWidth2 = DevExpress.Office.NumberConverters.NumberingFormat.DecimalFullWidth2,
		DecimalHalfWidth = DevExpress.Office.NumberConverters.NumberingFormat.DecimalHalfWidth,
		DecimalZero = DevExpress.Office.NumberConverters.NumberingFormat.DecimalZero,
		Ganada = DevExpress.Office.NumberConverters.NumberingFormat.Ganada,
		Hebrew1 = DevExpress.Office.NumberConverters.NumberingFormat.Hebrew1,
		Hebrew2 = DevExpress.Office.NumberConverters.NumberingFormat.Hebrew2,
		Hex = DevExpress.Office.NumberConverters.NumberingFormat.Hex,
		HindiConsonants = DevExpress.Office.NumberConverters.NumberingFormat.HindiConsonants,
		HindiDescriptive = DevExpress.Office.NumberConverters.NumberingFormat.HindiDescriptive,
		HindiNumbers = DevExpress.Office.NumberConverters.NumberingFormat.HindiNumbers,
		HindiVowels = DevExpress.Office.NumberConverters.NumberingFormat.HindiVowels,
		IdeographDigital = DevExpress.Office.NumberConverters.NumberingFormat.IdeographDigital,
		IdeographEnclosedCircle = DevExpress.Office.NumberConverters.NumberingFormat.IdeographEnclosedCircle,
		IdeographLegalTraditional = DevExpress.Office.NumberConverters.NumberingFormat.IdeographLegalTraditional,
		IdeographTraditional = DevExpress.Office.NumberConverters.NumberingFormat.IdeographTraditional,
		IdeographZodiac = DevExpress.Office.NumberConverters.NumberingFormat.IdeographZodiac,
		IdeographZodiacTraditional = DevExpress.Office.NumberConverters.NumberingFormat.IdeographZodiacTraditional,
		Iroha = DevExpress.Office.NumberConverters.NumberingFormat.Iroha,
		IrohaFullWidth = DevExpress.Office.NumberConverters.NumberingFormat.IrohaFullWidth,
		JapaneseCounting = DevExpress.Office.NumberConverters.NumberingFormat.JapaneseCounting,
		JapaneseDigitalTenThousand = DevExpress.Office.NumberConverters.NumberingFormat.JapaneseDigitalTenThousand,
		JapaneseLegal = DevExpress.Office.NumberConverters.NumberingFormat.JapaneseLegal,
		KoreanCounting = DevExpress.Office.NumberConverters.NumberingFormat.KoreanCounting,
		KoreanDigital = DevExpress.Office.NumberConverters.NumberingFormat.KoreanDigital,
		KoreanDigital2 = DevExpress.Office.NumberConverters.NumberingFormat.KoreanDigital2,
		KoreanLegal = DevExpress.Office.NumberConverters.NumberingFormat.KoreanLegal,
		LowerLetter = DevExpress.Office.NumberConverters.NumberingFormat.LowerLetter,
		LowerRoman = DevExpress.Office.NumberConverters.NumberingFormat.LowerRoman,
		None = DevExpress.Office.NumberConverters.NumberingFormat.None,
		NumberInDash = DevExpress.Office.NumberConverters.NumberingFormat.NumberInDash,
		Ordinal = DevExpress.Office.NumberConverters.NumberingFormat.Ordinal,
		OrdinalText = DevExpress.Office.NumberConverters.NumberingFormat.OrdinalText,
		RussianLower = DevExpress.Office.NumberConverters.NumberingFormat.RussianLower,
		RussianUpper = DevExpress.Office.NumberConverters.NumberingFormat.RussianUpper,
		TaiwaneseCounting = DevExpress.Office.NumberConverters.NumberingFormat.TaiwaneseCounting,
		TaiwaneseCountingThousand = DevExpress.Office.NumberConverters.NumberingFormat.TaiwaneseCountingThousand,
		TaiwaneseDigital = DevExpress.Office.NumberConverters.NumberingFormat.TaiwaneseDigital,
		ThaiDescriptive = DevExpress.Office.NumberConverters.NumberingFormat.ThaiDescriptive,
		ThaiLetters = DevExpress.Office.NumberConverters.NumberingFormat.ThaiLetters,
		ThaiNumbers = DevExpress.Office.NumberConverters.NumberingFormat.ThaiNumbers,
		UpperLetter = DevExpress.Office.NumberConverters.NumberingFormat.UpperLetter,
		UpperRoman = DevExpress.Office.NumberConverters.NumberingFormat.UpperRoman,
		VietnameseDescriptive = DevExpress.Office.NumberConverters.NumberingFormat.VietnameseDescriptive
	}
	#endregion
	#region ListNumberAlignment
	[ComVisible(true)]
	public enum ListNumberAlignment {
		Left = DevExpress.XtraRichEdit.Model.ListNumberAlignment.Left,
		Center = DevExpress.XtraRichEdit.Model.ListNumberAlignment.Center,
		Right = DevExpress.XtraRichEdit.Model.ListNumberAlignment.Right
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	#region Usings
	using System.Collections;
	using DevExpress.XtraRichEdit.Utils;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelParagraph = DevExpress.XtraRichEdit.Model.Paragraph;
	using ModelParagraphIndex = DevExpress.XtraRichEdit.Model.ParagraphIndex;
	using ModelAbstractNumberingListCollection = DevExpress.XtraRichEdit.Model.AbstractNumberingListCollection;
	using ModelAbstractNumberingList = DevExpress.XtraRichEdit.Model.AbstractNumberingList;
	using ModelNumberingListCollection = DevExpress.XtraRichEdit.Model.NumberingListCollection;
	using ModelNumberingListIndex = DevExpress.XtraRichEdit.Model.NumberingListIndex;
	using ModelListLevel = Model.ListLevel;
	using ModelNumberingList = Model.NumberingList;
	using DevExpress.XtraRichEdit.Localization;
	#endregion
	#region NativeAbstractNumberingListBase
	public abstract class NativeAbstractNumberingListBase : NumberingListBase {
		NativeReadOnlyListLevelCollection levels;
		readonly NativeDocument document;
		protected NativeAbstractNumberingListBase(NativeDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		protected internal abstract ModelAbstractNumberingList InnerAbstractNumberingList { get; }
		internal NativeDocument Document { get { return document; } }
		internal DocumentModel DocumentModel { get { return document.DocumentModel; } }
		public ReadOnlyListLevelCollection<ListLevel> Levels {
			get {
				if (levels == null)
					levels = new NativeReadOnlyListLevelCollection(document, InnerAbstractNumberingList);
				return levels;
			}
		}
		public virtual NumberingType NumberingType {
			get { return (NumberingType)NumberingListHelper.GetListType(InnerAbstractNumberingList); }
			set {
				if (NumberingType == value)
					return;
				NumberingListHelper.SetListType(InnerAbstractNumberingList, (Model.NumberingType)value);
			}
		}
		public virtual int Id { get { return InnerAbstractNumberingList.Id; } set { InnerAbstractNumberingList.SetId(value); } }
	}
	#endregion
	#region NativeTemplateAbstractNumberingList
	public class NativeTemplateAbstractNumberingList : NativeAbstractNumberingListBase, TemplateAbstractNumberingList {
		NumberingType type;
		public NativeTemplateAbstractNumberingList(NativeDocument document, NumberingType type)
			: base(document) {
			this.type = type;
		}
		internal ModelAbstractNumberingListCollection TemplatesList { get { return Document.DocumentServer.DocumentModelTemplate.AbstractNumberingLists; } }
		protected internal override ModelAbstractNumberingList InnerAbstractNumberingList { get { return NumberingListHelper.GetAbstractListByType(TemplatesList, (Model.NumberingType)type); } }
		public AbstractNumberingList CreateNew() {
			ModelAbstractNumberingList templateList = TemplatesList[NumberingListHelper.GetAbstractListIndexByType(TemplatesList, (Model.NumberingType)this.type)];
			NumberingListIndexCalculator calc = new NumberingListIndexCalculator(DocumentModel, NumberingListHelper.GetListType(templateList));
			AbstractNumberingListIndex listIndex = calc.CreateNewAbstractList(templateList);
			return new NativeAbstractNumberingList(Document, listIndex);
		}
	}
	#endregion
	#region NativeAbstractNumberingList
	public class NativeAbstractNumberingList : NativeAbstractNumberingListBase, AbstractNumberingList {
		readonly AbstractNumberingListIndex innerListIndex;
		public NativeAbstractNumberingList(NativeDocument document, AbstractNumberingListIndex listIndex)
			: base(document) {
			Guard.Equals(listIndex != AbstractNumberingListIndex.InvalidValue, true);
			innerListIndex = listIndex;
		}
		protected internal override ModelAbstractNumberingList InnerAbstractNumberingList { get { return DocumentModel.AbstractNumberingLists[innerListIndex]; } }
		public int Index { get { return ((IConvertToInt<AbstractNumberingListIndex>)innerListIndex).ToInt(); } }
	}
	#endregion
	#region NativeNumberingList
	public class NativeNumberingList : NativeAbstractNumberingListBase, NumberingList {
		readonly ModelNumberingList innerNumberingList;
		readonly NativeOverrideListLevelCollection overrideLevels;
		public NativeNumberingList(NativeDocument document, ModelNumberingListIndex numberingListIndex)
			: this(document, document.DocumentModel.NumberingLists[numberingListIndex]) {
		}
		public NativeNumberingList(NativeDocument document, ModelNumberingList modelNumberingList)
			: base(document) {
			Guard.ArgumentNotNull(modelNumberingList, "modelNumberingList");
			this.innerNumberingList = modelNumberingList;
			this.overrideLevels = new NativeOverrideListLevelCollection(document, modelNumberingList);
		}
		protected internal override ModelAbstractNumberingList InnerAbstractNumberingList { get { return InnerNumberingList.AbstractNumberingList; } }
		internal ModelNumberingList InnerNumberingList { get { return innerNumberingList; } }
		public int AbstractNumberingListIndex { get { return ((IConvertToInt<AbstractNumberingListIndex>)DocumentModel.AbstractNumberingLists.IndexOf(InnerAbstractNumberingList)).ToInt(); } }
		public AbstractNumberingList AbstractNumberingList { get { return Document.AbstractNumberingLists[AbstractNumberingListIndex]; } }
		public new ListLevelCollection<OverrideListLevel> Levels { get { return overrideLevels; } }
		public override NumberingType NumberingType {
			get { return (NumberingType)NumberingListHelper.GetListType(InnerNumberingList); }
			set {
				if (NumberingType == value)
					return;
				NumberingListHelper.SetListType(InnerNumberingList, (Model.NumberingType)value);
			}
		}
		public override int Id { get { return InnerNumberingList.Id; } set { InnerNumberingList.SetId(value); } }
		public int Index { get { return ((IConvertToInt<NumberingListIndex>)DocumentModel.NumberingLists.IndexOf(InnerNumberingList)).ToInt(); } }
	}
	#endregion
	#region NativeNumberedListCollectionBase (abstract class)
	public abstract class NativeNumberedListCollectionBase<TList, TModelList> : ISimpleCollection<TList>
		where TModelList : INumberingListBase
		where TList : class {
		#region Fields
		readonly Dictionary<int, TList> cachedItems;
		readonly NativeDocument document;
		#endregion
		internal NativeNumberedListCollectionBase(NativeDocument document) {
			this.document = document;
			this.cachedItems = new Dictionary<int, TList>();
		}
		#region Properties
		public TList this[int index] { get { return GetList(InnerLists[index]); } }
		public abstract int Count { get; }
		protected internal Model.DocumentModel DocumentModel { get { return document.DocumentModel; } }
		protected internal NativeDocument Document { get { return document; } }
		public abstract IList<TModelList> InnerLists { get; }
		#endregion
		public TList CreateNew() {
			TModelList innerList = CreateModelList();
			AddCore(innerList);
			return GetList(innerList);
		}
		public void Add(TList list) {
			TModelList innerList = GetModelList(list); 
			GetList(innerList);
		}
		public TList Add() {
			TModelList innerList = CreateModelList();
			AddCore(innerList);
			return GetList(innerList);
		}
		protected internal abstract void AddCore(TModelList innerList);
		internal void Invalidate() {
			cachedItems.Clear();
		}
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}
		#endregion
		#region IEnumerable<TList> Members
		IEnumerator<TList> IEnumerable<TList>.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			List<TList> result = new List<TList>();
			int count = InnerLists.Count;
			for (int i = 0; i < count; i++)
				result.Add(this[i]);
			Array.Copy(result.ToArray(), 0, array, index, count);
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
			}
		}
		#endregion
		TList GetList(int hashID, int index) {
			Guard.ArgumentNonNegative(hashID, "ID");
			Guard.ArgumentNonNegative(index, "Index");
			if (index >= Count) {
				if (cachedItems.ContainsKey(hashID))
					cachedItems.Remove(hashID);
				throw new ArgumentException("index");
			}
			TList list;
			if (!cachedItems.TryGetValue(hashID, out list)) {
				list = CreateList(index);
				cachedItems.Add(hashID, list);
			}
			return list;
		}
		TList GetList(TModelList list) {
			int id = GetModelListId(list);
			int index = GetModelListIndex(list);
			return GetList(id, index);
		}
		protected internal abstract TList CreateList(int modelListIndex);
		protected internal abstract TModelList CreateModelList();
		protected internal abstract TModelList GetModelList(TList list);
		protected internal abstract int GetModelListId(TModelList list);
		protected internal abstract int GetModelListIndex(TModelList list);
	}
	#endregion
	public class NativeAbstractNumberingListCollection : NativeNumberedListCollectionBase<AbstractNumberingList, Model.AbstractNumberingList>, AbstractNumberingListCollection {
		TemplateAbstractNumberingList numberedListTemplate;
		TemplateAbstractNumberingList multiLevelListTemplate;
		TemplateAbstractNumberingList bulletedListTemplate;
		public NativeAbstractNumberingListCollection(NativeDocument document)
			: base(document) {
		}
		public override IList<Model.AbstractNumberingList> InnerLists { get { return (IList<Model.AbstractNumberingList>)DocumentModel.AbstractNumberingLists; } }
		ModelAbstractNumberingListCollection InnerModelLists { get { return (ModelAbstractNumberingListCollection)DocumentModel.AbstractNumberingLists; } }
		public override int Count { get { return InnerModelLists.Count; } }
		public TemplateAbstractNumberingList NumberedListTemplate {
			get {
				if (numberedListTemplate == null)
					numberedListTemplate = new NativeTemplateAbstractNumberingList(Document, NumberingType.Simple);
				return numberedListTemplate;
			}
		}
		public TemplateAbstractNumberingList MultiLevelListTemplate {
			get {
				if (multiLevelListTemplate == null)
					multiLevelListTemplate = new NativeTemplateAbstractNumberingList(Document, NumberingType.MultiLevel);
				return multiLevelListTemplate;
			}
		}
		public TemplateAbstractNumberingList BulletedListTemplate {
			get {
				if (bulletedListTemplate == null)
					bulletedListTemplate = new NativeTemplateAbstractNumberingList(Document, NumberingType.Bullet);
				return bulletedListTemplate;
			}
		}
		protected internal override void AddCore(ModelAbstractNumberingList innerList) {
			if (!InnerModelLists.Contains(innerList))
				DocumentModel.AddAbstractNumberingListUsingHistory(innerList);
		}
		protected internal override AbstractNumberingList CreateList(int modelListIndex) {
			return new NativeAbstractNumberingList(Document, new Model.AbstractNumberingListIndex(modelListIndex));
		}
		protected internal override ModelAbstractNumberingList CreateModelList() {
			Model.AbstractNumberingList result = new ModelAbstractNumberingList(DocumentModel);
			DocumentModel.AddAbstractNumberingListUsingHistory(result);
			result.SetId(result.GenerateNewId());
			return result;
		}
		protected internal override ModelAbstractNumberingList GetModelList(AbstractNumberingList list) {
			return ((NativeAbstractNumberingList)list).InnerAbstractNumberingList;
		}
		protected internal override int GetModelListId(ModelAbstractNumberingList list) {
			return list.Id;
		}
		protected internal override int GetModelListIndex(ModelAbstractNumberingList list) {
			return ((IConvertToInt<AbstractNumberingListIndex>)InnerModelLists.IndexOf(list)).ToInt();
		}
		#region AbstractNumberingListCollection Members
		AbstractNumberingList AbstractNumberingListCollection.CreateNew() { return base.CreateNew(); }
		AbstractNumberingList AbstractNumberingListCollection.Add() { return base.Add(); }
		void AbstractNumberingListCollection.Add(AbstractNumberingList list) { base.Add(list); }
		#endregion
	}
	public class NativeNumberingListCollection : NativeNumberedListCollectionBase<NumberingList, Model.NumberingList>, NumberingListCollection {
		Model.AbstractNumberingListIndex lastIndex = Model.AbstractNumberingListIndex.InvalidValue;
		public NativeNumberingListCollection(NativeDocument document)
			: base(document) {
		}
		public override IList<Model.NumberingList> InnerLists { get { return (IList<Model.NumberingList>)DocumentModel.NumberingLists; } }
		Model.NumberingListCollection InnerLists1 { get { return (Model.NumberingListCollection)DocumentModel.NumberingLists; } }
		public override int Count { get { return InnerLists1.Count; } }
		protected internal override void AddCore(Model.NumberingList innerList) {
			if (!InnerLists1.Contains(innerList))
				DocumentModel.AddNumberingListUsingHistory(innerList);
		}
		protected internal override NumberingList CreateList(int modelListIndex) {
			return new NativeNumberingList(Document, new ModelNumberingListIndex(modelListIndex));
		}
		protected internal override Model.NumberingList CreateModelList() {
			Model.NumberingList result = new Model.NumberingList(DocumentModel, lastIndex);
			return result;
		}
		protected internal override Model.NumberingList GetModelList(NumberingList list) {
			return ((NativeNumberingList)list).InnerNumberingList;
		}
		protected internal override int GetModelListId(Model.NumberingList list) {
			return list.Id;
		}
		protected internal override int GetModelListIndex(Model.NumberingList list) {
			return ((IConvertToInt<NumberingListIndex>)InnerLists1.IndexOf(list)).ToInt();
		}
		#region NumberingListCollection Members
		NumberingList NumberingListCollection.CreateNew(int abstractNumberingListIndex) {
			lastIndex = new Model.AbstractNumberingListIndex(abstractNumberingListIndex);
			return base.CreateNew();
		}
		NumberingList NumberingListCollection.Add(int abstractNumberingListIndex) {
			lastIndex = new Model.AbstractNumberingListIndex(abstractNumberingListIndex);
			return base.Add();
		}
		void NumberingListCollection.Add(NumberingList list) {
			lastIndex = AbstractNumberingListIndex.InvalidValue;
			base.Add(list);
		}
		#endregion
	}
	#region NativeReadOnlyListLevelCollection
	public class NativeReadOnlyListLevelCollection : ReadOnlyListLevelCollection<ListLevel> {
		readonly NativeDocument document;
		readonly Dictionary<int, NativeListLevel> cachedItems = new Dictionary<int, NativeListLevel>();
		readonly Model.AbstractNumberingList abstractNumberingList;
		public NativeReadOnlyListLevelCollection(NativeDocument document, Model.AbstractNumberingList abstractNumberingList) {
			this.document = document;
			this.abstractNumberingList = abstractNumberingList;
		}
		Model.ListLevelCollection<Model.ListLevel> InnerListLevelCollection { get { return abstractNumberingList.Levels; } }
		public NativeDocument Document { get { return document; } }
		public int Count { get { return InnerListLevelCollection.Count; } }
		#region First/Last
		public ListLevel First { get { return Count > 0 ? GetItem(0) : null; } } 
		public ListLevel Last { 
			get {
				int count = Count;
				return count > 0 ? GetItem(count - 1) : null;
			}
		}
		#endregion
		#region ISimpleCollection<ListLevel> Members
		ListLevel ISimpleCollection<ListLevel>.this[int index] {
			get { return GetItem(index); }
		}
		#endregion
		#region IEnumerable<ListLevel> Members
		IEnumerator<ListLevel> IEnumerable<ListLevel>.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return GetItem(i);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return GetItem(i);
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			List<ListLevel> result = new List<ListLevel>();
			int count = Count;
			for (int i = 0; i < count; i++)
				result.Add(GetItem(i));
			Array.Copy(result.ToArray(), 0, array, index, count);
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
			}
		}
		#endregion
		#region ListLevel GetItem
		ListLevel GetItem(int index) {
			Guard.ArgumentNonNegative(index, "index");
			if (index >= Count) {
				if (cachedItems.ContainsKey(index))
					cachedItems.Remove(index);
				throw new ArgumentException("index");
			}
			NativeListLevel listlevel;
			if (!cachedItems.TryGetValue(index, out listlevel)) {
				listlevel = new NativeListLevel(Document, abstractNumberingList.Levels[index]);
				cachedItems.Add(index, listlevel);
			}
			return listlevel;
		}
		#endregion
	}
	#endregion
	#region NativeOverrideListLevelCollection
	public class NativeOverrideListLevelCollection : ListLevelCollection<OverrideListLevel> {
		readonly NativeDocument document;
		readonly Dictionary<int, NativeOverrideListLevel> cachedItems = new Dictionary<int, NativeOverrideListLevel>();
		readonly Model.NumberingList innerNumberingList;
		public NativeOverrideListLevelCollection(NativeDocument document, Model.NumberingList modelNumberingList) {
			this.document = document;
			this.innerNumberingList = modelNumberingList;
		}
		Model.ListLevelCollection<Model.IOverrideListLevel> InnerListLevelCollection { get { return innerNumberingList.Levels; } }
		public NativeDocument Document { get { return document; } }
		public int Count { get { return InnerListLevelCollection.Count; } }
		#region First/Last
		public ListLevel First { get { return Count > 0 ? GetItem(0) : null; } } 
		public ListLevel Last { 
			get {
				int count = Count;
				return count > 0 ? GetItem(count - 1) : null;
			}
		}
		#endregion
		#region ISimpleCollection<ListLevel> Members
		OverrideListLevel ISimpleCollection<OverrideListLevel>.this[int index] {
			get { return GetItem(index); }
		}
		#endregion
		#region IEnumerable<ListLevel> Members
		IEnumerator<OverrideListLevel> IEnumerable<OverrideListLevel>.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return GetItem(i);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			for (int i = 0; i < Count; i++)
				yield return GetItem(i);
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			List<ListLevel> result = new List<ListLevel>();
			int count = Count;
			for (int i = 0; i < count; i++)
				result.Add(GetItem(i));
			Array.Copy(result.ToArray(), 0, array, index, count);
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
			}
		}
		#endregion
		#region ListLevel GetItem
		OverrideListLevel GetItem(int index) {
			Guard.ArgumentNonNegative(index, "index");
			if (index >= Count) {
				if (cachedItems.ContainsKey(index))
					cachedItems.Remove(index);
				throw new ArgumentException("index");
			}
			NativeOverrideListLevel listlevel;
			if (!cachedItems.TryGetValue(index, out listlevel)) {
				listlevel = new NativeOverrideListLevel(Document, innerNumberingList.Levels[index]);
				cachedItems.Add(index, listlevel);
			}
			return listlevel;
		}
		#endregion
		public OverrideListLevel Add() {
			NativeOverrideListLevel newLevel = (NativeOverrideListLevel)CreateNew();
			InnerListLevelCollection.Add(newLevel.InnerListLevel);
			return newLevel;
		}
		public OverrideListLevel CreateNew() {
			Model.OverrideListLevel modelListLevel = new Model.OverrideListLevel(Document.DocumentModel);
			return new NativeOverrideListLevel(Document, modelListLevel);
		}
		public void Add(OverrideListLevel level) {
			InnerListLevelCollection.Add(((NativeOverrideListLevel)level).InnerListLevel);
		}
	}
	#endregion
	#region NativeListLevel
	public class NativeListLevel : ListLevel {
		#region Fields
		readonly Model.IListLevel innerListLevel;
		readonly NativeDocument document;
		NativeListLevelParagraphProperties parapgraphProperties = null;
		NativeSimpleCharacterProperties characterProperties = null;
		#endregion
		internal NativeListLevel(NativeDocument document, Model.IListLevel listLevel) {
			this.innerListLevel = listLevel;
			this.document = document;
		}
		public NativeDocument Document { get { return document; } }
		public Model.IListLevel InnerListLevel { get { return innerListLevel; } }
		public ModelPieceTable PieceTable { get { return document.PieceTable; } }
		#region OnChanged reserved
		public void OnChanged() {
		}
		#endregion
		#region ListLevelProperties
		#region Start
		public int Start {
			get { return InnerListLevel.ListLevelProperties.Start; }
			set { InnerListLevel.ListLevelProperties.Start = value; }
		}
		#endregion
		#region Format
		public NumberingFormat NumberingFormat {
			get {
				return (NumberingFormat)InnerListLevel.ListLevelProperties.Format;
			}
			set {
				if (NumberingFormat == value)
					return;
				InnerListLevel.ListLevelProperties.Format = (Office.NumberConverters.NumberingFormat)value;
				OnChanged();
			}
		}
		#endregion
		#region Alignment
		public ListNumberAlignment Alignment {
			get {
				return (ListNumberAlignment)InnerListLevel.ListLevelProperties.Alignment;
			}
			set {
				if (Alignment == value)
					return;
				InnerListLevel.ListLevelProperties.Alignment = (Model.ListNumberAlignment)value;
				OnChanged();
			}
		}
		#endregion
		#region ConvertPreviousLevelNumberingToDecimal
		public bool ConvertPreviousLevelNumberingToDecimal { get { return InnerListLevel.ListLevelProperties.ConvertPreviousLevelNumberingToDecimal; } set { InnerListLevel.ListLevelProperties.ConvertPreviousLevelNumberingToDecimal = value; } }
		#endregion
		#region Separator
		public char Separator { get { return InnerListLevel.ListLevelProperties.Separator; } set { InnerListLevel.ListLevelProperties.Separator = value; } }
		#endregion
		#region SuppressRestart
		public bool SuppressRestart { get { return InnerListLevel.ListLevelProperties.SuppressRestart; } set { InnerListLevel.ListLevelProperties.SuppressRestart = value; } }
		#endregion
		#region SuppressBulletResize
		public bool SuppressBulletResize { get { return InnerListLevel.ListLevelProperties.SuppressBulletResize; } set { InnerListLevel.ListLevelProperties.SuppressBulletResize = value; } }
		#endregion
		#region DisplayFormatString
		public string DisplayFormatString { get { return InnerListLevel.ListLevelProperties.DisplayFormatString; } set { InnerListLevel.ListLevelProperties.DisplayFormatString = value; } }
		#endregion
		#region RelativeRestartLevel
		public int RelativeRestartLevel { get { return InnerListLevel.ListLevelProperties.RelativeRestartLevel; } set { InnerListLevel.ListLevelProperties.RelativeRestartLevel = value; } }
		#endregion
		#region TemplateCode
		public int TemplateCode { get { return InnerListLevel.ListLevelProperties.TemplateCode; } set { InnerListLevel.ListLevelProperties.TemplateCode = value; } }
		#endregion
		#region OriginalLeftIndent
		public int OriginalLeftIndent { get { return InnerListLevel.ListLevelProperties.OriginalLeftIndent; } set { InnerListLevel.ListLevelProperties.OriginalLeftIndent = value; } }
		#endregion
		#endregion
		#region ParapgraphProperties
		public ParagraphPropertiesBase ParagraphProperties {
			get {
				if (parapgraphProperties == null)
					parapgraphProperties = new NativeListLevelParagraphProperties(document, null, InnerListLevel.ParagraphProperties);
				return parapgraphProperties;
			}
		}
		#endregion
		#region BulletLevel
		public bool BulletLevel { get { return InnerListLevel.BulletLevel; } }
		#endregion
		#region ParagraphStyle
		public ParagraphStyle ParagraphStyle {
			get {
				NativeParagraphStyleCollection styles = (NativeParagraphStyleCollection)Document.MainDocument.ParagraphStyles;
				return styles.GetStyle(InnerListLevel.ParagraphStyle);
			}
			set {
				Model.ParagraphStyle style = value != null ? ((NativeParagraphStyle)value).InnerStyle : null;
				((Model.ListLevel)innerListLevel).ParagraphStyleIndex = document.DocumentModel.ParagraphStyles.IndexOf(style);
			}
		}
		#endregion
		#region CharacterProperties
		public CharacterPropertiesBase CharacterProperties {
			get {
				if (characterProperties == null)
					characterProperties = new NativeSimpleCharacterProperties(InnerListLevel.CharacterProperties);
				return characterProperties;
			}
		}
		#endregion
	}
	#endregion
	#region NativeOverrideListLevel
	public class NativeOverrideListLevel : NativeListLevel, OverrideListLevel {
		readonly Model.IOverrideListLevel modelOverrideListLevel;
		public NativeOverrideListLevel(NativeDocument document, Model.IListLevel listLevel)
			: base(document, listLevel) {
			this.modelOverrideListLevel = (Model.IOverrideListLevel)listLevel;
		}
		public new Model.IOverrideListLevel InnerListLevel { get { return modelOverrideListLevel; } }
		#region OverrideListLevel Members
		public bool OverrideStart {
			get { return modelOverrideListLevel.OverrideStart; }
		}
		public int NewStart {
			get { return modelOverrideListLevel.NewStart; }
			set { modelOverrideListLevel.NewStart = value; }
		}
		public void SetOverrideStart(bool value) {
			modelOverrideListLevel.SetOverrideStart(value);
		}
		#endregion
	}
	#endregion
	#region NativeListLevelParagraphProperties
	public class NativeListLevelParagraphProperties : NativeStyleParagraphProperties {
		public NativeListLevelParagraphProperties(NativeDocument document, Model.TabProperties tabs, Model.ParagraphProperties modelProperties)
			: base(document, tabs, modelProperties) {
		}
	}
	#endregion
}

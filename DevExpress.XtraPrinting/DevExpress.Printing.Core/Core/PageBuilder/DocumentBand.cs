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
using System.Linq;
using System.Collections.Specialized;
using System.Drawing;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
#if DXPORTABLE
namespace DevExpress.XtraPrinting.Native {
	public class DocumentBand : IDisposable, IListWrapper<PageBreakInfo>, IListWrapper<Brick>, IListWrapper<DocumentBand> {
#else
using DevExpress.Utils.StoredObjects;
namespace DevExpress.XtraPrinting.Native {
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, Kind = {Kind}}")]
	[System.Diagnostics.DebuggerTypeProxy(typeof(DebuggerHelpers.DocumentBandDebuggerTypeProxy))]
#endif
	public class DocumentBand : IDisposable, IListWrapper<PageBreakInfo>, IListWrapper<Brick>, IListWrapper<DocumentBand> {
		public void StoreBricks(System.IO.BinaryWriter writer, IRepositoryProvider provider) {
			if(bricks == null) 
				return;
			StoredObjectList<Brick> innerList = StoredObjectList<Brick>.CreateInstance(provider, bricks);
			bricks = new BrickList(innerList);
		}
#endif // DXPORTABLE
		#region static
		public const float MaxBandHeightPix = Int32.MaxValue / 500;
		public const int UndefinedFriendLevel = int.MaxValue;
		static object synch = new object();
		static Int32 globalID;
		static BitVector32.Section kindSection = BitVector32.CreateSection(GetMaxValue(typeof(DocumentBandKind)));
		static BitVector32.Section printOnSection = BitVector32.CreateSection(GetMaxValue(typeof(DevExpress.XtraReports.UI.PrintOnPages)), kindSection);
		static readonly int bitHasDetailBands = BitVector32Helper.CreateMask(printOnSection);
		static readonly int bitKeepTogether = BitVector32.CreateMask(bitHasDetailBands);
		static readonly int bitRepeatEveryPage = BitVector32.CreateMask(bitKeepTogether);
		static readonly int bitPrintAtBottom = BitVector32.CreateMask(bitRepeatEveryPage);
		static readonly int bitKeepTogetherOnTheWHolePage = BitVector32.CreateMask(bitPrintAtBottom);
		static readonly int bitTopSpanActive = BitVector32.CreateMask(bitKeepTogetherOnTheWHolePage);
		static Int32 GetGlobalID() {
			if(globalID < Int32.MaxValue)
				return globalID++;
			throw new InvalidOperationException();
		}
		static short GetMaxValue(Type enumType) {
			int result = 0;
			foreach(int value in Enum.GetValues(enumType))
				result |= value;
			return (short)result;
		}
		public static DocumentBand CreateEmptyInstance(DocumentBandKind kind) {
			return new DocumentBand(kind);
		}
		public static DocumentBand CreatePageBreakBand(float pageBreak) {
			DocumentBand pageBreakBand = CreateEmptyInstance(DocumentBandKind.PageBreak);
			pageBreakBand.PageBreaks.Add(new PageBreakInfo(pageBreak));
			return pageBreakBand;
		}
		public static DocumentBand CreateEmptyMarginBand(DocumentBandKind kind, float height) {
			return new MarginDocumentBand(kind, height);
		}
		#endregion
		#region fields & properties
		DocumentBandCollection bands;
		BrickList bricks;
		PageBreakList pageBreaks;
		DocumentBand parent;
		protected DocumentBand primarySource = null;
		float topSpan;
		float bottomSpan;
		int rowIndex = -1;
		float fOffsetX = 0f;
		BitVector32 flags = new BitVector32();
		int friendLevel = UndefinedFriendLevel;
		public virtual object GroupKey {
			get { return null; }
			set { throw new NotSupportedException(); }
		}
		public virtual bool IsGroupItem {
			get { return false; }
		}
		public IBandManager BandManager { 
			set; get;
		}
		public Int32 ID { 
			get; private set;
		}
		public virtual MultiColumn MultiColumn {
			get { return null; }
			set { throw new NotSupportedException(); }
		}
		public int Index {
			get { return Parent != null ? Parent.Bands.IndexOf(this) : -1; }
		}
		public int FriendLevel {
			get { return friendLevel; }
			set { friendLevel = value; }
		}
		public bool IsFriendLevelSet {
			get { return FriendLevel != UndefinedFriendLevel; }
		}
		public PrintOnPages PrintOn {
			get { return (PrintOnPages)flags[printOnSection]; }
			set { flags[printOnSection] = (int)value; }
		}
		public bool HasDetailBands {
			get {
				return flags[bitHasDetailBands];
			}
			set {
				flags[bitHasDetailBands] = value;
			}
		}
		internal DocumentBand PrimarySource { 
			get { return primarySource; } 
		}
		internal DocumentBand PrimaryParent {
			get {
				return parent != null ? parent :
					primarySource != null ? primarySource.Parent :
					null;
			}
		}
		internal float Top { 
			get { return BrickBounds.Top - TopSpan; } 
		}
		protected internal virtual float Bottom { 
			get { return BrickBounds.Bottom + bottomSpan; }
		}
		public virtual float TotalHeight { 
			get { return TotalBottom - TotalTop; }
		}
		internal float TotalTop {
			get {
				float top = float.MaxValue;
				CalcMinTop(ref top, 0);
				if(top == float.MaxValue)
					top = 0;
				return top;
			}
		}
		internal float TotalBottom {
			get {
				float bottom = 0;
				CalcMaxBottom(ref bottom, 0);
				return bottom;
			}
		}
		public virtual float SelfHeight {
			get {
				float selfHeight = TotalBottom;
				return selfHeight != 0 || pageBreaks == null ? selfHeight : pageBreaks.MaxPageBreak;
			}
		}
		public virtual float MinSelfHeight {
			get {
				return 0f;
			}
		}
		public DocumentBandKind Kind { 
			get { return (DocumentBandKind)flags[kindSection]; } 
			set { flags[kindSection] = (int)value; } 
		}
#if DEBUGTEST
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal DocumentBandCollection InnerBands {
			get {
				return bands;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal BrickList InnerBricks {
			get {
				return bricks;
			}
		}
#endif
		public IListWrapper<DocumentBand> Bands {
			get {
				return this;
			}
		}
		public virtual bool ShouldAssignParent {
			get { return true; }
		}
		public IListWrapper<Brick> Bricks { 
			get { return this; } 
		}
		public RectangleF BrickBounds {
			get {
				return bricks != null ? bricks.Bounds : RectangleF.Empty;
			}
		}
		public IListWrapper<PageBreakInfo> PageBreaks {
			get {
				return this;
			}
		}
		public DocumentBand Parent { 
			get { return parent; } 
			set { parent = value; } 
		}
		public bool IsLast {
			get {
				return parent != null && parent.Bands.GetLast<DocumentBand>() == this;
			}
		}
		public bool TopSpanActive {
			get { return flags[bitTopSpanActive]; }
			set { flags[bitTopSpanActive] = value; }
		}
		public float TopSpan { 
			get { return topSpan; } 
			set { topSpan = Math.Max(0, value); } 
		}
		public float BottomSpan {
			get { return bottomSpan; }
			set { bottomSpan = Math.Max(0, value); }
		}
		public int RowIndex { 
			get { return rowIndex; } 
		}
		public bool RepeatEveryPage { 
			get { return flags[bitRepeatEveryPage]; }
			set { flags[bitRepeatEveryPage] = value; } 
		}
		public bool PrintAtBottom { 
			get { return flags[bitPrintAtBottom]; }
			set { flags[bitPrintAtBottom] = value; } 
		}
		public bool IsMarginBand {
			get { return this.IsKindOf(DocumentBandKind.TopMargin, DocumentBandKind.BottomMargin); }
		}
		public virtual bool IsDetailBand {
			get { return false; }
		}
		public virtual bool IsRoot {
			get { return false; }
		}
		public bool KeepTogether {
			get { return flags[bitKeepTogether]; }
			set { flags[bitKeepTogether] = value; }
		}
		public bool KeepTogetherOnTheWholePage {
			get { return flags[bitKeepTogetherOnTheWHolePage]; }
			set { flags[bitKeepTogetherOnTheWHolePage] = value; }
		}
		public float OffsetX { 
			get { return fOffsetX; } 
			set { fOffsetX = value; } 
		}
		public bool IsEmpty {
			get {
				return
					Bricks.Count == 0 &&
					PageBreaks.Count == 0 &&
					Bands.Count == 0;
			}
		}
		public virtual bool HasDataSource { 
			get { return false; } 
		}
		#endregion
		public DocumentBand()
			: this(DocumentBandKind.Detail) {
		}
		public DocumentBand(DocumentBandKind kind)
			: this(kind, -1) {
		}
		public DocumentBand(DocumentBandKind kind, int rowIndex) {
			this.Kind = kind;
			this.rowIndex = rowIndex;
			this.primarySource = this;
			PrintOn = PrintOnPages.AllPages;
			TopSpanActive = true;
			lock(synch)
				ID = GetGlobalID();
		}
		protected DocumentBand(DocumentBand source, int rowIndex)
			: this(source.Kind, rowIndex) {
			primarySource = source.primarySource;
			GenerateContent(source, rowIndex);
			topSpan = source.topSpan;
			TopSpanActive = source.TopSpanActive;
			bottomSpan = source.bottomSpan;
			PrintAtBottom = source.PrintAtBottom;
			fOffsetX = source.fOffsetX;
		}
		public virtual void Dispose() {
			if(bricks != null)
				((IDisposable)bricks).Dispose();
			for(int i = 0; i < Bands.Count; i++)
				Bands[i].Dispose();
			Bands.Clear();
		}
		public void InvalidateBrickBounds() {
			if(bricks != null)
				bricks.InvalidateBounds();
		}
		public virtual bool ProcessIsFinished(ProcessState processState, int bandIndex) {
			return true;
		}
		public int GetPageBreakIndex(float value) {
			for(int i = 0; i < PageBreaks.Count; i++) {
				if(PageBreaks[i].Value == value)
					return i;
			}
			return -1;
		}
		public virtual void Scale(double scaleFactor, Predicate<DocumentBand> shouldScale) {
			if(shouldScale != null && !shouldScale(this))
				return;
			this.OffsetX = MathMethods.Scale(this.OffsetX, scaleFactor);
			this.TopSpan = MathMethods.Scale(this.TopSpan, scaleFactor);
			this.BottomSpan = MathMethods.Scale(this.BottomSpan, scaleFactor);
			ScaleContent(scaleFactor);
			foreach(DocumentBand band in this.Bands) {
				if(band.IsMarginBand)
					continue;
				band.Scale(scaleFactor, shouldScale);
			}
		}
		protected void ScaleContent(double ratio) {
			if(ratio == 1d)
				return;
			foreach(Brick brick in this.Bricks) {
				if(brick.HasModifier(BrickModifier.MarginalHeader, BrickModifier.MarginalFooter))
					continue;
				brick.Scale(ratio);
			}
			foreach(ValueInfo pbInfo in this.PageBreaks)
				pbInfo.Value *= (float)ratio;
			InvalidateBrickBounds();
		}
		public virtual void Reset(bool shouldResetBricksOffset, bool pageBreaksActiveStatus) {
			TopSpanActive = true;
			if(shouldResetBricksOffset) {
				foreach(Brick brick in Bricks)
					brick.PageBuilderOffset = PointF.Empty;
			}
			foreach(ValueInfo pageBreak in PageBreaks) {
				pageBreak.Active = pageBreaksActiveStatus;
			}
			foreach(DocumentBand docBand in Bands) {
				docBand.Reset(shouldResetBricksOffset, pageBreaksActiveStatus);
			}
			InvalidateBrickBounds();
		}
		public DocumentBand GetDataSourceRoot() {
			return this.AllPrimaryParents().FirstOrDefault<DocumentBand>(item => item.HasDataSource);
		}
		public DocumentBand FindTopLevelBand() {
			DocumentBand band = this;
			DocumentBand result = null;
			while(!band.IsRoot) {
				if(!(band is DocumentBandContainer)) {
					result = band;
					if(band.IsKindOf(DocumentBandKind.PageBand))
						break;
				}
				band = band.PrimaryParent;
			}
			return result;
		}
		public virtual DocumentBand GetInstance(int rowIndex) {
			return new DocumentBand(this, rowIndex);
		}
		protected virtual void GenerateContent(DocumentBand source, int rowIndex) {
			if(source.Bands.Count > 0) {
				foreach(DocumentBand docBand in source.Bands) {
					int rowIndex2 = rowIndex >= 0 ? rowIndex : docBand.RowIndex;
					Bands.Add(docBand.GetInstance(rowIndex2));
				}
			} else {
				foreach(Brick brick in source.Bricks) {
					if(brick.IsPageBrick) {
						Bricks.Add(brick);
					} else {
						Brick brick1 = new BrickContainerBase(brick);
						brick1.Initialize(brick.PrintingSystem, brick.InitialRect);
						Bricks.Add(brick1);
					}
				}
				foreach(ValueInfo pbInfo in source.PageBreaks)
					this.PageBreaks.Add(new PageBreakInfo(pbInfo.Value));
			}
		}
		public bool KindEquals(DocumentBandKind kind) {
			return this.Kind == kind;
		}
		public bool IsKindOf(params DocumentBandKind[] kinds) {
			foreach(DocumentBandKind kind in kinds)
				if((this.Kind & kind) == kind)
					return true;
			return false;
		}
		void CalcMinTop(ref float top, float topSpan) {
			float totalTopSpan = topSpan + this.TopSpan;
			if(Bricks.Count > 0)
				top = Math.Min(top, BrickBounds.Top - totalTopSpan);
			else
				foreach(DocumentBand band in Bands)
					band.CalcMinTop(ref top, totalTopSpan);
		}
		void CalcMaxBottom(ref float bottom, float bottomSpan) {
			float totalBottomSpan = bottomSpan + this.bottomSpan;
			if(Bricks.Count > 0)
				bottom = Math.Max(bottom, BrickBounds.Bottom + totalBottomSpan);
			else
				foreach(DocumentBand band in Bands)
					band.CalcMaxBottom(ref bottom, totalBottomSpan);
		}
		public void ApplySpans() {
			this.ApplySpans(this.BrickBounds.Bottom);
		}
		public void ApplySpans(float height) {
			if(Bricks.Count > 0 || PageBreaks.Count > 0) {
				TopSpan = BrickBounds.Top;
				bottomSpan = height - BrickBounds.Bottom;
			}
			else {
				if(Bands.Count > 0) {
					TopSpan = 0;
					bottomSpan = height;
				}
				else
					TopSpan = bottomSpan = 0;
			}
		}
		public void SetParents() {
			foreach(DocumentBand band in Bands) {
				band.parent = this;
				band.SetParents();
			}
		}
		internal void AddBand(DocumentBand docBand) {
			docBand.parent = this;
			this.Bands.Add(docBand);
		}
		internal void InsertBand(DocumentBand docBand, int index) {
			docBand.parent = this;
			this.Bands.Insert(docBand, index);
		}
		internal protected virtual void BeforeBuild() {
		}
		internal protected virtual void AfterBuild() {
		}
		public void SortBands(IComparer<DocumentBand> comparer) {
			if(bands != null)
				bands.Sort(comparer);
		}
		public DocumentBand GetBand(DocumentBandKind kind) {
			return bands != null ? bands[kind] : null;
		}
		public DocumentBand GetPageBand(DocumentBandKind kind) {
			return bands != null ? bands.FindBand(kind, band => band.IsPageBand(kind)) : null;
		}
		public int GetRowIndex(int rowIndex) {
			return this.IsKindOf(DocumentBandKind.PageBand) ? rowIndex : this.RowIndex;
		}
		public bool IsPageBand(DocumentBandKind kind) {
			if(kind != DocumentBandKind.Header && kind != DocumentBandKind.Footer)
				throw new ArgumentException("kind");
			return this.IsKindOf(kind) && (this.IsKindOf(DocumentBandKind.PageBand) || this.RepeatEveryPage);
		}
		public bool HasDetailBandBehaviour {
			get { return IsKindOf(DocumentBandKind.Detail) || (IsKindOf(DocumentBandKind.Footer) && !IsPageBand(DocumentBandKind.Footer)); }
		}
		public bool IsValid {
			get { return this.Bands.Count > 0 || this.Bricks.Count > 0 || this.PageBreaks.Count > 0; }
		}
		public bool HasReportHeader() {
			return IsReportHeaderOnPosition(0) || IsReportHeaderOnPosition(1);
		}
		bool IsReportHeaderOnPosition(int index) {
			return Bands.Count > index && Bands[index].IsKindOf(DocumentBandKind.ReportHeader);
		}
		public DocumentBand GetSubRoot() {
			return PrimaryParent.AllPrimaryParents().FirstOrDefault<DocumentBand>(item => item is RootDocumentBand || item is ISubreportDocumentBand);
		}
		public DocumentBand GetBand(int index) {
			return GetBand(index, RectangleF.Empty, PointF.Empty);
		}
		public DocumentBand GetBand(int index, RectangleF bounds, PointF offset) {
			return BandManager != null && !BandManager.IsCompleted ? BandManager.GetBand(this, new PageBuildInfo(index, bounds, offset)) :
				(index >= 0 && index < Bands.Count) ? Bands[index] : null;
		}
		public DocumentBand GetBand(PageBuildInfo pageBuildInfo) {
			return BandManager != null && !BandManager.IsCompleted ? BandManager.GetBand(this, pageBuildInfo) : pageBuildInfo.Index < Bands.Count ? Bands[pageBuildInfo.Index] : null;
		}
		public DocumentBand GetPageFooterBand() {
			return BandManager != null && !BandManager.IsCompleted ? BandManager.GetPageFooterBand(this) : GetPageBand(DocumentBandKind.Footer);
		}
		public void EnsureReportFooterBand() {
			if(BandManager != null && !BandManager.IsCompleted)
				BandManager.EnsureReportFooterBand(this);
		}
		public void EnsureGroupFooter() {
			if(BandManager != null && !BandManager.IsCompleted)
				BandManager.EnsureGroupFooter(this);
		}
		public bool HasBands(RectangleF bounds, PointF offset) {
			return Bands.Count > 0 || GetBand(0, bounds, offset) != null;
		}
		public void GenerateBandChildren() {
			int i = 0;
			DocumentBand docBand = GetBand(0);
			while(docBand != null) {
				docBand.GenerateBandChildren();
				docBand = GetBand(++i);
			}
		}
		public virtual bool ContainsDetailBands() {
			return false;
		}
		public bool HasActivePageBreaks {
			get {
				foreach(ValueInfo pageBreak in PageBreaks)
					if(pageBreak.Active)
						return true;
				return false;
			}
		}
		public DocumentBand GetDetailBand() {
			if(Bands.Count > 0 && Bands[0] is DetailDocumentBand)
				return Bands[0];
			return this;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotSupportedException();
		}
		#region ISimpleList<PageBreakInfo> Members
		static object[] emptyArray = new object[] { };
		int IListWrapper<PageBreakInfo>.IndexOf(PageBreakInfo pageBreak) {
			throw new NotSupportedException();
		}
		void IListWrapper<PageBreakInfo>.Insert(PageBreakInfo pageBreak, int index) {
			throw new NotSupportedException();
		}
		int IListWrapper<PageBreakInfo>.Count {
			get {
				return pageBreaks != null ? pageBreaks.Count : 0;
			}
		}
		PageBreakInfo IListWrapper<PageBreakInfo>.this[int index] {
			get {
				if(pageBreaks != null)
					return pageBreaks[index];
				throw new IndexOutOfRangeException();
			}
		}
		void IListWrapper<PageBreakInfo>.Add(PageBreakInfo item) {
			if(pageBreaks == null)
				pageBreaks = new PageBreakList();
			pageBreaks.Add(item);
		}
		void IListWrapper<PageBreakInfo>.RemoveAt(int index) {
			if(pageBreaks != null) {
				pageBreaks.RemoveAt(index);
				if(pageBreaks.Count == 0)
					pageBreaks = null;
			}
		}
		void IListWrapper<PageBreakInfo>.Clear() {
			if(pageBreaks != null)
				pageBreaks.Clear();
			pageBreaks = null;
		}
		IEnumerator<PageBreakInfo> IEnumerable<PageBreakInfo>.GetEnumerator() {
			return pageBreaks != null ? ((IEnumerable<PageBreakInfo>)pageBreaks).GetEnumerator() : new Enumerator<PageBreakInfo>(emptyArray);
		}
		#endregion
		#region ISimpleList<Brick> Members
		void IListWrapper<Brick>.Insert(Brick brick, int index) {
			throw new NotSupportedException();
		}
		int IListWrapper<Brick>.Count {
			get {
				return bricks != null ? bricks.Count : 0;
			}
		}
		Brick IListWrapper<Brick>.this[int index] {
			get {
				if(bricks != null)
					return bricks[index];
				throw new IndexOutOfRangeException();
			}
		}
		void IListWrapper<Brick>.Add(Brick item) {
			if(bricks == null)
				bricks = new BrickList();
			if(Kind == DocumentBandKind.TopMargin)
				item.SetModifierRecursive(BrickModifier.MarginalHeader);
			else if(Kind == DocumentBandKind.BottomMargin)
				item.SetModifierRecursive(BrickModifier.MarginalFooter);
			bricks.Add(item);
		}
		void IListWrapper<Brick>.RemoveAt(int index) {
			if(bricks != null) {
				bricks.RemoveAt(index);
				if(bricks.Count == 0)
					bricks = null;
			}
		}
		void IListWrapper<Brick>.Clear() {
			if(bricks != null)
				bricks.Clear();
			bricks = null;
		}
		IEnumerator<Brick> IEnumerable<Brick>.GetEnumerator() {
			return bricks != null ? ((IEnumerable<Brick>)bricks).GetEnumerator() : new Enumerator<Brick>(emptyArray);
		}
		int IListWrapper<Brick>.IndexOf(Brick brick) {
			return bricks != null ? bricks.IndexOf(brick) : -1;
		}
		#endregion
		#region IDocumentBandCollection Members
		int IListWrapper<DocumentBand>.IndexOf(DocumentBand band) {
			return bands != null ? bands.FastIndexOf(band) : -1;
		}
		void IListWrapper<DocumentBand>.Insert(DocumentBand band, int index) {
			if(bands == null)
				bands = new DocumentBandCollection(this);
			bands.Insert(index, band);
		}
		int IListWrapper<DocumentBand>.Count {
			get {
				return bands != null ? bands.Count : 0;
			}
		}
		DocumentBand IListWrapper<DocumentBand>.this[int index] {
			get {
				if(bands != null)
					return bands[index];
				throw new IndexOutOfRangeException();
			}
		}
		void IListWrapper<DocumentBand>.Add(DocumentBand item) {
			if(bands == null)
				bands = new DocumentBandCollection(this);
			bands.Add(item);
		}
		void IListWrapper<DocumentBand>.RemoveAt(int index) {
			if(bands != null) {
				bands.RemoveAt(index);
				if(bands.Count == 0)
					bands = null;
			}
		}
		void IListWrapper<DocumentBand>.Clear() {
			if(bands != null)
				bands.Clear();
			bands = null;
		}
		IEnumerator<DocumentBand> IEnumerable<DocumentBand>.GetEnumerator() {
			return bands != null ? ((IEnumerable<DocumentBand>)bands).GetEnumerator() : new Enumerator<DocumentBand>(emptyArray);
		}
		#endregion
	}
	public static class DocumentBandExtensions {
		public static PrintingSystemBase GetPrintingSystem(this DocumentBand docBand) {
			if(docBand.BandManager != null)
				return docBand.BandManager.PrintingSystem;
			foreach(DocumentBand item in docBand.AllPrimaryParents()) {
				if(item.BandManager != null)
					return item.BandManager.PrintingSystem;
				else if(item is RootDocumentBand)
					return ((RootDocumentBand)item).PrintingSystem;
			}
			return null;
		}
		public static bool IsFooter(this DocumentBandKind kind) {
			return (kind & DocumentBandKind.Footer) > 0 || (kind & DocumentBandKind.BottomMargin) > 0 || (kind & DocumentBandKind.ReportFooter) > 0;
		}
		public static bool IsHeader(this DocumentBandKind kind) {
			return (kind & DocumentBandKind.Header) > 0 || (kind & DocumentBandKind.TopMargin) > 0 || (kind & DocumentBandKind.ReportHeader) > 0;
		}
		public static IEnumerable<DocumentBand> AllPrimaryParents(this DocumentBand band) {
			DocumentBand item = band;
			while(item != null) {
				yield return item;
				item = item.PrimaryParent;
			}
		}
		public static int[] GetPath(this DocumentBand band) {
			List<int> indices = new List<int>();
			CollectIndices(indices, band.PrimarySource);
			return indices.ToArray();
		}
		static void CollectIndices(IList<int> indices, DocumentBand documentBand) {
			if(documentBand == null || documentBand.Index < 0) return;
			indices.Insert(0, documentBand.Index);
			CollectIndices(indices, documentBand.Parent);
		}
		public static int GetBandsCountRecursive(this DocumentBand band) {
			int count = band.Bands.Count;
			foreach(DocumentBand childBand in band.Bands)
				count += GetBandsCountRecursive(childBand);
			return count;
		}
		public static DocumentBand CopyBand(this DocumentBand band, int rowIndex) {
			return band != null ? band.GetInstance(rowIndex) : null;
		}
		public static int GetID(this DocumentBand band) {
			return band != null ? band.ID : -1;
		}
	}
}

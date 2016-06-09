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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel.Design;
using DevExpress.Utils;
using DevExpress.Data.Mask;
using System.ComponentModel;
using DevExpress.XtraPrinting.Preview;
#if SL
using DevExpress.Xpf.Collections;
using System.Windows.Input;
#else
using System.Windows.Forms;
using DevExpress.Utils.StoredObjects;
#endif
namespace DevExpress.XtraPrinting.Native {
	public abstract class PrintingDocument : Document, IDocumentProxy {
		public readonly static SizeF MinPageSize = new SizeF(31.25f, 31.25f);
		const string documentCreatingMessage = "This property can't be changed because document is creating.";
		RootDocumentBand root;
		Action0 afterBuild;
		float savedScaleFactor = 1f;
		float scaleFactor = 1f;
		int autoFitToPagesWidth;
		DocumentHelper documentHelper;
		bool buildPagesInBackground;
		NavigationInfo navigationInfo;
		string infoString = string.Empty;
		RectangleF usefulPageRectF = RectangleF.Empty;
		RepositoryProvider repositoryProvider;
		public IRepositoryProvider RepositoryProvider {
			get {
				if(repositoryProvider == null)
					repositoryProvider = RepositoryProviderExtensions.CreateInstance();
				return repositoryProvider;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string InfoString {
			get { return infoString; }
			set {
				if(string.IsNullOrEmpty(infoString))
					infoString = value;
			}
		}
		internal NavigationInfo NavigationInfo {
			get { return navigationInfo; }
			set {
				if(navigationInfo != null)
					navigationInfo.Clear();
				navigationInfo = value;
			}
		}
		public override int PageCount {
			get {
				return documentHelper == null ? base.PageCount : documentHelper.PageCount;
			}
		}
		IEnumerable<Tuple<DocumentBand, Brick>> AllBricks(Predicate<DocumentBand> predicate) {
			DocumentBandEnumerator en = new DocumentBandEnumerator(root.Bands.GetEnumerator());
			while(en.MoveNext()) {
				if(predicate(en.Current)) {
					foreach(Brick item in en.Current.Bricks)
						yield return new Tuple<DocumentBand, Brick>(en.Current, item);
				}
			}
		}
		public override int AutoFitToPagesWidth {
			get { return autoFitToPagesWidth; }
			set {
				value = Math.Max(0, value);
				if(value != autoFitToPagesWidth) {
					autoFitToPagesWidth = value;
					if(this.IsCreated && autoFitToPagesWidth > 0) {
						if(SetScaleFactor(AutoFitScaleFactor))
							ps.OnScaleFactorChanged();
					}
				}
			}
		}
		bool IDocumentProxy.SmartXDivision {
			get {
				return VerticalContentSplitting == VerticalContentSplitting.Smart &&
					this.AutoFitToPagesWidth == 0;
			}
		}
		bool IDocumentProxy.SmartYDivision {
			get {
				return HorizontalContentSplitting == HorizontalContentSplitting.Smart;
			}
		}
		public VerticalContentSplitting VerticalContentSplitting {
			get; set;
		}
		public HorizontalContentSplitting HorizontalContentSplitting {
			get; set;
		}
		public bool BookmarkDuplicateSuppress {
			get; set;
		}
		float AutoFitScaleFactor {
			get {
				float bricksRight = GetBricksRight();
				return ((int)bricksRight) == 0 ?
					scaleFactor :
					scaleFactor * UsefulPageRect.Width * autoFitToPagesWidth / bricksRight * 0.99999f; 
			}
		}
		public override float ScaleFactor {
			get { return scaleFactor; }
			set {
				if(this.IsCreating)
					throw new Exception(documentCreatingMessage);
				autoFitToPagesWidth = 0;
				if(SetScaleFactor(value))
					ps.OnScaleFactorChanged();
			}
		}
		protected internal override RectangleF PageFootBounds { get { return RectangleF.Empty; } }
		protected internal override RectangleF PageHeadBounds { get { return RectangleF.Empty; } }
		protected internal override float MinPageHeight { get { return MinPageSize.Height; } }
		protected internal override float MinPageWidth { get { return MinPageSize.Width; } }
		internal override bool CanPerformContinuousExport { get { return Root != null && !Pages.ContainsExternalPages; } }
		public RootDocumentBand Root { get { return root; } }
		public bool CasheContent { 
			get; set; 
		}
		protected PrintingDocument(PrintingSystemBase ps, RootDocumentBand root, Action0 afterBuild)
			: base(ps) {
			this.root = root;
			this.afterBuild = afterBuild;
			BookmarkDuplicateSuppress = true;
			HorizontalContentSplitting = HorizontalContentSplitting.Exact;
			VerticalContentSplitting = VerticalContentSplitting.Exact;
			CasheContent = false;
		}
		protected internal override void Dispose(bool disposing) {
			try {
				if (disposing) {
					state = DocumentState.Empty;
					DisposeDocumentHelper();
					ProgressReflector.Reset();
					SetRoot(null);
					afterBuild = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		void DisposeDocumentHelper() {
			if(documentHelper != null) {
				documentHelper.Dispose();
				documentHelper = null;
			}
		}
		protected override PageList CreatePageList() {
			return CasheContent ? CreateStoredPageList(RepositoryProvider) :
				new PageList(this);
		}
		PageList CreateStoredPageList(IRepositoryProvider provider) {
			return new PageList(this, StoredObjectList<Page>.CreateInstance(provider, new Page[0]));
		}
		protected internal override ContinuousExportInfo GetContinuousExportInfo() {
			return new ExportPageBuildEngine(this).CreateContinuousExportInfo();
		}
		protected override object[] GetAvailableExportModes(Type exportModeType) {
			return Pages.ContainsExternalPages || !CanPerformContinuousExport ?
				base.GetAvailableExportModes(exportModeType) :
				DevExpress.Data.Utils.Helpers.GetEnumValues(exportModeType);
		}
		protected internal override void StopPageBuilding() {
			ICancellationService serv = ((IServiceProvider)ps).GetService(typeof(ICancellationService)) as ICancellationService;
			if(serv != null && serv.TokenSource != null) serv.TokenSource.Cancel();
			if(documentHelper != null)
				documentHelper.StopPageBuilding();
		}
		public void CopyScaleParameters(PrintingDocument document) {
			if(document == null)
				return;
			this.autoFitToPagesWidth = document.autoFitToPagesWidth;
			this.scaleFactor = document.scaleFactor;
		}
		protected void SetRoot(RootDocumentBand root) {
			if(this.root != null)
				this.root.Dispose();
			this.root = root;
		}
		protected internal override void AddBrick(Brick brick) {
		}
		protected internal override void Begin() {
			base.Begin();
			state = DocumentState.Creating;
			isCreated = false;
			savedScaleFactor = scaleFactor;
			scaleFactor = 1f;
		}
		bool ShouldScale {
			get {
				return AutoFitToPagesWidth > 0 || !FloatsComparer.Default.FirstEqualsSecond(savedScaleFactor, scaleFactor);
			}
		}
		protected internal override void End(bool buildPagesInBackground) {
			this.buildPagesInBackground = buildPagesInBackground;
			if(AutoFitToPagesWidth == 0) 
				SetScaleFactor(savedScaleFactor);
			BuildPages();
		}
		void ApplyAutoFitToPagesWidth() {
			float autoScaleFactor = AutoFitScaleFactor;
			if(!FloatsComparer.Default.FirstEqualsSecond(autoScaleFactor, scaleFactor)) {
				SetScaleFactor(autoScaleFactor);
				BuildPages();
			}
		} 
		protected internal override void HandleNewScaleFactor() {
			BuildPages();
		}
		protected internal override void HandleNewPageSettings() {
			if(AutoFitToPagesWidth > 0 && this.root != null)
				SetScaleFactor(AutoFitScaleFactor);
			if(usefulPageRectF != RectangleF.Empty && usefulPageRectF == this.ps.PageSettings.UsefulPageRectF) 
				return;
			BuildPages();
		}
		protected override void Clear() {
			if(navigationInfo != null)
				navigationInfo.Clear();
			if(this.root != null && !this.root.IsEmpty)
				SetRoot(new RootDocumentBand(this.PrintingSystem));
			if(repositoryProvider != null) {
				repositoryProvider.Dispose();
				repositoryProvider = null;
			}
			base.Clear();
		}
		float GetBricksRight() {
			float right = 0;
			foreach(Tuple<DocumentBand, Brick> t in AllBricks(band => !band.IsKindOf(DocumentBandKind.BottomMargin, DocumentBandKind.TopMargin))) {
				DocumentBand tBand = t.Item1;
				Brick tBrick = t.Item2;
				if(tBrick.Modifier != BrickModifier.MarginalHeader && tBrick.Modifier != BrickModifier.MarginalFooter)
					right = Math.Max(right, GetBrickRight(tBrick, tBand));
			}
			return right;
		}
		static float GetBrickRight(Brick brick, DocumentBand band) {
			float right = brick.InitialRect.Right;
			while(band != null) {
				right += band.OffsetX;
				band = band.Parent;
			}
			return right;
		}
		bool SetScaleFactor(float scaleFactor) {
			double ratio = (double)scaleFactor / (double)this.scaleFactor;
			if(root != null && ratio != 1) {				
				root.Scale(ratio, null);
				this.scaleFactor = scaleFactor;
				return true;
			}
			return false;
		}
		public void BuildPages() {
			this.PrintingSystem.OnBeforeBuildPages(EventArgs.Empty);
			if(documentHelper != null)
				documentHelper.Dispose();
			documentHelper = CreateDocumentHelper();
			state = DocumentState.Creating;
			usefulPageRectF = this.ps.PageSettings.UsefulPageRectF;
			documentHelper.BuildPages();
		}
		protected internal virtual void AfterBuild() {
			try {
				if(root == null) {
					state = DocumentState.Empty;
					PrintingSystem.OnAfterBuildPages(EventArgs.Empty);
					return;
				}
				root.Completed = true;
				state = DocumentState.PostProcessing;
				ProgressReflector.FinalizeRangeCount();
				ProgressReflector.InitializeRange(Pages.Count);
				AfterPrintOnPages();
				OnContentChanged();
				state = DocumentState.Created;
				isCreated = true;
				SetModified(false);
				if(afterBuild != null)
					afterBuild();
				AfterBuildPages();
				ProgressReflector.MaximizeRange();
			} finally {
				ProgressReflector.Reset();
			}
		}
		protected void AfterBuildPages() {
			PrintingSystem.OnAfterBuildPages(EventArgs.Empty);
			if(AutoFitToPagesWidth > 0)
				ApplyAutoFitToPagesWidth();
		}
		void AfterPrintOnPages() {
			MergeBrickHelper mergeHelper = PrintingSystem.GetService<MergeBrickHelper>();
			for(int i = 0; i < Pages.Count; i++) {
				Page page = Pages[i];
				page.Initialize();
				List<int> indices = new List<int>(10);
				page.AfterPrintOnPage(indices, i, Pages.Count, brick => {
					if(NavigationInfo != null && brick is VisualBrick)
						NavigationInfo.UpdateTargets(page, i, (VisualBrick)brick, indices.ToArray());
				});
				if(mergeHelper != null) 
					mergeHelper.ProcessPage(PrintingSystem, (PSPage)page);								
				ProgressReflector.RangeValue++;
			}
		}
		DocumentHelper CreateDocumentHelper() {
			PageBuildEngine engine = CreatePageBuildEngine(buildPagesInBackground, ps.PageSettings.RollPaper);
			return engine is BackgroundPageBuildEngine ? new BackgroundDocumentHelper(this, (BackgroundPageBuildEngine)engine) : new DocumentHelper(this, engine);
		}
		protected virtual PageBuildEngine CreatePageBuildEngine(bool buildPagesInBackground, bool rollPaper) {
			if(rollPaper)
				return new DevExpress.XtraPrinting.Native.PageBuilder.RollPageBuildEngine(this);
			if(buildPagesInBackground) {
				BackgroundPageBuildEngineStrategy strategy = ((IServiceProvider)PrintingSystem).GetService(typeof(BackgroundPageBuildEngineStrategy)) as BackgroundPageBuildEngineStrategy;
				if(strategy == null)
					strategy = new ApplicationBackgroundPageBuildEngineStrategy();
				return new BackgroundPageBuildEngine(this, strategy);
			}
			return new PageBuildEngine(Root, this);
		}
		void IDocumentProxy.AddPage(PSPage page) {
			System.Diagnostics.Debug.Assert(documentHelper != null);
			documentHelper.AddPage(page);
		}
	}
	class ListComparer<T> : IComparer<IList<T>> {
		public int Compare(IList<T> x, IList<T> y) {
			for(int i = 0; i < x.Count && i < y.Count; i++) {
				int result = Comparer<T>.Default.Compare(x[i], y[i]);
				if(result != 0) return result;
			}
			return Comparer<int>.Default.Compare(x.Count, y.Count);
		}
	}
	public class NavigationInfo {
		BrickPagePairCollection bookmarkBricks = new BrickPagePairCollection();
		BrickPagePairCollection navigationBricks = new BrickPagePairCollection();
		public BrickPagePairCollection NavigationBricks { get { return navigationBricks; } }
		public BrickPagePairCollection BookmarkBricks { get { return bookmarkBricks; } }
		public void Clear() {
			bookmarkBricks.Clear();
			navigationBricks.Clear();
		}
		public void UpdateTargets(Page page, int pageIndex, VisualBrick brick, int[] brickIndices) {
			RectangleF brickBounds = page.GetBrickBounds(brickIndices);
			if(RectHelper.RectangleFContains(page.UsefulPageRectF, brickBounds.Location, 3))
				UpdateBricks(brick, page, brickIndices, brickBounds);
		}
		void UpdateBricks(VisualBrick brick, Page page, int[] indices, RectangleF brickBounds) {
			System.Diagnostics.Debug.Assert(null != brick.PrintingSystem);
			BrickPagePair brickPagePair = null;
			if(brick.BookmarkInfo.HasBookmark) {
				brickPagePair = BrickPagePair.Create(indices, page, brickBounds);
				bookmarkBricks.Add(brickPagePair);
			}
			if(brick.BrickOwner.IsNavigationTarget) {
				if(brickPagePair == null)
					brickPagePair = BrickPagePair.Create(indices, page, brickBounds);
				navigationBricks.Add(brickPagePair);
			}
			if(brick.BrickOwner.IsNavigationLink) {
				if(brickPagePair == null)
					brickPagePair = BrickPagePair.Create(indices, page, brickBounds);
				navigationBricks.Add(brickPagePair);
			}
		}
	}
}

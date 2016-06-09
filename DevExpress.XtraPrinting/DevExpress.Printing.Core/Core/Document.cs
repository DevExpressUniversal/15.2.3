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
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Export;
using System.IO;
using System.Text;
#if !DXPORTABLE
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.DocumentView;
#endif
#if !SL
using System.Windows.Forms;
using DevExpress.Utils.StoredObjects;
#endif
namespace DevExpress.XtraPrinting {
	public static class PrintingSystemSerializationNames {
		public const string Bricks = "Bricks";
		public const string Brick = "Brick";
		public const string ImageEntry = "ImageEntry";
		public const string InnerBricks = "InnerBricks";
		public const string SharedBricks = "SharedBricks";
		public const string SharedImages = "SharedImages";
		public const string SharedStyles = "SharedStyles";
		public const string Style = "Style";
		public const string Modifier = "Modifier";
		public const string Offset = "Offset";
		public const string FillColor = "FillColor";
		public const string OriginalIndex = "OriginalIndex";
		public const string OriginalPageCount = "OriginalPageCount";
		public const string MarginsF = "MarginsF";
		public const string PageBackColor = "PageBackColor";
		public const string Generator = "Generator";
		public const string Shape = "Shape";
		public const string Rows = "Rows";
		public const string Nodes = "Nodes";
		public const string PageData = "PageData";
		public const string Hint = "Hint";
		public const string UseTextAsDefaultHint = "UseTextAsDefaultHint";
		public const string Line = "Line";
		public const string MultiColumnInfo = "MultiColumnInfo";
	}
#if !DXPORTABLE
#if DEBUGTEST
	[System.Diagnostics.DebuggerTypeProxy(typeof(DebuggerHelpers.DocumentDebuggerTypeProxy))]
#endif
	[SerializationContext(typeof(PrintingSystemSerializationContext))]
	public abstract class Document : IDisposable, IXtraSerializable, IXtraSortableProperties, IXtraRootSerializationObject {
#region inner classes
		class ContinuousExportBrickCollector : IBrickExportVisitor {
			Document document;
			public ContinuousExportBrickCollector(Document document) {
				this.document = document;
			}
			void IBrickExportVisitor.ExportBrick(double horizontalOffset, double verticalOffset, Brick brick) {
				document.AddBrickObjectsToCache(brick);
			}
		}
#endregion
		const string DefaultName = "Document";
		protected PageList fPages;
		protected PrintingSystemBase ps;
		private string name = DefaultName;
		private bool correctImportBrickBounds;
		private BookmarkNode rootBookmark;
		private bool isDisposed;
		bool isModified;
		bool canChangePageSettings = true;
		protected DocumentState state = DocumentState.Empty;
		protected bool isCreated;
		Guid contentIdentity = Guid.Empty;
		public Guid ContentIdentity {
			get {
				if(contentIdentity == Guid.Empty)
					contentIdentity = Guid.NewGuid();
				return contentIdentity;
			}
		}
		internal DocumentState State {
			get { return state; }
		}
		internal bool IsCreated {
			get { return isCreated; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentIsModified")]
#endif
		public bool IsModified {
			get { return isModified; }
		}
		internal void SetModified(bool value) {
			isModified = value;
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentIsCreating")]
#endif
		public virtual bool IsCreating {
			get {
				return state == DocumentState.Creating || state == DocumentState.PostProcessing;
			}
		}
		internal bool IsDisposed {
			get { return isDisposed; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentName")]
#endif
		public string Name {
			get {
				return name;
			}
			set {
				if (name == Bookmark)
					Bookmark = value;
				ps.ExportOptions.UpdateDefaultFileName(name, value);
				name = value;
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentBookmark")]
#endif
		public string Bookmark {
			get { return rootBookmark.Text; }
			set { rootBookmark.Text = value; }
		}
		internal bool CorrectImportBrickBounds { get { return correctImportBrickBounds; } set { correctImportBrickBounds = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentPages")]
#endif
		public PageList Pages {
			get {
				if(fPages == null)
					fPages = CreatePageList();
				return fPages; 
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentIsLocked")]
#endif
		public virtual bool IsLocked {
			get { return false; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentBaseOffset")]
#endif
		public virtual PointF BaseOffset {
			get { return PointF.Empty; }
			set { ;}
		}
		internal Page FirstPage { get { return Pages.First; }
		}
		internal Page LastPage { get { return Pages.Last; } 
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentPageCount")]
#endif
		public virtual int PageCount {
			get { return Pages.Count; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentPrintingSystem")]
#endif
		public PrintingSystemBase PrintingSystem {
			get { return ps; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentBookmarkNodes")]
#endif
		public IBookmarkNodeCollection BookmarkNodes {
			get { return rootBookmark.Nodes; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentAutoFitToPagesWidth")]
#endif
		public abstract int AutoFitToPagesWidth { get; set; }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentScaleFactor")]
#endif
		public abstract float ScaleFactor { get; set; }
		protected internal RectangleF UsefulPageRect { get { return ps.PageSettings.UsefulPageRectF; } }
		protected internal abstract RectangleF PageFootBounds { get; }
		protected internal abstract RectangleF PageHeadBounds { get; }
		protected internal abstract float MinPageHeight { get; }
		protected internal abstract float MinPageWidth { get; }
		public virtual bool IsEmpty { get { return PageCount == 0; } }
		internal ProgressReflector ProgressReflector {
			get {
				return ps.ProgressReflector;
			}
		}
		internal virtual bool CanPerformContinuousExport { get { return false; } }
#region serialization
		List<PageDataWithIndices> pageData;
		protected ObjectCache styles;
		protected ObjectCache images;
		protected ObjectCache bricks;
		internal List<PageDataWithIndices> PageData { get { return pageData; } }
		internal ObjectCache BricksSerializationCache { get { return bricks; } }
		internal ObjectCache StylesSerializationCache { get { return styles; } }
		internal ObjectCache ImagesSerializationCache { get { return images; } }
		internal BookmarkNode RootBookmark { get { return rootBookmark; } }
		protected internal abstract ContinuousExportInfo GetContinuousExportInfo();
		internal virtual ContinuousExportInfo GetDeserializationContinuousExportInfo() {
			return new DeserializedContinuousExportInfo();
		}
		internal virtual BrickPagePair CreateBrickPagePair(XtraPropertyInfo pageIndexProperty, XtraPropertyInfo brickIndicesProperty) {
			if(brickIndicesProperty == null || brickIndicesProperty == null)
				return BrickPagePair.Empty;
			int pageIndex = int.Parse(pageIndexProperty.Value.ToString());
			int[] indices = BrickPagePairHelper.ParseIndices(brickIndicesProperty.Value.ToString());
			if(ps != null) {
				IBrickPagePairFactory factory = ((IServiceProvider)ps).GetService(typeof(IBrickPagePairFactory)) as IBrickPagePairFactory;
				if(factory != null)
					return factory.CreateBrickPagePair(indices, pageIndex);
			}
			return BrickPagePair.Create(indices, Pages[pageIndex]);
		}		
		internal virtual void Serialize(Stream stream, XtraSerializer serializer) {
			ps.ProgressReflector.SetProgressRanges(new float[] { 1, 10 });
			ContinuousExportInfo continuousExportInfo = null;
			ps.ProgressReflector.EnsureRangeDecrement(() => continuousExportInfo = GetContinuousExportInfo());
			SerializeCore(stream, serializer, continuousExportInfo, Pages);
		}		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SerializeCore(Stream stream, XtraSerializer serializer, ContinuousExportInfo info, IList<Page> pages) {
			lock(this) {
				ArrayHelper.ForEach(Pages.ToArray(), page => page.PerformLayout(new PrintingSystemContextWrapper(ps, page)));
				DocumentSerializationCollection collection = new DocumentSerializationCollection();
				collection.Add(new DocumentSerializationOptions(this, pages.Count));
				collection.AddRange(pages, index => true);
				collection.Add(info);
				ps.ProgressReflector.InitializeRange(pages.Count);
				PreparePageSerialization(pages);
				info.ExecuteExport(new ContinuousExportBrickCollector(this), PrintingSystem);
				serializer.SerializeObjects(this, collection, stream, string.Empty, null);
			}
		}
		internal void Deserialize(Stream stream, XtraSerializer serializer) {
			DocumentDeserializationCollection collection = new DocumentDeserializationCollection(this, index => true);
			collection.Add(new DocumentSerializationOptions(this));
			collection.Add(GetDeserializationContinuousExportInfo());
			DeserializeCore(stream, serializer, collection);
		}
		protected void DeserializeCore(Stream stream, XtraSerializer serializer, DocumentSerializationCollection objects) {
			serializer.DeserializeObjects(this, objects, stream, string.Empty, null);
		}
#region IXtraSerializable Members
		void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			pageData = new List<PageDataWithIndices>();
			OnStartDeserializingCore();
		}
		protected virtual void OnStartDeserializingCore() {
			ExceptionHelper.ThrowInvalidOperationException();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			pageData = null;
			OnEndDeserializingCore();
		}
		protected virtual void OnEndDeserializingCore() {
			ExceptionHelper.ThrowInvalidOperationException();
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void PreparePageSerialization(IList<Page> pages) {
			CollectPageData(pages);
			CreateSerializationObjects();
			foreach(Page page in pages) {
				PageBrickEnumerator en = new PageBrickEnumerator(page);
				while(en.MoveNext()) {
					AddBrickObjectsToCache(en.CurrentBrick);
				}
			}
			bricks.StopCollectSharedObjects();
			images.StopCollectSharedObjects();
			styles.StopCollectSharedObjects();
		}
		void CollectPageData(IList<Page> pages) {
			Dictionary<ReadonlyPageData, StringBuilder> pageDataHT = new Dictionary<ReadonlyPageData, StringBuilder>();
			for(int i = 0; i < pages.Count; i++) {
				ReadonlyPageData item = pages[i].PageData;
				if(pageDataHT.ContainsKey(item)) {
					pageDataHT[item].Append(',');
					pageDataHT[item].Append(i.ToString());
				} else {
					pageDataHT.Add(item, new StringBuilder(i.ToString()));
				}
			}
			pageData = new List<PageDataWithIndices>();
			foreach(KeyValuePair<ReadonlyPageData, StringBuilder> pair in pageDataHT) {
				pageData.Add(new PageDataWithIndices(pair.Key, pair.Value.ToString()));
			}
		}
		protected virtual void CreateSerializationObjects() {
			styles = new ObjectCache();
			images = new ObjectCache(ImageEntry.ImageEntryEqualityComparer.Instance);
			bricks = new ObjectCache();
		}
		internal void AddBrickObjectsToCache(Brick brick) {
			bricks.AddSerializationObject(brick);
			Brick realBrick = brick.GetRealBrick();
			if(!object.ReferenceEquals(realBrick, brick))
				AddBrickObjectsToCache(realBrick);
			bricks.StopCollectSharedObjects();
			if(!(brick is CheckBoxTextBrick)) {
				foreach(Brick child in brick.Bricks) {
					AddBrickObjectsToCache(child);
				}
			}
			bricks.StartCollectSharedObjects();
			VisualBrick visualBrick = brick as VisualBrick;
			if(visualBrick != null) {
				styles.AddSerializationObject(visualBrick.Style);
			}
			ImageBrick imageBrick = brick as ImageBrick;
			if(imageBrick != null && imageBrick.ShouldSerializeImageEntryInternal()) {
				images.AddSerializationObject(imageBrick.ImageEntry);
			}
		}
		void IXtraSerializable.OnEndSerializing() {
			OnEndSerializingCore();
		}
		protected virtual void OnEndSerializingCore() {
		}
		protected internal virtual void LoadPage(int pageIndex) { 
		}
		protected internal virtual void ForceLoad() {
		}
#endregion
		bool IXtraSortableProperties.ShouldSortProperties() {
			return true;
		}
#region IXtraRootSerializationObject Members
		void IXtraRootSerializationObject.AfterSerialize() {
			pageData = null;
			NullCaches();
			ps.ProgressReflector.MaximizeRange();
		}
		protected void NullCaches() {
			styles = null;
			images = null;
			bricks = null;
		}
		SerializationInfo IXtraRootSerializationObject.GetIndexByObject(string propertyName, object obj) {
			switch(propertyName) { 
				case PrintingSystemSerializationNames.Brick:
				case PrintingSystemSerializationNames.Bricks:
				case PrintingSystemSerializationNames.InnerBricks:
				case PrintingSystemSerializationNames.SharedBricks:
					return bricks.GetIndexByObject(obj);
				case PrintingSystemSerializationNames.ImageEntry:
				case PrintingSystemSerializationNames.SharedImages:
					return images.GetIndexByObject(obj);
				case PrintingSystemSerializationNames.SharedStyles:
				case PrintingSystemSerializationNames.Style:
					return styles.GetIndexByObject(obj);
			}
			return ExceptionHelper.ThrowInvalidOperationException<SerializationInfo>();
		}
		object IXtraRootSerializationObject.GetObjectByIndex(string propertyName, int index) {
			return GetObjectByIndexCore(propertyName, index);
		}
		protected virtual object GetObjectByIndexCore(string propertyName, int index) {
			return ExceptionHelper.ThrowInvalidOperationException<object>();
		}
#endregion
#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("DocumentCanChangePageSettings")]
#endif
		public bool CanChangePageSettings { 
			get { return canChangePageSettings && !this.IsCreating; } 
			set { canChangePageSettings = value; } 
		}
		internal bool CanRecreatePages {
			get {
				return Pages.Count > 0 && CanChangePageSettings;
			}
		}
		internal AvailableExportModes AvailableExportModes {
			get {
				return new AvailableExportModes(
					ArrayHelper.ConvertAll<object, RtfExportMode>(GetAvailableExportModes(typeof(RtfExportMode)), obj => (RtfExportMode)obj),
					ArrayHelper.ConvertAll<object, HtmlExportMode>(GetAvailableExportModes(typeof(HtmlExportMode)), obj => (HtmlExportMode)obj),
					ArrayHelper.ConvertAll<object, ImageExportMode>(GetAvailableExportModes(typeof(ImageExportMode)), obj  => (ImageExportMode)obj),
					ArrayHelper.ConvertAll<object, XlsExportMode>(GetAvailableExportModes(typeof(XlsExportMode)), obj => (XlsExportMode)obj),
					ArrayHelper.ConvertAll<object, XlsxExportMode>(GetAvailableExportModes(typeof(XlsxExportMode)), obj => (XlsxExportMode)obj)
				);
			}
		}
		protected Document(PrintingSystemBase ps) {
			this.ps = ps;
			rootBookmark = CreateRootBookmarkNode(DefaultName);
			ps.AfterMarginsChange += new MarginsChangeEventHandler(ps_MarginsChanged);
			ps.PageSettingsChanged += new EventHandler(ps_PageSettingsChanged);
			ps.ScaleFactorChanged += new EventHandler(ps_ScaleFactorChanged);
		}
		protected virtual BookmarkNode CreateRootBookmarkNode(string name) {
			return new RootBookmarkNode(name);
		}
		protected abstract PageList CreatePageList();
		protected virtual object[] GetAvailableExportModes(Type exportModeType) {
			if(exportModeType == typeof(ImageExportMode))
				return new object[] { ImageExportMode.SingleFilePageByPage, ImageExportMode.DifferentFiles };
			if(exportModeType == typeof(RtfExportMode))
				return new object[] { RtfExportMode.SingleFilePageByPage };
			if(exportModeType == typeof(HtmlExportMode))
				return new object[] { HtmlExportMode.SingleFilePageByPage, HtmlExportMode.DifferentFiles };
			if(exportModeType == typeof(XlsExportMode))
				return new object[] { XlsExportMode.DifferentFiles };
			if(exportModeType == typeof(XlsxExportMode))
				return new object[] { XlsxExportMode.SingleFilePageByPage, XlsxExportMode.DifferentFiles };
			return null;
		}
		protected internal abstract void StopPageBuilding();
		protected internal void OnContentChanged() {		  
			ps.OnDocumentChanged(EventArgs.Empty);
		}
		protected internal virtual void Dispose(bool disposing) {
			if (disposing) {
				Clear();
				if (ps != null) {
					ps.AfterMarginsChange -= new MarginsChangeEventHandler(ps_MarginsChanged);
					ps.PageSettingsChanged -= new EventHandler(ps_PageSettingsChanged);
					ps.ScaleFactorChanged -= new EventHandler(ps_ScaleFactorChanged);
				}
				isDisposed = true;
			}
		}
		public virtual void Dispose() {
			Dispose(true);
		}
		protected internal virtual void OnClearPages() {
			BookmarkNodes.Clear();
			ps.OnClearPages();
			CanChangePageSettings = true;
		}
		internal virtual void ClearContent() {
			Clear();
		}
		internal void OnCreateException() {
			state = DocumentState.Empty;
		}
		protected virtual void Clear() {
			int pageCount = Pages.Count;
			foreach(Page page in Pages)
				page.AssignWatermark(null);
			Pages.Clear();
			ps.PerformIfNotNull<GroupingManager>(groupingManager => groupingManager.Clear());
			state = DocumentState.Empty;
			contentIdentity = Guid.Empty;
			if(pageCount > 0)
				OnContentChanged();
		}
		protected internal abstract void HandleNewPageSettings();
		protected internal abstract void HandleNewScaleFactor();
		private void ps_MarginsChanged(object sender, MarginsChangeEventArgs e) {
			if(!IsDisposed)
				ps.DelayedAction |= PrintingSystemAction.HandleNewPageSettings;
		}
		private void ps_PageSettingsChanged(object sender, EventArgs e) {
			if(IsDisposed || IsCreating)
				return;
			ps.DelayedAction |= PrintingSystemAction.HandleNewPageSettings;
		}
		private void ps_ScaleFactorChanged(object sender, EventArgs e) {
			if(!IsDisposed)
				ps.DelayedAction |= PrintingSystemAction.HandleNewScaleFactor;
		}
		public virtual void UpdateBaseOffset() {
		}
		protected internal abstract void AddBrick(Brick brick);
		protected internal virtual void Begin() {
			correctImportBrickBounds = false;
		}
		protected internal abstract void End(bool buildPagesInBackground);
		protected internal abstract void BeginReport(DocumentBand docBand, PointF offset);
		protected internal abstract void EndReport();
		protected internal abstract DocumentBand AddReportContainer();
		protected internal abstract void InsertPageBreak(float pos);
		protected internal abstract void InsertPageBreak(float pos, CustomPageData nextPageData);
		public abstract void ShowFromNewPage(Brick brick);
		public Brick GetBrick(BookmarkNode bookmarkNode) {
			if(bookmarkNode.PageIndex < 0 || bookmarkNode.PageIndex > Pages.Count)
				return null;
			Page page = Pages[bookmarkNode.PageIndex];
			return (Brick)page.GetBrickByIndices(bookmarkNode.Pair.BrickIndices);
		}
	}
#endif
}

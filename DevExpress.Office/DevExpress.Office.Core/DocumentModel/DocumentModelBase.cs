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
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Services.Internal;
using DevExpress.Office.Internal;
using DevExpress.Office.History;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Office.Model {
	#region DocumentModelBase<TFormat> (abstract class)
	public abstract class DocumentModelBase<TFormat> : DpiSupport, IDocumentModel, IBatchUpdateable, IBatchUpdateHandler, IServiceContainer, IDisposable {
		#region Fields
		static readonly float dpiX;
		static readonly float dpiY;
		static readonly float dpi;
		bool isDisposed;
		BatchUpdateHelper batchUpdateHelper;
		ServiceManager serviceManager;
		DocumentHistory history;
		DocumentModelUnitConverter unitConverter;
		DocumentModelUnitToLayoutUnitConverter toDocumentLayoutUnitConverter;
		DocumentLayoutUnitConverter layoutUnitConverter;
		DocumentLayoutUnit layoutUnit = DefaultLayoutUnit;
		FontCacheManager fontCacheManager;
		FontCache fontCache;
		ImageCacheBase imageCache;
		IDrawingCache drawingCache;
		UriBasedImageReplaceQueue uriBasedImageReplaceQueue;
		IOfficeTheme officeTheme;
		#endregion
		static DocumentModelBase() {
			dpiX = DevExpress.XtraPrinting.GraphicsDpi.Pixel;
			dpiY = DevExpress.XtraPrinting.GraphicsDpi.Pixel;
			dpi = DpiX;
		}
		public static float Dpi { get { return dpi; } }
		public static float DpiX { get { return dpiX; } }
		public static float DpiY { get { return dpiY; } }
		protected DocumentModelBase(float screenDpiX, float screenDpiY)
			: base(screenDpiX, screenDpiY) {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.serviceManager = CreateServiceManager();
			this.unitConverter = CreateDocumentModelUnitConverter();
			this.uriBasedImageReplaceQueue = new UriBasedImageReplaceQueue(this);
			this.imageCache = CreateImageCache();
			UpdateLayoutUnitConverter();
			CreateOfficeTheme();
			this.drawingCache = new DrawingCache<TFormat>(this);
		}
		protected DocumentModelBase()
			: this(DpiX, DpiY) {
		}
		#region Properties
		public static DocumentLayoutUnit DefaultLayoutUnit { get { return DocumentLayoutUnit.Document; } }
		public bool IsDisposed { get { return isDisposed; } }
		public virtual int MaxFieldSwitchLength { get { return 2; } }
		public virtual bool EnableFieldNames { get { return false; } }
		public ServiceManager ServiceManager { get { return serviceManager; } }
		public BatchUpdateHelper BatchUpdateHelper { get { return batchUpdateHelper; } }
		public DocumentHistory History { get { return history; } }
		public abstract IDocumentModelPart MainPart { get; }
		public DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		public DocumentModelUnitToLayoutUnitConverter ToDocumentLayoutUnitConverter { get { return toDocumentLayoutUnitConverter; } }
		public DocumentLayoutUnitConverter LayoutUnitConverter { get { return layoutUnitConverter; } }
		public DocumentLayoutUnit LayoutUnit {
			get { return layoutUnit; }
			set {
				if (layoutUnit == value)
					return;
				layoutUnit = value;
				OnLayoutUnitChanged();
			}
		}
		public FontCache FontCache { get { return fontCache; } }
		public ImageCacheBase ImageCache { get { return imageCache; } }
		public IDrawingCache DrawingCache { get { return drawingCache; } }
		public virtual IOfficeTheme OfficeTheme { get { return officeTheme; } set { ApplyTheme(value); } }
		public FontCacheManager FontCacheManager {
			get { return fontCacheManager; }
			set {
				if (fontCacheManager == value)
					return;
				SetFontCacheManager(value);
			}
		}
		public UriBasedImageReplaceQueue UriBasedImageReplaceQueue { get { return uriBasedImageReplaceQueue; } }
		#endregion
		#region IBatchUpdateable Members
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		public bool IsUpdateLockedOrOverlapped { get { return batchUpdateHelper.IsUpdateLocked || batchUpdateHelper.OverlappedTransaction; } }
		#endregion
		#region IBatchUpdateHandler Members
		public abstract void OnBeginUpdate();
		public abstract void OnCancelUpdate();
		public abstract void OnEndUpdate();
		public abstract void OnFirstBeginUpdate();
		public abstract void OnLastCancelUpdate();
		public abstract void OnLastEndUpdate();
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (isDisposed)
				return;
			if (disposing)
				DisposeCore();
			isDisposed = true;
		}
		protected internal virtual void DisposeCore() {
			ClearCore();
			if (this.serviceManager != null) {
				this.serviceManager.Dispose();
				this.serviceManager = null;
			}
			this.fontCacheManager = null;
		}
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (serviceManager != null)
				serviceManager.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if (serviceManager != null)
				serviceManager.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual object GetService(Type serviceType) {
			if (serviceManager != null)
				return serviceManager.GetService(serviceType);
			else
				return null;
		}
		#endregion
		protected virtual DocumentModelUnitConverter CreateDocumentModelUnitConverter() {
			return new DocumentModelUnitTwipsConverter(ScreenDpiX, ScreenDpiY);
		}
		public virtual T GetService<T>() where T : class {
			return ServiceUtils.GetService<T>(this);
		}
		public virtual T ReplaceService<T>(T newService) where T : class {
			return ServiceUtils.ReplaceService(this, newService);
		}
		protected virtual void CreateOfficeTheme() {
			this.officeTheme = OfficeThemeBuilder<TFormat>.CreateTheme(OfficeThemePreset.Office);
		}
		protected internal virtual ServiceManager CreateServiceManager() {
			return new ServiceManager();
		}
		public virtual bool IsNormalHistory { get { return !(this.history is EmptyHistory); } }
		protected virtual ImageCacheBase CreateImageCache() {
			return new ImageCache(this);
		}
		public virtual void SwitchToEmptyHistory(bool disposeHistory) {
			if (disposeHistory)
				DisposeHistory();
			this.history = CreateEmptyHistory();
			SubscribeHistoryEvents();
		}
		protected virtual DocumentHistory CreateEmptyHistory() {
			return new EmptyHistory(this);
		}
		public virtual void SwitchToNormalHistory(bool disposeHistory) {
			if (disposeHistory)
				DisposeHistory();
			this.history = CreateDocumentHistory();
			ResetMerging();
			SubscribeHistoryEvents();
		}
		public virtual DocumentHistory ReplaceHistory(DocumentHistory newHistory) {
			DocumentHistory result = this.history;
			UnsubscribeHistoryEvents();
			this.history = newHistory;
			ResetMerging();
			SubscribeHistoryEvents();
			return result;
		}
		protected internal virtual DocumentHistory CreateDocumentHistory() {
			return new DocumentHistory(this);
		}
		protected internal virtual void SubscribeHistoryEvents() {
			History.OperationCompleted += new EventHandler(OnHistoryOperationCompleted);
			History.ModifiedChanged += new EventHandler(OnHistoryModifiedChanged);
		}
		protected internal virtual void UnsubscribeHistoryEvents() {
			History.OperationCompleted -= new EventHandler(OnHistoryOperationCompleted);
			History.ModifiedChanged -= new EventHandler(OnHistoryModifiedChanged);
		}
		protected internal virtual void DisposeHistory() {
			if (History != null) {
				UnsubscribeHistoryEvents();
				History.Dispose();
			}
		}
		public virtual void ResetMerging() {
		}
		protected internal abstract void OnHistoryOperationCompleted(object sender, EventArgs e);
		protected internal abstract void OnHistoryModifiedChanged(object sender, EventArgs e);
		public virtual void ClearCore() {
			ClearFontCache();
			if (this.history != null) {
				UnsubscribeHistoryEvents();
				this.history.Dispose();
				this.history = null;
			}
		}
		protected internal virtual void ClearFontCache() {
			if (fontCache != null) {
				if (fontCacheManager != null)
					fontCacheManager.ReleaseFontCache(fontCache);
				fontCache = null;
			}
		}
		public virtual void SetFontCacheManager(FontCacheManager fontCacheManager) {
			Guard.ArgumentNotNull(fontCacheManager, "fontCacheManager");
			ClearFontCache();
			this.fontCacheManager = fontCacheManager;
			this.fontCache = fontCacheManager.CreateFontCache();
		}
		protected internal void UpdateLayoutUnitConverter() {
			this.toDocumentLayoutUnitConverter = unitConverter.CreateConverterToLayoutUnits(LayoutUnit);
			this.layoutUnitConverter = DocumentLayoutUnitConverter.Create(LayoutUnit, ScreenDpi);
		}
		protected internal void UpdateFontCache() {
			this.fontCacheManager = FontCacheManager.CreateDefault(LayoutUnitConverter);
			this.fontCache = fontCacheManager.CreateFontCache();
		}
		protected internal virtual void OnLayoutUnitChanged() {
			UpdateLayoutUnitConverter();
			this.FontCacheManager = FontCacheManager.CreateDefault(LayoutUnitConverter);
			RaiseLayoutUnitChanged();
		}
		EventHandler layoutUnitChanged;
		public event EventHandler LayoutUnitChanged { add { layoutUnitChanged += value; } remove { layoutUnitChanged -= value; } }
		private void RaiseLayoutUnitChanged() {
			if (layoutUnitChanged != null) {
				this.layoutUnitChanged(this, EventArgs.Empty);
			}
		}
		public virtual void Undo() {
			History.Undo();
		}
		public virtual void Redo() {
			History.Redo();
		}
		public virtual void PreprocessContentBeforeExport(TFormat format) {
		}
		public virtual TFormat AutodetectDocumentFormat(string fileName) {
			return AutodetectDocumentFormat(fileName, true);
		}
		public virtual TFormat AutodetectDocumentFormat(string fileName, bool useFormatFallback) {
			ImportHelper<TFormat, bool> importHelper = CreateDocumentImportHelper();
			IImportManagerService<TFormat, bool> importManagerService = GetImportManagerService();
			if (importManagerService == null)
				return importHelper.UndefinedFormat;
			IImporter<TFormat, bool> importer = importHelper.AutodetectImporter(fileName, importManagerService, useFormatFallback);
			if (importer == null)
				return importHelper.UndefinedFormat;
			else
				return importer.Format;
		}
		public virtual void LoadDocument(Stream stream, TFormat documentFormat, string sourceUri) {
			LoadDocument(stream, documentFormat, sourceUri, null);
		}
		public virtual void LoadDocument(Stream stream, TFormat documentFormat, string sourceUri, Encoding encoding) {
			ImportHelper<TFormat, bool> importHelper = CreateDocumentImportHelper();
			IImportManagerService<TFormat, bool> importManagerService = GetImportManagerService();
			if (importManagerService == null)
				importHelper.ThrowUnsupportedFormatException();
			importHelper.Import(stream, documentFormat, sourceUri, importManagerService, encoding);
		}
		public virtual void SaveDocument(Stream stream, TFormat documentFormat, string targetUri) {
			SaveDocument(stream, documentFormat, targetUri, null);
		}
		protected internal virtual void SaveDocument(Stream stream, TFormat documentFormat, string targetUri, Encoding encoding) {
			ExportHelper<TFormat, bool> exportHelper = CreateDocumentExportHelper(documentFormat);
			IExportManagerService<TFormat, bool> exportManagerService = GetExportManagerService();
			if (exportManagerService == null)
				exportHelper.ThrowUnsupportedFormatException();
			exportHelper.Export(stream, documentFormat, targetUri, exportManagerService, encoding);
		}
		public OfficeReferenceImage CreateImage(Stream stream) {
			long position = stream.Position;
			int crc32;
			using (Crc32Stream crc32Stream = new Crc32Stream(stream)) {
				crc32Stream.ReadToEnd();
				crc32 = (int)crc32Stream.ReadCheckSum;
			}
			Crc32ImageId id = new Crc32ImageId(crc32);
			OfficeReferenceImage cachedImage = ImageCache.GetImageById(id);
			if (cachedImage != null)
				return cachedImage;
			stream.Seek(position, SeekOrigin.Begin);
			OfficeNativeImage image = OfficeImage.CreateImage(stream, id);
			return ImageCache.AddImage(image);
		}
#if !SL
		public OfficeReferenceImage CreateImage(MemoryStreamBasedImage image) {
			return new OfficeReferenceImage(this, OfficeImage.CreateImage(image));
		}
		public OfficeReferenceImage CreateImage(Image image) {
			return new OfficeReferenceImage(this, OfficeImage.CreateImage(image));
		}
		public OfficeImage GetImageById(IUniqueImageId id) {
			OfficeReferenceImage referenceImage = ImageCache.GetImageById(id);
			if (referenceImage != null)
				return referenceImage.NativeRootImage;
			return null;
		}
#endif
		void ApplyTheme(IOfficeTheme officeTheme) {
			if (officeTheme == null)
				officeTheme = OfficeThemeBuilder<TFormat>.CreateTheme(OfficeThemePreset.Office);
			if (this.officeTheme != null)
				(this.officeTheme as DocumentModelBase<TFormat>).Dispose();
			this.officeTheme = officeTheme;
			RaiseThemeChanged();
		}
		#region ThemeChanged
		EventHandler onThemeChanged;
		public event EventHandler ThemeChanged { add { onThemeChanged += value; } remove { onThemeChanged -= value; } }
		protected internal virtual void RaiseThemeChanged() {
			if (onThemeChanged != null)
				onThemeChanged(this, EventArgs.Empty);
		}
		#endregion
		protected internal abstract ImportHelper<TFormat, bool> CreateDocumentImportHelper();
		public abstract ExportHelper<TFormat, bool> CreateDocumentExportHelper(TFormat documentFormat);
		protected internal abstract IImportManagerService<TFormat, bool> GetImportManagerService();
		protected internal abstract IExportManagerService<TFormat, bool> GetExportManagerService();	  
	}
	#endregion
}
namespace DevExpress.Office.DrawingML { 
	[Flags]
	public enum DocumentModelChangeActions {
		None = 0x00000000
	}
}

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
using DevExpress.Office.Utils;
using DevExpress.Office.Import;
using System.IO;
using DevExpress.Office.Localization;
namespace DevExpress.Office.Internal {
	#region IPictureImportManagerService
	public interface IPictureImportManagerService : IImportManagerService<OfficeImageFormat, OfficeImage> {
	}
	#endregion
	#region PictureFormatsManagerService
	public class PictureFormatsManagerService : ImportManagerService<OfficeImageFormat, OfficeImage>, IPictureImportManagerService {
		public PictureFormatsManagerService()
			: base() {
		}
		protected internal override void RegisterNativeFormats() {
			RegisterImporter(new BitmapPictureImporter());
			RegisterImporter(new JPEGPictureImporter());
			RegisterImporter(new PNGPictureImporter());
			RegisterImporter(new GifPictureImporter());
#if !SL
			RegisterImporter(new TiffPictureImporter());
			RegisterImporter(new EmfPictureImporter());
			RegisterImporter(new WmfPictureImporter());
#endif
		}
	}
	#endregion
	#region PictureImporterOptions (abstract class)
	public abstract class PictureImporterOptions : IImporterOptions {
		string sourceUri;
		protected PictureImporterOptions() {
			this.sourceUri = String.Empty;
		}
		public string SourceUri { get { return sourceUri; } set { sourceUri = value; } }
		public virtual void CopyFrom(IImporterOptions value) {
		}
	}
	#endregion
	#region PictureImporter (abstract class)
	public abstract class PictureImporter : IImporter<OfficeImageFormat, OfficeImage> {
		#region IImporter<OfficeImageFormat,OfficeImage> Members
		public abstract FileDialogFilter Filter { get; }
		public abstract OfficeImageFormat Format { get; }
		public abstract IImporterOptions SetupLoading();
		public virtual OfficeImage LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
			return documentModel.CreateImage(stream);
		}
		#endregion
	}
	#endregion
	#region PNGPictureImporterOptions
	public class PNGPictureImporterOptions : PictureImporterOptions {
	}
	#endregion
	#region PNGPictureImporter
	public class PNGPictureImporter : PictureImporter {
		static readonly FileDialogFilter filter = new FileDialogFilter(OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_PNGFiles), "png");
		#region IImporter<RichEditImageFormat> Members
		public override FileDialogFilter Filter { get { return filter; } }
		public override OfficeImageFormat Format { get { return OfficeImageFormat.Png; } }
		public override IImporterOptions SetupLoading() {
			return new PNGPictureImporterOptions();
		}
		#endregion
	}
	#endregion
	#region JPEGPictureImporterOptions
	public class JPEGPictureImporterOptions : PictureImporterOptions {
	}
	#endregion
	#region JPEGPictureImporter
	public class JPEGPictureImporter : PictureImporter {
		static readonly FileDialogFilter filter = new FileDialogFilter(OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_JPEGFiles), new string[] { "jpg", "jpeg" });
		#region IImporter<RichEditImageFormat> Members
		public override FileDialogFilter Filter { get { return filter; } }
		public override OfficeImageFormat Format { get { return OfficeImageFormat.Jpeg; } }
		public override IImporterOptions SetupLoading() {
			return new JPEGPictureImporterOptions();
		}
		#endregion
	}
	#endregion
	#region GifPictureImporterOptions
	public class GifPictureImporterOptions : PictureImporterOptions {
	}
	#endregion
	#region GifPictureImporter
	public class GifPictureImporter : PictureImporter {
		static readonly FileDialogFilter filter = new FileDialogFilter(OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_GifFiles), "gif");
		#region IImporter<RichEditImageFormat> Members
		public override FileDialogFilter Filter { get { return filter; } }
		public override OfficeImageFormat Format { get { return OfficeImageFormat.Gif; } }
		public override IImporterOptions SetupLoading() {
			return new GifPictureImporterOptions();
		}
		#endregion
	}
	#endregion
	#region TiffPictureImporterOptions
	public class TiffPictureImporterOptions : PictureImporterOptions {
	}
	#endregion
	#region TiffPictureImporter
	public class TiffPictureImporter : PictureImporter {
		static readonly FileDialogFilter filter = new FileDialogFilter(OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_TiffFiles), new string[] { "tif", "tiff" });
		#region IImporter<RichEditImageFormat> Members
		public override FileDialogFilter Filter { get { return filter; } }
		public override OfficeImageFormat Format { get { return OfficeImageFormat.Tiff; } }
		public override IImporterOptions SetupLoading() {
			return new TiffPictureImporterOptions();
		}
		#endregion
	}
	#endregion
	#region EmfPictureImporterOptions
	public class EmfPictureImporterOptions : PictureImporterOptions {
	}
	#endregion
	#region EmfPictureImporter
	public class EmfPictureImporter : PictureImporter {
		static readonly FileDialogFilter filter = new FileDialogFilter(OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_EmfFiles), "emf");
		#region IImporter<RichEditImageFormat> Members
		public override FileDialogFilter Filter { get { return filter; } }
		public override OfficeImageFormat Format { get { return OfficeImageFormat.Emf; } }
		public override IImporterOptions SetupLoading() {
			return new EmfPictureImporterOptions();
		}
		#endregion
	}
	#endregion
	#region WmfPictureImporterOptions
	public class WmfPictureImporterOptions : PictureImporterOptions {
	}
	#endregion
	#region WmfPictureImporter
	public class WmfPictureImporter : PictureImporter {
		static readonly FileDialogFilter filter = new FileDialogFilter(OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_WmfFiles), "wmf");
		#region IImporter<RichEditImageFormat> Members
		public override FileDialogFilter Filter { get { return filter; } }
		public override OfficeImageFormat Format { get { return OfficeImageFormat.Wmf; } }
		public override IImporterOptions SetupLoading() {
			return new WmfPictureImporterOptions();
		}
		#endregion
	}
	#endregion
	#region BitmapPictureImporterOptions
	public class BitmapPictureImporterOptions : PictureImporterOptions {
	}
	#endregion
	#region BitmapPictureImporter
	public class BitmapPictureImporter : PictureImporter {
		static readonly FileDialogFilter filter = new FileDialogFilter(OfficeLocalizer.GetString(OfficeStringId.FileFilterDescription_BitmapFiles), new string[] { "bmp", "dib" });
		#region IImporter<RichEditImageFormat> Members
		public override FileDialogFilter Filter { get { return filter; } }
		public override OfficeImageFormat Format { get { return OfficeImageFormat.Bmp; } }
		public override IImporterOptions SetupLoading() {
			return new BitmapPictureImporterOptions();
		}
		public override OfficeImage LoadDocument(IDocumentModel documentModel, Stream stream, IImporterOptions options) {
#if !SL
			return documentModel.CreateImage(stream);
#else
			OfficeImageSL result = new OfficeImageSL();
			stream.Position = 14;
			result.LoadDibFromStream(stream, 0, 0, 0);
			return result;
#endif
		}
		#endregion
	}
	#endregion
}

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

using System.Drawing;
using System.IO;
#if SL
using DevExpress.Xpf.Collections;
#else
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using DevExpress.Printing.Core.PdfExport.Metafile;
#endif
namespace DevExpress.XtraPrinting.Export.Pdf {
	public interface IPdfContentsOwner {
		PdfFonts Fonts { get; }
	}
	public class PdfVectorImage : PdfImageBase, IPdfContentsOwner, IXObjectsOwner, IPdfDocumentOwner {
		PdfFonts fonts = new PdfFonts();
		MetafileInfo metafileInfo;
		PdfFonts IPdfContentsOwner.Fonts { get { return fonts; } }
		PdfDocument document;
		Native.Measurer measurer;
		PdfXObjects xObjects = new PdfXObjects();
		IPdfDocumentOwner documentInfo;
		SizeF imageSizeInPoints;
		PdfObjectCollection<PdfTransparencyGS> transparencyGSCollection = new PdfObjectCollection<PdfTransparencyGS>();
		PdfObjectCollection<PdfShading> shadingCollection = new PdfObjectCollection<PdfShading>();
		PdfObjectCollection<PdfPattern> patternCollection = new PdfObjectCollection<PdfPattern>();
		public PdfVectorImage(IPdfDocumentOwner documentInfo, Image image, PdfDocument document, string name, bool compressed)
			: this(documentInfo, new MetafileInfo((Metafile)image), document, name, compressed) {
		}
		internal PdfVectorImage(IPdfDocumentOwner documentInfo, MetafileInfo metafileInfo, PdfDocument document, string name, bool compressed)
			: base(name, compressed) {
			this.document = document;
			this.documentInfo = documentInfo;
			this.measurer = new Native.GdiPlusMeasurer();
			this.metafileInfo = metafileInfo;
			PdfDrawContext context = PdfDrawContext.Create(this.Stream, this, new PdfHashtable()); 
			GraphicsUnit pageUnit = GraphicsUnit.Point;
			RectangleF bounds = metafileInfo.GetBounds(ref pageUnit);
			imageSizeInPoints = new SizeF(GraphicsUnitConverter.Convert(bounds.Size.Width, metafileInfo.HorizontalResolution, GraphicsDpi.Point),
				GraphicsUnitConverter.Convert(bounds.Size.Height, metafileInfo.VerticalResolution, GraphicsDpi.Point));
			if(metafileInfo.Metafile != null) {
				PdfGraphicsImpl graphics = new PdfGraphicsImpl(this, context, imageSizeInPoints, document, this);
				graphics.PageUnit = pageUnit;
				new MetaImage(this, metafileInfo.Metafile, context, graphics, bounds.Location).Write();
				graphics.ResetTransform();
			}
		}
		public override Matrix Transform(RectangleF correctedBounds) {
			return new Matrix(correctedBounds.Width / imageSizeInPoints.Width,
				0,
				0,
				correctedBounds.Height / imageSizeInPoints.Height,
				correctedBounds.X,
				correctedBounds.Y);
		}
		public override void FillUp() {
			base.FillUp();
			xObjects.FillUp();
			Attributes.Add("Subtype", "Form");
			PdfDictionary resourceDictionary = new PdfDictionary();
			PdfDictionary xObjectsDictionary = xObjects.CreateDictionary();
			if(xObjectsDictionary != null)
				resourceDictionary.Add("XObject", xObjectsDictionary);
			FillFonts(resourceDictionary);
			PdfArray procSet = new PdfArray();
			procSet.Add("PDF");
			procSet.Add("Text");
			procSet.Add("ImageB");
			procSet.Add("ImageC");
			procSet.Add("ImageI");
			resourceDictionary.Add("ProcSet", procSet);
			resourceDictionary.AddIfNotNull("ExtGState", transparencyGSCollection.CreateDictionary());
			resourceDictionary.AddIfNotNull("Shading", shadingCollection.CreateDictionary());
			resourceDictionary.AddIfNotNull("Pattern", patternCollection.CreateDictionary());
			Attributes.Add("Resources", resourceDictionary);
			SizeF bboxSize = GetBoundingBox();
			PdfArray bbox = new PdfArray();
			bbox.Add(0);
			bbox.Add(0);
			bbox.Add(new PdfDouble(bboxSize.Width));
			bbox.Add(new PdfDouble(bboxSize.Height));
			Attributes.Add("BBox", bbox);
			Attributes.Add("FormType", 1);
		}
		void FillFonts(PdfDictionary resourceDictionary) {
			if(fonts.Count > 0) {
				PdfDictionary fontDictionary = new PdfDictionary();
				for(int i = 0; i < fonts.Count; i++)
					fontDictionary.Add(fonts[i].Name, fonts[i].Dictionary);
				resourceDictionary.Add("Font", fontDictionary);
			}
		}
		SizeF GetBoundingBox() {
			return imageSizeInPoints;
		}
		public PdfImageBase CreateImage(Image image) {
			return document.CreatePdfImage(documentInfo, this, image, Color.Empty); 
		}
		public void AddNewXObject(PdfXObject xObject) {
			xObjects.Add(xObject);
		}
		protected override void WriteContent(StreamWriter writer) {
			xObjects.Write(writer);
			base.WriteContent(writer);
		}
		protected override void RegisterContent(PdfXRef xRef) {
			base.RegisterContent(xRef);
			xObjects.Register(xRef);
			fonts.Register(xRef);
			transparencyGSCollection.Register(xRef);
			shadingCollection.Register(xRef);
			patternCollection.Register(xRef);
		}
		void IXObjectsOwner.AddExistingXObject(PdfXObject xObject) {
			xObjects.AddUnique(xObject);
		}
		void IXObjectsOwner.AddTransparencyGS(PdfTransparencyGS transparencyGS) {
			transparencyGSCollection.AddUnique(transparencyGS);
		}
		void IXObjectsOwner.AddShading(PdfShading shading) {
			shadingCollection.AddUnique(shading);
		}
		void IXObjectsOwner.AddPattern(PdfPattern pattern) {
			patternCollection.AddUnique(pattern);
		}
		public PdfNeverEmbeddedFonts NeverEmbeddedFonts {
			get { return documentInfo.NeverEmbeddedFonts; }
		}
		public PdfImageCache ImageCache {
			get { return documentInfo.ImageCache; }
		}
		public Native.Measurer Measurer {
			get { return documentInfo.MetafileMeasurer; }
		}
		public Native.Measurer MetafileMeasurer {
			get { return documentInfo.MetafileMeasurer; }
		}
		public bool ScaleStrings {
			get { return documentInfo.ScaleStrings; }
		}
#if DEBUGTEST
		public PdfXObjects Test_XObjects {
			get { return xObjects; }
		}
		public PdfObjectCollection<PdfPattern> Test_PatternCollection {
			get { return patternCollection; }
		}
		public SizeF Test_GetBoundingBox() {
			return GetBoundingBox();
		}
#endif
	}
	class MetafileInfo {
		Metafile metafile;
		public Metafile Metafile { get { return metafile; } }
		public bool IsNull { get { return metafile == null; } }
		public float HorizontalResolution { get; set; }
		public float VerticalResolution { get; set; }
		public Size ImageSize { get; set; }
		internal RectangleF TestBounds { get; set; }
		internal GraphicsUnit TestPageUnit { get; set; }
		public MetafileInfo(Metafile metafile) {
			this.metafile = metafile;
			HorizontalResolution = GraphicsDpi.Point;
			VerticalResolution = GraphicsDpi.Point;
			if(metafile != null) {
				ImageSize = metafile.Size;
				HorizontalResolution = metafile.HorizontalResolution;
				VerticalResolution = metafile.VerticalResolution;
			}
		}
		internal RectangleF GetBounds(ref GraphicsUnit pageUnit) {
			if(metafile != null)
				return metafile.GetBounds(ref pageUnit);
			pageUnit = TestPageUnit;
			return TestBounds;
		}
	}
}

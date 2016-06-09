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
using System.IO;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfCatalog : PdfDocumentDictionaryObject {
		public static PdfCatalog CreateInstance(bool compressed, bool showPrintDialog) {
			return showPrintDialog ? new PdfPrintCatalog(compressed) : 
				new PdfCatalog(compressed);
		}
		PdfPages pages;
		PdfOutlines outlines;
		PdfAcroForm acroForm;
		PdfMetadata metadata;
		PdfICCProfile iccProfile;
		PdfEmbeddedFiles embeddedFiles;
		bool pdfACompatible;
		public PdfPages Pages { get { return pages; } }
		public PdfOutlines Outlines { get { return outlines; } }
		public PdfMetadata Metadata { get { return metadata; } }
		public bool PdfACompatible { 
			get { return pdfACompatible; } 
			set { pdfACompatible = Metadata.PdfACompatible = value; } 
		}
		public ICollection<PdfAttachment> Attachments {
			set {
				embeddedFiles.SetAttachments(value);
				metadata.HasEmbeddedFiles = embeddedFiles.Active;
			}
		}
		PdfICCProfile ICCProfile { 
			get{ 
				if(iccProfile == null)
					iccProfile = new PdfICCProfile(this.Compressed);
				return iccProfile;
			}
		}
		protected PdfCatalog(bool compressed) : base(compressed) {
			this.pages = new PdfPages(null, Compressed);
			this.pages.MediaBox = new PdfRectangle(0, 0, 596, 842);
			this.outlines = new PdfOutlines(Compressed);
			this.acroForm = new PdfAcroForm(Compressed);
			this.embeddedFiles = new PdfEmbeddedFiles(Compressed);
			this.metadata = new PdfMetadata(Compressed);
			this.metadata.HasEmbeddedFiles = embeddedFiles.Active;
		}
		protected override void RegisterContent(PdfXRef xRef) {
			base.RegisterContent(xRef);
			pages.Register(xRef);
			if(outlines.Active)
				outlines.Register(xRef);
			if(acroForm.Active)
				acroForm.Register(xRef);
			if(metadata.Active)
				metadata.Register(xRef);
			if(pdfACompatible)
				ICCProfile.Register(xRef);
			if(embeddedFiles.Active)
				embeddedFiles.Register(xRef);
		}
		protected override void WriteContent(StreamWriter writer) {
			base.WriteContent(writer);
			if(outlines.Active)
				outlines.Write(writer);
			if(acroForm.Active)
				acroForm.Write(writer);
			if(metadata.Active)
				metadata.Write(writer);
			if(pdfACompatible)
				ICCProfile.Write(writer);
			if(embeddedFiles.Active)
				embeddedFiles.Write(writer);
			pages.Write(writer);
		}
		public override void FillUp() {
			Dictionary.Add("Type", new PdfName("Catalog"));
			Dictionary.Add("Pages", pages.Dictionary);
			base.FillUp();
			if(outlines.Active) {
				outlines.FillUp();
				Dictionary.Add("Outlines", outlines.InnerObject);
				Dictionary.Add("PageMode", "UseOutlines");
			}
			if(acroForm.Active)
				Dictionary.Add("AcroForm", acroForm.InnerObject);
			if(metadata.Active) {
				metadata.FillUp();
				Dictionary.Add("Metadata", metadata.InnerObject);
			}
			if(pdfACompatible) {
				ICCProfile.FillUp();
				PdfDictionary outputIntent = new PdfDictionary();
				outputIntent.Add("Type", "OutputIntent");
				outputIntent.Add("S", "GTS_PDFA1");
				outputIntent.Add("OutputConditionIdentifier", new PdfLiteralString("sRGB IEC61966-2.1"));
				outputIntent.Add("DestOutputProfile", ICCProfile.InnerObject);
				PdfArray outputIntents = new PdfArray();
				outputIntents.Add(outputIntent);
				Dictionary.Add("OutputIntents", outputIntents);
			}
			if(embeddedFiles.Active) {
				embeddedFiles.FillUp();
				Dictionary.Add("AF", embeddedFiles.AFArray);
				Dictionary.Add("Names", embeddedFiles.NamesDictionary);
			}
			acroForm.FillUp();
			pages.FillUp();
		}
		public void PrepareOutlines() {
			CreateOutlineDestinations(outlines.Entries);
		}
		private void CreateOutlineDestinations(PdfOutlineEntryCollection collection) {
			for(int i = 0; i < collection.Count; i++) {
				PdfOutlineEntry entry = collection[i];
				int index = entry.DestPageIndex;
				PdfPage page = Pages.GetPage(ref index);
				if(page == null) continue;
				entry.SetDestination(new PdfDestination(page, entry.DestTop));
				CreateOutlineDestinations(entry.Entries);
			}
		}
		public void AddFormField(PdfSignatureWidgetAnnotation annotation) {
			acroForm.Fields.Add(annotation.InnerObject);
		}
	}
	public abstract class PdfPageTreeItem : PdfDocumentDictionaryObject {
		PdfRectangle mediaBox;
		PdfPageTreeItem parent;
		public PdfRectangle MediaBox { get { return mediaBox; } set { mediaBox = value; }
		}
		public PdfPageTreeItem Parent { get { return parent; } 
		}
		protected PdfPageTreeItem(PdfPageTreeItem parent, bool compressed)
			: base(compressed) {
			this.parent = parent;
		}
		public override void FillUp() {
			base.FillUp();
			if(mediaBox != null)
				Dictionary.Add("MediaBox", mediaBox);
			if(parent != null)
				Dictionary.Add("Parent", parent.Dictionary);
		}
	}
	public class PdfPages : PdfPageTreeItem {
		ArrayList pages = new ArrayList();
		public PdfPageTreeItem this[int index] { get { return pages[index] as PdfPageTreeItem; } 
		}
		public int Count { get { return pages.Count; } 
		}
		public int LeafCount {
			get {
				int result = 0;
				for(int i = 0; i < Count; i++) {
					if(this[i] is PdfPages)
						result += ((PdfPages)this[i]).LeafCount;
					else
						result++;
				}
				return result;
			}
		}
		public PdfPages(PdfPageTreeItem parent, bool compressed) : base(parent, compressed) {
		}
		PdfArray CreatePdfArray() {
			PdfArray result = new PdfArray();
			for(int i = 0; i < Count; i++)
				result.Add(this[i].Dictionary);
			return result;
		}
		protected override void RegisterContent(PdfXRef xRef) {
			for(int i = 0; i < Count; i++)
				this[i].Register(xRef);			
		}
		protected override void WriteContent(StreamWriter writer) {
			for(int i = 0; i < Count; i++)
				this[i].Write(writer);
		}
		public override void FillUp() {
			Dictionary.Add("Type", new PdfName("Pages"));
			Dictionary.Add("Kids", CreatePdfArray());
			Dictionary.Add("Count", new PdfNumber(LeafCount));
			base.FillUp();
			for(int i = 0; i < Count; i++)
				this[i].FillUp();
		}
		public PdfPage CreatePage() {
			PdfPage page = new PdfPage(this, Compressed);
			pages.Add(page);
			return page;
		}
		public void Clear() {
			pages.Clear();
		}
		public PdfPage GetPage(ref int index) {
			for(int i = 0; i < Count; i++) {
				if(this[i] is PdfPage) {
					index--;
					if(index < 0)
						return this[i] as PdfPage;
				} else {
					PdfPage page = ((PdfPages)this[i]).GetPage(ref index);
					if(page != null)
						return page;
				}
			}
			return null;
		}
	}
	public class PdfPage : PdfPageTreeItem, IPdfContentsOwner {
		PdfContents contents;
		PdfXObjects xObjects = new PdfXObjects();
		PdfFonts fonts = new PdfFonts();
		PdfAnnotations annotations = new PdfAnnotations();
		PdfArray procSet = new PdfArray();
		PdfObjectCollection<PdfTransparencyGS> transparencyGSCollection = new PdfObjectCollection<PdfTransparencyGS>();
		PdfObjectCollection<PdfShading> shadingCollection = new PdfObjectCollection<PdfShading>();
		PdfObjectCollection<PdfPattern> patternCollection = new PdfObjectCollection<PdfPattern>();
		PdfFonts IPdfContentsOwner.Fonts { get { return fonts; } }
		public PdfArray ProcSet { get { return procSet; } }
		public PdfXObjects XObjects { get { return xObjects; } }
		public PdfPage(PdfPageTreeItem parent, bool compressed) : base(parent, compressed) {			
			procSet.Add("PDF");
			procSet.Add("Text");
			procSet.Add("ImageC");
		}
		void FillFonts(PdfDictionary resourceDictionary) {
			if(fonts.Count > 0) {
				PdfDictionary fontDictionary = new PdfDictionary();
				for(int i = 0; i < fonts.Count; i++)
					fontDictionary.Add(fonts[i].Name, fonts[i].Dictionary);
				resourceDictionary.Add("Font", fontDictionary);
			}
		}
		void FillAnnotations() {
			if(annotations.Count > 0) {
				PdfArray annotationArray = new PdfArray();
				for(int i = 0; i < annotations.Count; i++)
					annotationArray.Add(annotations[i].Dictionary);
				Dictionary.Add("Annots", annotationArray);
			}
		}
		void FillProcSets(PdfDictionary resourceDictionary) {
			if(procSet.Count > 0)
				resourceDictionary.Add("ProcSet", procSet);
		}
		public override void FillUp() {
			Dictionary.Add("Type", new PdfName("Page"));
			PdfDictionary resourceDictionary = new PdfDictionary();
			FillFonts(resourceDictionary);
			resourceDictionary.AddIfNotNull("XObject", xObjects.CreateDictionary());
			resourceDictionary.AddIfNotNull("ExtGState", transparencyGSCollection.CreateDictionary());
			resourceDictionary.AddIfNotNull("Shading", shadingCollection.CreateDictionary());
			resourceDictionary.AddIfNotNull("Pattern", patternCollection.CreateDictionary());
			FillProcSets(resourceDictionary);
			Dictionary.Add("Resources", resourceDictionary);
			FillAnnotations();
			Dictionary.Add("Contents", this.contents.InnerObject);
			base.FillUp();
		}
		public void AddAnnotation(PdfAnnotation annotation) {
			annotations.AddUnique(annotation);
		}
		public void AddTransparencyGS(PdfTransparencyGS transparencyGS) {
			transparencyGSCollection.AddUnique(transparencyGS);
		}
		public void AddShading(PdfShading shading) {
			shadingCollection.AddUnique(shading);
		}
		public void AddPattern(PdfPattern pattern) {
			patternCollection.AddUnique(pattern);
		}
		public void InitializeContents(PdfContents contents) {
			if(this.contents == null)
				this.contents = contents;
		}
	}
	public class PdfPrintCatalog : PdfCatalog {
		#region inner classes
		class PdfJavaScriptOwner : PdfDocumentDictionaryObject {
			PdfJavaScript javaScript;
			public PdfJavaScriptOwner(bool compressed)
				: base(compressed) {
				javaScript = new PdfJavaScript(compressed);
			}
			protected override void RegisterContent(PdfXRef xRef) {
				base.RegisterContent(xRef);
				javaScript.Register(xRef);
			}
			protected override void WriteContent(StreamWriter writer) {
				base.WriteContent(writer);
				javaScript.Write(writer);
			}
			public override void FillUp() {
				javaScript.FillUp();
				PdfArray names = new PdfArray();
				names.Add(new PdfLiteralString("0"));
				names.Add(javaScript.InnerObject);
				Dictionary.Add("Names", names);
			}
		}
		class PdfJavaScript : PdfDocumentDictionaryObject {
			public PdfJavaScript(bool compressed)
				: base(compressed) {
			}
			public override void FillUp() {
				Dictionary.Add("S", new PdfName("JavaScript"));
				Dictionary.Add("JS", new PdfLiteralString("this.print({bUI: true,bSilent: false,bShrinkToFit: true});this.closeDoc();"));
			}
		}
		#endregion
		PdfJavaScriptOwner javaScriptOwner;
		public PdfPrintCatalog(bool compressed)
			: base(compressed) {
			javaScriptOwner = new PdfJavaScriptOwner(compressed);
		}
		protected override void RegisterContent(PdfXRef xRef) {
			base.RegisterContent(xRef);
			javaScriptOwner.Register(xRef);
		}
		protected override void WriteContent(StreamWriter writer) {
			base.WriteContent(writer);
			javaScriptOwner.Write(writer);
		}
		public override void FillUp() {
			base.FillUp();
			javaScriptOwner.FillUp();
			PdfDictionary names = new PdfDictionary();
			names.Add("JavaScript", javaScriptOwner.Dictionary);
			Dictionary.Add("Names", names);
		}
	}
	public class PdfMetadata : PdfDocumentStreamObject {
		public bool Active {
			get { return PdfACompatible; }
		}
		public bool PdfACompatible { get; set; }
		public string Author { get; set; }
		public string Application { get; set; }
		public string Title { get; set; }
		public string Subject { get; set; }
		public string Keywords { get; set; }
		public DateTime CreationDate { get; set; }
		public string AdditionalMetadata { get; set; }
		public bool HasEmbeddedFiles { get; set; }
		protected override bool UseFlateEncoding { get { return false; } }
		public PdfMetadata(bool compressed) 
			: base(compressed) {
		}
		public override void FillUp() {
			base.FillUp();
			Attributes.Add("Type", "Metadata");
			Attributes.Add("Subtype", "XML");
			Stream.SetString("<?xpacket begin=\"");
			Stream.SetBytes(new byte[] { 0xEF, 0xBB, 0xBF });
			Stream.SetStringLine("\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?><x:xmpmeta xmlns:x=\"adobe:ns:meta/\">");
			Stream.SetStringLine("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">");
			Stream.SetStringLine("<rdf:Description rdf:about=\"\" xmlns:pdfaid=\"http://www.aiim.org/pdfa/ns/id/\" xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\">");
			Stream.SetStringLine("<pdfaid:part>" + (HasEmbeddedFiles ? "3" : "2") + "</pdfaid:part><pdfaid:conformance>B</pdfaid:conformance>");
			Stream.SetStringLine("<pdf:Producer>" + PdfDocumentOptions.Producer + "</pdf:Producer>");
			AddValue(Keywords, "pdf:Keywords");
			AddValue(Author, "dc:creator", "rdf:Seq", "rdf:li");
			AddValue(Application, "xmp:CreatorTool");
			AddValue(Title, "dc:title", "rdf:Alt", "rdf:li xml:lang=\"x-default\"");
			AddValue(KeywordsAsList(Keywords), "dc:subject", "rdf:Bag");
			AddValue(Subject, "dc:description", "rdf:Alt", "rdf:li xml:lang=\"x-default\"");
#if !DEBUGTEST
			AddValue(CreationDate.ToUniversalTime().ToString("yyyy-MM-dd'T'hh:mm:ss'Z'"), "xmp:CreateDate");
#endif
			Stream.SetStringLine("</rdf:Description>");
			if(!string.IsNullOrEmpty(AdditionalMetadata)) {
				Stream.SetStringLine(AdditionalMetadata);
			}
			string space = new string(' ', 100);
			for(int i = 0; i < 20; i++)
				Stream.SetStringLine(space);
			Stream.SetStringLine("</rdf:RDF></x:xmpmeta><?xpacket end=\"w\"?>");
		}
		static string KeywordsAsList(string keywords) {
			if(string.IsNullOrEmpty(keywords))
				return "";
			return string.Join("", Array.ConvertAll(keywords.Split(';'), x => { string t = x.Trim(); return t.Length == 0 ? "" : "<rdf:li>" + t + "</rdf:li>";}));
		}
		void AddValue(string value, params string[] tags) {
			if(!string.IsNullOrEmpty(value)) {
				Stream.SetString(GetStartTags(tags));
				Stream.SetBytes(System.Text.Encoding.UTF8.GetBytes(value));
				System.Array.Reverse(tags);
				Stream.SetStringLine(GetEndTags(tags));
			}
		}
		static string GetStartTags(string[] tags) {
			return string.Concat(Array.ConvertAll(tags, tag => "<" + tag + ">"));
		}
		static string GetEndTags(string[] tags) {
			return string.Concat(Array.ConvertAll(tags, tag => { int i = tag.IndexOf(' '); return "</" + (i > 0 ? tag.Substring(0, i) : tag) + ">"; })); 
		}
	}
	public class PdfICCProfile : PdfDocumentStreamObject {
		public PdfICCProfile(bool compressed)
			: base(compressed) {
		}
		public override void FillUp() {
			base.FillUp();
			Attributes.Add("N", 3);
			using(Stream stream = DevExpress.Printing.ResFinder.GetManifestResourceStream("Core.PdfExport.ICCProfile.bin")) {
				byte[] bytes = new byte[stream.Length];
				stream.Read(bytes, 0, bytes.Length);
				Stream.SetBytes(bytes);
			}
		}
	}
}

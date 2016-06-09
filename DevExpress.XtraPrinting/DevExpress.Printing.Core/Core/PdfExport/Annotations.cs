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

using System.IO;
using System.Collections;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Export.Pdf {
	public abstract class PdfAnnotation : PdfDocumentDictionaryObject {
		public abstract string Subtype { get; }
		PdfRectangle rect;
		public PdfRectangle Rect {
			get { return rect; }
			set { rect = value; }
		}
		protected PdfAnnotation(bool compressed)
			: this(new PdfRectangle(), compressed) {
		}
		protected PdfAnnotation(PdfRectangle rect, bool compressed)
			: base(compressed) {
			if(rect == null)
				rect = new PdfRectangle();
			this.rect = rect;
		}
		public override void FillUp() {
			Dictionary.Add("Type", "Annot");
			Dictionary.Add("Subtype", Subtype);
			Dictionary.Add("Rect", Rect);
		}
	}
	public class PdfLinkAnnotation : PdfAnnotation {
		static PdfArray borders = new PdfArray();
		PdfAction action;
		static PdfLinkAnnotation() {
			borders.Add(0);
			borders.Add(0);
			borders.Add(0);
		}
		public override string Subtype {
			get { return "Link"; }
		}
		public bool PdfACompatible { get; set; }
		public PdfLinkAnnotation(PdfAction action, bool compressed)
			: base(compressed) {
			Initialize(action);
		}
		public PdfLinkAnnotation(PdfAction action, PdfRectangle rect, bool compressed)
			: base(rect, compressed) {
			Initialize(action);
		}
		void Initialize(PdfAction action) {
			this.action = action;
		}
		protected override void RegisterContent(PdfXRef xRef) {
			if(action != null)
				action.Register(xRef);
		}
		protected override void WriteContent(StreamWriter writer) {
			if(action != null)
				action.Write(writer);
		}
		public override void FillUp() {
			base.FillUp();
			Dictionary.Add("Border", borders);
			if(action != null) {
				Dictionary.Add("A", action.Dictionary);
				action.FillUp();
			}
			if(PdfACompatible) {
				const int Print = 4;
				Dictionary.Add("F", Print);
			}
		}
	}
	public class PdfSignatureWidgetAnnotation : PdfAnnotation {
		PdfPage page;
		PdfSignature sig;
		public override string Subtype {
			get { return "Widget"; }
		}
		public PdfSignatureWidgetAnnotation(PdfPage page, PdfSignature sig, bool compressed)
			: base(compressed) {
			this.page = page;
			this.sig = sig;
		}
		protected override void RegisterContent(PdfXRef xRef) {
			base.RegisterContent(xRef);
			if(sig != null)
				sig.Register(xRef);
		}
		public override void FillUp() {
			base.FillUp();
			Dictionary.Add("FT", "Sig");
			Dictionary.Add("T", new PdfLiteralString("Signature1"));
			Dictionary.Add("F", 132);
			Dictionary.Add("P", page.InnerObject);
			Dictionary.Add("V", sig.InnerObject);
			if(sig != null)
				sig.FillUp();
		}
	}
	public class PdfAnnotations : PdfObjectCollection<PdfAnnotation> {
	}
}

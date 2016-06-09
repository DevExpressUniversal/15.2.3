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
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting {
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions")]
	public class PdfExportOptions : PageByPageExportOptionsBase, IXtraSupportShouldSerialize {
		PdfDocumentOptions documentOptions = new PdfDocumentOptions();
		PdfPasswordSecurityOptions passwordSecurityOptions = new PdfPasswordSecurityOptions();
		PdfJpegImageQuality imageQuality = PdfJpegImageQuality.Highest;
		PdfSignatureOptions signatureOptions = new PdfSignatureOptions();
		string neverEmbeddedFonts = string.Empty;
		bool compressed = true;
		bool showPrintDialogOnOpen;
		bool convertImagesToJpeg = true;
		PdfACompatibility pdfACompatibility = PdfACompatibility.None;
		string additionalMetadata;
#if !DXPORTABLE
		List<PdfAttachment> attachments = new List<PdfAttachment>();
#endif
		public PdfExportOptions() {
		}
		PdfExportOptions(PdfExportOptions source)
			: base(source) {
		}
		#region properties
		protected internal override bool IsMultiplePaged {
			get { return true; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfExportOptionsConvertImagesToJpeg"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.ConvertImagesToJpeg"),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool ConvertImagesToJpeg {
			get { return convertImagesToJpeg; }
			set { convertImagesToJpeg = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfExportOptionsShowPrintDialogOnOpen"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.ShowPrintDialogOnOpen"),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool ShowPrintDialogOnOpen {
			get { return showPrintDialogOnOpen; }
			set { showPrintDialogOnOpen = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfExportOptionsDocumentOptions"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.DocumentOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public PdfDocumentOptions DocumentOptions { get { return documentOptions; } }
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.PasswordSecurityOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public PdfPasswordSecurityOptions PasswordSecurityOptions { get { return passwordSecurityOptions; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfExportOptionsCompressed"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.Compressed"),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool Compressed { get { return compressed; } set { compressed = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfExportOptionsNeverEmbeddedFonts"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.NeverEmbeddedFonts"),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string NeverEmbeddedFonts { get { return neverEmbeddedFonts; } set { neverEmbeddedFonts = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PdfExportOptionsImageQuality"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.ImageQuality"),
		DefaultValue(PdfJpegImageQuality.Highest),
		XtraSerializableProperty,
		]
		public PdfJpegImageQuality ImageQuality { get { return imageQuality; } set { imageQuality = value; } }
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.SignatureOptions")]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public PdfSignatureOptions SignatureOptions { get { return signatureOptions; } }
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.PdfACompatible")]
		[DefaultValue(false)]
		[TypeConverter(typeof(BooleanTypeConverter))]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool PdfACompatible { get { return pdfACompatibility != PdfACompatibility.None; } set { pdfACompatibility = value ? PdfACompatibility.PdfA2b : PdfACompatibility.None; } }
		[DefaultValue(PdfACompatibility.None)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[XtraSerializableProperty]
		[Browsable(false)]
		public PdfACompatibility PdfACompatibility { get { return pdfACompatibility; } set { pdfACompatibility = value; } }
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.AdditionalMetadata")]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string AdditionalMetadata { get { return additionalMetadata; } set { additionalMetadata = value; } }
#if !DXPORTABLE
		[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfExportOptions.Attachments")]
		[XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public ICollection<PdfAttachment> Attachments { get { return attachments; } }
#endif
		#endregion
		bool ShouldSerializeDocumentOptions() {
			return DocumentOptions.ShouldSerialize();
		}
		bool ShouldSerializePasswordSecurityOptions() {
			return PasswordSecurityOptions.ShouldSerialize();
		}
		bool ShouldSerializeSignatureOptions() {
			return SignatureOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeDocumentOptions() || ShouldSerializePasswordSecurityOptions() ||
				ShouldSerializeSignatureOptions() ||
				imageQuality != PdfJpegImageQuality.Highest || compressed != true || showPrintDialogOnOpen != false ||
				neverEmbeddedFonts != "" || base.ShouldSerialize() || !convertImagesToJpeg || pdfACompatibility != PdfACompatibility.None;
		}
		protected internal override ExportOptionsBase CloneOptions() {
			return new PdfExportOptions(this);
		}
		public override void Assign(ExportOptionsBase source) {
			base.Assign(source);
			PdfExportOptions pdfSource = (PdfExportOptions)source;
			documentOptions.Assign(pdfSource.documentOptions);
			passwordSecurityOptions.Assign(pdfSource.passwordSecurityOptions);
			convertImagesToJpeg = pdfSource.convertImagesToJpeg;
			compressed = pdfSource.compressed;
			neverEmbeddedFonts = pdfSource.neverEmbeddedFonts;
			imageQuality = pdfSource.imageQuality;
			showPrintDialogOnOpen = pdfSource.showPrintDialogOnOpen;
			signatureOptions.Assign(pdfSource.signatureOptions);
			pdfACompatibility = pdfSource.PdfACompatibility;
		}
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "DocumentOptions":
					return ShouldSerializeDocumentOptions();
				case "PasswordSecurityOptions":
					return ShouldSerializePasswordSecurityOptions();
				case "SignatureOptions":
					return ShouldSerializeSignatureOptions();
			}
			return true;
		}
	}
	public enum PdfACompatibility {
		None,
		PdfA2b
	}
}

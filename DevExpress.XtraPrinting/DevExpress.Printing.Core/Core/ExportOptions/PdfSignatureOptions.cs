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
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraPrinting {
	[TypeConverter(typeof(DevExpress.XtraPrinting.PdfSignatureOptions.PdfSignatureOptionsTypeConverter))]
	[DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.PdfSignatureOptions")]
#if !SL && !DXPORTABLE
	[Editor("DevExpress.XtraPrinting.Design.PdfSignatureOptionsEditor, " + AssemblyInfo.SRAssemblyPrinting, typeof(System.Drawing.Design.UITypeEditor))]
	[Editor("DevExpress.Xpf.Printing.Native.PdfSignatureOptionsEditor, " + AssemblyInfo.SRAssemblyXpfPrinting, typeof(DevExpress.Xpf.Core.Native.IDXTypeEditor))]
#endif
	public class PdfSignatureOptions : ICloneable {
		public class PdfSignatureOptionsTypeConverter : LocalizableObjectConverter {
			public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
				return false;
			}
			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
				PdfSignatureOptions options = value as PdfSignatureOptions;
				if(destinationType == typeof(string) && value != null) {
					List<string> displayOptions = new List<string>();
#if !SL
					if(options.Certificate != null)
						displayOptions.Add(PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfSignatureOptions_Certificate));
#endif
					if(!string.IsNullOrEmpty(options.Reason) )
						displayOptions.Add(PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfSignatureOptions_Reason));
					if(!string.IsNullOrEmpty(options.Location))
						displayOptions.Add(PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfSignatureOptions_Location));
					if(!string.IsNullOrEmpty(options.ContactInfo))
						displayOptions.Add(PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfSignatureOptions_ContactInfo));
					if(displayOptions.Count == 0)
						displayOptions.Add(PreviewLocalizer.GetString(PreviewStringId.ExportOption_PdfSignatureOptions_None));
					return StringUtils.Join("; ", displayOptions);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		[DefaultValue("")]
		[XtraSerializableProperty]
		public string Reason { get; set; }
		[DefaultValue("")]
		[XtraSerializableProperty]
		public string Location { get; set; }
		[DefaultValue("")]
		[XtraSerializableProperty]
		public string ContactInfo { get; set; }
#if !SL
		[Browsable(false)]
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PdfSignatureOptionsCertificate")]
#endif
		public System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate { get; set; }
#endif        
		public void Assign(PdfSignatureOptions options) {
			if(options == null)
				throw new ArgumentNullException("options");
#if !SL
			this.Certificate = options.Certificate;
#endif        
			this.Reason = options.Reason;
			this.Location = options.Location;
			this.ContactInfo = options.ContactInfo;
		}
		public object Clone() {
			PdfSignatureOptions options = new PdfSignatureOptions();
			options.Assign(this);
			return options;
		}
		public override bool Equals(object obj) {
			PdfSignatureOptions options = obj as PdfSignatureOptions;
			if(options == null)
				return false;
			return this.Reason == options.Reason 
				&& this.Location == options.Location 
				&& this.ContactInfo == options.ContactInfo 
#if !SL
				&& object.Equals(this.Certificate, options.Certificate)
#endif
				;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		internal bool ShouldSerialize() {
			return !string.IsNullOrEmpty(Reason) || !string.IsNullOrEmpty(Location) || !string.IsNullOrEmpty(ContactInfo);
		}
	}
}

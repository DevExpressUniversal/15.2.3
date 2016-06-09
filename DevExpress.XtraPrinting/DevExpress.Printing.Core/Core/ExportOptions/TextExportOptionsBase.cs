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
using System.Reflection;
using System.Text;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting {
	public abstract class TextExportOptionsBase : ExportOptionsBase {
		const BindingFlags PublicStaticFlags = BindingFlags.Static | BindingFlags.Public;
		const TextExportMode DefaultTextExportMode = TextExportMode.Text;
#if !SILVERLIGHT
		static readonly Encoding defaultEncoding = DXEncoding.Default;
#else
		static readonly Encoding defaultEncoding = Encoding.Unicode;
#endif
		Encoding encoding;
		TextExportMode textExportMode = DefaultTextExportMode;
		string separator;
		bool quoteStringsWithSeparators;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextExportOptionsBaseEncoding"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.TextExportOptionsBase.Encoding"),
#if !DXPORTABLE
		 TypeConverter(typeof(EncodingConverter)),
#endif
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public Encoding Encoding {
			get {
				return encoding;
			}
			set {
				if(value == null)
					throw new ArgumentNullException("Encoding");
				if(encoding != value)
					encoding = (Encoding)value.Clone();
			}
		}
		[DefaultValue(DefaultTextExportMode),
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextExportOptionsBaseTextExportMode"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.TextExportOptionsBase.TextExportMode"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty
		]
		public TextExportMode TextExportMode {
			get { return textExportMode; }
			set { textExportMode = value; }
		}
		protected internal override bool IsMultiplePaged {
			get { return false; }
		}
		[
		DefaultValue(EncodingType.Default),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Localizable(true),
		XtraSerializableProperty,
		]
		public EncodingType EncodingType {
			get {
				PropertyInfo[] properties = typeof(Encoding).GetProperties(PublicStaticFlags);
				foreach(PropertyInfo propertyInfo in properties) {
					if(object.Equals(propertyInfo.GetValue(null, null), encoding))
						return (EncodingType)Enum.Parse(typeof(EncodingType), propertyInfo.Name, false);
				}
				return EncodingType.Default;
			}
			set {
				PropertyInfo propertyInfo = typeof(Encoding).GetProperty(value.ToString(), PublicStaticFlags);
				System.Diagnostics.Debug.Assert(propertyInfo != null);
				if(propertyInfo != null)
					Encoding = (Encoding)propertyInfo.GetValue(null, null);
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextExportOptionsBaseSeparator"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.TextExportOptionsBase.Separator"),
#if !DXPORTABLE
		 TypeConverter(typeof(SeparatorConverter)),
#endif
		 Localizable(true),
		 RefreshProperties(RefreshProperties.All),
		 XtraSerializableProperty,
		]
		public string Separator { get { return separator; } set { separator = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextExportOptionsBaseQuoteStringsWithSeparators"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.TextExportOptionsBase.QuoteStringsWithSeparators"),
#if !DXPORTABLE
		 TypeConverter(typeof(QuoteStringsWithSeparatorsConverter)),
#endif
		 XtraSerializableProperty,
		]
		public bool QuoteStringsWithSeparators { get { return quoteStringsWithSeparators; } set { quoteStringsWithSeparators = value; } }
		protected static Encoding DefaultEncoding { get { return defaultEncoding; } }
		TextExportOptionsBase(Encoding encoding) {
			Encoding = encoding;
			quoteStringsWithSeparators = GetDefaultQuoteStringsWithSeparators();
		}
		protected TextExportOptionsBase()
			: this(defaultEncoding) {
		}
		protected TextExportOptionsBase(string separator, Encoding encoding)
			: this(encoding) {
			this.separator = separator;
		}
		protected TextExportOptionsBase(string separator, Encoding encoding, TextExportMode textExportMode)
			: this(separator, encoding) {
			this.textExportMode = textExportMode;
		}
		protected TextExportOptionsBase(TextExportOptionsBase source)
			: base(source) {
		}
		public override void Assign(ExportOptionsBase source) {
			TextExportOptionsBase textSource = (TextExportOptionsBase)source;
			Encoding = textSource.Encoding;
			separator = textSource.Separator;
			quoteStringsWithSeparators = textSource.QuoteStringsWithSeparators;
			textExportMode = textSource.TextExportMode;
		}
		protected abstract string GetDefaultSeparator();		
		protected abstract bool GetDefaultQuoteStringsWithSeparators();
		bool ShouldSerializeSeparator() {
			return separator != GetDefaultSeparator();
		}
		bool ShouldSerializeEncoding() {
			return !Object.Equals(encoding, defaultEncoding);
		}
		bool ShouldSerializeQuoteStringsWithSeparators() {
			return quoteStringsWithSeparators != GetDefaultQuoteStringsWithSeparators();
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeEncoding() || ShouldSerializeSeparator() || ShouldSerializeQuoteStringsWithSeparators() ||
				textExportMode != DefaultTextExportMode || EncodingType != EncodingType.Default;
		}
	}
}

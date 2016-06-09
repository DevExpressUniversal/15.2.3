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
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UI {
	#region XRRichTextBase
	[
	ToolboxItem(false),
	DefaultBindableProperty("Rtf"),
	DefaultProperty("Text"),
	]
	public abstract class XRRichTextBase : XRFieldEmbeddableControl {
		protected static class PropertyNames {
			public const string Rtf = "Rtf";
			public const string Html = "Html";
			public const string Multiline = "Multiline";
			public const string DetectUrls = "DetectUrls";
		}
		string rtf = string.Empty;
		protected override string DisplayProperty {
			get { return Rtf; }
			set { Rtf = value; }
		}
		protected override string DisplayPropertyName {
			get { return "Rtf"; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBaseWordWrap"),
#endif
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Bindable(true),
		DefaultValue(""),
		]
		public virtual string Rtf { get { return rtf; } set { rtf = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBaseRtfText"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRRichTextBase.RtfText"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public SerializableString RtfText {
			get { return new SerializableString(Rtf); }
			set {
				Rtf = value != null ? value.Value : string.Empty;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(""),
		XtraSerializableProperty,
		]
		public string SerializableRtfString {
			get {
				return Convert.ToBase64String(Encoding.Unicode.GetBytes(Rtf));
			}
			set {
				Rtf = new string(Encoding.Unicode.GetChars(Convert.FromBase64String(value)));
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBaseProcessNullValues"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.ProcessNullValues"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override ValueSuppressType ProcessNullValues {
			get { return base.ProcessNullValues; }
			set { base.ProcessNullValues = value; }
		}
		[
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		]
		public override string NullValueText {
			get { return base.NullValueText; }
			set { base.NullValueText = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBaseProcessDuplicatesMode"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.ProcessDuplicatesMode"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override ProcessDuplicatesMode ProcessDuplicatesMode {
			get { return base.ProcessDuplicatesMode; }
			set { base.ProcessDuplicatesMode = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRRichTextBaseProcessDuplicatesTarget"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLabel.ProcessDuplicatesTarget"),
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override ProcessDuplicatesTarget ProcessDuplicatesTarget {
			get { return base.ProcessDuplicatesTarget; }
			set { base.ProcessDuplicatesTarget = value; }
		}
		protected XRRichTextBase()
			: base() {
			KeepTogether = false;
		}
		bool ShouldSerializeRtfText() {
			return !string.IsNullOrEmpty(Text);
		}
		protected override string ConvertValue(object val) {
			Image image = val is Image ? (Image)val : val is Byte[] ? DevExpress.XtraPrinting.Native.PSConvert.ImageFromArray((Byte[])val) : null;
			if(image != null) {
				return DevExpress.XtraPrinting.Export.Rtf.RtfExportProvider.GetRtfImageContent(image, image.RawFormat);
			}
			if(val is string) 
				return this.GetMailMergeFieldInfosCalculator().PrepareString((string)val);
			return base.ConvertValue(val);
		}
		protected override void SetBrickText(VisualBrick brick, string text, object textValue) {
		}
		protected override bool NeedSuppressNullValue() {
			return IsEmptyValue(Rtf);
		}
		protected override object GetValueForSuppress() {
			return Rtf;
		}
	}
	#endregion // XRRichTextBase
}

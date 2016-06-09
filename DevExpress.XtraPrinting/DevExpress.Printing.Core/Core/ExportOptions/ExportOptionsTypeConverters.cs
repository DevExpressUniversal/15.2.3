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

using System.ComponentModel;
using System;
using DevExpress.XtraPrinting;
using System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
#if SILVERLIGHT
using DevExpress.Xpf.ComponentModel;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.Utils.Design {
	public abstract class PageRangeConverter : StringConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(context == null)
				return sourceType == typeof(string);
			if(GetOptionsDisableValue(context))
				return false;
			return sourceType != typeof(string) ? base.CanConvertFrom(context, sourceType) : true;
		}
		protected abstract bool GetOptionsDisableValue(ITypeDescriptorContext context);
	}
	public class HtmlPageRangeConverter : PageRangeConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			HtmlExportOptionsBase options = context.Instance as HtmlExportOptionsBase;
			if(options != null && options.ExportMode == HtmlExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class HtmlPageBorderWidthConverter : Int32Converter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(context == null)
				return sourceType == typeof(string);
			if(GetOptionsDisableValue(context))
				return false;
			return sourceType != typeof(string) ? base.CanConvertFrom(context, sourceType) : true;
		}
		protected virtual bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			HtmlExportOptionsBase options = context.Instance as HtmlExportOptionsBase;
			if(options != null && options.ExportMode == HtmlExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public abstract class ExportWatermarksConverter : BooleanTypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(context == null)
				return sourceType == typeof(string);
			if(GetOptionsDisableValue(context))
				return false;
			return sourceType != typeof(string) ? base.CanConvertFrom(context, sourceType) : true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			if(context == null)
				return base.GetStandardValuesSupported(context);
			if(GetOptionsDisableValue(context))
				return false;
			return base.GetStandardValuesSupported(context);
		}
		protected abstract bool GetOptionsDisableValue(ITypeDescriptorContext context);
	}
	public class RtfPageRangeConverter : PageRangeConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			RtfExportOptions options = context.Instance as RtfExportOptions;
			if(options != null && options.ExportMode == RtfExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class ImagePageRangeConverter : PageRangeConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			ImageExportOptions options = context.Instance as ImageExportOptions;
			if(options != null && options.ExportMode == ImageExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class XlsPageRangeConverter : PageRangeConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			XlsExportOptions options = context.Instance as XlsExportOptions;
			if(options != null && options.ExportMode == XlsExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class XlsxPageRangeConverter : PageRangeConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			XlsxExportOptions options = context.Instance as XlsxExportOptions;
			if(options != null && options.ExportMode == XlsxExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class ImagePageBorderWidthConverter : HtmlPageBorderWidthConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			ImageExportOptions options = context.Instance as ImageExportOptions;
			if(options != null && options.ExportMode == ImageExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class ImagePageBorderColorConverter : HtmlPageBorderColorConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			ImageExportOptions options = context.Instance as ImageExportOptions;
			if(options != null && options.ExportMode == ImageExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class QuoteStringsWithSeparatorsConverter : ExportWatermarksConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			TextExportOptionsBase options = context.Instance as TextExportOptionsBase;
			if(options != null && string.IsNullOrEmpty(options.Separator))
				return true;
			return false;
		}
	}
	public class RtfExportWatermarksConverter : ExportWatermarksConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			RtfExportOptions options = context.Instance as RtfExportOptions;
			if(options != null && options.ExportMode == RtfExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class HtmlPageBorderColorConverter : ColorConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(context == null)
				return sourceType == typeof(string);
			if(GetOptionsDisableValue(context))
				return false;
			return sourceType != typeof(string) ? base.CanConvertFrom(context, sourceType) : true;
		}
		protected virtual bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			HtmlExportOptionsBase options = context.Instance as HtmlExportOptionsBase;
			if(options != null && options.ExportMode == HtmlExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class HtmlExportWatermarksConverter : ExportWatermarksConverter {
		protected override bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			HtmlExportOptionsBase options = context.Instance as HtmlExportOptionsBase;
			if(options != null && options.ExportMode == HtmlExportMode.SingleFile)
				return true;
			return false;
		}
	}
	public class RtfSingleFileOptionConverter : BooleanTypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(context == null)
				return sourceType == typeof(string);
			if(GetOptionsDisableValue(context))
				return false;
			return sourceType != typeof(string) ? base.CanConvertFrom(context, sourceType) : true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			if(context == null)
				return base.GetStandardValuesSupported(context);
			if(GetOptionsDisableValue(context))
				return false;
			return base.GetStandardValuesSupported(context);
		}
		protected bool GetOptionsDisableValue(ITypeDescriptorContext context) {
			RtfExportOptions options = context.Instance as RtfExportOptions;
			if(options != null && options.ExportMode == RtfExportMode.SingleFilePageByPage)
				return true;
			return false;
		}
	}
}

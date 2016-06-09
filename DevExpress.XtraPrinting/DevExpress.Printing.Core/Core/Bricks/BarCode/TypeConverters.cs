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
using System.Globalization;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils;
using DevExpress.Data.Browsing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Parameters;
using System.Drawing.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.XtraReports.Design {
	public class DataBarExpandedWidthConverter : Int32Converter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			DataBarGenerator generator = context.Instance as DataBarGenerator;
			return sourceType != typeof(string) || generator != null && generator.Type == DataBarType.ExpandedStacked ?
				base.CanConvertFrom(context, sourceType) : false;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			DataBarGenerator generator = context.Instance as DataBarGenerator;
			return destinationType != typeof(string) || generator != null && generator.Type == DataBarType.ExpandedStacked ?
				base.ConvertTo(context, culture, value, destinationType) :
				Utils.Design.DesignSR.NoneValueString;
		}
	}
	public class DataBarExpandedFNC1Converter : StringConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			DataBarGenerator generator = context.Instance as DataBarGenerator;
			return sourceType != typeof(string) || generator != null && (generator.Type == DataBarType.ExpandedStacked || generator.Type == DataBarType.Expanded) ?
				base.CanConvertFrom(context, sourceType) : false;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			DataBarGenerator generator = context.Instance as DataBarGenerator;
			return destinationType != typeof(string) || generator != null && (generator.Type == DataBarType.ExpandedStacked || generator.Type == DataBarType.Expanded) ?
				base.ConvertTo(context, culture, value, destinationType) :
				Utils.Design.DesignSR.NoneValueString;
		}
	}
	public class Code128LeadZeroConverter : DevExpress.Utils.Design.BooleanTypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			Code128Generator generator = context.Instance as Code128Generator;
			return (generator != null && generator.CharacterSet == Code128Charset.CharsetC) ?
				base.GetStandardValuesSupported(context) : false;
		}
	}
}

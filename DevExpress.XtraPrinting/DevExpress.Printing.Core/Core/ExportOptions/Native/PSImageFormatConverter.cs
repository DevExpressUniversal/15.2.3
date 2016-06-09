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

using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if !SILVERLIGHT
using System.Drawing;
using System.Drawing.Imaging;
#else
using DevExpress.Xpf.ComponentModel;
using DevExpress.Xpf.Drawing;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
using DevExpress.Xpf.Drawing.Imaging;
#endif
namespace DevExpress.XtraPrinting.Native {
#if DXPORTABLE
	public class PSImageFormatConverter : TypeConverter {
	}
#else
	public class PSImageFormatConverter : ImageFormatConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection values = base.GetStandardValues(context);
			List<ImageFormat> supportedValues = new List<ImageFormat>();
			foreach(ImageFormat format in values) {
				if(ImageExportOptions.GetImageFormatSupported(format))
					supportedValues.Add(format);
			}
			return new StandardValuesCollection(supportedValues);
		}
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return base.ConvertTo(context, culture, value, destinationType).ToString().ToUpper(culture);
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
#endif
	}

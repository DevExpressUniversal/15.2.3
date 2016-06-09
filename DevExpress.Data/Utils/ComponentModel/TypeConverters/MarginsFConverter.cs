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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using DevExpress.XtraPrinting.Native;
using System.Xml;
using DevExpress.Utils.Serializing;
#if SILVERLIGHT
using DevExpress.Xpf.ComponentModel;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
#endif
namespace DevExpress.Utils.Design {
	public class MarginsFConverter : TypeConverter {		
		#region inner classes
		class XmlMarginsFConverter : StructFloatConverter {
			public static readonly XmlMarginsFConverter Instance = new XmlMarginsFConverter();
			public override Type Type { get { return typeof(MarginsF); } }
			XmlMarginsFConverter() {
			}
			protected override float[] GetValues(object obj) {
				MarginsF margins = (MarginsF)obj;
				return new float[] { margins.Left, margins.Right, margins.Top, margins.Bottom };
			}
			protected override object CreateObject(float[] values) {
				return new MarginsF(values[0], values[1], values[2], values[3]);
			}
		}
		#endregion
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return (destinationType == typeof(string));
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str != null)
				return XmlMarginsFConverter.Instance.FromString(str);
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null)
				throw new ArgumentNullException("destinationType");
			if(value is MarginsF && destinationType == typeof(string))
				return XmlMarginsFConverter.Instance.ToString(value);
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

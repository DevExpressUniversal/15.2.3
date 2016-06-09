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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.Diagram.Core.TypeConverters {
	public sealed class DiagramTextDecorationCollectionConverter : TypeConverterWrapper, IOneTypeObjectConverter {
		readonly TypeConverter baseConverter;
		public DiagramTextDecorationCollectionConverter() {
			this.baseConverter = new TextDecorationCollectionConverter();
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if((destinationType == typeof(string)) && (value is TextDecorationCollection)) {
				TextDecorationCollection col = (TextDecorationCollection)value;
				string id = string.Empty;
				if(FontHelper.TextDecorationsToIsUnderline(col)) {
					id += "UNDERLINE";
				}
				if(FontHelper.TextDecorationsToIsStrikethrough(col)) {
					if(!string.IsNullOrEmpty(id)) id += ", ";
					id += "STRIKETHROUGH";
				}
				return id;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected override ITypeDescriptorContext GetWrapperContext(ITypeDescriptorContext context) {
			return context;
		}
		protected override TypeConverter BaseConverter { get { return baseConverter; } }
		#region IOneTypeObjectConverter
		string IOneTypeObjectConverter.ToString(object obj) {
			return ConvertTo(obj, typeof(string)) as string;
		}
		object IOneTypeObjectConverter.FromString(string value) {
			return ConvertFrom(value);
		}
		Type IOneTypeObjectConverter.Type { get { return typeof(TextDecorationCollection); } }
		#endregion
	}
}

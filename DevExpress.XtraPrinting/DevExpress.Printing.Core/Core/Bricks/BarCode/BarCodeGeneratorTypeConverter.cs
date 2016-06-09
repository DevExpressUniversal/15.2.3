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
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class BarCodeGeneratorTypeConverter : ExpandableObjectConverter {
		#region static
		static string DoConvertToString(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			BarCodeGeneratorBase generator = value as BarCodeGeneratorBase;			
			if(generator == null)
				return value.ToString();
			BarCodeSymbology symbology = generator.SymbologyCode;
			return symbology.ToString();
		}
		static InstanceDescriptor DoConvertToInstanceDescriptor(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { });
			return new InstanceDescriptor(ci, new object[0], false);
		}
		#endregion
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor))
				return true;
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string) && context != null && context.Instance != null)
				return DoConvertToString(context, culture, value, destinationType);
			if(destinationType == typeof(InstanceDescriptor))
				return DoConvertToInstanceDescriptor(context, culture, value, destinationType);
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

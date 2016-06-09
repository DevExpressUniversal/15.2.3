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
using System.ComponentModel.Design;
using System.Globalization;
namespace DevExpress.XtraSplashScreen.Design {
	class SplashScreenTypeConverter : TypeConverter {
		static string noneItem = "(None)";
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string str = value as string;
			if(str != null) {
				if(string.Equals(str, noneItem, StringComparison.Ordinal))
					return null;
				SplashScreenManagerDesigner designer = ConverterHelper.GetDesignerObject(context);
				TypeInfo result = designer.ScreensStorage.Parse(str);
				if(result == null)
					throw new InvalidOperationException();
				return result;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType != typeof(string))
				return base.ConvertTo(context, culture, value, destinationType);
			if(value == null)
				return noneItem;
			TypeInfo item = value as TypeInfo;
			if(item == null)
				throw new InvalidOperationException();
			return item.ToString();
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SplashScreenManagerDesigner designer = ConverterHelper.GetDesignerObject(context);
			designer.ScreensStorage.Refresh();
			return new TypeConverter.StandardValuesCollection(ConverterHelper.CreateList(designer.ScreensStorage, context));
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
	}
	class ConverterHelper {
		public static SplashScreenManagerDesigner GetDesignerObject(ITypeDescriptorContext context) {
			IDesignerHost host = (IDesignerHost)context.GetService(typeof(IDesignerHost));
			return (SplashScreenManagerDesigner)host.GetDesigner(GetComponent(context));
		}
		static IComponent GetComponent(ITypeDescriptorContext context) {
			if(context.Instance is IComponent)
				return (IComponent)context.Instance;
			if(context.Instance is DesignerActionList)
				return ((DesignerActionList)context.Instance).Component;
			else throw new InvalidOperationException("Can't get SplashScreenManager object");
		}
		public static List<TypeInfo> CreateList(SplashScreensInfoStorage storage, ITypeDescriptorContext context) {
			List<TypeInfo> res = new List<TypeInfo>();
			bool rootUserControl = VSServiceHelper.IsUserControlRootComponent(GetComponent(context));
			res.Add(null);
			foreach(TypeInfo ti in storage.Items) {
				if(ShouldAddTypeInfo(ti, rootUserControl)) res.Add(ti);
			}
			return res;
		}
		static bool ShouldAddTypeInfo(TypeInfo ti, bool rootUserControl) {
			if(rootUserControl) return ti.Mode == Mode.WaitForm;
			return true;
		}
	}
}

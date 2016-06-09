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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Design {
	public class FileManagerFileSystemProviderTypeNameConverter : ReferenceConverter {
		public const string NoneValue = "(none)";
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			if(val is string)
				return (string)val == NoneValue ? string.Empty : val;
			return base.ConvertFrom(context, culture, val);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) ? true : base.CanConvertTo(context, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(string) ? true : base.CanConvertFrom(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
			if(destType == typeof(string))
				return (val == null || (string)val == String.Empty) ? NoneValue : val;
			return base.ConvertTo(context, culture, val, destType);
		}
		public FileManagerFileSystemProviderTypeNameConverter() : base(typeof(string)) { }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SortedList list = new SortedList();
			list.Add(NoneValue, null);
			ITypeDiscoveryService srv = (ITypeDiscoveryService)((IServiceProvider)((System.Web.UI.Control)context.Instance).Site).GetService(typeof(ITypeDiscoveryService));
			foreach(Type type in srv.GetTypes(typeof(object), true)) {
				if(typeof(FileSystemProviderBase).IsAssignableFrom(type) && !IsPredefinedFileSystemProvider(type))
					list.Add(type.FullName, type.FullName);
			}
			return new StandardValuesCollection(list.Values);
		}
		internal static bool IsPredefinedFileSystemProvider(Type type) {
			return type.IsDefined(typeof(PredefinedFileSystemProviderAttribute), false);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
}

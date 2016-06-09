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
namespace DevExpress.XtraDiagram.Utils {
	public class TypeConvesionUtils {
		public static string ToString(object obj) {
			if(obj == null) {
				throw new ArgumentNullException("val");
			}
			TypeConverter c = TypeDescriptor.GetConverter(obj);
			if(c == null || !c.CanConvertFrom(typeof(string))) {
				throw new ArgumentException("There is no appropriate converter to string");
			}
			return (string)c.ConvertTo(null, CultureInfo.InvariantCulture,  obj, typeof(string));
		}
		public static T ToObject<T>(string stringValue) where T : class {
			if(string.IsNullOrEmpty(stringValue)) {
				throw new ArgumentNullException("val");
			}
			TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
			if(c == null || !c.CanConvertFrom(typeof(string))) {
				throw new ArgumentException("There is no appropriate converter from string");
			}
			return (T)c.ConvertFrom(null, CultureInfo.InvariantCulture, stringValue);
		}
	}
}

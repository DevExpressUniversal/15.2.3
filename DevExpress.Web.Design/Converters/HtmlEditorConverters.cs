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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	public class ShortcutConverter : ReferenceConverter {
		public const string NoneValue = "(none)";
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			if(val is string) {
				if((string)val == NoneValue)
					return string.Empty;
				if(string.IsNullOrEmpty(HtmlEditorShortcut.GetShortcutString((string)val)))
					return string.Empty;
				return val;
			}
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
		public ShortcutConverter() : base(typeof(string)) { }
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			SortedList list = new SortedList();
			List<string> scs = new List<string>() { NoneValue };
			List<string> keys = GetKeys();
			List<string> mods = GetModificator(string.Empty, 0, HtmlEditorShortcut.ModificatorKeys);
			foreach(string mod in mods)
				scs.AddRange(keys.ConvertAll(k => string.Format("{0}{1}", mod, k)));
			scs.AddRange(keys);
			return new StandardValuesCollection(scs);
		}
		List<string> GetModificator(string curString, int curIndex, string[] modArray) {
			List<string> res = new List<string>();
			for(int i = curIndex; i < modArray.Length; i++) {
				string mod = curString + modArray[i] + "+";
				res.Add(mod);
				res.AddRange(GetModificator(mod, i + 1, modArray));
			}
			return res;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public List<string> GetKeys() {
			List<string> keys = new List<string>(Array.ConvertAll("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(), c => c.ToString()));
			keys.AddRange(HtmlEditorShortcut.NonSymbolKeys);
			for(int i = 1; i <= 12; i++)
				keys.Add(string.Format("F{0}", i));
			return keys;
		}
	}
}

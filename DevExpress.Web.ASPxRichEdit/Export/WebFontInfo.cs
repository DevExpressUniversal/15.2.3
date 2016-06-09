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

using DevExpress.Utils.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class WebFontInfo : IHashtableProvider {
		public string Name { get; private set; }
		public string CssString { get; set; }
		public double ScriptMultiplier { get; set; }
		public double SubScriptOffset { get; set; }
		public bool CanBeSet { get; set; }
		public WebFontInfo(string name, bool canBeSet)
			:this(name, canBeSet, name) { }		
		public WebFontInfo(string name, bool canBeSet, string cssString) {
			Name = name;
			CssString = cssString;
			var descriptor = FontManager.GetFontDescriptor(name, false, false);
			ScriptMultiplier = (double)descriptor.FontInfo.GetSuperscriptYSize(100) / 100;
			SubScriptOffset = (double)descriptor.FontInfo.GetSubscriptYOffset(100) / 100;
			CanBeSet = canBeSet;
		}
		public void FillHashtable(System.Collections.Hashtable result) {
			result[((int)JSONFontInfoProperty.Name).ToString()] = Name;
			result[((int)JSONFontInfoProperty.ScriptMultiplier).ToString()] = ScriptMultiplier;
			result[((int)JSONFontInfoProperty.CssString).ToString()] = CssString;
			result[((int)JSONFontInfoProperty.CanBeSet).ToString()] = CanBeSet;
			result[((int)JSONFontInfoProperty.SubScriptOffset).ToString()] = SubScriptOffset;
		}
		public override bool Equals(object obj) {
			var prop = obj as WebFontInfo;
			if (prop == null)
				return false;
			return prop.Name == Name;
		}
		public override int GetHashCode() {
			return Name.GetHashCode();
		}
	}
}

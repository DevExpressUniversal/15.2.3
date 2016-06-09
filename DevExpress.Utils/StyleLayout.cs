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
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
namespace DevExpress.Utils {
	#region Styles
	public class StyleLayout {
		static void WriteStyle(XmlTextWriter tw, AppearanceObject vs) {
		}
		public static bool ValidStringForColor(string s) {
			int i = s.IndexOf("[");
			if(s.Substring(0, 5) != "Color" || i == -1) return false;
			return true;
		}
		public static Color ColorFromString(string s) {
			if(ValidStringForColor(s)) {
				int i = s.IndexOf("[") + 1, j;
				string ds = s.Substring(i, s.Length - i - 1);
				if(ds.Substring(0, 2) == "R=") ds = "A=255, " + ds;
				if(ds.Substring(0, 2) != "A=") {
					if(ds == "Empty") return Color.Empty;
					return Color.FromName(ds);
				}
				else {
					return Color.FromArgb(
						Convert.ToInt32(ds.Substring(i = 2, (j = ds.IndexOf("R=") - 2) - i)), 
						Convert.ToInt32(ds.Substring(j + 4, (i = ds.IndexOf("G=") - 2) - j - 4)), 
						Convert.ToInt32(ds.Substring(i + 4, (j = ds.IndexOf("B=") - 2) - i - 4)),
						Convert.ToInt32(ds.Substring(j + 4, ds.Length - j - 4)));
				}
			} else 
				return Color.Empty;
		}
	}
	#endregion
	#region Appearance
	public class AppearanceLayoutHelper {
		public static bool IsExistAttribute(System.Xml.XmlNode node, string name) {
			if(node == null) return true;
			return node.Attributes[name] != null;
		}
		public static Color GetColorByAttribute(System.Xml.XmlNode node, string attribute) {
			if(IsExistAttribute(node, attribute)) {
				string color = node.Attributes[attribute].Value;
				if(StyleLayout.ValidStringForColor(color))
					return StyleLayout.ColorFromString(color);
			} 
			return Color.Empty;
		}
		public static void SetAppearanceFontColors(System.Xml.XmlNode node, AppearanceObject appObject) {
			if(IsExistAttribute(node, "fontname")) {
				string fontName = node.Attributes["fontname"].Value;
				Single fontSize = 8;
				if(IsExistAttribute(node, "fontsize")) {
					string fs = node.Attributes["fontsize"].Value.Replace(",", "."); 
					fontSize = Convert.ToSingle(fs, System.Globalization.CultureInfo.InvariantCulture);
				}
				if(fontSize > 8 && fontSize < 9) fontSize = 8;
				FontStyle fontStyle = FontStyle.Regular;
				if(IsExistAttribute(node, "fontstyle"))
					fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), node.Attributes["fontstyle"].Value);
				try {
					appObject.Font = new Font(fontName, fontSize, fontStyle);
				} 
				catch {}
			}
			appObject.BackColor = GetColorByAttribute(node, "backcolor");
			appObject.BackColor2 = GetColorByAttribute(node, "backcolor2");
			appObject.BorderColor = GetColorByAttribute(node, "bordercolor");
			appObject.ForeColor = GetColorByAttribute(node, "forecolor");
			if(node.Attributes["gradient"] != null) {
				appObject.GradientMode = (System.Drawing.Drawing2D.LinearGradientMode)Enum.Parse(typeof(System.Drawing.Drawing2D.LinearGradientMode), node.Attributes["gradient"].Value);
			}
		}
	}
	#endregion
}

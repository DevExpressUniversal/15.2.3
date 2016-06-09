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

using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraRichEdit.SyntaxEdit;
namespace DevExpress.XtraReports.Design {
	public class SyntaxColors : ISyntaxColors {
		static Color DefaultErrorColor { get { return Color.Red; } }
		static Color DefaultCommentColor { get { return Color.Green; } }
		static Color DefaultKeywordColor { get { return Color.Blue; } }
		static Color DefaultStringColor { get { return Color.Brown; } }
		static Color DefaultXmlCommentColor { get { return Color.Gray; } }
		static Color DefaultTextColor { get { return Color.Black; } }
		static Color DefaultLineNumbersColor { get { return Color.FromArgb(43, 145, 175); } }
		static Color DefaultLineColor { get { return Color.FromArgb(175, DefaultLineNumbersColor); } }
		static Color DefaultBracketHighlightColor { get { return Color.FromArgb(40, Color.Black); } }
		static Color DefaultBackgroundColor { get { return Color.White; } }
		UserLookAndFeel lookAndFeel;
		public SyntaxColors(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		#region ISyntaxColors Members
		public Color BackgroundColor {
			get {
				Color defaultColor = GetCommonColorByName(CommonColors.Window, DefaultBackgroundColor);
				return GetColorByName("BackgroundColor", defaultColor);
			}
		}
		public Color ControlColor { get { return GetCommonColorByName(CommonColors.Control, DefaultTextColor); } }
		public Color CommentColor { get { return GetCommonColorByName(CommonSkins.SkinInformationColor, DefaultCommentColor); } }
		public Color KeywordColor { get { return GetCommonColorByName(CommonSkins.SkinQuestionColor, DefaultKeywordColor); } }
		public Color ErrorColor { get { return GetCommonColorByName(CommonSkins.SkinCriticalColor, DefaultErrorColor); } }
		public Color TextColor { get { return GetCommonColorByName(CommonColors.WindowText, DefaultTextColor); } }
		public Color XmlCommentColor { get { return GetCommonColorByName(CommonColors.DisabledText, DefaultXmlCommentColor); } }
		public Color StringColor { get { return GetCommonColorByName(CommonSkins.SkinWarningColor, DefaultStringColor); } }
		public Color LineNumbersColor { get { return GetColorByName("LineNumbersColor", DefaultLineNumbersColor); } }
		public Color LineColor { get { return GetColorByName("LineColor", DefaultLineColor); } }
		public Color BracketHighlightColor { get { return GetColorByName("BracketHighlightColor", DefaultBracketHighlightColor); } }
		#endregion
		Color GetColorByName(string colorName, Color defaultColor) {
			Skin skin = ReportsSkins.GetSkin(lookAndFeel);
			SkinElement skinEl = skin[ReportsSkins.SkinScriptControl];
			if(skinEl == null)
				return defaultColor;
			Color color = skinEl.Properties.GetColor(colorName);
			int alpha = skinEl.Properties.GetInteger(colorName + "Alpha");
			if(alpha != 0)
				return Color.FromArgb(alpha, color);
			if(color == Color.Empty)
				return defaultColor;
			return color;
		}
		Color GetCommonColorByName(string colorName, Color defaultColor) {
			Skin skin = CommonSkins.GetSkin(lookAndFeel);
			if(skin == null)
				return defaultColor;
			return skin.Colors[colorName];
		}
	}
}

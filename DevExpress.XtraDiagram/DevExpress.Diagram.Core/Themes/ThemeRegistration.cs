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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using DevExpress.Diagram.Core.Localization;
using DevExpress.Diagram.Core.Shapes.Native;
namespace DevExpress.Diagram.Core {
	public static class DiagramThemes {
		public static DiagramTheme Office  { get { return ThemeRegistrator.GetTheme("Office"); } }
		public static DiagramTheme Linear  { get { return ThemeRegistrator.GetTheme("Linear"); } }
		public static DiagramTheme NoTheme  { get { return ThemeRegistrator.GetTheme("NoTheme"); } }
		public static DiagramTheme Integral  { get { return ThemeRegistrator.GetTheme("Integral"); } }
		public static DiagramTheme Daybreak  { get { return ThemeRegistrator.GetTheme("Daybreak"); } }
		public static DiagramTheme Parallel  { get { return ThemeRegistrator.GetTheme("Parallel"); } }
		public static DiagramTheme Sequence  { get { return ThemeRegistrator.GetTheme("Sequence"); } }
		public static DiagramTheme Lines  { get { return ThemeRegistrator.GetTheme("Lines"); } }
	}
	internal static class ThemeRegistratorHelper {
		readonly static Dictionary<string, DiagramControlStringId> themeStringIdTable;
		static ThemeRegistratorHelper() {
			themeStringIdTable = new Dictionary<string, DiagramControlStringId>();
			themeStringIdTable.Add("Office", DiagramControlStringId.Themes_Office_Name);
			themeStringIdTable.Add("Linear", DiagramControlStringId.Themes_Linear_Name);
			themeStringIdTable.Add("NoTheme", DiagramControlStringId.Themes_NoTheme_Name);
			themeStringIdTable.Add("Integral", DiagramControlStringId.Themes_Integral_Name);
			themeStringIdTable.Add("Daybreak", DiagramControlStringId.Themes_Daybreak_Name);
			themeStringIdTable.Add("Parallel", DiagramControlStringId.Themes_Parallel_Name);
			themeStringIdTable.Add("Sequence", DiagramControlStringId.Themes_Sequence_Name);
			themeStringIdTable.Add("Lines", DiagramControlStringId.Themes_Lines_Name);
		}
		public static DiagramControlStringId GetThemeStringId(string themeId) {
			return themeStringIdTable[themeId];
		}
	}
}

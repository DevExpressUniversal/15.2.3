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
	public enum DiagramThemeColorId {
		White,
		Black,
		Light,
		Dark,
		Accent1,
		Accent2,
		Accent3,
		Accent4,
		Accent5,
		Accent6,
		White_1,
		Black_1,
		Light_1,
		Dark_1,
		Accent1_1,
		Accent2_1,
		Accent3_1,
		Accent4_1,
		Accent5_1,
		Accent6_1,
		White_2,
		Black_2,
		Light_2,
		Dark_2,
		Accent1_2,
		Accent2_2,
		Accent3_2,
		Accent4_2,
		Accent5_2,
		Accent6_2,
		White_3,
		Black_3,
		Light_3,
		Dark_3,
		Accent1_3,
		Accent2_3,
		Accent3_3,
		Accent4_3,
		Accent5_3,
		Accent6_3,
		White_4,
		Black_4,
		Light_4,
		Dark_4,
		Accent1_4,
		Accent2_4,
		Accent3_4,
		Accent4_4,
		Accent5_4,
		Accent6_4,
		White_5,
		Black_5,
		Light_5,
		Dark_5,
		Accent1_5,
		Accent2_5,
		Accent3_5,
		Accent4_5,
		Accent5_5,
		Accent6_5,
	}
	public static class DefaultDiagramStyleId {
		public static readonly DiagramItemStyleId Variant1 = DiagramItemStyleId.Create("Variant1", 1, theme => theme.VariantShapeStyles[0]);
		public static readonly DiagramItemStyleId Variant2 = DiagramItemStyleId.Create("Variant2", 2, theme => theme.VariantShapeStyles[1]);
		public static readonly DiagramItemStyleId Variant3 = DiagramItemStyleId.Create("Variant3", 3, theme => theme.VariantShapeStyles[2]);
		public static readonly DiagramItemStyleId Variant4 = DiagramItemStyleId.Create("Variant4", 4, theme => theme.VariantShapeStyles[3]);
		public static readonly DiagramItemStyleId Subtle1 = DiagramItemStyleId.Create("ConnectorId.Subtle1", DiagramControlStringId.Themes_SubtleEffect_Name, 1, theme => theme.ConnectorStyles[0]);
	}
	public static class DiagramShapeStyleId {
		public static readonly DiagramItemStyleId Variant1 = DiagramItemStyleId.Create("Variant1", 1, theme => theme.VariantShapeStyles[0]);
		public static readonly DiagramItemStyleId Variant2 = DiagramItemStyleId.Create("Variant2", 2, theme => theme.VariantShapeStyles[1]);
		public static readonly DiagramItemStyleId Variant3 = DiagramItemStyleId.Create("Variant3", 3, theme => theme.VariantShapeStyles[2]);
		public static readonly DiagramItemStyleId Variant4 = DiagramItemStyleId.Create("Variant4", 4, theme => theme.VariantShapeStyles[3]);
		public static readonly DiagramItemStyleId Subtle1 = DiagramItemStyleId.Create("ShapeId.Subtle1", DiagramControlStringId.Themes_SubtleEffect_Name, 1, theme => theme.ThemeShapeStyles[0]);
		public static readonly DiagramItemStyleId Subtle2 = DiagramItemStyleId.Create("ShapeId.Subtle2", DiagramControlStringId.Themes_SubtleEffect_Name, 2, theme => theme.ThemeShapeStyles[1]);
		public static readonly DiagramItemStyleId Subtle3 = DiagramItemStyleId.Create("ShapeId.Subtle3", DiagramControlStringId.Themes_SubtleEffect_Name, 3, theme => theme.ThemeShapeStyles[2]);
		public static readonly DiagramItemStyleId Subtle4 = DiagramItemStyleId.Create("ShapeId.Subtle4", DiagramControlStringId.Themes_SubtleEffect_Name, 4, theme => theme.ThemeShapeStyles[3]);
		public static readonly DiagramItemStyleId Subtle5 = DiagramItemStyleId.Create("ShapeId.Subtle5", DiagramControlStringId.Themes_SubtleEffect_Name, 5, theme => theme.ThemeShapeStyles[4]);
		public static readonly DiagramItemStyleId Subtle6 = DiagramItemStyleId.Create("ShapeId.Subtle6", DiagramControlStringId.Themes_SubtleEffect_Name, 6, theme => theme.ThemeShapeStyles[5]);
		public static readonly DiagramItemStyleId Subtle7 = DiagramItemStyleId.Create("ShapeId.Subtle7", DiagramControlStringId.Themes_SubtleEffect_Name, 7, theme => theme.ThemeShapeStyles[6]);
		public static readonly DiagramItemStyleId Refined1 = DiagramItemStyleId.Create("ShapeId.Refined1", DiagramControlStringId.Themes_RefinedEffect_Name, 1, theme => theme.ThemeShapeStyles[7]);
		public static readonly DiagramItemStyleId Refined2 = DiagramItemStyleId.Create("ShapeId.Refined2", DiagramControlStringId.Themes_RefinedEffect_Name, 2, theme => theme.ThemeShapeStyles[8]);
		public static readonly DiagramItemStyleId Refined3 = DiagramItemStyleId.Create("ShapeId.Refined3", DiagramControlStringId.Themes_RefinedEffect_Name, 3, theme => theme.ThemeShapeStyles[9]);
		public static readonly DiagramItemStyleId Refined4 = DiagramItemStyleId.Create("ShapeId.Refined4", DiagramControlStringId.Themes_RefinedEffect_Name, 4, theme => theme.ThemeShapeStyles[10]);
		public static readonly DiagramItemStyleId Refined5 = DiagramItemStyleId.Create("ShapeId.Refined5", DiagramControlStringId.Themes_RefinedEffect_Name, 5, theme => theme.ThemeShapeStyles[11]);
		public static readonly DiagramItemStyleId Refined6 = DiagramItemStyleId.Create("ShapeId.Refined6", DiagramControlStringId.Themes_RefinedEffect_Name, 6, theme => theme.ThemeShapeStyles[12]);
		public static readonly DiagramItemStyleId Refined7 = DiagramItemStyleId.Create("ShapeId.Refined7", DiagramControlStringId.Themes_RefinedEffect_Name, 7, theme => theme.ThemeShapeStyles[13]);
		public static readonly DiagramItemStyleId Balanced1 = DiagramItemStyleId.Create("ShapeId.Balanced1", DiagramControlStringId.Themes_BalancedEffect_Name, 1, theme => theme.ThemeShapeStyles[14]);
		public static readonly DiagramItemStyleId Balanced2 = DiagramItemStyleId.Create("ShapeId.Balanced2", DiagramControlStringId.Themes_BalancedEffect_Name, 2, theme => theme.ThemeShapeStyles[15]);
		public static readonly DiagramItemStyleId Balanced3 = DiagramItemStyleId.Create("ShapeId.Balanced3", DiagramControlStringId.Themes_BalancedEffect_Name, 3, theme => theme.ThemeShapeStyles[16]);
		public static readonly DiagramItemStyleId Balanced4 = DiagramItemStyleId.Create("ShapeId.Balanced4", DiagramControlStringId.Themes_BalancedEffect_Name, 4, theme => theme.ThemeShapeStyles[17]);
		public static readonly DiagramItemStyleId Balanced5 = DiagramItemStyleId.Create("ShapeId.Balanced5", DiagramControlStringId.Themes_BalancedEffect_Name, 5, theme => theme.ThemeShapeStyles[18]);
		public static readonly DiagramItemStyleId Balanced6 = DiagramItemStyleId.Create("ShapeId.Balanced6", DiagramControlStringId.Themes_BalancedEffect_Name, 6, theme => theme.ThemeShapeStyles[19]);
		public static readonly DiagramItemStyleId Balanced7 = DiagramItemStyleId.Create("ShapeId.Balanced7", DiagramControlStringId.Themes_BalancedEffect_Name, 7, theme => theme.ThemeShapeStyles[20]);
		public static readonly DiagramItemStyleId Moderate1 = DiagramItemStyleId.Create("ShapeId.Moderate1", DiagramControlStringId.Themes_ModerateEffect_Name, 1, theme => theme.ThemeShapeStyles[21]);
		public static readonly DiagramItemStyleId Moderate2 = DiagramItemStyleId.Create("ShapeId.Moderate2", DiagramControlStringId.Themes_ModerateEffect_Name, 2, theme => theme.ThemeShapeStyles[22]);
		public static readonly DiagramItemStyleId Moderate3 = DiagramItemStyleId.Create("ShapeId.Moderate3", DiagramControlStringId.Themes_ModerateEffect_Name, 3, theme => theme.ThemeShapeStyles[23]);
		public static readonly DiagramItemStyleId Moderate4 = DiagramItemStyleId.Create("ShapeId.Moderate4", DiagramControlStringId.Themes_ModerateEffect_Name, 4, theme => theme.ThemeShapeStyles[24]);
		public static readonly DiagramItemStyleId Moderate5 = DiagramItemStyleId.Create("ShapeId.Moderate5", DiagramControlStringId.Themes_ModerateEffect_Name, 5, theme => theme.ThemeShapeStyles[25]);
		public static readonly DiagramItemStyleId Moderate6 = DiagramItemStyleId.Create("ShapeId.Moderate6", DiagramControlStringId.Themes_ModerateEffect_Name, 6, theme => theme.ThemeShapeStyles[26]);
		public static readonly DiagramItemStyleId Moderate7 = DiagramItemStyleId.Create("ShapeId.Moderate7", DiagramControlStringId.Themes_ModerateEffect_Name, 7, theme => theme.ThemeShapeStyles[27]);
		public static readonly DiagramItemStyleId Focused1 = DiagramItemStyleId.Create("ShapeId.Focused1", DiagramControlStringId.Themes_FocusedEffect_Name, 1, theme => theme.ThemeShapeStyles[28]);
		public static readonly DiagramItemStyleId Focused2 = DiagramItemStyleId.Create("ShapeId.Focused2", DiagramControlStringId.Themes_FocusedEffect_Name, 2, theme => theme.ThemeShapeStyles[29]);
		public static readonly DiagramItemStyleId Focused3 = DiagramItemStyleId.Create("ShapeId.Focused3", DiagramControlStringId.Themes_FocusedEffect_Name, 3, theme => theme.ThemeShapeStyles[30]);
		public static readonly DiagramItemStyleId Focused4 = DiagramItemStyleId.Create("ShapeId.Focused4", DiagramControlStringId.Themes_FocusedEffect_Name, 4, theme => theme.ThemeShapeStyles[31]);
		public static readonly DiagramItemStyleId Focused5 = DiagramItemStyleId.Create("ShapeId.Focused5", DiagramControlStringId.Themes_FocusedEffect_Name, 5, theme => theme.ThemeShapeStyles[32]);
		public static readonly DiagramItemStyleId Focused6 = DiagramItemStyleId.Create("ShapeId.Focused6", DiagramControlStringId.Themes_FocusedEffect_Name, 6, theme => theme.ThemeShapeStyles[33]);
		public static readonly DiagramItemStyleId Focused7 = DiagramItemStyleId.Create("ShapeId.Focused7", DiagramControlStringId.Themes_FocusedEffect_Name, 7, theme => theme.ThemeShapeStyles[34]);
		public static readonly DiagramItemStyleId Intense1 = DiagramItemStyleId.Create("ShapeId.Intense1", DiagramControlStringId.Themes_IntenseEffect_Name, 1, theme => theme.ThemeShapeStyles[35]);
		public static readonly DiagramItemStyleId Intense2 = DiagramItemStyleId.Create("ShapeId.Intense2", DiagramControlStringId.Themes_IntenseEffect_Name, 2, theme => theme.ThemeShapeStyles[36]);
		public static readonly DiagramItemStyleId Intense3 = DiagramItemStyleId.Create("ShapeId.Intense3", DiagramControlStringId.Themes_IntenseEffect_Name, 3, theme => theme.ThemeShapeStyles[37]);
		public static readonly DiagramItemStyleId Intense4 = DiagramItemStyleId.Create("ShapeId.Intense4", DiagramControlStringId.Themes_IntenseEffect_Name, 4, theme => theme.ThemeShapeStyles[38]);
		public static readonly DiagramItemStyleId Intense5 = DiagramItemStyleId.Create("ShapeId.Intense5", DiagramControlStringId.Themes_IntenseEffect_Name, 5, theme => theme.ThemeShapeStyles[39]);
		public static readonly DiagramItemStyleId Intense6 = DiagramItemStyleId.Create("ShapeId.Intense6", DiagramControlStringId.Themes_IntenseEffect_Name, 6, theme => theme.ThemeShapeStyles[40]);
		public static readonly DiagramItemStyleId Intense7 = DiagramItemStyleId.Create("ShapeId.Intense7", DiagramControlStringId.Themes_IntenseEffect_Name, 7, theme => theme.ThemeShapeStyles[41]);
		public static readonly ReadOnlyCollection<DiagramItemStyleId> Styles;
		static readonly Dictionary<string, DiagramItemStyleId> stylesCore;
		static DiagramShapeStyleId() {
			stylesCore = new Dictionary<string, DiagramItemStyleId>();
			stylesCore.Add(Variant1.Id, Variant1);
			stylesCore.Add(Variant2.Id, Variant2);
			stylesCore.Add(Variant3.Id, Variant3);
			stylesCore.Add(Variant4.Id, Variant4);
			stylesCore.Add(Subtle1.Id, Subtle1);
			stylesCore.Add(Subtle2.Id, Subtle2);
			stylesCore.Add(Subtle3.Id, Subtle3);
			stylesCore.Add(Subtle4.Id, Subtle4);
			stylesCore.Add(Subtle5.Id, Subtle5);
			stylesCore.Add(Subtle6.Id, Subtle6);
			stylesCore.Add(Subtle7.Id, Subtle7);
			stylesCore.Add(Refined1.Id, Refined1);
			stylesCore.Add(Refined2.Id, Refined2);
			stylesCore.Add(Refined3.Id, Refined3);
			stylesCore.Add(Refined4.Id, Refined4);
			stylesCore.Add(Refined5.Id, Refined5);
			stylesCore.Add(Refined6.Id, Refined6);
			stylesCore.Add(Refined7.Id, Refined7);
			stylesCore.Add(Balanced1.Id, Balanced1);
			stylesCore.Add(Balanced2.Id, Balanced2);
			stylesCore.Add(Balanced3.Id, Balanced3);
			stylesCore.Add(Balanced4.Id, Balanced4);
			stylesCore.Add(Balanced5.Id, Balanced5);
			stylesCore.Add(Balanced6.Id, Balanced6);
			stylesCore.Add(Balanced7.Id, Balanced7);
			stylesCore.Add(Moderate1.Id, Moderate1);
			stylesCore.Add(Moderate2.Id, Moderate2);
			stylesCore.Add(Moderate3.Id, Moderate3);
			stylesCore.Add(Moderate4.Id, Moderate4);
			stylesCore.Add(Moderate5.Id, Moderate5);
			stylesCore.Add(Moderate6.Id, Moderate6);
			stylesCore.Add(Moderate7.Id, Moderate7);
			stylesCore.Add(Focused1.Id, Focused1);
			stylesCore.Add(Focused2.Id, Focused2);
			stylesCore.Add(Focused3.Id, Focused3);
			stylesCore.Add(Focused4.Id, Focused4);
			stylesCore.Add(Focused5.Id, Focused5);
			stylesCore.Add(Focused6.Id, Focused6);
			stylesCore.Add(Focused7.Id, Focused7);
			stylesCore.Add(Intense1.Id, Intense1);
			stylesCore.Add(Intense2.Id, Intense2);
			stylesCore.Add(Intense3.Id, Intense3);
			stylesCore.Add(Intense4.Id, Intense4);
			stylesCore.Add(Intense5.Id, Intense5);
			stylesCore.Add(Intense6.Id, Intense6);
			stylesCore.Add(Intense7.Id, Intense7);
			Styles = new ReadOnlyCollection<DiagramItemStyleId>(stylesCore.Values.ToList());
		}
		public static DiagramItemStyleId GetStyleIdByKey(string key) {
			DiagramItemStyleId styleId = null;
			if(stylesCore.TryGetValue(key, out styleId))
				return styleId;
			return null;
		}
	}
	public static class DiagramConnectorStyleId {
		public static readonly DiagramItemStyleId Subtle1 = DiagramItemStyleId.Create("ConnectorId.Subtle1", DiagramControlStringId.Themes_SubtleEffect_Name, 1, theme => theme.ConnectorStyles[0]);
		public static readonly DiagramItemStyleId Subtle2 = DiagramItemStyleId.Create("ConnectorId.Subtle2", DiagramControlStringId.Themes_SubtleEffect_Name, 2, theme => theme.ConnectorStyles[1]);
		public static readonly DiagramItemStyleId Subtle3 = DiagramItemStyleId.Create("ConnectorId.Subtle3", DiagramControlStringId.Themes_SubtleEffect_Name, 3, theme => theme.ConnectorStyles[2]);
		public static readonly DiagramItemStyleId Subtle4 = DiagramItemStyleId.Create("ConnectorId.Subtle4", DiagramControlStringId.Themes_SubtleEffect_Name, 4, theme => theme.ConnectorStyles[3]);
		public static readonly DiagramItemStyleId Subtle5 = DiagramItemStyleId.Create("ConnectorId.Subtle5", DiagramControlStringId.Themes_SubtleEffect_Name, 5, theme => theme.ConnectorStyles[4]);
		public static readonly DiagramItemStyleId Subtle6 = DiagramItemStyleId.Create("ConnectorId.Subtle6", DiagramControlStringId.Themes_SubtleEffect_Name, 6, theme => theme.ConnectorStyles[5]);
		public static readonly DiagramItemStyleId Subtle7 = DiagramItemStyleId.Create("ConnectorId.Subtle7", DiagramControlStringId.Themes_SubtleEffect_Name, 7, theme => theme.ConnectorStyles[6]);
		public static readonly DiagramItemStyleId Moderate1 = DiagramItemStyleId.Create("ConnectorId.Moderate1", DiagramControlStringId.Themes_ModerateEffect_Name, 1, theme => theme.ConnectorStyles[7]);
		public static readonly DiagramItemStyleId Moderate2 = DiagramItemStyleId.Create("ConnectorId.Moderate2", DiagramControlStringId.Themes_ModerateEffect_Name, 2, theme => theme.ConnectorStyles[8]);
		public static readonly DiagramItemStyleId Moderate3 = DiagramItemStyleId.Create("ConnectorId.Moderate3", DiagramControlStringId.Themes_ModerateEffect_Name, 3, theme => theme.ConnectorStyles[9]);
		public static readonly DiagramItemStyleId Moderate4 = DiagramItemStyleId.Create("ConnectorId.Moderate4", DiagramControlStringId.Themes_ModerateEffect_Name, 4, theme => theme.ConnectorStyles[10]);
		public static readonly DiagramItemStyleId Moderate5 = DiagramItemStyleId.Create("ConnectorId.Moderate5", DiagramControlStringId.Themes_ModerateEffect_Name, 5, theme => theme.ConnectorStyles[11]);
		public static readonly DiagramItemStyleId Moderate6 = DiagramItemStyleId.Create("ConnectorId.Moderate6", DiagramControlStringId.Themes_ModerateEffect_Name, 6, theme => theme.ConnectorStyles[12]);
		public static readonly DiagramItemStyleId Moderate7 = DiagramItemStyleId.Create("ConnectorId.Moderate7", DiagramControlStringId.Themes_ModerateEffect_Name, 7, theme => theme.ConnectorStyles[13]);
		public static readonly DiagramItemStyleId Intense1 = DiagramItemStyleId.Create("ConnectorId.Intense1", DiagramControlStringId.Themes_IntenseEffect_Name, 1, theme => theme.ConnectorStyles[14]);
		public static readonly DiagramItemStyleId Intense2 = DiagramItemStyleId.Create("ConnectorId.Intense2", DiagramControlStringId.Themes_IntenseEffect_Name, 2, theme => theme.ConnectorStyles[15]);
		public static readonly DiagramItemStyleId Intense3 = DiagramItemStyleId.Create("ConnectorId.Intense3", DiagramControlStringId.Themes_IntenseEffect_Name, 3, theme => theme.ConnectorStyles[16]);
		public static readonly DiagramItemStyleId Intense4 = DiagramItemStyleId.Create("ConnectorId.Intense4", DiagramControlStringId.Themes_IntenseEffect_Name, 4, theme => theme.ConnectorStyles[17]);
		public static readonly DiagramItemStyleId Intense5 = DiagramItemStyleId.Create("ConnectorId.Intense5", DiagramControlStringId.Themes_IntenseEffect_Name, 5, theme => theme.ConnectorStyles[18]);
		public static readonly DiagramItemStyleId Intense6 = DiagramItemStyleId.Create("ConnectorId.Intense6", DiagramControlStringId.Themes_IntenseEffect_Name, 6, theme => theme.ConnectorStyles[19]);
		public static readonly DiagramItemStyleId Intense7 = DiagramItemStyleId.Create("ConnectorId.Intense7", DiagramControlStringId.Themes_IntenseEffect_Name, 7, theme => theme.ConnectorStyles[20]);
		public static readonly ReadOnlyCollection<DiagramItemStyleId> Styles;
		static readonly Dictionary<string, DiagramItemStyleId> stylesCore;
		static DiagramConnectorStyleId() {
			stylesCore = new Dictionary<string, DiagramItemStyleId>();
			stylesCore.Add(Subtle1.Id, Subtle1);
			stylesCore.Add(Subtle2.Id, Subtle2);
			stylesCore.Add(Subtle3.Id, Subtle3);
			stylesCore.Add(Subtle4.Id, Subtle4);
			stylesCore.Add(Subtle5.Id, Subtle5);
			stylesCore.Add(Subtle6.Id, Subtle6);
			stylesCore.Add(Subtle7.Id, Subtle7);
			stylesCore.Add(Moderate1.Id, Moderate1);
			stylesCore.Add(Moderate2.Id, Moderate2);
			stylesCore.Add(Moderate3.Id, Moderate3);
			stylesCore.Add(Moderate4.Id, Moderate4);
			stylesCore.Add(Moderate5.Id, Moderate5);
			stylesCore.Add(Moderate6.Id, Moderate6);
			stylesCore.Add(Moderate7.Id, Moderate7);
			stylesCore.Add(Intense1.Id, Intense1);
			stylesCore.Add(Intense2.Id, Intense2);
			stylesCore.Add(Intense3.Id, Intense3);
			stylesCore.Add(Intense4.Id, Intense4);
			stylesCore.Add(Intense5.Id, Intense5);
			stylesCore.Add(Intense6.Id, Intense6);
			stylesCore.Add(Intense7.Id, Intense7);
			Styles = new ReadOnlyCollection<DiagramItemStyleId>(stylesCore.Values.ToList());
		}
		public static DiagramItemStyleId GetStyleIdByKey(string key) {
			DiagramItemStyleId styleId = null;
			if(stylesCore.TryGetValue(key, out styleId))
				return styleId;
			return null;
		}
	}
}

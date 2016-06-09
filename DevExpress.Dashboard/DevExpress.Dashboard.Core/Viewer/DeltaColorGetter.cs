#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
namespace DevExpress.DashboardCommon.Viewer {
	public class DeltaColorsGetter : IDeltaColors {
		public readonly static Color DefaultDeltaNeutralResultColor = DefaultDeltaColors.DefaultDeltaNeutralResultColor;
		public readonly static Color DefaultDeltaBadResultColor = DefaultDeltaColors.DefaultDeltaBadResultColor;
		public readonly static Color DefaultDeltaGoodResultColor = DefaultDeltaColors.DefaultDeltaGoodResultColor;
		public readonly static Color DefaultDeltaWarningResultColor = DefaultDeltaColors.DefaultDeltaWarningResultColor;
		public readonly static Color DefaultActualValueColor = DefaultDeltaColors.DefaultActualValueColor;
		public readonly static Color DefaultBarColor = DefaultDeltaColors.DefaultBarColor;
		internal readonly static Color InvalidColor = Color.FromArgb(-1);
		readonly Skin dashboardSkin;
		protected Skin DashboardSkin { get { return dashboardSkin; } }
		protected Skin CommonSkin { get { return dashboardSkin.CommonSkin; } }
		public virtual Color Neutral {
			get {
				return dashboardSkin == null ? DefaultDeltaNeutralResultColor : CommonSkin.Colors["DisabledText"];
			}
		}
		public virtual Color ActualValueColor {
			get {
				if(dashboardSkin == null)
					return DefaultActualValueColor;
				else {
					Color color = CommonSkin.Colors["WindowText"];
					return color == InvalidColor ? DefaultActualValueColor : color;
				}
			}
		}
		public Color BarColor {
			get {
				if(dashboardSkin == null)
					return DefaultBarColor;
				else {
					Color color = CommonSkin.Colors["Question"];
					return color == InvalidColor ? DefaultBarColor : color;
				}
			}
		}
		public virtual Color Bad {
			get {
				if(dashboardSkin == null)
					return DefaultDeltaBadResultColor;
				else {
					Color color = dashboardSkin.Colors.GetColor("DeltaBadResultColor", InvalidColor);
					return color == InvalidColor ? CommonSkin.Colors["Critical"] : color;
				}
			}
		}
		public virtual Color Warning {
			get {
				if(dashboardSkin == null)
					return DefaultDeltaWarningResultColor;
				else {
					Color color = dashboardSkin.Colors.GetColor("DeltaWarningResultColor", InvalidColor);
					return color == InvalidColor ? CommonSkin.Colors["Warning"] : color;
				}
			}
		}
		public virtual Color Good {
			get {
				if(dashboardSkin == null)
					return DefaultDeltaGoodResultColor;
				else {
					Color color = dashboardSkin.Colors.GetColor("DeltaGoodResultColor", InvalidColor);
					return color == InvalidColor ? CommonSkin.Colors["Information"] : color;
				}
			}
		}
		public DeltaColorsGetter(UserLookAndFeel lookAndFeel) {
			this.dashboardSkin = DashboardSkins.GetSkin(lookAndFeel);
		}
		public DeltaColorsGetter(Skin skin) {
			this.dashboardSkin = skin;
		}
		public virtual Color GetActualValueColor(AppearanceObject appearanceObject, ObjectState objectState) {
			return ActualValueColor;
		}
	}
	public class GridDeltaColorsGetter : DeltaColorsGetter {
		Skin gridSkin;
		public override Color GetActualValueColor(AppearanceObject appearanceObject, ObjectState objectState) {
			SkinElement skinElement = gridSkin[GridSkins.SkinGridRow];
			Color color;
			if (appearanceObject != null)
				color = skinElement.GetForeColor(appearanceObject, objectState);
			else
				color = skinElement.GetForeColor(objectState);
			if (!color.IsSystemColor)
				return color;
			Color systemColor = DashboardSkin.CommonSkin.Colors[color.Name];
			return systemColor == InvalidColor ? DefaultActualValueColor : systemColor;
		}
		public GridDeltaColorsGetter(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
			this.gridSkin = GridSkins.GetSkin(lookAndFeel);
		}
	}
}

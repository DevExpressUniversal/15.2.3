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

using DevExpress.Utils;
using DevExpress.XtraDiagram.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using PlatformTextAlignment = System.Windows.TextAlignment;
using PlatformVerticalAlignment = System.Windows.VerticalAlignment;
using DevExpress.XtraDiagram.Base;
namespace DevExpress.XtraDiagram.Extensions {
	public static class AppearanceObjectExtensions {
		public static TextPathOptions CreateTextPathOptions(this AppearanceObject appearance) {
			return new TextPathOptions(appearance.Font, appearance.GetAlignment(), appearance.GetLineAlignment());
		}
		public static StringAlignment GetAlignment(this AppearanceObject appearance) {
			switch(appearance.TextOptions.HAlignment) {
				case HorzAlignment.Default:
				case HorzAlignment.Center:
					return StringAlignment.Center;
				case HorzAlignment.Near:
					return StringAlignment.Near;
				case HorzAlignment.Far:
					return StringAlignment.Far;
				default:
					throw new ArgumentException("appearance");
			}
		}
		public static StringAlignment GetLineAlignment(this AppearanceObject appearance) {
			switch(appearance.TextOptions.VAlignment) {
				case VertAlignment.Default:
				case VertAlignment.Center:
					return StringAlignment.Center;
				case VertAlignment.Top:
					return StringAlignment.Near;
				case VertAlignment.Bottom:
					return StringAlignment.Far;
				default:
					throw new ArgumentException("appearance");
			}
		}
		public static AppearanceObject Update(this AppearanceObject appearance, string fontFamily = null, double? fontSize = null, FontStyle? fontStyle = null) {
			Font font = appearance.Font;
			appearance.Font = font.Update(fontFamily, fontSize, fontStyle);
			return appearance;
		}
		public static AppearanceObject Update(this AppearanceObject appearance, PlatformTextAlignment? horzAlignment = null, PlatformVerticalAlignment? vertAlignment = null) {
			if(horzAlignment != null)
				appearance.TextOptions.HAlignment = ToHorzAlignment(horzAlignment.Value);
			if(vertAlignment != null)
				appearance.TextOptions.VAlignment = ToVertAlignment(vertAlignment.Value);
			return appearance;
		}
		static HorzAlignment ToHorzAlignment(PlatformTextAlignment alignment) {
			switch(alignment) {
				case PlatformTextAlignment.Center:
				case PlatformTextAlignment.Justify:
					return HorzAlignment.Default;
				case PlatformTextAlignment.Left:
					return HorzAlignment.Near;
				case PlatformTextAlignment.Right:
					return HorzAlignment.Far;
				default:
					throw new ArgumentException("alignment");
			}
		}
		static VertAlignment ToVertAlignment(PlatformVerticalAlignment alignment) {
			switch(alignment) {
				case PlatformVerticalAlignment.Stretch:
				case PlatformVerticalAlignment.Center:
					return VertAlignment.Default;
				case PlatformVerticalAlignment.Top:
					return VertAlignment.Top;
				case PlatformVerticalAlignment.Bottom:
					return VertAlignment.Bottom;
				default:
					throw new ArgumentException("alignment");
			}
		}
		public static PlatformTextAlignment ToPlatformTextAlignment(this DiagramAppearanceObject appearance) {
			switch(appearance.TextOptions.HAlignment) {
				case HorzAlignment.Default:
				case HorzAlignment.Center:
					return PlatformTextAlignment.Center;
				case HorzAlignment.Near:
					return PlatformTextAlignment.Left;
				case HorzAlignment.Far:
					return PlatformTextAlignment.Right;
				default:
					throw new ArgumentException("alignment");
			}
		}
		public static PlatformVerticalAlignment ToPlatformVerticalAlignment(this DiagramAppearanceObject appearance) {
			switch(appearance.TextOptions.VAlignment) {
				case VertAlignment.Default:
				case VertAlignment.Center:
					return PlatformVerticalAlignment.Center;
				case VertAlignment.Top:
					return PlatformVerticalAlignment.Top;
				case VertAlignment.Bottom:
					return PlatformVerticalAlignment.Bottom;
				default:
					throw new ArgumentException("alignment");
			}
		}
	}
}

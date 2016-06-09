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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit {
	#region RichEditRangePermissionVisibility
	[ComVisible(true)]
	public enum RichEditRangePermissionVisibility {
		Auto,
		Visible,
		Hidden
	}
	#endregion
	#region RangePermissionOptions
	[ComVisible(true)]
	public class RangePermissionOptions : RichEditNotificationOptions {
		#region Fields
		const RichEditRangePermissionVisibility defaultVisibility = RichEditRangePermissionVisibility.Auto;
		static readonly Color defaultColor = DXColor.FromArgb(127, 127, 127);
		static readonly Color defaultHighlightBracketsColor = DXColor.FromArgb(0xA4, 0xA0, 0);
		static readonly Color defaultBracketsColor = DXColor.FromArgb(0x7F, 0x7F, 0x7F);
		static readonly Color defaultHighlightColor = DXColor.FromArgb(0xFF, 0xFE, 0xD5);
		static readonly Color[] defaultColors = new Color[] { DXColor.FromArgb(0xD5, 0xD5, 0xFF),
																DXColor.FromArgb(0xFF, 0xD5, 0xFF),
																DXColor.FromArgb(0xFF, 0xE6, 0xD5),
																DXColor.FromArgb(0xD5, 0xFF, 0xFE),
																DXColor.FromArgb(0xFF, 0xFE, 0xD5),
																DXColor.FromArgb(0xE9, 0xE9, 0xE9),
																DXColor.FromArgb(0xFF, 0xD5, 0xD5),
																DXColor.FromArgb(0xD5, 0xFF, 0xD5)};
		static int colorIndex;
		internal static Color GetColor() {
			if(colorIndex >= defaultColors.Length)
				colorIndex = 0;
			Color result = defaultColors[colorIndex];
			colorIndex++;
			return result;
		}
		RichEditRangePermissionVisibility visibility;
		Color color;
		Color bracketsColor;
		Color highlightColor;
		Color highlightBracketsColor;
		#endregion
		#region Properties
		#region Visibility
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RangePermissionOptionsVisibility"),
#endif
NotifyParentProperty(true), DefaultValue(defaultVisibility), XtraSerializableProperty()]
		public RichEditRangePermissionVisibility Visibility {
			get { return visibility; }
			set {
				if (Visibility == value)
					return;
				RichEditRangePermissionVisibility oldValue = Visibility;
				visibility = value;
				OnChanged("RangePermissionVisibility", oldValue, value);
			}
		}
		#endregion
		#region Color
		[
NotifyParentProperty(true), XtraSerializableProperty()]
		private Color Color {
			get { return color; }
			set {
				if (Color == value)
					return;
				Color oldValue = Color;
				color = value;
				OnChanged("RangePermissionColor", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeColor() {
			return Color != defaultColor;
		}
		protected internal virtual void ResetColor() {
			Color = defaultColor;
		}
		#endregion
		#region BracketsColor
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RangePermissionOptionsBracketsColor"),
#endif
NotifyParentProperty(true), XtraSerializableProperty()]
		public Color BracketsColor {
			get { return bracketsColor; }
			set {
				if(BracketsColor == value)
					return;
				Color oldValue = BracketsColor;
				bracketsColor = value;
				OnChanged("RangePermissionBracketsColor", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeBracketsColor() {
			return BracketsColor != defaultBracketsColor;
		}
		protected virtual void ResetBracketsColor() { 
			BracketsColor = defaultBracketsColor;
		}
		#endregion
		#region HighlightBracketsColor
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RangePermissionOptionsHighlightBracketsColor"),
#endif
NotifyParentProperty(true), XtraSerializableProperty()]
		public Color HighlightBracketsColor {
			get { return highlightBracketsColor; }
			set {
				if(HighlightBracketsColor == value)
					return;
				Color oldValue = HighlightBracketsColor;
				highlightBracketsColor = value;
				OnChanged("RangePermissionHighlightBracketsColor", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeHighlightBracketsColor() {
			return HighlightBracketsColor != defaultHighlightBracketsColor;
		}
		protected virtual void ResetHighlightBracketsColor() {
			HighlightBracketsColor = defaultHighlightBracketsColor;
		}
		#endregion
		#region HighlightColor
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("RangePermissionOptionsHighlightColor"),
#endif
NotifyParentProperty(true), XtraSerializableProperty()]
		public Color HighlightColor {
			get { return highlightColor; }
			set {
				if(HighlightColor == value)
					return;
				Color oldValue = HighlightColor;
				highlightColor = value;
				OnChanged("RangePermissionHighlightColor", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeHighlightColor() {
			return HighlightColor != defaultHighlightColor;
		}
		protected virtual void ResetHighlightColor() {
			HighlightColor = defaultHighlightColor;
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			Visibility = defaultVisibility;
			Color = defaultColor;
			HighlightColor = defaultHighlightColor;
			BracketsColor = defaultBracketsColor;
			HighlightBracketsColor = defaultHighlightBracketsColor;
		}
	}
	#endregion
}

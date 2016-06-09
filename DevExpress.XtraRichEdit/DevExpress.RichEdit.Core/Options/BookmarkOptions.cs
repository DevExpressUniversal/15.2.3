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
	#region RichEditBookmarkVisibility
	[ComVisible(true)]
	public enum RichEditBookmarkVisibility {
		Auto,
		Visible,
		Hidden
	}
	#endregion
	#region BookmarkOptions
	[ComVisible(true)]
	public class BookmarkOptions : RichEditNotificationOptions {
		#region Fields
		const RichEditBookmarkVisibility defaultVisibility = RichEditBookmarkVisibility.Auto;
		const bool defaultAllowNameResolution = false;
		internal static readonly Color defaultColor = DXColor.FromArgb(127, 127, 127);
		RichEditBookmarkVisibility visibility;
		Color color;
		bool allowNameResolution;
		#endregion
		#region Properties
		#region Visibility
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("BookmarkOptionsVisibility"),
#endif
NotifyParentProperty(true), DefaultValue(defaultVisibility), XtraSerializableProperty()]
		public virtual RichEditBookmarkVisibility Visibility {
			get { return visibility; }
			set {
				if (Visibility == value)
					return;
				RichEditBookmarkVisibility oldValue = Visibility;
				visibility = value;
				OnChanged("BookmarkVisibility", oldValue, value);
			}
		}
		#endregion
		#region Color
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("BookmarkOptionsColor"),
#endif
NotifyParentProperty(true), XtraSerializableProperty()]
		public virtual Color Color {
			get { return color; }
			set {
				if (Color == value)
					return;
				Color oldValue = Color;
				color = value;
				OnChanged("BookmarkColor", oldValue, value);
			}
		}
		protected internal virtual bool ShouldSerializeColor() {
			return Color != defaultColor;
		}
		protected internal virtual void ResetColor() {
			Color = defaultColor;
		}
		#endregion
		#region AllowNameResolution
		[
#if !SL
	DevExpressRichEditCoreLocalizedDescription("BookmarkOptionsColor"),
#endif
		NotifyParentProperty(true), XtraSerializableProperty()]
		public virtual bool AllowNameResolution {
			get { return allowNameResolution; }
			set {
				if (allowNameResolution == value)
					return;
				allowNameResolution = value;
				OnChanged("AllowNameResolution", !value, value);
			}
		}
		#endregion
		#endregion
		protected internal override void ResetCore() {
			Visibility = defaultVisibility;
			Color = defaultColor;
			AllowNameResolution = defaultAllowNameResolution;
		}
	}
	#endregion
}

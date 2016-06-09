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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDiagram.ViewInfo;
namespace DevExpress.XtraDiagram.Paint {
	#region PaintArgs
	public class DiagramBackgroundObjectInfoArgs : ObjectInfoArgs {
		DiagramAppearanceObject paintAppearance;
		UserLookAndFeel lookAndFeel;
		DiagramPaintCache paintCache;
		public DiagramBackgroundObjectInfoArgs() {
			this.paintAppearance = null;
			this.lookAndFeel = null;
			this.paintCache = null;
		}
		public virtual void Initialize(DiagramControlViewInfo viewInfo) {
			this.Bounds = viewInfo.ClientRect;
			this.lookAndFeel = viewInfo.LookAndFeel;
			this.paintCache = viewInfo.PaintCache;
		}
		public virtual void Clear() {
			this.paintAppearance = null;
			this.lookAndFeel = null;
			this.paintCache = null;
		}
		public DiagramAppearanceObject PaintAppearance {
			get { return paintAppearance; }
			set { paintAppearance = value; }
		}
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public DiagramPaintCache PaintCache { get { return paintCache; } }
	}
	#endregion
	#region Painter
	public class DiagramBackgroundPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DiagramBackgroundObjectInfoArgs args = (DiagramBackgroundObjectInfoArgs)e;
			base.DrawObject(e);
			DrawBackground(args);
		}
		protected virtual void DrawBackground(DiagramBackgroundObjectInfoArgs args) {
			SkinElementInfo skinElementInfo = GetBackgroundElementInfo(args);
			if(skinElementInfo.Element.Image != null) {
				args.Graphics.DrawImageUnscaled(args.PaintCache.GetBackgroundImage(args.Bounds, skinElementInfo), args.Bounds.Location);
			}
			else {
				ObjectPainter.DrawObject(args.Cache, SkinElementPainter.Default, GetBackgroundElementInfo(args));
			}
		}
		protected virtual SkinElementInfo GetBackgroundElementInfo(DiagramBackgroundObjectInfoArgs args) {
			SkinElement skinElement = PrintingSkins.GetSkin(args.LookAndFeel)[PrintingSkins.SkinBackgroundPreview];
			return new SkinElementInfo(skinElement, args.Bounds);
		}
	}
	#endregion
}

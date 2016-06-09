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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Drawing {
	#region SpreadsheetViewBackgroundPainter (abstract class)
	public abstract class SpreadsheetViewBackgroundPainter : IDisposable {
		readonly SpreadsheetView view;
		readonly SpreadsheetControl control;
		protected SpreadsheetViewBackgroundPainter(SpreadsheetView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
			this.control = (SpreadsheetControl)view.Control;
		}
		public SpreadsheetView View { get { return view; } }
		protected internal SpreadsheetControl Control { get { return control; } }
		public virtual UserLookAndFeel LookAndFeel { get { return Control.LookAndFeel; } }
		public virtual void Draw(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(SystemColors.ControlDarkDark, bounds);
		}
		protected internal virtual Color GetActualPageBackColor() {
			if (!Control.Enabled && !SpreadsheetViewAccessor.ShouldSerializeBackColor(View))
				return SystemColors.Control;
			else
				return View.ActualBackColor;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region SpreadsheetViewBackgroundPainterFactory (abstract class)
	public abstract class SpreadsheetViewBackgroundPainterFactory : ISpreadsheetViewVisitor {
		SpreadsheetViewBackgroundPainter painter;
		protected SpreadsheetViewBackgroundPainter Painter { get { return painter; } set { painter = value; } }
		public SpreadsheetViewBackgroundPainter CreatePainter(SpreadsheetView view) {
			Painter = null;
			view.Visit(this);
			return Painter;
		}
		public abstract void Visit(NormalView view);
	}
	#endregion
	#region SpreadsheetViewBackgroundPainterFlatFactory
	public class SpreadsheetViewBackgroundPainterFlatFactory : SpreadsheetViewBackgroundPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewFlatBackgroundPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewBackgroundPainterUltraFlatFactory
	public class SpreadsheetViewBackgroundPainterUltraFlatFactory : SpreadsheetViewBackgroundPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewUltraFlatBackgroundPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewBackgroundPainterStyle3DFactory
	public class SpreadsheetViewBackgroundPainterStyle3DFactory : SpreadsheetViewBackgroundPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewStyle3DBackgroundPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewBackgroundPainterOffice2003Factory
	public class SpreadsheetViewBackgroundPainterOffice2003Factory : SpreadsheetViewBackgroundPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewOffice2003BackgroundPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewBackgroundPainterWindowsXPFactory
	public class SpreadsheetViewBackgroundPainterWindowsXPFactory : SpreadsheetViewBackgroundPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewWindowsXPBackgroundPainter(view);
		}
	}
	#endregion
	#region SpreadsheetViewBackgroundPainterSkinFactory
	public class SpreadsheetViewBackgroundPainterSkinFactory : SpreadsheetViewBackgroundPainterFactory {
		public override void Visit(NormalView view) {
			Painter = new NormalViewSkinBackgroundPainter(view);
		}
	}
	#endregion
	#region NormalViewBackgroundPainter (abstract class)
	public abstract class NormalViewBackgroundPainter : SpreadsheetViewBackgroundPainter {
		protected NormalViewBackgroundPainter(NormalView view)
			: base(view) {
		}
		public override void Draw(GraphicsCache cache, Rectangle bounds) {
			cache.FillRectangle(GetActualPageBackColor(), bounds);
		}
		protected internal override Color GetActualPageBackColor() {
			if (!Control.Enabled && !SpreadsheetViewAccessor.ShouldSerializeBackColor(View))
				return SystemColors.Control;
			else
				return View.ActualBackColor;
		}
	}
	#endregion
	#region NormalViewFlatBackgroundPainter
	public class NormalViewFlatBackgroundPainter : NormalViewBackgroundPainter {
		public NormalViewFlatBackgroundPainter(NormalView view)
			: base(view) {
		}
	}
	#endregion
	#region NormalViewUltraFlatBackgroundPainter
	public class NormalViewUltraFlatBackgroundPainter : NormalViewBackgroundPainter {
		public NormalViewUltraFlatBackgroundPainter(NormalView view)
			: base(view) {
		}
	}
	#endregion
	#region NormalViewStyle3DBackgroundPainter
	public class NormalViewStyle3DBackgroundPainter : NormalViewBackgroundPainter {
		public NormalViewStyle3DBackgroundPainter(NormalView view)
			: base(view) {
		}
	}
	#endregion
	#region NormalViewOffice2003BackgroundPainter
	public class NormalViewOffice2003BackgroundPainter : NormalViewBackgroundPainter {
		public NormalViewOffice2003BackgroundPainter(NormalView view)
			: base(view) {
		}
	}
	#endregion
	#region NormalViewWindowsXPBackgroundPainter
	public class NormalViewWindowsXPBackgroundPainter : NormalViewBackgroundPainter {
		public NormalViewWindowsXPBackgroundPainter(NormalView view)
			: base(view) {
		}
	}
	#endregion
	#region NormalViewSkinBackgroundPainter
	public class NormalViewSkinBackgroundPainter : NormalViewBackgroundPainter {
		public NormalViewSkinBackgroundPainter(NormalView view)
			: base(view) {
		}
		protected internal override Color GetActualPageBackColor() {
			if (!Control.Enabled && !SpreadsheetViewAccessor.ShouldSerializeBackColor(View))
				return CommonSkins.GetSkin(LookAndFeel).Colors[CommonColors.DisabledControl];
			Color color = View.ActualBackColor;
			if (color.IsSystemColor)
				return GridSkins.GetSkin(LookAndFeel)[GridSkins.SkinGridLine].Color.Owner.GetSystemColor(color);
			else
				return color;
		}
	}
	#endregion
}

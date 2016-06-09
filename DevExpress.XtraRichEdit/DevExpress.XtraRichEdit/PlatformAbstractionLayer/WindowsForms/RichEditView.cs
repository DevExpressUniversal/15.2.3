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
using System.ComponentModel;
using DevExpress.Utils.Drawing;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.LookAndFeel;
using DevExpress.XtraRichEdit.Painters;
using DevExpress.XtraRichEdit.Drawing;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit {
	public class WinFormsRichEditViewRepository : RichEditViewRepository {
		public WinFormsRichEditViewRepository(RichEditControl control)
			: base(control) {
		}
		protected internal override PrintLayoutView CreatePrintLayoutView() {
			return new PrintLayoutView(Control);
		}
		protected internal override DraftView CreateDraftView() {
			return new DraftView(Control);
		}
		protected internal override SimpleView CreateSimpleView() {
			return new SimpleView(Control);
		}
		protected internal override ReadingLayoutView CreateReadingLayoutView() {
			return new ReadingLayoutView(Control);
		}
	}
}
namespace DevExpress.XtraRichEdit.Drawing {
	#region RichEditViewPainterFactory (abstract class)
	public abstract class RichEditViewPainterFactory : IRichEditViewVisitor {
		RichEditViewPainter painter;
		protected RichEditViewPainter Painter { get { return painter; } set { painter = value; } }
		public RichEditViewPainter CreatePainter(RichEditView view) {
			Painter = null;
			view.Visit(this);
			return Painter;
		}
		public abstract void Visit(SimpleView view);
		public abstract void Visit(DraftView view);
		public abstract void Visit(PrintLayoutView view);
		public abstract void Visit(ReadingLayoutView view);
	}
	#endregion
	#region RichEditViewPainterFlatFactory
	public class RichEditViewPainterFlatFactory : RichEditViewPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewFlatPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewFlatPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewFlatPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewFlatPainter(view);
		}
	}
	#endregion
	#region RichEditViewPainterUltraFlatFactory
	public class RichEditViewPainterUltraFlatFactory : RichEditViewPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewUltraFlatPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewUltraFlatPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewUltraFlatPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewUltraFlatPainter(view);
		}
	}
	#endregion
	#region RichEditViewPainterStyle3DFactory
	public class RichEditViewPainterStyle3DFactory : RichEditViewPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewStyle3DPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewStyle3DPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewStyle3DPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewStyle3DPainter(view);
		}
	}
	#endregion
	#region RichEditViewPainterOffice2003Factory
	public class RichEditViewPainterOffice2003Factory : RichEditViewPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewOffice2003Painter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewOffice2003Painter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewOffice2003Painter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewOffice2003Painter(view);
		}
	}
	#endregion
	#region RichEditViewPainterWindowsXPFactory
	public class RichEditViewPainterWindowsXPFactory : RichEditViewPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewWindowsXPPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewWindowsXPPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewWindowsXPPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewWindowsXPPainter(view);
		}
	}
	#endregion
	#region RichEditViewPainterSkinFactory
	public class RichEditViewPainterSkinFactory : RichEditViewPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewSkinPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewSkinPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewSkinPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewSkinPainter(view);
		}
	}
	#endregion
	#region RichEditViewBackgroundPainterFactory (abstract class)
	public abstract class RichEditViewBackgroundPainterFactory : IRichEditViewVisitor {
		RichEditViewBackgroundPainter painter;
		protected RichEditViewBackgroundPainter Painter { get { return painter; } set { painter = value; } }
		public RichEditViewBackgroundPainter CreatePainter(RichEditView view) {
			Painter = null;
			view.Visit(this);
			return Painter;
		}
		public abstract void Visit(SimpleView view);
		public abstract void Visit(DraftView view);
		public abstract void Visit(PrintLayoutView view);
		public abstract void Visit(ReadingLayoutView view);
	}
	#endregion
	#region RichEditViewBackgroundPainterFlatFactory
	public class RichEditViewBackgroundPainterFlatFactory : RichEditViewBackgroundPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewFlatBackgroundPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewFlatBackgroundPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewFlatBackgroundPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewFlatBackgroundPainter(view);
		}
	}
	#endregion
	#region RichEditViewBackgroundPainterUltraFlatFactory
	public class RichEditViewBackgroundPainterUltraFlatFactory : RichEditViewBackgroundPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewUltraFlatBackgroundPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewUltraFlatBackgroundPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewUltraFlatBackgroundPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewUltraFlatBackgroundPainter(view);
		}
	}
	#endregion
	#region RichEditViewBackgroundPainterStyle3DFactory
	public class RichEditViewBackgroundPainterStyle3DFactory : RichEditViewBackgroundPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewStyle3DBackgroundPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewStyle3DBackgroundPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewStyle3DBackgroundPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewStyle3DBackgroundPainter(view);
		}
	}
	#endregion
	#region RichEditViewBackgroundPainterOffice2003Factory
	public class RichEditViewBackgroundPainterOffice2003Factory : RichEditViewBackgroundPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewOffice2003BackgroundPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewOffice2003BackgroundPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewOffice2003BackgroundPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewOffice2003BackgroundPainter(view);
		}
	}
	#endregion
	#region RichEditViewBackgroundPainterWindowsXPFactory
	public class RichEditViewBackgroundPainterWindowsXPFactory : RichEditViewBackgroundPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewWindowsXPBackgroundPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewWindowsXPBackgroundPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewWindowsXPBackgroundPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewWindowsXPBackgroundPainter(view);
		}
	}
	#endregion
	#region RichEditViewBackgroundPainterSkinFactory
	public class RichEditViewBackgroundPainterSkinFactory : RichEditViewBackgroundPainterFactory {
		public override void Visit(SimpleView view) {
			Painter = new SimpleViewSkinBackgroundPainter(view);
		}
		public override void Visit(DraftView view) {
			Painter = new DraftViewSkinBackgroundPainter(view);
		}
		public override void Visit(PrintLayoutView view) {
			Painter = new PrintLayoutViewSkinBackgroundPainter(view);
		}
		public override void Visit(ReadingLayoutView view) {
			Painter = new ReadingLayoutViewSkinBackgroundPainter(view);
		}
	}
	#endregion
}

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
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout.Handlers;
namespace DevExpress.XtraLayout.Customization.Behaviours {
	public class LayoutBaseBehaviour :IDisposable {
		protected internal List<LayoutGlyph> glyphs;
		protected LayoutAdornerWindowHandler owner;
		public LayoutBaseBehaviour(LayoutAdornerWindowHandler handler) {
			owner = handler;
			glyphs = new List<LayoutGlyph>();
		}
		public LayoutGlyph GetGlyphAtPoint(Point p) {
			foreach(LayoutGlyph g in glyphs) {
				if(g.Bounds.Contains(p)) {
					return g;
				}
			}
			return null;
		}
		public virtual bool ProcessEvent(EventType eventType, MouseEventArgs e) {
			return false;
		}
		public virtual bool ProcessEvent(EventType eventType, object sender, MouseEventArgs e, KeyEventArgs key) {
			return ProcessEvent(eventType, e);
		}
		public virtual void Invalidate() {
			glyphs.Clear();
		}
		public virtual void Paint(GraphicsCache cache) {
			foreach(LayoutGlyph glyph in glyphs) {
				glyph.Paint(cache);
			}
		}
		public virtual void Dispose() {
			glyphs.Clear();
		}
	}
	public class LayoutGlyph {
		protected LayoutBaseBehaviour owner;
		protected ILayoutControl layoutControl;
		public Rectangle Bounds { get; set; }
		public LayoutGlyph(ILayoutControl layoutControl) {
			this.layoutControl = layoutControl;
		}
		public virtual void Paint(GraphicsCache cache) {
		}
	}
}

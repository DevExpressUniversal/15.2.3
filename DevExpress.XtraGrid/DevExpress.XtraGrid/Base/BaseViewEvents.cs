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
using System.Windows.Forms;
using System.IO;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
namespace DevExpress.XtraGrid.Views.Base {
	public class CustomDrawEventArgs : EventArgs {
		GraphicsCache cache;
		AppearanceObject appearance;
		Rectangle bounds;
		bool handled;
		MethodInvoker defaultDraw;
		public CustomDrawEventArgs(GraphicsCache cache, Rectangle bounds, AppearanceObject appearance) {
			this.cache = cache;
			this.appearance = appearance;
			this.handled = false;
			this.bounds = bounds;
		}
		internal void SetDefaultDraw(MethodInvoker defaultDraw) {
			this.defaultDraw = defaultDraw;
		}
		public virtual Rectangle Bounds {
			get { return bounds; }
		}
		public bool Handled {
			get { return handled; }
			set {
				handled = value;
			}
		}
		public void DefaultDraw() {
			if(defaultDraw != null && !Handled) {
				Handled = true;
				defaultDraw();
			}
		}
		public virtual AppearanceObject Appearance { 
			get { return appearance; }
		}
		public GraphicsCache Cache { get { return cache; } }
		public Graphics Graphics { get { return Cache.Graphics; } }
		internal void SetupCache(GraphicsCache cache) {
			this.cache = cache;
		}
		internal void SetupBase(AppearanceObject appearance, Rectangle bounds) {
			this.appearance = appearance;
			this.bounds = bounds;
		}
	}
	public delegate void CustomDrawEventHandler(object sender, CustomDrawEventArgs e);
	public delegate void PrintInitializeEventHandler(object sender, PrintInitializeEventArgs e);
	public class PrintInitializeEventArgs : EventArgs {
		IPrintingSystem printingSystem;
		PrintableComponentLinkBase link;
		public PrintInitializeEventArgs(IPrintingSystem printingSystem,  PrintableComponentLinkBase link) {
			this.printingSystem = printingSystem;
			this.link = link;
		}
		public IPrintingSystem PrintingSystem { get { return printingSystem; } }
		public PrintableComponentLinkBase Link { get { return link; } }
	}
}

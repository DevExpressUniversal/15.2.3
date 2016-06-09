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
using System.Text;
using System.Windows.Forms.Design;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel.Helpers;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
namespace DevExpress.XtraReports.Design {
	class ComponentTrayEx : ComponentTray, ISupportLookAndFeel {
		ObjectInfoArgs viewInfo = new ObjectInfoArgs();
		ControlUserLookAndFeel userLookAndFeel;
		bool isDisposed;
		ComponentTrayPainter painter;
		ComponentTrayPainter Painter {
			get {
				if(painter == null)
					painter = ReportPaintStyles.GetPaintStyle(LookAndFeel).CreateComponentTrayPainter(LookAndFeel);
				return painter;
			}
		}
		#region ISupportLookAndFeel
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return false; }
		}
		public UserLookAndFeel LookAndFeel {
			get { return userLookAndFeel; }
		}
		#endregion //ISupportLookAndFeel
		public ComponentTrayEx(IDesigner designer, IServiceProvider sp)
			: base(designer, sp) {
			userLookAndFeel = new ControlUserLookAndFeel(this);
			userLookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelStyleChanged);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(userLookAndFeel != null) {
					userLookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelStyleChanged);
					userLookAndFeel.Dispose();
					userLookAndFeel = null;
				}
			}
			if(!isDisposed) {
				isDisposed = true;
				base.Dispose(disposing);
			}
		}
		protected override void OnPaintBackground(PaintEventArgs e) {
			viewInfo.Bounds = DisplayRectangle;
			using(GraphicsCache cache = new GraphicsCache(e)) {
				ObjectPainter.DrawObject(cache, Painter, viewInfo);
			}
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			MethodInfo mi = typeof(Control).GetMethod("SetStyle", BindingFlags.NonPublic | BindingFlags.Instance);
			mi.Invoke(e.Control, new Object[] { ControlStyles.SupportsTransparentBackColor, true });
			SetControlView(e.Control);
			base.OnControlAdded(e);
		}
		void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			Invalidate();
			painter = null;
			foreach(Control control in Controls)
				SetControlView(control);
		}
		void SetControlView(Control control) {
			control.ForeColor = Painter.GetForeColor();
			control.BackColor = Color.Transparent;
		}
	}
}

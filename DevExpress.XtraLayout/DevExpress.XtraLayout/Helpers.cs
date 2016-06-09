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
using System.IO;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Timers;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using Accessibility;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DXUtils = DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Printing;
using DevExpress.XtraLayout.Dragging;
using DevExpress.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Customization;
using Padding = DevExpress.XtraLayout.Utils.Padding;
using DevExpress.XtraLayout.Accessibility;
using DevExpress.Skins;
namespace DevExpress.XtraLayout {
	public class UniversalUpdaterBase : EventArgs, IDisposable {
		protected bool cancel = false;
		protected object sender;
		public UniversalUpdaterBase() {
			Start();
		}
		protected virtual void Start() { }
		protected virtual void End() { }
		public void Dispose() {
			End();
		}
	}
	public class AllowSetIsModifiedHelper : UniversalUpdaterBase {
		ILayoutControl owner;
		bool originalAllowSetIsModified;
		public AllowSetIsModifiedHelper(ILayoutControl ilc) {
			owner = ilc;
			originalAllowSetIsModified = owner.AllowSetIsModified;
		}
		protected override void End() {
			owner.AllowSetIsModified = originalAllowSetIsModified;
			owner = null;
			base.End();
		}
	}
	public static class RDPHelper {
		public static bool IsRemoteSession {
			get { return SystemInformation.TerminalServerSession; }
		}
	}
	public static class FlowHelper {
		static SolidBrush backgroundBrush = new SolidBrush(Color.FromArgb(41, 91, 187));
		static DPIHelper DPIHelper = new DPIHelper();
		static SizeF backgroundSize = new SizeF(14* DPIHelper.DPIScale, 12 *  DPIHelper.DPIScale);
		static Brush textBrush = Brushes.White;
		private static bool CanShowNumber(LayoutGroup group, int i) {
			if(!(group.Owner is LayoutControl)) return false;
			LayoutControl custedControl = group.Owner as LayoutControl;
			if(custedControl.Parent == null) return false;
			Point location = custedControl.PointToScreen(group[i].ViewInfo.BoundsRelativeToControl.Location);
			Rectangle screenRect = new Rectangle(custedControl.Parent.PointToScreen(custedControl.Location), custedControl.Size);
			if(custedControl.CustomizationFormBounds.IntersectsWith(new Rectangle(location,backgroundSize.ToSize()))) return true;
			return !screenRect.Contains(location);
		}
		public static void DrawItems(LayoutGroup group, GraphicsCache graphicsCache) {
			for(int i = 0; i < group.Items.Count; i++) {	
				if(CanShowNumber(group, i) || !group[i].Visible) continue;
				RectangleF drawRect = new RectangleF(group[i].ViewInfo.BoundsRelativeToControl.Location, backgroundSize);
				graphicsCache.FillRectangle(backgroundBrush, drawRect);
				int offsetX = i < 10 ? -3 : -1;
				PointF numberLocation = new PointF(group[i].ViewInfo.BoundsRelativeToControl.Location.X - offsetX, group[i].ViewInfo.BoundsRelativeToControl.Location.Y + 1);
				Font fontToDraw = new Font(group[i].AppearanceItemCaption.Font.FontFamily, 6.25f,FontStyle.Bold);
				graphicsCache.Graphics.DrawString(i.ToString(), fontToDraw, textBrush, numberLocation);
			}
		}
	}
}

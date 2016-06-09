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
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.Utils;
using DevExpress.XtraBars.InternalItems;
namespace DevExpress.XtraBars.ViewInfo {
	public class DockControlWindowsXPViewInfo : DockControlViewInfo {
		public DockControlWindowsXPViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl barControl) : base(manager, parameters, barControl) {
		}
		public override bool IsDrawForeground { get { return true; } }
	}
	public class DockedBarControlWindowsXPViewInfo : DockedBarControlViewInfo {
		public DockedBarControlWindowsXPViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		public override void UpdateControlRegion(Control control) {
			control.Region = null;
		}
		public override bool LinksUseRealBounds { get { return true; } }
	}
	public class FloatingBarControlWindowsXPViewInfo : FloatingBarControlViewInfo {
		public FloatingBarControlWindowsXPViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		public override bool LinksUseRealBounds { get { return true; } }
	}
	public class QuickCustomizationBarControlWindowsXPViewInfo : QuickCustomizationBarControlViewInfo {
		public QuickCustomizationBarControlWindowsXPViewInfo(BarManager manager, BarDrawParameters parameters, CustomControl bar) : base(manager, parameters, bar) {
		}
		public new virtual QuickCustomizationBarControl BarControl { get { return base.BarControl as QuickCustomizationBarControl; } }
	}
	public class BarMdiButtonLinkWindowsXPViewInfo : BarMdiButtonLinkViewInfo {
		public BarMdiButtonLinkWindowsXPViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		protected override BarLinkState CalcLinkState() {
			return BarLinkState.Normal;
		}
		public override Image GetLinkImage(BarLinkState state) {
			if(!Link.IsLargeImageExist || IsLinkInMenu) return base.GetLinkImage(state);
			state = base.CalcLinkState();
			if(DrawDisabled) return Link.Item.LargeGlyphDisabled;
			if(state == BarLinkState.Pressed) return Link.Item.LargeGlyphPressed;
			if(state == BarLinkState.Highlighted) return Link.Item.LargeGlyphHot;
			return Link.Item.LargeGlyph;
		}
		public override bool UpdateLinkState() {
			BarLinkState newState = base.CalcLinkState();
			if(newState == LinkState) return false;
			BarControlViewInfo vi = BarControlInfo as BarControlViewInfo;
			if(vi != null) 
				vi.OnLinkStateUpdated(this);
			return true;
		}
	}
}

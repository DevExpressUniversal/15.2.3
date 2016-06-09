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
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraLayout {
	public class LayoutAdorner {
		LayoutControl ownerControl;
		public LayoutAdorner(Control adornedControl) {
			adornedControlCore = adornedControl;													
			ownerControl = adornedControl as LayoutControl;
			CreateAdornerWindowWindow();
		}
		internal LayoutAdornerLayeredWindow adornerWindow;
		protected virtual void CreateAdornerWindowWindow() {
			adornerWindow = new LayoutAdornerLayeredWindow(ownerControl);
		}
		public void Invalidate() {
			if(adornerWindow.IsVisible) adornerWindow.Invalidate();
		}
		public void EnsureInitialized() {
			if(!adornerWindow.IsCreated) adornerWindow.Create(AdornedControl);
		}
		Control adornedControlCore;
		public Control AdornedControl {
			get { return adornedControlCore; }
		}
		public static Control FindParentControl(Control control) {
			Form form = control.FindForm();
			if(form != null) return form;
			while(control != null) {
				if(control.Parent == null)
					return control;
				control = control.Parent;
			}
			return null;
		}
		public bool NeedHideShowAdorner() {
			if(adornerWindow == null) return false;
			if(!adornerWindow.IsCreated || !AdornedControl.IsHandleCreated) return false;
			Point ownerLocation = AdornedControl.PointToScreen(Point.Empty);
			Control ownerForm = FindParentControl(AdornedControl);
			if(ownerForm == null) return false;
			if(!adornerWindow.IsVisible) return true;
			Point adornerLocationNew = ownerForm.PointToScreen(
				new Point(-adornerPadding.Width, -adornerPadding.Height));
			adornerLocationNew.Offset(ownerLocation.X - adornerLocationNew.X, ownerLocation.Y - adornerLocationNew.Y);
			Size size = new Size(ownerForm.ClientSize.Width + adornerPadding.Width * 2,
				ownerForm.ClientSize.Height + adornerPadding.Height * 2);
			return adornerLocation != adornerLocationNew || size != adornerWindow.Size;
		}
		Point adornerLocation;
		internal Size adornerPadding = new Size(100, 100);
		public void Show() {
			EnsureInitialized();
			Point ownerLocation = AdornedControl.PointToScreen(Point.Empty);
			Control ownerForm = FindParentControl(AdornedControl);
			adornerLocation = ownerForm.PointToScreen(
				new Point(-adornerPadding.Width, -adornerPadding.Height));
			adornerLocation.Offset(ownerLocation.X - adornerLocation.X, ownerLocation.Y - adornerLocation.Y);
			Size size = new Size(ownerForm.ClientSize.Width + adornerPadding.Width * 2,
				ownerForm.ClientSize.Height + adornerPadding.Height * 2);
			adornerWindow.Size = size;
			adornerWindow.Show(adornerLocation);
		}
		public void Hide() {
			if(adornerWindow.IsCreated && adornerWindow.IsVisible) {
				adornerWindow.Hide();
			}
		}
	}
	public class LayoutAdornerLayeredWindow : DXLayeredWindowEx {
		LayoutControl ownerControl;
		public LayoutAdornerLayeredWindow(LayoutControl dlc) {
			ownerControl = dlc;
		}
		protected override void DrawCore(GraphicsCache cache) {
			ownerControl.layoutAdornerWindowHandler.Paint(cache);
		}
	}
}

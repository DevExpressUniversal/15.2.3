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
using System.Windows.Forms;
using DevExpress.XtraGauges.Core.Resources;
namespace DevExpress.XtraGauges.Win.Wizard {
	public abstract class GaugeOverviewDesignerPage : BaseGaugeDesignerPage {
		Image imageCore;
		public GaugeOverviewDesignerPage(Image image, string caption)
			: base(0, 0, caption, image) {
		}
		protected override void OnCreate() {
			LayoutChanged();
			imageCore = LoadOverviewImage();
		}
		protected override void OnDispose() {
			imageCore = null;
		}
		protected override void OnSetDesignerControl(GaugeDesignerControl designer) { }
		protected internal override bool IsAllowed { get { return true; } }
		protected internal override bool IsModified { get { return false; } }
		protected internal override bool IsHidden { get { return false; } }
		protected internal override void ApplyChanges() { }
		protected override void OnResize(EventArgs eventargs) {
			base.OnResize(eventargs);
			LayoutChanged();
		}
		protected internal override void LayoutChanged() {
			if(Owner == null) return;
		}
		protected abstract Image LoadOverviewImage();
		protected Image OverviewImage {
			get { return imageCore; }
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(OverviewImage == null) return;
			Rectangle rect = new Rectangle(ClientRectangle.Width - OverviewImage.Width, ClientRectangle.Height - OverviewImage.Height, OverviewImage.Width, OverviewImage.Height);
			e.Graphics.DrawImageUnscaled(OverviewImage, rect);
		}
	}
	public class CircularGaugeOverviewDesignerPage : GaugeOverviewDesignerPage {
		public CircularGaugeOverviewDesignerPage(string caption)
			: base(UIHelper.GaugeTypeImages[0], caption) {
			LayoutChanged();
		}
		protected override Image LoadOverviewImage() {
			return UIHelper.OverviewImages[0];
		}
	}
	public class LinearGaugeOverviewDesignerPage : GaugeOverviewDesignerPage {
		public LinearGaugeOverviewDesignerPage(string caption)
			: base(UIHelper.GaugeTypeImages[1], caption) {
			LayoutChanged();
		}
		protected override Image LoadOverviewImage() {
			return UIHelper.OverviewImages[1];
		}
	}
	public class StateIndicatorGaugeOverviewDesignerPage : GaugeOverviewDesignerPage {
		public StateIndicatorGaugeOverviewDesignerPage(string caption)
			: base(UIHelper.GaugeTypeImages[2], caption) {
			LayoutChanged();
		}
		protected override Image LoadOverviewImage() {
			return UIHelper.OverviewImages[3];
		}
	}
	public class DigitalGaugeOverviewDesignerPage : GaugeOverviewDesignerPage {
		public DigitalGaugeOverviewDesignerPage(string caption)
			: base(UIHelper.GaugeTypeImages[3], caption) {
			LayoutChanged();
		}
		protected override Image LoadOverviewImage() {
			return UIHelper.OverviewImages[2];
		}
	}
}

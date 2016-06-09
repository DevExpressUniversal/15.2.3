#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class SmallImageControl : XtraUserControl, IToolTipControlClient {
		ToolTipController toolTipController;
		protected ResourceColorImage Image { get; set; }
		public Color ImageColor { get; set; }
		public SuperToolTip SuperTip { get; set; }
		public string ToolTip { get; set; }
		public bool ImmediateTooltip { get; set; }
		public ToolTipController ToolTipController {
			get { return toolTipController; }
			set {
				if(ToolTipController == value)
					return;
				if(ToolTipController != null)
					ToolTipController.RemoveClientControl(this);
				toolTipController = value;
				if(ToolTipController != null)
					ToolTipController.AddClientControl(this);
			}
		}
		public SmallImageControl(string imageName) {
			if(string.IsNullOrWhiteSpace(imageName))
				Image = null;
			else
				Image = CreateImage(imageName);
			ImageColor = Color.Empty;
			ImmediateTooltip = true;
		}
		protected virtual ResourceColorImage CreateImage(string imageName) {
			return new ResourceColorImage(imageName);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(Image == null)
				base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e))
				Image.Paint(ImageColor, cache);
		}
		bool IToolTipControlClient.ShowToolTips { get { return !String.IsNullOrEmpty(ToolTip) || SuperTip != null; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			if(((IToolTipControlClient)this).ShowToolTips) {
				ToolTipControlInfo res = new ToolTipControlInfo(this, ToolTip, ImmediateTooltip, ToolTipIconType.None);
				res.AllowHtmlText = (SuperTip == null) ? DefaultBoolean.False : SuperTip.AllowHtmlText;
				res.SuperTip = SuperTip;
				return res;
			}
			return null;
		}
	}
}

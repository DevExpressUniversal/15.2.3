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

using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.DashboardCommon.Viewer;
namespace DevExpress.DashboardWin.Native {
	public class ScrollBarWrapper : DevExpress.DashboardCommon.Viewer.IScrollBar {
		readonly ScrollTouchBase scrollBar;
		public bool Enabled {
			get { return scrollBar.Enabled; }
			set { scrollBar.Enabled = value; }
		}
		public bool Visible {
			get { return scrollBar.ActualVisible; }
			set { scrollBar.SetVisibility(value); }
		}
		public int Width {
			get { return scrollBar.Width; }
			set { scrollBar.Width = value; }
		}
		public int Height {
			get { return scrollBar.Height; }
			set { scrollBar.Height = value; }
		}
		public int Value {
			get { return scrollBar.Value; }
			set { scrollBar.Value = value; }
		}
		public int Minimum {
			get { return scrollBar.Minimum; }
			set { scrollBar.Minimum = value; }
		}
		public int Maximum {
			get { return scrollBar.Maximum; }
			set { scrollBar.Maximum = value; }
		}
		public int SmallChange {
			get { return scrollBar.SmallChange; }
			set { scrollBar.SmallChange = value; }
		}
		public int LargeChange {
			get { return scrollBar.LargeChange; }
			set { scrollBar.LargeChange = value; }
		}
		public Point Location {
			get { return scrollBar.Location; }
			set { scrollBar.Location = value; }
		}
		public bool TouchMode { get { return scrollBar.TouchMode; } }
		public ScrollBarWrapper(ScrollTouchBase scrollBar) {
			this.scrollBar = scrollBar;
		}
	}
}

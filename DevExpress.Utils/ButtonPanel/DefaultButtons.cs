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
using System.Linq;
using System.Text;
using DevExpress.XtraBars.Docking2010;
namespace DevExpress.XtraEditors.ButtonPanel {
	public interface IDefaultButton { }
	public class DefaultButton : BaseButton, IDefaultButton {
		public DefaultButton() {
			Visible = false;
		}
		public override bool UseCaption { get { return false; } }
	}
	public class BaseCloseButton : DefaultButton {
		public override int VisibleIndex {
			get { return ButtonPainter.CloseButtonVisibleIndex; }
		}
	}
	public class BasePinButton : DefaultButton {
		public override int VisibleIndex {
			get { return ButtonPainter.PinButtonVisibleIndex; }
		}
		public override ButtonStyle Style {
			get { return ButtonStyle.CheckButton; }
		}
	}
	public class BaseMaximizeButton : DefaultButton {
		public override int VisibleIndex {
			get { return ButtonPainter.MaximizeButtonVisibleIndex; }
		}
		public override ButtonStyle Style {
			get { return ButtonStyle.CheckButton; }
		}
	}
	public class ButtonPainter {
		#region const
		public const int
			MinimizeIndex = 0,
			MaximizeIndex = 1,
			CloseIndex = 2,
			HideIndex = 3,
			PinDownIndex = 4,
			MinimizeHotIndex = 5,
			MaximizeHotIndex = 6,
			CloseHotIndex = 7,
			HideHotIndex = 8,
			PinDownHotIndex = 9;
		public const int
			MaximizeButtonVisibleIndex = 100,
			PinButtonVisibleIndex = 101,
			CloseButtonVisibleIndex = 102;
		public const int HotIndexOffset = 5;
		#endregion
	}
}

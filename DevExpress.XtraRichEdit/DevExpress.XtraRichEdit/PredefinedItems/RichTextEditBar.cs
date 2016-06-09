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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraRichEdit.UI {
	#region RichEditBarController
	[Designer("DevExpress.XtraRichEdit.Design.RichEditBarControllerDesigner," + AssemblyInfo.SRAssemblyRichEditDesign), DXToolboxItem(false), ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "RichEditBarController.bmp")]
	public class RichEditBarController : ControlCommandBarControllerBase<RichEditControl, RichEditCommandId> {
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public RichEditControl RichEditControl { get { return Control; } set { Control = value; } }
		protected override void SetControlCore(RichEditControl value) {
			base.SetControlCore(value);
		}
		protected override void SubscribeControlEvents() {
			base.SubscribeControlEvents();
		}
		private void RichEditCommentControl_LostFocus(object sender, EventArgs e) {
			RichEditCommentControl_ChangeFocus();
		}
		private void RichEditCommentControl_GotFocus(object sender, EventArgs e) {
			RichEditCommentControl_ChangeFocus();
		}
		void RichEditCommentControl_ChangeFocus() {
		}
		protected override void UnsubscribeControlEvents() {
			base.UnsubscribeControlEvents();
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit {
	#region FakeClassInRootNamespace (helper class)
	internal class FakeClassInRootNamespace {
	}
	#endregion
}

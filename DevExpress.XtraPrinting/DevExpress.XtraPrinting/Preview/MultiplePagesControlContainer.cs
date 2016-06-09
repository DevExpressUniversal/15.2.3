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
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Localization;
using System.ComponentModel;
using DevExpress.DocumentView.Controls;
using DevExpress.XtraEditors;
namespace DevExpress.XtraPrinting.Preview
{
	[ToolboxItem(false)]
	public class MultiplePagesControlContainer : SizeChooserPopupControlContainer, IPrintPreviewPopupControlContainer {
		PrintControl printControl;
		protected override int DefaultMaxPageRows { get { return PrintControl.MaxPageRows; } }
		protected override int DefaultMaxPageColumns { get { return PrintControl.MaxPageColumns; } }
		protected override string CancelButtonCaption { get { return PreviewLocalizer.GetString(PreviewStringId.Button_Cancel); } }
		protected override string SizeStringFormat { get { return "{0} x {1} " + PreviewLocalizer.GetString(PreviewStringId.MPForm_Lbl_Pages); } }
		internal PrintControl PrintControl {
			get { return printControl; }
			set { printControl = value; }
		}
		protected override void UpdatePanelHeight(int width, int rows, int columns) {
			LabelControl label = Panel.Controls[0] as LabelControl;
			SizeF realSize = XtraPrinting.Native.Measurement.MeasureString(String.Format(SizeStringFormat, rows, columns), label.Font, width, label.Appearance.GetStringFormat(), GraphicsUnit.Pixel);
			Panel.Height = (int)Math.Ceiling(realSize.Height) + 2 * InnerMargin; 
		}
		PrintControl IPrintPreviewPopupControlContainer.PrintControl { get { return this.PrintControl; } set { this.PrintControl = value; } }
		protected override void OnPopup() {
			LookAndFeel.ParentLookAndFeel = Manager.GetController().LookAndFeel;
			base.OnPopup();
		}
		protected override Bitmap LoadItemBitmap() {
			return ResourceImageHelper.CreateBitmapFromResources("DevExpress.XtraPrinting.Images.MultiplePageForm.png", Assembly.GetExecutingAssembly());
		}
	}
}

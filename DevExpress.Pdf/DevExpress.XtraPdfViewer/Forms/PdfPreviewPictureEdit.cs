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

using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.Pdf.Drawing;
namespace DevExpress.XtraPdfViewer.Native {
	[DXToolboxItem(false)]
	public class PdfPreviewPictureEdit : PictureEdit, INavigatableControl {
		PdfPrintDialogViewModel model;
		public int Position {
			get { return model.PagePreviewIndex; }
		}
		public int RecordCount {
			get { return model.PageNumbers.Length; }
		}
		public void SetModel(PdfPrintDialogViewModel model) {
			this.model = model;
		}
		public void DoAction(NavigatorButtonType type) {
			switch (type) {
				case NavigatorButtonType.First:
					model.PagePreviewIndex = 0;
					break;
				case NavigatorButtonType.Prev:
					model.PagePreviewIndex--;
					break;
				case NavigatorButtonType.Next:
					model.PagePreviewIndex++;
					break;
				case NavigatorButtonType.Last:
					model.PagePreviewIndex = model.PageNumbers.Length - 1;
					break;
			}
		}
		public bool IsActionEnabled(NavigatorButtonType type) {
			switch (type) {
				case NavigatorButtonType.First:
				case NavigatorButtonType.Prev:
				case NavigatorButtonType.Next:
				case NavigatorButtonType.Last:
					return true;
				default:
					return false;
			}
		}
		public void AddNavigator(INavigatorOwner owner) {
		}
		public void RemoveNavigator(INavigatorOwner owner) {
		}
	}
}

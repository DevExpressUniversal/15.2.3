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
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.RichEdit.UI;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Office.Internal;
namespace DevExpress.XtraRichEdit.Internal {
	#region RichEditBarItemCommandUIState
	public class RichEditBarItemCommandUIState : BarItemCommandUIState {
		public RichEditBarItemCommandUIState(BarItem item)
			: base(item) {
		}
		public override object EditValue {
			get {
				GalleryStyleItem galleryItem = Item as GalleryStyleItem;
				if (galleryItem != null)
					return galleryItem.EditValue;
				if(Item is DevExpress.Xpf.Office.UI.BarSplitButtonColorEditItem) 
					return null;
				return base.EditValue;
			}
			set {
				try {
					GalleryStyleItem galleryItem = Item as GalleryStyleItem;
					if (galleryItem != null) {
						StyleFormattingBase oldValue = galleryItem.EditValue as StyleFormattingBase;
						StyleFormattingBase newValue = value as StyleFormattingBase;
						if (oldValue == null)
							galleryItem.EditValue = value;
						else if (oldValue != null && newValue != null && oldValue.StyleId != newValue.StyleId)
							galleryItem.EditValue = value;
						return;
					}
				}
				catch {
					return;
				}
				base.EditValue = value;
			}
		}
	}
	#endregion
}

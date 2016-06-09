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

using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.Viewer {
	public class ItemCaptionContentInfo {
		public string CaptionText { get; set; }
		public string FilterValuesText { get; set; }
		public ItemCaptionContentInfo() {
			CaptionText = String.Empty;
			FilterValuesText = String.Empty;
		}
	}
	public interface IItemCaption {
		void Update(ItemCaptionContentInfo itemCaptionContentInfo);
	}
	public class ItemCaptionViewControl {
		readonly IItemCaption itemCaption;
		ItemCaptionContentInfo itemCaptionContentInfo;
		public ItemCaptionViewControl(IItemCaption itemCaption) {
			this.itemCaption = itemCaption;
		}
		public void Update(string caption, IEnumerable<FormattableValue> filterValues) {
			itemCaptionContentInfo = new ItemCaptionContentInfo();
			itemCaptionContentInfo.CaptionText = caption;
			StringBuilder sb = new StringBuilder();
			if(filterValues != null) {
				foreach(FormattableValue filterValue in filterValues) {
					FormatterBase formatterBase = FormatterBase.CreateFormatter(filterValue.Format);
					sb.AppendFormat(" - {0}", formatterBase.Format(filterValue.Value));
				}
			}
			itemCaptionContentInfo.FilterValuesText = sb.ToString();
			itemCaption.Update(itemCaptionContentInfo);
		}
		public void Update() {
			if (itemCaptionContentInfo != null) {
				itemCaption.Update(itemCaptionContentInfo);
			}
		}
	}
}

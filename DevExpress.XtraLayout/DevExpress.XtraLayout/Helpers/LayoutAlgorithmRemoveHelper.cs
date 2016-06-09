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

using DevExpress.XtraLayout.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraLayout.Helpers {
	public class LayoutAlgorithmRemoveHelper {
		public static void Process(List<BaseLayoutItem> listBLI, LayoutType lType, BaseLayoutItem itemToRemove) {
			LayoutRectangle biglRect = new LayoutRectangle(BaseItemCollection.CalcItemsBounds(listBLI), lType);
			LayoutRectangle removeItemRectangle = new LayoutRectangle(itemToRemove.Bounds, lType);
			if(removeItemRectangle.Width == biglRect.Width) {
				listBLI.Remove(itemToRemove);
				foreach(BaseLayoutItem item in listBLI) {
					LayoutRectangle newRectangle = new LayoutRectangle(item.Bounds, lType);
					if(newRectangle.Y > removeItemRectangle.Y) newRectangle.Y -= removeItemRectangle.Height;
					item.SetBounds(newRectangle);
				}
			} else {
				if(itemToRemove.Parent != null) itemToRemove.Parent.Remove(itemToRemove);
			}
		}
		public static bool Process(LayoutGroupItemCollection listBLI, LayoutType lType, BaseLayoutItem itemToRemove) {
			LayoutRectangle bigLRect = new LayoutRectangle(BaseItemCollection.CalcItemsBounds(listBLI.ConvertToTypedList()), lType);
			LayoutRectangle removeItemRectangle = new LayoutRectangle(itemToRemove.Bounds, lType);
			if(removeItemRectangle.Width == bigLRect.Width) {
				foreach(BaseLayoutItem item in listBLI) {
					LayoutRectangle newRectangle = new LayoutRectangle(item.Bounds, lType);
					if(newRectangle.Y > removeItemRectangle.Y) newRectangle.Y -= removeItemRectangle.Height;
					item.SetBounds(newRectangle);
				}
				return true;
			}
			return false;
		}
	}
}

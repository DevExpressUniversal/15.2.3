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
using System.Collections;
using System.Text;
using DevExpress.XtraLayout;
namespace DevExpress.XtraLayout.Customization {
	public class LayoutControlWalker {
		LayoutGroup rootGroup;
		protected ArrayList elements;
		public LayoutControlWalker(LayoutGroup root) {
			rootGroup = root;
		}
		protected virtual void FillElements() {
			elements = new ArrayList(rootGroup.Items);
		}
		public ArrayList ArrangeElements(OptionsFocus options) {
			if(rootGroup == null) return null; 
			FillElements();
			ArrayList tempList = new ArrayList(elements);
			ArrayList resultList = new ArrayList();
			int watchdog = tempList.Count;
			while(tempList.Count > 0) {
				if(watchdog < 0) throw new LayoutControlInternalException("could not arrange elements");
				BaseLayoutItem item = GetElementInListByDirection(tempList, options);
				if(item != null) {
					resultList.Add(item);
					tempList.Remove(item);
				}
				watchdog--;
			}
			return resultList;
		}
		protected ArrayList GetElementInListByDirection1(ArrayList list, OptionsFocus options) {
			ArrayList result = new ArrayList();
			int minx = int.MaxValue;
			int maxx = int.MinValue;
			int miny = int.MaxValue;
			foreach(BaseLayoutItem item in list) {
				if(options.MoveFocusDirection == MoveFocusDirection.DownThenAcross) {
					if(!options.MoveFocusRightToLeft) {
						if(item.X == minx) {
							result.Add(item);
						}
						if(item.X < minx) {
							result.Clear();
							result.Add(item);
							minx = item.X;
						}
					}
					else {
						if(item.X == minx) {
							result.Add(item);
						}
						if(item.X > maxx) {
							result.Clear();
							result.Add(item);
							maxx = item.X;
						}
					}				
				}
				else {
					if(item.Y == miny) {
						result.Add(item);
					}
					if(item.Y < miny) {
						result.Clear();
						result.Add(item);
						miny = item.Y;
					}
				}
			}
			return result;
		}
		protected BaseLayoutItem GetElementInListByDirection2(ArrayList list, OptionsFocus options) {
			BaseLayoutItem result = null;
			int miny = int.MaxValue;
			int maxx = int.MinValue;
			int minx = int.MaxValue;
			foreach(BaseLayoutItem item in list) {
				if(options.MoveFocusDirection == MoveFocusDirection.DownThenAcross) {
					if(item.Y < miny) {
						result = item;
						miny = item.Y;
					}
				}
				else {
					if(options.MoveFocusRightToLeft) {
						if(item.X > maxx) {
							result = item;
							maxx = item.X;
						}
					}
					else {
						if(item.X < minx) {
							result = item;
							minx = item.X;
						}
					}
				}
			}
			return result;
		}
		protected BaseLayoutItem GetElementInListByDirection(ArrayList list, OptionsFocus options) {
			ArrayList firstStageResult = GetElementInListByDirection1(list, options);
			BaseLayoutItem item = GetElementInListByDirection2(firstStageResult, options);
			return item;
		}
	}	
}

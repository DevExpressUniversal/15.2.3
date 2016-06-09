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
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraPrinting.Native.LayoutAdjustment {
	public class LayoutAdjuster {
		#region static
		static float CalcMinInitialBottomSpan(ILayoutData data, IList<ILayoutData> collection) {
			float span = Int32.MaxValue;
			foreach(ILayoutData predData in collection)
				span = Math.Min(span, data.InitialRect.Top - predData.InitialRect.Bottom);
			if(span.Equals(Int32.MaxValue))
				span = 0;
			return span;
		}
		static void SetControlViewDataY(ILayoutData data, IList<ILayoutData> collection) {
			if(data.AnchorVertical != VerticalAnchorStyles.None)
				return;
			float y = Int32.MinValue;
			float span = CalcMinInitialBottomSpan(data, collection);
			foreach(ILayoutData predData in collection)
				y = Math.Max(y, predData.Bottom + span);
			if(!y.Equals(Int32.MinValue))
				data.SetBoundsY(y);
		}		
		#endregion
		protected float dpi;
		public LayoutAdjuster(float dpi) {
			this.dpi = dpi;
		}
		public void Process(List<ILayoutData> layoutData) {
			if(!NeedAdjust(layoutData))
				return;
			Adjust(layoutData);
		}
		protected virtual void Adjust(List<ILayoutData> layoutData) {
			layoutData.Sort(new LayoutDataComparer());
			LayoutCollection[] layoutCollections = new LayoutCollection[layoutData.Count];
			for(int i = 0; i < layoutData.Count; i++)
				layoutCollections[i] = new LayoutCollection(layoutData, i);
			for(int i = 0; i < layoutData.Count; i++) {
				ILayoutData item = layoutData[i];
				new LayoutAdjuster(dpi).Process(item.ChildrenData);
				SetControlViewDataY(item, layoutCollections[i]);
				if((item.AnchorVertical & VerticalAnchorStyles.Bottom) == 0 && item.NeedAdjust)
					item.UpdateViewBounds();
			}
		}
		bool NeedAdjust(List<ILayoutData> layoutData) {
			if(layoutData != null) {
				foreach(ILayoutData item in layoutData) {
					if(item.NeedAdjust)
						return true;
				}
			}
			return false;
		}
	}
	public class LayoutAdjusterWithAnchoring : LayoutAdjuster {
		public LayoutAdjusterWithAnchoring(float dpi)
			: base(dpi) {
		}
		protected override void Adjust(List<ILayoutData> layoutData) {
			base.Adjust(layoutData);
			foreach (ILayoutData item in layoutData) {
				float delta = Math.Max(item.Bottom - item.InitialRect.Bottom, 0);
				item.Anchor(delta, dpi);
			}
		}
	}
}

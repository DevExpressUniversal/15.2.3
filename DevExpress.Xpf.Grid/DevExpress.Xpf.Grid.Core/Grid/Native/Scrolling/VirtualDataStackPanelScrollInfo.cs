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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Native;
namespace DevExpress.Xpf.Grid.Native {
	public class VirtualDataStackPanelScrollInfo : ScrollInfo {
		public static readonly DependencyProperty OrientationProperty;
		static VirtualDataStackPanelScrollInfo() {
			OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(VirtualDataStackPanelScrollInfo), new PropertyMetadata(Orientation.Vertical, (d, e) => ((VirtualDataStackPanelScrollInfo)d).OnOrientationChanged((Orientation)e.NewValue)));
		}
		public VirtualDataStackPanelScrollInfo(IScrollInfoOwner owner)
			: base(owner) {
		}
		Orientation orientationCore = Orientation.Vertical;
		public Orientation Orientation {
			get { return orientationCore; } 
			set { SetValue(OrientationProperty, value); }
		}
		public override ScrollByItemInfo DefineSizeScrollInfo {
			get { return (ScrollByItemInfo)(Orientation == Orientation.Vertical ? VerticalScrollInfo : HorizontalScrollInfo); }
		}
		public override ScrollByPixelInfo SecondarySizeScrollInfo {
			get { return (ScrollByPixelInfo)(Orientation == Orientation.Vertical ? HorizontalScrollInfo : VerticalScrollInfo); } 
		}
		protected override ScrollInfoBase CreateVerticalScrollInfo() {
			return CreateScrollInfo(Owner.VerticalScrollMode, SizeHelperBase.GetDefineSizeHelper(Orientation.Vertical));
		}
		protected override ScrollInfoBase CreateHorizontalScrollInfo() {
			return CreateScrollInfo(Owner.HorizontalScrollMode, SizeHelperBase.GetDefineSizeHelper(Orientation.Horizontal));
		}
		ScrollInfoBase CreateScrollInfo(DataControlScrollMode scrollMode, SizeHelperBase sizeHelper) {
			if(scrollMode == DataControlScrollMode.Pixel)
				return new ScrollByPixelInfo(Owner, sizeHelper);
			if(scrollMode == DataControlScrollMode.Item)
				return new ScrollByItemInfo(Owner, sizeHelper);
			if(scrollMode == DataControlScrollMode.ItemPixel)
				return new ScrollByItemPixelInfo(Owner, sizeHelper);
			return new ScrollByRowPixelInfo(Owner, sizeHelper);
		}
		void OnOrientationChanged(Orientation newOrientation) {
			orientationCore = newOrientation;
			ClearScrollInfo();
		}
	}
}

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

using System.Windows;
using System;
using DevExpress.Xpf.Utils;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Editors {
	public class RangeEditBaseInfo : DependencyObject {
		public static readonly DependencyProperty AnimationProgressProperty;
		public static readonly DependencyProperty LayoutInfoProperty;
		public static readonly DependencyProperty LeftSidePositionProperty;
		public static readonly DependencyProperty RightSidePositionProperty;
		public static readonly DependencyProperty DisplayValueProperty;
		static RangeEditBaseInfo() {
			Type ownerType = typeof(RangeEditBaseInfo);
			LayoutInfoProperty = DependencyPropertyManager.RegisterAttached("LayoutInfo", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			AnimationProgressProperty = DependencyPropertyManager.Register("AnimationProgress", typeof(double), ownerType);
			LeftSidePositionProperty = DependencyPropertyManager.Register("LeftSidePosition", typeof(double), ownerType);
			RightSidePositionProperty = DependencyPropertyManager.Register("RightSidePosition", typeof(double), ownerType);
			DisplayValueProperty = DependencyPropertyManager.Register("DisplayValue", typeof(double), ownerType);
		}
		public static RangeEditBaseInfo GetLayoutInfo(DependencyObject d) {
			return (RangeEditBaseInfo)d.GetValue(LayoutInfoProperty);
		}
		public static void SetLayoutInfo(DependencyObject d, RangeEditBaseInfo info) {
			d.SetValue(LayoutInfoProperty, info);
		}
		public double DisplayValue {
			get { return (double)GetValue(DisplayValueProperty); }
			set { SetValue(DisplayValueProperty, value); }
		}
		public double AnimationProgress {
			get { return (double)GetValue(AnimationProgressProperty); }
			set { SetValue(AnimationProgressProperty, value); }
		}
		public double LeftSidePosition {
			get { return (double)GetValue(LeftSidePositionProperty); }
			set { SetValue(LeftSidePositionProperty, value); }
		}
		public double RightSidePosition {
			get { return (double)GetValue(RightSidePositionProperty); }
			set { SetValue(RightSidePositionProperty, value); }
		}
		public RangeEditBaseInfo() {
		}
	}
}

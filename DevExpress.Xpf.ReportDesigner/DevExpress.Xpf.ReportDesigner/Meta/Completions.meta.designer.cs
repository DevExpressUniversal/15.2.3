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

namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Mvvm.UI.Native;
using Binders;
using XtraReports.UI;
using Diagram;
	partial class DefaultLayout {
		public static readonly DependencyProperty XRControlProperty;
		public XRControl XRControl {
			get { return (XRControl)GetValue(XRControlProperty); }
			set { SetValue(XRControlProperty, value); }
		}
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Diagram;
using Mvvm.UI.Native;
	partial class DiagramItemLayout {
		public static readonly DependencyProperty DpiProperty;
		public float Dpi {
			get { return (float)GetValue(DpiProperty); }
			set { SetValue(DpiProperty, value); }
		}
		public static readonly DependencyProperty PositionFProperty;
		public System.Drawing.PointF PositionF {
			get { return (System.Drawing.PointF)GetValue(PositionFProperty); }
			set { SetValue(PositionFProperty, value); }
		}
		public static readonly DependencyProperty WidthFProperty;
		public float WidthF {
			get { return (float)GetValue(WidthFProperty); }
			set { SetValue(WidthFProperty, value); }
		}
		public static readonly DependencyProperty HeightFProperty;
		public float HeightF {
			get { return (float)GetValue(HeightFProperty); }
			set { SetValue(HeightFProperty, value); }
		}
		public static readonly DependencyProperty PositionProperty;
		public Point Position {
			get { return (Point)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}
		public static readonly DependencyProperty WidthProperty;
		public double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public static readonly DependencyProperty HeightProperty;
		public double Height {
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		public static readonly DependencyProperty LeftProperty;
		public double Left {
			get { return (double)GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}
		public static readonly DependencyProperty TopProperty;
		public double Top {
			get { return (double)GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}
		public static readonly DependencyProperty RightProperty;
		public double Right {
			get { return (double)GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}
		public static readonly DependencyProperty BottomProperty;
		public double Bottom {
			get { return (double)GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}
		public static readonly DependencyProperty ParentLeftAbsoluteProperty;
		public double ParentLeftAbsolute {
			get { return (double)GetValue(ParentLeftAbsoluteProperty); }
			set { SetValue(ParentLeftAbsoluteProperty, value); }
		}
		public static readonly DependencyProperty ParentTopAbsoluteProperty;
		public double ParentTopAbsolute {
			get { return (double)GetValue(ParentTopAbsoluteProperty); }
			set { SetValue(ParentTopAbsoluteProperty, value); }
		}
		public static readonly DependencyProperty LeftAbsoluteProperty;
		public double LeftAbsolute {
			get { return (double)GetValue(LeftAbsoluteProperty); }
			set { SetValue(LeftAbsoluteProperty, value); }
		}
		public static readonly DependencyProperty TopAbsoluteProperty;
		public double TopAbsolute {
			get { return (double)GetValue(TopAbsoluteProperty); }
			set { SetValue(TopAbsoluteProperty, value); }
		}
		public static readonly DependencyProperty RightAbsoluteProperty;
		public double RightAbsolute {
			get { return (double)GetValue(RightAbsoluteProperty); }
			set { SetValue(RightAbsoluteProperty, value); }
		}
		public static readonly DependencyProperty BottomAbsoluteProperty;
		public double BottomAbsolute {
			get { return (double)GetValue(BottomAbsoluteProperty); }
			set { SetValue(BottomAbsoluteProperty, value); }
		}
		public static readonly DependencyProperty LayoutProperty;
		public static DiagramItemLayout GetLayout(DiagramItem d) {
			return (DiagramItemLayout)d.GetValue(LayoutProperty);
		}
		public static void SetLayout(DiagramItem d, DiagramItemLayout value) {
			d.SetValue(LayoutProperty, value);
		}
		public static readonly DependencyProperty ItemProperty;
		public DiagramItem Item {
			get { return (DiagramItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Diagram;
	partial class DiagramControlEx {
		public static readonly DependencyProperty OwnerProperty;
		static readonly DependencyPropertyKey OwnerPropertyKey;
		public static DiagramItem GetOwner(DiagramItem d) {
			return (DiagramItem)d.GetValue(OwnerProperty);
		}
		static void SetOwner(DiagramItem d, DiagramItem value) {
			d.SetValue(OwnerPropertyKey, value);
		}
	}
}

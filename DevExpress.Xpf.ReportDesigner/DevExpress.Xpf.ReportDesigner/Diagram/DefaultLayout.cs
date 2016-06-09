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
	public partial class DefaultLayout : DiagramItemLayout {
		static DefaultLayout() {
			DependencyPropertyRegistrator<DefaultLayout>.New()
				.Register(d => d.XRControl, out XRControlProperty, (XRControl)null)
			;
		}
		public DefaultLayout() {
			XRBinder.Bind(this, XRControlProperty, (XRControl x) => x.LocationF, PositionFProperty);
			XRBinder.Bind(this, XRControlProperty, (XRControl x) => x.WidthF, WidthFProperty);
			XRBinder.Bind(this, XRControlProperty, (XRControl x) => x.HeightF, HeightFProperty);
			DpiBinder.BindPoint(this, DpiProperty, PositionFProperty, PositionProperty);
			DpiBinder.BindScalar(this, DpiProperty, WidthFProperty, WidthProperty);
			DpiBinder.BindScalar(this, DpiProperty, HeightFProperty, HeightProperty);
			PointToScalarBinder.BindX(this, PositionProperty, LeftProperty);
			PointToScalarBinder.BindY(this, PositionProperty, TopProperty);
			FarBoundBinder.Bind(this, LeftProperty, WidthProperty, RightProperty);
			FarBoundBinder.Bind(this, TopProperty, HeightProperty, BottomProperty);
			AbsoluteBinder.BindX(this, LeftProperty, LeftAbsoluteProperty);
			AbsoluteBinder.BindX(this, RightProperty, RightAbsoluteProperty);
			AbsoluteBinder.BindY(this, TopProperty, TopAbsoluteProperty);
			AbsoluteBinder.BindY(this, BottomProperty, BottomAbsoluteProperty);
		}
		protected override void BindItem(DiagramItem item) {
			DiagramBinder.Bind(this, PositionProperty, item, DiagramItem.PositionProperty);
			DiagramBinder.Bind(this, WidthProperty, item, DiagramItem.WidthProperty);
			DiagramBinder.Bind(this, HeightProperty, item, DiagramItem.HeightProperty);
		}
	}
}

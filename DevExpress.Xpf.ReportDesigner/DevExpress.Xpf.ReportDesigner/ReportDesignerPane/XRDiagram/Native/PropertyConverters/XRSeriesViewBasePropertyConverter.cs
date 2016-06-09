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
using System.ComponentModel;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.PropertyConverters {
	public class XRSeriesViewBasePropertyConverter : TypeConverter, IXRPropertyConverter {
		public Type PropertyType { get { return typeof(SeriesViewBase); } }
		public Type VirtualPropertyType { get { return typeof(ViewType); } }
		public object Convert(object value, object owner) {
			return new SeriesViewType((SeriesViewBase)value, SeriesViewFactory.GetViewType((SeriesViewBase)value));
		}
		public object ConvertBack(object value) {
			var newView = SeriesViewFactory.CreateInstance(((SeriesViewType)value).ViewType);
			CommonUtils.CopySettings(newView, ((SeriesViewType)value).View);
			return newView;
		}
	}
}
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public struct SeriesViewType {
		public SeriesViewType(SeriesViewBase view, ViewType viewType) {
			this.view = view;
			this.viewType = viewType;
		}
		readonly SeriesViewBase view;
		public SeriesViewBase View { get { return view; } }
		readonly ViewType viewType;
		public ViewType ViewType { get { return viewType; } }
		public override string ToString() {
			return SeriesViewFactory.GetStringID(ViewType);
		}
	}
}

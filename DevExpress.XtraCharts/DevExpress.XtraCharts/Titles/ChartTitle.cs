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
using System.Globalization;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraCharts {
	[TypeConverter(typeof(ChartTitleTypeConverter))]
	public class ChartTitle : DockableTitle {
		protected override int VisibilityPriority { get { return (int)ChartElementVisibilityPriority.ChartTitle; } }
		protected override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.DefaultChartTitle); } }
		protected override Font DefaultFont { get { return DefaultFonts.Tahoma18; } }
		public ChartTitle() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new ChartTitle();
		}
	}
	public class ChartTitleCollection : DockableTitleCollectionBase {
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ChartTitleCollectionItem")]
#endif
		public new ChartTitle this[int index] { get { return (ChartTitle)base[index]; } }
		internal ChartTitleCollection(Chart chart) : base(chart) {
		}
		internal bool Contains(object obj) {
			ChartTitle title = obj as ChartTitle;
			return title != null && Contains(title);
		}
		internal void InternalInsert(int index, ChartTitle title) {
			InnerList.Insert(index, title);
			ChangeOwnerForItem(title);
			RaiseControlChanged();
		}
		internal void InternalRemoveAt(int index) {
			InnerList.RemoveAt(index);
		}
		public int Add(ChartTitle title) {
			return base.Add(title);
		}
		public bool Contains(ChartTitle title) {
			return base.Contains(title);
		}
		public void AddRange(ChartTitle[] coll) {
			base.AddRange(coll);
		}
		public void Remove(ChartTitle title) {
			base.Remove(title);
		}
		public void Insert(int index, ChartTitle title) {
			base.Insert(index, title);
		}
	}
}

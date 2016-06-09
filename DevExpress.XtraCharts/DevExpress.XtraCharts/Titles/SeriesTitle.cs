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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SeriesTitleTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SeriesTitle : DockableTitle {
		bool UseTemplateHit {
			get {
				SimpleDiagramSeriesViewBase view = Owner as SimpleDiagramSeriesViewBase;
				if(view != null) {
					Series series = view.Owner as Series;
					return series != null && series.UseTemplateHit;
				}
				return false;
			}
		}
		SeriesTitle TemplateTitle {
			get {
				SimpleDiagramSeriesViewBase view = (SimpleDiagramSeriesViewBase)Owner;
				SimpleDiagramSeriesViewBase templateView = (SimpleDiagramSeriesViewBase)view.Chart.DataContainer.SeriesTemplate.View;
				int index = view.Titles.IndexOf(this);
				ChartDebug.Assert(index >= 0 && index < templateView.Titles.Count);
				return templateView.Titles[index];
			}
		}
		string SeriesName {
			get {
				SimpleDiagramSeriesViewBase view = Owner as SimpleDiagramSeriesViewBase;
				if(view != null) {
					Series series = view.Owner as Series;
					if(series != null)
						return series.Name;
				}
				return String.Empty;
			}
		}
		protected override int VisibilityPriority { get { return (int)ChartElementVisibilityPriority.SeriesTitle; } }
		protected override string DefaultText { get { return PatternUtils.SeriesNamePattern; } }
		protected override Font DefaultFont { get { return DefaultFonts.Tahoma12; } }
		#region IHitTest implementation
		protected internal override object HitObject { get { return UseTemplateHit ? TemplateTitle.HitObject : base.HitObject; } }
		protected internal override HitTestState HitState { get { return UseTemplateHit ? TemplateTitle.HitState :  base.HitState; } }
		#endregion
		public SeriesTitle() : base() {
		}
		protected internal override string ConstrucActualText() {
			if (String.IsNullOrEmpty(Text))
				return String.Empty;
			List<string> parsedPattern = PatternUtils.ParsePattern(Text);
			if (parsedPattern.Count == 0)
				return String.Empty;
			string result = String.Empty;
			foreach(string fragment in parsedPattern) {
				if(fragment == PatternUtils.SeriesNamePattern || fragment == PatternUtils.SeriesNamePatternLowercase)
					result += SeriesName;
				else
					result += fragment;
			}
			return result;
		}
		protected override ChartElement CreateObjectForClone() {
			return new SeriesTitle();
		}
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartCollectionSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SeriesTitleCollection : DockableTitleCollectionBase {
		public new SeriesTitle this[int index] { get { return (SeriesTitle)base[index]; } }
		internal SeriesTitleCollection(ChartElement owner) : base(owner) {
		}
		public int Add(SeriesTitle title) {
			return base.Add(title);
		}
		public void AddRange(SeriesTitle[] coll) {
			base.AddRange(coll);
		}
		public void Remove(SeriesTitle title) {
			base.Remove(title);
		}
		public void Insert(int index, SeriesTitle title) {
			base.Insert(index, title);
		}
		public bool Contains(SeriesTitle title) {
			return base.Contains(title);
		}
		internal bool Contains(object obj) {
			SeriesTitle title = obj as SeriesTitle;
			return title != null && Contains(title);
		}
		internal int IndexOf(SeriesTitle title) {
			return base.IndexOf(title);
		}
	}
}

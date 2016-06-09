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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public abstract class PointViewBaseModel : ColorEachSupportViewBaseModel {
		protected new PointSeriesViewBase SeriesView { get { return (PointSeriesViewBase)base.SeriesView; } }
		public PointViewBaseModel(PointSeriesViewBase seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
	}
	[ModelOf(typeof(PointSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(PointSeriesViewTypeConverter))]
	public class PointViewModel : PointViewBaseModel {
		SimpleMarkerModel simpleMarkerModel;
		protected new PointSeriesView SeriesView { get { return (PointSeriesView)base.SeriesView; } }
		[PropertyForOptions,
		AllocateToGroup("Marker Options")]
		public SimpleMarkerModel PointMarkerOptions { get { return simpleMarkerModel; } }
		public PointViewModel(PointSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if (simpleMarkerModel != null)
				Children.Add(simpleMarkerModel);
			base.AddChildren();
		}
		public override void Update() {
			this.simpleMarkerModel = new SimpleMarkerModel(SeriesView.PointMarkerOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(BubbleSeriesView)),
	GroupPrefix("View: "),
	TypeConverter(typeof(BubbleSeriesViewTypeConverter))]
	public class BubbleViewModel : PointViewBaseModel {
		MarkerBaseModel markerOptionsModel;
		protected new BubbleSeriesView SeriesView { get { return (BubbleSeriesView)base.SeriesView; } }
		[PropertyForOptions]
		public bool AutoSize {
			get { return SeriesView.AutoSize; }
			set { SetProperty("AutoSize", value); }
		}
		[PropertyForOptions]
		public double MaxSize {
			get { return SeriesView.MaxSize; }
			set { SetProperty("MaxSize", value); }
		}
		[PropertyForOptions]
		public double MinSize {
			get { return SeriesView.MinSize; }
			set { SetProperty("MinSize", value); }
		}
		public byte Transparency {
			get { return SeriesView.Transparency; }
			set { SetProperty("Transparency", value); }
		}
		[PropertyForOptions,
		AllocateToGroup("Marker Options")]
		public MarkerBaseModel BubbleMarkerOptions { get { return markerOptionsModel; } }
		public BubbleViewModel(BubbleSeriesView seriesView, CommandManager commandManager)
			: base(seriesView, commandManager) {
		}
		protected override void AddChildren() {
			if (markerOptionsModel != null)
				Children.Add(markerOptionsModel);
			base.AddChildren();
		}
		public override List<DataMemberInfo> GetDataMembersInfo() {
			DesignerSeriesModelBase seriesModel = Parent as DesignerSeriesModelBase;
			List<DataMemberInfo> dataMembersInfo = new List<DataMemberInfo>();
			if (seriesModel != null) {
				dataMembersInfo.Add(new DataMemberInfo("ValueDataMember", "Value", seriesModel.ValueDataMembers[0], ValueScaleTypes));
				dataMembersInfo.Add(new DataMemberInfo("WeightDataMember", "Weight", seriesModel.ValueDataMembers[1], ValueScaleTypes));
			}
			return dataMembersInfo;
		}
		public override void Update() {
			this.markerOptionsModel = new MarkerBaseModel(SeriesView.BubbleMarkerOptions, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
}

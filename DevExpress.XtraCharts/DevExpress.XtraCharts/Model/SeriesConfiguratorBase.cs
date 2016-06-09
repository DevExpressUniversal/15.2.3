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

using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using Model = DevExpress.Charts.Model;
namespace DevExpress.XtraCharts.ModelSupport.Implementation {
	public abstract class SeriesConfiguratorBase {
		public virtual Diagram Diagram { get; set; }
		public abstract void Configure(Series series, Model.SeriesModel model);
		public virtual void ConfigureSecondaryAxes(Series series, Model.SeriesModel model) {
		}
	}
	public abstract class ComplexSeriesConfigurator : SeriesConfiguratorBase {
		List<SeriesConfiguratorBase> list;
		protected List<SeriesConfiguratorBase> List { get { return list; } }
		public ComplexSeriesConfigurator() {
			this.list = new List<SeriesConfiguratorBase>();
			FillConfiguratorList();
		}
		public abstract void FillConfiguratorList();
		protected void AddConfigurator(SeriesConfiguratorBase item) {
			if(!List.Contains(item)) {
				List.Add(item);
			}
		}
		public override void Configure(Series series, Model.SeriesModel model) {
			int count = list.Count;
			for(int i = 0; i < count; i++) {
				SeriesConfiguratorBase item = List[i];
				item.Diagram = Diagram;
				item.Configure(series, model);
			}
		}
		public override void ConfigureSecondaryAxes(Series series, Model.SeriesModel model) {
			base.ConfigureSecondaryAxes(series, model);
			foreach (SeriesConfiguratorBase configurator in list)
				if (configurator is SecondaryAxesConfigurator) {
					configurator.Diagram = Diagram;
					configurator.Configure(series, model);
					break;
				}
		}
	}
}

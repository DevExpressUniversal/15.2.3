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
using System.ComponentModel;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(DataContainer))]
	public class DataContainerModel : DesignerChartElementModelBase {
		readonly DataContainer dataContainer;
		readonly DesignerSeriesModelBase seriesTemplate;
		protected internal override ChartElement ChartElement {
			get { return dataContainer; }
		}
		public string DataMember {
			get { return dataContainer.DataMember; }
		}
		[Category("Misc")]
		public object DataSource {
			get { return dataContainer.ActualDataSource; }
			set {
				SetProperty("DataSource", value);
			}
		}
		[Category("Misc")]
		public DesignerSeriesModelBase SeriesTemplate { get { return seriesTemplate; } }
		public DataContainerModel(DataContainer dataContainer, CommandManager commandManager) : base(commandManager) {
			this.dataContainer = dataContainer;
			this.seriesTemplate = new DesignerSeriesModelBase(dataContainer.SeriesTemplate, CommandManager);
		}
		public override void Update() {
			seriesTemplate.Parent = this;
			seriesTemplate.Update();
			base.Update();
		}
	}
}

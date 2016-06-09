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

using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Designer.Native {
	public class DeleteSeriesCommand : DeleteCommandBase<Series> {
		readonly SeriesCollection seriesCollection;
		protected override ChartCollectionBase ChartCollection { get { return seriesCollection; } }
		public DeleteSeriesCommand(CommandManager commandManager, SeriesCollection seriesCollection)
			: base(commandManager) {
			this.seriesCollection = seriesCollection;
		}
		protected override void InsertIntoCollection(int index, Series chartElement) {
			seriesCollection.Insert(index, chartElement);
		}
	}
	public class DeleteOnlySeriesCommand : DeleteSeriesCommand {
		public DeleteOnlySeriesCommand(CommandManager commandManager, SeriesCollection seriesCollection)
			: base(commandManager, seriesCollection) {
		}
		protected override object CreateCollectionPropertiesCache(Series chartElement) {
			IChartContainer container = CommonUtils.FindChartContainer(chartElement);
			return container.Chart.AnnotationRepository.ToArray();
		}
		protected override void RestoreCollectionProperties(Series chartElement, object properties) {
			Annotation[] restoringAnnotations = (Annotation[])properties;
			IChartContainer container = CommonUtils.FindChartContainer(chartElement);
			AnnotationRepository annotations = container.Chart.AnnotationRepository;
			annotations.BeginUpdate();
			try {
				annotations.Clear();
				annotations.AddRange(restoringAnnotations);
			} finally {
				annotations.EndUpdate();
			}
		}
	}
}

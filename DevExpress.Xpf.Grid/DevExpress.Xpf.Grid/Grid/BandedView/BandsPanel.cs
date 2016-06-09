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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Bars;
using System.ComponentModel;
using DevExpress.Mvvm.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class BandsPanelBase : Panel {
		public static readonly DependencyProperty BandsProperty;
		static BandsPanelBase() {
			Type ownerType = typeof(BandsPanelBase);
			BandsProperty = DependencyProperty.Register("Bands", typeof(IList<BandBase>), ownerType, new PropertyMetadata(null, (d, e) => ((BandsPanelBase)d).InvalidateMeasure()));
		}
		public IList<BandBase> Bands {
			get { return (IList<BandBase>)GetValue(BandsProperty); }
			set { SetValue(BandsProperty, value); }
		}
		Dictionary<BandBase, FrameworkElement> bandsDictionary = new Dictionary<BandBase, FrameworkElement>();
		List<FrameworkElement> freeBands = new List<FrameworkElement>();
		protected override Size MeasureOverride(Size availableSize) {
			if(Bands == null)
				return Size.Empty;
			List<BandBase> unusedBands = bandsDictionary.Keys.ToList();
			RemoveUnusedBands(Bands, unusedBands, 0);
			foreach(BandBase band in unusedBands) {
				freeBands.Add(bandsDictionary[band]);
				bandsDictionary.Remove(band);
			}
			double height = 0;
			double width = 0;
			foreach(BandBase band in Bands) {
				height = Math.Max(height, MeasureBand(band, width));
				width += GetBandWidth(band);
			}
			foreach(FrameworkElement element in freeBands) {
				element.Visibility = Visibility.Collapsed;
				element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			}
			return new Size(width, height);
		}
		public bool AllowVirtualization { get; set; }
		bool ActualAllowVirtualization { get { return AllowVirtualization && TableView.AllowHorizontalScrollingVirtualization; } }
		void RemoveUnusedBands(IList<BandBase> bands, List<BandBase> unusedBands, double currentWidth) {
			foreach(BandBase band in bands) {
				if(!ActualAllowVirtualization || (currentWidth + GetBandWidth(band) > TableView.TableViewBehavior.HorizontalOffset && currentWidth <= TableView.TableViewBehavior.HorizontalOffset + TableView.HorizontalViewport)) {
					unusedBands.Remove(band);
				}
				if(band.VisibleBands.Count != 0) RemoveUnusedBands(band.VisibleBands, unusedBands, currentWidth);
				currentWidth += band.ActualHeaderWidth;
			}
		}
		protected ITableView TableView { get { return DataView as ITableView; } }
		protected DataViewBase DataView { get { return DataControlBase.GetCurrentView(this); } }
		double MeasureBand(BandBase band, double currentWidth) {
			if(ActualAllowVirtualization) {
				if(currentWidth > TableView.TableViewBehavior.HorizontalOffset + TableView.HorizontalViewport + TableView.TotalGroupAreaIndent) return 0;
				if(currentWidth + GetBandWidth(band) <= TableView.TableViewBehavior.HorizontalOffset) return 0;
			}
			FrameworkElement bandElement = null;
			if(!bandsDictionary.TryGetValue(band, out bandElement)) {
				if(freeBands.Count > 0) {
					bandElement = freeBands[0];
					freeBands.RemoveAt(0);
					bandElement.Visibility = Visibility.Visible;
					bandElement.DataContext = band;
#if !SL
					BandHeaderControl.SetGridColumn(bandElement, band);
#endif
				} else {
					bandElement = CreateBandElement(band);
					Children.Add(bandElement);
				}
				bandsDictionary[band] = bandElement;
			}
			bandElement.Measure(new Size(GetBandWidth(band), double.PositiveInfinity));
			double height = 0;
			foreach(BandBase subBand in band.VisibleBands) {
				height = Math.Max(height, MeasureBand(subBand, currentWidth));
				currentWidth += subBand.ActualHeaderWidth;
			}
			return bandElement.DesiredSize.Height + height;
		}
		protected abstract FrameworkElement CreateBandElement(BandBase band);
		protected override Size ArrangeOverride(Size finalSize) {
			if(Bands == null)
				return finalSize;
			double width = 0;
			foreach(BandBase band in Bands) {
				ArrangeBand(band, new Point(width, 0), finalSize.Height);
				width += GetBandWidth(band);
			}
			return finalSize;
		}
		void ArrangeBand(BandBase band, Point offset, double height) {
			if(ActualAllowVirtualization) {
				if(offset.X > TableView.TableViewBehavior.HorizontalOffset + TableView.HorizontalViewport + TableView.TotalGroupAreaIndent) return;
				if(offset.X + GetBandWidth(band) <= TableView.TableViewBehavior.HorizontalOffset) return;
			}
			FrameworkElement bandElement = bandsDictionary[band];
			double width = 0;
			foreach(BandBase subBand in band.VisibleBands) {
				ArrangeBand(subBand, new Point(offset.X + width, offset.Y + bandElement.DesiredSize.Height), height - bandElement.DesiredSize.Height);
				width += GetBandWidth(subBand);
			}
			bandElement.Arrange(new Rect(offset.X, offset.Y, GetBandWidth(band), band.VisibleBands.Count > 0 ? bandElement.DesiredSize.Height : height));
		}
		protected virtual double GetBandWidth(BandBase band) {
			return band.ActualHeaderWidth;
		}
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeBands(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
	}
	public class BandsPanel : BandsPanelBase, ILayoutNotificationHelperOwner {
		LayoutNotificationHelper notificationHelper;
		public BandsPanel() {
			notificationHelper = new LayoutNotificationHelper(this);
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(DataControl != null)
				notificationHelper.Subscribe();
			return base.MeasureOverride(availableSize);
		}
		protected override FrameworkElement CreateBandElement(BandBase band) {
			BandHeaderControl bandElement = new BandHeaderControl() { DataContext = band, CanSyncColumnPosition = true };
#if !SL
			BandHeaderControl.SetGridColumn(bandElement, band);
#endif
			BarManager.SetDXContextMenu(bandElement, band.View.DataControlMenu);
			GridPopupMenu.SetGridMenuType(bandElement, GridMenuType.Band);
			GridColumn.SetHeaderPresenterType(bandElement, HeaderPresenterType.Headers);
			GridViewHitInfoBase.SetHitTestAcceptor(bandElement, new DevExpress.Xpf.Grid.HitTest.BandHeaderTableViewHitTestAcceptor());
			return bandElement;
		}
		DependencyObject ILayoutNotificationHelperOwner.NotificationManager {
			get { return DataControl as DependencyObject; }
		}
		DataControlBase DataControl { get { return DataView.Return(x => x.DataControl, null); } }
	}
}

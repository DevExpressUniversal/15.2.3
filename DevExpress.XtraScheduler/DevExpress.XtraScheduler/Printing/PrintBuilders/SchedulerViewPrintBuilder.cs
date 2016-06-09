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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing.Native {
	abstract class SchedulerViewPrintBuilder : PrintViewInfoBuilder {
		TimeInterval currentInterval;
		ViewPart currentPart;
		int currentResourceIndex;
		int resourcesOnPage;
		ResourceBaseCollection printResourceCollection;
		public SchedulerViewPrintBuilder(PrintStyleWithResourceOptions printStyle, SchedulerControl control, GraphicsInfo gInfo)
			: base(printStyle, control, gInfo) {
		}
		protected internal new PrintStyleWithResourceOptions PrintStyle { get { return (PrintStyleWithResourceOptions)base.PrintStyle; } }
		protected internal TimeInterval CurrentInterval { get { return currentInterval; } set { currentInterval = value; } }
		protected internal int CurrentResourceIndex { get { return currentResourceIndex; } set { currentResourceIndex = value; } }
		protected internal ViewPart CurrentPart { get { return currentPart; } set { currentPart = value; } }
		protected internal ResourceBaseCollection PrintResourceCollection { get { return printResourceCollection; } set { printResourceCollection = value; } }
		protected internal int ResourcesOnPage { get { return resourcesOnPage; } set { resourcesOnPage = value; } }
		protected abstract PageLayout Layout { get; }
		protected internal abstract ViewPart CalculateFirstViewPart();
		protected internal abstract TimeInterval CalculateFirstInterval(DateTime startRangeDate, DateTime endRangeDate);
		protected abstract void CreateSingleIntervalViewInfo(CompositeViewInfo printViewInfo, Rectangle currentPageBounds);
		public override IPrintableObjectViewInfo CreateViewInfo(Rectangle pageBounds) {
			CompositeViewInfo printViewInfo = new CompositeViewInfo();
			Rectangle currentPageBounds = pageBounds;
			while (MoveNext()) {
				CreateSingleIntervalViewInfo(printViewInfo, currentPageBounds);
				currentPageBounds.Y = currentPageBounds.Bottom;
			}
			return printViewInfo;
		}
		protected internal virtual bool MoveNext() {
			if (currentInterval == null) {
				Reset();
				return true;
			}
			bool mustSwitchToNextResources = SwitchToNextViewPart();
			if (!mustSwitchToNextResources)
				return true;
			bool mustSwitchToNextInterval = SwitchToNextResources();
			if (!mustSwitchToNextInterval)
				return true;
			currentInterval = CalculateNextInterval(currentInterval);
			return currentInterval.Start <= PrintStyle.EndRangeDate;
		}
		protected internal virtual void Reset() {
			currentPart = CalculateFirstViewPart();
			currentInterval = CalculateFirstInterval(PrintStyle.StartRangeDate, PrintStyle.EndRangeDate);
			ResetResources();
		}
		protected internal virtual bool SwitchToNextViewPart() {
			if (currentPart == ViewPart.Left) {
				currentPart = ViewPart.Right;
				return false;
			} else if (currentPart == ViewPart.Right)
				currentPart = ViewPart.Left;
			return true;
		}
		protected internal virtual void ResetResources() {
			printResourceCollection = new ResourceBaseCollection();
			printResourceCollection.AddRange(GetResources());
			if (PrintStyle.ResourceOptions.PrintAllResourcesOnOnePage)
				resourcesOnPage = printResourceCollection.Count;
			else
				resourcesOnPage = PrintStyle.ResourceOptions.ResourcesPerPage;
			currentResourceIndex = 0;
		}
		protected internal virtual ResourceBaseCollection GetResources() {
			ResourceOptions resourceOptions = PrintStyle.ResourceOptions;
			if (resourceOptions.PrintCustomCollection)
				return resourceOptions.CustomResourcesCollection;
			switch (resourceOptions.ResourcesKind) {
				case ResourcesKind.All:
					return ((IInternalSchedulerStorageBase)Control.DataStorage).GetFilteredResources(true);
				case ResourcesKind.Visible:
					return Control.InnerControl.GetFilteredResources();
				case ResourcesKind.OnScreen:
					return Control.ActiveView.VisibleResources;
				default:
					return new ResourceBaseCollection();
			}
		}
		protected internal virtual bool SwitchToNextResources() {
			currentResourceIndex += resourcesOnPage;
			if (currentResourceIndex < printResourceCollection.Count)
				return false;
			currentResourceIndex = 0;
			return true;
		}
		protected internal virtual ResourceBaseCollection GetPrintedResources() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			int lastResourceIndex = Math.Min(currentResourceIndex + resourcesOnPage, printResourceCollection.Count);
			for (int i = currentResourceIndex; i < lastResourceIndex; i++)
				result.Add(printResourceCollection[i]);
			return result;
		}
		protected internal abstract TimeInterval CalculateNextInterval(TimeInterval currentInterval);
	}
}

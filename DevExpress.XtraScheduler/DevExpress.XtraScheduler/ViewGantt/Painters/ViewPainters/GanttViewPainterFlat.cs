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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class GanttViewPainterFlat : GanttViewPainter {
		protected internal override GanttDependenciesPainter CreateDependencyPainter() {
			return new GanttDependenciesFlatPainter();
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderFlatPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalFlatPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return new NavigationButtonFlatPainter();
		}
		protected internal override AppearanceDefaultInfo[] GetDefaultAppearances() {
			return new AppearanceDefaultInfo[] {
												   new AppearanceDefaultInfo(GanttViewAppearanceNames.Dependency, new AppearanceDefault(Color.Gray, Color.Transparent)),
												   new AppearanceDefaultInfo(GanttViewAppearanceNames.SelectedDependency, new AppearanceDefault(Color.Black, Color.Transparent)),
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateAlternateHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionAppearance(HorzAlignment.Center),
												   HeaderAppearanceFlatHelper.CreateResourceHeaderCaptionLineAppearance(),
												   HeaderAppearanceFlatHelper.CreateSelectionAppearance(HorzAlignment.Center),
												   AppointmentDefaultAppearancesHelper.CreateAppointmentAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonAppearance(),
												   NavigationButtonFlatAppearancesHelper.CreateNavigationButtonDisabledAppearance()
											   };
		}
		protected override TimeIndicatorPainter CreateTimeIndicatorPainter() {
			return new TimeIndicatorFlatPainter();
		}
	}
}

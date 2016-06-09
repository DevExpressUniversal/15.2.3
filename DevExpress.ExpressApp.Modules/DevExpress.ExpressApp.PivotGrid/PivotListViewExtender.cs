#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.Persistent.Base;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.XtraPivotGrid;
namespace DevExpress.ExpressApp.PivotGrid {
	public interface IModelPivotListView {
		IModelPivotSettings PivotSettings { get; }
	}
	public interface IPivotSettings {
		[Category("Appearance")]
		[Editor("DevExpress.ExpressApp.PivotGrid.Win.PivotGridSettingsEditor, DevExpress.ExpressApp.PivotGrid.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		[ Localizable(true)]
		string Settings { get; set; }
		[Category("Appearance")]
		[DefaultValue(false)]
		bool ShowChart { get; set; }
		[Category("Appearance")]
		[DefaultValue(true)]
		bool ChartDataVertical { get; set; }
		[Category("Appearance")]
		[DefaultValue(false)]
		bool ShowRowTotals { get; set; }
		[Category("Appearance")]
		[DefaultValue(false)]
		bool ShowRowGrandTotals { get; set; }
		[Category("Appearance")]
		[DefaultValue(false)]
		bool ShowColumnTotals { get; set; }
		[Category("Appearance")]
		[DefaultValue(false)]
		bool ShowColumnGrandTotals { get; set; }
		[Category("Appearance")]
		[Browsable(false)]
		[ Localizable(true)]
		string LayoutSettings { get; set; }
		[Category("Appearance")]
		[Browsable(false)]
		[ Localizable(true)]
		string ChartSettings { get; set; }
		[Category("Behavior")]
		[DefaultValue(true)]
		bool CustomizationEnabled { get; set; }
		[Category("Diffs Behavior")]
		[DefaultValue(true)]
		bool AddNewColumns { get; set; }
		[Category("Diffs Behavior")]
		[DefaultValue(false)]
		bool RemoveOldColumns { get; set; }
	}
#if !SL
	[DevExpressExpressAppPivotGridLocalizedDescription("PivotGridIModelPivotSettings")]
#endif
	public interface IModelPivotSettings : IModelNode, IPivotSettings {
	}
}

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

using System.ComponentModel;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Win.SystemModule;
namespace DevExpress.ExpressApp.Win.Model {
	[ModelAbstractClass()]
	[ModelVirtualTreeDisplayPropertiesAttribute("AutoFillDown")]
	public interface IModelBandWin : IModelBand {
		[Category("Layout")]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandWinAutoFillDown")]
#endif
		[DefaultValue(true)]
		bool AutoFillDown { get; set; }
	}
	[ModelAbstractClass()]
	[ModelVirtualTreeDisplayPropertiesAttribute("RowIndex", "BandColumnRowCount")]
	public interface IModelBandedColumnWin : IModelColumnWin, IModelBandedLayoutItem {
		[Category("Layout")]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandedColumnWinRowIndex")]
#endif
		[Browsable(false)]
		int RowIndex { get; set; }
		[DefaultValue(1)]
		[Category("Layout")]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandedColumnWinBandColumnRowCount")]
#endif
		[ModelPersistentName("BandColumnRowCount")]
		[Browsable(false)]
		int BandColumnRowCount { get; set; }
	}
	[ModelAbstractClass()]
	public interface IModelBandsLayoutWin : IModelBandsLayout {
		[Category("OptionsCustomization")]
		[DefaultValue(false)]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandsLayoutWinAllowChangeBandParent")]
#endif
		[ModelBrowsable(typeof(ModelBandsLayoutPropertyVisibilityCalculator))]
		bool AllowChangeBandParent { get; set; }
		[Category("OptionsCustomization")]
		[DefaultValue(false)]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandsLayoutWinAllowChangeColumnParent")]
#endif
		[ModelBrowsable(typeof(ModelBandsLayoutPropertyVisibilityCalculator))]
		bool AllowChangeColumnParent { get; set; }
		[Category("OptionsCustomization")]
		[DefaultValue(true)]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandsLayoutWinAllowColumnMoving")]
#endif
		[ModelBrowsable(typeof(ModelBandsLayoutPropertyVisibilityCalculator))]
		bool AllowColumnMoving { get; set; }
		[Category("OptionsCustomization")]
		[DefaultValue(true)]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandsLayoutWinAllowBandMoving")]
#endif
		[ModelBrowsable(typeof(ModelBandsLayoutPropertyVisibilityCalculator))]
		bool AllowBandMoving { get; set; }
		[Category("OptionsView")]
		[DefaultValue(true)]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandsLayoutWinShowBands")]
#endif
		[ModelBrowsable(typeof(ModelBandsLayoutPropertyVisibilityCalculator))]
		bool ShowBands { get; set; }
		[DefaultValue(true)]
		[Category("OptionsView")]
#if !SL
	[DevExpressExpressAppWinLocalizedDescription("IModelBandsLayoutWinShowColumnHeaders")]
#endif
		[ModelBrowsable(typeof(ModelBandsLayoutPropertyVisibilityCalculator))]
		bool ShowColumnHeaders { get; set; }
		[Category("Layout")]
		[DefaultValue(-1)]
		int BandPanelRowHeight { get; set; }
		[Category("Layout")]
		[DefaultValue(1)]
		int MinBandPanelRowCount { get; set; }
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ModelBandsLayoutPropertyVisibilityCalculator : IModelIsVisible {
		public bool IsVisible(IModelNode node, string propertyName) {
			IModelBandsLayout bandsModel = node as IModelBandsLayout;
			return propertyName == "Enable" || bandsModel.Enable;
		}
	}
}

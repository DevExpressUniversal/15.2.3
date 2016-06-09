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

using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class CircularGaugeModel : GaugeModelBase {
		public static readonly DependencyProperty ModelFullProperty = DependencyPropertyManager.Register("ModelFull",
			typeof(PartialCircularGaugeModel), typeof(CircularGaugeModel), new PropertyMetadata(null));
		public static readonly DependencyProperty ModelHalfTopProperty = DependencyPropertyManager.Register("ModelHalfTop",
			typeof(PartialCircularGaugeModel), typeof(CircularGaugeModel));
		public static readonly DependencyProperty ModelQuarterTopLeftProperty = DependencyPropertyManager.Register("ModelQuarterTopLeft",
			typeof(PartialCircularGaugeModel), typeof(CircularGaugeModel));
		public static readonly DependencyProperty ModelQuarterTopRightProperty = DependencyPropertyManager.Register("ModelQuarterTopRight",
			typeof(PartialCircularGaugeModel), typeof(CircularGaugeModel));
		public static readonly DependencyProperty ModelThreeQuartersProperty = DependencyPropertyManager.Register("ModelThreeQuarters",
			typeof(PartialCircularGaugeModel), typeof(CircularGaugeModel));
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PartialCircularGaugeModel ModelFull {
			get { return (PartialCircularGaugeModel)GetValue(ModelFullProperty); }
			set { SetValue(ModelFullProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PartialCircularGaugeModel ModelHalfTop {
			get { return (PartialCircularGaugeModel)GetValue(ModelHalfTopProperty); }
			set { SetValue(ModelHalfTopProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PartialCircularGaugeModel ModelQuarterTopLeft {
			get { return (PartialCircularGaugeModel)GetValue(ModelQuarterTopLeftProperty); }
			set { SetValue(ModelQuarterTopLeftProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PartialCircularGaugeModel ModelQuarterTopRight {
			get { return (PartialCircularGaugeModel)GetValue(ModelQuarterTopRightProperty); }
			set { SetValue(ModelQuarterTopRightProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PartialCircularGaugeModel ModelThreeQuarters {
			get { return (PartialCircularGaugeModel)GetValue(ModelThreeQuartersProperty); }
			set { SetValue(ModelThreeQuartersProperty, value); }
		}
		internal ArcScaleModel GetScaleModel(ArcScaleLayoutMode scaleLayoutMode, int index) {
			switch (scaleLayoutMode) {
				case ArcScaleLayoutMode.HalfTop:
					return ModelHalfTop.GetScaleModel(index);
				case ArcScaleLayoutMode.QuarterTopLeft:
					return ModelQuarterTopLeft.GetScaleModel(index);
				case ArcScaleLayoutMode.QuarterTopRight:
					return ModelQuarterTopRight.GetScaleModel(index);
				case ArcScaleLayoutMode.ThreeQuarters:
					return ModelThreeQuarters.GetScaleModel(index);
				default:
					return ModelFull.GetScaleModel(index);
			}
		}
		internal ArcScaleNeedleModel GetNeedleModel(ArcScaleLayoutMode scaleLayoutMode, int index) {
			switch (scaleLayoutMode) {
				case ArcScaleLayoutMode.HalfTop:
					return ModelHalfTop.GetNeedleModel(index);
				case ArcScaleLayoutMode.QuarterTopLeft:
					return ModelQuarterTopLeft.GetNeedleModel(index);
				case ArcScaleLayoutMode.QuarterTopRight:
					return ModelQuarterTopRight.GetNeedleModel(index);
				case ArcScaleLayoutMode.ThreeQuarters:
					return ModelThreeQuarters.GetNeedleModel(index);
				default:
					return ModelFull.GetNeedleModel(index);
			}
		}
		internal ArcScaleMarkerModel GetMarkerModel(int index) {
			return ModelFull.GetMarkerModel(index);
		}
		internal ArcScaleRangeBarModel GetRangeBarModel(int index) {
			return ModelFull.GetRangeBarModel(index);
		}
		internal ArcScaleRangeModel GetRangeModel(int index) {
			return ModelFull.GetRangeModel(index);
		}
		internal LayerModel GetLayerModel(int index) {
			return ModelFull.GetLayerModel(index);
		}
		internal LayerModel GetScaleLayerModel(ArcScaleLayoutMode scaleLayoutMode, int index) {
			switch (scaleLayoutMode) {
				case ArcScaleLayoutMode.HalfTop:
					return ModelHalfTop.GetScaleLayerModel(index);
				case ArcScaleLayoutMode.QuarterTopLeft:
					return ModelQuarterTopLeft.GetScaleLayerModel(index);
				case ArcScaleLayoutMode.QuarterTopRight:
					return ModelQuarterTopRight.GetScaleLayerModel(index);
				case ArcScaleLayoutMode.ThreeQuarters:
					return ModelThreeQuarters.GetScaleLayerModel(index);
				default:
					return ModelFull.GetScaleLayerModel(index);
			}
		}
		protected override void OwnerChanged() {
			base.OwnerChanged();
			IOwnedElement model = ModelFull as IOwnedElement;
			if (model != null)
				model.Owner = Owner;
			model = ModelHalfTop as IOwnedElement;
			if (model != null)
				model.Owner = Owner;
			model = ModelQuarterTopLeft as IOwnedElement;
			if (model != null)
				model.Owner = Owner;
			model = ModelQuarterTopRight as IOwnedElement;
			if (model != null)
				model.Owner = Owner;
			model = ModelThreeQuarters as IOwnedElement;
			if (model != null)
				model.Owner = Owner;
		}
	}
	public class CircularDefaultModel : CircularGaugeModel {
		public override string ModelName { get { return "Default"; } }
		public CircularDefaultModel() {
			ModelFull = new CircularDefaultFullModel();
			ModelHalfTop = new CircularDefaultHalfTopModel();
			ModelQuarterTopLeft = new CircularDefaultQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularDefaultQuarterTopRightModel();
			ModelThreeQuarters = new CircularDefaultThreeQuartersModel();
		}
	}
	public class CircularCleanWhiteModel : CircularGaugeModel {
		public override string ModelName { get { return "Clean White"; } }
		public CircularCleanWhiteModel() {
			ModelFull = new CircularCleanWhiteFullModel();
			ModelHalfTop = new CircularCleanWhiteHalfTopModel();
			ModelQuarterTopLeft = new CircularCleanWhiteQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularCleanWhiteQuarterTopRightModel();
			ModelThreeQuarters = new CircularCleanWhiteThreeQuartersModel();
		}
	}
	public class CircularCosmicModel : CircularGaugeModel {
		public override string ModelName { get { return "Cosmic"; } }
		public CircularCosmicModel() {
			ModelFull = new CircularCosmicFullModel();
			ModelHalfTop = new CircularCosmicHalfTopModel();
			ModelQuarterTopLeft = new CircularCosmicQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularCosmicQuarterTopRightModel();
			ModelThreeQuarters = new CircularCosmicThreeQuartersModel();
		}
	}
	public class CircularSmartModel : CircularGaugeModel {
		public override string ModelName { get { return "Smart"; } }
		public CircularSmartModel() {
			ModelFull = new CircularSmartFullModel();
			ModelHalfTop = new CircularSmartHalfTopModel();
			ModelQuarterTopLeft = new CircularSmartQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularSmartQuarterTopRightModel();
			ModelThreeQuarters = new CircularSmartThreeQuartersModel();
		}
	}
	public class CircularRedClockModel : CircularGaugeModel {
		public override string ModelName { get { return "Red Clock"; } }
		public CircularRedClockModel() {
			InnerPadding = new Thickness(0, 7, 0, 4);
			ModelFull = new CircularRedClockFullModel();
			ModelHalfTop = new CircularRedClockHalfTopModel();
			ModelQuarterTopLeft = new CircularRedClockQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularRedClockQuarterTopRightModel();
			ModelThreeQuarters = new CircularRedClockThreeQuartersModel();
		}
	}
	public class CircularProgressiveModel : CircularGaugeModel {
		public override string ModelName { get { return "Progressive"; } }
		public CircularProgressiveModel() {
			ModelFull = new CircularProgressiveFullModel();
			ModelHalfTop = new CircularProgressiveHalfTopModel();
			ModelQuarterTopLeft = new CircularProgressiveQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularProgressiveQuarterTopRightModel();
			ModelThreeQuarters = new CircularProgressiveThreeQuartersModel();
		}
	}
	public class CircularEcoModel : CircularGaugeModel {
		public override string ModelName { get { return "Eco"; } }
		public CircularEcoModel() {
			ModelFull = new CircularEcoFullModel();
			ModelHalfTop = new CircularEcoHalfTopModel();
			ModelQuarterTopLeft = new CircularEcoQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularEcoQuarterTopRightModel();
			ModelThreeQuarters = new CircularEcoThreeQuartersModel();
		}
	}
	public class CircularFutureModel : CircularGaugeModel {
		public override string ModelName { get { return "Future"; } }
		public CircularFutureModel() {
			ModelFull = new CircularFutureFullModel();
			ModelHalfTop = new CircularFutureHalfTopModel();
			ModelQuarterTopLeft = new CircularFutureQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularFutureQuarterTopRightModel();
			ModelThreeQuarters = new CircularFutureThreeQuartersModel();
		}
	}
	public class CircularClassicModel : CircularGaugeModel {
		public override string ModelName { get { return "Classic"; } }
		public CircularClassicModel() {
			ModelFull = new CircularClassicFullModel();
			ModelHalfTop = new CircularClassicHalfTopModel();
			ModelQuarterTopLeft = new CircularClassicQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularClassicQuarterTopRightModel();
			ModelThreeQuarters = new CircularClassicThreeQuartersModel();
		}
	}
	public class CircularIStyleModel : CircularGaugeModel {
		public override string ModelName { get { return "iStyle"; } }
		public CircularIStyleModel() {
			ModelFull = new CircularIStyleFullModel();
			ModelHalfTop = new CircularIStyleHalfTopModel();
			ModelQuarterTopLeft = new CircularIStyleQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularIStyleQuarterTopRightModel();
			ModelThreeQuarters = new CircularIStyleThreeQuartersModel();
		}
	}
	public class CircularYellowSubmarineModel : CircularGaugeModel {
		public override string ModelName { get { return "Yellow Submarine"; } }
		public CircularYellowSubmarineModel() {
			ModelFull = new CircularYellowSubmarineFullModel();
			ModelHalfTop = new CircularYellowSubmarineHalfTopModel();
			ModelQuarterTopLeft = new CircularYellowSubmarineQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularYellowSubmarineQuarterTopRightModel();
			ModelThreeQuarters = new CircularYellowSubmarineThreeQuartersModel();
		}
	}
	public class CircularMagicLightModel : CircularGaugeModel {
		public override string ModelName { get { return "Magic Light"; } }
		public CircularMagicLightModel() {
			ModelFull = new CircularMagicLightFullModel();
			ModelHalfTop = new CircularMagicLightHalfTopModel();
			ModelQuarterTopLeft = new CircularMagicLightQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularMagicLightQuarterTopRightModel();
			ModelThreeQuarters = new CircularMagicLightThreeQuartersModel();
		}
	}
	public class CircularFlatLightModel : CircularGaugeModel {
		public override string ModelName { get { return "Flat Light"; } }
		public CircularFlatLightModel() {
			ModelFull = new CircularFlatLightFullModel();
			ModelHalfTop = new CircularFlatLightHalfTopModel();
			ModelQuarterTopLeft = new CircularFlatLightQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularFlatLightQuarterTopRightModel();
			ModelThreeQuarters = new CircularFlatLightThreeQuartersModel();
		}
	}
	public class CircularFlatDarkModel : CircularGaugeModel {
		public override string ModelName { get { return "Flat Dark"; } }
		public CircularFlatDarkModel() {
			ModelFull = new CircularFlatDarkFullModel();
			ModelHalfTop = new CircularFlatDarkHalfTopModel();
			ModelQuarterTopLeft = new CircularFlatDarkQuarterTopLeftModel();
			ModelQuarterTopRight = new CircularFlatDarkQuarterTopRightModel();
			ModelThreeQuarters = new CircularFlatDarkThreeQuartersModel();
		}
	}
}

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
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class PartialCircularGaugeModel : GaugeModelBase {
		public static readonly DependencyProperty ScaleModelsProperty = DependencyPropertyManager.Register("ScaleModels",
			typeof(ArcScaleModelCollection), typeof(PartialCircularGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty NeedleModelsProperty = DependencyPropertyManager.Register("NeedleModels",
			typeof(ArcScaleNeedleModelCollection), typeof(PartialCircularGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty MarkerModelsProperty = DependencyPropertyManager.Register("MarkerModels",
			typeof(ArcScaleMarkerModelCollection), typeof(PartialCircularGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty RangeBarModelsProperty = DependencyPropertyManager.Register("RangeBarModels",
			typeof(ArcScaleRangeBarModelCollection), typeof(PartialCircularGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty RangeModelsProperty = DependencyPropertyManager.Register("RangeModels",
			typeof(ArcScaleRangeModelCollection), typeof(PartialCircularGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty LayerModelsProperty = DependencyPropertyManager.Register("LayerModels",
			typeof(LayerModelCollection), typeof(PartialCircularGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty ScaleLayerModelsProperty = DependencyPropertyManager.Register("ScaleLayerModels",
			typeof(LayerModelCollection), typeof(PartialCircularGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ArcScaleModelCollection ScaleModels {
			get { return (ArcScaleModelCollection)GetValue(ScaleModelsProperty); }
			set { SetValue(ScaleModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ArcScaleNeedleModelCollection NeedleModels {
			get { return (ArcScaleNeedleModelCollection)GetValue(NeedleModelsProperty); }
			set { SetValue(NeedleModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ArcScaleMarkerModelCollection MarkerModels {
			get { return (ArcScaleMarkerModelCollection)GetValue(MarkerModelsProperty); }
			set { SetValue(MarkerModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ArcScaleRangeBarModelCollection RangeBarModels {
			get { return (ArcScaleRangeBarModelCollection)GetValue(RangeBarModelsProperty); }
			set { SetValue(RangeBarModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ArcScaleRangeModelCollection RangeModels {
			get { return (ArcScaleRangeModelCollection)GetValue(RangeModelsProperty); }
			set { SetValue(RangeModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public LayerModelCollection LayerModels {
			get { return (LayerModelCollection)GetValue(LayerModelsProperty); }
			set { SetValue(LayerModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public LayerModelCollection ScaleLayerModels {
			get { return (LayerModelCollection)GetValue(ScaleLayerModelsProperty); }
			set { SetValue(ScaleLayerModelsProperty, value); }
		}
		internal ArcScaleModel GetScaleModel(int index) {
			return GetModel(ScaleModelsProperty, index) as ArcScaleModel;
		}
		internal ArcScaleNeedleModel GetNeedleModel(int index) {
			return GetModel(NeedleModelsProperty, index) as ArcScaleNeedleModel;
		}
		internal ArcScaleMarkerModel GetMarkerModel(int index) {
			return GetModel(MarkerModelsProperty, index) as ArcScaleMarkerModel;
		}
		internal ArcScaleRangeBarModel GetRangeBarModel(int index) {
			return GetModel(RangeBarModelsProperty, index) as ArcScaleRangeBarModel;
		}
		internal ArcScaleRangeModel GetRangeModel(int index) {
			return GetModel(RangeModelsProperty, index) as ArcScaleRangeModel;
		}
		internal LayerModel GetLayerModel(int index) {
			return GetModel(LayerModelsProperty, index) as LayerModel;
		}
		internal LayerModel GetScaleLayerModel(int index) {
			return GetModel(ScaleLayerModelsProperty, index) as LayerModel;
		}
	}
	public class CircularDefaultFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Default Full"; } }
		public CircularDefaultFullModel() {
			DefaultStyleKey = typeof(CircularDefaultFullModel);
		}
	}
	public class CircularDefaultHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Default Half Top"; } }
		public CircularDefaultHalfTopModel() {
			DefaultStyleKey = typeof(CircularDefaultHalfTopModel);
		}
	}
	public class CircularDefaultQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Default Quarter Top Left"; } }
		public CircularDefaultQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularDefaultQuarterTopLeftModel);
		}
	}
	public class CircularDefaultQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Default Quarter Top Right"; } }
		public CircularDefaultQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularDefaultQuarterTopRightModel);
		}
	}
	public class CircularDefaultThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Default Three Quarters"; } }
		public CircularDefaultThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularDefaultThreeQuartersModel);
		}
	}
	public class CircularCleanWhiteFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Clean White Full"; } }
		public CircularCleanWhiteFullModel() {
			DefaultStyleKey = typeof(CircularCleanWhiteFullModel);
		}
	}
	public class CircularCleanWhiteHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Clean White Half Top"; } }
		public CircularCleanWhiteHalfTopModel() {
			DefaultStyleKey = typeof(CircularCleanWhiteHalfTopModel);
		}
	}
	public class CircularCleanWhiteQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Clean White Quarter Top Left"; } }
		public CircularCleanWhiteQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularCleanWhiteQuarterTopLeftModel);
		}
	}
	public class CircularCleanWhiteQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Clean White Quarter Top Right"; } }
		public CircularCleanWhiteQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularCleanWhiteQuarterTopRightModel);
		}
	}
	public class CircularCleanWhiteThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Clean White Three Quarters"; } }
		public CircularCleanWhiteThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularCleanWhiteThreeQuartersModel);
		}
	}
	public class CircularCosmicFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Cosmic Full"; } }
		public CircularCosmicFullModel() {
			DefaultStyleKey = typeof(CircularCosmicFullModel);
		}
	}
	public class CircularCosmicHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Cosmic Half Top"; } }
		public CircularCosmicHalfTopModel() {
			DefaultStyleKey = typeof(CircularCosmicHalfTopModel);
		}
	}
	public class CircularCosmicQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Cosmic Quarter Top Left"; } }
		public CircularCosmicQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularCosmicQuarterTopLeftModel);
		}
	}
	public class CircularCosmicQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Cosmic Quarter Top Right"; } }
		public CircularCosmicQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularCosmicQuarterTopRightModel);
		}
	}
	public class CircularCosmicThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Cosmic Three Quarters"; } }
		public CircularCosmicThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularCosmicThreeQuartersModel);
		}
	}
	public class CircularSmartFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Smart Full"; } }
		public CircularSmartFullModel() {
			DefaultStyleKey = typeof(CircularSmartFullModel);
		}
	}
	public class CircularSmartHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Smart Half Top"; } }
		public CircularSmartHalfTopModel() {
			DefaultStyleKey = typeof(CircularSmartHalfTopModel);
		}
	}
	public class CircularSmartQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Smart Quarter Top Left"; } }
		public CircularSmartQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularSmartQuarterTopLeftModel);
		}
	}
	public class CircularSmartQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Smart Quarter Top Right"; } }
		public CircularSmartQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularSmartQuarterTopRightModel);
		}
	}
	public class CircularSmartThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Smart Three Quarters"; } }
		public CircularSmartThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularSmartThreeQuartersModel);
		}
	}
	public class CircularRedClockFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Red Clock Full"; } }
		public CircularRedClockFullModel() {
			DefaultStyleKey = typeof(CircularRedClockFullModel);
		}
	}
	public class CircularRedClockHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Red Clock Half Top"; } }
		public CircularRedClockHalfTopModel() {
			DefaultStyleKey = typeof(CircularRedClockHalfTopModel);
		}
	}
	public class CircularRedClockQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Red Clock Quarter Top Left"; } }
		public CircularRedClockQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularRedClockQuarterTopLeftModel);
		}
	}
	public class CircularRedClockQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Red Clock Quarter Top Right"; } }
		public CircularRedClockQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularRedClockQuarterTopRightModel);
		}
	}
	public class CircularRedClockThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Red Clock Three Quarters"; } }
		public CircularRedClockThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularRedClockThreeQuartersModel);
		}
	}
	public class CircularProgressiveFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Progressive Full"; } }
		public CircularProgressiveFullModel() {
			DefaultStyleKey = typeof(CircularProgressiveFullModel);
		}
	}
	public class CircularProgressiveHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Progressive Half Top"; } }
		public CircularProgressiveHalfTopModel() {
			DefaultStyleKey = typeof(CircularProgressiveHalfTopModel);
		}
	}
	public class CircularProgressiveQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Progressive Quarter Top Left"; } }
		public CircularProgressiveQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularProgressiveQuarterTopLeftModel);
		}
	}
	public class CircularProgressiveQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Progressive Quarter Top Right"; } }
		public CircularProgressiveQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularProgressiveQuarterTopRightModel);
		}
	}
	public class CircularProgressiveThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Progressive Three Quarters"; } }
		public CircularProgressiveThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularProgressiveThreeQuartersModel);
		}
	}
	public class CircularEcoFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Eco Full"; } }
		public CircularEcoFullModel() {
			DefaultStyleKey = typeof(CircularEcoFullModel);
		}
	}
	public class CircularEcoHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Eco Half Top"; } }
		public CircularEcoHalfTopModel() {
			DefaultStyleKey = typeof(CircularEcoHalfTopModel);
		}
	}
	public class CircularEcoQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Eco Quarter Top Left"; } }
		public CircularEcoQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularEcoQuarterTopLeftModel);
		}
	}
	public class CircularEcoQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Eco Quarter Top Right"; } }
		public CircularEcoQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularEcoQuarterTopRightModel);
		}
	}
	public class CircularEcoThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Eco Three Quarters"; } }
		public CircularEcoThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularEcoThreeQuartersModel);
		}
	}
	public class CircularFutureFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Future Full"; } }
		public CircularFutureFullModel() {
			DefaultStyleKey = typeof(CircularFutureFullModel);
		}
	}
	public class CircularFutureHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Future Half Top"; } }
		public CircularFutureHalfTopModel() {
			DefaultStyleKey = typeof(CircularFutureHalfTopModel);
		}
	}
	public class CircularFutureQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Future Quarter Top Left"; } }
		public CircularFutureQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularFutureQuarterTopLeftModel);
		}
	}
	public class CircularFutureQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Future Quarter Top Right"; } }
		public CircularFutureQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularFutureQuarterTopRightModel);
		}
	}
	public class CircularFutureThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Future Three Quarters"; } }
		public CircularFutureThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularFutureThreeQuartersModel);
		}
	}
	public class CircularClassicFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Classic Full"; } }
		public CircularClassicFullModel() {
			DefaultStyleKey = typeof(CircularClassicFullModel);
		}
	}
	public class CircularClassicHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Classic Half Top"; } }
		public CircularClassicHalfTopModel() {
			DefaultStyleKey = typeof(CircularClassicHalfTopModel);
		}
	}
	public class CircularClassicQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Classic Quarter Top Left"; } }
		public CircularClassicQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularClassicQuarterTopLeftModel);
		}
	}
	public class CircularClassicQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Classic Quarter Top Right"; } }
		public CircularClassicQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularClassicQuarterTopRightModel);
		}
	}
	public class CircularClassicThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Classic Three Quarters"; } }
		public CircularClassicThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularClassicThreeQuartersModel);
		}
	}
	public class CircularIStyleFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "iStyle Full"; } }
		public CircularIStyleFullModel() {
			DefaultStyleKey = typeof(CircularIStyleFullModel);
		}
	}
	public class CircularIStyleHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "IStyle Half Top"; } }
		public CircularIStyleHalfTopModel() {
			DefaultStyleKey = typeof(CircularIStyleHalfTopModel);
		}
	}
	public class CircularIStyleQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "IStyle Quarter Top Left"; } }
		public CircularIStyleQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularIStyleQuarterTopLeftModel);
		}
	}
	public class CircularIStyleQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "IStyle Quarter Top Right"; } }
		public CircularIStyleQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularIStyleQuarterTopRightModel);
		}
	}
	public class CircularIStyleThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "IStyle Three Quarters"; } }
		public CircularIStyleThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularIStyleThreeQuartersModel);
		}
	}
	public class CircularYellowSubmarineFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Yellow Submarine Full"; } }
		public CircularYellowSubmarineFullModel() {
			DefaultStyleKey = typeof(CircularYellowSubmarineFullModel);
		}
	}
	public class CircularYellowSubmarineHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Yellow Submarine Half Top"; } }
		public CircularYellowSubmarineHalfTopModel() {
			DefaultStyleKey = typeof(CircularYellowSubmarineHalfTopModel);
		}
	}
	public class CircularYellowSubmarineQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Yellow Submarine Quarter Top Left"; } }
		public CircularYellowSubmarineQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularYellowSubmarineQuarterTopLeftModel);
		}
	}
	public class CircularYellowSubmarineQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Yellow Submarine Quarter Top Right"; } }
		public CircularYellowSubmarineQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularYellowSubmarineQuarterTopRightModel);
		}
	}
	public class CircularYellowSubmarineThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Yellow Submarine Three Quarters"; } }
		public CircularYellowSubmarineThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularYellowSubmarineThreeQuartersModel);
		}
	}
	public class CircularMagicLightFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Magic Light Full"; } }
		public CircularMagicLightFullModel() {
			DefaultStyleKey = typeof(CircularMagicLightFullModel);
		}
	}
	public class CircularMagicLightHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Magic Light Half Top"; } }
		public CircularMagicLightHalfTopModel() {
			DefaultStyleKey = typeof(CircularMagicLightHalfTopModel);
		}
	}
	public class CircularMagicLightQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Magic Light Quarter Top Left"; } }
		public CircularMagicLightQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularMagicLightQuarterTopLeftModel);
		}
	}
	public class CircularMagicLightQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Magic Light Quarter Top Right"; } }
		public CircularMagicLightQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularMagicLightQuarterTopRightModel);
		}
	}
	public class CircularMagicLightThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Magic Light Three Quarters"; } }
		public CircularMagicLightThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularMagicLightThreeQuartersModel);
		}
	}
	public class CircularFlatLightFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Light Full"; } }
		public CircularFlatLightFullModel() {
			DefaultStyleKey = typeof(CircularFlatLightFullModel);
		}
	}
	public class CircularFlatLightHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Light Half Top"; } }
		public CircularFlatLightHalfTopModel() {
			DefaultStyleKey = typeof(CircularFlatLightHalfTopModel);
		}
	}
	public class CircularFlatLightQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Light Quarter Top Left"; } }
		public CircularFlatLightQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularFlatLightQuarterTopLeftModel);
		}
	}
	public class CircularFlatLightQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Light Quarter Top Right"; } }
		public CircularFlatLightQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularFlatLightQuarterTopRightModel);
		}
	}
	public class CircularFlatLightThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Light Three Quarters"; } }
		public CircularFlatLightThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularFlatLightThreeQuartersModel);
		}
	}
	public class CircularFlatDarkFullModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Dark Full"; } }
		public CircularFlatDarkFullModel() {
			DefaultStyleKey = typeof(CircularFlatDarkFullModel);
		}
	}
	public class CircularFlatDarkHalfTopModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Dark Half Top"; } }
		public CircularFlatDarkHalfTopModel() {
			DefaultStyleKey = typeof(CircularFlatDarkHalfTopModel);
		}
	}
	public class CircularFlatDarkQuarterTopLeftModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Dark Quarter Top Left"; } }
		public CircularFlatDarkQuarterTopLeftModel() {
			DefaultStyleKey = typeof(CircularFlatDarkQuarterTopLeftModel);
		}
	}
	public class CircularFlatDarkQuarterTopRightModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Dark Quarter Top Right"; } }
		public CircularFlatDarkQuarterTopRightModel() {
			DefaultStyleKey = typeof(CircularFlatDarkQuarterTopRightModel);
		}
	}
	public class CircularFlatDarkThreeQuartersModel : PartialCircularGaugeModel {
		public override string ModelName { get { return "Flat Dark Three Quarters"; } }
		public CircularFlatDarkThreeQuartersModel() {
			DefaultStyleKey = typeof(CircularFlatDarkThreeQuartersModel);
		}
	}
}

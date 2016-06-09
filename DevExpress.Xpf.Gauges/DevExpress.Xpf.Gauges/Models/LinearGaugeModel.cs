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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class LinearGaugeModel : GaugeModelBase {
		public static readonly DependencyProperty ScaleModelsProperty = DependencyPropertyManager.Register("ScaleModels",
			typeof(LinearScaleModelCollection), typeof(LinearGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty MarkerModelsProperty = DependencyPropertyManager.Register("MarkerModels",
			typeof(LinearScaleMarkerModelCollection), typeof(LinearGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty RangeBarModelsProperty = DependencyPropertyManager.Register("RangeBarModels",
			typeof(LinearScaleRangeBarModelCollection), typeof(LinearGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty LevelBarModelsProperty = DependencyPropertyManager.Register("LevelBarModels",
			typeof(LinearScaleLevelBarModelCollection), typeof(LinearGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty RangeModelsProperty = DependencyPropertyManager.Register("RangeModels",
			typeof(LinearScaleRangeModelCollection), typeof(LinearGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty LayerModelsProperty = DependencyPropertyManager.Register("LayerModels",
			typeof(LayerModelCollection), typeof(LinearGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		public static readonly DependencyProperty ScaleLayerModelsProperty = DependencyPropertyManager.Register("ScaleLayerModels",
			typeof(LayerModelCollection), typeof(LinearGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public LinearScaleModelCollection ScaleModels {
			get { return (LinearScaleModelCollection)GetValue(ScaleModelsProperty); }
			set { SetValue(ScaleModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public LinearScaleMarkerModelCollection MarkerModels {
			get { return (LinearScaleMarkerModelCollection)GetValue(MarkerModelsProperty); }
			set { SetValue(MarkerModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public LinearScaleRangeBarModelCollection RangeBarModels {
			get { return (LinearScaleRangeBarModelCollection)GetValue(RangeBarModelsProperty); }
			set { SetValue(RangeBarModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public LinearScaleLevelBarModelCollection LevelBarModels {
			get { return (LinearScaleLevelBarModelCollection)GetValue(LevelBarModelsProperty); }
			set { SetValue(LevelBarModelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public LinearScaleRangeModelCollection RangeModels {
			get { return (LinearScaleRangeModelCollection)GetValue(RangeModelsProperty); }
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
		internal LinearScaleModel GetScaleModel(int index) {
			return GetModel(ScaleModelsProperty, index) as LinearScaleModel;
		}
		internal LinearScaleMarkerModel GetMarkerModel(int index) {
			return GetModel(MarkerModelsProperty, index) as LinearScaleMarkerModel;
		}
		internal LinearScaleRangeBarModel GetRangeBarModel(int index) {
			return GetModel(RangeBarModelsProperty, index) as LinearScaleRangeBarModel;
		}
		internal LinearScaleLevelBarModel GetLevelBarModel(int index) {
			return GetModel(LevelBarModelsProperty, index) as LinearScaleLevelBarModel;
		}
		internal LinearScaleRangeModel GetRangeModel(int index) {
			return GetModel(RangeModelsProperty, index) as LinearScaleRangeModel;
		}
		internal LayerModel GetLayerModel(int index) {
			return GetModel(LayerModelsProperty, index) as LayerModel;
		}
		internal LayerModel GetScaleLayerModel(int index) {
			return GetModel(ScaleLayerModelsProperty, index) as LayerModel;
		}
	}
	public class LinearDefaultModel : LinearGaugeModel {
		public override string ModelName { get { return "Default"; } }
		public LinearDefaultModel() {
			DefaultStyleKey = typeof(LinearDefaultModel);
		}
	}
	public class LinearCleanWhiteModel : LinearGaugeModel {
		public override string ModelName { get { return "Clean White"; } }
		public LinearCleanWhiteModel() {
			DefaultStyleKey = typeof(LinearCleanWhiteModel);
		}
	}
	public class LinearCosmicModel : LinearGaugeModel {
		public override string ModelName { get { return "Cosmic"; } }
		public LinearCosmicModel() {
			DefaultStyleKey = typeof(LinearCosmicModel);
		}
	}
	public class LinearSmartModel : LinearGaugeModel {
		public override string ModelName { get { return "Smart"; } }
		public LinearSmartModel() {
			DefaultStyleKey = typeof(LinearSmartModel);
		}
	}
	public class LinearProgressiveModel : LinearGaugeModel {
		public override string ModelName { get { return "Progressive"; } }
		public LinearProgressiveModel() {
			DefaultStyleKey = typeof(LinearProgressiveModel);
		}
	}
	public class LinearEcoModel : LinearGaugeModel {
		public override string ModelName { get { return "Eco"; } }
		public LinearEcoModel() {
			DefaultStyleKey = typeof(LinearEcoModel);
		}
	}
	public class LinearFutureModel : LinearGaugeModel {
		public override string ModelName { get { return "Future"; } }
		public LinearFutureModel() {
			DefaultStyleKey = typeof(LinearFutureModel);
		}
	}
	public class LinearClassicModel : LinearGaugeModel {
		public override string ModelName { get { return "Classic"; } }
		public LinearClassicModel() {
			DefaultStyleKey = typeof(LinearClassicModel);
		}
	}
	public class LinearIStyleModel : LinearGaugeModel {
		public override string ModelName { get { return "iStyle"; } }
		public LinearIStyleModel() {
			DefaultStyleKey = typeof(LinearIStyleModel);
		}
	}
	public class LinearYellowSubmarineModel : LinearGaugeModel {
		public override string ModelName { get { return "Yellow Submarine"; } }
		public LinearYellowSubmarineModel() {
			DefaultStyleKey = typeof(LinearYellowSubmarineModel);
		}
	}
	public class LinearMagicLightModel : LinearGaugeModel {
		public override string ModelName { get { return "Magic Light"; } }
		public LinearMagicLightModel() {
			DefaultStyleKey = typeof(LinearMagicLightModel);
		}
	}
	public class LinearRedThermometerModel : LinearGaugeModel {
		public override string ModelName { get { return "Red Thermometer"; } }
		public LinearRedThermometerModel() {
			DefaultStyleKey = typeof(LinearRedThermometerModel);
		}
	}
	public class LinearFlatLightModel : LinearGaugeModel {
		public override string ModelName { get { return "Flat Light"; } }
		public LinearFlatLightModel() {
			DefaultStyleKey = typeof(LinearFlatLightModel);
		}
	}
	public class LinearFlatDarkModel : LinearGaugeModel {
		public override string ModelName { get { return "Flat Dark"; } }
		public LinearFlatDarkModel() {
			DefaultStyleKey = typeof(LinearFlatDarkModel);
		}
	}
}

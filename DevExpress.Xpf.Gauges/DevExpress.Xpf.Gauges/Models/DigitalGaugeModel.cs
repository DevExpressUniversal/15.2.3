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
	public abstract class DigitalGaugeModel : GaugeModelBase {
		public static readonly DependencyProperty SevenSegmentsModelProperty = DependencyPropertyManager.Register("SevenSegmentsModel",
			typeof(SevenSegmentsModel), typeof(DigitalGaugeModel), new PropertyMetadata(null, PropertyChanged));
		public static readonly DependencyProperty FourteenSegmentsModelProperty = DependencyPropertyManager.Register("FourteenSegmentsModel",
			typeof(FourteenSegmentsModel), typeof(DigitalGaugeModel), new PropertyMetadata(null, PropertyChanged));
		public static readonly DependencyProperty Matrix5x8ModelProperty = DependencyPropertyManager.Register("Matrix5x8Model",
			typeof(Matrix5x8Model), typeof(DigitalGaugeModel), new PropertyMetadata(null, PropertyChanged));
		public static readonly DependencyProperty Matrix8x14ModelProperty = DependencyPropertyManager.Register("Matrix8x14Model",
			typeof(Matrix8x14Model), typeof(DigitalGaugeModel), new PropertyMetadata(null, PropertyChanged));
		public static readonly DependencyProperty LayerModelsProperty = DependencyPropertyManager.Register("LayerModels",
			typeof(LayerModelCollection), typeof(DigitalGaugeModel), new PropertyMetadata(null, CollectionPropertyChanged));
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public SevenSegmentsModel SevenSegmentsModel {
			get { return (SevenSegmentsModel)GetValue(SevenSegmentsModelProperty); }
			set { SetValue(SevenSegmentsModelProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public FourteenSegmentsModel FourteenSegmentsModel {
			get { return (FourteenSegmentsModel)GetValue(FourteenSegmentsModelProperty); }
			set { SetValue(FourteenSegmentsModelProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Matrix5x8Model Matrix5x8Model {
			get { return (Matrix5x8Model)GetValue(Matrix5x8ModelProperty); }
			set { SetValue(Matrix5x8ModelProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Matrix8x14Model Matrix8x14Model {
			get { return (Matrix8x14Model)GetValue(Matrix8x14ModelProperty); }
			set { SetValue(Matrix8x14ModelProperty, value); }
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
		internal LayerModel GetLayerModel(int index) {
			return GetModel(LayerModelsProperty, index) as LayerModel;
		}
	}
	public class DigitalDefaultModel : DigitalGaugeModel {
		public override string ModelName { get { return "Default"; } }
		public DigitalDefaultModel() {
			DefaultStyleKey = typeof(DigitalDefaultModel);
		}
	}
	public class DigitalCleanWhiteModel : DigitalGaugeModel {
		public override string ModelName { get { return "Clean White"; } }
		public DigitalCleanWhiteModel() {
			DefaultStyleKey = typeof(DigitalCleanWhiteModel);
		}
	}
	public class DigitalCosmicModel : DigitalGaugeModel {
		public override string ModelName { get { return "Cosmic"; } }
		public DigitalCosmicModel() {
			DefaultStyleKey = typeof(DigitalCosmicModel);
		}
	}
	public class DigitalSmartModel : DigitalGaugeModel {
		public override string ModelName { get { return "Smart"; } }
		public DigitalSmartModel() {
			DefaultStyleKey = typeof(DigitalSmartModel);
		}
	}
	public class DigitalRedClockModel : DigitalGaugeModel {
		public override string ModelName { get { return "Red Clock"; } }
		public DigitalRedClockModel() {
			DefaultStyleKey = typeof(DigitalRedClockModel);
		}
	}
	public class DigitalProgressiveModel : DigitalGaugeModel {
		public override string ModelName { get { return "Progressive"; } }
		public DigitalProgressiveModel() {
			DefaultStyleKey = typeof(DigitalProgressiveModel);
		}
	}
	public class DigitalEcoModel : DigitalGaugeModel {
		public override string ModelName { get { return "Eco"; } }
		public DigitalEcoModel() {
			DefaultStyleKey = typeof(DigitalEcoModel);
		}
	}
	public class DigitalFutureModel : DigitalGaugeModel {
		public override string ModelName { get { return "Future"; } }
		public DigitalFutureModel() {
			DefaultStyleKey = typeof(DigitalFutureModel);
		}
	}
	public class DigitalClassicModel : DigitalGaugeModel {
		public override string ModelName { get { return "Classic"; } }
		public DigitalClassicModel() {
			DefaultStyleKey = typeof(DigitalClassicModel);
		}
	}
	public class DigitalIStyleModel : DigitalGaugeModel {
		public override string ModelName { get { return "iStyle"; } }
		public DigitalIStyleModel() {
			DefaultStyleKey = typeof(DigitalIStyleModel);
		}
	}
	public class DigitalYellowSubmarineModel : DigitalGaugeModel {
		public override string ModelName { get { return "Yellow Submarine"; } }
		public DigitalYellowSubmarineModel() {
			DefaultStyleKey = typeof(DigitalYellowSubmarineModel);
		}
	}
	public class DigitalMagicLightModel : DigitalGaugeModel {
		public override string ModelName { get { return "Magic Light"; } }
		public DigitalMagicLightModel() {
			DefaultStyleKey = typeof(DigitalMagicLightModel);
		}
	}
}

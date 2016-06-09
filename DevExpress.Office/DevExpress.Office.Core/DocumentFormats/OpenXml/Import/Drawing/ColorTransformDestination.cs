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
using System.IO;
using System.Xml;
using System.Globalization;
using System.Drawing;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Office.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office.Import.OpenXml {
	#region ColorTransformDestinationBase (abstract class)
	public abstract class ColorTransformDestinationBase : LeafElementDestination<DestinationAndXmlBasedImporter> {
		readonly DrawingColor color;
		protected ColorTransformDestinationBase(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer) {
			this.color = color;
		}
		protected DrawingColor Color { get { return color; } }
		protected int GetIntegerValue(XmlReader reader) {
			return Importer.GetIntegerValue(reader, "val");
		}
	}
	#endregion
	#region ColorTransformDestinationAlpha
	public class ColorTransformDestinationAlpha : ColorTransformDestinationBase {
		public ColorTransformDestinationAlpha(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new AlphaColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationAlphaModulation
	public class ColorTransformDestinationAlphaModulation : ColorTransformDestinationBase {
		public ColorTransformDestinationAlphaModulation(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new AlphaModulationColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationAlphaOffset
	public class ColorTransformDestinationAlphaOffset : ColorTransformDestinationBase {
		public ColorTransformDestinationAlphaOffset(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new AlphaOffsetColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationBlue
	public class ColorTransformDestinationBlue : ColorTransformDestinationBase {
		public ColorTransformDestinationBlue(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new BlueColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationBlueModification
	public class ColorTransformDestinationBlueModification : ColorTransformDestinationBase {
		public ColorTransformDestinationBlueModification(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new BlueModulationColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationBlueOffset
	public class ColorTransformDestinationBlueOffset : ColorTransformDestinationBase {
		public ColorTransformDestinationBlueOffset(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new BlueOffsetColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationComplement
	public class ColorTransformDestinationComplement : ColorTransformDestinationBase {
		public ColorTransformDestinationComplement(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new ComplementColorTransform());
		}
	}
	#endregion
	#region ColorTransformDestinationGamma
	public class ColorTransformDestinationGamma : ColorTransformDestinationBase {
		public ColorTransformDestinationGamma(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new GammaColorTransform());
		}
	}
	#endregion
	#region ColorTransformDestinationGray
	public class ColorTransformDestinationGray : ColorTransformDestinationBase {
		public ColorTransformDestinationGray(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new GrayscaleColorTransform());
		}
	}
	#endregion
	#region ColorTransformDestinationGreen
	public class ColorTransformDestinationGreen : ColorTransformDestinationBase {
		public ColorTransformDestinationGreen(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new GreenColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationGreenModification
	public class ColorTransformDestinationGreenModification : ColorTransformDestinationBase {
		public ColorTransformDestinationGreenModification(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new GreenModulationColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationGreenOffset
	public class ColorTransformDestinationGreenOffset : ColorTransformDestinationBase {
		public ColorTransformDestinationGreenOffset(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new GreenOffsetColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationHue
	public class ColorTransformDestinationHue : ColorTransformDestinationBase {
		public ColorTransformDestinationHue(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new HueColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationHueModulate
	public class ColorTransformDestinationHueModulate : ColorTransformDestinationBase {
		public ColorTransformDestinationHueModulate(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new HueModulationColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationHueOffset
	public class ColorTransformDestinationHueOffset : ColorTransformDestinationBase {
		public ColorTransformDestinationHueOffset(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new HueOffsetColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationInverse
	public class ColorTransformDestinationInverse : ColorTransformDestinationBase {
		public ColorTransformDestinationInverse(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new InverseColorTransform());
		}
	}
	#endregion
	#region ColorTransformDestinationInverseGamma
	public class ColorTransformDestinationInverseGamma : ColorTransformDestinationBase {
		public ColorTransformDestinationInverseGamma(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new InverseGammaColorTransform());
		}
	}
	#endregion
	#region ColorTransformDestinationLuminance
	public class ColorTransformDestinationLuminance : ColorTransformDestinationBase {
		public ColorTransformDestinationLuminance(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new LuminanceColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationLuminanceModulation
	public class ColorTransformDestinationLuminanceModulation : ColorTransformDestinationBase {
		public ColorTransformDestinationLuminanceModulation(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new LuminanceModulationColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationLuminanceOffset
	public class ColorTransformDestinationLuminanceOffset : ColorTransformDestinationBase {
		public ColorTransformDestinationLuminanceOffset(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new LuminanceOffsetColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationRed
	public class ColorTransformDestinationRed : ColorTransformDestinationBase {
		public ColorTransformDestinationRed(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new RedColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationRedModulation
	public class ColorTransformDestinationRedModulation : ColorTransformDestinationBase {
		public ColorTransformDestinationRedModulation(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new RedModulationColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationRedOffset
	public class ColorTransformDestinationRedOffset : ColorTransformDestinationBase {
		public ColorTransformDestinationRedOffset(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new RedOffsetColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationSaturation
	public class ColorTransformDestinationSaturation : ColorTransformDestinationBase {
		public ColorTransformDestinationSaturation(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new SaturationColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationSaturationModulation
	public class ColorTransformDestinationSaturationModulation : ColorTransformDestinationBase {
		public ColorTransformDestinationSaturationModulation(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new SaturationModulationColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationSaturationOffset
	public class ColorTransformDestinationSaturationOffset : ColorTransformDestinationBase {
		public ColorTransformDestinationSaturationOffset(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new SaturationOffsetColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationShade
	public class ColorTransformDestinationShade : ColorTransformDestinationBase {
		public ColorTransformDestinationShade(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new ShadeColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
	#region ColorTransformDestinationTint
	public class ColorTransformDestinationTint : ColorTransformDestinationBase {
		public ColorTransformDestinationTint(DestinationAndXmlBasedImporter importer, DrawingColor color)
			: base(importer, color) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			Color.Transforms.AddCore(new TintColorTransform(GetIntegerValue(reader)));
		}
	}
	#endregion
}

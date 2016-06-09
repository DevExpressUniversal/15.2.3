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

using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.OpenXml.Export;
using DevExpress.Utils;
namespace DevExpress.Office.Import.OpenXml {
	#region DrawingEffectDestinationBase (abstract class)
	public abstract class DrawingEffectDestinationBase : LeafElementDestination<DestinationAndXmlBasedImporter> {
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
		readonly DrawingEffectCollection effects;
		protected DrawingEffectDestinationBase(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer) {
			this.effects = effects;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		protected DrawingEffectCollection Effects { get { return effects; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			CheckPropertyValues();
			effects.AddCore(CreateEffect());
		}
		protected virtual void CheckPropertyValues() {
		}
		protected abstract IDrawingEffect CreateEffect();
	}
	#endregion
	#region DrawingColorEffectDestinationBase (abstract class)
	public abstract class DrawingColorEffectDestinationBase : DrawingColorDestinationBase {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			AddDrawingColorHandlers(result);
			return result;
		}
		#endregion
		readonly DrawingEffectCollection effects;
		protected DrawingColorEffectDestinationBase(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, new DrawingColor(effects.DocumentModel)) {
			this.effects = effects;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		protected DrawingEffectCollection Effects { get { return effects; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			CheckPropertyValues();
			effects.AddCore(CreateEffect());
		}
		protected virtual void CheckPropertyValues() {
			if (!HasColor)
				Importer.ThrowInvalidFile();
		}
		protected abstract IDrawingEffect CreateEffect();
	}
	#endregion
	#region DrawingEffectsListDestination
	public class DrawingEffectsListDestination : ElementDestination<DestinationAndXmlBasedImporter> {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		protected static void AddEffectListHandlerTable(ElementHandlerTable<DestinationAndXmlBasedImporter> table) {
			table.Add("blur", OnBlurEffect);
			table.Add("fillOverlay", OnFillOverlayEffect);
			table.Add("glow", OnGlowEffect);
			table.Add("innerShdw", OnInnerShadowEffect);
			table.Add("outerShdw", OnOuterShadowEffect);
			table.Add("prstShdw", OnPresetShadowEffect);
			table.Add("reflection", OnReflectionEffect);
			table.Add("softEdge", OnSoftEdgeEffect);
		}
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			AddEffectListHandlerTable(result);
			return result;
		}
		static DrawingEffectsListDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingEffectsListDestination)importer.PeekDestination();
		}
		static Destination OnBlurEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new BlurEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnFillOverlayEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new FillOverlayEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnGlowEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new GlowEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnInnerShadowEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new InnerShadowEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnOuterShadowEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new OuterShadowEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnPresetShadowEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new PresetShadowEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnReflectionEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ReflectionEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnSoftEdgeEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new SoftEdgeEffectDestination(importer, GetThis(importer).Effects);
		}
		#endregion
		ContainerEffect containerEffect;
		public DrawingEffectsListDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
			this.containerEffect = new ContainerEffect(importer.ActualDocumentModel);
		}
		public DrawingEffectsListDestination(DestinationAndXmlBasedImporter importer, ContainerEffect containerEffect)
			: base(importer) {
			Guard.ArgumentNotNull(containerEffect, "containerEffect");
			this.containerEffect = containerEffect;
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public ContainerEffect ContainerEffect { get { return containerEffect; } }
		protected DrawingEffectCollection Effects { get { return containerEffect.Effects; } }
		#endregion
	}
	#endregion
	#region DrawingEffectsDAGDestination
	public class DrawingEffectsDAGDestination : DrawingEffectsListDestination {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("alphaBiLevel", OnAlphaBiLevelEffect);
			result.Add("alphaCeiling", OnAlphaCeilingEffect);
			result.Add("alphaFloor", OnAlphaFloorEffect);
			result.Add("alphaInv", OnAlphaInverseEffect);
			result.Add("alphaMod", OnAlphaModulateEffect);
			result.Add("alphaModFix", OnAlphaModulateFixedEffect);
			result.Add("alphaRepl", OnAlphaReplaceEffect);
			result.Add("alphaOutset", OnAlphaOutsetEffect);
			result.Add("biLevel", OnBiLevelEffect);
			result.Add("blend", OnBlendEffect);
			result.Add("clrChange", OnColorChangeEffect);
			result.Add("cont", OnContainerEffect);
			result.Add("duotone", OnDuotoneEffect);
			result.Add("effect", OnEffect);
			result.Add("fill", OnFillEffect);
			result.Add("grayscl", OnGrayScaleEffect);
			result.Add("hsl", OnHSLEffect);
			result.Add("lum", OnLuminanceEffect);
			result.Add("relOff", OnRelativeOffsetEffect);
			result.Add("clrRepl", OnSolidColorReplacement);
			result.Add("tint", OnTintEffect);
			result.Add("xfrm", OnTransformEffect);
			result.Add("extLst", OnExtensionList);
			AddEffectListHandlerTable(result);
			return result;
		}
		static DrawingEffectsDAGDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (DrawingEffectsDAGDestination)importer.PeekDestination();
		}
		static Destination OnAlphaBiLevelEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaBiLevelEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaCeilingEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaCeilingEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaFloorEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaFloorEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaInverseEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaInverseEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaModulateEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaModulateEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaModulateFixedEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaModulateFixedEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaReplaceEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaReplaceEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnAlphaOutsetEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new AlphaOutsetEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnBiLevelEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new BiLevelEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnBlendEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new BlendEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnColorChangeEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ColorChangeEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnContainerEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new ContainerEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnDuotoneEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DuotoneEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new EffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnFillEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new FillEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnGrayScaleEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new GrayScaleEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnHSLEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new HSLEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnLuminanceEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new LuminanceEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnRelativeOffsetEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new RelativeOffsetEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnSolidColorReplacement(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new SolidColorReplacementDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnTintEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new TintEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnTransformEffect(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new TransformEffectDestination(importer, GetThis(importer).Effects);
		}
		static Destination OnExtensionList(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new EmptyDestination<DestinationAndXmlBasedImporter>(importer);
		}
		#endregion
		public DrawingEffectsDAGDestination(DestinationAndXmlBasedImporter importer)
			: base(importer) {
		}
		public DrawingEffectsDAGDestination(DestinationAndXmlBasedImporter importer, ContainerEffect containerEffect)
			: base(importer, containerEffect) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			IDocumentModel documentModel = Importer.ActualDocumentModel;
			documentModel.BeginUpdate();
			try {
				ContainerEffect.Name = Importer.ReadAttribute(reader, "name");
				ContainerEffect.Type = Importer.GetWpEnumValue(reader, "type", OpenXmlExporterBase.DrawingEffectContainerTypeTable, DrawingEffectContainerType.Sibling);
				ContainerEffect.HasEffectsList = false;
			}
			finally {
				documentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region ImportShadowEffectHelper
	public static class ImportShadowEffectHelper {
		public static ScalingFactorInfo GetScalingFactorInfo(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			int horizontalScalingFactor = importer.GetIntegerValue(reader, "sx", DrawingValueConstants.ThousandthOfPercentage);
			int verticalScalingFactor = importer.GetIntegerValue(reader, "sy", DrawingValueConstants.ThousandthOfPercentage);
			return new ScalingFactorInfo(horizontalScalingFactor, verticalScalingFactor);
		}
		public static SkewAnglesInfo GetSkewAnglesInfo(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			int horizontalSkew = importer.GetIntegerValue(reader, "kx", 0);
			int verticalSkew = importer.GetIntegerValue(reader, "ky", 0);
			DrawingValueChecker.CheckFixedAngle(horizontalSkew, "horizontalSkew");
			DrawingValueChecker.CheckFixedAngle(verticalSkew, "verticalSkew");
			return new SkewAnglesInfo(horizontalSkew, verticalSkew);
		}
		public static OuterShadowEffectInfo GetOuterShadowEffectInfo(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			RectangleAlignType shadowAlignment = importer.GetWpEnumValue<RectangleAlignType>(reader, "algn", OpenXmlExporterBase.RectangleAlignTypeTable, RectangleAlignType.Bottom);
			long blurRadius = importer.GetLongValue(reader, "blurRad", 0);
			DrawingValueChecker.CheckPositiveCoordinate(blurRadius, "blurRadius");
			bool rotateWithShape = importer.GetOnOffValue(reader, "rotWithShape", true);
			return new OuterShadowEffectInfo(GetOffsetShadowInfo(importer, reader), GetScalingFactorInfo(importer, reader), GetSkewAnglesInfo(importer, reader), shadowAlignment, blurRadius, rotateWithShape);
		}
		public static OffsetShadowInfo GetOffsetShadowInfo(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			long distance = importer.GetLongValue(reader, "dist", 0);
			int direction = importer.GetIntegerValue(reader, "dir", 0);
			DrawingValueChecker.CheckPositiveCoordinate(distance, "distance");
			DrawingValueChecker.CheckPositiveFixedAngle(direction, "direction");
			return new OffsetShadowInfo(distance, direction);
		}
	}
	#endregion
	#region AlphaBiLevelEffectDestination
	public class AlphaBiLevelEffectDestination : DrawingEffectDestinationBase {
		int thresh;
		public AlphaBiLevelEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		protected int Thresh { get { return thresh; } }
		public override void ProcessElementOpen(XmlReader reader) {
			thresh = Importer.GetIntegerValue(reader, "thresh", int.MinValue);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckPositiveFixedPercentage(thresh, "thresh");
		}
		protected override IDrawingEffect CreateEffect() {
			return new AlphaBiLevelEffect(thresh);
		}
	}
	#endregion
	#region AlphaCeilingEffectDestination
	public class AlphaCeilingEffectDestination : DrawingEffectDestinationBase {
		public AlphaCeilingEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		protected override IDrawingEffect CreateEffect() {
			return DrawingEffect.AlphaCeilingEffect;
		}
	}
	#endregion
	#region AlphaFloorEffectDestination
	public class AlphaFloorEffectDestination : DrawingEffectDestinationBase {
		public AlphaFloorEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		protected override IDrawingEffect CreateEffect() {
			return DrawingEffect.AlphaFloorEffect;
		}
	}
	#endregion
	#region AlphaInverseEffectDestination
	public class AlphaInverseEffectDestination : DrawingColorEffectDestinationBase {
		public AlphaInverseEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		protected override IDrawingEffect CreateEffect() {
			return new AlphaInverseEffect(Color);
		}
		protected override void CheckPropertyValues() {
		}
	}
	#endregion
	#region AlphaModulateEffectDestination
	public class AlphaModulateEffectDestination : DrawingEffectDestinationBase {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("cont", OnCont);
			return result;
		}
		static AlphaModulateEffectDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (AlphaModulateEffectDestination)importer.PeekDestination();
		}
		static Destination OnCont(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			AlphaModulateEffectDestination destination = GetThis(importer);
			ContainerEffect containerEffect = new ContainerEffect(destination.Effects.DocumentModel);
			destination.containerEffect = containerEffect;
			return new DrawingEffectsDAGDestination(importer, containerEffect);
		}
		#endregion
		ContainerEffect containerEffect;
		public AlphaModulateEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		#region Properties
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		protected ContainerEffect ContainerEffect { get { return containerEffect; } }
		#endregion
		protected override IDrawingEffect CreateEffect() {
			return new AlphaModulateEffect(containerEffect);
		}
	}
	#endregion
	#region AlphaModulateFixedEffectDestination
	public class AlphaModulateFixedEffectDestination : DrawingEffectDestinationBase {
		int amount;
		public AlphaModulateFixedEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			amount = Importer.GetIntegerValue(reader, "amt", DrawingValueConstants.ThousandthOfPercentage);
		}
		protected override void CheckPropertyValues() {
			Guard.ArgumentNonNegative(amount, "amount");
		}
		protected override IDrawingEffect CreateEffect() {
			return new AlphaModulateFixedEffect(amount);
		}
	}
	#endregion
	#region AlphaOutsetEffectDestination
	public class AlphaOutsetEffectDestination : DrawingEffectDestinationBase {
		long radius;
		public AlphaOutsetEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			radius = Importer.GetLongValue(reader, "rad", 0);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckPositiveCoordinate(radius, "radius");
		}
		protected override IDrawingEffect CreateEffect() {
			return new AlphaOutsetEffect(radius);
		}
	}
	#endregion
	#region AlphaReplaceEffectDestination
	public class AlphaReplaceEffectDestination : DrawingEffectDestinationBase {
		int alpha;
		public AlphaReplaceEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			alpha = Importer.GetIntegerValue(reader, "a", int.MinValue);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckPositiveFixedPercentage(alpha, "alpha");
		}
		protected override IDrawingEffect CreateEffect() {
			return new AlphaReplaceEffect(alpha);
		}
	}
	#endregion
	#region BiLevelEffectDestination
	public class BiLevelEffectDestination : AlphaBiLevelEffectDestination {
		public BiLevelEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		protected override IDrawingEffect CreateEffect() {
			return new BiLevelEffect(Thresh);
		}
	}
	#endregion
	#region BlendEffectDestination
	public class BlendEffectDestination : AlphaModulateEffectDestination {
		BlendMode? blendMode;
		public BlendEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			blendMode = Importer.GetWpEnumOnOffNullValue(reader, "blend", OpenXmlExporterBase.BlendModeTable);
			if (!blendMode.HasValue)
				Importer.ThrowInvalidFile();
		}
		protected override IDrawingEffect CreateEffect() {
			return new BlendEffect(ContainerEffect, blendMode.Value);
		}
	}
	#endregion
	#region BlurEffectDestination
	public class BlurEffectDestination : DrawingEffectDestinationBase {
		#region Fields
		bool glow;
		long radius;
		#endregion
		public BlurEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			glow = Importer.GetOnOffValue(reader, "grow", true);
			radius = Importer.GetLongValue(reader, "rad", 0);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckPositiveCoordinate(radius, "radius");
		}
		protected override IDrawingEffect CreateEffect() {
			return new BlurEffect(radius, glow);
		}
	}
	#endregion
	#region ColorChangeEffectDestination
	public class ColorChangeEffectDestination : DrawingColorEffectDestinationBase {
		#region Static Members
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			result.Add("clrFrom", OnColorFrom);
			result.Add("clrTo", OnColorTo);
			return result;
		}
		static ColorChangeEffectDestination GetThis(DestinationAndXmlBasedImporter importer) {
			return (ColorChangeEffectDestination)importer.PeekDestination();
		}
		static Destination OnColorFrom(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			return new DrawingColorDestination(importer, GetColor(importer));
		}
		static Destination OnColorTo(DestinationAndXmlBasedImporter importer, XmlReader reader) {
			ColorChangeEffectDestination destination = GetThis(importer);
			DrawingColor colorTo = new DrawingColor(destination.Effects.DocumentModel);
			destination.colorTo = colorTo;
			return new DrawingColorDestination(importer, colorTo);
		}
		#endregion
		#region Fields
		DrawingColor colorTo;
		bool useAlpha;
		#endregion
		public ColorChangeEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			useAlpha = Importer.GetOnOffValue(reader, "useA", true);
		}
		protected override IDrawingEffect CreateEffect() {
			if (!HasColor)
				Importer.ThrowInvalidFile();
			return new ColorChangeEffect(Color, colorTo, useAlpha);
		}
	}
	#endregion
	#region ContainerEffectDestination
	public class ContainerEffectDestination : DrawingEffectsDAGDestination {
		DrawingEffectCollection effects;
		public ContainerEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer) {
			this.effects = effects;
		}
		public override void ProcessElementClose(XmlReader reader) {
			effects.Add(ContainerEffect);
		}
	}
	#endregion
	#region DuotoneEffectDestination
	public class DuotoneEffectDestination : DrawingColorEffectDestinationBase {
		#region Fields
		readonly DrawingColor secondColor;
		bool hasSecondColor;
		#endregion
		public DuotoneEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
			this.secondColor = new DrawingColor(effects.DocumentModel);
		}
		#region Properties
		protected override DrawingColor Color { get { return IsFirstColor ? base.Color : secondColor; } }
		protected override bool HasColor {
			get { return IsFirstColor ? base.HasColor : hasSecondColor; }
			set {
				if (IsFirstColor)
					base.HasColor = value;
				else
					hasSecondColor = value;
			}
		}
		bool IsFirstColor { get { return base.Color.Info.IsEmpty; } }
		#endregion
		protected override IDrawingEffect CreateEffect() {
			return new DuotoneEffect(base.Color, secondColor);
		}
		protected override void CheckPropertyValues() {
			if (!base.HasColor || !hasSecondColor)
				Importer.ThrowInvalidFile();
		}
	}
	#endregion
	#region EffectDestination
	public class EffectDestination : DrawingEffectDestinationBase {
		string referenceToken;
		public EffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			referenceToken = Importer.ReadAttribute(reader, "ref");
		}
		protected override IDrawingEffect CreateEffect() {
			return new Effect(referenceToken);
		}
	}
	#endregion
	#region FillEffectDestination
	public class FillEffectDestination : DrawingFillDestinationBase {
		#region Handler Table
		static readonly ElementHandlerTable<DestinationAndXmlBasedImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<DestinationAndXmlBasedImporter> CreateElementHandlerTable() {
			ElementHandlerTable<DestinationAndXmlBasedImporter> result = new ElementHandlerTable<DestinationAndXmlBasedImporter>();
			AddFillHandlers(result);
			return result;
		}
		#endregion
		readonly DrawingEffectCollection effects;
		public FillEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer) {
			this.effects = effects;
		}
		protected override ElementHandlerTable<DestinationAndXmlBasedImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			effects.Add(CreateEffect());
		}
		protected virtual IDrawingEffect CreateEffect() {
			return new FillEffect(Fill);
		}
	}
	#endregion
	#region FillOverlayEffectDestination
	public class FillOverlayEffectDestination : FillEffectDestination {
		BlendMode? blendMode;
		public FillOverlayEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			blendMode = Importer.GetWpEnumOnOffNullValue<BlendMode>(reader, "blend", OpenXmlExporterBase.BlendModeTable);
			if (!blendMode.HasValue)
				Importer.ThrowInvalidFile();
		}
		protected override IDrawingEffect CreateEffect() {
			return new FillOverlayEffect(Fill, blendMode.Value);
		}
	}
	#endregion
	#region GrayScaleEffectDestination
	public class GrayScaleEffectDestination : DrawingEffectDestinationBase {
		public GrayScaleEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		protected override IDrawingEffect CreateEffect() {
			return DrawingEffect.GrayScaleEffect;
		}
	}
	#endregion
	#region GlowEffectDestination
	public class GlowEffectDestination : DrawingColorEffectDestinationBase {
		long radius;
		public GlowEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			radius = Importer.GetLongValue(reader, "rad", 0);
		}
		protected override void CheckPropertyValues() {
			base.CheckPropertyValues();
			DrawingValueChecker.CheckPositiveCoordinate(radius, "radius");
		}
		protected override IDrawingEffect CreateEffect() {
			return new GlowEffect(Color, radius);
		}
	}
	#endregion
	#region HSLEffectDestination
	public class HSLEffectDestination : DrawingEffectDestinationBase {
		#region Fields
		int hue;
		int saturation;
		int luminance;
		#endregion
		public HSLEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			hue = Importer.GetIntegerValue(reader, "hue", 0);
			saturation = Importer.GetIntegerValue(reader, "sat", 0);
			luminance = Importer.GetIntegerValue(reader, "lum", 0);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckPositiveFixedAngle(hue, "hue");
			DrawingValueChecker.CheckFixedPercentage(saturation, "saturation");
			DrawingValueChecker.CheckFixedPercentage(luminance, "luminance");
		}
		protected override IDrawingEffect CreateEffect() {
			return new HSLEffect(hue, saturation, luminance);
		}
	}
	#endregion
	#region InnerShadowEffectDestination
	public class InnerShadowEffectDestination : DrawingColorEffectDestinationBase {
		#region Fields
		long blurRadius;
		OffsetShadowInfo info;
		#endregion
		public InnerShadowEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			blurRadius = Importer.GetLongValue(reader, "blurRad", 0);
			info = ImportShadowEffectHelper.GetOffsetShadowInfo(Importer, reader);
		}
		protected override void CheckPropertyValues() {
			base.CheckPropertyValues();
			DrawingValueChecker.CheckPositiveCoordinate(blurRadius, "blurRadius");
		}
		protected override IDrawingEffect CreateEffect() {
			return new InnerShadowEffect(Color, info, blurRadius);
		}
	}
	#endregion
	#region LuminanceEffectDestination
	public class LuminanceEffectDestination : DrawingEffectDestinationBase {
		#region Fields
		int bright;
		int contrast;
		#endregion
		public LuminanceEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			bright = Importer.GetIntegerValue(reader, "bright", 0);
			contrast = Importer.GetIntegerValue(reader, "contrast", 0);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckFixedPercentage(bright, "bright");
			DrawingValueChecker.CheckFixedPercentage(contrast, "contrast");
		}
		protected override IDrawingEffect CreateEffect() {
			return new LuminanceEffect(bright, contrast);
		}
	}
	#endregion
	#region OuterShadowEffectDestination
	public class OuterShadowEffectDestination : DrawingColorEffectDestinationBase {
		OuterShadowEffectInfo info;
		public OuterShadowEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			info = ImportShadowEffectHelper.GetOuterShadowEffectInfo(Importer, reader);
		}
		protected override IDrawingEffect CreateEffect() {
			return new OuterShadowEffect(Color, info);
		}
	}
	#endregion
	#region PresetShadowEffectDestination
	public class PresetShadowEffectDestination : DrawingColorEffectDestinationBase {
		#region Fields
		PresetShadowType? type;
		OffsetShadowInfo info;
		#endregion
		public PresetShadowEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			type = Importer.GetWpEnumOnOffNullValue(reader, "prst", OpenXmlExporterBase.PresetShadowTypeTable);
			if (!type.HasValue)
				Importer.ThrowInvalidFile();
			info = ImportShadowEffectHelper.GetOffsetShadowInfo(Importer, reader);
		}
		protected override IDrawingEffect CreateEffect() {
			return new PresetShadowEffect(Color, type.Value, info);
		}
	}
	#endregion
	#region ReflectionEffectDestination
	public class ReflectionEffectDestination : DrawingEffectDestinationBase {
		#region Fields
		OuterShadowEffectInfo outerShadowEffectInfo;
		int startOpacity;
		int endOpacity;
		int startPosition;
		int endPosition;
		int fadeDirection;
		#endregion
		public ReflectionEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			startOpacity = Importer.GetIntegerValue(reader, "stA", DrawingValueConstants.ThousandthOfPercentage);
			endOpacity = Importer.GetIntegerValue(reader, "endA", 0);
			startPosition = Importer.GetIntegerValue(reader, "stPos", 0);
			endPosition = Importer.GetIntegerValue(reader, "endPos", DrawingValueConstants.ThousandthOfPercentage);
			fadeDirection = Importer.GetIntegerValue(reader, "fadeDir", DrawingValueConstants.MaxFixedAngle);
			outerShadowEffectInfo = ImportShadowEffectHelper.GetOuterShadowEffectInfo(Importer, reader);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckPositiveFixedPercentage(startOpacity, "startOpacity");
			DrawingValueChecker.CheckPositiveFixedPercentage(startPosition, "startPosition");
			DrawingValueChecker.CheckPositiveFixedPercentage(endOpacity, "endOpacity");
			DrawingValueChecker.CheckPositiveFixedPercentage(endPosition, "endPosition");
			DrawingValueChecker.CheckPositiveFixedAngle(fadeDirection, "fadeDirection");
		}
		protected override IDrawingEffect CreateEffect() {
			ReflectionOpacityInfo reflectionOpacityInfo = new ReflectionOpacityInfo(startOpacity, startPosition, endOpacity, endPosition, fadeDirection);
			return new ReflectionEffect(reflectionOpacityInfo, outerShadowEffectInfo);
		}
	}
	#endregion
	#region RelativeOffsetEffectDestination
	public class RelativeOffsetEffectDestination : DrawingEffectDestinationBase {
		#region Fields
		int offsetX;
		int offsetY;
		#endregion
		public RelativeOffsetEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			offsetX = Importer.GetIntegerValue(reader, "tx", 0);
			offsetY = Importer.GetIntegerValue(reader, "ty", 0);
		}
		protected override IDrawingEffect CreateEffect() {
			return new RelativeOffsetEffect(offsetX, offsetY);
		}
	}
	#endregion
	#region SolidColorReplacementDestination
	public class SolidColorReplacementDestination : DrawingColorEffectDestinationBase {
		public SolidColorReplacementDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		protected override IDrawingEffect CreateEffect() {
			return new SolidColorReplacementEffect(Color);
		}
	}
	#endregion
	#region SoftEdgeEffectDestination
	public class SoftEdgeEffectDestination : DrawingEffectDestinationBase {
		long radius;
		public SoftEdgeEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			radius = Importer.GetLongValue(reader, "rad", long.MinValue);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckPositiveCoordinate(radius, "radius");
		}
		protected override IDrawingEffect CreateEffect() {
			return new SoftEdgeEffect(radius);
		}
	}
	#endregion
	#region TintEffectDestination
	public class TintEffectDestination : DrawingEffectDestinationBase {
		#region Fields
		int hue;
		int amount;
		#endregion
		public TintEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			hue = Importer.GetIntegerValue(reader, "hue", 0);
			amount = Importer.GetIntegerValue(reader, "amt", 0);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckPositiveFixedAngle(hue, "hue");
			DrawingValueChecker.CheckFixedPercentage(amount, "amount");
		}
		protected override IDrawingEffect CreateEffect() {
			return new TintEffect(hue, amount);
		}
	}
	#endregion
	#region TransformEffectDestination
	public class TransformEffectDestination : DrawingEffectDestinationBase {
		#region Fields
		ScalingFactorInfo scalingFactorInfo;
		SkewAnglesInfo skewAnglesInfo;
		long horizontalShift;
		long verticalShift;
		#endregion
		public TransformEffectDestination(DestinationAndXmlBasedImporter importer, DrawingEffectCollection effects)
			: base(importer, effects) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			scalingFactorInfo = ImportShadowEffectHelper.GetScalingFactorInfo(Importer, reader);
			skewAnglesInfo = ImportShadowEffectHelper.GetSkewAnglesInfo(Importer, reader);
			horizontalShift = Importer.GetLongValue(reader, "tx", 0);
			verticalShift = Importer.GetLongValue(reader, "ty", 0);
		}
		protected override void CheckPropertyValues() {
			DrawingValueChecker.CheckCoordinate(horizontalShift, "horizontalShift");
			DrawingValueChecker.CheckCoordinate(verticalShift, "verticalShift");
		}
		protected override IDrawingEffect CreateEffect() {
			return new TransformEffect(scalingFactorInfo, skewAnglesInfo, new CoordinateShiftInfo(horizontalShift, verticalShift));
		}
	}
	#endregion
}

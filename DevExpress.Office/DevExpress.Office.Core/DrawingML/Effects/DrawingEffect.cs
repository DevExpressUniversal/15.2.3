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
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office.Drawing {
	#region DrawingEffectCollection
	public class DrawingEffectCollection : UndoableClonableCollection<IDrawingEffect> {
		public DrawingEffectCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
		}
		public void ApplyEffects(IDrawingEffectVisitor visitor) {
			int count = Count;
			for (int i = 0; i < count; i++)
				InnerList[i].Visit(visitor);
		}
		public override IDrawingEffect GetCloneItem(IDrawingEffect item, IDocumentModelPart documentModelPart) {
			return item.CloneTo(documentModelPart.DocumentModel);
		}
		public override UndoableClonableCollection<IDrawingEffect> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new DrawingEffectCollection(documentModelPart.DocumentModel);
		}
	}
	#endregion
	#region DrawingEffects
	#region DrawingEffect
	public sealed class DrawingEffect : IDrawingEffect {
		public static DrawingEffect AlphaCeilingEffect = new DrawingEffect(alphaCeilingEffectIndex);
		public static DrawingEffect AlphaFloorEffect = new DrawingEffect(alphaFloorEffectIndex);
		public static DrawingEffect GrayScaleEffect = new DrawingEffect(grayScaleEffectIndex);
		const int alphaCeilingEffectIndex = 0;
		const int alphaFloorEffectIndex = 1;
		const int grayScaleEffectIndex = 2;
		readonly int index;
		private DrawingEffect(int index) {
			this.index = index;
		}
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			if (index == alphaCeilingEffectIndex)
				visitor.AlphaCeilingEffectVisit();
			else if (index == alphaFloorEffectIndex)
				visitor.AlphaFloorEffectVisit();
			else
				visitor.GrayScaleEffectVisit();
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			if (index == alphaCeilingEffectIndex)
				return DrawingEffect.AlphaCeilingEffect;
			if (index == alphaFloorEffectIndex)
				return DrawingEffect.AlphaFloorEffect;
			return DrawingEffect.GrayScaleEffect;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingEffect value = obj as DrawingEffect;
			if (value == null)
				return false;
			return index == value.index;
		}
		public override int GetHashCode() {
			return index;
		}
		#endregion
	}
	#endregion
	#region AlphaBiLevelEffect
	public class AlphaBiLevelEffect : IDrawingEffect {
		readonly int thresh; 
		public AlphaBiLevelEffect(int thresh) {
			this.thresh = thresh;
		}
		public int Thresh { get { return thresh; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new AlphaBiLevelEffect(thresh);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			AlphaBiLevelEffect value = obj as AlphaBiLevelEffect;
			if (value == null)
				return false;
			return thresh == value.thresh;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ thresh.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region AlphaInverseEffect
	public class AlphaInverseEffect : IDrawingEffect {
		readonly DrawingColor color;
		public AlphaInverseEffect(DrawingColor color)  {
			Guard.ArgumentNotNull(color, "AlphaInverseColor");
			this.color = color;
		}
		public DrawingColor Color { get { return color; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new AlphaInverseEffect(color.CloneTo(documentModel));
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			AlphaInverseEffect value = obj as AlphaInverseEffect;
			if (value == null)
				return false;
			return color.Equals(value.color);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ color.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region AlphaModulateEffect
	public class AlphaModulateEffect : IDrawingEffect {
		readonly ContainerEffect container;
		public AlphaModulateEffect(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.container = new ContainerEffect(documentModel);
		}
		public AlphaModulateEffect(ContainerEffect container) {
			Guard.ArgumentNotNull(container, "ContainerEffect");
			this.container = container;
		}
		public ContainerEffect Container { get { return container; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new AlphaModulateEffect((ContainerEffect)container.CloneTo(documentModel));
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			AlphaModulateEffect value = obj as AlphaModulateEffect;
			if (value == null)
				return false;
			return container.Equals(value.container);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ container.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region AlphaModulateFixedEffect
	public class AlphaModulateFixedEffect : IDrawingEffect {
		readonly int amount; 
		public AlphaModulateFixedEffect(int amount) {
			this.amount = amount;
		}
		public int Amount { get { return amount; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new AlphaModulateFixedEffect(amount);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			AlphaModulateFixedEffect value = obj as AlphaModulateFixedEffect;
			if (value == null)
				return false;
			return amount == value.amount;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ amount.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region AlphaOutsetEffect
	public class AlphaOutsetEffect : IDrawingEffect {
		readonly long radius; 
		public AlphaOutsetEffect() {
		}
		public AlphaOutsetEffect(long radius) {
			this.radius = radius;
		}
		public long Radius { get { return radius; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new AlphaOutsetEffect(radius);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			AlphaOutsetEffect value = obj as AlphaOutsetEffect;
			if (value == null)
				return false;
			return radius == value.radius;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ radius.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region AlphaReplaceEffect
	public class AlphaReplaceEffect : IDrawingEffect {
		readonly int alpha; 
		public AlphaReplaceEffect(int alpha) {
			this.alpha = alpha;
		}
		public int Alpha { get { return alpha; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new AlphaReplaceEffect(alpha);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			AlphaReplaceEffect value = obj as AlphaReplaceEffect;
			if (value == null)
				return false;
			return alpha == value.alpha;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ alpha.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region BiLevelEffect
	public class BiLevelEffect : IDrawingEffect {
		readonly int thresh; 
		public BiLevelEffect(int thresh) {
			this.thresh = thresh;
		}
		public int Thresh { get { return thresh; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new BiLevelEffect(thresh);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			BiLevelEffect value = obj as BiLevelEffect;
			if (value == null)
				return false;
			return thresh == value.thresh;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ thresh.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region BlendEffect
	public enum BlendMode {
		Overlay = 0,
		Multiply = 1,
		Screen = 2, 
		Darken = 3,
		Lighten = 4
	}
	public class BlendEffect : IDrawingEffect {
		#region Fields
		readonly BlendMode blendMode; 
		readonly ContainerEffect container;
		#endregion
		public BlendEffect(ContainerEffect container, BlendMode blendMode) {
			Guard.ArgumentNotNull(container, "DrawingEffectContainer");
			this.container = container;
			this.blendMode = blendMode;
		}
		public BlendEffect(IDocumentModel documentModel, BlendMode blendMode) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.container = new ContainerEffect(documentModel);
			this.blendMode = blendMode;
		}
		#region Properties
		public BlendMode BlendMode { get { return blendMode; } }
		public ContainerEffect Container { get { return container; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new BlendEffect((ContainerEffect)container.CloneTo(documentModel), blendMode);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			BlendEffect value = obj as BlendEffect;
			if (value == null)
				return false;
			return blendMode == value.blendMode && container.Equals(value.container);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ blendMode.GetHashCode() ^ container.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region BlurEffect
	public class BlurEffect : IDrawingEffect {
		#region Fields
		readonly long radius;	   
		readonly bool grow;		 
		#endregion
		public BlurEffect(long radius, bool grow) {
			this.radius = radius;
			this.grow = grow;
		}
		#region Properties
		public long Radius { get { return radius; } }
		public bool Grow { get { return grow; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new BlurEffect(radius, grow);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			BlurEffect value = obj as BlurEffect;
			if (value == null)
				return false;
			return grow == value.grow && radius == value.radius;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ grow.GetHashCode() ^ radius.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region ColorChangeEffect
	public class ColorChangeEffect : IDrawingEffect {
		#region Fields
		readonly DrawingColor colorFrom;
		readonly DrawingColor colorTo;
		bool useAlpha;				  
		#endregion
		public ColorChangeEffect(DrawingColor colorFrom, DrawingColor colorTo, bool useAlpha) {
			Guard.ArgumentNotNull(colorFrom, "ColorChange ColorFrom");
			Guard.ArgumentNotNull(colorTo, "ColorChange ColorTo");
			this.colorFrom = colorFrom;
			this.colorTo = colorTo;
			this.useAlpha = useAlpha;
		}
		public ColorChangeEffect(DrawingColor colorFrom, DrawingColor colorTo)
			: this(colorFrom, colorTo, true) {
		}
		#region Properties
		public DrawingColor ColorFrom { get { return colorFrom; } }
		public DrawingColor ColorTo { get { return colorTo; } }
		public bool UseAlpha { get { return useAlpha; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new ColorChangeEffect(colorFrom.CloneTo(documentModel), colorTo.CloneTo(documentModel), useAlpha);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			ColorChangeEffect value = obj as ColorChangeEffect;
			if (value == null)
				return false;
			return 
				useAlpha == value.useAlpha && 
				colorFrom.Equals(value.colorFrom) &&
				colorTo.Equals(value.colorTo);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ useAlpha.GetHashCode() ^ colorFrom.GetHashCode() ^ colorTo.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region ContainerEffect
	#region EffectContainerType
	public enum DrawingEffectContainerType {
		Sibling,
		Tree
	}
	#endregion
	public class ContainerEffect : IDrawingEffect, ISupportsCopyFrom<ContainerEffect> {
		#region Fields
		readonly DrawingEffectCollection effects;
		string name = String.Empty;				  
		DrawingEffectContainerType type;			 
		bool hasEffectsList = true;
		#endregion
		public ContainerEffect(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.effects = new DrawingEffectCollection(documentModel);
		}
		#region Properties
		public DrawingEffectCollection Effects { get { return effects; } }
		public string Name { get { return name; } set { SetName(value); } }
		public DrawingEffectContainerType Type { get { return type; } set { SetType(value); } }
		public bool HasEffectsList { get { return hasEffectsList; } set { SetHasEffectsList(value); } }
		public bool IsEmpty { get { return effects.Count == 0; } }
		IDocumentModel DocumentModel { get { return effects.DocumentModel; } }
		#endregion
		void SetName(string value) {
			if (name != value)
				ApplyHistoryItem(new DrawingContainerEffectNameChangedHistoryItem(DocumentModel.MainPart, this, name, value));
		}
		internal void SetNameCore(string value) {
			name = value;
		}
		void SetType(DrawingEffectContainerType value) {
			if (type != value)
				ApplyHistoryItem(new DrawingContainerEffectTypeChangedHistoryItem(DocumentModel.MainPart, this, type, value));
		}
		internal void SetTypeCore(DrawingEffectContainerType value) {
			type = value;
		}
		internal void ApplyEffects(IDrawingEffectVisitor visitor) {
			Effects.ApplyEffects(visitor); 
		}
		void SetHasEffectsList(bool value) {
			if (hasEffectsList != value)
				ApplyHistoryItem(new DrawingContainerEffectHasEffectsListChangedHistoryItem(DocumentModel.MainPart, this, hasEffectsList, value));
		}
		internal void SetHasEffectsListCore(bool value) {
			hasEffectsList = value;
		}
		void ApplyHistoryItem(HistoryItem item) { 
			DocumentModel.History.Add(item);
			item.Execute();
		} 
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			ContainerEffect result = new ContainerEffect(documentModel);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ContainerEffect> Members
		public void CopyFrom(ContainerEffect value) {
			effects.CopyFrom(value.effects);
			name = value.name;
			type = value.type;
			hasEffectsList = value.hasEffectsList;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			ContainerEffect value = obj as ContainerEffect;
			if (value == null)
				return false;
			return
				StringExtensions.CompareInvariantCultureIgnoreCase(name, value.name) == 0 && 
				type == value.type &&
				hasEffectsList == value.hasEffectsList &&
				effects.Equals(value.effects);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ name.GetHashCode() ^ type.GetHashCode() ^ effects.GetHashCode() ^ hasEffectsList.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region DuotoneEffect
	public class DuotoneEffect : IDrawingEffect {
		#region Fields
		readonly DrawingColor firstColor;
		readonly DrawingColor secondColor;
		#endregion
		public DuotoneEffect(DrawingColor firstColor, DrawingColor secondColor) {
			Guard.ArgumentNotNull(firstColor, "Duotone first color");
			Guard.ArgumentNotNull(secondColor, "Duotone second color");
			this.firstColor = firstColor;
			this.secondColor = secondColor;
		}
		#region Properties
		public DrawingColor FirstColor { get { return firstColor; } }
		public DrawingColor SecondColor { get { return secondColor; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new DuotoneEffect(firstColor.CloneTo(documentModel), secondColor.CloneTo(documentModel));
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DuotoneEffect value = obj as DuotoneEffect;
			if (value == null)
				return false;
			return firstColor.Equals(value.firstColor) && secondColor.Equals(value.secondColor);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ firstColor.GetHashCode() ^ secondColor.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region Effect
	public class Effect : IDrawingEffect {
		readonly string reference = String.Empty;
		public Effect(string reference) {
			this.reference = reference;
		}
		public string Reference { get { return reference; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new Effect(reference);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			Effect value = obj as Effect;
			if (value == null)
				return false;
			return StringExtensions.CompareInvariantCultureIgnoreCase(reference, value.reference) == 0;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ reference.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region FillEffect
	public class FillEffect : IDrawingEffect {
		readonly IDrawingFill fill;
		public FillEffect(IDrawingFill fill) {
			Guard.ArgumentNotNull(fill, "drawingFill");
			this.fill = fill;
		}
		public IDrawingFill Fill { get { return fill; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new FillEffect(fill.CloneTo(documentModel));
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			FillEffect value = obj as FillEffect;
			if (value == null)
				return false;
			return fill.Equals(value.fill);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ fill.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region FillOverlayEffect
	public class FillOverlayEffect : IDrawingEffect {
		#region Fields
		readonly IDrawingFill fill;
		readonly BlendMode blendMode;
		#endregion
		public FillOverlayEffect(IDrawingFill fill, BlendMode blendMode) {
			Guard.ArgumentNotNull(fill, "drawingFill");
			this.fill = fill;
			this.blendMode = blendMode;
		}
		#region Properties
		public BlendMode BlendMode { get { return blendMode; } }
		public IDrawingFill Fill { get { return fill; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new FillOverlayEffect(fill.CloneTo(documentModel), blendMode);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			FillOverlayEffect value = obj as FillOverlayEffect;
			if (value == null)
				return false;
			return fill.Equals(value.Fill) && blendMode == value.blendMode;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ fill.GetHashCode() ^ blendMode.GetHashCode(); 
		}
		#endregion
	}
	#endregion
	#region GlowEffect
	public class GlowEffect : IDrawingEffect {
		#region Fields
		readonly long radius;		   
		readonly DrawingColor color;
		#endregion
		public GlowEffect(DrawingColor color, long radius) {
			Guard.ArgumentNotNull(color, "Glow Color");
			this.color = color;
			this.radius = radius;
		}
		#region Properties
		public DrawingColor Color { get { return color; } }
		public long Radius { get { return radius; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new GlowEffect(color.CloneTo(documentModel), radius);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			GlowEffect value = obj as GlowEffect;
			if (value == null)
				return false;
			return radius == value.radius && color.Equals(value.color);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ radius.GetHashCode() ^ color.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region HSLEffect
	public class HSLEffect : IDrawingEffect {
		#region Fields
		readonly int hue;			   
		readonly int saturation;		
		readonly int luminance;		 
		#endregion
		public HSLEffect(int hue, int saturation, int luminance) {
			this.hue = hue;
			this.saturation = saturation;
			this.luminance = luminance;
		}
		#region Properties
		public int Hue { get { return hue; } }
		public int Saturation { get { return saturation; } }
		public int Luminance { get { return luminance; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new HSLEffect(hue, saturation, luminance);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			HSLEffect value = obj as HSLEffect;
			if (value == null)
				return false;
			return hue == value.hue && saturation == value.saturation && luminance == value.luminance;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ hue.GetHashCode() ^ saturation.GetHashCode() ^ luminance.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region InnerShadowEffect
	public class InnerShadowEffect : IDrawingEffect {
		#region Fields
		readonly DrawingColor color;
		readonly OffsetShadowInfo info;
		readonly long blurRadius;	   
		#endregion
		public InnerShadowEffect(DrawingColor color, OffsetShadowInfo info, long blurRadius) {
			Guard.ArgumentNotNull(color, "InnerShadowColor");
			Guard.ArgumentNotNull(info, "OffsetShadowInfo");
			this.color = color;
			this.info = info;
			this.blurRadius = blurRadius;
		}
		#region Properies
		public DrawingColor Color { get { return color; } }
		public OffsetShadowInfo OffsetShadow { get { return info; } }
		public long BlurRadius { get { return blurRadius; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new InnerShadowEffect(color.CloneTo(documentModel), info.Clone(), blurRadius);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			InnerShadowEffect value = obj as InnerShadowEffect;
			if (value == null)
				return false;
			return color.Equals(value.color) && blurRadius == value.blurRadius && info.Equals(value.info);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ color.GetHashCode() ^ blurRadius.GetHashCode() ^ info.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region LuminanceEffect
	public class LuminanceEffect : IDrawingEffect {
		#region Fields
		readonly int bright;	
		readonly int contrast;  
		#endregion
		public LuminanceEffect(int bright, int contrast) {
			this.bright = bright;
			this.contrast = contrast;
		}
		#region Properties
		public int Bright { get { return bright; } }
		public int Contrast { get { return contrast; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new LuminanceEffect(bright, contrast);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			LuminanceEffect value = obj as LuminanceEffect;
			if (value == null)
				return false;
			return
				bright == value.bright &&
				contrast == value.contrast;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ bright.GetHashCode() ^ contrast.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region OuterShadowEffect
	public class OuterShadowEffect : IDrawingEffect {
		#region Fields
		readonly DrawingColor color;
		readonly OuterShadowEffectInfo info;
		#endregion
		public OuterShadowEffect(DrawingColor color, OuterShadowEffectInfo info) {
			Guard.ArgumentNotNull(color, "OuterShadowColor");
			Guard.ArgumentNotNull(info, "OuterShadowEffectInfo");
			this.color = color;
			this.info = info;
		}
		#region Properies
		public DrawingColor Color { get { return color; } }
		public OuterShadowEffectInfo Info { get { return info; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new OuterShadowEffect(color.CloneTo(documentModel), info.Clone());
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			OuterShadowEffect value = obj as OuterShadowEffect;
			if (value == null)
				return false;
			return
				color.Equals(value.color) &&
				info.Equals(value.info);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ color.GetHashCode() ^ info.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region PresetShadowEffect
	#region PresetShadowType
	public enum PresetShadowType { 
		TopLeftDrop = 1, 
		TopRightDrop = 2, 
		BackLeftPerspective = 3, 
		BackRightPerspective = 4, 
		BottomLeftDrop = 5,  
		BottomRightDrop = 6, 
		FrontLeftPerspective = 7, 
		FrontRightPerspective = 8, 
		TopLeftSmallDrop = 9, 
		TopLeftLargeDrop = 10, 
		BackLeftLongPerspective = 11, 
		BackRightLongPerspective = 12, 
		TopLeftDoubleDrop = 13, 
		BottomRightSmallDrop = 14, 
		FrontLeftLongPerspective = 15, 
		FrontRightLongPerspective = 16,
		OuterBox3d = 17, 
		InnerBox3d = 18, 
		BackCenterPerspective = 19, 
		FrontBottomShadow = 20 
	}
	#endregion
	public class PresetShadowEffect : IDrawingEffect {
		#region Fields
		readonly DrawingColor color;
		readonly PresetShadowType type; 
		readonly OffsetShadowInfo info; 
		#endregion
		public PresetShadowEffect(DrawingColor color, PresetShadowType type, OffsetShadowInfo info) {
			Guard.ArgumentNotNull(color, "PresetShadowColor");
			Guard.ArgumentNotNull(info, "OffsetShadowInfo");
			this.color = color;
			this.type = type;
			this.info = info;
		}
		#region Properties
		public DrawingColor Color { get { return color; } }
		public PresetShadowType Type { get { return type; } }
		public OffsetShadowInfo OffsetShadow { get { return info; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new PresetShadowEffect(color.CloneTo(documentModel), type, info.Clone());
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			PresetShadowEffect value = obj as PresetShadowEffect;
			if (value == null)
				return false;
			return
				color.Equals(value.color) &&
				type == value.type &&
				info.Equals(value.info);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ color.GetHashCode() ^ type.GetHashCode() ^ info.GetHashCode();
		}
		#endregion
	}
	#endregion 
	#region ReflectionEffect
	public class ReflectionEffect : IDrawingEffect {
		#region Fields
		readonly OuterShadowEffectInfo outerShadowEffectInfo;
		readonly ReflectionOpacityInfo reflectionOpacityInfo;
		#endregion
		public ReflectionEffect(ReflectionOpacityInfo reflectionOpacityInfo, OuterShadowEffectInfo outerShadowEffectInfo) {
			Guard.ArgumentNotNull(reflectionOpacityInfo, "reflectionOpacityInfo");
			Guard.ArgumentNotNull(outerShadowEffectInfo, "OuterShadowEffectInfo");
			this.outerShadowEffectInfo = outerShadowEffectInfo;
			this.reflectionOpacityInfo = reflectionOpacityInfo;
		}
		#region Properies
		public OuterShadowEffectInfo OuterShadowEffectInfo { get { return outerShadowEffectInfo; } }
		public ReflectionOpacityInfo ReflectionOpacity { get { return reflectionOpacityInfo; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new ReflectionEffect(reflectionOpacityInfo.Clone(), outerShadowEffectInfo.Clone());
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			ReflectionEffect value = obj as ReflectionEffect;
			if (value == null)
				return false;
			return 
				outerShadowEffectInfo.Equals(value.outerShadowEffectInfo) &&
				reflectionOpacityInfo.Equals(value.reflectionOpacityInfo);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ outerShadowEffectInfo.GetHashCode() ^ reflectionOpacityInfo.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region RelativeOffsetEffect
	public class RelativeOffsetEffect : IDrawingEffect {
		#region Fields
		readonly int offsetX; 
		readonly int offsetY; 
		#endregion
		public RelativeOffsetEffect(int offsetX, int offsetY) {
			this.offsetX = offsetX;
			this.offsetY = offsetY;
		}
		#region Properties
		public int OffsetX { get { return offsetX; } }
		public int OffsetY { get { return offsetY; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new RelativeOffsetEffect(offsetX, offsetY);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			RelativeOffsetEffect value = obj as RelativeOffsetEffect;
			if (value == null)
				return false;
			return offsetX == value.offsetX && offsetY == value.offsetY;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ offsetX.GetHashCode() ^ offsetY.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region SoftEdgeEffect
	public class SoftEdgeEffect : IDrawingEffect {
		readonly long radius; 
		public SoftEdgeEffect(long radius) {
			this.radius = radius;
		}
		public long Radius { get { return radius; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new SoftEdgeEffect(radius);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			SoftEdgeEffect value = obj as SoftEdgeEffect;
			if (value == null)
				return false;
			return radius == value.radius;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ radius.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region SolidColorReplacementEffect
	public class SolidColorReplacementEffect : IDrawingEffect {
		readonly DrawingColor color;
		public SolidColorReplacementEffect(DrawingColor color) {
			Guard.ArgumentNotNull(color, "ClrRepl Color");
			this.color = color;
		}
		public DrawingColor Color { get { return color; } }
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new SolidColorReplacementEffect(color.CloneTo(documentModel));
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			SolidColorReplacementEffect value = obj as SolidColorReplacementEffect;
			if (value == null)
				return false;
			return color.Equals(value.color);
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ color.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region TintEffect
	public class TintEffect : IDrawingEffect {
		#region Fields
		int amount; 
		int hue;	
		#endregion
		public TintEffect(int hue, int amount) {
			this.amount = amount;
			this.hue = hue;
		}
		#region Properties
		public int Amount { get { return amount; } }
		public int Hue { get { return hue; } }
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new TintEffect(hue, amount);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			TintEffect value = obj as TintEffect;
			if (value == null)
				return false;
			return
				amount == value.amount &&
				hue == value.hue;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ amount.GetHashCode() ^ hue.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region TransformEffect
	public class TransformEffect : IDrawingEffect {
		#region Fields
		readonly ScalingFactorInfo scalingFactorInfo;
		readonly SkewAnglesInfo skewAnglesInfo;
		readonly CoordinateShiftInfo coordinateShiftInfo;
		#endregion
		public TransformEffect(ScalingFactorInfo scalingFactorInfo, SkewAnglesInfo skewAnglesInfo, CoordinateShiftInfo coordinateShiftInfo) {
			Guard.ArgumentNotNull(scalingFactorInfo, "scalingFactorInfo");
			Guard.ArgumentNotNull(skewAnglesInfo, "skewAnglesInfo");
			Guard.ArgumentNotNull(coordinateShiftInfo, "coordinateShiftInfo");
			this.scalingFactorInfo = scalingFactorInfo;
			this.skewAnglesInfo = skewAnglesInfo;
			this.coordinateShiftInfo = coordinateShiftInfo;
		}
		#region Properties
		public ScalingFactorInfo ScalingFactor { get { return scalingFactorInfo; } }
		public SkewAnglesInfo SkewAngles { get { return skewAnglesInfo; } }
		public CoordinateShiftInfo CoordinateShift { get { return coordinateShiftInfo; } }  
		#endregion
		#region IDrawingEffect Members
		public void Visit(IDrawingEffectVisitor visitor) {
			visitor.Visit(this);
		}
		public IDrawingEffect CloneTo(IDocumentModel documentModel) {
			return new TransformEffect(scalingFactorInfo.Clone(), skewAnglesInfo.Clone(), coordinateShiftInfo.Clone());
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			TransformEffect value = obj as TransformEffect;
			if (value == null)
				return false;
			return
				scalingFactorInfo.Equals(value.scalingFactorInfo) &&
				skewAnglesInfo.Equals(value.skewAnglesInfo) &&
				coordinateShiftInfo.Equals(value.coordinateShiftInfo);
		}
		public override int GetHashCode() {
			return 
				GetType().GetHashCode() ^ scalingFactorInfo.GetHashCode() ^ 
				skewAnglesInfo.GetHashCode() ^ coordinateShiftInfo.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region OffsetShadowInfo
	public class OffsetShadowInfo : ICloneable<OffsetShadowInfo> {
		#region Fields
		readonly int direction; 
		readonly long distance; 
		#endregion
		public OffsetShadowInfo(long distance, int direction) {
			this.direction = direction;
			this.distance = distance;
		}
		#region Properties
		public long Distance { get { return distance; } }
		public int Direction { get { return direction; } }
		#endregion
		#region ICloneable<OffsetShadowInfo> Members
		public OffsetShadowInfo Clone() {
			return new OffsetShadowInfo(distance, direction);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			OffsetShadowInfo value = obj as OffsetShadowInfo;
			if (value == null)
				return false;
			return distance == value.distance && direction == value.direction;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ distance.GetHashCode() ^ direction.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region CoordinateShiftInfo
	public class CoordinateShiftInfo : ICloneable<CoordinateShiftInfo> {
		#region Fields
		readonly long horizontal;	
		readonly long vertical;	  
		#endregion
		public CoordinateShiftInfo(long horizontal, long vertical) {
			this.horizontal = horizontal;
			this.vertical = vertical;
		}
		#region Properties
		public long Horizontal { get { return horizontal; } }
		public long Vertical { get { return vertical; } }
		#endregion
		#region ICloneable<CoordinateShiftInfo> Members
		public CoordinateShiftInfo Clone() {
			return new CoordinateShiftInfo(horizontal, vertical);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			CoordinateShiftInfo value = obj as CoordinateShiftInfo;
			if (value == null)
				return false;
			return horizontal == value.horizontal && vertical == value.vertical;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ horizontal.GetHashCode() ^ vertical.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region ScalingFactorInfo
	public class ScalingFactorInfo : ICloneable<ScalingFactorInfo> {
		#region Fields
		readonly int horizontal;	
		readonly int vertical;	  
		#endregion
		public ScalingFactorInfo(int horizontal, int vertical) {
			this.horizontal = horizontal;
			this.vertical = vertical;
		}
		#region Properties
		public int Horizontal { get { return horizontal; } }
		public int Vertical { get { return vertical; } }
		#endregion
		#region ICloneable<ScalingFactorInfo> Members
		public ScalingFactorInfo Clone() {
			return new ScalingFactorInfo(horizontal, vertical);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			ScalingFactorInfo value = obj as ScalingFactorInfo;
			if (value == null)
				return false;
			return horizontal == value.horizontal && vertical == value.vertical;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ horizontal.GetHashCode() ^ vertical.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region SkewAnglesInfo
	public class SkewAnglesInfo : ICloneable<SkewAnglesInfo> {
		#region Fields
		readonly int horizontal;	
		readonly int vertical;	  
		#endregion
		public SkewAnglesInfo(int horizontal, int vertical) {
			this.horizontal = horizontal;
			this.vertical = vertical;
		}
		#region Properties
		public int Horizontal { get { return horizontal; } }
		public int Vertical { get { return vertical; } }
		#endregion
		#region ICloneable<SkewAnglesInfo> Members
		public SkewAnglesInfo Clone() {
			return new SkewAnglesInfo(horizontal, vertical);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			SkewAnglesInfo value = obj as SkewAnglesInfo;
			if (value == null)
				return false;
			return horizontal == value.horizontal && vertical == value.vertical;
		}
		public override int GetHashCode() {
			return GetType().GetHashCode() ^ horizontal.GetHashCode() ^ vertical.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region OuterShadowEffectInfo
	public class OuterShadowEffectInfo : ICloneable<OuterShadowEffectInfo> {
		#region Static Members
		public static OuterShadowEffectInfo Create(long blurRadius, long dist, int dir, int sx, int sy, int kx, int ky, RectangleAlignType align, bool rotWithShape) {
			OffsetShadowInfo offsetShadowInfo = new OffsetShadowInfo(dist, dir);
			ScalingFactorInfo scalingFactorInfo = new ScalingFactorInfo(sx, sy);
			SkewAnglesInfo skewAnglesInfo = new SkewAnglesInfo(kx, ky);
			return new OuterShadowEffectInfo(offsetShadowInfo, scalingFactorInfo, skewAnglesInfo, align, blurRadius, rotWithShape);
		}
		#endregion
		#region Fields
		readonly OffsetShadowInfo offsetShadowInfo;
		readonly ScalingFactorInfo scalingFactorInfo;
		readonly SkewAnglesInfo skewAnglesInfo;
		readonly RectangleAlignType shadowAlignment;			 
		readonly long blurRadius;								
		readonly bool rotateWithShape;						   
		#endregion
		public OuterShadowEffectInfo(OffsetShadowInfo offsetShadowInfo, ScalingFactorInfo scalingFactorInfo,
			SkewAnglesInfo skewAnglesInfo, RectangleAlignType shadowAlignment,
			long blurRadius, bool rotateWithShape) {
			Guard.ArgumentNotNull(offsetShadowInfo, "offsetShadowInfo");
			Guard.ArgumentNotNull(scalingFactorInfo, "scalingFactorInfo");
			Guard.ArgumentNotNull(skewAnglesInfo, "skewAnglesInfo");
			this.offsetShadowInfo = offsetShadowInfo;
			this.scalingFactorInfo = scalingFactorInfo;
			this.shadowAlignment = shadowAlignment;
			this.skewAnglesInfo = skewAnglesInfo;
			this.blurRadius = blurRadius;
			this.rotateWithShape = rotateWithShape;
		}
		#region Properties
		public OffsetShadowInfo OffsetShadow { get { return offsetShadowInfo; } }
		public ScalingFactorInfo ScalingFactor { get { return scalingFactorInfo; } }
		public SkewAnglesInfo SkewAngles { get { return skewAnglesInfo; } }
		public RectangleAlignType ShadowAlignment { get { return shadowAlignment; } }
		public long BlurRadius { get { return blurRadius; } }
		public bool RotateWithShape { get { return rotateWithShape; } }
		#endregion
		#region ICloneable<OuterShadowEffectInfo> Members
		public OuterShadowEffectInfo Clone() {
			return new OuterShadowEffectInfo(offsetShadowInfo.Clone(), scalingFactorInfo.Clone(), skewAnglesInfo.Clone(), shadowAlignment, blurRadius, rotateWithShape);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			OuterShadowEffectInfo value = obj as OuterShadowEffectInfo;
			if (value == null)
				return false;
			return
				offsetShadowInfo.Equals(value.offsetShadowInfo) &&
				scalingFactorInfo.Equals(value.scalingFactorInfo) &&
				skewAnglesInfo.Equals(value.skewAnglesInfo) &&
				shadowAlignment == value.shadowAlignment &&
				blurRadius == value.blurRadius &&
				rotateWithShape == value.rotateWithShape;
		}
		public override int GetHashCode() {
			return
				GetType().GetHashCode() ^ offsetShadowInfo.GetHashCode() ^ scalingFactorInfo.GetHashCode() ^
				skewAnglesInfo.GetHashCode() ^ shadowAlignment.GetHashCode() ^ blurRadius.GetHashCode() ^
				rotateWithShape.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region ReflectionOpacityInfo
	public class ReflectionOpacityInfo : ICloneable<ReflectionOpacityInfo> {
		#region Fields
		readonly int startOpacity;	
		readonly int startPosition;   
		readonly int endOpacity;	  
		readonly int endPosition;	 
		readonly int fadeDirection;   
		#endregion
		public ReflectionOpacityInfo(int startOpacity, int startPosition, int endOpacity, int endPosition, int fadeDirection) {
			this.startOpacity = startOpacity;
			this.endOpacity = endOpacity;
			this.startPosition = startPosition;
			this.endPosition = endPosition;
			this.fadeDirection = fadeDirection;
		}
		#region Properties
		public int StartOpacity { get { return startOpacity; } }
		public int EndOpacity { get { return endOpacity; } }
		public int StartPosition { get { return startPosition; } }
		public int EndPosition { get { return endPosition; } }
		public int FadeDirection { get { return fadeDirection; } }
		#endregion
		#region ICloneable<ReflectionOpacityInfo> Members
		public ReflectionOpacityInfo Clone() {
			return new ReflectionOpacityInfo(startOpacity, startPosition, endOpacity, endPosition, fadeDirection);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			ReflectionOpacityInfo value = obj as ReflectionOpacityInfo;
			if (value == null)
				return false;
			return
				startOpacity == value.startOpacity &&
				endOpacity == value.endOpacity &&
				startPosition == value.startPosition &&
				endPosition == value.endPosition &&
				fadeDirection == value.fadeDirection;
		}
		public override int GetHashCode() {
			return
				GetType().GetHashCode() ^ startOpacity.GetHashCode() ^ endOpacity.GetHashCode() ^
				startPosition.GetHashCode() ^ endPosition.GetHashCode() ^ fadeDirection.GetHashCode();
		}
		#endregion
	}
	#endregion
	#endregion
}

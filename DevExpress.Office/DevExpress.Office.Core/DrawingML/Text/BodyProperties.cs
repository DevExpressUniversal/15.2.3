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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using DevExpress.Office.DrawingML;
namespace DevExpress.Office.Drawing {
	#region DrawingText3DType
	public enum DrawingText3DType {
		Automatic,
		Shape3D,
		FlatText
	}
	#endregion
	#region DrawingTextBodyInfo
	public class DrawingTextBodyInfo : ICloneable<DrawingTextBodyInfo>, ISupportsCopyFrom<DrawingTextBodyInfo>, ISupportsSizeOf {
		readonly static DrawingTextBodyInfo defaultInfo = new DrawingTextBodyInfo();
		public static DrawingTextBodyInfo DefaultInfo { get { return defaultInfo; } }
		#region Fields
		#region PackedValues
		const uint maskAnchor =				0x00007;	  
		const uint maskHorizontalOverflow =	0x00008;	  
		const uint maskVerticalOverflow =	  0x00030;	  
		const uint maskTextWrapping =		  0x00040;	  
		const uint maskAnchorCenter =		  0x00080;	  
		const uint maskCompatibleLineSpacing = 0x00100;	  
		const uint maskForceAntiAlias =		0x00200;	  
		const uint maskFromWordArt =		   0x00400;	  
		const uint maskRightToLeftColumns =	0x00800;	  
		const uint maskParagraphSpacing =	  0x01000;	  
		const uint maskUprightText =		   0x02000;	  
		const uint maskVerticalText =		  0x1C000;	  
		#endregion
		#region PackedOptionsValues
		const uint maskHasRotation =			  0x00001;   
		const uint maskHasParagraphSpacing =	  0x00002;   
		const uint maskHasVerticalOverflow =	  0x00004;   
		const uint maskHasHorizontalOverflow =	0x00008;   
		const uint maskHasVerticalText =		  0x00010;   
		const uint maskHasTextWrapping =		  0x00020;   
		const uint maskHasNumberOfColumns =	   0x00040;   
		const uint maskHasSpaceBetweenColumns =   0x00080;   
		const uint maskHasRightToLeftColumns =	0x00100;   
		const uint maskHasFromWordArt =		   0x00200;   
		const uint maskHasAnchor =				0x00400;   
		const uint maskHasAnchorCenter =		  0x00800;   
		const uint maskHasForceAntiAlias =		0x01000;   
		const uint maskHasCompatibleLineSpacing = 0x02000;   
		#endregion
		uint packedValues = 0x0406D;
		uint packedOptionsValues;
		int numberOfColumns = 1;
		int rotation = 0;
		int spaceBetweenColumns = 0;
		#endregion
		#region Properties
		#region InPackedValues
		public DrawingTextAnchoringType Anchor {
			get { return (DrawingTextAnchoringType)PackedValues.GetIntBitValue(this.packedValues, maskAnchor, 0); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskAnchor, 0, (int)value); }
		}
		public DrawingTextHorizontalOverflowType HorizontalOverflow {
			get { return (DrawingTextHorizontalOverflowType)PackedValues.GetIntBitValue(this.packedValues, maskHorizontalOverflow, 3); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskHorizontalOverflow, 3, (int)value); }
		}
		public DrawingTextVerticalOverflowType VerticalOverflow {
			get { return (DrawingTextVerticalOverflowType)PackedValues.GetIntBitValue(this.packedValues, maskVerticalOverflow, 4); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskVerticalOverflow, 4, (int)value); }
		}
		public DrawingTextWrappingType TextWrapping {
			get { return (DrawingTextWrappingType)PackedValues.GetIntBitValue(this.packedValues, maskTextWrapping, 6); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskTextWrapping, 6, (int)value); }
		}
		public bool AnchorCenter {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskAnchorCenter); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskAnchorCenter, value); }
		}
		public bool CompatibleLineSpacing {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskCompatibleLineSpacing); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskCompatibleLineSpacing, value); }
		}
		public bool ForceAntiAlias {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskForceAntiAlias); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskForceAntiAlias, value); }
		}
		public bool FromWordArt {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskFromWordArt); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskFromWordArt, value); }
		}
		public bool RightToLeftColumns {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskRightToLeftColumns); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskRightToLeftColumns, value); }
		}
		public bool ParagraphSpacing {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskParagraphSpacing); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskParagraphSpacing, value); }
		}
		public bool UprightText {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskUprightText); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskUprightText, value); }
		}
		public DrawingTextVerticalTextType VerticalText {
			get { return (DrawingTextVerticalTextType)PackedValues.GetIntBitValue(this.packedValues, maskVerticalText, 14); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskVerticalText, 14, (int)value); }
		}
		#endregion
		#region InPackedOptionsValues
		public bool HasRotation {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasRotation); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasRotation, value); }
		}
		public bool HasParagraphSpacing {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasParagraphSpacing); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasParagraphSpacing, value); }
		}
		public bool HasVerticalOverflow {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasVerticalOverflow); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasVerticalOverflow, value); }
		}
		public bool HasHorizontalOverflow {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasHorizontalOverflow); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasHorizontalOverflow, value); }
		}
		public bool HasVerticalText {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasVerticalText); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasVerticalText, value); }
		}
		public bool HasTextWrapping {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasTextWrapping); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasTextWrapping, value); }
		}
		public bool HasNumberOfColumns {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasNumberOfColumns); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasNumberOfColumns, value); }
		}
		public bool HasSpaceBetweenColumns {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasSpaceBetweenColumns); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasSpaceBetweenColumns, value); }
		}
		public bool HasRightToLeftColumns {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasRightToLeftColumns); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasRightToLeftColumns, value); }
		}
		public bool HasFromWordArt {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasFromWordArt); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasFromWordArt, value); }
		}
		public bool HasAnchor {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasAnchor); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasAnchor, value); }
		}
		public bool HasAnchorCenter {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasAnchorCenter); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasAnchorCenter, value); }
		}
		public bool HasForceAntiAlias {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasForceAntiAlias); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasForceAntiAlias, value); }
		}
		public bool HasCompatibleLineSpacing {
			get { return PackedValues.GetBoolBitValue(this.packedOptionsValues, maskHasCompatibleLineSpacing); }
			set { PackedValues.SetBoolBitValue(ref this.packedOptionsValues, maskHasCompatibleLineSpacing, value); }
		}
		#endregion
		public int NumberOfColumns {
			get { return numberOfColumns; }
			set {
				ValueChecker.CheckValue(value, 1, 16, "NumberOfColumns");
				numberOfColumns = value;
			}
		}
		public int Rotation { get { return rotation; } set { rotation = value; } }
		public int SpaceBetweenColumns {
			get { return spaceBetweenColumns; }
			set {
				DrawingValueChecker.CheckPositiveCoordinate32(spaceBetweenColumns, "SpaceBetweenColumns");
				spaceBetweenColumns = value;
			}
		}
		#endregion
		#region ICloneable<DrawingTextBodyPropertiesInfo> Members
		public DrawingTextBodyInfo Clone() {
			DrawingTextBodyInfo result = new DrawingTextBodyInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextBodyPropertiesInfo> Members
		public void CopyFrom(DrawingTextBodyInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.packedOptionsValues = value.packedOptionsValues;
			this.numberOfColumns = value.numberOfColumns;
			this.rotation = value.rotation;
			this.spaceBetweenColumns = value.spaceBetweenColumns;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingTextBodyInfo other = obj as DrawingTextBodyInfo;
			if (other == null)
				return false;
			return 
				this.packedValues == other.packedValues &&
				this.packedOptionsValues == other.packedOptionsValues &&
				this.numberOfColumns == other.numberOfColumns &&
				this.rotation == other.rotation &&
				this.spaceBetweenColumns == other.spaceBetweenColumns;
		}
		public override int GetHashCode() {
			return (int)this.packedValues ^ (int)this.packedOptionsValues ^ numberOfColumns ^ rotation ^ spaceBetweenColumns;
		}
	}
	#endregion
	#region DrawingTextBodyInfoCache
	public class DrawingTextBodyInfoCache : UniqueItemsCache<DrawingTextBodyInfo> {
		public const int DefaultItemIndex = 0;
		public DrawingTextBodyInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingTextBodyInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DrawingTextBodyInfo();
		}
	}
	#endregion
	#region ITextInset
	public interface ITextInset : ITextInsetOptions {
		ITextInsetOptions Options { get; }
		int Left { get; set; }
		int Right { get; set; }
		int Top { get; set; }
		int Bottom { get; set; }
	}
	#endregion
	#region ITextInsetOptions
	public interface ITextInsetOptions {
		bool HasLeft { get; }
		bool HasRight { get; }
		bool HasTop { get; }
		bool HasBottom { get; }
	}
	#endregion
	#region DrawingTextInset
	public class DrawingTextInset : ISupportsCopyFrom<DrawingTextInset>, ITextInset {
		#region Fields
		const int LeftIndex = 0;
		const int RightIndex = 1;
		const int TopIndex = 2;
		const int BottomIndex = 3;
		public const int DefaultTopBottom = 72;
		public const int DefaultLeftRight = 144;
		readonly IDocumentModel documentModel;
		readonly int[] inset;
		readonly bool[] hasValues;
		#endregion
		public DrawingTextInset(IDocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.inset = new int[4] { DefaultLeftRight, DefaultLeftRight, DefaultTopBottom, DefaultTopBottom};
			this.hasValues = new bool[4] { false, false, false, false };
		}
		#region Properties
		public IDocumentModel DocumentModel { get { return documentModel; } }
		public int Left { get { return inset[LeftIndex]; } set { SetInset(LeftIndex, value); } }
		public int Right { get { return inset[RightIndex]; } set { SetInset(RightIndex, value); } }
		public int Top { get { return inset[TopIndex]; } set { SetInset(TopIndex, value); } }
		public int Bottom { get { return inset[BottomIndex]; } set { SetInset(BottomIndex, value); } }
		public ITextInsetOptions Options { get { return this; } }
		bool ITextInsetOptions.HasLeft { get { return hasValues[LeftIndex]; } }
		bool ITextInsetOptions.HasRight { get { return hasValues[RightIndex]; } }
		bool ITextInsetOptions.HasTop { get { return hasValues[TopIndex]; } }
		bool ITextInsetOptions.HasBottom { get { return hasValues[BottomIndex]; } }
		public bool IsDefault { get { return !hasValues[LeftIndex] && !hasValues[RightIndex] && !hasValues[TopIndex] && !hasValues[BottomIndex]; } }
		#endregion
		public void SetInsetCore(int index, int value) {
			inset[index] = value;
		}
		public void SetInsetHasValuesCore(int index, bool value) {
			hasValues[index] = value;
		}
		void SetInset(int index, int value) {
			if (inset[index] == value && hasValues[index])
				return;
			if (inset[index] == value) {
				ApplyHistoryItem(new DrawingTextInsetHasValuesChangeHistoryItem(this, index, hasValues[index], true));
				return;
			}
			DrawingValueChecker.CheckCoordinate32(inset[index], "InsetCoordinate");
			DocumentModel.History.BeginTransaction();
			ApplyHistoryItem(new DrawingTextInsetHasValuesChangeHistoryItem(this, index, hasValues[index], true));
			ApplyHistoryItem(new DrawingTextInsetChangeHistoryItem(this, index, inset[index], value));
			DocumentModel.History.EndTransaction();
		}
		void SetInset(int index, int value, bool hasValue) {
			if (hasValues[index] != hasValue)
				ApplyHistoryItem(new DrawingTextInsetHasValuesChangeHistoryItem(this, index, hasValues[index], hasValue));
			if (inset[index] != value)
				ApplyHistoryItem(new DrawingTextInsetChangeHistoryItem(this, index, inset[index], value));
		}
		void ApplyHistoryItem(HistoryItem item) {
			DocumentModel.History.Add(item);
			item.Execute();
		}
		#region ISupportsCopyFrom<DrawingTextInset>
		public void CopyFrom(DrawingTextInset value) {
			SetInset(LeftIndex, value.Left, value.Options.HasLeft);
			SetInset(RightIndex, value.Right, value.Options.HasRight);
			SetInset(TopIndex, value.Top, value.Options.HasTop);
			SetInset(BottomIndex, value.Bottom, value.Options.HasBottom);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingTextInset value = obj as DrawingTextInset;
			if (value == null)
				return false;
			return
				this.inset[LeftIndex] == value.Left && 
				this.inset[RightIndex] == value.Right && 
				this.inset[TopIndex] == value.Top && 
				this.inset[BottomIndex] == value.Bottom &&
				this.hasValues[LeftIndex] == value.Options.HasLeft &&
				this.hasValues[RightIndex] == value.Options.HasRight &&
				this.hasValues[TopIndex] == value.Options.HasTop &&
				this.hasValues[BottomIndex] == value.Options.HasBottom;
		}
		public override int GetHashCode() {
			return 
				inset[LeftIndex].GetHashCode() ^ inset[RightIndex].GetHashCode() ^ inset[TopIndex].GetHashCode() ^ inset[BottomIndex].GetHashCode() ^
				hasValues[LeftIndex].GetHashCode() ^ hasValues[RightIndex].GetHashCode() ^ hasValues[TopIndex].GetHashCode() ^ hasValues[BottomIndex].GetHashCode();
		}
		#endregion
		public void ResetToStyle() {
			SetInset(LeftIndex, DefaultLeftRight, false);
			SetInset(RightIndex, DefaultLeftRight, false);
			SetInset(TopIndex, DefaultTopBottom, false);
			SetInset(BottomIndex, DefaultTopBottom, false);
		}
	}
	#endregion
	#region ITextBodyOptions
	public interface ITextBodyOptions {
		bool HasRotation { get; }
		bool HasParagraphSpacing { get; }
		bool HasVerticalOverflow { get; }
		bool HasHorizontalOverflow { get; }
		bool HasVerticalText { get; }
		bool HasTextWrapping { get; }
		bool HasNumberOfColumns { get; }
		bool HasSpaceBetweenColumns { get; }
		bool HasRightToLeftColumns { get; }
		bool HasFromWordArt { get; }
		bool HasAnchor { get; }
		bool HasAnchorCenter { get; }
		bool HasForceAntiAlias { get; }
		bool HasCompatibleLineSpacing { get; }
	}
	#endregion
	#region DrawingTextBodyProperties
	public class DrawingTextBodyProperties : DrawingUndoableIndexBasedObject<DrawingTextBodyInfo>, ICloneable<DrawingTextBodyProperties>, ISupportsCopyFrom<DrawingTextBodyProperties>, ITextBodyOptions {
		readonly DrawingTextInset inset;
		readonly Scene3DProperties scene3d;
		IDrawingText3D text3D;
		IDrawingTextAutoFit autoFit;
		public DrawingTextBodyProperties(IDocumentModel documentModel) 
			: base(documentModel.MainPart) {
			this.inset = new DrawingTextInset(documentModel);
			this.scene3d = new Scene3DProperties(documentModel);
			this.autoFit = DrawingTextAutoFit.Automatic;
			this.text3D = DrawingText3D.Automatic;
		}
		#region Properties
		public DrawingTextInset Inset { get { return inset; } }
		public Scene3DProperties Scene3D { get { return scene3d; } }
		public ITextBodyOptions Options { get { return this; } }
		#region AutoFit
		public IDrawingTextAutoFit AutoFit {
			get { return autoFit; }
			set {
				if (value == null)
					value = DrawingTextAutoFit.Automatic;
				if (!IsValidAutoFit(value))
					throw new ArgumentException("Wrong autofit type.");
				if (autoFit.Equals(value))
					return;
				SetAutoFit(value);
			}
		}
		void SetAutoFit(IDrawingTextAutoFit value) {
			DrawingTextBodyPropertiesAutoFitChangedHistoryItem historyItem = new DrawingTextBodyPropertiesAutoFitChangedHistoryItem(this, autoFit, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetAutoFitCore(IDrawingTextAutoFit value) {
			autoFit = value;
			InvalidateParent();
		}
		bool IsValidAutoFit(IDrawingTextAutoFit value) {
			return value.Type == DrawingTextAutoFitType.None ||
				value.Type == DrawingTextAutoFitType.Normal ||
				value.Type == DrawingTextAutoFitType.Shape ||
				value.Type == DrawingTextAutoFitType.Automatic;
		}
		#endregion
		#region Anchor
		public DrawingTextAnchoringType Anchor {
			get { return Info.Anchor; }
			set {
				if (Anchor == value && Options.HasAnchor)
					return;
				SetPropertyValue(SetAnchorCore, value);
			}
		}
		DocumentModelChangeActions SetAnchorCore(DrawingTextBodyInfo info, DrawingTextAnchoringType value) {
			info.Anchor = value;
			info.HasAnchor = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HorizontalOverflow
		public DrawingTextHorizontalOverflowType HorizontalOverflow {
			get { return Info.HorizontalOverflow; }
			set {
				if (HorizontalOverflow == value && Options.HasHorizontalOverflow)
					return;
				SetPropertyValue(SetHorizontalOverflowCore, value);
			}
		}
		DocumentModelChangeActions SetHorizontalOverflowCore(DrawingTextBodyInfo info, DrawingTextHorizontalOverflowType value) {
			info.HorizontalOverflow = value;
			info.HasHorizontalOverflow = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region VerticalOverflow
		public DrawingTextVerticalOverflowType VerticalOverflow {
			get { return Info.VerticalOverflow; }
			set {
				if (VerticalOverflow == value && Options.HasVerticalOverflow)
					return;
				SetPropertyValue(SetVerticalOverflowCore, value);
			}
		}
		DocumentModelChangeActions SetVerticalOverflowCore(DrawingTextBodyInfo info, DrawingTextVerticalOverflowType value) {
			info.VerticalOverflow = value;
			info.HasVerticalOverflow = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region TextWrapping
		public DrawingTextWrappingType TextWrapping {
			get { return Info.TextWrapping; }
			set {
				if (TextWrapping == value && Options.HasTextWrapping)
					return;
				SetPropertyValue(SetTextWrappingCore, value);
			}
		}
		DocumentModelChangeActions SetTextWrappingCore(DrawingTextBodyInfo info, DrawingTextWrappingType value) {
			info.TextWrapping = value;
			info.HasTextWrapping = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region VerticalText
		public DrawingTextVerticalTextType VerticalText {
			get { return Info.VerticalText; }
			set {
				if (VerticalText == value && Options.HasVerticalText)
					return;
				SetPropertyValue(SetVerticalTextCore, value);
			}
		}
		DocumentModelChangeActions SetVerticalTextCore(DrawingTextBodyInfo info, DrawingTextVerticalTextType value) {
			info.VerticalText = value;
			info.HasVerticalText = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AnchorCenter
		public bool AnchorCenter {
			get { return Info.AnchorCenter; }
			set {
				if (AnchorCenter == value && Options.HasAnchorCenter)
					return;
				SetPropertyValue(SetAnchorCenterCore, value);
			}
		}
		DocumentModelChangeActions SetAnchorCenterCore(DrawingTextBodyInfo info, bool value) {
			info.AnchorCenter = value;
			info.HasAnchorCenter = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region CompatibleLineSpacing
		public bool CompatibleLineSpacing {
			get { return Info.CompatibleLineSpacing; }
			set {
				if (CompatibleLineSpacing == value && Options.HasCompatibleLineSpacing)
					return;
				SetPropertyValue(SetCompatibleLineSpacingCore, value);
			}
		}
		DocumentModelChangeActions SetCompatibleLineSpacingCore(DrawingTextBodyInfo info, bool value) {
			info.CompatibleLineSpacing = value;
			info.HasCompatibleLineSpacing = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ForceAntiAlias
		public bool ForceAntiAlias {
			get { return Info.ForceAntiAlias; }
			set {
				if (ForceAntiAlias == value && Options.HasForceAntiAlias)
					return;
				SetPropertyValue(SetForceAntiAliasCore, value);
			}
		}
		DocumentModelChangeActions SetForceAntiAliasCore(DrawingTextBodyInfo info, bool value) {
			info.ForceAntiAlias = value;
			info.HasForceAntiAlias = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FromWordArt
		public bool FromWordArt {
			get { return Info.FromWordArt; }
			set {
				if (FromWordArt == value && Options.HasFromWordArt)
					return;
				SetPropertyValue(SetFromWordArtCore, value);
			}
		}
		DocumentModelChangeActions SetFromWordArtCore(DrawingTextBodyInfo info, bool value) {
			info.FromWordArt = value;
			info.HasFromWordArt = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region RightToLeftColumns
		public bool RightToLeftColumns {
			get { return Info.RightToLeftColumns; }
			set {
				if (RightToLeftColumns == value && Options.HasRightToLeftColumns)
					return;
				SetPropertyValue(SetRightToLeftColumnsCore, value);
			}
		}
		DocumentModelChangeActions SetRightToLeftColumnsCore(DrawingTextBodyInfo info, bool value) {
			info.RightToLeftColumns = value;
			info.HasRightToLeftColumns = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ParagraphSpacing
		public bool ParagraphSpacing {
			get { return Info.ParagraphSpacing; }
			set {
				if (ParagraphSpacing == value && Options.HasParagraphSpacing)
					return;
				SetPropertyValue(SetParagraphSpacingCore, value);
			}
		}
		DocumentModelChangeActions SetParagraphSpacingCore(DrawingTextBodyInfo info, bool value) {
			info.ParagraphSpacing = value;
			info.HasParagraphSpacing = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region UprightText
		public bool UprightText {
			get { return Info.UprightText; }
			set {
				if (UprightText == value)
					return;
				SetPropertyValue(SetUprightTextCore, value);
			}
		}
		DocumentModelChangeActions SetUprightTextCore(DrawingTextBodyInfo info, bool value) {
			info.UprightText = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region NumberOfColumns
		public int NumberOfColumns {
			get { return Info.NumberOfColumns; }
			set {
				if (NumberOfColumns == value && Options.HasNumberOfColumns)
					return;
				SetPropertyValue(SetNumberOfColumnsCore, value);
			}
		}
		DocumentModelChangeActions SetNumberOfColumnsCore(DrawingTextBodyInfo info, int value) {
			info.NumberOfColumns = value;
			info.HasNumberOfColumns = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Rotation
		public int Rotation {
			get { return Info.Rotation; }
			set {
				if (Rotation == value && Options.HasRotation)
					return;
				SetPropertyValue(SetRotationCore, value);
			}
		}
		DocumentModelChangeActions SetRotationCore(DrawingTextBodyInfo info, int value) {
			info.Rotation = value;
			info.HasRotation = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SpaceBetweenColumns
		public int SpaceBetweenColumns {
			get { return Info.SpaceBetweenColumns; }
			set {
				if (SpaceBetweenColumns == value && Options.HasSpaceBetweenColumns)
					return;
				SetPropertyValue(SetSpaceBetweenColumnsCore, value);
			}
		}
		DocumentModelChangeActions SetSpaceBetweenColumnsCore(DrawingTextBodyInfo info, int value) {
			info.SpaceBetweenColumns = value;
			info.HasSpaceBetweenColumns = true;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region IDrawingText3D
		public IDrawingText3D Text3D {
			get { return text3D; }
			set {
				if (value == null)
					value = DrawingText3D.Automatic;
				if (!IsValidText3D(value))
					throw new ArgumentException("Wrong text3d type.");
				if (text3D == value)
					return;
				DrawingTextBodyPropertiesText3DChangedHistoryItem historyItem = new DrawingTextBodyPropertiesText3DChangedHistoryItem(this, text3D, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		protected internal void SetText3DCore(IDrawingText3D value) {
			this.text3D = value;
			InvalidateParent();
		}
		bool IsValidText3D(IDrawingText3D value) {
			return value.Type == DrawingText3DType.Automatic ||
				value.Type == DrawingText3DType.Shape3D ||
				value.Type == DrawingText3DType.FlatText;
		}
		#endregion
		#region ITextBodyOptions Members
		bool ITextBodyOptions.HasRotation { get { return Info.HasRotation; } }
		bool ITextBodyOptions.HasParagraphSpacing { get { return Info.HasParagraphSpacing; } }
		bool ITextBodyOptions.HasVerticalOverflow { get { return Info.HasVerticalOverflow; } }
		bool ITextBodyOptions.HasHorizontalOverflow { get { return Info.HasHorizontalOverflow; } }
		bool ITextBodyOptions.HasVerticalText { get { return Info.HasVerticalText; } }
		bool ITextBodyOptions.HasTextWrapping { get { return Info.HasTextWrapping; } }
		bool ITextBodyOptions.HasNumberOfColumns { get { return Info.HasNumberOfColumns; } }
		bool ITextBodyOptions.HasSpaceBetweenColumns { get { return Info.HasSpaceBetweenColumns; } }
		bool ITextBodyOptions.HasRightToLeftColumns { get { return Info.HasRightToLeftColumns; } }
		bool ITextBodyOptions.HasFromWordArt { get { return Info.HasFromWordArt; } }
		bool ITextBodyOptions.HasAnchor { get { return Info.HasAnchor; } }
		bool ITextBodyOptions.HasAnchorCenter { get { return Info.HasAnchorCenter; } }
		bool ITextBodyOptions.HasForceAntiAlias { get { return Info.HasForceAntiAlias; } }
		bool ITextBodyOptions.HasCompatibleLineSpacing { get { return Info.HasCompatibleLineSpacing; } }
		#endregion
		public bool IsDefault {
			get {
				return
					Index == DrawingTextBodyInfoCache.DefaultItemIndex &&
					inset.IsDefault && scene3d.IsDefault && text3D.Type == DrawingText3DType.Automatic &&
					autoFit.Type == DrawingTextAutoFitType.Automatic;
			}
		}
		#endregion
		#region ISupportsCopyFrom<DrawingTextBodyProperties> Members
		public void CopyFrom(DrawingTextBodyProperties value) {
			Guard.ArgumentNotNull(value, "value");
			base.CopyFrom(value);
			this.inset.CopyFrom(value.inset);
			this.scene3d.CopyFrom(value.scene3d);
			this.autoFit = value.autoFit.CloneTo(DocumentModel);
			this.text3D = value.text3D.CloneTo(DocumentModel);
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingTextBodyProperties other = obj as DrawingTextBodyProperties;
			if (other == null)
				return false;
			return 
				Info.Equals(other.Info) && inset.Equals(other.inset) && scene3d.Equals(other.scene3d) &&
				autoFit.Equals(other.autoFit) && text3D.Equals(other.text3D);
		}
		public override int GetHashCode() {
			return Info.GetHashCode() ^ inset.GetHashCode() ^ scene3d.GetHashCode() ^ autoFit.GetHashCode() ^ text3D.GetHashCode();
		}
		protected internal override UniqueItemsCache<DrawingTextBodyInfo> GetCache(IDocumentModel documentModel) {
			return DrawingCache.DrawingTextBodyInfoCache;
		}
		public DrawingTextBodyProperties Clone() {
			DrawingTextBodyProperties result = new DrawingTextBodyProperties(DocumentModel);
			result.CopyFrom(this);
			return result;
		}
		public void ResetToStyle() {
			if (IsUpdateLocked)
				Info.CopyFrom(DrawingCache.DrawingTextBodyInfoCache.DefaultItem);
			else
				ChangeIndexCore(DrawingTextBodyInfoCache.DefaultItemIndex, DocumentModelChangeActions.None);
			this.inset.ResetToStyle();
			this.scene3d.ResetToStyle();
			AutoFit = DrawingTextAutoFit.Automatic;
			Text3D = DrawingText3D.Automatic;
		}
	}
	#endregion
	#region IDrawingText3D
	public interface IDrawingText3D {
		DrawingText3DType Type { get; }
		IDrawingText3D CloneTo(IDocumentModel documentModel);
		void Visit(IDrawingText3DVisitor visitor);
	}
	#endregion
	#region IDrawingText3DVisitor
	public interface IDrawingText3DVisitor {
		void Visit(Shape3DProperties text3d);
		void Visit(DrawingText3DFlatText text3d);
	}
	#endregion
	#region DrawingText3D (Automatic)
	public sealed class DrawingText3D : IDrawingText3D {
		public static DrawingText3D Automatic = new DrawingText3D();
		DrawingText3D() {
		}
		public DrawingText3DType Type { get { return DrawingText3DType.Automatic; } }
		public override bool Equals(object obj) {
			return obj is DrawingText3D;
		}
		public override int GetHashCode() {
			return (int)DrawingText3DType.Automatic;
		}
		public IDrawingText3D CloneTo(IDocumentModel documentModel) {
			return Automatic;
		}
		public void Visit(IDrawingText3DVisitor visitor) {
		}
	}
	#endregion
	#region DrawingText3DFlatText
	public class DrawingText3DFlatText : IDrawingText3D {
		readonly long zCoordinate;
		public DrawingText3DFlatText(long zCoordinate) {
			this.zCoordinate = zCoordinate;
		}
		public long ZCoordinate { get { return zCoordinate; } }
		public DrawingText3DType Type { get { return DrawingText3DType.FlatText; } }
		public override bool Equals(object obj) {
			DrawingText3DFlatText other = obj as DrawingText3DFlatText;
			return other != null && zCoordinate == other.zCoordinate;
		}
		public override int GetHashCode() {
			return zCoordinate.GetHashCode();
		}
		public DrawingText3DFlatText Clone() {
			return new DrawingText3DFlatText(zCoordinate);
		}
		#region IDrawingText3D Members
		public IDrawingText3D CloneTo(IDocumentModel documentModel) {
			return new DrawingText3DFlatText(zCoordinate);
		}
		public void Visit(IDrawingText3DVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
	}
	#endregion
}

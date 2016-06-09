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

using System.Drawing;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region FillInfo
	public class FillInfo : ICloneable<FillInfo>, ISupportsCopyFrom<FillInfo>, ISupportsSizeOf {
		#region Fields
		XlPatternType patternType;
		int foreColorIndex;
		int backColorIndex;
		#endregion
		#region Properties
		public XlPatternType PatternType { get { return patternType; } set { patternType = value; } }
		public int ForeColorIndex { get { return foreColorIndex; } set { foreColorIndex = value; } }
		public int BackColorIndex { get { return backColorIndex; } set { backColorIndex = value; } }
		#endregion
		public ColorModelInfo GetForeColorModelInfo(DocumentModel documentModel) {
			return documentModel.Cache.ColorModelInfoCache[ForeColorIndex];
		}
		public ColorModelInfo GetBackColorModelInfo(DocumentModel documentModel) {
			return documentModel.Cache.ColorModelInfoCache[BackColorIndex];
		}
		public bool HasVisible(DocumentModel documentModel, bool isDifferential) {
			if (isDifferential)
				return backColorIndex != ColorModelInfoCache.DefaultItemIndex || foreColorIndex != ColorModelInfoCache.DefaultItemIndex;
			if (patternType == XlPatternType.None)
				return false;
			if (patternType == XlPatternType.Solid)
				return !CheckTransparentOrEmptyColor(documentModel, GetForeColorModelInfo); 
			else
				return !CheckTransparentOrEmptyColor(documentModel, GetBackColorModelInfo);
		}
		bool CheckTransparentOrEmptyColor(DocumentModel documentModel, Func<DocumentModel, ColorModelInfo> getInfo) {
			ColorModelInfo colorInfo = getInfo(documentModel);
			return DXColor.IsTransparentOrEmpty(colorInfo.ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors));
		}
		public override bool Equals(object obj) {
			FillInfo info = obj as FillInfo;
			if (info == null)
				return false;
			return
				EqualsNoColorIndexes(info) &&
				ForeColorIndex == info.ForeColorIndex &&
				BackColorIndex == info.BackColorIndex;
		}
		internal bool EqualsForDifferentWorkbooks(FillInfo otherInfo, DocumentModel targetDocumentModel, DocumentModel otherDocumentModel) {
			return
				EqualsNoColorIndexes(otherInfo) &&
				GetBackColorModelInfo(targetDocumentModel).Rgb == otherInfo.GetBackColorModelInfo(otherDocumentModel).Rgb &&
				GetForeColorModelInfo(targetDocumentModel).Rgb == otherInfo.GetForeColorModelInfo(otherDocumentModel).Rgb;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32((int)patternType, foreColorIndex, backColorIndex);
		}
		#region ICloneable<FillInfo> Members
		public FillInfo Clone() {
			FillInfo result = new FillInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<FillInfo> Members
		public void CopyFrom(FillInfo value) {
			PatternType = value.PatternType;
			ForeColorIndex = value.ForeColorIndex;
			BackColorIndex = value.BackColorIndex;
		}
		#endregion
		public void CopyFrom(IActualFillInfo value) {
			this.PatternType = value.PatternType;
			this.BackColorIndex = value.BackColorIndex;
			this.ForeColorIndex = value.ForeColorIndex;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
		internal bool EqualsNoColorIndexes(FillInfo info) {
			return PatternType == info.PatternType;
		}
	}
	#endregion
	#region FillInfoCache
	public class FillInfoCache : UniqueItemsCache<FillInfo> {
		internal const int DefaultItemIndex = 0;
		public FillInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
			UniquenessProviderType = DevExpress.Utils.DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			AppendItem(new FillInfo()); 
			FillInfo gray125 = new FillInfo();
			gray125.PatternType = XlPatternType.Gray125;
			AppendItem(gray125);
		}
		protected override FillInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new FillInfo();
		}
#if DEBUGTEST
		public static bool CheckDefaults2(FillInfoCache collection) {
			bool result = true;
			result &= collection != null;
			result &= collection.Count > 0;
			FillInfo info = (FillInfo)collection.DefaultItem;
			result &= 0 == info.BackColorIndex;
			result &= 0 == info.ForeColorIndex;
			result &= XlPatternType.None == info.PatternType;
			return result;
		}
#endif
	}
	#endregion
	#region IFillInfo
	public interface IFillInfo {
		XlPatternType PatternType { get; set; }
		Color ForeColor { get; set; }
		Color BackColor { get; set; }
		IGradientFillInfo GradientFill { get; }
		ModelFillType FillType { get; set; }
		void Clear();
	}
	#endregion
	#region IActualFillInfo
	public interface IActualFillInfo {
		XlPatternType PatternType { get; }
		Color ForeColor { get; }
		Color BackColor { get; }
		int ForeColorIndex { get; }
		int BackColorIndex { get; }
		bool ApplyPatternType { get; }
		bool ApplyBackColor { get; }
		bool ApplyForeColor { get; }
		bool IsDifferential { get; }
		IActualGradientFillInfo GradientFill { get; }
		ModelFillType FillType { get; }
	}
	#endregion
	#region GradientFillType
	public enum ModelGradientFillType {
		Linear = 0,
		Path = 1
	}
	#endregion
	#region IColorModelInfo
	public interface IColorModelInfo {
		Color Rgb { get; set; }
		ThemeColorIndex Theme { get; set; }
		int ColorIndex { get; set; }
		bool Auto { get; set; }
		double Tint { get; set; }
	}
	#endregion
	#region IColorModel
	public interface IColorModel : IColorModelInfo {
		ColorType ColorType { get; }
		IColorModelInfo Color { get; }
	}
	#endregion
	#region GradientFillInfo
	public class GradientFillInfo : ICloneable<GradientFillInfo>, ISupportsCopyFrom<GradientFillInfo>, ISupportsSizeOf {
		#region Fields
		internal const float DefaultConvergenceValue = 0;
		internal const double DefaultDegree = 0;
		internal const ModelGradientFillType DefaultType = ModelGradientFillType.Linear;
		ModelGradientFillType type;
		float left;
		float right;
		float top;
		float bottom;
		double degree;
		#endregion
		#region Properties
		public ModelGradientFillType Type { get { return type; } set { type = value; } }
		public float Left {
			get { return left; }
			set {
				ValueChecker.CheckValue(value, 0, 1, "ConvergenceLeft");
				left = value;
			}
		}
		public float Right {
			get { return right; }
			set {
				ValueChecker.CheckValue(value, 0, 1, "ConvergenceRight");
				right = value;
			}
		}
		public float Top {
			get { return top; }
			set {
				ValueChecker.CheckValue(value, 0, 1, "ConvergenceTop");
				top = value;
			}
		}
		public float Bottom {
			get { return bottom; }
			set {
				ValueChecker.CheckValue(value, 0, 1, "ConvergenceBottom");
				bottom = value;
			}
		}
		public double Degree { get { return degree; } set { degree = value; } }
		#endregion
		public override bool Equals(object obj) {
			GradientFillInfo info = obj as GradientFillInfo;
			if (info == null)
				return false;
			return
				type == info.type && left == info.left && right == info.right &&
				top == info.top && bottom == info.bottom && degree == info.degree;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32((int)type, left.GetHashCode(), right.GetHashCode(), top.GetHashCode(), bottom.GetHashCode(), degree.GetHashCode());
		}
		#region ICloneable<GradientFillInfo> Members
		public GradientFillInfo Clone() {
			GradientFillInfo result = new GradientFillInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<GradientFillInfo> Members
		public void CopyFrom(GradientFillInfo value) {
			type = value.type;
			left = value.left;
			right = value.right;
			top = value.top;
			bottom = value.bottom;
			degree = value.degree;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
#if DEBUGTEST
		public static bool CheckDefaults2(GradientFillInfo info) {
			bool result = true;
			result &= GradientFillInfo.DefaultType == info.Type;
			result &= GradientFillInfo.DefaultDegree == info.Degree;
			result &= GradientFillInfo.DefaultConvergenceValue == info.Left;
			result &= GradientFillInfo.DefaultConvergenceValue == info.Right;
			result &= GradientFillInfo.DefaultConvergenceValue == info.Top;
			result &= GradientFillInfo.DefaultConvergenceValue == info.Bottom;
			return result;
		}
#endif
	}
	#endregion
	#region GradientFillInfoCache
	public class GradientFillInfoCache : UniqueItemsCache<GradientFillInfo> {
		internal const int DefaultItemIndex = 0;
		public GradientFillInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
			UniquenessProviderType = DevExpress.Utils.DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		protected override GradientFillInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new GradientFillInfo();
		}
#if DEBUGTEST
		public static bool CheckDefaults2(GradientFillInfoCache collection) {
			return 1 == collection.Count && GradientFillInfo.CheckDefaults2(collection.DefaultItem);
		}
#endif
	}
	#endregion
	#region GradientStopInfo
	public class GradientStopInfo : ICloneable<GradientStopInfo>, ISupportsCopyFrom<GradientStopInfo>, ISupportsSizeOf {
		#region Static Members
		public static GradientStopInfo Create(DocumentModel documentModel, double position, ColorModelInfo colorInfo) {
			GradientStopInfo result = new GradientStopInfo();
			result.SetColorIndex(documentModel, colorInfo);
			result.position = position;
			return result;
		}
		#endregion
		#region Fields
		double position;
		int colorIndex;
		#endregion
		#region Properties
		public double Position {
			get { return position; }
			set {
				ValueChecker.CheckValue(value, 0, 1, "ConvergenceRight");
				position = value;
			}
		}
		public int ColorIndex { get { return colorIndex; } set { colorIndex = value; } }
		#endregion
		protected internal void SetColorIndex(DocumentModel documentModel, ColorModelInfo colorInfo) {
			colorIndex = documentModel.Cache.ColorModelInfoCache.AddItem(colorInfo);
		}
		protected internal Color GetColor(DocumentModel documentModel) {
			return GetColorModelInfo(documentModel).ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors);
		}
		protected internal ColorModelInfo GetColorModelInfo(DocumentModel documentModel) {
			return documentModel.Cache.ColorModelInfoCache[colorIndex];
		}
		public override bool Equals(object obj) {
			GradientStopInfo info = obj as GradientStopInfo;
			if (info == null)
				return false;
			return position == info.position && colorIndex == info.colorIndex;
		}
		internal bool EqualsForDifferentWorkbooks(GradientStopInfo otherInfo, DocumentModel documentModel, DocumentModel otherDocumentModel) {
			return position == otherInfo.position && GetColorModelInfo(documentModel).Rgb == otherInfo.GetColorModelInfo(otherDocumentModel).Rgb;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(position.GetHashCode(), colorIndex);
		}
		#region ICloneable<GradientStopInfo> Members
		public GradientStopInfo Clone() {
			GradientStopInfo result = new GradientStopInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<GradientStopInfo> Members
		public void CopyFrom(GradientStopInfo value) {
			position = value.position;
			colorIndex = value.colorIndex;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
#if DEBUGTEST
		public static bool CheckDefaults2(GradientStopInfo info) {
			return (0 == info.Position) && (0 == info.ColorIndex);
		}
#endif
	}
	#endregion
	#region GradientStopInfoCache
	public class GradientStopInfoCache : UniqueItemsCache<GradientStopInfo> {
		internal const int DefaultItemIndex = 0;
		public GradientStopInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
			UniquenessProviderType = DevExpress.Utils.DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		protected override GradientStopInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new GradientStopInfo();
		}
#if DEBUGTEST
		public static bool CheckDefaults2(GradientStopInfoCache collection) {
			return 1 == collection.Count && GradientStopInfo.CheckDefaults2(collection.DefaultItem);
		}
#endif
	}
	#endregion
	#region IGradientFillInfo
	public interface IGradientFillInfo {
		ModelGradientFillType Type { get; set; }
		IConvergenceInfo Convergence { get; }
		double Degree { get; set; }
		IGradientStopCollection GradientStops { get; }
	}
	#endregion
	#region IActualGradientFillInfo
	public interface IActualGradientFillInfo {
		ModelGradientFillType Type { get; }
		IActualConvergenceInfo Convergence { get; }
		double Degree { get; }
		IActualGradientStopCollection GradientStops { get; }
	}
	#endregion
	#region IActualGradientStopCollection
	public interface IActualGradientStopCollection {
		IGradientStopInfo this[int index] { get; }
		int Count { get; }
	}
	#endregion
	#region IGradientStopCollection
	public interface IGradientStopCollection : IActualGradientStopCollection {
		int Add(double position, int index);
		int Add(double position, Color rgb);
		int Add(double position, ThemeColorIndex theme);
		int Add(double position, ThemeColorIndex theme, double tint);
		void ForEach(Action<int> action);
		void CopyFrom(IGradientStopCollection stops);
		IGradientStopCollection Clone();
		void Clear();
		void RemoveAt(int index);
	}
	#endregion
	#region IGradientStopInfo
	public interface IGradientStopInfo {
		Color Color { get; }
		int ColorIndex { get; }
		double Position { get; }
	}
	#endregion
	#region IConvergenceInfo
	public interface IConvergenceInfo {
		float Left { get; set; }
		float Right { get; set; }
		float Top { get; set; }
		float Bottom { get; set; }
	}
	#endregion
	#region IActualConvergenceInfo
	public interface IActualConvergenceInfo {
		float Left { get; }
		float Right { get; }
		float Top { get; }
		float Bottom { get; }
	}
	#endregion
	#region GradientStopInfoCollection
	public class GradientStopInfoCollection : UndoableClonableCollection<int>, IGradientStopCollection, IActualGradientStopCollection {
		public GradientStopInfoCollection(IDocumentModel documentModelPart)
			: base(documentModelPart.MainPart) {
		}
		#region Properties
		protected internal new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		GradientStopInfoCache Cache { get { return DocumentModel.Cache.GradientStopInfoCache; } }
		#endregion
		#region IActualGradientStopCollection Members
		IGradientStopInfo IActualGradientStopCollection.this[int index] { 
			get {
				int infoIndex = this[index];
				return new GradientStopInfoPropertyAccessor(DocumentModel, infoIndex);
			} 
		}
		#endregion
		#region IGradientStopCollection Members
		int IGradientStopCollection.Add(double position, int index) {
			return Add(position, ColorModelInfo.Create(index));
		}
		int IGradientStopCollection.Add(double position, Color rgb) {
			return Add(position, ColorModelInfo.Create(rgb));
		}
		int IGradientStopCollection.Add(double position, ThemeColorIndex theme) {
			return Add(position, ColorModelInfo.Create(theme));
		}
		int IGradientStopCollection.Add(double position, ThemeColorIndex theme, double tint) {
			return Add(position, ColorModelInfo.Create(theme, tint));
		}
		void IGradientStopCollection.ForEach(Action<int> action) {
			base.ForEach(action);
		}
		int Add(double position, ColorModelInfo colorInfo) {
			GradientStopInfo info = GradientStopInfo.Create(DocumentModel, position, colorInfo);
			return Add(Cache.AddItem(info));
		}
		#endregion
		#region UndoableClonableCollection<int> Members
		public override UndoableClonableCollection<int> GetNewCollection(IDocumentModelPart documentModelPart) {
			return new GradientStopInfoCollection(documentModelPart.DocumentModel);
		}
		public override int GetCloneItem(int item, IDocumentModelPart documentModelPart) {
			return item;
		}
		#endregion
		void IGradientStopCollection.CopyFrom(IGradientStopCollection stops) {
			GradientStopInfoCollection collection = stops as GradientStopInfoCollection;
			System.Diagnostics.Debug.Assert(collection != null && Object.ReferenceEquals(collection.DocumentModel, DocumentModel)); 
			DocumentModel.BeginUpdate();
			try {
				Clear();
				int count = collection.Count;
				for (int i = 0; i < count; i++) {
					GradientStopInfo info = Cache[collection[i]];
					Add(info.Position, info.GetColorModelInfo(DocumentModel));
				}
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		internal bool EqualsForDifferentWorkbooks(GradientStopInfoCollection other) {
			if (Count != other.Count)
				return false;
			for (int i = 0; i < Count; i++) {
				GradientStopInfo thisInfo = Cache[this[i]];
				GradientStopInfo otherInfo = other.Cache[other[i]];
				if (thisInfo.EqualsForDifferentWorkbooks(otherInfo, DocumentModel, other.DocumentModel))
					return false;
			}
			return true;
		}
		IGradientStopCollection IGradientStopCollection.Clone() {
			return base.Clone() as GradientStopInfoCollection;
		}
	}
	#endregion
	#region GradientStopInfoPropertyAccessor
	public class GradientStopInfoPropertyAccessor : IGradientStopInfo {
		#region Static Members
		public static void CopyDeferredInfo(GradientStopInfoCollection owner, GradientStopInfoCollection from) {
			if (!Object.ReferenceEquals(owner.DocumentModel, from.DocumentModel)) {
				int count = from.Count;
				for (int i = 1; i < count; i++) {
					int formatIndex = ColorModelInfo.ConvertColorIndex(from.DocumentModel.Cache.ColorModelInfoCache, from[i], owner.DocumentModel.Cache.ColorModelInfoCache);
					owner.AddCore(formatIndex);
				}
			} else
				owner.CopyFrom(from);
		}
		#endregion
		#region Fields
		readonly DocumentModel documentModel;
		readonly int infoIndex;
		#endregion
		public GradientStopInfoPropertyAccessor(DocumentModel documentModel, int infoIndex) {
			this.documentModel = documentModel;
			this.infoIndex = infoIndex;
		}
		#region IGradientStopInfo Members
		GradientStopInfo Info { get { return documentModel.Cache.GradientStopInfoCache[infoIndex]; } }
		public int ColorIndex { get { return Info.ColorIndex; } }
		public double Position { get { return Info.Position; } }
		public Color Color { get { return Info.GetColor(documentModel); } }
		#endregion
	}
	#endregion
	#region ModelFillType
	public enum ModelFillType {
		Pattern = 0,
		Gradient = 1
	}
	#endregion
}

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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region StripeSizeInfo
	public struct StripeSizeInfo : ICloneable<StripeSizeInfo>, ISupportsSizeOf {
		#region Static Members
		readonly static StripeSizeInfo defaultStyle = Create(StripeSizeInfo.DefaultValue);
		public static StripeSizeInfo Create(int value) {
			StripeSizeInfo result = new StripeSizeInfo();
			result.StripeSize = value;
			return result;
		}
		#endregion
		public const int DefaultValue = 1;
		int stripeSize;
		public int StripeSize { get { return stripeSize; } set { this.stripeSize = value; } }
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(StripeSizeInfo));
		}
		#endregion
		#region ICloneable<StripeSizeInfo> Members
		public StripeSizeInfo Clone() {
			return StripeSizeInfo.Create(StripeSize);
		}
		#endregion
	}
	#endregion
	#region TableStyleElementFormatStripeSizeInfoIndexAccessor
	public class TableStyleElementFormatStripeSizeInfoIndexAccessor : IIndexAccessor<TableStyleElementFormat, StripeSizeInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<TableStyleElementFormat, StripeSizeInfo> Members
		public int GetIndex(TableStyleElementFormat owner) {
			return owner.StripeSizeInfoIndex;
		}
		public int GetDeferredInfoIndex(TableStyleElementFormat owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(TableStyleElementFormat owner, int value) {
			owner.AssignStripeSizeInfoIndex(value);
		}
		public int GetInfoIndex(TableStyleElementFormat owner, StripeSizeInfo value) {
			return value.StripeSize;
		}
		public StripeSizeInfo GetInfo(TableStyleElementFormat owner) {
			StripeSizeInfo info = new StripeSizeInfo();
			info.StripeSize = owner.StripeSizeInfoIndex;
			return info;
		}
		public bool IsIndexValid(TableStyleElementFormat owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(TableStyleElementFormat owner) {
			return new TableStyleElementFormatStripeSizeInfoIndexChangeHistoryItem(owner);
		}
		public StripeSizeInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableStyleElementFormatBatchUpdateHelper)helper).StripeSizeInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, StripeSizeInfo info) {
			((TableStyleElementFormatBatchUpdateHelper)helper).StripeSizeInfo = info.Clone();
		}
		public void InitializeDeferredInfo(TableStyleElementFormat owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(TableStyleElementFormat owner, TableStyleElementFormat from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(TableStyleElementFormat owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region TableStyleElementFormatDifferentialFormatIndexAccessor
	public class TableStyleElementFormatDifferentialFormatIndexAccessor : IIndexAccessor<TableStyleElementFormat, FormatBase, DocumentModelChangeActions> {
		#region IIndexAccessor Members
		public int GetIndex(TableStyleElementFormat owner) {
			return owner.DifferentialFormatIndex;
		}
		public int GetDeferredInfoIndex(TableStyleElementFormat owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(TableStyleElementFormat owner, int value) {
			owner.AssignDifferentialFormatIndex(value);
		}
		public int GetInfoIndex(TableStyleElementFormat owner, FormatBase value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public FormatBase GetInfo(TableStyleElementFormat owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(TableStyleElementFormat owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<FormatBase> GetInfoCache(TableStyleElementFormat owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(TableStyleElementFormat owner) {
			return new TableStyleElementFormatDifferentialFormatIndexChangeHistoryItem(owner);
		}
		public FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableStyleElementFormatBatchUpdateHelper)helper).DifferentialFormat;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			TableStyleElementFormatBatchUpdateHelper tableStyleElementFormatBatchUpdateHelper = helper as TableStyleElementFormatBatchUpdateHelper;
			if (tableStyleElementFormatBatchUpdateHelper.DifferentialFormat == null)
				tableStyleElementFormatBatchUpdateHelper.DifferentialFormat = (DifferentialFormat)info.Clone();
			else {
				tableStyleElementFormatBatchUpdateHelper.DifferentialFormat.BeginUpdate();
				info.BeginUpdate();
				try {
					tableStyleElementFormatBatchUpdateHelper.DifferentialFormat.CopyFromDeferred(info); 
				}
				finally {
					info.EndUpdate();
					tableStyleElementFormatBatchUpdateHelper.DifferentialFormat.EndUpdate();
				}
			}
		}
		public void InitializeDeferredInfo(TableStyleElementFormat owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(TableStyleElementFormat owner, TableStyleElementFormat from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(TableStyleElementFormat owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region TableStyleElementFormatBatchUpdateHelper
	public class TableStyleElementFormatBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		DifferentialFormat differentialFormat;
		StripeSizeInfo stripeSizeInfo;
		public TableStyleElementFormatBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public StripeSizeInfo StripeSizeInfo { get { return stripeSizeInfo; } set { stripeSizeInfo = value; } }
		public DifferentialFormat DifferentialFormat { get { return differentialFormat; } set { differentialFormat = value; } }
	}
	#endregion
	#region TableStyleElementFormatBatchInitHelper
	public class TableStyleElementFormatBatchInitHelper : MultiIndexBatchUpdateHelper {
		public TableStyleElementFormatBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region ITableStyleElementFormat
	public interface ITableStyleElementFormat : IDifferentialFormat {
		int StripeSize { get; set; }
	}
	#endregion
	#region TableStyleElementFormat
	public class TableStyleElementFormat : MultiIndexObject<TableStyleElementFormat, DocumentModelChangeActions>, ITableStyleElementFormat, ICloneable<TableStyleElementFormat>, IRunFontInfo, IFillInfo, IBorderInfo, ICellAlignmentInfo, ICellProtectionInfo, IGradientFillInfo, IConvergenceInfo {
		#region Static Members
		readonly static TableStyleElementFormatDifferentialFormatIndexAccessor differentialFormatIndexAccessor = new TableStyleElementFormatDifferentialFormatIndexAccessor();
		readonly static TableStyleElementFormatStripeSizeInfoIndexAccessor stripeSizeInfoIndexAccessor = new TableStyleElementFormatStripeSizeInfoIndexAccessor();
		readonly static IIndexAccessorBase<TableStyleElementFormat, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<TableStyleElementFormat, DocumentModelChangeActions>[] {
			differentialFormatIndexAccessor, 
			stripeSizeInfoIndexAccessor
		};
		public static TableStyleElementFormatDifferentialFormatIndexAccessor DifferentialFormatIndexAccessor { get { return differentialFormatIndexAccessor; } }
		public static TableStyleElementFormatStripeSizeInfoIndexAccessor StripeSizeInfoIndexAccessor { get { return stripeSizeInfoIndexAccessor; } }
		#endregion
		#region Fields
		readonly IDocumentModel documentModel;
		int differentialFormatIndex = CellFormatCache.DefaultDifferentialFormatIndex;
		int stripeSizeInfoIndex = StripeSizeInfo.DefaultValue;
		#endregion
		public TableStyleElementFormat(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		#region Properties
		internal int DifferentialFormatIndex { get { return differentialFormatIndex; } }
		internal int StripeSizeInfoIndex { get { return stripeSizeInfoIndex; } }
		internal new TableStyleElementFormatBatchUpdateHelper BatchUpdateHelper { get { return (TableStyleElementFormatBatchUpdateHelper)base.BatchUpdateHelper; } }
		internal DifferentialFormat FormatInfo { get { return BatchUpdateHelper != null ? BatchUpdateHelper.DifferentialFormat : FormatInfoCore; } }
		DifferentialFormat FormatInfoCore { get { return (DifferentialFormat)differentialFormatIndexAccessor.GetInfo(this); } }
		StripeSizeInfo StripeSizeInfo { get { return BatchUpdateHelper != null ? BatchUpdateHelper.StripeSizeInfo : StripeSizeInfoCore; } }
		StripeSizeInfo StripeSizeInfoCore { get { return stripeSizeInfoIndexAccessor.GetInfo(this); } }
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		internal bool HasDefaultDifferentialFormatting { get { return DifferentialFormatIndex == CellFormatCache.DefaultDifferentialFormatIndex; } }
		#endregion
		#region ITableStyleElementFormat Members
		public int StripeSize {
			get { return StripeSizeInfo.StripeSize; }
			set {
				if (StripeSizeInfo.StripeSize == value)
					return;
				SetPropertyValueForStruct(stripeSizeInfoIndexAccessor, SetStripeSize, value);
			}
		}
		DocumentModelChangeActions SetStripeSize(ref StripeSizeInfo info, int value) {
			info.StripeSize = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IDifferentialFormat Members
		public IRunFontInfo Font { get { return this; } }
		public ICellAlignmentInfo Alignment { get { return this; } }
		public IBorderInfo Border { get { return this; } }
		public IFillInfo Fill { get { return this; } }
		public ICellProtectionInfo Protection { get { return this; } }
		#region FormatString members
		public string FormatString {
			get { return FormatInfo.FormatString; }
			set {
				if (FormatInfo.FormatString == value && FormatInfo.MultiOptionsInfo.ApplyNumberFormat)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetNumberFormat, value);
			}
		}
		DocumentModelChangeActions SetNumberFormat(FormatBase info, string value) {
			((DifferentialFormat)info).FormatString = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo Members
		#region IRunFontInfo.Name
		string IRunFontInfo.Name {
			get { return FormatInfo.Font.Name; }
			set {
				if (FormatInfo.Font.Name == value && FormatInfo.MultiOptionsInfo.ApplyFontName)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontName, value);
			}
		}
		DocumentModelChangeActions SetFontName(FormatBase info, string value) {
			info.Font.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Color
		Color IRunFontInfo.Color {
			get { return FormatInfo.Font.Color; }
			set {
				if (FormatInfo.Font.Color == value && FormatInfo.MultiOptionsInfo.ApplyFontColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontColor, value);
			}
		}
		DocumentModelChangeActions SetFontColor(FormatBase info, Color value) {
			info.Font.Color = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Bold
		bool IRunFontInfo.Bold {
			get { return FormatInfo.Font.Bold; }
			set {
				if (FormatInfo.Font.Bold == value && FormatInfo.MultiOptionsInfo.ApplyFontBold)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontBold, value);
			}
		}
		DocumentModelChangeActions SetFontBold(FormatBase info, bool value) {
			info.Font.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Condense
		bool IRunFontInfo.Condense {
			get { return FormatInfo.Font.Condense; }
			set {
				if (FormatInfo.Font.Condense == value && FormatInfo.MultiOptionsInfo.ApplyFontCondense)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontCondense, value);
			}
		}
		DocumentModelChangeActions SetFontCondense(FormatBase info, bool value) {
			info.Font.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Extend
		bool IRunFontInfo.Extend {
			get { return FormatInfo.Font.Extend; }
			set {
				if (FormatInfo.Font.Extend == value && FormatInfo.MultiOptionsInfo.ApplyFontExtend)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontExtend, value);
			}
		}
		DocumentModelChangeActions SetFontExtend(FormatBase info, bool value) {
			info.Font.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Italic
		bool IRunFontInfo.Italic {
			get { return FormatInfo.Font.Italic; }
			set {
				if (FormatInfo.Font.Italic == value && FormatInfo.MultiOptionsInfo.ApplyFontItalic)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontItalic, value);
			}
		}
		DocumentModelChangeActions SetFontItalic(FormatBase info, bool value) {
			info.Font.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Outline
		bool IRunFontInfo.Outline {
			get { return FormatInfo.Font.Outline; }
			set {
				if (FormatInfo.Font.Outline == value && FormatInfo.MultiOptionsInfo.ApplyFontOutline)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontOutline, value);
			}
		}
		DocumentModelChangeActions SetFontOutline(FormatBase info, bool value) {
			info.Font.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Shadow
		bool IRunFontInfo.Shadow {
			get { return FormatInfo.Font.Shadow; }
			set {
				if (FormatInfo.Font.Shadow == value && FormatInfo.MultiOptionsInfo.ApplyFontShadow)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontShadow, value);
			}
		}
		DocumentModelChangeActions SetFontShadow(FormatBase info, bool value) {
			info.Font.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return FormatInfo.Font.StrikeThrough; }
			set {
				if (FormatInfo.Font.StrikeThrough == value && FormatInfo.MultiOptionsInfo.ApplyFontStrikeThrough)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontStrikeThrough, value);
			}
		}
		DocumentModelChangeActions SetFontStrikeThrough(FormatBase info, bool value) {
			info.Font.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Charset
		int IRunFontInfo.Charset {
			get { return FormatInfo.Font.Charset; }
			set {
				if (FormatInfo.Font.Charset == value && FormatInfo.MultiOptionsInfo.ApplyFontCharset)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontCharset, value);
			}
		}
		DocumentModelChangeActions SetFontCharset(FormatBase info, int value) {
			info.Font.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.FontFamily
		int IRunFontInfo.FontFamily {
			get { return FormatInfo.Font.FontFamily; }
			set {
				if (FormatInfo.Font.FontFamily == value && FormatInfo.MultiOptionsInfo.ApplyFontFamily)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontFamily, value);
			}
		}
		DocumentModelChangeActions SetFontFamily(FormatBase info, int value) {
			info.Font.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Size
		double IRunFontInfo.Size {
			get { return FormatInfo.Font.Size; }
			set {
				if (FormatInfo.Font.Size == value && FormatInfo.MultiOptionsInfo.ApplyFontSize)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontSize, value);
			}
		}
		DocumentModelChangeActions SetFontSize(FormatBase info, double value) {
			info.Font.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.SchemeStyle
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return FormatInfo.Font.SchemeStyle; }
			set {
				if (FormatInfo.Font.SchemeStyle == value && FormatInfo.MultiOptionsInfo.ApplyFontSchemeStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontSchemeStyle, value);
			}
		}
		DocumentModelChangeActions SetFontSchemeStyle(FormatBase info, XlFontSchemeStyles value) {
			info.Font.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Script
		XlScriptType IRunFontInfo.Script {
			get { return FormatInfo.Font.Script; }
			set {
				if (FormatInfo.Font.Script == value && FormatInfo.MultiOptionsInfo.ApplyFontScript)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontScript, value);
			}
		}
		DocumentModelChangeActions SetFontScript(FormatBase info, XlScriptType value) {
			info.Font.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IRunFontInfo.Underline
		XlUnderlineType IRunFontInfo.Underline {
			get { return FormatInfo.Font.Underline; }
			set {
				if (FormatInfo.Font.Underline == value && FormatInfo.MultiOptionsInfo.ApplyFontUnderline)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFontUnderline, value);
			}
		}
		DocumentModelChangeActions SetFontUnderline(FormatBase info, XlUnderlineType value) {
			info.Font.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region IFillInfo Members
		#region IFillInfo.Clear
		void IFillInfo.Clear() {
			if (!FormatInfo.MultiOptionsInfo.ApplyFillNone)
				ClearFill();
		}
		#endregion
		#region IFillInfo.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return FormatInfo.Fill.PatternType; }
			set {
				if (FormatInfo.Fill.PatternType == value && FormatInfo.MultiOptionsInfo.ApplyFillPatternType)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFillPatternType, value);
			}
		}
		DocumentModelChangeActions SetFillPatternType(FormatBase info, XlPatternType value) {
			info.Fill.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.ForeColor
		Color IFillInfo.ForeColor {
			get { return FormatInfo.Fill.ForeColor; }
			set {
				if (FormatInfo.Fill.ForeColor == value && FormatInfo.MultiOptionsInfo.ApplyFillForeColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFillForeColor, value);
			}
		}
		DocumentModelChangeActions SetFillForeColor(FormatBase info, Color value) {
			info.Fill.ForeColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.BackColor
		Color IFillInfo.BackColor {
			get { return FormatInfo.Fill.BackColor; }
			set {
				if (FormatInfo.Fill.BackColor == value && FormatInfo.MultiOptionsInfo.ApplyFillBackColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetFillBackColor, value);
			}
		}
		DocumentModelChangeActions SetFillBackColor(FormatBase info, Color value) {
			info.Fill.BackColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IFillInfo.GradientFill
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		#endregion
		#region IFillInfo.FillType
		ModelFillType IFillInfo.FillType {
			get { return FormatInfo.Fill.FillType; }
			set {
				if (FormatInfo.Fill.FillType == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetModelFillType, value);
			}
		}
		protected DocumentModelChangeActions SetModelFillType(FormatBase info, ModelFillType value) {
			info.Fill.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo Members
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return FormatInfo.Fill.GradientFill.GradientStops; } }
		#region IGradientFillInfo.Type
		ModelGradientFillType IGradientFillInfo.Type {
			get { return FormatInfo.Fill.GradientFill.Type; }
			set {
				if (FormatInfo.Fill.GradientFill.Type == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoType, value);
			}
		}
		DocumentModelChangeActions SetGradientFillInfoType(FormatBase info, ModelGradientFillType value) {
			info.Fill.GradientFill.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IGradientFillInfo.Degree
		double IGradientFillInfo.Degree {
			get { return FormatInfo.Fill.GradientFill.Degree; }
			set {
				if (FormatInfo.Fill.GradientFill.Degree == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoDegree, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoDegree(FormatBase info, double value) {
			info.Fill.GradientFill.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo Members
		#region IConvergenceInfo.Left
		float IConvergenceInfo.Left {
			get { return FormatInfo.Fill.GradientFill.Convergence.Left; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Left == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoLeft, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoLeft(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Right
		float IConvergenceInfo.Right {
			get { return FormatInfo.Fill.GradientFill.Convergence.Right; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Right == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoRight, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoRight(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Top
		float IConvergenceInfo.Top {
			get { return FormatInfo.Fill.GradientFill.Convergence.Top; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Top == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoTop, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoTop(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IConvergenceInfo.Bottom
		float IConvergenceInfo.Bottom {
			get { return FormatInfo.Fill.GradientFill.Convergence.Bottom; }
			set {
				if (FormatInfo.Fill.GradientFill.Convergence.Bottom == value)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetGradientFillInfoBottom, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoBottom(FormatBase info, float value) {
			info.Fill.GradientFill.Convergence.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#endregion
		#region IBorderInfo Members
		#region IBorderInfo.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return FormatInfo.Border.LeftLineStyle; }
			set {
				if (FormatInfo.Border.LeftLineStyle == value && FormatInfo.BorderOptionsInfo.ApplyLeftLineStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderLeftLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.LeftColor
		Color IBorderInfo.LeftColor {
			get { return FormatInfo.Border.LeftColor; }
			set {
				if (FormatInfo.Border.LeftColor == value && FormatInfo.BorderOptionsInfo.ApplyLeftColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderLeftColor, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColor(FormatBase info, Color value) {
			info.Border.LeftColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.LeftColorIndex
		int IBorderInfo.LeftColorIndex {
			get { return FormatInfo.Border.LeftColorIndex; }
			set {
				if (FormatInfo.Border.LeftColorIndex == value && FormatInfo.BorderOptionsInfo.ApplyLeftColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderLeftColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderLeftColorIndex(FormatBase info, int value) {
			info.Border.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightLineStyle
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return FormatInfo.Border.RightLineStyle; }
			set {
				if (FormatInfo.Border.RightLineStyle == value && FormatInfo.BorderOptionsInfo.ApplyRightLineStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderRightLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderRightLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightColor
		Color IBorderInfo.RightColor {
			get { return FormatInfo.Border.RightColor; }
			set {
				if (FormatInfo.Border.RightColor == value && FormatInfo.BorderOptionsInfo.ApplyRightColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderRightColor, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColor(FormatBase info, Color value) {
			info.Border.RightColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.RightColorIndex
		int IBorderInfo.RightColorIndex {
			get { return FormatInfo.Border.RightColorIndex; }
			set {
				if (FormatInfo.Border.RightColorIndex == value && FormatInfo.BorderOptionsInfo.ApplyRightColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderRightColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderRightColorIndex(FormatBase info, int value) {
			info.Border.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopLineStyle
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return FormatInfo.Border.TopLineStyle; }
			set {
				if (FormatInfo.Border.TopLineStyle == value && FormatInfo.BorderOptionsInfo.ApplyTopLineStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderTopLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderTopLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopColor
		Color IBorderInfo.TopColor {
			get { return FormatInfo.Border.TopColor; }
			set {
				if (FormatInfo.Border.TopColor == value && FormatInfo.BorderOptionsInfo.ApplyTopColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderTopColor, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColor(FormatBase info, Color value) {
			info.Border.TopColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.TopColorIndex
		int IBorderInfo.TopColorIndex {
			get { return FormatInfo.Border.TopColorIndex; }
			set {
				if (FormatInfo.Border.TopColorIndex == value && FormatInfo.BorderOptionsInfo.ApplyTopColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderTopColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderTopColorIndex(FormatBase info, int value) {
			info.Border.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomLineStyle
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return FormatInfo.Border.BottomLineStyle; }
			set {
				if (FormatInfo.Border.BottomLineStyle == value && FormatInfo.BorderOptionsInfo.ApplyBottomLineStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderBottomLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomColor
		Color IBorderInfo.BottomColor {
			get { return FormatInfo.Border.BottomColor; }
			set {
				if (FormatInfo.Border.BottomColor == value && FormatInfo.BorderOptionsInfo.ApplyBottomColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderBottomColor, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColor(FormatBase info, Color value) {
			info.Border.BottomColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.BottomColorIndex
		int IBorderInfo.BottomColorIndex {
			get { return FormatInfo.Border.BottomColorIndex; }
			set {
				if (FormatInfo.Border.BottomColorIndex == value && FormatInfo.BorderOptionsInfo.ApplyBottomColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderBottomColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderBottomColorIndex(FormatBase info, int value) {
			info.Border.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalLineStyle
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return FormatInfo.Border.HorizontalLineStyle; }
			set {
				if (FormatInfo.Border.HorizontalLineStyle == value && FormatInfo.BorderOptionsInfo.ApplyHorizontalLineStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderHorizontalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalColor
		Color IBorderInfo.HorizontalColor {
			get { return FormatInfo.Border.HorizontalColor; }
			set {
				if (FormatInfo.Border.HorizontalColor == value && FormatInfo.BorderOptionsInfo.ApplyHorizontalColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderHorizontalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColor(FormatBase info, Color value) {
			info.Border.HorizontalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.HorizontalColorIndex
		int IBorderInfo.HorizontalColorIndex {
			get { return FormatInfo.Border.HorizontalColorIndex; }
			set {
				if (FormatInfo.Border.HorizontalColorIndex == value && FormatInfo.BorderOptionsInfo.ApplyHorizontalColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderHorizontalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderHorizontalColorIndex(FormatBase info, int value) {
			info.Border.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalLineStyle
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return FormatInfo.Border.VerticalLineStyle; }
			set {
				if (FormatInfo.Border.VerticalLineStyle == value && FormatInfo.BorderOptionsInfo.ApplyVerticalLineStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderVerticalLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalColor
		Color IBorderInfo.VerticalColor {
			get { return FormatInfo.Border.VerticalColor; }
			set {
				if (FormatInfo.Border.VerticalColor == value && FormatInfo.BorderOptionsInfo.ApplyVerticalColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderVerticalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColor(FormatBase info, Color value) {
			info.Border.VerticalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.VerticalColorIndex
		int IBorderInfo.VerticalColorIndex {
			get { return FormatInfo.Border.VerticalColorIndex; }
			set {
				if (FormatInfo.Border.VerticalColorIndex == value && FormatInfo.BorderOptionsInfo.ApplyVerticalColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderVerticalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderVerticalColorIndex(FormatBase info, int value) {
			info.Border.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalUpLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return FormatInfo.Border.DiagonalUpLineStyle; }
			set {
				if (FormatInfo.Border.DiagonalUpLineStyle == value && FormatInfo.BorderOptionsInfo.ApplyDiagonalLineStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderDiagonalUpLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalUpLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalUpLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalDownLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return FormatInfo.Border.DiagonalDownLineStyle; }
			set {
				if (FormatInfo.Border.DiagonalDownLineStyle == value && FormatInfo.BorderOptionsInfo.ApplyDiagonalLineStyle)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderDiagonalDownLineStyle, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalDownLineStyle(FormatBase info, XlBorderLineStyle value) {
			info.Border.DiagonalDownLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalColor
		Color IBorderInfo.DiagonalColor {
			get { return FormatInfo.Border.DiagonalColor; }
			set {
				if (FormatInfo.Border.DiagonalColor == value && FormatInfo.BorderOptionsInfo.ApplyDiagonalColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderDiagonalColor, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColor(FormatBase info, Color value) {
			info.Border.DiagonalColor = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.DiagonalColorIndex
		int IBorderInfo.DiagonalColorIndex {
			get { return FormatInfo.Border.DiagonalColorIndex; }
			set {
				if (FormatInfo.Border.DiagonalColorIndex == value && FormatInfo.BorderOptionsInfo.ApplyDiagonalColor)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderDiagonalColorIndex, value);
			}
		}
		DocumentModelChangeActions SetBorderDiagonalColorIndex(FormatBase info, int value) {
			info.Border.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region IBorderInfo.Outline
		bool IBorderInfo.Outline {
			get { return FormatInfo.Border.Outline; }
			set {
				if (FormatInfo.Border.Outline == value && FormatInfo.BorderOptionsInfo.ApplyOutline)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetBorderOutline, value);
			}
		}
		DocumentModelChangeActions SetBorderOutline(FormatBase info, bool value) {
			info.Border.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region ICellAlignmentInfo Members
		#region ICellAlignmentInfo.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return FormatInfo.Alignment.WrapText; }
			set {
				if (FormatInfo.Alignment.WrapText == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentWrapText)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentWrapText, value);
			}
		}
		DocumentModelChangeActions SetAlignmentWrapText(FormatBase info, bool value) {
			info.Alignment.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return FormatInfo.Alignment.JustifyLastLine; }
			set {
				if (FormatInfo.Alignment.JustifyLastLine == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentJustifyLastLine)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentJustifyLastLine, value);
			}
		}
		DocumentModelChangeActions SetAlignmentJustifyLastLine(FormatBase info, bool value) {
			info.Alignment.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return FormatInfo.Alignment.ShrinkToFit; }
			set {
				if (FormatInfo.Alignment.ShrinkToFit == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentShrinkToFit)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentShrinkToFit, value);
			}
		}
		DocumentModelChangeActions SetAlignmentShrinkToFit(FormatBase info, bool value) {
			info.Alignment.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return FormatInfo.Alignment.TextRotation; }
			set {
				if (FormatInfo.Alignment.TextRotation == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentTextRotation)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentTextRotation, value);
			}
		}
		DocumentModelChangeActions SetAlignmentTextRotation(FormatBase info, int value) {
			info.Alignment.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Indent
		byte ICellAlignmentInfo.Indent {
			get { return FormatInfo.Alignment.Indent; }
			set {
				if (FormatInfo.Alignment.Indent == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentIndent)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentIndent(FormatBase info, byte value) {
			info.Alignment.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return FormatInfo.Alignment.RelativeIndent; }
			set {
				if (FormatInfo.Alignment.RelativeIndent == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentRelativeIndent)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentRelativeIndent, value);
			}
		}
		DocumentModelChangeActions SetAlignmentRelativeIndent(FormatBase info, int value) {
			info.Alignment.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return FormatInfo.Alignment.Horizontal; }
			set {
				if (FormatInfo.Alignment.Horizontal == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentHorizontal)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentHorizontal, value);
			}
		}
		DocumentModelChangeActions SetAlignmentHorizontal(FormatBase info, XlHorizontalAlignment value) {
			info.Alignment.Horizontal = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return FormatInfo.Alignment.Vertical; }
			set {
				if (FormatInfo.Alignment.Vertical == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentVertical)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentVertical, value);
			}
		}
		DocumentModelChangeActions SetAlignmentVertical(FormatBase info, XlVerticalAlignment value) {
			info.Alignment.Vertical = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellAlignmentInfo.XlReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return FormatInfo.Alignment.ReadingOrder; }
			set {
				if (FormatInfo.Alignment.ReadingOrder == value && FormatInfo.MultiOptionsInfo.ApplyAlignmentReadingOrder)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetAlignmentReadingOrder, value);
			}
		}
		DocumentModelChangeActions SetAlignmentReadingOrder(FormatBase info, XlReadingOrder value) {
			info.Alignment.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region ICellProtectionInfo Members
		#region ICellProtectionInfo.Locked
		bool ICellProtectionInfo.Locked {
			get { return FormatInfo.Protection.Locked; }
			set {
				if (FormatInfo.Protection.Locked == value && FormatInfo.MultiOptionsInfo.ApplyProtectionLocked)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetProtectionLocked, value);
			}
		}
		DocumentModelChangeActions SetProtectionLocked(FormatBase info, bool value) {
			info.Protection.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region ICellProtectionInfo.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return FormatInfo.Protection.Hidden; }
			set {
				if (FormatInfo.Protection.Hidden == value && FormatInfo.MultiOptionsInfo.ApplyProtectionHidden)
					return;
				SetPropertyValue(differentialFormatIndexAccessor, SetProtectionHidden, value);
			}
		}
		DocumentModelChangeActions SetProtectionHidden(FormatBase info, bool value) {
			info.Protection.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#region ClearFormatting
		internal void RemoveBorders() {
			DocumentModel.BeginUpdate();
			try {
				DifferentialFormat info = (DifferentialFormat)GetInfoForModification(DifferentialFormatIndexAccessor);
				info.RemoveBorders();
				ReplaceInfo(DifferentialFormatIndexAccessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		internal void ClearFont() {
			DocumentModel.BeginUpdate();
			try {
				DifferentialFormat info = (DifferentialFormat)GetInfoForModification(DifferentialFormatIndexAccessor);
				info.ClearFont();
				ReplaceInfo(DifferentialFormatIndexAccessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		internal void ClearFill() {
			DocumentModel.BeginUpdate();
			try {
				DifferentialFormat info = (DifferentialFormat)GetInfoForModification(DifferentialFormatIndexAccessor);
				info.Fill.Clear();
				ReplaceInfo(DifferentialFormatIndexAccessor, info, DocumentModelChangeActions.None);
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region MultiIndexObject members
		protected override IIndexAccessorBase<TableStyleElementFormat, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		protected override IDocumentModel GetDocumentModel() {
			return documentModel;
		}
		public override TableStyleElementFormat GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new TableStyleElementFormatBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new TableStyleElementFormatBatchInitHelper(this);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected  override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		#endregion
		#region Index Management
		internal void AssignDifferentialFormatIndex(int value) {
			this.differentialFormatIndex = value;
		}
		internal void AssignStripeSizeInfoIndex(int value) {
			this.stripeSizeInfoIndex = value;
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			TableStyleElementFormat other = obj as TableStyleElementFormat;
			if (other == null)
				return false;
			return base.Equals(other);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion
		#region ICloneable<TableStyleElementFormat> Members
		public TableStyleElementFormat Clone() {
			TableStyleElementFormat clone = CreateEmptyClone(DocumentModel);
			CloneCore(clone);
			return clone;
		}
		public virtual TableStyleElementFormat CreateEmptyClone(IDocumentModel documentModel) {
			return new TableStyleElementFormat((DocumentModel)documentModel);
		}
		#endregion
		internal void CopySimple(TableStyleElementFormat item) {
			this.differentialFormatIndex = item.differentialFormatIndex;
			this.stripeSizeInfoIndex = item.stripeSizeInfoIndex;
		}
	}
	#endregion
	#region TableStyleElementFormatCache
	public class TableStyleElementFormatCache : UniqueItemsCache<TableStyleElementFormat> {
		public const int DefaultItemIndex = 0;
		public TableStyleElementFormatCache(IDocumentModelUnitConverter unitConverter, DocumentModel documentModel)
			: base(unitConverter, documentModel) {
			UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		protected override TableStyleElementFormat CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new TableStyleElementFormat((DocumentModel)workbook);
		}
		public void CopyFrom(TableStyleElementFormatCache source) {
			this.Items.Clear();
			if (ItemDictionary != null)
				this.ItemDictionary.Clear();
			int currentIndex = 0;
			foreach (TableStyleElementFormat item in source.Items) {
				TableStyleElementFormat clone = item.CreateEmptyClone(workbook);
				clone.CopySimple(item);
				Items.Add(clone);
				if (UniquenessProviderType == DXCollectionUniquenessProviderType.MaximizePerformance)
					ItemDictionary.Add(clone, currentIndex);
				currentIndex++;
			}
		}
	}
	#endregion
}

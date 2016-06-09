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

namespace DevExpress.Snap.Core.API {
	using System.Collections.Generic;
	using DevExpress.XtraRichEdit.API.Native;
	using DevExpress.XtraRichEdit.API.Native.Implementation;
	using DevExpress.Office;
	using System.Runtime.InteropServices;
	using System.Drawing;
	[ComVisible(true)]
	public interface ThemeTableStyleCollection : ISimpleCollection<TableStyle> {
		TableStyle this[string name] { get; }
	}
	[ComVisible(true)]
	public interface ThemeTableCellStyleCollection : ISimpleCollection<TableCellStyle> {
		TableCellStyle this[string name] { get; }
	}
	[ComVisible(true)]
	public interface Theme {
		string Name { set; get; }
		Image Icon { set; get; }
		bool IsDefault { get; }
		void UpdateToMatchDocumentStyles();
	}
	[ComVisible(true)]
	public interface ThemeCollection : ISimpleCollection<Theme> {
		Theme this[string name] { get; }
		Theme CreateNew();
		Theme CreateNew(string baseThemeName);
		void Add(Theme theme);
		void Delete(Theme theme);
	}
	public static class Themes {
		public static readonly string Casual = "Casual";
		public static readonly string ContrastCyan = "Contrast Cyan";
		public static readonly string ContrastOrange = "Contrast Orange";
		public static readonly string ContrastRed = "Contrast Red";
		public static readonly string ContrastSalmon = "Contrast Salmon";
		public static readonly string ContrastYellow = "Contrast Yellow";
		public static readonly string DodgerBlue = "Dodger Blue";
		public static readonly string FormalBlue = "Formal Blue";
		public static readonly string MildBlue = "Mild Blue";
		public static readonly string MildBrown = "Mild Brown";
		public static readonly string MildCyan = "Mild Cyan";
		public static readonly string MildViolet = "Mild Violet";
		public static readonly string SoftLilac = "Soft Lilac";
	}
}
namespace DevExpress.Snap.API.Native {
	using System.Collections.Generic;
	using DevExpress.XtraRichEdit.API.Native;
	using DevExpress.XtraRichEdit.API.Native.Implementation;
	using DevExpress.Office;
	using DevExpress.Snap.Core.API;
	using ModelTableStyle = DevExpress.XtraRichEdit.Model.TableStyle;
	using ModelTableCellStyle = DevExpress.XtraRichEdit.Model.TableCellStyle;
	using ModelThemeCollection = DevExpress.Snap.Core.Native.ThemeCollection;
	using ModelTheme = DevExpress.Snap.Core.Native.Theme;
	using SnapDocumentModel = DevExpress.Snap.Core.Native.SnapDocumentModel;
	using DevExpress.Office.History;
	using System;
	using DevExpress.Snap.Localization;
	using DevExpress.Office.Utils;
	using DevExpress.Utils;
	#region NativeThemeTableStyleCollection
	public class NativeThemeTableStyleCollection : NativeReadOnlyStyleCollectionBase<TableStyle, ModelTableStyle>, ThemeTableStyleCollection {
		List<ModelTableStyle> modelStyles;
		public NativeThemeTableStyleCollection(NativeDocument document, List<ModelTableStyle> styles)
			: base(document) {
			this.modelStyles = styles;
		}
		public override TableStyle this[int index] { get { return GetStyle(modelStyles[index]); } }
		public override TableStyle this[string name] { get { return GetStyle(modelStyles.Find(style => { return style.StyleName == name; })); } }
		public override int Count { get { return modelStyles.Count; } }
		internal List<ModelTableStyle> ModelStyles { get { return modelStyles; } }
		protected internal override TableStyle CreateNew(ModelTableStyle style) {
			return new NativeTableStyle(Document, style);
		}
	}
	#endregion
	#region NativeThemeTableCellStyleCollection
	public class NativeThemeTableCellStyleCollection : NativeReadOnlyStyleCollectionBase<TableCellStyle, ModelTableCellStyle>, ThemeTableCellStyleCollection {
		List<ModelTableCellStyle> modelStyles;
		public NativeThemeTableCellStyleCollection(NativeDocument document, List<ModelTableCellStyle> styles)
			: base(document) {
			this.modelStyles = styles;
		}
		public override TableCellStyle this[int index] { get { return GetStyle(modelStyles[index]); } }
		public override TableCellStyle this[string name] { get { return GetStyle(modelStyles.Find(style => { return style.StyleName == name; })); } }
		public override int Count { get { return modelStyles.Count; } }
		internal List<ModelTableCellStyle> ModelStyles { get { return modelStyles; } }
		protected internal override TableCellStyle CreateNew(ModelTableCellStyle style) {
			return new NativeTableCellStyle(Document, style);
		}
	}
	#endregion
	#region NativeTheme
	public class NativeTheme : Theme {
		#region Fields
		readonly ModelTheme modelTheme;
		readonly SnapNativeDocument document;
		#endregion
		public NativeTheme(SnapNativeDocument document, ModelTheme modelTheme) {
			this.document = document;
			this.modelTheme = modelTheme;
		}
		#region Properties
		SnapDocumentModel DocumentModel { get { return document.DocumentModel; } }
		internal ModelTheme ModelTheme { get { return modelTheme; } }
		public string Name { get { return ModelTheme.Name; } set { ModelTheme.Name = value; } }
		public System.Drawing.Image Icon {
			get {
				if (ModelTheme.Icon == null)
					return null;
				return ModelTheme.Icon.NativeImage;
			}
			set {
				if (Icon == value)
					return;
				ModelTheme.SetIcon(OfficeImage.CreateImage(value), true);
			}
		}
		public bool IsDefault { get { return ModelTheme.IsDefault; } }
		#endregion
		public void UpdateToMatchDocumentStyles() {
			ModelTheme.Actualize(document.DocumentModel);
		}
	}
	#endregion
	#region NativeThemeCollection
	public class NativeThemeCollection : NativeReadOnlyCollectionBase<Theme, ModelTheme>, ThemeCollection {
		public NativeThemeCollection(NativeDocument document)
			: base(document) {
		}
		#region Properties
		public override Theme this[int index] { get { return GetItem(ModelThemes[index]); } }
		public Theme this[string name] { get { return GetItem(ModelThemes.GetThemeByName(name)); } }
		public override int Count { get { return ModelThemes.Count; } }
		new SnapDocumentModel DocumentModel { get { return base.DocumentModel as SnapDocumentModel; } }
		new SnapNativeDocument Document { get { return base.Document as SnapNativeDocument; } }
		internal ModelThemeCollection ModelThemes { get { return DocumentModel.Themes; } }
		#endregion
		protected internal override Theme CreateNew(ModelTheme item) {
			return new NativeTheme(Document, item);
		}
		public Theme CreateNew() {
			return CreateNew(Themes.Casual);
		}
		public Theme CreateNew(string baseThemeName) {
			ModelTheme baseTheme = ModelThemes.GetThemeByName(baseThemeName);
			if (baseTheme == null)
				Exceptions.ThrowArgumentException("baseThemeName", baseThemeName);
			return GetItem(baseTheme.Clone());
		}
		public void Add(Theme theme) {
			ModelTheme modelTheme = GetModelTheme(theme);
			DocumentModel.AddTheme(modelTheme);
		}
		public void Delete(Theme theme) {
			if (theme.IsDefault)
				Exceptions.ThrowInvalidOperationException(SnapLocalizer.GetString(SnapStringId.Msg_CannotDeleteDefaultTheme));
			ModelTheme modelTheme = GetModelTheme(theme);
			DocumentModel.RemoveTheme(modelTheme);
		}
		ModelTheme GetModelTheme(Theme theme) {
			return ((NativeTheme)theme).ModelTheme;
		}
	}
	#endregion
}

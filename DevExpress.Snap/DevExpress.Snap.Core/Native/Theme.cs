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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Export;
using DevExpress.Snap.Core.Import;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Model.History;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Native {
	#region Theme
	public class Theme : IDisposable, ICloneable<Theme>, ISupportsCopyFrom<Theme>, INotifyPropertyChanged {
		#region Fields
		readonly List<TableStyle> tableStyles;
		readonly List<TableCellStyle> tableCellStyles;
		readonly SnapDocumentModel documentModel;
		OfficeImage icon;
		string name;
		string defaultName;
		bool isLoaded;
		readonly bool isDefault;
		bool suppressUpdateIcon;
		string nativeName;
		int version;
		#endregion
		internal Theme(SnapDocumentModel documentModel, string name, bool isDefault, string nativeName) {
			this.documentModel = documentModel;
			this.tableStyles = new List<TableStyle>();
			this.tableCellStyles = new List<TableCellStyle>();
			this.name = name;
			this.defaultName = name;
			this.isDefault = isDefault;
			this.nativeName = nativeName;
		}
		internal Theme(SnapDocumentModel documentModel, string name, bool isDefault)
			: this(documentModel, name, isDefault, null) {
		}
		public Theme(SnapDocumentModel documentModel, string name)
			: this(documentModel, name, false) {
		}
		internal Theme(SnapDocumentModel documentModel)
			: this(documentModel, null) {
		}
		#region Properties
		internal string NativeName { get { return nativeName; } set { nativeName = value; } }
		internal List<TableCellStyle> TableCellStyles { get { return tableCellStyles; } }
		internal List<TableStyle> TableStyles { get { return tableStyles; } }
		public OfficeImage Icon {
			get { return icon; }
			set {
				if (Icon == value)
					return;
				SetIcon(value, false);
			}
		}
		public string Name {
			get { return name; }
			set {
				if (Name == value)
					return;
				SetName(value);
			}
		}
		public SnapDocumentModel DocumentModel { get { return documentModel; } }
		public bool IsLoaded { get { return isLoaded; } internal set { isLoaded = value; } }
		internal bool IsDefault { get { return isDefault; } }
		internal int Version { get { return version; } }
		public bool IsModified { get { return Version != 0; } }
		internal bool SuppressUpdateIcon { get { return suppressUpdateIcon; } set { suppressUpdateIcon = value; } }
		#endregion
		protected internal virtual void Apply(DocumentModel targetModel) {
			targetModel.BeginUpdate();
			foreach (TableStyle sourceStyle in TableStyles) {
				TableStyle destinationStyle = targetModel.TableStyles.GetStyleByName(sourceStyle.StyleName);
				if (destinationStyle != null) {
					destinationStyle.CopyProperties(sourceStyle);
					destinationStyle.ConditionalStyleProperties.CopyFrom(sourceStyle.ConditionalStyleProperties);
				}
				else
					sourceStyle.Copy(targetModel);
			}
			foreach (TableCellStyle sourceStyle in TableCellStyles) {
				TableCellStyle destinationStyle = targetModel.TableCellStyles.GetStyleByName(sourceStyle.StyleName);
				if (destinationStyle != null)
					destinationStyle.CopyProperties(sourceStyle);
				else
					sourceStyle.Copy(targetModel);
			}
			targetModel.EndUpdate();
		}
		public void Apply() {
			if (!EnsureThemeLoaded())
				Exceptions.ThrowInvalidOperationException(String.Format(SnapLocalizer.GetString(SnapStringId.Msg_ThemeIsNotLoaded), Name));
			Apply(DocumentModel);
		}
		void LoadDefaultTheme(ThemeInfo info) {
			Guard.ArgumentNotNull(info, "info");
			BeginLoadTheme();
			try {				
				Theme sourceTheme = info.SourceModel.Themes[0];
				if (String.IsNullOrEmpty(Name)) {
					Name = sourceTheme.Name;
					this.defaultName = sourceTheme.Name;
				}
				CopyFrom(sourceTheme);
			}
			finally {
				EndLoadTheme();
			}
		}
		public void Load(Stream content) {
			Guard.ArgumentNotNull(content, "content");
			BeginLoadTheme();
			try {
				LoadCore(content);
			}
			finally {
				EndLoadTheme();
			}
		}
		void BeginLoadTheme() {
			DocumentModel.History.DisableHistory();
		}
		void EndLoadTheme() {
			DocumentModel.History.EnableHistory();
			this.version = 0;
		}
		void LoadCore(Stream content) {
			using (SnapDocumentModel targetModel = new SnapDocumentModel(false, DocumentModel.DataSourceDispatcher.CreateNew(), DocumentModel.DocumentFormatsDependencies)) {
				lock (content) {
					if (content.CanSeek)
						content.Position = 0;
					ImportThemeCore(content, targetModel);
				}
				Theme sourceTheme = targetModel.Themes[0];
				if (String.IsNullOrEmpty(Name)) {
					Name = sourceTheme.Name;
					this.defaultName = sourceTheme.Name;
				}
				CopyFrom(sourceTheme);
			}
		}
		protected internal virtual void Actualize(DocumentModel sourceModel) {
			if (!EnsureThemeLoaded())
				Exceptions.ThrowInvalidOperationException(String.Format(SnapLocalizer.GetString(SnapStringId.Msg_ThemeIsNotLoaded), Name));
			DocumentModel.BeginUpdate();
			try {
				int oldVersion = Version;
				BeginThemeChangeTracking();
				foreach (TableStyle destinationStyle in TableStyles) {
					TableStyle sourceStyle = sourceModel.TableStyles.GetStyleByName(destinationStyle.StyleName);
					if (sourceStyle != null) {
						destinationStyle.CopyProperties(sourceStyle);
						destinationStyle.ConditionalStyleProperties.CopyFrom(sourceStyle.ConditionalStyleProperties);
					}
				}
				foreach (TableCellStyle destinationStyle in TableCellStyles) {
					TableCellStyle sourceStyle = sourceModel.TableCellStyles.GetStyleByName(destinationStyle.StyleName);
					if (sourceStyle != null)
						destinationStyle.CopyProperties(sourceStyle);
				}
				EndThemeChangeTracking();
				if (oldVersion != Version && !SuppressUpdateIcon)
					Icon = CreateIcon();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void BeginThemeChangeTracking() {
			DocumentModelChangeTracker.BeginTracking(DocumentModel);
		}
		void EndThemeChangeTracking() {
			DocumentModelChangeTracker.EndTracking(DocumentModel);
			if (DocumentModelChangeTracker.IsChanged) {
				ChangeThemeVersionHistoryItem item = new ChangeThemeVersionHistoryItem(this);
				DocumentModel.History.Add(item);
				item.Execute();
			}
		}
		internal OfficeImage CreateIcon() {
			return OfficeImage.CreateImage(ThemeHelper.CreateBitmapForTheme(this));
		}
		void SetName(string newName) {
			ChangeThemeNameHistoryItem historyItem = new ChangeThemeNameHistoryItem(this);
			historyItem.NewName = newName;
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		internal void SetIcon(OfficeImage icon, bool suppressUpdateIcon) {
			UpdateThemeIconHistoryItem historyItem = new UpdateThemeIconHistoryItem(this, icon, suppressUpdateIcon);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		internal void IncreaseVersion() {
			this.version++;
		}
		internal void DecreaseVersion() {
			this.version--;
		}
		internal void SetNameCore(string newName) {
			this.name = newName;
			OnPropertyChanged("Name");
		}
		internal void SetIconCore(OfficeImage image) {
			this.icon = image;
		}
		public void Save(Stream outputStream) {
			using (SnapDocumentModel documentModel = new SnapDocumentModel(false, DocumentModel.DataSourceDispatcher.CreateNew(), DocumentModel.DocumentFormatsDependencies)) {
				documentModel.BeginUpdate();
				try {
					Theme theme = Clone(documentModel);
					documentModel.Themes.Add(theme);
				}
				finally {
					documentModel.EndUpdate();
				}
				ExportThemeCore(outputStream, documentModel);
			}
		}
		public void CopyFrom(Theme theme) {
			Guard.ArgumentNotNull(theme, "theme");
			DocumentModel.BeginUpdate();
			try {
				Clear();
				Icon = theme.CreateIconClone(DocumentModel);
				foreach (TableStyle tableStyle in theme.TableStyles) {
					TableStyles.Add(CopyTableStyle(tableStyle));
				}
				foreach (TableCellStyle tableCellStyle in theme.TableCellStyles) {
					TableCellStyles.Add(CopyTableCellStyle(tableCellStyle));
				}
				this.isLoaded = theme.IsLoaded;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		OfficeImage CreateIconClone(DocumentModel documentModel) {
			if (Icon == null)
				return null;
			lock (Icon) {
				return Icon.Clone(documentModel);
			}
		}
		TableStyle CopyTableStyle(TableStyle source) {
			TableStyle result = new TableStyle(DocumentModel);
			result.StyleName = source.StyleName;
			result.CopyProperties(source);
			if (source.HasConditionalStyleProperties)
				result.ConditionalStyleProperties.CopyFrom(source.ConditionalStyleProperties);
			if (source.Parent != null) {
				TableStyle parentStyle = TableStyles.Find(style => style.StyleName == source.Parent.StyleName);
				if (parentStyle == null) {
					parentStyle = CopyTableStyle(source.Parent);
					TableStyles.Add(parentStyle);
				}
				result.Parent = parentStyle;
			}
			source.ApplyPropertiesDiff(result);
			return result;
		}
		TableCellStyle CopyTableCellStyle(TableCellStyle source) {
			TableCellStyle result = new TableCellStyle(DocumentModel);
			result.CopyProperties(source);
			result.StyleName = source.StyleName;
			if (source.Parent != null) {
				TableCellStyle parentStyle = TableCellStyles.Find(style => style.StyleName == source.Parent.StyleName);
				if (parentStyle == null) {
					parentStyle = CopyTableCellStyle(source.Parent);
					TableCellStyles.Add(parentStyle);
				}
				result.Parent = parentStyle;
			}
			source.ApplyPropertiesDiff(result);
			return result;
		}
		internal static void ImportThemeCore(Stream stream, SnapDocumentModel documentModel) {
			SnapImporter importer = new SnapImporter(documentModel, new SnapDocumentImporterOptions());
			importer.Import(stream);
		}
		void ExportThemeCore(Stream stream, SnapDocumentModel documentModel) {
			SnapExporter exporter = new SnapExporter(documentModel, new SnapDocumentExporterOptions());
			exporter.Export(stream);
		}
		public void Clear() {
			TableCellStyles.Clear();
			TableStyles.Clear();
			IsLoaded = false;
			this.version = 0;
		}
		public bool EnsureThemeLoaded() {
			if (!IsLoaded) {
				if (String.IsNullOrEmpty(NativeName))
					Exceptions.ThrowInternalException();
				ThemeInfo defaultTheme = ThemeRepository.GetThemeByCaption(NativeName);
				if (defaultTheme != null)
					LoadDefaultTheme(defaultTheme);
			}
			return IsLoaded;
		}
		public void RestoreDefaults() {
			if (!(IsDefault && IsModified))
				return;
			DocumentModel.BeginUpdate();
			try {
				Clear();
				Name = this.defaultName;
				ThemeInfo defaultTheme = ThemeRepository.GetThemeByCaption(NativeName);
				if (defaultTheme != null)
					LoadCore(defaultTheme.Content);
				else
					Exceptions.ThrowInternalException();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (icon != null) {
					icon.Dispose();
					icon = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region ICloneable<Theme> Members
		public Theme Clone() {
			return Clone(DocumentModel);
		}
		Theme Clone(SnapDocumentModel targetModel) {
			Theme result = new Theme(targetModel, Name, false, NativeName);
			targetModel.History.DisableHistory();
			try {
				result.CopyFrom(this);
			}
			finally {
				targetModel.History.EnableHistory();
			}
			return result;
		}
		#endregion
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		void OnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	#endregion
	#region DocumentModelChangeTracker
	public static class DocumentModelChangeTracker {
		static int historyItemsCount = -1;
		public static bool IsChanged { get; set; }
		public static void BeginTracking(DocumentModel documentModel) {
			historyItemsCount = GetHistoryItemsCount(documentModel);
		}
		public static void EndTracking(DocumentModel documentModel) {
			int newCount = GetHistoryItemsCount(documentModel);
			IsChanged = historyItemsCount >= 0 && historyItemsCount < newCount;
			historyItemsCount = -1;
		}
		static int GetHistoryItemsCount(DocumentModel documentModel) {
			CompositeHistoryItem transaction = documentModel.History.Transaction;
			return transaction != null ? transaction.Count : documentModel.History.Count;
		}
	}
	#endregion
	#region ThemeCollection
	public class ThemeCollection : NotificationCollection<Theme> {
		public Theme GetThemeByName(string name) {
			return this.Find((theme) => {
				return theme.Name == name;
			});
		}
		public bool Contains(string themeName) {
			return GetThemeByName(themeName) != null;
		}
	}
	#endregion
	#region ThemeInfo
	public class ThemeInfo : IDisposable {
		SnapDocumentModel sourceModel;
		public ThemeInfo(string name, string caption, string hint, Stream content) {
			Name = name;
			Caption = caption;
			Hint = hint;
			Content = content;
		}
		public string Name { get; private set; }
		public string Caption { get; private set; }
		public string Hint { get; private set; }
		public Stream Content { get; private set; }
		public SnapDocumentModel SourceModel {
			get {
				if (sourceModel == null)
					LoadTheme();
				return sourceModel;
			}
		}
		void LoadTheme() {
			lock (Content) {
				if (sourceModel != null)
					return;
				SnapDocumentModel result = new SnapDocumentModel(false, new ServerDataSourceDispatcher(), SnapDocumentFormatsDependecies.CreateDocumentFormatsDependencies());
				if (Content.CanSeek)
					Content.Position = 0;
				Theme.ImportThemeCore(Content, result);
				sourceModel = result;
			}
		}
		#region IDisposable Members
		public void Dispose() {
			Content.Dispose();
		}
		#endregion
	}
	#endregion
	#region ThemeRepository
	public static class ThemeRepository {
		static Dictionary<string, ThemeInfo> themeInfos = CreateDefaultThemes();
		public static ICollection<ThemeInfo> ThemeInfos { get { return themeInfos.Values; } }
		static void LoadDefaultTheme(Dictionary<string, ThemeInfo> list, string themeFile, string name, string caption) {
			Stream stream = typeof(SnapDocumentModel).Assembly.GetManifestResourceStream(themeFile);
			ThemeInfo theme = new ThemeInfo(name, caption, caption, stream);
			Debug.Assert(!list.ContainsKey(name));
			list[name] = theme;
		}
		static Dictionary<string, ThemeInfo> CreateDefaultThemes() {
			Dictionary<string, ThemeInfo> result = new Dictionary<string, ThemeInfo>();
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.Casual.snt", "Casual", SnapLocalizer.GetString(SnapStringId.ThemeName_Casual));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.ContrastCyan.snt", "Contrast Cyan", SnapLocalizer.GetString(SnapStringId.ThemeName_ContrastCyan));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.ContrastOrange.snt", "Contrast Orange", SnapLocalizer.GetString(SnapStringId.ThemeName_ContrastOrange));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.ContrastRed.snt", "Contrast Red", SnapLocalizer.GetString(SnapStringId.ThemeName_ContrastRed));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.FormalBlue.snt", "Formal Blue", SnapLocalizer.GetString(SnapStringId.ThemeName_FormalBlue));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.MildBrown.snt", "Mild Brown", SnapLocalizer.GetString(SnapStringId.ThemeName_MildBrown));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.MildCyan.snt", "Mild Cyan", SnapLocalizer.GetString(SnapStringId.ThemeName_MildCyan));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.MildViolet.snt", "Mild Violet", SnapLocalizer.GetString(SnapStringId.ThemeName_MildViolet));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.MildBlue.snt", "Mild Blue", SnapLocalizer.GetString(SnapStringId.ThemeName_MildBlue));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.ContrastSalmon.snt", "Contrast Salmon", SnapLocalizer.GetString(SnapStringId.ThemeName_ContrastSalmon));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.SoftLilac.snt", "Soft Lilac", SnapLocalizer.GetString(SnapStringId.ThemeName_SoftLilac));
			LoadDefaultTheme(result, "DevExpress.Snap.Themes.DodgerBlue.snt", "Dodger Blue", SnapLocalizer.GetString(SnapStringId.ThemeName_DodgerBlue));
			return result;
		}
		public static ThemeInfo GetThemeByCaption(string caption) {
			ThemeInfo result;
			themeInfos.TryGetValue(caption, out result);
			return result;
		}
	}
	#endregion
	#region ThemeHelper
	public static class ThemeHelper {
		public static Bitmap CreateBitmapForTheme(Theme theme) {
			Color list1HFColor = GetHeaderFooterColor(theme, "List1-Header", "List1-Footer");
			Color list1GroupHFColor = GetHeaderFooterColor(theme, "List1-GroupHeader1", "List1-GroupFooter1");
			Color list2HFColor = GetHeaderFooterColor(theme, "List2-Header", "List2-Footer");
			Color list2GroupHFColor = GetHeaderFooterColor(theme, "List2-GroupHeader2", "List2-GroupFooter2");
			return CreateBitmap(list1HFColor, list1GroupHFColor, list2HFColor, list2GroupHFColor);
		}
		static Color GetHeaderFooterColor(Theme theme, string headerStyleName, string footerStyleName) {
			Color result = GetTableCellStyleBackgroundColor(theme, headerStyleName);
			if (result.IsEmpty) {
				result = GetTableCellStyleBackgroundColor(theme, footerStyleName);
				if (result.IsEmpty)
					result = Color.White;
			}
			return result;
		}
		static Color GetTableCellStyleBackgroundColor(Theme theme, string styleName) {
			TableCellStyle style = theme.TableCellStyles.Find((s) => {
				return s.StyleName == styleName;
			});
			if (style != null)
				return style.TableCellProperties.BackgroundColor;
			else
				return Color.Empty;
		}
		static Bitmap CreateBitmap(Color list1HFColor, Color list1GroupHF1Color, Color list2HFColor, Color listGroup2HFColor) {
			Bitmap bitmap = new Bitmap(65, 46);
			using (Graphics gr = Graphics.FromImage(bitmap)) {
				gr.FillRectangle(new SolidBrush(list1HFColor), new RectangleF(0, 0, 65, 10));
				gr.FillRectangle(new SolidBrush(list1GroupHF1Color), new RectangleF(0, 11, 65, 11));
				gr.FillRectangle(new SolidBrush(list2HFColor), new RectangleF(0, 23, 65, 10));
				gr.FillRectangle(new SolidBrush(listGroup2HFColor), new RectangleF(0, 34, 65, 11));
			}
			return bitmap;
		}
	}
	#endregion
	#region ChangeActiveThemeHistoryItem
	public class ChangeActiveThemeHistoryItem : RichEditHistoryItem {
		Theme newActiveTheme;
		Theme oldActiveTheme;
		public ChangeActiveThemeHistoryItem(SnapDocumentModel documentModel)
			: base(documentModel.ActivePieceTable) {
		}
		public Theme NewActiveTheme { get { return newActiveTheme; } set { newActiveTheme = value; } }
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		protected override void RedoCore() {
			this.oldActiveTheme = DocumentModel.ActiveTheme;
			DocumentModel.ActiveTheme = NewActiveTheme;
		}
		protected override void UndoCore() {
			DocumentModel.ActiveTheme = this.oldActiveTheme;
		}
	}
	#endregion
	#region AddThemeHistoryItem
	public class AddThemeHistoryItem : RichEditHistoryItem {
		Theme addedTheme;
		public AddThemeHistoryItem(SnapDocumentModel documentModel)
			: base(documentModel.ActivePieceTable) {
		}
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public Theme AddedTheme { get { return addedTheme; } set { addedTheme = value; } }
		protected override void RedoCore() {
			DocumentModel.Themes.Add(AddedTheme);
		}
		protected override void UndoCore() {
			DocumentModel.Themes.Remove(addedTheme);
		}
	}
	#endregion
	#region RemoveThemeHistoryItem
	public class RemoveThemeHistoryItem : RichEditHistoryItem {
		Theme removedTheme;
		public RemoveThemeHistoryItem(SnapDocumentModel documentModel)
			: base(documentModel.ActivePieceTable) {
		}
		public new SnapDocumentModel DocumentModel { get { return (SnapDocumentModel)base.DocumentModel; } }
		public Theme RemovedTheme { get { return removedTheme; } set { removedTheme = value; } }
		protected override void RedoCore() {
			DocumentModel.Themes.Remove(removedTheme);
		}
		protected override void UndoCore() {
			DocumentModel.Themes.Add(removedTheme);
		}
	}
	#endregion
	#region UpdateThemeIconHistoryItem
	public class UpdateThemeIconHistoryItem : RichEditHistoryItem {
		OfficeImage oldIcon;
		OfficeImage newIcon;
		bool oldSuppressUpdateIcon;
		bool newSuppressUpdateIcon;
		Theme theme;
		public UpdateThemeIconHistoryItem(Theme theme, OfficeImage icon, bool suppressUpdateIcon)
			: base(theme.DocumentModel.ActivePieceTable) {
			this.newIcon = icon;
			this.newSuppressUpdateIcon = suppressUpdateIcon;
			this.theme = theme;
		}
		protected override void RedoCore() {
			this.oldIcon = this.theme.Icon;
			this.oldSuppressUpdateIcon = this.theme.SuppressUpdateIcon;
			this.theme.SetIconCore(this.newIcon);
			this.theme.SuppressUpdateIcon = newSuppressUpdateIcon;
			this.theme.IncreaseVersion();
		}
		protected override void UndoCore() {
			this.theme.SetIconCore(this.oldIcon);
			this.theme.SuppressUpdateIcon = this.oldSuppressUpdateIcon;
			this.theme.DecreaseVersion();
		}
	}
	#endregion
	#region ChangeThemeVersionHistoryItem
	public class ChangeThemeVersionHistoryItem : RichEditHistoryItem {
		readonly Theme theme;
		public ChangeThemeVersionHistoryItem(Theme theme)
			: base(theme.DocumentModel.MainPieceTable) {
			this.theme = theme;
		}
		protected override void RedoCore() {
			this.theme.IncreaseVersion();
		}
		protected override void UndoCore() {
			this.theme.DecreaseVersion();
		}
	}
	#endregion
	#region ChangeThemeNameHistoryItem
	public class ChangeThemeNameHistoryItem : RichEditHistoryItem {
		readonly Theme theme;
		string oldName;
		public ChangeThemeNameHistoryItem(Theme theme)
			: base(theme.DocumentModel.MainPieceTable) {
			this.theme = theme;
		}
		public string NewName { get; set; }
		protected override void RedoCore() {
			this.oldName = theme.Name;
			this.theme.SetNameCore(NewName);
			this.theme.IncreaseVersion();
		}
		protected override void UndoCore() {
			this.theme.SetNameCore(this.oldName);
			this.theme.DecreaseVersion();
		}
	}
	#endregion
}

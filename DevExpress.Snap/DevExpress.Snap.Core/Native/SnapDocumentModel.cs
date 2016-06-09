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
using System.Drawing;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Office.History;
using DevExpress.Office.Internal;
using DevExpress.Office.Utils;
using DevExpress.Snap.API.Internal;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.History;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native.Data.Implementations;
using DevExpress.Snap.Core.Native.Operations;
using DevExpress.Snap.Core.Native.Options;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Snap.Core.Native.Templates;
using DevExpress.Snap.Core.Options;
using DevExpress.Snap.Core.Services;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Native.Services;
using DevExpress.Utils;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Utils;
using ApiDataAccessService = DevExpress.Snap.Services.DataAccessService;
namespace DevExpress.Snap.Core.Native {
	public class SnapDocumentModel : DocumentModel {
		internal const string DefaultListStyleName = "List";
		internal const string DefaultListHeaderStyleName = "Header";
		internal const string DefaultListFooterStyleName = "Footer";
		internal const string DefaultGroupHeaderStyleName = "GroupHeader";
		internal const string DefaultGroupFooterStyleName = "GroupFooter";
		internal string DefaultThemeName = SnapLocalizer.GetString(SnapStringId.ThemeName_Casual);
		readonly string DefaultDataSourceName = "Data Source";
		int dataSourceNameIndex = 1;
		DataSourceInfoCollection fDataSources = new DataSourceInfoCollection();
		IFieldContext rootDataContext;
		SnapSelectionInfo selectionInfo;
		DocumentSaveOptions fileExportOptions;
		DevExpress.Snap.Core.API.ParameterCollection parameters = new DevExpress.Snap.Core.API.ParameterCollection();
		LastInsertedChartRunInfo lastInsertedChartRunInfo;
		int dataSourceUpdateCount;
		ThemeCollection themes = new ThemeCollection();
		Theme activeTheme;
		IDataSourceDispatcher dataSourceDispatcher;
		public SnapDocumentModel(IDataSourceDispatcher dataSourceDispatcher)
			: this(dataSourceDispatcher, SnapDocumentFormatsDependecies.CreateDocumentFormatsDependencies()) {		
		}
		public SnapDocumentModel(IDataSourceDispatcher dataSourceDispatcher, DocumentFormatsDependencies documentFormatsDependencies)
			: this(true, dataSourceDispatcher, documentFormatsDependencies) {
		}
		internal SnapDocumentModel(bool createDefaultStyles, IDataSourceDispatcher dataSourceDispatcher, DocumentFormatsDependencies documentFormatsDependencies) : base(documentFormatsDependencies) {
			selectionInfo = new SnapSelectionInfo(this);
			this.dataSourceDispatcher = dataSourceDispatcher;
			this.dataSourceDispatcher.IncRefCount();
			dataSourceDispatcher.Initialize(this);
			CreateSnapOptions();
			fDataSources.CollectionChanged += new EventHandler(OnDataSourceCollectionChanged);
			fDataSources.DataSourceChanged += new DataSourceChangedEventHandler(OnNamedDataSourceChanged);
			parameters.ListChanged += new ListChangedEventHandler(OnParametersChanged);
			this.lastInsertedChartRunInfo = new LastInsertedChartRunInfo() { PieceTable = MainPieceTable };
			if (createDefaultStyles) {
				GenerateDefaultThemes();
				SetDefaultTheme();
			}
		}
		#region Events
		public event ConfigureDataConnectionEventHandler ConfigureDataConnection;
		public event CustomFilterExpressionEventHandler CustomFilterExpression;
		public event ConnectionErrorEventHandler ConnectionError;
		event EventHandler currentSnapBookmarkChanged;
		public event EventHandler CurrentSnapBookmarkChanged { add { currentSnapBookmarkChanged += value; } remove { currentSnapBookmarkChanged = Delegate.Remove(currentSnapBookmarkChanged, value) as EventHandler; } }
		protected internal virtual void RaiseCurrentSnapBookmarkChanged() {
			if (currentSnapBookmarkChanged != null)
				currentSnapBookmarkChanged(this, EventArgs.Empty);
		}
		event EventHandler dataSourceChanged;
		public event EventHandler DataSourceChanged { add { dataSourceChanged += value; } remove { dataSourceChanged = Delegate.Remove(dataSourceChanged, value) as EventHandler; } }
		protected internal virtual void OnDataSourceChanged() {
			if (dataSourceUpdateCount == 0 && dataSourceChanged != null) {
				ResetRootDataContext();
				dataSourceChanged(this, EventArgs.Empty);
			}
		}
		#region BeforeConversion
		EventHandler onBeforeConversion;
		public event EventHandler BeforeConversion { add { onBeforeConversion += value; } remove { onBeforeConversion -= value; } }
		public virtual void RaiseBeforeConversion() {
			if (onBeforeConversion != null)
				onBeforeConversion(this, EventArgs.Empty);
		}
		#endregion
		#region SnapMailMergeDataSourceChanged
		event EventHandler snapMailMergeDataSourceChanged;
		internal event EventHandler SnapMailMergeDataSourceChanged {
			add { snapMailMergeDataSourceChanged += value; }
			remove { snapMailMergeDataSourceChanged -= value; }
		}
		protected internal virtual void RaiseSnapMailMergeDataSourceChanged() {
			if (snapMailMergeDataSourceChanged != null)
				snapMailMergeDataSourceChanged(this, EventArgs.Empty);
		}
		#endregion
		#region SnapMailMergeActiveRecordChanging
		EventHandler onSnapMailMergeActiveRecordChanging;
		public event EventHandler SnapMailMergeActiveRecordChanging { add { onSnapMailMergeActiveRecordChanging += value; } remove { onSnapMailMergeActiveRecordChanging -= value; } }
		protected internal virtual void RaiseSnapMailMergeActiveRecordChanging() {
			if (onSnapMailMergeActiveRecordChanging != null)
				onSnapMailMergeActiveRecordChanging(this, EventArgs.Empty);
		}
		#endregion
		#region SnapMailMergeActiveRecordChanged
		EventHandler onSnapMailMergeActiveRecordChanged;
		public event EventHandler SnapMailMergeActiveRecordChanged { add { onSnapMailMergeActiveRecordChanged += value; } remove { onSnapMailMergeActiveRecordChanged -= value; } }
		protected internal virtual void RaiseSnapMailMergeActiveRecordChanged() {
			if (onSnapMailMergeActiveRecordChanged != null)
				onSnapMailMergeActiveRecordChanged(this, EventArgs.Empty);
		} 
		#endregion
		#region SnapMailMergeStarted
		SnapMailMergeStartedEventHandler onSnapMailMergeStarted;
		public event SnapMailMergeStartedEventHandler SnapMailMergeStarted { add { onSnapMailMergeStarted += value; } remove { onSnapMailMergeStarted -= value; } }
		protected internal virtual bool RaiseSnapMailMergeStarted(SnapMailMergeStartedEventArgs args) {
			if (onSnapMailMergeStarted != null) {
				onSnapMailMergeStarted(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region SnapMailMergeRecordStarted
		SnapMailMergeRecordStartedEventHandler onSnapMailMergeRecordStarted;
		public event SnapMailMergeRecordStartedEventHandler SnapMailMergeRecordStarted { add { onSnapMailMergeRecordStarted += value; } remove { onSnapMailMergeRecordStarted -= value; } }
		protected internal virtual bool RaiseSnapMailMergeRecordStarted(SnapMailMergeRecordStartedEventArgs args) {
			if (onSnapMailMergeRecordStarted != null) {
				onSnapMailMergeRecordStarted(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region SnapMailMergeRecordRecordFinished
		SnapMailMergeRecordFinishedEventHandler onSnapMailMergeRecordFinished;
		public event SnapMailMergeRecordFinishedEventHandler SnapMailMergeRecordFinished { add { onSnapMailMergeRecordFinished += value; } remove { onSnapMailMergeRecordFinished -= value; } }
		protected internal virtual bool RaiseSnapMailMergeRecordFinished(SnapMailMergeRecordFinishedEventArgs args) {
			if (onSnapMailMergeRecordFinished != null) {
				onSnapMailMergeRecordFinished(this, args);
				return !args.Cancel;
			}
			return true;
		}
		#endregion
		#region SnapMailMergeFinished
		SnapMailMergeFinishedEventHandler onSnapMailMergeFinished;
		public event SnapMailMergeFinishedEventHandler SnapMailMergeFinished { add { onSnapMailMergeFinished += value; } remove { onSnapMailMergeFinished -= value; } }
		protected internal virtual void RaiseSnapMailMergeFinished(SnapMailMergeFinishedEventArgs args) {
			if (onSnapMailMergeFinished != null)
				onSnapMailMergeFinished(this, args);
		}
		#endregion
		#region ActiveThemeChanged
		EventHandler onActiveThemeChanged;
		internal event EventHandler ActiveThemeChanged { add { onActiveThemeChanged += value; } remove { onActiveThemeChanged -= value; } }
		protected internal virtual void OnActiveThemeChanged() {
			if (onActiveThemeChanged != null)
				onActiveThemeChanged(this, EventArgs.Empty);
		}
		#endregion
		#region BeforeDataSourceExport
		BeforeDataSourceExportEventHandler onBeforeDataSourceExport;
		public event BeforeDataSourceExportEventHandler BeforeDataSourceExport { add { onBeforeDataSourceExport += value; } remove { onBeforeDataSourceExport -= value; } }
		protected internal virtual void RaiseBeforeDataSourceExport(BeforeDataSourceExportEventArgs e) {
			if (onBeforeDataSourceExport != null)
				onBeforeDataSourceExport(this, e);
		}
		#endregion
		#region AfterDataSourceImport
		AfterDataSourceImportEventHandler onAfterDataSourceImport;
		public event AfterDataSourceImportEventHandler AfterDataSourceImport { add { onAfterDataSourceImport += value; } remove { onAfterDataSourceImport -= value; } }
		protected internal virtual void RaiseAfterDataSourceImport(AfterDataSourceImportEventArgs e) {
			if (onAfterDataSourceImport != null)
				onAfterDataSourceImport(this, e);
		}
		#endregion
		#region AsynchronousOperationPerforming
		protected internal bool AsynchronousOperationPerforming { get; private set; }
		EventHandler asynchronousOperationStarted;
		public event EventHandler AsynchronousOperationStarted { add { asynchronousOperationStarted += value; } remove { asynchronousOperationStarted -= value; } }
		protected internal virtual void RaiseAsynchronousOperationStarted() {
			AsynchronousOperationPerforming = true;
			if (asynchronousOperationStarted != null)
				asynchronousOperationStarted(this, EventArgs.Empty);
		}
		#endregion
		#region AsynchronousOperationFinished
		EventHandler asynchronousOperationFinished;
		public event EventHandler AsynchronousOperationFinished { add { asynchronousOperationFinished += value; } remove { asynchronousOperationFinished -= value; } }
		protected internal virtual void RaiseAsynchronousOperationFinished() {
			AsynchronousOperationPerforming = false;
			if (asynchronousOperationFinished != null)
				asynchronousOperationFinished(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		internal Func<SnapDocumentModel> CreateModelForExportFunction { get; set; }
		object parametersDataSource;
		internal object ParametersDataSource {
			get {
				if (parametersDataSource == null) {
					parametersDataSource = new ParametersDataSource(parameters, "Parameters"); 
				}
				return parametersDataSource;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new RichEditMailMergeOptions MailMergeOptions { get { throw new NotImplementedException(); } }
		public DataSourceInfoCollection DataSources {
			get { return fDataSources; }
		}
		public DevExpress.Snap.Core.API.ParameterCollection Parameters {
			get { return parameters; }
		}
		public override bool SeparateModelForApiExport { get { return true; } }
		public Stack<EnteredBookmarkInfo> EnteredBookmarks { get { return selectionInfo.EnteredBookmarks; } }
		public SnapSelectionInfo SelectionInfo { get { return selectionInfo; } }
		public new SnapFieldOptions FieldOptions { get { return (SnapFieldOptions)base.FieldOptions; } }
		public override int MaxFieldSwitchLength { get { return Int32.MaxValue / 2; } }
		public override bool EnableFieldNames { get { return true; } }
		public new SnapPieceTable MainPieceTable { get { return (SnapPieceTable)base.MainPieceTable; } }
		public new SnapPieceTable ActivePieceTable { get { return (SnapPieceTable)base.ActivePieceTable; } }
		public new SnapInternalAPI InternalAPI { get { return (SnapInternalAPI)base.InternalAPI; } }
		public new SnxDocumentSaveOptions DocumentSaveOptions { get { return (SnxDocumentSaveOptions)base.DocumentSaveOptions; } }
		public DocumentSaveOptions FileExportOptions { get { return fileExportOptions; } }
		public SnapMailMergeVisualOptions SnapMailMergeVisualOptions { get; protected internal set; }
		public ThemeCollection Themes { get { return themes; } }
		public Theme ActiveTheme {
			get { return activeTheme; }
			internal set {
				if (ActiveTheme == value)
					return;
				activeTheme = value;
				OnActiveThemeChanged();
			}
		}
		internal IDataSourceDispatcher DataSourceDispatcher { get { return dataSourceDispatcher; } }
		protected internal virtual void CreateSnapOptions() {
			this.SnapMailMergeVisualOptions = CreateSnapMailMergeVisualOptions();
			this.fileExportOptions = CreateFileExportOptions();
		}
		protected  override Office.History.DocumentHistory CreateDocumentHistory() {
			return new SnapDocumentHistory(this);
		}
		internal Field HighlightedField { get; set; }
		void OnParametersChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
			OnDataSourceChanged();
		}
		protected internal string[] GetListNames() {
			List<string> listNames = new List<string>();
			this.MainPieceTable.Fields.ForEach(field => {
				SnapFieldCalculatorService calculator = new SnapFieldCalculatorService();
				SNListField snList = calculator.ParseField(MainPieceTable, field) as SNListField;
				if (snList != null && !listNames.Contains(snList.Name)) {
					listNames.Add(snList.Name);
				}
			});
			return listNames.ToArray();
		}
		protected internal string GetListName(List<string> listNames, string dataSourceName) {
			string baseName = dataSourceName;
			if (string.IsNullOrEmpty(dataSourceName) && DataSources.DefaultDataSourceInfo.DataSource != null)
				baseName = DataSources.DefaultDataSourceInfo.DataSource.GetType().Name;
			return NameHelper.GetName(listNames, baseName);
		}
		public void BeginUpdateDataSource() {
			dataSourceUpdateCount++;
		}
		public void EndUpdateDataSource() {
			dataSourceUpdateCount--;
			if (dataSourceUpdateCount == 0)
				OnDataSourceChanged();
		}
		protected virtual void OnNamedDataSourceChanged(object sender, DataSourceChangedEventArgs e) {
			OnNamedDataSourceChanged(e);
		}
		protected internal virtual void OnNamedDataSourceChanged(DataSourceChangedEventArgs e) {
			if (e.DataSourceChangeType == DataSourceChangeType.DataSource) {
				ResetRootDataContext();
				UpdateFields();
			}
			OnDataSourceChanged();
		}
		protected internal void UpdateFields() {
			GetPieceTables(false).ForEach(pieceTable => pieceTable.FieldUpdater.UpdateFields(UpdateFieldOperationType.Normal));
			ForceSelectionLeaveFieldResultEnd();
		}
		void ForceSelectionLeaveFieldResultEnd() {
			DocumentModelPosition position = Selection.Interval.Start;
			RunIndex runIndex = position.RunIndex;
			if (Selection.PieceTable.Runs[runIndex] is FieldResultEndRun) {
				IVisibleTextFilter visibleTextFilter = Selection.PieceTable.VisibleTextFilter;
				DocumentLogPosition newPosition = visibleTextFilter.GetNextVisibleLogPosition(position, false);
				Selection.Start = newPosition;
				Selection.End = newPosition;
			}
		}
		protected virtual void OnDataSourceCollectionChanged(object sender, EventArgs e) {
			OnDataSourceChanged();
		}
		public override DocumentModel CreateDocumentModelForExport(Action<DocumentModel> initializeEmptyDocumentModel) {
			RaiseBeforeConversion();
			if (CreateModelForExportFunction != null)
				return CreateModelForExportFunction();
			UpdateDocument();
			return base.CreateDocumentModelForExport(initializeEmptyDocumentModel);
		}
		protected void UpdateDocument() {
			SnapCaretPosition caretPosition = SelectionInfo.GetSnapCaretPositionFromSelection(Selection.PieceTable, Selection.NormalizedStart);
			BeginUpdate();
			SelectionInfo.UpdateDocument(caretPosition);
			EndUpdate();
		}
		protected override void InitializeDefaultProperties() {
			base.InitializeDefaultProperties();
			DefaultCharacterProperties.BeginInit();
			DefaultCharacterProperties.DoubleFontSize = 18;
			DefaultCharacterProperties.FontName = "Segoe UI";
			DefaultCharacterProperties.EndInit();
		}
		public void LoadTheme(Stream content) {
			Guard.ArgumentNotNull(content, "content");
			Theme theme = new Theme(this);
			theme.Load(content);
			if (!theme.IsLoaded)
				Exceptions.ThrowArgumentException("content", content);
			BeginUpdate();
			try {
				AddTheme(theme);
				ApplyTheme(theme);
			}
			finally {
				EndUpdate();
			}
		}
		internal void AddTheme(Theme theme) {
			if (Themes.Contains(theme.Name))
				throw new ArgumentException(String.Format(SnapLocalizer.GetString(SnapStringId.Msg_CollectionAlreadyContainsTheme), theme.Name));
			AddThemeCore(theme);
		}
		void AddThemeCore(Theme theme) {
			AddThemeHistoryItem historyItem = new AddThemeHistoryItem(this);
			historyItem.AddedTheme = theme;
			History.Add(historyItem);
			historyItem.Execute();
		}
		public void ApplyTheme(string themeName) {
			Theme theme = Themes.GetThemeByName(themeName);
			if (theme == null)
				Exceptions.ThrowArgumentException("themeName", themeName);
			ApplyTheme(theme);
		}
		void GenerateDefaultThemes() {
			Themes.BeginUpdate();
			try {
				foreach (ThemeInfo themeInfo in ThemeRepository.ThemeInfos) {
					Theme theme = new Theme(this, themeInfo.Caption, true, themeInfo.Name);
					Themes.Add(theme);
				}
			}
			finally {
				Themes.EndUpdate();
			}
		}
		public void SaveCurrentTheme(Stream stream) {
			SaveCurrentTheme(stream, null);
		}
		public void SaveCurrentTheme(Stream stream, string newName) {
			Guard.ArgumentNotNull(stream, "stream");
			Theme currentTheme = ActiveTheme != null ? ActiveTheme : Themes.GetThemeByName(DefaultThemeName);
			if (currentTheme == null)
				Exceptions.ThrowInternalException();
			using (Theme theme = currentTheme.Clone()) {
				History.DisableHistory();
				try {
					theme.Actualize(this);
				}
				finally {
					History.EnableHistory();
				}
				if (!String.IsNullOrEmpty(newName)) {
					theme.Name = newName;
					theme.NativeName = newName;
				}
				theme.Save(stream);
			}
		}
		protected internal virtual void SetDefaultTheme() {
			Theme theme = Themes.GetThemeByName(DefaultThemeName);
			if (theme != null) {
				bool disabled = History.IsHistoryDisabled;
				if(!disabled)
					History.DisableHistory();
				try {
					using (HistoryTransaction transaction = new HistoryTransaction(History)) {
						theme.Apply();
						ChangeActiveTheme(null);
					}
				}
				finally {
					if(!disabled)
						History.EnableHistory();
				}
			}
		}
		protected internal virtual void RestoreDefaults(Theme theme) {
			BeginUpdate();
			try {
				theme.RestoreDefaults();
				if (Object.ReferenceEquals(theme, ActiveTheme))
					ChangeActiveTheme(null);
			}
			finally {
				EndUpdate();
			}
		}
		internal void ApplyTheme(Theme theme) {
			using (HistoryTransaction transaction = new HistoryTransaction(History)) {
				theme.Apply();
				ChangeActiveTheme(theme);
			}
		}
		void ChangeActiveTheme(Theme theme) {
			ChangeActiveThemeHistoryItem historyItem = new ChangeActiveThemeHistoryItem(this);
			historyItem.NewActiveTheme = theme;
			History.Add(historyItem);
			historyItem.Execute();
		}
		public void RemoveTheme(Theme theme) {
			if (theme != null) {
				using (HistoryTransaction transaction = new HistoryTransaction(History)) {
					if (Object.ReferenceEquals(theme, ActiveTheme))
						ChangeActiveTheme(null);
					RemoveThemeCore(theme);
				}
			}
		}
		void RemoveThemeCore(Theme theme) {
			RemoveThemeHistoryItem historyItem = new RemoveThemeHistoryItem(this);
			historyItem.RemovedTheme = theme;
			History.Add(historyItem);
			historyItem.Execute();
		}
		public void ActualizeTheme(Theme theme) {
			Guard.ArgumentNotNull(theme, "theme");
			theme.Actualize(this);
		}
		internal bool IsDefaultStyle(string styleName) {
			if (string.IsNullOrEmpty(styleName))
				return false;
			bool isDefaultListStyleName = IsDefaultListStyle(styleName);
			if (isDefaultListStyleName)
				return true;
			if (styleName.StartsWith(DefaultListHeaderStyleName))
				return IsAllDigits(styleName.Substring(DefaultListHeaderStyleName.Length));
			if (styleName.StartsWith(DefaultListFooterStyleName))
				return IsAllDigits(styleName.Substring(DefaultListFooterStyleName.Length));
			if (styleName.StartsWith(DefaultGroupHeaderStyleName))
				return IsAllDigits(styleName.Substring(DefaultGroupHeaderStyleName.Length));
			if (styleName.StartsWith(DefaultGroupFooterStyleName))
				return IsAllDigits(styleName.Substring(DefaultGroupFooterStyleName.Length));
			return false;
		}
		internal int GetDefaultListStyleNameLevel(string styleName) {
			if (!IsDefaultListStyle(styleName))
				return -1;
			return Convert.ToInt32(styleName.Substring(DefaultListStyleName.Length));
		}
		bool IsDefaultListStyle(string styleName) {
			if (string.IsNullOrEmpty(styleName))
				return false;
			if (styleName.StartsWith(DefaultListStyleName)) {
				return IsAllDigits(styleName.Substring(DefaultListStyleName.Length));
			}
			return false;
		}
		static bool IsAllDigits(string source) {
			if (string.IsNullOrEmpty(source))
				return false;
			for (int i = 0; i < source.Length; i++) {
				if (!char.IsDigit(source[i]))
					return false;
			}
			return true;
		}
		#region Comment
		protected internal override void InitializeDefaultStyles() {
		}
		#endregion
		void InitListHeader1Style(TableCellStyle listHeader1Style) {
			listHeader1Style.TableCellProperties.BeginInit();
			listHeader1Style.TableCellProperties.GeneralSettings.BeginInit();
			listHeader1Style.TableCellProperties.CellMargins.Top.BeginInit();
			listHeader1Style.TableCellProperties.CellMargins.Top.OnBeginAssign();
			listHeader1Style.TableCellProperties.CellMargins.Top.Type = WidthUnitType.ModelUnits;
			listHeader1Style.TableCellProperties.CellMargins.Top.Value = this.UnitConverter.HundredthsOfInchToModelUnits(9);
			listHeader1Style.TableCellProperties.CellMargins.Top.OnEndAssign();
			listHeader1Style.TableCellProperties.CellMargins.Top.EndInit();
			listHeader1Style.TableCellProperties.CellMargins.Bottom.BeginInit();
			listHeader1Style.TableCellProperties.CellMargins.Bottom.OnBeginAssign();
			listHeader1Style.TableCellProperties.CellMargins.Bottom.Type = WidthUnitType.ModelUnits;
			listHeader1Style.TableCellProperties.CellMargins.Bottom.Value = this.UnitConverter.HundredthsOfInchToModelUnits(9);
			listHeader1Style.TableCellProperties.CellMargins.Bottom.OnEndAssign();
			listHeader1Style.TableCellProperties.CellMargins.Bottom.EndInit();
			listHeader1Style.TableCellProperties.Borders.RightBorder.BeginInit();
			listHeader1Style.TableCellProperties.Borders.RightBorder.OnBeginAssign();
			listHeader1Style.TableCellProperties.Borders.RightBorder.Color = Color.White;
			listHeader1Style.TableCellProperties.Borders.RightBorder.Style = BorderLineStyle.Single;
			listHeader1Style.TableCellProperties.Borders.RightBorder.Width = this.UnitConverter.PointsToModelUnits(1);
			listHeader1Style.TableCellProperties.Borders.RightBorder.OnEndAssign();
			listHeader1Style.TableCellProperties.Borders.RightBorder.EndInit();
			listHeader1Style.TableCellProperties.Borders.BottomBorder.BeginInit();
			listHeader1Style.TableCellProperties.Borders.BottomBorder.OnBeginAssign();
			listHeader1Style.TableCellProperties.Borders.BottomBorder.Color = Color.White;
			listHeader1Style.TableCellProperties.Borders.BottomBorder.Style = BorderLineStyle.Single;
			listHeader1Style.TableCellProperties.Borders.BottomBorder.Width = this.UnitConverter.PointsToModelUnits(1);
			listHeader1Style.TableCellProperties.Borders.BottomBorder.OnEndAssign();
			listHeader1Style.TableCellProperties.Borders.BottomBorder.EndInit();
			listHeader1Style.CharacterProperties.BeginInit();
			listHeader1Style.CharacterProperties.ForeColor = Color.White;
			listHeader1Style.TableCellProperties.GeneralSettings.BackgroundColor = Color.FromArgb(127, 127, 127);
			listHeader1Style.CharacterProperties.EndInit();
			listHeader1Style.TableCellProperties.GeneralSettings.EndInit();
			listHeader1Style.TableCellProperties.EndInit();
		}
		protected internal override void OnLastEndUpdateCore() {
			if ((DeferredChanges.ChangeActions & DocumentModelChangeActions.RaiseEmptyDocumentCreated) != 0)
				if (object.ReferenceEquals(DataSourceDispatcher.GetDataSource(SnapMailMergeVisualOptions.DataSourceName), null))
					ResetMailMergeOptions();
			base.OnLastEndUpdateCore();
		}
		public override void LoadDocument(Stream stream, DocumentFormat documentFormat, string sourceUri, System.Text.Encoding encoding) {
			ResetMailMergeOptions();
			base.LoadDocument(stream, documentFormat, sourceUri, encoding);
		}
		void ResetMailMergeOptions() {
			NativeSnapMailMergeVisualOptions options = SnapMailMergeVisualOptions as NativeSnapMailMergeVisualOptions;
			if (options != null)
				options.ResetMailMerge();
		}
		protected internal override FieldOptions CreateFieldOptions() {
			return new SnapFieldOptions();
		}
		protected internal override DocumentSaveOptions CreateDocumentSaveOptions() {
			return new SnxDocumentSaveOptions();
		}
		protected internal virtual DocumentSaveOptions CreateFileExportOptions() {
			return new DocumentSaveOptions();
		}
		protected internal override InternalAPI CreateInternalAPI() {
			return new SnapInternalAPI(this, DocumentFormatsDependencies.ExportersFactory, DocumentFormatsDependencies.ImportersFactory);
		}
		protected internal override ICommandsCreationStrategy CreateCommandCreationStrategy() {
			return new SnapCommandsCreationStrategy();
		}
		protected SnapDocumentModel(bool addDefaultsList, bool changeDefaultTableStyle, DocumentFormatsDependencies documentFormatsDependencies)
			: base(addDefaultsList, changeDefaultTableStyle, documentFormatsDependencies) {
		}
		protected internal override void ClearDocumentCore(bool addDefaultsList, bool changeDefaultTableStyle) {
			base.ClearDocumentCore(addDefaultsList, changeDefaultTableStyle);
			if (Themes.Count > 0) {
				Themes.Clear();
				GenerateDefaultThemes();
			}
			this.activeTheme = null;
			selectionInfo = new SnapSelectionInfo(this);
			DataSources.Clear();
			DataSources.SetDefaultDataSource(null);
		}
		public override void ClearCore() {
			base.ClearCore();
			ResetRootDataContext();
		}
		public override void BeginSetContent() {
			base.BeginSetContent();
			SetDefaultTheme();
			ResetRootDataContext();
		}
		protected internal virtual void ResetRootDataContext() {
			rootDataContext = null;
			SetMailMergeRootDataContext();
		}
		protected internal void SetMailMergeRootDataContext() {
			if (Object.ReferenceEquals(SnapMailMergeVisualOptions, null))
				return;
			DataSourceInfo info;
			if (object.ReferenceEquals(SnapMailMergeVisualOptions.DataSourceName, null))
				return;
			info = DataSourceDispatcher.GetInfo(SnapMailMergeVisualOptions.DataSourceName);
			SetRootDataContext(info);
		}
		protected internal void SetRootDataContext(DataSourceInfo info) {
			SetRootDataContext(info, SnapMailMergeVisualOptions, SnapMailMergeVisualOptions.CurrentRecordIndex);
		}
		protected internal void SetRootDataContext(DataSourceInfo info, IDataDispatcherOptions options, int currentRecordIndex) {
			SetRootDataContext(CreateRootDataContext(info, options, currentRecordIndex));
		}
		internal IFieldContext CreateRootDataContext(DataSourceInfo info, IDataDispatcherOptions options, int currentRecordIndex) {
			if (info == null)
				return null;
			ListFieldContext listContext = CreateListContext(info.DataSourceName, options);
			IFieldDataAccessService fieldDataAccessService = GetService<IFieldDataAccessService>();
			IFieldContextService fieldContextService = fieldDataAccessService.FieldContextService;
			DataControllerCalculationContext calculationContext = fieldContextService.BeginCalculation(DataSourceDispatcher) as DataControllerCalculationContext;
			try {
				if (calculationContext == null)
					return null;
				SnapListController controller = calculationContext.GetListController(listContext);
				if (controller == null)
					return null;
				int visibleIndex = controller.GetVisibleIndex(currentRecordIndex);
				SingleListItemFieldContext singleListItemContext = new SingleListItemFieldContext(listContext, visibleIndex, currentRecordIndex, currentRecordIndex + 1);
				return singleListItemContext;
			}
			finally {
				fieldContextService.EndCalculation(calculationContext);
			}
		}
		ListFieldContext CreateListContext(string dataSourceName, IDataDispatcherOptions options) {
			ISingleObjectFieldContext listParent = new SnapMailMergeRootFieldContext(DataSourceDispatcher, dataSourceName);
			if (!String.IsNullOrEmpty(options.DataMember))
				listParent = new SimplePropertyFieldContext(listParent, options.DataMember);
			List<GroupProperties> groups = MailMergeParamsConverter.ConvertToGroupPropertiesList(options.Sorting);
			FilterProperties filters = MailMergeParamsConverter.ConvertToFilterProperties(options.FilterString);
			return new ListFieldContext(listParent, new ListParameters(groups, filters));
		}
		protected internal void AddDataSource(IDataComponent dataSource) {
			DataSourceInfo changingDataSourceInfo = DataSources[dataSource.Name];
			if (changingDataSourceInfo == null)
				AddDataSourceCore(dataSource);
			else
				ChangeDataSource(changingDataSourceInfo, dataSource);
		}
		void AddDataSourceCore(IDataComponent newDataSource) {
			AddDataSourceHistoryItem historyItem = new AddDataSourceHistoryItem(this, newDataSource);
			History.Add(historyItem);
			historyItem.Execute();
		}
		void ChangeDataSource(DataSourceInfo changingDataSourceInfo, IDataComponent newDataSource) {
			ChangeDataSourceHistoryItem historyItem = new ChangeDataSourceHistoryItem(this, newDataSource, changingDataSourceInfo);
			History.Add(historyItem);
			historyItem.Execute();
		}
		protected override void AddDataServices() {
			AddService(typeof(IMailMergeDataService), new SnapMailMergeDataService(MailMergeDataController, this));
			AddService(typeof(IFieldDataService), new SnapFieldDataService());
			AddService(typeof(IFieldCalculatorService), new SnapFieldCalculatorService());
			FieldDataAccessService fieldDataAccessService = new FieldDataAccessService(this, new FieldPathService(), new DataControllerFieldContextService(parameters));
			AddService(typeof(IFieldDataAccessService), fieldDataAccessService);
			AddService(typeof(IUpdateFieldService), fieldDataAccessService);
			AddService(typeof(IFilterStringAccessService), new FilterStringAccessService(this));
			AddService(typeof(IDataAccessService), new DataAccessService(this));
			AddService(typeof(ApiDataAccessService), new ApiDataAccessService(this));
			AddService(typeof(IDataSourceDisplayNameProvider), new DataSourceDisplayNameProvider(this));
			AddService(typeof(ISelectedFieldService), new SelectedFieldService());
			AddService(typeof(IDataSourceNameCreationService), new DataSourceNameCreationService(this));
		}
		protected virtual void AddThemeService() {
			ThemeService themeService = new ThemeService();
			foreach (ThemeInfo item in ThemeRepository.ThemeInfos) {
				themeService.AddTheme(item);
			}
			AddService(typeof(IThemeService), themeService);
		}
		protected internal override void AddServices() {
			base.AddServices();
			AddService(typeof (IDataConnectionParametersService), new DataConnectionParametersService(this));
			AddService(typeof(ICustomRunBoxLayoutExporterService), new CustomRunBoxLayoutExporterService());
			AddService(typeof(IDropFieldsService), new SnapDropFieldsService());
			AddService(typeof(ITableViewInfoControllerService), new TableViewInfoControllerService());
			AddService(typeof(IChartService), new ChartService());
			AddService(typeof(IExportService), new SnapExportService());
			AddService(typeof(IDBSchemaProvider), new DBSchemaProvider());
		}
		protected internal TemplateBuilder CreateTemplateBuilder() {
			return new TemplateBuilder();
		}
		protected internal virtual SnapMailMergeVisualOptions CreateSnapMailMergeVisualOptions() {
			return new NativeSnapMailMergeVisualOptions(this);
		}
		public override void InheritDataServices(DocumentModel documentModel) {
			InheritDataServicesCore(documentModel);
			InheritDataSources(documentModel);
		}
		public void InheritDataServicesForMailMerge(DocumentModel documentModel) {
			InheritDataServicesCore(documentModel);
			InheritDataSourcesForMailMerge(documentModel);
		}
		void InheritDataServicesCore(DocumentModel documentModel) {
			base.InheritDataServices(documentModel);
			InheritService<IFieldDataService>(documentModel);
			InheritService<IFieldCalculatorService>(documentModel);
			InheritService<IFieldDataAccessService>(documentModel);
			InheritService<IChartService>(documentModel);
			parameters = ((SnapDocumentModel)documentModel).Parameters;
		}
		void InheritDataSources(DocumentModel documentModel) {
			SnapDocumentModel snapDocumentModel = (SnapDocumentModel)documentModel;
			fDataSources = snapDocumentModel.fDataSources;
			if (!object.ReferenceEquals(this.dataSourceDispatcher, snapDocumentModel.DataSourceDispatcher)) {
				this.dataSourceDispatcher.Dispose();
				this.dataSourceDispatcher = snapDocumentModel.DataSourceDispatcher;
				this.dataSourceDispatcher.IncRefCount();
			}
		}
		void InheritDataSourcesForMailMerge(DocumentModel documentModel) {
			SnapDocumentModel snapDocumentModel = (SnapDocumentModel)documentModel;
			fDataSources.Clear();
			foreach (var dsInfo in snapDocumentModel.DataSources)
				fDataSources.Add(dsInfo);
		}
		protected internal override PieceTable CreatePieceTable(ContentTypeBase contentType) {
			return new SnapPieceTable(this, contentType);
		}
		public override DocumentModel CreateNew() {
			return new SnapDocumentModel(this.dataSourceDispatcher.CreateNew(), DocumentFormatsDependencies);
		}
		public override DocumentModel CreateNew(bool modelForTextExport) {
			return new SnapDocumentModel(!modelForTextExport, this.dataSourceDispatcher.CreateNew(), DocumentFormatsDependencies);
		}
		public override DocumentModel CreateNew(bool addDefaultLists, bool changeDefaultTableStyle) {
			return new SnapDocumentModel(addDefaultLists, changeDefaultTableStyle, DocumentFormatsDependencies);
		}
		public override CopySectionOperation CreateCopySectionOperation(DocumentModelCopyManager copyManager) {
			return new SnapCopySectionOperation(copyManager);
		}
		protected override void OnMailMergeDataSourceChanged(object sender, EventArgs e) {
			rootDataContext = null;
			base.OnMailMergeDataSourceChanged(sender, e);
		}
		internal IFieldContext GetRootDataContext() {
			if (rootDataContext == null)
				SetMailMergeRootDataContext();
			if (rootDataContext == null)
				rootDataContext = new RootFieldContext(DataSourceDispatcher, DataSourceDispatcher.DefaultDataSourceName);
			return rootDataContext;
		}
		internal void SetRootDataContext(IFieldContext rootDataContext) {
			DisposeRootDataContext();
			this.rootDataContext = rootDataContext;
		}
		protected internal override void ToggleAllFieldCodes(bool showCodes) {
			BeginUpdate();
			try {
				SetSelectionAfterTopLevelField();
				SelectionInfo.CheckCurrentSnapBookmark(false, true);
				base.ToggleAllFieldCodes(showCodes);
			}
			finally {
				EndUpdate();
			}
		}
		protected void SetSelectionAfterTopLevelField() {
			((SnapPieceTable)Selection.PieceTable).SetSelectionAfterTopLevelField();
		}
		public override ExportHelper<DocumentFormat, bool> CreateDocumentExportHelper(DocumentFormat documentFormat) {
			UpdateDocument();
			if (documentFormat == SnapDocumentFormat.Snap)
				return base.CreateDocumentExportHelper(documentFormat);
			DocumentModel documentModel = CreateDocumentModelForExport(InitializeEmptyDocumentModel);
			return new DocumentExportHelper(documentModel);
		}
		protected internal override void CopyDocumentModelOptions(DocumentModel result) {
			base.CopyDocumentModelOptions(result);
			((SnapDocumentModel)result).SnapMailMergeVisualOptions.CopyFrom(SnapMailMergeVisualOptions);
		}
		protected internal virtual void InitializeEmptyDocumentModel(DocumentModel documentModel) {
		}
		protected internal override void UpdateTableOfContents() {
			BeginFieldsUpdate();
			UpdateTableOfContentsCore();
			EndFieldsUpdate();
		}
		void UpdateTableOfContentsCore() {
			BeginUpdate();
			try {
				List<PieceTable> pieceTables = GetPieceTables(false);
				int count = pieceTables.Count;
				for (int i = 0; i < count; i++)
					pieceTables[i].UpdateTableOfContents(UpdateFieldOperationType.Normal);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override void PreprocessContentBeforeInsert(PieceTable destination, DocumentLogPosition pos) {
			base.PreprocessContentBeforeInsert(destination, pos);
			List<PieceTable> pieceTables = GetPieceTables(false);
			int count = pieceTables.Count;
			for (int i = 0; i < count; i++) {
				SnapPieceTable source = (SnapPieceTable)pieceTables[i];
				InsertFieldController controller = new InsertFieldController(source, (SnapPieceTable)destination, pos);
				source.ProcessFieldsRecursive(null, controller.ValidateField);
			}
		}
		protected internal void AddParameterWithReplace(DevExpress.Snap.Core.API.Parameter item) {
			int count = Parameters.Count;
			for (int i = 0; i < count; i++) {
				if (Parameters[i].Name == item.Name) {
					Parameters.RemoveAt(i);
					break;
				}
			}
			Parameters.Add(item);
		}
		public ICollection<CalculatedField> GetCalculatedFieldsByDataSourceName(string dataSourceName, bool createDataSourceIfNotExists) {
			DataSourceInfo info = DataSourceDispatcher.GetInfo(dataSourceName);
			if (object.ReferenceEquals(info, null) && !createDataSourceIfNotExists)
				return null;
			ICollection<CalculatedField> result = info.CalculatedFields;
			if (result != null || !createDataSourceIfNotExists)
				return result;
			info = new DataSourceInfo(dataSourceName, null);
			DataSources.Add(info);
			return info.CalculatedFields;
		}
		public override void BeginSetContentForExport() {
			base.BeginSetContentForExport();
			SubstituteDataAccessService();
			AddService(typeof(IExportService), new SnapExportService());
		}
		public override DocumentModel GetFieldResultModel() {
			DocumentModel result = base.GetFieldResultModel();
			result.InheritService<IExportService>(this);
			return result;
		}
		void SubstituteDataAccessService() {
			IDataAccessService dataAccessService = GetService<IDataAccessService>();
			if (dataAccessService != null) {
				RemoveService(typeof(IDataAccessService));
				AddService(typeof(IDataAccessService), dataAccessService.GetDataAccessServiceForPrinting());
			}
		}
		protected internal override MailMergeDataMode GetMailMergeDataMode() {
			if (ModelForExport)
				return MailMergeDataMode.FinalMerging;
			return base.GetMailMergeDataMode();
		}
		protected internal override NumberingListCountersCalculator CreateNumberingListCountersCalculator(AbstractNumberingList list) {
			return new SnapNumberingListCountersCalculator(this, list);
		}
		public LastInsertedChartRunInfo GetLastInsertedChartRunInfo(SnapPieceTable pieceTable) {
			if (!Object.ReferenceEquals(pieceTable, lastInsertedChartRunInfo.PieceTable))
				lastInsertedChartRunInfo.Reset(pieceTable);
			return lastInsertedChartRunInfo;
		}
		public event BeforeInsertSnListEventHandler BeforeInsertSnList;
		public event PrepareSnListEventHandler PrepareSnList;
		public event AfterInsertSnListEventHandler AfterInsertSnList;
		public virtual void RaiseBeforeInsertSnList(BeforeInsertSnListEventArgs e) { if (BeforeInsertSnList != null) BeforeInsertSnList(this, e); }
		public virtual void RaisePrepareSnList(PrepareSnListEventArgs e) { if (PrepareSnList != null) PrepareSnList(this, e); }
		public virtual void RaiseAfterInsertSnList(AfterInsertSnListEventArgs e) { if (AfterInsertSnList != null) AfterInsertSnList(this, e); }
		public event BeforeInsertSnListColumnsEventHandler BeforeInsertSnListColumns;
		public event PrepareSnListColumnsEventHandler PrepareSnListColumns;
		public event AfterInsertSnListColumnsEventHandler AfterInsertSnListColumns;
		public virtual void RaiseBeforeInsertSnListColumns(BeforeInsertSnListColumnsEventArgs e) { if (BeforeInsertSnListColumns != null) BeforeInsertSnListColumns(this, e); }
		public virtual void RaisePrepareSnListColumns(PrepareSnListColumnsEventArgs e) { if (PrepareSnListColumns != null) PrepareSnListColumns(this, e); }
		public virtual void RaiseAfterInsertSnListColumns(AfterInsertSnListColumnsEventArgs e) { if (AfterInsertSnListColumns != null) AfterInsertSnListColumns(this, e); }
		public event BeforeInsertSnListDetailEventHandler BeforeInsertSnListDetail;
		public event PrepareSnListDetailEventHandler PrepareSnListDetail;
		public event AfterInsertSnListDetailEventHandler AfterInsertSnListDetail;
		public virtual void RaiseBeforeInsertSnListDetail(BeforeInsertSnListDetailEventArgs e) { if (BeforeInsertSnListDetail != null) BeforeInsertSnListDetail(this, e); }
		public virtual void RaisePrepareSnListDetail(PrepareSnListDetailEventArgs e) { if (PrepareSnListDetail != null) PrepareSnListDetail(this, e); }
		public virtual void RaiseAfterInsertSnListDetail(AfterInsertSnListDetailEventArgs e) { if (AfterInsertSnListDetail != null) AfterInsertSnListDetail(this, e); }
		public event BeforeInsertSnListRecordDataEventHandler BeforeInsertSnListRecordData;
		public event AfterInsertSnListRecordDataEventHandler AfterInsertSnListRecordData;
		internal virtual void RaiseBeforeInsertSnListRecordData(BeforeInsertSnListRecordDataEventArgs e) { if (BeforeInsertSnListRecordData != null) BeforeInsertSnListRecordData(this, e); }
		internal virtual void RaiseAfterInsertSnListRecordData(AfterInsertSnListRecordDataEventArgs e) { if (AfterInsertSnListRecordData != null) AfterInsertSnListRecordData(this, e); }
		internal DataConnectionParametersBase RaiseConfigureDataConnection(string connectionName, DataConnectionParametersBase parameters) {
			if (ConfigureDataConnection != null) {
				ConfigureDataConnectionEventArgs eventArgs = new ConfigureDataConnectionEventArgs(connectionName, parameters);
				ConfigureDataConnection(this, eventArgs);
				return eventArgs.ConnectionParameters;
			}
			return parameters;
		}
		internal DevExpress.Data.Filtering.CriteriaOperator RaiseCustomFilterExpression(CustomFilterExpressionEventArgs eventArgs) {
			if (CustomFilterExpression != null) {
				CustomFilterExpression(this, eventArgs);
				return eventArgs.FilterExpression;
			}
			return null;
		}
		internal DataConnectionParametersBase RaiseHandleConnectionError(ConnectionErrorEventArgs args) {
			if (ConnectionError != null)
				ConnectionError(this, args);
			if (args.Handled)
				return args.Cancel ? null : args.ConnectionParameters;
			return args.ConnectionParameters;
		}
		protected internal override void BeforeFloatingObjectDrop(DocumentLogPosition oldPosition, DocumentLogPosition newPosition, PieceTable pieceTable) {
			base.BeforeFloatingObjectDrop(oldPosition, newPosition, pieceTable);
			SnapPieceTable snapPieceTable = pieceTable as SnapPieceTable;
			if (snapPieceTable == null)
				return;
			SelectionInfo.CheckCurrentSnapBookmark(true, true, newPosition, newPosition, snapPieceTable);
		}
		protected override void DisposeCore() {
			base.DisposeCore();
			if (Themes != null) {
				Themes.ForEach(theme => theme.Dispose());
				this.themes = null;
			}
			DisposeRootDataContext();
			DisposeDataSourceDispatcher();
		}
		void DisposeRootDataContext() {
			IDisposable disposableContext = this.rootDataContext as IDisposable;
			if (disposableContext != null)
				disposableContext.Dispose();
		}
		void DisposeDataSourceDispatcher() {
			if (this.dataSourceDispatcher != null) {
				this.dataSourceDispatcher.Dispose();
				this.dataSourceDispatcher = null;
			}
		}
		public DataSourceInfo GetNotNullDocumentModelDataSourceInfo(object dataSource) {
			DataSourceInfo info = DataSourceDispatcher.GetInfo(dataSource);
			return GetNotNullDocumentModelDataSourceInfo(info.DataSourceName);
		}
		public DataSourceInfo GetNotNullDocumentModelDataSourceInfo(string name) {
			DataSourceInfo sourceInfo = DataSources[name];
			if (sourceInfo == null)
				sourceInfo = DataSources.Add(name, null);
			return sourceInfo;
		}
		public DataSourceInfo RegisterDataSource(object dataSource) {
			BeginUpdateDataSource();
			try {
				string name = string.Format("{0} {1}", DefaultDataSourceName, dataSourceNameIndex);
				while (!object.ReferenceEquals(DataSourceDispatcher.GetDataSource(name), null)) {
					dataSourceNameIndex++;
					name = string.Format("{0} {1}", DefaultDataSourceName, dataSourceNameIndex);
				}
				DataSourceInfo info = new DataSourceInfo(name, dataSource);
				DataSources.Add(info);
				return info;
			}
			finally {
				EndUpdateDataSource();
			}
		}
		protected internal override DocumentModelCopyOptions CreateDocumentModelCopyOptions(DocumentLogPosition from, int length) {
			return new SnapDocumentModelCopyOptions(from, length);
		}
		protected internal override void SetModelForExportCopyOptions(DocumentModelCopyOptions copyOptions) {
			base.SetModelForExportCopyOptions(copyOptions);
			((SnapDocumentModelCopyOptions)copyOptions).CopySnapBookmark = false;
		}
		internal bool HasNonEmptyDataSource() {
			for (int i = 0; i < DataSources.Count; i++)
				if (!DataSources[i].IsEmpty)
					return true;
			return false;
		}
		internal string GetNewDataSourceName(string oldName) {
			string name;
			int num = 1;
			do {
				name = oldName + "_" + num;
				num++;
			} while (ShouldChangeDataSourceName(name));
			return name;
		}
		internal bool ShouldChangeDataSourceName(string dataSourceName) {
			if (string.IsNullOrEmpty(dataSourceName))
				return false;
			for (int i = 0; i < DataSources.Count; i++)
				if (DataSources[i].DataSourceName == dataSourceName)
					return true;
			return false;
		}
	}
	public delegate void BeforeInsertSnListEventHandler(object sender, BeforeInsertSnListEventArgs e);
	public delegate void PrepareSnListEventHandler(object sender, PrepareSnListEventArgs e);
	public delegate void AfterInsertSnListEventHandler(object sender, AfterInsertSnListEventArgs e);
	public delegate void BeforeInsertSnListColumnsEventHandler(object sender, BeforeInsertSnListColumnsEventArgs e);
	public delegate void PrepareSnListColumnsEventHandler(object sender, PrepareSnListColumnsEventArgs e);
	public delegate void AfterInsertSnListColumnsEventHandler(object sender, AfterInsertSnListColumnsEventArgs e);
	public delegate void BeforeInsertSnListDetailEventHandler(object sender, BeforeInsertSnListDetailEventArgs e);
	public delegate void PrepareSnListDetailEventHandler(object sender, PrepareSnListDetailEventArgs e);
	public delegate void AfterInsertSnListDetailEventHandler(object sender, AfterInsertSnListDetailEventArgs e);
	public delegate void BeforeInsertSnListRecordDataEventHandler(object sender, BeforeInsertSnListRecordDataEventArgs e);
	public delegate void AfterInsertSnListRecordDataEventHandler(object sender, AfterInsertSnListRecordDataEventArgs e);
	public class BeforeInsertSnListEventArgs : EventArgs {
		DocumentLogPosition position;
		SNDataInfo[] dataFields;
		bool induceRelation = true;
		public BeforeInsertSnListEventArgs(DocumentLogPosition position, SNDataInfo[] dataFields) {
			this.position = position;
			this.dataFields = dataFields;
		}
		public DocumentLogPosition Position { get { return position; } set { position = value; } }
		public SNDataInfo[] DataFields { get { return dataFields; } set { dataFields = value; } }
		public bool InduceRelation { get { return induceRelation; } set { induceRelation = value; } }
	}
	public class PrepareSnListEventArgs : EventArgs {
		DocumentModel template;
		public PrepareSnListEventArgs(DocumentModel template) {
			this.template = template;
		}
		public DocumentModel Template { get { return template; } }
	}
	public class AfterInsertSnListEventArgs : EventArgs {
		DocumentLogPosition start;
		int length;
		public AfterInsertSnListEventArgs(DocumentLogPosition start, int length) {
			this.start = start;
			this.length = length;
		}
		public DocumentLogPosition Start { get { return start; } }
		public int Length { get { return length; } }
	}
	public class BeforeInsertSnListColumnsEventArgs : EventArgs {
		Field targetField;
		int targetColumnIndex;	  
		SNDataInfo[] dataFields;
		public BeforeInsertSnListColumnsEventArgs(Field field, int targetColumnIndex, SNDataInfo[] dataFields) {
			this.targetField = field;
			this.targetColumnIndex = targetColumnIndex;
			this.dataFields = dataFields;
		}
		public Field TargetField { get { return targetField; } }
		public int TargetColumnIndex { get { return targetColumnIndex; } set { targetColumnIndex = value; } }
		public SNDataInfo[] DataFields { get { return dataFields; } set { dataFields = value; } }
	}
	public class BeforeInsertSnListDetailEventArgs : EventArgs {
		readonly Field master;
		SNDataInfo[] dataFields;
		public BeforeInsertSnListDetailEventArgs(Field master, SNDataInfo[] dataFields) {
			this.master = master;
			this.dataFields = dataFields;
		}
		public Field Master { get { return master; } }
		public SNDataInfo[] DataFields { get { return dataFields; } set { dataFields = value; } }
	}
	public class PrepareSnListDetailEventArgs : EventArgs {
		DocumentModel template;
		public PrepareSnListDetailEventArgs(DocumentModel template) {
			this.template = template;
		}
		public DocumentModel Template { get { return template; } }
	}
	public class AfterInsertSnListDetailEventArgs : EventArgs {
		Field master;
		public AfterInsertSnListDetailEventArgs(Field master) {
			this.master = master;
		}
		public Field Master { get { return master; } }
	}
	public class PrepareSnListColumnsEventArgs : EventArgs {
		DocumentModel header;
		DocumentModel body;
		public PrepareSnListColumnsEventArgs(DocumentModel header, DocumentModel body) {
			this.header = header;
			this.body = body;
		}
		public DocumentModel Header { get { return header; } }
		public DocumentModel Body { get { return body; } }
	}
	public class AfterInsertSnListColumnsEventArgs : EventArgs {
		Field snList;
		public AfterInsertSnListColumnsEventArgs(Field snList) {
			this.snList = snList;
		}
		public Field SnList { get { return snList; } }
	}
	public class BeforeInsertSnListRecordDataEventArgs : EventArgs {
		readonly Field targetField;
		int targetColumnIndex;	  
		SNDataInfo[] dataFields;
		public BeforeInsertSnListRecordDataEventArgs(Field field, int targetColumnIndex, SNDataInfo[] dataFields) {
			this.targetField = field;
			this.targetColumnIndex = targetColumnIndex;
			this.dataFields = dataFields;
		}
		public Field TargetField { get { return targetField; } }
		public int TargetColumnIndex { get { return targetColumnIndex; } }
		public SNDataInfo[] DataFields { get { return dataFields; } set { dataFields = value; } }
	}
	public class AfterInsertSnListRecordDataEventArgs : EventArgs {
		DocumentLogPosition start;
		int length;
		public AfterInsertSnListRecordDataEventArgs(DocumentLogPosition start, int length) {
			this.start = start;
			this.length = length;
		}
		public DocumentLogPosition Start { get { return start; } }
		public int Length { get { return length; } }
	}
	public static class MailMergeParamsConverter {
		public static List<GroupProperties> ConvertToGroupPropertiesList(SnapListSorting sorting) {
			List<GroupProperties> result = new List<GroupProperties>();
			for (int i = 0; i < sorting.Count; i++) {
				GroupProperties properties = new GroupProperties();
				SnapListGroupParam param = sorting[i];
				GroupFieldInfo info = properties.AddField(param.FieldName);
				info.SortOrder = param.SortOrder;
				info.GroupInterval = param.Interval;
				result.Add(properties);
			}
			return result;
		}
		public static FilterProperties ConvertToFilterProperties(string filterString) {
			FilterProperties result = new FilterProperties();
			if (!string.IsNullOrEmpty(filterString))
				result.AddFilter(filterString);
			return result;
		}
	}
}

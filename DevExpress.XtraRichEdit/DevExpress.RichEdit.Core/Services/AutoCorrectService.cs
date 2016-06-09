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
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office.Utils;
using DevExpress.Office.Services.Implementation;
using System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Services {
	#region IAutoCorrectService
	[ComVisible(true)]
	public interface IAutoCorrectService {
		AutoCorrectInfo CalculateAutoCorrectInfo();
		void ApplyAutoCorrectInfo(AutoCorrectInfo info);
		void RegisterProvider(IAutoCorrectProvider provider);
		void UnregisterProvider(IAutoCorrectProvider provider);
		void SetReplaceTable(AutoCorrectReplaceInfoCollection replaceTable);
	}
	#endregion
	#region IAutoCorrectProvider
	[ComVisible(true)]
	public interface IAutoCorrectProvider {
		AutoCorrectInfo CalculateAutoCorrectInfo();
	}
	#endregion
}
namespace DevExpress.XtraRichEdit {
	#region AutoCorrectReplaceInfoCollection
	[ComVisible(true)]
	public class AutoCorrectReplaceInfoCollection {
		readonly List<AutoCorrectReplaceInfo> greedyReplaceTable = new List<AutoCorrectReplaceInfo>();
		readonly List<AutoCorrectReplaceInfo> replaceTable = new List<AutoCorrectReplaceInfo>();
		public bool CaseSensitive { get; set; }
		public void Add(string what, string with) {
			Add(new AutoCorrectReplaceInfo(what, with));
		}
		public void Add(string what, string with, bool immediateReplacement) {
			Add(new AutoCorrectReplaceInfo(what, with, immediateReplacement));
		}
		public void Add(AutoCorrectReplaceInfo info) {
			string what = info.What;
			if (String.IsNullOrEmpty(what))
				return;
			if (IsSeparator(what[what.Length - 1]) || info.ImmediateReplacement)
				greedyReplaceTable.Add(info);
			else
				replaceTable.Add(info);
		}
		protected internal void Sort() {
			AutoCorrectReplaceInfoComparer comparer = new AutoCorrectReplaceInfoComparer();
			replaceTable.Sort(comparer);
			greedyReplaceTable.Sort(comparer);
		}
		protected internal static bool IsSeparator(char ch) {
			return Char.IsPunctuation(ch) || Char.IsSeparator(ch) || Char.IsWhiteSpace(ch);
		}
		protected internal IList<AutoCorrectReplaceInfo> GreedyReplaceTable { get { return greedyReplaceTable; } }
		protected internal IList<AutoCorrectReplaceInfo> ReplaceTable { get { return replaceTable; } }
	}
	#endregion
	#region AutoCorrectReplaceInfo
	[ComVisible(true)]
	public class AutoCorrectReplaceInfo {
		#region Fields
		string what;
		string reversedWhat;
		object with;
		bool immediateReplacement;
		#endregion
		public AutoCorrectReplaceInfo() {
			this.what = String.Empty;
			this.reversedWhat = String.Empty;
		}
		public AutoCorrectReplaceInfo(string what, object with)
			: this(what, with, false) {
		}
		public AutoCorrectReplaceInfo(string what, object with, bool immediateReplacement) {
			this.what = what;
			this.with = with;
			this.immediateReplacement = immediateReplacement;
			this.reversedWhat = Reverse(what);
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("AutoCorrectReplaceInfoWhat")]
#endif
		public string What { get { return what; } set { what = value; this.reversedWhat = Reverse(what); } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("AutoCorrectReplaceInfoWith")]
#endif
		public object With { get { return with; } set { with = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("AutoCorrectReplaceInfoImmediateReplacement")]
#endif
		public bool ImmediateReplacement { get { return immediateReplacement; } set { immediateReplacement = value; } }
		protected internal string ReversedWhat { get { return reversedWhat; } }
		#endregion
		public string ReverseString(string value) {
			return AutoCorrectReplaceInfo.Reverse(value);
		}
		[ComVisible(false)]
		public static string Reverse(string value) {
			if (String.IsNullOrEmpty(value))
				return String.Empty;
			StringBuilder result = new StringBuilder(value.Length);
			for (int i = value.Length - 1; i >= 0; i--)
				result.Append(value[i]);
			return result.ToString();
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Services.Implementation {
	#region AutoCorrectProviderCollection
	public class AutoCorrectProviderCollection : List<IAutoCorrectProvider> {
	}
	#endregion
	public interface ITableBasedAutoCorrectProvider {
		AutoCorrectReplaceInfoCollection ReplaceTable { get; set; }
	}
	#region AutoCorrectService
	public class AutoCorrectService : IAutoCorrectService {
		#region Fields
		readonly InnerRichEditDocumentServer documentServer;
		readonly AutoCorrectProviderCollection providers;
		#endregion
		public AutoCorrectService(InnerRichEditDocumentServer documentServer) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			this.documentServer = documentServer;
			this.providers = new AutoCorrectProviderCollection();
			RegisterDefaultProviders();
		}
		#region Properties
		public InnerRichEditDocumentServer DocumentServer { get { return documentServer; } }
		public DocumentModel DocumentModel { get { return documentServer.DocumentModel; } }
		public AutoCorrectProviderCollection Providers { get { return providers; } }
		#endregion
		protected internal virtual void RegisterDefaultProviders() {
			AutoCorrectReplaceInfoCollection replaceTable = new AutoCorrectReplaceInfoCollection();
			InnerRegisterProvider(new TableBasedGreedyAutoCorrectProvider(DocumentServer, replaceTable));
			InnerRegisterProvider(new TableBasedAutoCorrectProvider(DocumentServer, replaceTable));
			InnerRegisterProvider(new UrlAutoCorrectProvider(DocumentServer));
			InnerRegisterProvider(new TwoInitialCapitalsAutoCorrectProvider(DocumentServer));
			InnerRegisterProvider(new SpellCheckerAutoCorrectProvider(DocumentServer));
			InnerRegisterProvider(new EventAutoCorrectProvider(DocumentServer));
		}
		#region IAutoCorrectService Members
		public AutoCorrectInfo CalculateAutoCorrectInfo() {
			int count = Providers.Count;
			for (int i = 0; i < count; i++) {
				try {
					AutoCorrectInfo info = Providers[i].CalculateAutoCorrectInfo();
					if (info != null)
						return info;
				}
				catch {
				}
			}
			return null;
		}
		public void ApplyAutoCorrectInfo(AutoCorrectInfo info) {
			if (info == null)
				return;
			HyperlinkInfo hyperlinkInfo = info.ReplaceWith as HyperlinkInfo;
			if (hyperlinkInfo != null) {
				ReplaceWithHyperlink(info, hyperlinkInfo);
				return;
			}
			string replaceString = info.ReplaceWith as String;
			if (replaceString != null) {
				ReplaceWithText(info, replaceString);
				return;
			}
			OfficeImage richEditImage = info.ReplaceWith as OfficeImage;
			if (richEditImage != null) {
				ReplaceWithPicture(info, richEditImage.Clone(DocumentModel));
				return;
			}
#if !SL
			Image image = info.ReplaceWith as Image;
			if (image != null) {
				ReplaceWithPicture(info, info.DocumentServer.DocumentModel.CreateImage(image));
				return;
			}
#endif
		}
		protected internal void ReplaceWithText(AutoCorrectInfo info, string replaceString) {
			int length = info.End.LogPosition - info.Start.LogPosition;
			if (length <= 0)
				return;
			IThreadSyncService service = DocumentModel.GetService<IThreadSyncService>();
			if (service == null)
				return;
			Action action = delegate() {
				ReplaceWithTextCore(info, replaceString, length);
			};
			service.EnqueueInvokeInUIThread(action);
		}
		protected internal void ReplaceWithTextCore(AutoCorrectInfo info, string replaceString, int length) {
			info.DocumentServer.BeginUpdate();
			try {
				info.PieceTable.ReplaceTextWithMultilineText(info.Start.LogPosition, length, replaceString);
				UpdateCaretPositionX(info);
			}
			finally {
				info.DocumentServer.EndUpdate();
			}
		}
		protected internal void UpdateCaretPositionX(AutoCorrectInfo info) {
			InnerRichEditControl control = info.DocumentServer as InnerRichEditControl;
			if (control != null && !control.IsDisposed) {
				CaretPosition caretPosition = control.ActiveView.CaretPosition;
				caretPosition.Update(DocumentLayoutDetailsLevel.Character);
				if (caretPosition.LayoutPosition.Character != null)
					caretPosition.X = caretPosition.CalculateCaretBounds().X;
			}
		}
		protected internal void ReplaceWithHyperlink(AutoCorrectInfo info, HyperlinkInfo hyperlinkInfo) {
			int length = info.End.LogPosition - info.Start.LogPosition;
			if (length <= 0)
				return;
			IThreadSyncService service = DocumentModel.GetService<IThreadSyncService>();
			if (service == null)
				return;
			Action action = delegate() {
				ReplaceWithHyperlinkCore(info, hyperlinkInfo, length);
			};
			service.EnqueueInvokeInUIThread(action);
		}
		protected internal void ReplaceWithHyperlinkCore(AutoCorrectInfo info, HyperlinkInfo hyperlinkInfo, int length) {
			info.DocumentServer.BeginUpdate();
			try {
				info.PieceTable.CreateHyperlink(info.Start.LogPosition, length, hyperlinkInfo);
				UpdateCaretPositionX(info);
			}
			finally {
				info.DocumentServer.EndUpdate();
			}
		}
		protected internal void ReplaceWithPicture(AutoCorrectInfo info, OfficeImage image) {
			int length = info.End.LogPosition - info.Start.LogPosition;
			if (length <= 0)
				return;
			IThreadSyncService service = DocumentModel.GetService<IThreadSyncService>();
			if (service == null)
				return;
			Action action = delegate() {
				ReplaceWithPictureCore(info, image, length);
			};
			service.EnqueueInvokeInUIThread(action);
		}
		protected internal void ReplaceWithPictureCore(AutoCorrectInfo info, OfficeImage image, int length) {
			info.DocumentServer.BeginUpdate();
			try {
				info.PieceTable.ReplaceTextWithPicture(info.Start.LogPosition, length, image);
				UpdateCaretPositionX(info);
			}
			finally {
				info.DocumentServer.EndUpdate();
			}
		}
		public void RegisterProvider(IAutoCorrectProvider provider) {
			if (provider == null)
				return;
			Providers.Insert(0, provider);
		}
		public void InnerRegisterProvider(IAutoCorrectProvider provider) {
			if (provider == null)
				return;
			Providers.Add(provider);
		}
		public void UnregisterProvider(IAutoCorrectProvider provider) {
			if (provider == null)
				return;
			int index = Providers.IndexOf(provider);
			if (index >= 0)
				Providers.RemoveAt(index);
		}
		public void SetReplaceTable(AutoCorrectReplaceInfoCollection replaceTable) {
			if (replaceTable == null)
				replaceTable = new AutoCorrectReplaceInfoCollection();
			replaceTable.Sort();
			int count = Providers.Count;
			for (int i = 0; i < count; i++) {
				ITableBasedAutoCorrectProvider provider = Providers[i] as ITableBasedAutoCorrectProvider;
				if (provider != null)
					provider.ReplaceTable = replaceTable;
			}
		}
		#endregion
	}
	#endregion
	#region TableBasedGreedyAutoCorrectProvider (abstract class)
	public class TableBasedGreedyAutoCorrectProvider : IAutoCorrectProvider, ITableBasedAutoCorrectProvider {
		readonly InnerRichEditDocumentServer documentServer;
		readonly Dictionary<char, bool> triggerChars = new Dictionary<char,bool>();
		AutoCorrectReplaceInfoCollection replaceTable;
		int maxEntryLength = 0;
		public TableBasedGreedyAutoCorrectProvider(InnerRichEditDocumentServer documentServer, AutoCorrectReplaceInfoCollection replaceTable) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			Guard.ArgumentNotNull(replaceTable, "replaceTable");
			this.documentServer = documentServer;
			this.ReplaceTable = replaceTable;
		}
		public AutoCorrectReplaceInfoCollection ReplaceTable {
			get { return replaceTable; }
			set {
				replaceTable = value;
				maxEntryLength = 0;
				int count = replaceTable.GreedyReplaceTable.Count;
				for (int i = 0; i < count; i++) {
					string what = replaceTable.GreedyReplaceTable[i].What;
					if (!String.IsNullOrEmpty(what)) {
						char trigger = what[what.Length - 1];
						if (!IsSeparator(trigger))
							triggerChars[trigger] = true;
					}
					maxEntryLength = Math.Max(maxEntryLength, what.Length);
				}
			}
		}
		public InnerRichEditDocumentServer DocumentServer { get { return documentServer; } }
		public DocumentModel DocumentModel { get { return DocumentServer.DocumentModel; } }
		#region IAutoCorrectProvider Members
		public virtual AutoCorrectInfo CalculateAutoCorrectInfo() {
			if (!DocumentModel.AutoCorrectOptions.ReplaceTextAsYouType)
				return null;
			if (ReplaceTable.GreedyReplaceTable.Count <= 0)
				return null;
			AutoCorrectInfo info = new AutoCorrectInfo(DocumentServer);
			string text = info.Text;
			if (String.IsNullOrEmpty(text) || !IsTriggerChar(text[0]))
				return null;
			if (!BeforeFirstGetText(info))
				return null;
			text = info.Text;
			if (!IsValidInitialText(text))
				return null;
			for (; ; ) {
				int index = Algorithms.BinarySearch(ReplaceTable.GreedyReplaceTable, new AutoCorrectReplaceInfoSuffixComparable(text));
				if (index >= 0) {
					index = FindFinalReplaceTableIndex(text, index);
					if (index >= 0) {
						if (!info.CanDecrementStart || IsSeparator(text[0])) {
							info.ReplaceWith = ReplaceTable.GreedyReplaceTable[index].With;
							return info;
						}
						else {
							if (!info.DecrementStartPosition() || IsSeparator(info.Text[0])) {
								info.IncrementStartPosition();
								info.ReplaceWith = ReplaceTable.GreedyReplaceTable[index].With;
								return info;
							}
							info.IncrementStartPosition();
						}
					}
					if (!info.DecrementStartPosition())
						return null;
					text = info.Text;
					if (text.Length > maxEntryLength)
						return null;
				}
				else
					return null;
			}
		}
		protected internal virtual bool IsValidInitialText(string text) {
			return !String.IsNullOrEmpty(text);
		}
		protected internal virtual bool IsSeparator(char ch) {
			return AutoCorrectReplaceInfoCollection.IsSeparator(ch);
		}
		protected internal virtual bool IsTriggerChar(char ch) {
			return IsSeparator(ch) || triggerChars.ContainsKey(ch);
		}
		protected internal virtual int FindFinalReplaceTableIndex(string text, int initialIndex) {
			AutoCorrectReplaceInfoSuffixComparable comparable = new AutoCorrectReplaceInfoSuffixComparable(text);
			int result = FindFinalReplaceTableIndexCore(text, comparable, initialIndex, -1, -1);
			if (result >= 0)
				return result;
			return FindFinalReplaceTableIndexCore(text, comparable, initialIndex + 1, ReplaceTable.GreedyReplaceTable.Count, 1);
		}
		protected internal virtual int FindFinalReplaceTableIndexCore(string text, AutoCorrectReplaceInfoSuffixComparable comparable, int from, int to, int step) {
			for (int i = from; i != to; i += step) {
				if (comparable.CompareTo(ReplaceTable.GreedyReplaceTable[i]) != 0)
					return -1;
				if (String.Compare(ReplaceTable.GreedyReplaceTable[i].What, text, StringComparison.OrdinalIgnoreCase) == 0)
					return i;
			}
			return -1;
		}
		#endregion
		protected internal virtual bool BeforeFirstGetText(AutoCorrectInfo info) {
			return true;
		}
	}
	#endregion
	#region ConditionalWordReplaceAutoCorrectProvider (abstract class)
	public abstract class ConditionalWordReplaceAutoCorrectProvider : IAutoCorrectProvider {
		readonly InnerRichEditDocumentServer documentServer;
		protected ConditionalWordReplaceAutoCorrectProvider(InnerRichEditDocumentServer documentServer) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			this.documentServer = documentServer;
		}
		public InnerRichEditDocumentServer DocumentServer { get { return documentServer; } }
		public DocumentModel DocumentModel { get { return documentServer.DocumentModel; } }
		#region IAutoCorrectProvider Members
		public virtual AutoCorrectInfo CalculateAutoCorrectInfo() {
			AutoCorrectInfo info = new AutoCorrectInfo(DocumentServer);
			if (!IsAutoCorrectAllowed(info))
				return null;
			string text = info.Text;
			if (String.IsNullOrEmpty(text) || !IsTriggerChar(text[0]))
				return null;
			if (!BeforeFirstGetText(info))
				return null;
			text = info.Text;
			if (!IsValidInitialText(text))
				return null;
			for (; ; ) {
				if (!info.DecrementStartPosition())
					return null;
				text = info.Text;
				bool isSeparator = IsSeparator(text[0]);
				if (!info.CanDecrementStart || isSeparator) {
					if (isSeparator)
						info.IncrementStartPosition();
					text = info.Text;
					object with = CalculateWordReplacement(text);
					if (with != null) {
						info.ReplaceWith = with;
						return info;
					}
					else
						return null;
				}
			}
		}
		#endregion
		protected internal virtual bool IsValidInitialText(string text) {
			return !String.IsNullOrEmpty(text) && !IsTriggerChar(text[0]);
		}
		protected internal virtual bool BeforeFirstGetText(AutoCorrectInfo info) {
			info.DecrementEndPosition();
			return info.DecrementStartPosition();
		}
		protected internal virtual bool IsAutoCorrectAllowed(AutoCorrectInfo info) {
			return true;
		}
		protected internal virtual bool IsSeparator(char ch) {
			return Char.IsPunctuation(ch) || Char.IsSeparator(ch) || Char.IsWhiteSpace(ch) || ch == '\r' || ch == '\n' || ch == Characters.NonBreakingSpace || ch == Characters.PageBreak || ch == Characters.ColumnBreak;
		}
		protected internal virtual bool IsTriggerChar(char ch) {
			return Char.IsWhiteSpace(ch) || ch == '\r' || ch == '\n' || ch == Characters.NonBreakingSpace || ch == Characters.PageBreak || ch == Characters.ColumnBreak;
		}
		protected internal abstract object CalculateWordReplacement(string word);
	}
	#endregion
	#region TableBasedAutoCorrectProvider
	public class TableBasedAutoCorrectProvider : ConditionalWordReplaceAutoCorrectProvider, ITableBasedAutoCorrectProvider {
		AutoCorrectReplaceInfoCollection replaceInfoCollection;
		IList<AutoCorrectReplaceInfo> table;
		public TableBasedAutoCorrectProvider(InnerRichEditDocumentServer documentServer, AutoCorrectReplaceInfoCollection replaceInfoCollection)
			: base(documentServer) {
			Guard.ArgumentNotNull(replaceInfoCollection, "replaceInfoCollection");
			this.ReplaceTable = replaceInfoCollection;
		}
		#region Properties
		public AutoCorrectReplaceInfoCollection ReplaceTable {
			get { return replaceInfoCollection; }
			set {
				replaceInfoCollection = value;
				table = replaceInfoCollection.ReplaceTable;
			}
		}
		#endregion
		protected internal IList<AutoCorrectReplaceInfo> Table { get { return table; } }
		public override AutoCorrectInfo CalculateAutoCorrectInfo() {
			if (!DocumentModel.AutoCorrectOptions.ReplaceTextAsYouType)
				return null;
			if (Table.Count <= 0)
				return null;
			return base.CalculateAutoCorrectInfo();
		}
		protected internal override bool IsTriggerChar(char ch) {
			return Char.IsWhiteSpace(ch) || Char.IsPunctuation(ch) || Char.IsSeparator(ch) || ch == '\r' || ch == '\n' || ch == Characters.NonBreakingSpace || ch == Characters.PageBreak || ch == Characters.ColumnBreak;
		}
		protected internal override object CalculateWordReplacement(string word) {
			int index = Algorithms.BinarySearch(Table, new AutoCorrectReplaceInfoExactSuffixComparable(word, !ReplaceTable.CaseSensitive));
			if (index >= 0)
				return Table[index].With;
			else
				return null;
		}
	}
	#endregion
	#region TwoInitialCapitalsAutoCorrectProvider
	public class TwoInitialCapitalsAutoCorrectProvider : ConditionalWordReplaceAutoCorrectProvider {
		public TwoInitialCapitalsAutoCorrectProvider(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
		}
		protected internal override object CalculateWordReplacement(string word) {
			if (word.Length < 3)
				return null;
			if (!Char.IsUpper(word[0]) || !Char.IsUpper(word[1]))
				return null;
			int count = word.Length;
			for (int i = 2; i < count; i++) {
				if (!Char.IsLower(word[i]))
					return null;
			}
			string lowerChar = new String(Char.ToLowerInvariant(word[1]), 1);
			word = word.Remove(1, 1);
			word = word.Insert(1, lowerChar);
			return word;
		}
		public override AutoCorrectInfo CalculateAutoCorrectInfo() {
			if (!DocumentModel.AutoCorrectOptions.CorrectTwoInitialCapitals)
				return null;
			return base.CalculateAutoCorrectInfo();
		}
	}
	#endregion
	#region SpellCheckerAutoCorrectProvider
	public class SpellCheckerAutoCorrectProvider : ConditionalWordReplaceAutoCorrectProvider {
		readonly InnerRichEditControl innerControl;
		public SpellCheckerAutoCorrectProvider(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
			this.innerControl = DocumentServer as InnerRichEditControl;
		}
		protected internal override object CalculateWordReplacement(string word) {
			ISpellChecker spellChecker = innerControl.SpellChecker;
			ICheckSpellingResult result = spellChecker.CheckText(innerControl.Owner, word, 0, spellChecker.Culture);
			if (!result.HasError || result.Result == CheckSpellingResultType.Repeating)
				return null;
			string[] suggestions = spellChecker.GetSuggestions(word, spellChecker.Culture);
			if (suggestions != null && suggestions.Length == 1)
				return suggestions[0];
			else
				return null;
		}
		public override AutoCorrectInfo CalculateAutoCorrectInfo() {
			if (!DocumentModel.AutoCorrectOptions.UseSpellCheckerSuggestions)
				return null;
			if (innerControl == null || innerControl.SpellChecker == null)
				return null;
			return base.CalculateAutoCorrectInfo();
		}
	}
	#endregion
	#region UrlAutoCorrectProvider
	public class UrlAutoCorrectProvider : ConditionalWordReplaceAutoCorrectProvider {
		public UrlAutoCorrectProvider(InnerRichEditDocumentServer documentServer)
			: base(documentServer) {
		}
		protected internal override bool IsSeparator(char ch) {
			return IsTriggerChar(ch);
		}
		protected internal override object CalculateWordReplacement(string word) {
			if (!IsValidUrl(word))
				return null;
			HyperlinkInfo hyperlinkInfo = new HyperlinkInfo();
			hyperlinkInfo.NavigateUri = CreateNavigateUri(word);
			return hyperlinkInfo;
		}
		string CreateNavigateUri(string word) {
			if (!word.Contains(":") && word.Contains("@"))
				return "mailto:" + word;
			else
				return word;
		}
		static readonly Regex urlRegex = new Regex(@"(?:[a-z][\w-]+:(?:/{1,3}([^./]*:[^./]*@){0,1})|www\d{0,3}[.]|ftp[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\([^\s<>]*\))+(?:\([^\s<>]*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’])", RegexOptions.IgnoreCase);
		static readonly Regex emailRegex = new Regex(@"(mailto:)?[-\w!#$%&'*+/=?^_`{|}~]+(?:\.[-\w!#$%&'*+/=?^_`{|}~]+)*@(?:\w+([-\w]*\w)?\.)*[\w]+", RegexOptions.IgnoreCase);
		internal bool IsValidUrl(string text) {
			Match match = emailRegex.Match(text);
			if (match.Index == 0 && match.Length == text.Length)
				return true;
			match = urlRegex.Match(text);
			if (match.Index == 0 && match.Length == text.Length)
				return true;
			return false;
		}
		protected internal override bool IsAutoCorrectAllowed(AutoCorrectInfo info) {
			Field field = DocumentServer.DocumentModel.Selection.PieceTable.FindFieldByRunIndex(info.End.RunIndex);
			return field == null; 
		}
		public override AutoCorrectInfo CalculateAutoCorrectInfo() {
			if (!DocumentModel.AutoCorrectOptions.DetectUrls)
				return null;
			return base.CalculateAutoCorrectInfo();
		}
	}
	#endregion
	#region EventAutoCorrectProvider
	public class EventAutoCorrectProvider : IAutoCorrectProvider {
		readonly InnerRichEditDocumentServer documentServer;
		public EventAutoCorrectProvider(InnerRichEditDocumentServer documentServer) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			this.documentServer = documentServer;
		}
		public InnerRichEditDocumentServer DocumentServer { get { return documentServer; } }
		#region IAutoCorrectProvider Members
		public AutoCorrectInfo CalculateAutoCorrectInfo() {
			return DocumentServer.RaiseAutoCorrect();
		}
		#endregion
	}
	#endregion
	#region AutoCorrectReplaceInfoComparer
	public class AutoCorrectReplaceInfoComparer : IComparer<AutoCorrectReplaceInfo> {
		#region IComparer<AutoCorrectReplaceInfo> Members
		public int Compare(AutoCorrectReplaceInfo x, AutoCorrectReplaceInfo y) {
			return StringComparer.OrdinalIgnoreCase.Compare(x.ReversedWhat, y.ReversedWhat);
		}
		#endregion
	}
	#endregion
	#region AutoCorrectReplaceInfoSuffixComparable
	public class AutoCorrectReplaceInfoSuffixComparable : IComparable<AutoCorrectReplaceInfo> {
		string suffix;
		public AutoCorrectReplaceInfoSuffixComparable(string suffix) {
			this.suffix = AutoCorrectReplaceInfo.Reverse(suffix);
		}
		#region IComparable<AutoCorrectReplaceInfo> Members
		public int CompareTo(AutoCorrectReplaceInfo other) {
			if (other.ReversedWhat.StartsWith(suffix, StringComparison.OrdinalIgnoreCase))
				return 0;
			return -StringComparer.OrdinalIgnoreCase.Compare(suffix, other.ReversedWhat);
		}
		#endregion
	}
	#endregion
	#region AutoCorrectReplaceInfoExactSuffixComparable
	public class AutoCorrectReplaceInfoExactSuffixComparable : IComparable<AutoCorrectReplaceInfo> {
		string suffix;
		StringComparer comparer;
		public AutoCorrectReplaceInfoExactSuffixComparable(string suffix, bool ignoreCase) {
			this.suffix = AutoCorrectReplaceInfo.Reverse(suffix);
			this.comparer = GetComparer(ignoreCase);
		}
		StringComparer GetComparer(bool ignoreCase) {
			return ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
		}
		#region IComparable<AutoCorrectReplaceInfo> Members
		public int CompareTo(AutoCorrectReplaceInfo other) {
			return -this.comparer.Compare(suffix, other.ReversedWhat);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraRichEdit {
	#region AutoCorrectInfo
	[ComVisible(true)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1409")]
	public class AutoCorrectInfo {
		#region Fields
		object replaceWith = String.Empty;
		DocumentModelPosition start;
		DocumentModelPosition end;
		string text = String.Empty;
		readonly InnerRichEditDocumentServer documentServer;
		readonly VisibleCharactersStopAtFieldsDocumentModelIterator iterator;
		#endregion
		public AutoCorrectInfo(IRichEditControl control)
			: this(GetInnerServer(control)) {
		}
		protected internal AutoCorrectInfo(InnerRichEditDocumentServer documentServer) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			this.documentServer = documentServer;
			this.end = documentServer.DocumentModel.Selection.Interval.NormalizedEnd;
			this.start = end.Clone();
			this.iterator = new VisibleCharactersStopAtFieldsDocumentModelIterator(PieceTable);
			DecrementStartPosition();
		}
		static InnerRichEditDocumentServer GetInnerServer(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			return control.InnerDocumentServer;
		}
		#region Properties
		internal DocumentModelPosition Start { get { return start; } }
		internal DocumentModelPosition End { get { return end; } }
		internal PieceTable PieceTable { get { return end.PieceTable; } }
		internal InnerRichEditDocumentServer DocumentServer { get { return documentServer; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("AutoCorrectInfoCanDecrementStart")]
#endif
		public bool CanDecrementStart { get { return Start.LogPosition != DocumentLogPosition.Zero; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("AutoCorrectInfoReplaceWith")]
#endif
		public object ReplaceWith { get { return replaceWith; } set { replaceWith = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("AutoCorrectInfoText")]
#endif
		public string Text {
			get {
				return text;
			}
		}
		#endregion
		public virtual void DecrementEndPosition() {
			this.end = iterator.MoveBack(end);
			if (text.Length >= 1)
				text = text.Remove(text.Length - 1, 1);
		}
		public virtual void IncrementEndPosition() {
			this.end = iterator.MoveForward(end);
			TextRunBase run = PieceTable.Runs[end.RunIndex];
			string value = run.GetPlainText(PieceTable.TextBuffer, end.RunOffset, end.RunOffset);
			if (value.Length > 1)
				value = value.Substring(0, 1);
			text += value;
		}
		public virtual bool DecrementStartPosition() {
			DocumentLogPosition pos = start.LogPosition;
			this.start = iterator.MoveBack(start);
			if (pos > start.LogPosition) {
				TextRunBase run = PieceTable.Runs[start.RunIndex];
				string value = run.GetPlainText(PieceTable.TextBuffer, start.RunOffset, start.RunOffset);
				if (value.Length > 1)
					value = value.Substring(0, 1);
				text = text.Insert(0, value);
				return true;
			}
			else
				return false;
		}
		public virtual bool IncrementStartPosition() {
			DocumentLogPosition pos = start.LogPosition;
			this.start = iterator.MoveForward(start);
			if (pos < start.LogPosition) {
				text = text.Remove(0, 1);
				return true;
			}
			else
				return false;
		}
	}
	#endregion
}

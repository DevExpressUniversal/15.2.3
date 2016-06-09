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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.API.Native {
	#region Capture
	[ComVisible(true)]
	public interface Capture {
		DocumentPosition Position { get; }
		int Length { get; }
		string Value { get; }
		DocumentRange GetRange();
	}
	#endregion
	#region CaptureCollection
	[ComVisible(true)]
	public interface CaptureCollection : ISimpleCollection<Capture> {
	}
	#endregion
	#region Group
	[ComVisible(true)]
	public interface Group : Capture {
		CaptureCollection Captures { get; }
	}
	#endregion
	#region GroupCollection
	[ComVisible(true)]
	public interface GroupCollection : ISimpleCollection<Group> {
	}
	#endregion
	#region Match
	[ComVisible(true)]
	public interface Match : Group {
		GroupCollection Groups { get; }
	}
	#endregion
	#region IRegexSearchResult
	[ComVisible(true)]
	public interface IRegexSearchResult : ISearchResult {
		Match Match { get; }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ExternalRegex = System.Text.RegularExpressions.Regex;
	using ExternalRegexOptions = System.Text.RegularExpressions.RegexOptions;
	using ExternalMatch = System.Text.RegularExpressions.Match;
	using ExternalCapture = System.Text.RegularExpressions.Capture;
	using ExternalGroup = System.Text.RegularExpressions.Group;
	using DevExpress.XtraRichEdit.Model;
	using DevExpress.XtraRichEdit.Utils;
	using System.Collections;
	using DevExpress.Office.Utils;
	#region NativeCapture
	public class NativeCapture : Capture {
		#region Fields
		readonly NativeSubDocument document;
		readonly DocumentLogPosition position;
		readonly string value;
		DocumentRange range;
		#endregion
		public NativeCapture(NativeSubDocument document, DocumentLogPosition position, string value) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			Guard.ArgumentNotNull(position, "position");
			this.position = position;
			Guard.ArgumentNotNull(value, "value");
			this.value = value;
		}
		protected NativeSubDocument Document { get { return document; } }
		#region Capture Members
		public DocumentPosition Position { get { return Document.CreatePositionCore(LogPosition); } }
		public int Length { get { return Value.Length; } }
		public string Value { get { return value; } }
		protected internal DocumentLogPosition LogPosition { get { return position; } }
		public DocumentRange GetRange() {
			if (range == null)
				range = document.CreateRange(Position, Length);
			return range;
		}
		#endregion
		public override string ToString() {
			return value;
		}
	}
	#endregion
	#region NativeCaptureCollection
	public class NativeCaptureCollection : List<NativeCapture>, CaptureCollection {
		#region ISimpleCollection<Capture> Members
		Capture ISimpleCollection<Capture>.this[int index] { get { return this[index]; } }
		#endregion
		#region IEnumerable<Capture> Members
		IEnumerator<Capture> IEnumerable<Capture>.GetEnumerator() {
			return new EnumeratorAdapter<Capture, NativeCapture>(this.GetEnumerator());
		}
		#endregion
	}
	#endregion
	#region NativeGroup
	public class NativeGroup : NativeCapture, Group {
		readonly NativeCaptureCollection captures;
		public NativeGroup(NativeSubDocument document, DocumentLogPosition position, string value)
			: base(document, position, value) {
			this.captures = new NativeCaptureCollection();
			this.captures.Add(this);
		}
		#region Group Members
		public CaptureCollection Captures { get { return captures; } }
		#endregion
	}
	#endregion
	#region NativeGroupCollection
	public class NativeGroupCollection : List<NativeGroup>, GroupCollection {
		#region ISimpleCollection<Group> Members
		Group ISimpleCollection<Group>.this[int index] { get { return this[index]; } }
		#endregion
		#region IEnumerable<Group> Members
		IEnumerator<Group> IEnumerable<Group>.GetEnumerator() {
			return new EnumeratorAdapter<Group, NativeGroup>(this.GetEnumerator());
		}
		#endregion
	}
	#endregion
	#region NativeMatch
	public class NativeMatch : NativeGroup, Match {
		#region Fields
		readonly NativeGroupCollection groups;
		readonly ExternalMatch match;
		#endregion
		public NativeMatch(NativeSubDocument document, DocumentLogPosition position, string value, ExternalMatch match)
			: base(document, position, value) {
			Guard.ArgumentNotNull(match, "match");
			this.match = match;
			this.groups = new NativeGroupCollection();
			this.groups.Add(this);
		}
		#region Match Members
		public GroupCollection Groups { get { return groups; } }
		#endregion
		internal string Result(string replacement) {
			return match.Result(replacement);
		}
	}
	#endregion
	#region NativeRegexSearch (abstract class)
	public abstract class NativeRegexSearch {
		readonly NativeSubDocument document;
		readonly ExternalRegex regex;
		readonly int maxGuaranteedSearchResultLength;
		protected NativeRegexSearch(NativeSubDocument document, ExternalRegex regex, int maxGuaranteedSearchResultLength) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			Guard.ArgumentNotNull(regex, "regex");
			this.regex = regex;
			Guard.ArgumentPositive(maxGuaranteedSearchResultLength, "int maxGuaranteedSearchResultLength");
			this.maxGuaranteedSearchResultLength = maxGuaranteedSearchResultLength;
		}
		protected internal NativeSubDocument Document { get { return document; } }
		protected internal ExternalRegex Regex { get { return regex; } }
		protected internal int MaxGuaranteedSearchResultLength { get { return maxGuaranteedSearchResultLength; } }
		protected internal virtual NativeMatch Match(DocumentRange range) {
			if (range.Length == 0)
				return null;
			BufferedRegexSearchBase regexSearch = CreateRegexSearch();
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			DocumentLogPosition startPosition = nativeRange.Start.LogPosition;
			DocumentLogPosition endPosition = Algorithms.Min(nativeRange.End.LogPosition, Document.PieceTable.DocumentEndLogPosition);
			BufferedRegexSearchResult result = regexSearch.Match(startPosition, endPosition);
			if (result == null)
				return null;
			return CreateMatch(result.Offset, result.Match);
		}
		protected internal virtual DocumentRange[] Matches(DocumentRange range) {
			BufferedRegexSearchBase regexSearch = CreateRegexSearch();
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			DocumentLogPosition startPosition = nativeRange.Start.LogPosition;
			DocumentLogPosition endPosition = Algorithms.Min(nativeRange.End.LogPosition, Document.PieceTable.DocumentEndLogPosition);
			BufferedRegexSearchResultCollection results = regexSearch.Matches(startPosition, endPosition);
			List<DocumentRange> ranges = new List<DocumentRange>();
			int count = results.Count;
			for (int i = 0; i < count; i++) {
				BufferedRegexSearchResult result = results[i];
				BufferedDocumentCharacterIteratorForward iterator = new BufferedDocumentCharacterIteratorForward(result.Offset);
				DocumentModelPosition start = iterator.GetPosition(result.Match.Index);
				DocumentModelPosition end = iterator.GetPosition(start, result.Match.Length);
				ranges.Add(new NativeDocumentRange(document, start, end));
			}
			return ranges.ToArray();
		}
		protected internal abstract NativeMatch NextMatch(Match match, DocumentRange range);
		protected internal virtual NativeMatch CreateMatch(DocumentModelPosition offset, ExternalMatch match) {
			DocumentLogPosition position = CreatePosition(offset.Clone(), match.Index);
			NativeMatch result = new NativeMatch(document, position, match.Value, match);
			NativeGroupCollection groups = (NativeGroupCollection)result.Groups;
			int groupsCount = match.Groups.Count;
			for (int i = 1; i < groupsCount; i++)
				groups.Add(CreateGroup(offset, match.Groups[i]));
			NativeCaptureCollection captures = (NativeCaptureCollection)result.Captures;
			int capturesCount = match.Captures.Count;
			for (int i = 1; i < capturesCount; i++)
				captures.Add(CreateCapture(offset, match.Captures[i]));
			return result;
		}
		protected internal virtual NativeGroup CreateGroup(DocumentModelPosition offset, ExternalGroup group) {
			DocumentLogPosition position = CreatePosition(offset.Clone(), group.Index);
			NativeGroup result = new NativeGroup(document, position, group.Value);
			NativeCaptureCollection captures = (NativeCaptureCollection)result.Captures;
			int count = group.Captures.Count;
			for (int i = 1; i < count; i++)
				captures.Add(CreateCapture(offset, group.Captures[i]));
			return result;
		}
		protected internal virtual NativeCapture CreateCapture(DocumentModelPosition offset, ExternalCapture capture) {
			DocumentLogPosition position = CreatePosition(offset.Clone(), capture.Index);
			return new NativeGroup(document, position, capture.Value);
		}
		protected internal virtual DocumentLogPosition CreatePosition(DocumentModelPosition pos, int offset) {
			BufferedDocumentCharacterIteratorForward iterator = new BufferedDocumentCharacterIteratorForward(pos);
			return iterator.GetPosition(offset).LogPosition;
		}
		protected abstract BufferedRegexSearchBase CreateRegexSearch();
	}
	#endregion
	#region NativeRegexSearchForward
	public class NativeRegexSearchForward : NativeRegexSearch {
		public NativeRegexSearchForward(NativeSubDocument document, ExternalRegex regex, int maxGuaranteedSearchResultLength)
			: base(document, regex, maxGuaranteedSearchResultLength) {
		}
		protected internal override NativeMatch NextMatch(Match match, DocumentRange range) {
			DocumentRange prevResult = match.GetRange();
			int length = range.End.LogPosition - prevResult.End.LogPosition;
			DocumentRange newRange = Document.CreateRange(prevResult.End, length);
			return Match(newRange);
		}
		protected override BufferedRegexSearchBase CreateRegexSearch() {
			return new BufferedRegexSearchForward(Document.PieceTable, Regex, MaxGuaranteedSearchResultLength);
		}
	}
	#endregion
	#region NativeRegexSearchBackward
	public class NativeRegexSearchBackward : NativeRegexSearch {
		public NativeRegexSearchBackward(NativeSubDocument document, ExternalRegex regex, int maxGuaranteedSearchResultLength)
			: base(document, regex, maxGuaranteedSearchResultLength) {
		}
		protected internal override NativeMatch NextMatch(Match match, DocumentRange range) {
			DocumentRange prevResult = match.GetRange();
			int length = prevResult.Start.LogPosition - range.Start.LogPosition + 1;
			DocumentRange newRange = Document.CreateRange(range.Start, length);
			return Match(newRange);
		}
		protected override BufferedRegexSearchBase CreateRegexSearch() {
			return new BufferedRegexSearchBackward(Document.PieceTable, Regex, MaxGuaranteedSearchResultLength);
		}
	}
	#endregion
	#region NativeRegexSearchResult (abstract class)
	public abstract class NativeRegexSearchResult : IRegexSearchResult {
		#region Fields
		readonly NativeRegexSearch regexSearch;
		readonly NativeSubDocument document;
		readonly DocumentRange range;
		readonly int maxGuaranteedSearchResultLength;
		NativeMatch match;
		#endregion
		protected NativeRegexSearchResult(NativeSubDocument document, ExternalRegex regex, DocumentRange range, int maxGuaranteedSearchResultLength) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			Guard.ArgumentNotNull(range, "range");
			this.range = range;
			Guard.ArgumentPositive(maxGuaranteedSearchResultLength, "maxGuaranteedSearchResultLength");
			this.maxGuaranteedSearchResultLength = maxGuaranteedSearchResultLength;
			this.regexSearch = CreateRegexSearch(document, regex);
		}
		protected internal int MaxGuaranteedSearchResultLength { get { return maxGuaranteedSearchResultLength; } }
		protected abstract NativeRegexSearch CreateRegexSearch(NativeSubDocument document, ExternalRegex regex);
		#region IRegexSearchResult Members
		public Match Match { get { return match; } }
		#endregion
		#region ISearchResult Members
		public DocumentRange CurrentResult {
			get {
				if (match == null)
					return null;
				return match.GetRange();
			}
		}
		public void Reset() {
			match = null;
		}
		public bool FindNext() {
			NativeMatch result = (match != null) ? regexSearch.NextMatch(match, range) : regexSearch.Match(range);
			if (result == null)
				return false;
			match = result;
			return true;
		}
		public void Replace(string replaceWith) {
			if (match == null)
				return;
			string result = match.Result(replaceWith);
			document.Replace(CurrentResult, result);
		}
		#endregion
	}
	#endregion
	#region NativeRegexSearchResultForward
	public class NativeRegexSearchResultForward : NativeRegexSearchResult {
		public NativeRegexSearchResultForward(NativeSubDocument document, ExternalRegex regex, DocumentRange range, int maxGuaranteedSearchResultLength)
			: base(document, regex, range, maxGuaranteedSearchResultLength) {
		}
		protected override NativeRegexSearch CreateRegexSearch(NativeSubDocument document, ExternalRegex regex) {
			return new NativeRegexSearchForward(document, regex, MaxGuaranteedSearchResultLength);
		}
	}
	#endregion
	#region NativeRegexSearchResultBackward
	public class NativeRegexSearchResultBackward : NativeRegexSearchResult {
		public NativeRegexSearchResultBackward(NativeSubDocument document, ExternalRegex regex, DocumentRange range, int maxGuaranteedSearchResultLength)
			: base(document, regex, range, maxGuaranteedSearchResultLength) {
		}
		protected override NativeRegexSearch CreateRegexSearch(NativeSubDocument document, ExternalRegex regex) {
			return new NativeRegexSearchBackward(document, regex, MaxGuaranteedSearchResultLength);
		}
	}
	#endregion
}

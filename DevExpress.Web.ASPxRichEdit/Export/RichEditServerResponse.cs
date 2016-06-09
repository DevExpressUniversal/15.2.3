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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
namespace DevExpress.Web.ASPxRichEdit.Export {
	public class RichEditServerResponse : IHashtableProvider {
		readonly PieceTable pieceTable;
		readonly ChunkHashtableCollection chunks;
		readonly ParagraphHashtableCollection paragraphs;
		readonly CharacterFormattingBaseExporter characterFormattingBaseExporter;
		readonly ParagraphFormattingBaseExporter paragraphFormattingBaseExporter;
		readonly CharacterFormattingInfoExporter mergedCharacterFormattingInfoExporter;
		readonly ParagraphFormattingInfoExporter mergedParagraphFormattingInfoExporter;
		public RichEditServerResponse(PieceTable pieceTable, WebFontInfoCache fontInfoCache) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.chunks = new ChunkHashtableCollection();
			this.paragraphs = new ParagraphHashtableCollection();
			this.mergedCharacterFormattingInfoExporter = new CharacterFormattingInfoExporter(fontInfoCache);
			this.characterFormattingBaseExporter = new CharacterFormattingBaseExporter(DocumentModel, fontInfoCache);
			this.mergedParagraphFormattingInfoExporter = new ParagraphFormattingInfoExporter();
			this.paragraphFormattingBaseExporter = new ParagraphFormattingBaseExporter(DocumentModel);
			FontInfoCache = fontInfoCache;
		}
		#region Properties
		#region Chunks
		public ChunkHashtableCollection Chunks {
			[DebuggerStepThrough]
			get { return chunks; }
		}
		#endregion
		#region Paragraphs
		public ParagraphHashtableCollection Paragraphs {
			[DebuggerStepThrough]
			get { return paragraphs; }
		}
		#endregion
		protected PieceTable PieceTable { get { return pieceTable; } }
		protected DocumentCache Cache { get { return pieceTable.DocumentModel.Cache; } }
		protected DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		protected internal WebFontInfoCache FontInfoCache { get; private set; }
		#endregion
		public void FillHashtable(Hashtable result) {
			CollectAllProperties();
			result.Add("chunks", this.chunks.ToHashtableCollection());
			result.Add("paragraphs", this.paragraphs.ToHashtableCollection());
			result.Add("characterPropertiesCache", this.characterFormattingBaseExporter.Cache);
			result.Add("mergedCharacterPropertiesCache", this.mergedCharacterFormattingInfoExporter.Cache);
			result.Add("paragraphPropertiesCache", this.paragraphFormattingBaseExporter.Cache);
			result.Add("mergedParagraphPropertiesCache", this.mergedParagraphFormattingInfoExporter.Cache);
			CollectAffectedIntervals(result);
		}
		void CollectAllProperties() {
			CollectMergedCharacterProperties();
			CollectCharacterProperties();
			CollectMergedParagraphProperties();
			CollectParagraphsProperties();
		}
		void CollectMergedCharacterProperties() {
			for(int i = 0; i < this.chunks.Count; i++)
				AddChunkMergedCharacterProperties(this.chunks[i]);
		}
		void AddChunkMergedCharacterProperties(Chunk chunk) {
			for (int i = 0; i < chunk.Runs.Count; i++)
				AddRunMergedCharacterProperties(chunk.Runs[i]);
		}
		void AddRunMergedCharacterProperties(WebTextRunBase run) {
			int key = run.MergedCharacterFormattingCacheIndex;
			this.mergedCharacterFormattingInfoExporter.RegisterItem(key, Cache.MergedCharacterFormattingInfoCache[key]);
		}
		void CollectCharacterProperties() {
			for (int i = 0; i < this.chunks.Count; i++)
				AddChunkCharacterProperties(this.chunks[i]);
		}
		void AddChunkCharacterProperties(Chunk chunk) {
			for (int i = 0; i < chunk.Runs.Count; i++)
				AddRunCharacterProperties(chunk.Runs[i]);
		}
		void AddRunCharacterProperties(WebTextRunBase run) {
			int key = run.CharacterPropertiesIndex;
			this.characterFormattingBaseExporter.RegisterItem(key, Cache.CharacterFormattingCache[key]);
		}
		void CollectMergedParagraphProperties() {
			for (int i = 0; i < this.paragraphs.Count; i++)
				AddMergedParagraphProperties(this.paragraphs[i]);
		}
		void AddMergedParagraphProperties(WebParagraph paragraph) {
			int key = paragraph.MergedParagraphFormattingCacheIndex;
			this.mergedParagraphFormattingInfoExporter.RegisterItem(key, Cache.MergedParagraphFormattingInfoCache[key]);
		}
		void CollectParagraphsProperties() {
			for (int i = 0; i < this.paragraphs.Count; i++)
				AddParagraphProperties(this.paragraphs[i]);
		}
		void AddParagraphProperties(WebParagraph paragraph) {
			int key = paragraph.ParagraphPropertiesIndex;
			this.paragraphFormattingBaseExporter.RegisterItem(key, Cache.ParagraphFormattingCache[key]);
		}
		void CollectAffectedIntervals(Hashtable hashtable) {
			AddNonEmptyHashCollection(CollectAffectedPermissions(), hashtable, "permissions");
		}
		void AddNonEmptyHashCollection<T>(HashtableCollection<T> collection, Hashtable hashtable, string name) where T : IHashtableProvider {
			if (collection.Count > 0)
				hashtable.Add(name, collection);
		}
		internal HashtableCollection<WebBookmark> CollectAffectedBookmarks() {
			return CollectAffectedIntervalCore(PieceTable.Bookmarks, WebBookmark.FromModelBookmark);
		}
		internal HashtableCollection<WebComment> CollectAffectedComments() {
			return CollectAffectedIntervalCore(PieceTable.Comments, WebComment.FromModelComment);
		}
		internal HashtableCollection<WebRangePermission> CollectAffectedPermissions() {
			return CollectAffectedIntervalCore(PieceTable.RangePermissions, WebRangePermission.FromModelRangePermission);
		}
		HashtableCollection<T> CollectAffectedIntervalCore<T, U>(BookmarkBaseCollection<U> sourceCollection, Func<U, T> converter)
			where T : IHashtableProvider
			where U : BookmarkBase {
			HashtableCollection<T> result = new HashtableCollection<T>();
			if (this.chunks.Count == 0)
				return result;
			DocumentLogPosition start = this.chunks[0].Start;
			DocumentLogPosition end = this.chunks[chunks.Count - 1].Start + this.chunks[chunks.Count - 1].Length - 1;
			for (int i = 0; i < sourceCollection.Count; i++) {
				U interval = sourceCollection[i];
				if (interval.End <= start)
					continue;
				if (interval.End < end) {
					result.Add(converter(interval));
					continue;
				}
				if (interval.Start > end)
					break;
				if (interval.Start >= start)
					result.Add(converter(interval));
				if (interval.Start < start && interval.End > end)
					result.Add(converter(interval));
			}
			return result;
		}
	}
}

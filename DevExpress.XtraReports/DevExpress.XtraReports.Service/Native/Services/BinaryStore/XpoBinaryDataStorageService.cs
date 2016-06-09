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
using System.IO;
using System.Linq;
using DevExpress.Printing.Core.Native;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraReports.Service.ConfigSections.Native;
using DevExpress.XtraReports.Service.Extensions;
using DevExpress.XtraReports.Service.Native.DAL;
namespace DevExpress.XtraReports.Service.Native.Services.BinaryStore {
	public class XpoBinaryDataStorageService : IBinaryDataStorageService {
		readonly IConfigurationService configurationService;
		int? chunkSize;
		public int ChunkSize {
			get {
				if(chunkSize == null) {
					chunkSize = GetChunkSize();
				}
				return chunkSize.Value;
			}
		}
		public XpoBinaryDataStorageService(IConfigurationService configurationService) {
			Guard.ArgumentNotNull(configurationService, "configurationService");
			this.configurationService = configurationService;
		}
		#region IBinaryDataStorageService
		public string Create(Stream stream, Session session) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(session, "session");
			var key = IdGenerator.GenerateRandomId();
			var owner = new BinaryChunkOwner(session) {
				LastModifiedTime = DateTime.UtcNow,
				ExternalKey = key
			};
			CreateChunks(stream, owner);
			return key;
		}
		public void Append(string key, Stream stream, Session session) {
			Guard.ArgumentNotNull(key, "key");
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNull(session, "session");
			var owner = LoadOwner(key, session);
			CreateChunks(stream, owner);
		}
		public Stream LoadStream(string key, Session session) {
			Guard.ArgumentNotNull(session, "session");
			if(key == null) {
				return null;
			}
			var owner = LoadOwner(key, session);
			return new BinaryChunkStream(owner.Chunks);
		}
		public byte[] LoadBytes(string key, Session session) {
			Guard.ArgumentNotNull(session, "session");
			if(key == null) {
				return null;
			}
			var owner = LoadOwner(key, session);
			return ToArray(owner.Chunks);
		}
		public void Delete(string key, Session session) {
			Guard.ArgumentNotNull(key, "key");
			Guard.ArgumentNotNull(session, "session");
			var owner = LoadOwner(key, session);
			session.Delete(owner);
		}
		public void Clean(IEnumerable<string> keys, Session session) {
			Guard.ArgumentNotNull(keys, "keys");
			Guard.ArgumentNotNull(session, "session");
			session.Delete(BinaryChunkOwner.FindByKeys(keys, session));
		}
		#endregion
		IList<BinaryChunk> CreateChunks(Stream stream, BinaryChunkOwner owner) {
			var result = new List<BinaryChunk>();
			while(true) {
				var buffer = new byte[ChunkSize];
				int size = stream.Read(buffer, 0, buffer.Length);
				if(size <= 0) {
					break;
				}
				byte[] content = size == buffer.Length
					? buffer
					: CreateBufferForChunk(buffer, size);
				var chunk = new BinaryChunk(owner.Session) {
					Owner = owner,
					Content = content
				};
				result.Add(chunk);
			}
			return result;
		}
		int GetChunkSize() {
			IDocumentDataStorageProvider documentStoreConfiguration = configurationService.DocumentStoreConfiguration;
			return documentStoreConfiguration != null
				? documentStoreConfiguration.BinaryStorageChunkSize
				: ConfigurationDefaultConstants.BinaryStorageChunkSize;
		}
		static BinaryChunkOwner LoadOwner(string key, Session session) {
			var owner = session.FindBy<BinaryChunkOwner>(x => x.ExternalKey, key);
			if(owner == null) {
				throw XpoEntityNotFoundException.Create<BinaryChunkOwner>(key);
			}
			owner.LastModifiedTime = DateTime.UtcNow;
			return owner;
		}
		static byte[] CreateBufferForChunk(byte[] buffer, int chunkSize) {
			var result = new byte[chunkSize];
			Buffer.BlockCopy(buffer, 0, result, 0, chunkSize);
			return result;
		}
		static byte[] ToArray(IEnumerable<IBinaryContentProvider> chunks) {
			var length = chunks.Sum(x => x.Content.LongLength);
			var result = new byte[length];
			int resultIndex = 0;
			foreach(var chunk in chunks) {
				var content = chunk.Content;
				var contentLength = content.Length;
				Buffer.BlockCopy(content, 0, result, resultIndex, contentLength);
				resultIndex += contentLength;
			}
			return result;
		}
	}
}

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
using System.Linq;
using System.Data.Services.Providers;
using System.IO;
using DevExpress.Xpo.Metadata;
using System.Data.Services;
using System.Text;
using System.Globalization;
namespace DevExpress.Xpo {
	internal class XpoStreamProvider : IDataServiceStreamProvider2, IDisposable {
		readonly XpoContext dataContext;
		string tempFile;
		Dictionary<string, Tuple<string, Stream>> namedStreamFiles;
		IXPSimpleObject cachedObject;
		IXPSimpleObject cachedNamedStreamObject;
		Stream streamBuffer;
		string streamProperty;
		public XpoStreamProvider(XpoContext dataContext) {
			this.dataContext = dataContext;
			tempFile = Path.GetTempFileName();
			namedStreamFiles = new Dictionary<string, Tuple<string, Stream>>();
		}
		#region IDataServiceStreamProvider Members
		void IDataServiceStreamProvider.DeleteStream(object entity, System.Data.Services.DataServiceOperationContext operationContext) { }
		System.IO.Stream IDataServiceStreamProvider.GetReadStream(object entity, string etag, bool? checkETagForEquality, System.Data.Services.DataServiceOperationContext operationContext) {
			if (checkETagForEquality != null) {
				throw new DataServiceException(400, "This sample service does not support the ETag header for a media resource.");
			}
			StreamInfo streamInfo;
			if (!dataContext.Metadata.TryResolveMediaResource(entity.GetType(), out streamInfo))
				throw new DataServiceException(500, string.Format("Cannot determine stream property for object of type '{0}'", entity.GetType()));
			IXPSimpleObject tempObject = (IXPSimpleObject)entity;
			byte[] streamValue = (byte[])(tempObject.ClassInfo.GetMember(streamInfo.PropertyName)).GetValue(tempObject);
			if (streamValue == null) return new MemoryStream();
			return new MemoryStream(streamValue);
		}
		Uri IDataServiceStreamProvider.GetReadStreamUri(object entity, System.Data.Services.DataServiceOperationContext operationContext) {
			return null;
		}
		string IDataServiceStreamProvider.GetStreamContentType(object entity, System.Data.Services.DataServiceOperationContext operationContext) {
			StreamInfo streamInfo;
			if (dataContext.Metadata.TryResolveMediaResource(entity.GetType(), out streamInfo)) return streamInfo.ContentType;
			return string.Empty;
		}
		string IDataServiceStreamProvider.GetStreamETag(object entity, System.Data.Services.DataServiceOperationContext operationContext) {
			return null;
		}
		System.IO.Stream IDataServiceStreamProvider.GetWriteStream(object entity, string etag, bool? checkETagForEquality, System.Data.Services.DataServiceOperationContext operationContext) {
			if (checkETagForEquality != null) {
				throw new DataServiceException(400, "This demo does not support ETags associated with BLOBs");
			}
			StreamInfo streamInfo;
			if (dataContext.Metadata.TryResolveMediaResource(entity.GetType(), out streamInfo)) streamProperty = streamInfo.PropertyName;
			else throw new DataServiceException(500, string.Format("Cannot determine stream property for object of type '{0}'", entity.GetType()));
			cachedObject = (IXPSimpleObject)entity;
			streamBuffer = new FileStream(tempFile, FileMode.Open);
			return streamBuffer;
		}
		string IDataServiceStreamProvider.ResolveType(string entitySetName, System.Data.Services.DataServiceOperationContext operationContext) {
			ResourceType resType;
			if (dataContext.Metadata.TryResolveResourceType(entitySetName, out resType)) return resType.FullName;
			return null;
		}
		int IDataServiceStreamProvider.StreamBufferSize {
			get { return 64000; }
		}
		#endregion
		#region IDataServiceStreamProvider2 Members
		Stream IDataServiceStreamProvider2.GetReadStream(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext) {
			IXPSimpleObject tempObject = (IXPSimpleObject)entity;
			NamedStreamInfo propertyData;
			if (!dataContext.Metadata.TryResolveNamedStream(entity.GetType(), streamProperty, out propertyData))
				new DataServiceException(500, string.Format("Cannot determine stream property for object of type '{0}'", entity.GetType()));
			XPMemberInfo mi = tempObject.ClassInfo.GetMember(propertyData.Name);
			object value = (mi.Converter == null) ? mi.GetValue(entity) : mi.Converter.ConvertToStorageType(mi.GetValue(entity));
			if (value == null) return new MemoryStream();
			byte[] streamValue = (mi.StorageType == typeof(string)) ? Encoding.UTF8.GetBytes((string)value) : (byte[])value;
			return new MemoryStream(streamValue);
		}
		Uri IDataServiceStreamProvider2.GetReadStreamUri(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext) {
			return null;
		}
		string IDataServiceStreamProvider2.GetStreamContentType(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext) {
			NamedStreamInfo propertyData;
			if (!dataContext.Metadata.TryResolveNamedStream(entity.GetType(), streamProperty, out propertyData))
				new DataServiceException(500, string.Format("Cannot determine stream property for object of type '{0}'", entity.GetType()));
			return propertyData.ContentType;
		}
		string IDataServiceStreamProvider2.GetStreamETag(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext) {
			return null;
		}
		Stream IDataServiceStreamProvider2.GetWriteStream(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext) {
			if (checkETagForEquality != null) {
				throw new DataServiceException(400, "This demo does not support ETags associated with BLOBs");
			}
			if (cachedNamedStreamObject == null) cachedNamedStreamObject = (IXPSimpleObject)entity;
			NamedStreamInfo propertyData;
			if (dataContext.Metadata.TryResolveNamedStream(entity.GetType(), streamProperty, out propertyData)) {
				string tempFileName;
				Stream tempFileStream;
				Tuple<string, Stream> streamBuffer;
				if (!namedStreamFiles.TryGetValue(propertyData.Name, out streamBuffer)) {
					tempFileName = Path.GetTempFileName();
					tempFileStream = new FileStream(tempFileName, FileMode.Open);
					streamBuffer = new Tuple<string, Stream>(tempFileName, tempFileStream);
					namedStreamFiles.Add(propertyData.Name, streamBuffer);
				}
				return streamBuffer.Item2;
			} else throw new DataServiceException(500, string.Format("Cannot determine stream property for object of type '{0}'", entity.GetType()));
		}
		#region IDisposable Members
		public void Dispose() {
			try {
				if (cachedObject != null) {
					XPMemberInfo mInfo = cachedObject.ClassInfo.GetMember(streamProperty);
					byte[] b = File.ReadAllBytes(tempFile);
					object streamValue;
					if(mInfo.StorageType == typeof(string))
						streamValue = (mInfo.Converter == null) ? Encoding.UTF8.GetString(b) : mInfo.Converter.ConvertFromStorageType(Encoding.UTF8.GetString(b));
					else
						streamValue = (mInfo.Converter == null) ? b : mInfo.Converter.ConvertFromStorageType(b);
					mInfo.SetValue(cachedObject, streamValue);
					((UnitOfWork)cachedObject.Session).CommitChanges();
				}
				if (cachedNamedStreamObject != null) {
					XPMemberInfo mInfo;
					Tuple<string, Stream> streamData;
					foreach (string property in namedStreamFiles.Keys) {
						mInfo = cachedNamedStreamObject.ClassInfo.GetMember(property);
						if (namedStreamFiles.TryGetValue(property, out streamData)) {
							byte[] b = File.ReadAllBytes(streamData.Item1);
							object streamValue;
							if (mInfo.StorageType == typeof(string))
								streamValue = (mInfo.Converter == null) ? Encoding.UTF8.GetString(b) : mInfo.Converter.ConvertFromStorageType(Encoding.UTF8.GetString(b));
							else
								streamValue = (mInfo.Converter == null) ? b : mInfo.Converter.ConvertFromStorageType(b);
							mInfo.SetValue(cachedNamedStreamObject, streamValue);
						}
					}
					((UnitOfWork)cachedNamedStreamObject.Session).CommitChanges();
				}
			} catch (Exception) {
			} finally {
				if (streamBuffer != null) streamBuffer.Dispose();
			}
			if (File.Exists(tempFile)) {
				try {
					File.Delete(tempFile);
				} catch (Exception) { }
			}
			if (namedStreamFiles.Count != 0) {
				foreach (Tuple<string, Stream> tempFiles in namedStreamFiles.Values) {
					if (tempFiles.Item2 != null) tempFiles.Item2.Dispose();
					if (File.Exists(tempFiles.Item1)) {
						try {
							File.Delete(tempFiles.Item1);
						} catch (Exception) { }
					}
				}
			}
		}
		#endregion
		#endregion
	}
	public class NamedStreamInfo {
		public string Name { get; set; }
		public string ContentType { get; set; }
		public NamedStreamInfo(string name, string contentType) {
			Name = name;
			ContentType = contentType;
		}
	}
	public class StreamInfo {
		public string PropertyName { get; set; }
		public string ContentType { get; set; }
		public StreamInfo(string propertyName, string contentType) {
			PropertyName = propertyName;
			ContentType = contentType;
		}
	}
}

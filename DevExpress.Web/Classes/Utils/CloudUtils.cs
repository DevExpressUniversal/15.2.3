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
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using DevExpress.Utils.OAuth;
namespace DevExpress.Web.Internal {
	public class CloudRequestEventArgs : EventArgs {
		public HttpWebRequest Request { get; private set; }
		public CloudRequestEventArgs(HttpWebRequest request) {
			Request = request;
		}
	}
	public class CloudRequestExceptionEventArgs : EventArgs {
		public HttpStatusCode ResponseCode { get; private set; }
		public string RequestResult { get; private set; }
		public Exception Exception { get; set; }
		public CloudRequestExceptionEventArgs(HttpStatusCode responseCode, string requestResult) {
			ResponseCode = responseCode;
			RequestResult = requestResult;
		}
	}
	public delegate void CloudRequestEventHandler(object sender, CloudRequestEventArgs e);
	public delegate void CloudRequestExceptionEventHander(object sender, CloudRequestExceptionEventArgs e);
	public class RequestInfo {
		public const int ContentSizeNotSpecified = 0;
		public const HttpStatusCode SuccessCodeNotSpecified = HttpStatusCode.OK;
		public string ObjectName { get; set; }
		public NameValueCollection Parameters { get; set; }
		public NameValueCollection Headers { get; set; }
		public NameValueCollection OutputHeaders { get; set; }
		public Stream Content { get; set; }
		public int ContentSize { get; set; }
		public string ContentType { get; set; }
		public HttpStatusCode SuccessCode { get; set; }
		public string HttpMethod { get; set; }
		public DateTime Date { get; set; }
		public RequestInfo() {
		}
		public RequestInfo(string objectName, string httpMethod)
			: this(objectName, httpMethod, SuccessCodeNotSpecified) {
		}
		public RequestInfo(string objectName, string httpMethod, NameValueCollection parameters)
			: this(objectName, httpMethod, parameters, null) {
		}
		public RequestInfo(string objectName, string httpMethod, NameValueCollection parameters, Stream content)
			: this(objectName, httpMethod, SuccessCodeNotSpecified, content, 0, parameters, null){
		}
		public RequestInfo(string objectName, string httpMethod, HttpStatusCode successCode)
			: this(objectName, httpMethod, successCode, null, 0) {
		}
		public RequestInfo(string objectName, string httpMethod, HttpStatusCode successCode, Stream content, int contentSize)
			: this(objectName, httpMethod, successCode, content, contentSize, null, null) {
		}
		public RequestInfo(string objectName, string httpMethod, HttpStatusCode successCode, Stream content, int contentSize,
			NameValueCollection parameters, NameValueCollection headers) {
			ObjectName = objectName;
			HttpMethod = httpMethod;
			SuccessCode = successCode;
			Content = content;
			ContentSize = contentSize;
			Parameters = parameters;
			Headers = headers;
		}
	}
	class DropboxChunkedUploadInfo {
		public DropboxChunkedUploadInfo() {
			upload_id = string.Empty;
		}
		public string upload_id { get; set; }
		public string offset { get; set; }
		public string expires { get; set; }
	}
	public abstract class CloudServiceHelperBase {
		public event CloudRequestEventHandler CloudRequestEvent;
		public event CloudRequestExceptionEventHander CloudRequestExceptionEvent;
		public HttpWebRequest GetRequest(string objectName, string httpMethod) {
			RequestInfo requestInfo = new RequestInfo(objectName, httpMethod);
			return GetRequest(requestInfo);
		}
		protected virtual void PrepareRequestHeaders(HttpWebRequest request, RequestInfo requestInfo) {
			throw new NotImplementedException();
		}
		protected virtual int GetRequestContentLength(RequestInfo requestInfo) {
			throw new NotImplementedException();
		}
		protected virtual string GetRequestUrlCore(RequestInfo requestInfo) {
			throw new NotImplementedException();
		}
		protected string GetRequestUrl(RequestInfo requestInfo) {
			string url = GetRequestUrlCore(requestInfo);
			if(requestInfo.Parameters == null || requestInfo.Parameters.Count == 0)
				return url;
			string queryString = requestInfo.Parameters.Keys.Cast<string>().
				Select(key => new {
					Name = key,
					Value = requestInfo.Parameters[key]
				}).
				Select(entry => string.IsNullOrEmpty(entry.Value) ? entry.Name : entry.Name + "=" + CloudUtils.UrlEncode(entry.Value)).
				Aggregate((part1, part2) => part1 + "&" + part2);
			return url + "?" + queryString;
		}
		protected string ExecuteRequest(RequestInfo requestInfo) {
			HttpWebRequest request = GetRequest(requestInfo);
			return ExecuteRequest(request, requestInfo);
		}
		string ExecuteRequest(HttpWebRequest request, RequestInfo requestInfo) {
			RaiseCloudRequestEvent(request);
			HttpStatusCode responseCode;
			string requestResult = string.Empty;
			try {
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				using(Stream stream = response.GetResponseStream()) {
					using(StreamReader reader = new StreamReader(stream)) {
						requestResult = reader.ReadToEnd();
					}
				}
				responseCode = response.StatusCode;
				if(requestInfo.OutputHeaders != null) {
					List<string> keys = requestInfo.OutputHeaders.Keys.Cast<string>().ToList();
					foreach(string key in keys)
						requestInfo.OutputHeaders[key] = response.Headers[key];
				}
				response.Close();
			}
			catch(WebException ex) {
				HttpWebResponse response = ex.Response as HttpWebResponse;
				using(Stream stream = ex.Response.GetResponseStream()) {
					using(StreamReader reader = new StreamReader(stream)) {
						requestResult = reader.ReadToEnd();
					}
				}
				throw GetRequestException(response.StatusCode, requestResult);
			}
			if(responseCode != requestInfo.SuccessCode)
				throw GetRequestException(responseCode, requestResult);
			return requestResult;
		}
		protected HttpWebRequest GetRequest(RequestInfo requestInfo) {
			HttpWebRequest request = CreateRequest(GetRequestUrl(requestInfo), requestInfo.HttpMethod);
			PrepareRequest(request, requestInfo);
			return request;
		}
		protected virtual HttpWebRequest CreateRequest(string url, string HttpMethod) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = HttpMethod;
			return request;
		}
		protected virtual void PrepareRequest(HttpWebRequest request, RequestInfo requestInfo) {
			requestInfo.Date = DateTime.UtcNow;
			PrepareRequestHeaders(request, requestInfo);
			PrepareRequestContent(request, requestInfo);
		}
		void PrepareRequestContent(HttpWebRequest request, RequestInfo requestInfo) {
			if(requestInfo.Content != null) {
				request.ContentLength = GetRequestContentLength(requestInfo);
				requestInfo.Content.Position = 0;
				using(Stream requestStream = request.GetRequestStream()) {
					requestInfo.Content.CopyTo(requestStream);
					requestStream.Close();
				}
			}
			else
				request.ContentLength = 0;
		}
		Exception RaiseCloudRequestExceptionEvent(HttpStatusCode responseCode, string requestResult) {
			Exception exception = null;
			if(CloudRequestExceptionEvent != null) {
				CloudRequestExceptionEventArgs args = new CloudRequestExceptionEventArgs(responseCode, requestResult);
				CloudRequestExceptionEvent(this, args);
				exception = args.Exception;
			}
			return exception;
		}
		void RaiseCloudRequestEvent(HttpWebRequest request) {
			if(CloudRequestEvent != null)
				CloudRequestEvent(this, new CloudRequestEventArgs(request));
		}
		Exception GetRequestException(HttpStatusCode responseCode, string requestResult) {
			Exception exception = RaiseCloudRequestExceptionEvent(responseCode, requestResult);
			if(exception != null)
				return exception;
			string errorMessage = string.Format("Bad request.\nStatusCode: {0}\nRequestResult: {1}", responseCode, requestResult);
			return new Exception(errorMessage);
		}
	}
	public class AzureBlobStorageHelper : CloudServiceHelperBase {
		public string Account { get; set; }
		public string Key { get; set; }
		public string Container { get; set; }
		public AzureBlobStorageHelper() {
		}
		public AzureBlobStorageHelper(string account, string key, string container) {
			Account = account;
			Key = key;
			Container = container;
		}
		public void PutBlockList(string blobName, long blockCount, string contentType) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "comp", "blocklist" }
			};
			string content = GetBlockListRequestContent(blockCount);
			byte[] contentData = Encoding.UTF8.GetBytes(content);
			MemoryStream contentStream = new MemoryStream(contentData);
			RequestInfo requestInfo = new RequestInfo(blobName, "PUT", HttpStatusCode.Created, contentStream, contentData.Length, parameters, null);
			requestInfo.ContentType = contentType;
			ExecuteRequest(requestInfo);
		}
		public void PutBlock(string blobName, Stream blockData, int blockSize, long blockIndex) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "comp", "block" },
				{ "blockid", GetBlockID(blockIndex) }
			};
			RequestInfo requestInfo = new RequestInfo(blobName, "PUT", HttpStatusCode.Created, blockData, blockSize, parameters, null);
			ExecuteRequest(requestInfo);
		}
		public void PutBlob(string blobName, Stream blobStream) {
			PutBlob(blobName, blobStream, null);
		}
		public void PutBlob(string blobName, Stream blobStream, string contentType) {
			NameValueCollection headers = new NameValueCollection() {
				{ "x-ms-blob-type", "BlockBlob" }
			};
			int blobSize = blobStream != null ? (int)blobStream.Length : 0;
			RequestInfo requestInfo = new RequestInfo(blobName, "PUT", HttpStatusCode.Created, blobStream, blobSize, null, headers);
			requestInfo.ContentType = contentType;
			ExecuteRequest(requestInfo);
		}
		public string GetDownloadUrl(string path) {
			string version = "2012-02-12";
			string expireTime = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ");
			string permission = "r";
			string stringToSign = permission + "\n" + "\n" + expireTime + "\n" + "/" + Account + "/" + Container + "/" + path + "\n" + "\n" + version;
			string signature = GetSignature(stringToSign);
			NameValueCollection parameters = new NameValueCollection() {
				{ "sv", version },
				{ "se", expireTime },
				{ "sr", "b" },
				{ "sp", permission },
				{ "sig", signature }
			};
			RequestInfo requestInfo = new RequestInfo { ObjectName = path, Parameters = parameters };
			return GetRequestUrl(requestInfo);
		}
		public string GetBlobsResponse(string rootFolder) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "comp", "list" }
			};
			if(!string.IsNullOrEmpty(rootFolder))
				parameters.Add("prefix", rootFolder);
			RequestInfo requestInfo = new RequestInfo(string.Empty, "GET", HttpStatusCode.OK, null, 0, parameters, null);
			return ExecuteRequest(requestInfo);
		}
		public void DeleteItem(string path) {
			RequestInfo requestInfo = new RequestInfo(path, "DELETE", HttpStatusCode.Accepted);
			ExecuteRequest(requestInfo);
		}
		public void CopyItem(string path, string sourcePath) {
			NameValueCollection headers = new NameValueCollection() {
					{ "x-ms-copy-source", sourcePath }
				};
			RequestInfo requestInfo = new RequestInfo(path, "PUT", HttpStatusCode.Accepted, null, 0, null, headers);
			ExecuteRequest(requestInfo);
		}
		protected override void PrepareRequestHeaders(HttpWebRequest request, RequestInfo requestInfo) {
			NameValueCollection reqHeaders = new NameValueCollection() {
				{ "x-ms-date", GetRequestDate() }
			};
			bool isPutOperation = request.Method == "PUT";
			if(isPutOperation)
				reqHeaders.Add("x-ms-version", "2012-02-12");
			if(!string.IsNullOrEmpty(requestInfo.ContentType))
				reqHeaders.Add("x-ms-blob-content-type", requestInfo.ContentType);
			if(requestInfo.Headers != null && requestInfo.Headers.Count > 0) {
				foreach(string headerKey in requestInfo.Headers.Keys)
					reqHeaders.Add(headerKey, requestInfo.Headers[headerKey]);
			}
			request.Headers.Add(reqHeaders);
			if(isPutOperation)
				reqHeaders.Add("Content-Length", requestInfo.ContentSize.ToString());
			string canonHeaders = GetCanonicalizedHeaders(reqHeaders);
			string canonResource = GetCanonicalizedResource(requestInfo.ObjectName, requestInfo.Parameters, isPutOperation);
			string stringToSign = GetStringToSing(request.Method, reqHeaders, canonHeaders, canonResource, isPutOperation);
			string signature = GetSignature(stringToSign);
			string authorizationHeaderValue = string.Format("SharedKey {0}:{1}", Account, signature);
			request.Headers.Add("Authorization", authorizationHeaderValue);
		}
		protected override int GetRequestContentLength(RequestInfo requestInfo) {
			return requestInfo.ContentSize;
		}
		protected override string GetRequestUrlCore(RequestInfo requestInfo) {
			string url = string.Format(@"https://{0}.blob.core.windows.net/{1}/{2}", Account,
				CloudUtils.UrlEncode(Container), CloudUtils.UrlEncode(requestInfo.ObjectName));
			if(url.EndsWith("/"))
				url = url.Remove(url.Length - 1);
			return url;
		}
		string GetBlockListRequestContent(long blockCount) {
			StringBuilder sb = new StringBuilder();
			using(XmlWriter xmlWriter = XmlWriter.Create(sb)) {
				xmlWriter.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
				xmlWriter.WriteWhitespace("\n");
				xmlWriter.WriteStartElement("BlockList");
				for(long i = 0; i < blockCount; i++) {
					xmlWriter.WriteElementString("Latest", GetBlockID(i));
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Close();
			}
			return sb.ToString();
		}
		string GetBlockID(long chunkIndex) {
			int blockIDCharCount = 10;
			string blockIDStr = chunkIndex.ToString("D" + blockIDCharCount);
			byte[] blockIDData = Encoding.UTF8.GetBytes(blockIDStr);
			return Convert.ToBase64String(blockIDData);
		}
		string GetRequestDate() {
			return DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
		}
		string GetCanonicalizedHeaders(NameValueCollection headers) {
			var headersSequence = headers.Keys.Cast<string>().
				Where(key => key.StartsWith("x-ms-", StringComparison.InvariantCultureIgnoreCase)).
				Select(key => new {
					Key = key,
					OutputKey = key.ToLowerInvariant().Trim(),
					OutputValue = headers[key].Trim()
				}).
				OrderBy(entry => entry.OutputKey).
				Select(entry => string.Format("{0}:{1}\n", entry.OutputKey, entry.OutputValue));
			return string.Join(string.Empty, headersSequence);
		}
		string GetCanonicalizedResource(string blobName, NameValueCollection parameters, bool isPutOperation) {
			StringBuilder sb = new StringBuilder();
			sb.Append("/" + Account);
			sb.Append("/" + CloudUtils.UrlEncode(Container) + (string.IsNullOrEmpty(blobName) ? string.Empty : "/" + CloudUtils.UrlEncode(blobName)));
			if(parameters == null || parameters.Count == 0)
				return sb.ToString();
			sb.Append(isPutOperation ? "\n" : "?");
			var paramsSequence = parameters.Keys.Cast<string>().
				Select(key => new {
					Key = key,
					OutputKey = key.ToLowerInvariant(),
					Value = parameters[key]
				}).
				Where(param => param.Key != "prefix").
				OrderBy(entry => entry.OutputKey).
				Select(entry => string.Format("{0}{1}{2}", entry.OutputKey, isPutOperation ? ":" : "=", entry.Value));
			sb.Append(string.Join(isPutOperation ? "\n" : "&", paramsSequence));
			return sb.ToString();
		}
		string GetStringToSing(string verb, NameValueCollection headers, string canonicalizedHeaders, string canonicalizedResource, bool isPutOperation) {
			string[] headerNames = isPutOperation ?
				new[] { "Content-Encoding", "Content-Language", "Content-Length", "Content-MD5", "Content-Type", "Date", 
				"If-Modified-Since", "If-Match", "If-None-Match", "If-Unmodified-Since", "Range" } :
				new[] { "Content-Length", "Content-Type", "Date" };
			char delimeter = '\n';
			StringBuilder sb = new StringBuilder();
			sb.Append(verb);
			sb.Append(delimeter);
			foreach(string headerName in headerNames) {
				string headerValue = headers[headerName];
				if(!string.IsNullOrWhiteSpace(headerValue))
					sb.Append(headerValue);
				sb.Append(delimeter);
			}
			sb.Append(canonicalizedHeaders);
			sb.Append(canonicalizedResource);
			return sb.ToString();
		}
		string GetSignature(string stringToSing) {
			byte[] data = Encoding.UTF8.GetBytes(stringToSing);
			byte[] key = Convert.FromBase64String(Key);
			byte[] hash = null;
			using(HMACSHA256 hasher = new HMACSHA256(key)) {
				hash = hasher.ComputeHash(data);
			}
			return Convert.ToBase64String(hash);
		}
	}
	public class AmazonS3Helper : CloudServiceHelperBase {
		const string DefaultRegion = "us-east-1";
		string region;
		class HeaderEntry {
			public string Name { get; set; }
			public string CanonName { get; set; }
		}
		public string AccessKey { get; set; }
		public string SecretAccessKey { get; set; }
		public string BucketName { get; set; }
		public string Region {
			get {
				if(string.IsNullOrEmpty(region))
					return DefaultRegion;
				return region;
			}
			set {
				region = value;
			}
		}
		public AmazonS3Helper() {
		}
		public AmazonS3Helper(string accessKey, string secretAccessKey, string bucketName)
			: this(accessKey, secretAccessKey, bucketName, string.Empty) {
		}
		public AmazonS3Helper(string accessKey, string secretAccessKey, string bucketName, string region) {
			AccessKey = accessKey;
			SecretAccessKey = secretAccessKey;
			BucketName = bucketName;
			Region = region;
		}
		public string InitiateMultipartUpload(string objectName, string contentType) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "uploads", "" }
			};
			RequestInfo requestInfo = new RequestInfo(objectName, "POST", HttpStatusCode.OK, null, 0, parameters, null);
			requestInfo.ContentType = contentType;
			string requestResult = ExecuteRequest(requestInfo);
			return ParseInitiateUploadResult(requestResult);
		}
		public void AbortMultipartUpload(string objectName, string uploadID) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "uploadId", uploadID }
			};
			RequestInfo requestInfo = new RequestInfo(objectName, "DELETE", HttpStatusCode.NoContent, null, 0, parameters, null);
			ExecuteRequest(requestInfo);
		}
		public string UploadPart(string objectName, Stream partData, string uploadID, int partNumber) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "partNumber", partNumber.ToString() },
				{ "uploadId", uploadID }
			};
			NameValueCollection responseHeaders = new NameValueCollection() {
				{ "ETag", null }
			};
			RequestInfo requestInfo = new RequestInfo(
				objectName, "PUT", HttpStatusCode.OK, partData, RequestInfo.ContentSizeNotSpecified, parameters, null);
			requestInfo.OutputHeaders = responseHeaders;
			ExecuteRequest(requestInfo);
			return requestInfo.OutputHeaders["ETag"];
		}
		public void CompleteMultipartUpload(string objectName, string uploadID, List<string> eTags) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "uploadId", uploadID }
			};
			byte[] content = GetCompleteUploadContent(eTags);
			MemoryStream contentStream = new MemoryStream(content);
			RequestInfo requestInfo = new RequestInfo(
				objectName, "POST", HttpStatusCode.OK, contentStream, RequestInfo.ContentSizeNotSpecified, parameters, null);
			ExecuteRequest(requestInfo);
		}
		public void PutObject(string objectName, Stream content, string contentType) {
			RequestInfo requestInfo = new RequestInfo(objectName, "PUT", HttpStatusCode.OK, content, RequestInfo.ContentSizeNotSpecified);
			requestInfo.ContentType = contentType;
			ExecuteRequest(requestInfo);
		}
		public string GetDownloadUrl(string path) {
			DateTime dateTimeNow = DateTime.UtcNow;
			var expiresOn = DateTime.UtcNow.AddDays(1);
			var period = Convert.ToInt64((expiresOn.ToUniversalTime() - DateTime.UtcNow).TotalSeconds);
			NameValueCollection parameters = new NameValueCollection() {
				{ "X-Amz-Expires", period.ToString() },
				{ "X-Amz-Algorithm", "AWS4-HMAC-SHA256" },
				{ "X-Amz-Credential", string.Format("{0}/{1}", AccessKey, GetScope(dateTimeNow)) },
				{ "X-Amz-Date", dateTimeNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture) },
				{ "X-Amz-SignedHeaders",  "host" },
			};
			NameValueCollection headers = new NameValueCollection() {
				{ "Host", GetRequestHost() }
			};
			RequestInfo requestInfo = new RequestInfo(
				path, "GET", RequestInfo.SuccessCodeNotSpecified, null, RequestInfo.ContentSizeNotSpecified, parameters, headers);
			requestInfo.Date = dateTimeNow;
			string signature = GetSignature(requestInfo, true);
			parameters.Add("X-Amz-Signature", signature);
			return GetRequestUrl(requestInfo);
		}
		public string GetContentsList(string rootFolder) {
			return GetContentsList(rootFolder, null);
		}
		public string GetContentsList(string rootFolder, string marker) {
			NameValueCollection parameters = new NameValueCollection();
			if(!string.IsNullOrEmpty(rootFolder))
				parameters.Add("prefix", rootFolder);
			if(!string.IsNullOrEmpty(marker))
				parameters.Add("marker", marker);
			RequestInfo requestInfo = new RequestInfo(string.Empty, "GET", HttpStatusCode.OK, null, 0, parameters, null);
			return ExecuteRequest(requestInfo);
		}
		public void DeleteItem(string path) {
			RequestInfo requestInfo = new RequestInfo(path, "DELETE", HttpStatusCode.NoContent);
			ExecuteRequest(requestInfo);
		}
		public void DeleteItems(List<string> paths) {
			XmlDocument body = new XmlDocument();
			XmlElement deleteNode = body.CreateElement("Deleted");
			foreach(var path in paths) {
				XmlElement objectElement = body.CreateElement("Object");
				XmlElement keyElement = body.CreateElement("Key");
				keyElement.InnerText = path;
				objectElement.AppendChild(keyElement);
				deleteNode.AppendChild(objectElement);
			}
			body.AppendChild(deleteNode);
			NameValueCollection parameters = new NameValueCollection() {
				{ "delete", string.Empty }
			};
			using(Stream stream = new MemoryStream()) {
				body.Save(stream);
				stream.Position = 0;
				string md5Hash = string.Empty;
				using(MD5 md5 = MD5.Create()) {
					byte[] hash = md5.ComputeHash(stream);
					md5Hash = Convert.ToBase64String(hash);
				}
				NameValueCollection headers = new NameValueCollection() {
					{ "Content-MD5", md5Hash }
				};
				RequestInfo requestInfo = new RequestInfo(
					string.Empty, "POST", HttpStatusCode.OK, stream, RequestInfo.ContentSizeNotSpecified, parameters, headers);
				ExecuteRequest(requestInfo);
			}
		}
		public void CopyItem(string path, string sourcePath) {
			NameValueCollection headers = new NameValueCollection() {
				{ "x-amz-copy-source", sourcePath }
			};
			RequestInfo requestInfo = new RequestInfo(path, "PUT", HttpStatusCode.OK, null, 0, null, headers);
			ExecuteRequest(requestInfo);
		}
		public void CreateFolder(string path) {
			RequestInfo requestInfo = new RequestInfo(path, "PUT", HttpStatusCode.OK);
			ExecuteRequest(requestInfo);
		}
		byte[] GetCompleteUploadContent(List<string> eTags) {
			XDocument doc = new XDocument(
				new XElement("CompleteMultipartUpload",
					eTags.Select((eTag, index) =>
						new XElement("Part",
							new XElement("PartNumber", (index + 1).ToString()),
							new XElement("ETag", eTag)))));
			string contentString = doc.ToString(SaveOptions.DisableFormatting);
			return Encoding.UTF8.GetBytes(contentString);
		}
		string ParseInitiateUploadResult(string result) {
			XElement element = XElement.Parse(result);
			return element.Elements().First(el => el.Name.LocalName == "UploadId").Value;
		}
		protected override void PrepareRequestHeaders(HttpWebRequest request, RequestInfo requestInfo) {
			NameValueCollection reqHeaders = new NameValueCollection() {
				{ "x-amz-date", requestInfo.Date.ToString("R", CultureInfo.InvariantCulture) }
			};
			reqHeaders.Add("x-amz-content-sha256", GetDataHash(requestInfo.Content));
			if(requestInfo.Headers != null && requestInfo.Headers.Count > 0) {
				foreach(string headerKey in requestInfo.Headers.Keys) {
					reqHeaders.Add(headerKey, headerKey == "x-amz-copy-source"
						? CloudUtils.UrlEncode(requestInfo.Headers[headerKey]) : requestInfo.Headers[headerKey]);
				}
			}
			request.Headers.Add(reqHeaders);
			string requestHost = GetRequestHost();
			request.Host = requestHost;
			if(!string.IsNullOrEmpty(requestInfo.ContentType)) {
				request.ContentType = requestInfo.ContentType;
				reqHeaders.Add("Content-Type", requestInfo.ContentType);
			}
			reqHeaders.Add("Content-Length", requestInfo.Content == null ? "0" : requestInfo.Content.Length.ToString());
			reqHeaders.Add("Host", requestHost);
			requestInfo.Headers = reqHeaders;
			string authorizationHeaderValue = GetAuthorizationHeaderValue(requestInfo);
			request.Headers.Add("Authorization", authorizationHeaderValue);
		}
		protected override int GetRequestContentLength(RequestInfo requestInfo) {
			return (int)requestInfo.Content.Length;
		}
		protected override string GetRequestUrlCore(RequestInfo requestInfo) {
			return string.Format(@"https://{0}{1}", GetRequestHost(), GetResourceUri(requestInfo.ObjectName));
		}
		string GetAuthorizationHeaderValue(RequestInfo requestInfo) {
			string scope = GetScope(requestInfo.Date);
			string signedHeaders = GetSignedHeaders(requestInfo.Headers);
			string signature = GetSignature(requestInfo, false);
			return string.Format("AWS4-HMAC-SHA256 Credential={0}/{1},SignedHeaders={2},Signature={3}",
				AccessKey, scope, signedHeaders, signature);
		}
		string GetScope(DateTime date) {
			return date.ToString("yyyyMMdd") + "/" + Region + "/s3/aws4_request";
		}
		string GetSignedHeaders(NameValueCollection headers) {
			string result = GetCanonHeaders(headers).
				Select(entry => entry.CanonName).
				Aggregate((name1, name2) => name1 + ";" + name2);
			return result;
		}
		IEnumerable<HeaderEntry> GetCanonHeaders(NameValueCollection headers) {
			string[] headerNames = { "host", "content-type", "content-md5" };
			var entrySequence = headers.Keys.Cast<string>().
				Select(key => new HeaderEntry {
					Name = key,
					CanonName = key.ToLowerInvariant()
				}).
				Where(entry => (entry.CanonName.StartsWith("x-amz-", StringComparison.InvariantCultureIgnoreCase)
					|| Array.IndexOf(headerNames, entry.CanonName) >= 0)).
				OrderBy(entry => entry.CanonName);
			return entrySequence;
		}
		string GetCanonicalizedHeaders(NameValueCollection headers) {
			string result = GetCanonHeaders(headers).
				Select(entry => string.Format("{0}:{1}\n", entry.CanonName, headers[entry.Name].Trim())).
				Aggregate((part1, part2) => part1 + part2);
			return result;
		}
		string GetSignature(RequestInfo requestInfo, bool isDownloadSignature) {
			string stringToSing = GetStringToSign(requestInfo, isDownloadSignature);
			byte[] signingKey = GetSigningKey(requestInfo.Date);
			byte[] signatureData = GetHMACSHA256(stringToSing, signingKey);
			return ConvertByteArrayToHexString(signatureData);
		}
		string GetStringToSign(RequestInfo requestInfo, bool isDownloadSignature) {
			StringBuilder sb = new StringBuilder("AWS4-HMAC-SHA256" + "\n");
			sb.Append(requestInfo.Date.ToString(isDownloadSignature ? "yyyyMMddTHHmmssZ" : "R", CultureInfo.InvariantCulture) + "\n");
			sb.Append(GetScope(requestInfo.Date) + "\n");
			string canonicalRequest = GetCanonicalRequest(requestInfo, isDownloadSignature);
			byte[] canonRequestData = Encoding.UTF8.GetBytes(canonicalRequest);
			string canonRequestHash = GetDataHash(canonRequestData);
			sb.Append(canonRequestHash);
			return sb.ToString();
		}
		byte[] GetSigningKey(DateTime date) {
			string secretKeyString = "AWS4" + SecretAccessKey;
			byte[] secretKey = Encoding.UTF8.GetBytes(secretKeyString);
			byte[] dateKey = GetHMACSHA256(date.ToString("yyyyMMdd"), secretKey);
			byte[] dateRegionKey = GetHMACSHA256(Region, dateKey);
			byte[] dateRegionServiceKey = GetHMACSHA256("s3", dateRegionKey);
			return GetHMACSHA256("aws4_request", dateRegionServiceKey);
		}
		string GetCanonicalRequest(RequestInfo requestInfo, bool isDownloadSignature) {
			string delimeter = "\n";
			StringBuilder sb = new StringBuilder(requestInfo.HttpMethod + delimeter);
			sb.Append(GetResourceUri(requestInfo.ObjectName) + delimeter);
			sb.Append(GetCanonicalQueryString(requestInfo.Parameters) + delimeter);
			sb.Append(GetCanonicalizedHeaders(requestInfo.Headers) + delimeter);
			sb.Append(GetSignedHeaders(requestInfo.Headers) + delimeter);
			sb.Append(isDownloadSignature ? "UNSIGNED-PAYLOAD" : GetDataHash(requestInfo.Content));
			return sb.ToString();
		}
		string GetCanonicalQueryString(NameValueCollection parameters) {
			if(parameters == null || parameters.Count == 0)
				return string.Empty;
			string result = parameters.Keys.Cast<string>().
				Select(key => new {
					OutputKey = CloudUtils.UrlEncode(key),
					OutputValue = CloudUtils.UrlEncode(parameters[key], true)
				}).
				OrderBy(entry => entry.OutputKey).
				Select(entry => string.Format("{0}={1}", entry.OutputKey, entry.OutputValue)).
				Aggregate((entry1, entry2) => entry1 + "&" + entry2);
			return result;
		}
		string GetRequestHost() {
			return string.Format("{0}.s3.amazonaws.com", BucketName);
		}
		string GetResourceUri(string objectName) {
			return "/" + CloudUtils.UrlEncode(objectName);
		}
		string GetDataHash(byte[] data) {
			Stream dataStream = new MemoryStream(data);
			return GetDataHash(dataStream);
		}
		string GetDataHash(Stream data) {
			if(data == null || data.Length == 0)
				return "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
			byte[] hash = null;
			data.Position = 0;
			using(SHA256Managed hasher = new SHA256Managed()) {
				hash = hasher.ComputeHash(data);
			}
			return ConvertByteArrayToHexString(hash);
		}
		byte[] GetHMACSHA256(string dataString, byte[] key) {
			byte[] hash = null;
			byte[] data = Encoding.UTF8.GetBytes(dataString);
			using(HMACSHA256 hasher = new HMACSHA256(key)) {
				hash = hasher.ComputeHash(data);
			}
			return hash;
		}
		string ConvertByteArrayToHexString(byte[] byteArray) {
			StringBuilder hexBuilder = new StringBuilder(byteArray.Length * 2);
			foreach(byte @byte in byteArray)
				hexBuilder.AppendFormat("{0:x2}", @byte);
			return hexBuilder.ToString();
		}
	}
	public class DropboxHelper : CloudServiceHelperBase {
		const string AppKeyValue = "aspxKeyValue";
		const string AppKeySecret = "aspxKeySecret";
		const string AccessUrl = "https://api.dropbox.com/1/oauth/access_token";
		const string GetUrl = "https://api-content.dropbox.com/1/files/auto";
		const string DeleteUrl = "https://api.dropbox.com/1/fileops/delete";
		const string CopyUrl = "https://api.dropbox.com/1/fileops/copy";
		const string MoveUrl = "https://api.dropbox.com/1/fileops/move";
		const string CreateFolderUrl = "https://api.dropbox.com/1/fileops/create_folder";
		const string DeltaUrl = "https://api.dropbox.com/1/delta";
		const string ThumbnailsUrl = "https://api-content.dropbox.com/1/thumbnails/auto";
		const string Oauth2TokenUrl = "https://api.dropbox.com/1/oauth2/token_from_oauth1";
		const string MediaUrl = "https://api.dropbox.com/1/media/auto";
		const string PutFileUrl = "https://api-content.dropbox.com/1/files_put/auto/";
		const string ChunkedUploadUrl = "https://api-content.dropbox.com/1/chunked_upload";
		const string CommitChunkedUploadUrl = "https://api-content.dropbox.com/1/commit_chunked_upload/auto/";
		byte[] buffer = new byte[8192];
		Consumer consumer;
		Consumer Consumer {
			get {
				if(consumer == null)
					CreateConsumer();
				return consumer;
			}
		}
		IToken AccessToken {
			get { return new Token(AppKeyValue, AppKeySecret, AccessTokenValue, AccessTokenValue); }
		}
		public string AccessTokenValue { get; set; }
		public DropboxHelper() { }
		public DropboxHelper(string accessTokenValue) {
			AccessTokenValue = accessTokenValue;
		}
		protected override HttpWebRequest CreateRequest(string url, string httpMethod) {
			return Consumer.CreateHttpRequest(new Uri(url), httpMethod, null, Consumer.AccessToken, Consumer.Signature, "2.0");
		}
		protected override string GetRequestUrlCore(RequestInfo requestInfo) {
			return requestInfo.ObjectName;
		}
		protected override void PrepareRequestHeaders(HttpWebRequest request, RequestInfo requestInfo) { }
		protected override int GetRequestContentLength(RequestInfo requestInfo) {
			return (int)requestInfo.Content.Length;
		}
		public void CreateFolder(string path) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "root", "dropbox" },
				{ "path", path }
			};
			RequestInfo requestInfo = new RequestInfo(CreateFolderUrl, "POST", parameters);
			ExecuteRequest(requestInfo);
		}
		public void CopyNode(string path, string targetPath) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "root", "dropbox" },
				{ "from_path", path },
				{ "to_path", targetPath }
			};
			RequestInfo requestInfo = new RequestInfo(CopyUrl, "POST", parameters);
			ExecuteRequest(requestInfo);
		}
		public void DeleteNode(string path) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "root", "dropbox" },
				{ "path", path }
			};
			RequestInfo requestInfo = new RequestInfo(DeleteUrl, "POST", parameters);
			ExecuteRequest(requestInfo);
		}
		public void MoveNode(string path, string targetPath) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "root", "dropbox" },
				{ "from_path", path },
				{ "to_path", targetPath }
			};
			RequestInfo requestInfo = new RequestInfo(MoveUrl, "POST", parameters);
			ExecuteRequest(requestInfo);
		}
		public void PutObject(string filePath, Stream content, string contentType = null) {
			RequestInfo requestInfo = new RequestInfo(PutFileUrl + filePath, "POST", null, content);
			ExecuteRequest(requestInfo);
		}
		public string UploadPart(Stream chunk, string upload_id, long offset) {
			var chunkedInfo = new DropboxChunkedUploadInfo();
			var serializer = new JavaScriptSerializer();
			NameValueCollection parameters = new NameValueCollection() {
				{ "offset", offset.ToString() }
			};
			if(!string.IsNullOrEmpty(upload_id))
				parameters.Add("upload_id", upload_id);
			RequestInfo requestInfo = new RequestInfo(ChunkedUploadUrl, "POST", parameters, chunk);
			string response = ExecuteRequest(requestInfo);
			chunkedInfo = serializer.Deserialize<DropboxChunkedUploadInfo>(response);
			chunk.SetLength(0);
			return upload_id ?? chunkedInfo.upload_id;
		}
		public void CompleteMultipartUpload(string filePath, string upload_id) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "upload_id", upload_id }
			};
			RequestInfo requestInfo = new RequestInfo(CommitChunkedUploadUrl + filePath, "POST", parameters);
			ExecuteRequest(requestInfo);
		}
		public void AbortMultipartUpload() { }
		public string GetDelta(string rootFolder, string cursor) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "path_prefix", rootFolder }
			};
			if(!string.IsNullOrEmpty(cursor))
				parameters.Add("cursor", cursor);
			RequestInfo requestInfo = new RequestInfo(DeltaUrl, "POST", parameters);
			return ExecuteRequest(requestInfo);
		}
		public string GetFileLink(string filePath) {
			RequestInfo requestInfo = new RequestInfo(MediaUrl + filePath, "POST");
			return ExecuteRequest(requestInfo);
		}
		public HttpWebRequest GetFileThumbnail(string filePath) {
			NameValueCollection parameters = new NameValueCollection() {
				{ "size", "m" }
			};
			RequestInfo requestInfo = new RequestInfo(ThumbnailsUrl + filePath, "GET", parameters);
			return GetRequest(requestInfo);
		}
		public HttpWebRequest GetFileResponse(string filePath) {
			RequestInfo requestInfo = new RequestInfo(GetUrl + filePath, "GET");
			return GetRequest(requestInfo);
		}
		void CreateConsumer() {
			consumer = new Consumer();
			consumer.AccessToken = AccessToken;
			consumer.AccessUri = new Uri(AccessUrl);
			consumer.Signature = Signature.HMACSHA1;
		}
	}
	public class CloudUtils {
		public static string UrlEncode(string value) {
			return UrlEncode(value, false);
		}
		public static string UrlEncode(string value, bool encodeSlash) {
			string safeCharacters = "_-~.";
			StringBuilder sb = new StringBuilder();
			byte[] bytes = UTF8Encoding.UTF8.GetBytes(value);
			foreach(byte b in bytes) {
				char ch = (char)b;
				if((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z') || (ch >= '0' && ch <= '9') || safeCharacters.IndexOf(ch) > -1)
					sb.Append(ch);
				else if(ch == '/')
					sb.Append(encodeSlash ? "%2F" : ch.ToString());
				else {
					byte @byte = Convert.ToByte(ch);
					sb.Append("%" + @byte.ToString("x2").ToUpper());
				}
			}
			return sb.ToString();
		}
	}
}

#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ImageProcessorsHelper {
		private static BinaryDataRequestProcessorFromDatabase binaryDataProcessorFromDatabase = new BinaryDataRequestProcessorFromDatabase();
		private static Dictionary<IObjectSpace, ImageProcessorsHelper> processedObjectSpaces = new Dictionary<IObjectSpace, ImageProcessorsHelper>();
		private Dictionary<object, BinaryDataRequestProcessorFromCurrentObject> imageProcessors = new Dictionary<object, BinaryDataRequestProcessorFromCurrentObject>();
		private IObjectSpace objectSpace;
		private string id = Guid.NewGuid().ToString();
		private ImageProcessorsHelper(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
			objectSpace.Disposed += RemoveImageProsessor;
		}
		public static BinaryDataRequestProcessorFromDatabase BinaryDataProcessorFromDatabase {
			get {
				return binaryDataProcessorFromDatabase;
			}
		}
		public static byte[] GetBinaryDataByUrl(string imageUrl, out string contentType) {
			byte[] imageBytes = null;
			contentType = string.Empty;
			NameValueCollection urlParameters = HttpUtility.ParseQueryString(imageUrl);
			IBinaryDataRequestProcessor binaryDataProcessor = BinaryDataProcessorsCashe.GetBinaryDataProcessor(urlParameters[BinaryDataRequestProcessorFromDatabase.BinaryDataProcessorKey]);
			if(binaryDataProcessor != null) {
				imageBytes = binaryDataProcessor.GetBinaryDataByParameters(urlParameters, out contentType);
			}
			return imageBytes;
		}
		public static void RegisterBinaryDataProcessor(IBinaryDataRequestProcessor processor) {
			BinaryDataProcessorsCashe.AddBinaryDataProcessor(processor.Id, processor);
		}
		public static void RemoveBinaryDataProcessor(IBinaryDataRequestProcessor processor) {
			BinaryDataProcessorsCashe.RemoveBinaryDataProcessor(processor.Id);
		}
		public static string GetImageUrl(IObjectSpace objectSpace, object currentObject, string propertyName) {
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(currentObject.GetType());
			IMemberInfo memberInfo = typeInfo.FindMember(propertyName);
			if(memberInfo != null) {
				return GetImageUrl(objectSpace, currentObject, propertyName, memberInfo);
			}
			return null;
		}
		public static string GetImageUrl(IObjectSpace objectSpace, object currentObject, string propertyName, IMemberInfo memberInfo) {
			NameValueCollection parameters;
			if(!IsMemberModified(currentObject, propertyName, objectSpace) && memberInfo.IsDelayed) {
				BinaryDataRequestProcessorFromDatabase binaryDataProcessor = BinaryDataProcessorFromDatabase;
				parameters = binaryDataProcessor.GetParameters(objectSpace, currentObject, propertyName, memberInfo);
			}
			else {
				ImageProcessorsHelper imageProcessor = GetImageProsessor(objectSpace);
				BinaryDataRequestProcessorFromCurrentObject binaryDataProcessor = imageProcessor.GetBinaryDataProcessor(currentObject);
				parameters = binaryDataProcessor.GetParameters(propertyName, memberInfo);
			}
			return UrlHelper.BuildQueryString(parameters);
		}
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static bool IsMemberModified(object currentObject, string propertyName, IObjectSpace objectSpace) {
			return objectSpace.ModifiedObjects.Contains(currentObject);
		}
		private static ImageProcessorsHelper GetImageProsessor(IObjectSpace objectSpace) {
			ImageProcessorsHelper result;
			if(!processedObjectSpaces.TryGetValue(objectSpace, out result)) {
				result = new ImageProcessorsHelper(objectSpace);
				processedObjectSpaces[objectSpace] = result;
			}
			return result;
		}
		private void RemoveImageProsessor(object sender, EventArgs e) {
			objectSpace.Disposed -= RemoveImageProsessor;
			processedObjectSpaces.Remove(objectSpace);
			CleareCashe();
			objectSpace = null;
		}
		private BinaryDataRequestProcessorFromCurrentObject GetBinaryDataProcessor(object currentObject) {
			BinaryDataRequestProcessorFromCurrentObject result;
			if(!imageProcessors.TryGetValue(currentObject, out result)) {
				result = new BinaryDataRequestProcessorFromCurrentObject(objectSpace, currentObject);
				BinaryDataProcessorsCashe.AddBinaryDataProcessor(id, result);
				imageProcessors.Add(currentObject, result);
			}
			return result;
		}
		private void CleareCashe() {
			BinaryDataProcessorsCashe.RemoveBinaryDataProcessor(id);
			imageProcessors.Clear();
		}
		private static class BinaryDataProcessorsCashe {
			public static Dictionary<string, Dictionary<string, IBinaryDataRequestProcessor>> BinaryDataProcessors {
				get {
					IValueManager<Dictionary<string, Dictionary<string, IBinaryDataRequestProcessor>>> valueManager = ValueManager.GetValueManager<Dictionary<string, Dictionary<string, IBinaryDataRequestProcessor>>>("BinaryDataHttpHanlder_BinaryDataProcessors");
					Dictionary<string, Dictionary<string, IBinaryDataRequestProcessor>> binaryDataProcessors = valueManager.Value;
					if(binaryDataProcessors == null) {
						lock(valueManager) {
							if(binaryDataProcessors == null) {
								binaryDataProcessors = new Dictionary<string, Dictionary<string, IBinaryDataRequestProcessor>>();
								valueManager.Value = binaryDataProcessors;
							}
						}
					}
					return valueManager.Value;
				}
			}
			public static void AddBinaryDataProcessor(string ownerId, IBinaryDataRequestProcessor binaryDataProcessor) {
				string binaryDataProcessorId = binaryDataProcessor.Id;
				if(!BinaryDataProcessors.ContainsKey(binaryDataProcessorId)) {
					Dictionary<string, IBinaryDataRequestProcessor> binaryData = new Dictionary<string, IBinaryDataRequestProcessor>();
					binaryData.Add(ownerId, binaryDataProcessor);
					BinaryDataProcessors.Add(binaryDataProcessorId, binaryData);
				}
				else {
					Dictionary<string, IBinaryDataRequestProcessor> binaryData = BinaryDataProcessors[binaryDataProcessorId];
					if(!binaryData.ContainsKey(ownerId)) {
						binaryData.Add(ownerId, binaryDataProcessor);
					}
				}
			}
			public static void RemoveBinaryDataProcessor(string imageProcessorsHelperId) {
				List<string> itemKeysToRemove = new List<string>();
				foreach(KeyValuePair<string, Dictionary<string, IBinaryDataRequestProcessor>> binaryData in BinaryDataProcessors) {
					binaryData.Value.Remove(imageProcessorsHelperId);
					if(binaryData.Value.Count == 0) {
						itemKeysToRemove.Add(binaryData.Key);
					}
				}
				foreach(string key in itemKeysToRemove) {
					BinaryDataProcessors.Remove(key);
				}
			}
			public static IBinaryDataRequestProcessor GetBinaryDataProcessor(string binaryDataProcessorId) {
				if(!string.IsNullOrEmpty(binaryDataProcessorId)) {
					if(BinaryDataRequestProcessorFromDatabase.BinaryDataRequestProcessorFromDatabaseId == binaryDataProcessorId) {
						return ImageProcessorsHelper.BinaryDataProcessorFromDatabase;
					}
					else {
						Dictionary<string, IBinaryDataRequestProcessor> binaryData;
						if(BinaryDataProcessors.TryGetValue(binaryDataProcessorId, out binaryData)) {
							foreach(IBinaryDataRequestProcessor binaryDataProcessor in binaryData.Values) {
								return binaryDataProcessor;
							}
						}
					}
				}
				return null;
			}
		}
#if DebugTest
		public static Dictionary<string, Dictionary<string, IBinaryDataRequestProcessor>> DebugTest_BinaryDataProcessors { get { return BinaryDataProcessorsCashe.BinaryDataProcessors; } }
		public static void AddBinaryDataProcessorForTests(string imageProcessorsHelperId, IBinaryDataRequestProcessor binaryDataProcessor) {
			BinaryDataProcessorsCashe.AddBinaryDataProcessor(imageProcessorsHelperId, binaryDataProcessor);
		}
		public static void DebugTest_RemoveBinaryDataProcessor(string imageProcessorsHelperId) {
			BinaryDataProcessorsCashe.RemoveBinaryDataProcessor(imageProcessorsHelperId);
		}
#endif
	}
}

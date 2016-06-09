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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp {
	public enum ModelDifferenceCopyBehavior {
		Merge,
		Overwrite
	}
	public interface IModelDifference {
		String UserId { get; set; }
		String UserName { get; }
		String ContextId { get; set; }
		Int32 Version { get; set; }
		IList<IModelDifferenceAspect> Aspects { get; }
	}
	public interface IModelDifferenceAspect {
		String Name { get; set; }
		String Xml { get; set; }
		IModelDifference Owner { get; set; }
	}
	public class ModelDifferenceDbStore : ModelDifferenceStore {
		public const String UserIdPropertyName = "UserId";
		public const String ContextIdPropertyName = "ContextId";
		public const String UserNamePropertyName = "UserName";
		public const String XafmlHeader = @"<?xml version=""1.0"" encoding=""utf-8""?>";
		public const String XafmlRootElement = "<Application/>";
		public static String EmptyXafml = String.Format("{0}{1}{2}", XafmlHeader, Environment.NewLine, XafmlRootElement);
		private XafApplication application;
		private Type modelDifferenceType;
		private String contextId;
		private Boolean isSharedModelDifference;
		private IModelDifference FindModelDifference(IObjectSpace objectSpace) {
			String userId = "";
			if(!isSharedModelDifference) {
				userId = UserIdTypeConverter.ConvertToInvariantString(GetCurrentUserId());
			}
			return FindModelDifference(objectSpace, modelDifferenceType, userId, contextId);
		}
		protected virtual Object GetCurrentUserId() {
			return SecuritySystem.CurrentUserId;
		}
		public ModelDifferenceDbStore(XafApplication application, Type modelDifferenceType, Boolean isSharedModelDifference, String contextId) {
			this.application = application;
			this.modelDifferenceType = modelDifferenceType;
			this.isSharedModelDifference = isSharedModelDifference;
			this.contextId = contextId;
			if(!typeof(IModelDifference).IsAssignableFrom(modelDifferenceType)) {
				ReflectionHelper.ThrowInvalidCastException(typeof(IModelDifference), modelDifferenceType);
			}
		}
		public ModelDifferenceDbStore(XafApplication application, Type modelDifferenceType, Boolean isSharedModelDifference)
			: this(application, modelDifferenceType, isSharedModelDifference, null) {
		}
		public override String Name {
			get { return GetType().Name; }
		}
		public override void Load(ModelApplicationBase model) {
			if(UserTypeInfo != null) {
				try {
					using(IObjectSpace objectSpace = application.CreateObjectSpace(modelDifferenceType)) {
						IModelDifference modelDifference = FindModelDifference(objectSpace);
						if((modelDifference == null) && isSharedModelDifference) {
							modelDifference = (IModelDifference)objectSpace.CreateObject(modelDifferenceType);
							modelDifference.UserId = "";
							modelDifference.ContextId = String.IsNullOrWhiteSpace(contextId) ? "" : contextId;
							ModelDifferenceDbStore.ImportSharedModelDifference(application, objectSpace, modelDifference);
							objectSpace.CommitChanges();
						}
						if(modelDifference != null) {
							model.Version = modelDifference.Version;
							ModelXmlReader reader = new ModelXmlReader();
							foreach(IModelDifferenceAspect aspect in modelDifference.Aspects) {
								if(!String.IsNullOrWhiteSpace(aspect.Xml)) {
									String aspectName = (aspect.Name != null) ? aspect.Name : "";
									reader.ReadFromString(model, aspectName, aspect.Xml);
								}
							}
						}
					}
				}
				catch(Exception e) {
					if(!(e is InvalidOperationException)) {
						Tracing.Tracer.LogError(e);
						throw new UserFriendlyException(UserVisibleExceptions.CannotLoadUserSettingsFromTheDatabase + UserVisibleExceptions.UserSettingsFailureSuggestion + Environment.NewLine + e.Message);
					}
				}
			}
		}
		public override void SaveDifference(ModelApplicationBase model) {
			if(UserTypeInfo != null) {
				try {
					using(IObjectSpace objectSpace = application.CreateObjectSpace(modelDifferenceType)) {
						IModelDifference modelDifference = FindModelDifference(objectSpace);
						if(
							((modelDifference == null) || (modelDifference.Version <= model.Version))
							&&
							(isSharedModelDifference || (GetCurrentUserId() != null))
						) {
							if(modelDifference == null) {
								modelDifference = (IModelDifference)objectSpace.CreateObject(modelDifferenceType);
								if(isSharedModelDifference) {
									modelDifference.UserId = "";
								}
								else {
									modelDifference.UserId = UserIdTypeConverter.ConvertToInvariantString(GetCurrentUserId());
								}
								modelDifference.ContextId = String.IsNullOrWhiteSpace(contextId) ? "" : contextId;
							}
							Type modelDifferenceAspectType = ModelDifferenceDbStore.GetModelDifferenceAspectType(objectSpace.TypesInfo, modelDifferenceType);
							for(Int32 i = 0; i < model.AspectCount; i++) {
								ModelXmlWriter writer = new ModelXmlWriter();
								String xml = writer.WriteToString(model, i);
								if(!String.IsNullOrEmpty(xml)) {
									String aspectName = model.GetAspect(i);
									IModelDifferenceAspect modelDifferenceAspect = ModelDifferenceDbStore.FindModelDifferenceAspect(modelDifference, aspectName);
									if(modelDifferenceAspect == null) {
										modelDifferenceAspect = (IModelDifferenceAspect)objectSpace.CreateObject(modelDifferenceAspectType);
										modelDifferenceAspect.Name = aspectName;
										modelDifferenceAspect.Owner = modelDifference;
									}
									modelDifferenceAspect.Xml = String.Format("{0}{1}{2}", XafmlHeader, Environment.NewLine, xml);
								}
							}
							objectSpace.CommitChanges();
						}
					}
				}
				catch(Exception e) {
					if(!(e is InvalidOperationException)) {
						Tracing.Tracer.LogError(e);
						throw new UserFriendlyException(UserVisibleExceptions.CannotSaveUserSettingsToTheDatabase + UserVisibleExceptions.UserSettingsFailureSuggestion + Environment.NewLine + e.Message);
					}
				}
			}
		}
		private static ITypeInfo userTypeInfo;
		private static TypeConverter userIdTypeConverter;
		private static String sharedModelDifferenceName;
		public static ITypeInfo UserTypeInfo {
			get {
				if(userTypeInfo == null) {
					userTypeInfo = XafTypesInfo.Instance.FindTypeInfo(SecuritySystem.UserType);
				}
				return userTypeInfo;
			}
			set {
				userTypeInfo = value;
			}
		}
		public static TypeConverter UserIdTypeConverter {
			get {
				if(userIdTypeConverter == null) {
					userIdTypeConverter = TypeDescriptor.GetConverter(UserTypeInfo.KeyMember.MemberType);
				}
				return userIdTypeConverter;
			}
		}
		public static String SharedModelDifferenceName {
			get {
				if(String.IsNullOrWhiteSpace(sharedModelDifferenceName)) {
					sharedModelDifferenceName = CaptionHelper.GetLocalizedText("Texts", "SharedModelDifferenceName");
				}
				return sharedModelDifferenceName;
			}
			set {
				sharedModelDifferenceName = value;
			}
		}
		public static Type GetModelDifferenceAspectType(ITypesInfo typesInfo, Type modelDifferenceType) {
			return typesInfo.FindTypeInfo(modelDifferenceType).FindMember("Aspects").ListElementType;
		}
		public static IModelDifference FindModelDifference(IObjectSpace objectSpace, Type modelDifferenceType, String userId, String contextId) {
			GroupOperator criteria = new GroupOperator(GroupOperatorType.And);
			if(String.IsNullOrWhiteSpace(userId)) {
				criteria.Operands.Add(
					new GroupOperator(GroupOperatorType.Or, new NullOperator(UserIdPropertyName), new BinaryOperator(UserIdPropertyName, "")));
			}
			else {
				criteria.Operands.Add(new BinaryOperator(UserIdPropertyName, userId));
			}
			if(String.IsNullOrWhiteSpace(contextId)) {
				criteria.Operands.Add(
					new GroupOperator(GroupOperatorType.Or, new NullOperator(ContextIdPropertyName), new BinaryOperator(ContextIdPropertyName, "")));
			}
			else {
				criteria.Operands.Add(new BinaryOperator(ContextIdPropertyName, contextId));
			}
			return (IModelDifference)objectSpace.FindObject(modelDifferenceType, criteria);
		}
		public static IModelDifferenceAspect FindModelDifferenceAspect(IModelDifference modelDifference, String aspectName) {
			IModelDifferenceAspect result = null;
			foreach(IModelDifferenceAspect modelDifferenceAspect in modelDifference.Aspects) {
				if(String.IsNullOrEmpty(aspectName)) {
					if((modelDifferenceAspect.Name == null) || (modelDifferenceAspect.Name == "")) {
						result = modelDifferenceAspect;
						break;
					}
				}
				else {
					if(modelDifferenceAspect.Name == aspectName) {
						result = modelDifferenceAspect;
						break;
					}
				}
			}
			return result;
		}
		public static void ImportSharedModelDifference(XafApplication application, IObjectSpace objectSpace, IModelDifference modelDifference) {
			String applicationFolder = PathHelper.GetApplicationFolder();
			FileModelStore fileModelStore = new FileModelStore(applicationFolder, application.GetDiffDefaultName(applicationFolder));
			IEnumerable<String> aspects = fileModelStore.GetAspects().Concat(new String[] { "" });
			Type modelDifferenceAspectType = ModelDifferenceDbStore.GetModelDifferenceAspectType(objectSpace.TypesInfo, modelDifference.GetType());
			foreach(String aspectName in aspects) {
				try {
					IModelDifferenceAspect modelDifferenceAspect = ModelDifferenceDbStore.FindModelDifferenceAspect(modelDifference, aspectName);
					if(modelDifferenceAspect == null) {
						modelDifferenceAspect = (IModelDifferenceAspect)objectSpace.CreateObject(modelDifferenceAspectType);
						modelDifferenceAspect.Name = aspectName;
						modelDifferenceAspect.Owner = modelDifference;
					}
					String aspectFileName = Path.Combine(applicationFolder, fileModelStore.GetFileNameForAspect(aspectName));
					if(File.Exists(aspectFileName)) {
						using(Stream stream = File.Open(aspectFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
							ModelXmlReader reader = new ModelXmlReader();
							Encoding aspectFileEncoding = reader.GetStreamEncoding(stream);
							if(aspectFileEncoding == null) {
								aspectFileEncoding = Encoding.UTF8;
							}
							using(StreamReader streamReader = new StreamReader(stream, aspectFileEncoding)) {
								modelDifferenceAspect.Xml = streamReader.ReadToEnd();
							}
						}
					}
				}
				catch(Exception e) {
					Tracing.Tracer.LogError(e);
					throw new UserFriendlyException(UserVisibleExceptions.CannotLoadUserSettingsFromFile + UserVisibleExceptions.UserSettingsFailureSuggestion + Environment.NewLine + e.Message);
				}
			}
		}
		#region Obsolete 14.2
		[Obsolete("Use the 'ModelDifferenceDbStore(XafApplication application, Type modelDifferenceType, Boolean isSharedModelDifference)' constructor instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ModelDifferenceDbStore(XafApplication application, Boolean isSharedModelDifference)
			: this(application, ModelDifferenceDbStore.ModelDifferenceType, isSharedModelDifference) {
		}
		[Obsolete("Use the 'ModelDifferenceDbStore(XafApplication application, Type modelDifferenceType, Boolean isSharedModelDifference)' constructor instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static Type ModelDifferenceType { get; set; }
		#endregion
	}
}

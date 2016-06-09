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
using System.Text;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Localization {
	public enum ExceptionId {
		CannotAddCollectionProperty = 1000,
		CannotAddDuplicateProperty = 1001,
		CurrentObjectIsNotSet = 1002,
		ExceptionDialogLabel = 1003,
		IncorrectConfigFile = 1004,
		TypeDoesNotContainTheMember = 1005,
		UnableToSetReadOnlyProperty = 1006,
		UnableToExecuteDisabledOrInactiveAction = 1007,
		TheTypeIsNotDescendantOf = 1008,
		ErrorOccursWhileAddingTheCustomProperty = 1009,
		ModulesHasAlreadyBeenLoaded = 1010,
		TheAssemblyDoesntReferAssembly = 1011,
		TheExpressionStringContainsANumberOfExpressions = 1012,
		SourceNodeExpressionShouldStartWith = 1013,
		SourceAttributePathShouldConsistOf = 1014,
		AttributeShouldStartWithSymbol = 1015,
		ItsImpossibleToFindPathForNode = 1016,
		ItsImpossibleToFindSourceNode = 1017,
		SourceAttributeExpressionShouldStartWith = 1019,
		TheExpressionShouldConsistOf = 1020,
		PassedObjectBelongsToAnotherObjectSpace = 1021,
		ResourceItemWithNameIsNotFound = 1022,
		YouCannotChangeCurrentAspect = 1023,
		TheAspectIsNotPresentInAspectsCollection = 1024,
		UnableToChangeValueOfAttributeDueItIsLocked = 1025,
		AnErrorHasOccurredWhileLoadingTheModel = 1026,
		CannotReadDictionaryFromResource = 1027,
		UnableToChangeChildrenOfNodeDueItIsLocked = 1028,
		CannotAddToEmptySingleton = 1029,
		DictionaryPathItemParsingFailed = 1030,
		DictionaryPathParsingFailed = 1031,
		CannotCreateAppropriateDetailItem = 1032,
		CannotCreateAppropriatePropertyEditorForTheProperty = 1033,
		TheClassNameAttributeIsEmpty = 1034,
		TheClassNameAttributeRefersToADifferentType = 1035,
		UnknownViewNodeName = 1036,
		AnErrorHasOccurredWhileProcessingAShortcut = 1037,
		TheCreateApplicationModulesManagerReturnsNull = 1038,
		CannotCreateTemplateByName = 1039,
		UnableToFindAListViewForType = 1040,
		UnableToFindADetailViewForType = 1041,
		NodeWasNotFound = 1042,
		UnableToChangeConnectionString = 1043,
		CannotFindThePropertyWithinTheClass = 1044,
		CannotGenerateContent = 1045,
		TheTypeAttributeCannotBeEmpty = 1046,
		AutoCreateOptionIs = 1047,
		UnableToCreateObjectSpace = 1048,
		UnableToCreateUpdateSessionAutoCreateOption = 1049,
		UnableToCreateUpdateSessionDataLayerIsNull = 1050,
		UnableToCreateUpdatingReadonlySessionAutoCreateOption = 1051,
		XPDictionaryIsNull = 1052,
		DataLayerAlreadyExists = 1053,
		CannotCreateSchemaForAttribute = 1054,
		TheValueWasLocked = 1055,
		PermissionIsDenied = 1056,
		AnUnsavedObjectCannotBeShown = 1057,
		InvalidApplicationVersion = 1058,
		AnErrorHasOccurredWithContext = 1059,
		UnknownValue = 1060,
		NameAndValueCollectionsShouldHaveEqualLengths = 1061,
		TypeIsNotAEnum = 1062,
		MethodShouldBeUsed = 1063,
		IsNotValidNamespacePrefixDescription = 1064,
		ListEditorShouldImplement = 1065,
		CannotObtainTheEditingObject = 1066,
		AttributeModifyingFailed = 1067,
		ThePassedFileIsNotADllOrConfig = 1068,
		TheClassIsNotFoundInBOModel = 1069,
		GridControlIsAlreadyCreated = 1070,
		CannotSetEditValueListIsEmpty = 1071,
		CannotSetEditValueAvailableValues = 1072,
		UnableToCast = 1073,
		LayoutControlInvalidCount = 1074,
		CannotShowViewUnknownContext = 1075,
		CannotShowViewUnknownTarget = 1076,
		ProcessWasAborted = 1077,
		PageCanOnlyContainOneInstanceOfControl = 1078,
		UnknownException = 1079,
		CannotProcessTheRequestedUrl = 1080,
		WindowWasNotFound = 1081,
		CannotProcessTheRequestedUrlActionItemWasNotFound = 1082,
		ApplicationIsNotStarted = 1083,
		DuplicateComponentIdentifier = 1084,
		ThereIsAlreadyRegisteredCreatorForFormat = 1085,
		CannotFindListViewWithId = 1086,
		RuleSetClearingIsDenied = 1087,
		ClassDoesntImplementInterface = 1088,
		UnableToFindType = 1089,
		UnableToFindPropertyInType = 1090,
		ValueCannotBeStoredIntoTheProperty = 1091,
		InfiniteRecursionDetected = 1092,
		GridColumnExists = 1093,
		GridColumnDoesNotExist = 1094,
		UnableToInitializeFileDataProperty = 1095,
		AlreadyUsedObjectSpace = 1096,
		ConfigFileDoesNotExists = 1097,
		ConfigFileHasNoModuleNames = 1098,
		IncompatibleParametersCombinationOnViewChanged = 1099,
		UnableToAssignDisabledActionItem = 1100,
		UnableToAssignInvisibleActionItem = 1101,
		ActionMethodShouldBeParameterless = 1102,
		ActionMethodShouldBeNonGeneric = 1103,
		ValidationMessageTemplateHasInvalidFormat = 1104,
		InvalidParametrizedActionValueType = 1105,
		ObjectFormatterFormatStringIsInvalid = 1106,
		RequiresCurrentObjectException = 1107,
		NotRegisteredReadOnlyParameterException = 1108,
		ParameterConflictsWithPropertyException = 1109,
		CompatibilityDatabaseIsOldError = 1110,
		CompatibilityApplicationIsOldError = 1111,
		ApplicationUpdaterAssemblyDoesNotExist = 1112,
		ApplicationUpdaterDatabaseVersionIsNewer = 1113,
		CustomizePopupWindowParamsEventArgsViewIsNull = 1114,
		InvalidValueOfEnumProperty = 1115,
		CannotAccessCollectionCollectionSourceIsBeingIntialized = 1116,
		ValidationCannotFindThePropertyWithinTheClass = 1117,
		TemplateTypeInfoMissed = 1118,
		ValidationCannotFindTheAssociatedMemberInTargetClassForCollectionProperty = 1119,
		CannotFindTheMemberWithinTheClass = 1120,
		InfiniteRecursionDetectedInGenerateColumns = 1121
	};
	public enum UserVisibleExceptionId {
		SystemExceptionMessageFormat = 2000,
		UserFriendlyConnectionFailedException = 2002,
		UserFriendlySqlException = 2003,
		LogonAttemptsAmountedToLimitWin = 2004,
		TheFollowingErrorOccurred = 2005,
		RequestedObjectIsNotFound = 2006,
		RequestedObjectHasBeenDeleted = 2007,
		ObjectAccessPermissionCannotBeEmpty = 2009,
		ObjectToSaveWasChanged = 2010,
		FieldValueSizeExceedsMaxLength = 2011,
		SimultaneousChangeDataMessage = 2014,
		SimultaneousChangeDataMessageItem = 2015,
		MaskValidationErrorMessage = 2016,
		ApplicationUpdateError = 2017,
		CriteriaEditorsDataTypeIsNull = 2018,
		ActionIsNotSingleChoiceAction = 2019,
		CircularReference = 3001
	}
	public interface IExceptionLocalizer : IXafResourceLocalizer {
		string GetExceptionMessage(string messageId, int errorNumber);
		string GetExceptionMessage(string messageId, int errorNumber, params object[] args);
	}
	public abstract class ExceptionLocalizer : XafResourceLocalizer, IExceptionLocalizer {
		private static List<Type> exceptionLocalizers = new List<Type>();
		public static void Register(Type exceptionLocalizerType) {
			lock(exceptionLocalizers) {
				if(exceptionLocalizers.IndexOf(exceptionLocalizerType) == -1) {
					exceptionLocalizers.Add(exceptionLocalizerType);
				}
			}
		}
		public static void ResetRegistration() {
			lock(exceptionLocalizers) {
				exceptionLocalizers.Clear();
			}
		}
		internal static IEnumerable<Type> ExceptionLocalizers {
			get {
				return exceptionLocalizers;
			}
		}
		public abstract string GetExceptionMessage(string messageId, int errorNumber);
		public string GetExceptionMessage(string messageId, int errorNumber, params object[] args) {
			lock (DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
				return string.Format(GetExceptionMessage(messageId, errorNumber), args);
			}
		}
	}
	[DisplayName("System Exceptions")]
	public class SystemExceptionResourceLocalizer : ExceptionLocalizer {
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			string[] path = new string[] { "Exceptions", "SystemExceptions" };
			return new XafResourceManagerParameters(
				null,
				path,
				"DevExpress.ExpressApp.Localization.SystemExceptions",
				"",
				typeof(SystemExceptionLocalizer).Assembly
				);
		}
		public override void Activate() {
			SystemExceptionLocalizer.Instance = this;
		}
		public override string GetExceptionMessage(string messageId, int errorNumber) {
			string localizedMessage = GetLocalizedString(messageId);
			string localizedSystemExceptionMessageFormat = UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.SystemExceptionMessageFormat);
			return string.Format(localizedSystemExceptionMessageFormat, errorNumber, localizedMessage);
		}
	}
	[DisplayName("User Friendly Exceptions")]
	public class UserVisibleExceptionResourceLocalizer : ExceptionLocalizer {
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			string[] path = new string[] { "Exceptions", "UserVisibleExceptions" };
			return new XafResourceManagerParameters(
				null,
				path,
				"DevExpress.ExpressApp.Localization.UserVisibleExceptions",
				"",
				typeof(UserVisibleExceptionLocalizer).Assembly
				);
		}
		public override string GetExceptionMessage(string messageId, int errorNumber) {
			return GetLocalizedString(messageId);
		}
		public override void Activate() {
			UserVisibleExceptionLocalizer.Instance = this;
		}
	}
	public class ExceptionLocalizerTemplate<LocalizerType, ExceptionIdEnumType> where LocalizerType : ExceptionLocalizer, new() where ExceptionIdEnumType : IConvertible {
		private static LocalizerType instance;
		public static LocalizerType Instance {
			get {
				lock(DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
					if(instance == null) {
						instance = new LocalizerType();
					}
				}
				return instance;
			}
			set {
				instance = value;
			}
		}
		protected ExceptionLocalizerTemplate() {
		}
		public static string GetExceptionMessage(string messageId, int errorNumber) {
			lock (DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
				return Instance.GetExceptionMessage(messageId, errorNumber);
			}
		}
		public static string GetExceptionMessage(string messageId, int errorNumber, params object[] args) {
			lock (DevExpress.ExpressApp.DC.TypesInfo.lockObject) {
				return Instance.GetExceptionMessage(messageId, errorNumber, args);
			}
		}
		public static string GetExceptionMessage(ExceptionIdEnumType exceptionId) {
			return GetExceptionMessage(exceptionId.ToString(), exceptionId.ToInt32(System.Globalization.NumberFormatInfo.CurrentInfo));
		}
		public static string GetExceptionMessage(ExceptionIdEnumType exceptionId, params object[] args) {
			return GetExceptionMessage(exceptionId.ToString(), exceptionId.ToInt32(System.Globalization.NumberFormatInfo.CurrentInfo), args);
		}
	}
	public class SystemExceptionLocalizer : ExceptionLocalizerTemplate<SystemExceptionResourceLocalizer, ExceptionId> {
	}
	public class UserVisibleExceptionLocalizer : ExceptionLocalizerTemplate<UserVisibleExceptionResourceLocalizer, UserVisibleExceptionId> {
	}
}

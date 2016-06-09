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

namespace DevExpress.Xpo.Exceptions {
	using System;
	using System.Globalization;
	using System.Runtime.Serialization;
	using System.Collections;
	using DevExpress.Xpo.Helpers;
	using DevExpress.Xpo.Metadata;
	using System.ComponentModel;
	[Serializable]
	public class TypeNotFoundException : Exception {
		int typeId;
#if !CF && !SL
		protected TypeNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public TypeNotFoundException(Int32 typeId)
			: base(Res.GetString(Res.Session_TypeNotFound, typeId)) {
			this.typeId = typeId;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("TypeNotFoundExceptionTypeId")]
#endif
		public Int32 TypeId { get { return typeId; } set { typeId = value; } }
	}
	[Serializable]
	public class TypeFieldIsEmptyException : Exception {
		string baseType;
#if !CF && !SL
		protected TypeFieldIsEmptyException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public TypeFieldIsEmptyException(string baseType)
			: base(Res.GetString(Res.Session_TypeFieldIsEmpty, XPObjectType.ObjectTypePropertyName, baseType)) {
			this.baseType = baseType;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("TypeFieldIsEmptyExceptionBaseType")]
#endif
		public string BaseType { get { return baseType; } }
	}
	[Serializable]
	public class UnableToFillRefTypeException : Exception {
		string objectName;
		string memberName;
#if !CF && !SL
		protected UnableToFillRefTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
#if !SL
	[DevExpressXpoLocalizedDescription("UnableToFillRefTypeExceptionObjectName")]
#endif
		public string ObjectName { get { return objectName; } set { objectName = value; } }
#if !SL
	[DevExpressXpoLocalizedDescription("UnableToFillRefTypeExceptionMemberName")]
#endif
		public string MemberName { get { return memberName; } set { memberName = value; } }
		public UnableToFillRefTypeException(string objectName, string memberName, Exception innerException)
			:
			base(Res.GetString(Res.ConnectionProvider_UnableToFillRefType, objectName, memberName, innerException.Message), innerException) {
			this.objectName = objectName;
			this.memberName = memberName;
		}
	}
	[Serializable]
	public class KeyPropertyAbsentException : Exception {
		string className;
#if !CF && !SL
		protected KeyPropertyAbsentException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public KeyPropertyAbsentException(string className)
			: base(Res.GetString(Res.MetaData_KeyPropertyAbsent, className)) {
			this.className = className;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("KeyPropertyAbsentExceptionClassName")]
#endif
		public string ClassName { get { return className; } }
	}
	[Serializable]
	public class DuplicateKeyPropertyException : Exception {
		string className;
#if !CF && !SL
		protected DuplicateKeyPropertyException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public DuplicateKeyPropertyException(string className)
			: base(Res.GetString(Res.MetaData_DuplicateKeyProperty, className)) {
			this.className = className;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("DuplicateKeyPropertyExceptionClassName")]
#endif
		public string ClassName { get { return className; } }
	}
	[Serializable]
	public class CannotResolveClassInfoException : Exception {
		string typeName;
		string assemblyName;
#if !CF && !SL
		protected CannotResolveClassInfoException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public CannotResolveClassInfoException(string assemblyName, string typeName)
			: base(Res.GetString(Res.MetaData_CannotResolveClassInfo, assemblyName, typeName)) {
			this.typeName = typeName;
			this.assemblyName = assemblyName;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("CannotResolveClassInfoExceptionTypeName")]
#endif
		public string TypeName { get { return typeName; } }
		string AssemblyName { get { return assemblyName; } }
	}
	[Serializable]
	public class XMLDictionaryException : Exception {
#if !CF && !SL
		protected XMLDictionaryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public XMLDictionaryException(string message) : base(Res.GetString(Res.MetaData_XMLLoadError, message)) { }
	}
	[Serializable, Obsolete("Use AssociationInvalidException instead", true)]
	public class PropertyTypeMismatchException : AssociationInvalidException {
#if !CF && !SL
		protected PropertyTypeMismatchException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public PropertyTypeMismatchException(string message)
			: base(message) {
		}
	}
	[Serializable]
	public class AssociationInvalidException : Exception {
		public AssociationInvalidException(string message) : base(message) { }
#if !CF && !SL
		protected AssociationInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
	}
	[Serializable]
	public class PropertyMissingException : Exception {
		string objectType;
		string propertyName;
#if !CF && !SL
		protected PropertyMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public PropertyMissingException(string objectType, string propertyName)
			:
			base(Res.GetString(Res.MetaData_PropertyMissing, propertyName, objectType)) {
			this.objectType = objectType;
			this.propertyName = propertyName;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("PropertyMissingExceptionObjectType")]
#endif
		public string ObjectType { get { return objectType; } }
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("PropertyMissingExceptionPropertyName")]
#endif
		public string PropertyName { get { return propertyName; } }
	}
	[Serializable]
	public class AssociationElementTypeMissingException : Exception {
		string propertyName;
#if !CF && !SL
		protected AssociationElementTypeMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public AssociationElementTypeMissingException(string propertyName)
			: base(Res.GetString(Res.MetaData_AssociationElementTypeMissing, propertyName)) {
			this.propertyName = propertyName;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("AssociationElementTypeMissingExceptionPropertyName")]
#endif
		public string PropertyName { get { return propertyName; } }
	}
	[Serializable]
	public class RequiredAttributeMissingException : Exception {
		string propertyName;
		string attributeName;
#if !CF && !SL
		protected RequiredAttributeMissingException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public RequiredAttributeMissingException(string propertyName, string attributeName)
			:
			base(Res.GetString(Res.MetaData_RequiredAttributeMissing, propertyName, attributeName)) {
			this.propertyName = propertyName;
			this.attributeName = attributeName;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("RequiredAttributeMissingExceptionPropertyName")]
#endif
		public string PropertyName { get { return propertyName; } }
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("RequiredAttributeMissingExceptionAttributeName")]
#endif
		public string AttributeName { get { return attributeName; } }
	}
	[Serializable]
	public class NonPersistentReferenceFoundException : Exception {
		string objectType;
#if !CF && !SL
		protected NonPersistentReferenceFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public NonPersistentReferenceFoundException(string objectType)
			:
			base(Res.GetString(Res.MetaData_NonPersistentReferenceFound, objectType)) {
			this.objectType = objectType;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("NonPersistentReferenceFoundExceptionObjectType")]
#endif
		public string ObjectType { get { return objectType; } }
	}
	[Serializable]
	public class CannotFindAppropriateConnectionProviderException : Exception {
#if !CF && !SL
		protected CannotFindAppropriateConnectionProviderException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public CannotFindAppropriateConnectionProviderException(string connectionString) : base(Res.GetString(Res.Session_WrongConnectionString, connectionString)) { }
	}
	[Serializable]
	public class DifferentObjectsWithSameKeyException : Exception {
#if !CF && !SL
		protected DifferentObjectsWithSameKeyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public DifferentObjectsWithSameKeyException() : base(Res.GetString(Res.Session_DifferentObjectsWithSameKey)) { }
	}
	[Serializable]
	public class CannotChangePropertyWhenSessionIsConnectedException : Exception {
#if !CF && !SL
		protected CannotChangePropertyWhenSessionIsConnectedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public CannotChangePropertyWhenSessionIsConnectedException(string propertyName) : base(Res.GetString(Res.Session_CannotChangePropertyWhenSessionIsConnected, propertyName)) { }
	}
	[Serializable]
	public class CannotLoadObjectsException : Exception {
#if !CF && !SL
		protected CannotLoadObjectsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public CannotLoadObjectsException(string objects) : base(Res.GetString(Res.Session_CannotReloadPurgedObject, objects)) { }
	}
	[Obsolete("Use CannotLoadObjectsException instead", true)]
	public class CannotReloadPurgedObjectException: Exception {
	}
	[Serializable]
	public class TransactionSequenceException : Exception {
#if !CF && !SL
		protected TransactionSequenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public TransactionSequenceException(string explanation) : base(explanation) { }
	}
	[Serializable]
	public class SessionCtorAbsentException : Exception {
		string objectType;
#if !CF && !SL
		protected SessionCtorAbsentException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public SessionCtorAbsentException(XPClassInfo classInfo)
			:
			base(Res.GetString(Res.MetaData_SessionCtorAbsent, classInfo.FullName)) {
			this.objectType = classInfo.FullName;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("SessionCtorAbsentExceptionObjectType")]
#endif
		public string ObjectType { get { return this.objectType; } }
	}
	[Serializable]
	public class SessionMixingException : Exception {
		Session session;
		object obj;
#if !CF && !SL
		protected SessionMixingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public SessionMixingException(Session session, object obj)
			:
			base(Res.GetString(Res.Session_SessionMixing, obj.GetType().FullName)) {
			this.session = session;
			this.obj = obj;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SessionMixingExceptionSession")]
#endif
public Session Session { get { return session; } }
#if !SL
	[DevExpressXpoLocalizedDescription("SessionMixingExceptionObject")]
#endif
public object Object { get { return obj; } }
	}
	[Serializable]
	public class ObjectCacheException : Exception {
		object obj;
#if !CF && !SL
		protected ObjectCacheException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public ObjectCacheException(object id, object theObject, object oldObject)
			:
			base(Res.GetString(Res.Session_CannotAddObjectToObjectCache, theObject, id, oldObject)) {
			this.obj = theObject;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectCacheExceptionObject")]
#endif
public object Object { get { return obj; } }
	}
	[Serializable]
	public class KeysAutogenerationNonSupportedTypeException : Exception {
		string typeName;
#if !CF && !SL
		protected KeysAutogenerationNonSupportedTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public KeysAutogenerationNonSupportedTypeException(string typeName)
			: base(Res.GetString(Res.ConnectionProvider_KeysAutogenerationNonSupportedTypeException, typeName)) {
			this.typeName = typeName;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("KeysAutogenerationNonSupportedTypeExceptionTypeName")]
#endif
		public string TypeName { get { return typeName; } }
	}
	[Serializable]
	public class CannotLoadInvalidTypeException : Exception {
		string typeName;
#if !CF && !SL
		protected CannotLoadInvalidTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public CannotLoadInvalidTypeException(string typeName)
			: base(Res.GetString(Res.Session_CannotLoadInvalidType, typeName)) {
			this.typeName = typeName;
		}
		[Obsolete("Use Message instead.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
#if !SL
	[DevExpressXpoLocalizedDescription("CannotLoadInvalidTypeExceptionTypeName")]
#endif
		public string TypeName { get { return typeName; } }
	}
	[Serializable]
	public class SameTableNameException : Exception {
#if !CF && !SL
		protected SameTableNameException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
		public SameTableNameException(XPClassInfo firstClass, XPClassInfo secondClass) : base(Res.GetString(Res.Metadata_SameTableName, firstClass.FullName, secondClass.FullName)) { }
	}
	[Serializable]
	public class ObjectLayerSecurityException : InvalidOperationException {
		string typeName;
		string propertyName;
		bool isDeletion;
#if !CF && !SL
		protected ObjectLayerSecurityException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
#endif
		public ObjectLayerSecurityException()
			:base(Res.GetString(Res.Security_TheCommitOperationWasProhibitedByTheRules)){
		}
		public ObjectLayerSecurityException(string typeName, bool isDeletion)
			: base(Res.GetString(isDeletion ? Res.Security_DeletingAnObjectWasProhibitedByTheRulesX0 : Res.Security_SavingAnObjectWasProhibitedByTheRulesX0, typeName)) {
			this.typeName = typeName;
			this.isDeletion = isDeletion;
		}
		public ObjectLayerSecurityException(string typeName, string propertyName)
			:base(Res.GetString(Res.Security_SavingThePropertyWasProhibitedByTheRulesX0X1, typeName, propertyName)) {
			this.typeName = typeName;
			this.propertyName = propertyName;
		}
		public string TypeName { get { return typeName; } set { typeName = value; } }
		public string PropertyName { get { return propertyName; } set { propertyName = value; } }
		public bool IsDeletion { get { return isDeletion; } set { isDeletion = value; } }
	}
	[Serializable]
	public class MSSqlLocalDBApiException : InvalidOperationException {
		int? errorCode;
		public int? ErrorCode { get { return errorCode; } set { errorCode = value; } }
		public MSSqlLocalDBApiException() { }
		public MSSqlLocalDBApiException(string message) : base(message) { }
		public MSSqlLocalDBApiException(string message, int errorCode)
			: this(message) {
			this.errorCode = errorCode;
		}
		public MSSqlLocalDBApiException(string message, Exception innerException) : base(message, innerException) { }
#if !CF && !SL
		protected MSSqlLocalDBApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
	}
}

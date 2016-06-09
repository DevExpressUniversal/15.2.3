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
using System.Collections;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Validation {
	public class ContextValidatingEventArgs : EventArgs {
		private ArrayList targetObjects;
		private ContextIdentifier context;
		public ContextValidatingEventArgs(ContextIdentifier context, ArrayList targetObjects) {
			this.targetObjects = targetObjects;
			this.context = context;
		}
		public ArrayList TargetObjects {
			get { return targetObjects; }
		}
		public ContextIdentifier Context {
			get { return context; }
		}
	}
	public class DeleteContextValidatingEventArgs : ContextValidatingEventArgs {
		public DeleteContextValidatingEventArgs(ContextIdentifier context, ArrayList targetObjects, object deletingObject)
			: base(context, targetObjects) {
			this.DeletingObject = deletingObject;
		}
		public object DeletingObject { get; private set; }
	}
	public abstract class ValidationTargetObjectSelector : IDisposable {
		private ICollection FindAggregatedObjects(object owner) {
			Guard.ArgumentNotNull(owner, "owner");
			ArrayList result = new ArrayList();
			DevExpress.ExpressApp.DC.ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(owner.GetType());
			if(typeInfo != null) {
				foreach(DevExpress.ExpressApp.DC.IMemberInfo memberInfo in typeInfo.Members) {
					if(!(memberInfo.IsAggregated && memberInfo.IsVisible)) {
						continue;
					}
					object value = memberInfo.GetValue(owner);
					if(value != null) {
						if(value is ICollection) {
							result.AddRange((ICollection)value);
						}
						else {
							result.Add(value);
						}
					}
				}
			}
			return result;
		}
		private void RegisterObjectToValidate(IObjectSpace objectSpace, ArrayList resultList, object obj) {
			if(resultList.Contains(obj)) {
				return;
			}
			if(IsIntermediateObject(objectSpace, obj)) {
				Object left = null;
				Object right = null;
				GetIntermediateObjectReferences(objectSpace, obj, out left, out right);
				RegisterObjectToValidate(objectSpace, resultList, left);
				RegisterObjectToValidate(objectSpace, resultList, right);
			}
			else {
				NeedToValidateObjectEventArgs args = new NeedToValidateObjectEventArgs(obj);
				args.NeedToValidate = !(obj is XafDataViewRecord) && NeedToValidateObject(objectSpace, obj);
				OnNeedToValidateCurrentObject(args);
				if(args.NeedToValidate) {
					resultList.Add(obj);
				}
			}
		}
		protected Boolean IsIntermediateObject(IObjectSpace objectSpace, Object obj) {
			Boolean result = false;
			if(objectSpace is BaseObjectSpace) {
				result = ((BaseObjectSpace)objectSpace).IsIntermediateObject(obj);
			}
			return result;
		}
		protected void GetIntermediateObjectReferences(IObjectSpace objectSpace, Object obj, out Object left, out Object right) {
			left = null;
			right = null;
			if(objectSpace is BaseObjectSpace) {
				((BaseObjectSpace)objectSpace).GetIntermediateObjectReferences(obj, out left, out right);
			}
		}
		protected abstract bool NeedToValidateObject(IObjectSpace objectSpace, object targetObject);
		protected void OnCustomGetAggregatedObjectsToValidate(CustomGetAggregatedObjectsToValidateEventArgs args) {
			if(CustomGetAggregatedObjectsToValidate != null) {
				CustomGetAggregatedObjectsToValidate(this, args);
			}
		}
		protected void OnNeedToValidateCurrentObject(NeedToValidateObjectEventArgs args) {
			if(CustomNeedToValidateObject != null) {
				CustomNeedToValidateObject(this, args);
			}
		}
		public ArrayList GetObjectsToValidate(IObjectSpace objectSpace, object currentObject) {
			ArrayList result = new ArrayList();
			ArrayList modifiedObjects = new ArrayList();
			ArrayList aggregatedObjects = new ArrayList();
			if(currentObject != null) {
				modifiedObjects.Add(currentObject);
				CustomGetAggregatedObjectsToValidateEventArgs args = new CustomGetAggregatedObjectsToValidateEventArgs(currentObject);
				OnCustomGetAggregatedObjectsToValidate(args);
				if(args.Handled) {
					aggregatedObjects.AddRange(args.AggregatedObjects);
				}
				else {
					aggregatedObjects.AddRange(FindAggregatedObjects(currentObject));
				}
				modifiedObjects.AddRange(aggregatedObjects);
			}
			modifiedObjects.AddRange(GetObjectsToDelete(objectSpace, currentObject));
			modifiedObjects.AddRange(GetObjectsToSave(objectSpace, currentObject));
			foreach(object obj in modifiedObjects) {
				RegisterObjectToValidate(objectSpace, result, obj);
			}
			if((objectSpace is INestedObjectSpace) && (currentObject != null)) {
				bool resultContainsCurrentObject = result.Contains(currentObject);
				ArrayList unfilteredResult = new ArrayList(result);
				result.Clear();
				if(resultContainsCurrentObject) {
					result.Add(currentObject);
					foreach(object aggregatedObject in aggregatedObjects) {
						if(unfilteredResult.Contains(aggregatedObject)) {
							result.Add(aggregatedObject);
						}
					}
				}
			}
			return result;
		}
		protected virtual ICollection GetObjectsToSave(IObjectSpace objectSpace, object currentObject) {
			return objectSpace.GetObjectsToSave(false);
		}
		protected virtual ICollection GetObjectsToDelete(IObjectSpace objectSpace, object currentObject) {
			return objectSpace.GetObjectsToDelete(false);
		}
		public void Dispose() {
			CustomGetAggregatedObjectsToValidate = null;
			CustomNeedToValidateObject = null;
		}
		public event EventHandler<CustomGetAggregatedObjectsToValidateEventArgs> CustomGetAggregatedObjectsToValidate;
		public event EventHandler<NeedToValidateObjectEventArgs> CustomNeedToValidateObject;
	}
	public class SaveContextTargetObjectSelector : ValidationTargetObjectSelector {
		protected override bool NeedToValidateObject(IObjectSpace objectSpace, object targetObject) {
			return !objectSpace.IsDeletedObject(targetObject) && !objectSpace.IsObjectToDelete(targetObject);
		}
	}
	public class SaveContextTargetObjectSelectorModifiedOnly : SaveContextTargetObjectSelector {
		protected override bool NeedToValidateObject(IObjectSpace objectSpace, object targetObject) {
			if(!base.NeedToValidateObject(objectSpace, targetObject)) {
				return false;
			}
			if(objectSpace.IsObjectToSave(targetObject) || objectSpace.IsNewObject(targetObject)) {
				return true;
			}
			foreach(Object obj in objectSpace.GetObjectsToSave(false)) {
				if(IsIntermediateObject(objectSpace, obj)) {
					Object left = null;
					Object right = null;
					GetIntermediateObjectReferences(objectSpace, obj, out left, out right);
					if((left == targetObject) || (right == targetObject)) {
						return true;
					}
				}
			}
			ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objectSpace.GetObjectType(targetObject));
			foreach(IMemberInfo aggregatedMemberInfo in typeInfo.Members.Where((m) => m.IsAggregated && !m.IsList)) {
				object aggregatedObject = aggregatedMemberInfo.GetValue(targetObject);
				if(aggregatedObject != null && objectSpace.IsObjectToSave(aggregatedObject)) {
					return true;
				}
			}
			return false;
		}
	}
	public class DeleteContextTargetObjectSelector : ValidationTargetObjectSelector {
		protected override bool NeedToValidateObject(IObjectSpace objectSpace, object targetObject) {
			return objectSpace.IsDeletedObject(targetObject) || objectSpace.IsObjectToDelete(targetObject);
		}
	}
	public class ProjectedDeleteContextTargetObjectSelector : ValidationTargetObjectSelector {
		protected override bool NeedToValidateObject(IObjectSpace objectSpace, object targetObject) {
			return true;
		}
		protected override ICollection GetObjectsToSave(IObjectSpace objectSpace, object currentObject) {
			return new object[0];
		}
		protected override ICollection GetObjectsToDelete(IObjectSpace objectSpace, object currentObject) {
			return new object[0];
		}
	}
	public class AllContextsTargetObjectSelector : ValidationTargetObjectSelector {
		protected override bool NeedToValidateObject(IObjectSpace objectSpace, object targetObject) {
			return !objectSpace.IsDeletedObject(targetObject) && !objectSpace.IsObjectToDelete(targetObject);
		}
	}
	public class NeedToValidateObjectEventArgs : EventArgs {
		private object currentObject;
		private bool needToValidate = true;
		public NeedToValidateObjectEventArgs(object currentObject) {
			this.currentObject = currentObject;
		}
		public object CurrentObject {
			get { return currentObject; }
		}
		public bool NeedToValidate {
			get { return needToValidate; }
			set { needToValidate = value; }
		}
	}
	public class CustomGetAggregatedObjectsToValidateEventArgs : HandledEventArgs {
		private object ownerObject;
		private ArrayList aggregatedObjects = new ArrayList();
		public CustomGetAggregatedObjectsToValidateEventArgs(object ownerObject) {
			this.ownerObject = ownerObject;
		}
		public object OwnerObject {
			get { return ownerObject; }
		}
		public ArrayList AggregatedObjects {
			get { return aggregatedObjects; }
		}
	}
	public class PersistenceValidationController : ViewController {
		private ValidationTargetObjectSelector CreateSaveContextTargetObjectSelector() {
			if(View is ListView) {
				return new SaveContextTargetObjectSelectorModifiedOnly();
			}
			return new SaveContextTargetObjectSelector();
		}
		private void SubscribeSelectorEvents(ValidationTargetObjectSelector selector) {
			selector.CustomNeedToValidateObject += new EventHandler<NeedToValidateObjectEventArgs>(selector_SelectorNeedToValidateObject);
			selector.CustomGetAggregatedObjectsToValidate += new EventHandler<CustomGetAggregatedObjectsToValidateEventArgs>(selector_SelectorCustomGetAggregatedObjectsToValidate);
		}
		private void CustomizeSaveValidationException(ValidationCompletedEventArgs args) {
			args.Exception.MessageHeader = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.SaveErrorMessageHeader);
			args.Exception.ObjectHeaderFormat = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.SaveErrorMessageObjectFormat);
		}
		private void CustomizeDeleteValidationException(ValidationCompletedEventArgs args) {
			args.Exception.MessageHeader = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.DeleteErrorMessageHeader);
			args.Exception.ObjectHeaderFormat = ValidationExceptionLocalizer.GetExceptionMessage(ValidationExceptionId.DeleteErrorMessageObjectFormat);
		}
		ArrayList preValidatedDeletedObjects;
		private void ObjectSpace_CustomDeleteObjects(object sender, CustomDeleteObjectsEventArgs e) {
			preValidatedDeletedObjects = null;
			ArrayList targetObjects = new ArrayList();
			foreach(object deletingObject in e.Objects) {
				using(ProjectedDeleteContextTargetObjectSelector deleteSelector = new ProjectedDeleteContextTargetObjectSelector()) {
					SubscribeSelectorEvents(deleteSelector);
					DeleteContextValidatingEventArgs deleteContextArgs = new DeleteContextValidatingEventArgs(ContextIdentifier.Delete,
						  deleteSelector.GetObjectsToValidate(ObjectSpace, deletingObject), deletingObject);
					OnDeleteContextValidating(deleteContextArgs);
					targetObjects.AddRange(deleteContextArgs.TargetObjects);
				}
			}
			if(!Validator.RuleSet.ValidateAll(ObjectSpace, targetObjects, ContextIdentifier.Delete, CustomizeDeleteValidationException)) {
				preValidatedDeletedObjects = new ArrayList(targetObjects);
			}
		}
		private void ObjectSpace_Committing(object sender, CancelEventArgs e) {
			using(ValidationTargetObjectSelector deleteSelector = new DeleteContextTargetObjectSelector()) {
				SubscribeSelectorEvents(deleteSelector);
				DeleteContextValidatingEventArgs deleteContextArgs = new DeleteContextValidatingEventArgs(ContextIdentifier.Delete,
					deleteSelector.GetObjectsToValidate(ObjectSpace, null), null);
				OnDeleteContextValidating(deleteContextArgs);
				if(preValidatedDeletedObjects != null && preValidatedDeletedObjects.Count > 0) {
					foreach(object obj in preValidatedDeletedObjects) {
						deleteContextArgs.TargetObjects.Remove(obj);
					}
					preValidatedDeletedObjects = null;
				}
				Validator.RuleSet.ValidateAll(ObjectSpace, deleteContextArgs.TargetObjects, ContextIdentifier.Delete, CustomizeDeleteValidationException);
			}
			using(ValidationTargetObjectSelector saveSelector = CreateSaveContextTargetObjectSelector()) {
				SubscribeSelectorEvents(saveSelector);
				ContextValidatingEventArgs saveContextArgs = new ContextValidatingEventArgs(ContextIdentifier.Save,
					saveSelector.GetObjectsToValidate(ObjectSpace, ((ObjectView)View).CurrentObject));
				OnContextValidating(saveContextArgs);
				Validator.RuleSet.ValidateAll(ObjectSpace, saveContextArgs.TargetObjects, ContextIdentifier.Save, CustomizeSaveValidationException);
			}
		}
		private void selector_SelectorCustomGetAggregatedObjectsToValidate(object sender, CustomGetAggregatedObjectsToValidateEventArgs args) {
			if(CustomGetAggregatedObjectsToValidate != null) {
				CustomGetAggregatedObjectsToValidate(this, args);
			}
		}
		private void selector_SelectorNeedToValidateObject(object sender, NeedToValidateObjectEventArgs args) {
			if(NeedToValidateObject != null) {
				NeedToValidateObject(this, args);
			}
		}
		protected virtual void OnDeleteContextValidating(DeleteContextValidatingEventArgs args) {
			if(ContextValidating != null) {
				ContextValidating(this, args);
			}
			if(DeleteContextValidating != null) {
				DeleteContextValidating(this, args);
			}
		}
		protected virtual void OnContextValidating(ContextValidatingEventArgs args) {
			if(ContextValidating != null) {
				ContextValidating(this, args);
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			ObjectSpace.CustomDeleteObjects += new EventHandler<CustomDeleteObjectsEventArgs>(ObjectSpace_CustomDeleteObjects);
			ObjectSpace.Committing += new EventHandler<CancelEventArgs>(ObjectSpace_Committing);
		}
		protected override void OnDeactivated() {
			ObjectSpace.CustomDeleteObjects -= new EventHandler<CustomDeleteObjectsEventArgs>(ObjectSpace_CustomDeleteObjects);
			ObjectSpace.Committing -= new EventHandler<CancelEventArgs>(ObjectSpace_Committing);
			base.OnDeactivated();
		}
		public PersistenceValidationController()
			: base() {
			TargetViewNesting = Nesting.Root;
			TypeOfView = typeof(ObjectView);
		}
		public event EventHandler<ContextValidatingEventArgs> ContextValidating;
		public event EventHandler<DeleteContextValidatingEventArgs> DeleteContextValidating;
		public event EventHandler<CustomGetAggregatedObjectsToValidateEventArgs> CustomGetAggregatedObjectsToValidate;
		public event EventHandler<NeedToValidateObjectEventArgs> NeedToValidateObject;
	}
}

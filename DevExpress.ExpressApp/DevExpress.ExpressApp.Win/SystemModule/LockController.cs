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
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.Win.SystemModule {
	[Serializable]
	public class SimultaneousDataModificationException : Exception {
		private ReadOnlyCollection<SimultaneousChange> simultaneousChanges;
		protected SimultaneousDataModificationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public SimultaneousDataModificationException(string message, ReadOnlyCollection<SimultaneousChange> simultaneousChanges)
			: base(message) {
			this.simultaneousChanges = simultaneousChanges;
		}
		public SimultaneousDataModificationException() { }
		public ReadOnlyCollection<SimultaneousChange> SimultaneousChanges {
			get { return simultaneousChanges; }
		}
	}
	public class CustomFormatSimultaneousChangeMessageEntryEventArgs : EventArgs {
		private string messageEntry;
		private string formatString;
		private IObjectSpace objectSpace;
		private View view;
		private object targetObject;
		public CustomFormatSimultaneousChangeMessageEntryEventArgs(string defaultMessageEntry, string formatString, IObjectSpace objectSpace, View view, object targetObject) {
			this.messageEntry = defaultMessageEntry;
			this.formatString = formatString;
			this.objectSpace = objectSpace;
			this.view = view;
			this.targetObject = targetObject;
		}
		public object TargetObject {
			get { return targetObject; }
		}
		public View View {
			get { return view; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public string FormatString {
			get { return formatString; }
		}
		public string MessageEntry {
			get { return messageEntry; }
		}
	}
	public class CustomFormatSimultaneousChangeMessageEventArgs : EventArgs {
		private string message;
		private string formatString;
		private string messageEntries;
		public CustomFormatSimultaneousChangeMessageEventArgs(string defaultMessage, string formatString, string messageEntries) {
			this.message = defaultMessage;
			this.formatString = formatString;
			this.messageEntries = messageEntries;
		}
		public string Message {
			get { return message; }
			set { message = value; }
		}
		public string FormatString {
			get { return formatString; }
		}
		public string MessageEntries {
			get { return messageEntries; }
		}
	}
	public class CustomProcessSimultaneousModificationsExceptionEventArgs : HandledEventArgs {
		private SimultaneousDataModificationException exception;
		public CustomProcessSimultaneousModificationsExceptionEventArgs(SimultaneousDataModificationException exception) {
			this.exception = exception;
		}
		public SimultaneousDataModificationException Exception {
			get { return exception; }
		}
	}
	public class CustomCollectModifiedObjectsEventArgs : HandledEventArgs {
		private class IsNewObjectFilter {
			private IObjectSpace objectSpace;
			public IsNewObjectFilter(IObjectSpace objectSpace) {
				this.objectSpace = objectSpace;
			}
			public bool CheckCondition(object obj) {
				return !objectSpace.IsNewObject(obj);
			}
		}
		private IObjectSpace objectSpace;
		private List<object> modifiedObjects = new List<object>();
		public CustomCollectModifiedObjectsEventArgs(IObjectSpace objectSpace) {
			this.objectSpace = objectSpace;
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public List<object> ModifiedObjects {
			get { return modifiedObjects; }
		}
		public IList CollectModifiedObjects(Boolean checkObjectSpaceIsModified) {
			if(!checkObjectSpaceIsModified || objectSpace.IsModified) {
				return CollectionsHelper.ConditionalCopy(objectSpace.ModifiedObjects, new IsNewObjectFilter(objectSpace).CheckCondition);
			}
			else {
				return new ArrayList();
			}
		}
	}
	public class SimultaneousChange : SimultaneousChangeEntry {
		private List<SimultaneousChangeEntry> entries = new List<SimultaneousChangeEntry>();
		public SimultaneousChange(object modifiedObject, View view, IObjectSpace objectSpace)
			: base(modifiedObject, view, objectSpace) {
		}
		public List<SimultaneousChangeEntry> Entries {
			get { return entries; }
		}
	}
	public class SimultaneousChangeEntry {
		private object modifiedObject;
		private View view;
		private IObjectSpace objectSpace;
		public SimultaneousChangeEntry(object modifiedObject, View view, IObjectSpace objectSpace) {
			this.modifiedObject = modifiedObject;
			this.objectSpace = objectSpace;
			this.view = view;
		}
		public object ModifiedObject {
			get { return modifiedObject; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public View View {
			get { return view; }
		}
	}
	public class LockController : ViewController {
		private static List<LockController> activeControllers = new List<LockController>();
		private static List<Int32> processedLocks = new List<int>();
		private Boolean checkObjectSpaceIsModified;
		private void propertyEditor_ControlValueChanged(object sender, EventArgs e) {
			if(View != null) {
				CheckLocking();
			}
		}
		private void ViewObjectSpace_ModifiedChanged(object sender, EventArgs e) {
			if(View == null) {
				return;
			}
			if(!((IObjectSpace)sender).IsModified) {
				processedLocks.Remove(sender.GetHashCode());
			}
		}
		private void ViewObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if(View == null) {
				return;
			}
			CheckLocking();
		}
		private void CheckLocking() {
			if(!processedLocks.Contains(OwnerObjectSpace.GetHashCode())) {
				try {
					CheckLocking(this);
				}
				catch(SimultaneousDataModificationException e) {
					processedLocks.Add(OwnerObjectSpace.GetHashCode());
					CustomProcessSimultaneousModificationsExceptionEventArgs args = new CustomProcessSimultaneousModificationsExceptionEventArgs(e);
					if(CustomProcessSimultaneousModificationsException != null) {
						CustomProcessSimultaneousModificationsException(this, args);
					}
					if(!args.Handled) {
						throw;
					}
				}
			}
		}
		private static string GetContextCaption(IObjectSpace objectSpace, string defaultCaption) {
			foreach(LockController controller in activeControllers) {
				if((controller.OwnerObjectSpace == objectSpace) && controller.View.IsRoot) {
					return controller.View.Caption;
				}
			}
			return defaultCaption;
		}
		private static string FormatSimultaneousChangeMessage(string formatString, string messageEntries) {
			string defaultString = string.Format(UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.SimultaneousChangeDataMessage),
				messageEntries);
			CustomFormatSimultaneousChangeMessageEventArgs args = new CustomFormatSimultaneousChangeMessageEventArgs(
				defaultString, formatString, messageEntries);
			if(CustomFormatSimultaneousChangeMessage != null) {
				CustomFormatSimultaneousChangeMessage(null, args);
			}
			return args.Message;
		}
		private static string FormatSimultaneousChangeMessageEntry(string formatString, IObjectSpace objectSpace, View view, object obj) {
			string defaultString = string.Format(
				formatString,
				GetContextCaption(objectSpace, view.Caption), "ID: '" + view.Id + "', '" + view.GetHashCode() + "'",
				ReflectionHelper.GetObjectDisplayText(obj), obj.GetType().Name + ", " + objectSpace.GetKeyValue(obj));
			CustomFormatSimultaneousChangeMessageEntryEventArgs args = new CustomFormatSimultaneousChangeMessageEntryEventArgs(
				defaultString, formatString, objectSpace, view, obj);
			if(CustomFormatSimultaneousChangeMessageEntry != null) {
				CustomFormatSimultaneousChangeMessageEntry(null, args);
			}
			return args.MessageEntry;
		}
		private static bool IsParentObjectSpace(IObjectSpace queriedObjectSpace, IObjectSpace currentObjectSpace) {
			do {
				if(queriedObjectSpace == currentObjectSpace) {
					return true;
				}
				INestedObjectSpace nested = currentObjectSpace as INestedObjectSpace;
				currentObjectSpace = nested != null ? nested.ParentObjectSpace : null;
			} while(currentObjectSpace != null);
			return false;
		}
		protected override void OnActivated() {
			base.OnActivated();
			if(View.IsRoot) {
				OwnerObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ViewObjectSpace_ObjectChanged);
				OwnerObjectSpace.ModifiedChanged += new EventHandler(ViewObjectSpace_ModifiedChanged);
				activeControllers.Add(this);
			}
			if(View is DetailView) {
				foreach(PropertyEditor propertyEditor in ((DetailView)View).GetItems<PropertyEditor>()) {
					propertyEditor.ControlValueChanged += new EventHandler(propertyEditor_ControlValueChanged);
				}
			}
		}
		protected override void OnDeactivated() {
			activeControllers.Remove(this);
			if(OwnerObjectSpace != null) {
				processedLocks.Remove(OwnerObjectSpace.GetHashCode());
				OwnerObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ViewObjectSpace_ObjectChanged);
				OwnerObjectSpace.ModifiedChanged -= new EventHandler(ViewObjectSpace_ModifiedChanged);
				if(View is DetailView) {
					foreach(PropertyEditor propertyEditor in ((DetailView)View).GetItems<PropertyEditor>()) {
						propertyEditor.ControlValueChanged -= new EventHandler(propertyEditor_ControlValueChanged);
					}
				}
			}
			base.OnDeactivated();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				activeControllers.Remove(this);
			}
		}
		public LockController()
			: base() {
			TypeOfView = typeof(ObjectView);
		}
		public virtual IList GetModifiedObjects() {
			CustomCollectModifiedObjectsEventArgs args = new CustomCollectModifiedObjectsEventArgs(OwnerObjectSpace);
			if(CustomCollectModifiedObjects != null) {
				CustomCollectModifiedObjects(this, args);
			}
			if(args.Handled) {
				return args.ModifiedObjects;
			}
			return args.CollectModifiedObjects(checkObjectSpaceIsModified);
		}
		public static void CheckLocking(IObjectSpace queriedObjectSpace, IList objectsToCheck, View view) {
			if(queriedObjectSpace == null) {
				throw new ArgumentNullException("objectSpace");
			}
			if(objectsToCheck == null || objectsToCheck.Count == 0) {
				return;
			}
			List<SimultaneousChange> simultaneousChanges = new List<SimultaneousChange>();
			foreach(LockController lockController in activeControllers) {
				if(!IsParentObjectSpace(queriedObjectSpace, lockController.ObjectSpace) &&
				   !IsParentObjectSpace(lockController.ObjectSpace, queriedObjectSpace)) {
					IList lockControllerObjects = lockController.GetModifiedObjects();
					SimultaneousChange simultaneousChange = null;
					foreach(object objLeft in lockControllerObjects) {
						for(int i = 0; i < objectsToCheck.Count; i++) {
							object objRight = objectsToCheck[i];
							Object objLeftKey = queriedObjectSpace.GetKeyValue(objLeft);
							Object objRightKey = queriedObjectSpace.GetKeyValue(objRight);
							if((objLeft.GetType() == objRight.GetType())
									&& (objLeftKey != null) && (objRightKey != null) && Object.Equals(objLeftKey, objRightKey)) {
								if(simultaneousChange == null) {
									simultaneousChange = new SimultaneousChange(objLeft, view, queriedObjectSpace);
								}
								simultaneousChange.Entries.Add(new SimultaneousChangeEntry(objLeft, lockController.View, lockController.OwnerObjectSpace));
							}
						}
					}
					if(simultaneousChange != null) {
						simultaneousChanges.Add(simultaneousChange);
					}
				}
			}
			if(simultaneousChanges.Count > 0) {
				StringBuilder result = new StringBuilder();
				foreach(SimultaneousChange simultaneousChange in simultaneousChanges) {
					result.AppendLine(FormatSimultaneousChangeMessageEntry(UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.SimultaneousChangeDataMessageItem),
						simultaneousChange.ObjectSpace,
						simultaneousChange.View, simultaneousChange.ModifiedObject));
					foreach(SimultaneousChangeEntry entry in simultaneousChange.Entries) {
						result.AppendLine(FormatSimultaneousChangeMessageEntry(UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.SimultaneousChangeDataMessageItem),
							entry.ObjectSpace, entry.View, entry.ModifiedObject));
					}
				}
				throw new UserFriendlyException(new SimultaneousDataModificationException(
					FormatSimultaneousChangeMessage(UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.SimultaneousChangeDataMessage), result.ToString()),
					new ReadOnlyCollection<SimultaneousChange>(simultaneousChanges)));
			}
		}
		public static void CheckLocking(LockController controller) {
			if(controller == null) {
				throw new ArgumentNullException("queryController");
			}
			foreach(LockController activeController in activeControllers) {
				activeController.checkObjectSpaceIsModified = (activeController.ObjectSpace != controller.ObjectSpace);
			}
			CheckLocking(controller.OwnerObjectSpace, controller.GetModifiedObjects(), controller.View);
		}
		public IObjectSpace OwnerObjectSpace {
			get { return View.ObjectSpace; }
		}
		public event EventHandler<CustomProcessSimultaneousModificationsExceptionEventArgs> CustomProcessSimultaneousModificationsException;
		public event EventHandler<CustomCollectModifiedObjectsEventArgs> CustomCollectModifiedObjects;
		public static event EventHandler<CustomFormatSimultaneousChangeMessageEntryEventArgs> CustomFormatSimultaneousChangeMessageEntry;
		public static event EventHandler<CustomFormatSimultaneousChangeMessageEventArgs> CustomFormatSimultaneousChangeMessage;
	}
}

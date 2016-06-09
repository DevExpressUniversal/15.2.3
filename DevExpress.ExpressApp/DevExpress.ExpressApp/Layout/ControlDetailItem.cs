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
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Layout {
	#region Obsolete 15.2
	[Obsolete("Use ControlViewItem instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ControlDetailItem : ControlViewItem {
		protected internal ControlDetailItem(String controlTypeName, String id, String caption, Type objectType)
			: base(controlTypeName, id, caption, objectType) {
		}
		public ControlDetailItem(String id, String caption, Object control)
			: base(id, caption, control) {
		}
		public ControlDetailItem(String id, Object control)
			: base(id, control) {
		}
		public ControlDetailItem(IModelControlDetailItem model, Type objectType)
			: base(model, objectType) {
		}
	}
	#endregion
	public class ControlViewItem : ViewItem, IComplexViewItem {
		private Object control;
		private String caption;
		private Boolean isCaptionVisible;
		private String controlTypeName;
		private IObjectSpace objectSpace = null;
		private XafApplication application = null;
		protected override Object CreateControlCore() {
			if(!String.IsNullOrEmpty(controlTypeName)) {
				control = ReflectionHelper.CreateObject(controlTypeName);
				if(control is IComplexControl) {
					((IComplexControl)control).Setup(objectSpace, application);
				}
			}
			return control;
		}
		private void objectSpace_Reloaded(Object sender, EventArgs e) {
			if(control is IComplexControl) {
				((IComplexControl)control).Refresh();
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(objectSpace != null) {
				objectSpace.Reloaded -= objectSpace_Reloaded;
			}
		}
		protected internal ControlViewItem(String controlTypeName, String id, String caption, Type objectType)
			: base(objectType, id) {
			this.controlTypeName = controlTypeName;
			this.caption = caption;
			isCaptionVisible = true;
		}
		public ControlViewItem(String id, String caption, Object control)
			: base(null, id) {
			this.control = control;
			if(!String.IsNullOrEmpty(caption)) {
				this.caption = caption;
			}
			else {
				this.caption = id;
			}
			isCaptionVisible = true;
			if(control != null) {
				CreateControl();
			}
		}
		public ControlViewItem(String id, Object control)
			: this(id, null, control) {
			isCaptionVisible = false;
		}
		public ControlViewItem(IModelControlDetailItem model, Type objectType)
			: this(model.ControlTypeName, model.Id, model.Caption, objectType) {
		}
		public override void BreakLinksToControl(Boolean unwireEventsOnly) {
			base.BreakLinksToControl(unwireEventsOnly);
			if(!unwireEventsOnly) {
				control = null;
			}
		}
		public override String Caption {
			get { return caption; }
		}
		public override Boolean IsCaptionVisible {
			get { return isCaptionVisible; }
		}
		public void Setup(IObjectSpace objectSpace, XafApplication application) {
			this.objectSpace = objectSpace;
			this.application = application;
			if(objectSpace != null) {
				objectSpace.Reloaded += objectSpace_Reloaded;
			}
		}
	}
}

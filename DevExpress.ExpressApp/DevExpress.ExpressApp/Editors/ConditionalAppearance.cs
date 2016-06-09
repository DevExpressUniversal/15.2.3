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
using System.Drawing;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Persistent.Base;
using DevExpress.Utils;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Editors {
	public enum ViewItemVisibility {
		[ImageName("State_ItemVisibility_Hide")]
		Hide,
		ShowEmptySpace,
		[ImageName("State_ItemVisibility_Show")]
		Show
	}
	public class CustomizeAppearanceEventArgs : EventArgs {
		private string name;
		private IAppearanceBase item;
		private object contextObject;
		private EvaluatorContextDescriptor evaluatorContextDescriptor;
		public CustomizeAppearanceEventArgs(string name, IAppearanceBase item, object contextObject, IViewInfo viewInfo) : this(name, null, item, contextObject, viewInfo) { }
		public CustomizeAppearanceEventArgs(string name, string appearanceItemType, IAppearanceBase item, object contextObject, IViewInfo viewInfo) {
			this.name = name;
			this.AppearanceItemType = appearanceItemType;
			this.item = item;
			this.contextObject = contextObject;
			this.ViewInfo = viewInfo;
		}
		public CustomizeAppearanceEventArgs(string name, IAppearanceBase item, object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor): this(name,item, contextObject, evaluatorContextDescriptor, null){}
		public CustomizeAppearanceEventArgs(string name, IAppearanceBase item, object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor, IViewInfo viewInfo)
			: this(name, null, item, contextObject, evaluatorContextDescriptor, viewInfo) { }
		public CustomizeAppearanceEventArgs(string name, string appearanceItemType, IAppearanceBase item, object contextObject, EvaluatorContextDescriptor evaluatorContextDescriptor, IViewInfo viewInfo)
			: this(name, appearanceItemType, item, contextObject, viewInfo) {
			this.evaluatorContextDescriptor = evaluatorContextDescriptor;
		}
		public CustomizeAppearanceEventArgs(string name, IAppearanceBase item, object contextObject) : this(name,item, contextObject, null,null){			
		}
		public string Name {
			get { return name; }
		}
		public string AppearanceItemType { get; private set; }
		public IAppearanceBase Item {
			get { return item; }
		}
		public object ContextObject {
			get { return contextObject; }
		}
		public EvaluatorContextDescriptor EvaluatorContextDescriptor {
			get { return evaluatorContextDescriptor; }
		}
		public IViewInfo ViewInfo { get; set; }
	}
	public interface ISupportAppearanceCustomization {
		event EventHandler<CustomizeAppearanceEventArgs> CustomizeAppearance;
	}
	public class AppearanceResetProperties {
		public const string BackColor = "BackColor";
		public const string FontColor = "FontColor";
		public const string FontStyle = "FontStyle";
		public const string Visibility = "Visibility";
		public const string Enabled = "Enabled";
	}
	public interface IAppearanceBase { }
	public interface IAppearanceFormat : IAppearanceBase {
		FontStyle FontStyle { get; set; }
		Color FontColor { get; set; }
		Color BackColor { get; set; }
		void ResetFontStyle();
		void ResetFontColor();
		void ResetBackColor();
	}
	public interface IAppearanceEnabled : IAppearanceBase {
		bool Enabled { get; set; }
		void ResetEnabled();
	}
	public interface IAppearanceVisibility : IAppearanceBase {
		ViewItemVisibility Visibility { get; set; }
		void ResetVisibility();
	}
	public interface INotifyAppearanceVisibilityChanged {
		event EventHandler VisibilityChanged;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class AppearanceObjectAdapter : IAppearanceFormat {
		private Font defaultFont;
		private Color defaultFontColor;
		private Color defaultBackColor;
		public AppearanceObjectAdapter(AppearanceObject appearanceObject) {
			this.AppearanceObject = appearanceObject;
			defaultFont = appearanceObject.Font;
			defaultFontColor = appearanceObject.ForeColor;
			defaultBackColor = appearanceObject.BackColor;
		}
		public AppearanceObject AppearanceObject { get; private set; }
		#region IAppearanceFormat Members
		FontStyle IAppearanceFormat.FontStyle {
			get { return AppearanceObject.Font.Style; }
			set { AppearanceObject.Font = new Font(AppearanceObject.Font, value); }
		}
		Color IAppearanceFormat.FontColor {
			get { return AppearanceObject.ForeColor; }
			set { AppearanceObject.ForeColor = value; }
		}
		Color IAppearanceFormat.BackColor {
			get { return AppearanceObject.BackColor; }
			set { AppearanceObject.BackColor = value; }
		}
		void IAppearanceFormat.ResetFontStyle() { AppearanceObject.Font = defaultFont; }
		void IAppearanceFormat.ResetFontColor() { AppearanceObject.ForeColor = defaultFontColor; }
		void IAppearanceFormat.ResetBackColor() { AppearanceObject.BackColor = defaultBackColor; }
		#endregion
	}
	public interface IViewInfo {
		BoolList AllowEdit { get; }
		Type ViewType { get;  }
		string ViewId { get;  }
		IObjectSpace ObjectSpace { get; }
		IModelClass ModelClass { get;  }
	}
	public class ViewInfo : IViewInfo {
		public ViewInfo() {
			AllowEdit = new BoolList();
		}
		public static ViewInfo FromView(ObjectView view) {
			ViewInfo result = new ViewInfo();
			if(view != null) {
				result.AllowEdit = view.AllowEdit;
				result.ViewId = view.Id;
				result.ViewType = view.GetType();
				result.ObjectSpace = view.ObjectSpace;
				if(view.Model != null) {
					result.ModelClass = view.Model.ModelClass;
				}
			}
			return result;
		}
		public BoolList AllowEdit { get; private set; }
		public Type ViewType { get; set; }
		public string ViewId { get; set; }
		public IObjectSpace ObjectSpace { get; set; }
		public IModelClass ModelClass { get; set; }
	}
}

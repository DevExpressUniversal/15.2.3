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

using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.XtraReports.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public interface IDesignerActionMethodItem : IDesignerActionPropertyItem {
		void Invoke();
	}
	public interface IDesignerActionPropertyItem : IDesignerActionItem {
		string MemberName { get; }
	}
	public interface IDesignerActionList {
		IComponent Component { get; }
		IDesignerActionItemCollection GetSortedActionItems();
		PropertyDescriptor FilterProperty(string name, PropertyDescriptor property);
		object PropertiesContainer { get; }
	}
	public class DesignerActionMethodItemBase : IDesignerActionMethodItem {
		readonly object owner;
		readonly string memberName;
		readonly string displayName;
		MethodInfo methodInfo;
		public DesignerActionMethodItemBase(object owner, string memberName, string displayName) {
			this.displayName = displayName;
			this.owner = owner;
			this.memberName = memberName;
			InitializeAction();
		}
		public DesignerActionMethodItemBase(Delegate action, string displayName) {
			this.displayName = displayName;
			this.owner = action.Target;
			this.memberName = action.Method.Name;
			this.methodInfo = action.Method;
			InitializeAction();
		}
		protected object Owner { get { return owner; } }
		public string MemberName { get { return memberName; } }
		public string DisplayName { get { return displayName; } }
		protected virtual void InitializeAction() {
			if(this.methodInfo != null)
				return;
			MethodInfo methodInfo = owner.GetType().GetMethod(MemberName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			if(methodInfo == null)
				throw new InvalidOperationException();
		}
		public void Invoke() {
			this.methodInfo.Invoke(Owner, new object[0]);
		}
	}
	public abstract class LinkLineController : BaseLineController {
		protected LinkLine LinkLine {
			get { return (LinkLine)fLine; }
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				LinkLine.LinkClicked -= new EventHandler(OnLinkClicked);
			}
			base.Dispose(disposing);
		}
		protected abstract void OnLinkClicked(object sender, EventArgs e);
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new LinkLine();
		}
		protected override void InitLine() {
			base.InitLine();
			LinkLine.LinkClicked += new EventHandler(OnLinkClicked);
		}
	}
	public class MethodLineController : LinkLineController {
		IDesignerActionMethodItem actionMethodItem;
		public MethodLineController(IDesignerActionMethodItem actionMethodItem) {
			this.actionMethodItem = actionMethodItem;
		}
		protected override void OnLinkClicked(object sender, EventArgs e) {
			actionMethodItem.Invoke();
		}
		protected override void InitLine() {
			base.InitLine();
			fLine.SetText(actionMethodItem.DisplayName);
		}
	}
	class VerbLineController : LinkLineController {
		DesignerVerb verb;
		public VerbLineController(DesignerVerb verb) {
			this.verb = verb;
		}
		protected override void OnLinkClicked(object sender, EventArgs e) {
			verb.Invoke();
		}
		protected override void InitLine() {
			base.InitLine();
			fLine.SetText(verb.Text);
		}
	}
	abstract class PropertyLineBaseController : BaseLineController {
		protected IDesignerActionPropertyItem actionItem;
		protected IDesignerActionList actionList;
		PropertyDescriptor property;
		protected PropertyDescriptor Property {
			get {
				if (property == null) {
					PropertyDescriptor value = XRAccessor.GetPropertyDescriptor(actionList.PropertiesContainer, actionItem.MemberName);
					PropertyDescriptor filteredValue = actionList.FilterProperty(actionItem.MemberName, value);
					property = filteredValue != null ? filteredValue : value;
				}
				return property;
			}
		}
		protected PropertyLineBaseController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList) {
			this.actionItem = actionItem;
			this.actionList = actionList;
		}
	}
	class BooleanLineController : PropertyLineBaseController {
		BooleanLine BooleanLine {
			get { return (BooleanLine)fLine; }
		}
		public BooleanLineController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList)
			: base(actionItem, actionList) {
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new BooleanLine(Property, actionList.PropertiesContainer);
		}
		protected override void InitLine() {
			base.InitLine();
			BooleanLine.SetText(actionItem.DisplayName);
		}
	}
	internal abstract class EditorPropertyLineBaseController : PropertyLineBaseController {
		protected EditorPropertyLineBase EditorPropertyLineBase {
			get { return (EditorPropertyLineBase)fLine; }
		}
		protected EditorPropertyLineBaseController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList)
			: base(actionItem, actionList) {
		}
		protected override void InitLine() {
			base.InitLine();
			EditorPropertyLineBase.SetText(actionItem.DisplayName);
		}
		protected TypeDescriptorContext CreateContext() {
			return new TypeDescriptorContext(actionList.Component.Site, Property, ObjectWrapperComponentHelper.GetObjectInstance(actionList.Component));
		}
		protected TypeStringConverter CreateStringConverter() {
			return new TypeStringConverter(Property.Converter, CreateContext());
		}
	}
	class EditorPropertyLineController : EditorPropertyLineBaseController {
		static IServiceProvider GetServiceProvider(IDesignerActionList actionList) {
			return actionList != null && actionList.Component != null
				? actionList.Component.Site :
				null;
		}
		public override bool IsRunning {
			get { return ((EditorPropertyLine)fLine).IsRunning; }
		}
		public EditorPropertyLineController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList)
			: base(actionItem, actionList) {
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new EditorPropertyLine(GetServiceProvider(actionList), actionList.Component, CreateStringConverter(), Property, actionList.PropertiesContainer);
		}
	}
	class StandardValuesContainer : IEnumerable {
		TypeDescriptorContext context;
		TypeConverter converter;
		public StandardValuesContainer(TypeDescriptorContext context, TypeConverter converter) {
			this.context = context;
			this.converter = converter;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			object[] values = RuntimeTypeDescriptorContext.GetStandardValues(context, converter);
			return values != null ? values.GetEnumerator() : new object[0].GetEnumerator();
		}
	}
	class ComboBoxPropertyLineController : EditorPropertyLineBaseController {
		public ComboBoxPropertyLineController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList)
			: base(actionItem, actionList) {
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new ComboBoxPropertyLine(CreateStringConverter(), new StandardValuesContainer(CreateContext(), Property.Converter), Property, actionList.PropertiesContainer);
		}
	}
	class ColorPropertyLineController : EditorPropertyLineBaseController {
		public ColorPropertyLineController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList)
			: base(actionItem, actionList) {
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new ColorPropertyLine(CreateStringConverter(), Property, actionList.PropertiesContainer);
		}
	}
	class FloatNumericPropertyLineController : EditorPropertyLineBaseController {
		public FloatNumericPropertyLineController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList)
			: base(actionItem, actionList) {
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new FloatNumericPropertyLine(CreateStringConverter(), Property, actionList.PropertiesContainer);
		}
	}
	class NumericPropertyLineController : EditorPropertyLineBaseController {
		public NumericPropertyLineController(IDesignerActionPropertyItem actionItem, IDesignerActionList actionList)
			: base(actionItem, actionList) {
		}
		protected override ILine CreateLine(LineFactoryBase lineFactory) {
			return new NumericPropertyLine(CreateStringConverter(), Property, actionList.PropertiesContainer);
		}
	}
}

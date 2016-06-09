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
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.LookAndFeel;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraReports.Design;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraReports.Native;
using System.Reflection;
namespace DevExpress.XtraPrinting.Native {
	public interface IComponentDesigner {
		IDesignerActionListCollection ActionLists { get; }
		DesignerVerbCollection Verbs { get; }
	}
	public interface IDesignerActionListCollection : IEnumerable {
		IDesignerActionList this[int index] { get; }
		int Count { get; }
	}
	public interface IDesignerActionItemCollection : IEnumerable {		
	}
	public interface IDesignerActionItem {
		string DisplayName { get; }
	}
	public class SmartTagServiceBase : IDisposable {
		#region static
		public static bool HasSmartTagPresentation(IComponentDesigner designer) {
			return designer != null && (designer.ActionLists.Count > 0 || designer.Verbs.Count > 0);
		}
		#endregion
		#region Inner classes
		public class PopupHelper {
			readonly SmartTagServiceBase smartTagSvc;
			readonly System.Windows.Forms.Control container;
			LinesContainer linesContainer;
			public PopupHelper(SmartTagServiceBase smartTagSvc, System.Windows.Forms.Control container) {
				this.smartTagSvc = smartTagSvc;
				this.container = container;
				linesContainer = new LinesContainer();
				container.Controls.Add(linesContainer);
			}
			protected System.Windows.Forms.Control Container { get { return container; } }
			public SmartTagServiceBase SmartTagSvc { get { return smartTagSvc; } }
			public UserLookAndFeel LookAndFeel {
				get {
					ISupportLookAndFeel supportLookAndFeel = Container as ISupportLookAndFeel;
					if (supportLookAndFeel == null)
						return null;
					else
						return supportLookAndFeel.LookAndFeel;
				}
			}
			public Size FillLinesController(BaseLineController[] controllers, Point location) {
				BaseLine[] lines = Array.ConvertAll<ILine, BaseLine>(BaseLineController.GetLines(controllers, new LineFactory()), delegate(ILine line) { return (BaseLine)line; });
				linesContainer.FillWithLines(lines, LookAndFeel, container.MinimumSize.Width, 3, 8);
				linesContainer.Location = location;
				return linesContainer.Size;
			}
		}
		#endregion
		public virtual BaseLineController[] CreateLineControllers(IComponentDesigner designer) {
			List<string> controllersActionsNames = new List<string>();
			List<BaseLineController> controllers = new List<BaseLineController>();
			IDesignerActionListCollection actionListCollection = designer.ActionLists;
			if (designer.Verbs != null && designer.Verbs.Count > 0) {				
				foreach (DesignerVerb verb in designer.Verbs) {
					if (!IncludeInSmartTag(verb))
						continue;
					if (verb.Enabled && verb.Visible) {
						controllers.Add(new VerbLineController(verb));
						controllersActionsNames.Add(verb.Text);
					}
				}
			}
			List<BaseLineController> existControllers = new List<BaseLineController>();
			foreach (IDesignerActionList actionList in actionListCollection) {
				IDesignerActionItemCollection actionItemCollection = actionList.GetSortedActionItems();
				foreach (IDesignerActionItem actionItem in actionItemCollection) {
					if (controllersActionsNames.Contains(actionItem.DisplayName)) continue;
					BaseLineController controller = CreateLineController(actionItem, actionList);
					if (controller != null) {
						existControllers.Add(controller);
						controllersActionsNames.Add(actionItem.DisplayName);
					}
				}
				if (existControllers.Count > 0) {
					if (controllers.Count > 0)
						controllers.Add(new SeparatorLineController());
					controllers.AddRange(existControllers);
					existControllers.Clear();
				}
			}
			controllersActionsNames.Clear();
			BaseLineController[] resultArray = controllers.ToArray();
			controllers.Clear();
			return resultArray;
		}
		protected virtual bool IncludeInSmartTag(DesignerVerb verb) {
			return true;
		}
		protected virtual BaseLineController CreateLineController(IDesignerActionItem actionItem, IDesignerActionList actionList) {
			IDesignerActionMethodItem actionMethodItem = actionItem as IDesignerActionMethodItem;
			if (actionMethodItem != null) {
				try {
					CustomLineControllerAttribute lineController = GetLineControllerAttribute(actionList, actionMethodItem.MemberName);
					if (lineController != null)
						return lineController.Type.GetConstructor(new Type[] { typeof(IDesignerActionMethodItem) }).Invoke(new object[] { actionMethodItem }) as BaseLineController;
				} catch { }
				return new MethodLineController(actionMethodItem);
			}
			else if (actionItem is IDesignerActionPropertyItem) {
				IDesignerActionPropertyItem actionPropertyItem = (IDesignerActionPropertyItem)actionItem;
				PropertyDescriptor property = XRAccessor.GetPropertyDescriptor(actionList.PropertiesContainer, actionPropertyItem.MemberName);
				try {
					CustomLineControllerAttribute lineController = GetLineControllerAttribute(property);
					if (lineController != null)
						return lineController.Type.GetConstructor(new Type[] { typeof(IDesignerActionPropertyItem), typeof(IDesignerActionList) }).Invoke(new object[] { actionItem, actionList }) as BaseLineController;
				} catch { }
				if (property.PropertyType == typeof(Color))
					return new ColorPropertyLineController(actionPropertyItem, actionList);
				else if (property.PropertyType == typeof(bool))
					return new BooleanLineController(actionPropertyItem, actionList);
				else if (PSNativeMethods.IsFloatType(property.PropertyType)) {
					return new FloatNumericPropertyLineController(actionPropertyItem, actionList);
				}
				else if (PSNativeMethods.IsNumericalType(property.PropertyType))
					return new NumericPropertyLineController(actionPropertyItem, actionList);
				UITypeEditor editor = property.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
				if (editor != null)
					return new EditorPropertyLineController(actionPropertyItem, actionList);
				else
					return new ComboBoxPropertyLineController(actionPropertyItem, actionList);
			}
			return null;
		}
		CustomLineControllerAttribute GetLineControllerAttribute(PropertyDescriptor property) {
			int count = property.Attributes.Count;
			for (int i = 0; i < count; i++)
				if (property.Attributes[i] is CustomLineControllerAttribute)
					return property.Attributes[i] as CustomLineControllerAttribute;
			return null;
		}
		CustomLineControllerAttribute GetLineControllerAttribute(IDesignerActionList actionList, string memberName) {
			if(string.IsNullOrEmpty(memberName))
				return null;
			MethodInfo methodInfo = actionList.GetType().GetMethod(memberName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			CustomLineControllerAttribute attribute = (CustomLineControllerAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(CustomLineControllerAttribute));
			return attribute;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		~SmartTagServiceBase() {
			Dispose(false);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
	#region lines
	[ToolboxItem(false)]
	public class LinkLabelControl : LinkLabel {
		public virtual void Initialize(UserLookAndFeel lookAndFeel) {
			VisitedLinkColor = ActiveLinkColor = LinkColor = LinkColorHelper.GetSkinColor(lookAndFeel);
		}
		public override Size GetPreferredSize(Size proposedSize) {
			using(XtraEditors.LabelControl label = new XtraEditors.LabelControl()) {
				label.Text = Text;
				Size size = label.GetPreferredSize(proposedSize);
				Size baseSize = base.GetPreferredSize(Size.Empty);
				return new Size(Math.Max(size.Width, baseSize.Width), Math.Max(size.Height, baseSize.Height));
			}
		}
	}
	public class LinkLine : BaseLine {
		static readonly object LinkClickedEvent = new object();
		public event EventHandler LinkClicked {
			add { Events.AddHandler(LinkClickedEvent, value); }
			remove { Events.RemoveHandler(LinkClickedEvent, value); }
		}
		string linkText;
		protected LinkLabelControl linkLabel;
		public LinkLine()
			: base() {
		}
		public override void SetText(string text) {
			linkText = text;
		}
		public override void Init(UserLookAndFeel lookAndFeel) {
			linkLabel = new LinkLabelControl();
			linkLabel.TabStop = true;
			linkLabel.Dock = DockStyle.Left;
			linkLabel.AutoSize = true;
			linkLabel.BackColor = Color.Transparent;
			linkLabel.Initialize(lookAndFeel);
			linkLabel.TextAlign = ContentAlignment.MiddleLeft;
			linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(OnLinkClicked);
			linkLabel.Text = linkText;
			linkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
			base.Init(lookAndFeel);
		}
		public override Size GetLineSize() {
			return linkLabel.GetPreferredSize(Size.Empty);
		}
		protected override System.Windows.Forms.Control[] GetControls() {
			return new System.Windows.Forms.Control[] { linkLabel };
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (linkLabel != null)
					linkLabel.LinkClicked -= new LinkLabelLinkClickedEventHandler(OnLinkClicked);
			}
			base.Dispose(disposing);
		}
		void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			EventHandler handler = (EventHandler)this.Events[LinkClickedEvent];
			if(handler != null)
				CallHandler(handler, sender, e);
		}
		protected virtual void CallHandler(EventHandler handler, object sender, LinkLabelLinkClickedEventArgs e) {
			handler(this, e);
		}
	}
	#endregion
}

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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Description.Controls.Windows;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Internal;
namespace DevExpress.Description.Controls {
	public class GuideControl {
		List<GuideControlDescription> descriptions, descriptionTemplates;
		Control root;
		public GuideControl() {
		}
		public List<GuideControlDescription> Descriptions {
			get {
				if(descriptions == null) descriptions = new List<GuideControlDescription>();
				return descriptions;
			}
		}
		public List<GuideControlDescription> DescriptionTemplates {
			get {
				if(descriptionTemplates == null) descriptionTemplates = new List<GuideControlDescription>();
				return descriptionTemplates;
			}
		}
		public Control Root {
			get {
				return root;
			}
		}
		public virtual void Init(List<GuideControlDescription> descriptionTemplates, Control root) {
			this.descriptionTemplates = descriptionTemplates;
			this.root = root;
			InitControls();
		}
		DXGuideLayeredWindow window;
		protected internal DXGuideLayeredWindow Window { get { return window; } }
		public void Show() {
			if(!HasValidDescriptions) return;
			if(IsVisible || Root == null || !Root.Visible) return;
			OnShowing();
			window = CreateWindow();
			Rectangle bounds = root.RectangleToScreen(root.ClientRectangle);
			if(!UseClientRectangle) {
				bounds = root.RectangleToScreen(root.RectangleToClient(root.Bounds));
			}
			window.Bounds = bounds;
			window.Create(Root);
			window.Show();
			Root.LocationChanged += OnRootLocationChanged;
			Root.SizeChanged += OnRootLocationChanged;
			Root.Move += OnRootLocationChanged;
		}
		void OnRootLocationChanged(object sender, EventArgs e) {
			Hide();
		}
		protected virtual bool UseClientRectangle { get { return Root is Form ? false : true; } }
		protected virtual void OnShowing() {
		}
		protected virtual DXGuideLayeredWindow CreateWindow() { return new DXGuideLayeredWindow(this); }
		public virtual bool IsVisible { get { return window != null && window.Visible; } }
		public virtual void Hide() {
			if(!IsVisible) return;
			window.Hide();
			Root.LocationChanged -= OnRootLocationChanged;
			Root.SizeChanged -= OnRootLocationChanged;
			Root.Move -= OnRootLocationChanged;
			OnHide();
		}
		protected virtual void OnHide() {
		}
		public bool HasValidDescriptions { get { return Descriptions.Count(q => q.AssociatedControl != null && q.ControlVisible) > 0; } }
		void InitControls() {
			var allControls = ParseAllControls();
			foreach(Control c in allControls) {
				foreach(var description in DescriptionTemplates) {
					if(IsMatch(description, c)) {
						AddDescription(description, c);
						break;
					}
				}
			}
			Descriptions.Sort(CompareByRect);
		}
		int CompareByRect(GuideControlDescription x, GuideControlDescription y) {
			if(object.ReferenceEquals(x, y)) return 0;
			Rectangle b1 = x.ControlBounds, b2 = y.ControlBounds;
			Point p1 = DevExpress.Skins.RectangleHelper.GetCenterBounds(b1, new Size(1, 1)).Location;
			Point p2 = DevExpress.Skins.RectangleHelper.GetCenterBounds(b2, new Size(1, 1)).Location;
			if(b1.Contains(b2)) return 1;
			if(b2.Contains(b1)) return -1;
			b2.Offset(-b1.X, -b1.Y);
			b1.Offset(-b1.X, -b1.Y);
			if(b1.IntersectsWith(b2)) {
			}
			int resHeight = CompareHeight(b1, b2);
			if(b1.Right < b2.X || b1.Right == b2.X) {
				if(resHeight != 0) return -1;
				if(b1.Y < b2.Y) return -1;
				return 1;
			}
			if(b1.X < b2.X || b1.X == b2.X) {
				if(resHeight != 0) return -1;
				if(b1.Y < b2.Y) return -1;
				return 1;
			}
			if(resHeight != 0) return resHeight;
			if(b1.Y < b2.Y) return -1;
			return 1;
		}
		int CompareHeight(Rectangle b1, Rectangle b2) {
			if(Math.Abs(b1.Y - b2.Y) < 25) {
				if(Math.Abs(b1.Height - b2.Height) > 50) {
					if(b1.Height < b2.Height) return -1;
					return 1;
				}
			}
			return 0;
		}
		void AddDescription(GuideControlDescription description, Control c) {
			if(!c.Visible) return; 
			IGuideDescription gdc = c as IGuideDescription;
			if(gdc != null) description = LookupSubType(c, gdc, description);
			var desc = description.Clone();
			desc.AssociatedControl = c;
			desc.ControlBounds = ConvertBounds(root.RectangleToClient(desc.AssociatedControl.RectangleToScreen(desc.AssociatedControl.ClientRectangle))); 
			desc.ScreenBounds = desc.AssociatedControl.RectangleToScreen(desc.AssociatedControl.ClientRectangle);
			desc.ControlVisible = desc.AssociatedControl.Visible;
			Descriptions.Add(desc);
		}
		GuideControlDescription LookupSubType(Control c, IGuideDescription gdc, GuideControlDescription description) {
			string typeName = description.ControlTypeName == null ? c.GetType().FullName : description.ControlTypeName;
			if(gdc == null || string.IsNullOrEmpty(gdc.SubType)) return description;
			var res = DescriptionTemplates.FirstOrDefault(q => q.ControlTypeName == typeName + ":" + gdc.SubType);
			return res == null ? description : res;
		}
		protected Rectangle ConvertBounds(Rectangle rectangle) {
			if(UseClientRectangle) return rectangle;
			rectangle.Location = ConvertPoint(rectangle.Location);
			return rectangle;
		}
		protected Point ConvertPoint(Point point) {
			if(UseClientRectangle) return point;
			var delta = root.PointToClient(Root.Location);
			point.X -= delta.X;
			point.Y -= delta.Y;
			return point;
		}
		protected GuideControlDescription Lookup(List<GuideControlDescription> collection, Control c) {
			foreach(GuideControlDescription description in collection) {
				if(IsMatch(description, c)) return description;
			}
			return null;
		}
		protected virtual bool IsMatch(GuideControlDescription description, Control c) {
			if(description.ControlType != null) {
				if(!c.GetType().Equals(description.ControlType)) return false;
			}
			if(description.ControlTypeName != null) {
				string name = c.GetType().FullName;
				if(name == description.ControlTypeName) return true;
				return false;
			}
			if(!string.IsNullOrEmpty(description.ControlName)) {
				return c.Name == description.ControlName;
			}
			return true;
		}
		List<Control> ParseAllControls() {
			List<Control> res = new List<Control>();
			ParseAllControls(res, Root);
			return res;
		}
		void ParseAllControls(List<Control> res, Control parent) {
			res.Add(parent);
			var desc = Lookup(DescriptionTemplates, parent);
			if(desc != null && !desc.AllowParseChildren) return;
			if(parent.HasChildren) {
				foreach(Control c in parent.Controls) {
					ParseAllControls(res, c);
				}
			}
		}
	}
}

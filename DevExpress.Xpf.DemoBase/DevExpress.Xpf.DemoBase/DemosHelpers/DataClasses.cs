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

#if !SILVERLIGHT
using System.Data;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;
using DevExpress.DemoData.Helpers;
using DevExpress.Utils;
namespace DevExpress.Xpf.DemoBase.DataClasses {
	public class Movie {
		public int ID { get; set; }
		public string Name { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public decimal Total { get { return Price * Quantity; } }
		public string Preview { get; set; }
	}
	public class Cars {
		ImageSource imageSource;
		public int ID { get; set; }
		public string Trademark { get; set; }
		public string Model { get; set; }
		public int HP { get; set; }
		public double Liter { get; set; }
		public int Cyl { get; set; }
		[XmlElement("Transmiss Speed Count")]
		public int TransmissSpeedCount { get; set; }
		[XmlElement("Transmiss Automatic")]
		public string TransmissAutomatic { get; set; }
		public int MPGCity { get; set; }
		public int MPGHighway { get; set; }
		public string Category { get; set; }
		public string Description { get; set; }
		public string Hyperlink { get; set; }
		public byte[] Picture { get; set; }
		public ImageSource ImageSource {
			get {
				if(imageSource == null)
					imageSource = ImageSourceHelper.GetImageSource(new MemoryStream(Picture));
				return imageSource;
			}
		}
		public decimal Price { get; set; }
		[XmlElement("Delivery Date")]
		public DateTime DeliveryDate { get; set; }
		[XmlElement("Is In Stock")]
		public bool IsInStock { get; set; }
	}
	public class Order {
		public int ID { get; set; }
		public string Name { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
		public decimal Total { get { return Price * Quantity; } }
		public string Preview { get; set; }
	}
	public class Employee : IComparable, INotifyPropertyChanged {
		public int Id { get; set; }
		public int ParentId { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string JobTitle { get; set; }
		public string Phone { get; set; }
		public string EmailAddress { get; set; }
		public string AddressLine1 { get; set; }
		public string City { get; set; }
		public string StateProvinceName { get; set; }
		public string PostalCode { get; set; }
		public string CountryRegionName { get; set; }
		[XmlIgnore]
		string groupNameCore;
		public string GroupName {
			get { return groupNameCore; }
			set {
				if(groupNameCore != value) {
					RaisePropertyChanged("GroupName");
					groupNameCore = value;
				}
			}
		}
		public DateTime BirthDate { get; set; }
		public DateTime HireDate { get; set; }
		public string Gender { get; set; }
		public string MaritalStatus { get; set; }
		public string Title { get; set; }
		public byte[] ImageData { get; set; }
		public ImageSource Image { get { return Gender == "F" ? EmployeesData.ImageFemale : EmployeesData.ImageMale; } }
		public override string ToString() {
			return FirstName + " " + LastName;
		}
		#region Equality
		public int CompareTo(object obj) {
			Employee empl = obj as Employee;
			if(empl == null)
				throw new ArgumentException();
			return string.Compare(FirstName + LastName, empl.FirstName + empl.LastName);
		}
		#endregion
		public override bool Equals(object obj) {
			if(obj == null) return false;
			return ToString() == obj.ToString();
		}
		public override int GetHashCode() {
			return Id;
		}
		void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
				PropertyChanged(this, e);
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	public class OrderDataSourceCreator {
		public static List<Order> CreateDataSource() { return CreateDataSource(1000); }
		public static List<Order> CreateDataSource(int rowCount) {
			List<Order> list = new List<Order>();
			for(int i = 0; i < rowCount; i++) {
				list.Add(CreateItem(i + 1));
			}
			return list;
		}
		static Order CreateItem(int id) {
			Order order = new Order();
			order.ID = id;
			order.Name = "Name " + id.ToString();
			order.OrderDate = DateTime.Today.AddDays(-id % 20);
			order.Price = 100 + id % 10;
			order.Quantity = 10 + id % 15;
			order.Preview = "c:/0.png";
			return order;
		}
	}
	public class MovieDataSourceCreator {
		public static List<Movie> CreateDataSource() { return CreateDataSource(10); }
		public static List<Movie> CreateDataSource(int rowCount) {
			List<Movie> list = new List<Movie>();
			for(int i = 0; i < 1000; i++) {
				list.Add(CreateItem(i + 1));
			}
			return list;
		}
		static Movie CreateItem(int id) {
			Movie order = new Movie();
			order.ID = id;
			order.Name = "Name " + id.ToString();
			order.OrderDate = DateTime.Today.AddDays(-id % 20);
			order.Price = 100 + id % 10;
			order.Quantity = 10 + id % 15;
			order.Preview = "movies/0.wmv";
			return order;
		}
	}
	[XmlRoot("NewDataSet")]
	public class CarsData : List<Cars> {
		static XmlSerializer Serializer = new XmlSerializer(typeof(CarsData));
		public static List<Cars> NewDataSource {
			get {
				return (List<Cars>)Serializer.Deserialize(GetDataStream());
			}
		}
		public static List<Cars> DataSource {
			get {
				return NewDataSource;
			}
		}
#if !SILVERLIGHT
		public static DataView NewDataView {
			get {
				DataSet ds = new DataSet();
				ds.ReadXml(GetDataStream());
				return ds.Tables[0].DefaultView;
			}
		}
#endif
		static Stream GetDataStream() { return AssemblyHelper.GetResourceStream(typeof(CarsData).Assembly, "Data/Cars.xml", true); }
	}
	[XmlRoot("Employees")]
	public class EmployeesData : List<Employee> {
		static ImageSource imageMaleSrc;
		static ImageSource imageFemaleSrc;
		static XmlSerializer Serializer = new XmlSerializer(typeof(EmployeesData));
		public static List<Employee> NewDataSource {
			get {
				return (List<Employee>)Serializer.Deserialize(GetDataStream());
			}
		}
		public static List<Employee> DataSource {
			get {
				return NewDataSource;
			}
		}
#if !SILVERLIGHT
		public static DataView NewDataView {
			get {
				DataSet ds = new DataSet();
				ds.ReadXml(GetDataStream());
				return ds.Tables[0].DefaultView;
			}
		}
#endif
		public static Stream GetDataStream() { return AssemblyHelper.GetResourceStream(typeof(EmployeesData).Assembly, "Data/Employees.xml", true); }
		public static ImageSource ImageMale {
			get {
				if(imageMaleSrc == null)
					imageMaleSrc = ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(EmployeesData).Assembly, "Images/Person_Male.png"));
				return imageMaleSrc;
			}
		}
		public static ImageSource ImageFemale {
			get {
				if(imageFemaleSrc == null)
					imageFemaleSrc = ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(EmployeesData).Assembly, "Images/Person_Female.png"));
				return imageFemaleSrc;
			}
		}
	}
	[XmlRoot("Employees")]
	public class EmployeesWithPhotoData : List<Employee> {
		static XmlSerializer Serializer = new XmlSerializer(typeof(EmployeesWithPhotoData));
		static XmlSerializer OrdersRelationsSerializer = new XmlSerializer(typeof(List<NWindOrderToNewEmployee>));
		public static List<Employee> NewDataSource {
			get {
				return (List<Employee>)Serializer.Deserialize(GetDataStream());
			}
		}
		public static List<Employee> DataSource {
			get {
				return NewDataSource;
			}
		}
		static List<NWindOrderToNewEmployee> ordersRelations;
		public static List<NWindOrderToNewEmployee> OrdersRelations {
			get {
				if(ordersRelations == null)
					ordersRelations = (List<NWindOrderToNewEmployee>)OrdersRelationsSerializer.Deserialize(GetOrdersRelationsStream());
				return ordersRelations;
			}
		}
		static Dictionary<int, int> ordersRelationsDictionary;
		public static Dictionary<int, int> OrdersRelationsDictionary {
			get {
				if(ordersRelationsDictionary == null) {
					ordersRelationsDictionary = new Dictionary<int, int>();
					foreach(NWindOrderToNewEmployee rel in OrdersRelations)
						ordersRelationsDictionary.Add(rel.NWindOrderId, rel.EmployeeId);
				}
				return ordersRelationsDictionary;
			}
		}
#if !SILVERLIGHT
		public static DataView NewDataView {
			get {
				DataSet ds = new DataSet();
				ds.ReadXml(GetDataStream());
				return ds.Tables[0].DefaultView;
			}
		}
#endif
		public static Stream GetDataStream() { return AssemblyHelper.GetResourceStream(typeof(EmployeesWithPhotoData).Assembly, "Data/EmployeesWithPhoto.xml", true); }
		public static Stream GetOrdersRelationsStream() { return AssemblyHelper.GetResourceStream(typeof(EmployeesWithPhotoData).Assembly, "Data/NWindOrdToNewEmployee.xml", true); }
	}
	public class NWindOrderToNewEmployee {
		public int NWindOrderId { get; set; }
		public int EmployeeId { get; set; }
	}
}

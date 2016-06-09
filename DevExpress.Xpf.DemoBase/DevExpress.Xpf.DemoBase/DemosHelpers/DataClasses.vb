#If Not SILVERLIGHT Then
Imports System.Data
#End If
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Windows.Media
Imports System.Xml.Serialization
Imports DevExpress.DemoData.Helpers
Imports DevExpress.Utils

Namespace DevExpress.Xpf.DemoBase.DataClasses
    Public Class Movie
        Public Property ID() As Integer
        Public Property Name() As String
        Public Property OrderDate() As Date
        Public Property Price() As Decimal
        Public Property Quantity() As Integer
        Public ReadOnly Property Total() As Decimal
            Get
                Return Price * Quantity
            End Get
        End Property
        Public Property Preview() As String
    End Class
    Public Class Cars

        Private imageSource_Renamed As ImageSource

        Public Property ID() As Integer
        Public Property Trademark() As String
        Public Property Model() As String
        Public Property HP() As Integer
        Public Property Liter() As Double
        Public Property Cyl() As Integer
        <XmlElement("Transmiss Speed Count")>
        Public Property TransmissSpeedCount() As Integer
        <XmlElement("Transmiss Automatic")>
        Public Property TransmissAutomatic() As String
        Public Property MPGCity() As Integer
        Public Property MPGHighway() As Integer
        Public Property Category() As String
        Public Property Description() As String
        Public Property Hyperlink() As String
        Public Property Picture() As Byte()
        Public ReadOnly Property ImageSource() As ImageSource
            Get
                If imageSource_Renamed Is Nothing Then
                    imageSource_Renamed = ImageSourceHelper.GetImageSource(New MemoryStream(Picture))
                End If
                Return imageSource_Renamed
            End Get
        End Property
        Public Property Price() As Decimal
        <XmlElement("Delivery Date")>
        Public Property DeliveryDate() As Date
        <XmlElement("Is In Stock")>
        Public Property IsInStock() As Boolean
    End Class
    Public Class Order
        Public Property ID() As Integer
        Public Property Name() As String
        Public Property OrderDate() As Date
        Public Property Price() As Decimal
        Public Property Quantity() As Integer
        Public ReadOnly Property Total() As Decimal
            Get
                Return Price * Quantity
            End Get
        End Property
        Public Property Preview() As String
    End Class
    Public Class Employee
        Implements IComparable, INotifyPropertyChanged

        Public Property Id() As Integer
        Public Property ParentId() As Integer
        Public Property FirstName() As String
        Public Property MiddleName() As String
        Public Property LastName() As String
        Public Property JobTitle() As String
        Public Property Phone() As String
        Public Property EmailAddress() As String
        Public Property AddressLine1() As String
        Public Property City() As String
        Public Property StateProvinceName() As String
        Public Property PostalCode() As String
        Public Property CountryRegionName() As String
        <XmlIgnore>
        Private groupNameCore As String
        Public Property GroupName() As String
            Get
                Return groupNameCore
            End Get
            Set(ByVal value As String)
                If groupNameCore <> value Then
                    RaisePropertyChanged("GroupName")
                    groupNameCore = value
                End If
            End Set
        End Property
        Public Property BirthDate() As Date
        Public Property HireDate() As Date
        Public Property Gender() As String
        Public Property MaritalStatus() As String
        Public Property Title() As String
        Public Property ImageData() As Byte()
        Public ReadOnly Property Image() As ImageSource
            Get
                Return If(Gender = "F", EmployeesData.ImageFemale, EmployeesData.ImageMale)
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return FirstName & " " & LastName
        End Function
        #Region "Equality"
        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            Dim empl As Employee = TryCast(obj, Employee)
            If empl Is Nothing Then
                Throw New ArgumentException()
            End If
            Return String.Compare(FirstName & LastName, empl.FirstName & empl.LastName)
        End Function
        #End Region
        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Nothing Then
                Return False
            End If
            Return ToString() = obj.ToString()
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return Id
        End Function

        Private Sub RaisePropertyChanged(ByVal propertyName As String)
            If PropertyChangedEvent IsNot Nothing Then
                Dim e As New PropertyChangedEventArgs(propertyName)
                RaiseEvent PropertyChanged(Me, e)
            End If
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    End Class
    Public Class OrderDataSourceCreator
        Public Shared Function CreateDataSource() As List(Of Order)
            Return CreateDataSource(1000)
        End Function
        Public Shared Function CreateDataSource(ByVal rowCount As Integer) As List(Of Order)
            Dim list As New List(Of Order)()
            For i As Integer = 0 To rowCount - 1
                list.Add(CreateItem(i + 1))
            Next i
            Return list
        End Function
        Private Shared Function CreateItem(ByVal id As Integer) As Order
            Dim order As New Order()
            order.ID = id
            order.Name = "Name " & id.ToString()
            order.OrderDate = Date.Today.AddDays(-id Mod 20)
            order.Price = 100 + id Mod 10
            order.Quantity = 10 + id Mod 15
            order.Preview = "c:/0.png"
            Return order
        End Function
    End Class
    Public Class MovieDataSourceCreator
        Public Shared Function CreateDataSource() As List(Of Movie)
            Return CreateDataSource(10)
        End Function
        Public Shared Function CreateDataSource(ByVal rowCount As Integer) As List(Of Movie)
            Dim list As New List(Of Movie)()
            For i As Integer = 0 To 999
                list.Add(CreateItem(i + 1))
            Next i
            Return list
        End Function
        Private Shared Function CreateItem(ByVal id As Integer) As Movie
            Dim order As New Movie()
            order.ID = id
            order.Name = "Name " & id.ToString()
            order.OrderDate = Date.Today.AddDays(-id Mod 20)
            order.Price = 100 + id Mod 10
            order.Quantity = 10 + id Mod 15
            order.Preview = "movies/0.wmv"
            Return order
        End Function
    End Class
    <XmlRoot("NewDataSet")>
    Public Class CarsData
        Inherits List(Of Cars)

        Private Shared Serializer As New XmlSerializer(GetType(CarsData))
        Public Shared ReadOnly Property NewDataSource() As List(Of Cars)
            Get
                Return DirectCast(Serializer.Deserialize(GetDataStream()), List(Of Cars))
            End Get
        End Property
        Public Shared ReadOnly Property DataSource() As List(Of Cars)
            Get
                Return NewDataSource
            End Get
        End Property
#If Not SILVERLIGHT Then
        Public Shared ReadOnly Property NewDataView() As DataView
            Get
                Dim ds As New DataSet()
                ds.ReadXml(GetDataStream())
                Return ds.Tables(0).DefaultView
            End Get
        End Property
#End If
        Private Shared Function GetDataStream() As Stream
            Return AssemblyHelper.GetResourceStream(GetType(CarsData).Assembly, "Data/Cars.xml", True)
        End Function
    End Class
    <XmlRoot("Employees")>
    Public Class EmployeesData
        Inherits List(Of Employee)

        Private Shared imageMaleSrc As ImageSource
        Private Shared imageFemaleSrc As ImageSource
        Private Shared Serializer As New XmlSerializer(GetType(EmployeesData))
        Public Shared ReadOnly Property NewDataSource() As List(Of Employee)
            Get
                Return DirectCast(Serializer.Deserialize(GetDataStream()), List(Of Employee))
            End Get
        End Property
        Public Shared ReadOnly Property DataSource() As List(Of Employee)
            Get
                Return NewDataSource
            End Get
        End Property
#If Not SILVERLIGHT Then
        Public Shared ReadOnly Property NewDataView() As DataView
            Get
                Dim ds As New DataSet()
                ds.ReadXml(GetDataStream())
                Return ds.Tables(0).DefaultView
            End Get
        End Property
#End If
        Public Shared Function GetDataStream() As Stream
            Return AssemblyHelper.GetResourceStream(GetType(EmployeesData).Assembly, "Data/Employees.xml", True)
        End Function
        Public Shared ReadOnly Property ImageMale() As ImageSource
            Get
                If imageMaleSrc Is Nothing Then
                    imageMaleSrc = ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(GetType(EmployeesData).Assembly, "Images/Person_Male.png"))
                End If
                Return imageMaleSrc
            End Get
        End Property
        Public Shared ReadOnly Property ImageFemale() As ImageSource
            Get
                If imageFemaleSrc Is Nothing Then
                    imageFemaleSrc = ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(GetType(EmployeesData).Assembly, "Images/Person_Female.png"))
                End If
                Return imageFemaleSrc
            End Get
        End Property
    End Class
    <XmlRoot("Employees")>
    Public Class EmployeesWithPhotoData
        Inherits List(Of Employee)

        Private Shared Serializer As New XmlSerializer(GetType(EmployeesWithPhotoData))
        Private Shared OrdersRelationsSerializer As New XmlSerializer(GetType(List(Of NWindOrderToNewEmployee)))
        Public Shared ReadOnly Property NewDataSource() As List(Of Employee)
            Get
                Return DirectCast(Serializer.Deserialize(GetDataStream()), List(Of Employee))
            End Get
        End Property
        Public Shared ReadOnly Property DataSource() As List(Of Employee)
            Get
                Return NewDataSource
            End Get
        End Property


        Private Shared ordersRelations_Renamed As List(Of NWindOrderToNewEmployee)
        Public Shared ReadOnly Property OrdersRelations() As List(Of NWindOrderToNewEmployee)
            Get
                If ordersRelations_Renamed Is Nothing Then
                    ordersRelations_Renamed = DirectCast(OrdersRelationsSerializer.Deserialize(GetOrdersRelationsStream()), List(Of NWindOrderToNewEmployee))
                End If
                Return ordersRelations_Renamed
            End Get
        End Property


        Private Shared ordersRelationsDictionary_Renamed As Dictionary(Of Integer, Integer)
        Public Shared ReadOnly Property OrdersRelationsDictionary() As Dictionary(Of Integer, Integer)
            Get
                If ordersRelationsDictionary_Renamed Is Nothing Then
                    ordersRelationsDictionary_Renamed = New Dictionary(Of Integer, Integer)()
                    For Each rel As NWindOrderToNewEmployee In OrdersRelations
                        ordersRelationsDictionary_Renamed.Add(rel.NWindOrderId, rel.EmployeeId)
                    Next rel
                End If
                Return ordersRelationsDictionary_Renamed
            End Get
        End Property

#If Not SILVERLIGHT Then
        Public Shared ReadOnly Property NewDataView() As DataView
            Get
                Dim ds As New DataSet()
                ds.ReadXml(GetDataStream())
                Return ds.Tables(0).DefaultView
            End Get
        End Property
#End If

        Public Shared Function GetDataStream() As Stream
            Return AssemblyHelper.GetResourceStream(GetType(EmployeesWithPhotoData).Assembly, "Data/EmployeesWithPhoto.xml", True)
        End Function
        Public Shared Function GetOrdersRelationsStream() As Stream
            Return AssemblyHelper.GetResourceStream(GetType(EmployeesWithPhotoData).Assembly, "Data/NWindOrdToNewEmployee.xml", True)
        End Function
    End Class

    Public Class NWindOrderToNewEmployee
        Public Property NWindOrderId() As Integer
        Public Property EmployeeId() As Integer
    End Class
End Namespace
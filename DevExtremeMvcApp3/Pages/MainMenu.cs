﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtremeMvcApp3.Pages
{
    public static class MainMenu
    {
        public static class Customer
        {
            public const string PageName = "Khách hàng";
            public const string RoleName = "Customer";
            public const string Path = "/Customer/Index";
            public const string ControllerName = "Customer";
            public const string ActionName = "Index";
        }

        public static class Vendor
        {
            public const string PageName = "Nhà cung cấp";
            public const string RoleName = "Vendor";
            public const string Path = "/Vendor/Index";
            public const string ControllerName = "Vendor";
            public const string ActionName = "Index";
        }

        public static class Product
        {
            public const string PageName = "Vật tư";
            public const string RoleName = "Product";
            public const string Path = "/Products/Index";
            public const string ControllerName = "Product";
            public const string ActionName = "Index";
        }

        public static class PurchaseOrder
        {
            public const string PageName = "Đơn mua hàng";
            public const string RoleName = "Purchase Order";
            public const string Path = "/PurchaseOrder/Index";
            public const string ControllerName = "PurchaseOrder";
            public const string ActionName = "Index";
        }

        public static class GoodsReceivedNote
        {
            public const string PageName = "Goods Received Note";
            public const string RoleName = "Goods Received Note";
            public const string Path = "/GoodsReceivedNote/Index";
            public const string ControllerName = "GoodsReceivedNote";
            public const string ActionName = "Index";
        }

        public static class Bill
        {
            public const string PageName = "Hóa đơn";
            public const string RoleName = "Bill";
            public const string Path = "/Bill/Index";
            public const string ControllerName = "Bill";
            public const string ActionName = "Index";
        }

        public static class PaymentVoucher
        {
            public const string PageName = "Chứng từ thanh toán";
            public const string RoleName = "Payment Voucher";
            public const string Path = "/PaymentVoucher/Index";
            public const string ControllerName = "PaymentVoucher";
            public const string ActionName = "Index";
        }

        public static class SalesOrder
        {
            public const string PageName = "Đơn đặt hàng";
            public const string RoleName = "Sales Order";
            public const string Path = "/SalesOrder/Index";
            public const string ControllerName = "SalesOrder";
            public const string ActionName = "Index";
        }
           public static class SalesOrderLine
        {
            public const string PageName = "Chi tiết đơn đặt hàng";
            public const string RoleName = "Sales Order Lines";
            public const string Path = "/SalesOrderLines/Index";
            public const string ControllerName = "SalesOrderLines";
            public const string ActionName = "Index";
        }

        public static class Shipment
        {
            public const string PageName = "Lô hàng";
            public const string RoleName = "Shipment";
            public const string Path = "/Shipment/Index";
            public const string ControllerName = "Shipment";
            public const string ActionName = "Index";
        }

        public static class Invoice
        {
            public const string PageName = "Hóa đơn";
            public const string RoleName = "Invoice";
            public const string Path = "/Invoice/Index";
            public const string ControllerName = "Invoice";
            public const string ActionName = "Index";
        }

        public static class PaymentReceive
        {
            public const string PageName = "Payment Receive";
            public const string RoleName = "Payment Receive";
            public const string Path = "/PaymentReceive/Index";
            public const string ControllerName = "PaymentReceive";
            public const string ActionName = "Index";
        }

        public static class BillType
        {
            public const string PageName = "Loại hóa đơn";
            public const string RoleName = "Bill Type";
            public const string Path = "/BillType/Index";
            public const string ControllerName = "BillType";
            public const string ActionName = "Index";
        }

        public static class Branch
        {
            public const string PageName = "Chi nhánh";
            public const string RoleName = "Branch";
            public const string Path = "/Branch/Index";
            public const string ControllerName = "Branch";
            public const string ActionName = "Index";
        }

        public static class CashBank
        {
            public const string PageName = "Ngân hàng";
            public const string RoleName = "Cash Bank";
            public const string Path = "/CashBank/Index";
            public const string ControllerName = "CashBank";
            public const string ActionName = "Index";
        }

        public static class Currency
        {
            public const string PageName = "Tiền tệ";
            public const string RoleName = "Currency";
            public const string Path = "/Currency/Index";
            public const string ControllerName = "Currency";
            public const string ActionName = "Index";
        }

        public static class CustomerType
        {
            public const string PageName = "Loại khách hàng";
            public const string RoleName = "Customer Type";
            public const string Path = "/CustomerType/Index";
            public const string ControllerName = "CustomerType";
            public const string ActionName = "Index";
        }

        public static class InvoiceType
        {
            public const string PageName = "Loại hoá đơn";
            public const string RoleName = "Invoice Type";
            public const string Path = "/InvoiceType/Index";
            public const string ControllerName = "InvoiceType";
            public const string ActionName = "Index";
        }

        public static class PaymentType
        {
            public const string PageName = "Hình thức thanh toán";
            public const string RoleName = "Payment Type";
            public const string Path = "/PaymentType/Index";
            public const string ControllerName = "PaymentType";
            public const string ActionName = "Index";
        }

        public static class ProductType
        {
            public const string PageName = "Loại vật tư";
            public const string RoleName = "Product Type";
            public const string Path = "/ProductType/Index";
            public const string ControllerName = "ProductType";
            public const string ActionName = "Index";
        }

        public static class SalesType
        {
            public const string PageName = "Loại hình bán hàng";
            public const string RoleName = "Sales Type";
            public const string Path = "/SalesType/Index";
            public const string ControllerName = "SalesType";
            public const string ActionName = "Index";
        }

        public static class ShipmentType
        {
            public const string PageName = "Loại lô hàng";
            public const string RoleName = "Shipment Type";
            public const string Path = "/ShipmentType/Index";
            public const string ControllerName = "ShipmentType";
            public const string ActionName = "Index";
        }

        public static class UnitOfMeasure
        {
            public const string PageName = "Đơn vị đo lường";
            public const string RoleName = "Unit Of Measure";
            public const string Path = "/UnitOfMeasure/Index";
            public const string ControllerName = "UnitOfMeasure";
            public const string ActionName = "Index";
        }

        public static class VendorType
        {
            public const string PageName = "Loại nhà cung cấp";
            public const string RoleName = "Vendor Type";
            public const string Path = "/VendorType/Index";
            public const string ControllerName = "VendorType";
            public const string ActionName = "Index";
        }

        public static class Warehouse
        {
            public const string PageName = "Tồn kho";
            public const string RoleName = "Warehouse";
            public const string Path = "/Warehouse/Index";
            public const string ControllerName = "Warehouse";
            public const string ActionName = "Index";
        }

        public static class PurchaseType
        {
            public const string PageName = "Loại mua hàng";
            public const string RoleName = "Purchase Type";
            public const string Path = "/PurchaseType/Index";
            public const string ControllerName = "PurchaseType";
            public const string ActionName = "Index";
        }

        public static class User
        {
            public const string PageName = "Người dùng";
            public const string RoleName = "User";
            public const string Path = "/UserRole/Index";
            public const string ControllerName = "UserRole";
            public const string ActionName = "Index";
        }

        public static class ChangePassword
        {
            public const string PageName = "Đổi mật khẩu";
            public const string RoleName = "Change Password";
            public const string Path = "/UserRole/ChangePassword";
            public const string ControllerName = "UserRole";
            public const string ActionName = "ChangePassword";
        }

        public static class Role
        {
            public const string PageName = "Quyền";
            public const string RoleName = "Role";
            public const string Path = "/UserRole/Role";
            public const string ControllerName = "UserRole";
            public const string ActionName = "Role";
        }

        public static class ChangeRole
        {
            public const string PageName = "Thay đổi quyền";
            public const string RoleName = "Change Role";
            public const string Path = "/UserRole/ChangeRole";
            public const string ControllerName = "UserRole";
            public const string ActionName = "ChangeRole";
        }

        public static class Dashboard
        {
            public const string PageName = "Thống kê";
            public const string RoleName = "Dashboard Main";
            public const string Path = "/Dashboard/Index";
            public const string ControllerName = "Dashboard";
            public const string ActionName = "Index";
        }

    }
}

USE WideWorldImporters
GO
select * from Purchasing.PurchaseOrderLines
--1
SELECT ???(ExpectedUnitPricePerOuter) TotalUnitPricePerOrder FROM Purchasing.PurchaseOrderLines

/*
	Result:
	TotalUnitPricePerOrder
	748043.85
	
	Question: What Function should we use above?
*/
SELECT sum (ExpectedUnitPricePerOuter) TotalUnitPricePerOrder FROM Purchasing.PurchaseOrderLines

--2
SELECT ???() CurrentDateTime

/*
	Question: What Function should we use to get the current date and time above?
*/
SELECT CURRENT_DATE() 
--Select Current_Time()
--Select Current_TimeStamp()
Select getdate()


Select getdate() currentdatetime

--3
--SELECT CustomerID, CustomerTransactionID, CASE WHEN ???(PaymentMethodID, '') = '' THEN 9999 else PaymentMethodID END PayMethod FROM SALES.CustomerTransactions
--Select * from Sales.CustomerTransactions
/*
	Question: What Function should we use here?
*/
SELECT CustomerID, CustomerTransactionID, CASE WHEN ISNull(PaymentMethodID, '') = '' THEN 9999 else PaymentMethodID END PayMethod FROM SALES.CustomerTransactions

--Select * from Sales.Invoices

--4
/*SELECT InvoiceDate, CustomerPOrdNum, DeliveryInstructions FROM (
select InvoiceID, InvoiceDate IVDate, CustomerPurchaseOrderNumber CPOrderNumber, DeliveryInstructions from sales.Invoices
) SubQ
ORDER BY CPOrderNumber, DeliveryInstructions


SELECT InvoiceID, InvoiceDate AS IVDate, CustomerPurchaseOrderNumber AS CPOrderNumber, DeliveryInstructions FROM (
select InvoiceID, InvoiceDate AS IVDate, CustomerPurchaseOrderNumber AS CPOrderNumber, DeliveryInstructions from sales.Invoices
WHERE InvoiceDate >= '20160531'
Group by Invoice ID 
)SubQ
group BY CPOrderNumber, DeliveryInstructions
 
 SELECT IVDate, CPOrderNumber, DeliveryInstructions FROM (
select InvoiceID, InvoiceDate IVDate, CustomerPurchaseOrderNumber CPOrderNumber, DeliveryInstructions from sales.Invoices
) SubQ
WHERE IVDate >= '20160530'
ORDER BY CPOrderNumber, DeliveryInstructions


--5

/*
	How can we return the first 100 rows from the table above?
*/

 Select Top 100 * from sales.CustomerTransactions 
  
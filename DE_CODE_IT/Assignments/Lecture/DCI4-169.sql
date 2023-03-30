/*we need a list of customername,customercategoryname,and both the primary and alternate 
contact names for those individuals.

A secondary task is which individuals speak multiple languages.
*/
 
	select CustomerName, CustomercategoryName , Peeps.FullName PrimaryContact,
	Case 
	     when Pops.FullName IS NULL THEN  'N/A'  
		 Else Pops.FullName
		 END SecondContact,
     Case
	     when Peeps.Otherlanguages IS NOT NULL THEN 'Language'
		 Else 0
		 END Primarybillingual,
    Case
	     when Pops.Otherlanguages IS NOT NULL THEN 'Language'
		 Else 0
		 END Secondarybillingual
      from Sales.Customers CUST
    INNER JOIN  Sales.CustomerCategories CAT On CUST.Customercategoryid = CAT.Customercategoryid 
    Left outer JOIN [Application].People Peeps ON Peeps.PersonID = PrimaryContactPersonID
	Left Outer JOIN [Application].People Pops ON Pops.PersonID = AlternateContactPersonID
	 Where BuyingGroupId is Null
	Order By CustomerName
     
Select * from Sales.Customers 
select * from [application].people 
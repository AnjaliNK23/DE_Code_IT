USE WideWorldImporters
GO

--Anjali
SELECT * FROM Warehouse.ColdRoomTemperatures Order By ColdRoomTemperatureID

Insert Into Warehouse.ColdRoomTemperatures(ColdRoomSensorNumber,RecordedWhen,Temperature)
values(5,'20220531 10:12pm',4.75)  

/*delete from Warehouse.ColdRoomTemperatures where ColdRoomSensorNumber = 5*/
---i wrote date wrong so i deleted the row then added new row  again 
 
 /*Update Warehouse.ColdRoomTemperatures set ColdRoomTemperatureID = 3654741 
 where ColdRoomSensorNumber = 5*/
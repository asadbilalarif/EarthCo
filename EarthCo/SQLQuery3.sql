ALTER TABLE tblCustomer
ADD ContactFirstName nvarchar(500),
ContactLastName nvarchar(500),
Email nvarchar(500),
UserName nvarchar(500),
Password nvarchar(500),
ContactAddress nvarchar(500),
ContactPhone nvarchar(500),
ContactAltPhone nvarchar(500),
ContactFax nvarchar(500),
ContactNotes nvarchar(500),
isLoginAllow bit,
CustomerTypeId int,
FOREIGN KEY (CustomerTypeId) REFERENCES tblCustomerType(CustomerTypeId);


ALTER TABLE tblContact
ADD AltPhone nvarchar(500),
UserName nvarchar(500),
Password nvarchar(500),
Comments nvarchar(500),
isLoginAllow bit;



"# DoDBCrypto" 

一個使用.NET 4.8的一類別庫（.dll）

內建立有AES加解密

利用C# Function  註冊到MSsql

在MS sql 建立 純量函數做子串的加解密 HASH
*/
``` sql
語法：
--1.CLR 支援允許 SQL Server 執行由 .NET 框架編寫的代碼，這樣可以擴展 SQL Server 的功能
EXEC sp_configure 'clr enabled',1;
RECONFIGURE;
GO
--2.設置 SQL Server 數據庫屬性，以允許執行高權限的 CLR 程序碼
    ALTER DATABASE [your database] SET TRUSTWORTHY ON;
GO 
--3.備註 ： 注意事項
    --將類別庫建制後找到.dll使用檔案的絕對路徑
    --DLL內方法需要靜態static 錯誤訊息： 組件 'DoDBCrypto' 中類別 'DoDBCrypto.DBCrypto' 的方法、屬性或欄位 'AESEncrypt' 不是靜態。
    --將dll注入到MSsql
    --建立一個CLR（Common Language Runtime）
```
``` sql
Create ASSEMBLY clr_DoDBCrypto--（clr_DoDBCrypto 建立的名稱）
FROM 'path\DoDBCrypto\bin\Debug\DoDBCrypto.dll'
WITH PERMISSION_SET = UNSAFE;
GO
```
/*
``` sql
--移除注入的dll 
DROP ASSEMBLY clr_DoDBCrypto;
GO
```
``` sql
--修改CLR
ALTER ASSEMBLY clr_DoDBCrypto
FROM 'C:\path\to\your\modified\assembly.dll'
WITH PERMISSION_SET = UNSAFE;
```
``` sql
--查詢clr
SELECT  *
FROM  sys.assemblies;
*/
```
``` sql
--建立純量函數
/* 範例
-- EXTERNAL：CLR assembly 被部署在 SQL Server 中，但其實際的執行代碼在 SQL Server 的外部
CREATE FUNCTION dbo.fn_EncryptData (@input NVARCHAR(MAX), @key NVARCHAR(MAX), @iv NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME clr名稱.[namespace.className].functionName;
GO
*/
```

``` sql

CREATE FUNCTION dbo.fn_EncryptData (@input NVARCHAR(MAX), @key NVARCHAR(MAX), @iv NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME clr_DoDBCrypto.[DoDBCrypto.DBCrypto].AESEncrypt;
GO

CREATE FUNCTION dbo.fn_DecryptData (@input NVARCHAR(MAX), @key NVARCHAR(MAX), @iv NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME clr_DoDBCrypto.[DoDBCrypto.DBCrypto].AESDecrypt;
GO
```
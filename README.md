# StockManager (C# WinForms + Entity Framework)

Katmanlı mimariyle geliştirilmiş stok ve kategori yönetimi uygulaması.  
Entity Framework, Repository ve Unit of Work desenleriyle yazılmıştır.

## Özellikler
- Ürün & Kategori CRUD işlemleri  
- Soft Delete (IsDeleted alanı)
- MSSQL bağlantısı (EF Context)
- Katmanlı yapı (Entities, DataAccess, Context, UI)
- WinForms arayüzü

## Proje Yapısı
📁 StockManager
┣ 📁 Context (EF veritabanı bağlantısı)
┣ 📁 DataAccess (Repository + UnitOfWork)
┣ 📁 Entities (Model sınıfları)
┣ 📁 UI (FormProduct, FormCategory)
┗ App.config

## Kurulum
1. Visual Studio ile aç.  
2. `App.config` dosyasındaki bağlantıyı kendi MSSQL sunucuna göre düzenle.  
3. Veritabanını oluşturup projeyi çalıştır.  

## Teknolojiler
- C# (.NET Framework)
- WinForms
- Entity Framework
- MSSQL

![StockManager Screenshot](https://github.com/Kaganucar/StockManager-WinForms/blob/main/FormProduct.png)
![Category Form](https://github.com/Kaganucar/StockManager-WinForms/blob/main/FormCategory.png)

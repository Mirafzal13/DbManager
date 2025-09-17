# 📦 DbManager API

DbManager API — bu **PostgreSQL serverlariga ulanish**, **bazalardan metadata olish** va **SQL query’larni bajarish** imkonini beruvchi REST API.  
Loyiha **Clean Architecture (CQRS + MediatR)** asosida qurilgan.  

UI qismi hali mavjud emas, ammo API’ni **Swagger** yoki **Postman** orqali to‘liq test qilish mumkin.  

---

## 🚀 Boshlash

### 1. Talablar
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- PostgreSQL 15+

### 2. Loyihani ishga tushirish
```bash
git clone https://github.com/username/dbmanager-api.git
cd dbmanager-api/src/DbManager.API
dotnet run
```

API quyidagi manzilda ishga tushadi:
- `https://localhost:5001`
- `http://localhost:5000`

---

## 📖 Swagger UI
Swagger avtomatik ochiladi:  
👉 `https://localhost:5001/swagger/index.html`

---

## 🔑 API’dan foydalanish bosqichlari

Quyida API’larni ishlatishning **ketma-ketligi** ko‘rsatilgan:

---

### 1️⃣ Serverga ulanish
**Endpoint:**  
`POST /api/connections/connect`

**Body:**
```json
{
  "name": "Test Server",
  "host": "127.0.0.1",
  "port": 5432,
  "username": "postgres",
  "password": "postgres",
  "autoSave": true
}
```

**Response:**
```json
"777ad80a-6c7d-4ade-bd78-1085a0952abe"
```
👉 Bu yerda qaytgan **connectionId** keyingi API’larda ishlatiladi.

---

### 2️⃣ Faol ulanishlarni ko‘rish
**Endpoint:**  
`GET /api/connections/active`

**Response (misol):**
```json
[
  {
    "connectionId": "777ad80a-6c7d-4ade-bd78-1085a0952abe",
    "serverName": "Test Server"
  }
]
```

---

### 3️⃣ Serverdagi barcha database’larni olish
**Endpoint:**  
`GET /api/metadata/{connectionId}/databases`

**Response:**
```json
[
  "postgres",
  "TEST",
  "Template1"
]
```

---

### 4️⃣ Ma’lum bir database ichidagi schema’larni olish
**Endpoint:**  
`GET /api/metadata/{connectionId}/{database}/schemas`

**Response:**
```json
[
  "public",
  "audit"
]
```

---

### 5️⃣ Schema ichidagi table’larni olish
**Endpoint:**  
`GET /api/metadata/{connectionId}/{database}/{schema}/tables`

**Response:**
```json
[
  "Employees",
  "Departments"
]
```

---

### 6️⃣ Table columnlarini olish
**Endpoint:**  
`GET /api/metadata/{connectionId}/{database}/{schema}/{table}/columns`

**Response:**
```json
[
  { "name": "Id", "dataType": "uuid", "isNullable": false, "maxLength" : 256, "isPrimaryKey" : true},
  { "name": "FullName", "dataType": "text", "isNullable": true, "maxLength" : 256, "isPrimaryKey" : false },
  { "name": "HireDate", "dataType": "timestamp", "isNullable": false, "maxLength" : 256, "isPrimaryKey" : false }
]
```

---

### 7️⃣ SQL query bajarish
**Endpoint:**  
`POST /api/query/execute`

**Body:**
```json
{
  "connectionId": "777ad80a-6c7d-4ade-bd78-1085a0952abe",
  "database": "TEST",
  "sqlQuery": "SELECT * FROM public.\"Employees\" LIMIT 10;"
}
```

**Response (misol):**
```json
{
  "columns": ["Id", "FullName", "HireDate"],
  "rows": [
    ["3de4a102-2a24-4516-98ad-34619b86effb", "John Doe", "2021-01-01T00:00:00"],
    ["2b3e9d5a-4f2a-45bd-a12e-93cfa9129ac1", "Alice Smith", "2022-03-15T00:00:00"]
  ]
}
```

---

### 8️⃣ Query tarixini olish
**Endpoint:**  
`GET /api/query/history/{connectionId}`

**Response:**
```json
[
  {
    "id": "5b3e9d8a-4f2a-45bd-a12e-93cfa9129ac9",
    "connectionId": "8b3e9d8a-4f2a-65bd-a12e-93cfa9129ac3",
    "database": "TEST",
    "sql": "SELECT * FROM public.\"Employees\" LIMIT 10;",
    "executedAt": "2025-09-14T12:30:45Z"
  }
]
```

---

### 9️⃣ Ulanishni o‘chirish
**Endpoint:**  
`DELETE /api/connections/{connectionId}`

**Response:**  
`204 No Content`

---

## ⚡ Arxitektura

Loyiha **Clean Architecture** tamoyiliga asoslangan:

- **Domain** — asosiy biznes qoidalar
- **Application** — CQRS (MediatR) command/query handler’lar
- **Infrastructure** — EF Core va boshqa servislar
- **API** — Controller endpointlari

---

## 📝 Event Loglar

Har bir muvaffaqiyatli yoki xatolik bilan tugagan command/query uchun **SystemEventLogs** jadvaliga yozuv tushadi:

- UserId
- EventType (Success/Error)
- EventData
- DetectedAt

---
# Elasticsearch

- Veriler arasında arama yapmayı kolaylaştıran bir teknolojidir.

## Elasticsearch için docker-compose işlemi

- docker-compose.yml dosyasına, **Elasticsearch** ve **Kibana** ile ilgili container bilgileri eklenir.
- Bu container'ların birbirine dependency_on field'ı ile ve Elasticsearch URL'i ile bağlılığı sağlanır.
- Volume'lerin, yani bu iki teknolojinin depolama konumlarının doğru seçilmesi önemli
- Orchestration işleminden sonra Elasticsearch ve Kibana'ya ilgili URL'lerinden erişebiliriz.

## Elasticsearch API

### Creating Document

- Kibana Console'dan yahut herhangi bir client üzerinden istek atabiliriz.
- **PUT** keyword'ü ile istediğimiz uzantıyı da seçerek doküman oluşturabiliriz. (JSON data yazar gibi)
- Data => Document
- Column => Field
- Table => Index

```
PUT products/_doc/1
{
	"name": "iPhone 6",
	"rating": 100,
	"published": true,
	"category": "mobile phone"
	"price": {
	   "usd": 3000,
	   "eur": 2500
	}
}
```

- Bu kullanım, **Dynamic Mapping** olarak adlandırılır. Veri tipini belirtmediğimiz ve Elasticsearch'ün kendisi çıkarım yaptığı durum.
- Dönen response;

```
{
	"_index": "products", (hangi index'e kaydedildi?)
	"_id": "1", (Document ID)
	"_version": 1, (her update işleminden sonra artırılır)
	"result": "created",
	"_shards": {
	   "total": 2,
	   "successful": 1,
	   "failed": 0
	},
	"_seq_no": 0, (her bir operasyonda artar)
	"_primary_term": 1
}
```

### Retrieving Document

- **GET** isteği ile ilgili index'i ve ID'yi seçerek hedef document'ı getirebiliriz.
- _doc yerine _source kullanırsak, sadece source field'ını elde ederiz.

```
GET products/_doc/1 

=>

{
	"_index": "products",
	"_id": "1",
	"_version": 1,
	"_seq_no": 0,
	"_primary_term": 1,
	"found": true, (buraya kadar olan kısım metadata bilgileri)
	"_source": { (asıl datamızı içeren field)
		"name": "iPhone 6",
		"rating": 100,
		"published": true,
		"category": "mobile phone"
		"price": {
		   "usd": 3000,
		   "eur": 2500
		}
	}
}
```

### Retrieving Shards

- GET isteği ile hedef index'e ait shard bilgilerine erişebiliriz.

```
GET _cat/shards/products

=>

products 0 p STARTED
products 0 r UNASSIGNED
```

- Response'u değerlendirecek olursak, iki satır döndürdü. İlk satır primary, ikinci satır ise replica shard'a işaret eder